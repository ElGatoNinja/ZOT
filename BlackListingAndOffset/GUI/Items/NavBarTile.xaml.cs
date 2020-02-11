using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using ZOT.resources.ZOTlib;

namespace ZOT.GUI.Items
{
    /// <summary>
    /// Lógica de interacción para NavBarTile.xaml
    /// </summary>
    public partial class NavBarTile : UserControl
    {
        private MainWindow window;
        public IZotApp app;

        /// <summary>
        /// Informacion que muestra el contexto de la instancia de la aplicacion
        /// </summary>
        public string PreviewInfo { get; set; }

        /// <summary>
        /// Resalta la tile para indicar que algo relevante ha sucedido en esta instancia de la apliacion
        /// y se requiere interaccion del ususario
        /// </summary>
        public bool Notify { get; set; }

        public NavBarTile(MainWindow owner, IZotApp app)
        {
            window = owner;
            InitializeComponent();
            mainBtn.Title = app.AppName;
            this.app = app;
            mainBtn.Click += new RoutedEventHandler(owner.ReOpen_App_FromToolBar);
            
        }

        //Elimina control de la barra de tareas y pide a main window que elimine la instancia de la herramienta
        private void CloseThisApp(object sender, RoutedEventArgs e)
        {
            int index = window.toolBar.Children.IndexOf(WPFForms.FindParent<NavBarTile>((DependencyObject)sender));

            for (int i = index + 1; i < window.toolBar.Children.Count; i++)
            {
                ((NavBarTile)window.toolBar.Children[i]).Margin = new Thickness(0, ((NavBarTile)window.toolBar.Children[i]).Margin.Top - 100, 0, 0);
            }
            window.toolBar.Children.RemoveAt(index);
            window.RemoveApp(this.app);
        }
    }
}
