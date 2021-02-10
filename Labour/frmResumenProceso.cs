using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using DevExpress.XtraEditors;
using DevExpress.XtraReports.UI;
using System.Runtime.InteropServices;
using DevExpress.Utils.Menu;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;

namespace Labour
{
    public partial class frmResumenProceso : DevExpress.XtraEditors.XtraForm, IConjuntosCondicionales
    {
        #region "CONJUNTO CONDICIONALES"
        public void CargarCodigoConjunto(string code)
        {
            txtConjunto.Text = code;
        }
        #endregion

        /// <summary>
        /// Nombre de condicion.
        /// </summary>
        public string NombreCondicion { get; set; } = "";


        [DllImport("user32")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32")]
        static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem,  uint uEnable);

        const int MF_BYCOMMAND = 0;
        const int MF_DISABLED = 2;
        const int SC_CLOSED = 0xF060;

        //PARA GUARDAR LISTA 
        List<TablaResumenProceso> tablaFinal = new List<TablaResumenProceso>();

        /// <summary>
        /// Solo para guardar dataset consulta para reporte.
        /// </summary>
        private DataSet DataSourceReport = new DataSet();

        //PARA GUARDAR FILTRO USUARIO LOGUEADO
        private string FiltroUsuario { get; set; } = User.GetUserFilter();

        /// <summary>
        /// Usuario puede ver fichas privadas...
        /// </summary>
        private bool ShowPrivados { get; set; } = User.ShowPrivadas();

        public frmResumenProceso()
        {
            InitializeComponent();
        }

        private void frmResumenProceso_Load(object sender, EventArgs e)
        {

            Cursor.Current = Cursors.WaitCursor;
            var sm = GetSystemMenu(Handle, false);
            EnableMenuItem(sm, SC_CLOSED, MF_BYCOMMAND | MF_DISABLED);
            datoCombobox.spLlenaPeriodos("SELECT anomes FROM parametro ORDER BY anomes DESC", txtComboPeriodo, "anomes", "anomes", true);

            if (txtComboPeriodo.Properties.DataSource != null)
            {
                txtComboPeriodo.ItemIndex = 0;
                LlenaTabla(Convert.ToInt32(txtComboPeriodo.EditValue), "");
                //LlenaTablaNueva(Convert.ToInt32(txtComboPeriodo.EditValue), "", 1);
                fnSistema.spOpcionesGrilla(viewRes);
                viewRes.OptionsCustomization.AllowSort = false;
                FormatoColumnas();
                FormatearGrilla();             
            }
        }

        #region "MANEJO DE DATOS"
        #region "METODOS PARA NUEVO REPORTE"

        //FORMATEAR COLUMNAS GRILLA
        private void FormatoColumnas()
        {
            if (viewRes.RowCount>0)
            {                

                viewRes.Columns[0].Caption = "#";
                viewRes.Columns[0].Width = 15;
                viewRes.Columns[1].Caption = "Item";
                viewRes.Columns[1].Width = 150;
                viewRes.Columns[2].Caption = "Total";
                viewRes.Columns[2].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                viewRes.Columns[2].DisplayFormat.FormatString = "n0";
                viewRes.Columns[3].Visible = false;
                viewRes.Columns[4].Visible = false;
                viewRes.Columns[5].Visible = false;

                //viewRes.Columns[5].Group();
                
            }
        }  

        /// <summary>
        /// Obtenemos cadena con condicion adicional para generar reporte
        /// <para>Hace referencia a si el usuario tiene filtro, no puede ver fichas privadas, etc.</para>
        /// </summary>
        /// <returns></returns>
        private string getCondition(string pConjunto, int pPeriodo)
        {
            string cadena = "";

            //USUARIO TIENE FILTRO?
            if (FiltroUsuario != "0")
            {
                //LA BUSQUEDA TIENE FILTROS?
                if (pConjunto != "")
                {
                    //PUEDE VER FICHAS privadas?
                    if (ShowPrivados == false)
                    {
                        cadena = $"AND itemtrabajador.contrato IN (SELECT contrato FROM trabajador WHERE {Conjunto.GetCondicionFromCode(FiltroUsuario)} AND {Conjunto.GetCondicionFromCode(pConjunto)} AND privado=0 AND anomes={pPeriodo})";
                    }
                    else
                    {
                        cadena = $"AND itemtrabajador.contrato IN (SELECT contrato FROM trabajador WHERE {Conjunto.GetCondicionFromCode(FiltroUsuario)} AND {Conjunto.GetCondicionFromCode(pConjunto)} AND anomes={pPeriodo})";
                    }
                }
                else
                {
                    //NO HAY BUSQUEDA CONDICIONADA...
                    if(ShowPrivados == false)
                        cadena = $"AND itemtrabajador.contrato IN (SELECT contrato FROM trabajador WHERE {Conjunto.GetCondicionFromCode(FiltroUsuario)} AND privado=0 AND anomes={pPeriodo})";
                    else
                        cadena = $"AND itemtrabajador.contrato IN (SELECT contrato FROM trabajador WHERE {Conjunto.GetCondicionFromCode(FiltroUsuario)} AND privado=0 AND anomes={pPeriodo})";
                }
            }
            else
            {
                //USUARIO NO TIENE FILTROS
                //LA BUSQUEDA TIENE FILTROS?
                if (pConjunto != "")
                {
                    //PUEDE VER FICHAS privadas?
                    if (ShowPrivados == false)
                    {
                        cadena = $"AND itemtrabajador.contrato IN (SELECT contrato FROM trabajador WHERE {Conjunto.GetCondicionFromCode(pConjunto)} AND privado=0 AND anomes={pPeriodo})";
                    }
                    else
                    {
                        cadena = $"AND itemtrabajador.contrato IN (SELECT contrato FROM trabajador WHERE {Conjunto.GetCondicionFromCode(pConjunto)} AND anomes={pPeriodo})";
                    }
                }
                else
                {
                    //NO HAY BUSQUEDA CONDICIONADA...
                    if (ShowPrivados == false)
                        cadena = $"AND itemtrabajador.contrato IN (SELECT contrato FROM trabajador WHERE privado=0 AND anomes={pPeriodo})";
                    else
                        cadena = "";
                }
            }

            return cadena;
        }

