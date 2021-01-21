using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using System.Data.SqlClient;
using DevExpress.Utils.Menu;

namespace Labour
{
    public partial class frmBuscarTrabajador : DevExpress.XtraEditors.XtraForm
    {
        //PARA OBTENER EL FILTRO DEL USER LOGUEADO
        private string FiltroUsuario { get; set; } = User.GetUserFilter();

        //PARA SABER SI BUSCA USUARIOS INACTIVOS
        public bool ShowInactivos { get; set; }

        //PARA SABER SI BUSCA USUARIO ACTIVOS
        public bool ShowActivos { get; set; }

        //USUARIO PUEDE VER FICHAS PRIVADAS
        public bool ShowPrivados { get; set; } = User.ShowPrivadas();

        /// <summary>
        /// Consulta base
        /// </summary>
        string sqlBase = "SELECT rut, CONCAT(apepaterno, ' ', apematerno, ' ', nombre) " +
                     $"as nombre, MAX(anomes) as anomes, contrato, IIF(MAX(anomes)<{Calculo.PeriodoObservado},0,status) as status FROM trabajador ";

        string sqlBaseSimple = "SELECT rut, CONCAT(apepaterno, ' ', apematerno, ' ', nombre) " +
                     "as nombre, anomes,contrato, status FROM trabajador ";


        public IBuscarTrabajador opener { set; get; }
        public frmBuscarTrabajador()
        {
            InitializeComponent();
          
        }
        private void frmBuscarTrabajador_Load(object sender, EventArgs e)
        {            
            txtbusqueda.Properties.MaxLength = 50;

            fnBuscarTrabajadorV2("");

            //fnSistema.spllenaGridView(gridTrabajdor,grilla);
            //propiedades grilla
            fnSistema.spOpcionesGrilla(viewTrabajador);
            //caption para las columnas
            fnColumnasGrilla();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            //LLAMAMOS A LA FUNCION BUSCAR TRABAJADOR
            fnBuscarTrabajadorV2(txtbusqueda.Text);
        }
        //SOLO NUMEROS EN CAJA RUT        
        //METODO PARA LIMPIAR CAMPOS
        private void fnLimpiarCampos()
        {
            txtbusqueda.Text = "";
            txtbusqueda.Focus();
        }

        #region "MANEJO DATOS"
        //METODO PARA BUSQUEDA DE TRABAJADOR
        private void fnBuscarTrabajadorV2(string pBusqueda)
        {
            //VARIABLE PARA GUARDAR EL PERIODO EN EVALUACION

            string sqlFiltro = "";
            string sql = "";
            string SqlLike = $" AND (nombre LIKE '%{pBusqueda}%' OR apepaterno LIKE '%{pBusqueda}%' OR apematerno LIKE '%{pBusqueda}%' OR contrato LIKE '%{pBusqueda}%')";

            sqlFiltro = Calculo.GetSqlFiltro(FiltroUsuario, "", ShowPrivados);

            if (pBusqueda.Length > 0)
            {
                if (ShowInactivos && ShowActivos)
                {
                    sql = sqlBase + $" WHERE ((contrato " +
                                    $"NOT IN (SELECT contrato FROM trabajador GROUP by contrato HAVING MAX(anomes) = {Calculo.PeriodoObservado})) " +
                                    $"OR (status = 0 AND anomes={Calculo.PeriodoObservado}) OR (status = 1 AND anomes={Calculo.PeriodoObservado})) {sqlFiltro} {SqlLike}" +
                                    $"GROUP BY contrato, rut, nombre, apepaterno, apematerno, status " +
                                    $"ORDER BY apepaterno ";
                }
                else if (ShowInactivos)
                {
                    sql = sqlBase + $" WHERE contrato " +
                                    $"NOT IN (SELECT contrato FROM trabajador GROUP BY contrato HAVING MAX(anomes) = {Calculo.PeriodoObservado}) OR (status=0 AND anomes={Calculo.PeriodoObservado}) {sqlFiltro} {SqlLike}" +
                                    $"GROUP BY contrato, rut, nombre, apepaterno, apematerno, status " +
                                    $" ORDER BY apepaterno";
                }
                else
                {
                    sql = sqlBaseSimple + $" WHERE status = 1 AND anomes = {Calculo.PeriodoObservado} {sqlFiltro} {SqlLike} ORDER BY apepaterno ";
                }
            }
            else
            {
                if (ShowInactivos && ShowActivos)
                {
                    sql = sqlBase + $" WHERE (contrato " +
                                    $"NOT IN (SELECT contrato FROM trabajador GROUP by contrato HAVING MAX(anomes) = {Calculo.PeriodoObservado})) " +
                                    $"OR (status = 0 AND anomes={Calculo.PeriodoObservado}) OR (status = 1 AND anomes={Calculo.PeriodoObservado}) {sqlFiltro}" +
                                    $"GROUP BY contrato, rut, nombre, apepaterno, apematerno, status " +
                                    $"ORDER BY apepaterno ";
                }
                else if (ShowInactivos)
                {
                    sql = sqlBase + $" WHERE contrato " +
                                    $"NOT IN (SELECT contrato FROM trabajador GROUP BY contrato HAVING MAX(anomes) = {Calculo.PeriodoObservado}) OR (status=0 AND anomes={Calculo.PeriodoObservado}) {sqlFiltro}" +
                                    $"GROUP BY contrato, rut, nombre, apepaterno, apematerno, status " +
                                    $" ORDER BY apepaterno";
                }
                else
                {
                    sql = sqlBaseSimple + $" WHERE status = 1 AND anomes = {Calculo.PeriodoObservado} {sqlFiltro} ORDER BY apepaterno ";
                }
            }

            fnRecargarGrilla(gridTrabajdor, sql);
            fnColumnasGrilla();
            //ChangeValueCell();
        }

