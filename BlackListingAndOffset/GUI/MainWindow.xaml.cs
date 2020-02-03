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
using ZOT.GUI.Items;
using ZOT.resources.ZOTlib;

namespace ZOT.GUI
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private List<IZotApp> Tools = new List<IZotApp>();
        private Grid _grid;

        public Grid Grid { get { return _grid; } }
        public MainWindow()
        { 
            InitializeComponent();
            _grid = toolBar;
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
        public void ReOpen_App_FromToolBar(object sender, RoutedEventArgs e)
        {
            int index = Grid.GetRow(WPFForms.FindParent<NavBarTile>((Tile)sender));
            visibleTool.Content = Tools[index];
            visibleTool.Visibility = Visibility.Visible;
            toolsMenuFlyout.IsOpen = false;
        }

        public void RemoveApp(IZotApp app)
        {
            visibleTool.Content = null;
            visibleTool.Visibility = Visibility.Collapsed;
            Tools.Remove(app);
        }
        private void Launch_BLnOFF(object sender, RoutedEventArgs e)
        {
            StartApp(new ZOT.BLnOFF.GUI.TabControlBLnOFF());
        }
        private void StartApp(IZotApp app)
        {
            IZotApp zotApp = app;
            visibleTool.Content = app;

            Tools.Add(zotApp); //La ventana tiene que mantener la referencia de todas las herramientas abiertas

            // Mostrar en la barra de tareas
            var toolTile = new NavBarTile(this, app)
            {
                Width = Double.NaN, //auto
                Height = 100,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0, 100 * toolBar.Children.Count, 0, 0)

        };
            //var rowDef = new RowDefinition();
            //rowDef.Height = new GridLength(100);
            //toolBar.RowDefinitions.Add(rowDef);
            toolBar.Children.Add(toolTile);
            //Grid.SetRow(toolTile, toolBar.RowDefinitions.Count - 1);
 
            visibleTool.Visibility = Visibility.Visible;
        }
    }

    /// <summary>
    /// Las herramientas que implementen esta interfaz podrán usar el launcher y sus elementos
    /// </summary>
    public interface IZotApp
    {
        /// <summary>
        /// El nombre de la aplicacion que el launcher debe mostrar
        /// </summary>
        string AppName { get; }

        bool Notify { get; set; }

    }
}
