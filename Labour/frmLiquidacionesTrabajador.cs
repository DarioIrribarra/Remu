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
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using DevExpress.XtraReports.UI;
using DevExpress.Utils.Menu;
using System.Threading;

namespace Labour
{
    public partial class frmLiquidacionesTrabajador : DevExpress.XtraEditors.XtraForm
    {
        [DllImport("user32")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32")]
        static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        const int MF_BYCOMMAND = 0;
        const int MF_DISABLED = 2;
        const int SC_CLOSED = 0xF060;

        //PARA GUARDAR PERIODO FICHA
        public int PeriodoFicha { get; set; } = 0;

        //PARA GUARDAR CONTRATO
        private string Contrato = "";

        //PARA GUARDAR NOMBRE TRABAJADOR
        private string Nombre = "";

        //PARA GUARDAR EL RUT DEL TRABAJADOR
        private string Rut = "";

        //DATASET PARA CONSULTA
        DataSet ds = new DataSet();

        //LISTADO DE PERIODOS ENCONTRADOS
        List<int> Periodos = new List<int>();

        //GUARDAMOS CONTRATO Y PERIODO DE LA LIQUIDACION HISTORICA...
        List<LiquidacionHistorico> Liquidaciones = new List<LiquidacionHistorico>();

        //PARA GUARDAR EL PERIODO DE LA FILA SELECCIONADA
        private int PeriodoFila = 0;


        public frmLiquidacionesTrabajador()
        {
            InitializeComponent();
        }

        //CONSTRUCTOR PARAMETRIZADO
        public frmLiquidacionesTrabajador(string contrato, string nombre, string rut)
        {
            InitializeComponent();
            Contrato = contrato;
            Nombre = nombre;
            Rut = rut;
        }

        private void frmLiquidacionesTrabajador_Load(object sender, EventArgs e)
        {           

            var sm = GetSystemMenu(Handle, false);
            EnableMenuItem(sm, SC_CLOSED, MF_BYCOMMAND | MF_DISABLED);            

            if (Contrato != "" && Nombre != "" && Rut != "")
            {                            

                Text = "Liquidaciones => " + Nombre;

                //POR DEFECTO DESHABILITAMOS FILTRO POR RANGO
                cbRango.Checked = false;
                
                txtDesde.Enabled = false;
                txtHasta.Enabled = false;                

                lblTrabajador.Text = "Trabajador: " + Nombre;
                lblrut.Text = "Rut: " + fnSistema.fFormatearRut2(Rut);

                //POR DEFECTO CARGAMOS TODO EL HISTORICO
                CargaLiquidaciones(Rut);

                //COMBOS
                CargaCombo(txtDesde);
                CargaCombo(txtHasta);



            }
        }

        #region "MANEJO DE DATOS"
   

        //CARGAR DATOS DESDE TABLA LIQUIDACION HISTORICA
        //CARGA TODAS LAS LIQUIDACIONES REGISTRADAS AL NUMERO DE CONTRATO ASOCIADO
        private void CargaLiquidaciones(string Rut)
        {
            string sql = "SELECT trabajador.contrato, trabajador.anomes, rut, ingreso, salida,  imponible, " +
                         "haberes, descuentos, liquido, pago, sysdiastr, sysdiaslic, mail FROM liquidacionhistorico " +
                        "INNER JOIN trabajador ON trabajador.contrato = liquidacionHistorico.contrato AND " +
                        "trabajador.anomes = liquidacionhistorico.anomes " +
                        "INNER JOIN calculoMensual ON calculomensual.contrato = trabajador.contrato AND " +
                        "calculoMensual.anomes = trabajador.anomes " +
                        "WHERE trabajador.rut = @rut " +
                        "ORDER BY trabajador.anomes desc, ingreso desc";

            SqlCommand cmd;
            SqlDataAdapter ad = new SqlDataAdapter();

            ds.Clear();
            Periodos.Clear();
            Liquidaciones.Clear();

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@rut", Rut));

                        ad.SelectCommand = cmd;
                        ad.Fill(ds, "liquidacion");
                        cmd.Dispose();
                        ad.Dispose();
                        fnSistema.sqlConn.Close();

                        if (ds.Tables[0].Rows.Count>0)
                        {
                            gridLiquidacion.DataSource = ds.Tables[0];
                            fnSistema.spOpcionesGrilla(viewLiquidacion);
                            ColumnasGrilla();
                            btnImprimir.Enabled = true;
                            btnImpresionRapida.Enabled = true;                         

                            DataTable tabla = ds.Tables[0];

                            foreach (DataRow row in tabla.Rows)
                            {
                                //GUARDAR PERIODOS EN LISTA
                                //Periodos.Add((int)row["anomes"]);

                                //GUARDAMOS LISTADO DE CONTRATOS
                                Liquidaciones.Add(new LiquidacionHistorico() { Contrato = (string)row["contrato"], Periodo = Convert.ToInt32(row["anomes"]), Rut = (string)row["rut"], Mail = (string)row["mail"]});
                            }
                        }
                        else
                        {
                            XtraMessageBox.Show("No se encontraron registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            btnImprimir.Enabled = false;
                            btnImpresionRapida.Enabled = false;                            
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        //BUSCAR LIQUIDACION EN UN RANGO DE FECHAS
        private void CargaRangoLiq(int desde, int hasta, string Rut)
        {
            string sql = "SELECT trabajador.contrato, trabajador.anomes, rut, ingreso, salida,  imponible, " +
                         "haberes, descuentos, liquido, pago, sysdiastr, sysdiaslic, mail FROM liquidacionhistorico " +
                        "INNER JOIN trabajador ON trabajador.contrato = liquidacionHistorico.contrato AND " +
                        "trabajador.anomes = liquidacionhistorico.anomes " +
                        "INNER JOIN calculoMensual ON calculomensual.contrato = trabajador.contrato AND " +
                        "calculoMensual.anomes = trabajador.anomes " +
                        "WHERE trabajador.rut = @rut AND liquidacionHistorico.anomes " +
                        "BETWEEN @desde AND @hasta ORDER BY trabajador.anomes DESC";

            SqlCommand cmd;
            SqlDataAdapter ad = new SqlDataAdapter();

            ds.Clear();
            Periodos.Clear();
            Liquidaciones.Clear();

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@rut", Rut));
                        cmd.Parameters.Add(new SqlParameter("@desde", desde));
                        cmd.Parameters.Add(new SqlParameter("@hasta", hasta));

                        ad.SelectCommand = cmd;
                        ad.Fill(ds, "liquidacion");
                        ad.Dispose();
                        cmd.Dispose();
                        fnSistema.sqlConn.Close();

                        if (ds.Tables[0].Rows.Count>0)
                        {
                            gridLiquidacion.DataSource = ds.Tables[0];
                            fnSistema.spOpcionesGrilla(viewLiquidacion);
                            ColumnasGrilla();
                            btnImpresionRapida.Enabled = true;
                            btnImprimir.Enabled = true;

                            DataTable tabla = ds.Tables[0];

                            foreach (DataRow row in tabla.Rows)
                            {
                                //GUARDAR PERIODOS EN LISTA
                                //Periodos.Add((int)row["anomes"]);

                                Liquidaciones.Add(new LiquidacionHistorico() { Contrato = (string)row["contrato"], Periodo = Convert.ToInt32(row["anomes"]), Rut = (string)row["rut"]});
                            }
                        }
                        else
                        {
                            XtraMessageBox.Show("No se encontraron liquidaciones", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            btnImpresionRapida.Enabled = false;
                            btnImprimir.Enabled = false;
                        }                       
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        //CARGAR COMBO
        private void CargaCombo(LookUpEdit pCombo)
        {
            string sql = "SELECT anomes FROM parametro order by anomes";
            SqlCommand cmd;
            SqlDataReader rd;
            List<datoCombobox> lista = new List<datoCombobox>();

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
                              
                                lista.Add(new datoCombobox() { KeyInfo = (int)rd["anomes"], descInfo = (int)rd["anomes"] + ""});
                            }
                        }
                        else
                        {
                            XtraMessageBox.Show("No hay registros");
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
        }

        //Limpiar campos
        private void LimpiarCampos()
        {
            cbRango.Checked = false;            
            txtDesde.Enabled = false;
            txtHasta.Enabled = false;
            //gridLiquidacion.DataSource = null;
        }

        //COLUMNAS GRILLA
        private void ColumnasGrilla()
        {
            viewLiquidacion.Columns[0].Visible = false;

            viewLiquidacion.Columns[1].Caption = "Periodo";
            viewLiquidacion.Columns[1].DisplayFormat.Format = new FormatCustom();
            viewLiquidacion.Columns[1].DisplayFormat.FormatString = "periodo";
            viewLiquidacion.Columns[1].Width = 100;

            viewLiquidacion.Columns[2].Caption = "Rut";
            viewLiquidacion.Columns[2].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            viewLiquidacion.Columns[2].DisplayFormat.FormatString = "Rut";
            viewLiquidacion.Columns[2].DisplayFormat.Format = new FormatCustom();

            //viewLiquidacion.Columns[2].DisplayFormat.FormatString = "Rut";

            viewLiquidacion.Columns[3].Caption = "Fecha ingreso";

            viewLiquidacion.Columns[4].Caption = "Fecha Salida";

            viewLiquidacion.Columns[5].Caption = "Imponible";
            viewLiquidacion.Columns[5].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            viewLiquidacion.Columns[5].DisplayFormat.FormatString = "n0";

            viewLiquidacion.Columns[6].Caption = "Haberes";
            viewLiquidacion.Columns[6].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            viewLiquidacion.Columns[6].DisplayFormat.FormatString = "n0";

            viewLiquidacion.Columns[7].Caption = "Descuentos";
            viewLiquidacion.Columns[7].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            viewLiquidacion.Columns[7].DisplayFormat.FormatString = "n0";

            viewLiquidacion.Columns[8].Caption = "Liquido";
            viewLiquidacion.Columns[8].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            viewLiquidacion.Columns[8].DisplayFormat.FormatString = "n0";

            viewLiquidacion.Columns[9].Caption = "Pago";
            viewLiquidacion.Columns[9].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            viewLiquidacion.Columns[9].DisplayFormat.FormatString = "n0";

            viewLiquidacion.Columns[10].Caption = "Dias trab";
            viewLiquidacion.Columns[11].Caption = "Dias lic";
            viewLiquidacion.Columns[12].Visible = false;
        }

        //VERIFICAR SI EL PERIODO EXISTE
        private bool ExistePeriodo(string contrato, int periodo)
        {
            string sql = "SELECT contrato FROM trabajador WHERE contrato=@contrato AND anomes=@periodo";
            SqlCommand cmd;
            SqlDataReader rd;
            bool existe = false;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@contrato", contrato));
                        cmd.Parameters.Add(new SqlParameter("@periodo", periodo));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            existe = true;
                        }
                        else
                        {
                            existe = false;
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

            return existe;
        }


        //IMPRIME USANDO FORMATO 2
        private void ImprimeDocu(int? pPeriodoInicio = 0, int? pPeriodoTermino = 0, bool? impresionRapida = false, bool? pdfFile = false)
        {
            int contador = 0;
            double Pago = 0;
            XtraReport reporteAux = new XtraReport();
            reporteAux.CreateDocument();
            
            if (Liquidaciones.Count > 0 && Contrato != "")
            {
                splashScreenManager1.ShowWaitForm();
                //RECORREMOS Y CALCULAMOS
                foreach (var liquidacion in Liquidaciones)
                {
                    Pago = Calculo.GetValueFromCalculoMensaul(liquidacion.Contrato, liquidacion.Periodo, "syspago");

                    if (Pago > 0)
                    {
                        Documento d = new Documento(liquidacion.Contrato, liquidacion.Periodo);
                        XtraReport reporte = new XtraReport();

                        reporte = d.SoloHaberesAnteriores();

                        reporte.CreateDocument();

                        contador++;

                        if (contador >= 1)
                        {
                            //UNIMOS PAGINAS (MERGE PAGE)
                            reporteAux.Pages.AddRange(reporte.Pages);
                        }
                    }
                }

                splashScreenManager1.CloseWaitForm();
                //MOSTRAMOS REPORTE
                Documento d1 = new Documento("", 0);
                if ((bool)impresionRapida)
                    d1.PrintDocument(reporteAux);
                else if ((bool)pdfFile)
                    d1.ExportToPdf(reporteAux, $"Liquidaciones{((pPeriodoInicio > 0 && pPeriodoTermino > 0)? $"[{pPeriodoInicio}-{pPeriodoTermino}]":"")}[{Contrato}]");
                else
                    d1.ShowDocument(reporteAux);            
            }
            else
            {
                XtraMessageBox.Show("No hay registros", "informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /*GENERAR ARCHIVO PDF DE ACUERDO A LISTADO DE LIQUIDACIONES*/        

        private void ImprimeDocuIndividual()
        {
            if (viewLiquidacion.RowCount>0)
            {
                string contrato = (string)viewLiquidacion.GetFocusedDataRow()["contrato"];
                int periodo = Convert.ToInt32(viewLiquidacion.GetFocusedDataRow()["anomes"]);

                //Haberes hab = new Haberes(contrato, periodo);
                //hab.CalculoGenerico();

                XtraReport reporte = new XtraReport();
                Documento d = new Documento(contrato, periodo);
                reporte = d.SoloHaberesAnteriores();

                d.PrintDocument(reporte);
            }
        }

        #endregion

        private void cbRango_CheckedChanged(object sender, EventArgs e)
        {
            if (cbRango.Checked)
            {
                txtDesde.Enabled = true;
                txtHasta.Enabled = true;
                txtDesde.Focus();
                txtDesde.ItemIndex = 0;
                txtHasta.ItemIndex = 0;
            }
            else
            {
                txtDesde.Enabled = false;
                txtHasta.Enabled = false;
                CargaLiquidaciones(Rut);
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            int desde = 0, hasta = 0;
            if (cbRango.Checked)
            {
                //BUSCAMOS POR RANGO
                if (txtDesde.EditValue == null)
                { XtraMessageBox.Show("Por favor selecciona el periodo de comienzo", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);txtDesde.Focus(); return;}
                if (txtHasta.EditValue == null)
                { XtraMessageBox.Show("Por favor selecciona el periodo de termino", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);return; }

                desde = Convert.ToInt32(txtDesde.EditValue);
                hasta = Convert.ToInt32(txtHasta.EditValue);

                if (desde > hasta)
                { XtraMessageBox.Show("La fecha de inicio no puede ser superior a la fecha de termino de periodo", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);return;}

                //BUSCAMOS
                CargaRangoLiq(desde, hasta, Rut);
            }
            else
            {
                //MOSTRAMOS TODOS LOS REGISTROS ENCONTRAROS PARA EL CONTRATO
                CargaLiquidaciones(Rut);
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            LimpiarCampos();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (objeto.ValidaAcceso(User.GetUserGroup(), "rptliqhistorico") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            int desde = 0, hasta = 0;
            if (cbRango.Checked)
            {
                desde = Convert.ToInt32(txtDesde.EditValue);
                hasta = Convert.ToInt32(txtHasta.EditValue);

                DialogResult pregunta = XtraMessageBox.Show("¿Seguro deseas generar reporte con las liquidaciones entre los periodos " + desde + " y " + hasta + "?", "Informacion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (pregunta == DialogResult.Yes)
                {
                    ImprimeDocu();
                }
            }
            else
            {
                DialogResult pregunta = XtraMessageBox.Show("¿Seguro deseas generar reporte con todas las liquidaciones de este trabajador?", "Informacion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (pregunta == DialogResult.Yes)
                {
                    ImprimeDocu();
                }
            }            
        }

        private void txtDesde_EditValueChanged(object sender, EventArgs e)
        {
            txtHasta.EditValue = txtDesde.EditValue;
        }

        private void btnImpresionRapida_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (objeto.ValidaAcceso(User.GetUserGroup(), "rptliqhistorico") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            int desde = 0, hasta = 0;
            if (cbRango.Checked)
            {
                desde = Convert.ToInt32(txtDesde.EditValue);
                hasta = Convert.ToInt32(txtHasta.EditValue);

                DialogResult pregunta = XtraMessageBox.Show("¿Seguro deseas imprimir las liquidaciones entre los periodos " + desde + " y " + hasta + "?", "Informacion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (pregunta == DialogResult.Yes)
                {
                    ImprimeDocu(0, 0, true);
                }
            }
            else
            {
                DialogResult pregunta = XtraMessageBox.Show("¿Seguro deseas imprimir todas las liquidaciones de este trabajador?", "Informacion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (pregunta == DialogResult.Yes)
                {
                    ImprimeDocu(0, 0, true);
                }
            }
        }

        private void gridLiquidacion_DoubleClick(object sender, EventArgs e)
        {
            //NUEVA SESION
            Sesion.NuevaActividad();

            if (viewLiquidacion.RowCount>0)
            {
                if (objeto.ValidaAcceso(User.GetUserGroup(), "frmliquidacion") == false)
                { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                DialogResult pregunta = XtraMessageBox.Show("¿Deseas ver esta liquidacion?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (pregunta == DialogResult.Yes)
                {
                    //NECESITAMOS EL PERIODO Y EL NUMERO DE CONTRATO ASOCIADO
                    string contrato = "";
                    int periodo = 0;
                    contrato = viewLiquidacion.GetFocusedDataRow()["contrato"].ToString();
                    periodo = Convert.ToInt32(viewLiquidacion.GetFocusedDataRow()["anomes"]);

                    frmPrevLiquidacion prev = new frmPrevLiquidacion(contrato, periodo);
                    prev.ShowDialog();

                    //Documento d = new Documento(contrato, periodo);
                    //XtraReport rpt = new XtraReport();
                    //rpt = d.SoloHaberes();
                    //d.ShowDocument(rpt);
                }                
            }
        }

        private void gridLiquidacion_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyData == Keys.Enter)
            //{
            //    if (viewLiquidacion.RowCount > 0)
            //    {
            //        DialogResult pregunta = XtraMessageBox.Show("¿Deseas ver esta liquidacion?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            //        if (pregunta == DialogResult.Yes)
            //        {
            //            //NECESITAMOS EL PERIODO Y EL NUMERO DE CONTRATO ASOCIADO
            //            string contrato = "";
            //            int periodo = 0;
            //            contrato = viewLiquidacion.GetFocusedDataRow()["contrato"].ToString();
            //            periodo = Convert.ToInt32(viewLiquidacion.GetFocusedDataRow()["anomes"]);

            //            Documento d = new Documento(contrato, periodo);
            //            XtraReport rpt = new XtraReport();
            //            rpt = d.SoloHaberes();
            //            d.ShowDocument(rpt);
            //        }
            //    }
            //}
        }

        private void viewLiquidacion_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            DXPopupMenu menu = e.Menu;     

            if (menu != null)
            {
                //CREAMOS SUBMENU
                DXMenuItem submenu = new DXMenuItem("Ver liquidacion", new EventHandler(ShowLiquidacion_Click));
                DXMenuItem MenuPrint = new DXMenuItem("Imprimir", new EventHandler(Imprimir_Click));               
                submenu.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("images/actions/show_16x16.png");
                MenuPrint.ImageOptions.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("images/print/print_16x16.png");
                menu.Items.Clear();
                menu.Items.Add(submenu);
                menu.Items.Add(MenuPrint);                
            }
        }

        private void ShowLiquidacion_Click(object sender, EventArgs e)
        {
            if (viewLiquidacion.RowCount > 0)
            {
                if (objeto.ValidaAcceso(User.GetUserGroup(), "frmdochis") == false)
                { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                //NECESITAMOS EL PERIODO Y EL NUMERO DE CONTRATO ASOCIADO
                string contrato = "";
                int periodo = 0;
                contrato = viewLiquidacion.GetFocusedDataRow()["contrato"].ToString();
                periodo = Convert.ToInt32(viewLiquidacion.GetFocusedDataRow()["anomes"]);

                frmPrevLiquidacion prev = new frmPrevLiquidacion(contrato, periodo);
                prev.ShowDialog();
            }
        }

        private void Imprimir_Click(object sender, EventArgs e)
        {
            ImprimeDocuIndividual();
        }

        private void gridLiquidacion_Click(object sender, EventArgs e)
        {

        }

        private void btnPdf_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (objeto.ValidaAcceso(User.GetUserGroup(), "rptliqhistorico") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            int desde = 0, hasta = 0;
            if (cbRango.Checked)
            {
                desde = Convert.ToInt32(txtDesde.EditValue);
                hasta = Convert.ToInt32(txtHasta.EditValue);

                DialogResult pregunta = XtraMessageBox.Show("¿Seguro deseas generar documento con liquidaciones entre los periodos " + desde + " y " + hasta + "?", "Informacion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (pregunta == DialogResult.Yes)
                {
                    ImprimeDocu(desde, hasta, false, true);
                }
            }
            else
            {
                DialogResult pregunta = XtraMessageBox.Show("¿Seguro deseas generar documento con todas las liquidaciones de este trabajador?", "Informacion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (pregunta == DialogResult.Yes)
                {
                    ImprimeDocu(0, 0, false, true);
                }
            }
        }

        private void gridLiquidacion_DragDrop(object sender, DragEventArgs e)
        {

        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
         
        }

        private void btnMail_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmenviocorreo") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta función", "Acceso", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            frmEnvioCorreo correo = new frmEnvioCorreo();
            correo.RutTrabajador = Rut;
            correo.StartPosition = FormStartPosition.CenterParent;
            correo.ShowDialog();


            /*if (Liquidaciones.Count > 0)
            {
                val = Persona.GeneraPdfLiquidaciones(Liquidaciones, true);
            }
            else
            {
                XtraMessageBox.Show("No se encontraron liquidaciones", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }*/
        }

        private void btnZipLiquidaciones_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (objeto.ValidaAcceso(User.GetUserGroup(), "rptliqhistorico") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            //List<LiquidacionHistorico> lista = new List<LiquidacionHistorico>();
            //lista = Persona.ListadoLiquidaciones();
            //Persona.GeneraPdfLiquidacionesTodos(lista, true);

            if (Liquidaciones.Count > 0)
            {              
                //GENERAMOS ARCHIVO
                Persona.GeneraPdfLiquidaciones(Liquidaciones, true);              
            }

        }

        private void txtHasta_EditValueChanged(object sender, EventArgs e)
        {

        }
    }
}