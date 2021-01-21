using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.XtraReports.UI;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Threading;

namespace Labour
{
    public partial class FrmPlanillaSueldos : DevExpress.XtraEditors.XtraForm, IConjuntosCondicionales
    {
        public IConjuntosCondicionales opener { get; set; }
        public List<string> pListado { get; set; }
        List<LiquidacionHistorico> ListadoLiq = new List<LiquidacionHistorico>();

        public string FiltroUsuario { get; set; } = User.GetUserFilter();

        //PARA SABER SI PUEDE VER FICHAS PRIVADAS
        public bool privados { get; set; } = User.ShowPrivadas();

        public IMainChildInterface CambioEstadoOpen { get; set; }

        //BARRA PROGRESO
        BarraProgreso Barra;

        //DISABLE BUTTON
        delegate void DisableButton(SimpleButton pButton, bool Option);

        /// <summary>
        /// Para mostrar alerta.
        /// </summary>
        private Alerta Al { get; set; }

        string SqlConsulta = "SELECT contrato, rut, anomes, mail FROM trabajador ";

        #region "INTERFAZ"
        public void CargarCodigoConjunto(string code)
        {
            string sql = "";
            
            txtConjunto.Text = code;
            if (txtPeriodo.Properties.DataSource != null)
            {                
                if (Conjunto.ExisteConjunto(code) && Calculo.PeriodoValido(Convert.ToInt32(txtPeriodo.EditValue)))
                {
                    sql = Calculo.GetSqlFiltro(FiltroUsuario, code, privados);

                    sql = SqlConsulta + $" WHERE anomes={Convert.ToInt32(txtPeriodo.EditValue)} {sql}";
                    //SqlConsulta = SqlConsulta + $" WHERE anomes={Convert.ToInt32(txtPeriodo.EditValue)} {sql}";

                    ListadoLiq = Persona.ListadoLiquidaciones(sql);

                    if (ListadoLiq.Count > 0)
                        txtRegistros.Text = ListadoLiq.Count.ToString();
                    else
                        txtRegistros.Text = "0";

                }
            }   
        }
        #endregion

        #region "DATA"
        //GENERA SQL DINAMICA
        private string GetSql(int pPeriodo)
        {
            string sql = "";
            string codConjunto = "";

            //BUSCAMOS TODOS LOS CONTRATOS DEL PERIODO
            if (cbTodos.Checked)
            {
                sql = $"SELECT contrato FROM trabajador WHERE anomes={pPeriodo} {(privados == false ? "AND privado=0":"")}" + " ORDER BY apepaterno";
            }
            else
            {
                //BUSCAMOS POR CONJUNTO
                codConjunto = Conjunto.GetCondicionFromCode(txtConjunto.Text);
                sql = $"SELECT contrato FROM trabajador WHERE anomes={pPeriodo} {(privados == false ? "AND privado=0" : "")} AND {codConjunto}" + " ORDER BY apepaterno";
            }

            return sql;
        }

