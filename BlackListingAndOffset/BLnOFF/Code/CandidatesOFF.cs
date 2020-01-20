using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZOT.BLnOFF.Code
{
    class CandidatesOFF
    {
        public DataTable data;
        private readonly string[] colNames = { "Label", "ENBID SOURCE", "LnCell SOURCE", "Name SOURCE", "ENBID TARGET", "LnCell TARGET", "Name TARGET", "Distance", "HO Success(%)", "Offset", "HO Attempts", "Blacklist", "HO errores SR", "Num colindancias", "Cols ya en OFF", "Cols disp OFF", "SelectedOFF", "CandidataBL" };
        private readonly System.Type[] colType = { typeof(string), typeof(string), typeof(int), typeof(string), typeof(string), typeof(int), typeof(string), typeof(double), typeof(double), typeof(double), typeof(int), typeof(double), typeof(double), typeof(int), typeof(int), typeof(int), typeof(bool), typeof(bool) };

        public CandidatesOFF(TimingAdvance TA, Colindancias colin,CandidatesBL candBL)
        {
            data = new DataTable();
            for (int i = 0; i < colNames.Length; i++)
            {
                data.Columns.Add(colNames[i], colType[i]);
            }

            foreach (DataRow row in colin.data.Rows)
            {
                int tech_num = row.Field<int>("LnCell SOURCE") % 10; //siendo las unidades el numero de LTE (decenas el sector) 
                if ((double)row["Distance"] >= TA.radioLines.Select("[LNCELL] = '" + (string)row["Name SOURCE"] + "'")[0].Field<double>("RADIO") && (row["HO Success(%)"] != DBNull.Value) && (double)row["HO Success(%)"] < CONSTANTS.OFF.MIN_SUCCESS_HANDOVER)
                {
                    //numero de colindancias totales para este LnCell ID en este emplazamiento concreto (se hace por fuerza bruta, queda como ejercicio para un futuro becario optimizar esto un poco, si hiciera falta)
                    int num_colin = 0;
                    int num_colsInOFF = 0;
                    for (int i = 0; i < colin.data.Rows.Count; i++)
                    {
                        if ((int)colin.data.Rows[i]["LnCell SOURCE"] == (int)row["LnCell SOURCE"] && (string)colin.data.Rows[i]["ENBID SOURCE"] == (string)row["ENBID SOURCE"])
                        {
                            num_colin++;
                            if (colin.data.Rows[i]["Offset"] != DBNull.Value && (int)colin.data.Rows[i]["Offset"] < 15)
                                num_colsInOFF++;
                        }

                    }
                    //se calcula el numero de lineas que se pueden meter en BL usando el criterio mas restrictivo 
                    int maxDispOFF = (int)CONSTANTS.OFF.MAX_COLIN[tech_num] - num_colsInOFF;
                    int criteria2 = (int)(num_colin * CONSTANTS.OFF.MAX_PER_COLIN[tech_num] / 100.0) - num_colsInOFF;
                    if (criteria2 < maxDispOFF)
                        maxDispOFF = criteria2;

                    object[] candidateRow = new object[16] { row[0], row[1], row[2], row[3], row[4], row[5], row[6], row[7], row[8], row[9], row[10], row[11], row[12], num_colin, num_colsInOFF, maxDispOFF };
                    data.Rows.Add(candidateRow);
                }

            }
            DataView dt = data.DefaultView;
            dt.Sort = " [ENBID SOURCE] desc,[LnCell Source] desc,[HO errores SR] desc";
            data = dt.ToTable();

            //seleccion de lineas a las que hacer off, se marcan en verde
            int currentLnCell = (int)data.Rows[0]["LnCell Source"];
            int asignedOFF = 0;
            for (int i = 0; i < data.Rows.Count; i++)
            {
                if ((int)data.Rows[i]["LnCell Source"] == currentLnCell)
                {
                    if ((int)data.Rows[i]["Cols disp OFF"] > asignedOFF)
                    {
                        //solo aplica offset si no aplica BL 
                        DataRow[] posibleBLandOFF = candBL.data.Select("[Name SOURCE] = '" + data.Rows[i]["Name SOURCE"] + "' AND [Name TARGET] = '" + data.Rows[i]["Name Target"] + "'");
                        if (posibleBLandOFF.Length > 0)
                        {
                            if (!(bool)posibleBLandOFF[0]["SelectedBL"])
                            {
                                data.Rows[i]["SelectedOFF"] = true;
                                data.Rows[i]["CandidataBL"] = false;
                                asignedOFF++;
                            }
                            else
                            {
                                data.Rows[i]["SelectedOFF"] = false;
                                data.Rows[i]["CandidataBL"] = true;
                            }
                        }
                        else
                        {
                            data.Rows[i]["SelectedOFF"] = true;
                            data.Rows[i]["CandidataBL"] = false;
                            asignedOFF++;
                        }
                    }
                    else
                    {
                        data.Rows[i]["SelectedOFF"] = false;
                        data.Rows[i]["CandidataBL"] = false;
                    }
                }
                else
                {
                    currentLnCell = (int)data.Rows[i]["LnCell Source"];
                    asignedOFF = 0;
                    i--;
                }
            }
        }
    }
}
