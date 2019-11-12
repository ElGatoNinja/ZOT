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
        Excel._Worksheet ws = (Excel.Worksheet)excelApp.ActiveSheet;
        ws.Cells[1,"A"] = "Hello Excel";
    }
    public static void Main (string[] args){
        SayHello();
    }
}