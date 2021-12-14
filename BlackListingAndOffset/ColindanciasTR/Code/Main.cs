using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZOT.ColindanciasTR.Code
{
    class Main
    {

        String TRPath = "";
        String ExportNortePath = "";


        DataTable A_WCEL_NORTE = null;
        DataTable TRTableNorte = null;

        DataTable dtActualizacionesNorte = null;


        List<String> TRsitesector;
        List<String> errores;





        public Main(string tRPath, string exportNortePath)
        {
            TRPath = tRPath;
            ExportNortePath = exportNortePath;
            errores = new List<string>();



            dtActualizacionesNorte = new DataTable();
            dtActualizacionesNorte.Columns.Add("site-sector", typeof(String));
            dtActualizacionesNorte.Columns.Add("portadora", typeof(String));
            dtActualizacionesNorte.Columns.Add("U2100P1_Cell", typeof(String));
            dtActualizacionesNorte.Columns.Add("U2100P1_ID", typeof(String));
            dtActualizacionesNorte.Columns.Add("U2100P1_Azimuth", typeof(String));

            dtActualizacionesNorte.Columns.Add("U2100P8_Cell", typeof(String));
            dtActualizacionesNorte.Columns.Add("U2100P8_ID", typeof(String));
            dtActualizacionesNorte.Columns.Add("U2100P8_Azimuth", typeof(String));

            dtActualizacionesNorte.Columns.Add("U2100P9_Cell", typeof(String));
            dtActualizacionesNorte.Columns.Add("U2100P9_ID", typeof(String));
            dtActualizacionesNorte.Columns.Add("U2100P9_Azimuth", typeof(String));




            //comprobar si son null
            if (TRPath == "")
            {
                System.Windows.Forms.MessageBox.Show("La ruta del TR no es valida.");
            }
            if(ExportNortePath == "")
            {
                System.Windows.Forms.MessageBox.Show("La ruta del export no es valida.");
            }



            //Si todo correcto cargo los datos del TR
            TRTableNorte = new DataTable();
            TRTableNorte = cargarDatosTR(TRPath);



            //si todo es correcto cargo los datos del export en la datatable (con el metodo cargarDatosExports)
            A_WCEL_NORTE = new DataTable();
            A_WCEL_NORTE = cargarDatosExports(ExportNortePath);


            // Lista para poder movernos por los site sector
            TRsitesector = new List<string>();
            //Rellenamos recorriendo el TR
            TRsitesector = rellenaritesector(TRTableNorte);

            // Vamos recorriendo el export y tratando cada uno
            var contador_row = 0;
            foreach (DataRow dtRow in A_WCEL_NORTE.Rows)
            {
                //nos interesa comparar columna 5
                String sitesector = dtRow["sitesector"].ToString();



                //Comprobamos que existe ya
                bool exists = TRsitesector.Contains(sitesector);

                //Si existe miro si se actualizan los datos
                if (exists)
                {
                    //Sacamos la posicion donde esta
                    int index = TRsitesector.FindIndex(x => x.Equals(sitesector));



                    //Sacmos el sector
                    try
                    {
                        int sector = int.Parse(dtRow["sector"].ToString());

                    }
                    catch
                    {
                        errores.Add("El site-sector " + sitesector.ToString() + " ha fallado al sacar al sector.");
                        
                    }

                    //la portadora (p8 por ejempplo) esta en la posicion 5 la p y 6 el numero de portadora
                    //pero si por ejemplo tiene un espacio al final tengo que sumar uno a esta posicion
                    //para ello se debe limpiar el string quitando espacios al final
                    String nombre = dtRow["name"].ToString();

                    //llamamos al metodo para limpiar el final
                    String portadora = sacarNombrePortadora(nombre);

                    String cell = "";
                    String ID = "";
                    String Azimuth = "";
                    DataRow trRow = null;

                    string cid = "";
                    string lac = "";
                    string ID_Export = "";


                    //hago un switch de la portadora
                    switch (portadora)
                    {
                        //miro si p1 esta relleno
                        case "p1":
                            trRow = TRTableNorte.Rows[index];
                            cell = trRow["U2100P1_Cell"].ToString();
                            ID = trRow["U2100P1_ID"].ToString();
                            Azimuth = trRow["U2100P1_Azimuth"].ToString();

                            cid = dtRow["CId"].ToString();
                            lac = dtRow["LAC"].ToString();
                            ID_Export = lac + "-" + cid;

                            if((cell == "" && nombre != "") || (ID == "" && ID_Export != "")) {
                                //Se propone la actualizacion (+++++++TEMPORAL)
                                dtActualizacionesNorte.Rows.Add(sitesector,portadora,nombre, ID_Export, "", "","","", "","","");

                            }


                            break;


                        case "p8":
                            trRow = TRTableNorte.Rows[index];
                            cell = trRow["U2100P8_Cell"].ToString();
                            ID = trRow["U2100P8_ID"].ToString();
                            Azimuth = trRow["U2100P8_Azimuth"].ToString();

                            cid = dtRow["CId"].ToString();
                            lac = dtRow["LAC"].ToString();
                            ID_Export = lac + "-" + cid;

                            if ((cell == "" && nombre != "") || (ID == "" && ID_Export != ""))
                            {
                                //Se propone la actualizacion (+++++++TEMPORAL)
                                dtActualizacionesNorte.Rows.Add(sitesector, portadora, "", "", "", nombre, ID_Export, "", "", "", "");

                            }

                            break;


                        case "p9":
                            trRow = TRTableNorte.Rows[index];
                            cell = trRow["U2100P9_Cell"].ToString();
                            ID = trRow["U2100P9_ID"].ToString();
                            Azimuth = trRow["U2100P9_Azimuth"].ToString();

                            cid = dtRow["CId"].ToString();
                            lac = dtRow["LAC"].ToString();
                            ID_Export = lac + "-" + cid;

                            if ((cell == "" && nombre != "") || (ID == "" && ID_Export != ""))
                            {
                                //Se propone la actualizacion (+++++++TEMPORAL)
                                dtActualizacionesNorte.Rows.Add(sitesector, portadora, "", "", "", "", "", "", nombre, ID_Export, "");

                            }

                            break;

                    }


                    contador_row++;

                }


                //Si no existe se propone para meter o se mira porque no esta
                else
                {

                }



                int aadfa = 0;

            }





            int a = 0;




        }


        public DataTable cargarDatosExports(String path)
        {
            DataTable dataTablaTemp = new DataTable();


            string connString = "Provider=Microsoft.ACE.OLEDB.12.0;data source=" + path;


            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                OleDbCommand cmd = new OleDbCommand("SELECT Right([A_WCEL.LcrID],7) AS LcrId, A_WCEL.CId, A_WCEL.LAC, A_WCEL.name, Right([LcrID],7)+'-'+Right([name],1) AS [sitesector], Right([name],1) AS sector FROM A_WCEL;", conn);

                conn.Open();

                OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);

                adapter.Fill(dataTablaTemp);
            }


            return dataTablaTemp;

        }




        public DataTable cargarDatosTR(String path)
        {

            DataTable dtTemp = new DataTable();

            OleDbConnection conn = null;


            String constr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties='Excel 8.0;HDR=YES;';";

            conn = new OleDbConnection(constr);

            OleDbCommand cmd = new OleDbCommand("Select * From [NORTE$]", conn);

            conn.Open();


            OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);


            adapter.Fill(dtTemp);

            return dtTemp;




        }


        public List<String> rellenaritesector(DataTable dtTR)
        {
            List<String> listaTemp = new List<string>();

            foreach (DataRow dtRow in dtTR.Rows)
            {
                
                String sitesector = dtRow["site-sector"].ToString();
                listaTemp.Add(sitesector);


            }


                return listaTemp;

        }



        /// <summary>
        /// Limpia los espacios del final del nombre.
        /// </summary>
        /// <param name="nombre"></param>
        /// <returns>Nombre sin espacios al final.</returns>
        public string limpiarNombrePortadora(string nombre)
        {
            return nombre.TrimEnd();

        }

        /// <summary>
        /// Saca la portadora del nombre
        /// </summary>
        /// <param name="nombre"></param>
        /// <returns>La portadora del tipo px, donde x es el numero 1, 8 o 9.</returns>
        public string sacarNombrePortadora(string nombre)
        {
            //Primero quito espacios del final del nombre
            nombre = limpiarNombrePortadora(nombre);

            //Saco la posion 5 y 4
            string portadora = nombre.Substring(nombre.Length - 5, 2);

            return portadora;

        }




    }
}
