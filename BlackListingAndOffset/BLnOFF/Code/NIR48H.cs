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
        private readonly string[] dataColumns = { "PERIOD_START_TIME", "LNBTS name", "LNCEL name","Tecnologia", "Intentos Inter", "% Exitos Inter" , "Exitos HO INTER" , "Errores HO INTER", "Intentos Intra", "% Exitos Intra", "Exitos HO INTRA", "Errores HO INTRA" };
        private readonly Type[] dataTypes = { typeof(string), typeof(string), typeof(string), typeof(string), typeof(int), typeof(double), typeof(double), typeof(double), typeof(int), typeof(double), typeof(double), typeof(double) };
        public DataTable data;

        private readonly string[] errorColumns = {"Fecha", "LNBTS name", "Tecnologia", "Intentos Inter", "Exitos Inter", "% Exitos Inter", "Errores Inter","Mejorar Inter", "Intentos Intra", "Exitos Intra", "%Exitos Intra", "Errores Intra", "Mejorar Intra","Intentos INTER+INTRA", "% Exitos INTER+INTRA", "Errores INTER+INTRA"};
        private readonly Type[] types = { typeof(string), typeof(string), typeof(string), typeof(int), typeof(int), typeof(double), typeof(int), typeof(int), typeof(int), typeof(int), typeof(double), typeof(int), typeof(int), typeof(int), typeof(double),typeof(int)};
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

                                    line[0] = aux[0]; //Fecha
                                    line[1] = aux[2]; //Lnbts
                                    line[2] = aux[3]; //Lncel
                                    line[3] = TECH_NUM.GetName(TECH_NUM.GetTechFromLNCEL(aux[3])); //Tecnología

                                    double? value;
                                    Conversion.ToDouble(aux[11],out value); //Intentos Inter
                                    line[4] = value;

                                    Conversion.ToDouble(aux[12], out value); //% Exitos Inter
                                    line[5] = value;

                                    if (line[4] != null && line[5] != null) 
                                    {
                                        line[6] = Math.Round((double)line[5] * (double)line[4] / 100.0, 0); //Exito Inter
                                        line[7] = (double)line[5] - (double)line[6]; // Errores Inter
                                    }

                                    Conversion.ToDouble(aux[9], out value); //Intentos Intra
                                    line[8] = value;

                                    Conversion.ToDouble(aux[10], out value); //Intentos Intra
                                    line[9] = value;

                                    if (line[9] != null && line[8] != null) 
                                    {
                                        line[10] = Math.Round((double)line[8] * (double)line[9] / 100.0, 0); //Exito Intra
                                        line[11] = (double)line[9] - (double)line[10]; // Errores Intra
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
                throw new FileNotFoundException("No se ha podido encontrar la NIR 48h en " + path);
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

                for (byte tech_i = 0; tech_i < 5; tech_i++) // para cada nodo y fecha se saca una linea para cada tecnologia que tenga datos
                {
                    double successInter = 0;
                    double errInter = 0;
                    double err2ImproveInter = 0;

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

    
                    
                    object[] aux = new object[16] { data.Rows[i-1]["PERIOD_START_TIME"], data.Rows[i-1]["LNBTS name"], TECH_NUM.GetName(tech_i), currentData[tech_i, 0], currentData[tech_i, 1], successInter, errInter, err2ImproveInter, currentData[tech_i, 2], currentData[tech_i,3], successIntra, errIntra, err2ImproveIntra, currentData[tech_i, 0] + currentData[tech_i, 2], (currentData[tech_i, 1] + currentData[tech_i, 3]) / (currentData[tech_i, 0] + currentData [tech_i,2]) * 100 , errInterPlusIntra};
                    errors.Rows.Add(aux);

                }
            }
        }
    }
}
