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
    public partial class TabalRelleno : Form
    {

        string respuesta = "";
        public TabalRelleno(string tipo)
        {
            InitializeComponent();
            labelTipo.Text = tipo;
            this.TopMost = true;
        }

        public string Tipo { get; }
        
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBoxIntegracion_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            respuesta = comboBox.GetItemText(this.comboBox.SelectedItem);
            //Console.WriteLine(respuesta);
        }
        public string respuestaComboBox()
        {
            return respuesta;
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void TabalRelleno_Load(object sender, EventArgs e)
        {
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            comboBox.SelectedIndex = 2;
            respuesta = comboBox.GetItemText(this.comboBox.SelectedItem);
            //Console.WriteLine(respuesta);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
