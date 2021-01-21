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
using DevExpress.XtraPrinting;
using DevExpress.Export.Xl;

namespace Labour
{
    public partial class frmFiniquito : DevExpress.XtraEditors.XtraForm
    {
        /// <summary>
        /// Numero de contrato asociado a trabajador.
        /// </summary>
        public string Contrato { get; set; }
        /// <summary>
        /// Periodo ficha
        /// </summary>
        public int Periodo { get; set; }
        /// <summary>
        /// Para obtener toda la informacion relacionada al trabajador.
        /// </summary>
        private Persona Trabajador;
        /// <summary>
        /// Nos permite manipular la barra de progreso de este formulario.
        /// </summary>
        private BarraProgreso Barra;
        /// <summary>
        /// Indica si hay o no un proceso corriendo.
        /// </summary>
        private bool Running = false;

        /// <summary>
        /// Nos permite manipular las propiedades de un simplebutton control desde otro hilo.
        /// </summary>
        /// <param name="pButton">Control simplebutton</param>
        private delegate void Disable(SimpleButton pButton, bool Option);

        /// <summary>
        /// Permite manipular el gridcontrol desde otro hilo.
        /// </summary>
        /// <param name="pGrid"></param>
        /// <param name="pListado"></param>
        private delegate void LoadGrid(DevExpress.XtraGrid.GridControl pGrid, List<PrevFiniquito> pListado);

        /// <summary>
        /// Indica si se genera o no el documento final
        /// </summary>
        private bool GeneraDocumento = false;

        /// <summary>
        /// Nos indica si el trabajador tiene un contrato que se remunera por horas trabajadas.
        /// </summary>
        private bool PagoEnHoras = false;

        public frmFiniquito()
        {
            InitializeComponent();
        }

        public frmFiniquito(string pContrato, int pPeriodo)
        {
            InitializeComponent();
            Contrato = pContrato;
            Periodo = pPeriodo;
        }

