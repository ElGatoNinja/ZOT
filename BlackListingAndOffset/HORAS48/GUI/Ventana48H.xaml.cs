using System;
using System.Collections.Generic;
using System.Data;
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
using ZOT.HORAS48.Code;
using System.Diagnostics;
using System.Windows.Forms;
using ZOT.resources;
using System.IO;
using System.Threading;
using Application = System.Windows.Forms.Application;
using System.Windows.Threading;

namespace ZOT.HORAS48.GUI
{
    /// <summary>
    /// Lógica de interacción para Ventana48H.xaml
    /// </summary>
    public partial class Ventana48H : Window
    {


        private delegate void EmptyDelegate();


        String fileNorte = "";
        String fileSur = "";
        String fileCyL = "";



        List<Fila> tratados = null;
        DataTable tablafinal = null;


        public Ventana48H()
        {

            InitializeComponent();

            buttonExportar.Visibility = Visibility.Hidden;
            labelNombreFichero.Visibility = Visibility.Hidden;
            textBoxnombrefichero.Visibility = Visibility.Hidden;
        }





        OpenFileDialog ofd = new OpenFileDialog();

        private void buttonNorte_Click(object sender, RoutedEventArgs e)
        {
            ofd.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
            ofd.Title = "Selecciona el Histórico Norte ";

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBoxNorte.Text = ofd.FileName;
                fileNorte = ofd.FileName;
            }

            Console.WriteLine(fileNorte);

            //Con esto saco la ruta hasta el file--> Console.WriteLine(Path.GetDirectoryName(fileNorte));

        }

        private void buttonSur_Click(object sender, RoutedEventArgs e)
        {
            ofd.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
            ofd.Title = "Selecciona el Histórico Sur ";

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBoxSur.Text = ofd.FileName;
                fileSur = ofd.FileName;
            }

