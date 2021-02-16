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
using DevExpress.XtraReports.Configuration;
using DevExpress.Utils.Menu;
using DevExpress.XtraGrid;
using System.IO;

namespace Labour
{
    public partial class frmLiquidacionHistorica : DevExpress.XtraEditors.XtraForm, IConjuntosCondicionales
    {

        #region "CONJUNTOS CONDICIONALES"
        public void CargarCodigoConjunto(string code)
        {
            txtConjunto.Text = code;
        }
        #endregion

        [DllImport("user32")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32")]
        static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        const int MF_BYCOMMAND = 0;
        const int MF_DISABLED = 2;
        const int SC_CLOSED = 0xF060;

        //PARA MANIPULAR LA IMPRESION
        DataSet data = new DataSet();

        //SUMATORIAS
        double totalImponible = 0, totalHaberes = 0, totalDescuentos = 0, totalLiquido = 0, totalPago = 0;

        //PARA GUARDAR EL NUMERO DE CONTRATO Y EL NOMBRE DEL TRABJAJADOR
        string contrato = "", nombre = "";

        //PROPIEDAD PARA GUARDAR EL FILTRO DEL USUARIO LOGUEADO
        private string FiltroUsuario { get; set; } = User.GetUserFilter();

        /// <summary>
        /// Indica si la persona puede o no ver fichas con caracter privado.
        /// </summary>
        private bool ShowPrivados { get; set; } = User.ShowPrivadas();

        /// <summary>
        /// Representa la consulta base 
        /// </summary>
        string sqlConsulta = "SELECT liquidacionhistorico.anomes, concat(apepaterno, ' ', apematerno, ' ', trabajador.nombre) as nombre, " +
                             "liquidacionhistorico.contrato, dbo.fnFormateaRut(trabajador.rut) as rut, " +
                             "imponible, haberes, descuentos, liquido, pago, sysdiastr, sysdiaslic, " +
                             "CONCAT(ccosto.dato02, ' - ', ccosto.nombre) as centrocosto, sucursal.descsucursal as sucursal, " +
                             "area.nombre as area, cargo.nombre as cargo " + 
                             " from liquidacionHistorico " + 
                             "LEFT JOIN trabajador ON trabajador.contrato = liquidacionhistorico.contrato " +
                             "AND liquidacionHistorico.anomes = trabajador.anomes " +
                             "INNER JOIN calculomensual ON calculomensual.contrato = trabajador.contrato " +
                             "AND calculomensual.anomes= trabajador.anomes " +
                             "INNER JOIN ccosto on ccosto.id = trabajador.ccosto " +
                             "INNER JOIN sucursal ON sucursal.codSucursal = trabajador.sucursal " +
                             "INNER JOIN cargo ON cargo.id = trabajador.cargo " +
                             "INNER JOIN area on area.id = trabajador.area";

        string CondicionBusqueda = "";

        string PeriodoObservado = "";

        public frmLiquidacionHistorica()
        {
            InitializeComponent();
        }

        private void frmLiquidacionHistorica_Load(object sender, EventArgs e)
        {
            var sm = GetSystemMenu(Handle, false);
            EnableMenuItem(sm, SC_CLOSED, MF_BYCOMMAND | MF_DISABLED);

            datoCombobox.spLlenaPeriodos("SELECT anomes FROM parametro ORDER BY anomes DESC", comboPeriodo, "anomes", "anomes", true);            
            
            cbTodos.Checked = true;

            if (comboPeriodo.Properties.DataSource != null)
            {
                comboPeriodo.ItemIndex = 0;
                Busqueda("", Convert.ToInt32(comboPeriodo.EditValue), txtConjunto.Text);
            }

            datoCombobox.AgrupaList(txtAgrupa);
         
        }

        #region "MANEJO DE DATOS"

        //PREPARA FILTRO USUARIO
        private string PreparaFiltroUsuario(string condicion)
        {
            if (condicion != "")
            {
                condicion = condicion.ToLower();
                if (condicion.Contains("contrato"))
                    condicion = condicion.Replace("contrato", "liquidacionhistorico.contrato");
                //if (condicion.Contains("rut"))
                  //  condicion = condicion.Replace("rut", "trabajador.rut");
                if (condicion.Contains("anomes"))
                    condicion = condicion.Replace("anomes", "liquidacionhistorico.anomes");

                return condicion;
            }
            else
                return "";
        }
        private void Busqueda(string busqueda, int periodo, string Conjunto, bool? imprime = false, bool? FastPrint = false)
        {
            totalImponible = 0; totalHaberes = 0; totalDescuentos = 0; totalLiquido = 0; totalPago = 0;
            //RESET DATASET
            data.Clear();
            int dat = 0;

            string sql = "", sqlFiltro = "";
            PeriodoObservado = "";

            if (cbTodos.Checked)
            {
                sqlFiltro = Calculo.GetSqlFiltro(FiltroUsuario, "", ShowPrivados);
                if (FiltroUsuario != "0")
                    CondicionBusqueda = Labour.Conjunto.GetDescConjunto(FiltroUsuario);
                else
                    CondicionBusqueda = "No aplica";
            }
            else
            {
                sqlFiltro = Calculo.GetSqlFiltro(FiltroUsuario, Conjunto, ShowPrivados);
                CondicionBusqueda = Labour.Conjunto.GetDescConjunto(Conjunto);
                if (FiltroUsuario != "0")
                {
                    CondicionBusqueda = CondicionBusqueda + ";" + Labour.Conjunto.GetDescConjunto(FiltroUsuario);
                }               
                    
            }

            sqlFiltro = PreparaFiltroUsuario(sqlFiltro);

            sqlFiltro = sqlConsulta + $" WHERE liquidacionHistorico.anomes={periodo} #condition# {sqlFiltro} \n ORDER BY trabajador.apepaterno";

            //Quiero realizar una busqueda???
            if (busqueda.Length > 0)
            {
                //Es rut o contrato
                if (fnSistema.IsNumeric(busqueda))
                {
                    sqlFiltro = sqlFiltro.Replace("#condition#", $"AND (liquidacionHistorico.contrato LIKE '%{busqueda}%' OR trabajador.rut LIKE '%{busqueda}%')");
                }
                else
                {
                    sqlFiltro = sqlFiltro.Replace("#condition#", $" AND (trabajador.nombre LIKE '%{busqueda}%' OR trabajador.apepaterno LIKE '%{busqueda}%' OR trabajador.apepaterno LIKE '%{busqueda}%')");
                }
            }
            else
            {
                sqlFiltro = sqlFiltro.Replace("#condition#", "");
            }            

            SqlCommand cmd;        
            SqlDataAdapter ad = new SqlDataAdapter();         
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sqlFiltro, fnSistema.sqlConn))
                    {
                        cmd.Parameters.Add(new SqlParameter("@periodo", periodo));
                        ad.SelectCommand = cmd;
                        ad.Fill(data, "historico");

                        cmd.Dispose();
                        ad.Dispose();
                        fnSistema.sqlConn.Close();

                        if (data.Tables[0].Rows.Count > 0)
                        {
                            lblCount.Text = "Registros: " + data.Tables[0].Rows.Count.ToString();
                            gridHistorico.DataSource = data.Tables[0];
                            fnSistema.spOpcionesGrilla(viewHistorico);
                            ColumnasGrilla();
                            txtBusqueda.Focus();
                            PeriodoObservado = fnSistema.PrimerMayuscula(fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(periodo)));

                            viewHistorico.Columns["imponible"].Summary.Clear();
                            viewHistorico.Columns["haberes"].Summary.Clear();
                            viewHistorico.Columns["descuentos"].Summary.Clear();
                            viewHistorico.Columns["liquido"].Summary.Clear();
                            viewHistorico.Columns["pago"].Summary.Clear();
                            viewHistorico.Columns["sysdiastr"].Summary.Clear();
                            viewHistorico.Columns["sysdiaslic"].Summary.Clear();
                            viewHistorico.Columns["rut"].Summary.Clear();

                            GridColumnSummaryItem item1 = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "imponible", "{0:#,#}");
                            GridColumnSummaryItem item2 = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "haberes", "{0:#,#}");
                            GridColumnSummaryItem item3 = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "descuentos", "{0:#,#}");
                            GridColumnSummaryItem item4 = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "liquido", "{0:#,#}");
                            GridColumnSummaryItem item5 = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "pago", "{0:#,#}");
                            GridColumnSummaryItem item6 = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "sysdiastr", "{0}");
                            GridColumnSummaryItem item7 = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "sysdiaslic", "{0}");
                            GridColumnSummaryItem item8 = new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "rut", "Totales");

                            viewHistorico.Columns["imponible"].Summary.Add(item1);
                            viewHistorico.Columns["haberes"].Summary.Add(item2);
                            viewHistorico.Columns["descuentos"].Summary.Add(item3);
                            viewHistorico.Columns["liquido"].Summary.Add(item4);
                            viewHistorico.Columns["pago"].Summary.Add(item5);
                            viewHistorico.Columns["sysdiastr"].Summary.Add(item6);
                            viewHistorico.Columns["sysdiaslic"].Summary.Add(item7);
                            viewHistorico.Columns["rut"].Summary.Add(item8);

                        }
                        else
                        {
                            lblCount.Text = "Registros:0";
                            XtraMessageBox.Show("No se encontraron resultado", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);                          
                            txtBusqueda.Focus();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }           
        }

        //COLUMNAS
        private void ColumnasGrilla()
        {
            viewHistorico.Columns[0].Caption = "Periodo";
            viewHistorico.Columns[0].DisplayFormat.Format = new FormatCustom();
            viewHistorico.Columns[0].DisplayFormat.FormatString = "periodo";
            viewHistorico.Columns[0].Width = 90;

            viewHistorico.Columns[1].Caption = "Trabajador";
            viewHistorico.Columns[1].Width = 200;

            viewHistorico.Columns[2].Caption = "Contrato";
            viewHistorico.Columns[2].Width = 70;

            viewHistorico.Columns[3].Caption = "Rut";
            viewHistorico.Columns[3].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            viewHistorico.Columns[3].DisplayFormat.FormatString = "Rut";
            viewHistorico.Columns[3].DisplayFormat.Format = new FormatCustom();           

            viewHistorico.Columns[4].Caption = "Imponible";            
            viewHistorico.Columns[4].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            viewHistorico.Columns[4].DisplayFormat.FormatString = "n1";

            viewHistorico.Columns[5].Caption = "Haberes";
            viewHistorico.Columns[5].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            viewHistorico.Columns[5].DisplayFormat.FormatString = "n1";

            viewHistorico.Columns[6].Caption = "Descuentos";
            viewHistorico.Columns[6].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            viewHistorico.Columns[6].DisplayFormat.FormatString = "n1";

            viewHistorico.Columns[7].Caption = "Liquido";
            viewHistorico.Columns[7].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            viewHistorico.Columns[7].DisplayFormat.FormatString = "n1";

            viewHistorico.Columns[8].Caption = "Pago";
            viewHistorico.Columns[8].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            viewHistorico.Columns[8].DisplayFormat.FormatString = "n1";

            viewHistorico.Columns[9].Caption = "Trabajados";
            viewHistorico.Columns[9].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            viewHistorico.Columns[9].DisplayFormat.FormatString = "n2";

            viewHistorico.Columns[10].Caption = "Licencias";
            viewHistorico.Columns[10].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            viewHistorico.Columns[10].DisplayFormat.FormatString = "n2";

            viewHistorico.Columns[11].Visible = false;
            viewHistorico.Columns[12].Visible = false;
            viewHistorico.Columns[13].Visible = false;
            viewHistorico.Columns[14].Visible = false;

        }

        //IMPRIME DOCUMENTO
        private void ImprimeDocumento(bool? impresionRapida = false, bool editar = false)
        {
            if (data.Tables[0].Rows.Count > 0)
            {
                //rptLiquidacionHistorica liq = new rptLiquidacionHistorica();
                ReportesExternos.rptLiquidacionHistorica liq = new ReportesExternos.rptLiquidacionHistorica();
                liq.LoadLayoutFromXml(Path.Combine(fnSistema.RutaCarpetaReportesExterno, "rptLiquidacionHistorica.repx"));
                liq.DataSource = data.Tables[0];
                liq.DataMember = "historico";
                string field = "";

                Empresa emp = new Empresa();
                emp.SetInfo();

                //PARAMETROS
                liq.Parameters["totalimponible"].Value = totalImponible;
                liq.Parameters["totalhaberes"].Value = totalHaberes;
                liq.Parameters["totaldescuento"].Value = totalDescuentos;
                liq.Parameters["totalliquido"].Value = totalLiquido;
                liq.Parameters["totalpago"].Value = totalPago;
                liq.Parameters["empresa"].Value = emp.Razon;
                liq.Parameters["rutEmpresa"].Value = emp.Rut;
                liq.Parameters["condicion"].Value = CondicionBusqueda;
                liq.Parameters["periodo"].Value = PeriodoObservado;

                //PARAMETROS VISIBLE                

                foreach (DevExpress.XtraReports.Parameters.Parameter parametro in liq.Parameters)
                    parametro.Visible = false;

                if (txtAgrupa.Properties.DataSource != null)
                {
                    if (txtAgrupa.EditValue.ToString() != "0")
                    {
                        liq.Parameters["agrupacion"].Value = txtAgrupa.Text;
                        liq.Parameters["agrupacion"].Visible = false;

                        liq.groupFooterBand1.Visible = true;

                        liq.groupHeaderBand1.GroupFields.Clear();
                        GroupField groupField = new GroupField();
                        field = txtAgrupa.Text.ToLower();
                        groupField.FieldName = field;
                        liq.groupHeaderBand1.GroupFields.Add(groupField);

                        XRLabel labelGroup = new XRLabel { ForeColor = System.Drawing.Color.Black, WidthF = 747, TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft };
                        labelGroup.Font = new Font("Arial", 9, FontStyle.Italic | FontStyle.Bold);

                        if (Settings.Default.UserDesignerOptions.DataBindingMode == DataBindingMode.Bindings)
                        {
                            labelGroup.DataBindings.Add("Text", null, $"itemtrabajador.{field}");
                        }
                        else
                        {
                            labelGroup.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", $"[itemtrabajador.{field}]"));
                        }
                        liq.groupHeaderBand1.Controls.Add(labelGroup);
                    }
                    else
                        liq.groupFooterBand1.Visible = false;
                }

                Documento d = new Documento("", 0);

                if (editar)
                {
                    splashScreenManager1.ShowWaitForm();
                    //Se le pasa el waitform para que se cierre una vez cargado
                    DiseñadorReportes.MostrarEditorLimitado(liq, "rptLiquidacionHistorica.repx", splashScreenManager1);
                }
                else
                {
                    //liq.SaveLayoutToXml(Path.Combine(fnSistema.RutaCarpetaReportesExterno, "rptLiquidacionHistorica.repx"));
                    if ((bool)impresionRapida)
                        d.PrintDocument(liq);
                    else
                        d.ShowDocument(liq);
                }


            }
            else 
            {
                { XtraMessageBox.Show("No existen resultados", "Periodo", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
            }
        }

        //ORDEN


        #endregion

        private void txtBusqueda_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar) || char.IsLetter(e.KeyChar) || e.KeyChar == (char)45)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void btnConsultar_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (Calculo.PeriodoValido(Convert.ToInt32(comboPeriodo.EditValue)) == false)
            { XtraMessageBox.Show("Por favor ingresa un periodo válido", "Periodo", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }            

            if (cbTodos.Checked)
            {
                //BUSCAMOS TODOS LOS REGISTROS
                Busqueda(txtBusqueda.Text, Convert.ToInt32(Convert.ToInt32(comboPeriodo.EditValue)), "");
            }
            else
            {
                //BUSCAMOS PARA EL CONJUNTO SELECCIONADO
                if (Conjunto.ExisteConjunto(txtConjunto.Text) == false)
                { XtraMessageBox.Show("Parece ser que el conjunto ingresado no existe", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

                //BuscarConjunto(txtConjunto.Text, Convert.ToInt32(comboPeriodo.EditValue));
                Busqueda(txtBusqueda.Text, Convert.ToInt32(comboPeriodo.EditValue), txtConjunto.Text);
            }
                  
        }        

        private void frmLiquidacionHistorica_Shown(object sender, EventArgs e)
        {
            txtBusqueda.Focus();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            int periodoConsulta = 0;
            periodoConsulta = Convert.ToInt32(comboPeriodo.EditValue);
            Busqueda("", periodoConsulta, "");           
            txtBusqueda.Text = "";
            txtBusqueda.Focus();
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (objeto.ValidaAcceso(User.GetUserGroup(), "rptliqtotal") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            ImprimeDocumento();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            Close();
        }

        private void viewHistorico_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            DXPopupMenu menu = e.Menu;
            string nombre = "";
            if (menu != null)
            {
                if (viewHistorico.RowCount > 0) nombre = (string)viewHistorico.GetFocusedDataRow()["nombre"];

                DXMenuItem submenu = new DXMenuItem("Ver todas las liquidaciones de " + nombre, new EventHandler(VerLiquidaciones_Click));
                submenu.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("images/actions/show_16x16.png");
                menu.Items.Clear();
                menu.Items.Add(submenu);
            }
        }

        private void VerLiquidaciones_Click(object sender, EventArgs e)
        {
            if (viewHistorico.RowCount > 0)
            {
                if (objeto.ValidaAcceso(User.GetUserGroup(), "frmdochis") == false)
                { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

               string contratoFila = "", trabajador = "", rut = ""; int periodoFila = 0;
               contratoFila = viewHistorico.GetFocusedDataRow()["contrato"] + "";
               periodoFila = Convert.ToInt32(viewHistorico.GetFocusedDataRow()["anomes"]);
               trabajador = viewHistorico.GetFocusedDataRow()["nombre"].ToString();
               rut = fnSistema.fnExtraerCaracteres(viewHistorico.GetFocusedDataRow()["rut"].ToString());

               frmLiquidacionesTrabajador liq = new frmLiquidacionesTrabajador(contratoFila, trabajador, rut);
               liq.StartPosition = FormStartPosition.CenterParent;
               liq.ShowDialog();                
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
                txtConjunto.Text = "";
                txtConjunto.Focus();
                btnConjunto.Enabled = true;
            }           
        }

        private void txtConjunto_DoubleClick(object sender, EventArgs e)
        {
            FrmFiltroBusqueda filtro = new FrmFiltroBusqueda(true);
            filtro.opener = this;
            filtro.ShowDialog();
        }

        private void textEdit1_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
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
            FrmFiltroBusqueda filtro = new FrmFiltroBusqueda(true);
            filtro.opener = this;
            filtro.ShowDialog();
        }

        private void gridHistorico_Click(object sender, EventArgs e)
        {        
           
            if (viewHistorico.RowCount > 0)
            {
                lblLiquidaciones.Text = $"LIQUIDACIONES DISPONIBLES PARA {viewHistorico.GetFocusedDataRow()["nombre"]}: " + Calculo.GetLiqCount(viewHistorico.GetFocusedDataRow()["rut"].ToString());
            }
        }

        private void gridHistorico_KeyUp(object sender, KeyEventArgs e)
        {
            if (viewHistorico.RowCount > 0)
            {
                lblLiquidaciones.Text = $"LIQUIDACIONES DISPONIBLES PARA {viewHistorico.GetFocusedDataRow()["nombre"]} :" + Calculo.GetLiqCount(viewHistorico.GetFocusedDataRow()["rut"].ToString());
            }
        }

        private void btnEditarReporte_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (objeto.ValidaAcceso(User.GetUserGroup(), "rptliqtotal") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            ImprimeDocumento(editar: true);
        }

        private void btnConjunto_Click(object sender, EventArgs e)
        {
            FrmFiltroBusqueda filtro = new FrmFiltroBusqueda(true);
            filtro.opener = this;
            filtro.ShowDialog();
        }


        private void btnImpresionRapida_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (objeto.ValidaAcceso(User.GetUserGroup(), "rptliqtotal") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            ImprimeDocumento(true);
        }

        private void txtBusqueda_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtPeriodo_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void gridHistorico_DoubleClick(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            if (viewHistorico.RowCount>0)
            {
                if (objeto.ValidaAcceso(User.GetUserGroup(), "frmdochis") == false)
                { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                DialogResult pregunta = XtraMessageBox.Show("¿Desea revisar todas las liquidaciones de este trabajador?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (pregunta == DialogResult.Yes)
                {
                    string contratoFila = "", trabajador = "", rut = ""; int periodoFila = 0;
                    contratoFila = viewHistorico.GetFocusedDataRow()["contrato"] + "";
                    periodoFila = Convert.ToInt32(viewHistorico.GetFocusedDataRow()["anomes"]);
                    trabajador = viewHistorico.GetFocusedDataRow()["nombre"].ToString();
                    rut = fnSistema.fnExtraerCaracteres(viewHistorico.GetFocusedDataRow()["rut"].ToString());

                    frmLiquidacionesTrabajador liq = new frmLiquidacionesTrabajador(contratoFila, trabajador, rut);
                    liq.StartPosition = FormStartPosition.CenterParent;
                    liq.ShowDialog();
                }             
            }
        }

   
    }
}