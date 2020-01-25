using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;

namespace ZOT.GUI
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private List<UserControl> Tools = new List<UserControl>();
        public MainWindow()
        { 
            InitializeComponent();
            toolsMenuFlyout.IsOpen = false;
        }

        public void toggle_tool_menu(object sender, RoutedEventArgs e)
        {
            toolsMenuFlyout.IsOpen = true;
        }

        public void click_menuBtn(object sender, RoutedEventArgs e)
        {
            visibleTool.Visibility = Visibility.Collapsed;
        }

        //Evento generado al hacer click en una de las herramientas ya instanciadas de la barra
        //Vuelve a renderizarla en pantalla
        private void ReOpen_App_FromToolBar(object sender, RoutedEventArgs e)
        {
            int index = Grid.GetRow((Tile)sender);
            visibleTool.Content = Tools[index];
            visibleTool.Visibility = Visibility.Visible;
            toolsMenuFlyout.IsOpen = false;
        }

        private void Launch_BLnOFF(object sender, RoutedEventArgs e)
        {
            var BLnOFFApp = new ZOT.BLnOFF.GUI.TabControlBLnOFF();
            visibleTool.Content = BLnOFFApp;

            Tools.Add(BLnOFFApp); //La ventana tiene que mantener la referencia de todas las herramientas abiertas

            // Mostrar en la barra de tareas
            var toolTile = new Tile();
            toolTile.Title = "BlackListing y Offset";
            toolTile.HorizontalTitleAlignment = HorizontalAlignment.Stretch;
            toolTile.Width = Double.NaN; //auto
            toolTile.Height = 100;
            toolTile.Click += new RoutedEventHandler(ReOpen_App_FromToolBar);
            toolTile.VerticalAlignment = VerticalAlignment.Top;

            var rowDef = new RowDefinition();
            rowDef.Height = new GridLength(100);
            toolBar.RowDefinitions.Add(rowDef);
            toolBar.Children.Add(toolTile);
            Grid.SetRow(toolTile, toolBar.RowDefinitions.Count - 1);

 
            visibleTool.Visibility = Visibility.Visible;
        }
    }
}
