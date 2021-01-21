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
using DevExpress.XtraReports.UI;
using System.Threading;
using DevExpress.XtraSplashScreen;
using System.Runtime.InteropServices;
using DevExpress.LookAndFeel;
using System.IO;

namespace Labour
{
    public partial class frmCalculoRemuneracion : DevExpress.XtraEditors.XtraForm
    {
        [DllImport("user32")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32")]
        static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        const int MF_BYCOMMAND = 0;
        const int MF_DISABLED = 2;
        const int SC_CLOSED = 0xF060;

        /// <summary>
        /// Barra estado proceso activo...
        /// </summary>
        public IMainChildInterface CambioEstadoOpen { get; set; }

        //PARA SABER LA CANTIDAD DE REGISTROS
        private int CantidadRegistros = 0;

        //PARA GUARDAR EL PERIODO QUE DESEA EVALUAR
        private int PeriodoCalculo = 0;

        //PARA GUARDAR EL LISTADO DE TRABAJADORES (ACORDE A PERIODO SELECCIONADO)
        private List<string> ListaTrabajadores = null;

        //DELEGADO PARA ACCEDER A LA BARRA DE PROGRESO DESDE UN HILO
        delegate void PropiedadesBarraDelegado(int totalElementos);

        //DELEGADO PARA MANIPULAR EL INCREMENTO DE LA BARRA DE PROGRESO
        delegate void AumentoBarraDelegado();

        //DELEGADO PARA MOSTRAR REPORTE
        delegate void ShowReportDelegate(XtraReport reporte);

        //DELEGADO PARA CONTROLAR EL LABEL DEL TIEMPO DE INICO DEL CALCULO DE REMUNERACIONES
        delegate void StartProcess();

        delegate void EndProcess(DateTime InicioProceso);

        //PROPIEDAD PARA OBTENER EL FILTRO DEL USUARIO LOGUEADO
        private string FiltroUsuario { get; set; } = User.GetUserFilter();

        //PARA SABER DESDE DONDE SE INVOCA EL FORM_CLOSING
        private bool Cerrando = false;

        //USUARIO PUEDE VER FICHAS PRIVADAS
        private bool ShowPrivados { get; set; } = User.ShowPrivadas();

        //DELEGADO PARA IR MOSTRANDO EL NOMBRE DEL TRABAJADOR
        delegate void ShowEmploye(string pText);

        /// <summary>
        /// Para mostrar alerta.
        /// </summary>
        private Alerta Al { get; set; }

        public frmCalculoRemuneracion()
        {
            InitializeComponent();
        }

        private void frmCalculoRemuneracion_Load(object sender, EventArgs e)
        {
            var sm = GetSystemMenu(Handle, false);
            EnableMenuItem(sm, SC_CLOSED, MF_BYCOMMAND | MF_DISABLED);
            
            txtPeriodo.ReadOnly = true;
            txtPeriodo.Text = Calculo.PeriodoObservado.ToString();
            txtMesPalabra.Text = fnSistema.PrimerMayuscula(fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(Calculo.PeriodoObservado)));

            //lblperiodoObservado.Text = fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(Calculo.PeriodoObservado));

            //labelControl4.Text = fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(Calculo.PeriodoObservado));

            //OBTENER LA CANTIDAD DE REGISTROS PARA EL PERIODO
            List<string> datos = new List<string>();
            datos = ListadoContratos(Calculo.PeriodoObservado);
            txtRegistros.Text = datos.Count.ToString();

            Al = new Alerta();
            Al.Formulario = this;
            Al.AControl = new DevExpress.XtraBars.Alerter.AlertControl();
        }

        #region "MANEJO DE DATOS"
        //OBTENER TODOS LOS CONTRATOS PARA EL PERIODO ACTUAL (ACTIVOS)
        private List<string> ListadoContratos(int periodo)
        {
            List<string> lista = new List<string>();
            int contador = 0;
            //FORMA ANTIGUA SIN FILTRO USUARIO
            //string sql = "SELECT contrato FROM trabajador WHERE anomes=@periodo";
            string sql = "", condUser = "";

            //SI PROPIEDAD SHOWPRIVADOS ES FALSE, NO TIENE PERMISO PARA VER FICHAS PRIVADAS

            if (FiltroUsuario == "0")
                sql = string.Format("SELECT contrato FROM trabajador WHERE status=1 AND anomes={0} " + (ShowPrivados==false?" AND privado=0":"") + " ORDER BY apepaterno", periodo);
            else
            {
                condUser = Conjunto.GetCondicionFromCode(FiltroUsuario);
                sql = string.Format("SELECT contrato FROM trabajador WHERE status=1 AND anomes={0} AND {1} " + (ShowPrivados==false?" AND privado=0":"") + " ORDER BY apepaterno", periodo, condUser);
            }                

            SqlCommand cmd;
            SqlDataReader rd;

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        //cmd.Parameters.Add(new SqlParameter("@periodo", periodo));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                lista.Add((string)rd["contrato"]);
                                contador++;
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
            CantidadRegistros = contador;

            return lista;
        }

