using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tester
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }


        private void button4_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            new VisibleRangeChangedDelayedSample().Show();
            Cursor = Cursors.Default;
        }



        private void button12_Click(object sender, EventArgs e)
        {
            new PowerfulCSharpEditor().Show();
        }

    }
}
