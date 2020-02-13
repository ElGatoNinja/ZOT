using LiveCharts;
using LiveCharts.Wpf;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ZOT.BLnOFF.GUI
{
    /// <summary>
    /// Lógica de interacción para Graphs.xaml
    /// </summary>
    public partial class Graphs : UserControl
    {
        private List<string> xAxis;
        public SeriesCollection ShownData { get; set; }
        public List<string> NodeList { get; set;}
        public List<string> TechList { get; set; }
        public ObservableCollection<string> LnCellList { get; set; }
        public DataTable _errors;
        public DataTable Errors 
        {
            get
            {
                return _errors;
            }
            set
            {
                _errors = value;

                //Se obtienen y conectan los nodos y sus tecnologías con las listas de la interfaz
                NodeList = value.AsEnumerable().Select(col => (string)col[1]).Distinct().ToList();
                Binding codeBinding = new Binding("NodeList");
                codeBinding.Source = this;
                siteListBox.SetBinding(ComboBox.ItemsSourceProperty, codeBinding);
                siteListBox.SelectedItem = NodeList[0];

                TechList = value.AsEnumerable().Where(row => (string)row[1] == NodeList[0]).Select(col => (string)col[2]).Distinct().ToList<string>();
                techListBox.ItemsSource = TechList;
                techListBox.SelectedItem = TechList[0];

               


            }
        }
        public Graphs()
        {
            InitializeComponent();

            percentAxisSlider.Minimum = 0;
            percentAxisSlider.Maximum = 102;

            Binding codeBinding = new Binding("ShownData");
            codeBinding.Source = this;
            graph.SetBinding(CartesianChart.SeriesProperty, codeBinding);

            /*          ENB_GR_LOS_LLANOS_LOMALI_01
          ENB_H_CARTAYA_NORTE_01
          ENB_H_DONANA_01
          ENB_H_MATALASCANAS_ER_01
          ENB_H_MAZAGON_URBANIZAC_01
          ENB_H_POLO_QUIMICO_EB_01
          ENB_H_PUEBLO_ANDALUZ_EB_01
          ENB_H_VILLANUEVA_CAST_E_01
          */

        }

        //tras cambiar de nodo se dibuja una grafica de una tecnología por defecto
        private void Node_ComBox_Changed(object sender, SelectionChangedEventArgs e)
        {
            TechList = Errors.AsEnumerable().Where(row => (string)row[1] == (string)siteListBox.SelectedItem).Select(col => (string)col[2]).Distinct().ToList<string>();
            techListBox.ItemsSource = TechList;
            techListBox.SelectedItem = TechList[0];


            DrawGraph();
        }

        private void Tech_ComBox_Changed(object sender, SelectionChangedEventArgs e)
        {
            //Se dibuja una grafica para el nodo y la tecnología elegida
            DrawGraph();
        }

        private void DrawGraph()
        {
            int maxBarAxis = Errors.AsEnumerable()
                                .Where(row => (string)row[1] == (string)siteListBox.SelectedItem && (string)row[2] == (string)techListBox.SelectedItem)
                                .Select(col => (int)col[3] + (int)col[8]).Max<int>();

            ShownData = new SeriesCollection
                {
                    
                    new StackedColumnSeries
                    {
                        Values = new ChartValues<int>(Errors.AsEnumerable()
                                .Where(row => (string)row[1] == (string)siteListBox.SelectedItem && (string)row[2] == (string)techListBox.SelectedItem)
                                .Select(col => (int)col[3]).ToList<int>()),
                        StackMode = StackMode.Values,
                        DataLabels = true,
                        //Fill = System.Windows.Media.Brushes.LightBlue,
                        ScalesYAt = 0
                    },
                    new StackedColumnSeries
                    {
                        Values = new ChartValues<int>(Errors.AsEnumerable()
                                .Where(row => (string)row[1] == (string)siteListBox.SelectedItem && (string)row[2] == (string)techListBox.SelectedItem)
                                .Select(col => (int)col[8]).ToList<int>()),
                        StackMode = StackMode.Values,
                        DataLabels = true,
                        //Fill = System.Windows.Media.Brushes.PaleVioletRed,
                        ScalesYAt = 0
                    },
                    new LineSeries
                    {
                        Values = new ChartValues<double>(Errors.AsEnumerable()
                                .Where(row => (string)row[1] == (string)siteListBox.SelectedItem && (string)row[2] == (string)techListBox.SelectedItem)
                                .Select(col => (double)col[5]).ToList<double>()),
                        ScalesYAt = 1,
                        //Stroke = System.Windows.Media.Brushes.Blue

                    },
                    new LineSeries
                    {
                        Values = new ChartValues<double>(Errors.AsEnumerable()
                                .Where(row => (string)row[1] == (string)siteListBox.SelectedItem && (string)row[2] == (string)techListBox.SelectedItem)
                                .Select(col => (double)col[10]).ToList<double>()),
                        //Fill = System.Windows.Media.Brushes.Red,
                        ScalesYAt = 1

                    }
                };
            graph.AxisY.Clear();
            graph.AxisY.Add(new Axis
            {
                Title = "Intentos",

            });
            graph.AxisY.Add(new Axis
            {
                Foreground = System.Windows.Media.Brushes.IndianRed,
                Title = "Tasa de errores",
                Position = AxisPosition.RightTop
            });

            

            graph.AxisX.Clear();
            xAxis = Errors.AsEnumerable().Where(row => (string)row[1] == (string)siteListBox.SelectedItem && (string)row[2] == (string)techListBox.SelectedItem)
                                       .Select(col => col[0].ToString()).ToList<string>();
            

            graph.AxisX.Add(new Axis
            {
                Labels = xAxis,
                LabelsRotation = 60,
                Separator = new LiveCharts.Wpf.Separator { Step = 1 }
            });
            
            graph.Series = ShownData;

            
            //tras dibujar la grafica se ajustan los sliders para estar acordes a los datos
            datesAxisSlider.Maximum = xAxis.Count;
            datesAxisSlider.UpperValue = datesAxisSlider.Maximum;
            datesAxisSlider.Minimum = 0;
            datesAxisSlider.LowerValue = 0;
            intentsAxisSlider.Maximum = maxBarAxis * 1.5;
            intentsAxisSlider.Value = intentsAxisSlider.Maximum;
            percentAxisSlider.LowerValue = 80;
            percentAxisSlider.UpperValue = 100;
        }
        private void Intents_Range_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            e.Handled = true;
            graph.AxisY[0] = new Axis
            {
                MaxValue = e.NewValue
            };
        }

        private void X_Axis_Range_Changed(object sender, RangeSelectionChangedEventArgs e)
        {
            e.Handled = true;
            if((int)e.NewUpperValue-(int)e.NewLowerValue < 1) //si el tamaño del eje tiende a 0, habrá un error en la gráfica
            {
                datesAxisSlider.UpperValue = e.OldUpperValue;
                datesAxisSlider.LowerValue = e.OldLowerValue;
                return;
            }
            graph.AxisX[0] = new Axis
            {
                MaxValue = (int)e.NewUpperValue,
                MinValue = (int)e.NewLowerValue,
                Labels = xAxis,
                LabelsRotation = 60,
                Separator = new LiveCharts.Wpf.Separator { Step = 1 }
            };
        }

        private void Percent_Range_Changed(object sender, RangeSelectionChangedEventArgs e)
        {
            e.Handled = true;
            if (e.NewUpperValue - e.NewLowerValue < 1) //si el tamaño del eje tiende a 0, habrá un error en la gráfica
            {
                percentAxisSlider.UpperValue = e.OldUpperValue+0.1;
                percentAxisSlider.LowerValue = e.OldLowerValue-0.1;
                return;
            }
            graph.AxisY[1] = new Axis
            {
                MaxValue = e.NewUpperValue,
                MinValue = e.NewLowerValue,
                
                Title = "Tasa de Errores",
                Position = AxisPosition.RightTop
            };
        }

    }
}