        //OBTENER EL ULTIMO PERIODO DESDE TABLA PARAMETROS
        private int PeriodoEnCurso()
        {
            String sql = "SELECT MAX(anomes) as periodo FROM parametro";
            SqlDataReader rd;
            SqlCommand cmd;
            int periodo = 0;
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
                                periodo = (int)rd["periodo"];
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
            return periodo;
        }

        private void CalculoLiquidaciones()
        {

            bool superaTope = false, HaySobregiro = false;
            Documento ShowDoc = new Documento("", 0);
            //OBTENER LISTA CON TODOS LOS CONTRATOS
            if (PeriodoCalculo != 0 && ListaTrabajadores != null)
            {
                Al.Mensaje = "Calculando...\nEsto puede tardar un poco.";
                Al.ShowMessage();

                Calculo.ChangeStatus(1, "001");
                XtraReport reporteAux = new XtraReport();
                reporteAux.CreateDocument();
                int contador = 0;
                string RutaArchivo = "";

                DateTime comienza = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 
                    DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond);

                if (CambioEstadoOpen != null)
                    CambioEstadoOpen.ChangeStatus("<color=Green>Calculando...</color>");

                ComienzaProceso();

                int cantidad = 0;
                cantidad = ListaTrabajadores.Count;
                PropiedadesBarra(cantidad);

                if (ListaTrabajadores.Count == 0) { Calculo.ChangeStatus(0, "001"); return; }
             
                foreach (var contrato in ListaTrabajadores)
                {                   
                    //LLAMAR A LA CLASE HABERES Y REALIZAMOS CALCULO
                    Haberes haber = new Haberes(contrato, PeriodoCalculo);
                    Persona per = Persona.GetInfo(contrato, PeriodoCalculo);

                    //NOMBRE COMPLETO
                    MostrarTrabajador(per.NombreCompleto + $", {contador + 1} de {ListaTrabajadores.Count}...");

                    haber.CalculoLiquidacion();
                    haber.CalculoLiquidacion();
                    
                    //LLAMAMOS A LA CLASE DOCUMENTO
                    Documento docu = new Documento(contrato, PeriodoCalculo);
                    XtraReport reporte = new XtraReport();
                    //reporte = docu.SoloHaberes();
                    reporte = docu.SoloHaberesAnteriores();

                    reporte.CreateDocument();

                    contador++;

                    if (contador >= 1)
                    {
                        reporteAux.Pages.AddRange(reporte.Pages);
                    }

                    //IR ACTUALIZANDO LA BARRA DE PROGRESO
                    ActualizarProgreso();                  
                }

                TerminaProceso(comienza);

                if (CambioEstadoOpen != null)
                    CambioEstadoOpen.ChangeStatus("");                

                superaTope = Calculo.SuperaTope(PeriodoCalculo);
                HaySobregiro = Calculo.LiqSobreGiro(PeriodoCalculo);

                //GENERAMOS ARCHIVO CON INFORMACION DE SOBREGIROS O TOPES DEL 15%
                if(superaTope || HaySobregiro)
                    RutaArchivo = Calculo.ArchivoTopes(PeriodoCalculo);

                if (HaySobregiro)
                { XtraMessageBox.Show("Se detectaron liquidaciones de sueldo con sobregiro, \nfavor verificar información.", "Importante", MessageBoxButtons.OK, MessageBoxIcon.Warning); }

                if (superaTope)
                { XtraMessageBox.Show("Se detectaron algunos items, cuyo descuento supera \nel tope legal permitido.", "Tope 15% Legal", MessageBoxButtons.OK, MessageBoxIcon.Warning); }

                if (Calculo.SueldoMenorMinimo(PeriodoCalculo))
                { XtraMessageBox.Show("Se detectaron trabajadores cuyo sueldo base es menor al sueldo mínimo.", "Sueldo Mínimo", MessageBoxButtons.OK, MessageBoxIcon.Warning); }

                if (HaySobregiro || superaTope)
                {
                    if (File.Exists(RutaArchivo))
                    {
                        XtraMessageBox.Show($"Se ha generado archivo en {RutaArchivo} con información al respecto.", "Archivo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                Al.Mensaje = "Proceso finalizado.";
                Al.ShowMessage();

                Calculo.ChangeStatus(0, "001");

                DialogResult VerDocu = XtraMessageBox.Show("¿Deseas ver las liquidaciones?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (VerDocu == DialogResult.Yes)
                    //ShowReportFromDelegate(reporteAux);   
                    ShowDoc.ShowDocument(reporteAux);                   

                //List<ItemTope> topes = new List<ItemTope>();
                //Calculo.ItemsTope.Clear();
                //topes = Calculo.ItemsTope;
            }            
        }

        //PROPIEDADES BARRA PROGRESO
        private void PropiedadesBarra(int totalElementos)
        {
            if (this.InvokeRequired)
            {
                PropiedadesBarraDelegado delegado = new PropiedadesBarraDelegado(PropiedadesBarra);

                //PARAMETROS
                object[] parametros = new object[] { totalElementos};

                //INVOCAMOS EL METODO A TRAVÉS DEL MISMO CONTEXTO DEL FORMULARIO
                this.Invoke(delegado, parametros);
            }
            else
            {
                barraProgreso.EditValue = 0;
                barraProgreso.Properties.ShowTitle = true;
                barraProgreso.Properties.Maximum = totalElementos;
                barraProgreso.Properties.Step = 1;
                barraProgreso.Properties.PercentView = true;
                barraProgreso.Properties.ProgressViewStyle = DevExpress.XtraEditors.Controls.ProgressViewStyle.Broken;
            }           
        }        

        //ACTUALIZAR BARRA DE PROGRESO
        private void ActualizarProgreso()
        {
            if (this.InvokeRequired)
            {
                AumentoBarraDelegado delegado = new AumentoBarraDelegado(ActualizarProgreso);

                //PARAMETROS NO TIENE
                this.Invoke(delegado);
            }
            else
            {
                //EN CASO CONTRARIO SE REALIZA EL LLAMADO A LOS CONTROLES
                barraProgreso.PerformStep();
                barraProgreso.Update();
            }
        }

        //MOASTRAR REPORTE  USANDO DELEGEADO
        private void ShowReportFromDelegate(XtraReport reporte)
        {
            if (this.InvokeRequired)
            {
                ShowReportDelegate delegado = new ShowReportDelegate(ShowReportFromDelegate);

                //PARAMETROS
                object[] parametros = new object[] {reporte };

                this.Invoke(delegado, parametros);
            }
            else
            {
                ReportPrintTool print = new ReportPrintTool(reporte);
                UserLookAndFeel look = new UserLookAndFeel(this);
                look.UseDefaultLookAndFeel = false;
                look.SkinName = "Office 2016 Black";

                print.ShowPreview(look);
            }
        }

        //TIEMPO EN QUE COMIENZA EL PROCESO DE CALCULO
        private void ComienzaProceso()
        {
            if (this.InvokeRequired)
            {
                StartProcess delegado = new StartProcess(ComienzaProceso);

                this.Invoke(delegado);
            }
            else
            {
                DateTime comienza = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond);
                lblComienza.Text = "Inicio:" + comienza.ToString("hh:mm:ss:fff");

                btnSalir.Enabled = false;
                btnCalculo.Enabled = false;
            }
        }

        //TIEMPO EN QUE TERMINA EL PROCESO DE CALCULO
        private void TerminaProceso(DateTime InicioProceso)
        {
            if (this.InvokeRequired)
            {
                EndProcess delegado = new EndProcess(TerminaProceso);

                //PARAMETROS
                object[] parametros = new object[] { InicioProceso};

                this.Invoke(delegado, parametros);
            }
            else
            {
                DateTime termina = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond);
                lblTermina.Text = "Termino:" + termina.ToString("hh:mm:ss:fff");

                //CALCULAMOS LA DIFERENCIA EN TIEMPO DESDE EL COMIENZO AL TERMINO DEL PROCESO...
                var diferencia = termina.Subtract(InicioProceso);
                string cadena = diferencia.Hours + " Horas, " + diferencia.Minutes + " Minutos, " + diferencia.Seconds + " Segundos y " + diferencia.Milliseconds + " milisegundos";
                lbltranscurrido.Text = "Transcurrido:" + cadena;

                btnSalir.Enabled = true;
                btnCalculo.Enabled = true;

            }
        }

        //MOSTRAR NOMBRE DEL TRABAJADOR QUE SE ESTÁ PROCESANDO
        private void MostrarTrabajador(string pText)
        {
            if (this.InvokeRequired)
            {
                ShowEmploye emp = new ShowEmploye(MostrarTrabajador);

                //PARAMETROS
                object[] parameters = new object[] { pText };

                this.Invoke(emp, parameters);
            }
            else
            {
                lblName.Visible = true;
                lblName.Text = pText;
            }
        }

        //CALCULO % BARRA DE PROGRESO
        private double PorcentajeProgreso(int cantidadregistros, int porcentaje)
        {
            //OBTENGO EL %
            double valor = 0;
            valor = (porcentaje * cantidadregistros) / 100;

            return valor;
        }

        //VALIDAR QUE EL PERIODO EXISTA Y SEA VALIDO
        private bool PeriodoEsValido(int periodo)
        {
            bool valido = false;
            string sql = "SELECT anomes FROM parametro WHERE anomes=@periodo";
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
                            //EXISTE PERIODO
                            valido = true;
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

            return valido;
        }

   

        //MANIPULAR EL HILO ACTUAL
        private void StopCurrentThread()
        {
            //OBTENEMOS EL HILO ACTUAL
            Thread hilo = Thread.CurrentThread;
            //OBTENEMOS EL NOMBRE DEL HILO
            string ThreadName = hilo.Name;

        }

        //ESTE METODO GENERA UN DATATABLE CON TODAS LOS ITEM ASOCIADOS AL CONTRATO 
        //REPRESENTA A TODOS LOS ITEM PREVIO AL RECALCULO (PARA COMPARAR EN CASO DE CAMBIOS)
        private DataTable GeneraDataTable(string pContrato, int pPeriodo)
        {
            string sql = string.Format("SELECT coditem, valor, valorcalculado FROM " +
                "itemtrabajador WHERE contrato='{0}' AND anomes={1}", pContrato, pPeriodo);

            SqlDataAdapter adapter;
            DataSet ds = new DataSet("items");
            DataTable tabla = new DataTable();

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    adapter = new SqlDataAdapter(sql, fnSistema.sqlConn);
                    //PASAMOS LOS DATOS A TRAVES DE FILL AL DATASET
                    adapter.Fill(ds, "itemtrabajador");
                    tabla = ds.Tables["itemtrabajador"];

                }
                fnSistema.sqlConn.Close();

            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            //RETORNAMOS EL DATATABLE
            return tabla;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            /*if (keyData == (Keys.Alt | Keys.F4))
            {
                return true;
            }*/
            return base.ProcessCmdKey(ref msg, keyData);
        }

