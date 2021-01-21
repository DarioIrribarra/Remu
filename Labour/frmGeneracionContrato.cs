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
using System.Threading;

namespace Labour
{
    public partial class frmGeneracionContrato : DevExpress.XtraEditors.XtraForm, IConjuntosCondicionales
    {
        public bool Running { get; set; } = false;
        public IConjuntosCondicionales opener { get; set; }

        //LISTADO DE CONTRATO
        public List<string> ListadoContratos { get; set; }

        List<LiquidacionHistorico> ListadoHistorico = new List<LiquidacionHistorico>();

        //PUEDE VER FICHAS PRIVADAS?
        public bool ShowPrivados { get; set; } = User.ShowPrivadas();
        public string FiltroUsuario { get; set; } = User.GetUserFilter();

        public string DirectorioGuardar { get; set; } = "";

        //DISABLE BUTTON
        delegate void DisableButton(SimpleButton pButton, bool Option);

        BarraProgreso barra;

        delegate void WriteLabel(string ptext);

        string sqlConsulta = "SELECT rut, contrato, anomes, mail FROM trabajador ";

        private string RutaPlantilla = "";

        private Configuracion config;
        public frmGeneracionContrato()
        {
            InitializeComponent();
        }

        #region "INTERFAZ"
        public void CargarCodigoConjunto(string code)
        {
            string sql = "";
            txtConjunto.Text = code;
            if (txtPeriodo.Properties.DataSource != null)
            {
                if (Conjunto.ExisteConjunto(code) && Calculo.PeriodoValido(Convert.ToInt32(txtPeriodo.EditValue)))
                {
                    sql = Calculo.GetSqlFiltro(FiltroUsuario, code, ShowPrivados);

                    sql = sqlConsulta + $" WHERE anomes={Convert.ToInt32(txtPeriodo.EditValue)} {sql}";

                    ListadoHistorico = Persona.ListadoLiquidaciones(sql);

                    //ListadoContratos = Calculo.ListadoTrabajadores(GetSql(Convert.ToInt32(txtPeriodo.EditValue)), "contrato");
                    if (ListadoHistorico.Count > 0)
                        txtRegistros.Text = ListadoHistorico.Count.ToString();
                    else
                        txtRegistros.Text = "0";
                }
            }
        }
        #endregion

        #region "DATA"
        private string GetSql(int pPeriodo)
        {
            string sql = "";
            string codConjunto = "";

            //BUSCAMOS TODOS LOS CONTRATOS DEL PERIODO
            if (cbTodos.Checked)
            {
                sql = $"SELECT contrato FROM trabajador WHERE anomes={pPeriodo} {(ShowPrivados == false ? "AND privado=0" : "")}";
            }
            else
            {
                //BUSCAMOS POR CONJUNTO
                codConjunto = Conjunto.GetCondicionFromCode(txtConjunto.Text);
                sql = $"SELECT contrato FROM trabajador WHERE anomes={pPeriodo} {(ShowPrivados == false ? "AND privado=0" : "")} AND {codConjunto}";
            }
            return sql;
        }

