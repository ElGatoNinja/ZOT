using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using ZOT.resources;
using ZOT.BLnOFF.Code;

namespace ZOT.BLnOFF.GUI
{
    //Todo el flujo de la herramienta se controla desde esta clase
    public partial class TabControlBLnOFF : UserControl
    {
        private List<StringWorkArround> lnBtsInputGrid;

        public TabControlBLnOFF()
        {
            //Cargar las constantes y umbrales que se usan para hacer evaluaciones en toda la aplicación
            ZOT.BLnOFF.Code.CONSTANTS.LoadConst();

            lnBtsInputGrid = new List<StringWorkArround>();
            InitializeComponent();
            for (int i = 0; i < 50; i++)
            {
                lnBtsInputGrid.Add(new StringWorkArround { LnBtsName = "" });
            }
            lnBtsVisualGrid.ItemsSource = lnBtsInputGrid;

            //Recuperar ultimos paths usados
            try
            {
                string[] storedPaths = System.IO.File.ReadAllLines(Path.Combine(Environment.CurrentDirectory, @"BlnOFF\Data\", "RememberPaths.txt"));
                RSLTE31_path.Text = storedPaths[0];
                TA_path.Text = storedPaths[1];
                SRAN_path.Text = storedPaths[2];
                FL18_path.Text = storedPaths[3];
                NIR_path.Text = storedPaths[4];
            }
            catch (FileNotFoundException) { /*comportamiento aceptable si no existe ya se creará */}

        }

        private void R31_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RSLTE31_path.Text = resources.ZOTlib.Files.FileFinder("Archivos CSV |*csv", "Consulta RSLTE31", Path.GetDirectoryName(RSLTE31_path.Text));
            }
            catch (Exception)
            {
                RSLTE31_path.Text = resources.ZOTlib.Files.FileFinder("Archivos CSV |*csv", "Consulta RSLTE31");
            }
        }
        private void TA_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TA_path.Text = resources.ZOTlib.Files.FileFinder("Archivos CSV |*csv", "Timming Advance", Path.GetDirectoryName(TA_path.Text));
            }
            catch (Exception)
            {
                TA_path.Text = resources.ZOTlib.Files.FileFinder("Archivos CSV |*csv", "Timming Advance");
            }
        }
        private void SRAN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SRAN_path.Text = resources.ZOTlib.Files.FileFinder("Access data base |*mdb", "Export SRAN", Path.GetDirectoryName(SRAN_path.Text));
            }
            catch(Exception)
            {
                SRAN_path.Text = resources.ZOTlib.Files.FileFinder("Access data base |*mdb", "Export SRAN");
            }
           
        }
        private void FL18_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FL18_path.Text = resources.ZOTlib.Files.FileFinder("Access data base |*mdb", "Export FL18", Path.GetDirectoryName(SRAN_path.Text));
            }
            catch(Exception)
            {
                FL18_path.Text = resources.ZOTlib.Files.FileFinder("Access data base |*mdb", "Export FL18");
            }
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
            string output_path = resources.ZOTlib.Files.SetDirectory("Seleciona el directorio en el que guardar la plantilla generada");
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
            string output_path = resources.ZOTlib.Files.SetDirectory("Seleciona el directorio en el que guardar la plantilla generada");
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
                //"ENB_PO_SAN_VICENTE_EB_01", "ENB_AV_BURGOHONDO_01" ->prueba
                //prueba_ 2 -> ENB_CA_BENALUP_EB_01, ENB_GR_ZAFARRAYA_01, ENB_J_JODAR_ALMAZARA_01, ENB_SE_SUPRANORTE_01 
                Colindancias colindancias = new Colindancias();

                String[] aux = new String[lnBtsInputGrid.Count];
                int n = 0;
                for (int i = 0; i < 49; i++)
                {
                    if (lnBtsInputGrid[i].LnBtsName != "")
                    {
                        aux[i] = lnBtsInputGrid[i].LnBtsName;
                        n++;
                    }
                }
                string[] lnBtsInputs = new string[n];
                for (int i = 0; i < n; i++)
                {
                    lnBtsInputs[i] = aux[i];
                }
                aux = null;

                //Guardar el path de los ultimos archivos en un fichero de texto
                string[] storePaths = new string[5] { RSLTE31_path.Text, TA_path.Text, SRAN_path.Text, FL18_path.Text, NIR_path.Text};
                System.IO.File.WriteAllLines(Path.Combine(Environment.CurrentDirectory, @"BlnOFF\Data\", "RememberPaths.txt"), storePaths);

                if ((bool)Is_BlnOFF_Enabled.IsChecked) 
                {
                    //Se crean objetos que albergan las tablas de datos que se necesitan en esta herramienta
                    RSLTE31 R31 = new RSLTE31(lnBtsInputs, RSLTE31_path.Text);
                    TimingAdvance TA = new TimingAdvance(lnBtsInputs, TA_path.Text);
                    Exports export = new Exports(TA.GetColumn("LNCEL name"), SRAN_path.Text, FL18_path.Text);

                    Parallel.ForEach(export.data.AsEnumerable(), dataRow =>
                    {
                        colindancias.CheckColin(dataRow, R31);
                    });
                    Parallel.ForEach(R31.NotInExports().AsEnumerable(), dataRow =>
                    {
                        colindancias.CheckColinsNotInExports(dataRow);
                    });
                    /*
                    foreach(DataRow dataRow in export.data.Rows)
                    {
                        colindancias.CheckColin(dataRow, R31);
                    }
                    foreach(DataRow dataRow in R31.NotInExports())
                    {
                        colindancias.CheckColinsNotInExports(dataRow);
                    }
                    */

                    colindancias.AddENBID();
                    DataView dv = colindancias.data.DefaultView;
                    dv.Sort = "[HO errores SR] DESC";
                    colindancias.data = dv.ToTable();
                    colinGrid.WorkingData = colindancias.data;

                    //Se calculan las candidatas para BlackListing y para Offset, que quedaran disponibles para la edicion manual del usuario en la interfaz grafica
                    CandidatesBL candBL = new CandidatesBL(colindancias);
                    dv = candBL.data.DefaultView;
                    dv.Sort = "[HO errores SR] DESC";
                    candBL.data = dv.ToTable();
                    candBLGrid.WorkingData = candBL.data;

                    CandidatesOFF candOFF = new CandidatesOFF(TA, colindancias, candBL);
                    dv = candOFF.data.DefaultView;
                    dv.Sort = "[HO errores SR] DESC";
                    candOFF.data = dv.ToTable();
                    candOFFGrid.WorkingData = candOFF.data;
                }

                if((bool)Is_PrevAnalysis_Enabled.IsChecked)
                {

                }

#if DEBUG
                globalWatch.Stop();
                Console.WriteLine("Global time: " + (double)globalWatch.ElapsedMilliseconds / 1000.0 + "s");
#endif
            }
            catch(Exception ex)
            {
                resources.ZOTlib.ShowError("Algo ha ido mal e la ejecucion:\n\n " + ex.Message);
            }
        }
    }

    public class StringWorkArround
    {
        public string LnBtsName { get; set; }
    }
}