using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace protect_system
{
    public partial class UserPreview : UserControl
    {
        public UserPreview()
        {
            InitializeComponent();
        }

        private void label1_MouseHover(object sender, EventArgs e)
        {
            this.BackColor = System.Drawing.ColorTranslator.FromHtml("#4EA6EA");
            this.ForeColor = Color.Black;
        }
    }
}
