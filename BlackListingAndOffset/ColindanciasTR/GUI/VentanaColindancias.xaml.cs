using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ZOT.ColindanciasTR.Code;

namespace ZOT.Colindancias.GUI
{
    /// <summary>
    /// Lógica de interacción para VentanaColindancias.xaml
    /// </summary>
    public partial class VentanaColindancias : Window
    {
        public VentanaColindancias()
        {
            InitializeComponent();
        }


        Main m = null;

        OpenFileDialog ofd = new OpenFileDialog();



        private void abrirTR(object sender, RoutedEventArgs e)
        {

            bool TRCorrecto = false;
            bool ExportNorteCorrecto = false;


            // 1. Cargar datos de la TR para actualizar
            ofd.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
            ofd.Title = "Selecciona la Tabla de Relaciones ";
            String nombreTR = "";

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                nombreTR = ofd.FileName;
                TRCorrecto = true;
            }
            else
            {
                return;
            }

            Console.WriteLine("Se ha leido el fichero de TR: " + nombreTR);


            // 2. Cargar el export
            ofd.Filter = "Access Files|*.accdb;*.mdb";
            ofd.Title = "Selecciona el Export de Norte ";
            String nombreExportNorte = "";

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                nombreExportNorte = ofd.FileName;
                ExportNorteCorrecto = true;


            }
            else
            {
                return;
            }


            //Si las rutas son correctas las paso al main
            if(TRCorrecto && ExportNorteCorrecto)
            {
                m = new Main(nombreTR, nombreExportNorte);
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Problemas abriendo TR o Export");
            }





            String a = "";






        }



    }


   


}
