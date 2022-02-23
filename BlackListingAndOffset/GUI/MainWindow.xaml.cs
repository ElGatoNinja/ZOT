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
using ZOT.HORAS48.GUI;
using ZOT.Colindancias.GUI;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Forms;
using ZOT.BLnOFF.GUI;

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

            Show();

            WPFForms.window = this; //permite a ZOTlib lanzar mensajes sobre esta ventana
            _grid = toolBar;
            toolsMenuFlyout.IsOpen = false;












 

        }

        public void toggle_tool_menu(object sender, RoutedEventArgs e)
        {
            toolsMenuFlyout.IsOpen = true;
        }

        public void click_menuBtn(object sender, RoutedEventArgs e)
        {
            visibleTool.Visibility = Visibility.Hidden;
            infoPage.Visibility = Visibility.Collapsed;
        } 
        
        public void click_open_folder(object sender, RoutedEventArgs e)
        {
            String pathFolder = System.IO.Path.Combine(Environment.CurrentDirectory, @"Data");

            Process.Start("explorer.exe", pathFolder);


        }        
        public void click_open_info(object sender, RoutedEventArgs e)
        {
            HistorialVersiones historialVersiones = new HistorialVersiones();
            historialVersiones.Show();


        }
        


        public void click_load_siteCoordr(object sender, RoutedEventArgs e)
        {

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "CSV Files|*.csv;";
            ofd.Title = "Selecciona el Histórico Norte ";
            String ruta = "";


            String pathFolder = System.IO.Path.Combine(Environment.CurrentDirectory, @"Data") + "\\SiteCoord.csv";


            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
               ruta = ofd.FileName;

                try
                {

                    File.Copy(ruta, pathFolder, true);
                    System.Windows.MessageBox.Show("SiteCoord actualizado correctamente.");
                }
                catch(Exception)
                {
                    System.Windows.MessageBox.Show("Error al copiar el nuevo SiteCoord");
                }



            }
            else
            {
                System.Windows.MessageBox.Show("Error al cargar el fichero SiteCoord nuevo");
            }


        }







        public void Click_ShowInfo(object sender, RoutedEventArgs e)
        {
            infoFlyout.IsOpen = !infoFlyout.IsOpen;
        }

        //Evento generado al hacer click en una de las herramientas ya instanciadas de la barra
        //Vuelve a renderizarla en pantalla
        public void ReOpen_App_FromToolBar(object sender, RoutedEventArgs e)
        {
            visibleTool.Content = WPFForms.FindParent<NavBarTile>((Tile)sender).app;
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

            try
            {
                StartApp(new ZOT.BLnOFF.GUI.TabControlBLnOFF());
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
          


            /*
            Window UserControlNewWindow = new Window
            {
                Title = "Some Title",
                Content = new ZOT.BLnOFF.GUI.TabControlBLnOFF()
            };

            UserControlNewWindow.ShowDialog();
            */

        }

        private void Launch_48H(object sender, RoutedEventArgs e)
        {
            
            Formulario48 form48 = new Formulario48();
            form48.Show();
        }

        
        private void Launch_Window48Horas(object sender, RoutedEventArgs e)
        {
            Ventana48H w = new Ventana48H();
            w.Show();

        }
        
        private void Launch_Colindancias(object sender, RoutedEventArgs e)
        {
            VentanaColindancias col = new VentanaColindancias();
            col.Show();

        }






        
        private void Launch_ComprobarTipoObra(object sender, RoutedEventArgs e)
        {

            ComprobarTipoObra cto = new ComprobarTipoObra();
            cto.Show();
        }
        
        private void Launch_ZelenzaWeb(object sender, RoutedEventArgs e)
        {
            //System.Diagnostics.Process.Start("https://www.zelenza.com");

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
;
            toolBar.Children.Add(toolTile);

 
            visibleTool.Visibility = Visibility.Visible;
        }
    

        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            Console.WriteLine("Cerrando ZOT");
            Environment.Exit(1);
        }

        void MainWindow_Activated(object sender, EventArgs e)
        {
            //Me aburria
            String[] fraseRandom = new string[] { "Muchas ideas crecen mejor cuando se trasplantan a otra mente diferente de la que surgieron - Oliver Wendell Holmes", "Juntarse es un comienzo, seguir juntos es un progreso y trabajar juntos es un éxito - Henry Ford.", "Any fool can write code that a computer can undertand. Good programmers write code that humans can understand.", "No importa lo despacio que vayas, siempre y cuando no te detengas - Confucio", "No esperes que las oportunidades lleguen solas. Tienes que hacer que ocurra - Dnis Diderot", "No busques los errores, busca la solución - Henry Ford (Y sino envia un email :) )", " Si crees que puedes, ya estás a medio camino - Theodore Roosevelt", "Trabajo deprisa para vivir despacio.", "Push yourself, because no one else is going to do it for you.", "Great things never come from comfort zones." };
            Random rnd = new Random();
            int numerorandom = rnd.Next(0, 10);
            iconoZelenza.ToolTip.ToString();


            iconoZelenza.ToolTip = fraseRandom[numerorandom].ToString();
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
