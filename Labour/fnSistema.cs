using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using System.Windows.Forms;
using DevExpress.XtraReports.UI;
using System.Drawing;
using System.IO;
using System.Globalization;
using DevExpress.XtraEditors;
using System.Drawing.Imaging;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using Microsoft.Win32;
using System.Threading;
using System.Management;
using System.Diagnostics;
using System.ServiceProcess;
using System.Collections.Specialized;
using System.Configuration;
/*PARA ENVÍO DE CORREOS*/
using System.Net.Mail;
using System.Net.Security;
using System.Net.Mime;
using System.Data.SqlTypes;
using DevExpress.XtraBars.Alerter;

namespace Labour
{
    class fnSistema
    {
        /// <summary>
        /// Nombre del servidor.
        /// </summary>
        public static string pgServer;
        /// <summary>
        /// Nombre de la base de datos.
        /// </summary>
        public static string pgDatabase;
        /// <summary>
        /// Nombre de usuario.
        /// </summary>
        public static string pgUser;
        /// <summary>
        /// Contraseña usuario base de datos.
        /// </summary>
        public static string pgPass;
        /// <summary>
        /// Nombre juego de datos.
        /// </summary>
        public static string ConfigName = "";

        //PARA GUARDAR OPCION FILTRO
        public static int OpcionBusqueda = 3;

        //PARA GUARDAR ULTIMO CONTRATO VISUALIZADO
        public static string ContratoVisualizado = "";

        /// <summary>
        /// Version actual del sistema
        /// </summary>
        public static string VersionSistema = "0.2.8";

        //PARA SABER SI YA HAY UNA INSTANCIA DE LA APLICACION CORRIENDO
        //public static bool AplicacionCorriendo = false;

        #region "BASE DE DATOS"
        public static string StringCon = "";
        public static SqlConnection sqlConn;
        public static DataSet ds;
        public static DataTable sqlData;
        public static SqlDataAdapter sqlDataAdapter;
        public static SqlCommandBuilder sqlDa;
        public static SqlCommand sqlCmd;
        public static SqlDataReader sqlRe;
        public static SqlDataReader sqlDr;

        /// <summary>
        /// Abre una conexion con la bae de datos
        /// </summary>
        /// <returns></returns>
        public static bool ConectarSQLServer()
        {
            if (sqlConn != null)
                sqlConn.Dispose();

            StringCon = String.Format("Server={0};Database={1};User Id={2};Password={3}; MultipleActiveResultSets=True;Application Name='Remu'", pgServer, pgDatabase, pgUser, pgPass);
            
            try
            {
                //CREO UNA NUEVA INSTANCIA DE LA CLASE
                sqlConn = new SqlConnection(StringCon);

                sqlConn.Open();                

                return true;
            }
            catch (SqlException ex)
            {
                return false;                
            }
        }
        #endregion
        /// <summary>
        /// Abre una conexion a la base de datos y devuelve un objeto sqlconnection.
        /// <para>Util para procesos en otros hilos de ejecución.</para>
        /// </summary>
        /// <returns></returns>
        public static SqlConnection OpenConnection()
        {
            StringCon = String.Format("Server={0};Database={1};User Id={2};Password={3}; MultipleActiveResultSets=True;Application Name='Remu'", pgServer, pgDatabase, pgUser, pgPass);
            SqlConnection con = new SqlConnection();
            try
            {
                con.ConnectionString = StringCon;                
                con.Open();
            }
            catch (SqlException ex)
            {
                //ERROR...
                con = null;
            }

            return con;
        }

        /// <summary>
        /// Hace un respaldo de la base de datos en el servidor.
        /// </summary>
        public static bool BackUpDataBase()
        {
            string sql = "BACKUP DATABASE @pBaseDatos " +
                         "TO DISK=@pFile";
            Configuracion conf = new Configuracion();
            conf.SetConfiguracion();

            string Name = "";
            Name = Calculo.PeriodoObservado + "-" + pgDatabase + ".bak";
            string File = conf.RutaRespaldo + "\\" + Name;

            SqlCommand cmd;
            SqlConnection cn;
            bool correcto = false;
            try
            {
                cn = OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@pBaseDatos", pgDatabase));
                            cmd.Parameters.Add(new SqlParameter("@pFile", File));

                            cmd.ExecuteNonQuery();
                            correcto = true;
                        }
                        cmd.Dispose();
                    }
                }
            }
            catch (SqlException ex)
            {
                //ERROR
                correcto = false;
            }

            return correcto;
        }
        
        #region "Grilla"
        /// <summary>
        /// Permite cargar un datasource en un control gridcontrol.
        /// </summary>
        /// <param name="pGridView">Control gridcontrol.</param>
        /// <param name="pSQL">Query sql.</param>
        public static void spllenaGridView(DevExpress.XtraGrid.GridControl pGridView, string pSQL)
        {

            try
            {
                if (ConectarSQLServer())
                {
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    DataSet ds = new DataSet();

                    SqlCommand command = new SqlCommand(pSQL, sqlConn);
                    adapter.SelectCommand = command;
                    adapter.Fill(ds);
                    adapter.Dispose();
                    command.Dispose();
                    sqlConn.Close();
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        pGridView.DataSource = ds.Tables[0];
                    }
                    else
                    {
                        pGridView.DataSource = null;
                    }
                }
            }
            catch (SqlException ex)
            {
                //ERROR...
            }
        }

        /// <summary>
        /// Permite configurar distintas propiedades de un control GridControl.
        /// </summary>
        /// <param name="pGrid">Control gridcontrol.</param>
        public static void spOpcionesGrilla(DevExpress.XtraGrid.Views.Grid.GridView pGrid, bool? Editable = false)
        {
            //setear propiedades de la grilla
            pGrid.OptionsSelection.EnableAppearanceHideSelection = false;
            pGrid.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFullFocus;
            if(Editable == true)
                pGrid.OptionsBehavior.Editable = true;
            else
                pGrid.OptionsBehavior.Editable = false;

            pGrid.OptionsSelection.EnableAppearanceFocusedCell = false;
            pGrid.OptionsSelection.MultiSelect = false;

            //deshabilitar menus contextuales
            pGrid.OptionsMenu.EnableColumnMenu = false;
            pGrid.OptionsMenu.EnableFooterMenu = false;
            pGrid.OptionsMenu.EnableGroupPanelMenu = false;

            //evitar filtrar por columnas y Ordenar por Columnas
            pGrid.OptionsCustomization.AllowFilter = false;
            pGrid.OptionsCustomization.AllowGroup = false;
            //pGrid.OptionsCustomization.AllowSort = false;
            pGrid.OptionsCustomization.AllowColumnResizing = true;
            pGrid.OptionsCustomization.AllowColumnMoving = false;

            //deshabilitar cabezera de la tabla
            pGrid.OptionsView.ShowGroupPanel = false;   

            pGrid.Appearance.FocusedRow.BackColor = Color.DodgerBlue;
            pGrid.Appearance.FocusedRow.ForeColor = Color.White;
            pGrid.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        }
        #endregion

        #region "RUT"
        /// <summary>
        /// Enmascara un rut.
        /// <para>Ej: 94561238 -> 094561238</para>
        /// </summary>
        /// <param name="pRut">Rut persona.</param>
        /// <returns></returns>
        public static string fEnmascaraRut(string pRut)
        {
            int iLargoStr = 0;
            iLargoStr = pRut.Length;//obtener el largo del parametro
            if (iLargoStr == 9)
            {
                return pRut;
            }

            if (iLargoStr == 8)
            {
                return "0" + pRut;
            }
            return "";
        }

        /// <summary>
        /// Desenmascara un rut.
        /// <para>Ej: 094561238 -> 94561238</para>
        /// </summary>
        /// <param name="pRut">Rut persona.</param>
        /// <returns></returns>
        public static string fnDesenmascararRut(string pRut)
        {
            if (pRut == "") return "";
            int largo = pRut.Length;
            
            string p = "";
            string sub = "";
            if (largo == 9)
            {
                //EXTRAEMOS EL PRIMER CARACTER
                p = fnFirstString(pRut);              
                //PREGUNTAMOS SI EL CARACTER ES 0
                if (p == "0")
                {
                    //CREAMOS UNA SUBCADENA SIN EL PRIMER CARACTER
                    sub = fnRightString(pRut, 8);
                    //RETORNAMOS LA CADENA
                    return sub;
                }
                else
                {
                    //RETORNAMOS LA CADENA SIN MODIFICAR
                    sub = pRut;
                    return sub;
                }
            }
           
            return "";
        }

        /// <summary>
        /// Valida que el digito verificador corresponda al rut.
        /// </summary>
        /// <param name="x123">Rut persona.</param>
        /// <returns></returns>
        public static bool fValidaRut(string x123)
        {
            int p, h, bander;
            string rut1 = "";
            string rut2, div, d1;

            rut2 = x123;
            p = rut2.Length;//largo cadena original
            //si es vacia la cadena retornamos false
            if (rut2 == "") return false;
            if (rut2.Length < 8) return false;           

            rut1 = rut2.Substring(0, 8);//extrae cadena sin digito verificador
           
            
            div = rut2.Substring((rut2.Length) - 1, 1);//ultimo caracter que representa el digito verificador                 
          
            rut1 = rut1.Trim();//quitar espacios y caracteres raros
            h = rut1.Length;//largo de la cadena          +
                            //rut1 = rut1.Substring(1, 8);      

            bander = 1;

            string numero = "";
            for (int i = 0; i < rut1.Length; i++)
            {
                //preguntamos por cada caracter si es numero
                numero = rut1.Substring(i, 1);

                if (IsNumeric(numero) == false)
                {
                    //si no es numero bander = 0
                    bander = 0;
                }
            }

            //si bander es cero return false
            if (bander == 0)
            {
                return false;
            }

            int k = 2, l = 0;

            //recorremos desde el ultimo elemento de la cadena
            //para calcular el digito verificador
            for (int i = 7; i >= 0; i--)
            {
                if (k == 8)
                {
                    //reset
                    k = 2;
                }
                //sumamos
                l = l + Convert.ToInt32(rut1.Substring(i, 1)) * k;
                //aumentamos k;
                k++;
            }
           
            //calculamos el digito verificador
            int d = l % 11;
            d = 11 - d;
            
            switch (d)
            {
                case 10:
                    d1 = "K";                    
                    break;
                case 11:
                    d1 = "0";
                    break;
                default:
                    d1 = d.ToString();
                    break;
            }
            
            //preguntamos si el valor de div concuerda con el valor calculado en d
            if (div.ToLower() != d1.ToLower())
            {
                //error porque los digitos no coinciden                
                return false;
                
            }
            else
            {
                //rut valido
                return true;
            }
        }

        /// <summary>
        /// Indica si una cadena de entrada es un numero.
        /// </summary>
        /// <param name="input">Cadena a evaluar.</param>
        /// <returns></returns>
        public static bool IsNumeric(string input)
        {
            int test;
            return int.TryParse(input, out test);
        }

        /// <summary>
        /// Verifica si una cadena de entrada es un numero decimal.
        /// </summary>
        /// <param name="input">Cadena a evaluar.</param>
        /// <returns></returns>
        public static bool IsDecimal(string input)
        {
            double test;
            return double.TryParse(input, out test);
        }    

        /// <summary>
        /// Agrega puntos y guion a una cadena de entrada que representa un rut.
        /// <para>Ej: 174536007 -> 17.453.600-7</para>
        /// </summary>
        /// <param name="pRut">Cadena a evaluar.</param>
        /// <returns></returns>
        public static string fFormatearRut2(string pRut)
        {
            if (pRut == "") return "";
            //RECORRER CADENA Y DEJAR EN FORMATO XXX.XXX.XXX-X
            //CADENA TIENE COMO MAXIMO 12 CARACTERES (INCLUIDO PUNTOS Y GUION)
            //123 456 789 123
            //REGLAS
            /*
             *@1 ANTES DEL ULTIMO ELEMENTO SIEMPRE VA UN GUION
             *@2 CADA 3 ELEMENTOS VA UN PUNTO DESDE DERECHA A IZQUIERDA Y SIN CONTAR EL GUION             
             */
            int original = pRut.Length;
            string digito = pRut.Substring(original-1, 1);
            //EXTRAEMOS CADENA SIN EL ULTIMO DIGITO (REPRESENTA A EL DIGITO VERIFICADOR)
            string subcadena = pRut.Substring(0, pRut.Length - 1);
            string[] invertir = new string[] { };
            //guardamos el largo de la subcadena
            int largo = subcadena.Length;
            string cad = "";
            string cadFinal = "";
            string res = "";
            int multiplo = 0;
            //RECORREMOS CADENA DE FORMA INVERSA
            for (int i = largo-1; i >=0; i--)
            {
                cad = cad + subcadena[i];               
            }
            
            //RECORREMOS NUEVAMENTE LA CADENA PARA ENCONTRAR EL MAYOR MULTIPLO DE 3
            for (int i = 0; i < cad.Length; i++)
            {
                //SI ES MULTIPLO DE 3
                if (i % 3 == 0)
                {
                    multiplo = i;
                }
            }

            for (int i = 0; i < cad.Length; i++)
            {
                if (i % 3 == 0)
                {
                    if (i < multiplo)
                    {
                        cadFinal = cadFinal + cad.Substring(i, 3) + ".";
                    }
                    else
                    {
                        
                      cadFinal = cadFinal + cad.Substring(i, (cad.Length)-i); 
                    }                  
                }
            }

            for (int i = (cadFinal.Length)-1; i >=0; i--)
            {
                res = res + cadFinal[i];
            }
            //agregamos digito verificador 
            res = res + "-" + digito;            

            return res;
        }

        //verificar que el rut contenga puntos y guion
        public static bool fnRutCorrecto(string pRut)
        {
            /*
             * QUE CONTENGA PUNTO Y GUION
             * QUE CONTENGA PUNTO Y NO CONTENGA GUION
             * QUE NO CONTENGA PUNTO Y CONTENGA GUION
             * QUE NO CONTENGA PUNTO Y NO CONTENGA GUION
             */

            /*
             * @2 CASES
             * @1 12 CARACTERES => 17.444.666-k
             * @2 11 CARACTERES => 8.456.159-0
             */
            if (pRut.Length == 12)
            {
                //SI CONTIENE 12 CARACTERES
                //REGLAS:
                /*
                 * EN TERCERA POSICION DEBERIA HABER UN . (SEGUNDA PARTIENDO DESDE CERO)
                 * EN SEPTIMA POSICION DEBERIA HABER UN . ( SEXTA PARTIENDO DESDE CERO)
                 * EN POSICION ONCE DEBERIA HABER UN - (DECIMA POSICION PARTIENDO DESDE CERO)
                 */

                //RECORRER CADENA Y PREGUNTAR CUANDO PUNTO TIENE
                //SI TIENE MAS DE 2 NO ES VALIDO
                //LO MISMO PARA LA COMA 
                int c = 0, p = 0;
                for (int i = 0; i < pRut.Length; i++)
                {
                    if (pRut[i].ToString() == ".") p++;
                    if (pRut[i].ToString() == "-") c++;
                }

                if (c > 1) return false;
                if (c == 0) return false;
                if (p > 2 || p < 2) return false;

                int posC = 0, posP1 = 0, posP2 = 0;
                for (int i = 0; i < pRut.Length; i++)
                {
                    if (pRut[i].ToString() == ".")
                    {
                        //GUARDO SU POSICION
                        if (i == 2) posP1 = i;
                        if (i == 6) posP2 = i;
                    }
                    if (pRut[i].ToString() == "-")
                    {
                        //GUARDO SU POSICION
                        if (i == 10) posC = i;
                    }
                }

                if (posP1 == 0 || posP2 == 0 || posC == 0) return false;
                else return true;

            }
            else if (pRut.Length == 11)
            {
                //SI CONTIENE 11 CARACTERES
                //REGLAS
                /*
                 * EN SEGUNDA POSICION DEBERIA HABER UN . (PRIMERA POSICION PARTIENDO DESDE CERO)
                 * EN SEXTA POSICION DEBERIA HABER UN . (QUINTA POSICION PARTIENDO DESDE CERO)
                 * EN DECIMA POSICION DEBERIA HABER UN - (NOVENA POSICION PARTIENDO DESDE CERO)
                 */

                int c = 0, p = 0;
                for (int i = 0; i < pRut.Length; i++)
                {
                    if (pRut[i].ToString() == ".") p++;
                    if (pRut[i].ToString() == "-") c++;
                }

                if (c > 1) return false;
                if (c == 0) return false;
                if (p > 2 || p < 2) return false;

                int posC = 0, posP1 = 0, posP2 = 0;
                for (int i = 0; i < pRut.Length; i++)
                {
                    if (pRut[i].ToString() == ".")
                    {
                        //GUARDO SU POSICION
                        if (i == 1) posP1 = i;
                        if (i == 5) posP2 = i;
                    }
                    if (pRut[i].ToString() == "-")
                    {
                        //GUARDO SU POSICION                        
                        if (i == 9) posC = i;
                    }
                }
                if (posP1 == 0 || posP2 == 0 || posC == 0) return false;
                else return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// Quita espacios entre medio de una cadena.
        /// </summary>
        /// <param name="cadena">Cadena de entrada.</param>
        /// <returns></returns>
        public static string QuitarEspacios(string cadena)
        {
            string cad = "";
            if (cadena.Length == 0) return "";
            for (int i = 0; i < cadena.Length; i++)
            {
                if (cadena[i].ToString() != " ")
                    cad = cad + cadena[i];
            }

            return cad;
        }

        /// <summary>
        /// Retorna el digito verificador de un rut.
        /// <para>Ej:17.453.600-7</para>
        /// <para>Devuelve 7</para>
        /// </summary>
        /// <param name="pRut">Rut persona.</param>
        /// <returns></returns>
        public static string Getdigito(string pRut)
        {
            string dv = "";
            if (pRut.Length>0)
            {
                if (pRut.Length == 9 || pRut.Length == 8)                
                    dv = pRut.Substring(pRut.Length - 1, 1);                                
            }
            return dv;
        }

        /// <summary>
        /// Retorna el rut sin el digito verificador.
        /// <para>Ej:17.453.600-7</para>
        /// <para>Devuelve 17.453.600</para>
        /// </summary>
        /// <param name="pRut">Rut de la persona.</param>
        /// <returns></returns>
        public static string GetRutsindv(string pRut)
        {
            string sindv = "";
            if (pRut.Length>0)
            {
                if(pRut.Length == 9)
                    sindv = pRut.Substring(0, 8);
                if (pRut.Length == 8)
                    sindv = pRut.Substring(0, 7);
            }

            return sindv;
        }
        #endregion

        #region "CADENAS"
        /// <summary>
        /// Extrae el último caracter de una cadena.
        /// </summary>
        /// <param name="pString">Cadena a evaluar.</param>
        /// <returns></returns>
        public static string fnLastString(string pString)
        {
            if (pString.Length == 1)
            {
                return pString;
            }

            string last = pString.Substring(pString.Length - 1, 1);

            return last;
        }

        /// <summary>
        /// Extrae el primer elemento de una cadena.
        /// </summary>
        /// <param name="pString">Cadena a evaluar.</param>
        /// <returns></returns>
        public static string fnFirstString(string pString)
        {
            if (pString.Length == 1)
            {
                return pString;
            }

            string first = pString.Substring(0, 1);
            return first;
        }

        /// <summary>
        /// Extrae de derecha a izquierda una subcadena.
        /// </summary>
        /// <param name="pString">Cadena a evaluar.</param>
        /// <param name="pCantidad">Representa la cantidad de elementos que se desea extraer.</param>
        /// <returns></returns>
        public static string fnRightString(string pString, int pCantidad)
        {
            //CADENA
            //EJEMPLO (CADENA, 2)
            int largo = pString.Length;           
            string subcadena = pString.Substring(largo - pCantidad, pCantidad);
            return subcadena;
        }

        /// <summary>
        /// Extrae de izquierda a derecha una subcadena.
        /// </summary>
        /// <param name="pString">Cadena a evaluar.</param>
        /// <param name="pCantidad">Representa la cantidad de elementos que se desea extraer.</param>
        /// <returns></returns>
        public static string fnLeftString(string pString, int pCantidad)
        {
            string subcadena = pString.Substring(0, pCantidad);
            return subcadena;
        }

        /// <summary>
        /// Valida que un correo tenga el formato correcto.
        /// </summary>
        /// <param name="pCorreo">Cadena a evaluar.</param>
        /// <returns></returns>
        public static bool fnValidaCorreo(string pCorreo)
        {
            if (pCorreo.Length == 0) return false;
            if (pCorreo.Contains("@") == false) return false;
            
            int c = 0;
            string[] cad = new string[2];
            string[] cad2 = new string[2];
            string cadPunto = "";
            string cadAntesPunto = "";
            char[] rules = new char[] { '@', '.'};

            if (pCorreo.Contains("@"))
            {
                //recorrer cadena para los arroba que contiene
                for (int i = 0; i < pCorreo.Length; i++)
                {
                    
                    if (pCorreo[i].ToString() == "@")
                        c++;
                }
                //SI C>1 ES PORQUE HAY MAS DE UNA @ (CORREO NO VALIDO)
                
                if (c > 1) return false;

                //SEPARAMOS LA CADENA EN DOS TOMANDO COMO SEPARADOR EL '@'                             
                cad = pCorreo.Split(rules[0]);                
                if (cad[0].Length == 0) return false;

                //SEGUNDA POSICION DEL ARREGLO REPRESENTA LA CADENA DESPUES DE ARROBA
                string despues = cad[1];
                //PREGUNTAMOS SI ESTA CADENA CONTIENE ALGUN '.'
                int p = 0;
                for (int i = 0; i < despues.Length; i++)
                {
                    if (despues[i].ToString() == ".")
                        p++;
                }
                //SI P>1 NO ES VALIDO
                if (p > 1) return false;
                //SI NO ENCUENTRA NINGUN PUNTO TAMPOCO ES VALIDO
                if (p == 0) return false;

                //SI ENCUENTRA PUNTO SEPARAMOS CADENA
                cad2 = despues.Split(rules[1]);
                //SI NO HAY NADA ANTES DEL PUNTO TAMPO ES VALIDO
                cadAntesPunto = cad2[0];
                if (cadAntesPunto.Length == 0) return false;
                if (cadAntesPunto.Contains("-") || cadAntesPunto.Contains("_")) return false;
                //GUARDAMOS LA CADENA DESPUES DEL PUNTO
                cadPunto = cad2[1];
                //SI CADENA TIENE MAS DE UN DIGITOS (CL, COM, ES, CC, PE, AR) ES VALIDO
                if (cadPunto.Length < 2) return false;                
            } 
            return true;
        }       

        /// <summary>
        /// Quita caracteres punto y guion en un rut.
        /// </summary>
        /// <param name="pCadena">Cadena a evaluar.</param>
        /// <returns></returns>
        public static string fnExtraerCaracteres(string pCadena)
        {
            if (pCadena.Length == 0) return "";
            string nsub = "";
            char[] rules = new char[] { '.', '-' };
            string[] sub = new string[] { };
            if (pCadena.Contains(".") || pCadena.Contains("-"))
            {
                //SEPARAMOS CADENA
                sub = pCadena.Split(rules);
                //generamos la nueva cadena                
                for (int i = 0; i < sub.Length; i++)
                {
                    //CONCATENAMOS NUEVA CADENA SIN PUNTOS NI GUION
                    nsub = nsub + sub[i];
                }
            }           

            //retornamos la nueva cadena
            return nsub;
        }

        /// <summary>
        /// Valida el formato correcto para un numero expresado en porcentajes.
        /// </summary>
        /// <param name="pCadena">Cadena a evaluar.</param>
        /// <returns></returns>
        public static bool fnValidarPorcentajes(string pCadena)
        {
            //recorrer cadena y buscar los '.' y comas
            int c = 0;
            for (int i = 0; i < pCadena.Length; i++)
            {
                if (pCadena[i].ToString() == ",") c++;
            }
            //SI HAY MAS DE UNA COMA NO ES VALIDO                      
            if (c > 1) return false;
            if (pCadena.Length == 1 && pCadena == ",") return false;

            //SEPARAMOS LA CADENA POR COMA
            char[] separador = new char[] { ',' };
            string[] cadenas = pCadena.Split(separador);
            //123,2 --> NO VALIDO
            if (cadenas[0].Length > 2) return false;
            //100 --> valido            
            //en otro caso es valido y retornamos true
            return true;
        }

        /// <summary>
        /// Entrega el formato para campo declaracion jurada
        /// </summary>
        /// <param name="pEntry"></param>
        /// <param name="pLargo"></param>
        /// <param name="pTipo"></param>
        /// <returns></returns>
        public static string GetFormat(string pEntry, int pLargo, int pTipo)
        {           
            double n = 0;
            //Largo 12;
           

            //Numerico
            if (pTipo == 1)
            {
                if (pEntry.Contains(","))
                    pEntry = pEntry.Replace(",", ".");

                if (Convert.ToDouble(pEntry) < 0)
                {

                    n = Convert.ToDouble(pEntry) * -1;
                    pEntry = new string('0', pLargo) + n;
                    pEntry = "-" + pEntry.Substring((pEntry.Length - (pLargo - 1)), pLargo -1);
                }
                else
                {
                    pEntry = new string('0', pLargo) + Convert.ToDouble(pEntry);
                    pEntry = pEntry.Substring((pEntry.Length - pLargo), pLargo);
                }
            }

            //Alfanumerico
            else if (pTipo == 2)
            {
                pEntry = pEntry + new string(' ', pLargo);
                pEntry = pEntry.Substring(0, pLargo);
                pEntry = GetCadenaSinAcento(pEntry);
                pEntry = pEntry.ToUpper();

            }

            return pEntry;
        }

        /// <summary>
        /// Quita los acentos
        /// </summary>
        /// <param name="pCadena"></param>
        /// <returns></returns>
        public static string GetCadenaSinAcento(string pCadena)
        {
            string cad = "";
            string NormalizedString = pCadena.Normalize(NormalizationForm.FormD);
            StringBuilder st = new StringBuilder();

            for (int i = 0; i < NormalizedString.Length; i++)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(NormalizedString[i]);
                if (uc != UnicodeCategory.NonSpacingMark)
                    st.Append(NormalizedString[i]);
            }

            return st.ToString().Normalize(NormalizationForm.FormC);
        }

        /// <summary>
        /// Extrae de rut los numero 2, 3 y 4 (para generacion de folio)
        /// </summary>
        /// <returns></returns>
        public static string GetDigRutFolio(string pRut)
        {
            string campos = "";
            try
            {
                if (pRut.Length > 0)
                {
                    campos = pRut.Substring(1, 3);
                }
            }
            catch (Exception ex)
            {
                campos = "";
            }

            return campos;
        }

        #endregion

        #region "Fechas"
        /// <summary>
        /// Retorna la cantidad de dias que tiene un determinado mes en un determinado año.
        /// </summary>
        /// <param name="pMes">Mes</param>
        /// <param name="pYear">Año</param>
        /// <returns></returns>
        public static int DiasMes(int pMes, int pYear)
        {
            int day = DateTime.DaysInMonth(pYear, pMes);
            return day;           
        }

        /// <summary>
        /// Retorna la cantidad de dias que tiene el mes y año en curso.
        /// </summary>
        /// <returns></returns>
        public static int DiasMesActual()
        {
            int y = DateTime.Now.Year;
            int m = DateTime.Now.Month;
            //MessageBox.Show(m.ToString());
            int day = DateTime.DaysInMonth(y, m);
            return day;
        }

        /// <summary>
        /// Retorna la cantidad de dias que hay entre un rango de fechas.
        /// </summary>
        /// <param name="fecha1">Fecha inicial.</param>
        /// <param name="fecha2">Fecha termino.</param>
        /// <returns></returns>
        public static int CantidadDiasFechas(DateTime fecha1, DateTime fecha2)
        {
            int dias = 0;
            TimeSpan fec = fecha2 - fecha1;
            dias = (fec.Days) + 1;

            return dias;
        }

        //OBTENER LA FECHA DE INGRESO CON FORMATO NOMBRE MES (12 JULIO 2017)
        /// <summary>
        /// Retorna una fecha con formato dia, mes en palabras, año.
        /// <para>12-07-2017</para>
        /// <para>Retorna 12 Julio 2017</para>
        /// </summary>
        /// <param name="ingreso">Fecha a evaluar.</param>
        /// <returns></returns>
        public static string FechaFormato(DateTime ingreso)
        {
            string mes = "", dia = "", year = "", final = "";
            mes = ingreso.ToString("MMMM");
            dia = ingreso.Day.ToString();
            year = ingreso.Year.ToString();

            if (int.Parse(dia) < 10)
                dia = "0" + dia;

            final = dia + " " + mes + " " + year;

            return final;
        }

        /// <summary>
        /// Retorna el mes en palabras desde una fecha.
        /// </summary>
        /// <param name="ingreso">Fecha a evaluar.</param>
        /// <returns></returns>
        public static string FechaFormatoSoloMes(DateTime ingreso)
        {
            string mes = "", dia = "", year = "", final = "";
            mes = ingreso.ToString("MMMM");
            dia = ingreso.Day.ToString();
            year = ingreso.Year.ToString();

            if (int.Parse(dia) < 10)
                dia = "0" + dia;

            final = mes + " " + year;

            return final;
        }

        /// <summary>
        /// Retorna el periodo anterior al periodo de entrada.
        /// <para>Ej: 201901</para>
        /// <para>Devuelve 201812</para>
        /// </summary>
        /// <param name="periodo">Periodo a evaluar.</param>
        /// <returns></returns>
        public static int fnObtenerPeriodoAnterior(int periodo)
        {
            //201710
            DateTime anterior = DateTime.Now.Date;
            int ant = 0;
            string date = "", mes = "", year = "";
            date = periodo.ToString();

            year = date.Substring(0, 4);
            mes = date.Substring(4, 2);


            date = "01-" + mes + "-" + year;
            anterior = DateTime.Parse(date);
            anterior = anterior.AddDays(-1);

            if (anterior.Month <= 9)
            {
                mes = "0" + (anterior.Month);

            }
            else
            {

                mes = (anterior.Month).ToString();
            }

            year = (anterior.Year).ToString();

            date = year + mes;

            ant = Convert.ToInt32(date);

            return ant;
        }

        /// <summary>
        /// Retorna el periodo siguiente al periodo de entrada.
        /// </summary>
        /// <param name="periodo">Periodo a evaluar.</param>
        /// <returns></returns>
        public static int PeriodoSiguiente(int periodo)
        {
            int siguiente = 0;
            string date = "", mes = "", year = "";
            DateTime d = DateTime.Now.Date;
            date = periodo.ToString();
            year = date.Substring(0, 4);
            mes = date.Substring(4, 2);

            date = "01-" + mes + "-" + year;
            d = DateTime.Parse(date);
            d = d.AddMonths(1);

            if (d.Month <= 9)
            {
                mes = "0" + d.Month;
            }
            else
            {
                mes = d.Month.ToString();
            }            
            year = d.Year.ToString();

            date = year + mes;
            siguiente = int.Parse(date);

            return siguiente;
        }

        /// <summary>
        /// Retorna una fecha en base al periodo ingresado.
        /// <para>Ej: 201902</para>
        /// <para>Devuelve 01-02-2019</para>
        /// </summary>
        /// <param name="periodo">Periodo a evaluar.</param>
        /// <returns></returns>
        public static DateTime FechaPeriodo(int periodo)
        {
            DateTime f = DateTime.Now.Date;
            string date = "", mes = "", year = "";
            date = periodo.ToString();
            year = date.Substring(0, 4);
            mes = date.Substring(4, 2);

            //CREAMOS FECHA CON EL PRIMER DIA DEL MES
            date = "01-" + mes + "-" + year;            

            f = DateTime.Parse(date);

            return f;
        }    

        /// <summary>
        /// Retorna el ultimo dia del mes de un periodo determinado.
        /// <para>Ej:201901</para>
        /// <para>Devuelve 31-01-2019</para>
        /// </summary>
        /// <param name="periodo">Periodo a evaluar.</param>
        /// <returns></returns>
        public static DateTime UltimoDiaMes(int periodo)
        {
            string mes = "", year = "", date = "";
            int lastDay = 0;
            DateTime ultimo = DateTime.Now.Date;

            date = periodo.ToString();
            year = date.Substring(0, 4);
            mes = date.Substring(4, 2);

            //OBTENER EL ULTIMO DIA DEL MES
            lastDay = DateTime.DaysInMonth(int.Parse(year), int.Parse(mes));

            date = lastDay + "-" + mes + "-" + year;
            ultimo = DateTime.Parse(date);

            return ultimo;

        }

        /// <summary>
        /// Retorna el primer dia de un periodo determinado
        /// <para>Ej: 201901</para>
        /// <para>Devuleve 01-01-2019</para>
        /// </summary>
        /// <param name="periodo">Periodo a evaluar.</param>
        /// <returns></returns>
        public static DateTime PrimerDiaMes(int periodo)
        {
            DateTime first = DateTime.Now.Date;
            string date = "", mes = "", year = "";

            date = periodo.ToString();
            year = date.Substring(0, 4);
            mes = date.Substring(4, 2);
            //GENERAS LA FECHA COMO STRING
            date = "01-" + mes + "-" + year;
            //TRANSFORMAMOS EL STRING A DATETIME
            first = DateTime.Parse(date);
            //RETORNAMOS FECHA
            return first;
        }

        /// <summary>
        /// Indica si una persona es mayor de 65 años.
        /// </summary>
        /// <param name="FechaNacimiento">Fecha de nacimiento.</param>
        /// <returns></returns>
        public static bool AdultoMayor(DateTime FechaNacimiento)
        {
            int edad = 0;
            //OBTENER EL AÑO EN CURSO
            DateTime today = DateTime.Now.Date;

            //RESTA
            edad = today.Year - FechaNacimiento.Year;

            if (edad > 65)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Indica si una persona es mayor de 60 de años.
        /// </summary>
        /// <param name="FechaNacimiento">Fecha de nacimiento.</param>
        /// <returns></returns>
        public static bool AdultoMayorMujer(DateTime FechaNacimiento)
        {            
            int edad = 0;
            DateTime today = DateTime.Now.Date;
            edad = today.Year - FechaNacimiento.Year;

            if (edad >=60)
            {
                return true;
            }
            else
            {
                return false;
            }            
        }
        
        /// <summary>
        /// Indica si un hombre es mayor de 65 años o una mujer es mayor de 60 años.
        /// </summary>
        /// <param name="FechaNacimiento">Fecha de nacimiento.</param>
        /// <param name="Sexo">Sexo.</param>
        /// <returns></returns>
        public static bool AdultoMay(DateTime FechaNacimiento, int Sexo)
        {
            DateTime hoy = DateTime.Now.Date;
            int year = 0;
            if (Sexo == 0)
            {
                //HOMBRE
                year = hoy.Year - FechaNacimiento.Year;
                if (year >= 65)
                    return true;
            }
            else if (Sexo == 1)
            {
                //MUJER
                year = hoy.Year - FechaNacimiento.Year;
                if (year >= 60)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Indica si una persona es menor de edad.
        /// </summary>
        /// <param name="FechaNacimiento">Fecha de nacimiento.</param>
        /// <returns></returns>
        public static bool MenorDeEdad(DateTime FechaNacimiento)
        {
            int edad = 0;
            DateTime today = DateTime.Now.Date;

            edad = today.Year - FechaNacimiento.Year;

            if (edad < 18)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Indica si una persona es mayor de edad.
        /// </summary>
        /// <param name="FechaNacimiento">Fecha de nacimiento.</param>
        /// <returns></returns>
        public static bool MayorDeEdad(DateTime FechaNacimiento)
        {
            DateTime today = DateTime.Now.Date;
            int edad = 0;

            edad = today.Year - FechaNacimiento.Year;

            if (edad >= 18)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //OBTENER EL PERIODO DE ACUERDO A FECHA
        //EJ: SI FECHA ES 27-02-2018 , PERIODO SERÁ > 201802
        /// <summary>
        /// Obtener el periodo en formato yyyyMM desde una fecha.
        /// <para>01-02-2019</para>
        /// <para>Devuelve 201902</para>
        /// </summary>
        /// <param name="fecha">Fecha a evaluar.</param>
        /// <returns></returns>
        public static int PeriodoFromDate(DateTime fecha)
        {
            int periodo = 0;
            string cad = "", m = "", y = "";
            int mes = 0, year = 0;
            if (fecha != null)
            {
                mes = fecha.Month;
                year = fecha.Year;

                if (mes < 10)
                    m = "0" + mes.ToString();
                else
                    m = mes.ToString();

                y = year.ToString();
                cad = y + m;
                periodo = Convert.ToInt32(cad);

                //RETORNAMOS CAD
                return periodo;                
            }

            return periodo;
        }

        //MOSTRAR PERIODO 201802 --> FEBRERO 2018
        /// <summary>
        /// Devuelve fecha mostrando solo mes y año.
        /// <para>201902</para>
        /// <para>Devuelve Febrero 2019</para>
        /// </summary>
        /// <param name="Fecha">Fecha a evaluar.</param>
        /// <returns></returns>
        public static string GetNameDate(DateTime Fecha)
        {
            string name = "", mes = "";
            int year = 0;
            mes = Fecha.ToString("MMMM", CultureInfo.CreateSpecificCulture("es"));
            year = Fecha.Year;
            name = mes + " " + year;            

            return name;
        }

        //PERIODO CON SALIDA MMYYYY
        public static string PeriodoInvertido(int periodo)
        {
            //201805
            string p = periodo.ToString();
            string y = "", m = "", inv = "";

            y = p.Substring(0, 4);
            m = p.Substring(4, 2);

            inv = m + y;

            return inv;
        }

        //PERIODO CON SALIDA MMYYYY (ULTIMO DIA FECHA REMUNERACION)
        public static string PeriodoInvertidoUltimo(int periodo)
        {
            DateTime fecha = UltimoDiaMes(periodo);
            string m = "", y = "", inv = "";

            //mes
            if (fecha.Month < 9)
                m = "0" + fecha.Month;
            else
                m = fecha.Month.ToString();

            //AÑO
            y = fecha.Year.ToString();

            //MMYYYY
            inv = m + y;

            return inv;
        }

        /// <summary>
        /// Devuelve la cadena de entrada con el primer caracter en mayuscula.
        /// </summary>
        /// <param name="pCadena">Cadena a evaluar.</param>
        /// <returns></returns>
        public static string PrimerMayuscula(string pCadena)
        {
            if (pCadena.Length > 0)
            {
                //pCadena = pCadena.First().ToString().ToUpper() + pCadena.Substring(1);
                pCadena = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(pCadena.ToLower());
            }

            return pCadena;

        }
        #endregion

        #region "PRINT"
        //MOSTRAR EL PDF UN REPORTE
        public static void ShowDocument(XtraReport reporte)
        {
            ReportPrintTool tool = new ReportPrintTool(reporte);            
            tool.ShowPreviewDialog();
          
        }
        #endregion        

        //OBTENER EL NOMBRE COMPLETO DEL TRABAJADOR DE ACUERDO A SU CONTRATO
        public static string GetNombreTrabajador(string pContrato)
        {
            string sql = "SELECT concat(nombre, ' ', apepaterno, ' ', apematerno) FROM trabajador " +
                "WHERE contrato=@pContrato AND anomes=@pPeriodo";
            SqlCommand cmd;
            string name = "";
            try
            {
                if (ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pPeriodo", Calculo.PeriodoObservado));
                        cmd.Parameters.Add(new SqlParameter("@pContrato", pContrato));

                        name = (string)cmd.ExecuteScalar();

                        cmd.Dispose();
                        sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return name;
        }
        /// <summary>
        /// Indica si el codigo de jornada laboral ingresado es un código válido
        /// </summary>
        /// <param name="pData">Código.</param>
        /// <returns></returns>
        public static bool ExisteJornada(int pData)
        {
            bool existe = false;

            switch (pData)
            {
                case 1:
                    existe = true;
                    break;
                case 2:
                    existe = true;
                    break;
                case 3:
                    existe = true;
                    break;
                default:
                    existe = false;
                    break;
            }

            return existe;
        }

        /// <summary>
        /// Combo para jornada laboral
        /// </summary>
        /// <param name="pCombo">Control LookupEdit</param>
        public static void fnComboJornada(LookUpEdit pCombo)
        {
            //List<strucStatus> struc = new List<strucStatus>();         
            List<PruebaCombo> lista1 = new List<PruebaCombo>();


            lista1.Add(new PruebaCombo() { key = 1, desc = "Lunes - Viernes" });
            lista1.Add(new PruebaCombo() { key = 2, desc = "Lunes - Sábado" });
            lista1.Add(new PruebaCombo() { key = 3, desc = "Turnos" });
            lista1.Add(new PruebaCombo() { key = 4, desc = "No aplica" });

            //PROPIEDADES COMBOBOX
            pCombo.Properties.DataSource = lista1.ToList();
            pCombo.Properties.ValueMember = "key";
            pCombo.Properties.DisplayMember = "desc";

            pCombo.Properties.PopulateColumns();

            //ocultamos la columan key
            pCombo.Properties.Columns[0].Visible = false;

            pCombo.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
            pCombo.Properties.AutoSearchColumnIndex = 1;
            pCombo.Properties.ShowHeader = false;

            if (pCombo.Properties.DataSource != null)
            {
                pCombo.ItemIndex = 0;
            }
        }

        /// <summary>
        /// Combo suspension laboral
        /// </summary>
        /// <param name="pCombo"></param>
        public static void fnComboSupensionLaboral(LookUpEdit pCombo)
        {
            List<datoCombobox> lista = new List<datoCombobox>();

            lista.Add(new datoCombobox() { KeyInfo = 0, descInfo = "No Aplica" });
            lista.Add(new datoCombobox() { KeyInfo = 13, descInfo = "Suspensión por Acto de Autoridad" });
            lista.Add(new datoCombobox() { KeyInfo = 14, descInfo = "Suspensión por Pacto" });
            lista.Add(new datoCombobox() { KeyInfo = 15, descInfo = "Reducción de jornada Laboral" });

            //PROPIEDADES COMBOBOX
            pCombo.Properties.DataSource = lista.ToList();
            pCombo.Properties.ValueMember = "KeyInfo";
            pCombo.Properties.DisplayMember = "descInfo";

            pCombo.Properties.PopulateColumns();

            //ocultamos la columan key
            pCombo.Properties.Columns[0].Visible = false;

            pCombo.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
            pCombo.Properties.AutoSearchColumnIndex = 1;
            pCombo.Properties.ShowHeader = false;

            if (pCombo.Properties.DataSource != null)
                pCombo.ItemIndex = 0;
        }

        /// <summary>
        /// Combo suspension laboral
        /// </summary>
        /// <param name="pCombo"></param>
        public static void fnComboPorcentajeSuspension(LookUpEdit pCombo)
        {
            List<datoCombobox> lista = new List<datoCombobox>();

            for (int i = 1; i <= 100; i++)
            {
                lista.Add(new datoCombobox() { KeyInfo = i, descInfo = i.ToString() });
            }

            //PROPIEDADES COMBOBOX
            pCombo.Properties.DataSource = lista.ToList();
            pCombo.Properties.ValueMember = "KeyInfo";
            pCombo.Properties.DisplayMember = "descInfo";

            pCombo.Properties.PopulateColumns();

            //ocultamos la columan key
            pCombo.Properties.Columns[0].Visible = false;

            pCombo.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
            pCombo.Properties.AutoSearchColumnIndex = 1;
            pCombo.Properties.ShowHeader = false;

            if (pCombo.Properties.DataSource != null)
                pCombo.ItemIndex = 0;
        }

        /// <summary>
        /// Nos entrega la version de sql usada
        /// </summary>
        /// <returns></returns>
        public static string GetInfoServer()
        {
            SqlConnection cn;
            string Version = "";
            int VersionNumber = 0;
            try
            {
                cn = OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        Version = cn.ServerVersion;
                        string[] serverVersionDetail = Version.Split(new string[] {"."}, StringSplitOptions.None);

                        VersionNumber = int.Parse(serverVersionDetail[0]);

                        switch (VersionNumber)
                        {
                            case 8:
                                Version = "SQL Server 2000";
                                break;
                            case 9:
                                Version = "SQL Server 2005";
                                break;
                            case 10:
                                Version = "SQL Server 2008";
                                break;
                            case 11:
                                Version = "SQL Server 2012";
                                break;
                            case 12:
                                Version = "SQL Server 2014";
                                break;
                            case 13:
                                Version = "SQL Server 2016";
                                break;
                            case 14:
                                Version = "SQL Server 2017";
                                break;
                            default:
                                Version = string.Format("SQL Server {0}", VersionNumber.ToString());
                                break;
                                
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
               //ERROR...
            }

            return Version;
        }

        /// <summary>
        /// indica si existe una comuna de acuerdo a su codigo
        /// </summary>
        public static bool ExisteComuna(int pCod)
        {
            bool existe = false;
            string sql = "SELECT count(*) FROM comuna WHERE codComuna=@pCod";
            SqlCommand cmd;
            SqlConnection cn;
            int count = 0;

            try
            {
                cn = OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@pCod", pCod));
                            count = Convert.ToInt32(cmd.ExecuteScalar());
                            if (count > 0)
                                existe = true;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                //ERROR...
            }

            return existe;
        }

        public static DataTable GetComunas()
        {
            DataTable Data = new DataTable();
            string sql = "SELECT codComuna as id, DescComuna, region FROM comuna ORDER BY region, descComuna";
            SqlCommand cmd;
            SqlDataAdapter ad = new SqlDataAdapter();
            DataSet ds = new DataSet();
            SqlConnection cn;

            try
            {
                cn = OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            ad.SelectCommand = cmd;
                            ad.Fill(ds, "data");

                            if (ds.Tables[0].Rows.Count > 0)
                                Data = ds.Tables[0];

                            ad.Dispose();
                            cmd.Dispose();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                //ERROR...
            }

            return Data;
        }

        //PARA AFP (NOMBRE AFP Y PORCENTAJE TOTAL A PAGAR)
        public static string PorcentajeAfp(string pContrato, int pPeriodo, bool RegimenAntiguo, double pImponible, double pTopeAfp, double pLic, double pDiastr, double pPorcentajeAdmin, double pImpAnterior, bool? showPreview = false, bool? EsSuspension = false, bool? Es13 = false, bool? Es14 = false)
        {
            CajaPrevision caja = new CajaPrevision();
            AseguradoraFondoPension Afp = new AseguradoraFondoPension();

            string info = "";
            double showValue = 0, porcPrevision = 0;
            double porcAdmin = 0;
            double ImponibleSuspension = 0;

            //imp = Calculo.GetValueFromCalculoMensaul(contrato, PeriodoEmpleado, "systimp");
            //topeAfp = Math.Round(varSistema.ObtenerValorLista("systopeafp"));
            //topeAfp = Calculo.GetValueFromCalculoMensaul(contrato, PeriodoEmpleado, "systopeafp");

            //Mostramos el tope
            if (EsSuspension == false)
            {
                if (pImponible > pTopeAfp)
                    showValue = pTopeAfp;
                else if (pTopeAfp > pImponible)
                    showValue = pImponible;
                else
                    showValue = pImponible;

                //TIENE LICENCIAS
                //double Lic = 0, diasTr = 0;
                //Lic = Calculo.GetValueFromCalculoMensaul(contrato, PeriodoEmpleado, "sysdiaslic");
                //diasTr = Calculo.GetValueFromCalculoMensaul(contrato, PeriodoEmpleado, "sysdiastr");

                if (pLic > 0)
                {
                    if (pImponible > pTopeAfp)
                        showValue = (showValue / 30) * pDiastr;
                }

                if (RegimenAntiguo)
                {
                    //Consultamos datos cajaprevision...
                    //porcPrevision = Calculo.GetValueFromCalculoMensaul(contrato, PeriodoEmpleado, "sysporcadmafp");
                    porcPrevision = pPorcentajeAdmin;
                    caja.SetInfo(pContrato, pPeriodo);
                    info = porcPrevision + "% Cotiz. " + caja.Nombre + " Sobre:" + showValue.ToString("N0");
                }
                else
                {
                    //Consultamos datos afp...
                    Afp = Persona.GetAfp(pContrato, pPeriodo);
                    //porcAdmin = Calculo.GetValueFromCalculoMensaul(contrato, PeriodoEmpleado, "sysporcadmafp");
                    porcAdmin = pPorcentajeAdmin;
                    info = (porcAdmin + Afp.porcFondo) + "% Cotiz. " + Afp.nombre + " Sobre:" + showValue.ToString("N0");
                }
            }
            else
            {
                if (pImpAnterior > pTopeAfp)
                    ImponibleSuspension = pTopeAfp;
                else
                    ImponibleSuspension = pImpAnterior;

                //ImponibleSuspension = (pImpAnterior / 30) * pDiastr;
                string cad = "";
                if (Es13 == true)
                    cad = " * Suspension autoridad";
                if (Es14 == true)
                    cad = " * Suspension pacto";

                if (RegimenAntiguo)
                {
                    //Consultamos datos cajaprevision...
                    //porcPrevision = Calculo.GetValueFromCalculoMensaul(contrato, PeriodoEmpleado, "sysporcadmafp");
                    porcPrevision = pPorcentajeAdmin;
                    caja.SetInfo(pContrato, pPeriodo);
                    info = caja.Nombre +  cad;
                }
                else
                {
                    //Consultamos datos afp...
                    Afp = Persona.GetAfp(pContrato, pPeriodo);
                    //porcAdmin = Calculo.GetValueFromCalculoMensaul(contrato, PeriodoEmpleado, "sysporcadmafp");
                    porcAdmin = pPorcentajeAdmin;
                    info =  Afp.nombre + cad;
                }

            }



            return info;
        }

        public static string GetCadenaSalud(bool AplicaPorc, bool AplicaUf, bool AplicaPesos, double pValue, double Lic, string pIsapre, int pCode, double ImpAnterior, bool? es13 = false, bool? es14 = false, bool? EsSuspension = false)
        {
            string cad = "", Isapre = "";
            int code = 0;

            code = pCode;
            Isapre = pIsapre;

            if (EsSuspension == true)
            {
                //FONASA
                if (code == 1)
                {
                    if (es13 == true)
                        cad = "Fonasa * Suspension autoridad";
                    if (es14 == true)
                        cad = "Fonasa * Suspension pacto";

                }
                //ISAPRE
                else if (code >= 2)
                {
                    if (es13 == true)
                        cad = $"{Isapre} * Suspension autoridad";
                    if (es14 == true)
                        cad = $"{Isapre} * Suspension pacto";

                }
            }
            else
            {
                //FONASA
                if (code == 1)
                {
                    if (AplicaPorc)
                    {
                        if (Lic != 0)
                            cad = $"{pValue}% FONASA (-Lic)";
                        else
                            cad = $"{pValue}% FONASA";
                    }
                    else if (AplicaUf)
                    {
                        if (Lic != 0)
                            cad = $"{pValue} UF FONASA (-Lic)";
                        else
                            cad = $"{pValue} UF FONASA";
                    }
                    else if (AplicaPesos)
                    {
                        if (Lic != 0)
                            cad = $"{pValue} Pesos FONASA (-Lic)";
                        else
                            cad = $"{pValue} Pesos FONASA";
                    }
                    else
                    {
                        if (Lic != 0)
                            //FONASA
                            cad = "7% FONASA (-Lic)";
                        else
                            cad = "7% FONASA";
                    }
                }
                //ISAPRE
                else if (code >= 2)
                {
                    //SI APLICA PORCENTAJE
                    if (AplicaPorc)
                    {
                        if (Lic != 0)
                            cad = $"{pValue}% {Isapre} (-Lic)";
                        else
                            cad = $"{pValue}% {Isapre}";
                    }
                    else if (AplicaUf)
                    {
                        if (Lic != 0)
                            cad = $"{pValue} UF {Isapre} (-Lic)";
                        else
                            cad = $"{pValue} UF {Isapre}";
                    }
                    else if (AplicaPesos)
                    {
                        if (Lic != 0)
                            cad = $"DESCUENTO {Isapre} (-Lic)";
                        else
                            cad = $"DESCUENTO {Isapre}";
                    }
                    else
                        cad = "DESCUENTO SALUD";
                }
            }


            return cad;
        }

        #region "PROGRAM"
        /// <summary>
        /// Indica si ya se está ejecutando un proceso.
        /// </summary>
        /// <param name="pName">Nombre del proceso.</param>
        /// <returns></returns>
        public static bool IsProcessRunning(string pName)
        {
            bool run = false;
            Process[] proc = Process.GetProcessesByName(pName);

            if (proc.Length > 1)
            {
                //HAY MAS DE UNA INSTANCIA DEL PROCESO EN EJECUCION...
                run = true;
            }
            return run;
        }

        /*LO MISMO PERO USANDO MUTEX*/
        public static bool IsProcessRunnigM(string pName)
        {
            bool run = false;
            bool appNewInstance = false;
            Mutex m = new Mutex(true, pName, out appNewInstance);

            if (!appNewInstance)
            {
                //YA HAY UNA INSTANCIA CORRIENDO
                run = true;
            }

            return run;
        }

        //INICIAR UN SERVICIO DE WINDOWS
        public static void IniciarServicio(string pServiceName)
        {
            ServiceController servicio = new ServiceController(pServiceName);
            try
            {
                if (servicio.Status == ServiceControllerStatus.Stopped)
                {
                    //PARMETROS
                    string[] parameters = new string[] {User.getUser(), fnSistema.pgDatabase};
                    servicio.Start(parameters);
                }                
            }
            catch (Exception ex)
            {
              //ERROR...
            }

        }

        //DETENER UN SERVICIO DE WINDOWS
        public static void DetenerServicio(string pServiceName)
        {
            ServiceController servicio = new ServiceController(pServiceName);
            try
            {
                if (servicio.Status == ServiceControllerStatus.Running)
                {
                    servicio.Stop();                    
                }
            }
            catch (Exception ex)
            {
                //ERROR...
            }
        }
        #endregion
    }

    /// <summary>
    /// Permite la manipulacion de comboboxs.
    /// </summary>
    class datoCombobox
    {
        /// <summary>
        /// Columna clave
        /// </summary>
        //public string KeyInfoString { get; set; }
        /// <summary>
        /// Columna clave
        /// </summary>
        public int KeyInfo { get; set; }
        /// <summary>
        /// Columna descripcion.
        /// </summary>
        public string descInfo { get; set; }

        /// <summary>
        /// Permite cargar un control LookupEdit.
        /// </summary>
        /// <param name="pSqlStr">Sql consulta.</param>
        /// <param name="pComboBox">Control LookupEdit.</param>
        /// <param name="pCampoKey">Field Key control LookupEdit.</param>
        /// <param name="pCampoInfo">Field Info control LookupEdit.</param>
        /// <param name="ocultarKey">Indica si se oculta o no el field key.</param>       
        public static void spllenaComboBox(string pSqlStr, DevExpress.XtraEditors.LookUpEdit pComboBox, string pCampoKey, string pCampoInfo, bool? ocultarKey=false)
        {
            List<datoCombobox> datosComboBox = new List<datoCombobox>();

            SqlCommand cmd;
            SqlDataReader re;
            SqlConnection cn;

            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        cmd = new SqlCommand(pSqlStr, cn);
                        re = cmd.ExecuteReader();

                        if (re.HasRows)
                        {
                            //int i = 0;
                            while (re.Read())
                            {
                                datosComboBox.Add(new datoCombobox() { KeyInfo = (int)re[pCampoKey], descInfo = (string)re[pCampoInfo] });
                            }
                        }
                        cmd.Dispose();
                        re.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
              //ERROR DE CONEXION
            }            
            
            //PROPIEDADES COMBOBOX
            pComboBox.Properties.DataSource = datosComboBox.ToList();
            pComboBox.Properties.ValueMember = "KeyInfo";
            pComboBox.Properties.DisplayMember = "descInfo";

            pComboBox.Properties.PopulateColumns();
            //SI OCULTAR KEY ES TRUE NO SE DEBE MOSTRAR LA COLUMNA KEY
            if (ocultarKey == true)
            {
                pComboBox.Properties.Columns[0].Visible = false;
            }
            pComboBox.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;            
            pComboBox.Properties.AutoSearchColumnIndex = 1;
            pComboBox.Properties.ShowHeader = false;

            if (pComboBox.Properties.DataSource != null)
            {
                pComboBox.ItemIndex = 0;
            }

            //EVITAR QUE SE PUEDAN ORDENAR LOS DATOS POR COLUMNA
            //pComboBox.Properties.Columns[0].AllowSort = DevExpress.Utils.DefaultBoolean.False;
            //pComboBox.Properties.Columns[1].AllowSort = DevExpress.Utils.DefaultBoolean.False;            
        }

        /// <summary>
        /// Permite cargar en un control LookupEdit un listado de periodos.
        /// </summary>
        /// <param name="pSqlStr">Sql consulta.</param>
        /// <param name="pComboBox">Control LookupEdit</param>
        /// <param name="pCampoKey">Filed key control LookupEdit.</param>
        /// <param name="pCampoInfo">Field info control LookUpEdit.</param>
        /// <param name="ocultarKey">indica si se oculta o no el campo clave.</param>       
        public static void spLlenaPeriodos(string pSqlStr, DevExpress.XtraEditors.LookUpEdit pComboBox, string pCampoKey, string pCampoInfo, bool? ocultarKey = false)
        {
            List<datoCombobox> datosComboBox = new List<datoCombobox>();

            SqlCommand cmd;
            SqlDataReader re;
            SqlConnection cn;

            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        cmd = new SqlCommand(pSqlStr, cn);
                        re = cmd.ExecuteReader();

                        if (re.HasRows)
                        {
                            //int i = 0;
                            while (re.Read())
                            {
                                datosComboBox.Add(new datoCombobox() { KeyInfo = (int)re[pCampoKey], descInfo = fnSistema.PrimerMayuscula(fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo((int)re[pCampoKey]))) });
                            }
                        }
                        cmd.Dispose();
                        re.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                //ERROR DE CONEXION
            }

            //PROPIEDADES COMBOBOX
            pComboBox.Properties.DataSource = datosComboBox.ToList();
            pComboBox.Properties.ValueMember = "KeyInfo";
            pComboBox.Properties.DisplayMember = "descInfo";

            pComboBox.Properties.PopulateColumns();
            //SI OCULTAR KEY ES TRUE NO SE DEBE MOSTRAR LA COLUMNA KEY
            if (ocultarKey == true)
            {
                pComboBox.Properties.Columns[0].Visible = false;
                //pComboBox.Properties.Columns[1].Visible = false;
            }
            pComboBox.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
            pComboBox.Properties.AutoSearchColumnIndex = 1;
            pComboBox.Properties.ShowHeader = false;

            if (pComboBox.Properties.DataSource != null)
                pComboBox.ItemIndex = 0;

        }

        /*Listado para agrupar reportes*/
        public static void AgrupaList(LookUpEdit pCombo)
        {
            //List<strucStatus> struc = new List<strucStatus>();         
            List<PruebaCombo> lista1 = new List<PruebaCombo>();

            lista1.Add(new PruebaCombo() { key = 0, desc = "Sin Agrupar"});
            lista1.Add(new PruebaCombo() { key = 1, desc = "CentroCosto" });
            lista1.Add(new PruebaCombo() { key = 2, desc = "Sucursal" });
            lista1.Add(new PruebaCombo() { key = 3, desc = "Area" });
            lista1.Add(new PruebaCombo() { key = 4, desc = "Cargo" });

            //PROPIEDADES COMBOBOX
            pCombo.Properties.DataSource = lista1.ToList();
            pCombo.Properties.ValueMember = "key";
            pCombo.Properties.DisplayMember = "desc";

            pCombo.Properties.PopulateColumns();

            //ocultamos la columan key
            pCombo.Properties.Columns[0].Visible = false;

            pCombo.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
            pCombo.Properties.AutoSearchColumnIndex = 1;
            pCombo.Properties.ShowHeader = false;

            if (pCombo.Properties.DataSource != null)
                pCombo.ItemIndex = 0;
        }
    }

    //CLASE OPCIONAL PARA EL CASO DEL STATUS Y EL JUBILADO YA QUE DA PROBLEMA CON EL TIPO DE DATO SHORT
    /// <summary>
    /// Solo para usar como objeto de carga de combobox para el caso de combos estáticos como jubilado y estatus.
    /// </summary>
    class PruebaCombo
    {
        /// <summary>
        /// Columna clave.
        /// </summary>
        public Int16 key { get; set; }
        /// <summary>
        /// Columna descripcion.
        /// </summary>
        public string desc { get; set; }
    }

    /// <summary>
    /// Permite manipular informacion de usuarios del sistema.
    /// </summary>
    class User
    {
        /// <summary>
        /// Password ingreso usuario.
        /// </summary>
        private static string password = "";

        /// <summary>
        /// Nombre usuario sistema.
        /// </summary>
        private static string user = "";

        //GET Y SET
        public static void SetUser(string User)
        {
            user = User;
        }

        /// <summary>
        /// Devuelve el nombre de usuario almacenado.
        /// </summary>
        /// <returns></returns>
        public static string getUser()
        {
            return user;
        }

        //CONSTRUCTOR
        public User()
        {
            //SIN PARAMETROS
        }

        public User(string User, string Password)
        {
            user = User;
            password = Password;
        }

        /// <summary>
        /// Obtiene el codigo del grupo al que pertenece el usuario.
        /// </summary>
        /// <returns></returns>
        public static int GetUserGroup()
        {
            int grupo = 0;
            if (user != "")
            {
                string sql = "SELECT grupo FROM usuario WHERE usuario=@pUser";
                SqlCommand cmd;                
                try
                {
                    if (fnSistema.ConectarSQLServer())
                    {
                        using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@pUser", user));

                            object data = cmd.ExecuteScalar();
                            if(data != DBNull.Value)
                                grupo = Convert.ToInt32(data);

                            cmd.Dispose();
                            fnSistema.sqlConn.Close();

                            return grupo;
                            
                        }
                    }
                }
                catch (SqlException ex)
                {
                  //...
                }
            }

            return grupo;
        }

        /// <summary>
        /// Obtiene el filtro asociado al usuario.
        /// <para>Si retorna '0' es porque no tiene filtro o condicionante.</para>
        /// </summary>
        /// <returns></returns>
        public static string GetUserFilter()
        {
            string filter = "";
            string sql = "SELECT filtro FROM usuario WHERE usuario=@usuario";
            SqlCommand cmd;

            try 
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@usuario", user));

                        filter = (string)cmd.ExecuteScalar();

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();                        
                    }
                }
            }
            catch (SqlException ex)
            {
              //...
            }

            return filter;
        }

        /// <summary>
        /// Devuelve la direccion ip local desde la cual el usuario está conectado al sistema.
        /// </summary>
        /// <returns></returns>
        public static string GetIpUser()
        {
            string localIp = "";
            try
            {
                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
                {
                    socket.Connect("8.8.8.8", 65530);
                    IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                    localIp = endPoint.Address.ToString();
                }
            }
            catch (Exception ex)
            {
                localIp = "127.0.0.1";
            }
          
            return localIp;
        }   

        //OBTENER EL SISTEMA OPERATIVO DONDE SE ESTÁ EJECUTANDO EL SISTEMA
        //OBTENER LA ARQUITECTURA DEL SISTEMA OPERATIVO DONDE SE ESTÁ EJECUTANDO EL SISTEMA
        /// <summary>
        /// Obtiene el sistema operativo desde el cual se está ejecutando el programa.
        /// <para>Nota: Extrae información desde regedit.</para>
        /// </summary>
        /// <returns></returns>
        public static string GetOperatingSystem()
        {
            RegistryKey reg = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows NT\\CurrentVersion");
            string pathname = (string)reg.GetValue("productName") + " " + (string)reg.GetValue("CSDVersion");

            return pathname;
        }

        public static string GetArquitectura()
        {
            string ar = "";

            if (Environment.Is64BitOperatingSystem)
                ar = "x64";
            else
                ar = "x86";

            return ar;
        }

       /// <summary>
       /// Indica si dos contraseñas son iguales.
       /// </summary>
       /// <param name="pClave">Contraseña.</param>
       /// <param name="pConfirma">Contraseña a comparar.</param>
       /// <returns></returns>
        public static bool ComparaContraseñas(string pClave, string pConfirma)
        {
            bool iguales = false;
            if (pClave.Length == 0) return false;
            if (pConfirma.Length == 0) return false;

            if (pClave.ToLower() == pConfirma.ToLower())
                iguales = true;

            return iguales;
        }

        /// <summary>
        /// Retorna la llave maestra del usuario.
        /// <para>Nota: La llave maestra es una segunda clave que se utiliza para reabrir un mes anterior.</para>
        /// </summary>
        /// <param name="pUser"></param>
        /// <returns></returns>
        public static string GetLlaveMaestra(string pUser)
        {
            string sql = "SELECT llavemaestra FROM usuario WHERE usuario=@pUser";           
            string key = "";
            SqlCommand cmd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pUser", pUser));

                        key = (string)cmd.ExecuteScalar();

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
            return key;
        }

        /// <summary>
        /// Actualiza o setea el ultimo registro visto por el usuario.
        /// </summary>
        /// <param name="pRegistro">Nuevo registro.</param>
        /// <param name="pUser">Usuario.</param>
        public static void UltimoRegistroVisto(string pRegistro, string pUser)
        {
            string sql = "UPDATE usuario SET lastview=@pRegistro WHERE usuario=@pUsuario";
            SqlCommand cmd;
            SqlConnection cn;
            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@pRegistro", pRegistro));
                            cmd.Parameters.Add(new SqlParameter("@pUsuario", pUser));

                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene el ultimo registro visualizado por el usuario.
        /// <para>Nota: El valor guardado en bd tiene el formato 'Contrato;Periodo'</para>
        /// <para>Nota2:Si opcion es 0: Devuelve el contrato.</para>
        /// <para>Nota3: Si opcion es 1: Devuelve el periodo.</para>
        /// </summary>
        /// <param name="pUser">Usuario.</param>
        /// <param name="option">Opcion.</param>
        /// <returns></returns>
        public static string GetLastView(string pUser, int? option = 0)
        {
            string sql = "SELECT lastview FROM usuario WHERE usuario=@pUsuario";
            string last = "";
            SqlCommand cmd;
            SqlConnection cn;
            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@pUsuario", pUser));

                            last = (string)cmd.ExecuteScalar();
                            cmd.Dispose();                            
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            if (last.Contains(";"))
            {
                string[] data = last.Split(';');
                if (data.Length > 0)
                {
                    //RETORNAMOS CONTRATO
                    if (option == 0)
                        last = data[0];
                    //RETORNAMOS PERIODO REGISTRO
                    if (option == 1)
                        last = data[1];
                }
            }

            return last;
        }

        /// <summary>
        /// Indica si el contrato y periodo registrado en la columna lastview del usuario
        /// es un contrato valido o existe en el periodo indicado.
        /// <para>Si el contrato existe retorna true</para>
        /// <para>Si el contrato no existe en periodo retorna false</para>
        /// </summary>
        public static bool VerificaLastView()
        {
            string sql = "SELECT count(*) FROM usuario " +
                         "WHERE lastview<> '0' " +
                         "AND substring(lastview, 0, charindex(';', lastview)) " +
                         "IN(SELECT contrato FROM trabajador " +
                         "WHERE anomes = RIGHT(lastview, (LEN(lastview) - CHARINDEX(';', lastview))) ) " +
                         " AND usuario = @pUser";
            SqlCommand cmd;
            SqlConnection cn;
            bool existe = false;

            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@pUser", getUser()));

                            object data = cmd.ExecuteScalar();
                            if (data != null)
                            {
                                if (Convert.ToInt32(data) > 0)
                                    existe = true;
                            }
                        }
                        cmd.Dispose();
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }

            return existe;

        }

        /// <summary>
        /// Indica si el usuario puede ver fichas con caracter de privadas.
        /// </summary>
        /// <returns></returns>
        public static bool ShowPrivadas()
        {
            bool show = false;
            string sql = "SELECT conf FROM usuario WHERE usuario=@pUser";
            SqlCommand cmd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pUser", user));

                        object data = cmd.ExecuteScalar();
                        if (data != DBNull.Value)
                            show = (bool)data;

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return show;
        }

        /// <summary>
        /// Limpiar el ultimo registro visto por el usuario.
        /// <para>Limpiar hace alución a dejar con valor 0 la columna.</para>
        /// </summary>
        public static void CleanLastView()
        {
            string sql = "UPDATE usuario SET lastview=0";
            SqlCommand cmd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Devuelve el nombre completo del usuario.
        /// </summary>
        /// <returns></returns>
        public static string NombreCompleto()
        {
            string name = "";
            string sql = "SELECT nombre FROM usuario WHERE usuario=@pUser";
            SqlCommand cmd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS    
                        cmd.Parameters.Add(new SqlParameter("@pUser", user));

                        object data = cmd.ExecuteScalar();
                        if (data != DBNull.Value)
                            name = (string)data;

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return name;
        }

        /// <summary>
        /// Verifica si el usuario está bloqueado para realizar procesos que implican cambios a nivel de base de datos.
        /// </summary>
        /// <returns></returns>
        public static bool Bloqueado()
        {
            bool bloq = false;
            string sql = "SELECT bloqueo FROM usuario WHERE usuario=@pUser";
            SqlCommand cmd;
            SqlConnection cn;
            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@pUser", user));

                            object data = cmd.ExecuteScalar();
                            if (data != DBNull.Value)
                                if (Convert.ToInt32(data) == 1)
                                    bloq = true;

                            cmd.Dispose();                           
                        }
                    }
                }

            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return bloq;
        }

       /// <summary>
       /// Desbloquea un usuario bloqueado.
       /// </summary>
       /// <param name="pUser">Usuario.</param>
       /// <returns></returns>
        public static bool Desbloquear(string pUser)
        {
            bool bloq = false;
            string sql = "UPDATE Usuario SET bloqueo=0 WHERE usuario=@pUser";
            SqlCommand cmd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pUser", pUser));

                        cmd.ExecuteNonQuery();

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return bloq;
        }

        /*VERIFICAR SI ALGUN USUARIO TIENE OTRO USUARIO QUE LO BLOQUEA*/
        /// <summary>
        /// Indica si un usuario bloquea a otro.
        /// </summary>
        /// <param name="pUser">Usuario a consultar.</param>
        /// <returns></returns>
        public static bool UsuarioBloquea(string pUser)
        {
            bool bloquea = false;
            string sql = "SELECT count(*) FROM usuario WHERE usrbloq=@pUser";
            SqlCommand cmd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql,fnSistema.sqlConn))
                    {
                        //PARAMETROS    
                        cmd.Parameters.Add(new SqlParameter("@pUser", pUser));

                        if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                            bloquea = true;

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return bloquea;
        }

        /// <summary>
        /// Entrega el nombre del usuario que está realizando un bloqueo
        /// </summary>
        public static string GetNombreBloqueo()
        {
            string sql = "SELECT usrbloq FROM usuario";
            SqlCommand cmd;
            SqlConnection cn;
            SqlDataReader rd;
            string us = "";
            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            rd = cmd.ExecuteReader();
                            if (rd.HasRows)
                            {
                                while (rd.Read())
                                {
                                    if ((string)rd["usrbloq"] != "0")
                                    {
                                        us = (string)rd["usrbloq"];
                                        break;
                                    }
                                }
                            }
                        }
                        rd.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                //ERROR...
            }

            return us;

        }
    }

    /// <summary>
    /// Permite el calculo de vacaciones de un trabajador.
    /// </summary>
    class vacaciones
    {
        /// <summary>
        /// Numero de contrato trabajador.
        /// </summary>
        public string contrato { get; set; }
        /// <summary>
        /// Dias legales acumalados por el total de años.
        /// </summary>
        public double DiasPropAnual { get; set; }
        /// <summary>
        /// Dias legales restantes (Fracion menor a un año).
        /// </summary>
        public double DiasPropRestantes { get; set; }
        /// <summary>
        /// Dias proporcionales usados.
        /// </summary>
        public double DiasPropTomados { get; set; }
        /// <summary>
        /// Dias proporcionales Anuales restantantes (Restando los dias ya usados.)
        /// </summary>
        public double DiasPropAnRestantes { get; set; }
        /// <summary>
        /// Total de dias proporcionales "Usables"
        /// </summary>
        public double TotalProp { get; set; }
        /// <summary>
        /// Dias progresivos obtenidos por el trabajador.
        /// </summary>
        public double diasProg { get; set; }
        /// <summary>
        /// Dias progresivos ya usados.
        /// </summary>
        public double diasProgTomados { get; set; }
        /// <summary>
        /// Total de dias progresivos restantes.
        /// </summary>
        public double TotalProg { get; set; }
        /// <summary>
        /// Total dias de vacaciones (Legales + Proporcionales.)
        /// </summary>
        public double TotalDias { get; set; }

        /// <summary>
        /// Fecha tope para calculo de vacaciones proporcionales.
        /// </summary>
        public DateTime FechaLimiteVacacionesLegales { get; set; }

        /// <summary>
        /// Fecha base para calculo de dias progresivos.
        /// </summary>
        public DateTime FechaBaseProgresivos { get; set; }

        public static List<VacacionDetalle> ListadoDetalle = new List<VacacionDetalle>();

        //SUMAR DIAS A UNA FECHA
        /// <summary>
        /// Agrega dias a una fecha determinada y devuelve la nueva fecha.
        /// </summary>
        /// <param name="inicial">Fecha inicial.</param>
        /// <param name="cantidad">Cantidad de dias.</param>
        /// <returns></returns>
        public static DateTime AgregarDias(DateTime inicial, int cantidad)
        {
            DateTime final = DateTime.Now.Date;
            final = inicial.AddDays(cantidad);

            return final;
        }

        //SUMAS DIAS SIN CONSIDERAR LOS FINES DE SEMANA 
        //RETORNA FECHA SUMANDO DIAS DE ACUERDO A PARAMETRO CANTIDAD SIN CONSIDERAR LOS FINES DE SEMANA
        /// <summary>
        /// Agrega dias a una fecha determinada sin considerar los fines de semana y devuelve la nueva fecha.
        /// </summary>
        /// <param name="inicial">Fecha inicial.</param>
        /// <param name="cantidad">Cantidad de dias.</param>
        /// <returns></returns>
        public static DateTime AgregarDiasFinSemana(DateTime inicial, double cantidad)
        {
            DateTime final = DateTime.Now.Date;
            DateTime retorna = DateTime.Now.Date;
            DateTime auxiliar = DateTime.Now.Date;
            //PREGUNTAR SI EL DIA ES FIN DE SEMANA

            if (cantidad <= 1)
            {
                return inicial;
            }
          
            //SUMA DIAS A LA FECHA INCIAL (fecha tope)
            final = inicial.AddDays(cantidad - 1);

            //RECORREMOS DESDE FECHA INICIO HASTA TOPE Y BUSCAMOS FINES DE SEMANA
            int finde = 0;
            auxiliar = inicial;
            //ITERAMOS LA FECHAS FECHAS ENTRE LA FECHA DE INICIO Y LA FECHA DE TERMINO
            //RANGO DE FECHAS Y CONTAMOS TODAS LAS APARICIONES DE SABADOS Y DOMINGO QUE HAY ENTRE LAS DOS FECHAS!!

            while (inicial <= final)
            {
                if (inicial.DayOfWeek == DayOfWeek.Saturday || inicial.DayOfWeek == DayOfWeek.Sunday || EsFeriado(inicial))
                {
                    finde++;
                }
                inicial = inicial.AddDays(1);
            }

            inicial = inicial.AddDays(-1);
            //UNA VEZ ENCONTRADOS TODOS LOS FINES DE SEMANA...
            if (finde == 0)
            {
                //NO HAY FINES DE SEMANA ENTRE MEDIO
                retorna = inicial;
            }
            else if (finde > 0)
            {
                //SI HAY FINES DE SEMANA CORRIMOS CANTIDAD DE DIAS
                //finde = finde + (days);

                //retorna = auxiliar.AddDays(finde);

                //AGREGAMOS LA CANTIDAD DE DIAS EQUIVALENTES A FINES DE SEMANA ENCONTRADO
                //PERO DEBEMOS VERIFICAR QUE EL DIA SEA UN DIA HABIL Y NO SABADO NI DOMINGO Y TAMPOCO SEA DIA FERIADO
                int x = 0;

                //USAMOS VARIABLE AUXILIAR Y GUARDAMOS INICIALMENTE LA FECHA INICIAL + 1 DIA
                auxiliar = inicial.AddDays(1);

                while (x < finde)
                {
                    if (auxiliar.DayOfWeek != DayOfWeek.Saturday && auxiliar.DayOfWeek != DayOfWeek.Sunday && EsFeriado(auxiliar) == false)
                    {
                        //SI ES DISTINTO DE SABADO Y DOMINGO Y NO ES FERIADO AGREGAMOS UN DIA A LA FECHA
                        inicial = auxiliar;
                        x++;
                        auxiliar = auxiliar.AddDays(1);
                    }
                    else
                    {
                        //ES FIN DE SEMANA
                        //SEGUIMOS AUMENTANDO DIAS
                        auxiliar = auxiliar.AddDays(1);
                    }
                }
            }
            //HAY QUE CONSIDERAR QUE LA FECHA DE TERMINO DEBE CONSIDERAR EL DIA INICIAL POR LO QUE RESTAREMOS UN DIA
            //A LA FECHA DE RETORNO            
            return (inicial);
        }

        /// <summary>
        /// Retorna la fecha en la cual el trabajador debe retornar al trabajo luego de cumplir sus vacaciones.
        /// <para>Nota: Se considera el primer día habil siguiente al termino de las vacaciones.</para>
        /// </summary>
        /// <param name="inicial">Fecha inicial</param>
        /// <param name="cantidad">Cantidad de dias.</param>
        /// <returns></returns>
        public static DateTime DiaRetornoTrabajo(DateTime inicial, double cantidad, int pJornada)
        {
            //SI LA JORNADA ES COD 2 (LUNES - SABADO) PODRÍA RETORNAR AL TRABAJO EL DÍA SÁBADO
            //SI LA JORNADA ES COD 1 (LUNES - SÁBADO) SOLO DÍAS HÁBILES (NO CONSIDERAR SABADO, DOMINGO O FESTIVOS).        
            //SI ES POR TURNOS CONSIDERAMOS SABADOS Y DOMINGOS?

            DateTime retorno = DateTime.Now.Date;
            //OBTENER EL ULTIMO DIA EN QUE SE TERMINA LAS VACACIONES
            DateTime DiaTerminoVacacion = DateTime.Now.Date;
            DiaTerminoVacacion = AgregarDiasFinSemana(inicial, cantidad);

            //SUMAMOS UN DIA AL DIA DE TERMINO DE VACACIONES
            DateTime auxiliar = DateTime.Now.Date;
            auxiliar = DiaTerminoVacacion;

            //LUNES - VIERNES
            if (pJornada == 1)
            {
                do
                {
                    //RECORREMOS HASTA ENCONTRAR EL PRÓXIMO DIA HABIL
                    auxiliar = auxiliar.AddDays(1);
                } while (auxiliar.DayOfWeek == DayOfWeek.Saturday || auxiliar.DayOfWeek == DayOfWeek.Sunday || EsFeriado(auxiliar));
            }

            //LUNES - SABADO
            if (pJornada == 2)
            {
                do
                {
                    //RECORREMOS HASTA ENCONTRAR EL PRÓXIMO DIA HABIL
                    auxiliar = auxiliar.AddDays(1);
                } while (auxiliar.DayOfWeek == DayOfWeek.Sunday || EsFeriado(auxiliar));
            }

            //TURNOS
            if (pJornada == 3)
            {
                auxiliar = auxiliar.AddDays(1);
            }

            //RETORNAMOS LA FECHA FINAL
            return auxiliar;
        }

        /// <summary>
        /// Cantidad de dias no habiles dentro de un rango de fechas.
        /// </summary>
        /// <param name="inicial">Fecha inicial.</param>
        /// <param name="cantidad">Cantidad de dias agregados a la fechas de inicio.</param>
        /// <returns></returns>
        public static int DiasNoHabiles(DateTime Inicial, double cantidad)
        {
            int count = 0;
            int Nohab = 0;
            while (count < cantidad)
            {
                //AGREGAMOS UN DIA                              
                if (Inicial.DayOfWeek != DayOfWeek.Saturday && Inicial.DayOfWeek != DayOfWeek.Sunday)
                {
                    count++;
                    if (count < cantidad)
                        Inicial = Inicial.AddDays(1);
                }
                else
                {
                    Nohab++;
                    Inicial = Inicial.AddDays(1);
                }
            }

            return Nohab;
        }

        //CANTIDAD DE DIAS QUE HAY ENTRE DOS FECHAS
        /// <summary>
        /// Retorna la cantidad de dias que hay entre un rango de fechas.
        /// <para>Nota: Considerar que la fecha de termino debe ser mayor a la fecha de inicio.</para>
        /// </summary>        
        /// <param name="inicio">Fecha inicial.</param>
        /// <param name="termino">Fecha final.</param>
        /// <returns></returns>
        public static int DiferenciaFechas(DateTime inicio, DateTime termino)
        {
            TimeSpan x;
            int dias = 0;
            x = termino.Subtract(inicio);

            dias = x.Days;

            return (dias + 1);
        }

        //DIAS PROPORCIONALES A LA CANTIDAD DE DIAS TRABAJADOS
        //EN UN AÑO SON 15 DIAS DE VACACIONES
        //EN OTRAS PALABRAS A 365 DIAS LE CORRESPONDEN 15 DIAS DE VACACIONES
        /// <summary>
        /// Calcula y retorna la cantidad de vacaciones proporcionales que tiene el trabajador en un determinado rango de fechas.
        /// </summary>
        /// <param name="FechaInicioProgresivo">Fecha de incio desde ficha.</param>
        /// <param name="FechaLimite">Fecha tope.</param>
        /// <returns></returns>
        public static double VacacionesProporcionales(DateTime FechaInicioProgresivo, DateTime FechaLimite)
        {        
            double cantidad = 0.0;
            double diasTrabajados = 0;
            int year = 0, diasYear = 0;
            DateTime First = DateTime.Now.Date;
            DateTime Last = DateTime.Now.Date;

            year = DateTime.Now.Date.Year;
            First = new DateTime(year, 1, 1);
            Last = new DateTime(year, 12, 31);
            diasYear = DiasYear(First, Last);
            
            //SUPONGAMOS QUE CONSIDERAMOS HASTA LA FECHA DE HOY (AÑO ACTUAL)

            //NECESITAMOS LOS DIAS TRABAJADOS EN EL AÑO POR EL EMPLEADO
            //SUPONIENDO QUE NO TIENE LICENCIAS NI FALTAS

            diasTrabajados = DiferenciaFechas(FechaInicioProgresivo, FechaLimite);            
            //cantidad = ((1.25/30) * diasTrabajados);
            cantidad = Convert.ToDouble((15 * diasTrabajados) / diasYear);
            
            return cantidad;
        }

        /// <summary>
        /// Obtiene la cantidad de dias que hay de feriado proporcional entre la fecha de inicio de contrato y la fecha de termino de contrato.
        /// </summary>
        /// <param name="Inicio">Fecha de inicio de contrato</param>
        /// <param name="Termino">Fecha termino de contrato</param>
        public static double FeriadosProp(DateTime Inicio, DateTime Termino)
        {
            int years = 0, month = 0, days = 0;
            DateTime Auxiliar = DateTime.Now.Date;
            //Corresponde a los que se paga por mes de vacaciones
            double MesF = 1.25;
            //Corresponde a lo que se paga por año de vacaciones.
            double AnioF = 15;
            //Corresponde a lo que se paga por dia de vacaciones (1.25 / 30)
            double diaF = (MesF / 30);            
          
            VacacionDetalle RowDetalle = new VacacionDetalle();

            double TotalDias = 0;            

            if (Inicio < Termino)
            {
                Auxiliar = Inicio;
                //CANTIDAD DE AÑOS
                while (Auxiliar <= Termino)
                {
                    //AGREGAMOS Y CONTAMOS AÑOS
                    Auxiliar = Auxiliar.AddYears(1);
                    if (Auxiliar <= Termino)
                    {
                        years++;
                        //Agregamos datos a objeto
                        ListadoDetalle.Add(new VacacionDetalle() { Periodo=Auxiliar.Year, DiasPropGanados=AnioF, EsAnual = true});
                    }                        
                    else
                    {
                        Auxiliar = Auxiliar.AddYears(-1);
                        break;
                    }
                }

                //VERIFICAMOS SI LA FECHA EN QUE QUEDAMOS SIGUE SIENDO MENOR A LA FECHA DE TERMINO
                if (Auxiliar < Termino)
                {
                    //AGREGAMOS MESES Y CONTAMOS
                    while (Auxiliar <= Termino)
                    {
                        Auxiliar = Auxiliar.AddMonths(1);
                        if (Auxiliar <= Termino)
                            month++;
                        else
                        {
                            Auxiliar = Auxiliar.AddMonths(-1);
                            break;
                        }
                    }
                }

                //SI AUN QUEDAN DIAS LOS CONTAMOS
                //SUMAR 1 YA QUE NO CONSIDERA LOS EXTREMOS
                if (Auxiliar < Termino)
                {
                    TimeSpan d = Termino - Auxiliar;
                    days = (d.Days) + 1;
                }                                

                //CALCULAMOS LOS PROPORCIONALES POR AÑO, MES Y DIA.
                double t = (AnioF * years) + (MesF * month) + (diaF * days);
                TotalDias = Math.Round((AnioF * years) + (MesF * month) + (diaF * days), 2);
                
                ListadoDetalle.Add(new VacacionDetalle() { Periodo=Auxiliar.Year, DiasPropGanados = (MesF * month) + (diaF * days), TotalDiasLegales = TotalDias});               

            }
            return TotalDias;
        }

        /// <summary>
        /// Nos entrega el periodo al que corresponden los dias a usar de vacaciones.
        /// </summary>
        /// <param name="Inicio">Fecha de inicio de contabilizacion de vacaciones.</param>
        /// <param name="Termino">Fecha termino o fecha tope</param>
        /// <param name="pDiasTomados">Dias acumulados usados.</param>
        /// <param name="pDiasAUsar">Dias a usar.</param>
        /// <returns></returns>
        public static string PeriodoDiasTomados(DateTime Inicio, DateTime Termino, double pDiasTomados, double pDiasAUsar)
        {
            int years = 0, month = 0, days = 0;
            bool Encontrato = false;
            DateTime Auxiliar = DateTime.Now.Date;
            //Corresponde a los que se paga por mes de vacaciones
            double MesF = 1.25;
            //Corresponde a lo que se paga por año de vacaciones.
            double AnioF = 15;
            //Corresponde a lo que se paga por dia de vacaciones (1.25 / 30)
            double diaF = (MesF / 30);

            double TotalDias = 0;

            //ACUMULADOR DIAS GANADOS POR AÑO
            double SumGanados = 0;
            //Indica la cantida de dias disponibles en un año especifico
            double DiasDispYear = 0;
            string YearVac = "";

            if (Inicio < Termino)
            {
                Auxiliar = Inicio;
                //CANTIDAD DE AÑOS
                while (Auxiliar <= Termino)
                {
                    //AGREGAMOS Y CONTAMOS AÑOS
                    Auxiliar = Auxiliar.AddYears(1);
                    if (Auxiliar <= Termino)
                    {
                        years++;
                        //ACUMULAMOS AÑOS GANADOS
                        SumGanados = SumGanados + AnioF;
                        //Total dias usados es mayor a SumGanados???
                        //SumGanados es mayor a los dias tomados???
                        //SI CORREsponde hay dias disponibles
                        if (SumGanados > pDiasTomados)
                        {
                            DiasDispYear = SumGanados - pDiasTomados;                            
                            //FALTARINA DIAS..
                            YearVac = YearVac + ";" + $"{Auxiliar.Year}";

                            if (DiasDispYear >= pDiasAUsar)
                            {
                                Encontrato = true;
                                break;                                
                            }                                
                        }                        
                    }
                    else
                    {
                        Auxiliar = Auxiliar.AddYears(-1);
                        break;
                    }
                }

                //SI AUN QUEDA PERIODO POR REVISAR SERIA EL ULTIMO AÑO
                if (Auxiliar < Termino && Encontrato == false)
                    YearVac = $"{Auxiliar.Year}";                                      

            }

            if (YearVac.Length > 0)
                if(YearVac[0] == ';')
                    YearVac = YearVac.Substring(1, YearVac.Length - 1);

            //RETORNAMOS CADENA
            return YearVac;
        }

        /// <summary>
        /// Devuelve la cantidad de años que hay entre un rango de fechas.
        /// </summary>
        /// <param name="inicio">Fecha inicio.</param>
        /// <param name="termino">Fecha de termino.</param>
        /// <returns></returns>
        public static int CantidadAniosFechas(DateTime inicio, DateTime termino)
        {
            int cantidad = 0;
            cantidad = termino.Year - inicio.Year;

            return cantidad;
        }

        //OBTENER LA CANTIDAD DE DIAS PROGRESIVOS QUE TIENE LA PERSONA
        //SUPONEMOS QUE YA TIENE LOS 10 AÑOS PREVIOS
        /// <summary>
        /// Calcula y retorna el total de vacaciones progresivas acumuladas.
        /// </summary>
        /// <param name=pFechaInicio">Fecha inicio desde donde se empiezan a calcular los progresivos.</param>
        /// <param name="pFechaProgresivo"> Fecha entrega certificado desde el cual se consideran los dias progresivos.</param>
        /// <param name="ConsideraProg">Si es true consideramos el dia progresivo como válido.</param>
        /// <returns></returns>
        public static int Progresivos(DateTime pFechaInicio, DateTime pFechaProgresivo, bool? ConsideraProg = false)
        {
            DateTime today = DateTime.Now.Date;
            //DEBEMOS SUMANDO DIAS A PARTIR DEL AÑOS NUMERO 3
            //IR SUMANDO UN AÑO POR CADA ITERACION

            int years = 0;
            int progre = 0;
            int factor = 1;
            int contador = 0;

            while (pFechaInicio <= today)
            {
                if (years % 3 == 0 && years != 0)
                {
                    if (contador == 2)
                    {
                        //SI CONTADOR ES IGUAL A TRES ES PORQUE HAN PASADO 3 AÑOS
                        //RESETEAMOS EL CONTADOR DE AÑOS
                        contador = 0;
                        //AUMENTAMOS EN UNO EL FACTOR
                        factor++;
                    }

                    //SOLO CONSIDERAMOS LOS PROGRESIVOS CUYA FECHA SEA MAYOR O IGUAL A LA FECHA DE ENTREGA CERTIFICADO (FECHA PROGRESIVO)
                    if ((bool)ConsideraProg)
                    {
                        if (pFechaInicio >= pFechaProgresivo)
                            progre = progre + factor;
                    }
                    else
                        progre = progre + factor;

                }
                else if (years > 3)
                {
                    if ((bool)ConsideraProg)
                    {
                        if (pFechaInicio >= pFechaProgresivo)
                            progre = progre + factor; 
                    }
                    else
                        progre = progre + factor;

                    contador = contador + 1;
                }

                //AGREGAMOS UN AÑO A LA FECHA
                pFechaInicio = pFechaInicio.AddYears(1);

                years = years + 1;
            }

            return progre;
        }

        //OBTENER LA CANTIDAD DE DIAS PROGRESIVOS QUE TIENE LA PERSONA
        //SUPONEMOS QUE YA TIENE LOS 10 AÑOS PREVIOS
        /// <summary>
        /// Calcula y retorna el total de vacaciones progresivas hasta una fecha final determinada por el usario
        /// </summary>
        /// <param name=pFechaInicio">Fecha inicio desde donde se empiezan a calcular los progresivos.</param>
        /// <param name="pFechaProgresivo"> Fecha entrega certificado desde el cual se consideran los dias progresivos.</param>
        /// <param name="ConsideraProg">Si es true consideramos el dia progresivo como válido.</param>
        /// <returns></returns>
        public static int ProgresivosEstimados(DateTime pFechaInicio, DateTime pFechaProgresivo, DateTime pFechaLimite, bool? ConsideraProg = false)
        {
            DateTime today = pFechaLimite;
            //DEBEMOS SUMANDO DIAS A PARTIR DEL AÑOS NUMERO 3
            //IR SUMANDO UN AÑO POR CADA ITERACION

            int years = 0;
            int progre = 0;
            int factor = 1;
            int contador = 0;

            while (pFechaInicio <= today)
            {
                if (years % 3 == 0 && years != 0)
                {
                    if (contador == 2)
                    {
                        //SI CONTADOR ES IGUAL A TRES ES PORQUE HAN PASADO 3 AÑOS
                        //RESETEAMOS EL CONTADOR DE AÑOS
                        contador = 0;
                        //AUMENTAMOS EN UNO EL FACTOR
                        factor++;
                    }

                    //SOLO CONSIDERAMOS LOS PROGRESIVOS CUYA FECHA SEA MAYOR O IGUAL A LA FECHA DE ENTREGA CERTIFICADO (FECHA PROGRESIVO)
                    if ((bool)ConsideraProg)
                    {
                        if (pFechaInicio >= pFechaProgresivo)
                            progre = progre + factor;
                    }
                    else
                        progre = progre + factor;

                }
                else if (years > 3)
                {
                    if ((bool)ConsideraProg)
                    {
                        if (pFechaInicio >= pFechaProgresivo)
                            progre = progre + factor;
                    }
                    else
                        progre = progre + factor;

                    contador = contador + 1;
                }

                //AGREGAMOS UN AÑO A LA FECHA
                pFechaInicio = pFechaInicio.AddYears(1);

                years = years + 1;
            }

            return progre;
        }

        public static string MesProgresivoUsado(DateTime pFechaInicio, DateTime pFechaProgresivo, double pDiasTomados, double pDiasAUsar, bool? ConsideraProg = false)
        {
            DateTime today = DateTime.Now.Date;
            //DEBEMOS SUMANDO DIAS A PARTIR DEL AÑOS NUMERO 3
            //IR SUMANDO UN AÑO POR CADA ITERACION

            int years = 0;
            int progre = 0;
            int factor = 1;
            int contador = 0;

            //ACUMULADOR DIAS GANADOS POR AÑO
            double SumGanados = 0;
            //Indica la cantidad de dias disponibles en un año especifico
            double DiasDispYear = 0;
            string YearVac = "";
            bool Encontrato = false;

            while (pFechaInicio <= today)
            {
                if (years % 3 == 0 && years != 0)
                {
                    if (contador == 2)
                    {
                        //SI CONTADOR ES IGUAL A TRES ES PORQUE HAN PASADO 3 AÑOS
                        //RESETEAMOS EL CONTADOR DE AÑOS
                        contador = 0;
                        //AUMENTAMOS EN UNO EL FACTOR
                        factor++;
                    }

                    //SOLO CONSIDERAMOS LOS PROGRESIVOS CUYA FECHA SEA MAYOR O IGUAL A LA FECHA DE ENTREGA CERTIFICADO (FECHA PROGRESIVO)
                    if ((bool)ConsideraProg)
                    {
                        if (pFechaInicio >= pFechaProgresivo)
                            progre = progre + factor;

                        //ACUMULAMOS AÑOS GANADOS
                        SumGanados = SumGanados + factor;
                        //Total dias usados es mayor a SumGanados???
                        //SumGanados es mayor a los dias tomados???
                        //SI CORREsponde hay dias disponibles
                        if (SumGanados > pDiasTomados)
                        {
                            DiasDispYear = SumGanados - pDiasTomados;
                            //FALTARINA DIAS..
                            YearVac = YearVac + ";" + $"{pFechaInicio.Year}";

                            if (DiasDispYear >= pDiasAUsar)
                            {
                                Encontrato = true;
                                break;
                            }
                        }

                    }
                    else
                        progre = progre + factor;

                }
                else if (years > 3)
                {
                    //SE CONSIDERA PROGRESIVO?
                    if ((bool)ConsideraProg)
                    {
                        if (pFechaInicio >= pFechaProgresivo)
                            progre = progre + factor;

                        //ACUMULAMOS AÑOS GANADOS
                        SumGanados = SumGanados + factor;
                        //Total dias usados es mayor a SumGanados???
                        //SumGanados es mayor a los dias tomados???
                        //SI CORREsponde hay dias disponibles
                        if (SumGanados > pDiasTomados)
                        {
                            DiasDispYear = SumGanados - pDiasTomados;
                            //FALTARINA DIAS..
                            YearVac = YearVac + ";" + $"{pFechaInicio.Year}";

                            if (DiasDispYear >= pDiasAUsar)
                            {
                                Encontrato = true;
                                break;
                            }
                        }
                    }
                    else
                        progre = progre + factor;

                    contador = contador + 1;
                }

                //AGREGAMOS UN AÑO A LA FECHA
                pFechaInicio = pFechaInicio.AddYears(1);

                years = years + 1;
            }

            if (YearVac.Length > 0)
                if(YearVac[0] == ';')
                    YearVac = YearVac.Substring(1, YearVac.Length - 1);

            return YearVac;
        }

        //OBTENER LA FECHA DEL PROXIMO AÑO
        //EJEMPLO: SI LA FECHA DE ENTRADA ES 12-01-2012 --> LA FECHA DE SALIDA SERIA => 12-01-2013
        /// <summary>
        /// Suma un año a una fecha.
        /// </summary>
        /// <param name="fecha">Fecha.</param>
        /// <returns></returns>
        public static DateTime SumaYear(DateTime fecha)
        {
            DateTime salida = DateTime.Now.Date;

            salida = fecha.AddYears(1);

            return salida;
        }

        //FUNCION PARA OBTENER EL ULTIMO AÑO DE PROPORCIONAL QUE LE CORRESPONDE AL TRABAJADOR
        //...
        /// <summary>
        /// Retorna el ultimo año proporcional de un trabajador.
        ///<para>Nota: Consideramos hasta la fecha en que se cumple el ultimo año hasta la fecha actual.</para>
        /// </summary>
        /// <param name="FechaProgresivo">Fecha progresivo.</param>
        /// <param name="pLimiteOpcional">Fecha limite para el caso de queramos limitar las fecha tope.</param>
        /// <returns></returns>
        public static DateTime UltimoAnioProporcional(DateTime FechaProgresivo, DateTime? pLimiteOpcional = null)
        {
            DateTime ultimo = DateTime.Now.Date;

            if (pLimiteOpcional != null)
                ultimo = Convert.ToDateTime(pLimiteOpcional);

            DateTime aux = DateTime.Now.Date;

            while (FechaProgresivo < ultimo)
            {
                FechaProgresivo = FechaProgresivo.AddYears(1);
                if (FechaProgresivo <= ultimo)
                {
                    aux = FechaProgresivo;
                    // proporcionales = proporcionales + 15;
                }
            }

            return aux;
        }

        /// <summary>
        /// Indica si una fecha es feriado.
        /// </summary>
        /// <param name="Fecha">Fecha.</param>
        /// <returns></returns>
        public static bool EsFeriado(DateTime Fecha)
        {
            bool feriado = false;
            string sql = "SELECT fecha FROM feriado WHERE fecha=@fecha";
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@fecha", Fecha));

                        try
                        {
                            rd = cmd.ExecuteReader();
                            if (rd.HasRows)
                            {
                                //SI RETORNA FILAS ES PORQUE LA FECHA ES UN FERIADO
                                feriado = true;
                            }
                            else
                            {
                                feriado = false;
                            }

                            cmd.Dispose();
                            rd.Close();
                            fnSistema.sqlConn.Close();
                        }
                        catch (SqlTypeException ex)
                        {
                            XtraMessageBox.Show(ex.Message);
                        }
                    }
                   
                }
            }
            catch (SqlException ex)
            {
                //EXCEPTION
                    
            }
            return feriado;
        }

        /// <summary>
        /// Retorna los dias restantes proporcionales acumulados por año.
        /// </summary>
        /// <param name="totalDias">Total dias anuales acumulados.</param>
        /// <param name="diasUsados">Dias anuales ya usados.</param>
        /// <returns></returns>
        public static double DiasAnualesRestantes(double totalDias, double diasUsados)
        {
            double restantes = 0;
            restantes = totalDias - diasUsados;

            return restantes;
        }

        //SABER SI APLICA PROPORCIONAL ANUAL
        /// <summary>
        /// Indica si aplica calcular vacaciones anuales proporcionales.
        /// <para>Nota: Si al sumar un año a la fecha de entrada, esta es superior a la fecha actual no aplica.</para>
        /// </summary>
        /// <param name="FechaProgresivo"></param>
        /// <returns></returns>
        public static bool AplicaProporcionalAnual(DateTime FechaProgresivo)
        {
            bool aplica = false;
            DateTime today = DateTime.Now.Date;
            DateTime auxiliar = DateTime.Now.Date;
            //SUMAR UN AÑO AL AÑO DE PROGRESIVO
            auxiliar = FechaProgresivo.AddYears(1);

            //SI AL SUMAR UN AÑO LA FECHA ES SUPERIOR AL DIA DE HOY NO APLICA AÑO PROPORCIONAL
            if (auxiliar > today)
                aplica = false;
            else
                aplica = true;

            return aplica;
        }

        /// <summary>
        /// Obtiene la cantidad de dias que tiene un año.
        /// </summary>
        /// <param name="First">Primer dia del año.</param>
        /// <param name="Last">Ultimo dia del año.</param>
        /// <returns></returns>
        public static int DiasYear(DateTime First, DateTime Last)
        {
            int dias = 0;
            var diferencia = Last - First;
            dias = (diferencia.Days)+1;
            return dias;
        }

        //--------------------PARA EMISION DE COMPROBANTE DE VACACIONES ------------------------------------
        //OBTENER LA CANTIDAD DE DIAS PROGRESIVOS QUE TIENE LA PERSONA
        //SUPONEMOS QUE YA TIENE LOS 10 AÑOS PREVIOS
        /// <summary>
        /// Emite comprobante de vacaciones.
        /// </summary>
        /// <param name="FechaProgresivo"></param>
        /// <param name="FechaTope"></param>
        /// <returns></returns>
        public static int ProgresivosComprobante(DateTime FechaProgresivo, DateTime FechaTope)
        {
            DateTime today = DateTime.Now.Date;
            //DEBEMOS SUMANDO DIAS A PARTIR DEL AÑOS NUMERO 3
            //IR SUMANDO UN AÑO POR CADA ITERACION

            int years = 0;
            int progre = 0;
            int factor = 1;
            int contador = 0;

            while (FechaProgresivo <= FechaTope)
            {
                if (years % 3 == 0 && years != 0)
                {
                    if (contador == 2)
                    {
                        //SI CONTADOR ES IGUAL A TRES ES PORQUE HAN PASADO 3 AÑOS
                        //RESETEAMOS EL CONTADOR DE AÑOS
                        contador = 0;
                        //AUMENTAMOS EN UNO EL FACTOR
                        factor++;
                    }
                    progre = progre + factor;
                }
                else if (years > 3)
                {

                    progre = progre + factor;
                    contador = contador + 1;
                }

                //AGREGAMOS UN AÑO A LA FECHA
                FechaProgresivo = FechaProgresivo.AddYears(1);

                years = years + 1;
            }

            return progre;
        }

        //FUNCION PARA OBTENER EL ULTIMO AÑO DE PROPORCIONAL QUE LE CORRESPONDE AL TRABAJADOR
        //...
        /// <summary>
        /// Retorna el ultimo año proporcional que le corresponde a un trabajador.
        /// </summary>
        /// <param name="FechaProgresivo">Fecha progresivo.</param>
        /// <param name="FechaTope">Fecha Tope.</param>
        /// <returns></returns>
        public static DateTime UltimoAnioProporcionalComprobante(DateTime FechaProgresivo, DateTime FechaTope)
        {
            DateTime ultimo = FechaTope;
            DateTime aux = DateTime.Now.Date;

            while (FechaProgresivo < ultimo)
            {
                FechaProgresivo = FechaProgresivo.AddYears(1);
                if (FechaProgresivo <= ultimo)
                {
                    aux = FechaProgresivo;
                    // proporcionales = proporcionales + 15;
                }
            }

            //SI LA FECHA DE AUXILIAR SIGUE SIENDO LA FECHA DE HOY USAMOS LA FECHA DE TOPE
            if (aux == DateTime.Now.Date)
                aux = FechaTope;

            return aux;
        }

        /*VERIFICAR SI UN RANGO DE FECHAS ESTAN DENTRO*/
        /*FECHA INICIAL Y FECHA FINAL --> EJ: [02-02-2019];[04-02-2019]*/
        /// <summary>
        /// Indica si una fecha esta dentro de un rango de fechas.
        /// </summary>
        /// <param name="pFechaInicio">Fecha inicio rango.</param>
        /// <param name="pFechaTermino">Fecha termino rango.</param>
        /// <param name="pContrato">Numero de contrato. trabajador.</param>
        /// <returns></returns>
        public static bool FechaOcupada(DateTime pFechaInicio, DateTime pFechaTermino, string pContrato)
        {
            string sql = "SELECT salida, finaliza FROM vacaciondetalle WHERE contrato = @pContrato";
            SqlCommand cmd;
            SqlConnection cn;
            SqlDataReader rd;
            DateTime salida = DateTime.Now.Date;
            DateTime finaliza = DateTime.Now.Date;
            DateTime FechaItera = DateTime.Now.Date;
            bool novalido = false;
            
            //SOLO PARA ITERAR
            FechaItera = pFechaInicio;
            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@pContrato", pContrato));

                            rd = cmd.ExecuteReader();
                            if (rd.HasRows)
                            {
                                while (rd.Read())
                                {
                                    salida = Convert.ToDateTime(rd["salida"]);
                                    finaliza = Convert.ToDateTime(rd["finaliza"]);

                                    if (salida == pFechaInicio || salida == pFechaTermino)
                                    { novalido = true; break; }

                                    if (finaliza == pFechaTermino || finaliza == pFechaInicio)
                                    { novalido = true; break; }

                                    //FECHA DE INICIO ESTÁ DENTRO DEL RANGO DE FECHAS ENTRE SALIDA Y FINALIZA
                                    if (pFechaInicio >= salida && pFechaInicio <= finaliza)
                                    { novalido = true; break; }

                                    if (pFechaTermino >= salida && pFechaTermino <= finaliza)
                                    { novalido = true; break; }

                                    //ITERAMOS RANGO FECHAS ENTRE SALIDA Y FINALIZA
                                    //EJEMPLO: 12-03-2019 AL 20-03-2019
                                    while (salida <= finaliza)
                                    {
                                        //ITERAMOS FECHAS ENTRE pFechaInicio y pFechaTermino
                                        while (pFechaInicio <= pFechaTermino)
                                        {
                                            if (pFechaInicio == salida)
                                            { novalido = true; break; }

                                            //AUMENTAMOS EN UN DIA LA FECHA 
                                            pFechaInicio = pFechaInicio.AddDays(1);
                                        }

                                        if (novalido)
                                            break;

                                        //AGREGAMOS UN DIA A LA FECHA   
                                        salida = salida.AddDays(1);

                                        //RESETEAR fecha inicio parametro
                                        pFechaInicio = FechaItera;
                                    }
                                }
                            }
                        }
                        cmd.Dispose();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return novalido;

        }

        /// <summary>
        /// Indica la cantidad de dias, progresivos o proporcionales usados por el trabajador.
        /// </summary>
        /// <param name="contrato">Numero contrato asociado al trabajador.</param>
        /// <returns>Retorna un hashtable</returns>
        public static Hashtable diasUsados(string contrato, DateTime? pFechaLimite = null)
        {
            //TABLA HASH PARA GUARDAR LA CANTIDAD DE DIAS PROGRESIVOS Y LA CANTIDAD DE DIAS PROPORCIONALES USADOS
            string sql = "SELECT dias, tipo FROM VACACIONDETALLE WHERE contrato = @contrato";

            if (pFechaLimite != null)
            {
                sql = sql + $" AND finaliza <= @pFecha";
            }

            SqlCommand cmd;
            SqlDataReader rd;
            Hashtable tablaDatos = new Hashtable();
            double sumaProp = 0, sumaProg = 0;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@contrato", contrato));
                        if (pFechaLimite != null)
                            cmd.Parameters.Add(new SqlParameter("@pFecha", pFechaLimite));

                        rd = cmd.ExecuteReader();

                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                if ((Int16)rd["tipo"] == 1)
                                {

                                    //SON PROPORCIONALES
                                    sumaProp = sumaProp + Convert.ToDouble((decimal)rd["dias"]);

                                }
                                else if ((Int16)rd["tipo"] == 2)
                                {
                                    //SON PROGRESIVOS
                                    sumaProg = sumaProg + Convert.ToDouble((decimal)rd["dias"]);
                                }

                            }
                        }
                    }
                    cmd.Dispose();
                    rd.Close();
                    fnSistema.sqlConn.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            //AGREGAMOS SUMAS A HASH
            tablaDatos.Add("proporcional", sumaProp);
            tablaDatos.Add("progresivo", sumaProg);

            return tablaDatos;
        }

        #region "FERIADO COLECTIVO"
        /// <summary>
        /// Permite guardar una vacacion.
        /// </summary>
        /// <param name="pContrato"></param>
        /// <param name="pSalida"></param>
        /// <param name="pFinaliza"></param>
        /// <param name="pDias"></param>
        /// <param name="pTipo"></param>
        /// <param name="pRetorna"></param>
        /// <param name="pFolio"></param>
        public static void GuardarVacaciones(string pContrato, DateTime pSalida, DateTime pFinaliza, double pDias, int pTipo, DateTime pRetorna, int pFolio)
        {
            string sql = "INSERT INTO vacaciondetalle(contrato, salida, finaliza, dias, tipo, retorna, folio) " +
                         "VALUES(@pContrato, @pSalida, @pFinaliza, @pDias, @pTipo, @pRetorna, @pFolio)";
            SqlCommand cmd;
            SqlConnection cn;            

            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            cmd.Parameters.Add(new SqlParameter("@pContrato", pContrato));
                            cmd.Parameters.Add(new SqlParameter("@pSalida", pSalida));
                            cmd.Parameters.Add(new SqlParameter("@pFinaliza", pFinaliza));
                            cmd.Parameters.Add(new SqlParameter("@pDias", pDias));
                            cmd.Parameters.Add(new SqlParameter("@pTipo", pTipo));
                            cmd.Parameters.Add(new SqlParameter("@pRetorna", pRetorna));
                            cmd.Parameters.Add(new SqlParameter("@pFolio", pRetorna));

                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                            
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                //ERROR...
            }

        }
        /// <summary>
        /// Obtiene el ultimo numero de folio ingresado.
        /// </summary>
        /// <param name="pContrato"></param>
        public static int GetLastFolio(string pContrato)
        {
            string sql = "SELECT ISNULL(MAX(folio), 0) FROM vacacionDetalle WHERE contrato=@pContrato";
            SqlCommand cmd;
            SqlConnection cn;
            int Folio = 0;
            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@pContrato", pContrato));

                            object data = cmd.ExecuteScalar();
                            if (data != DBNull.Value)
                            {
                                Folio = Convert.ToInt32(data);
                            }
                            cmd.Dispose();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
              //ERROR...
            }

            return Folio;
        }
        #endregion

        /// <summary>
        /// Setea informacion referente a la vacaciones de un trabajador.
        /// </summary>
        /// <param name="pContrato">Numero de contrato asociado al trabajador.</param>
        public void SetInfo(string pContrato)
        {
            string sql = "SELECT contrato, diasPropAnual, diasPropRestantes, diasPropTomados, " +
                         "diasPropAnRestantes, totalProp, diasProg, diasProgTomados, totalProg, " +
                         "totalDias FROM vacacion WHERE contrato=@pContrato ";
            SqlCommand cmd;
            SqlDataReader rd;
            SqlConnection cn;

            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@pContrato", pContrato));

                            rd = cmd.ExecuteReader();
                            if (rd.HasRows)
                            {
                                while (rd.Read())
                                {
                                    //SETEAMOS PROPIEDADES
                                    contrato = (string)rd["contrato"];
                                    DiasPropAnual = Convert.ToDouble(rd["diasPropAnual"]);
                                    DiasPropRestantes = Convert.ToDouble(rd["diasPropRestantes"]);
                                    DiasPropTomados = Convert.ToDouble(rd["diasPropTomados"]);
                                    DiasPropAnRestantes = Convert.ToDouble(rd["diasPropAnRestantes"]);
                                    TotalProp = Convert.ToDouble(rd["totalProp"]);
                                    diasProg = Convert.ToDouble(rd["diasProg"]);
                                    diasProgTomados = Convert.ToDouble(rd["diasProgTomados"]);
                                    TotalProg = Convert.ToDouble(rd["totalProg"]);
                                    TotalDias = Convert.ToDouble(rd["totalDias"]);
                                }
                            }
                            cmd.Dispose();
                            rd.Close();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                //ERROR...
            }
        }

        /// <summary>
        /// Verifica si hay registros en tabla vacacion
        /// </summary>
        /// <param name="pContrato"></param>
        /// <returns></returns>
        public static bool ExistenRegistros(string pContrato)
        {
            bool existe = false;
            string sql = "SELECT count(*) FROM vacacion WHERE contrato=@pContrato";
            SqlCommand cmd;
            SqlConnection cn;

            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            //PARAMETRO
                            cmd.Parameters.Add(new SqlParameter("@pContrato", pContrato));

                            if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                                existe = true;

                        }
                        cmd.Dispose();
                    }
                }               
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return existe;
        }

        /// <summary>
        /// Ingresa informacion de vacacion.
        /// </summary>
        /// <param name="pVac">Objeto vacaciones.</param>
        /// <param name="pContrato">Numero de contrato asociado al trabajador.</param>
        public static void NuevaInformacionVacacion(vacaciones pVac, string pContrato)
        {
            string sql = "INSERT INTO vacacion(contrato, diaspropanual, diasproprestantes, diasproptomados, totalprop, " +
                "diasprog, diasprogtomados, totalprog, totaldias, diaspropanrestantes) VALUES(@contrato, @diaspropanual, @diasproprestantes, " +
                "@diasproptomados, @totalprop, @diasprog, @diasprogtomados, @totalprog, @totaldias, @diasanrestantes)";

            SqlCommand cmd;
            SqlConnection cn;
            int res = 0;
            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@contrato", pContrato));
                            cmd.Parameters.Add(new SqlParameter("@diaspropanual", pVac.DiasPropAnual));
                            cmd.Parameters.Add(new SqlParameter("@diasproprestantes", pVac.DiasPropRestantes));
                            cmd.Parameters.Add(new SqlParameter("@diasproptomados", pVac.DiasPropTomados));
                            cmd.Parameters.Add(new SqlParameter("@totalprop", pVac.TotalProp));
                            cmd.Parameters.Add(new SqlParameter("@diasprog", pVac.diasProg));
                            cmd.Parameters.Add(new SqlParameter("@diasprogtomados", pVac.diasProgTomados));
                            cmd.Parameters.Add(new SqlParameter("@totalprog", pVac.TotalProg));
                            cmd.Parameters.Add(new SqlParameter("@totaldias", pVac.TotalDias));
                            cmd.Parameters.Add(new SqlParameter("@diasanrestantes", pVac.DiasPropAnRestantes));

                            res = cmd.ExecuteNonQuery();
                            if (res > 0)
                            {
                                //INGRESO CORRECTO
                            }
                            else
                            {
                                //SE PRODUJO ALGUN ERROR
                            }
                        }
                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// MOdificar informacion de vacacion.
        /// </summary>
        /// <param name="pVac">Objeto vacaciones</param>
        /// <param name="pContrato">Numero de contrato asociado al trabajador.</param>
        public static void ModificarInformacionVacacion(vacaciones pVac, string pContrato)
        {
            string sql = "UPDATE VACACION SET diaspropanual=@diaspropanual, diasproprestantes=@diasproprestantes, " +
                "diasproptomados=@diasproptomados, totalprop=@totalprop, diasprog=@diasprog, diasprogtomados=@diasprogtomados, " +
                " totalprog=@totalprog, totaldias=@totaldias, diaspropanrestantes=@diaspropanrestantes WHERE contrato=@contrato";
            SqlCommand cmd;
            int res = 0;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@contrato", pContrato));
                        cmd.Parameters.Add(new SqlParameter("@diaspropanual", pVac.DiasPropAnual));
                        cmd.Parameters.Add(new SqlParameter("@diasproprestantes", pVac.DiasPropRestantes));
                        cmd.Parameters.Add(new SqlParameter("@diasproptomados", pVac.DiasPropTomados));
                        cmd.Parameters.Add(new SqlParameter("@totalprop", pVac.TotalProp));
                        cmd.Parameters.Add(new SqlParameter("@diasprog", pVac.diasProg));
                        cmd.Parameters.Add(new SqlParameter("@diasprogtomados", pVac.diasProgTomados));
                        cmd.Parameters.Add(new SqlParameter("@totalprog", pVac.TotalProg));
                        cmd.Parameters.Add(new SqlParameter("@totaldias", pVac.TotalDias));
                        cmd.Parameters.Add(new SqlParameter("@diaspropanrestantes", pVac.DiasPropAnRestantes));

                        res = cmd.ExecuteNonQuery();                        
                    }

                    cmd.Dispose();                 
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        //PARA PRIMER INGRESO 
        /// <summary>
        /// </summary>
        /// <param name="FechaProgresivo">Corresponde a la fecha en que se recibe el certificado de cotizaciones.</param>
        public static vacaciones PrimerIngreso(Persona pTrabajador, Hashtable pDataReporte, bool? Reporte = false, DateTime? pFechaReporte = null)
        {
            bool AplicaAnual = false;
            double progresivos = 0, propAnuales = 0, propRestantes = 0, propUsados = 0, progUsados = 0, propRestantesAnuales = 0;
            double totalProporcionales = 0, totalProgresivos = 0, totalDias = 0;
            bool ConsideraProgresivo = false;
            Hashtable datos = new Hashtable();

            vacaciones Vc = new vacaciones();

            //Solo para reporte
            Hashtable datosRepo = new Hashtable();
            DateTime FechaLimite = DateTime.Now.Date;
            
            pDataReporte.Clear();

            double CotFaltante = 0, CotReq = 120, diasPropTotalesUsados = 0, diasProgTotalesUsados = 0;
            DateTime FechaBase = DateTime.Now.Date;
            //ULTIMO AÑO EN QUE SE ALCANZA A CUMPLIR UN AÑO (CONSIDERADO DESDE LA FECHA DE INICIO DE VACACIONES)
            DateTime UltimoAnioVacaciones = DateTime.Now.Date;

            //Si es para reporte la fecha limite será hasta la fecha del registro (ultimo dia registro)

            if (pTrabajador != null)
            {
                //CALCULAMOS LA CANTIDAD DE DIAS DE VACACIONES HASTA EL DIA DE HOY
                //SI LA FECHA DE TERMINO DE CONTRATO ES MENOR AL DÍA DE HOY CALCULAMOS HASTA LA FECHA DE TERMINO DE CONTRATO
                if (pTrabajador.Salida < DateTime.Now.Date)
                    FechaLimite = pTrabajador.Salida;
                else if (pTrabajador.Salida > DateTime.Now.Date)
                    FechaLimite = DateTime.Now.Date;
                else
                    FechaLimite = DateTime.Now.Date;

                // ------------------------------
                //  FERIADOS PROGRESIVOS        |
                //-------------------------------

                //@ DEBEMOS VERIFICAR LA CANTIDAD DE COTIZACIONES (MESES) QUE TIENE REGISTRADA LA FICHA
                //@ DEBEMOS CONSIDERAR LA FECHA EN QUE SE ENTREGÓ EL CERTIFICADO DE COTIZACIONES.

                //@ CASO1: NO TIENE COTIZACIONES Y TAMPOCO TIENE FECHA DE PROGRESIVO
                if (pTrabajador.AnosProgresivos == 0 || pTrabajador.FechaProgresivo > pTrabajador.Salida)
                {
                    //NO CONSIDERAMOS DIAS DE COTIZACIONES SI LA FECHA ES 01-01-3000
                    ConsideraProgresivo = false;
                    FechaBase = pTrabajador.Ingreso.AddYears(10);
                    //SI FECHA BASE ES MAYOR A LA FECHA EN QUE CUMPLE AÑO (LE CORRESPONDE ANUALIDAD VACACION LO CORREMOS OTRO AÑO)
                }
                //@CASO2: TIENE FECHA DE PROGRESIVO, CONSIDERAMOS MESES DE COTIZACIONES
                else
                {
                    ConsideraProgresivo = true;
                    //DEBEMOS OBTERNER LA FECHA EN LA CUAL SE CUMPLEN LOS 10 AÑOS (SI TIENE MENOS DE 120 COTIZACIONES)
                    if (pTrabajador.AnosProgresivos < 120)
                    {
                        //OBTENEMOS LA CANTIDAD DE MESES FALTANTES PARA OBTENER LOS 10 AÑOS DE BASE
                        CotFaltante = CotReq - pTrabajador.AnosProgresivos;

                        //SI HAY MESES FALTANTES PARA LOS 10 AÑOS DE BASE OBTENEMOS LA FECHA FINAL (FECHA EN QUE CUMPLIRÍA LOS 10 AÑOS)
                        if (CotFaltante > 0)
                            FechaBase = pTrabajador.FechaVacacion.AddMonths(Convert.ToInt32(CotFaltante));
                        //SI NO CONSIDERAMOS LA FECHA INICIAL FECHA VACACIONES
                        else
                            FechaBase = pTrabajador.FechaVacacion;
                    }
                    else
                    {
                        //CUMPLE CON LAS 120 COTIZACIONES
                        FechaBase = pTrabajador.FechaVacacion;
                    }
                }

                //OBTENEMOS LOS DIAS PROGRESIVOS COMO PUNTO DE PARTIDA LA FECHA BASE OBTENIDA...
                progresivos = vacaciones.Progresivos(FechaBase, pTrabajador.FechaProgresivo, ConsideraProgresivo);

                //vacaciones.MesProgresivoUsado(FechaBase, pTrabajador.FechaProgresivo, 3, 5, true);

                // ---------------------------------------------------------------------------------
                // FERIADOS LEGALES (15 X AÑO)                                                     |
                // ---------------------------------------------------------------------------------
                //@ PASADO UN AÑO DESDE EL INICIO DE CONTRATO, CORRESPONDE 15 DIAS DE VACACIONES.  |
                //@ POR MES = > 15/12 = 1.25                                                       |
                //@ POR DIA => 1.25/30 = 0.041667                                                  |
                //----------------------------------------------------------------------------------

                //ULTIMO AÑO DISPONIBLE...
                UltimoAnioVacaciones = vacaciones.UltimoAnioProporcional(pTrabajador.FechaVacacion);

                AplicaAnual = vacaciones.AplicaProporcionalAnual(pTrabajador.FechaVacacion);

                //CONSIDERAMOS LA FECHA DE VACACION GUARDADA EN LA FICHA
                if (AplicaAnual)
                    propAnuales = Math.Round(vacaciones.FeriadosProp(pTrabajador.FechaVacacion, UltimoAnioVacaciones), 2);
                else
                    propAnuales = 0;

                //VACACIONES RESTANTES (FECHA DESDE EL ULTIMO AÑO ENCONTRATO HASTA EL ULTIMO DIA DE CONTRATO O TODAY)

                if (AplicaAnual)
                {
                    if (UltimoAnioVacaciones < FechaLimite)
                        propRestantes = Math.Round(vacaciones.FeriadosProp(UltimoAnioVacaciones, FechaLimite), 2);
                    else
                        propRestantes = 0;
                }
                else
                {
                    propRestantes = vacaciones.FeriadosProp(pTrabajador.FechaVacacion, FechaLimite);
                }

                //acaciones.PeriodoDiasTomados(pTrabajador.FechaVacacion, FechaLimite, 0, 5);

                //OBTENEMOS LA CANTIDAD DE DIAS PROPORCIONALES USADOS AL IGUAL QUE LA CANTIDAD DE DIAS PROGRESIVOS               
                datos = diasUsados(pTrabajador.Contrato);

                //Guardamos el total de dias proprcionales usados y Progresivos
                diasPropTotalesUsados = Convert.ToDouble(datos["proporcional"]);
                diasProgTotalesUsados = Convert.ToDouble(datos["progresivo"]);

                //PROPORCIONALES ANUALES RESTANTES
                if (diasPropTotalesUsados > propAnuales)
                    propRestantesAnuales = 0;
                else
                    propRestantesAnuales = propAnuales - diasPropTotalesUsados;

                //CANTIDAD DE DIAS PROPORCIONALES DISPONIBLES (RESTAMOS LOS USADOS)
                totalProporcionales = Math.Round((propAnuales + propRestantes) - diasPropTotalesUsados, 2);

                if (diasProgTotalesUsados == progresivos)
                    totalProgresivos = 0;
                else if (diasProgTotalesUsados > progresivos)
                    totalProgresivos = 0;
                else
                    totalProgresivos = Math.Round(progresivos - diasProgTotalesUsados, 2);

                //TOTAL ENTRE DIAS PROGRESIVOS Y DIAS LEGALES
                totalDias = Math.Round(totalProporcionales + totalProgresivos, 2);

                //Obtenemos los dias usados hasta la fecha del registro comprobante (Solo para reporte)
                if ((bool)Reporte && pFechaReporte != null)
                {
                    datosRepo = diasUsados(pTrabajador.Contrato, Convert.ToDateTime(pFechaReporte));
                    propUsados = Convert.ToDouble(datosRepo["proporcional"]);
                    progUsados = Convert.ToDouble(datosRepo["progresivo"]);

                    pDataReporte.Add("propUsados", propUsados);
                    pDataReporte.Add("progUsados", progUsados);
                    pDataReporte.Add("propRestante", totalProporcionales);
                    pDataReporte.Add("progRestante", totalProgresivos);
                    pDataReporte.Add("propUsadosTotales", diasPropTotalesUsados);
                    pDataReporte.Add("progUsadosTotales", diasProgTotalesUsados);
                }

                //totalProgresivos = Math.Round(progresivos - progUsados, 2);              
               
                Vc.contrato = pTrabajador.Contrato;
                Vc.DiasPropAnual = Convert.ToDouble(propAnuales);
                Vc.DiasPropRestantes = Convert.ToDouble(propRestantes);
                Vc.DiasPropAnRestantes = Convert.ToDouble(propRestantesAnuales);
                Vc.DiasPropTomados = Convert.ToDouble(diasPropTotalesUsados);
                Vc.TotalProp = Convert.ToDouble(totalProporcionales);
                Vc.diasProg = Convert.ToDouble(progresivos);
                Vc.diasProgTomados = Convert.ToDouble(diasProgTotalesUsados);
                Vc.TotalProg = Convert.ToDouble(totalProgresivos);
                Vc.TotalDias = Convert.ToDouble(totalDias);
                Vc.FechaBaseProgresivos = FechaBase;
                Vc.FechaLimiteVacacionesLegales = FechaLimite;
            }

            //RETORNAMOS OBJETO VACACIONES
            return Vc;
        }

        //GENERA DOCUMENTO COMPROBANTE DE VACACIONES
        public static RptComprobanteVacacion GeneraComprobante(DateTime salida, DateTime finaliza, string contrato, Hashtable data)
        {
            string sql = "select DISTINCT rut, concat(nombre, ' ', apepaterno, ' ', apematerno) as name, " +
                "vacacionDetalle.contrato, vacaciondetalle.salida, finaliza, dias, tipo, retorna, folio, pervac " +
                "from vacacionDetalle " +
                "INNER JOIN trabajador ON trabajador.contrato = vacacionDetalle.contrato " +
                "where vacacionDetalle.contrato = @contrato AND vacaciondetalle.salida = @salida " +
                "AND vacaciondetalle.finaliza = @finaliza";

            SqlCommand cmd;
            SqlDataAdapter ad = new SqlDataAdapter();
            DataSet ds = new DataSet();
            RptComprobanteVacacion vaca = new RptComprobanteVacacion();
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@contrato", contrato));
                        cmd.Parameters.Add(new SqlParameter("@salida", salida));
                        cmd.Parameters.Add(new SqlParameter("@finaliza", finaliza));

                        ad.SelectCommand = cmd;
                        ad.Fill(ds, "data");

                        if (ds.Tables[0].Rows.Count > 0)
                        {                            
                            vaca.DataSource = ds.Tables[0];
                            vaca.DataMember = "data";

                            Empresa emp = new Empresa();
                            emp.SetInfo();

                            foreach (DevExpress.XtraReports.Parameters.Parameter parametro in vaca.Parameters)
                            {
                                parametro.Visible = false;
                            }

                            vaca.Parameters["empresa"].Value = emp.Razon;
                            vaca.Parameters["rutEmpresa"].Value = fnSistema.fFormatearRut2(emp.Rut);
                            vaca.Parameters["propusados"].Value = (double)data["propUsadosTotales"];
                            vaca.Parameters["progusados"].Value = (double)data["progUsadosTotales"];
                            vaca.Parameters["proprestante"].Value = (double)data["propRestante"];
                            vaca.Parameters["progrestante"].Value = Convert.ToDecimal(data["progRestante"]);
                            vaca.Parameters["propUsadosRepo"].Value = Convert.ToDouble(data["propUsados"]);
                            vaca.Parameters["progUsadosRepo"].Value = Convert.ToDouble(data["progUsados"]);

                            //DÍAS TOTALES DE VACACIONES ACUMULADAS ANUALMENTE
                            double diasVacacionesAcumuladas = (double)data["propRestante"] + (double)data["propUsadosTotales"];
                            vaca.Parameters["diasVacacionesAcumuladas"].Value = diasVacacionesAcumuladas;

                            //DÍAS TOTALES DE VACACIONES ACUMULADAS ANUALMENTE
                            double diasProgAcumulados = (double)data["progRestante"] + (double)data["progUsadosTotales"];
                            vaca.Parameters["diasProgAcumulados"].Value = diasProgAcumulados;

                            //SUB TOTAL DE DIAS SUMADOS PROGRESIVOS Y PROPRESTANTES
                            // double subTotalDiasAcumulados = diasVacacionesAcumuladas + (double)data["propRestante"] + (double)data["propRestante"];
                            //vaca.Parameters["subTotalDiasAcumulados"].Value = subTotalDiasAcumulados;

                            //TOTAL DÍAS PENDIENTES (RESTA DE LOS USADOS)
                            // double totalDiasPendientes = subTotalDiasAcumulados - ((double)data["propUsadosTotales"] + (double)data["progUsadosTotales"]);
                            // vaca.Parameters["totalDiasPendientes"].Value = totalDiasPendientes;
                           
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
            return vaca;           
        }

        /// <summary>
        /// Retorna la cantidad de dias tomados anteoriores al registro que se desea modificar
        /// </summary>
        public static double DiasTomadosAnt(string pContrato, DateTime pFechaTope, int pTipoVac)
        {
            string sql = "select ISNULL(SUM(dias), 0) from vacaciondetalle WHERE contrato = @pContrato AND salida < @pFechaTope AND tipo = @pTipo";
            SqlCommand cmd;
            SqlConnection cn;
            double count = 0;

            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@pContrato", pContrato));
                            cmd.Parameters.Add(new SqlParameter("@pFechaTope", pFechaTope));
                            cmd.Parameters.Add(new SqlParameter("@pTipo", pTipoVac));

                            object data = cmd.ExecuteScalar();
                            if (data != null)
                            {
                                count = Convert.ToDouble(data);
                            }

                            cmd.Dispose();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                //ERROR...
            }

            return count;
        }
    }

    /// <summary>
    /// Clase para obtener informacion del proceso de calculo de las vacaciones de un trabajador.
    /// <para>Usa metodos auxiliares de la clase Vacaciones.</para>
    /// </summary>
    class VacacionDetalle
    {
        /// <summary>
        /// Periodo al que pertenecen los dias de vacaciones
        /// </summary>
        public int Periodo { get; set; }
        /// <summary>
        /// Dias proporcionales ganados en periodo.
        /// </summary>
        public double DiasPropGanados { get; set; }
        /// <summary>
        /// Dias proporcionales usados en ese periodo
        /// </summary>
        public double DiasPropUsados { get; set; }
        /// <summary>
        /// Dias proporcionales pendientes de ese periodo.
        /// <para>Dias ganados - Dias usados</para>
        /// </summary>
        public double DiasPropPendientes { get; set; }
        /// <summary>
        /// Dias progresivos ganados en ese periodo.
        /// </summary>
        public double DiasProgGanados { get; set; }
        /// <summary>
        /// Dias progresivos usados en ese periodo.
        /// </summary>
        public double DiasProgUsados { get; set; }
        /// <summary>
        /// Dias progresivos pendientes.
        /// <para>Dias progresivos ganados - Dias progresivos usados</para>
        /// </summary>
        public double DiasProgPendientes { get; set; }
        /// <summary>
        /// Dias adicionales ganados en ese periodo.
        /// </summary>
        public double DiasAdiGanados { get; set; }
        /// <summary>
        /// Dias adicionales usados en ese periodo.
        /// </summary>
        public double DiasAdiUsados { get; set; }
        /// <summary>
        /// Dias adicionales pendientes.
        /// <para>Dias adicionales ganados - Dias adicionales usados</para>
        /// </summary>
        public double DiasAdiPendientes { get; set; }
        /// <summary>
        /// Sumatoria total de dias legales disponibles.
        /// </summary>
        public double TotalDiasLegales { get; set; }
        /// <summary>
        /// Sumatoria total de dias progresivos disponibles.
        /// </summary>
        public double TotalDiasProgresivos { get; set; }
        /// <summary>
        /// Sumatoria total de dias progresivos disponibles.
        /// </summary>
        public double TotalDiasAdicionales { get; set; }
        /// <summary>
        /// Si es true es un feriado de año, Si es falso es una fraccion menor a un año (Meses - Dias)
        /// </summary>
        public bool EsAnual { get; set; }
    }

  



    /// <summary>
    /// Clase para manipular imagen ingresada en ficha y reportes.
    /// </summary>
    class Imagen
    {
        public static Image imagen;
        private static Size OriginalSizeImage;
        public static string PathFile = "";

        /// <summary>
        /// Permite al usuario cargar una imagen.
        /// <para>Nota: Retorna un objeto Image.</para>
        /// </summary>
        /// <returns></returns>
        public static Image GenerarImagenFromUser()
        {
            OpenFileDialog ventana = new OpenFileDialog();
            /*FILTRO FORMATO*/
            ventana.Filter = "*.bmp;*.gif;*.jpg;*.png|*.bmp;*.gif;*.jpg;*.png";
            /*TITULO VENTANA*/
            ventana.Title = "Seleccione una imagen";

            if (ventana.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    //GENERAMOS IMAGEN EN BASE A IMAGEN CARGADA
                    imagen = Image.FromFile(ventana.FileName);

                    //GUARDAMOS RUTA DE ARCHIVO
                    PathFile = ventana.FileName;

                    //RETORNAMOS IMAGEN...
                    return imagen;
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return null;

                }
                
            }

            return null;
        }

        /*CARGAR IMAGEN EN PICTURE BOX*/
        /// <summary>
        /// Carga una imagen en un control PictureBox.
        /// </summary>
        /// <param name="picture">Representa la imagen.</param>
        /// <param name="pControl">Control picturebox.</param>
        public static void CargarImagen(Image picture, PictureEdit pControl)
        {
            if (picture != null)
            {
                int Ancho = 0, Alto = 0;

                Ancho = picture.Width;
                Alto = picture.Width;

                //CARGAMOS IMAGEN EN CONTROL...
                pControl.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;
                pControl.Image = picture;
                OriginalSizeImage = new Size(Ancho, Alto);
            }
        }

        /*MIMETYPE*/
        /// <summary>
        /// Obtiene el mime correspondiente a una imagen de acuerdo a su extension.
        /// </summary>
        /// <param name="ext">Extension archivo.</param>
        /// <returns></returns>
        private static string GetMimeType(string ext)
        {
            switch (ext.ToLower())
            {
                case ".bpm":
                case ".dib":
                case ".rle":
                    return "ima/bmp";

                case ".jpg":
                case ".jpeg":
                case ".jpe":
                case ".fif":
                    return "image/jpeg";

                case "gif":
                    return "image/gif";

                case ".tif":
                case ".tiff":
                    return "image/tiff";

                case "png":
                    return "image/png";
                default:
                    return "image/jpeg";
            }
        }

        /*OBTENER EXTENSION IMAGEN*/        
        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageDecoders();

            ImageCodecInfo encoder = (from enc in encoders where enc.MimeType == mimeType select enc).First();
            return encoder;
        }

        /*COMPRIMIR IMAGEN*/
        /// <summary>
        /// Comprime una imagen y la almacena de forma temporal en el disco local.
        /// </summary>
        /// <param name="InputFile">Ruta del archivo.</param>
        /// <param name="OutputFile">Ruta donde se guardará la imagen comprimida.</param>
        /// <param name="compression">Factor de compresion</param>
        /// <param name="Ancho">Ancho imagen.</param>
        /// <param name="Largo">Largo imagen.</param>
        public static void ComprimirImagen(string InputFile, string OutputFile, long compression, int Ancho, int Largo)
        {
            string mimeType = "";

            //GENERAMOS UN BITMAP DE ACUERDO A ARCHIVO DE ENTRADA
            Bitmap b = new Bitmap(InputFile);
            Bitmap resize = new Bitmap(b, new Size(Ancho, Largo));

            EncoderParameters eps = new EncoderParameters(1);

            eps.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, compression);
            mimeType = GetMimeType(new FileInfo(InputFile).Extension);
            ImageCodecInfo ici = GetEncoderInfo(mimeType);

            //GUARDAMOS BITMAP 
            //OUTPUT FILE REPRESENTA LA RUTA 
            resize.Save(OutputFile, ici, eps);

            //LIBERAMOS RECURSOS
            resize.Dispose();
            b.Dispose();
        }

        /*CROP IMAGEN*/
        /// <summary>
        /// Permite recortar una imagen.
        /// </summary>
        /// <param name="pControl"></param>
        /// <param name="crop"></param>
        /// <param name="PathFile"></param>
        public static void CropImagen(PictureEdit pControl, Rectangle crop, string PathFile)
        {
            try
            {
                Bitmap ImagenOriginal = new Bitmap(pControl.Image, pControl.Width, pControl.Height);
                //CROP REPRESENTA LA IMAGEN RECORTADA
                Bitmap ImagenRecortada = new Bitmap(crop.Width, crop.Height);

                /*PARA DIBUJAR DENTRO DEL CONTROL*/
                Graphics g = Graphics.FromImage(ImagenRecortada);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                //DIBUJAMOS...
                g.DrawImage(ImagenOriginal, 0, 0, crop, GraphicsUnit.Pixel);

                //GUARDAMOS IMAGEN RECORTADA EN DISCO 
                ImagenRecortada.Save(PathFile + "empleado.bmp", ImageFormat.Bmp);

                pControl.Image = ImagenRecortada;
                pControl.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;

                ComprimirImagen(PathFile + "empleado.bmp", PathFile + "empleado_resize.jpg", 90, 300, 300);

                ImagenOriginal.Dispose();
            }
            catch (Exception)
            {
                //ERROR...
            }
        }

        /*OBTENER IMAGEN DESDE BD*/
        /// <summary>
        /// Devuelve el logo de la empresa almacenado en bd.
        /// </summary>
        /// <returns></returns>
        public static Image GetLogoFromBd()
        {
            Image img = null;

            string sql = "SELECT logo FROM empresa";
            SqlCommand cmd;
            SqlDataReader rd;
            SqlConnection cn;
            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            rd = cmd.ExecuteReader();
                            if (rd.HasRows)
                            {
                                while (rd.Read())
                                {
                                    if (rd["logo"] as byte[] != null)
                                        img = GetImageFromStream((byte[])rd["logo"]);
                                }
                            }
                        }
                        cmd.Dispose();
                        rd.Close();
                    }
                }

            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return img;
        }

        /*GENERAR IMAGEN TRABAJADOR DESDE BD*/
        /// <summary>
        /// Devuelve la imagen almacenada del trabajador.
        /// </summary>
        /// <param name="pContrato">Numero de contrato asociado al trabajador.</param>
        /// <returns></returns>
        public static Image GetImagenTrabajador(string pContrato)
        {
            Image image = null;
            string sql = "select rutafoto FROM trabajador WHERE contrato = @pContrato AND anomes=@pPeriodo";
           
            SqlCommand cmd;
            SqlDataReader rd;
            SqlConnection cn;
            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@pContrato", pContrato));
                            cmd.Parameters.Add(new SqlParameter("@pPeriodo", Calculo.PeriodoObservado));

                            rd = cmd.ExecuteReader();
                            if (rd.HasRows)
                            {
                                while (rd.Read())
                                {
                                    if (rd["rutafoto"] as byte[] != null)
                                        image = GetImageFromStream((byte[])rd["rutafoto"]);
                                }
                            }
                            cmd.Dispose();
                            rd.Close();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return image;
        }

        /// <summary>
        /// Genera una imagen en blanco en el caso de que no se haya ingresado logo empresa.
        /// </summary>
        /// <param name="picture">Control XRPictureBox reporte.</param>
        public static void SinImagen(XRPictureBox picture)
        {
            picture.Image = Labour.Properties.Resources.logo_vacio;
            picture.Sizing = DevExpress.XtraPrinting.ImageSizeMode.StretchImage;
        }            

        /*GENERAR BYTE DESDE IMAGEN*/
        /// <summary>
        /// Genera un arreglo byte[] desde un imagen.
        /// </summary>
        /// <param name="pControl">PictureBox control donde está cargada la imagen.</param>
        /// <returns></returns>
        public static byte[] GetStreamFromImage(PictureBox pControl)
        {
            Image img = pControl.Image;

            MemoryStream memory = new MemoryStream();

            img.Save(memory, ImageFormat.Png);

            //GUARDAMOS EN ARREGLO
            byte[] imageBt = memory.ToArray();
            return imageBt;
        }

        /*GUARDAR IMAGEN EN BD*/
        /// <summary>
        /// Guarda en bd la representacion en binario de una imagen.
        /// </summary>
        /// <param name="pathFle">Ruta del archivo.</param>
        /// <returns></returns>
        public static byte[] GuardarImagenBd(string pathFile)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                FileStream fs = new FileStream(pathFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                ms.SetLength(fs.Length);
                fs.Read(ms.GetBuffer(), 0, (int)fs.Length);

                byte[] arrImg = ms.GetBuffer();
                ms.Flush();
                fs.Close();

                return arrImg;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //GENERAR IMAGEN DESDE BD
        /// <summary>
        /// Genera la correspondiente imagen desde su representacion en binario.
        /// </summary>
        /// <param name="img">Imagen.</param>
        /// <returns></returns>
        public static Image GetImageFromStream(byte[] img)
        {
            MemoryStream memo = new MemoryStream(img);
            Image imagenFinal = Image.FromStream(memo);

            return imagenFinal;                 
        }

        /*LIMPIAR DIBUJO EN PICTURE EDIT*/
        /// <summary>
        /// Limpia una figura pintada encima de un control PictureBox.
        /// </summary>
        /// <param name="draw">Objeto Rectangulo.</param>
        public static void CleanDraw(Rectangle draw)
        {
            draw.Width = 0;
            draw.Height = 0;
        }

        //ELIMINAR IMAGENES
        public static void CleanImage(string pathFile)
        {
            if (File.Exists(pathFile + "empleado.bmp"))
            {
                File.Delete(pathFile + "empleado.bmp");
            }
            if (File.Exists(pathFile + "empleado_resize.jpg"))
            {
                File.Delete(pathFile + "empleado_resize.jpg");
            }
            if (File.Exists(pathFile + "ConvertImg.jpg"))
            {
                File.Delete(pathFile + "ConvertImg.jpg");
            }
        }
    }

    /*CLASE PARA GENERACION DE ARCHIVO DE TEXTO PARA PREVIRED*/
    class Archivo
    {
        /*CREAR NUEVO ARCHIVO DE TEXTO*/
        /// <summary>
        /// Crea un nuervo archivo de texto.
        /// </summary>
        /// <param name="PathFile">Ruta del archivo.</param>
        /// <returns></returns>
        public static bool CrearArchivo(string PathFile)
        {
            try
            {
                /*CREAMOS ARCHIVO*/
                using (FileStream file = File.Create(PathFile))
                {
                    if (File.Exists(PathFile))
                        return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        /*ESCRIBIR EN ARCHIVO*/
        /// <summary>
        /// Escribe texto dentro de un archivo.
        /// </summary>
        /// <param name="PathFile">Ruta del archivo.</param>
        /// <param name="cadena">Nueva cadena.</param>
        public static void EscribirArchivo(string PathFile, string cadena)
        {
            if (File.Exists(PathFile))
            {
                try
                {
                    StreamWriter readFile = new StreamWriter(PathFile, true, Encoding.Default);
                    readFile.WriteLine(cadena);

                    //CERRAMOS...
                    readFile.Close();
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show(ex.Message);
                }                
            }
        }

        /*AGREGAR INFORMACION A ARCHIVO EXISTENTE*/
        /// <summary>
        /// Agrega una nueva linea a un archivo existente.
        /// </summary>
        /// <param name="PathFile">Ruta del archivo.</param>
        /// <param name="cadena">Nueva linea.</param>
        public static void AgregarInfoFile(string PathFile, string cadena)
        {
            //PREGUNTAMOS SI EL ARCHIVO EXISTE (RUTA VALIDA)
            if (File.Exists(PathFile))
            {
                try
                {
                    using (StreamWriter file = new StreamWriter(PathFile, true))
                    {
                        file.WriteLine(cadena);
                    }
                }
                catch (Exception ex)
                {
                  //ERROR
                }
            }
        }

        /*LIMPIAR ARCHIVO DE TEXTO SI EXISTE*/
        /// <summary>
        /// Limpia un archivo de texto.
        /// </summary>
        /// <param name="PathFile">Ruta del archivo.</param>
        public static void CleanTextFile(string PathFile)
        {
            if (File.Exists(PathFile))
            {
                try
                {
                    using (var fs = new FileStream(PathFile, FileMode.Truncate))
                    {
                        //...
                    }
                }
                catch (Exception ex)
                {
                  //ERROR...
                }
            }
        }

        //PARA LEER DATOS DE ARCHIVO DE TEXTO CON INFORMACION DE LA EMPRESA
        /// <summary>
        /// Obtiene el valor de un parametro dentro de un archivo de texto.
        /// <para>Nota: Solo se usa para extraer informacion de la empresa.</para>
        /// </summary>
        /// <param name="PathFile">Ruta del archivo.</param>
        /// <param name="Parameter">Nombre del parametro.</param>
        /// <returns></returns>
        public static string GetParametro(string PathFile, string Parameter)
        {
            string value = "";
            
            if (File.Exists(PathFile))
            {
                try
                {
                    using (StreamReader rd = new StreamReader(PathFile))
                    {
                        string Line = "";
                        while (Line != null)
                        {
                            //LEEMOS LINEA
                            Line = rd.ReadLine();

                            if (Line.ToLower().Contains(Parameter) && Line != null)
                            {
                                //SEPARAMOS POR "="
                                string[] spl = Line.Split('=');
                                if (spl.Length > 0)
                                {
                                    //CONSIDERAMOS LA PARTE DERECHA
                                    value = spl[1].Trim();
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                  //ERROR...
                }
            }

            return value;
        }
    }

    /*CLASE CALCULOS FICHA PREVIRED*/
    /// <summary>
    /// Permite generar archivo para plataforma previred.
    /// </summary>
    class Previred
    {
        /*PROPIEDADES DE ENTRADA*/
        /// <summary>
        /// Rut trabaajador.
        /// </summary>
        private string rut;
        /// <summary>
        /// Periodo consultado.
        /// </summary>
        private int periodo = 0;
        /// <summary>
        /// Numero de contrato asociado a trabajador.
        /// </summary>
        private string contrato;

        private string cadenaCarga = "";

        //SOLO PARA SABER SI TIENE AFP
        private bool TieneAfp = false;
        private bool TieneRegimenAntiguo = false;
        private bool TieneFonasa = false;
        private bool TieneIsapre = false;
        private bool CotizaSalud = false;

        private int CantidadLicencias = 0;
        private int CantidadPermisos = 0;
        private int CantidadAusentismos = 0;
        private int CantidadAccidente = 0;

        public string Condicion = "";

        public string Rut
        {
            set { rut = value; }
            get { return rut; }
        }

        public int Periodo
        {
            set { periodo = value; }
            get { return periodo; }
        }

        public string Contrato
        {
            set { contrato = value; }
            get { return contrato; }
        }

        /*CONSTRUCTOR*/
        public Previred()
        { }

        public Previred(string rut, int periodo)
        {
            Rut = rut;
            Periodo = periodo;
        }

        /// <summary>
        /// Metodo publico que genera archivo para previred.
        /// </summary>
        /// <returns></returns>
        public List<string> GenerarData(List<ReportePrevired> pDataSourcePrevired)
        {
            Hashtable informacion = new Hashtable();
            List<MovimientoPersonal> Listado = new List<MovimientoPersonal>();
            //TODO LOS MOVIMIENTOS GENERADOS
            List<string> data = new List<string>();
            List<string> adicional = new List<string>();
            //LISTADO DE CONTRATOS ASOCIADOS AL TRABAJADOR
            List<string> ListaContratos = new List<string>();
            List<string> DataSource = new List<string>();
            List<MovimientoPersonal> Suspensiones = new List<MovimientoPersonal>();
            //PARA CONTRATOS
            string tipoLinea = "00";
            if (Rut != "" && Periodo != 0)
            {
                //BUSCAMOS CONTRATOS ASOCIADOS AL RUT

                ListaContratos = ContratosTrabajador(fnSistema.fEnmascaraRut(Rut), Periodo, Condicion);
                if (ListaContratos.Count > 0)
                {
                    foreach (var contrato in ListaContratos)
                    {
                        //OBTENEMOS LOS MOVIMIENTOS ASOCIADOS AL CONTRATO DE LA LISTA
                        Listado = GetMovimientoPersonal(Periodo, contrato);

                        Suspensiones = new List<MovimientoPersonal>();
                        Suspensiones = Listado.FindAll(x => x.codMovimiento == "13" || x.codMovimiento == "14" || x.codMovimiento == "15");

                        //LLAMAMOS AL METODO QUE NOS TRAE LOS DATOS DEL TRABAJADOR
                        //informacion = DataPersona(contrato, Periodo);

                        DataSource = Calculo(Listado, contrato, Periodo, tipoLinea, pDataSourcePrevired);
                        if (DataSource == null || DataSource.Count == 0)
                            return null;

                        data.AddRange(DataSource);
                        //GUARDAMOS CADENA
                        //data.Add(cadenaCarga);                       


                        //LIMPIAMOS CADENA
                        cadenaCarga = "";

                        if (Suspensiones.Count > 0)
                        {
                            //PREGUNTAMOS SI HAY MAS MOVIMIENTOS
                            if (Listado.Count > 0)
                            {
                                adicional = CalculoAdicional(Listado, contrato, Periodo, true);

                                //RECORREMOS Y GUARDAMOS EN LISTA
                                foreach (var cadena in adicional)
                                {
                                    data.Add(cadena);
                                }
                            }
                        }
                        else
                        {
                            //PREGUNTAMOS SI HAY MAS MOVIMIENTOS
                            if (Listado.Count > 1)
                            {
                                adicional = CalculoAdicional(Listado, contrato, Periodo);

                                //RECORREMOS Y GUARDAMOS EN LISTA
                                foreach (var cadena in adicional)
                                {
                                    data.Add(cadena);
                                }
                            }
                        }
                      

                        //SI TIENE MAS CONTRATOS EL TIPO DE LINEA SERÁ --> 002 (SEGUNDO CONTRATO)
                        tipoLinea = "02";
                    }
                }

                //LISTA FINAL --> DATA  
            }

            return data;
        }

        /*OBTENER DATOS DEL TRABAJADOR DE ACUERDO A DATOS DE ENTRADA (N° CONTRATO Y PERIODO REMUNERACION)*/

        //@@@ => CODIGO PARA DEFINIR NACIONALIDAD PERSONA
        // 0 --> CHILENA
        // 1 --> EXTRANJERA
        private string GetCodigoNacionalidad(int pNacionalidad)
        {
            string code = "";
            string sql = "SELECT dato01 FROM nacion WHERE id=@pId";
            SqlCommand cmd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pId", pNacionalidad));

                        code = (string)cmd.ExecuteScalar();
                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return code;
        }

        //@@@ => CODIGO SEXO
        // M -> MASCULINO
        // F -> FEMENINO
        private string GetCodigoSexo(int pData)
        {
            string code = "";
            if (pData >= 0)
            {
                if (pData == 0)
                    code = "M";
                else
                    code = "F";
            }
            return code;
        }

        //TIPO DE TRABAJADOR
        /*
         * 0 -> ACTIVO (NO PENSIONADO) 
         * 1 -> PENSIONADO Y COTIZA
         * 2 -> PENSIONADO Y NO COTIZA
         * 3 -> ACTIVO > 65 AÑOS
         */

        private int GetTipoTrabajador(int pData, DateTime Fechanac)
        {
            //SI NO ES JUBILADO Y TIENE MAYOR DE 65 RETORNAMOS 3
            //SI ES PENSIONADO Y COTIZA RETORNAMOS 1
            //SI ES PENSIONADO Y NO COTIZA RETORNAMOS 2
            //SI ES ACTIVO NO PENSIONADO MENOR DE 65 RETORNAMOS 0

            DateTime Nacimiento = Fechanac;
            DateTime mayor = Fechanac.AddYears(65);
            //EDAD
            int edad = 0;
            edad = DateTime.Now.Year - Fechanac.Year;

            if (pData == 0 && (mayor <= DateTime.Now.Date))
                return 3;
            else if (pData == 0)
                return 0;
            else if (pData == 1)
                return 2;
            else if (pData == 2)
                return 1;

            return -1;
        }

        //OBTENER TRAMO ASIGNACION FAMILIAR
        private string GetTramoAsignacionFamiliar(int pData)
        {
            string sql = "SELECT dato01 FROM impunico WHERE tramo=@pTramo";
            SqlCommand cmd1;
            string tramo = "";
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd1 = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd1.Parameters.Add(new SqlParameter("@pTramo", pData));

                        tramo = (string)cmd1.ExecuteScalar();

                        cmd1.Dispose();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return tramo;
        }

        //FAMILIARES SIMPLE
        private int GetCargasSimple(string pContrato)
        {
            string sql = "SELECT count(rutcarga) as cantidad FROM cargafamiliar " +
                          "WHERE contrato=@pContrato AND (invalido=0 AND maternal=0)";
            SqlCommand cmd;
            int count = 0;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pContrato", pContrato));

                        count = (int)cmd.ExecuteScalar();

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return count;
        }
        //FAMILIARES MATERNALES
        private int GetCargasMaternales(string pContrato)
        {
            string sql = "SELECT count(rutcarga) as cantidad FROM cargafamiliar " +
                    "WHERE contrato=@pContrato AND maternal=1";
            SqlCommand cmd;
            int count = 0;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pContrato", pContrato));

                        count = (int)cmd.ExecuteScalar();

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return count;
        }
        //FAMILIARES INVALIDOS
        private int GetCargasInvalidas(string pContrato)
        {
            string sql = "SELECT count(rutcarga) as cantidad FROM cargafamiliar " +
                    "WHERE contrato=@pContrato AND invalido=1";
            SqlCommand cmd;
            int count = 0;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pContrato", pContrato));
                        count = (int)cmd.ExecuteScalar();

                        fnSistema.sqlConn.Close();
                        cmd.Dispose();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return count;
        }

        //MONTO CARGA FAMILIAR SIMPLE
        private double GetMontoCarga(string pContrato)
        {
            string sql = "select valorcalculado from itemtrabajador where contrato=@pContrato " +
                         "AND anomes=@pPeriodo AND coditem='ASIGFAM'";
            SqlCommand cmd;
            SqlDataReader rd;
            double sum = 0;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pContrato", pContrato));
                        cmd.Parameters.Add(new SqlParameter("@pPeriodo", Periodo));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //SUMAMOS VALOR
                                sum = sum + Convert.ToDouble(rd["valorcalculado"]);
                            }
                        }
                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                        rd.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return sum;
        }

        //MONTO CARGA FAMILIAR RETROACTIVA
        private double GetMontoCargaRetroactiva(string pContrato)
        {
            string sql = "select valorcalculado from itemtrabajador where contrato=@pContrato " +
                         "AND anomes=@pPeriodo AND coditem='ASIFAR'";
            double sum = 0;
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pContrato", pContrato));
                        cmd.Parameters.Add(new SqlParameter("@pPeriodo", Periodo));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //SUMAMOS VALOR 
                                sum = sum + Convert.ToDouble(rd["valorcalculado"]);
                            }
                        }

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                        rd.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
            return sum;

        }

        //REGIMEN PREVISION
        private string GetRegimenPrevision(int pRegimen, DateTime pFechaNacimiento)
        {
            string regimen = "";
            ////SI ES MAYOR DE 65 AÑOS GUARDAMOS CON SIP(SIN INTITUCION PREVISIONAL)
            //if (fnSistema.AdultoMayor(pFechaNacimiento))
            //{
            //    regimen = "SIP";
            //}

            if (pRegimen == 1)
            { regimen = "AFP"; TieneAfp = true; }
            if (pRegimen == 2)
            { regimen = "AFP"; }
            if (pRegimen == 3)
                regimen = "INP";
            if (pRegimen == 4)
                regimen = "INP";
            if (pRegimen == 5)
                regimen = "INP";
            if (pRegimen == 6)
                regimen = "SIP";

            return regimen;

        }

        //OBTENER CODIGO AFP
        private string GetCodigoAfp(int pAfp)
        {
            string code = "";

            //SI EL DATO ES 0 NO ESTÁ EN AFP
            if (pAfp == 0)
                return "00";

            string sql = "SELECT dato01 FROM afp WHERE id=@pId";
            SqlCommand cmd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pId", pAfp));

                        code = (string)cmd.ExecuteScalar();

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                    }
                }

            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return code;
        }

        //CODIGO COTIZACION SALUD (SI COTIZA)       
        private string GetCodigoSalud(int pSalud)
        {
            string sql = "SELECT dato01 FROM isapre WHERE id=@pId";
            SqlCommand cmd;
            string code = "";
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pId", pSalud));
                        code = (string)cmd.ExecuteScalar();

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return code;
        }

        //COTIZACION PACTADA CON ISAPRE
        private string GetCotizacionPactada(string pContrato, int periodo, bool? es13 = false, bool? es14=false)
        {
            double val = 0, imponible = 0, uf = 0, final = 0;
            string sql = "";
            sql = "SELECT valor, uf, pesos, porc, (select uf FROM valoresmes WHERE anomes=@pPeriodo) as montouf FROM itemtrabajador " +
                    "WHERE anomes=@pPeriodo AND contrato=@pContrato AND " +
                    "coditem='SALUD' AND (splab13=0 AND splab14=0)";

            if (es13 == true)
                sql = "SELECT valor, uf, pesos, porc, (select uf FROM valoresmes WHERE anomes=@pPeriodo) as montouf FROM itemtrabajador " +
                    "WHERE anomes=@pPeriodo AND contrato=@pContrato AND " +
                    "coditem='SALUD' AND splab13=1";
            if (es14 == true)
                sql = "SELECT valor, uf, pesos, porc, (select uf FROM valoresmes WHERE anomes=@pPeriodo) as montouf FROM itemtrabajador " +
                    "WHERE anomes=@pPeriodo AND contrato=@pContrato AND " +
                    "coditem='SALUD' AND splab14=1";

            SqlCommand cmd;
            SqlDataReader rd;

            //OBTENEMOS EL VALOR DE LA UF
            //uf = GetValueUf(periodo);

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pContrato", pContrato));
                        cmd.Parameters.Add(new SqlParameter("@pPeriodo", periodo));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                val = Convert.ToDouble(rd["valor"]);
                                uf = Convert.ToDouble(rd["montouf"]);
                                if ((bool)rd["uf"])
                                {
                                    //COTIZACION EN UF
                                    final = val * uf;
                                }
                                else if ((bool)rd["pesos"])
                                {
                                    //COTIZACION EN PESOS
                                    final = val;
                                }
                                else if ((bool)rd["porc"])
                                {
                                    //COTIZACION EN PORCENTAJE
                                    final = (val / 100) * imponible;
                                }
                            }
                        }

                        cmd.Dispose();
                    }


                    fnSistema.sqlConn.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return Math.Round(final, MidpointRounding.AwayFromZero).ToString();
        }

        private double GetValueUf(int pPeriodo)
        {
            double value = 0;
            string sql = "SELECT uf FROM valoresmes WHERE anomes=@pPeriodo";
            SqlCommand cmd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pPeriodo", pPeriodo));

                        value = Convert.ToDouble(cmd.ExecuteScalar());

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return value;
        }

        //SABER SI EL RUT TIENE ASOCIADO MAS DE UN CONTRATO EL PERIODO EVALUADO
        /// <summary>
        /// Genera un listado con todos los contratos asociados a un trabajador en el mes.
        /// </summary>
        /// <param name="rut">Rut persona.</param>
        /// <param name="periodo">Periodo consulta.</param>
        /// <returns></returns>
        private List<string> ContratosTrabajador(string rut, int periodo, string pCondicion)
        {
            //GUARDAR LOS CONTRATO EN UNA LISTA
            List<string> contratos = new List<string>();
            //SOLO CONTRATOS QUE CUMPLAN LA CONDICION
            string sql = "";
            sql = "SELECT contrato FROM trabajador WHERE rut=@pRut AND anomes=@pAnomes ";
            if (pCondicion.Length > 0)
            {
                //FILTRAMOS BUSQUEDA
                sql = sql + " AND " + pCondicion;
            }

            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pRut", rut));
                        cmd.Parameters.Add(new SqlParameter("@pAnomes", periodo));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                contratos.Add((string)rd["contrato"]);
                            }
                        }
                    }
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                    rd.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return contratos;
        }

        //OBTENER EL TIPO DE CONTRATO DEL TRABAJADOR
        private string GetTipoContrato(int TipoContrato)
        {
            string tipo = "";
            if (TipoContrato == 0)
                tipo = "1";
            if (TipoContrato == 1 || TipoContrato == 2)
                tipo = "7";

            return tipo;

        }

        //RENTA IMPONIBLE IPS
        //SE DEBE CALCULAR EN BASE A TOPE CON UF PERIODO ANTERIOR
        //ESTE MONTO ES OBLIGATORIO SI SE INFORM COTIZACIONES EN LOS CAMPOS 65, 70, 71, 72 Y/O 73
        private double GetMontoIps(int periodo, double imp, string pRegimenPrevisional)
        {
            int periodoAnterior = fnSistema.fnObtenerPeriodoAnterior(periodo);
            string sql = "select uf, topeafp from valoresmes WHERE anomes=@pPeriodo";
            double valorTope = 0, imponible = 0, total = 0;
            bool existeAnterior = false;
            SqlCommand cmd;
            SqlDataReader rd;

            existeAnterior = Labour.Calculo.ExistePeriodoAnterior(periodoAnterior);
            double uf60 = 0;
            valorTope = Labour.Calculo.GetValueFromCalculoMensaul(Contrato, periodo, "systopeafp");

            if (pRegimenPrevisional.ToLower() == "INP")
            {
                /*USAMOS UF MES ANTERIOR*/
                if (existeAnterior)
                {
                    try
                    {
                        if (fnSistema.ConectarSQLServer())
                        {
                            using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                            {
                                //PARAMETROS
                                cmd.Parameters.Add(new SqlParameter("@pPeriodo", periodoAnterior));
                                rd = cmd.ExecuteReader();
                                if (rd.HasRows)
                                {
                                    while (rd.Read())
                                    {
                                        valorTope = Convert.ToDouble(rd["topeafp"]) * Convert.ToDouble(rd["uf"]);

                                        //valorTope = Convert.ToDouble(rd["topeafp"]) * 28222.33;
                                        uf60 = Convert.ToDouble(rd["uf"]) * 60;
                                    }
                                }

                                cmd.Dispose();
                                rd.Close();
                                fnSistema.sqlConn.Close();
                            }
                        }


                        imponible = imp;

                        if (imponible > valorTope)
                            total = valorTope;
                        else if (imponible < valorTope)
                            total = imponible;
                        else
                            total = imponible;

                        if (total > uf60)
                            total = uf60;

                    }
                    catch (SqlException ex)
                    {
                        XtraMessageBox.Show(ex.Message);
                    }
                }
                else
                {
                    if (imp > valorTope)
                        total = valorTope;
                    else if (imp < valorTope)
                        total = imp;
                    else
                        total = imp;
                }

            }
            else
            {
                //MES EN CURSO
                if (imp > valorTope)
                    total = valorTope;
                else if (imp < valorTope)
                    total = imp;
                else
                    total = imp;
            }



            return total;
        }

        //VER SI CONTRATO TIENE PERMISO SIN GOCE DE SUELDO
        private bool TienePermisos(int periodo, string contrato)
        {
            string sql = "SELECT count(*) FROM AUSENTISMO WHERE contrato = @pContrato AND periodoAnterior=@pPeriodo " +
                " AND motivo = 3 AND rebsueldo=1";

            int count = 0;
            SqlCommand cmd;
            bool tiene = false;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pContrato", contrato));
                        cmd.Parameters.Add(new SqlParameter("@pPeriodo", periodo));

                        count = Convert.ToInt32(cmd.ExecuteScalar());

                        if (count > 0)
                        {
                            tiene = true;
                            CantidadPermisos = count;
                        }


                        cmd.Parameters.Clear();
                        cmd.Dispose();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return tiene;
        }

        //VER SI CONTRATO TIENE ACCIDENTE DE TRABAJO
        private bool TieneAccidenteTrabajo(int periodo, string contrato)
        {
            string sql = "SELECT count(*) FROM ausentismo WHERE contrato=@pContrato AND periodoAnterior=@pPeriodo " +
                " AND motivo=5";
            SqlCommand cmd;
            int count = 0;
            bool tiene = false;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pContrato", contrato));
                        cmd.Parameters.Add(new SqlParameter("@pPeriodo", periodo));

                        count = Convert.ToInt32(cmd.ExecuteScalar());
                        if (count > 0)
                        {
                            tiene = true;
                            CantidadAccidente = count;
                        }


                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return tiene;
        }

        //VER SI CONTRATO TIENE AUSENTISMOS (INASISTENCIA)
        private bool TieneAusentismos(int periodo, string contrato)
        {
            string sql = "SELECT count(*) FROM ausentismo WHERE contrato=@pContrato AND periodoAnterior=@pPeriodo " +
                " AND motivo = 4";

            SqlCommand cmd;
            int count = 0;
            bool existe = false;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pContrato", contrato));
                        cmd.Parameters.Add(new SqlParameter("@pPeriodo", periodo));

                        count = Convert.ToInt32(cmd.ExecuteScalar());
                        if (count > 0)
                        {
                            existe = true;
                            CantidadAusentismos = count;
                        }

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return existe;
        }

        //VER SI TIENE LICENCIAS (LAS LICENCIAS SE CONSIDERAN COMO SUBSIDIO)
        private bool TieneLicencias(int periodo, string contrato)
        {
            bool tiene = false;
            string sql = "SELECT count(*) FROM ausentismo WHERE contrato=@pContrato AND periodoAnterior=@pPeriodo" +
                " AND (motivo = 1 OR motivo = 2)";
            SqlCommand cmd;
            int count = 0;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pContrato", contrato));
                        cmd.Parameters.Add(new SqlParameter("@pPeriodo", periodo));

                        count = Convert.ToInt32(cmd.ExecuteScalar());
                        if (count > 0)
                        {
                            tiene = true;
                            CantidadLicencias = count;
                        }
                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return tiene;
        }

        //VER SI EL CONTRATO EN EL PERIODO ANTERIOR TIENE UN TIPO CONTRATO DISTINTO AL PERIODO EN CURSO
        //VER SI PASA DE FIJO A INDEFINIDO
        private bool CambioContratoFijoaIndefinido(int periodo, string contrato)
        {
            int periodoAnterior = fnSistema.fnObtenerPeriodoAnterior(periodo);
            bool cambia = false;
            string sqlAntiguo = "SELECT tipocontrato FROM trabajador WHERE contrato=@pContrato AND anomes=@pPeriodo ";
            string sqlNuevo = "SELECT tipocontrato FROM trabajador WHERE contrato=@pContrato AND anomes=@pPeriodo";
            SqlCommand cmd;
            int tipoAntiguo = 0, tipoNuevo = 0;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    //PARA PERIODO ANTERIOR
                    using (cmd = new SqlCommand(sqlAntiguo, fnSistema.sqlConn))
                    {
                        cmd.Parameters.Add(new SqlParameter("@pContrato", contrato));
                        cmd.Parameters.Add(new SqlParameter("@pPeriodo", periodoAnterior));

                        tipoAntiguo = Convert.ToInt32(cmd.ExecuteScalar());

                        cmd.Parameters.Clear();
                        cmd.Dispose();
                    }

                    //PARA PERIODO NUEVO
                    using (cmd = new SqlCommand(sqlNuevo, fnSistema.sqlConn))
                    {
                        //PARAMETROS    
                        cmd.Parameters.Add(new SqlParameter("@pContrato", contrato));
                        cmd.Parameters.Add(new SqlParameter("@pPeriodo", periodo));

                        tipoNuevo = Convert.ToInt32(cmd.ExecuteScalar());

                        cmd.Parameters.Clear();
                        cmd.Dispose();
                    }

                    fnSistema.sqlConn.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            //SI TIPO CONTRATO PERIODO ANTERIOR ES 1(FIJO) Y TIPO CONTRATO PERIODO ACTUAL ES INDEFINIDO
            if (tipoAntiguo == 1 && tipoNuevo == 0)
                cambia = true;

            return cambia;
        }

        //GENERAR FECHAS ACORDES A PERIODO EVAUADO
        private Hashtable GeneraFecha(int Periodo, DateTime FechaInicio, DateTime FechaTermino)
        {
            //201805
            Hashtable dates = new Hashtable();
            DateTime primerDia = DateTime.Now.Date;
            DateTime ultimoDia = DateTime.Now.Date;
            DateTime InicioFinal = DateTime.Now.Date;
            DateTime TerminoFinal = DateTime.Now.Date;

            primerDia = fnSistema.PrimerDiaMes(Periodo);
            ultimoDia = fnSistema.UltimoDiaMes(Periodo);

            //SI FECHA ESTA DENTRO DEL PERIODO LA DEJAMOS TAL CUAL
            if ((FechaInicio >= primerDia && FechaInicio <= ultimoDia) && (FechaTermino >= primerDia && FechaTermino <= ultimoDia))
            {
                InicioFinal = FechaInicio;
                TerminoFinal = FechaTermino;
            }
            //SI FECHA INICIO NO ESTA DENTRO DEL RANGO (MENOR A RANGO PERIODO)
            //PRIMER DIA MES
            if (FechaInicio < primerDia)
            {
                InicioFinal = primerDia;
            }
            //SI FECHA INICIO ESTA DENTRO DEL RANGO 
            if (FechaInicio >= primerDia && FechaInicio <= ultimoDia)
            {
                InicioFinal = FechaInicio;
            }
            //SI FECHA TERMINO NO ESTA DENTRO DEL RANGO (ES MAYOR AL RANGO)
            //FECHA TERMINO ES EL ULTIMO DIA DEL MES
            if (FechaTermino > ultimoDia)
            {
                TerminoFinal = ultimoDia;
            }
            //SI FECHA TERMINO ESTA DENTRO DEL RANGO
            if (FechaTermino >= primerDia && FechaTermino <= ultimoDia)
            {
                TerminoFinal = FechaTermino;
            }

            dates.Add("inicio", InicioFinal);
            dates.Add("termino", TerminoFinal);

            return dates;
        }

        //RUT PAGADORA SUBSIDIOS (PARA EL CASO DE LAS LICENCIAS)
        public string RutPagadoraSubsidio(int periodo, string contrato)
        {
            string sql = " SELECT isapre.rut as rut from trabajador " +
                        " INNER JOIN isapre ON isapre.id = trabajador.salud " +
                        " WHERE contrato = @pContrato AND anomes = @pPeriodo";

            SqlCommand cmd;
            string rut = "";
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pContrato", contrato));
                        cmd.Parameters.Add(new SqlParameter("@pPeriodo", periodo));

                        rut = (string)cmd.ExecuteScalar();

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return rut;
        }

        //BUSCAR FECHAS DENTRO DE PERIODO
        private bool FechaEnPeriodo(int Periodo, DateTime FechaInicio, DateTime FechaTermino)
        {
            bool correcto = false;
            DateTime primerDia = DateTime.Now.Date;
            DateTime ultimoDia = DateTime.Now.Date;

            primerDia = fnSistema.PrimerDiaMes(Periodo);
            ultimoDia = fnSistema.UltimoDiaMes(Periodo);

            //12-04-2018 --> 25-06-2018
            if ((FechaInicio >= primerDia && FechaInicio <= ultimoDia) || (FechaTermino >= primerDia && FechaTermino <= ultimoDia))
            {
                correcto = true;
            }

            //RECORRER RANGO FECHAS
            while (FechaInicio <= FechaTermino)
            {
                if (FechaInicio >= primerDia && FechaInicio <= ultimoDia)
                { correcto = true; break; }

                //++DAY
                FechaInicio = FechaInicio.AddDays(1);
            }

            return correcto;
        }

        //MOVIMIENTO DE PERSONAL
        /// <summary>
        /// Permite obtener movimientos de personal que hayan ocurrido en el periodo evaluado.
        /// </summary>
        /// <param name="periodo">Periodo consultado.</param>
        /// <param name="contrato">Numero de contrato asociado.</param>
        /// <returns></returns>
        public List<MovimientoPersonal> GetMovimientoPersonal(int periodo, string contrato)
        {
            //0 - SIN MOVIMIENTO
            //1 - CONTRATACION A PLAZO INDEFINIDO (PERSONA QUE SE INGRESA POR PRIMERA VEZ EN EL PERIODO OBSERVADO)
            //2 - RETIRO
            //3 - SUBSIDIOS
            //4 - PERMISO SIN GOSE DE SUELDO
            //5 - INCORPORACION EN EL LUGAR DE TRABAJO
            //6 - ACCIDENTES DEL TRABAJO
            //7 - CONTRATACION A PLAZO FIJO (PERSONA QUE SE INGRESA POR PRIMERA VEZ EN EL PERIODO OBSERVADO)
            //8 - CAMBIO CONTRATO PLAZO FIJO A PLAZO INDEFINIDO
            //11 - OTROS MOVIMIENTOS (AUSENTISMOS)
            //12 - RELIQUIDACION, PREMIO, BONO
            //13- Suspension contrato acto  de autoridad
            //14- Suspension contrato por pacto
            //15- Reduccion de Jornada

            string sqlLicencias = "", sqlPermisos = "", sqlAccidentes = "", sqlAusentismos = "", sqlCambioCont = "", sqlFiniquito = "";
            string sqlContFijo = "", sqlActoAutoridad = "", sqlPacto = "", sqlReduc = "";
            bool cambioFijoIndef = false;
            bool primerContrato = false;
            SqlCommand cmd;
            SqlDataReader rd;
            List<MovimientoPersonal> Movimientos = new List<MovimientoPersonal>();
            Hashtable Fechas = new Hashtable();
            cambioFijoIndef = CambioContratoFijoaIndefinido(periodo, contrato);
            //VERIFICAR SI ES EL PRIMER CONTRATO
            primerContrato = Persona.PrimerContrato(contrato, periodo);

            sqlLicencias = " SELECT fechaevento, fecfin FROM AUSENTISMO " +
                         " where contrato = @pContrato " +
                         " AND (motivo = 1 OR motivo = 2)";

            sqlPermisos = "SELECT fechaevento, fecfin FROM ausentismo " +
                          " WHERE contrato=@pContrato  " +
                          " AND motivo = 3 AND rebsueldo=1";

            sqlAccidentes = "SELECT fechaevento, fecfin FROM ausentismo " +
                            " WHERE contrato=@pContrato  " +
                            " AND motivo = 5";

            sqlAusentismos = "SELECT fechaevento, fecfin FROM ausentismo " +
                            " WHERE contrato=@pContrato  " +
                            " AND motivo = 4";

            sqlCambioCont = "SELECT ingreso, salida FROM trabajador WHERE contrato=@pContrato AND anomes=@pPeriodo";

            //BUSCAMOS HISTORICO PARA CONTRATO
            // 0--> INDEFINIDO
            // 1--> FIJO
            sqlContFijo = "SELECT ingreso, salida, tipocontrato FROM trabajador WHERE contrato=@pContrato AND anomes=@pPeriodo AND (ingreso>=@pPrimerDia AND ingreso<=@pUltimoDia)";

            /*RETIRO CODE 2*/
            sqlFiniquito = "SELECT ingreso, salida FROM trabajador WHERE contrato=@pContrato AND anomes=@pPeriodo AND salida<=@pFecha";

            //Suspension por acto de autoridad
            sqlActoAutoridad = "SELECT fechaAplic, fecfinApli FROM AUSENTISMO " +
                               " where contrato = @pContrato " +
                              " AND motivo = 13";

            //Suspension por pacto
            sqlPacto = "SELECT fechaAplic, fecfinApli FROM AUSENTISMO " +
                              " where contrato = @pContrato " +
                             " AND motivo = 14";

            //Suspension por reduccion de jornada laboral
            sqlReduc = "SELECT fechaAplic, fecfinApli FROM AUSENTISMO " +
                              " where contrato = @pContrato " +
                             " AND motivo = 15";




            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    /*LICENCIAS --> SUBSIDIOS CODE 3*/
                    using (cmd = new SqlCommand(sqlLicencias, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pContrato", contrato));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //CARGAR DATOS
                                //SI LA FECHA ESTA DENTRO DEL PERIODO
                                if (FechaEnPeriodo(periodo, Convert.ToDateTime(rd["fechaevento"]), Convert.ToDateTime(rd["fecfin"])))
                                {
                                    Fechas = GeneraFecha(periodo, Convert.ToDateTime(rd["fechaevento"]), Convert.ToDateTime(rd["fecfin"]));
                                    Movimientos.Add(new MovimientoPersonal()
                                    {
                                        codMovimiento = "3",
                                        inicioMovimiento = Convert.ToDateTime(Fechas["inicio"]),
                                        terminoMovimiento = Convert.ToDateTime(Fechas["termino"])
                                    });
                                }
                            }
                        }
                        cmd.Dispose();
                        rd.Close();
                        Fechas.Clear();
                    }

                    /*PERMISOS CODE 4*/
                    using (cmd = new SqlCommand(sqlPermisos, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pContrato", contrato));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //cargar data
                                //SI FECHA ESTA DENTRO DEL PERIODO
                                if (FechaEnPeriodo(periodo, Convert.ToDateTime(rd["fechaevento"]), Convert.ToDateTime(rd["fecfin"])))
                                {
                                    Fechas = GeneraFecha(periodo, Convert.ToDateTime(rd["fechaevento"]), Convert.ToDateTime(rd["fecfin"]));

                                    Movimientos.Add(new MovimientoPersonal()
                                    {
                                        codMovimiento = "4",
                                        inicioMovimiento = Convert.ToDateTime(Fechas["inicio"]),
                                        terminoMovimiento = Convert.ToDateTime(Fechas["termino"])
                                    });
                                }
                            }
                        }
                        cmd.Dispose();
                        Fechas.Clear();
                        rd.Close();
                    }

                    /*ACCIDENTES CODE 6*/
                    using (cmd = new SqlCommand(sqlAccidentes, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pContrato", contrato));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //CARGAR DATA
                                //SI FECHA EXISTE EN PERIODO
                                if (FechaEnPeriodo(periodo, Convert.ToDateTime(rd["fechaevento"]), Convert.ToDateTime(rd["fecfin"])))
                                {
                                    Fechas = GeneraFecha(periodo, Convert.ToDateTime(rd["fechaevento"]), Convert.ToDateTime(rd["fecfin"]));

                                    Movimientos.Add(new MovimientoPersonal()
                                    {
                                        codMovimiento = "6",
                                        inicioMovimiento = Convert.ToDateTime(Fechas["inicio"]),
                                        terminoMovimiento = Convert.ToDateTime(Fechas["termino"])
                                    });
                                }
                            }
                        }
                        cmd.Dispose();
                        rd.Close();
                        Fechas.Clear();
                    }

                    /*AUSENTISMOS CODE 11*/
                    using (cmd = new SqlCommand(sqlAusentismos, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pContrato", contrato));
                        // cmd.Parameters.Add(new SqlParameter("@pPeriodo", Periodo));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //SI FECHA EXISTE EN PERIODO
                                if (FechaEnPeriodo(periodo, Convert.ToDateTime(rd["fechaevento"]), Convert.ToDateTime(rd["fecfin"])))
                                {
                                    Fechas = GeneraFecha(periodo, Convert.ToDateTime(rd["fechaevento"]), Convert.ToDateTime(rd["fecfin"]));

                                    Movimientos.Add(new MovimientoPersonal()
                                    {
                                        codMovimiento = "11",
                                        inicioMovimiento = Convert.ToDateTime(Fechas["inicio"]),
                                        terminoMovimiento = Convert.ToDateTime(Fechas["termino"])
                                    });
                                }
                            }
                        }
                        cmd.Dispose();
                        rd.Close();
                        Fechas.Clear();
                    }

                    /*CONTRATACION A PLAZO FIJO O INDEFINIDO CODE 1 - 7*/
                    if (primerContrato)
                    {
                        using (cmd = new SqlCommand(sqlContFijo, fnSistema.sqlConn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@pPeriodo", periodo));
                            cmd.Parameters.Add(new SqlParameter("@pContrato", contrato));
                            cmd.Parameters.Add(new SqlParameter("@pPrimerDia", fnSistema.PrimerDiaMes(periodo)));
                            cmd.Parameters.Add(new SqlParameter("@pUltimoDia", fnSistema.UltimoDiaMes(periodo)));

                            rd = cmd.ExecuteReader();
                            if (rd.HasRows)
                            {
                                while (rd.Read())
                                {
                                    //Fechas = GeneraFecha(periodo, Convert.ToDateTime(rd["ingreso"]), Convert.ToDateTime(rd["salida"]));

                                    //SI CONTRATO ES INDEFINIDO CODE 1
                                    //SI CONTRATO ES FIJO CODE 7
                                    if (Convert.ToInt32(rd["tipocontrato"]) == 0)
                                    {
                                        Movimientos.Add(new MovimientoPersonal()
                                        {
                                            codMovimiento = "1",
                                            inicioMovimiento = Convert.ToDateTime(rd["ingreso"]),
                                            terminoMovimiento = Convert.ToDateTime(rd["salida"])
                                        });
                                    }

                                    if (Convert.ToInt32(rd["tipocontrato"]) == 1)
                                    {
                                        Movimientos.Add(new MovimientoPersonal()
                                        {
                                            codMovimiento = "7",
                                            inicioMovimiento = Convert.ToDateTime(rd["ingreso"]),
                                            terminoMovimiento = Convert.ToDateTime(rd["salida"])
                                        });
                                    }
                                }
                            }

                            cmd.Dispose();
                            rd.Close();
                            Fechas.Clear();
                        }
                    }

                    /*TERMINO DE CONTRATO CODE 2*/
                    using (cmd = new SqlCommand(sqlFiniquito, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pContrato", contrato));
                        cmd.Parameters.Add(new SqlParameter("@pPeriodo", periodo));
                        cmd.Parameters.Add(new SqlParameter("@pFecha", fnSistema.UltimoDiaMes(periodo)));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                Movimientos.Add(new MovimientoPersonal() {
                                    codMovimiento = "2",
                                    inicioMovimiento = Convert.ToDateTime(rd["ingreso"]),
                                    terminoMovimiento = Convert.ToDateTime(rd["salida"])
                                });
                            }
                        }

                        cmd.Dispose();


                    }

                    /*Suspension por acto de autoridad cd:13*/
                    using (cmd = new SqlCommand(sqlActoAutoridad, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pContrato", contrato));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //CARGAR DATOS
                                //SI LA FECHA ESTA DENTRO DEL PERIODO
                                if (FechaEnPeriodo(periodo, Convert.ToDateTime(rd["fechaaplic"]), Convert.ToDateTime(rd["fecfinapli"])))
                                {
                                    Fechas = GeneraFecha(periodo, Convert.ToDateTime(rd["fechaaplic"]), Convert.ToDateTime(rd["fecfinapli"]));
                                    TimeSpan time = Convert.ToDateTime(Fechas["termino"]) - Convert.ToDateTime(Fechas["inicio"]);

                                    Movimientos.Add(new MovimientoPersonal()
                                    {
                                        codMovimiento = "13",
                                        inicioMovimiento = Convert.ToDateTime(Fechas["inicio"]),
                                        terminoMovimiento = Convert.ToDateTime(Fechas["termino"]),
                                        Dias = time.Days + 1
                                    });
                                }
                            }
                        }
                        cmd.Dispose();
                        rd.Close();
                        Fechas.Clear();
                    }

                    /*Suspension por pacto cd:14*/
                    using (cmd = new SqlCommand(sqlPacto, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pContrato", contrato));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //CARGAR DATOS
                                //SI LA FECHA ESTA DENTRO DEL PERIODO
                                if (FechaEnPeriodo(periodo, Convert.ToDateTime(rd["fechaaplic"]), Convert.ToDateTime(rd["fecfinapli"])))
                                {
                                    Fechas = GeneraFecha(periodo, Convert.ToDateTime(rd["fechaaplic"]), Convert.ToDateTime(rd["fecfinapli"]));
                                    TimeSpan time = Convert.ToDateTime(Fechas["termino"]) - Convert.ToDateTime(Fechas["inicio"]);
                                    Movimientos.Add(new MovimientoPersonal()
                                    {
                                        codMovimiento = "14",
                                        inicioMovimiento = Convert.ToDateTime(Fechas["inicio"]),
                                        terminoMovimiento = Convert.ToDateTime(Fechas["termino"]),
                                        Dias = time.Days + 1
                                    });
                                }
                            }
                        }
                        cmd.Dispose();
                        rd.Close();
                        Fechas.Clear();
                    }

                    /*Suspension por acto de autoridad cd:15*/
                    using (cmd = new SqlCommand(sqlReduc, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pContrato", contrato));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //CARGAR DATOS
                                //SI LA FECHA ESTA DENTRO DEL PERIODO
                                if (FechaEnPeriodo(periodo, Convert.ToDateTime(rd["fechaaplic"]), Convert.ToDateTime(rd["fecfinapli"])))
                                {
                                    Fechas = GeneraFecha(periodo, Convert.ToDateTime(rd["fechaaplic"]), Convert.ToDateTime(rd["fecfinapli"]));
                                    TimeSpan time = Convert.ToDateTime(Fechas["termino"]) - Convert.ToDateTime(Fechas["inicio"]);
                                    Movimientos.Add(new MovimientoPersonal()
                                    {
                                        codMovimiento = "15",
                                        inicioMovimiento = Convert.ToDateTime(Fechas["inicio"]),
                                        terminoMovimiento = Convert.ToDateTime(Fechas["termino"]),
                                        Dias = time.Days + 1
                                    });
                                }
                            }
                        }
                        cmd.Dispose();
                        rd.Close();
                        Fechas.Clear();
                        fnSistema.sqlConn.Close();
                    }

                }
                fnSistema.sqlConn.Close();
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return Movimientos;
        }

        //CODIGO CAJA COMPENSACION
        private string GetCodigoCaja()
        {
            string code = "", sql = "";
            sql = "select cajaCompensacion.codprevired as code from empresa " +
                  " INNER JOIN cajacompensacion ON cajaCompensacion.id = empresa.nombreCCAF";
            SqlCommand cmd;

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {

                        code = (string)cmd.ExecuteScalar();

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }


            return code;
        }
        //CODIGO MUTUAL
        private string GetCodigoMutual()
        {
            string code = "", sql = "";
            sql = "select mutual.codPrevired as code from empresa " +
                  "INNER JOIN mutual ON mutual.id = empresa.nombreMut";

            SqlCommand cmd;

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {

                        code = (string)cmd.ExecuteScalar();

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }


            return code;
        }

        //IMPONIBLES QUE NO TENGAN LICENCIA
        private double ImponibleAnteriorGenerico(string contrato, int PeriodoActual)
        {
            string sql = "SELECT distinct anomes FROM itemtrabajador WHERE contrato=@pcontrato AND anomes < @periodo ORDER BY anomes desc";
            SqlCommand cmd;
            SqlDataReader rd;
            double DiasLic = 0;

            //LISTA PARA GUARDAR TODOS LOS PERIODOS ENCONTRADOS QUE CUMPLAN LA CONDICION DE LA CONSULTA
            List<int> periodos = new List<int>();
            bool aus = false;
            double imponible = 0;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pcontrato", contrato));
                        cmd.Parameters.Add(new SqlParameter("@periodo", PeriodoActual));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //LLENAMOS LISTA CON PERIODOS ENCONTRADOS
                                periodos.Add((int)rd["anomes"]);
                            }
                        }
                        else
                        {
                            //NO SE ENCONTRARON PERIODOS ANTERIORES PARA EL MISMO CONTRATO :-(
                        }
                    }
                    //LIBERAR RECURSOS
                    cmd.Dispose();
                    rd.Close();
                    fnSistema.sqlConn.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            //UNA VEZ LLENADA LA LISTA CON TODOS LOS PERIODOS QUE CUMPLEN LA CONDICION (DE MAYOR A MENOR)
            //VOLVEMOS A PREGUNTAR A LA BD EN ESTE CASO SI TIENEN AUSENTISMO
            //RECORREMOS LISTA...

            if (periodos.Count > 0)
            {
                foreach (var anomes in periodos)
                {
                    //VERIFICAMOS QUE LA PERSONA HAYA TRABAJADO EL MES COMPLETO
                    double diasTr = 0;
                    diasTr = Labour.Calculo.GetValueFromCalculoMensaul(contrato, anomes, "sysdiastr");
                    aus = Persona.TieneAusentismos(contrato, anomes);

                    if (diasTr == 30)
                    {
                        imponible = Labour.Calculo.GetValueFromCalculoMensaul(contrato, anomes, "systimp");
                        //HACEMOS BREAK AL BUCLE PARA NO SEGUIR ITERANDO...
                        break;
                    }
                }
            }

            //SI IMPONIBLE SIGUE SIENDO 0 ES PORQUE TODOS LOS PERIODOS ANTERIORES TIENEN LICENCIA
            //EN ESTE CASO EL IMPONIBLE A USAR SERA LA SUMATORIA DE LOS VALORES ORIGINALES (HABERES IMPONIBLES)
            //PARA EL PERIODO EN CURSO...
            if (imponible == 0)
            {
                imponible = -1;
            }

            //RETORNAMOS VALOR IMPONIBLE CUALQUIERA SEA EL CASO
            return imponible;
        }

        //IMPONIBLE SEGURO CESANTIA
        private double GetImponibleSeguroCes(string pContrato, int pPeriodo)
        {
            //OBTENER TODOS LOS HABERES IMPONIBLES PROPORCIONALES
            string sqlProporcional = "select valorcalculado from itemtrabajador where contrato=@pContrato " +
                                    "AND anomes=@pPeriodo AND tipo=1 AND proporcional = 1 AND " +
                                    "orden < (SELECT orden FROM item WHERE coditem = 'GRATIFI')";

            //OBTENER TODOS LOS HABERES IMPONIBLES NO PROPORCIONALES A EXCEPCION DE LA GRATIFICACION
            string sqlNoProp = "select valorcalculado from itemtrabajador " +
                               " where contrato=@pContrato AND anomes = @pPeriodo " +
                                "AND tipo = 1 AND proporcional = 0 AND coditem<> 'GRATIFI' " +
                                "AND orden < (SELECT orden FROM item WHERE coditem='GRATIFI')";

            //VALORES HABERES MAYORES A GRATIFICACION (N° ORDEN)
            string sqlRest = "select ISNULL(SUM(valorcalculado), 0) as suma FROM itemtrabajador " +
                                " where contrato = @pContrato AND anomes = @pPeriodo " +
                                " AND tipo = 1 " +
                                " AND orden > (SELECT orden FROM item WHERE coditem='GRATIFI')";

            SqlCommand cmd;
            SqlDataReader rd;
            double imp = 0, impProp = 0, impNopProp = 0, topeSeg = 0, calculoFormula = 0, resultado = 0, impRest = 0, impAnterior = 0, diaslic = 0, diastr = 0;

            int numItem = 0;
            //string formula = FormulaSistema.GetValueFormula("FGRATIF");
            //numItem = FormulaSistema.GetItemNumber(pContrato, pPeriodo, "GRATIFI");
            //topeSeg = varSistema.ObtenerValorLista("systopeseg");
            topeSeg = Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(pContrato, pPeriodo, "systopeseg"));
            diaslic = Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(pContrato, pPeriodo, "sysdiaslic"));
            diastr = Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(pContrato, pPeriodo, "sysdiastr"));
            bool ExisteimpAnterior = Labour.Calculo.ExisteImpAnterior(pContrato);

            //SI TIENE LICENCIAS DEBEMOS BUSCAR PERIODOS HACIA ATRÁS...
            if (diaslic != 0)
            {
                //Si hay licencias y no existe data en tabla datoscalculo
                //buscamos el monto
                if (diaslic > 0 && ExisteimpAnterior == false)
                {
                    impAnterior = ImponibleAnteriorGenerico(contrato, pPeriodo);
                    //impAnterior = Labour.Calculo.GetImpLicencia(pPeriodo, "", pContrato);
                    if (impAnterior == -1)
                        impAnterior = 0;
                }                    
                //SI HAY LICENCIAS PERO HAY IMPONIBLE ANTERIOR SOLO OBTENEMOS SU VALOR
                else if (diaslic > 0 && ExisteimpAnterior)
                    impAnterior = Labour.Calculo.ConsultaImpAnterior(contrato);


                //Si hay licencia el imponible que debemos considerar consta de dos montos:
                //1-  Imponible que haya tenido la persona en el mes
                //2-  Imponible proporcional por los dias de licencia
                //3- El imponible <<impAnterior>> corresponde a imponible completo, este 
                //se debe proporcional <<(ImpLic/30)*diasLic>>

                double impPropLic = 0;
                impPropLic = (impAnterior / 30) * diaslic;

                imp = Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(pContrato, pPeriodo, "systimp"));
                if (imp > topeSeg)
                    imp = topeSeg;

                resultado = Math.Round(impPropLic + imp);

                //resultado = impAnterior;

                
            
            }
            else
            {
                //SI NO TIENE LICENCIAS SOLO RETORNAMOS EL IMPONIBLE
                //resultado = varSistema.ObtenerValorLista("systimp");
                imp = Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(pContrato, pPeriodo, "systimp"));
                if (imp > topeSeg)
                    resultado = topeSeg;
                else if (imp <= topeSeg)
                    resultado = imp;
            }

            return resultado;
        }

        //OBTENER EL CODIGO DE SUCURSAL CONTRATO
        private string GetSucursal(string pContrato, int pPeriodo)
        {
            string code = "";
            string sql = "SELECT sucursal FROM trabajador WHERE contrato=@pContrato AND anomes=@pPeriodo";
            SqlCommand cmd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pContrato", pContrato));
                        cmd.Parameters.Add(new SqlParameter("@pPeriodo", pPeriodo));

                        code = (Convert.ToInt32(cmd.ExecuteScalar())).ToString();

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return code;
        }

        //OBTENER EL NUMERO DE FUN DEL TRABAJADOR (PARA EL CASO DE ISAPRE)
        private string GetFun(string pContrato, int pPeriodo)
        {
            string value = "";
            string sql = "SELECT fun FROM trabajador WHERE contrato=@pContrato AND anomes=@pPeriodo";
            SqlCommand cmd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pContrato", pContrato));
                        cmd.Parameters.Add(new SqlParameter("@pPeriodo", pPeriodo));

                        value = (string)cmd.ExecuteScalar();

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return value;
        }

        //AGREGAR CEROS A COTIZACION PACTADA EN CASO DE SER NECESARIA
        private string FormatearCotizacionPactada(string pValue)
        {
            if (pValue == "0") return "00000000";
            if (pValue.Length == 8) return pValue;
            string val = pValue;
            int restante = 8 - pValue.Length;

            if (restante > 0)
            {
                for (int i = 0; i < restante; i++)
                {
                    val = "0" + val;
                }
            }

            return val;

        }

        //OBTENER DATOS DE TRABAJADOR
        /// <summary>
        /// Entrega un hashtable con informacion del trabajador.
        /// </summary>
        /// <param name="contrato">Numero de contrato asociado al trabajador.</param>
        /// <param name="periodo">Periodo consulta.</param>
        /// <returns></returns>
        private Hashtable DataPersona(string contrato, int periodo)
        {
            Hashtable data = new Hashtable();

            SqlCommand cmd;
            SqlDataReader rd;

            //OBTENEMOS CADA CONTRATO DEL RUT ASOCIADO

            string sql = "SELECT contrato, anomes, apepaterno, apematerno, nombre, fechanac, sexo, nacion, jubilado, tramo, " +
                   " regimen, afp, salud, tipocontrato FROM trabajador WHERE " +
                   " contrato=@pContrato AND anomes=@pPeriodo";
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pContrato", contrato));
                        cmd.Parameters.Add(new SqlParameter("@pPeriodo", periodo));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //GUARDAMOS DATOS EN HASHTABLE
                                data.Add("apepaterno", rd["apepaterno"]);
                                data.Add("apematerno", rd["apematerno"]);
                                data.Add("nombre", rd["nombre"]);
                                data.Add("fechanac", rd["fechanac"]);
                                data.Add("sexo", rd["sexo"]);
                                data.Add("nacion", rd["nacion"]);
                                data.Add("jubilado", rd["jubilado"]);
                                data.Add("tramo", rd["tramo"]);
                                data.Add("regimen", rd["regimen"]);
                                data.Add("afp", rd["afp"]);
                                data.Add("salud", rd["salud"]);
                                data.Add("tipocontrato", rd["tipocontrato"]);
                                data.Add("contrato", rd["contrato"]);
                                data.Add("periodo", rd["anomes"]);
                            }
                        }
                    }
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                    rd.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return data;
        }

        /// <summary>
        /// Obtiene el centro de costo del trabajador.
        /// </summary>
        /// <param name="pContrato"></param>
        /// <param name="pPeriodo"></param>
        private string GetCentroCosto(string pContrato, int pPeriodo)
        {
            string sql = "select dato01 FROM trabajador INNER JOIN ccosto ON ccosto.id = trabajador.ccosto " +
                         "where CONTRATO = @pContrato AND anomes = @pPeriodo";
            SqlCommand cmd;
            SqlConnection cn;

            string data = "";
            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@pContrato", pContrato));
                            cmd.Parameters.Add(new SqlParameter("@pPeriodo", pPeriodo));

                            object dt = cmd.ExecuteScalar();
                            if (dt != DBNull.Value)
                            {
                                data = Convert.ToString(dt);
                            }

                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                //ERROR...
            }

            return data;
        }

        /// <summary>
        /// Calculo y setea cada columna del archivo de previred.
        /// </summary>
        /// <param name="ListadoMovimientos">Listado de movimientos empleados.</param>
        /// <param name="pContrato">Numero asociado de contrato.</param>
        /// <param name="pPeriodo">Period consulta.</param>
        /// <param name="pTipoLinea">Tipo linea para archivo previred.</param>
        /// <param name="pDataPrevired">Listado para reporte previred</param>
        /// <returns></returns>
        private List<string> Calculo(List<MovimientoPersonal> ListadoMovimientos, string pContrato, int pPeriodo, string pTipoLinea, List<ReportePrevired> pDataPrevired)
        {
            double days = 0, daysLic = 0, imponible, topeAfp = 0, topeSalud = 0, Fonasa = 0, daySus13 = 0, daySus14 = 0;
            string diasTrabajados = "", cargaNormal = "", cargaMaternal = "", cargaInvalido = "", montoCarga = "", montoCargaRetroactiva = "";
            string codSalud = "";
            bool MenorEdad = false, AdultoMayor = false, tieneAccidente = false, tienePermiso = false, tieneAusencia = false, tieneLicencia = false;
            double MontoTotalCargasFamiliares = 0;
            double ImpoAnterior = 0, Prestamos = 0, AhorroCaja = 0, SeguroVidaCaja =0;
            int sexo = 0;
            bool EsSuspension = false, isFirst = false;
            int count = 0;
            List<string> CadenasArchivo = new List<string>();

            //INFORMACION PERSONA
            Persona persona = Persona.GetInfo(pContrato, pPeriodo);
            ReportePrevired DataPreviredAfp;
            ReportePrevired DataFonasaPrevired;
            ReportePrevired DataIsaprePrevired;
            ReportePrevired DataIpsPrevired;
            ReportePrevired DataMutual;
            ReportePrevired DataCaja;

            ReportePrevired DataPreviredAfpNormal;
            ReportePrevired DataFonasaPreviredNormal;
            ReportePrevired DataIsaprePreviredNormal;
            ReportePrevired DataIpsPreviredNormal;
            ReportePrevired DataMutualNormal;
            ReportePrevired DataCajaNormal;

            Hashtable DataSuspension;

            List<MovimientoPersonal> Suspensiones = new List<MovimientoPersonal>();

            if (persona != null)
            {
                #region "Datos de inicio"
                Contrato = pContrato;
                days = Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(pContrato, pPeriodo, "sysdiastr"), MidpointRounding.AwayFromZero);
                diasTrabajados = days.ToString();

                //Dias de suspension (cod:13)
                daySus13 = Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(pContrato, pPeriodo, "sysdiassp13"), MidpointRounding.AwayFromZero);
                daySus14 = Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(pContrato, pPeriodo, "sysdiassp14"), MidpointRounding.AwayFromZero);
                daysLic = Labour.Calculo.GetValueFromCalculoMensaul(pContrato, pPeriodo, "sysdiaslic");

                IndiceMensual I = new IndiceMensual();
                I.SetInfoMes(fnSistema.fnObtenerPeriodoAnterior(Labour.Calculo.PeriodoObservado));
                //topeAfpAnterior = Math.Round(I.TopeAfp * I.Uf);

                //Campos 85-87-88
                Prestamos = Math.Round(Labour.Calculo.GetPrestamos(persona.Contrato, persona.PeriodoPersona));
                AhorroCaja = Math.Round(Labour.Calculo.GetLeasing(persona.Contrato, persona.PeriodoPersona));
                SeguroVidaCaja = Math.Round(Labour.Calculo.GetSeguros(persona.Contrato, persona.PeriodoPersona));

                //OBTENER LAS CARGAS FAMILIARES SI EXISTEN
                Familiar fam = new Familiar(pContrato, pPeriodo);
                cargaNormal = fam.GetNumCargasSimplesV2().ToString();
                cargaMaternal = fam.GetNumCargasMaternal().ToString();
                cargaInvalido = fam.GetNumCargasInvalidezV2().ToString();
                montoCarga = fam.GetTotalAsignacionesSimples().ToString();
                montoCargaRetroactiva = fam.GetTotalAsignacionesRetroc().ToString();
                MontoTotalCargasFamiliares = Math.Round(Convert.ToDouble(montoCarga) + Convert.ToDouble(montoCargaRetroactiva));

                //DataCaja.AsignacionFam = MontoTotalCargasFamiliares;

                /*PARA VER SI EN EL PERIODO EVALUADO SE REALIZA CALCULA DE ASIGNACIONES FAMILIARES*/
                if (fam.CalculaAsignacion("ASIGFAM") == false)
                    cargaNormal = "0";
                if (fam.CalculaAsignacion("ASIGMAT") == false)
                    cargaMaternal = "0";
                if (fam.CalculaAsignacion("ASIGINV") == false)
                    cargaInvalido = "0";

                //CALCULO PARA MONTOS REQUERIDOS
                // Haberes hab = new Haberes(pContrato, pPeriodo);
                // hab.CalculoPrevired(Convert.ToDouble(days));

                Fonasa = Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(pContrato, pPeriodo, "sysfonasa"));
                //DataSourcePrev.CotizacionFonasa = Fonasa;

                //imponible = Math.Round(varSistema.ObtenerValorLista("systimp"));
                imponible = Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(pContrato, pPeriodo, "systimp"));
                //Imponible mes anterior                           

                //topeAfp = Math.Round(varSistema.ObtenerValorLista("systopeafp"));
                topeAfp = Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(pContrato, pPeriodo, "systopeafp"));
                //topeSalud = Math.Round(varSistema.ObtenerValorLista("systopesalud"));
                topeSalud = Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(pContrato, pPeriodo, "systopesalud"));

                AdultoMayor = fnSistema.AdultoMay(Convert.ToDateTime(persona.Nacimiento), persona.Sexo);
                MenorEdad = fnSistema.MenorDeEdad(Convert.ToDateTime(persona.Nacimiento));
                codSalud = GetCodigoSalud(persona.codSalud);

                tieneLicencia = TieneLicencias(pPeriodo, pContrato);
                tienePermiso = TienePermisos(pPeriodo, pContrato);
                tieneAusencia = TieneAusentismos(pPeriodo, pContrato);
                tieneAccidente = TieneAccidenteTrabajo(pPeriodo, pContrato);

                string RegPrevisional = GetRegimenPrevision(persona.Regimen, persona.Nacimiento);
                #endregion              

                //Ecuentra movimientos de tipo de suspension
                //Si hay los guarda en otra lista y los elimina de la lista principal
                if (ListadoMovimientos.Count > 0)
                {
                    Suspensiones = new List<MovimientoPersonal>();
                    Suspensiones = ListadoMovimientos.FindAll(x => x.codMovimiento == "13" || x.codMovimiento == "14" || x.codMovimiento == "15");

                    //Eliminar elementos que se encontraron
                    if (Suspensiones.Count > 0)
                        ListadoMovimientos.RemoveAll(x => x.codMovimiento == "13" || x.codMovimiento == "14" || x.codMovimiento == "15");

                }

                //if (Suspensiones.Count > 0)
                //{
                //    string TipoLineaSus = "00";
                    
                //    //Recorremos cada suspension y le hacemos el tratamiento correspondiente...
                //    foreach (MovimientoPersonal mov in Suspensiones)
                //    {

                //        #region "DATA REPORTE"
                //        DataPreviredAfp = new ReportePrevired();
                //        DataPreviredAfp.Tipo = "1 - AFP";
                //        DataPreviredAfp.Suspesion = true;
                //        DataFonasaPrevired = new ReportePrevired();
                //        DataFonasaPrevired.Tipo = "2 - FONASA";
                //        DataFonasaPrevired.Suspesion = true;
                //        DataIsaprePrevired = new ReportePrevired();
                //        DataIsaprePrevired.Tipo = "3 - ISAPRE";
                //        DataIsaprePrevired.Suspesion = true;
                //        DataIpsPrevired = new ReportePrevired();
                //        DataIpsPrevired.Tipo = "4 - IPS";
                //        DataIpsPrevired.Suspesion = true;
                //        DataMutual = new ReportePrevired();
                //        DataMutual.Tipo = "5 - MUTUAL";
                //        DataMutual.Suspesion = true;
                //        DataCaja = new ReportePrevired();
                //        DataCaja.Tipo = "6 - CAJA COMPENSACION";
                //        DataCaja.Suspesion = true;

                //        //Entidad para reporte
                //        DataPreviredAfp.CentroCosto = persona.centro;
                //        DataPreviredAfp.Sucursal = persona.sucursal;
                //        DataPreviredAfp.Cargo = persona.Cargo;
                //        DataPreviredAfp.Area = persona.NombreArea;
                //        DataPreviredAfp.Entidad = persona.NombreAfp + " *Suspension";

                //        DataFonasaPrevired.CentroCosto = persona.centro;
                //        DataFonasaPrevired.Sucursal = persona.sucursal;
                //        DataFonasaPrevired.Cargo = persona.Cargo;
                //        DataFonasaPrevired.Area = persona.NombreArea;
                //        DataFonasaPrevired.Entidad = persona.codSalud == 1 ? (persona.NombreSalud + " *Suspension") : "";

                //        DataIsaprePrevired.CentroCosto = persona.centro;
                //        DataIsaprePrevired.Sucursal = persona.sucursal;
                //        DataIsaprePrevired.Cargo = persona.Cargo;
                //        DataIsaprePrevired.Area = persona.NombreArea;
                //        DataIsaprePrevired.Entidad = persona.codSalud > 1 ? (persona.NombreSalud + " *Suspension") : "";

                //        DataIpsPrevired.CentroCosto = persona.centro;
                //        DataIpsPrevired.Sucursal = persona.sucursal;
                //        DataIpsPrevired.Cargo = persona.Cargo;
                //        DataIpsPrevired.Area = persona.NombreArea;
                //        DataIpsPrevired.EntidadFonasa = persona.NombreSalud;
                //        DataIpsPrevired.Entidad = persona.NombreCajaPrev + " *Suspension";

                //        DataMutual.CentroCosto = persona.centro;
                //        DataMutual.Sucursal = persona.sucursal;
                //        DataMutual.Cargo = persona.Cargo;
                //        DataMutual.Area = persona.NombreArea;
                //        DataMutual.Entidad = "Ley Sanna * Suspension";

                //        DataCaja.CentroCosto = persona.centro;
                //        DataCaja.Sucursal = persona.sucursal;
                //        DataCaja.Cargo = persona.Cargo;
                //        DataCaja.Area = persona.NombreArea;
                //        DataCaja.Entidad = "Caja * Suspension";

                //        #endregion

                //        double FonasaSus = 0;
                //        if (mov.codMovimiento == "13")
                //            FonasaSus = Labour.Calculo.GetValueFromCalculoMensaul(persona.Contrato, persona.PeriodoPersona, "syssaludsp13");
                //        if (mov.codMovimiento == "14")
                //            FonasaSus = Labour.Calculo.GetValueFromCalculoMensaul(persona.Contrato, persona.PeriodoPersona, "syssaludsp14");
                //        if (mov.codMovimiento == "15")
                //            FonasaSus = Labour.Calculo.GetValueFromCalculoMensaul(persona.Contrato, persona.PeriodoPersona, "syssaludsp15");

                //        DatosTrabajadorSuspension(diasTrabajados, pTipoLinea, cargaNormal, cargaMaternal, cargaInvalido, montoCarga,
                //        mov, montoCargaRetroactiva, persona, daySus13.ToString(), "", "", TipoLineaSus);

                //        ////DATOS AFP
                //        //DatosAfpSuspension(AdultoMayor, topeAfp, imponible, persona, DataPreviredAfp, mov, daySus13, daySus14, 0);

                //        //DATOS AHORRO PREVISIONAL VOLUNTARIO
                //        DatosAhorroVoluntarioIndividualSuspension(persona, DataPreviredAfp);

                //        //DATOS AHORRO PREVISIONAL COLECTIVO
                //        DatosAhorroVoluntarioColectivoSuspension();

                //        //DATOS AFILIADO VOLUNTARIO
                //        DatosAfiliadoVoluntarioSuspension();

                //        //DATOS IPS
                //        //DatosIpsSuspension(codSalud, imponible, FonasaSus,
                //        //    persona, MontoTotalCargasFamiliares.ToString(), RegPrevisional, DataFonasaPrevired, DataIpsPrevired, mov, daySus13, daySus14, 0);

                //        ////DATOS SALUD
                //        //DatosSaludSuspension(codSalud, imponible, topeSalud, DataIsaprePrevired, persona, mov, daySus13, daySus14, 0);

                //        ////DATOS CAJA DE COMPENSACION
                //        //DatosCajaSuspension(imponible, topeAfp, codSalud, DataCaja, mov, persona, daySus13, daySus14, 0, I);

                //        ////DATOS MUTULIDAD
                //        //DatosMutualidadSuspension(imponible, pContrato, pPeriodo, topeAfp, DataMutual, mov, persona, daySus13, 0, 0);

                //        ////DATOS ADMINISTRADOR SEGURO CENSATIA
                //        //DatosAdministradorSeguroSuspension(AdultoMayor, MenorEdad, imponible, pContrato, pPeriodo, persona, DataPreviredAfp, daySus13, 0, 0, mov);

                //        //DATOS PAGADOR SUBSIDIO
                //        DatosPagadorSubsidio(ListadoMovimientos, pContrato);

                //        //Agrega a listado
                //        if (DataPreviredAfp.Cotizacion > 0 || DataPreviredAfp.SegAfiliado > 0 || DataPreviredAfp.SegInv > 0 || DataPreviredAfp.SegEmpresa > 0)
                //            pDataPrevired.Add(DataPreviredAfp);
                //        if (DataMutual.Cotizacion > 0)
                //            pDataPrevired.Add(DataMutual);
                //        if (DataCaja.Cotizacion > 0 || DataCaja.AsignacionFam > 0)
                //            pDataPrevired.Add(DataCaja);
                //        if (DataFonasaPrevired.Cotizacion > 0 || DataFonasaPrevired.CotizacionFonasa > 0)
                //            pDataPrevired.Add(DataFonasaPrevired);
                //        if (DataIsaprePrevired.Cotizacion > 0)
                //            pDataPrevired.Add(DataIsaprePrevired);
                //        if (DataIpsPrevired.Cotizacion > 0 || DataIpsPrevired.CotizacionFonasa > 0)
                //            pDataPrevired.Add(DataIpsPrevired);

                //        CadenasArchivo.Add(cadenaCarga);
                //        cadenaCarga = "";

                //    }
                //}

                //Tiene o no movimientos pero no hay suspensiones
                if (Suspensiones.Count == 0)
                {
                    //Tipo linea cero

                    #region "DATA REPORTE"
                    DataPreviredAfp = new ReportePrevired();
                    DataPreviredAfp.Tipo = "1 - AFP";
                    DataPreviredAfp.Suspesion = false;
                    DataFonasaPrevired = new ReportePrevired();
                    DataFonasaPrevired.Tipo = "2 - FONASA";
                    DataFonasaPrevired.Suspesion = false;
                    DataIsaprePrevired = new ReportePrevired();
                    DataIsaprePrevired.Tipo = "3 - ISAPRE";
                    DataIsaprePrevired.Suspesion = false;
                    DataIpsPrevired = new ReportePrevired();
                    DataIpsPrevired.Tipo = "4 - IPS";
                    DataIpsPrevired.Suspesion = false;
                    DataMutual = new ReportePrevired();
                    DataMutual.Tipo = "5 - MUTUAL";
                    DataMutual.Suspesion = false;
                    DataCaja = new ReportePrevired();
                    DataCaja.Tipo = "6 - CAJA COMPENSACION";
                    DataCaja.Suspesion = false;
                  

                    //Entidad para reporte
                    DataPreviredAfp.CentroCosto = persona.centro;
                    DataPreviredAfp.Sucursal = persona.sucursal;
                    DataPreviredAfp.Cargo = persona.Cargo;
                    DataPreviredAfp.Area = persona.NombreArea;
                    DataPreviredAfp.Entidad = persona.NombreAfp;

                    DataFonasaPrevired.CentroCosto = persona.centro;
                    DataFonasaPrevired.Sucursal = persona.sucursal;
                    DataFonasaPrevired.Cargo = persona.Cargo;
                    DataFonasaPrevired.Area = persona.NombreArea;
                    DataFonasaPrevired.Entidad = persona.codSalud == 1 ? persona.NombreSalud : "";

                    DataIsaprePrevired.CentroCosto = persona.centro;
                    DataIsaprePrevired.Sucursal = persona.sucursal;
                    DataIsaprePrevired.Cargo = persona.Cargo;
                    DataIsaprePrevired.Area = persona.NombreArea;
                    DataIsaprePrevired.Entidad = persona.codSalud > 1 ? persona.NombreSalud : "";

                    DataIpsPrevired.CentroCosto = persona.centro;
                    DataIpsPrevired.Sucursal = persona.sucursal;
                    DataIpsPrevired.Cargo = persona.Cargo;
                    DataIpsPrevired.Area = persona.NombreArea;
                    DataIpsPrevired.EntidadFonasa = persona.NombreSalud;
                    DataIpsPrevired.Entidad = persona.NombreCajaPrev;

                    DataMutual.CentroCosto = persona.centro;
                    DataMutual.Sucursal = persona.sucursal;
                    DataMutual.Cargo = persona.Cargo;
                    DataMutual.Area = persona.NombreArea;
                    DataMutual.Entidad = "Mutual (* Incluye Ley Sanna)";

                    DataCaja.CentroCosto = persona.centro;
                    DataCaja.Sucursal = persona.sucursal;
                    DataCaja.Cargo = persona.Cargo;
                    DataCaja.Area = persona.NombreArea;
                    DataCaja.Entidad = "Caja de Compensación";
                    DataCaja.AsignacionFam = MontoTotalCargasFamiliares;
                    //DataCaja.Prestamo = Prestamos;
                    //DataCaja.SeguroVida = SeguroVidaCaja;
                    #endregion

                    //DATOS TRABAJADOR
                    if (DatosTrabajador(diasTrabajados, pTipoLinea, cargaNormal, cargaMaternal, cargaInvalido, montoCarga,
                         ListadoMovimientos, montoCargaRetroactiva, persona, daySus13.ToString(), "", "") == false)
                        return null;

                    //DATOS AFP
                    DatosAfp(AdultoMayor, topeAfp, imponible, persona, DataPreviredAfp, ListadoMovimientos, daySus13, 0, 0);

                    //DATOS AHORRO PREVISIONAL VOLUNTARIO
                    DatosAhorroVoluntarioIndividual(persona, DataPreviredAfp);

                    //DATOS AHORRO PREVISIONAL COLECTIVO
                    DatosAhorroVoluntarioColectivo();

                    //DATOS AFILIADO VOLUNTARIO
                    DatosAfiliadoVoluntario();

                    //DATOS IPS
                    DatosIps(codSalud, imponible, Fonasa, persona, MontoTotalCargasFamiliares.ToString(), RegPrevisional, DataFonasaPrevired, DataIpsPrevired, ListadoMovimientos);

                    //DATOS SALUD
                    DatosSalud(codSalud, imponible, topeSalud, pContrato, DataIsaprePrevired, persona);

                    //DATOS CAJA DE COMPENSACION
                    DatosCaja(imponible, topeAfp, codSalud, DataCaja, ListadoMovimientos, persona, I, MontoTotalCargasFamiliares, Prestamos, AhorroCaja, SeguroVidaCaja);

                    //DATOS MUTUALIDAD
                    DatosMutualidad(imponible, pContrato, pPeriodo, topeAfp, DataMutual, ListadoMovimientos, persona, I);

                    //DATOS ADMINISTRADOR SEGURO CENSATIA
                    DatosAdministradorSeguro(AdultoMayor, MenorEdad, imponible, pContrato, pPeriodo, persona, DataPreviredAfp);

                    //DATOS PAGADOR SUBSIDIO
                    DatosPagadorSubsidio(ListadoMovimientos, pContrato);

                    //DATOS EMPRESA
                    DatosEmpresa(pContrato, pPeriodo);

                    //Agrega a listado
                    if (DataPreviredAfp.Cotizacion > 0 || DataPreviredAfp.SegAfiliado > 0 || DataPreviredAfp.SegInv > 0 || DataPreviredAfp.SegEmpresa > 0)
                        pDataPrevired.Add(DataPreviredAfp);
                    if (DataMutual.Cotizacion > 0)
                        pDataPrevired.Add(DataMutual);
                    if (DataCaja.Cotizacion > 0 || DataCaja.AsignacionFam > 0 || DataCaja.SeguroVida >0 || DataCaja.Prestamo >0 || DataCaja.Leasing > 0)
                        pDataPrevired.Add(DataCaja);
                    if (DataFonasaPrevired.Cotizacion > 0 || DataFonasaPrevired.CotizacionFonasa > 0)
                        pDataPrevired.Add(DataFonasaPrevired);
                    if (DataIsaprePrevired.Cotizacion > 0)
                        pDataPrevired.Add(DataIsaprePrevired);
                    if (DataIpsPrevired.Cotizacion > 0 || DataIpsPrevired.CotizacionFonasa > 0)
                        pDataPrevired.Add(DataIpsPrevired);

                    CadenasArchivo.Add(cadenaCarga);
                    cadenaCarga = "";
                }
                else
                {
                    //Hay suspensiones
                    //Primera linea debe contener la suma de los dos datos
                    string TipoLineaSus = "00";

                    DataSuspension = new Hashtable();
                    DataSuspension = DatosSuspension(persona, RegPrevisional, Fonasa, topeSalud, daysLic, topeAfp, I, codSalud);

                    if (DataSuspension == null)
                    {
                        XtraMessageBox.Show("No se pudo generar el archivo", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return null;
                    }

                    //Recorremos cada suspension y le hacemos el tratamiento correspondiente...
                    foreach (MovimientoPersonal mov in Suspensiones)
                    { 
                        count++;

                        #region "DATA REPORTE"
                        DataPreviredAfp = new ReportePrevired();
                        DataPreviredAfp.Tipo = "1 - AFP";
                        DataPreviredAfp.Suspesion = true;
                        DataFonasaPrevired = new ReportePrevired();
                        DataFonasaPrevired.Tipo = "2 - FONASA";
                        DataFonasaPrevired.Suspesion = true;
                        DataIsaprePrevired = new ReportePrevired();
                        DataIsaprePrevired.Tipo = "3 - ISAPRE";
                        DataIsaprePrevired.Suspesion = true;
                        DataIpsPrevired = new ReportePrevired();
                        DataIpsPrevired.Tipo = "4 - IPS";
                        DataIpsPrevired.Suspesion = true;
                        DataMutual = new ReportePrevired();
                        DataMutual.Tipo = "5 - MUTUAL";
                        DataMutual.Suspesion = true;
                        DataCaja = new ReportePrevired();
                        DataCaja.Tipo = "6 - CAJA COMPENSACION";
                        DataCaja.Suspesion = true;

                        //Entidad para reporte
                        DataPreviredAfp.CentroCosto = persona.centro;
                        DataPreviredAfp.Sucursal = persona.sucursal;
                        DataPreviredAfp.Cargo = persona.Cargo;
                        DataPreviredAfp.Area = persona.NombreArea;
                        DataPreviredAfp.Entidad = persona.NombreAfp + " *Suspension";

                        DataFonasaPrevired.CentroCosto = persona.centro;
                        DataFonasaPrevired.Sucursal = persona.sucursal;
                        DataFonasaPrevired.Cargo = persona.Cargo;
                        DataFonasaPrevired.Area = persona.NombreArea;
                        DataFonasaPrevired.Entidad = persona.codSalud == 1 ? (persona.NombreSalud + " *Suspension") : "";

                        DataIsaprePrevired.CentroCosto = persona.centro;
                        DataIsaprePrevired.Sucursal = persona.sucursal;
                        DataIsaprePrevired.Cargo = persona.Cargo;
                        DataIsaprePrevired.Area = persona.NombreArea;
                        DataIsaprePrevired.Entidad = persona.codSalud > 1 ? (persona.NombreSalud + " *Suspension") : "";

                        DataIpsPrevired.CentroCosto = persona.centro;
                        DataIpsPrevired.Sucursal = persona.sucursal;
                        DataIpsPrevired.Cargo = persona.Cargo;
                        DataIpsPrevired.Area = persona.NombreArea;
                        DataIpsPrevired.EntidadFonasa = persona.NombreSalud;
                        DataIpsPrevired.Entidad = persona.NombreCajaPrev + " *Suspension";

                        DataMutual.CentroCosto = persona.centro;
                        DataMutual.Sucursal = persona.sucursal;
                        DataMutual.Cargo = persona.Cargo;
                        DataMutual.Area = persona.NombreArea;
                        DataMutual.Entidad = "Ley Sanna * Suspension";

                        DataCaja.CentroCosto = persona.centro;
                        DataCaja.Sucursal = persona.sucursal;
                        DataCaja.Cargo = persona.Cargo;
                        DataCaja.Area = persona.NombreArea;
                        DataCaja.Entidad = "Caja * Suspension";

                        #endregion

                        #region "DATA REPORTE NORMAL"
                        DataPreviredAfpNormal = new ReportePrevired();
                        DataPreviredAfpNormal.Tipo = "1 - AFP";
                        DataPreviredAfpNormal.Suspesion = false;
                        DataFonasaPreviredNormal = new ReportePrevired();
                        DataFonasaPreviredNormal.Tipo = "2 - FONASA";
                        DataFonasaPreviredNormal.Suspesion = false;
                        DataIsaprePreviredNormal = new ReportePrevired();
                        DataIsaprePreviredNormal.Tipo = "3 - ISAPRE";
                        DataIsaprePreviredNormal.Suspesion = false;
                        DataIpsPreviredNormal = new ReportePrevired();
                        DataIpsPreviredNormal.Tipo = "4 - IPS";
                        DataIpsPreviredNormal.Suspesion = false;
                        DataMutualNormal = new ReportePrevired();
                        DataMutualNormal.Tipo = "5 - MUTUAL";
                        DataMutualNormal.Suspesion = false;
                        DataCajaNormal = new ReportePrevired();
                        DataCajaNormal.Tipo = "6 - CAJA COMPENSACION";
                        DataCajaNormal.Suspesion = false;

                        //Entidad para reporte
                        DataPreviredAfpNormal.CentroCosto = persona.centro;
                        DataPreviredAfpNormal.Sucursal = persona.sucursal;
                        DataPreviredAfpNormal.Cargo = persona.Cargo;
                        DataPreviredAfpNormal.Area = persona.NombreArea;
                        DataPreviredAfpNormal.Entidad = persona.NombreAfp;

                        DataFonasaPreviredNormal.CentroCosto = persona.centro;
                        DataFonasaPreviredNormal.Sucursal = persona.sucursal;
                        DataFonasaPreviredNormal.Cargo = persona.Cargo;
                        DataFonasaPreviredNormal.Area = persona.NombreArea;
                        DataFonasaPreviredNormal.Entidad = persona.codSalud == 1 ? persona.NombreSalud : "";

                        DataIsaprePreviredNormal.CentroCosto = persona.centro;
                        DataIsaprePreviredNormal.Sucursal = persona.sucursal;
                        DataIsaprePreviredNormal.Cargo = persona.Cargo;
                        DataIsaprePreviredNormal.Area = persona.NombreArea;
                        DataIsaprePreviredNormal.Entidad = persona.codSalud > 1 ? persona.NombreSalud : "";

                        DataIpsPreviredNormal.CentroCosto = persona.centro;
                        DataIpsPreviredNormal.Sucursal = persona.sucursal;
                        DataIpsPreviredNormal.Cargo = persona.Cargo;
                        DataIpsPreviredNormal.Area = persona.NombreArea;
                        DataIpsPreviredNormal.EntidadFonasa = persona.NombreSalud;
                        DataIpsPreviredNormal.Entidad = persona.NombreCajaPrev;

                        DataMutualNormal.CentroCosto = persona.centro;
                        DataMutualNormal.Sucursal = persona.sucursal;
                        DataMutualNormal.Cargo = persona.Cargo;
                        DataMutualNormal.Area = persona.NombreArea;
                        DataMutualNormal.Entidad = "Mutual (* Incluye Ley Sanna)";

                        DataCajaNormal.CentroCosto = persona.centro;
                        DataCajaNormal.Sucursal = persona.sucursal;
                        DataCajaNormal.Cargo = persona.Cargo;
                        DataCajaNormal.Area = persona.NombreArea;
                        DataCajaNormal.Entidad = "Caja de Compensación";
                        if(count == 1)
                            DataCajaNormal.AsignacionFam = MontoTotalCargasFamiliares;
                        if (count == 1)
                            DataCajaNormal.Prestamo = Prestamos;
                        if(count == 1)
                            DataCajaNormal.SeguroVida = SeguroVidaCaja;
                        if (count == 1)
                            DataCajaNormal.Leasing = AhorroCaja;
                        #endregion

                        double FonasaSus = 0;
                        if (mov.codMovimiento == "13")
                            FonasaSus = Labour.Calculo.GetValueFromCalculoMensaul(persona.Contrato, persona.PeriodoPersona, "syssaludsp13");
                        if (mov.codMovimiento == "14")
                            FonasaSus = Labour.Calculo.GetValueFromCalculoMensaul(persona.Contrato, persona.PeriodoPersona, "syssaludsp14");
                        if (mov.codMovimiento == "15")
                            FonasaSus = Labour.Calculo.GetValueFromCalculoMensaul(persona.Contrato, persona.PeriodoPersona, "syssaludsp15");

                        DatosTrabajadorSuspension(diasTrabajados, pTipoLinea, cargaNormal, cargaMaternal, cargaInvalido, montoCarga,
                        mov, montoCargaRetroactiva, persona, daySus13.ToString(), "", "", TipoLineaSus, count == 1? true: false);

                        //DATOS AFP
                        DatosAfpSuspension(AdultoMayor, topeAfp, imponible, persona, DataPreviredAfp, DataPreviredAfpNormal, mov, daySus13, daySus14, 0, DataSuspension);

                        //DATOS AHORRO PREVISIONAL VOLUNTARIO
                        DatosAhorroVoluntarioIndividualSuspension(persona, DataPreviredAfp, DataSuspension);

                        //DATOS AHORRO PREVISIONAL COLECTIVO
                        DatosAhorroVoluntarioColectivoSuspension();

                        //DATOS AFILIADO VOLUNTARIO
                        DatosAfiliadoVoluntarioSuspension();

                        //DATOS IPS
                        DatosIpsSuspension(codSalud, imponible, FonasaSus, Fonasa,
                            persona, MontoTotalCargasFamiliares.ToString(), RegPrevisional, DataFonasaPrevired,DataFonasaPreviredNormal, DataIpsPrevired, DataIpsPreviredNormal, mov, daySus13, daySus14, 0, DataSuspension);

                        //DATOS SALUD
                        DatosSaludSuspension(codSalud, imponible, topeSalud, DataIsaprePrevired, DataIsaprePreviredNormal, persona, mov, daySus13, daySus14, 0, DataSuspension);

                        //DATOS CAJA DE COMPENSACION
                        DatosCajaSuspension(imponible, topeAfp, codSalud, DataCaja, DataCajaNormal, mov, persona, daySus13, daySus14, 0, I, DataSuspension, count == 1?MontoTotalCargasFamiliares:0,  count == 1?Prestamos:0, count == 1?AhorroCaja:0, count == 1?SeguroVidaCaja:0);

                        //DATOS MUTULIDAD
                        DatosMutualidadSuspension(imponible, pContrato, pPeriodo, topeAfp,DataMutualNormal, DataMutual, mov, persona, daySus13, 0, 0, I, DataSuspension);

                        //DATOS ADMINISTRADOR SEGURO CENSATIA
                        DatosAdministradorSeguroSuspension(AdultoMayor, MenorEdad, imponible, pContrato, pPeriodo, persona, DataPreviredAfp, DataPreviredAfpNormal, daySus13, 0, 0, mov, DataSuspension);

                        //DATOS PAGADOR SUBSIDIO
                        DatosPagadorSubsidio(ListadoMovimientos, pContrato);

                        //Agrega a listado
                        if (DataPreviredAfp.Cotizacion > 0 || DataPreviredAfp.SegAfiliado > 0 || DataPreviredAfp.SegInv > 0 || DataPreviredAfp.SegEmpresa > 0)
                            pDataPrevired.Add(DataPreviredAfp);
                        if (DataMutual.Cotizacion > 0)
                            pDataPrevired.Add(DataMutual);
                        if (DataCaja.Cotizacion > 0 || DataCaja.AsignacionFam > 0)
                            pDataPrevired.Add(DataCaja);
                        if (DataFonasaPrevired.Cotizacion > 0 || DataFonasaPrevired.CotizacionFonasa > 0)
                            pDataPrevired.Add(DataFonasaPrevired);
                        if (DataIsaprePrevired.Cotizacion > 0)
                            pDataPrevired.Add(DataIsaprePrevired);
                        if (DataIpsPrevired.Cotizacion > 0 || DataIpsPrevired.CotizacionFonasa > 0)
                            pDataPrevired.Add(DataIpsPrevired);

                        //Normal
                        if (DataPreviredAfpNormal.Cotizacion > 0 || DataPreviredAfpNormal.SegAfiliado > 0 || DataPreviredAfp.SegInv > 0 || DataPreviredAfp.SegEmpresa > 0)
                            pDataPrevired.Add(DataPreviredAfpNormal);
                        if (DataMutualNormal.Cotizacion > 0)
                            pDataPrevired.Add(DataMutualNormal);
                        if (DataCajaNormal.Cotizacion > 0 || DataCajaNormal.AsignacionFam > 0 || DataCajaNormal.SeguroVida >0 || DataCajaNormal.Prestamo > 0 || DataCajaNormal.Leasing > 0)
                            pDataPrevired.Add(DataCajaNormal);
                        if (DataFonasaPreviredNormal.Cotizacion > 0 || DataFonasaPreviredNormal.CotizacionFonasa > 0)
                            pDataPrevired.Add(DataFonasaPreviredNormal);
                        if (DataIsaprePreviredNormal.Cotizacion > 0)
                            pDataPrevired.Add(DataIsaprePreviredNormal);
                        if (DataIpsPreviredNormal.Cotizacion > 0 || DataIpsPreviredNormal.CotizacionFonasa > 0)
                            pDataPrevired.Add(DataIpsPreviredNormal);


                        CadenasArchivo.Add(cadenaCarga);
                        cadenaCarga = "";

                    }
                }

            }

            return CadenasArchivo;
        }

        //CALCULO ADICIONAL PARA AGREGAR OTRO MOVIMIENTO DE PERSONAL SI ES EL CASO
        /// <summary>
        /// Permite agregar una nueva linea al archivo si es que hay un tipo linea adicional.
        /// </summary>
        /// <param name="InfoContrato"></param>
        /// <param name="ListadoMovimientos"></param>
        /// <param name="pContrato"></param>
        /// <returns></returns>
        private List<string> CalculoAdicional(List<MovimientoPersonal> ListadoMovimientos, string pContrato, int pPeriodo, bool? Suspension = false)
        {
            //CADA CADENA REPRESENTA UN NUEVO MOVIMIENTO DE PERSONAL
            List<string> cadenas = new List<string>();

            Persona persona = Persona.GetInfo(pContrato, pPeriodo);

            if (Suspension == true)
            {
                if (ListadoMovimientos.Count > 0 && persona != null)
                {
                    for (int i = 0; i < ListadoMovimientos.Count; i++)
                    {
                        //01 - RUT TRABAJADOR
                        AgregaCampo(fnSistema.GetRutsindv(Rut));
                        //02 - DV trabajador
                        AgregaCampo(fnSistema.Getdigito(Rut));
                        //03 - APELLIDO PATERNO
                        AgregaCampo(persona.Apepaterno);
                        //04 - APELLIDO MATERNO
                        AgregaCampo(persona.Apematerno);
                        //05 - NOMBRES
                        AgregaCampo(persona.Nombre);
                        //06 - SEXO           
                        AgregaCampo(GetCodigoSexo(persona.Sexo));
                        //07 - NACIONALIDAD
                        AgregaCampo("0");
                        //08 - TIPO PAGO
                        AgregaCampo("01");
                        //09 - PERIODO(DESDE)
                        AgregaCampo(fnSistema.PeriodoInvertido(pPeriodo));
                        //10 - PERIODO(HASTA)
                        AgregaCampo(fnSistema.PeriodoInvertidoUltimo(pPeriodo));
                        //11 - REGIMEN PREVISIONAL
                        AgregaCampo(GetRegimenPrevision(persona.Regimen, persona.Nacimiento));
                        //12 - TIPO TRABAJADOR
                        AgregaCampo("0");
                        //13 - DIAS TRABAJADOS
                        AgregaCampo("0");
                        //14 - TIPO DE LINEA --> LINEA ADICIONAL SI ES MOVIMIENTO DE PERSONAL
                        AgregaCampo("01");
                        //15 - CODIGO MOVIMIENTO PERSONAL
                        AgregaCampo(ListadoMovimientos[i].codMovimiento);
                        //16 - FECHA DESDE MOVIMIENTO
                        AgregaCampo(ListadoMovimientos[i].inicioMovimiento.ToShortDateString());
                        //17 - FECHA HASTA MOVIMIENTO
                        AgregaCampo(ListadoMovimientos[i].terminoMovimiento.ToShortDateString());
                        //18 - TRAMO ASIGNACION FAMILIAR
                        AgregaCampo("");
                        //19 - N° CARGAS SIMPLES
                        AgregaCampo("0");
                        //20 - N° CARGAS MATERNALES
                        AgregaCampo("0");
                        //21 - N° CARGAS INVALIDAS
                        AgregaCampo("0");
                        //22 - ASIGNACION FAMILIAR
                        AgregaCampo("0");
                        //23 - ASIGNACION FAMILIAR RETROACTIVA
                        AgregaCampo("0");
                        //24 - REINTEGRO CARGAS FAMILIARES
                        AgregaCampo("0");
                        //25 - SOLICITUD TRABAJADOR JOVEN
                        AgregaCampo("");
                        //26 - CODIGO DE LA AFP
                        AgregaCampo("0");
                        //27 - RENTA IMPONIBLE AFP
                        AgregaCampo("0");
                        //28 - COTIZACION OBLIGATORIA AFP
                        AgregaCampo("0");
                        //29 - COTIZACION SEGURO DE INVALIDES
                        AgregaCampo("0");
                        //30 - CUENTA DE AHORRO VOLUNTARIO AFP
                        AgregaCampo("0");
                        //31 - RENTA IMPONIBLE SUST AFP
                        AgregaCampo("0");
                        //32 - TASA PACTADA (SUST)
                        AgregaCampo("0");
                        //33 - APORTE INDEM
                        AgregaCampo("0");
                        //34 - N° PERIODOS (SUSTI)
                        AgregaCampo("0");
                        //35 - PERIODO DESDE
                        AgregaCampo("");
                        //36 - PERIODO HASTA
                        AgregaCampo("");
                        //37 - PUESTO DE TRABAJO PESADO
                        AgregaCampo("");
                        //38 - % COTIZACION TRABAJO PESADO
                        AgregaCampo("0");
                        //39 - COTIZACION TRABAJO PESADO
                        AgregaCampo("0");
                        //40 - CODIGO DE LA INSTITUCION APVI
                        AgregaCampo("0");
                        //41 - NUMERO DE ONTRATO APVI
                        AgregaCampo("");
                        //42 - FORMA DE PAGO APVI
                        AgregaCampo("0");
                        //43 - COTIZACION APVI
                        AgregaCampo("0");
                        //44 - COTIZACION DEPOSITOS CONVENIDOS
                        AgregaCampo("0");
                        //45 - CODIGO INSTITUCION AUTORIZADA APVC
                        AgregaCampo("0");
                        //46 - NUMERO DE CONTRATO APVC
                        AgregaCampo("");
                        //47 - FORMA DE PAGO APV
                        AgregaCampo("0");
                        //48 - COTIZACION TRABAJADOR APVC
                        AgregaCampo("0");
                        //49 - COTIZACION EMPLEADOR APVC
                        AgregaCampo("0");
                        //50 - RUT AFILIADO VOLUNTARIO
                        AgregaCampo("0");
                        //51 - DV AFILIADO VOLUNTARIO
                        AgregaCampo("");
                        //52 - APELLIDO PATERNO
                        AgregaCampo("");
                        //53 - APELLIDO MATERNO
                        AgregaCampo("");
                        //54 - NOMBRES
                        AgregaCampo("");
                        //55 - CODIGO MOVIMIENTO DE PERSONAL
                        AgregaCampo("0");
                        //56 - FECHA DESDE 
                        AgregaCampo("");
                        //57 - FECHA HASTA
                        AgregaCampo("");
                        //58 - CODIGO DE LA AFP
                        AgregaCampo("0");
                        //59 - MONTO CAPITALCION VOLUNTARIA
                        AgregaCampo("0");
                        //60 - MONTO AHORRO VOLUNTARIO
                        AgregaCampo("0");
                        //61 - NUMERO DE PERIODOS DE COTIZACION
                        AgregaCampo("0");
                        //62 - CODIGO EX-CAJA REGIMEN
                        AgregaCampo("0");
                        //63 - TASA COTIZACION EX-CAJA PREVISION
                        AgregaCampo("0");
                        //64 - RENTA IMPOINIBLES IPS
                        AgregaCampo("0");
                        //65 - COTIZACION OBLIGATORIA IPS
                        AgregaCampo("0");
                        //66 - RENTA IMPONIBLE DESAHUCIO
                        AgregaCampo("0");
                        //67 - CODIGO EX-CAJA REGIMEN DESAHUCIO
                        AgregaCampo("0");
                        //68 - TASA COTIZACION DESAHUCIO EX CAJAS DE PREVISION
                        AgregaCampo("0");
                        //69 - COTIZACION DESAHUCIO
                        AgregaCampo("0");
                        //70 - COTIZACION ACC. TRABAJO (ISL)
                        AgregaCampo("0");
                        //71 - BONIFICACION LEY 15386
                        AgregaCampo("0");
                        //72 - DESCUENTO POR CARGAS FAMILIARES DE ISL
                        AgregaCampo("0");
                        //73 - DESCUENTO POR CARGAS FAMILIARES DE ISL
                        AgregaCampo("0");
                        //74 - BONOS GOBIERNO
                        AgregaCampo("0");
                        //75 - CODIGO INSTITUCION DE SALUD
                        AgregaCampo("0");
                        //76 - NUMERO DEL FUN
                        AgregaCampo("");
                        //77 - RENTA IMPONIBLE ISAPRE
                        AgregaCampo("0");
                        //78 - MONEDA DEL PLAN PACTADO
                        AgregaCampo("0");
                        //79 - COTIZACION PACTADA
                        AgregaCampo("0");
                        //80 - COTIZACION OBLIGATORIA ISAPRE
                        AgregaCampo("0");
                        //81 - COTIZACION ADICIONAL VOLUNTARIA
                        AgregaCampo("0");
                        //82 - MONTO GARANTIA EXPLICITA DE SALUD
                        AgregaCampo("0");
                        //83 - CODIGO CCAF
                        AgregaCampo("0");
                        //84 - RENTA IMPONIBLE CCAF
                        AgregaCampo("0");
                        //85 - CREDITOS PERSONAL CCAF
                        AgregaCampo("0");
                        //86 - DESCUENTO DENTAL CCAF
                        AgregaCampo("0");
                        //87 - DESCUENTOS POR LEASING (PROGRAMA AHORRO)
                        AgregaCampo("0");
                        //88 - DESCUENTOS POR SEGURO DE VIDA CCAF
                        AgregaCampo("0");
                        //89 - OTROS DESCUENTOS CCAF
                        AgregaCampo("0");
                        //90 - COTIZACION A CCAF DE NO AFILIADOS A ISAPRES
                        AgregaCampo("0");
                        //91 - DESCUENTO CARGAS FAMILIARES CCAF
                        AgregaCampo("0");
                        //92 - OTROS DESCUENTOS CCAF 1 (USO FUTURO)
                        AgregaCampo("0");
                        //93 - OTRO0S DESCUENTOS CCAF 2 (USO FUTURO)
                        AgregaCampo("0");
                        //94 - BONOS GOBIERNO (USO FUTURO)
                        AgregaCampo("0");
                        //95 - CODIGO DE SUSCURAL (USO FUTURO)
                        AgregaCampo("");
                        //96 - CODIGO MUTUALIDAD
                        AgregaCampo("0");
                        //97 - RENTA IMPONIBLE MUTUAL
                        AgregaCampo("0");
                        //98 - COTIZACION ACCIDENTE DEL TRABAJO
                        AgregaCampo("0");
                        //99 - SUCURSAL PARA PAGO MUTUAL
                        AgregaCampo("0");
                        //100 - RENTA IMPONIBLE SEGURO CESANTIA 
                        AgregaCampo("0");
                        //101 - APORTE TRABAJADOR SEGURO CESANTIA
                        AgregaCampo("0");
                        //102 - APORTE EMPLEADOR SEGURO CESANTIA
                        AgregaCampo("0");
                        //103 - RUT PAGADORA SUBSIDIO
                        if (ListadoMovimientos[i].codMovimiento == "3")
                            AgregaCampo(fnSistema.GetRutsindv(RutPagadoraSubsidio(pPeriodo, pContrato)));
                        else
                            AgregaCampo("0");
                        //104 - DV PAGADIRA SUBSIDIO
                        if (ListadoMovimientos[i].codMovimiento == "3")
                            AgregaCampo(fnSistema.Getdigito(RutPagadoraSubsidio(pPeriodo, pContrato)));
                        else
                            AgregaCampo("");
                        //105 - CENTRO DE COSTOS, SUCURSAL, AGENCIA, OTRA, REGION
                        //AgregaCampo("");

                        //AGREGAMOS A LISTA
                        cadenas.Add(cadenaCarga);
                        //LIMPIAMOS CADENA CARGA
                        cadenaCarga = "";
                    }

                }
            }
            else
            {
                if (ListadoMovimientos.Count > 1 && persona != null)
                {
                    for (int i = 1; i < ListadoMovimientos.Count; i++)
                    {
                        //01 - RUT TRABAJADOR
                        AgregaCampo(fnSistema.GetRutsindv(Rut));
                        //02 - DV trabajador
                        AgregaCampo(fnSistema.Getdigito(Rut));
                        //03 - APELLIDO PATERNO
                        AgregaCampo(persona.Apepaterno);
                        //04 - APELLIDO MATERNO
                        AgregaCampo(persona.Apematerno);
                        //05 - NOMBRES
                        AgregaCampo(persona.Nombre);
                        //06 - SEXO           
                        AgregaCampo(GetCodigoSexo(persona.Sexo));
                        //07 - NACIONALIDAD
                        AgregaCampo("0");
                        //08 - TIPO PAGO
                        AgregaCampo("01");
                        //09 - PERIODO(DESDE)
                        AgregaCampo(fnSistema.PeriodoInvertido(pPeriodo));
                        //10 - PERIODO(HASTA)
                        AgregaCampo(fnSistema.PeriodoInvertidoUltimo(pPeriodo));
                        //11 - REGIMEN PREVISIONAL
                        AgregaCampo(GetRegimenPrevision(persona.Regimen, persona.Nacimiento));
                        //12 - TIPO TRABAJADOR
                        AgregaCampo("0");
                        //13 - DIAS TRABAJADOS
                        AgregaCampo("0");
                        //14 - TIPO DE LINEA --> LINEA ADICIONAL SI ES MOVIMIENTO DE PERSONAL
                        AgregaCampo("01");
                        //15 - CODIGO MOVIMIENTO PERSONAL
                        AgregaCampo(ListadoMovimientos[i].codMovimiento);
                        //16 - FECHA DESDE MOVIMIENTO
                        AgregaCampo(ListadoMovimientos[i].inicioMovimiento.ToShortDateString());
                        //17 - FECHA HASTA MOVIMIENTO
                        AgregaCampo(ListadoMovimientos[i].terminoMovimiento.ToShortDateString());
                        //18 - TRAMO ASIGNACION FAMILIAR
                        AgregaCampo("");
                        //19 - N° CARGAS SIMPLES
                        AgregaCampo("0");
                        //20 - N° CARGAS MATERNALES
                        AgregaCampo("0");
                        //21 - N° CARGAS INVALIDAS
                        AgregaCampo("0");
                        //22 - ASIGNACION FAMILIAR
                        AgregaCampo("0");
                        //23 - ASIGNACION FAMILIAR RETROACTIVA
                        AgregaCampo("0");
                        //24 - REINTEGRO CARGAS FAMILIARES
                        AgregaCampo("0");
                        //25 - SOLICITUD TRABAJADOR JOVEN
                        AgregaCampo("");
                        //26 - CODIGO DE LA AFP
                        AgregaCampo("0");
                        //27 - RENTA IMPONIBLE AFP
                        AgregaCampo("0");
                        //28 - COTIZACION OBLIGATORIA AFP
                        AgregaCampo("0");
                        //29 - COTIZACION SEGURO DE INVALIDES
                        AgregaCampo("0");
                        //30 - CUENTA DE AHORRO VOLUNTARIO AFP
                        AgregaCampo("0");
                        //31 - RENTA IMPONIBLE SUST AFP
                        AgregaCampo("0");
                        //32 - TASA PACTADA (SUST)
                        AgregaCampo("0");
                        //33 - APORTE INDEM
                        AgregaCampo("0");
                        //34 - N° PERIODOS (SUSTI)
                        AgregaCampo("0");
                        //35 - PERIODO DESDE
                        AgregaCampo("");
                        //36 - PERIODO HASTA
                        AgregaCampo("");
                        //37 - PUESTO DE TRABAJO PESADO
                        AgregaCampo("");
                        //38 - % COTIZACION TRABAJO PESADO
                        AgregaCampo("0");
                        //39 - COTIZACION TRABAJO PESADO
                        AgregaCampo("0");
                        //40 - CODIGO DE LA INSTITUCION APVI
                        AgregaCampo("0");
                        //41 - NUMERO DE ONTRATO APVI
                        AgregaCampo("");
                        //42 - FORMA DE PAGO APVI
                        AgregaCampo("0");
                        //43 - COTIZACION APVI
                        AgregaCampo("0");
                        //44 - COTIZACION DEPOSITOS CONVENIDOS
                        AgregaCampo("0");
                        //45 - CODIGO INSTITUCION AUTORIZADA APVC
                        AgregaCampo("0");
                        //46 - NUMERO DE CONTRATO APVC
                        AgregaCampo("");
                        //47 - FORMA DE PAGO APV
                        AgregaCampo("0");
                        //48 - COTIZACION TRABAJADOR APVC
                        AgregaCampo("0");
                        //49 - COTIZACION EMPLEADOR APVC
                        AgregaCampo("0");
                        //50 - RUT AFILIADO VOLUNTARIO
                        AgregaCampo("0");
                        //51 - DV AFILIADO VOLUNTARIO
                        AgregaCampo("");
                        //52 - APELLIDO PATERNO
                        AgregaCampo("");
                        //53 - APELLIDO MATERNO
                        AgregaCampo("");
                        //54 - NOMBRES
                        AgregaCampo("");
                        //55 - CODIGO MOVIMIENTO DE PERSONAL
                        AgregaCampo("0");
                        //56 - FECHA DESDE 
                        AgregaCampo("");
                        //57 - FECHA HASTA
                        AgregaCampo("");
                        //58 - CODIGO DE LA AFP
                        AgregaCampo("0");
                        //59 - MONTO CAPITALCION VOLUNTARIA
                        AgregaCampo("0");
                        //60 - MONTO AHORRO VOLUNTARIO
                        AgregaCampo("0");
                        //61 - NUMERO DE PERIODOS DE COTIZACION
                        AgregaCampo("0");
                        //62 - CODIGO EX-CAJA REGIMEN
                        AgregaCampo("0");
                        //63 - TASA COTIZACION EX-CAJA PREVISION
                        AgregaCampo("0");
                        //64 - RENTA IMPOINIBLES IPS
                        AgregaCampo("0");
                        //65 - COTIZACION OBLIGATORIA IPS
                        AgregaCampo("0");
                        //66 - RENTA IMPONIBLE DESAHUCIO
                        AgregaCampo("0");
                        //67 - CODIGO EX-CAJA REGIMEN DESAHUCIO
                        AgregaCampo("0");
                        //68 - TASA COTIZACION DESAHUCIO EX CAJAS DE PREVISION
                        AgregaCampo("0");
                        //69 - COTIZACION DESAHUCIO
                        AgregaCampo("0");
                        //70 - COTIZACION ACC. TRABAJO (ISL)
                        AgregaCampo("0");
                        //71 - BONIFICACION LEY 15386
                        AgregaCampo("0");
                        //72 - DESCUENTO POR CARGAS FAMILIARES DE ISL
                        AgregaCampo("0");
                        //73 - DESCUENTO POR CARGAS FAMILIARES DE ISL
                        AgregaCampo("0");
                        //74 - BONOS GOBIERNO
                        AgregaCampo("0");
                        //75 - CODIGO INSTITUCION DE SALUD
                        AgregaCampo("0");
                        //76 - NUMERO DEL FUN
                        AgregaCampo("");
                        //77 - RENTA IMPONIBLE ISAPRE
                        AgregaCampo("0");
                        //78 - MONEDA DEL PLAN PACTADO
                        AgregaCampo("0");
                        //79 - COTIZACION PACTADA
                        AgregaCampo("0");
                        //80 - COTIZACION OBLIGATORIA ISAPRE
                        AgregaCampo("0");
                        //81 - COTIZACION ADICIONAL VOLUNTARIA
                        AgregaCampo("0");
                        //82 - MONTO GARANTIA EXPLICITA DE SALUD
                        AgregaCampo("0");
                        //83 - CODIGO CCAF
                        AgregaCampo("0");
                        //84 - RENTA IMPONIBLE CCAF
                        AgregaCampo("0");
                        //85 - CREDITOS PERSONAL CCAF
                        AgregaCampo("0");
                        //86 - DESCUENTO DENTAL CCAF
                        AgregaCampo("0");
                        //87 - DESCUENTOS POR LEASING (PROGRAMA AHORRO)
                        AgregaCampo("0");
                        //88 - DESCUENTOS POR SEGURO DE VIDA CCAF
                        AgregaCampo("0");
                        //89 - OTROS DESCUENTOS CCAF
                        AgregaCampo("0");
                        //90 - COTIZACION A CCAF DE NO AFILIADOS A ISAPRES
                        AgregaCampo("0");
                        //91 - DESCUENTO CARGAS FAMILIARES CCAF
                        AgregaCampo("0");
                        //92 - OTROS DESCUENTOS CCAF 1 (USO FUTURO)
                        AgregaCampo("0");
                        //93 - OTRO0S DESCUENTOS CCAF 2 (USO FUTURO)
                        AgregaCampo("0");
                        //94 - BONOS GOBIERNO (USO FUTURO)
                        AgregaCampo("0");
                        //95 - CODIGO DE SUSCURAL (USO FUTURO)
                        AgregaCampo("");
                        //96 - CODIGO MUTUALIDAD
                        AgregaCampo("0");
                        //97 - RENTA IMPONIBLE MUTUAL
                        AgregaCampo("0");
                        //98 - COTIZACION ACCIDENTE DEL TRABAJO
                        AgregaCampo("0");
                        //99 - SUCURSAL PARA PAGO MUTUAL
                        AgregaCampo("0");
                        //100 - RENTA IMPONIBLE SEGURO CESANTIA 
                        AgregaCampo("0");
                        //101 - APORTE TRABAJADOR SEGURO CESANTIA
                        AgregaCampo("0");
                        //102 - APORTE EMPLEADOR SEGURO CESANTIA
                        AgregaCampo("0");
                        //103 - RUT PAGADORA SUBSIDIO
                        if (ListadoMovimientos[i].codMovimiento == "3")
                            AgregaCampo(fnSistema.GetRutsindv(RutPagadoraSubsidio(pPeriodo, pContrato)));
                        else
                            AgregaCampo("0");
                        //104 - DV PAGADIRA SUBSIDIO
                        if (ListadoMovimientos[i].codMovimiento == "3")
                            AgregaCampo(fnSistema.Getdigito(RutPagadoraSubsidio(pPeriodo, pContrato)));
                        else
                            AgregaCampo("");
                        //105 - CENTRO DE COSTOS, SUCURSAL, AGENCIA, OTRA, REGION
                        //AgregaCampo("");

                        //AGREGAMOS A LISTA
                        cadenas.Add(cadenaCarga);
                        //LIMPIAMOS CADENA CARGA
                        cadenaCarga = "";
                    }

                }
            }
           

            return cadenas;
        }

        //AHORRO VOLUNTARIO
        private string GetAhorroVoluntario(string pContrato, int pPeriodo)
        {
            string sql = "SELECT valorCalculado FROM itemtrabajador WHERE contrato = @pContrato " +
                "AND anomes=@pPeriodo AND coditem = 'AFPAHO'";
            string val = "";
            SqlCommand cmd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pContrato", pContrato));
                        cmd.Parameters.Add(new SqlParameter("@pPeriodo", pPeriodo));

                        val = (Convert.ToDouble(cmd.ExecuteScalar())) + "";
                    }
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return val;
        }


        #region "FUNCIONES"
        //***************************
        // DATOS DEL TRABAJADOR     |
        //***************************

        private bool DatosTrabajador(string diasTrabajados, string TipoLinea,
            string CargaNormal, string CargaMaternal, string CargaInvalido, string MontoCarga,
            List<MovimientoPersonal> listado, string MontoCargaRetroactivo, Persona person,
            string pDiasSus13, string pDiasSus14, string pDiasSus15)
        {

            //01-RUT SIN DIGITO
            //ENMASCARAR RUT
            AgregaCampo(fnSistema.GetRutsindv(Rut));
            //02-DIGITO VERIFICADOR
            AgregaCampo(fnSistema.Getdigito(Rut));
            //03-APELLIDO PATERNO
            if (person.Apepaterno.ToString().Length > 30)
            { XtraMessageBox.Show("El apellido paterno no puede contener mas de 30 caracteres", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return false; }
            else
                AgregaCampo(person.Apepaterno);
            //04-APELLIDO MATERNO
            if (person.Apematerno.ToString().Length > 30)
            { XtraMessageBox.Show("El apellido materno no puede contener mas de 30 caracteres", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return false; }
            else
                AgregaCampo(person.Apematerno);
            //05-NOMBRES
            if ((person.Nombre.ToString().Length > 30))
            { XtraMessageBox.Show("El nombre no puede contener mas de 30 caracteres", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return false; }
            else
                AgregaCampo(person.Nombre);
            //06-SEXO
            AgregaCampo(GetCodigoSexo(person.Sexo));
            //07-NACIONALIDAD
            AgregaCampo((GetCodigoNacionalidad(person.codNacion)).ToString());
            //08-TIPO DE PAGO
            AgregaCampo("01");

            //09-FECHA REMUNERACION DESDE       
            AgregaCampo(fnSistema.PeriodoInvertido(Periodo));

            //10-FECHA REMUNERACION HASTA       
            AgregaCampo(fnSistema.PeriodoInvertidoUltimo(Periodo));

            //11-REGIMEN PREVISIONAL
            AgregaCampo(GetRegimenPrevision(person.Regimen, person.Nacimiento));

            //12-TIPO TRABAJADOR
            AgregaCampo((GetTipoTrabajador(person.Jubilado, person.Nacimiento)).ToString());

            //13-DIAS TRABAJADOS           
            AgregaCampo(diasTrabajados);

            //14-TIPO DE LINEA
            AgregaCampo(TipoLinea);

            //15-CODIGO MOVIMIENTO PERSONAL
            //VEMOS SI TIENE LICENCIAS U OTRO MOVIMIENTO DE PERSONAL 

            if (listado.Count > 0)
            {
                AgregaCampo(listado[0].codMovimiento);
            }
            else
            {
                AgregaCampo("0");
            }

            //16-FECHA DESDE --> FECHA INICIO MOVIMIENTO PERSONAL (dd mm yyyy)
            if (listado.Count > 0)
            {
                if (listado[0].codMovimiento == "1" || listado[0].codMovimiento == "3" ||
                    listado[0].codMovimiento == "4" || listado[0].codMovimiento == "5" ||
                    listado[0].codMovimiento == "6" || listado[0].codMovimiento == "7" ||
                    listado[0].codMovimiento == "8" || listado[0].codMovimiento == "11"
                    )
                    AgregaCampo(listado[0].inicioMovimiento.ToShortDateString());
                else
                    AgregaCampo("");
            }
            else
            {
                AgregaCampo("");
            }


            //17-FECHA HASTA --> FECHA FIN MOVIMIENTO PERSONAL (DD MM YYYY)
            //Tambien en el caso de haya suspension laboral se debe agregar fecha
            if (listado.Count > 0)
            {
                if (listado[0].codMovimiento == "2" || listado[0].codMovimiento == "3" ||
                    listado[0].codMovimiento == "4" || listado[0].codMovimiento == "6" ||
                    listado[0].codMovimiento == "7" ||
                    listado[0].codMovimiento == "11")
                    AgregaCampo(listado[0].terminoMovimiento.ToShortDateString());
                else
                    AgregaCampo("");
            }
            else
            {
                AgregaCampo("");
            }


            //18-TRAMO ASIGNACION FAMILIAR 
            AgregaCampo(GetTramoAsignacionFamiliar(person.Tramo));

            //19-NUMERO CARGAS FAMILIARES SIMPLES
            if (Convert.ToInt32(CargaNormal) > 13)
            { XtraMessageBox.Show("Las cargas simples no pueden ser mayor a 13", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return false; }

            AgregaCampo(CargaNormal);

            //20-NUMERO CARGAS FAMILIARES MATERNALES
            if (Convert.ToInt32(CargaMaternal) > 1)
            { XtraMessageBox.Show("Las cargas maternales no pueden ser superiores a 1", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return false; }
            AgregaCampo(CargaMaternal);

            //21-NUMERO CARGAS FAMILIARES INVALIDAS
            if (Convert.ToInt32(CargaInvalido) > 1)
            { XtraMessageBox.Show("las cargas invalidas no pueden ser superio a 1", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return false; }
            AgregaCampo(CargaInvalido);

            //22-MONTO ASIGNACION EN PESOS CARGAS FAMILIARES
            AgregaCampo(MontoCarga);
            //23-ASIGNACION FAMILIAR RETROACTIVA
            AgregaCampo(MontoCargaRetroactivo);

            //24-REINTEGRO CARGAS FAMILIARES
            AgregaCampo("0");

            //25-SOLICITUD TRABAJADOR JOVEN
            AgregaCampo("N");

            return true;
        }

        //*************
        //  DATOS AFP |
        //*************
        private void DatosAfp(bool AdultoMayor, double TopeAfp, double imponible,
            Persona person, ReportePrevired pReportePrevired,
            List<MovimientoPersonal> pMovimientos, double pDiasSp13, double pDiasSp14, double pDiasSp15)
        {
            string codAfp = "";
            double afp = 0, comafp = 0, segInv = 0, afpaho = 0;
            codAfp = GetCodigoAfp(person.codAfp);
            //26-CODIGO AFP
            AgregaCampo(codAfp);

            //27-RENTA IMPONIBLE AFP (SIEMPRE Y CUANDO COTICE EN AFP)
            //En caso de suspension debe ser la renta imponible que utilizó en calculo para suspension
            //podemos usar el imponible de mes completo y calcular un proporcional 
            //en base a los dias de suspension del movimiento de personal
            /* 1 -> AFP - SALUD
             * 
             */
            if (codAfp != "SIP" && person.Regimen == 1)
            {
                if (imponible > TopeAfp)
                    imponible = TopeAfp;
                else if (imponible < TopeAfp)
                    imponible = Math.Round(imponible);
                else
                    imponible = Math.Round(imponible);

                //AGREGAMOS VARIABLE IMPONIBLE
                AgregaCampo(imponible.ToString());
            }
            else
                AgregaCampo("0");

            //28-COTIZACION OBLIGATORIA AFP
            //SUMAMOS COTIZACION INDIVIDUAL + COTIZACION ADMINSTRACION AFP
            if (codAfp != "SIP" && person.Regimen == 1)
            {
                afp = Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(person.Contrato, person.PeriodoPersona, "sysciafp"));
                comafp = Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(person.Contrato, person.PeriodoPersona, "syscomafp"));
                AgregaCampo((afp + comafp).ToString());
                pReportePrevired.Cotizacion = (afp + comafp);
                //AgregaCampo((Math.Round(varSistema.ObtenerValorLista("sysciafp") + varSistema.ObtenerValorLista("syscomafp"))).ToString());
            }
            else
                AgregaCampo("0");

            //29-COTIZACION SEGURO DE INVALIDEZ
            //if (AdultoMayor == false)
            segInv = Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(person.Contrato, person.PeriodoPersona, "syssis"));
            AgregaCampo(segInv.ToString());
            pReportePrevired.SegInv = segInv;
            //AgregaCampo(Math.Round(varSistema.ObtenerValorLista("syssis")).ToString());
            //else
            //AgregaCampo("0");

            //30-CUENTA AHORRO VOLUNTARIO
            //AgregaCampo((Math.Round(varSistema.ObtenerValorLista("sysaprevol"))).ToString());            
            //AgregaCampo(GetAhorroVoluntario((string)data["contrato"], Convert.ToInt32(data["periodo"])));
            afpaho = ItemTrabajador.GetValue("afpaho", person.Contrato, person.PeriodoPersona);
            AgregaCampo(afpaho + "");
            pReportePrevired.AhorroVoluntario = afpaho;

            //31-RENTA IMPONIBLE SUSTITUTIVA
            AgregaCampo("0");

            //32-TASA PACTADA PARA RENTA SUSTITUTIVA
            AgregaCampo("0,00");

            //33-MONTO APORTE INDEMNIZACION
            AgregaCampo("0");

            //34-N° PERIODOS DE APORTE SUSTITUTIVO CONVENIDO
            AgregaCampo("00");

            //35-DIA, MES, AÑO INICIO APORTE SUSTITUTIVO CONVENIDO
            AgregaCampo("0");

            //36-DIA, MES, AÑO TERMINO APORTE SUSTITUTIVO CONVENIDO
            AgregaCampo("0");

            //37-PUESTO DE TRABAJO PESADO
            AgregaCampo("");

            //38-% COTIZACION TRABAJO PESADO (2% o 4%)
            AgregaCampo("00,00");

            //39-COTIZACION TRABAJO PESADO
            AgregaCampo("0");
        }

        //***************************************
        //  DATOS AHORRO PREVISIONAL VOLUNTARIO |
        //***************************************
        private void DatosAhorroVoluntarioIndividual(Persona pPerson, ReportePrevired pReportePrevired)
        {
            double Aprevol = 0;
            Aprevol = ItemTrabajador.GetValue("aprevol", pPerson.Contrato, pPerson.PeriodoPersona);
            string codAfp = GetCodigoAfp(pPerson.codAfp);


            //40-CODIGO INSTITUCION APVI
            if (pPerson.codCajaPrev == 0)
            {
                if (Aprevol > 0)
                    AgregaCampo("0" + codAfp);
                else
                    AgregaCampo("000");
            }
            else
            {
                AgregaCampo("000");
            }


            //41-NUMERO CONTRATO APVI
            AgregaCampo("");

            //42-FORMA DE PAGO APVI (1:directo 2:indirecto)
            if (pPerson.codCajaPrev == 0)
            {
                if (Aprevol > 0)
                    AgregaCampo("2");
                else
                    AgregaCampo("0");
            }
            else
            {
                AgregaCampo("0");
            }

            //43-COTIZACION APVI
            //Aprevol = ItemTrabajador.GetValue("aprevol", pPerson.Contrato, pPerson.PeriodoPersona);
            if (pPerson.codCajaPrev == 0)
            {
                if (Aprevol > 0)
                    AgregaCampo(Aprevol + "");
                else
                    AgregaCampo("0");
                pReportePrevired.AhorroPrevisional = Aprevol;
            }
            else
            {
                AgregaCampo("0");
            }


            //44-COTIZACION DEPOSITOS CONVENIDOS
            AgregaCampo("0");
        }

        //***************************************************
        // DATOS AHORRO PREVISIONAL VOLUNTARIO COLECTIVO    |
        //***************************************************
        private void DatosAhorroVoluntarioColectivo()
        {
            //45-CODIGO INSTITUCION AUTORIZADA APVC
            AgregaCampo("000");
            //46-NUMERO DE CONTRATO APVC
            AgregaCampo("");
            //47-FORMA DE PAGO APVC
            AgregaCampo("0");
            //48-COTIZACION TRABAJADOR APVC
            AgregaCampo("0");
            //49-COTIZACION EMPLEADOR APVC
            AgregaCampo("0");
        }

        //*****************************
        //  DATOS AFILIADO VOLUNTARIO |
        //*****************************
        private void DatosAfiliadoVoluntario()
        {
            //50-RUT AFILIADO VOLUNTARIO
            AgregaCampo("0");
            //51-DV AFILIADO VOLUNTARIO
            AgregaCampo(" ");
            //52-APELLIDO PATERNO
            AgregaCampo("");
            //53-APELLIDO MATERNO
            AgregaCampo("");
            //54-NOMBRES
            AgregaCampo("");
            //55-CODIGO MOVIMIENTO DE PERSONAL
            AgregaCampo("0");
            //56-FECHA DESDE
            AgregaCampo("");
            //57-FECHA HASTA
            AgregaCampo("");
            //58-CODIGO DE LA AFP
            AgregaCampo("0");
            //59-MONTO CAPITALIZACION VOLUNTARIA --> debe ser mayor o igual al 10% del sueldo minimo
            //del trabajador dependiente
            AgregaCampo("0");
            //60-MONTO AHORRO VOLUNTARIO
            AgregaCampo("0");
            //61-NUMERO DE PERIODOS DE COTIZACION
            AgregaCampo("0");
        }

        //********************************
        // DATOS IPS-ISL-FONASA
        //******************************
        private void DatosIps(string codSalud, double Imponible, double Fonasa, Persona pPerson,
            string pMontoCargasTotal, string pRegimenPrevisional,
            ReportePrevired pReportePreviredFonasa, ReportePrevired pReportePreviredIps,
            List<MovimientoPersonal> pMovimientos)
        {
            double TotalFonasa = 0, ImponibleIps = 0, topeAfpAnterior = 0;
            //DATOS DE CAJAPREVISION
            CajaPrevision caja = new CajaPrevision();
            caja.SetInfo(pPerson.Contrato, pPerson.PeriodoPersona);

            IndiceMensual I = new IndiceMensual();
            I.SetInfoMes(fnSistema.fnObtenerPeriodoAnterior(Labour.Calculo.PeriodoObservado));
            topeAfpAnterior = Math.Round(I.TopeAfp * I.Uf);

            //REGIMEN 4 Y 5 SIN IPS
            //62-CODIGO EX-CAJA REGIMEN
            if (pPerson.Regimen == 4 || pPerson.Regimen == 5)
            {
                if (caja.id != 0)
                {
                    AgregaCampo(caja.dato01);
                }
                else
                    AgregaCampo("0000");
            }
            else
                AgregaCampo("0000");

            //63-TAZA COTIZACION EX-CAJA PREVISION
            if (pPerson.Regimen == 4 || pPerson.Regimen == 5)
            {
                if (caja.id != 0)
                {
                    AgregaCampo((caja.porcPension * 100).ToString());
                }
                else
                    AgregaCampo("0000");
            }
            else
                AgregaCampo("00,00");

            //64-RENTA IMPONIBLE IPS
            if (codSalud == "07")
            {

                ImponibleIps = GetMontoIps(periodo, Imponible, pRegimenPrevisional);
                if (ImponibleIps > I.TopeIpsPesos)
                    ImponibleIps = Math.Round(I.TopeIpsPesos);

                AgregaCampo(Math.Round(ImponibleIps) + "");

            }
            else
                AgregaCampo("0");

            //65-COTIZACION OBLIGATORIA IPS (100% suspension laboral)
            if (pPerson.Regimen == 4 || pPerson.Regimen == 5)
            {
                if (caja.id != 0)
                {
                    //Obtenemos el porcentaje desde el imponible.                    
                    AgregaCampo(Math.Round(caja.porcPension * ImponibleIps).ToString());
                    pReportePreviredIps.Cotizacion = Math.Round(caja.porcPension * ImponibleIps);
                }
                else
                    AgregaCampo("0");
            }
            else
                AgregaCampo("0");
            //66-RENTA IMPONIBLE DESAHUCIO
            AgregaCampo("0");
            //67-CODIGO EX-CAJA REGIMEN DESAHUCIO
            AgregaCampo("0");
            //68-TASA COTIZACION DESAHUCIO EX-CAJAS DE PREVISION
            AgregaCampo("00,00");
            //69-COTIZACION DESAHUCIO
            AgregaCampo("0");
            //70-COTIZACION FONASA       
            //EN CASO DE EXISTIR ASOCIACION A CAJA EL MONTO A PAGAR EA
            // SALUD -> 6.4%
            // CAJA -> 0.6
            //SI NO HAY CAJA ES 7%
            if (GetCodigoCaja() != "00")
            {
                if (codSalud == "07")
                {
                    //Es regimen antiguo?
                    if (pPerson.Regimen == 4 || pPerson.Regimen == 5)
                    {
                        //OBTENEMOS EL 6.4% DEL IMPONIBLE
                        AgregaCampo(Math.Round(ImponibleIps * 0.064).ToString());
                        pReportePreviredIps.CotizacionFonasa = Math.Round(ImponibleIps * 0.064);
                    }
                    else
                    {
                        //TotalFonasa = ((0.064) * (varSistema.ObtenerValorLista("sysfonasa")))/(0.07);      
                        if (Fonasa > (0.07 * ImponibleIps))
                            TotalFonasa = Math.Round(0.064 * (ImponibleIps));
                        else
                            TotalFonasa = Math.Round(((0.064) * Fonasa) / (0.07));

                        pReportePreviredFonasa.Cotizacion = TotalFonasa;
                        AgregaCampo(Math.Round(TotalFonasa).ToString());
                    }
                }
                else
                    AgregaCampo("0");
            }
            else
            {

                if (codSalud == "07")
                {
                    //ES REGIMEN ANTIGUO???
                    if (pPerson.Regimen == 4 || pPerson.Regimen == 5)
                    {
                        AgregaCampo(Math.Round(ImponibleIps * caja.porcSalud) + "");
                        pReportePreviredIps.CotizacionFonasa = Math.Round(ImponibleIps * caja.porcSalud);
                    }
                    else
                    {
                        TotalFonasa = Math.Round(Fonasa);
                        pReportePreviredFonasa.Cotizacion = TotalFonasa;
                        AgregaCampo(TotalFonasa.ToString());
                    }
                }
                else
                {
                    AgregaCampo("0");
                }
            }

            //71-COTIZACION ACC. TRABAJO (ISL)
            AgregaCampo("0");
            //72-BONIFICACION LEY 15.386
            AgregaCampo("0");
            //73-DESCUENTO POR CARGAS FAMILIARES DE ISL (SE DEBE INFORMAR SI NO ESTÁ EN CAJA DE COMPENSACION)
            if (GetCodigoCaja() == "00")
                AgregaCampo(pMontoCargasTotal);
            else
                AgregaCampo("0");
            //74-BONOS GOBIERNO
            AgregaCampo("0");
        }

        //**************************
        // DATOS SALUD 
        //**************************
        private void DatosSalud(string codSalud, double imponible, double TopeSalud, string pContrato, ReportePrevired pReportePreviredIsapre, Persona person)
        {
            double isapre = 0, imponibleFinal = 0, adicional = 0, salud = 0, parcial = 0;
            string val = "";
            //75-CODIGO INSTITUCION SALUD
            AgregaCampo(codSalud);
            //76-NUMERO DEL FUN
            if (codSalud != "00" && codSalud != "07")
                AgregaCampo(GetFun(pContrato, Periodo));
            else
                AgregaCampo("");
            //77-RENTA IMPONIBLE ISAPRE
            if (codSalud != "00" && codSalud != "07")
            {
                //COTIZACION FINAL DE ISAPRE
                //isapre = Math.Round(varSistema.ObtenerValorLista("sysisapre"));
                isapre = Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(pContrato, Periodo, "sysisapre"));

                if (imponible > TopeSalud)
                {
                    imponibleFinal = TopeSalud;
                    salud = Math.Round(0.07 * TopeSalud, 0, MidpointRounding.AwayFromZero);
                }
                else if (TopeSalud > imponible)
                {
                    imponibleFinal = imponible;
                    salud = Math.Round(0.07 * imponible, 0, MidpointRounding.AwayFromZero);
                }
                else
                {
                    imponibleFinal = imponible;
                    salud = Math.Round(0.07 * imponible, 0, MidpointRounding.AwayFromZero);
                }

                //VALOR DE PAGO ADICIONAL
                if (isapre > salud)
                    adicional = isapre - salud;
                else if (salud > isapre)
                    adicional = 0;
                else if (salud == isapre)
                    adicional = 0;

                AgregaCampo(imponibleFinal.ToString());
            }
            else { AgregaCampo("0"); }
            //78-MONEDA DEL PLAN PACTADO
            //if (codSalud == "07")
            AgregaCampo("1");
            //else
            //  AgregaCampo("2");
            //79-COTIZACION PACTADA
            if (codSalud != "00" && codSalud != "07")
            {
                if (Labour.Calculo.GetValueFromCalculoMensaul(person.Contrato, person.PeriodoPersona, "sysdiaslic") != 0)
                {
                    //parcial = Math.Round(varSistema.ObtenerValorLista("sysisapre"));
                    parcial = Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(pContrato, Periodo, "sysisapre"));
                    AgregaCampo(FormatearCotizacionPactada(parcial.ToString()));
                    pReportePreviredIsapre.Cotizacion = parcial;
                }
                else
                {
                    //OBTENEMOS VALOR PACTADO
                    val = GetCotizacionPactada(pContrato, Periodo);
                    AgregaCampo(FormatearCotizacionPactada(val));
                    if (Convert.ToDouble(val) < salud)
                        pReportePreviredIsapre.Cotizacion = Convert.ToDouble(salud);
                    else
                        pReportePreviredIsapre.Cotizacion = Convert.ToDouble(val);
                }
            }
            else
                AgregaCampo("00000000");

            //80-COTIZACION OBLIGATORIA ISAPRE
            //CORRESPONDE A RENTA IMPONIBLE ISAPRE (CAMPO 77) * 7%
            if (codSalud != "00" && codSalud != "07")
            {
                AgregaCampo(salud.ToString());
                //if (pReportePreviredIsapre.Cotizacion == 0)
                //    if (salud > 0)
                       // pReportePreviredIsapre.Cotizacion = salud;

            }                
            else
                AgregaCampo("0");
            //81-COTIZACION ADICIONAL VOLUNTARIA
            if (codSalud != "00" && codSalud != "07")
            {
                AgregaCampo(Math.Round(adicional, 0, MidpointRounding.AwayFromZero).ToString());
                //pReportePreviredIsapre.AdicionalIsapre = Math.Round(adicional, 0, MidpointRounding.AwayFromZero);
            }
            else
                AgregaCampo("0");

            //82-MONTO GARANTIA EXPLICITA DE SALUD (USO FUTURO)
            AgregaCampo("0");
        }

        //*******************************
        // DATOS CAJA DE COMPENSACION   |
        //*******************************
        private void DatosCaja(double imponible, double topeAfp, string codSalud, ReportePrevired pReportePrevired, List<MovimientoPersonal> pMovimientos, Persona person, IndiceMensual pIndice, double pMontoFam, double pPrestamos, double pAhorro, double pSeguroVida)
        {
            //83-CODIGO CCAF
            double Tope = 0;


            //Si es regimen antiguo el tope debe ser el tope ips
            if (person.codCajaPrev == 0)
            {
                if (imponible > topeAfp)
                    Tope = Math.Round(topeAfp);
                else
                    Tope = Math.Round(imponible);
            }
            else
            {
                if (imponible > pIndice.TopeIpsPesos)
                    Tope = Math.Round(pIndice.TopeIpsPesos);
                else
                    Tope = Math.Round(imponible);

            }


            string codCaja = GetCodigoCaja();
            AgregaCampo(codCaja == "00" ? "0" : codCaja);
            //84-RENTA IMPONIBLE CCAF (SIEMPRE Y CUANDO ESTE AFILIADO A CAJA)
            if (codCaja != "00")
            {

                AgregaCampo(Math.Round(Tope, 0, MidpointRounding.AwayFromZero).ToString());
                //if (imponible > topeAfp)
                //    AgregaCampo(Math.Round(topeAfp, 0, MidpointRounding.AwayFromZero).ToString());
                //else if (imponible < topeAfp)
                //    AgregaCampo(Math.Round(imponible, 0, MidpointRounding.AwayFromZero).ToString());
                //else
                //    AgregaCampo(Math.Round(imponible, 0, MidpointRounding.AwayFromZero).ToString());
            }
            else
                AgregaCampo("0");

            //85-CREDITOS PERSONALES CCAF
            if (codCaja != "00")
            {
                AgregaCampo(pPrestamos.ToString());
                //Agregamos monto a reporte
                pReportePrevired.Prestamo = pPrestamos;
            }                
            else
            {
                AgregaCampo("0");
            }
            //86-DESCUENTOS DENTAL CCAF
            AgregaCampo("0");
            //87-DESCUENTO POR LEASING
            if (codCaja != "00")
            {
                AgregaCampo(pAhorro.ToString());
                pReportePrevired.Leasing = pAhorro;
            }               
            else
            {
                AgregaCampo("0");
            }
            //88-DESCUENTOS POR SEGURO DE VIDA
            if (codCaja != "00")
            {
                AgregaCampo(pSeguroVida.ToString());
                //Agregamos monto a reporte
                pReportePrevired.SeguroVida = pSeguroVida;
            }               
            else
            {
                AgregaCampo("0");
            }
            //89-OTROS DESCUENTOS
            AgregaCampo("0");
            //90-COTIZACION CCAG DE NO AFILIADOS A ISAPRES (SOLO APLICA SI COTIZA EN FONASA)
            //CORRESPONDE RENTA IMPONIBLE * 0.6% (RENTA IMPONIBLE DEL CAMPO 84)
            //0.6 corresponde a lo que se paga si esta afiliado a caja de compensacion
            if (codCaja != "00")
            {
                if (codSalud == "07")
                {
                    AgregaCampo(Math.Round(0.006 * Tope).ToString());
                    pReportePrevired.Cotizacion = Math.Round(0.006 * Tope);
                }
                else
                {
                    AgregaCampo("0");
                }
            }
            else
                AgregaCampo("0");

            //91-DESCUENTOS CARGAS FAMILIARES CCAF (SOLO SI ESTA EN CCAF)
            //AgregaCampo(varSistema.ObtenerValorLista("systfam").ToString());
           if (codCaja != "00")
           {
                AgregaCampo(Math.Round(pMontoFam).ToString());
           }               
            else
              AgregaCampo("0");
            //92-OTROS DESCUENTOS CCAF 1
            AgregaCampo("0");
            //93-OTROS DESCUENTOS CCAF 2
            AgregaCampo("0");
            //94-BONOS GOBIERNO 
            AgregaCampo("0");
            //95-CODIGO DE SUCURSAL
            AgregaCampo("");
        }

        //*******************
        // DATOS MUTUALIDAD |
        //*******************        

        private void DatosMutualidad(double imponible, string pContrato, int pPeriodo, double pTopeAfp, ReportePrevired pReportePrevired, List<MovimientoPersonal> pMovimientos, Persona person, IndiceMensual pIndice)
        {
            double imponibleFinal = 0, tope = 0, mutual = 0;
            DateTime Fecha = DateTime.Now.Date;
            Fecha = fnSistema.PrimerDiaMes(pPeriodo);
            int DiasMes = DateTime.DaysInMonth(Fecha.Year, Fecha.Month);
            //96-CODIGO MUTUALIDAD            
            AgregaCampo(GetCodigoMutual());
            //97-RENTA IMPONIBLE MUTUAL
            if (GetCodigoMutual() != "00")
            {
                //SI EL TRABAJADOR NO TIENE IMPONIBLES POR ESTAR CON LICENCIA DEBEMOS DECLARAR EL IMPONEBLE DEL
                //MES EN EL CUAL NO HAYAN LICENCIAS Y TENGA TRABAJADO MES COMPLETO                
                //pTopeAfp = Labour.Calculo.GetValueFromCalculoMensaul(pContrato, pPeriodo, "systopeafp");
                //SI ES VERDADERO NO TUVO IMPONIBLE EN EL MES
                if (Labour.Calculo.GetValueFromCalculoMensaul(pContrato, pPeriodo, "sysdiaslic") == DiasMes)
                {
                    Haberes hab = new Haberes(pContrato, pPeriodo);

                    //SI EXISTE SOLO LO CONSULTAMOS
                    if (Labour.Calculo.ExisteImpAnterior(pContrato))
                        imponibleFinal = Labour.Calculo.ConsultaImpAnterior(pContrato);
                    else
                        //SI NO EXISTE DEBEMOS CALCULARLO
                        imponibleFinal = hab.ImponibleAnteriorGenerico(hab.getContrato(), hab.getPeriodo());

                    //SI LOS DIAS DE LICENCIA SON 31 DEBEMOS ADICIONAL AL IMPONIBLE EL DIA 31
                    imponibleFinal = (imponibleFinal * 31) / 30;

                    //FINALMENTE VERIFICAMOS QUE NO SUPERE EL TOPE LEAGL
                    if (imponibleFinal > pTopeAfp)
                        imponibleFinal = pTopeAfp;

                }
                else
                {
                    //Dias proporcionales
                    //Tuvo dias trabajados
                    //SOLO USAMOS EL IMPONIBLE GENERADO EN EL MES
                    if (person.codCajaPrev == 0)
                    {
                        if (imponible > pTopeAfp)
                            imponibleFinal = pTopeAfp;
                        else if (imponible < pTopeAfp)
                            imponibleFinal = imponible;
                        else
                            imponibleFinal = imponible;
                    }
                    else
                    {
                        if (imponible > pIndice.TopeIpsPesos)
                            imponibleFinal = Math.Round(pIndice.TopeIpsPesos);
                        else
                            imponibleFinal = Math.Round(imponible);
                    }



                }

                if(imponibleFinal.ToString() == "0")
                    AgregaCampo("0.0");
                else
                    AgregaCampo(Math.Round(imponibleFinal).ToString());
            }
            else { AgregaCampo("0"); }

            //98-COTIZACION ACCIDENTE DEL TRABAJO
            //SE CALCULA EN BASE A RENTA IMPONIBLE (CAMPO 97) * TASSA ASIGNADA A LA EMPRESA

            tope = Labour.Calculo.GetValueFromCalculoMensaul(pContrato, pPeriodo, "systopesalud");
            mutual = Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(pContrato, pPeriodo, "sysmutual"), 0, MidpointRounding.AwayFromZero) + Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(pContrato, pPeriodo, "sysvalsanna"), 0, MidpointRounding.AwayFromZero);

            AgregaCampo(mutual.ToString());
            pReportePrevired.Cotizacion = mutual;
            //99-CODIGO SUCURSAL PARA PAGO MUTUAL
            Empresa emp = new Empresa();
            emp.SetInfo();
            //AgregaCampo(GetSucursal(pContrato, pPeriodo));
            AgregaCampo(emp.CodEmpresa);
        }

        //*****************************
        // DATOS ADMINSTRADORA SEGURO  |  
        //*****************************
        private void DatosAdministradorSeguro(bool AdultoMayor, bool MenorEdad, double imponible, string pContrato, int pPeriodo, Persona pPerson, ReportePrevired pReportePrevired)
        {
            double diasTrab = 0, Imp30 = 0;
            double ImpCompleto = 0;
            double AporteEmpresa = 0, AporteTrab = 0;
            diasTrab = Labour.Calculo.GetValueFromCalculoMensaul(pContrato, pPeriodo, "sysdiastr");
            //100-RENTA IMPONIBLE SEGURO CESANTIA
            //AgregaCampo(Math.Round(GetImponibleSeguroCes(pContrato, pPeriodo), 0, MidpointRounding.AwayFromZero).ToString());            

            //SOLO PAGAMOS SEGURO DE CESANTIA SI APLICA
            //LAS PERSONAS JUBILADAS NO PAGAN SEGURO DE CESANTÍA
            //LOS MENORES DE 18 AÑOS
            DateTime Fecha = DateTime.Now.Date;
            Fecha = pPerson.Nacimiento.AddYears(18);
            //SI LA FECHA EN QUE CUMPLE LOS 18 AÑOS ES MAYOR AL ULTIMO DIA DE PERIODO EVALUADO, ES MENOR DE EDAD.
            DateTime Last = DateTime.Now.Date;
            Last = fnSistema.UltimoDiaMes(pPeriodo);

            //1 --> SI, NO COTIZA
            if (pPerson.Jubilado == 1 || (Fecha > Last))
            {
                //100
                AgregaCampo("0");
                //101
                AgregaCampo("0");
                //102
                AgregaCampo("0");
            }
            else
            {
                //100 - RENTA IMPONIBLE SEGURO CESANTÍA
                //SI DIAS TRABAJADOS ES IGUAL A 0, NO TRABAJÓ EN EL MES
                //EN ESTE CASO SE DEBE COLOCAR MES IMPONIBLE MES COMPLETO        
                ImpCompleto = GetImponibleSeguroCes(pContrato, pPeriodo);
                AgregaCampo(Math.Round(ImpCompleto, MidpointRounding.AwayFromZero).ToString());
                //101-APORTE TRABAJADOR SEGURO CESANTIA        
                AporteTrab = Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(pContrato, pPeriodo, "syscicest"));
                AgregaCampo(AporteTrab.ToString());
                pReportePrevired.SegAfiliado = AporteTrab;

                //102-APORTE EMPLEADOR SEGURO CESANTIA
                AporteEmpresa = Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(pContrato, pPeriodo, "sysfscese") + Labour.Calculo.GetValueFromCalculoMensaul(pContrato, pPeriodo, "syscicese"));
                AgregaCampo(AporteEmpresa.ToString());
                //AgregaCampo("0");
                pReportePrevired.SegEmpresa = AporteEmpresa;
            }

        }

        //************************
        // DATOS PAGADOR SUBSIDIO|
        //************************
        private void DatosPagadorSubsidio(List<MovimientoPersonal> Listado, string pContrato)
        {
            //103-RUT PAGADORA SUBSIDIOS
            if (Listado.Count > 0)
            {
                //SUBSIDIO
                if (Listado[0].codMovimiento == "3")
                {
                    AgregaCampo(fnSistema.GetRutsindv(RutPagadoraSubsidio(Periodo, pContrato)));
                }
                else { AgregaCampo("0"); }
            }
            else { AgregaCampo("0"); }

            //104-DV PAGADORA SUBSIDIO
            if (Listado.Count > 0)
            {
                if (Listado[0].codMovimiento == "3")
                {
                    AgregaCampo(fnSistema.Getdigito(RutPagadoraSubsidio(Periodo, pContrato)));
                }
                else { AgregaCampo(" "); }
            }
            else { AgregaCampo(" "); }
        }

        //****************
        // DATOS EMPRESA |
        //****************
        private void DatosEmpresa(string pContrato, int pPeriodo)
        {
            //105-CENTRO COSTO
            //string centro = "";
            //centro = GetCentroCosto(pContrato, pPeriodo);

            //if (centro.Length == 0)
            //    AgregaCampo("0");
            //else
            //{
            //    centro = fnSistema.GetFormat(centro, 20, 2);
            //    AgregaCampo(centro);

            //}

        }
        #endregion

        #region "FUNCION PARA SUSPENSION"
        //***************************
        // DATOS DEL TRABAJADOR     |
        //***************************

        private bool DatosTrabajadorSuspension(string diasTrabajados, string TipoLinea,
            string CargaNormal, string CargaMaternal, string CargaInvalido, string MontoCarga,
            MovimientoPersonal Movimiento, string MontoCargaRetroactivo, Persona person,
            string pDiasSus13, string pDiasSus14, string pDiasSus15, string pTipoLine, bool? isFirst = false)
        {

            //01-RUT SIN DIGITO
            //ENMASCARAR RUT
            AgregaCampo(fnSistema.GetRutsindv(Rut));
            //02-DIGITO VERIFICADOR
            AgregaCampo(fnSistema.Getdigito(Rut));
            //03-APELLIDO PATERNO
            if (person.Apepaterno.ToString().Length > 30)
            { XtraMessageBox.Show("El apellido paterno no puede contener mas de 30 caracteres", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return false; }
            else
                AgregaCampo(person.Apepaterno);
            //04-APELLIDO MATERNO
            if (person.Apematerno.ToString().Length > 30)
            { XtraMessageBox.Show("El apellido materno no puede contener mas de 30 caracteres", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return false; }
            else
                AgregaCampo(person.Apematerno);
            //05-NOMBRES
            if ((person.Nombre.ToString().Length > 30))
            { XtraMessageBox.Show("El nombre no puede contener mas de 30 caracteres", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return false; }
            else
                AgregaCampo(person.Nombre);
            //06-SEXO
            AgregaCampo(GetCodigoSexo(person.Sexo));
            //07-NACIONALIDAD
            AgregaCampo((GetCodigoNacionalidad(person.codNacion)).ToString());
            //08-TIPO DE PAGO
            AgregaCampo("01");

            //09-FECHA REMUNERACION DESDE       
            AgregaCampo(fnSistema.PeriodoInvertido(Periodo));

            //10-FECHA REMUNERACION HASTA       
            AgregaCampo(fnSistema.PeriodoInvertidoUltimo(Periodo));

            //11-REGIMEN PREVISIONAL
            AgregaCampo(GetRegimenPrevision(person.Regimen, person.Nacimiento));

            //12-TIPO TRABAJADOR
            AgregaCampo((GetTipoTrabajador(person.Jubilado, person.Nacimiento)).ToString());

            //13-DIAS TRABAJADOS           
            AgregaCampo((Movimiento.Dias + Convert.ToDouble(diasTrabajados)) + "");

            //14-TIPO DE LINEA
            AgregaCampo(pTipoLine);

            //15-CODIGO MOVIMIENTO PERSONAL
            //VEMOS SI TIENE LICENCIAS U OTRO MOVIMIENTO DE PERSONAL 

            if (Movimiento != null)
            {
                AgregaCampo(Movimiento.codMovimiento);
            }
            else
            {
                AgregaCampo("0");
            }

            //16-FECHA DESDE --> FECHA INICIO MOVIMIENTO PERSONAL (dd mm yyyy)
            if (Movimiento != null)
            {
                AgregaCampo(Movimiento.inicioMovimiento.ToShortDateString());
            }
            else
            {
                AgregaCampo("");
            }


            //17-FECHA HASTA --> FECHA FIN MOVIMIENTO PERSONAL (DD MM YYYY)
            //Tambien en el caso de haya suspension laboral se debe agregar fecha
            if (Movimiento != null)
            {
                AgregaCampo(Movimiento.terminoMovimiento.ToShortDateString());
            }
            else
            {
                AgregaCampo("");
            }

            //18-TRAMO ASIGNACION FAMILIAR 
            AgregaCampo(GetTramoAsignacionFamiliar(person.Tramo));

            //19-NUMERO CARGAS FAMILIARES SIMPLES
            if (isFirst == true)
            {
                if (Convert.ToInt32(CargaNormal) > 13)
                { XtraMessageBox.Show("Las cargas simples no pueden ser mayor a 13", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return false; }
                AgregaCampo(CargaNormal);
            }
            else
            {
                AgregaCampo("0");
            }

            //20-NUMERO CARGAS FAMILIARES MATERNALES
            if (isFirst == true)
            {
                if (Convert.ToInt32(CargaMaternal) > 1)
                { XtraMessageBox.Show("Las cargas maternales no pueden ser superiores a 1", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return false; }
                AgregaCampo(CargaMaternal);
            }
            else
            {
                AgregaCampo("0");
            }


            //21-NUMERO CARGAS FAMILIARES INVALIDAS
            if (isFirst == true)
            {
                if (Convert.ToInt32(CargaInvalido) > 1)
                { XtraMessageBox.Show("las cargas invalidas no pueden ser superio a 1", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return false; }
                AgregaCampo(CargaInvalido);
            }
            else
            {
                AgregaCampo("0");
            }


            //22-MONTO ASIGNACION EN PESOS CARGAS FAMILIARES
            if (isFirst == true)
                AgregaCampo(MontoCarga);
            else
                AgregaCampo("0");
            //23-ASIGNACION FAMILIAR RETROACTIVA
            if (isFirst == true)
                AgregaCampo(MontoCargaRetroactivo);
            else
                AgregaCampo("0");

            //24-REINTEGRO CARGAS FAMILIARES
            AgregaCampo("0");

            //25-SOLICITUD TRABAJADOR JOVEN
            AgregaCampo("N");

            return true;
        }

        //*************
        //  DATOS AFP |
        //*************
        private void DatosAfpSuspension(bool AdultoMayor, double TopeAfp, double imponible,
            Persona person, ReportePrevired pReportePreviredSus, ReportePrevired pReportePreviredNormal,
            MovimientoPersonal pMovimiento, double pDiasSp13, double pDiasSp14, double pDiasSp15,Hashtable pData, bool? isFirst = false)
        {
            string codAfp = "";
            double afp = 0, comafp = 0, segInvSus = 0, afpaho = 0;
            double segInvNormal = 0;
            double AfpSuspension = 0;
            codAfp = GetCodigoAfp(person.codAfp);
            //26-CODIGO AFP
            AgregaCampo(codAfp);

            //27-RENTA IMPONIBLE AFP (SIEMPRE Y CUANDO COTICE EN AFP)
            //En caso de suspension debe ser la renta imponible que utilizó en calculo para suspension
            //podemos usar el imponible de mes completo y calcular un proporcional 
            //en base a los dias de suspension del movimiento de personal
            /* 1 -> AFP - SALUD
             * 
             */
            if (codAfp != "SIP" && person.Regimen == 1)
            {
                if (pMovimiento != null)
                {
                    //if (pMovimiento.codMovimiento == "13")
                    //    imponible = ((Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(person.Contrato, person.PeriodoPersona, "systimp30"))) / 30) * pDiasSp13;
                    //if (pMovimiento.codMovimiento == "14")
                    imponible = Convert.ToDouble(pData["ImponibleAfpSuspension"]) + Convert.ToDouble(pData["ImponibleOriginal"]);
                    //imponible = Labour.Calculo.GetValueFromLiquidacionHistorica(person.Contrato, fnSistema.fnObtenerPeriodoAnterior(person.PeriodoPersona), "imponible");
                    //if (imponible == 0)
                    //    imponible = Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(person.Contrato, person.PeriodoPersona, "systimp30"));
                    //else
                    //{
                    //    imponible = Math.Round((Configuracion.ConfiguracionGlobal.PorcentajeSuspension * imponible) / 100, 0);
                    //}

                    //if (imponible > TopeAfp)
                    //    imponible = TopeAfp;

                    //if(pMovimiento.codMovimiento == "13")
                    //    imponible = Math.Round((pMovimiento.Dias * imponible) / pDiasSp13);
                    //if(pMovimiento.codMovimiento == "14")
                    //    imponible = Math.Round((pMovimiento.Dias * imponible) / pDiasSp14);
                }
                else
                {
                    if (imponible > TopeAfp)
                        imponible = TopeAfp;
                    else if (imponible < TopeAfp)
                        imponible = Math.Round(imponible);
                    else
                        imponible = Math.Round(imponible);

                    //imponible = Convert.ToDouble(pData["ImponibleAfp"]);
                }

                //AGREGAMOS VARIABLE IMPONIBLE
                AgregaCampo(imponible.ToString());
            }
            else
                AgregaCampo("0");

            //28-COTIZACION OBLIGATORIA AFP
            //SUMAMOS COTIZACION INDIVIDUAL + COTIZACION ADMINSTRACION AFP
            if (codAfp != "SIP" && person.Regimen == 1)
            {
                //afp = Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(person.Contrato, person.PeriodoPersona, "sysciafp"));
                //comafp = Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(person.Contrato, person.PeriodoPersona, "syscomafp"));

                //Normal + Cotizacion afp por suspension
                afp = Convert.ToDouble(pData["CotizacionAfp"]);
                AfpSuspension = Convert.ToDouble(pData["AfpSuspension13"]) + Convert.ToDouble(pData["AfpSuspension14"]);
               
                //if (pMovimiento.codMovimiento == "13")
                //{
                //    AfpSuspension = Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(person.Contrato, person.PeriodoPersona, "sysafpsp13"));
                //    //afp = Math.Round((afp * pMovimiento.Dias) / pDiasSp13);
                //}

                //if (pMovimiento.codMovimiento == "14")
                //{
                //    AfpSuspension = Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(person.Contrato, person.PeriodoPersona, "sysafpsp14"));
                //    //afp = Math.Round((afp * pMovimiento.Dias) / pDiasSp14);
                //}

                //if (pMovimiento.codMovimiento == "15")
                //{
                //    AfpSuspension = Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(person.Contrato, person.PeriodoPersona, "sysafpsp15"));
                //    //afp = Math.Round((afp * pMovimiento.Dias) / pDiasSp15);

                //}

                //Afp normal si es que hay
                //Cotizacion por dias normales
                //afp = Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(person.Contrato, person.PeriodoPersona, "sysciafp"));
                //comafp = Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(person.Contrato, person.PeriodoPersona, "syscomafp"));
                pReportePreviredNormal.Cotizacion = afp;

                //por suspension
                //Si es el primer elemento 
                AgregaCampo((AfpSuspension + afp).ToString());
                pReportePreviredSus.Cotizacion = AfpSuspension;
                //AgregaCampo((Math.Round(varSistema.ObtenerValorLista("sysciafp") + varSistema.ObtenerValorLista("syscomafp"))).ToString());
            }
            else
                AgregaCampo("0");

            //29-COTIZACION SEGURO DE INVALIDEZ
            
            //if (pMovimiento.codMovimiento == "13")
            //{
            //    segInvSus = Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(person.Contrato, person.PeriodoPersona, "sysinv13"));
            //    //segInv = Math.Round((segInv * pMovimiento.Dias) / pDiasSp13);
            //}

            //if (pMovimiento.codMovimiento == "14")
            //{
            //    segInvSus = Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(person.Contrato, person.PeriodoPersona, "sysinv14"));
            //    //segInv = Math.Round((segInv * pMovimiento.Dias) / pDiasSp14);
            //}

            //if (pMovimiento.codMovimiento == "15")
            //{
            //    segInvSus = Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(person.Contrato, person.PeriodoPersona, "sysinv15"));
            //    //segInv = Math.Round((segInv * pMovimiento.Dias) / pDiasSp15);
            //}

            segInvSus = Convert.ToDouble(pData["SeguroInvalidezSuspension13"]) + Convert.ToDouble(pData["SeguroInvalidezSuspension14"]);
            segInvNormal = Convert.ToDouble(pData["SeguroInvalidez"]);

            //seguro invalides por dias normales
            //segInvNormal = Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(person.Contrato, person.PeriodoPersona, "syssis"));
            pReportePreviredNormal.SegInv = segInvNormal;

            //Seguro invalides por dias de suspension
            AgregaCampo(segInvSus + segInvNormal + "");
            pReportePreviredSus.SegInv = segInvSus;

            //30-CUENTA AHORRO VOLUNTARIO
            afpaho = Convert.ToDouble(pData["AhorroAfp"]);
            //afpaho = ItemTrabajador.GetValue("afpaho", person.Contrato, person.PeriodoPersona);
            AgregaCampo(afpaho + "");
            pReportePreviredNormal.AhorroVoluntario = afpaho;

            //31-RENTA IMPONIBLE SUSTITUTIVA
            AgregaCampo("0");

            //32-TASA PACTADA PARA RENTA SUSTITUTIVA
            AgregaCampo("0,00");

            //33-MONTO APORTE INDEMNIZACION
            AgregaCampo("0");

            //34-N° PERIODOS DE APORTE SUSTITUTIVO CONVENIDO
            AgregaCampo("00");

            //35-DIA, MES, AÑO INICIO APORTE SUSTITUTIVO CONVENIDO
            AgregaCampo("0");

            //36-DIA, MES, AÑO TERMINO APORTE SUSTITUTIVO CONVENIDO
            AgregaCampo("0");

            //37-PUESTO DE TRABAJO PESADO
            AgregaCampo("");

            //38-% COTIZACION TRABAJO PESADO (2% o 4%)
            AgregaCampo("00,00");

            //39-COTIZACION TRABAJO PESADO
            AgregaCampo("0");
        }

        //***************************************
        //  DATOS AHORRO PREVISIONAL VOLUNTARIO |
        //***************************************
        private void DatosAhorroVoluntarioIndividualSuspension(Persona pPerson, ReportePrevired pReportePrevired, Hashtable pData)
        {
            double Aprevol = 0;
            Aprevol = Convert.ToDouble(pData["AhorroVoluntarioIndividual"]);
            //Aprevol = ItemTrabajador.GetValue("aprevol", pPerson.Contrato, pPerson.PeriodoPersona);
            string codAfp = GetCodigoAfp(pPerson.codAfp);

            //40-CODIGO INSTITUCION APVI
            if (pPerson.codCajaPrev == 0)
            {
                if (Aprevol > 0)
                    AgregaCampo("0" + codAfp);
                else
                    AgregaCampo("000");
            }
            else
            {
                AgregaCampo("000");
            }

            //41-NUMERO CONTRATO APVI
            AgregaCampo("");

            //42-FORMA DE PAGO APVI (1:directo 2:indirecto)
            if (pPerson.codCajaPrev == 0)
            {
                if (Aprevol > 0)
                    AgregaCampo("2");
                else
                    AgregaCampo("0");
            }
            else
            {
                AgregaCampo("0");
            }
            //43-COTIZACION APVI
            //Aprevol = ItemTrabajador.GetValue("aprevol", pPerson.Contrato, pPerson.PeriodoPersona);
            if (pPerson.codCajaPrev == 0)
            {
                if (Aprevol > 0)
                    AgregaCampo(Aprevol + "");
                else
                    AgregaCampo("0");
                pReportePrevired.AhorroPrevisional = Aprevol;
            }
            else
            {
                AgregaCampo("0");
            }
            //pReportePrevired.AhorroPrevisional = 0;
            //44-COTIZACION DEPOSITOS CONVENIDOS
            AgregaCampo("0");
        }

        //***************************************************
        // DATOS AHORRO PREVISIONAL VOLUNTARIO COLECTIVO    |
        //***************************************************
        private void DatosAhorroVoluntarioColectivoSuspension()
        {
            //45-CODIGO INSTITUCION AUTORIZADA APVC
            AgregaCampo("000");
            //46-NUMERO DE CONTRATO APVC
            AgregaCampo("");
            //47-FORMA DE PAGO APVC
            AgregaCampo("0");
            //48-COTIZACION TRABAJADOR APVC
            AgregaCampo("0");
            //49-COTIZACION EMPLEADOR APVC
            AgregaCampo("0");
        }

        //*****************************
        //  DATOS AFILIADO VOLUNTARIO |
        //*****************************
        private void DatosAfiliadoVoluntarioSuspension()
        {
            //50-RUT AFILIADO VOLUNTARIO
            AgregaCampo("0");
            //51-DV AFILIADO VOLUNTARIO
            AgregaCampo(" ");
            //52-APELLIDO PATERNO
            AgregaCampo("");
            //53-APELLIDO MATERNO
            AgregaCampo("");
            //54-NOMBRES
            AgregaCampo("");
            //55-CODIGO MOVIMIENTO DE PERSONAL
            AgregaCampo("0");
            //56-FECHA DESDE
            AgregaCampo("");
            //57-FECHA HASTA
            AgregaCampo("");
            //58-CODIGO DE LA AFP
            AgregaCampo("0");
            //59-MONTO CAPITALIZACION VOLUNTARIA --> debe ser mayor o igual al 10% del sueldo minimo
            //del trabajador dependiente
            AgregaCampo("0");
            //60-MONTO AHORRO VOLUNTARIO
            AgregaCampo("0");
            //61-NUMERO DE PERIODOS DE COTIZACION
            AgregaCampo("0");
        }

        //********************************
        // DATOS IPS-ISL-FONASA
        //******************************
        private void DatosIpsSuspension(string codSalud, double Imponible, 
            double FonasaSuspension, double pFonasaNormal, Persona pPerson,
            string pMontoCargasTotal, string pRegimenPrevisional,
            ReportePrevired pReportePreviredFonasa, ReportePrevired pReporteFonasaNormal, 
            ReportePrevired pReportePreviredIpsSuspension, ReportePrevired pReporteIpsNormal,
            MovimientoPersonal pMovimiento, double pDias13, double pDias14, double pDias15, Hashtable pData)
        {
            double TotalFonasa = 0, ImponibleIps = 0, topeAfpAnterior = 0;
            double ImponibleIpsNormal = 0;
            //DATOS DE CAJAPREVISION
            CajaPrevision caja = new CajaPrevision();
            caja.SetInfo(pPerson.Contrato, pPerson.PeriodoPersona);

            IndiceMensual I = new IndiceMensual();
            I.SetInfoMes(fnSistema.fnObtenerPeriodoAnterior(Labour.Calculo.PeriodoObservado));
            topeAfpAnterior = Math.Round(I.TopeAfp * I.Uf);

            //REGIMEN 4 Y 5 SIN IPS
            //62-CODIGO EX-CAJA REGIMEN
            if (pPerson.Regimen == 4 || pPerson.Regimen == 5)
            {
                if (caja.id != 0)
                {
                    AgregaCampo(caja.dato01);
                }
                else
                    AgregaCampo("0000");
            }
            else
                AgregaCampo("0000");

            //63-TAZA COTIZACION EX-CAJA PREVISION
            if (pPerson.Regimen == 4 || pPerson.Regimen == 5)
            {
                if (caja.id != 0)
                {
                    AgregaCampo((caja.porcPension * 100).ToString());
                }
                else
                    AgregaCampo("0000");
            }
            else
                AgregaCampo("00,00");

            //64-RENTA IMPONIBLE IPS
            if (codSalud == "07")
            {
                                
                ImponibleIpsNormal = Convert.ToDouble(pData["ImponibleIps"]);

                if (pMovimiento != null)
                {
                    ImponibleIps = Convert.ToDouble(pData["ImponibleIpsSuspension"]);

                }         

                AgregaCampo(Math.Round(ImponibleIps + ImponibleIpsNormal) + "");

            }
            else
                AgregaCampo("0");

            //65-COTIZACION OBLIGATORIA IPS (100% suspension laboral)
            if (pPerson.Regimen == 4 || pPerson.Regimen == 5)
            {
                if (caja.id != 0)
                {
                    //Cotizacion ips normal
                    double IpsSuspension = Convert.ToDouble(pData["AfpSuspensionIps"]);
                    double IpsNormal = Convert.ToDouble(pData["CotizacionIps"]);
                    pReporteIpsNormal.Cotizacion = IpsNormal;

                    //Obtenemos el porcentaje desde el imponible.       
                    //Agregamos la suma de las dos (normal y suspension)
                    AgregaCampo((IpsSuspension + IpsNormal) + "");
                    pReportePreviredIpsSuspension.Cotizacion = IpsSuspension;                    

                }
                else
                    AgregaCampo("0");
            }
            else
                AgregaCampo("0");
            //66-RENTA IMPONIBLE DESAHUCIO
            AgregaCampo("0");
            //67-CODIGO EX-CAJA REGIMEN DESAHUCIO
            AgregaCampo("0");
            //68-TASA COTIZACION DESAHUCIO EX-CAJAS DE PREVISION
            AgregaCampo("00,00");
            //69-COTIZACION DESAHUCIO
            AgregaCampo("0");
            //70-COTIZACION FONASA       
            //EN CASO DE EXISTIR ASOCIACION A CAJA EL MONTO A PAGAR EA
            // SALUD -> 6.4%
            // CAJA -> 0.6
            //SI NO HAY CAJA ES 7%
            if (GetCodigoCaja() != "00")
            {
                if (codSalud == "07")
                {
                    //Es regimen antiguo?
                    if (pPerson.Regimen == 4 || pPerson.Regimen == 5)
                    {
                        //AgregaCampo(Math.Round(ImponibleIpsNormal * 0.064).ToString());
                        pReporteIpsNormal.CotizacionFonasa = Convert.ToDouble(pData["CotizacionFonasaIps"]);

                        //OBTENEMOS EL 6.4% DEL IMPONIBLE
                        //Sumamos ambos
                        AgregaCampo(Math.Round(Convert.ToDouble(pData["FonasaIpsSuspension"])) + Math.Round(Convert.ToDouble(pData["CotizacionFonasaIps"])) + "");
                        pReportePreviredIpsSuspension.CotizacionFonasa = Convert.ToDouble(pData["FonasaIpsSuspension"]);
                    }
                    else
                    {

                        pReporteFonasaNormal.Cotizacion = Convert.ToDouble(pData["CotizacionFonasaNormal"]);

                        //Fonasa por suspension
                        pReportePreviredFonasa.Cotizacion = Convert.ToDouble(pData["FonasaNormalSuspension"]);
                        AgregaCampo(Math.Round(pReporteFonasaNormal.Cotizacion + pReportePreviredFonasa.Cotizacion) + "");
                    }
                }
                else
                    AgregaCampo("0");
            }
            else
            {

                if (codSalud == "07")
                {
                    //ES REGIMEN ANTIGUO???
                    if (pPerson.Regimen == 4 || pPerson.Regimen == 5)
                    {
                        //Salud normal
                        pReporteIpsNormal.CotizacionFonasa = Convert.ToDouble(pData["CotizacionFonasaIps"]);

                        //Sumamos los dos
                        AgregaCampo(Math.Round(Convert.ToDouble(pData["CotizacionFonasaIps"])) + Math.Round(Convert.ToDouble(pData["FonasaIpsSuspension"])) + "");
                        //salud suspension
                        pReportePreviredIpsSuspension.CotizacionFonasa = Math.Round(Convert.ToDouble(pData["FonasaIpsSuspension"]));
                    }
                    else
                    {
                        //Fonasa normal                        
                        pReporteFonasaNormal.Cotizacion = Convert.ToDouble(pData["CotizacionFonasaNormal"]);

                        //Por suspension
                        //TotalFonasa = Math.Round(FonasaSuspension);
                        pReportePreviredFonasa.Cotizacion = Math.Round(Convert.ToDouble(pData["FonasaNormalSuspension"]));
                        AgregaCampo(Math.Round(Convert.ToDouble(pData["CotizacionFonasaNormal"])) + Math.Round(Convert.ToDouble(pData["FonasaNormalSuspension"])) + "");
                    }
                }
                else
                {
                    AgregaCampo("0");
                }
            }

            //71-COTIZACION ACC. TRABAJO (ISL)
            AgregaCampo("0");
            //72-BONIFICACION LEY 15.386
            AgregaCampo("0");
            //73-DESCUENTO POR CARGAS FAMILIARES DE ISL (SE DEBE INFORMAR SI NO ESTÁ EN CAJA DE COMPENSACION)
            AgregaCampo("0");
            //74-BONOS GOBIERNO
            AgregaCampo("0");
        }

        //**************************
        // DATOS SALUD 
        //**************************
        private void DatosSaludSuspension(string codSalud, double imponible, double TopeSalud,
            ReportePrevired pReportePreviredIsapreSuspendido, ReportePrevired pReporteIsapreNormal, Persona person,
            MovimientoPersonal pMovimiento, double pDias13, double pDias14, double pDias15, Hashtable pData)
        {
            double isapre = 0, adicional = 0, salud = 0, parcial = 0, isapreNormal = 0, adicionalNormal = 0;
            double saludNormal = 0, imponibleFinalNormal = 0;
            string val = "";
            //75-CODIGO INSTITUCION SALUD
            AgregaCampo(codSalud);
            //76-NUMERO DEL FUN
            if (codSalud != "00" && codSalud != "07")
                AgregaCampo(GetFun(person.Contrato, Periodo));
            else
                AgregaCampo("");
            //77-RENTA IMPONIBLE ISAPRE
            if (codSalud != "00" && codSalud != "07")
            {

                isapreNormal = Convert.ToDouble(pData["CotizacionIsapre"]);
                imponibleFinalNormal = Convert.ToDouble(pData["ImponibleIsapre"]);
                saludNormal = Convert.ToDouble(pData["SaludMinimaIsapre"]);
                adicionalNormal = Convert.ToDouble(pData["AdicionalIsapre"]) ;

                imponible = Convert.ToDouble(pData["ImponibleIsapreSuspension"]);
                isapre = Convert.ToDouble(pData["CotizacionIsapreSuspension"]);
                salud = Convert.ToDouble(pData["SaludIsapreSuspensionMinimo"]);
                adicional = Convert.ToDouble(pData["AdicionalIsapreSuspension"]) ;


                AgregaCampo(Math.Round(imponible + imponibleFinalNormal) + "");
            }
            else { AgregaCampo("0"); }
            //78-MONEDA DEL PLAN PACTADO
            //if (codSalud == "07")
            AgregaCampo("1");
            //else
            //  AgregaCampo("2");
            //79-COTIZACION PACTADA
            if (codSalud != "00" && codSalud != "07")
            {
                //val = GetCotizacionPactada(person.Contrato, Periodo, pMovimiento.codMovimiento == "13"? true: false, pMovimiento.codMovimiento == "14" ? true : false);
                //AgregaCampo(FormatearCotizacionPactada(val));
                pReportePreviredIsapreSuspendido.Cotizacion = Convert.ToDouble(pData["CotizacionIsapreSuspension"]);
                if (isapreNormal < saludNormal)
                    pReporteIsapreNormal.Cotizacion = saludNormal;
                else
                    pReporteIsapreNormal.Cotizacion = isapreNormal;
                //pReporteIsapreNormal.Cotizacion = Convert.ToDouble(pData["CotizacionIsapre"]);
                AgregaCampo(Convert.ToDouble(pData["CotizacionIsapre"]) + Convert.ToDouble(pData["CotizacionIsapreSuspension"]) + "");
            }
            else
                AgregaCampo("00000000");

            //80-COTIZACION OBLIGATORIA ISAPRE
            //CORRESPONDE A RENTA IMPONIBLE ISAPRE (CAMPO 77) * 7%
            if (codSalud != "00" && codSalud != "07")
            {
                AgregaCampo(Math.Round(salud + saludNormal) + "");
                //if (pReporteIsapreNormal.Cotizacion == 0)
                //    if (saludNormal > 0)
                //        pReporteIsapreNormal.Cotizacion = saludNormal;
            }                
            else
                AgregaCampo("0");
            //81-COTIZACION ADICIONAL VOLUNTARIA
            if (codSalud != "00" && codSalud != "07")
            {
                
                //pReportePreviredIsapreSuspendido.AdicionalIsapre = Math.Round(adicional, 0, MidpointRounding.AwayFromZero);
                //pReporteIsapreNormal.AdicionalIsapre = Math.Round(adicionalNormal, 0, MidpointRounding.AwayFromZero);
                AgregaCampo((Math.Round(adicional, 0, MidpointRounding.AwayFromZero) + adicionalNormal) + "");
            }
            else
                AgregaCampo("0");

            //82-MONTO GARANTIA EXPLICITA DE SALUD (USO FUTURO)
            AgregaCampo("0");
        }

        //*******************************
        // DATOS CAJA DE COMPENSACION   |
        //*******************************
        private void DatosCajaSuspension(double imponible, double topeAfp, string codSalud, ReportePrevired pReportePrevired, ReportePrevired pReporteNormal
            , MovimientoPersonal pMovimiento, Persona person, double pDias13, double pDias14, double pDias15, IndiceMensual pIndice, Hashtable pData, double pMontoFam, double pPrestamos, double pAhorros, double pSegurosVida)
        {
            double TopeNormal = 0;

            //83-CODIGO CCAF
            string codCaja = GetCodigoCaja();
            AgregaCampo(codCaja == "00" ? "0" : codCaja);


            //84-RENTA IMPONIBLE CCAF (SIEMPRE Y CUANDO ESTE AFILIADO A CAJA)
            if (codCaja != "00")
            {
                if (pMovimiento != null)
                {
                    imponible = Convert.ToDouble(pData["ImponibleCajaSuspension"]);
                    AgregaCampo(Math.Round(imponible, 0, MidpointRounding.AwayFromZero).ToString());
                }
                else
                {
                    AgregaCampo(Math.Round(imponible, 0, MidpointRounding.AwayFromZero).ToString());
                }
            }
            else
                AgregaCampo("0");

            //85-CREDITOS PERSONALES CCAF
            if (codCaja != "00")
                AgregaCampo(pPrestamos.ToString());
            else
            {
                AgregaCampo("0");
            }
            //86-DESCUENTOS DENTAL CCAF
            AgregaCampo("0");
            //87-DESCUENTO POR LEASING
            if (codCaja != "00")
                AgregaCampo(pAhorros.ToString());
            else
            {
                AgregaCampo("0");
            }
            //88-DESCUENTOS POR SEGURO DE VIDA
            if (codCaja != "00")
                AgregaCampo(pSegurosVida.ToString());
            else
                AgregaCampo("0");
            //89-OTROS DESCUENTOS
            AgregaCampo("0");
            string d = cadenaCarga;
            //90-COTIZACION CCAG DE NO AFILIADOS A ISAPRES (SOLO APLICA SI COTIZA EN FONASA)
            //CORRESPONDE RENTA IMPONIBLE * 0.6% (RENTA IMPONIBLE DEL CAMPO 84)
            //0.6 corresponde a lo que se paga si esta afiliado a caja de compensacion
            if (codCaja != "00")
            {
                if (codSalud == "07")
                {
                    //Normal
                    pReporteNormal.Cotizacion = Math.Round(Convert.ToDouble(pData["CotizacionCaja"]));

                   //Suspension
                    pReportePrevired.Cotizacion = Math.Round(Convert.ToDouble(pData["CotizacionCajaSuspension"]));
                    //Agregamos la suma de los dos
                    AgregaCampo(Math.Round(Convert.ToDouble(pData["CotizacionCaja"])) + Math.Round(Convert.ToDouble(pData["CotizacionCajaSuspension"])) + "");
                }
                else
                {
                    AgregaCampo("0");
                }
            }
            else
                AgregaCampo("0");

            //91-DESCUENTOS CARGAS FAMILIARES CCAF (SOLO SI ESTA EN CCAF)
            //AgregaCampo(varSistema.ObtenerValorLista("systfam").ToString());
            if (codCaja != "00")
                AgregaCampo(Math.Round(pMontoFam).ToString());
            else
                AgregaCampo("0");
            //92-OTROS DESCUENTOS CCAF 1
            AgregaCampo("0");
            //93-OTROS DESCUENTOS CCAF 2
            AgregaCampo("0");
            //94-BONOS GOBIERNO 
            AgregaCampo("0");
            //95-CODIGO DE SUCURSAL
            AgregaCampo("");
        }

        //*******************
        // DATOS MUTUALIDAD |
        //*******************        

        private void DatosMutualidadSuspension(double imponible, string pContrato, int pPeriodo, double pTopeAfp, ReportePrevired pReportePreviredNormal,
            ReportePrevired pReporteSuspendido,
            MovimientoPersonal pMovimiento, Persona person, double pDias13, double pDias14, double pDias15, IndiceMensual pIndice, Hashtable pData)
        {
            double imponibleFinal = 0, tope = 0, mutual = 0, sanna = 0, imponibleFinalNormal = 0;
            double mutualNormal = 0;
            DateTime Fecha = DateTime.Now.Date;
            Fecha = fnSistema.PrimerDiaMes(pPeriodo);
            int DiasMes = DateTime.DaysInMonth(Fecha.Year, Fecha.Month);
            //96-CODIGO MUTUALIDAD            
            AgregaCampo(GetCodigoMutual());
            //97-RENTA IMPONIBLE MUTUAL
            if (GetCodigoMutual() != "00")
            {
                imponibleFinalNormal = Math.Round(Convert.ToDouble(pData["ImponibleMutual"]));

                if (pMovimiento != null)
                {
                    imponibleFinal = Math.Round(Convert.ToDouble(pData["ImponibleMutualSuspension"]));              
                }

                AgregaCampo(Math.Round(imponibleFinal).ToString());
            }
            else { AgregaCampo("0"); }

            //98-COTIZACION ACCIDENTE DEL TRABAJO
            //SE CALCULA EN BASE A RENTA IMPONIBLE (CAMPO 97) * TASSA ASIGNADA A LA EMPRESA

            //Mutual normal
            mutualNormal = Convert.ToDouble(pData["CotizacionMutual"]);
            mutual = Convert.ToDouble(pData["CotizacionMutualSuspension"]);
            //mutualNormal = Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(pContrato, pPeriodo, "sysmutual"), 0, MidpointRounding.AwayFromZero) + Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(pContrato, pPeriodo, "sysvalsanna"), 0, MidpointRounding.AwayFromZero);
            pReportePreviredNormal.Cotizacion = mutualNormal;
            //Agregamos la suma de los dos
            AgregaCampo(Math.Round(mutual + mutualNormal) + "");
            pReporteSuspendido.Cotizacion = mutual;
            //99-CODIGO SUCURSAL PARA PAGO MUTUAL
            Empresa emp = new Empresa();
            emp.SetInfo();
            //AgregaCampo(GetSucursal(pContrato, pPeriodo));
            AgregaCampo(emp.CodEmpresa);
        }

        //*****************************
        // DATOS ADMINSTRADORA SEGURO  |  
        //*****************************
        private void DatosAdministradorSeguroSuspension(bool AdultoMayor, bool MenorEdad, double imponible, string pContrato, int pPeriodo, 
            Persona pPerson, ReportePrevired pReportePrevired,  ReportePrevired pReporteNormal,
            double pDias13, double pDias14, double pDias15, MovimientoPersonal pMovimiento, Hashtable pData)
        {
            double diasTrab = 0, Imp30 = 0;
            double ImpCompleto = 0, ImpCompletoNormal = 0;
            double AporteEmpresa = 0, AporteTrab = 0, AporteTrabNormal = 0, AporteEmpresaNormal = 0;
            //diasTrab = Labour.Calculo.GetValueFromCalculoMensaul(pContrato, pPeriodo, "sysdiastr");
            //100-RENTA IMPONIBLE SEGURO CESANTIA
            //AgregaCampo(Math.Round(GetImponibleSeguroCes(pContrato, pPeriodo), 0, MidpointRounding.AwayFromZero).ToString());            

            //SOLO PAGAMOS SEGURO DE CESANTIA SI APLICA
            //LAS PERSONAS JUBILADAS NO PAGAN SEGURO DE CESANTÍA
            //LOS MENORES DE 18 AÑOS
            DateTime Fecha = DateTime.Now.Date;
            Fecha = pPerson.Nacimiento.AddYears(18);
            //SI LA FECHA EN QUE CUMPLE LOS 18 AÑOS ES MAYOR AL ULTIMO DIA DE PERIODO EVALUADO, ES MENOR DE EDAD.
            DateTime Last = DateTime.Now.Date;
            Last = fnSistema.UltimoDiaMes(pPeriodo);

            //1 --> SI, NO COTIZA
            if (pPerson.Jubilado == 1 || (Fecha > Last))
            {
                //100
                AgregaCampo("0");
                //101
                AgregaCampo("0");
                //102
                AgregaCampo("0");
            }
            else
            {
                //100 - RENTA IMPONIBLE SEGURO CESANTÍA
                //SI DIAS TRABAJADOS ES IGUAL A 0, NO TRABAJÓ EN EL MES
                //EN ESTE CASO SE DEBE COLOCAR MES IMPONIBLE MES COMPLETO        
                //ImpCompleto = GetImponibleSeguroCes(pContrato, pPeriodo);
                //PARA DIAS NORMALES
                ImpCompletoNormal = Math.Round(Convert.ToDouble(pData["ImponibleSeguroCesantia"]));

                //ImpCompleto = Math.Round(Labour.Calculo.GetValueFromLiquidacionHistorica(pPerson.Contrato, fnSistema.fnObtenerPeriodoAnterior(pPerson.PeriodoPersona), "imponible"), 0);
                ImpCompleto = Math.Round(Convert.ToDouble(pData["ImponibleSeguroCesantiaSuspension"]));

                AgregaCampo(Math.Round(ImpCompleto, MidpointRounding.AwayFromZero).ToString());
                //101-APORTE TRABAJADOR SEGURO CESANTIA       

                //Aporte normal no suspension
                //AporteTrabNormal = Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(pContrato, pPeriodo, "syscicest"));
                AporteTrab = Math.Round(Convert.ToDouble(pData["SeguroTrabajadorSuspension"]));
                AporteTrabNormal = Math.Round(Convert.ToDouble(pData["SeguroCesantiaTrabajador"]));

                AgregaCampo(Math.Round(AporteTrab) + AporteTrabNormal + "");
                pReportePrevired.SegAfiliado = AporteTrab;
                pReporteNormal.SegAfiliado = AporteTrabNormal;

                //102-APORTE EMPLEADOR SEGURO CESANTIA

                //Aporte normal, no suspension
                AporteEmpresa = Math.Round(Convert.ToDouble(pData["SeguroEmpresaSuspension"]));
                AporteEmpresaNormal = Math.Round(Convert.ToDouble(pData["SeguroCesantiaEmpresa"]));
                //AporteEmpresaNormal = Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(pContrato, pPeriodo, "sysfscese") + Labour.Calculo.GetValueFromCalculoMensaul(pContrato, pPeriodo, "syscicese"));

                AgregaCampo(Math.Round(AporteEmpresa) + Math.Round(AporteEmpresaNormal) + "");
                pReportePrevired.SegEmpresa = AporteEmpresa;
                pReporteNormal.SegEmpresa = AporteEmpresaNormal;
            }

        }

        //************************
        // DATOS PAGADOR SUBSIDIO|
        //************************
        private void DatosPagadorSubsidioSuspension(MovimientoPersonal pMovimiento, string pContrato)
        {
            //103-RUT PAGADORA SUBSIDIOS
            AgregaCampo("0");

            //104-DV PAGADORA SUBSIDIO
            AgregaCampo(" ");
            
        }

        /// <summary>
        /// Datos leyes sociales para los dias normales de trabajo
        /// </summary>
        /// <param name="pTrabajador">Clase persona, representa al trabajador</param>
        /// <param name="pRegimenPrevisional">Regimen previsional que tiene la persona</param>
        /// <param name="pMontoFonasaOriginal">Monto de cotizacion original de fonasa</param>
        /// <param name="pTopeSalud">Tope cotizacion fonasa</param>
        /// <param name="pDiasLic">Dias de licencia que tuvo la persona en el mes</param>
        /// <param name="pTopeAfp">Tope cotizacion para afp</param>
        /// <param name="pIndice">Objeto con datos de los indices del mes (Ut, utm, etc)</param>
        /// <param name="pCodigoSalud">Indica que codigo de salud tiene (Ej; 07 fonasa)</param>
        /// <returns></returns>
        private Hashtable DatosSuspension(Persona pTrabajador, string pRegimenPrevisional, 
            double pMontoFonasaOriginal, double pTopeSalud, double pDiasLic, double pTopeAfp, 
            IndiceMensual pIndice, string pCodigoSalud)
        {
            
            double CotizacionAfp = 0, SeguroInvalidez = 0, AhorroAfp = 0, AhorroVoluntarioInd = 0;
            double ImponibleIps = 0, ImponibleOriginal = 0, CotizacionIps = 0, CotizacionFonasa = 0;
            double CotizacionFonasaIps = 0, CotizacionIsapre = 0, ImponibleIsapre = 0;
            double AdicionalIsapre = 0, CotizacionIsaprePactada = 0, ImponibleCcAf = 0;
            double DctoFamiliares = 0, ImponibleMutual = 0, CotizacionMutual = 0;
            double ImponibleSeguroCesantia = 0, SeguroAporteEmpresa = 0, SeguroAporteTrabajador = 0;
            DateTime Fecha = DateTime.Now.Date;
            Fecha = fnSistema.PrimerDiaMes(pTrabajador.PeriodoPersona);
            int DiasMes = DateTime.DaysInMonth(Fecha.Year, Fecha.Month);
            //Corresponde a la cotizacion minima (7% de salud)
            double SaludMinimaIsapre = 0, CotizacionCaja = 0;
            Fecha = DateTime.Now.Date;
            Fecha = pTrabajador.Nacimiento.AddYears(18);
            //SI LA FECHA EN QUE CUMPLE LOS 18 AÑOS ES MAYOR AL ULTIMO DIA DE PERIODO EVALUADO, ES MENOR DE EDAD.
            DateTime Last = DateTime.Now.Date;
            Last = fnSistema.UltimoDiaMes(pTrabajador.PeriodoPersona);
            //Codigo afp, para saber si cotiza en afp
            string codAfp = GetCodigoAfp(pTrabajador.codAfp);

            //Codigo caja de compension
            string CodigoCaja = GetCodigoCaja();
            //Codigo de Mutual
            string CodigoMutual = GetCodigoMutual();

            //Variables para montos relacionados con suspension
            double FonasaSuspension13 = 0, FonasaSuspension14 = 0, AfpSuspension13 = 0, AfpSuspension14 = 0;
            double SeguroInvalidezSuspension13 = 0, SeguroInvalidezSuspension14 = 0;
            double AfpSuspensionIps = 0, FonasaIpsSuspension = 0, FonasaNormalSuspension13 = 0, FonasaNormalSuspension14 = 0;
            double ImponibleAfpSuspension = 0, ImponibleIpsSuspension = 0, ImponibleIsapreSuspension = 0;
            double SaludIsapreSuspensionMinimo = 0, CotizacionIsapreSuspension = 0, AdicionalIsapreSuspension = 0;
            double ImponibleCajaSuspension = 0, CotizacionCajaSuspension = 0, ImponibleMutualSuspension = 0;
            double CotizacionMutualSuspension = 0, ImponibleSuspensionSeguro = 0;
            double SeguroEmpresaSuspension = 0, SeguroTrabajadorSuspension = 0;

            //Para guardar informacion
            Hashtable Data = new Hashtable();            

            IndiceMensual I = new IndiceMensual();
            IndiceMensual IActual = new IndiceMensual();

            //Imponible que tuvo la persona en el mes
            ImponibleOriginal = Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(pTrabajador.Contrato, pTrabajador.PeriodoPersona, "systimp"));
            Data.Add("ImponibleOriginal", ImponibleOriginal);
            IActual.SetInfoMes(pTrabajador.PeriodoPersona);

            //Solo para tope ips (se considera el del mes anterior)     
            I.SetInfoMes(fnSistema.fnObtenerPeriodoAnterior(Labour.Calculo.PeriodoObservado));

            //Datos de caja regimen antiguo
            CajaPrevision cajaRegimen = new CajaPrevision();
            cajaRegimen.SetInfo(pTrabajador.Contrato, pTrabajador.PeriodoPersona);

            //RentaImponibleIps
            if (pCodigoSalud == "07")
            {
                ImponibleIps = GetMontoIps(periodo, ImponibleOriginal, pRegimenPrevisional);
                if (ImponibleIps > I.TopeIpsPesos)
                    ImponibleIps = Math.Round(I.TopeIpsPesos);

                Data.Add("ImponibleIps", ImponibleIps);
            }
            else
            {
                Data.Add("ImponibleIps", 0);
            }
           

            //@1 - MONTOS AFP
            if (codAfp != "SIP" && pTrabajador.Regimen == 1)
            {
                CotizacionAfp = Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(pTrabajador.Contrato, pTrabajador.PeriodoPersona, "sysciafp")) + Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(pTrabajador.Contrato, pTrabajador.PeriodoPersona, "syscomafp"));
                Data.Add("CotizacionAfp", CotizacionAfp);
            }
            else
            {
                Data.Add("CotizacionAfp", 0);
            }             
                

            //@2- - SEGURO DE INVALIDEZ
            SeguroInvalidez = Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(pTrabajador.Contrato, pTrabajador.PeriodoPersona, "syssis"));
            Data.Add("SeguroInvalidez", SeguroInvalidez);

            //@3 - Ahorro Afp
            AhorroAfp =  ItemTrabajador.GetValue("afpaho", pTrabajador.Contrato, pTrabajador.PeriodoPersona);
            Data.Add("AhorroAfp", AhorroAfp);

            //@4 - Ahorro Voluntario individual
            AhorroVoluntarioInd = ItemTrabajador.GetValue("aprevol", pTrabajador.Contrato, pTrabajador.PeriodoPersona);
            Data.Add("AhorroVoluntarioIndividual", AhorroVoluntarioInd);

            //@5 - Cotizacion Ips (Regimen antiguo)
            if (pTrabajador.Regimen == 4 || pTrabajador.Regimen == 5)
            {
                if (cajaRegimen.id != 0)
                {
                    CotizacionIps = Math.Round(cajaRegimen.porcPension * ImponibleIps);
                    Data.Add("CotizacionIps", CotizacionIps);
                }
                else
                {
                    Data.Add("CotizacionIps", 0);
                }
            }
            else
            {
                Data.Add("CotizacionIps", 0);
            }


            //@6 - Cortizacion Fonasa
            //Si esta en caja se calculo el 6.4%, sino se calcula el 7%
            //Calculamos el 7% (Cotizacion mínima) del imponible ips
            if (CodigoCaja != "00")
            {
                if (pCodigoSalud == "07")
                {
                    //Cotizacion Fonana regimen antiguo
                    if (pTrabajador.Regimen == 4 || pTrabajador.Regimen == 5)
                    {
                        //@7 - Cotizacion Fonasa Ips
                        CotizacionFonasaIps = Math.Round(ImponibleIps * 0.064);
                        Data.Add("CotizacionFonasaIps", CotizacionFonasaIps);
                        Data.Add("CotizacionFonasaNormal", 0);
                    }
                    else
                    {
                        if (pMontoFonasaOriginal > 0.07 * ImponibleIps)
                            CotizacionFonasa = Math.Round(0.064 * ImponibleIps);
                        else
                            CotizacionFonasa = Math.Round(((0.064) * pMontoFonasaOriginal) / (0.07));

                        Data.Add("CotizacionFonasaNormal", CotizacionFonasa);
                        Data.Add("CotizacionFonasaIps", 0);
                    }
                }
                else
                {
                    //no hay cotizacion
                    Data.Add("CotizacionFonasaNormal", 0);
                    Data.Add("CotizacionFonasaIps", 0);
                }

            }
            else
            {
                if (pCodigoSalud == "07")
                {
                    if (pTrabajador.Regimen == 4 || pTrabajador.Regimen == 5)
                    {
                        CotizacionFonasaIps = Math.Round(ImponibleIps * cajaRegimen.porcSalud);
                        Data.Add("CotizacionFonasaIps", CotizacionFonasaIps);
                        Data.Add("CotizacionFonasaNormal", 0);
                    }
                    else
                    {
                        CotizacionFonasa = pMontoFonasaOriginal;
                        Data.Add("CotizacionFonasaNormal", CotizacionFonasa);
                        Data.Add("CotizacionFonasaIps", 0);
                    }
                }
                else
                {
                    Data.Add("CotizacionFonasaNormal", 0);
                    Data.Add("CotizacionFonasaIps", 0);
                }
            }

            //@8 - Cotizacion Isapre
            if (pCodigoSalud != "00" && pCodigoSalud != "07")
            {
                CotizacionIsapre = Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(pTrabajador.Contrato, pTrabajador.PeriodoPersona, "sysisapre"));
                Data.Add("CotizacionIsapre", CotizacionIsapre);
            }
            else
            {
                Data.Add("CotizacionIsapre", 0);
            }

            //Imponible para isapre (Considerando tope de salud)
            if (pCodigoSalud != "00" && pCodigoSalud != "07")
            {
                if (ImponibleOriginal > pTopeSalud)
                {
                    ImponibleIsapre = pTopeSalud;
                    SaludMinimaIsapre = Math.Round(0.07 * pTopeSalud, 0, MidpointRounding.AwayFromZero);
                }
                else
                {
                    ImponibleIsapre = ImponibleOriginal;
                    SaludMinimaIsapre = Math.Round(0.07 * ImponibleOriginal, 0, MidpointRounding.AwayFromZero);
                }
                Data.Add("SaludMinimaIsapre", SaludMinimaIsapre);
                Data.Add("ImponibleIsapre", ImponibleIsapre);
            }
            else
            {
                Data.Add("ImponibleIsapre", 0);
                Data.Add("SaludMinimaIsapre", 0);
            }

            //Adicional que se paga por isapre (si paga mas del 7% de salud)
            if(pCodigoSalud != "00" && pCodigoSalud != "07")
            {
                if (CotizacionIsapre > SaludMinimaIsapre)
                    AdicionalIsapre = CotizacionIsapre - SaludMinimaIsapre;
                else
                    AdicionalIsapre = 0;

                Data.Add("AdicionalIsapre", AdicionalIsapre);
            }
            else
            {
                Data.Add("AdicionalIsapre", AdicionalIsapre);
            }


            //Cotizacion pactada de isapre 
            //Si tuvo licencias en el mes consideramos la misma cotizacion de isapre
            //Contrario obtenemos la cotizacion pactada (Monto en uf por ejemplo)}
            if (pCodigoSalud != "00" && pCodigoSalud != "07")
            {
                if (pDiasLic != 0)
                    CotizacionIsaprePactada = CotizacionIsapre;
                else
                    CotizacionIsaprePactada = Math.Round(Convert.ToDouble(GetCotizacionPactada(pTrabajador.Contrato, pTrabajador.PeriodoPersona)));

                Data.Add("CotizacionIsaprePactada", CotizacionIsaprePactada);
            }
            else
            {
                Data.Add("CotizacionIsaprePactada", 0);
            }


            //Cotizacion obligatoria corresponde a SaludMinimaIsapre

            //@9 - Cotizacion de no afiliados a isapre
            //Si es regimen antiguo debemos considerar el tope ips
            if (CodigoCaja != "00")
            {
                if (pTrabajador.codCajaPrev == 0)
                {
                    if (ImponibleOriginal > pTopeAfp)
                        ImponibleCcAf = Math.Round(pTopeAfp);
                    else
                        ImponibleCcAf = Math.Round(ImponibleOriginal);
                }
                else
                {
                    if (ImponibleOriginal > pIndice.TopeIpsPesos)
                        ImponibleCcAf = Math.Round(pIndice.TopeIpsPesos);
                    else
                        ImponibleCcAf = Math.Round(ImponibleOriginal);

                }

                Data.Add("ImponibleCaja", ImponibleCcAf);
            }
            else
            {
                Data.Add("ImponibleCaja", 0);
            }


            //@10 - Cotizacion CCAF (7% - 6.4%) SOLO SI COTIZA EN FONASA
            //Siempre y cuando esté asociado a caja
            if (CodigoCaja != "00")
            {
                if (pCodigoSalud == "07")
                {
                    CotizacionCaja = Math.Round(0.006 * ImponibleCcAf);
                    Data.Add("CotizacionCaja", CotizacionCaja);
                }
                else
                {
                    Data.Add("CotizacionCaja", 0);
                }
               
            }
            else
            {
                Data.Add("CotizacionCaja", 0);
            }

            //@11 - Descuentos cargas familiares, solo si está asociado a caja
            if (CodigoCaja != "00")
            {
                DctoFamiliares = Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(Contrato, Periodo, "systfam"));
                Data.Add("DescuentosFamiliaresCaja", DctoFamiliares);
            }
            else
            {
                Data.Add("DescuentosFamiliaresCaja", 0);
            }


            //@12 - Imponible Mutual y cotizacion sanna + mutual
            //Siempre y cuando esté asociado a mutual

            if (CodigoMutual != "00")
            {
                if (pDiasLic == DiasMes)
                {
                    Haberes hab = new Haberes(pTrabajador.Contrato, pTrabajador.PeriodoPersona);

                    //SI EXISTE SOLO LO CONSULTAMOS
                    if (Labour.Calculo.ExisteImpAnterior(pTrabajador.Contrato))
                        ImponibleMutual = Labour.Calculo.ConsultaImpAnterior(pTrabajador.Contrato);
                    else
                        //SI NO EXISTE DEBEMOS CALCULARLO
                        ImponibleMutual = hab.ImponibleAnteriorGenerico(hab.getContrato(), hab.getPeriodo());

                    //SI LOS DIAS DE LICENCIA SON 31 DEBEMOS ADICIONAL AL IMPONIBLE EL DIA 31
                    ImponibleMutual = (ImponibleMutual * 31) / 30;

                    //FINALMENTE VERIFICAMOS QUE NO SUPERE EL TOPE LEAGL
                    if (ImponibleMutual > pTopeAfp)
                        ImponibleMutual = pTopeAfp;

                    Data.Add("ImponibleMutual", ImponibleMutual);

                }
                else
                {
                    //Dias proporcionales
                    //Tuvo dias trabajados
                    //SOLO USAMOS EL IMPONIBLE GENERADO EN EL MES
                    if (pTrabajador.codCajaPrev == 0)
                    {
                        if (ImponibleOriginal > pTopeAfp)
                            ImponibleMutual = pTopeAfp;
                        else
                            ImponibleMutual = ImponibleOriginal;
                    }
                    else
                    {

                        if (ImponibleOriginal > pIndice.TopeIpsPesos)
                            ImponibleMutual = Math.Round(pIndice.TopeIpsPesos);
                        else
                            ImponibleMutual = Math.Round(ImponibleOriginal);
                    }


                    Data.Add("ImponibleMutual", ImponibleMutual);

                }
            }
            else
            {
                Data.Add("ImponibleMutual", 0);
            }

            //Cotizacion mutual + cotizacion sanna
            if (CodigoMutual != "00")
            {
                CotizacionMutual = Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(pTrabajador.Contrato, pTrabajador.PeriodoPersona, "sysmutual"), 0, MidpointRounding.AwayFromZero) + Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(pTrabajador.Contrato, pTrabajador.PeriodoPersona, "sysvalsanna"), 0, MidpointRounding.AwayFromZero);
                Data.Add("CotizacionMutual", CotizacionMutual);
            }
            else
            {
                Data.Add("CotizacionMutual", 0);
            }            


            //@13 - Imponible Seguro Cesantia 
            //SOLO PAGAMOS SEGURO DE CESANTIA SI APLICA
            //LAS PERSONAS JUBILADAS NO PAGAN SEGURO DE CESANTÍA
            //LOS MENORES DE 18 AÑOS
            //1 --> SI, NO COTIZA
            if (pTrabajador.Jubilado == 1 || (Fecha > Last))
            {
                //No se calcula seguro (lo dejamos en cero)
                Data.Add("ImponibleSeguroCesantia", 0);
                Data.Add("SeguroCesantiaTrabajador", 0);
                Data.Add("SeguroCesantiaEmpresa", 0);
            }
            else
            {
                //100 - RENTA IMPONIBLE SEGURO CESANTÍA
                //SI DIAS TRABAJADOS ES IGUAL A 0, NO TRABAJÓ EN EL MES
                //EN ESTE CASO SE DEBE COLOCAR MES IMPONIBLE MES COMPLETO        
                
                ImponibleSeguroCesantia = GetImponibleSeguroCes(pTrabajador.Contrato, pTrabajador.PeriodoPersona);
                Data.Add("ImponibleSeguroCesantia", ImponibleSeguroCesantia);

                //101-APORTE TRABAJADOR SEGURO CESANTIA        
                SeguroAporteTrabajador = Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(pTrabajador.Contrato, pTrabajador.PeriodoPersona, "syscicest"));
                Data.Add("SeguroCesantiaTrabajador", SeguroAporteTrabajador);

                //102-APORTE EMPLEADOR SEGURO CESANTIA
                SeguroAporteEmpresa = Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(pTrabajador.Contrato, pTrabajador.PeriodoPersona, "sysfscese") + Labour.Calculo.GetValueFromCalculoMensaul(pTrabajador.Contrato, pTrabajador.PeriodoPersona, "syscicese"));
                Data.Add("SeguroCesantiaEmpresa", SeguroAporteEmpresa);

            }

            //////////////////////////////////////////
            /////   DATOS SUSPENSION
            //////////////////////////////////////////

            //Imponible Suspension Afp
            if (codAfp != "SIP" && pTrabajador.Regimen == 1)
            {
                
                ImponibleAfpSuspension = Labour.Calculo.GetValueAfc(pTrabajador.Rut, pTrabajador.PeriodoPersona);
                if (ImponibleAfpSuspension == -1)
                {
                    XtraMessageBox.Show($"No se encontró monto afc para persona rut --> {pTrabajador.Rut}\nNombre: {pTrabajador.NombreCompleto}", "Falta información", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return null;
                }
                //ImponibleAfpSuspension = Labour.Calculo.GetValueFromLiquidacionHistorica(pTrabajador.Contrato, fnSistema.fnObtenerPeriodoAnterior(pTrabajador.PeriodoPersona), "imponible");

                if (ImponibleAfpSuspension == 0)
                    ImponibleAfpSuspension = Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(pTrabajador.Contrato, pTrabajador.PeriodoPersona, "systimp30"));
                else
                {
                    ImponibleAfpSuspension = Math.Round((Configuracion.ConfiguracionGlobal.PorcentajeSuspension * ImponibleAfpSuspension) / 100, 0);
                }

                if (ImponibleAfpSuspension > pTopeAfp)
                    ImponibleAfpSuspension = pTopeAfp;

                Data.Add("ImponibleAfpSuspension", ImponibleAfpSuspension);
            }
            else
            {
                Data.Add("ImponibleApfSuspension", 0);
            }

            //Datos Para Suspension
            FonasaSuspension13 = Labour.Calculo.GetValueFromCalculoMensaul(pTrabajador.Contrato, pTrabajador.PeriodoPersona, "syssaludsp13");
            FonasaSuspension14 = Labour.Calculo.GetValueFromCalculoMensaul(pTrabajador.Contrato, pTrabajador.PeriodoPersona, "syssaludsp14");
            Data.Add("FonasaSuspension13", FonasaSuspension13);
            Data.Add("FonasaSuspension14", FonasaSuspension14);

            //Afp Suspesion
            AfpSuspension13 = Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(pTrabajador.Contrato, pTrabajador.PeriodoPersona, "sysafpsp13"));
            AfpSuspension14 = Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(pTrabajador.Contrato, pTrabajador.PeriodoPersona, "sysafpsp14"));
            Data.Add("AfpSuspension13", AfpSuspension13);
            Data.Add("AfpSuspension14", AfpSuspension14);

            //Seguro Invalidez
            SeguroInvalidezSuspension13 = Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(pTrabajador.Contrato, pTrabajador.PeriodoPersona, "sysinv13"));
            SeguroInvalidezSuspension14 = Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(pTrabajador.Contrato, pTrabajador.PeriodoPersona, "sysinv14"));
            Data.Add("SeguroInvalidezSuspension13", SeguroInvalidezSuspension13);
            Data.Add("SeguroInvalidezSuspension14", SeguroInvalidezSuspension14);

            //Imponible IPS
            if (pCodigoSalud == "07")
            {
                //ImponibleIpsSuspension = Labour.Calculo.GetValueAfc(pTrabajador.Rut, pTrabajador.PeriodoPersona);
                ImponibleIpsSuspension = Labour.Calculo.GetImponibleSuspension(pTrabajador.Contrato);
                //ImponibleIpsSuspension = Labour.Calculo.GetValueFromLiquidacionHistorica(pTrabajador.Contrato, fnSistema.fnObtenerPeriodoAnterior(pTrabajador.PeriodoPersona), "imponible"); ;
                if (ImponibleIpsSuspension == 0)
                    ImponibleIpsSuspension = Labour.Calculo.GetValueFromCalculoMensaul(pTrabajador.Contrato, pTrabajador.PeriodoPersona, "systimp30");
               

                if (pTrabajador.Regimen == 4 || pTrabajador.Regimen == 5)
                {
                    //Tope va a ser el tope para ips
                    //Si es regimen antiguo usamos este imponible
                    ImponibleIpsSuspension = Labour.Calculo.GetValueAfc(pTrabajador.Rut, pTrabajador.PeriodoPersona);
                    if (ImponibleIpsSuspension == -1)
                    {
                        XtraMessageBox.Show($"No se encontró monto afc para persona rut --> {pTrabajador.Rut}\nNombre: {pTrabajador.NombreCompleto}", "Falta información", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return null;
                    }

                    if (ImponibleIpsSuspension > IActual.TopeIpsPesos)
                        ImponibleIpsSuspension = Math.Round(IActual.TopeIpsPesos);
                }
                else
                {
                    if (ImponibleIpsSuspension > IActual.TopeAfpPesos)
                        ImponibleIpsSuspension = Math.Round(IActual.TopeAfpPesos);
                }
                

                ImponibleIpsSuspension = Math.Round((Configuracion.ConfiguracionGlobal.PorcentajeSuspension * ImponibleIpsSuspension) / 100, 0);

                Data.Add("ImponibleIpsSuspension", ImponibleIpsSuspension);
            }
            else
            {
                Data.Add("ImponibleIpsSuspension", 0);
            }

            //Cotizacion ips suspension
            if (pTrabajador.Regimen == 4 || pTrabajador.Regimen == 5)
            {
                AfpSuspensionIps = Math.Round(cajaRegimen.porcPension * ImponibleIpsSuspension);
                Data.Add("AfpSuspensionIps", AfpSuspensionIps);
            }
            else
            {
                Data.Add("AfpSuspensionIps", 0);
            }


            //Cotizacion fonasa 6.4% si está en caja de compensacion
            if (CodigoCaja != "00")
            {
                if (pCodigoSalud == "07")
                {
                    if (pTrabajador.Regimen == 4 || pTrabajador.Regimen == 5)
                    {
                        FonasaIpsSuspension = Math.Round(ImponibleIpsSuspension * 0.064);
                        Data.Add("FonasaIpsSuspension", FonasaIpsSuspension);
                        Data.Add("FonasaNormalSuspension", 0);
                    }
                    else
                    {
                        if (FonasaSuspension13 > (0.07 * ImponibleIpsSuspension))
                            FonasaNormalSuspension13 = Math.Round(0.064 * (ImponibleIpsSuspension));
                        else
                            FonasaNormalSuspension13 = Math.Round(((0.064) * FonasaSuspension13) / (0.07));

                        if (FonasaSuspension14 > (0.07 * ImponibleIpsSuspension))
                            FonasaNormalSuspension14 = Math.Round(0.064 * (ImponibleIpsSuspension));
                        else
                            FonasaNormalSuspension14 = Math.Round(((0.064) * FonasaSuspension14) / (0.07));

                        Data.Add("FonasaNormalSuspension", Math.Round(FonasaNormalSuspension13 + FonasaNormalSuspension14));
                        Data.Add("FonasaIpsSuspension", 0);
                    }
                }
            }
            else
            {
                if (pCodigoSalud == "07")
                {
                    //Regimen antiguo
                    if (pTrabajador.Regimen == 4 || pTrabajador.Regimen == 5)
                    {
                        FonasaIpsSuspension = Math.Round(ImponibleIpsSuspension * cajaRegimen.porcSalud);
                        Data.Add("FonasaIpsSuspension", FonasaIpsSuspension);
                        Data.Add("FonasaNormalSuspension", 0);
                    }
                    else
                    {
                        FonasaNormalSuspension13 = Math.Round(FonasaSuspension13) + Math.Round(FonasaSuspension14);
                        Data.Add("FonasaNormalSuspension", FonasaNormalSuspension13);
                        Data.Add("FonasaIpsSuspension", 0);
                    }
                }
                else
                {
                    Data.Add("FonasaIpsSuspension", 0);
                    Data.Add("FonasaNormalSuspension", 0);
                }
            }

            //Imponible Isapre suspension mas cotizaciones si esta en isapre
            if (pCodigoSalud != "00" && pCodigoSalud != "07")
            {
                CotizacionIsapreSuspension = Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(pTrabajador.Contrato, pTrabajador.PeriodoPersona, "syssaludsp13")) + Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(pTrabajador.Contrato, pTrabajador.PeriodoPersona, "syssaludsp14"));

                ImponibleIsapreSuspension = Labour.Calculo.GetImponibleSuspension(pTrabajador.Contrato);
                //ImponibleIsapreSuspension = Labour.Calculo.GetValueFromLiquidacionHistorica(pTrabajador.Contrato, fnSistema.fnObtenerPeriodoAnterior(pTrabajador.PeriodoPersona), "imponible");
                if (ImponibleIsapreSuspension == 0)
                    ImponibleIsapreSuspension = Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(pTrabajador.Contrato, pTrabajador.PeriodoPersona, "systimp30"));

                if (ImponibleIsapreSuspension > pTopeSalud)
                    ImponibleIsapreSuspension = Math.Round(pTopeSalud);

                //imponible = Math.Round(imponible / 2);

                //imponible = Math.Round((imponible * pMovimiento.Dias) / 15);
                ImponibleIsapreSuspension = Math.Round((Configuracion.ConfiguracionGlobal.PorcentajeSuspension * ImponibleIsapreSuspension) / 100);


                SaludIsapreSuspensionMinimo = Math.Round(0.07 * ImponibleIsapreSuspension, 0, MidpointRounding.AwayFromZero);

                //VALOR DE PAGO ADICIONAL
                if (CotizacionIsapreSuspension > SaludIsapreSuspensionMinimo)
                    AdicionalIsapreSuspension = CotizacionIsapreSuspension - SaludIsapreSuspensionMinimo;              
                else
                    AdicionalIsapreSuspension = 0;

                Data.Add("CotizacionIsapreSuspension", CotizacionIsapreSuspension);
                Data.Add("ImponibleIsapreSuspension", ImponibleIsapreSuspension);
                //Corresponde al 7% minimo de cotizacion
                Data.Add("SaludIsapreSuspensionMinimo", SaludIsapreSuspensionMinimo);
                Data.Add("AdicionalIsapreSuspension", AdicionalIsapreSuspension);
            }
            else
            {
                Data.Add("CotizacionIsapreSuspension", 0);
                Data.Add("ImponibleIsapreSuspension", 0);
                //Corresponde al 7% minimo de cotizacion
                Data.Add("SaludIsapreSuspensionMinimo", 0);
                Data.Add("AdicionalIsapreSuspension", 0);
            }

            //Imponible caja de compensacion para suspension
            if (CodigoCaja != "00")
            {
                ImponibleCajaSuspension = Labour.Calculo.GetImponibleSuspension(pTrabajador.Contrato);
                //ImponibleCajaSuspension = Labour.Calculo.GetValueFromLiquidacionHistorica(pTrabajador.Contrato, fnSistema.fnObtenerPeriodoAnterior(pTrabajador.PeriodoPersona), "imponible");
                if (ImponibleCajaSuspension == 0)
                    ImponibleCajaSuspension = Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(pTrabajador.Contrato, pTrabajador.PeriodoPersona, "systimp30"));

                //Si es regimen antiguo deberia ser el imponible de ips

                //NO es regimen antiguo
                if (pTrabajador.codCajaPrev == 0)
                {
                    if (ImponibleCajaSuspension > pTopeAfp)
                        ImponibleCajaSuspension = Math.Round(pTopeAfp);
                }
                else
                {

                    //REGimen antiguo
                    if (ImponibleCajaSuspension > pIndice.TopeIpsPesos)
                        ImponibleCajaSuspension = Math.Round(pIndice.TopeIpsPesos);
                }

                ImponibleCajaSuspension = Math.Round((Configuracion.ConfiguracionGlobal.PorcentajeSuspension * ImponibleCajaSuspension) / 100);

                Data.Add("ImponibleCajaSuspension", ImponibleCajaSuspension);
            }
            else
            {
                Data.Add("ImponibleCajaSuspension", 0);
            }

            //Cotizacion caja de compensacion
            if (CodigoCaja != "00")
            {
                if (pCodigoSalud == "07")
                {
                    CotizacionCajaSuspension = Math.Round(0.006 * ImponibleCajaSuspension);
                    Data.Add("CotizacionCajaSuspension", CotizacionCajaSuspension);
                }
                else
                {
                    Data.Add("CotizacionCajaSuspension", 0);
                }
            }
            else
            {
                Data.Add("CotizacionCajaSuspension", 0);
            }


            //RentaImponible para mutual
            if (CodigoMutual != "00")
            {
                ImponibleMutualSuspension = Labour.Calculo.GetImponibleSuspension(pTrabajador.Contrato);
                //ImponibleMutualSuspension = Math.Round(Labour.Calculo.GetValueFromLiquidacionHistorica(pTrabajador.Contrato, fnSistema.fnObtenerPeriodoAnterior(pTrabajador.PeriodoPersona), "imponible"), 0);
                if (ImponibleMutualSuspension == 0)
                    ImponibleMutualSuspension = Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(pTrabajador.Contrato, pTrabajador.PeriodoPersona, "systimp30"));

                if (ImponibleMutualSuspension > pTopeAfp)
                    ImponibleMutualSuspension = Math.Round(pTopeAfp);

                ImponibleMutualSuspension = Math.Round((Configuracion.ConfiguracionGlobal.PorcentajeSuspension * ImponibleMutualSuspension) / 100, 0);

                Data.Add("ImponibleMutualSuspension", ImponibleMutualSuspension);
            }
            else
            {
                Data.Add("ImponibleMutualSuspension", 0);
            }

            //CotizacionMutual suspension
            CotizacionMutualSuspension = Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(pTrabajador.Contrato, pTrabajador.PeriodoPersona, "syssanna13"), 0, MidpointRounding.AwayFromZero) + Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(pTrabajador.Contrato, pTrabajador.PeriodoPersona, "syssanna14"), 0, MidpointRounding.AwayFromZero);
            Data.Add("CotizacionMutualSuspension", CotizacionMutualSuspension);

            //Imponible Seguro Cesantia
            if (pTrabajador.Jubilado == 1 || (Fecha > Last))
            {
                //No cotizacion
                Data.Add("ImponibleSeguroCesantiaSuspension", 0);
                Data.Add("SeguroTrabajadorSuspension", 0);
                Data.Add("SeguroEmpresaSuspension", 0);
            }
            else
            {
                ImponibleSuspensionSeguro = Labour.Calculo.GetImponibleSuspension(pTrabajador.Contrato);
                //ImponibleSuspensionSeguro = Math.Round(Labour.Calculo.GetValueFromLiquidacionHistorica(pTrabajador.Contrato, fnSistema.fnObtenerPeriodoAnterior(pTrabajador.PeriodoPersona), "imponible"), 0);
                if (ImponibleSuspensionSeguro == 0)
                    ImponibleSuspensionSeguro = Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(pTrabajador.Contrato, pTrabajador.PeriodoPersona, "systimp30"));

                //ImpCompleto = (ImpCompleto / 30) * pMovimiento.Dias;      
                ImponibleSuspensionSeguro = Math.Round((Configuracion.ConfiguracionGlobal.PorcentajeSuspension * ImponibleSuspensionSeguro) / 100);

                //AporteTrabajador
                SeguroTrabajadorSuspension = Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(pTrabajador.Contrato, pTrabajador.PeriodoPersona, "syssegtrabsp13")) + Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(pTrabajador.Contrato, pTrabajador.PeriodoPersona, "syssegtrabsp14"));

                //Aporte Empresa
                SeguroEmpresaSuspension = Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(pTrabajador.Contrato, pTrabajador.PeriodoPersona, "syssegemsp13")) + Math.Round(Labour.Calculo.GetValueFromCalculoMensaul(pTrabajador.Contrato, pTrabajador.PeriodoPersona, "syssegemsp14"));

                Data.Add("ImponibleSeguroCesantiaSuspension", ImponibleSuspensionSeguro);
                Data.Add("SeguroTrabajadorSuspension", SeguroTrabajadorSuspension);
                Data.Add("SeguroEmpresaSuspension", SeguroEmpresaSuspension);

            }


            return Data;
        }

        #endregion

        private void AgregaCampo(string pCampo)
        {
            cadenaCarga = cadenaCarga + pCampo + ";";
        }
    }

  
    class MovimientoPersonal
    {
        public string codMovimiento { get; set; }
        public DateTime inicioMovimiento { get; set; }
        public DateTime terminoMovimiento { get; set; }
        public int Dias { get; set; } = 0;
    }

    class Libro
    {
        public static DataTable TablaLibroRemuneraciones = new DataTable();
        public static List<Libro> ListHeaderRow = new List<Libro>();
        public static List<double> ListadoLiquidos = new List<double>();
        public static double TotalImponibleLibro = 0;
        public string ContratoHeader;
        public string NameHeader;
        public string DiasHeader;
        public int PeriodoHeader = 0;
        public string DiasLicHeader = "";

        //P0RUEBA TRAER DATOS
        public static DataTable pruebaTraerdatos(string pContrato, int pPeriodo)
        {

            DataTable tabla = new DataTable();
            string sql = "SELECT itemTrabajador.valorCalculado , libro.orden, libro. negrita, libro.cursiva, libro.alias " +
                "FROM itemTrabajador " +
                "INNER JOIN libro ON libro.coditem = itemTrabajador.coditem " +
                "WHERE libro.visible != 0 AND contrato = @contrato AND anomes = @periodo";
            SqlCommand sqlcmd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (sqlcmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        sqlcmd.Parameters.Add(new SqlParameter("@contrato", pContrato));
                        sqlcmd.Parameters.Add(new SqlParameter("@periodo", pPeriodo));
                        var datareader = sqlcmd.ExecuteReader();
                        tabla.Load(datareader);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

            return tabla;
        }

        

        //GENERA UN DATATABLE 
        public static DataTable GetTablaFromLibro()
        {
            DataTable tabla = new DataTable();
            string sql = "SELECT coditem FROM libro WHERE visible=1 ORDER BY orden";
            SqlCommand cmd;
            SqlDataReader rd;

            //tabla.Columns.Add("nombre", typeof(string));
            //tabla.Columns.Add("contrato", typeof(string));
           // tabla.Columns.Add("diastrabajados", typeof(double));

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //GENERAMOS FILA POR CADA REGISTRO
                                tabla.Columns.Add((string)rd["coditem"]);
                                
                                //tabla.Rows.Add(SetFila(Convert.ToInt32(rd["orden"]), (string)rd["coditem"], (bool)rd["visible"], (bool)rd["negrita"], (bool)rd["cursiva"], tabla));
                            }
                        }

                        cmd.Dispose();
                        rd.Close();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return tabla;
        }

        //GENERA UN DATATABLE 
        public static DataTable GetDatosFromLibro()
        {
            DataTable tabla = new DataTable();
            string sql = "SELECT orden, coditem FROM libro WHERE visible=1 ORDER BY orden";
            SqlCommand cmd;
            SqlDataReader rd;

            //tabla.Columns.Add("nombre", typeof(string));
            //tabla.Columns.Add("contrato", typeof(string));
            // tabla.Columns.Add("diastrabajados", typeof(double));

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {

                        var datareader = cmd.ExecuteReader();
                        tabla.Load(datareader);
                        //rd = cmd.ExecuteReader();
                        //if (rd.HasRows)
                        //{
                        //    while (rd.Read())
                        //    {
                        //        //GENERAMOS FILA POR CADA REGISTRO
                        //        tabla.Rows.Add(Convert.ToInt32(rd["orden"]), (string)rd["coditem"]);
                        //        //tabla.Columns.Add((string)rd["coditem"]);

                        //        //tabla.Rows.Add(SetFila(Convert.ToInt32(rd["orden"]), (string)rd["coditem"], (bool)rd["visible"], (bool)rd["negrita"], (bool)rd["cursiva"], tabla));
                        //    }
                        //}

                        cmd.Dispose();
                        //rd.Close();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return tabla;
        }

        //GENERA LA CANTIDAD DE CONTRATOS EN ESE PERIODO
        public static int GetNumeroContratos(string pPeriodo)
        {
            int numeroContratos = 0;
            string sql = "SELECT COUNT(contrato) " +
                            "FROM[dbo].[calculoMensual]" +
                            "WHERE anomes = @periodo";
            SqlCommand cmd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        cmd.Parameters.Add(new SqlParameter("@periodo", pPeriodo));
                        numeroContratos = int.Parse(cmd.ExecuteScalar().ToString());
                    }
                    cmd.Dispose();
                    //rd.Close();
                    fnSistema.sqlConn.Close();
                }
                
            }
            catch (Exception)
            {

                return 0;
            }
            return numeroContratos;
        }

        //VER SI UN ELEMENTO APLICA NEGRITA
        public static bool AplicaEstilo(string pParameter, string pItem)
        {
            string sql = $"SELECT {pParameter} FROM libro WHERE coditem='{pItem}'";
            SqlCommand cmd;
            bool data = false, aplica = false;

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        data = (bool)cmd.ExecuteScalar();
                        if (data)
                            aplica = true;

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return aplica;
        }

        //GET TYPE ITEM
        public static int GetTypeItem(string pItem)
        {            
            int tipo = 0;
            string sql = "SELECT tipo FROM item WHERE coditem=@pItem";
            SqlCommand cmd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pItem", pItem));

                        tipo = Convert.ToInt32(cmd.ExecuteScalar());

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
            return tipo;
        }

        //GENERA UN DATATABLE 
        public static DataTable GetColumnsTable()
        {
            DataTable tabla = new DataTable();
            string sql = "SELECT coditem FROM libro WHERE visible=1 ORDER BY orden";
            SqlCommand cmd;
            SqlDataReader rd;

            tabla.Columns.Add("contrato", typeof(string));
            //tabla.Columns.Add("contrato", typeof(string));
            // tabla.Columns.Add("diastrabajados", typeof(double));

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //GENERAMOS FILA POR CADA REGISTRO
                                tabla.Columns.Add((string)rd["coditem"]);
                                //tabla.Rows.Add(SetFila(Convert.ToInt32(rd["orden"]), (string)rd["coditem"], (bool)rd["visible"], (bool)rd["negrita"], (bool)rd["cursiva"], tabla));
                            }
                        }

                        cmd.Dispose();
                        rd.Close();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            //COLUMNAS TOTAL SYSPAGO
            tabla.Columns.Add("liqpago");

            return tabla;
        }

        //VER SI ITEM ESTA USADO 
        public static bool ItemUsado(string pCoditem)
        {
            bool usado = false;
            int count = 0;
            string sql = "SELECT COUNT(*) FROM libro WHERE coditem=@pItem";
            SqlCommand cmd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pItem", pCoditem));

                        count = Convert.ToInt32(cmd.ExecuteScalar());
                        if (count > 0)
                            usado = true;

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return usado;
        }

        //VER SI APLICA ALIAS
        public static bool AplicaAlias(string pCoditem)
        {
            bool APlica = false;
            string sql = "SELECT alias FROM libro WHERE coditem = @pCoditem";
            SqlCommand cmd;
            SqlConnection cn;
            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            //PARAMETROS    
                            cmd.Parameters.Add(new SqlParameter("@pCoditem", pCoditem));

                            if (cmd.ExecuteScalar().ToString().Length > 0)
                                APlica = true;
                        }
                        cmd.Dispose();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }


            return APlica;
        }

        //OBTENER ALIAS DE ITEM
        public static string GetAlias(string pCoditem)
        {
            string Alias = "";
            string sql = "SELECT alias FROM libro WHERE coditem=@pCoditem";
            SqlConnection cn;
            SqlCommand cmd;
            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@pCoditem", pCoditem));

                            Alias = (string)cmd.ExecuteScalar();
                        }
                        cmd.Dispose();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return Alias;
        }

        //public void UpdateAlias()
        //{
        //    string sql = "UPDATE libro SET alias = item.descripcion " +
        //        "FROM item " +
        //        "INNER JOIN libro ON libro.coditem = item.coditem " +
        //        "WHERE libro.alias = ''";

        //}
    }

    /*CONEXION REMULICENCIA*/
    class Licencia
    {
        public static SqlConnection Lconexion = null;
        /// <summary>
        /// No direccion ip del servidor
        /// </summary>
        public static string ServerName { get; set; } = "";
        /// <summary>
        /// Nombre de la base de datos donde está alojada la licencia
        /// </summary>
        public static string DataBase { get; set; } = "perlicencia";
        /// <summary>
        /// Nombre de usuario para conexion con la base de datos.
        /// </summary>
        public static string UserId { get; set; } = "";
        /// <summary>
        /// Contraseña para conexion con base de datos.
        /// </summary>
        public static string Password { get; set; } = "";

        //SOLO PARA GUARDAR DATABASE QUE SE UTILIZARÁ PARA GENERAR EL CODIGO DE LICENCIAS
        //NECESITAMOS SABER A QUE BASE APUNTAR PARA OBTENER EL VALOR DE TABLA EMPRESA PARA ESE JUEGO DE DATOS
        /// <summary>
        /// Nombre del juego de datos o base de datos de la cual se va a obtener la data para generar codigo de licencia.
        /// </summary>
        public static string BDLicencia { get; set; } = "";

        public static bool JuegoValido { get; set; }
        public static bool HayDatos  { get; set; }

        //VALOR POR DEFECTO PARA SESIONES SIMULTANEAS (USUARIOS CONCURRENTES)
        /// <summary>
        /// Indica la cantidad de usuario que pueden estar conectado en simultaeno al sistema.
        /// </summary>
        public static int Concurrencia { get; set; } = 5;

        //PARA GUARDAR EL NOMBRE DEL JUEGO DE DATOS (NOMBRE DE LA CONEXION)
        public static string ConfigName { get; set; } = "";

        public static bool LicenciaRegistrada { get; set; } = false;

        //FORZAR CIERRE DE APLICACION SIN PREGUNTAR
        /// <summary>
        /// Variable de tipo booleana que nos indica si hay algun problema con la licencia.
        /// </summary>
        public static bool ForzarCierre { get; set; }

        /*CONEXION CON BASE DE DATOS */
        /// <summary>
        /// Crea una nueva conexion con la base de datos de la licencia.
        /// </summary>
        /// <returns></returns>
        public static bool NuevaConexion()
        {
            if (Lconexion != null)
                Lconexion.Dispose();

            try
            {
                string sql = $"SERVER={ServerName}; DATABASE={DataBase};USER ID={UserId}; PASSWORD={Password}; MultipleActiveResultSets=True;";
                Lconexion = new SqlConnection();
                Lconexion.ConnectionString = sql;
                Lconexion.Open();

                return true;
            }
            catch (SqlException ex)
            {
                //XtraMessageBox.Show(ex.Message);
                return false;
            }            
        }      

        /*AGREGAR DATOS EN TABLA */
        /// <summary>
        /// Inserta o actualiza codigo de licencia en tabla GLSH
        /// </summary>
        /// <param name="pCode">Codigo de licencia</param>
        /// <returns></returns>
        public static bool IngresaLicencia(string pCode)
        {
            string sql = "INSERT INTO GLSH(kapp) VALUES(@pKapp)";
            string sqlUpdate = "UPDATE GLSH SET kapp=@pKapp";
            SqlCommand cmd;
            int count = 0;
            bool correcto = false;
            bool existeReg = ExisteLicencia();
            try
            {
                if (!existeReg)
                {
                    //INGRESAMOS REGISTRO
                    if (NuevaConexion())
                    {
                        using (cmd = new SqlCommand(sql, Lconexion))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@pKapp", pCode));

                            count = cmd.ExecuteNonQuery();
                            if (count > 0)
                                correcto = true;
                            else
                                correcto = false;

                            cmd.Dispose();
                            Lconexion.Close();
                        }
                    }
                }
                else
                {
                    if (NuevaConexion())
                    {
                        using (cmd = new SqlCommand(sqlUpdate, Lconexion))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@pKapp", pCode));

                            count = cmd.ExecuteNonQuery();
                            if (count > 0)
                                correcto = true;
                            else
                                correcto = false;

                            cmd.Dispose();
                            Lconexion.Close();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return correcto;
        }

        //VER SI LA TABLA LICENCIA YA TIENE UN REGISTRO
        /// <summary>
        /// Nos indica si existe registro de licencia en tabla GLSH
        /// </summary>
        /// <returns></returns>
        public static bool ExisteLicencia()
        {
            bool existe = false;
            string sql = "SELECT count(*) FROM glsh";
            SqlCommand cmd;
            try
            {
                if (NuevaConexion())
                {
                    using (cmd = new SqlCommand(sql, Lconexion))
                    {
                        if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                            existe = true;

                        cmd.Dispose();
                        Lconexion.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return existe;
        }

        /*OBTENER LA LICENCIA SIN CONSIDERAR EL NUMERO DE USUARIOS (TRAMO FINAL &)*/
        /// <summary>
        /// Extrae parte del codigo de licencia que no considera la cantidad de usuarios que se permiten conectar en simultaneo
        /// </summary>      
        /// <param name="pCode">Codigo de licencia</param>        
        /// <returns></returns>
        public static string GetLicenciaSinUsers(string pCode)
        {
            string extraer = "";
            string[] partes = new string[2];
            if (pCode.Length > 0)
            {
                partes = pCode.Split('@');
                //SOLO CONSIDERAMOS EL CODIGO QUE ESTÁ ANTES DEL &
                if (partes.Length > 0)
                    extraer = partes[0];
            }

            return extraer;
        }

        //OBTENER CODIGO DE LICENCIA
        /// <summary>
        /// Entrega el codigo de licencia que se encuentra guardado en base de datos.
        /// <para>Corresponde al campo 'kapp' de la tabla 'GLSH'</para>
        /// </summary>
        /// <returns></returns>
        public static string GetLicencia()
        {
            string cod = "";
            string sql = "SELECT top 1 kapp FROM glsh";
            SqlCommand cmd;
            try
            {
                if (NuevaConexion())
                {
                    using (cmd = new SqlCommand(sql, Lconexion))
                    {
                        object data = cmd.ExecuteScalar();
                        if (data != DBNull.Value)
                            cod = (string)data;
                        
                        Lconexion.Close();
                        cmd.Dispose();

                        if (cod == null)
                            cod = "";
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return cod;
        }        

        /*OBTENER INFORMACION DE JUEGO DE DATOS (BASE DE DATOS) PARA GENERAR CODIGO DE LICENCIA*/
        /// <summary>
        /// Genera un codigo de licencia válido.
        /// </summary>
        /// <returns></returns>
        public static string GenerarLicencia()
        {
            string ConnectionString = "", sql = "", Licencia = "";
            Empresa emp = new Empresa();

            //OBTENER BASE DE DATOS PRIMARIA PARA GENERAR LICENCIA DESDE ARCHIVO DE CONFIGURACION
            //string p = XmlC.GetValueKey("primary", "appSettings");
            if (ExisteLicencia())
            {
                string p = GetBdPrimary();
                Cifrado cif = new Cifrado();
                p = cif.DesencriptaTripleDesc(p);

                if (p.Length > 0)
                {
                    //GUARDAR EL NOMBRE DEL JUEGO DE DATOS
                    ConfigName = p;
                    string[] information = SqlLite.fnBuscarConfigLicencia(p);
                    if (information.Length > 0 && (information[0] != "" && information[1] != "" && information[2] != "" && information[3] != ""))
                    {
                        //SETEAMOS EN TRUE VARIABLE BDVALIDA
                        JuegoValido = true;

                        //GENERAMOS CADENA DE CONEXION
                        ConnectionString = CreateStringBuilder(information);

                        sql = "SELECT rutemp, razon, giro from EMPRESA";

                        //OBTENEMOS VALORES
                        emp = GetInfo(ConnectionString, sql);
                        //GENERAMOS CODIGO DE LICENCIA
                        Licencia = CalculaLicencia(emp.Rut, emp.Giro, emp.Razon);
                    }
                    else
                    {
                        JuegoValido = false;
                    }
                }
                else
                    JuegoValido = false;
            }            

            return Licencia;
        }

        //CREATE STRING BUILDER CONNECTION
        /// <summary>
        /// Crea una cadena de conexion.
        /// </summary>
        /// <param name="pData">Array con todos los datos necesarios para abrir una conexion sql server</param>
        /// <returns></returns>
        private static string CreateStringBuilder(string[] pData)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            Cifrado cif = new Cifrado();
            if (pData.Length > 0)
            {
                try
                {
                    //SERVER
                    builder.DataSource = pData[0];
                    builder.InitialCatalog = pData[1];
                    builder.UserID = pData[2];
                    builder.Password = cif.DesencriptaTripleDesc(pData[3]);
                }
                catch (Exception  ex)
                {
                    //ERROR                    
                }                         
            }

            //RETORNAMOS LA CADENA DE CONEXION GENERADA EN BASE A LOS PARAMETROS...
            return builder.ConnectionString;            
        }

        /*GENERA CODIGO DE LICENCIA */
        /*CIFRADO --> RUT + GIRO + RAZON*/
        /// <summary>
        /// Genera codigo de licencia en base a rut empresa, giro empresa y razon social
        /// </summary>
        /// <param name="pRut">Rut de la empresa</param>
        /// <param name="pGiro">Giro de la empresa</param>
        /// <param name="pRazon">Razon social de la empresa</param>
        /// <returns></returns>
        private static string CalculaLicencia(string pRut, string pGiro, string pRazon)
        {
            string codigo = "", r = "", g = "", rz = "", num = "";
            Cifrado cif = new Cifrado();
            if (pRut.Length > 0 && pGiro.Length > 0 && pRazon.Length > 0)
            {
                HayDatos = true;

                //GENERAMOS CIFRADO PARA RUT + GIRO + RAZON
                //NO CONSIDERAMOS EL NUMERO DE USUARIOS 
                r = cif.Encripta(pRut);
                g = cif.Encripta(pGiro);
                rz = cif.Encripta(pRazon);                

                codigo = r + "&" + g + "&" + rz;
            }
            else
            {
                HayDatos = false;
            }
            return codigo;
        }

        //GENERA CONEXION SOLO PARA OBTENER LOS DATOS
        /// <summary>
        /// Obtiene información relacionada con la empresa.
        /// </summary>
        /// <param name="pStringConnection">Cadena de conexion sql server</param>
        /// <param name="pQuery">Consulta sql</param>
        /// <returns></returns>
        private static Empresa GetInfo(string pStringConnection, string pQuery)
        {
            Empresa emp = new Empresa();
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader rd;

            if (pStringConnection.Length > 0)
            {
                try
                {
                    using (cn = new SqlConnection(pStringConnection))
                    {
                        cn.Open();
                        using (cmd = new SqlCommand(pQuery, cn))
                        {
                            //RECORREMOS Y OBTENEMOS EL VALOR
                            rd = cmd.ExecuteReader();
                            if (rd.HasRows)
                            {
                                while (rd.Read())
                                {
                                    emp.Rut = (string)rd["rutemp"];
                                    emp.Giro = (string)rd["giro"];
                                    emp.Razon = (string)rd["razon"];
                                }
                            }
                        }
                        cmd.Dispose();
                        cn.Close();
                        rd.Close();
                    }
                }
                catch (SqlException ex)
                {
                  //ERROR DE CONEXION
                }
            }

            return emp;
        }

        //GENERA LICENCIA DE PRUEBA (HASTA QUE SE INGRESEN LOS DATOS EN TABLA EMPRESA)
        public static string GeneraTrial()
        {
            string trial = "", MaxCount = "";
            Cifrado cif = new Cifrado();
            trial = cif.EncriptaTripleDesc("#%$&%&/(&/)/()4564¨[*][.~~");
            MaxCount = cif.EncryptBase64(Concurrencia);
            trial = trial + "&" + MaxCount;
            return trial;
        }

        //OBTENER BD PRIMARY
        /// <summary>
        /// Obtiene desde base de datos licencia la base de datos o el juego de datos 
        /// que se usara para extraer y generar el codigo de licencia.
        /// <para>Corresponde el campo **bd** de la tabla **GLSH** de la base de datos **PERLICENCIA**</para>
        /// </summary>
        /// <returns></returns>
        private static string GetBdPrimary()
        {
            string sql = "SELECT TOP 1 bd FROM GLSH";
            string data = "";
            SqlCommand cmd;

            try
            {
                if (NuevaConexion())
                {
                    using (cmd = new SqlCommand(sql, Lconexion))
                    {
                        //SI ES DISTINTO DE NULO TRATAMOS DE CONVERTIR A STRING
                        if (!(cmd.ExecuteScalar().Equals(System.DBNull.Value)))
                        {
                            data = (string)cmd.ExecuteScalar();
                        }                        

                        cmd.Dispose();
                        Lconexion.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                //XtraMessageBox.Show(ex.Message);
            }

            return data;
        }

        /// <summary>
        /// Nos entrega la fecha de expiracion de la licencia
        /// </summary>
        /// <returns></returns>
        public static DateTime GetExpiration()
        {
            DateTime Date = Convert.ToDateTime("01-01-1900");
            string sql = "SELECT top 1 xp FROM glsh";
            SqlCommand cmd;
            string data = "";
            Cifrado cif = new Cifrado();

            try
            {
                if (NuevaConexion())
                {
                    using (cmd = new SqlCommand(sql, Lconexion))
                    {
                        //SI ES DISTINTO DE NULO TRATAMOS DE CONVERTIR A STRING
                        if (!(cmd.ExecuteScalar().Equals(System.DBNull.Value)))
                        {
                            data = (string)cmd.ExecuteScalar();
                            data = cif.DesencriptaTripleDesc(data);
                           
                            if (data.Length > 0)
                                Date = Convert.ToDateTime(data);
                        }

                        cmd.Dispose();
                        Lconexion.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                //XtraMessageBox.Show(ex.Message);
            }

            return Date;
        }
    }

    /*MANEJO DE SESIONES*/
    class Sesion
    {
        public static int SpidSesion { get; set; }
        public static bool Activa { get; set; }        

        // HORA EN LA QUE SE REALIZÓ LA ULTIMA CONSULTA A LA BASE DE DATOS (ULTIMA ACTIVIDAD DEL USUARIO)
        public static DateTime TiempoUltimaActividad { get; set; } = DateTime.Now.Date;

        /*INGRESA DATOS EN TABLA ACCESS (SESIONES USUARIOS)*/
        public static void NewAccess(string pUser, string pHardware, string pClientIp, string pBd)
        {
            string sql = "INSERT INTO ACCESS(usr, hw, clientip, bd, hostname, lstr) " +
                "VALUES(@pUsr, @pHw, @pClientip, @pBd, @pHostName, getdate())";
            SqlCommand cmd;

            Cifrado cif = new Cifrado();
            DateTime now = DateTime.Now;
            //CIFRAMOS FECHA 
           // string cifrado = cif.EncriptaTripleDesc(now.ToString("dd-MM-yyyy H:mm:ss"));            
           // string descr = cif.DesencriptaTripleDesc(cifrado);
           // DateTime dat = Convert.ToDateTime(descr);
            try
            {
                if (Licencia.NuevaConexion())
                {
                    using (cmd = new SqlCommand(sql, Licencia.Lconexion))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pUsr", cif.EncriptaTripleDesc(pUser)));
                        cmd.Parameters.Add(new SqlParameter("@pHw", cif.EncriptaTripleDesc(pHardware)));
                        cmd.Parameters.Add(new SqlParameter("@pClientip", cif.EncriptaTripleDesc(pClientIp)));                        
                        cmd.Parameters.Add(new SqlParameter("@pBd", cif.EncriptaTripleDesc(pBd)));
                        cmd.Parameters.Add(new SqlParameter("@pHostName", cif.EncriptaTripleDesc(Environment.MachineName)));

                        cmd.ExecuteNonQuery();
                    }
                    cmd.Dispose();
                    Licencia.Lconexion.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        /*SOBREESCRIBIR ULTIMA ACTIVIDAD USUARIO CON SESION ABIERTA*/
        public static void NuevaActividad()
        {
            string sql = "UPDATE ACCESS SET lstr=getdate() WHERE usr=@pUser";
            SqlCommand cmd;
            Cifrado cif = new Cifrado();
            
            try
            {
                if (Licencia.NuevaConexion())
                {
                    using (cmd = new SqlCommand(sql, Licencia.Lconexion))
                    {
                        cmd.Parameters.Add(new SqlParameter("@pUser", cif.EncriptaTripleDesc(User.getUser().ToLower())));

                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                        Licencia.Lconexion.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        /*ELIMINAR DATOS DE SESION DE UN USUARIO EN PARTICULAR*/
        public static bool DeleteAccess(string pUser)
        {
            string sql = "DELETE FROM ACCESS WHERE usr=@pUser AND bd=@pDatabase";
            int count = 0;
            SqlCommand cmd;
            Cifrado cif = new Cifrado();
            try
            {
                if (Licencia.NuevaConexion())
                {
                    using (cmd = new SqlCommand(sql, Licencia.Lconexion))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pUser", cif.EncriptaTripleDesc(pUser)));
                        cmd.Parameters.Add(new SqlParameter("@pDatabase", cif.EncriptaTripleDesc(fnSistema.pgDatabase)));

                        count = cmd.ExecuteNonQuery();
                        cmd.Dispose();
                        Licencia.Lconexion.Close();

                        if (count > 0)
                            return true;
                        else
                            return false;
                    }                
                }
            }
            catch (SqlException ex)
            {
                //   XtraMessageBox.Show(ex.Message);
                return false;
            }
            return false;
        }

        /*OBTENER TODOS LOS DATOS DE LA SESSION*/
        public static Hashtable GetInformationUserSession(string pUser, string pDatabase)
        {
            string sql = "SELECT * FROM access WHERE usr=@pUser AND bd=@pDatabase";
            SqlCommand cmd;
            SqlDataReader rd;
            Cifrado cif = new Cifrado();
            Hashtable data = new Hashtable();
            try
            {
                if (Licencia.NuevaConexion())
                {
                    using (cmd = new SqlCommand(sql, Licencia.Lconexion))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pUser", cif.EncriptaTripleDesc(pUser)));
                        cmd.Parameters.Add(new SqlParameter("@pDatabase", cif.EncriptaTripleDesc(pDatabase)));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //GUARDAMOS DATOS EN HASH
                                data.Add("ip", cif.DesencriptaTripleDesc((string)rd["clientip"]));
                                data.Add("hostname", cif.DesencriptaTripleDesc((string)rd["hostname"]));
                                data.Add("hw", cif.DesencriptaTripleDesc((string)rd["hw"]));                                
                            }
                        }
                        cmd.Dispose();
                        rd.Close();
                        Licencia.Lconexion.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return data;
        }

        /*LIMITE DE USUARIOS*/
        public static bool UserLimit(int pMaxCount, string pDatabase)
        {
            bool supera = false;
            int count = 0;
            string sql = "SELECT COUNT(*) from access WHERE bd = @pDataBase";
            SqlCommand cmd;
            Cifrado cif = new Cifrado();
            try
            {
                if (Licencia.NuevaConexion())
                {
                    using (cmd = new SqlCommand(sql, Licencia.Lconexion))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pDataBase", cif.EncriptaTripleDesc(pDatabase)));

                        count = Convert.ToInt32(cmd.ExecuteScalar());

                        if (count >= pMaxCount)
                            supera = true;

                        cmd.Dispose();
                        Licencia.Lconexion.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return supera;
        }

        /*HORA ULTIMA CONSULTA REALIZADA POR UN USUARIO X (DATETIME)*/
        public static DateTime LastRequestUser(int pSid)
        {
            //OBTENEMOS EL SPID DE LA CONEXION (ID PROCESS)
            string sql = "SELECT last_batch " +
                          "From sysprocesses " +
                          "WHERE spid = @pSid";

            SqlCommand cmd;
            DateTime LastRequest = DateTime.Now.Date;
            try
            {

                if (Licencia.NuevaConexion())
                {
                    using (cmd = new SqlCommand(sql, Licencia.Lconexion))
                    {
                        cmd.Parameters.Add(new SqlParameter("@pSid", pSid));

                        //GUARDAMOS EL DATETIME...
                        LastRequest = Convert.ToDateTime(cmd.ExecuteScalar());                        

                        cmd.Dispose();
                        Licencia.Lconexion.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return LastRequest;
        }

        /*HORA ULTIMA CONSULTA REALIZADA POR UN USUARIO X EN TABLA ACCESS*/
        public static DateTime LastRequestUserAc()
        {
            DateTime fecha = DateTime.Now.Date;
            string sql = "SELECT lstr FROM access WHERE usr=@pUser AND bd=@pDataBase";
            SqlCommand cmd;
            Cifrado cif = new Cifrado();

            //SI LA FECHA ESTA ENCRIPTADA DEBEMOS DESENCRIPTARLA Y TRANSFORMARLA A FORMATO FECHA
            try
            {
                if (Licencia.NuevaConexion())
                {
                    using (cmd = new SqlCommand(sql, Licencia.Lconexion))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pUser", cif.EncriptaTripleDesc(User.getUser().ToLower())));
                        cmd.Parameters.Add(new SqlParameter("@pDataBase", cif.EncriptaTripleDesc(fnSistema.pgDatabase)));

                        // Convert.ToDateTime(cif.DesencriptaTripleDesc(cmd.ExecuteScalar));
                        fecha = Convert.ToDateTime(cmd.ExecuteScalar());
                        cmd.Dispose();
                        Licencia.Lconexion.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return fecha;
        }

        /*OBTENER DIRECCION IP DEL EQUIPO CONECTADO*/
        /*APNTAMOS A LA BASE DE DATOS (JUEGO DE DATOS) DEL CLIENTE*/
        public static String GetIpSesion(int pSid)
        {
            string sql = "SELECT TOP 1 " +
                         " client_net_address FROM sys.dm_exec_connections" +
                         " WHERE session_id=@pSid";
            string ip = "";
            SqlCommand cmd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pSid", pSid));
                        ip = (string)cmd.ExecuteScalar();

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }           

            return ip;
        }

        /*GET SPID*/
        //APUNTAMOS A LA BASE DE DATOS DEL USUARIO (JUEGO DE DATOS) YA QUE NECESITAMOS OBTENER EL 
        //ID DEL PROCESO EN ESA BASE DE DATOS Y NO EN LA BASE DE DATOS DONDE GESTIONAMOS LAS LICENCIAS
        //USAMOS FUNCION DE LA CLASE ESTATICA FNSISTEMA Y NO DE LA CLASE LICENCIAS
        public static void SetSpid()
        {
            string sql = "SELECT @@spid";
            int spid = 0;
            SqlCommand cmd;

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //OBTENEMOS ID PROCESO...
                        spid = Convert.ToInt32(cmd.ExecuteScalar());

                        //SETEAMOS PROPIEDAD...
                        SpidSesion = spid;
                    }

                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

        }

        /*KILL SESSION */
        public static void Kill(int pId)
        {
            string sql = "DECLARE @session_id AS INTEGER " +
                          "DECLARE @query  AS varchar(50) " +
                          "SET @session_id = @pId " +
                          "SET @query = 'kill ' + CAST(@session_id as VARCHAR(10)) " +
                          "exec(@query)";

            SqlCommand cmd;
            try
            {
                if (Licencia.NuevaConexion())
                {
                    using (cmd = new SqlCommand(sql, Licencia.Lconexion))
                    {
                        cmd.Parameters.Add(new SqlParameter("@pId", pId));
                        cmd.ExecuteNonQuery();
                    }
                    cmd.Dispose();
                    Licencia.Lconexion.Close();
                }
            }
            catch (SqlException ex)
            {
               XtraMessageBox.Show(ex.Message);
            }
        }

        /*VERIFICAR QUE SPID EXISTA*/
        public static bool ExisteSpid(int pId)
        {
            bool existe = false;
            string sql = "SELECT count(*) FROM sysprocesses WHERE spid=@pPid";
            SqlCommand cmd;
            try
            {
                if (Licencia.NuevaConexion())
                {
                    using (cmd = new SqlCommand(sql, Licencia.Lconexion))
                    {
                        //PARAMETROS    
                        cmd.Parameters.Add(new SqlParameter("@pPid", pId));

                        if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                            existe = true;

                        cmd.Dispose();
                        Licencia.Lconexion.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return existe;
        }

        //VERIFICAR SI EL USUARIO YA TIENE LA SESION ABIERTA
        public static bool Abierta(string pUser, string pBaseDatos)
        {
            bool abierta = false;
            string sql = "SELECT count(*) FROM access WHERE usr=@pUser AND bd=@pBd";
            SqlCommand cmd;
            string cifs = "";
            Cifrado cif = new Cifrado();
            
            try
            {
                if (Licencia.NuevaConexion())
                {
                    using (cmd = new SqlCommand(sql, Licencia.Lconexion))
                    {
                        //PARAMETROS

                            cmd.Parameters.Add(new SqlParameter("@pUser", cif.EncriptaTripleDesc(pUser)));
                        cmd.Parameters.Add(new SqlParameter("@pBd", cif.EncriptaTripleDesc(pBaseDatos)));

                        if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                            abierta = true;

                        cmd.Dispose();
                        Licencia.Lconexion.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return abierta;
        }

        //USUARIOS DE GRUPO CON SESION ABIERTA
        public static bool GrupoAbierta(int pGrupo)
        {
            bool abierto = false;
            string sql = "SELECT COUNT(*) FROM usuario " +
                         "INNER JOIN perLicencia.dbo.access ON access.usr = usuario.usuario " +
                          "WHERE grupo=@pGrupo";
            SqlCommand cmd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pGrupo", pGrupo));

                        if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                            abierto = true;

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return abierto;
        }

        //CANTIDAD DE SESIONES ABIERTAS
        public static int SesionesAbiertas(string pDataBase)
        {
            string sql = "SELECT count(*) FROM access WHERE bd=@pDatabase";
            string bas = "";
            int count = 0;
            SqlCommand cmd;
            Cifrado cif = new Cifrado();

            //bas = cif.DesencriptaTripleDesc(pDataBase);
            try
            {
                if (Licencia.NuevaConexion())
                {
                    using (cmd = new SqlCommand(sql, Licencia.Lconexion))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pDatabase", cif.EncriptaTripleDesc(pDataBase)));

                        count = Convert.ToInt32(cmd.ExecuteScalar());

                        cmd.Dispose();
                        Licencia.Lconexion.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return count;
        }

        //LLENA GRILLA CON TODAS LAS SESIONES
        public static void ShowSesion(GridControl pGridView, string pDataBase)
        {
            string sql = "SELECT usr, regdt, hw, clientip, bd, hostname, lstr FROM access WHERE bd=@pBase";
            SqlCommand cmd;
            SqlConnection cn;
            SqlDataAdapter ad = new SqlDataAdapter();
            DataSet ds = new DataSet();

            try
            {
                if (Licencia.NuevaConexion())
                {
                    using (cmd = new SqlCommand(sql, Licencia.Lconexion))
                    {
                        cmd.Parameters.Add(new SqlParameter("@pBase", pDataBase));
                        ad.SelectCommand = cmd;
                        ad.Fill(ds, "data");

                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            pGridView.DataSource = ds.Tables[0];
                        }

                        ad.Dispose();
                        cmd.Dispose();
                    }
                }
            }
            catch (SqlException ex)
            {
              //ERROR...
            }
        }

        /// <summary>
        /// Retorna un listado con todos los usuarios conectados a una base de datos.
        /// </summary>
        /// <param name="pBaseDatos"></param>
        /// <returns></returns>
        public static List<string> GetListUsers(string pBaseDatos)
        {
            List<string> Usuarios = new List<string>();
            string sql = "SELECT usr FROM access WHERE bd=@pBase";
            Cifrado cif = new Cifrado();
            SqlCommand cmd;
            //SqlConnection cn;
            SqlDataReader rd;

            try
            {
                if (Licencia.NuevaConexion())
                {
                    using (cmd = new SqlCommand(sql, Licencia.Lconexion))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pBase", cif.EncriptaTripleDesc(pBaseDatos)));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                Usuarios.Add((cif.DesencriptaTripleDesc((string)rd["usr"])));
                            }
                        }
                    }
                    rd.Close(); 
                }
            }
            catch (SqlException ex)
            {
                //ERROR...
            }

            return Usuarios;
        }
    }

    class Empresa
    {
        /// <summary>
        /// Rut empresa.
        /// </summary>
        public string Rut { get; set; } = "";
        /// <summary>
        /// Razon social empresa.
        /// </summary>
        public string Razon { get; set; } = "";
        /// <summary>
        /// Giro empresa.
        /// </summary>
        public string Giro { get; set; } = "";
        /// <summary>
        /// Direccion empresa.
        /// </summary>
        public string Direccion { get; set; } = "";
        /// <summary>
        /// Ciudad empresa.
        /// </summary>
        public int Ciudad { get; set; }
        /// <summary>
        /// NOMBRE DE LA CIUDAD DE LA EMPRESA
        /// </summary>
        public string NombreCiudad { get; set; }
        /// <summary>
        /// Telefono empresa.
        /// </summary>
        public string Telefono { get; set; } = "";
        /// <summary>
        /// Email empresa.
        /// </summary>
        public string EmailEmp { get; set; } = "";
        /// <summary>
        /// Rut representante legal.
        /// </summary>
        public string RutRep { get; set; } = "";
        /// <summary>
        /// Nombre representante legal.
        /// </summary>
        public string NombreRep { get; set; } = "";
        /// <summary>
        /// Rut persona recurso humanos.
        /// </summary>
        public string RutRh { get; set; } = "";
        /// <summary>
        /// Nombre persona a cargo de recursos humanos.
        /// </summary>
        public string NombreRh { get; set; } = "";
        /// <summary>
        /// Nombre cargo persona recursos humanos.
        /// </summary>
        public string CargoRh { get; set; } = "";
        /// <summary>
        /// Email persona recursos humanos.
        /// </summary>
        public string EmailRh { get; set; } = "";
        /// <summary>
        /// Codigo actividad empresa.
        /// </summary>
        public string CodActividad { get; set; } = "";
        /// <summary>
        /// Fecha inicio de actividades empresa.
        /// </summary>
        public DateTime InicioActividades { get; set; } 
        /// <summary>
        /// Codigo de afiliacion.
        /// <para>Nota: Hace referencia a si la empresa tiene mutual y caja, solo mutual, solo caja, etc.</para>
        /// </summary>
        public int codAfiliacion { get; set; }
        /// <summary>
        /// Numero asociado a la mutual.
        /// </summary>
        public string codMutual { get; set; } = "";
        /// <summary>
        /// Porcentaje de cotizacion a mutual.
        /// <para>Nota: No considerar en este porcentaje la cotizacion ley sanna.</para>
        /// </summary>
        public double cotMutual { get; set; }
        /// <summary>
        /// Codigo mutual asociado.
        /// <para>Nota: Hace referencia al codigo de tabla mutual.</para>
        /// </summary>
        public int Mutual { get; set; }
        /// <summary>
        /// Codigo caja de compensacion.
        /// <para>Nota: Codigo viene de tabla cajacompensacion.</para>
        /// </summary>
        public int NombreCCAF { get; set; }
        /// <summary>
        /// Codigo de empresa.
        /// <para>Nota: se usa en archivo previred.</para>
        /// </summary>
        public string CodEmpresa { get; set; }
        /// <summary>
        /// Nombre de la comuna a la cual pertenece la empresa
        /// </summary>
        public string Comuna { get; set; }
        /// <summary>
        /// Código pais para numero telefónico
        /// </summary>
        public string CodigoPais { get; set; }
        /// <summary>
        /// Código de area ciudad para numero telefónico
        /// </summary>
        public string CodigoArea { get; set; }

        /*OBTENER DATOS DE EMPRESA*/
        /// <summary>
        /// Obtiene informacion de empresa.
        /// </summary>
        public void SetInfo()
        {
            string sql = "SELECT rutemp, razon, giro, direccion, ciudad, descCiudad, telefono, emailEmp," +
                "rutrep, nombreRep, rutRrhh, nombreRrhh, cargoRrhh, emailRrhh, codActividad, " +
                "inicioActividades, codAfiliacion, codMut, cotMut, nombreMut, nombreCcaf, codempresa, " +
                "descComuna as Comuna, cdArea, cdPais " +
                "FROM empresa " +
                "INNER JOIN ciudad ON empresa.ciudad = ciudad.idCiudad " +
                "INNER JOIN comuna ON comuna.codComuna = empresa.comuna ";

            SqlCommand cmd;
            SqlDataReader rd;
            SqlConnection cn;
            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            rd = cmd.ExecuteReader();
                            if (rd.HasRows)
                            {
                                while (rd.Read())
                                {
                                    Giro = (string)rd["giro"];
                                    Razon = (string)rd["razon"];
                                    Rut = (string)rd["rutemp"];
                                    Direccion = (string)rd["direccion"];
                                    Ciudad = (int)rd["ciudad"];
                                    NombreCiudad = (string)rd["descCiudad"];
                                    Telefono = (string)rd["telefono"];
                                    EmailEmp = (string)rd["emailEmp"];
                                    RutRep = (string)rd["rutrep"];
                                    NombreRep = (string)rd["nombreRep"];
                                    RutRh = (string)rd["rutRrhh"];
                                    NombreRh = (string)rd["nombreRrhh"];
                                    CargoRh = (string)rd["cargoRrhh"];
                                    EmailRh = (string)rd["emailRrhh"];
                                    CodActividad = (string)rd["codActividad"];
                                    InicioActividades = Convert.ToDateTime(rd["inicioActividades"]);
                                    codAfiliacion = Convert.ToInt32(rd["codAfiliacion"]);
                                    codMutual = (string)rd["codMut"];
                                    cotMutual = Convert.ToDouble(rd["cotMut"]);
                                    Mutual = Convert.ToInt32(rd["nombreMut"]);
                                    NombreCCAF = Convert.ToInt32(rd["nombreCcaf"]);
                                    CodEmpresa = (string)rd["codempresa"];
                                    Comuna = (string)rd["Comuna"];
                                    CodigoArea = (string)rd["cdArea"];
                                    CodigoPais = (string)rd["cdPais"];
                                }
                            }
                        }
                        cmd.Dispose();
                        rd.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }       
        }

        /*GENERA CODIGO DE VALIDACION*/
        /// <summary>
        /// Genera codigo de validacion para validar licencia.
        /// <para>Nota: Si no hay datos de empresa usamos codigo 999999999 para rut.</para>
        /// <para>Nota2: Si no hay datos de empresa usamos codigo 0 para giro. </para>
        /// <para>Nota3: Si no ha datos de empresa usamos codigo 0 para razon.</para>
        /// </summary>
        /// <returns></returns>
        public string GeneraCodValidacion()
        {
            string cod = "";
            string conca = "";
            
            //CONCATENAMOS TODA LA INFORMACION
            conca = Rut.Trim() + Giro.Trim() + Razon.Trim();
            if (conca.Trim() == "99999999900")
                return "0";

            //GENERAMOS CODIGO EN BASE A CADENA CON DATOS EMPRESA (HASH)
            Encriptacion enc = new Encriptacion(conca);
            cod = enc.getHas();

            //CIFRAMOS CODIGO GENERADO (CIFRADO DEL CODIGO HASH)
            Cifrado cif = new Cifrado();
            string cifrado = cif.Encripta(cod);

            return cifrado;
        }

        /*GENERA CODIGO DE LICENCIA */
        /*CIFRADO --> RUT + GIRO + RAZON + N° USUARIOS*/
        /// <summary>
        /// Genera codigo de licencia para juego de datos, de acuerdo a datos de empresa.
        /// </summary>
        /// <param name="pNum">Representa la cantidad de usuarios concurrentes.</param>
        /// <returns></returns>
        public string GeneraLicenciaUser(int pNum)
        {
            string codigo = "", r = "", g = "", rz = "", num = "";
            Cifrado cif = new Cifrado();
            if (Rut.Length > 0 && Giro.Length > 0 && Razon.Length > 0)
            {
                //GENERAMOS CIFRADO PARA RUT
                r = cif.Encripta(Rut);
                g = cif.Encripta(Giro);
                rz = cif.Encripta(Razon);

                //GENERAMOS CIFRADO EN BASE PNUM               
                num = cif.EncryptBase64(pNum);

                codigo = r + "&" + g + "&" + rz + "&" + num;             
            }
            return codigo;
            
        }

        /*VERIFICAR SI HAY REGISTROS EN TABLA EMPRESA*/
        /// <summary>
        /// Indica si hay o no registros en tabla empresa.
        /// </summary>
        /// <returns></returns>
        public bool HayRegistros()
        {
            bool existe = false;
            int count = 0;
            string sql = "SELECT COUNT(*) FROM empresa";
            SqlCommand cmd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        count = Convert.ToInt32(cmd.ExecuteScalar());
                        if (count > 0)
                            existe = true;
                    }

                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return existe;
        }

        /*OBTENER EL CODIGO DE VALIDACION GUARDAR EN TABLA EMPRESA*/
        /// <summary>
        /// Obtiene codigo de validacion sistema desde base de datos.
        /// </summary>
        /// <returns></returns>
        public string GetCodValidacion()
        {
            string val = "";
            string sql = "SELECT lemp FROM empresa";
            SqlCommand cmd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        val = (string)cmd.ExecuteScalar();                        
                    }
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return val;
        }

        /*COMPARA CODIGOS DE VALIDACION*/
        /*RECIBE COMO PARAMETRO CODIGO CIFRADO (LO GENERAMOS)*/
        /// <summary>
        /// Verifica que la licencia del sistema es valida.
        /// </summary>
        /// <param name="pCod">Codigo licencia.</param>
        /// <returns></returns>
        public bool CodValCorrecto(string pCod)
        {
            bool valido = false;
            //OBTENE CODIGO LICENCIA GUARDADO EN CAMPO EMPRESA
            string codBd = GetCodValidacion();

            //SI SON IGUALES ES UN CODIGO VALIDO
            if (codBd == pCod)
                valido = true;

            return valido;
        }

        //GUARDAR INFORMACION DE EMPRESA EN ARCHIVO DE TEXTO
        /// <summary>
        /// Genera y guarda informacion de empresa en un archivo de texto.
        /// </summary>
        /// <param name="pFile">Ruta de salida del archivo.</param>
        public void ExportToTextFile(string pFile)
        {
            string cadena = "";
            Cifrado cif = new Cifrado();

            //DEBEMOS SETEAR LAS VARIABLES INVOCANDO AL METODO SetInfo()
            if (pFile.Length > 0)
            {
                //CREAMOS ARCHIVO
                Archivo.CrearArchivo(pFile);

                if (File.Exists(pFile))
                {
                    //AGREGAMOS DATA A ARCHIVO
                    cadena = $"[Codigo Empresa] ? {CodEmpresa + Environment.NewLine}[Rut] ? {Rut + Environment.NewLine}[Razon] ? {Razon + Environment.NewLine}[Giro] ? {Giro + Environment.NewLine}" +
                        $"[Direccion] ? {Direccion + Environment.NewLine}[Ciudad] ? {Ciudad + Environment.NewLine}[Telefono] ? {Telefono + Environment.NewLine}" +
                        $"[Email] ? {EmailEmp + Environment.NewLine}[Rut Representante Legal] ? {RutRep + Environment.NewLine}[Nombre Representante Legal] ? {NombreRep + Environment.NewLine}" +
                        $"[Rut RRHH] ? {RutRh + Environment.NewLine}[Nombre RRHH] ? {NombreRh + Environment.NewLine}[Cargo RRHH] ? {CargoRh + Environment.NewLine}" +
                        $"[Email RRHH] ? {EmailRh + Environment.NewLine}[Codigo Actividad] ? {CodActividad + Environment.NewLine}" +
                        $"[Inicio Actividades] ? {InicioActividades.ToString("dd-MM-yyyy") + Environment.NewLine}[Codigo Afiliacion] ? {codAfiliacion + Environment.NewLine}" +
                        $"[Numero Asociado Mutual] ? {codMutual + Environment.NewLine}[Cotizacion Mutual] ? {cotMutual + Environment.NewLine}" +
                        $"[Mutual] ? {Mutual + Environment.NewLine}[Caja] ? {NombreCCAF + Environment.NewLine}[bd] ? {fnSistema.pgDatabase + Environment.NewLine}[server] ? {fnSistema.pgServer + Environment.NewLine}" + 
                        $"[user] ? {cif.EncriptaTripleDesc(fnSistema.pgUser) + Environment.NewLine}[psw] ? {cif.EncriptaTripleDesc(fnSistema.pgPass) + Environment.NewLine}";

                    //ELIMINAMOS REGISTROS SI EL ARCHIVO YA ESTABA CREADO...
                    Archivo.CleanTextFile(pFile);

                    Archivo.AgregarInfoFile(pFile, cadena);
                }
            }
        }

        /*OBTENER CAJA DE COMPENSACION ASOCIADA DE LA EMPRESA*/
        /// <summary>
        /// Obtiene el nombre de la caja de compensacion asociada a la empresa.
        /// </summary>
        /// <returns></returns>
        public string CajaCompensacion()
        {
            string sql = "SELECT cajaCompensacion.descCaja as caja from empresa " +
                          "INNER JOIN cajaCompensacion ON cajaCompensacion.id = empresa.nombreCCAF";
            string caja = "";
            SqlCommand cmd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        object data = cmd.ExecuteScalar();
                        if (!data.Equals(DBNull.Value))
                            caja = (string)cmd.ExecuteScalar();

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return caja;
        }

        /*OBTENER VALOR DESDE COLUMNA VS (VERSION)*/
        /// <summary>
        /// Obtiene el numero de version actual del sistema.
        /// </summary>
        /// <returns></returns>
        public string GetCodvs()
        {
            string sql = "SELECT vs FROM empresa";
            string cod = "";
            SqlCommand cmd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        object vs = cmd.ExecuteScalar();
                        if (vs != DBNull.Value)
                            cod = (string)vs;
                    }
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
            return cod;
        }
    }

    class XmlC
    {
        //LEER UN VALOR DE LA SECTION APPSETTINGS DEL ARCHIVO CONFIG
        public static string GetValueKey(string pKey, string pSection)
        {
            string value = "";
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);                
                

                //OBTENEMOS LA SECCION
                ConfigurationSection section = config.GetSection(pSection);

                //CANTIDAD DE ELEMENTOS DE LA COLECCION
                int count = section.CurrentConfiguration.AppSettings.Settings.Count;
                if (count > 0)
                {
                    //OBTENEMOS VALOR DE LA KEY
                    value = ConfigurationManager.AppSettings.Get(pKey);

                    //SI VALUE ES NULL, ES PORQUE NO EXISTE LA CLAVE 
                    if (value == null)
                        value = "";
                }
            }
            catch (ConfigurationErrorsException ex)
            {
                value = "";             
            }        
            
            return value;
        }

        //AGREGAR UNA NUEVA CLAVE AL ARCHIVO DE CONFIGURACION
        public static void AddKey(string pKey, string pValue)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;

                //PREGUNTAMOS SI LA CLAVE QUE QUEREMOS AGREGAR YA EXISTE
                if (settings[pKey] == null)
                {
                    settings.Add(pKey, pValue);
                }
                else
                {
                    //ACTUALIZAMOS VALOR
                    settings[pKey].Value = pValue;
                }

                //GUARDAR CAMBIOS
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        //CIFRAR SECCION O DESCRIFA SECCION DEPENDIENTO DEL II PARAMETRO
        public static void EncryptSection(string pSection, bool? Decrypt = false)
        {
            try
            {
                //SI NO QUEREMOSPASAR COMO PARAMETRO EL NOMBRE DEL ARCHIVO 
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                //USAR TYPE VAR PARA NO ESPECIFICAR EL TIPO DE SETTING QUE QUEREMOS MANIPULAR
                var section = config.GetSection(pSection);                

                if (section.CurrentConfiguration.AppSettings.Settings.Count > 0)
                {
                    //CIFRAMOS
                    if (Decrypt == false)
                        section.SectionInformation.ProtectSection("DataProtectionConfigurationProvider");
                    else
                        section.SectionInformation.UnprotectSection();

                    //GUARDAR LOS CAMBIOS
                    config.Save();
                }               
            }
            catch (ConfigurationErrorsException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        //PREGUNTAR SI LA STRING ESTA CIFRADA
        public static bool IsProtected(string pSection)
        {
            bool cipher = false;

            try
            {
                //OBTENEMOS EL ARCHIVO DE CONFIGURACION
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                //ConnectionStringsSection section = config.GetSection("connectionStrings") as ConnectionStringsSection;
                var section = config.GetSection(pSection);

                //SI ESTA CIFRADA
                if (section.SectionInformation.IsProtected)
                    cipher = true;

            }
            catch (ConfigurationErrorsException ex)
            {
                Console.WriteLine(ex.Message);
            }


            return cipher;
        }

        //VERIFICAR SI EXISTE EL ARCHIVO DE CONFIGURACION
        public static bool ExisteArchivoConfiguracion()
        {
            try
            {
                if (File.Exists(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).FilePath))
                    return true;
                else
                    return false;
            }
            catch (ConfigurationErrorsException ex)
            {
                //ERROR
                return false;
            }
            catch (FileNotFoundException ex)
            {
                //
                return false;
            }            
            
        }
    }

    /*CLASE PARA ENVIO DE CORREO ELECTRÓNICO*/
    class Mail
    {   

        /// <summary>
        /// Servidor smtp.
        /// </summary>
        public string ServerSmtp = "";
        /// <summary>
        /// Puerto.
        /// </summary>
        public int PuertoSmtp = 0;
        /// <summary>
        /// Email servidor.
        /// </summary>
        public string EmailServer = "";
        /// <summary>
        /// Password email servidor.
        /// </summary>
        public string PasswordServer = "";
        /// <summary>
        /// Email cliente.
        /// </summary>
        public string ClientEmail = "";
        public int Tipo = 0;
        /// <summary>
        /// Ruta archivo que se desea adjuntar.
        /// </summary>
        public string pRutaArchivo = "";
        /// <summary>
        /// Mensaje correo.
        /// </summary>
        public string Message = "";
        /// <summary>
        /// Titulo mensaje.
        /// </summary>
        public string TitleMessage = "";
        /// <summary>
        /// Usa o no cifrado ssl.
        /// </summary>
        public bool Ssl = false;

        /*CONSTRUCTOR PARAMETRIZADO*/
        public Mail(string pServerSmtp, int pPuertoSmtp, string pEmailServer, string pPasswordServer, 
            string pClientEmail, int pTipo, string pRuta, string pMessage, string pTitleMessage, bool pSsl)
        {
            this.ServerSmtp = pServerSmtp;
            this.PuertoSmtp = pPuertoSmtp;
            this.EmailServer = pEmailServer;            
            this.PasswordServer = pPasswordServer;
            this.ClientEmail = pClientEmail;
            this.Tipo = pTipo;
            this.pRutaArchivo = pRuta;
            this.Message = pMessage;
            this.TitleMessage = pTitleMessage;
            this.Ssl = pSsl;
        }

        /*CONSTRUCTOR SIN PARAMETROS*/
        public Mail()
        { }

        /// <summary>
        /// Permite enviar un correo electronico.
        /// </summary>
        /// <returns></returns>
        public bool SendMail()
        {
            bool sendOk = false;
            try
            {
                //RECIBE COMO PARAMETRO EL MAIL QUE ENVIA Y EL MAIL DEL DESTINATARIO
                MailMessage Email = new MailMessage(this.EmailServer, this.ClientEmail);

                //CREDENCIALES 
                NetworkCredential Credentials = new NetworkCredential();
                //EL USUARIO CORRESPONDE AL CORREO QUE ENVÍA
                Credentials.UserName = this.EmailServer;
                Credentials.Password = this.PasswordServer;

                //CONFIGURACION SMTP
                SmtpClient Smtp = new SmtpClient();
                Smtp.Port = this.PuertoSmtp;
                Smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                Smtp.UseDefaultCredentials = false;
                Smtp.Host = this.ServerSmtp;
                Smtp.Credentials = Credentials;

                if(this.Ssl)
                    Smtp.EnableSsl = true;

                //TITULO MENSAJE
                Email.Subject = this.TitleMessage;
                //MENSAJE
                Email.Body = this.Message;

                //ADJUNTO
                Attachment Adjunto = new Attachment(this.pRutaArchivo);
                Email.Attachments.Add(Adjunto);
                
                //ENVIAR MAIL
                Smtp.Send(Email);
                sendOk = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                sendOk = false;
            }

            return sendOk;
           
        }

        /// <summary>
        /// Permite testear si hay conexion con un servidor Smtp.
        /// </summary>
        /// <returns></returns>
        public bool TestSmtp()
        {
            bool running = false;

            try
            {
                TcpClient tcp = new TcpClient();
                string SmtpServer = this.ServerSmtp;
                int smtpPort = this.PuertoSmtp;

                //INTENTAMOS CONECTAR
                tcp.Connect(SmtpServer, smtpPort);
                running = true;
            }
            catch (Exception ex)
            {
                running = false;
            }

            return running;
        }

        /// <summary>
        /// Envía un correo de prueba.
        /// </summary>
        /// <returns></returns>
        public bool SendTestMail()
        {
            bool exito = false;
            try
            {
                //RECIBE COMO PARAMETRO EL MAIL QUE ENVIA Y EL MAIL DEL DESTINATARIO
                MailMessage Email = new MailMessage(this.EmailServer, this.ClientEmail);

                //CREDENCIALES 
                NetworkCredential Credentials = new NetworkCredential();

                //EL USUARIO CORRESPONDE AL CORREO QUE ENVÍA
                Credentials.UserName = this.EmailServer;
                Credentials.Password = this.PasswordServer;                

                //CONFIGURACION SMTP
                SmtpClient Smtp = new SmtpClient();
                Smtp.Port = this.PuertoSmtp;
                Smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                Smtp.UseDefaultCredentials = false;
                Smtp.Host = this.ServerSmtp;
                Smtp.Credentials = Credentials;

                Email.IsBodyHtml = false;
                //Email.AlternateViews.Add(new AlternateView(this.Message, new ContentType("text/html")));

                if (this.Ssl)
                    Smtp.EnableSsl = true;               

                //TITULO MENSAJE
                Email.Subject = this.TitleMessage;
                //MENSAJE
                Email.Body = this.Message;
                //ENVIAR MAIL
                Smtp.Send(Email);

                exito = true;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                exito = false;
            }

            return exito;

        }

        /*OBTENER LA CONFIGURACION GUARDAR EN BD*/
        /*SETEAMOS LAS VARIABLES*/
        /// <summary>
        /// Obtiene la configuracion de correo electronico desde bd.
        /// </summary>
        public void SetConfiguration()
        {
            string sql = "SELECT TOP 1 mailserver, password, port, ssl, smtpserver FROM correo";
            SqlCommand cmd;
            SqlDataReader rd;
            SqlConnection cn;
            Cifrado cipher = new Cifrado();
            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            rd = cmd.ExecuteReader();
                            if (rd.HasRows)
                            {
                                while (rd.Read())
                                {
                                    /*SETEAMOS VARIABLES*/
                                    this.EmailServer = cipher.DesencriptaTripleDesc((string)rd["mailserver"]);
                                    this.PasswordServer = cipher.DesencriptaTripleDesc((string)rd["password"]);
                                    this.PuertoSmtp = Convert.ToInt32(rd["port"]);
                                    this.Ssl = (bool)rd["ssl"];
                                    this.ServerSmtp = cipher.DesencriptaTripleDesc((string)rd["smtpserver"]);
                                }
                            }
                        }
                        cmd.Dispose();
                        rd.Close();
                    }
                }              
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

    }

    /*BARRA DE PROGRESO*/
    class BarraProgreso
    {
        /*MOSTRAR OCULTAR CONTROL*/
        /// <summary>
        /// Indica si se muestra u oculta control.
        /// </summary>
        public bool ShowControl = false;

        //DELEGADO PARA ACCEDER A LA BARRA DE PROGRESO DESDE UN HILO
        /// <summary>
        /// Delegado para setear las propiedades de la barra de otro hilo de ejecución.
        /// </summary>
        delegate void PropiedadesBarraDelegado();

        //DELEGADO PARA MANIPULAR EL INCREMENTO DE LA BARRA DE PROGRESO
        /// <summary>
        /// Delegado que permite aumentar el progreso de la barra desde otro hilo de ejecucion.
        /// </summary>
        delegate void AumentoBarraDelegado();

        //DELEGADO PARA MOSTRAR U OCULTAR CONTROL
        /// <summary>
        /// Delegado que permite ocultar u mostrar la barra de progreso.
        /// </summary>
        delegate void ShowHideDelegado();

        //RESET PROGRESSBAR
        /// <summary>
        /// Permite resetear la barra desde otro hilo de ejecucion.
        /// </summary>
        delegate void ResetProgressBar();

        //CERRAR FORMULARIO
        /// <summary>
        /// Delegado que permite cerrar el formulario que invoca.
        /// </summary>
        delegate void CerrarFormDelegate();

        /// <summary>
        /// Fomulario en el cual se utiliza el control.
        /// </summary>
        XtraForm Formulario;

        //CONTROL PROPIAMENTE TAL
        /// <summary>
        /// ProgressBarControl.
        /// </summary>
        ProgressBarControl Barra;

        //PROPIEDADES 
        public int Step = 0;
        public bool ShowPercent = false;
        public bool ShowTitle = false;
        public int Maximum = 100;

        public BarraProgreso()
        { }

        public BarraProgreso(ProgressBarControl pProgressBar, int pStep, bool pShowPercent, bool pShowTitle, XtraForm pForm)
        {
            this.Formulario = pForm;
            this.Barra = pProgressBar;
            this.Step = pStep;
            this.ShowPercent = pShowPercent;
            this.ShowTitle = pShowTitle;        
        }

        /// <summary>
        /// Permite setear las propiedades de un control ProgressBarControl.
        /// </summary>
        public void Begin()
        {
            if (this.Formulario.InvokeRequired)
            {
                PropiedadesBarraDelegado propiedades = new PropiedadesBarraDelegado(Begin);

                this.Formulario.Invoke(propiedades);
            }
            else
            {
                this.Barra.Visible = this.ShowControl;
                this.Barra.EditValue = 0;
                this.Barra.Properties.ShowTitle = this.ShowTitle;
                this.Barra.Properties.Maximum = this.Maximum;
                this.Barra.Properties.Step = this.Step;
                this.Barra.Properties.PercentView = this.ShowPercent;
                this.Barra.Properties.ProgressViewStyle = DevExpress.XtraEditors.Controls.ProgressViewStyle.Solid;                
            }
        }

        /*AUMENTAR BARRA DE ACUERTO A PROPIEDAD STEP*/
        /// <summary>
        /// Incrementa la posicion de la barra de progreso.
        /// </summary>
        public void Increase()
        {
            if (this.Formulario.InvokeRequired)
            {
                AumentoBarraDelegado aumento = new AumentoBarraDelegado(Increase);

                this.Formulario.Invoke(aumento);
            }
            else
            {
                this.Barra.PerformStep();
                this.Barra.Update();
            }          
        }

        /*VISIBILIDAD CONTROL*/
        /// <summary>
        /// Muestra u oculta la barra de progreso.
        /// </summary>
        public void ShowClose()
        {
            if (this.Formulario.InvokeRequired)
            {
                ShowHideDelegado show = new ShowHideDelegado(ShowClose);
                
                this.Formulario.Invoke(show);
            }
            else
            {
                //SETEAMOS 
                this.Barra.Visible = this.ShowControl;
            }
        }

        /*RESET PROGRESS BAR*/
        /// <summary>
        /// Restaura la posicion de la barra de progreso.
        /// </summary>
        public void Reset()
        {
            if (this.Formulario.InvokeRequired)
            {
                ResetProgressBar reset = new ResetProgressBar(Reset);

                this.Formulario.Invoke(reset);
            }
            else
            {
                this.Barra.EditValue = 0;
            }
        }

        /*CERRAR FORMULARIO*/
        /// <summary>
        /// Permite cerrar el formulario que invocó la barra de progreso.
        /// </summary>
        public void CloseForm()
        {
            if (this.Formulario.InvokeRequired)
            {
                CerrarFormDelegate cerrar = new CerrarFormDelegate(CloseForm);

                this.Formulario.Invoke(cerrar);
            }
            else
            {
                this.Formulario.Close();
            }
        }
    }

    /// <summary>
    /// Clase para gestionar mensajes de alert
    /// </summary>
    class Alerta
    {
        /// <summary>
        /// Delegado para llamar al metodo Show del control desde otro hilo.
        /// </summary>
        delegate void Show();
        /// <summary>
        /// Titulo del cuadro
        /// </summary>
        public string Titulo { get; set; } = $"Sopytec Remuneraciones \u00a9 {DateTime.Now.Date.Year}";
        /// <summary>
        /// Mensaje de la alerta
        /// </summary>
        public string Mensaje { get; set; }
        /// <summary>
        /// Imagen alerta
        /// </summary>
        public Image Imagen { get; set; } = Labour.Properties.Resources.notificationX24;

        /// <summary>
        /// Formulario desde el cual se invoca la alerta.
        /// </summary>
        public XtraForm Formulario { get; set; }

        /// <summary>
        /// Representa un control AlertControl
        /// </summary>
        public AlertControl AControl { get; set; }

        /// <summary>
        /// Nos indica si se detiene las notificaciones.
        /// </summary>
        public static bool StopAlerts { get; set; } = false;

        /// <summary>
        /// Para guardar el temporizador que inicia los monitoreos o notificaciones.
        /// </summary>
        public static System.Windows.Forms.Timer StatusMonitor { get; set; }

        public Alerta()
        { }

        /// <summary>
        /// Constructor clase.
        /// </summary>
        /// <param name="Title"></param>
        /// <param name="Message"></param>
        /// <param name="pForm"></param>
        /// <param name="pControl"></param>
        public Alerta(string Title, string Message, XtraForm pForm, AlertControl pControl)
        {
            this.Formulario = pForm;
            this.Mensaje = Message;
            this.Titulo = Title;
            this.AControl = pControl;                       
        }     

        /// <summary>
        /// Muestra alerta con mensaje.
        /// </summary>
        public void ShowMessage()
        {
            if (this.Formulario != null && this.Mensaje != "" && this.Titulo != "" && this.AControl != null)
            {
                if (this.Formulario.InvokeRequired)
                {
                    Show sh = new Show(ShowMessage);
                    this.Formulario.Invoke(sh);
                    
                }
                else
                {
                    this.AControl.Show(this.Formulario, this.Titulo, this.Mensaje, this.Imagen);
                }             
               
            }
        }
        
    }

    /// <summary>
    /// Clase para gestionar los datos de entradas necesarios para funcionamiento del sistema.
    /// </summary>
    class Configuracion
    {

        public static Configuracion ConfiguracionGlobal = new Configuracion();
        /// <summary>
        /// Representa la ruta donde se debe realizar el respaldo de la base de datos.
        /// </summary>
        public string RutaRespaldo { get; set; }
        /// <summary>
        /// Representa la ruta de plantilla para creacion de finiquitos.
        /// </summary>
        public string RutaPlantillaFiniquito { get; set; }
        /// <summary>
        /// Representa la ruta de plantilla para creacion de contratos.
        /// </summary>
        public string RutaPlantillaContrato { get; set; }
        /// <summary>
        /// Representa la ruta de plantilla para creacion de cartas de aviso.
        /// </summary>
        public string RutaPlantillaAviso { get; set; }

        /// <summary>
        /// Porcentaje entre 1 y 100 de reajuste de leyes sociales para suspension laboral
        /// </summary>
        public int PorcentajeSuspension { get; set; }

        /// <summary>
        /// Ingresa un nuevo registro
        /// </summary>
        /// <param name="pRuta"></param>
        public bool Nuevo(string pRutaRespaldo, string pRutaFin, string pRutacon, string pRutaAviso, int pPorcentaje)
        {
            string sql = "INSERT INTO configuracion(filerp, fileplfin, fileplcon, fileplaviso, porsuspen) VALUES(@pFilerp, @pFilefin, @pFilecon, @pFileaviso, @pPorcentaje)";
            SqlCommand cmd;
            SqlConnection cn;
            int count = 0;
            bool correcto = false;

            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@pFilerp", pRutaRespaldo));
                            cmd.Parameters.Add(new SqlParameter("@pFilefin", pRutaFin));
                            cmd.Parameters.Add(new SqlParameter("@pFilecon", pRutacon));
                            cmd.Parameters.Add(new SqlParameter("@pFileaviso", pRutaAviso));
                            cmd.Parameters.Add(new SqlParameter("@pPorcentaje", pPorcentaje));

                            count = cmd.ExecuteNonQuery();
                            if (count > 0)
                            {
                                logRegistro log = new logRegistro(User.getUser(), "SE INGRESA REGISTRO EN TABLA CONFIGURACION", "CONFIGURACION", "", pRutaRespaldo, "INGRESAR");
                                log.Log();

                                correcto = true;
                            }
                        }
                        cmd.Dispose();
                    }
                }
            }
            catch (SqlException ex)
            {
                //ERROR...
                correcto = false;
            }

            return correcto;
        }

        /// <summary>
        /// Modifica un registro
        /// </summary>
        /// <param name="pRuta"></param>
        public bool Modifica(string pRutaRespaldo, string pRutaFin, string pRutacon, string pRutaAviso, int pPorcentaje)
        {
            string sql = "UPDATE configuracion SET filerp=@pFilerp, fileplfin=@pFilefin, " +
                "fileplcon=@pFilecon, fileplaviso=@pFileaviso, porsuspen=@pPorcentaje";
            SqlCommand cmd;
            SqlConnection cn;
            int count = 0;
            bool correcto = false;

            Hashtable data = new Hashtable();
            data = DataReg();

            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@pFilerp", pRutaRespaldo));
                            cmd.Parameters.Add(new SqlParameter("@pFilefin", pRutaFin));
                            cmd.Parameters.Add(new SqlParameter("@pFilecon", pRutacon));
                            cmd.Parameters.Add(new SqlParameter("@pFileaviso", pRutaAviso));
                            cmd.Parameters.Add(new SqlParameter("@pPorcentaje", pPorcentaje));

                            count = cmd.ExecuteNonQuery();
                            if (count > 0)
                            {
                                correcto = true;
                                WriteEvent(pRutaRespaldo,pRutaFin, pRutacon, pRutaAviso, data);
                            }
                        }
                        cmd.Dispose();
                    }
                }
            }
            catch (SqlException ex)
            {
                //ERROR...
                correcto = false;
            }

            return correcto;
        }

        /// <summary>
        /// Indica si hay datos en tabla.
        /// </summary>
        public bool Existe()
        {
            string sql = "SELECT count(*) FROM configuracion";
            SqlCommand cmd;
            SqlConnection cn;
            bool existe = false;
            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            object data = cmd.ExecuteScalar();
                            if (data != DBNull.Value)
                            {
                                if (Convert.ToInt32(data) > 0)
                                    existe = true;
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                //ERROR...
                existe = false;
            }

            return existe;
        }

        #region "LOG"
        /// <summary>
        /// Registra un evento en log de cambios.
        /// </summary>
        /// <param name="pRutaRespaldo"></param>
        /// <param name="pData"></param>
        private void WriteEvent(string pRutaRespaldo,string pFilefin, string pFilecon, string pFileAviso, Hashtable pData)
        {
            if (pData.Count > 0)
            {
                if ((string)pData["filerp"] != pRutaRespaldo)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICAR CAMPO FILERP", "CONFIGURACION", (string)pData["filerp"], pRutaRespaldo, "MODIFICAR");
                    log.Log();
                }
                if ((string)pData["fileplfin"] != pFilefin)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICAR CAMPO FILEPLFIN", "CONFIGURACION", (string)pData["fileplfin"], pFilefin, "MODIFICAR");
                    log.Log();
                }
                if ((string)pData["fileplcon"] != pFilecon)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICAR CAMPO FILEPLCON", "CONFIGURACION", (string)pData["fileplcon"], pFilecon, "MODIFICAR");
                    log.Log();
                }
                if ((string)pData["fileplaviso"] != pFileAviso)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICAR CAMPO FILEPLAVISO", "CONFIGURACION", (string)pData["fileplaviso"], pFileAviso, "MODIFICAR");
                    log.Log();
                }
            }
        }

        /// <summary>
        /// Devuelve un hashtable con informacion de configuracion.
        /// </summary>
        /// <returns></returns>
        private Hashtable DataReg()
        {
            string sql = "SELECT TOP 1 filerp, fileplfin, fileplcon, fileplaviso FROM configuracion";
            SqlCommand cmd;
            SqlConnection cn;
            SqlDataReader rd;
            Hashtable data = new Hashtable();
            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            rd = cmd.ExecuteReader();
                            if (rd.HasRows)
                            {
                                while (rd.Read())
                                {
                                    data.Add("filerp", (string)rd["filerp"]);
                                    data.Add("filefin", (string)rd["fileplfin"]);
                                    data.Add("filecon", (string)rd["fileplcon"]);
                                    data.Add("fileaviso", (string)rd["fileplaviso"]);
                                }
                            }
                        }
                        rd.Close();
                        cmd.Dispose();
                    }
                }
            }
            catch (SqlException ex)
            {
              //ERROR...
            }

            return data;
        }
        #endregion

        /// <summary>
        /// Permite setear toda la informacion de la tabla en las propiedades correspondientes.
        /// </summary>
        public void SetConfiguracion()
        {
            string sql = "SELECT top 1 filerp, fileplfin, fileplcon, fileplaviso, porsuspen FROM configuracion";
            SqlCommand cmd;
            SqlDataReader rd;
            SqlConnection cn;
            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            rd = cmd.ExecuteReader();
                            if (rd.HasRows)
                            {
                                while (rd.Read())
                                {
                                    RutaRespaldo = (string)rd["filerp"];
                                    RutaPlantillaFiniquito = (string)rd["fileplfin"];
                                    RutaPlantillaContrato = (string)rd["fileplcon"];
                                    RutaPlantillaAviso = (string)rd["fileplaviso"];
                                    PorcentajeSuspension = Convert.ToInt32(rd["porsuspen"]);
                                }
                            }
                        }
                        cmd.Dispose();
                        rd.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                //ERROR...
            }
        }

        public static void SetGlobalConfiguration()
        {
            string sql = "SELECT top 1 filerp, fileplfin, fileplcon, fileplaviso, porsuspen FROM configuracion";
            SqlCommand cmd;
            SqlDataReader rd;
            SqlConnection cn;
            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            rd = cmd.ExecuteReader();
                            if (rd.HasRows)
                            {
                                while (rd.Read())
                                {
                                    ConfiguracionGlobal.RutaRespaldo = (string)rd["filerp"];
                                    ConfiguracionGlobal.RutaPlantillaFiniquito = (string)rd["fileplfin"];
                                    ConfiguracionGlobal.RutaPlantillaContrato = (string)rd["fileplcon"];
                                    ConfiguracionGlobal.RutaPlantillaAviso = (string)rd["fileplaviso"];
                                    ConfiguracionGlobal.PorcentajeSuspension = Convert.ToInt32(rd["porsuspen"]);
                                }
                            }
                        }
                        cmd.Dispose();
                        rd.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                //ERROR...
            }
        }





        
    }
}