        /// <summary>
        /// Retorna el sql final para generar reporte resumen del proceso.
        /// </summary>
        /// <param name="pCondition">Trozo de código que representa las condiciones que tenga un usuario.</param>
        /// <returns></returns>
        private string getSqlResumen(string pCondition)
        {
            string sql = "DECLARE @TotalHaberesImponibles DECIMAL \n" +
                        " DECLARE @TotalHaberesNoImponibles DECIMAL \n" +
                        " DECLARE @TotalLeyesSociales DECIMAL \n" +
                        " DECLARE @TotalDescuentos DECIMAL \n" +
                        " DECLARE @TotalRemu DECIMAL \n" +
                        " DECLARE @Sanna DECIMAL \n" +                        
                        " DECLARE @TotalPeriodo DECIMAL \n" +
                        " SET @TotalHaberesImponibles = (SELECT SUM(valorcalculado) FROM itemtrabajador INNER JOIN trabajador On trabajador.contrato=itemtrabajador.contrato AND trabajador.anomes=itemtrabajador.anomes WHERE trabajador.anomes = @pPeriodo AND tipo = 1 {condition} AND suspendido = 0 AND trabajador.status=1) \n" +
                        " SET @TotalHaberesNoImponibles = (SELECT SUM(valorcalculado) FROM itemtrabajador INNER JOIN trabajador On trabajador.contrato=itemtrabajador.contrato AND trabajador.anomes=itemtrabajador.anomes  WHERE trabajador.anomes = @pPeriodo AND(tipo = 2 OR tipo = 3) {condition} AND suspendido = 0 AND trabajador.status=1) \n" +
                        " SET @TotalLeyesSociales = (SELECT SUM(valorcalculado) FROM itemtrabajador INNER JOIN trabajador On trabajador.contrato=itemtrabajador.contrato AND trabajador.anomes=itemtrabajador.anomes WHERE trabajador.anomes = @pPeriodo AND tipo = 4 AND(coditem <> 'SCEMPRE' AND coditem <> 'SEGINV') {condition} AND suspendido = 0 AND trabajador.status=1) \n" +
                        " SET @TotalDescuentos = (SELECT SUM(valorcalculado) FROM itemtrabajador INNER JOIN trabajador On trabajador.contrato=itemtrabajador.contrato AND trabajador.anomes=itemtrabajador.anomes WHERE trabajador.anomes = @pPeriodo AND tipo = 5 {condition} AND suspendido = 0 AND trabajador.status=1) \n" +
                        $" SET @TotalRemu = (select SUM(syspago) from calculomensual INNER JOIN trabajador ON trabajador.contrato = calculoMensual.contrato AND trabajador.anomes=calculoMensual.anomes WHERE trabajador.anomes = @pPeriodo AND trabajador.status=1 {pCondition.Replace("itemtrabajador.contrato", "calculomensual.contrato")} ) \n" +
                        " SET @TotalPeriodo = (SELECT ISNULL(SUM(valorcalculado), 0) FROM itemtrabajador INNER JOIN trabajador On trabajador.contrato=itemtrabajador.contrato AND trabajador.anomes=itemtrabajador.anomes WHERE trabajador.anomes = @pPeriodo {condition} AND suspendido = 0 AND trabajador.status=1) \n" +
                        " SET @Sanna = (SELECT SUM(sysvalsanna) FROM calculomensual INNER JOIN itemtrabajador ON itemtrabajador.contrato = calculomensual.contrato \n" +
                        " AND itemtrabajador.anomes = calculomensual.anomes \n" +
                        "INNER JOIN trabajador ON trabajador.contrato=calculomensual.contrato AND trabajador.anomes=calculomensual.anomes \n" +
                        " WHERE calculomensual.anomes = @pPeriodo {condition} AND coditem = 'SANNA' AND suspendido = 0 AND trabajador.status=1) \n" +
                        "" +
                        "SELECT N, descripcion, total, detalle, tipo, suborden FROM \n" +
                        "( \n" +                              
                             "SELECT 0 as 'N', '1 - HABERES' as descripcion, 0 as total, 0 as tipo, 'titulo' as detalle, 0 as 'suborden' \n" +
                            "UNION \n" +
                                 " SELECT count(itemtrabajador.coditem), descripcion, SUM(valorcalculado) as total, itemTrabajador.tipo, 'dato', 1 \n" +
                                 " FROM itemtrabajador INNER JOIN item ON item.coditem = itemTrabajador.coditem \n" +
                                 " INNER JOIN trabajador ON trabajador.contrato = itemtrabajador.contrato AND trabajador.anomes=itemtrabajador.anomes" +
                                 " WHERE trabajador.anomes = @pPeriodo AND itemTrabajador.tipo = 1 \n" +
                                 " {condition} AND suspendido = 0 AND trabajador.status=1 \n" +
                                 " GROUP BY descripcion, itemTrabajador.tipo \n" +
                            "UNION \n" +
                                "SELECT 0, 'TOTAL HABERES', @TotalHaberesImponibles, 1, 'subtotal', 2 \n" +
                            "UNION \n" +
                                " SELECT 0, '2 - HABERES NO IMPONIBLES', 0, 2, 'titulo', 3 \n" +
                            "UNION \n" +
                                 " SELECT count(itemtrabajador.coditem), descripcion, SUM(valorcalculado), itemTrabajador.tipo, 'dato', 4 \n" +
                                 " FROM itemtrabajador INNER JOIN item ON item.coditem = itemTrabajador.coditem \n" +
                                 " INNER JOIN trabajador ON trabajador.contrato = itemtrabajador.contrato AND trabajador.anomes=itemtrabajador.anomes " +
                                 " WHERE trabajador.anomes = @pPeriodo AND (itemTrabajador.tipo = 2 OR itemTrabajador.tipo = 3) \n" +
                                 " {condition} AND suspendido = 0 AND trabajador.status=1 \n" +
                                 " GROUP BY descripcion, itemTrabajador.tipo \n" +
                            "UNION \n" +
                                "SELECT 0, 'TOTAL HABERES NO IMPONIBLES', @TotalHaberesNoImponibles, 2, 'subtotal', 5 \n" +
                            "UNION \n" +
                            " SELECT 0, '3 - LEYES SOCIALES', 0, 4, 'titulo', 6 \n" +
                            "UNION \n" +
                                " SELECT count(itemtrabajador.coditem), descripcion, SUM(valorcalculado), itemTrabajador.tipo, 'dato', 7 \n" +
                                "FROM itemtrabajador INNER JOIN item ON item.coditem = itemTrabajador.coditem \n" +
                                " INNER JOIN trabajador ON trabajador.contrato = itemtrabajador.contrato AND trabajador.anomes = itemtrabajador.anomes " +
                                "WHERE trabajador.anomes = @pPeriodo AND itemTrabajador.tipo = 4 AND(itemTrabajador.coditem <> 'SCEMPRE' AND itemTrabajador.coditem <> 'SEGINV') \n" +
                                "{condition} AND suspendido = 0 AND trabajador.status=1 \n" +
                                "GROUP BY descripcion, itemTrabajador.tipo \n" +
                            " UNION \n" +
                                "SELECT 0, 'TOTAL LEYES SOCIALES', @TotalLeyesSociales, 4, 'subtotal', 8 \n" +
                             "UNION \n" +
                                "SELECT 0, '4 - DESCUENTOS', 0, 5, 'titulo', 9 \n" +
                            "UNION \n" +
                                  "SELECT count(itemtrabajador.coditem), descripcion, SUM(valorcalculado), itemTrabajador.tipo, 'dato', 10 \n" +
                                  "FROM itemtrabajador INNER JOIN item ON item.coditem = itemtrabajador.coditem \n" +
                                  " INNER JOIN trabajador on trabajador.contrato = itemtrabajador.contrato AND trabajador.anomes=itemtrabajador.anomes \n" +
                                  "WHERE trabajador.anomes = @pPeriodo AND itemTrabajador.tipo = 5 \n" +
                                  "{condition} AND suspendido = 0 AND trabajador.status=1 \n" +
                                  "GROUP BY descripcion, itemTrabajador.tipo \n" +
                            "UNION \n" +
                                "SELECT 0, 'TOTAL DESCUENTOS', @TotalDescuentos, 5, 'subtotal', 11 \n" +
                            "UNION \n" +
                                "SELECT 0, '5 - APORTES', 0, 6, 'titulo', 12 \n" +
                            "UNION \n" +
                                 " SELECT count(itemtrabajador.coditem), descripcion, SUM(valorcalculado), itemTrabajador.tipo, 'dato', 13 \n" +
                                 " FROM itemtrabajador INNER JOIN ITEM ON item.coditem = itemTrabajador.coditem \n" +
                                 " INNER JOIN trabajador ON trabajador.contrato = itemtrabajador.contrato AND trabajador.anomes = itemtrabajador.anomes \n" +
                                 " WHERE trabajador.anomes = @pPeriodo AND itemTrabajador.tipo = 6 AND itemtrabajador.coditem = 'CAJACOM' \n" +
                                 " {condition} AND suspendido = 0 AND trabajador.status=1 \n" +
                                 " GROUP BY descripcion, itemTrabajador.tipo \n" +
                            "UNION \n" +
                                 "SELECT count(*), 'MUTUAL', ISNULL(ROUND(SUM(sysmutual), 0), 0), 6, 'dato', 13 \n" +
                                 "FROM calculomensual INNER JOIN itemtrabajador On itemtrabajador.contrato = calculomensual.contrato \n" +
                                 "AND itemtrabajador.anomes = calculomensual.anomes  \n" +
                                 "INNER JOIN trabajador ON trabajador.contrato=calculomensual.contrato AND trabajador.anomes = calculomensual.anomes \n" + 
                                 "WHERE calculomensual.anomes = @pPeriodo \n" +
                                 "{condition} AND coditem = 'MUTUALI' AND suspendido = 0 AND trabajador.status=1\n" +
                            "UNION \n" +
                                 "SELECT count(*), 'LEY SANNA', ISNULL(ROUND(SUM(sysvalsanna), 0), 0), 6, 'dato', 13 \n" +
                                 "FROM calculomensual INNER JOIN  itemtrabajador on itemtrabajador.contrato = calculomensual.contrato \n" +
                                 "AND itemtrabajador.anomes = calculomensual.anomes \n" +
                                 "INNER JOIN trabajador ON trabajador.contrato = calculoMensual.contrato AND trabajador.anomes=calculomensual.anomes \n" +
                                 "WHERE itemtrabajador.anomes = @pPeriodo \n" +
                                 "{condition} AND coditem = 'SANNA' AND suspendido = 0 AND trabajador.status=1\n" +
                            "UNION \n" +
                                  "SELECT count(itemtrabajador.coditem), descripcion, SUM(valorcalculado), 6, 'dato', 14 \n" +
                                  "FROM itemtrabajador INNER JOIN item ON item.coditem = itemTrabajador.coditem \n" +
                                  "INNER JOIN trabajador on trabajador.contrato = itemtrabajador.contrato AND trabajador.anomes = itemtrabajador.anomes \n" +
                                  "WHERE itemtrabajador.anomes = @pPeriodo AND itemTrabajador.coditem = 'SEGINV' \n" +
                                  "{condition} AND suspendido = 0 AND trabajador.status=1 \n" +
                                  "GROUP BY descripcion, itemTrabajador.tipo \n" +
                            "UNION \n" +
                                  "SELECT count(*), 'SEGURO CUENTA INDIVIDUAL EMPRESA', SUM(syscicese), 6, 'dato', 15 \n" +
                                  "FROM calculoMensual INNER JOIN itemtrabajador ON itemtrabajador.contrato = calculomensual.contrato \n" +
                                  "AND itemtrabajador.anomes = calculomensual.anomes \n" +
                                  "INNER JOIN trabajador ON trabajador.contrato = calculomensual.contrato AND trabajador.anomes = calculomensual.anomes \n" +
                                  "WHERE calculomensual.anomes = @pPeriodo \n" +
                                  "{condition} AND coditem = 'SCEMPRE' AND suspendido = 0 AND trabajador.status=1 \n" +
                            "UNION \n" +
                                  "SELECT count(*), 'SEGURO CUENTA FONDO SOLIDARIO', SUM(sysfscese), 6, 'dato', 16 \n" +
                                  "FROM calculoMensual INNER JOIN itemtrabajador ON itemtrabajador.contrato = calculomensual.contrato \n" +
                                  "AND itemtrabajador.anomes = calculomensual.anomes \n" +
                                  "INNER JOIN trabajador ON trabajador.contrato = calculoMensual.contrato AND trabajador.anomes=calculoMensual.anomes \n" +
                                  "WHERE calculomensual.anomes = @pPeriodo \n" +
                                  "{condition} AND coditem = 'SCEMPRE' AND suspendido = 0 AND trabajador.status=1 \n" +
                            "UNION \n" +
                                   "SELECT 0, 'TOTAL APORTES EMPRESA', \n" +
                                   " (SELECT ISNULL(SUM(valorcalculado), 0) FROM itemtrabajador \n" +
                                   "INNER JOIN trabajador ON trabajador.contrato = itemtrabajador.contrato AND trabajador.anomes=itemtrabajador.anomes \n" +
                                   " WHERE trabajador.anomes = @pPeriodo AND tipo = 6 AND coditem <> 'SANNA' {condition} AND suspendido = 0 AND trabajador.status=1) + \n" +
                                   " (SELECT ISNULL(SUM(valorcalculado), 0) FROM itemtrabajador \n" +
                                   "INNER JOIN trabajador ON trabajador.contrato = itemtrabajador.contrato AND trabajador.anomes=itemtrabajador.anomes \n" +
                                   "WHERE trabajador.anomes = @pPeriodo AND(coditem = 'SEGINV' OR coditem = 'SCEMPRE') {condition} AND suspendido = 0 AND trabajador.status=1) + \n" +
                                   " @Sanna,  6, 'subtotal', 17 \n" +
                            "UNION \n" +
                                   "SELECT 0, '6 - RESUMEN', 0, 7, 'titulo', 18 \n" +
                            "UNION \n" +
                                    " SELECT 0, 'TOTAL PAGOS POR REMUNERACIONES', @TotalRemu, 7, 'totalizado', 19 \n" +
                            //"UNION " +                                      
                            //        "SELECT 'DESCUENTOS + PAGO POR REMUNERACIONES', @TotalRemu + @TotalDescuentos, 9, 'dato', 20 " +
                            //"UNION \n" +
                            //        "SELECT 0, 'TOTAL PERIODO', ISNULL(@TotalPeriodo, 0), 7, 'totalizado', 21 \n" +
                                    ")data \n" +
                            "ORDER BY suborden \n";

            sql = sql.Replace("{condition}", pCondition);

            return sql;
        }

