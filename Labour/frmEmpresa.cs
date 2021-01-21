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
using System.Collections;
using System.Runtime.InteropServices;
//using static System.Windows.Forms.Menu;
using System.IO;
using System.Drawing.Imaging;

namespace Labour
{
    
    public partial class frmEmpresa : DevExpress.XtraEditors.XtraForm
    {
        [DllImport("user32")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32")]
        static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        const int MF_BYCOMMAND = 0;
        const int MF_DISABLED = 2;
        const int SC_CLOSE = 0xF060;

        //variable para controlar la posicion del arreglo
        int pos = 0;
        //PARA SABER EL RUT QUE SE ESTA MOSTRANDO (REGISTRO QUE SE ESTA CARGANDO)
        string actual = "";

        //VARIABLE PARA GUARDAR LA RUTA DONDE SE GUARDARAN LAS IMAGENES TEMPORALES PARA EL LOGO
        private string PathLogo = Directory.GetCurrentDirectory();

        //PARA MANIPULAR LA IMAGEN QUE SUBE EL USUARIO
        private Image ImagenUsuario = null;

        //PARA SABER SI SE RECORTO O NO LA IMAGEN
        private bool recortar = false;

        //PARA GUARDAR LAS COORDENADAS QUE SE SELECCIONAN EN LA IMAGEN
        Point startpoint = new Point();
        Point endpoint = new Point();

        //RECTANGULO PARA DIBUJAR EL AREA SELECCIONADA
        Rectangle Rectangulo;

        //GUARDAR LA RUTA DONDE SE GUARDA LA IMAGEN EN DISCO UNA VEZ CARGADA...
        string PathImage = "";

        bool MouseClicked = false;

        //DEFAULT REGISTRO EN TABLA EMPRESA
        public bool DefaultEmpresa { get; set; } = false;
        public string RutEmpDb { get; set; } = "";

        public frmEmpresa()
        {
           InitializeComponent();          
        }
        
        private void frmEmpresa_Load(object sender, EventArgs e)
        {
            //PARA DESHABLITAR EL BOTON DE CIERRE DE VENTANA(ESQUINA SUPERIOR DERECHA)
            //CON LA SALVEDAD DE QUE CUANDO SE MAXIMIZA VUELVE A APARECER (HOW TO FIX THAT?)
            var sm = GetSystemMenu(Handle, false);
            EnableMenuItem(sm, SC_CLOSE, MF_BYCOMMAND | MF_DISABLED);

            /*Para no tener problemas de escitura en C: (Usuario no administrador)*/
            if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\AppRemu\\"))
                PathLogo = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\AppRemu\\";
            else
            {
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\AppRemu\\");
                PathLogo = Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\AppRemu\\").FullName;
            }

            //CARGAR COMBO CIUDAD
            datoCombobox.spllenaComboBox("SELECT idCiudad, descCiudad FROM ciudad ORDER BY descCiudad",txtCiudad,"idCiudad", "descCiudad",true);
            //COMBO CAJA Y MUTUAL
            datoCombobox.spllenaComboBox("SELECT id, codmutual FROM mutual", txtMutual, "id", "codmutual", true);
            datoCombobox.spllenaComboBox("SELECT id, desccaja FROM cajacompensacion", txtCajacompensacion, "id", "desccaja", true);
            datoCombobox.spllenaComboBox("SELECT codComuna, descComuna FROM comuna ORDER BY region, descComuna", txtComuna, "codComuna", "descComuna", true);

            //PROPIEDADES POR DEFECTO
            fnDefaultProperties();
            //cargar combo afiliacion
            fnComboAfiliacion();
            //cargamos campos
            string[] registros = fnRegistros();            

            //SOLO CARGAR SI REGISTROS NO ES NULO Y EL RUT NO ES CERO
            if (registros != null)
            {
                actual = registros[pos];
                fnCargarEmpresa(registros[pos]);
            }                    
        }        

        #region "GENERALES"
        //INGRESO REGISTRO EMPRESA
        //PARAMETROS DE ENTRADA
        /*
         * RAZON
         * RUT EMPRESA
         * GIRO
         * DOMICILIO
         * CIUDAD   --> INT REFERENCES CARGO TABLE
         * TELEFONO
         * EMAIL EMPRESA
         * RUT REPRESENTANTE LEGAL
         * NOMBRE REPRESENTANTE LEGAL
         * RUT RRHH
         * NOMBRE RRHH
         * CARGO RRHH
         * EMAIL RRHH
         * CODIGO ACTIVIDAD
         * FECHA INICIO ACTIVIDADES
         */
        private void fnNuevaEmpresa(TextEdit pRazon, string prutEmp,TextEdit pGiro,TextEdit pDomicilio, LookUpEdit pCiudad,
            TextEdit pFono, TextEdit pmailEmp, string prutRep, TextEdit pnombreRep, string prutRh, 
            TextEdit pnombreRh, TextEdit pcargoRh, TextEdit pmailRh, TextEdit pcodActividad, DateEdit pActividad,
            LookUpEdit pcodAfi, LookUpEdit pnomMutual, TextEdit pnumMutual, TextEdit pcotMutual, LookUpEdit pAsignacion,
            string pLogo, string pCodEmpresa, LookUpEdit pComuna, TextEdit pCodPais, TextEdit pCodArea)
        {
            //SQL INSERT
            string sql = "INSERT INTO empresa(rutEmp, razon, giro, direccion, ciudad, telefono, emailEmp, rutRep," +
                "nombreRep, rutRRHH, nombreRRHH, cargoRRHH, emailRRHH, codActividad, inicioActividades, codAfiliacion," +
                "nombreMut, codMut, cotMut, nombreCCAF, logo, lemp, codempresa, comuna, cdArea, cdPais) VALUES(" +
                "@prutEmp, @pRazon, @pGiro, @pDomicilio, @pCiudad, @pFono, @pmailEmp, @prutRep, @pnombreRep," +
                "@prutRh, @pnombreRh, @pcargoRh, @pmailRh, @pcodActividad, @pActividad, @pcodAfi, @pnomMutual, @pnumMutual," +
                "@pcotMutual, @pAsignacion, @pLogo, @pCodVal, @pCodEmpresa, @pComuna, @pCodArea, @pCodPais)";

            SqlCommand cmd;
            int res = 0;
            Empresa emp = new Empresa();            

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        //VALIDAR RUT ANTES DE INGRESAR
                        cmd.Parameters.Add(new SqlParameter("@prutEmp", prutEmp));
                        cmd.Parameters.Add(new SqlParameter("@pRazon", pRazon.Text));
                        cmd.Parameters.Add(new SqlParameter("@pGiro", pGiro.Text));
                        cmd.Parameters.Add(new SqlParameter("@pDomicilio", pDomicilio.Text));
                        cmd.Parameters.Add(new SqlParameter("@pCiudad", pCiudad.EditValue));
                        cmd.Parameters.Add(new SqlParameter("@pFono", pFono.Text));
                        cmd.Parameters.Add(new SqlParameter("@pmailEmp", pmailEmp.Text));
                        //VALIDAR RUT REPRESENTANTE LEGAL ANTES DE INGRESAR
                        cmd.Parameters.Add(new SqlParameter("@prutRep", prutRep));
                        cmd.Parameters.Add(new SqlParameter("@pnombreRep", pnombreRep.Text));
                        //VALIDAR RUT RECURSOS HUMANOS ANTES DE INGRESAR
                        cmd.Parameters.Add(new SqlParameter("@prutRh", prutRh));
                        cmd.Parameters.Add(new SqlParameter("@pnombreRh", pnombreRh.Text));
                        cmd.Parameters.Add(new SqlParameter("@pcargoRh", pcargoRh.Text));
                        cmd.Parameters.Add(new SqlParameter("@pmailRh", pmailRh.Text));
                        cmd.Parameters.Add(new SqlParameter("@pcodActividad", pcodActividad.Text));
                        cmd.Parameters.Add(new SqlParameter("@pActividad", pActividad.EditValue));
                        cmd.Parameters.Add(new SqlParameter("@pcodAfi", pcodAfi.EditValue));
                        cmd.Parameters.Add(new SqlParameter("@pnomMutual", pnomMutual.EditValue));
                        cmd.Parameters.Add(new SqlParameter("@pnumMutual", pnumMutual.Text));
                        cmd.Parameters.Add(new SqlParameter("@pcotMutual", decimal.Parse(pcotMutual.Text)));
                        cmd.Parameters.Add(new SqlParameter("@pAsignacion", pAsignacion.EditValue));
                        cmd.Parameters.Add(new SqlParameter("@pCodEmpresa", pCodEmpresa));
                        cmd.Parameters.Add(new SqlParameter("@pComuna", Convert.ToInt32(pComuna.EditValue)));
                        cmd.Parameters.Add(new SqlParameter("@pCodArea", pCodArea.Text));
                        cmd.Parameters.Add(new SqlParameter("@pCodPais", pCodPais.Text));

                        //CODIGO DE VALIDACION       
                        emp.Rut = prutEmp;
                        emp.Giro = pGiro.Text;
                        emp.Razon = pRazon.Text;
                        cmd.Parameters.Add(new SqlParameter("@pCodVal", emp.GeneraCodValidacion()));                        

                        //PARA GUARDAR IMAGEN
                        if (pLogo != "")
                        {
                            byte[] img = Imagen.GuardarImagenBd(pLogo);
                            cmd.Parameters.Add(new SqlParameter("@pLogo", img));
                        }
                        else
                        {
                            //GUARDAMOS COMO NULO
                            cmd.Parameters.Add("@pLogo", SqlDbType.VarBinary).Value = DBNull.Value;
                        }
                            
                        //ejecutamos consulta
                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            XtraMessageBox.Show("Registro Guardado", "Guardar", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            recortar = false;

                            //LIMPIAR RECTANGULO DIBUJA SOBRE PICTURE EDIT
                            Imagen.CleanDraw(Rectangulo);

                            logRegistro log = new logRegistro(User.getUser(), "SE INGRESAN DATOS EMPRESA", "EMPRESA", "0", pRazon.Text, "INGRESAR");
                            log.Log();

                            //cargamos campos
                            string[] registros = fnRegistros();
                            if (registros != null)
                            {
                                actual = registros[pos];
                                fnCargarEmpresa(registros[pos]);
                            }

                           //logRegistro log = new logRegistro(User.getUser(), "NUEVO INGRESO DATOS EMPRESA", fnSistema.pgDatabase, "EMPRESA");
                            //log.Log();
                        }
                        else
                        {
                            XtraMessageBox.Show("Error al intentar guardar", "Guardar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    //LIBERAMOS RECURSOS
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                }
                else
                {
                    XtraMessageBox.Show("No hay conexion con la base de datos", "BASE DE DATOS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

        }


        //METODO PARA MODIFICAR REGISTRO
        //PARAMETROS DE ENTRADA: LOS MISMOS QUE EL METODO INGRESO
        private void fnModificarEmpresa(TextEdit pRazon, TextEdit prutEmp, TextEdit pGiro, TextEdit pDomicilio, LookUpEdit pCiudad,
            TextEdit pFono, TextEdit pmailEmp, string prutRep, TextEdit pnombreRep, string prutRh,
            TextEdit pnombreRh, TextEdit pcargoRh, TextEdit pmailRh, TextEdit pcodActividad, DateEdit pActividad,
            LookUpEdit pcodAfi, LookUpEdit pnomMutual, TextEdit pnumMutual, TextEdit pcotMutual, LookUpEdit pAsignacion, 
            string pLogo, string pCodEmpresa, string pRutBd, LookUpEdit pComuna, TextEdit pCodPais, TextEdit pCodArea)
        {
            //QUERY UPDATE
            string sql = "UPDATE empresa SET rutEmp=@pNuevoRut, " +
                "razon=@pRazon, giro=@pGiro, direccion=@pDomicilio," +
                "ciudad=@pCiudad, telefono=@pFono, emailEmp=@pmailEmp, " +
                "rutRep=@prutRep, nombreRep=@pnombreRep," +
                "rutRRHH=@prutRh, nombreRRHH=@pnombreRh, " +
                "cargoRRHH=@pcargoRh, emailRRHH=@pmailRh, codActividad=@pcodActividad," +
                "inicioActividades=@pActividad, codAfiliacion=@pcodAfi, nombreMut=@pnomMutual, codMut=@pnumMutual," +
                "cotMut=@pcotMutual, nombreCCAF=@pAsignacion, logo=@pLogo, lemp=@pCodVal, " +
                "codempresa=@pCodEmpresa, comuna=@pComuna, cdArea=@pCodArea, cdPais=@pCodPais " +
                "WHERE rutEmp=@prutEmp";
            
            SqlCommand cmd;
            int res = 0;

            Empresa emp = new Empresa();            

            //TABLA HASH PARA LOG
            Hashtable datosEmpresa = new Hashtable();
            datosEmpresa = PrecargaEmpresa(fnSistema.fnExtraerCaracteres(prutEmp.Text));

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        //PARAMETROS
                        //VALIDAR RUT ANTES DE INGRESAR
                        cmd.Parameters.Add(new SqlParameter("@prutEmp", pRutBd));
                        cmd.Parameters.Add(new SqlParameter("@pNuevoRut", fnSistema.fnExtraerCaracteres(prutEmp.Text)));
                        cmd.Parameters.Add(new SqlParameter("@pRazon", pRazon.Text));
                        cmd.Parameters.Add(new SqlParameter("@pGiro", pGiro.Text));
                        cmd.Parameters.Add(new SqlParameter("@pDomicilio", pDomicilio.Text));
                        cmd.Parameters.Add(new SqlParameter("@pCiudad", pCiudad.EditValue));
                        cmd.Parameters.Add(new SqlParameter("@pFono", pFono.Text));
                        cmd.Parameters.Add(new SqlParameter("@pmailEmp", pmailEmp.Text));
                        //VALIDAR RUT REPRESENTANTE LEGAL ANTES DE INGRESAR
                        cmd.Parameters.Add(new SqlParameter("@prutRep", prutRep));
                        cmd.Parameters.Add(new SqlParameter("@pnombreRep", pnombreRep.Text));
                        //VALIDAR RUT RECURSOS HUMANOS ANTES DE INGRESAR
                        cmd.Parameters.Add(new SqlParameter("@prutRh", prutRh));
                        cmd.Parameters.Add(new SqlParameter("@pnombreRh", pnombreRh.Text));
                        cmd.Parameters.Add(new SqlParameter("@pcargoRh", pcargoRh.Text));
                        cmd.Parameters.Add(new SqlParameter("@pmailRh", pmailRh.Text));
                        cmd.Parameters.Add(new SqlParameter("@pcodActividad", pcodActividad.Text));
                        cmd.Parameters.Add(new SqlParameter("@pActividad", pActividad.EditValue));
                        cmd.Parameters.Add(new SqlParameter("@pcodAfi", pcodAfi.EditValue));
                        cmd.Parameters.Add(new SqlParameter("@pnomMutual", Convert.ToInt32(pnomMutual.EditValue)));
                        cmd.Parameters.Add(new SqlParameter("@pnumMutual", pnumMutual.Text));
                        cmd.Parameters.Add(new SqlParameter("@pcotMutual", decimal.Parse(pcotMutual.Text)));
                        cmd.Parameters.Add(new SqlParameter("@pAsignacion", Convert.ToInt32(pAsignacion.EditValue)));
                        cmd.Parameters.Add(new SqlParameter("@pCodEmpresa", pCodEmpresa));
                        cmd.Parameters.Add(new SqlParameter("@pComuna", Convert.ToInt32(pComuna.EditValue)));
                        cmd.Parameters.Add(new SqlParameter("@pCodPais", pCodPais.Text));
                        cmd.Parameters.Add(new SqlParameter("@pCodArea", pCodArea.Text));

                        //UPDATE CODIGO DE VALIDACION
                        emp.Giro = pGiro.Text;
                        emp.Razon = pRazon.Text;
                        emp.Rut = fnSistema.fnExtraerCaracteres(prutEmp.Text);
                        cmd.Parameters.Add(new SqlParameter("@pCodVal", emp.GeneraCodValidacion()));

                        if (pLogo != "")
                        {
                            byte[] img = Imagen.GuardarImagenBd(pLogo);
                            cmd.Parameters.Add(new SqlParameter("@pLogo", img));
                        }
                        else {
                            //GUARDAMOS NULO                            
                            cmd.Parameters.Add("@pLogo", SqlDbType.VarBinary).Value = DBNull.Value;                            
                        }

                        //ejecutamos consulta
                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            XtraMessageBox.Show("Registro Actualizado", "Actualizar", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            recortar = false;

                            //RESETAMOS RECTANGULO DIBUJO
                            Imagen.CleanDraw(Rectangulo);

                            //SI HAY CAMBIOS GUARDAMOS EVENTOS EN LOG
                            ComparaValorEmpresa(fnSistema.fnExtraerCaracteres(prutEmp.Text), datosEmpresa, pRazon.Text,
                                pGiro.Text, pDomicilio.Text, Convert.ToInt32(pCiudad.EditValue), pFono.Text, 
                                pmailEmp.Text,prutRep, pnombreRep.Text,prutRh, pnombreRh.Text, pcargoRh.Text, 
                                pmailRh.Text, pcodActividad.Text, (DateTime)pActividad.EditValue, Convert.ToInt32(pcodAfi.EditValue), 
                                Convert.ToInt32(pnomMutual.EditValue), pnumMutual.Text, decimal.Parse(pcotMutual.Text), 
                                Convert.ToInt32(pAsignacion.EditValue), Convert.ToInt32(pComuna.EditValue),
                                pCodArea.Text, pCodPais.Text);

                            //cargamos campos
                            string[] registros = fnRegistros();
                            if (registros != null)
                            {
                                actual = registros[pos];
                                fnCargarEmpresa(registros[pos]);
                            }

                            //ACTUALIZAR VALOR DE LICENCIA
                          /*  if (fnSistema.ConfigName.Length > 0 && Licencia.ConfigName.Length > 0)
                            {
                                if (fnSistema.ConfigName.ToLower() == Licencia.ConfigName.ToLower())
                                {
                                    //VOLVEMOS A GENERAR EL CODIGO DE LICENCIA
                                    Licencia.IngresaLicencia(Licencia.GenerarLicencia());
                                }
                            }    */                        
                        }
                        else
                        {
                            XtraMessageBox.Show("Error al intentar Actualizar", "Actualizar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    //LIBERAR RECURSOS
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                }
                else
                {
                    XtraMessageBox.Show("No hay conexion con la base de datos", "Base de Datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);

            }

        }

        /*
         * CAMPOS SOLO NUMEROS
         * -------------------------
         * RUT EMPRESA
         * TELEFONO
         * RUT REPRESENTANTE LEGAL
         * RUT RRHH
         * CODIGO ACTIVIDAD         
         * ------------------------
         */

        //PROPIEDADES POR DEFECTO
        private void fnDefaultProperties()
        {
            txtrutEmp.Properties.MaxLength = 12;
            txtrutEmp.Properties.Appearance.FontStyleDelta = FontStyle.Bold;
            txtRazon.Properties.MaxLength = 100;
            txtGiro.Properties.MaxLength = 100;
            txtDom.Properties.MaxLength = 100;
            txtCiudad.ItemIndex = 0;
            txtFono.Properties.MaxLength = 50;
            txtmailEmp.Properties.MaxLength = 100;
            txtrutRep.Properties.MaxLength = 12;
            txtrutRh.Properties.MaxLength = 12;
            txtnomRep.Properties.MaxLength = 100;
            txtnombreRh.Properties.MaxLength = 100;
            txtCargo.Properties.MaxLength = 100;
            txtmailRh.Properties.MaxLength = 50;
            txtcodActividad.Properties.MaxLength = 7;
            dtActividad.EditValue = DateTime.Now.Date;
            btnGuardar.AllowFocus = false;   
            
            separadorEmpresa.TabStop = false;
                   
           
            lblrutRh.Visible = false;
            lblrutEmp.Visible = false;
            lblrutRep.Visible = false;

            dtActividad.Properties.ShowClear = false;
            txtcotMutual.Properties.MaxLength = 5;            
            txtnumMutual.Properties.MaxLength = 50;            
            txtCiudad.Properties.PopupSizeable = false;
            txtcodAfi.Properties.PopupSizeable = false;
            txtcodAfi.ItemIndex = 0;

        }

        //CARGAR CAMPOS
        private void fnCargarEmpresa(string prutEmp)
        {
            string sql = "SELECT rutEmp, razon, giro, direccion, ciudad, telefono, emailEmp, rutRep, nombreRep, rutRRHH," +
                "nombreRRHH, cargoRRHH, emailRRHH, codActividad, inicioActividades, codAfiliacion," +
                " nombreMut, codMut, cotMut, nombreCCAF, logo, lemp, codempresa, comuna, cdPais, cdArea " +
                " FROM empresa WHERE rutEmp=@prutEmp";          

            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@prutEmp", prutEmp));
                        //EJECUTAR CONSULTA
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //DEJAR COMO SOLO LECTURA EL CAMPO RUT EMPRESA
                            if (prutEmp == "999999999")
                                DefaultEmpresa = true;                                                              
                            else
                                txtrutEmp.ReadOnly = true;

                            RutEmpDb = prutEmp;

                            while (rd.Read())
                            {
                                string rutE = "";
                                string rutR = "";
                                string rutRh = "";

                                rutE = fnSistema.fnDesenmascararRut(rd["rutEmp"].ToString());
                                rutR = fnSistema.fnDesenmascararRut(rd["rutRep"].ToString());
                                rutRh = fnSistema.fnDesenmascararRut(rd["rutRRHH"].ToString());                                                                                              

                                txtrutEmp.Text = fnSistema.fFormatearRut2(rutE);
                                txtRazon.Text = (string)rd["razon"];
                                txtGiro.Text = (string)rd["giro"];
                                txtDom.Text = (string)rd["direccion"];
                                txtCiudad.EditValue = (int)rd["ciudad"];
                                txtFono.Text = (string)rd["telefono"];
                                txtmailEmp.Text = (string)rd["emailEmp"];
                                txtrutRep.Text = fnSistema.fFormatearRut2(rutR);
                                txtnomRep.Text = (string)rd["nombreRep"];
                                txtrutRh.Text = fnSistema.fFormatearRut2(rutRh);
                                txtnombreRh.Text = (string)rd["nombreRRHH"];
                                txtCargo.Text = (string)rd["cargoRRHH"];
                                txtmailRh.Text = (string)rd["emailRRHH"];
                                txtcodActividad.Text = (string)rd["codActividad"];
                                dtActividad.EditValue = (DateTime)rd["inicioActividades"];
                                txtcodAfi.EditValue = (int)rd["codAfiliacion"];
                                txtMutual.EditValue = Convert.ToInt32(rd["nombreMut"]);                           
                                txtnumMutual.Text = (string)rd["codMut"];
                                txtcotMutual.Text =((decimal)rd["cotMut"]).ToString();
                                txtCajacompensacion.EditValue = Convert.ToInt32(rd["nombreCCAF"]);
                                txtCodEmpresa.Text = (string)rd["codempresa"];
                                txtComuna.EditValue = Convert.ToInt32(rd["comuna"]);
                                txtcodPais.Text = (string)rd["cdPais"];
                                txtCodArea.Text = (string)rd["cdArea"];

                                //CODIGO DE VALIDACION
                                txtCodigoVal.Text = (string)rd["lemp"];

                                //CARGAMOS LOGO SI NO ES NULO
                                if (rd["logo"] as byte[] != null)
                                {
                                    byte[] img = (byte[])rd["logo"];
                                    Image imagen = Imagen.GetImageFromStream(img);

                                    //CARGAR IMAGEN EN PICTURE EDIT
                                    Imagen.CargarImagen(imagen, pictureLogo);

                                    //GUARDAMOS IMAGEN DE FORMA TEMPORAL EN PC
                                    imagen.Save(PathLogo + "trabajador.jpg", ImageFormat.Jpeg);

                                    //GUARDAMOS LA RUTA DE LA IMAGEN EN VARIABLE PATHIMAGE
                                    PathImage = PathLogo + "trabajador.jpg";

                                    //GUARDAMOS IMAGEN EN IMAGEN USUARIO
                                    ImagenUsuario = imagen;
                                }
                                else {
                                    pictureLogo.Image = Labour.Properties.Resources.logo_vacio;
                                    pictureLogo.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;

                                    //DEJAMOS IMAGEN USUARIO NULL
                                    ImagenUsuario = null;
                                }
                            }
                        }
                        else
                        {
                            //DEJAR COMO CAMPO NORMAL RUT EMPRESA   
                            txtrutEmp.ReadOnly = false;
                            XtraMessageBox.Show("no hay registros", "Registros", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    //LIBERAMOS RECURSOS
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                    rd.Close();
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

        //BUSCAR RUT Y GUARDAR EN ARRAY
        private string[] fnRegistros()
        {
            //QUERY SQL
            string sql = "SELECT rutEmp FROM empresa";
            SqlCommand cmd;
            SqlDataReader rd;
            string[] reg = new string[] { };
            List<string> lista = new List<string>();
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //recorremos y guardamos en array
                            while (rd.Read())
                            {
                                //LLENAMOS LA LISTA
                                lista.Add(rd["rutEmp"].ToString());
                            }
                            //PASAMOS LA LISTA AL ARRAY
                            reg = lista.ToArray();
                        }
                        else
                        {
                            //array sin elementos porque no encontro nada
                            reg = null;
                        }
                                           
                    }
                    //LIBERAMOS MEMORIA
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                    rd.Close();
                }
                else
                {
                    XtraMessageBox.Show("No hay conexion con la base de datos", "Base de datos",MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
            //retornamos el arreglo
            return reg;
        }

        //verificar que el rut contenga puntos y guion
        private bool fnRutCorrecto(string pRut)
        {
            /*
             * QUE CONTENGA PUNTO Y GUION
             * QUE CONTENGA PUNTO Y NO CONTENGA GUION
             * QUE NO CONTENGA PUNTO Y CONTENGA GUION
             * QUE NO CONTENGA PUNTO Y NO CONTENGA GUION
             */

            /*
             * @2 CASES
             * @1 12 CARACTERES => 17.444.666-k
             * @2 11 CARACTERES => 8.456.159-0
             */
            if (pRut.Length == 12)
            {
                //SI CONTIENE 12 CARACTERES
                //REGLAS:
                /*
                 * EN TERCERA POSICION DEBERIA HABER UN . (SEGUNDA PARTIENDO DESDE CERO)
                 * EN SEPTIMA POSICION DEBERIA HABER UN . ( SEXTA PARTIENDO DESDE CERO)
                 * EN POSICION ONCE DEBERIA HABER UN - (DECIMA POSICION PARTIENDO DESDE CERO)
                 */

                //RECORRER CADENA Y PREGUNTAR CUANDO PUNTO TIENE
                //SI TIENE MAS DE 2 NO ES VALIDO
                //LO MISMO PARA LA COMA 
                int c = 0, p = 0;
                for (int i = 0; i < pRut.Length; i++)
                {
                    if (pRut[i].ToString() == ".") p++;
                    if (pRut[i].ToString() == "-") c++;                    
                }

                if (c > 1) return false;
                if (c == 0) return false;
                if (p > 2 || p < 2) return false;

                int posC = 0, posP1 = 0, posP2 = 0;
                for (int i = 0; i < pRut.Length; i++)
                {
                    if (pRut[i].ToString() == ".")
                    {
                        //GUARDO SU POSICION
                        if (i == 2) posP1 = i;
                        if (i == 6) posP2 = i;                       
                    }
                    if (pRut[i].ToString() == "-")
                    {
                        //GUARDO SU POSICION
                        if (i == 10) posC = i;
                    }
                }

                if (posP1 == 0 || posP2 == 0 || posC == 0) return false;
                else return true;

            }
            else if (pRut.Length == 11)
            {
                //SI CONTIENE 11 CARACTERES
                //REGLAS
                /*
                 * EN SEGUNDA POSICION DEBERIA HABER UN . (PRIMERA POSICION PARTIENDO DESDE CERO)
                 * EN SEXTA POSICION DEBERIA HABER UN . (QUINTA POSICION PARTIENDO DESDE CERO)
                 * EN DECIMA POSICION DEBERIA HABER UN - (NOVENA POSICION PARTIENDO DESDE CERO)
                 */

                int c = 0, p = 0;
                for (int i = 0; i < pRut.Length; i++)
                {
                    if (pRut[i].ToString() == ".") p++;
                    if (pRut[i].ToString() == "-") c++;
                }

                if (c > 1) return false;
                if (c == 0) return false;
                if (p > 2 || p < 2) return false;

                int posC = 0, posP1 = 0, posP2 = 0;
                for (int i = 0; i < pRut.Length; i++)
                {
                    if (pRut[i].ToString() == ".")
                    {
                        //GUARDO SU POSICION
                        if (i == 1) posP1 = i;
                        if (i == 5) posP2 = i;
                    }
                    if (pRut[i].ToString() == "-")
                    {
                        //GUARDO SU POSICION                        
                        if (i == 9) posC = i;
                    }
                }               
                if (posP1 == 0 || posP2 == 0 || posC == 0) return false;
                else return true;
            }
            else
            {
                return false;
            }       
            
        }

        //VERIFICAR CAMBIOS EN ALGUNA CAJA
        private bool fnVericarCambios(string pRut)
        {
            //QUERY SQL
            string sql = "SELECT rutEmp, razon, giro, direccion, ciudad, telefono, emailEmp, rutRep, nombreRep," +
                "rutRRHH, nombreRRHH, cargoRRHH, emailRRHH, codActividad, inicioActividades," +
                "codAfiliacion, nombreMut, codMut, cotMut, nombreCCAF, codempresa FROM empresa WHERE rutEmp=@pRut";

            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pRut", pRut));
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //COMPARAMOS
                            while (rd.Read())
                            {
                                //COMPARAR RUT SIN . NI -
                                //COMPARA RUT ENMASCARADOS
                                //VOLVER A ENMASCARAR LOS RUT QUE ESTAN EN LAS CAJAS DE TEXTO
                                string r1 = "", r2 = "", r3 = "";
                                string s1 = "", s2 = "", s3 = "";                              

                                r1 = fnSistema.fnExtraerCaracteres(txtrutEmp.Text);                                
                                r2 = fnSistema.fnExtraerCaracteres(txtrutRep.Text);                                
                                r3 = fnSistema.fnExtraerCaracteres(txtrutRh.Text);

                                s1 = fnSistema.fEnmascaraRut(r1);
                                s2 = fnSistema.fEnmascaraRut(r2);
                                s3 = fnSistema.fEnmascaraRut(r3);


                                if (s1 != rd["rutEmp"].ToString()) return true;
                                if (txtRazon.Text != rd["razon"].ToString()) return true;
                                if (txtGiro.Text != rd["giro"].ToString()) return true;
                                if (txtDom.Text != rd["direccion"].ToString()) return true;
                                if (txtCiudad.EditValue.ToString() != rd["ciudad"].ToString()) return true;
                                if (txtFono.Text != rd["telefono"].ToString()) return true;
                                if (txtmailEmp.Text != rd["emailEmp"].ToString()) return true;
                                if (s2 != rd["rutRep"].ToString()) return true;
                                if (txtnomRep.Text != rd["nombreRep"].ToString()) return true;
                                if (s3 != rd["rutRRHH"].ToString()) return true;
                                if (txtnombreRh.Text != rd["nombreRRHH"].ToString()) return true;
                                if (txtCargo.Text != rd["cargoRRHH"].ToString()) return true;
                                if (txtmailRh.Text != rd["emailRRHH"].ToString())return true;
                                if (txtcodActividad.Text != rd["codActividad"].ToString()) return true;
                                if (dtActividad.EditValue.ToString() != rd["inicioActividades"].ToString()) return true;
                                if (txtcodAfi.EditValue.ToString() != rd["codAfiliacion"].ToString()) return false;
                                if (txtMutual.EditValue.ToString() != rd["nombreMut"].ToString()) return false;
                                if (txtnumMutual.Text != rd["codMut"].ToString())return false;
                                if (txtCajacompensacion.EditValue.ToString() != rd["nombreCCAF"].ToString()) return false;
                                if (txtCodEmpresa.Text != (string)rd["codempresa"]) return true;
                            }
                        }                        
                    }
                    //LIBERAR RECURSOS
                    cmd.Dispose();
                    rd.Close();
                    fnSistema.sqlConn.Close();
                }
                else
                {
                    XtraMessageBox.Show("No hay conexion con la base de datos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return false;
        }

        private bool DecimalValido(string pCadena)
        {
            //recorrer cadena y buscar los '.' y comas
            int c = 0;

            if (pCadena.Length == 0) return false;
            if (pCadena.Length == 1 && pCadena == ",") return false;

            for (int i = 0; i < pCadena.Length; i++)
            {
                if (pCadena[i].ToString() == ",") c++;
            }

            //SI HAY MAS DE UNA COMA NO ES VALIDO                      
            if (c > 1) return false;          

            if (pCadena.Contains(","))
            {
                //SEPARAMOS POR COMA
                char[] separador = new char[] { ',' };
                string[] cadenas = pCadena.Split(separador);

                //123,2 --> NO VALIDO
                if (cadenas[0].Length > 1) return false;
                if (cadenas[1].Length > 3) return false;
            }
            else
            {
                //ASUMIMOS QUE SOLO ES UN NUMERO
                if (pCadena.Length > 1) return false;
            }
            
            return true;
        }

        #region "LOG REGISTRO EMPRESA"
        //TABLA HASH PARA OBTENER LOS VALOR DE LA EMPRESA
        //DATO DE ENTRADA: RUT EMPRESA
        private Hashtable PrecargaEmpresa(string pRut)
        {
            Hashtable datos = new Hashtable();
            string sql = "SELECT razon, giro, direccion, ciudad, telefono, emailemp, rutrep, nombrerep, rutrrhh, " +
                "nombrerrhh, cargorrhh, emailrrhh, codactividad, inicioactividades, codafiliacion, nombremut, " +
                "codmut, cotmut, nombreccaf, comuna, cdArea, cdPais FROM EMPRESA WHERE rutEmp = @pRut";
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pRut", pRut));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //AGREGAMOS DATOS A HASH
                                datos.Add("razon", (string)rd["razon"]);
                                datos.Add("giro", (string)rd["giro"]);
                                datos.Add("direccion", (string)rd["direccion"]);
                                datos.Add("ciudad", (int)rd["ciudad"]);
                                datos.Add("telefono", (string)rd["telefono"]);
                                datos.Add("emailemp", (string)rd["emailemp"]);
                                datos.Add("rutrep", (string)rd["rutrep"]);
                                datos.Add("nombrerep", (string)rd["nombrerep"]);
                                datos.Add("rutrrhh", (string)rd["rutrrhh"]);
                                datos.Add("nombrerrhh", (string)rd["nombrerrhh"]);
                                datos.Add("cargorrhh", (string)rd["cargorrhh"]);
                                datos.Add("emailrrhh", (string)rd["emailrrhh"]);
                                datos.Add("codactividad", (string)rd["codactividad"]);
                                datos.Add("inicioactividades", (DateTime)rd["inicioactividades"]);
                                datos.Add("codafiliacion", (int)rd["codafiliacion"]);
                                datos.Add("nombremut", Convert.ToInt32(rd["nombremut"]));
                                datos.Add("codmut", rd["codmut"]);
                                datos.Add("cotmut", rd["cotmut"]);
                                datos.Add("nombreccaf", Convert.ToInt32(rd["nombreccaf"]));
                                datos.Add("comuna", Convert.ToInt32(rd["comuna"]));
                                datos.Add("codArea", (string)rd["cdArea"]);
                                datos.Add("codPais", (string)rd["cdPais"]);
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

            return datos;
        }

        //COMPARAR VALORES
        private void ComparaValorEmpresa(string pRut, Hashtable Datos, string pRazon, string pGiro, string pDireccion,
            int pCiudad, string pTelefono, string pEmailEmp, string pRutRep, string pNombreRep, string pRutRH,
            string pNombreRH, string pCargoRH, string pEmailRh, string pCodActividad, DateTime pInicioActividad,
            int pCodAfiliacion, int pNombreMut, string pCodigoMutual, decimal pCotizacionMutual, int pNombreCcaf, 
            int pComuna, string pCodArea, string pCodPais)
        {
            if (Datos.Count > 0)
            {
                if ((string)Datos["razon"] != pRazon)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA RAZON SOCIAL", "EMPRESA", (string)Datos["razon"], pRazon, "MODIFICAR");
                    log.Log();
                }
                if ((string)Datos["giro"] != pGiro)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA GIRO", "EMPRESA", (string)Datos["giro"], pGiro, "MODIFICAR");
                    log.Log();
                }
                if ((string)Datos["direccion"] != pDireccion)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA DIRECCION", "EMPRESA", (string)Datos["direccion"], pDireccion, "MODIFICAR");
                    log.Log();
                }
                if ((int)Datos["ciudad"] != pCiudad)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA CIUDAD", "EMPRESA", (int)Datos["ciudad"] + "", pCiudad + "", "MODIFICAR");
                    log.Log();
                }
                if ((string)Datos["telefono"] != pTelefono)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA TELEFONO", "EMPRESA", (string)Datos["telefono"], pTelefono, "MODIFICAR");
                    log.Log();
                }
                if ((string)Datos["emailemp"] != pEmailEmp)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICAR EMAIL", "EMPRESA", (string)Datos["emailemp"], pEmailEmp, "MODIFICAR");
                    log.Log();
                }
                if ((string)Datos["rutrep"] != pRutRep)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA RUT REPRESENTANTE LEGAL", "EMPRESA", (string)Datos["rutrep"], pRutRep, "MODIFICAR");
                    log.Log();
                }
                if ((string)Datos["nombrerep"] != pNombreRep)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA NOMBRE REPRESENTANTE LEGAL", "EMPRESA", (string)Datos["nombrerep"], pNombreRep, "MODIFICAR");
                    log.Log();
                }
                if ((string)Datos["rutrrhh"] != pRutRH )
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA RUT RRHH", "EMPRESA", (string)Datos["rutrrhh"], pRutRH, "MODIFICAR");
                    log.Log();
                }
                if ((string)Datos["nombrerrhh"] != pNombreRH)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA NOMBRE RRHH", "EMPRESA", (string)Datos["nombrerrhh"], pNombreRH, "MODIFICAR");
                    log.Log();
                }
                if ((string)Datos["cargorrhh"] != pCargoRH)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA RAZON SOCIAL", "EMPRESA", (string)Datos["cargorrhh"], pCargoRH, "MODIFICAR");
                    log.Log();
                }
                if ((string)Datos["emailrrhh"] != pEmailRh)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA EMAIL RRHH", "EMPRESA", (string)Datos["emailrrhh"], pEmailRh, "MODIFICAR");
                    log.Log();
                }
                if ((string)Datos["codactividad"] != pCodActividad)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA CODIGO ACTIVIDAD", "EMPRESA", (string)Datos["codactividad"], pCodActividad, "MODIFICAR");
                    log.Log();
                }
                if ((DateTime)Datos["inicioactividades"] != pInicioActividad)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA INICIO DE ACTIVIDADES", "EMPRESA", (DateTime)Datos["inicioactividades"] + "", pInicioActividad + "", "MODIFICAR");
                    log.Log();
                }
                if ((int)Datos["codafiliacion"] != pCodAfiliacion)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA CODIGO AFILIACION", "EMPRESA", (int)Datos["codafiliacion"] + "", pCodAfiliacion+"", "MODIFICAR");
                    log.Log();
                }
                if (Convert.ToInt32(Datos["nombremut"]) != pNombreMut)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA CODIGO MUTUAL", "EMPRESA", Convert.ToInt32(Datos["nombremut"]) + "", pNombreMut + "", "MODIFICAR");
                    log.Log();
                }
                if ((string)Datos["codmut"] != pCodigoMutual)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA RAZON SOCIAL", "EMPRESA", (string)Datos["codmut"], pCodigoMutual, "MODIFICAR");
                    log.Log();
                }
                if ((decimal)Datos["cotmut"] != pCotizacionMutual)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA COTIZACION MUTUAL", "EMPRESA", (decimal)Datos["cotmut"]+"", pCotizacionMutual+"", "MODIFICAR");
                    log.Log();
                }
                if (Convert.ToInt32(Datos["nombreccaf"]) != pNombreCcaf)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA NOMBRE CAJA", "EMPRESA", Convert.ToInt32(Datos["nombreccaf"]) + "", pNombreCcaf + "", "MODIFICAR");
                    log.Log();
                }
                if (Convert.ToInt32(Datos["comuna"]) != pComuna)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA COMUNA", "EMPRESA", Convert.ToInt32(Datos["comuna"]) + "", pComuna + "", "MODIFICAR");
                    log.Log();
                }
                if ((string)(Datos["codArea"]) != pCodArea)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA CODIGO DE AREA EMPRESA ", "EMPRESA", (string)(Datos["cdArea"]) + "", pCodArea + "", "MODIFICAR");
                    log.Log();
                }
                if ((string)(Datos["codPais"]) != pCodPais)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA CODIGO DE PAIS EMPRESA ", "EMPRESA", (string)(Datos["cdPais"]) + "", pCodPais + "", "MODIFICAR");
                    log.Log();
                }
            }
        }
        #endregion

        #endregion

        #region "CONTROLES GENERAL"  

        private void txtrutEmp_KeyPress(object sender, KeyPressEventArgs e)
        {
            //CARACTER 45-> '-'
            //CARACTER 46-> '.'
            
            if (char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }            
            else
            {
                e.Handled = true;
            }
        }

        private void txtFono_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void txtrutRep_KeyPress(object sender, KeyPressEventArgs e)
        {
            //CARACTER 45-> '-'
            //CARACTER 46-> '.'
            
            if (char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }          
            else
            {
                e.Handled = true;
            }
        }

        private void txtrutRh_KeyPress(object sender, KeyPressEventArgs e)
        {
            //CARACTER 45-> '-'
            //CARACTER 46-> '.'
           
            if (char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }           
            else
            {
                e.Handled = true;
            }
        }       

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            /*CAMPOS EN BLANCO*/
            if (txtrutEmp.Text == "") { XtraMessageBox.Show("Rut Empresa no válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);txtrutEmp.Focus();return;}
            if (txtRazon.Text == "") { XtraMessageBox.Show("Razon social no válida", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);txtRazon.Focus();return;}
            if (txtGiro.Text == "") { XtraMessageBox.Show("Giro no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtGiro.Focus();return;}
            if (txtcodActividad.Text == "") { XtraMessageBox.Show("Codigo de Actividad no valido","Error" ,MessageBoxButtons.OK, MessageBoxIcon.Error);txtcodActividad.Focus(); return;}
            if (dtActividad.EditValue.ToString() == "") { XtraMessageBox.Show("Actividad no validad", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);dtActividad.Focus();return;}
            if (txtrutRep.Text == "") { XtraMessageBox.Show("Rut representante legal no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);txtrutRep.Focus();return;}
            if (txtnomRep.Text == "") { XtraMessageBox.Show("Nombre representante legal no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);txtnomRep.Focus();return;}
            if (txtrutRh.Text == "") { XtraMessageBox.Show("Rut RRHH no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);txtrutRh.Focus();return; }
            if (txtnombreRh.Text == "") { XtraMessageBox.Show("Nombre RRHH no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);txtnombreRh.Focus();return;}

            string rEmpresa = "", rHumano = "", rRepresentante = "", ImgLogo = "";
            bool porc;

            //SI LA VARIABLE RECORTAR ES TRUE ES PORQUE SE GENERO CROP DE IMAGEN
            if (recortar)
                ImgLogo = PathLogo + "empleado_resize.jpg";
            else
            {
                //SI RECORTAR, PERO IMAGEN USUARIO ES TRUE QUIERE DECIR QUE SE CARGO AL MENOS UNA IMAGEN
                if (ImagenUsuario != null)
                {
                    ImgLogo = PathImage;
                   
                    //CONVERTIMOS LA IMAGEN DE ACUERDO A RUTA
                    Imagen.ComprimirImagen(PathImage, PathLogo + "logo.png", 90, 300, 300);
                    //GUARDAMOS NUEVA RUTA DEL ARCHIVO GENERADO
                    ImgLogo = PathLogo + "logo.png";
                }                    
            }

            //SI VAR DEFAULTRUTEMP == "999999999" ES PORQUE LA TABLA TIENE EL REGISTRO POR DEFECTO
            if (txtrutEmp.ReadOnly || DefaultEmpresa)
            {
                //UPDATE               
                //SI EL FORMATO ES CORRECTO (11.111.111-K)
                //QUITAMOS LOS PUNTOS Y GUION CORRESPONDIENTES
                rRepresentante = fnSistema.fnExtraerCaracteres(txtrutRep.Text);
                rHumano = fnSistema.fnExtraerCaracteres(txtrutRh.Text);

                //SI LA CADENA SIN CARACTERES RAROS ES MENOR A 8 NO ES VALIDA
                if (rRepresentante.Length < 8 || rRepresentante.Length>9) { XtraMessageBox.Show("rut representante no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);return;}
                if (rHumano.Length<8 || rHumano.Length>9) { XtraMessageBox.Show("rut RRHH no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

                //ENMASCARAR RUT ANTES DE GUARDAR
                string r1 = fnSistema.fEnmascaraRut(rHumano);
                string r2 = fnSistema.fEnmascaraRut(rRepresentante);
                //MessageBox.Show(r2);

                bool rutRepresentante = fnSistema.fValidaRut(r2);
                //SI ES TRUE ES VALIDO
                if (rutRepresentante == false) { XtraMessageBox.Show("Rut Representante Legal no valido", "Representante legal", MessageBoxButtons.OK, MessageBoxIcon.Error); txtrutRep.Focus(); return; }

                bool rutRh = fnSistema.fValidaRut(r1);
                //SI ES TRUE ES VALIDO
                if (rutRh == false) { XtraMessageBox.Show("Rut RRHH no valido", "RRHH", MessageBoxButtons.OK, MessageBoxIcon.Error); txtrutRh.Focus(); return; }

                bool mail1 = fnSistema.fnValidaCorreo(txtmailEmp.Text);
                //SI ES TRUE ES VALIDO
                if (mail1 == false) { XtraMessageBox.Show("Correo empresa no es valido", "Correo", MessageBoxButtons.OK, MessageBoxIcon.Error); txtmailEmp.Focus(); return; }

                bool mail2 = fnSistema.fnValidaCorreo(txtmailRh.Text);
                //SI ES TRUE ES VALIDO
                if (mail2 == false) { XtraMessageBox.Show("Correo RRHH no valido", "Correo", MessageBoxButtons.OK, MessageBoxIcon.Error); txtmailRh.Focus(); return; }              

                //VALIDAR CAMPO COTIZACION MUTUAL SIEMPRE Y CUANDO ESTE SELECCIONADO LA OPCION
                //PREGUNTAR QUE OPCION ESTA SELECCIONADA EN EL COMBO AFILIACION
                if (txtcodAfi.EditValue.ToString() == "1")
                {
                    //SOLO INP
                    //NO SE VALIDA NADA
                }
                else if (txtcodAfi.EditValue.ToString() == "2")
                {
                    //SOLO CAJA
                    //VALIDAR CAMPO NOMBRE ASIGNACION FAMILIAR (CCAF)
                    if (txtCajacompensacion.ItemIndex == 0) { XtraMessageBox.Show("Debe ingresar el nombre de la caja de compensacion", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);txtCajacompensacion.Focus();return;}
                }
                else if (txtcodAfi.EditValue.ToString() == "3")
                {
                    // CAJA Y MUTUAL
                    //SE VALIDAN TODOS LOS CAMPOS
                    //VALIDAR NOMBRE MUTUAL
                    if (txtMutual.ItemIndex == 0) { XtraMessageBox.Show("Debes ingresar el nombre de la mutual", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);txtMutual.Focus();return;}
                    if (txtnumMutual.Text == "") { XtraMessageBox.Show("Numero mutual no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);txtnumMutual.Focus();return;}
                    //validar porcentaje en campo cotizacion mutual
                    if (txtcotMutual.Text == "") { XtraMessageBox.Show("Porcentaje de cotizacion no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);txtcotMutual.Focus();return;}
                    porc = DecimalValido(txtcotMutual.Text);
                    //SI RETORNA TRUE LA VARIABLE PORC ES UN VALOR VALIDO
                    if (porc == false) { XtraMessageBox.Show("Porcentaje de cotizacion no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);txtcotMutual.Focus();return;}
                    if (txtCajacompensacion.ItemIndex == 0) { XtraMessageBox.Show("Debe ingresar el nombre de la caja de compensacion de asignacion Familiar", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtCajacompensacion.Focus(); return;}
                }
                else
                {
                    //SOLO MUTUAL
                    if (txtMutual.ItemIndex == 0) { XtraMessageBox.Show("Debes ingresar el nombre de la mutual", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtMutual.Focus(); return; }
                    if (txtnumMutual.Text == "") { XtraMessageBox.Show("Numero mutual no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtnumMutual.Focus(); return; }
                    //validar porcentaje en campo cotizacion mutual
                    if (txtcotMutual.Text == "") { XtraMessageBox.Show("Porcentaje de cotizacion no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtcotMutual.Focus(); return; }
                    porc = DecimalValido(txtcotMutual.Text);
                    //SI RETORNA TRUE LA VARIABLE PORC ES UN VALOR VALIDO
                    if (porc == false) { XtraMessageBox.Show("Porcentaje de cotizacion no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtcotMutual.Focus(); return; }
                }

                //llamamos a la funcion actualizar
                fnModificarEmpresa(txtRazon,txtrutEmp, txtGiro, txtDom, txtCiudad, txtFono, txtmailEmp,r2 ,
                    txtnomRep, r1, txtnombreRh, txtCargo, txtmailRh, txtcodActividad, dtActividad,txtcodAfi,txtMutual,
                    txtnumMutual, txtcotMutual, txtCajacompensacion, ImgLogo, txtCodEmpresa.Text, RutEmpDb, 
                    txtComuna, txtcodPais, txtCodArea);                
            }
            else
            {
                //INSERT                

                //SI EL FORMATO ES CORRECTO (11.111.111-K)
                //QUITAMOS LOS PUNTOS Y GUION CORRESPONDIENTES
                rRepresentante = fnSistema.fnExtraerCaracteres(txtrutRep.Text);
                rHumano = fnSistema.fnExtraerCaracteres(txtrutRh.Text);
                rEmpresa = fnSistema.fnExtraerCaracteres(txtrutEmp.Text);

                //SI LA CADENA SIN CARACTERES RAROS ES MENOR A 8 NO ES VALIDA
                if (rRepresentante.Length < 8 || rRepresentante.Length > 9) { XtraMessageBox.Show("rut representante no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                if (rHumano.Length < 8 || rHumano.Length > 9) { XtraMessageBox.Show("rut RRHH no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                if (rEmpresa.Length < 8 || rEmpresa.Length > 9) { XtraMessageBox.Show("rut empresa no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

                //ENMASCARAR RUT ANTES DE INGRESAR
                string r1 = fnSistema.fEnmascaraRut(rHumano);
                string r2 = fnSistema.fEnmascaraRut(rRepresentante);
                string r3 = fnSistema.fEnmascaraRut(rEmpresa);

                bool rutEmpresa = fnSistema.fValidaRut(r3);
                //SI ES TRUE ES VALIDO
                if (rutEmpresa == false) { XtraMessageBox.Show("Rut Empresa no valido", "Rut Empresa", MessageBoxButtons.OK, MessageBoxIcon.Error); txtrutEmp.Focus(); return; }

                bool rutRepresentante = fnSistema.fValidaRut(r2);
                //SI ES TRUE ES VALIDO
                if (rutRepresentante == false) { XtraMessageBox.Show("Rut Representante Legal no valido", "Representante legal", MessageBoxButtons.OK, MessageBoxIcon.Error); txtrutRep.Focus(); return; }

                bool rutRh = fnSistema.fValidaRut(r1);
                //SI ES TRUE ES VALIDO
                if (rutRh == false) { XtraMessageBox.Show("Rut RRHH no valido", "RRHH", MessageBoxButtons.OK, MessageBoxIcon.Error); txtrutRh.Focus(); return; }

                bool mail1 = fnSistema.fnValidaCorreo(txtmailEmp.Text);
                //SI ES TRUE ES VALIDO
                if (mail1 == false) { XtraMessageBox.Show("Correo empresa no es valido", "Correo", MessageBoxButtons.OK, MessageBoxIcon.Error); txtmailEmp.Focus(); return; }

                bool mail2 = fnSistema.fnValidaCorreo(txtmailRh.Text);
                //SI ES TRUE ES VALIDO
                if (mail2 == false) { XtraMessageBox.Show("Correo RRHH no valido", "Correo", MessageBoxButtons.OK, MessageBoxIcon.Error); txtmailRh.Focus(); return; }                

                //PREGUNTAR QUE OPCION ESTA SELECCIONADA EN EL COMBO AFILIACION
                if (txtcodAfi.EditValue.ToString() == "1")
                {
                    //SOLO INP
                    //NO SE VALIDA NADA
                }
                else if (txtcodAfi.EditValue.ToString() == "2")
                {
                    //SOLO CAJA
                    //VALIDAR CAMPO NOMBRE ASIGNACION FAMILIAR (CCAF)
                    if (txtCajacompensacion.ItemIndex == 0) { XtraMessageBox.Show("Debe ingresar el nombre de la caja de compensacion", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtCajacompensacion.Focus(); return; }
                }
                else if (txtcodAfi.EditValue.ToString() == "3")
                {
                    // CAJA Y MUTUAL
                    //SE VALIDAN TODOS LOS CAMPOS
                    //VALIDAR NOMBRE MUTUAL
                    if (txtMutual.ItemIndex == 0) { XtraMessageBox.Show("Debes ingresar el nombre de la mutual", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtMutual.Focus(); return; }
                    if (txtnumMutual.Text == "") { XtraMessageBox.Show("Numero mutual no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtnumMutual.Focus(); return; }
                    //validar porcentaje en campo cotizacion mutual
                    if (txtcotMutual.Text == "") { XtraMessageBox.Show("Porcentaje de cotizacion no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtcotMutual.Focus(); return; }
                    porc = DecimalValido(txtcotMutual.Text);
                    //SI RETORNA TRUE LA VARIABLE PORC ES UN VALOR VALIDO
                    if (porc == false) { XtraMessageBox.Show("Porcentaje de cotizacion no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtcotMutual.Focus(); return; }
                    if (txtCajacompensacion.ItemIndex == 0) { XtraMessageBox.Show("Debe ingresar el nombre de la caja de compensacion", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtCajacompensacion.Focus(); return; }
                }
                else
                {
                    //SOLO MUTUAL
                    if (txtMutual.ItemIndex == 0) { XtraMessageBox.Show("Debes ingresar el nombre de la mutual", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtMutual.Focus(); return; }
                    if (txtnumMutual.Text == "") { XtraMessageBox.Show("Numero mutual no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtnumMutual.Focus(); return; }
                    //validar porcentaje en campo cotizacion mutual
                    if (txtcotMutual.Text == "") { XtraMessageBox.Show("Porcentaje de cotizacion no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtcotMutual.Focus(); return; }
                    porc = DecimalValido(txtcotMutual.Text);
                    //SI RETORNA TRUE LA VARIABLE PORC ES UN VALOR VALIDO
                    if (porc == false) { XtraMessageBox.Show("Porcentaje de cotizacion no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtcotMutual.Focus(); return; }
                }

                //GUARDAMOS REGISTRO EN BD
                fnNuevaEmpresa(txtRazon, r3, txtGiro, txtDom, txtCiudad, txtFono, txtmailEmp,r2, txtnomRep,
                    r1, txtnombreRh, txtCargo, txtmailRh, txtcodActividad, dtActividad, txtcodAfi, txtMutual,
                    txtnumMutual, txtcotMutual, txtCajacompensacion, ImgLogo, txtCodEmpresa.Text, txtComuna, 
                    txtcodPais, txtCodArea);
            }           
        }        

        private void txtmailEmp_KeyPress(object sender, KeyPressEventArgs e)
        {
            //PERMITIR LETRAS ESPACIO @ _ - .
            /*
             * CODE 45 --> '.'
             * CODE 46 --> '-'
             * CODE 64 --> '@'
             * CODE 95 --> '_'
             */
            if (e.KeyChar == (char)45)
            {
                e.Handled = false;
            }
            else if (e.KeyChar == (char)46)
            {
                e.Handled = false;
            }
            else if (e.KeyChar == (char)64)
            {
                e.Handled = false;
            }
            else if (e.KeyChar == (char)95)
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
            else if (char.IsLetter(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void txtmailRh_KeyPress(object sender, KeyPressEventArgs e)
        {
            //PERMITIR LETRAS ESPACIO @ _ - .
            /*
             * CODE 45 --> '.'
             * CODE 46 --> '-'
             * CODE 64 --> '@'
             * CODE 95 --> '_'
             */
            if (e.KeyChar == (char)45)
            {
                e.Handled = false;
            }
            else if (e.KeyChar == (char)46)
            {
                e.Handled = false;
            }
            else if (e.KeyChar == (char)64)
            {
                e.Handled = false;
            }
            else if (e.KeyChar == (char)95)
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
            else if (char.IsLetter(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void txtcodActividad_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar))
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

        private void txtrutEmp_KeyDown(object sender, KeyEventArgs e)
        {
            bool valida = false;
            bool rutValido;
            //if (txtrutEmp.Text =="" ) return;
            string cadena = "";

            if (e.KeyCode == Keys.Enter)
            {
                if (txtrutEmp.Text == "") { lblrutEmp.Visible = true;lblrutEmp.Text = "rut no valido";return; }
                if(txtrutEmp.Text.Length<8 || txtrutEmp.Text.Length>9) { lblrutEmp.Visible = true; lblrutEmp.Text = "rut no valido"; return; }
                if (txtrutEmp.Text.Contains(".") || txtrutEmp.Text.Contains("-"))
                {
                    cadena = fnSistema.fnExtraerCaracteres(txtrutEmp.Text);
                }
                else
                {
                    cadena = txtrutEmp.Text;
                }
                //ENMASCARAMOS ANTES DE VALIDAR
                cadena = fnSistema.fEnmascaraRut(cadena);
                rutValido = fnSistema.fValidaRut(cadena);
                if (rutValido == false)
                {
                    //RUT NO VALIDO
                    lblrutEmp.Visible = true;
                    lblrutEmp.Text = "rut no valido";
                    //FORMATEAMOS CADENA
                    txtrutEmp.Text = fnSistema.fFormatearRut2(cadena);
                }
                else
                {
                    //FORMATEAMOS CADENA
                    txtrutEmp.Text = fnSistema.fFormatearRut2(cadena);
                    lblrutEmp.Text = "";
                }


            }


        }

        private void txtrutRep_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Back) return;
            bool valida = false;
            bool rutValido;
            //if (txtrutEmp.Text =="" ) return;
            string cadena = "";

            if (e.KeyCode == Keys.Enter)
            {
                if (txtrutRep.Text == "") { lblrutRep.Visible = true;lblrutRep.Text = "rut no valido";return;}
                if (txtrutRep.Text.Length < 8 || txtrutRep.Text.Length > 9) { lblrutRep.Visible = true; lblrutRep.Text = "rut no valido"; return; }
                if (txtrutRep.Text.Contains(".") || txtrutRep.Text.Contains("-"))
                {
                    cadena = fnSistema.fnExtraerCaracteres(txtrutRep.Text);
                }
                else
                {
                    cadena = txtrutRep.Text;
                }
                //ENMASCARAMOS ANTES DE VALIDAR
                cadena = fnSistema.fEnmascaraRut(cadena);
                rutValido = fnSistema.fValidaRut(cadena);
                if (rutValido == false)
                {
                    //RUT NO VALIDO
                    lblrutRep.Visible = true;
                    lblrutRep.Text = "rut no valido";
                    //FORMATEAMOS CADENA
                    txtrutRep.Text = fnSistema.fFormatearRut2(cadena);
                }
                else
                {
                    //FORMATEAMOS CADENA
                    txtrutRep.Text = fnSistema.fFormatearRut2(cadena);
                    
                }
               
               
            }

            

            
        }      
        private void txtrutEmp_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
           
        }

        private void txtrutRh_KeyDown(object sender, KeyEventArgs e)
        {
            bool valida = false;
            bool rutValido;
            //if (txtrutEmp.Text =="" ) return;
            string cadena = "";

            if (e.KeyCode == Keys.Enter)
            {
                if (txtrutRh.Text == "") { lblrutRh.Visible = true;lblrutRh.Text = "rut no valido";return;};
                if (txtrutRh.Text.Length < 8 || txtrutRh.Text.Length > 9) { lblrutRh.Visible = true; lblrutRh.Text = "rut no valido"; return; }
                if (txtrutRh.Text.Contains(".") || txtrutRh.Text.Contains("-"))
                {
                    cadena = fnSistema.fnExtraerCaracteres(txtrutRh.Text);
                }
                else
                {
                    cadena = txtrutRh.Text;
                }
                //ENMASCARAMOS ANTES DE VALIDAR
                cadena = fnSistema.fEnmascaraRut(cadena);
                rutValido = fnSistema.fValidaRut(cadena);
                if (rutValido == false)
                {
                    //RUT NO VALIDO
                    lblrutRh.Visible = true;
                    lblrutRh.Text = "rut no valido";
                    //FORMATEAMOS CADENA
                    txtrutRh.Text = fnSistema.fFormatearRut2(cadena);
                }
                else
                {
                    //FORMATEAMOS CADENA
                    txtrutRh.Text = fnSistema.fFormatearRut2(cadena);
                    lblrutRh.Text = "";
                }


            }
        }

        private void txtmailEmp_KeyDown(object sender, KeyEventArgs e)
        {
            bool validaMail;
            //VALIDAR CORREO ELECTRONICO SI SE PRESIONA LA TECLA ENTER
            if (e.KeyCode == Keys.Enter)
            {
                validaMail = fnSistema.fnValidaCorreo(txtmailEmp.Text);
                if (validaMail == false)
                {
                    XtraMessageBox.Show("Email empresa no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtmailEmp.EnterMoveNextControl = false;
                }
                else
                {
                    //CORREO ELECTRONICO VALIDO
                    txtmailEmp.EnterMoveNextControl = true;
                }
            }
        }

        private void txtmailRh_KeyDown(object sender, KeyEventArgs e)
        {
            bool validaMail;
            //VALIDAR CORREO ELECTRONICO SI SE PRESIONA LA TECLA ENTER
            if (e.KeyCode == Keys.Enter)
            {
                validaMail = fnSistema.fnValidaCorreo(txtmailRh.Text);
                if (validaMail == false)
                {
                    XtraMessageBox.Show("Email RRHH no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtmailRh.EnterMoveNextControl = false;
                }
                else
                {
                    //CORREO ELECTRONICO VALIDO
                    txtmailRh.EnterMoveNextControl = true;
                }
            }
        }
        #endregion

        #region "LEGALES"
        //FUNCION PARA INGREOSO DATOS LEGALES
        private void fnUpdateLegales(string prutEmp, LookUpEdit pcodAfi, TextEdit pnomMut, TextEdit pnumMut, TextEdit pcotMut,
            TextEdit pAsignacion)
        {
            //QUERY SQL
            string sql = "UPDATE empresa SET " +
                "codAfiliacion=@pcodAfi, nombreMut=@pnomMut, codMut=@pnumMut, cotMut=@pcotMut," +
                "nombreCCAF=@pAsignacion" +
                " WHERE rutEmp=@prutEmp";

            /*
             * CAMPOS NUMERICOS
             * -------------------------------
             * COD MUTUAL (numero mutual)     |
             * COT MUTUAL (cotizacion mutual) |
             * -------------------------------
             */

            SqlCommand cmd;
            int res = 0;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pcodAfi", pcodAfi.EditValue));
                        cmd.Parameters.Add(new SqlParameter("@pnomMut", pnomMut.Text));
                        cmd.Parameters.Add(new SqlParameter("@pnumMut", pnumMut.Text));
                        //LA COTIZACION MUTUAL ES UN NUMERO DECIMAL
                        cmd.Parameters.Add(new SqlParameter("@pcotMut",decimal.Parse(pcotMut.Text)));
                        cmd.Parameters.Add(new SqlParameter("@pAsignacion", pAsignacion.Text));
                        cmd.Parameters.Add(new SqlParameter("@prutEmp", prutEmp));
                        //ejecutamos consulta
                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            XtraMessageBox.Show("Registro Actualizado", "Actualizar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            XtraMessageBox.Show("Error al intentar actualizar", "Actualizar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    //LIBERAMOS RECURSOS
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                }
                else
                {
                    XtraMessageBox.Show("No hay conexion a base de datos", "Base de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        //DEFAULT PROPERTIES
        private void fndefaultLegales()
        {
            /*txtcodAfi.Properties.MaxLength = 5;
            txtnomMut.Properties.MaxLength = 100;
            txtnumMut.Properties.MaxLength = 100;
            txtcotMut.Properties.MaxLength = 5;
            txtAsignacion.Properties.MaxLength = 100;           
            btnGuardar.AllowFocus = false;
            separadorLegal.TabStop = false;
            panelLegal.TabStop = false;
            txtcodAfi.EditValue = 1;*/
        }

        //CARGAR CAMPOS LEGALES
        private void fnCargarLegales(string prutEmp)
        {            
            //QUERY SQL
            string sql = "SELECT codAfiliacion, nombreMut, codMut, cotMut, nombreCCAF" +
                " FROM empresa WHERE rutEmp=@prutEmp";
            
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@prutEmp", prutEmp));                      
                        rd = cmd.ExecuteReader();                        
                        if (rd.HasRows)
                        {                        
                            while (rd.Read())
                            {
                                //CARGAR CAMPOS
                                txtcodAfi.EditValue = (int)rd["codAfiliacion"];                                
                                //txtnomMut.Text = (string)rd["nombreMut"];
                                //txtnumMut.Text = (string)rd["codMut"];
                                //txtcotMut.Text = ((decimal)rd["cotMut"]).ToString();
                                //txtAsignacion.Text = (string)rd["nombreCCAF"];
                            }
                        }
                        else
                        {
                            XtraMessageBox.Show("No hay filas");
                        }
                    }
                    //LIBERAR RECURSOS
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                    rd.Close();
                }
                else
                {
                    XtraMessageBox.Show("no hay conexion con la base de datos", "Base de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

        }

        //COMBO AFILIACION
        /*
         * 1 --> SOLO INP
         * 2 --> SOLO CAJA
         * 3 --> CAJA Y MUTUAL
         */
        private void fnComboAfiliacion()
        {
            List<datoCombobox> lista = new List<datoCombobox>();

            lista.Add(new datoCombobox() {KeyInfo = 1, descInfo = "SOLO INP"});
            lista.Add(new datoCombobox() { KeyInfo = 2, descInfo = "SOLO CAJA" });
            lista.Add(new datoCombobox() { KeyInfo = 3, descInfo = "CAJA Y MUTUAL"});
            lista.Add(new datoCombobox() { KeyInfo = 4, descInfo = "SOLO MUTUAL" });

            txtcodAfi.Properties.DataSource = lista.ToList();
            txtcodAfi.Properties.ValueMember = "KeyInfo";
            txtcodAfi.Properties.DisplayMember = "descInfo";

            txtcodAfi.Properties.PopulateColumns();

            //OCULTAMOS KEY
            txtcodAfi.Properties.Columns[0].Visible = false;

            txtcodAfi.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
            txtcodAfi.Properties.AutoSearchColumnIndex = 1;
            txtcodAfi.Properties.ShowHeader = false;
            txtcodAfi.ItemIndex = 0;
        }

        //VERIFICAR CAMBIOS EN CAJA
        private bool fnCambiosLegal(string pRut)
        {
            //QUERY SQL
           /* string sql = "SELECT codAfiliacion, nombreMut, codMut, cotMut, nombreCCAF" +
                " FROM empresa WHERE rutEmp=@pRut";

            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pRut", pRut));
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //COMPARAMOS
                                if (txtcodAfi.EditValue.ToString() != rd["codAfiliacion"].ToString()) return true;
                                if (txtnomMut.Text != rd["nombreMut"].ToString()) return true;
                                if (txtnumMut.Text != rd["codMut"].ToString()) return true;
                                if (txtcotMut.Text != rd["cotMut"].ToString()) return true;
                                if (txtAsignacion.Text != rd["nombreCCAF"].ToString()) return true;
                            }
                        }
                    }
                    //LIBERAR RECURSOS
                    cmd.Dispose();
                    rd.Close();
                    fnSistema.sqlConn.Close();
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
            */
            return false;
        }

        #endregion

        #region "CONTROLES LEGALES"        

        private void tabMain_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            /*if (tabMain.SelectedTabPage.Equals(tabGeneral))
            {
                //CODE
                //PESTAÑA GENERAL
                //CARGAR COMBO CIUDAD
                datoCombobox.spllenaComboBox("SELECT idCiudad, descCiudad FROM ciudad", txtCiudad, "idCiudad", "descCiudad", true);
                //PROPIEDADES POR DEFECTO
                fnDefaultProperties();
                //cargamos campos
                string[] registros = fnRegistros();
                if (registros != null)
                {
                    actual = registros[pos];
                    fnCargarEmpresa(registros[pos]);
                }
            }*/
            /*else if (tabMain.SelectedTabPage.Equals(tabLegales))
            {
                //CODE
                //PESTAÑA LEGALES
                fndefaultLegales();
                fnComboAfiliacion();
                //CARGAR CAMPOS ACTUALES 
                if(actual != "")
                fnCargarLegales(actual);               

            }*/
        }
        

        //SOLO NUMEROS PARA EL CAMPO NUMERO MUTUAL       

        private void tabMain_Deselecting(object sender, DevExpress.XtraTab.TabPageCancelEventArgs e)
        {
            //OBTENER LA PAGINA ACTUAL
            string page = e.Page.Text;
            bool cambio;
            if (page == "General")
            {
                //CODE
                if (actual != "") {
                   cambio = fnVericarCambios(actual);
                    //SI CAMBIO ES TRUE ES PORQUE SE MODIFICO ALGUN CAMPO
                    if (cambio)
                    {
                        DialogResult dialogo = XtraMessageBox.Show("¿Desea cambiar de pestaña de todas maneras?", "Cambios sin Guardar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dialogo == DialogResult.No)
                        {
                            //NO DEJAMOS CAMBIAR DE PESTAÑA
                            e.Cancel = true;
                        }
                    }
                }
                
            }
            else if (page == "Datos Legales")
            {
                //CODE
                if (actual != "")
                {
                    cambio = fnCambiosLegal(actual);
                    //SI CAMBIO ES TRUE ES PORQUE SE MODIFICO ALGUN CAMPO
                    if (cambio)
                    {
                        DialogResult dialogo = XtraMessageBox.Show("¿Desea cambiar de pestaña de todas maneras?", "Cambios sin Guardar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dialogo == DialogResult.No)
                        {
                            //NO DEJAMOS CAMBIAR DE PESTAÑA
                            e.Cancel = true;
                        }
                    }
                }
            }
        }        

        protected override bool ProcessDialogKey(Keys keyData)
        {
           
            bool validaCorreo;
            bool rutValido;
            //if (txtrutEmp.Text =="" ) return;
            string cadena = "";
            if (keyData == Keys.Tab)
                {
                //SI EL FOCO ESTA EN EL CAMPO RUT REPRESENTANTE
                if (txtrutRep.ContainsFocus)
                    {
                    if (txtrutRep.Text == "") { lblrutRep.Visible = true;lblrutRep.Text = "rut no valido";return false;}                   
                    if (txtrutRep.Text.Contains(".") || txtrutRep.Text.Contains("-"))
                    {
                        cadena = fnSistema.fnExtraerCaracteres(txtrutRep.Text);
                    }
                    else
                    {
                        cadena = txtrutRep.Text;
                    }
                    if (cadena.Length < 8 || cadena.Length > 9)
                    {
                        lblrutRep.Visible = true;
                        lblrutRep.Text = "rut no valido";
                        txtrutRep.Text=fnSistema.fFormatearRut2(cadena) ;
                    }
                    else
                    {
                        //CADENA VALIDA
                        //ENMASCARAMOS ANTES DE VALIDAR
                        cadena = fnSistema.fEnmascaraRut(cadena);
                        rutValido = fnSistema.fValidaRut(cadena);
                        if (rutValido == false)
                        {
                            //RUT NO VALIDO
                            lblrutRep.Visible = true;
                            lblrutRep.Text = "rut no valido";
                            //FORMATEAMOS CADENA
                            txtrutRep.Text = fnSistema.fFormatearRut2(cadena);
                        }
                        else
                        {
                            //FORMATEAMOS CADENA
                            txtrutRep.Text = fnSistema.fFormatearRut2(cadena);
                            lblrutRep.Text = "";
                        }
                    }
                  

                }

                //SI EL FOCO ESTA EN EL CAMPO RUT RRHH              
                if (txtrutRh.ContainsFocus)
                {
                    if (txtrutRh.Text == "") { lblrutRh.Visible = true; lblrutRh.Text = "rut no valido"; return false; }                    
                    if (txtrutRh.Text.Contains(".") || txtrutRh.Text.Contains("-"))
                    {
                        cadena = fnSistema.fnExtraerCaracteres(txtrutRh.Text);
                    }
                    else
                    {
                        cadena = txtrutRh.Text;
                    }

                    if (cadena.Length < 8 || cadena.Length > 9)
                    {
                        lblrutRh.Visible = true;
                        lblrutRh.Text = "rut no valido";
                        //FORMATEAR CADENA
                        txtrutRh.Text = fnSistema.fFormatearRut2(cadena);

                    }
                    else
                    {
                        //CADENA CORRECTA
                        //ENMASCARAMOS ANTES DE VALIDAR
                        cadena = fnSistema.fEnmascaraRut(cadena);
                        rutValido = fnSistema.fValidaRut(cadena);
                        if (rutValido == false)
                        {
                            //RUT NO VALIDO
                            lblrutRh.Visible = true;
                            lblrutRh.Text = "rut no valido";
                            //FORMATEAMOS CADENA
                            txtrutRh.Text = fnSistema.fFormatearRut2(cadena);
                        }
                        else
                        {
                            //FORMATEAMOS CADENA
                            txtrutRh.Text = fnSistema.fFormatearRut2(cadena);                        
                            lblrutRh.Text = "";
                        }
                    }                  

                }

                //SI EL FOCO ESTA EN EL CAMPO RUT RRHH              
                if (txtrutEmp.ContainsFocus)
                {
                    if (txtrutEmp.Text == "") { lblrutEmp.Visible = true; lblrutEmp.Text = "rut no valido"; return false; }                    
                    if (txtrutEmp.Text.Contains(".") || txtrutRh.Text.Contains("-"))
                    {
                        cadena = fnSistema.fnExtraerCaracteres(txtrutEmp.Text);
                    }
                    else
                    {
                        cadena = txtrutEmp.Text;
                    }

                    if (cadena.Length < 8 || cadena.Length > 9)
                    {
                        lblrutEmp.Visible = true;
                        lblrutEmp.Text = "rut no valido";
                        txtrutEmp.Text=fnSistema.fFormatearRut2(cadena);
                    }
                    else
                    {
                        //CADENA VALIDA
                        //ENMASCARAMOS ANTES DE VALIDAR
                        cadena = fnSistema.fEnmascaraRut(cadena);
                        rutValido = fnSistema.fValidaRut(cadena);
                        if (rutValido == false)
                        {
                            //RUT NO VALIDO
                            lblrutEmp.Visible = true;
                            lblrutEmp.Text = "rut no valido";
                            //FORMATEAMOS CADENA
                            txtrutEmp.Text = fnSistema.fFormatearRut2(cadena);
                        }
                        else
                        {
                            //FORMATEAMOS CADENA
                            txtrutEmp.Text = fnSistema.fFormatearRut2(cadena);
                            lblrutEmp.Text = "";
                        }
                    }                  
                }

                //SI EL FOCO ESTA EN CAMPO CORREO
                if (txtmailEmp.ContainsFocus)
                {
                    //VALIDAMOS CORREO ELECTRONICO EMPRESA
                    if (txtmailEmp.Text == "") { XtraMessageBox.Show("correo empresa no valido", "Correo Electronico", MessageBoxButtons.OK, MessageBoxIcon.Error); return false;}
                    validaCorreo = fnSistema.fnValidaCorreo(txtmailEmp.Text);
                    //SI VALIDACORREO ES TRUE EL CORREO EL VALIDO
                    if (validaCorreo == false)
                    {
                        XtraMessageBox.Show("Correo empresa no valido", "Correo Electronico", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }                    
                }

                //SI EL FOCO ESTA EN EL CAMPO RUT RRHH
                if (txtmailRh.ContainsFocus)
                {
                    //VALIDAMOS CORREO ELECTRONICO EMPRESA
                    if (txtmailRh.Text == "") { XtraMessageBox.Show("correo RRHH no valido", "Correo Electronico", MessageBoxButtons.OK, MessageBoxIcon.Error); return false; }
                    validaCorreo = fnSistema.fnValidaCorreo(txtmailRh.Text);
                    //SI VALIDACORREO ES TRUE EL CORREO EL VALIDO
                    if (validaCorreo == false)
                    {
                        XtraMessageBox.Show("Correo RRHH no valido", "Correo Electronico", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
                }            
            
            return base.ProcessDialogKey(keyData);
        }
        #endregion

        private void txtcotMutual_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)44)
            {
                e.Handled = false;
            }
            else if (char.IsDigit(e.KeyChar))
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

        private void txtnumMutual_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || e.KeyChar == (char)45)
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

        private void txtcodAfi_EditValueChanged(object sender, EventArgs e)
        {
            //SI ES SOLO INP SE DESABILITAN LOS CAMPOS 
            if (txtcodAfi.EditValue.ToString() == "1")
            {
                //SOLO INP
                txtMutual.Enabled = false;
                txtnumMutual.Enabled = false;
                txtcotMutual.Enabled = false;
                txtCajacompensacion.Enabled = false;
                //DEJAR EN BLANCO TODOS LOS CAMPOS
                txtMutual.ItemIndex = 0;
                txtnumMutual.Text = "";
                txtcotMutual.Text = "0";
                txtCajacompensacion.ItemIndex = 0;
            }
            else if (txtcodAfi.EditValue.ToString() == "2")
            {
                //SOLO CAJA
                txtMutual.Enabled = false;
                txtnumMutual.Enabled = false;
                txtcotMutual.Enabled = false;
                txtCajacompensacion.Enabled = true;
                txtMutual.ItemIndex = 0;
                txtnumMutual.Text = "";
                txtcotMutual.Text = "0";                     

            }
            else if (txtcodAfi.EditValue.ToString() == "3")
            {
                //CAJA Y MUTUAL
                txtMutual.Enabled = true;
                txtMutual.ItemIndex = 0;
                txtnumMutual.Enabled = true;
                txtcotMutual.Enabled = true;
                txtCajacompensacion.Enabled = true;
                txtCajacompensacion.ItemIndex = 0;
            }
            else
            {
                //SOLO MUTUAL
                txtMutual.Enabled = true;
                txtMutual.ItemIndex = 0;
                txtnumMutual.Enabled = true;
                txtcotMutual.Enabled = true;
                txtCajacompensacion.Enabled = false;
                txtCajacompensacion.ItemIndex = 0;
            }
        }

        private void txtrutEmp_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtRazon_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtGiro_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtcodActividad_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void dtActividad_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtDom_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtCiudad_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtFono_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtmailEmp_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtnomRep_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtrutRep_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtnombreRh_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtrutRh_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtCargo_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtmailRh_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtcodAfi_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtnomMutual_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtnumMutual_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtcotMutual_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtAsignacion_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            if (txtrutEmp.Text != "")
            {
                string rut = txtrutEmp.Text;
                rut = fnSistema.fnExtraerCaracteres(rut);                

                if (fnVericarCambios(rut))
                {
                    DialogResult cambios = XtraMessageBox.Show("Hay cambios sin guardar, ¿Deseas realmente salir?", "Cambios sin guardar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (cambios == DialogResult.Yes)
                    {
                        Close();
                    }
                }
                else
                {
                    Close();
                }
            }
            else
            {
                Close();
            }
        }

        private void btnImagen_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            /*ABRIMOS VENTANA PARA QUE EL USUARIO CARGUE UNA IMAGEN*/
            if (Imagen.GenerarImagenFromUser() != null)
            {
                //GENERAMOS IMAGEN EN BASE A IMAGEN VARGADA
                ImagenUsuario = Imagen.imagen;

                //GUARDAMOS LA RUTA DEL ARCHIVO
                PathImage = Imagen.PathFile;                

                /*CARGAMOS IMAGEN EN PICTURE BOX*/
                Imagen.CargarImagen(ImagenUsuario, pictureLogo);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            recortar = false;
            btnRecortar.Enabled = false;

            if (pictureLogo.Image != null && ImagenUsuario != null)
                Imagen.CargarImagen(ImagenUsuario, pictureLogo);
            else
                XtraMessageBox.Show("No hay imagen", "informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        private void pictureLogo_MouseDown(object sender, MouseEventArgs e)
        {
            if (ImagenUsuario == null) return;

            //NO HACEMOS NADA SI SE HACE CLICK CON EL BOTON DERECHO
            if (e.Button == MouseButtons.Right) return;

            Cursor = Cursors.Cross;

            MouseClicked = true;

            //GUARDAMOS LAS POSICIONES INICIALES
            startpoint.X = e.X;
            startpoint.Y = e.Y;

            endpoint.X = e.X - 1;
            endpoint.Y = e.Y - 1;

            //DIBUJAMOS AREA 
            Rectangulo = new Rectangle(new Point(e.X, e.Y), new Size());
        }

        private void pictureLogo_MouseMove(object sender, MouseEventArgs e)
        {
            if (ImagenUsuario == null) return;

            Point ptCurrent = new Point(e.X, e.Y);

            if (MouseClicked && e.Button == MouseButtons.Left)
            {
                endpoint = ptCurrent;

                if (e.X > startpoint.X && e.Y > startpoint.Y)
                {
                    Rectangulo.Width = e.X - startpoint.X;
                    Rectangulo.Height = e.Y - startpoint.Y;
                }
                else if (e.X < startpoint.X && e.Y > startpoint.Y)
                {
                    Rectangulo.Width = startpoint.X - e.X;
                    Rectangulo.Height = e.Y - startpoint.Y;
                    Rectangulo.X = e.X;
                    Rectangulo.Y = startpoint.Y;
                }
                else if (e.X > startpoint.X && e.Y < startpoint.Y)
                {
                    Rectangulo.Width = e.X - startpoint.X;
                    Rectangulo.Height = startpoint.Y - e.Y;
                    Rectangulo.X = startpoint.X;
                    Rectangulo.Y = e.Y;
                }
                else
                {
                    Rectangulo.Width = startpoint.X - e.X;
                    Rectangulo.Height = startpoint.Y - e.Y;
                    Rectangulo.X = e.X;
                    Rectangulo.Y = e.Y;
                }
                pictureLogo.Refresh();
            }
        }

        private void pictureLogo_MouseUp(object sender, MouseEventArgs e)
        {
            if (ImagenUsuario == null) return;

            if (endpoint.X != -1)
            {
                Point currentPoint = new Point(e.X, e.Y);
            }

            MouseClicked = false;

            endpoint.X = -1;
            endpoint.Y = -1;
            startpoint.X = -1;
            startpoint.Y = -1;

            //CAMBIAMOS EL CURSOR DE CRUZ A EL POR DEFECTO
            Cursor cursor = Cursors.Default;

            //SI LAS MEDIDAS SON CERO NO HABILITAMOS LOS BOTONES RESET Y RECORTAR
            if (Rectangulo.Width == 0 || Rectangulo.Height == 0)
                btnRecortar.Enabled = false;
            else
                btnRecortar.Enabled = true;                
        }

        private void pictureLogo_Paint(object sender, PaintEventArgs e)
        {
            Pen drawline = new Pen(Color.Black, 2);
            drawline.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            e.Graphics.DrawRectangle(drawline, Rectangulo);
        }

        private void btnRecortar_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            if (Rectangulo.Width == 0 || Rectangulo.Height == 0)
            { XtraMessageBox.Show("Por favor selecciona una area", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            //DEJAMOS VARIABLE RECORTAR EN TRUE
            recortar = true;

            Cursor = Cursors.Default;

            Imagen.CropImagen(pictureLogo, Rectangulo, PathLogo);
                        
            btnRecortar.Enabled = false;
            btnReset.Enabled = true;

            //LIMPIAMOS RECTANGULO
            Imagen.CleanDraw(Rectangulo);
        }

        private void pictureLogo_Click(object sender, EventArgs e)
        {
            pictureLogo.Refresh();
            Cursor = Cursors.Default;
        }

        private void txtCodEmpresa_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) || char.IsNumber(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            Empresa emp = new Empresa();

            if (emp.HayRegistros())
            {
                emp.SetInfo();                

                SaveFileDialog SaveFile = new SaveFileDialog();
                SaveFile.Filter = "txt files (*.txt)|*.txt";
                SaveFile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string ext = "";

                if (SaveFile.ShowDialog() == DialogResult.OK)
                {
                    ext = Path.GetExtension(SaveFile.FileName);
                    if (ext != ".txt")
                    { XtraMessageBox.Show("Extension de archivo no valida", "Extension", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                    //EXPORTAMOS ARCHIVO DE TEXTO
                    emp.ExportToTextFile(SaveFile.FileName);

                    if (File.Exists(SaveFile.FileName))
                    {
                        XtraMessageBox.Show($"Archivo creado en {SaveFile.FileName}", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
                        DialogResult ver = XtraMessageBox.Show($"¿Deseas ver el archivo {SaveFile.FileName}?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (ver == DialogResult.Yes)
                        {
                            //ABRIMOS ARCHIVO
                            System.Diagnostics.Process.Start(SaveFile.FileName);                            
                        }
                    }                        
                    else
                        XtraMessageBox.Show("No se pudo generar la información", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);


                }                
            }
        }

        private void txtcodPais_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtCodArea_KeyPress(object sender, KeyPressEventArgs e)
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
    }
}