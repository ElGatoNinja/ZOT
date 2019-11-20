using System.IO;
using System.Collections.Generic;
using System;

public class RSLTE31 : FileReader
{
    public List<String[]> data; 
    public RSLTE31(string[] lnBtsInputs) : base("Archivo CSV (*.csv)|*.csv","SElecciona la consulta RSLTE31")
    {

        #if DEBUG
            _path[0] = "C:\\Users\\IS2-G-21\\Desktop\\R31.csv";
        #endif

        this.data = new List<String[]>();
        
        using(StreamReader reader = new StreamReader(_path[0]))
        {
            data.Add(reader.ReadLine().Split(';')); //Linea de titulo, necesaria ya que en ese titulo trae la distancia
            while(!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] aux = line.Split(';');

                foreach(string lnBts in lnBtsInputs)
                {
                    if(aux[3].Equals(lnBts))
                    {
                        data.Add(aux);
                        break;
                    }
                }
            }
        }
    }

}