        //LLENAR GRIDVIEW 
        private void LlenaTabla(int pPeriodo, string pCodeConjunto)
        {
            //string sql = getQueryResumen(pCodeConjunto);
            string sql = "";
            string condition = getCondition(pCodeConjunto, pPeriodo);            
            sql = getSqlResumen(condition);

            DataSourceReport.Clear();

            SqlDataAdapter ad = new SqlDataAdapter();
            SqlCommand cmd;
            DataSet ds = new DataSet();

            //Descripción de condicion (si es que aplica)
            if (FiltroUsuario.Length > 0 && FiltroUsuario != "0")
                NombreCondicion = Conjunto.GetDescConjunto(FiltroUsuario) + ";" + Conjunto.GetDescConjunto(pCodeConjunto);
            else if (pCodeConjunto.Length > 0)
                NombreCondicion = Conjunto.GetDescConjunto(pCodeConjunto);
            else
                NombreCondicion = "No Aplica";

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pPeriodo", pPeriodo));
                        //Console.WriteLine(sql);
                        ad.SelectCommand = cmd;
                        ad.Fill(ds, "tabla");                     

                        if (ds.Tables[0].Rows.Count > 0)
                        {                          
                            gridRes.DataSource = ds.Tables[0];                           
                            lblPeriodoEval.Text = "EVALUANDO: " + fnSistema.PrimerMayuscula(fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(pPeriodo)));

                            DataSourceReport = ds;
                        }

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                        ad.Dispose();
                    }          
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        private void LlenaTablaNueva(int pPeriodo, string pCodeConjunto, int pOption)
        {
            #region "sql"
            string sql = "DECLARE @opt AS INTEGER " +
                         "SET @opt = {option} " +
                         "IF(@opt = 1) " +
                         "SELECT count(*) as num, coditem as item, SUM(valorcalculado) as valor, " +
                         "case WHEN tipo = 1 THEN '1- Haberes imponibles' WHEN tipo = 2 THEN '2 -Haberes No Imponibles' " +
                         "WHEN tipo = 4 THEN '3- Leyes Sociales' WHEN tipo = 5 THEN '4- Otros Descuentos' " +
                         "WHEN tipo = 6 THEN '5- Aportes' WHEN tipo = 7 THEN '6- Totales' END as tipo " +
                         "FROM " +
                         "(SELECT CONCAT(c.nombre, '-', c.id) as centro, ca.nombre as cargo,  " +
                         "CONCAT(su.descSucursal, ' (code: ', su.codsucursal, ')') as sucursal, ar.nombre as area,  " +
                         "CONCAT(item.coditem, ' - ', item.descripcion) as coditem, " +
                         "valorcalculado, " +
                         "case WHEN(it.coditem = 'SCEMPLE' OR item.coditem = 'SEGINV') " +
                         "THEN 6 " +
                         "WHEN(it.tipo = 3) " +
                         "THEN 2 ELSE it.tipo " +
                         "END as tipo " +
                         "FROM itemTrabajador it " +
                         "INNER JOIN TRABAJADOR T on it.contrato = t.contrato " +
                         "AND it.anomes = t.anomes " +
                         "INNER JOIN ccosto c on c.id = t.ccosto " +
                         "INNER JOIN cargo ca on ca.id = t.cargo " +
                         "INNER JOIN area ar on ar.id = t.area " +
                         "INNER JOIN sucursal su on su.codSucursal = t.sucursal " +
                         "INNER JOIN item on item.coditem = it.coditem " +
                         "WHERE t.anomes = @pPeriodo AND status = 1 AND suspendido = 0 {expresion} " +
                         ") as t " +
                         "GROUP BY coditem, tipo " +
                         "ORDER BY tipo " +
                         "ELSE " +
                         "select centro, cargo, sucursal, area, coditem, valorcalculado,  " +
	                     "case WHEN tipo = 1 THEN '1- Haberes imponibles' WHEN tipo = 2 THEN '2 -Haberes No Imponibles' " +
                         "WHEN tipo = 4 THEN '3- Leyes Sociales' WHEN tipo = 5 THEN '4- Otros Descuentos' " +
                         "WHEN tipo = 6 THEN '5- Aportes' WHEN tipo = 7 THEN '6- Totales' END as tipo " +
                         "FROM " +
                         "(SELECT CONCAT(c.nombre, '-', c.id) as centro, ca.nombre as cargo,  " +
                         "CONCAT(su.descSucursal, ' (code: ', su.codsucursal, ')') as sucursal, ar.nombre as area,  " +
                         "CONCAT(item.coditem, ' - ', item.descripcion) as coditem,  " +
                         "valorcalculado, " +
                         "case WHEN(it.coditem = 'SCEMPLE' OR item.coditem = 'SEGINV') " +
                         "THEN 6 " +
                         "WHEN(it.tipo = 3) " +
                         "THEN 2 ELSE it.tipo " +
                         "END as tipo " +
                         "FROM itemTrabajador it " +
                         "INNER JOIN TRABAJADOR T on it.contrato = t.contrato " +
                         "AND it.anomes = t.anomes " +
                         "INNER JOIN ccosto c on c.id = t.ccosto " +
                         "INNER JOIN cargo ca on ca.id = t.cargo " +
                         "INNER JOIN area ar on ar.id = t.area " +
                         "INNER JOIN sucursal su on su.codSucursal = t.sucursal " +
                         "INNER JOIN item on item.coditem = it.coditem " +
                         "WHERE t.anomes = @pPeriodo AND status = 1 AND suspendido = 0 {expresion} ) as t";
                        
            #endregion
            string sqlFil = Calculo.GetSqlFiltro(FiltroUsuario, pCodeConjunto, ShowPrivados, true);
            SqlConnection cn;
            SqlCommand cmd;
            DataSet ds = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            if (sqlFil.Length > 0)
                sql = sql.Replace("{expresion}", $"AND t.contrato IN (select contrato FROM trabajador {sqlFil})");
            else
                sql = sql.Replace("{expresion}", "");

            sql = sql.Replace("{option}", pOption.ToString());

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
                            cmd.Parameters.Add(new SqlParameter("@pPeriodo", pPeriodo));
                            //cmd.Parameters.Add(new SqlParameter("@opt", pOption));

                            ad.SelectCommand = cmd;
                            ad.Fill(ds, "data");

                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                gridRes.DataSource = ds.Tables[0];
                                //gridRes.DataMember = "data";

                                viewRes.Columns[3].Group();
                                viewRes.OptionsView.GroupFooterShowMode = GroupFooterShowMode.VisibleAlways;
                                GridGroupSummaryItem item = new GridGroupSummaryItem();
                                item.FieldName = "valor";
                                item.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                                item.DisplayFormat = "Total: {0:c2}";
                                viewRes.GroupSummary.Add(item);
                                viewRes.ExpandAllGroups();                                
                            }

