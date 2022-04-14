
namespace ScriptEditor
{
    partial class GrabViewer
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
            this.pbFrame = new System.Windows.Forms.PictureBox();
            this.btnCopy = new System.Windows.Forms.Button();
            this.tbCurrent = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnCapture = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.tbBottomRight = new System.Windows.Forms.TextBox();
            this.tbTopLeft = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pnlControls = new System.Windows.Forms.Panel();
            this.pnlGraphics = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pbFrame)).BeginInit();
            this.pnlControls.SuspendLayout();
            this.pnlGraphics.SuspendLayout();
            this.SuspendLayout();
            // 
            // pbFrame
            // 
            this.pbFrame.Location = new System.Drawing.Point(0, 0);
            this.pbFrame.Name = "pbFrame";
            this.pbFrame.Size = new System.Drawing.Size(446, 208);
            this.pbFrame.TabIndex = 0;
            this.pbFrame.TabStop = false;
            this.pbFrame.Paint += new System.Windows.Forms.PaintEventHandler(this.pbFrame_Paint);
            this.pbFrame.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbFrame_MouseDown);
            this.pbFrame.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbFrame_MouseMove);
            this.pbFrame.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbFrame_MouseUp);
            // 
            // btnCopy
            // 
            this.btnCopy.Location = new System.Drawing.Point(289, 10);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(75, 23);
            this.btnCopy.TabIndex = 8;
            this.btnCopy.Text = "Copy";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // tbCurrent
            // 
            this.tbCurrent.Location = new System.Drawing.Point(429, 10);
            this.tbCurrent.Name = "tbCurrent";
            this.tbCurrent.ReadOnly = true;
            this.tbCurrent.Size = new System.Drawing.Size(100, 23);
            this.tbCurrent.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(376, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 15);
            this.label3.TabIndex = 6;
            this.label3.Text = "Current";
            // 
            // btnCapture
            // 
            this.btnCapture.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnCapture.Location = new System.Drawing.Point(616, 10);
            this.btnCapture.Name = "btnCapture";
            this.btnCapture.Size = new System.Drawing.Size(75, 23);
            this.btnCapture.TabIndex = 5;
            this.btnCapture.Text = "Capture";
            this.btnCapture.UseVisualStyleBackColor = true;
            this.btnCapture.Click += new System.EventHandler(this.btnCapture_Click);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(535, 10);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 4;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // tbBottomRight
            // 
            this.tbBottomRight.Location = new System.Drawing.Point(183, 10);
            this.tbBottomRight.Name = "tbBottomRight";
            this.tbBottomRight.ReadOnly = true;
            this.tbBottomRight.Size = new System.Drawing.Size(100, 23);
            this.tbBottomRight.TabIndex = 3;
            // 
            // tbTopLeft
            // 
            this.tbTopLeft.Location = new System.Drawing.Point(77, 10);
            this.tbTopLeft.Name = "tbTopLeft";
            this.tbTopLeft.ReadOnly = true;
            this.tbTopLeft.Size = new System.Drawing.Size(100, 23);
            this.tbTopLeft.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Rectangle";
            // 
            // pnlControls
            // 
            this.pnlControls.Controls.Add(this.btnCopy);
            this.pnlControls.Controls.Add(this.label1);
            this.pnlControls.Controls.Add(this.btnCapture);
            this.pnlControls.Controls.Add(this.btnReset);
            this.pnlControls.Controls.Add(this.tbCurrent);
            this.pnlControls.Controls.Add(this.tbBottomRight);
            this.pnlControls.Controls.Add(this.tbTopLeft);
            this.pnlControls.Controls.Add(this.label3);
            this.pnlControls.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlControls.Location = new System.Drawing.Point(0, 0);
            this.pnlControls.Name = "pnlControls";
            this.pnlControls.Size = new System.Drawing.Size(704, 42);
            this.pnlControls.TabIndex = 3;
            // 
            // pnlGraphics
            // 
            this.pnlGraphics.AutoScroll = true;
            this.pnlGraphics.Controls.Add(this.pbFrame);
            this.pnlGraphics.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlGraphics.Location = new System.Drawing.Point(0, 42);
            this.pnlGraphics.Name = "pnlGraphics";
            this.pnlGraphics.Size = new System.Drawing.Size(704, 409);
            this.pnlGraphics.TabIndex = 4;
            // 
            // GrabViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(704, 451);
            this.Controls.Add(this.pnlGraphics);
            this.Controls.Add(this.pnlControls);
            this.MinimumSize = new System.Drawing.Size(720, 490);
            this.Name = "GrabViewer";
            this.Text = "GrabViewer";
            ((System.ComponentModel.ISupportInitialize)(this.pbFrame)).EndInit();
            this.pnlControls.ResumeLayout(false);
            this.pnlControls.PerformLayout();
            this.pnlGraphics.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pbFrame;
        private System.Windows.Forms.Button btnCapture;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.TextBox tbBottomRight;
        private System.Windows.Forms.TextBox tbTopLeft;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbCurrent;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.Panel pnlControls;
        private System.Windows.Forms.Panel pnlGraphics;
    }
}