using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZOT.HORAS48.Code;

namespace ZOT.HORAS48.GUI
{
    public partial class ComprobarTipoObra : Form
    {

        String file = "";


        public ComprobarTipoObra()
        {
            InitializeComponent();
        }

        private void ComprobarTipoObra_Load(object sender, EventArgs e)
        {

        }

        private void buttonAbrir_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Iniciando comprobacion de Tipos de Obra de 48Horas");

            /*
             * Lectura de las rutas a los ficheros
             */
            String patch = textBoxNorte.Text;
            Console.WriteLine(" RUTA: " + patch);

          


            if (patch != "")
            {
                programa(patch);
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


        public void programa(String ruta)
        {
            //Para mostrar el cursos girando
            Cursor.Current = Cursors.WaitCursor;


            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();


            Main main = new Main();

            if (ruta != "")
            {
                DataTable dt = main.comprobarTiposObra(ruta);
                this.dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
                int contador = 0;
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    if (row.Cells["TIPO"].Value != null){ contador++; }

                }
                if(contador != 0) { buttonAbrir.Visible = true; }
                dataGridView.DataSource = dt;
            }
            




           



        }


            OpenFileDialog ofd = new OpenFileDialog();

        private void buttonNorte_Click(object sender, EventArgs e)
        {

            ofd.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
            ofd.Title = "Selecciona el Histórico Norte ";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                textBoxNorte.Text = ofd.FileName;
                file = ofd.FileName;
            }

            //Con esto saco la ruta hasta el file--> Console.WriteLine(Path.GetDirectoryName(file));
        }

        

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void buttonGuardar_Click(object sender, EventArgs e)
        {

            var mapCambios = new Dictionary<string, string>();

            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.Cells["TIPO"].Value != null)
                {
                    
                    string tipo = row.Cells["TIPO"].Value.ToString();
                    string estado = row.Cells["ESTADO"].Value.ToString();
                    Console.WriteLine(tipo + "----" + estado);

                    if(estado == "NO" || estado == "ENCENDIDO" || estado == "INTEGRACION"){
                        mapCambios.Add(tipo, estado);
                    }
                    else { MessageBox.Show("Un tipo no tiene estado (NO, ENCENDIDO o INTEGRACION [sin tildes]"); break; }
                }
                
                
            }


            Microsoft.Office.Interop.Excel.Workbook mWorkBook;
            Microsoft.Office.Interop.Excel.Sheets mWorkSheets;
            Microsoft.Office.Interop.Excel.Worksheet mWSheet1;
            Microsoft.Office.Interop.Excel.Application oXL;

            string path = Path.Combine(Environment.CurrentDirectory, @"Data\", "Tabla.xlsx");
            oXL = new Microsoft.Office.Interop.Excel.Application();
            oXL.Visible = true;
            oXL.DisplayAlerts = false;
            mWorkBook = oXL.Workbooks.Open(path, 0, false, 5, "", "", false, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "", true, false, 0, true, false, false);
            //Get all the sheets in the workbook
            mWorkSheets = mWorkBook.Worksheets;
            //Get the allready exists sheet
            mWSheet1 = (Microsoft.Office.Interop.Excel.Worksheet)mWorkSheets.get_Item("TABLA");
            Microsoft.Office.Interop.Excel.Range range = mWSheet1.UsedRange;
            int colCount = range.Columns.Count; 
            int rowCount = range.Rows.Count; //Devuelve el indice de la ultima fila

            //Console.WriteLine("SALIDA DEL EXCEL: --> " + colCount.ToString() + " -- " + rowCount.ToString());

            int i = 1;
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.Cells["TIPO"].Value != null)
                {
                    //Añado los estad
                    oXL.Cells[rowCount + i, 1] = row.Cells["TIPO"].Value.ToString();
                    oXL.Cells[rowCount + i, 2] = row.Cells["ESTADO"].Value.ToString();
                    i++;
                }
            }

            oXL.DisplayAlerts = false;

            mWorkBook.SaveAs(path, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing,
            Type.Missing, Type.Missing);
            mWorkBook.Close();
            oXL.Quit();

            MessageBox.Show("Tabla modificada");






        }
    }
}
