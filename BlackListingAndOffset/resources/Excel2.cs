using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using _Excel = Microsoft.Office.Interop.Excel;

namespace ZOT.resources
{
    class Excel2
    {
        string path = "";
        _Application excel = new _Excel.Application();
        Workbook wb;
        Worksheet ws;


        public Excel2(string path, int Sheet)
        {
            this.path = path;
            wb = excel.Workbooks.Open(path);
            ws = wb.Worksheets[Sheet];
        }

        public Excel2()
        {
            
        }


        public void CreateNewFile()
        {
            this.wb = excel.Workbooks.Add();
            this.ws = wb.Worksheets[1];
        }

        public void test()
        {


           

        }


        public void CreateNewSheet(String nombre)
        {
            var newSheet = wb.Worksheets.Add(Before: ws);
            newSheet.Name = nombre;
        }


        public string ReadCell(int i, int j)
        {
            //Para entender mejor la celda ya que excel empieza en [1,1] y sino aqui seria posicion 0, 0
            i++;
            j++;

            //Value2 es el valor dentro de la celda
            if (ws.Cells[i, j].Value2 != null)
                return ws.Cells[i, j].Value2;
            else
                return "";
        }


        public void WriteToCell(int i, int j, string s)
        {
            //Para entender mejor la celda ya que excel empieza en [1,1] y sino aqui seria posicion 0, 0
            i++;
            j++;

            ws.Cells[i, j].Value2 = s;

        }


        public void Save()
        {
            wb.Save();
        }

        public void SaveAs(string path)
        {
            wb.SaveAs(path);
        }

        public void Close()
        {
            wb.Close();
        }

    }


}
