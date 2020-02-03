using System;
using System.Collections.Generic;
using System.Threading;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using ZOT.resources;
using System.Xml;
using System.Windows.Controls;

namespace ZOT.BLnOFF.Code
{
    public class Colindancias 
    {
        public DataTable data;
        private SiteCoords siteCoords;
        private readonly string[] colNames = { "Label", "ENBID SOURCE", "LnCell SOURCE", "Name SOURCE", "ENBID TARGET", "LnCell TARGET", "Name TARGET", "Distance", "HO Success(%)", "Offset", "HO Attempts", "Blacklist", "HO errores SR", "HO Succes Prep(%)", "HO errores Prep", "InterfazX2", "Comentarios" };
        private readonly System.Type[] colType = { typeof(string), typeof(string), typeof(int), typeof(string), typeof(string), typeof(int), typeof(string), typeof(double), typeof(double), typeof(int), typeof(double), typeof(int),typeof(double), typeof(double), typeof(double), typeof(int), typeof(string) };
        
        public Colindancias()
        {
            siteCoords = new SiteCoords();

            data = new DataTable();
            for(int i = 0;i < colNames.Length;i++)
            {
                data.Columns.Add(colNames[i],colType[i]);
            }
        }
        /// <summary>
        /// (Threadsafe) Añade a la tabla de colindancias las lineas que se han extraido de los exports
        /// </summary>
        /// <param name="exportRow"></param>
        /// <param name="r31"></param>
        public void CheckColin(DataRow exportRow, RSLTE31 r31)
        {
            bool found = false;

            double? dist;
            double? HOatem;  //estos valores pueden no estar definidos en el data set, asi que tienen que ser nullables
            double? HOsucc;
            double? HOsuccSR;
            int? interfaceX2;
            Object[] aux = new object[14];

            try
            {
                DataRow[] matchArray = r31.data.Select("[Source LNCEL name] = '" + exportRow["srcName"] + "'");

                foreach (DataRow match in matchArray)
                {
                    if (match.Field<string>("Target LNCEL name") == exportRow.Field<string>("dstName"))
                    {
                        found = true;
                        r31.inExports[r31.data.Rows.IndexOf(match)] = true;  //como esta linea existe en los exports se levanta el flag correspondiente

                        //Esto es una triquiñuela para sacar el codigo de emplazamiento a partir de los cell names (sacarlos en una columna de los exports podría ser beneficioso)
                        String[] aux1 = exportRow.Field<String>("srcName").Split('_');
                        String[] aux2 = exportRow.Field<String>("dstName").Split('_');
                        int site1 = Convert.ToInt32(aux1[aux1.Length - 2]) / 100;
                        int site2 = Convert.ToInt32(aux2[aux2.Length - 2]) / 100;

                        dist = siteCoords.Distance(site1, site2);
                        if (dist > 0.01)
                        {
                            resources.ZOTlib.Conversion.ToDouble((string)match["Inter eNB neighbor HO: Att"], out HOatem);
                            resources.ZOTlib.Conversion.ToDouble((string)match["Inter eNB neighbor HO: SR"], out HOsucc);
                            resources.ZOTlib.Conversion.ToDouble((string)match["Inter eNB neighbor HO: Prep SR"], out HOsuccSR);

                            if (exportRow["x2LinkStatus"] != DBNull.Value)
                                interfaceX2 = (int)exportRow["x2LinkStatus"];
                            else
                                interfaceX2 = null;
                        }
                        else //si la distancia es 0, el site coincide y por tanto los KPI relevantes son intra en vez de inter.
                        {
                            resources.ZOTlib.Conversion.ToDouble((string)match["Intra eNB neighbor HO: Att"], out HOatem);
                            resources.ZOTlib.Conversion.ToDouble((string)match["Intra eNB neighbor HO: SR"], out HOsucc);
                            resources.ZOTlib.Conversion.ToDouble((string)match["Intra eNB neighbor HO: Prep SR"], out HOsuccSR);
                            interfaceX2 = 1;
                        }
                        double? HOerrSR = (100 - HOsucc) * HOatem / 100.0;
                        if (HOerrSR != null)
                            HOerrSR = Math.Round((double)HOerrSR, 0);
                        double? HOerrPrep = (100 - HOsuccSR) * HOatem / HOsuccSR;
                        if (HOerrPrep != null)
                            HOerrPrep = Math.Round((double)HOerrPrep, 0);

                        aux = new object[17] { exportRow["Label"], exportRow["mrbtsId"], exportRow["lnCelId"], exportRow["srcName"], exportRow["ecgiAdjEnbId"], exportRow["ecgiLcrId"], exportRow["dstName"], dist, HOsucc, exportRow["cellIndOffNeigh"], HOatem, exportRow["handoverAllowed"], HOerrSR, HOsuccSR, HOerrPrep, interfaceX2, "" };
                        break; //solo coincidira una vez
                    }
                }
                if (found == false)
                {
                    String[] aux1 = exportRow.Field<String>("srcName").Split('_');
                    String[] aux2 = exportRow.Field<String>("dstName").Split('_');
                    int site1 = Convert.ToInt32(aux1[aux1.Length - 2]) / 100;
                    int site2 = Convert.ToInt32(aux2[aux2.Length - 2]) / 100;
                    dist = siteCoords.Distance(site1, site2);

                    if (exportRow["x2LinkStatus"] != DBNull.Value)
                        interfaceX2 = (int)exportRow["x2LinkStatus"];
                    else
                        interfaceX2 = null;

                    aux = new object[17] { exportRow["Label"], exportRow["mrbtsId"], exportRow["lnCelId"], exportRow["srcName"], exportRow["ecgiAdjEnbId"], exportRow["ecgiLcrId"], exportRow["dstName"],dist, null, exportRow["cellIndOffNeigh"], null, null, null, exportRow["handoverAllowed"], null, interfaceX2, "No esta en el RSLTE31" };
                }
                lock (data) //hay que porteger la escritura de la lista para hacer multithreading
                {
                    data.Rows.Add(aux);
                }
            }
            catch (EvaluateException ee)
            {
                //si no se encuentra en la consulta RLTE31
                Console.WriteLine(ee.Message);
            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine("Hay que actualizar la hoja de coordenadas, la ejecucion continua normalmente pero quedan colindancias sin comprobar");
            }

        }

