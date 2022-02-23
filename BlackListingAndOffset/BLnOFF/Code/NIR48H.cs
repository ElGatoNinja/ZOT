﻿using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZOT.resources.ZOTlib;
using ZOT.BLnOFF.GUI;

namespace ZOT.BLnOFF.Code
{
    /// <summary>
    /// Mantiene y procesa datos de la NIR 48H para la herramienta de blacklisting y offset
    /// </summary>
    class NIR48H
    {
        public HashSet<string> celdasMasmovilNIR = new HashSet<string>();


        private readonly string[] dataColumns = { "PERIOD_START_TIME", "LNBTS name", "LNCEL name","Tecnologia", "Intentos Inter", "% Exitos Inter" , "Exitos HO INTER" , "Errores HO INTER", "Intentos Intra", "% Exitos Intra", "Exitos HO INTRA", "Errores HO INTRA" };
        private readonly Type[] dataTypes = { typeof(string), typeof(string), typeof(string), typeof(string), typeof(int), typeof(double), typeof(double), typeof(double), typeof(int), typeof(double), typeof(double), typeof(double) };
        public DataTable data;

        private readonly string[] errorColumns = {"Fecha", "LNBTS name", "Tecnologia", "Intentos Inter", "Exitos Inter", "% Exitos Inter", "Errores Inter","Mejorar Inter", "Intentos Intra", "Exitos Intra", "%Exitos Intra", "Errores Intra", "Mejorar Intra","Intentos INTER+INTRA", "% Exitos INTER+INTRA", "Errores INTER+INTRA"};
        private readonly Type[] types = { typeof(string), typeof(string), typeof(string), typeof(int), typeof(int), typeof(double), typeof(int), typeof(int), typeof(int), typeof(int), typeof(double), typeof(int), typeof(int), typeof(int), typeof(double),typeof(int)};
        public DataTable errors;
        
