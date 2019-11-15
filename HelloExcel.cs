using System;
using System.Diagnostics;
//using ExcelDna.Integration;
using Excel =  Microsoft.Office.Interop.Excel;

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
    private static void CheckTimingAdvance()
    {
        var excelApp = new Excel.Application();
        excelApp.Visible = true;
        Excel.Workbook wb = (Excel.Workbook)excelApp.Workbooks.Add();
        Excel.Worksheet ws = (Excel.Worksheet)wb.ActiveSheet;
        Excel.Worksheet ws2 = (Excel.Worksheet)wb.Sheets["Hoja2"];
        Excel.Range row = ws.Range["A1:BW1"];
        string[] lnBtsInputs = {"ENB_O_AVILES_MAGDALENA_CT_01","ENB_PO_SAN_VICENTE_EB_01"};
        TimingAdvance TA = new TimingAdvance(lnBtsInputs);
        for(int i = 0;i<TA.data.Count;i++)
        {
            row.Offset[i,0].Value2 = TA.data[i];
        }
        RSLTE31 R31 = new RSLTE31(lnBtsInputs);
        row = ws2.Range["A1:AB1"];
        for(int i = 0;i<R31.data.Count;i++)
        {
            row.Offset[i,0].Value2 = R31.data[i];
        }
    }
    [STAThread]
    public static void Main(string[] args)
    {
            Stopwatch globalWatch = new Stopwatch();
            globalWatch.Start();
            CheckTimingAdvance();
            globalWatch.Stop();
            Console.WriteLine("Global time: "+ globalWatch.ElapsedMilliseconds); 
    }
}