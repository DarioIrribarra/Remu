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
    public partial class frmMaestra : DevExpress.XtraEditors.XtraForm, IReabreMes
    {

        public IMenu Opener { get; set; }

        #region "INTERFAZ COMUNICACION FRMMAESTRA-FRMREABREMES"
        public void SetVariable(string pData)
        {
            ChangePeriodo = pData;
        }

        public void CloseKeyMaster()
        {
            Close();
        }
        #endregion

        public string ChangePeriodo { get; set; }
        public frmMaestra()
        {
            InitializeComponent();
        }

        private void frmMaestra_Load(object sender, EventArgs e)
        {
            
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            if (txtPassword.Text == "")
            { XtraMessageBox.Show("Por favor ingresa contraseña", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtPassword.Focus(); return; }

            if (AccesoValido(txtPassword.Text) == false)
            { XtraMessageBox.Show("Contraseña ingresada no es válida", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Error); txtPassword.Focus(); return; }

            //ABRIMOS FORMULARIO REABRE MES
            //XtraMessageBox.Show("Acceso confirmado", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            frmReabrePeriodo reabre = new frmReabrePeriodo();
            reabre.Opener = this;
            reabre.StartPosition = FormStartPosition.CenterParent;
            reabre.ShowDialog();           
            
        }

        #region "MANEJO DATOS"
        //VALIDA CONTRASEÑA CORRECTA
        private bool AccesoValido(string pClave)
        {
            bool valido = false;

            Encriptacion enc = new Encriptacion();            

            if (enc.EncodePassword(pClave) == User.GetLlaveMaestra(User.getUser()))
                valido = true;

            return valido;
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Tab)
            {
                if (txtPassword.ContainsFocus)
                {
                    if (txtPassword.Text == "")
                    { lblError.Visible = true; lblError.Text = "Por favor ingresa contraseña"; txtPassword.Focus(); return false; }

                    lblError.Visible = false;
                }
            }

            return base.ProcessDialogKey(keyData);
        }
        #endregion

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (txtPassword.Text == "")
                { lblError.Visible = true; lblError.Text = "Por favor ingresa contraseña"; txtPassword.Focus(); return; }

                lblError.Visible = false;               
            }
        }

        private void frmMaestra_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Opener != null)
                Opener.CargarUser($"User: {User.NombreCompleto()}", $"BD: {fnSistema.pgDatabase}", ChangePeriodo);
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            Close();
        }
    }
}