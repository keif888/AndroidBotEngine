// <copyright file="FindTextValidate.cs" company="Keith Martin">
// Copyright (c) Keith Martin
// Licensed under the Apache License, Version 2.0 (the "License")</copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using BotEngineClient;
using FindTextClient;
using AdvancedSharpAdbClient;
using AdvancedSharpAdbClient.Models;

namespace ScriptEditor
{
    public partial class FindTextValidate : Form
    {
        private Image loadedFromADBImage;
        private BOTConfig gameConfig;
        private readonly FindText findText;
        private Rectangle adbScreenSize;
        private bool foundDone;

        public FindTextValidate()
        {
            InitializeComponent();
            gameConfig = null;
            loadedFromADBImage = null;
            findText = new FindText();
            adbScreenSize = new Rectangle(0, 0, 10, 10);
            foundDone = false;
        }

        // ToDo: Update the FindText tester, so that you can tweak settings, and copy/paste them back

        /// <summary>
        /// Grab a frame from the selected ADB.  And put it on the panel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnGrab_Click(object sender, EventArgs e)
        {
            if (gameConfig == null)
            {
                MessageBox.Show("Error: Game Config not loaded.This shouldn't happen!", "No GameConfig", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                AdbClient client = new AdbClient();
                string deviceId = cbDevices.SelectedItem.ToString();
                DeviceData device = DeviceData.CreateFromAdbData(deviceId);

                Framebuffer framebuffer = new Framebuffer(device, client);
                System.Threading.CancellationToken cancellationToken = default;
                framebuffer.Refresh(false);
                loadedFromADBImage = framebuffer.ToImage();
                adbScreenSize = new Rectangle(0, 0, loadedFromADBImage.Width, loadedFromADBImage.Height);
                BtnReset_Click(sender, e);
            }
        }

        /// <summary>
        /// Check if the selected FindString is available on the image.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnTest_Click(object sender, EventArgs e)
        {
            if (loadedFromADBImage != null)
            {
                if (gameConfig.FindStrings.ContainsKey(cbFindString.SelectedItem.ToString()))
                {
                    int zx = adbScreenSize.X, zy = adbScreenSize.Y, w = adbScreenSize.Width, h = adbScreenSize.Height;
                    FindString searchString = gameConfig.FindStrings[cbFindString.SelectedItem.ToString()];

                    findText.LoadImage(loadedFromADBImage, ref zx, ref zy, ref w, ref h);
                    List<SearchResult>? dataresult = findText.SearchText(searchString.SearchArea.X, searchString.SearchArea.Y, searchString.SearchArea.X + searchString.SearchArea.Width, searchString.SearchArea.Y + searchString.SearchArea.Height, searchString.BackgroundTolerance, searchString.TextTolerance, searchString.SearchString, false, cbShowAll.Checked, false, searchString.OffsetX, searchString.OffsetY);
                    if (dataresult != null)
                    {
                        tssText.Text = string.Format("Found {0} instances of FindString {1}", dataresult.Count, cbFindString.SelectedItem.ToString());
                        pnlGraphics.SuspendLayout();
                        pbFrame.SuspendLayout();
                        if (foundDone)
                            pbFrame.Image.Dispose();
                        Image localImage = (Image)loadedFromADBImage.Clone();
                        using Graphics graphics = Graphics.FromImage(localImage);
                        foreach (SearchResult result in dataresult)
                        {
                            Rectangle rectangle = new Rectangle(result.TopLeftX, result.TopLeftY, result.Width, result.Height);
                            graphics.DrawRectangle(Pens.LightGray, rectangle);
                            Debug.WriteLine("Found at {0},{1},{2},{3}", result.TopLeftX, result.TopLeftY, result.Width, result.Height);
                        }
                        pbFrame.Image = localImage;
                        pbFrame.ResumeLayout();
                        pnlGraphics.ResumeLayout();
                        foundDone = true;
                    }
                    else
                    {
                        tssText.Text = string.Format("Found 0 instances of FindString {0}", cbFindString.SelectedItem.ToString());
                    }
                }
            }
            else
            {
                MessageBox.Show("You have to Grab an image 1st.", "No Image", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Resets the image back to the one loaded from ADB.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnReset_Click(object sender, EventArgs e)
        {
            if (loadedFromADBImage != null)
            {
                if (foundDone)
                    pbFrame.Image.Dispose();
                pnlGraphics.SuspendLayout();
                pbFrame.SuspendLayout();
                pbFrame.Image = loadedFromADBImage;
                pbFrame.Width = loadedFromADBImage.Width;
                pbFrame.Height = loadedFromADBImage.Height;
                pbFrame.ResumeLayout();
                pnlGraphics.ResumeLayout();
                foundDone = false;
            }
            else
            {
                MessageBox.Show("You have to Grab an image 1st.", "No Image", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Configures the combo boxes on the form
        /// </summary>
        /// <param name="GameConfig"></param>
        /// <param name="devicesList"></param>
        public void SetupFindTextValidate(BOTConfig GameConfig, List<string> devicesList)
        {
            gameConfig = GameConfig;
            cbFindString.Items.Clear();
            foreach (KeyValuePair<string, FindString> item in gameConfig.FindStrings)
            {
                cbFindString.Items.Add(item.Key);
            }
            cbFindString.SelectedIndex = 0;
            cbDevices.Items.Clear();
            foreach (string item in devicesList)
            {
                cbDevices.Items.Add(item);
            }
            cbDevices.SelectedIndex = 0;
        }
    }
}
