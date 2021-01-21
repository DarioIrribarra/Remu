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
    public partial class frmPeriodoInicio : DevExpress.XtraEditors.XtraForm
    {
        public frmPeriodoInicio()
        {
            InitializeComponent();
        }

        private void frmPeriodoInicio_Load(object sender, EventArgs e)
        {
            txtPeriodo.Focus();
        }

        private void txtPeriodo_KeyPress(object sender, KeyPressEventArgs e)
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

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (txtPeriodo.Text == "")
            { XtraMessageBox.Show("Por favor ingresa un periodo válido", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            //VALIDAR FORMATO
            if (Calculo.ValidaFormatoPeriodo(txtPeriodo.Text) == false)
            { XtraMessageBox.Show("Por favor ingresa un periodo válido", "Formato Incorrecto", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            //GUARDAMOS PERIODO EN TABLA
            if (Calculo.GuardarPeriodo(Convert.ToInt32(txtPeriodo.Text)))
            {
                XtraMessageBox.Show("Periodo guardado correctamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                //CERRAMOS ESTE FORMULARIO
                this.Close();
                return;
            }
            else
            {
                XtraMessageBox.Show("No se pudo guardar periodo", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}