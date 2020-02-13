using System.IO;
using System.Collections.Generic;
using System;
using System.Data;
using ZOT.resources;
using ZOT.resources.ZOTlib;
using System.Linq;
using System.Data.OleDb;
using static ZOT.BLnOFF.GUI.TabControlBLnOFF;
using System.ComponentModel;

namespace ZOT.BLnOFF.Code
{
    public class RSLTE31
    {
        public DataTable data;
        public bool[] inExports;
        public RSLTE31(string[] lnBtsInputs, string path)
        {
            data = new DataTable();
            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    string[] aux = reader.ReadLine().Split(';'); //Linea de titulo

                    foreach (string col in aux)
                    {
                        data.Columns.Add(col);
                    }

                    while (!reader.EndOfStream)
                    {
                        aux = reader.ReadLine().Split(';');
                        if (aux.Length > 4)
                        {
                            foreach (string lnBts in lnBtsInputs)
                            {
                                if (aux[3].Equals(lnBts))
                                {
                                    if(aux[9] == "" || aux[9] == "0") //Taget desconocido
                                    {
                                        int eci_id = Convert.ToInt32(aux[14]);
                                        int cell_id = eci_id & 0b1111_1111;
                                        int enb_id = (eci_id - cell_id) >> 8;

                                        aux[11] = cell_id.ToString();
                                        aux[8] = enb_id.ToString();
                                        aux[9] = "UNKNOWN";
                                    }
                                    data.Rows.Add(aux);
                                    break;
                                }
                            }
                        }
                    }
                }
                 
            }
            catch(FileNotFoundException)
            {
                throw new FileNotFoundException("No se ha podido encontrar la consulta 31 en " + path);
            }
            inExports = new bool[data.Rows.Count];

        }
        /// <summary>
        /// Intenta completar la consulta 31 a partir de los exports, en caso de faltar algo
        /// </summary>
        /// <param name="pathSRAN">Direccion de el export SRAN</param>
        /// <param name="pathFL18">Direccion del export FL18</param>
        public void completeR31(string pathSRAN, string pathFL18)
        {
            var emptyData = data.AsEnumerable().Where(row => (string)row[9] == "UNKNOWN").ToList();
            if (emptyData == null)
                return;

            string directionSRAN = string.Format("provider=Microsoft.ACE.OLEDB.12.0;Data Source= {0}", pathSRAN);
            string directionFL18 = string.Format("provider=Microsoft.ACE.OLEDB.12.0;Data Source= {0}", pathFL18);

            DataTable exportData = new DataTable();

            //no se pueden hacer busquedas sql con mas de 99 OR y AND, así que el siguiente bucle hace consultas "paginadas"
            //hace una busqueda para cada 48 emplazamientos, 49 * (1OR + 1AND) + 1AND = 99
            int i = 0;
            while (i < emptyData.Count)
            {
                string SQLconditions = "(lnCelId = " + emptyData[0][11] + " AND lnBtsId = " + emptyData[0][8] + ")";
                for (int j = 0; j < 49 && i < emptyData.Count; j++)
                {
                    SQLconditions += " OR (lnCelId = " + emptyData[i][11] + " AND lnBtsId = " + emptyData[i][8] + ")";
                    i++;
                }

                string SQLquerry = "SELECT name, lnCelId, lnBtsId "
                    + "FROM A_LTE_MRBTS_LNBTS_LNCEL "
                    + "WHERE  " + SQLconditions + ";";
 
                using (OleDbConnection connectionSRAN = new OleDbConnection(directionSRAN))
                using (OleDbConnection connectionFL18 = new OleDbConnection(directionFL18))
                {
                    OleDbCommand commandSRAN = new OleDbCommand(SQLquerry, connectionSRAN);
                    OleDbCommand commandFL18 = new OleDbCommand(SQLquerry, connectionFL18);

                    //se cargan los resultados de la base de datos en una unica tabla

                    try
                    {
                        connectionSRAN.Open();
                        using (OleDbDataReader SRANreader = commandSRAN.ExecuteReader())
                        {
                            exportData.Load(SRANreader);
                        }

                        connectionFL18.Open();
                        using (OleDbDataReader FL18reader = commandFL18.ExecuteReader())
                        {
                            exportData.Load(FL18reader);
                        }
                    }
                    catch (InvalidOperationException)
                    {
                        throw new InvalidOperationException("No se han podido acceder a access, podría deberse a no tener instalado Microsoft Access Engine 2010" +
                            "Redistributable o usar una version de compilacion no compatible con la version de office instalada actualmente. En cualquier caso es un problema" +
                            "complicado, avisad al informático más cercano. Y si no encuentra la solución dejadme un mensaje");
                    }
                }
            }
            /*busca en el dataset cada fila que se corresponde a un hueco vacío y lo rellena en la tabla
                ENB_CO_CORTE_INGLES_TEJARES_02
                ENB_H_ANDEVALO_SUR_01
                ENB_J_LA_GARZA_GOLF_01
                */


            foreach (DataRow dataRow in emptyData)
            {
                foreach(DataRow exportRow in exportData.Rows)
                {
                    if ((string)dataRow[8] == exportRow[2].ToString() && (string)dataRow[11] == exportRow[1].ToString()) // si conincide enbid y cell id 
                    {
                        dataRow[9] = exportRow[0]; //se rellena el nombre 
                    }
                }
            }
        }

        public List<DataRow> NotInExports()
        {
            List<DataRow> linesNotInExports = new List<DataRow>();
            for (int i = 0; i < data.Rows.Count; i++)
            {
                if (!inExports[i])
                    linesNotInExports.Add(data.Rows[i]);
            }
            return linesNotInExports;
        }
    }
}