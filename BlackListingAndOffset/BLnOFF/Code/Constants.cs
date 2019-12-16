using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZOT.BlnOFF.Code
{
    /// <summary>
    /// Clase que contiene todas las definiciones necesarias para la aplicacion de blacklisting and offset
    /// </summary>
    public static class CONSTANTS
    {
        private static string constants_path = Path.Combine(Environment.CurrentDirectory, @"BlnOFF\Data\", "BLnOFFconstants.txt");

        /// <summary>
        /// Funcion que carga todas las constantes estaticas, debería usarse al inicio de la aplicación
        /// </summary>
        public static void LoadConst()
        {
            using (StreamReader reader = new StreamReader(constants_path))
            {
                string[] valuesBL = reader.ReadLine().Split(';');
                CONSTANTS.BL.SetConst(Array.ConvertAll<string, double>(valuesBL, new Converter<string, double>(Convert.ToDouble)));
                string[] valuesOFF = reader.ReadLine().Split(';');
                CONSTANTS.OFF.SetConst(Array.ConvertAll<string, double>(valuesOFF, new Converter<string, double>(Convert.ToDouble)));
                string[] valuesUmbralInter = reader.ReadLine().Split(';');
                CONSTANTS.U_INTER.SetConst(Array.ConvertAll<string, double>(valuesUmbralInter, new Converter<string, double>(Convert.ToDouble)));
                string[] valuesUmbralIntra = reader.ReadLine().Split(';');
                CONSTANTS.U_INTRA.SetConst(Array.ConvertAll<string, double>(valuesUmbralIntra, new Converter<string, double>(Convert.ToDouble)));
            }
        }
        /// <summary>
        /// Guarda el estado actual de las constantes en un txt para que no se pierda la info entre ejecuciones
        /// </summary>
        public static void SaveConst()
        {
            
            String line1 = string.Join(";", ZOT.BlnOFF.Code.CONSTANTS.BL.GetConst());
            String line2 = string.Join(";", ZOT.BlnOFF.Code.CONSTANTS.OFF.GetConst());
            String line3 = string.Join(";", ZOT.BlnOFF.Code.CONSTANTS.U_INTER.GetConst());
            String line4 = string.Join(";", ZOT.BlnOFF.Code.CONSTANTS.U_INTRA.GetConst());

            //En debug parece no funcionar, eso es porque cada vez que se compila se pasa el txt de constantes al path del ejecutable, y cuando aqui se intenta guardar,
            //se guarda en el txt que hemos copiado, pero el original del proyecto no es alterado, si se ejecuta el codigo sin compilar, o en la version Release
            //los cambios se mantienen si ningun problema
            using (StreamWriter file = new System.IO.StreamWriter(constants_path))
            {
                file.WriteLine(line1);
                file.WriteLine(line2);
                file.WriteLine(line3);
                file.WriteLine(line4);
            }
        }

        public static class BL
        {
            public static double[] MAX_DIST;
            public static double[] MAX_PER_COLIN;   //PER-> PERcentage
            public static double[] MAX_COLIN;
            public static double MIN_SUCCESS_HANDOVER;

            /// <summary>
            /// Clase que contiene todas las definiciones necesarias para el calculo de BlackListing
            /// </summary>
            /// <param name="values">por orden una array con los 12 valores</param>
            public static void SetConst(double[] values)
            {
                MAX_DIST = new double[]{values[0],values[1],values[2],values[3],values[4]};
                MAX_PER_COLIN= new double[] { values[5], values[6], values[7], values[8], values[9] };
                MAX_COLIN = new double[] { values[10], values[11], values[12], values[13], values[14]};
                MIN_SUCCESS_HANDOVER = values[15];
            }
            /// <summary>
            /// Extrae todas las constantes en un array
            /// </summary>
            /// <returns></returns>
            public static double[] GetConst()
            {
                List<double> consts = MAX_DIST.Concat(MAX_PER_COLIN).Concat(MAX_COLIN).ToList();
                consts.Add(MIN_SUCCESS_HANDOVER);
                return consts.ToArray();
            } 
        }

        public static class OFF
        {
            public static double PERCENTILE_CELL_RANGE;
            public static double MIN_SUCCESS_HANDOVER;
            public static double[] MAX_PER_COLIN;
            public static double[] MAX_COLIN;

            /// <summary>
            /// Clase que contiene todas las definiciones necesarias para el calculo del Offset
            /// </summary>
            /// <param name="values">por orden una array con los 12 valores</param>
            public static void SetConst(double[] values)
            {  
                MAX_PER_COLIN = new double[] {values[0], values[1],values[2],values[3],values[4]};
                MAX_COLIN = new double[] { values[5], values[6], values[7], values[8], values[9]};
                PERCENTILE_CELL_RANGE = values[10];
                MIN_SUCCESS_HANDOVER = values[11];
            }
            /// <summary>
            /// Extrae todas las constantes en un array
            /// </summary>
            /// <returns></returns>
            public static double[] GetConst()
            {
                List<double> consts = MAX_PER_COLIN.Concat(MAX_COLIN).ToList();
                consts.Add(PERCENTILE_CELL_RANGE);
                consts.Add(MIN_SUCCESS_HANDOVER);
                return consts.ToArray();
            }
        }

        public static  class U_INTER
        {
            public static double[] PER;

            /// <summary>
            /// Clase que contiene todas los umbrales de INTER
            /// </summary>
            /// <param name="values">por orden una array con los 12 valores</param>
            public static void SetConst(double[] values)
            {
                PER = new double[] { values[0], values[1], values[2], values[3], values[4] };
            }
            /// <summary>
            /// Extrae todas las constantes en un array
            /// </summary>
            /// <returns></returns>
            public static double[] GetConst()
            {
                return PER;
            }
        }

        public static class U_INTRA
        {
            public static double[] PER;
            /// <summary>
            /// Clase que contiene todas los umbrales de INTER
            /// </summary>
            /// <param name="values">por orden una array con los 12 valores</param>
            public static void SetConst(double[] values)
            {
                PER = new double[] { values[0], values[1], values[2], values[3], values[4] };
            }
            /// <summary>
            /// Extrae todas las constantes en un array
            /// </summary>
            /// <returns></returns>
            public static double[] GetConst()
            {
                return PER;
            }
        }
    }
}
