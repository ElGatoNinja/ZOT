using System.IO;
using System.Collections.Generic;
using System;
using System.Data;
using BlackListingAndOffset.resources;

public class RSLTE31 : GenericTable
{
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
    }

    public DataRow[] NotInExports(Exports export)
    {
        return this.NotIntersectingWithThis(export, "Source LNCEL name", "srcName");
    }
}