        public NIR48H(string[] lnBtsInputs, string path)
        {
            data = new DataTable();

            //SACO SI EL FICHERO ES .xlsx
            string extension = Path.GetExtension(path);

            if (extension == ".xlsx")
            {
                celdasMasmovilNIR = new HashSet<string>();

                //Si es un xlsx se pasa a .csv
                // para ello se abre y se guarda como csv. Pero esto lo guarda con , como delimintador asi que se tiene qeu abrir el csv y cambiar las , por ;
                // despues si un numero tiene , separando los mires se cambia elimina, con .Replace(',','')

                Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
                Microsoft.Office.Interop.Excel.Workbook wb = excel.Workbooks.Open(path);
                Microsoft.Office.Interop.Excel.Worksheet ws = wb.Sheets[1];


                Microsoft.Office.Interop.Excel.Range rngFila2 = (Microsoft.Office.Interop.Excel.Range)ws.Application.Rows[2, Type.Missing];
                rngFila2.Delete(Microsoft.Office.Interop.Excel.XlDirection.xlUp);


                try
                {
                    //Para borrar celdas masmovil
                    int rows = ws.UsedRange.Rows.Count;
                    int fRowIndex = ((Microsoft.Office.Interop.Excel.Range)(ws.UsedRange)).Row;

                    for (int rowCounter = rows; rowCounter > 1; rowCounter--)
                    {
                        //Si no tiene un guion bajo la borro por ser de MasMovil
                        String s = ws.Rows.Cells[rowCounter, 4].Value;
                        if (! s.Contains('_') || s[0] == '=' || (s[0] == 'A' && s[1] == '9')){
                            celdasMasmovilNIR.Add(s);
                            ((Microsoft.Office.Interop.Excel.Range)ws.Rows[rowCounter]).Delete(Microsoft.Office.Interop.Excel.XlDeleteShiftDirection.xlShiftUp);
                        }
                    }

                }
                catch (Exception e)
                {
                }







                Console.WriteLine(Path.GetDirectoryName(path));
                String fileName = Path.ChangeExtension(Path.GetFileName(path), null);
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
                formattedCsvToSave = formattedCsvToSave.Replace(",","");
                

                // Write single string to file
                File.WriteAllText(pathCSV_final, formattedCsvToSave);
                parser.Close();

                if (File.Exists(pathCSV))
                {
                    File.Delete(pathCSV);
                }

                path = pathCSV_final;
            }

                try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    //Establecer Columnas
                    string[] aux = reader.ReadLine().Split(';'); //Linea de titulo
                    for (int i = 0; i < dataColumns.Length; i++)
                    {
                        data.Columns.Add(dataColumns[i], dataTypes[i]);
                    }

                    while (!reader.EndOfStream)
                    {
                        aux = reader.ReadLine().Split(';');


                        if (aux.Length > 4)
                        {

                            foreach (string lnBts in lnBtsInputs)
                            {


                                if (aux[2].Equals(lnBts))
                                {
                                    object[] line = new object[data.Columns.Count];

                                    line[0] = change_fecha(aux[0]); //Fecha

                                    line[1] = aux[2]; //Lnbts

                                    line[2] = aux[3].Replace("=", String.Empty); //Lncel
                                    line[3] = TECH_NUM.GetName(TECH_NUM.GetTechFromLNCEL(aux[3])); //Tecnología

                                    double? value;
                                    Conversion.ToDouble(aux[11], out value); //Intentos Inter
                                    line[4] = value;

                                    Conversion.ToDouble(aux[12], out value); // Exitos Inter
                                    line[5] = value;

                                    Conversion.ToDouble(aux[9], out value); //Intentos Intra
                                    line[8] = value;

                                    Conversion.ToDouble(aux[10], out value); //Exitos Intra
                                    line[9] = value;

                                    if (line[4] == null && line[5] == null && line[8] == null && line[9] == null)
                                    {
                                        continue;
                                    }

                                    else
                                    {

                                        if (line[4] != null && line[5] != null)
                                        {

                                            line[6] = Math.Round((double)line[5] * (double)line[4] / 100.0, 0); //Exito Inter
                                            line[7] = (double)line[5] - (double)line[6];
                                        }
                                        else
                                        {
                                            line[4] = 0;
                                            line[5] = 0;
                                            line[6] = 0;
                                            line[7] = 0;
                                        }

                                        if (line[9] != null && line[8] != null)
                                        {
                                            line[10] = Math.Round((double)line[8] * (double)line[9] / 100.0, 0); //Exito Intra
                                            line[11] = (double)line[9] - (double)line[10]; // Errores Intra
                                        }
                                        else
                                        {
                                            line[8] = 0;
                                            line[9] = 0;
                                            line[10] = 0;
                                            line[11] = 0;
                                        }



                                    }
                                    data.Rows.Add(line);

                              
                                   
                                    


                                }
                            }
                        }
                        else continue;
                        
                    }
                  
                }
                
