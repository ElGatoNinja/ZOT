using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.ComponentModel;
using ZOT.resources.ZOTlib;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace ZOT.GUI.Items
{
    /// <summary>
    /// Lógica de interacción para AdvancedDataGrid.xaml
    /// </summary>
    public partial class AdvancedDataGrid : DataGrid
    {
        private DataTable _workingData;
        public List<FilterHierarchyC> FilterHierarchy { get; set; }
        private int activeFilters = 0;

        private ObservableCollection<FilterListItem> backUpFilterList = null;
        private DataGridSortOrder LastSortOrder;
        public AdvancedDataGridFlags Flags { get; }
        private DataGridCell _selectedCell;
        private List<ListBox> temporalListBoxContainer = new List<ListBox>();

        #region INITIALIZATION
        public AdvancedDataGrid()
        {
            this.DataContext = this;
            Flags = new AdvancedDataGridFlags();

            InitializeComponent();
        }

        /// <summary>
        /// Establece el origen de datos con el que la tabla va a trabajar 
        /// </summary>
        public DataTable WorkingData
        {
            get
            {
                return _workingData;
            }

            set
            {
                ItemsSource = value.DefaultView;
                _workingData = value;
                FilterHierarchy = null;
                LastSortOrder = new DataGridSortOrder();
                backUpFilterList = null;
                Flags.ResetState();
                _selectedCell = null;
                temporalListBoxContainer = new List<ListBox>();

                FilterHierarchy = new List<FilterHierarchyC>() ;
                try
                {
                    for (int i = 0; i < value.Columns.Count; i++)
                    {
                        List<object> columnValues = new List<object>();
                        //valores unicos de toda la columna
                        columnValues = ((DataView)(this.ItemsSource)).ToTable().AsEnumerable().Select(x => x[i]).Distinct().ToList();

                        FilterHierarchy.Add(new FilterHierarchyC());
                        FilterHierarchy[i].Filter = new ObservableCollection<FilterListItem>();
                        foreach (object item in columnValues)
                        {
                            FilterHierarchy[i].Filter.Add(new FilterListItem { NotFiltered = true, Name = item.ToString(), IsTextFiltered = false, IsFilteredInOtherFilter = false });
                            FilterHierarchy[i].priority = 0;
                            FilterHierarchy[i].column = i;
                        }

                    }
                }
                catch (NullReferenceException)  //causado por inicializacion sin ItemSource, es decir la mayoria de las veces
                {
                    temporalListBoxContainer = new List<ListBox>();
                }
            }
        }

        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString(); //numera las lineas
        }

        //crea bindings en runtime, esto es necesario porque los filtros se crean dinamicamente tras vincular el dataset y saber cuantas columnas tiene
        private void FilterList_Initialized_From_Template(object sender,EventArgs e)
        {
            if (((ListBox)sender).DataContext.GetType() == typeof(string))
            {
                temporalListBoxContainer.Add((ListBox)sender);
            }
        }

        //cada vez que se filtra, se reinician las columnas y por tanto los filtros, hay que comprobar si esta columna esta filtrada
        //Y cambiar el simbolo del filtro para que el usuario lo sepa
        private void FilterBtn_Initialized_From_Template(object sender, EventArgs e)
        {
            //inicializacion vacía
            if (_workingData == null) return;
            if (((DataGridColumnHeader)((ToggleButton)sender).TemplatedParent).Column == null) return;

            int column = ((DataGridColumnHeader)((ToggleButton)sender).TemplatedParent).Column.DisplayIndex;
           
            if (FilterHierarchy[column].Filter.All(item => item.NotFiltered))
            {
                ((MahApps.Metro.IconPacks.PackIconMaterial)((ToggleButton)sender).Content).Kind = MahApps.Metro.IconPacks.PackIconMaterialKind.Filter;
                if (FilterHierarchy[column].priority > 0)
                {
                    foreach (FilterHierarchyC fh in FilterHierarchy)
                    {
                        if (fh.priority > FilterHierarchy[column].priority)
                            fh.priority--;
                    }
                    FilterHierarchy[column].priority = 0;
                }
            }
            else
            {
                ((MahApps.Metro.IconPacks.PackIconMaterial)((ToggleButton)sender).Content).Kind = MahApps.Metro.IconPacks.PackIconMaterialKind.FilterMenu;
            }
                

        }
        #endregion

        //==============================================================================================================================
        #region COLUMN FILTER EVENTS


        private void On_Filter_Open(object sender, RoutedEventArgs e)
        {
            //crea una copia que permite al usuario editar tranquilamente permitiendo resetear los valores del filtro en caso de cancelar
            int column = ((DataGridColumnHeader)((ToggleButton)sender).TemplatedParent).Column.DisplayIndex;
            Binding codeBinding = new Binding("FilterHierarchy[" + column + "].Filter");
            codeBinding.Source = this;
            temporalListBoxContainer[column].SetBinding(ListBox.ItemsSourceProperty, codeBinding);
            backUpFilterList = new ObservableCollection<FilterListItem>(FilterHierarchy[column].Filter.Select(item => (FilterListItem)item.Clone()));
        }

        //Hace invisibles para el usuario las columnas que estén filtradas por texto
        private void Text_Filter_Changed(object sender, EventArgs e)
        {
            try
            {
                int column = ((DataGridColumnHeader)((TextBox)sender).TemplatedParent).Column.DisplayIndex;
                foreach (FilterListItem filterItem in FilterHierarchy[column].Filter)
                {
                    filterItem.IsTextFiltered = true;
                }
                foreach (FilterListItem filterItem in FilterHierarchy[column].Filter.Where(item => item.Name.Contains(((TextBox)sender).Text)))
                {
                    filterItem.IsTextFiltered = false;
                }
            }
            catch (NullReferenceException) { } //Durante la actualizacion de los datos tras el filtrado
        }


        //Aquí sucede la autentica magia, coge todas las elecciones individuales de cada filtro y las aplica a todo el dataset, permitiendo
        //hacer filtros compuestos de todo el data set
        private void Filter_Click(object sender, RoutedEventArgs e)
        {
#if DEBUG
            var timer = new Stopwatch();
            timer.Start();
#endif

            int index = ((DataGridColumnHeader)((Control)sender).TemplatedParent).Column.DisplayIndex;
            if (FilterHierarchy[index].priority == 0)
            {
                //al añadir un filtro este se añade con la minima prioridad posible y los demas filtros avanzan en la jerarquia
                for (int i = 0; i < FilterHierarchy.Count; i++)
                {
                    if (FilterHierarchy[i].priority != 0)
                        FilterHierarchy[i].priority++;
                }
                FilterHierarchy[index].priority++;
            }
            var orderedFilterHierarchy = FilterHierarchy.OrderByDescending(item => item.priority).ToList();

#if DEBUG
            timer.Stop();
            Console.WriteLine("Ordenar Filtros: " + timer.Elapsed.ToString(@"m\:ss\.fff"));
            timer.Reset();
            timer.Start();
#endif

            List<DataRow> auxData = _workingData.AsEnumerable().ToList();
            for (int i = 0; i < orderedFilterHierarchy[0].priority ; i++)
            {
                //se filtra el dataset con cada filtro que tenga prioridad > 0
                auxData = auxData.AsEnumerable().Where(row => orderedFilterHierarchy[i].Filter
                                    .Where(item =>
                                    {
                                        if (item.IsTextFiltered) //se aplica el filtro de texto sobre los filtros individuales
                                        {                       
                                            item.NotFiltered = false;
                                            item.IsTextFiltered = false; //y se quita el filtrado de texto para que la lista se vea correctamente tras la carga
                                        }                               
                                        return item.NotFiltered;
                                    })
                                    .Any(condition => condition.Name == row[orderedFilterHierarchy[i].column].ToString())).ToList();
                //se ocultan los valores que han quedado inaccesibles en el siguiente filtro
                var aux = auxData.AsEnumerable().Select(col => col[orderedFilterHierarchy[i + 1].column].ToString()).Distinct();
                foreach (FilterListItem filterItem in orderedFilterHierarchy[i + 1].Filter)
                {
                    
                    if(!aux.Contains(filterItem.Name))
                    {
                        filterItem.IsFilteredInOtherFilter = true;
                    }
                    else
                    {
                        filterItem.IsFilteredInOtherFilter = false;
                    }
                }
            }
#if DEBUG
            timer.Stop();
            Console.WriteLine("Filtros con prioridad: " + timer.Elapsed.ToString(@"m\:ss\.fff"));
            timer.Reset();
            timer.Start();
#endif

            //Se filtran las listas de los filtros vacios
            for (int i = orderedFilterHierarchy[0].priority; i < orderedFilterHierarchy.Count; i++) 
            {
                var aux = auxData.AsEnumerable().Select(col => col[orderedFilterHierarchy[i].column].ToString()).Distinct();
                foreach (FilterListItem filterItem in orderedFilterHierarchy[i].Filter)
                {
                    if (!aux.Contains(filterItem.Name))
                    {
                        filterItem.IsFilteredInOtherFilter = true;
                    }
                    else
                    {
                        filterItem.IsFilteredInOtherFilter = false;
                    }
                }
            }

#if DEBUG
            timer.Stop();
            Console.WriteLine("Quitar valores inacesibles de los otros filtros: " + timer.Elapsed.ToString(@"m\:ss\.fff"));
#endif


            //se ordena por la ultima columna ordenada
            if (LastSortOrder.order == ListSortDirection.Ascending)
                auxData = auxData.AsEnumerable().OrderBy(item => item[LastSortOrder.column]).ToList();
            else if (LastSortOrder.order == ListSortDirection.Descending)
                auxData = auxData.AsEnumerable().OrderByDescending(item => item[LastSortOrder.column]).ToList();

            ItemsSource = auxData.CopyToDataTable().AsDataView();

            backUpFilterList = null;
            temporalListBoxContainer = new List<ListBox>(); //Al reasignar ItemsSource los filtros desplegables se joden, misterios de la naturaleza
        }

        //Vuelve a marcar todo en la interfaz del filtro o si esta todo marcado lo quita todo para que el usuario pueda marcar solo una
        //o 2 cosas sin tener que quitar todas las casillas a mano
        private void Clean_Filter(object sender, EventArgs e)
        {
            int column = ((DataGridColumnHeader)((Button)sender).TemplatedParent).Column.DisplayIndex;

            if (FilterHierarchy[column].Filter.All(item => item.NotFiltered))
            {
                foreach (FilterListItem item in FilterHierarchy[column].Filter)
                {
                    item.NotFiltered = false;
                }
            }
            else
            {
                foreach (FilterListItem item in FilterHierarchy[column].Filter)
                {
                    item.NotFiltered = true;
                }
            }
        }
        //devuelve todos los filtros y el dataset a su estado inicial
        private void ResetFilters()
        {
            FilterHierarchy = new List<FilterHierarchyC>();
            ItemsSource = _workingData.AsDataView();
            for (int i = 0; i < _workingData.Columns.Count; i++)
            {
                List<object> columnValues = new List<object>();
                //valores unicos de toda la columna
                columnValues = ((DataView)(this.ItemsSource)).ToTable().AsEnumerable().Select(x => x[i]).Distinct().ToList();

                FilterHierarchy.Add(new FilterHierarchyC());
                FilterHierarchy[i].Filter = new ObservableCollection<FilterListItem>();
                foreach (object item in columnValues)
                {
                    FilterHierarchy[i].Filter.Add(new FilterListItem { NotFiltered = true, Name = item.ToString(), IsTextFiltered = false, IsFilteredInOtherFilter = false });
                    FilterHierarchy[i].priority = 0;
                    FilterHierarchy[i].column = i;
                }

            }

        }

        //Deshace todos los cambios temporales que el usuario haya hecho sobre la interfaz del filtro y lo cierra
        private void Cancel_Filter(object sender, EventArgs e)
        {
            if (backUpFilterList == null)
                return;

            int column = 0;
            if (sender is Popup)
                column = ((DataGridColumnHeader)((Popup)sender).TemplatedParent).Column.DisplayIndex;
            else if (sender is Button)
                column = ((DataGridColumnHeader)((Control)sender).TemplatedParent).Column.DisplayIndex;

            FilterHierarchy[column].Filter = new ObservableCollection<FilterListItem>(backUpFilterList.Select(item => (FilterListItem)item.Clone()));
            backUpFilterList = null;
            if(sender is Button)
            {
                WPFForms.FindParent<Popup>((Button)sender).IsOpen = false;
            }
        }

        private void Column_Sorted(object sender, DataGridSortingEventArgs e)
        {
            switch(e.Column.SortDirection)
            {
                case ListSortDirection.Ascending:
                    LastSortOrder.order = ListSortDirection.Descending;
                    break;
                case ListSortDirection.Descending:
                    LastSortOrder.order = ListSortDirection.Ascending;
                    break;
                default:
                    LastSortOrder.order = ListSortDirection.Ascending;
                    break;
            }
            LastSortOrder.column = e.Column.DisplayIndex;
        }
        #endregion

        //================================================================================================================
        #region EDITION EVENTS
        private void Selected_Cells_Changed(object sender, SelectedCellsChangedEventArgs e)
        {

            if (Flags.MultiEdit) //cambiar el valor de toda la seleccion de una columna de golpe
            {
                foreach(DataGridCellInfo cellInfo in e.AddedCells) //si se hace a mano no hace falta bucle, pero quien sabe si en un futuro..., asi que por si acaso 
                {
                    try
                    {
                        DataGridCell cell = (DataGridCell)cellInfo.Column.GetCellContent(cellInfo.Item).Parent;
                        if (cell.Column.DisplayIndex == _selectedCell.Column.DisplayIndex)
                        {
                            DataGridRow row = DataGridRow.GetRowContainingElement(cell);
                            if (_selectedCell.Content is TextBlock)//si la celda tiene texto
                            {
                                string oldValue =_workingData.Rows[_workingData.Rows.IndexOf(((DataRowView)row.Item).Row)][cell.Column.DisplayIndex].ToString();
                                _workingData.Rows[_workingData.Rows.IndexOf(((DataRowView)row.Item).Row)][cell.Column.DisplayIndex] = ((TextBlock)_selectedCell.Content).Text;

                                if (!_workingData.AsEnumerable().Any(item => item[cell.Column.DisplayIndex].ToString() == oldValue))
                                {
                                    FilterHierarchy[cell.Column.DisplayIndex].Filter.Remove(FilterHierarchy[cell.Column.DisplayIndex].Filter.Where(item => item.Name == oldValue).Single());
                                }
                                //se actualiza el valor en el origen de datos
                                
                            }
                            else if(_selectedCell.Content is CheckBox) //si la celda tiene un checkbox
                            {
                                _workingData.Rows[_workingData.Rows.IndexOf(((DataRowView)row.Item).Row)][cell.Column.DisplayIndex] = ((CheckBox)_selectedCell.Content).IsChecked;
                                //((DataView)(this.ItemsSource)).Table.Rows[row.GetIndex()][cell.Column.DisplayIndex] = ((CheckBox)selectedCell.Content).IsChecked;
                            }
                        }
                        else
                        {
                            //solo se puede seleccionar una columna para imitar el efecto de arrastre de excel 
                            cell.IsSelected = false;
                        }
                    }
                    catch (Exception) { }
                }
            }
        }

        private void Cell_Edited(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (Flags.MultiEdit) return;

            if (e.EditingElement is TextBox) //si es True/False el contenido de los filtros nunca va a cambiar
            {
                string originalValue = _workingData.Rows[_workingData.Rows.IndexOf(((DataRowView)e.Row.Item).Row)].ItemArray[e.Column.DisplayIndex].ToString();
                string currentValue = ((TextBox)e.EditingElement).Text;
                int column = e.Column.DisplayIndex;

                //si el nuevo valor no existía en el filtro se añade
                if (!FilterHierarchy[e.Column.DisplayIndex].Filter.Any(item => item.Name == currentValue))
                {
                    FilterHierarchy[e.Column.DisplayIndex].Filter.Add(new FilterListItem { IsTextFiltered = false, Name = currentValue, NotFiltered = false });
                }

                //si solo había una copia del valor original en toda la tabla, es decir, que ya no hay, se elimina del filtro
                if (_workingData.AsEnumerable().Select(item => item.ItemArray[e.Column.DisplayIndex].ToString() == originalValue).Count() > 1)
                {
                    FilterHierarchy[e.Column.DisplayIndex].Filter.Remove(FilterHierarchy[e.Column.DisplayIndex].Filter.Where(item => item.Name == originalValue).Single());
                }
            }
        }

        private void MouseDown_Cell(object sender, MouseButtonEventArgs e)
        {

            this._selectedCell = (DataGridCell)sender;

            if(((DataGridCell)sender).Content is CheckBox && !Flags.MultiEdit) //solo 1 click para cambiar las cell con checkbox de true a false
            {
                e.Handled = true;
                ((DataRowView)((DataGridCell)sender).DataContext).Row[((DataGridCell)sender).Column.DisplayIndex] = !(bool)((DataRowView)((DataGridCell)sender).DataContext).Row[((DataGridCell)sender).Column.DisplayIndex];
            }
        }

        private void MouseUp_dataGrid(object sender, MouseButtonEventArgs e)
        {
            if (Flags.MultiEdit)
            {
                Flags.MultiEdit = false;
                e.Handled = true;
            }
        }

        private void PasteToGrid()
        {
            try
            {
                string clipBoardText = Clipboard.GetText();
                DataGridRow dgrow = DataGridRow.GetRowContainingElement(_selectedCell);
                int x_index = dgrow.GetIndex();
                DataRowView row = (DataRowView)dgrow.Item;
                DataView view = row.DataView;

                string[] rows = clipBoardText.Split(new[] { "\r\n" }, StringSplitOptions.None);
                for (int i = 0; i < rows.Length; i++)
                {
                    string[] cols = rows[i].Split('\t');
                    for (int j = 0; j < cols.Length; j++)
                    {
                        //cada columna de la tabla puede emplear un tipo de datos distinto, si no coinciden no se pega
                        Type typeOfCell = _workingData.Rows[_workingData.Rows.IndexOf(view[i].Row)][_selectedCell.Column.DisplayIndex + j].GetType();
                        if (typeOfCell == typeof(string))
                            _workingData.Rows[_workingData.Rows.IndexOf(view[i].Row)][_selectedCell.Column.DisplayIndex + j] = cols[j].Trim();
                        else if (typeOfCell == typeof(double))
                        {
                            double? aux;
                            if (Conversion.ToDouble(cols[j].Trim(), out aux))
                                _workingData.Rows[_workingData.Rows.IndexOf(view[i].Row)][_selectedCell.Column.DisplayIndex + j] = aux;
                        }
                        else if (typeOfCell == typeof(int))
                        {
                            int? aux;
                            if (Conversion.ToInt(cols[j].Trim(), out aux))
                                _workingData.Rows[_workingData.Rows.IndexOf(view[i].Row)][_selectedCell.Column.DisplayIndex + j] = aux;
                        }
                    }
                }
            }
            catch(Exception)
            {
                WPFForms.ShowError("No se han podido pegar correctamente los datos", "Puede ser por un tamaño de datos superior al de esta tabla o al uso de un tipo de dato inesperado");
            }
        }

        #endregion
        private void KeyBoard_Commands(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                if (e.Key == Key.D) //fijar seleccion para editar varios valores de golpe
                {
                    e.Handled = true;
                    this.SelectionUnit = DataGridSelectionUnit.Cell;
                    this.SelectedCells.Clear();
                    Flags.MultiEdit = !Flags.MultiEdit;
                }
                else if (e.Key == Key.R)  //cambiar modo seleccion de celdas, seleccion de filas
                {
                    e.Handled = true;
                    Flags.MultiEdit = false;
                    this.SelectionUnit = (DataGridSelectionUnit)(((int)this.SelectionUnit + 1) % 2);
                }
                else if (e.Key == Key.V) //no activo aun 
                {
                    //e.Handled = true;
                    //PasteToGrid();
                }
                else if(e.Key == Key.F)
                {
                    e.Handled = true;
                    ResetFilters();
                }
                else if(e.Key == Key.H)
                {
                    e.Handled = true;
                    ClipboardCopyMode = (DataGridClipboardCopyMode)((int)ClipboardCopyMode % 2 + 1);
                }
            }
        }
    }

    //====================================================================================================================================
    #region HELPER CLASSES
    
    /// <summary>
    /// La clase que se usa dentro de la interfaz desplegable del filtro
    /// </summary>
    public class FilterListItem : ICloneable, INotifyPropertyChanged
    {
        private bool notFiltered = true;
        private bool isTextFiltered = false;
        private string name;
        private bool isFilteredInOtherFilter = false;
        public bool NotFiltered
        {
            get { return notFiltered; }
            set { notFiltered = value; OnPropertyChanged("NotFiltered"); }
        }

        public string Name
        {
            get { return name; }
            set { name = value; OnPropertyChanged("Name"); }
        }
        
        public bool IsTextFiltered
        {
            get { return isTextFiltered; }
            set { isTextFiltered = value; OnPropertyChanged("IsTextFiltered"); }
        }

        public bool IsFilteredInOtherFilter
        {
            get { return isFilteredInOtherFilter; }
            set { isFilteredInOtherFilter = value; OnPropertyChanged("IsFilteredInOtherFilter"); }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }
        public object Clone()
        {
            return new FilterListItem { NotFiltered = this.NotFiltered, Name = this.Name, IsTextFiltered = this.IsTextFiltered};
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

        /// <summary>
        /// Reinicia las flags para cargar nuevos datos
        /// </summary>
        public void ResetState()
        {
            MultiEdit = false;
        }
        public void OnPropertyChanged(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }
    }

    public class WPFBool : INotifyPropertyChanged
    {
        private bool _value = true;
        public bool Value
        {
            get { return _value; }
            set { _value = value; OnPropertyChanged("Value"); }
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
    class DataGridSortOrder
    {
        public ListSortDirection? order = null;
        public int column;
    }

    public class FilterHierarchyC
    {
        public ObservableCollection<FilterListItem> Filter { get; set; }
        public int priority;
        public int column;
    }

    #endregion
}
