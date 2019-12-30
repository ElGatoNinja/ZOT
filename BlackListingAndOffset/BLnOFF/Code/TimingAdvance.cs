using System.IO;
using System.Collections.Generic;
using System.Data;
using System;
using ZOT.resources;
using System.Globalization;

namespace ZOT.BLnOFF.Code
{
    public class TimingAdvance
    {
        public DataTable data;
        public DataTable radioLines;
        private const byte _FIRST_VALUE_COL = 6;
        //_RADIO_TABLE_KM se corresponde con los titulos de el archivo TimingAdvance. Dios no quiera que tengas que retocar estos valores futuro becario
        private readonly double[] _RADIO_TABLE_KM = { 0.078, 0.156, 0.312, 0.468, 0.624, 0.780, 1.092, 1.404, 1.794, 2.262, 2.262, 0.5, 1, 1.5, 2, 2.7, 3.4, 4.1, 4.8, 5.6, 5.6, 1, 2, 3, 4, 5.3, 6.9, 8.6, 9.5, 11.1, 11.1, 1.5, 3, 4.5, 6, 8, 10.4, 12.9, 14.6, 16.6, 16.6, 3, 6, 9, 12, 16, 21, 26, 33, 33, 6, 12, 15, 18, 24, 32, 41, 52, 63, 63, 10, 20, 30, 40, 53, 69, 87, 105, 105 };

        public TimingAdvance(string[] lnBtsInputs, string path)
        {
            data = new DataTable();
            data.Columns.Add("PERIOD_START_TIME", typeof(string));
            data.Columns.Add("MRBTS/SBTS name", typeof(string));
            data.Columns.Add("LNBTS name", typeof(string));
            data.Columns.Add("LNCEL name", typeof(string));
            for (int i = 4; i < 75; i++)
            {
                data.Columns.Add("col" + i, typeof(string));
            }

            using (StreamReader reader = new StreamReader(path))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    object[] aux = (object[])line.Split(';');

                    //Se guarda cada una de la lineas del Timing Advance cuyo LNCEL sea uno de los que el usuario quiere comprobar 
                    foreach (string lnBts in lnBtsInputs)
                    {
                        if (aux[2].Equals(lnBts))
                        {
                            for (int i = 0; i < 75; i++) //hay que eliminar los ""
                            {
                                if (aux[i].Equals(""))
                                    aux[i] = "0";
                            }
                            data.Rows.Add(aux);
                        }

                    }
                }
            }
            GetRadioLines();
        }


        /* funcion que calcula el radio de efecto de las lineas en funcion del valor de PERCENTILE_CELL_RANGE
         * a partir de la información extraida del TimingAdvance*/
        private void GetRadioLines()
        {
            radioLines = new DataTable();
            radioLines.Columns.Add("LNCELL", typeof(string));
            radioLines.Columns.Add("RADIO", typeof(double));
            for (int j = 0; j < data.Rows.Count; j++)
            {
                double accumulatedValue = 0;
                int i = _FIRST_VALUE_COL;
                while (accumulatedValue < ZOT.BLnOFF.Code.CONSTANTS.OFF.PERCENTILE_CELL_RANGE && i<75)
                {
                    accumulatedValue += Convert.ToDouble(data.Rows[j][i], CultureInfo.GetCultureInfo("en-US"));
                    i++;
                    
                }
                //Esto significa que no hay registros en timing advance, posiblemente porque esté bloqueada
                if (i == 75) continue;

                Object[] line = new Object[2] { data.Rows[j]["LNCEL name"], _RADIO_TABLE_KM[i - _FIRST_VALUE_COL-1] };
                radioLines.Rows.Add(line);
            }
        }
        ///<summary>Funcion que devuelve una columna de la tabla como String[] </summary>
        /// <param name="name" > El nombre de la columna que se quiere extraer </param>
        public String[] GetColumn(string name)
        {
            String[] column = new String[data.Rows.Count];
            for (int i = 0; i < data.Rows.Count; i++)
            {
                column[i] = data.Rows[i][name].ToString();
            }
            return column;
        }
    }

}

