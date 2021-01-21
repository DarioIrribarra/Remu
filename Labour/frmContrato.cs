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
    public partial class frmContrato : DevExpress.XtraEditors.XtraForm
    {
        /// <summary>
        /// Numero de contrato asociado al trabajador.
        /// </summary>
        private string Contrato = "";
        /// <summary>
        /// Periodo de la ficha.
        /// </summary>
        private int Periodo = 0;
        /// <summary>
        /// Representa a un trabajador.
        /// </summary>
        private Persona Trabajador;
        /// <summary>
        /// Nos indica si hay o no un proceso corriendo.
        /// </summary>
        private bool Running = false;
        /// <summary>
        /// Nos permite manipular un control ProgressBarControl
        /// </summary>
        private BarraProgreso barraProc;

        /// <summary>
        /// Delegado para deshabilitar los botones desde otro hilo.
        /// </summary>
        /// <param name="pButton">Control simplebutton</param>
        /// <param name="Status">indica si se habilita o deshabilita el boton.</param>
        private delegate void DisableButton(SimpleButton pButton, bool Status);


        public frmContrato()
        {
            InitializeComponent();
        }

        public frmContrato(string pContrato, int pPeriodo)
        {
            InitializeComponent();
            Contrato = pContrato;
            Periodo = pPeriodo;
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            Close();
        }

        private void frmContrato_Load(object sender, EventArgs e)
        {
            if (Contrato != "" && Periodo != 0)
            {
                Configuracion conf = new Configuracion();
                conf.SetConfiguracion();

                txtPlantilla.Text = conf.RutaPlantillaContrato;

                Trabajador = Persona.GetInfo(Contrato, Periodo);

                if (Trabajador.Contrato != "" && Trabajador.PeriodoPersona != 0)
                {
                    lblNombre.Text = Trabajador.ApellidoNombre;
                    txtContrato.Text = Trabajador.Contrato;

                    barraProc = new BarraProgreso(BarraProceso, 1, false, false, this);
                }
            }
        }

        private void btnPlantilla_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            OpenFileDialog diag = new OpenFileDialog();
            diag.Title = "Selecciona una plantilla";
            diag.Filter = "Word 2007 Documents (*.docx)|*.docx|Word 97-2003 Documents (*.doc)|*.doc";

            if (diag.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(diag.FileName))
                    txtPlantilla.Text = diag.FileName;
                else
                    XtraMessageBox.Show("Archivo no existe", "Archivo", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void btnSalida_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            FolderBrowserDialog brow = new FolderBrowserDialog();

            if (brow.ShowDialog() == DialogResult.OK)
            {
                if (Directory.Exists(brow.SelectedPath))
                    txtSalida.Text = brow.SelectedPath;
                else
                    XtraMessageBox.Show("Directorio no válido", "Directorio", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {          
            Sesion.NuevaActividad();           

            if (Contrato == "")
            { XtraMessageBox.Show("Contrato no válido", "Contrato", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (Periodo == 0 || Calculo.PeriodoValido(Periodo) == false)
            { XtraMessageBox.Show("Periodo no válido", "Periodo", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (Persona.ExisteContrato(Contrato, Periodo) == false)
            { XtraMessageBox.Show("Numero de contrato no válido", "Contrato", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (File.Exists(txtPlantilla.Text) == false)
            { XtraMessageBox.Show("Plantilla no válida", "Plantilla", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtPlantilla.Focus(); return; }

            if (Directory.Exists(txtSalida.Text) == false)
            { XtraMessageBox.Show("Directorio de salida no válido", "Directorio", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtSalida.Focus(); return; }

            ThreadStart dele = new ThreadStart(CrearDocumento);
            Thread hilo = new Thread(dele);
            hilo.Start();            
            
        }

        private void txtContrato_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();            
        }

        private void textEdit1_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void textEdit2_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }
        /// <summary>
        /// Porceso que genera el documento de contrato desde un subproceso.
        /// </summary>
        private void CrearDocumento()
        {
            DataSet ds = new DataSet();
            string FileName = "";
            Running = true;

            DetenerBoton(btnPlantilla, false);
            DetenerBoton(btnSalida, false);
            DetenerBoton(btnCrear, false);

            if (Trabajador != null)
            {
                ds = Persona.GetDataSource(Trabajador.Contrato, Trabajador.PeriodoPersona);

                if (ds == null)
                {
                    XtraMessageBox.Show("No se pudo generar documento", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    Running = false;
                    DetenerBoton(btnPlantilla, true);
                    DetenerBoton(btnSalida, true);
                    DetenerBoton(btnCrear, true);
                    return;
                }

                if (ds.Tables[0].Rows.Count > 0)
                {
                    barraProc.Maximum = ds.Tables[0].Rows.Count;
                    barraProc.ShowControl = true;
                    barraProc.ShowClose();
                    barraProc.Begin();              

                    FileName = txtSalida.Text + "\\Contrato_" + Trabajador.Contrato + ".docx";                   
                    
                    Documento doc = new Documento(Trabajador.Contrato, Trabajador.PeriodoPersona);
                    doc.GeneraContrato(ds, FileName, true, txtPlantilla.Text);                    

                    barraProc.Increase();

                    if (File.Exists(FileName))
                    {
                        DialogResult advertencia = XtraMessageBox.Show($"Archivo {FileName} creado correctamente,\n¿Desea ver documento?", "Archivo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (advertencia == DialogResult.Yes)
                            System.Diagnostics.Process.Start(FileName);

                        DetenerBoton(btnPlantilla, false);
                        DetenerBoton(btnSalida, false);
                        DetenerBoton(btnCrear, false);
                    }
                    else
                    {
                        XtraMessageBox.Show("No se pudo crear documento", "Documento", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        Running = false;                       
                    }

                    barraProc.ShowControl = false;
                    barraProc.ShowClose();
                }
                else
                {
                    XtraMessageBox.Show("No se pudo crear documento", "Documento", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    Running = false;                 
                }
            }
            else
            {
                XtraMessageBox.Show("No se pudo crear documento", "Documento", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Running = false;               
            }

            DetenerBoton(btnPlantilla, true);
            DetenerBoton(btnSalida, true);
            DetenerBoton(btnCrear, true);
        }
        /// <summary>
        /// Genera un archivo con todas las variables para documento word.
        /// </summary>
        private void GetVariables()
        {
            DataSet ds = new DataSet();
            string FileName = "";   

            if (Trabajador != null)
            {
                ds = Persona.GetDataSource(Trabajador.Contrato, Trabajador.PeriodoPersona);

                if (ds == null)
                {
                    XtraMessageBox.Show("No se pudo generar documento", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);                   
                    return;
                }

                if (ds.Tables[0].Rows.Count > 0)
                {
                    FileName = txtSalida.Text + "\\VariableContratoSistema.txt";

                    //RECORREMOS COLUMNAS DATASET
                     DataColumnCollection Columnas = ds.Tables[0].Columns;
                    //Creamos archivo
                    Archivo.CrearArchivo(FileName);

                    string RowFile = "";
                    if (Columnas.Count > 0)
                    {
                        RowFile = "<<<<<<<<< Variables para creacion de documento contrato >>>>>>>>" + Environment.NewLine + 
                                  "<<<<<<<<< Se deben respetar Mayúsculas y Minúsculas >>>>>>>>>>>>" + Environment.NewLine + 
                                  "----------------------------------------------------------------";
                        
                        Archivo.EscribirArchivo(FileName, RowFile);
                        foreach (DataColumn Col in Columnas)
                        {
                            RowFile = Col.ColumnName;
                            Archivo.EscribirArchivo(FileName, RowFile);
                        }

                        if (File.Exists(FileName))
                        {
                            DialogResult advertencia = XtraMessageBox.Show($"Archivo generado en {FileName}, ¿Deseas ver archivo?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (advertencia == DialogResult.Yes)
                                System.Diagnostics.Process.Start(FileName);
                        }
                    }
                }
                else
                {
                    XtraMessageBox.Show("No se pudo crear documento", "Documento", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    Running = false;
                }
            }
            else
            {
                XtraMessageBox.Show("No se pudo crear documento", "Documento", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Running = false;
            }

        }

        private void DetenerBoton(SimpleButton pButton, bool Status)
        {
            if (this.InvokeRequired)
            {
                DisableButton dis = new DisableButton(DetenerBoton);

                //PARAMETROS
                object[] parametros = new object[] { pButton, Status };

                this.Invoke(dis, parametros);
            }
            else
            {
                pButton.Enabled = Status;
            }
        }

        private void btnVariables_Click(object sender, EventArgs e)
        {
            //Generar archivo de texto con variables.
            GetVariables();
        }
    }
}