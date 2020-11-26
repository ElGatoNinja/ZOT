using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ExcelDataReader;
using System.Runtime.InteropServices;
using ZOT.resources;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Threading;
using Excel = Microsoft.Office.Interop.Excel;
using System.Globalization;
using ZOT.HORAS48.Code;
using System.Diagnostics;

namespace ZOT.HORAS48.GUI
{
    public partial class Formulario48 : Form
    {

        String fileNorte = "";
        String fileSur = "";
        String fileCyL = "";



        List<Fila> tratados = null;
        DataTable tablafinal = null;


        public Formulario48()
        {
            InitializeComponent();
            
        }


        private void Formulario48_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        OpenFileDialog ofd = new OpenFileDialog();

        private void button1_Click(object sender, EventArgs e)
        {

            ofd.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
            ofd.Title = "Selecciona el Histórico Norte ";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                textBoxNorte.Text = ofd.FileName;
                fileNorte = ofd.FileName;
            }

            //Con esto saco la ruta hasta el file--> Console.WriteLine(Path.GetDirectoryName(fileNorte));

        }

        private void buttonSur_Click(object sender, EventArgs e)
        {

            ofd.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
            ofd.Title = "Selecciona el Histórico Sur ";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                textBoxSur.Text = ofd.FileName;
                fileSur = ofd.FileName;
            }

        }

        private void buttonCyL_Click(object sender, EventArgs e)
        {

            ofd.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
            ofd.Title = "Selecciona el Histórico CyL ";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                textBoxCyL.Text = ofd.FileName;
                fileCyL = ofd.FileName;
            }

        }


        private static string GetTableName(string connectionString, int row)
        {
            OleDbConnection conn = new OleDbConnection(connectionString);
            try
            {
                conn.Open();
                return conn.GetSchema("Tables").Rows[row]["TABLE_NAME"] + "";
            }
            catch { }
            finally { conn.Close(); }
            return "sheet1";
        }



        private void buttonAbrirNorte_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Iniciando 48Horas");

            buttonExportar.Visible = false;
            labelNombreFichero.Visible = false;
            textBoxnombrefichero.Visible = false;



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
                result = MessageBox.Show(message, caption, buttons);
            }

        }



        public void programa(String rutaNorte, String rutaSur, String rutaCyL)
        {
            //Para mostrar el cursos girando
            Cursor.Current = Cursors.WaitCursor;


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


            
            



            
            if(tratadosNorte != null)
            {
                tratados = tratadosNorte;
                if(tratadosSur != null)
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
                    if(tratadosCyL != null)
                    {
                        List<Fila> aux = tratados.Concat<Fila>(tratadosCyL).ToList();
                        tratados = aux;
                    }
                }
            }
            else
            {
                if(tratadosSur != null)
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
                    if(tratadosCyL != null){
                        tratados = tratadosCyL;
                    }
                }
            }



            Dictionary<string, string> borradasProque = null;
            borradasProque = main.BorradosPorque;

            if (borradasProque != null)
            {
                //borrradosporque borrados = new borrradosporque(borradasProque);
                //borrados.Show();
            }




            tablafinal = ConvertListToDataTable(tratados);

                DataView view = new DataView(tablafinal);
                dataGridFinal.DataSource = view;
                buttonExportar.Visible = true;
                labelNombreFichero.Visible = true;
                textBoxnombrefichero.Visible = true;


            // Parar el reloj para mostrar el tiempo de ejecuccion (antes del excel porque es un tiempo insignificativo el de crear el excel)
            stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
                tmpejecucion.Text = elapsedTime.ToString();
                Console.WriteLine(elapsedTime.ToString());
                Cursor.Current = Cursors.Default;


                    


        }






        public void OpenFile()
        {
            Excel2 excel = new Excel2(@"E:\Trabajo\IS2\IS2 copia\IS2 copia\48H\test.xlsx", 1);

            MessageBox.Show(excel.ReadCell(0, 0));

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

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void grid_items_norte2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            //Ejecutandose formejecutandose = new Ejecutandose();
           // formejecutandose.Ejecutandose_Load();
            //formejecutandose.Show();
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

        private void textBoxNorte_TextChanged(object sender, EventArgs e)
        {

        }

        private void buttonExportar_Click(object sender, EventArgs e)
        {
            string nombrefichero = textBoxnombrefichero.Text;
            var isValid = !string.IsNullOrEmpty(nombrefichero) &&
              nombrefichero.IndexOfAny(Path.GetInvalidFileNameChars()) < 0;
            var tieneEspacios = nombrefichero.IndexOf(' ') >= 0;
            if (!isValid || tieneEspacios)
            {
                MessageBox.Show("El nombre del ficherono puede tener espacios en blanco, ni contener caracteres especiales");
            }
            else {
                
                //Se guarda el excel con los datos
                Auxiliar48 main2 = new Auxiliar48();
                main2.exportarExcel(tablafinal, nombrefichero);
                
            }
        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void button1_Click_2(object sender, EventArgs e)
        {

        }

        private void button1_Click_3(object sender, EventArgs e)
        {
            Ayuda_48H ayuda = new Ayuda_48H();
            ayuda.Show();
        }

        private void textBoxCyL_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBoxSur_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }


    }



}
