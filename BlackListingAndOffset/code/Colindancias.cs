using System;
using System.Collections.Generic;
using System.Threading;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using BlackListingAndOffset.resources;
using System.Xml;

public class Colindancias : GenericTable
{
    private SiteCoords siteCoords;
    private readonly string[] colNames = { "Label", "ENBID SOURCE", "LnCell SOURCE", "Name SOURCE","ENBID TARGET","LnCell TARGET","Name TARGET","Distance","HO Success(%)","Offset","HO Attempts","Blacklist","HO errores SR","HO Succes Prep(%)","HO errores Prep","InterfazX2","Comentarios"};
    //private readonly System.Type[] size = {typeof(string), typeof(string), typeof(long), typeof(string), typeof(string), typeof(long), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string)}
    public Colindancias()
    {
        siteCoords = new SiteCoords();
        data = new DataTable();
        foreach(string title in colNames)
        {
            data.Columns.Add(title);
        }
        data.Columns["Distance"].DataType = typeof(double); //Si se deja como string la ordenacion es por orden alfabético en vez de numerico
    }
    public void CheckColin(DataRow exportRow,RSLTE31 r31)
    {
        bool found = false;

        double dist = -1f;
        double? HOatem;
        double? HOsucc;
        double? HOsuccSR;
        int interfaceX2;
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
                        ZOTUtiles.Conversion.ToDouble((string)match["Inter eNB neighbor HO: Att"], out HOatem);
                        ZOTUtiles.Conversion.ToDouble((string)match["Inter eNB neighbor HO: SR"], out HOsucc);
                        ZOTUtiles.Conversion.ToDouble((string)match["Inter eNB neighbor HO: Prep SR"], out HOsuccSR);
                        interfaceX2 = (int)exportRow["x2LinkStatus"];
                    }
                    else //si la distancia es 0, el site coincide y por tanto los KPI relevantes son intra en vez de inter.
                    {
                        ZOTUtiles.Conversion.ToDouble((string)match["Intra eNB neighbor HO: Att"], out HOatem);
                        ZOTUtiles.Conversion.ToDouble((string)match["Intra eNB neighbor HO: SR"], out HOsucc);
                        ZOTUtiles.Conversion.ToDouble((string)match["Intra eNB neighbor HO: Prep SR"], out HOsuccSR);
                        interfaceX2 = 1;
                    }

                    aux = new object[17] { exportRow["Label"], exportRow["mrbtsId"], exportRow["lnCelId"], exportRow["srcName"], exportRow["ecgiAdjEnbId"], exportRow["ecgiLcrId"], exportRow["dstName"], Math.Round(dist,2), HOsucc, exportRow["cellIndOffNeigh"], HOatem,exportRow["handoverAllowed"],Math.Round((100-HOsucc)*HOatem/100.0,2),HOsuccSR,Math.Round((100-HOsuccSR)*HOatem/HOsuccSR,2), interfaceX2, "" };
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

                interfaceX2 = (int)exportRow["x2LinkStatus"];
                aux = new object[17] { exportRow["Label"], exportRow["mrbtsId"], exportRow["lnCelId"], exportRow["srcName"], exportRow["ecgiAdjEnbId"], exportRow["ecgiLcrId"], exportRow["dstName"], Math.Round(dist,2), "", exportRow["cellIndOffNeigh"], "","BlackList?","" ,exportRow["handoverAllowed"],"", interfaceX2, "No esta en el RSLTE31" };
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

    public void CheckColinsNotInExports(DataRow line)
    {
        double dist;
        double HOatem;
        double HOsucc;
        double HOsuccSR;

        try
        {
            String[] aux1 = line.Field<String>("Source LNCEL name").Split('_');
            String[] aux2 = line.Field<String>("Target LNCEL name").Split('_');
            int site1 = Convert.ToInt32(aux1[aux1.Length - 2]) / 100;
            int site2 = Convert.ToInt32(aux2[aux2.Length - 2]) / 100;
            dist = siteCoords.Distance(site1, site2);
            if (dist > 0.01)
            {
                Double.TryParse((string)line["Inter eNB neighbor HO: Att"], out HOatem);
                Double.TryParse((string)line["Inter eNB neighbor HO: SR"], out HOsucc);
                Double.TryParse((string)line["Inter eNB neighbor HO: Prep SR"], out HOsuccSR);
            }
            else //si la distancia es 0, el site coincide y por tanto los KPI relevantes son intra en vez de inter.
            {
                Double.TryParse((string)line["Intra eNB neighbor HO: Att"], out HOatem);
                Double.TryParse((string)line["Intra eNB neighbor HO: SR"], out HOsucc);
                Double.TryParse((string)line["Intra eNB neighbor HO: Prep SR"], out HOsuccSR);
            }
            aux1 = line.Field<String>("Source LNCEL name").Split('_');
            aux2 = line.Field<String>("Target LNCEL name").Split('_');
            int srcLnCellID = Convert.ToInt32(aux1[aux1.Length - 1]);
            int trgLnCellID = Convert.ToInt32(aux2[aux2.Length - 1]);

            Object[] aux = new object[17] { "", "?", srcLnCellID, line["Source LNCEL name"], line["Target LNBTS ID"], trgLnCellID, line["Target LNCEL name"], Math.Round(dist,2), HOsucc, 15, HOatem,"Blacklist?", Math.Round((100 - HOsucc) * HOatem / 100.0, 2), HOsuccSR, Math.Round((100 - HOsuccSR) * HOatem / HOsuccSR, 2), "?","No esta presente en el export" };
            lock (data) //hay que porteger la escritura de la lista para hacer multithreading
            {
                data.Rows.Add(aux);
            }
        }
        catch(ArgumentNullException ane)
        {
            Console.WriteLine("Error en la busqueda de corrdenadas just in R31");
        }
    }



}
