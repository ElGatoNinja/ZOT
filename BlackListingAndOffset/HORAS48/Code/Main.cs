using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZOT.HORAS48.GUI;
using Excel = Microsoft.Office.Interop.Excel;


namespace ZOT.HORAS48.Code
{
    class Main
    {
        //Variable globales
        HashSet<string> vNpo = new HashSet<string>();
        HashSet<string> vNpoTodos = new HashSet<string>();
        Dictionary<string,string> borradosPorque = new Dictionary<string, string>();
        Dictionary<string,string> sinEstadoenEjecucion = new Dictionary<string, string>();

        public Dictionary<string, string> BorradosPorque { get => borradosPorque; set => borradosPorque = value; }
        public Dictionary<string, string> SinEstadoenEjecucion { get => sinEstadoenEjecucion; set => sinEstadoenEjecucion = value; }

        public string RellenarCeldaTabla(string tipo)
        {
            Console.WriteLine("Se encontro la celda en tabla con estado vacio o distinto de INTEGRACION, ENCENDIDO o NO");
            string estado = "";

            TabalRelleno tablarelleno = new TabalRelleno(tipo);
            DialogResult diares = tablarelleno.ShowDialog();
            if(diares == DialogResult.OK)
            {
                estado = tablarelleno.respuestaComboBox();
            }
            else
            {
                if(diares == DialogResult.Cancel)
                {
                    //Console.WriteLine("El usuario cancelo");
                    tablarelleno.Close();
                }
            }

            return estado;
        }


