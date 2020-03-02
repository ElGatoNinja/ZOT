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

            
        
        public Graphs()
        {
            InitializeComponent();

            percentAxisSlider.Minimum = 0;
            percentAxisSlider.Maximum = 102;

            /* ENB_GR_LOS_LLANOS_LOMALI_01
          ENB_H_CARTAYA_NORTE_01
          ENB_H_DONANA_01
          ENB_H_MATALASCANAS_ER_01
          ENB_H_MAZAGON_URBANIZAC_01
          ENB_H_POLO_QUIMICO_EB_01
          ENB_H_PUEBLO_ANDALUZ_EB_01
          ENB_H_VILLANUEVA_CAST_E_01
          */

        }

        /// <summary>
        /// Dibuja una grafica con los controles y el formato específico para el analisis estadistico asociado a blacklisting y offset
        /// </summary>
        /// <param name="data">Colecciones de datos a representar </param>
        /// <param name="maxBarAxis"> El tamaño del elemento mas grande del grafico de barras </param>
        /// <param name="xAxis"> Coleccion de fechas que se quieren poner en el eje x</param>
        public void DrawGraph(SeriesCollection data, int maxBarAxis, List<string> xAxis)
        {
            graph.DisableAnimations = true;
            
            this.xAxis = xAxis;
            graph.AxisY.Clear();
            graph.AxisY.Add(new Axis
            {
                Title = "Intentos",

            });
            graph.AxisY[0].Separator.StrokeThickness = 0;
            graph.AxisY.Add(new Axis
            {
                Title = "Tasa de errores",
                Position = AxisPosition.RightTop
            });


            graph.AxisX.Clear();
            graph.AxisX.Add(new Axis
            {
                Labels = xAxis,
                LabelsRotation = 60,
                Separator = new LiveCharts.Wpf.Separator { Step = 2 }
            }); 
            
            graph.Series = data;
            

            
            //tras dibujar la grafica se ajustan los sliders para estar acordes a los datos
            datesAxisSlider.Maximum = xAxis.Count;
            datesAxisSlider.UpperValue = datesAxisSlider.Maximum;
            datesAxisSlider.Minimum = 0;
            datesAxisSlider.LowerValue = datesAxisSlider.UpperValue - 60 ;
            intentsAxisSlider.Maximum = maxBarAxis * 1.5;
            intentsAxisSlider.Value = maxBarAxis;
            percentAxisSlider.LowerValue = 80;
            percentAxisSlider.UpperValue = 100;
        }
        private void Intents_Range_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            e.Handled = true;
            graph.AxisY[0] = new Axis
            {
                MaxValue = e.NewValue,
            };
            graph.AxisY[0].Separator.StrokeThickness = 0;
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
            if((datesAxisSlider.UpperValue - datesAxisSlider.LowerValue) < 50)
            graph.AxisX[0] = new Axis
            {
                MaxValue = (int)e.NewUpperValue,
                MinValue = (int)e.NewLowerValue,
                Labels = xAxis,
                LabelsRotation = 60,
                Separator = new LiveCharts.Wpf.Separator { Step = 1 }
            };

            else if ((datesAxisSlider.UpperValue - datesAxisSlider.LowerValue) < 70)
                graph.AxisX[0] = new Axis
                {
                    MaxValue = (int)e.NewUpperValue,
                    MinValue = (int)e.NewLowerValue,
                    Labels = xAxis,
                    LabelsRotation = 60,
                    Separator = new LiveCharts.Wpf.Separator { Step = 2 }
                };
            else
                graph.AxisX[0] = new Axis
                {
                    MaxValue = (int)e.NewUpperValue,
                    MinValue = (int)e.NewLowerValue,
                    Labels = xAxis,
                    LabelsRotation = 60,
                    Separator = new LiveCharts.Wpf.Separator { Step = 3 }
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
