using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using ZOT.resources;
using ZOT.BLnOFF.Code;

namespace ZOT.BLnOFF.GUI
{
    //Todo el flujo de la aplicacion se controla desde esta clase
    public partial class TabControlBlnOFF : UserControl
    {
        private List<StringWorkArround> lnBtsInputGrid;
        public Colindancias colindancias;

        public TabControlBlnOFF()
        {
            //Cargar las constantes y umbrales que se usan para hacer evaluaciones en toda la aplicación
            ZOT.BLnOFF.Code.CONSTANTS.LoadConst();

            lnBtsInputGrid = new List<StringWorkArround>();
            colindancias = new Colindancias();
            InitializeComponent();
            for (int i = 0; i < 15; i++)
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
        //funcion que simplemente numera las filas de la tabla
        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void Constant_Editor(object sender, RoutedEventArgs e)
        {
            ConstantEditorBLnOFF constant_editor = new ConstantEditorBLnOFF();
            constant_editor.Show();
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
            for (int j = 0; j < i; j++)
            {
                lnBtsInputs[j] = aux[j];
            }
            aux = null;

            RSLTE31 R31 = new RSLTE31(lnBtsInputs, RSLTE31_path.Text);
            TimingAdvance TA = new TimingAdvance(lnBtsInputs, TA_path.Text);

            globalWatch.Stop();
            Console.WriteLine("R31+TA time: " + (double)globalWatch.ElapsedMilliseconds / 1000.0 + "s");
            globalWatch.Start();
            lnBtsInputs = null;

            Exports export = new Exports(TA.GetColumn("LNCEL name"), SRAN_path.Text, FL18_path.Text);

            globalWatch.Stop();
            Console.WriteLine("Export time: " + (double)globalWatch.ElapsedMilliseconds / 1000.0 + "s");
            globalWatch.Start();

            Parallel.ForEach(export.data.AsEnumerable(), dataRow =>
            {
                colindancias.CheckColin(dataRow, R31);
            });
            Parallel.ForEach(R31.NotInExports().AsEnumerable(), dataRow =>
            {
                colindancias.CheckColinsNotInExports(dataRow);
            });

            globalWatch.Stop();
            Console.WriteLine("Parrallel colin time: " + (double)globalWatch.ElapsedMilliseconds / 1000.0 + "s");
            globalWatch.Start();
            colindancias.addENBID();
            colinGrid.ItemsSource = colindancias.data.DefaultView;
            
            CandidatesBL candBL = new CandidatesBL(colindancias);
            candBLGrid.ItemsSource = candBL.data.DefaultView;
            globalWatch.Stop();
            Console.WriteLine("Global time: " + (double)globalWatch.ElapsedMilliseconds / 1000.0 + "s");
        }

    }
    public class StringWorkArround
    {
        public string lnBtsName { get; set; }
    }
}