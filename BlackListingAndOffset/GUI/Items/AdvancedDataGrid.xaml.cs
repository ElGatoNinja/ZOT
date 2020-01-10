using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZOT.resources;
using System.ComponentModel;

namespace ZOT.GUI.Items
{
    /// <summary>
    /// Lógica de interacción para AdvancedDataGrid.xaml
    /// </summary>
    public partial class AdvancedDataGrid : DataGrid
    {
        DataTable originalData = null;
        List<FilterListItem>[] filterLists;
        ObservableCollection<FilterListItem> auxFilterLists;
        int lastFilteredColumn;
        public AdvancedDataGridFlags Flags { get; }
        private DataGridCell selectedCell;

        public AdvancedDataGrid()
        {
            this.DataContext = this;
            Flags = new AdvancedDataGridFlags();
            InitializeComponent();
        }

        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        #region COLUMN FILTER EVENTS
        private void Data_Loaded(object sender, EventArgs e)
        {
            if (originalData == null && dataGrid.ItemsSource != null)
            {
                originalData = ((DataView)(dataGrid.ItemsSource)).Table;
                filterLists = new List<FilterListItem>[originalData.Columns.Count];
            }
        }
        
        
        private void On_Filter_Open(object sender, RoutedEventArgs e)
        {
            var colHeader = (DataGridColumnHeader)((ToggleButton)sender).TemplatedParent;
            var col = colHeader.Column;
            lastFilteredColumn = col.DisplayIndex;

            //la primera vez que se abre la interfaz del filtro se buscan los posibles valores a filtrar
            if (filterLists[lastFilteredColumn] == null)
            {
                List<object> columnValues = new List<object>();
                //valores unicos de toda la columna
                columnValues = originalData.AsEnumerable().Select(x => x[lastFilteredColumn]).Distinct().ToList();

                filterLists[lastFilteredColumn] = new List<FilterListItem>();
                foreach (object item in columnValues)
                {
                    filterLists[lastFilteredColumn].Add(new FilterListItem { NotFiltered = true, Name = item.ToString() });
                }
            }
            //crea una copia que permite al usuario editar tranquilamente sin que afecte a los valores reales del filtro
            auxFilterLists = new ObservableCollection<FilterListItem>(filterLists[lastFilteredColumn].Select(item => (FilterListItem)item.Clone()));

            //si todo va bien, nunca nadie verá esto, es una chapuzilla para lograr asignar filter list a los popup
            //fui incapaz de hacerlo usando bindings en xaml
            var buttonSiblings = ((Grid)((ToggleButton)sender).Parent).Children;
            foreach (UIElement buttonSibling in buttonSiblings)
            {
                if (buttonSibling is Popup)
                {
                    var popupChilds = ((Grid)(((Popup)buttonSibling).Child)).Children;
                    foreach (UIElement popupChild in popupChilds)
                    {
                        if (popupChild is ListBox)
                        {
                            ((ListBox)popupChild).ItemsSource = auxFilterLists;
                        }
                    }
                }
            }
        }

        private void Text_Filter_Changed(object sender, EventArgs e)
        {
            auxFilterLists = new ObservableCollection<FilterListItem>(auxFilterLists.Where(item => item.Name.Contains(((TextBox)sender).Text)));
            var textBoxSiblings = ((Grid)((TextBox)sender).Parent).Children;
            foreach(UIElement item in textBoxSiblings)
            {
                if(item is ListBox)
                {
                    ((ListBox)item).ItemsSource = auxFilterLists;
                }
            }
        }
        //Aquí autentica magia, coge todas las elecciones individuales de cada filtro y las aplica a todo el dataset, permitiendo
        //hacer filtros compuestos de todo el data set
        private void Filter_Click(object sender, RoutedEventArgs e)
        {
            //en caso de filtrar hay que incorporar la informacion de el filtro auxiliar al real
            for(int i = 0;i < filterLists[lastFilteredColumn].Count;i++)
            {
                bool found = false;
                for(int j = 0;j < auxFilterLists.Count;j++)
                {
                    if(filterLists[lastFilteredColumn][i].Name == auxFilterLists[j].Name)
                    {
                        filterLists[lastFilteredColumn][i].NotFiltered = auxFilterLists[j].NotFiltered;
                        found = true;
                    }
                }
                //si no esta en aux, es porque se ha filtrado por texto y hay que quitarlo tambien
                //de esta forma el filtro real de datos no tiene que tener en cuenta el filtro de texto
                if(!found)
                {
                    filterLists[lastFilteredColumn][i].NotFiltered = false;
                }
            }
            try
            {
                DataTable newTable = (originalData.AsEnumerable()
                                .Where(row => Enumerable.Range(0, originalData.Columns.Count - 1)
                                    .Where(col => filterLists[col] != null)
                                    .All(col => filterLists[col]
                                        .Where(p => p.NotFiltered)
                                        .Any(condition => condition.Name == row[col].ToString()))))
                                .CopyToDataTable();
                this.ItemsSource = newTable.DefaultView;
            }
            catch(InvalidOperationException)
            {
                DataTable emptyTable = originalData.Clone();
                this.ItemsSource = emptyTable.DefaultView;    
            }
        }

