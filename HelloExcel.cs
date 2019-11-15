using System;
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
        Excel.Worksheet ws = (Excel.Worksheet)excelApp.Workbooks.Add().ActiveSheet;
        Excel.Range row = ws.Range["A1:BX1"];
        string[] lnBtsInputs = {"ENB_O_AVILES_MAGDALENA_CT_01","ENB_PO_SAN_VICENTE_EB_01"};
        TimingAdvance TA = new TimingAdvance(lnBtsInputs);
        for(int i = 0;i<TA.data.Count;i++)
        {
            row.Offset[i,0].Value2 = TA.data[i];
        }
    }
    [STAThread]
    public static void Main(string[] args) => CheckTimingAdvance();
}