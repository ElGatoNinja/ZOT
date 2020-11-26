using System;
using System.Data;
using System.Data.OleDb;
using System.IO;
using ZOT.resources;
using ZOT.resources.ZOTlib;

namespace ZOT.BLnOFF.Code
{
    public class Exports
    {
        public DataTable data;
        public Exports(string[] cellNames, string pathSRAN, string pathFL18, string pathSRAN2, bool srandividido) : base()
        {

            string directionSRAN = string.Format("provider=Microsoft.ACE.OLEDB.12.0;Data Source= {0}", pathSRAN);
            string directionSRAN2 = string.Format("provider=Microsoft.ACE.OLEDB.12.0;Data Source= {0}", pathSRAN2);
            string directionFL18 = string.Format("provider=Microsoft.ACE.OLEDB.12.0;Data Source= {0}", pathFL18);




            int i = 0;
            
            while (i < cellNames.Length-1)
            {

                //no se pueden hacer busquedas sql con mas de 99 OR y AND, así que el siguiente bucle hace consultas "paginadas"
                //hace una busqueda para cada 98 emplazamientos, 98 * (1OR) = 98 OR
                string SQLconditions = "A_LTE_MRBTS_LNBTS_LNCEL.cellName = \"" + cellNames[i] + "\"";
                for (int j = 0; j < 98 && i < cellNames.Length; j++)
                {
                    SQLconditions += " OR A_LTE_MRBTS_LNBTS_LNCEL.cellName = \"" + cellNames[i] + "\"";
                    i++;
                }

                string SQLquerry = "SELECT \"LNREL-\" & A_LTE_MRBTS_LNBTS_LNCEL_LNREL.lnRelId AS Label, "
                                + "A_LTE_MRBTS_LNBTS_LNCEL_LNREL.mrbtsId, A_LTE_MRBTS_LNBTS_LNCEL_LNREL.lnCelId, "
                                + "A_LTE_MRBTS_LNBTS_LNCEL.cellName AS srcName, "
                                + "A_LTE_MRBTS_LNBTS_LNCEL_LNREL.ecgiAdjEnbId, "
                                + "A_LTE_MRBTS_LNBTS_LNCEL_LNREL.ecgiLcrId, "
                                + "A_LTE_MRBTS_LNBTS_LNCEL_1.cellName AS dstName, "
                                + "A_LTE_MRBTS_LNBTS_LNCEL_LNREL.handoverAllowed, "
                                + "A_LTE_MRBTS_LNBTS_LNCEL_LNREL.cellIndOffNeigh, "
                                + "A_LTE_MRBTS_LNBTS_LNADJ.x2LinkStatus "
                                + "FROM((A_LTE_MRBTS_LNBTS_LNCEL_LNREL INNER JOIN A_LTE_MRBTS_LNBTS_LNCEL "
                                + "ON(A_LTE_MRBTS_LNBTS_LNCEL_LNREL.mrbtsId +0 = A_LTE_MRBTS_LNBTS_LNCEL.mrbtsId + 0 ) "
                                + "AND(A_LTE_MRBTS_LNBTS_LNCEL_LNREL.lnBtsId = A_LTE_MRBTS_LNBTS_LNCEL.lnBtsId) "
                                + "AND(A_LTE_MRBTS_LNBTS_LNCEL_LNREL.lnCelId = A_LTE_MRBTS_LNBTS_LNCEL.lnCelId)) "
                                + "INNER JOIN A_LTE_MRBTS_LNBTS_LNCEL AS A_LTE_MRBTS_LNBTS_LNCEL_1 "
                                + "ON(A_LTE_MRBTS_LNBTS_LNCEL_LNREL.ecgiLcrId = A_LTE_MRBTS_LNBTS_LNCEL_1.lnCelId) "
                                + "AND(A_LTE_MRBTS_LNBTS_LNCEL_LNREL.ecgiAdjEnbId = A_LTE_MRBTS_LNBTS_LNCEL_1.mrbtsId +0 )) "
                                + "LEFT JOIN A_LTE_MRBTS_LNBTS_LNADJ "
                                + "ON(A_LTE_MRBTS_LNBTS_LNCEL_LNREL.mrbtsId + 0 = A_LTE_MRBTS_LNBTS_LNADJ.mrbtsId +0) "
                                + "AND(A_LTE_MRBTS_LNBTS_LNCEL_LNREL.ecgiAdjEnbId = A_LTE_MRBTS_LNBTS_LNADJ.adjEnbId) "
                                + "WHERE " + SQLconditions + ";";


                if (srandividido)
                {
                    using (OleDbConnection connectionSRAN = new OleDbConnection(directionSRAN))
                    using (OleDbConnection connectionSRAN2 = new OleDbConnection(directionSRAN2))
                    using (OleDbConnection connectionFL18 = new OleDbConnection(directionFL18))
                    {
                        OleDbCommand commandSRAN = new OleDbCommand(SQLquerry, connectionSRAN);
                        OleDbCommand commandSRAN2 = new OleDbCommand(SQLquerry, connectionSRAN2);
                        OleDbCommand commandFL18 = new OleDbCommand(SQLquerry, connectionFL18);

                        //se cargan los resultados de la base de datos en una unica tabla
                        try
                        {
                            data = new DataTable();
                            connectionSRAN.Open();
                            using (OleDbDataReader SRANreader = commandSRAN.ExecuteReader())
                            {
                                data.Load(SRANreader);
                            }

                            connectionSRAN2.Open();
                            using (OleDbDataReader SRANreader2 = commandSRAN2.ExecuteReader())
                            {
                                data.Load(SRANreader2);
                            }


                            connectionFL18.Open();
                            using (OleDbDataReader FL18reader = commandFL18.ExecuteReader())
                            {
                                data.Load(FL18reader);
                            }


                           
                        }

                        catch (InvalidOperationException e)
                        {
                            //Si da error aqui suele ser que no estan los drivers de access database engine 2010 -- 32BITS
                            throw new InvalidOperationException("No se han podido acceder a access, podría deberse a no tener instalado Microsoft Access Engine 2010" +
                                "Redistributable o usar una version de compilacion no compatible con la version de office instalada actualmente. En cualquier caso es un problema " +
                                "complicado, avisad al informático más cercano. Y si no encuentra la solución dejadme un mensaje" + "  Access Database Engine se descarga de: https://www.microsoft.com/es-ES/download/details.aspx?id=13255    " + e);
                        }
                    }
                }

                // si solo se mete un fichero no se modifica nada
                else
                {
                    using (OleDbConnection connectionSRAN = new OleDbConnection(directionSRAN))
                    using (OleDbConnection connectionFL18 = new OleDbConnection(directionFL18))
                    {
                        OleDbCommand commandSRAN = new OleDbCommand(SQLquerry, connectionSRAN);
                        OleDbCommand commandFL18 = new OleDbCommand(SQLquerry, connectionFL18);

                        //se cargan los resultados de la base de datos en una unica tabla
                        try
                        {
                            data = new DataTable();
                            connectionSRAN.Open();
                            using (OleDbDataReader SRANreader = commandSRAN.ExecuteReader())
                            {
                                data.Load(SRANreader);
                            }

                            connectionFL18.Open();
                            using (OleDbDataReader FL18reader = commandFL18.ExecuteReader())
                            {
                                data.Load(FL18reader);
                            }
                        }
                        catch (InvalidOperationException e)
                        {
                            throw new InvalidOperationException("No se han podido acceder a access, podría deberse a no tener instalado Microsoft Access Engine 2010" +
                                "Redistributable o usar una version de compilacion no compatible con la version de office instalada actualmente. En cualquier caso es un problema " +
                                "complicado, avisad al informático más cercano. Y si no encuentra la solución dejadme un mensaje" + "  Access Database Engine se descarga de: https://www.microsoft.com/es-ES/download/details.aspx?id=13255    " + e);
                        }
                    }
                }


            }
            if (data == null)
            {
                throw new ArgumentException("No se ha podido extraer informacion de los exports, puede deberse a que los nodos de input no esten escritos correctamente");
            }
        } 
    }

}