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
using DevExpress.XtraReports.Configuration;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using DevExpress.Utils.Menu;
using DevExpress.XtraReports.UI;

namespace Labour
{
    public partial class frmDetalleItem : DevExpress.XtraEditors.XtraForm, IFormItemInformacion, IConjuntosCondicionales
    {
        [DllImport("user32")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32")]
        static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        const int MF_BYCOMMAND = 0;
        const int MF_DISABLED = 2;
        const int SC_CLOSED = 0xF060;

        /// <summary>
        /// Para agrega la condicion usada (si aplica) en reporte
        /// </summary>
        public string DescripcionCondicion { get; set; } = "";

        #region "PARA INTERFAZ DE COMUNICACION"
        public void CargarCodigoItem(string item)
        {
            txtcodigo.Text = item;   
        }

        public void RecargaGrilla()
        {
            //RECARGAR GRILLA
            if (txtComboPeriodo.Properties.DataSource != null)
            {
                if (txtcodigo.Text != "" && Calculo.PeriodoValido(Convert.ToInt32(txtComboPeriodo.EditValue)))
                {
                    if (cbTodos.Checked)
                    {
                        //BUSCAMOS PARA TODOS LOS REGISTROS
                        DialogResult pre = XtraMessageBox.Show("¿Deseas realizar la busqueda de forma automatica?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (pre == DialogResult.Yes)
                        {
                            buscarInfo(Convert.ToInt32(txtComboPeriodo.EditValue), txtcodigo.Text, "", cbShowOriginal.Checked, false);
                        }
                    }
                    else
                    {
                        if (txtConjunto.Text != "")
                        {
                            DialogResult pre = XtraMessageBox.Show("¿Deseas realizar la busqueda de forma automatica?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (pre == DialogResult.Yes)
                            {
                                string condicion = "";
                                condicion = CadenaConjunto(txtConjunto.Text);
                                buscarInfo(Convert.ToInt32(txtComboPeriodo.EditValue), txtcodigo.Text, txtConjunto.Text, cbShowOriginal.Checked, false);
                            }
                        }
                    }
                } 
            }            
        }

        #endregion

        #region "INTERFAZ CONJUNTO CONDICIONAL"
        public void CargarCodigoConjunto(string codigo)
        {
            txtConjunto.Text = codigo;
        }
        #endregion

        //DATASET
        DataSet data = new DataSet();

        //CODIGO ITEM CONSULTA
        string itemBusqueda = "";

        //PARA SABER SI SE INCLUYE COLUMNA VALOR ORIGINAL
        private bool ShowOriginal = false;
        private bool ShowHis = false;

        //TOTAL SUMA VALOR ORIGINAL
        private double totalOriginal = 0;
        private double totalCalculado = 0;

        //PARA GUARDAR EL FILTRO USUARIO
        private string FiltroUsuario { get; set; } = User.GetUserFilter();

        //USUARIO PUEDE VER FICHAS PRIVADAS
        private bool ShowPrivados { get; set; } = User.ShowPrivadas();

        /// <summary>
        /// Consulta sql base para buscar el valor de un item.
        /// </summary>
        string SqlItemBase = "SELECT dbo.fnFormateaRut(trabajador.rut) as rut, trabajador.contrato, CONCAT(apepaterno, ' ', apematerno, ' ', trabajador.nombre) as nombre, " +
                             "ccosto.nombre as centrocosto, cargo.nombre as cargo, sucursal.descSucursal as sucursal, area.nombre as area, " +
                             "{original} valorcalculado, itemTrabajador.coditem " +
                             "FROM itemtrabajador " +
                             "INNER JOIN trabajador On trabajador.contrato = itemtrabajador.contrato " +
                             "INNER JOIN area on area.id = trabajador.area " +
                             "INNER JOIN ccosto on ccosto.id = trabajador.ccosto " +
                             "INNER JOIN cargo on cargo.id = trabajador.cargo " +
                             "INNER JOIN sucursal on sucursal.codSucursal = trabajador.sucursal " +
                             "AND trabajador.anomes = itemtrabajador.anomes ";

        /// <summary>
        /// Consulta sql para obtener informacion totales liquidacion historica.
        /// </summary>
        string SqlItemHistorico = "SELECT dbo.fnFormateaRut(trabajador.rut) as rut, trabajador.contrato, concat(apepaterno, ' ', apematerno, ' ', trabajador.nombre) as nombre, " +
                                  "{original} pago as valorcalculado, '' as coditem, " +
                                  "ccosto.nombre as centrocosto, cargo.nombre as cargo, area.nombre as area, sucursal.descSucursal as sucursal " +
                                  "FROM liquidacionHistorico " +
                                  "INNER JOIN trabajador On trabajador.contrato = liquidacionHistorico.contrato " +
                                 "INNER JOIN area on area.id = trabajador.area " +
                                 "INNER JOIN ccosto on ccosto.id = trabajador.ccosto " +
                                 "INNER JOIN cargo on cargo.id = trabajador.cargo " +
                                 "INNER JOIN sucursal on sucursal.codsucursal = trabajador.sucursal " +
                                 "AND trabajador.anomes = liquidacionHistorico.anomes ";
        

        public frmDetalleItem()
        {
            InitializeComponent();
        }

        private void txtcodigo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || char.IsLetter(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void txtperiodo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsNumber(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void frmDetalleItem_Load(object sender, EventArgs e)
        {
            var sm = GetSystemMenu(Handle, false);
            EnableMenuItem(sm, SC_CLOSED, MF_BYCOMMAND | MF_DISABLED);

            datoCombobox.spLlenaPeriodos("SELECT anomes FROM parametro ORDER BY anomes DESC", txtComboPeriodo, "anomes", "anomes", true);

            propiedadesDefecto();
            fnSistema.spOpcionesGrilla(viewDetalleCalculado);
            if (txtComboPeriodo.Properties.DataSource != null)
                txtComboPeriodo.ItemIndex = 0;

            datoCombobox.AgrupaList(txtAgrupa);
        }

        #region "MANEJO DE DATOS"        

        //@1 --> GENERAR DATA EN BASE A CONJUNTO
        //@2 --> GENERAR FILTRO DEJANDO SOLO LOS REGISTROS QUE CORRESPONDAN AL PERIODO SELECCIONADO
        private string PreparaCondicion(string condicion, bool Historico)
        {
            if (condicion != "")
            {
                condicion = condicion.ToLower();
                if (Historico)
                {
                    if (condicion.Contains("contrato"))
                        condicion = condicion.Replace("contrato", "liquidacionhistorico.contrato");
                    if (condicion.Contains("rut"))
                        condicion = condicion.Replace("rut", "trabajador.rut");
                    if (condicion.Contains("anomes"))
                        condicion = condicion.Replace("anomes", "liquidacionhistorico.anomes");
                }
                else
                {
                    if (condicion.Contains("contrato"))
                        condicion = condicion.Replace("contrato", "itemtrabajador.contrato");
                    if (condicion.Contains("rut"))
                        condicion = condicion.Replace("rut", "itemtrabajador.rut");
                    if (condicion.Contains("anomes"))
                        condicion = condicion.Replace("anomes", "itemtrabajador.anomes");
                }               

                return condicion;
            }
            else
                return "";
        }

        //BUSCAR TODOS LOS ITEMS CON CAMPO ORIGINAL INCLUIDO SIN CONDICIONAL
        private void buscarInfo(int pPeriodo, string pItem, string pConjunto, bool pShowOriginal, bool pShowHistorico)
        {
            string sqlFiltro = "", sql = "";

            sqlFiltro = Calculo.GetSqlFiltro(FiltroUsuario, pConjunto, ShowPrivados);
            DescripcionCondicion = Conjunto.GetCondicionReporte(pConjunto, FiltroUsuario);            

            //Mostrar valor original?
            if (pShowOriginal)
            {
                //Mostrar un valor total?
                if (pShowHistorico)
                {
                    sql = SqlItemHistorico.Replace("{original}", "0 as valor,");
                    sqlFiltro = sql + $" WHERE liquidacionhistorico.anomes={pPeriodo} {PreparaCondicion(sqlFiltro, pShowHistorico)} ORDER BY apepaterno";
                }
                else
                {
                    sql = SqlItemBase.Replace("{original}", "valor, ");
                    sqlFiltro = sql + $" WHERE itemtrabajador.anomes={pPeriodo} AND itemtrabajador.coditem='{pItem}' AND suspendido=0 {PreparaCondicion(sqlFiltro, pShowHistorico)}  ORDER BY apepaterno";
                }          
            }
            else
            {
                //Mostrar un valor total?
                if (pShowHistorico)
                {
                    sql = SqlItemHistorico.Replace("{original}", "");
                    sqlFiltro = sql + $" WHERE liquidacionhistorico.anomes={pPeriodo} {PreparaCondicion(sqlFiltro, pShowHistorico)} ORDER BY apepaterno";
                }
                else
                {
                    sql = SqlItemBase.Replace("{original}", "");
                    sqlFiltro = sql + $" WHERE itemtrabajador.anomes={pPeriodo} AND itemtrabajador.coditem='{pItem}' AND suspendido=0 {PreparaCondicion(sqlFiltro, pShowHistorico)} ORDER BY apepaterno";
                }         
            }

                       
            SqlCommand cmd;
            SqlDataAdapter ad = new SqlDataAdapter();
            DateTime fechaformatoperiodo = DateTime.Now.Date;
            //RESETEAMOS DATASET
            viewDetalleCalculado.Columns.Clear();            
            DataSet ds = new DataSet();
            data.Clear();
            totalOriginal = 0;
            totalCalculado = 0;

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sqlFiltro, fnSistema.sqlConn))
                    {
                        ad.SelectCommand = cmd;
                        ad.Fill(ds, "itemtrabajador");

                        cmd.Dispose();
                        ad.Dispose();                        
                        fnSistema.sqlConn.Close();

                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            //PASAMOS DATASET COMO DATASOURCE
                            gridDetalleItem.DataSource = ds.Tables[0];
                            data = ds.Copy();
                            itemBusqueda = pItem;
                            btnImprimir.Enabled = true;

                            fechaformatoperiodo = fnSistema.FechaPeriodo(pPeriodo);
                            groupPeriodo.Text = "Periodo evaluado: " + fechaformatoperiodo.ToString("MMMM yyyy");
                            ShowOriginal = pShowOriginal;
                            ShowHis = pShowHistorico;
                            ColumnasGrilla();

                            foreach (DataTable tabla in ds.Tables)
                            {
                                foreach (DataRow row in tabla.Rows)
                                {
                                    totalCalculado = totalCalculado + Convert.ToDouble(row["valorcalculado"]);
                                    if(pShowOriginal)
                                        totalOriginal = totalOriginal + Convert.ToDouble(row["valor"]);
                                }
                            }

                            lblTotalCalculado.Text = "$" + totalCalculado.ToString("N0");
                            lblTotalOriginal.Text = "$" + totalOriginal.ToString("N0");
                        }
                        else
                        {
                            gridDetalleItem.DataSource = null;
                            XtraMessageBox.Show("No se encontraron registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            txtcodigo.Focus();                       
                            groupPeriodo.Text = "Periodo Evaluado:";
                            return;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }


        //METODO PARA BUSQUEDA
        private void buscarInfoItem(int periodo, string item, string condicional, bool? incluyeOriginal = false, bool? todos=false)
        {
            string sqlCalculado = string.Format("SELECT concat(nombre, ' ', apepaterno, ' ', apematerno) as nombre, valorcalculado, " +
                                    "itemtrabajador.coditem from itemtrabajador " +
                                    "INNER JOIN trabajador ON trabajador.contrato = itemTrabajador.contrato AND " +
                                    "trabajador.anomes = itemtrabajador.anomes " +
                                    "WHERE {0} AND itemTrabajador.anomes = {1} AND itemTrabajador.coditem = '{2}' ORDER BY nombre", 
                                     condicional, periodo, item);

            string sqlOriginal = string.Format("SELECT concat(nombre, ' ', apepaterno, ' ', apematerno) as nombre, valor, " +
                                 "valorcalculado, itemtrabajador.coditem FROM itemtrabajador INNER JOIN trabajador ON" +
                                 " trabajador.contrato = itemtrabajador.contrato AND trabajador.anomes = itemtrabajador.anomes " +
                                 " WHERE {0} AND itemtrabajador.anomes={1} AND itemtrabajador.coditem='{2}' ORDER BY nombre", 
                                  condicional, periodo, item);

            string sqlTodosOriginal = string.Format("SELECT concat(nombre, ' ', apepaterno, ' ', apematerno) as nombre, valor, " +
                                 "valorcalculado, itemtrabajador.coditem FROM itemtrabajador INNER JOIN trabajador ON" +
                                 " trabajador.contrato = itemtrabajador.contrato AND trabajador.anomes = itemtrabajador.anomes " +
                                 " WHERE itemtrabajador.anomes={0} AND itemtrabajador.coditem='{1}' ORDER BY nombre",
                                  periodo, item);

            string sqlTodosCalculado = string.Format("SELECT concat(nombre, ' ', apepaterno, ' ', apematerno) as nombre, valorcalculado, " +
                                 " itemtrabajador.coditem FROM itemtrabajador INNER JOIN trabajador ON" +
                                 " trabajador.contrato = itemtrabajador.contrato AND trabajador.anomes = itemtrabajador.anomes " +
                                 " WHERE itemtrabajador.anomes={0} AND itemtrabajador.coditem='{1}' ORDER BY nombre",
                                  periodo, item);

            string sql = "";

            if ((bool)incluyeOriginal && (bool)todos)
                sql = sqlTodosOriginal;
            if ((bool)incluyeOriginal && todos == false)
                sql = sqlOriginal;
            if (incluyeOriginal == false && todos == true)
                sql = sqlTodosCalculado;
            if (incluyeOriginal == false && todos == false)
                sql = sqlCalculado;           

            SqlCommand cmd;
            SqlDataAdapter adapter = new SqlDataAdapter();
            DateTime fechaFormatoPeriodo = DateTime.Now.Date;
            //RESET DATASET
            data.Clear();          
            
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETRO
                        cmd.Parameters.Add(new SqlParameter("@periodo", periodo));
                        cmd.Parameters.Add(new SqlParameter("@item", item));

                        adapter.SelectCommand = cmd;
                        adapter.Fill(data, "itemtrabajador");
                        cmd.Dispose();
                        adapter.Dispose();
                        fnSistema.sqlConn.Close();

                        if (data.Tables[0].Rows.Count>0)
                        {                            
                            gridDetalleItem.DataSource = data.Tables[0];                   
                            fnSistema.spOpcionesGrilla(viewDetalleCalculado);  
                            
                           // ColumnasGrilla();
                            lblTotalCalculado.Text = SumaItemsCalculado(periodo, item).ToString("N0");
                            btnImprimir.Enabled = true;
                            itemBusqueda = item;
                            fechaFormatoPeriodo = fnSistema.FechaPeriodo(periodo);
                            groupPeriodo.Text = "Periodo evaluado: " + fechaFormatoPeriodo.ToString("MMMM yyyy");
                        }
                        else
                        {
                            XtraMessageBox.Show("No se encontraron registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            txtcodigo.Focus();
                            gridDetalleItem.DataSource = null;
                            groupPeriodo.Text = "Periodo Evaluado:";                     
                            return;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

        }

        //SUMATORIA VALOR TOTAL ITEMS (PARA TODOS LOS ELEMENTOS D ELA BUSQUEDA)
        private double SumaItemsCalculado(int periodo, string item)
        {
            double sum = 0;
            string sql = "select valorcalculado from itemtrabajador " +
                       "INNER JOIN trabajador ON trabajador.contrato = itemTrabajador.contrato AND trabajador.anomes = itemtrabajador.anomes " +
                       "WHERE itemTrabajador.anomes = @periodo AND itemTrabajador.coditem = @item";
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@periodo", periodo));
                        cmd.Parameters.Add(new SqlParameter("@item", item));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                sum = sum + Convert.ToDouble((decimal)rd["valorcalculado"]);
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

            return sum;
        }

        private double SumaItemsOriginal(int periodo, string item)
        {
            double sum = 0;
            string sql = "select valor from itemtrabajador " +
                       "INNER JOIN trabajador ON trabajador.contrato = itemTrabajador.contrato AND trabajador.anomes = itemtrabajador.anomes " +
                       "WHERE itemTrabajador.anomes = @periodo AND itemTrabajador.coditem = @item";
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@periodo", periodo));
                        cmd.Parameters.Add(new SqlParameter("@item", item));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                sum = sum + Convert.ToDouble((decimal)rd["valor"]);
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

            return sum;
        }

        //PROPIEDADES GRIDVIEW
        private void ColumnasGrilla()
        {         


            if (ShowOriginal == false)
            {
                //Rut
                viewDetalleCalculado.Columns[0].Visible = true;
                //contrato
                viewDetalleCalculado.Columns[1].Visible = false;
                
                //nombre completo
                viewDetalleCalculado.Columns[2].Caption = "Trabajador";
                viewDetalleCalculado.Columns[2].Width = 180;               

                //Centro de costo
                viewDetalleCalculado.Columns[3].Visible = false;
                //cargo
                viewDetalleCalculado.Columns[4].Visible = false;
                //Sucursal
                viewDetalleCalculado.Columns[5].Visible = false;
                //Area
                viewDetalleCalculado.Columns[6].Visible = false;

                //Valor calculado
                viewDetalleCalculado.Columns[7].Caption = "Calculado";
                viewDetalleCalculado.Columns[7].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                viewDetalleCalculado.Columns[7].DisplayFormat.FormatString = "n2";
                viewDetalleCalculado.Columns[7].Width = 100;

                //REPRESENTARIS EL CODIGO DEL ITEM               
                viewDetalleCalculado.Columns[8].Visible = false;

            }
            else
            {
                //Rut
                viewDetalleCalculado.Columns[0].Visible = true;
                //contrato
                viewDetalleCalculado.Columns[1].Visible = false;

                //nombre completo
                viewDetalleCalculado.Columns[2].Caption = "Trabajador";
                viewDetalleCalculado.Columns[2].Width = 180;

                //Centro de costo
                viewDetalleCalculado.Columns[3].Visible = false;
                //cargo
                viewDetalleCalculado.Columns[4].Visible = false;
                //Sucursal
                viewDetalleCalculado.Columns[5].Visible = false;
                //Area
                viewDetalleCalculado.Columns[6].Visible = false;

                //Valor original
                viewDetalleCalculado.Columns[7].Caption = "Original";

                //Valor calculado
                viewDetalleCalculado.Columns[8].Caption = "Calculado";
                viewDetalleCalculado.Columns[8].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                viewDetalleCalculado.Columns[8].DisplayFormat.FormatString = "n2";
                viewDetalleCalculado.Columns[8].Width = 100;

                //REPRESENTARIS EL CODIGO DEL ITEM               
                viewDetalleCalculado.Columns[9].Visible = false;
            }            
        }

        //VERIFICAR SI EXISTE EL CODIGO DE ITEM INGRESADO
        private bool ExisteItem(string item)
        {
            bool existe = false;
            string sql = "SELECT coditem FROM item WHERE coditem=@item";
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@item", item));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //EL ITEM EXISTE
                            existe = true;
                        }
                        else
                        {
                            existe =false;
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
            return existe;
        }

        //MANEJO DE TECLA TAB
        protected override bool ProcessDialogKey(Keys keyData)
        {
            bool valido = false;
           
            if (keyData == Keys.Tab)
            {
               
               if (txtConjunto.ContainsFocus)
                {
                    //VALIDAR QUE EL CODIGO EXISTA
                    if (cbTodos.Checked == false)
                    {
                        if (txtConjunto.Text == "")
                        {
                            lblmensaje.Visible = true;
                            lblmensaje.Text = "Por favor ingresa un codigo de conjunto";
                            txtConjunto.Focus();
                            return false;
                        }
                        else
                        {
                            //VALIDAR QUE EL CONJUNTO EXISTA
                            if (CadenaConjunto(txtConjunto.Text) == "")
                            {
                                lblmensaje.Visible = true;
                                lblmensaje.Text = "El codigo de conjunto ingresado no existe";
                                txtConjunto.Focus();
                                return false;
                            }
                            else
                            {
                                lblmensaje.Visible = false;
                            }
                        }
                    }
                }
            }
            return base.ProcessDialogKey(keyData);  
        }

        //PROPIEDADES POR DEFECTO
        private void propiedadesDefecto()
        {
            gridDetalleItem.TabStop = false;         
            btnLimpiar.AllowFocus = false;
            btnImprimir.Enabled = false;
            cbTodos.Checked = true;
            txtConjunto.Text = "";
            txtConjunto.Enabled = false;
            lblmensaje.Visible = false;
        }

        //OBTENER DESCRIPCION ITEM
        private string DescripcionItem(string item)
        {
            string sql = "SELECT descripcion FROM item WHERE coditem=@item";
            SqlCommand cmd;
            SqlDataReader rd;
            string desc = "";
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@item", item));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                desc = (string)rd["descripcion"];
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

            return desc;
        }

        //IMPRIME DOCUMENTO (PDF)
        private void ImprimeDocumento(int periodo, bool? incluyeOriginal = false)
        {
            string descripcion = "", field = "";
            if (data.Tables[0].Rows.Count>0 && itemBusqueda != "")
            {
                Empresa emp = new Empresa();
                emp.SetInfo();               

                //SI TIENE DATOS IMPRIMIMOS
                DateTime fecha = DateTime.Now.Date;
                fecha = fnSistema.FechaPeriodo(periodo);
                descripcion = DescripcionItem(itemBusqueda);
                //rptDetalleItem rpt = new rptDetalleItem();
                RptDetalleItemv2 rpt = new RptDetalleItemv2();

                rpt.DataSource = data.Tables[0];               
                rpt.DataMember = "itemtrabajador";

                //rpt.Parameters["rutEmpresa"].Value = fnSistema.fFormatearRut2(emp.Rut);
                rpt.Parameters["periodo"].Value = fnSistema.FechaFormatoSoloMes(fecha).ToUpperInvariant();
                //rpt.Parameters["fecha"].Value = DateTime.Now.Date;
                //rpt.Parameters["total"].Value = "Total: " + totalCalculado.ToString("N0");
                //rpt.Parameters["totalOriginal"].Value = "Total: " + totalOriginal.ToString("N0");
                rpt.Parameters["item"].Value = descripcion;
                rpt.Parameters["registros"].Value = data.Tables[0].Rows.Count;
                rpt.Parameters["empresa"].Value = emp.Razon;
                rpt.Parameters["condicion"].Visible = false;
                rpt.Parameters["condicion"].Value = DescripcionCondicion;                

                

                foreach (DevExpress.XtraReports.Parameters.Parameter parametro in rpt.Parameters)
                {
                    parametro.Visible = false;
                }

                if (txtAgrupa.Properties.DataSource != null)
                {
                    rpt.Parameters["agrupacion"].Visible = false;
                    rpt.Parameters["agrupacion"].Value = txtAgrupa.Text;

                    if (txtAgrupa.EditValue.ToString() != "0")
                    {
                        rpt.groupFooterBand1.Visible = true;

                        rpt.groupHeaderBand1.GroupFields.Clear();
                        GroupField groupField = new GroupField();
                        field = txtAgrupa.Text.ToLower();
                        groupField.FieldName = field;
                        rpt.groupHeaderBand1.GroupFields.Add(groupField);

                        XRLabel labelGroup = new XRLabel { ForeColor = System.Drawing.Color.Black, WidthF = 747, TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft};
                        labelGroup.Font = new Font("Arial", 9, FontStyle.Italic | FontStyle.Bold);

                        if (Settings.Default.UserDesignerOptions.DataBindingMode == DataBindingMode.Bindings)
                        {
                            labelGroup.DataBindings.Add("Text", null, $"itemtrabajador.{field}");
                        }
                        else
                        {
                            labelGroup.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", $"[itemtrabajador.{field}]"));
                        }
                        rpt.groupHeaderBand1.Controls.Add(labelGroup);


                    }
                    else
                        rpt.groupFooterBand1.Visible = false;
                }

                Documento docu = new Documento("", 0);
                docu.ShowDocument(rpt);
            }
        }

        //GENERAR LISTA CON TODO LOS DATOS DE ACUERDO A CODIGO
        private string CadenaConjunto(string codigo)
        {
            string sql = "SELECT cadena FROM conjunto WHERE codigo = @codigo";
            SqlCommand cmd;
            SqlDataReader rd;
            string cadena = "";
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@codigo", codigo));
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                cadena = (string)rd["cadena"];
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
            return cadena;
        }

        //GENERA LISTADO DE ACUERDO A CONDICION DE CONJUNTO
        private List<Employe> ConjuntoEmpleados(string condicional)
        {
            List<Employe> lista = new List<Employe>();
            string sqlcalculado = string.Format("SELECT rut, contrato, CONCAT(nombre, ' ', apepaterno, ' ', apematerno) as nombre " +
                ", anomes FROM trabajador where {0}", condicional);
     
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sqlcalculado, fnSistema.sqlConn))
                    {
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                lista.Add(new Employe() {nombre = (string)rd["nombre"], rut=(string)rd["rut"],
                                 contrato = (string)rd["contrato"], anomes = (int)rd["anomes"]});
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

            return lista;
        }

        //GENERAR LISTA FINAL DE ACUERDO A PERIODO SELECCIONADO
        private List<Employe> ListaFinal(List<Employe> data, int periodo)
        {
            List<Employe> lista = new List<Employe>();
            if (data.Count>0)
            {
                foreach (var objeto in data)
                {
                    if (objeto.anomes == periodo)
                        lista.Add(objeto);
                }
            }
            return lista;
        }

        #endregion

   

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            bool AplicaOriginal = false;
            AplicaOriginal = cbShowOriginal.Checked;

            if (txtComboPeriodo.Properties.DataSource == null)
            { XtraMessageBox.Show("Periodo no válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (txtcodigo.Text != "" && Calculo.PeriodoValido(Convert.ToInt32(txtComboPeriodo.EditValue)))
            {
                if (cbTodos.Checked == false)
                {
                    //BUSCAMOS USANDO COMO EL FILTRO EL CONJUNTO SELECCIONADO
                    if (txtConjunto.Text == "")
                    {
                        XtraMessageBox.Show("Por favor selecciona un conjunto a evaluar", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    else
                    {
                        if (CadenaConjunto(txtConjunto.Text) == "")
                        {
                            lblmensaje.Visible = true;
                            lblmensaje.Text = "Codigo de conjunto no existe";
                            return;
                        }

                        lblmensaje.Visible = false;

                        //string condicion = "";
                        //condicion = CadenaConjunto(txtConjunto.Text);

                        buscarInfo(Convert.ToInt32(txtComboPeriodo.EditValue), txtcodigo.Text, txtConjunto.Text, cbShowOriginal.Checked, txtcodigo.Text == "TPAGO" ? true: false);
                    }
                }
                else
                {

                    buscarInfo(Convert.ToInt32(txtComboPeriodo.EditValue), txtcodigo.Text, "", cbShowOriginal.Checked, txtcodigo.Text == "TPAGO" ? true: false);
                }                

            }
            else
            {
                XtraMessageBox.Show("Por favor llena los campos antes de continuar", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Warning);
               

                return;
            }        
        } 


        private void btnCargaitem_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            //ABRIR FORMULARIO PARA CARGAR CODIGO DE ITEM
            frmGrillaItem fm = new frmGrillaItem();
            fm.opener = this;
            fm.ShowDialog();
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            txtcodigo.Text = "";            
            txtcodigo.Focus();
            gridDetalleItem.DataSource = null;
            groupPeriodo.Text = "Periodo evaluacion:";
            lblTotalCalculado.Text = "0";
            lblTotalOriginal.Text = "0";
            lblmensaje.Text = "";
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (objeto.ValidaAcceso(User.GetUserGroup(), "rptitemtrabajador") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
            
            // buscarInfoItem(Convert.ToInt32(txtperiodo.Text), txtcodigo.Text, 1);           
            ImprimeDocumento(Convert.ToInt32(txtComboPeriodo.EditValue));
        }

        private void txtcodigo_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtperiodo_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void panelControl1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            Close();
        }

        private void btnCondicional_Click(object sender, EventArgs e)
        {
            FrmFiltroBusqueda filtro = new FrmFiltroBusqueda();
            filtro.ShowDialog();
        }

        private void cbTodos_CheckedChanged(object sender, EventArgs e)
        {
            if (cbTodos.Checked)
            {
                lblmensaje.Visible = false;
                txtConjunto.Enabled = false;
                txtConjunto.Text = "";
                btnConjunto.Enabled = false;
            }
            else
            {
                txtConjunto.Enabled = true;
                btnConjunto.Enabled = true;
                txtConjunto.Focus();
            }
        }

        private void textEdit1_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            DXPopupMenu p = e.Menu;
            if (p != null)
            {
                e.Menu.Items.Clear();
                DXMenuItem menu = new DXMenuItem("Buscar conjunto", new EventHandler(BuscarConjunto_click));
                menu.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("images/actions/show_16x16.png");

                p.Items.Add(menu);
            }            
        }

        private void BuscarConjunto_click(object sender, EventArgs e)
        {
            FrmFiltroBusqueda filtro = new FrmFiltroBusqueda(true);
            filtro.opener = this;
            filtro.ShowDialog();
        }

        private void txtConjunto_DoubleClick(object sender, EventArgs e)
        {
            FrmFiltroBusqueda filtro = new FrmFiltroBusqueda(true);
            filtro.opener = this;            
            filtro.ShowDialog();
        }

        private void txtConjunto_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void txtConjunto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                //VALIDAR QUE EL CODIGO EXISTA
                if (cbTodos.Checked == false)
                {
                    if (txtConjunto.Text == "")
                    {
                        lblmensaje.Visible = true;
                        lblmensaje.Text = "Por favor ingresa un codigo de conjunto";
                        txtConjunto.Focus();
                        return;
                    }
                    else
                    {
                        //VALIDAR QUE EL CONJUNTO EXISTA
                        if (CadenaConjunto(txtConjunto.Text) == "")
                        {
                            lblmensaje.Visible = true;
                            lblmensaje.Text = "El codigo de conjunto ingresado no existe";
                            txtConjunto.Focus();
                            return;
                        }
                        else
                        {
                            lblmensaje.Visible = false;
                        }
                    }
                }
            }
        }

        private void btnConjunto_Click(object sender, EventArgs e)
        {
            FrmFiltroBusqueda filtro = new FrmFiltroBusqueda(true);
            filtro.opener = this;
            filtro.ShowDialog();
        }

        private void cbShowOriginal_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            if (viewDetalleCalculado.RowCount == 0)
            { XtraMessageBox.Show("No se encontró información", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (viewDetalleCalculado.RowCount > 0)
            {
                SaveFileDialog save = new SaveFileDialog();
                save.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                save.Filter = "Excel Worksheets|*.xlsx";
                if (save.ShowDialog() == DialogResult.OK)
                {
                    viewDetalleCalculado.ExportToXlsx(save.FileName);
                    if (System.IO.File.Exists(save.FileName))
                    {
                        DialogResult adv = XtraMessageBox.Show("Archivo generado correctamente\n¿Deseas ver archivo?", "Archivo", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                        if (adv == DialogResult.Yes)
                            System.Diagnostics.Process.Start(save.FileName);
                    }
                }
            }
        }
    }
}