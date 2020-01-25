using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using WinSCP;


namespace ZOT.resources
{
    class SiteCoords
    {
        public DataTable data;
        private string path = Path.Combine(Environment.CurrentDirectory, @"Data\", "SiteCoord.csv");
        public SiteCoords()
        {
            data = new DataTable();
            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    string[] line = reader.ReadLine().Split(';');
                    foreach (string title in line)
                    {
                        data.Columns.Add(title, typeof(long));
                    }
                    while (!reader.EndOfStream)
                    {
                        object[] aux = (object[])reader.ReadLine().Split(';');
                        data.Rows.Add(aux);
                    }
                }
            }
            catch(FileNotFoundException)
            {
                ZOTlib.ShowError("No se encuentra sitecoords");
            }
        }


        public double Distance(int site1, int site2)
        {
            DataRow[] site1data = data.Select("SiteID = " + site1);
            DataRow[] site2data = data.Select("SiteID = " + site2 );
            
            if (site1data.Length == 0 || site2data.Length == 0)
            {
                throw new Exception("No se encuentra el emplazamiento: " + site1 + "\nSe debe actualizar la hoja de cordenadas de la carpeta Data\\");
            }

            //distancia por su definicion geometrica
            return Math.Sqrt((Math.Pow((long)site1data[0]["Longitude"] - (long)site2data[0]["Longitude"], 2) + Math.Pow((long)site1data[0]["Latitude"] - (long)site2data[0]["Latitude"], 2)))/1000.0;
        }
    }
}
