﻿
namespace ScriptEditor
{
    partial class FindTextEdit
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.dgvImage = new System.Windows.Forms.DataGridView();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnLoadText = new System.Windows.Forms.Button();
            this.btnADB = new System.Windows.Forms.Button();
            this.btnLoadPic = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.tbRed = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tbGreen = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbBlue = new System.Windows.Forms.TextBox();
            this.tbColour = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbGray = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbComment = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbModify = new System.Windows.Forms.CheckBox();
            this.btnSavePic = new System.Windows.Forms.Button();
            this.btnCropBottom3 = new System.Windows.Forms.Button();
            this.btnCropBottomNegative = new System.Windows.Forms.Button();
            this.btnCropBottom = new System.Windows.Forms.Button();
            this.btnCropRight3 = new System.Windows.Forms.Button();
            this.btnCropRight = new System.Windows.Forms.Button();
            this.btnCropRightNegative = new System.Windows.Forms.Button();
            this.btnCropLeftNegative = new System.Windows.Forms.Button();
            this.btnCropLeft = new System.Windows.Forms.Button();
            this.btnCropAuto = new System.Windows.Forms.Button();
            this.btnCropLeft3 = new System.Windows.Forms.Button();
            this.btnCropTop3 = new System.Windows.Forms.Button();
            this.btnCropTop = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCropTopNegative = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.btnGray2Two = new System.Windows.Forms.Button();
            this.tbGrayThreshold = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.btnGrayDiff2Two = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.btnColour2Two = new System.Windows.Forms.Button();
            this.tbColourSlider = new System.Windows.Forms.TrackBar();
            this.label11 = new System.Windows.Forms.Label();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.btnColourPos2Two = new System.Windows.Forms.Button();
            this.tbColourPosSlider = new System.Windows.Forms.TrackBar();
            this.label12 = new System.Windows.Forms.Label();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.btnColourDiff2Two = new System.Windows.Forms.Button();
            this.label15 = new System.Windows.Forms.Label();
            this.nudBlue = new System.Windows.Forms.NumericUpDown();
            this.label14 = new System.Windows.Forms.Label();
            this.nudGreen = new System.Windows.Forms.NumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this.nudRed = new System.Windows.Forms.NumericUpDown();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.btnUndo = new System.Windows.Forms.Button();
            this.cbFindMultiColour = new System.Windows.Forms.CheckBox();
            this.label16 = new System.Windows.Forms.Label();
            this.nudRGB = new System.Windows.Forms.NumericUpDown();
            this.btnReset = new System.Windows.Forms.Button();
            this.tbOutputText = new System.Windows.Forms.TextBox();
            this.btnGenText = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.tbGrayDifference = new System.Windows.Forms.NumericUpDown();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvImage)).BeginInit();
            this.panel2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbColourSlider)).BeginInit();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbColourPosSlider)).BeginInit();
            this.tabPage5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudBlue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGreen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRed)).BeginInit();
            this.tabPage6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRGB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbGrayDifference)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.dgvImage);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(824, 344);
            this.panel1.TabIndex = 0;
            // 
            // dgvImage
            // 
            this.dgvImage.AllowUserToAddRows = false;
            this.dgvImage.AllowUserToDeleteRows = false;
            this.dgvImage.AllowUserToResizeColumns = false;
            this.dgvImage.AllowUserToResizeRows = false;
            this.dgvImage.ColumnHeadersHeight = 10;
            this.dgvImage.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvImage.ColumnHeadersVisible = false;
            this.dgvImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvImage.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvImage.Location = new System.Drawing.Point(0, 0);
            this.dgvImage.MultiSelect = false;
            this.dgvImage.Name = "dgvImage";
            this.dgvImage.ReadOnly = true;
            this.dgvImage.RowHeadersVisible = false;
            this.dgvImage.RowHeadersWidth = 10;
            this.dgvImage.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvImage.RowTemplate.Height = 10;
            this.dgvImage.ShowCellErrors = false;
            this.dgvImage.ShowEditingIcon = false;
            this.dgvImage.ShowRowErrors = false;
            this.dgvImage.Size = new System.Drawing.Size(824, 344);
            this.dgvImage.TabIndex = 0;
            this.dgvImage.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvImage_CellClick);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnLoadText);
            this.panel2.Controls.Add(this.btnADB);
            this.panel2.Controls.Add(this.btnLoadPic);
            this.panel2.Controls.Add(this.label9);
            this.panel2.Controls.Add(this.label8);
            this.panel2.Controls.Add(this.tbRed);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.tbGreen);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.tbBlue);
            this.panel2.Controls.Add(this.tbColour);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.tbGray);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.tbComment);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.cbModify);
            this.panel2.Controls.Add(this.btnSavePic);
            this.panel2.Controls.Add(this.btnCropBottom3);
            this.panel2.Controls.Add(this.btnCropBottomNegative);
            this.panel2.Controls.Add(this.btnCropBottom);
            this.panel2.Controls.Add(this.btnCropRight3);
            this.panel2.Controls.Add(this.btnCropRight);
            this.panel2.Controls.Add(this.btnCropRightNegative);
            this.panel2.Controls.Add(this.btnCropLeftNegative);
            this.panel2.Controls.Add(this.btnCropLeft);
            this.panel2.Controls.Add(this.btnCropAuto);
            this.panel2.Controls.Add(this.btnCropLeft3);
            this.panel2.Controls.Add(this.btnCropTop3);
            this.panel2.Controls.Add(this.btnCropTop);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.btnCropTopNegative);
            this.panel2.Controls.Add(this.tabControl1);
            this.panel2.Controls.Add(this.btnReset);
            this.panel2.Controls.Add(this.tbOutputText);
            this.panel2.Controls.Add(this.btnGenText);
            this.panel2.Controls.Add(this.btnOK);
            this.panel2.Controls.Add(this.btnCancel);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 344);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(824, 217);
            this.panel2.TabIndex = 1;
            // 
            // btnLoadText
            // 
            this.btnLoadText.Location = new System.Drawing.Point(82, 154);
            this.btnLoadText.Name = "btnLoadText";
            this.btnLoadText.Size = new System.Drawing.Size(75, 23);
            this.btnLoadText.TabIndex = 42;
            this.btnLoadText.Text = "Load Text";
            this.btnLoadText.UseVisualStyleBackColor = true;
            this.btnLoadText.Click += new System.EventHandler(this.btnLoadText_Click);
            // 
            // btnADB
            // 
            this.btnADB.Location = new System.Drawing.Point(739, 9);
            this.btnADB.Name = "btnADB";
            this.btnADB.Size = new System.Drawing.Size(75, 23);
            this.btnADB.TabIndex = 41;
            this.btnADB.Text = "ADB";
            this.btnADB.UseVisualStyleBackColor = true;
            this.btnADB.Click += new System.EventHandler(this.btnADB_Click);
            // 
            // btnLoadPic
            // 
            this.btnLoadPic.Location = new System.Drawing.Point(738, 38);
            this.btnLoadPic.Name = "btnLoadPic";
            this.btnLoadPic.Size = new System.Drawing.Size(75, 23);
            this.btnLoadPic.TabIndex = 40;
            this.btnLoadPic.Text = "Load Pic";
            this.btnLoadPic.UseVisualStyleBackColor = true;
            this.btnLoadPic.Click += new System.EventHandler(this.btnLoadPic_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 187);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(66, 15);
            this.label9.TabIndex = 39;
            this.label9.Text = "Search Text";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(567, 16);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(14, 15);
            this.label8.TabIndex = 38;
            this.label8.Text = "R";
            // 
            // tbRed
            // 
            this.tbRed.Location = new System.Drawing.Point(587, 13);
            this.tbRed.Name = "tbRed";
            this.tbRed.ReadOnly = true;
            this.tbRed.Size = new System.Drawing.Size(31, 23);
            this.tbRed.TabIndex = 37;
            this.tbRed.Text = "888";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(624, 17);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(15, 15);
            this.label7.TabIndex = 36;
            this.label7.Text = "G";
            // 
            // tbGreen
            // 
            this.tbGreen.Location = new System.Drawing.Point(645, 14);
            this.tbGreen.Name = "tbGreen";
            this.tbGreen.ReadOnly = true;
            this.tbGreen.Size = new System.Drawing.Size(31, 23);
            this.tbGreen.TabIndex = 35;
            this.tbGreen.Text = "888";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(682, 17);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(14, 15);
            this.label6.TabIndex = 34;
            this.label6.Text = "B";
            // 
            // tbBlue
            // 
            this.tbBlue.Location = new System.Drawing.Point(702, 14);
            this.tbBlue.Name = "tbBlue";
            this.tbBlue.ReadOnly = true;
            this.tbBlue.Size = new System.Drawing.Size(31, 23);
            this.tbBlue.TabIndex = 33;
            this.tbBlue.Text = "888";
            // 
            // tbColour
            // 
            this.tbColour.Location = new System.Drawing.Point(460, 14);
            this.tbColour.Name = "tbColour";
            this.tbColour.ReadOnly = true;
            this.tbColour.Size = new System.Drawing.Size(100, 23);
            this.tbColour.TabIndex = 32;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(411, 17);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(43, 15);
            this.label5.TabIndex = 31;
            this.label5.Text = "Colour";
            // 
            // tbGray
            // 
            this.tbGray.Location = new System.Drawing.Point(305, 14);
            this.tbGray.Name = "tbGray";
            this.tbGray.ReadOnly = true;
            this.tbGray.Size = new System.Drawing.Size(100, 23);
            this.tbGray.TabIndex = 30;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(268, 17);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(31, 15);
            this.label4.TabIndex = 29;
            this.label4.Text = "Gray";
            // 
            // tbComment
            // 
            this.tbComment.Location = new System.Drawing.Point(197, 122);
            this.tbComment.Name = "tbComment";
            this.tbComment.Size = new System.Drawing.Size(109, 23);
            this.tbComment.TabIndex = 28;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(130, 125);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 15);
            this.label2.TabIndex = 27;
            this.label2.Text = "Comment";
            // 
            // cbModify
            // 
            this.cbModify.AutoSize = true;
            this.cbModify.Location = new System.Drawing.Point(15, 125);
            this.cbModify.Name = "cbModify";
            this.cbModify.Size = new System.Drawing.Size(108, 19);
            this.cbModify.TabIndex = 26;
            this.cbModify.Text = "Modify Graphic";
            this.cbModify.UseVisualStyleBackColor = true;
            // 
            // btnSavePic
            // 
            this.btnSavePic.Location = new System.Drawing.Point(738, 67);
            this.btnSavePic.Name = "btnSavePic";
            this.btnSavePic.Size = new System.Drawing.Size(75, 23);
            this.btnSavePic.TabIndex = 25;
            this.btnSavePic.Text = "Save Pic";
            this.btnSavePic.UseVisualStyleBackColor = true;
            // 
            // btnCropBottom3
            // 
            this.btnCropBottom3.Location = new System.Drawing.Point(189, 71);
            this.btnCropBottom3.Name = "btnCropBottom3";
            this.btnCropBottom3.Size = new System.Drawing.Size(35, 23);
            this.btnCropBottom3.TabIndex = 19;
            this.btnCropBottom3.Text = "B3";
            this.btnCropBottom3.UseVisualStyleBackColor = true;
            this.btnCropBottom3.Click += new System.EventHandler(this.btnCropBottom3_Click);
            // 
            // btnCropBottomNegative
            // 
            this.btnCropBottomNegative.Location = new System.Drawing.Point(97, 71);
            this.btnCropBottomNegative.Name = "btnCropBottomNegative";
            this.btnCropBottomNegative.Size = new System.Drawing.Size(35, 23);
            this.btnCropBottomNegative.TabIndex = 18;
            this.btnCropBottomNegative.Text = "-B";
            this.btnCropBottomNegative.UseVisualStyleBackColor = true;
            this.btnCropBottomNegative.Click += new System.EventHandler(this.btnCropBottomNegative_Click);
            // 
            // btnCropBottom
            // 
            this.btnCropBottom.Location = new System.Drawing.Point(138, 71);
            this.btnCropBottom.Name = "btnCropBottom";
            this.btnCropBottom.Size = new System.Drawing.Size(45, 23);
            this.btnCropBottom.TabIndex = 17;
            this.btnCropBottom.Text = "B";
            this.btnCropBottom.UseVisualStyleBackColor = true;
            this.btnCropBottom.Click += new System.EventHandler(this.btnCropBottom_Click);
            // 
            // btnCropRight3
            // 
            this.btnCropRight3.Location = new System.Drawing.Point(271, 42);
            this.btnCropRight3.Name = "btnCropRight3";
            this.btnCropRight3.Size = new System.Drawing.Size(35, 23);
            this.btnCropRight3.TabIndex = 16;
            this.btnCropRight3.Text = "R3";
            this.btnCropRight3.UseVisualStyleBackColor = true;
            this.btnCropRight3.Click += new System.EventHandler(this.btnCropRight3_Click);
            // 
            // btnCropRight
            // 
            this.btnCropRight.Location = new System.Drawing.Point(230, 42);
            this.btnCropRight.Name = "btnCropRight";
            this.btnCropRight.Size = new System.Drawing.Size(35, 23);
            this.btnCropRight.TabIndex = 15;
            this.btnCropRight.Text = "R";
            this.btnCropRight.UseVisualStyleBackColor = true;
            this.btnCropRight.Click += new System.EventHandler(this.btnCropRight_Click);
            // 
            // btnCropRightNegative
            // 
            this.btnCropRightNegative.Location = new System.Drawing.Point(189, 42);
            this.btnCropRightNegative.Name = "btnCropRightNegative";
            this.btnCropRightNegative.Size = new System.Drawing.Size(35, 23);
            this.btnCropRightNegative.TabIndex = 14;
            this.btnCropRightNegative.Text = "-R";
            this.btnCropRightNegative.UseVisualStyleBackColor = true;
            this.btnCropRightNegative.Click += new System.EventHandler(this.btnCropRightNegative_Click);
            // 
            // btnCropLeftNegative
            // 
            this.btnCropLeftNegative.Location = new System.Drawing.Point(15, 42);
            this.btnCropLeftNegative.Name = "btnCropLeftNegative";
            this.btnCropLeftNegative.Size = new System.Drawing.Size(35, 23);
            this.btnCropLeftNegative.TabIndex = 13;
            this.btnCropLeftNegative.Text = "-L";
            this.btnCropLeftNegative.UseVisualStyleBackColor = true;
            this.btnCropLeftNegative.Click += new System.EventHandler(this.btnCropLeftNegative_Click);
            // 
            // btnCropLeft
            // 
            this.btnCropLeft.Location = new System.Drawing.Point(56, 42);
            this.btnCropLeft.Name = "btnCropLeft";
            this.btnCropLeft.Size = new System.Drawing.Size(35, 23);
            this.btnCropLeft.TabIndex = 12;
            this.btnCropLeft.Text = "L";
            this.btnCropLeft.UseVisualStyleBackColor = true;
            this.btnCropLeft.Click += new System.EventHandler(this.btnCropLeft_Click);
            // 
            // btnCropAuto
            // 
            this.btnCropAuto.Location = new System.Drawing.Point(138, 42);
            this.btnCropAuto.Name = "btnCropAuto";
            this.btnCropAuto.Size = new System.Drawing.Size(45, 23);
            this.btnCropAuto.TabIndex = 11;
            this.btnCropAuto.Text = "Auto";
            this.btnCropAuto.UseVisualStyleBackColor = true;
            // 
            // btnCropLeft3
            // 
            this.btnCropLeft3.Location = new System.Drawing.Point(97, 42);
            this.btnCropLeft3.Name = "btnCropLeft3";
            this.btnCropLeft3.Size = new System.Drawing.Size(35, 23);
            this.btnCropLeft3.TabIndex = 10;
            this.btnCropLeft3.Text = "L3";
            this.btnCropLeft3.UseVisualStyleBackColor = true;
            this.btnCropLeft3.Click += new System.EventHandler(this.btnCropLeft3_Click);
            // 
            // btnCropTop3
            // 
            this.btnCropTop3.Location = new System.Drawing.Point(189, 13);
            this.btnCropTop3.Name = "btnCropTop3";
            this.btnCropTop3.Size = new System.Drawing.Size(35, 23);
            this.btnCropTop3.TabIndex = 9;
            this.btnCropTop3.Text = "T3";
            this.btnCropTop3.UseVisualStyleBackColor = true;
            this.btnCropTop3.Click += new System.EventHandler(this.btnCropTop3_Click);
            // 
            // btnCropTop
            // 
            this.btnCropTop.Location = new System.Drawing.Point(138, 13);
            this.btnCropTop.Name = "btnCropTop";
            this.btnCropTop.Size = new System.Drawing.Size(45, 23);
            this.btnCropTop.TabIndex = 8;
            this.btnCropTop.Text = "T";
            this.btnCropTop.UseVisualStyleBackColor = true;
            this.btnCropTop.Click += new System.EventHandler(this.btnCropTop_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 15);
            this.label1.TabIndex = 7;
            this.label1.Text = "Cropping";
            // 
            // btnCropTopNegative
            // 
            this.btnCropTopNegative.Location = new System.Drawing.Point(97, 13);
            this.btnCropTopNegative.Name = "btnCropTopNegative";
            this.btnCropTopNegative.Size = new System.Drawing.Size(35, 23);
            this.btnCropTopNegative.TabIndex = 6;
            this.btnCropTopNegative.Text = "-T";
            this.btnCropTopNegative.UseVisualStyleBackColor = true;
            this.btnCropTopNegative.Click += new System.EventHandler(this.btnCropTopNegative_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Controls.Add(this.tabPage6);
            this.tabControl1.Location = new System.Drawing.Point(312, 42);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(419, 100);
            this.tabControl1.TabIndex = 5;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.btnGray2Two);
            this.tabPage1.Controls.Add(this.tbGrayThreshold);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(411, 72);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Gray";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // btnGray2Two
            // 
            this.btnGray2Two.Location = new System.Drawing.Point(207, 7);
            this.btnGray2Two.Name = "btnGray2Two";
            this.btnGray2Two.Size = new System.Drawing.Size(75, 23);
            this.btnGray2Two.TabIndex = 2;
            this.btnGray2Two.Text = "Gray 2 Two";
            this.btnGray2Two.UseVisualStyleBackColor = true;
            this.btnGray2Two.Click += new System.EventHandler(this.btnGray2Two_Click);
            // 
            // tbGrayThreshold
            // 
            this.tbGrayThreshold.Location = new System.Drawing.Point(100, 7);
            this.tbGrayThreshold.Name = "tbGrayThreshold";
            this.tbGrayThreshold.Size = new System.Drawing.Size(100, 23);
            this.tbGrayThreshold.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 7);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(86, 15);
            this.label3.TabIndex = 0;
            this.label3.Text = "Gray Threshold";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.tbGrayDifference);
            this.tabPage2.Controls.Add(this.btnGrayDiff2Two);
            this.tabPage2.Controls.Add(this.label10);
            this.tabPage2.Location = new System.Drawing.Point(4, 24);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(411, 72);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "GrayDiff";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // btnGrayDiff2Two
            // 
            this.btnGrayDiff2Two.Location = new System.Drawing.Point(207, 7);
            this.btnGrayDiff2Two.Name = "btnGrayDiff2Two";
            this.btnGrayDiff2Two.Size = new System.Drawing.Size(95, 23);
            this.btnGrayDiff2Two.TabIndex = 5;
            this.btnGrayDiff2Two.Text = "Gray Diff 2 Two";
            this.btnGrayDiff2Two.UseVisualStyleBackColor = true;
            this.btnGrayDiff2Two.Click += new System.EventHandler(this.btnGrayDiff2Two_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(7, 7);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(88, 15);
            this.label10.TabIndex = 3;
            this.label10.Text = "Gray Difference";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.btnColour2Two);
            this.tabPage3.Controls.Add(this.tbColourSlider);
            this.tabPage3.Controls.Add(this.label11);
            this.tabPage3.Location = new System.Drawing.Point(4, 24);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(411, 72);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Colour";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // btnColour2Two
            // 
            this.btnColour2Two.Location = new System.Drawing.Point(308, 7);
            this.btnColour2Two.Name = "btnColour2Two";
            this.btnColour2Two.Size = new System.Drawing.Size(95, 23);
            this.btnColour2Two.TabIndex = 6;
            this.btnColour2Two.Text = "Colour 2 Two";
            this.btnColour2Two.UseVisualStyleBackColor = true;
            // 
            // tbColourSlider
            // 
            this.tbColourSlider.Location = new System.Drawing.Point(70, 5);
            this.tbColourSlider.Maximum = 100;
            this.tbColourSlider.Name = "tbColourSlider";
            this.tbColourSlider.Size = new System.Drawing.Size(219, 45);
            this.tbColourSlider.TabIndex = 2;
            this.tbColourSlider.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbColourSlider.Value = 100;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(7, 7);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(56, 15);
            this.label11.TabIndex = 1;
            this.label11.Text = "Similarity";
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.btnColourPos2Two);
            this.tabPage4.Controls.Add(this.tbColourPosSlider);
            this.tabPage4.Controls.Add(this.label12);
            this.tabPage4.Location = new System.Drawing.Point(4, 24);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(411, 72);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "ColourPos";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // btnColourPos2Two
            // 
            this.btnColourPos2Two.Location = new System.Drawing.Point(295, 7);
            this.btnColourPos2Two.Name = "btnColourPos2Two";
            this.btnColourPos2Two.Size = new System.Drawing.Size(108, 23);
            this.btnColourPos2Two.TabIndex = 9;
            this.btnColourPos2Two.Text = "Colour Pos 2 Two";
            this.btnColourPos2Two.UseVisualStyleBackColor = true;
            // 
            // tbColourPosSlider
            // 
            this.tbColourPosSlider.Location = new System.Drawing.Point(70, 5);
            this.tbColourPosSlider.Maximum = 100;
            this.tbColourPosSlider.Name = "tbColourPosSlider";
            this.tbColourPosSlider.Size = new System.Drawing.Size(219, 45);
            this.tbColourPosSlider.TabIndex = 8;
            this.tbColourPosSlider.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbColourPosSlider.Value = 100;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(7, 7);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(56, 15);
            this.label12.TabIndex = 7;
            this.label12.Text = "Similarity";
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.btnColourDiff2Two);
            this.tabPage5.Controls.Add(this.label15);
            this.tabPage5.Controls.Add(this.nudBlue);
            this.tabPage5.Controls.Add(this.label14);
            this.tabPage5.Controls.Add(this.nudGreen);
            this.tabPage5.Controls.Add(this.label13);
            this.tabPage5.Controls.Add(this.nudRed);
            this.tabPage5.Location = new System.Drawing.Point(4, 24);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(411, 72);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "ColourDiff";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // btnColourDiff2Two
            // 
            this.btnColourDiff2Two.Location = new System.Drawing.Point(297, 9);
            this.btnColourDiff2Two.Name = "btnColourDiff2Two";
            this.btnColourDiff2Two.Size = new System.Drawing.Size(108, 23);
            this.btnColourDiff2Two.TabIndex = 10;
            this.btnColourDiff2Two.Text = "Colour Diff 2 Two";
            this.btnColourDiff2Two.UseVisualStyleBackColor = true;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(191, 11);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(30, 15);
            this.label15.TabIndex = 5;
            this.label15.Text = "Blue";
            // 
            // nudBlue
            // 
            this.nudBlue.Location = new System.Drawing.Point(227, 9);
            this.nudBlue.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudBlue.Name = "nudBlue";
            this.nudBlue.Size = new System.Drawing.Size(48, 23);
            this.nudBlue.TabIndex = 4;
            this.nudBlue.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(93, 9);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(38, 15);
            this.label14.TabIndex = 3;
            this.label14.Text = "Green";
            // 
            // nudGreen
            // 
            this.nudGreen.Location = new System.Drawing.Point(137, 7);
            this.nudGreen.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudGreen.Name = "nudGreen";
            this.nudGreen.Size = new System.Drawing.Size(48, 23);
            this.nudGreen.TabIndex = 2;
            this.nudGreen.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(6, 9);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(27, 15);
            this.label13.TabIndex = 1;
            this.label13.Text = "Red";
            // 
            // nudRed
            // 
            this.nudRed.Location = new System.Drawing.Point(39, 6);
            this.nudRed.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudRed.Name = "nudRed";
            this.nudRed.Size = new System.Drawing.Size(48, 23);
            this.nudRed.TabIndex = 0;
            this.nudRed.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.btnUndo);
            this.tabPage6.Controls.Add(this.cbFindMultiColour);
            this.tabPage6.Controls.Add(this.label16);
            this.tabPage6.Controls.Add(this.nudRGB);
            this.tabPage6.Location = new System.Drawing.Point(4, 24);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage6.Size = new System.Drawing.Size(411, 72);
            this.tabPage6.TabIndex = 5;
            this.tabPage6.Text = "MultiColour";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // btnUndo
            // 
            this.btnUndo.Location = new System.Drawing.Point(233, 6);
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.Size = new System.Drawing.Size(75, 23);
            this.btnUndo.TabIndex = 5;
            this.btnUndo.Text = "Unfo";
            this.btnUndo.UseVisualStyleBackColor = true;
            // 
            // cbFindMultiColour
            // 
            this.cbFindMultiColour.AutoSize = true;
            this.cbFindMultiColour.Location = new System.Drawing.Point(108, 8);
            this.cbFindMultiColour.Name = "cbFindMultiColour";
            this.cbFindMultiColour.Size = new System.Drawing.Size(119, 19);
            this.cbFindMultiColour.TabIndex = 4;
            this.cbFindMultiColour.Text = "Find Multi Colour";
            this.cbFindMultiColour.UseVisualStyleBackColor = true;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(6, 8);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(39, 15);
            this.label16.TabIndex = 3;
            this.label16.Text = "R/G/B";
            // 
            // nudRGB
            // 
            this.nudRGB.Location = new System.Drawing.Point(51, 6);
            this.nudRGB.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudRGB.Name = "nudRGB";
            this.nudRGB.Size = new System.Drawing.Size(48, 23);
            this.nudRGB.TabIndex = 2;
            this.nudRGB.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(737, 96);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 4;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // tbOutputText
            // 
            this.tbOutputText.Location = new System.Drawing.Point(82, 184);
            this.tbOutputText.Name = "tbOutputText";
            this.tbOutputText.Size = new System.Drawing.Size(649, 23);
            this.tbOutputText.TabIndex = 3;
            this.tbOutputText.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbOutputText_KeyDown);
            // 
            // btnGenText
            // 
            this.btnGenText.Location = new System.Drawing.Point(737, 125);
            this.btnGenText.Name = "btnGenText";
            this.btnGenText.Size = new System.Drawing.Size(75, 23);
            this.btnGenText.TabIndex = 2;
            this.btnGenText.Text = "Gen Text";
            this.btnGenText.UseVisualStyleBackColor = true;
            this.btnGenText.Click += new System.EventHandler(this.btnGenText_Click);
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(737, 154);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(737, 183);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "bmp";
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "BMP files|*.bmp|PNG files|*.png|All files|*.*";
            this.openFileDialog1.Title = "Image File";
            // 
            // tbGrayDifference
            // 
            this.tbGrayDifference.Location = new System.Drawing.Point(102, 7);
            this.tbGrayDifference.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.tbGrayDifference.Name = "tbGrayDifference";
            this.tbGrayDifference.Size = new System.Drawing.Size(99, 23);
            this.tbGrayDifference.TabIndex = 6;
            this.tbGrayDifference.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // FindTextEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(824, 561);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.MinimumSize = new System.Drawing.Size(840, 600);
            this.Name = "FindTextEdit";
            this.Text = "FindTextEdit";
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvImage)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbColourSlider)).EndInit();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbColourPosSlider)).EndInit();
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudBlue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGreen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRed)).EndInit();
            this.tabPage6.ResumeLayout(false);
            this.tabPage6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRGB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbGrayDifference)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnLoadText;
        private System.Windows.Forms.Button btnADB;
        private System.Windows.Forms.Button btnLoadPic;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbRed;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbGreen;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbBlue;
        private System.Windows.Forms.TextBox tbColour;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbGray;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbComment;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox cbModify;
        private System.Windows.Forms.Button btnSavePic;
        private System.Windows.Forms.Button btnCropBottom3;
        private System.Windows.Forms.Button btnCropBottomNegative;
        private System.Windows.Forms.Button btnCropBottom;
        private System.Windows.Forms.Button btnCropRight3;
        private System.Windows.Forms.Button btnCropRight;
        private System.Windows.Forms.Button btnCropRightNegative;
        private System.Windows.Forms.Button btnCropLeftNegative;
        private System.Windows.Forms.Button btnCropLeft;
        private System.Windows.Forms.Button btnCropAuto;
        private System.Windows.Forms.Button btnCropLeft3;
        private System.Windows.Forms.Button btnCropTop3;
        private System.Windows.Forms.Button btnCropTop;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnCropTopNegative;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button btnGray2Two;
        private System.Windows.Forms.TextBox tbGrayThreshold;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.TextBox tbOutputText;
        private System.Windows.Forms.Button btnGenText;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnGrayDiff2Two;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button btnColour2Two;
        private System.Windows.Forms.TrackBar tbColourSlider;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button btnColourPos2Two;
        private System.Windows.Forms.TrackBar tbColourPosSlider;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.NumericUpDown nudGreen;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.NumericUpDown nudRed;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnColourDiff2Two;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.NumericUpDown nudBlue;
        private System.Windows.Forms.Button btnUndo;
        private System.Windows.Forms.CheckBox cbFindMultiColour;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.NumericUpDown nudRGB;
        private System.Windows.Forms.DataGridView dgvImage;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.NumericUpDown tbGrayDifference;
    }
}