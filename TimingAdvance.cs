using System.IO;
using System.Collections.Generic;
using System;

public class TimingAdvance : FileReader
{
    public List<String[]> data; 
    public TimingAdvance(string[] lnBtsInputs) : base("Archivo CSV (*.csv)|*.csv")
    {
        data = new List<String[]>();
        using(StreamReader reader = new StreamReader(this.path))
        {
            data.Add(reader.ReadLine().Split(';')); //Linea de titulo, necesaria ya que en ese titulo trae la distancia
            while(!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] aux = line.Split(';');

                //Se guarda cada una de la lineas del Timing Advance cuyo LNCEL sea uno de los que el usuario quiere comprobar 
                foreach(string lnBts in lnBtsInputs)
                {
                    if(aux[2].Equals(lnBts))
                    {
                        data.Add(aux);
                        break;
                    }
                }
            }
        }
    }

}

