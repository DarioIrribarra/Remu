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
using System.Threading;

namespace Labour
{
    public partial class frmFiniquitosMasivos : DevExpress.XtraEditors.XtraForm, IConjuntosCondicionales
    {
        private string FiltroUsuario = User.GetUserFilter();
        private bool Privadas = User.ShowPrivadas();

        private string SqlConsultada = "SELECT anomes, contrato FROM trabajador ";

        /// <summary>
        /// nos indica si los trabajadores tiene modalidad de pago por horas trabajadas.
        /// </summary>
        private bool AplicaHoras = false;

        private List<LiquidacionHistorico> ListadoContratos = new List<LiquidacionHistorico>();

        BarraProgreso barra;

        //DELEGADO PARA IR MOSTRANDO EL NOMBRE DEL TRABAJADOR
        delegate void ShowEmploye(string pText);
        delegate void Hidelbl(LabelControl pLabel, bool Visible);

        private string RutaPlantilla = "";
        private string RutaSalida = "";
        private DateTime FechaFin = DateTime.Now.Date;

        #region "INTERFAZ"
        public void CargarCodigoConjunto(string code)
        {
            txtCondicion.Text = code;           
        }
        #endregion

        public frmFiniquitosMasivos()
        {
            InitializeComponent();
        }

        private void frmFiniquitosMasivos_Load(object sender, EventArgs e)
        {
            datoCombobox.spLlenaPeriodos("SELECT anomes FROM parametro ORDER BY anomes", txtPeriodo, "anomes", "anomes", true);
            dtFecha.DateTime = DateTime.Now.Date;
            Configuracion con = new Configuracion();
            con.SetConfiguracion();
            txtRutaPlantilla.Text = con.RutaPlantillaFiniquito;

            //Para barra de progreso
            barra = new BarraProgreso(BarraCalculo, 1, true, true, this);
        }

        private void btnGenera_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            List<LiquidacionHistorico> Listado = new List<LiquidacionHistorico>();

            if (File.Exists(txtRutaPlantilla.Text) == false)
            { XtraMessageBox.Show("Por favor verifica la ruta de la plantilla", "Plantilla", MessageBoxButtons.OK, MessageBoxIcon.Stop);txtRutaPlantilla.Focus(); return; }

            if (Directory.Exists(txtSalida.Text) == false)
            { XtraMessageBox.Show("Por favor ingresa una ruta de salida válida", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);txtSalida.Focus(); return; }

            AplicaHoras = cbHoras.Checked;           

            if (cbTodos.Checked)
            {
                Listado = GetListado(SqlConsultada, Calculo.GetSqlFiltro(FiltroUsuario, "", Privadas), Convert.ToInt32(txtPeriodo.EditValue));

                if (Listado.Count == 0)
                { XtraMessageBox.Show("No se encontró información", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
            }
            else
            {
                if (Conjunto.ExisteConjunto(txtCondicion.Text) == false)
                { XtraMessageBox.Show("Por favor ingresa una condición válida", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                Listado = GetListado(SqlConsultada, Calculo.GetSqlFiltro(FiltroUsuario, txtCondicion.Text, Privadas), Convert.ToInt32(txtPeriodo.EditValue));

                if (Listado.Count == 0)
                { XtraMessageBox.Show("No se encontró información", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
            }

            ListadoContratos.Clear();
            ListadoContratos = Listado;
            RutaPlantilla = txtRutaPlantilla.Text;
            RutaSalida = txtSalida.Text;
                
            ThreadStart del = new ThreadStart(Calcular);
            Thread hilo = new Thread(del);
            hilo.Name = "Hilo";
            hilo.Start();
            
        }

        private void cbTodos_CheckedChanged(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            if (cbTodos.Checked)
            {
                txtCondicion.Text = "";
                txtCondicion.Enabled = false;
                btnConjunto.Enabled = false;
            }
            else
            {
                txtCondicion.Text = "";
                txtCondicion.Enabled = true;
                txtCondicion.Focus();
                btnConjunto.Enabled = true;
            }
        }

        private void btnPlantilla_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            OpenFileDialog di = new OpenFileDialog();
            di.Filter = "Word 2007 Documents (*.docx)|*.docx|Word 97-2003 Documents (*.doc)|*.doc";
            di.Title = "Selecciona una plantilla";
            if (di.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(di.FileName))
                {
                    txtRutaPlantilla.Text = di.FileName;
                }
            }
        }

        private void btnSalida_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            FolderBrowserDialog di = new FolderBrowserDialog();
            //di.Filter = "Word 2007 Documents (*.docx)|*.docx|Word 97-2003 Documents (*.doc)|*.doc";
            //di.Title = "Guardar en...";            

            if (di.ShowDialog() == DialogResult.OK)
            {
                txtSalida.Text = di.SelectedPath;
            }
        }

        private void btnConjunto_Click(object sender, EventArgs e)
        {
            if (txtCondicion.Enabled)
            {
                FrmFiltroBusqueda busqueda = new FrmFiltroBusqueda(true);
                busqueda.StartPosition = FormStartPosition.CenterParent;
                busqueda.opener = this;
                busqueda.ShowDialog();
            }
        }

        /// <summary>
        /// Retorna listado de contratos de acuerdo a condicion o filtro establecido por el usuario.
        /// </summary>
        /// <param name="PsQL"></param>
        /// <param name="pConjunto"></param>
        /// <param name="pFiltro"></param>
        /// <param name="pPeriodo"></param>
        private List<LiquidacionHistorico> GetListado(string PsQL, string pFiltro, int pPeriodo)
        {

            if (pFiltro == "")
                PsQL = PsQL + " WHERE anomes=@pPeriodo  " + pFiltro + " ORDER BY rut, contrato, apepaterno";
            else
                PsQL = PsQL + $" WHERE anomes=@pPeriodo  {pFiltro}  ORDER BY rut, contrato";

            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader rd;
            List<LiquidacionHistorico> Listado = new List<LiquidacionHistorico>();

            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(PsQL, cn))
                        {
                            //Parametros    
                            cmd.Parameters.Add(new SqlParameter("@pPeriodo", pPeriodo));

                            rd = cmd.ExecuteReader();
                            if (rd.HasRows)
                            {
                                while (rd.Read())
                                {
                                    Listado.Add(new LiquidacionHistorico() { Contrato = (string)rd["contrato"], Periodo = Convert.ToInt32(rd["anomes"])});
                                }
                            }                            
                        }
                        cmd.Dispose();
                    }
                }
            }
            catch (SqlException ex)
            {
                //ERROR
            }