        //Vuelve a marcar todo en la interfaz del filtro o si esta todo marcado lo quita todo para que el usuario pueda marcar solo una
        //o 2 cosas sin tener que quitar todas las casillas a mano
        private void Clean_Filter(object sender, EventArgs e)
        {
            if (auxFilterLists.All(item => item.NotFiltered))
            {
                foreach (FilterListItem item in auxFilterLists)
                {
                    item.NotFiltered = false;
                }
            }
            else
            {
                foreach (FilterListItem item in auxFilterLists)
                {
                    item.NotFiltered = true;
                }
            }

            var popupChilds = ((Grid)((Button)sender).Parent).Children;
            foreach (UIElement popupChild in popupChilds)
            {
                if(popupChild is ListBox)
                {
                    ((ListBox)popupChild).ItemsSource = new ObservableCollection<FilterListItem>();
                    ((ListBox)popupChild).ItemsSource = auxFilterLists;
                }
            }

        }
        //Deshace todos los cambios temporales que le usuario haya hecho sobre el a interfaz del filtro y lo cierra
        private void Cancel_Filter(object sender, EventArgs e)
        {
            auxFilterLists = null;
            if(sender is Button)
            {
                ((Popup)((Grid)((Button)sender).Parent).Parent).IsOpen = false;
            }
        }
        #endregion

        #region EDITION EVENTS
        private void Selected_Cells_Changed(object sender, SelectedCellsChangedEventArgs e)
        {
            if (Flags.MultiEdit) //cambiar el valor de toda la seleccion de una columna de golpe
            {
                foreach(DataGridCellInfo cellInfo in e.AddedCells)
                {
                    try
                    {
                        DataGridCell cell = (DataGridCell)cellInfo.Column.GetCellContent(cellInfo.Item).Parent;
                        if (cell.Column.DisplayIndex == selectedCell.Column.DisplayIndex)
                        {
                            DataGridRow row = DataGridRow.GetRowContainingElement(cell);
                            if (selectedCell.Content is TextBlock)//si la celda tiene texto
                            {
                                originalData.Rows[row.GetIndex()][cell.Column.DisplayIndex] = ((TextBlock)selectedCell.Content).Text;
                            }
                            else if(selectedCell.Content is CheckBox) //si la celda tiene un checkbox
                            {
                                originalData.Rows[row.GetIndex()][cell.Column.DisplayIndex] = ((CheckBox)selectedCell.Content).IsChecked;
                            }
                        }
                    }
                    catch (Exception) { }
                }
            }
        }

        #endregion
        private void KeyBoard_Commands(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                if (e.Key == Key.D) //fijar seleccion para editar varios valores de golpe
                {
                    if (dataGrid.SelectedCells.Count == 1 && !Flags.MultiEdit)
                    {
                        Flags.MultiEdit = true;
                        selectedCell = (DataGridCell)dataGrid.SelectedCells[0].Column.GetCellContent(dataGrid.SelectedCells[0].Item).Parent;
                    }
                    else
                    {
                        Flags.MultiEdit = false;
                    }

                }
                else if (e.Key == Key.R)  //cambiar modo seleccion de celdas, seleccion de filas
                {
                    this.SelectionUnit = (DataGridSelectionUnit)(((int)this.SelectionUnit + 1) % 2);
                }
            }
        }
    }

    public class FilterListItem : ICloneable
    {
        public bool NotFiltered { get; set; }
        public string Name { get; set; }

        public object Clone()
        {
            return new FilterListItem { NotFiltered = this.NotFiltered, Name = this.Name };
        }
    }

    /// <summary>
    /// Clase con informacion que debe actualizarse ente UI y data source
    /// </summary>
    public class AdvancedDataGridFlags : INotifyPropertyChanged
    {
        bool multiEdit = false;
        public bool MultiEdit 
        {
            get { return multiEdit; }
            set { multiEdit = value; OnPropertyChanged("MultiEdit"); }
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
