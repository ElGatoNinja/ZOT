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
using System.Linq;
using LiveCharts;
using LiveCharts.Wpf;
using System.Windows.Media;
using System.Windows.Data;
using System.Globalization;
using Xamarin.Forms.PlatformConfiguration;

namespace ZOT.BLnOFF.GUI
{ 
    //Todo el flujo de la herramienta se controla desde esta clase
    public partial class TabControlBLnOFF : UserControl, IZotApp
    {
        private readonly string[] interColorsLines = {"1A2C33","345766", "4E8399", "68AECC", "82DAFF" };
        private readonly string[] intraColorsLines = {"781B18", "9E231F", "BF2B26", "E6342E", "FF3A33"};
        private List<StringWorkArround> lnBtsInputGrid;

        //se usan para hacer las graficas
        private DataTable erroresAMejorar;
        private DataTable nirPlotData;
        public List<SectorListItem> Sectors { get; set; }

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
            CommandBinding PasteCmdBinding = new CommandBinding(ApplicationCommands.Paste, PasteExecuted);
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
                RSLTE31_path.Text = ZOTFiles.FileFinder("Archivos CSV |*csv", "Consulta RSLTE31");
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
            catch (Exception)
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
            catch (Exception)
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
            string output_path = ZOTFiles.FileSaver("Archivos CSV |*csv", "csv", "Exportando plantilla de Blacklisting");
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
            public DataTable nirPlotData;
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
                {   //se genera la tabla de analis previo
                    errGrid.WorkingData = output.error;
                    WPFForms.FindParent<TabItem>(errGrid).Visibility = Visibility.Visible;

                    //se genera la gárafica por nodos
                    this.erroresAMejorar = output.error;

                    List<string> sites = erroresAMejorar.AsEnumerable().Select(col => (string)col[1]).Distinct().ToList();
                    siteListBox_1.ItemsSource = sites;
                    siteListBox_1.SelectedItem = sites[0];

                    List<string> Tech = this.erroresAMejorar.AsEnumerable().Where(row => (string)row[1] == (string)siteListBox_1.SelectedItem).Select(col => (string)col[2]).Distinct().ToList<string>();
                    techListBox_1.ItemsSource = Tech;
                    techListBox_1.SelectedItem = Tech[0];

                    UpdateGraph_1();
                }
                if (output.nirPlotData != null)
                {
                    this.nirPlotData = output.nirPlotData;

                    List<string> sites = erroresAMejorar.AsEnumerable().Select(col => (string)col[1]).Distinct().ToList();
                    siteListBox_2.ItemsSource = sites;
                    siteListBox_2.SelectedItem = sites[0];

                    List<string> tech = this.nirPlotData.AsEnumerable().Where(row => (string)row[1] == (string)siteListBox_2.SelectedItem).Select(col => (string)col[3]).Distinct().ToList<string>();
                    techListBox_2.ItemsSource = tech;
                    techListBox_2.SelectedItem = tech[0];

                    var cells2Plot = nirPlotData.AsEnumerable()
                          .Where(row => (string)row[1] == (string)siteListBox_2.SelectedItem && (string)row[3] == (string)techListBox_2.SelectedItem)
                          .Select(col => (string)col[2]).Distinct().Where(item => !item.Contains("="));

                    List<SectorListItem> sectors = new List<SectorListItem>();
                    foreach (var cell in cells2Plot)
                    {
                        sectors.Add(new SectorListItem() { Enabled = true, Sector = TECH_NUM.GetSectorFromLNCEL(cell) });
                    }
                    this.Sectors = sectors;

                    Binding codeBinding = new Binding("Sectors");
                    codeBinding.Source = this;
                    sectorListBox.SetBinding(ListBox.ItemsSourceProperty, codeBinding);

                    UpdateGraph_2();
                }


            }
            else
            {
                //control de errores en BackGround
                switch (e.Error.GetType().Name)
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

                foreach (DataRow dataRow in export.data.Rows)
                {
                    colindancias.CheckColin(dataRow, R31);
                }
                foreach (DataRow dataRow in R31.NotInExports())
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
                results.nirPlotData = nir.data;
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

        #region GRAPHS
        private void Node_ComBox_Changed_Graph_1(object sender, SelectionChangedEventArgs e)
        {
            List<string> Tech = this.erroresAMejorar.AsEnumerable().Where(row => (string)row[1] == (string)siteListBox_1.SelectedItem).Select(col => (string)col[2]).Distinct().ToList<string>();
            techListBox_1.ItemsSource = Tech;
            techListBox_1.SelectedItem = Tech[Tech.Count-1];

            UpdateGraph_1();
        }

        private void Tech_ComBox_Changed_Graph_1(object sender, SelectionChangedEventArgs e)
        {
            UpdateGraph_1();
        }

        private List<String> sort_fechas(List<String> ld)

        {
            List<String> fechas = new List<String>();
            char separator = '.';
            
            foreach(String fecha in ld)
            {
                String aux = "";
                String[] strings = fecha.Split(separator);
                aux = aux + strings[2] + "/" + strings[0] + "/" + strings[1];
                fechas.Add(aux);
            }

            fechas.Sort();
            List<String> fechas2 = new List<String>();
            separator = '/';
            foreach (String fecha in fechas)
            {
                String aux = "";
                String[] strings = fecha.Split(separator);
                aux = aux + strings[2] + "/" + strings[1] + "/" + strings[0];
                fechas2.Add(aux);
            }
            

            return fechas2;
    
        }

        private void UpdateGraph_1()
        {
            int maxBarAxis = erroresAMejorar.AsEnumerable()
                                .Where(row => (string)row[1] == (string)siteListBox_1.SelectedItem && (string)row[2] == (string)techListBox_1.SelectedItem)
                                .Select(col => (int)col[3] + (int)col[8]).Max<int>();
            List<string> xAxis = erroresAMejorar.AsEnumerable()
                                .Where(row => (string)row[1] == (string)siteListBox_1.SelectedItem && (string)row[2] == (string)techListBox_1.SelectedItem)
                                .Select(col => (string)col[0]).ToList<string>();
            List<string> xAxis2 = sort_fechas(xAxis);

            SeriesCollection data = new SeriesCollection
            {
                    new StackedColumnSeries
                    {
                        Values = new ChartValues<int>(erroresAMejorar.AsEnumerable()
                                .Where(row => (string)row[1] == (string)siteListBox_1.SelectedItem && (string)row[2] == (string)techListBox_1.SelectedItem)
                                .Select(col => (int)col[3]).ToList<int>()),
                        //StackMode = StackMode.Values,
                        DataLabels = true,
                        Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#78B4CC")),
                        ScalesYAt = 0,
                        Title ="Intentos HO Inter",

                    },
                    new StackedColumnSeries
                    {
                        Values = new ChartValues<int>(erroresAMejorar.AsEnumerable()
                                .Where(row => (string)row[1] == (string)siteListBox_1.SelectedItem && (string)row[2] == (string)techListBox_1.SelectedItem)
                                .Select(col => (int)col[8]).ToList<int>()),
                        StackMode = StackMode.Values,
                        DataLabels = true,
                        Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF897D")),
                        ScalesYAt = 0,
                        Title ="Intentos HO Intra"
                    },
                    new LineSeries
                    {
                        Values = new ChartValues<double>(erroresAMejorar.AsEnumerable()
                                .Where(row => (string)row[1] == (string)siteListBox_1.SelectedItem && (string)row[2] == (string)techListBox_1.SelectedItem)
                                .Select(col => (double)col[5]).ToList<double>()),
                        ScalesYAt = 1,
                        Fill = Brushes.Transparent,
                        Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom("#CC5A6F")),
                        StrokeThickness = 3,
                        Title =" % Exitos HO Inter",
                        Opacity = 0


                    },
                    new LineSeries
                    {
                        Values = new ChartValues<double>(erroresAMejorar.AsEnumerable()
                                .Where(row => (string)row[1] == (string)siteListBox_1.SelectedItem && (string)row[2] == (string)techListBox_1.SelectedItem)
                                .Select(col => (double)col[10]).ToList<double>()),
                        Fill = Brushes.Transparent,
                        Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom("#407F99")),
                        ScalesYAt = 1,
                        StrokeThickness = 3,
                        Title ="% Exitos HO Intra"



                    },
                    /* new StackedColumnSeries
                    {
                        Values = new ChartValues<int>(erroresAMejorar.AsEnumerable()
                                .Where(row => (string)row[1] == (string)siteListBox_1.SelectedItem && (string)row[2] == (string)techListBox_1.SelectedItem)
                                .Select(col => (int)col[13]).ToList<int>()),
                        //StackMode = StackMode.Values,
                        DataLabels = true,
                        Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#7058CC")),
                        ScalesYAt = 0,
                        Title ="Intentos HO Inter + Intra"
                    },
                    */

                      new LineSeries
                    {
                        Values = new ChartValues<double>(erroresAMejorar.AsEnumerable()
                                .Where(row => (string)row[1] == (string)siteListBox_1.SelectedItem && (string)row[2] == (string)techListBox_1.SelectedItem)
                                .Select(col => (double)col[14]).ToList<double>()),
                        Fill = Brushes.Transparent,
                        Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom("#941C99")),
                        ScalesYAt = 1,
                        StrokeThickness = 3,
                        Title ="% Exitos HO Inter + Intra",
                        Opacity = 0


                    }

                };


            graphObject_1.DrawGraph(data, maxBarAxis, xAxis2);
        }



        private void Node_ComBox_Changed_Graph_2(object sender, SelectionChangedEventArgs e)
        {
            List<string> Tech = this.erroresAMejorar.AsEnumerable().Where(row => (string)row[1] == (string)siteListBox_2.SelectedItem).Select(col => (string)col[2]).Distinct().ToList<string>();
            techListBox_2.ItemsSource = Tech;
            techListBox_2.SelectedItem = Tech[0];

            var cells2Plot = nirPlotData.AsEnumerable()
                              .Where(row => (string)row[1] == (string)siteListBox_2.SelectedItem && (string)row[3] == (string)techListBox_2.SelectedItem)
                             .Select(col => (string)col[2]).Distinct();

            List<SectorListItem> sectors = new List<SectorListItem>();
            foreach (var cell in cells2Plot)
            {
                sectors.Add(new SectorListItem(){ Enabled = true, Sector = TECH_NUM.GetSectorFromLNCEL(cell)});
            }
            this.Sectors = sectors;

            Binding codeBinding = new Binding("Sectors");
            codeBinding.Source = this;
            sectorListBox.SetBinding(ListBox.ItemsSourceProperty, codeBinding);
            UpdateGraph_2();
        }

        private void Tech_ComBox_Changed_Graph_2(object sender, SelectionChangedEventArgs e)
        {
            var cells2Plot = nirPlotData.AsEnumerable()
                  .Where(row => (string)row[1] == (string)siteListBox_2.SelectedItem && (string)row[3] == (string)techListBox_2.SelectedItem)
                 .Select(col => (string)col[2]).Distinct();

            List<SectorListItem> sectors = new List<SectorListItem>();
            foreach (var cell in cells2Plot)
            {
                sectors.Add(new SectorListItem() { Enabled = true, Sector = TECH_NUM.GetSectorFromLNCEL(cell) });
            }
            this.Sectors = sectors;

            Binding codeBinding = new Binding("Sectors");
            codeBinding.Source = this;
            sectorListBox.SetBinding(ListBox.ItemsSourceProperty, codeBinding);
            UpdateGraph_2();
        }

        private void Toggle_Inter(object sender, RoutedEventArgs e)
        {
            UpdateGraph_2();
        }

        private void Toggle_Intra(object sender, RoutedEventArgs e)
        {
            UpdateGraph_2();
        }

        private void Sectors_Changed(object sender, RoutedEventArgs e)
        {
            UpdateGraph_2();
        }

        private void UpdateGraph_2()
        {
            var plotSet = nirPlotData.AsEnumerable().Where(row => (string)row[1] == (string)siteListBox_2.SelectedItem && (string)row[3] == (string)techListBox_2.SelectedItem
               && !((string)row[2]).Contains("=") && Sectors.Where(sector => sector.Enabled).Any(condition => condition.Sector == TECH_NUM.GetSectorFromLNCEL((string)row[2])));

            List<string> xAxis = nirPlotData.AsEnumerable()
                                .Where(row => (string)row[1] == (string)siteListBox_2.SelectedItem && (string)row[2] == (string)techListBox_2.SelectedItem)
                                .Select(col => (string)col[0]).ToList<string>();

            List<String> xAxis2 = sort_fechas(xAxis);
           
            
            var cells2Plot = nirPlotData.AsEnumerable()
                              .Where(row => (string)row[1] == (string)siteListBox_2.SelectedItem && (string)row[3] == (string)techListBox_2.SelectedItem)
                             .Select(col => (string)col[2]);


            SeriesCollection data = new SeriesCollection();

            var plotSectors = Sectors.Where(item => item.Enabled).ToList();

            if ((bool)checkInter.IsChecked)
            {
                for(int i = 0; i< plotSectors.Count;i++) // Intentos
                {
                    data.Add(new StackedColumnSeries
                    {
                        Values = new ChartValues<int>(plotSet.Where(row => TECH_NUM.GetSectorFromLNCEL((string)row["LNCEL name"]) == (byte)plotSectors[i].Sector).Select(col => (int)col["Intentos Inter"])),
                        StackMode = StackMode.Values,
                        DataLabels = true,
                        Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#AA"+interColorsLines[i])),
                        ScalesYAt = 0,
                        Title = "Intentos HO Inter s" + plotSectors[i].Sector
                    });
                }

                
            }
            if ((bool)checkIntra.IsChecked)
            {
                for (int i = 0; i < plotSectors.Count; i++) // Intentos
                {
                    data.Add(new StackedColumnSeries
                    {
                        Values = new ChartValues<int>(plotSet.Where(row => TECH_NUM.GetSectorFromLNCEL((string)row["LNCEL name"]) == (byte)plotSectors[i].Sector).Select(col => (int)col["Intentos Intra"])),
                        StackMode = StackMode.Values,
                        DataLabels = true,
                        Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#AA" + intraColorsLines[i])),
                        ScalesYAt = 0,
                        Title = "Intentos HO Intra s" + plotSectors[i].Sector
                    });
                }
            }
            if ((bool)checkInter.IsChecked)
            { 
                for (int i = 0; i < plotSectors.Count; i++) //Tasa de Exito
                {
                    data.Add(new LineSeries
                    {
                        Values = new ChartValues<double>(plotSet.Where(row => TECH_NUM.GetSectorFromLNCEL((string)row["LNCEL name"]) == (byte)plotSectors[i].Sector).Select(col => (double)col["% Exitos Inter"])),
                        ScalesYAt = 1,
                        StrokeThickness = 2.5,
                        Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom("#"+interColorsLines[i])),
                        Fill = Brushes.Transparent,
                        Title = " % Exitos HO Inter s" + plotSectors[i].Sector

                    });
                }

            }
            if ((bool)checkIntra.IsChecked)
            {
                for (int i = 0; i < plotSectors.Count; i++) //Tasa de Exito
                {
                    data.Add(new LineSeries
                    {
                        Values = new ChartValues<double>(plotSet.Where(row => TECH_NUM.GetSectorFromLNCEL((string)row["LNCEL name"]) == (byte)plotSectors[i].Sector).Select(col => (double)col["% Exitos Intra"])),
                        ScalesYAt = 1,
                        StrokeThickness = 2.5,
                        Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom("#"+intraColorsLines[i])),
                        Fill = Brushes.Transparent,
                        Title = " % Exitos HO Intra s" + plotSectors[i].Sector,
                    });
                }
            }
           
            //arreglar lo de los ejes, te puedes inspirar en UpdateGraph_1(), y la ordenacion de las fechas, buena suerte, y pregunta cosas a la gente 
            graphObject_2.DrawGraph(data, 50000, xAxis);
        }


        #endregion
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

    /// <summary>
    /// Checkbox y texto para habilitar o desabilitar sectores en la grafica
    /// </summary>
    public class SectorListItem : INotifyPropertyChanged
    {
        private bool enabled = true;
        private int sector;
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; OnPropertyChanged("Enabled"); }
        }

        public int Sector
        {
            get { return sector; }
            set { sector = value; OnPropertyChanged("Sector"); }
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