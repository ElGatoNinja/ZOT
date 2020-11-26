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
    public partial class borrradosporque : Form
    {


        Dictionary<string, string> borradasProque = null;
        Dictionary<string, string> sinEstadoenEjecucion = null;
        


        public borrradosporque(Dictionary<string, string> borradosporque, Dictionary<string, string> sinEstadoenEjecucion)
        {

            this.borradasProque = borradosporque;
            this.sinEstadoenEjecucion = sinEstadoenEjecucion;
            InitializeComponent();


        }

        private void borrradosporque_Load(object sender, EventArgs e)
        {

            dataGridViewporque.DataSource = (from d in this.borradasProque orderby d.Value select new { d.Key, d.Value }).ToList();
            dataGridViewporque.Columns[0].HeaderText = "COC";
            dataGridViewporque.Columns[1].HeaderText = "Borrado Porque";

            dataGridViewfaltanEstados.DataSource = (from d in this.sinEstadoenEjecucion orderby d.Value select new { d.Key, d.Value }).ToList();
            dataGridViewfaltanEstados.Columns[0].HeaderText = "TIPO";
            dataGridViewfaltanEstados.Columns[1].HeaderText = "ESTADO";




        }
    }
}
