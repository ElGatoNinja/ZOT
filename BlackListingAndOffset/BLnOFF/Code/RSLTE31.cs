using System.IO;
using System.Collections.Generic;
using System;
using System.Data;
using ZOT.resources;
using ZOT.resources.ZOTlib;
using System.Linq;
using System.Data.OleDb;
using static ZOT.BLnOFF.GUI.TabControlBLnOFF;
using System.Windows;
using System.Text;
using Microsoft.VisualBasic.FileIO;
using ZOT.BLnOFF.GUI;

namespace ZOT.BLnOFF.Code
{
    public class RSLTE31
    {
        public HashSet<string> celdasMasmovilRSLTE = new HashSet<string>();
        public String nuevoPatch = "";

        public DataTable data;
        public bool[] inExports;
        public RSLTE31(string[] lnBtsInputs, string path)
        {
            nuevoPatch = path;


            data = new DataTable();



            //SACO SI EL FICHERO ES .xlsx
            string extension = Path.GetExtension(path);

            if (extension == ".xlsx")
            {
                celdasMasmovilRSLTE = new HashSet<string>();

                //Si es un xlsx se pasa a .csv
                // para ello se abre y se guarda como csv. Pero esto lo guarda con , como delimintador asi que se tiene qeu abrir el csv y cambiar las , por ;
                // despues si un numero tiene , separando los mires se cambia elimina, con .Replace(',','')

                //Tambien se modifican las cabeceras de los ficheros


                Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
                Microsoft.Office.Interop.Excel.Workbook wb = excel.Workbooks.Open(path);
                Microsoft.Office.Interop.Excel.Worksheet ws = wb.Sheets[1];


                Microsoft.Office.Interop.Excel.Range rngFila2 = (Microsoft.Office.Interop.Excel.Range)ws.Application.Rows[2, Type.Missing];
                rngFila2.Delete(Microsoft.Office.Interop.Excel.XlDirection.xlUp);

                //Se borra la columna 16
                //ws.Columns[16].Delete();

                //Se ponen las cabeceras
                ws.Cells[1, 16].Value = "Intra eNB neighbor HO: Prep SR";
                ws.Cells[1, 17].Value = "Intra eNB neighbor HO: SR";
                ws.Cells[1, 18].Value = "Intra eNB neighbor HO: Att";
                ws.Cells[1, 19].Value = "Intra eNB neighbor HO: Cancel R";
                ws.Cells[1, 20].Value = "Inter eNB neighbor HO: Prep SR";
                ws.Cells[1, 21].Value = "Inter eNB neighbor HO: SR";
                ws.Cells[1, 22].Value = "Inter eNB neighbor HO: Att";
                ws.Cells[1, 23].Value = "Inter eNB neighbor HO: FR";
                ws.Cells[1, 24].Value = "Load Balancing HO, per neighbor: SR";
                ws.Cells[1, 25].Value = "Load Balancing HO, per neighbor: Att";
                ws.Cells[1, 26].Value = "Late/Early HO ratio, per neighbor: Late HO";
                ws.Cells[1, 27].Value = "Late/Early HO ratio, per neighbor: Early HO, type 1";
                ws.Cells[1, 28].Value = "Late/Early HO ratio, per neighbor: Early HO, type 2";

                try {
                    //Para borrar celdas masmovil
                    int rows = ws.UsedRange.Rows.Count;
                    int fRowIndex = ((Microsoft.Office.Interop.Excel.Range)(ws.UsedRange)).Row;

                    for (int rowCounter = rows; rowCounter > 1; rowCounter--) //Hasta row 3 porque la siguiente es la cabecera (nueva cabecera tienen 2 filas)
                    {
                        //Si no tiene un guion bajo la borro por ser de MasMovil
                        String s = ws.Rows.Cells[rowCounter, 5].Value;
                        
                        
                        if ( !s.Contains('_') || s[0] == '=' || (s[0] == 'A' && s[1] == '9')) {
                            celdasMasmovilRSLTE.Add(ws.Rows.Cells[rowCounter, 5].Value);
                            ((Microsoft.Office.Interop.Excel.Range)ws.Rows[rowCounter]).Delete(Microsoft.Office.Interop.Excel.XlDeleteShiftDirection.xlShiftUp);
                        }
                    }

                }
                catch(Exception e)
                {
                }

               



                Console.WriteLine(Path.GetDirectoryName(path));
                String fileName = Path.ChangeExtension( Path.GetFileName(path), null);
                String pathCSV = Path.GetDirectoryName(path) + "\\" + fileName + "_auxiliar.csv";
                String pathCSV_final = Path.GetDirectoryName(path) + "\\" + fileName + "_generado.csv";

                if (File.Exists(pathCSV))
                {   
                    File.Delete(pathCSV);
                }
                if (File.Exists(pathCSV_final))
                {   
                    File.Delete(pathCSV_final);
                }



                ws.SaveAs(pathCSV, Microsoft.Office.Interop.Excel.XlFileFormat.xlCSV);


                wb.Close(false, Type.Missing, Type.Missing);

                excel.Quit();

                var parser = new TextFieldParser(new StringReader(File.ReadAllText(pathCSV)))
                {
                    HasFieldsEnclosedInQuotes = true,
                    Delimiters = new string[] { "," },
                    TrimWhiteSpace = true
                };

                var csvSplitList = new List<string>();

                // Reads all fields on the current line of the CSV file and returns as a string array
                // Joins each field together with new delimiter "|"
                while (!parser.EndOfData)
                {
                    csvSplitList.Add(String.Join(";", parser.ReadFields()));
                }

                // Newline characters added to each line and flattens List<string> into single string
                String formattedCsvToSave = String.Join(Environment.NewLine, csvSplitList.Select(x => x));
                formattedCsvToSave = formattedCsvToSave.Replace(",", "");


                // Write single string to file
                File.WriteAllText(pathCSV_final, formattedCsvToSave);
                parser.Close();

                if (File.Exists(pathCSV))
                {   
                    File.Delete(pathCSV);
                }

                path = pathCSV_final;
                nuevoPatch = path;

            }
            
            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    string[] aux = reader.ReadLine().Split(';'); //Linea de titulo

                    foreach (string col in aux)
                    {
                        data.Columns.Add(col);
                    }

                    while (!reader.EndOfStream)
                    {
                        aux = reader.ReadLine().Split(';');
                        if (aux.Length > 4)
                        {
                            foreach (string lnBts in lnBtsInputs)
                            {
                                if (aux[3].Equals(lnBts))
                                {
                                    if (aux[9] == "" || aux[9] == "0") //Taget desconocido
                                    {
                                        int eci_id = Convert.ToInt32(aux[14]);
                                        int cell_id = eci_id & 0b1111_1111;
                                        int enb_id = (eci_id - cell_id) >> 8;

                                        aux[11] = cell_id.ToString();
                                        aux[8] = enb_id.ToString();
                                        aux[9] = "UNKNOWN";
                                    }

                                    int indice_s_axu = 0;
                                    foreach (string s_aux in aux)
                                    {
                                        if (s_aux.Contains(","))
                                        {
                                            string[] parts = s_aux.Split(',');
                                            string ss_aux = "";
                                            foreach(string s_parts in parts)
                                            {
                                                ss_aux += s_parts;

                                            }
                                            aux[indice_s_axu] = ss_aux;
                                            int dddd = 0;
                                        }
                                        indice_s_axu += 1;
                                    }

                                    data.Rows.Add(aux);
                                    break;
                                }
                            }
                        }
                    }
                }

            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException("No se ha podido encontrar la consulta 31 en " + path);
            }

            







            


