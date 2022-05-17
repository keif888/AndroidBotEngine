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
            this.btnGrab = new System.Windows.Forms.Button();
            this.btnTest = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.pnlControls = new System.Windows.Forms.Panel();
            this.cbFindString = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pnlGraphics = new System.Windows.Forms.Panel();
            this.pbFrame = new System.Windows.Forms.PictureBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tssText = new System.Windows.Forms.ToolStripStatusLabel();
            this.pnlControls.SuspendLayout();
            this.pnlGraphics.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbFrame)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnGrab
            // 
            this.btnGrab.Location = new System.Drawing.Point(344, 12);
            this.btnGrab.Name = "btnGrab";
            this.btnGrab.Size = new System.Drawing.Size(75, 23);
            this.btnGrab.TabIndex = 0;
            this.btnGrab.Text = "Grab";
            this.btnGrab.UseVisualStyleBackColor = true;
            this.btnGrab.Click += new System.EventHandler(this.BtnGrab_Click);
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(425, 12);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(75, 23);
            this.btnTest.TabIndex = 1;
            this.btnTest.Text = "Test";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.BtnTest_Click);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(506, 12);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 2;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.BtnReset_Click);
            // 
            // pnlControls
            // 
            this.pnlControls.Controls.Add(this.cbFindString);
            this.pnlControls.Controls.Add(this.label1);
            this.pnlControls.Controls.Add(this.btnGrab);
            this.pnlControls.Controls.Add(this.btnReset);
            this.pnlControls.Controls.Add(this.btnTest);
            this.pnlControls.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlControls.Location = new System.Drawing.Point(0, 0);
            this.pnlControls.Name = "pnlControls";
            this.pnlControls.Size = new System.Drawing.Size(980, 47);
            this.pnlControls.TabIndex = 3;
            // 
            // cbFindString
            // 
            this.cbFindString.FormattingEnabled = true;
            this.cbFindString.Location = new System.Drawing.Point(82, 12);
            this.cbFindString.Name = "cbFindString";
            this.cbFindString.Size = new System.Drawing.Size(256, 23);
            this.cbFindString.Sorted = true;
            this.cbFindString.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 15);
            this.label1.TabIndex = 3;
            this.label1.Text = "Find String";
            // 
            // pnlGraphics
            // 
            this.pnlGraphics.AutoScroll = true;
            this.pnlGraphics.Controls.Add(this.pbFrame);
            this.pnlGraphics.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlGraphics.Location = new System.Drawing.Point(0, 47);
            this.pnlGraphics.Name = "pnlGraphics";
            this.pnlGraphics.Size = new System.Drawing.Size(980, 433);
            this.pnlGraphics.TabIndex = 4;
            // 
            // pbFrame
            // 
            this.pbFrame.Location = new System.Drawing.Point(0, 0);
            this.pbFrame.Name = "pbFrame";
            this.pbFrame.Size = new System.Drawing.Size(318, 272);
            this.pbFrame.TabIndex = 0;
            this.pbFrame.TabStop = false;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tssText});
            this.statusStrip1.Location = new System.Drawing.Point(0, 480);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(980, 22);
            this.statusStrip1.TabIndex = 5;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tssText
            // 
            this.tssText.Name = "tssText";
            this.tssText.Size = new System.Drawing.Size(88, 17);
            this.tssText.Text = "Nothing Found";
            // 
            // FindTextValidate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(980, 502);
            this.Controls.Add(this.pnlGraphics);
            this.Controls.Add(this.pnlControls);
            this.Controls.Add(this.statusStrip1);
            this.Name = "FindTextValidate";
            this.Text = "FindTextValidate";
            this.pnlControls.ResumeLayout(false);
            this.pnlControls.PerformLayout();
            this.pnlGraphics.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbFrame)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

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
    }
}