            return Listado;
        }

        private void Calcular()
        {
            DataSet data = new DataSet();
            string FileName = "";
            int count = 0;

            if (ListadoContratos.Count > 0)
            {
                
                barra.Maximum = ListadoContratos.Count;
                barra.ShowControl = true;
                barra.Begin();

                try
                {
                    foreach (LiquidacionHistorico x in ListadoContratos)
                    {
                        
                        Persona per = new Persona();
                        per = Persona.GetInfo(x.Contrato, x.Periodo);

                        if (FechaFin < per.Salida)
                        { XtraMessageBox.Show("Por favor verifica que la fecha de finiquito no sea menor a la fecha de termino de contrato \npara trabajador " + per.ApellidoNombre + " N° " + per.Contrato, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                        MostrarTrabajador(per.ApellidoNombre);
                        Finiquito fin = new Finiquito(per, FechaFin, cbAviso.Checked, cbPrestamo.Checked, cbSeguroCes.Checked ? Convert.ToDouble(txtSeguroCes.Text) : 0, AplicaHoras);
                        if (AplicaHoras)
                            data = fin.CalculaFiniquitoHoras();
                        else
                            data = fin.CalculaFiniquito();

                        if (data == null || data.Tables[0].Rows.Count == 0)
                        {
                            XtraMessageBox.Show("No se encontró información", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            return;
                        }

                        FileName = txtSalida.Text + "\\Finiquito_" + per.Contrato + ".docx";
                        Documento doc = new Documento("", 0);
                        doc.GeneraCartaAviso(data, txtRutaPlantilla.Text, FileName);

                        HideLabel(lblname, true);
                        MostrarTrabajador(per.ApellidoNombre);

                        barra.Increase();
                    }

                    XtraMessageBox.Show("Archivos generados en " + FileName, "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    MostrarTrabajador("100%");
                    DialogResult adv = XtraMessageBox.Show("¿Desea ver directorio?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (adv == DialogResult.Yes)
                        System.Diagnostics.Process.Start(txtSalida.Text);
                }
                catch (Exception ex)
                {
                    //ERROR..
                    XtraMessageBox.Show("no se pudieron generar todos los documentos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }


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
                lblname.Visible = true;
                lblname.Text = pText;
            }
        }

        private void HideLabel(LabelControl pLabel, bool Visible)
        {
            if (this.InvokeRequired)
            {
                Hidelbl emp = new Hidelbl(HideLabel);

                //PARAMETROS
                object[] parameters = new object[] { pLabel, Visible };

                this.Invoke(emp, parameters);
            }
            else
            {
                lblname.Visible = Visible;
                
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            Close();
        }

        private void txtSeguroCes_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {

        }

        private void cbSeguroCes_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSeguroCes.Checked)
            {
                txtSeguroCes.Enabled = true;
                txtSeguroCes.Text = "";
                txtSeguroCes.Focus();
            }
            else
            {
                txtSeguroCes.Enabled = false;
                txtSeguroCes.Text = "";
            }
        }

        private void txtSeguroCes_KeyPress(object sender, KeyPressEventArgs e)
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
    }
}