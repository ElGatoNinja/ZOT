using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Xml;

namespace BlackListingAndOffset.resources
{
    public static class ZOTUtiles
    {
        /// <summary>
        /// Llama al explorador de windows para buscar archivos
        /// </summary>
        /// <param name="filter">Establece que tipo de archivos se pueden seleccionar ej:(Fichero de texto (*.txt)|*.txt)</param>
        /// <param name="title">El titulo de la venatana que se abre</param>
        /// <returns>El path del archivo seleccionado</returns>
        public static string FileFinder(string filter, string title)
        {
            OpenFileDialog explorer = new OpenFileDialog();
            explorer.Filter = filter;
            explorer.Title = title;
            if (explorer.ShowDialog() == true)
            {
                return explorer.FileName;
            }
            return "Error";

        }

        public static class Conversion
        {
            public static bool ToDouble(string value, out Nullable<double> output)
            {
                if (value != "")
                {
                    output = XmlConvert.ToDouble(value);
                    return true;
                }
                else
                {
                    output = null;
                    return false;
                }
            }
        }
    }
}
