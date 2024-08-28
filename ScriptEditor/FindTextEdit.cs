// <copyright file="FindTextEdit.cs" company="Keith Martin">
// Copyright (c) Keith Martin
// Licensed under the Apache License, Version 2.0 (the "License")</copyright>

using AdvancedSharpAdbClient;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FindTextClient;
using System.Threading;
using System.Buffers.Text;
using System.Collections;
using AdvancedSharpAdbClient.Models;

namespace ScriptEditor
{
    public partial class FindTextEdit : Form
    {
        private readonly Dictionary<int, ColourDictionaryEntry> bitmaps;
        private readonly Bitmap blackBitmap;
        private readonly Bitmap whiteBitmap;
        private bool HasBeenGrayed;
        private AdbServer? server;
        private string colourString;
        private FindText.FindMode findMode;
        private bool UsePos;
        private readonly List<CellItems> savedRows;
        private int removedTop, removedBottom, removedLeft, removedRight;
        private Point lastSelected;
        private bool MultiColour;

        public string SearchText { get; private set; }
        public Rectangle SearchRectangle { get; private set; }


        public FindTextEdit()
        {
            InitializeComponent();
            SearchRectangle = new Rectangle();
            SearchText = string.Empty;
            bitmaps = new Dictionary<int, ColourDictionaryEntry>();
            blackBitmap = new Bitmap(10, 10);
            Graphics g = Graphics.FromImage(blackBitmap);
            g.Clear(Color.Black);
            whiteBitmap = new Bitmap(10, 10);
            g = Graphics.FromImage(whiteBitmap);
            g.Clear(Color.White);
            HasBeenGrayed = false;
            colourString = string.Empty;
            findMode = FindText.FindMode.greyThresholdMode;
            savedRows = new List<CellItems>();
            removedBottom = removedLeft = removedRight = removedTop = 0;
            lastSelected = Point.Empty;
            // Double buffering can make DGV slow in remote desktop
            if (!System.Windows.Forms.SystemInformation.TerminalServerSession)
            {
                Type dgvType = dgvImage.GetType();
                System.Reflection.PropertyInfo? pi = dgvType.GetProperty("DoubleBuffered",
                  System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                if (pi != null)
                    pi.SetValue(dgvImage, true, null);
            }
            MultiColour = false;
            UsePos = false;
            tcColourTabs.SelectedTab = tpGray;
        }

        private void BtnLoadPic_Click(object sender, EventArgs e)
        {
            ClearUI();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Image image = Image.FromFile(openFileDialog1.FileName);
                    LoadGrabAndGrid(ref image);
                    return;
                }
                catch (Exception)
                {
                    return;
                }
            }
            else
                return;
        }

