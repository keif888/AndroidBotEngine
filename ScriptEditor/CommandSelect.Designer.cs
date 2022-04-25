
namespace ScriptEditor
{
    partial class CommandSelect
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
            this.lbCommands = new System.Windows.Forms.ListBox();
            this.rtbCommandHelp = new System.Windows.Forms.RichTextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lbCommands
            // 
            this.lbCommands.FormattingEnabled = true;
            this.lbCommands.ItemHeight = 15;
            this.lbCommands.Items.AddRange(new object[] {
            "Click",
            "ClickWhenNotFoundInArea",
            "Drag",
            "Exit",
            "EnterLoopCoordinate",
            "FindClick",
            "FindClickAndWait",
            "IfExists",
            "IfNotExists",
            "LoopCoordinates",
            "LoopUntilFound",
            "LoopUntilNotFound",
            "Restart",
            "RunAction",
            "Sleep",
            "StartGame",
            "StopGame",
            "WaitFor",
            "WaitForThenClick",
            "WaitForChange",
            "WaitForNoChange"});
            this.lbCommands.Location = new System.Drawing.Point(12, 12);
            this.lbCommands.Name = "lbCommands";
            this.lbCommands.ScrollAlwaysVisible = true;
            this.lbCommands.Size = new System.Drawing.Size(269, 424);
            this.lbCommands.TabIndex = 0;
            this.lbCommands.SelectedIndexChanged += new System.EventHandler(this.lbCommands_SelectedIndexChanged);
            // 
            // rtbCommandHelp
            // 
            this.rtbCommandHelp.Location = new System.Drawing.Point(288, 13);
            this.rtbCommandHelp.Name = "rtbCommandHelp";
            this.rtbCommandHelp.ReadOnly = true;
            this.rtbCommandHelp.Size = new System.Drawing.Size(500, 396);
            this.rtbCommandHelp.TabIndex = 1;
            this.rtbCommandHelp.Text = "";
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(631, 415);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(712, 415);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // CommandSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.rtbCommandHelp);
            this.Controls.Add(this.lbCommands);
            this.Name = "CommandSelect";
            this.Text = "CommandSelect";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lbCommands;
        private System.Windows.Forms.RichTextBox rtbCommandHelp;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}