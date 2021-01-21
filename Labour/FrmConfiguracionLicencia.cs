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

    public partial class FrmConfiguracionLicencia : DevExpress.XtraEditors.XtraForm
    {
        public ILicenciaAcceso openLicencia { get; set; }

        public FrmConfiguracionLicencia()
        {
            InitializeComponent();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (txtKey.Text.Length == 0)
            {
                XtraMessageBox.Show("Licencia no válida", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); Close();
                if (openLicencia != null)
                {
                    Licencia.ForzarCierre = true;
                    openLicencia.CloseForms();
                }
                return;
            }

            Cifrado cif = new Cifrado();
            //GENERA CODIGO DE LICENCIA SIN CONSIDERAR EL PARAMETRO NUMERO DE USUARIOS CONCURRENTES
            string cod = Licencia.GenerarLicencia();
           

            if (cod.Length == 0)
            {
                XtraMessageBox.Show("Error al verificar codigo de licencia", "Licencia", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                if (openLicencia != null)
                {
                    Licencia.ForzarCierre = true;
                    openLicencia.CloseForms();
                }
                return;
            }

            //OBTENEMOS LA CANTIDAD DE USUARIO DE LAS DOS CADENAS
            string users = cif.GetUserCount(txtKey.Text.Trim()).ToString();
            users = "@" + cif.EncryptBase64(Convert.ToInt32(users));

            if ((cod.Trim() + users) == txtKey.Text.Trim())
            {
                //GUARDAMOS VALOR EN TABLA 
                if (Licencia.IngresaLicencia(txtKey.Text.Trim()))
                {
                    XtraMessageBox.Show("Licencia registrada correctamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //CERRAMOS FORMULARIO
                    Licencia.ForzarCierre = false;
                    Close();                    
                }
                else
                {
                    XtraMessageBox.Show("Se ha producido un error al registrar la licencia", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);

                    if (openLicencia != null)
                    {
                        Licencia.ForzarCierre = true;
                        openLicencia.CloseForms();
                    }
                }              
            }
            else
            {
                XtraMessageBox.Show("Codigo de licencia proporcionado no es válido, \n verifica la informacion y vuelve a intentarlo", "Licencia", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (openLicencia != null)
                {
                    Licencia.ForzarCierre = true;
                    openLicencia.CloseForms();
                }
                return;
            }
        }

        private void FrmConfiguracionLicencia_Load(object sender, EventArgs e)
        {

        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}