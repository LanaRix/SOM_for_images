using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImagesRecovery3.View.Forms
{
    public partial class Form1 : Form
    {
        int t;
        bool use;
        public Form1()
        {
            InitializeComponent();
            t = 100;
            use = false;
        }

        public int T { get => t; set => t = value; }
        public bool Use { get => use; set => use = value; }

        private void button1_Click(object sender, EventArgs e)
        {
            t = Convert.ToInt32(textBox1.Text);
            this.Close();
            use = true;
        }
    }
}
