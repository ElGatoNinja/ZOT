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
        public Exports(string[] cellNames, string pathSRAN, string pathFL18, string pathSRAN2, bool srandividido, bool conFL18) : base()
        {

            string directionSRAN = string.Format("provider=Microsoft.ACE.OLEDB.12.0;Data Source= {0}", pathSRAN);
            string directionSRAN2 = string.Format("provider=Microsoft.ACE.OLEDB.12.0;Data Source= {0}", pathSRAN2);
            string directionFL18 = string.Format("provider=Microsoft.ACE.OLEDB.12.0;Data Source= {0}", pathFL18);



            data = new DataTable();

            int i = 0;
            
            while (i < cellNames.Length)
            {
                string SQLquerry = "";
                if (srandividido)
                {

                    //no se pueden hacer busquedas sql con mas de 99 OR y AND, así que el siguiente bucle hace consultas "paginadas"
                    //hace una busqueda para cada 98 emplazamientos, 98 * (1OR) = 98 OR
                    string SQLconditions = "A_LTE_MRBTS_LNBTS_LNCEL.cellName = \"" + cellNames[i] + "\"";
                    for (int j = 0; j < 98 && i < cellNames.Length; j++)
                    {

                        SQLconditions += " OR A_LTE_MRBTS_LNBTS_LNCEL.cellName = \"" + cellNames[i] + "\"";
                        i++;
                    }

                    SQLquerry = "SELECT \"LNREL-\" & A_LTE_MRBTS_LNBTS_LNCEL_LNREL.lnRelId AS Label, "
                                     + "A_LTE_MRBTS_LNBTS_LNCEL_LNREL.mrbtsId, A_LTE_MRBTS_LNBTS_LNCEL_LNREL.lnCelId, "
                                    + "A_LTE_MRBTS_LNBTS_LNCEL.cellName AS srcName, "
                                    + "A_LTE_MRBTS_LNBTS_LNCEL_LNREL.ecgiAdjEnbId, "
                                    + "A_LTE_MRBTS_LNBTS_LNCEL_LNREL.ecgiLcrId, "
                                    + "A_LTE_MRBTS_LNBTS_LNCEL_1.cellName AS dstName, "
                                    + "A_LTE_MRBTS_LNBTS_LNCEL_LNREL.handoverAllowed, "
                                    + "A_LTE_MRBTS_LNBTS_LNCEL_LNREL.cellIndOffNeigh, "
                                    + "A_LTE_MRBTS_LNBTS_LNADJ.x2LinkStatus "
                                    + "FROM((A_LTE_MRBTS_LNBTS_LNCEL_LNREL INNER JOIN A_LTE_MRBTS_LNBTS_LNCEL "
                                    + "ON(A_LTE_MRBTS_LNBTS_LNCEL_LNREL.mrbtsId +0 = A_LTE_MRBTS_LNBTS_LNCEL.mrbtsId +0 ) "
                                    + "AND(A_LTE_MRBTS_LNBTS_LNCEL_LNREL.lnBtsId = A_LTE_MRBTS_LNBTS_LNCEL.lnBtsId) "
                                    + "AND(A_LTE_MRBTS_LNBTS_LNCEL_LNREL.lnCelId = A_LTE_MRBTS_LNBTS_LNCEL.lnCelId)) "
                                    + "LEFT JOIN A_LTE_MRBTS_LNBTS_LNCEL AS A_LTE_MRBTS_LNBTS_LNCEL_1 "
                                    + "ON(A_LTE_MRBTS_LNBTS_LNCEL_LNREL.ecgiLcrId = A_LTE_MRBTS_LNBTS_LNCEL_1.lnCelId) "
                                    + "AND(A_LTE_MRBTS_LNBTS_LNCEL_LNREL.ecgiAdjEnbId = A_LTE_MRBTS_LNBTS_LNCEL_1.mrbtsId +0 )) "
                                    + "LEFT JOIN A_LTE_MRBTS_LNBTS_LNADJ "
                                    + "ON(A_LTE_MRBTS_LNBTS_LNCEL_LNREL.mrbtsId +0 = A_LTE_MRBTS_LNBTS_LNADJ.mrbtsId +0) "
                                    + "AND(A_LTE_MRBTS_LNBTS_LNCEL_LNREL.ecgiAdjEnbId = A_LTE_MRBTS_LNBTS_LNADJ.adjEnbId) "
                                    + "WHERE " + SQLconditions + ";";


                }

                //si no esta dividido la consulta es la vieja
                else
                {

                    //no se pueden hacer busquedas sql con mas de 99 OR y AND, así que el siguiente bucle hace consultas "paginadas"
                    //hace una busqueda para cada 98 emplazamientos, 98 * (1OR) = 98 OR
                    string SQLconditions = "A_LTE_MRBTS_LNBTS_LNCEL.cellName = \"" + cellNames[i] + "\"";
                    for (int j = 0; j < 98 && i < cellNames.Length; j++)
                    {

                        SQLconditions += " OR A_LTE_MRBTS_LNBTS_LNCEL.cellName = \"" + cellNames[i] + "\"";
                        i++;
                    }

                    SQLquerry = "SELECT \"LNREL-\" & A_LTE_MRBTS_LNBTS_LNCEL_LNREL.lnRelId AS Label, "
                                    + "A_LTE_MRBTS_LNBTS_LNCEL_LNREL.mrbtsId, A_LTE_MRBTS_LNBTS_LNCEL_LNREL.lnCelId, "
                                    + "A_LTE_MRBTS_LNBTS_LNCEL.cellName AS srcName, "
                                    + "A_LTE_MRBTS_LNBTS_LNCEL_LNREL.ecgiAdjEnbId, "
                                    + "A_LTE_MRBTS_LNBTS_LNCEL_LNREL.ecgiLcrId, "
                                    + "A_LTE_MRBTS_LNBTS_LNCEL_1.cellName AS dstName, "
                                    + "A_LTE_MRBTS_LNBTS_LNCEL_LNREL.handoverAllowed, "
                                    + "A_LTE_MRBTS_LNBTS_LNCEL_LNREL.cellIndOffNeigh, "
                                    + "A_LTE_MRBTS_LNBTS_LNADJ.x2LinkStatus "
                                    + "FROM((A_LTE_MRBTS_LNBTS_LNCEL_LNREL INNER JOIN A_LTE_MRBTS_LNBTS_LNCEL "
                                    + "ON(A_LTE_MRBTS_LNBTS_LNCEL_LNREL.mrbtsId +0 = A_LTE_MRBTS_LNBTS_LNCEL.mrbtsId +0 ) "
                                    + "AND(A_LTE_MRBTS_LNBTS_LNCEL_LNREL.lnBtsId = A_LTE_MRBTS_LNBTS_LNCEL.lnBtsId) "
                                    + "AND(A_LTE_MRBTS_LNBTS_LNCEL_LNREL.lnCelId = A_LTE_MRBTS_LNBTS_LNCEL.lnCelId)) "
                                    + "INNER JOIN A_LTE_MRBTS_LNBTS_LNCEL AS A_LTE_MRBTS_LNBTS_LNCEL_1 "
                                    + "ON(A_LTE_MRBTS_LNBTS_LNCEL_LNREL.ecgiLcrId = A_LTE_MRBTS_LNBTS_LNCEL_1.lnCelId) "
                                    + "AND(A_LTE_MRBTS_LNBTS_LNCEL_LNREL.ecgiAdjEnbId = A_LTE_MRBTS_LNBTS_LNCEL_1.mrbtsId +0 )) "
                                    + "LEFT JOIN A_LTE_MRBTS_LNBTS_LNADJ "
                                    + "ON(A_LTE_MRBTS_LNBTS_LNCEL_LNREL.mrbtsId +0 = A_LTE_MRBTS_LNBTS_LNADJ.mrbtsId +0) "
                                    + "AND(A_LTE_MRBTS_LNBTS_LNCEL_LNREL.ecgiAdjEnbId = A_LTE_MRBTS_LNBTS_LNADJ.adjEnbId) "
                                    + "WHERE " + SQLconditions + ";";

                }




                if (srandividido && conFL18)
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
                    //Relleno si vacios -- Pelayo 2020 para dos exports rellenar colindancias
                    OleDbConnection connectionSRANBusqueda1 = new OleDbConnection(directionSRAN);
                    using (connectionSRANBusqueda1)
                    {
                        //contador para sacar la row en la tabla
                        int contadorRow = 0;
                        foreach (DataRow row in data.Rows)
                        {
                            var ecgiAdjEnbId = row.Field<int>(4);
                            var ecgiLcrId = row.Field<int>(5);
                            var dstName = row.Field<string>(6);

                            //si esta vacio dstName tengo que buscarlo en el otro
                            if (dstName == null | dstName == "")
                            {
                                //Lo busco en el primer export
                                string SQLquerryBusqueda1 = "SELECT A_LTE_MRBTS_LNBTS_LNCEL.cellName, A_LTE_MRBTS_LNBTS_LNCEL.lnBtsId, A_LTE_MRBTS_LNBTS_LNCEL.lnCelId FROM A_LTE_MRBTS_LNBTS_LNCEL WHERE(((A_LTE_MRBTS_LNBTS_LNCEL.lnBtsId) = " + ecgiAdjEnbId + ") AND((A_LTE_MRBTS_LNBTS_LNCEL.lnCelId) = " + ecgiLcrId + "));";



                                OleDbCommand commandSRANBusqueda1 = new OleDbCommand(SQLquerryBusqueda1, connectionSRANBusqueda1);
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
                                        data.Rows[contadorRow].SetField("dstName", nuevodtName);
                                    }
                                }
                                commandSRANBusqueda1.Connection.Close();
                            }

                            contadorRow++;
                        }
                    }
                    connectionSRANBusqueda1.Close();


                    //Relleno si vacios pero en el segundo SRAN -- Pelayo 2020 para dos exports rellenar colindancias
                    OleDbConnection connectionSRANBusqueda2 = new OleDbConnection(directionSRAN2);
                    using (connectionSRANBusqueda2)
                    {
                        //contador para sacar la row en la tabla
                        int contadorRow2 = 0;
                        foreach (DataRow row in data.Rows)
                        {
                            var ecgiAdjEnbId = row.Field<int>(4);
                            var ecgiLcrId = row.Field<int>(5);
                            var dstName = row.Field<string>(6);

                            //si esta vacio dstName tengo que buscarlo en el otro
                            if (dstName == null | dstName == "")
                            {
                                //Lo busco en el primer export
                                string SQLquerryBusqueda2 = "SELECT A_LTE_MRBTS_LNBTS_LNCEL.cellName, A_LTE_MRBTS_LNBTS_LNCEL.lnBtsId, A_LTE_MRBTS_LNBTS_LNCEL.lnCelId FROM A_LTE_MRBTS_LNBTS_LNCEL WHERE(((A_LTE_MRBTS_LNBTS_LNCEL.lnBtsId) = " + ecgiAdjEnbId + ") AND((A_LTE_MRBTS_LNBTS_LNCEL.lnCelId) = " + ecgiLcrId + "));";



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
                                        data.Rows[contadorRow2].SetField("dstName", nuevodtName);
                                    }
                                }
                                commandSRANBusqueda2.Connection.Close();
                            }

                            contadorRow2++;
                        }
                    }
                    connectionSRANBusqueda2.Close();

                    //Recooro una vez mas por si no esta en ninnguno de los dos
                    int contadorRow3 = 0;
                    foreach (DataRow row in data.Rows)
                    {
                        var dstName = row.Field<string>(6);
                        //si esta vacio dstName tengo que ponerle UNKNOWN (no se busca en fl18 porque se presupone que no va a estar)
                        if (dstName == null | dstName == "")
                        {
                            data.Rows[contadorRow3].SetField("dstName", "UNKNOWN");
                        }

                        contadorRow3++;
                    }


                }


                else if (srandividido && !conFL18)
                {

                    using (OleDbConnection connectionSRAN = new OleDbConnection(directionSRAN))
                    using (OleDbConnection connectionSRAN2 = new OleDbConnection(directionSRAN2))
                    {
                        OleDbCommand commandSRAN = new OleDbCommand(SQLquerry, connectionSRAN);
                        OleDbCommand commandSRAN2 = new OleDbCommand(SQLquerry, connectionSRAN2);

                        //se cargan los resultados de la base de datos en una unica tabla
                        try
                        {
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






                        }

                        catch (InvalidOperationException e)
                        {
                            //Si da error aqui suele ser que no estan los drivers de access database engine 2010 -- 32BITS
                            throw new InvalidOperationException("No se han podido acceder a access, podría deberse a no tener instalado Microsoft Access Engine 2010" +
                                "Redistributable o usar una version de compilacion no compatible con la version de office instalada actualmente. En cualquier caso es un problema " +
                                "complicado, avisad al informático más cercano. Y si no encuentra la solución dejadme un mensaje" + "  Access Database Engine se descarga de: https://www.microsoft.com/es-ES/download/details.aspx?id=13255    " + e);
                        }

                    }
                    //Relleno si vacios -- Pelayo 2020 para dos exports rellenar colindancias
                    OleDbConnection connectionSRANBusqueda1 = new OleDbConnection(directionSRAN);
                    using (connectionSRANBusqueda1)
                    {
                        //contador para sacar la row en la tabla
                        int contadorRow = 0;
                        foreach (DataRow row in data.Rows)
                        {
                            var ecgiAdjEnbId = row.Field<int>(4);
                            var ecgiLcrId = row.Field<int>(5);
                            var dstName = row.Field<string>(6);

                            //si esta vacio dstName tengo que buscarlo en el otro
                            if (dstName == null | dstName == "")
                            {
                                //Lo busco en el primer export
                                string SQLquerryBusqueda1 = "SELECT A_LTE_MRBTS_LNBTS_LNCEL.cellName, A_LTE_MRBTS_LNBTS_LNCEL.lnBtsId, A_LTE_MRBTS_LNBTS_LNCEL.lnCelId FROM A_LTE_MRBTS_LNBTS_LNCEL WHERE(((A_LTE_MRBTS_LNBTS_LNCEL.lnBtsId) = " + ecgiAdjEnbId + ") AND((A_LTE_MRBTS_LNBTS_LNCEL.lnCelId) = " + ecgiLcrId + "));";



                                OleDbCommand commandSRANBusqueda1 = new OleDbCommand(SQLquerryBusqueda1, connectionSRANBusqueda1);
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
                                        data.Rows[contadorRow].SetField("dstName", nuevodtName);
                                    }
                                }
                                commandSRANBusqueda1.Connection.Close();
                            }

                            contadorRow++;
                        }
                    }
                    connectionSRANBusqueda1.Close();


                    //Relleno si vacios pero en el segundo SRAN -- Pelayo 2020 para dos exports rellenar colindancias
                    OleDbConnection connectionSRANBusqueda2 = new OleDbConnection(directionSRAN2);
                    using (connectionSRANBusqueda2)
                    {
                        //contador para sacar la row en la tabla
                        int contadorRow2 = 0;
                        foreach (DataRow row in data.Rows)
                        {
                            var ecgiAdjEnbId = row.Field<int>(4);
                            var ecgiLcrId = row.Field<int>(5);
                            var dstName = row.Field<string>(6);

                            //si esta vacio dstName tengo que buscarlo en el otro
                            if (dstName == null | dstName == "")
                            {
                                //Lo busco en el primer export
                                string SQLquerryBusqueda2 = "SELECT A_LTE_MRBTS_LNBTS_LNCEL.cellName, A_LTE_MRBTS_LNBTS_LNCEL.lnBtsId, A_LTE_MRBTS_LNBTS_LNCEL.lnCelId FROM A_LTE_MRBTS_LNBTS_LNCEL WHERE(((A_LTE_MRBTS_LNBTS_LNCEL.lnBtsId) = " + ecgiAdjEnbId + ") AND((A_LTE_MRBTS_LNBTS_LNCEL.lnCelId) = " + ecgiLcrId + "));";



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
                                        data.Rows[contadorRow2].SetField("dstName", nuevodtName);
                                    }
                                }
                                commandSRANBusqueda2.Connection.Close();
                            }

                            contadorRow2++;
                        }
                    }
                    connectionSRANBusqueda2.Close();

                    //Recooro una vez mas por si no esta en ninnguno de los dos
                    int contadorRow3 = 0;
                    foreach (DataRow row in data.Rows)
                    {
                        var dstName = row.Field<string>(6);
                        //si esta vacio dstName tengo que ponerle UNKNOWN (no se busca en fl18 porque se presupone que no va a estar)
                        if (dstName == null | dstName == "")
                        {
                            data.Rows[contadorRow3].SetField("dstName", "UNKNOWN");
                        }

                        contadorRow3++;
                    }




                }


                //si no esta dividio y no tiene FL18
                else if (!srandividido && !conFL18)
                {
                    using (OleDbConnection connectionSRAN = new OleDbConnection(directionSRAN))
                    {
                        OleDbCommand commandSRAN = new OleDbCommand(SQLquerry, connectionSRAN);

                        //se cargan los resultados de la base de datos en una unica tabla
                        try
                        {
                            data = new DataTable();
                            connectionSRAN.Open();
                            using (OleDbDataReader SRANreader = commandSRAN.ExecuteReader())
                            {
                                data.Load(SRANreader);
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