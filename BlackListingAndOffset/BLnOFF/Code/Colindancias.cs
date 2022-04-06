﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using ZOT.resources;
using System.Xml;
using System.Windows.Controls;
using System.Linq;
using System.Data.OleDb;
using ZOT.resources.ZOTlib;
using System.Windows.Forms;

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

            double? dist = null;
            double? HOatem;  //estos valores pueden no estar definidos en el data set, asi que tienen que ser nullables
            double? HOsucc;
            double? HOsuccSR;
            int? interfaceX2 = null;
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

                        //Si es UNKNOWN SALGO
                        var dstName = exportRow.Field<string>(6);
                        if (dstName == "UNKNOWN")
                        {
                            return;
                        }


                        String[] aux1 = exportRow.Field<String>("srcName").Split('_');
                        String[] aux2 = exportRow.Field<String>("dstName").Split('_');
                        int site1 = 0;
                        int site2 = 0;



                        site1 = Convert.ToInt32(aux1[aux1.Length - 2]) / 100;
                        int.TryParse(aux2[aux2.Length - 2], out site2);
                        site2 = site2 / 100;


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
                        data.Rows.Add(aux);
                        break; //solo coincidira una vez
                    }
                }
                if (found == false)
                {
                    try
                    {
                        var dstName = exportRow.Field<string>(6);
                        if(dstName == "UNKNOWN")
                        {
                            return;
                        }





                        String[] aux1 = null;
                        String[] aux2 = null;

                    
                        aux1 = exportRow.Field<String>("srcName").Split('_');

                        aux2 = exportRow.Field<String>("dstName").Split('_');


                        try
                        {
                            int site1 = Convert.ToInt32(aux1[aux1.Length - 2]) / 100;
                            int site2 = Convert.ToInt32(aux2[aux2.Length - 2]) / 100;
                            dist = siteCoords.Distance(site1, site2);
                        }
                        catch (Exception)
                        {
                            //MessageBox.Show("Una celda no se reconoce: \n   src = " + exportRow.Field<String>("srcName") + "\n   dstName = " + exportRow.Field<String>("dstName"),  "Celda ignorada",  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            
                            dist = -1;
                        }


                        try
                        {


                        if (exportRow["x2LinkStatus"] != DBNull.Value)
                            interfaceX2 = (int)exportRow["x2LinkStatus"];
                        else
                            interfaceX2 = null;

                        }
                        catch (Exception e)
                        {
                            int a = 0;
                        }
                        aux = new object[17] { exportRow["Label"], exportRow["mrbtsId"], exportRow["lnCelId"], exportRow["srcName"], exportRow["ecgiAdjEnbId"], exportRow["ecgiLcrId"], exportRow["dstName"], dist, null, exportRow["cellIndOffNeigh"], null, null, null, exportRow["handoverAllowed"], null, interfaceX2, "No esta en el RSLTE31" };
                        data.Rows.Add(aux);

                       
                    }
                    catch (NullReferenceException)
                    {
                        Console.WriteLine("BlackListingAndOffset->Colindancias.cs: Alguno de los coponentes de exportRow es nullo, la linea se salta");
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
            catch (IndexOutOfRangeException)
            {
                int xadafsf = 0;
                return;
            }
            catch (Exception)
            {
                int dadfadfa = 0;
                return;
            }

        }

        /// <summary>
        /// (Threadsafe) Añade las lineas que solo están en la consulta 31 a la tabla de colindancias
        /// </summary>
        /// <param name="line"></param>
        public void CheckColinsNotInExports(DataRow line)
        {
            double? dist = null;
            double? HOatem;
            double? HOsucc;
            double? HOsuccSR;
            String[] aux1;
            try
            {
                aux1 = line.Field<String>("Source LNCEL name").Split('_');
                String[] aux2 = null;
                if (line.Field<String>("Target LNCEL name") != "UNKNOWN" && line.Field<String>("Target LNCEL name") != null)
                {
                    aux2 = line.Field<String>("Target LNCEL name").Split('_');
                    int site1 = Convert.ToInt32(aux1[aux1.Length - 2]) / 100;
                    int site2 = -1;
                    try
                    {
                        site2 = Convert.ToInt32(aux2[aux2.Length - 2]) / 100;

                        dist = siteCoords.Distance(site1, site2);
                    }
                    catch(Exception e)
                    {
                        return;
                    }
                }


                if (dist == null || dist > 0.01) //si la distancia es desconocida, se saca igual pero no aplicara ni a Blacklisting ni a offset
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


                int srcLnCellID = Convert.ToInt32(aux1[aux1.Length - 1]);

                
                int? trgLnCellID = null;
                if (aux2 != null)
                {
                    trgLnCellID = Convert.ToInt32(aux2[aux2.Length - 1]);

                    
                }
                double? HOerrSR = (100 - HOsucc) * HOatem / 100.0;
                if (HOerrSR != null)
                    HOerrSR = Math.Round((double)HOerrSR, 0);
                double? HOerrPrep = (100 - HOsuccSR) * HOatem / HOsuccSR;
                if (HOerrPrep != null)
                    HOerrPrep = Math.Round((double)HOerrPrep, 0);


                Object[] aux = new object[17] { "", "", srcLnCellID, line["Source LNCEL name"], line["Target LNBTS ID"], trgLnCellID, line["Target LNCEL name"], dist, HOsucc, 15, HOatem, null, HOerrSR, HOsuccSR, HOerrPrep, null, "No esta presente en el export" };
                lock (data) //hay que porteger la escritura de la lista para hacer multithreading
                {
                    if(dist == null)
                    {
                        aux[7] = -1;
                    }
                    if(trgLnCellID == null)
                    {
                        aux[5] = line["Target LCR ID"];
                    }

                   // var a = data[4];

                    data.Rows.Add(aux);
                }



            }
            catch(IndexOutOfRangeException ioer)
            {
                Console.WriteLine("Error en colindancias: " + ioer.Message);
                //Errores en los ficheros de entrada suelen acabar dando un error de este tipo al intentar parear tipos de dato inesperados
                //simplemente se ignora la linea
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en colindancias: " + e.Message);
                //Errores en los ficheros de entrada suelen acabar dando un error de este tipo al intentar parear tipos de dato inesperados
                //simplemente se ignora la linea
            }
        }

        /// <summary>
        /// Genera los ENBID para las lineas que no estan en los exports
        /// </summary>
        public void AddENBID() 
        {
            DataView auxdv = data.DefaultView;
            auxdv.Sort = "[Name SOURCE] desc, [ENBID SOURCE] desc";
            data = auxdv.ToTable();

            int i = 0;
            try
            {
                string enbid = (string)data.Rows[0]["ENBID SOURCE"];

                while (true)
                {
                    while (i < data.Rows.Count && ((string)data.Rows[i]["ENBID SOURCE"] == enbid || (string)data.Rows[i]["ENBID SOURCE"] == ""))
                    {
                        data.Rows[i]["ENBID SOURCE"] = enbid;
                        i++;
                    }
                    if (i >= data.Rows.Count) break;
                    enbid = (string)data.Rows[i]["ENBID SOURCE"];
                }
            }
            catch (IndexOutOfRangeException oor)
            {
                Console.WriteLine("No hay nodos que cumplan las condiciones");
            }
        }



        /// <summary>
        ///Rellena los label de los UNKNOWN
        ///Para ellos:
        ///     - Se consulta en la tabla LNREL (si tiene un SRAN solo uno y si tiene dos los dos)
        ///         - lnBtsId
        ///         - LnCellId
        ///         - ecgiAdjEnbl
        ///         - ecgiLcrId
        ///     
        /// </summary>
        public void rellenarLabelUnknown(String rutaSRAN1, String rutaSRAN2, bool estaDividido)
        {
            DataView auxdv = data.DefaultView;
            data = auxdv.ToTable();
            


            //si no esta dividio solo miro en un SRAN
            if (!estaDividido)
            {
                string directionSRAN = string.Format("provider=Microsoft.ACE.OLEDB.12.0;Data Source= {0}", rutaSRAN1);


                OleDbConnection connectionSRANBusqueda1 = new OleDbConnection(directionSRAN);
                using (connectionSRANBusqueda1)
                {
                    //contador para sacar la row en la tabla
                    int contadorRow2 = 0;
                    foreach (DataRow row in data.Rows)
                    {
                        String isUnknown = row[6].ToString();
                        if (isUnknown == "UNKNOWN")
                        {
                            var mrbtsId = row[1].ToString();
                            var lnCelId = row[2].ToString();
                            var ecgiAdjEnbId = row[4].ToString();
                            var ecgiLcrId = row[5].ToString();


                            if (mrbtsId != "" && lnCelId != "" && ecgiAdjEnbId != "" && ecgiLcrId != "")
                            {
                                //Lo busco en el primer export
                                string SQLquerryBusqueda2 = "SELECT A_LTE_MRBTS_LNBTS_LNCEL_LNREL.lnRelId FROM A_LTE_MRBTS_LNBTS_LNCEL_LNREL WHERE(((A_LTE_MRBTS_LNBTS_LNCEL_LNREL.lnBtsId) = "+ mrbtsId +") AND((A_LTE_MRBTS_LNBTS_LNCEL_LNREL.lnCelId) = " + lnCelId + ") AND((A_LTE_MRBTS_LNBTS_LNCEL_LNREL.ecgiAdjEnbId) =  " + ecgiAdjEnbId + ") AND((A_LTE_MRBTS_LNBTS_LNCEL_LNREL.ecgiLcrId) =  " + ecgiLcrId + "));";
                                    
                                    /**
                                 * SELECT A_LTE_MRBTS_LNBTS_LNCEL_LNREL.mrbtsId, A_LTE_MRBTS_LNBTS_LNCEL_LNREL.ecgiAdjEnbId, A_LTE_MRBTS_LNBTS_LNCEL_LNREL.ecgiLcrId, A_LTE_MRBTS_LNBTS_LNCEL_LNREL.lnRelId
FROM A_LTE_MRBTS_LNBTS_LNCEL_LNREL
WHERE (((A_LTE_MRBTS_LNBTS_LNCEL_LNREL.mrbtsId)="370212"));
                                */



                                OleDbCommand commandSRANBusqueda1 = new OleDbCommand(SQLquerryBusqueda2, connectionSRANBusqueda1);
                                connectionSRANBusqueda1.Open();
                                using (OleDbDataReader SRANreaderBusqueda1 = commandSRANBusqueda1.ExecuteReader())
                                {
                                    var nuevodtName = "";
                                    while (SRANreaderBusqueda1.Read())
                                    {
                                        nuevodtName = SRANreaderBusqueda1[0].ToString();
                                    }
                                    SRANreaderBusqueda1.Close();
                                    if (nuevodtName != null && nuevodtName != "")
                                    {
                                        data.Rows[contadorRow2].SetField("Label", "LNREL-" +nuevodtName);
                                    }
                                }
                                commandSRANBusqueda1.Connection.Close();
                            }
                        }
                        contadorRow2++;
                    }

                    connectionSRANBusqueda1.Close();

                }
            }

            //Si esta dividio tengo que hacer lo mismo en los dos
            else
            {
                string directionSRAN = string.Format("provider=Microsoft.ACE.OLEDB.12.0;Data Source= {0}", rutaSRAN1);
                string directionSRAN2 = string.Format("provider=Microsoft.ACE.OLEDB.12.0;Data Source= {0}", rutaSRAN2);



                OleDbConnection connectionSRANBusqueda1 = new OleDbConnection(directionSRAN);
                using (connectionSRANBusqueda1)
                {
                    //contador para sacar la row en la tabla
                    int contadorRow2 = 0;
                    foreach (DataRow row in data.Rows)
                    {
                        String isUnknown = row[6].ToString();
                        String isLabel = row[0].ToString();
                        if (isLabel == "")
                        {
                            Console.WriteLine("Label Empty");
                        }

                        if (isUnknown == "UNKNOWN" )
                        {
                            var mrbtsId = row[1].ToString();
                            var lnCelId = row[2].ToString();
                            var ecgiAdjEnbId = row[4].ToString();
                            var ecgiLcrId = row[5].ToString();

                            if (mrbtsId != "" && lnCelId != "" && ecgiAdjEnbId != "" && ecgiLcrId != "")
                            {
                                //Lo busco en el primer export
                                string SQLquerryBusqueda2 = "SELECT A_LTE_MRBTS_LNBTS_LNCEL_LNREL.lnRelId FROM A_LTE_MRBTS_LNBTS_LNCEL_LNREL WHERE(((A_LTE_MRBTS_LNBTS_LNCEL_LNREL.lnBtsId) = " + mrbtsId + ") AND((A_LTE_MRBTS_LNBTS_LNCEL_LNREL.lnCelId) = " + lnCelId + ") AND((A_LTE_MRBTS_LNBTS_LNCEL_LNREL.ecgiAdjEnbId) =  " + ecgiAdjEnbId + ") AND((A_LTE_MRBTS_LNBTS_LNCEL_LNREL.ecgiLcrId) =  " + ecgiLcrId + "));";

                                /**
                             * SELECT A_LTE_MRBTS_LNBTS_LNCEL_LNREL.mrbtsId, A_LTE_MRBTS_LNBTS_LNCEL_LNREL.ecgiAdjEnbId, A_LTE_MRBTS_LNBTS_LNCEL_LNREL.ecgiLcrId, A_LTE_MRBTS_LNBTS_LNCEL_LNREL.lnRelId
FROM A_LTE_MRBTS_LNBTS_LNCEL_LNREL
WHERE (((A_LTE_MRBTS_LNBTS_LNCEL_LNREL.mrbtsId)="370212"));
                            */



                                OleDbCommand commandSRANBusqueda1 = new OleDbCommand(SQLquerryBusqueda2, connectionSRANBusqueda1);
                                connectionSRANBusqueda1.Open();
                                using (OleDbDataReader SRANreaderBusqueda1 = commandSRANBusqueda1.ExecuteReader())
                                {
                                    var nuevodtName = "";
                                    while (SRANreaderBusqueda1.Read())
                                    {
                                        nuevodtName = SRANreaderBusqueda1[0].ToString();
                                    }
                                    SRANreaderBusqueda1.Close();
                                    if (nuevodtName != null && nuevodtName != "")
                                    {
                                        data.Rows[contadorRow2].SetField("Label", "LNREL-" + nuevodtName);
                                    }
                                }
                                commandSRANBusqueda1.Connection.Close();
                            }
                        }
                        contadorRow2++;
                    }

                    connectionSRANBusqueda1.Close();


                }



                //En SRAN2
                OleDbConnection connectionSRANBusqueda2 = new OleDbConnection(directionSRAN2);
                using (connectionSRANBusqueda2)
                {
                    //contador para sacar la row en la tabla
                    int contadorRow2 = 0;
                    foreach (DataRow row in data.Rows)
                    {
                        String isUnknown = row[6].ToString();
                        if (isUnknown == "UNKNOWN")
                        {
                            var mrbtsId = row[1].ToString();
                            var lnCelId = row[2].ToString();
                            var ecgiAdjEnbId = row[4].ToString();
                            var ecgiLcrId = row[5].ToString();

                            if (mrbtsId != "" && lnCelId != "" && ecgiAdjEnbId != "" && ecgiLcrId != "")
                            {
                                //Lo busco en el primer export
                                string SQLquerryBusqueda2 = "SELECT A_LTE_MRBTS_LNBTS_LNCEL_LNREL.lnRelId FROM A_LTE_MRBTS_LNBTS_LNCEL_LNREL WHERE(((A_LTE_MRBTS_LNBTS_LNCEL_LNREL.lnBtsId) = " + mrbtsId + ") AND((A_LTE_MRBTS_LNBTS_LNCEL_LNREL.lnCelId) = " + lnCelId + ") AND((A_LTE_MRBTS_LNBTS_LNCEL_LNREL.ecgiAdjEnbId) =  " + ecgiAdjEnbId + ") AND((A_LTE_MRBTS_LNBTS_LNCEL_LNREL.ecgiLcrId) =  " + ecgiLcrId + "));";




                                OleDbCommand commandSRANBusqueda2 = new OleDbCommand(SQLquerryBusqueda2, connectionSRANBusqueda2);
                                connectionSRANBusqueda2.Open();
                                using (OleDbDataReader SRANreaderBusqueda2 = commandSRANBusqueda2.ExecuteReader())
                                {
                                    var nuevodtName = "";
                                    while (SRANreaderBusqueda2.Read())
                                    {
                                        nuevodtName = SRANreaderBusqueda2[0].ToString();
                                    }
                                    SRANreaderBusqueda2.Close();
                                    if (nuevodtName != null && nuevodtName != "")
                                    {
                                        data.Rows[contadorRow2].SetField("Label", "LNREL-" + nuevodtName);
                                    }
                                }
                                commandSRANBusqueda2.Connection.Close();
                            }
                        }
                        contadorRow2++;
                    }

                    connectionSRANBusqueda2.Close();

                }

            }


                    //si es unknown hago la consulta a la bd y relleno

                    //SELECT A_LTE_MRBTS_LNBTS_LNCEL_LNREL.mrbtsId, A_LTE_MRBTS_LNBTS_LNCEL_LNREL.lnCelId, A_LTE_MRBTS_LNBTS_LNCEL_LNREL.ecgiAdjEnbId, A_LTE_MRBTS_LNBTS_LNCEL_LNREL.ecgiLcrId, A_LTE_MRBTS_LNBTS_LNCEL_LNREL.lnRelId FROM A_LTE_MRBTS_LNBTS_LNCEL_LNREL;

                }





        /// <summary>
        /// Obtiene una cadena con todos los emplazamientos que no se pudieron encontrar tras la ejecucion de las colindancias
        /// </summary>
        /// <returns></returns>
        public string GetSiteCoordErr()
        {
            string output ="";
            if (siteCoords.errorLog.Count > 0)
            {
                foreach(int site in siteCoords.errorLog.Distinct())
                {
                    output += site + "\n";
                }
            }
                
            return output;
        }

    }

}