            Console.WriteLine(fileSur);
        }

        private void buttonCyL_Click(object sender, RoutedEventArgs e)
        {

            ofd.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
            ofd.Title = "Selecciona el Histórico CyL ";

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBoxCyL.Text = ofd.FileName;
                fileCyL = ofd.FileName;
            }

            Console.WriteLine(fileCyL);
        }


        List<Fila> filterModeLisst = new List<Fila>();
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            filterModeLisst.Clear();

            if (textBoxBuscarCOC.Text.Equals(""))
            {
                filterModeLisst.AddRange(tratados);
            }
            else
            {
                foreach (Fila f in tratados)
                {

                    if (f.CObra.Contains(textBoxBuscarCOC.Text))
                    {
                        filterModeLisst.Add(f);
                    }
                }
            }

            dataGridFinal.ItemsSource = filterModeLisst.ToList();
        }




        private void buttonEjecutar_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Iniciando 48Horas");

            buttonExportar.Visibility = Visibility.Hidden;
            labelNombreFichero.Visibility = Visibility.Hidden;
            textBoxnombrefichero.Visibility = Visibility.Hidden;



            /*
             * Lectura de las rutas a los ficheros
             */
            String patch_norte = textBoxNorte.Text;
            Console.WriteLine(" RUTA NORTE: " + patch_norte);

            String patch_sur = textBoxSur.Text;
            Console.WriteLine(" RUTA SUR: " + patch_sur);


            String patch_CyL = textBoxCyL.Text;
            Console.WriteLine(" RUTA CyL: " + patch_CyL);


            if (patch_norte != "" || patch_sur != "" || patch_CyL != "")
            {
                programa(patch_norte, patch_sur, patch_CyL);
            }
            else
            {
                string message = "Debes elegir la ruta para el fichero";
                string caption = "Ruta vacia";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result;

                // Displays the MessageBox.
                result = System.Windows.Forms.MessageBox.Show(message, caption, buttons);
            }

        }




        public void programa(String rutaNorte, String rutaSur, String rutaCyL)
        {
            //Para mostrar el cursos girando
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;


            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            List<Fila> tratadosNorte = null;
            List<Fila> tratadosSur = null;
            List<Fila> tratadosCyL = null;

            Main main = new Main();
            //Primero se llama al metodo que carga el vNpo
            main.sacarvNpo();

            

            if (rutaNorte != "")
            {

                Console.WriteLine("+++++++++++------------++++++++++++++ TRATANDO NORTE +++++++++++------------++++++++++++++");

                

                tratadosNorte = main.sacarLista(rutaNorte, "norte");

                


            }
            if (rutaSur != "")
            {

                Console.WriteLine("+++++++++++------------++++++++++++++ TRATANDO SUR +++++++++++------------++++++++++++++");


                tratadosSur = main.sacarLista(rutaSur, "sur");


            }
            if (rutaCyL != "")
            {

                Console.WriteLine("+++++++++++------------++++++++++++++ TRATANDO CyL +++++++++++------------++++++++++++++");



                tratadosCyL = main.sacarLista(rutaCyL, "CyL");


            }








            if (tratadosNorte != null)
            {
                tratados = tratadosNorte;
                if (tratadosSur != null)
                {
                    List<Fila> aux = tratados.Concat<Fila>(tratadosSur).ToList();
                    tratados = aux;
                    if (tratadosCyL != null)
                    {
                        List<Fila> aux2 = tratados.Concat<Fila>(tratadosCyL).ToList();
                        tratados = aux2;
                    }
                }
                else
                {
                    if (tratadosCyL != null)
                    {
                        List<Fila> aux = tratados.Concat<Fila>(tratadosCyL).ToList();
                        tratados = aux;
                    }
                }
            }
            else
            {
                if (tratadosSur != null)
                {
                    tratados = tratadosSur;
                    if (tratadosCyL != null)
                    {
                        List<Fila> aux2 = tratados.Concat<Fila>(tratadosCyL).ToList();
                        tratados = aux2;
                    }
                }
                else
                {
                    if (tratadosCyL != null)
                    {
                        tratados = tratadosCyL;
                    }
                }
            }



            Dictionary<string, string> borradasProque = null;
            borradasProque = main.BorradosPorque;

            Dictionary<string, string> sinEstadoenEjecucion = null;
            sinEstadoenEjecucion = main.SinEstadoenEjecucion;


            borrradosporque borrados = new borrradosporque(borradasProque, sinEstadoenEjecucion);
            borrados.Show();





            tablafinal = ConvertListToDataTable(tratados);
            DataView view = new DataView(tablafinal);
            dataGridFinal.ItemsSource = tratados;

            buttonExportar.Visibility = Visibility.Visible;
            labelNombreFichero.Visibility = Visibility.Visible;
            textBoxnombrefichero.Visibility = Visibility.Visible;


            // Parar el reloj para mostrar el tiempo de ejecuccion (antes del excel porque es un tiempo insignificativo el de crear el excel)
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
            tmpejecucion.Content = elapsedTime.ToString();
            Console.WriteLine(elapsedTime.ToString());
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;





        }



        public void OpenFile()
        {
            Excel2 excel = new Excel2(@"E:\Trabajo\IS2\IS2 copia\IS2 copia\48H\test.xlsx", 1);

            System.Windows.Forms.MessageBox.Show(excel.ReadCell(0, 0));

            excel.Close();
        }


        public void WriteData()
        {
            //El 1 indica el Sheet
            Excel2 excel = new Excel2(@"E:\Trabajo\IS2\IS2 copia\IS2 copia\48H\test.xlsx", 1);

            excel.WriteToCell(0, 0, "TEST 2");
            excel.SaveAs(@"E:\Trabajo\IS2\IS2 copia\IS2 copia\48H\test2.xlsx");



            excel.Close();
        }

        private static void ConvertToDateTime(string value)
        {
            DateTime convertedDate;
            try
            {
                convertedDate = Convert.ToDateTime(value);
                Console.WriteLine("'{0}' converts to {1} {2} time.",
                                  value, convertedDate,
                                  convertedDate.Kind.ToString());
            }
            catch (FormatException)
            {
                Console.WriteLine("'{0}' is not in the proper format.", value);
            }
        }


        static DataTable ConvertListToDataTable(List<Fila> list)
        {
            Console.WriteLine(list.Count.ToString());
            // New table.
            DataTable table = new DataTable();
            DataRow row;
            DataColumn column;


            //Se crean las 12 columnas
            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "zona";
            table.Columns.Add(column);

            // Create second column.
            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "provincia";
            table.Columns.Add(column);

            // Create second column.
            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "codigoEmplazamiento";
            table.Columns.Add(column);

            // Create second column.
            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "emplazamiento";
            table.Columns.Add(column);

            // Create second column.
            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "tipoObra";
            table.Columns.Add(column);

            // Create second column.
            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "tipoTrabajo";
            table.Columns.Add(column);

            // Create second column.
            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "cObra";
            table.Columns.Add(column);

            // Create second column.
            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "fechaIntegraEncendido";
            table.Columns.Add(column);

            // Create second column.
            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "fechacumpleLTE";
            table.Columns.Add(column);

            // Create second column.
            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "fechacumple3G";
            table.Columns.Add(column);


            // Create second column.
            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "fechacumple2G";
            table.Columns.Add(column);

            // Create second column.
            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "fechacumple";
            table.Columns.Add(column);


            // Add rows.
            foreach (Fila f in list)
            {
                row = table.NewRow();
                row["zona"] = f.Zona;
                row["provincia"] = f.Provincia;
                row["codigoEmplazamiento"] = f.CodigoEmplazamiento;
                row["emplazamiento"] = f.Emplazamiento;
                row["tipoObra"] = f.TipoObra;
                row["tipoTrabajo"] = f.TipoTrabajo;
                row["cObra"] = f.CObra;
                row["fechaIntegraEncendido"] = f.FechaIntegraEncendido;
                row["fechacumpleLTE"] = f.FechacumpleLTE.ToString("dd/MM/yyyy");
                row["fechacumple3G"] = f.Fechacumple3G.ToString("dd/MM/yyyy");
                row["fechacumple2G"] = f.Fechacumple2G.ToString("dd/MM/yyyy");
                row["fechacumple"] = f.Fechacumple.ToString("dd/MM/yyyy");

                table.Rows.Add(row);
            }

            return table;
        }

        private void subMenuHerramientasAbrirUbicacionExcel(object sender, RoutedEventArgs e)
        {
            Auxiliar48 aux = new Auxiliar48();
            string ruta = aux.rutaExcelTabla();


            Process.Start("explorer.exe", ruta);


        }

        private void subMenuHerramientasComprobarTipoDescripciones(object sender, RoutedEventArgs e)
        {

            ComprobarTipoObra cto = new ComprobarTipoObra();
            cto.Show();


        }

        private void menuAbrirAyuda(object sender, RoutedEventArgs e)
        {

            Ayuda_48H ayuda = new Ayuda_48H();
            ayuda.Show();


        }

        private void buttonExportar_Click(object sender, RoutedEventArgs e)
        {
            
            
            string nombrefichero = textBoxnombrefichero.Text;
            var isValid = !string.IsNullOrEmpty(nombrefichero) &&
              nombrefichero.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) < 0;
            var tieneEspacios = nombrefichero.IndexOf(' ') >= 0;
            if (!isValid || tieneEspacios)
            {
                System.Windows.Forms.MessageBox.Show("El nombre del ficherono puede tener espacios en blanco, ni contener caracteres especiales");
            }
            else
            {
                //Para mostrar el cursos girando
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
                //Se guarda el excel con los datos
                Auxiliar48 aux = new Auxiliar48();
                aux.exportarExcel(tablafinal, nombrefichero);

                //Cursor por defecto
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;

            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        public static void DoEvents()
        {
            Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Background,
                    new EmptyDelegate(delegate { }));
        }
    }
}
