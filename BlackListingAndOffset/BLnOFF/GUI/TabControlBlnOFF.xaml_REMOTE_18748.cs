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
    //Todo el flujo de la herramienta se controla desde esta clase
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
            for (int i = 0; i < 50; i++)
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
        //simplemente numera las filas de las tablas que incluyan este evento
        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void Constant_Editor(object sender, RoutedEventArgs e)
        {
            ConstantEditorBLnOFF constant_editor = new ConstantEditorBLnOFF();
            constant_editor.Show();
        }

        private void BL_template_gen(object sender, RoutedEventArgs e)
        {
            DataTable data = ((DataView)candBLGrid.ItemsSource).ToTable();
            string output_path = ZOTUtiles.SetDirectory("Seleciona el directorio en el que guardar la plantilla generada");
            using (StreamWriter writer = new StreamWriter(output_path + "\\" + fileNameBL.Text))
            {                
                writer.WriteLine("Objeto;mrbtsId;lnBtsId;lnCelId;lnRelId;handowerAllowed;removeAllowed;;");
                foreach (DataRow row in data.Rows)
                {
                    if ((bool)row["SelectedBL"])
                    {
                        string toCSV = "LNREL;";
                        toCSV += (string)row["ENBID SOURCE"] + ";";
                        toCSV += (string)row["ENBID SOURCE"] + ";";
                        toCSV += row["LnCell SOURCE"].ToString() + ";";
                        if ((string)row["Label"] != "")
                            toCSV += ((string)row["Label"]).Split('-')[1] + ";";
                        else
                            toCSV += ";";
                        toCSV += "1";
                        toCSV += "0";
                        toCSV += (string)row["Name SOURCE"] + ";";
                        toCSV += (string)row["Name TARGET"];

                        writer.WriteLine(toCSV);
                    }
                }
            }
        }

        private void OFF_template_gen(object sender, RoutedEventArgs e)
        {
            DataTable data = ((DataView)candOFFGrid.ItemsSource).ToTable();
            string output_path = ZOTUtiles.SetDirectory("Seleciona el directorio en el que guardar la plantilla generada");
            using (StreamWriter writer = new StreamWriter(output_path + "\\" + fileNameOFF.Text))
            {
                writer.WriteLine("Objeto;mrbtsId;lnBtsId;lnCelId;lnRelId;cellIndOffNeigh;;");
                foreach (DataRow row in data.Rows)
                {
                    if ((bool)row["SelectedOFF"])
                    {
                        string toCSV = "LNREL;";
                        toCSV += (string)row["ENBID SOURCE"] + ";";
                        toCSV += (string)row["ENBID SOURCE"] + ";";
                        toCSV += row["LnCell SOURCE"].ToString() + ";";

                        if ((string)row["Label"] != "")
                            toCSV += ((string)row["Label"]).Split('-')[1] + ";";
                        else
                            toCSV += ";";

                        if ((double)row["Offset"] > 9)
                            toCSV += ((double)row["Offset"] - 3).ToString() + ";";
                        else
                            toCSV += ((double)row["Offset"] - 1).ToString() + ";";

                        toCSV += (string)row["Name SOURCE"] + ";";
                        toCSV += (string)row["Name TARGET"];

                        writer.WriteLine(toCSV);
                    }
                }
            }
        }


        private void Launch(object sender, RoutedEventArgs e)
        {
            try
            {
#if DEBUG
                Stopwatch globalWatch = new Stopwatch();
                globalWatch.Start();
#endif
                //Al tener que usar un wraper para poder pasar una lista de strings al Data grid ahora hay que hacer esta movida para recuperarlo
                //"ENB_O_AVILES_MAGDALENA_CT_01","ENB_PO_SAN_VICENTE_EB_01" ->prueba
                String[] aux = new String[lnBtsInputGrid.Count];
                int i;
                for (i = 0; i < 50; i++)
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

                //Se crean objetos que albergan las tablas de datos que se necesitan en esta herramienta
                RSLTE31 R31 = new RSLTE31(lnBtsInputs, RSLTE31_path.Text);
                TimingAdvance TA = new TimingAdvance(lnBtsInputs, TA_path.Text);
                Exports export = new Exports(TA.GetColumn("LNCEL name"), SRAN_path.Text, FL18_path.Text);

                /*Procesado paralelo de cada una de las colindancias, ya que son independientes salvo en la escritura, que está sincronizada
                Parallel.ForEach(export.data.AsEnumerable(), dataRow =>
                {
                    colindancias.CheckColin(dataRow, R31);
                });
                Parallel.ForEach(R31.NotInExports().AsEnumerable(), dataRow =>
                {
                    colindancias.CheckColinsNotInExports(dataRow);
                });
                */
                foreach(DataRow dataRow in export.data.Rows)
                {
                    colindancias.CheckColin(dataRow, R31);
                }
                foreach(DataRow dataRow in R31.NotInExports())
                {
                    colindancias.CheckColinsNotInExports(dataRow);
                }

                colindancias.AddENBID();
                colinGrid.ItemsSource = colindancias.data.DefaultView;

                //Se calculan las candidatas para BlackListing y para Offset, que quedaran disponibles para la edicion manual del usuario en la interfaz grafica
                CandidatesBL candBL = new CandidatesBL(colindancias);
                candBLGrid.ItemsSource = candBL.data.DefaultView;
                CandidatesOFF candOFF = new CandidatesOFF(TA, colindancias, candBL);
                candOFFGrid.ItemsSource = candOFF.data.DefaultView;

#if DEBUG
                globalWatch.Stop();
                Console.WriteLine("Global time: " + (double)globalWatch.ElapsedMilliseconds / 1000.0 + "s");
#endif
            }
            catch(Exception ex)
            {
                ZOTUtiles.ShowError("Algo ha ido mal e la ejecucion:\n\n " + ex.Message);
            }
        }
     }


    public class StringWorkArround
    {
        public string lnBtsName { get; set; }
    }
}