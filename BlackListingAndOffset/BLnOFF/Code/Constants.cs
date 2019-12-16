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
            public static double MAX_DIST;
            public static double MAX_DIST_2600;
            public static double MAX_DIST_2100;
            public static double MIN_SUCCESS_HANDOVER;
            public static double MAX_PER_COLIN_800;   //PER-> PERcentage
            public static double MAX_PER_COLIN_1800;
            public static double MAX_PER_COLIN_2600;
            public static double MAX_PER_COLIN_2100;
            public static double MAX_COLIN_800;
            public static double MAX_COLIN_1800;
            public static double MAX_COLIN_2600;
            public static double MAX_COLIN_2100;

            /// <summary>
            /// Clase que contiene todas las definiciones necesarias para el calculo de BlackListing
            /// </summary>
            /// <param name="values">por orden una array con los 12 valores</param>
            public static void SetConst(double[] values)
            {
                MAX_DIST = values[0];
                MAX_DIST_2600 = values[1];
                MAX_DIST_2100 = values[2];
                MIN_SUCCESS_HANDOVER = values[3];
                MAX_PER_COLIN_800 = values[4];
                MAX_PER_COLIN_1800 = values[5];
                MAX_PER_COLIN_2600 = values[6];
                MAX_PER_COLIN_2100 = values[7];
                MAX_COLIN_800 = values[8];
                MAX_COLIN_1800 = values[9];
                MAX_COLIN_2600 = values[10];
                MAX_COLIN_2100 = values[11];
            }
            /// <summary>
            /// Extrae todas las constantes en un array
            /// </summary>
            /// <returns></returns>
            public static double[] GetConst()
            {
                double[] consts = { MAX_DIST, MAX_DIST_2600, MAX_DIST_2100, MIN_SUCCESS_HANDOVER, MAX_PER_COLIN_800, MAX_PER_COLIN_1800, MAX_PER_COLIN_2600, MAX_PER_COLIN_2100, MAX_COLIN_800, MAX_COLIN_1800, MAX_COLIN_2600, MAX_COLIN_2100 };
                return consts;
            } 
        }

        public static class OFF
        {
            public static double PERCENTILE_CELL_RANGE;
            public static double MIN_SUCCESS_HANDOVER;
            public static double MAX_PER_COLIN_800;   //PER-> PERcentage
            public static double MAX_PER_COLIN_1800;
            public static double MAX_PER_COLIN_2600;
            public static double MAX_PER_COLIN_2100;
            public static double MAX_COLIN_800;
            public static double MAX_COLIN_1800;
            public static double MAX_COLIN_2600;
            public static double MAX_COLIN_2100;

            /// <summary>
            /// Clase que contiene todas las definiciones necesarias para el calculo del Offset
            /// </summary>
            /// <param name="values">por orden una array con los 12 valores</param>
            public static void SetConst(double[] values)
            {
                PERCENTILE_CELL_RANGE = values[0];
                MIN_SUCCESS_HANDOVER = values[1];
                MAX_PER_COLIN_800 = values[2];
                MAX_PER_COLIN_1800 = values[3];
                MAX_PER_COLIN_2600 = values[4];
                MAX_PER_COLIN_2100 = values[5];
                MAX_COLIN_800 = values[6];
                MAX_COLIN_1800 = values[7];
                MAX_COLIN_2600 = values[8];
                MAX_COLIN_2100 = values[9];
            }
            /// <summary>
            /// Extrae todas las constantes en un array
            /// </summary>
            /// <returns></returns>
            public static double[] GetConst()
            {
                double[] consts = { PERCENTILE_CELL_RANGE, MIN_SUCCESS_HANDOVER, MAX_PER_COLIN_800, MAX_PER_COLIN_1800, MAX_PER_COLIN_2600, MAX_PER_COLIN_2100, MAX_COLIN_800, MAX_COLIN_1800, MAX_COLIN_2600, MAX_COLIN_2100 };
                return consts;
            }
        }

        public static  class U_INTER
        {
            public static double PER_800;
            public static double PER_1800;
            public static double PER_2600;
            public static double PER_2100;
            /// <summary>
            /// Clase que contiene todas los umbrales de INTER
            /// </summary>
            /// <param name="values">por orden una array con los 12 valores</param>
            public static void SetConst(double[] values)
            {
                PER_800 = values[0];
                PER_1800 = values[1];
                PER_2600 = values[2];
                PER_2100 = values[3];
            }
            /// <summary>
            /// Extrae todas las constantes en un array
            /// </summary>
            /// <returns></returns>
            public static double[] GetConst()
            {
                double[] consts = { PER_800, PER_1800, PER_2600, PER_2100 };
                return consts;
            }
        }

        public static class U_INTRA
        {
            public static double PER_800;
            public static double PER_1800;
            public static double PER_2600;
            public static double PER_2100;
            /// <summary>
            /// Clase que contiene todas los umbrales de INTER
            /// </summary>
            /// <param name="values">por orden una array con los 12 valores</param>
            public static void SetConst(double[] values)
            {
                PER_800 = values[0];
                PER_1800 = values[1];
                PER_2600 = values[2];
                PER_2100 = values[3];
            }
            /// <summary>
            /// Extrae todas las constantes en un array
            /// </summary>
            /// <returns></returns>
            public static double[] GetConst()
            {
                double[] consts = { PER_800, PER_1800, PER_2600, PER_2100 };
                return consts;
            }
        }
    }
}
