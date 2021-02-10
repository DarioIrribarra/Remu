using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.Skins;
using System.IO;
using System.Data.SqlClient;
using DevExpress.XtraReports.UI;
using System.Collections;
using System.Transactions;
using System.Runtime.InteropServices;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;

namespace Labour
{
    public partial class frmEmpleado : DevExpress.XtraEditors.XtraForm, IBuscarTrabajador, ITrabajadorCombo, IEmpleadoClase, ICausalTermino, ICopiaEmp
    {
        [DllImport("user32")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32")]
        static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        const int MF_BYCOMMAND = 0;
        const int MF_DISABLED = 2;
        const int SC_CLOSE = 0xF060;

        #region "INTERFAZ"
        public void CargarBusqueda(string pContrato, int periodo)
        {
            //llamamos al metodo de busqueda definido en la region 'manejo de datos'
            fnNavDatos(pContrato, periodo);
            Navega.SetPosition(pContrato);
            User.UltimoRegistroVisto(pContrato + ";" + periodo, User.getUser());
        }
        #endregion

        #region "INTERFAZ TRABAJADOR COMBO"
        public void RecargarComboSalud()
        {
            datoCombobox.spllenaComboBox("SELECT id, nombre FROM isapre ORDER BY nombre", txtSalud, "id", "nombre", true);
            if (txtSalud.Properties.DataSource != null)
                txtSalud.ItemIndex = 0;
        }

        public void RecargarComboAfp()
        {
            datoCombobox.spllenaComboBox("SELECT id, nombre FROM afp ORDER BY nombre", txtAfp, "id", "nombre", true);
            if (txtAfp.Properties.DataSource != null)
                txtAfp.ItemIndex = 0;
        }

        public void RecargarComboCaja()
        {
            datoCombobox.spllenaComboBox("SELECT id, nombre FROM cajaPrevision ORDER BY nombre", txtCajaPrevision, "id", "nombre", true);
            if (txtCajaPrevision.Properties.DataSource != null)
                txtCajaPrevision.ItemIndex = 0;
        }

        //RECARGAR COMBOBOX NACIONALIDAD
        public void RecargarComboNacionalidad()
        {
            datoCombobox.spllenaComboBox("SELECT id, nombre FROM nacion ORDER BY nombre", txtNacion, "id", "nombre", true);
            if (txtNacion.Properties.DataSource != null)
                txtNacion.ItemIndex = 0;
        }

        //RECARGAR COMBOBOX PAGO
        public void RecargarComboPago()
        {
            datoCombobox.spllenaComboBox("SELECT id, nombre FROM formaPago ORDER BY nombre", txtfPago, "id", "nombre", true);
            if (txtfPago.Properties.DataSource != null)
                txtfPago.ItemIndex = 0;
        }

        //RECARGAR COMBOBOX BANCO
        public void RecargarComboBanco()
        {
            datoCombobox.spllenaComboBox("SELECT id, nombre FROM banco ORDER BY nombre", txtBanc, "id", "nombre", true);
            if (txtBanc.Properties.DataSource != null)
                txtBanc.ItemIndex = 0;
        }

        //RECARGAR COMBOBOX CARGO
        public void RecargarComboCargo()
        {
            datoCombobox.spllenaComboBox("SELECT id, nombre FROM cargo ORDER BY nombre", txtCargo, "id", "nombre", true);
            if (txtCargo.Properties.DataSource != null)
                txtCargo.ItemIndex = 0;
        }

        //RECARGAR COMBOX AREA
        public void RecargarComboArea()
        {
            datoCombobox.spllenaComboBox("SELECT id, nombre FROM area ORDER BY nombre", txtArea, "id", "nombre", true);
            if (txtArea.Properties.DataSource != null)
                txtArea.ItemIndex = 0;
        }

        //RECARGAR COMBOBOX CENTRO COSTO
        public void RecargarCombocCosto()
        {
            datoCombobox.spllenaComboBox("SELECT id, nombre FROM ccosto ORDER BY nombre", txtccosto, "id", "nombre", true);
            if (txtccosto.Properties.DataSource != null)
                txtccosto.ItemIndex = 0;
        }
        //RECARGAR COMBOBOX SUCURSAL
        public void RecargarComboSucursal()
        {
            datoCombobox.spllenaComboBox("SELECT codSucursal, descSucursal FROM SUCURSAL ORDER BY descSucursal", txtSucursal, "codSucursal", "descSucursal", true);
            if (txtSucursal.Properties.DataSource != null)
                txtSucursal.ItemIndex = 0;
        }
        //RECARGAR COMBOBOX CAUSAL
        public void RecargarComboCausal()
        {
            datoCombobox.spllenaComboBox("SELECT codcausal, desccausal FROM causalTermino ORDER BY desccausal", txtCausal, "codcausal", "desccausal", true);
            if (txtCausal.Properties.DataSource != null)
                txtCausal.ItemIndex = 0;
        }
        #endregion

        #region "INTERFAZ TRABAJADOR Y FORM CLASE"
        public void RecargarDatosClase()
        {
            //LLAMAR AL METODO
            //fnCargarClasesTrabajador(int.Parse(txtClase.EditValue.ToString()), PeriodoEmpleado);
        }
        #endregion

        #region "CAUSAL TERMINO"
        public void CargarComboCausal()
        {
            //fnComboCausal(txtCausal, true);
            datoCombobox.spllenaComboBox("SELECT codCausal, descCausal FROM causaltermino ORDER BY descCausal", txtCausal, "codCausal", "descCausal", true);
            if(txtCausal.Properties.DataSource != null)
                txtCausal.ItemIndex = 0;
        }
        #endregion

        #region "INTERFAZ FRM EMPLEADO Y FRMCARGATRABAJADOR"
        public void CargarCopia(string pRutNuevo, string pContratoCopia, int pPeriodoFichaCopia, string pPrimerNombre, string pSegundoNombre, string pApellidoPaterno, string pApellidoMaterno, string pNuevoContrato)
        {
            EsCopia = true;
            fnDatosCopia(pRutNuevo, pContratoCopia, pPeriodoFichaCopia, pPrimerNombre, pSegundoNombre, pApellidoPaterno, pApellidoMaterno, pNuevoContrato);
        }
        #endregion

        //VARIABLE PARA GUARDAR EL ULTIMO PERIODO DISPONIBLE
        private string UltimoPeriodo = "";

        //VARIABLE PARA GUARDAR EL NOMBRE COMPLETO DEL TRABAJADOR
        private string NombreCompleto = "";

        /*variable para guardar imagen que guardar el usuario*/
        Image imagenUsuario;
        /*variable para almacenar el ancho y el alto de la imagen del usuario*/
        Size OriginalImageSize;

        /*representa el lapiz o figura que muestra el mouse al posicionarse dentro de la imagen*/
        // public Pen lapiz;

        /*para estilizar el lapiz*/
        //public DashStyle stilo = DashStyle.DashDot;

        Boolean mouseClicked;
        Point startpoint = new Point();
        Point endpoint = new Point();
        Rectangle rectCropArea;

        //para obtener la ruta de la imagen
        string pathImage = "";

        //para saber si se realizo el crop de la imagen
        private bool recortar;

        private string pathFile = "";

        //para boton navegacional
        int posicion;

        Image img = null;

        //PARA GUARDAR PERIODO DE EMPLEADOR SELECCIONADO
        int PeriodoEmpleado = 0;

        //PARA GUARDAR LA OPCION DE ORDEN
        private string ElementoOrden = "";

        //BOTONES NAVEGACIONALES
        NavegaTrabajador Navega;

        //BOTON NUEVO
        Operacion op;

        //FICHA HISTORICA
        private bool FichaHistorica { get; set; } = false;
        /// <summary>
        /// Guarda el estatus de una ficha histórica.
        /// </summary>
        private int StatusHistorico { get; set; } = -1;

        private int StatusTrabajador = 0;

        /// <summary>
        /// Para saber si es una copia.
        /// </summary>
        private bool EsCopia = false;

        public frmEmpleado()
        {
            InitializeComponent();
            
            mouseClicked = false;
        }

        private void btnImagen_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            //DESPLIEGA UNA VENTANA PARA SELECCIONAR UNA IMAGEN Y POSTERIORMENTE CARGARLA EN EL PICTUREBOX
            if (Imagen.GenerarImagenFromUser() != null)
            {
                //CARGAMOS EL ARCHIVO
                img = Imagen.imagen;
                imagenUsuario = Imagen.imagen;

                //RUTA DEL ARCHIVO
                pathImage = Imagen.PathFile;

                //CARGAMOS IMAGEN
                Imagen.CargarImagen(imagenUsuario, pictureEmpleado);
            }       
        }

        private void frmEmpleado_Load(object sender, EventArgs e)
        {          
            var sm = GetSystemMenu(Handle, false);
            EnableMenuItem(sm, SC_CLOSE, MF_BYCOMMAND | MF_DISABLED);      

            //OBTENER ULTIMO PERIODO           
            UltimoPeriodo = Calculo.PeriodoObservado + "";          
          
            SkinManager.EnableFormSkins();
            SkinManager.EnableMdiFormSkins();

            //CARGAR COMBOS
            datoCombobox.spllenaComboBox("SELECT id, nombre FROM afp ORDER BY nombre", txtAfp, "id", "nombre", true);
            datoCombobox.spllenaComboBox("SELECT id, nombre FROM area ORDER BY nombre", txtArea, "id", "nombre", true);
            datoCombobox.spllenaComboBox("SELECT id, nombre FROM banco ORDER BY nombre", txtBanc, "id", "nombre", true);
            datoCombobox.spllenaComboBox("SELECT id, nombre FROM cargo ORDER BY nombre", txtCargo, "id", "nombre", true);
            datoCombobox.spllenaComboBox("SELECT id, nombre FROM ccosto ORDER BY nombre", txtccosto, "id", "nombre", true);
            datoCombobox.spllenaComboBox("SELECT idCiudad, descCiudad FROM ciudad ORDER BY descCiudad", txtCiudad, "idCiudad", "descCiudad", true);
            datoCombobox.spllenaComboBox("SELECT id, nombre FROM ecivil ORDER BY nombre", txteCivil, "id", "nombre", true);
            datoCombobox.spllenaComboBox("SELECT id, nombre FROM formaPago ORDER BY nombre", txtfPago, "id", "nombre", true);
            datoCombobox.spllenaComboBox("SELECT id, nombre FROM isapre ORDER BY nombre", txtSalud, "id", "nombre", true);
            datoCombobox.spllenaComboBox("SELECT id, nombre FROM nacion ORDER BY nombre", txtNacion, "id", "nombre", true);
            datoCombobox.spllenaComboBox("SELECT id, nombre FROM regimen", txtRegimen, "id", "nombre", true);
            datoCombobox.spllenaComboBox("SELECT id, nombre FROM cajaPrevision ORDER BY nombre", txtCajaPrevision, "id", "nombre", true);
            datoCombobox.spllenaComboBox("SELECT id, nombre FROM tipoCuenta ORDER BY nombre", txtTipoCuenta, "id", "nombre", true);
            datoCombobox.spllenaComboBox("SELECT codSucursal, descSucursal FROM SUCURSAL ORDER BY descSucursal", txtSucursal, "codSucursal", "descSucursal", true);
            datoCombobox.spllenaComboBox("SELECT codCausal, descCausal FROM causaltermino ORDER BY descCausal", txtCausal, "codCausal", "descCausal", true);
            datoCombobox.spllenaComboBox("SELECT codesc, descesc FROM escolaridad ORDER BY descesc", txtEstudios, "codesc", "descesc", true);
            datoCombobox.spllenaComboBox("SELECT id, deschor FROM horario ORDER BY deschor", txtHorario, "id", "deschor", true);
            datoCombobox.spllenaComboBox("SELECT id, descSin FROM sindicato ORDER BY descSin", txtSindicato, "id", "descSin", true);
            datoCombobox.spllenaComboBox("SELECT codComuna, descComuna FROM comuna ORDER BY region, descComuna", txtComuna, "codComuna", "descComuna", true);

            fnComboTramo(txtTramo);
            fnComboJubilado(txtjubilado);
            fnComboRegimenSalario(txtRegsal);
            fnComboTipoContrato(txtTipoCont);
            fnComboStatus(txtEstado);
            //fnComboCausal(txtCausal, true);
            fnComboSexo(txtSexo);
            fnSistema.fnComboJornada(txtJornadaLaboral);
            fnSistema.fnComboSupensionLaboral(txtSuspende);

            //COMBOBOX CLASE
            datoCombobox.spllenaComboBox("SELECT DISTINCT codClase, descClase FROM clase", txtClase, "codClase", "descClase", true);

            op = new Operacion();

            Navega = new NavegaTrabajador(radioOrden.Properties.Items[radioOrden.SelectedIndex].Description, comboVista);

            fnDefaultProperties();

            //ruta en la cual se guardaran las imagenes recortadas
            //pathFile = Directory.GetCurrentDirectory() + "\\";
            if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\AppRemu\\"))
                pathFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\AppRemu\\";
            else
            {
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\AppRemu\\");
                pathFile = Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\AppRemu\\").FullName;
            }                

            if (Navega.Universo > 0)
            {
                //Navega.SetPosition(NavegaTrabajador.LastSearch);
                if (User.GetLastView(User.getUser()) != "0" && User.VerificaLastView())
                {
                    fnNavDatos(User.GetLastView(User.getUser()), Convert.ToInt32(User.GetLastView(User.getUser(), 1)));
                    Navega.SetPosition(User.GetLastView(User.getUser()));                   
                }                    
                else
                {
                    //fnNavDatos(Navega.Listado[Navega.Posicion], Calculo.PeriodoObservado);
                    //User.UltimoRegistroVisto(Navega.Listado[Navega.Posicion], User.getUser());                    
                    fnNavDatos(Navega.Listado[Navega.Posicion].Contrato, Navega.Listado[Navega.Posicion].PeriodoPersona);
                    User.UltimoRegistroVisto(Navega.Listado[Navega.Posicion].Contrato + ";" + Navega.Listado[Navega.Posicion].PeriodoPersona, User.getUser());                    
                }                
            }                 

            //inicialmente la variable recortar tendrá el valor false
            recortar = false;          
        
            //Deshabilitar boton recortar
            btnRecortar.Enabled = false;
            btnReset.Enabled = false;
            //NO mostrar menu contextual en el pictureEdit
            pictureEmpleado.Properties.ShowMenu = false;
        }

        #region "CALCULO CROP IMAGEN"

        private void pictureEmpleado_MouseDown(object sender, MouseEventArgs e)
        {
            if (imagenUsuario == null) return;
            //if (pictureEmpleado.Image == Labour.Properties.Resources.SinFoto) return;
            //no hacemos nada si se hace click con el boton derecho
            if (e.Button == MouseButtons.Right) return;
            
            Cursor = Cursors.Cross;
            //CON EL MOUSE DOWN OBTENEMOS LA POSICION INCIAL (CUAL SE HACE CLICK CON EL MOUSE POR PRIMERA VEZ DENTRO DE LA IMAGEN)
            mouseClicked = true;
            //btnRecortar.Enabled = true;
            //btnReset.Enabled = true;
            //OBTENEMOS LA POSICIONES INICIALES
            startpoint.X = e.X;
            startpoint.Y = e.Y;

            endpoint.X = -1;
            endpoint.Y = -1;

            rectCropArea = new Rectangle(new Point(e.X, e.Y), new Size());
            
        }

        private void pictureEmpleado_MouseUp(object sender, MouseEventArgs e)
        {
            if (imagenUsuario == null) return;
            // if (pictureEmpleado.Image == Labour.Properties.Resources.SinFoto) return;

            mouseClicked = false;
            if (endpoint.X != -1)
            {
                Point currentPoint = new Point(e.X, e.Y);
            }

            endpoint.X = -1;
            endpoint.Y = -1;
            startpoint.X = -1;
            startpoint.Y = -1;

            Cursor = Cursors.Default;

            //SI LAS MEDIDAS SON CERO NO HABILITAMOS LOS BOTONES RESET Y RECORTAR
            if (rectCropArea.Width == 0 || rectCropArea.Height == 0)
            {
                btnRecortar.Enabled = false;
                // btnReset.Enabled = false;
            }
            else
            {
                //btnReset.Enabled = true;
                btnRecortar.Enabled = true;
            }
        }

        private void pictureEmpleado_MouseMove(object sender, MouseEventArgs e)
        {
            //OBTENEMOS LAS COORDENADAS QUE SE GENERAN CUANDO SE MUEVE EL MOUSE, PARTIENDO DESDE LA POSICION INICIAL
            //POSICION INICIAL: PRIMER CLICK DEL MOUSE(EVENTO MOUSE_DOWN)
            //accion cuando el mouse se mueve
            //tenemos que capturar las coordenadas
            if (imagenUsuario == null) return;
            //if (pictureEmpleado.Image == Labour.Properties.Resources.SinFoto) return;

            //btnReset.Enabled = true;
            //btnRecortar.Enabled = true;
            Point ptCurrent = new Point(e.X, e.Y);

            //SI LA VARIABLE MOUSE ES TRUE Y SE HIZO CLICK CON EL BOTON IZQUIERDO
            if (mouseClicked && e.Button == MouseButtons.Left)
            {
                endpoint = ptCurrent;

                if (e.X > startpoint.X && e.Y > startpoint.Y)
                {
                    rectCropArea.Width = e.X - startpoint.X;
                    rectCropArea.Height = e.Y - startpoint.Y;
                }
                else if (e.X < startpoint.X && e.Y > startpoint.Y)
                {
                    rectCropArea.Width = startpoint.X - e.X;
                    rectCropArea.Height = e.Y - startpoint.Y;
                    rectCropArea.X = e.X;
                    rectCropArea.Y = startpoint.Y;
                }
                else if (e.X > startpoint.X && e.Y < startpoint.Y)
                {
                    rectCropArea.Width = e.X - startpoint.X;
                    rectCropArea.Height = startpoint.Y - e.Y;
                    rectCropArea.X = startpoint.X;
                    rectCropArea.Y = e.Y;
                }
                else
                {
                    rectCropArea.Width = startpoint.X - e.X;
                    rectCropArea.Height = startpoint.Y - e.Y;
                    rectCropArea.X = e.X;
                    rectCropArea.Y = e.Y;
                }

                pictureEmpleado.Refresh();
            }
        }

        private void pictureEmpleado_Paint(object sender, PaintEventArgs e)
        {
            Pen drawline = new Pen(Color.Black, 2);
            drawline.DashStyle = DashStyle.Solid;
            e.Graphics.DrawRectangle(drawline, rectCropArea);
        }

