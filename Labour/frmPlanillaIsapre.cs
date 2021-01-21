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
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Configuration;

namespace Labour
{
    public partial class frmPlanillaIsapre : DevExpress.XtraEditors.XtraForm, IConjuntosCondicionales
    {
        [DllImport("user32")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32")]
        static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        const int MF_BYCOMMAND = 0;
        const int MF_DISABLED = 2;
        const int SC_CLOSE = 0XF060;

        #region "INTERFAZ CONDICIONAES"
        public void CargarCodigoConjunto(string pConjunto)
        {
            if (pConjunto.Length > 0)
            {
                txtConjunto.Text = pConjunto;
            }
        }

        #endregion

        //LISTA DE DATOS
        List<PlanillaIsapre> lista = new List<PlanillaIsapre>();

        //VALORES PARA TOTALES
        double sumImponibles = 0, sumSalud = 0, sumExcedente = 0, sumCotizacion = 0, sumIsapre = 0;

        //NOMBRE ISAPRE
        string isapreObservacion = "";

        //FILTRO USUARIO
        private string FiltroUsuario { get; set; } = User.GetUserFilter();

        //USUARIO PUEDE VER FICHAS PRIVADAS
        private bool ShowPrivadas { get; set; } = User.ShowPrivadas();

        /// <summary>
        /// Nombre condicion usada.
        /// </summary>
        public string DescripcionCondicion { get; set; } = "";

        /// <summary>
        /// Sql base consulta.
        /// </summary>
        string sqlConsulta = "SELECT contrato FROM trabajador ";

        public frmPlanillaIsapre()
        {
            InitializeComponent();
            txtIsapre.Properties.PopupSizeable = false;
        }

        private void frmPlanillaIsapre_Load(object sender, EventArgs e)
        {
            var sm = GetSystemMenu(Handle, false);
            EnableMenuItem(sm, SC_CLOSE, MF_BYCOMMAND | MF_DISABLED);                      
           
            datoCombobox.spllenaComboBox("SELECT id, nombre FROM isapre WHERE id>1", txtIsapre, "id", "nombre", true);
            datoCombobox.spLlenaPeriodos("SELECT anomes FROM parametro ORDER BY anomes DESC", txtComboPeriodo, "anomes", "anomes", true);
            txtIsapre.ItemIndex = 0;
            btnImpresionRapida.Enabled = false;
            btnImprimir.Enabled = false;
            btnPdf.Enabled = false;

            if (txtComboPeriodo.Properties.DataSource != null)
                txtComboPeriodo.ItemIndex = 0;

            datoCombobox.AgrupaList(txtAgrupacion);
            
        }

        #region "MANEJO DE DATOS"
        //LISTADO CON TODOS LOS CONTRATOS QUE TIENE ISAPRE PARA EL PERIODO SELECCIONADO
        private List<string> ContratosIsapre(int pPeriodo, int pSalud, string pConjunto, string pBusqueda)
        {
            List<string> lista = new List<string>();
            string sql = "";
            string sqlFiltro = "";

            sqlFiltro = Calculo.GetSqlFiltro(FiltroUsuario, pConjunto, ShowPrivadas);

            DescripcionCondicion = Conjunto.GetCondicionReporte(pConjunto, FiltroUsuario);

            if (pBusqueda.Length > 0)
            {
                sql = sqlConsulta + $" WHERE salud=@pSalud AND status=1 AND anomes=@pPeriodo AND (nombre LIKE '%{pBusqueda}%' " +
                    $" OR apepaterno LIKE '%{pBusqueda}%' OR apematerno LIKE '%{pBusqueda}%' OR rut LIKE '%{pBusqueda}%') {sqlFiltro} ORDER BY apepaterno";
            }
            else
            {
                sql = sqlConsulta + $" WHERE salud=@pSalud AND status=1 AND anomes=@pPeriodo {sqlFiltro} ORDER BY apepaterno";
            }            
              
            
            SqlCommand cmd;
            SqlDataReader rd;

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        cmd.Parameters.Add(new SqlParameter("@pPeriodo", pPeriodo));
                        cmd.Parameters.Add(new SqlParameter("@pSalud", pSalud));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //GUARDAMOS DATO EN LISTA
                                lista.Add((string)rd["contrato"]);
                            }
                        }
                        else
                        {
                           // XtraMessageBox.Show("No hay registros");
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

            return lista;
        }

