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
    public partial class frmReabrePeriodo : DevExpress.XtraEditors.XtraForm
    {
        public IReabreMes Opener { get; set; }
        public frmReabrePeriodo()
        {
            InitializeComponent();
        }

        private void btnReabrirPeriodo_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            if (txtAbierto.Text == "" || txtReabre.Text == "")
            { XtraMessageBox.Show("Periodo no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

            if (Calculo.PeriodoValido(Convert.ToInt32(txtAbierto.Text)) == false)
            { XtraMessageBox.Show($"Periodo {fnSistema.PrimerMayuscula(fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(Convert.ToInt32(txtAbierto.Text))))} no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);return; }

            if (Calculo.PeriodoValido(Convert.ToInt32(txtReabre.Text)) == false)
            { XtraMessageBox.Show($"Periodo {fnSistema.PrimerMayuscula(fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(Convert.ToInt32(txtReabre.Text))))} no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            //REABRIMOS MES
            DialogResult advertencia = XtraMessageBox.Show($"Estas a punto de reabrir el periodo {fnSistema.PrimerMayuscula(fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(Convert.ToInt32(txtReabre.Text))))}, ¿Estás seguro de realizar esta operación?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (advertencia == DialogResult.Yes)
            {
                if (Calculo.ReabrirMes(Convert.ToInt32(txtReabre.Text)))
                    if (Opener != null)
                    {
                        Opener.SetVariable(fnSistema.PrimerMayuscula(fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(Calculo.PeriodoObservado))));
                        Opener.CloseKeyMaster();
                    }                        

                Close();
            }
            else
            {
                if (Opener != null)
                {
                    Opener.CloseKeyMaster();
                }
                Close();
            }
        }

        #region "MANEJO DE DATOS"


        #endregion

        private void frmReabrePeriodo_Load(object sender, EventArgs e)
        {
            txtAbierto.Text = Calculo.PeriodoObservado.ToString();
            txtMesAbierto.Text = fnSistema.PrimerMayuscula(fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(Calculo.PeriodoObservado)));
            txtReabre.Text = fnSistema.fnObtenerPeriodoAnterior(Calculo.PeriodoObservado).ToString();
            txtMesAbre.Text = fnSistema.PrimerMayuscula(fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(fnSistema.fnObtenerPeriodoAnterior(Calculo.PeriodoObservado))));
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            if (Opener != null)
                Opener.CloseKeyMaster();

            Close();
        }
    }
}