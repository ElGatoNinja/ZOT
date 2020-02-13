using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using ZOT.BLnOFF.Code;
using ZOT.resources.ZOTlib;
using ZOT.GUI;
using System.ComponentModel;
using System.Windows.Input;
using System.Diagnostics;
using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;

namespace ZOT.BLnOFF.GUI
{
    //Todo el flujo de la herramienta se controla desde esta clase
    public partial class TabControlBLnOFF : UserControl ,IZotApp
    {
        private List<StringWorkArround> lnBtsInputGrid;

        #region IZOTAPP
        public string AppName
        {
            get { return "BlackListing And Offset"; }
        }
        public bool Notify
        {
            get { return false; }
            set { value = false; }
        }
        #endregion

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

            //relacciona las funciones que permiten pegar celdas por lotes con la grid de inputs
            CommandBinding PasteCmdBinding = new CommandBinding(ApplicationCommands.Paste,PasteExecuted);
            lnBtsVisualGrid.CommandBindings.Add(PasteCmdBinding);

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

        private void PasteExecuted(object target, ExecutedRoutedEventArgs e)
        {
            string clipBoardText = Clipboard.GetText();
            int index = ((List<StringWorkArround>)((DataGrid)target).ItemsSource).IndexOf((StringWorkArround)((DataGrid)target).SelectedCells[0].Item);

            string[] rows = clipBoardText.Split(new[] { "\r\n" }, StringSplitOptions.None);
            for (int i = 0; i < rows.Length; i++)
            {
                string value = rows[i].Split('\t')[0].Trim();
                lnBtsInputGrid[index + i].LnBtsName = value; 
            }
        }

