using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.Skins;
using System.Data.SQLite;
using System.Data.SqlClient;
using DevExpress.LookAndFeel;
using DevExpress.XtraBars;
using System.IO;
using System.Globalization;
using System.Threading;

namespace Labour
{
    public partial class frmMain : DevExpress.XtraEditors.XtraForm, IMenu, IMainChildInterface
    {
        //VARIABLE PARA ALMACENAR EL NOMBRE DEL USUARIO
        public static string NombreUsuario = "";

        //PARA SABER SI SE PRESIONÓ LA COMBINACION DE TECLAS ALT-F4
        private bool Alt_F4 = false;

        //PARA CUANDO SE CIERRA EL BOTON SALIR EN FRM ACCESO (NO ELIMINA SESION)
        public static bool SendClose { get; set; } = false;

        public static Form Mdi { get; set; }

        delegate void DelegateOpenForm();

        //USUARIO OUEDE VER FICHAS PRIVADAS
        public bool ShowPrivadas { get; set; } = false;

        #region "INTERFAZ MENU"
        public void ShowMenu()
        {
            //HABILITAR LOS MENÚ (SEAN VISIBLES)
            //barSuperior.Visible = true;
            //barInferior.Visible = true;

            Bars barras = barManager1.Bars;
            foreach (Bar item in barras)
            {
                item.Visible = true;
            }
        }

        public void CargarUser(string user, string DataBase, string pPeriodo)
        {
            barUser.Caption = user;
            barBaseDatos.Caption = DataBase;
            timer1.Start();
            barHora.Caption = DateTime.Now.Date.ToLongTimeString();
            NombreUsuario = user;
            barPeriodoEvaluado.Caption = pPeriodo;
            InfoEmpresa();
            barProcesos.Caption = "";            
        }

        public void OpenForm()
        {
            if (this.InvokeRequired)
            {
                DelegateOpenForm open = new DelegateOpenForm(OpenForm);
                this.Invoke(open);
            }
            else
            {
                if (Sesion.Activa)
                {
                    CloseAllActiveForms();

                    Bars barras = barManager1.Bars;
                    foreach (Bar item in barras)
                    {
                        item.Visible = false;
                    }

                    barSuperior.Reset();
                    //barSuperior.Visible = false;
                    //barInferior.Visible = false;
                    Sesion.Activa = false;

                    frmAcceso ac = new frmAcceso();
                    ac.MdiParent = this;
                    ac.open = this;
                    ac.StartPosition = FormStartPosition.CenterScreen;
                    ac.Show();
                }
                
            }
        }

        #endregion

        #region "INTERFAZ STATUS PROCESO"
        public void ChangeStatus(string pNewStatus)
        {
            barProcesos.Caption = pNewStatus;
           // Image gl = DevExpress.Images.ImageResourceCache.Default.GetImage("loading.gif");
           // barProcesos.Glyph = DevExpress.Utils.Controls.ImageHelper.MakeTransparent(gl);
        }

        public void ShowBloqueo(string pBloqueo, bool ShowWarning)
        {
            bartextBloqueo.Caption = pBloqueo;
            if (ShowWarning)
            {
                barimgWarning.Visibility = BarItemVisibility.Always;
                barimgWarning.ImageOptions.Image = Labour.Properties.Resources.warningX16;
            }
            else
            {
                barimgWarning.Visibility = BarItemVisibility.Always;
                barimgWarning.ImageOptions.Image = Labour.Properties.Resources.checkedX16;
            }
                

        }
        #endregion

    
      


        public frmMain()
        {
            InitializeComponent();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("es-ES");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("es-ES");
        }        

