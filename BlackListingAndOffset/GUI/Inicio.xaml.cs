using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using BlackListingAndOffset.resources;
using BlackListingAndOffset.GUI;

namespace BlackListingAndOffset.GUI
{
    /// <summary>
    /// Lógica de interacción para InicioControl.xaml
    /// </summary>
    public partial class InicioControl : UserControl
    {
        private List<StringWorkArround> lnBtsInputGrid;
        private TabControlBlnOFF parentTabControl;
        public InicioControl()
        {
            //acceso al objeto que permite crear nuevas pesatañas, la ejecucion de este metodo lleva a crear unas cuantas
            parentTabControl = (TabControlBlnOFF)this.Parent;

            lnBtsInputGrid = new List<StringWorkArround>();
            InitializeComponent();
            
            for(int i = 0; i<15; i++)
            {
                lnBtsInputGrid.Add(new StringWorkArround { lnBtsName = "" });
            }
            lnBtsVisualGrid.ItemsSource = lnBtsInputGrid;
        }
        

        private void R31_Click(object sender, RoutedEventArgs e)
        {
            RSLTE31_path.Text = ZOTUtiles.FileFinder("Archivos CSV |*csv", "Consulta RSLTE31");
        }
        private void TA_Click(object sender, RoutedEventArgs e)
        {
            TA_path.Text = ZOTUtiles.FileFinder("Archivos CSV |*csv", "Timming Advance");
        }
        private void SRAN_Click(object sender, RoutedEventArgs e)
        {
            SRAN_path.Text = ZOTUtiles.FileFinder("Access data base |*mdb", "Export SRAN");
        }
        private void FL18_Click(object sender, RoutedEventArgs e)
        {
            FL18_path.Text = ZOTUtiles.FileFinder("Access data base |*mdb", "Export FL18");
        }

        private void Launch(object sender, RoutedEventArgs e)
        {
            Stopwatch globalWatch = new Stopwatch();
            globalWatch.Start();

            //Al tener que usar un wraper para poder pasar una lista de strings al Data grid ahora hay que hacer esta movida para recuperarlo
            //"ENB_O_AVILES_MAGDALENA_CT_01","ENB_PO_SAN_VICENTE_EB_01" ->prueba
            String[] aux = new String[lnBtsInputGrid.Count];
            int i;
            for (i = 0; i < 15; i++)
            {
                if (lnBtsInputGrid[i].lnBtsName == "")
                    break;
                aux[i] = lnBtsInputGrid[i].lnBtsName;
            }
            string[] lnBtsInputs = new string[i];
            for(int j = 0;j<i;j++)
            {
                lnBtsInputs[j] = aux[j];
            }
            aux = null;

            RSLTE31 R31 = new RSLTE31(lnBtsInputs, RSLTE31_path.Text);
            TimingAdvance TA = new TimingAdvance(lnBtsInputs,TA_path.Text);
            lnBtsInputs = null;

            Exports export = new Exports(TA.GetColumn("LNCEL name"),SRAN_path.Text,FL18_path.Text);

            Parallel.ForEach(export.data.AsEnumerable(), dataRow =>
            {
                parentTabControl.colindancias.CheckColin(dataRow, R31);
            });

            /*foreach (DataRow dataRow in export.data.Rows)
            {
                colindancias.CheckColin(dataRow, R31);
            }*/

            /*foreach (object[] line in colindancias.colinLines)
            {
                foreach (object item in line)
                {
                    Console.Write(item + "  -  ");
                }
                Console.Write('\n');
            }*/

            globalWatch.Stop();
            Console.WriteLine("Global time: " + (double)globalWatch.ElapsedMilliseconds / 1000.0 + "s");
        }


    }
}