        private void frmFiniquito_Load(object sender, EventArgs e)
        {
            datoCombobox.spllenaComboBox("SELECT codCausal, descCausal FROM causaltermino ORDER BY descCausal", txtCausal, "codCausal", "descCausal", true);
            ComboTipoContrato(txtTipoContrato);
            ComboTipoRemuneracion(txtTipoRemuneracion);
            cbSeguroCes.Checked = false;
            txtSeguroCes.Enabled = false;
            txtSeguroCes.Text = "0";

            Configuracion conf = new Configuracion();
            conf.SetConfiguracion();

            txtPlantilla.Text = conf.RutaPlantillaFiniquito;

            Barra = new BarraProgreso(BarraProceso, 1, false, false, this);

            if (Contrato.Length == 0)
            { XtraMessageBox.Show("Numero de contrato no existe", "Contrato", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (Calculo.PeriodoValido(Periodo) == false)
            { XtraMessageBox.Show("Periodo no válido", "Periodo", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (Persona.ExisteContrato(Contrato, Periodo) && Calculo.PeriodoValido(Periodo))
            {
                //OBTENEMOS LA INFORMACION DE LA PERSONA
                Trabajador = Persona.GetInfo(Contrato, Periodo);
                if (Trabajador != null)
                {
                    txtContrato.Text = Contrato;
                    dtIniciocontrato.EditValue = Trabajador.Ingreso;
                    dtTerminocontrato.EditValue = Trabajador.Salida;
                    txtTipoContrato.EditValue = Trabajador.Tipocontrato;
                    txtTipoRemuneracion.EditValue = Trabajador.RegimenSalario;
                    txtCausal.EditValue = Trabajador.codCausal;
                    dtFechaFiniquito.DateTime = DateTime.Now.Date;
                    lblNombre.Text = Trabajador.NombreCompleto;
                  
                }
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();           

            Close();
        }

        #region "Manejo de datos"
        /// <summary>
        /// Cargar combo tipo contrato.
        /// </summary>
        /// <param name="pCombo"></param>
        private void ComboTipoContrato(LookUpEdit pCombo)
        {
            List<datoCombobox> lista = new List<datoCombobox>();
            int i = 0;
            while (i <= 2)
            {
                if (i == 0) lista.Add(new datoCombobox() { KeyInfo = i, descInfo = "INDEFINIDO" });
                if (i == 1) lista.Add(new datoCombobox() { KeyInfo = i, descInfo = "PLAZO FIJO" });
                if (i == 2) lista.Add(new datoCombobox() { KeyInfo = i, descInfo = "OBRA O FAENA" });
                i++;
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

        /// <summary>
        /// Combo tipo remuneracion.
        /// </summary>
        /// <param name="pCombo"></param>
        private void ComboTipoRemuneracion(LookUpEdit pCombo)
        {
            List<datoCombobox> lista = new List<datoCombobox>();
            int i = 0;
            while (i <= 1)
            {
                //agregamos valores a la lista
                if (i == 0) lista.Add(new datoCombobox() { KeyInfo = i, descInfo = "Variable" });
                if (i == 1) lista.Add(new datoCombobox() { KeyInfo = i, descInfo = "Fijo" });
                i++;
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

        private void ColumnasGrilla()
        {
            if (viewFiniquito.RowCount > 0)
            {
                viewFiniquito.Columns[0].Caption = "Descripción";
                viewFiniquito.Columns[1].Caption = "Monto";
                viewFiniquito.Columns[1].AppearanceCell.FontStyleDelta = FontStyle.Bold;
                viewFiniquito.Columns[1].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                viewFiniquito.Columns[1].DisplayFormat.FormatString = "n2";
                viewFiniquito.Columns[2].Visible = false;
                viewFiniquito.Columns[3].Visible = false;
            }
        }

        private void LoadGridControl(DevExpress.XtraGrid.GridControl pGrid, List<PrevFiniquito> pListado)
        {
            if (this.InvokeRequired)
            {
                LoadGrid dis = new LoadGrid(LoadGridControl);

                //PARMETROS
                object[] parameters = new object[] { pGrid, pListado };

                this.Invoke(dis, parameters);
            }
            else
            {
                pGrid.DataSource = pListado;
                fnSistema.spOpcionesGrilla(viewFiniquito);
                if (viewFiniquito.RowCount > 0)
                {                   
                    ColumnasGrilla();
                    viewFiniquito.BestFitColumns();
                }
                    
            }
        }

        #endregion

        private void btnGenerar_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            if (cbSimula.Checked == false && cbGenera.Checked == false)
            { XtraMessageBox.Show("Por favor selecciona al menos una opción", "Opcion", MessageBoxButtons.OK, MessageBoxIcon.Warning); cbSimula.Focus(); return; }

            if (Contrato.Length == 0)
            { XtraMessageBox.Show("Contrato no válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (Calculo.PeriodoValido(Periodo) == false)
            { XtraMessageBox.Show("Periodo no válido", "Periodo", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            //Solo si se quiere generar el documento.
            if (cbGenera.Checked)
            {
                if (Directory.Exists(txtSalida.Text) == false)
                { XtraMessageBox.Show("Directorio de salida no válido", "Directorio", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                if (File.Exists(txtPlantilla.Text) == false)
                { XtraMessageBox.Show("Plantilla no válida", "Plantilla", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; } 
            }

            if (dtFechaFiniquito.DateTime < dtTerminocontrato.DateTime)
            { XtraMessageBox.Show("La fecha de finiquito no puede ser menor a la fecha de termino de contrato", "Error en fechas", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            //TimeSpan dias = (dtTerminocontrato.DateTime - dtIniciocontrato.DateTime);
            //if ((dias.Days + 1) <= 30)
            //{ XtraMessageBox.Show("No se genera finiquito para contratos de trabajo cuya duracion es menor o igual a 30 días", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }          

            PagoEnHoras = cbHoras.Checked;

            ThreadStart del = new ThreadStart(ComenzarProceso);
            Thread hilo = new Thread(del);
            hilo.Start();
        }

        private void btnSalida_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            FolderBrowserDialog Brow = new FolderBrowserDialog();          
            if (Brow.ShowDialog() == DialogResult.OK)
            {
                if (Directory.Exists(Brow.SelectedPath))
                {
                    txtSalida.Text = Brow.SelectedPath;
                }
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
                    txtPlantilla.Text = di.FileName;
                }
            }
        }

        private void cbSeguroCes_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSeguroCes.Checked)
            {
                txtSeguroCes.Enabled = true;
                txtSeguroCes.Focus();
                txtSeguroCes.Text = "";
            }
            else
            {
                txtSeguroCes.Text = "0";
                txtSeguroCes.Enabled = false;
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

        private void ComenzarProceso()
        {
            Running = true;
            ShowCloseButton(false);

            DataSet data = new DataSet();
            string FileName = "";            

            if (Trabajador != null)
            {
                Finiquito fin = new Finiquito(Trabajador, Convert.ToDateTime(dtFechaFiniquito.EditValue), cbAviso.Checked, cbPrestamo.Checked, cbSeguroCes.Checked ? Convert.ToDouble(txtSeguroCes.Text) : 0, PagoEnHoras);

                if (PagoEnHoras == false)
                    data = fin.CalculaFiniquito();
                else
                    data = fin.CalculaFiniquitoHoras();

                    if (data.Tables.Count == 0)
                    {
                        XtraMessageBox.Show("No se encontró información", "Información", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        Running = false;
                        ShowCloseButton(true);
                        return;
                    }

                if (data.Tables[0].Rows.Count > 0)
                {
                    Barra.ShowControl = true;
                    Barra.Maximum = data.Tables[0].Rows.Count;
                    Barra.Begin();

                    if (fin.GetListPrevisualiza().Count > 0)
                    {
                        LoadGridControl(gridFiniquito, fin.GetListPrevisualiza());
                    }

                    FileName = txtSalida.Text + "\\Finiquito_" + Trabajador.Contrato + ".docx";
                    Documento doc = new Documento("", 0);
                    if (GeneraDocumento)
                    {
                        doc.GeneraCartaAviso(data, txtPlantilla.Text, FileName);
                    }

                    Barra.Increase();

                    if (GeneraDocumento)
                    {
                        if (File.Exists(FileName))
                        {
                            DialogResult preg = XtraMessageBox.Show($"Archivo {FileName} creado correctamente" + Environment.NewLine + "¿Deseas ver el archivo?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (preg == DialogResult.Yes)
                                System.Diagnostics.Process.Start(FileName);
                        }
                        else
                        {
                            XtraMessageBox.Show("No se pudo generar archivo", "Archivo", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        }
                    }

                    Barra.ShowControl = false;
                    Barra.ShowClose();
                    Running = false;
                    ShowCloseButton(true);
                }
                else
                {
                    XtraMessageBox.Show("No se pudo generar archivo", "Archivo", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    Running = false;
                }
           
            }
            Running = false;
            ShowCloseButton(true);
        }

        private void frmFiniquito_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Running)
            { XtraMessageBox.Show("No puedes cerrar esta ventana porque hay un proceso activo", "Proceso", MessageBoxButtons.OK, MessageBoxIcon.Stop); e.Cancel = true; }
        }

        private void DisableButton(SimpleButton pButton, bool pOption)
        {
            if (this.InvokeRequired)
            {
                Disable dis = new Disable(DisableButton);

                //PARMETROS
                object[] parameters = new object[] { pButton, pOption};

                this.Invoke(dis, parameters);
            }
            else
            {
                pButton.Enabled = pOption;
            }
        }

        private void ShowCloseButton(bool pOption)
        {
            DisableButton(btnGenerar, pOption);
            DisableButton(btnPlantilla, pOption);
            DisableButton(btnSalida, pOption);
        }

        private void txtSeguroCes_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtPlantilla_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtSalida_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void btnCalcular_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();            

            if (Contrato.Length == 0)
            { XtraMessageBox.Show("Contrato no válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (Calculo.PeriodoValido(Periodo) == false)
            { XtraMessageBox.Show("Periodo no válido", "Periodo", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (Directory.Exists(txtSalida.Text) == false)
            { XtraMessageBox.Show("Directorio de salida no válido", "Directorio", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (File.Exists(txtPlantilla.Text) == false)
            { XtraMessageBox.Show("Plantilla no válida", "Plantilla", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (dtFechaFiniquito.DateTime < dtTerminocontrato.DateTime)
            { XtraMessageBox.Show("La fecha de finiquito no puede ser menor a la fecha de termino de contrato", "Error en fechas", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (cbSimula.Checked == false && cbGenera.Checked == false)
            { XtraMessageBox.Show("Por favor selecciona al menos una opción", "Opcion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            TimeSpan dias = (dtTerminocontrato.DateTime - dtIniciocontrato.DateTime);
            if ((dias.Days + 1) <= 30)
            { XtraMessageBox.Show("No se genera finiquito para contratos de trabajo cuya duracion es menor o igual a 30 días", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

            
            ThreadStart del = new ThreadStart(ComenzarProceso);
            Thread hilo = new Thread(del);
            hilo.Start();
        }

        private void cbSimula_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSimula.Checked)
            {
                cbGenera.Checked = false;
                GeneraDocumento = false;
            }
            else
            {
                cbGenera.Checked = true;
            }
        }

        private void cbGenera_CheckedChanged(object sender, EventArgs e)
        {
            if (cbGenera.Checked)
            {
                cbSimula.Checked = false;
                GeneraDocumento = true;
            }
            else
            {
                cbSimula.Checked = true;
            }
        }

        private void btnPdf_Click(object sender, EventArgs e)
        {
            if (viewFiniquito.RowCount == 0)
            { XtraMessageBox.Show("No hay información para exportar", "Exportar", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            string FileName = "";
            if (viewFiniquito.RowCount > 0)
            {
                FolderBrowserDialog dil = new FolderBrowserDialog();
                dil.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);                
                if (dil.ShowDialog() == DialogResult.OK)
                {
                    FileName = dil.SelectedPath + "\\Finiquito_" + Contrato + ".pdf";
                    ExportOptions(FileName, 2);
                    //gridFiniquito.ExportToPdf(FileName);
                    if (File.Exists(FileName))
                    {
                        System.Diagnostics.Process.Start(FileName);
                    }
                    else
                    {
                        XtraMessageBox.Show("No se pudo exportar la información", "Exportar", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                    
                }
            }
        }

       

        private void btnExcel_Click_1(object sender, EventArgs e)
        {
            if (viewFiniquito.RowCount == 0)
            { XtraMessageBox.Show("No hay información para exportar", "Exportar", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            string FileName = "";

            if (viewFiniquito.RowCount > 0)
            {
                FolderBrowserDialog dil = new FolderBrowserDialog();
                dil.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                if (dil.ShowDialog() == DialogResult.OK)
                {
                    FileName = dil.SelectedPath + "\\Finiquito_" + Contrato + ".xlsx";
                    ExportOptions(FileName, 1);
                    //gridFiniquito.ExportToXlsx(FileName);
                    if (File.Exists(FileName))
                    {
                        System.Diagnostics.Process.Start(FileName);
                    }
                    else
                    {
                        XtraMessageBox.Show("No se pudo exportar la información", "Exportar", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }

                }
            }
        }

        private void ExportOptions(string FilePath, int pOption)
        {        
          
            switch (pOption)
            {

                /*Excel*/
                case 1:
                    XlsxExportOptionsEx OptionsExcel = new XlsxExportOptionsEx();
                    OptionsExcel.CustomizeSheetHeader += OptionsExcel_CustomizeSheetHeader;
                    viewFiniquito.ExportToXlsx(FilePath, OptionsExcel);
                    break;
                 /*PDF*/
                case 2:
                    if(Trabajador != null)
                        viewFiniquito.OptionsPrint.RtfPageHeader = Trabajador.NombreCompleto + "\t\tN°" + Trabajador.Contrato;
                    viewFiniquito.OptionsPrint.RtfPageFooter = DateTime.Now.Date.ToShortDateString();
                    viewFiniquito.ExportToPdf(FilePath);
                                        
                    break;

                default:
                    break;
            }

        }

        private void OptionsExcel_CustomizeSheetHeader(DevExpress.Export.ContextEventArgs e)
        {
            if (Trabajador != null)
            {
                e.ExportContext.AddRow(new object[] { Trabajador.NombreCompleto });
                //e.ExportContext.InsertImage(Properties.Resources.accountx16, new XlCellRange(new XlCellPosition(0, 0), new XlCellPosition(0,0)));
                e.ExportContext.MergeCells(new XlCellRange(new XlCellPosition(0, 0), new XlCellPosition(viewFiniquito.VisibleColumns.Count - 1, 0)));
            }            
                 
        }

        private void CreateDocument()
        {
            PrintableComponentLink link = new PrintableComponentLink();
            
            link.Component = gridFiniquito;

            //PageHeaderFooter phf = link.PageHeaderFooter as PageHeaderFooter;
            //if (phf != null)
            //{
            //    phf.Header.Content.Clear();
            //    phf.Header.Content.AddRange(new string[] { "", "", ""});
            //    phf.Header.Font = new Font("Arial", 12, FontStyle.Bold | FontStyle.Italic);

            //}

            //link.Margins = new System.Drawing.Printing.Margins(40, 40, 52, 40);
            //link.Landscape = true;

            link.CreateDocument();
            //link.Print();
        }
    }
}