        private void btnRecortar_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            //PARA EL CASO DE QUE NO SE SELECCIONE UNA AREA VALIDA
            if (rectCropArea.Width == 0 || rectCropArea.Height == 0)
            {
                XtraMessageBox.Show("Por favor selecciona una area", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //si es true es porque por lo menos se recortó la imagen una vez
            recortar = true;

            //HACEMOS CROP IMAGEN
            Imagen.CropImagen(pictureEmpleado, rectCropArea, pathFile);            

            btnRecortar.Enabled = false;
            btnReset.Enabled = true;

            //LIMPIAMOS RECTANGULO
            Imagen.CleanDraw(rectCropArea);

            //rectCropArea.Width = 0;
            //rectCropArea.Height = 0;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            recortar = false;
            //DEBERIA MODIFICAR EL PATHIMAGE Y APUNTAR HACIA LA IMAGEN SIN CROP (LA QUE ESTABA EN BD ORIGINALMENTE)
            btnRecortar.Enabled = false;
            if (pictureEmpleado.Image != null && imagenUsuario != null)
            {
                Imagen.CargarImagen(imagenUsuario, pictureEmpleado);
                //cargarImagen();
            }
            else {
                XtraMessageBox.Show("No hay Imagen", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Question);
            }
        }

        private void pictureEmpleado_Click(object sender, EventArgs e)
        {
            pictureEmpleado.Refresh();
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            //SI LA RESPUESTA ES SI USAMOS EL TRABAJADOR COMO PLANTILLA (EN OTRO FORMULARIO)
            //SI LA RESPUESTA ES NO SOLO LIMPIAMOS TODO           

            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            lblerror.Visible = false;

            fnAparienciaClean();

            //if (txtRut.Text == "") { txtRut.Focus(); return; }
            bool existen = false;
            //listado = ListadoContratosAnteriores(Calculo.PeriodoObservado);
            existen = Persona.ExistenRegistros(Calculo.PeriodoObservado);
           
            if (existen)
            {
                if (op.Cancela == false)
                {
                    DialogResult pregunta = XtraMessageBox.Show("¿Desea usar un trabajador como plantilla?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (pregunta == DialogResult.Yes)
                    {
                        //SI ES ASI MOSTRAMOS FORMULARIO PARA BUSCAR EMPLEADO Y POSTERIORMENTE CARGAR
                        bool activos = false, inactivos = false;
                        if (comboVista.Properties.Items[1].CheckState == CheckState.Checked)
                            inactivos = true;
                        if (comboVista.Properties.Items[0].CheckState == CheckState.Checked)
                            activos = true;     

                        frmCargarTrabajador frm = new frmCargarTrabajador();
                        frm.ShowActivos = activos;
                        frm.ShowInactivos = inactivos;
                        frm.Opener = this;
                        //frm.Owner = this;
                        frm.ShowDialog();
                    }
                    else
                    {
                        fnLimpiarCampos();
                        op.Cancela = true;
                        op.SetButtonProperties(btnNuevo, 2);
                        FichaHistorica = false;
     
                    }
                }
                else
                {
                    //RESET
                    
                    op.Cancela = false;
                    op.SetButtonProperties(btnNuevo, 1);
                    if (Navega.Universo > 0)
                    {
                        if (User.GetLastView(User.getUser()) != "0")
                        {
                            fnNavDatos(User.GetLastView(User.getUser()), Convert.ToInt32(User.GetLastView(User.getUser(), 1)));
                            Navega.SetPosition(User.GetLastView(User.getUser()));
                        }
                        else
                        {
                            fnNavDatos(Navega.Listado[Navega.Posicion].Contrato, Navega.Listado[Navega.Posicion].PeriodoPersona);
                            User.UltimoRegistroVisto(Navega.Listado[Navega.Posicion].Contrato + ";" + Navega.Listado[Navega.Posicion].PeriodoPersona, User.getUser());
                        }
                    }
                }
            }
            else
            {
                //SOLO LIMPIAMOS CAMPOS
                fnLimpiarCampos();
                if (op.Cancela)
                {

                    op.Cancela = false;
                    op.SetButtonProperties(btnNuevo, 1);
                    
                    if (Navega.Universo > 0)
                    {
                        //Navega.SetPosition(NavegaTrabajador.LastSearch);

                        if (User.GetLastView(User.getUser()) != "0")
                        {
                            fnNavDatos(User.GetLastView(User.getUser()), Convert.ToInt32(User.GetLastView(User.getUser(), 1)));
                            Navega.SetPosition(User.GetLastView(User.getUser()));
                        }
                        else
                        {
                            fnNavDatos(Navega.Listado[Navega.Posicion].Contrato, Navega.Listado[Navega.Posicion].PeriodoPersona);
                            User.UltimoRegistroVisto(Navega.Listado[Navega.Posicion].Contrato + ";" + Navega.Listado[Navega.Posicion].PeriodoPersona, User.getUser());
                        }
                    }

                }
                else
                {

                    op.Cancela = true;
                    op.SetButtonProperties(btnNuevo, 2);
                }
            }           
        }
        #endregion

        #region "LOGICA BOTONES NAVEGACIONALES"

        //LOGICA PARA BOTON SIGUIENTE 
        private void btnnext_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            fnAparienciaClean();

            bool cambio;            
            //PARA LIMPIAR EL RECTANGULO 
            rectCropArea = new Rectangle();
            rectCropArea.Height = 0;
            rectCropArea.Width = 0;

            //LLAMAMOS A FUNCION COMPARAR            
            if (txtcontrato.ReadOnly)
            {             
                cambio = fnComparar(txtcontrato.Text, PeriodoEmpleado);
                //SI LA VARIABLE CAMBIO ES TRUE ES PORQUE SE MODIFICÓ ALGUN VALOR
                if (cambio || img != null)
                {
                    DialogResult dialogo = XtraMessageBox.Show("¿Seguro quieres revisar otro registro, los cambios no guardados se perderán?", "Informacion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dialogo == DialogResult.Yes)
                    {
                        Navega.Siguiente();
                        fnNavDatos(Navega.Listado[Navega.Posicion].Contrato, Navega.Listado[Navega.Posicion].PeriodoPersona);                        
                     
                        //fnNext();
                        img = null;
                        lblerror.Visible = false;
                        recortar = false;
                    }
                }
                else
                {
                     Navega.Siguiente();
                     fnNavDatos(Navega.Listado[Navega.Posicion].Contrato, Navega.Listado[Navega.Posicion].PeriodoPersona);

                    //cambio es false porque lo no se modificaron valores
                    //SE PUEDE AVANZAR AL SIGUIENTE REGISTRO SIN MENSAJE
                    //fnNext();
                    img = null;
                    lblerror.Visible = false;

                }
            }
            else if (txtcontrato.ReadOnly == false)
            {             
                //ES UN REGISTRO EN BLANCO
                bool Novacios = fnCamposVacios();              
                //SI VARIABLE NOVACIOS ES TRUE DEBEMOS REALIZAR PREGUNTA ANTES DE CAMBIAR DE REGISTRO                
                if (Novacios || img != null)
                {
                    DialogResult dialogo = XtraMessageBox.Show("¿Seguro quieres revisar otro registro? los cambios no guardados se perderán.", "Informacion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dialogo == DialogResult.Yes) {
                        Navega.Siguiente();
                        fnNavDatos(Navega.Listado[Navega.Posicion].Contrato, Navega.Listado[Navega.Posicion].PeriodoPersona);

                        //fnNext();
                        img = null;                  
                        lblerror.Visible = false;
                    }
                }
                else
                {
                    //MOVEMOS SIN PROBLEMAS
                    Navega.Siguiente();
                    fnNavDatos(Navega.Listado[Navega.Posicion].Contrato, Navega.Listado[Navega.Posicion].PeriodoPersona);
                    //fnNext();
                    img = null;
                    lblerror.Visible = false;
                }
            }

            NavegaTrabajador.LastSearch = Navega.Listado[Navega.Posicion].Contrato;
            User.UltimoRegistroVisto(Navega.Listado[Navega.Posicion].Contrato + ";" + Navega.Listado[Navega.Posicion].PeriodoPersona, User.getUser());
        } 

        //LOGICA PARA BOTON ANTERIOR
        private void btnPrev_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            fnAparienciaClean();

            bool cambio;
            
            //PARA LIMPIAR EL RECTANGULO 
            rectCropArea = new Rectangle();
            rectCropArea.Height = 0;
            rectCropArea.Width = 0;

            //LLAMAMOS A LA FUNCION COMPARAR
            if (txtcontrato.ReadOnly)
            {
                cambio = fnComparar(txtcontrato.Text, PeriodoEmpleado);
                //SI CAMBIO ES TRUE ES PORQUE SE MODIFICO ALGUN CAMPO
                if (cambio || img != null)
                {
                    DialogResult dialogo = XtraMessageBox.Show("¿Seguro quieres revisar otro registro, los cambios no guardados se perderán?", "Informacion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dialogo == DialogResult.Yes)
                    {
                        Navega.Anterior();
                        fnNavDatos(Navega.Listado[Navega.Posicion].Contrato, Navega.Listado[Navega.Posicion].PeriodoPersona);

                        //MOVEMOS AL ANTERIOR
                        //fnPrev();
                        lblerror.Visible = false;
                        recortar = false;
                        img = null;

                    }
                }
                else
                {
                    Navega.Anterior();
                    fnNavDatos(Navega.Listado[Navega.Posicion].Contrato, Navega.Listado[Navega.Posicion].PeriodoPersona);
                    //SOLO MOVEMOS
                    //fnPrev();
                    lblerror.Visible = false;
                    recortar = false;

                }

            }
            else if (txtcontrato.ReadOnly == false)
            {
                //ES UN REGISTRO EN BLANCO
                bool Novacios = fnCamposVacios();
                //SI VARIABLE NOVACIOS ES TRUE DEBEMOS REALIZAR PREGUNTA ANTES DE CAMBIAR DE REGISTRO                
                if (Novacios || img != null)
                {
                    DialogResult dialogo = XtraMessageBox.Show("¿Seguro quieres revisar otro registro, los cambios no guardados se perderán?", "Informacion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dialogo == DialogResult.Yes) {
                        Navega.Anterior();
                        fnNavDatos(Navega.Listado[Navega.Posicion].Contrato, Navega.Listado[Navega.Posicion].PeriodoPersona);
                        //fnPrev();
                        img = null;
                        lblerror.Visible = false;
                        recortar = false;

                    }
                }
                else
                {
                    //MOVEMOS SIN PROBLEMAS
                    Navega.Anterior();
                    fnNavDatos(Navega.Listado[Navega.Posicion].Contrato, Navega.Listado[Navega.Posicion].PeriodoPersona);
                    //fnPrev();
                    lblerror.Visible = false;
                    recortar = false;

                }
            }

            NavegaTrabajador.LastSearch = Navega.Listado[Navega.Posicion].Contrato;
            User.UltimoRegistroVisto(Navega.Listado[Navega.Posicion].Contrato + ";" + Navega.Listado[Navega.Posicion].PeriodoPersona, User.getUser());
        }     

        //BOTON NAVEGACIONAL PRIMERO
        private void btnFirst_Click(object sender, EventArgs e)
        {
            //NUEVA SESION DE ACTIVIDAD
            Sesion.NuevaActividad();

            fnAparienciaClean();

            bool cambio;
          
            //PARA LIMPIAR EL RECTANGULO 
            rectCropArea = new Rectangle();
            rectCropArea.Height = 0;
            rectCropArea.Width = 0;

            //LLAMAMOS A LA FUNCION COMPARAR
            if (txtcontrato.ReadOnly)
            {
                cambio = fnComparar(txtcontrato.Text, PeriodoEmpleado);
                if (cambio || img != null)
                {
                    DialogResult dialogo = XtraMessageBox.Show("¿Seguro quieres revisar otro registro, los cambios no guardados se perderán?", "Informacion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dialogo == DialogResult.Yes)
                    {
                        Navega.Primer();
                        fnNavDatos(Navega.Listado[Navega.Posicion].Contrato, Navega.Listado[Navega.Posicion].PeriodoPersona);
                        img = null;                        
                    }
                }
                else {
                    //movemos sin problemas
                    Navega.Primer();
                    fnNavDatos(Navega.Listado[Navega.Posicion].Contrato, Navega.Listado[Navega.Posicion].PeriodoPersona);
                    //fnFirst();
                    lblerror.Visible = false;
                    recortar = false;
                }
            }
            else if (txtcontrato.ReadOnly == false)
            {
                //ES UN REGISTRO EN BLANCO
                bool Novacios = fnCamposVacios();
                //SI VARIABLE NOVACIOS ES TRUE DEBEMOS REALIZAR PREGUNTA ANTES DE CAMBIAR DE REGISTRO                
                if (Novacios || img != null)
                {
                    DialogResult dialogo = XtraMessageBox.Show("¿Seguro quieres revisar otro registro, los cambios no guardados se perderán?", "Informacion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dialogo == DialogResult.Yes)
                    {
                        Navega.Primer();
                        fnNavDatos(Navega.Listado[Navega.Posicion].Contrato, Navega.Listado[Navega.Posicion].PeriodoPersona);
                        img = null;                     
                    }
                }
                else
                {
                    //MOVEMOS SIN PROBLEMAS
                     Navega.Ultimo();
                     fnNavDatos(Navega.Listado[Navega.Posicion].Contrato, Navega.Listado[Navega.Posicion].PeriodoPersona);
                    //fnFirst();
                    lblerror.Visible = false;
                    recortar = false;
                }
            }

            NavegaTrabajador.LastSearch = Navega.Listado[Navega.Posicion].Contrato;
            User.UltimoRegistroVisto(Navega.Listado[Navega.Posicion].Contrato + ";" + Navega.Listado[Navega.Posicion].PeriodoPersona, User.getUser());
        }      

        //BOTON NAVEGACIONAL ULTIMO
        private void btnLast_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            fnAparienciaClean();

            bool cambio;
            //NavegaTrabajador.LastPosition = Navega.GetPosition();
            //PARA LIMPIAR EL RECTANGULO 
            rectCropArea = new Rectangle();
            rectCropArea.Height = 0;
            rectCropArea.Width = 0;

            if (txtcontrato.ReadOnly)
            {
                cambio = fnComparar(txtcontrato.Text, PeriodoEmpleado);
                //SI CAMBIO ES TRUE ES PORQUE SE MODIFICO UN CAMPO
                if (cambio || img != null)
                {
                    DialogResult dialogo = XtraMessageBox.Show("¿Seguro quieres revisar otro registro, los cambios no guardados se perderán?", "Informacion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dialogo == DialogResult.Yes) {
                        Navega.Ultimo();
                        fnNavDatos(Navega.Listado[Navega.Posicion].Contrato, Navega.Listado[Navega.Posicion].PeriodoPersona);
                        //fnLast();
                        lblerror.Visible = false;
                        recortar = false;
                        img = null;

                    }
                }
                else
                {
                    //MOVEMOS SIN PROBLEMAS
                    Navega.Ultimo();
                    fnNavDatos(Navega.Listado[Navega.Posicion].Contrato, Navega.Listado[Navega.Posicion].PeriodoPersona);
                    //fnLast();
  
                }
            }
            else if (txtcontrato.ReadOnly == false)
            {
                //ES UN REGISTRO EN BLANCO
                bool Novacios = fnCamposVacios();
                //SI VARIABLE NOVACIOS ES TRUE DEBEMOS REALIZAR PREGUNTA ANTES DE CAMBIAR DE REGISTRO                
                if (Novacios || img != null)
                {
                    DialogResult dialogo = XtraMessageBox.Show("¿Seguro quieres revisar otro registro, los cambios no guardados se perderán?", "Informacion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dialogo == DialogResult.Yes) {
                       Navega.Ultimo();
                       fnNavDatos(Navega.Listado[Navega.Posicion].Contrato, Navega.Listado[Navega.Posicion].PeriodoPersona);
                        // fnLast();
                        lblerror.Visible = false;
                        recortar = false;
                        img = null;
                     
                    }
                }
                else
                {
                    //MOVEMOS SIN PROBLEMAS
                    Navega.Ultimo();
                    fnNavDatos(Navega.Listado[Navega.Posicion].Contrato, Navega.Listado[Navega.Posicion].PeriodoPersona);
                    //fnLast();
                   
                    lblerror.Visible = false;
                    recortar = false;
                }
            }

            NavegaTrabajador.LastSearch = Navega.Listado[Navega.Posicion].Contrato;
            User.UltimoRegistroVisto(Navega.Listado[Navega.Posicion].Contrato + ";" + Navega.Listado[Navega.Posicion].PeriodoPersona, User.getUser());
        }


        #endregion

        #region "CONTROLES FORMULARIO"

        private void btnGrabar_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            fnAparienciaClean();

            //OBTENER EL AÑO Y MES ACTUAL
            int date = Calculo.PeriodoObservado;

            //VERIFICAR SI EL USUARIO TIENE BLOQUEO
            if (User.Bloqueado())
            { XtraMessageBox.Show("No tienes permitido editar esta ficha", "Información", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            //SI PERIODO DE REGISTRO ES DISTINTO DEL PERIODO ABIERTO, NO DEJAMOS REALIZAR CAMBIOS
            //if (FichaHistorica)
            //{ XtraMessageBox.Show("No puedes editar una ficha histórica", "Ficha", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (FichaHistorica)
            {
                //SE SETEO LA VARIABLE
                if (StatusHistorico != -1)
                {
                    //SE CAMBIA A ACTIVO???
                    if (Convert.ToInt32(txtEstado.EditValue) == 1)
                    {
                        DialogResult ad = XtraMessageBox.Show("¿Estás seguro que quieres reactivar esta ficha?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (ad == DialogResult.Yes)
                        {
                            //FECHA DE INICIO DE CONTRATO NO PUEDE SER SUPERIOR AL PERIODO OBSERVADO
                            if (Convert.ToDateTime(dtingr.DateTime) > fnSistema.UltimoDiaMes(date))
                            {
                                XtraMessageBox.Show("Por favor verifica que la fecha de inicio de contrato no sea mayor al periodo en observación", "Fecha inicio contrato", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                return;
                            }
                            //FECHA DE TERMINO DE CONTRATO ES MENOR A LA FECHA DEL PERIODO ABIERTO
                            if (dtSal.DateTime < fnSistema.PrimerDiaMes(date))
                            { XtraMessageBox.Show("Por favor verifica la fecha de termino de contrato", "Fecha", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                            if (dtSegCes.DateTime < dtingr.DateTime)
                            { XtraMessageBox.Show("Por favor verifica que la fecha de seguro de cesantía no sea inferior a la fecha de inicio de contrato", "Seguro Cesantía", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                            if (dtVac.DateTime > dtSal.DateTime)
                            { XtraMessageBox.Show("Por favor verifica que la fecha de vacaciones no sea superior a la fecha de termino de contrato", "Vacaciones", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                            if (fnFechasCorrectas(DateTime.Parse(dtingr.EditValue.ToString()), DateTime.Parse(dtSal.EditValue.ToString())) != "")
                            { XtraMessageBox.Show(fnFechasCorrectas(DateTime.Parse(dtingr.EditValue.ToString()), DateTime.Parse(dtSal.EditValue.ToString())), "Error fechas", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                            //ES UN STATUS DISTINTO EL DEL COMBO BOX Y EL DE LA BASE DE DATOS
                            ReactivaTrabajador(txtcontrato.Text, PeriodoEmpleado);
                        }
                    }
                    else
                    {
                        XtraMessageBox.Show("No puedes editar esta ficha", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }

                  
                }
                else
                {
                    XtraMessageBox.Show("No puedes editar esta ficha", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }

                return;
            }            

      
          
            //SI RUT ESTA EN BLANCO Y PASAPORTE ES DISTINTO DE CERO NO DEBERIAMOS VALIDAR EL RUT, 
            //YA QUE SE USARA PASAPORTE EN VEZ DE RUT 

            string RutaImg = "";

            //PREGUNTAMOS SI LA VARIABLE RECORTAR ES TRUE LO CUAL NOS DICE QUE SE RECORTO LA IMAGEN AL MENOS UNA VEZ
            if (recortar)
            {
                //MessageBox.Show("con recortar");
                //pasamos como imagen bd la imagen recortada
                RutaImg = pathFile + "empleado_resize.jpg";
            }
            else
            {
                //MessageBox.Show("sin recortar");
                //RUTA DE LA IMAGEN ORIGINAL
                if (imagenUsuario != null)
                {
                    RutaImg = pathImage;

                    //DEBEMOS CONVERTIR LA IMAGEN ORIGINAL
                    //ComprimirImagen(RutaImg, pathFile + "ConvertImg.jpg", 90);
                    Imagen.ComprimirImagen(RutaImg, pathFile + "ConvertImg.jpg", 90, 450, 600);
                    
                    //GUARDAMOS NUEVA RUTA DEL ARCHIVO GENERADO
                    RutaImg = pathFile + "ConvertImg.jpg";
                }                              
            }

            //SOLO LECTURA --> UPDATE
            if (txtRut.ReadOnly && txtcontrato.ReadOnly)
            {
                //QUITAR PUNTO Y COMA DE RUT
                //SI LA VARIABLE IMAGENUSUARIO ES NULL NO HAY IMAGEN, DEJAMOS LA VARIABLE RUTAIMG VACIA
                if (imagenUsuario == null) { RutaImg = ""; }

                if (txtClase.EditValue == null) { XtraMessageBox.Show("Debes seleccionar clase remuneracion", "Clase", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                if (txtCausal.EditValue == null) { XtraMessageBox.Show("Debes seleccionar una causal de termino", "Causal", MessageBoxButtons.OK, MessageBoxIcon.Warning); return;}

                //FECHA DE INICIO DE CONTRATO NO PUEDE SER SUPERIOR AL PERIODO OBSERVADO
                if (Convert.ToDateTime(dtingr.DateTime) > fnSistema.UltimoDiaMes(date))
                {
                    DialogResult adv = XtraMessageBox.Show("Estás tratando de ingresar una fecha de inicio de contrato superior al mes en observación, ¿Deseas continuar de todas maneras?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    //XtraMessageBox.Show("Por favor verifica que la fecha de inicio de contrato no sea mayor al periodo en observación", "Fecha inicio contrato", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    if(adv == DialogResult.No)
                        return;
                }
                //FECHA DE TERMINO DE CONTRATO ES MENOR A LA FECHA DEL PERIODO ABIERTO
                if (dtSal.DateTime < fnSistema.PrimerDiaMes(date))
                { XtraMessageBox.Show("Por favor verifica la fecha de termino de contrato", "Fecha", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
                
                if (dtSegCes.DateTime < dtingr.DateTime)
                { XtraMessageBox.Show("Por favor verifica que la fecha de seguro de cesantía no sea inferior a la fecha de inicio de contrato", "Seguro Cesantía", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                //if (dtVac.DateTime < dtingr.DateTime)
                //{ XtraMessageBox.Show("Por favor verifica que la fecha de vacaciones no sea inferior a la fecha de inicio de contrato", "Vacaciones", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                if (dtSegCes.DateTime > dtSal.DateTime)
                { XtraMessageBox.Show("Por favor verifica que la fecha de seguro de cesantía no sea superior a la fecha de termino de contrato", "Seguro Cesantía", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                if (dtVac.DateTime > dtSal.DateTime)
                { XtraMessageBox.Show("Por favor verifica que la fecha de vacaciones no sea superior a la fecha de termino de contrato", "Vacaciones", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                //if (dtProg.DateTime < dtingr.DateTime)
                //{ XtraMessageBox.Show("Por favor verifica que la fecha de progresivos no sea inferior a la fecha de inicio de contrato", "Progresivos", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                //if (dtProg.DateTime > dtSal.DateTime)
                //{ XtraMessageBox.Show("Por favor verifica que la fecha de progresivos no sea superior a la fecha de termino de contrato", "Progresivos", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                if (fnFechasCorrectas(DateTime.Parse(dtingr.EditValue.ToString()), DateTime.Parse(dtSal.EditValue.ToString())) != "")
                    { XtraMessageBox.Show(fnFechasCorrectas(DateTime.Parse(dtingr.EditValue.ToString()), DateTime.Parse(dtSal.EditValue.ToString())), "Error fechas", MessageBoxButtons.OK, MessageBoxIcon.Warning);return;}

                //if (VerificaFechaNuevoContrato(txtRut.Text, date, Convert.ToDateTime(dtingr.EditValue), Convert.ToDateTime(dtSal.EditValue), txtcontrato.Text))
                //{ XtraMessageBox.Show("Verifica las fechas contractuales antes de continuar", "Fechas", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                if (txtSucursal.Properties.DataSource == null || txtSucursal.EditValue.ToString() == "")
                { XtraMessageBox.Show("Por favor selecciona una sucursal valida", "Sucursal", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                if (txtHorario.Properties.DataSource == null || txtHorario.EditValue.ToString() == "")
                { XtraMessageBox.Show("por favor selecciona un horario válido", "Horario", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                if (txtSindicato.Properties.DataSource == null)
                { XtraMessageBox.Show("Por favor seleccionar un una opcion válida en combo sindicato", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                //REALIZAMOS UPDATE
                // fnModificarTrabajador(fnSistema.fnExtraerCaracteres(txtRut.Text), txtNombre, txtapePaterno, txtApeMat, txtDirec, txtCiudad,
                // txtArea, txtccosto, dtFecNac, txtNacion, txteCivil, txtFono, dtingr, dtSal, txtCargo, txtTipoCont,
                // txtRegsal, txtRegimen, txtAfp, txtSalud, dtSegCes, txtTramo, txtfPago, txtBanc, txtcuenta,
                // dtVac, dtProg, spProg, txtEstado, RutaImg, txtcontrato, txtjubilado, txtCajaPrevision, txtPasaporte, txtTipoCuenta, txtClase, txtCausal, txtSexo);

                ModificarTrabajadorTransaccion(fnSistema.fnExtraerCaracteres(txtRut.Text), txtNombre, txtapePaterno, txtApeMat, txtDirec, txtCiudad,
                txtArea, txtccosto, dtFecNac, txtNacion, txteCivil, txtFono, dtingr, dtSal, txtCargo, txtTipoCont,
                txtRegsal, txtRegimen, txtAfp, txtSalud, dtSegCes, txtTramo, txtfPago, txtBanc, txtcuenta,
                dtVac, dtProg, spProg, txtEstado, RutaImg, txtcontrato, txtjubilado, txtCajaPrevision, txtPasaporte,
                txtTipoCuenta, txtClase, txtCausal, txtSexo, txtSucursal, txtFun, cbPrivado, txtEmail, txtEstudios, 
                txtFonoEmer, txtNombreEmer, txtTalla, txtCalzado, txtHorario, txtJornadaLaboral, txtSindicato, txtComuna, txtSuspende);
            }
            else
            {
                if (txtRut.Text == "") { XtraMessageBox.Show("Rut no válido", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                //VERIFICAMOS QUE EL RUT SEA VALIDO
                //QUITAR PUNTOS Y GUION
                string rut = fnSistema.fnExtraerCaracteres(txtRut.Text);

                //ENMASCARAR Y DESPUES VALIDAR
                string enmascarado = "";
                enmascarado = fnSistema.fEnmascaraRut(rut);
                if (enmascarado.Length < 8 || enmascarado.Length > 9) { XtraMessageBox.Show("Rut no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

                //AHORA VALIDAMOS RUT
                bool rutValido = fnSistema.fValidaRut(rut);
                if (rutValido == false) { XtraMessageBox.Show("Rut no Valido", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Error); txtRut.Focus(); return; }

                //VALIDAR QUE CONTRATO NO ESTE VACIO
                if (txtcontrato.Text == "") { XtraMessageBox.Show("Contrato No valido", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }                

                //SI LA VARIABLE IMAGENUSUARIO ES NULL NO HAY IMAGEN
                //SI NO HAY IMAGEN LA VARIABLE DONDE GUARDAMOS LA RUTA LA DEJAMOS EN BLANCO
                if (imagenUsuario == null) { RutaImg = ""; }
                //if (imagenUsuario == null) { XtraMessageBox.Show("Se necesita una imagen del trabajador"); return; }

                //VALIDAR CAMPOS EN BLANCO
                bool blanco = fnCamposBlanco();
                if (blanco == true) return;

                //REALIZAMOS INSERT
                //DEBEMOS VERIFICAR SI EL CONTRATO A INGRESAR YA EXISTE EN LA BASE DE DATOS
                bool ContratoBd = Persona.ExisteContrato(txtcontrato.Text, date);
                //bool ContratoBd = fnVerContrato(txtcontrato.Text, date);

                if (ContratoBd) { XtraMessageBox.Show("El Contrato que Intentas ingresar ya está registrado", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                if (txtClase.EditValue == null) { XtraMessageBox.Show("Debes seleccionar clase remuneracion", "Clase", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                if (txtCausal.EditValue == null) { XtraMessageBox.Show("Debes seleccionar una causal de termino", "Causal", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                //FECHA DE INICIO DE CONTRATO NO PUEDE SER SUPERIOR AL PERIODO OBSERVADO
                if (Convert.ToDateTime(dtingr.DateTime) > fnSistema.UltimoDiaMes(date))
                {
                    DialogResult adv = XtraMessageBox.Show("Estás tratando de ingresar un fecha de inicio de contrato superior al mes en observación, ¿Deseas continuar de todas maneras?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    //XtraMessageBox.Show("Por favor verifica que la fecha de inicio de contrato no sea mayor al periodo en observación", "Fecha inicio contrato", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    if(adv == DialogResult.No)
                        return;
                }

                //FECHA DE TERMINO DE CONTRATO ES MENOR A LA FECHA DEL PERIODO ABIERTO
                if (dtSal.DateTime < fnSistema.PrimerDiaMes(date))
                { XtraMessageBox.Show("Por favor verifica la fecha de termino de contrato", "Fecha", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                if (dtSegCes.DateTime < dtingr.DateTime)
                { XtraMessageBox.Show("Por favor verifica que la fecha de seguro de cesantía no sea inferior a la fecha de inicio de contrato", "Seguro Cesantía", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                //if (dtVac.DateTime < dtingr.DateTime)
                //{ XtraMessageBox.Show("Por favor verifica que la fecha de vacaciones no sea inferior a la fecha de inicio de contrato", "Vacaciones", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                if (dtSegCes.DateTime > dtSal.DateTime)
                { XtraMessageBox.Show("Por favor verifica que la fecha de seguro de cesantía no sea superior a la fecha de termino de contrato", "Seguro Cesantía", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                if (dtVac.DateTime > dtSal.DateTime)
                { XtraMessageBox.Show("Por favor verifica que la fecha de vacaciones no sea superior a la fecha de termino de contrato", "Vacaciones", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                //if (dtProg.DateTime < dtingr.DateTime)
                //{ XtraMessageBox.Show("Por favor verifica que la fecha de progresivos no sea inferior a la fecha de inicio de contrato", "Progresivos", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                //if (dtProg.DateTime > dtSal.DateTime)
                //{ XtraMessageBox.Show("Por favor verifica que la fecha de progresivos no sea superior a la fecha de termino de contrato", "Progresivos", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                if (fnFechasCorrectas(DateTime.Parse(dtingr.EditValue.ToString()), DateTime.Parse(dtSal.EditValue.ToString())) != "")
                { XtraMessageBox.Show(fnFechasCorrectas(DateTime.Parse(dtingr.EditValue.ToString()), DateTime.Parse(dtSal.EditValue.ToString())), "Error fechas", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                
                //if (VerificaFechaNuevoContrato(enmascarado, date, Convert.ToDateTime(dtingr.EditValue), Convert.ToDateTime(dtSal.EditValue), ""))
                //{ XtraMessageBox.Show("Verifica las fechas contractuales antes de continuar", "Fechas", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                if (txtSucursal.Properties.DataSource == null || txtSucursal.EditValue.ToString() == "")
                { XtraMessageBox.Show("Por favor selecciona una sucursal valida", "Sucursal", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                if (txtHorario.Properties.DataSource == null || txtHorario.EditValue.ToString() == "")
                { XtraMessageBox.Show("por favor selecciona un horario válido", "Horario", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                if (txtSindicato.Properties.DataSource == null)
                { XtraMessageBox.Show("Por favor seleccionar un una opcion válida en combo sindicato", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                //LLAMAMOS A LA FUNCION INGRESO EMPLEADO
                //fnIngresoTrabajador(enmascarado, txtNombre, txtapePaterno, txtApeMat, txtDirec, txtCiudad,
                //      txtArea, txtccosto, dtFecNac, txtNacion, txteCivil, txtFono, dtingr, dtSal, txtCargo, txtTipoCont,
                //    txtRegsal, txtRegimen, txtAfp, txtSalud, dtSegCes, txtTramo, txtfPago, txtBanc, txtcuenta, dtVac,
                //  dtProg, spProg, txtEstado, RutaImg, txtPasaporte, txtcontrato, date, txtjubilado, txtCajaPrevision, txtTipoCuenta, txtClase, txtCausal, txtSexo);

                NuevoTrabajadorTransaccion(enmascarado, txtNombre, txtapePaterno, txtApeMat, txtDirec, txtCiudad,
                        txtArea, txtccosto, dtFecNac, txtNacion, txteCivil, txtFono, dtingr, dtSal, txtCargo, txtTipoCont,
                        txtRegsal, txtRegimen, txtAfp, txtSalud, dtSegCes, txtTramo, txtfPago, txtBanc, txtcuenta, dtVac,
                        dtProg, spProg, txtEstado, RutaImg, txtPasaporte, txtcontrato, date, txtjubilado, txtCajaPrevision,
                        txtTipoCuenta, txtClase, txtCausal, txtSexo, txtSucursal, txtFun, cbPrivado, txtEmail, txtEstudios, 
                        txtFonoEmer, txtNombreEmer, txtTalla, txtCalzado, txtHorario, txtJornadaLaboral, txtSindicato, txtComuna, txtSuspende);
            }
        }

        //SOLO PERMITIR NUMEROS EN CAMPO TELEFONO
        private void txtFono_KeyPress(object sender, KeyPressEventArgs e)
        {
           
        }

        //SOLO LETRAS EN CAMPO APELLIDO PATERNO
        private void txtapePaterno_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsLetter(e.KeyChar) || e.KeyChar == (char)32)
            {
                e.Handled = false;
            }
            else if (char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        //SOLO LETRAS EN CAMPO APELLIDO MATERNO
        private void txtApeMat_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsLetter(e.KeyChar) || e.KeyChar == (char)32)
            {
                e.Handled = false;
            }
            else if (char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        //SOLO NUMEROS EN CAMPO RUT
        private void txtRut_KeyPress(object sender, KeyPressEventArgs e)
        {
            //CARACTER 45-> '-'
            //CARACTER 46-> '.'
            if (e.KeyChar == (char)45)
            {
                e.Handled = false;
            }
            else if (char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (e.KeyChar == (char)46)
            {
                e.Handled = false;
            }
            else if (e.KeyChar == (char)75)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            fnAparienciaClean();

            bool VerInactivos = false, veractivos = false;
            if (comboVista.Properties.Items[1].CheckState == CheckState.Checked)
                VerInactivos = true;
            if (comboVista.Properties.Items[0].CheckState == CheckState.Checked)
                veractivos = true;

            //LANZAR VENTANA PARA BUSQUEDA DE TRABAJADOR POR NOMBRE O POR RUT
            frmBuscarTrabajador btrab = new frmBuscarTrabajador();
            btrab.StartPosition = FormStartPosition.CenterScreen;
            btrab.ShowInactivos = VerInactivos;
            btrab.ShowActivos = veractivos;
            btrab.opener = this;
            btrab.ShowDialog();
        }

        private void txtPasaporte_KeyPress(object sender, KeyPressEventArgs e)
        {
            //NO PERMITIR VALORES RAROS -> "#$&/%&/&/(&/(/*-            
            if (e.KeyChar == (char)97)
            {
                e.Handled = false;
            }
            else if (char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (e.KeyChar == (char)100)
            {
                e.Handled = false;
            }
            else if (e.KeyChar == (char)115)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }

        }

        private void txtcuenta_KeyPress(object sender, KeyPressEventArgs e)
        {
            //SOLO NUMEROS
            if (char.IsControl(e.KeyChar) || e.KeyChar == (char)45)
            {
                e.Handled = false;
            }
            else if (char.IsNumber(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void frmEmpleado_FormClosing(object sender, FormClosingEventArgs e)
        {
            //LLAMAMOS AL METODO ELIMINAR IMAGENES TEMPORALES
            //fnCleanImage();
            Imagen.CleanImage(pathFile);
        }

        private void txtRut_KeyDown(object sender, KeyEventArgs e)
        {
           
            if (e.KeyData == Keys.Enter)
            {
                //CODE
                ValidaRut();
            }
        }

        private void ValidaRut()
        {
            bool rutValida;
            string cadena = "";

            //CODE
            if (txtRut.Text == "") { lblerror.Visible = true; lblerror.Text = "Rut no valido"; return; }
            if (txtRut.Text.Contains(".") || txtRut.Text.Contains("-"))
            {
                cadena = fnSistema.fnExtraerCaracteres(txtRut.Text);
            }
            else
            {
                cadena = txtRut.Text;
            }

            if (cadena.Length < 8 || cadena.Length > 9)
            {
                //CADENA NO VALIDA
                lblerror.Visible = true;
                lblerror.Text = "Rut no valido";
                //FORMATEAR
                txtRut.Text = fnSistema.fFormatearRut2(cadena);
                return;
            }
            else
            {
                //validar rut
                cadena = fnSistema.fEnmascaraRut(cadena);
                rutValida = fnSistema.fValidaRut(cadena);
                if (rutValida == false)
                {
                    lblerror.Visible = true;
                    lblerror.Text = "Rut no valido";
                    //FORMATEAR
                    txtRut.Text = fnSistema.fFormatearRut2(cadena);
                    return;
                }
                else
                {
                    //TODO OK
                    txtRut.Text = fnSistema.fFormatearRut2(cadena);
                    txtRut.EnterMoveNextControl = true;
                    lblerror.Text = "";
                    lblerror.Visible = false;

                    //Generar codigo correlativo para contrato
                    string cont = Calculo.GetNuevoContrato(cadena);
                    if(txtRut.ReadOnly == false)
                        txtcontrato.Text = cont;

                }
            }
        }

        private void ValidaContrato()
        {
            if (txtcontrato.Text == "")
            {
                lblerror.Visible = true;
                lblerror.Text = "Ingrese contrato";
                return ;
            }
            else
            {
                if (txtcontrato.ReadOnly == false)
                {

                    //VALIDAMOS QUE EL CODIGO DE CONTRATO QUE SE INTENTA INGRESAR NO EXISTE EN BD                            
                    //bool existe = fnVerificaContrato(txtcontrato.Text, Calculo.PeriodoObservado);
                    bool existe = Persona.ExisteContrato(txtcontrato.Text, Calculo.PeriodoObservado);
                    if (existe)
                    {
                        //CONTRATO EXISTE EN BD
                        lblerror.Visible = true;
                        lblerror.Text = "El contrato que intentas ingresar ya existe";
                        return ;
                    }
                    else
                    {
                        lblerror.Visible = false;
                    }
                }
            }
        }

        private void ValidaNacimiento()
        {
            if ((DateTime.Now.Date.Year - dtFecNac.DateTime.Year) < 15)
            {
                lblerror.Visible = true;
                lblerror.Text = "Por favor verifica la fecha de nacimiento";
            }
            else
            {
                lblerror.Visible = false;
                lblerror.Text = "";
            }
        }

        #endregion

        #region "MANEJO DATOS"
        //NUEVO REGISTRO DE EMPLEADO
        //PARAMETROS DE ENTRADA
        /*
         * -----------------------------------------------------------------
         * RUT  @@STRING                                        TEXTEDIT
         * CONTRATO @@STRING                                    TEXTEDIT
         * NOMBRE @@STRING                                      TEXTEDIT
         * APELLIDO PATERNO @@STRING                            TEXTEDIT
         * APELLIDO MATERNO @@STRING                            TEXTEDIT
         * DIRECCION   @@STRING                                 TEXTEDIT
         * CIUDAD => TABLA CIUDAD  @fk @INT                     LOOK UP EDIT
         * EMPRESA => TABLA EMPRESA @FK @INT                    LOOK UP EDIT
         * AREA => TABLA AREA  @FK @INT                         LOOK UP EDIT
         * CENTRO COSTRO => TABLA CENTRO COSTO @FK INT          LOOK UP EDIT
         * FECHA NACIMIENTO                                     DATE EDIT
         * NACION => TABLA NACION(PAIS) @FK INT                 LOOK UP EDIT
         * ESTADO CIVIL => TABLA ESTADOCIVIL @FK INT            LOOK UP EDIT
         * TELEFONO @STRING                                     TEXTEDIT
         * INGRESO @DATE                                        DATE EDIT
         * SALIDA @DATE                                         DATE EDIT
         * CARGO => TABLA CARGO @FK INT                         LOOK UP EDIT
         * TIPOCONTRATO => TABLA CONTRATO @FK INT               LOOK UP EDIT
         * REGIMENSALARIO => TABLA @FK INT                      LOOK UP EDIT
         * REGIMEN => TABLA @FK INT                             LOOK UP EDIT
         * AFP => TABLA PREVISION @FK INT                       LOOK UP EDIT
         * SALUD => TABLA SALUD @FK INT                         LOOK UP EDIT
         * FECHA SEGURO CESANTIA @DATE                          DATE EDIT
         * TRAMO => TABLA TRAMO @FK INT                         LOOK UP EDIT
         * FORMAPAGO => TABLA PAGO @FK INT                      LOOK UP EDIT
         * BANCO => TABLA BANCO @FK INT                         LOOK UP EDIT
         * CUENTA @STRING                                       TEXT EDIT
         * FECHA VACACIONES @DATE                               DATE EDIT
         * FECHA PROGRESIVO    @DATE                            DATE EDIT
         * AÑOS PROGRESIVO     @INT                             SPIN CONTROL
         * ESTADO => TABLA ESTADO  @FK INT                      LOOK UP EDIT
         * RUTA FOTO @VARBINARY (REPRESENTA LA IMAGEN)          PICTURE EDIT
         * PASAPORTE @STRING (OPTIONAL)                         TEXT EDIT
         * TIPO DE CUENTA (CUENTA BANCARIA)                     LOOK UP EDIT
         * -------------------------------------------------------------------
         */

        //VERSION 2 PARA INGRESO DE TRABAJOR (USANDO TRANSACCION SQL)
        private void NuevoTrabajadorTransaccion(string pRut, TextEdit pNombre, TextEdit pApePat, TextEdit pApeMat,
            TextEdit pDirec, LookUpEdit pCiudad, LookUpEdit pArea, LookUpEdit pcCosto,
            DateEdit pNac, LookUpEdit pNacion, LookUpEdit peCivil, TextEdit pFono, DateEdit pIngreso,
            DateEdit pSalida, LookUpEdit pCargo, LookUpEdit ptContr, LookUpEdit pregSal, LookUpEdit pReg,
            LookUpEdit pAfp, LookUpEdit pSalud, DateEdit psegCes, LookUpEdit pTramo, LookUpEdit pformPag,
            LookUpEdit pBanco, TextEdit pCuenta, DateEdit pVac, DateEdit pfeProg, SpinEdit pAnProg,
            LookUpEdit pEstado, string pImage, TextEdit pPasa, TextEdit pContrato, int pDate,
            LookUpEdit pJub, LookUpEdit pPrev, LookUpEdit pTipoCuenta, LookUpEdit pClase, LookUpEdit pCausal,
            LookUpEdit pSexo, LookUpEdit pcodSucursal, TextEdit pFun, CheckEdit pPrivado, TextEdit pMail, 
            TextEdit pEscolaridad, TextEdit pNumerEmer, TextEdit pNomEmer, TextEdit pTalla, TextEdit pCalzado, 
            LookUpEdit pHorario, LookUpEdit pJornada, LookUpEdit pSindicato, LookUpEdit pComuna, LookUpEdit pSuspension)
        {

            //--------------------------------------------------
            //QUERY SQL PARA INGRESAR EL NUEVO TRABAJADOR       |
            //--------------------------------------------------

            string sqlTrabajador = "INSERT INTO trabajador " +
                "(rut, nombre, apepaterno, apematerno, direccion," +
                "ciudad, sucursal, area, ccosto, fechanac, nacion, ecivil, telefono, ingreso, salida, cargo," +
                " tipocontrato, regimenSalario, regimen, afp, salud, fechaSegCes, tramo, formapago, banco, " +
                "cuenta, fechavacacion, fechaprogresivo, anosprogresivo, status, rutafoto, pasaporte, contrato," +
                " anomes, jubilado, cajaPrevision, tipoCuenta, clase, causal, sexo, fun, privado, mail, esco, " +
                " numemer, nomemer, talla, calzado, horario, jornada, sindicato, comuna, suslab) " +
                "VALUES (" +
                "@pRut, @pNombre, @pApePat, @pApeMat, @pDirec, @pCiudad, @pSucursal, @pArea, @pcCosto, @pNac, @pNacion," +
                "@peCivil, @pFono, @pIngreso, @pSalida, @pCargo, @ptContr, @pregSal, @pReg, @pAfp, @pSalud, @psegCes," +
                "@pTramo, @pformPag, @pBanco, @pCuenta, @pVac, @pfeProg, @pAnProg, @pEstado, @pImage, @pPasa, " +
                "@pContrato, @pAnoMes, @pJub, @pPrev, @pTipoCuenta, @pclase, @pCausal, @psexo, @pFun, " +
                "@pPrivado, @pMail, @pEsco, @pNumEmer, @pNomEmer, @pTalla, @pCalzado, @pHorario, " +
                "@pJornada, @pSindicato, @pComuna, @pSuspension)";

            //-------------------------------------------------------------------
            // QUERY PARA INSERT MASIVO DE ITEM DE ACUERDO A CLASE SELECCIONADA |
            //-------------------------------------------------------------------
            string sqlItemClase = "";
            //string sqlItemClase = "INSERT INTO itemtrabajador(rut, contrato, anomes, coditem, formula, tipo, orden, numitem) " +
            //                      "SELECT rut, contrato, anomes, itemclase.item, itemclase.formula, tipo, orden, row_number() OVER(ORDER BY numitem) FROM( " +
            //                            "SELECT rut, contrato, anomes, itemClase.item, itemClase.formula, tipo, orden, numitem " +
            //                                    "FROM trabajador inner JOIN clase on clase.codClase = trabajador.clase " +
            //                                    "INNER JOIN itemclase on itemClase.clase = clase.codClase " +
            //                                    "INNER JOIN item on item.coditem = itemClase.item " +
            //                                    "WHERE contrato = @pcontrato AND clase.codClase = @pclase AND trabajador.anomes = @periodo " +
            //                                    "EXCEPT " +
            //                                    "SELECT itemTrabajador.rut, itemTrabajador.contrato, trabajador.anomes, " +
            //                                    "itemTrabajador.coditem, itemTrabajador.formula, itemTrabajador.tipo, itemtrabajador.orden, itemclase.numitem " +
            //                                    "FROM itemTrabajador INNER JOIN trabajador on trabajador.contrato = itemTrabajador.contrato " +
            //                                    "INNER JOIN clase on clase.codClase = trabajador.clase " +
            //                                    "INNER JOIN itemClase on itemClase.clase = clase.codClase " +
            //                                    "WHERE itemTrabajador.contrato = @pcontrato " +
            //                                   "AND clase.codClase = @pclase AND itemtrabajador.anomes = @periodo " +
            //                       ")itemclase";

            sqlItemClase = Calculo.GetItemClaseSql();


            //PARA SABER SI LA EDAD DEL TRABAJADOR ESTA DENTRO DE LO PERMITIDO (MAYOR A 15)
            bool edadValida = false, transaccionCorrecta = false;

            //PARA GUARDAR EL VALOR DE PREVISION, SALUD, Y CAJA
            int afp = 0, salud = 0, caja = 0;

            //PARA MANIPULAR SI ES JUBILADO Y EL PASAPORTE
            string pasaporte = "";
            Int16 jubilado = 0;

            if (pAfp.Enabled)
                afp = Convert.ToInt32(pAfp.EditValue);
            if (pSalud.Enabled)
                salud = Convert.ToInt32(pSalud.EditValue);
            if (pPrev.Enabled)
                caja = Convert.ToInt32(pPrev.EditValue);

            //*********************************************************************************
            // SI PASAPORTE VIENE EN BLANCO GUARDAMOS 0 QUE REPRESENTA QUE NO TIENE PASAPORTE |
            //*********************************************************************************

            if (pPasa.Text != "")
                pasaporte = pPasa.Text;

            //SI EL COMBOBOX JUBILADO ESTA HABILITADO GUARDAMOS SU CONTENIDO, CASO CONTRARIO GUARDAMOS CON '0'
            if (pJub.Enabled) jubilado = Convert.ToInt16(pJub.EditValue.ToString());

            edadValida = fnMayorEdad(pNac.DateTime);
            if (edadValida == false)
            {
                XtraMessageBox.Show("Por favor verifique la fecha de nacimiento", "Nacimiento", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //***************************************************************************
            // COMIENZO TRANSACCION...                                                  |
            //***************************************************************************
            SqlCommand cmd;
            SqlTransaction tran;
            SqlDataReader rd;
            SqlConnection cn;
            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        //INICIALIZAMOS LA TRANSACCION...
                        tran = cn.BeginTransaction();
                        try
                        {
                            // @1->INGRESAMOS DATA TRABAJADOR
                            using (cmd = new SqlCommand(sqlTrabajador, cn))
                            {
                                //PARAMETROS...
                                cmd.Parameters.Add(new SqlParameter("@pRut", pRut));
                                cmd.Parameters.Add(new SqlParameter("@pNombre", pNombre.Text));
                                cmd.Parameters.Add(new SqlParameter("@pApePat", pApePat.Text));
                                cmd.Parameters.Add(new SqlParameter("@pApeMat", pApeMat.Text));
                                cmd.Parameters.Add(new SqlParameter("@pDirec", pDirec.Text));
                                cmd.Parameters.Add(new SqlParameter("@pCiudad", pCiudad.EditValue));
                                cmd.Parameters.Add(new SqlParameter("@pSucursal", pcodSucursal.EditValue));
                                cmd.Parameters.Add(new SqlParameter("@pArea", pArea.EditValue));
                                cmd.Parameters.Add(new SqlParameter("@pcCosto", pcCosto.EditValue));
                                cmd.Parameters.Add(new SqlParameter("@pNac", pNac.EditValue));
                                cmd.Parameters.Add(new SqlParameter("@pNacion", pNacion.EditValue));
                                cmd.Parameters.Add(new SqlParameter("@peCivil", peCivil.EditValue));
                                cmd.Parameters.Add(new SqlParameter("@pFono", pFono.Text));
                                cmd.Parameters.Add(new SqlParameter("@pIngreso", pIngreso.EditValue));
                                cmd.Parameters.Add(new SqlParameter("@pSalida", pSalida.EditValue));
                                cmd.Parameters.Add(new SqlParameter("@pCargo", pCargo.EditValue));
                                cmd.Parameters.Add(new SqlParameter("@ptContr", ptContr.EditValue));
                                cmd.Parameters.Add(new SqlParameter("@pregSal", pregSal.EditValue));
                                cmd.Parameters.Add(new SqlParameter("@pReg", pReg.EditValue));
                                cmd.Parameters.Add(new SqlParameter("@pAfp", afp));
                                cmd.Parameters.Add(new SqlParameter("@pSalud", salud));
                                cmd.Parameters.Add(new SqlParameter("@psegCes", psegCes.EditValue));
                                cmd.Parameters.Add(new SqlParameter("@pTramo", pTramo.EditValue));
                                cmd.Parameters.Add(new SqlParameter("@pformPag", pformPag.EditValue));
                                cmd.Parameters.Add(new SqlParameter("@pBanco", pBanco.EditValue));
                                cmd.Parameters.Add(new SqlParameter("@pCuenta", pCuenta.Text));
                                cmd.Parameters.Add(new SqlParameter("@pVac", pVac.EditValue));
                                cmd.Parameters.Add(new SqlParameter("@pfeProg", pfeProg.EditValue));
                                cmd.Parameters.Add(new SqlParameter("@pAnProg", pAnProg.EditValue));
                                cmd.Parameters.Add(new SqlParameter("@pEstado", pEstado.EditValue));
                                cmd.Parameters.Add(new SqlParameter("@pPasa", pasaporte));
                                cmd.Parameters.Add(new SqlParameter("@pContrato", pContrato.Text));
                                cmd.Parameters.Add(new SqlParameter("@pAnoMes", pDate));
                                cmd.Parameters.Add(new SqlParameter("@pJub", jubilado));
                                cmd.Parameters.Add(new SqlParameter("@pPrev", caja));
                                cmd.Parameters.Add(new SqlParameter("@ptipoCuenta", pTipoCuenta.EditValue));
                                cmd.Parameters.Add(new SqlParameter("@pClase", pClase.EditValue));
                                cmd.Parameters.Add(new SqlParameter("@pCausal", pCausal.EditValue));
                                cmd.Parameters.Add(new SqlParameter("@psexo", pSexo.EditValue));
                                cmd.Parameters.Add(new SqlParameter("@pFun", pFun.Enabled ? pFun.Text : "0"));
                                cmd.Parameters.Add(new SqlParameter("@pPrivado", pPrivado.Checked));
                                cmd.Parameters.Add(new SqlParameter("@pMail", pMail.Text));
                                cmd.Parameters.Add(new SqlParameter("@pEsco", Convert.ToInt32(pEscolaridad.EditValue)));
                                cmd.Parameters.Add(new SqlParameter("@pNumEmer", pNumerEmer.Text));
                                cmd.Parameters.Add(new SqlParameter("@pNomEmer", pNomEmer.Text));
                                cmd.Parameters.Add(new SqlParameter("@pTalla", pTalla.Text));
                                cmd.Parameters.Add(new SqlParameter("@pCalzado", pCalzado.Text));
                                cmd.Parameters.Add(new SqlParameter("@pHorario", Convert.ToInt32(pHorario.EditValue)));
                                cmd.Parameters.Add(new SqlParameter("@pJornada", Convert.ToInt16(pJornada.EditValue)));
                                cmd.Parameters.Add(new SqlParameter("@pSindicato", Convert.ToInt32(pSindicato.EditValue)));
                                cmd.Parameters.Add(new SqlParameter("@pComuna", Convert.ToInt32(pComuna.EditValue)));
                                cmd.Parameters.Add(new SqlParameter("@pSuspension", Convert.ToInt32(pSuspension.EditValue)));

                                //***************************************************************************
                                //PARA LA IMAGEN PRIMERO DEBEMOS GENERAR EL BYTE DE LA IMAGEN               |
                                //LLAMAMOS A LA FUNCION QUE GENERA EL ARREGLO CORRESPONDIENTE A LA IMAGEN   |
                                //SE NECESITA LA RUTA DE LA IMAGEN                                          |
                                //***************************************************************************

                                //--> EL CAMPO pImage REPRESENTA LA RUTA FISICA DE LA IMAGEN (c:\asd\asdasd.jpg)
                                //@1 SI VIENE VACIA ES PORQUE LA IMAGEN NO SE CREO (NO SE CARGÓ IMAGEN)
                                //@2 SI NO VIENE VACIA ES PORQUE SE CREO LA IMAGEN (POR LO TANTO HAY RUTA FISICA)
                                if (pImage != "")
                                {
                                    byte[] img = Imagen.GuardarImagenBd(pImage);
                                    // byte[] img = GuardarImagenBd(pImage);//METODO QUE GENERA NUESTRA IMAGEN
                                    cmd.Parameters.Add(new SqlParameter("@pImage", img));//AGREGAMOS COMO PARAMETRO
                                }
                                else
                                    //GUARDAMOS COMO CAMPO NULO (NO HAY IMAGEN)
                                    cmd.Parameters.Add("@pImage", System.Data.SqlDbType.VarBinary).Value = DBNull.Value;

                                //AGREGAMOS A TRANSACCION
                                cmd.Transaction = tran;

                                //EJECUTAMOS CONSULTA
                                cmd.ExecuteNonQuery();

                                cmd.Parameters.Clear();
                            }

                            // @4->CARGAR ITEMS A TABLA ITEMTRABAJADOR DE ACUERDO A CLASE SELECCIONADA
                            using (cmd = new SqlCommand(sqlItemClase, cn))
                            {
                                //PARAMETROS
                                cmd.Parameters.Add(new SqlParameter("@pClase", Convert.ToInt32(txtClase.EditValue)));
                                cmd.Parameters.Add(new SqlParameter("@pPeriodo", pDate));
                                cmd.Parameters.Add(new SqlParameter("@pContrato", pContrato.Text));

                                //AGREGAMOS A TRANSACCION
                                cmd.Transaction = tran;

                                //EJECUTAMOS QUERY
                                cmd.ExecuteNonQuery();

                                //agregaItem = true;

                                cmd.Parameters.Clear();
                            }

                            //SI TODO SALIÓ BIEN HACEMOS COMMIT PARA GUARDAR LOS CAMBIOS
                            tran.Commit();
                            transaccionCorrecta = true;

                            op.SetButtonProperties(btnNuevo, 1);
                            op.Cancela = false;
                            cmd.Dispose();
                        }
                        catch (Exception ex)
                        {
                            //SI OCURRE ALGUN ERROR DESHACEMOS LOS CAMBIOS...
                            transaccionCorrecta = false;
                            tran.Rollback();
                        }
                    }
                }
             
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            //SI LA VARIABLE TRANSACCIONCORRECTA ES TRUE, SIGNIFICA QUE TODOS LOS PROCESOS SE HICIERON DE FORMA CORRECTA!
            if (transaccionCorrecta)
            {
                recortar = false;
                //cargar el ultimo registro
                Navega = new NavegaTrabajador(radioOrden.Properties.Items[radioOrden.SelectedIndex].Description, comboVista);
                Navega.SetPosition(pContrato.Text);
                fnNavDatos(pContrato.Text, pDate);

                //CAMBIAMOS NUMERO DE ITEM A LOS ITEM DE CLASE AGREGADOS (SI SE AGREGO ALGUNO)
                //if (agregaItem)
                  //  fnUpdateNumero(Convert.ToInt32(pClase.EditValue));

                //GUARDAR EVENTO EN LOG
                logRegistro log = new logRegistro(User.getUser(), "SE HA CREADO NUEVO TRABAJADOR CON CONTRATO N° " + pContrato.Text + " Y RUT " + pRut, "TRABAJADOR", "0", pNombre.Text + " " + pApePat.Text + " " + pApeMat.Text, "INGRESAR");
                log.Log();               

                XtraMessageBox.Show("Ingreso correcto de trabajador", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                XtraMessageBox.Show("Error al intentar guardar informacion trabajador", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        //VERSION 2 PARA MODIFICAR DATA TRABAJADOR (USANDO TRANSACCION SQL)
        private void ModificarTrabajadorTransaccion(string pRut, TextEdit pNombre, TextEdit pApePat, TextEdit pApeMat,
            TextEdit pDirec, LookUpEdit pCiudad, LookUpEdit pArea, LookUpEdit pcCosto,
            DateEdit pNac, LookUpEdit pNacion, LookUpEdit peCivil, TextEdit pFono, DateEdit pIngreso,
            DateEdit pSalida, LookUpEdit pCargo, LookUpEdit ptContr, LookUpEdit pregSal, LookUpEdit pReg,
            LookUpEdit pAfp, LookUpEdit pSalud, DateEdit psegCes, LookUpEdit pTramo, LookUpEdit pformPag,
            LookUpEdit pBanco, TextEdit pCuenta, DateEdit pVac, DateEdit pfeProg, SpinEdit pAnProg,
            LookUpEdit pEstado, string pImage, TextEdit pContrato, LookUpEdit pJub, LookUpEdit pPrev,
            TextEdit pPasa, LookUpEdit pTipoCuenta, LookUpEdit pclase, LookUpEdit pCausal, LookUpEdit pSexo, 
            LookUpEdit pcodSucursal, TextEdit pFun, CheckEdit pPrivado, TextEdit pMail, LookUpEdit pEscolaridad,
            TextEdit pNumEmer, TextEdit pNomEmer, TextEdit pTalla, TextEdit pCalzado, LookUpEdit pHorario,             
            LookUpEdit pJornada, LookUpEdit pSindicato, LookUpEdit pComuna, LookUpEdit pSuspension)
        {
            //SQL UPDATE
            string UPDATE = "UPDATE TRABAJADOR SET rut=@pRut, nombre=@pNombre, apepaterno=@pApePat, apematerno=@pApeMat, " +
                "direccion=@pDirec, ciudad=@pCiudad, sucursal=@pSucursal, area=@pArea, ccosto=@pcCosto, " +
                "fechanac=@pNac, nacion=@pNacion, ecivil=@peCivil, telefono=@pFono, ingreso=@pIngreso, salida=@pSalida," +
                "cargo=@pCargo, tipocontrato=@ptContr, regimenSalario=@pregSal, regimen=@pReg, afp=@pAfp, salud=@pSalud," +
                "fechaSegCes=@psegCes, tramo=@pTramo, formapago=@pformPag, banco=@pBanco, cuenta=@pCuenta, " +
                "fechavacacion=@pVac, fechaprogresivo=@pfeProg, anosprogresivo=@pAnProg, status=@pEstado, " +
                "rutafoto=@pImage, jubilado=@pJub, cajaPrevision=@pPrev," +
                " pasaporte=@pPasa, tipoCuenta=@pTipoCuenta, clase=@pclase, causal=@pCausal, sexo=@psexo, " +
                "fun=@pFun, privado=@pPrivado, mail=@pMail, esco=@pEsco, numemer=@pNumEmer, nomemer=@pNomEmer, " +
                " talla=@pTalla, calzado=@pCalzado, horario=@pHorario, jornada=@pJornada, sindicato=@pSindicato, " +
                "comuna=@pComuna, suslab=@pSuspension " +
                "WHERE contrato=@pContrato AND anomes=@periodo";

            //SQL PARA SABER SI LA CLASE QUE DESEA GUARDAR ES DISTINTA A LA CLASE QUE TIENE ASOCIADA EL TRABAJADOR
            string sqlClaseActual = "SELECT clase FROM trabajador WHERE anomes=@periodo AND contrato=@pcontrato";

            //SQL PARA ELIMINAR ITEM DE CLASE (EN CASO DE QUE SE CAMBIA LA CLASE PLANTILLA)
            string sqlEliminaClase = "DELETE itemTrabajador  FROM itemTrabajador " +
                        " INNER JOIN trabajador ON trabajador.contrato = itemTrabajador.contrato " +
                        " INNER JOIN clase on clase.codClase = trabajador.clase " +
                        " WHERE itemTrabajador.contrato = @pcontrato " +
                        " AND trabajador.clase = @pclase AND itemtrabajador.anomes = @periodo AND " +
                        " itemtrabajador.esclase = 1";

            //AGREGAR ITEM CLASE
            string sqlitemClase = "";
            //string sqlitemClase = "	declare @num INT " +
            //                      "select @num = MAX(numitem) + 1 FROM itemtrabajador WHERE contrato = @pcontrato AND anomes = @periodo " +
            //                     "INSERT INTO itemtrabajador(rut, contrato, anomes, coditem, formula, tipo, orden, numitem) " +
            //                     "SELECT rut, contrato, anomes, itemclase.item, itemclase.formula, tipo, orden, row_number() OVER(ORDER BY numitem) + ISNULL(@num, 0) FROM( " +
            //                            "SELECT rut, contrato, anomes, itemClase.item, itemClase.formula, tipo, orden, numitem " +
            //                                    "FROM trabajador inner JOIN clase on clase.codClase = trabajador.clase " +
            //                                    "INNER JOIN itemclase on itemClase.clase = clase.codClase " +
            //                                    "INNER JOIN item on item.coditem = itemClase.item " +
            //                                    "WHERE contrato = @pcontrato AND clase.codClase = @pclase AND trabajador.anomes = @periodo " +
            //                                    "EXCEPT " +
            //                                    "SELECT itemTrabajador.rut, itemTrabajador.contrato, trabajador.anomes, " +
            //                                    "itemTrabajador.coditem, itemTrabajador.formula, itemTrabajador.tipo, itemtrabajador.orden, itemclase.numitem " +
            //                                    "FROM itemTrabajador INNER JOIN trabajador on trabajador.contrato = itemTrabajador.contrato " +
            //                                    "INNER JOIN clase on clase.codClase = trabajador.clase " +
            //                                    "INNER JOIN itemClase on itemClase.clase = clase.codClase " +
            //                                    "WHERE itemTrabajador.contrato = @pcontrato " +
            //                                   "AND clase.codClase = @pclase AND itemtrabajador.anomes = @periodo " +
            //                       ")itemclase";

            sqlitemClase = Calculo.GetItemClaseSql();
                                

            //PARA GUARDAR EL VALOR DE PREVISION, SALUD, Y CAJA
            int afp = 0, salud = 0, caja = 0;

            //PARA GUARDAR EL CODIGO DE LA CLASE ACUTAL Y LA CLASE NUEVA (EN CASO DE QUE SE CAMBIE)
            int claseActual = 0;

            //PARA MANIPULAR SI ES JUBILADO Y EL PASAPORTE
            string pasaporte = "";
            Int16 jubilado = 0;

            bool TransaccionCorrecta = false, edadValida = false, agregaItem = false;

            if (pAfp.Enabled)
                afp = Convert.ToInt32(pAfp.EditValue);
            if (pSalud.Enabled)
                salud = Convert.ToInt32(pSalud.EditValue);
            if (pPrev.Enabled)
                caja = Convert.ToInt32(pPrev.EditValue);

            //*********************************************************************************
            // SI PASAPORTE VIENE EN BLANCO GUARDAMOS 0 QUE REPRESENTA QUE NO TIENE PASAPORTE |
            //*********************************************************************************

            if (pPasa.Text != "")
                pasaporte = pPasa.Text;

            //SI EL COMBOBOX JUBILADO ESTA HABILITADO GUARDAMOS SU CONTENIDO, CASO CONTRARIO GUARDAMOS CON '0'
            if (pJub.Enabled) jubilado = Convert.ToInt16(pJub.EditValue.ToString());

            edadValida = fnMayorEdad(pNac.DateTime);
            if (edadValida == false)
            {
                XtraMessageBox.Show("Por favor verifique la fecha de nacimiento", "Nacimiento", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //SI SE CAMBIA EL ESTADO A INACTIVO (CODE 0)
            if (txtEstado.EditValue.ToString() == "0")
            {
                DialogResult message = XtraMessageBox.Show("¿Estás seguro de cambiar el estado al contrato " + txtcontrato.Text + "?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (message == DialogResult.No)
                    return;
            }

            //TABLA HASH PARA LOG REGISTRO EMPLEADO
            Hashtable tablaEmpleado = new Hashtable();
            tablaEmpleado = PrecargaEmpleado(pContrato.Text, PeriodoEmpleado);

            SqlCommand cmd;
            SqlDataReader rd;
            SqlTransaction tran;
            SqlConnection cn;
            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        //INICIALIZAMOS LA TRANSACCION
                        tran = cn.BeginTransaction();
                        try
                        {
                            //@1 CONSULTAR POR LA CLASE ACTUAL QUE TIENE EL TRABAJADOR
                            using (cmd = new SqlCommand(sqlClaseActual, cn))
                            {
                                //PARAMETROS
                                cmd.Parameters.Add(new SqlParameter("@pcontrato", pContrato.Text));
                                cmd.Parameters.Add(new SqlParameter("@periodo", PeriodoEmpleado));

                                //AGREGAMOS A LA TRANSACCION
                                cmd.Transaction = tran;
                                rd = cmd.ExecuteReader();
                                if (rd.HasRows)
                                {
                                    while (rd.Read())
                                    {
                                        //GUARDAMOS CLASE ACTUAL
                                        claseActual = (int)rd["clase"];
                                    }
                                }

                                rd.Close();
                                cmd.Parameters.Clear();
                            }

                            //@2 COMPARAR CLASE ACTUAL CON LA QUE SE INTENTA GUARDAR, SI SON DISTINTAS
                            //ELIMINAMOS LA CLASE ANTERIOR Y AGREGAMOS LA NUEVA
                            if (claseActual != Convert.ToInt32(pclase.EditValue))
                            {
                                using (cmd = new SqlCommand(sqlEliminaClase, cn))
                                {
                                    //PARAMETROS
                                    cmd.Parameters.Add(new SqlParameter("@pcontrato", pContrato.Text));
                                    cmd.Parameters.Add(new SqlParameter("@pclase", claseActual));
                                    cmd.Parameters.Add(new SqlParameter("@periodo", PeriodoEmpleado));

                                    //AGREGAMOS A TRANSACCION
                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();

                                    cmd.Parameters.Clear();
                                }
                            }

                            //@3 MODIFICAMOS DATA DE TRABAJADOR
                            using (cmd = new SqlCommand(UPDATE, cn))
                            {
                                //PARAMETROS
                                cmd.Parameters.Add(new SqlParameter("@pRut", pRut));
                                cmd.Parameters.Add(new SqlParameter("@pNombre", pNombre.Text));
                                cmd.Parameters.Add(new SqlParameter("@pApePat", pApePat.Text));
                                cmd.Parameters.Add(new SqlParameter("@pApeMat", pApeMat.Text));
                                cmd.Parameters.Add(new SqlParameter("@pDirec", pDirec.Text));
                                cmd.Parameters.Add(new SqlParameter("@pCiudad", pCiudad.EditValue));
                                cmd.Parameters.Add(new SqlParameter("@pSucursal", pcodSucursal.EditValue));
                                cmd.Parameters.Add(new SqlParameter("@pArea", pArea.EditValue));
                                cmd.Parameters.Add(new SqlParameter("@pcCosto", pcCosto.EditValue));
                                cmd.Parameters.Add(new SqlParameter("@pNac", pNac.EditValue));
                                cmd.Parameters.Add(new SqlParameter("@pNacion", pNacion.EditValue));
                                cmd.Parameters.Add(new SqlParameter("@peCivil", peCivil.EditValue));
                                cmd.Parameters.Add(new SqlParameter("@pFono", pFono.Text));
                                cmd.Parameters.Add(new SqlParameter("@pIngreso", pIngreso.EditValue));
                                cmd.Parameters.Add(new SqlParameter("@pSalida", pSalida.EditValue));
                                cmd.Parameters.Add(new SqlParameter("@pCargo", pCargo.EditValue));
                                cmd.Parameters.Add(new SqlParameter("@ptContr", ptContr.EditValue));
                                cmd.Parameters.Add(new SqlParameter("@pregSal", pregSal.EditValue));
                                cmd.Parameters.Add(new SqlParameter("@pReg", pReg.EditValue));
                                cmd.Parameters.Add(new SqlParameter("@pAfp", afp));
                                cmd.Parameters.Add(new SqlParameter("@pSalud", salud));
                                cmd.Parameters.Add(new SqlParameter("@psegCes", psegCes.EditValue));
                                cmd.Parameters.Add(new SqlParameter("@pTramo", pTramo.EditValue));
                                cmd.Parameters.Add(new SqlParameter("@pformPag", pformPag.EditValue));
                                cmd.Parameters.Add(new SqlParameter("@pBanco", pBanco.EditValue));
                                cmd.Parameters.Add(new SqlParameter("@pCuenta", pCuenta.Text));
                                cmd.Parameters.Add(new SqlParameter("@pVac", pVac.EditValue));
                                cmd.Parameters.Add(new SqlParameter("@pfeProg", pfeProg.EditValue));
                                cmd.Parameters.Add(new SqlParameter("@pAnProg", pAnProg.Value));
                                cmd.Parameters.Add(new SqlParameter("@pEstado", pEstado.EditValue));
                                cmd.Parameters.Add(new SqlParameter("@pContrato", pContrato.Text));
                                cmd.Parameters.Add(new SqlParameter("@pJub", jubilado));
                                cmd.Parameters.Add(new SqlParameter("@pPrev", caja));
                                cmd.Parameters.Add(new SqlParameter("@pPasa", pasaporte));
                                cmd.Parameters.Add(new SqlParameter("@pTipoCuenta", pTipoCuenta.EditValue));
                                cmd.Parameters.Add(new SqlParameter("@pclase", pclase.EditValue));
                                cmd.Parameters.Add(new SqlParameter("@pCausal", pCausal.EditValue));
                                cmd.Parameters.Add(new SqlParameter("@periodo", PeriodoEmpleado));
                                cmd.Parameters.Add(new SqlParameter("@psexo", pSexo.EditValue));
                                cmd.Parameters.Add(new SqlParameter("@pFun", pFun.Enabled ? pFun.Text : "0"));
                                cmd.Parameters.Add(new SqlParameter("@pPrivado", pPrivado.Checked));
                                cmd.Parameters.Add(new SqlParameter("@pMail", pMail.Text));
                                cmd.Parameters.Add(new SqlParameter("@pEsco", Convert.ToInt32(pEscolaridad.EditValue)));
                                cmd.Parameters.Add(new SqlParameter("@pNumEmer", pNumEmer.Text));
                                cmd.Parameters.Add(new SqlParameter("@pNomEmer", pNomEmer.Text));
                                cmd.Parameters.Add(new SqlParameter("@pTalla", pTalla.Text));
                                cmd.Parameters.Add(new SqlParameter("@pCalzado", pCalzado.Text));
                                cmd.Parameters.Add(new SqlParameter("@pHorario", Convert.ToInt32(pHorario.EditValue)));
                                cmd.Parameters.Add(new SqlParameter("@pJornada", Convert.ToInt16(pJornada.EditValue)));
                                cmd.Parameters.Add(new SqlParameter("@pSindicato", Convert.ToInt32(pSindicato.EditValue)));
                                cmd.Parameters.Add(new SqlParameter("@pComuna", Convert.ToInt32(pComuna.EditValue)));
                                cmd.Parameters.Add(new SqlParameter("@pSuspension", Convert.ToInt32(pSuspension.EditValue)));

                                //***************************************************************************
                                //PARA LA IMAGEN PRIMERO DEBEMOS GENERAR EL BYTE DE LA IMAGEN               |
                                //LLAMAMOS A LA FUNCION QUE GENERA EL ARREGLO CORRESPONDIENTE A LA IMAGEN   |
                                //SE NECESITA LA RUTA DE LA IMAGEN                                          |
                                //***************************************************************************

                                //--> EL CAMPO pImage REPRESENTA LA RUTA FISICA DE LA IMAGEN (c:\asd\asdasd.jpg)
                                //@1 SI VIENE VACIA ES PORQUE LA IMAGEN NO SE CREO (NO SE CARGÓ IMAGEN)
                                //@2 SI NO VIENE VACIA ES PORQUE SE CREO LA IMAGEN (POR LO TANTO HAY RUTA FISICA)
                                if (pImage != "")
                                {
                                    byte[] img = Imagen.GuardarImagenBd(pImage);
                                    //byte[] img = GuardarImagenBd(pImage);//METODO QUE GENERA NUESTRA IMAGEN
                                    cmd.Parameters.Add(new SqlParameter("@pImage", img));//AGREGAMOS COMO PARAMETRO
                                }
                                else
                                    //GUARDAMOS COMO CAMPO NULO (NO HAY IMAGEN)
                                    cmd.Parameters.Add("@pImage", System.Data.SqlDbType.VarBinary).Value = DBNull.Value;

                                //AGREGAMOS A TRANSACCION
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();

                                cmd.Parameters.Clear();
                            }

                            //@4 AGREGAR NUEVO ITEMS DE CLASE
                            using (cmd = new SqlCommand(sqlitemClase, cn))
                            {
                                //PARAMETROS
                                cmd.Parameters.Add(new SqlParameter("@pContrato", pContrato.Text));
                                cmd.Parameters.Add(new SqlParameter("@pClase", Convert.ToInt32(pclase.EditValue)));
                                cmd.Parameters.Add(new SqlParameter("@pPeriodo", PeriodoEmpleado));

                                //AGREGAMOS A TRANSACCION
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                                agregaItem = true;
                            }

                            //SI LLEGAMOS A ESTE PUNTO ES PORQUE TODO SE REALIZÓ CORRECTAMENTE
                            //GUARDAMOS CAMBIOS
                            tran.Commit();
                            TransaccionCorrecta = true;
                            cmd.Dispose();                           
                        }
                        catch (Exception ex)
                        {
                            //SI SE PRODUCE ALGUN ERROR DESHACEMOS LOS CAMBIOS
                            TransaccionCorrecta = false;
                            tran.Rollback();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            //SI TRANSACCIONCORRECTA ES TRUE ES PORQUE TODO EL PROCESO SE REALIZO DE FORMA CORRECTA
            if (TransaccionCorrecta)
            {
                XtraMessageBox.Show("Registro Actualizado Correctamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);

                //if (pSuspension.Properties.DataSource != null && pSuspension.EditValue != null)
                //{
                //    if (Convert.ToInt32(pSuspension.EditValue) == 13 || Convert.ToInt32(pSuspension.EditValue) == 14)
                //        Calculo.UpdateItemSuspension(pContrato.Text, PeriodoEmpleado);
                //    else
                //        Calculo.UpdateItemSuspension(pContrato.Text, PeriodoEmpleado, true);
                //}

                //volvemos a resetear la variable recortar
                recortar = false;
                //volvemos a cargar el registro actualizado
                fnNavDatos(pContrato.Text, PeriodoEmpleado);

                if (cbPrivado.Checked)
                    Persona.FichasPrivadas(pContrato.Text);
                else
                    Persona.FichasPrivadas(pContrato.Text, true);

                //SI SE MODIFICO CORRECTAMENTE GUARDAMOS EVENTO EN LOG
                ComparaDatosEmpleado(tablaEmpleado, pRut, pasaporte, Convert.ToInt16(pEstado.EditValue), pNombre.Text, pApePat.Text,
                    pApeMat.Text, pDirec.Text, Convert.ToInt32(pCiudad.EditValue), Convert.ToInt32(pArea.EditValue), Convert.ToInt32(pcCosto.EditValue), (DateTime)pNac.EditValue,
                    Convert.ToInt32(pNacion.EditValue), Convert.ToInt32(peCivil.EditValue), pFono.Text, (DateTime)pIngreso.EditValue, (DateTime)pSalida.EditValue,
                    Convert.ToInt32(pCargo.EditValue), Convert.ToInt32(ptContr.EditValue), Convert.ToInt32(pregSal.EditValue), Convert.ToInt16(jubilado), Convert.ToInt32(pReg.EditValue),
                    Convert.ToInt32(pAfp.EditValue), Convert.ToInt32(pSalud.EditValue), Convert.ToInt32(pPrev.EditValue), (DateTime)psegCes.EditValue, Convert.ToInt32(pTramo.EditValue),
                    Convert.ToInt32(pformPag.EditValue), Convert.ToInt32(pBanco.EditValue), pCuenta.Text, (DateTime)pVac.EditValue, (DateTime)pfeProg.EditValue,
                    pAnProg.Value, Convert.ToInt32(pTipoCuenta.EditValue), Convert.ToInt32(pclase.EditValue), Convert.ToInt32(pCausal.EditValue), (Int16)pSexo.EditValue,
                    pContrato.Text, Convert.ToInt32(pcodSucursal.EditValue), pFun.Text, pMail.Text, Convert.ToInt32(pEscolaridad.EditValue), 
                    pNumEmer.Text, pNomEmer.Text, pTalla.Text, pCalzado.Text, Convert.ToInt32(pHorario.EditValue), Convert.ToInt16(pJornada.EditValue),
                    Convert.ToInt32(pSindicato.EditValue), Convert.ToInt32(pComuna.EditValue));


            }
            else
            {
                XtraMessageBox.Show("Error al intentar actualizar registro", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        /// <summary>
        /// Permite reactivar una ficha que está suspendida.
        /// Si el periodo de la ficha es menor al periodo observado, debemos crear una copia de la ficha con el nuevo periodo.
        /// Si el periodo es igual al periodo observado, solo cambiado de status.
        /// </summary>
        private void ReactivaTrabajador(string pContratoFicha, int pPeriodoFicha)
        {

            //Crea una copia de la ficha (SOLO SI EL PERIODO ES MENOR AL PERIODO OBSERVADO)
            string SqlCopia = $" SELECT * INTO ##copia1 FROM trabajador WHERE anomes = @pPeriodoFicha " +
                              "AND contrato = @pContratoFicha ";

            string ActualizaCopia = $"UPDATE ##copia1 SET anomes=@pPeriodoActual, status=1 " +
                               "WHERE contrato = @pContratoFicha AND anomes=@pPeriodoFicha";

            string sqlExiste = "SELECT count(*) FROM trabajador WHERE contrato=@pContratoFicha AND anomes=@pPeriodo";

            string InsertCopia = $"INSERT INTO trabajador SELECT * FROM ##copia1 ";

            string ClaseCopia = $"SELECT clase FROM ##copia1 WHERE contrato=@pContratoFicha AND anomes=@pPeriodoFicha ";

            //AGREGAR ITEM CLASE
            string sqlitemClase = "";
            //string sqlitemClase = "	declare @num INT " +
            //                      "select @num = MAX(numitem) + 1 FROM itemtrabajador WHERE contrato = @pcontrato AND anomes = @periodo " +
            //                     "INSERT INTO itemtrabajador(rut, contrato, anomes, coditem, formula, tipo, orden, numitem) " +
            //                     "SELECT rut, contrato, anomes, itemclase.item, itemclase.formula, tipo, orden, row_number() OVER(ORDER BY numitem) + ISNULL(@num, 0) FROM( " +
            //                            "SELECT rut, contrato, anomes, itemClase.item, itemClase.formula, tipo, orden, numitem " +
            //                                    "FROM trabajador inner JOIN clase on clase.codClase = trabajador.clase " +
            //                                    "INNER JOIN itemclase on itemClase.clase = clase.codClase " +
            //                                    "INNER JOIN item on item.coditem = itemClase.item " +
            //                                    "WHERE contrato = @pcontrato AND clase.codClase = @pclase AND trabajador.anomes = @periodo " +
            //                                    "EXCEPT " +
            //                                    "SELECT itemTrabajador.rut, itemTrabajador.contrato, trabajador.anomes, " +
            //                                    "itemTrabajador.coditem, itemTrabajador.formula, itemTrabajador.tipo, itemtrabajador.orden, itemclase.numitem " +
            //                                    "FROM itemTrabajador INNER JOIN trabajador on trabajador.contrato = itemTrabajador.contrato " +
            //                                    "INNER JOIN clase on clase.codClase = trabajador.clase " +
            //                                    "INNER JOIN itemClase on itemClase.clase = clase.codClase " +
            //                                    "WHERE itemTrabajador.contrato = @pcontrato " +
            //                                   "AND clase.codClase = @pclase AND itemtrabajador.anomes = @periodo " +
            //                       ")itemclase";
            sqlitemClase = Calculo.GetItemClaseSql();

            int codClase = 0;
            SqlConnection cn;
            SqlCommand cmd;
            SqlTransaction tr;
            bool correcto = false, existe = false;

            if (Calculo.PeriodoValido(pPeriodoFicha))
            {
                if (Persona.ExisteContrato(pContratoFicha, pPeriodoFicha))
                {
                    try
                    {
                        cn = fnSistema.OpenConnection();
                        if (cn != null)
                        {                            
                            using (cn)
                            {
                                tr = cn.BeginTransaction();
                                try
                                {
                                    //0- VERIFICAR SI CONTRAT YA EXISTE EN FICHA
                                    using (cmd = new SqlCommand(sqlExiste, cn))
                                    {
                                        cmd.Parameters.Add(new SqlParameter("@pContratoFicha", pContratoFicha));
                                        cmd.Parameters.Add(new SqlParameter("@pPeriodo", Calculo.PeriodoObservado));

                                        cmd.Transaction = tr;
                                        object data = cmd.ExecuteScalar();
                                        if (data != null)
                                        {
                                            if (Convert.ToInt32(data) > 0)
                                                existe = true;
                                        }

                                    }

                                    //EXISTE ? CONTRATO
                                    if (existe == false)
                                    {
                                        // 1- Crear copia de ficha
                                        using (cmd = new SqlCommand(SqlCopia, cn))
                                        {
                                            cmd.Parameters.Add(new SqlParameter("@pContratoFicha", pContratoFicha));
                                            cmd.Parameters.Add(new SqlParameter("@pPeriodoFicha", pPeriodoFicha));

                                            cmd.Transaction = tr;
                                            cmd.ExecuteNonQuery();
                                        }

                                        //2- Actualizar datos de copia
                                        using (cmd = new SqlCommand(ActualizaCopia, cn))
                                        {
                                            cmd.Parameters.Add(new SqlParameter("@pContratoFicha", pContratoFicha));
                                            cmd.Parameters.Add(new SqlParameter("@pPeriodoFicha", pPeriodoFicha));
                                            cmd.Parameters.Add(new SqlParameter("@pPeriodoActual", Calculo.PeriodoObservado));

                                            cmd.Transaction = tr;
                                            cmd.ExecuteNonQuery();
                                        }

                                        //3 - INSERTAR COPIA EN TABLA TRABAJADOR
                                        using (cmd = new SqlCommand(InsertCopia, cn))
                                        {
                                            cmd.Transaction = tr;
                                            cmd.ExecuteNonQuery();
                                        }

                                        //4 - PARA OBTENER EL CODIGO DE CLASE SELECCIONADA.
                                        using (cmd = new SqlCommand(ClaseCopia, cn))
                                        {
                                            cmd.Parameters.Add(new SqlParameter("@pContratoFicha", pContratoFicha));
                                            cmd.Parameters.Add(new SqlParameter("@pPeriodoFicha", Calculo.PeriodoObservado));

                                            cmd.Transaction = tr;

                                            object data = cmd.ExecuteScalar();
                                            if (data != null)
                                            {
                                                codClase = Convert.ToInt32(data);
                                            }

                                            //cmd.ExecuteNonQuery();
                                        }

                                        if (codClase != 0)
                                        {
                                            //5 - PARA INGRESAR LOS ITEMS DE CLASE
                                            using (cmd = new SqlCommand(sqlitemClase, cn))
                                            {
                                                //CONTRATO FICHA NUEVA
                                                cmd.Parameters.Add(new SqlParameter("@pContrato", pContratoFicha));
                                                //PERIODO FICHA NUEVA
                                                cmd.Parameters.Add(new SqlParameter("@pPeriodo", Calculo.PeriodoObservado));
                                                cmd.Parameters.Add(new SqlParameter("@pClase", codClase));

                                                cmd.Transaction = tr;
                                                cmd.ExecuteNonQuery();
                                            }


                                            tr.Commit();
                                            correcto = true;
                                        }

                                       
                                    }
                                }
                                catch (SqlException ex)
                                {
                                    tr.Rollback();
                                    correcto = false;
                                }
                            }
                        }                       
                        
                    }
                    catch (SqlException ex)
                    {
                        //ERROR...
                    }
                } 
            }

            if (correcto)
            {
                XtraMessageBox.Show("Ficha reactivada correctamente", "información", MessageBoxButtons.OK, MessageBoxIcon.Information);

                logRegistro log = new logRegistro(User.getUser(), "SE REACTIVA FICHA CONTRATO N° " + pContratoFicha, "TRABAJADOR", "", pContratoFicha, "INGRESAR");
                log.Log();

                //Cargar ficha
                Navega = new NavegaTrabajador(radioOrden.Properties.Items[radioOrden.SelectedIndex].Description, comboVista);
                Navega.SetPosition(pContratoFicha);
                fnNavDatos(pContratoFicha, Calculo.PeriodoObservado);
            }
            else
            {
                XtraMessageBox.Show("No se pudo reactivar ficha", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }
   
        //OBTIENE LISTADO CON TODOS LOS CONTRATOS Y SU PERIODO (PERIODO EN CURSO)
        private List<Empleado> ListadoContratos(int periodo)
        {
            string sql = "";
            List<Empleado> listado = new List<Empleado>();
            //CONSULTA ANTIGUA SIN FILTRO USUARIO
            //sql = "SELECT contrato, anomes, status FROM trabajador WHERE anomes=@periodo";

            //FILTRO USER LOGUEADO EN SISTEMA
            string filtro = User.GetUserFilter();
            string condicion = "";
            string Orden = GetcadenaOrden(ElementoOrden);
            //GUARDAR ORDEN SELECCIONADO            
          
            /*EL USUARIO ACTUAL NO TIENE NINGUN FILTRO*/
            if (filtro == "0")
            {
                sql = string.Format("SELECT contrato, anomes, status FROM trabajador WHERE anomes={0} {1}", periodo, Orden);
                if (VisualizaElemento("inactivos"))
                 sql = string.Format("SELECT contrato, anomes, status FROM trabajador WHERE anomes={0} AND status = 0 {1}", periodo, Orden); 
                if (VisualizaElemento("activos"))
                 sql = string.Format("SELECT contrato, anomes, status FROM trabajador WHERE anomes={0} AND status = 1  {1}", periodo, Orden); 
                if (VisualizaElemento("inactivos") && VisualizaElemento("activos"))
                 sql = string.Format("SELECT contrato, anomes, status FROM trabajador WHERE anomes={0} AND (status = 1 OR status = 0) {1}", periodo, Orden);
            }                
            else
            {
                condicion = Conjunto.GetCondicionFromCode(filtro);
                sql = string.Format("SELECT contrato, anomes, status FROM trabajador WHERE anomes={0} AND {1} AND status=1 {2}",
                    periodo, condicion, Orden);
                if (VisualizaElemento("inactivos"))
                    sql = string.Format("SELECT contrato, anomes, status FROM trabajador WHERE anomes={0} AND {1} AND status=0 {2}",
                    periodo, condicion, Orden);
                if (VisualizaElemento("activos"))
                    sql = string.Format("SELECT contrato, anomes, status FROM trabajador WHERE anomes={0} AND {1} AND status=1 {2}",
                    periodo, condicion, Orden);
                if (VisualizaElemento("activos") && VisualizaElemento("inactivos"))
                    sql = string.Format("SELECT contrato, anomes, status FROM trabajador WHERE anomes={0} AND {1} AND (status=1 OR status=0) {2}",
                    periodo, condicion, Orden);
            }

      
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS 
                        //cmd.Parameters.Add(new SqlParameter("@periodo", periodo));
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                
                                listado.Add(new Empleado() { contrato =(string)rd["contrato"], periodo = (int)rd["anomes"], estado=(Int16)rd["status"]});
                            }
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
            return listado;
        }

        //METODO PARA BOTON NAVEGACION NEXT
        //PARAMETRO ENTRADA
        /*
         * ELEMENTO DEL ARREGLO QUE CONTIENE TODOS LOS RUT 
         * DE ACUERDO A VALOR DE LA VARIABLE POSICION
         * @@OUTPUT
         * CAMPOS CARGADOS
         */
        private void fnNavDatos(string pContrato, int periodo)
        {
            //SQL BUSQUEDA
            string QUERY = "SELECT contrato, rut, nombre, apepaterno, apematerno, direccion, ciudad, sucursal, area, ccosto, fechanac," +
                "nacion, ecivil, telefono, ingreso, salida, cargo, tipocontrato, regimenSalario, regimen, afp, salud," +
                "fechaSegCes, tramo, formapago, banco, cuenta, fechavacacion, fechaprogresivo, anosprogresivo, " +
                "status, rutafoto, pasaporte, jubilado, cajaPrevision, anomes, clase, tipoCuenta, causal, sexo, fun, privado, " +
                "mail, esco, numemer, nomemer, talla, calzado, horario, jornada, sindicato, comuna, suslab " +
                " FROM trabajador" +
                " WHERE contrato=@pContrato AND anomes=@periodo";

            SqlCommand cmd;
            SqlDataReader rd;
            SqlConnection cn;

            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(QUERY, cn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@pContrato", pContrato));
                            cmd.Parameters.Add(new SqlParameter("@periodo", periodo));

                            //EJECUTAMOS CONSULTA
                            rd = cmd.ExecuteReader();
                            if (rd.HasRows)
                            {
                                while (rd.Read())
                                {
                                    //HABILITAR BOTONES ITEM DEL MES Y AUSENTISMO
                                    btnAusentismo.Enabled = true;
                                    btnItemMes.Enabled = true;
                                    btnFamiliares.Enabled = true;
                                    btnFirst.Enabled = true;
                                    btnPrev.Enabled = true;
                                    btnLast.Enabled = true;
                                    btnnext.Enabled = true;
                                    btnLiquidacion.Enabled = true;
                                    btnVacaciones.Enabled = true;
                                    btnHistorico.Enabled = true;
                                    btnFichaTrabajador.Enabled = true;
                                    btnFiniquito.Enabled = true;
                                    btnBuscar.Enabled = true;
                                    btnEliminar.Enabled = true;
                                    btnDocContrato.Enabled = true;
                                    btnAntiguedad.Enabled = true;
                                    btnObservaciones.Enabled = true;
                                    btnAdicional.Enabled = true;

                                    op.Cancela = false;
                                    op.SetButtonProperties(btnNuevo, 1);

                                    txtRut.ReadOnly = true;
                                    txtcontrato.ReadOnly = true;
                                    //CARGAMOS CAMPOS      
                                    txtcontrato.Text = (string)rd["contrato"];
                                    txtRut.Text = fnSistema.fFormatearRut2((string)rd["rut"]);
                                    txtNombre.Text = (string)rd["nombre"];
                                    txtapePaterno.Text = (string)rd["apepaterno"];
                                    txtApeMat.Text = (string)rd["apematerno"];
                                    txtDirec.Text = (string)rd["direccion"];
                                    txtCiudad.EditValue = (int)rd["ciudad"];
                                    txtSucursal.EditValue = (Convert.ToInt32(rd["sucursal"]));
                                    txtArea.EditValue = (int)rd["area"];
                                    txtccosto.EditValue = (int)rd["ccosto"];
                                    dtFecNac.EditValue = (DateTime)rd["fechanac"];
                                    txtNacion.EditValue = (int)rd["nacion"];
                                    txteCivil.EditValue = (int)rd["ecivil"];
                                    txtFono.Text = (string)rd["telefono"];
                                    dtingr.EditValue = (DateTime)rd["ingreso"];
                                    dtSal.EditValue = (DateTime)rd["salida"];
                                    txtCargo.EditValue = (int)rd["cargo"];
                                    txtTipoCont.EditValue = (int)rd["tipocontrato"];
                                    txtRegsal.EditValue = (int)rd["regimenSalario"];
                                    txtRegimen.EditValue = (int)rd["regimen"];
                                    txtAfp.EditValue = (int)rd["afp"];
                                    txtSalud.EditValue = (int)rd["salud"];
                                    dtSegCes.EditValue = (DateTime)rd["fechaSegCes"];
                                    txtTramo.EditValue = (int)rd["tramo"];
                                    txtfPago.EditValue = (int)rd["formapago"];
                                    txtBanc.EditValue = (int)rd["banco"];
                                    txtcuenta.EditValue = (string)rd["cuenta"];
                                    dtVac.EditValue = (DateTime)rd["fechavacacion"];
                                    dtProg.EditValue = (DateTime)rd["fechaprogresivo"];
                                    spProg.Value = (decimal)rd["anosprogresivo"];
                                    Int16 stat = (Int16)rd["status"];
                                    //FICHA ESTABA ACTIVA EN ESE PERIODO PERO ES UN PERIODO DISTINTO A EL ACTUAL
                                    if(periodo != Calculo.PeriodoObservado)
                                        txtEstado.EditValue = (Int16)0;  
                                    else
                                        txtEstado.EditValue = (Int16)rd["status"];

                                    txtPasaporte.Text = (string)rd["pasaporte"];
                                    txtjubilado.EditValue = (Int16)rd["jubilado"];
                                    lblperiodo.Text = "PERIODO:" + rd["anomes"];
                                    txtTipoCuenta.EditValue = (int)rd["tipoCuenta"];
                                    txtCajaPrevision.EditValue = (int)rd["cajaPrevision"];
                                    txtCausal.EditValue = Convert.ToInt32(rd["causal"]);
                                    txtSexo.EditValue = (Int16)rd["sexo"];
                                    txtFun.Text = (string)rd["fun"];
                                    cbPrivado.Checked = (bool)rd["privado"];
                                    txtEmail.Text = (string)rd["mail"];
                                    txtEstudios.EditValue = Convert.ToInt32(rd["esco"]);
                                    txtFonoEmer.Text = (string)rd["numemer"];
                                    txtNombreEmer.Text = (string)rd["nomemer"];
                                    txtTalla.Text = (string)rd["talla"];
                                    txtCalzado.Text = (string)rd["calzado"];
                                    txtHorario.EditValue = Convert.ToInt32(rd["horario"]);
                                    txtJornadaLaboral.EditValue = Convert.ToInt16(rd["jornada"]);
                                    txtSindicato.EditValue = Convert.ToInt32(rd["sindicato"]);
                                    txtComuna.EditValue = Convert.ToInt32(rd["comuna"]);
                                    txtSuspende.EditValue = Convert.ToInt32(rd["suslab"]);

                                    txtClase.EditValue = (int)rd["clase"];

                                    //NOMBRE COMPLETO
                                    NombreCompleto = (string)rd["nombre"] + " " + (string)rd["apepaterno"] + " " + (string)rd["apematerno"];

                                    //para la imagen debemos crear un file stream y luego cargar la imagen en pictureBox 

                                    //SI LA IMAGEN ES DISTINTA DE NULO TRATAMOS LA IMAGEN
                                    if ((rd["rutafoto"] as byte[]) != null)
                                    {
                                        //SI LA IMAGEN ES NULA NO LA CARGAMOS
                                        byte[] img = (byte[])rd["rutafoto"];

                                        MemoryStream ms = new MemoryStream(img);
                                        Image imagen = Image.FromStream(ms);

                                        //CARGAR IMAGEN EN PICTURE EDIT
                                        pictureEmpleado.Image = imagen;
                                        pictureEmpleado.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;

                                        //GUARDAR IMAGEN DE FORMA TEMPORAL EN COMPUTADOR
                                        imagen.Save(pathFile + "trabajador.jpg", ImageFormat.Jpeg);

                                        //guardamos la ruta de la imagen en la variable pathimage
                                        pathImage = pathFile + "trabajador.jpg";

                                        //guardamos imagen en imagen usuario
                                        imagenUsuario = imagen;
                                    }
                                    else
                                    {
                                        //SI EL NULO MOSTRAMOS LA IMAGEN POR DEFECTO
                                        pictureEmpleado.Image = Labour.Properties.Resources.SinFoto;
                                        pictureEmpleado.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;

                                        //DEJAR IMAGEN USUARIO NULA
                                        imagenUsuario = null;
                                    }

                                    //MOSTRAR NOMBRE COMPLETO Y EDAD DEBAJO DE LA FOTO DE PERFIL
                                    lblApellido.Text = (string)rd["apepaterno"] + " " + (string)rd["apematerno"];
                                    lblnombre.Text = (string)rd["nombre"];
                                    //EDAD
                                    DateTime nac = (DateTime)rd["fechanac"];
                                    //lbledad.Text = "EDAD: " + fnCalcularEdad(nac).ToString() + " AÑOS";
                                    lbledad.Text = "EDAD: " + Persona.GetEdadFromNac(nac).ToString() + " AÑOS";

                                    //GUARDAR EL PERIODO EN VARIABLE CORRESPONDIENTE
                                    PeriodoEmpleado = (int)rd["anomes"];

                                    StatusTrabajador = Convert.ToInt32(rd["status"]);

                                    //SI PERIODO EMPLEADO ES MEMOR AL PERIODO ABIERTO, CONSIDERAMOS QUE ES UNA FICHA HISTORICA
                                    if (PeriodoEmpleado < Calculo.PeriodoObservado)
                                    {
                                        FichaHistorica = true;
                                        StatusHistorico = Convert.ToInt16(rd["status"]);
                                        
                                    }
                                    else
                                    {
                                        FichaHistorica = false;
                                        StatusHistorico = -1;
                                    }                                        
                                }
                            }
                            else
                            {
                                XtraMessageBox.Show("No hay registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            cmd.Dispose();
                            rd.Close();                 
                        }
                    }
                }                
                else
                {
                    XtraMessageBox.Show("No hay conexion a Base de Datos", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }    

        //METODO PARA VERIFICAR LA EXISTENCIA DE UN PASAPORTE EN BD
        //PARAMETRO DE ENTRADA: STRING QUE REPRESENTA EL PASAPORTE
        //RETORNA TRUE EN CASO DE ENCONTRAR ALGUNA CONCIDENCIA EN BD
        private bool fnVerificarPasaporte(string pPas)
        {
            //QUERY SQL
            string query = "SELECT pasaporte FROM trabajador WHERE pasaporte=@pPas";
            SqlCommand cmd;
            SqlDataReader rd;
            bool coincidencia = false;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(query, fnSistema.sqlConn))
                    {
                        //DEFINICION DE PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pPas", pPas));
                        rd = cmd.ExecuteReader();
                        //SI NO ENCONTRO FILAS RETORNAMOS TRUE
                        if (!rd.HasRows)
                        {
                            coincidencia = true;
                        }
                        else
                        {
                            coincidencia = false;
                        }
                    }
                    //LIBERAR RECURSOS
                    cmd.Dispose();
                    rd.Close();
                    fnSistema.sqlConn.Close();
                }
                else
                {
                    XtraMessageBox.Show("No hay conexion con Base de datos", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
            return coincidencia;
        }

        //METODO PARA SABER SI SE HA MODIFICADO UN VALOR DE UNA CAJA CON EL VALOR EN BD
        //PRAMETRO DE ENTRADA : CONTRATO
        private bool fnComparar(string pContrato, int Periodo)
        {
            //SQL
            string sql = "SELECT * FROM trabajador WHERE contrato=@pContrato AND anomes=@periodo";

            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        cmd.Parameters.Add(new SqlParameter("@pContrato", pContrato));
                        cmd.Parameters.Add(new SqlParameter("@periodo", Periodo));
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //COMPARAMOS EL VALOR EN BD CON EL VALOR EN LA CAJA DE TEXTO
                            //return true EN CASO DE QUE SEAN DISTINTOS (MODIFICADO)
                            while (rd.Read())
                            {
                               
                                //ANTES DE COMPARA RUT DEBEMOS enmascarar y quitar punto y -
                                string rut = fnSistema.fnExtraerCaracteres(txtRut.Text);
                                rut = fnSistema.fEnmascaraRut(rut);
                              
                                if (rd["nombre"].ToString() != txtNombre.Text) return true;                                
                                if (rd["rut"].ToString() != rut) return true;                                
                                if (rd["pasaporte"].ToString() != txtPasaporte.Text) return true;                                
                                if (rd["apepaterno"].ToString() != txtapePaterno.Text) return true;                                
                                if (rd["apematerno"].ToString() != txtApeMat.Text) return true;                                
                                if (rd["direccion"].ToString() != txtDirec.Text) return true;
                                if (rd["ciudad"].ToString() != txtCiudad.EditValue.ToString()) return true;                                
                                if (Convert.ToInt32(rd["sucursal"]) != Convert.ToInt32(txtSucursal.EditValue)) return true;
                                if (rd["area"].ToString() != txtArea.EditValue.ToString()) return true;
                                if (rd["ccosto"].ToString() != txtccosto.EditValue.ToString()) return true;
                                if (rd["fechanac"].ToString() != dtFecNac.EditValue.ToString()) return true;                                
                                if (rd["nacion"].ToString() != txtNacion.EditValue.ToString()) return true;                                
                                if (rd["ecivil"].ToString() != txteCivil.EditValue.ToString()) return true;                                
                                if (rd["telefono"].ToString() != txtFono.Text) return true;
                                if (rd["ingreso"].ToString() != dtingr.EditValue.ToString()) return true;
                                if (rd["salida"].ToString() != dtSal.EditValue.ToString()) return true;                                
                                if (rd["cargo"].ToString() != txtCargo.EditValue.ToString()) return true;
                                if (rd["tipocontrato"].ToString() != txtTipoCont.EditValue.ToString()) return true;
                                if (rd["regimenSalario"].ToString() != txtRegsal.EditValue.ToString()) return true;
                                if (rd["jubilado"].ToString() != txtjubilado.EditValue.ToString()) return true;
                                if (rd["regimen"].ToString() != txtRegimen.EditValue.ToString()) return true;
                                if (rd["afp"].ToString() != txtAfp.EditValue.ToString()) return true;
                                if (rd["salud"].ToString() != txtSalud.EditValue.ToString()) return true;
                                if (rd["cajaPrevision"].ToString() != txtCajaPrevision.EditValue.ToString()) return true;
                                if (rd["fechaSegCes"].ToString() != dtSegCes.EditValue.ToString()) return true;
                                if (rd["tramo"].ToString() != txtTramo.EditValue.ToString()) return true;
                                if (rd["formapago"].ToString() != txtfPago.EditValue.ToString()) return true;
                                if (rd["banco"].ToString() != txtBanc.EditValue.ToString()) return true;
                                if (rd["cuenta"].ToString() != txtcuenta.EditValue.ToString()) return true;
                                if (rd["fechavacacion"].ToString() != dtVac.EditValue.ToString()) return true;
                                if (rd["fechaprogresivo"].ToString() != dtProg.EditValue.ToString()) return true;
                                if ((decimal)rd["anosprogresivo"] != spProg.Value) return true;
                                if (rd["status"].ToString() != txtEstado.EditValue.ToString())
                                {
                                    if(Periodo == Calculo.PeriodoObservado)
                                        return true;
                                }
                                    
                                if (rd["tipoCuenta"].ToString() != txtTipoCuenta.EditValue.ToString()) return true;
                                if (rd["clase"].ToString() != txtClase.EditValue.ToString()) return true;
                                if (rd["sexo"].ToString() != txtSexo.EditValue.ToString()) return true;                           
                                if (rd["fun"].ToString() != txtFun.Text) return true;
                            }
                        }
                        else
                        {
                            XtraMessageBox.Show("No hay registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    //LIBERAR RECURSOS
                    cmd.Dispose();
                    rd.Close();
                    fnSistema.sqlConn.Close();
                }
                else
                {
                    XtraMessageBox.Show("No hay conexion con Base de Datos");
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }


            return false;
        }

        //METODO PARA SABER SI LOS CAMPOS NO ESTAN VACIOS
        private bool fnCamposVacios()
        {
            if (txtRut.Text != "") return true;
            if (txtNombre.Text != "") return true;
            if (txtapePaterno.Text != "") return true;
            if (txtApeMat.Text != "") return true;
            if (txtArea.EditValue.ToString() != "") return true;
            if (dtFecNac.EditValue.ToString() != "") return true;
            if (txteCivil.EditValue.ToString() != "") return true;
            if (txtFono.Text != "") return true;
            if (txtDirec.Text != "") return true;
            if (txtCiudad.EditValue.ToString() != "") return true;
            if (txtNacion.EditValue.ToString() != "") return true;
            if (txtEstado.EditValue.ToString() != "") return true;
            if (txtAfp.EditValue.ToString() != "" && txtAfp.Enabled) return true;
            if (txtSalud.EditValue.ToString() != "" && txtSalud.Enabled) return true;
            if (dtSegCes.EditValue.ToString() != "") return true;
            if (txtTramo.EditValue.ToString() != "") return true;
            if (txtCajaPrevision.EditValue.ToString() != "" && txtCajaPrevision.Enabled) return true;
            if (txtjubilado.EditValue.ToString() != "" && txtjubilado.Enabled) return true;            
            if (txtcontrato.Text != "") return true;
            if (txtTipoCont.EditValue.ToString() != "") return true;
            if (txtArea.EditValue.ToString() != "") return true;
            if (txtCargo.EditValue.ToString() != "") return true;
            if (txtRegimen.EditValue.ToString() != "") return true;
            if (txtRegsal.EditValue.ToString() != "") return true;
            if (txtccosto.EditValue.ToString() != "") return true;
            if (dtProg.EditValue.ToString() != "") return true;
            if (dtVac.EditValue.ToString() != "") return true;
            if (spProg.Value.ToString() != "") return true;
            if (dtingr.EditValue.ToString() != "") return true;
            if (dtSal.EditValue.ToString() != "" && dtSal.Enabled) return true;
            if (txtfPago.EditValue.ToString() != "") return true;
            if (txtBanc.EditValue.ToString() != "") return true;
            if (txtcuenta.Text != "") return true;
            if (txtTipoCuenta.EditValue.ToString() != "") return true;
            if (txtSexo.EditValue.ToString() != "") return true;            
            return false;

        }

        //METODO PARA CARGAR COMBO tramo
        //TIENE CUATRO VALORES 1, 2, 3, 4
        private void fnComboTramo(LookUpEdit pCombo)
        {
            //instanciamos a la clase combo
            List<datoCombobox> lista = new List<datoCombobox>();
            int i = 1;
            while (i <= 4)
            {
                lista.Add(new datoCombobox() { descInfo = "" + i, KeyInfo = i });
                i++;
            }

            //PROPIEDADES COMBOBOX
            pCombo.Properties.DataSource = lista.ToList();
            pCombo.Properties.ValueMember = "KeyInfo";
            pCombo.Properties.DisplayMember = "descInfo";

            pCombo.Properties.PopulateColumns();

            //ocultamos la columan key
            pCombo.Properties.Columns[0].Visible = false;

            pCombo.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
            pCombo.Properties.AutoSearchColumnIndex = 1;
            pCombo.Properties.ShowHeader = false;
        }

        //METODO PARA CARGAR COMBO JUBILADO
        //TIENE LOS SIGUIENTES VALORES:
        // 0 --> NO
        /* 1 --> SI, NO COTIZA
         * 2 --> SI, COTIZA 
         */
        private void fnComboJubilado(LookUpEdit pCombo)
        {
            List<PruebaCombo> lista = new List<PruebaCombo>();
            Int16 i = 0;
            while (i <= 2)
            {
                //agregamos objetos a la lista
                if (i == 0) lista.Add(new PruebaCombo() { key = i, desc = "No" }); //ACTIVO (NO PENSIONADO)
                if (i == 1) lista.Add(new PruebaCombo() { key = i, desc = "Si, No Cotiza" }); // (PENSIONADO Y NO COTIZA)
                if (i == 2) lista.Add(new PruebaCombo() { key = i, desc = "Si, Cotiza" }); // (PENSIONADO Y COTIZA)
                //if (i == 3) lista.Add(new PruebaCombo() { key = i, desc = "Activo mayor 65"});
                i++;
            }

            //PROPIEDADES COMBOBOX
            pCombo.Properties.DataSource = lista.ToList();
            pCombo.Properties.ValueMember = "key";
            pCombo.Properties.DisplayMember = "desc";

            pCombo.Properties.PopulateColumns();

            //ocultamos la columna key
            pCombo.Properties.Columns[0].Visible = false;

            pCombo.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
            pCombo.Properties.AutoSearchColumnIndex = 1;
            pCombo.Properties.ShowHeader = false;
        }

        //METODO PARA CARGAR COMBO REGIMEN SALARIO
        //TIENE DOS VALORES 
        /*
         * 0 --> VARIABLE
         * 1 --> FIJO
         */
        private void fnComboRegimenSalario(LookUpEdit pCombo)
        {
            List<datoCombobox> lista = new List<datoCombobox>();
            int i = 0;
            while (i <= 1)
            {
                //agregamos valores a la lista
                if (i == 0) lista.Add(new datoCombobox() { KeyInfo = i, descInfo = "Variable" });
                if (i == 1) lista.Add(new datoCombobox() { KeyInfo = i, descInfo = "Fijo" });
                i++;
            }
            //PROPIEDADES COMBOBOX
            pCombo.Properties.DataSource = lista.ToList();
            pCombo.Properties.ValueMember = "KeyInfo";
            pCombo.Properties.DisplayMember = "descInfo";

            pCombo.Properties.PopulateColumns();

            //ocultamos la columan key
            pCombo.Properties.Columns[0].Visible = false;

            pCombo.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
            pCombo.Properties.AutoSearchColumnIndex = 1;
            pCombo.Properties.ShowHeader = false;
        }

        //METODO PARA CARGAR COMBO TIPO CONTRATO
        //TIENE LOS SIGUIENTES VALORES
        /*
         * 0 --> INDEFINIDO
         * 1 --> PLAZO FIJO
         * 2 --> OBRA O FAENA
         */
        private void fnComboTipoContrato(LookUpEdit pCombo)
        {
            List<datoCombobox> lista = new List<datoCombobox>();
            int i = 0;
            while (i <= 2)
            {
                if (i == 0) lista.Add(new datoCombobox() { KeyInfo = i, descInfo = "INDEFINIDO" });
                if (i == 1) lista.Add(new datoCombobox() { KeyInfo = i, descInfo = "PLAZO FIJO" });
                if (i == 2) lista.Add(new datoCombobox() { KeyInfo = i, descInfo = "OBRA O FAENA" });
                //if (i == 3) lista.Add(new datoCombobox() { KeyInfo = i, descInfo = "SUSPENSION LABORAL" });
                i++;
            }
            //PROPIEDADES COMBOBOX
            pCombo.Properties.DataSource = lista.ToList();
            pCombo.Properties.ValueMember = "KeyInfo";
            pCombo.Properties.DisplayMember = "descInfo";

            pCombo.Properties.PopulateColumns();

            //ocultamos la columan key
            pCombo.Properties.Columns[0].Visible = false;

            pCombo.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
            pCombo.Properties.AutoSearchColumnIndex = 1;
            pCombo.Properties.ShowHeader = false;

        }

   

        //PARA CARGA EL COMBO BOX PARA EL TIPO DE CONTRATO 
        //... 
        //METODO PARA CARGAR COMBO STATUS
        //TIENE LOS SIGUIENTES VALORES
        /*
         * 1 --> ACTIVO
         * 0 --> NO ACTIVO
         */
        private void fnComboStatus(LookUpEdit pCombo)
        {
            //List<strucStatus> struc = new List<strucStatus>();         
            List<PruebaCombo> lista1 = new List<PruebaCombo>();


            lista1.Add(new PruebaCombo() { key = 1, desc = "Activo" });
            lista1.Add(new PruebaCombo() { key = 0, desc = "No Activo" });

            //PROPIEDADES COMBOBOX
            pCombo.Properties.DataSource = lista1.ToList(); 
            pCombo.Properties.ValueMember = "key";
            pCombo.Properties.DisplayMember = "desc";

            pCombo.Properties.PopulateColumns();

            //ocultamos la columan key
            pCombo.Properties.Columns[0].Visible = false;

            pCombo.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
            pCombo.Properties.AutoSearchColumnIndex = 1;
            pCombo.Properties.ShowHeader = false;
        }
        //METODO PARA CARGAR COMBO TIPO CUENTA
        //TIPOS:
        /*
         * 1 --> CUENTA CORRIENTE
         * 2 --> CUENTA AHORRO
         * 3 --> CUENTA VISTA
         * 4 --> CHEQUERA ELECTRONICA
         */
        private void fnComboTipoCuenta(LookUpEdit pCombo, int? bank = 0)
        {
            List<PruebaCombo> lista = new List<PruebaCombo>();

            lista.Add(new PruebaCombo() { key = 1, desc = "CUENTA CORRIENTE" });
            lista.Add(new PruebaCombo() { key = 2, desc = "CUENTA AHORRO" });
            lista.Add(new PruebaCombo() { key = 3, desc = "CUENTA VISTA" });
            lista.Add(new PruebaCombo() { key = 4, desc = "CHEQUERA ELECTRONICA" });

            //MOSTRAR CUENTA RUT SOLO PARA BANCO ESTADO
            if (bank == 1)
                lista.Add(new PruebaCombo() { key = 5, desc = "CUENTA RUT" });

            //PROPIEDADES COMBOBOX
            pCombo.Properties.DataSource = lista.ToList();
            pCombo.Properties.ValueMember = "key";
            pCombo.Properties.DisplayMember = "desc";

            pCombo.Properties.PopulateColumns();

            //ocultamos la columan key
            pCombo.Properties.Columns[0].Visible = false;

            pCombo.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
            pCombo.Properties.AutoSearchColumnIndex = 1;
            pCombo.Properties.ShowHeader = false;
        }

        //COMBO SEXO
        //0--> M
        //1--> F
        private void fnComboSexo(LookUpEdit pCombo)
        {
            List<PruebaCombo> listado = new List<PruebaCombo>();

            listado.Add(new PruebaCombo() { key = 0, desc = "M"});
            listado.Add(new PruebaCombo() { key = 1, desc = "F" });

            //PROPIEDADES COMBOBOX
            pCombo.Properties.DataSource = listado.ToList();
            pCombo.Properties.ValueMember = "key";
            pCombo.Properties.DisplayMember = "desc";

            pCombo.Properties.PopulateColumns();

            //ocultamos la columan key
            pCombo.Properties.Columns[0].Visible = false;

            pCombo.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
            pCombo.Properties.AutoSearchColumnIndex = 1;
            pCombo.Properties.ShowHeader = false;
        }

        //MANEJAR TECLA TAB CUANDO SE PRESIONE EN CAMPO RUT
        protected override bool ProcessDialogKey(Keys keyData)
        {
            string cadena = "";
            bool rutValida;
            if (keyData == Keys.Tab)
            {
                //PREGUNTAMOS SI EL CAMPO RUT TIENE EL FOCO
                if (txtRut.ContainsFocus)
                {
                    ValidaRut();                  
                }
                //PREGUNTAMOS SI EL CAMPO CONTRATO TIENE EL FOCO
                if (txtcontrato.ContainsFocus)
                {
                    //SI ESTA VACIO NO ES VALIDO
                    ValidaContrato();
                }

                if (dtFecNac.ContainsFocus)
                {
                    ValidaNacimiento();
                }
            }
            return base.ProcessDialogKey(keyData);
        }

        //FUNCION PARA LIMPIAR CAMPOS
        private void fnLimpiarCampos()
        {           
            txtcontrato.Text = "";
            txtcontrato.ReadOnly = false;
            txtRut.Text = "";
            txtRut.Focus();
            txtPasaporte.Text = "0";
            txtRut.ReadOnly = false;
            txtNombre.Text = "";
            txtApeMat.Text = "";
            txtapePaterno.Text = "";
            txtFono.Text = "0";
            txtDirec.Text = "";
            txtcuenta.Text = "0";
            txtCausal.ItemIndex = 0;
            txtEmail.Text = "";
            txtTalla.Text = "";
            txtCalzado.Text = "";
            txtNombreEmer.Text = "";
            txtFonoEmer.Text = "";
            
            //colocar la fecha actual por defecto en los campos fecha
            
            dtFecNac.EditValue = fnSistema.PrimerDiaMes(Calculo.PeriodoObservado);
            dtSegCes.EditValue = fnSistema.PrimerDiaMes(Calculo.PeriodoObservado);
            dtVac.EditValue = fnSistema.PrimerDiaMes(Calculo.PeriodoObservado);
            //dtSal.EditValue = DateTime.Now.Date;
            dtingr.EditValue = fnSistema.PrimerDiaMes(Calculo.PeriodoObservado);
            dtProg.EditValue = Convert.ToDateTime("01-01-3000");

            //Colocar por defecto el primer elemento del combobox
            txteCivil.ItemIndex = 3;
            txtNacion.ItemIndex = 0;
            txtCiudad.ItemIndex = 0;
            txtAfp.ItemIndex = 0;
            txtSalud.ItemIndex = 0;
            txtTramo.ItemIndex = 3;
            //txtEmpresa.ItemIndex = 0;
            txtArea.ItemIndex = 0;
            txtCargo.ItemIndex = 0;
            txtTipoCont.ItemIndex = 0;
            txtRegimen.ItemIndex = 0;
            txtRegsal.ItemIndex = 0;
            txtfPago.ItemIndex = 0;
            txtBanc.ItemIndex = 0;
            txtccosto.ItemIndex = 0;
            txtjubilado.ItemIndex = 0;
            txtCajaPrevision.ItemIndex = 0;
            txtTipoCuenta.ItemIndex = 0;
            txtSexo.ItemIndex = 0;
            txtEstado.ItemIndex = 0;
            txtClase.ItemIndex = 0;
            txtSucursal.ItemIndex = 0;
            //resetear spineditor
            spProg.EditValue = 0.0;
            txtJornadaLaboral.ItemIndex = 0;
            txtHorario.ItemIndex = 0;
            txtSindicato.ItemIndex = 0;

            //dejar imagen por defecto en pictureedit
            pictureEmpleado.Image = Labour.Properties.Resources.SinFoto;
            //evitar que se pueda editar imagen
            imagenUsuario = null;
            lblerror.Visible = false;       
            lblApellido.Text = "";
            lblnombre.Text = "";
            lbledad.Text = "";
            lblperiodo.Text = "PERIODO:" + Calculo.PeriodoObservado;

            //DESHABILITAR BOTON ITEM DEL MES Y BOTON AUSENTISMO         
            btnAusentismo.Enabled = false;
            btnItemMes.Enabled = false;
            btnFamiliares.Enabled = false;
            btnLiquidacion.Enabled = false;
            btnVacaciones.Enabled = false;
            btnHistorico.Enabled = false;
            btnFichaTrabajador.Enabled = false;
            btnFiniquito.Enabled = false;
            btnDocContrato.Enabled = false;
            btnAntiguedad.Enabled = false;
            btnObservaciones.Enabled = false;
            btnAdicional.Enabled = false;

            FichaHistorica = false;
        }

        //FUNCION PARA SETEAR PROPIEDADES POR DEFAULT
        private void fnDefaultProperties()
        {
            lblperiodo.Text = "PERIODO:" + Calculo.PeriodoObservado;
            //CARACTERES MAXIMOS PARA CAMPOS            
            txtRut.Properties.MaxLength = 12;
            txtNombre.Properties.MaxLength = 50;
            txtApeMat.Properties.MaxLength = 50;
            txtapePaterno.Properties.MaxLength = 50;
            txtDirec.Properties.MaxLength = 100;
            txtFono.Properties.MaxLength = 50;
            txtcuenta.Properties.MaxLength = 20;
            txtPasaporte.Properties.MaxLength = 9;
            txtcontrato.Properties.MaxLength = 15;
            txtTipoCuenta.ItemIndex = 0;
            txtCausal.ItemIndex = 0;
            txtSexo.ItemIndex = 0;

            txtNacion.ItemIndex = 0;
            txteCivil.ItemIndex = 0;
            txtNacion.ItemIndex = 0;
            txtCiudad.ItemIndex = 0;
            txtTipoCont.ItemIndex = 0;
            txtArea.ItemIndex = 0;
            txtCargo.ItemIndex = 0;
            txtccosto.ItemIndex = 0;
            txtRegimen.ItemIndex = 0;
            txtRegsal.ItemIndex = 0;
            txtClase.ItemIndex = 0;
            txtSalud.ItemIndex = 0;
            txtAfp.ItemIndex = 0;
            txtjubilado.ItemIndex = 0;
            txtTramo.ItemIndex = 0;
            txtBanc.ItemIndex = 0;
            txtTipoCuenta.ItemIndex = 0;
            txtfPago.ItemIndex = 0;
            txtEstado.ItemIndex = 0;
            txtSucursal.ItemIndex = 0;
            
            //txtClase.ItemIndex = 0;

            txteCivil.Properties.PopupSizeable = false;
            txtEstado.Properties.PopupSizeable = false;
            txtNacion.Properties.PopupSizeable = false;
            txtCiudad.Properties.PopupSizeable = false;
            txtTipoCont.Properties.PopupSizeable = false;
            txtArea.Properties.PopupSizeable = false;
            txtCargo.Properties.PopupSizeable = false;
            txtccosto.Properties.PopupSizeable = false;
            txtRegsal.Properties.PopupSizeable = false;
            txtRegimen.Properties.PopupSizeable = false;
            txtSalud.Properties.PopupSizeable = false;
            txtAfp.Properties.PopupSizeable = false;
            txtCajaPrevision.Properties.PopupSizeable = false;
            txtjubilado.Properties.PopupSizeable = false;
            txtTramo.Properties.PopupSizeable = false;
            txtfPago.Properties.PopupSizeable = false;
            txtBanc.Properties.PopupSizeable = false;
            txtTipoCuenta.Properties.PopupSizeable = false;
            txtCausal.Properties.PopupSizeable = false;
            txtSexo.Properties.PopupSizeable = false;
            txtClase.Properties.PopupSizeable = false;
            txtSucursal.Properties.PopupSizeable = false;
            
            //--------------------------
            //VALOR MAXIMO Y CANTIDAD DE NUMEROS PARA EL SPINEDIT
            spProg.Properties.MaxValue = 300;
            spProg.Properties.MaxLength = 3;

            //Dejar por defecto en los date edit la fecha actual
            //dtFecNac.Properties.MinValue = DateTime.Parse("01-01-1900");
            dtFecNac.EditValue = DateTime.Now.Date;
            dtingr.EditValue = DateTime.Now.Date;
            dtProg.EditValue = DateTime.Now.Date;
            dtSal.EditValue = DateTime.Now.Date;
            dtSegCes.EditValue = DateTime.Now.Date;
            dtVac.EditValue = DateTime.Now.Date;

            //NO permitir editar los combos de fecha
            //dtFecNac.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            //dtingr.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            //dtProg.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            //dtSal.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            //dtSegCes.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            //dtVac.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;

            //QUITAR EL BOTON VACIAR DE LOS DATE EDIT
            dtFecNac.Properties.ShowClear = false;
            dtingr.Properties.ShowClear = false;
            dtProg.Properties.ShowClear = false;
            dtSal.Properties.ShowClear = false;
            dtSegCes.Properties.ShowClear = false;
            dtVac.Properties.ShowClear = false;

            //EVITAR FOCO EN LOS SIGUIENTES CONTROLES
            btnNuevo.AllowFocus = false;
            btnGrabar.AllowFocus = false;
            btnBuscar.AllowFocus = false;
            btnFirst.AllowFocus = false;
            btnLast.AllowFocus = false;
            btnnext.AllowFocus = false;
            btnPrev.AllowFocus = false;
            btnEliminar.AllowFocus = false;
            btnAusentismo.AllowFocus = false;
            btnItemMes.AllowFocus = false;
            btnHistorico.AllowFocus = false;
            btnFichaTrabajador.AllowFocus = false;            
            btnItemMes.TabStop = false;
            btnVacaciones.TabStop = false;
            btnHistorico.TabStop = false;
            btnFichaTrabajador.TabStop = false;
            btnObservaciones.TabStop = false;
            radioOrden.SelectedIndex = fnSistema.OpcionBusqueda;

            //DEJAR CON NEGRITA CAMPO RUT Y CAMPO CONTRATO
            txtRut.Properties.Appearance.FontStyleDelta = FontStyle.Bold;
            txtcontrato.Properties.Appearance.FontStyleDelta = FontStyle.Bold;

            //OBTENER LA CANTIDAD DE CONTRATOS            
            //List<Empleado> contratos = new List<Empleado>();
            //contratos = ListadoContratos(Calculo.PeriodoObservado);           

            //SI LA CANTIDAD DE ELEMENTOS ES CERO DESHABILITAMOS LOS BOTONES NAVEGACIONALES
            if (Navega.Universo == 0)
            {
                btnnext.Enabled = false;
                btnLast.Enabled = false;
                btnPrev.Enabled = false;
                btnFirst.Enabled = false;
                btnItemMes.Enabled = false;
                btnAusentismo.Enabled = false;
                btnFamiliares.Enabled = false;
                btnLiquidacion.Enabled = false;
                btnVacaciones.Enabled = false;
                btnHistorico.Enabled = false;
                btnFichaTrabajador.Enabled = false;
                btnBuscar.Enabled = false;
                btnEliminar.Enabled = false;
                btnFiniquito.Enabled = false;
                btnDocContrato.Enabled = false;
                btnObservaciones.Enabled = false;
                btnAntiguedad.Enabled = false;
            }
            else
            {
                btnnext.Enabled = true;
                btnLast.Enabled = true;
                btnPrev.Enabled = true;
                btnFirst.Enabled = true;
                btnItemMes.Enabled = true;
                btnAusentismo.Enabled = true;
                btnFamiliares.Enabled = true;
                btnLiquidacion.Enabled = true;
                btnVacaciones.Enabled = true;
                btnHistorico.Enabled = true;
                btnFichaTrabajador.Enabled = true;
                btnBuscar.Enabled = true;
                btnEliminar.Enabled = true;
                btnFiniquito.Enabled = true;
                btnDocContrato.Enabled = true;
                btnObservaciones.Enabled = true;
                btnAntiguedad.Enabled = true;                    
            }
        }   

        //VALIDAR CAMPOS EN BLANCO ANTES DE GUARDAR
        private bool fnCamposBlanco()
        {
            if (txtNombre.Text == "") { XtraMessageBox.Show("Debes ingresar Nombre", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return true; }
            if (txtapePaterno.Text == "") { XtraMessageBox.Show("Debes ingresar Apellido Paterno", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return true; }
            if (txtApeMat.Text == "") { XtraMessageBox.Show("Debes ingresar Apellido Materno", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return true; }
            if (dtFecNac.EditValue.ToString() == "") { XtraMessageBox.Show("Debes ingresar Fecha Nacimiento", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return true; }
            if (txteCivil.EditValue.ToString() == "") { XtraMessageBox.Show("Debes seleccionar Estado Civil ", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return true; }
            if (txtCiudad.EditValue.ToString() == "") { XtraMessageBox.Show("Debes seleccionar una ciudad", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return true; }
            if (txtNacion.EditValue.ToString() == "") { XtraMessageBox.Show("Debes seleccionar una nacion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return true; }
            if (txtEstado.EditValue.ToString() == "") { XtraMessageBox.Show("Debes seleccionar un estado", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return true; }
            //if (txtAfp.EditValue.ToString() == "") { XtraMessageBox.Show("Debes seleccionar una prevision", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return true; }
            //if (txtSalud.EditValue.ToString() == "") { XtraMessageBox.Show("Debes seleccionar salud", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return true; }
            if (dtSegCes.EditValue.ToString() == "") { XtraMessageBox.Show("Debes ingresar seguro de Cesantia", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return true; }
            if (txtTramo.EditValue.ToString() == "") { XtraMessageBox.Show("Debes seleccionar un tramo", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return true; }
            //if (txtCajaPrevision.EditValue.ToString() == "") { XtraMessageBox.Show("Debes seleccionar caja de prevision", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return true; }
            if (txtjubilado.EditValue.ToString() == "") { XtraMessageBox.Show("Debes completar campo jubilacion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return true; }
            //if (txtEmpresa.EditValue.ToString() == "") { XtraMessageBox.Show("Debes seleccionar", "", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (txtTipoCont.EditValue.ToString() == "") { XtraMessageBox.Show("Debes seleccionar tipo contrato", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return true; }
            if (txtArea.EditValue.ToString() == "") { XtraMessageBox.Show("Debes seleccionar un area", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return true; }
            if (txtCargo.EditValue.ToString() == "") { XtraMessageBox.Show("Debes seleccionar un cargo", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return true; }
            if (txtRegimen.EditValue.ToString() == "") { XtraMessageBox.Show("Debes seleccionar un regimen", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return true; }
            if (txtRegsal.EditValue.ToString() == "") { XtraMessageBox.Show("debes seleccionar regimen salarial", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return true; }
            if (txtccosto.EditValue.ToString() == "") { XtraMessageBox.Show("Debes seleccionar centro de costo", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return true; }
            if (dtProg.EditValue.ToString() == "") { XtraMessageBox.Show("Ingresar fecha Progresivo", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return true; }
            if (dtingr.EditValue.ToString() == "") { XtraMessageBox.Show("Ingresar fecha ingreso", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return true; }
            if (dtSal.EditValue.ToString() == "") { XtraMessageBox.Show("Ingresar fecha Salida", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return true; }
            if (txtfPago.EditValue.ToString() == "") { XtraMessageBox.Show("Ingresar forma de pago", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return true; }
            if (txtBanc.EditValue.ToString() == "") { XtraMessageBox.Show("Seleccione un banco", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return true; }
            if (txtcuenta.Text == "") { XtraMessageBox.Show("Ingrese un numero de cuenta", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return true; }
            if (txtSexo.EditValue.ToString() == "") { XtraMessageBox.Show("Seleccione un sexo", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return true; }

            return false;
        }             

        //CALCULO EDAD PARA ACTIVAR COMBO JUBILADO
        //SI LA EDAD ES MAYOR O IGUAL A 60 --> ACTIVAR COMBO JUBILADO
        //SI LA EDAD ES MENOR A 60 --> DESHABILITAR COMBO JUBILADO
        private void fnNeceitaJubilar(DateTime Nacimiento)
        {
            int edad = 0;
            edad = Persona.GetEdadFromNac(Nacimiento);

            if (edad >= 60)
            {
                txtjubilado.Enabled = true;
                txtjubilado.ItemIndex = 0;
            }
            else
            {
                txtjubilado.Enabled = false;
            }
        } 

        //COMBO CAUSAL
        private void fnComboCausal(LookUpEdit pCombobox, bool ocultarkey)
        {
            List<formula> lista = new List<formula>();

            string sql = "SELECT codCausal, descCausal FROM causaltermino";
            SqlCommand cmd;
            SqlDataReader re;

            if (fnSistema.ConectarSQLServer())
            {
                cmd = new SqlCommand(sql, fnSistema.sqlConn);
                re = cmd.ExecuteReader();

                if (re.HasRows)
                {
                    //int i = 0;
                    while (re.Read())
                    {
                        lista.Add(new formula() { desc = (string)re["descCausal"], key = (string)re["codCausal"]});
                    }
                }
                cmd.Dispose();
                re.Close();
                fnSistema.sqlConn.Close();
            }


            //PROPIEDADES COMBOBOX
            pCombobox.Properties.DataSource = lista.ToList();
            pCombobox.Properties.ValueMember = "key";
            pCombobox.Properties.DisplayMember = "desc";

            pCombobox.Properties.PopulateColumns();
            //SI OCULTAR KEY ES TRUE NO SE DEBE MOSTRAR LA COLUMNA KEY
            if (ocultarkey == true)
            {
                pCombobox.Properties.Columns[0].Visible = false;
            }
            pCombobox.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
            pCombobox.Properties.AutoSearchColumnIndex = 1;
            pCombobox.Properties.ShowHeader = false;

        }    

        //VERIFICAR SI EL CONTRATO TIENE AUSENTISMOS ASOCIADOS
        private bool fnAusentismoAsociado(string pContrato)
        {
            string sql = "SELECT contrato FROM ausentismo WHERE contrato=@pContrato";
            SqlCommand cmd;
            SqlDataReader rd;
            SqlConnection cn;
            bool tiene = false;
            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@pContrato", pContrato));

                            rd = cmd.ExecuteReader();                            
                            if (rd.HasRows)
                            {
                                //EXISTE O TIENE AUSENTISMOS ASOCIADOS
                                tiene = true;
                            }
                            else
                            {
                                tiene = false;
                            }
                            cmd.Dispose();
                            rd.Close();
                        }                        
                    }
                }
                else
                {
                    XtraMessageBox.Show("No hay conexion con la base de datos", "Base de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
            return tiene;
        }

        //SABER SI LA PERSONA ES MAYOR DE EDAD
        //NECESITAMOS LA FECHA DE NACIMIENTO DE LA PERSONA
        private bool fnMayorEdad(DateTime pNacimiento)
        {
            bool Mayor = false;
            int yearNac = pNacimiento.Year;
            //AÑO ACTUAL
            int yearActual = DateTime.Now.Date.Year;

            int edad = 0;
            //RESTAMOS EL AÑO EN CURSO CON EN AÑO DE LA FECHA DE NACIMIENTO
            //SI LA CANTIDAD ES 18 HACIA ARRIBA ES MAYOR DE EDAD (PUEDE TRABAJAR)
            //SI ES MENOR DE EDAD SE SUPONE QUE NO PUEDE SER UN TRABAJADOR VALIDO
            edad = yearActual - yearNac;
            if (edad >= 15)
            {
                //ES MAYOR DE EDAD
                Mayor = true;
            }
            else
            {
                //ES MENOR DE EDAD
                Mayor = false;
            }
            return Mayor;
        }

        //VERIFICAR QUE LA FECHA DE INGRESO ES MENOR A LA FECHA DE TERMINO DE CONTRATO Y VICEVERSA
        private string fnFechasCorrectas(DateTime pInicio, DateTime pTermino)
        {
            int comparacionIn = 0, comparacionTer = 0;
            comparacionIn = DateTime.Compare(pInicio, pTermino);
            comparacionTer = DateTime.Compare(pTermino, pInicio);
            string mensaje = "";
            if (comparacionIn < 0)
            {
                //INICIO ES ANTERIOR A TERMINO
                //LO CUAL ES CORRECTO
                mensaje = "";
            }
            //else if (comparacionIn  == 0)
            //{
            //    //SON FECHAS IGUALES
            //    mensaje = "La fecha de inicio no puede ser igual a la fecha de termino de contrato";
                
            //}
            else if (comparacionIn > 0)
            {
                //INICIO ES PORTERIOR A TERMINO
                mensaje = "La fecha de inicio no puede ser superior a la fecha de termino de contrato";
            }
            else if (comparacionTer < 0 )
            {
                //TERMINO ES ANTERIOR A INICIO
                mensaje = "La fecha de termino no puede ser inferior a la fecha de inicio";
            }
            else if (comparacionTer > 0)
            {
                //TERMINO ES POSTERIOR A INICIO
                mensaje = "";
            }

            return mensaje;
        }

        //VERIFICAR QUE LA FECHA DE INGRESO DE CONTRATO ESTE DENTRO DEL PERIODO EN CURSO????  
        private bool FechasPeriodo(DateTime pInicio, DateTime pTermino, int periodo)
        {
            string date = "", mes = "", year = "";
            DateTime inicioPeriodo = DateTime.Now.Date;
            DateTime TerminoPeriodo = DateTime.Now.Date;

            inicioPeriodo = fnSistema.FechaPeriodo(periodo);

            int dias = 0;
            date = periodo.ToString();
            year = date.Substring(0, 4);
            mes = date.Substring(4, 2);

            dias = DateTime.DaysInMonth(int.Parse(year), int.Parse(mes));

            date = dias + "-" + mes + "-" + year;
            TerminoPeriodo = DateTime.Parse(date);

            if ((pInicio >= inicioPeriodo && pInicio <= TerminoPeriodo) && (pTermino >= inicioPeriodo))
            {
                //LAS FECHAS ESTAN DENTRO DEL RANGO DE FECHAS DEL PERIODO EN CURSO
                return true;
            }
            else
            {
                //FECHA FUERA DE INTERVALO
                return false;
            }
        }

        //OBTENER LISTADO DE CONTRATO ASOCIADOS AL PERIODO ACTUAL
        private List<string> ListadoContratoPeriodo(int periodoActual)
        {
            string sql = "SELECT contrato FROM trabajador WHERE anomes=@periodo";
            SqlCommand cmd;
            SqlDataReader rd;
            List<string> lista = new List<string>();
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@periodo", periodoActual));
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                lista.Add((string)rd["contrato"]);
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

            return lista;
        }

        //TRANSACCION PARA ELIMINACION DE TODOS LOS DATOS ASOCIADOS AL TRABAJADOR EN EL PERIODO SELECCIONADO!
        private bool Transaccion(string pContrato, string pRut)
        {
            //SI EL TRABAJADOR TIENE HISTORICO NO SE DEBE ELIMINAR

            bool tieneItem = false, tieneAusentismos = false, tieneFamiliar = false, tieneHistorico = false, tieneVacacion = false, tieneDetalleVacacion = false;
            bool existenDatos = false, transaccionCorrecta = false, existeCalculo = false, existeMensual = false;

            tieneItem = Persona.TieneItems(pContrato, Calculo.PeriodoObservado);
            tieneAusentismos = fnAusentismoAsociado(pContrato);
            tieneFamiliar = tieneFamiliares(pContrato);
            tieneHistorico = Calculo.ExisteLiquidacionHistorico(pContrato);
            tieneVacacion = ExisteVacaciones(pContrato);
            tieneDetalleVacacion = ExisteDetalleVacaciones(pContrato);
            existenDatos = ExistenDatosDeCalculo(pContrato);
            //existeCalculo = ExisteDataCalculo(pContrato);
            existeMensual = Calculo.ExisteCalculoMensual(pContrato, Calculo.PeriodoObservado);

            //PARA LA TRANSACCION
            SqlCommand cmd;
            SqlTransaction tran;
            SqlConnection cn;

            //QUERIES PARA ELIMINACION
            string sqlItem = "DELETE FROM Itemtrabajador WHERE contrato=@contrato";          

            string sqlAusentismo = "DELETE FROM AUSENTISMO WHERE contrato=@pContrato";

            string sqlFamiliar = "DELETE FROM CARGAFAMILIAR WHERE contrato=@contrato";

            string sqlHistorico = "DELETE FROM LIQUIDACIONHISTORICO WHERE contrato=@contrato";

            string sqlDetalleVacacion = "DELETE FROM VACACIONDETALLE WHERE contrato=@contrato";

            string sqlVacacion = "DELETE FROM VACACION WHERE contrato=@contrato";

            string sqlDatosCalculo = "DELETE FROM DATOSCALCULO WHERE contrato=@contrato";

            string sqlTrabajador = "DELETE FROM trabajador WHERE contrato=@contrato";

            //PARA ELIMINAR LOS DATOS DE CALCULO
            string sqlTablaCalculo = "DELETE FROM CALCULO WHERE contrato=@contrato";

            //ELIMINAR DATOS DESDE TABLA CALCULOMENSUAL
            string sqlTablaCalculoMensual = "DELETE FROM CALCULOMENSUAL WHERE contrato=@contrato";
            

            DialogResult advertencia = XtraMessageBox.Show("¿Realmente desea eliminar este trabajador?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (advertencia == DialogResult.Yes)
            {
                try
                {
                    cn = fnSistema.OpenConnection();
                    if (cn != null)
                    {
                        using (cn)
                        {
                            tran = cn.BeginTransaction();
                            try
                            {
                                if (tieneItem)
                                {
                                    //@1 ELIMINAR ITEM ASOCIADOS SI ES QUE HAY CLARO ESTA!
                                    cmd = new SqlCommand(sqlItem, cn);

                                    //PARAMETROS
                                    cmd.Parameters.Add(new SqlParameter("@contrato", pContrato));

                                    //LE DECIMOS QUE VAMOS A TRABAJADOR LA CONSULTA COMO UNA TRANSACCION
                                    cmd.Transaction = tran;

                                    //EJECUTAMOS LA CONSULTA
                                    cmd.ExecuteNonQuery();
                                }
                                if (tieneAusentismos)
                                {
                                    //@2 ELIMINAR AUSENTISMOS ASOCIADOS
                                    cmd = new SqlCommand(sqlAusentismo, cn);

                                    //PARAMETROS
                                    cmd.Parameters.Add(new SqlParameter("@pContrato", pContrato));

                                    //LE DECIMOS QUE VAMOS A TRABAJAR LA CONSULTA COMO UNA TRANSACCION
                                    cmd.Transaction = tran;

                                    //EJECUTAMOS LA CONSULTA
                                    cmd.ExecuteNonQuery();
                                }
                                if (tieneFamiliar)
                                {
                                    //@3 ELIMINADAR FAMILIARES ASOCIADOS
                                    cmd = new SqlCommand(sqlFamiliar, cn);

                                    //PARAMETROS
                                    cmd.Parameters.Add(new SqlParameter("@contrato", pContrato));

                                    //LE DECIMOS QUE LO VAMOS A TRABAJAR COMO SI FUERA UNA TRANSACCION
                                    cmd.Transaction = tran;

                                    //EJECUTAMOS LA CONSULTA
                                    cmd.ExecuteNonQuery();
                                }
                                if (tieneHistorico)
                                {
                                    //@4 ELIMINAMOS LIQUIDACION HISTORICA
                                    cmd = new SqlCommand(sqlHistorico, cn);

                                    //PARAMETROS
                                    cmd.Parameters.Add(new SqlParameter("@contrato", pContrato));

                                    //EJECUTAMOS COMO TRANSACCION
                                    cmd.Transaction = tran;

                                    cmd.ExecuteNonQuery();
                                }
                                if (tieneDetalleVacacion)
                                {
                                    cmd = new SqlCommand(sqlDetalleVacacion, cn);

                                    //PARAMETROS
                                    cmd.Parameters.Add(new SqlParameter("@contrato", pContrato));

                                    cmd.Transaction = tran;

                                    cmd.ExecuteNonQuery();
                                }
                                if (tieneVacacion)
                                {
                                    cmd = new SqlCommand(sqlVacacion, cn);

                                    //PARAMETRO
                                    cmd.Parameters.Add(new SqlParameter("@contrato", pContrato));

                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                }
                                if (existenDatos)
                                {
                                    //ELIMINAR DATOS DE CALCULO
                                    cmd = new SqlCommand(sqlDatosCalculo, cn);

                                    //PARAMETROS
                                    cmd.Parameters.Add(new SqlParameter("@contrato", pContrato));

                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                }
                                if (existeCalculo)
                                {
                                    cmd = new SqlCommand(sqlTablaCalculo, cn);
                                    //PARAMETROS
                                    cmd.Parameters.Add(new SqlParameter("@contrato", pContrato));

                                    cmd.Transaction = tran;
                                    cmd.ExecuteNonQuery();
                                }
                                if (existeMensual)
                                {
                                    //ELIMINAR DATOS CALCULOMENSUAL
                                    using (cmd = new SqlCommand(sqlTablaCalculoMensual, cn))
                                    {
                                        //PARAMETROS
                                        cmd.Parameters.Add(new SqlParameter("@contrato", pContrato));
                                        cmd.Transaction = tran;
                                        cmd.ExecuteNonQuery();
                                    }
                                }

                                //ELIMINAR FINALMENTE DATOS DE TABLA TRABAJADOR
                                cmd = new SqlCommand(sqlTrabajador, cn);

                                //PARAMETROS
                                cmd.Parameters.Add(new SqlParameter("@contrato", pContrato));
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();

                                //REALIZAMOS COMMIT SI TODO SALIO BIEN
                                tran.Commit();

                                transaccionCorrecta = true;
                                cmd.Dispose();
                            }
                            catch (Exception ex)
                            {
                                //SI HAY ERROR DESHACEMOS LOS CAMBIOS
                                tran.Rollback();
                                transaccionCorrecta = false;
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    XtraMessageBox.Show(ex.Message);
                }
            }
            //RETORNAMOS VARIABLE 
            return transaccionCorrecta;
        }

       

        //VERIFICAR SI EXISTEN DATOS PARA ESE CONTRATO EN TABLA CALCULO
        private bool ExisteDataCalculo(string contrato)
        {
            string sql = "SELECT contrato FROM CALCULO where contrato=@contrato";
            SqlCommand cmd;
            SqlDataReader rd;
            SqlConnection cn;
            bool existe = false;
            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@contrato", contrato));

                            rd = cmd.ExecuteReader();
                            if (rd.HasRows)
                            {
                                //SI HAY REGISTROS EXISTE ES TRUE!
                                existe = true;
                            }
                            else
                            {
                                existe = false;
                            }
                            cmd.Dispose();
                            rd.Close();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return existe;
        }

        //VERIFICAR QUE SE HAYAN SETEADO LOS INDICES MENSUALES
        private bool ExistenIndicesMensuales(int periodo)
        {
            string sql = "select uf, utm, ingresominimo, sis, topeafp, topesec " +
                "FROM valoresmes WHERE anomes=@periodo";
            SqlCommand cmd;
            SqlDataReader rd;
            bool existe = false;

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@periodo", periodo));
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                if ((decimal)rd["uf"] == 0 || (decimal)rd["utm"] == 0 
                                    || (decimal)rd["ingresominimo"] == 0 
                                    || (decimal)rd["sis"] == 0 || (decimal)rd["topeafp"] == 0 
                                    || (decimal)rd["topesec"] == 0)
                                {
                                    //SI ALGUNO ES CERO RETORNAMOS TRUE
                                    existe = true;
                                }                                
                            }
                        }
                        else
                        {
                            existe = true;
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

            return existe;
        }

        //VERIFICAR LAS FECHAS DE CONTRATO Y EL MISMO RUT YA TIENE OTRO CONTRATO ASOCIADO EN EL MISMO PERIODO
        private bool VerificaFechaNuevoContrato(string rut, int periodo, DateTime Inicio, DateTime Termino, string contratoBd)
        {
            bool Novalido = false, encontrado = false;
            //OBTENER LA FECHA DE INGRESO DEL             
            SqlCommand cmd;
            SqlDataReader rd;
            SqlConnection cn;
            DateTime fechaIni = DateTime.Now.Date;
            DateTime fechaTer = DateTime.Now.Date;

            //OBTENER LAS FECHAS CONTRACTUALES DEL RUT
            string sql = "";
            if (contratoBd == "")
                sql = "SELECT ingreso, salida FROM trabajador WHERE anomes=@pAnomes AND rut=@pRut";
            else
                sql = "SELECT ingreso, salida FROM trabajador WHERE anomes=@pAnomes AND rut=@pRut AND contrato<>@pContrato";

            if (RutTieneContratos(rut, periodo))
            {
                try
                {
                    cn = fnSistema.OpenConnection();
                    if (cn != null)
                    {
                        using (cn)
                        {
                            using (cmd = new SqlCommand(sql, cn))
                            {
                                //PARAMETROS
                                cmd.Parameters.Add(new SqlParameter("@pAnomes", periodo));
                                cmd.Parameters.Add(new SqlParameter("@pRut", rut));
                                if (contratoBd != "")
                                    cmd.Parameters.Add(new SqlParameter("@pContrato", contratoBd));

                                rd = cmd.ExecuteReader();
                                if (rd.HasRows)
                                {
                                    while (rd.Read())
                                    {
                                        fechaIni = Convert.ToDateTime(rd["ingreso"]);
                                        fechaTer = Convert.ToDateTime(rd["salida"]);

                                        //COMPARAMOS FECHAS 
                                        //SI LAS FECHA NUEVAS ESTAN DENTRO DE LAS FECHAS YA REGISTRADAS NO SON VALIDAS
                                        for (DateTime i = fechaIni; i <= fechaTer; i = i.AddDays(1))
                                        {
                                            for (DateTime j = Inicio; j <= Termino; j = j.AddDays(1))
                                            {
                                                if (i == j)
                                                { Novalido = true; encontrado = true; break; }
                                            }

                                            if (encontrado)
                                                break;
                                        }

                                        if (Novalido)
                                            break;
                                    }
                                }

                                cmd.Dispose();
                                rd.Close();
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                  //ERROR...
                }
            }      


            return Novalido;
        }

        //VERIFICAR SI TIENE EL RUT YA TIENE UN CONTRATO ASOCIADO PARA EL PERIODO EN EVALUACION
        private bool RutTieneContratos(string rut, int periodo)
        {
            bool tiene = false;
            
            string sql = "SELECT contrato FROM trabajador WHERE rut=@pRut AND anomes=@pAnomes";
            
            SqlCommand cmd;
            SqlDataReader rd;
            SqlConnection cn;
            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@pRut", rut));
                            cmd.Parameters.Add(new SqlParameter("@pAnomes", periodo));

                            rd = cmd.ExecuteReader();
                            if (rd.HasRows)
                            {
                                tiene = true;
                            }

                            cmd.Dispose();                     
                            rd.Close();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return tiene;
        }

        /// <summary>
        /// Permite cargar los datos de una ficha para copia da información.
        /// </summary>
        /// <param name="pContratoCopia">Numero de contrato copia</param>
        /// <param name="pPeriodoCopia">Periodo ficha copia.</param>
        /// <param name="pPrimerNombre">Primer nombre nuevo trabajador.</param>
        /// <param name="pSegundoNombre">Segundo nombre nuevo trabajador.</param>
        /// <param name="pApellidoPaterno">Apellido paterno nuevo trabajador.</param>
        /// <param name="pApellidoMaterno">Apellido materno nuevo trabajador.</param>
        /// <param name="pNuevoContrato">Numero de contrato nuevo trabajador.</param>
        /// <param name="pRutNuevo">Rut nuevo trabajador.</param>
        private void fnDatosCopia(string pRutNuevo, string pContratoCopia, int pPeriodoCopia, string pPrimerNombre, string pSegundoNombre, string pApellidoPaterno, string pApellidoMaterno, string pNuevoContrato)
        {
            //SQL 
            string sql = "SELECT contrato, rut, pasaporte, status, anomes, nombre, apepaterno, apematerno, direccion," +
                        "ciudad, area, ccosto, fechanac, nacion, ecivil, telefono, ingreso, salida, cargo, tipocontrato, " +
                        "regimenSalario, jubilado, regimen, afp, salud, cajaPrevision, fechaSegCes, tramo, formapago, banco, " +
                        "cuenta, fechavacacion, fechaprogresivo, rutafoto, tipoCuenta, anosprogresivo, clase, causal, sexo, " +
                        "sucursal, mail, esco, numemer, nomemer, talla, calzado, horario, jornada, sindicato, comuna " +
                        " FROM trabajador" +
                        " WHERE contrato = @pContrato AND anomes=@Periodo ";

            SqlCommand cmd;
            SqlDataReader rd;
            SqlConnection cn;

            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            //PARAMETROS                            
                            cmd.Parameters.Add(new SqlParameter("@pContrato", pContratoCopia));
                            cmd.Parameters.Add(new SqlParameter("@Periodo", pPeriodoCopia));

                            //EJECUTAR CONSULTA
                            rd = cmd.ExecuteReader();
                            if (rd.HasRows)
                            {
                                //CARGAMOS DATOS
                                while (rd.Read())
                                {
                                    //DESHABILITAR BOTONES 
                                    btnAusentismo.Enabled = false;
                                    btnFamiliares.Enabled = false;
                                    btnHistorico.Enabled = false;
                                    btnItemMes.Enabled = false;
                                    btnVacaciones.Enabled = false;
                                    btnLiquidacion.Enabled = false;
                                    btnFichaTrabajador.Enabled = false;
                                    btnFiniquito.Enabled = false;
                                    btnDocContrato.Enabled = false;
                                    btnAntiguedad.Enabled = false;
                                    btnObservaciones.Enabled = false;

                                    //DEJAMOS RUT Y CONTRATO PROPIEDAD REAONLY EN FALSE
                                    txtRut.ReadOnly = false;
                                    txtcontrato.ReadOnly = false;

                                    FichaHistorica = false;

                                    //BOTON NUEVO
                                    op.Cancela = true;
                                    op.SetButtonProperties(btnNuevo, 2);

                                    //CARGAMOS EN CAJA DE TEXTO NUEVO NUMERO DE CONTRATO
                                    txtcontrato.Text = pNuevoContrato;
                                    txtRut.Text = pRutNuevo;

                                    //NOMBRES DEL NUEVO TRABAJADOR
                                    txtNombre.Text = pPrimerNombre + " " + pSegundoNombre;
                                    txtapePaterno.Text = pApellidoPaterno;
                                    txtApeMat.Text = pApellidoMaterno;

                                    txtPasaporte.Text = (string)rd["pasaporte"];
                                    txtEstado.EditValue = 1;

                                    dtingr.EditValue = (DateTime)rd["ingreso"];
                                    dtSal.EditValue = (DateTime)rd["salida"];
                                    dtProg.EditValue = (DateTime)rd["fechaprogresivo"];

                                    txtDirec.Text = (string)rd["direccion"];
                                    txtCiudad.EditValue = (int)rd["ciudad"];
                                    txtArea.EditValue = (int)rd["area"];
                                    txtccosto.EditValue = (int)rd["ccosto"];
                                    dtFecNac.EditValue = (DateTime)rd["fechanac"];
                                    txtNacion.EditValue = (int)rd["nacion"];
                                    txteCivil.EditValue = (int)rd["ecivil"];
                                    txtFono.Text = (string)rd["telefono"];                                    

                                    txtCargo.EditValue = (int)rd["cargo"];
                                    txtTipoCont.EditValue = (int)rd["tipoContrato"];
                                    txtRegsal.EditValue = (int)rd["regimenSalario"];
                                    txtjubilado.EditValue = (Int16)rd["jubilado"];
                                    txtRegimen.EditValue = (int)rd["regimen"];
                                    txtAfp.EditValue = (int)rd["afp"];

                                    txtCajaPrevision.EditValue = (int)rd["cajaPrevision"];
                                    dtSegCes.EditValue = (DateTime)rd["fechaSegCes"];
                                    txtTramo.EditValue = (int)rd["tramo"];
                                    txtfPago.EditValue = (int)rd["formapago"];
                                    txtBanc.EditValue = (int)rd["banco"];
                                    txtcuenta.EditValue = (string)rd["cuenta"];
                                    dtVac.EditValue = (DateTime)rd["fechaVacacion"];

                                    spProg.Value = (decimal)rd["anosprogresivo"];
                                    txtTipoCuenta.EditValue = (int)rd["tipoCuenta"];
                                    txtClase.EditValue = (int)rd["clase"];
                                    txtCausal.EditValue = Convert.ToInt32(rd["causal"]);
                                    txtSexo.EditValue = (Int16)rd["sexo"];
                                    txtSucursal.EditValue = (Convert.ToInt32(rd["sucursal"]));
                                    txtSalud.EditValue = Convert.ToInt32(rd["salud"]);
                                    txtEmail.Text = (string)rd["mail"];
                                    txtEstudios.EditValue = Convert.ToInt32(rd["esco"]);

                                    txtFonoEmer.Text = (string)rd["numemer"];
                                    txtNombreEmer.Text = (string)rd["nomemer"];
                                    txtTalla.Text = (string)rd["talla"];
                                    txtCalzado.Text = (string)rd["calzado"];
                                    txtHorario.EditValue = Convert.ToInt32(rd["horario"]);
                                    txtJornadaLaboral.EditValue = Convert.ToInt16(rd["jornada"]);
                                    txtSindicato.EditValue = Convert.ToInt32(rd["sindicato"]);
                                    txtComuna.EditValue = Convert.ToInt32(rd["comuna"]);

                                    //SOLO MOSTRAR IMAGEN POR DEFECTO
                                    pictureEmpleado.Image = Labour.Properties.Resources.SinFoto;
                                    pictureEmpleado.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;

                                    //GUARDAR IMAGEN DE FORMA TEMPORAL EN COMPUTADOR
                                    Labour.Properties.Resources.SinFoto.Save(pathFile + "trabajador.jpg", ImageFormat.Jpeg);

                                    //guardamos la ruta de la imagen en la variable pathimage
                                    pathImage = pathFile + "trabajador.jpg";

                                    //DEJAR IMAGEN USUARIO NULA
                                    imagenUsuario = null;

                                    //MOSTRAR NOMBRE COMPLETO Y EDAD DEBAJO D FOTO DE PERFIL
                                    int ed = Persona.GetEdadFromNac((DateTime)rd["fechanac"]);
                                    lblApellido.Text = pApellidoPaterno + " " + pApellidoMaterno;
                                    lblnombre.Text = pPrimerNombre + " " + pSegundoNombre;
                                    lbledad.Text = "EDAD: " + ed + " AÑOS";

                                    // lblperiodo.Text = "PERIODO: " + Calculo.PeriodoEvaluar();
                                }
                            }
                        }
                        //LIBERAMOS RECURSOS
                        cmd.Dispose();
                        rd.Close();
                    }
                }
                else
                {
                    XtraMessageBox.Show("No hay conexion con la base de datos", "Base de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        #region "LOG REGISTRO EMPLEADO"
        //CREAR TABLA HASH DE DATOS DEL EMPLEADO
        //DATOS DE ENTRADA: CONTRATO Y PERIODO 
        private Hashtable PrecargaEmpleado(string Contrato, int Periodo)
        {
            Hashtable datos = new Hashtable();
            string sql = "SELECT rut, pasaporte, status, nombre, apepaterno, apematerno, direccion, ciudad, area, " +
                "ccosto, fechanac, nacion, ecivil, telefono, ingreso, salida, cargo, tipocontrato, regimensalario, " +
                "jubilado, regimen, afp, salud, cajaprevision, fechasegces, tramo, formapago, banco, cuenta, " +
                "fechavacacion, fechaprogresivo, anosprogresivo, tipocuenta, clase, causal, sexo, sucursal, " +
                "fun, privado, mail, esco, numemer, nomemer, talla, calzado, horario, jornada, sindicato, comuna " +
                "FROM trabajador WHERE contrato=@contrato AND anomes=@periodo";
            SqlCommand cmd;
            SqlDataReader rd;
            SqlConnection cn;
            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@contrato", Contrato));
                            cmd.Parameters.Add(new SqlParameter("@periodo", Periodo));

                            rd = cmd.ExecuteReader();
                            if (rd.HasRows)
                            {
                                while (rd.Read())
                                {
                                    //LLENAR TABLA HASH
                                    datos.Add("rut", (string)rd["rut"]);
                                    datos.Add("pasaporte", (string)rd["pasaporte"]);
                                    datos.Add("status", (Int16)rd["status"]);
                                    datos.Add("nombre", (string)rd["nombre"]);
                                    datos.Add("apepaterno", (string)rd["apepaterno"]);
                                    datos.Add("apematerno", (string)rd["apematerno"]);
                                    datos.Add("direccion", (string)rd["direccion"]);
                                    datos.Add("ciudad", (int)rd["ciudad"]);
                                    datos.Add("area", (int)rd["area"]);
                                    datos.Add("ccosto", (int)rd["ccosto"]);
                                    datos.Add("fechanac", (DateTime)rd["fechanac"]);
                                    datos.Add("nacion", (int)rd["nacion"]);
                                    datos.Add("ecivil", (int)rd["ecivil"]);
                                    datos.Add("telefono", (string)rd["telefono"]);
                                    datos.Add("ingreso", (DateTime)rd["ingreso"]);
                                    datos.Add("salida", (DateTime)rd["salida"]);
                                    datos.Add("cargo", (int)rd["cargo"]);
                                    datos.Add("tipocontrato", (int)rd["tipocontrato"]);
                                    datos.Add("regimensalario", (int)rd["regimensalario"]);
                                    datos.Add("jubilado", (Int16)rd["jubilado"]);
                                    datos.Add("regimen", (int)rd["regimen"]);
                                    datos.Add("afp", (int)rd["afp"]);
                                    datos.Add("salud", (int)rd["salud"]);
                                    datos.Add("cajaprevision", (int)rd["cajaprevision"]);
                                    datos.Add("fechasegces", (DateTime)rd["fechasegces"]);
                                    datos.Add("tramo", (int)rd["tramo"]);
                                    datos.Add("formapago", (int)rd["formapago"]);
                                    datos.Add("banco", (int)rd["banco"]);
                                    datos.Add("cuenta", (string)rd["cuenta"]);
                                    datos.Add("fechavacacion", (DateTime)rd["fechavacacion"]);
                                    datos.Add("fechaprogresivo", (DateTime)rd["fechaprogresivo"]);
                                    datos.Add("anosprogresivo", (decimal)rd["anosprogresivo"]);
                                    datos.Add("tipocuenta", (int)rd["tipocuenta"]);
                                    datos.Add("clase", (int)rd["clase"]);
                                    datos.Add("causal", Convert.ToInt32(rd["causal"]));
                                    datos.Add("sexo", (Int16)rd["sexo"]);
                                    datos.Add("sucursal", Convert.ToInt32(rd["sucursal"]));
                                    datos.Add("fun", (string)rd["fun"]);
                                    datos.Add("mail", (string)rd["mail"]);
                                    datos.Add("esco", Convert.ToInt32(rd["esco"]));
                                    datos.Add("numemer", (string)rd["numemer"]);
                                    datos.Add("nomemer", (string)rd["nomemer"]);
                                    datos.Add("talla", (string)rd["talla"]);
                                    datos.Add("calzado", (string)rd["calzado"]);
                                    datos.Add("horario", Convert.ToInt32(rd["horario"]));
                                    datos.Add("jornada", Convert.ToInt16(rd["jornada"]));
                                    datos.Add("sindicato", Convert.ToInt32(rd["sindicato"]));
                                    datos.Add("comuna", Convert.ToInt32(rd["comuna"]));
                                }
                            }
                        }
                        cmd.Dispose();
                        rd.Close();
                    }
                }            
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return datos;
        }

        //COMPARA VALORES 
        private void ComparaDatosEmpleado(Hashtable Listado, string pRut, string pPasaporte, Int16 pStatus, string pNombre, 
            string pPaterno, string pMaterno, string pDireccion, int pCiudad, int pArea, int pCosto, DateTime pNaci, 
            int pNacion, int pCivil, string pTelefono, DateTime pIngreso, DateTime pSalida, int pCargo, int pTipoCont, 
            int pRegimenSalario, Int16 pJubilado, int pRegimen, int pAfp, int pSalud, int pCajaPrevision, 
            DateTime pFechaSegCes, int pTramo, int pFormaPago, int pBanco, string pCuenta, DateTime pFechaVac, 
            DateTime pFechaProgresivo, decimal pAniosProgresivo, int pTipoCuenta, int pClase, int pCausal, 
            Int16 pSex, string pContrato, int pSucursal, string pFun, string pMail, int pEscolaridad, string pNumEmer, 
            string pNomEmer, string pTalla, string pCalzado, int pHorario, int pJornada, int pSindicato, int pComuna)
        {               
            
            string Descripcion = "", Antiguo = "", Nuevo = "";
            
            if (Listado.Count > 0)
            {
                //COMPARAMOS VALORES Y SI SON DISTINTOS ES PORQUE HAY CAMBIOS (GUARDAMOS EN LOG)
                if ((string)Listado["rut"] != pRut)
                {
                    Descripcion = "SE MODIFICA RUT TRABAJADOR N° " + pContrato;
                    Antiguo = (string)Listado["rut"];
                    Nuevo = pRut;
                    logRegistro log = new logRegistro(User.getUser(), Descripcion, "TRABAJADOR", Antiguo, Nuevo, "MODIFICAR");
                    log.Log();
                }
                if ((string)Listado["pasaporte"] != pPasaporte )
                {
                    Descripcion = "SE MODIFICA PASAPORTE TRABAJADOR N° " + pContrato;
                    Antiguo = (string)Listado["pasaporte"];
                    Nuevo = pPasaporte;
                    logRegistro log = new logRegistro(User.getUser(), Descripcion, "TRABAJADOR", Antiguo, Nuevo, "MODIFICAR");
                    log.Log();
                }
                if ((Int16)Listado["status"] != pStatus)
                {
                    Descripcion = "SE MODIFICA STATUS TRABAJADOR N° " + pContrato;
                    Antiguo = (Int16)Listado["status"] + "";
                    Nuevo = pStatus + "";
                    logRegistro log = new logRegistro(User.getUser(), Descripcion, "TRABAJADOR", Antiguo, Nuevo, "MODIFICAR");
                    log.Log();
                }
                if ((string)Listado["nombre"] != pNombre)
                {
                    Descripcion = "SE MODIFICA NOMBRE TRABAJADOR N°" + pContrato;
                    Antiguo = (string)Listado["nombre"];
                    Nuevo = pNombre;
                    logRegistro log = new logRegistro(User.getUser(), Descripcion, "TRABAJADOR", Antiguo, Nuevo, "MODIFICAR");
                    log.Log();
                }
                if ((string)Listado["apepaterno"] != pPaterno)
                {
                    Descripcion = "SE MODIFICA APELLIDO PATERNO TRABAJADOR N° " + pContrato;
                    Antiguo = (string)Listado["apepaterno"];
                    Nuevo = pPaterno;
                    logRegistro log = new logRegistro(User.getUser(), Descripcion, "TRABAJADOR", Antiguo, Nuevo, "MODIFICAR");
                    log.Log();
                }
                if ((string)Listado["apematerno"] != pMaterno)
                {
                    Descripcion = "SE MODIFICA APELLIDO MATERNO TRABAJADOR N° " + pContrato;
                    Antiguo = (string)Listado["apematerno"];
                    Nuevo = pMaterno;
                    logRegistro log = new logRegistro(User.getUser(), Descripcion, "TRABAJADOR", Antiguo, Nuevo, "MODIFICAR");
                    log.Log();
                }
                if ((string)Listado["direccion"] != pDireccion)
                {
                    Descripcion = "SE MODIFICA DIRECCION TRABAJADOR N°" + pContrato;
                    Antiguo = (string)Listado["direccion"];
                    Nuevo = pDireccion;
                    logRegistro log = new logRegistro(User.getUser(), Descripcion, "TRABAJADOR", Antiguo, Nuevo, "MODIFICAR");
                    log.Log();
                }
                if ((int)Listado["ciudad"] != pCiudad)
                {
                    Descripcion = "SE MODIFICA CIUDAD TRABAJADOR N° " + pContrato;
                    Antiguo = (int)Listado["ciudad"] + "";
                    Nuevo = pCiudad + "";
                    logRegistro log = new logRegistro(User.getUser(), Descripcion, "TRABAJADOR", Antiguo, Nuevo, "MODIFICAR");
                    log.Log();
                }
                if ((int)Listado["area"] != pArea)
                {
                    Descripcion = "SE MODIFICA AREA TRABAJADOR N° " + pContrato;
                    Antiguo = (int)Listado["area"] + "";
                    Nuevo = pArea + "";
                    logRegistro log = new logRegistro(User.getUser(), Descripcion, "TRABAJADOR", Antiguo, Nuevo, "MODIFICAR");
                    log.Log();
                }
                if ((int)Listado["ccosto"] != pCosto)
                {
                    Descripcion = "SE MODIFICA CENTRO COSTO TRABAJADOR N°" + pContrato;
                    Antiguo = (int)Listado["ccosto"] + "";
                    Nuevo = pCosto + "";
                    logRegistro log = new logRegistro(User.getUser(), Descripcion, "TRABAJADOR", Antiguo, Nuevo, "MODIFICAR");
                    log.Log();
                }
                if ((DateTime)Listado["fechanac"] != pNaci)
                {
                    Descripcion = "SE MODIFICA FECHA NACIMIENTO TRABAJADOR N°" + pContrato;
                    Antiguo = (DateTime)Listado["fechanac"] + "";
                    Nuevo = pNaci + "";
                    logRegistro log = new logRegistro(User.getUser(), Descripcion, "TRABAJADOR", Antiguo, Nuevo, "MODIFICAR");
                    log.Log();
                }
                if ((int)Listado["nacion"] != pNacion)
                {
                    Descripcion = "SE MODIFICA PAIS TRABAJADOR N°" + pContrato;
                    Antiguo = (int)Listado["nacion"] + "";
                    Nuevo = pNacion + "";
                    logRegistro log = new logRegistro(User.getUser(), Descripcion, "TRABAJADOR", Antiguo, Nuevo, "MODIFICAR");
                    log.Log();
                }
                if ((int)Listado["ecivil"] != pCivil)
                {
                    Descripcion = "SE MODIFICA ESTADO CIVIL TRABAJADOR N° " + pContrato;
                    Antiguo = (int)Listado["ecivil"] + "";
                    Nuevo = pCivil + "";
                    logRegistro log = new logRegistro(User.getUser(), Descripcion, "TRABAJADOR", Antiguo, Nuevo, "MODIFICAR");
                    log.Log();
                }
                if ((string)Listado["telefono"] != pTelefono)
                {
                    Descripcion = "SE MODIFICA TELEFONO TRABAJADOR N°" + pContrato;
                    Antiguo = (string)Listado["telefono"];
                    Nuevo = pTelefono;
                    logRegistro log = new logRegistro(User.getUser(), Descripcion, "TRABAJADOR", Antiguo, Nuevo, "MODIFICAR");
                    log.Log();
                }
                if ((DateTime)Listado["ingreso"] != pIngreso)
                {
                    Descripcion = "SE MODIFICA FECHA INICIO CONTRATO TRABAJADOR N° " + pContrato;
                    Antiguo = (DateTime)Listado["ingreso"] + "";
                    Nuevo = pIngreso + "";
                    logRegistro log = new logRegistro(User.getUser(), Descripcion, "TRABAJADOR", Antiguo, Nuevo, "MODIFICAR");
                    log.Log();
                }
                if ((DateTime)Listado["salida"] != pSalida)
                {
                    Descripcion = "SE MODIFICA FECHA TERMINO CONTRATO TRABAJADOR N°" + pContrato;
                    Antiguo = (DateTime)Listado["salida"] + "";
                    Nuevo = pSalida + "";
                    logRegistro log = new logRegistro(User.getUser(), Descripcion, "TRABAJADOR", Antiguo, Nuevo, "MODIFICAR");
                    log.Log();
                }
                if ((int)Listado["cargo"] != pCargo)
                {
                    Descripcion = "SE MODIFICA CARGO TRABAJADOR N°" + pContrato;
                    Antiguo = (int)Listado["cargo"] + "";
                    Nuevo = pCargo + "";
                    logRegistro log = new logRegistro(User.getUser(), Descripcion, "TRABAJADOR", Antiguo, Nuevo, "MODIFICAR");
                    log.Log();
                }
                if ((int)Listado["tipocontrato"] != pTipoCont)
                {
                    Descripcion = "SE MODIFICA TIPO CONTRATO TRABAJADOR N°" + pContrato;
                    Antiguo = (int)Listado["tipocontrato"] + "";
                    Nuevo = pTipoCont + "";
                    logRegistro log = new logRegistro(User.getUser(), Descripcion, "TRABAJADOR", Antiguo, Nuevo, "MODIFICAR");
                    log.Log();
                }
                if ((int)Listado["regimensalario"] != pRegimenSalario)
                {
                    Descripcion = "SE MODIFICA REGIMEN SALARIO TRABAJADOR N° " + pContrato;
                    Antiguo = (int)Listado["regimensalario"] + "";
                    Nuevo = pRegimenSalario + "";
                    logRegistro log = new logRegistro(User.getUser(), Descripcion, "TRABAJADOR", Antiguo, Nuevo, "MODIFICAR");
                    log.Log();
                }
                if ((Int16)Listado["jubilado"] != pJubilado)
                {
                    Descripcion = "SE MODIFICA CAMPO JUBILADO TRABAJADOR N° " + pContrato;
                    Antiguo = (Int16)Listado["jubilado"] + "";
                    Nuevo = pJubilado + "";
                    logRegistro log = new logRegistro(User.getUser(), Descripcion, "TRABAJADOR", Antiguo, Nuevo, "MODIFICAR");
                    log.Log();
                }
                if ((int)Listado["regimen"] != pRegimen)
                {
                    Descripcion = "SE MODIFICA REGIMEN TRABAJADOR N° " + pContrato;
                    Antiguo = (int)Listado["regimen"] + "";
                    Nuevo = pRegimen + "";
                    logRegistro log = new logRegistro(User.getUser(), Descripcion, "TRABAJADOR", Antiguo, Nuevo, "MODIFICAR");
                    log.Log();
                }
                if ((int)Listado["afp"] != pAfp)
                {
                    Descripcion = "SE MODIFICA AFP TRABAJADOR N° " + pContrato;
                    Antiguo = (int)Listado["afp"] + "";
                    Nuevo = pAfp + "";
                    logRegistro log = new logRegistro(User.getUser(), Descripcion, "TRABAJADOR", Antiguo, Nuevo, "MODIFICAR");
                    log.Log();
                }
                if ((int)Listado["salud"] != pSalud)
                {
                    Descripcion = "SE MODIFICA SALUD TRABAJADOR N°" + pContrato;
                    Antiguo = (int)Listado["salud"] + "";
                    Nuevo = pSalud + "";
                    logRegistro log = new logRegistro(User.getUser(), Descripcion, "TRABAJADOR", Antiguo, Nuevo, "MODIFICAR");
                    log.Log();
                }
                if ((int)Listado["cajaprevision"] != pCajaPrevision)
                {
                    Descripcion = "SE MODIFICA CAJA PREVISION TRABAJADOR N°" + pContrato;
                    Antiguo = (int)Listado["cajaprevision"] + "";
                    Nuevo = pCajaPrevision + "";
                    logRegistro log = new logRegistro(User.getUser(), Descripcion, "TRABAJADOR", Antiguo, Nuevo, "MODIFICAR");
                    log.Log();
                }
                if ((DateTime)Listado["fechasegces"] != pFechaSegCes)
                {
                    Descripcion = "SE MODIFICA FECHA SEGURO CESANTIA TRABAJADOR N° " + pContrato;
                    Antiguo = (DateTime)Listado["fechasegces"] + "";
                    Nuevo = pFechaSegCes + "";
                    logRegistro log = new logRegistro(User.getUser(), Descripcion, "TRABAJADOR", Antiguo, Nuevo, "MODIFICAR");
                    log.Log();
                }
                if ((int)Listado["tramo"] != pTramo)
                {
                    Descripcion = "SE MODIFICA TRAMO TRABAJADOR N° " + pContrato;
                    Antiguo = (int)Listado["tramo"] + "";
                    Nuevo = pTramo + "";
                    logRegistro log = new logRegistro(User.getUser(), Descripcion, "TRABAJADOR", Antiguo, Nuevo, "MODIFICAR");
                    log.Log();
                }
                if ((int)Listado["formapago"] != pFormaPago)
                {
                    Descripcion = "SE MODIFICA FORMA PAGO TRABAJADOR N° " + pContrato;
                    Antiguo = (int)Listado["formapago"] + "";
                    Nuevo = pFormaPago + "";
                    logRegistro log = new logRegistro(User.getUser(), Descripcion, "TRABAJADOR", Antiguo, Nuevo, "MODIFICAR");
                    log.Log();
                }
                if ((int)Listado["banco"] != pBanco)
                {
                    Descripcion = "SE MODIFICA BANCO TRABAJADOR N° " + pContrato;
                    Antiguo = (int)Listado["banco"] + "";
                    Nuevo = pBanco + "";
                    logRegistro log = new logRegistro(User.getUser(), Descripcion, "TRABAJADOR", Antiguo, Nuevo, "MODIFICAR");
                    log.Log();
                }
                if ((string)Listado["cuenta"] != pCuenta)
                {
                    Descripcion = "SE MODIFICA CUENTA TRABAJADOR N° " + pContrato;
                    Antiguo = (string)Listado["cuenta"];
                    Nuevo = pCuenta;
                    logRegistro log = new logRegistro(User.getUser(), Descripcion, "TRABAJADOR", Antiguo, Nuevo, "MODIFICAR");
                    log.Log();
                }
                if ((DateTime)Listado["fechavacacion"] != pFechaVac)
                {
                    Descripcion = "SE MODIFICA FECHA VACACIONES TRABAJADOR N° " + pContrato;
                    Antiguo = (DateTime)Listado["fechavacacion"] + "";
                    Nuevo = pFechaVac + "";
                    logRegistro log = new logRegistro(User.getUser(), Descripcion, "TRABAJADOR", Antiguo, Nuevo, "MODIFICAR");
                    log.Log();
                }
                if ((DateTime)Listado["fechaprogresivo"] != pFechaProgresivo)
                {
                    Descripcion = "SE MODIFICA FECHA PROGRESIVO TRABAJADOR N° " + pContrato;
                    Antiguo = (DateTime)Listado["fechaprogresivo"] + "";
                    Nuevo = pFechaProgresivo + "";
                    logRegistro log = new logRegistro(User.getUser(), Descripcion, "TRABAJADOR", Antiguo, Nuevo, "MODIFICAR");
                    log.Log();
                }
                if ((decimal)Listado["anosprogresivo"] != pAniosProgresivo)
                {
                    Descripcion = "SE MODIFICA AÑOS PROGRESIVO TRABAJADOR N°" + pContrato;
                    Antiguo = (decimal)Listado["anosprogresivo"] + "";
                    Nuevo = pAniosProgresivo + "";
                    logRegistro log = new logRegistro(User.getUser(), Descripcion, "TRABAJADOR", Antiguo, Nuevo, "MODIFICAR");
                    log.Log();
                }
                if ((int)Listado["tipocuenta"] != pTipoCuenta)
                {
                    Descripcion = "SE MODIFICA TIPO CUENTA TRABAJADOR N° " + pContrato;
                    Antiguo = (int)Listado["tipocuenta"] + "";
                    Nuevo = pTipoCuenta + "";
                    logRegistro log = new logRegistro(User.getUser(), Descripcion, "TRABAJADOR", Antiguo, Nuevo, "MODIFICAR");
                    log.Log();
                }
                if ((int)Listado["clase"] != pClase)
                {
                    Descripcion = "SE MODIFICA CLASE REMUNERACION TRABAJADOR N° " + pContrato;
                    Antiguo = (int)Listado["clase"] + "";
                    Nuevo = pClase + "";
                    logRegistro log = new logRegistro(User.getUser(), Descripcion, "TRABAJADOR", Antiguo, Nuevo, "MODIFICAR");
                    log.Log();
                }
                if ((int)Listado["causal"] != pCausal)
                {
                    Descripcion = "SE MODIFICA CAUSAL TRABAJADOR N°" + pContrato;
                    Antiguo = Convert.ToInt32(Listado["causal"]).ToString();
                    Nuevo = pCausal.ToString();
                    logRegistro log = new logRegistro(User.getUser(), Descripcion, "TRABAJADOR", Antiguo, Nuevo, "MODIFICAR");
                    log.Log();
                }
                if ((Int16)Listado["sexo"] != pSex)
                {
                    Descripcion = "SE MODIFICA SEXO TRABAJADOR N° " + pContrato;
                    Antiguo = (Int16)Listado["sexo"] + "";
                    Nuevo = pSex + "";
                    logRegistro log = new logRegistro(User.getUser(), Descripcion, "TRABAJADOR", Antiguo, Nuevo, "MODIFICAR");
                    log.Log();
                }
                if (Convert.ToInt32(Listado["sucursal"]) != pSucursal)
                {
                    Descripcion = "SE MODIFICA SUCURSAL TRABAJADOR N° " + pContrato;
                    Antiguo = Convert.ToInt32(Listado["sucursal"]) + "";
                    Nuevo = pSucursal + "";
                    logRegistro log = new logRegistro(User.getUser(), Descripcion, "TRABAJADOR", Antiguo, Nuevo, "MODIFICAR");
                    log.Log();
                }
                if ((string)Listado["fun"] != pFun)
                {
                    Descripcion = "SE MODIFICA N° FUN TRABAJADOR N° " + pContrato;
                    Antiguo = (string)Listado["fun"];
                    Nuevo = pFun;
                    logRegistro log = new logRegistro(User.getUser(), Descripcion, "TRABAJADOR", Antiguo, Nuevo, "MODIFICAR");
                    log.Log();
                }
                if ((string)Listado["mail"] != pMail)
                {
                    Descripcion = "SE MODIFICA EMAIL TRABAJADOR N° " + pContrato;
                    Antiguo = (string)Listado["mail"];
                    Nuevo = pMail;
                    logRegistro log = new logRegistro(User.getUser(), Descripcion, "TRABAJADOR", Antiguo, Nuevo, "MODIFICAR");
                    log.Log();
                }
                if (Convert.ToInt32(Listado["esco"]) != pEscolaridad)
                {
                    Descripcion = "SE MODIFICA ESCOLARIDAD TRABAJADOR N° " + pContrato;
                    Antiguo = Convert.ToInt32(Listado["esco"]) + "";
                    Nuevo = pEscolaridad.ToString();
                    logRegistro log = new logRegistro(User.getUser(), Descripcion, "TRABAJADOR", Antiguo, Nuevo, "MODIFICAR");
                    log.Log();
                }
                if ((string)Listado["numemer"] != pNumEmer)
                {
                    Descripcion = "SE MODIFICA NUMERO DE EMERGNECIA TRABAJADOR N° " + pContrato;
                    Antiguo = Listado["numemer"].ToString();
                    Nuevo = pNumEmer;
                    logRegistro log = new logRegistro(User.getUser(), Descripcion, "TRABAJADOR", Antiguo, Nuevo, "MODIFICAR");
                    log.Log();
                }
                if ((string)Listado["nomemer"] != pNomEmer)
                {
                    Descripcion = "SE MODIFICA NOMBRE DE CONTACTO EMERGENCIA TRABAJADOR N° " + pContrato;
                    Antiguo = Listado["nomemer"].ToString();
                    Nuevo = pNomEmer;
                    logRegistro log = new logRegistro(User.getUser(), Descripcion, "TRABAJADOR", Antiguo, Nuevo, "MODIFICAR");
                    log.Log();
                }
                if ((string)Listado["talla"] != pTalla)
                {
                    Descripcion = "SE MODIFICA TALLA ROPA TRABAJADOR N° " + pContrato;
                    Antiguo = Listado["talla"].ToString();
                    Nuevo = pTalla;
                    logRegistro log = new logRegistro(User.getUser(), Descripcion, "TRABAJADOR", Antiguo, Nuevo, "MODIFICAR");
                    log.Log();
                }
                if ((string)Listado["calzado"] != pCalzado)
                {
                    Descripcion = "SE MODIFICA NUMERO DE CALZADO TRABAJADOR N° " + pContrato;
                    Antiguo = Listado["calzado"].ToString();
                    Nuevo = pTalla;
                    logRegistro log = new logRegistro(User.getUser(), Descripcion, "TRABAJADOR", Antiguo, Nuevo, "MODIFICAR");
                    log.Log();
                }
                if (Convert.ToInt32(Listado["horario"]) != pHorario)
                {
                    Descripcion = "SE MODIFICA HORARIO TRABAJADOR N° " + pContrato;
                    Antiguo = Convert.ToInt32(Listado["horario"]) + "";
                    Nuevo = pHorario.ToString();
                    logRegistro log = new logRegistro(User.getUser(), Descripcion, "TRABAJADOR", Antiguo, Nuevo, "MODIFICAR");
                    log.Log();
                }
                if (Convert.ToInt32(Listado["jornada"]) != pJornada)
                {
                    Descripcion = "SE MODIFICA JORNADA LABORAL TRABAJADOR N° " + pContrato;
                    Antiguo = Convert.ToInt32(Listado["jornada"]) + "";
                    Nuevo = pJornada.ToString();
                    logRegistro log = new logRegistro(User.getUser(), Descripcion, "TRABAJADOR", Antiguo, Nuevo, "MODIFICAR");
                    log.Log();
                }
                if (Convert.ToInt32(Listado["sindicato"]) != pSindicato)
                {
                    Descripcion = "SE MODIFICA CAMPO SINDICATO TRABAJADOR N° " + pContrato;
                    Antiguo = Convert.ToInt32(Listado["sindicato"]) + "";
                    Nuevo = pSindicato.ToString();
                    logRegistro log = new logRegistro(User.getUser(), Descripcion, "TRABAJADOR", Antiguo, Nuevo, "MODIFICAR");
                    log.Log();
                }
                if (Convert.ToInt32(Listado["comuna"]) != pComuna)
                {
                    Descripcion = "SE MODIFICA CAMPO COMUNA TRABAJADOR N° " + pContrato;
                    Antiguo = Convert.ToInt32(Listado["comuna"]) + "";
                    Nuevo = pComuna.ToString();
                    logRegistro log = new logRegistro(User.getUser(), Descripcion, "TRABAJADOR", Antiguo, Nuevo, "MODIFICAR");
                    log.Log();
                }
            }
        }

        #endregion

        #endregion

        #region "Tratamiento Imagen"       

        //OBTENER EXTENSION IMAGEN
        static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageDecoders();

            ImageCodecInfo encoder = (from enc in encoders where enc.MimeType == mimeType select enc).First();
            return encoder;

        }

        static string GetMimeType(string ext)
        {
            switch (ext.ToLower())
            {
                case ".bpm":
                case ".dib":
                case ".rle":
                    return "ima/bmp";

                case ".jpg":
                case ".jpeg":
                case ".jpe":
                case ".fif":
                    return "image/jpeg";

                case "gif":
                    return "image/gif";

                case ".tif":
                case ".tiff":
                    return "image/tiff";

                case "png":
                    return "image/png";
                default:
                    return "image/jpeg";
            }
        }

        /** ***************************************/
        /* METODOS PARA COMPRESION DE IMAGEN
        * **************************************/
        static void ComprimirImagen(string inputFile, string OutputFile, long compression)
        {
            /*agregar ancho y alto a la imagen*/
            Bitmap b = new Bitmap(inputFile);
            Bitmap resize = new Bitmap(b, new Size(450, 600));

            //@ SI QUEREMOS OBTENER EL ANCHO Y EL LARGO DEL ELEMENTO 
            //Bitmap resize = new Bitmap(b, new Size(b.Width, b.Height));
            EncoderParameters eps = new EncoderParameters(1);

            eps.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, compression);
            string mimetype = GetMimeType(new System.IO.FileInfo(inputFile).Extension);
            ImageCodecInfo ici = GetEncoderInfo(mimetype);

            resize.Save(OutputFile, ici, eps);

            //guardar imagen generada en variable imgDB           

            //liberamos de memoria
            b.Dispose();
            resize.Dispose();
        }  
        #endregion
        
        //PARA CAMBIAR LA APARIENCIA DE LOS PANEL CONTROL
        #region "APARIENCIA PANELES"


        private void txtNombre_Enter(object sender, EventArgs e)
        {
            fnAparienciaPersonal();            
            
        }

        private void txtRut_Enter(object sender, EventArgs e)
        {
            fnAparienciaAsociado();
        }

        private void txtPasaporte_Enter(object sender, EventArgs e)
        {
            fnAparienciaPersonal();
        }

        private void txtapePaterno_Enter(object sender, EventArgs e)
        {
            fnAparienciaPersonal();
            
        }

        private void txtApeMat_Enter(object sender, EventArgs e)
        {
            fnAparienciaPersonal();
        }

        private void dtFecNac_Enter(object sender, EventArgs e)
        {
            fnAparienciaPersonal();
        }

        private void txteCivil_Enter(object sender, EventArgs e)
        {
            fnAparienciaPersonal();
        }

        private void txtFono_Enter(object sender, EventArgs e)
        {
            fnAparienciaPersonal();
        }

        private void txtDirec_Enter(object sender, EventArgs e)
        {
            fnAparienciaPersonal();
        }

        private void txtCiudad_Enter(object sender, EventArgs e)
        {
            fnAparienciaPersonal();
        }

        private void txtNacion_Enter(object sender, EventArgs e)
        {
            fnAparienciaPersonal();
        }

        private void txtEstado_Enter(object sender, EventArgs e)
        {
            fnAparienciaEstado();
        }

        //APARIENCIA PARA PANEL PREVISION
        private void txtAfp_Enter(object sender, EventArgs e)
        {
            fnAparienciaPrevision();
        }

        private void txtSalud_Enter(object sender, EventArgs e)
        {
            fnAparienciaPrevision();
        }

        private void dtSegCes_Enter(object sender, EventArgs e)
        {
            fnAparienciaPrevision();
        }

        private void txtTramo_Enter(object sender, EventArgs e)
        {
            fnAparienciaPrevision();
        }

        private void txtCajaPrevision_Enter(object sender, EventArgs e)
        {
            fnAparienciaPrevision();
        }

        private void txtjubilado_Enter(object sender, EventArgs e)
        {
            fnAparienciaPrevision();
        }
        //APARIENCIA PARA PANEL LABORALES
        private void txtcontrato_Enter(object sender, EventArgs e)
        {
            fnAparienciaAsociado();
        }

        private void txtTipoCont_Enter(object sender, EventArgs e)
        {
            fnAparienciaLaboral();       
            
        }

        private void txtCargo_Enter(object sender, EventArgs e)
        {
            fnAparienciaLaboral();
        }

        private void txtRegsal_Enter(object sender, EventArgs e)
        {
            fnAparienciaLaboral();
        }

        private void dtProg_Enter(object sender, EventArgs e)
        {
            fnAparienciaLaboral();
        }

        private void spProg_Enter(object sender, EventArgs e)
        {
            fnAparienciaLaboral();
        }

        private void dtSal_Enter(object sender, EventArgs e)
        {
            fnAparienciaLaboral();
        }

        private void txtArea_Enter(object sender, EventArgs e)
        {
            fnAparienciaLaboral();
        }

        private void txtRegimen_Enter(object sender, EventArgs e)
        {
            fnAparienciaPrevision();
        }

        private void txtccosto_Enter(object sender, EventArgs e)
        {
            fnAparienciaLaboral();
        }

        private void dtVac_Enter(object sender, EventArgs e)
        {
            fnAparienciaLaboral();
        }

        private void dtingr_Enter(object sender, EventArgs e)
        {
            fnAparienciaLaboral();

            dtSegCes.EditValue = dtingr.EditValue;
        }

        //APARIENCIA PARA PANEL PAGOS
        private void txtfPago_Enter(object sender, EventArgs e)
        {
            fnAparienciaPagos();
        }

        private void txtBanc_Enter(object sender, EventArgs e)
        {
            fnAparienciaPagos();
        }

        private void txtcuenta_Enter(object sender, EventArgs e)
        {
            fnAparienciaPagos();
            

            if (txtTipoCuenta.EditValue.ToString() == "3")
            {
                //SI SELECCIONA CUENTA RUT CARGAMOS EL RUT DEL TRABAJADOR
                if (txtRut.Text != "")
                {
                    string rut = fnSistema.fnExtraerCaracteres(txtRut.Text);
                    //txtcuenta.Text = rut.Substring(0, rut.Length-1);
                }
                else
                {
                    txtcuenta.Text = "0";
                }
            }
        }

        //METODO PARA APARIENCIA PANEL DATOS PERSONALES
        private void fnAparienciaPersonal()
        {
            //panelPersonales.Appearance.BackColor = Color.FromArgb(183,181, 180);
            groupPersonales.BackColor = Color.FromArgb(216, 217, 223);
            groupPrevision.BackColor = Color.FromArgb(240, 240, 240);
            groupLaborales.BackColor = Color.FromArgb(240,240,240);
            groupMediosPago.BackColor = Color.FromArgb(240, 240, 240);
            groupFoto.BackColor = Color.FromArgb(240, 240, 240);
            groupAsociado.BackColor = Color.FromArgb(240, 240, 240);
            groupStatus.BackColor = Color.FromArgb(240, 240, 240);
            groupButtons.BackColor = Color.FromArgb(240, 240, 240);
            groupFiltros.BackColor = Color.FromArgb(240, 240, 240);
        }
        //METODO PARA APARIENCIA PANEL DATOS PREVISION
        private void fnAparienciaPrevision()
        {
            groupPersonales.BackColor = Color.FromArgb(240,240,240);
            groupPrevision.BackColor = Color.FromArgb(216, 217, 223);
            groupLaborales.BackColor = Color.FromArgb(240, 240, 240);
            groupMediosPago.BackColor = Color.FromArgb(240, 240, 240);
            groupFoto.BackColor = Color.FromArgb(240, 240, 240);
            groupAsociado.BackColor = Color.FromArgb(240, 240, 240);
            groupStatus.BackColor = Color.FromArgb(240, 240, 240);
            groupButtons.BackColor = Color.FromArgb(240, 240, 240);
            groupFiltros.BackColor = Color.FromArgb(240, 240, 240);
        }

        //METODO PARA APARIENCIA PANEL DATOS LABORALES
        private void fnAparienciaLaboral()
        {
            groupPersonales.BackColor = Color.FromArgb(240, 240, 240);
            groupPrevision.BackColor = Color.FromArgb(240, 240, 240);
            groupMediosPago.BackColor = Color.FromArgb(240, 240, 240);
            groupLaborales.BackColor = Color.FromArgb(216, 217, 223);
            groupFoto.BackColor = Color.FromArgb(240, 240, 240);
            groupAsociado.BackColor = Color.FromArgb(240, 240, 240);
            groupStatus.BackColor = Color.FromArgb(240, 240, 240);
            groupButtons.BackColor = Color.FromArgb(240, 240, 240);
            groupFiltros.BackColor = Color.FromArgb(240, 240, 240);
        }

        //METODO PARA APARIENCIA PANEL PAGO
        private void fnAparienciaPagos()
        {
            groupPersonales.BackColor = Color.FromArgb(240, 240, 240);
            groupPrevision.BackColor = Color.FromArgb(240, 240, 240);
            groupLaborales.BackColor = Color.FromArgb(240, 240, 240);
            groupMediosPago.BackColor = Color.FromArgb(216, 217, 223);
            groupFoto.BackColor = Color.FromArgb(240, 240, 240);
            groupAsociado.BackColor = Color.FromArgb(240, 240, 240);
            groupStatus.BackColor = Color.FromArgb(240, 240, 240);
            groupButtons.BackColor = Color.FromArgb(240, 240, 240);
            groupFiltros.BackColor = Color.FromArgb(240, 240, 240);
        }

        //METODO PARA APRIENCIA PANEL PERFIL EMPLEADO
        private void fnAparienciaPerfil()
        {
            groupPersonales.BackColor = Color.FromArgb(240, 240, 240);
            groupPrevision.BackColor = Color.FromArgb(240, 240, 240);
            groupLaborales.BackColor = Color.FromArgb(240, 240, 240);
            groupMediosPago.BackColor = Color.FromArgb(240, 240, 240);
            groupFoto.BackColor = Color.FromArgb(216, 217, 223);
            groupAsociado.BackColor = Color.FromArgb(240,240,240);
            groupStatus.BackColor = Color.FromArgb(240, 240, 240);
            groupButtons.BackColor = Color.FromArgb(240, 240, 240);
            groupFiltros.BackColor = Color.FromArgb(240, 240, 240);
        }

        //METODO PARA APARIENCIA PANEL REGISTRO ASOCIADO
        private void fnAparienciaAsociado()
        {
            groupPersonales.BackColor = Color.FromArgb(240, 240, 240);
            groupPrevision.BackColor = Color.FromArgb(240, 240, 240);
            groupLaborales.BackColor = Color.FromArgb(240, 240, 240);
            groupMediosPago.BackColor = Color.FromArgb(240, 240, 240);
            groupFoto.BackColor = Color.FromArgb(240, 240, 240);
            groupAsociado.BackColor = Color.FromArgb(216, 217, 223);
            groupStatus.BackColor = Color.FromArgb(240, 240, 240);
            groupButtons.BackColor = Color.FromArgb(240, 240, 240);
            groupFiltros.BackColor = Color.FromArgb(240, 240, 240);
        }

        //PANEL ESTADO
        private void fnAparienciaEstado()
        {
            groupPersonales.BackColor = Color.FromArgb(240, 240, 240);
            groupPrevision.BackColor = Color.FromArgb(240, 240, 240);
            groupLaborales.BackColor = Color.FromArgb(240, 240, 240);
            groupMediosPago.BackColor = Color.FromArgb(240, 240, 240);
            groupFoto.BackColor = Color.FromArgb(240, 240, 240);
            groupAsociado.BackColor = Color.FromArgb(240, 240, 240);
            groupStatus.BackColor = Color.FromArgb(216, 217, 223);
            groupButtons.BackColor = Color.FromArgb(240,240,240);
            groupFiltros.BackColor = Color.FromArgb(240, 240, 240);
        }

        private void fnAparienciaButtons()
        {
            groupPersonales.BackColor = Color.FromArgb(240, 240, 240);
            groupPrevision.BackColor = Color.FromArgb(240, 240, 240);
            groupLaborales.BackColor = Color.FromArgb(240, 240, 240);
            groupMediosPago.BackColor = Color.FromArgb(240, 240, 240);
            groupFoto.BackColor = Color.FromArgb(240, 240, 240);
            groupAsociado.BackColor = Color.FromArgb(240, 240, 240);
            groupStatus.BackColor = Color.FromArgb(240, 240, 240);
            groupButtons.BackColor = Color.FromArgb(216, 217, 223);
            groupFiltros.BackColor = Color.FromArgb(240, 240, 240);
        }

        private void fnAparienciaFiltros()
        {
            groupPersonales.BackColor = Color.FromArgb(240, 240, 240);
            groupPrevision.BackColor = Color.FromArgb(240, 240, 240);
            groupLaborales.BackColor = Color.FromArgb(240, 240, 240);
            groupMediosPago.BackColor = Color.FromArgb(240, 240, 240);
            groupFoto.BackColor = Color.FromArgb(240, 240, 240);
            groupAsociado.BackColor = Color.FromArgb(240, 240, 240);
            groupStatus.BackColor = Color.FromArgb(240, 240, 240);
            groupButtons.BackColor = Color.FromArgb(240, 240, 240);
            groupFiltros.BackColor = Color.FromArgb(216, 217, 223);
        }

        private void fnAparienciaClean()
        {
            groupPersonales.BackColor = Color.FromArgb(240, 240, 240);
            groupPrevision.BackColor = Color.FromArgb(240, 240, 240);
            groupLaborales.BackColor = Color.FromArgb(240, 240, 240);
            groupMediosPago.BackColor = Color.FromArgb(240, 240, 240);
            groupFoto.BackColor = Color.FromArgb(240, 240, 240);
            groupAsociado.BackColor = Color.FromArgb(240, 240, 240);
            groupStatus.BackColor = Color.FromArgb(240, 240, 240);
            groupButtons.BackColor = Color.FromArgb(240, 240, 240);
            groupFiltros.BackColor = Color.FromArgb(240, 240, 240);
        }

        private void btnImagen_Enter(object sender, EventArgs e)
        {
            fnAparienciaPerfil();
        }

        private void btnRecortar_Enter(object sender, EventArgs e)
        {
            fnAparienciaPerfil();
        }

        private void btnReset_Enter(object sender, EventArgs e)
        {
            fnAparienciaPerfil();
        }

        private void txtCausal_Enter(object sender, EventArgs e)
        {
            fnAparienciaLaboral();
        }
        #endregion    

        #region "DATOS CALCULO"

        //VERIFICAR SI EXISTEN DATOS DE CALCULO
        private bool ExistenDatosDeCalculo(string contrato)
        {
            bool existe = false;
            string sql = "SELECT contrato FROM DATOSCALCULO WHERE contrato = @contrato";
            SqlCommand cmd;
            SqlDataReader rd;
            SqlConnection cn;
            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@contrato", contrato));

                            rd = cmd.ExecuteReader();
                            if (rd.HasRows)
                            {
                                //EXISTE REGISTRO
                                existe = true;
                            }
                            else
                            {
                                existe = false;
                            }
                        }
                        cmd.Dispose();
                        rd.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return existe;
        }
        #endregion

        #region "CARGAS FAMILIARES"
        //VERIFICAR SI EL CONTRATO TIENE CARGAS FAMILIARES ASOCIADAS
        private bool tieneFamiliares(string contrato)
        {
            bool tiene = false;
            string sql = "SELECT rutCarga FROM CARGAFAMILIAR WHERE contrato=@contrato";
            SqlCommand cmd;
            SqlDataReader rd;
            SqlConnection cn;
            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@contrato", contrato));

                            rd = cmd.ExecuteReader();
                            if (rd.HasRows)
                            {
                                tiene = true;
                            }
                            else
                            {
                                tiene = false;
                            }
                        }
                        cmd.Dispose();
                        rd.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return tiene;
        }
        #endregion

        #region "VACACIONES"
        //VERIFICAR SI EXISTEN LOS REGISTROS
        private bool ExisteVacaciones(string contrato)
        {
            bool existe = false;
            string sql = "SELECT * FROM vacacion WHERE contrato=@contrato";
            SqlCommand cmd;
            SqlDataReader rd;
            SqlConnection cn;
            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@contrato", contrato));
                            rd = cmd.ExecuteReader();
                            if (rd.HasRows)
                            {
                                existe = true;
                            }
                            else
                            {
                                existe = false;
                            }
                        }
                        cmd.Dispose();
                        rd.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }


            return existe;
        }
        private bool ExisteDetalleVacaciones(string contrato)
        {
            bool existe = false;
            string sql = "SELECT salida FROM VACACIONDETALLE WHERE contrato=@contrato";
            SqlCommand cmd;
            SqlDataReader rd;
            SqlConnection cn;
            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@contrato", contrato));
                            rd = cmd.ExecuteReader();
                            if (rd.HasRows)
                            {
                                existe = true;
                            }
                            else
                            {
                                existe = false;
                            }
                        }
                        cmd.Dispose();
                        rd.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }


            return existe;
        }
        #endregion

        private void frmEmpleado_Shown(object sender, EventArgs e)
        {
            txtRut.Focus();
        }

        private void btnItemMes_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            fnAparienciaButtons();

            //VER SI EL USUARIO ESTA BLOQUEADO 
            if (User.Bloqueado())
            { XtraMessageBox.Show("No puedes realizar modificaciones", "Información", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmitemtrabajador") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (Calculo.ViewStatus())
            { XtraMessageBox.Show("Hay un proceso de calculo en curso, por favor espere un momento", $"Proceso activo por {User.getUser()}", MessageBoxButtons.OK, MessageBoxIcon.Warning); Close(); return; }

            if (FichaHistorica)
            { XtraMessageBox.Show("No puedes editar los items de una ficha histórica", "Histórica", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

            if (StatusTrabajador == 0)
            { XtraMessageBox.Show("No puedes editar una ficha con estado inactiva", "Ficha inactiva", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
            

            bool indiceSeteado = false;
            indiceSeteado = ExistenIndicesMensuales(Calculo.PeriodoObservado);
            if (indiceSeteado == false)
            {
                //MOSTRAR EL FORMULARIO ITEM HISTORICO
                string rut = "";
                string contrato = "";
                int periodo = 0;
                if (txtRut.Text != "") rut = txtRut.Text;
                if (txtcontrato.Text != "") contrato = txtcontrato.Text;
                if (PeriodoEmpleado != 0) periodo = PeriodoEmpleado;

                //ABRIMOS FORMULARIO PASANDO COMO PARAMETRO AL CONSTRUCTOR EL RUT Y EL CONTRATO DEL 
                //TRABAJADOR SELECCIONADO EN FORMULARIO               

                frmitemTrabajador frmhis = new frmitemTrabajador(contrato, rut, periodo);
                frmhis.StartPosition = FormStartPosition.CenterScreen;
                frmhis.ShowDialog();
            }
            else
            {
                XtraMessageBox.Show("Hemos detectado que no se han registrado los indices mensuales, porfavor ingresa los indices para el periodo " + Calculo.PeriodoObservado, "Informacion Importante", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
        }

        private void txtRegimen_EditValueChanged(object sender, EventArgs e)
        {
            //DESHABILITAR COMBOS DE ACUERDO A LA SELECCION QUE SE HAGA EN EL COMBO REGIMEN
            if (txtRegimen.EditValue.ToString() == "1")
            {
                //AFP-SALUD
                //HABILITAMOS SOLO EL COMBO AFP Y SALUD
                if (txtSalud.Enabled == false)
                {
                    txtSalud.Enabled = true;
                    txtSalud.ItemIndex = 0;
                }

                if (txtAfp.Enabled == false)
                {
                    txtAfp.Enabled = true;
                    txtAfp.ItemIndex = 0;
                }                    

                txtCajaPrevision.Enabled = false;
                txtCajaPrevision.EditValue = 0;
            }
            else if (txtRegimen.EditValue.ToString() == "2")
            {
                //SOLO SALUD
                //SOLO HABILITAMOS COMBO Salud
                if (txtSalud.Enabled == false)
                {
                    txtSalud.Enabled = true;
                    txtSalud.ItemIndex = 0;
                }                    

                txtCajaPrevision.Enabled = false;
                txtCajaPrevision.EditValue = 0;
                txtAfp.Enabled = true;
                txtAfp.EditValue = 0;

            }
            else if (txtRegimen.EditValue.ToString() == "3")
            {
                //SOLO CAJA
                //SOLO HABILITAMOS COMBO CAJA PREVISION
                txtSalud.Enabled = false;
                txtSalud.EditValue = 0;
                txtAfp.Enabled = false;
                txtAfp.EditValue = 0;

                if (txtCajaPrevision.Enabled == false)
                {
                    txtCajaPrevision.Enabled = true;
                    txtCajaPrevision.ItemIndex = 0;
                }
                    
            }
            else if (txtRegimen.EditValue.ToString() == "4")
            {
                //REGIMEN ANTIGUO
                //R.A. -SALUD
                txtAfp.Enabled = true;
                txtAfp.EditValue = 0;
                txtCajaPrevision.Enabled = true;
                txtCajaPrevision.EditValue = 0;

                if (txtSalud.Enabled == false)
                {
                    txtSalud.Enabled = true;
                    txtSalud.ItemIndex = 0;
                }                    
            }
            else if (txtRegimen.EditValue.ToString() == "5")
            {
                //REGIMEN ANTIGUO
                //R.A - SOLO SALUD
                txtAfp.Enabled = true;
                txtAfp.EditValue = 0;
                txtCajaPrevision.Enabled = false;
                txtCajaPrevision.EditValue = 0;
                if (txtSalud.Enabled == false)
                {
                    txtSalud.Enabled = true;
                    txtSalud.ItemIndex = 0;
                }
            }
            else if (txtRegimen.EditValue.ToString() == "6")
            {
                //NO APLICA
                //DESHABILITAMOS TODO
                txtSalud.Enabled = false;
                txtSalud.EditValue = 0;
                txtAfp.Enabled = false;
                txtAfp.EditValue = 0;
                txtCajaPrevision.Enabled = false;
                txtCajaPrevision.EditValue = 0;
            }
        }

        private void txtBanc_EditValueChanged(object sender, EventArgs e)
        {
          
        }

        private void txtTipoCuenta_EditValueChanged(object sender, EventArgs e)
        {
           
        }

        private void txtcontrato_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                ValidaContrato();
            }
        }

        private void txtcontrato_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if (char.IsControl(e.KeyChar) || e.KeyChar == (char)45)
            //{
            //    e.Handled = false;
            //}
            //else if (char.IsDigit(e.KeyChar))
            //{
            //    e.Handled = false;
            //}
            //else if (char.IsLetter(e.KeyChar))
            //{
            //    e.Handled = false;
            //}
            //else
            //{
            //    e.Handled = true;
            //}
        }

        private void txtNombre_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (char.IsLetter(e.KeyChar))
            {
                e.Handled = false;
            }          
            else if (e.KeyChar == (char)32)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
            
        }

        private void txtDirec_KeyPress(object sender, KeyPressEventArgs e)
        {
          
        }

        private void txtTipoCont_EditValueChanged(object sender, EventArgs e)
        {
            //SI EL TIPO DE CONTRATO ES INDEFINIDO SE DEBE COLOCAR LA FECHA 01-01-3000
            if (txtTipoCont.EditValue.ToString() == "0")
            {
                dtSal.EditValue = DateTime.Parse("3000-01-01");
            }           
        }

        private void dtingr_EditValueChanged(object sender, EventArgs e)
        {
            //LA FECHA QUE SE SELECCIONE ACA SE DEBE COLOCAR EN EL COMBO SEGURO CESANTIA
            dtSegCes.EditValue = dtingr.EditValue;

            //LA FECHA QUE SE SELECCIONE ACA DEBE SER LA MISMA QUE SE SELECCIONE EN FECHA VACACIONES
            dtVac.EditValue = dtingr.EditValue;

            //LA FECHA DE PROGRESIVO ES LA MISMA QUE LA FECHA QUE SE SELECCIONE ACA
            dtProg.EditValue = dtingr.EditValue;
        }

        private void dtSegCes_EditValueChanged(object sender, EventArgs e)
        {
            //GUARDAR EL MISMO VALOR QUE TIENE EL COMBO FECHA INICIO CONTRATO
            //dtSegCes.EditValue = dtingr.EditValue;
        }

        private void txtNacion_DoubleClick(object sender, EventArgs e)
        {
            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmnacion") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);return; }          

            //MOSTRAR FORMULARIO NACIONALIDAD AL HACER DOBLE CLICK EN COMBO NACION (FORMULARIO TABLAS)
            frmTablas nacionalidad = new frmTablas("nacionalidad");
            nacionalidad.Text = "NACIONALIDAD";
            nacionalidad.opener = this;
            nacionalidad.ShowDialog();
        }

        private void txtArea_DoubleClick(object sender, EventArgs e)
        {
            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmarea") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop) ;return; }

            //MOSTRAR FORMULARIO AREA AL HACER DOBLE CLICK EN COMBO AREA (FORMULARIO TABLAS)
            frmTablas area = new frmTablas("area");
            area.Text = "AREA";
            area.opener = this;
            area.ShowDialog();          
        }

        private void txtCargo_DoubleClick(object sender, EventArgs e)
        {
            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmcargo") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion"); return; }

                //MOSTRAR FORMULARIO CARGO AL HACER DOBLE CLICK EN COMBO AREA (FORMULARIO TABLAS)
                frmTablas cargo = new frmTablas("cargo");
                cargo.Text = "CARGO";
                cargo.opener = this;
                cargo.ShowDialog();

            
        }

        private void txtccosto_DoubleClick(object sender, EventArgs e)
        {
            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmccosto") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                //MOSTRAR FORMULARIO CENTRO COSTO AL HACER DOBLE CLICK EN COMBO CENTRO COSTO (FORMULARIO TABLAS)
                frmTablas costo = new frmTablas("centrocosto");
                costo.Text = "CENTRO COSTO";
                costo.opener = this;
                costo.ShowDialog();
         
        }

        private void txtfPago_DoubleClick(object sender, EventArgs e)
        {
            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmformapago") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            //MOSTRAR FORMULARIO FORMA PAGO AL HACER DOBLE CLICK EN COMBO FORMA DE PAGO (FORMULARIO TABLAS)
            frmTablas pago = new frmTablas("formapago");
            pago.Text = "PAGO";
            pago.opener = this;
            pago.ShowDialog();
     
        }

        private void txtBanc_DoubleClick(object sender, EventArgs e)
        {
            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmbanco") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);return; }

                //MOSTRAR FORMULARIO BANCO AL HACER DOBLE CLICK EN COMBO BANCO (FORMULARIO TABLAS)
                frmTablas banco = new frmTablas("banco");
                banco.Text = "BANCO";
                banco.opener = this;
                banco.ShowDialog();

        }

        private void txtSalud_DoubleClick(object sender, EventArgs e)
        {
            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmsalud") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                //MOSTRAR FORMULARIO ISAPRE AL HACER DOBLE CLICK EN COMBO SALUD (FORMULARIO LEYES SOCIALES)
                frmLeyesSociales salud = new frmLeyesSociales("salud");
                salud.Text = "SALUD";
                salud.opener = this;
                salud.ShowDialog();

        }

        private void txtAfp_DoubleClick(object sender, EventArgs e)
        {
            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmafp") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);return; }

                //MOSTRAR FORMULARIO AFP AL HACER DOBLE CLICK EN COMBO AFP (FORMULARIO LEYES SOCIALES)
                frmLeyesSociales afp = new frmLeyesSociales("afp");
                afp.Text = "AFP";
                afp.opener = this;
                afp.ShowDialog();

        }

        private void txtCajaPrevision_DoubleClick(object sender, EventArgs e)
        {
            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmcaja") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);return; }

                //MOSTRAR FORMULARIO AFP AL HACER DOBLE CLICK EN COMBO CAJA PREVISION (FORMULARIO LEYES SOCIALES)
                frmLeyesSociales caja = new frmLeyesSociales("caja");
                caja.Text = "CAJA DE COMPENSACION";
                caja.opener = this;
                caja.ShowDialog();
     
        }

        private void dtFecNac_EditValueChanged(object sender, EventArgs e)
        {            
                
        }

        private void txtClase_DoubleClick(object sender, EventArgs e)
        {
            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmclase") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);return; }

                //ABRIR FORMULARIO CLASE
                string rut = "", contrato = "";
                if (txtcontrato.Text != "" && txtRut.Text != "")
                {
                    rut = txtRut.Text;
                    contrato = txtcontrato.Text;

                    frmClase clas = new frmClase(rut, contrato);
                    clas.opener = this;
                    clas.StartPosition = FormStartPosition.CenterScreen;
                    clas.ShowDialog();
                }
                          
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESIION
            Sesion.NuevaActividad();

            fnAparienciaClean();

            //SI EL PERIODO DEL REGISTRO ES DISTINTO AL PERIODO DE TRABAJO NO PODEMOS ELIMINAR
            if (FichaHistorica)
            { XtraMessageBox.Show("No puedes eliminar este registro", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            bool correcto = false, PrimerContrato = false;
            bool privado = User.ShowPrivadas();
            string ContratoEmpleado = "";
            if (txtcontrato.Text == "")
            { XtraMessageBox.Show("Registro no válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
            if (txtcontrato.ReadOnly == false)
            { XtraMessageBox.Show("Registro no válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
            if (txtRut.Text == "")
            { XtraMessageBox.Show("Registro no válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            ContratoEmpleado = txtcontrato.Text;

            //SI LA PERSONA TIENE HISTORICO NO PODEMOS ELIMINAR
            if (Persona.TieneHistorico(fnSistema.fnExtraerCaracteres(txtRut.Text), Calculo.PeriodoObservado))
            {
                XtraMessageBox.Show("Este trabajador no se puede eliminar porque tiene informacion historica asociada", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);

                //Aunque tenga información historica el rut, verificar si el numero de contrato
                //es el primero y este contrato en particular no tiene histórico.

                PrimerContrato = Persona.PrimerContratoRut(fnSistema.fnExtraerCaracteres(txtRut.Text), txtcontrato.Text, Calculo.PeriodoObservado);
                if (PrimerContrato)
                {
                    DialogResult adv = XtraMessageBox.Show("Hemos detectado que este es el primer contrato para este rut.\n¿Desea eliminar solo esta ficha?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (adv == DialogResult.Yes)
                    {
                        if (Persona.TieneHistoricoContrato(fnSistema.fnExtraerCaracteres(txtRut.Text), txtcontrato.Text, Calculo.PeriodoObservado))
                        {
                            XtraMessageBox.Show("Registro eliminado correctamente", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            RefreshData(privado);

                            logRegistro log = new logRegistro(User.getUser(), "SE ELIMINA TRABAJADOR CON CONTRATO " + ContratoEmpleado, "TRABAJADOR", "N° " + ContratoEmpleado, "0", "ELIMINAR");
                            log.Log();
                        }
                        else
                        {
                            XtraMessageBox.Show("No se pudo eliminar registro", "Información", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }

                return;
            }

            //METODO QUE ELIMINA TODA LA DATA DEL TRABAJADOR 
            correcto = Transaccion(txtcontrato.Text, fnSistema.fnExtraerCaracteres(txtRut.Text));
            //SI CORRECTO ES TRUE ES PORQUE LA DATA DEL TRABAJADOR SE ELIMINÓ CORRECTAMENTE!!
            if (correcto)
            {
               XtraMessageBox.Show("Trabajador Eliminado correctamente", "Eliminar", MessageBoxButtons.OK, MessageBoxIcon.Information);

                //MOSTRAR EL PRIMER ELEMENTO
                //RESETEAR POSICION 
                //posicion = 0;
                //VERIFICAR SI HAY REGISTROS
                //if(Persona.ExistenRegistros(Calculo.PeriodoObservado))
                //fnFirst();

                RefreshData(privado);        

                //GUARDAR EVENTO EN LOG
                logRegistro log = new logRegistro(User.getUser(), "SE ELIMINA TRABAJADOR CON CONTRATO " + ContratoEmpleado , "TRABAJADOR", "N° " + ContratoEmpleado, "0", "ELIMINAR");
                log.Log();
                }
            else
            {
               XtraMessageBox.Show("Se ha producido un error al intentar eliminar trabajador", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }          
        
        }

        /// <summary>
        /// Recarga la informacion de los trabajadores cuando se elimina un registro
        /// </summary>
        /// <param name="pPrivado"></param>
        private void RefreshData(bool pPrivado)
        {
            if (Calculo.GetTotalRegistros(pPrivado) > 0)
            {
                Navega = new NavegaTrabajador(radioOrden.Properties.Items[radioOrden.SelectedIndex].Description, comboVista);
                Navega.Primer();
                fnNavDatos(Navega.Listado[Navega.Posicion].Contrato, Navega.Listado[Navega.Posicion].PeriodoPersona);
                User.UltimoRegistroVisto(Navega.Listado[Navega.Posicion].Contrato + ";" + Navega.Listado[Navega.Posicion].PeriodoPersona, User.getUser());
            }
            else
            {
                fnLimpiarCampos();
                btnEliminar.Enabled = false;
                btnnext.Enabled = false;
                btnLast.Enabled = false;
                btnPrev.Enabled = false;
                btnFirst.Enabled = false;
                btnBuscar.Enabled = false;
                //Navega = new NavegaTrabajador(radioOrden.Properties.Items[radioOrden.SelectedIndex].Description, comboVista);
                Navega.Universo = 0;
                User.CleanLastView();
            }
        }

        private void txtTipoCuenta_DoubleClick(object sender, EventArgs e)
        {
            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmtipocuenta") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            //MOSTRAR FORMULARIO AREA AL HACER DOBLE CLICK EN COMBO AREA (FORMULARIO TABLAS)
            frmTablas causal = new frmTablas("tipocuenta");
            causal.Text = "Tipo Cuenta";
            causal.opener = this;
            causal.StartPosition = FormStartPosition.CenterParent;
            causal.ShowDialog();
        }

        private void btnAusentismo_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESSION
            Sesion.NuevaActividad();

            fnAparienciaButtons();

            Cursor.Current = Cursors.WaitCursor;

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmausentismo") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            string contrato = "";
            int periodo = 0;
            if (txtcontrato.Text != "")
            {
                contrato = txtcontrato.Text;
            }
            if (PeriodoEmpleado != 0) periodo = PeriodoEmpleado;
            frmAusentismo aus = new frmAusentismo(contrato, PeriodoEmpleado);
            aus.StartPosition = FormStartPosition.CenterScreen;
            aus.ShowDialog();
            
        }

        private void txtCausal_DoubleClick(object sender, EventArgs e)
        {
            /*if (objeto.ValidaAcceso(User.GetUserGroup(), "frmcausal") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilzar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            frmCausalTermino cau = new frmCausalTermino();
            cau.opener = this;
            cau.ShowDialog();*/

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmcausal") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            //MOSTRAR FORMULARIO AREA AL HACER DOBLE CLICK EN COMBO AREA (FORMULARIO TABLAS)
            frmTablas causal = new frmTablas("causal");
            causal.Text = "CAUSAL";
            causal.opener = this;
            causal.ShowDialog();

        }

       

        private void btnReporte_Click(object sender, EventArgs e)
        {
           
            
        }

        #region "REPORTE"
        //CARGAR PRELISTA
        private void CargarListado()
        {
            List<Data> listado = new List<Data>();
            Data d1 = new Data();
            Data d2 = new Data();

            string sql = "select item, original, calculado " +
                        " from calculo " +
                        " WHERE contrato = @contrato AND anomes = @periodo";

            int tipo = 0, cExentos = 0, cFam = 0, cLeyes = 0, cDesc = 0, cAportes = 0;
            string[] arreglo = new string[2];
            double original = 0, calculado = 0;
            string item = "", cadImpuesto = "", cadAfp = "";
            SqlCommand cmd;
            SqlDataReader rd;

            //OBTENER CADENA PARA IMPUESTO
            cadImpuesto = ImpuestoEmpleado();

            //CADENA PARA AFP
            cadAfp = PorcentajeAfp();

             try
              {
                  if (fnSistema.ConectarSQLServer())
                  {
                      using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                      {
                          cmd.Parameters.Add(new SqlParameter("@periodo", PeriodoEmpleado));
                          cmd.Parameters.Add(new SqlParameter("@contrato", txtcontrato.Text));
                          rd = cmd.ExecuteReader();
                          if (rd.HasRows)
                          {
                            //PRIMER ELEMENTO ES HABERES IMPONIBLES
                            listado.Add(new Data() { Item = "HABERES" });
                            while (rd.Read())
                              {
                                  arreglo = tipoItem((string)rd["item"]);
                                  tipo = Convert.ToInt32(arreglo[0]);
                                  item = arreglo[1];
                                  original = Convert.ToDouble((decimal)rd["original"]);
                                  calculado = Math.Round(Convert.ToDouble((decimal)rd["calculado"]));

                                if (tipo == 1)
                                {
                                    //HABERES IMPONIBLES
                                    listado.Add(new Data() { Item = item.ToLower(), Vo = original + "", Haber = calculado + ""});
                                }
                                else if (tipo == 2)
                                {
                                    if (cExentos == 0)
                                        listado.Add(new Data() {Item = "OTROS HABERES"});

                                    //HABERES EXENTOS
                                    listado.Add(new Data() { Item = item.ToLower(), Vo = original + "", Haber = calculado + "" });

                                    cExentos++;
                                }
                                else if (tipo == 3)
                                {
                                    if (cFam == 0)
                                        listado.Add(new Data() { Item = "ASIGNACIONES FAMILIARES"});

                                    //FAMILIARES
                                    listado.Add(new Data() { Item = item.ToLower(), Vo = original + "", Haber = calculado + "" });

                                    cFam++;
                                }
                                else if (tipo == 4 && (rd["item"].ToString() != "SCEMPRE" && rd["item"].ToString() != "SEGINV"))
                                {
                                    if (cLeyes == 0)
                                        listado.Add(new Data() { Item = "DESCUENTOS LEGALES"});

                                    //DESCUENTOS LEGALES (LEYES SOCIALES)
                                    original = 0;

                                    if (rd["item"].ToString() == "PREVISI")
                                        item = cadAfp;
                                    if (rd["item"].ToString() == "IMPUEST" && calculado != 0)
                                        item = cadImpuesto;
                                    if (rd["item"].ToString() == "IMPUEST" && calculado == 0)
                                        { item = "EXENTO DE IMPUESTO";}
                                    if (rd["item"].ToString() == "SALUD")
                                        item = SaludEmpleado(Convert.ToDouble((decimal)rd["original"]));

                                    listado.Add(new Data() {Item = item.ToLower(), Descuento = calculado + "" });

                                    cLeyes++;
                                }
                                else if (tipo == 5)
                                {
                                    //DESCUENTOS
                                    if (cDesc == 0)
                                        listado.Add(new Data() {Item = "DESCUENTOS"});

                                    listado.Add(new Data() { Item = item.ToLower(), Descuento = calculado + ""});

                                    cDesc++;
                                }
                                else if (tipo == 6)
                                {
                                    //CONTRIBUCIONES
                                    //..
                                }
                                else
                                {
                                    //CEMPRE Y SIS
                                    //listado.Add(new Data() {Item = item.ToLower(), Vo = calculado + "" });
                                    if (cAportes == 0)
                                    { d1.Item = item.ToLower(); d1.Vo = calculado + "";}
                                    if (cAportes == 1)
                                    { d2.Item = item.ToLower(); d2.Vo = calculado + ""; }                                        

                                    cAportes++;
                                }                                 
                              }
                          }
                      }
                      cmd.Dispose();
                      rd.Close();
                      fnSistema.sqlConn.Close();

                    //AGREGAMOS D1 Y D2 A LA LISTA
                    listado.Add(new Data() { Item = "APORTES"});
                    listado.Add(d1);
                    listado.Add(d2);
                        
                  }
              }
              catch (SqlException ex)
              {
                  XtraMessageBox.Show(ex.Message);
              }

            List<string> datosEmpleado = new List<string>();
            datosEmpleado = CabeceraEmpleado(txtcontrato.Text, PeriodoEmpleado);

            //PASAR COMO DATASOURCE A REPORTE
            //RptLiquidacion reporte = new RptLiquidacion();
            //Report externo
            ReportesExternos.rptLiquidacion reporte = new ReportesExternos.rptLiquidacion();
            reporte.DataSource = listado;

            //SETEAR PARAMETROS...
            reporte.Parameters["imagen"].Value = Imagen.GetLogoFromBd();
            reporte.Parameters["imponible"].Value = varSistema.ObtenerValorLista("systimp");
            reporte.Parameters["descuentos"].Value = varSistema.ObtenerValorLista("systdctos");
            reporte.Parameters["haberes"].Value = varSistema.ObtenerValorLista("systhab");
            reporte.Parameters["liquido"].Value = varSistema.ObtenerValorLista("sysliq");
            reporte.Parameters["pago"].Value = varSistema.ObtenerValorLista("syspago");
            reporte.Parameters["dias"].Value = varSistema.ObtenerValorLista("sysdiastr");
            reporte.Parameters["cargo"].Value = datosEmpleado[2];
            reporte.Parameters["nombre"].Value = datosEmpleado[0];
            reporte.Parameters["rut"].Value = datosEmpleado[1];
            reporte.Parameters["ingreso"].Value = datosEmpleado[3];


            reporte.Parameters["imponible"].Visible = false;
            reporte.Parameters["descuentos"].Visible = false;
            reporte.Parameters["haberes"].Visible = false;
            reporte.Parameters["liquido"].Visible = false;
            reporte.Parameters["pago"].Visible = false;
            reporte.Parameters["dias"].Visible = false;
            reporte.Parameters["cargo"].Visible = false;
            reporte.Parameters["nombre"].Visible = false;
            reporte.Parameters["rut"].Visible = false;
            reporte.Parameters["ingreso"].Visible = false;

            fnSistema.ShowDocument(reporte);           
        }

        //OBTENER EL TIPO DE ACUERDO A CODIGO ITEM
        private string[] tipoItem(string item)
        {
            string sql = "SELECT tipo, descripcion FROM item WHERE coditem = @item";
            string[] datos = new string[2];
            SqlCommand cmd2;
            SqlDataReader rd2;
            try
            {
                using (cmd2 = new SqlCommand(sql, fnSistema.sqlConn))
                {
                    //PARAMETROS
                    cmd2.Parameters.Add(new SqlParameter("@item", item));

                    rd2 = cmd2.ExecuteReader();
                    if (rd2.HasRows)
                    {
                        while (rd2.Read())
                        {
                            datos[0] = (int)rd2["tipo"] + "";
                            datos[1] = (string)rd2["descripcion"];
                        }
                    }
                }
                cmd2.Dispose();

            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return datos;
        }

        //PARA AFP (NOMBRE AFP Y PORCENTAJE TOTAL A PAGAR)
        private string PorcentajeAfp()
        {
            string sql = "select porcFondo, porcAdmin, afp.nombre from trabajador " +
                        "INNER JOIN afp ON afp.id = trabajador.afp " +
                        "WHERE contrato = @contrato AND anomes = @periodo";
            SqlCommand cmd;
            SqlDataReader rd;
            string[] datosAfp = new string[2];
            string info = "";

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS 
                        cmd.Parameters.Add(new SqlParameter("@contrato", txtcontrato.Text));
                        cmd.Parameters.Add(new SqlParameter("@periodo", PeriodoEmpleado));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                datosAfp[0] = ((decimal)rd["porcFondo"] + (decimal)rd["porcAdmin"]) + "";
                                datosAfp[1] = (string)rd["nombre"];
                            }
                        }
                    }
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
            info = datosAfp[0] + "% Cotiz. " + datosAfp[1] + " Sobre:" + varSistema.ObtenerValorLista("systimp");

            return info;
        }

        //PARA SALUD (TIPO SALUD)
        private string SaludEmpleado(double original)
        {
            string sql = "select isapre.id as identificador, isapre.nombre as nombre from trabajador " +
                        "INNER JOIN isapre ON isapre.id = trabajador.salud " +
                        "WHERE contrato = @contrato AND anomes =@periodo";
            SqlCommand cmd;
            SqlDataReader rd;
            string info = "";
            int id = 0;
            try
            {
                using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                {
                    //PARAMETROS
                    cmd.Parameters.Add(new SqlParameter("@contrato", txtcontrato.Text));
                    cmd.Parameters.Add(new SqlParameter("@periodo", PeriodoEmpleado));

                    rd = cmd.ExecuteReader();
                    if (rd.HasRows)
                    {

                        while (rd.Read())
                        {
                            id = (int)rd["identificador"];
                            if (id == 1)
                            {
                                //FONASA
                                info = "7% FONASA";
                            }
                            else
                            {
                                //ISAPRE                               
                                if (varSistema.ObtenerValorLista("sysdiaslic") != 0)
                                {
                                    //TIENE LICENCIAS
                                    info = "UF" + original + " Cotiz." + (string)rd["nombre"] + " (-Lic)";
                                }
                                else
                                {
                                    info = "UF" + original + " Cotiz." + (string)rd["nombre"];
                                }                                
                            }
                        }
                    }

                }
                cmd.Dispose();
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return info;
        }

        //IMPUESTO
        private string ImpuestoEmpleado()
        {
            string info = "";
            if (varSistema.ObtenerValorLista("sysdiaslic") == 0)
            {
                //NO TIENE LICENCIAS ASOCIADAS
                info = "Impto " + varSistema.ObtenerValorLista("sysfactorimpto") * 100 + "% " +
                "de: " + varSistema.ObtenerValorLista("systributo") + " -Rebaja:" + varSistema.ObtenerValorLista("sysrebimpto");
            }
            else
            {
                info = "Impto " + varSistema.ObtenerValorLista("sysfactorimpto") * 100 + "% " +
                "de: " + varSistema.ObtenerValorLista("systributo") + " -Rebaja:" + varSistema.ObtenerValorLista("sysrebimpto") + " Lic(-)";
            }



            return info;
        }

        //OBTENER NOMBRE COMPLETO, CARGO, RUT, FECHA INGRESO
        private List<string> CabeceraEmpleado(string contrato, int periodo)
        {
            string sql = "SELECT concat(apepaterno, ' ', apematerno,' ', trabajador.nombre) as nombre, " +
                "rut, cargo.nombre as cargo, ingreso FROM trabajador " +
                "INNER JOIN cargo ON cargo.id = trabajador.cargo " +
                "WHERE contrato = @contrato AND anomes = @periodo";

            List<string> lista = new List<string>();
            string d = "";
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@periodo", periodo));
                        cmd.Parameters.Add(new SqlParameter("@contrato", contrato));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //GUARDAMOS LOS DATOS
                                lista.Add((string)rd["nombre"]);
                                lista.Add(fnSistema.fFormatearRut2((string)rd["rut"]));
                                lista.Add((string)rd["cargo"]);                               
                                d = fnSistema.FechaFormato((DateTime)rd["ingreso"]);                           
                                lista.Add(d);                                
                                    
                            }
                        }
                    }
                    cmd.Dispose();
                    rd.Close();
                    fnSistema.sqlConn.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            //RETORNAMOS LA LISTA
            return lista;
        }

        #endregion

        private void btnFamiliares_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            fnAparienciaButtons();

            Cursor.Current = Cursors.WaitCursor;

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmcargas") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            string contrato = "";
            if (txtcontrato.Text != "" && txtcontrato.ReadOnly)
            {
                contrato = txtcontrato.Text;
                frmCargaFamiliar fam = new frmCargaFamiliar(contrato);
                fam.ShowDialog();
            }
            else
            {
                XtraMessageBox.Show("Trabajador no Valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
               
        }

        private void txtDirec_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void txtClase_Enter(object sender, EventArgs e)
        {
            fnAparienciaLaboral();
        }

        private void btnLiquidacion_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            fnAparienciaButtons();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmliquidacion") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (Calculo.ViewStatus())
            { XtraMessageBox.Show("Hay un proceso de calculo en curso, por favor espere un momento", $"Proceso activo por {User.getUser()}", MessageBoxButtons.OK, MessageBoxIcon.Warning); Close(); return; }

            if (FichaHistorica)
            { XtraMessageBox.Show("Liquidación no disponible", "Información", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (ExistenIndicesMensuales(Calculo.PeriodoObservado))
            { XtraMessageBox.Show("Hemos detectado que no se han ingresado los indices mensuales, por favor ingrese los indices mensuales para el periodo " + Calculo.PeriodoObservado, "Información Importante", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            if (StatusTrabajador == 0)
            { XtraMessageBox.Show("No puedes ver una liquidación de una ficha con estado inactiva", "Ficha inactiva", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            string contrato = "";
            int periodo = 0;         

            if (txtcontrato.ReadOnly)
            {
                contrato = txtcontrato.Text;
                periodo = PeriodoEmpleado;

                frmPrevLiquidacion liq = new frmPrevLiquidacion(contrato, periodo);
                liq.ShowDialog();                          
            }               
        }

        private void btnVacaciones_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            fnAparienciaButtons();

            Cursor.Current = Cursors.WaitCursor;

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmvacacion") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            string contrato = "";

            if (Persona.ExisteContrato(txtcontrato.Text, PeriodoEmpleado) == false)
            { XtraMessageBox.Show("No existe contrato", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (txtcontrato.ReadOnly && txtcontrato.Text != "")
            {
                contrato = txtcontrato.Text;
                frmVacaciones vac = new frmVacaciones(contrato, PeriodoEmpleado);
                vac.StartPosition = FormStartPosition.CenterParent;
                vac.ShowDialog();
            }
        }

        private void dtingr_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                //LA FECHA QUE SE SELECCIONE ACA SE DEBE COLOCAR EN EL COMBO SEGURO CESANTIA
                dtSegCes.EditValue = dtingr.EditValue;

                //LA FECHA QUE SE SELECCIONE ACA DEBE SER LA MISMA QUE SE SELECCIONE EN FECHA VACACIONES
                dtVac.EditValue = dtingr.EditValue;

                //LA FECHA DE PROGRESIVO ES LA MISMA QUE LA FECHA QUE SE SELECCIONE ACA
                dtProg.EditValue = dtingr.EditValue;
            }
        }

        private void txtRut_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtcontrato_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtNombre_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtPasaporte_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtapePaterno_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtApeMat_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtDirec_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtFono_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtcuenta_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtSexo_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void dtFecNac_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txteCivil_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtNacion_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtCiudad_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtTipoCont_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtArea_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void dtingr_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void dtSal_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtCargo_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtccosto_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtRegsal_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void dtVac_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void dtProg_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtClase_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtCausal_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtRegimen_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtSalud_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtAfp_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void dtSegCes_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtCajaPrevision_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtjubilado_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtTramo_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtfPago_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtBanc_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtTipoCuenta_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            string contrato = "";
            if (FichaHistorica)
            { Close();return; }

            if (Navega.Universo > 0)
            {
                if (txtcontrato.ReadOnly && txtcontrato.Text != "")
                {
                    contrato = txtcontrato.Text;
                    if (fnComparar(contrato, PeriodoEmpleado))
                    {
                        DialogResult adv = XtraMessageBox.Show("Hay cambios sin guardar, ¿Deseas cerrar de todas formas?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (adv == DialogResult.Yes)
                            Close();
                    }
                    else
                        Close();
                }
                else
                    Close();
            }
            else
                Close();
        }

        private void btnHistorico_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            fnAparienciaButtons();

            Cursor.Current = Cursors.WaitCursor;

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmdochis") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion" , "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            string contrato = "", nombre = "", rut = "";
            if (txtcontrato.ReadOnly)
            {
                rut = fnSistema.fnExtraerCaracteres(txtRut.Text);
                nombre = NombreCompleto;
                contrato = txtcontrato.Text;
                frmLiquidacionesTrabajador liq = new frmLiquidacionesTrabajador(contrato, nombre, rut);
                liq.StartPosition = FormStartPosition.CenterParent;
                liq.ShowDialog();
            }       
        }

        private void txtNacion_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //MOSTRAR FORMULARIO NACIONALIDAD AL HACER DOBLE CLICK EN COMBO NACION (FORMULARIO TABLAS)
            frmTablas nacionalidad = new frmTablas("nacionalidad");
            nacionalidad.Text = "NACIONALIDAD";
            nacionalidad.opener = this;
            nacionalidad.ShowDialog();
        }

        private void btnFichaTrabajador_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            fnAparienciaButtons();

            Cursor.Current = Cursors.WaitCursor;

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmfichaempleado") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            string contrato = "";
            int periodo = 0;
            if (txtcontrato.ReadOnly && txtcontrato.Text != "") contrato = txtcontrato.Text;
            if (PeriodoEmpleado != 0) periodo = PeriodoEmpleado;

            frmFichaEmpleado ficha = new frmFichaEmpleado(contrato, periodo);
            ficha.StartPosition = FormStartPosition.CenterParent;
            ficha.ShowDialog();
        }

        private void txtcodSucursal_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void txtcodSucursal_Enter(object sender, EventArgs e)
        {
            fnAparienciaLaboral();
        }

        private void txtFun_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtFun_Enter(object sender, EventArgs e)
        {
            fnAparienciaPrevision();
        }

        private void txtSalud_EditValueChanged(object sender, EventArgs e)
        {
            //SI SALUD ES FONASA NUMERO DE FUN LO DESHABILITAMOS 
            if (Convert.ToInt32(txtSalud.EditValue)== 1)
            {
                txtFun.Text = "0";
                txtFun.Enabled = false;
            }
            else
            {
                txtFun.Text = "0";
                txtFun.Enabled = true;
                   
            }
        }

        private void comboVista_EditValueChanged(object sender, EventArgs e)
        {
            CheckedListBoxItemCollection items = comboVista.Properties.Items;
            int count = 0;
            foreach (CheckedListBoxItem elemento in items)
            {
                if (elemento.CheckState == CheckState.Checked)
                    count++;                    
            }

            if (count == 0)
            {
                XtraMessageBox.Show("Debes seleccionar al menos una opcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                comboVista.Properties.Items[0].CheckState = CheckState.Checked;
                return;
            }           

            Navega.CambiaVisualizacion(comboVista);

            if (Navega.Listado.Count == 0)
            {
                XtraMessageBox.Show("No se encontraron registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                comboVista.Properties.Items[0].CheckState = CheckState.Checked;
                comboVista.Properties.Items[1].CheckState = CheckState.Unchecked;
                return;
            }

            //VERIFICAR SI HAY REGISTROS            
            /*if (ListadoContratos(Calculo.PeriodoObservado).Count == 0)
            {
                XtraMessageBox.Show("No se encontraron registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                comboVista.Properties.Items[0].CheckState = CheckState.Checked;
                comboVista.Properties.Items[1].CheckState = CheckState.Unchecked;
                return;
            }*/


            //MOSTRAR EL PRIMER ELEMENTO
            //posicion = 0;
            //fnFirst();                
        }

        //PREGUNTAR SI ESTA ACTIVO LA BUSQUEDA DE TRABAJADORES ACTIVO
        private bool VisualizaElemento(string pDescripcion)
        {
            bool busca = false;

            CheckedListBoxItemCollection items = comboVista.Properties.Items;

            foreach (CheckedListBoxItem elemento in items)
            {                
                if (elemento.CheckState == CheckState.Checked && elemento.Description.ToLower() == pDescripcion.ToLower())
                    busca = true;
            }

            return busca;
        }

        


        private void radioOrden_SelectedIndexChanged(object sender, EventArgs e)
        {
            RadioGroup radio = sender as RadioGroup;

            fnAparienciaFiltros();

            //GUARDAMOS EL ELEMENTO SELECCIONADO
            ElementoOrden = radio.Properties.Items[radio.SelectedIndex].Description;
            fnSistema.OpcionBusqueda = radio.SelectedIndex;

            if (Navega != null)                
                Navega.IsChangeOrder(ElementoOrden, comboVista);
            //XtraMessageBox.Show(radio.Properties.Items[radio.SelectedIndex].Description);   
            
        }

        //DEVUELVE CADENA PARA SQL
        private string GetcadenaOrden(string value)
        {
            string cad = "";

            switch (value.ToLower())
            {
                case "rut":
                    cad = " ORDER BY rut";
                    break;
                case "contrato":
                    cad = " ORDER BY contrato";
                    break;
                case "nombre":
                    cad = " ORDER BY nombre";
                    break;
                case "centro costo":
                    cad = " ORDER BY ccosto";
                    break;
                case "apellido":
                    cad = " ORDER BY apepaterno";
                    break;
                default:
                    cad = " ORDER BY nombre, apepaterno";
                    break;
            }

            return cad;
        }

        private void txtTramo_EditValueChanged(object sender, EventArgs e)
        {            
            if (txtTramo.Properties.DataSource != null)
            {
                if (Convert.ToInt32(txtTramo.EditValue) == 1)
                    txtTramoLetra.Text = "A";
                if (Convert.ToInt32(txtTramo.EditValue) == 2)
                    txtTramoLetra.Text = "B";
                if (Convert.ToInt32(txtTramo.EditValue) == 3)
                    txtTramoLetra.Text = "C";
                if (Convert.ToInt32(txtTramo.EditValue) == 4)
                    txtTramoLetra.Text = "D";
            }
        }

        private void txtSucursal_DoubleClick(object sender, EventArgs e)
        {
            frmTablas tabla = new frmTablas("sucursal");
            tabla.Text = "Sucursal";
            tabla.opener = this;
            tabla.StartPosition = FormStartPosition.CenterParent;
            tabla.ShowDialog();

        }

        private void txtSucursal_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void txtjubilado_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void txtAfp_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void txtCajaPrevision_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void btnShowContrato_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            fnAparienciaButtons();

            if (FileExcel.IsWordInstalled() == false)
            { XtraMessageBox.Show("Parece ser que no tienes instalado Microsoft Office", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            //RUTA DEL ARCHIVO 
            string path = Application.StartupPath + @"\Documentos\contrato.doc";
            if (File.Exists(path) == false)
            {
                XtraMessageBox.Show("Archivo contrato no existe", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            DataSet data = new DataSet();

            //VERIFICAMOS SI CONTRATO EXISTE
            if (Persona.ExisteContrato(txtcontrato.Text, PeriodoEmpleado) == false)
            { XtraMessageBox.Show("Contrato no valido", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            //OBTENEMOS LOS DATOS DEL TRABAJADOR
            data = Persona.GetDataSource(txtcontrato.Text, PeriodoEmpleado);

            if (data == null)
            { XtraMessageBox.Show("No se encontró información", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            Documento doc = new Documento(txtcontrato.Text, PeriodoEmpleado);
           // doc.GeneraContrato(data, Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + $"contrato{txtcontrato.Text}.doc");           
           
        }

        private void txtArea_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void txtEmail_Enter(object sender, EventArgs e)
        {
            fnAparienciaPersonal();
        }

        private void cbPrivado_CheckedChanged(object sender, EventArgs e)
        {
            fnAparienciaEstado();
        }

        private void btnNuevo_Enter(object sender, EventArgs e)
        {
            fnAparienciaButtons();
        }

        private void btnGrabar_Enter(object sender, EventArgs e)
        {
            fnAparienciaButtons();
        }

        private void btnFirst_Enter(object sender, EventArgs e)
        {
            fnAparienciaButtons();
        }

        private void btnPrev_Enter(object sender, EventArgs e)
        {
            fnAparienciaButtons();
        }

        private void btnnext_Enter(object sender, EventArgs e)
        {
            fnAparienciaButtons();
        }

        private void btnLast_Enter(object sender, EventArgs e)
        {
            fnAparienciaButtons();
        }

        private void btnBuscar_Enter(object sender, EventArgs e)
        {
            fnAparienciaButtons();
        }

        private void btnEliminar_Enter(object sender, EventArgs e)
        {
            fnAparienciaButtons();
        }

        private void comboVista_Enter(object sender, EventArgs e)
        {
            fnAparienciaFiltros();
        }

        private void cbPrivado_Click(object sender, EventArgs e)
        {
            fnAparienciaEstado();
        }

        private void txtCausal_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void btnFiniquito_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmfiniquito") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta función", "Acceso", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            Cursor.Current = Cursors.WaitCursor;

            if (txtcontrato.ReadOnly)
            {
                if (Persona.ExisteContrato(txtcontrato.Text, PeriodoEmpleado))
                {
                    frmFiniquito fin = new frmFiniquito(txtcontrato.Text, PeriodoEmpleado);
                    fin.StartPosition = FormStartPosition.CenterParent;
                    fin.ShowDialog();
                }
                else
                {
                    XtraMessageBox.Show("Numero de contrato no válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
            else
            {
                XtraMessageBox.Show("Registro no válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void txtEmail_Properties_BeforeShowMenu(object sender, BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void spProg_Properties_BeforeShowMenu(object sender, BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void btnDocContrato_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmcontrato") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta función", "Acceso", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (Persona.ExisteContrato(txtcontrato.Text, PeriodoEmpleado))
            {
                frmContrato form = new frmContrato(txtcontrato.Text, PeriodoEmpleado);
                form.StartPosition = FormStartPosition.CenterParent;
                form.ShowDialog();
            }
        }

        private void btnAntiguedad_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            fnAparienciaButtons();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmcontrato") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta función", "Acceso", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            string contrato = "";
            int periodo = 0;
            if (txtcontrato.ReadOnly && txtcontrato.Text != "") contrato = txtcontrato.Text;
            if (PeriodoEmpleado != 0) periodo = PeriodoEmpleado;

            frmAntiguedad form = new frmAntiguedad(txtcontrato.Text, PeriodoEmpleado);
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog();
        }

        private void btnAntiguedad_Click_1(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmAntiguedad") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta función", "Acceso", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (Persona.ExisteContrato(txtcontrato.Text, PeriodoEmpleado))
            {
                frmAntiguedad form = new frmAntiguedad(txtcontrato.Text, PeriodoEmpleado);
                form.StartPosition = FormStartPosition.CenterParent;
                form.ShowDialog();
            }
        }

        private void btnCuentaRut_Click(object sender, EventArgs e)
        {
            string cuenta = "";

            //SI EL RUT TIENE PUNTOS
            if (txtRut.Text.Contains(".") || txtRut.Text.Contains("-"))
            {
                cuenta = txtRut.Text.Replace(".", "");
                cuenta = cuenta.GetCharsBefore("-");

                if (cuenta[0].ToString() == "0")
                {
                    string rutSinCero = cuenta.Remove(0, 1);
                    txtcuenta.Text = rutSinCero;
                }
                else
                {
                    txtcuenta.Text = cuenta;
                }
                
            }
            else
            {
                if (fnSistema.fValidaRut(txtRut.Text))
                {
                    string rut = fnSistema.fFormatearRut2(txtRut.Text);
                    cuenta = rut.Replace(".", "");
                    cuenta = cuenta.GetCharsBefore("-");
                    if (cuenta[0].ToString() == "0")
                    {
                        string rutSinCero = cuenta.Remove(0, 1);
                        txtcuenta.Text = rutSinCero;
                    }
                    else
                    {
                        txtcuenta.Text = cuenta;
                    }

                }
                else
                {
                    XtraMessageBox.Show("Ingrese un número de rut correcto", "Cuenta Rut", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtcuenta.Text = ""; txtRut.Focus(); return;
                }
            }


            //if (fnSistema.fValidaRut(txtRut.Text))
            //{
            //    if (txtRut.Text.Contains(".") || txtRut.Text.Contains("-"))
            //    {
            //        cuenta = txtRut.Text.Replace(".", "");
            //        cuenta = cuenta.GetCharsBefore("-");
            //        txtcuenta.Text = cuenta;
            //    }
            //    else
            //    {
            //        string rut = fnSistema.fFormatearRut2(txtRut.Text);
            //        cuenta = rut.Replace(".", "");
            //        cuenta = cuenta.GetCharsBefore("-");
            //        txtcuenta.Text = cuenta;
            //    }
                
            //}
            //else
            //{
            //    XtraMessageBox.Show("Ingrese un número de rut correcto", "Cuenta Rut", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtRut.Focus(); return;
            //}
        }

        private void txtNacion_EditValueChanged(object sender, EventArgs e)
        {
            
        }

        private void txtEstado_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void txtClase_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void btnObservaciones_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmobservacion") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta función", "Acceso", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            Cursor.Current = Cursors.WaitCursor;

            if (Persona.ExisteContrato(txtcontrato.Text, PeriodoEmpleado))
            {                
                frmObservaciones obs = new frmObservaciones(txtcontrato.Text, PeriodoEmpleado);
                obs.StartPosition = FormStartPosition.CenterParent;
                obs.ShowDialog();
            }
         
        }

        private void txtRut_Leave(object sender, EventArgs e)
        {
            ValidaRut();
        }

        private void txtcontrato_Leave(object sender, EventArgs e)
        {
            ValidaContrato();
        }

        private void dtFecNac_Leave(object sender, EventArgs e)
        {
            ValidaNacimiento();
        }

        private void txtHorario_Enter(object sender, EventArgs e)
        {
            fnAparienciaLaboral();
        }

        private void txtJornadaLaboral_Enter(object sender, EventArgs e)
        {
            fnAparienciaLaboral();
        }

        private void txtSindicato_Enter(object sender, EventArgs e)
        {
            fnAparienciaLaboral();
        }

        private void dtingr_Leave(object sender, EventArgs e)
        {
          
        }

        private void labelControl52_Click(object sender, EventArgs e)
        {

        }

        private void btnAdicional_Click(object sender, EventArgs e)
        {
            if (txtcontrato.Text.Length > 0 && Persona.ExisteContrato(txtcontrato.Text, PeriodoEmpleado))
            {
                bool Historica = false;
                if (PeriodoEmpleado != Calculo.PeriodoObservado)
                    Historica = true;

                frmDetalleEmpleado detail = new frmDetalleEmpleado(txtcontrato.Text, PeriodoEmpleado, Historica);
                detail.StartPosition = FormStartPosition.CenterParent;
                detail.ShowDialog();
            }
            else
            {
                XtraMessageBox.Show("N° de contrato no es válido", "Ficha", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }
    }



    class Empleado
    {
        public string contrato { get; set; }
        public int periodo { get; set; }
        public Int16 estado { get; set; }
    }


    
}
