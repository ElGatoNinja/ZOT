using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZOT.resources.ZOTlib;

namespace ZOT.BLnOFF.GUI
{
    public partial class FormcandOff : Form
    {
        public DataTable dt;
        public DataView dv;

        public FormcandOff()
        {

            dt = new DataTable();
            InitializeComponent();

        }


        public FormcandOff(DataTable colindancias)
        {

            dt = colindancias;
            InitializeComponent();


            this.inyectarTabla();
        }




        private void formColindancias_Load(object sender, EventArgs e)
        {
        }

        private void dataGridViewColindancias_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }


        /// <summary>
        /// Metodo para inyectar la tabla en el advancedDatagrid
        /// </summary>
        public void inyectarTabla()
        {
            this.dv = new DataView(dt);


            this.advancedDataGridViewCol.AutoGenerateColumns = true;
            this.Controls.Add(this.advancedDataGridViewCol);

            BindingSource source = new BindingSource();
            source.DataSource = dv;

            this.advancedDataGridViewCol.DataSource = source;


            //recorro las filas actualizar color

            actualizarColoresCeldasCheck();

            //this.advancedDataGridViewCol.DataSource = dt;
            advancedDataGridViewSearchToolBar1.SetColumns(advancedDataGridViewCol.Columns);

            typeof(Control).GetProperty("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance)
               .SetValue(advancedDataGridViewCol, true, null);


        }

        /// <summary>
        /// Funcion para resetear los filtro al pulsar ctrl+f
        /// </summary>
        public void resetearTabla()
        {
            advancedDataGridViewCol.DataSource = null;
            advancedDataGridViewCol.Refresh();
            this.dv = new DataView(dt);


            this.advancedDataGridViewCol.AutoGenerateColumns = true;
            this.Controls.Add(this.advancedDataGridViewCol);

            BindingSource source = new BindingSource();
            source.DataSource = dv;

            this.advancedDataGridViewCol.DataSource = source;

            //this.advancedDataGridViewCol.DataSource = dt;
            advancedDataGridViewSearchToolBar1.SetColumns(advancedDataGridViewCol.Columns);



            //recorro las filas actualizar color

            actualizarColoresCeldasCheck();

        }


        //no se usa
        private void actualizarColoresCeldasCheck()
        {
            /*
            foreach (DataGridViewRow row in advancedDataGridViewCol.Rows)
            {
                var i = row.Cells[0].Value.ToString();
                if(i == "True")
                {
                    advancedDataGridViewCol.Rows[row.Index].DefaultCellStyle.BackColor = Color.Green;
                }
                var i2 = row.Cells[1].Value.ToString();
                if (i2 == "True")
                {
                    advancedDataGridViewCol.Rows[row.Index].DefaultCellStyle.BackColor = Color.Red;
                }
            }
            */
        }


        /// <summary>
        /// Metodo para pintar las celdas en funcion de las dos primeras columnas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void advancedDataGridViewCol_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in advancedDataGridViewCol.Rows)
                {
                    bool ninguno = true;
                    if(row.Cells[0].Value != null) {
                        bool i = (bool)row.Cells[0].Value;
                        if (i == true)
                        {
                            advancedDataGridViewCol.Rows[row.Index].DefaultCellStyle.BackColor = Color.FromArgb(86, 234, 42);
                            ninguno = false;
                        }
                    }
                    if (row.Cells[1].Value != null)
                    {
                        bool i2 = (bool)row.Cells[1].Value;
                        if (i2 == true)
                        {
                            advancedDataGridViewCol.Rows[row.Index].DefaultCellStyle.BackColor = Color.Red;
                            ninguno = false;
                        }
                    }

                    if (ninguno)
                    {
                        advancedDataGridViewCol.Rows[row.Index].DefaultCellStyle.BackColor = Color.White;
                    }
                }
            }
            catch { }