        /// <summary>
        /// (Threadsafe) Añade las lineas que solo están en la consulta 31 a la tabla de colindancias
        /// </summary>
        /// <param name="line"></param>
        public void CheckColinsNotInExports(DataRow line)
        {
            double? dist;
            double? HOatem;
            double? HOsucc;
            double? HOsuccSR;
            String[] aux1;
            try
            {
                aux1 = line.Field<String>("Source LNCEL name").Split('_');
                String[] aux2 = line.Field<String>("Target LNCEL name").Split('_');
                int site1 = Convert.ToInt32(aux1[aux1.Length - 2]) / 100;
                int site2 = Convert.ToInt32(aux2[aux2.Length - 2]) / 100;
                dist = siteCoords.Distance(site1, site2);
                if (dist > 0.01)
                {
                    resources.ZOTlib.Conversion.ToDouble((string)line["Inter eNB neighbor HO: Att"], out HOatem);
                    resources.ZOTlib.Conversion.ToDouble((string)line["Inter eNB neighbor HO: SR"], out HOsucc);
                    resources.ZOTlib.Conversion.ToDouble((string)line["Inter eNB neighbor HO: Prep SR"], out HOsuccSR);
                }
                else //si la distancia es 0, el site coincide y por tanto los KPI relevantes son intra en vez de inter.
                {
                    resources.ZOTlib.Conversion.ToDouble((string)line["Intra eNB neighbor HO: Att"], out HOatem);
                    resources.ZOTlib.Conversion.ToDouble((string)line["Intra eNB neighbor HO: SR"], out HOsucc);
                    resources.ZOTlib.Conversion.ToDouble((string)line["Intra eNB neighbor HO: Prep SR"], out HOsuccSR);
                }
                aux1 = line.Field<String>("Source LNCEL name").Split('_');
                aux2 = line.Field<String>("Target LNCEL name").Split('_');
                int srcLnCellID = Convert.ToInt32(aux1[aux1.Length - 1]);
                int trgLnCellID = Convert.ToInt32(aux2[aux2.Length - 1]);
                double? HOerrSR = (100 - HOsucc) * HOatem / 100.0;
                if (HOerrSR != null)
                    HOerrSR = Math.Round((double)HOerrSR, 0);
                double? HOerrPrep = (100 - HOsuccSR) * HOatem / HOsuccSR;
                if (HOerrPrep != null)
                    HOerrPrep = Math.Round((double)HOerrPrep, 0);


                Object[] aux = new object[17] { "", "", srcLnCellID, line["Source LNCEL name"], line["Target LNBTS ID"], trgLnCellID, line["Target LNCEL name"], dist, HOsucc, 15, HOatem, null, HOerrSR, HOsuccSR, HOerrPrep, null, "No esta presente en el export" };
                lock (data) //hay que porteger la escritura de la lista para hacer multithreading
                {
                    data.Rows.Add(aux);
                }
            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine("Error en la busqueda de corrdenadas en R31");
            }
            catch(IndexOutOfRangeException ioer)
            {
                dist = null;
                resources.ZOTlib.Conversion.ToDouble((string)line["Inter eNB neighbor HO: Att"], out HOatem);
                resources.ZOTlib.Conversion.ToDouble((string)line["Inter eNB neighbor HO: SR"], out HOsucc);
                resources.ZOTlib.Conversion.ToDouble((string)line["Inter eNB neighbor HO: Prep SR"], out HOsuccSR);
            }
        }

        /// <summary>
        /// Genera los ENBID para las lineas que no estan en los exports
        /// </summary>
        public void AddENBID() 
        {
            DataView auxdv = data.DefaultView;
            auxdv.Sort = "[ENBID SOURCE] desc, [Name SOURCE] desc";
            data = auxdv.ToTable();
            
            int i = 0;
            string enbid;
            do{ // la primera linea podria ser una de las que no tienen enbid
                enbid = (string)data.Rows[i]["ENBID Source"];
                i++;
            } while (enbid == "" && i < data.Rows.Count);
            i = 0;
            while (true)
            {
                while ( i < data.Rows.Count && ((string)data.Rows[i]["ENBID Source"] == enbid || (string)data.Rows[i]["ENBID Source"] == ""))
                {
                    data.Rows[i]["ENBID Source"] = enbid;
                    i++;
                }
                if (i >= data.Rows.Count) break; //No soy muy fan de usar breaks en vez de condiciones en el while, pero aquí parece necesario
                enbid = (string)data.Rows[i]["ENBID Source"];
            }
        }

        /// <summary>
        /// Obtiene una cadena con todos los emplazamientos que no se pudieron encontrar tras la ejecucion de las colindancias
        /// </summary>
        /// <returns></returns>
        public string GetSiteCoordErr()
        {
            string output ="";
            if (siteCoords.errorLog != "")
                output = siteCoords.errorLog + "\nActualiza el fichero siteCords.csv en la carpeta Data/";
            return output;
        }

    }

}
