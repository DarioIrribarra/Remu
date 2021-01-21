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
using System.Runtime.InteropServices;
using System.Data.SqlClient;
using System.Collections;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Configuration;

namespace Labour
{
    public partial class frmPlanillaMutual : DevExpress.XtraEditors.XtraForm, IConjuntosCondicionales
    {
        [DllImport("user32")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32")]
        static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        const int MF_BYCOMMAND = 0;
        const int MF_DISABLED = 2;
        const int SC_CLOSED = 0xF060;

        /// <summary>
        /// Representa la descripcion de la condicion de busqueda utilizada.
        /// </summary>
        public string DescripcionCondicion { get; set; } = "";

        #region "INTERFAZ CONDICIONES"
        public void CargarCodigoConjunto(string pConjunto)
        {
            if (pConjunto.Length > 0)
            {
                txtConjunto.Text = pConjunto;
            }
        }
        #endregion

        //LISTA CLASE PLANILLA MUTUAL
        List<PlanillaMutual> lista = new List<PlanillaMutual>();

        //SUMATORIO TOTAL IMPONIBLE
        private double sumImponible = 0;

        //PARA GUARDAR EL FILTRO USUARIO
        private string FiltroUsuario { get; set; } = User.GetUserFilter();

        //USUARIO PUEDE VER FICHAS PRIVADAS
        private bool ShowPrivados { get; set; } = User.ShowPrivadas();

        /// <summary>
        /// Sql consulta base.
        /// </summary>
        string sqlConsulta = "SELECT contrato FROM trabajador ";

        public frmPlanillaMutual()
        {
            InitializeComponent();
        }

        #region "MANEJO DE DATOS"

        //LISTADO DE CONTRATOS DE ACUERDO A PERIODO PARAMETRO
        private List<string> ListadoContratos(int pPeriodo, string pConjunto, string pBusqueda)
        {
            List<string> lista = new List<string>();
            SqlCommand cmd;
            SqlDataReader rd;
            string sql = "";
            string sqlFiltro = "";

            sqlFiltro = Calculo.GetSqlFiltro(FiltroUsuario, pConjunto, ShowPrivados);

            DescripcionCondicion = Conjunto.GetCondicionReporte(pConjunto, FiltroUsuario);

            if (pBusqueda.Length > 0)
            {
                sql = sqlConsulta + $" WHERE anomes=@pPeriodo AND status=1 AND (nombre LIKE '%{pBusqueda}%' " +
                    $"OR apepaterno LIKE '%{pBusqueda}%' OR apematerno LIKE '%{pBusqueda}%' " +
                    $"OR contrato LIKE '%{pBusqueda}%') {sqlFiltro} ORDER BY apepaterno";
            }
            else
            {
                sql = sqlConsulta + $" WHERE status=1 AND anomes=@pPeriodo  {sqlFiltro} \nOrder by apepaterno";
            }


            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pPeriodo", pPeriodo));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //LLENAMOS LA LIST
                                lista.Add((string)rd["contrato"]);
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
            return lista;
        }

        //LISTA FILTRADA POR COINCIDENCIAS DE CONTRATO
        //PARAMETRO DE ENTRADA: NUMERO DE CONTRATO
        private List<string> ListaContratoFiltrada(string busqueda, int periodo, int option)
        {
            List<string> registros = new List<string>();
            string sqlContrato = "", sqlNombre = "", condUser = "";

            //SI PROPIEDAD SHOWPRIVADOS ES FALSE, EL USUARIO NO PUEDE VER LAS FICHAS PRIVADAS

            if (FiltroUsuario == "0")
                sqlContrato = string.Format("SELECT contrato FROM trabajador WHERE contrato LIKE '%{0}%' AND anomes={1} " + (ShowPrivados==false?" AND privado=0":"") + " ORDER BY apepaterno", busqueda, periodo);
            else
            {
                condUser = Conjunto.GetCondicionFromCode(FiltroUsuario);
                sqlContrato = string.Format("SELECT contrato FROM trabajador WHERE contrato LIKE '%{0}%' AND anomes={1} AND {2} " + (ShowPrivados == false ? " AND privado=0" : "") + " ORDER BY apepaterno", busqueda, periodo, condUser);
            }

            if (FiltroUsuario == "0")
                sqlNombre = string.Format("SELECT contrato FROM trabajador WHERE (apepaterno LIKE '%{0}%' " +
                "OR apematerno LIKE '%{1}%' OR apematerno LIKE '%{2}%' OR nombre LIKE '%{3}%') AND anomes={4} " + (ShowPrivados == false ? " AND privado=0" : "") + " ORDER BY apepaterno", busqueda, busqueda, busqueda, busqueda, periodo);
            else
            {
                condUser = Conjunto.GetCondicionFromCode(FiltroUsuario);
                sqlNombre = string.Format("SELECT contrato FROM trabajador WHERE (apepaterno LIKE '%{0}%' " +
                "OR apematerno LIKE '%{1}%' OR apematerno LIKE '%{2}%' OR nombre LIKE '%{3}%') AND anomes={4} AND {5} " + (ShowPrivados == false ? " AND privado=0" : "") + " ORDER BY apepaterno", busqueda, busqueda, busqueda, busqueda, periodo, condUser);
            }                

            string uso = "";

            //OPTION 1 --> BUQUEDA POR NUMERO DE CONTRATO
            //OPTIN 2 --> BUSQUEDA POR NOMBRE
            if (option == 1)
                uso = sqlContrato;
            else if (option == 2)
                uso = sqlNombre;

            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(uso, fnSistema.sqlConn))
                    {
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                registros.Add((string)rd["contrato"]);
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
            return registros;
        }

        //BUSQUEDA DE TRABAJADORES POR CONTRATO
        private void CargaDatos(List<string> pRegistros, int pPeriodo)
        {
            int periodoActual = 0;
            periodoActual = Calculo.PeriodoObservado;
            double diasTrabajados = 0, imponibleFinal = 0;
            double imp = 0, topeAfp = 0;            
            sumImponible = 0;
          
            //LIMPIAMOS LISTA
            lista.Clear();
            if (pRegistros.Count>0)
            {
                //SI PERIODO COINCIDE CON EL PERIODO EN MARCHA RECALCULAMOS...
                //SI PERIODO ES ANTERIOR PODEMOS CONSULTA LA TABLA HISTORICO...
                splashScreenManager1.ShowWaitForm();
                    //RECALCULAMOS
                    foreach (var elemento in pRegistros)
                    {
                        //Haberes hab = new Haberes(elemento, periodo);
                        //hab.CalculoGenerico();
                        //hab.CalculoGenericoItemTrabajador(null);
                        diasTrabajados = Calculo.GetValueFromCalculoMensaul(elemento, pPeriodo, "sysdiastr");
                        Persona p = new Persona();
                        p = Persona.GetInfo(elemento, pPeriodo);

                        //Hashtable datos = new Hashtable();
                        //datos = NombreCompleto(elemento, periodo);
                        
                        imp = Calculo.GetValueFromCalculoMensaul(elemento, pPeriodo, "systimp");
                        topeAfp = Calculo.GetValueFromCalculoMensaul(elemento, pPeriodo, "systopeafp");
                        
                        //IMPONIBLE
                        if (imp > topeAfp)
                            imponibleFinal = topeAfp;
                        else if (imp < topeAfp)
                            imponibleFinal = imp;
                        else
                            imponibleFinal = imp;

                    lista.Add(new PlanillaMutual()
                    {
                        Dias = diasTrabajados,
                        Nombre = p.ApellidoNombre,
                        Imponible = imponibleFinal,
                        Rut = fnSistema.fFormatearRut2(fnSistema.fnDesenmascararRut(p.Rut)),
                        Nacimiento = p.Nacimiento,
                        Sexo = p.Sexo == 0 ? "Masculino" : "Femenino",
                        Area = p.NombreArea,
                        CentroCosto = p.centro,
                        Cargo = p.Cargo,
                        Sucursal = p.sucursal
                                                 
                        });

                        //sumImponible = sumImponible + imponibleFinal;
                    }

                //AGREGAR NUEVA FILA CON TOTAL
                //  lista.Add(new PlanillaMutual() { Dias = 0, Imponible = sumImponible, Nombre = "", Rut = "", Sexo = ""});

                if (lista.Count > 0)
                {

                    btnImprimir.Enabled = true;
                    btnImpresionRapida.Enabled = true;
                    btnPdf.Enabled = true;
                    XtraMessageBox.Show($"{lista.Count} registros encontrados", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else {
                    btnImprimir.Enabled = false;
                    btnImpresionRapida.Enabled = false;
                    btnPdf.Enabled = false;
                    XtraMessageBox.Show("No se encontró información", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                   

                splashScreenManager1.CloseWaitForm();
            }
            else
            {
                XtraMessageBox.Show("No se encontraron registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }            
        }

        //BUSQUEDA POR NUMERO DE CONTRATO
        private void BusquedaContrato(List<string> contratos, int periodo)
        {
            lista.Clear();
            sumImponible = 0;
           
            double imponibleFinal = 0, diasTrabajados = 0, imp = 0, topeAfp = 0;
            //PERIODO EN EVALUACION (SOLO PARA COMPARAR)
            int periodoActual = 0;
            periodoActual = Calculo.PeriodoObservado;
          
            if (contratos.Count>0)
            {
                //RECORREMOS TODOS LOS CONTRATOS DE LA LISTA
                    splashScreenManager1.ShowWaitForm();
                    foreach (var elemento in contratos)
                    {
                        //RECALCULAMOS
                        //Haberes hab = new Haberes(elemento, periodo);
                        //hab.CalculoGenerico();
                        imp = Calculo.GetValueFromCalculoMensaul(elemento, periodo, "systimp");
                        topeAfp = Calculo.GetValueFromCalculoMensaul(elemento, periodo, "systopeafp");

                        Persona p = new Persona();
                        p = Persona.GetInfo(elemento, periodo);

                        //Hashtable data = new Hashtable();
                        //data = NombreCompleto(elemento, periodo);

                        //IMPONIBLE
                        if (imp > topeAfp)
                            imponibleFinal = topeAfp;
                        else if (imp < topeAfp)
                            imponibleFinal = imp;
                        else
                            imponibleFinal = imp;

                        //DIAS TRABAJADOS                        
                        diasTrabajados = Calculo.GetValueFromCalculoMensaul(elemento, periodo, "sysdiastr");

                        lista.Add(new PlanillaMutual() {
                            Rut = fnSistema.fFormatearRut2(fnSistema.fnDesenmascararRut(p.Rut)),
                            Nombre = p.NombreCompleto,
                            Sexo = p.Sexo == 0? "Masculino":"Femenino",
                            Dias = diasTrabajados,
                            Imponible = imponibleFinal,
                            Nacimiento = p.Nacimiento
                        });

                        //SUMAS IMPONIBLES
                        sumImponible = sumImponible + imponibleFinal;                    
                    }

                    if (lista.Count > 0)
                    {
                       

                        btnImpresionRapida.Enabled = true;
                        btnImprimir.Enabled = true;
                       
                    }

                    splashScreenManager1.CloseWaitForm();
               

            }
            else
            {
                XtraMessageBox.Show("No se encontraron registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
            }

        }     

        //Columnas grilla
    

        //IMPRIMIR DOCUMENTO
        private void ImprimeDocumento(int periodo, bool? impresionRapida = false, bool? GeneraPdf = false)
        {
            //PARA DATOS EMpresa
           // Hashtable dataEmp = new Hashtable();
           // dataEmp = dataEmpresa();

            Empresa emp = new Empresa();
            emp.SetInfo();
            string field = "";
          
            if (lista.Count>0)
            {
                rptPlanillaMutual reporte = new rptPlanillaMutual();
                reporte.DataSource = lista;

                //PARAMETROS
                reporte.Parameters["empresa"].Value = emp.Razon;
                reporte.Parameters["rutempresa"].Value = fnSistema.fFormatearRut2(emp.Rut);
                reporte.Parameters["totalimponible"].Value = sumImponible;
                reporte.Parameters["periodo"].Value = fnSistema.PrimerMayuscula(fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(periodo)));
                reporte.Parameters["condicion"].Value = DescripcionCondicion;

                //FORMA MAS SIMPLE DE OCULTAR LOS PARAMETROS CUANDO SE MOUESTRA EL DOCUMENTO...
                foreach (DevExpress.XtraReports.Parameters.Parameter parametro in reporte.Parameters)
                {
                    parametro.Visible = false;
                }

                /*reporte.Parameters["empresa"].Visible = false;
                reporte.Parameters["rutempresa"].Visible = false;
                reporte.Parameters["totalimponible"].Visible = false;
                reporte.Parameters["periodo"].Visible = false;*/

                if (txtAgrupa.Properties.DataSource != null)
                {
                    reporte.Parameters["agrupacion"].Visible = false;
                    reporte.Parameters["agrupacion"].Value = txtAgrupa.Text;

                    if (txtAgrupa.EditValue.ToString() != "0")
                    {
                        reporte.groupFooterBand1.Visible = true;

                        reporte.groupHeaderBand1.GroupFields.Clear();
                        GroupField groupField = new GroupField();
                        field = txtAgrupa.Text.ToLower();
                        groupField.FieldName = field;
                        reporte.groupHeaderBand1.GroupFields.Add(groupField);

                        XRLabel labelGroup = new XRLabel { ForeColor = System.Drawing.Color.Black, WidthF = 830, TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft };
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

                if ((bool)impresionRapida)
                    d.PrintDocument(reporte);
                else if ((bool)GeneraPdf)
                    d.ExportToPdf(reporte, $"PlanillaMutual_{fnSistema.PrimerMayuscula(fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(periodo)))}");
                else
                    d.ShowDocument(reporte);

            }
        }

        //IMPONIBLE HISTORICO
        private double ImponibleHistorico(string contrato, int periodo)
        {
            double imp = 0;
            string sql = "select imponible from liquidacionHistorico " +
                "WHERE contrato = @contrato AND anomes=@periodo";
            //string p = string.Format("SELECT imponible from liquidacionhistorico WHERE " +
              //  "contrato ='{0}' AND anomes={1}", contrato, periodo);
            //XtraMessageBox.Show(p);
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
                                imp = Convert.ToDouble((decimal)rd["imponible"]);
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

            return imp;
        }

        #endregion

        private void txtContrato_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtPeriodo_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void frmPlanillaMutual_Load(object sender, EventArgs e)
        {
            var sm = GetSystemMenu(Handle, false);
            EnableMenuItem(sm, SC_CLOSED, MF_BYCOMMAND | MF_DISABLED);

            datoCombobox.spLlenaPeriodos("SELECT anomes FROM parametro ORDER BY anomes DESC", txtComboPeriodo, "anomes", "anomes", true);

            btnImpresionRapida.Enabled = false;
            btnImprimir.Enabled = false;
            btnPdf.Enabled = false;
            if (txtComboPeriodo.Properties.DataSource != null)
                txtComboPeriodo.ItemIndex = 0;

            datoCombobox.AgrupaList(txtAgrupa);

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
                txtConjunto.Text = "";
                txtConjunto.Enabled = true;
                txtConjunto.Focus();
                btnConjunto.Enabled = true;                
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            Close();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            List<string> Listado = new List<string>();

            if (txtComboPeriodo.Properties.DataSource == null)
            { XtraMessageBox.Show("Por favor selecciona un periodo válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (Calculo.PeriodoValido(Convert.ToInt32(txtComboPeriodo.EditValue)) == false)
            { XtraMessageBox.Show("Por favor selecciona un periodo válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (cbTodos.Checked)
            {                 
                Listado = ListadoContratos(Convert.ToInt32(txtComboPeriodo.EditValue), "", "");
                
                if (Listado.Count == 0)
                { XtraMessageBox.Show("No se encontraron registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

                CargaDatos(Listado, Convert.ToInt32(txtComboPeriodo.EditValue));
            }
            else
            {
                Listado = ListadoContratos(Convert.ToInt32(txtComboPeriodo.EditValue), txtConjunto.Text, "");

                if (Listado.Count == 0)
                { XtraMessageBox.Show("No se encontraron registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

                CargaDatos(Listado, Convert.ToInt32(txtComboPeriodo.EditValue));
            }        

        }

       



        private void btnImprimir_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "rptplanillamutual") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (txtComboPeriodo.Properties.DataSource != null)
            {
                if (Calculo.PeriodoValido(Convert.ToInt32(txtComboPeriodo.EditValue)))
                {
                    ImprimeDocumento(Convert.ToInt32(txtComboPeriodo.EditValue));  
                }
            }
        }

        private void btnImpresionRapida_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "rptplanillamutual") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (txtComboPeriodo.Properties.DataSource != null)
            {
                if (Calculo.PeriodoValido(Convert.ToInt32(txtComboPeriodo.EditValue)))
                {
                    ImprimeDocumento(Convert.ToInt32(txtComboPeriodo.EditValue), true);  
                }
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

        private void txtPeriodo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) || char.IsNumber(e.KeyChar))
            {
                e.Handled = false;
            }
            else
                e.Handled = true;
        }

        private void btnPdf_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "rptplanillamutual") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (txtComboPeriodo.Properties.DataSource != null)
            {
                if (Calculo.PeriodoValido(Convert.ToInt32(txtComboPeriodo.EditValue)))
                {
                    ImprimeDocumento(Convert.ToInt32(txtComboPeriodo.EditValue), false, true);  
                }
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void btnConjunto_Click(object sender, EventArgs e)
        {
            FrmFiltroBusqueda filtro = new FrmFiltroBusqueda(true);
            filtro.opener = this;
            filtro.StartPosition = FormStartPosition.CenterParent;
            filtro.ShowDialog();
        }
    }
}