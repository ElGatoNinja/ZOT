using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZOT.HORAS48.GUI
{
    public partial class estado48 : Form
    {
        public estado48()
        {
            InitializeComponent();
        }
        
        public estado48(string estado)
        {
            InitializeComponent();
            labelejecutando.Text = estado;
            this.TopMost = true;
        }


        private void estado48_Load(object sender, EventArgs e)
        {

        }
    }
}
