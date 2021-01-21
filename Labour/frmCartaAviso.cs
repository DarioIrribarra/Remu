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
    public partial class frmCartaAviso : DevExpress.XtraEditors.XtraForm, IConjuntosCondicionales
    {

        #region "INTERFAZ PARA CONJUNTOS"
        public void CargarCodigoConjunto(string code)
        {
            txtConjunto.Text = code;

            string sql = "";
            if (txtPeriodo.Properties.DataSource != null)
            {
                //EXISTE PERIODO Y CONJUNTO EXISTE?
                if (Calculo.PeriodoValido(Convert.ToInt32(txtPeriodo.EditValue)) && Conjunto.ExisteConjunto(code))
                {
                  
                    sql = Calculo.GetSqlFiltro(FiltroUsuario, txtConjunto.Text, ShowPrivado);

                    sql = SqlConsultada + $" WHERE anomes={Convert.ToInt32(txtPeriodo.EditValue)} {sql}";

                    LiquidacionHis = Persona.ListadoLiquidaciones(sql);
                    if (LiquidacionHis.Count > 0)
                    {
                        txtRegistros.Text = LiquidacionHis.Count.ToString();
                    }
                    else
                    {
                        txtRegistros.Text = "0";
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// Representa el listado de registro encontrados
        /// </summary>
        List<LiquidacionHistorico> LiquidacionHis = new List<LiquidacionHistorico>();

        /// <summary>
        /// Representa la consulta base para consulta.
        /// </summary>
        string SqlConsultada = "SELECT contrato, rut, anomes, mail FROM trabajador ";

        /// <summary>
        /// Indica si un proceso se está ejecutando
        /// </summary>
        private bool Running = false;

        /// <summary>
        /// Nos indica si el usuario puede ver fichas privadas
        /// </summary>
        public bool ShowPrivado { get; set; } = User.ShowPrivadas();
        /// <summary>
        /// Obtiene el filtro del usuario, si es 0 no tiene filtro.
        /// </summary>
        public string FiltroUsuario { get; set; } = User.GetUserFilter();

        /// <summary>
        /// Listado de contratos.
        /// </summary>
        private List<string> ListadoPersonas = new List<string>();

        /// <summary>
        /// Delegado que nos permite seter la propiedad text de un label desde otro hilo.
        /// </summary>
        /// <param name="pText">Cadena de texto a setear.</param>
        delegate void WriteLabel(string pText);
        /// <summary>
        /// Nos permite ocultar o mostrar un control LabelControl.
        /// </summary>
        /// <param name="pLabel"></param>
        delegate void MostrarLabel(LabelControl pLabel, bool Status);            

        /// <summary>
        /// Para manipular barra de progreso.
        /// </summary>
        BarraProgreso barra;
        public frmCartaAviso()
        {
            InitializeComponent();
        }

        private void panelControl1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            if (Running == false)
                Close();
            else
                XtraMessageBox.Show("Por favor espere mientras termina de ejecutarse el proceso", "Proceso ejecutandose", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            
        }

        private void btnPlantilla_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            OpenFileDialog FileDia = new OpenFileDialog();
            FileDia.Title = "Buscar plantilla";
            FileDia.Filter = "Word 97-2003 Documents (*.doc)|*.doc|Word 2007 Documents (*.docx)|*.docx";
            if (FileDia.ShowDialog() == DialogResult.OK)
            {               

                if (File.Exists(FileDia.FileName))
                    txtPlantilla.Text = FileDia.FileName;
                else
                { XtraMessageBox.Show("Plantilla no válida", "Plantilla", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
            }
            
        }
        private void btnSalida_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            FolderBrowserDialog folder = new FolderBrowserDialog();
            if (folder.ShowDialog() == DialogResult.OK)
            {
                if (Directory.Exists(folder.SelectedPath))
                {
                    txtSalida.Text = folder.SelectedPath;
                }
                else
                {
                    XtraMessageBox.Show("Directorio de salida no válido.", "Directorio", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
            }
        }

        private void frmCartaAviso_Load(object sender, EventArgs e)
        {
            datoCombobox.spLlenaPeriodos("SELECT anomes FROM parametro ORDER BY anomes DESC", txtPeriodo, "anomes", "anomes", true);
            if (txtPeriodo.Properties.DataSource != null)
                txtPeriodo.ItemIndex = 0;

            cbTodos.Checked = true;
            txtConjunto.Text = "";
            txtConjunto.Enabled = false;
            btnConjunto.Enabled = false;
            barra = new BarraProgreso(BarraCartas, 1, true, true, this);

            Configuracion conf = new Configuracion();
            conf.SetConfiguracion();
            txtPlantilla.Text = conf.RutaPlantillaAviso;
        }

        private void btnConjunto_Click(object sender, EventArgs e)
        {
            if (txtConjunto.Enabled)
            {
                FrmFiltroBusqueda busqueda = new FrmFiltroBusqueda(true);
                busqueda.StartPosition = FormStartPosition.CenterParent;
                busqueda.opener = this;
                busqueda.ShowDialog();
            }
        }

        private void cbTodos_CheckedChanged(object sender, EventArgs e)
        {
            if (cbTodos.Checked)
            {
                txtConjunto.Text = "";
                txtConjunto.Enabled = false;
                btnConjunto.Enabled = false;

                string sql = "";
                if (txtPeriodo.Properties.DataSource != null)
                {
                    if (Calculo.PeriodoValido(Convert.ToInt32(txtPeriodo.EditValue)))
                    {
                        sql = Calculo.GetSqlFiltro(FiltroUsuario, "", ShowPrivado);                        

                        sql = SqlConsultada + $" WHERE anomes={Convert.ToInt32(txtPeriodo.EditValue)} {sql}";

                        LiquidacionHis = Persona.ListadoLiquidaciones(sql);
                        if (LiquidacionHis.Count > 0)
                        {
                            txtRegistros.Text = LiquidacionHis.Count.ToString();
                        }
                        else
                        {
                            txtRegistros.Text = "0";
                        }
                    }
                }
            }
            else
            {
                txtConjunto.Text = "";
                txtConjunto.Enabled = true;
                txtConjunto.Focus();
                btnConjunto.Enabled = true;
                txtRegistros.Text = "0";



            }
        }

        private void btnGenerar_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();
            
            if (txtPeriodo.Properties.DataSource == null)
            { XtraMessageBox.Show("Por favor selecciona un periodo", "Periodo", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (Calculo.PeriodoValido(Convert.ToInt32(txtPeriodo.EditValue)) == false)
            { XtraMessageBox.Show("Por favor selecciona un periodo válido", "Periodo", MessageBoxButtons.OK, MessageBoxIcon.Stop);return; }

            if (txtPlantilla.Text.Length == 0 || File.Exists(txtPlantilla.Text) == false)
            { XtraMessageBox.Show("Plantilla no válida", "Plantilla", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtPlantilla.Focus(); return; }

            if (txtSalida.Text.Length == 0 || Directory.Exists(txtSalida.Text) == false)
            { XtraMessageBox.Show("Directorio de salida no válido", "Salida", MessageBoxButtons.OK, MessageBoxIcon.Stop);txtSalida.Focus(); return; }

            if (cbTodos.Checked)
            {
                //OBTENEMOS LISTADO DE CONTRATOS    
                //ListadoPersonas = GetListado(Convert.ToInt32(txtPeriodo.EditValue), "");

                if (LiquidacionHis.Count == 0)
                { XtraMessageBox.Show("No se encontró información", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
            }
            else
            {
                //BUSCAMOS UN GRUPO ESPECIFICO
                if (txtConjunto.Text.Length == 0)
                { XtraMessageBox.Show("Por favor ingresa un conjunto de filtro válido", "Conjunto", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtConjunto.Focus(); return; }

                if (Conjunto.ExisteConjunto(txtConjunto.Text) == false)
                { XtraMessageBox.Show("Conjunto ingresado no existe", "Conjunto", MessageBoxButtons.OK, MessageBoxIcon.Stop);txtConjunto.Focus(); return; }

                //ListadoPersonas = GetListado(Convert.ToInt32(txtPeriodo.EditValue), txtConjunto.Text);

                if (LiquidacionHis.Count == 0)
                { XtraMessageBox.Show("No se encontró información", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
            }

            if (LiquidacionHis.Count > 0)
            {
                //GENERAMOS CARTAS DE AVISO
                ThreadStart delegado = new ThreadStart(GeneraCartas);
                Thread hilo = new Thread(delegado);
                hilo.IsBackground = true;
                hilo.Start();
            }
        }

        /// <summary>
        /// Genera un listado de contratos.
        /// </summary>
        /// <param name="pPeriodo">Periodo consulta.</param>
        /// <param name="pConjunto">Conjunto filtro.</param>
        private List<string> GetListado(int pPeriodo, string pConjunto)
        {
            string sql = "SELECT contrato FROM trabajador WHERE anomes=@pPeriodo ";
            string sqlFiltro = Calculo.GetSqlFiltro(FiltroUsuario, pConjunto, ShowPrivado);
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader rd;
            if (sqlFiltro.Length > 0)
            {
                //AGREGAMOS A LA CONSULTA ORIGINAL LA PARTE DEL FILTRO
                sql = sql + sqlFiltro;
                try
                {
                    cn = fnSistema.OpenConnection();
                    if (cn != null)
                    {
                        using (cn)
                        {
                            using (cmd = new SqlCommand(sql, cn))
                            {
                                //PARAMETROS
                                cmd.Parameters.Add(new SqlParameter("@pPeriodo", pPeriodo));

                                rd = cmd.ExecuteReader();
                                if (rd.HasRows)
                                {
                                    while (rd.Read())
                                    {
                                        //LLENAMOS LISTADO
                                        ListadoPersonas.Add(rd["contrato"].ToString());
                                    }
                                }
                                cmd.Dispose();
                                rd.Close();
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                  //ERROR...
                }
            }

            return ListadoPersonas;
            
        }
        /// <summary>
        /// Proceso que genera cartas de aviso para un conjunto de personas.
        /// </summary>
        private void GeneraCartas()
        {
            if (LiquidacionHis.Count > 0)
            {
                Running = true;
                Documento doc = new Documento("", 0);
                int count = 0;
                string FileName = "";
                DataSet Data = new DataSet();
                FileName = txtSalida.Text;

                ShowCloseLabel(lblProgress, true);
                barra.Maximum = LiquidacionHis.Count;
                barra.ShowControl = true;
                barra.Begin();

                //RECORREMOS LISTADO DE CONTRATOS
                foreach (LiquidacionHistorico elemento in LiquidacionHis)
                {
                    Persona per = new Persona();
                    per = Persona.GetInfo(elemento.Contrato, elemento.Periodo);
                    //OBTENESMOS DATASET
                    Data = Persona.GetDataAviso(elemento.Contrato, elemento.Periodo);
                    //MOSTRAMOS NOMBRE DEL TRABAJADOR
                    ShowEmploye($"{per.NombreCompleto} {count+1} de {LiquidacionHis.Count}...");

                    if (Data.Tables[0].Rows.Count > 0)
                    {
                        //GENERAMOS CARTA DE AVISO.
                        doc.GeneraCartaAviso(Data, txtPlantilla.Text, FileName + @"\" + elemento.Contrato + ".docx");
                    }

                    count++;
                    barra.Increase();
                }

                ShowEmploye("Proceso terminado.");
                Running = false;

                if (Directory.Exists(txtSalida.Text) && Persona.TieneArchivos(txtSalida.Text))
                {
                    DialogResult Pregunta = XtraMessageBox.Show($"Archivos generado en {txtSalida.Text} correctamente, \n¿Desea ver directorio?", "Archivos", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (Pregunta == DialogResult.Yes)
                        System.Diagnostics.Process.Start(txtSalida.Text);
                }
                else
                {
                    XtraMessageBox.Show("No se pudo generar archivos", "Archivos", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
                
            }
        }
        /// <summary>
        /// Nos permite mostrar una cadena encima de la barra de proceso.
        /// </summary>
        /// <param name="pText"></param>
        private void ShowEmploye(string pText)
        {
            if (this.InvokeRequired)
            {
                WriteLabel label = new WriteLabel(ShowEmploye);
                //PARAMETROS
                object[] parameters = new object[] { pText};

                this.Invoke(label, parameters);
            }
            else
            {
                lblProgress.Text = pText;
            }
        }

        private void ShowCloseLabel(LabelControl pLabel, bool Status)
        {
            if (this.InvokeRequired)
            {
                MostrarLabel mos = new MostrarLabel(ShowCloseLabel);
                //PARAMETROS
                object[] parameters = new object[] { pLabel, Status};

                this.Invoke(mos, parameters);
            }
            else
            {
                pLabel.Visible = Status;
            }
        }

        private void frmCartaAviso_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Running)
            {
                XtraMessageBox.Show("Por favor espere mientras termina de ejecutarse el proceso en segundo plano", "Proceso ejecutandose", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                e.Cancel = true;
            }
        }

        private void txtPlantilla_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtSalida_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtConjunto_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtPeriodo_EditValueChanged(object sender, EventArgs e)
        {
            string sql = "";
            if (txtPeriodo.Properties.DataSource != null)
            {
                if (Calculo.PeriodoValido(Convert.ToInt32(txtPeriodo.EditValue)))
                {
                    if (cbTodos.Checked)
                    {
                        sql = Calculo.GetSqlFiltro(FiltroUsuario, "", ShowPrivado);
                    }
                    else
                    {
                        sql = Calculo.GetSqlFiltro(FiltroUsuario, txtConjunto.Text, ShowPrivado);
                    }

                    sql = SqlConsultada + $" WHERE anomes={Convert.ToInt32(txtPeriodo.EditValue)} {sql}";

                    LiquidacionHis = Persona.ListadoLiquidaciones(sql);
                    if (LiquidacionHis.Count > 0)
                    {
                        txtRegistros.Text = LiquidacionHis.Count.ToString();
                    }
                    else
                    {
                        txtRegistros.Text = "0";
                    }
                }
            }
        }
    }
}