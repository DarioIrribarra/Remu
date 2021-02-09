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
using System.Diagnostics;

namespace Labour
{
    public partial class frmRespaldo : DevExpress.XtraEditors.XtraForm
    {
        private Alerta Al { set; get; }

        public frmRespaldo()
        {
            InitializeComponent();
        }

        private void frmRespaldo_Load(object sender, EventArgs e)
        {
            txtPeriodo.Text = fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(Calculo.PeriodoObservado));

            Al = new Alerta();
            Al.Formulario = this;
            Al.AControl = new DevExpress.XtraBars.Alerter.AlertControl();
        }

        private void btnRespaldar_Click(object sender, EventArgs e)
        {
            DialogResult adv = XtraMessageBox.Show($"¿Estás seguro de respaldar el mes {fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(Calculo.PeriodoObservado))}?", "Respaldo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            bool resultado = false;
            if (adv == DialogResult.Yes)
            {
                //Hacemos respaldo
                Al.Mensaje = "Iniciando respaldo...";
                Al.ShowMessage();
                try
                {
                    resultado = fnSistema.BackUpDataBase();
                    if (resultado)
                    {
                        DialogResult resp = XtraMessageBox.Show("Base de datos respaldada correctamente\n¿Desea ver carpeta de respaldos?", "Respaldo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (resp == DialogResult.Yes)
                        {
                            AbrirCarpetaRespaldos();
                        }
                            
                    }
                    else
                    {
                        XtraMessageBox.Show("No se pudo realizar respaldo", "Respaldo", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }

                    Al.Mensaje = "Proceso finalizado.";
                    Al.ShowMessage();
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show("No se pudo realizar respaldo", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            AbrirCarpetaRespaldos();
        }

        private void AbrirCarpetaRespaldos() 
        {
            try
            {
                string ruta = Configuracion.ConfiguracionGlobal.RutaRespaldo;
                //CREA DIRECTORIO SI NO EXISTE
                if (!Directory.Exists(ruta))
                {
                    Directory.CreateDirectory(ruta);
                }

                Process.Start(ruta);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Error al intentar acceder a carpeta de respaldos", "Respaldo", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            
        }
    }
}