        //GENERACION DE CONTRATOS
        private void GeneraContrato()
        {            
            if (ListadoHistorico.Count > 0 && Calculo.PeriodoValido(Convert.ToInt32(txtPeriodo.EditValue)) && Directory.Exists(DirectorioGuardar))
            {
                //SOLO PARA INDICAR QUE HAY UN PROCESO CORRIENDO...
                Running = true;

                int count = 0;

                //DESHABILITAMOS BOTON GENERA CARGA
                EnableButton(btnGenerar, false);
                EnableButton(btnPlantilla, false);
                EnableButton(btnRutaSalida, false);                

                barra.ShowControl = true;
                barra.Maximum = ListadoHistorico.Count;
                barra.Begin();

                foreach (LiquidacionHistorico Elemento in ListadoHistorico)
                {                    
                    //DATASET PARA GENERAR CONTRATO
                    DataSet ds = new DataSet();
                    //DataSet items = new DataSet();
                    ds = Persona.GetDataSource(Elemento.Contrato, Convert.ToInt32(txtPeriodo.EditValue));
                   // items = Persona.SetDataSetColumns(pContrato, Convert.ToInt32(txtPeriodo.EditValue));
                    Persona p = Persona.GetInfo(Elemento.Contrato, Convert.ToInt32(txtPeriodo.EditValue));

                    //SOLO PARA MOSTRAR EL NOMBRE DEL TRABAJADOR EN LA BARRA DE PROGRESO...
                    ShowEmploye(p.NombreCompleto + $", {count + 1} de {ListadoHistorico.Count}...");

                    Documento doc = new Documento(Elemento.Contrato, Convert.ToInt32(txtPeriodo.EditValue));
                    doc.GeneraContrato(ds, DirectorioGuardar + @"\" + $"{Elemento.Contrato}.docx", true, RutaPlantilla != "" ? RutaPlantilla: "");

                    barra.Increase();

                    count++;
                }

                if (Persona.TieneArchivos(DirectorioGuardar))
                {
                   DialogResult pregunta = XtraMessageBox.Show($"Archivos generados correctamente en {DirectorioGuardar} \n¿Deseas ver directorio?", "Archivos", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (pregunta == DialogResult.Yes)
                        System.Diagnostics.Process.Start(DirectorioGuardar);

                }

                ShowEmploye("Proceso terminado.");

                EnableButton(btnGenerar, true);
                EnableButton(btnPlantilla, true);
                EnableButton(btnRutaSalida, true);

                Running = false;

            }
        }
        private void ShowEmploye(string pText)
        {
            if (this.InvokeRequired)
            {
                WriteLabel write = new WriteLabel(ShowEmploye);

                //PARAMETROS
                object[] parameters = new object[] { pText};

                this.Invoke(write, parameters);

            }
            else
            {
                lblName.Visible = true;
                lblName.Text = $"{pText}...";
            }
        }

        private void EnableButton(SimpleButton pButton, bool Option)
        {
            if (this.InvokeRequired)
            {
                DisableButton disable = new DisableButton(EnableButton);

                //PARAMETROS
                object[] parameters = new object[] { pButton, Option };

                this.Invoke(disable, parameters);
            }
            else
            {
                pButton.Enabled = Option;
            }
        }

        #endregion

        private void frmGeneracionContrato_Load(object sender, EventArgs e)
        {
            datoCombobox.spLlenaPeriodos("SELECT anomes FROM parametro ORDER BY anomes DESC", txtPeriodo, "anomes", "anomes", true);
            if (txtPeriodo.Properties.DataSource != null)
                txtPeriodo.ItemIndex = 0;

            cbTodos.Checked = true;
            txtConjunto.Enabled = false;
            txtConjunto.Text = "";
            btnConjunto.Enabled = false;

            barra = new BarraProgreso(BarraContratos, 1, true, true, this);

            config = new Configuracion();
            config.SetConfiguracion();

            txtRutaPlantilla.Text = config.RutaPlantillaContrato;
        }

        private void btnGenerar_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            if (txtPeriodo.Properties.DataSource == null)
            { XtraMessageBox.Show("Periodo no válido", "Periodo", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (Calculo.PeriodoValido(Convert.ToInt32(txtPeriodo.EditValue)) == false)
            { XtraMessageBox.Show("Periodo no válido", "Periodo", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (cbPersonalizada.Checked)
            {
                if (txtRutaPlantilla.Text == "" || File.Exists(txtRutaPlantilla.Text) == false)
                { XtraMessageBox.Show("Por favor ingresa una ruta para la plantilla válida", "Archivo no existe", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
            }

            if (txtRutaSalida.Text == "")
            { XtraMessageBox.Show("Por favor ingresa una ruta de salida válida", "Ruta no existe", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (Directory.Exists(txtRutaSalida.Text) == false)
            { XtraMessageBox.Show("Por favor ingresa un ruta de salida válida", "Ruta no existe", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (cbTodos.Checked)
            {
                //ListadoContratos = Calculo.ListadoTrabajadores(GetSql(Convert.ToInt32(txtPeriodo.EditValue)), "contrato");
                if (ListadoHistorico.Count == 0 || ListadoHistorico == null)
                { XtraMessageBox.Show("No se encontraron registros", "registros", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                DirectorioGuardar = txtRutaSalida.Text;
                ThreadStart delegado = new ThreadStart(GeneraContrato);
                Thread hilo = new Thread(delegado);
                hilo.Start();
            }
            else
            {
                if (txtConjunto.Text == "")
                { XtraMessageBox.Show("Por favor ingresa un filtro válido", "Filtro", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                if (Conjunto.ExisteConjunto(txtConjunto.Text) == false)
                { XtraMessageBox.Show("Filtro ingresado no existe", "Conjunto", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                //ListadoContratos = Calculo.ListadoTrabajadores(GetSql(Convert.ToInt32(txtPeriodo.EditValue)), "contrato");
                if (ListadoHistorico.Count == 0 || ListadoHistorico == null)
                { XtraMessageBox.Show("No se encontraron registros", "registros", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                DirectorioGuardar = txtRutaSalida.Text;
                ThreadStart delegado = new ThreadStart(GeneraContrato);
                Thread hilo = new Thread(delegado);
                hilo.Start();
            }
        }

        private void btnPlantilla_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            string extension = "";
            if (cbPersonalizada.Checked)
            {
                OpenFileDialog OpenFile = new OpenFileDialog();
                OpenFile.Title = "Buscar plantilla";
                //OpenFile.Filter = "Word Documents|*.docx";
                OpenFile.Filter = "Word 97-2003 Documents (*.doc)|*.doc|Word 2007 Documents (*.docx)|*.docx";
                if (OpenFile.ShowDialog() == DialogResult.OK)
                {
                    extension = Path.GetExtension(OpenFile.FileName);
                    if (extension != ".doc" && extension != ".docx")
                    { XtraMessageBox.Show("Archivo no válido", "Archivo", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                    txtRutaPlantilla.Text = OpenFile.FileName;
                    //GUARDAMOS RUTA DE PLANTALLA
                    if (File.Exists(OpenFile.FileName))
                        RutaPlantilla = OpenFile.FileName;
                }
            }
        }

        private void btnRutaSalida_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            FolderBrowserDialog dir = new FolderBrowserDialog();
            
            if (dir.ShowDialog() == DialogResult.OK)
            {
                if (Directory.Exists(dir.SelectedPath) == false)
                { XtraMessageBox.Show("Directorio ingresado no existe", "Directorio", MessageBoxButtons.OK, MessageBoxIcon.Stop);  return; }                    

                txtRutaSalida.Text = dir.SelectedPath;
            }
        }

        private void cbPersonalizada_CheckedChanged(object sender, EventArgs e)
        {
            if (cbPersonalizada.Checked)
            {
                txtRutaPlantilla.Enabled = true;
                txtRutaPlantilla.Text = "";
                txtRutaPlantilla.Focus();
                btnPlantilla.Enabled = true;
            }
            else
            {
                txtRutaPlantilla.Enabled = false;
                txtRutaPlantilla.Text = config.RutaPlantillaContrato;     
                btnPlantilla.Enabled = false;
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
                        sql = Calculo.GetSqlFiltro(FiltroUsuario, "", ShowPrivados);

                        sql = sqlConsulta + $" WHERE anomes={Convert.ToInt32(txtPeriodo.EditValue)} {sql}";

                        ListadoHistorico = Persona.ListadoLiquidaciones(sql);
                        //ListadoContratos = Calculo.ListadoTrabajadores(GetSql(Convert.ToInt32(txtPeriodo.EditValue)), "contrato");
                        if (ListadoHistorico.Count > 0)
                            txtRegistros.Text = ListadoHistorico.Count.ToString();
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

        private void txtPeriodo_EditValueChanged(object sender, EventArgs e)
        {
            string sql = "";
            if (txtPeriodo.Properties.DataSource != null)
            {
                if (Calculo.PeriodoValido(Convert.ToInt32(txtPeriodo.EditValue)))
                {
                    if (cbTodos.Checked)
                    {
                        sql = Calculo.GetSqlFiltro(FiltroUsuario, "", ShowPrivados);
                    }
                    else
                    {
                        sql = Calculo.GetSqlFiltro(FiltroUsuario, txtConjunto.Text, ShowPrivados);
                    }

                    sql = sqlConsulta + $" WHERE anomes={Convert.ToInt32(txtPeriodo.EditValue)} {sql}";

                    ListadoHistorico = Persona.ListadoLiquidaciones(sql);
                    //ListadoContratos = Calculo.ListadoTrabajadores(GetSql(Convert.ToInt32(txtPeriodo.EditValue)), "contrato");
                    if (ListadoHistorico.Count > 0)
                        txtRegistros.Text = ListadoHistorico.Count.ToString();
                    else
                        txtRegistros.Text = "0";
                }
            }
        }

        private void btnConjunto_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            //AL HACER DOBLE CLICK LANZAMOS EL FORMULARIO DE EXPRESION 
            
                FrmFiltroBusqueda filtro = new FrmFiltroBusqueda(true);
                filtro.StartPosition = FormStartPosition.CenterParent;
                filtro.opener = this;
                filtro.ShowDialog();
            
        }

        private void frmGeneracionContrato_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Running)
            {
                XtraMessageBox.Show("No puedes cerrar esta ventana", "Proceso ejecutandose", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                e.Cancel = true;
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            Close();
        }
    }
}