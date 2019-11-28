using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using BlackListingAndOffset.resources;

public class Colindancias
{
    public List<object[]> colinLines;
    private SiteCoords siteCoords;
    private readonly string[] colNames = { "label", "eutraOrigen", "idOrigen", "nombreOrigen","eutraDestino","idDestiono","nombreDestino","distancia","hoCumplido","offset","hoIntentos","Razon","interfazX2"};

    public Colindancias()
    {
        siteCoords = new SiteCoords();
        colinLines = new List<object[]>();
        colinLines.Add(colNames);
    }
    public void CheckColindance(DataRow exportRow,RSLTE31 r31)
    {
        double dist = -1f;
        double HOatem = -1f;
        double HOsucc = -1f;
        double HOsuccSR = -1f;

        int interfaceX2;
            
        try
        { 
            DataRow[] matchArray = r31.data.Select("[Source LNCEL name] = '" + exportRow["srcName"]+ "'" );

            foreach (DataRow match in matchArray)
            {
                if (match.Field<string>("Target LNCEL name") == exportRow.Field<string>("dstName"))
                {
                    //Esto es una triquiñuela para sacar el codigo de emplazamiento a partir de los cell names (sacarlos en una columna de los exports puede ser beneficioso)
                    String[] aux1 = exportRow.Field<String>("srcName").Split('_');
                    String[] aux2 = exportRow.Field<String>("dstName").Split('_');
                    int site1 = Convert.ToInt32(aux1[aux1.Length - 2])/100;
                    int site2 = Convert.ToInt32(aux2[aux2.Length - 2])/100;

                    dist = siteCoords.Distance(site1,site2);
                    if (dist > 0.01)
                    {
                        Double.TryParse((string)match["Inter eNB neighbor HO: Att"],out HOatem);
                        Double.TryParse((string)match["Inter eNB neighbor HO: SR"],out HOsucc);
                        Double.TryParse((string)match["Inter eNB neighbor HO: Prep SR"],out HOsuccSR);
                        interfaceX2 = (int)exportRow["x2LinkStatus"];
                    }
                    else //si la distancia es 0, el site coincide y por tanto los KPI relevantes son intra en vez de inter.
                    {
                        Double.TryParse((string)match["Intra eNB neighbor HO: Att"], out HOatem);
                        Double.TryParse((string)match["Intra eNB neighbor HO: SR"], out HOsucc);
                        Double.TryParse((string)match["Intra eNB neighbor HO: Prep SR"], out HOsuccSR);
                        interfaceX2 = 1;
                    }

                    object[] aux = new object[14]{ exportRow["Label"],exportRow["mrbtsId"],exportRow["lnCelId"], exportRow["srcName"],exportRow["ecgiAdjEnbId"],exportRow["ecgiLcrId"],exportRow["dstName"],dist,HOsucc,exportRow["cellIndOffNeigh"], HOatem,exportRow["handoverAllowed"],HOsuccSR,interfaceX2};
                    lock (colinLines) //hay que porteger la escritura de la lista para hacer multithreading
                    {
                        colinLines.Add(aux);
                    }
                }
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
}

