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

namespace Labour
{
    public partial class frmCargaExternaAusentismo : DevExpress.XtraEditors.XtraForm
    {
        public frmCargaExternaAusentismo()
        {
            InitializeComponent();
        }

        #region "DATA"
        private void AgregaAusentismos(DataTable pTabla)
        {
            if (pTabla.Rows.Count > 0)
            {

            }
        }
        #endregion

        private void panelControl1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnRuta_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();

            if (open.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(open.FileName))
                {
                    txtRuta.Text = open.FileName;
                }
                else
                {
                    XtraMessageBox.Show("Archivo no válido", "Archivo", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
        }

        private void btnCargarArchivo_Click(object sender, EventArgs e)
        {
            if (File.Exists(txtRuta.Text) == false || txtRuta.Text.Length == 0)
            { XtraMessageBox.Show("Ruta archivo no válido", "Ruta", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            DataTable TablaAus = FileExcel.ReadExcelDev(txtRuta.Text);

            if (TablaAus == null || TablaAus.Rows.Count == 0)
            { XtraMessageBox.Show("No se encontró información", "Información", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (TablaAus.Rows.Count > 0)
            {

            }
        }
    }
}