using System.IO;
using System.Collections.Generic;
using System;
using System.Data;
using BlackListingAndOffset.resources;

public class RSLTE31 : FileReader
{
    public DataTable data;
    public RSLTE31(string[] lnBtsInputs) : base("Archivo CSV (*.csv)|*.csv","SElecciona la consulta RSLTE31")
    {
        #if DEBUG
            _path[0] = Directory.GetCurrentDirectory() + "\\data\\R31.csv";
        #endif

        data = new DataTable();
        using (StreamReader reader = new StreamReader(_path[0]))
        {
            string[] aux = reader.ReadLine().Split(';'); //Linea de titulo

            foreach( string col in aux)
            {
                data.Columns.Add(col);
            }

            while(!reader.EndOfStream)
            {
                aux = reader.ReadLine().Split(';');
                foreach(string lnBts in lnBtsInputs)
                {
                    if(aux[3].Equals(lnBts))
                    {
                        data.Rows.Add(aux);
                        break;
                    }
                }
            }
        }
    }

    ///<summary>Funcion que devuelve una columna de los datos extraidos del RLTE31 como String[] </summary>
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