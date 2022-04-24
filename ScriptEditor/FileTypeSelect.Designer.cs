
namespace ScriptEditor
{
    partial class FileTypeSelect
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbDeviceConfig = new System.Windows.Forms.RadioButton();
            this.rbListConfig = new System.Windows.Forms.RadioButton();
            this.rbGameConfig = new System.Windows.Forms.RadioButton();
            this.button1 = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbDeviceConfig);
            this.groupBox1.Controls.Add(this.rbListConfig);
            this.groupBox1.Controls.Add(this.rbGameConfig);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(301, 160);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Config File Type";
            // 
            // rbDeviceConfig
            // 
            this.rbDeviceConfig.AutoSize = true;
            this.rbDeviceConfig.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.rbDeviceConfig.Location = new System.Drawing.Point(7, 104);
            this.rbDeviceConfig.Name = "rbDeviceConfig";
            this.rbDeviceConfig.Size = new System.Drawing.Size(273, 49);
            this.rbDeviceConfig.TabIndex = 2;
            this.rbDeviceConfig.TabStop = true;
            this.rbDeviceConfig.Text = "Device Config - Contains the Last Action Times\r\n                             and " +
    "Enable/Disable and \r\n                             schedule overides";
            this.rbDeviceConfig.UseVisualStyleBackColor = true;
            this.rbDeviceConfig.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // rbListConfig
            // 
            this.rbListConfig.AutoSize = true;
            this.rbListConfig.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.rbListConfig.Location = new System.Drawing.Point(7, 63);
            this.rbListConfig.Name = "rbListConfig";
            this.rbListConfig.Size = new System.Drawing.Size(257, 34);
            this.rbListConfig.TabIndex = 1;
            this.rbListConfig.TabStop = true;
            this.rbListConfig.Text = "List Config -      Contains the lists of\r\n                             coordinate" +
    "s for Commands";
            this.rbListConfig.UseVisualStyleBackColor = true;
            this.rbListConfig.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // rbGameConfig
            // 
            this.rbGameConfig.AutoSize = true;
            this.rbGameConfig.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.rbGameConfig.Location = new System.Drawing.Point(7, 23);
            this.rbGameConfig.Name = "rbGameConfig";
            this.rbGameConfig.Size = new System.Drawing.Size(290, 34);
            this.rbGameConfig.TabIndex = 0;
            this.rbGameConfig.TabStop = true;
            this.rbGameConfig.Text = "Game Config - Contains Actions, Commands and \r\n                           Find St" +
    "rings to drive the bot.";
            this.rbGameConfig.UseVisualStyleBackColor = true;
            this.rbGameConfig.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Location = new System.Drawing.Point(239, 179);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(158, 178);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // FileTypeSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(330, 210);
            this.ControlBox = false;
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox1);
            this.Name = "FileTypeSelect";
            this.Text = "File Type Select";
            this.TopMost = true;
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbDeviceConfig;
        private System.Windows.Forms.RadioButton rbListConfig;
        private System.Windows.Forms.RadioButton rbGameConfig;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnOk;
    }
}