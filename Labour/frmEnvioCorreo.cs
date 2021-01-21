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
using System.Threading;

namespace Labour
{
    public partial class frmEnvioCorreo : DevExpress.XtraEditors.XtraForm
    {
        //PARA GUARDAR EL CORREO QUE SE CARGA (DESDE TABLA TRABAJADOR)
        public string CorreoTrabajador { get; set; } = "";
        //PARA GUARDAR EL CONTRATO DEL TRABAJADOR
        public string RutTrabajador { get; set; } = "";

        delegate void DisableButton(bool pStatus, SimpleButton pButton);

        BarraProgreso Barra;

        /// <summary>
        /// Para mostrar alerta
        /// </summary>
        private Alerta Al { get; set; }

        #region "FUNCIONES"
        //METODO PARA ENVIAR CORRE EN SEGUNDO PLANO
        private void EnviarCorreoBackGround()
        {
            bool statusOk = false;
            int count = 0;

            Al.Mensaje = "Enviando...";
            Al.ShowMessage();

            HabilitarButton(false, btnArchivo);
            HabilitarButton(false, btnEnviar);
            HabilitarButton(false, btnConfiguracion);
            HabilitarButton(false, btnSalir);

            Barra.ShowControl = true;
            Barra.Begin();
            
            //PODEMOS ENVIAR CORREO
            Mail email = new Mail();

            //SETEAMOS VARIABLES DESDE TABLA BD
            email.SetConfiguration();

            if (email.ServerSmtp == "" || email.PasswordServer == "" || email.EmailServer == "")
            {XtraMessageBox.Show("Servidor smtp no válido", "Configuración", MessageBoxButtons.OK, MessageBoxIcon.Stop); Al.Mensaje = "Verifique los parametros de configuracion de su correo.";Al.ShowMessage(); return; }

            //TITULO MENSAJE
            email.TitleMessage = txtTitulo.Text;

            //MENSAJE
            email.Message = txtMensaje.Text;

            //RUTA ARCHIVO ADJUNTO
            email.pRutaArchivo = txtRutaAdjunto.Text;

            //CORREO DESTINATARIO
            email.ClientEmail = txtCorreoDestino.Text;

            //SIMULAMOS PROGRESO
            while (count <90)
            {
                Barra.Increase();

                count++;
            }

            //ENVIAR CORRECTO
            statusOk = email.SendMail();

            //VOLVEMOS A SIMULAR PROGRESO
            count = 0;
            while (count <10)
            {
                Barra.Increase();
                count++;
            }
            
            if (statusOk)
            {
                XtraMessageBox.Show("Correo enviado correctamente", "Envio", MessageBoxButtons.OK, MessageBoxIcon.Information);

                Al.Mensaje = "Información enviada correctamente.";
                Al.ShowMessage();
            }
            else
            {
                XtraMessageBox.Show("No se pudo enviar correo", "Error envío", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Al.Mensaje = "Correo enviado.";
                Al.ShowMessage();
            }

            Barra.ShowControl = false;
            Barra.ShowClose();

            HabilitarButton(true, btnArchivo);
            HabilitarButton(true, btnEnviar);
            HabilitarButton(true, btnConfiguracion);
            HabilitarButton(true, btnSalir);

            if(statusOk)
                Barra.CloseForm();
        }

        //HABILITAR O DESHABILITAR
        private void HabilitarButton(bool pStatus, SimpleButton pButton)
        {
            if (this.InvokeRequired)
            {
                DisableButton disable = new DisableButton(HabilitarButton);

                //PARAMETROS
                object[] parameters = new object[] {pStatus, pButton };

                this.Invoke(disable, parameters);
            }
            else
            {
                pButton.Enabled = pStatus;
            }
        }
        #endregion

        public frmEnvioCorreo()
        {
            InitializeComponent();
        }

        private void btnConfiguracion_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmconfcorreo") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta función", "Acceso", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            frmConfCorreo correo = new frmConfCorreo();
            correo.StartPosition = FormStartPosition.CenterParent;
            correo.ShowDialog();
        }

        private void btnEnviar_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            if (txtCorreoDestino.Text == "")
            { lblMessage.Visible = true; lblMessage.Text = "Por favor ingresa el correo electronico del destinatario"; return; }

            if (txtTitulo.Text == "")
            { lblMessage.Visible = true; lblMessage.Text = "Por favor ingresa un título para el correo"; return; }

            if (txtMensaje.Text == "")
            {
                DialogResult Advertencia = XtraMessageBox.Show("¿Estás seguro de enviar el correo sin ningún mensaje?", "Mensaje", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (Advertencia == DialogResult.No)
                    return;
            }

            if (txtRutaAdjunto.Text == "")
            { lblMessage.Visible = true; lblMessage.Text = "Ruta de archivo no válida"; return; }

            if (File.Exists(txtRutaAdjunto.Text) == false)
            { lblMessage.Visible = true; lblMessage.Text = "Archivo no existe"; return; }

            lblMessage.Visible = false;           

            /*ENVIAMOS EL MAIL EN SEGUNDO PLANO*/
            ThreadStart delegado = new ThreadStart(EnviarCorreoBackGround);
            Thread Hilo = new Thread(delegado);
            Hilo.Name = "Email";
            Hilo.Start();
          

            lblMessage.Visible = false;
           

        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            Close();
        }

        private void btnAdjunto_Click(object sender, EventArgs e)
        {

        }

        private void cbOtroMail_CheckedChanged(object sender, EventArgs e)
        {
            if (cbOtroMail.Checked)
            {
                txtCorreoDestino.ReadOnly = false;
                txtCorreoDestino.Text = "";
                txtCorreoDestino.Focus();
            }
            else
            {                

                txtCorreoDestino.ReadOnly = true;
                txtCorreoDestino.Text = CorreoTrabajador;
                txtTitulo.Focus();
            }
        }

        private void frmEnvioCorreo_Load(object sender, EventArgs e)
        {
            //CARGAMOS CORREO SI ES QUE HAY
            if (RutTrabajador.Length > 0)
            {
                txtCorreoDestino.Text = Persona.GetMail(RutTrabajador);
                CorreoTrabajador = txtCorreoDestino.Text;
            }

            //INICIALIZAMOS BARRA DE PROGRESO
            Barra = new BarraProgreso(BarraProgresoCorreo, 1, false, false, this);

            Al = new Alerta();
            Al.Formulario = this;
            Al.AControl = new DevExpress.XtraBars.Alerter.AlertControl();
        }

        private void btnArchivo_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            OpenFileDialog abrir = new OpenFileDialog();
            abrir.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            abrir.Filter = "All files (*.*)|*.*";

            if (abrir.ShowDialog() == DialogResult.OK)
            {
                txtRutaAdjunto.Text = abrir.FileName;
            }
        }
    }
}