        //EN BASE A LA LISTA DE CONTRATOS DEBEMOS OBTENER LOS VALORES DE SALUD ASOCIADOS
        private void CalculoDatos(List<string> Listado, int pPeriodo)
        {
            double imponible = 0, tope = 0, isapre = 0, salud = 0, imponibleFinal = 0, adicional = 0;
            sumCotizacion = 0; sumExcedente = 0; sumImponibles = 0; sumIsapre = 0; sumSalud = 0;
            bool usaTope = false;
            string IniMov = "", TerMov = "";
            //List<PlanillaIsapre> lista = new List<PlanillaIsapre>();
            lista.Clear();
       
            if (Listado.Count>0 && pPeriodo != 0)
            {
                splashScreenManager1.ShowWaitForm();
                foreach (var elemento in Listado)
                {

                    //DATOS IMPORTANTES...
                    //TOTAL IMPONIBLE --> systimp
                    //TOPE SALUD      --> systopesalud
                    //ISAPRE          --> sysisapre
                    //FONASA          --> sysfonasa

                    //imponible = Math.Round(varSistema.ObtenerValorLista("systimp"));
                    imponible = Math.Round(Calculo.GetValueFromCalculoMensaul(elemento, pPeriodo, "systimp"));
                    tope = Math.Round(Calculo.GetValueFromCalculoMensaul(elemento, pPeriodo, "systopesalud"));
                    //tope = Math.Round(varSistema.ObtenerValorLista("systopesalud"));                 

                    //COTIZACION FINAL DE ISAPRE
                    //isapre = Math.Round(varSistema.ObtenerValorLista("sysisapre"));
                    isapre = Math.Round(Calculo.GetValueFromCalculoMensaul(elemento, pPeriodo, "sysisapre"));

                    //7% SALUD OBLIGATORIO                   

                    if (imponible > tope)
                    {
                        imponibleFinal = tope;
                        usaTope = true;
                        salud = Math.Round(0.07 * tope);
                    }
                    else if (tope > imponible)
                    {
                        imponibleFinal = imponible;
                        usaTope = false;
                        salud = Math.Round(0.07 * imponible);
                    }
                    else
                    {
                        imponibleFinal = imponible;
                        usaTope = false;
                        salud = Math.Round(0.07 * imponible);
                    }

                    if (isapre > salud)
                        adicional = isapre - salud;
                    else if (salud > isapre)
                        adicional = 0;
                    else if (salud == isapre)
                        adicional = 0;

                    //MOVIMIENTOS DE PERSONAL
                    Previred prev = new Previred(elemento, pPeriodo);
                    List<MovimientoPersonal> Movimientos = prev.GetMovimientoPersonal(pPeriodo, elemento);

                    Persona p = new Persona();
                    p = Persona.GetInfo(elemento, pPeriodo);

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
                                AddLista(lista, elemento, fnSistema.fFormatearRut2(fnSistema.fnDesenmascararRut(p.Rut)), p.NombreCompleto, 0,
                                    0, 0, 0, 0,
                                    Convert.ToInt32(Movimientos[i].codMovimiento),
                                    IniMov,
                                    TerMov, 
                                    "",
                                    p.centro, p.sucursal, p.NombreArea, p.Cargo
                                    );
                            }
                            else
                            {
                                //PRIMERA LINEA
                                AddLista(lista, elemento, fnSistema.fFormatearRut2(fnSistema.fnDesenmascararRut(p.Rut)), p.NombreCompleto, imponibleFinal,
                                   salud, adicional, Math.Round(UfIsapre(elemento, pPeriodo)), isapre,
                                   Convert.ToInt32(Movimientos[i].codMovimiento),
                                   IniMov,
                                   TerMov, 
                                   p.Fun,
                                    p.centro, p.sucursal, p.NombreArea, p.Cargo
                                   );
                            }
                        }
                    }
                    else
                    {
                        AddLista(lista, elemento, fnSistema.fFormatearRut2(fnSistema.fnDesenmascararRut(p.Rut)), p.ApellidoNombre, imponibleFinal,
                                salud, adicional, Math.Round(UfIsapre(elemento, pPeriodo)), isapre,
                                0,"","", p.Fun, p.centro, p.sucursal, p.NombreArea, p.Cargo);
                    }                   
                }