        private void fnBuscarTrabajador(string pDato, string filtro)
        {
            string condicion = "";
            //VARIABLE PARA GUARDAR EL PERIODO EN EVALUACION
            //int PeriodoEvaluacion = 0;
            //PeriodoEvaluacion = Calculo.PeriodoObservado;

            #region "SI NO SE INGRESA NADA EN CAJA BUSQUEDA"
            if (pDato == "")
            {
                //XtraMessageBox.Show("Busqueda no Valida");
                string grilla = "";
                if (filtro == "0")
                {
                    if (ShowInactivos && ShowActivos)
                        grilla = "SELECT contrato, rut, CONCAT(apepaterno, ' ', apematerno, ' ', nombre) as nombre, MAX(anomes) as anomes, status  " +
                                 "FROM trabajador " +
                                 $"WHERE((contrato NOT IN(SELECT contrato FROM trabajador GROUP BY contrato HAVING MAX(anomes) = {Calculo.PeriodoObservado})) " +
                                 $"OR status = 1 OR status=0) {(ShowPrivados == false? " AND privado=0":"")}" +
                                 "GROUP BY contrato, rut, nombre, apepaterno, apematerno, status " +
                                 $" ORDER BY apepaterno";
                    else if (ShowInactivos)
                        grilla = "SELECT contrato, rut, CONCAT(apepaterno, ' ', apematerno, ' ', nombre) as nombre, MAX(anomes) as anomes, status " +
                                 "FROM trabajador " +
                                 $"WHERE contrato NOT IN (SELECT contrato FROM trabajador GROUP BY contrato HAVING MAX(anomes) = {Calculo.PeriodoObservado}) OR status=0 {(ShowPrivados == false?" AND privado = 0":"")}" +
                                 "GROUP BY contrato, rut, nombre, apepaterno, apematerno, status " +
                                 $" ORDER BY apepaterno";
                    else
                        grilla = "SELECT contrato, rut, CONCAT(apepaterno, ' ', apematerno, ' ', nombre) as nombre, anomes, status " +
                                 "FROM trabajador " +
                                 $"WHERE status = 1 AND anomes = {Calculo.PeriodoObservado} {(ShowPrivados == false?" AND privado=0":"")} " +
                                 $" ORDER BY apepaterno";

                    //grilla = string.Format("SELECT contrato, rut, CONCAT(nombre, ' ', apepaterno, ' ',apematerno) as nombre, anomes FROM trabajador WHERE anomes={0} " + (ShowInactivos ? "" : "AND status=1") + (ShowPrivados == false ? " AND privado=0" : "") + " ORDER BY nombre", PeriodoEvaluacion);
                }
                else
                {
                    condicion = Conjunto.GetCondicionFromCode(filtro);
                    if (ShowInactivos && ShowActivos)
                        grilla = "SELECT contrato, rut, CONCAT(apepaterno, ' ', apematerno, ' ', nombre) as nombre, MAX(anomes) as anomes, status  " +
                                "FROM trabajador " +
                                $"WHERE((contrato NOT IN(SELECT contrato FROM trabajador GROUP BY contrato HAVING MAX(anomes) = {Calculo.PeriodoObservado})) " +
                                $"OR status = 1 OR status = 0) AND {condicion} {(ShowPrivados == false? " AND privado=0":"")}" +
                                " GROUP BY contrato, rut, nombre, apepaterno, apematerno, status " +
                                $" ORDER BY apepaterno";
                    else if(ShowInactivos)
                        grilla = "SELECT contrato, rut, CONCAT(apepaterno, ' ', apematerno, ' ', nombre) as nombre, MAX(anomes) as anomes, status  " +
                                 "FROM trabajador " +
                                 $"WHERE contrato NOT IN(SELECT contrato FROM trabajador GROUP BY contrato HAVING MAX(anomes) = {Calculo.PeriodoObservado}) OR status = 0 AND {condicion} {(ShowPrivados == false?" AND privado=0":"")}" +
                                 "GROUP BY contrato, rut, nombre, apepaterno, apematerno, status " +
                                 $" ORDER BY apepaterno";
                    else
                        grilla = "SELECT contrato, rut, CONCAT(apepaterno, ' ', apematerno, ' ', nombre) as nombre, anomes, status " +
                                 "FROM trabajador " +
                                 $"WHERE status = 1 AND anomes = {Calculo.PeriodoObservado} AND {condicion} {(ShowPrivados == false?" AND privado=0":"")} " +
                                 $" ORDER BY apepaterno";

                    //grilla = string.Format("SELECT contrato, rut, CONCAT(nombre, ' ', apepaterno, ' ',apematerno) as nombre, anomes FROM trabajador WHERE anomes={0} AND {1} " + (ShowInactivos ? "" : "AND status=1") + (ShowPrivados == false ? " AND privado=0" : "") + " ORDER BY nombre", PeriodoEvaluacion, condicion);                    
                }

                fnSistema.spllenaGridView(gridTrabajdor, grilla);
                groupResultados.Text = "Resultados";
                return;
            }
            #endregion

            string busqueda = "", sqlNombre = "", sqlRut = "";

            //SI SE BUSCA POR NOMBRE
            if (filtro == "0")
            {
                if (ShowInactivos && ShowActivos)
                    sqlNombre = "SELECT contrato, rut, CONCAT(apepaterno, ' ', apematerno, ' ', nombre) as nombre, MAX(anomes) as anomes, status " +
                        $"FROM trabajador WHERE (nombre LIKE '%{pDato}%' OR apepaterno LIKE '%{pDato}%' OR apematerno LIKE '%{pDato}%') " +
                        $"AND ((contrato NOT IN (SELECT contrato FROM trabajador GROUP BY contrato HAVING MAX(anomes) = {Calculo.PeriodoObservado})) " +
                        $"OR status = 1 OR status=0)  {(ShowPrivados == false?" AND privado=0":"")}" +
                        $"GROUP BY contrato, rut, nombre, apepaterno, apematerno, status " +
                        $" ORDER BY apepaterno";
                else if (ShowInactivos)
                    sqlNombre = "SELECT contrato, rut, CONCAT(apepaterno, ' ', apematerno, ' ', nombre) as nombre, anomes, status " +
                        $"FROM trabajador WHERE (nombre LIKE '%{pDato}%' OR apepaterno LIKE '%{pDato}%' OR apematerno LIKE '%{pDato}%') " +
                        $"AND contrato NOT IN (SELECT contrato FROM trabajador GROUP BY contrato HAVING MAX(anomes) = {Calculo.PeriodoObservado}) OR status=0 {(ShowPrivados == false? " AND privado=0":"")}" +
                        "GROUP BY contrato, rut, nombre, apepaterno, apematerno, status " +
                        $" ORDER BY apepaterno";
                else
                    sqlNombre = "SELECT contrato, rut, CONCAT(apepaterno, ' ', apematerno, ' ', nombre) as nombre, anomes, status " +
                        $"FROM trabajador WHERE (nombre LIKE '%{pDato}%' OR apepaterno LIKE '%{pDato}%' OR apematerno LIKE '%{pDato}%') " +
                        $"AND status = 1 AND anomes = {Calculo.PeriodoObservado} {(ShowPrivados == false?" AND privado=0":"")} "+
                        $" ORDER BY apepaterno";

                //  sqlNombre = string.Format("SELECT contrato, rut, CONCAT(nombre, ' ', apepaterno, ' ', apematerno) as nombre, anomes  " +
                //  "FROM trabajador WHERE (nombre LIKE '%{0}%' OR apepaterno LIKE '%{1}%' OR apematerno LIKE '%{2}%') AND anomes={3} " + (ShowInactivos? "" : "AND status=1") + (ShowPrivados == false ? " AND privado=0" : "") + " ORDER BY nombre", pDato, pDato, pDato, PeriodoEvaluacion);
            }                
            else
            {
                condicion = Conjunto.GetCondicionFromCode(filtro);
                if (ShowInactivos && ShowActivos)
                    sqlNombre = "SELECT contrato, rut, CONCAT(apepaterno, ' ', apematerno, ' ', nombre) as nombre, MAX(anomes) as anomes, status " +
                        $"FROM trabajador WHERE (nombre LIKE '%{pDato}%' OR apepaterno LIKE '%{pDato}%' OR apematerno LIKE '%{pDato}%') " +
                        $"AND ((contrato NOT IN (SELECT contrato FROM trabajador GROUP BY contrato HAVING MAX(anomes) = {Calculo.PeriodoObservado})) " +
                        $"OR status = 1 OR status=0) AND {condicion} {(ShowPrivados == false?" AND privado=0":"")}" +
                        $"GROUP BY contrato, rut, nombre, apepaterno, apematerno, status " +
                        $" ORDER BY apepaterno";
                else if (ShowInactivos)
                    sqlNombre = "SELECT contrato, rut, CONCAT(apepaterno, ' ', apematerno, ' ', nombre) as nombre, anomes, status " +
                        $"FROM trabajador WHERE (nombre LIKE '%{pDato}%' OR apepaterno LIKE '%{pDato}%' OR apematerno LIKE '%{pDato}%') " +
                        $"AND contrato NOT IN (SELECT contrato FROM trabajador GROUP BY contrato HAVING MAX(anomes) = {Calculo.PeriodoObservado}) OR status=0 " +
                        $"AND {condicion} {(ShowPrivados == false?" AND privado=0":"")}" +
                        "GROUP BY contrato, rut, nombre, apepaterno, apematerno, status " +
                        $" ORDER BY apepaterno";
                else
                    sqlNombre = "SELECT contrato, rut, CONCAT(apepaterno, ' ', apematerno, ' ', nombre) as nombre, anomes, status " +
                         $"FROM trabajador WHERE (nombre LIKE '%{pDato}%' OR apepaterno LIKE '%{pDato}%' OR apematerno LIKE '%{pDato}%') " +
                         $"AND status = 1 AND anomes = {Calculo.PeriodoObservado} AND {condicion} {(ShowPrivados == false?" AND privado=0":"")} " +
                         $" ORDER BY apepaterno";

                //sqlNombre = string.Format("SELECT contrato, rut, CONCAT(nombre, ' ', apepaterno, ' ', apematerno) as nombre, anomes  " +
                //"FROM trabajador WHERE (nombre LIKE '%{0}%' OR apepaterno LIKE '%{1}%' OR apematerno LIKE '%{2}%') AND anomes={3} AND {4} " + (ShowInactivos ? "" : "AND status=1") + (ShowPrivados == false ? " AND privado=0" : "") + " ORDER BY nombre", pDato, pDato, pDato, PeriodoEvaluacion, condicion);
            }

            //SI SE BUSCA POR RUT
            if (filtro == "0")
            {
                if (ShowInactivos && ShowActivos)
                    sqlRut = $"SELECT contrato, rut, CONCAT(apepaterno, ' ', apematerno, ' ', nombre) as nombre, MAX(anomes) as anomes, status " +
                        $"FROM trabajador WHERE rut LIKE '%{pDato}%' " +
                        $"AND ((contrato NOT IN (SELECT contrato FROM trabajador GROUP BY contrato HAVING MAX(anomes) = {Calculo.PeriodoObservado})) " +
                        $"OR status = 1 OR status=0) {(ShowPrivados == false ? " AND privado=0" : "")}" +
                        $"GROUP BY contrato, rut, nombre, apepaterno, apematerno, status "+
                        $" ORDER BY apepaterno";
                else if (ShowInactivos)
                    sqlRut = $"SELECT contrato, rut, CONCAT(apepaterno, ' ', apematerno, ' ', nombre) as nombre, anomes, status " +
                        $"FROM trabajador WHERE rut LIKE '%{pDato}%' " +
                        $"AND contrato NOT IN (SELECT contrato FROM trabajador GROUP BY contrato HAVING MAX(anomes) = {Calculo.PeriodoObservado}) OR status=0 {(ShowPrivados == false ? " AND privado=0" : "")}" +
                        "GROUP BY contrato, rut, nombre, apepaterno, apematerno, status " +
                        $" ORDER BY apepaterno";
                else
                    sqlRut = $"SELECT contrato, rut, CONCAT(apepaterno, ' ', apematerno, ' ', nombre) as nombre, anomes, status " +
                       $"FROM trabajador WHERE rut LIKE '%{pDato}%' " +
                       $"AND status = 1 AND anomes = {Calculo.PeriodoObservado} {(ShowPrivados == false ? " AND privado=0" : "")} " +
                       $" ORDER BY apepaterno";

                //sqlRut = string.Format("SELECT contrato, rut, CONCAT(nombre, ' ', apepaterno, ' ', apematerno) as nombre, anomes  " +
                //"FROM trabajador WHERE rut LIKE '%{0}%' AND anomes={1} " + (ShowInactivos ? "" : "AND status=1") + (ShowPrivados == false ? " AND privado=0" : "") + " ORDER BY nombre", pDato, PeriodoEvaluacion);
            }                
            else
            {
                condicion = Conjunto.GetCondicionFromCode(filtro);
                if (ShowInactivos && ShowActivos)
                    sqlRut = $"SELECT contrato, rut, CONCAT(apepaterno, ' ', apematerno, ' ', nombre) as nombre, MAX(anomes) as anomes, status " +
                        $"FROM trabajador WHERE rut LIKE '%{pDato}%' " +
                        $"AND ((contrato NOT IN (SELECT contrato FROM trabajador GROUP BY contrato HAVING MAX(anomes) = {Calculo.PeriodoObservado})) " +
                        $"OR status = 1 OR status=0) AND {condicion} {(ShowPrivados == false?" AND privado=0":"")}" +
                        $"GROUP BY contrato, rut, nombre, apepaterno, apematerno, status " + 
                        $" ORDER BY apepaterno";
                else if(ShowInactivos)
                    sqlRut = $"SELECT contrato, rut, CONCAT(apepaterno, ' ', apematerno, ' ', nombre) as nombre, anomes, status " +
                       $"FROM trabajador WHERE rut LIKE '%{pDato}%' " +
                       $"AND contrato NOT IN (SELECT contrato FROM trabajador GROUP BY contrato HAVING MAX(anomes) = {Calculo.PeriodoObservado}) OR status=0 AND {condicion} {(ShowPrivados == false?" AND privado=0":"")}" +
                       "GROUP BY contrato, rut, nombre, apepaterno, apematerno, status " +
                       $" ORDER BY apepaterno";
                else
                    sqlRut = $"SELECT contrato, rut, CONCAT(apepaterno, ' ', apematerno, ' ', nombre) as nombre, anomes, status " +
                       $"FROM trabajador WHERE rut LIKE '%{pDato}%' " +
                       $"AND status = 1 AND anomes = {Calculo.PeriodoObservado} AND {condicion} {(ShowPrivados == false?" AND privado=0":"")} "+
                       $" ORDER BY apepaterno";

                //sqlRut = string.Format("SELECT contrato, rut, CONCAT(nombre, ' ', apepaterno, ' ', apematerno) as nombre, anomes  " +
                //"FROM trabajador WHERE rut LIKE '%{0}%' AND anomes={1} AND {2} " + (ShowInactivos ? "" : "AND status=1") + (ShowPrivados == false ? " AND privado=0" : "") + " ORDER BY nombre", pDato, PeriodoEvaluacion, condicion);
            }                

            int largo = pDato.Length;
            string sub, sub1;
            //PREGUNTAR SI EL DATO ES NUMERICO (COMO UN RUT) O LETRAS
            //CASE 1: BUSCAR POR RUT 
            //CASE 2: BUSCAR POR NOMBRE
            //------ VER SI ES RUT //
            if (largo == 9)
            {
                //PODRIA SER UN RUT                
                //extraemos el ultimo caracter
                sub = pDato.Substring(largo-1,1);               
                sub1 = pDato.Substring(0, largo-1);
                
                if (fnSistema.IsNumeric(sub) && fnSistema.IsNumeric(sub1))
                {
                    
                    //PODRIAMOS DECIR QUE ES UN RUT
                    busqueda = sqlRut;
                }
                else if (sub.Equals("k") && fnSistema.IsNumeric(sub1))
                {
                  
                    //PODRIAMOS DECIR QUE ES UN RUT
                    busqueda = sqlRut;
                }
                else if (fnSistema.IsNumeric(sub) == false && fnSistema.IsNumeric(sub1)==false)
                {
                    //son solo letras                   
                    busqueda = sqlNombre;
                }
                fnRecargarGrilla(gridTrabajdor, busqueda);
               
            }
            else if (largo == 8)
            {
                sub = pDato.Substring(largo-1, 1);
                
                sub1 = pDato.Substring(0, largo - 1);
               
              
                if (fnSistema.IsNumeric(sub) && fnSistema.IsNumeric(sub1))
                {
                    
                    //PODRIAMOS DECIR QUE ES UN RUT
                    busqueda = sqlRut;
                }
                else if (sub.Equals("k") && fnSistema.IsNumeric(sub1))
                {
                    
                    //PODRIAMOS DECIR QUE ES UN RUT
                    busqueda = sqlRut;
                }
                else if (fnSistema.IsNumeric(sub) == false && fnSistema.IsNumeric(sub1) == false)
                {
                    busqueda = sqlNombre;
                }
                fnRecargarGrilla(gridTrabajdor, busqueda);            
            }
            else if (largo !=9 || largo != 8)
            {
                if (fnSistema.IsNumeric(pDato))
                {
                    busqueda = sqlRut;
                    fnRecargarGrilla(gridTrabajdor, busqueda);
                  
                } 
                else {
                    //SI LA CADENA TIENE ESPACIOS, SEPARAMOS LAS PALABRAS
                    if (pDato.Contains(' '))
                    {
                        string[] palabras = pDato.Split(' ');
                        int l = palabras.Length;
                        for (int i = 0; i < l; i++)
                        {
                            //recorrer
                            if (filtro == "0")
                            {
                                if (ShowInactivos)
                                    sqlNombre = $"SELECT contrato, rut, CONCAT(nombre, ' ', apepaterno, ' ', apematerno) as nombre, MAX(anomes) " +
                                        $"FROM trabajador WHERE (nombre LIKE '%{palabras[i]}%' OR apepaterno LIKE '%{palabras[i]}%' OR apematerno LIKE '%{palabras[i]}%') " +
                                        $"AND salida < '{fnSistema.PrimerDiaMes(Calculo.PeriodoObservado)}' {(ShowPrivados == false ? " AND privado=0" : "")}" +
                                        $"GROUP BY contrato, rut, nombre, apepaterno, apematerno";
                                else
                                    sqlNombre = $"SELECT contrato, rut, CONCAT(nombre, ' ', apepaterno, ' ', apematerno) as nombre, anomes " +
                                        $"FROM trabajador WHERE (nombre LIKE '%{palabras[i]}%' OR apepaterno LIKE '%{palabras[i]}%' OR apematerno LIKE '%{palabras[i]}%') " +
                                        $"AND anomes={Calculo.PeriodoObservado} {(ShowPrivados == false ? " AND privado=0" : "")}";


                                //sqlNombre = string.Format("SELECT contrato, rut, CONCAT(nombre, ' ', apepaterno, ' ', apematerno)" +
                                //"as nombre, anomes FROM trabajador WHERE (nombre LIKE '%{0}%' OR apepaterno LIKE '%{1}%' OR" +
                                //" apematerno LIKE '%{2}%') AND anomes={3} " + (ShowInactivos ? "" : "AND status=1") + (ShowPrivados == false ? " AND privado=0" : "") + " ORDER BY nombre", palabras[i], palabras[i], palabras[i], PeriodoEvaluacion);
                            }                                
                            else
                            {
                                condicion = Conjunto.GetCondicionFromCode(filtro);

                                if (ShowInactivos)
                                    sqlNombre = $"SELECT contrato, rut, CONCAT(nombre, ' ', apepaterno, ' ', apematerno) as nombre, MAX(anomes) " +
                                        $"FROM trabajador WHERE (nombre LIKE '%{palabras[i]}%' OR apepaterno LIKE '%{palabras[i]}%' OR apematerno LIKE '%{palabras[i]}%') " +
                                        $"AND salida < '{fnSistema.PrimerDiaMes(Calculo.PeriodoObservado)}' AND {condicion} {(ShowPrivados == false ? " AND privado=0" : "")}" +
                                        $"GROUP BY contrato, rut, nombre, apepaterno, apematerno";
                                else
                                    sqlNombre = $"SELECT contrato, rut, CONCAT(nombre, ' ', apepaterno, ' ', apematerno) as nombre, anomes " +
                                        $"FROM trabajador WHERE (nombre LIKE '%{palabras[i]}%' OR apepaterno LIKE '%{palabras[i]}%' OR apematerno LIKE '%{palabras[i]}%') " +
                                        $"AND anomes={Calculo.PeriodoObservado} AND {condicion} {(ShowPrivados == false ? " AND privado=0" : "")}";

                                //sqlNombre = string.Format("SELECT contrato, rut, CONCAT(nombre, ' ', apepaterno, ' ', apematerno)" +
                                //"as nombre, anomes FROM trabajador WHERE (nombre LIKE '%{0}%' OR apepaterno LIKE '%{1}%' OR" +
                                //" apematerno LIKE '%{2}%') AND anomes={3} AND {4} " + (ShowInactivos ? "" : "AND status=1") + (ShowPrivados == false ? " AND privado=0" : "") + " ORDER BY nombre", palabras[i], palabras[i], palabras[i], PeriodoEvaluacion, condicion);
                            }                                

                            fnRecargarGrilla(gridTrabajdor, sqlNombre);                          
                        }
                    }
                    else
                    {
                        busqueda = sqlNombre;                        
                        fnRecargarGrilla(gridTrabajdor, busqueda);
                    }                   
                }
            }
            //MessageBox.Show(busqueda);
           // fnRecargarGrilla(gridTrabajdor, busqueda);
            //fnColumnasGrilla();         
        }

