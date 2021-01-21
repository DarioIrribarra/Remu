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
using System.Data.SqlClient;

namespace Labour
{
    public partial class frmClave : DevExpress.XtraEditors.XtraForm
    {
        //VARIABLE PARA SABER SI EL FORMULARIO DE ABRIO DESDE FORMULARIO DE NUEVO USUario
        public bool NuevoUsuarioForm { get; set; } = false;
        public string UserName { get; set; } = "";

        //PARA SABER QUE TIPO DE CLAVE ES (CLAVE NORMAL O CLAVE MAESTRA)
        public string tipo { get; set; }

        public frmClave()
        {
            InitializeComponent();
        }

        public frmClave(bool NuevoUsuario, string User, string tipo)
        {
            InitializeComponent();
            NuevoUsuarioForm = NuevoUsuario;
            UserName = User;
            this.tipo = tipo;
        }

        private void frmClave_Load(object sender, EventArgs e)
        {            
            fnDefaultProperties();
            if (NuevoUsuarioForm)
            {
                //SE ABRIO DESDE FORMULARIO DE NUEVO USUARIO
                txtUsuario.Text = UserName;
            }
            else
            {
                txtUsuario.Text = User.getUser();
                txtPass.Focus();
            }            
        }


        #region "MANEJO DE DATOS"
        //MODIFICAR CONTRASEÑA USUARIO
        /*
         * PARAMETROS DE ENTRADA:
         * NOMBRE USUARIO
         * password
         */
        private void fnModificarPassword(TextEdit pUser, string pClave)
        {
            string sql = "UPDATE usuario SET password=@pClave WHERE usuario=@pUser";
            SqlCommand cmd;
            //SqlDataReader rd;
            int res = 0;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pUser", pUser.Text));
                        //ENCRIPTAR CONTRASEÑA

                        string hash = "";
                        Encriptacion enc = new Encriptacion();
                        hash = enc.EncodePassword(pClave);

                        cmd.Parameters.Add(new SqlParameter("@pClave", hash));

                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            XtraMessageBox.Show("Contraseña Actualizada", "Actualizar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            //GUARDAR EVENTO EN LOG

                            // logRegistro log = new logRegistro(User.getUser(), "USUARIO " + pUser.Text + " HA CAMBIADO SU CONTRASEÑA", fnSistema.pgDatabase, "USUARIO");
                            // log.Log();
                            CloseAllActiveForms();
                           
                            Close();
                           
                        }
                        else
                        {
                            XtraMessageBox.Show("Error al intentar actualizar", "Actualizar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    //LIBERAR RECURSOS
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
            
        }   

        private void fnDefaultProperties()
        {
          
            panelUser.TabStop = false;
            txtUsuario.ReadOnly = true;
        }

        //MANEJO TECLA TAB
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Tab)
            {
                if (txtPass.ContainsFocus)
                {
                    if (txtPass.Text == "")
                    {
                        lblerror.Visible = true;
                        lblerror.Text = "Por favor ingresa una contraseña";
                        return false;
                    }
                    else
                    {
                        lblerror.Visible = false;
                    }
                }
                else if (txtPass2.ContainsFocus)
                {
                    if (txtPass2.Text == "")
                    {
                        lblerror.Visible = true;
                        lblerror.Text = "Por favor ingresa contraseña de confirmacion";
                        return false;
                    }
                    else
                    {
                        lblerror.Visible = false;
                        //VALIDAR QUE LAS DOS CONTRASEÑAS COINCIDAN
                        if (txtPass.Text != txtPass2.Text)
                        {
                            lblerror.Visible = true;
                            lblerror.Text = "Las contraseñas no coinciden";
                            return false;
                        }
                        else
                        {
                            lblerror.Visible = false;
                        }
                    }
                }
            }

            return base.ProcessDialogKey(keyData);
        }

        //MODIFICAR LLAVE MAESTRA
        private void fnModificaPasswordMaestro(TextEdit pUser, string pClave)
        {
            string sql = "UPDATE usuario SET llavemaestra=@pLlavemaestra WHERE usuario=@pUser";
            SqlCommand cmd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        cmd.Parameters.Add(new SqlParameter("@pUser", pUser.Text));
                        Encriptacion enc = new Encriptacion();
                        cmd.Parameters.Add(new SqlParameter("@pLlavemaestra", enc.EncodePassword(pClave)));
                        if (cmd.ExecuteNonQuery() > 0)
                        {
                            XtraMessageBox.Show("Clave maestra modificada correctamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            CloseAllActiveForms();
                            Close();
                        }
                        else
                        {
                            XtraMessageBox.Show("Error al intentar modificar clave maestra", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }
        #endregion

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            if (txtPass.Text == "")
            {
                lblerror.Visible = true;
                lblerror.Text = "Por favor ingrese contraseña";
                txtPass.Focus();
                return;
            }

            if (txtPass2.Text == "")
            {
                lblerror.Visible = true;
                lblerror.Text = "Por favor ingrese contraseña de confirmacion";
                txtPass2.Focus();
                return;
            }
            lblerror.Visible = false;

            //VALIDAR QUE LAS DOS CONTRASEÑA SEAN IGUALES
            if (txtPass.Text != txtPass2.Text)
            {
                lblerror.Visible = true;
                lblerror.Text = "Las contraseñas no coinciden";
                return;
            }
            else
            {
                lblerror.Visible = false;
                //MODIFICAR CONTRASEÑA
                if (tipo == "normal")
                    fnModificarPassword(txtUsuario, txtPass2.Text);
                else if (tipo == "maestra")
                    fnModificaPasswordMaestro(txtUsuario, txtPass2.Text);
                else
                    fnModificarPassword(txtUsuario, txtPass2.Text);

            }
        }

        /*PARA CERRAR TODOS LOS FORMS ABIERTOS EXCEPTO EL QUE TIENE EL FOCO*/
        private void CloseAllActiveForms()
        {
            foreach (Form child in this.MdiChildren)
            {
                if (!child.Focused)
                {
                    child.Close();
                }
            }
        }

        private void txtPass_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (txtPass.Text == "")
                {
                    lblerror.Visible = true;
                    lblerror.Text = "Por favor ingresa una contraseña";
                    return;
                }
                else
                {
                    lblerror.Visible = false;
                }
            }
        }

        private void txtPass2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (txtPass2.Text == "")
                {
                    lblerror.Visible = true;
                    lblerror.Text = "Por favor ingresa la contraseña de confirmacion";
                    return;
                }
                else
                {
                    lblerror.Visible = false;
                    //VERIFICAR QUE LAS CONTRASEÑAS SEAN IGUALES
                    if (txtPass.Text != txtPass2.Text)
                    {
                        lblerror.Visible = true;
                        lblerror.Text = "Las contraseñas no coinciden";
                        return;
                    }
                    else
                    {
                        lblerror.Visible = false;
                    }
                }
            }
        }

        private void frmClave_Shown(object sender, EventArgs e)
        {
            txtPass.Focus();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            Close();
        }
    }
}