using System;
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

namespace ZOT.GUI.Items
{
    /// <summary>
    /// Lógica de interacción para AdvancedTile.xaml
    /// </summary>
    public partial class AdvancedTile : UserControl
    {
        public AdvancedTile()
        {
            InitializeComponent();
            innerForm = this.InnerForm; //Se establece la interfaz oculta del tile que se haya bindeado en la propiedad innerForm en el XAML
        }

        public static readonly DependencyProperty InnerFormProperty =
            DependencyProperty.Register("InnerForm", typeof(UserControl), typeof(AdvancedTile));
        /// <summary>
        /// Permite incluir una interfaz cualquiera dentro de Advanced Tile
        /// </summary>
        public UserControl InnerForm
        {
            get { return (UserControl)GetValue(InnerFormProperty); }
            set { SetValue(InnerFormProperty, value); }
        }
        private void Tile_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