        private void R31_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RSLTE31_path.Text = ZOTFiles.FileFinder("Archivos CSV |*csv", "Consulta RSLTE31", Path.GetDirectoryName(RSLTE31_path.Text));
            }
            catch (Exception)
            {
                RSLTE31_path.Text =ZOTFiles.FileFinder("Archivos CSV |*csv", "Consulta RSLTE31");
            }
        }
        private void TA_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TA_path.Text = ZOTFiles.FileFinder("Archivos CSV |*csv", "Timming Advance", Path.GetDirectoryName(TA_path.Text));
            }
            catch (Exception)
            {
                TA_path.Text = ZOTFiles.FileFinder("Archivos CSV |*csv", "Timming Advance");
            }
        }

        private void NIR_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NIR_path.Text = ZOTFiles.FileFinder("Archivos CSV |*csv", "NIR 48H Completa", Path.GetDirectoryName(NIR_path.Text));
            }
            catch (Exception)
            {
                NIR_path.Text = ZOTFiles.FileFinder("Archivos CSV |*csv", "NIR 48H Completa");
            }
        }

        private void SRAN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SRAN_path.Text = ZOTFiles.FileFinder("Access data base |*mdb", "Export SRAN", Path.GetDirectoryName(SRAN_path.Text));
            }
            catch(Exception)
            {
                SRAN_path.Text = ZOTFiles.FileFinder("Access data base |*mdb", "Export SRAN");
            }
           
        }
        private void FL18_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FL18_path.Text = ZOTFiles.FileFinder("Access data base |*mdb", "Export FL18", Path.GetDirectoryName(SRAN_path.Text));
            }
            catch(Exception)
            {
                FL18_path.Text = ZOTFiles.FileFinder("Access data base |*mdb", "Export FL18");
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
            string output_path = ZOTFiles.FileSaver("Archivos CSV |*csv","csv","Exportando plantilla de Blacklisting");
            using (StreamWriter writer = new StreamWriter(output_path))
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
                        toCSV += "1;";
                        toCSV += "0;";
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
            string output_path = ZOTFiles.FileSaver("Archivos CSV |*csv", "csv", "Exportando plantilla de Offset");
            using (StreamWriter writer = new StreamWriter(output_path))
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
        struct bgwBlnOffArgument
        {
            public string[] lnBtsInputs;
            public string pathR31;
            public string pathTA;
            public string pathSRAN;
            public string pathFL18;
            public string pathNIR48;
            public bool BLnOffEnabled;
            public bool prevAnalisysEnabled;
        }
        struct bgwBlnOffResult
        {
            public DataTable colindancias;
            public DataTable candBl;
            public DataTable candOff;
            public DataTable error;
            public string siteCoordErrors;
        }

        public struct bgwBlnOffProgress
        {
            public Exception exception;
            public string errorTitle;
            public string errorInfo;
            public int progres;
            public string progresInfo;
        }

        private void Launch(object sender, RoutedEventArgs e)
        {
            String[] aux = new String[lnBtsInputGrid.Count];
            int n = 0;
            for (int i = 0; i < lnBtsInputGrid.Count; i++)
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

            if (lnBtsInputs.Length == 0)
            {
                WPFForms.ShowError("No hay nodos de entrada", "Rellena la tabla de INPUT SITES");
                return;
            }
            //se preparan los inputs para pasarlos como argumento al background worker
            bgwBlnOffArgument args = new bgwBlnOffArgument();
            args.lnBtsInputs = lnBtsInputs;
            args.pathR31 = RSLTE31_path.Text;
            args.pathTA = TA_path.Text;
            args.pathSRAN = SRAN_path.Text;
            args.pathFL18 = FL18_path.Text;
            args.pathNIR48 = NIR_path.Text;
            args.BLnOffEnabled = (bool)Is_BlnOFF_Enabled.IsChecked;
            args.prevAnalisysEnabled = (bool)Is_PrevAnalysis_Enabled.IsChecked;

            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += BackGround_Work;
            worker.ProgressChanged += BackGround_Progress;
            worker.RunWorkerCompleted += BackGround_Completed;
            worker.RunWorkerAsync(args);

            //Task<ProgressDialogController> progressBar = WPFForms.ShowProgress("Ejecutando BlackListing y Offset","Procesando consulta 31");

            //Guardar el path de los ultimos archivos en un fichero de texto
            string[] storePaths = new string[5] { RSLTE31_path.Text, TA_path.Text, SRAN_path.Text, FL18_path.Text, NIR_path.Text };
            System.IO.File.WriteAllLines(Path.Combine(Environment.CurrentDirectory, @"BlnOFF\Data\", "RememberPaths.txt"), storePaths);
        }

        //esta a huevo para implementar una barra de progreso
        private void BackGround_Progress(object sender, ProgressChangedEventArgs e)
        {
            
        }

        //tras acabar el procesado en segundo plano se actualiza la interfaz con ellos
        private void BackGround_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                bgwBlnOffResult output = (bgwBlnOffResult)e.Result;
                if (output.colindancias != null)
                {
                    colinGrid.WorkingData = output.colindancias;
                    WPFForms.FindParent<TabItem>(colinGrid).Visibility = Visibility.Visible;
                }

                if (output.siteCoordErrors != "" && (bool)Is_BlnOFF_Enabled.IsChecked)
                    WPFForms.ShowError("Faltan las coordenadas de los siguientes emplazamientos", output.siteCoordErrors);

                if (output.candBl != null)
                {
                    candBLGrid.WorkingData = output.candBl;
                    WPFForms.FindParent<TabItem>(candBLGrid).Visibility = Visibility.Visible;
                }
                if (output.candOff != null)
                {
                    candOFFGrid.WorkingData = output.candOff;
                    WPFForms.FindParent<TabItem>(candOFFGrid).Visibility = Visibility.Visible;
                }
                if (output.error != null)
                {
                    errGrid.WorkingData = output.error;
                    WPFForms.FindParent<TabItem>(errGrid).Visibility = Visibility.Visible;

                    graphObject.Errors = output.error;
                }


            }
            else
            {
                //control de errores en BackGround
                switch(e.Error.GetType().Name)
                {
                    case "FileNotFoundException":
                        WPFForms.ShowError("Falta un archivo", e.Error.Message);
                        break;
                    case "InvalidOperationException":
                        WPFForms.ShowError("Error critico", e.Error.Message);
                        break;
                    case "ArgumentException":
                        WPFForms.ShowError("Error en los datos", e.Error.Message);
                        break;
                    default:
                        WPFForms.ShowError("Error no controlado", e.Error.Message);
                        break;
                }
            }
        }

        //nunca se puede acceder a elementos de la interfaz directamente desde este metodo, acceder a la interfaz desde otro thread
        //bloquea la aplicacion
        private void BackGround_Work(object sender, DoWorkEventArgs e)
        {
#if DEBUG
            var totalTimer = new Stopwatch();
            totalTimer.Start();
            var timer = new Stopwatch();
            timer.Start();
#endif
            bgwBlnOffArgument args = (bgwBlnOffArgument)e.Argument;
            bgwBlnOffResult results = new bgwBlnOffResult();

            if (args.BLnOffEnabled)
            {

                //Se crean objetos que contienen las tablas de datos que se necesitan en esta herramienta
                Colindancias colindancias = new Colindancias();
                RSLTE31 R31 = new RSLTE31(args.lnBtsInputs, args.pathR31);
                R31.completeR31(args.pathSRAN, args.pathFL18);

#if DEBUG
                timer.Stop();
                Console.WriteLine("Completar consulta31: " + timer.Elapsed.ToString(@"m\:ss\.fff"));
                timer.Reset();
                timer.Start();
#endif

                TimingAdvance TA = new TimingAdvance(args.lnBtsInputs, args.pathTA);

#if DEBUG
                timer.Stop();
                Console.WriteLine("Sacar Timing Advance: " + timer.Elapsed.ToString(@"m\:ss\.fff"));
                timer.Reset();
                timer.Start();
#endif
                Exports export = new Exports(TA.GetColumn("LNCEL name"), args.pathSRAN, args.pathFL18);
#if DEBUG
                timer.Stop();
                Console.WriteLine("Sacar Exports: " + timer.Elapsed.ToString(@"m\:ss\.fff"));
                timer.Reset();
                timer.Start();
#endif
                    
                foreach(DataRow dataRow in export.data.Rows)
                {
                    colindancias.CheckColin(dataRow, R31);
                }
                foreach(DataRow dataRow in R31.NotInExports())
                {
                    colindancias.CheckColinsNotInExports(dataRow);
                }
                colindancias.AddENBID();

                //Dibujar tablas
                DataView dv = colindancias.data.DefaultView;
                dv.Sort = "[HO errores SR] DESC";
                colindancias.data = dv.ToTable();
                results.colindancias = colindancias.data;
                results.siteCoordErrors = colindancias.GetSiteCoordErr();
  

#if DEBUG
                timer.Stop();
                Console.WriteLine("Calcular Colindancias: " + timer.Elapsed.ToString(@"m\:ss\.fff"));
                timer.Reset();
                timer.Start();
#endif

                //Se calculan las candidatas para BlackListing y para Offset, que quedaran disponibles para la edicion manual del usuario en la interfaz grafica
                CandidatesBL candBL = new CandidatesBL(colindancias);
                dv = candBL.data.DefaultView;
                dv.Sort = "[HO errores SR] DESC";
                candBL.data = dv.ToTable();
                results.candBl = candBL.data;
#if DEBUG
                timer.Stop();
                Console.WriteLine("Calcular candidatas de blacklisting: " + timer.Elapsed.ToString(@"m\:ss\.fff"));
                timer.Reset();
                timer.Start();
#endif

                CandidatesOFF candOFF = new CandidatesOFF(TA, colindancias, candBL);
                dv = candOFF.data.DefaultView;
                dv.Sort = "[HO errores SR] DESC";
                candOFF.data = dv.ToTable();
                results.candOff = candOFF.data;

#if DEBUG
                timer.Stop();
                Console.WriteLine("Calcular candidatas de offset: " + timer.Elapsed.ToString(@"m\:ss\.fff"));
                timer.Reset();
                timer.Start();
#endif

            }
            if (args.prevAnalisysEnabled)
            {
                NIR48H nir = new NIR48H(args.lnBtsInputs, args.pathNIR48);
                results.error = nir.errors;
#if DEBUG
                timer.Stop();
                Console.WriteLine("Analisis Previo: " + timer.Elapsed.ToString(@"m\:ss\.fff"));
#endif
            }
            e.Result = results;
#if DEBUG
            timer.Stop();
            totalTimer.Stop();
            Console.WriteLine("Tiempo Total: " + totalTimer.Elapsed.ToString(@"m\:ss\.fff"));
#endif

        }

        private void Graphs_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }

    public class StringWorkArround : INotifyPropertyChanged
    {
        private string _value;
        public string LnBtsName
        {
            get { return _value; }
            set { _value = value; OnPropertyChanged("LnBtsName"); }        
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }
    }
}