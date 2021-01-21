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
using DevExpress.XtraGrid.Views.Grid;

namespace Labour
{
    public partial class frmObservaciones : DevExpress.XtraEditors.XtraForm
    {
        /// <summary>
        /// Solo para saber si se actualiza o ingresa un registro.
        /// </summary>
        private bool UpdateReg = false;

        /// <summary>
        /// Para saber a que trabajador hacer la asociacion
        /// </summary>
        Persona Trabajador;
        private string ContratoPersona = "";
        private int PeriodoPersona = 0;
        private bool Cancela = false;

        private string SqlGrid = $"SELECT cod, descripcion, calificacion, fecha FROM observacion WHERE rutTrab='@Persona'";
      
        
        public frmObservaciones()
        {
            InitializeComponent();
        }

        public frmObservaciones(string pContrato, int pPeriodo)
        {
            InitializeComponent();
            ContratoPersona = pContrato;
            PeriodoPersona = pPeriodo;
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();
            Close();
        }

        private void frmObservaciones_Load(object sender, EventArgs e)
        {
            Trabajador = Persona.GetInfo(ContratoPersona, PeriodoPersona);

            SqlGrid = SqlGrid.Replace("@Persona", Trabajador.Rut);
            LoadCombo(txtCalificaciones);
         
            
            fnSistema.spllenaGridView(gridObservaciones, SqlGrid);
            if (viewObservaciones.DataSource != null)
            {
                fnSistema.spOpcionesGrilla(viewObservaciones);
                ColumnasGrilla();
                CargaDatos();
            }
            else
            {
                btnEliminar.Enabled = false;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            Observacion obs = new Observacion();
            int cod = 0;

            if (Trabajador.Rut.Length == 0)
            { XtraMessageBox.Show("Información no válida para trabajador", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (txtCalificaciones.EditValue == null || txtCalificaciones.Properties.DataSource == null)
            { XtraMessageBox.Show("por favor selecciona una calificación válida", "Calificación", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (txtDescripcion.Text == "")
            { XtraMessageBox.Show("Por favor ingresa una descripción", "Descripcion", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

            //Nuevo registro
            if (UpdateReg == false)
            {
                if (obs.Add(Trabajador.Rut, txtCalificaciones.EditValue.ToString(), DateTime.Now.Date, txtDescripcion.Text))
                {
                    XtraMessageBox.Show("Registro ingresado correctamente", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    fnSistema.spllenaGridView(gridObservaciones, SqlGrid);
                    if (viewObservaciones.RowCount > 0)
                    {
                        fnSistema.spOpcionesGrilla(viewObservaciones);
                        ColumnasGrilla();
                        CargaDatos();
                        ResetButton();
                    }
                }                
            }
            else
            {
                //Actualiza registro
                if (viewObservaciones.RowCount == 0)
                { XtraMessageBox.Show("Registro no válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                if (viewObservaciones.RowCount > 0)
                {
                    cod = Convert.ToInt32(viewObservaciones.GetFocusedDataRow()["cod"]);
                    if (cod != 0)
                    {
                        if (obs.Update(cod, Trabajador.Rut, txtCalificaciones.EditValue.ToString(), txtDescripcion.Text))
                        {
                            XtraMessageBox.Show("Registro actualizado correctamente", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            fnSistema.spllenaGridView(gridObservaciones, SqlGrid);
                            if (viewObservaciones.RowCount > 0)
                            {
                                fnSistema.spOpcionesGrilla(viewObservaciones);
                                ColumnasGrilla();
                                CargaDatos();
                                ResetButton();
                            }
                        }
                        else
                        {
                            XtraMessageBox.Show("Error al intentar actualizar registro", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        XtraMessageBox.Show("Error al intentar actualizar registro", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        #region "DATA"
        private void ColumnasGrilla()
        {
            if (viewObservaciones.RowCount > 0)
            {
                viewObservaciones.Columns[0].Caption = "#";
                viewObservaciones.Columns[1].Caption = "Descripcion";
                viewObservaciones.Columns[2].Caption = "Calificación";
                viewObservaciones.Columns[3].Caption = "Fecha";

                /*Ajustar tamaño de columnas*/
                viewObservaciones.BestFitColumns();
            }
        }

        private void CleanFields()
        {
            if (txtCalificaciones.Properties.DataSource != null)
                txtCalificaciones.ItemIndex = 0;
            txtDescripcion.Text = "";
            UpdateReg = false;
            btnEliminar.Enabled = false;         
            txtCalificaciones.Focus();
            btnNuevo.ImageOptions.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("office2013/actions/cancel_32x32.png");

        }

        private void CargaDatos()
        {
            if (viewObservaciones.RowCount > 0)
            {
                txtCalificaciones.EditValue = (string)viewObservaciones.GetFocusedDataRow()["calificacion"];
                txtDescripcion.Text = (string)viewObservaciones.GetFocusedDataRow()["descripcion"];

                UpdateReg = true;
                btnEliminar.Enabled = true;
                
            }
        }

        private void LoadCombo(LookUpEdit pComboBox)
        {
            List<formula> listado = new List<formula>();
            listado.Add(new formula() { key = "7.0", desc = "7.0"});
            listado.Add(new formula() { key = "6.0", desc = "6.0" });
            listado.Add(new formula() { key = "5.0", desc = "5.0" });
            listado.Add(new formula() { key = "4.0", desc = "4.0" });
            listado.Add(new formula() { key = "3.0", desc = "3.0" });
            listado.Add(new formula() { key = "2.0", desc = "2.0" });
            listado.Add(new formula() { key = "1.0", desc = "1.0" });

            pComboBox.Properties.DataSource = listado.ToList();
            pComboBox.Properties.ValueMember = "key";
            pComboBox.Properties.DisplayMember = "desc";

            pComboBox.Properties.PopulateColumns();
            //SI OCULTAR KEY ES TRUE NO SE DEBE MOSTRAR LA COLUMNA KEY
            pComboBox.Properties.Columns[0].Visible = false;

            pComboBox.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
            pComboBox.Properties.AutoSearchColumnIndex = 1;
            pComboBox.Properties.ShowHeader = false;

            if (pComboBox.Properties.DataSource != null)
                pComboBox.ItemIndex = 0;

        }


        private void ResetButton()
        {
            btnNuevo.ImageOptions.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("office2013/actions/add_32x32.png");
            btnNuevo.ToolTip = "Nueva Observación";
            Cancela = false;
        }
        #endregion

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            //Primer click
            if (Cancela == false)
            {
                CleanFields();
                Cancela = true;
                btnNuevo.ToolTip = "Cancelar operación";
            }
            else
            {                
                //SEGUNDO CLICK
                CargaDatos();
                Cancela = false;
                btnNuevo.ImageOptions.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("office2013/actions/add_32x32.png");
                btnNuevo.ToolTip = "Nueva Observación";
            }

            
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();
            int cod = 0;
            Observacion obs = new Observacion();

            if (viewObservaciones.RowCount == 0)
            { XtraMessageBox.Show("Registro no es válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);return; }

            if (viewObservaciones.RowCount > 0)
            {
                cod = Convert.ToInt32(viewObservaciones.GetFocusedDataRow()["cod"]);
                if (cod != 0)
                {
                    DialogResult advertencia = XtraMessageBox.Show("¿Estás seguro que deseas eliminar este registro?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (advertencia == DialogResult.No)
                        return;

                    if (obs.Delete(cod, Trabajador.Rut))
                    {
                        XtraMessageBox.Show("Registro eliminado correctamente", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        fnSistema.spllenaGridView(gridObservaciones, SqlGrid);
                        if (viewObservaciones.RowCount > 0)
                        {
                            fnSistema.spOpcionesGrilla(viewObservaciones);
                            ColumnasGrilla();
                            CargaDatos();
                            ResetButton();
                        }
                    }
                    else
                    {
                        XtraMessageBox.Show("No se pudo eliminar el registro", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                }
                else
                {
                    XtraMessageBox.Show("No se pudo eliminar el registro", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
        }

        private void gridObservaciones_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();
            CargaDatos();
            ResetButton();
        }

        private void gridObservaciones_KeyUp(object sender, KeyEventArgs e)
        {
            Sesion.NuevaActividad();
            CargaDatos();
            ResetButton();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();
            CargaDatos();
        }

        private void btnDoc_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();
            if (viewObservaciones.RowCount == 0)
            { XtraMessageBox.Show("No hay información para exportar", "información", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            string FileName = "";

            if (viewObservaciones.RowCount > 0)
            {
                FolderBrowserDialog folder = new FolderBrowserDialog();
                if (folder.ShowDialog() == DialogResult.OK)
                {
                    if (Directory.Exists(folder.SelectedPath))
                    {
                        FileName = folder.SelectedPath + "\\" + Trabajador.Rut + ".docx";
                        gridObservaciones.ExportToDocx(FileName);

                        if (File.Exists(FileName))
                        {
                            DialogResult adv = XtraMessageBox.Show($"Archivo generado correctamente en {FileName}.\n¿Deseas ver archivo?", "Exportar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (adv == DialogResult.Yes)
                            {
                                System.Diagnostics.Process.Start(FileName);
                            }
                        }
                        else
                        {
                            XtraMessageBox.Show("No se pudo exportar la información", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        }
                    }
                    else
                    {
                        XtraMessageBox.Show("Directorio de salida no válido.", "Directorio", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }
                }
            }
        }

        private void btnPdf_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();
            if (viewObservaciones.RowCount == 0)
            { XtraMessageBox.Show("No hay información para exportar", "información", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            string FileName = "";

            if (viewObservaciones.RowCount > 0)
            {
                FolderBrowserDialog folder = new FolderBrowserDialog();
                if (folder.ShowDialog() == DialogResult.OK)
                {
                    if (Directory.Exists(folder.SelectedPath))
                    {
                        FileName = folder.SelectedPath + "\\" + Trabajador.Rut + ".pdf";
                        gridObservaciones.ExportToPdf(FileName);

                        if (File.Exists(FileName))
                        {
                            DialogResult adv = XtraMessageBox.Show($"Archivo generado correctamente en {FileName}.\n¿Deseas ver archivo?", "Exportar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (adv == DialogResult.Yes)
                            {
                                System.Diagnostics.Process.Start(FileName);
                            }
                        }
                        else
                        {
                            XtraMessageBox.Show("No se pudo exportar la información", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        }
                    }
                    else
                    {
                        XtraMessageBox.Show("Directorio de salida no válido.", "Directorio", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }
                }
            }
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();
            if (viewObservaciones.RowCount == 0)
            { XtraMessageBox.Show("No hay información para exportar", "información", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            string FileName = "";

            if (viewObservaciones.RowCount > 0)
            {
                FolderBrowserDialog folder = new FolderBrowserDialog();
                if (folder.ShowDialog() == DialogResult.OK)
                {
                    if (Directory.Exists(folder.SelectedPath))
                    {
                        FileName = folder.SelectedPath + "\\" + Trabajador.Rut + ".xlsx";
                        gridObservaciones.ExportToXlsx(FileName);

                        if (File.Exists(FileName))
                        {
                            DialogResult adv = XtraMessageBox.Show($"Archivo generado correctamente en {FileName}.\n¿Deseas ver archivo?", "Exportar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (adv == DialogResult.Yes)
                            {
                                System.Diagnostics.Process.Start(FileName);
                            }
                        }
                        else
                        {
                            XtraMessageBox.Show("No se pudo exportar la información", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        }
                    }
                    else
                    {
                        XtraMessageBox.Show("Directorio de salida no válido.", "Directorio", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }
                }
            }
        }

        private void viewObservaciones_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            GridView CurrentView = sender as GridView;
            double Nota = 0;
            try
            {
                if (e.Column.FieldName == "calificacion")
                {
                    string value = "";
                    value = CurrentView.GetRowCellValue(e.RowHandle, "calificacion").ToString();
                    Nota = Convert.ToDouble(CurrentView.GetRowCellValue(e.RowHandle, "calificacion"));
                    if (Nota > 40)
                        e.Appearance.ForeColor = Color.Green;
                    else
                        e.Appearance.ForeColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                //ERROR...
            }

        }
    }
}