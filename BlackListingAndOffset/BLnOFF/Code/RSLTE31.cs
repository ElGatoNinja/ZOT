using System.IO;
using System.Collections.Generic;
using System;
using System.Data;
using ZOT.resources;
using ZOT.resources.ZOTlib;

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
                                    if(aux[7] == "") //Taget desconocido
                                    {
                                        int eci_id = Convert.ToInt32(aux[14]);
                                        int cell_id = eci_id & 0b1111_1111;
                                        int enb_id = (eci_id - cell_id) >> 8;

                                        aux[11] = cell_id.ToString();
                                        aux[8] = enb_id.ToString();
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
                WPFForms.ShowError("No se ha podido encontrar el fichero: " + path);
            }
            inExports = new bool[data.Rows.Count];
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