using System.IO;
using System.Collections.Generic;
using System.Data;
using System;
using BlackListingAndOffset.resources;

public class TimingAdvance :GenericTable
{
    public List<Object[]> radioLines;
    private const double _OFF_TRUST = 95;
    private const byte _FIRST_VALUE_COL = 7;
    //_RADIO_TABLE_KM se corresponde con los titulos de el archivo TimingAdvance. Dios no quiera que tengas que retocar estos valores futuro becario
    private readonly double[] _RADIO_TABLE_KM = {0.78,0.156,0.312,0.468,0.624,0.780,1.092,1.404,1.794,2.262,0.5,1,1.5,2,2.7,3.4,4.1,4.8,5.6,5.6,1,2,3,4,5.3,6.9,8.6,9.5,11.1,11.1,1.5,3,4.5,6,8,10.4,12.9,14.6,16.6,16.6,3,6,9,12,16,21,26,33,33,6,12,15,18,24,32,41,52,63,63,10,20,30,40,53,69,87,105,105};
   
    public TimingAdvance(string[] lnBtsInputs,string path) 
    {
        data = new DataTable();
        data.Columns.Add("PERIOD_START_TIME",typeof(string));
        data.Columns.Add("MRBTS/SBTS name",typeof(string));
        data.Columns.Add("LNBTS name",typeof(string));
        data.Columns.Add("LNCEL name",typeof(string));
        for (int i = 4;i<75;i++)
        {
            data.Columns.Add("col" + i,typeof(double));
        }

        radioLines = new List<Object[]>();
        
        using(StreamReader reader = new StreamReader(path))
        {
            while(!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                object[] aux = (object[])line.Split(';');

                //Se guarda cada una de la lineas del Timing Advance cuyo LNCEL sea uno de los que el usuario quiere comprobar 
                foreach(string lnBts in lnBtsInputs)
                {
                    if(aux[2].Equals(lnBts))
                    {
                        for(int i = 0;i< 75;i++) //hay que eliminar los ""
                        {
                            if (aux[i].Equals("")) 
                                aux[i] = "0";
                        }
                        data.Rows.Add(aux);
                    }

                }
            }
        }
        GetRadioLines();
    }


    /* funcion que calcula el radio de efecto de las lineas en funcion de el factor constante _OFF_TRUST
     * a partir de la información extraida del TimingAdvance*/
    private void GetRadioLines()
    {
        for (int j = 0; j < data.Rows.Count; j++)
        {
            double accumulatedValue = 0;
            int i = _FIRST_VALUE_COL;
            while (accumulatedValue < _OFF_TRUST)
            {
                accumulatedValue +=(double)data.Rows[j][i];
                i++;
            }
            Object[] line = new Object[2];
            line[0] = data.Rows[j]["LNCEL name"];
            line[1] = _RADIO_TABLE_KM[_FIRST_VALUE_COL+i];
            radioLines.Add(line);
        }
    }

   
}