            /*
            Color c = e.CellStyle.BackColor;
            bool estaVerde = false;
            if (this.advancedDataGridViewCol.Columns[e.ColumnIndex].Name == "SelectedOFF" && (e.CellStyle.BackColor != Color.White || e.CellStyle.BackColor != Color.Red || e.CellStyle.BackColor != Color.Green))
            {
                if (e.Value != null)
                {
                    // Check for the string "pink" in the cell.
                    string stringValue = e.Value.ToString();
                   
                    //si es true lo pintamos todo a verde
                    if (stringValue == "True")
                    {
                        estaVerde = true;
                        //Pintamos esa celda y el resto de filas
                        e.CellStyle.BackColor = Color.Green;
                        int fila = e.RowIndex;
                        foreach (DataGridViewRow row in advancedDataGridViewCol.Rows)
                        {
                            if (row.Index == fila)
                            {
                                advancedDataGridViewCol.Rows[row.Index].DefaultCellStyle.BackColor = Color.Green;
                            }
                        }
                    }
                    //si es false lo pintamos sin color
                    else
                    {
                        estaVerde = false;
                        //Pintamos esa celda y el resto de filas
                        e.CellStyle.BackColor = Color.White;
                        int fila = e.RowIndex;
                        foreach (DataGridViewRow row in advancedDataGridViewCol.Rows)
                        {
                            if (row.Index == fila)
                            {
                                advancedDataGridViewCol.Rows[row.Index].DefaultCellStyle.BackColor = Color.White;
                            }
                        }
                    }
                }
            }

            if (this.advancedDataGridViewCol.Columns[e.ColumnIndex].Name == "CandidataBL" && (e.CellStyle.BackColor != Color.White   || e.CellStyle.BackColor != Color.Red || e.CellStyle.BackColor != Color.Green)    ) 
            {
                if (e.Value != null)
                {
                    // Check for the string "pink" in the cell.
                    string stringValue = e.Value.ToString();

                    if (stringValue == "True")
                    {
                        //Pintamos esa celda y el resto de filas
                        e.CellStyle.BackColor = Color.Red; 
                        int fila = e.RowIndex;
                        foreach (DataGridViewRow row in advancedDataGridViewCol.Rows)
                        {
                            if (row.Index == fila)
                            {
                                advancedDataGridViewCol.Rows[row.Index].DefaultCellStyle.BackColor = Color.Red;
                            }
                        }
                    }

                    else
                    {
                        if (estaVerde)
                        {
                            e.CellStyle.BackColor = Color.Green; 
                            int fila = e.RowIndex;
                            foreach (DataGridViewRow row in advancedDataGridViewCol.Rows)
                            {
                                if (row.Index == fila)
                                {
                                    advancedDataGridViewCol.Rows[row.Index].DefaultCellStyle.BackColor = Color.Green;
                                }
                            }

                        }
                        else
                        {
                            e.CellStyle.BackColor = Color.White; 
                            int fila = e.RowIndex;
                            foreach (DataGridViewRow row in advancedDataGridViewCol.Rows)
                            {
                                if (row.Index == fila)
                                {
                                    advancedDataGridViewCol.Rows[row.Index].DefaultCellStyle.BackColor = Color.White;
                                }
                            }

                        }
                    }

                }
            }
            */

        }




        private void advancedDataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }





        /// <summary>
        /// Metodo para buscar con la barra de busqueda.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void advancedDataGridViewSearchToolBar1_Search(object sender, Zuby.ADGV.AdvancedDataGridViewSearchToolBarSearchEventArgs e)
        {
            bool restartsearch = true;
            int startColumn = 0;
            int startRow = 0;
            if (!e.FromBegin)
            {


                bool endcol = advancedDataGridViewCol.CurrentCell.ColumnIndex + 1 >= advancedDataGridViewCol.ColumnCount;
                bool endrow = advancedDataGridViewCol.CurrentCell.RowIndex + 1 >= advancedDataGridViewCol.RowCount;

                if (endcol && endrow)
                {
                    startColumn = advancedDataGridViewCol.CurrentCell.ColumnIndex;
                    startRow = advancedDataGridViewCol.CurrentCell.RowIndex;
                }
                else
                {
                    startColumn = endcol ? 0 : advancedDataGridViewCol.CurrentCell.ColumnIndex + 1;
                    startRow = advancedDataGridViewCol.CurrentCell.RowIndex + (endcol ? 1 : 0);
                }
            }
            DataGridViewCell c = advancedDataGridViewCol.FindCell(
                e.ValueToSearch,
                e.ColumnToSearch != null ? e.ColumnToSearch.Name : null,
                startRow,
                startColumn,
                e.WholeWord,
                e.CaseSensitive);
            if (c == null && restartsearch)
                c = advancedDataGridViewCol.FindCell(
                    e.ValueToSearch,
                    e.ColumnToSearch != null ? e.ColumnToSearch.Name : null,
                    0,
                    0,
                    e.WholeWord,
                    e.CaseSensitive);
            if (c != null)
                advancedDataGridViewCol.CurrentCell = c;
        }

        private void advancedDataGridViewSearchToolBar1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {


            if (e.ClickedItem.AccessibleName == "BorrarfiltrosAc")
            {

                advancedDataGridViewCol.CleanFilterAndSort();
            }


        }



        private void formColindancias_FormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
        }

        private void formColindancias_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Borrar filtros");

        }

        private void advancedDataGridViewCol_KeyPress(object sender, KeyPressEventArgs e)
        {


        }


        //Capturamos el control F (resetear filtros) y el F5 recargar (arregala ciertas cosas)
        private void advancedDataGridViewCol_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Control && e.KeyCode == Keys.F))
            {
                advancedDataGridViewCol.CleanFilterAndSort();
            }
            if ((e.KeyCode == Keys.F5))
            {
                resetearTabla();
            }

        }

        private void advancedDataGridViewSearchToolBar1_ItemClicked_1(object sender, ToolStripItemClickedEventArgs e)
        {

            if (e.ClickedItem.AccessibleName == "BorrarfiltrosAc")
            {

                advancedDataGridViewCol.CleanFilterAndSort();
            }
             
            
            /*
             * if (e.ClickedItem.AccessibleName == "ayudabtn")
            {

                ayudaCandOff ayuda = new ayudaCandOff();
                ayuda.Show();
            }
            */
        }

        private void advancedDataGridViewCol_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            if (advancedDataGridViewCol.Columns[e.ColumnIndex].Name == "*")
            {
                bool flag = (bool)advancedDataGridViewCol.Rows[e.RowIndex].Cells[e.ColumnIndex].EditedFormattedValue;
                if (flag)
                    advancedDataGridViewCol.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Red;
                else
                    advancedDataGridViewCol.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {

            ayudaCandOff ayuda = new ayudaCandOff();
            ayuda.Show();
        }

        private void btnExportarCandOff_Click(object sender, EventArgs e)
        {

            DataTable data = ToDataTable(advancedDataGridViewCol);


            string output_path = ZOTFiles.FileSaver("Archivos CSV |*csv", "csv", "Exportando plantilla de Blacklisting");

            if (output_path != "error")
            {


                try{

                    using (StreamWriter writer = new StreamWriter(output_path))
                    {
                        writer.WriteLine("Objeto;mrbtsId;lnBtsId;lnCelId;lnRelId;cellIndOffNeigh;;");
                        foreach (DataRow row in data.Rows)
                        {
                            if (row["Column1"].ToString() == "True")
                            {
                                string toCSV = "LNREL;";
                                toCSV += (string)row["Column4"] + ";";
                                toCSV += (string)row["Column4"] + ";";
                                toCSV += row["Column5"].ToString() + ";";

                                if ((string)row["Column3"] != "")
                                    toCSV += ((string)row["Column3"]).Split('-')[1] + ";";
                                else
                                    toCSV += ";";

                                if (int.Parse(row["Column12"].ToString()) > 9)
                                    toCSV += (int.Parse(row["Column12"].ToString()) - 3).ToString() + ";";
                                else
                                    toCSV += (int.Parse(row["Column12"].ToString()) - 1).ToString() + ";";

                                toCSV += (string)row["Column6"] + ";";
                                toCSV += (string)row["Column9"];

                                writer.WriteLine(toCSV);
                            }
                        }




                    }

                    MessageBox.Show("CSV generado correctamente", "Exportado", MessageBoxButtons.OK, MessageBoxIcon.Information);


                
                }
                
                catch (IOException)
                {
                    MessageBox.Show("El CSV debe cerrarse para poder exportarlo", "ERRoR", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
                catch (Exception)
                {
                    MessageBox.Show("Se produjo en error en la generación del csv", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
                






            }
            else
            {
                MessageBox.Show("La ruta no es valida o se canceló la generación del csv.", "Cancelado", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

        }


            private DataTable ToDataTable(DataGridView dataGridView)
            {
                var dt = new DataTable();
                foreach (DataGridViewColumn dataGridViewColumn in dataGridView.Columns)
                {
                    if (dataGridViewColumn.Visible)
                    {
                        dt.Columns.Add();
                    }
                }
                var cell = new object[dataGridView.Columns.Count];
                foreach (DataGridViewRow dataGridViewRow in dataGridView.Rows)
                {
                    for (int i = 0; i < dataGridViewRow.Cells.Count; i++)
                    {
                        cell[i] = dataGridViewRow.Cells[i].Value;
                    }
                    dt.Rows.Add(cell);
                }
                return dt;
            }



        }


    }

