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
        private DataTable _workingData;
        ObservableCollection<FilterListItem>[] filterLists = null;
        ObservableCollection<FilterListItem> backUpFilterList = null;
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
                filterLists = null;
            }
        }

        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString(); //numera las lineas
        }

        //crea bindings en runtime, esto es necesario porque los filtros se crean dinamicamente tras vincular el dataset y saber cuantas columnas tiene
        private void Filter_Initialized_From_Template(object sender,EventArgs e)
        {
            if (((ListBox)sender).DataContext.GetType() == typeof(string))
                temporalListBoxContainer.Add((ListBox)sender);
        }

        //cuando todos los datos estan cargados se meten en los filtros con el formato adecuado
        private void DataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            if (filterLists != null)
                return;

            filterLists = new ObservableCollection<FilterListItem>[this.Columns.Count];
            try
            {
                for (int i = 0; i < this.Columns.Count; i++)
                {
                    List<object> columnValues = new List<object>();
                    //valores unicos de toda la columna
                    columnValues = ((DataView)(this.ItemsSource)).ToTable().AsEnumerable().Select(x => x[i]).Distinct().ToList();

                    filterLists[i] = new ObservableCollection<FilterListItem>();
                    foreach (object item in columnValues)
                    {
                        filterLists[i].Add(new FilterListItem { NotFiltered = true, Name = item.ToString(), IsTextFiltered = false });
                    }

                }
            }
            catch (NullReferenceException)  //causado por inicializacion sin ItemSource, es decir la mayoria de las veces
            {
                temporalListBoxContainer = new List<ListBox>();
            }  

        }

        public ObservableCollection<FilterListItem>[] FilterLists
        {
            get
            {
                return filterLists;
            }
        }
        #endregion

        #region COLUMN FILTER EVENTS


        private void On_Filter_Open(object sender, RoutedEventArgs e)
        {
            //crea una copia que permite al usuario editar tranquilamente permitiendo resetear los valores del filtro en caso de cancelar
            int index = ((DataGridColumnHeader)((ToggleButton)sender).TemplatedParent).Column.DisplayIndex;
            Binding codeBinding = new Binding("FilterLists[" + index + "]");
            codeBinding.Source = this;
            temporalListBoxContainer[index].SetBinding(ListBox.ItemsSourceProperty, codeBinding);
            backUpFilterList = new ObservableCollection<FilterListItem>(filterLists[index].Select(item => (FilterListItem)item.Clone()));
        }

        //Hace invisibles para el usuario las columnas que estén filtradas por texto
        private void Text_Filter_Changed(object sender, EventArgs e)
        {
            try
            {
                int index = ((DataGridColumnHeader)((TextBox)sender).TemplatedParent).Column.DisplayIndex;
                foreach (FilterListItem filterItem in filterLists[index])
                {
                    filterItem.IsTextFiltered = true;
                }
                foreach (FilterListItem filterItem in filterLists[index].Where(item => item.Name.Contains(((TextBox)sender).Text)))
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
            var matches = _workingData.AsEnumerable()
                            .Where(row => Enumerable.Range(0, filterLists.Length)
                            .All(col => filterLists[col]
                                .Where(item =>
                                {
                                    if (item.IsTextFiltered) //se aplica el filtro de texto sobre los filtros individuales
                                    {
                                        item.NotFiltered = false;
                                        item.IsTextFiltered = false; //y se quita el filtrado de texto para que la lista se vea correctamente tras la carga
                                    }
                                    return item.NotFiltered;
                                })
                                .Any(condition => condition.Name == row[col].ToString())));

            ItemsSource = matches.AsDataView();
            backUpFilterList = null;
            temporalListBoxContainer = new List<ListBox>(); //Al reasignar ItemsSource los filtros desplegables se joden, misterios de la naturaleza
        }

        //Vuelve a marcar todo en la interfaz del filtro o si esta todo marcado lo quita todo para que el usuario pueda marcar solo una
        //o 2 cosas sin tener que quitar todas las casillas a mano
        private void Clean_Filter(object sender, EventArgs e)
        {
            int index = ((DataGridColumnHeader)((Button)sender).TemplatedParent).Column.DisplayIndex;
            if (filterLists[index].All(item => item.NotFiltered))
            {
                foreach (FilterListItem item in filterLists[index])
                {
                    item.NotFiltered = false;
                }
            }
            else
            {
                foreach (FilterListItem item in filterLists[index])
                {
                    item.NotFiltered = true;
                }
            }
        }

        //Deshace todos los cambios temporales que el usuario haya hecho sobre la interfaz del filtro y lo cierra
        private void Cancel_Filter(object sender, EventArgs e)
        {
            int index = 0;
            if (backUpFilterList == null)
                return;

            if(sender is Popup)
                index = ((DataGridColumnHeader)((Popup)sender).TemplatedParent).Column.DisplayIndex;
            else if (sender is Button)
                index = ((DataGridColumnHeader)((Button)sender).TemplatedParent).Column.DisplayIndex;

            filterLists[index] = new ObservableCollection<FilterListItem>(backUpFilterList.Select(item => (FilterListItem)item.Clone()));
            backUpFilterList = null;
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
                                    filterLists[cell.Column.DisplayIndex].Remove(filterLists[cell.Column.DisplayIndex].Where(item => item.Name == oldValue).Single());
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

                //si el nuevo valor no existía en el filtro se añade
                if (!filterLists[e.Column.DisplayIndex].Any(item => item.Name == currentValue))
                {
                    filterLists[e.Column.DisplayIndex].Add(new FilterListItem { IsTextFiltered = false, Name = currentValue, NotFiltered = false });
                }

                //si solo había una copia del valor original en toda la tabla, es decir, que ya no hay, se elimina del filtro
                if (_workingData.AsEnumerable().Select(item => item.ItemArray[e.Column.DisplayIndex] == originalValue).Count() > 1)
                {
                    filterLists[e.Column.DisplayIndex].Remove(filterLists[e.Column.DisplayIndex].Where(item => item.Name == originalValue).Single());
                }
            }
        }

        private void MouseDown_Cell(object sender, MouseButtonEventArgs e)
        {
            if (Flags.MultiEdit)
            {
                this._selectedCell = (DataGridCell)sender;
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
                    this.ClipboardCopyMode = (DataGridClipboardCopyMode)(((int)this.ClipboardCopyMode) % 2 + 1);
                }
            }
        }
    }

    /// <summary>
    /// La clase que se usa dentro de la interfaz desplegable del filtro
    /// </summary>
    public class FilterListItem : ICloneable, INotifyPropertyChanged
    {
        private bool notFiltered = true;
        private bool isTextFiltered = false;
        private string name;
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

    public class FilterConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DataGridRow targetObject = values[0] as DataGridRow;
            if (targetObject == null)
            {
                int index = targetObject.GetIndex();
                if (((List<WPFBool>)values[1])[index].Value == true)
                {
                    return true;
                }
            }
            return false;
        }

        public object[] ConvertBack(object values, Type[] targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
