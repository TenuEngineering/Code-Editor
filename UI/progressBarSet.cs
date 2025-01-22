using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;

namespace Tester
{
    public partial class progressBarSet : Form
    {
        public ProgressBar progressBar { get; set; }

        public progressBarSet()
        {
            InitializeComponent();
            progressBar = progressBar1;

            
        }
        
    }
}
