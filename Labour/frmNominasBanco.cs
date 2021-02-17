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
using System.Data.SqlClient;
using System.Diagnostics;
//DLL PROPIA
//using Nominas;
using BancoItau;
using System.Globalization;
using System.Collections;
using DevExpress.XtraReports.UI;

namespace Labour
{
    public partial class frmNominasBanco : DevExpress.XtraEditors.XtraForm, IConjuntosCondicionales
    {
        //PARA SABER SI EL USUARIO PUEDE VER FICHAS PRIVADAS
        public bool ShowPrivados { get; set; } = User.ShowPrivadas();
        //PARA SABER SI EL USUARIO TIENE ALGUN FILTRO DE VISUALIZACION
        public string Filtro { get; set; } = User.GetUserFilter();
        public IConjuntosCondicionales opener { get; set; }
        //Representa el nombre del banco seleccionado.
        public string NombreBanco { get; set; }

        #region "INTERFAZ"
        public void CargarCodigoConjunto(string code)
        {
            //txtConjunto.Text = code;
            cbxSeleccionConjunto.Text = code;
        }
        #endregion     

        public frmNominasBanco()
        {
            InitializeComponent();
        }

        #region "DATA"

        private void fnComboItem(string pSql, LookUpEdit pCombo, string pCampoKey, string pCampoDesc, bool? pOcultarKey = false, bool? pMostrarValorKeyAlSeleccionar = false)
        {
            List<Combos> lista = new List<Combos>();
            SqlCommand cmd;
            SqlDataReader rd;

            if (fnSistema.ConectarSQLServer())
            {
                using (cmd = new SqlCommand(pSql, fnSistema.sqlConn))
                {
                    rd = cmd.ExecuteReader();
                    if (rd.HasRows)
                    {
                        while (rd.Read())
                        {
                            //AGREGAMOS VALORES A LA LISTA
                            lista.Add(new Combos() { key = (string)rd[pCampoKey], desc = (string)rd[pCampoDesc] });
                        }
                    }
                }
                //LIBERAR RECURSOS
                cmd.Dispose();
                rd.Close();
                fnSistema.sqlConn.Close();
            }

            
            if (pMostrarValorKeyAlSeleccionar == true)
            {
                pCombo.Properties.DataSource = lista.ToList();
                pCombo.Properties.ValueMember = "key";
                pCombo.Properties.DisplayMember = "key";

            }
            else
            {
                //AGREGAMOS TOTAL PAGO
                lista.Add(new Combos() { key = "TPAGO", desc = "TOTAL PAGO" });
                pCombo.Properties.DataSource = lista.ToList();
                pCombo.Properties.ValueMember = "key";
                pCombo.Properties.DisplayMember = "desc";
            }

            pCombo.Properties.PopulateColumns();
            //SI OCULTAR KEY ES TRUE NO SE DEBE MOSTRAR LA COLUMNA KEY
            if (pOcultarKey == true)
            {
                pCombo.Properties.Columns[1].Visible = false;
            }
            pCombo.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
            pCombo.Properties.AutoSearchColumnIndex = 1;
            pCombo.Properties.ShowHeader = false;
        }
        private static void fnComboBanco(LookUpEdit pComboBox)
        {
            string query = "SELECT id, nombre FROM banco order by nombre";
            SqlCommand cmd;
            SqlConnection cn;
            var datatable = new DataTable();
            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        cmd = new SqlCommand(query, cn);
                        var datareader = cmd.ExecuteReader();
                        datatable.Load(datareader);
                    }
                }
            }
            catch (SqlException ex)
            {
                //ERROR DE CONEXION
            }

            //PROPIEDADES COMBOBOX
            pComboBox.Properties.DataSource = datatable;
            pComboBox.Properties.ValueMember = "id";
            pComboBox.Properties.DisplayMember = "nombre";

            pComboBox.Properties.PopulateColumns();

            pComboBox.Properties.Columns[0].Visible = false;

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

        //private void fnComboBanco(LookUpEdit pComboBox)
        //{
        //    //BancoItau.ComboOption option = new BancoItau.ComboOption();
        //    //List<BancoItau.ComboOption> Options = new List<BancoItau.ComboOption>();
        //    //Options = option.GetList();

        //    /*CARGAMOS DESDE BASE DE DATOS*/



        //    if (Options.Count > 0)
        //    {
        //        //PROPIEDADES COMBOBOX
        //        pComboBox.Properties.DataSource = Options.ToList();
        //        pComboBox.Properties.ValueMember = "Code";
        //        pComboBox.Properties.DisplayMember = "Entidad";

        //        pComboBox.Properties.PopulateColumns();
        //        //SI OCULTAR KEY ES TRUE NO SE DEBE MOSTRAR LA COLUMNA KEY

        //        pComboBox.Properties.Columns[0].Visible = false;
        //        pComboBox.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
        //        pComboBox.Properties.AutoSearchColumnIndex = 1;
        //        pComboBox.Properties.ShowHeader = false;
        //    }
        //}

        #endregion

        private void frmNominasBanco_Load(object sender, EventArgs e)
        {

            //CARGAR COMBOS
            //fnComboBanco(txtBanco);
            fnComboBanco(txtBanco);
            fnComboItem("SELECT coditem, descripcion FROM item ORDER BY coditem", txtItem, "coditem", "descripcion", true);
            fnComboItem("SELECT codigo, descripcion FROM conjunto ORDER BY codigo", cbxSeleccionConjunto, "codigo", "descripcion", false, true);
            cbActual.Checked = true;
            txtPeriodo.Text = Calculo.PeriodoObservado.ToString();
            cbTodos.Checked = true;
            dtFechaProceso.DateTime = DateTime.Now.Date;
            if (txtItem.Properties.DataSource != null)
                txtItem.ItemIndex = 0;
            if (txtBanco.Properties.DataSource != null)
                txtBanco.ItemIndex = 0;

            //POR DEFECTPO DESHABILITAMOS BOTON REPORTE
            btnReporte.Visible = false;
        }

        private void cbActual_CheckedChanged(object sender, EventArgs e)
        {
            if (cbActual.Checked)
            {
                //CARGAMOS EN CAJA DE TEXTO EL PERIODO ACTUAL
                txtPeriodo.Text = Calculo.PeriodoObservado.ToString();
                txtPeriodo.ReadOnly = true;
            }
            else
            {
                txtPeriodo.Text = "";
                txtPeriodo.ReadOnly = false;
                txtPeriodo.Focus();
            }
        }

        private void cbTodos_CheckedChanged(object sender, EventArgs e)
        {
            if (cbTodos.Checked)
            {
                //txtConjunto.Text = "";
                //txtConjunto.Enabled = false;
                
                cbxSeleccionConjunto.ItemIndex = -1;
                cbxSeleccionConjunto.Enabled = false;

                btnFiltro.Enabled = false;

            }
            else
            {
                //txtConjunto.Enabled = true;
                //txtConjunto.Text = "";
                //txtConjunto.Focus();

                cbxSeleccionConjunto.ItemIndex = -1;
                cbxSeleccionConjunto.Enabled = true;
                cbxSeleccionConjunto.Focus();
                btnFiltro.Enabled = true;
            }
        }

        private void btnRuta_Click(object sender, EventArgs e)
        {

        }

        private void btnGenerar_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            //List<Cuerpo> listado = new List<Cuerpo>();
            

            if (txtPeriodo.Text == "")
            { XtraMessageBox.Show("Periodo ingresado no es válido", "Periodo", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtPeriodo.Focus(); return; }
            if (Calculo.PeriodoValido(Convert.ToInt32(txtPeriodo.Text)) == false)
            { XtraMessageBox.Show("Periodo ingresado no es válido", "Periodo", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtPeriodo.Focus(); return; }

            if (txtItem.Properties.DataSource == null)
            { XtraMessageBox.Show("Item seleccionado no es válido", "Item", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (txtBanco.Properties.DataSource == null)
            { XtraMessageBox.Show("Banco seleccionado no es válido", "Banco", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            //COMIENZO A ENVIAR DATA A BANCO
            seleccionBanco(Convert.ToInt32(txtBanco.EditValue));

        }

        

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnFiltro_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();
            if (cbTodos.Checked == false)
            {
                FrmFiltroBusqueda filtro = new FrmFiltroBusqueda(true);
                filtro.StartPosition = FormStartPosition.CenterParent;
                filtro.opener = this;
                filtro.ShowDialog();
            }
        }

        private void txtPeriodo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Genera condicion dinamica de acuerdo a opciones del usuario.
        /// <para>Puede ser: un filtro, una condicion, fichas confidenciales.</para>
        /// </summary>
        private string GetCondition(string pConjunto)
        {
            string sql = "";
            //USUARIO TIENE ALGUN FILTRO?
            if (Filtro != "0")
            {
                //APLICA BUSQUEDA FILTRADA?
                if (pConjunto != "")
                {
                    //PUEDE VER FICHAS PRIVADAS?
                    if (ShowPrivados)
                        sql = $"AND {Conjunto.GetCondicionFromCode(Filtro)} AND {Conjunto.GetCondicionFromCode(pConjunto)}";
                    else
                        sql = $"AND {Conjunto.GetCondicionFromCode(Filtro)} AND {Conjunto.GetCondicionFromCode(pConjunto)} AND privado=0";
                }
                else
                {
                    //PUEDE VER FICHAS PRIVADAS
                    if (ShowPrivados)
                        sql = $"AND {Conjunto.GetCondicionFromCode(Filtro)}";
                    else
                        sql = $"AND {Conjunto.GetCondicionFromCode(Filtro)} AND privado=0";
                }

            }
            else
            {
                //APLICA BUSQUEDA FILTRADA?
                if (pConjunto != "")
                {
                    //PUEDE VER FICHAS PRIVADAS?
                    if (ShowPrivados)
                        sql = $"AND {Conjunto.GetCondicionFromCode(pConjunto)}";
                    else
                        sql = $"AND {Conjunto.GetCondicionFromCode(pConjunto)} AND privado=0";
                }
                else
                {
                    //PUEDE VER FICHAS PRIVADAS
                    if (!ShowPrivados)
                        sql = $"AND privado=0";
                }
            }
            //RETORNAMOS CADENA FINAL

            if (sql.ToLower().Contains("contrato") || sql.ToLower().Contains("anomes"))
            {
                if (sql.ToLower().Contains("contrato"))
                    sql = sql.ToLower().Replace("contrato", "trabajador.contrato");
                if (sql.ToLower().Contains("anomes"))
                    sql = sql.ToLower().Replace("anomes", "trabajador.anomes");
                if (sql.ToLower().Contains("nombre"))
                    sql = sql.ToLower().Replace("nombre", "trabajador.nombre");
                if (sql.ToLower().Contains("rut"))
                    sql = sql.ToLower().Replace("rut", "trabajador.rut");

            }

            return sql;

        }

        /// <summary>
        /// Retorna sql para reporte banco.
        /// </summary>
        /// <returns></returns>
        private string GetSqlReport(int pOption)
        {
            string Sql = "-- ORDER PAGOS POR CODIGO DE BANCO ASOCIADO AL trabajador \n" +
                         "SELECT trabajador.rut, CONCAT(apepaterno, ' ', apematerno, ' ', trabajador.nombre) as nombre, " +
                         "banco, cuenta, valorCalculado FROM trabajador " +
                         "INNER JOIN banco ON banco.id = trabajador.banco " +
                         "INNER JOIN itemTrabajador " +
                         "ON itemTrabajador.contrato = trabajador.contrato AND itemTrabajador.anomes = trabajador.anomes " +
                         "WHERE trabajador.anomes = @Periodo AND coditem = @Coditem {condicion} " +
                         "ORDER BY banco, apepaterno";

            string SqlPago = "SELECT trabajador.rut, CONCAT(apepaterno, ' ', apematerno, ' ', trabajador.nombre) as nombre, banco, cuenta, pago as ValorCalculado  " +
                             "FROM trabajador " +
                             "INNER JOIN liquidacionHistorico On trabajador.contrato = liquidacionHistorico.contrato AND trabajador.anomes = liquidacionHistorico.anomes " +
                             "INNER JOIN banco ON banco.id = trabajador.banco " +
                             "WHERE trabajador.anomes = @Periodo {condicion} " +
                             "ORDER BY banco, apepaterno";

            string res = "";

            if (pOption == 0)
                res = Sql;
            else
                res = SqlPago;


            return res;
        }

        /// <summary>
        /// Genera reporte nomina.
        /// </summary>
        /// <param name="pSql">Sentencia sql para reporte.</param>
        /// <param name="pCoditem">Codigo item seleccionado.</param>
        /// <param name="pPeriodo">CMes consultado.</param>
        /// <param name="pBank">Nombre banco seleccionado en combobox.</param>
        private XtraReport CreateReport(string pSql, string pCoditem, int pPeriodo, string pBank)
        {
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataAdapter ad = new SqlDataAdapter();
            DataSet ds = new DataSet();
            //rptNominasBanco report = new rptNominasBanco();
            ReportesExternos.rptNominasBanco report = new ReportesExternos.rptNominasBanco();
            //report.LoadLayoutFromXml(Path.Combine(fnSistema.RutaCarpetaReportesExterno, "rptNominasBanco.repx"));

            foreach (var parametro in report.Parameters)
            {
                parametro.Visible = false;
            }

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
                                //PARAMETROS
                                cmd.Parameters.Add(new SqlParameter("@Periodo", pPeriodo));
                                cmd.Parameters.Add(new SqlParameter("@Coditem", pCoditem));

                                ad.SelectCommand = cmd;
                                ad.Fill(ds, "data");

                                ad.Dispose();
                                if (ds.Tables[0].Rows.Count > 0)
                                {
                                    //DATASE REPORT
                                    report.DataSource = ds.Tables[0];
                                    report.DataMember = "data";

                                    
                                    report.Parameters["NombreBanco"].Value = pBank;
                                    report.Parameters["Periodo"].Value = fnSistema.PrimerMayuscula(fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(pPeriodo)));
                                    report.Parameters["Item"].Value = pCoditem;
                                }

                                cmd.Dispose();
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    //ERROR...
                    XtraMessageBox.Show(ex.Message);
                }
            }
            //report.SaveLayoutToXml(Path.Combine(fnSistema.RutaCarpetaReportesExterno, "rptNominasBanco.repx"));
            return report;
        }

        private void btnReporte_Click(object sender, EventArgs e)
        {
            //MOSTRAMOS CURSOR CON ICONO DE ESPERA.
            Cursor.Current = Cursors.AppStarting;

            string condition = "", SqlReporte = "";
            //rptNominasBanco report;
            ReportesExternos.rptNominasBanco report;
            Documento doc = new Documento("", 0);

            if (txtItem.Properties.DataSource == null || txtBanco.Properties.DataSource == null)
                return;

            //GUARDAMOS EL NOMBRE DEL BANCO SELECCIONADO EN COMBOBOX
            NombreBanco = txtBanco.Text;

            //TODOS LOS REGISTROS DEL PERIODO
            if (cbTodos.Checked)
            {
                condition = GetCondition("");

                //OBTENEMOS SQL
                if (txtItem.EditValue.ToString() == "TPAGO")
                    SqlReporte = GetSqlReport(1);
                else
                    SqlReporte = GetSqlReport(0);

                //REEMPLAZAMOS EN SQL LA CONDICION CORRESPONDIENTE
                if (SqlReporte.Length > 0)
                {
                    SqlReporte = SqlReporte.Replace("{condicion}", condition);

                    //GENERAMOS REPORTE
                    report = (ReportesExternos.rptNominasBanco)CreateReport(SqlReporte, txtItem.EditValue.ToString(), Convert.ToInt32(txtPeriodo.Text), NombreBanco);
                    if (report.DataSource != null)
                        doc.ShowDocument(report);
                }
            }
            else
            {
                //if (txtConjunto.Text == "")
                //{ XtraMessageBox.Show("Conjunto ingresado no existe", "Conjunto", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                if (Conjunto.ExisteConjunto(cbxSeleccionConjunto.EditValue.ToString()) == false)
                { XtraMessageBox.Show("Conjunto ingresado no existe", "Conjunto", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                //FITLRO 
                condition = GetCondition(cbxSeleccionConjunto.EditValue.ToString());

                //OBTENEMOS SQL
                if (txtItem.EditValue.ToString() == "TPAGO")
                    SqlReporte = GetSqlReport(1);
                else
                    SqlReporte = GetSqlReport(0);

                //REEMPLAZAMOS EN SQL LA CONDICION CORRESPONDIENTE
                if (SqlReporte.Length > 0)
                {
                    SqlReporte = SqlReporte.Replace("{condicion}", condition);

                    //GENERAMOS REPORTE
                    report = (ReportesExternos.rptNominasBanco)CreateReport(SqlReporte, txtItem.EditValue.ToString(), Convert.ToInt32(txtPeriodo.Text), NombreBanco);
                    if (report.DataSource != null)
                        doc.ShowDocument(report);
                }
            }


            //MOSTRAMOS CURSOR CON ICONO DE ESPERA.
            Cursor.Current = Cursors.Default;
        }

        private void txtBanco_EditValueChanged(object sender, EventArgs e)
        {
            //DESHABILITAR BOTON REPORT
            btnReporte.Enabled = false;
        }

        private void txtItem_EditValueChanged(object sender, EventArgs e)
        {
            //DESHABILITAR BOTON REPORT
            btnReporte.Enabled = false;
        }

        /// <summary>
        /// MÉTODO QUE MANEJA LA SELECCIÓN DE BANCOS
        /// </summary>
        /// <param name="pNumeroBanco"></param>
        private void seleccionBanco(int pNumeroBanco)
        {   /***DLL***/
            Empresa emp = new Empresa();
            Hashtable datosCabecera = new Hashtable();
            SqlCommand cmd;
            SqlConnection cn;
            emp.SetInfo();
            string DinamicQuery = "";
            int sbif = 0;
            string query = "SELECT dato01 FROM banco WHERE id = @id";

            /*VERIFICAR EL VALOR SBIF DESDE LA BASE DE DATOS*/
            cn = fnSistema.OpenConnection();
            if (cn != null)
            {
                using (cn)
                {
                    cmd = new SqlCommand(query, cn);
                    cmd.Parameters.Add(new SqlParameter("@id", pNumeroBanco));
                    sbif = int.Parse(cmd.ExecuteScalar().ToString());
                }
            }
            /*ACÁ SOLO SE LLENAN LOS VALORES DE LA EMPRESA Y EL USUARIO PARA CADA BANCO A LLAMAR*/

            /***BANCO ITAU***/
            if (sbif == 39)
            {
                DataBancoItau DataBanco;
                datosCabecera.Add("rutempresa", emp.Rut);
                datosCabecera.Add("nombreEmpresa", emp.Razon);
                datosCabecera.Add("direccionEmpresa", emp.Direccion);
                datosCabecera.Add("ciudadEmpresa", emp.NombreCiudad);

                if (cbTodos.Checked)
                {
                    /*NUEVA FORMA DE HACERLO*/

                    DataBanco = new DataBancoItau("", true, txtItem.EditValue.ToString(), ShowPrivados, Filtro, GetCondition(Filtro),
                        dtFechaProceso.DateTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), Convert.ToInt32(txtPeriodo.Text), sbif, datosCabecera, fnSistema.pgServer, fnSistema.pgDatabase, fnSistema.pgUser, fnSistema.pgPass);

                    DinamicQuery = GetCondition("");

                    if (DataBanco.Listado.Count == 0)
                    { XtraMessageBox.Show("No se encontró información", "Registros", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
                }
                else
                {
                    //GENERAR DOCUMENTO PARA CONJUNTO SELECCIONADO
                    if (Conjunto.ExisteConjunto(cbxSeleccionConjunto.EditValue.ToString()) == false)
                    { XtraMessageBox.Show("Por favor ingresa un conjunto válido", "Conjunto", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                    string condiTionCode = Conjunto.GetCondicionFromCode(cbxSeleccionConjunto.EditValue.ToString());
                    //string condiTionCode = Conjunto.GetCondicionFromCode(txtConjunto.Text);

                    DataBanco = new DataBancoItau(condiTionCode, false, txtItem.EditValue.ToString(), ShowPrivados, Filtro, GetCondition(Filtro),
                        dtFechaProceso.DateTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), Convert.ToInt32(txtPeriodo.Text), sbif, datosCabecera, fnSistema.pgServer, fnSistema.pgDatabase, fnSistema.pgUser, fnSistema.pgPass);

                    DinamicQuery = GetCondition(cbxSeleccionConjunto.EditValue.ToString());
                    //DinamicQuery = GetCondition(txtConjunto.Text);

                    if (DataBanco.Listado.Count == 0)
                    { XtraMessageBox.Show("No se encontró información", "Registros", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                }
            }
        }

    }
}