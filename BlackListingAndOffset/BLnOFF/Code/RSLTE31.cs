using System.IO;
using System.Collections.Generic;
using System;
using System.Data;
using ZOT.resources;

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
                        foreach (string lnBts in lnBtsInputs)
                        {
                            if (aux[3].Equals(lnBts))
                            {
                                data.Rows.Add(aux);
                                break;
                            }
                        }
                    }
                }
            }
            catch(FileNotFoundException)
            {
                ZOTUtiles.ShowError("El no se ha podido encontrar el fichero: " + path);
            }
            catch(Exception e)
            {
                ZOTUtiles.ShowError("Algo ha salido mal en el procesado de la consulta RSLTE31. Error: " + e.Message);
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