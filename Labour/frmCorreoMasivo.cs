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
using System.Data.SqlClient;
//HILOS
using System.Threading;

namespace Labour
{
    public partial class frmCorreoMasivo : DevExpress.XtraEditors.XtraForm, IConjuntosCondicionales
    {
       private List<Paquete> Paquetes = new List<Paquete>();

        private string UserFilter = User.GetUserFilter();
        private bool ShowPrivados = User.ShowPrivadas();
        string Option = "";

        BarraProgreso barra;

        delegate void ChangeTextBox(string pText);

        List<LiquidacionHistorico> Listado = new List<LiquidacionHistorico>();

        //PARA SABER SI HAY UN PROCESO EN SEGUNDO PLANO QUE AUN NO HA FINALIZADO
        private bool running { get; set; } = false;

        //PARA DESHABILITAR BOTONES DESDE OTRO HILO
        delegate void DisableButton(SimpleButton pButton, bool pOption);

        //CERRAR FORMULARIO
        delegate void CloseForm();

        #region "INTERFAZ"
        public void CargarCodigoConjunto(string code)
        {
            txtConjunto.Text = code;
        }
        #endregion

        public frmCorreoMasivo()
        {
            InitializeComponent();
        }

        #region "MANEJO DE DATOS"
        //SQL PARA LISTADO DE TRABAJADORES
        public string GetSql(string pConjunto)
        {
            string sql = "";
            string condicion = "", condCaja = "";            

            if (UserFilter != "0")
            {
                //USA FILTRO
                if (pConjunto != "0")
                {
                    condicion = Conjunto.GetCondicionFromCode(UserFilter);
                    condCaja = Conjunto.GetCondicionFromCode(pConjunto);
                    sql = "SELECT trabajador.contrato, rut, trabajador.anomes, mail FROM liquidacionhistorico " +
                          "INNER JOIN trabajador ON trabajador.contrato = liquidacionHistorico.contrato AND " +
                          "trabajador.anomes = liquidacionhistorico.anomes " +
                          "INNER JOIN calculoMensual ON calculomensual.contrato = trabajador.contrato AND " +
                          "calculoMensual.anomes = trabajador.anomes " +
                          $"WHERE trabajador.contrato IN(SELECT contrato FROM trabajador WHERE {condicion} AND {condCaja}) " +
                          $" {(ShowPrivados == false ? " AND privado = 0" : "")}" +
                          "ORDER BY trabajador.contrato, trabajador.anomes asc, trabajador.rut asc";
                }
                else
                {
                    condicion = Conjunto.GetCondicionFromCode(UserFilter);             
                    sql = "SELECT trabajador.contrato, rut, trabajador.anomes, mail FROM liquidacionhistorico " +
                          "INNER JOIN trabajador ON trabajador.contrato = liquidacionHistorico.contrato AND " +
                          "trabajador.anomes = liquidacionhistorico.anomes " +
                          "INNER JOIN calculoMensual ON calculomensual.contrato = trabajador.contrato AND " +
                          "calculoMensual.anomes = trabajador.anomes " +
                          $"WHERE trabajador.contrato IN(SELECT contrato FROM trabajador WHERE {condicion} " +
                          $" {(ShowPrivados == false ? " AND privado = 0" : "")}" +
                          "ORDER BY trabajador.contrato, trabajador.anomes asc, trabajador.rut asc";
                }                
            }
            else
            {
                //NO USA FILTRO
                if (pConjunto != "0")
                {
                    condCaja = Conjunto.GetCondicionFromCode(pConjunto);
                    sql = "SELECT trabajador.contrato, rut, trabajador.anomes, mail FROM liquidacionhistorico " +
                     "INNER JOIN trabajador ON trabajador.contrato = liquidacionHistorico.contrato AND " +
                     "trabajador.anomes = liquidacionhistorico.anomes " +
                     "INNER JOIN calculoMensual ON calculomensual.contrato = trabajador.contrato AND " +
                     "calculoMensual.anomes = trabajador.anomes " +
                     $"WHERE trabajador.contrato IN(SELECT contrato FROM trabajador WHERE {condCaja}) " +
                     $" {(ShowPrivados == false ? " AND privado = 0" : "")}" +
                     "ORDER BY trabajador.contrato, trabajador.anomes asc, trabajador.rut asc";
                }
                else
                {
                    sql = "SELECT trabajador.contrato, rut, trabajador.anomes, mail FROM liquidacionhistorico " +
                     "INNER JOIN trabajador ON trabajador.contrato = liquidacionHistorico.contrato AND " +
                     "trabajador.anomes = liquidacionhistorico.anomes " +
                     "INNER JOIN calculoMensual ON calculomensual.contrato = trabajador.contrato AND " +
                     "calculoMensual.anomes = trabajador.anomes " +
                     $" {(ShowPrivados == false? "WHERE privado = 0":"")}" +
                     "ORDER BY trabajador.contrato, trabajador.anomes asc, trabajador.rut asc";
                }               
            }

            return sql;
        }

