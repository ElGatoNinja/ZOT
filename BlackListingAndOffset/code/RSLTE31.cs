using System.IO;
using System.Collections.Generic;
using System;
using System.Data;
using BlackListingAndOffset.resources;

public class RSLTE31 : GenericTable
{
    public bool[] inExports;
    public RSLTE31(string[] lnBtsInputs, string path)
    {
        data = new DataTable();
        using (StreamReader reader = new StreamReader(path))
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
        inExports = new bool[data.Rows.Count];
    }

    public List<DataRow> NotInExports()
    {
        List<DataRow> linesNotInExports = new List<DataRow>();
        for(int i = 0;i<data.Rows.Count;i++)
        {
            if (!inExports[i])
                linesNotInExports.Add(data.Rows[i]);
        }
        return linesNotInExports;
    }
}