        //definir captions para las columnas de la grilla
        private void fnColumnasGrilla()
        {       

            viewTrabajador.Columns[0].Caption = "Rut";
            viewTrabajador.Columns[0].Width = 80;
            viewTrabajador.Columns[0].DisplayFormat.FormatString = "Rut";
            viewTrabajador.Columns[0].DisplayFormat.Format = new FormatCustom();

            viewTrabajador.Columns[1].Caption = "Trabajador";
            //viewTrabajador.Columns[1].AppearanceCell.FontStyleDelta = FontStyle.Bold;
            viewTrabajador.Columns[1].Width = 300;

            //PERIODO
            viewTrabajador.Columns[2].Visible = false;

            viewTrabajador.Columns[3].Caption = "Contrato";
            //viewTrabajador.Columns[3].AppearanceCell.FontStyleDelta = FontStyle.Bold;
            //viewTrabajador.Columns[0].OptionsColumn.AllowSize = false;
            viewTrabajador.Columns[3].Width = 120;
            viewTrabajador.Columns[3].Visible = true;

            //STATUS
            viewTrabajador.Columns[4].Caption = "Estado";
            viewTrabajador.Columns[4].Width = 50;
            viewTrabajador.Columns[4].DisplayFormat.Format = new FormatCustom();
            viewTrabajador.Columns[4].DisplayFormat.FormatString = "estado";
            viewTrabajador.Columns[4].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        }

