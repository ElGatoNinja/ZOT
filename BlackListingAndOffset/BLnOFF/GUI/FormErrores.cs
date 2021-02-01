using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZOT.BLnOFF.GUI
{
    public partial class FormErrores : Form
    {
        public DataTable dt;
        public DataView dv;

        public FormErrores()
        {

            dt = new DataTable();
            InitializeComponent();
        }


        public FormErrores(DataTable errores)
        {

            dt = errores;
            InitializeComponent();


            this.inyectarTabla();
        }




        private void formColindancias_Load(object sender, EventArgs e)
        {

        }

        private void dataGridViewColindancias_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }


        public void inyectarTabla()
        {
            this.dv = new DataView(dt);


            this.advancedDataGridViewCol.AutoGenerateColumns = true;
            this.Controls.Add(this.advancedDataGridViewCol);

            BindingSource source = new BindingSource();
            source.DataSource = dv;

            this.advancedDataGridViewCol.DataSource = source;

            //this.advancedDataGridViewCol.DataSource = dt;
            advancedDataGridViewSearchToolBar1.SetColumns(advancedDataGridViewCol.Columns);




        }







        private void advancedDataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }






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

            //MessageBox.Show("pulsada una tecla");

        }

        private void advancedDataGridViewCol_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Control && e.KeyCode == Keys.F))
            {
                advancedDataGridViewCol.CleanFilterAndSort();
            }


        }

        private void advancedDataGridViewSearchToolBar1_ItemClicked_1(object sender, ToolStripItemClickedEventArgs e)
        {

            if (e.ClickedItem.AccessibleName == "BorrarfiltrosAc")
            {

                advancedDataGridViewCol.CleanFilterAndSort();
            }

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {

            ayudaNormal ayuda = new ayudaNormal();
            ayuda.Show();


        }
    }
}

