using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZOT.resources
{
    public abstract class GenericTable
    {
        public DataTable data;

        public GenericTable()
        {
            data = new DataTable();
        }

        ///<summary>Funcion que devuelve una columna de la tabla como String[] </summary>
        /// <param name="name" > El nombre de la columna que se quiere extraer </param>
        public String[] GetColumn(string name)
        {
            String[] column = new String[data.Rows.Count];
            for (int i = 0; i < data.Rows.Count; i++)
            {
                column[i] = data.Rows[i][name].ToString();
            }
            return column;
        }

        ///<summary>Funcion que devuelve una columna de la tabla como String[] </summary>
        /// <param name="index" > El indice de la columna que se quiere extraer </param>
        public String[] GetColumn(int index)
        {
            String[] column = new String[data.Rows.Count];
            for (int i = 0; i < data.Rows.Count; i++)
            {
                column[i] = data.Rows[i][index].ToString();
            }
            return column;
        }
    }
}