        //PROCESO EN SEGUNDO PLANO 
        private void GeneraArchivo()
        {
            barra.ShowControl = true;
            barra.Begin();

            //PARA SABER SI HAY UN PROCESO EN SEGUNDO PLANO CORRIENDO
            running = true;

            //DESHABILITAMOS BOTONES
            DeshabilitaBoton(btnConfiguracion, false);
            DeshabilitaBoton(btnConjunto, false);
            DeshabilitaBoton(btnEnviar, false);
            DeshabilitaBoton(btnSalir, false);

            //LIMPIAMOS LISTA
            Listado.Clear();            

            int x = 0;
            while (x <70)
            {
                barra.Increase();
                x++;
            }

            //GENERAMOS ARCHIVOS PARA TODOS LOS TRABAJADORES
            Listado = Persona.ListadoLiquidaciones(GetSql(Option));

            //barra.Increase();
            if (Listado.Count > 0)
            {
                Paquetes = Persona.GeneraPdfLiquidacionesTodos(Listado, true);
                //if (Paquetes.Count > 0)
                    //ChangeText(Listado.Count.ToString());
                //else
                //    ChangeText("0");
            }

            x = 0;
            while (x < 30)
            {
                barra.Increase();
                x++;
           }

            barra.ShowControl = false;
            barra.ShowClose();

            DeshabilitaBoton(btnConfiguracion, true);
            DeshabilitaBoton(btnConjunto, true);
            DeshabilitaBoton(btnEnviar, true);
            DeshabilitaBoton(btnSalir, true);
            running = false;

            if (Paquetes.Count > 0)
            { XtraMessageBox.Show("Liquidaciones generadas correctamente", "Liquidaciones", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
            else
                XtraMessageBox.Show("No se pudieran generar los archivos", "Liquidaciones", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        //PROCESO EN SEGUNDO PLANO 
        private void SendMail()
        {
            bool correcto = false;
            barra.ShowControl = true;
            barra.Begin();

            running = true;
            DeshabilitaBoton(btnConfiguracion, false);
            DeshabilitaBoton(btnConjunto, false);
            DeshabilitaBoton(btnEnviar, false);
            DeshabilitaBoton(btnSalir, false);

            int x = 0;
            while (x < 70)
            {
                barra.Increase();
                x++;
            }

            Listado = Persona.ListadoLiquidaciones(GetSql(Option));
            if (Listado.Count > 0)
            {
                Paquetes = Persona.GeneraPdfLiquidacionesTodos(Listado, true);
                //PROCESO...
                //ENVIAMOS CORREOS...
                if (Paquetes.Count > 0)
                {
                    foreach (var paquete in Paquetes)
                    {
                        if (paquete.Destinatario != "")
                        {
                            //CADA NOMBRE DE ARCHIVO TIENE ASOCIADO EL RUT DE LA PERSONA                    
                            Mail Email = new Mail();
                            Email.SetConfiguration();

                            if (Email.ServerSmtp == "" || Email.PasswordServer == "" || Email.EmailServer == "")
                            { XtraMessageBox.Show("La configuracion del servidor no es válida", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                            //MENSAJE CORREO
                            Email.Message = txtMessage.Text;

                            //TITULO CORREO ELECTRONICO
                            Email.TitleMessage = txtTitle.Text;

                            //MAIL DESTINATARIO                    
                            Email.ClientEmail = paquete.Destinatario;

                            //...
                            Email.pRutaArchivo = paquete.Ruta;

                            //ENVIAR    
                            correcto = Email.SendMail();
                        }
                    }
                }
                else
                {
                    XtraMessageBox.Show("No se pudieron enviar los registros", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
            }
            else
            {
                XtraMessageBox.Show("No se encontraron registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            x = 0;
            while (x < 30)
            {
                barra.Increase();
                x++;
            }

            barra.ShowControl = false;
            barra.ShowClose();

            //HABILITAR BOTONES
            DeshabilitaBoton(btnConfiguracion, true);
            DeshabilitaBoton(btnConjunto, true);
            DeshabilitaBoton(btnEnviar, true);
            DeshabilitaBoton(btnSalir, true);
            running = false;

            if (correcto)
            {
                XtraMessageBox.Show("Correos enviados correctamente", "Correo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CerrarFormulario();
            }                
            else
            {
                XtraMessageBox.Show("No se lograron enviar los correos", "Correo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            
        }

        private async Task SendMailAsync()
        {
            await Task.Run(() => {
                //CODE...
                bool correcto = false;
                barra.ShowControl = true;
                barra.Begin();

                running = true;
                DeshabilitaBoton(btnConfiguracion, false);
                DeshabilitaBoton(btnConjunto, false);
                DeshabilitaBoton(btnEnviar, false);
                DeshabilitaBoton(btnSalir, false);

                int x = 0;
                while (x < 70)
                {
                    barra.Increase();
                    x++;
                }

                Listado = Persona.ListadoLiquidaciones(GetSql(Option));
                if (Listado.Count > 0)
                {
                    Paquetes = Persona.GeneraPdfLiquidacionesTodos(Listado, true);
                    //PROCESO...
                    //ENVIAMOS CORREOS...
                    if (Paquetes.Count > 0)
                    {
                        foreach (var paquete in Paquetes)
                        {
                            if (paquete.Destinatario != "")
                            {
                                //CADA NOMBRE DE ARCHIVO TIENE ASOCIADO EL RUT DE LA PERSONA                    
                                Mail Email = new Mail();
                                Email.SetConfiguration();

                                //MENSAJE CORREO
                                Email.Message = txtMessage.Text;

                                //TITULO CORREO ELECTRONICO
                                Email.TitleMessage = txtTitle.Text;

                                //MAIL DESTINATARIO                    
                                Email.ClientEmail = paquete.Destinatario;

                                //...
                                Email.pRutaArchivo = paquete.Ruta;

                                //ENVIAR    
                                correcto = Email.SendMail();
                            }
                        }
                    }
                    else
                    {
                        XtraMessageBox.Show("No se pudieron enviar los registros", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }
                }
                else
                {
                    XtraMessageBox.Show("No se encontraron registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }

                x = 0;
                while (x < 30)
                {
                    barra.Increase();
                    x++;
                }

                barra.ShowControl = false;
                barra.ShowClose();

                //HABILITAR BOTONES
                DeshabilitaBoton(btnConfiguracion, true);
                DeshabilitaBoton(btnConjunto, true);
                DeshabilitaBoton(btnEnviar, true);
                DeshabilitaBoton(btnSalir, true);
                running = false;

                if (correcto)
                {
                    XtraMessageBox.Show("Correos enviados correctamente", "Correo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CerrarFormulario();
                }
                else
                {
                    XtraMessageBox.Show("No se lograron enviar los correos", "Correo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });
        }

        //DESHABILITAR BOTON
        private void DeshabilitaBoton(SimpleButton pButton, bool pOption)
        {
            if (this.InvokeRequired)
            {
                DisableButton des = new DisableButton(DeshabilitaBoton);

                //PARAMETROS
                object[] parameters = new object[] { pButton, pOption };

                this.Invoke(des, parameters);
            }
            else
            {
                pButton.Enabled = pOption;
            }
        }

        private void CerrarFormulario()
        {
            if (this.InvokeRequired)
            {
                CloseForm close = new CloseForm(CerrarFormulario);

                this.Invoke(close);
            }
            else
            {
                Close();
            }
        }



        #endregion

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void btnConjunto_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            //AL HACER DOBLE CLICK LANZAMOS EL FORMULARIO DE EXPRESION 
            if (cbTodos.Checked == false)
            {
                FrmFiltroBusqueda filtro = new FrmFiltroBusqueda(true);
                filtro.StartPosition = FormStartPosition.CenterParent;
                filtro.opener = this;
                filtro.ShowDialog();
            }
        }

        private void cbTodos_CheckedChanged(object sender, EventArgs e)
        {
            if (cbTodos.Checked)
            {
                txtConjunto.Text = "";
                txtConjunto.ReadOnly = true;
            }
            else
            {
                txtConjunto.Text = "";
                txtConjunto.ReadOnly = false;
                txtConjunto.Focus();
            }
        }



        private void btnEnviar_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (cbTodos.Checked)
            {
                Option = "0";

                //VERIFICAMOS QUE LA CONFIGURACION SEA VÁLIDA
                Mail email = new Mail();
                email.SetConfiguration();

                if (email.ServerSmtp == "" || email.EmailServer == "" || email.PasswordServer == "" || email.PuertoSmtp == 0)
                { XtraMessageBox.Show("La configuracion del servidor no es correcta, \npor favor verifica la información y vuelve a intentarlo", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                ThreadStart delegado = new ThreadStart(SendMail);
                Thread proc = new Thread(delegado);
                proc.Name = "proceso";                
                proc.Start();

                //Paquetes = Persona.GetListadoAsync(GetSql(Option)).Result;      
                
            }
            else
            {
                if (txtConjunto.Text == "")
                { XtraMessageBox.Show("Por favor ingresa un conjunto a evaluar", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                if (Conjunto.ExisteConjunto(txtConjunto.Text) == false)
                { XtraMessageBox.Show("Por favor ingresa un conjunto válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                Option = txtConjunto.Text;

                //VERIFICAMOS QUE LA CONFIGURACION SEA VÁLIDA
                Mail email = new Mail();
                email.SetConfiguration();

                if (email.ServerSmtp == "" || email.EmailServer == "" || email.PasswordServer == "" || email.PuertoSmtp == 0)
                { XtraMessageBox.Show("La configuracion del servidor no es correcta, \npor favor verifica la información y vuelve a intentarlo", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                ThreadStart delegado = new ThreadStart(SendMail);
                Thread proc = new Thread(delegado);
                proc.Name = "proceso";
                //proc.IsBackground = true;
                proc.Start();
            }             
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

        private void frmCorreoMasivo_Load(object sender, EventArgs e)
        {
            barra = new BarraProgreso(BarraProceso, 1, false, false, this);
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            //NUEVA SESION
            Sesion.NuevaActividad();

            Close();
        }

        private void frmCorreoMasivo_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (running)
            {
                DialogResult Advertencia = XtraMessageBox.Show("Hay un proceso que aún no ha finalizado", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                e.Cancel = true;
            }
        }
    }
}