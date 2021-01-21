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
    public partial class frmSaveReportBuilder : DevExpress.XtraEditors.XtraForm
    {
        /// <summary>
        /// Para guardar consulta sql.
        /// </summary>
        private string ReportBody = "";

        /// <summary>
        /// Para porder guardar el reporte
        /// </summary>
        private ReportBuilder Reporte = new ReportBuilder();

        /// <summary>
        /// Nos indica si es una actualizacion del reporte y no un ingreso
        /// </summary>
        private bool UpdateReport = false;

        //Reporte que se setea cuando se desea cambiar el nombre del reporte
        private ReportBuilder reportUd = new ReportBuilder();

        /// <summary>
        /// Para comunicacion desacoplada con formualario savebuilder
        /// </summary>
        public IEditaBuilder IEditaOpen {get;set;}

        public frmSaveReportBuilder()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor para actualizar nombre.
        /// </summary>
        /// <param name="Actualiza"></param>
        /// <param name="pReport"></param>
        public frmSaveReportBuilder(bool Actualiza, ReportBuilder pReport)
        {
            InitializeComponent();
            this.UpdateReport = Actualiza;
            this.reportUd = pReport;
        }
        public frmSaveReportBuilder(string pReport)
        {
            InitializeComponent();
            this.ReportBody = pReport;
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            Close();
        }

        private void frmSaveReportBuilder_Load(object sender, EventArgs e)
        {
            if (UpdateReport)
            {
                txtNombre.Text = this.reportUd.Name;
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (txtNombre.Text.Length == 0)
            { XtraMessageBox.Show("Por favor indica el nombre para el reporte", "Nombre reporte", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (this.UpdateReport)
            {               
                //Es una actualizacion
                if (Reporte.ModReport(reportUd.Id, reportUd.Sql, txtNombre.Text))
                {
                    XtraMessageBox.Show("Registro actualizado correctamente", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    if (IEditaOpen != null)
                        IEditaOpen.ReloadGridControl();
                }
                else
                {
                    XtraMessageBox.Show("No se pudo actualizar registro", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                Close();
            }
            else
            {
                //Es un registro nuevo
                if (this.ReportBody.Length > 0)
                {
                    if (Reporte.AddReport(this.ReportBody, txtNombre.Text))
                    {
                        XtraMessageBox.Show("Reporte guardado correctamente", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        XtraMessageBox.Show("No se pudo guardar el reporte", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }

                    Close();
                }
            }

          
        }
    }
}