using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ZOT.GUI;

namespace ZOT.BLnOFF.GUI
{
    /// <summary>
    /// Lógica de interacción para INICIO.xaml
    /// </summary>
    public partial class INICIO : Window
    {
        public INICIO()
        {
            InitializeComponent();


            haciendoCargaAsync();
        }



        private async Task haciendoCargaAsync()
        {
            this.Show();

            Console.WriteLine("START");

            Random rnd = new Random();

            int seg = rnd.Next(1000, 3500);
            await PutTaskDelay(seg);
            Console.WriteLine("END " +seg.ToString() +"mseconds");



            Console.WriteLine("Abriendo ZOT");

            this.Hide();
            MainWindow mw = new MainWindow();
            mw.Show();
            mw.Activate();

        }

        async Task PutTaskDelay(int seg)
        {
            await Task.Delay(seg);
        }



        void INICIO_Closing(object sender, CancelEventArgs e)
        {
            Console.WriteLine("Cerrando ZOT");
            Environment.Exit(1);
        }


    }

}