            inExports = new bool[data.Rows.Count];

        }
        /// <summary>
        /// Intenta completar la consulta 31 a partir de los exports, en caso de faltar algo
        /// </summary>
        /// <param name="pathSRAN">Direccion de el export SRAN</param>
        /// <param name="pathFL18">Direccion del export FL18</param>
        public void completeR31(string pathSRAN, string pathFL18, string pathSRAN2, bool estadividido, bool conFL18)
        {
            var emptyData = data.AsEnumerable().Where(row => (string)row[9] == "UNKNOWN").ToList();
            if (emptyData == null)
                return;

            string directionSRAN = string.Format("provider=Microsoft.ACE.OLEDB.12.0;Data Source= {0}", pathSRAN);
            string directionSRAN2 = string.Format("provider=Microsoft.ACE.OLEDB.12.0;Data Source= {0}", pathSRAN2);
            string directionFL18 = string.Format("provider=Microsoft.ACE.OLEDB.12.0;Data Source= {0}", pathFL18);

            DataTable exportData = new DataTable();

            //no se pueden hacer busquedas sql con mas de 99 OR y AND, así que el siguiente bucle hace consultas "paginadas"
            //hace una busqueda para cada 48 emplazamientos, 49 * (1OR + 1AND) + 1AND = 99
            int i = 0;
            while (i < emptyData.Count-1)
            {
                string SQLconditions = "(lnCelId = " + emptyData[0][11] + " AND lnBtsId = " + emptyData[0][8] + ")";
                for (int j = 0; j < 49 && i < emptyData.Count-1; j++)
                {
                    i++;
                    SQLconditions += " OR (lnCelId = " + emptyData[i][11] + " AND lnBtsId = " + emptyData[i][8] + ")";
                    //i++;
                }

                string SQLquerry = "SELECT name, lnCelId, lnBtsId "
                    + "FROM A_LTE_MRBTS_LNBTS_LNCEL "
                    + "WHERE  " + SQLconditions + ";";



                if (estadividido && conFL18)
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
                                exportData.Load(SRANreader);
                            }
                            connectionSRAN2.Open();
                            using (OleDbDataReader SRANreader2 = commandSRAN2.ExecuteReader())
                            {
                                exportData.Load(SRANreader2);
                            }

                            connectionFL18.Open();
                            using (OleDbDataReader FL18reader = commandFL18.ExecuteReader())
                            {
                                exportData.Load(FL18reader);
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
                //si srandivididor y no FL18
                else if(estadividido && !conFL18)
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
                                exportData.Load(SRANreader);
                            }
                            connectionSRAN2.Open();
                            using (OleDbDataReader SRANreader2 = commandSRAN2.ExecuteReader())
                            {
                                exportData.Load(SRANreader2);
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

                //si no esta dividio y no tiene FL18
                else if (!estadividido && !conFL18)
                {

                    using (OleDbConnection connectionSRAN = new OleDbConnection(directionSRAN))
                    {
                        OleDbCommand commandSRAN = new OleDbCommand(SQLquerry, connectionSRAN);

                        //se cargan los resultados de la base de datos en una unica tabla

                        try
                        {
                            connectionSRAN.Open();
                            using (OleDbDataReader SRANreader = commandSRAN.ExecuteReader())
                            {
                                exportData.Load(SRANreader);
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
                            connectionSRAN.Open();
                            using (OleDbDataReader SRANreader = commandSRAN.ExecuteReader())
                            {
                                exportData.Load(SRANreader);
                            }

                            connectionFL18.Open();
                            using (OleDbDataReader FL18reader = commandFL18.ExecuteReader())
                            {
                                exportData.Load(FL18reader);
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
            /*busca en el dataset cada fila que se corresponde a un hueco vacío y lo rellena en la tabla
                ENB_CO_CORTE_INGLES_TEJARES_02
                ENB_H_ANDEVALO_SUR_01
                ENB_J_LA_GARZA_GOLF_01
                */


            foreach (DataRow dataRow in emptyData)
            {
                foreach(DataRow exportRow in exportData.Rows)
                {
                    if ((string)dataRow[8] == exportRow[2].ToString() && (string)dataRow[11] == exportRow[1].ToString()) // si conincide enbid y cell id 
                    {
                        dataRow[9] = exportRow[0]; //se rellena el nombre 
                    }
                }
            }


            
        }

        public List<DataRow> NotInExports()
        {
            List<DataRow> linesNotInExports = new List<DataRow>();
            for (int i = 0; i < data.Rows.Count; i++)
            {
                if (!inExports[i])
                    linesNotInExports.Add(data.Rows[i]);
            }
            return linesNotInExports;
        }



        public static DataTable ConvertExcelToDataTable(string FileName)
        {
            DataTable dtResult = null;
            int totalSheet = 0; //No of sheets on excel file  

            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
           // Excel.Workbook newWorkbook = excelApp.Workbooks.Add();
            excelApp.Visible = true;
            string workbookPath = FileName;
            Microsoft.Office.Interop.Excel.Workbook excelWorkbook = excelApp.Workbooks.Open(workbookPath,
                    0, false, 5, "", "", false, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "",
                    true, false, 0, true, false, false);
            Microsoft.Office.Interop.Excel.Sheets excelSheets = excelWorkbook.Worksheets;
            Microsoft.Office.Interop.Excel.Worksheet excelWorksheet = (Microsoft.Office.Interop.Excel.Worksheet)excelApp.Worksheets[1];



            int lastRow = excelWorksheet.Cells.SpecialCells(Microsoft.Office.Interop.Excel.XlCellType.xlCellTypeLastCell, Type.Missing).Row;

            Microsoft.Office.Interop.Excel.Range xlRange = (Microsoft.Office.Interop.Excel.Range)excelWorksheet.Range[excelWorksheet.Cells[1, 6], excelWorksheet.Cells[lastRow, 28]];

            xlRange.NumberFormat = "General";



            


            excelWorkbook.Close();
            excelApp.Quit();


            using (OleDbConnection objConn = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + FileName + ";Extended Properties='Excel 12.0;HDR=YES;IMEX=1;';"))
            {
                objConn.Open();
                OleDbCommand cmd = new OleDbCommand();
                OleDbDataAdapter oleda = new OleDbDataAdapter();
                DataSet ds = new DataSet();
                DataTable dt = objConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                string sheetName = string.Empty;
                if (dt != null)
                {
                    var tempDataTable = (from dataRow in dt.AsEnumerable()
                                         where !dataRow["TABLE_NAME"].ToString().Contains("FilterDatabase")
                                         select dataRow).CopyToDataTable();
                    dt = tempDataTable;
                    totalSheet = dt.Rows.Count;
                    sheetName = dt.Rows[0]["TABLE_NAME"].ToString();
                }

                



                cmd.Connection = objConn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT * FROM [" + sheetName + "]";
                oleda = new OleDbDataAdapter(cmd);
                oleda.Fill(ds, "excelData");
                dtResult = ds.Tables["excelData"];
                objConn.Close();






                return dtResult; //Returning Dattable  

            }
        }








    }
}