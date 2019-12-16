using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Xml;

namespace ZOT.resources
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

        /// <summary>
        /// Versiones alternativas de conversiones de datos adaptadas a las necesidades de los rarisimos inputs de esta empresa
        /// </summary>
        public static class Conversion
        {
            /// <summary>
            /// Convierte strings en cadenas siempre que el valor sea del tipo doble o "", sino habra una excepcion
            /// </summary>
            /// <param name="value">Una cadena con un numero de tipo double ("10,3425") o una cadena vacía ("")</param>
            /// <param name="output">Variable de salida</param>
            /// <returns></returns>
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

    //referencias constantes a los numeros de cada tecnologia de LTE, para hacer el código mas legible
    public static class TECH_NUM
    {
        public const byte L1800 = 0;
        public const byte L800 = 1;
        public const byte L2600 = 2;
        public const byte L900 = 3;
        public const byte L2100 = 4;

    }
}
