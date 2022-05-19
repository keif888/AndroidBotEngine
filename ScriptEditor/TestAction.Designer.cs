// <copyright file="TestAction.Designer.cs" company="Keith Martin">
// Copyright (c) Keith Martin
// Licensed under the Apache License, Version 2.0 (the "License")</copyright>


namespace ScriptEditor
{
    partial class TestAction
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
            this.pnlControls = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnTest = new System.Windows.Forms.Button();
            this.cbActions = new System.Windows.Forms.ComboBox();
            this.tbLogger = new System.Windows.Forms.TextBox();
            this.testWorker = new System.ComponentModel.BackgroundWorker();
            this.pnlControls.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlControls
            // 
            this.pnlControls.Controls.Add(this.btnCancel);
            this.pnlControls.Controls.Add(this.label1);
            this.pnlControls.Controls.Add(this.btnTest);
            this.pnlControls.Controls.Add(this.cbActions);
            this.pnlControls.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlControls.Location = new System.Drawing.Point(0, 0);
            this.pnlControls.Name = "pnlControls";
            this.pnlControls.Size = new System.Drawing.Size(800, 45);
            this.pnlControls.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(378, 11);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "Action";
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(297, 12);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(75, 23);
            this.btnTest.TabIndex = 1;
            this.btnTest.Text = "Test";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.BtnTest_Click);
            // 
            // cbActions
            // 
            this.cbActions.FormattingEnabled = true;
            this.cbActions.Location = new System.Drawing.Point(60, 12);
            this.cbActions.Name = "cbActions";
            this.cbActions.Size = new System.Drawing.Size(231, 23);
            this.cbActions.TabIndex = 0;
            // 
            // tbLogger
            // 
            this.tbLogger.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbLogger.Location = new System.Drawing.Point(0, 45);
            this.tbLogger.Multiline = true;
            this.tbLogger.Name = "tbLogger";
            this.tbLogger.ReadOnly = true;
            this.tbLogger.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbLogger.Size = new System.Drawing.Size(800, 405);
            this.tbLogger.TabIndex = 1;
            this.tbLogger.WordWrap = false;
            // 
            // testWorker
            // 
            this.testWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.TestWorker_DoWork);
            this.testWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.TestWorker_RunWorkerCompleted);
            // 
            // TestAction
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tbLogger);
            this.Controls.Add(this.pnlControls);
            this.Name = "TestAction";
            this.Text = "TestAction";
            this.Load += new System.EventHandler(this.TestAction_Load);
            this.pnlControls.ResumeLayout(false);
            this.pnlControls.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlControls;
        private System.Windows.Forms.TextBox tbLogger;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.ComboBox cbActions;
        private System.Windows.Forms.Button btnCancel;
        private System.ComponentModel.BackgroundWorker testWorker;
    }
}