        #endregion

        private void btnCalculo_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            //VER SI EL USUARIO ESTA BLOQUEADO 
            if (User.Bloqueado())
            { XtraMessageBox.Show("No puedes realizar modificaciones", "Información", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (objeto.ValidaAcceso(User.GetUserGroup(), "rptcalcremun") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }          

            // ThreadStart delegado = new ThreadStart(test);
            // Thread hilo = new Thread(delegado);
            // hilo.Start();

            if (txtPeriodo.Text == "") { XtraMessageBox.Show("Por favor ingresa un periodo valido", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtPeriodo.Focus(); return; }

            //VALIDAR QUE EL PERIODO EXISTA REALMENTE
            if (PeriodoEsValido(Convert.ToInt32(txtPeriodo.Text)) == false)
            { XtraMessageBox.Show("Periodo ingresado no existe", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtPeriodo.Focus(); return; }

            //SETEAMOS VARIABLE PERIODO CALCULO
            PeriodoCalculo = Convert.ToInt32(txtPeriodo.Text);

            ListaTrabajadores = new List<string>();
            ListaTrabajadores = ListadoContratos(PeriodoCalculo);
            lblEvaluando.Text = "Evaluando: " + fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(PeriodoCalculo));
            lblEvaluando.Appearance.FontStyleDelta = FontStyle.Bold;

            if (ListaTrabajadores.Count == 0)
            {
                XtraMessageBox.Show("No se encontraron registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPeriodo.Focus();
                return;
            }      

            //CORREMOS PROCESO EN OTRO HILO
            ThreadStart delegado = new ThreadStart(CalculoLiquidaciones);
            Thread SubProceso = new Thread(delegado);
            SubProceso.SetApartmentState(ApartmentState.STA);
            SubProceso.Name = "HiloCalculo";
            SubProceso.Start();           

        }

        private void panelControl1_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            
        }

        private void txtRegistros_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void cbActual_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void txtPeriodo_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            Cerrando = true;
            Close();
        }

        private void txtPeriodo_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void btnAbortar_Click(object sender, EventArgs e)
        {
            StopCurrentThread();
        }

        private void frmCalculoRemuneracion_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Cerrando == false)
                e.Cancel = true;
            
        }
    }
}