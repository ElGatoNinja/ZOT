using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using ZOT.resources;
using System.Xml;
using ZOT.resources.ZOTlib;

namespace ZOT.BLnOFF.Code
{
    class CandidatesBL 
    {
        public DataTable data;
        private readonly string[] colNames = {"SelectedBL", "Label", "ENBID SOURCE", "LnCell SOURCE", "Name SOURCE", "ENBID TARGET", "LnCell TARGET", "Name TARGET", "Distance", "HO Success(%)", "Offset", "HO Attempts", "Blacklist", "HO errores SR","Num colindancias","Cols ya en BL","Cols disp BL"};
        private readonly System.Type[] colType = { typeof(bool), typeof(string), typeof(string), typeof(int), typeof(string), typeof(string), typeof(int), typeof(string), typeof(double), typeof(double), typeof(double), typeof(int), typeof(double),typeof(double), typeof(int), typeof(int), typeof(int)};

        public CandidatesBL(Colindancias colin)
        {
            data = new DataTable();
            for(int i = 0;i<colNames.Length;i++)
            {
                data.Columns.Add(colNames[i],colType[i]);
            }

            foreach (DataRow row in colin.data.Rows)
            {
               
                int tech_num = row.Field<int>("LnCell SOURCE") % 10; //siendo las unidades el numero de LTE (decenas el sector)
                try { 
                    if (row["Distance"] != DBNull.Value && (double)row["Distance"] >= CONSTANTS.BL.MAX_DIST[tech_num] && (row["HO Success(%)"] != DBNull.Value)  && (double)row["HO Success(%)"] < CONSTANTS.BL.MIN_SUCCESS_HANDOVER) 
                    {
                        //numero de colindancias totales para este LnCell ID en este emplazamiento concreto (se hace por fuerza bruta, queda como ejercicio para un futuro becario optimizar esto un poco, si hiciera falta)
                        int num_colin = 0;
                        int num_colsInBL = 0;
                        for (int i = 0;i<colin.data.Rows.Count;i++)
                        {
                            if ((int)colin.data.Rows[i]["LnCell SOURCE"] == (int)row["LnCell SOURCE"] && (string)colin.data.Rows[i]["ENBID SOURCE"] == (string)row["ENBID SOURCE"])
                            {
                                num_colin++;
                                if (colin.data.Rows[i]["Blacklist"] != DBNull.Value && (int)colin.data.Rows[i]["Blacklist"] == 1)
                                    num_colsInBL++;
                            }
                        }
                        //se calcula el numero de lineas que se pueden meter en BL usando el criterio mas restrictivo 
                        int maxDispBL = (int)CONSTANTS.BL.MAX_COLIN[tech_num]-num_colsInBL;
                        int criteria2 = (int)(num_colin * CONSTANTS.BL.MAX_PER_COLIN[tech_num] / 100.0) - num_colsInBL;
                        if (criteria2 < maxDispBL)
                            maxDispBL = criteria2;

                        object[] candidateRow = new object[17] {false, row[0], row[1], row[2], row[3], row[4], row[5], row[6], row[7], row[8], row[9], row[10], row[11], row[12], num_colin, num_colsInBL, maxDispBL };
                        data.Rows.Add(candidateRow);
                    }
                }catch(IndexOutOfRangeException e)
                {//Con esto salta por ejemplo si tiene vacio
                    Console.WriteLine("Saltada la fila: " + row.ToString());
                }
            }
            DataView dt = data.DefaultView;
            dt.Sort = " [ENBID SOURCE] desc,[LnCell Source] desc,[HO errores SR] desc";
            data = dt.ToTable();


            try
            {
                //seleccion de lineas a las que hacer blacklisting, se marcan en verde
                int currentLnCell = (int)data.Rows[0]["LnCell Source"];
                int asignedBl = 0;
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    if ((int)data.Rows[i]["LnCell Source"] == currentLnCell)
                    {
                        if ((int)data.Rows[i]["Cols disp BL"] > asignedBl)
                        {
                            data.Rows[i]["SelectedBL"] = true;
                            asignedBl++;
                        }
                        else
                        {
                            data.Rows[i]["SelectedBL"] = false;
                        }
                    }
                    else
                    {
                        currentLnCell = (int)data.Rows[i]["LnCell Source"];
                        asignedBl = 0;
                        i--;
                    }
                }
            }
            catch (IndexOutOfRangeException e)
            {
                Console.WriteLine("No hay candidatas");
            }
        }
    }
}
