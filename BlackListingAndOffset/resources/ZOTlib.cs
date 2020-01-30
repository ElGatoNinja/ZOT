using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Data;
using System.Windows.Forms;
using System.Windows;
using System.Windows.Media;

namespace ZOT.resources
{
    namespace ZOTlib
    {
        public static class ZOTFiles
        {
            /// <summary>
            /// Llama al explorador de windows para buscar archivos
            /// </summary>
            /// <param name="filter">Establece que tipo de archivos se pueden seleccionar ej:(Fichero de texto (*.txt)|*.txt)</param>
            /// <param name="title">El titulo de la venatana que se abre</param>
            /// <returns>El path del archivo seleccionado</returns>
            public static string FileFinder(string filter, string title)
            {
                System.Windows.Forms.OpenFileDialog explorer = new OpenFileDialog();
                explorer.Filter = filter;
                explorer.Title = title;
                if (explorer.ShowDialog() == DialogResult.OK)
                {
                    return explorer.FileName;
                }
                return "error";
            }

            /// <summary>
            /// Llama al explorador de windows para buscar archivos
            /// </summary>
            /// <param name="filter">Establece que tipo de archivos se pueden seleccionar ej:(Fichero de texto (*.txt)|*.txt)</param>
            /// <param name="title">El titulo de la venatana que se abre</param>
            /// <param name="initDir">Directorio en el que se empezará la busqueda</param>
            /// <returns>El path del archivo seleccionado</returns>
            public static string FileFinder(string filter, string title, string initDir)
            {
                System.Windows.Forms.OpenFileDialog explorer = new OpenFileDialog();
                explorer.InitialDirectory = initDir;
                explorer.Filter = filter;
                explorer.Title = title;
                if (explorer.ShowDialog() == DialogResult.OK)
                {
                    return explorer.FileName;
                }
                return "error";
            }

            /// <summary>
            /// Llama al explorador de windows para elegir un directorio
            /// </summary>
            /// <param name="title">El titulo de la venatana que se abre</param>
            /// <returns>El path del directorio</returns>
            public static string SetDirectory(string title)
            {
                using (FolderBrowserDialog explorer = new FolderBrowserDialog())
                {
                    explorer.Description = title;
                    explorer.SelectedPath = Directory.GetCurrentDirectory();
                    if (explorer.ShowDialog() == DialogResult.OK)
                    {
                        return explorer.SelectedPath;
                    }
                    return "error";
                }
            }
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

        /// <summary>
        /// Contiene funciones utiles para manejar formularios de WPF
        /// </summary>
        public static class WPFForms
        {
            /// <summary>
            /// Escala el arbol visual de wpf hasta encontrar un elemento del tipo de T
            /// </summary>
            /// <typeparam name="T">El tipo del padre que se está buscando</typeparam>
            /// <param name="child">El control del que se busca su padre</param>
            /// <returns></returns>
            public static T FindParent<T>(DependencyObject child)
                where T:DependencyObject
            {
                DependencyObject item = child;
                do
                {
                    item = LogicalTreeHelper.GetParent(item);
                    if (item is T)
                        return (item as T);
                } while (item != null);
                return null;
            }

            /// <summary>
            /// Lanza un mensaje de error emergente que interrumpe la ejecucion hasta que se cierre
            /// </summary>
            /// <param name="error">Texto que debe mostrar el error</param>
            public static void ShowError(string error)
            {
                GUI.ErrorBox err = new GUI.ErrorBox(error);
                err.ShowDialog();
            }
        }

        /// <summary>
        /// referencias constantes a los numeros de cada tecnologia de LTE, para hacer el código mas legible 
        /// y metodos utiles para usarlas
        /// </summary>
        public static class TECH_NUM
        {
            public const byte L1800 = 0;
            public const byte L800 = 1;
            public const byte L2600 = 2;
            public const byte L900 = 3;
            public const byte L2100 = 4;

            /// <summary>
            /// Extrae la tecnología de los ultimos 3 numeros del LNCEL
            /// </summary>
            /// <param name="LNCELname">El LNCEL que del que se quiere extraer la tecnología</param>
            /// <returns></returns>
            public static byte GetTechFromLNCEL(string LNCELname)
            {
                string[] aux = LNCELname.Split('_');
                return (byte)(Convert.ToInt32(aux[aux.Length - 1]) % 10);
            }

            /// <summary>
            /// Retorna el nombre de la tecnología de 4G correspondiente
            /// </summary>
            /// <param name="techNum">Numero de la tecnología</param>
            /// <returns></returns>
            public static string GetName(byte techNum)
            {
                switch(techNum)
                {
                    case 0:
                        return "L1800";
                    case 1:
                        return "L800";
                    case 2:
                        return "L2600";
                    case 3:
                        return "L900";
                    case 4:
                        return "L2100";
                    default:
                        return "error";
                }
            }
        }
    }

    

}