                ProcessData();
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException("No se ha podido encontrar la NIR 48h en " + path);
            }
            catch (Exception e)
            {
               
            }
        }


        // Cambia el formato de las fechas para que se ordenen correctamente
        private String change_fecha(String fecha)
        {
            char separator = '.';
            String aux = "";
            String[] strings = fecha.Split(separator);
            aux = aux + strings[2] + "/" + strings[0] + "/" + strings[1];
            return aux;
        }


        // Analiza los errores de los datos guardados y añade las conclusiones como columnas al dataset
        private void ProcessData()
        {
            DataView dv = data.DefaultView;
            dv.Sort = "PERIOD_START_TIME ASC, [LNCEL name] ASC";
            data = dv.ToTable();

            errors = new DataTable();
            for(int j = 0; j<errorColumns.Length;j++)
            {
                errors.Columns.Add(errorColumns[j], types[j]);
            }

            int i = 0;
            while (i  < data.Rows.Count)
            {
                //PARA NUEVAS TECNOLOGIAS CAMBIAR el 6 por uno mas por cada tecnologia nueva
                double[,] currentData = new double[6, 4]; //Una matriz que contiene una fila por cada tecnología y 4 columnas de datos( 2 inter y 2 intra)

                do
                {
                    try
                    {
                        byte tech = TECH_NUM.GetTechFromLNCEL((string)data.Rows[i]["LNCEL name"]);
                        if (data.Rows[i]["Intentos Inter"] != DBNull.Value)
                            currentData[tech, 0] += (int)data.Rows[i]["Intentos Inter"];
                        if (data.Rows[i]["Exitos HO INTER"] != DBNull.Value)
                            currentData[tech, 1] += (double)data.Rows[i]["Exitos HO INTER"];
                        if (data.Rows[i]["Intentos Intra"] != DBNull.Value)
                            currentData[tech, 2] += (int)data.Rows[i]["Intentos Intra"];
                        if (data.Rows[i]["Exitos HO INTRA"] != DBNull.Value)
                            currentData[tech, 3] += (double)data.Rows[i]["Exitos HO INTRA"];
                    }
                    catch (FormatException fe)
                    {
                        Console.WriteLine("BlackListingAndOffset->AnalisisPrevio->Nir48.cs: Una de las celdas de la NIR no tiene el formato deseado. Detalles:\n " + fe.StackTrace);

                    }
                    i++;

                } while (i < data.Rows.Count && (string)data.Rows[i]["PERIOD_START_TIME"] == (string)data.Rows[i - 1]["PERIOD_START_TIME"] && (string)data.Rows[i]["LNBTS name"] == (string)data.Rows[i - 1]["LNBTS name"]);


                //PARA NUEVAS TECNOLOGIAS CAMBIAR EL 6 por uno mas por cada tecnologia nueva
                for (byte tech_i = 0; tech_i < 6; tech_i++) // para cada nodo y fecha se saca una linea para cada tecnologia que tenga datos
                {
                    double successInter = 0;
                    double errInter = 0;
                    double err2ImproveInter = 0;
                    double successInterIntra = 0;

                    if (currentData[tech_i, 0] <= 0 && currentData[tech_i, 2] <= 0) continue; //Si no hubiera datos ni de inter ni de intra se salta la linea

                    if (currentData[tech_i, 0] > 0)
                    {
                        successInter = Math.Round(currentData[tech_i, 1] / currentData[tech_i, 0] * 100.0, 2); //Redondear
                        errInter = currentData[tech_i, 0] - currentData[tech_i, 1];
                        double MaxErr = currentData[tech_i, 0] - (CONSTANTS.U_INTER.PER[tech_i] * currentData[tech_i, 0] / 100.0);
                        err2ImproveInter = 0;
                        if (errInter > MaxErr)
                        {
                            err2ImproveInter = (errInter - Math.Round(MaxErr, 0)) + 1;
                        }
                    }

                    double successIntra = 0;
                    double errIntra = 0;
                    double err2ImproveIntra = 0;
                    if (currentData[tech_i, 2] > 0)
                    {
                        successIntra = Math.Round(currentData[tech_i, 3] / currentData[tech_i, 2] * 100, 2); //Redondear
                        errIntra = currentData[tech_i, 2] - currentData[tech_i, 3];
                        double MaxErr = currentData[tech_i, 2] - (CONSTANTS.U_INTRA.PER[tech_i] * currentData[tech_i, 2] / 100);
                        err2ImproveIntra = 0;
                        if (errIntra > MaxErr)
                        {
                            err2ImproveIntra = (errIntra - Math.Round(MaxErr, 0)) + 1;
                        }
                    }


                    double errInterPlusIntra = 0;
                    errInterPlusIntra = currentData[tech_i, 2]+currentData[tech_i,0] - (currentData[tech_i, 3] + currentData[tech_i,1]);
                    successInterIntra = (currentData[tech_i, 1] + currentData[tech_i, 3]) / (currentData[tech_i, 0] + currentData[tech_i, 2]) * 100;
                    successInterIntra = Math.Round(successInterIntra, 2);



                    object[] aux = new object[16] { data.Rows[i-1]["PERIOD_START_TIME"], data.Rows[i-1]["LNBTS name"], TECH_NUM.GetName(tech_i), currentData[tech_i, 0], currentData[tech_i, 1], successInter, errInter, err2ImproveInter, currentData[tech_i, 2], currentData[tech_i,3], successIntra, errIntra, err2ImproveIntra, currentData[tech_i, 0] + currentData[tech_i, 2], successInterIntra , errInterPlusIntra};
                    errors.Rows.Add(aux);

                }
            }
        }
    }
}
