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
using System.Threading;

namespace Labour
{
    public partial class frmConfCorreo : DevExpress.XtraEditors.XtraForm
    {

        //DESHABILITAR BOTON GUARDAR
        delegate void HideButton(bool Status, SimpleButton pButton);

        delegate void FlagServer(bool pSatus);

        //SI SERVER STATUS ES TRUE SE LOGRÓ REALIZAR PRUEBA DE CONEXION CON SMTP
        public bool ServerStatus { get; set; } = false;

        BarraProgreso Barra;

        private Alerta Al { get; set; }

        public frmConfCorreo()
        {
            InitializeComponent();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void txtPuerto_KeyPress(object sender, KeyPressEventArgs e)
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

        private void frmConfCorreo_Load(object sender, EventArgs e)
        {
            Barra = new BarraProgreso(ProgressBar, 1, false, false, this);
            txtSmtp.Focus();
            CargarDatos();

            Al = new Alerta();
            Al.Formulario = this;
            Al.AControl = new DevExpress.XtraBars.Alerter.AlertControl();
        }

        #region "MANEJO DE DATOS"
        //GUARDAR CONFIGURACION
        private bool NuevaConfiguracion(TextEdit pSmtp, TextEdit pMail, TextEdit pPassword, TextEdit pPort, CheckEdit pSsl)
        {
            string sql = "INSERT INTO correo(mailserver, password, port, ssl, smtpserver) VALUES(@pMailServer, @pPassword, @pPort, @pSsl, @pSmtpserver)";
            string sqlDel = "DELETE FROM CORREO";
            SqlCommand cmd;

            int count = 0;

            Cifrado cif = new Cifrado();
            SqlTransaction transaction;

            bool correcto = false;          

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    //INICIAMOS LA TRANSACCION
                    transaction = fnSistema.sqlConn.BeginTransaction();

                    try
                    {
                        //ELIMINAR DATOS DE TABLA CORREOS SI ES QUE HAY
                        using (cmd = new SqlCommand(sqlDel, fnSistema.sqlConn))
                        {
                            cmd.Transaction = transaction;

                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                            cmd.Dispose();
                        }

                        //INGRESAMOS DATA EN TABLA
                        using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@pMailServer", cif.EncriptaTripleDesc(pMail.Text)));
                            cmd.Parameters.Add(new SqlParameter("@pPassword", cif.EncriptaTripleDesc(pPassword.Text)));
                            cmd.Parameters.Add(new SqlParameter("@pPort", pPort.Text));
                            cmd.Parameters.Add(new SqlParameter("@pSsl", pSsl.Checked));
                            cmd.Parameters.Add(new SqlParameter("@pSmtpserver", cif.EncriptaTripleDesc(pSmtp.Text)));

                            cmd.Transaction = transaction;

                            count = cmd.ExecuteNonQuery();
                            if (count > 0)
                            {
                                XtraMessageBox.Show("Configuración guardada correctamente", "Configuración", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }

                            cmd.Parameters.Clear();
                            cmd.Dispose();
                        }

                        transaction.Commit();
                        
                        correcto = true;
                    }
                    catch (SqlException ex)
                    {
                        //ERROR
                        correcto = false;
                        transaction.Rollback();
                    }                  
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return correcto;
        }

        //ACTUALIZAR CONFIGURACION
        private void ActualizarConfiguracion(TextEdit pSmtp, TextEdit pMail, TextEdit pPassword, TextEdit pPort, CheckEdit pSsl)
        {
            string sql = "UPDATE CORREO set mailserver = @pMailServer, password = @pPassword, port = @pPort, ssl=@pSsl, smtpserver=@pSmtpserver";
            SqlCommand cmd;

            int count = 0;

            Encriptacion enc = new Encriptacion();
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pMailServer", enc.EncodePassword(pMail.Text)));
                        cmd.Parameters.Add(new SqlParameter("@pPassword", enc.EncodePassword(pPassword.Text)));
                        cmd.Parameters.Add(new SqlParameter("@pPort", pPort.Text));
                        cmd.Parameters.Add(new SqlParameter("@pSsl", pSsl.Checked));
                        cmd.Parameters.Add(new SqlParameter("@pSmtpserver", enc.EncodePassword(pSmtp.Text)));

                        count = cmd.ExecuteNonQuery();
                        if (count > 0)
                        {
                            XtraMessageBox.Show("Configuración guardada correctamente", "Configuración", MessageBoxButtons.OK, MessageBoxIcon.Stop);
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

        //CARGAR DATOS EN CAJAS
        private void CargarDatos()
        {
            string sql = "SELECT TOP 1 mailserver, password, port, ssl, smtpserver FROM correo";
            SqlCommand cmd;
            SqlDataReader rd;
            Cifrado cipher = new Cifrado();
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //CARGAMOS CAJAS
                                txtMail.Text = cipher.DesencriptaTripleDesc((string)rd["mailserver"]);
                                txtPassword.Text = cipher.DesencriptaTripleDesc((string)rd["password"]);
                                txtPasswordConfirm.Text = cipher.DesencriptaTripleDesc((string)rd["password"]);
                                txtPuerto.Text = Convert.ToInt32(rd["port"]) + "";
                                txtSmtp.Text = cipher.DesencriptaTripleDesc((string)rd["smtpserver"]);
                                cbSsl.Checked = (bool)rd["ssl"];

                            }
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

        /*PROCESO QUE SE EJECUTA EN UN SEGUNDO HILO*/
        private void TestMail()
        {
            Al.Mensaje = "Realizando prueba de correo electrónico.";
            Al.ShowMessage();

            DisableButton(false, btnGuardar);
            DisableButton(false, btnTestMail);

            Mail email = new Mail(txtSmtp.Text, Convert.ToInt32(txtPuerto.Text), txtMail.Text, txtPassword.Text, txtMail.Text, 0, "", "Este correo fue generado de forma automática por Sopytec Remuneraciones.", "Correo de Prueba", cbSsl.Checked);
            
            bool Correcto = false;
            int x = 0;
                
            Barra.ShowControl = true;
            Barra.Begin();

            //SIMULAMOS PROGRESO...
            while (x<90)
            {     
                Barra.Increase();
                x++;
            }

            //ENVIAR CORREO DE PRUEBA
            Correcto = email.SendTestMail();
          
            x = 0;
            //SIMULAMOS PROGRESO...
            while (x < 10)
            {                
                Barra.Increase();
                x++;
            }

            if (Correcto)
                XtraMessageBox.Show("Correo enviado exitosamente", "Correo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                XtraMessageBox.Show("No se pudo enviar correo", "Error envío", MessageBoxButtons.OK, MessageBoxIcon.Stop);

            Al.Mensaje = "Prueba finalizada";
            Al.ShowMessage();

            Barra.ShowControl = false;
            Barra.ShowClose();
            DisableButton(true, btnGuardar);
            DisableButton(true, btnTestMail);

            //if (Correcto)
            //    Barra.CloseForm();
        }

        /*PRUEBA AL SERVIDOR SMTP*/
        private void TestSmtp()
        {
            DisableButton(false, btnTestMail);
            DisableButton(false, btnGuardar);

            Mail email = new Mail(txtSmtp.Text, Convert.ToInt32(txtPuerto.Text), txtMail.Text, txtPassword.Text, txtMail.Text, 0, "", "Esto es un correo de prueba en otro hilo, " + Environment.NewLine + "Sistema Remu", "Correo de Prueba", cbSsl.Checked);            

            bool Correcto = false;
            int x = 0;
            //PropiedadesBarra();
            Barra.ShowControl = true;
            Barra.Begin();            

            //SIMULAMOS PROGRESO...
            while (x < 90)
            {
                //UpdateProgress();
                Barra.Increase();
                x++;
            }

            Correcto = email.TestSmtp();

            x = 0;
            //SIMULAMOS PROGRESO...
            while (x < 10)
            {
                //UpdateProgress();
                Barra.Increase();
                x++;
            }

            if (Correcto == false)
            {
                SmtpStatus(false);
                XtraMessageBox.Show("No se pudo establecer conexion con servidor smtp", "Servidor Smtp", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            else
            {
                SmtpStatus(true);

                if (NuevaConfiguracion(txtSmtp, txtMail, txtPassword, txtPuerto, cbSsl))
                {
                    Al.Mensaje = "Configuración de correo guardada.";
                    Al.ShowMessage();

                    //GUARDADO CORRECTAMENTE
                    logRegistro log = new logRegistro(User.getUser(), "SE GUARDA CONFIGURACION CORREO", "CORREO", "0", txtSmtp.Text, "INGRESAR");
                    log.Log();

                    //CARGA DATA DESDE TABLA EN CAJAS DE TEXTO
                    CargarDatos();

                }
            }

            //HideBar();
            Barra.ShowControl = false;
            Barra.ShowClose();
            DisableButton(true, btnTestMail);
            DisableButton(true, btnGuardar);

            if (Correcto)
                Barra.CloseForm();

        }

        /*FLAG SMTP*/
        private void SmtpStatus(bool pStatus)
        {
            if (this.InvokeRequired)
            {
                FlagServer delegado = new FlagServer(SmtpStatus);

                //PARAMETROS
                object[] parameters = new object[] { pStatus };

                this.Invoke(delegado, parameters);
            }
            else
            {
                this.ServerStatus = true;
            }
        }

        /**/
        private void DisableButton(bool pStatus, SimpleButton pButton)
        {
            if (this.InvokeRequired)
            {
                HideButton hide = new HideButton(DisableButton);

                object[] parameters = new object[] { pStatus, pButton };

                this.Invoke(hide, parameters);
            }
            else
            {
                pButton.Enabled = pStatus;
            }
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (txtPassword.ContainsFocus)
            {
                if (txtPassword.Text == "")
                { lblMessage.Visible = true; lblMessage.Text = "Por favor ingresa una contraseña"; return false; }

                lblMessage.Visible = false;
            }
            if (txtPasswordConfirm.ContainsFocus)
            {
                if (txtPasswordConfirm.Text == "")
                { lblMessage.Visible = true; lblMessage.Text = "Por favor confirma la contraseña"; return false; }

                if (txtPassword.Text != txtPasswordConfirm.Text)
                { lblMessage.Visible = true; lblMessage.Text = "Las contraseñas no coinciden"; return false; }

                lblMessage.Visible = false;
            }

            return base.ProcessDialogKey(keyData);
        }
        #endregion

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            if (txtMail.Text == "")
            { lblMessage.Visible = true; lblMessage.Text = "Por favor ingresa correo electrónico"; txtMail.Focus(); return; }
            if (txtSmtp.Text == "")
            { lblMessage.Visible = true; lblMessage.Text = "Por favor ingresa servidor smtp"; txtSmtp.Focus(); return; }
            if (txtPuerto.Text == "")
            { lblMessage.Visible = true; lblMessage.Text = "Por favor ingresa numero de puerto"; txtPuerto.Focus(); return; }

            if(txtPassword.Text == "")
            { lblMessage.Visible = true; lblMessage.Text = "Por favor ingresa contraseña"; txtPassword.Focus(); return; }

            if(txtPasswordConfirm.Text == "")
            { lblMessage.Visible = true; lblMessage.Text = "Por favor confirma la contraseña"; txtPasswordConfirm.Focus(); return; }

            if (txtPassword.Text != txtPasswordConfirm.Text)
            { lblMessage.Visible = true; lblMessage.Text = "Contraseñas no coinciden"; txtPassword.Focus(); return; }

            //PRUEBA AL SERVER SMTP
            ThreadStart delegado = new ThreadStart(TestSmtp);
            Thread SubProceso = new Thread(delegado);
            SubProceso.Name = "HiloCalculo";
            SubProceso.Start();
        }

        private void txtPasswordConfirm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (txtPasswordConfirm.Text == "")
                { lblMessage.Visible = true; lblMessage.Text = "Por favor confirme la contraseña"; return; }

                if (txtPassword.Text != txtPasswordConfirm.Text)
                { lblMessage.Visible = true; lblMessage.Text = "Las contraseñas no coinciden"; return; }

                lblMessage.Visible = false;
            }
        }

        private void btnTestMail_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            if (txtMail.Text == "")
            { lblMessage.Visible = true; lblMessage.Text = "Por favor ingresa correo electrónico"; txtMail.Focus(); return; }
            if (txtSmtp.Text == "")
            { lblMessage.Visible = true; lblMessage.Text = "Por favor ingresa servidor smtp"; txtSmtp.Focus(); return; }
            if (txtPuerto.Text == "")
            { lblMessage.Visible = true; lblMessage.Text = "Por favor ingresa numero de puerto"; txtPuerto.Focus(); return; }

            if (txtPassword.Text == "")
            { lblMessage.Visible = true; lblMessage.Text = "Por favor ingresa contraseña"; txtPassword.Focus(); return; }

            if (txtPasswordConfirm.Text == "")
            { lblMessage.Visible = true; lblMessage.Text = "Por favor confirma la contraseña"; txtPasswordConfirm.Focus(); return; }

            if (txtPassword.Text != txtPasswordConfirm.Text)
            { lblMessage.Visible = true; lblMessage.Text = "Contraseñas no coinciden"; txtPassword.Focus(); return; }                       

            //PARA REALIZAR TEST EMAIL
            ThreadStart delegado = new ThreadStart(TestMail);
            Thread SubProceso = new Thread(delegado);
            SubProceso.Name = "HiloCalculo";
            SubProceso.Start();

        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}