using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.XtraEditors;
using System.Data.SqlClient;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Timers;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraBars.Alerter;
using DevExpress.XtraReports.UI;

namespace Labour
{
    public partial class frmAcceso : DevExpress.XtraEditors.XtraForm, Iform, ILicenciaAcceso, IUpdateComboDatos
    {
        //VARIABLE PARA GUARDAR EL ULTIMO PERIODO QUE ESTA EN LA BASE DE DATOS (TABLA PARAMETROS)
        private int UltimoPeriodo = 0;

        //VARIABLE PARA GUARDAR EL PERIODO EN CURSO DE ACUERDO A FECHA
        private int PeriodoEnCurso = 0;

        public IMenu open { get; set; }        

        //PARA TENER CONTROL DE VERSION DEL SISTEMA
        //------------------------------------------->
        //private string version { get; set; } = "0.001";
        //------------------------------------------->

        //TIMER PARA SESIONES
        private System.Windows.Forms.Timer Tmlast;

        /// <summary>
        /// Solo para monitorear procesos activos
        /// </summary>
        private System.Windows.Forms.Timer TimerProcesos;

        //PARA SABER SI HIZO CLICK EN BOTON SALIR Y SELECCIONÓ LA OPCION NO
        public static bool Cierra { get; set; }

        /// <summary>
        /// Para barra de estado, mensajes.
        /// </summary>
        public IMainChildInterface OpenStatus  { get; set; }

        public IMonitor MonitorInterface { get; set; }
        
        /// <summary>
        /// Representa un control AlertControl.
        /// </summary>
        private Alerta Al { get; set; }

        IndiceMensual indice = new IndiceMensual();
        //Contador de segundos.
        private int TimeC = 0;

        DateTime FechaExpiracion = DateTime.Now.Date;

        #region "METODOS INTERFAZ"
        public void CargarCajas(string txt1, string txt2, string txt3)
        {
            cbDatos.EditValue = txt3;
        }

        public void CargarCombo()
        {
            ComboLite.CargarCombo(cbDatos, "SELECT nombre from datos");
        }
        #endregion

        #region "INTERFAZ LICENCIA"
        public void CloseForms()
        {
            MdiParent.Close();
            Close();
        }
        #endregion

        //INTERFAZ DE COMUNICACION ENTRE ESTE FORMULARIO Y EL FORMULARIO DE INGRESO DE JUEGOS DE DATOS
        #region "INTERFAZ JUEGO DE DATOS"
        public void RecargarCombo()
        {
            //CARGAR LOOK UP EDIT CON JUEGOS DE DATOS
            ComboLite.CargarCombo(cbDatos, "SELECT nombre From datos");
        }

        #endregion
        public frmAcceso()
        {
            InitializeComponent();            
        }

        private void frmAcceso_Load(object sender, EventArgs e)
        {
            //PERIODO EN CURSO
            //SI ESTA EN TRUE DETENEMOS EL TIMER  

            //PARA INICIALIZAR EL TEMPORIZADOR DE FORMA MANUAL          

            SkinManager.EnableFormSkins();
            SkinManager.EnableMdiFormSkins();

            txtuser.Properties.MaxLength = 40;
            txtpass.Properties.MaxLength = 40;
            this.txtuser.Focus();

            //CARGAR LOOK UP EDIT CON JUEGOS DE DATOS
            ComboLite.CargarCombo(cbDatos, "SELECT nombre From datos");

            Al = new Alerta();
            Al.AControl = new AlertControl();            
            Al.Formulario = this;
            
        }    

        private void cbDatos_DoubleClick(object sender, EventArgs e)
        {
            //MOSTRAR FORMULARIO CON TABLA JUEGO DATOS!
            try
            {
                ComboBoxItemCollection collection = cbDatos.Properties.Items;
                if (collection.Count > 0)
                {                   
                     frmDatosUpdate update = new frmDatosUpdate();
                     //update.MdiParent = this.MdiParent;
                     update.Opener = this;
                     update.OpenerUpdateJuegoDatos = this;
                     update.Show();
                }
               
            }
            catch (Exception ex)
            {
              //ERROR...
            }
        }

        private void groupControl1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            //CERRAR PROGRAMA     
            //DEJAR EN TRUE 
            Licencia.ForzarCierre = true;        
            MdiParent.Close();

