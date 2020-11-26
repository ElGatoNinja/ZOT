using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZOT.HORAS48.Code
{
    class Auxiliar48
    {



        public string rutaExcelTabla()
        {
            return Path.Combine(Environment.CurrentDirectory, @"Data");
        }




        /// <summary> Metodo para exportar el Excel con el formato deseado
        /// <list type="bullet">
        /// <item>DataTable con los datos a exportar</item>
        /// <item>Nombre del fichero</item>
        /// </list>
        /// Ademas pregunta por la ruta al usuario
        /// </summary>
        public void exportarExcel(DataTable tablelist, string excelFilename)
        {
            try
            {
                var folderBrowserDialog1 = new FolderBrowserDialog();

                // Mostramos dialogo para sacar la ruta
                DialogResult result = folderBrowserDialog1.ShowDialog();
                if (result == DialogResult.OK)
                {
                    string folderName = folderBrowserDialog1.SelectedPath;
                    Console.WriteLine(folderName);



                    //Escribimos la cabecera formateada en el excel
                    Microsoft.Office.Interop.Excel.Application objexcelapp = new Microsoft.Office.Interop.Excel.Application();
                    objexcelapp.Application.Workbooks.Add(Type.Missing);
                    objexcelapp.Columns.AutoFit();
                    for (int i = 1; i < tablelist.Columns.Count + 1; i++)
                    {
                        Microsoft.Office.Interop.Excel.Range xlRange = (Microsoft.Office.Interop.Excel.Range)objexcelapp.Cells[1, i];
                        xlRange.Font.Bold = -1;
                        xlRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Green);
                        xlRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                        objexcelapp.Cells[1, i] = tablelist.Columns[i - 1].ColumnName;
                    }

                    //Rellendo el excel con la tabla y formatenado la columna del COC
                    for (int i = 0; i < tablelist.Rows.Count; i++)
                    {
                        for (int j = 0; j < tablelist.Columns.Count; j++)
                        {
                            if (tablelist.Rows[i][j] != null)
                            {
                                //Si estamos en la columna de cObra lo modificamos para que sea numero de 0 decimales
                                if (j == 6)
                                {
                                    Microsoft.Office.Interop.Excel.Range xlRange2 = (Microsoft.Office.Interop.Excel.Range)objexcelapp.Cells[i + 2, j + 1];
                                    xlRange2.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                                    xlRange2.Borders.Weight = 1d;
                                    xlRange2.NumberFormat = "0";
                                }
                                Microsoft.Office.Interop.Excel.Range xlRange = (Microsoft.Office.Interop.Excel.Range)objexcelapp.Cells[i + 2, j + 1];
                                xlRange.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                                xlRange.Borders.Weight = 1d;
                                xlRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                                objexcelapp.Cells[i + 2, j + 1] = tablelist.Rows[i][j].ToString();
                            }
                        }
                    }
                    objexcelapp.Columns.AutoFit(); // Para que se ajusten al tamaño
                    System.Windows.Forms.Application.DoEvents();

                    string ruta_completa = folderName + "\\" + excelFilename + ".xlsx";
                    if (Directory.Exists(folderName)) // Verificamos que existe la ruta...
                    {
                        objexcelapp.ActiveWorkbook.SaveCopyAs(ruta_completa);
                    }
                    else
                    {
                        Directory.CreateDirectory(folderName);
                        objexcelapp.ActiveWorkbook.SaveCopyAs(ruta_completa);
                    }
                    objexcelapp.ActiveWorkbook.Saved = true;
                    System.Windows.Forms.Application.DoEvents();
                    foreach (Process proc in System.Diagnostics.Process.GetProcessesByName("EXCEL"))
                    {
                        proc.Kill();
                    }
                    MessageBox.Show("Excel generado correctamente en: " + folderName, "Excel Correcto");
                    Process.Start("explorer.exe", folderName);
                }



            }
            catch (System.Runtime.InteropServices.COMException e)
            {
                MessageBox.Show("Hay un problema al generar el excel, posiblemente tengas otra Excel igual abierto.", "Excel No creado");
            }
        }








    }
}
