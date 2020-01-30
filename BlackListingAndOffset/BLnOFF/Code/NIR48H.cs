using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZOT.resources.ZOTlib;

namespace ZOT.BLnOFF.Code
{
    /// <summary>
    /// Mantiene y procesa datos de la NIR 48H para la herramienta de blacklisting y offset
    /// </summary>
    class NIR48H
    {
        private DataTable data;
        private readonly string[] errorColumns = {"Fecha", "LNBTS name", "Tecnologia", "Intentos Inter", "Exitos Inter", "% Exitos Inter", "Errores Inter","Mejorar Inter", "Intentos Intra", "Exitos Intra", "%Exitos Intra", "Errores Intra", "Mejorar Intra"};
        private readonly Type[] types = { typeof(string), typeof(string), typeof(string), typeof(int), typeof(int), typeof(double), typeof(int), typeof(int), typeof(int), typeof(int), typeof(double), typeof(int), typeof(int) };
        public DataTable errors;
        
        public NIR48H(string[] lnBtsInputs, string path)
        {
            data = new DataTable();
            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    //Establecer Columnas
                    string[] aux = reader.ReadLine().Split(';'); //Linea de titulo
                    for (int i = 0; i < aux.Length; i++)
                    {
                        data.Columns.Add(aux[i]);
                        if (i < 4)
                            data.Columns[aux[i]].DataType = typeof(string);
                        else
                            data.Columns[aux[i]].DataType = typeof(double);
                    }
                    data.Columns.Add("Exitos HO INTER", typeof(double));
                    data.Columns.Add("Fallos HO INTER", typeof(double));
                    data.Columns.Add("Exitos HO INTRA", typeof(double));
                    data.Columns.Add("Fallos HO INTRA", typeof(double));

                    while (!reader.EndOfStream)
                    {
                        aux = reader.ReadLine().Split(';');
                        if (aux.Length > 4)
                        {
                            foreach (string lnBts in lnBtsInputs)
                            {
                                if (aux[2].Equals(lnBts))
                                {
                                    object[] line = new object[aux.Length + 4];
                                    for (int i = 0; i < aux.Length; i++)
                                    {
                                        if (i < 4)
                                            line[i] = aux[i];
                                        else
                                        {
                                            double? value;
                                            Conversion.ToDouble(aux[i], out value);
                                            line[i] = value;
                                        }
                                    }

                                    //se inluyen algunos calculos a las 4 columnas que se han añadido manualmente
                                    if (line[9] != null && line[10] != null) //Intra
                                    {
                                        line[aux.Length + 2] = Math.Round((double)line[9] * (double)line[10] / 100.0, 0); //Exito Intra
                                        line[aux.Length + 3] = (double)line[9] - (double)line[aux.Length + 2]; // Errores Intra
                                    }
                                    if (line[11] != null && line[12] != null) //Inter
                                    {
                                        line[aux.Length] = Math.Round((double)line[12] * (double)line[11] / 100.0, 0); //Exito Inter
                                        line[aux.Length + 1] = (double)line[11] - (double)line[aux.Length]; // Errores Inter
                                    }


                                    data.Rows.Add(line);
                                    break;
                                }
                            }
                        }
                    }
                }
                ProcessData();
            }
            catch (FileNotFoundException)
            {
                WPFForms.ShowError("No se ha podido encontrar el fichero: " + path);
            }
        }


        // Analiza los errores de los datos guardados y añade las conclusiones como columnas al dataset
        private void ProcessData()
        {
            DataView dv = data.DefaultView;
            dv.Sort = "[LNBTS name] DESC, [PERIOD_START_TIME] ASC";
            data = dv.ToTable();

            errors = new DataTable();
            for(int j = 0; j<errorColumns.Length;j++)
            {
                errors.Columns.Add(errorColumns[j], types[j]);
            }

            int i = 0;
            while (i  < data.Rows.Count)
            {
                double[,] currentData = new double[5, 4]; //Una matriz que contiene una fila por cada tecnología y 4 columnas de datos( 2 inter y 2 intra)

                do
                {
                    byte tech = TECH_NUM.GetTechFromLNCEL((string)data.Rows[i]["LNCEL name"]);
                    currentData[tech, 0] += (double)data.Rows[i]["Inter X2 based HO prep"];
                    currentData[tech, 1] += (double)data.Rows[i]["Exitos HO INTER"];
                    currentData[tech, 2] += (double)data.Rows[i]["Intra HO Att"];
                    currentData[tech, 3] += (double)data.Rows[i]["Exitos HO INTRA"];
                    i++;

                } while (i < data.Rows.Count && (string)data.Rows[i]["PERIOD_START_TIME"] == (string)data.Rows[i - 1]["PERIOD_START_TIME"] && (string)data.Rows[i]["LNBTS name"] == (string)data.Rows[i - 1]["LNBTS name"]);

                for (byte tech_i = 0; tech_i < 5; tech_i++) // para cada nodo y fecha se saca una linea para cada tecnologia que tenga datos
                {
                    double perSuccessInter = 0;
                    double errInter = 0;
                    double err2ImproveInter = 0;

                    if (currentData[tech_i, 0] <= 0 && currentData[tech_i, 2] <= 0) continue; //Si no hubiera datos ni de inter ni de intra se salta la linea

                    if (currentData[tech_i, 0] > 0)
                    {
                        perSuccessInter = Math.Round(currentData[tech_i, 1] / currentData[tech_i, 0] * 100.0, 2); //Redondear
                        errInter = currentData[tech_i, 0] - currentData[tech_i, 1];
                        double MaxErr = currentData[tech_i, 0] - (CONSTANTS.U_INTER.PER[tech_i] * currentData[tech_i, 0] / 100.0);
                        err2ImproveInter = 0;
                        if (errInter > MaxErr)
                        {
                            err2ImproveInter = (errInter - Math.Round(MaxErr, 0)) + 1;
                        }
                    }

                    double perSuccessIntra = 0;
                    double errIntra = 0;
                    double err2ImproveIntra = 0;
                    if (currentData[tech_i, 2] > 0)
                    {
                        perSuccessIntra = Math.Round(currentData[tech_i, 3] / currentData[tech_i, 2] * 100, 2); //Redondear
                        errIntra = currentData[tech_i, 2] - currentData[tech_i, 3];
                        double MaxErr = currentData[tech_i, 2] - (CONSTANTS.U_INTRA.PER[tech_i] * currentData[tech_i, 2] / 100);
                        err2ImproveIntra = 0;
                        if (errIntra > MaxErr)
                        {
                            err2ImproveIntra = (errIntra - Math.Round(MaxErr, 0)) + 1;
                        }
                    }
                    object[] aux = new object[13] { data.Rows[i-1]["PERIOD_START_TIME"], data.Rows[i-1]["LNBTS name"], TECH_NUM.GetName(tech_i), currentData[tech_i, 0], currentData[tech_i, 1], perSuccessInter, errInter, err2ImproveInter, currentData[tech_i, 2], currentData[tech_i,3], perSuccessIntra, errIntra, err2ImproveIntra};
                    errors.Rows.Add(aux);
                }
                
            }
        }
    }
}