            //SELECCIONO LA OPCION NO CIERRA
            if(Cierra)
                this.Close();
        }

        private void btnIngreso_Click(object sender, EventArgs e)
        {
            if (cbDatos.SelectedItem == null)
            {
                XtraMessageBox.Show("Selecciona un juego de datos", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);

                Al.Mensaje = "Juego de datos no válido";
                Al.ShowMessage();
                return;
            }

            //GUARDAR EL VALOR DEL COMBOBOX
            fnSistema.ConfigName = cbDatos.EditValue.ToString();
            Cifrado cif = new Cifrado();
            string vsBd = "";
            //llamamos a la funcion para setear la configuracion para la base de datos
            string[] configuracion = new string[4];
            configuracion = SqlLite.fnBuscarConfig(fnSistema.ConfigName);
            fnSistema.pgServer = configuracion[0];
            fnSistema.pgDatabase = configuracion[1];
            fnSistema.pgUser = configuracion[2];
            fnSistema.pgPass = cif.DesencriptaTripleDesc(configuracion[3]);

            //USAMOS EL MISMO SERVERNAME QUE EL JUEGO DE DATOS SELECCIONADO
            Licencia.ServerName = configuracion[0];
            //USAMOS LA MISMA CLAVE
            Licencia.Password = cif.DesencriptaTripleDesc(configuracion[3]);
            Licencia.UserId = fnSistema.pgUser;

            //GUARDAR LAS CREDENCIALES PARA CONEXION
            if (txtpass.Text == "" && txtuser.Text == "")
            {
                XtraMessageBox.Show("Credenciales no validas", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                txtuser.Focus();
                return;
            }
            else
            {          
                splashScreenManager1.ShowWaitForm();
           
                if (fnSistema.ConectarSQLServer() == false || Licencia.NuevaConexion() == false)
                {            
                    splashScreenManager1.CloseWaitForm();
                    XtraMessageBox.Show("No hay conexion con la base de datos, Verifique configuración", "Error de Conexion", MessageBoxButtons.OK, MessageBoxIcon.Stop);

                    Al.Mensaje = "Hay un problema de conexion con la base de datos.";
                    Al.ShowMessage();
                    return;
                }
                else
                {
                    //SI LA CONEXION ESTÁ ABIERTA LA CERRAMOS
                    if(fnSistema.sqlConn.State == ConnectionState.Open)
                        fnSistema.sqlConn.Close();

                    if(Licencia.Lconexion.State == ConnectionState.Open)
                        Licencia.Lconexion.Close();

                    //CONEXION VALIDA                
                    //VALIDAR CODIGO DE VALIDACION
                    Empresa emp = new Empresa();

                    //OBTENER EL NUMERO DE VERSION...
                    vsBd = emp.GetCodvs();

                    if (vsBd != fnSistema.VersionSistema)
                    {
                        splashScreenManager1.CloseWaitForm();
                        XtraMessageBox.Show("Parece ser que la version que estás usando es antigua, actualiza el software y vuelve a intentarlo", "Version Obsoleta", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        Al.Mensaje = "Version obsoleta.\nPor favor comuniquese con soporte.";
                        Al.ShowMessage();
                        //User.ChangeStatusUser(0, txtuser.Text);       
                        Licencia.ForzarCierre = true;
                        this.MdiParent.Close();
                        return;
                    }                                     
                    
                    //ESTO ES VALIDO PARA LA LICENCIA POR EMPRESA (JUEGO DE DATOS)
                    if (emp.HayRegistros())
                    {
                        //SETEAMOS LAS VARIABLES
                        emp.SetInfo();
                        //QUE BASE DE DATOS SE USARÁ COMO PARAMETRO PARA OBTENER LOS VALORES DE LA TABLA EMPRESA               
                        
                        if (emp.CodValCorrecto(emp.GeneraCodValidacion()) == false)
                        {
                            XtraMessageBox.Show("Error en codigo de validacion sistema", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            splashScreenManager1.CloseWaitForm();
                            return;
                        }
                    }
                    else
                    {
                        XtraMessageBox.Show("Por favor ingrese los datos de su empresa antes de comenzar a trabajar", "Informacion Importante", MessageBoxButtons.OK, MessageBoxIcon.Information);                                                   
                    }

                    //GENERAMOS LICENCIA PARA VER SI CONCUERDA CON LA LICENCIA GUARDADA EN TABLA 
                    string LicenciaUsuario = Licencia.GenerarLicencia();                    
                    //string LicenciaBd = Licencia.GetLicenciaSinUsers(Licencia.GetLicencia());
                  
                   //BASE DE DATOS PARAMETRIZADA EXISTE EN SERVER...
                   //TABLA EMPRESA NO TIENE DATOS?

                   //HAY DATOS
                   if (Licencia.ExisteLicencia())
                      {
                          //GENERAMOS LICENCIA...                             
                         if (Licencia.GetLicencia().Length > 0)
                            {
                                 //SI ES QUE HAY DATOS VERIFICAR SI ES UN CODIGO VALIDO
                                if ((Licencia.GetLicenciaSinUsers(Licencia.GetLicencia()) != Licencia.GenerarLicencia()) || cif.GetUserCount(Licencia.GetLicencia()) == 0)
                                   {
                                    //SI LA LICENCIA NO ES VALIDA MOSTRAR VENTANA DONDE SE TENGA QUE REINGRESAR LA LICENCIA                                   
                                    splashScreenManager1.CloseWaitForm();
                                    //XtraMessageBox.Show("Licencia no válida", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                    ShowLicencia();
                                    return;
                                   }
                             }
                             else
                             {
                                 //SI ES VACÍO, AGREGAMOS...
                                 //Licencia.IngresaLicencia(LicenciaUsuario);
                                 splashScreenManager1.CloseWaitForm();
                                 ShowLicencia();
                                 return;
                             }
                      }
                      else
                      {
                       //NO HAY REGISTRO EN TABLA DE LICENCIA...
                       splashScreenManager1.CloseWaitForm();
                       ShowLicencia();
                       return;
                      }


                    FechaExpiracion = Licencia.GetExpiration();
                    if (FechaExpiracion < DateTime.Now.Date)
                    { XtraMessageBox.Show("La licencia a expirado, por favor comunicate con servicio técnico para obtener una nueva.", "Licencia caducada", MessageBoxButtons.OK, MessageBoxIcon.Stop); splashScreenManager1.CloseWaitForm(); return; }
                    

                    //VALIDAR USUARIO
                    bool usuario = false;
                    usuario = fnUserValido(txtuser.Text, txtpass.Text);
                    if (usuario)
                    {
                        //if (!Licencia.ExisteBd())
                        //{ splashScreenManager1.CloseWaitForm(); XtraMessageBox.Show("Error con la base de datos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                        //SETEAMOS USUARIOS
                        User.SetUser(txtuser.Text.ToUpper());

                        int count = 0;
                        count = cif.GetUserCount(Licencia.GetLicencia());

                        if (Sesion.UserLimit(count, fnSistema.pgDatabase))
                        {
                            splashScreenManager1.CloseWaitForm();
                            XtraMessageBox.Show("No quedan sesiones disponibles, intentelo mas tarde", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            Licencia.ForzarCierre = true; this.MdiParent.Close();

                            Al.Mensaje = "No hay sesiones disponibles";
                            Al.ShowMessage();
                            return; }

                        //VERIFICAR SI EL USUARIO TIENE SESION ABIERTA
                        if (Sesion.Abierta(User.getUser().ToLower(), fnSistema.pgDatabase))
                        {
                            splashScreenManager1.CloseWaitForm();
                            XtraMessageBox.Show("Usuario con sesion abierta", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); 
                            //DESEA CERRAR LA SESION?
                            DialogResult Pregunta = XtraMessageBox.Show("¿Deseas cerrar la sesion del usuario " + User.getUser() + "?", "Cerrar Sesión", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (Pregunta == DialogResult.Yes)
                            {
                                if (Sesion.DeleteAccess(User.getUser().ToLower()))
                                {
                                    XtraMessageBox.Show("Se ha cerrado exitosamente la sesion", "Sesion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                else
                                {
                                    XtraMessageBox.Show("No se pudo cerrar la sesion del usuario " + User.getUser(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                }
                            }
                            //this.MdiParent.Close();
                            return;
                        }

                        //INICIALIZAMOS LAS VARIABLES DE SISTEMA
                        List<varSistema> lista = new List<varSistema>();
                        lista = ListadoVariables();
                        varSistema.CargarListadoVariables(lista);

                        Configuracion.SetGlobalConfiguration();

                        //VARIABLE GLOBAL PARA SABER EL PERIODO QUE SE ESTA EVALUANDO...
                        if (Calculo.ExisteParametro())
                            Calculo.PeriodoObservado = Calculo.PeriodoEvaluar();
                        else
                        {
                            splashScreenManager1.CloseWaitForm();
                            XtraMessageBox.Show("No hay ningun periodo ingresado", "Informacion Importante", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            return;
                        }                         

                        if (Calculo.PeriodoObservado == 0)
                        { splashScreenManager1.CloseWaitForm(); XtraMessageBox.Show("No se encontró ningún periodo", "Periodo", MessageBoxButtons.OK, MessageBoxIcon.Stop); Licencia.ForzarCierre = true; this.MdiParent.Close(); return; }

                        //fnSistema.IniciarServicio("myservice01");

                        //USUARIO EXISTE
                        //Se precarga un reporte para acelerar el ReportDesigner
                        //DiseñadorReportes.PreCargarDependenciasDiseñador();

                        //SI SON CORRECTAS MOSTRAMOS MENUS   
                        this.Close();
                        this.open.ShowMenu();
                        this.open.CargarUser("User: " + User.NombreCompleto(), "BD: " + configuracion[1], fnSistema.PrimerMayuscula(fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(Calculo.PeriodoObservado))));
                        Sesion.Activa = true;

                        //SETEAMOS SPID SESION                        
                        Sesion.SetSpid();

                        //GUARDAR DATOS DE SESION     
                        // ThreadStart delegado = new ThreadStart(SesionThread);
                        // Thread Hilo = new Thread(delegado);
                        // Hilo.Start();
                        if(User.getUser().ToLower() != "super")
                            Sesion.NewAccess(User.getUser().ToLower(), User.GetOperatingSystem(), User.GetIpUser(), fnSistema.pgDatabase);

                        //Sesion.GetInformationUserSession(User.getUser(), fnSistema.pgDatabase);                  

                        //SE EJECUTA CADA 5 SEGUNDOS...
                        if (User.getUser().ToLower() != "super")
                        {
                            Tmlast = new System.Windows.Forms.Timer();
                            Tmlast.Interval = 5000;
                            Tmlast.Tick += new EventHandler(Tmlast_Tick);
                            Tmlast.Start();
                        }

                        //ACTIVAMOS TIMER PARA MONITOREAR PROCESOS
                        TimerProcesos = new System.Windows.Forms.Timer();
                        TimerProcesos.Interval = 1000;
                        TimerProcesos.Tick += TimerProcesos_Tick;
                        TimerProcesos.Start();
                        Alerta.StatusMonitor = TimerProcesos;
                    }
                    else
                    {
                        splashScreenManager1.CloseWaitForm();
                        XtraMessageBox.Show("Usuario o contraseña desconocido", "Credenciales", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtpass.Focus();

                        Al.Mensaje = "Hay un problema de inicio de sesión.\nPor favor verifique la información ingresada.";
                        Al.ShowMessage();
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Entrega informacion de algun proceso activo.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerProcesos_Tick(object sender, EventArgs e)
        {
            Tarea task1 = new Tarea();
            Tarea task2 = new Tarea();
            Tarea task3 = new Tarea();

            try
            {
                Monitor.CleanTaskColgadas();

                if (TimeC == 0)
                    indice.SetInfoMes(Calculo.PeriodoObservado);

                //VERIFICAR SI UN USUARIO ESTA REALIZANDO UN BLOQUEO
                string bloq = User.GetNombreBloqueo();

                //Contamos un segundo.
                TimeC++;

                task1 = Monitor.GetProcesoActivo("001");
                if (task1 != null)
                {
                    if (task1.Activo == 1)
                    {
                        if (OpenStatus != null)
                        {
                            if (task1.User.ToLower() != User.getUser().ToLower())
                                OpenStatus.ChangeStatus($"<color=Green>Usuario {task1.User} está ejecutando {task1.DescTarea}...</color>");
                        }
                    }
                }

                task2 = Monitor.GetProcesoActivo("002");
                if (task2 != null)
                {
                    if (task2.Activo == 1)
                    {
                        if (OpenStatus != null)
                        {
                            if (task2.User.ToLower() != User.getUser().ToLower())
                                OpenStatus.ChangeStatus($"<color=Green>Usuario {task2.User} está ejecutando {task2.DescTarea}...</color>");
                        }
                    }
                }

                task3 = Monitor.GetProcesoActivo("003");
                if (task3 != null)
                {
                    if (task3.Activo == 1)
                    {
                        if (OpenStatus != null)
                        {
                            if (task3.User.ToLower() != User.getUser().ToLower())
                                OpenStatus.ChangeStatus($"<color=Green>Usuario {task3.User} está ejecutando {task3.DescTarea}...</color>");
                        }
                    }

                }

                if (task1 == null && task2 == null && task3 == null)
                {
                    if (OpenStatus != null)
                        OpenStatus.ChangeStatus("");
                }

                if (task1.Activo == 0 && task2.Activo == 0 && task3.Activo == 0)
                {
                    if (OpenStatus != null)
                        OpenStatus.ChangeStatus("");
                }

                if (indice.IngresoMinimo == 0 || indice.Sanna == 0 || indice.Sis == 0 || indice.Uf == 0 ||
                    indice.Utm == 0 || indice.TopeAfp == 0 || indice.TopeSec == 0 || indice.TopeIps == 0)
                {
                    if (TimeC == 30)
                    {
                        Al.Mensaje = "Recuerda ingresar los indices previsionales en el menú correspondiente.";
                        Al.ShowMessage();

                        TimeC = 0;
                    }
                }

                if (bloq != "")
                {
                    if (OpenStatus != null)
                        OpenStatus.ShowBloqueo("<color=#a31026>Usuario " + bloq + " está realizando un bloqueo de procesos.</color>", true);

                    if (TimeC == 0)
                    {
                        Al.Mensaje = "Cuando un usuario realiza un bloqueo, los demás usuarios no pueden realizar modificaciones.";
                        Al.ShowMessage();
                    }
                }
                else
                {
                    if (OpenStatus != null)
                        OpenStatus.ShowBloqueo(bloq, false);
                }
            }
            catch (Exception ex)
            {
                /*Error...*/
            }
        }

        //SE EJECUTA CADA X SEG
        private void Tmlast_Tick(object sender, EventArgs e)
        {
            //OBTENER LA HORA DE LA ULTIMA CONSULTA REALIZADA A LA BASE DE DATOS
            Sesion.TiempoUltimaActividad = Sesion.LastRequestUserAc();

            //HORA ACTUAL
            DateTime CurrentTime = DateTime.Now;

            //SABER CUANTO TIEMPO HA TRANSCURRIDO ENTRE LA ULTIMA REQUEST Y LA HORA ACTUAL
            TimeSpan Diferencia = CurrentTime.Subtract(Sesion.TiempoUltimaActividad);

            double minutes = Diferencia.TotalMinutes;

            //if (Sesion.ExisteSpid(Sesion.SpidSesion) == false)
            //    minutes = 0;

            //SI CLOSE ES FALSE ES PORQUE EL FORMULARIO ACCESO NO SE HA REABIERTO
            if (minutes > 30)
            {
                //SOLO SI ES LA PRIMERA VEZ...
                //CERRAMOS LA SESION (ELIMINANDO REGISTRO EN TABLA)
                //CERRAMOS TODAS LAS VENTANAS QUE ESTEN ABIERTAS Y VOLVEMOS A MOSTRAR LA VENTANA DE INICIO DE SESION...                             

                Tmlast.Stop();

                //ELIMINAR LA SESION DE TABLA ACCESS
                if (Sesion.Activa)
                {
                    Sesion.DeleteAccess(User.getUser().ToLower());
                    //Sesion.Kill(Sesion.SpidSesion);                    
                }

                //REABRIR FORMULARIO ACCESO
                this.open.OpenForm();
            }
        }

        private void ShowLicencia()
        {
            FrmConfiguracionLicencia Lic = new FrmConfiguracionLicencia();
            Lic.StartPosition = FormStartPosition.CenterParent;
            Lic.openLicencia = this;
            Lic.ShowDialog();
        }

        private void txtuser_KeyPress(object sender, KeyPressEventArgs e)
        {
            //NO PERMITIR VALORES RAROS A EXCEPCION DEL '.' '-' '_'
            if (char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (char.IsLetter(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (e.KeyChar == (char)32)
            {
                e.Handled = false;
            }
            else if (e.KeyChar == (char)46)
            {
                e.Handled = false;
            }
            else if (e.KeyChar == (char)45)
            {
                e.Handled = false;
            }
            else if (e.KeyChar == (char)95)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void btnBD_Click(object sender, EventArgs e)
        {         

            //MOSTRAR FORMULARIO CON TABLA JUEGO DATOS!
            try
            {
                ComboBoxItemCollection collection = cbDatos.Properties.Items;
                if (collection.Count > 0)
                {

                    frmDatosUpdate update = new frmDatosUpdate();
                    //update.MdiParent = this.MdiParent;
                    update.Opener = this;
                    update.OpenerUpdateJuegoDatos = this;
                    update.Show();

                }

            }
            catch (Exception ex)
            {
                //ERROR...
            }

        }    

     

        //UPDATE PERIODO
        private void NuevoPeriodo(int periodo)
        {
            string sql = "INSERT INTO PARAMETRO(anomes) VALUES(@periodo)";
            SqlCommand cmd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@periodo", periodo));

                        cmd.ExecuteNonQuery();
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

        //METODO PARA CREAR UNA LISTA PRECARGADA CON TODOS LOS ITEM SISTEMA (TABLA VARIABLES SISTEMA)
        private List<varSistema> ListadoVariables()
        {
            List<varSistema> sis = new List<varSistema>();
            string sql = "SELECT codvariable FROM variablesistema";
            SqlCommand cmd;
            SqlDataReader rd;
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
                                sis.Add(new varSistema() { nombre = (string)rd["codvariable"], valor = 0 });
                            }
                        }
                    }
                    //LIBERAR RECURSOS
                    cmd.Dispose();
                    rd.Close();
                    fnSistema.sqlConn.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return sis;

        }

        //PARA OBTENER EL NUMERO DE VERSION
        private string getVersion()
        {
            string vs = "";
            string sql = "SELECT cod FROM version";
            SqlCommand cmd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        //cmd.Parameters.Add(new SqlParameter("@pPeriodo", Calculo.PeriodoObservado));
                        vs = (string)cmd.ExecuteScalar();

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();

                        return vs;
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return vs;

        }

        //VERIFICAR SI SE HAN INGRESADO LOS DATOS DE LA EMPRESA
        private bool IngresaEmpresa()
        {
            bool ingresa = false;
            string sql = "SELECT count(*) FROM empresa";
            SqlCommand cmd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                            ingresa = true;

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
            return ingresa;
        }
        
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

        private void SesionThread()
        {
            //GUARDAMOS NUEVA SESION
            Sesion.NewAccess(User.getUser(), User.GetOperatingSystem(), User.GetIpUser(), fnSistema.pgDatabase);
        }

        #region "LOGIN"
        //VALIDAR QUE USUARIO EXISTA
        private bool fnUserValido(string pUser, string pClave)
        {
            string sql = "SELECT usuario, password FROM usuario WHERE usuario=@pUser AND password=@pClave";
            SqlCommand cmd;
            SqlDataReader rd;
            bool valido = false;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //ENCRIPTAR PRIMERAMENTE EL PASSWORD ANTES DE HACER CONSULTA
                        string hash = "";
                        Encriptacion enc = new Encriptacion();
                        hash = enc.EncodePassword(pClave);
                        
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pUser", pUser));
                        cmd.Parameters.Add(new SqlParameter("@pClave", hash));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //USUARIO EXISTE!!!
                            valido = true;
                        }
                        else
                        {
                            valido = false;
                        }
                    }
                    //LIBERAR RECURSOS
                    cmd.Dispose();
                    rd.Close();
                    fnSistema.sqlConn.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return valido;
        }

        #endregion

        private void cbDatos_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void frmAcceso_MouseClick(object sender, MouseEventArgs e)
        {
            
        }

        private void txtpass_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)32)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txtuser_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtpass_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            Close();
        }
    }
}
