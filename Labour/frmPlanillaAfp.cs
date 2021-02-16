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
using System.Collections;
using System.Runtime.InteropServices;
using DevExpress.XtraReports.Configuration;
using DevExpress.XtraReports.UI;
using System.IO;

namespace Labour
{
    public partial class frmPlanillaAfp : DevExpress.XtraEditors.XtraForm, IConjuntosCondicionales
    {
        [DllImport("user32")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32")]
        static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        const int MF_BYCOMMAND = 0;
        const int MF_DISABLED = 2;
        const int SC_CLOSE = 0XF060;

        /// <summary>
        /// Para guardar nombre de condicion usada (si aplica).
        /// </summary>
        public string DescripcionCondicion { get; set; } = "";

        #region "INTERFAZ"
        public void CargarCodigoConjunto(string pCodigo)
        {
            if (pCodigo.Length > 0)
            {
                txtConjunto.Text = pCodigo;
            }
        }
        #endregion

        //LISTA PARA DATA
        List<PlanillaAfp> lista = new List<PlanillaAfp>();

        //PARA GUARDAR LA PREVISION EN CONSULTA
        string AfpConsulta = "";

        //PARA SUMATORIAS
        double sumImponibles = 0, sumSis = 0, sumCotizacion = 0, sumSegTrab = 0, sumSegEmp = 0, sumAhorro = 0;

        //PERIODO
        int periodoObservado = 0;

        //GUARDAR PERFIL USUARIO LOGIN
        private string FiltroUsuario { get; set; } = User.GetUserFilter();

        //USUARIO PUEDE VER FICHAS PRIVADAS
        private bool ShowPrivados { get; set; } = User.ShowPrivadas();

        /// <summary>
        /// Consulta afp todos los contratos.
        /// </summary>
        string sqlConsulta = "SELECT contrato FROM trabajador ";

        public frmPlanillaAfp()
        {
            InitializeComponent();
            txtAfp.Properties.PopupSizeable = false;
        }

        private void frmPlanillaAfp_Load(object sender, EventArgs e)
        {
            var sm = GetSystemMenu(Handle, false);
            EnableMenuItem(sm, SC_CLOSE, MF_BYCOMMAND | MF_DISABLED);

            datoCombobox.spLlenaPeriodos("SELECT anomes FROM parametro ORDER BY anomes DESC", txtComboPeriodo, "anomes", "anomes", true);
            datoCombobox.spllenaComboBox("SELECT id, nombre FROM afp", txtAfp, "id", "nombre", true);
            txtAfp.ItemIndex = 0;          
            
            btnImpresionRapida.Enabled = false;
            btnImprimir.Enabled = false;
            btnEditarReporte.Enabled = false;
            btnPdf.Enabled = false;

            if (txtComboPeriodo.Properties.DataSource != null)
            {
                txtComboPeriodo.ItemIndex = 0;
            }

            datoCombobox.AgrupaList(txtAgrupacion);
            
        }

        #region "Manejo de Datos"


        //OBTENER LISTA CON TODOS LOS CONTRATOS QUE TIENEN LA MISMA AFP
        private List<string> ListadoContratos(int pPrevision, int pPeriodo, string pConjunto, string pBusqueda)
        {
            List<string> lista = new List<string>();
            string sql = "";
            string sqlFiltro = "";

            sqlFiltro = Calculo.GetSqlFiltro(FiltroUsuario, pConjunto, ShowPrivados);

            DescripcionCondicion = Conjunto.GetCondicionReporte(pConjunto, FiltroUsuario);

            if (pBusqueda.Length > 0)
            {
                sql = sqlConsulta + $" WHERE afp={pPrevision} AND status=1 AND anomes={pPeriodo} " +
                    $"AND (nombre LIKE '%{pBusqueda}%' OR apepaterno LIKE '%{pBusqueda}%' " +
                    $"OR apematerno LIKE '%{pBusqueda}%' OR contrato LIKE '%{pBusqueda}%') {sqlFiltro} \n ORDER BY apepaterno";
            }
            else
            {
                sql = sqlConsulta + $" WHERE afp={pPrevision} AND status=1 AND anomes={pPeriodo}  {sqlFiltro} \n ORDER BY apepaterno";
            }            

            SqlCommand cmd;
            SqlDataReader rd;
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
                                //AGREGAR DATOS A LISTA                              
                                lista.Add((string)rd["contrato"]);
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

            //RETORNAMOS LA LISTA DE CONTRATOS QUE TIENEN LA PREVISION EN CUESTION
            return lista;
        }

        //CARGAR DATOS EN GRILLA DE ACUERDO A BUSQUEDA
        //MOSTRAR TODO LOS DATOS DE ACUERDO A AFP SELECCIONADA
        private void DataCalculo(List<string> Contratos, int pPeriodo)
        {
            double TopeAfp = 0, imponible = 0, imp = 0, sysfscese = 0, syscicese = 0, syscicest = 0, syssis = 0, sysciafp = 0, syscomafp = 0, valAhorro = 0;
            lista.Clear();          
            string IniMov = "", TerMov = "";
            //ULTIMO DIA MES EVALUADO
            DateTime UltimoDiaPeriodo = DateTime.Now.Date;

            if (Contratos.Count>0)
            {
                //RECORREMOS CADA CONTRATO Y HACEMOS RECALCULO
                foreach (var elemento in Contratos)
                {
                    Persona per = new Persona();
                    per = Persona.GetInfo(elemento, pPeriodo);                    

                    //VALORES IMPORTANTES
                    //TOPE AFP --> systopeafp
                    //TOTAL IMPONIBLE --> systimp
                    //CUENTA INDIVIDUAL --> sysciafp
                    //COMISION AFP --> syscomafp
                    //SEGURO TRABAJADOR --> syscicese
                    //SEGURO EMPRESA --> sysfscese & sysfscest
                    //SIS --> syssis
                   
                    //TopeAfp = Math.Round(varSistema.ObtenerValorLista("systopeafp"));
                    TopeAfp = Math.Round(Calculo.GetValueFromCalculoMensaul(elemento, pPeriodo, "systopeafp"));
                    imp = Math.Round(Calculo.GetValueFromCalculoMensaul(elemento, pPeriodo, "systimp"));

                    syscicese = Math.Round(Calculo.GetValueFromCalculoMensaul(elemento, pPeriodo, "syscicese"));
                    sysfscese = Math.Round(Calculo.GetValueFromCalculoMensaul(elemento, pPeriodo, "sysfscese"));
                    syscicest = Math.Round(Calculo.GetValueFromCalculoMensaul(elemento, pPeriodo, "syscicest"));
                    syssis = Math.Round(Calculo.GetValueFromCalculoMensaul(elemento, pPeriodo, "syssis"));
                    sysciafp = Math.Round(Calculo.GetValueFromCalculoMensaul(elemento, pPeriodo, "sysciafp"));
                    syscomafp = Math.Round(Calculo.GetValueFromCalculoMensaul(elemento, pPeriodo, "syscomafp"));

                    if (imp > TopeAfp)
                        imponible = TopeAfp;
                    else if (imp < TopeAfp)
                        imponible = Math.Round(imp);
                    else
                        imponible = Math.Round(imp);

                    //OBTENER LOS MOVIMIENTOS DE PERSONAL
                    Previred prev = new Previred(elemento, pPeriodo);
                    List<MovimientoPersonal> Movimientos = prev.GetMovimientoPersonal(pPeriodo, elemento);

                    UltimoDiaPeriodo = fnSistema.UltimoDiaMes(pPeriodo);

                    //PAGADORA DE SUBSIDIO SOLO PARA EL CASO DE QUE EL MOVIMIENTO DE PERSONAL SEA CODIGO 3 (SUBSIDIO)                   
                    
                    if (Movimientos.Count > 0)
                    {
                        for (int i = 0; i < Movimientos.Count; i++)
                        {
                            //SOLO EN ESTOS CASOS MOSTRAMOS FECHA INICIO MOVIMIENTO
                            if (Movimientos[i].codMovimiento == "1" || Movimientos[i].codMovimiento == "3" ||
                                Movimientos[i].codMovimiento == "4" || Movimientos[i].codMovimiento == "5" ||
                                Movimientos[i].codMovimiento == "6" || Movimientos[i].codMovimiento == "7" ||
                                Movimientos[i].codMovimiento == "8" || Movimientos[i].codMovimiento == "11")
                                IniMov = Movimientos[i].inicioMovimiento.ToString("dd-MM-yyyy");

                            //SOLO EN ESTOS CASOS MOSTRAMOS FECHA TERMINO MOVIMIENTO
                            if (Movimientos[i].codMovimiento == "2" || Movimientos[i].codMovimiento == "3" ||
                               Movimientos[i].codMovimiento == "4" || Movimientos[i].codMovimiento == "6" ||
                               Movimientos[i].codMovimiento == "7" ||
                               Movimientos[i].codMovimiento == "11")
                                TerMov = Movimientos[i].terminoMovimiento.ToString("dd-MM-yyyy");

                            //SEGUNDA LINEA
                            if (i > 0)
                            {
                                AddLista(lista, fnSistema.fFormatearRut2(fnSistema.fnDesenmascararRut(per.Rut)), 
                                    per.ApellidoNombre,
                                    0, 0, 0, 0, 0, Convert.ToInt32(Movimientos[i].codMovimiento),
                                    IniMov,
                                    TerMov,
                                    Convert.ToInt32(Movimientos[i].codMovimiento) == 3 ? fnSistema.fFormatearRut2(prev.RutPagadoraSubsidio(pPeriodo, elemento)) : "",
                                    0, per.centro, per.sucursal, per.NombreArea, per.Cargo);                             
                            }
                            else
                            {
                                valAhorro = ItemTrabajador.GetValue("afpaho", elemento, pPeriodo);
                                //PRIMER LINEA
                                AddLista(lista, fnSistema.fFormatearRut2(fnSistema.fnDesenmascararRut(per.Rut)),
                                        per.ApellidoNombre,
                                        imponible, Math.Round(sysfscese + syscicese), Math.Round(syscicest),
                                        syssis, Math.Round(sysciafp + syscomafp), Convert.ToInt32(Movimientos[i].codMovimiento),
                                        IniMov,
                                        TerMov,
                                        Convert.ToInt32(Movimientos[i].codMovimiento) == 3 ? fnSistema.fFormatearRut2(prev.RutPagadoraSubsidio(pPeriodo, elemento)) : "", 
                                        valAhorro, per.centro, per.sucursal, per.NombreArea, per.Cargo);                          

                            }
                        }
                    }
                    else
                    {
                        //NO HAY MOVIMIENTOS
                        valAhorro = ItemTrabajador.GetValue("afpaho", elemento, pPeriodo);

                        AddLista(lista, fnSistema.fFormatearRut2(fnSistema.fnDesenmascararRut(per.Rut)),
                            per.ApellidoNombre,
                            imponible, Math.Round(sysfscese + syscicese), Math.Round(syscicest),
                            syssis, Math.Round(sysciafp + syscomafp), 0,
                            "", "", "", valAhorro, per.centro, per.sucursal, per.NombreArea, per.Cargo);
                    }                 
                }

                if (lista.Count > 0)
                {
                    XtraMessageBox.Show("Busqueda realizada correctamente", $"{lista.Count} registros encontratos", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    btnImprimir.Enabled = true;
                    btnImpresionRapida.Enabled = true;
                    btnEditarReporte.Enabled = true;
                    btnPdf.Enabled = true;
                    periodoObservado = pPeriodo;
                }
                else
                {
                    XtraMessageBox.Show("No se encontró información para la afp consultada", "Sin Información", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    btnImprimir.Enabled = false;
                    btnEditarReporte.Enabled = false;
                    btnImpresionRapida.Enabled = false;
                    btnPdf.Enabled = false;
                }
         
            }
            else
            {
                XtraMessageBox.Show("No hay registros!!!", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);                
            }
        }

        private void AddLista(List<PlanillaAfp> pList, string pRut, string pNombre, double pImponible, 
            double pSegEmpresa, double pSegTrabajador, double pSis, double pCotizacion, 
            int pCodMovimiento, string pInicioMov, string pTerminoMov, string pRutPagadora, double pAhorro,
            string pCentro, string pSucursal, string pArea, string pCargo)
        {
            pList.Add(new PlanillaAfp()
            {
                Rut = pRut,
                Nombre = pNombre,
                Imponible = pImponible,
                SegEmpresa = pSegEmpresa,
                SegTrabajador = pSegTrabajador,
                Sis = pSis,
                Cotizacion = pCotizacion,
                CodMovimiento = pCodMovimiento,
                InicioMovimiento = pInicioMov,
                TerminoMovimiento = pTerminoMov,
                RutPagadoraSubsidio = pRutPagadora,
                CuentaAhorro = pAhorro,
                CentroCosto = pCentro,
                Sucursal = pSucursal,
                Area = pArea,
                Cargo = pCargo
            });
        }

     

        //VALIDAR QUE EXISTE EL CONTRATO CON LA PREVISION SELECCIONADA
        private bool ExisteContrato(int periodo, string contrato, int prevision)
        {
            bool existe = false;
            string sql = "SELECT contrato FROM trabajador WHERE contrato=@contrato AND anomes=@periodo AND afp=@afp";
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@contrato", contrato));
                        cmd.Parameters.Add(new SqlParameter("@periodo", periodo));
                        cmd.Parameters.Add(new SqlParameter("@afp", prevision));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //EXISTE CONTRATO
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

        //IMPRIME DOCUMENTO
        private void ImprimeDocumento(bool? impresionRapida = false, bool? GeneraPdf = false, bool editar = false)
        {
            if (lista.Count>0)
            {

                //rptPlanillaAfp reporte = new rptPlanillaAfp();
                //Reporte externo
                ReportesExternos.rptPlanillaAfp reporte = new ReportesExternos.rptPlanillaAfp();
                reporte.LoadLayoutFromXml(Path.Combine(fnSistema.RutaCarpetaReportesExterno, "rptPlanillaAfp.repx"));
                reporte.DataSource = lista;               
                string field = "";

                //OBTENEMOS VALORES DE EMPRESA
                Empresa emp = new Empresa();
                emp.SetInfo();

                //PARAMETROS                
                reporte.Parameters["empresa"].Value = emp.Razon;
                reporte.Parameters["periodo"].Value = fnSistema.PrimerMayuscula(fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(periodoObservado)));
                reporte.Parameters["afp"].Value = AfpConsulta;
                reporte.Parameters["rutemp"].Value = fnSistema.fFormatearRut2(emp.Rut);             
                reporte.Parameters["condicion"].Value = DescripcionCondicion;

                //PARA QUE NO APARESCA LA VENTANA PIDIENTO EL VALOR PARA EL PARAMETRO
                foreach (DevExpress.XtraReports.Parameters.Parameter parametro in reporte.Parameters)
                {
                    parametro.Visible = false;
                }

                if (txtAgrupacion.Properties.DataSource != null)
                {
                    reporte.Parameters["agrupacion"].Visible = false;
                    reporte.Parameters["agrupacion"].Value = txtAgrupacion.Text;

                    if (txtAgrupacion.EditValue.ToString() != "0")
                    {
                        reporte.groupFooterBand1.Visible = true;

                        reporte.groupHeaderBand1.GroupFields.Clear();
                        GroupField groupField = new GroupField() ;
                        field = txtAgrupacion.Text.ToLower();
                        groupField.FieldName = field;                        
                        reporte.groupHeaderBand1.GroupFields.Add(groupField);

                        XRLabel labelGroup = new XRLabel { ForeColor = System.Drawing.Color.Black, WidthF = 1054, TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft };
                        labelGroup.Font = new Font("Arial", 9, FontStyle.Italic | FontStyle.Bold);
                        labelGroup.Borders = DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Bottom | DevExpress.XtraPrinting.BorderSide.Right;

                        if (Settings.Default.UserDesignerOptions.DataBindingMode == DataBindingMode.Bindings)
                        {
                            labelGroup.DataBindings.Add("Text", null, $"{field}");
                        }
                        else
                        {
                            labelGroup.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", $"[{field}]"));
                        }
                        reporte.groupHeaderBand1.Controls.Add(labelGroup);

                    }
                    else
                        reporte.groupFooterBand1.Visible = false;
                }

                Documento d = new Documento("", 0);
                //reporte.SaveLayoutToXml(Path.Combine(fnSistema.RutaCarpetaReportesExterno, "rptPlanillaAfp.repx"));

                if (editar)
                {
                    splashScreenManager1.ShowWaitForm();
                    //Se le pasa el waitform para que se cierre una vez cargado
                    DiseñadorReportes.MostrarEditorLimitado(reporte, "rptPlanillaAfp.repx", splashScreenManager1);
                }
                else 
                {
                    if ((bool)impresionRapida)
                        d.PrintDocument(reporte);
                    else if ((bool)GeneraPdf)
                        d.ExportToPdf(reporte, $"Planilla{AfpConsulta}_{fnSistema.PrimerMayuscula(fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(periodoObservado)))}");
                    else
                        d.ShowDocument(reporte);
                }

                
            }
        }
   
        #endregion   

        private void cbTodos_CheckedChanged(object sender, EventArgs e)
        {
            if (cbTodos.Checked == true)
            {
                txtConjunto.Enabled = false;
                txtConjunto.Text = "";
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


        private void btnBuscar_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();           

            List<string> Listado = new List<string>();

            if (txtComboPeriodo.Properties.DataSource == null) { XtraMessageBox.Show("Por favor ingresa un periodo valido", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);txtComboPeriodo.Focus();return;}
            if (Calculo.PeriodoValido(Convert.ToInt32(txtComboPeriodo.EditValue)) == false) { XtraMessageBox.Show("Por favor selecciona un periodo válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);return; }
            if (txtAfp.Properties.DataSource == null) { XtraMessageBox.Show("Por favor selecciona una afp", "Afp", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (cbTodos.Checked)
            {
                //BUSCAMOS TODOS LOS REGISTROS
                if (txtAfp.EditValue == null) { XtraMessageBox.Show("Por favor selecciona una afp", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);txtAfp.Focus();return;}
                Listado = ListadoContratos(Convert.ToInt32(txtAfp.EditValue), Convert.ToInt32(txtComboPeriodo.EditValue), "", "");

                //GUARDAR NOMBRE DE PREVISION EN CONSULTA
                AfpConsulta = txtAfp.Text;              

                if (Listado.Count == 0)
                { XtraMessageBox.Show("No se encontraron registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);return;}

                 DataCalculo(Listado, Convert.ToInt32(txtComboPeriodo.EditValue));
            }
            else
            {                
                //BUSCAMOS CONTRATO EN PARTICULAR
                if (txtAfp.EditValue == null) { XtraMessageBox.Show("Por favor selecciona una afp", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtAfp.Focus(); return; }

                if (txtConjunto.Text == "") { XtraMessageBox.Show("Por favor ingresa un conjunto válido", "Conjunto", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtConjunto.Focus(); return; }
                if (Conjunto.ExisteConjunto(txtConjunto.Text) == false)
                { XtraMessageBox.Show("Por favor Ingresa un conjunto válido", "Conjunto", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtConjunto.Focus(); return; }              

                //GUARDAR NOMBRE DE PREVISION EN CONSULTA
                AfpConsulta = txtAfp.Text;

                Listado = ListadoContratos(Convert.ToInt32(txtAfp.EditValue), Convert.ToInt32(txtComboPeriodo.EditValue), txtConjunto.Text, "");                

                if(Listado.Count == 0)
                { XtraMessageBox.Show("No se encontraron registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                DataCalculo(Listado, Convert.ToInt32(txtComboPeriodo.EditValue));         
 
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

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "rptplanillaafp") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            ImprimeDocumento();
        }

        private void btnImpresionRapida_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "rptplanillaafp") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            ImprimeDocumento(true);
        }

        private void frmPlanillaAfp_Shown(object sender, EventArgs e)
        {
          
        }

        private void btnPdf_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "rptplanillaafp") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            ImprimeDocumento(false, true);
        }

        private void btnConjunto_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            FrmFiltroBusqueda filtro = new FrmFiltroBusqueda(true);
            filtro.opener = this;
            filtro.StartPosition = FormStartPosition.CenterParent;
            filtro.ShowDialog();            
        }

        private void txtConjunto_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void btnEditarReporte_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "rptplanillaafp") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            ImprimeDocumento(editar:true);
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            Close();
        }

        private void txtContrato_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtPeriodo_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtAfp_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtContrato_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }
    }
}