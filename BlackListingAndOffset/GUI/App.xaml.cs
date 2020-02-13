using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ZOT.resources.ZOTlib;

namespace ZOT.GUI
{
    /// <summary>
    /// Lógica de interacción para App.xaml
    /// </summary>
    public partial class App : Application
    {   
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            //WPFForms.ShowError("Error crítico", e.Exception.Message);
            Console.WriteLine("ZOT ha capturado una excepcion no controlada en " + e.Exception.StackTrace);
            e.Handled = true;
        }
    }
}
