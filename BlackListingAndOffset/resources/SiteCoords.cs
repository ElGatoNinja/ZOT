using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using ZOT.resources.ZOTlib;

namespace ZOT.resources
{
    class SiteCoords
    {
        public DataTable data;
        public List<int> errorLog = new List<int>();
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
                WPFForms.ShowError("No se encuentra el fichero de coordenadas","Deberia estar en data/ dentro del directorio de la aplicacion");
            }
        }


        public double? Distance(int site1, int site2)
        {
            DataRow[] site1data = data.Select("SiteID = " + site1);
            DataRow[] site2data = data.Select("SiteID = " + site2 );
            
            if (site1data.Length == 0)
            {
                errorLog.Add(site1);
                return null;
            }
            else if(site2data.Length == 0)
            {
                errorLog.Add(site2);
                return null;
            }

            //distancia por su definicion geometrica
            return Math.Round(Math.Sqrt((Math.Pow((long)site1data[0]["Longitude"] - (long)site2data[0]["Longitude"], 2) + Math.Pow((long)site1data[0]["Latitude"] - (long)site2data[0]["Latitude"], 2)))/1000.0,2);
        }
    }
}