                if (lista.Count > 0)
                {
                    XtraMessageBox.Show("Operación realizada correctamente", $"{lista.Count} registros encontrados", MessageBoxButtons.OK, MessageBoxIcon.Stop);

                    btnImprimir.Enabled = true;
                    btnImpresionRapida.Enabled = true;
                    btnPdf.Enabled = true;
                }
                else
                {
                    XtraMessageBox.Show("No se encontró información", "Sin información", MessageBoxButtons.OK, MessageBoxIcon.Stop);

                    btnImprimir.Enabled = false;
                    btnImpresionRapida.Enabled = false;
                    btnPdf.Enabled = false;
                }
              

                splashScreenManager1.CloseWaitForm();
            }
            else
            {
                XtraMessageBox.Show("No se a podido realizar la operacion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void AddLista(List<PlanillaIsapre> pList, string pContrato, string pRut, string pNombre, double pImponible,
          double pSaludObligatorio, double pExcedente, double pPagoIsapre, double pPagoFinal,
          int pCodMovimiento, string pInicioMov, string pTerminoMov, string pFun, 
          string pCentro, string pSucursal, string pArea, string pCargo
          )
        {
            pList.Add(new PlanillaIsapre()
            {
                Contrato = pContrato,
                Rut = pRut,
                NombreTrabajador = pNombre,
                Imponible = pImponible,
                SaludObligatorio = pSaludObligatorio,
                Exedente = pExcedente,
                PagoIsapre = pPagoIsapre,
                PagoFinal = pPagoFinal,
                CodMovimiento = pCodMovimiento,
                InicioMovimiento = pInicioMov,
                TerminoMovimiento = pTerminoMov,
                Fun = pFun,
                CentroCosto = pCentro,
                Sucursal = pSucursal,
                Area = pArea,
                Cargo = pCargo
            });
        }


        //VALOR EN UF QUE PAGA EL TRABAJADOR
        private double UfIsapre(string contrato, int periodo)
        {
            double valor = 0, uf = 0;
            uf = ValorUf(periodo);
            string sql = "SELECT original FROM calculo WHERE contrato=@contrato AND anomes=@periodo AND item='salud'";
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

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                valor = Convert.ToDouble((decimal)rd["original"]);
                            }
                        }
                        else
                        {
                            valor = 0;
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

            if (valor != 0)
                valor = valor * uf;
            else
                valor = 0;

            return valor;
        }

        //OBTENER VALOR UF PARA EL PERIODO SELECCIONADO
        private double ValorUf(int periodo)
        {
            double uf = 0;
            string sql = "SELECT uf FROM valoresmes WHERE anomes=@periodo";
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

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                uf = Convert.ToDouble((decimal)rd["uf"]);
                            }
                        }
                        else
                        {
                            uf = 0;
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
            return uf;
        }

        //VERIFICAR SI EXISTE EL CONTRATO
        private bool ExisteContrato(string contrato, int periodo, int salud)
        {
            string sql = "SELECT contrato FROM trabajador WHERE contrato=@contrato AND anomes=@periodo AND salud=@salud";
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
                        cmd.Parameters.Add(new SqlParameter("@salud", salud));

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

        //IMPRIME
        private void ImprimeDocumento(int periodo, bool? impresionRapida = false, bool? GeneraPdf = false)
        {
            Empresa emp = new Empresa();
            emp.SetInfo();
            if (lista.Count>0)
            {
                rptPlanillaIsapre reporte = new rptPlanillaIsapre();
                reporte.DataSource = lista;

                string field = "";

                //PARAMETROS
                reporte.Parameters["rutempresa"].Value = fnSistema.fFormatearRut2(emp.Rut);
                reporte.Parameters["empresa"].Value = emp.Razon;
                reporte.Parameters["periodo"].Value = fnSistema.PrimerMayuscula(fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(periodo)));
                reporte.Parameters["registros"].Value = lista.Count;               
                reporte.Parameters["isapre"].Value = isapreObservacion;
                reporte.Parameters["condicion"].Visible = false;
                reporte.Parameters["condicion"].Value = DescripcionCondicion;

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
                        GroupField groupField = new GroupField();
                        field = txtAgrupacion.Text.ToLower();
                        groupField.FieldName = field;
                        reporte.groupHeaderBand1.GroupFields.Add(groupField);

                        XRLabel labelGroup = new XRLabel { ForeColor = System.Drawing.Color.Black, WidthF = 1052, TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft };
                        labelGroup.Font = new Font("Arial", 9, FontStyle.Italic | FontStyle.Bold);
                        labelGroup.Borders = DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom;

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
                if ((bool)impresionRapida)
                    d.PrintDocument(reporte);
                else if ((bool)GeneraPdf)
                    d.ExportToPdf(reporte, $"Planilla{isapreObservacion}_{fnSistema.PrimerMayuscula(fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(periodo)))}");
                else
                    d.ShowDocument(reporte);
          
            }
        }

        private void ImprimeDocumentoExcel(int periodo)
        { 

            Empresa emp = new Empresa();
            emp.SetInfo();
         
            if (lista.Count > 0)
            {
                rptPlanillaIsapre reporte = new rptPlanillaIsapre();
                reporte.DataSource = lista;

                //PARAMETROS
                reporte.Parameters["rutempresa"].Value = fnSistema.fFormatearRut2(emp.Rut);
                reporte.Parameters["empresa"].Value = emp.Razon;
                reporte.Parameters["periodo"].Value = fnSistema.FechaPeriodo(periodo);
                reporte.Parameters["registros"].Value = lista.Count;
                reporte.Parameters["sumcotizaciontotal"].Value = sumCotizacion;
                reporte.Parameters["sumImponible"].Value = sumImponibles;
                reporte.Parameters["sumsalud"].Value = sumSalud;
                reporte.Parameters["sumexcedente"].Value = sumExcedente;
                reporte.Parameters["sumpagoisapre"].Value = sumIsapre;
                reporte.Parameters["isapre"].Value = isapreObservacion;

                foreach (DevExpress.XtraReports.Parameters.Parameter parametro in reporte.Parameters)
                {
                    parametro.Visible = false;
                }

                /*reporte.Parameters["rutempresa"].Visible = false;
                reporte.Parameters["empresa"].Visible = false;
                reporte.Parameters["periodo"].Visible = false;
                reporte.Parameters["registros"].Visible = false;
                reporte.Parameters["sumcotizaciontotal"].Visible = false;
                reporte.Parameters["sumImponible"].Visible = false;
                reporte.Parameters["sumsalud"].Visible = false;
                reporte.Parameters["sumexcedente"].Visible = false;
                reporte.Parameters["sumpagoisapre"].Visible = false;
                reporte.Parameters["isapre"].Visible = false;*/

                Documento d = new Documento("", 0);
                d.CreateExcelDocument(reporte);
            }
        }

        #endregion



        private void cbTodos_CheckedChanged(object sender, EventArgs e)
        {
            if (cbTodos.Checked == true)
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

        private void btnImpresionRapida_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "rptplanillasalud") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (txtComboPeriodo.Properties.DataSource != null)
            {
                if (Calculo.PeriodoValido(Convert.ToInt32(txtComboPeriodo.EditValue)))
                    ImprimeDocumento(Convert.ToInt32(txtComboPeriodo.EditValue), true); 
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

        private void labelControl7_Click(object sender, EventArgs e)
        {

        }

        private void labelControl11_Click(object sender, EventArgs e)
        {

        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            Close();
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            if (txtComboPeriodo.Properties.DataSource != null)
            {
                if (Calculo.PeriodoValido(Convert.ToInt32(txtComboPeriodo.EditValue)))
                    ImprimeDocumentoExcel(Convert.ToInt32(txtComboPeriodo.EditValue)); 
            }
        }

        private void txtPeriodo_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtContrato_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void btnConjunto_Click(object sender, EventArgs e)
        {
            FrmFiltroBusqueda filtro = new FrmFiltroBusqueda(true);
            filtro.opener = this;
            filtro.StartPosition = FormStartPosition.CenterParent;
            filtro.ShowDialog();
        }

        private void btnPdf_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "rptplanillasalud") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (txtComboPeriodo.Properties.DataSource != null)
            {
                if (Calculo.PeriodoValido(Convert.ToInt32(txtComboPeriodo.EditValue)))
                    ImprimeDocumento(Convert.ToInt32(txtComboPeriodo.EditValue), false, true); 
            }
        }



        private void frmPlanillaIsapre_Shown(object sender, EventArgs e)
        {
            
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            List<string> Listado = new List<string>();
 
            if (txtComboPeriodo.Properties.DataSource == null) { XtraMessageBox.Show("Periodo no valido", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);txtComboPeriodo.Focus();return;}
            if (Calculo.PeriodoValido(Convert.ToInt32(txtComboPeriodo.EditValue)) == false)
            { XtraMessageBox.Show("Periodo no válido", "rror", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (txtIsapre.Properties.DataSource == null)
            { XtraMessageBox.Show("Isapre seleccionada no es válida", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (cbTodos.Checked == false)
            {
                if (txtIsapre.EditValue == null) { XtraMessageBox.Show("Por favor selecciona una Isapre", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);txtIsapre.Focus();return;}

                if (Conjunto.ExisteConjunto(txtConjunto.Text) == false)
                { XtraMessageBox.Show("Por favor ingrese una condición válida", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                isapreObservacion = txtIsapre.Text;             

                Listado = ContratosIsapre(Convert.ToInt32(txtComboPeriodo.EditValue), Convert.ToInt32(txtIsapre.EditValue), txtConjunto.Text, "");                

                //BUSCAR POR CONTRATO
                //BusquedaContrato(txtContrato.Text, Convert.ToInt32(txtPeriodo.Text), Convert.ToInt32(txtIsapre.EditValue));
                if(Listado.Count == 0)
                { XtraMessageBox.Show("Lamentablemente no se encontraron registros para el periodo seleccionado", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                 CalculoDatos(Listado, Convert.ToInt32(txtComboPeriodo.EditValue));
            }
            else
            {
                if (txtIsapre.EditValue == null) { XtraMessageBox.Show("Por favor selecciona una Isapre", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtIsapre.Focus(); return; }

                isapreObservacion = txtIsapre.Text;                
            
                Listado = ContratosIsapre(Convert.ToInt32(txtComboPeriodo.EditValue), Convert.ToInt32(txtIsapre.EditValue), txtConjunto.Text, "");

                if(Listado.Count == 0) { XtraMessageBox.Show("Lamentablemente no se encontraron registros para el periodo seleccionado", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);return; }

                CalculoDatos(Listado, Convert.ToInt32(txtComboPeriodo.EditValue));
            }
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "rptplanillasalud") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (txtComboPeriodo.Properties.DataSource != null)
            {
                if (Calculo.PeriodoValido(Convert.ToInt32(txtComboPeriodo.EditValue)))
                    ImprimeDocumento(Convert.ToInt32(txtComboPeriodo.EditValue)); 
            }
        }
    }
}