                            ds.Dispose();
                            ad.Dispose();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
               //Error...
            }
        }

        private void MostrarReporte(int pPeriodo, bool? Imprime = false, bool? GeneraPdf=false)
        {       
            if (DataSourceReport.Tables[0].Rows.Count > 0)
            {
                DataTable pTabla = new DataTable();
                pTabla = DataSourceReport.Tables[0];

                //TestRptResumenProcesoExterno res = new TestRptResumenProcesoExterno();
                ReportesExternos.rptResumenProcesov2 res = new ReportesExternos.rptResumenProcesov2();
                //RptResumenProcesov2 res = new RptResumenProcesov2();
                res.DataSource = DataSourceReport.Tables[0];
                res.DataSource = pTabla;
                res.DataMember = "tabla";

                Empresa emp = new Empresa();
                emp.SetInfo();

                //PARAMETROS REPORTE
                foreach (DevExpress.XtraReports.Parameters.Parameter parametro in res.Parameters)
                {
                    parametro.Visible = false;
                }

                res.Parameters["periodo"].Value = fnSistema.PrimerMayuscula(fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(pPeriodo)));
                res.Parameters["empresa"].Value = emp.Razon;
                res.Parameters["condicion"].Value = NombreCondicion;
                //Pasar la imagen al reporte dll como parámetro
                res.Parameters["imagen"].Value = Imagen.GetLogoFromBd();

                Documento d = new Documento("", 0);

                if ((bool)Imprime)
                    d.PrintDocument(res);
                else if ((bool)GeneraPdf)
                    d.ExportToPdf(res, $"ResumenProceso_{fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(pPeriodo))}");
                else
                    d.ShowDocument(res);
            }        
        }

        //ITERAR GRILLA
        private void FormatearGrilla()
        {
            if (viewRes.RowCount>0)
            {
                for (int i = 0; i < viewRes.DataRowCount; i++)
                {
                    if (viewRes.GetRowCellValue(i, "detalle").ToString() == "titulo")
                    {
                        //PONER EN BLANCO TOTAL
                        viewRes.SetRowCellValue(i, "total", "");
                        viewRes.SetRowCellValue(i, "N", "");
                    }

                    if (viewRes.GetRowCellValue(i, "detalle").ToString() == "subtotal")
                    {
                        viewRes.SetRowCellValue(i, "N", "");
                    }

                    if (viewRes.GetRowCellValue(i, "detalle").ToString() == "totalizado")
                    {
                        viewRes.SetRowCellValue(i, "N", "");
                    }
                }
            }
        }
        
        #endregion

        #endregion
 

        private void btnCargarDatos_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (objeto.ValidaAcceso(User.GetUserGroup(), "rptresproc") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (txtComboPeriodo.Properties.DataSource == null)
            { XtraMessageBox.Show("Por favor ingresa un periodo a evaluar", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); txtComboPeriodo.Focus(); return; }            

            if (Calculo.PeriodoValido(Convert.ToInt32(txtComboPeriodo.EditValue)) == false)
            { XtraMessageBox.Show("Por favor ingresa un periodo válido", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); txtComboPeriodo.Focus(); return; }

            if (cbTodos.Checked)
            {
                //BUSCAMOS POR TODOS LOS REGISTROS DEL PERIODO SELECCIONADO
                //ShowProceso(Convert.ToInt32(txtPeriodo.Text));
                LlenaTabla(Convert.ToInt32(txtComboPeriodo.EditValue),"");
                if (viewRes.RowCount > 0)
                {
                    fnSistema.spOpcionesGrilla(viewRes);
                    FormatoColumnas();
                    FormatearGrilla(); 
                }
            }
            else
            {
                if (txtConjunto.Text == "")
                { XtraMessageBox.Show("Por favor ingresa una condición válida", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                //BUSCAMOS POR CONJUNTO SELECCIONADO
                if (Conjunto.ExisteConjunto(txtConjunto.Text) == false)
                { XtraMessageBox.Show("Parece ser que el conjunto ingresado no existe", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

                LlenaTabla(Convert.ToInt32(txtComboPeriodo.EditValue), txtConjunto.Text);
                if (viewRes.RowCount > 0)
                {
                    fnSistema.spOpcionesGrilla(viewRes);
                    FormatoColumnas();
                    FormatearGrilla();
                }                

                //ShowProcesoConjunto(Convert.ToInt32(txtPeriodo.Text), txtConjunto.Text);
            }        
        }

        private void viewResumen_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "Haberes" || e.Column.FieldName == "Nohaberes" || e.Column.FieldName == "LeyesSociales" || e.Column.FieldName == "Descuentos" || e.Column.FieldName == "Aportes")
            {
                //if (Convert.ToDouble(e.Value) == 0)
               // {
                 //   e.DisplayText = "";
               // }
            }
        }

        private void txtPeriodo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void btnImpresionRapida_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (txtComboPeriodo.Properties.DataSource != null)
            {
                //ImprimeDocumento(Convert.ToInt32(txtPeriodo.Text), true);
                if (Calculo.PeriodoValido(Convert.ToInt32(txtComboPeriodo.EditValue)) == false)
                { XtraMessageBox.Show("Por favor ingresa un periodo valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtComboPeriodo.Focus(); return; }

                MostrarReporte(Convert.ToInt32(txtComboPeriodo.EditValue), true);
            }
            else
            {
                XtraMessageBox.Show("Por favor ingresa un periodo a evaluar", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtComboPeriodo.Focus();
            }
        }

        private void cbTodos_CheckedChanged(object sender, EventArgs e)
        {
            if (cbTodos.Checked)
            {
                txtConjunto.Text = "";
                txtConjunto.Enabled = false;
                btnConjunto.Enabled = false;
            }
            else
            {
                txtConjunto.Enabled = true;
                btnConjunto.Enabled = true;
                txtConjunto.Text = "";
                txtConjunto.Focus();
            }
        }

        private void txtConjunto_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) || char.IsLetter(e.KeyChar) || char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void txtConjunto_DoubleClick(object sender, EventArgs e)
        {
            FrmFiltroBusqueda filtro = new FrmFiltroBusqueda(true);
            filtro.opener = this;
            filtro.ShowDialog();
        }

        private void txtConjunto_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            DXPopupMenu me = e.Menu;
            if (me != null)
            {
                me.Items.Clear();

                DXMenuItem menu = new DXMenuItem("Agregar conjunto", new EventHandler(AgregarConjunto_Click));
                menu.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("images/actions/show_16x16.png");

                me.Items.Add(menu);
            }
        }

        private void AgregarConjunto_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            FrmFiltroBusqueda filtro = new FrmFiltroBusqueda(true);
            filtro.opener = this;
            filtro.ShowDialog();
        }

        private void viewRes_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            if (viewRes.RowCount>0)
            {
                if (e.Column.FieldName == "descripcion")
                {             
                    if (e.CellValue.ToString() == "1 - HABERES" || e.CellValue.ToString() == "2 - HABERES NO IMPONIBLES" || 
                        e.CellValue.ToString() == "3 - LEYES SOCIALES" || e.CellValue.ToString() == "4 - DESCUENTOS" || 
                        e.CellValue.ToString() == "5 - APORTES" || e.CellValue.ToString() == "6 - RESUMEN")
                    {
                        e.Appearance.FontStyleDelta = FontStyle.Bold;
                    } 
                }
            }           
        }

        private void btnPdf_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (txtComboPeriodo.Properties.DataSource != null)
            {
                //ShowResumenProceso(Convert.ToInt32(txtPeriodo.Text));
                //ImprimeDocumento(Convert.ToInt32(txtPeriodo.Text));
                if (Calculo.PeriodoValido(Convert.ToInt32(txtComboPeriodo.EditValue)) == false)
                { XtraMessageBox.Show("Selecciona un periodo valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtComboPeriodo.Focus(); return; }

                MostrarReporte(Convert.ToInt32(txtComboPeriodo.EditValue), false, true);

            }
            else
            {
                XtraMessageBox.Show("Por favor ingresa un periodo a evaluar", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtComboPeriodo.Focus();
            }
        }

        private void btnConjunto_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            FrmFiltroBusqueda filtro = new FrmFiltroBusqueda(true);
            filtro.StartPosition = FormStartPosition.CenterParent;
            filtro.opener = this;
            filtro.ShowDialog();
        }


        private void btnImprimir_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;
            if (txtComboPeriodo.Properties.DataSource != null)
            {
                //ShowResumenProceso(Convert.ToInt32(txtPeriodo.Text));
                //ImprimeDocumento(Convert.ToInt32(txtPeriodo.Text));
                if (Calculo.PeriodoValido(Convert.ToInt32(txtComboPeriodo.EditValue)) == false)
                { XtraMessageBox.Show("Selecciona un periodo valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtComboPeriodo.Focus(); return; }

                MostrarReporte(Convert.ToInt32(txtComboPeriodo.EditValue));             
            }
            else
            {
                XtraMessageBox.Show("Por favor ingresa un periodo a evaluar", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtComboPeriodo.Focus();
            }
        }

        private void txtPeriodo_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION 
            Sesion.NuevaActividad();

            Close();
        }
    }

    //CLASE QUE REPRESENTA LA TABLA RESUMEN
    class TablaResumenProceso
    {
        //CODIGO ITEM
        private string item { get; set; } = ""; //CODIGO ITEM
        private string descripcion { get; set; } = "";
        private string haberes { get; set; } = "";
        private string nohaberes { get; set; } = "";
        private string leyesSociales { get; set; } = "";
        private string descuentos { get; set; } = "";
        private string aportes { get; set; } = "";

        public string Item {
            get { return this.item; }
            set { this.item = value; }
        }

        public string Descripcion {
            get { return this.descripcion; }
            set { this.descripcion = value; }
        }

        public string Haberes
        {
            get { return this.haberes; }
            set { this.haberes = value; }
        }

        public string Nohaberes {
            get { return this.nohaberes; }
            set { this.nohaberes = value; }
        }

        public string LeyesSociales {
            get { return this.leyesSociales; }
            set { this.leyesSociales = value; }
        }

        public string Descuentos {
            get { return this.descuentos; }
            set { this.descuentos = value; }
        }
        public string Aportes {
            get { return this.aportes; }
            set { this.aportes = value; }
        }

    }    

    class TablaBloqueTipo
    {
        public string Item { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public double Valor { get; set; } = 0;
    }

 
}