using System;
using System.Data;
using System.Data.OleDb;
using System.IO;

public class Exports : FileReader
{
    public DataTable data;
    public Exports(string[] cellNames) :base("MicroSoft Data Base (*.mdb)|*.mdb","Selecciona los exports",true)
    {
        #if DEBUG
            string directionSRAN = string.Format("provider=Microsoft.ACE.OLEDB.12.0;Data Source= {0}\\data\\SRAN.mdb", Directory.GetCurrentDirectory());
            string directionFL18 = string.Format("provider=Microsoft.ACE.OLEDB.12.0;Data Source= {0}\\data\\FL18.mdb", Directory.GetCurrentDirectory());
        #else
            string directionSRAN = "provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + _path[0] + ";";
            string directionFL18 = "provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + _path[1] + ";";
        #endif

        string SQLconditions = "A_LTE_MRBTS_LNBTS_LNCEL.cellName = \"" + cellNames[0] + "\"";
        for (int i = 1;i<cellNames.Length;i++)
        {
            SQLconditions += " OR A_LTE_MRBTS_LNBTS_LNCEL.cellName = \"" + cellNames[i] + "\"";
        }

        string SQLquerry = "SELECT \"LNREL-\" & A_LTE_MRBTS_LNBTS_LNCEL_LNREL.lnRelId AS Label, "
                        +"A_LTE_MRBTS_LNBTS_LNCEL_LNREL.mrbtsId, "
                        +"A_LTE_MRBTS_LNBTS_LNCEL_LNREL.lnCelId, "
                        +"A_LTE_MRBTS_LNBTS_LNCEL.cellName as srcName, "
                        +"A_LTE_MRBTS_LNBTS_LNCEL_LNREL.ecgiAdjEnbId, "
                        +"A_LTE_MRBTS_LNBTS_LNCEL_LNREL.ecgiLcrId, "
                        +"A_LTE_MRBTS_LNBTS_LNCEL_1.cellName as dstName, "
                        +"A_LTE_MRBTS_LNBTS_LNCEL_LNREL.handoverAllowed, "
                        +"A_LTE_MRBTS_LNBTS_LNCEL_LNREL.cellIndOffNeigh, "
                        +"A_LTE_MRBTS_LNBTS_LNADJ.x2LinkStatus "
                        +"FROM((A_LTE_MRBTS_LNBTS_LNCEL_LNREL INNER JOIN A_LTE_MRBTS_LNBTS_LNCEL "
                        +"ON(A_LTE_MRBTS_LNBTS_LNCEL_LNREL.lnCelId = A_LTE_MRBTS_LNBTS_LNCEL.lnCelId) "
                        +"AND(A_LTE_MRBTS_LNBTS_LNCEL_LNREL.lnBtsId = A_LTE_MRBTS_LNBTS_LNCEL.lnBtsId) "
                        +"AND(A_LTE_MRBTS_LNBTS_LNCEL_LNREL.mrbtsId = A_LTE_MRBTS_LNBTS_LNCEL.mrbtsId)) "
                        +"INNER JOIN A_LTE_MRBTS_LNBTS_LNCEL AS A_LTE_MRBTS_LNBTS_LNCEL_1 "
                        +"ON(A_LTE_MRBTS_LNBTS_LNCEL_LNREL.ecgiAdjEnbId = A_LTE_MRBTS_LNBTS_LNCEL_1.mrbtsId) "
                        +"AND(A_LTE_MRBTS_LNBTS_LNCEL_LNREL.ecgiLcrId = A_LTE_MRBTS_LNBTS_LNCEL_1.lnCelId)) "
                        +"INNER JOIN A_LTE_MRBTS_LNBTS_LNADJ "
                        +"ON(A_LTE_MRBTS_LNBTS_LNCEL_LNREL.ecgiAdjEnbId = A_LTE_MRBTS_LNBTS_LNADJ.adjEnbId) "
                        +"AND(A_LTE_MRBTS_LNBTS_LNCEL_LNREL.mrbtsId = A_LTE_MRBTS_LNBTS_LNADJ.mrbtsId) "
                        +"WHERE " + SQLconditions + ";";

        using (OleDbConnection connectionSRAN = new OleDbConnection(directionSRAN))
        using(OleDbConnection connectionFL18 = new OleDbConnection(directionFL18))
        {
            OleDbCommand commandSRAN = new OleDbCommand(SQLquerry, connectionSRAN);
            OleDbCommand commandFL18 = new OleDbCommand(SQLquerry,connectionFL18);

            try
            {
                data = new DataTable();
                connectionSRAN.Open();
                using (OleDbDataReader SRANreader = commandSRAN.ExecuteReader())
                {
                   data.Load(SRANreader);
                }

                connectionFL18.Open();
                using(OleDbDataReader FL18reader= commandFL18.ExecuteReader())
                {
                    data.Load(FL18reader);
                }
            }
            catch (InvalidOperationException ioe)
            {
                Console.WriteLine(ioe.StackTrace);
                Console.WriteLine("Instalad Microsoft database 2010 redistributable compatible con vuestra version de office.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Fallo en la consulta de la base de datos. \n Mensaje de error: " + e.Message);
            }

        }


    }

}