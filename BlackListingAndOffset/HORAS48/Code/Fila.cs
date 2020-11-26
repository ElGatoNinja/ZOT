using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZOT.HORAS48.Code
{
    class Fila
    {
        String zona;
        String provincia;
        String codigoEmplazamiento;
        String emplazamiento;
        String tipoObra;
        String tipoTrabajo;
        String cObra;
        String fechaIntegraEncendido;
        DateTime fechacumpleLTE;
        DateTime fechacumple3G;
        DateTime fechacumple2G;
        DateTime fechacumple;

        public Fila(string zona, string provincia, string codigoEmplazamiento, string emplazamiento, string tipoObra, string tipoTrabajo, string cObra, string fechaIntegraEncendido)
        {
            this.zona = zona;
            this.provincia = provincia;
            this.codigoEmplazamiento = codigoEmplazamiento;
            this.emplazamiento = emplazamiento;
            this.tipoObra = tipoObra;
            this.tipoTrabajo = tipoTrabajo;
            this.cObra = cObra;
            this.fechaIntegraEncendido = fechaIntegraEncendido;
            this.fechacumple2G = new DateTime(1900, 1, 1);
            this.fechacumple3G = new DateTime(1900, 1, 1);
            this.fechacumpleLTE = new DateTime(1900, 1, 1);

        }

        public string Zona { get => zona; set => zona = value; }
        public string Provincia { get => provincia; set => provincia = value; }
        public string CodigoEmplazamiento { get => codigoEmplazamiento; set => codigoEmplazamiento = value; }
        public string Emplazamiento { get => emplazamiento; set => emplazamiento = value; }
        public string TipoObra { get => tipoObra; set => tipoObra = value; }
        public string TipoTrabajo { get => tipoTrabajo; set => tipoTrabajo = value; }
        public string CObra { get => cObra; set => cObra = value; }
        public string FechaIntegraEncendido { get => fechaIntegraEncendido; set => fechaIntegraEncendido = value; }
        public DateTime Fechacumple { get => fechacumple; set => fechacumple = value; }
        public DateTime FechacumpleLTE { get => fechacumpleLTE; set => fechacumpleLTE = value; }
        public DateTime Fechacumple3G { get => fechacumple3G; set => fechacumple3G = value; }
        public DateTime Fechacumple2G { get => fechacumple2G; set => fechacumple2G = value; }



        public DateTime SacarFechaCumple()
        {

            //Console.WriteLine("Comparando fechas " + this.fechacumpleLTE.ToString() + this.fechacumple3G.ToString() + this.fechacumple2G.ToString() );
            if (this.fechacumpleLTE == this.fechacumple3G && this.fechacumple3G == this.fechacumple2G)
            {
                //Console.WriteLine("Son las misma fechas");
                //son las mismas fechas
                return fechacumpleLTE;
            }
            else
            {
                DateTime[] dates = new DateTime[] { this.fechacumpleLTE, this.fechacumple3G, this.fechacumple2G };
                Array.Sort(dates);
                //Console.WriteLine("Fechas de mas vieja a mas nueva: {0}, {1}, {2}", dates[0], dates[1], dates[2]);
                return dates[2];
            }
        }



       
    }

}