        public List<Fila> sacarLista(string ruta, string zona)
        {
            

            OleDbConnection conNorte = null;
            OleDbConnection conTabla = null;
            List<Fila> tratados = null;


            try { 
            
                // 1.Sacamos la tabla de relaciones
                
            string pathTabla = Path.Combine(Environment.CurrentDirectory, @"Data\", "Tabla.xlsx");

            String constrTabla = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                               pathTabla +
                                ";Extended Properties='Excel 8.0;HDR=YES;';";

                conTabla = new OleDbConnection(constrTabla);



                String nameTabla = "TABLA";
                OleDbCommand oconnTabla = new OleDbCommand("Select * From[" + nameTabla + "$]", conTabla);
            

                OleDbDataAdapter sdaTabla = new OleDbDataAdapter(oconnTabla);













                // 2. Sacamos los que tengan el COC en resumen - 2G - 3G - LTE
                String constrNorte = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                            ruta +
                            ";Extended Properties='Excel 8.0;HDR=YES;';";

                conNorte = new OleDbConnection(constrNorte);

                conNorte.Open();


               // Console.WriteLine("MOSTRANDO NOMBRES DE LAS TABLAS");
                DataTable dtSheets = null;
                dtSheets = conNorte.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                String[] excelSheets2 = new String[dtSheets.Rows.Count];
                int numbersheets = 0;
                foreach (DataRow row in dtSheets.Rows)
                {
                    excelSheets2[numbersheets] = row["TABLE_NAME"].ToString();
                    
                    numbersheets++;
                    
                }

                /*
            * Para que no se modifiquen los nombres de las hojas se ha decidido sacarlos por su orden, vienen dados por orden alfabetico por eso estan asi
            * 3G_48_H_OK - 1
            * 2G 48 H OK  - 0
            * LTE_48_H_OK - 10
            * RESUMEN - 11
             */
                // ORDENES DE LAS HOJAS
                int s3G = 1;
                int s2G = 0;
                int sLTE = 10;
                int sresumen = 11;

                /*Console.WriteLine(excelSheets2[s2G]);
                Console.WriteLine(excelSheets2[s3G]);
                Console.WriteLine(excelSheets2[sLTE]);
                Console.WriteLine(excelSheets2[sresumen]);
                */



                //    1. Preparamos la consulta para los que no estan en Resumen
                String tablaResumen = "RESUMEN";
                String tabla2g = "2G 48 H OK ";
                String tabla3g = "3G 48 H OK ";
                String tablaLTE = "LTE 48 H OK";




                //Consulta para Norte2G   
                //OleDbCommand oconnNorte2 = new OleDbCommand("Select * From ["+ excelSheets2[s2G] + "] WHERE COC LIKE '20%' AND COC NOT IN(SELECT COC FROM[" + tablaResumen + "$])  AND (`TIPO TRABAJO` = ? OR `TIPO TRABAJO` = ? OR `TIPO TRABAJO` = ? )", conNorte);
                OleDbCommand oconnNorte2 = new OleDbCommand("Select * From [" + tabla2g + "$] WHERE COC LIKE '20%' AND COC NOT IN(SELECT COC FROM[" + tablaResumen + "$])  AND (`TIPO TRABAJO` = ? OR `TIPO TRABAJO` = ? OR `TIPO TRABAJO` = ? )", conNorte);
                oconnNorte2.Parameters.Add("TIPO TRABAJO", OleDbType.VarChar, 80).Value = "ENCENDIDO";
                oconnNorte2.Parameters.Add("TIPO TRABAJO", OleDbType.VarChar, 80).Value = "INTEGRACION";
                oconnNorte2.Parameters.Add("TIPO TRABAJO", OleDbType.VarChar, 80).Value = "INTEGRACION+e";





                //Consulta para Norte3G 
                OleDbCommand oconnNorte3 = new OleDbCommand("Select * From[" + tabla3g + "$] WHERE COC LIKE '20%' AND COC NOT IN(SELECT COC FROM[" + tablaResumen + "$])  AND  (`TIPO TRABAJO` = ? OR `TIPO TRABAJO` = ? OR `TIPO TRABAJO` = ?) ", conNorte);
                oconnNorte3.Parameters.Add("TIPO TRABAJO", OleDbType.VarChar, 80).Value = "ENCENDIDO";
                oconnNorte3.Parameters.Add("TIPO TRABAJO", OleDbType.VarChar, 80).Value = "INTEGRACION";
                oconnNorte3.Parameters.Add("TIPO TRABAJO", OleDbType.VarChar, 80).Value = "INTEGRACION+e";



                //Consulta para NorteLTG
                OleDbCommand oconnNorteLTE = new OleDbCommand("Select * From[" + tablaLTE + "$] WHERE COC LIKE '20%' AND  COC NOT IN(SELECT COC FROM[" + tablaResumen + "$])  AND  (`TIPO TRABAJO` = ? OR `TIPO TRABAJO` = ? OR `TIPO TRABAJO` = ?) ", conNorte);
                oconnNorteLTE.Parameters.Add("TIPO TRABAJO", OleDbType.VarChar, 80).Value = "ENCENDIDO";
                oconnNorteLTE.Parameters.Add("TIPO TRABAJO", OleDbType.VarChar, 80).Value = "INTEGRACION";
                oconnNorteLTE.Parameters.Add("TIPO TRABAJO", OleDbType.VarChar, 80).Value = "INTEGRACION+e";






                // 2. Creamos un dataset donde metemos todo lo de NORTE


                // Se pintan en las Tablas
                OleDbDataAdapter sdaNorte2 = new OleDbDataAdapter(oconnNorte2);
                OleDbDataAdapter sdaNorte3 = new OleDbDataAdapter(oconnNorte3);
                OleDbDataAdapter sdaNorteLTE = new OleDbDataAdapter(oconnNorteLTE);

                DataSet dsNorte = new DataSet("Norte"); //Contendra varias tablas



            Console.WriteLine("En Select 3G " + zona);
            sdaNorte3.Fill(dsNorte, "3G_48_H_OK");

            Console.WriteLine("En Select LTE " + zona);
            sdaNorteLTE.Fill(dsNorte, "LTE_48_H_OK");


            Console.WriteLine("En Select 2G " + zona);
            sdaNorte2.Fill(dsNorte, "2G_48_H_OK");








                /*
                 * Verificado que vNpo lee correctamente los que tienen fecha
                 * 
                 */
                // 3.Se junta la tabla de norte 3G con la de los tipo de obra
                sdaTabla.Fill(dsNorte, "Tabla");
                

                var dt = new DataTable();
                dt = dsNorte.Tables["Tabla"];


            //Recorro la tabla y saco los tipos de obra que ya existen
            Console.WriteLine("Recorriendo la tabla");

            List<String> listTablaTipos = new List<string>();
            foreach (DataRow pRow in dsNorte.Tables["Tabla"].Rows)
            {
                String tipo = pRow["TIPO"].ToString();
                if (!listTablaTipos.Contains(tipo))
                {
                    listTablaTipos.Add(tipo);
                }
            }









            Console.WriteLine("2G BORRANDO+++++++++++++++++++++");
                var watch = new System.Diagnostics.Stopwatch();
                watch.Start();


                //Sie sta en vNpo lo borro
                foreach (DataRow pRow in dsNorte.Tables["2G_48_H_OK"].Rows)
                {
                    bool borrada = false;

                    if (!vNpoTodos.Contains(pRow["COC"].ToString()))
                    {
                        Console.WriteLine("No esta en vNpo y la borro");
                    if (!borradosPorque.ContainsKey(pRow["COC"].ToString()))
                    {

                        borradosPorque.Add(pRow["COC"].ToString(), "No esta en vNpo - " + zona);
                    }
                        pRow.Delete();
                        borrada = true;
                    }
                    if (!borrada) {
                        if (vNpo.Contains(pRow["COC"].ToString() ))
                        {
                        //if (!borradosPorque.ContainsKey(pRow["COC"].ToString())) { borradosPorque.Add(pRow["COC"].ToString(), "Con fecha en vNpo 2G " + zona); }
                        
                        pRow.Delete();
                        }
                    }
                
                }
                dsNorte.Tables["2G_48_H_OK"].AcceptChanges();

                watch.Stop();
                Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");







                Console.WriteLine("3G BORRANDO+++++++++++++++++++++");
                var watch3 = new System.Diagnostics.Stopwatch();
                watch3.Start();


                foreach (DataRow pRow in dsNorte.Tables["3G_48_H_OK"].Rows)
                {
                    bool borrada = false;

                    if (!vNpoTodos.Contains(pRow["COC"].ToString()))
                    {
                        Console.WriteLine("No esta en vNpo y la borro");
                        if (!borradosPorque.ContainsKey(pRow["COC"].ToString()))
                        {
                             borradosPorque.Add(pRow["COC"].ToString(), "No esta en vNpo - " + zona);
                        }
                        
                        pRow.Delete();
                        borrada = true;
                    }
                    if (!borrada)
                    {
                        if (vNpo.Contains(pRow["COC"].ToString()))
                        {

                            //if (!borradosPorque.ContainsKey(pRow["COC"].ToString())){borradosPorque.Add(pRow["COC"].ToString(), "Con fecha en vNpo 3G " + zona);}
                        
                            pRow.Delete();
                        }
                    }


            }
                dsNorte.Tables["3G_48_H_OK"].AcceptChanges();

                watch3.Stop();
                Console.WriteLine($"Execution Time: {watch3.ElapsedMilliseconds} ms");





                Console.WriteLine("LTE BORRANDO+++++++++++++++++++++");
                var watchL = new System.Diagnostics.Stopwatch();
                watchL.Start();


                foreach (DataRow pRow in dsNorte.Tables["LTE_48_H_OK"].Rows)
                {
                    bool borrada = false;

                    if (!vNpoTodos.Contains(pRow["COC"].ToString()))
                    {
                        Console.WriteLine("No esta en vNpo y la borro");
                        if (!borradosPorque.ContainsKey(pRow["COC"].ToString()))
                        {
                            borradosPorque.Add(pRow["COC"].ToString(), "No esta en vNpo - " + zona);
                        }   
                    
                        pRow.Delete();
                        borrada = true;
                    }
                    if (!borrada)
                    {
                        if (vNpo.Contains(pRow["COC"].ToString()))
                        {
                            //if (!borradosPorque.ContainsKey(pRow["COC"].ToString())) {borradosPorque.Add(pRow["COC"].ToString(), "Con fecha en vNpo LTE " + zona);}
                            pRow.Delete();
                        }
                    }


            }
                dsNorte.Tables["LTE_48_H_OK"].AcceptChanges();

                watchL.Stop();
                Console.WriteLine($"Execution Time: {watchL.ElapsedMilliseconds} ms");



                Console.WriteLine("FIN DEL BORRADO");

                //Pintamos la tabla con los nuevos datos
                DataTable dtNorte = new DataTable();//Una tabla que se saca del data Set
                dtNorte = dsNorte.Tables["2G_48_H_OK"];
                //grid_items_norte2.DataSource = dtNorte;




                /*
                 * 
                 * TESTEO PARA PODER FILTRAR LOS DATOS
                 * 
                 * 
                var data = new List<String>();
                foreach (DataRow pRow in dsNorte.Tables["LTE_48_H_OK"].Rows)
                {
                    data.Add(pRow["COC"].ToString());
                }
                Console.WriteLine("+++++++++++++++++++++++++++++++++++++++");
                advancedDataGridView1.DataSource = data;

                */



                dtNorte = dsNorte.Tables["3G_48_H_OK"];
                //grid_items_norte3.DataSource = dtNorte;
                dtNorte = dsNorte.Tables["LTE_48_H_OK"];
                //grid_items_norteLTE.DataSource = dtNorte;







                /*
                 * Se prepara la TABLA para comparas
                 */
                // Primero si esta vacio un campo ESTADO SE RELLENA
                /*
                * CASOS ESPECIALES
                */
                //Si en tabla esta vacio el ESTADO se pide que se rellene  
                
                var mapCambios = new Dictionary<string, string>();
                bool haycambios = false;

                foreach (DataRow tRow in dsNorte.Tables["Tabla"].Rows)
                {
                    if (tRow["ESTADO"].ToString() != "NO" && tRow["ESTADO"].ToString() != "INTEGRACION" && tRow["ESTADO"].ToString() != "ENCENDIDO")
                    {

                        string estadonuevo = RellenarCeldaTabla(tRow["TIPO"].ToString());
                        if (estadonuevo != "")
                        {
                            mapCambios.Add(tRow["TIPO"].ToString(), estadonuevo);
                            haycambios = true;
                            //Lo modifico en la tabla
                            tRow["ESTADO"] = estadonuevo;
                        }
                        else { Console.WriteLine("No hay cambios para " + tRow["TIPO"].ToString()); }
                    }
                }

                dsNorte.Tables["Tabla"].AcceptChanges();




            /*




            
           


            //Sacar los que esten en la tabla
            /*
             * 
             * Recorre la tabla de 2G:
             *      Se recorre la tabla de Tabla para comparar
             *          -Si la fila tiene la misma descripcion que sale en Fila --> se mira si se borra o no
             *          
             *              -Si no esta en Tabla el Tipo lo añado para indicar que hay que ejecutar la herramienta adicional
             *          
             *              -Si tiene estado NO se borra la fila y se sale del bucle de la tabla
             *              
             *              -Si es de tipo integracion+e se borra, pero antes se miro si era no
             *              
             *              -Si Tipo trabajo no coincide con el de tabla, es decir, en un lado ENCENDIDO y en otro INTEGRACION se borra
             *              
             *              -Si Tipo trabajo coincide con el de tabla, es decir, en un lado ENCENDIDO y en otro ENCENDIDO no se borra y se sale del bucle para esa fila
             */

            Console.WriteLine("+++++++ COMPARNDO CON TABLA LOS QUE SE QUEDAN +++++++++");
                //Se recorre la tabla 2G
                foreach (DataRow pRow in dsNorte.Tables["2G_48_H_OK"].Rows)
                {

                    //Si la tabla con los tipos no tiene esa DESCRIPCION OBRA la meto en la lista para informar
                    if (!listTablaTipos.Contains(pRow["DESCRIPCION OBRA"]))
                    {
                        //Si no lo añadi ya lo añado
                        if (!sinEstadoenEjecucion.ContainsKey(pRow["DESCRIPCION OBRA"].ToString()))
                        {
                            Console.WriteLine("No esta en Tabla: " + pRow["DESCRIPCION OBRA"].ToString());
                            sinEstadoenEjecucion.Add(pRow["DESCRIPCION OBRA"].ToString(), "");
                        }
                        

                    }


                //Console.WriteLine("EN 2G --> " + pRow["DESCRIPCION"].ToString() + " +++++ " + pRow["TIPO TRABAJO"].ToString());
                foreach (DataRow tRow in dsNorte.Tables["Tabla"].Rows)
                    {
                    //SI coinciden las descripcione verifico que se hace
                    if ((pRow["DESCRIPCION OBRA"].ToString() == tRow["TIPO"].ToString()))
                    {

                        //Console.WriteLine("     COINCIDE --> " + pRow["DESCRIPCION"].ToString() + " +++++ " + pRow["TIPO TRABAJO"].ToString() + " ------- " +  pRow["DESCRIPCION"].ToString() + " +++++ " + pRow["TIPO TRABAJO"].ToString());



                        //0 - Si ESTADO = NO entonces la borro
                        if (tRow["ESTADO"].ToString() == "NO")
                        {
                            //Console.WriteLine("Borrado porque descripcion es: " + pRow["DESCRIPCION OBRA"].ToString() + " +++++ y tabla tiene  " + tRow["TIPO"].ToString() + " ------- " + tRow["ESTADO"].ToString() + " +++++ " + pRow["TIPO TRABAJO"].ToString());

                            pRow.Delete();

                            //                          Console.WriteLine("Borrado porque tiene un NO");
                            break;
                        }
                        //Si es de integracion+e me lo quedo, pero ya mire que no fuera un NO
                        if (pRow["TIPO TRABAJO"].ToString() == "INTEGRACION+e"){
                            //Console.WriteLine("Es de integracion+e " + pRow["TIPO TRABAJO"].ToString());
                            break;
                        }



                            //Si el estado no coincide lo borro
                            if (pRow["TIPO TRABAJO"].ToString() != tRow["ESTADO"].ToString())
                            {
                                //Console.WriteLine("Borrado porque no coincide ****" + pRow["TIPO TRABAJO"].ToString() + " --" + tRow["ESTADO"].ToString());

                                pRow.Delete();

                                break;
                            }
                            //Si el estado coincide dejo de tratarla porque me quedo con esa fila y no pierdo tiempo
                            if (pRow["TIPO TRABAJO"].ToString() == tRow["ESTADO"].ToString())
                            {
                                //Console.WriteLine("NO BORRADO porque coincide ****" + pRow["TIPO TRABAJO"].ToString() + " --" + tRow["ESTADO"].ToString());

                                break;
                            }
                            


                        }
                        

                    }


                }

                dsNorte.Tables["2G_48_H_OK"].AcceptChanges();
                Console.WriteLine("REPINTANDO LA TABLA 2G");
                dtNorte = dsNorte.Tables["2G_48_H_OK"];
                //grid_items_norte2.DataSource = dtNorte;






                Console.WriteLine("+++++++ COMPARNDO CON TABLA LOS QUE SE QUEDAN +++++++++");
                //Se recorre la tabla 3G
               
                foreach (DataRow pRow in dsNorte.Tables["3G_48_H_OK"].Rows)
                {


                    //Si la tabla con los tipos no tiene esa DESCRIPCION OBRA la meto en la lista para informar
                    if (!listTablaTipos.Contains(pRow["DESCRIPCION OBRA"]))
                    {
                        //Si no lo añadi ya lo añado
                        if (!sinEstadoenEjecucion.ContainsKey(pRow["DESCRIPCION OBRA"].ToString()))
                        {
                            Console.WriteLine("No esta en Tabla: " + pRow["DESCRIPCION OBRA"].ToString());
                            sinEstadoenEjecucion.Add(pRow["DESCRIPCION OBRA"].ToString(), "");
                        }


                    }

                //Console.WriteLine("EN 3G --> " + pRow["DESCRIPCION"].ToString() + " +++++ " + pRow["TIPO TRABAJO"].ToString());
                foreach (DataRow tRow in dsNorte.Tables["Tabla"].Rows)
                    {
                        //SI coinciden las descripcione verifico que se hace
                        if ((pRow["DESCRIPCION OBRA"].ToString() == tRow["TIPO"].ToString()) )
                        {

                            //Console.WriteLine("     COINCIDE --> " + pRow["DESCRIPCION"].ToString() + " +++++ " + pRow["TIPO TRABAJO"].ToString() + " ------- " +  pRow["DESCRIPCION"].ToString() + " +++++ " + pRow["TIPO TRABAJO"].ToString());
                            //1 - Si ESTADO = NO entonces la borro
                            if (tRow["ESTADO"].ToString() == "NO")
                            {
                                //Console.WriteLine("Borrado porque descripcion es: " + pRow["DESCRIPCION OBRA"].ToString() + " +++++ y tabla tiene  " + tRow["TIPO"].ToString() + " ------- " + tRow["ESTADO"].ToString() + " +++++ " + pRow["TIPO TRABAJO"].ToString());

                                pRow.Delete();

                                //                          Console.WriteLine("Borrado porque tiene un NO");
                                break;
                            }

                        //Si es de integracion+e me lo quedo, pero ya mire que no fuera un NO
                        if (pRow["TIPO TRABAJO"].ToString() == "INTEGRACION+e")
                        {
                            //Console.WriteLine("Es de integracion+e " + pRow["TIPO TRABAJO"].ToString());
                            break;
                        }


                        //Si el estado no coincide lo borro
                        if (pRow["TIPO TRABAJO"].ToString() != tRow["ESTADO"].ToString())
                            {
                                //Console.WriteLine("Borrado porque no coincide ****" + pRow["TIPO TRABAJO"].ToString() + " --" + tRow["ESTADO"].ToString());

                                pRow.Delete();

                                break;
                            }
                            //Si el estado coincide dejo de tratarla porque me quedo con esa fila y no pierdo tiempo
                            if (pRow["TIPO TRABAJO"].ToString() == tRow["ESTADO"].ToString())
                            {
                                //Console.WriteLine("NO BORRADO porque coincide ****" + pRow["TIPO TRABAJO"].ToString() + " --" + tRow["ESTADO"].ToString());

                                break;
                            }



                        }

                    }


                }



                dsNorte.Tables["3G_48_H_OK"].AcceptChanges();
                //Console.WriteLine("REPINTANDO LA TABLA 3G");
                dtNorte = dsNorte.Tables["3G_48_H_OK"];
                //grid_items_norte2.DataSource = dtNorte;








                Console.WriteLine("+++++++ COMPARNDO CON TABLA LOS QUE SE QUEDAN +++++++++");
                //Se recorre la tabla LTE
                foreach (DataRow pRow in dsNorte.Tables["LTE_48_H_OK"].Rows)
                {



                    //Si la tabla con los tipos no tiene esa DESCRIPCION OBRA la meto en la lista para informar
                    if (!listTablaTipos.Contains(pRow["DESCRIPCION OBRA"]))
                    {
                        //Si no lo añadi ya lo añado
                        if (!sinEstadoenEjecucion.ContainsKey(pRow["DESCRIPCION OBRA"].ToString()))
                        {
                            Console.WriteLine("No esta en Tabla: " + pRow["DESCRIPCION OBRA"].ToString());
                            sinEstadoenEjecucion.Add(pRow["DESCRIPCION OBRA"].ToString(), "");
                        }


                    }

                //Console.WriteLine("EN 3G --> " + pRow["DESCRIPCION"].ToString() + " +++++ " + pRow["TIPO TRABAJO"].ToString());
                foreach (DataRow tRow in dsNorte.Tables["Tabla"].Rows)
                    {
                        //SI coinciden las descripcione verifico que se hace
                        if ((pRow["DESCRIPCION OBRA"].ToString() == tRow["TIPO"].ToString()))
                        {

                            //Console.WriteLine("     COINCIDE --> " + pRow["DESCRIPCION"].ToString() + " +++++ " + pRow["TIPO TRABAJO"].ToString() + " ------- " +  pRow["DESCRIPCION"].ToString() + " +++++ " + pRow["TIPO TRABAJO"].ToString());
                            //1 - Si ESTADO = NO entonces la borro
                            if (tRow["ESTADO"].ToString() == "NO")
                            {
                                //Console.WriteLine("Borrado porque descripcion es: " + pRow["DESCRIPCION OBRA"].ToString() + " +++++ y tabla tiene  " + tRow["TIPO"].ToString() + " ------- " + tRow["ESTADO"].ToString() + " +++++ " + pRow["TIPO TRABAJO"].ToString());

                                pRow.Delete();

                                //                          Console.WriteLine("Borrado porque tiene un NO");
                                break;
                            }



                        //Si es de integracion+e me lo quedo, pero ya mire que no fuera un NO
                        if (pRow["TIPO TRABAJO"].ToString() == "INTEGRACION+e")
                        {
                            //Console.WriteLine("Es de integracion+e " + pRow["TIPO TRABAJO"].ToString());
                            break;
                        }



                        //Si el estado no coincide lo borro
                        if (pRow["TIPO TRABAJO"].ToString() != tRow["ESTADO"].ToString())
                            {
                                //Console.WriteLine("Borrado porque no coincide ****" + pRow["TIPO TRABAJO"].ToString() + " --" + tRow["ESTADO"].ToString());

                                pRow.Delete();

                                break;
                            }
                            //Si el estado coincide dejo de tratarla porque me quedo con esa fila y no pierdo tiempo
                            if (pRow["TIPO TRABAJO"].ToString() == tRow["ESTADO"].ToString())
                            {
                                //Console.WriteLine("NO BORRADO porque coincide ****" + pRow["TIPO TRABAJO"].ToString() + " --" + tRow["ESTADO"].ToString());

                                break;
                            }



                        }

                    }


                }

                dsNorte.Tables["LTE_48_H_OK"].AcceptChanges();
                //Console.WriteLine("REPINTANDO LA TABLA LTE");
                dtNorte = dsNorte.Tables["LTE_48_H_OK"];
                //grid_items_norte2.DataSource = dtNorte;






                //Sacar el de mayor fecha
                //Creamos una nueva tabla donde estaran los datos finales
                DataTable final = dsNorte.Tables.Add("Final");
                tratados = new List<Fila>();
                var tratadosCOC = new List<string>();
                string[] formats = { "MM.dd.yyyy" };


                //Recorro las filas de LTE
                foreach (DataRow pRow in dsNorte.Tables["LTE_48_H_OK"].Rows)
                {
                    Fila f = null;
                    //Si no esta tratado saco los datos
                    if (!tratadosCOC.Contains(pRow["COC"].ToString()))
                    {

                        string[] formats3 = { "MM.dd.yyyy" };
                        //sacamos la fecha primera de LTE no tiene porque ser la fecha final mas proxima
                        var fecha = DateTime.ParseExact(pRow["fecha cumple 48H"].ToString(), formats3, new CultureInfo("en-US"), DateTimeStyles.None);
                        f = new Fila(pRow["ZONA"].ToString(), pRow["PROVINCIA"].ToString(), pRow["COD EMPL"].ToString(), pRow["NOMBRE SITE"].ToString(), pRow["DESCRIPCION OBRA"].ToString(), pRow["TIPO TRABAJO"].ToString(), pRow["COC"].ToString(), pRow["FECHA Integracion"].ToString());
                        f.FechacumpleLTE = fecha;

                        //recorro el resto para modificar la fecha

                        foreach (DataRow pRow2 in dsNorte.Tables["LTE_48_H_OK"].Rows)
                        {
                            //Si encuentro otro que tenga el mismo COC comparo las fechas
                            if (f.CObra == pRow2["COC"].ToString())
                            {
                                //Compara la fecha
                                string[] formats2 = { "MM.dd.yyyy" };
                                var fecha2 = DateTime.ParseExact(pRow2["fecha cumple 48H"].ToString(), formats2, new CultureInfo("en-US"), DateTimeStyles.None);
                                //Si la comparacion es mayor que cero, la nueva fecha es mas actual y la cambio en la fila
                                if (fecha.CompareTo(fecha2) < 0)
                                {
                                    //Actualizamos en caso de una fecha mas proxima
                                    f.FechacumpleLTE = fecha2;
                                    //Console.WriteLine("Actualizada: " + pRow2["COC"].ToString() + " -- " + fecha2);
                                }
                            }
                        }


                    //Recorremos 3G para sacar la fecha de 3g cumple

                    bool hayUna = false;
                    foreach (DataRow pRow3G in dsNorte.Tables["3G_48_H_OK"].Rows)
                        {
                            //si tienen el mismo COC miro las fecha
                            if (f.CObra == pRow3G["COC"].ToString())
                            {
                                string[] formats2 = { "MM.dd.yyyy" };
                                var fecha3G = DateTime.ParseExact(pRow3G["fecha cumple 48H"].ToString(), formats2, new CultureInfo("en-US"), DateTimeStyles.None);
                                //Si hay una las comparo y me quedo la mayor
                                if (hayUna)
                                {
                                    if (f.Fechacumple3G.CompareTo(fecha3G) < 0)
                                    {
                                        //Actualizamos en caso de una fecha mas proxima
                                        f.Fechacumple3G = fecha3G;
                                        //Console.WriteLine("Actualizada 3G: " + pRow3G["COC"].ToString() + " -- " + fecha3G);
                                    }
                                }
                                //Sino me quedo la  primera que tengo
                                else
                                {
                                    f.Fechacumple3G = fecha3G;
                                    hayUna = true;
                                }

                            }

                        }


                    //Recorremos 2G para sacar la fecha de 2G cumple
                    bool hayUna2 = false;
                    foreach (DataRow pRow2G in dsNorte.Tables["2G_48_H_OK"].Rows)
                        {

                            //si tienen el mismo COC miro las fecha
                            if (f.CObra == pRow2G["COC"].ToString())
                            {
                                //Console.WriteLine("CELDAS EN 2g --------------" + pRow2G["fecha cumple 48H"].ToString());
                                string[] formats2 = { "MM.dd.yyyy" };
                                var fecha2G = DateTime.ParseExact(pRow2G["fecha cumple 48H"].ToString(), formats2, new CultureInfo("en-US"), DateTimeStyles.None);
                                //Si hay una las comparo y me quedo la mayor
                                if (hayUna2)
                                {
                                    if (f.Fechacumple2G.CompareTo(fecha2G) < 0)
                                    {
                                        //Actualizamos en caso de una fecha mas proxima
                                        f.Fechacumple2G = fecha2G;
                                        //Console.WriteLine("Actualizada 2G: " + pRow2G["COC"].ToString() + " -- " + fecha2G);
                                    }
                                }
                                //Sino me quedo la  primera que tengo
                                else
                                {
                                    f.Fechacumple2G = fecha2G;
                                    hayUna2 = true;
                                }

                            }

                        }


                        //Sacamos la fecha de cumplimiento como la más alta de LTE, 3G y 2G
                        f.Fechacumple = f.SacarFechaCumple();



                    }
                  


                    //cuando acabe de tratar esa fila puedo meterla en tratadas porque ya trate todas con ese COC
                    tratadosCOC.Add(pRow["COC"].ToString());
                    if (f != null)
                    {
                        tratados.Add(f);
                    }

                }
            //Con el metodo anteriro solo trato si estan en LTE pero si no tiene celda en LTE, debo mirar 2G y crearlo y tambien 3G y crearlo
            //Recorro las fila de 2G
            foreach (DataRow pRow in dsNorte.Tables["2G_48_H_OK"].Rows)
            {
                Fila f = null;
                //Miro si el COC esta tratado ya, en ese caso ignoro la fila
                if (!tratadosCOC.Contains(pRow["COC"].ToString()))
                {
                    //sacamos la fecha primera de 2G no tiene porque ser la fecha final mas proxima
                    var fecha = DateTime.ParseExact(pRow["fecha cumple 48H"].ToString(), formats, new CultureInfo("en-US"), DateTimeStyles.None);
                    f = new Fila(pRow["ZONA"].ToString(), pRow["PROVINCIA"].ToString(), pRow["COD EMPL"].ToString(), pRow["NOMBRE SITE"].ToString(), pRow["DESCRIPCION OBRA"].ToString(), pRow["TIPO TRABAJO"].ToString(), pRow["COC"].ToString(), pRow["FECHA Integracion"].ToString());
                    f.Fechacumple2G = fecha;


                    //Recorremos el resto del 2g buscando otras iguales
                    foreach (DataRow pRow2 in dsNorte.Tables["2G_48_H_OK"].Rows)
                    {
                        //Si encuentro otro que tenga el mismo COC comparo las fechas
                        if (f.CObra == pRow2["COC"].ToString())
                        {
                            //Compara la fecha
                            string[] formats2 = { "MM.dd.yyyy" };
                            var fecha2 = DateTime.ParseExact(pRow2["fecha cumple 48H"].ToString(), formats2, new CultureInfo("en-US"), DateTimeStyles.None);
                            //Si la comparacion es mayor que cero, la nueva fecha es mas actual y la cambio en la fila
                            if (fecha.CompareTo(fecha2) < 0)
                            {
                                //Actualizamos en caso de una fecha mas proxima
                                f.Fechacumple2G = fecha2;
                                //Console.WriteLine("Actualizada: " + pRow2["COC"].ToString() + " -- " + fecha2);
                            }
                        }
                    }

                    //Recorremos 3G para sacar la fecha de 3g cumple
                    bool hayUna = false;
                    foreach (DataRow pRow3G in dsNorte.Tables["3G_48_H_OK"].Rows)
                    {
                        //si tienen el mismo COC miro las fecha
                        if (f.CObra == pRow3G["COC"].ToString())
                        {
                            string[] formats2 = { "MM.dd.yyyy" };
                            var fecha3G = DateTime.ParseExact(pRow3G["fecha cumple 48H"].ToString(), formats2, new CultureInfo("en-US"), DateTimeStyles.None);
                            //Si hay una las comparo y me quedo la mayor
                            if (hayUna)
                            {
                                if (f.Fechacumple3G.CompareTo(fecha3G) < 0)
                                {
                                    //Actualizamos en caso de una fecha mas proxima
                                    f.Fechacumple3G = fecha3G;
                                    //Console.WriteLine("Actualizada 3G: " + pRow3G["COC"].ToString() + " -- " + fecha3G);
                                }
                            }
                            //Sino me quedo la  primera que tengo
                            else
                            {
                                f.Fechacumple3G = fecha3G;
                                hayUna = true;
                            }

                        }

                    }
                    //No recorremos LTE porque sabemos que no va a tener
                    //La fecha se rellena en el constructor
                    //f.FechacumpleLTE = new DateTime(1900, 1, 1);

                    //Sacamos la fecha de cumplimiento como la más alta de LTE, 3G y 2G
                    f.Fechacumple = f.SacarFechaCumple();


                }

                //cuando acabe de tratar esa fila puedo meterla en tratadas porque ya trate todas con ese COC
                tratadosCOC.Add(pRow["COC"].ToString());
                if (f != null)
                {
                    tratados.Add(f);
                }


            }
            //Recorro las fila de 3G
            foreach (DataRow pRow in dsNorte.Tables["3G_48_H_OK"].Rows)
            {
                Fila f = null;
                //Miro si el COC esta tratado ya, en ese caso ignoro la fila
                if (!tratadosCOC.Contains(pRow["COC"].ToString()))
                {
                    Console.WriteLine("Creando uno para solo 3G " + pRow["COC"].ToString());
                    //sacamos la fecha primera de 2G no tiene porque ser la fecha final mas proxima
                    var fecha = DateTime.ParseExact(pRow["fecha cumple 48H"].ToString(), formats, new CultureInfo("en-US"), DateTimeStyles.None);
                    f = new Fila(pRow["ZONA"].ToString(), pRow["PROVINCIA"].ToString(), pRow["COD EMPL"].ToString(), pRow["NOMBRE SITE"].ToString(), pRow["DESCRIPCION OBRA"].ToString(), pRow["TIPO TRABAJO"].ToString(), pRow["COC"].ToString(), pRow["FECHA Integracion/_encendido"].ToString());
                    f.Fechacumple3G = fecha;


                    //Recorremos el resto del 3g buscando otras iguales
                    foreach (DataRow pRow2 in dsNorte.Tables["3G_48_H_OK"].Rows)
                    {
                        //Si encuentro otro que tenga el mismo COC comparo las fechas
                        if (f.CObra == pRow2["COC"].ToString())
                        {
                            //Compara la fecha
                            string[] formats2 = { "MM.dd.yyyy" };
                            var fecha2 = DateTime.ParseExact(pRow2["fecha cumple 48H"].ToString(), formats2, new CultureInfo("en-US"), DateTimeStyles.None);
                            //Si la comparacion es mayor que cero, la nueva fecha es mas actual y la cambio en la fila
                            if (fecha.CompareTo(fecha2) < 0)
                            {
                                //Actualizamos en caso de una fecha mas proxima
                                f.Fechacumple3G = fecha2;
                                //Console.WriteLine("Actualizada: " + pRow2["COC"].ToString() + " -- " + fecha2);
                            }
                        }
                    }

                    
                    //No recorremos LTE ni 3G porque sabemos que no va a estar
                    //Pero si rellenamos sus fechas
                    //f.FechacumpleLTE = new DateTime(1900, 1, 1);
                    //f.Fechacumple2G = new DateTime(1900, 1, 2);

                    //Sacamos la fecha de cumplimiento como la más alta de LTE, 3G y 2G
                    f.Fechacumple = f.SacarFechaCumple();


                }

                //cuando acabe de tratar esa fila puedo meterla en tratadas porque ya trate todas con ese COC
                tratadosCOC.Add(pRow["COC"].ToString());
                if (f != null)
                {
                    tratados.Add(f);
                }

            }


            Console.WriteLine("TODO ACABADO");
                conNorte.Close();
                conTabla.Close();

            
            }
            catch (OleDbException)
            {
                //Si salta errror aqui suele ser o porque no tienen los nombres adecuados las hojas de excel o porque se tiene algo abierto.
                //Tambien se detecto que al copiar excels con Windows dejan de funcionar y se tienen que abrir y guardar como con otro nombre.

                MessageBoxButtons buttons = MessageBoxButtons.OK;
                string caption = "Posible error en el Excel";
                string message = "Cierre todos los Excel abiertos y vuelve a ejecutar." + "\n\n\n" + "Si el problema persiste Abre todas las NIR y guardalas con otro nombre";
                MessageBox.Show(message, caption, buttons);
            }
            catch (System.ArgumentException e)
            {
                MessageBox.Show("Revisa las tablas de los Excel y que son los excel correctos", "Posible error en las cabeceras");
            }
            catch (System.FormatException)
            {
                MessageBox.Show("Una de las fechas de la NIR no tiene el formato correcto", "Error en la fechas de la NIR");
            }
            
            finally
            {
                if (conNorte != null)
                {
                    conNorte.Close();
                }
                if (conTabla != null)
                {
                    conTabla.Close();
                }
            }
           
            
            return tratados;
        }






        /// <summary> Metodo para carga una unica vez la tabla vNpo
        /// De esta forma se evita leer y cargar la tabla con cada zona.
        /// Se carga la lista vNpo con los que tienen fecha (para borrar) y la vNpoTodos porque si alguno no esta se debe borrar.
        /// </summary>

        public void sacarvNpo()
        {
            OleDbConnection conTabla = null;

            string pathTabla = Path.Combine(Environment.CurrentDirectory, @"Data\", "Tabla.xlsx");

            String constrTabla = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                               pathTabla +
                                ";Extended Properties='Excel 8.0;HDR=YES;';";

            conTabla = new OleDbConnection(constrTabla);


            String namevpno = "VPNO";
            OleDbCommand oconnvNpo = new OleDbCommand("Select * From[" + namevpno + "$] WHERE KPI IS NOT NULL ", conTabla);
            OleDbCommand oconnvNpoTodos = new OleDbCommand("Select COC From[" + namevpno + "$] ", conTabla);


            OleDbDataAdapter sdavNpo = new OleDbDataAdapter(oconnvNpo);
            OleDbDataAdapter sdavNpoTodos = new OleDbDataAdapter(oconnvNpoTodos);


            DataSet dsvNpo = new DataSet("vNpo");

            sdavNpo.Fill(dsvNpo, "vNpo");
            sdavNpoTodos.Fill(dsvNpo, "vNpoTodos");



            //Recoroo todos los vNpo con fecha y los meto a la lista
            
            foreach (DataRow vRow in dsvNpo.Tables["vNpo"].Rows)
            {
                vNpo.Add(vRow["COC"].ToString());
            }

            
            foreach (DataRow vRow in dsvNpo.Tables["vNpoTodos"].Rows)
            {
                vNpoTodos.Add(vRow["COC"].ToString());
            }

        }



        /// <summary> Metodo para comprobar que no hay algun tipo de obra sin poner en Tabla
        /// <list type="bullet">
        /// <item>Ruta al fichero NIR</item>
        /// </list>
        /// </summary>
        public DataTable comprobarTiposObra(string ruta)
        {

            Console.WriteLine("En comprobar tipo de obra");

            OleDbConnection conNorte = null;
            OleDbConnection conTabla = null;
            List<Fila> tratados = null;

           
                // 1.Sacamos la tabla de relaciones
                //------------------------------------------------------------------------------------------------------------------------CAMBIAR LA RUTA



            string pathTabla = Path.Combine(Environment.CurrentDirectory, @"Data\", "Tabla.xlsx");
            //Console.WriteLine(pathTabla);

            




            String constrTabla = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                                pathTabla +
                                ";Extended Properties='Excel 8.0;HDR=YES;';";

                conTabla = new OleDbConnection(constrTabla);



                String nameTabla = "TABLA";
                OleDbCommand oconnTabla = new OleDbCommand("Select * From[" + nameTabla + "$]", conTabla);


                OleDbDataAdapter sdaTabla = new OleDbDataAdapter(oconnTabla);


                DataSet dsNorte = new DataSet("Norte"); //Contendra varias tablas

                sdaTabla.Fill(dsNorte, "Tabla");











                // 2. Sacamos los que tengan el COC en resumen - 2G - 3G - LTE
                String constrNorte = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                            ruta +
                            ";Extended Properties='Excel 8.0;HDR=YES;';";

                conNorte = new OleDbConnection(constrNorte);

                conNorte.Open();


                // Console.WriteLine("MOSTRANDO NOMBRES DE LAS TABLAS");
                DataTable dtSheets = null;
                dtSheets = conNorte.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                String[] excelSheets2 = new String[dtSheets.Rows.Count];
                int numbersheets = 0;
                foreach (DataRow row in dtSheets.Rows)
                {
                    excelSheets2[numbersheets] = row["TABLE_NAME"].ToString();

                    numbersheets++;

                }
           

            /*
        * Para que no se modifiquen los nombres de las hojas se ha decidido sacarlos por su orden, vienen dados por orden alfabetico por eso estan asi
        * 3G_48_H_OK - 1
        * 2G 48 H OK  - 0
        * LTE_48_H_OK - 10
        * RESUMEN - 11
         */
            // ORDENES DE LAS HOJAS
            int s3G = 1;
                int s2G = 0;
                int sLTE = 10;
                int sresumen = 11;
                Console.WriteLine(excelSheets2[0]);
                Console.WriteLine(excelSheets2[1]);
                Console.WriteLine(excelSheets2[10]);
                Console.WriteLine(excelSheets2[11]);




            //    1. Preparamos la consulta para los que no estan en Resumen
            // DISTINCT evita duplicados

            //Consulta para Norte2G   
            OleDbCommand oconnNorte2 = new OleDbCommand("Select DISTINCT `DESCRIPCION OBRA` From [" + excelSheets2[s2G] + "] WHERE COC NOT IN(SELECT COC FROM[" + excelSheets2[sresumen] + "]) ", conNorte);
       





                //Consulta para Norte3G 
                OleDbCommand oconnNorte3 = new OleDbCommand("Select DISTINCT `DESCRIPCION OBRA` From[" + excelSheets2[s3G] + "] WHERE COC NOT IN(SELECT COC FROM[" + excelSheets2[sresumen] + "]) ", conNorte);




                //Consulta para NorteLTG
                OleDbCommand oconnNorteLTE = new OleDbCommand("Select DISTINCT `DESCRIPCION OBRA` From[" + excelSheets2[sLTE] + "] WHERE COC NOT IN(SELECT COC FROM[" + excelSheets2[sresumen] + "])  ", conNorte);







                // 2. Creamos un dataset donde metemos todo lo de NORTE


                // Se pintan en las Tablas
                OleDbDataAdapter sdaNorte2 = new OleDbDataAdapter(oconnNorte2);
                OleDbDataAdapter sdaNorte3 = new OleDbDataAdapter(oconnNorte3);
                OleDbDataAdapter sdaNorteLTE = new OleDbDataAdapter(oconnNorteLTE);


            //Si da error aqui del tipo [Error no esperado desde el controlador de la base de datos externa (1) ]
            // probar a guadar el excel otra vez 
            Console.WriteLine("Leyendo 2G");
                 sdaNorte2.Fill(dsNorte, "2G_48_H_OK");

                Console.WriteLine("Leyendo 3G");
                sdaNorte2.Fill(dsNorte, "3G_48_H_OK");

                Console.WriteLine("Leyendo LTE");
                sdaNorte2.Fill(dsNorte, "LTE_48_H_OK");

                DataTable dtNorte = new DataTable();
               
                





            //Recorro la tabla y saco los tipos de obra que ya existen
            Console.WriteLine("Recorriendo la tabla");
            
                List<String> listTabla = new List<string>();
                foreach (DataRow pRow in dsNorte.Tables["Tabla"].Rows)
                {
                    String tipo = pRow["TIPO"].ToString();
                    if (!listTabla.Contains(tipo))
                    {
                        listTabla.Add(tipo);
                    }
                }


            List<String> listSinEstado = new List<string>();

            //Se crea la tabla para añadir las nuevas
            DataTable dtSinEstado = new DataTable();
            dtSinEstado.Clear();
            dtSinEstado.Columns.Add("TIPO");
            dtSinEstado.Columns.Add("ESTADO");


            //Recorro 2G
            foreach (DataRow pRow in dsNorte.Tables["2G_48_H_OK"].Rows)
            {
                string tipo = pRow["DESCRIPCION OBRA"].ToString();
                if (!listTabla.Contains(tipo) && !listSinEstado.Contains(tipo))
                {
                    //Console.WriteLine("No contiene: " + tipo);
                    listSinEstado.Add(tipo);
                    DataRow _fila = dtSinEstado.NewRow();
                    _fila["TIPO"] = tipo;
                    _fila["ESTADO"] = "";
                    dtSinEstado.Rows.Add(_fila);

                }
            }
             //Recorro 3G
            foreach (DataRow pRow in dsNorte.Tables["3G_48_H_OK"].Rows)
            {
                string tipo = pRow["DESCRIPCION OBRA"].ToString();
                if (!listTabla.Contains(tipo) && !listSinEstado.Contains(tipo))
                {
                    //Console.WriteLine("No contiene: " + tipo);
                    listSinEstado.Add(tipo);
                    DataRow _fila = dtSinEstado.NewRow();
                    _fila["TIPO"] = tipo;
                    _fila["ESTADO"] = "";
                    dtSinEstado.Rows.Add(_fila);
                }
            }
             //Recorro LTE
            foreach (DataRow pRow in dsNorte.Tables["LTE_48_H_OK"].Rows)
            {
                string tipo = pRow["DESCRIPCION OBRA"].ToString();
                if (!listTabla.Contains(tipo) && !listSinEstado.Contains(tipo))
                {
                    //Console.WriteLine("No contiene: " + tipo);
                    listSinEstado.Add(tipo);
                    DataRow _fila = dtSinEstado.NewRow(); 
                    _fila["TIPO"] = tipo;
                    _fila["ESTADO"] = "";
                    dtSinEstado.Rows.Add(_fila);
                }
            }




  





            

                Console.WriteLine("Acabado comprobaciones tipo de obra");












            return dtSinEstado;

            }






        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                MessageBox.Show("Unable to release the Object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }






    }








}