        private void DgvImage_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            ColourGridTag? tag;
            if (dgvImage.Rows[e.RowIndex].Cells[e.ColumnIndex] != null)
            {
                DataGridViewImageCell cell = (DataGridViewImageCell)dgvImage.Rows[e.RowIndex].Cells[e.ColumnIndex];
                if (cell.Tag != null)
                {
                    tag = (ColourGridTag?)cell.Tag;
                    if (tag != null)
                    {
                        tbColour.Text = tag.ColourString;
                        tbGray.Text = tag.Gray.ToString();
                        tbRed.Text = tag.Red.ToString();
                        tbGreen.Text = tag.Green.ToString();
                        tbBlue.Text = tag.Blue.ToString();
                        lastSelected.X = e.ColumnIndex;
                        lastSelected.Y = e.RowIndex;

                        if (cbModify.Checked)
                        {
                            if (HasBeenGrayed)
                            {
                                if (tag.Black)
                                    cell.Value = whiteBitmap;
                                else
                                    cell.Value = blackBitmap;
                                tag.Black = !tag.Black;
                            }
                        }
                        if (MultiColour)
                        {
                            string cellColour = string.Format("{0}/{1}/{2}", e.ColumnIndex, e.RowIndex, tag.ColourString[2..]);
                            if (tbOutputText.Text.Length > 0)
                            {
                                tbOutputText.Text = string.Format("{0},{1}", tbOutputText.Text, cellColour);
                            }
                            else
                            {
                                tbOutputText.Text = string.Format("|<{0}>##{1}${2}", tbComment.Text, nudRGB.Value,cellColour);
                            }
                        }
                    }
                }
            }
        }

        public static void Populate<T>(T[] array, int startIndex, int count, T value)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if ((uint)startIndex >= array.Length)
            {
                throw new ArgumentOutOfRangeException("startIndex", "");
            }
            if (count < 0 || ((uint)(startIndex + count) > array.Length))
            {
                throw new ArgumentOutOfRangeException("count", "");
            }
            const int Gap = 32;
            int i = startIndex;

            if (count <= Gap * 2)
            {
                while (count > 0)
                {
                    array[i] = value;
                    count--;
                    i++;
                }
                return;
            }
            int aval = Gap;
            count -= Gap;

            do
            {
                array[i] = value;
                i++;
                --aval;
            } while (aval > 0);

            aval = Gap;
            while (true)
            {
                Array.Copy(array, startIndex, array, i, aval);
                i += aval;
                count -= aval;
                aval *= 2;
                if (count <= aval)
                {
                    Array.Copy(array, startIndex, array, i, count);
                    break;
                }
            }
        }

        private void BtnGray2Two_Click(object sender, EventArgs e)
        {
            dgvImage.SuspendLayout();
            if (!HasBeenGrayed)
            {
                HasBeenGrayed = true;
            }
            int Threshold;
            if (tbGrayThreshold.Text == string.Empty)
            {
                int[] pp = new int[256];
                Populate(pp, 0, 256, 0);
                // nW = width of the on screen grid
                // nH = height of the on screen grid
                //Loop, % nW*nH
                //  if (show[A_Index])
                //    pp[gray[A_Index]]++

                for (int i = 0; i < dgvImage.Columns.Count; i++)
                    for (int j = 0; j < dgvImage.Rows.Count; j++)
                    {
                        ColourGridTag? tag = (ColourGridTag)dgvImage.Rows[j].Cells[i].Tag;
                        if (tag != null)
                            pp[tag.Gray]++;
                    }
                int IS = 0, IP = 0;
                for (int i = 0; i < 255; i++)
                {
                    IP += i * pp[i];
                    IS += pp[i];
                }
                Threshold = IP / IS;
                for (int i = 0; i < 20; i++)
                {
                    int lastThreshold = Threshold;
                    int IP1 = 0, IS1 = 0;
                    for (int k = 0; k < lastThreshold + 1; k++)
                    {
                        IP1 += k * pp[k];
                        IS1 += pp[k];
                    }
                    int IP2 = IP - IP1, IS2 = IS - IS1;
                    if (IS1 != 0 && IS2 != 0)
                        Threshold = (IP1 / IS1 + IP2 / IS2) / 2;
                    if (Threshold == lastThreshold)
                        break;
                }
                tbGrayThreshold.Text = Threshold.ToString();
            }
            Threshold = int.Parse(tbGrayThreshold.Text);
            colourString = "*" + Threshold.ToString();
            findMode = FindText.FindMode.greyThresholdMode;
            for (int i = 0; i < dgvImage.Columns.Count; i++)
                for (int j = 0; j < dgvImage.Rows.Count; j++)
                {
                    ColourGridTag? tag = (ColourGridTag) dgvImage.Rows[j].Cells[i].Tag;
                    if (tag != null)
                    {
                        if (tag.Gray <= Threshold)
                        {
                            tag.Black = true;
                            dgvImage.Rows[j].Cells[i].Value = blackBitmap;
                        }
                        else
                        {
                            tag.Black = false;
                            dgvImage.Rows[j].Cells[i].Value = whiteBitmap;
                        }
                    }
                }
            dgvImage.ResumeLayout();
        }

        private void BtnGrayDiff2Two_Click(object sender, EventArgs e)
        {
            int GrayDiff = (int)tbGrayDifference.Value;
            colourString = string.Format("**{0}", GrayDiff);

            findMode = FindText.FindMode.greyDifferenceMode;
            dgvImage.SuspendLayout();
            for (int i = 0; i < dgvImage.Columns.Count; i++)
                for (int j = 0; j < dgvImage.Rows.Count; j++)
                {
                    ColourGridTag? tag = (ColourGridTag)dgvImage.Rows[j].Cells[i].Tag;
                    if (tag != null)
                    {
                        if (tag.Gray <= GrayDiff)
                        {
                            tag.Black = true;
                            dgvImage.Rows[j].Cells[i].Value = blackBitmap;
                        }
                        else
                        {
                            tag.Black = false;
                            dgvImage.Rows[j].Cells[i].Value = whiteBitmap;
                        }
                    }
                }


            dgvImage.ResumeLayout();
            if (!HasBeenGrayed)
            {
                HasBeenGrayed = true;
            }
        }

        private void BtnColour2Two_Click(object sender, EventArgs e)
        {
            //GuiControlGet, c,, SelColor
            //if (c="")
            //{
            //  Gui, +OwnDialogs
            //  MsgBox, 4096, Tip, % Lang["12"] " !", 1
            //  return
            //}
            if (tbColour.Text == "")
            {
                MessageBox.Show("Select a colour square on the grid to select the colour to be similar to.", "Select Colour", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //UsePos:=(cmd="ColorPos2Two") ? 1:0
            //GuiControlGet, n,, Similar1
            //n:=Round(n/100,2), color:=c "@" n
            string colourSelected = tbColour.Text;
            decimal colourSimilarity = Math.Round(tbColourSimilarity.Value / 100.0m, 2);
            colourString = string.Format("{0}@{1}", colourSelected, colourSimilarity);
            //, n:=Floor(512*9*255*255*(1-n)*(1-n)), k:=i:=0
            colourSimilarity = Math.Floor(512m * 9m * 255m * 255m * (1m - colourSimilarity) * (1m - colourSimilarity));
            uint colour = Convert.ToUInt32(tbColour.Text[2..], 16);
            //, rr:=(c>>16)&0xFF, gg:=(c>>8)&0xFF, bb:=c&0xFF
            int rr = (int)((colour >> 16) & 0xFF);
            int gg = (int)((colour >> 8) & 0xFF);
            int bb = (int)(colour & 0xFF);
            //Loop, % nW*nH
            //{
            //  c:=cors[++k], r:=((c>>16)&0xFF)-rr
            //  , g:=((c>>8)&0xFF)-gg, b:=(c&0xFF)-bb, j:=r+rr+rr
            //  , ascii[k]:=v:=((1024+j)*r*r+2048*g*g+(1534-j)*b*b<=n)
            //  if (show[k])
            //    i:=(v?i+1:i-1), c:=(v?"Black":"White"), %Gui_%("SetColor")
            //}
            //bg:=i>0 ? "1":"0"


            findMode = FindText.FindMode.colourMode;
            UsePos = false;
            dgvImage.SuspendLayout();
            for (int i = 0; i < dgvImage.Columns.Count; i++)
                for (int j = 0; j < dgvImage.Rows.Count; j++)
                {
                    ColourGridTag? tag = (ColourGridTag)dgvImage.Rows[j].Cells[i].Tag;
                    if (tag != null)
                    {
                        colour = Convert.ToUInt32(tag.ColourString[2..], 16);
                        int r = (int)tag.Red - rr;
                        int g = (int)tag.Green - gg;
                        int b = (int)tag.Blue - bb;
                        int jx = (int)r + (int)rr + (int)rr;

                        if ((1024 + jx) * r * r + 2048 * g * g + (1534 - jx) * b * b <= colourSimilarity)
                        {
                            tag.Black = true;
                            dgvImage.Rows[j].Cells[i].Value = blackBitmap;
                        }
                        else
                        {
                            tag.Black = false;
                            dgvImage.Rows[j].Cells[i].Value = whiteBitmap;
                        }
                    }
                }


            dgvImage.ResumeLayout();
            if (!HasBeenGrayed)
            {
                HasBeenGrayed = true;
            }
        }

        private void BtnColourPos2Two_Click(object sender, EventArgs e)
        {
            if (tbColour.Text == "")
            {
                MessageBox.Show("Select a colour square on the grid to select the colour to be similar to.", "Select Colour", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string colourSelected = tbColour.Text;
            decimal colourSimilarity = Math.Round(tbColourPosSimilarity.Value / 100.0m, 2);
            colourString = string.Format("{0}@{1}", colourSelected, colourSimilarity);
            colourSimilarity = Math.Floor(512m * 9m * 255m * 255m * (1m - colourSimilarity) * (1m - colourSimilarity));
            uint colour = Convert.ToUInt32(tbColour.Text[2..], 16);
            int rr = (int)((colour >> 16) & 0xFF);
            int gg = (int)((colour >> 8) & 0xFF);
            int bb = (int)(colour & 0xFF);

            findMode = FindText.FindMode.colourMode;
            UsePos = true;
            dgvImage.SuspendLayout();
            for (int i = 0; i < dgvImage.Columns.Count; i++)
                for (int j = 0; j < dgvImage.Rows.Count; j++)
                {
                    ColourGridTag? tag = (ColourGridTag)dgvImage.Rows[j].Cells[i].Tag;
                    if (tag != null)
                    {
                        colour = Convert.ToUInt32(tag.ColourString[2..], 16);
                        int r = (int)tag.Red - rr;
                        int g = (int)tag.Green - gg;
                        int b = (int)tag.Blue - bb;
                        int jx = (int)r + (int)rr + (int)rr;

                        if ((1024 + jx) * r * r + 2048 * g * g + (1534 - jx) * b * b <= colourSimilarity)
                        {
                            tag.Black = true;
                            dgvImage.Rows[j].Cells[i].Value = blackBitmap;
                        }
                        else
                        {
                            tag.Black = false;
                            dgvImage.Rows[j].Cells[i].Value = whiteBitmap;
                        }
                    }
                }


            dgvImage.ResumeLayout();
            if (!HasBeenGrayed)
            {
                HasBeenGrayed = true;
            }
        }

        private void BtnColourDiff2Two_Click(object sender, EventArgs e)
        {
            if (tbColour.Text == "")
            {
                MessageBox.Show("Select a core colour square on the grid to select the colour to be similar to.", "Select Colour", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            uint colour = Convert.ToUInt32(tbColour.Text[2..], 16);
            int rr = (int)((colour >> 16) & 0xFF);
            int gg = (int)((colour >> 8) & 0xFF);
            int bb = (int)(colour & 0xFF);
            uint newColour = (uint)nudRed.Value << 16 | (uint)nudGreen.Value << 8 | (uint)nudBlue.Value;
            colourString = string.Format("{0}-{1:X6}", tbColour.Text[2..], newColour);

            findMode = FindText.FindMode.greyDifferenceMode;
            dgvImage.SuspendLayout();
            for (int i = 0; i < dgvImage.Columns.Count; i++)
                for (int j = 0; j < dgvImage.Rows.Count; j++)
                {
                    ColourGridTag? tag = (ColourGridTag)dgvImage.Rows[j].Cells[i].Tag;
                    if (tag != null)
                    {
                        colour = Convert.ToUInt32(tag.ColourString[2..], 16);
                        int r = (int)tag.Red;
                        int g = (int)tag.Green;
                        int b = (int)tag.Blue;

                        if (Math.Abs(r - rr) < nudRed.Value && Math.Abs(g - gg) < nudGreen.Value && Math.Abs(b - bb) < nudBlue.Value)
                        {
                            tag.Black = true;
                            dgvImage.Rows[j].Cells[i].Value = blackBitmap;
                        }
                        else
                        {
                            tag.Black = false;
                            dgvImage.Rows[j].Cells[i].Value = whiteBitmap;
                        }
                    }
                }


            dgvImage.ResumeLayout();
            if (!HasBeenGrayed)
            {
                HasBeenGrayed = true;
            }

        }


        private void BtnReset_Click(object sender, EventArgs e)
        {
            if (savedRows.Count > 0)
            {
                ClearUI();
                dgvImage.SuspendLayout();
                dgvImage.Columns.Clear();
                int colourARGB = Color.Gray.ToArgb();
                for (int i = 0; i < savedRows[0].Cells.Count; i++)
                {
                    DataGridViewImageColumn dgvColumn = new DataGridViewImageColumn
                    {
                        Image = bitmaps[colourARGB].Bitmap,
                        Width = 10
                    };
                    dgvImage.Columns.Add(dgvColumn);
                }
                dgvImage.Rows.Add(savedRows.Count);

                for (int i = 0; i < dgvImage.RowCount; i++)
                {
                    for (int j = 0; j < dgvImage.ColumnCount; j++)
                    {
                        colourARGB = savedRows[i].Cells[j];
                        DataGridViewImageCell? cell = (DataGridViewImageCell)dgvImage.Rows[i].Cells[j];
                        if (cell != null)
                        {
                            cell.Value = bitmaps[colourARGB].Bitmap;
                            cell.Tag = bitmaps[colourARGB].Tag.ShallowCopy();
                            cell.ToolTipText = bitmaps[colourARGB].Tag.ColourString;
                        }
                    }
                }
                dgvImage.ResumeLayout();
            }
        }

        private void BtnCropTop_Click(object sender, EventArgs e)
        {
            if (dgvImage.Rows.Count > 0)
            {
                dgvImage.SuspendLayout();
                dgvImage.Rows.RemoveAt(0);
                dgvImage.ResumeLayout();
                removedTop++;
            }
        }

        private void BtnCropTop3_Click(object sender, EventArgs e)
        {

            if (dgvImage.Rows.Count > 3)
            {
                dgvImage.SuspendLayout();
                dgvImage.Rows.RemoveAt(0);
                dgvImage.Rows.RemoveAt(0);
                dgvImage.Rows.RemoveAt(0);
                dgvImage.ResumeLayout();
                removedTop += 3;
            }
        }

        private void BtnCropBottom_Click(object sender, EventArgs e)
        {
            if (dgvImage.Rows.Count > 1)
            {
                dgvImage.SuspendLayout();
                int maxCounter = dgvImage.Rows.Count - 1;
                dgvImage.Rows.RemoveAt(maxCounter);
                dgvImage.ResumeLayout();
                removedBottom++;
            }
        }

        private void BtnCropBottom3_Click(object sender, EventArgs e)
        {
            if (dgvImage.Rows.Count > 3)
            {
                dgvImage.SuspendLayout();
                int maxCounter = dgvImage.Rows.Count - 3;
                dgvImage.Rows.RemoveAt(maxCounter);
                dgvImage.Rows.RemoveAt(maxCounter);
                dgvImage.Rows.RemoveAt(maxCounter);
                dgvImage.ResumeLayout();
                removedBottom += 3;
            }
        }

        private void BtnCropRight_Click(object sender, EventArgs e)
        {
            if (dgvImage.Columns.Count > 1)
            {
                dgvImage.SuspendLayout();
                int maxCounter = dgvImage.Columns.Count - 1;
                dgvImage.Columns.RemoveAt(maxCounter);
                dgvImage.ResumeLayout();
                removedRight++;
            }
        }

        private void BtnCropRight3_Click(object sender, EventArgs e)
        {
            if (dgvImage.Columns.Count > 3)
            {
                dgvImage.SuspendLayout();
                int maxCounter = dgvImage.Columns.Count - 3;
                dgvImage.Columns.RemoveAt(maxCounter);
                dgvImage.Columns.RemoveAt(maxCounter);
                dgvImage.Columns.RemoveAt(maxCounter);
                dgvImage.ResumeLayout();
                removedRight += 3;
            }
        }

        private void BtnCropLeft_Click(object sender, EventArgs e)
        {
            if (dgvImage.Columns.Count > 1)
            {
                dgvImage.SuspendLayout();
                dgvImage.Columns.RemoveAt(0);
                dgvImage.ResumeLayout();
                removedLeft++;
            }
        }

        private void BtnCropLeft3_Click(object sender, EventArgs e)
        {
            if (dgvImage.Columns.Count > 3)
            {
                dgvImage.SuspendLayout();
                dgvImage.Columns.RemoveAt(0);
                dgvImage.Columns.RemoveAt(0);
                dgvImage.Columns.RemoveAt(0);
                dgvImage.ResumeLayout();
                removedLeft += 3;
            }
        }

        private void BtnCropTopNegative_Click(object sender, EventArgs e)
        {
            if (removedTop > 0)
            {
                removedTop--;
                dgvImage.Rows.Insert(0, 1);
                dgvImage.SuspendLayout();

                for (int j = 0; j < dgvImage.ColumnCount; j++)
                {
                    int colourARGB = savedRows[removedTop].Cells[j + removedLeft];
                    DataGridViewImageCell cell = (DataGridViewImageCell)dgvImage.Rows[0].Cells[j];
                    if (cell != null)
                    {
                        cell.Value = bitmaps[colourARGB].Bitmap;
                        cell.Tag = bitmaps[colourARGB].Tag.ShallowCopy();
                        cell.ToolTipText = bitmaps[colourARGB].Tag.ColourString;
                    }
                }
                dgvImage.ResumeLayout();
            }
        }

        private void BtnCropBottomNegative_Click(object sender, EventArgs e)
        {
            if (removedBottom > 0)
            {
                removedBottom--;

                int currentPos = dgvImage.Rows.Add();
                dgvImage.SuspendLayout();

                for (int j = 0; j < dgvImage.ColumnCount; j++)
                {
                    int colourARGB = savedRows[removedTop + currentPos].Cells[j + removedLeft];
                    DataGridViewImageCell cell = (DataGridViewImageCell)dgvImage.Rows[currentPos].Cells[j];
                    if (cell != null)
                    {
                        cell.Value = bitmaps[colourARGB].Bitmap;
                        cell.Tag = bitmaps[colourARGB].Tag.ShallowCopy();
                        cell.ToolTipText = bitmaps[colourARGB].Tag.ColourString;
                    }
                }
                dgvImage.ResumeLayout();
            }
        }

        private void BtnCropLeftNegative_Click(object sender, EventArgs e)
        {
            if (removedLeft > 0)
            {
                removedLeft--;

                DataGridViewImageColumn dgvColumn = new DataGridViewImageColumn
                {
                    Width = 10
                };
                dgvImage.Columns.Insert(0, dgvColumn);
                dgvImage.SuspendLayout();
                for (int i = 0; i < dgvImage.RowCount; i++)
                {
                    int colourARGB = savedRows[i + removedTop].Cells[removedLeft];
                    DataGridViewImageCell cell = (DataGridViewImageCell)dgvImage.Rows[i].Cells[0];
                    if (cell != null)
                    {
                        cell.Value = bitmaps[colourARGB].Bitmap;
                        cell.Tag = bitmaps[colourARGB].Tag.ShallowCopy();
                        cell.ToolTipText = bitmaps[colourARGB].Tag.ColourString;
                    }
                }
                dgvImage.ResumeLayout();
            }
        }

        private void BtnCropRightNegative_Click(object sender, EventArgs e)
        {
            if (removedRight > 0)
            {
                removedRight--;

                DataGridViewImageColumn dgvColumn = new DataGridViewImageColumn
                {
                    Width = 10
                };
                int currentPos = dgvImage.Columns.Add(dgvColumn);
                dgvImage.SuspendLayout();
                for (int i = 0; i < dgvImage.RowCount; i++)
                {
                    int colourARGB = savedRows[i + removedTop].Cells[removedLeft + currentPos];
                    DataGridViewImageCell cell = (DataGridViewImageCell)dgvImage.Rows[i].Cells[currentPos];
                    if (cell != null)
                    {
                        cell.Value = bitmaps[colourARGB].Bitmap;
                        cell.Tag = bitmaps[colourARGB].Tag.ShallowCopy();
                        cell.ToolTipText = bitmaps[colourARGB].Tag.ColourString;
                    }
                }
                dgvImage.ResumeLayout();
            }
        }

        private void TbOutputText_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.A) && Control.ModifierKeys == Keys.Control)
            {
                tbOutputText.SelectAll();
            }
        }

        /// <summary>
        /// Receives and Image, and uses GrabViewer to display that image.
        /// With the returned rectangle from GrabViewer, the dgvImage is 
        /// populated.
        /// </summary>
        /// <param name="image">The image that is to be used to allow selecting a rectangle within for display</param>
        /// <returns></returns>
        private bool LoadGrabAndGrid(ref Image image)
        {
            GrabViewer grabViewer = new GrabViewer();
            grabViewer.SetImage(image);
            if (grabViewer.ShowDialog() == DialogResult.OK)
            {
                SearchRectangle = grabViewer.GetCaptureBox();
                dgvImage.SuspendLayout();
                dgvImage.Columns.Clear();
                savedRows.Clear();
                Rectangle grabArea = grabViewer.GetCaptureBox();
                Bitmap loadedBitmap = new Bitmap(image);

                int colourARGB = Color.Gray.ToArgb();
                if (!bitmaps.ContainsKey(colourARGB))
                {
                    Bitmap bitmap = new Bitmap(10, 10);
                    Graphics g = Graphics.FromImage(bitmap);
                    g.Clear(Color.Gray);
                    int gray = (((colourARGB >> 16) & 0xFF) * 38 + ((colourARGB >> 8) & 0xFF) * 75 + (colourARGB & 0xFF) * 15) >> 7;
                    ColourGridTag tag = new ColourGridTag(colourARGB, Color.Gray.R, Color.Gray.G, Color.Gray.B, gray, string.Format("0x{0:X6}", (colourARGB & 0x00ffffff)), false);
                    bitmaps.Add(colourARGB, new ColourDictionaryEntry(bitmap, tag));
                }

                for (int i = 0; i < grabArea.Width; i++)
                {
                    DataGridViewImageColumn dgvColumn = new DataGridViewImageColumn
                    {
                        //Image = bitmaps[colourARGB].Bitmap,
                        Width = 10
                    };
                    dgvImage.Columns.Add(dgvColumn);
                }
                dgvImage.Rows.Add(grabArea.Height);

                // Create the save space
                for (int i = 0; i < grabArea.Height; i++)
                {
                    CellItems cells = new CellItems();
                    for (int j = 0; j < grabArea.Width; j++)
                    {
                        cells.Cells.Add(0);
                    }
                    savedRows.Add(cells);
                }
                
                int x = 0;
                for (int i = grabArea.X; i < grabArea.X + grabArea.Width; i++)
                {
                    int y = 0;
                    for (int j = grabArea.Y; j < grabArea.Y + grabArea.Height; j++)
                    {
                        colourARGB = loadedBitmap.GetPixel(i, j).ToArgb();
                        if (!bitmaps.ContainsKey(colourARGB))
                        {
                            Bitmap bitmap = new Bitmap(10, 10);
                            Graphics g = Graphics.FromImage(bitmap);
                            Color colour = Color.FromArgb(colourARGB);
                            g.Clear(colour);
                            //g.Clear(Color.Silver);
                            int gray = (((colourARGB >> 16) & 0xFF) * 38 + ((colourARGB >> 8) & 0xFF) * 75 + (colourARGB & 0xFF) * 15) >> 7;
                            ColourGridTag tag = new ColourGridTag(colourARGB, colour.R, colour.G, colour.B, gray, string.Format("0x{0:X6}", (colourARGB & 0x00ffffff)), false);
                            bitmaps.Add(colourARGB, new ColourDictionaryEntry(bitmap, tag));
                        }
                        (dgvImage.Rows[y].Cells[x] as DataGridViewImageCell).Value = bitmaps[colourARGB].Bitmap;
                        (dgvImage.Rows[y].Cells[x] as DataGridViewImageCell).Tag = bitmaps[colourARGB].Tag.ShallowCopy();
                        (dgvImage.Rows[y].Cells[x] as DataGridViewImageCell).ToolTipText = bitmaps[colourARGB].Tag.ColourString;
                        savedRows[y].Cells[x] = colourARGB;
                        y++;
                    }
                    x++;
                }
                dgvImage.ResumeLayout();
                return true;
            }
            return false;
        }

        private void BtnLoadText_Click(object sender, EventArgs e)
        {
            string v = tbOutputText.Text;
            Regex regexComment = new Regex(@"<([^>]*)>");
            Regex regexSquareBrackets = new Regex(@"\[([^\]]*)]");
            Regex regexColourClean = new Regex(@"[!*#\s]");

            string comment = string.Empty;

            if (regexComment.IsMatch(v))
            {
                MatchCollection matches = regexComment.Matches(v);
                foreach (Match? match in matches)
                {
                    if (match != null)
                    {
                        comment = match.Groups[1].Value.Trim();
                        v = v.Replace(match.Value, "");
                    }
                }
            }

            if (regexSquareBrackets.IsMatch(v))
            {
                MatchCollection matches = regexSquareBrackets.Matches(v);
                if (matches.Count == 1)
                {
                    v = v.Replace(matches[0].Value, "");
                }
                else
                {
                    v = v.Replace(matches[0].Value + "," + matches[1].Value, "");
                }
            }

            string colourString = v.Split("$")[0];
            v = v[(v.IndexOf("$") + 1)..].Trim();

            FindText.FindMode mode = colourString.Contains("!!") ? FindText.FindMode.imageMode
                        : colourString.Contains("##") ? FindText.FindMode.multiColourMode
                        : colourString.Contains("-") ? FindText.FindMode.colourDifferenceMode
                        : colourString.Contains("#") ? FindText.FindMode.colourPositionMode
                        : colourString.Contains("**") ? FindText.FindMode.greyDifferenceMode
                        : colourString.Contains("*") ? FindText.FindMode.greyThresholdMode
                        : FindText.FindMode.colourMode;


            colourString = regexColourClean.Replace(colourString, "");

            //PictureInfo picInfo = FindText.PicInfo(textToLoad);
            //ToDo: Add more colour modes to btnLoadText_Click
            if (mode == FindText.FindMode.greyThresholdMode)
            {
                string[] rv = v.Split(".");
                int w = int.Parse(rv[0]);
                v = FindText.Base64tobit(rv[1]);
                int h = v.Length / w;

                ClearUI();
                dgvImage.SuspendLayout();
                dgvImage.Columns.Clear();
                savedRows.Clear();
                int colourARGB = Color.Gray.ToArgb();
                if (!bitmaps.ContainsKey(colourARGB))
                {
                    Bitmap bitmap = new Bitmap(10, 10);
                    Graphics g = Graphics.FromImage(bitmap);
                    g.Clear(Color.Gray);
                    int gray = (((colourARGB >> 16) & 0xFF) * 38 + ((colourARGB >> 8) & 0xFF) * 75 + (colourARGB & 0xFF) * 15) >> 7;
                    ColourGridTag tag = new ColourGridTag(colourARGB, Color.Gray.R, Color.Gray.G, Color.Gray.B, gray, string.Format("0x{0:X6}", (colourARGB & 0x00ffffff)), false);
                    bitmaps.Add(colourARGB, new ColourDictionaryEntry(bitmap, tag));
                }

                for (int i = 0; i < w; i++)
                {
                    DataGridViewImageColumn dgvColumn = new DataGridViewImageColumn
                    {
                        Width = 10
                    };
                    dgvImage.Columns.Add(dgvColumn);
                }
                dgvImage.Rows.Add(h);

                // Create the save space
                for (int i = 0; i < h; i++)
                {
                    CellItems cells = new CellItems();
                    for (int j = 0; j < w; j++)
                    {
                        cells.Cells.Add(0);
                    }
                    savedRows.Add(cells);
                }

                int x = 0; int y = 0; int counter = 0;
                for (int i = 0; i < h; i++)
                {
                    x = 0;
                    for (int j = 0; j < w; j++)
                    {
                        if (v[counter++] == '0')
                            colourARGB = Color.White.ToArgb();
                        else
                            colourARGB = Color.Black.ToArgb();
                        if (!bitmaps.ContainsKey(colourARGB))
                        {
                            Bitmap bitmap = new Bitmap(10, 10);
                            Graphics g = Graphics.FromImage(bitmap);
                            Color colour = Color.FromArgb(colourARGB);
                            g.Clear(colour);
                            //g.Clear(Color.Silver);
                            int gray = (((colourARGB >> 16) & 0xFF) * 38 + ((colourARGB >> 8) & 0xFF) * 75 + (colourARGB & 0xFF) * 15) >> 7;
                            ColourGridTag tag = new ColourGridTag(colourARGB, colour.R, colour.G, colour.B, gray, string.Format("0x{0:X6}", (colourARGB & 0x00ffffff)), false);
                            bitmaps.Add(colourARGB, new ColourDictionaryEntry(bitmap, tag));
                        }
                        (dgvImage.Rows[y].Cells[x] as DataGridViewImageCell).Value = bitmaps[colourARGB].Bitmap;
                        (dgvImage.Rows[y].Cells[x] as DataGridViewImageCell).Tag = bitmaps[colourARGB].Tag.ShallowCopy();
                        (dgvImage.Rows[y].Cells[x] as DataGridViewImageCell).ToolTipText = bitmaps[colourARGB].Tag.ColourString;
                        (dgvImage.Rows[i].Cells[j].Tag as ColourGridTag).Black = colourARGB == Color.Black.ToArgb();
                        savedRows[y].Cells[x] = colourARGB;
                        x++;
                    }
                    y++;
                }
                dgvImage.ResumeLayout();
                tbComment.Text = comment;
                tbGrayThreshold.Text = colourString.Replace("|","");
                HasBeenGrayed = true;
            }
            else if (mode == FindText.FindMode.imageMode)
            {
                string[] rv = v.Split(".");
                int w = int.Parse(rv[0]);
                int h = int.Parse(rv[1]);
                v = rv[2];
                byte[] byteArray = Convert.FromBase64String(v);
                uint[][] colourArray = new uint[h][];
                for (int i = 0; i < h; i++)
                {
                    colourArray[i] = new uint[w];
                    Buffer.BlockCopy(byteArray, i * w * 4, colourArray[i], 0, w * 4);
                }

                ClearUI();
                dgvImage.SuspendLayout();
                dgvImage.Columns.Clear();
                savedRows.Clear();
                int colourARGB = Color.Gray.ToArgb();
                if (!bitmaps.ContainsKey(colourARGB))
                {
                    Bitmap bitmap = new Bitmap(10, 10);
                    Graphics g = Graphics.FromImage(bitmap);
                    g.Clear(Color.Gray);
                    int gray = (((colourARGB >> 16) & 0xFF) * 38 + ((colourARGB >> 8) & 0xFF) * 75 + (colourARGB & 0xFF) * 15) >> 7;
                    ColourGridTag tag = new ColourGridTag(colourARGB, Color.Gray.R, Color.Gray.G, Color.Gray.B, gray, string.Format("0x{0:X6}", (colourARGB & 0x00ffffff)), false);
                    bitmaps.Add(colourARGB, new ColourDictionaryEntry(bitmap, tag));
                }

                for (int i = 0; i < w; i++)
                {
                    DataGridViewImageColumn dgvColumn = new DataGridViewImageColumn {
                        Width = 10
                    };
                    dgvImage.Columns.Add(dgvColumn);
                }
                dgvImage.Rows.Add(h);

                // Create the save space
                for (int i = 0; i < h; i++)
                {
                    CellItems cells = new CellItems();
                    for (int j = 0; j < w; j++)
                    {
                        cells.Cells.Add(0);
                    }
                    savedRows.Add(cells);
                }

                int x = 0; int y = 0; int counter = 0;
                for (int i = 0; i < h; i++)
                {
                    x = 0;
                    for (int j = 0; j < w; j++)
                    {
                        colourARGB = (int)colourArray[i][j];
                        if (!bitmaps.ContainsKey(colourARGB))
                        {
                            Bitmap bitmap = new Bitmap(10, 10);
                            Graphics g = Graphics.FromImage(bitmap);
                            Color colour = Color.FromArgb(colourARGB);
                            g.Clear(colour);
                            //g.Clear(Color.Silver);
                            int gray = (((colourARGB >> 16) & 0xFF) * 38 + ((colourARGB >> 8) & 0xFF) * 75 + (colourARGB & 0xFF) * 15) >> 7;
                            ColourGridTag tag = new ColourGridTag(colourARGB, colour.R, colour.G, colour.B, gray, string.Format("0x{0:X6}", (colourARGB & 0x00ffffff)), false);
                            bitmaps.Add(colourARGB, new ColourDictionaryEntry(bitmap, tag));
                        }
                        (dgvImage.Rows[y].Cells[x] as DataGridViewImageCell).Value = bitmaps[colourARGB].Bitmap;
                        (dgvImage.Rows[y].Cells[x] as DataGridViewImageCell).Tag = bitmaps[colourARGB].Tag.ShallowCopy();
                        (dgvImage.Rows[y].Cells[x] as DataGridViewImageCell).ToolTipText = bitmaps[colourARGB].Tag.ColourString;
                        (dgvImage.Rows[i].Cells[j].Tag as ColourGridTag).Black = colourARGB == Color.Black.ToArgb();
                        savedRows[y].Cells[x] = colourARGB;
                        x++;
                    }
                    y++;
                }
                dgvImage.ResumeLayout();
                tbComment.Text = comment;
                HasBeenGrayed = false;
            }
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void TcColourTabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tcColourTabs.SelectedTab != tpMultiColour)
            {
                cbFindMultiColour.Checked = false;
            }
            if (tcColourTabs.SelectedTab == tpImage)
            {
                findMode = FindText.FindMode.imageMode;
            }
        }

        private void CbFindMultiColour_CheckedChanged(object sender, EventArgs e)
        {
            MultiColour = cbFindMultiColour.Checked;
            if (MultiColour)
            {
                tbOutputText.Text = String.Empty;
            }
        }

        private void BtnUndo_Click(object sender, EventArgs e)
        {
            if (MultiColour)
            {
                if (tbOutputText.TextLength > 0)
                {
                    string[] results = tbOutputText.Text.Split(",");
                    tbOutputText.Text = string.Empty;
                    for (int i = 0; i < results.Length - 1; i++)
                    {
                        if (tbOutputText.TextLength == 0)
                            tbOutputText.Text = results[i];
                        else
                            tbOutputText.Text = string.Format("{0},{1}", tbOutputText.Text, results[i]);
                    }
                }
            }
        }

        private void NudRGB_ValueChanged(object sender, EventArgs e)
        {
            if (MultiColour)
            {
                if (tbOutputText.TextLength > 0)
                {
                    string[] results = tbOutputText.Text.Split("$");
                    tbOutputText.Text = string.Format("|<{0}>##{1}${2}", tbComment.Text, nudRGB.Value, results[1]);
                }
            }
        }

        private void TbComment_TextChanged(object sender, EventArgs e)
        {
            if (tbOutputText.TextLength > 0)
            {
                string[] results = tbOutputText.Text.Split(">");
                tbOutputText.Text = string.Format("|<{0}>{1}", tbComment.Text, results[1]);
            }
        }

        private void BtnCopy_Click(object sender, EventArgs e)
        {
            string clipboard = string.Format("{{\"FindString\":\"{0}\", \"searchArea\":{{\"X\":{1}, \"Y\":{2}, \"width\":{3}, \"height\":{4}}}}}", SearchText, SearchRectangle.X, SearchRectangle.Y, SearchRectangle.Width, SearchRectangle.Height);
            Clipboard.SetText(clipboard);
        }

        /// <summary>
        /// Update the tbOutputText value based on the changing value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nudImageRGB_ValueChanged(object sender, EventArgs e)
        {
            if (tbOutputText.TextLength > 0)
            {
                string[] results = tbOutputText.Text.Split("$");
                tbOutputText.Text = string.Format("|<{0}>!!{1}${2}", tbComment.Text, nudImageRGB.Value, results[1]);
            }
        }

        private void ClearUI()
        {
            lastSelected = Point.Empty;
            tbGrayThreshold.Text = string.Empty;
            tbGrayDifference.Value = 50;
            nudRed.Value = 255;
            nudGreen.Value = 255;
            nudBlue.Value = 255;
            nudRGB.Value = 255;
            nudImageRGB.Value = 0;
            cbFindMultiColour.Checked = false;
            cbModify.Checked = false;
            removedBottom = removedLeft = removedRight = removedTop = 0;
        }

        /// <summary>
        /// Grabs the latest frame from ADB
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnADB_Click(object sender, EventArgs e)
        {
            ClearUI();
            if (server == null)
            {
                server = new AdbServer();
                StartServerResult result = server.StartServer(AppDomain.CurrentDomain.BaseDirectory + @"\ADB\adb.exe", restartServerIfNewer: true);
                if (result != StartServerResult.AlreadyRunning)
                {
                    Thread.Sleep(1500);
                    AdbServerStatus status = server.GetStatus();
                    if (!status.IsRunning)
                    {
                        MessageBox.Show("Unable to start ADB server");
                        return;
                    }
                }
            }

            AdbClient client = new AdbClient();
            IEnumerable<DeviceData> devices = client.GetDevices();

            List<string> devicesList = new List<string>();
            DeviceSelect deviceSelect = new DeviceSelect();
            foreach (DeviceData device in devices)
            {
                string deviceState = device.State == DeviceState.Online ? "device" : device.State.ToString().ToLower();
                string deviceId = String.Format("{0} {1} product:{2} model:{3} device:{4} features:{5}  transport_id:{6}", device.Serial, deviceState, device.Product, device.Model, device.Name, device.Features, device.TransportId);
                devicesList.Add(deviceId);
            }
            deviceSelect.LoadList(devicesList);
            if (deviceSelect.ShowDialog() == DialogResult.OK)
            {
                string deviceId = deviceSelect.selectedItem;
                DeviceData device = DeviceData.CreateFromAdbData(deviceId);

                Framebuffer framebuffer = new Framebuffer(device, client);
                System.Threading.CancellationToken cancellationToken = default;
                framebuffer.Refresh(false);
                Image image = framebuffer.ToImage();
                LoadGrabAndGrid(ref image);
            }
        }

        private void BtnGenText_Click(object sender, EventArgs e)
        {
            string textString;
            string colour = colourString;
            string txt = string.Empty;

            if (findMode == FindText.FindMode.multiColourMode)
            {
                SearchText = tbOutputText.Text;
            }
            else if ( findMode == FindText.FindMode.imageMode)
            {
                colour = String.Format("!!{0}", nudImageRGB.Value);
                int w = dgvImage.Columns.Count;
                int h = dgvImage.Rows.Count;

                if (w < 1 || h < 1)
                    return;

                List<uint> data = new List<uint>();
                for (int y = 0; y < h; y++)
                {
                    for (int innerLoop = 0; innerLoop < w; innerLoop++)
                    {
                        data.Add((uint)(dgvImage.Rows[y].Cells[innerLoop].Tag as ColourGridTag).ColourARGB);
                    }
                }
                byte[] byteArray = new byte[data.Count * 4];
                Buffer.BlockCopy(data.ToArray(), 0, byteArray, 0, data.Count * 4);
                string v = Convert.ToBase64String(byteArray,Base64FormattingOptions.None);
                textString = string.Format(@"|<{0}>{1}${2}.{3}.{4}", tbComment.Text, colour, w, h, v);
                tbOutputText.Text = textString;
                SearchText = textString;
            }
            else
            {
                if (!HasBeenGrayed)
                    return;

                for (int i = 0; i < dgvImage.Rows.Count; i++)
                    for (int j = 0; j < dgvImage.Columns.Count; j++)
                    {
                        if ((dgvImage.Rows[i].Cells[j].Tag as ColourGridTag).Black == true)
                            txt += "1";
                        else
                            txt += "0";
                    }
                if (findMode == FindText.FindMode.colourMode && UsePos && !MultiColour)
                {
                    string[] r = colour.Split("@");
                    int k = 0, l = 0;
                    for (int j = 0; j < dgvImage.Rows.Count; j++)
                        for (int i = 0; i < dgvImage.Columns.Count; i++)
                        {
                            k++;
                            if ((dgvImage.Rows[j].Cells[i].Tag as ColourGridTag).Black == false)
                                continue;
                            if (lastSelected == new Point(i, j))
                            {
                                l = k;
                                goto breakDoubleFor;
                            }
                        }
                    breakDoubleFor:
                    if (l == 0)
                    {
                        MessageBox.Show("Please Set Gray Difference First");
                        return;
                    }
                    colour = "#" + (l - 1).ToString() + "@" + r[1];
                }
                txt = string.Format("{0:d}", dgvImage.Columns.Count) + "." + FindText.Bit2base64(txt);
                textString = string.Format(@"|<{0}>{1}${2}", tbComment.Text, colour, txt);
                tbOutputText.Text = textString;
                SearchText = textString;
            }
        }
    }

    internal class ColourDictionaryEntry
    {
        public ColourDictionaryEntry(Bitmap bitmap, ColourGridTag tag)
        {
            Bitmap = bitmap;
            Tag = tag;
        }

        public Bitmap Bitmap { get; }
        public ColourGridTag Tag { get; }
    }

    internal class ColourGridTag
    {
        public ColourGridTag(int colourARGB, int red, int green, int blue, int gray, string colourString, bool black)
        {
            ColourARGB = colourARGB;
            Red = red;
            Green = green;
            Blue = blue;
            Gray = gray;
            ColourString = colourString;
            Black = black;
        }

        public int ColourARGB { get; }
        public int Red { get; }
        public int Green { get; }
        public int Blue { get; }
        public int Gray { get; set; }
        public string ColourString { get; }
        public bool Black { get; set; }

        public ColourGridTag ShallowCopy()
        {
            return (ColourGridTag)this.MemberwiseClone();
        }
    }

    internal class TagRows
    {
        public List<ColourGridTag> Cells { get; set; }

        public TagRows()
        {
            Cells = new List<ColourGridTag>();
        }
    }

    internal class CellItems
    {
        public List<int> Cells { get; set; }
        public CellItems()
        {
            Cells = new List<int>();
        }
    }
}
