using System;
using System.Data;
using System.Data.OleDb;
public class Exports : FileReader
{
    public DataTable data;
    public Exports(string[] cellNames) :base("MicroSoft Data Base (*.mdb)|*.mdb","Selecciona los exports",true)
    {
        #if DEBUG
            string directionSRAN = "provider=Microsoft.ACE.OLEDB.12.0;Data Source= C:\\Users\\IS2-G-21\\Desktop\\SRAN.mdb";
            string directionFL18 = "provider=Microsoft.ACE.OLEDB.12.0;Data Source= C:\\Users\\IS2-G-21\\Desktop\\FL18.mdb";
        #else
            string directionSRAN = "provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + _path[0] + ";";
            string directionFL18 = "provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + _path[1] + ";";
        #endif

        string SQLconditions = "";
        foreach (string cell in cellNames)
        {
            SQLconditions += "A_LTE_MRBTS_LNBTS_LNCEL.cellName = " + cell + "AND";
        }

        string SQLquerry = "SELECT \"LNREL-\" &  A_LTE_MRBTS_LNBTS_LNCEL_LNREL.lnRelId AS Label,"
                            +"A_LTE_MRBTS_LNBTS_LNCEL_LNREL.mrbtsId, "
                            +"A_LTE_MRBTS_LNBTS_LNCEL_LNREL.lnCelId, "
                            +"A_LTE_MRBTS_LNBTS_LNCEL.cellName as srcName, "
                            +"A_LTE_MRBTS_LNBTS_LNCEL_LNREL.ecgiAdjEnbId, "
                            +"A_LTE_MRBTS_LNBTS_LNCEL_LNREL.ecgiLcrId, "
                            +"A_LTE_MRBTS_LNBTS_LNCEL_1.cellName as dstName, "
                            +"A_LTE_MRBTS_LNBTS_LNCEL_LNREL.HandoverAllowed, "
                            +"A_LTE_MRBTS_LNBTS_LNCEL_LNREL.cellIndOffNeigh "
                            +"FROM (A_LTE_MRBTS_LNBTS_LNCEL INNER JOIN A_LTE_MRBTS_LNBTS_LNCEL_LNREL "
                            +"ON (A_LTE_MRBTS_LNBTS_LNCEL.lnCelId = A_LTE_MRBTS_LNBTS_LNCEL_LNREL.lnCelId) "
                            +"AND (A_LTE_MRBTS_LNBTS_LNCEL.lnBtsId = A_LTE_MRBTS_LNBTS_LNCEL_LNREL.lnBtsId) "
                            +"AND (A_LTE_MRBTS_LNBTS_LNCEL.mrbtsId = A_LTE_MRBTS_LNBTS_LNCEL_LNREL.mrbtsId)) "
                            +"INNER JOIN A_LTE_MRBTS_LNBTS_LNCEL AS A_LTE_MRBTS_LNBTS_LNCEL_1 "
                            +"ON (A_LTE_MRBTS_LNBTS_LNCEL_LNREL.ecgiLcrId = A_LTE_MRBTS_LNBTS_LNCEL_1.lnCelId) "
                            +"AND (A_LTE_MRBTS_LNBTS_LNCEL_LNREL.ecgiAdjEnbId = A_LTE_MRBTS_LNBTS_LNCEL_1.mrbtsId)" 
                            +"WHERE" + SQLconditions ;
        using(OleDbConnection connectionSRAN = new OleDbConnection(directionSRAN))
        using(OleDbConnection connectionFL18 = new OleDbConnection(directionFL18))
        {
            OleDbCommand commandSRAN = new OleDbCommand(SQLquerry,connectionSRAN);
            OleDbCommand commandFL18 = new OleDbCommand(SQLquerry,connectionSRAN);

            try
            {
                connectionSRAN.Open();
                using(OleDbDataReader SRANreader = commandSRAN.ExecuteReader())
                {
                    data = new DataTable();
                    data.Load(SRANreader);
                }
                
                connectionFL18.Open();
                using(OleDbDataReader FL18reader= commandFL18.ExecuteReader())
                {
                    data.Load(FL18reader,LoadOption.PreserveChanges);
                }
            }
            catch (InvalidOperationException ioe)
            {
                Console.WriteLine(ioe.Message);
                Console.WriteLine("Posible solucion, instalar Microsoft Access Database Engine 2010 Redistributable.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Fallo en la consulta de la base de datos. \n Mensaje de error: " +e.Message);
            }
            
        }


    }
}