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
    public partial class frmListadoItems : DevExpress.XtraEditors.XtraForm, IConjuntosCondicionales
    {

        #region "INTERFAZ"
        public void CargarCodigoConjunto(string pConjunto)
        {
            txtConjunto.Text = pConjunto;
        }
        #endregion

        public frmListadoItems()
        {
            InitializeComponent();
        }

        private void frmListadoItems_Load(object sender, EventArgs e)
        {
            datoCombobox.spLlenaPeriodos("SELECT anomes FROM parametro ORDER BY anomes DESC", txtComboPeriodo, "anomes", "anomes", true);

            cbTodos.Checked = true;
            txtConjunto.Enabled = false;
            btnConjunto.Enabled = false;
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

        private void btnConjunto_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            if (!cbTodos.Checked)
            {
                FrmFiltroBusqueda filtro = new FrmFiltroBusqueda(true);
                filtro.opener = this;
                filtro.StartPosition = FormStartPosition.CenterParent;
                filtro.ShowDialog(); 
            }
        }

        private void btnGenerar_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (txtComboPeriodo.Properties.DataSource == null)
            { XtraMessageBox.Show("Por favor selecciona un periodo válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (Calculo.PeriodoValido(Convert.ToInt32(txtComboPeriodo.EditValue)) == false)
            { XtraMessageBox.Show("Por favor selecciona un periodo válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            DataTable tb = new DataTable();
            string FileName = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\ListadoItems_" + Convert.ToUInt32(txtComboPeriodo.EditValue) + ".xlsx";
            if (cbTodos.Checked)
            {
                //TODOS LOS REGISTROS DEL PERIODO
                tb = Persona.GetDataDinamic(Convert.ToInt32(txtComboPeriodo.EditValue), "");

                if (tb.Rows.Count > 0)
                {
                    //GENERAMOS EXCEL
                    if (FileExcel.CrearArchivoExcelDev(tb, FileName))
                    {
                        DialogResult Adv = XtraMessageBox.Show($"Archivo {FileName} creado correctamente, \n¿Deseas ver archivo?", "Archivo", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                        if (Adv == DialogResult.Yes)
                            System.Diagnostics.Process.Start(FileName);
                    }
                    else
                    {
                        XtraMessageBox.Show("No se pudo crear archivo", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                }
                else
                {
                    XtraMessageBox.Show("No se encontró información", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
            else
            {

                if (Conjunto.ExisteConjunto(txtConjunto.Text) == false)
                { XtraMessageBox.Show("Por favor ingresa una condición válida", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                //BUSCAMOS CONJUNTO
                tb = Persona.GetDataDinamic(Convert.ToInt32(txtComboPeriodo.EditValue), txtConjunto.Text);

                if (tb.Rows.Count > 0)
                {
                    //GENERAMOS EXCEL
                    if (FileExcel.CrearArchivoExcelDev(tb, FileName))
                    {
                        DialogResult Adv = XtraMessageBox.Show($"Archivo {FileName} creado correctamente, \n¿Deseas ver archivo?", "Archivo", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                        if (Adv == DialogResult.Yes)
                            System.Diagnostics.Process.Start(FileName);
                    }
                    else
                    {
                        XtraMessageBox.Show("No se pudo crear archivo", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                }
                else
                {
                    XtraMessageBox.Show("No se encontró información", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void txtConjunto_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar) || char.IsLetter(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void txtConjunto_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }
    }
}