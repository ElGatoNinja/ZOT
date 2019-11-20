using System;
using System.Diagnostics;
using System.Threading;
//using ExcelDna.Integration;
using Excel =  Microsoft.Office.Interop.Excel;
using System.Data;

public class HelloExcel
{
    public static void SayHello()
    {
        var excelApp = new Excel.Application();
        excelApp.Visible = true;
        excelApp.Workbooks.Add();
        Excel.Worksheet ws = (Excel.Worksheet)excelApp.ActiveSheet;
        Excel.Range range = ws.Range["A1:D1"];

        string[] values = {"HOLA EXCEL","TE HABLO","DESDE C#","PORQUE VBA APESTA"};
        range.Value = values;
    }
    private static void CheckExports()
    {
        return;
    }
    private static void CheckR31()
    {
        string[] lnBtsInputs = {"ENB_O_AVILES_MAGDALENA_CT_01","ENB_PO_SAN_VICENTE_EB_01"};
        RSLTE31 R31 = new RSLTE31(lnBtsInputs);
        Console.WriteLine(R31.data.Count);
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
            string[] lnBtsInputs = {"ENB_O_AVILES_MAGDALENA_CT_01","ENB_PO_SAN_VICENTE_EB_01"};
            RSLTE31 R31 = new RSLTE31(lnBtsInputs);
            
            TimingAdvance TA = new TimingAdvance(lnBtsInputs);

            //Console.WriteLine(TA.radioLines[3][0] +": " + TA.radioLines[3][1]);
           

            Exports export = new Exports(TA.GetColumn("LNCEL name"));

            /*Thread Main = Thread.CurrentThread;
            Thread TA = new Thread(CheckTimingAdvance);
            TA.Start();
            Thread R31 = new Thread(CheckR31);
            R31.Start();
            TA.Join();
            R31.Join();
            Console.WriteLine("Me he motivado");*/
            globalWatch.Stop();
            Console.WriteLine("Global time: "+ globalWatch.ElapsedMilliseconds); 
    }
}