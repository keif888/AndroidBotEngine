// <copyright file="FindTextValidate.Designer.cs" company="Keith Martin">
// Copyright (c) Keith Martin
// Licensed under the Apache License, Version 2.0 (the "License")</copyright>


namespace ScriptEditor
{
    partial class FindTextValidate
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
            btnGrab = new System.Windows.Forms.Button();
            btnTest = new System.Windows.Forms.Button();
            btnReset = new System.Windows.Forms.Button();
            pnlControls = new System.Windows.Forms.Panel();
            label2 = new System.Windows.Forms.Label();
            cbDevices = new System.Windows.Forms.ComboBox();
            cbFindString = new System.Windows.Forms.ComboBox();
            label1 = new System.Windows.Forms.Label();
            pnlGraphics = new System.Windows.Forms.Panel();
            pbFrame = new System.Windows.Forms.PictureBox();
            statusStrip1 = new System.Windows.Forms.StatusStrip();
            tssText = new System.Windows.Forms.ToolStripStatusLabel();
            cbShowAll = new System.Windows.Forms.CheckBox();
            pnlControls.SuspendLayout();
            pnlGraphics.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbFrame).BeginInit();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // btnGrab
            // 
            btnGrab.Location = new System.Drawing.Point(731, 11);
            btnGrab.Name = "btnGrab";
            btnGrab.Size = new System.Drawing.Size(75, 23);
            btnGrab.TabIndex = 0;
            btnGrab.Text = "Grab";
            btnGrab.UseVisualStyleBackColor = true;
            btnGrab.Click += BtnGrab_Click;
            // 
            // btnTest
            // 
            btnTest.Location = new System.Drawing.Point(812, 11);
            btnTest.Name = "btnTest";
            btnTest.Size = new System.Drawing.Size(75, 23);
            btnTest.TabIndex = 1;
            btnTest.Text = "Test";
            btnTest.UseVisualStyleBackColor = true;
            btnTest.Click += BtnTest_Click;
            // 
            // btnReset
            // 
            btnReset.Location = new System.Drawing.Point(893, 11);
            btnReset.Name = "btnReset";
            btnReset.Size = new System.Drawing.Size(75, 23);
            btnReset.TabIndex = 2;
            btnReset.Text = "Reset";
            btnReset.UseVisualStyleBackColor = true;
            btnReset.Click += BtnReset_Click;
            // 
            // pnlControls
            // 
            pnlControls.Controls.Add(cbShowAll);
            pnlControls.Controls.Add(label2);
            pnlControls.Controls.Add(cbDevices);
            pnlControls.Controls.Add(cbFindString);
            pnlControls.Controls.Add(label1);
            pnlControls.Controls.Add(btnGrab);
            pnlControls.Controls.Add(btnReset);
            pnlControls.Controls.Add(btnTest);
            pnlControls.Dock = System.Windows.Forms.DockStyle.Top;
            pnlControls.Location = new System.Drawing.Point(0, 0);
            pnlControls.Name = "pnlControls";
            pnlControls.Size = new System.Drawing.Size(980, 47);
            pnlControls.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(12, 15);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(42, 15);
            label2.TabIndex = 7;
            label2.Text = "Device";
            // 
            // cbDevices
            // 
            cbDevices.DropDownWidth = 400;
            cbDevices.FormattingEnabled = true;
            cbDevices.Location = new System.Drawing.Point(61, 12);
            cbDevices.Name = "cbDevices";
            cbDevices.Size = new System.Drawing.Size(200, 23);
            cbDevices.Sorted = true;
            cbDevices.TabIndex = 6;
            // 
            // cbFindString
            // 
            cbFindString.FormattingEnabled = true;
            cbFindString.Location = new System.Drawing.Point(339, 12);
            cbFindString.Name = "cbFindString";
            cbFindString.Size = new System.Drawing.Size(256, 23);
            cbFindString.Sorted = true;
            cbFindString.TabIndex = 4;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(269, 16);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(64, 15);
            label1.TabIndex = 3;
            label1.Text = "Find String";
            // 
            // pnlGraphics
            // 
            pnlGraphics.AutoScroll = true;
            pnlGraphics.Controls.Add(pbFrame);
            pnlGraphics.Dock = System.Windows.Forms.DockStyle.Fill;
            pnlGraphics.Location = new System.Drawing.Point(0, 47);
            pnlGraphics.Name = "pnlGraphics";
            pnlGraphics.Size = new System.Drawing.Size(980, 433);
            pnlGraphics.TabIndex = 4;
            // 
            // pbFrame
            // 
            pbFrame.Location = new System.Drawing.Point(0, 0);
            pbFrame.Name = "pbFrame";
            pbFrame.Size = new System.Drawing.Size(318, 272);
            pbFrame.TabIndex = 0;
            pbFrame.TabStop = false;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { tssText });
            statusStrip1.Location = new System.Drawing.Point(0, 480);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new System.Drawing.Size(980, 22);
            statusStrip1.TabIndex = 5;
            statusStrip1.Text = "statusStrip1";
            // 
            // tssText
            // 
            tssText.Name = "tssText";
            tssText.Size = new System.Drawing.Size(88, 17);
            tssText.Text = "Nothing Found";
            // 
            // cbShowAll
            // 
            cbShowAll.AutoSize = true;
            cbShowAll.Location = new System.Drawing.Point(601, 14);
            cbShowAll.Name = "cbShowAll";
            cbShowAll.Size = new System.Drawing.Size(72, 19);
            cbShowAll.TabIndex = 8;
            cbShowAll.Text = "Show All";
            cbShowAll.UseVisualStyleBackColor = true;
            // 
            // FindTextValidate
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(980, 502);
            Controls.Add(pnlGraphics);
            Controls.Add(pnlControls);
            Controls.Add(statusStrip1);
            MinimumSize = new System.Drawing.Size(990, 540);
            Name = "FindTextValidate";
            Text = "FindTextValidate";
            pnlControls.ResumeLayout(false);
            pnlControls.PerformLayout();
            pnlGraphics.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pbFrame).EndInit();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button btnGrab;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Panel pnlControls;
        private System.Windows.Forms.ComboBox cbFindString;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel pnlGraphics;
        private System.Windows.Forms.PictureBox pbFrame;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tssText;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbDevices;
        private System.Windows.Forms.CheckBox cbShowAll;
    }
}