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

namespace Labour
{
    public partial class frmdetallerepcontable : DevExpress.XtraEditors.XtraForm
    {
        /// <summary>
        /// Para guardar el codigo del reporte al cual pertenecen las columnas
        /// </summary>
        private int CodReporte = 0;
        private bool UpdateCol = false;

        string sqlBase = "SELECT repo, col, titulo, valor FROM detrepocontable where repo={0}";

        public frmdetallerepcontable()
        {
            InitializeComponent();
        }

        public frmdetallerepcontable(int pCodReporte)
        {
            InitializeComponent();
            this.CodReporte = pCodReporte;
        }        

        private void frmdetallerepcontable_Load(object sender, EventArgs e)
        {
            LoadCombo(txtValor);

            if (CodReporte != 0)
            {
                sqlBase = string.Format(sqlBase, CodReporte);
                fnSistema.spllenaGridView(gridColumnas, sqlBase);
                if (viewColumnas.RowCount > 0)
                {
                    fnSistema.spOpcionesGrilla(viewColumnas);
                    columnas();
                    CargarDatos();
                }
            }
        }

        #region "datos"
        private void LoadCombo(LookUpEdit pComboBox)
        {
            List<datoCombobox> Listado = new List<datoCombobox>();

            for (int i = 1; i <= 25; i++)
            {
                Listado.Add(new datoCombobox() { KeyInfo = i, descInfo = "Columna N°" + i });
            }

            pComboBox.Properties.DataSource = Listado.ToList();
            pComboBox.Properties.ValueMember = "KeyInfo";
            pComboBox.Properties.DisplayMember = "descInfo";

            pComboBox.Properties.PopulateColumns();
            //SI OCULTAR KEY ES TRUE NO SE DEBE MOSTRAR LA COLUMNA KEY

            pComboBox.Properties.Columns[0].Visible = false;
            pComboBox.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
            pComboBox.Properties.AutoSearchColumnIndex = 1;
            pComboBox.Properties.ShowHeader = false;

            if (pComboBox.Properties.DataSource != null)
            {
                pComboBox.ItemIndex = 0;
            }

        }
        private void CargarDatos()
        {
            if (viewColumnas.RowCount > 0)
            {
                txtTitulo.Text = (string)viewColumnas.GetFocusedDataRow()["titulo"];
                txtValor.EditValue = Convert.ToInt32(viewColumnas.GetFocusedDataRow()["valor"]);
                UpdateCol = true;
            }
        }

        private void columnas()
        {
            if (viewColumnas.RowCount > 0)
            {
                viewColumnas.Columns[0].Caption = "";
                viewColumnas.Columns[0].Visible = false;
                viewColumnas.Columns[1].Caption = "#";
                viewColumnas.Columns[1].Width = 20;
                viewColumnas.Columns[2].Caption = "Titulo";
                viewColumnas.Columns[3].Caption = "Valor";                
            }
        }
        #endregion

        private void gridColumnas_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            CargarDatos();
        }

        private void gridColumnas_KeyUp(object sender, KeyEventArgs e)
        {
            Sesion.NuevaActividad();

            CargarDatos();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            if (txtValor.Properties.DataSource == null || txtValor.EditValue == null)
            { XtraMessageBox.Show("Por favor selecciona un valor valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            ReporteContableDetalle det = new ReporteContableDetalle();

            int col = 0;

            if (UpdateCol)
            {
                //ACTUALZAR
                if (viewColumnas.RowCount == 0)
                { XtraMessageBox.Show("No se pudo actualizar registro", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                col = Convert.ToInt32(viewColumnas.GetFocusedDataRow()["col"]);
                if (col > 0)
                {
                    if (det.UpdateDetalleReporte(CodReporte, col, txtTitulo.Text, txtValor.EditValue.ToString()))
                    {
                        XtraMessageBox.Show("Registro actualizado correctamente", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        fnSistema.spllenaGridView(gridColumnas, sqlBase);
                        if (viewColumnas.RowCount > 0)
                        {
                            fnSistema.spOpcionesGrilla(viewColumnas);
                            columnas();
                            CargarDatos();
                        }
                    }
                    else
                    {
                        XtraMessageBox.Show("No se pudo actualizar registro", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                }
                else
                {
                    XtraMessageBox.Show("No se pudo actualizar registro", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }            
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            Close();
        }
    }
}