using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackListingAndOffset.resources
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
        /// <summary>
        /// Encuentra la coleccion de filas que no intersectan entre la tabla propia y una externa 
        /// </summary>
        /// <param name="intersectingTable"> tabla con la que se compara</param>
        /// <param name="thisColumn">columna de la tabla propia en la que se buscan coincidencias</param>
        /// <param name="intersectingColumn">columna de la tabla de comparacion con la que se buscan las coincidencias</param>
        /// <returns></returns>
        public DataRow[] NotIntersectingWithThis(GenericTable intersectingTable, string thisColumn, string intersectingColumn)
        {
            String[] columValues = intersectingTable.GetColumn(intersectingColumn);
            string query = "";
            for(int i = 0; i<columValues.Length;i++)
            {
                query += intersectingTable.data.Columns[intersectingColumn] + " <>  " + columValues[i] + " AND ";
            }
            return data.Select(query);
        }
    }
}
