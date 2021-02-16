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
using System.IO;
using DevExpress.XtraReports.UI;
using System.Data.SqlClient;
using System.Collections;

namespace Labour
{
    public partial class frmComprobanteContable : DevExpress.XtraEditors.XtraForm, IConjuntosCondicionales
    {
        #region "INTERFAZ CONJUNTOS CONDICIONALES"
        public void CargarCodigoConjunto(string code)
        {
            this.txtConjunto.Text = code;
        }
        #endregion

        /// <summary>
        /// Guardar la información de la consulta.
        /// </summary>
        public DataTable TablaDataSource { get; set; } = new DataTable();

        /// <summary>
        /// Para guardar informacion del esquema usado actualmente
        /// </summary>
        public Esquema Esq { get; set; }
        public string FiltroUsuario { get; private set; } = User.GetUserFilter();
        public bool ShowPrivados { get; private set; } = User.ShowPrivadas();

        private string CondicionBusqueda = "";

        public frmComprobanteContable()
        {
            InitializeComponent();
        }

        private void frmComprobanteContable_Load(object sender, EventArgs e)
        {
            datoCombobox.spLlenaPeriodos("SELECT anomes from parametro ORDER BY anomes desc", txtPeriodo, "anomes", "anomes", true);
            if (txtPeriodo.Properties.DataSource != null)
                txtPeriodo.ItemIndex = 0;

            GrupoContable grupo = new GrupoContable();
            int codusado = 0;
            codusado = grupo.GetEsquemaUsado();
            //Seteamos toda la informacion del esquema asociado.
            Esq = new Esquema();
            Esq.SetInfo(codusado);

            txtConjunto.Enabled = false;
            btnConjunto.Enabled = false;
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();
            TablaDataSource.Clear();

            Cursor.Current = Cursors.WaitCursor;

            string FileName = "";
            if (txtPeriodo.Properties.DataSource == null)
            { XtraMessageBox.Show("Periodo no válido", "Periodo", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (Directory.Exists(txtSalida.Text) == false)
            { XtraMessageBox.Show("Directorio de salida no válido", "Ruta", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtSalida.Focus(); return; }

            if (Calculo.PeriodoValido(Convert.ToInt32(txtPeriodo.EditValue)) == false)
            { XtraMessageBox.Show("Por favor selecciona un periodo válido", "Periodo", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (checkBox1.Checked == false)
            {
                //Se selecciona una condicion adicional
                if (txtConjunto.Text == "")
                { XtraMessageBox.Show("Por favor ingresa una condición válida", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtConjunto.Focus(); return; }

                if(Conjunto.ExisteConjunto(txtConjunto.Text) == false)
                { XtraMessageBox.Show("Por favor ingresa una condición válida", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtConjunto.Focus(); return; }
            }

            DataTable tabla = new DataTable();
            
            //PROCESS NAME
            string Proc = "Remuneraciones " + Convert.ToDouble(txtPeriodo.EditValue);
            
            if(Esq.Formato == 1)
                FileName = txtSalida.Text + @"\Comprobante_" + Convert.ToInt32(txtPeriodo.EditValue) + ".txt";
            else
                FileName = txtSalida.Text + @"\Comprobante_" + Convert.ToInt32(txtPeriodo.EditValue) + ".xlsx";

            ////string sql = CreateSqlFile(Convert.ToInt32(txtPeriodo.EditValue));            
            string sql = "";

            if (checkBox1.Checked)
            {
                sql = CreateSqlReport(Convert.ToInt32(txtPeriodo.EditValue), "");
                CondicionBusqueda = Conjunto.GetCondicionReporte("", User.GetUserFilter());
            }                
            else
            {
                sql = CreateSqlReport(Convert.ToInt32(txtPeriodo.EditValue), Conjunto.GetCondicionFromCode(txtConjunto.Text));
                CondicionBusqueda = Conjunto.GetCondicionReporte(txtConjunto.Text, User.GetUserFilter());
            }
            sql = sql.Replace("afp.rut", "dbo.fnRutGuion(afp.rut)");
            sql = sql.Replace("isapre.rut", "dbo.fnRutGuion(isapre.rut)");
            sql = sql.Replace("cajaprevision.rut", "dbo.fnRutGuion(cajaprevision.rut)");
            try
            {
                if (sql.Length > 0)
                {
                    tabla = GetDataSource(sql);
                    //GUARDAMOS INFORMACION EN TABLA DATASOURCE                
                    TablaDataSource = tabla;
                    if (tabla.Rows.Count > 0)
                    {
                        
                        TablaDataSource = tabla;                       
                        btnDocumento.Enabled = true;
                        GenerarArchivo(tabla, FileName, Convert.ToInt32(txtPeriodo.EditValue), Esq.SeparadorCad, Esq.Formato);
                        //GeneraReporte(tabla, Convert.ToInt32(txtPeriodo.EditValue));
                    }
                    else
                    {
                        XtraMessageBox.Show("No se pudo generar archivo", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        btnDocumento.Enabled = false;
                    }
                }
                else
                {
                    XtraMessageBox.Show("No se pudo generar archivo", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    btnDocumento.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("No se pudo generar archivo", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }

        }

        public string GetCadenaOrden()
        {
            ReporteContable rep = new ReporteContable();
            rep.SetInfo();

            StringBuilder str = new StringBuilder();
            List<OrdenContable> n = new List<OrdenContable>();

            str.Append("ORDER BY ");

            if (rep.or1 != 0)
            {
                str.Append($"Col{rep.or1},");
            }
            if (rep.or2 != 0)
            {
                str.Append($"Col{rep.or2},");
            }
            if (rep.or3 != 0)
            {
                str.Append($"Col{rep.or3},");
            }
            if (rep.or4 != 0)
            {
                str.Append($"Col{rep.or4},");
            }
            if (rep.or5 != 0)
            {
                str.Append($"Col{rep.or5},");
            }
            if (rep.or6 != 0)
            {
                str.Append($"Col{rep.or6},");
            }
            if (rep.or7 != 0)
            {
                str.Append($"Col{rep.or7},");
            }
            if (rep.or8 != 0)
            {
                str.Append($"Col{rep.or8},");
            }


            string d = str.ToString();

            if (d.Length > 0)
            {
                if (d.Contains(","))
                    d = d.Substring(0, d.Length - 1);
            }



            return d;
        }

        private void btnSalida_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (Directory.Exists(dialog.SelectedPath))
                {
                    txtSalida.Text = dialog.SelectedPath;
                }
            }            
        }

        #region "DATOS"
        /// <summary>
        /// Genera el sql para consulta reporte.
        /// </summary>
        /// <returns></returns>
        private string CreateSqlReport(int pPeriodo, string pConjunto)
        {
            GrupoContable grup = new GrupoContable();
            List<ConfiguracionItem> Configuraciones = grup.GetInfoCuentas();
            ConfiguracionItem itembus = new ConfiguracionItem();
            string SubDato = "", valorFinal = "";
            Relacion rel = new Relacion();
            Relacion tabla = new Relacion();
            List<Relacion> Relaciones = new List<Relacion>();
            string Sql1 = "", Sql2 = "", Sql3 = "", Sql4 = "";
            string Query = "";
            List<formula> ColumnasTabla = new List<formula>();

            //PARA IR AGREGANDO SQL 
            List<string> GruposSql = new List<string>();
            string concatena = "";

            /*
             * MAPA SQL 
             * ASUMIENTO COMO PARAMETRO "DATO"
             * @DATO FIJO => SELECT DATO
             * @DATO EMPLEADO => SELECT DATO ; JOIN TABLA CORRESPONDIENTE
             * @DATO EMPLEADO => SELECT DATO 
             * @DATO ITEM => SELECT DATO ; JOIN ITEM ; JOIN ITEMTRABAJADOR ; GRUPO DETALLE
             * @DATO ENTIDAD => SELECT DATO; JOIN TABLA 'ENTIDAD'
             * 
             * CODIGOS SELECCION SUPERIOR
             * ---------------------------
             * 1 - DATO FIJO            |
             * 2 - DATO EMPLEADO        |
             * 3 - DATO ITEM            |
             * 4 - DATO ENTIDAD         |
             * ---------------------------
             */

            string SqlFiltroUsuario = "";
            SqlFiltroUsuario = Calculo.GetSqlFiltro(User.GetUserFilter(), "", User.ShowPrivadas(), true);
            //SqlFiltroUsuario = getCondition("", pPeriodo);

            //TABLA PARTIDA 'TRABAJADOR'
            //TABLAS QUE SIEMPRE VAN A ESTAR 'ITEMTRABAJADOR', 'ITEM', 'GRUPOCONTABLE'
            string sqlEmpleado = "", sqlEmpleadoCaja = "";
            string sqlItem = "", sqlItemCaja = "", sqlEntidadCaja = "";                          
            //EN EL CASO DE CAJA Y MUTUAL NO ESTAN DIRECTAMENTE RELACIONADAS CON TRABAJADOR
            //DEBEMOS USA LA TABLA EMPRESA
            string sqlEntidad = "";

            string SqlBase = "SELECT {campos} \n" +
                             "FROM TRABAJADOR\n" +
                             "INNER JOIN itemtrabajador On itemtrabajador.contrato = trabajador.contrato\n" +
                             "AND itemtrabajador.anomes = trabajador.anomes\n" +                             
                             "{linktablas}";
               

            string Footer = "\nWHERE itemtrabajador.anomes={periodo} AND itemtrabajador.coditem={item} AND tipocon={tipocontable} AND suspendido = 0 ";
            string FooterCaja = "\nWHERE itemtrabajador.anomes={periodo} AND (itemtrabajador.coditem={item}) AND tipocon={tipocontable} AND cajaprevision <> 0 AND suspendido=0 ";
            string SqlAgrupa = "GROUP BY {agrupa}";

            string sqlFiltro = $"AND itemtrabajador.contrato IN (select CONTRATO from TRABAJADOR {SqlFiltroUsuario} [condition])";
            string sqlFiltroCaja = $"AND itemtrabajador.contrato IN (select CONTRATO from TRABAJADOR {SqlFiltroUsuario} [condition])";            
            string sqlParCajaPrev = "";

            ///SOLO PARA RESET
            string sqlBaseReset = "", sqlFooterReset = "", sqlAgrupaReset = "", Agrupa = "", 
                AgrupaCaja = "", SqlFooterResCaj = "", sqlFiltroReset = "", sqlFiltroResCaj = "";

            //SON TODOS LOS CAMPOS (REPRESENTAN CADA COLUMNA DEFINIDA EN EL DETALLE DE LA CUENTA CONTABLE...)
            string SqlParametros = "", da= "";
            int Tipo = 0, contar = 0, colN = 1;

            if (Configuraciones.Count > 0)
            {
                sqlFooterReset = Footer;
                sqlBaseReset = SqlBase;
                sqlAgrupaReset = SqlAgrupa;
                SqlFooterResCaj = FooterCaja;
                sqlFiltroReset = sqlFiltro;
                sqlFiltroResCaj = sqlFiltroCaja;

                //RECORREMOS Y OBTENEMOS CAJA COLUMNA

                for (int i = 0; i < Configuraciones.Count; i++)
                {                   

                    //Si tipo cambia guardamos consulta y volvemos a empezar...
                    if (i > 0)
                    {
                        //CAMBIA EL TIPO 
                        if (Configuraciones[i-1].Tipo != Configuraciones[i].Tipo || Configuraciones[i-1].ItemObservado != Configuraciones[i].ItemObservado)
                        {
                            //GUARDAMOS TODAS LAS RELACIONES ENCONTRADAS 
                            //da = sqlEmpleado + sqlEntidad + sqlItem;

                            if (sqlEntidad.Contains("afp.id=trabajador.afp"))                                
                                sqlFiltro = sqlFiltro + " AND afp <> 0";

                            LlenaData(Configuraciones[i-1], SqlBase, SqlParametros, sqlEmpleado, sqlEntidad, sqlItem, 
                                Footer, Agrupa, SqlAgrupa, pPeriodo, GruposSql, sqlFiltro, Configuraciones[i-1].AgrupaRut, pConjunto, false, true);

                            if (Configuraciones[i-1].ItemObservado == "PREVISI" || Configuraciones[i-1].ItemObservado == "SCEMPRE" || Configuraciones[i-1].ItemObservado == "SCEMPLE")
                            {
                                if (sqlEntidad.Contains("afp.id=trabajador.afp"))
                                {
                                    LlenaData(Configuraciones[i - 1], SqlBase, sqlParCajaPrev, sqlEmpleadoCaja, sqlEntidadCaja, sqlItemCaja,
                                FooterCaja, AgrupaCaja, SqlAgrupa, pPeriodo, GruposSql, sqlFiltroCaja, Configuraciones[i - 1].AgrupaRut, pConjunto);
                                }                               
                            }

                            SqlBase = sqlBaseReset;
                            SqlParametros = "";
                            sqlEmpleado = "";
                            sqlEntidad = "";
                            sqlItem = "";
                            da = "";
                            Footer = sqlFooterReset;
                            SqlAgrupa = sqlAgrupaReset;
                            Agrupa = "";
                            sqlEmpleadoCaja = "";
                            sqlParCajaPrev = "";
                            sqlEntidadCaja = "";
                            sqlItemCaja = "";
                            AgrupaCaja = "";
                            FooterCaja = SqlFooterResCaj;
                            sqlFiltro = sqlFiltroReset;
                            sqlFiltroCaja = sqlFiltroResCaj;
                            colN = 1;
                        }
                    }          

                    //SE SELECCIONÓ DATO FIJO?
                    if (Configuraciones[i].TipoSeleccionado == 1)
                    {
                        //NUMERO?
                        if (Configuraciones[i].SubtipoSeleccionado == "1")
                        {
                            valorFinal = Configuraciones[i].ValorSeleccionado;
                            //SqlParametros = SqlParametros + $"'{valorFinal}', ";
                        }
                        //TEXTO
                        else if (Configuraciones[i].SubtipoSeleccionado == "2")
                        {
                            valorFinal = Configuraciones[i].ValorSeleccionado;
                            //SqlParametros = SqlParametros + $"'{valorFinal}', ";
                        }
                        //FECHA
                        else if (Configuraciones[i].SubtipoSeleccionado == "3")
                        {
                            valorFinal = Configuraciones[i].ValorSeleccionado;
                            //SqlParametros = SqlParametros + $"'{valorFinal}', ";
                        }

                        SqlParametros = SqlParametros + $"(SELECT '{valorFinal}') as col{colN}, ";

                        if (Configuraciones[i].ItemObservado == "PREVISI" || Configuraciones[i].ItemObservado == "SCEMPRE" || Configuraciones[i].ItemObservado == "SCEMPLE")
                            sqlParCajaPrev = sqlParCajaPrev + $"(SELECT '{valorFinal}') as col{colN}, ";
                    }
                    //SE SELECCIONÓ DATO EMPLEADO?
                    else if (Configuraciones[i].TipoSeleccionado == 2)
                    {
                        //REPRESENTA EL CAMPO DE LA TABLA (O REFERENCIA )
                        SubDato = Configuraciones[i].SubtipoSeleccionado;
                        if (SubDato.Length > 0)
                        {
                            //ES FORANEO
                            valorFinal = Configuraciones[i].ValorSeleccionado;
                            Relaciones = rel.GetRelaciones("trabajador");
                            if (Relaciones.Count > 0)
                            {
                                //ES CLAVE FORANEA
                                if (rel.EsForaneo(Relaciones, SubDato))
                                {
                                    tabla = rel.GetInfo(Relaciones, SubDato);
                                    //REALIZAR CONSULTA
                                    //OBTENER EL VALOR CAMPO DESDE TABLA FORANEA (JOIN ENTRE TABLAS...)
                                    SqlParametros = SqlParametros + $"{tabla.TablaRef}.{valorFinal} as col{colN}, ";
                                    Agrupa = Agrupa + $"{tabla.TablaRef}.{valorFinal}, ";
                                    if (sqlEmpleado.Contains($"{tabla.TablaRef.ToLower()}.{tabla.ColumnaRef.ToLower()}") == false)
                                        sqlEmpleado = sqlEmpleado + $"INNER JOIN {tabla.TablaRef.ToLower()} ON {tabla.TablaOrigen.ToLower()}.{tabla.ColumnaOrigen.ToLower()} = {tabla.TablaRef.ToLower()}.{tabla.ColumnaRef.ToLower()}\n ";

                                    if (Configuraciones[i].ItemObservado == "PREVISI" || Configuraciones[i].ItemObservado == "SCEMPRE" || Configuraciones[i].ItemObservado == "SCEMPLE")
                                    {
                                        sqlParCajaPrev = sqlParCajaPrev + $"{tabla.TablaRef}.{valorFinal} as col{colN}, ";
                                        AgrupaCaja = AgrupaCaja + $"{tabla.TablaRef}.{valorFinal}, ";

                                        if (sqlEmpleadoCaja.Contains($"{tabla.TablaRef.ToLower()}.{tabla.ColumnaRef.ToLower()}") == false)
                                            sqlEmpleadoCaja = sqlEmpleadoCaja + $"INNER JOIN {tabla.TablaRef.ToLower()} ON {tabla.TablaOrigen.ToLower()}.{tabla.ColumnaOrigen.ToLower()} = {tabla.TablaRef.ToLower()}.{tabla.ColumnaRef.ToLower()}\n ";

                                       
                                    }                                   
                                }
                                else
                                {
                                    //NO HAY QUE LINKEAR A OTRA TABLA
                                    //OBTENEMOS EL VALOR DIRECTAMENTE DESDE TABLA TRABAJADOR...
                                    //Sql2 = $"SELECT {valorFinal} FROM trabajador";                                    
                                    if(valorFinal.ToLower().Contains("nombre"))
                                        SqlParametros = SqlParametros + $"CONCAT(apepaterno, ' ', apematerno, ' ', trabajador.{valorFinal}) as col{colN}, ";
                                    else
                                        SqlParametros = SqlParametros + $"trabajador.{valorFinal} as col{colN}, ";

                                    if(valorFinal.ToLower().Contains("nombre"))
                                        Agrupa = Agrupa + $"trabajador.{valorFinal}, trabajador.apepaterno, trabajador.apematerno, ";
                                    else
                                        Agrupa = Agrupa + $"trabajador.{valorFinal}, ";

                                    if (Configuraciones[i].ItemObservado == "PREVISI" || Configuraciones[i].ItemObservado == "SCEMPRE" || Configuraciones[i].ItemObservado == "SCEMPLE")
                                    {
                                        sqlParCajaPrev = sqlParCajaPrev + $"trabajador.{valorFinal} as col{colN}, ";
                                        AgrupaCaja = AgrupaCaja + $"trabajador.{valorFinal}, ";
                                        //sqlEmpleadoCaja = sqlEmpleadoCaja + $"INNER JOIN {tabla.TablaRef.ToLower()} ON {tabla.TablaOrigen.ToLower()}.{tabla.ColumnaOrigen.ToLower()} = {tabla.TablaRef.ToLower()}.{tabla.ColumnaRef.ToLower()}\n ";
                                    }
                                }
                            }
                        }
                        else
                        {
                            //NO HAY SUBSELECCION...
                            //USAMOS VALOR FINAL
                            valorFinal = Configuraciones[i].ValorSeleccionado;
                            SqlParametros = SqlParametros + $"{valorFinal} as col{colN}, ";
                            Agrupa = Agrupa + $"{valorFinal}, ";

                            if (Configuraciones[i].ItemObservado == "PREVISI" || Configuraciones[i].ItemObservado == "SCEMPRE" || Configuraciones[i].ItemObservado == "SCEMPLE")
                            {
                                sqlParCajaPrev = sqlParCajaPrev + $"{valorFinal} as col{colN}, ";
                                AgrupaCaja = AgrupaCaja + $"{valorFinal}, ";
                            }
                        }

                        // Query = Query + $"({Sql2}),\n";
                    }
                    //SE SELECCIONÓ DATO DE ITEM???
                    else if (Configuraciones[i].TipoSeleccionado == 3)
                    {
                        valorFinal = Configuraciones[i].ValorSeleccionado;

                        if (valorFinal.ToLower().Contains("grupocontable."))
                        {
                            /*Es una columna de la tabla grupo contable*/
                            
                            SqlParametros = SqlParametros + $"{valorFinal} col{colN}, ";
                            Agrupa = Agrupa + $"{valorFinal}, ";

                            if (sqlItem.Contains("grupocontable.coditem=itemtrabajador.coditem") == false)
                                sqlItem = sqlItem + $"INNER JOIN grupocontable ON grupocontable.coditem=itemtrabajador.coditem\n";

                            if (Configuraciones[i].ItemObservado == "PREVISI" || Configuraciones[i].ItemObservado == "SCEMPRE" || Configuraciones[i].ItemObservado == "SCEMPLE")
                            {
                                sqlParCajaPrev = sqlParCajaPrev + $"{valorFinal} col{colN}, ";
                                AgrupaCaja = AgrupaCaja + $"{valorFinal}, ";
                                if (sqlItemCaja.Contains("grupocontable.coditem=itemtrabajador.coditem") == false)
                                    sqlItemCaja = sqlItemCaja + $"INNER JOIN grupocontable ON grupocontable.coditem=itemtrabajador.coditem\n"; ;
                            }
                        }
                        else if (valorFinal.ToLower().Contains("item."))
                        {
                            /*Es una columna de la tabla item*/
                            SqlParametros = SqlParametros + $"{valorFinal} col{colN}, ";
                            Agrupa = Agrupa + $"{valorFinal}, ";

                            if (sqlItem.Contains("item.coditem=itemtrabajador.coditem") == false)
                                sqlItem = sqlItem + $"INNER JOIN item ON item.coditem=itemtrabajador.coditem\n";

                            if (Configuraciones[i].ItemObservado == "PREVISI" || Configuraciones[i].ItemObservado == "SCEMPRE" || Configuraciones[i].ItemObservado == "SCEMPLE")
                            {
                                sqlParCajaPrev = sqlParCajaPrev + $"{valorFinal} col{colN}, ";
                                AgrupaCaja = AgrupaCaja + $"{valorFinal}, ";
                                if (sqlItemCaja.Contains("item.coditem=itemtrabajador.coditem") == false)
                                    sqlItemCaja = sqlItemCaja + $"INNER JOIN item ON item.coditem=itemtrabajador.coditem\n";
                            }
                        }
                        else if (valorFinal.ToLower().Contains("itemtrabajador."))
                        {
                            /*Es una columna de la tabla itemtrabajador*/
                            //haberes
                            if (Configuraciones[i].Tipo == 1 && (valorFinal.ToLower().Contains("valor") || valorFinal.ToLower().Contains("valorcalculado")))
                            {
                                SqlParametros = SqlParametros + $"round(SUM({valorFinal}), 0) as col{colN}, 0 as col{colN + 1},";
                                colN++;
                            }
                            else if (Configuraciones[i].Tipo == 2 && (valorFinal.ToLower().Contains("valor") || valorFinal.ToLower().Contains("valorcalculado")))
                            {
                                SqlParametros = SqlParametros + $"0 as col{colN}, round(SUM({valorFinal}), 0) as col{colN + 1}, ";
                                colN++;
                            }
                            else
                            {
                                SqlParametros = SqlParametros + $"{valorFinal} as col{colN},";
                                Agrupa = Agrupa + $"{valorFinal}, ";
                            }                                                          

                            if (sqlItem.Contains("item.coditem=itemtrabajador.coditem") == false)
                                sqlItem = sqlItem + $"INNER JOIN item ON item.coditem=itemtrabajador.coditem\n";

                            if (Configuraciones[i].ItemObservado == "PREVISI" || Configuraciones[i].ItemObservado == "SCEMPRE" || Configuraciones[i].ItemObservado == "SCEMPLE")
                            {
                                //PARA REGIMEN ANTIGUO
                                if (Configuraciones[i].Tipo == 1 && (valorFinal.ToLower().Contains("valor") || valorFinal.ToLower().Contains("valorcalculado")))
                                {
                                    sqlParCajaPrev = sqlParCajaPrev + $"round(SUM({valorFinal}), 0) as col{colN - 1}, 0 as col{colN},";
                                }
                                else if (Configuraciones[i].Tipo == 2 && (valorFinal.ToLower().Contains("valor") || valorFinal.ToLower().Contains("valorcalculado")))
                                {
                                    sqlParCajaPrev = sqlParCajaPrev + $"0 as col{colN - 1}, round(SUM({valorFinal}), 0) as col{colN}, ";
                                }
                                else
                                {
                                    sqlParCajaPrev = sqlParCajaPrev + $"{valorFinal} col{colN}, ";
                                    AgrupaCaja = AgrupaCaja + $"{valorFinal}, ";
                                }                               
                                
                                if (sqlItemCaja.Contains("item.coditem=itemtrabajador.coditem") == false)
                                    sqlItemCaja = sqlItemCaja + $"INNER JOIN item ON item.coditem=itemtrabajador.coditem\n";
                            }
                        }

                        //ColumnasTabla = rel.GetColumnsTable("grupocontable");
                        //if (ColumnasTabla.Count > 0)
                        //{
                        //    //COLUMNA EXISTE EN GRUPO CONTABLE???
                        //    if (rel.ExisteColumna(valorFinal, ColumnasTabla))
                        //    {
                        //        //EXISTE columna en tabla grupocontable
                        //        SqlParametros = SqlParametros + $"grupocontable.{valorFinal} col{i + 1}, ";
                        //        Agrupa = Agrupa + $"grupocontable.{valorFinal}, ";
                        //        if (sqlItem.Contains("grupocontable.coditem=itemtrabajador.coditem") == false)
                        //            sqlItem = sqlItem + $"INNER JOIN grupocontable ON grupocontable.coditem=itemtrabajador.coditem\n";

                        //        if (Configuraciones[i].ItemObservado == "PREVISI" || Configuraciones[i].ItemObservado == "SCEMPRE" || Configuraciones[i].ItemObservado == "SCEMPLE")
                        //        {
                        //            sqlParCajaPrev = sqlParCajaPrev + $"grupocontable.{valorFinal} col{i + 1}, ";                                    
                        //            AgrupaCaja = AgrupaCaja + $"grupocontable.{valorFinal}, ";
                        //            if(sqlItemCaja.Contains("grupocontable.coditem=itemtrabajador.coditem") == false)
                        //                sqlItemCaja = sqlItemCaja + $"INNER JOIN grupocontable ON grupocontable.coditem=itemtrabajador.coditem\n"; ;
                        //        }                             
                        //    }
                        //    else
                        //    {
                        //        //COLUMNAS EXISTE EN ITEM
                        //        ColumnasTabla = rel.GetColumnsTable("item");
                        //        if (ColumnasTabla.Count > 0)
                        //        {
                        //            //EXISTE COLUMNA EN TABLA ITEM
                        //            if (rel.ExisteColumna(valorFinal, ColumnasTabla))
                        //            {
                        //                SqlParametros = SqlParametros + $"item.{valorFinal} col{i + 1}, ";
                        //                Agrupa = Agrupa + $"item.{valorFinal}, ";
                        //                if (sqlItem.Contains("item.coditem=itemtrabajador.coditem") == false)
                        //                    sqlItem = sqlItem + $"INNER JOIN item ON item.coditem=itemtrabajador.coditem\n";


                        //                if (Configuraciones[i].ItemObservado == "PREVISI" || Configuraciones[i].ItemObservado == "SCEMPRE" || Configuraciones[i].ItemObservado == "SCEMPLE")
                        //                {
                        //                    sqlParCajaPrev = sqlParCajaPrev + $"item.{valorFinal} col{i + 1}, ";                                            
                        //                    AgrupaCaja = AgrupaCaja + $"item.{valorFinal}, ";
                        //                    if(sqlItemCaja.Contains("item.coditem=itemtrabajador.coditem") == false)
                        //                        sqlItemCaja = sqlItemCaja + $"INNER JOIN item ON item.coditem=itemtrabajador.coditem\n";
                        //                }
                                       
                        //            }
                        //        }
                        //    }
                        //}

                    }
                    //SE SELECCIONÓ DATO ENTIDAD???
                    else if (Configuraciones[i].TipoSeleccionado == 4)
                    {
                        //CORRESPONDE A TABLA A LINKEAR
                        SubDato = Configuraciones[i].SubtipoSeleccionado + "";
                        valorFinal = Configuraciones[i].ValorSeleccionado;

                        if (SubDato == "1")
                        {
                            if (sqlEntidad.Contains("isapre.id=trabajador.salud") == false)
                                sqlEntidad = sqlEntidad + $"INNER JOIN isapre ON isapre.id=trabajador.salud \n";
                            SqlParametros = SqlParametros + $"isapre.{valorFinal} col{colN}, ";
                            Agrupa = Agrupa + $"isapre.{valorFinal}, ";

                            if (Configuraciones[i].ItemObservado == "PREVISI" || Configuraciones[i].ItemObservado == "SCEMPRE" || Configuraciones[i].ItemObservado == "SCEMPLE")
                            {
                                sqlParCajaPrev = sqlParCajaPrev + $"isapre.{valorFinal} col{colN}, ";                                
                                AgrupaCaja = AgrupaCaja + $"isapre.{valorFinal}, ";
                                if(sqlEntidadCaja.Contains("isapre.id=trabajador.salud") == false)
                                    sqlEntidadCaja = sqlEntidadCaja + $"INNER JOIN isapre ON isapre.id=trabajador.salud \n"; ;
                            }

                        }
                        else if (SubDato == "2")
                        {
                            if (sqlEntidad.Contains("afp.id=trabajador.afp") == false)
                                sqlEntidad = sqlEntidad + $"INNER JOIN afp ON afp.id=trabajador.afp \n";
                            SqlParametros = SqlParametros + $"afp.{valorFinal} col{colN}, ";
                            Agrupa = Agrupa + $"afp.{valorFinal}, ";

                            if (Configuraciones[i].ItemObservado == "PREVISI" || Configuraciones[i].ItemObservado == "SCEMPRE" || Configuraciones[i].ItemObservado == "SCEMPLE")
                            {
                                sqlParCajaPrev = sqlParCajaPrev + $"cajaprevision.{valorFinal} col{colN}, ";                                
                                AgrupaCaja = AgrupaCaja + $"cajaprevision.{valorFinal}, ";
                                if(sqlEntidadCaja.Contains("cajaprevision.id=trabajador.cajaprevision") == false)
                                    sqlEntidadCaja = sqlEntidadCaja + $"INNER JOIN cajaprevision ON cajaprevision.id=trabajador.cajaprevision \n";
                            }

                        }
                        else if (SubDato == "3")
                        {
                            //sqlEntidad = sqlEntidad + $"INNER JOIN {SubDato.ToLower()} ON mutual.id=trabajador.afp \n";
                            if (SqlParametros.Contains("empresa.nombremut=mutual.id") == false)
                                SqlParametros = SqlParametros + $"(SELECT mutual.{valorFinal} FROM mutual INNER JOIN empresa ON empresa.nombremut=mutual.id) col{colN},";
                            //Agrupa = Agrupa + $"mutual.{valorFinal},";

                            if (sqlParCajaPrev.Contains("empresa.nombremut=mutual.id") == false)
                            {
                                if (Configuraciones[i].ItemObservado == "PREVISI" || Configuraciones[i].ItemObservado == "SCEMPRE" || Configuraciones[i].ItemObservado == "SCEMPLE")
                                {
                                    sqlParCajaPrev = sqlParCajaPrev + $"(SELECT mutual.{valorFinal} FROM mutual INNER JOIN empresa ON empresa.nombremut=mutual.id) col{colN},";
                                    //sqlEntidadCaja = sqlEntidadCaja + $"INNER JOIN afp ON afp.id=trabajador.afp \n";
                                }
                            }
                        }
                        else if (SubDato == "4")
                        {
                            //...
                            if (SqlParametros.Contains("empresa.nombreccaf=cajacompensacion.id") == false)
                                SqlParametros = SqlParametros + $"(SELECT cajacompensacion.{valorFinal} from cajaCompensacion INNER JOIN empresa on empresa.nombreccaf=cajacompensacion.id) col{colN},";
                            //Agrupa = Agrupa + $"grupocontable.{valorFinal},";

                            if (sqlParCajaPrev.Contains("empresa.nombreccaf=cajacompensacion.id") == false)
                            {
                                if (Configuraciones[i].ItemObservado == "PREVISI" || Configuraciones[i].ItemObservado == "SCEMPRE" || Configuraciones[i].ItemObservado == "SCEMPLE")
                                {
                                    sqlParCajaPrev = sqlParCajaPrev + $"(SELECT cajacompensacion.{valorFinal} from cajaCompensacion INNER JOIN empresa on empresa.nombreccaf=cajacompensacion.id) col{colN},";
                                }
                            }
                        }
                    }
                    //SI ES EL ULTIMO ELEMENTO DE LA LISTA???
                    if (i == Configuraciones.Count - 1)
                    {
                        LlenaData(Configuraciones[i - 1], SqlBase, SqlParametros, sqlEmpleado, sqlEntidad, sqlItem,
                               Footer, Agrupa, SqlAgrupa, pPeriodo, GruposSql, sqlFiltro, Configuraciones[i-1].AgrupaRut, pConjunto, true, true);

                        if (Configuraciones[i - 1].ItemObservado == "PREVISI" || Configuraciones[i].ItemObservado == "SCEMPRE" || Configuraciones[i].ItemObservado == "SCEMPLE")
                        {
                            if (sqlEntidad.Contains("afp.id=trabajador.afp"))
                            {
                                LlenaData(Configuraciones[i - 1], SqlBase, sqlParCajaPrev, sqlEmpleadoCaja, sqlEntidadCaja, sqlItemCaja,
                        FooterCaja, AgrupaCaja, SqlAgrupa, pPeriodo, GruposSql, sqlFiltro,  Configuraciones[i - 1].AgrupaRut, pConjunto);
                            }                         
                        }            
                    }

                    //if (Configuraciones[i].ItemObservado == "PREVISI")
                    //    itembus = Configuraciones[i];

                    colN++;
                }

                int gr = GruposSql.Count;
                
                if (GruposSql.Count > 0)
                {

                    foreach (var item in GruposSql)
                    {
                        concatena = concatena + item;
                    }                 
                }
            }

            string order = "";
            order = GetCadenaOrden();

            //return (concatena + " ORDER BY col4, col9, col14, col5 ");
            return concatena + " " +  order;
        }

        /// <summary>
        /// Agrega cada sql a hashtable
        /// </summary>
        private void LlenaData(ConfiguracionItem pConfiguracion, string pSqlBase, 
            string pSqlParametros, string pSqlEmpleado, string pSqlEntidad, 
            string pSqlItem, string pFooter, string pAgrupa,string pSqlAgrupa, 
            int pPeriodo, List<string> pListado, string pFiltro, bool pAgrupaRes, string pCondicionalAdicional, 
            bool? UltimoRegistro = false, bool? NoEsCaja = false)
        {
            string Relaciones = "", UserFilter = "";
            UserFilter = User.GetUserFilter();

            if (pConfiguracion != null)
            {
                Relaciones = pSqlEmpleado + pSqlEntidad + pSqlItem;
                if (Relaciones.Contains("grupocontable.coditem") == false)
                    Relaciones = Relaciones + " INNER JOIN grupocontable On grupocontable.coditem = item.coditem";

                ////Debito
                //if (pConfiguracion.Tipo == 1)
                //{
                //    //AGRUPA???
                //    pSqlParametros = pSqlParametros + "SUM(valorcalculado) as col7, 0 as col8";   
                //}
                ////Credito
                //else if (pConfiguracion.Tipo == 2)
                //{
                //   pSqlParametros = pSqlParametros + " 0 as col7, SUM(valorcalculado) as col8";
                //}


                //quitar la ultima coma a sqlparametros
                pSqlParametros.TrimEnd();
                pSqlParametros = pSqlParametros.Substring(0, pSqlParametros.Length - 2);

                pSqlBase = pSqlBase.Replace("{campos}", pSqlParametros);
                pSqlBase = pSqlBase.Replace("{linktablas}", Relaciones);
                //AQUI REEMPLAZAR POR PREVISI, SCEMPLE, SCEMPRE EN EL CASO DE CAJA DE PREVISION
                if(NoEsCaja == false)
                    pFooter = pFooter.Replace("{item}", $"'{pConfiguracion.ItemObservado}'");
                else
                    pFooter = pFooter.Replace("{item}", $"'{pConfiguracion.ItemObservado}'");

                pFooter = pFooter.Replace("{tipocontable}", $"{pConfiguracion.Tipo}");
                pFooter = pFooter.Replace("{periodo}", pPeriodo.ToString());

                if (UserFilter == "0")
                {
                    //No hay filtro de usuario ni condicion adicional
                    if (pCondicionalAdicional == "")
                    {
                        pFiltro = pFiltro.Replace("[condition]", "");
                    }
                    else
                    {
                        //No hay filtro de usuario pero si condicion adicional
                        pFiltro = pFiltro.Replace("[condition]", " WHERE " + pCondicionalAdicional + " AND anomes=" + pPeriodo);
                    }
                }
                else
                {
                    //Si hay filtro de usuario pero no hay condicion adicional
                    if (pCondicionalAdicional == "")
                        pFiltro = pFiltro.Replace("[condition]", "");
                    else
                        //SE CUMPLEN LAS DOS
                        pFiltro = pFiltro.Replace("[condition]", " AND " + pCondicionalAdicional + " AND anomes=" + pPeriodo);
                }

                pFooter = pFooter + " " + pFiltro;         

                //SI ITEM ES PREVISI AGREGAMOS A FOOTER CONDICION CAJAPREVISION
                if (pConfiguracion.ItemObservado == "PREVISI" &&  NoEsCaja == true)
                    pFooter = pFooter + " AND cajaprevision = 0";

                if (pAgrupa.Length > 0)
                    pSqlAgrupa = pSqlAgrupa.Replace("{agrupa}", pAgrupa.Substring(0, pAgrupa.Length - 2));
                else
                    pSqlAgrupa = pSqlAgrupa.Replace("{agrupa}", "");

                if (UltimoRegistro == false)
                {
                    //AGRUPA LOS RESULTADOS???                 
                    pSqlBase = pSqlBase + pFooter + pSqlAgrupa + "\nUNION\n";
                }
                else
                {
                    pSqlBase = pSqlBase + pFooter + pSqlAgrupa;                    
   
                }                   

                //AGRGAMOS A LISTADO
                pListado.Add(pSqlBase);

                //LIMPIAMOS TODAS LAS CADENAS...
                //pSqlBase = sqlBaseReset;
                //pSqlParametros = "";
                //pSqlEmpleado = "";
                //pSqlEntidad = "";
                //pSqlItem = "";                
                //pFooter = sqlFooterReset;
                //pSqlAgrupa = sqlAgrupaReset;
                //pAgrupa = "";
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
                    if (ShowPrivados == false)
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
        /// Genera el sql para consulta reporte.
        /// </summary>
        /// <returns></returns>
        private string CreateSqlFile(int pPeriodo)
        {
            GrupoContable grup = new GrupoContable();
            List<ConfiguracionItem> Configuraciones = grup.GetInfoCuentas();
            string SubDato = "", valorFinal = "";
            Relacion rel = new Relacion();
            Relacion tabla = new Relacion();
            List<Relacion> Relaciones = new List<Relacion>();
            string Sql1 = "", Sql2 = "", Sql3 = "", Sql4 = "";
            string Query = "";
            List<formula> ColumnasTabla = new List<formula>();

            //PARA IR AGREGANDO SQL 
            List<string> GruposSql = new List<string>();
            string concatena = "";

            /*
             * MAPA SQL 
             * ASUMIENTO COMO PARAMETRO "DATO"
             * @DATO FIJO => SELECT DATO
             * @DATO EMPLEADO => SELECT DATO ; JOIN TABLA CORRESPONDIENTE
             * @DATO EMPLEADO => SELECT DATO 
             * @DATO ITEM => SELECT DATO ; JOIN ITEM ; JOIN ITEMTRABAJADOR ; GRUPO DETALLE
             * @DATO ENTIDAD => SELECT DATO; JOIN TABLA 'ENTIDAD'
             * 
             * CODIGOS SELECCION SUPERIOR
             * ---------------------------
             * 1 - DATO FIJO            |
             * 2 - DATO EMPLEADO        |
             * 3 - DATO ITEM            |
             * 4 - DATO ENTIDAD         |
             * ---------------------------
             */

            //TABLA PARTIDA 'TRABAJADOR'
            //TABLAS QUE SIEMPRE VAN A ESTAR 'ITEMTRABAJADOR', 'ITEM', 'GRUPOCONTABLE'
            string sqlEmpleado = "";
            string sqlItem = "";
            //EN EL CASO DE CAJA Y MUTUAL NO ESTAN DIRECTAMENTE RELACIONADAS CON TRABAJADOR
            //DEBEMOS USA LA TABLA EMPRESA
            string sqlEntidad = "";

            string SqlBase = "SELECT {campos} \n" +
                             "FROM TRABAJADOR\n" +
                             "INNER JOIN itemtrabajador On itemtrabajador.contrato = trabajador.contrato\n" +
                             "AND itemtrabajador.anomes = trabajador.anomes\n" +
                             "{linktablas}";

            ///SOLO PARA RESET
            string sqlBaseReset = "", sqlFooterReset = "", sqlAgrupaReset = "", Agrupa = "";

            string Footer = "\nWHERE itemtrabajador.anomes={periodo} AND itemtrabajador.coditem={item} AND tipocon={tipocontable}";

            string SqlAgrupa = "GROUP BY {agrupa}";

            //SON TODOS LOS CAMPOS (REPRESENTAN CADA COLUMNA DEFINIDA EN EL DETALLE DE LA CUENTA CONTABLE...)
            string SqlParametros = "", da = "";
            int Tipo = 0, contar = 0;

            if (Configuraciones.Count > 0)
            {
                sqlFooterReset = Footer;
                sqlBaseReset = SqlBase;
                sqlAgrupaReset = SqlAgrupa;
                //RECORREMOS Y OBTENEMOS CAJA COLUMNA

                for (int i = 0; i < Configuraciones.Count; i++)
                {
                    //Si tipo cambia guardamos consulta y volvemos a empezar...
                    if (i > 0)
                    {
                        //CAMBIA EL TIPO 
                        if (Configuraciones[i - 1].Tipo != Configuraciones[i].Tipo || Configuraciones[i - 1].ItemObservado != Configuraciones[i].ItemObservado)
                        {
                            //GUARDAMOS TODAS LAAS RELACIONES ENCONTRADAS 
                            da = sqlEmpleado + sqlEntidad + sqlItem;

                            //DEBITO
                            if (Configuraciones[i - 1].Tipo == 1)
                                SqlParametros = SqlParametros + "SUM(valorcalculado) as col7, 0 as col8";
                            if (Configuraciones[i - 1].Tipo == 2)
                                SqlParametros = SqlParametros + "0 as col7, SUM(valorcalculado) as col8 ";

                            //if (SqlParametros.Length > 0)
                            //    SqlParametros = SqlParametros.Substring(0, SqlParametros.Length - 2);

                            //REPLACE SQLBASE
                            SqlBase = SqlBase.Replace("{campos}", SqlParametros);
                            SqlBase = SqlBase.Replace("{linktablas}", da);
                            Footer = Footer.Replace("{item}", $"'{Configuraciones[i - 1].ItemObservado}'");
                            Footer = Footer.Replace("{tipocontable}", $"{Configuraciones[i - 1].Tipo}");
                            Footer = Footer.Replace("{periodo}", pPeriodo.ToString());
                            if (Agrupa.Length > 0)
                                SqlAgrupa = SqlAgrupa.Replace("{agrupa}", Agrupa.Substring(0, Agrupa.Length - 2));
                            SqlBase = SqlBase + Footer + SqlAgrupa + "\nUNION\n";
                     

                            ///Guardamos sql.
                            GruposSql.Add(SqlBase);

                            //GUARDAMOS EL NUEVO TIPO
                            //Tipo = elemento.Tipo;

                            SqlBase = sqlBaseReset;
                            SqlParametros = "";
                            sqlEmpleado = "";
                            sqlEntidad = "";
                            sqlItem = "";
                            da = "";
                            Footer = sqlFooterReset;
                            SqlAgrupa = sqlAgrupaReset;
                            Agrupa = "";
                        }
                    }

                    //SE SELECCIONÓ DATO FIJO?
                    if (Configuraciones[i].TipoSeleccionado == 1)
                    {
                        //NUMERO?
                        if (Configuraciones[i].SubtipoSeleccionado == "1")
                        {
                            valorFinal = Configuraciones[i].ValorSeleccionado;
                            //SqlParametros = SqlParametros + $"'{valorFinal}', ";
                        }
                        //TEXTO
                        else if (Configuraciones[i].SubtipoSeleccionado == "2")
                        {
                            valorFinal = Configuraciones[i].ValorSeleccionado;
                            //SqlParametros = SqlParametros + $"'{valorFinal}', ";
                        }
                        //FECHA
                        else if (Configuraciones[i].SubtipoSeleccionado == "3")
                        {
                            valorFinal = Configuraciones[i].ValorSeleccionado;
                            //SqlParametros = SqlParametros + $"'{valorFinal}', ";
                        }

                        SqlParametros = SqlParametros + $"(SELECT '{valorFinal}') as col{i + 1}, ";
                    }
                    //SE SELECCIONÓ DATO EMPLEADO?
                    else if (Configuraciones[i].TipoSeleccionado == 2)
                    {
                        //REPRESENTA EL CAMPO DE LA TABLA (O REFERENCIA )
                        SubDato = Configuraciones[i].SubtipoSeleccionado;
                        if (SubDato.Length > 0)
                        {
                            //ES FORANEO
                            valorFinal = Configuraciones[i].ValorSeleccionado;
                            Relaciones = rel.GetRelaciones("trabajador");
                            if (Relaciones.Count > 0)
                            {
                                //ES CLAVE FORANEA
                                if (rel.EsForaneo(Relaciones, SubDato))
                                {
                                    tabla = rel.GetInfo(Relaciones, SubDato);
                                    //REALIZAR CONSULTA
                                    //OBTENER EL VALOR CAMPO DESDE TABLA FORANEA (JOIN ENTRE TABLAS...)
                                    SqlParametros = SqlParametros + $"{tabla.TablaRef}.{valorFinal} as col{i + 1}, ";
                                    Agrupa = Agrupa + $"{tabla.TablaRef}.{valorFinal}, ";
                                    if (sqlEmpleado.Contains($"{tabla.TablaRef.ToLower()}.{tabla.ColumnaRef.ToLower()}") == false)
                                        sqlEmpleado = sqlEmpleado + $"INNER JOIN {tabla.TablaRef.ToLower()} ON {tabla.TablaOrigen.ToLower()}.{tabla.ColumnaOrigen.ToLower()} = {tabla.TablaRef.ToLower()}.{tabla.ColumnaRef.ToLower()}\n ";
                                }
                                else
                                {
                                    //NO HAY QUE LINKEAR A OTRA TABLA
                                    //OBTENEMOS EL VALOR DIRECTAMENTE DESDE TABLA TRABAJADOR...
                                    //Sql2 = $"SELECT {valorFinal} FROM trabajador";                                    
                                    SqlParametros = SqlParametros + $"trabajador.{valorFinal} as col{i + 1}, ";
                                    Agrupa = Agrupa + $"trabajador.{valorFinal}, ";
                                }
                            }
                        }
                        else
                        {
                            //NO HAY SUBSELECCION...
                            //USAMOS VALOR FINAL
                            valorFinal = Configuraciones[i].ValorSeleccionado;
                            SqlParametros = SqlParametros + $"{valorFinal} as col{i + 1}, ";
                            Agrupa = Agrupa + $"{valorFinal}, ";
                        }

                        // Query = Query + $"({Sql2}),\n";
                    }
                    //SE SELECCIONÓ DATO DE ITEM???
                    else if (Configuraciones[i].TipoSeleccionado == 3)
                    {
                        valorFinal = Configuraciones[i].ValorSeleccionado;
                        ColumnasTabla = rel.GetColumnsTable("grupocontable");
                        if (ColumnasTabla.Count > 0)
                        {
                            //COLUMNA EXISTE EN GRUPO CONTABLE???
                            if (rel.ExisteColumna(valorFinal, ColumnasTabla))
                            {
                                //EXISTE columna en tabla grupocontabla
                                SqlParametros = SqlParametros + $"grupocontable.{valorFinal} as col{i + 1}, ";
                                Agrupa = Agrupa + $"grupocontable.{valorFinal}, ";
                                if (sqlItem.Contains("grupocontable.coditem=itemtrabajador.coditem") == false)
                                    sqlItem = sqlItem + $"INNER JOIN grupocontable ON grupocontable.coditem=itemtrabajador.coditem\n";
                            }
                            else
                            {
                                //COLUMNAS EXISTE EN ITEM
                                ColumnasTabla = rel.GetColumnsTable("item");
                                if (ColumnasTabla.Count > 0)
                                {
                                    //EXISTE COLUMNA EN TABLA GRUPODETALLE
                                    if (rel.ExisteColumna(valorFinal, ColumnasTabla))
                                    {
                                        SqlParametros = SqlParametros + $"item.{valorFinal} as col{i + 1}, ";
                                        Agrupa = Agrupa + $"item.{valorFinal}, ";
                                        if (sqlItem.Contains("item.coditem=itemtrabajador.coditem") == false)
                                            sqlItem = sqlItem + $"INNER JOIN item ON item.coditem=itemtrabajador.coditem\n";
                                    }
                                }
                            }
                        }

                    }
                    //SE SELECCIONÓ DATO ENTIDAD???
                    else if (Configuraciones[i].TipoSeleccionado == 4)
                    {
                        //CORRESPONDE A TABLA A LINKEAR
                        SubDato = Configuraciones[i].SubtipoSeleccionado + "";
                        valorFinal = Configuraciones[i].ValorSeleccionado;

                        if (SubDato == "1")
                        {
                            if (sqlEntidad.Contains("isapre.id=trabajador.salud") == false)
                                sqlEntidad = sqlEntidad + $"INNER JOIN isapre ON isapre.id=trabajador.salud \n";
                            SqlParametros = SqlParametros + $"isapre.{valorFinal} as col{i + 1}, ";
                            Agrupa = Agrupa + $"isapre.{valorFinal}, ";
                        }
                        else if (SubDato == "2")
                        {
                            if (sqlEntidad.Contains("afp.id=trabajador.afp") == false)
                                sqlEntidad = sqlEntidad + $"INNER JOIN afp ON afp.id=trabajador.afp \n";
                            SqlParametros = SqlParametros + $"afp.{valorFinal} as col{i + 1}, ";
                            Agrupa = Agrupa + $"afp.{valorFinal}, ";
                        }
                        else if (SubDato == "3")
                        {
                            //sqlEntidad = sqlEntidad + $"INNER JOIN {SubDato.ToLower()} ON mutual.id=trabajador.afp \n";
                            if (SqlParametros.Contains("empresa.nombremut=mutual.id") == false)
                                SqlParametros = SqlParametros + $"(SELECT mutual.{valorFinal} FROM mutual INNER JOIN empresa ON empresa.nombremut=mutual.id) as col{i + 1},";
                        }
                        else if (SubDato == "4")
                        {
                            //...
                            if (SqlParametros.Contains("empresa.nombreccaf=cajacompensacion.id") == false)
                                SqlParametros = SqlParametros + $"(SELECT cajacompensacion.{valorFinal} from cajaCompensacion INNER JOIN empresa on empresa.nombreccaf=cajacompensacion.id) as col{i + 1},";
                        }
                    }

                    //SI ES EL ULTIMO ELEMENTO DE LA LISTA???
                    if (i == Configuraciones.Count - 1)
                    {
                        //GUARDAMOS TODAS LAAS RELACIONES ENCONTRADAS 
                        da = sqlEmpleado + sqlEntidad + sqlItem;

                        //DEBITO
                        if (Configuraciones[i - 1].Tipo == 1)
                            SqlParametros = SqlParametros + "SUM(valorcalculado) as col7, 0 as col8";
                        if (Configuraciones[i - 1].Tipo == 2)
                            SqlParametros = SqlParametros + "0 as col7, SUM(valorcalculado) as col8";

                        //if (SqlParametros.Length > 0)
                        //    SqlParametros = SqlParametros.Substring(0, SqlParametros.Length - 2);

                        //REPLACE SQLBASE
                        SqlBase = SqlBase.Replace("{campos}", SqlParametros);
                        SqlBase = SqlBase.Replace("{linktablas}", da);
                        Footer = Footer.Replace("{item}", $"'{Configuraciones[i].ItemObservado}'");
                        Footer = Footer.Replace("{tipocontable}", $"{Configuraciones[i - 1].Tipo}");
                        Footer = Footer.Replace("{periodo}", pPeriodo.ToString());
                        if(Agrupa.Length > 0)
                            SqlAgrupa = SqlAgrupa.Replace("{agrupa}", Agrupa.Substring(0, Agrupa.Length - 2));
                        SqlBase = SqlBase + Footer + SqlAgrupa;
                        //SqlBase = SqlBase + Footer;

                        ///Guardamos sql.
                        GruposSql.Add(SqlBase);
                    }
                }

                int gr = GruposSql.Count;

                if (GruposSql.Count > 0)
                {

                    foreach (var item in GruposSql)
                    {
                        concatena = concatena + item;
                    }

                }
            }

            return concatena;
        }

        /// <summary>
        /// Genera un datatable con informacion de la consulta.
        /// </summary>
        /// <param name="pSql"></param>
        /// <returns></returns>
        private DataTable GetDataSource(string pSql)
        {
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataAdapter ad;
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();

            if (pSql.Length > 0)
            {
                try
                {
                    cn = fnSistema.OpenConnection();
                    if (cn != null)
                    {
                        using (cn)
                        {
                            using (cmd = new SqlCommand(pSql, cn))
                            {
                                ad = new SqlDataAdapter();
                                ad.SelectCommand = cmd;
                                ad.Fill(ds, "data");

                                if (ds.Tables[0].Rows.Count > 0)
                                {
                                    dt = ds.Tables[0];
                                }

                                cmd.Dispose();
                                ad.Dispose();
                                ds.Dispose();
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                  //ERROR...
                }
            }

            return dt;
        }

        /// <summary>
        /// Genera archivo plano.
        /// </summary>
        /// <param name="pTabla"></param>
        /// <param name="pPathFile"></param>
        /// <param name="pPeriodo"></param>
        private void GenerarArchivo(DataTable pTabla, string pPathFile, int pPeriodo, string pDelimitador, int pTipoArchivo)
        {
            int MaxCol1 = 0, MaxCol2 = 0, MaxCol3 = 0, MaxCol4 = 0, MaxCol5 = 0, MaxCol6 = 0, MaxCol7 = 0, 
                MaxCol8 = 0, MaxCol9 = 0, MaxCol10 = 0, MaxCol11 = 0, MaxCol12 = 0, MaxCol13 = 0, MaxCol14 = 0, 
                MaxCol15 = 0, MaxCol16 = 0, MaxCol17 = 0, MaxCol18 = 0, MaxCol19 = 0, MaxCol20 = 0, MaxCol21 = 0, 
                MaxCol22 = 0, MaxCol23 = 0, MaxCol24 = 0, MaxCol25 = 0, MaxCol26 = 0;
            
            if (pTabla.Rows.Count > 0)
            {
                try
                {
                    //FORMATO
                    //1 -> ARCHIVO TEXTO
                    //2 -> ARCHIVO EXCEL                    

                    DataTable Plantilla = GetFormatoSalida();

                    Empresa emp = new Empresa();
                    emp.SetInfo();

                    Archivo.CrearArchivo(pPathFile);
                    string Line = "";
                    string preLine = "";
                    string NombreComprobante = "";
                    DateTime UltimoDia = DateTime.Now.Date;
                    UltimoDia = fnSistema.UltimoDiaMes(pPeriodo);

                    StringBuilder builder = new StringBuilder();

                    //NombreComprobante = $"Remuneraciones {UltimoDia.ToString("MM/yyyy")}\t ";
                    //preLine = $"T\t{UltimoDia.ToShortDateString()}\t{NombreComprobante}";                    

                    //string elemento = pTabla.OrderByDescending(s => s.Length).FirstOrDefault();
                    SetPeriodos(pTabla, pPeriodo);

                    #region "COLUMNAS COUNT"
                    MaxCol1 = GetMaxColumnLength("col1", pTabla);
                    if (pTabla.Rows.Count >= 2)
                        MaxCol2 = GetMaxColumnLength("col2", pTabla);
                    if (pTabla.Rows.Count >= 3)
                        MaxCol3 = GetMaxColumnLength("col3", pTabla);
                    if (pTabla.Rows.Count >= 4)
                        MaxCol4 = GetMaxColumnLength("col4", pTabla);
                    if (pTabla.Rows.Count >= 5)
                        MaxCol5 = GetMaxColumnLength("col5", pTabla);
                    if (pTabla.Rows.Count >= 6)
                        MaxCol6 = GetMaxColumnLength("col6", pTabla);
                    if (pTabla.Rows.Count >= 7)
                        MaxCol7 = GetMaxColumnLength("col7", pTabla);
                    if (pTabla.Rows.Count >= 8)
                        MaxCol8 = GetMaxColumnLength("col8", pTabla);
                    if (pTabla.Rows.Count >= 9)
                        MaxCol9 = GetMaxColumnLength("col9", pTabla);
                    if (pTabla.Rows.Count >= 10)
                        MaxCol10 = GetMaxColumnLength("col10", pTabla);
                    if (pTabla.Rows.Count >= 11)
                        MaxCol11 = GetMaxColumnLength("col11", pTabla);
                    if (pTabla.Rows.Count >= 12)
                        MaxCol12 = GetMaxColumnLength("col12", pTabla);
                    if (pTabla.Rows.Count >= 13)
                        MaxCol13 = GetMaxColumnLength("col13", pTabla);
                    if (pTabla.Rows.Count >= 14)
                        MaxCol14 = GetMaxColumnLength("col14", pTabla);
                    if (pTabla.Rows.Count >= 15)
                        MaxCol15 = GetMaxColumnLength("col15", pTabla);
                    if (pTabla.Rows.Count >= 16)
                        MaxCol16 = GetMaxColumnLength("col16", pTabla);
                    if (pTabla.Rows.Count >= 17)
                        MaxCol17 = GetMaxColumnLength("col17", pTabla);
                    if (pTabla.Rows.Count >= 18)
                        MaxCol18 = GetMaxColumnLength("col18", pTabla);
                    if (pTabla.Rows.Count >= 19)
                        MaxCol19 = GetMaxColumnLength("col19", pTabla);
                    if (pTabla.Rows.Count >= 20)
                        MaxCol20 = GetMaxColumnLength("col20", pTabla);
                    if (pTabla.Rows.Count >= 21)
                        MaxCol21 = GetMaxColumnLength("col20", pTabla);
                    if (pTabla.Rows.Count >= 22)
                        MaxCol22 = GetMaxColumnLength("col20", pTabla);
                    if (pTabla.Rows.Count >= 23)
                        MaxCol23 = GetMaxColumnLength("col20", pTabla);
                    if (pTabla.Rows.Count >= 24)
                        MaxCol24 = GetMaxColumnLength("col20", pTabla);
                    if (pTabla.Rows.Count >= 25)
                        MaxCol25 = GetMaxColumnLength("col20", pTabla);
                    if (pTabla.Rows.Count >= 26)
                        MaxCol26 = GetMaxColumnLength("col20", pTabla);
                    #endregion

                    //MaxCol1 = pTabla.AsEnumerable().SelectMany(row => row.ItemArray.OfType<string>()).Max(str => str.Length);

                    //PARA ORDENAR UNA DATATABLE
                    //pTabla.DefaultView.Sort = "COLUMNAME ASC|DESC";

                    //DEFINIMOS UNA MATRIZ
                    //FILA, COLUMNAS
                    string[,] mt = new string[pTabla.Rows.Count, 26];

                    DataRowCollection coleccion = pTabla.Rows;                    

                    string LlenaFila = "";
                    string dd = "";

                    foreach (DataRow row in pTabla.Rows)
                    {
                        LlenaFila = "";
                        for (int i = 0; i < pTabla.Columns.Count; i++)
                        {
                            string cell = row[i].ToString();
                            if (fnSistema.IsDecimal(cell))
                                if (cell.Contains(","))
                                    cell = Convert.ToDouble(cell).ToString();                           
                           
                            if (i == 0)
                                LlenaFila = LlenaFila + AgregaEspacios(cell, MaxCol1) + pDelimitador;
                            else if(i == 1)
                                LlenaFila = LlenaFila + AgregaEspacios(cell, MaxCol2) + pDelimitador;
                            else if (i == 2)
                                LlenaFila = LlenaFila + AgregaEspacios(cell, MaxCol3) + pDelimitador;
                            else if (i == 3)
                                LlenaFila = LlenaFila + AgregaEspacios(cell, MaxCol4) + pDelimitador;
                            else if (i == 4)
                                LlenaFila = LlenaFila + AgregaEspacios(cell, MaxCol5) + pDelimitador;
                            else if (i == 5)
                                LlenaFila = LlenaFila + AgregaEspacios(cell, MaxCol6) + pDelimitador;
                            else if (i == 6)
                                LlenaFila = LlenaFila + AgregaEspacios(cell, MaxCol7) + pDelimitador;
                            else if (i == 7)
                                LlenaFila = LlenaFila + AgregaEspacios(cell, MaxCol8) + pDelimitador;
                            else if(i == 8)
                                LlenaFila = LlenaFila + AgregaEspacios(cell, MaxCol9) + pDelimitador;
                            else if (i == 9)
                                LlenaFila = LlenaFila + AgregaEspacios(cell, MaxCol10) + pDelimitador;
                            else if (i == 10)
                                LlenaFila = LlenaFila + AgregaEspacios(cell, MaxCol11) + pDelimitador + " " + "\t";
                            else if (i == 11)
                                LlenaFila = LlenaFila + AgregaEspacios(cell, MaxCol12) + pDelimitador;
                            else if (i == 12)
                                LlenaFila = LlenaFila + AgregaEspacios(cell, MaxCol13) + pDelimitador;
                            else if (i == 13)
                                LlenaFila = LlenaFila + AgregaEspacios(cell, MaxCol14) + pDelimitador;
                            else if (i == 14)
                                LlenaFila = LlenaFila + AgregaEspacios(cell, MaxCol15) + pDelimitador;
                            else if (i == 15)
                                LlenaFila = LlenaFila + AgregaEspacios(cell, MaxCol16) + pDelimitador;
                            else if (i == 16)
                                LlenaFila = LlenaFila + AgregaEspacios(cell, MaxCol17) + pDelimitador;
                            else if (i == 17)
                                LlenaFila = LlenaFila + AgregaEspacios(cell, MaxCol18) + pDelimitador;
                            else if (i == 18)
                                LlenaFila = LlenaFila + AgregaEspacios(cell, MaxCol19) + pDelimitador;
                            else if (i == 19)
                                LlenaFila = LlenaFila + AgregaEspacios(cell, MaxCol20) + pDelimitador;
                            else if (i == 20)
                                LlenaFila = LlenaFila + AgregaEspacios(cell, MaxCol21) + pDelimitador;
                            else if (i == 21)
                                LlenaFila = LlenaFila + AgregaEspacios(cell, MaxCol22) + pDelimitador;
                            else if (i == 22)
                                LlenaFila = LlenaFila + AgregaEspacios(cell, MaxCol23) + pDelimitador;
                            else if (i == 23)
                                LlenaFila = LlenaFila + AgregaEspacios(cell, MaxCol24) + pDelimitador;
                            else if (i == 24)
                                LlenaFila = LlenaFila + AgregaEspacios(cell, MaxCol25) + pDelimitador;
                            else if (i == 25)
                                LlenaFila = LlenaFila + AgregaEspacios(cell, MaxCol26) + pDelimitador;

                        }

                        //Agregamos fila a archivo
                        Archivo.EscribirArchivo(pPathFile, LlenaFila);
                    }

                    //FILAS
                    //for (int i = 0; i < pTabla.Rows.Count; i++)
                    //{
                    //    DataRow Fila = pTabla.Rows[i];                   
                   
                    //    LlenaFila = "";
                    //    //COLUMNAS
                    //    for (int j = 0; j < mt.GetLength(1); j++)
                    //    {
                    //        //T
                    //        if (j == 0)
                    //            mt[i, j] = "T";
                    //        //FECHA
                    //        else if (j == 1)
                    //            mt[i, j] = UltimoDia.ToShortDateString();
                    //        //TITULO
                    //        else if (j == 2)
                    //            mt[i, j] = NombreComprobante;
                    //        //CUENTA CONTABLE
                    //        else if (j == 3)
                    //            mt[i, j] = AgregaEspacios(Fila["col1"].ToString(), MaxCol1);
                    //        //CENTRO COSTO
                    //        else if (j == 8)
                    //            mt[i, j] = AgregaEspacios(Fila["col2"].ToString(), MaxCol2);
                    //        //RUT ENTIDAD
                    //        else if (j == 12)
                    //            mt[i, j] = AgregaEspacios(Fila["col3"].ToString(), MaxCol3);
                    //        //DESCRIPCION
                    //        else if (j == 4)
                    //            mt[i, j] = AgregaEspacios(Fila["col4"].ToString(), MaxCol4);
                    //        //NOMBRE ENTIDAD
                    //        else if (j == 13)
                    //            mt[i, j] = AgregaEspacios(Fila["col5"].ToString(), MaxCol5);
                    //        //ALIAS
                    //        else if (j == 9)
                    //            mt[i, j] = AgregaEspacios(Fila["col6"].ToString(), MaxCol6);
                    //        //DEBITO
                    //        else if (j == 5)
                    //            mt[i, j] = AgregaEspacios(Convert.ToDouble(Fila["col7"]) + "", MaxCol5);
                    //        //CREDITO
                    //        else if (j == 6)
                    //            mt[i, j] = AgregaEspacios(Convert.ToDouble(Fila["col8"]) + "", MaxCol5);
                    //        //mm-yyyy
                    //        else if (j == 10)
                    //            mt[i, j] = fnSistema.FechaPeriodo(pPeriodo).ToString("MMyyyy");
                    //        //FECHA ULTIMO DIA MES
                    //        else if (j == 11)
                    //            mt[i, j] = UltimoDia.ToShortDateString();
                    //        else if (j == 14)
                    //            //CODIGO EMPRESA
                    //            mt[i, j] = "3";
                    //        else
                    //            mt[i, j] = "0";

                    //        string cell = mt[i, j];
                    //        //CREAMOS CADENA EN BASE A CELDAS DE CADA FILA DE LA MATRIZ.
                    //        LlenaFila = LlenaFila + cell + pDelimitador;
                    //    }

                    //    //AGREGAR LINEA AL ARCHIVO
                    //    if (pTipoArchivo == 1)
                    //    {
                    //        if (File.Exists(pPathFile))
                    //            Archivo.EscribirArchivo(pPathFile, LlenaFila.ToString());
                    //    }                        
                    //}

                    if(pTipoArchivo == 2)
                        FileExcel.CrearArchivoExcelDev(pTabla,pPathFile);
                    
                    DialogResult adv = XtraMessageBox.Show("Archivo generado correctamente.¿Deseas abrir archivo?", "Archivo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (adv == DialogResult.Yes)
                        System.Diagnostics.Process.Start(pPathFile);

                }
                catch (Exception ex)
                {
                    //ERROR...
                    //XtraMessageBox.Show(ex.Message);
                }
            }
        }

        private void GeneraReporte(DataTable pTabla, int pPeriodo, bool editar = false)
        {
            if (pTabla.Rows.Count > 0 || editar)
            {
                //rptContable reporte = new rptContable(pTabla);
                //reporte.DataSource = pTabla;
                //reporte.DataMember = "data";

                List<RContable> List = new List<RContable>();
                List = GetListDataSource(pTabla);

                //RptContable2 reporte = new RptContable2();
                //Reporte externo
                ReportesExternos.rptContable2 reporte =   new ReportesExternos.rptContable2();
                reporte.LoadLayoutFromXml(Path.Combine(fnSistema.RutaCarpetaReportesExterno, "rptContable2.repx"));

                reporte.DataSource = List;
                //reporte.DataMember = "data";

                foreach (var item in reporte.Parameters)
                {
                    item.Visible = false;
                }

                Empresa emp = new Empresa();
                emp.SetInfo();

                reporte.Parameters["periodo"].Value = fnSistema.PrimerMayuscula(fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(pPeriodo)));
                reporte.Parameters["empresa"].Value = emp.Razon;
                reporte.Parameters["condicion"].Value = CondicionBusqueda;

                Documento doc = new Documento("", 0);
                //reporte.SaveLayoutToXml(Path.Combine(fnSistema.RutaCarpetaReportesExterno, "rptContable2.repx"));

                if (editar)
                {
                    splashScreenManager1.ShowWaitForm();
                    //Se le pasa el waitform para que se cierre una vez cargado
                    DiseñadorReportes.MostrarEditorLimitado(reporte, "rptContable2.repx", splashScreenManager1);
                }
                else 
                {
                    doc.ShowDocument(reporte);
                }
                
            }
        }

        private List<RContable> GetListDataSource(DataTable pTabla)
        {
            List<RContable> Listado = new List<RContable>();
            ReporteContable rep = new ReporteContable();
            rep.SetInfo();
            if (pTabla.Rows.Count > 0)
            {
                foreach (DataRow row in pTabla.Rows)
                {
                    RContable b = new RContable();

                    foreach (DataColumn col in pTabla.Columns)
                    {                     
                        if (col.ColumnName.ToLower().Equals("col" + rep.c1))
                        {
                            b.Col1 = row[col].ToString();
                        }
                        else if (col.ColumnName.ToLower().Equals("col" + rep.c2))
                        {
                            b.Col2 = row[col].ToString();
                        }
                        else if (col.ColumnName.ToLower().Equals("col" + rep.c3))
                        {
                            b.Col3 = row[col].ToString();
                        }
                        else if (col.ColumnName.ToLower().Equals("col" + rep.c4))
                        {
                            b.Col4 = row[col].ToString();
                        }
                        else if (col.ColumnName.ToLower().Equals("col" + rep.c5))
                        {
                            b.Col5 = row[col].ToString();
                        }
                        else if (col.ColumnName.ToLower().Equals("col" + rep.c6))
                        {
                            b.Col6 = row[col].ToString();
                        }
                        else if (col.ColumnName.ToLower().Equals("col" + rep.c7))
                        {
                            b.Col7 = row[col].ToString();
                        }
                        else if (col.ColumnName.ToLower().Equals("col" + rep.c8))
                        {
                            b.Col8 = row[col].ToString();
                        }
                      
                    }

                    Listado.Add(b);
                }
            }

            return Listado;
        }

        /// <summary>
        /// Setea las celdas en las que se quiere colocar el periodo que se está evaluando.
        /// </summary>
        /// <param name="pData"></param>
        /// <param name="pPeriodo"></param>
        private void SetPeriodos(DataTable pData, int pPeriodo)
        {             
            if (pData.Rows.Count > 0)
            {
                foreach (DataRow row in pData.Rows)
                {
                    foreach (DataColumn col in pData.Columns)
                    {
                        if (row[col].ToString().Contains("{") && row[col].ToString().Contains("}"))
                        {
                            //Cambiamos valor correspondiente                         
                            row[col] = GetPeriodoFormat(pPeriodo, row[col].ToString());
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Extrae parametro desde celda y reeemplaza por el valor correspondiente
        /// <para>Solo se utiliza para celda donde se quiera mostrar el periodo observado</para>
        /// </summary>
        /// <param name="pPeriodo"></param>
        /// <param name="pCellValue"></param>
        /// <returns></returns>
        private string GetPeriodoFormat(int pPeriodo, string pCellValue)
        {
            /*
             * Parametros válidos
             * {periodo} = Corresponde al periodo que se está evaluando, por lo tanto, se reemplaza por este valor
             * {d} = dia del periodo (depende si es el primer dia del mes o el ultimo)
             * {m} = mes del periodo (depende si es el primer dia del mes o el ultimo)
             * {y} = año del periodo 
             * {start} = Significa que queremos mostrar el primer dia del periodo
             * {end} = Significa que queremos mostrar el ultimo dia del periodo
             * Ej: {periodo}{end} = 31-01-2019
             * Ej: {periodo}{start} = 01-01-2019
             * Ej: {periodo}{end}{d}-{m}-{y} = Muestra en formato dia mes año el ultimo dia del mes del periodo evaluado.
             */

            DateTime UltimoDiasMes = DateTime.Now.Date;
            DateTime PrimerDiaMes = DateTime.Now.Date;
            UltimoDiasMes = fnSistema.UltimoDiaMes(pPeriodo);
            PrimerDiaMes = fnSistema.PrimerDiaMes(pPeriodo);

            string value = "";
            string par = "";
            par = GetParameterCell(pCellValue);

            switch (par)
            {
                case "[{periodo}]":                    
                    value = pPeriodo.ToString();
                    pCellValue = pCellValue.Replace("[{periodo}]", value);
                    break;
                case "[{periodo}{start}]":
                    value = PrimerDiaMes.ToShortDateString();
                    pCellValue = pCellValue.Replace("[{periodo}{start}]", value);
                    break;
                case "[{periodo}{end}]":
                    value = UltimoDiasMes.ToShortDateString();
                    pCellValue = pCellValue.Replace("[{periodo}{end}]", value);
                    break;

                #region "PERIODO - PRIMER DIA DEL MES"
                case "[{periodo}{start}{d}-{m}-{y}]":
                    value = PrimerDiaMes.ToString("dd-MM-yyyy");
                    pCellValue = pCellValue.Replace("[{periodo}{start}{d}-{m}-{y}]", value);
                    break;
                case "[{periodo}{start}{y}-{m}-{d}]":
                    value = PrimerDiaMes.ToString("yyyy-MM-dd");
                    pCellValue = pCellValue.Replace("[{periodo}{start}{y}-{m}-{d}]", value);
                    break;
                case "[{periodo}{start}{y}/{m}/{d}]":
                    value = PrimerDiaMes.ToString("yyyy/MM/dd");
                    pCellValue = pCellValue.Replace("[{periodo}{start}{y}/{m}/{d}]", value);
                    break;
                case "[{periodo}{start}{d}/{m}/{y}]":
                    value = PrimerDiaMes.ToString("dd/MM/yyyy");
                    pCellValue = pCellValue.Replace("[{periodo}{start}{d}/{m}/{y}]", value);
                    break;

                case "[{periodo}{start}{m}/{y}]":
                    value = PrimerDiaMes.ToString("MM/yyyy");
                    pCellValue = pCellValue.Replace("[{periodo}{start}{m}/{y}]", value);
                    break;
                case "[{periodo}{start}{m}-{y}]":
                    value = PrimerDiaMes.ToString("MM-yyyy");
                    pCellValue = pCellValue.Replace("[{periodo}{start}{m}-{y}]", value);
                    break;

                #endregion

                #region "PERIODO - ULTIMO DIA DEL MES"
                case "[{periodo}{end}{d}-{m}-{y}]":
                    value = UltimoDiasMes.ToString("dd-MM-yyyy");
                    pCellValue = pCellValue.Replace("[{periodo}{end}{d}-{m}-{y}]", value);
                    break;

                case "[{periodo}{end}{y}-{m}-{d}]":
                    value = UltimoDiasMes.ToString("yyyy-MM-dd");
                    pCellValue = pCellValue.Replace("[{periodo}{end}{y}-{m}-{d}]", value);
                    break;

                case "[{periodo}{end}{d}/{m}/{y}]":
                    value = UltimoDiasMes.ToString("dd/MM/yyyy");
                    pCellValue = pCellValue.Replace("[{periodo}{end}{d}/{m}/{y}]", value);
                    break;

                case "[{periodo}{end}{y}/{m}/{d}]":
                    value = UltimoDiasMes.ToString("yyyy/MM/dd");
                    pCellValue = pCellValue.Replace("[{periodo}{end}{y}/{m}/{d}]", value);
                    break;

                case "[{periodo}{end}{m}/{y}]":
                    value = UltimoDiasMes.ToString("MM/yyyy");
                    pCellValue = pCellValue.Replace("[{periodo}{end}{m}/{y}]", value);
                    break;

                case "[{periodo}{end}{m}-{y}]":
                    value = UltimoDiasMes.ToString("MM-yyyy");
                    pCellValue = pCellValue.Replace("[{periodo}{end}{m}-{y}]", value);
                    break;
                #endregion


                case "[{periodo}{d}-{m}-{y}]":
                    value = UltimoDiasMes.ToString("dd-MM-yyyy");
                    pCellValue = pCellValue.Replace("[{periodo}{d}-{m}-{y}]", value);
                    break;
                case "[{periodo}{d}{m}{y}]":
                    value = UltimoDiasMes.ToString("ddMMyyyy");
                    pCellValue = pCellValue.Replace("[{periodo}{d}{m}{y}]", value);
                    break;
                case "[{periodo}{y}{m}{d}]":
                    value = UltimoDiasMes.ToString("yyyyMMdd");
                    pCellValue = pCellValue.Replace("[{periodo}{y}{m}{d}]", value);
                    break;
                case "[{periodo}{d}/{m}/{y}]":
                    value = UltimoDiasMes.ToString("dd/MM/yyyy");
                    pCellValue = pCellValue.Replace("[{periodo}{d}/{m}/{y}]", value);
                    break;
                case "[{periodo}{y}-{m}-{d}]":
                    value = UltimoDiasMes.ToString("yyyy-MM-dd");
                    pCellValue = pCellValue.Replace("[{periodo}{y}-{m}-{d}]", value);
                    break;
                case "[{periodo}{y}/{m}/{d}]":
                    value = UltimoDiasMes.ToString("yyyy/MM/dd");
                    pCellValue = pCellValue.Replace("[{periodo}{y}/{m}/{d}]", value);
                    break;
                case "[{periodo}{y}-{m}]":
                    value = UltimoDiasMes.ToString("yyyy-MM");
                    pCellValue = pCellValue.Replace("[{periodo}{y}-{m}]", value);
                    break;
                case "[{periodo}{y}/{m}]":
                    value = UltimoDiasMes.ToString("yyyy/MM");
                    pCellValue = pCellValue.Replace("[{periodo}{y}/{m}]", value);
                    break;
                case "[{periodo}{m}-{y}]":
                    value = UltimoDiasMes.ToString("MM-yyyy");
                    pCellValue = pCellValue.Replace("[{periodo}{m}-{y}]", value);
                    break;
                case "[{periodo}{m}/{y}]":
                    value = UltimoDiasMes.ToString("MM/yyyy");
                    pCellValue = pCellValue.Replace("[{periodo}{m}/{y}]", value);
                    break;
                case "[{periodo}{m}{y}]":
                    value = UltimoDiasMes.ToString("MMyyyy");
                    pCellValue = pCellValue.Replace("[{periodo}{m}{y}]", value);
                    break;
                case "[{periodo}{y}{m}]":
                    value = UltimoDiasMes.ToString("yyyyMM");
                    pCellValue = pCellValue.Replace("[{periodo}{y}{m}]", value);
                    break;

                default:
                    break;
            }


            return pCellValue;

        }

        /// <summary>
        /// Extrae seccion de parametro si es que existe
        /// </summary>
        /// <returns></returns>
        private string GetParameterCell(string pCellValue)
        {
            string parameter = "";
            int posinLlave = 0, posTerLlave = 0;
            try
            {
                if (pCellValue.Length > 0)
                {
                    if (pCellValue.Contains("[") && pCellValue.Contains("]"))
                    {
                        posinLlave = pCellValue.IndexOf("[");
                        posTerLlave = pCellValue.IndexOf("]");

                        //Extrae subcadena
                        parameter = pCellValue.Substring(posinLlave, (posTerLlave - posinLlave)+1);
                    }
                }
            }
            catch (Exception ex)
            {
                //ERROR...
            }

            return parameter;
        }

        /// <summary>
        /// Obtiene el orden de salida del esquema contable
        /// </summary>
        private DataTable GetFormatoSalida()
        {
            DataTable Plantilla = new DataTable();

            //Agregamos columnas a la tabla
            for (int i = 1; i <=26; i++)
            {
                Plantilla.Columns.Add(new DataColumn() { ColumnName = "col" + i, DataType = typeof(string) });
            }

            return Plantilla;
            
        }

        private string AgregaEspacios(string pCadena, int pMax)
        {
            if (pCadena.Length >= 0)
            {
                int dif = 0;
                //string elemento = pCadena.OrderByDescending(s => s.Length).FirstOrDefault();
                //Max = elemento.Length;

                if (pCadena.Length < pMax)
                {
                    dif = pMax - pCadena.Length;
                    pCadena = pCadena + new string(' ', dif);
                }        
             
            }

            return pCadena;
        }

        private int GetMaxColumnLength(string pColumnName, DataTable pTabla)
        {
            int n = 0;
            try
            {
                if (pColumnName.Length > 0 && pTabla.Rows.Count > 0)
                {
                    //CADENA CON MAYOR TAMAÑO POR COLUMNA
                    List<int> Len = Enumerable.Range(0, pTabla.Columns.Count).Select(col => pTabla.AsEnumerable().Select(row => row[pColumnName]).OfType<string>().Max(val => val.Length)).ToList();
                    if (Len.Count > 0)
                        n = Len[0];
                }
            }
            catch (Exception ex)
            {
                n = 0;
            }

            return n;
            
        }
        #endregion

        private void btnDocumento_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            //PERIODO...
            if (txtPeriodo.Properties.DataSource == null || txtPeriodo.EditValue == null)
            { XtraMessageBox.Show("Por favor ingresa un periodo válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            try
            {
                if (TablaDataSource.Rows.Count > 0)
                {
                    //GENERAMOS REPORTE
                    GeneraReporte(TablaDataSource, Convert.ToInt32(txtPeriodo.EditValue));
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("No se pudo generar documento", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }           
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                txtConjunto.Text = "";
                txtConjunto.Enabled = false;
                btnConjunto.Enabled = false;
            }
            else
            {
                txtConjunto.Text = "";
                txtConjunto.Enabled = true;
                txtConjunto.Focus();
                btnConjunto.Enabled = true;
            }
        }

        private void btnConjunto_Click(object sender, EventArgs e)
        {
            FrmFiltroBusqueda filtro = new FrmFiltroBusqueda(true);
            filtro.opener = this;
            filtro.StartPosition = FormStartPosition.CenterParent;
            filtro.Show();
        }

        private void btnEditarReporte_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            //PERIODO...
            if (txtPeriodo.Properties.DataSource == null || txtPeriodo.EditValue == null)
            { XtraMessageBox.Show("Por favor ingresa un periodo válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            try
            {
                //if (TablaDataSource.Rows.Count > 0)
                //{
                //    //GENERAMOS REPORTE
                GeneraReporte(TablaDataSource, Convert.ToInt32(txtPeriodo.EditValue), editar: true);
                //}
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("No se pudo generar documento", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }           
        }
    }
}