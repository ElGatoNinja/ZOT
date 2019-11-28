using System;
using System.Diagnostics;
using System.Threading;
//using ExcelDna.Integration;
using System.Data;
using System.Threading.Tasks;

public class HelloExcel
{
    private static void CheckExports()
    {
        return;
    }
    private static void CheckR31()
    {
        string[] lnBtsInputs = {"ENB_O_AVILES_MAGDALENA_CT_01","ENB_PO_SAN_VICENTE_EB_01"};
        RSLTE31 R31 = new RSLTE31(lnBtsInputs);
    }
    private static void CheckTimingAdvance()
    {
        string[] lnBtsInputs = {"ENB_O_AVILES_MAGDALENA_CT_01","ENB_PO_SAN_VICENTE_EB_01"};

        TimingAdvance TA = new TimingAdvance(lnBtsInputs);
        //Console.WriteLine(TA.radioLines[3][0] +": " + TA.radioLines[3][1]);

    }
    [STAThread]
    public static void Main(string[] args)
    {
        Stopwatch globalWatch = new Stopwatch();
        globalWatch.Start();

        string[] lnBtsInputs = {"ENB_O_AVILES_MAGDALENA_CT_01","ENB_PO_SAN_VICENTE_EB_01", "ENB_OU_HABANA_COREN_01" };
        RSLTE31 R31 = new RSLTE31(lnBtsInputs);
            
        TimingAdvance TA = new TimingAdvance(lnBtsInputs);
        Console.WriteLine(TA.radioLines[3][0] +": " + TA.radioLines[3][1]);
           
        Exports export = new Exports(TA.GetColumn("LNCEL name"));
        Colindancias colindancias = new Colindancias();

        /*Parallel.ForEach(export.data.AsEnumerable(), dataRow =>
        {
            colindancias.CheckColindance(dataRow, R31);
        });*/

        foreach(DataRow dataRow in export.data.Rows)
        {
            colindancias.CheckColindance(dataRow, R31);
        }

        foreach(object[] line in colindancias.colinLines)
        {
            foreach(object item in line)
            {
                Console.Write(item + "  -  ");
            }
            Console.Write('\n');
        }
        globalWatch.Stop();
        Console.WriteLine("Global time: "+ (double)globalWatch.ElapsedMilliseconds/1000.0 +"s");
    }
}