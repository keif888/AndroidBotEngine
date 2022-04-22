using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ScriptEditor
{
    public partial class JsonErrors : Form
    {
        public JsonErrors()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void setText(StringBuilder sb)
        {
            tbJsonErrors.Text = sb.ToString();
        }
    }
}