        private void frmAcceso_Load(object sender, EventArgs e)
        {            
            //llamar a la funcion para setear las propiedades para la conexion a BD
            SkinManager.EnableFormSkins();
            SkinManager.EnableMdiFormSkins();

            this.Text = "System Remuneraciones (" + fnSistema.VersionSistema + ")";

            //BarEditItem ite = new BarEditItem(barManager1);
            //ite.Edit = barManager1.RepositoryItems.Add("PictureEdit");
            //ite.ImageOptions.Image = Labour.Properties.Resources.loading;
            //ite.PaintStyle = BarItemPaintStyle.CaptionGlyph;       
            //barInferior.AddItem(ite);

            Bars barras = barManager1.Bars;
            //OCULTAMOS TODAS LAS BARRAS
            foreach (Bar item in barras)
            {
                item.Visible = false;
            }            

            barSuperior.Reset();      


            //barSuperior.Visible = false;

            //barInferior.Visible = false;

            //VERIFICAR SI EXISTE EL ARCHIVO DE CONFIGURACION DEL EJECUTABLE
            //if (!XmlC.ExisteArchivoConfiguracion())
            //{ XtraMessageBox.Show("Falta archivo de configuración", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            //VERIFICA SI EXISTE BASE DE DATOS DE CONFIGURACION (JUEGOS DE DATOS)
            string path = Environment.CurrentDirectory + @"\configdb.sqlite";
            if (!File.Exists(path))
            { XtraMessageBox.Show("Falta archivo de configuracion", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            ////NOMBRE DEL PROCESO (NOMBRE PROCESS QUE APARECE EN TASKBAR)
            //string nameProcess = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
            
            ////SOLO DEJAMOS ABRIR UN PROCESO A LA VEZ
            //if (fnSistema.IsProcessRunning(nameProcess))
            //{
              //fnSistema.AplicacionCorriendo = false;
            //    //XtraMessageBox.Show("No puedes abrir otra vez el mismo programa", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    Close();
            //    return;
            //}

            //ABRIR FORMULARIO ACCESO
            frmAcceso ac = new frmAcceso();
            ac.OpenStatus = this;
            ac.MdiParent = this;         
            ac.open = this;
            ac.Show();         
        }

        //private void cbDatos_DoubleClick(object sender, EventArgs e)
        //{
        //mostramos el formulario con grilla de juego de datos
        //frmDatosUpdate update = new frmDatosUpdate();
        //  update.ShowDialog(this);
        //}

        #region "MANEJO DE DATOS"
        //METODO PARA CARGAR USUARIO Y PASSWORD DE ACUERDO A VALUE DEL COMBOBOX
        /*
         * DATO DE ENTRADA
         * @-> VALUE COMBOEDIT
         * DATO SALIDA
         * @-> USER
         * @-> PASSWORD
         */
        private void fnCredenciales(string nombre)
        {
            string sql = "SELECT * from datos WHERE nombre=@pNombre";
            SQLiteDataReader rd;
            if (SqlLite.NuevaConexion())
            {
                //USAMOS LA VARIABLE CONEXION
                using (SqlLite.Sqlconn)
                {
                    SQLiteCommand cmd = new SQLiteCommand(sql, SqlLite.Sqlconn);
                    //PARAMETROS
                    cmd.Parameters.Add(new SQLiteParameter("@pNombre", nombre));
                    rd = cmd.ExecuteReader();
                    //PREGUNTAMOS SI HAY FILAS
                    if (rd.HasRows)
                    {
                        while (rd.Read())
                        {
                            //cargamos registros en cajas correspondientes
                            //txtpass.Text = (string)rd["password"];
                            //txtuser.Text = (string)rd["usuario"];
                        }
                    }
                    else
                    {
                        XtraMessageBox.Show("No hay Registros!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    //LIBERAMOS EL SQLCOMMAND
                    cmd.Dispose();
                    //LIBERAMOS EL READER
                    rd.Close();
                    //CERRAMOS LA CONEXION
                    SqlLite.Sqlconn.Close();
                }
            }
            else
            {
                XtraMessageBox.Show("No hay conexion con la base de datos!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //RAZON SOCIAL EMPRESA
        private void InfoEmpresa()
        {
            string sql = "SELECT RAZON FROM EMPRESA";
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                barEmpresa.Caption = (string)rd["razon"];
                            }
                        }
                        else
                        {
                            barEmpresa.Caption = "Empresa: NN";
                        }
                    }
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                    rd.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Alt | Keys.F4))
            {
                if (Calculo.ViewStatus())
                { XtraMessageBox.Show("No puedes cerrar el programa mientras este corriendo un proceso", "Denegado", MessageBoxButtons.OK, MessageBoxIcon.Stop); }

               /* DialogResult advertencia = XtraMessageBox.Show("Estas seguro de cerrar la aplicación", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (advertencia == DialogResult.Yes)
                {
                    Close();
                }
                else
                {
                    return true;
                }*/
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
        #endregion

        private void cbDatos_SelectedIndexChanged(object sender, EventArgs e)
        {
            //OBTENEMOS EL VALOR DEL COMBO
           // string value = cbDatos.EditValue.ToString();
            //LLAMAMOS AL METODO PARA OBTENER LAS CREDENCIALES
            //fnCredenciales(value);
        }

        private void btnIngreso_Click(object sender, EventArgs e)
        {
           //REALIZAR INGRESO DE ACUERDO A CREDENCIALES INGRESADAS            
        }     

        private void barButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {            
            this.Close();
        }

        private void barButtonItem9_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;
            
            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmficha"))
            {
                //NO DEJAR ABRIR ESTE FORMULARIO SI NO SE HAN INGRESADO LOS DATOS DE LA EMPRESA
                //Empresa emp = new Empresa();
                //if (!Licencia.HayDatos)
                //{ XtraMessageBox.Show("Por favor ingresa los datos de tu empresa antes de continuar", "Informacion Importante", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }                

                //VER SI EL FORM YA ESTA ABIERTO Y EVITAR QUE SE VUELVA A ABRIR
                Form frm = this.MdiChildren.FirstOrDefault(x => x is frmEmpleado);

                if (frm != null)
                {
                    frm.BringToFront();
                    return;
                }

                //SI EXISTE CREAMOS LA INSTANCIA
                frm = new frmEmpleado();
                frm.MdiParent = this;
                frm.Show();
            }
            else
                XtraMessageBox.Show("No tienes los provilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);          
        }       

        private void CloseAllActiveForms()
        {
            try
            {
                foreach (Form child in this.MdiChildren)
                {
                    if (!child.Focused)
                    {
                        child.Close();
                    }
                }

                if (Application.OpenForms.Count > 0)
                {
                    foreach (Form x in Application.OpenForms)
                    {
                        if (x.Name.ToLower() != "frmmain")
                            x.Close();
                    } 
                }
            }
            catch (Exception ex)
            {
             //Error...
            }
        }

        private void barButtonItem16_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //CERRAR SESION
           
            DialogResult dialogo = XtraMessageBox.Show("¿Realmente desea salir?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogo == DialogResult.Yes)
            {
                //CAMBIAR STATUS A OFFLINE
                // User.ChangeStatusUser(0, User.getUser());

                Sesion.DeleteAccess(User.getUser().ToLower());
                //Sesion.Kill(Sesion.SpidSesion);

                //SI SE CIERRA SESION DETENER EL TIMER...
                CloseAllActiveForms();                          
                barSuperior.Reset();

                Bars barras = barManager1.Bars;
                foreach (Bar item in barras)
                {
                    item.Visible = false;
                }

                //PARAMOS TIMER DE NOTIFICACIONES
                if (Alerta.StatusMonitor != null)
                    Alerta.StatusMonitor.Stop();
                
                //Volvemos a mostrar ventana de acceso
                frmAcceso ac = new frmAcceso();
                Sesion.Activa = false;
                ac.MdiParent = this;              
                ac.open = this;
                ac.Show();                
            }          
        }

        private void barButtonItem10_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDADA
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmgeneracioncontrato") == false)
            { XtraMessageBox.Show("No tienes los provilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            Form frm = this.MdiChildren.FirstOrDefault(x => x is frmGeneracionContrato);

            if (frm != null)
            {
                frm.BringToFront();
                return;
            }

            //SI NO EXISTE CREAMOS EL FORM
            frm = new frmGeneracionContrato();
            frm.MdiParent = this;
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.Show();

        }

        private void barButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmtabla"))
            {

                //SE LOCALIZA EL FORMULARIO BUSCANDOLO ENTRE LOS FORM ABIERTOS
                Form frm = this.MdiChildren.FirstOrDefault(x => x is frmTablas);

                if (frm != null)
                {
                    //SI LA INSTANCIA EXISTE LA PONGO EN PRIMER PLANO
                    frm.BringToFront();
                    return;
                }

                //SI NO EXISTE LA CREAMOS
                frm = new frmTablas();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();

            }
            else
                XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);

        }

        private void baritemLLSS_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD SESION
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmllss"))
            {

                Form frm = this.MdiChildren.FirstOrDefault(x => x is frmLeyesSociales);

                if (frm != null)
                {
                    frm.BringToFront();
                    return;
                }

                //SI NO EXISTE LA CREAMOS
                frm = new frmLeyesSociales();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
            else
                XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);                           
        }

        private void baritemEmpresa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmempresa"))
            {      

                Form frm = this.MdiChildren.FirstOrDefault(x => x is frmEmpresa);

                if (frm != null)
                {
                    frm.BringToFront();
                    return;
                }

                //SI NO EXISTE LA CREAMOS
                frm = new frmEmpresa();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
            else
            {
                XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
               
        }       

        private void barItems_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmitem"))
            {

                //FORMULARIO EMPRESA
                Form frm = this.MdiChildren.FirstOrDefault(x => x is frmItem);

                if (frm != null)
                {
                    frm.BringToFront();
                    return;
                }

                //SI NO EXISTE CREAMOS EL FORM
                frm = new frmItem();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
            else
                XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
              
        
        }

        private void barItemParametro_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD   
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            //FORMULARIO EMPRESA
            Form frm = this.MdiChildren.FirstOrDefault(x => x is frmParametros);

            if (frm != null)
            {
                frm.BringToFront();
                return;
            }

            //SI NO EXISTE CREAMOS EL FORM
            frm = new frmParametros();
            frm.MdiParent = this;
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.Show();
        }

        private void barItemFormula_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmformula"))
            {
                Form frm = this.MdiChildren.FirstOrDefault(x => x is frmFormula);

                if (frm != null)
                {
                    frm.BringToFront();
                    return;
                }

                //SI NO EXISTE CREAMOS EL FORM
                frm = new frmFormula();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
            else
                XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);                
         
        }

        private void barItemIndicesMensuales_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;
            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmindice"))
            {

                Form frm = this.MdiChildren.FirstOrDefault(x => x is frmIndiceMensual);

                if (frm != null)
                {
                    frm.BringToFront();
                    return;
                }

                //SI NO EXISTE CREAMOS EL FORM
                frm = new frmIndiceMensual();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
            else
            {
                XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void batitemClase_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //SESION NUEVA ACIVDAD
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmclase"))
            {

                Form frm = this.MdiChildren.FirstOrDefault(x => x is frmClase);

                if (frm != null)
                {
                    frm.BringToFront();
                    return;
                }

                //SI NO EXISTE CREAMOS EL FORM
                frm = new frmClase();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
            else
                XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        private void baritemImpuestos_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmimpto"))
            {

                Form frm = this.MdiChildren.FirstOrDefault(x => x is frmImpuesto);

                if (frm != null)
                {
                    frm.BringToFront();
                    return;
                }

                //SI NO EXISTE CREAMOS EL FORM
                frm = new frmImpuesto();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
            else
            {
                XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }                
         
        }

        private void baritemAusentismo_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Form frm = this.MdiChildren.FirstOrDefault(x => x is frmAusentismo);

            if (frm != null)
            {
                frm.BringToFront();
                return;
            }

            //SI NO EXISTE CREAMOS EL FORM
            frm = new frmAusentismo();
            frm.MdiParent = this;
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.Show();

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //CARGAR HORA EN BAR HORA   
            barHora.Caption = DateTime.Now.ToLongTimeString();
        }

        private void barUsuario_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmuser"))
            {
                Form frm = this.MdiChildren.FirstOrDefault(x => x is frmUsuario);

                if (frm != null)
                {
                    frm.BringToFront();
                    return;
                }

                //SI NO EXISTE CREAMOS EL FORM
                frm = new frmUsuario();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
            else
                XtraMessageBox.Show("No tienes los privilegios necesarios para realizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        private void barCambiarPass_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD 
            Sesion.NuevaActividad();

            Form frm = this.MdiChildren.FirstOrDefault(x => x is frmClave);

            if (frm != null)
            {
                frm.BringToFront();
                return;
            }

            //SI NO EXISTE CREAMOS EL FORM
            frm = new frmClave();
            frm.ShowDialog();
        }

        private void batitemRemuneraciones_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD SESION
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmcalremun"))
            {
                //VE FICHAS PRIVADAS
                ShowPrivadas = User.ShowPrivadas();

                if (Calculo.GetTotalRegistros(ShowPrivadas) == 0)
                { XtraMessageBox.Show("No se encontraron registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                if (Calculo.ViewStatus())
                { XtraMessageBox.Show("Ya se está realizando un proceso de calculo, intentelo mas tarde", "Proceso activo usuario " + User.getUser(), MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                Form frm = this.MdiChildren.FirstOrDefault(x => x is frmCalculoRemuneracion);

                if (frm != null)
                {
                    frm.BringToFront();
                    return;
                }

                //SI NO EXISTE CREAMOS EL FORM        
                frmCalculoRemuneracion calc = new frmCalculoRemuneracion();
                calc.CambioEstadoOpen = this;
                calc.StartPosition = FormStartPosition.CenterParent;
                calc.ShowDialog();
                //frm = new frmCalculoRemuneracion();                
                //frm.StartPosition = FormStartPosition.CenterParent;              
                //frm.ShowDialog();
            }
            else
                XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        private void barItemDetalle_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void barItemResumenProceso_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmresproceso"))
            {
                //VE FICHAS PRIVADAS
                ShowPrivadas = User.ShowPrivadas();

                if (Calculo.GetTotalRegistros(ShowPrivadas) == 0)
                { XtraMessageBox.Show("No se encontraron registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                Form frm = this.MdiChildren.FirstOrDefault(x => x is frmResumenProceso);

                if (frm != null)
                {
                    frm.BringToFront();
                    return;
                }

                //SI NO EXISTE CREAMOS EL FORM
                frm = new frmResumenProceso();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
            else
                XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        private void baritemFeriados_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmferiado"))
            {
                Form frm = this.MdiChildren.FirstOrDefault(x => x is frmFeriado);

                if (frm != null)
                {
                    frm.BringToFront();
                    return;
                }

                //SI NO EXISTE CREAMOS EL FORM
                frm = new frmFeriado();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
            else
                XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        private void baritemLiqHistorica_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmliqhistorico"))
            {
                //VE FICHAS PRIVADAS
                ShowPrivadas = User.ShowPrivadas();

                if (Calculo.GetTotalRegistros(ShowPrivadas) == 0)
                { XtraMessageBox.Show("No se encontraron registros", "Informacion importante", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                frmLiquidacionHistorica liq = new frmLiquidacionHistorica();
                liq.StartPosition = FormStartPosition.CenterScreen;
                liq.ShowDialog();
            }
            else
                XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        private void baritemCierreMes_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmcierremes"))
            {
                //VE FICHAS PRIVADAS
                ShowPrivadas = User.ShowPrivadas();

                if (Calculo.GetTotalRegistros(ShowPrivadas) == 0)
                { XtraMessageBox.Show("No se encontraron registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                //VERIFICAMOS SI HAY ALGUN PROCESO CORRIENDO
                if (Calculo.ViewStatus())
                { XtraMessageBox.Show("Hay un proceso en ejecucion", "Proceso activo", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }                

                frmCierreMes cierre = new frmCierreMes();
                cierre.StartPosition = FormStartPosition.CenterScreen;
                cierre.Opener = this;
                cierre.ShowDialog();
            }
            else
                XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        private void baritemDatosEmpleado_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmfichaempleado"))
            {
                //VE FICHAS PRIVADAS
                ShowPrivadas = User.ShowPrivadas();

                if (Calculo.GetTotalRegistros(ShowPrivadas) == 0)
                { XtraMessageBox.Show("No hay registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                Form frm = this.MdiChildren.FirstOrDefault(x => x is frmDatosEmpleado);

                if (frm != null)
                {
                    frm.BringToFront();
                    return;
                }

                //SI NO EXISTE CREAMOS EL FORM
                frm = new frmDatosEmpleado();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
            else
                XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        private void batitemLibroRemuneraciones_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmlibroremun"))
            {
                //VE FICHAS PRIVADAS
                ShowPrivadas = User.ShowPrivadas();

                if (Calculo.GetTotalRegistros(ShowPrivadas) == 0)
                { XtraMessageBox.Show("No se encontraron registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                Form frm = this.MdiChildren.FirstOrDefault(x => x is frmLibroRemuneraciones);

                if (frm != null)
                {
                    frm.BringToFront();
                    return;
                }

                //SI NO EXISTE CREAMOS EL FORM
                frm = new frmLibroRemuneraciones();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
            else
                XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
           
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmplanillasalud"))
            {
                //VE FICHAS PRIVADAS
                ShowPrivadas = User.ShowPrivadas();

                if (Calculo.GetTotalRegistros(ShowPrivadas) == 0)
                { XtraMessageBox.Show("No se encontraron registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                Form frm = this.MdiChildren.FirstOrDefault(x => x is frmPlanillaIsapre);

                if (frm != null)
                {
                    frm.BringToFront();
                    return;
                }

                //SI NO EXISTE CREAMOS EL FORM
                frm = new frmPlanillaIsapre();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
            else
                XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        private void baritemAfp_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmplanillaafp"))
            {
                //VE FICHAS PRIVADAS
                ShowPrivadas = User.ShowPrivadas();

                if (Calculo.GetTotalRegistros(ShowPrivadas) == 0)
                { XtraMessageBox.Show("No se encontraron registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                Form frm = this.MdiChildren.FirstOrDefault(x => x is frmPlanillaAfp);

                if (frm != null)
                {
                    frm.BringToFront();
                    return;
                }

                //SI NO EXISTE CREAMOS EL FORM
                frm = new frmPlanillaAfp();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
            else
                XtraMessageBox.Show("No tienes privilegios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);

        }

        private void barbtnPagoMutual_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmplanillamutual"))
            {
                //VE FICHAS PRIVADAS
                ShowPrivadas = User.ShowPrivadas();

                if (Calculo.GetTotalRegistros(ShowPrivadas) == 0)
                { XtraMessageBox.Show("No se encontraron registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }                

                Form frm = this.MdiChildren.FirstOrDefault(x => x is frmPlanillaMutual);

                if (frm != null)
                {
                    frm.BringToFront();
                    return;
                }

                //SI NO EXISTE CREAMOS EL FORM
                frm = new frmPlanillaMutual();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
            else
                XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmconjuntobusqueda"))
            {
                FrmFiltroBusqueda filtro = new FrmFiltroBusqueda();
                filtro.StartPosition = FormStartPosition.CenterParent;
                filtro.ShowDialog();
            }
            else
                XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);                     
        }

        private void barsubitemInformacion_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frminfoitem"))
            {
                Form frm = this.MdiChildren.FirstOrDefault(x => x is frmDetalleItem);

                if (frm != null)
                {
                    frm.BringToFront();
                    return;
                }

                //SI NO EXISTE CREAMOS EL FORM
                frm = new frmDetalleItem();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
            else
                XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
           
        }

        private void barsubitemAgrega_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmadditemmasivo"))
            {                
                Form frm = this.MdiChildren.FirstOrDefault(x => x is frmAgregarItems);

                if (frm != null)
                {
                    frm.BringToFront();
                    return;
                }

                //SI NO EXISTE CREAMOS EL FORM
                frm = new frmAgregarItems();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
            else
                XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        
        }

        private void barsubitemEliminar_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmeliminaitemmasivo"))
            {

                Form frm = this.MdiChildren.FirstOrDefault(x => x is FrmEliminarItems);

                if (frm != null)
                {
                    frm.BringToFront();
                    return;
                }

                //SI NO EXISTE CREAMOS EL FORM
                frm = new FrmEliminarItems();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
            else
                XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        private void baritemModificarInformacion_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmeditinfoempleado"))
            {
                Form frm = this.MdiChildren.FirstOrDefault(x => x is FrmModificarInfoEmpleado);

                if (frm != null)
                {
                    frm.BringToFront();
                    return;
                }

                //SI NO EXISTE CREAMOS EL FORM
                frm = new FrmModificarInfoEmpleado();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
            else
                XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        private void baritemAgregaInformacion_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmimportdata"))
            {
                Form frm = this.MdiChildren.FirstOrDefault(x => x is FrmAgregarEmpleados);

                if (frm != null)
                {
                    frm.BringToFront();
                    return;
                }

                //SI NO EXISTE CREAMOS EL FORM
                frm = new FrmAgregarEmpleados();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
            else
                XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);               
        }

        private void barsubExportarArchivo_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmexportdata"))
            {

                Form frm = this.MdiChildren.FirstOrDefault(x => x is frmExportarEmpleados);

                if (frm != null)
                {
                    frm.BringToFront();
                    return;
                }

                //SI NO EXISTE CREAMOS EL FORM
                frm = new frmExportarEmpleados();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
            else
                XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //frmSeleccionItemElimina sel = new frmSeleccionItemElimina("PRESTAM");
            //sel.StartPosition = FormStartPosition.CenterParent;
            //sel.Show();
        }

        private void barSuperior_DockChanged(object sender, EventArgs e)
        {

        }

        private void barbtnSeguridad_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //SESION NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmseguridad") == false)
            { XtraMessageBox.Show("No tienes los permisos necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            Form frm = this.MdiChildren.FirstOrDefault(x => x is frmSeguridad);

            if (frm != null)
            {
                frm.BringToFront();
                return;
            }

            //SI NO EXISTE CREAMOS EL FORM
            frm = new frmSeguridad();
            frm.MdiParent = this;
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.Show();
        }

        private void barbtnarchivoprevired_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDA EN SESION
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmprevired") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            //VE FICHAS PRIVADAS
            ShowPrivadas = User.ShowPrivadas();

            if (Calculo.GetTotalRegistros(ShowPrivadas) == 0)
            { XtraMessageBox.Show("No se encontraron registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            frmPrevired prev = new frmPrevired();
            prev.StartPosition = FormStartPosition.CenterParent;
            prev.ShowDialog();
        }

        private void barButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
          
        }

        private void barimportaritems_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmimportaritems") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            frmImportarItems items = new frmImportarItems();
            items.StartPosition = FormStartPosition.CenterScreen;
            items.ShowDialog();
        }

        private void barUpdDesdeArchivo_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //SESION ACTIVIDAD
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmimportaupdtrab") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            frmImportarTrabUpd update = new frmImportarTrabUpd();
            update.StartPosition = FormStartPosition.CenterScreen;
            update.ShowDialog();
        }

        private void baritemReabrirMes_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //NUEVA SESION ACTIVIDAD    
            Sesion.NuevaActividad();

            //VER SI EL USUARIO ESTA BLOQUEADO 
            if (User.Bloqueado())
            { XtraMessageBox.Show("No puedes realizar modificaciones", "Información", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmreabreperiodo") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (User.GetLlaveMaestra(User.getUser()) == "0")
            { XtraMessageBox.Show("Solicita a soporte la llave maestra para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

            if (Calculo.ViewStatus())
            { XtraMessageBox.Show("Hay un proceso de calculo en curso, por favor espera un momento", $"Proceso activo por {User.getUser()}", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (Calculo.ExistePeriodoAnterior(fnSistema.fnObtenerPeriodoAnterior(Calculo.PeriodoObservado)) == false)
            { XtraMessageBox.Show("No existe ningun periodo que se pueda reabrir", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

            frmMaestra master = new frmMaestra();
            master.Opener = this;
            master.StartPosition = FormStartPosition.CenterParent;
            master.ShowDialog();
        }

        private void barsubitemModificaritems_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmmodificaitemmasivo") == false)
            { XtraMessageBox.Show("No tienes los provilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            Form frm = this.MdiChildren.FirstOrDefault(x => x is FrmModificaItems);

            if (frm != null)
            {
                frm.BringToFront();
                return;
            }

            //SI NO EXISTE CREAMOS EL FORM
            frm = new FrmModificaItems();
            frm.MdiParent = this;
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.Show();
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {

                //OCURRIÓ ALGUN ERROR Y CERRAMOS EL PROGRAMA SIN ADVERTIR
                //POR EJEMPLO SI LA LICENCIA NO ES VALIDA O LA VERSION ESTÁ OBSOLETA...
                if (Licencia.ForzarCierre)
                    return;

                var result = XtraMessageBox.Show("¿Estás seguro de cerrar la aplicación?", "Informacion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                {                    
                    e.Cancel = true;
                }                    
                else
                {
                    e.Cancel = false;                 
                    //ELIMINAMOS SESSION
                    Sesion.DeleteAccess(User.getUser().ToLower());

                    //VERIFICAR SI QUERDÓ ALGUN PROCESO NO CERRADO CORRECTAMENTE...
                    Monitor.CleanTask(User.getUser());

                }            
        }

        private void frmMain_KeyDown(object sender, KeyEventArgs e)
        {
            Alt_F4 = (e.KeyCode.Equals(Keys.F4) && e.Alt == true);
        }

        private void barItemAignacionFamiliar_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDADA
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmfamtramo") == false)
            { XtraMessageBox.Show("No tienes los provilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            Form frm = this.MdiChildren.FirstOrDefault(x => x is frmAsigFam);

            if (frm != null)
            {
                frm.BringToFront();
                return;
            }

            //SI NO EXISTE CREAMOS EL FORM
            frm = new frmAsigFam();
            frm.MdiParent = this;
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.Show();
        }

        private void baritemCargaMasivaitems_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //ACTIVIDAD
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmimportaritemmasivo") == false)
            { XtraMessageBox.Show("No tienes los provilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            Form frm = this.MdiChildren.FirstOrDefault(x => x is frmCargaMasivaItems);

            if (frm != null)
            {
                frm.BringToFront();
                return;
            }

            //SI NO EXISTE CREAMOS EL FORM
            frmCargaMasivaItems itemsmas = new frmCargaMasivaItems();
            itemsmas.StartPosition = FormStartPosition.CenterScreen;
            itemsmas.CambioEstadoOpen = this;
            itemsmas.Show();
            //frm = new frmCargaMasivaItems();
            //frm.MdiParent = this;
            //frm.StartPosition = FormStartPosition.CenterScreen;
            //frm.Show();
        }

        private void barButtonItem7_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void btnSubitemLicencia_ItemClick(object sender, ItemClickEventArgs e)
        {
            Sesion.NuevaActividad();

            //FORMULARIO SOLO PARA MOSTRAR LA LICENCIA DEL SISTEMA
            FrmLicencia lic = new FrmLicencia();            
            lic.StartPosition = FormStartPosition.CenterParent;
            lic.ShowDialog();
        }

        private void btnItemCaja_ItemClick(object sender, ItemClickEventArgs e)
        {
            //ACTIVIDAD
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmPlanillaCaja") == false)
            { XtraMessageBox.Show("No tienes los provilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            //VE FICHAS PRIVADAS
            ShowPrivadas = User.ShowPrivadas();

            if (Calculo.GetTotalRegistros(ShowPrivadas) == 0)
            { XtraMessageBox.Show("No se encontraron registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            Form frm = this.MdiChildren.FirstOrDefault(x => x is FrmPlanillaCaja);

            if (frm != null)
            {
                frm.BringToFront();
                return;
            }

            Empresa emp = new Empresa();
            emp.SetInfo();

            if (emp.NombreCCAF == 1)
            { XtraMessageBox.Show("Esta empresa no tiene asociada una caja de compensación", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

            //SI NO EXISTE CREAMOS EL FORM
            frm = new FrmPlanillaCaja();
            frm.MdiParent = this;
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.Show();
        }

        private void barBtnCorreo_ItemClick(object sender, ItemClickEventArgs e)
        {
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmconfcorreo") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta función", "Acceso", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            Form frm = this.MdiChildren.FirstOrDefault(x => x is frmConfCorreo);

            if (frm != null)
            {
                frm.BringToFront();
                return;
            }

            //SI NO EXISTE CREAMOS EL FORM
            frm = new frmConfCorreo();
            frm.MdiParent = this;
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.Show();
        }

        private void barBtnCorreoMasivo_ItemClick(object sender, ItemClickEventArgs e)
        {
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmenviocorreo") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta función", "Acceso", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            Form frm = this.MdiChildren.FirstOrDefault(x => x is frmCorreoMasivo);

            if (frm != null)
            {
                frm.BringToFront();
                return;
            }
            
            //SI NO EXISTE CREAMOS EL FORM
            frm = new frmCorreoMasivo();
            frm.MdiParent = this;
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.Show();
        }

        private void barBtnPlanillasSueldo_ItemClick(object sender, ItemClickEventArgs e)
        {
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmplanillasueldos") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta función", "Acceso", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            Form frm = this.MdiChildren.FirstOrDefault(x => x is FrmPlanillaSueldos);

            if (frm != null)
            {
                frm.BringToFront();
                return;
            }

            //SI NO EXISTE CREAMOS EL FORM
            FrmPlanillaSueldos sueldo = new FrmPlanillaSueldos();
            sueldo.CambioEstadoOpen = this;
            sueldo.StartPosition = FormStartPosition.CenterParent;
            sueldo.Show();

            //frm = new FrmPlanillaSueldos();
            //frm.MdiParent = this;
            //frm.StartPosition = FormStartPosition.CenterScreen;
            //frm.Show();
        }

        private void barBtnNominasBanco_ItemClick(object sender, ItemClickEventArgs e)
        {
            Sesion.NuevaActividad();

            //RUTA DLL
            //string RutaDll = Environment.CurrentDirectory + "\\Nominas.dll";
            //string BciDll = Environment.CurrentDirectory + "\\BancoBci.dll";
            string ItauDll = Environment.CurrentDirectory + "\\BancoItau.dll";
            if (File.Exists(ItauDll) == false)
            { XtraMessageBox.Show("No se encuentra archivo de configuración", "Archivo no encontrado", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmnominasbanco") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta función", "Acceso", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            Form frm = this.MdiChildren.FirstOrDefault(x => x is frmNominasBanco);

            if (frm != null)
            {
                frm.BringToFront();
                return;
            }

            //SI NO EXISTE CREAMOS EL FORM
            frm = new frmNominasBanco();
            frm.MdiParent = this;
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.Show();
        }

        private void barButtonItem12_ItemClick(object sender, ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDADA
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmcartaaviso") == false)
            { XtraMessageBox.Show("No tienes los provilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            Form frm = this.MdiChildren.FirstOrDefault(x => x is frmCartaAviso);

            if (frm != null)
            {
                frm.BringToFront();
                return;
            }

            //SI NO EXISTE CREAMOS EL FORM
            frm = new frmCartaAviso();
            frm.MdiParent = this;
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.Show();
        }

        private void barBtnContable_ItemClick(object sender, ItemClickEventArgs e)
        {
            Sesion.NuevaActividad();        

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmcomprobantecontable") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta función", "Acceso", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
            
            frmComprobanteContable comp = new frmComprobanteContable();
            comp.StartPosition = FormStartPosition.CenterParent;
            comp.ShowDialog();
        }   

        private void barbtnListadoItems_ItemClick_1(object sender, ItemClickEventArgs e)
        {
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmlistadoitems") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta función", "Acceso", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            frmListadoItems lista = new frmListadoItems();
            lista.StartPosition = FormStartPosition.CenterParent;
            lista.ShowDialog();
        }

        private void barBtnVacColectivas_ItemClick(object sender, ItemClickEventArgs e)
        {
            Sesion.NuevaActividad();

            if(objeto.ValidaAcceso(User.GetUserGroup(), "frmvacacionescolectivas") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta función", "Acceso", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            frmVacacionesColectivas colec = new frmVacacionesColectivas();
            colec.StartPosition = FormStartPosition.CenterParent;
            colec.ShowDialog();
        }

        private void barSubEmpresa_ItemClick(object sender, ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmempresa"))
            {

                Form frm = this.MdiChildren.FirstOrDefault(x => x is frmEmpresa);

                if (frm != null)
                {
                    frm.BringToFront();
                    return;
                }

                //SI NO EXISTE LA CREAMOS
                frm = new frmEmpresa();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
            else
            {
                XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void barSubInfoitems_ItemClick(object sender, ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frminfoitem"))
            {
                Form frm = this.MdiChildren.FirstOrDefault(x => x is frmDetalleItem);

                if (frm != null)
                {
                    frm.BringToFront();
                    return;
                }

                //SI NO EXISTE CREAMOS EL FORM
                frm = new frmDetalleItem();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
            else
                XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        private void barSubitems_ItemClick(object sender, ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmitem"))
            {

                //FORMULARIO EMPRESA
                Form frm = this.MdiChildren.FirstOrDefault(x => x is frmItem);

                if (frm != null)
                {
                    frm.BringToFront();
                    return;
                }

                //SI NO EXISTE CREAMOS EL FORM
                frm = new frmItem();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
            else
                XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        private void barSubIndices_ItemClick(object sender, ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmindice"))
            {

                Form frm = this.MdiChildren.FirstOrDefault(x => x is frmIndiceMensual);

                if (frm != null)
                {
                    frm.BringToFront();
                    return;
                }

                //SI NO EXISTE CREAMOS EL FORM
                frm = new frmIndiceMensual();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
            else
            {
                XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void barSubCargasTramos_ItemClick(object sender, ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDADA
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmfamtramo") == false)
            { XtraMessageBox.Show("No tienes los provilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            Form frm = this.MdiChildren.FirstOrDefault(x => x is frmAsigFam);

            if (frm != null)
            {
                frm.BringToFront();
                return;
            }

            //SI NO EXISTE CREAMOS EL FORM
            frm = new frmAsigFam();
            frm.MdiParent = this;
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.Show();
        }

        private void barSubFichasEmp_ItemClick(object sender, ItemClickEventArgs e)
        {
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmficha"))
            {
                //NO DEJAR ABRIR ESTE FORMULARIO SI NO SE HAN INGRESADO LOS DATOS DE LA EMPRESA
                //Empresa emp = new Empresa();
                //if (!Licencia.HayDatos)
                //{ XtraMessageBox.Show("Por favor ingresa los datos de tu empresa antes de continuar", "Informacion Importante", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }                

                //VER SI EL FORM YA ESTA ABIERTO Y EVITAR QUE SE VUELVA A ABRIR
                Form frm = this.MdiChildren.FirstOrDefault(x => x is frmEmpleado);

                if (frm != null)
                {
                    frm.BringToFront();
                    return;
                }

                //SI EXISTE CREAMOS LA INSTANCIA
                frm = new frmEmpleado();
                frm.MdiParent = this;
                frm.Show();
            }
            else
                XtraMessageBox.Show("No tienes los provilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        private void barSubLiquidaciones_ItemClick(object sender, ItemClickEventArgs e)
        {
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmplanillasueldos") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta función", "Acceso", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            Form frm = this.MdiChildren.FirstOrDefault(x => x is FrmPlanillaSueldos);

            if (frm != null)
            {
                frm.BringToFront();
                return;
            }

            //SI NO EXISTE CREAMOS EL FORM
            frm = new FrmPlanillaSueldos();
            frm.MdiParent = this;
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.Show();
        }

        private void barSubLiqHistoricas_ItemClick(object sender, ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmliqhistorico"))
            {
                //VE FICHAS PRIVADAS
                ShowPrivadas = User.ShowPrivadas();

                if (Calculo.GetTotalRegistros(ShowPrivadas) == 0)
                { XtraMessageBox.Show("No se encontraron registros", "Informacion importante", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                frmLiquidacionHistorica liq = new frmLiquidacionHistorica();
                liq.StartPosition = FormStartPosition.CenterScreen;
                liq.ShowDialog();
            }
            else
                XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        private void barSubCondiciones_ItemClick(object sender, ItemClickEventArgs e)
        {

            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmconjuntobusqueda"))
            {
                FrmFiltroBusqueda filtro = new FrmFiltroBusqueda();
                filtro.StartPosition = FormStartPosition.CenterParent;
                filtro.ShowDialog();
            }
            else
                XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        private void barSubCalculoLiq_ItemClick(object sender, ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD SESION
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmcalremun"))
            {
                //VE FICHAS PRIVADAS
                ShowPrivadas = User.ShowPrivadas();

                if (Calculo.GetTotalRegistros(ShowPrivadas) == 0)
                { XtraMessageBox.Show("No se encontraron registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                if (Calculo.ViewStatus())
                { XtraMessageBox.Show("Ya se está realizando un proceso de calculo, intentelo mas tarde", "Proceso activo usuario " + User.getUser(), MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                Form frm = this.MdiChildren.FirstOrDefault(x => x is frmCalculoRemuneracion);

                if (frm != null)
                {
                    frm.BringToFront();
                    return;
                }

                frmCalculoRemuneracion calc = new frmCalculoRemuneracion();
                calc.CambioEstadoOpen = this;
                calc.StartPosition = FormStartPosition.CenterParent;
                calc.ShowDialog();
                ////SI NO EXISTE CREAMOS EL FORM
                //frm = new frmCalculoRemuneracion();                
                //frm.ShowDialog();
            }
            else
                XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        private void barSubResumenProc_ItemClick(object sender, ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmresproceso"))
            {
                //VE FICHAS PRIVADAS
                ShowPrivadas = User.ShowPrivadas();

                if (Calculo.GetTotalRegistros(ShowPrivadas) == 0)
                { XtraMessageBox.Show("No se encontraron registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                Form frm = this.MdiChildren.FirstOrDefault(x => x is frmResumenProceso);

                if (frm != null)
                {
                    frm.BringToFront();
                    return;
                }

                //SI NO EXISTE CREAMOS EL FORM
                frm = new frmResumenProceso();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
            else
                XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        private void barSubPrevired_ItemClick(object sender, ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDA EN SESION
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmprevired") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            //VE FICHAS PRIVADAS
            ShowPrivadas = User.ShowPrivadas();

            if (Calculo.GetTotalRegistros(ShowPrivadas) == 0)
            { XtraMessageBox.Show("No se encontraron registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            frmPrevired prev = new frmPrevired();
            prev.StartPosition = FormStartPosition.CenterParent;
            prev.ShowDialog();
        }

        private void barSubBanco_ItemClick(object sender, ItemClickEventArgs e)
        {
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            //RUTA DLL
            //string RutaDll = Environment.CurrentDirectory + "\\Nominas.dll";
            //string BciDll = Environment.CurrentDirectory + "\\BancoBci.dll";
            string ItauDll = Environment.CurrentDirectory + "\\BancoItau.dll";
            if (File.Exists(ItauDll) == false)
            { XtraMessageBox.Show("No se encuentra archivo de configuración", "Archivo no encontrado", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmnominasbanco") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta función", "Acceso", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            Form frm = this.MdiChildren.FirstOrDefault(x => x is frmNominasBanco);

            if (frm != null)
            {
                frm.BringToFront();
                return;
            }

            //SI NO EXISTE CREAMOS EL FORM
            frm = new frmNominasBanco();
            frm.MdiParent = this;
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.Show();
        }

        private void barBtnMaestroContable_ItemClick(object sender, ItemClickEventArgs e)
        {
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmmaestrocontable") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utlizar esta función", "Acceso", MessageBoxButtons.OK, MessageBoxIcon.Stop);return; }

            Form frm = this.MdiChildren.FirstOrDefault(x => x is frmNominasBanco);

            if (frm != null)
            {
                frm.BringToFront();
                return;
            }

            frmCuentaContable cuenta = new frmCuentaContable();
            cuenta.StartPosition = FormStartPosition.CenterParent;
            cuenta.MdiParent = this;
            cuenta.Show();
        }

        private void barButtonItem18_ItemClick(object sender, ItemClickEventArgs e)
        {
            Sesion.NuevaActividad();

            //string FiltroSql = "", Path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Query.xlsx";
            //FiltroSql = Calculo.GetSqlFiltro(User.GetUserFilter(), "", User.ShowPrivadas(), true);

            //BuilderSql b = new BuilderSql();
            //b.ShowSqlBuilder(fnSistema.pgServer, fnSistema.pgDatabase, fnSistema.pgUser, fnSistema.pgPass, FiltroSql);

            //if (b.CustomSql.Length > 0)
            //{
            //    if (b.GetExcel(Path))
            //    {
            //        DialogResult d = XtraMessageBox.Show($"Archivo {Path} generado correctamete.¿Deseas revisar archivo?", "Informacion", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            //        if (d == DialogResult.Yes)
            //        {
            //            System.Diagnostics.Process.Start(Path);
            //        }
            //        else
            //        {
            //            XtraMessageBox.Show("No se pudo generar documento", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            //        }
            //    }
            //    else
            //    {
            //        XtraMessageBox.Show("No se pudo generar documento", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            //    }
            //}
            //else
            //{
            //    XtraMessageBox.Show("No se pudo generar información", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            //}
        }

        private void batbtnSesiones_ItemClick(object sender, ItemClickEventArgs e)
        {
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmsesiones") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utlizar esta función", "Acceso", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            Form frm = this.MdiChildren.FirstOrDefault(x => x is frmSesion);

            if (frm != null)
            {
                frm.BringToFront();
                return;
            }

            frmSesion cuenta = new frmSesion();
            cuenta.StartPosition = FormStartPosition.CenterParent;
            cuenta.MdiParent = this;
            cuenta.Show();
        }

        private void barbtnRespaldoBase_ItemClick(object sender, ItemClickEventArgs e)
        {
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmrespaldo") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta función", "Acceso", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            frmRespaldo resp = new frmRespaldo();
            resp.StartPosition = FormStartPosition.CenterParent;
            resp.ShowDialog();
        }

        private void barButtonItem20_ItemClick(object sender, ItemClickEventArgs e)
        {
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            // XtraMessageBox.Show("Módulo en construcción", "información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmquerybuilder") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta función", "Acceso", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }          

            frmGenerador gen = new frmGenerador();
            gen.StartPosition = FormStartPosition.CenterParent;
            gen.ShowDialog();
        }

        private void barbtnReporteContable_ItemClick(object sender, ItemClickEventArgs e)
        {
            Sesion.NuevaActividad();

            // XtraMessageBox.Show("Módulo en construcción", "información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //if (objeto.ValidaAcceso(User.GetUserGroup(), "frmquerybuilder") == false)
            //{ XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta función", "Acceso", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            frmConfRepoContable gen = new frmConfRepoContable();
            gen.StartPosition = FormStartPosition.CenterParent;
            gen.ShowDialog();
        }

        private void barBtnMontoAnual_ItemClick(object sender, ItemClickEventArgs e)
        {
            Sesion.NuevaActividad();

            frmMontoAnual mo = new frmMontoAnual();
            mo.MdiParent = this;
            mo.StartPosition = FormStartPosition.CenterParent;
            mo.Show();
        }

        private void barbtnfiniquitomasivo_ItemClick(object sender, ItemClickEventArgs e)
        {
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmfiniquitomasivo") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta función", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            frmFiniquitosMasivos mas = new frmFiniquitosMasivos();
            mas.StartPosition = FormStartPosition.CenterParent;
            mas.ShowDialog();
        }

        private void barbtnCertificadoRentas_ItemClick(object sender, ItemClickEventArgs e)
        {
            Sesion.NuevaActividad();

            frmCertificadoRentas cert = new frmCertificadoRentas();
            cert.StartPosition = FormStartPosition.CenterParent;
            cert.ShowDialog();

        }

        private void barButtonItem22_ItemClick(object sender, ItemClickEventArgs e)
        {
            Sesion.NuevaActividad();

            frmDeclaracionJurada dec = new frmDeclaracionJurada();
            dec.StartPosition = FormStartPosition.CenterParent;
            dec.ShowDialog();
        }

        private void barbtnMovimientos_ItemClick(object sender, ItemClickEventArgs e)
        {
            FrmMovimientos mov = new FrmMovimientos();
            mov.StartPosition = FormStartPosition.CenterScreen;
            mov.ShowDialog();
        }

        private void barResumenVac_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmvacacionest") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta función", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }


            FrmVacacionesEst est = new FrmVacacionesEst();
            est.StartPosition = FormStartPosition.CenterScreen;
            est.ShowDialog();
        }

        private void barItemCargaMasivaItem_ItemClick(object sender, ItemClickEventArgs e)
        {
            frmCargaMontoAfc ext = new frmCargaMontoAfc();
            ext.StartPosition = FormStartPosition.CenterScreen;
            ext.ShowDialog();
        }

        private void barbtnItemExtendido_ItemClick(object sender, ItemClickEventArgs e)
        {
            frmCargaItemExtendida fext = new frmCargaItemExtendida();
            fext.StartPosition = FormStartPosition.CenterScreen;
            fext.ShowDialog();
        }
    }
}