        private void fnRecargarGrilla(DevExpress.XtraGrid.GridControl pGrid, string pSql)
        {
            SqlConnection cn;
            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter();
                        DataSet ds = new DataSet();

                        SqlCommand cmd = new SqlCommand(pSql, cn);
                        //parametros
                        //cmd.Parameters.Add(new SqlParameter(pCampo, pDato));
                        adapter.SelectCommand = cmd;
                        adapter.Fill(ds);
                        adapter.Dispose();
                        cmd.Dispose();                        

                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            pGrid.DataSource = ds.Tables[0];
                            fnLimpiarCampos();
                            int filas = ds.Tables[0].Rows.Count;
                            groupResultados.Text = filas + " coincidencias";
                            fnColumnasGrilla();
                            //ChangeValueCell();
                        }
                        else
                        {
                            XtraMessageBox.Show("No se encontraron resultados", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            txtbusqueda.Text = "";
                            txtbusqueda.Focus();
                        }
                    }
                }

            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);   
            }
            
        }

        private void ChangeValueCell()
        {
            string contratoRow = "";
            int perRow = 0, statusCell = 0;

            try
            {
                if (viewTrabajador.RowCount > 0)
                {
                    for (int i = 0; i < viewTrabajador.DataRowCount; i++)
                    {
                        contratoRow = (string)viewTrabajador.GetRowCellValue(i, "contrato");
                        perRow = Convert.ToInt32(viewTrabajador.GetRowCellValue(i, "anomes"));
                        statusCell = Convert.ToInt32(viewTrabajador.GetRowCellValue(i, "status"));
                        if (contratoRow.Length > 0 && Calculo.PeriodoValido(perRow))
                        {
                            if (perRow < Calculo.PeriodoObservado)
                            {
                                if(statusCell == 1)
                                    viewTrabajador.SetRowCellValue(i, "status", 0);
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

        
        #endregion
        //NO PERMITIR CARACTERES RAROS  --> /()#$%&/%"#"!"#$!#*/-..
        private void txtbusqueda_KeyPress(object sender, KeyPressEventArgs e)
        {
           
        }

        private void gridTrabajdor_DoubleClick(object sender, EventArgs e)
        {            
            if (viewTrabajador.SelectedRowsCount==0)
            {
                XtraMessageBox.Show("Debes seleccionar un trabajador", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;              
            }
            string pContrato = "";
            int periodo = 0;
            //TOMAMOS LOS VALORES DE LA FILA SELECCIONADA            
            
            if (viewTrabajador.RowCount > 0)
            {
               pContrato = viewTrabajador.GetFocusedDataRow()["contrato"].ToString();
               periodo =int.Parse(viewTrabajador.GetFocusedDataRow()["anomes"].ToString());
               //llamamos al metodo para cargar los datos en el form trabajador usando la interfaz
               this.opener.CargarBusqueda(pContrato, periodo);
               //cerrar formulario
               this.Close();
            }
        }

        private void btnCargar_Click(object sender, EventArgs e)
        {
            if (viewTrabajador.SelectedRowsCount == 0)
            {
                XtraMessageBox.Show("Debes seleccionar un trabajador", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);                
                return;
            }
            string pContrato = "";
            int periodo = 0;
            //TOMAMOS LOS VALORES DE LA FILA SELECCIONADA            
            if (viewTrabajador.RowCount > 0)
            {
              pContrato = viewTrabajador.GetFocusedDataRow()["contrato"].ToString();
              periodo =int.Parse(viewTrabajador.GetFocusedDataRow()["anomes"].ToString());
                //llamamos al metodo para cargar los datos en el form trabajador usando la interfaz
              this.opener.CargarBusqueda(pContrato, periodo);
            }
        }

        private void txtbusqueda_Leave(object sender, EventArgs e)
        {
            //EVENTE QUE SE LANZA CUANDO SE PIERDE EL FOCO
            //txtbusqueda.Properties.Appearance.BackColor = Color.White;

        }

        private void gridTrabajdor_ProcessGridKey(object sender, KeyEventArgs e)
        {
            //si se presiona enter cargamos los datos del empleado
            if (e.KeyCode == Keys.Enter)
            {
                if (viewTrabajador.SelectedRowsCount == 0)
                {
                    XtraMessageBox.Show("Debes seleccionar un trabajador", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                string pContrato = "";
                int periodo = 0;
                //TOMAMOS LOS VALORES DE LA FILA SELECCIONADA
                
               if (viewTrabajador.RowCount > 0)
               {
                  pContrato = viewTrabajador.GetFocusedDataRow()["contrato"].ToString();
                  periodo = int.Parse(viewTrabajador.GetFocusedDataRow()["anomes"].ToString());
                  //llamamos al metodo para cargar los datos en el form trabajador usando la interfaz
                  this.opener.CargarBusqueda(pContrato, periodo);

                    //CERRAR FORMULARIO
                    this.Close();
               }              
            }           
        }

        private void txtbusqueda_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                fnBuscarTrabajadorV2(txtbusqueda.Text);
            }
        }

        private void frmBuscarTrabajador_Shown(object sender, EventArgs e)
        {
            txtbusqueda.Focus();
        }

        private void viewTrabajador_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            DXPopupMenu menu = e.Menu;
            string nombre = "";
            if (menu != null)
            {
                if (viewTrabajador.RowCount > 0) nombre = (string)viewTrabajador.GetFocusedDataRow()["nombre"];

                DXMenuItem submenu = new DXMenuItem("Seleccionar a " + nombre, new EventHandler(SelectEmploye_Click));
                submenu.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("images/actions/show_16x16.png");
                menu.Items.Clear();
                menu.Items.Add(submenu);
            }
        }

        private void SelectEmploye_Click(object sender, EventArgs e)
        {           
            if (viewTrabajador.SelectedRowsCount == 0)
            {
                XtraMessageBox.Show("Debes seleccionar un trabajador", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string pContrato = "";
            int periodo = 0;
            //TOMAMOS LOS VALORES DE LA FILA SELECCIONADA

            if (viewTrabajador.RowCount > 0)
            {
                pContrato = viewTrabajador.GetFocusedDataRow()["contrato"].ToString();
                periodo = int.Parse(viewTrabajador.GetFocusedDataRow()["anomes"].ToString());
                //llamamos al metodo para cargar los datos en el form trabajador usando la interfaz
                this.opener.CargarBusqueda(pContrato, periodo);

                //CERRAR FORMULARIO
                this.Close();
            }
        }

        private void txtbusqueda_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            Close();
        }
    }
}