        //GENERAR LIQUIDACIONES EN SEGUNDO PLANO
        private void GenerarLiq()
        {           

            XtraReport reporte = new XtraReport();
            Documento doc = new Documento("", 0);

            Al.Mensaje = "Generando...\nEsto puede tardar un momento.";
            Al.ShowMessage();

            if (CambioEstadoOpen != null)
                CambioEstadoOpen.ChangeStatus("<color=Green>Generando liquidaciones...</color>");

            EnableButton(btnConjunto, false);
            EnableButton(btnGenerar, false);

            //reporte = Calculo.GeneraLiquidaciones(Convert.ToInt32(txtPeriodo.EditValue), pListado, Barra, lblName, this);
            reporte = Calculo.GeneraReporteLiquidaciones(ListadoLiq, Barra, lblName, this);
            
            if (reporte == null)
            {
                Al.Mensaje = "Proceso terminado con errores.";
                Al.ShowMessage();

                XtraMessageBox.Show("No se pudo generar reporte", "Reporte", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                EnableButton(btnGenerar, true);
                EnableButton(btnConjunto, true);

                return;
            }

            if (CambioEstadoOpen != null)
                CambioEstadoOpen.ChangeStatus("");

            EnableButton(btnGenerar, true);
            EnableButton(btnConjunto, true);

            Al.Mensaje = $"Proceso terminado.";
            Al.ShowMessage();

            DialogResult Pregunta = XtraMessageBox.Show("Liquidaciones generadas correctamente, ¿Deseas abrir?", "Liquidaciones", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if(Pregunta == DialogResult.Yes)
                doc.ShowDocument(reporte);
        }

        //DESHABILITAR BOTONES MIENTRAS SE REALIZA LA OPERACION
        private void EnableButton(SimpleButton pButton, bool Option)
        {
            if (this.InvokeRequired)
            {
                DisableButton Disable = new DisableButton(EnableButton);

                //PARAMETROS
                object[] parameters = new object[] { pButton, Option };

                this.Invoke(Disable, parameters);
            }
            else
            {
                pButton.Enabled = Option;
            }
        }
        #endregion
        public FrmPlanillaSueldos()
        {
            InitializeComponent();
        }

        private void FrmPlanillaSueldos_Load(object sender, EventArgs e)
        {
            datoCombobox.spLlenaPeriodos("SELECT anomes FROM parametro ORDER BY ANOMES desc", txtPeriodo, "anomes", "anomes", true);
          

            txtConjunto.Enabled = false;
            txtConjunto.Text = "";
            btnConjunto.Enabled = false;           

            //INICIALIZAMOS BARRA DE PROGRESO
            Barra = new BarraProgreso(barLiquidaciones, 1, true, true, this);

            Al = new Alerta();
            Al.AControl = new DevExpress.XtraBars.Alerter.AlertControl();
            Al.Formulario = this;
        }

        private void btnGenerar_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            XtraReport reporte = new XtraReport();
            Documento doc = new Documento("", 0);

            if (txtPeriodo.Properties.DataSource == null)
            { XtraMessageBox.Show("Periodo no valido", "Periodo", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (Calculo.PeriodoValido(Convert.ToInt32(txtPeriodo.EditValue)) == false)
            { XtraMessageBox.Show("Periodo no valido", "Periodo", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }            

            if (cbTodos.Checked)
            {              
                if (ListadoLiq.Count == 0)
                { XtraMessageBox.Show("No se encontraron registros", "Registros", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                if (ListadoLiq.Count > 0)
                {
                    

                    ThreadStart delegado = new ThreadStart(GenerarLiq);
                    Thread Hilo = new Thread(delegado);
                    Hilo.SetApartmentState(System.Threading.ApartmentState.STA);
                    Hilo.Name = "Liquidacion";
                    Hilo.Start();               
                }                
            }
            else
            {
                if (Conjunto.ExisteConjunto(txtConjunto.Text) == false)
                { XtraMessageBox.Show("Conjunto ingresado no existe", "Conjunto", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                if (ListadoLiq.Count == 0)
                { XtraMessageBox.Show("No se encontraron registros", "Registros", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                if (ListadoLiq.Count > 0)
                {
                  

                    ThreadStart delegado = new ThreadStart(GenerarLiq);
                    Thread Hilo = new Thread(delegado);
                    Hilo.SetApartmentState(System.Threading.ApartmentState.STA);
                    Hilo.Name = "Liquidacion";
                    Hilo.Start();
                }

            }
        }

        private void cbTodos_CheckedChanged(object sender, EventArgs e)
        {
            string sql = "";
            if (cbTodos.Checked)
            {
                txtConjunto.Enabled = false;
                txtConjunto.Text = "";
                btnConjunto.Enabled = false;

                if (txtPeriodo.Properties.DataSource != null)
                {
                    if (Calculo.PeriodoValido(Convert.ToInt32(txtPeriodo.EditValue)))
                    {
                        //pListado = Calculo.ListadoTrabajadores(GetSql(Convert.ToInt32(txtPeriodo.EditValue)), "contrato");
                        sql = Calculo.GetSqlFiltro(FiltroUsuario, "", privados);

                        sql = SqlConsulta + $" WHERE anomes={Convert.ToInt32(txtPeriodo.EditValue)} {sql}";

                        ListadoLiq = Persona.ListadoLiquidaciones(sql);

                        if (ListadoLiq.Count > 0)
                            txtRegistros.Text = ListadoLiq.Count.ToString();
                        else
                            txtRegistros.Text = "0";
                    }
                }
            }
            else
            {
                txtConjunto.Enabled = true;
                txtConjunto.Text = "";
                txtConjunto.Focus();
                btnConjunto.Enabled = true;
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

        private void txtPeriodo_EditValueChanged(object sender, EventArgs e)
        {
            int periodo = 0;
            string sql = "";            

            if (txtPeriodo.Properties.DataSource != null)
            {
                periodo = Convert.ToInt32(txtPeriodo.EditValue);

                if (Calculo.PeriodoValido(periodo))
                {
                    //pListado = Calculo.ListadoTrabajadores(GetSql(periodo), "contrato");

                    if (cbTodos.Checked)
                    {
                        sql = Calculo.GetSqlFiltro(FiltroUsuario, "", privados);
                    }
                    else
                    {
                        sql = Calculo.GetSqlFiltro(FiltroUsuario, txtConjunto.Text, privados);
                    }

                    sql = SqlConsulta + $" WHERE anomes={Convert.ToInt32(txtPeriodo.EditValue)} {sql}";

                    ListadoLiq = Persona.ListadoLiquidaciones(sql);

                    if (ListadoLiq.Count > 0)
                        txtRegistros.Text = ListadoLiq.Count.ToString();
                    else
                        txtRegistros.Text = "0";
                }
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}