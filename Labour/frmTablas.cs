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
using DevExpress.Skins;
using DevExpress.XtraEditors.Repository;
using System.Collections;
using System.Runtime.InteropServices;
using DevExpress.XtraTab;

namespace Labour
{
    public partial class frmTablas : DevExpress.XtraEditors.XtraForm
    {
        [DllImport("user32")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32")]
        static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        const int MF_BYCOMMAND = 0;
        const int MF_DISABLED = 2;
        const int SC_CLOSE = 0xF060;

        //ESTA VARIABLE REPRESENTA LA PESTAÑA QUE DESEA DEJAR SELECCIONADA CUANDO SE HABRA EL FORM USANDO
        //EL SEGUNDO CONSTRUCTOR
        public string opcion;

        //PARA COMUNICACION DESACOPLADA CON FORMULARIO EMPEADO
        public ITrabajadorCombo opener { get; set; }

        private string SqlGridSucursal = "SELECT codSucursal, DescSucursal, dato01, dato02 FROM SUCURSAL WHERE codSucursal <> 0";
        private string SqlCusalTermino = "SELECT codCausal, descCausal, justificacion, indAviso, indServicio FROM causaltermino WHERE codCausal <> 0";
        private string SqlArea = "SELECT id, nombre, dato01, dato02 FROM area WHERE id <> 0";
        private string SqlBanco = "SELECT id, nombre, dato01, dato02 FROM banco";
        private string SqlCargo = "SELECT id, nombre, dato01, dato02 FROM cargo WHERE id <> 0";
        private string SqlCentroCosto = "SELECT id, nombre, dato01, dato02 FROM ccosto WHERE id <> 0";
        private string SqlFormaPago = "SELECT id, nombre, dato01 as 'Tipo Pago', dato02 FROM formaPago";
        private string SqlNacion = "SELECT id, nombre, dato01, dato02 FROM nacion";
        private string SqlEscolaridad = "SELECT codesc, descesc FROM escolaridad WHERE codesc <> 0";
        private string SqlHorario = "SELECT id, deschor, detalle FROM horario where id <> 0";
        private string SqlTipoCuenta = "SELECT id, nombre, dato01, dato02 FROM tipocuenta ";
        private string SqlSindicato = "SELECT id, descSin FROM sindicato WHERE id <> 0 ORDER BY id";

        //BOTON NUEVO
        Operacion op;

        //PARA BOTON NUEVO 

        bool updateSucursal = true;
        bool ModificaCaja = false;
        bool UpdateCausal = false;
        public bool UpdateEscolaridad = false;
        bool UpdateHorario = false;
        bool UpdateTipoCuenta = false;
        bool UpdateSindicato = false;

        public frmTablas()
        {
            InitializeComponent();
        }

        //CONSTRUCTOR PARAMETRIZADO SOLO USADO CUANDO SE INVOCA EL FORM DESDE EL FORM EMPLEADO (DOUBLE CLICK COMBOBOX)
        public frmTablas(string opcion)
        {
            InitializeComponent();
            this.opcion = opcion;
        }

        private void xtraTabControl1_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            //posicion pagina actual            
            //SABER CUAL ES LA PESTAÑA SELECCIONADA

            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            if (tabMain.SelectedTabPage.Equals(tabArea))
            {
                //CODE
                fnPropiedades(txtidArea, txtNombreArea, txtdato1Area, txtdato2Area, btnEliminarArea, btnNuevoArea,
                    btnGuardarArea, separador1Area, separador2Area, gridArea);
                fnSistema.spllenaGridView(gridArea, SqlArea);
                fnSistema.spOpcionesGrilla(viewArea);
                fnOpcionesGrilla(viewArea);
                fnCargarCampos(txtidArea, txtNombreArea, txtdato1Area, txtdato2Area, btnEliminarArea, viewArea, 0);
                op.Cancela = false;
                op.SetButtonProperties(btnNuevoArea, 1);

                this.Text = "Area";
            }
            else if (tabMain.SelectedTabPage.Equals(tabBanco))
            {

                //CODE
                fnPropiedades(txtIdBanco, txtNombreBanco, txtDato1Banco, txtDato2Banco, btnEliminarBanco, btnNuevoBanco,
                    btnguardarBanco, separador1Banco, separador2Banco, gridBanco);
                fnSistema.spllenaGridView(gridBanco, SqlBanco);
                fnSistema.spOpcionesGrilla(viewBanco);
                fnOpcionesGrilla(viewBanco);
                fnCargarCampos(txtIdBanco, txtNombreBanco, txtDato1Banco, txtDato2Banco, btnEliminarBanco, viewBanco, 0);
                op.Cancela = false;
                op.SetButtonProperties(btnNuevoBanco, 1);
                this.Text = "Banco";

            }
            else if (tabMain.SelectedTabPage.Equals(tabCargo))
            {
                //CODE
                fnPropiedades(txtidCargo, txtnombreCargo, txtdato1Cargo, txtdato2Cargo, btnEliminarCargo, btnNuevoCargo,
                    btnGuardarCargo, separador1Cargo, separador2Cargo, gridCargo);
                fnSistema.spllenaGridView(gridCargo, SqlCargo);
                fnSistema.spOpcionesGrilla(viewCargo);
                //PARA LAS COLUMNAS
                fnOpcionesGrilla(viewCargo);
                fnCargarCampos(txtidCargo, txtnombreCargo, txtdato1Cargo, txtdato2Cargo, btnEliminarCargo, viewCargo, 0);
                op.Cancela = false;
                op.SetButtonProperties(btnNuevoCargo, 1);

                this.Text = "Cargo";

            }
            else if (tabMain.SelectedTabPage.Equals(tabCosto))
            {
                //CODE
                fnPropiedades(txtidCosto, txtnombreCosto, txtdato1Costo, txtdato2Costo, btnEliminarCosto, btnNuevoCosto,
                    btnGuardarCosto, separador1Costo, separador2Costo, gridCosto);
                fnSistema.spllenaGridView(gridCosto, SqlCentroCosto);
                fnSistema.spOpcionesGrilla(viewCosto);
                //PARA LAS COLUMNAS
                fnOpcionesGrilla(viewCosto);
                fnCargarCampos(txtidCosto, txtnombreCosto, txtdato1Costo, txtdato2Costo, btnEliminarCosto, viewCosto, 0);
                op.Cancela = false;
                op.SetButtonProperties(btnNuevoCosto, 1);

                this.Text = "Centro de Costo";


            }
            else if (tabMain.SelectedTabPage.Equals(tabFormaPago))
            {
                //CODE
                fnPropiedadesFormaPago(txtidPago, txtnombrePago, cbxTipoFormaPago, txtdato2Pago, btnEliminarPago, btnNuevoPago,
                    btnGuardarPago, separador1Pago, separador2Pago, gridPago);
                fnSistema.spllenaGridView(gridPago, SqlFormaPago);
                fnSistema.spOpcionesGrilla(viewPago);
                //para las columnas
                fnOpcionesGrillaFormaPago(viewPago);
                fnCargarCamposFormaPago(txtidPago, txtnombrePago, cbxTipoFormaPago, txtdato2Pago, btnEliminarPago, viewPago, 0);
                op.Cancela = false;
                op.SetButtonProperties(btnNuevoPago, 1);
                this.Text = "Forma de Pago";
                //SE AGREGA COMBOBOX CON DATOS DE TIPO DE FORMA DE PAGO
                fnTipoFormaPago(cbxTipoFormaPago);

            }
            else if (tabMain.SelectedTabPage.Equals(tabNacion))
            {
                //code
                fnPropiedades(txtidNacion, txtnombreNacion, txtdato1Nacion, txtdato2Nacion, btnNuevoNacion, btnEliminarNacion,
                    btnGuardarNacion, separador1Nacion, separador2Nacion, gridNacion);
                fnSistema.spllenaGridView(gridNacion, SqlNacion);
                fnSistema.spOpcionesGrilla(viewNacion);
                //PARA LAS COLUMNAS
                fnOpcionesGrilla(viewNacion);
                fnCargarCampos(txtidNacion, txtnombreNacion, txtdato1Nacion, txtdato2Nacion, btnEliminarNacion, viewNacion, 0);
                op.Cancela = false;
                op.SetButtonProperties(btnNuevoNacion, 1);

                this.Text = "Nacionalidad";

            }
            else if (tabMain.SelectedTabPage.Equals(tabCajacompensacion))
            {
                this.Text = "CAJA COMPENSACION";
                CargarGrillaCaja();
                CargarDataFromGrid(0);

                op.Cancela = false;
                op.SetButtonProperties(btnNuevoCaja, 1);
            }
            else if (tabMain.SelectedTabPage.Equals(tabSucursal))
            {
                this.Text = "SUCURSAL";

                op.Cancela = false;
                op.SetButtonProperties(btnNuevaSucursal, 1);

                fnSistema.spllenaGridView(gridSucursal, SqlGridSucursal);
                if (viewSucursal.RowCount > 0)
                {
                    fnSistema.spOpcionesGrilla(viewSucursal);
                    ColumnasGirdSucursal();
                    CargarSucursal(0);
                }
            }
            else if (tabMain.SelectedTabPage.Equals(tabCausal))
            {
                this.Text = "CAUSAL";

                op.Cancela = false;
                op.SetButtonProperties(btnNuevoCausal, 1);

                fnSistema.spllenaGridView(gridCausal, SqlCusalTermino);
                if (viewCausal.RowCount > 0)
                {
                    fnSistema.spOpcionesGrilla(viewCausal);
                    fnColumnasCausal();
                    fnCargarCamposCausal(0);
                }
            }
            else if (tabMain.SelectedTabPage.Equals(tabEscolaridad))
            {
                this.Text = "Escolaridad";

                op.Cancela = false;
                op.SetButtonProperties(btnNuevoEsco, 1);

                fnSistema.spllenaGridView(gridEsco, SqlEscolaridad);
                if (viewEsco.RowCount > 0)
                {
                    Escolaridad escolar = new Escolaridad();

                    fnSistema.spOpcionesGrilla(viewEsco);
                    escolar.OpcionesGrilla(viewEsco);

                    CargarInfoEsco(0);
                }
            }
            else if (tabMain.SelectedTabPage.Equals(tabHorario))
            {
                this.Text = "Horario";

                op.Cancela = false;
                op.SetButtonProperties(btnNuevoHorario, 1);

                fnSistema.spllenaGridView(gridHorario, SqlHorario);
                if (viewHorario.RowCount > 0)
                {
                    fnSistema.spOpcionesGrilla(viewHorario);
                    ColumnasGridhorario();

                    CargaCamposHorario(0);
                }
            }
            else if (tabMain.SelectedTabPage.Equals(tabTipoCuenta))
            {
                this.Text = "Tipo Cuenta";

                fnSistema.spllenaGridView(gridTipoCuenta, SqlTipoCuenta);
                if (viewTipoCuenta.RowCount > 0)
                {
                    fnSistema.spOpcionesGrilla(viewTipoCuenta);
                    ColumnasCuenta();
                    CargarTipoCuenta(0);

                }
            }
            else if (tabMain.SelectedTabPage.Equals(tabSindicato))
            {
                this.Text = "Sindicato";

                fnSistema.spllenaGridView(gridSindicato, SqlSindicato);
                if (viewSindicato.RowCount > 0)
                {
                    fnSistema.spOpcionesGrilla(viewSindicato);
                    ColumnasSindicato();
                    CargaSind();
                }
            }     
        }

        private void frmTablas_Load(object sender, EventArgs e)
        {
            //OCULTAR TAB SI NO SE TIENE ACCESO
            OcultaTab();

            var sm = GetSystemMenu(Handle, false);
            EnableMenuItem(sm, SC_CLOSE, MF_BYCOMMAND | MF_DISABLED);

            op = new Operacion();

            //PARA CUANDO SE INVOCA EL FORMULARIO DESDE EL FORMULARIO TRABAJADOR
            if (opcion != "")
            {
                fnOpciones(opcion);
            }          
           
            SkinManager.EnableFormSkins();
            SkinManager.EnableMdiFormSkins();

            //SELECCIONAR POR DEFECTO LA PRIMERA PESTAÑA AL CARGAR EL FORMULARIO
            // tabMain.TabStop = false;
            tabArea.TabControl.TabStop = false;
            tabMain.SelectedTabPageIndex = 0;

            fnPropiedades(txtidArea, txtNombreArea, txtdato1Area, txtdato2Area, btnEliminarArea, btnNuevoArea,
                      btnGuardarArea, separador1Area, separador2Area, gridArea);
            fnSistema.spllenaGridView(gridArea, SqlArea);
            if (viewArea.RowCount > 0)
            {
                fnSistema.spOpcionesGrilla(viewArea);
                fnOpcionesGrilla(viewArea);
                fnCargarCampos(txtidArea, txtNombreArea, txtdato1Area, txtdato2Area, btnEliminarArea, viewArea, 0);
            }
            
        }

        //FUNCION PARA MOSTRAR DISTINTAS PESTAÑAS DE ACUERDO A OPCION ENVIADA DESDE FORMULARIO EMPLEADO
        private void fnOpciones(string opcion)
        {
            switch (opcion)
            {
                case "nacionalidad":
                    tabBanco.PageVisible = false;
                    tabCargo.PageVisible = false;
                    tabArea.PageVisible = false;
                    tabCosto.PageVisible = false;
                    tabFormaPago.PageVisible = false;
                    tabCajacompensacion.PageVisible = false;
                    tabSucursal.PageVisible = false;
                    tabCausal.PageVisible = false;
                    tabEscolaridad.PageVisible = false;
                    tabHorario.PageVisible = false;
                    tabTipoCuenta.PageVisible = false;
                    tabSindicato.PageVisible = false;
                    break;
                case "area":
                    tabNacion.PageVisible = false;
                    tabBanco.PageVisible = false;
                    tabCargo.PageVisible = false;
                    tabCosto.PageVisible = false;
                    tabFormaPago.PageVisible = false;
                    tabCajacompensacion.PageVisible = false;
                    tabSucursal.PageVisible = false;
                    tabCausal.PageVisible = false;
                    tabEscolaridad.PageVisible = false;
                    tabHorario.PageVisible = false;
                    tabTipoCuenta.PageVisible = false;
                    tabSindicato.PageVisible = false;
                    break;
                case "cargo":
                    tabNacion.PageVisible = false;
                    tabBanco.PageVisible = false;                    
                    tabCosto.PageVisible = false;
                    tabFormaPago.PageVisible = false;
                    tabArea.PageVisible = false;
                    tabCajacompensacion.PageVisible = false;
                    tabSucursal.PageVisible = false;
                    tabCausal.PageVisible = false;
                    tabEscolaridad.PageVisible = false;
                    tabHorario.PageVisible = false;
                    tabTipoCuenta.PageVisible = false;
                    tabSindicato.PageVisible = false;
                    break;
                case "centrocosto":
                    tabNacion.PageVisible = false;
                    tabBanco.PageVisible = false;                    
                    tabFormaPago.PageVisible = false;
                    tabArea.PageVisible = false;
                    tabCargo.PageVisible = false;
                    tabCajacompensacion.PageVisible = false;
                    tabSucursal.PageVisible = false;
                    tabCausal.PageVisible = false;
                    tabEscolaridad.PageVisible = false;
                    tabHorario.PageVisible = false;
                    tabTipoCuenta.PageVisible = false;
                    tabSindicato.PageVisible = false;
                    break;
                case "formapago":
                    //CODE
                    fnPropiedadesFormaPago(txtidPago, txtnombrePago, cbxTipoFormaPago, txtdato2Pago, btnEliminarPago, btnNuevoPago,
                        btnGuardarPago, separador1Pago, separador2Pago, gridPago);
                    fnSistema.spllenaGridView(gridPago, "SELECT id, nombre, dato01, dato02 FROM formaPago");
                    fnSistema.spOpcionesGrilla(viewPago);
                    //para las columnas
                    fnOpcionesGrilla(viewPago);
                    fnCargarCamposFormaPago(txtidPago, txtnombrePago, cbxTipoFormaPago, txtdato2Pago, btnEliminarPago, viewPago, 0);

                    tabNacion.PageVisible = false;
                    tabBanco.PageVisible = false;                    
                    tabArea.PageVisible = false;
                    tabCargo.PageVisible = false;
                    tabCosto.PageVisible = false;
                    tabCajacompensacion.PageVisible = false;
                    tabSucursal.PageVisible = false;
                    tabCausal.PageVisible = false;
                    tabEscolaridad.PageVisible = false;
                    tabHorario.PageVisible = false;
                    tabTipoCuenta.PageVisible = false;
                    tabSindicato.PageVisible = false;
                    break;
                case "banco":                    
                    tabNacion.PageVisible = false;                   
                    tabArea.PageVisible = false;
                    tabCargo.PageVisible = false;
                    tabCosto.PageVisible = false;
                    tabFormaPago.PageVisible = false;
                    tabCajacompensacion.PageVisible = false;
                    tabSucursal.PageVisible = false;
                    tabCausal.PageVisible = false;
                    tabEscolaridad.PageVisible = false;
                    tabHorario.PageVisible = false;
                    tabTipoCuenta.PageVisible = false;
                    tabSindicato.PageVisible = false;
                    break;
                case "sucursal":
                    tabBanco.PageVisible = false;
                    tabNacion.PageVisible = false;
                    tabArea.PageVisible = false;
                    tabCargo.PageVisible = false;
                    tabCosto.PageVisible = false;
                    tabFormaPago.PageVisible = false;
                    tabCajacompensacion.PageVisible = false;
                    tabCausal.PageVisible = false;
                    tabEscolaridad.PageVisible = false;
                    tabHorario.PageVisible = false;
                    tabTipoCuenta.PageVisible = false;
                    tabSindicato.PageVisible = false;
                    break;
                case "causal":
                    tabBanco.PageVisible = false;
                    tabNacion.PageVisible = false;
                    tabArea.PageVisible = false;
                    tabCargo.PageVisible = false;
                    tabCosto.PageVisible = false;
                    tabFormaPago.PageVisible = false;
                    tabCajacompensacion.PageVisible = false;
                    tabSucursal.PageVisible = false;
                    tabEscolaridad.PageVisible = false;
                    tabHorario.PageVisible = false;
                    tabTipoCuenta.PageVisible = false;
                    tabSindicato.PageVisible = false;
                    break;
                case "escolaridad":
                    tabBanco.PageVisible = false;
                    tabNacion.PageVisible = false;
                    tabArea.PageVisible = false;
                    tabCargo.PageVisible = false;
                    tabCosto.PageVisible = false;
                    tabFormaPago.PageVisible = false;
                    tabCajacompensacion.PageVisible = false;
                    tabSucursal.PageVisible = false;
                    tabHorario.PageVisible = false;
                    tabTipoCuenta.PageVisible = false;
                    tabSindicato.PageVisible = false;
                    break;

                case "horario":
                    tabBanco.PageVisible = false;
                    tabNacion.PageVisible = false;
                    tabArea.PageVisible = false;
                    tabCargo.PageVisible = false;
                    tabCosto.PageVisible = false;
                    tabFormaPago.PageVisible = false;
                    tabCajacompensacion.PageVisible = false;
                    tabSucursal.PageVisible = false;
                    tabEscolaridad.PageVisible = false;
                    tabTipoCuenta.PageVisible = false;
                    tabSindicato.PageVisible = false;
                    break;

                case "tipocuenta":
                    tabBanco.PageVisible = false;
                    tabNacion.PageVisible = false;
                    tabArea.PageVisible = false;
                    tabCargo.PageVisible = false;
                    tabCosto.PageVisible = false;
                    tabFormaPago.PageVisible = false;
                    tabCajacompensacion.PageVisible = false;
                    tabSucursal.PageVisible = false;
                    tabEscolaridad.PageVisible = false;
                    tabHorario.PageVisible = false;
                    tabCausal.PageVisible = false;
                    tabSindicato.PageVisible = false;
                    break;

                case "sindicato":
                    tabBanco.PageVisible = false;
                    tabNacion.PageVisible = false;
                    tabArea.PageVisible = false;
                    tabCargo.PageVisible = false;
                    tabCosto.PageVisible = false;
                    tabFormaPago.PageVisible = false;
                    tabCajacompensacion.PageVisible = false;
                    tabSucursal.PageVisible = false;
                    tabEscolaridad.PageVisible = false;
                    tabHorario.PageVisible = false;
                    tabCausal.PageVisible = false;
                    tabTipoCuenta.PageVisible = false;
                    break;
            }
        }        

        #region "AREA-CARGO-CENTRO COSTO - FORMA PAGO - NACION-BANCO"
        //METODO PARA NUEVO INGRESO
        //DATOS DE ENTRADA: ID, NOMBRE, DATO1, DATO2, SQL QUERY INSERT
        //RETORNA TRUE EN CASO DE INSERT CORRECTO
        private bool fnNuevoRegistro(TextEdit pId, TextEdit pNombre, TextEdit pDato1, TextEdit pDato2, string pSql)
        {
            //SQL
           // string sql = "INSERT INTO area(id, nombre, dato01, dato02) VALUES(@pId, @pNombre, @pDato1, @pDato2)";
            SqlCommand cmd;
            int res = 0;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(pSql, fnSistema.sqlConn))
                    {
                        //parametros
                        cmd.Parameters.Add(new SqlParameter("@pId", pId.Text));
                        cmd.Parameters.Add(new SqlParameter("@pNombre", pNombre.Text));
                        cmd.Parameters.Add(new SqlParameter("@pDato1", pDato1.Text));
                        cmd.Parameters.Add(new SqlParameter("@pDato2", pDato2.Text));
                        //EJECUTAMOS LA CONSULTA
                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            XtraMessageBox.Show("Registro Guardado", "Guardar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return true;
                            //VOLVER A CARGAR LA GRILLA 
                            //fnSistema.spllenaGridView(gridArea, "SELECT id, nombre, dato01, dato02 FROM area");
                            //CARGAR PRIMERA FILA
                            //fnCargarCampos(viewArea, 0);
                        }
                        else
                        {
                            XtraMessageBox.Show("Error al intentar guardar", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        }
                        //LIBERAR RECURSOS
                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                    }
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

        //METODO PARA NUEVO INGRESO
        //DATOS DE ENTRADA: ID, NOMBRE, DATO1, DATO2, SQL QUERY INSERT
        //RETORNA TRUE EN CASO DE INSERT CORRECTO
        private bool fnNuevoRegistroFormaPago(TextEdit pId, TextEdit pNombre, LookUpEdit pTipoFormapago, TextEdit pDato2, string pSql)
        {
            //SQL
            // string sql = "INSERT INTO area(id, nombre, dato01, dato02) VALUES(@pId, @pNombre, @pDato1, @pDato2)";
            SqlCommand cmd;
            int res = 0;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(pSql, fnSistema.sqlConn))
                    {
                        //parametros
                        cmd.Parameters.Add(new SqlParameter("@pId", pId.Text));
                        cmd.Parameters.Add(new SqlParameter("@pNombre", pNombre.Text));
                        cmd.Parameters.Add(new SqlParameter("@pDato1", pTipoFormapago.EditValue));
                        cmd.Parameters.Add(new SqlParameter("@pDato2", pDato2.Text));
                        //EJECUTAMOS LA CONSULTA
                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            XtraMessageBox.Show("Registro Guardado", "Guardar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return true;
                            //VOLVER A CARGAR LA GRILLA 
                            //fnSistema.spllenaGridView(gridArea, "SELECT id, nombre, dato01, dato02 FROM area");
                            //CARGAR PRIMERA FILA
                            //fnCargarCampos(viewArea, 0);
                        }
                        else
                        {
                            XtraMessageBox.Show("Error al intentar guardar", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        }
                        //LIBERAR RECURSOS
                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                    }
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

        //METODO PARA MODIFICAR UN REGISTRO
        //DATOS DE ENTRADA: ID, NOMBRE, DATO1, DATO2, SQL QUERY UPDATE
        //RETORNA TRUE EN CASO UPDATE CORRECTO
        private bool fnModificarRegistro(int pId, TextEdit pNombre, TextEdit pDato1, TextEdit pDato2, string pSql, string pNombreTabla)
        {
            //SQL
            string sql = "UPDATE area SET nombre=@pNombre, dato01=@pDato1, dato02=@pDato2 WHERE id=@pId";
            SqlCommand cmd;
            int res = 0;

            //TABLA HASH PARA LOG
            Hashtable tabla = new Hashtable();
            tabla = PrecargaDatos(pId, pNombreTabla);

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(pSql, fnSistema.sqlConn))
                    {
                        //parametros
                        cmd.Parameters.Add(new SqlParameter("@pNombre", pNombre.Text));
                        cmd.Parameters.Add(new SqlParameter("@pDato1", pDato1.Text));
                        cmd.Parameters.Add(new SqlParameter("@pDato2", pDato2.Text));
                        cmd.Parameters.Add(new SqlParameter("@pId", pId));

                        //EJECUTAMOS LA CONSULTA
                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            XtraMessageBox.Show("Registro Actualizado", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            //VERIFICAMOS SI HAY CAMBIOS PARA GUARDAR EN LOG
                            CompararDatos(pNombre.Text, pDato1.Text, pDato2.Text, pNombreTabla, tabla);

                            return true;
                            //volver a cargar la GRILLA
                            //fnSistema.spllenaGridView(gridArea, "SELECT id, nombre, dato01, dato02 FROM area");
                            //CARGAR FILA 0
                            //fnCargarCampos(viewArea, 0);
                        }
                        else
                        {
                            XtraMessageBox.Show("Error al intentar Actualizar", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    //LIBERAR RECURSOS
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                }
                else
                {
                    XtraMessageBox.Show("No hay conexion con Base de datos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
            return false;
        }

        //METODO PARA MODIFICAR UN REGISTRO EN FORMA PAGO
        //DATOS DE ENTRADA: ID, NOMBRE, DATO1, DATO2, SQL QUERY UPDATE
        //RETORNA TRUE EN CASO UPDATE CORRECTO
        private bool fnModificarRegistroFormaPago(int pId, TextEdit pNombre, LookUpEdit pTipoFormaPago, TextEdit pDato2, string pSql, string pNombreTabla)
        {
            //SQL
            string sql = "UPDATE area SET nombre=@pNombre, dato01=@pDato1, dato02=@pDato2 WHERE id=@pId";
            SqlCommand cmd;
            int res = 0;

            //TABLA HASH PARA LOG
            Hashtable tabla = new Hashtable();
            tabla = PrecargaDatos(pId, pNombreTabla);

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(pSql, fnSistema.sqlConn))
                    {
                        //parametros
                        cmd.Parameters.Add(new SqlParameter("@pNombre", pNombre.Text));
                        cmd.Parameters.Add(new SqlParameter("@pDato1", pTipoFormaPago.EditValue.ToString()));
                        cmd.Parameters.Add(new SqlParameter("@pDato2", pDato2.Text));
                        cmd.Parameters.Add(new SqlParameter("@pId", pId));

                        //EJECUTAMOS LA CONSULTA
                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            XtraMessageBox.Show("Registro Actualizado", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            //VERIFICAMOS SI HAY CAMBIOS PARA GUARDAR EN LOG
                            CompararDatos(pNombre.Text, pTipoFormaPago.EditValue.ToString(), pDato2.Text, pNombreTabla, tabla);

                            return true;
                            //volver a cargar la GRILLA
                            //fnSistema.spllenaGridView(gridArea, "SELECT id, nombre, dato01, dato02 FROM area");
                            //CARGAR FILA 0
                            //fnCargarCampos(viewArea, 0);
                        }
                        else
                        {
                            XtraMessageBox.Show("Error al intentar Actualizar", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    //LIBERAR RECURSOS
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                }
                else
                {
                    XtraMessageBox.Show("No hay conexion con Base de datos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
            return false;
        }


        //METODO PARA ELIMINAR UN REGISTRO
        //DATOS DE ENTRADA: ID, SQL QUERTY DELETE
        //RETORNA TRUE EN CASO DE ELIMINACION CORRECTA
        private bool fnEliminarRegistro(int pId, string pSql)
        {
            //SQL
            string sql = "DELETE FROM area WHERE id=@pId";
            SqlCommand cmd;
            int res = 0;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(pSql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pId", pId));
                        res = cmd.ExecuteNonQuery();
                        //ejecutamos la consulta
                        if (res > 0)
                        {
                            XtraMessageBox.Show("Registro Eliminado", "Eliminar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return true;
                            //VOLVER A CARGAR GRILLA
                            //fnSistema.spllenaGridView(gridArea, "SELECT id, nombre, dato01, dato02 FROM area");
                            //CARGAR FILA 0
                            //fnCargarCampos(viewArea, 0);
                        }
                        else
                        {
                            XtraMessageBox.Show("Error al intentar Eliminar", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    //LIBERAMOS RECURSOS
                    cmd.Dispose();
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

        //LIMPIAR CAMPOS
        private void fnLimpiar(TextEdit pId, TextEdit pNombre, TextEdit pDato1, TextEdit pDato2, SimpleButton pButton, string pTableName)
        {
            //pId.Text = "";
            pId.ReadOnly = false;
            pNombre.Text = "";
            pNombre.Focus();
            //pId.Focus();
            pDato1.Text = "";
            pDato2.Text = "";
            //DESHABILITAR BOTON ELIMINAR
            pButton.Enabled = false;

            //OBTENER ID DISPONIBLE
            int num = fnItemDisponibles(pTableName);
            pId.Text = num.ToString();
            lblMessage.Visible = false;
            lblBanco.Visible = false;
            lblCargo.Visible = false;
            lblCosto.Visible = false;
            lblMessage.Visible = false;
            lblNacion.Visible = false;
            lblPago.Visible = false;
        }

        //LIMPIAR CAMPOS
        private void fnLimpiarFormaPago(TextEdit pId, TextEdit pNombre, LookUpEdit pTipoFormaPago, TextEdit pDato2, SimpleButton pButton, string pTableName)
        {
            //pId.Text = "";
            pId.ReadOnly = false;
            pNombre.Text = "";
            pNombre.Focus();
            //pId.Focus();
            pTipoFormaPago.ItemIndex = 0;
            pDato2.Text = "";
            //DESHABILITAR BOTON ELIMINAR
            pButton.Enabled = false;

            //OBTENER ID DISPONIBLE
            int num = fnItemDisponibles(pTableName);
            pId.Text = num.ToString();
            lblMessage.Visible = false;
            lblBanco.Visible = false;
            lblCargo.Visible = false;
            lblCosto.Visible = false;
            lblMessage.Visible = false;
            lblNacion.Visible = false;
            lblPago.Visible = false;
        }

        //COLUMNAS PARA GRILLA
        private void fnOpcionesGrilla(DevExpress.XtraGrid.Views.Grid.GridView pGrid)
        {
            if (pGrid.RowCount > 0)
            {
                pGrid.Columns[0].Visible = true;
                pGrid.Columns[0].Width = 50;
                pGrid.Columns[0].Caption = "N°";
                pGrid.Columns[0].AppearanceCell.FontStyleDelta = FontStyle.Bold;

                pGrid.Columns[1].Caption = "Nombre";
                pGrid.Columns[1].Width = 300;               

                pGrid.Columns[2].Caption = "Dato N°1";
                pGrid.Columns[2].Width = 50;
                pGrid.Columns[3].Caption = "Dato N°2";
                pGrid.Columns[3].Width = 50;
            }          
        }

        //COLUMNAS PARA GRILLA FORMA PAGO
        private void fnOpcionesGrillaFormaPago(DevExpress.XtraGrid.Views.Grid.GridView pGrid)
        {
            if (pGrid.RowCount > 0)
            {
                pGrid.Columns[0].Visible = true;
                pGrid.Columns[0].Width = 50;
                pGrid.Columns[0].Caption = "N°";
                pGrid.Columns[0].AppearanceCell.FontStyleDelta = FontStyle.Bold;

                pGrid.Columns[1].Caption = "Nombre";
                pGrid.Columns[1].Width = 300;

                pGrid.Columns[2].Caption = "Tipo Pago";
                pGrid.Columns[2].Width = 50;
                pGrid.Columns[3].Caption = "Dato N°2";
                pGrid.Columns[3].Width = 50;
            }
        }

        //cargar cmapos con registros de grilla
        private void fnCargarCampos(TextEdit pId, TextEdit pNombre,
            TextEdit pDato1, TextEdit pDato2,SimpleButton pButton, DevExpress.XtraGrid.Views.Grid.GridView pGrid, int? pos = -1)
        {            

            if (pGrid.RowCount > 0)
            {
                if (pos == 0) pGrid.FocusedRowHandle = 0;
                //HABILITAR BOTON ELIMINAR
                pButton.Enabled = true;
                //dejar campo id como solo lectura
                pId.ReadOnly = true;

                lblMessage.Visible = false;
                lblBanco.Visible = false;
                lblCargo.Visible = false;
                lblCosto.Visible = false;
                lblMessage.Visible = false;
                lblNacion.Visible = false;
                lblPago.Visible = false;                        

                pId.Text = pGrid.GetFocusedDataRow()["id"].ToString();
                pNombre.Text = pGrid.GetFocusedDataRow()["nombre"].ToString();
                pDato1.Text = pGrid.GetFocusedDataRow()["dato01"].ToString();
                pDato2.Text = pGrid.GetFocusedDataRow()["dato02"].ToString();              

               // pNombre.Properties.Appearance.BackColor = Color.White;
            }
        }

        //cargar cmapos con registros de grilla
        private void fnCargarCamposFormaPago(TextEdit pId, TextEdit pNombre,
            LookUpEdit pTipoFormaPago, TextEdit pDato2, SimpleButton pButton, DevExpress.XtraGrid.Views.Grid.GridView pGrid, int? pos = -1)
        {

            if (pGrid.RowCount > 0)
            {
                if (pos == 0) pGrid.FocusedRowHandle = 0;
                //HABILITAR BOTON ELIMINAR
                pButton.Enabled = true;
                //dejar campo id como solo lectura
                pId.ReadOnly = true;

                lblMessage.Visible = false;
                lblBanco.Visible = false;
                lblCargo.Visible = false;
                lblCosto.Visible = false;
                lblMessage.Visible = false;
                lblNacion.Visible = false;
                lblPago.Visible = false;

                pId.Text = pGrid.GetFocusedDataRow()["id"].ToString();
                pNombre.Text = pGrid.GetFocusedDataRow()["nombre"].ToString();
                pTipoFormaPago.EditValue = pGrid.GetFocusedDataRow()["Tipo Pago"].ToString();
                pDato2.Text = pGrid.GetFocusedDataRow()["dato02"].ToString();

                // pNombre.Properties.Appearance.BackColor = Color.White;
            }
        }
        //SABER SI ID INGRESADO YA EXISTE EN BD
        //PARAMETROS DE ENTRADA: ID, SQL QUERY BUSQUEDA
        private bool fnVerId(int pId, string pSql)
        {
            //SQL
            string sql = "SELECT * FROM area WHERE id=@pId";
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(pSql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pId", pId));
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            return true;
                        }
                    }
                    //LIBERAMOS RECURSOS
                    cmd.Dispose();
                    rd.Close();
                    fnSistema.sqlConn.Close();
                }
                else
                {
                    XtraMessageBox.Show("No hay conexion con bade de datos", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
            return false;
        }

        //PROPIEDADES POR DEFECTO
        private void fnPropiedades(TextEdit pId, TextEdit pNombre, TextEdit pDato1, TextEdit pDato2, 
            SimpleButton pBtn1, SimpleButton pBtn2, SimpleButton pBtn3, SeparatorControl pSp1, SeparatorControl pSp2,
            DevExpress.XtraGrid.GridControl pGrid)
        {
            pBtn1.AllowFocus = false;
            pBtn2.AllowFocus = false;
            pBtn3.AllowFocus = false;
            pGrid.TabStop = false;
            pSp1.TabStop = false;
            pSp2.TabStop = false;

            pNombre.Properties.MaxLength = 50;
            pNombre.Properties.Appearance.FontStyleDelta = FontStyle.Bold;
            pDato1.Properties.MaxLength = 10;
            pDato2.Properties.MaxLength = 10;
            pId.Properties.MaxLength = 4;
            pId.Focus();
        }

        //PROPIEDADES POR DEFECTO
        private void fnPropiedadesFormaPago(TextEdit pId, TextEdit pNombre, LookUpEdit pTipoFormaPago, TextEdit pDato2,
            SimpleButton pBtn1, SimpleButton pBtn2, SimpleButton pBtn3, SeparatorControl pSp1, SeparatorControl pSp2,
            DevExpress.XtraGrid.GridControl pGrid)
        {
            pBtn1.AllowFocus = false;
            pBtn2.AllowFocus = false;
            pBtn3.AllowFocus = false;
            pGrid.TabStop = false;
            pSp1.TabStop = false;
            pSp2.TabStop = false;

            pNombre.Properties.MaxLength = 50;
            pNombre.Properties.Appearance.FontStyleDelta = FontStyle.Bold;
            //pDato1.Properties.MaxLength = 10;
            pDato2.Properties.MaxLength = 10;
            pId.Properties.MaxLength = 4;
            pId.Focus();
        }

        //VERIFICAR SI SE HIZO ALGUN CAMBIO SIN GUARDAR EN LOS COMBOBOX
        //RETORNA TRUE en caso de que se encuentren diferencias entre caja y lo que esta en bd
        private bool fnCambios(TextEdit pId, TextEdit pNombre, TextEdit pDato1, TextEdit pDato2, string pSql)
        {
            //EJ SQL --> SELECT id, nombre, dato01, dato02 FROM TABLA WHERE ID=@Pid
            //CONSULTA POR DATOS EN BD DE ACUERDO A EL ID
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(pSql, fnSistema.sqlConn))
                    {
                        //parametro
                        cmd.Parameters.Add(new SqlParameter("@pId", pId.Text));
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //COMPARAMOS CON LOS VALORES EN CAJA
                            while (rd.Read())
                            {
                                if (pNombre.Text.ToLower() != (rd["nombre"]).ToString().ToLower()) { return true; }
                                if (pDato1.Text.ToLower() != rd["dato01"].ToString().ToLower()) { return true; }
                                if (pDato2.Text.ToLower() != rd["dato02"].ToString().ToLower()) { return true; }
                            }
                        }
                    }
                    //LIBERAMOS RECURSOS
                    cmd.Dispose();
                    rd.Close();
                    fnSistema.sqlConn.Close();
                }
                else
                {
                    XtraMessageBox.Show("No hay conexion a base de datos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
            return false;
        }

        //VERIFICAR SI SE HIZO ALGUN CAMBIO SIN GUARDAR EN LOS COMBOBOX
        //RETORNA TRUE en caso de que se encuentren diferencias entre caja y lo que esta en bd
        private bool fnCambiosFormaPago(TextEdit pId, TextEdit pNombre, LookUpEdit pTipoFormaPago, TextEdit pDato2, string pSql)
        {
            //EJ SQL --> SELECT id, nombre, dato01, dato02 FROM TABLA WHERE ID=@Pid
            //CONSULTA POR DATOS EN BD DE ACUERDO A EL ID
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(pSql, fnSistema.sqlConn))
                    {
                        //parametro
                        cmd.Parameters.Add(new SqlParameter("@pId", pId.Text));
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //COMPARAMOS CON LOS VALORES EN CAJA
                            while (rd.Read())
                            {
                                if (pNombre.Text.ToLower() != (rd["nombre"]).ToString().ToLower()) { return true; }
                                if (pTipoFormaPago.EditValue.ToString() != rd["dato01"].ToString().ToLower()) { return true; }
                                if (pDato2.Text.ToLower() != rd["dato02"].ToString().ToLower()) { return true; }
                            }
                        }
                    }
                    //LIBERAMOS RECURSOS
                    cmd.Dispose();
                    rd.Close();
                    fnSistema.sqlConn.Close();
                }
                else
                {
                    XtraMessageBox.Show("No hay conexion a base de datos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
            return false;
        }

        //VERIFICAR SI EL REGISTRO ESTA SIENDO UTILIZADO POR UN EMPLEADO
        //EN ESE CASO NO SE DEBE ELIMINAR
        //RETORNA TRUE SI EL REGISTRO ESTA EN USO
        private bool fnRegistroEnUso(string pId, string pSql)
        {
            SqlCommand cmd;
            SqlDataReader rd;

            bool utilizado = false;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(pSql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pId", pId));

                        //EJECUTAR CONSULTA
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //SI ENCUENTRA REGISTROS ES PORQUE ESTA EN USO
                            utilizado = true;
                        }
                        else
                        {
                            utilizado = false;
                        }
                    }
                    //LIBERAR RECUSOS
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
            return utilizado;            
        }

        //VERIFICAR SI EL NOMBRE QUE SE INTENTA INGRESAR YA EXISTE EN BD
        private bool ExisteNombre(string pNombre, string pTableName)
        {
            //EJEMPLO QUERY --> SELECT nombre FROM area WHERE nombre=@pNombre

            //CONSULTA QUE TRAE TODOS LOS NOMBRE ACTUALES Y LOS GUARDA EN UNA LISTA
            List<string> names = new List<string>();
            string sql = string.Format("SELECT nombre FROM {0}", pTableName);

            bool existe = false;
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
                                //GUARDAMOS EN LISTA
                                names.Add(QuitaEspacios((string)rd["nombre"]));
                            }
                        }
                    }
                    cmd.Parameters.Clear();
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                    rd.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            if (names.Count>0)
            {
                //COMPARAMOS
                foreach (var item in names)
                {

                    if (item == pNombre)
                    { existe = true; break; }
                    else
                    { existe = false;}
                }
            } 
            
            return existe;
        }

        //PARA MANIPULAR LA TECLA TAB
        protected override bool ProcessDialogKey(Keys keyData)
        {
            string nombreAntiguo = "", limpiaCad = "";
            if (keyData == Keys.Tab)
            {
                if (txtNombreArea.ContainsFocus)
                {
                    if (txtNombreArea.Text == "") { lblMessage.Visible = true;lblMessage.Text = "Por favor ingresa un nombre";return false;}

                    limpiaCad = QuitaEspacios(txtNombreArea.Text);
                    
                    lblMessage.Visible = false;
                    if (txtidArea.ReadOnly == false)
                    {
                        if (ExisteNombre(limpiaCad, "area"))
                        { lblMessage.Visible = true; lblMessage.Text = "Nombre ingresado ya existe"; txtNombreArea.Focus(); return false; }
                        else
                            lblMessage.Visible = false;
                    }
                    else
                    {
                        if (viewArea.RowCount>0)
                        {
                            nombreAntiguo = viewArea.GetFocusedDataRow()["nombre"].ToString();
                            nombreAntiguo = QuitaEspacios(nombreAntiguo);
                            if (nombreAntiguo != limpiaCad)
                            {
                                if (ExisteNombre(limpiaCad, "area"))
                                { lblMessage.Visible = true; lblMessage.Text = "Nombre ingresado ya existe"; txtNombreArea.Focus(); return false; }

                            }
                            else
                                lblMessage.Visible = false;                            
                        }
                    }
                }
                if (txtNombreBanco.ContainsFocus)
                {
                    
                    if (txtNombreBanco.Text == "") { lblBanco.Visible = true; lblBanco.Text = "Por favor ingresa un nombre";txtNombreBanco.Focus(); return false; }

                    limpiaCad = QuitaEspacios(txtNombreBanco.Text);

                    if (txtIdBanco.ReadOnly == false)
                    {
                        if (ExisteNombre(limpiaCad, "banco"))
                        { lblBanco.Visible = true; lblBanco.Text = "Nombre ingresado ya existe"; txtNombreBanco.Focus(); return false; }
                        else
                            lblBanco.Visible = false;
                    }
                    else
                    {
                        if (viewBanco.RowCount > 0)
                        {
                            nombreAntiguo = viewBanco.GetFocusedDataRow()["nombre"].ToString();
                            nombreAntiguo = QuitaEspacios(nombreAntiguo);
                            if (nombreAntiguo != limpiaCad)
                            {
                                if (ExisteNombre(limpiaCad, "banco"))
                                { lblBanco.Visible = true; lblBanco.Text = "Nombre ingresado ya existe"; txtNombreBanco.Focus(); return false; }

                            }
                            else
                                lblBanco.Visible = false;
                        }
                    }
                }
                if (txtnombreCargo.ContainsFocus)
                {
                    if (txtnombreCargo.Text == "") { lblCargo.Visible = true; lblCargo.Text = "Por favor ingresa un nombre"; txtnombreCargo.Focus(); return false; }

                    limpiaCad = QuitaEspacios(txtnombreCargo.Text);

                    if (txtidCargo.ReadOnly == false)
                    {
                        if (ExisteNombre(limpiaCad, "cargo"))
                        { lblCargo.Visible = true; lblCargo.Text = "Nombre ingresado ya existe"; txtnombreCargo.Focus(); return false; }
                        else
                            lblCargo.Visible = false;
                    }
                    else
                    {
                        if (viewCargo.RowCount > 0)
                        {
                            nombreAntiguo = viewCargo.GetFocusedDataRow()["nombre"].ToString();
                            nombreAntiguo = QuitaEspacios(nombreAntiguo);
                            if (nombreAntiguo != limpiaCad)
                            {
                                if (ExisteNombre(limpiaCad, "cargo"))
                                { lblCargo.Visible = true; lblCargo.Text = "Nombre ingresado ya existe"; txtnombreCargo.Focus(); return false; }

                            }
                            else
                                lblCargo.Visible = false;
                        }
                    }
                }
                if (txtnombreCosto.ContainsFocus)
                {
                    if (txtnombreCosto.Text == "") { lblCosto.Visible = true; lblCosto.Text = "Por favor ingresa un nombre"; txtnombreCosto.Focus(); return false; }

                    limpiaCad = QuitaEspacios(txtnombreCosto.Text);

                    //if (txtidCosto.ReadOnly == false)
                    //{
                    //    if (ExisteNombre(limpiaCad, "ccosto"))
                    //    { lblCosto.Visible = true; lblCosto.Text = "Nombre ingresado ya existe"; txtnombreCosto.Focus(); return false; }
                    //    else
                    //        lblCosto.Visible = false;
                    //}
                    //else
                    //{
                    //    if (viewCosto.RowCount > 0)
                    //    {
                    //        nombreAntiguo = viewCosto.GetFocusedDataRow()["nombre"].ToString();
                    //        nombreAntiguo = QuitaEspacios(nombreAntiguo);
                    //        if (nombreAntiguo != limpiaCad)
                    //        {
                    //            if (ExisteNombre(limpiaCad, "ccosto"))
                    //            { lblCosto.Visible = true; lblCosto.Text = "Nombre ingresado ya existe"; txtnombreCosto.Focus(); return false; }

                    //        }
                    //        else
                    //            lblCosto.Visible = false;
                    //    }
                    //}
                }
                if (txtnombreNacion.ContainsFocus)
                {
                    if (txtnombreNacion.Text == "") { lblNacion.Visible = true; lblNacion.Text = "Por favor ingresa un nombre"; txtnombreNacion.Focus(); return false; }

                    limpiaCad = QuitaEspacios(txtnombreNacion.Text);

                    if (txtidNacion.ReadOnly == false)
                    {
                        if (ExisteNombre(limpiaCad, "nacion"))
                        { lblNacion.Visible = true; lblNacion.Text = "Nombre ingresado ya existe"; txtnombreNacion.Focus(); return false; }
                        else
                            lblNacion.Visible = false;
                    }
                    else
                    {
                        if (viewNacion.RowCount > 0)
                        {
                            nombreAntiguo = viewNacion.GetFocusedDataRow()["nombre"].ToString();
                            nombreAntiguo = QuitaEspacios(nombreAntiguo);

                            if (nombreAntiguo != limpiaCad)
                            {
                                if (ExisteNombre(limpiaCad, "nacion"))
                                { lblNacion.Visible = true; lblNacion.Text = "Nombre ingresado ya existe"; txtnombreNacion.Focus(); return false; }

                            }
                            else
                                lblNacion.Visible = false;
                        }
                    }
                }
                if (txtnombrePago.ContainsFocus)
                {
                    if (txtnombrePago.Text == "") { lblPago.Visible = true; lblPago.Text = "Por favor ingresa un nombre"; txtnombrePago.Focus(); return false; }
                    limpiaCad = QuitaEspacios(txtnombrePago.Text);
                    if (txtidPago.ReadOnly == false)
                    {
                        if (ExisteNombre(limpiaCad, "formapago"))
                        { lblPago.Visible = true; lblPago.Text = "Nombre ingresado ya existe"; txtnombrePago.Focus(); return false; }
                        else
                            lblNacion.Visible = false;
                    }
                    else
                    {
                        if (viewPago.RowCount > 0)
                        {
                            nombreAntiguo = viewPago.GetFocusedDataRow()["nombre"].ToString();
                            nombreAntiguo = QuitaEspacios(nombreAntiguo);
                            if (nombreAntiguo != limpiaCad)
                            {
                                if (ExisteNombre(limpiaCad,"formapago"))
                                { lblPago.Visible = true; lblPago.Text = "Nombre ingresado ya existe"; txtnombrePago.Focus(); return false; }

                            }
                            else
                                lblPago.Visible = false;
                        }
                    }
                }
                if (txtNombrecaja.ContainsFocus)
                {
                    if (txtNombrecaja.Text.Length == 0)
                    { lblmsgCaja.Visible = true; lblmsgCaja.Text = "Ingresa un nombre"; return false; ; }

                    lblmsgCaja.Visible = false;
                }
                if (txtCodCausal.ContainsFocus)
                {
                    //SOLO INSERT   
                    if (UpdateCausal == false)
                    {
                        if (txtCodCausal.Text.Length == 0)
                        { lblmsgCausal.Visible = true; lblmsgCausal.Text = "Ingresa código"; return false; }

                        if (ExisteCausal(Convert.ToInt32(txtCodCausal.Text)))
                        { lblmsgCausal.Visible = true; lblmsgCausal.Text = "Código ingresado ya existe"; return false; }

                        lblmsgCausal.Visible = false;
                    }              
                }
                if (txtCodEsco.ContainsFocus)
                {
                    //INSERT
                    if (UpdateEscolaridad == false)
                    {
                        Escolaridad escolar = new Escolaridad();

                        if (txtCodEsco.Text.Length == 0)
                        { lblMsgEsco.Visible = true; lblMsgEsco.Text = "Por favor ingresa un código"; return false; }

                        if (escolar.ExisteCodigo(Convert.ToInt32(txtCodEsco.Text)))
                        { lblMsgEsco.Visible = true; lblMsgEsco.Text = "Código ingresado ya existe"; return false; }

                        lblMsgEsco.Visible = false;
                    }
                }
                if (txtCodHorario.ContainsFocus)
                {
                    if (UpdateHorario == false)
                    {
                        if (txtCodHorario.Text.Length == 0)
                        { lblMsgHorario.Visible = true; lblmsgCaja.Text = "Por favor ingresa un código"; return false; }

                        Horario hor = new Horario();

                        if (hor.ExisteRegistro(Convert.ToInt32(txtCodHorario.Text)))
                        {
                            lblMsgHorario.Visible = true;
                            lblMsgHorario.Text = "Código ingresado ya existe";
                            return false;
                        }

                        lblMsgHorario.Visible = false;
                    }
                }                
            }
            return base.ProcessDialogKey(keyData);
        }

        //QUITAR ESPACIOS EN BLANCO ANTES DE COMPARAR SI EL NOMBRE INGRESADO YA EXISTE EN BD
        private string QuitaEspacios(string cadena)
        {
            string cad = "";
            if (cadena.Length == 0) return "";

            for (int i = 0; i < cadena.Length; i++)
            {
                if (cadena[i].ToString() != " ")
                    cad = cad + cadena[i];
            }

            return cad;
        }

        #region "REGISTROS LOG"
        //METODO QUE RETORNA UNA TABLA HASH CON LOS DATOS DEL REGISTRO EN EVALUACION
        //PARAMETROS DE ENTRADA: ID REGISTRO, NOMBRE TABLA
        private Hashtable PrecargaDatos(int pId, string pTabla)
        {
            Hashtable datos = new Hashtable();
            string sql = string.Format("SELECT nombre, dato01, dato02 FROM {0} WHERE id={1}", pTabla, pId);
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
                                //LLENAMOS LISTA
                                datos.Add("nombre" ,(string)rd["nombre"]);
                                datos.Add("dato01", (string)rd["dato01"]);
                                datos.Add("datos02", (string)rd["dato02"]);                                
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
            return datos;
        }

        //METODO PARA COMPARAR
        private void CompararDatos(string pNombre, string pDato01, string pDato02,string Tabla,  Hashtable Listado)
        {
            if (Listado.Count > 0)
            {
                try
                {
                    //COMPARAMOS VALORES
                    if ((string)Listado["nombre"] != pNombre)
                    {
                        logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA NOMBRE EN " + Tabla + " " + (string)Listado["nombre"], Tabla, (string)Listado["nombre"], pNombre, "MODIFICAR");
                        log.Log();
                    }
                    if ((string)Listado["dato01"] != pDato01)
                    {
                        logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA DATO 01 EN " + Tabla + " " + (string)Listado["nombre"], Tabla, (string)Listado["dato01"], pDato01, "MODIFICAR");
                        log.Log();
                    }
                    if ((string)Listado["dato02"] != pDato02)
                    {
                        logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA NOMBRE EN " + Tabla + " " + (string)Listado["nombre"], Tabla, (string)Listado["dato02"], pDato02, "MODIFICAR");
                        log.Log();
                    }
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show(ex.Message);
                }
               
            }
        }
        #endregion#

        #endregion

        #region "CONTROLES AREA"

        private void btnNuevoArea_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD   
            Sesion.NuevaActividad();

            fnLimpiar(txtidArea, txtNombreArea, txtdato1Area, txtdato2Area, btnEliminarArea, "area");
            if (op.Cancela == false)
            {
                op.SetButtonProperties(btnNuevoArea, 2);
                op.Cancela = true;

                //CAMBIAMOS COLOR TEXTBOX
                txtNombreArea.Properties.Appearance.BackColor = Color.LightGoldenrodYellow;
            }
            else
            {
                op.SetButtonProperties(btnNuevoArea, 1);
                op.Cancela = false;
                fnCargarCampos(txtidArea, txtNombreArea, txtdato1Area, txtdato2Area, btnEliminarArea, viewArea);
                txtNombreArea.Properties.Appearance.BackColor = Color.White;            
                
            }            
        }        

        private void btnGuardarArea_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();
            
            //SQL INSERR
            string sqlInsert = "INSERT INTO area(id, nombre, dato01, dato02) VALUES(@pId, @pNombre, @pDato1, @pDato2)";
            //SQL UPDATE
            string sqlUpdate = "UPDATE area SET nombre=@pNombre, dato01=@pDato1, dato02=@pDato2 WHERE id=@pId";
            //SQL BUSQUEDA
            string sqlBusqueda = "SELECT * FROM area WHERE id=@pId";

            if (txtidArea.Text == "") { XtraMessageBox.Show("Debes ingresar el id", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtidArea.Focus(); return; }
            if (txtNombreArea.Text == "") { XtraMessageBox.Show("Debes llenar el campo Nombre", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtNombreArea.Focus(); return; }

            string cadLimpia = "";
            cadLimpia = QuitaEspacios(txtNombreArea.Text);
            int id = 0;
            string nombreAntiguo = "";
            if (txtidArea.ReadOnly)
            {
                //HACEMOS UPDATE
                if (viewArea.RowCount > 0)
                {
                    id = int.Parse(viewArea.GetFocusedDataRow()["id"].ToString());
                    nombreAntiguo = viewArea.GetFocusedDataRow()["nombre"].ToString();
                    nombreAntiguo = QuitaEspacios(nombreAntiguo);

                    if (nombreAntiguo != cadLimpia)
                    {
                        if (ExisteNombre(cadLimpia, "area"))
                        { XtraMessageBox.Show("Nombre ingresado ya existe", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);txtNombreArea.Focus();return;}
                    }
                }               

                bool mod = fnModificarRegistro(id, txtNombreArea, txtdato1Area, txtdato2Area, sqlUpdate, "AREA");
                //SI MOD ES TRUE CARGAMOS GRILLA
                if (mod)
                {
                    //GUARDAR EVENTO EN LOG
                   // logRegistro log = new logRegistro(User.getUser(), "SE INGRESA NUEVA AREA", "AREA", "0", txtNombreArea.Text, "INGRESAR");
                   // log.Log();

                    fnSistema.spllenaGridView(gridArea, SqlArea);
                    fnCargarCampos(txtidArea, txtNombreArea, txtdato1Area, txtdato2Area, btnEliminarArea, viewArea, 0);                                       

                    //SI LA VARIABLE OPENER ES DISTINTA DE NULL ES PORQUE EL FORM SE INVOCO DESDE FORM EMPLEADO
                    if (opener != null)
                        opener.RecargarComboArea();
                }
            }
            else
            {
                //HACEMOS INSERT
                bool existe = fnVerId(int.Parse(txtidArea.Text), sqlBusqueda);
                if (existe) { XtraMessageBox.Show("Id ingresado ya existe", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

                //VERIFICAR QUE EL NOMBRE INGRESADO NO EXITA EN BD
                if (ExisteNombre(cadLimpia, "area"))
                { XtraMessageBox.Show("Nombre ingresado ya existe", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);txtNombreArea.Focus();return;}

                bool nuevo = fnNuevoRegistro(txtidArea, txtNombreArea, txtdato1Area, txtdato2Area, sqlInsert);
                //SI NUEVO ES TRUE ES INSERT CORRECTO CARGAMOS GRILLA
                if (nuevo)
                {
                    //GUARDAR EVENTO EN LOG
                    logRegistro log = new logRegistro(User.getUser(), "SE INGRESA NUEVA AREA", "AREA", "0", txtNombreArea.Text, "INGRESAR");
                    log.Log();

                    fnSistema.spllenaGridView(gridArea, SqlArea);
                    fnCargarCampos(txtidArea, txtNombreArea, txtdato1Area, txtdato2Area, btnEliminarArea, viewArea, 0);

                    op.Cancela = false;
                    op.SetButtonProperties(btnNuevoArea, 1);
              
                    
                    //SI LA VARIABLE OPENER ES DISTINTA DE NULL ES PORQUE EL FORM SE INVOCO DESDE FORM EMPLEADO
                    if (opener != null)
                        opener.RecargarComboArea();
                }
            }
        }

        private void btnEliminarArea_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD   
            Sesion.NuevaActividad();
            
            //SQL BUSQUEDA
            string sqlBusqueda = "SELECT * FROM area WHERE id=@pId";
            //SQL ELIMINAR
            string sqlDelete = "DELETE FROM area WHERE id=@pId";

            string area = "";
            if (txtidArea.Text == "")
            {
                XtraMessageBox.Show("Id no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtidArea.Focus();
                return;
            }
            //VALIDAR ID EN BD
            bool existe = fnVerId(int.Parse(txtidArea.Text), sqlBusqueda);
            //SI EXISTE ES FALSE NO ES VALIDO PARA ELIMINAR
            if (existe == false) { XtraMessageBox.Show("Registro no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtidArea.Focus(); return; }

            //VERIFICAR SI EL REGISTRO ESTA SIENDO USADO POR UN TRABAJADOR
            bool usado = false;
            usado = fnRegistroEnUso(txtidArea.Text, "SELECT area FROM trabajador WHERE area=@pId");
            if (usado)
            {
                //NO SE PUEDE ELIMINAR
                XtraMessageBox.Show("Este registro no se puede eliminar porque esta siendo utilizado por un trabajador", "Denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (viewArea.RowCount > 0) { area = viewArea.GetFocusedDataRow()["nombre"].ToString(); }
            DialogResult dialogo = XtraMessageBox.Show("¿Seguro Desea eliminar el area " + area + " ?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogo == DialogResult.Yes)
            {
                //LLAMAR AL METODO ELIMINAR
                //OBTENER EL ID DEL REGISTRO DE LA GRILLA SELECCIONADA
                int Id = 0;
                if (viewArea.RowCount > 0)
                {
                    Id = int.Parse(viewArea.GetFocusedDataRow()["id"].ToString());
                    bool del = fnEliminarRegistro(Id, sqlDelete);
                    //SI DEL ES TRUE ES PORQUE SE ELIMINO CORRECTAMENTE EL REGISTRO 
                    if (del)
                    {
                        //GUARDAR EVENTO EN LOG
                        logRegistro log = new logRegistro(User.getUser(), "SE ELIMINA AREA", "AREA", area, "0", "ELIMINAR");
                        log.Log();

                        //CARGAMOS GRILLA
                        fnSistema.spllenaGridView(gridArea, SqlArea);
                        fnCargarCampos(txtidArea, txtNombreArea, txtdato1Area, txtdato2Area, btnEliminarArea, viewArea, 0);                      

                        if(viewArea.RowCount == 0)
                            fnLimpiar(txtidArea, txtNombreArea, txtdato1Area, txtdato2Area, btnEliminarArea, "area");

                        //SI LA VARIABLE OPENER ES DISTINTA DE NULL ES PORQUE EL FORM SE INVOCO DESDE FORM EMPLEADO
                        if (opener != null)
                            opener.RecargarComboArea();
                    }
                }
            }

        }

        private void txtidArea_KeyPress(object sender, KeyPressEventArgs e)
        {
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

        private void gridArea_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD   
            Sesion.NuevaActividad();
            
            fnCargarCampos(txtidArea, txtNombreArea, txtdato1Area, txtdato2Area, btnEliminarArea, viewArea);
            op.Cancela = false;
            op.SetButtonProperties(btnNuevoArea, 1);
          
           
        }

        private void gridArea_KeyUp(object sender, KeyEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            fnCargarCampos(txtidArea, txtNombreArea, txtdato1Area, txtdato2Area, btnEliminarArea, viewArea);
            op.Cancela = false;
            op.SetButtonProperties(btnNuevoArea, 1);
        }
        #endregion

        #region "CONTROLES BANCO"      

        private void btnNuevoBanco_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            fnLimpiar(txtIdBanco, txtNombreBanco, txtDato1Banco, txtDato2Banco, btnEliminarBanco, "banco");
            if (op.Cancela == false)
            {
                op.Cancela = true;
                op.SetButtonProperties(btnNuevoBanco, 2);     
                txtNombreBanco.Properties.Appearance.BackColor = Color.LightGoldenrodYellow;
            }
            else
            {
                op.Cancela = false;
                op.SetButtonProperties(btnNuevoBanco, 1);
                fnCargarCampos(txtIdBanco, txtNombreBanco, txtDato1Banco, txtDato2Banco, btnEliminarBanco, viewBanco);

                txtNombreBanco.Properties.Appearance.BackColor = Color.White;
            }
        }        

        private void btnguardarBanco_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();
            
            //SQL
            string sqlUpdate = "UPDATE banco SET Nombre=@pNombre, dato01=@pDato1, dato02=@pDato2 WHERE id=@pId";
            //SQL
            string sqlInsert = "INSERT INTO banco(id, nombre, dato01, dato02) VALUES(@pId, @pNombre, @pDato1, @pDato2)";
            //SQL BUSQUEDA
            //sql 
            string sqlBusqueda = "SELECT * FROM banco WHERE id=@pId";

            if (txtIdBanco.Text == "") { XtraMessageBox.Show("Debes ingresar el id", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtIdBanco.Focus(); return; }
            if (txtNombreBanco.Text == "") { XtraMessageBox.Show("Debes llenar el campo Nombre", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtNombreBanco.Focus(); return; }
            if (txtDato1Banco.Text == "") { XtraMessageBox.Show("Debes ingresar el código SBIF", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtDato1Banco.Focus(); return; }
            int id = 0;
            string nombreAntiguo = "", cadlimpia = "";
            cadlimpia = QuitaEspacios(txtNombreBanco.Text);
            
            if (txtIdBanco.ReadOnly)
            {
                //HACEMOS UPDATE
                if (viewBanco.RowCount > 0)
                {
                    id = int.Parse(viewBanco.GetFocusedDataRow()["id"].ToString());
                    nombreAntiguo = viewBanco.GetFocusedDataRow()["nombre"].ToString();
                    nombreAntiguo = QuitaEspacios(nombreAntiguo);
                    if (nombreAntiguo != cadlimpia)
                    {
                        if (ExisteNombre(cadlimpia, "banco"))
                        { XtraMessageBox.Show("Nombre ingresado ya existe", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);txtNombreBanco.Focus();return;}
                    }
                }

                bool mod = fnModificarRegistro(id, txtNombreBanco, txtDato1Banco, txtDato2Banco, sqlUpdate, "BANCO");
                //SI MOD ES TRUE CARGAMOS GRILLA
                if (mod)
                {
                    //GUARDAR EVENTO EN LOG
                    //logRegistro log = new logRegistro(User.getUser(), "SE HA MODIFICADO REGISTRO CON ID " + id, fnSistema.pgDatabase, "BANCO");
                    //log.Log();

                    fnSistema.spllenaGridView(gridBanco, SqlBanco);
                    fnCargarCampos(txtIdBanco, txtNombreBanco, txtDato1Banco, txtDato2Banco, btnEliminarBanco, viewBanco, 0);
               

                    //SI LA VARIABLE OPENER ES DISTINTA DE NULL ES PORQUE EL FORM SE INVOCO DESDE FORM EMPLEADO
                    if (opener != null)
                        opener.RecargarComboBanco();
                }
            }
            else
            {
                //HACEMOS INSERT
                bool existe = fnVerId(int.Parse(txtIdBanco.Text), sqlBusqueda);
                if (existe) { XtraMessageBox.Show("Id ingresado ya existe", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

                //VERIFICAR SI EL NOMBRE INGRESADO YA EXISTE
                if (ExisteNombre(cadlimpia, "banco"))
                { XtraMessageBox.Show("Nombre ingresado ya existe", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);txtNombreArea.Focus();return;}

                bool nuevo = fnNuevoRegistro(txtIdBanco, txtNombreBanco, txtDato1Banco, txtDato2Banco, sqlInsert);
                //SI NUEVO ES TRUE ES INSERT CORRECTO CARGAMOS GRILLA
                if (nuevo)
                {
                    //GUARDAR EVENTO EN LOG REGISTRO
                    logRegistro log = new logRegistro(User.getUser(), "SE INGRESA NUEVO BANCO", "BANCO", "0", txtNombreBanco.Text, "INGRESAR");
                    log.Log();

                    fnSistema.spllenaGridView(gridBanco, SqlBanco);
                    fnCargarCampos(txtIdBanco, txtNombreBanco, txtDato1Banco, txtDato2Banco, btnEliminarBanco, viewBanco, 0);

                    op.Cancela = false;
                    op.SetButtonProperties(btnNuevoBanco, 1);                

                    //SI LA VARIABLE OPENER ES DISTINTA DE NULL ES PORQUE EL FORM SE INVOCO DESDE FORM EMPLEADO
                    if (opener != null)
                        opener.RecargarComboBanco();
                }
            }
        }

        private void btnEliminarBanco_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();
            
            //SQL DELETE
            string sqlDelete = "DELETE FROM banco WHERE id=@pId";
            //SQL BUSQUEDA            
            string sqlBusqueda = "SELECT * FROM banco WHERE id=@pId";

            string banco = "";
            if (txtIdBanco.Text == "")
            {
                XtraMessageBox.Show("Id no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtIdBanco.Focus();
                return;
            }
            //VALIDAR ID EN BD
            bool existe = fnVerId(int.Parse(txtIdBanco.Text), sqlBusqueda);
            //SI EXISTE ES FALSE NO ES VALIDO PARA ELIMINAR
            if (existe == false) { XtraMessageBox.Show("Registro no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtIdBanco.Focus(); return; }

            //VALIDAR QUE EL BANCO QUE SE DESEA ELIMINAR NO ESTE SIENDO USADO POR UN EMPLEADO
            bool usado = false;
            usado = fnRegistroEnUso(txtIdBanco.Text, "SELECT banco FROM trabajador WHERE banco=@pId");
            if (usado)
            {
                //REGISTRO NO SE PUEDE ELIMINAR
                XtraMessageBox.Show("Banco no se puede eliminar porque esta siendo utilizado por un trabajador", "Denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (viewBanco.RowCount > 0) { banco = viewBanco.GetFocusedDataRow()["nombre"].ToString(); }
            DialogResult dialogo = XtraMessageBox.Show("¿Seguro Desea eliminar el banco " + banco + " ?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogo == DialogResult.Yes)
            {
                //LLAMAR AL METODO ELIMINAR
                //OBTENER EL ID DEL REGISTRO DE LA GRILLA SELECCIONADA
                int Id = 0;
                if (viewBanco.RowCount > 0)
                {
                    Id = int.Parse(viewBanco.GetFocusedDataRow()["id"].ToString());
                    bool del = fnEliminarRegistro(Id, sqlDelete);
                    //SI DEL ES TRUE ES PORQUE SE ELIMINO CORRECTAMENTE EL REGISTRO 
                    if (del)
                    {
                        //GUARDAR EVENTO EN LOG
                        logRegistro log = new logRegistro(User.getUser(), "SE ELIMINA BANCO", "BANCO", banco, "0", "ELIMINAR");
                        log.Log();

                        //CARGAMOS GRILLA
                        fnSistema.spllenaGridView(gridBanco, SqlBanco);
                        fnCargarCampos(txtIdBanco, txtNombreBanco, txtDato1Banco, txtDato2Banco, btnEliminarBanco, viewBanco, 0);

                        if (viewBanco.RowCount == 0)
                            fnLimpiar(txtIdBanco, txtNombreBanco, txtDato1Banco, txtDato2Banco, btnEliminarBanco, "banco");

                        //SI LA VARIABLE OPENER ES DISTINTA DE NULL ES PORQUE EL FORM SE INVOCO DESDE FORM EMPLEADO
                        if (opener != null)
                            opener.RecargarComboBanco();
                    }
                }
            }
        }

        private void gridBanco_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            fnCargarCampos(txtIdBanco, txtNombreBanco, txtDato1Banco, txtDato2Banco, btnEliminarBanco, viewBanco);

            //RESET BOTON NUEVO
            op.SetButtonProperties(btnNuevoBanco, 1);
            op.Cancela = false;
        }

        private void gridBanco_KeyUp(object sender, KeyEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            fnCargarCampos(txtIdBanco, txtNombreBanco, txtDato1Banco, txtDato2Banco, btnEliminarBanco, viewBanco);

            //RESET BOTON NUEVO
            op.Cancela = false;
            op.SetButtonProperties(btnNuevoBanco, 1);

        }

        private void txtIdBanco_KeyPress(object sender, KeyPressEventArgs e)
        {
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

        #endregion

        #region "CONTROLES CARGO"        

        private void btnNuevoCargo_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            fnLimpiar(txtidCargo, txtnombreCargo, txtdato1Cargo, txtdato2Cargo, btnEliminarCargo, "cargo");
            if (op.Cancela == false)
            {
                op.Cancela = true;
                op.SetButtonProperties(btnNuevoCargo, 2);              
                txtnombreCargo.Properties.Appearance.BackColor = Color.LightGoldenrodYellow;
            }
            else
            {
                op.SetButtonProperties(btnNuevoCargo, 1);
                op.Cancela = false;

                fnCargarCampos(txtidCargo, txtnombreCargo, txtdato1Cargo, txtdato2Cargo, btnEliminarCargo, viewCargo);

                txtnombreCargo.Properties.Appearance.BackColor = Color.White;
            }
        }

        private void btnGuardarCargo_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();
            
            //SQL
            string sqlUpdate = "UPDATE cargo SET Nombre=@pNombre, dato01=@pDato1, dato02=@pDato2 WHERE id=@pId";
            //SQL
            string sqlInsert = "INSERT INTO cargo(id, nombre, dato01, dato02) VALUES(@pId, @pNombre, @pDato1, @pDato2)";
            //SQL BUSQUEDA
            //sql 
            string sqlBusqueda = "SELECT * FROM cargo WHERE id=@pId";


            if (txtidCargo.Text == "") { XtraMessageBox.Show("Debes ingresar el id", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtidCargo.Focus(); return; }
            if (txtnombreCargo.Text == "") { XtraMessageBox.Show("Debes llenar el campo Nombre", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtnombreCargo.Focus(); return; }

            int id = 0;
            string nombreAntiguo = "", cadLimpia = "";
            cadLimpia = QuitaEspacios(txtnombreCargo.Text);
            if (txtidCargo.ReadOnly)
            {
                //HACEMOS UPDATE
                if (viewCargo.RowCount > 0)
                {
                    id = int.Parse(viewCargo.GetFocusedDataRow()["id"].ToString());
                    nombreAntiguo = viewCargo.GetFocusedDataRow()["nombre"].ToString();
                    nombreAntiguo = QuitaEspacios(nombreAntiguo);

                    if (nombreAntiguo != cadLimpia)
                    {
                        if (ExisteNombre(cadLimpia, "cargo"))
                        { XtraMessageBox.Show("Nombre ingresado ya existe", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);txtnombreCargo.Focus();return;}
                    }
                }
                bool mod = fnModificarRegistro(id, txtnombreCargo, txtdato1Cargo, txtdato2Cargo, sqlUpdate, "CARGO");
                //SI MOD ES TRUE CARGAMOS GRILLA
                if (mod)
                {
                    //LOG REGISTRO
                    //logRegistro log = new logRegistro(User.getUser(), "SE HA MODIFICADO REGISTRO CON ID" + id, fnSistema.pgDatabase, "CARGO");
                    //log.Log();

                    fnSistema.spllenaGridView(gridCargo, SqlCargo);
                    fnCargarCampos(txtidCargo, txtnombreCargo, txtdato1Cargo, txtdato2Cargo, btnEliminarCargo, viewCargo, 0);                   

                    //SI VARIABLE OPENER ES DISTINTA DE NULL ES PORQUE EL FORM SE INVOCO DESDE FORM EMPLEADO
                    if (opener != null)
                        opener.RecargarComboCargo();
                }
            }
            else
            {
                //HACEMOS INSERT
                bool existe = fnVerId(int.Parse(txtidCargo.Text), sqlBusqueda);
                if (existe) { XtraMessageBox.Show("Id ingresado ya existe", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

                // VALIDAR QUE NOMBRE DE CARGO EXISTE
                if (ExisteNombre(cadLimpia, "cargo"))
                { XtraMessageBox.Show("Nombre ingresado ya existe", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); txtnombreCargo.Focus(); return; }

                bool nuevo = fnNuevoRegistro(txtidCargo, txtnombreCargo, txtdato1Cargo, txtdato2Cargo, sqlInsert);
                //SI NUEVO ES TRUE ES INSERT CORRECTO CARGAMOS GRILLA
                if (nuevo)
                {
                    //GUARDAR EVENTO EN LOG
                    logRegistro log = new logRegistro(User.getUser(), "SE INGRESA NUEVO CARGO", "CARGO", "0", txtnombreCargo.Text, "INGRESAR");
                    log.Log();

                    fnSistema.spllenaGridView(gridCargo, SqlCargo);
                    fnCargarCampos(txtidCargo, txtnombreCargo, txtdato1Cargo, txtdato2Cargo, btnEliminarCargo, viewCargo, 0);

                    op.Cancela = false;
                    op.SetButtonProperties(btnNuevoCargo, 1);


                    //SI VARIABLE OPENER ES DISTINTA DE NULL ES PORQUE EL FORM SE INVOCO DESDE FORM EMPLEADO
                    if (opener != null)
                        opener.RecargarComboCargo();
                }
            }
        }

        private void btnEliminarCargo_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            //SQL DELETE
            string sqlDelete = "DELETE FROM cargo WHERE id=@pId";
            //SQL BUSQUEDA            
            string sqlBusqueda = "SELECT * FROM cargo WHERE id=@pId";

            string cargo = "";
            if (txtidCargo.Text == "")
            {
                XtraMessageBox.Show("Id no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtidCargo.Focus();
                return;
            }
            //VALIDAR ID EN BD
            bool existe = fnVerId(int.Parse(txtidCargo.Text), sqlBusqueda);
            //SI EXISTE ES FALSE NO ES VALIDO PARA ELIMINAR
            if (existe == false) { XtraMessageBox.Show("Registro no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtidCargo.Focus(); return; }

            //VERIFICA SI EL REGISTRO ESTA SIENDO USADO POR UN TRABAJADOR
            bool usado = false;
            usado = fnRegistroEnUso(txtidCargo.Text, "SELECT cargo FROM trabajador WHERE cargo=@pId");
            if (usado)
            {
                //NO SE PUEDE ELIMINAR EL REGISTRO
                XtraMessageBox.Show("No se puede eliminar este cargo porque esta siendo utilizado por un trabajador", "Denegado" ,MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (viewCargo.RowCount > 0) { cargo = viewCargo.GetFocusedDataRow()["nombre"].ToString(); }
            DialogResult dialogo = XtraMessageBox.Show("¿Seguro Desea eliminar el cargo " + cargo + " ?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogo == DialogResult.Yes)
            {
                //LLAMAR AL METODO ELIMINAR
                //OBTENER EL ID DEL REGISTRO DE LA GRILLA SELECCIONADA
                int Id = 0;
                if (viewCargo.RowCount > 0)
                {
                    Id = int.Parse(viewCargo.GetFocusedDataRow()["id"].ToString());
                    bool del = fnEliminarRegistro(Id, sqlDelete);
                    //SI DEL ES TRUE ES PORQUE SE ELIMINO CORRECTAMENTE EL REGISTRO 
                    if (del)
                    {
                        //CARGAMOS GRILLA
                        fnSistema.spllenaGridView(gridCargo, SqlCargo);
                        fnCargarCampos(txtidCargo, txtnombreCargo, txtdato1Cargo, txtdato2Cargo, btnEliminarCargo, viewCargo, 0);

                        //GUARDAR EVENTO EN LOG
                        logRegistro log = new logRegistro(User.getUser(), "SE ELIMINAR CARGO", "CARGO", cargo, "0",  "ELIMINAR");
                        log.Log();

                        if (viewCargo.RowCount == 0)
                        {                      
                            fnLimpiar(txtidCargo, txtnombreCargo, txtdato1Cargo, txtdato2Cargo, btnEliminarCargo, "cargo");
                        }
                            

                        //SI VARIABLE OPENER ES DISTINTA DE NULL ES PORQUE EL FORM SE INVOCO DESDE FORM EMPLEADO
                        if (opener != null)
                            opener.RecargarComboCargo();
                    }
                }
            }
        }

        private void txtidCargo_KeyPress(object sender, KeyPressEventArgs e)
        {

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

        private void gridCargo_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            fnCargarCampos(txtidCargo, txtnombreCargo, txtdato1Cargo, txtdato2Cargo, btnEliminarCargo, viewCargo);

            //RESET BOTON NUEVO
            op.Cancela = false;
            op.SetButtonProperties(btnNuevoCargo, 1);
            
        }

        private void gridCargo_KeyUp(object sender, KeyEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            fnCargarCampos(txtidCargo, txtnombreCargo, txtdato1Cargo, txtdato2Cargo, btnEliminarCargo, viewCargo);

            //RESET BOTON NUEVO
            op.Cancela = false;
            op.SetButtonProperties(btnNuevoCargo, 1);

        }

        #endregion

        #region "CONTROLES COSTO"        

        private void btnNuevoCosto_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            fnLimpiar(txtidCosto, txtnombreCosto, txtdato1Costo, txtdato2Costo, btnEliminarCosto, "ccosto");
            if (op.Cancela == false)
            {
                op.Cancela = true;
                op.SetButtonProperties(btnNuevoCosto, 2);               
                txtnombreCosto.Properties.Appearance.BackColor = Color.LightGoldenrodYellow;
            }
            else
            {
                op.Cancela = false;
                op.SetButtonProperties(btnNuevoCosto, 1);                
                fnCargarCampos(txtidCosto, txtnombreCosto, txtdato1Costo, txtdato2Costo, btnEliminarCosto, viewCosto);
                txtnombreCosto.Properties.Appearance.BackColor = Color.White;
            }
        }

        private void btnGuardarCosto_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();
            
            //SQL
            string sqlUpdate = "UPDATE ccosto SET Nombre=@pNombre, dato01=@pDato1, dato02=@pDato2 WHERE id=@pId";
            //SQL
            string sqlInsert = "INSERT INTO ccosto(id, nombre, dato01, dato02) VALUES(@pId, @pNombre, @pDato1, @pDato2)";
            //SQL BUSQUEDA
            //sql 
            string sqlBusqueda = "SELECT * FROM ccosto WHERE id=@pId";


            if (txtidCosto.Text == "") { XtraMessageBox.Show("Debes ingresar el id", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtidCosto.Focus(); return; }
            if (txtnombreCosto.Text == "") { XtraMessageBox.Show("Debes llenar el campo Nombre", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtnombreCosto.Focus(); return; }
            int id = 0;
            string nombreAntiguo = "", cadLimpia = "";
            cadLimpia = QuitaEspacios(txtnombreCosto.Text);
            if (txtidCosto.ReadOnly)
            {
                //HACEMOS UPDATE
                if (viewCosto.RowCount > 0)
                {
                    id = int.Parse(viewCosto.GetFocusedDataRow()["id"].ToString());
                    nombreAntiguo = viewCosto.GetFocusedDataRow()["nombre"].ToString();
                    nombreAntiguo = QuitaEspacios(nombreAntiguo);
                    if (nombreAntiguo != cadLimpia)
                    {
                        if (ExisteNombre(cadLimpia, "ccosto"))
                        { XtraMessageBox.Show("Nombre ingresado ya existe", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);txtnombreCosto.Focus();return; }
                    }
                }

                bool mod = fnModificarRegistro(id, txtnombreCosto, txtdato1Costo, txtdato2Costo, sqlUpdate, "CCOSTO");
                //SI MOD ES TRUE CARGAMOS GRILLA
                if (mod)
                {
                    //GUARDAR EVENTO EN LOG
                    //logRegistro log = new logRegistro(User.getUser(), "SE HA MODIFICADO REGISTRO CON ID " + id, fnSistema.pgDatabase, "CCOSTO");
                    //log.Log();

                    fnSistema.spllenaGridView(gridCosto, SqlCentroCosto);
                    fnCargarCampos(txtidCosto, txtnombreCosto, txtdato1Costo, txtdato2Costo, btnEliminarCosto, viewCosto, 0);

                    
                    //SI VARAIBLE OPENER ES DISTINTA DE NULL ES PORQUE FORM SE INVOCO DESDE FORM EMPLEADO
                    if (opener != null)
                        opener.RecargarCombocCosto();
                }
            }
            else
            {
                //HACEMOS INSERT
                bool existe = fnVerId(int.Parse(txtidCosto.Text), sqlBusqueda);
                if (existe) { XtraMessageBox.Show("Id ingresado ya existe", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

                //VERIFICAR SI EXISTE NOMBRE
                //if (ExisteNombre(cadLimpia, "ccosto"))
                //{ XtraMessageBox.Show("Nombre ingresado ya existe", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); txtnombreCosto.Focus(); return; }

                bool nuevo = fnNuevoRegistro(txtidCosto, txtnombreCosto, txtdato1Costo, txtdato2Costo, sqlInsert);
                //SI NUEVO ES TRUE ES INSERT CORRECTO CARGAMOS GRILLA
                if (nuevo)
                {
                    //GUARDAR EVENTO EN LOG
                    logRegistro log = new logRegistro(User.getUser(), "SE INGRESA NUEVO CENTRO COSTO", "CCOSTO", "0", txtnombreCosto.Text, "INGRESAR");
                    log.Log();

                    fnSistema.spllenaGridView(gridCosto, SqlCentroCosto);
                    fnCargarCampos(txtidCosto, txtnombreCosto, txtdato1Costo, txtdato2Costo, btnEliminarCosto, viewCosto, 0);

                    op.Cancela = false;
                    op.SetButtonProperties(btnNuevoCosto, 1);


                    //SI VARAIBLE OPENER ES DISTINTA DE NULL ES PORQUE FORM SE INVOCO DESDE FORM EMPLEADO
                    if (opener != null)
                        opener.RecargarCombocCosto();
                }
            }
        }

        private void btnEliminarCosto_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();
            
            //SQL DELETE
            string sqlDelete = "DELETE FROM ccosto WHERE id=@pId";
            //SQL BUSQUEDA            
            string sqlBusqueda = "SELECT * FROM ccosto WHERE id=@pId";

            string costo = "";
            if (txtidCosto.Text == "")
            {
                XtraMessageBox.Show("Id no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtidCosto.Focus();
                return;
            }
            //VALIDAR ID EN BD
            bool existe = fnVerId(int.Parse(txtidCosto.Text), sqlBusqueda);
            //SI EXISTE ES FALSE NO ES VALIDO PARA ELIMINAR
            if (existe == false) { XtraMessageBox.Show("Registro no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtidCosto.Focus(); return; }

            //VERIFICAR QUE EL REGISTRO NO ESTE SIENDO USADO POR UN TRABAJADOR
            bool usado = false;
            usado = fnRegistroEnUso(txtidCosto.Text, "SELECT ccosto FROM trabajador WHERE ccosto=@pId");
            if (usado)
            {
                //NO SE PUEDE ELIMINAR 
                XtraMessageBox.Show("Este registro no se puede eliminar porque esta siendo utilizado por un trabajador", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (viewCosto.RowCount > 0) { costo = viewCosto.GetFocusedDataRow()["nombre"].ToString(); }
            DialogResult dialogo = XtraMessageBox.Show("¿Seguro Desea eliminar el cargo " + costo + " ?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogo == DialogResult.Yes)
            {
                //LLAMAR AL METODO ELIMINAR
                //OBTENER EL ID DEL REGISTRO DE LA GRILLA SELECCIONADA
                int Id = 0;
                if (viewCosto.RowCount > 0)
                {
                    Id = int.Parse(viewCosto.GetFocusedDataRow()["id"].ToString());
                    bool del = fnEliminarRegistro(Id, sqlDelete);
                    //SI DEL ES TRUE ES PORQUE SE ELIMINO CORRECTAMENTE EL REGISTRO 
                    if (del)
                    {
                        //GUARDAR EVENTO EN LOG
                        logRegistro log = new logRegistro(User.getUser(), "SE ELIMINAR CENTRO COSTO", "CCOSTO", costo, "0", "ELIMINAR");
                        log.Log();

                        //CARGAMOS GRILLA
                        fnSistema.spllenaGridView(gridCosto, SqlCentroCosto);
                        fnCargarCampos(txtidCosto, txtnombreCosto, txtdato1Costo, txtdato2Costo, btnEliminarCosto, viewCosto, 0);

                        if (viewCosto.RowCount == 0)
                            fnLimpiar(txtidCosto, txtnombreCosto, txtdato1Costo, txtdato2Costo, btnEliminarCosto, "ccosto");

                        //SI VARAIBLE OPENER ES DISTINTA DE NULL ES PORQUE FORM SE INVOCO DESDE FORM EMPLEADO
                        if (opener != null)
                            opener.RecargarCombocCosto();
                    }
                }
            }
        }

        private void gridCosto_Click(object sender, EventArgs e)
        {
            //SESION
            Sesion.NuevaActividad();

            fnCargarCampos(txtidCosto, txtnombreCosto, txtdato1Costo, txtdato2Costo, btnEliminarCosto, viewCosto);

            op.SetButtonProperties(btnNuevoCosto, 1);
            op.Cancela = false;

        }

        private void gridCosto_KeyUp(object sender, KeyEventArgs e)
        {
            //SESION
            Sesion.NuevaActividad();

            fnCargarCampos(txtidCosto, txtnombreCosto, txtdato1Costo, txtdato2Costo, btnEliminarCosto, viewCosto);

            op.Cancela = false;
            op.SetButtonProperties(btnNuevoCosto, 1);

        }

        private void txtidCosto_KeyPress(object sender, KeyPressEventArgs e)
        {
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
        #endregion

        #region "CONTROLES PAGO"
        
        private void gridPago_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            fnCargarCamposFormaPago(txtidPago, txtnombrePago, cbxTipoFormaPago, txtdato2Pago, btnEliminarPago, viewPago);

            op.Cancela = false;
            op.SetButtonProperties(btnNuevoPago, 1);
            //CARGAR EL COMBOBOX DE FORMA PAGO

            //cbxTipoFormaPago.EditValue = viewPago.GetFocusedDataRow()["dato01"].ToString();
        }

        private void gridPago_KeyUp(object sender, KeyEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            fnCargarCamposFormaPago(txtidPago, txtnombrePago, cbxTipoFormaPago, txtdato2Pago, btnEliminarPago, viewPago);

            op.Cancela = false;
            op.SetButtonProperties(btnNuevoCargo, 1);

        }

        private void btnNuevoPago_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            fnLimpiarFormaPago(txtidPago, txtnombrePago, cbxTipoFormaPago, txtdato2Pago, btnEliminarPago, "formaPago");
            if (op.Cancela == false)
            {
                op.SetButtonProperties(btnNuevoPago, 2);
                op.Cancela = true;
                txtnombreCargo.Properties.Appearance.BackColor = Color.LightGoldenrodYellow;
            }
            else
            {
                op.SetButtonProperties(btnNuevoPago, 1);
                op.Cancela = false;
                fnCargarCamposFormaPago(txtidPago, txtnombrePago, cbxTipoFormaPago, txtdato2Pago, btnEliminarPago, viewPago);
                txtnombrePago.Properties.Appearance.BackColor = Color.White;
            }
        }

        private void btnGuardarPago_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();
            
            //SQL
            string sqlUpdate = "UPDATE formaPago SET Nombre=@pNombre, dato01=@pDato1, dato02=@pDato2 WHERE id=@pId";
            //SQL
            string sqlInsert = "INSERT INTO formaPago(id, nombre, dato01, dato02) VALUES(@pId, @pNombre, @pDato1, @pDato2)";
            //SQL BUSQUEDA
            //sql 
            string sqlBusqueda = "SELECT * FROM formaPago WHERE id=@pId";


            if (txtidPago.Text == "") { XtraMessageBox.Show("Debes ingresar el id", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtidPago.Focus(); return; }
            if (txtnombrePago.Text == "") { XtraMessageBox.Show("Debes llenar el campo Nombre", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtnombrePago.Focus(); return; }
            if (cbxTipoFormaPago.ItemIndex < 0) { XtraMessageBox.Show("Debes llenar el campo Tipo", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); cbxTipoFormaPago.Focus(); return; }
            int id = 0;
            string nombreAntiguo = "", cadLimpia = "";
            cadLimpia = txtnombrePago.Text;
            if (txtidPago.ReadOnly)
            {
                //HACEMOS UPDATE
                if (viewPago.RowCount > 0)
                {
                    id = int.Parse(viewPago.GetFocusedDataRow()["id"].ToString());
                    nombreAntiguo = viewPago.GetFocusedDataRow()["nombre"].ToString();
                    nombreAntiguo = QuitaEspacios(nombreAntiguo);
                    if (nombreAntiguo != cadLimpia)
                    {
                        if (ExisteNombre(cadLimpia, "formapago"))
                        { XtraMessageBox.Show("Nombre ingresado ya existe", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);txtnombrePago.Focus();return;}
                    }
                }

                bool mod = fnModificarRegistroFormaPago(id, txtnombrePago, cbxTipoFormaPago, txtdato2Pago, sqlUpdate, "FORMAPAGO");
                //SI MOD ES TRUE CARGAMOS GRILLA
                if (mod)
                {
                    //GUARDAR EVENTO EN LOG
                    //logRegistro log = new logRegistro(User.getUser(), "SE HA MODIFICADO REGISTRO CON ID " + id, fnSistema.pgDatabase, "FORMAPAGO");
                    //log.Log();

                    fnSistema.spllenaGridView(gridPago, SqlFormaPago);
                    fnCargarCamposFormaPago(txtidPago, txtnombrePago, cbxTipoFormaPago, txtdato2Pago, btnEliminarPago, viewPago, 0);
                  
                    //SI OPENER ES DISTINTA DE NULL ES PORQUE EL FORM SE INVOCO DESDE EL FORM EMPLEADO
                    if (opener != null)
                        opener.RecargarComboPago();
                }
            }
            else
            {
                //HACEMOS INSERT
                bool existe = fnVerId(int.Parse(txtidPago.Text), sqlBusqueda);
                if (existe) { XtraMessageBox.Show("Id ingresado ya existe", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

                //SABER SI EXISTE NOMBRE EN BD
                if (ExisteNombre(cadLimpia, "formapago"))
                { XtraMessageBox.Show("Nombre ingresado ya existe", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); txtnombrePago.Focus(); return; }

                bool nuevo = fnNuevoRegistroFormaPago(txtidPago, txtnombrePago, cbxTipoFormaPago, txtdato2Pago, sqlInsert);
                //SI NUEVO ES TRUE ES INSERT CORRECTO CARGAMOS GRILLA
                if (nuevo)
                {
                    //GUARDAR EVENTO EN LOG
                    logRegistro log = new logRegistro(User.getUser(), "SE INGRESA NUEVA FORMA DE PAGO", "FORMAPAGO", "0", txtnombrePago.Text, "INGRESAR");
                    log.Log();

                    fnSistema.spllenaGridView(gridPago, SqlFormaPago);
                    fnCargarCamposFormaPago(txtidPago, txtnombrePago, cbxTipoFormaPago, txtdato2Pago, btnEliminarPago, viewPago, 0);

                    op.Cancela = false;
                    op.SetButtonProperties(btnNuevoPago, 1);                  
                   
                    //SI OPENER ES DISTINTA DE NULL ES PORQUE EL FORM SE INVOCO DESDE EL FORM EMPLEADO
                    if (opener != null)
                        opener.RecargarComboPago();
                }
            }
        }

        private void btnEliminarPago_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD 
            Sesion.NuevaActividad();
            
            //SQL DELETE
            string sqlDelete = "DELETE FROM formaPago WHERE id=@pId";
            //SQL BUSQUEDA            
            string sqlBusqueda = "SELECT * FROM formaPago WHERE id=@pId";

            string pago = "";
            if (txtidPago.Text == "")
            {
                XtraMessageBox.Show("Id no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtidPago.Focus();
                return;
            }
            //VALIDAR ID EN BD
            bool existe = fnVerId(int.Parse(txtidPago.Text), sqlBusqueda);
            //SI EXISTE ES FALSE NO ES VALIDO PARA ELIMINAR
            if (existe == false) { XtraMessageBox.Show("Registro no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtidPago.Focus(); return; }

            //VERIFICAR SI EL REGISTRO ESTA SIENDO UTILIZADO POR UN TRABAAJDOR
            bool usado = false;
            usado = fnRegistroEnUso(txtidPago.Text, "SELECT formapago FROM trabajador WHERE formapago=@pId");
            if (usado)
            {
                //ESTA EN USO, NO SE PUEDE ELIMINAR
                XtraMessageBox.Show("Este registro no se puede eliminar porque esta siendo usado por un empleado", "Denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (viewPago.RowCount > 0) { pago = viewPago.GetFocusedDataRow()["nombre"].ToString(); }
            DialogResult dialogo = XtraMessageBox.Show("¿Seguro Desea eliminar el medio de pago " + pago + " ?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogo == DialogResult.Yes)
            {
                //LLAMAR AL METODO ELIMINAR
                //OBTENER EL ID DEL REGISTRO DE LA GRILLA SELECCIONADA
                int Id = 0;
                if (viewPago.RowCount > 0)
                {
                    Id = int.Parse(viewPago.GetFocusedDataRow()["id"].ToString());
                    bool del = fnEliminarRegistro(Id, sqlDelete);
                    //SI DEL ES TRUE ES PORQUE SE ELIMINO CORRECTAMENTE EL REGISTRO 
                    if (del)
                    {
                        //GUARDAR EVENTO EN LOG
                        logRegistro log = new logRegistro(User.getUser(), "SE ELIMINA FORMA DE PAGO", "FORMAPAGO", pago, "0", "ELIMINAR");
                        log.Log();

                        //CARGAMOS GRILLA
                        fnSistema.spllenaGridView(gridPago, SqlFormaPago);
                        fnCargarCamposFormaPago(txtidPago, txtnombrePago, cbxTipoFormaPago, txtdato2Pago, btnEliminarPago, viewPago, 0);

                        if (viewPago.RowCount == 0)
                            fnLimpiarFormaPago(txtidPago, txtnombrePago, cbxTipoFormaPago, txtdato2Pago, btnEliminarPago, "formapago");

                        //SI OPENER ES DISTINTA DE NULL ES PORQUE EL FORM SE INVOCO DESDE EL FORM EMPLEADO
                        if (opener != null)
                            opener.RecargarComboPago();
                    }
                }
            }
        }

        private void txtidPago_KeyPress(object sender, KeyPressEventArgs e)
        {
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

        private static void fnTipoFormaPago(LookUpEdit pComboBox)
        {
            string query = "SELECT DISTINCT dato01, descTipo FROM formaPago WHERE descTipo <> ''";
            SqlCommand cmd;
            SqlConnection cn;
            var datatable = new DataTable();
            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        cmd = new SqlCommand(query, cn);
                        var datareader = cmd.ExecuteReader();
                        datatable.Load(datareader);
                    }
                }
            }
            catch (SqlException ex)
            {
                //ERROR DE CONEXION
            }

            //PROPIEDADES COMBOBOX
            pComboBox.Properties.DataSource = datatable;
            pComboBox.Properties.ValueMember = "dato01";
            pComboBox.Properties.DisplayMember = "dato01";

            pComboBox.Properties.PopulateColumns();

            //pComboBox.Properties.Columns[0].Visible = false;

            pComboBox.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
            pComboBox.Properties.AutoSearchColumnIndex = 1;
            pComboBox.Properties.ShowHeader = false;

            if (pComboBox.Properties.DataSource != null)
            {
                pComboBox.ItemIndex = 0;
            }

            //EVITAR QUE SE PUEDAN ORDENAR LOS DATOS POR COLUMNA
            //pComboBox.Properties.Columns[0].AllowSort = DevExpress.Utils.DefaultBoolean.False;
            //pComboBox.Properties.Columns[1].AllowSort = DevExpress.Utils.DefaultBoolean.False;            
        }

        #endregion

        #region "CONTROLES NACION"

        private void btnNuevoNacion_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            fnLimpiar(txtidNacion, txtnombreNacion, txtdato1Nacion, txtdato2Nacion, btnEliminarNacion, "nacion");
            if (op.Cancela == false)
            {
                op.Cancela = true;
                op.SetButtonProperties(btnNuevoNacion, 2);
  
                txtnombreNacion.Properties.Appearance.BackColor = Color.LightGoldenrodYellow;
            }
            else
            {
                op.SetButtonProperties(btnNuevoNacion, 1);
                op.Cancela = false;

                fnCargarCampos(txtidNacion, txtnombreNacion, txtdato1Nacion, txtdato2Nacion, btnEliminarNacion, viewNacion);
                txtnombreNacion.Properties.Appearance.BackColor = Color.White;              
            }
        }

        private void btnGuardarNacion_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            //SQL
            string sqlUpdate = "UPDATE nacion SET Nombre=@pNombre, dato01=@pDato1, dato02=@pDato2 WHERE id=@pId";
            //SQL
            string sqlInsert = "INSERT INTO nacion(id, nombre, dato01, dato02) VALUES(@pId, @pNombre, @pDato1, @pDato2)";
            //SQL BUSQUEDA
            //sql 
            string sqlBusqueda = "SELECT * FROM nacion WHERE id=@pId";
      

            if (txtidNacion.Text == "") { XtraMessageBox.Show("Debes ingresar el id", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtidNacion.Focus(); return; }
            if (txtnombreNacion.Text == "") { XtraMessageBox.Show("Debes llenar el campo Nombre", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtnombreNacion.Focus(); return; }
            int id = 0;
            string nombreAntiguo = "", cadLimpia = "";
            cadLimpia = QuitaEspacios(txtnombreNacion.Text);
            if (txtidNacion.ReadOnly)
            {
                //HACEMOS UPDATE
                if (viewNacion.RowCount > 0)
                {
                    id = int.Parse(viewNacion.GetFocusedDataRow()["id"].ToString());
                    nombreAntiguo = viewNacion.GetFocusedDataRow()["nombre"].ToString();
                    nombreAntiguo = QuitaEspacios(nombreAntiguo);
                    if (nombreAntiguo != cadLimpia)
                    {
                        if (ExisteNombre(cadLimpia, "nacion"))
                        { XtraMessageBox.Show("Nombre ingresado ya existe", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);txtnombreNacion.Focus();return; }
                    }
                }

                bool mod = fnModificarRegistro(id, txtnombreNacion, txtdato1Nacion, txtdato2Nacion, sqlUpdate, "NACION");
                //SI MOD ES TRUE CARGAMOS GRILLA
                if (mod)
                {
                    //GUARDAR EVENTO EN LOG
                    //logRegistro log = new logRegistro(User.getUser(), "SE HA MODIFICADO REGISTRO CON ID " + id, fnSistema.pgDatabase, "NACION");
                    //log.Log();

                    fnSistema.spllenaGridView(gridNacion, SqlNacion);
                    fnCargarCampos(txtidNacion, txtnombreNacion, txtdato1Nacion, txtdato2Nacion, btnEliminarNacion, viewNacion, 0);                    

                    //SI VARIABLE OPENER ES DISTINTO DE NULL ES PORQUE FORM SE INVOCO DESDE FORM EMPLEADO
                    if (opener != null)
                        opener.RecargarComboNacionalidad();
                }
            }
            else
            {
                //HACEMOS INSERT
                bool existe = fnVerId(int.Parse(txtidNacion.Text), sqlBusqueda);
                if (existe) { XtraMessageBox.Show("Id ingresado ya existe", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

                //VERIFICAR SI EXISTE NOMBRE EN BD
                if (ExisteNombre(cadLimpia, "nacion"))
                { XtraMessageBox.Show("Nombre ingresado ya existe", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); txtnombreNacion.Focus(); return; }

                bool nuevo = fnNuevoRegistro(txtidNacion, txtnombreNacion, txtdato1Nacion, txtdato2Nacion, sqlInsert);
                //SI NUEVO ES TRUE ES INSERT CORRECTO CARGAMOS GRILLA
                if (nuevo)
                {
                    //GUARDAR EVENTO EN LOG
                   logRegistro log = new logRegistro(User.getUser(), "SE INGRESA NUEVA NACIONALIDAD", "NACION", "0", txtnombreNacion.Text, "INGRESAR");
                   log.Log();

                    fnSistema.spllenaGridView(gridNacion, SqlNacion);
                    fnCargarCampos(txtidNacion, txtnombreNacion, txtdato1Nacion, txtdato2Nacion, btnEliminarNacion, viewNacion, 0);

                    op.Cancela = false;
                    op.SetButtonProperties(btnNuevoNacion, 1);

                    //SI VARIABLE OPENER ES DISTINTO DE NULL ES PORQUE FORM SE INVOCO DESDE FORM EMPLEADO
                    if (opener != null)
                        opener.RecargarComboNacionalidad();
                }
            }
        }

        private void btnEliminarNacion_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();
            
            //SQL DELETE
            string sqlDelete = "DELETE FROM nacion WHERE id=@pId";
            //SQL BUSQUEDA            
            string sqlBusqueda = "SELECT * FROM nacion WHERE id=@pId";

            string nacion = "";
            if (txtidNacion.Text == "")
            {
                XtraMessageBox.Show("Id no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtidNacion.Focus();
                return;
            }
            //VALIDAR ID EN BD
            bool existe = fnVerId(int.Parse(txtidNacion.Text), sqlBusqueda);
            //SI EXISTE ES FALSE NO ES VALIDO PARA ELIMINAR
            if (existe == false) { XtraMessageBox.Show("Registro no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtidNacion.Focus(); return; }

            //VERIFICAR SI EL REGISTRO ESTA SIENDO USADO 
            bool usado = false;
            usado = fnRegistroEnUso(txtidNacion.Text, "SELECT nacion FROM trabajador WHERE nacion=@pId");
            if (usado)
            {
                XtraMessageBox.Show("Este registro esta siendo usado por un empleado por lo que no se puede eliminar", "Denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (viewNacion.RowCount > 0) { nacion = viewNacion.GetFocusedDataRow()["nombre"].ToString(); }
            DialogResult dialogo = XtraMessageBox.Show("¿Seguro Desea eliminar nacion " + nacion + " ?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogo == DialogResult.Yes)
            {
                //LLAMAR AL METODO ELIMINAR
                //OBTENER EL ID DEL REGISTRO DE LA GRILLA SELECCIONADA
                int Id = 0;
                if (viewNacion.RowCount > 0)
                {
                    Id = int.Parse(viewNacion.GetFocusedDataRow()["id"].ToString());
                    bool del = fnEliminarRegistro(Id, sqlDelete);
                    //SI DEL ES TRUE ES PORQUE SE ELIMINO CORRECTAMENTE EL REGISTRO 
                    if (del)
                    {
                        //GUARDAR EVENTO EN LOG
                           logRegistro log = new logRegistro(User.getUser(), "SE ELIMINA NACIONALIDAD", "NACION", txtnombreNacion.Text, "0", "ELIMINAR");
                           log.Log();

                        //CARGAMOS GRILLA
                        fnSistema.spllenaGridView(gridNacion, SqlNacion);
                        fnCargarCampos(txtidNacion, txtnombreNacion, txtdato1Nacion, txtdato2Nacion, btnEliminarNacion, viewNacion, 0);

                        if (viewNacion.RowCount == 0)
                            fnLimpiar(txtidNacion, txtnombreNacion, txtdato1Nacion, txtdato2Nacion, btnEliminarNacion, "nacion");

                        //SI VARIABLE OPENER ES DISTINTO DE NULL ES PORQUE FORM SE INVOCO DESDE FORM EMPLEADO
                        if (opener != null)
                            opener.RecargarComboNacionalidad();
                    }
                }
            }
        }

        private void txtidNacion_KeyPress(object sender, KeyPressEventArgs e)
        {
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

        private void gridNacion_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            fnCargarCampos(txtidNacion, txtnombreNacion, txtdato1Nacion, txtdato2Nacion, btnEliminarNacion, viewNacion);

            op.Cancela = false;
            op.SetButtonProperties(btnNuevoNacion, 1);

        }

        private void gridNacion_KeyUp(object sender, KeyEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            fnCargarCampos(txtidNacion, txtnombreNacion, txtdato1Nacion, txtdato2Nacion, btnEliminarNacion, viewNacion);

            op.Cancela = false;
            op.SetButtonProperties(btnNuevoNacion, 1);

        }

        #endregion

        #region "MANEJO INCREMENTO ID"

        //OBTENER LA CANTIDAD DE ITEM DISPONIBLES DENTRO DE UN RANGO DE NUMEROS
        //ej: entre 3 y 7 hay 3 elementos disponibles {4,5,6}
        private int fnItemDisponibles(string pTableName)
        {
            int[] elementos = fnAllitem(pTableName);
            int num1 = 0, num2 = 0;
            int generado = 0;
            int devuelve = 0;

            if (elementos.Length > 1)
            {
                //ORDENAR DE FORMA ASCENDENTE LOS NUMEROS DE ITEMS
                Array.Sort(elementos);
                for (int pos = 0; pos < elementos.Length - 1; pos++)
                {
                    num1 = elementos[pos];
                    num2 = elementos[pos + 1];
                    if ((num2 - num1) > 1)
                    {
                        //HAY ELEMENTOS QUE NO ESTAN EN USO...
                        generado = num1 + 1;
                    }
                }

                //SI GENERADO SIGUE SIENDO 0 A PESAR DE RECORRER TODO EL ARREGLO
                //ES PORQUE NO HAY DIGITOS ENTRE MEDIO DISPONIBLES
                if (generado == 0)
                {
                    //RETORNAMOS EL ULTIMO ELEMENTO INGRESADO EN BD + 1                    
                    devuelve = fnUltimoNumero(pTableName) + 1;

                }
                else
                {
                    //SI ES DISTINTO DE CERO RETORNAMOS EL NUMERO GENERADO
                    devuelve = generado;
                }

            }
            else if (elementos.Length == 1)
            {
                //SI SOLO TIENE UN ELEMENTO...
                num1 = elementos[0];
                if (num1 == 1)
                {
                    devuelve = num1 + 1;
                }
                else if (num1 > 1 && num1 != 0)
                {
                    devuelve = num1 - 1;
                }
            }
            else
            {
                //SI NO TIENE ELEMENTOS RETORNA UN 1
                devuelve = 1;
            }


            return devuelve;
        }

        //TRAER TODOS LOS NUMEROS DE ITEM DESDE LA BASE DE DATOS
        private int[] fnAllitem(string pTablaName)
        {
            int total = 0;
            total = fnCantidad(pTablaName);
            int[] numeros = new int[total];

            int posicion = 0;

            //sql consulta
            string sql = string.Format("SELECT id FROM {0}", pTablaName);
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
                                //GUARDAR DATOS EN ARREGLO...
                                numeros[posicion] = (int)rd["id"];
                                posicion++;
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
            return numeros;

        }

        //OBTENER LA CANTIDAD DE ELEMENTOS DE LA TABLA ITEM TRABAJADOR PARA UN TRABAJADOR EN PARTICULAR
        private int fnCantidad(string pTablaName)
        {
            string sql = string.Format("SELECT count(*) as cantidad FROM {0}", pTablaName);
            SqlCommand cmd;
            SqlDataReader rd;

            int total = 0;
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
                                total = (int)rd["cantidad"];
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
            return total;

        }

        //CONSULTAR POR EL ULTIMO ITEM INGRESADO
        private int fnUltimoNumero(string pTablaName)
        {
            string sql = string.Format("SELECT max(id) as maximo FROM {0}", pTablaName);
            SqlCommand cmd;
            SqlDataReader rd;
            int num = 0;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //RECORREMOS SI ENCONTRO REGISTROS
                            while (rd.Read())
                            {
                                num = (int)rd["maximo"];
                            }
                        }
                        else
                        {
                            //SI NO ENCONTRO REGISTROS RETORNAMOS -1 QUE SIGNIFICA QUE NO HAY NUMEROS INGRESADOS
                            num = -1;
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

            return num;
        }

        #endregion

        #region "CAJA COMPENSACION"
        //INGRESAR NUEVA CAJA DE COMPENSACION
        private void NuevaCajaCompensacion(TextEdit pNombre, TextEdit pPrevired)
        {
            string sql = "INSERT INTO cajacompensacion(codprevired, desccaja) VALUES(@pPrevired, @pDesc)";
            SqlCommand cmd;
            int count = 0;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pPrevired", pPrevired.Text));
                        cmd.Parameters.Add(new SqlParameter("@pDesc", pNombre.Text));

                        count = cmd.ExecuteNonQuery();
                        if (count > 0)
                        {
                            XtraMessageBox.Show("Ingreso correcto", "informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            logRegistro l = new logRegistro(User.getUser(), "SE INGRESA NUEVA CAJA DE COMPENSACION ", "CAJACOMPENSACION", "0", pNombre.Text, "INGRESAR");
                            l.Log();

                            //CARGAMOS GRILLA
                            CargarGrillaCaja();
                            CargarDataFromGrid();

                            lblmsgCaja.Visible = false;
                        }
                        else
                        {
                            XtraMessageBox.Show("No se pudo realizar ingreso", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
        }

        //MODIFICAR CAJA COMPENSACION
        private void ModificarCajaCompensacion(int pId, TextEdit pNombre, TextEdit pPrevired)
        {
            string sql = "UPDATE cajacompensacion SET desccaja=@pDesc, codprevired=@pPrevired " +
                "WHERE id=@pId";
            SqlCommand cmd;
            int count = 0;

            Hashtable DataCaja = PrecargaCajaCom(pId);

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pDesc", pNombre.Text));
                        cmd.Parameters.Add(new SqlParameter("@pPrevired", pPrevired.Text));
                        cmd.Parameters.Add(new SqlParameter("@pId", pId));

                        count = cmd.ExecuteNonQuery();
                        if (count > 0)
                        {
                            XtraMessageBox.Show("Registro actualizado correctamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            //COMPARAMOS
                            ComparaCajaCom(DataCaja, pPrevired.Text, pNombre.Text);

                            //CARGAMOS GRILLA
                            CargarGrillaCaja();
                            CargarDataFromGrid();

                            lblmsgCaja.Visible = false;
                        }
                        else
                        {
                            XtraMessageBox.Show("Registro no se pudo actualizar", "informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        }

        //VERIFICAR SI EL CODIGO DE REGISTRO ESTA EN USO
        private bool CajaenUso(int pId)
        {
            bool usada = false;
            string sql = "SELECT count(*) as cantidad FROM empresa WHERE nombreccaf=@pId";
            int count = 0;
            SqlCommand cmd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pId", pId));

                        count = Convert.ToInt32(cmd.ExecuteScalar());
                        if (count > 0)
                            usada = true;
                    }

                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return usada;
        }

        //ELIMINAR CAJA DE COMPENSACION
        private void EliminarCajaCompensacion(int pId, string caja)
        {
            bool usada = false;
            SqlCommand cmd;
            string sql = "DELETE FROM cajacompensacion WHERE id=@pId";
            int count = 0;

            usada = CajaenUso(pId);
            if (usada)
            {
                XtraMessageBox.Show("Registro no se puede eliminar porque está en uso", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            DialogResult advertencia = XtraMessageBox.Show($"¿Realmente deseas eliminar registro {caja}?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (advertencia == DialogResult.Yes)
            {
                //ELIMINAMOS
                try
                {
                    if (fnSistema.ConectarSQLServer())
                    {
                        using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@pId", pId));

                            count = cmd.ExecuteNonQuery();
                            if (count > 0)
                            {
                                XtraMessageBox.Show("Registro eliminado correctamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                logRegistro l = new logRegistro(User.getUser(), "SE ELIMINA CAJA COMPENSACION CODIGO " + pId, "CAJACOMPENSACION", pId.ToString(), "0", "ELIMINAR");
                                l.Log();

                                //CARGAMOS GRILLA
                                CargarGrillaCaja();
                                CargarDataFromGrid();

                                lblmsgCaja.Visible = false;
                            }
                            else
                            {
                                XtraMessageBox.Show("No se pudo eliminar registro", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            }
        }

        //CARGAR GRILLA
        private void CargarGrillaCaja()
        {
            string sql = "SELECT id, descCaja, codprevired FROM cajacompensacion WHERE codprevired <> 00";            
            fnSistema.spllenaGridView(gridCajacompensacion, sql);
            if (viewCajacompensacion.RowCount > 0)
            {
                fnSistema.spOpcionesGrilla(viewCajacompensacion);
                ColumnasGrillaCaja();
            }            
        }

        //COLUMNAS GRILLA   
        private void ColumnasGrillaCaja()
        {
            if (viewCajacompensacion.RowCount>0)
            {
                viewCajacompensacion.Columns[0].Caption = "N°";
                viewCajacompensacion.Columns[0].AppearanceCell.FontStyleDelta = FontStyle.Bold;
                viewCajacompensacion.Columns[0].Width = 20;

                viewCajacompensacion.Columns[1].Caption = "Descripción";
                viewCajacompensacion.Columns[1].Width = 150;

                viewCajacompensacion.Columns[2].Caption = "Dato N°1";
                viewCajacompensacion.Columns[2].Width = 30;
            }
        }

        //CARGAR CAJAS DESDE GRILLA
        private void CargarDataFromGrid(int? fila = -1)
        {
            if (viewCajacompensacion.RowCount>0)
            {
                if (fila != -1) viewCajacompensacion.FocusedRowHandle = 0;

                ModificaCaja = true;

                txtcodPrevired.Text = (string)viewCajacompensacion.GetFocusedDataRow()["codprevired"];
                txtNombrecaja.Text = (string)viewCajacompensacion.GetFocusedDataRow()["desccaja"];

                op.Cancela = false;
                op.SetButtonProperties(btnNuevoCaja, 1);

                btnEliminarcaja.Enabled = true;
            }
        }

        //LIMPIAR CAMPOS
        private void LimpiarCajaCompensacion()
        {
            txtcodPrevired.Text = "";
            txtNombrecaja.Text = "";
            txtNombrecaja.Focus();
            ModificaCaja = false;
        }

        //VALIDAR SI EXISTE UNA CAJA CON ESE NOMBRE
        private bool ExisteCaja(string pCaja)
        {
            string sql = "";            
                sql = "SELECT count(*) as cantidad FROM cajacompensacion WHERE desccaja=@pDesc ";                    
            SqlCommand cmd;
            int count = 0;
            bool existe = false;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pDesc", pCaja));
                        count = Convert.ToInt32(cmd.ExecuteScalar());

                        if (count > 0)
                            existe = true;                            
                    }
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();                    
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return existe;
        }

        //VER SI HAY CMABIOS
        private bool CambiosSinGuardar(int pId, string pPrevired, string pDesc)
        {
            string sql = "SELECT codprevired, desccaja FROM cajacompensacion WHERE id=@pId";
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pId", pId));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //COMPARAMOS 
                                if (pPrevired != (string)rd["codprevired"]) return true;
                                if (pDesc != (string)rd["desccaja"]) return true ;
                            }
                        }
                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                        rd.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return false;
        }

        //PARA LOG
        /*HASHTABLE CON DATOS */
        private Hashtable PrecargaCajaCom(int pId)
        {
            Hashtable data = new Hashtable();
            string sql = "SELECT codprevired, desccaja FROM CAJACOMPENSACION WHERE id=@pId";
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pId", pId));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //GUARDAMOS 
                                data.Add("previred", (string)rd["codprevired"]);
                                data.Add("desccaja", (string)rd["desccaja"]);
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
            return data;
        }

        //COMPARAR VALORES
        private void ComparaCajaCom(Hashtable pTabla, string pCodPrevired, string pDescCaja)
        {
            if (pTabla.Count > 0)
            {
                if ((string)pTabla["previred"] != pCodPrevired)
                {
                    logRegistro l = new logRegistro(User.getUser(), "SE MODIFICAR VALOR CAMPO PREVIRED ", "CAJACOMPENSACION", (string)pTabla["previred"], pCodPrevired, "MODIFICAR");
                    l.Log();
                }
                if ((string)pTabla["desccaja"] != pDescCaja)
                {
                    logRegistro l = new logRegistro(User.getUser(), "SE MODIFICAR VALOR CAMPO DESCCAJA ", "CAJACOMPENSACION", (string)pTabla["desccaja"], pDescCaja, "MODIFICAR");
                    l.Log();
                }
            }
        }

        #endregion

        #region "SUCURSAL"
        //INGRESAR UNA NUEVA SUCURSAL
        private void IngresarSucursal(TextEdit pCod, TextEdit pNombre, TextEdit pDato1, TextEdit pDato2)
        {
            string sql = "INSERT INTO SUCURSAL(codsucursal, descsucursal, dato01, dato02) VALUES " +
                "(@pCod, @pDesc, @pDato1, @pDato2)";

            SqlCommand cmd;
            int count = 0;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pCod", Convert.ToInt32(pCod.Text)));
                        cmd.Parameters.Add(new SqlParameter("@pDesc", pNombre.Text));
                        cmd.Parameters.Add(new SqlParameter("@pDato1", pDato1.Text));
                        cmd.Parameters.Add(new SqlParameter("@pDato2", pDato2.Text));

                        count = cmd.ExecuteNonQuery();
                        if (count > 0)
                        {
                            XtraMessageBox.Show("Sucursal ingresada correctamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            //CARGAR GRILLA
                            //CARGAR GRILLA
                            logRegistro log = new logRegistro(User.getUser(), $"SE INGRESA NUEVA SUCURSAL CON CODIGO {Convert.ToInt32(pCod.Text)}", "SUCURSAL", "0", Convert.ToInt32(pCod.Text).ToString(), "INGRESAR");
                            log.Log();

                            fnSistema.spllenaGridView(gridSucursal, SqlGridSucursal);
                            CargarSucursal(0);

                            op.Cancela = false;
                            op.SetButtonProperties(btnNuevaSucursal, 1);
            

                            if (opener != null)
                                opener.RecargarComboSucursal();
                        }
                        else
                        {
                            XtraMessageBox.Show("Error al intentar ingresar sucursal", "infomacion", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        //MODIFICAR SUCURSAL
        private void ModificaSucursal(TextEdit pCod, TextEdit pNombre, TextEdit pDato1, TextEdit pDato2, int pCodBd)
        {
            string sql = "UPDATE SUCURSAL SET codSucursal=@pCod, descSucursal=@pDesc, dato01=@pDato1, " +
                "dato02=@pDato2 WHERE codSucursal=@pCodBd";
            SqlCommand cmd;
            int count = 0;

            //OBTENER DATOS DE BD
            Hashtable data = new Hashtable();
            data = GetDataSucursal(pCodBd);

            //XtraMessageBox.Show(data["descSucursal"].ToString());

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pCod", Convert.ToInt32(pCod.Text)));
                        cmd.Parameters.Add(new SqlParameter("@pDesc", pNombre.Text));
                        cmd.Parameters.Add(new SqlParameter("@pDato1", pDato1.Text));
                        cmd.Parameters.Add(new SqlParameter("@pDato2", pDato2.Text));
                        cmd.Parameters.Add(new SqlParameter("@pCodBd", pCodBd));

                        count = cmd.ExecuteNonQuery();
                        if (count > 0)
                        {
                            XtraMessageBox.Show("Sucursal actualizada correctamente", "informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            //ACTUALIZAMOS GRILLA     
                            ComparaDatosSucur(data, Convert.ToInt32(pCod.Text), pNombre.Text, pDato1.Text, pDato2.Text);

                            //CARGAR GRILLA
                            fnSistema.spllenaGridView(gridSucursal, SqlGridSucursal);
                            CargarSucursal(0);

                            if (opener != null)
                                opener.RecargarComboSucursal();
                                
                        }
                        else
                        {
                            XtraMessageBox.Show("Error al intentar actualizar sucursal", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        //ELIMINAR SUCURSAL
        private void EliminarSucursal(int pCod)
        {
            string sql = "DELETE FROM sucursal WHERE codSucursal = @pCod";
            SqlCommand cmd;
            int count = 0;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pCod", pCod));

                        count = cmd.ExecuteNonQuery();
                        if (count > 0)
                        {
                            XtraMessageBox.Show("Sucursal eliminada correctamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            logRegistro log = new logRegistro(User.getUser(), $"SE ELIMINAR SUCURSAL CON CODIGO {pCod}", "SUCURSAL", pCod + "", "0", "ELIMINAR");
                            log.Log();

                            //CARGAR GRILLA
                            fnSistema.spllenaGridView(gridSucursal, SqlGridSucursal);                            
                            CargarSucursal(0);


                            if (opener != null)
                                opener.RecargarComboSucursal();
                        }
                        else
                        {
                            XtraMessageBox.Show("Error al intentar eliminar sucursal", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
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

        //VERIFICAR SI EXISTE CODIGO ANTES DE REALIZAR OPERACION DE ELIMINACION
        private bool ExisteSucursal(int pCod)
        {
            bool existe = false;
            string sql = "SELECT count(*) FROM sucursal WHERE codSucursal = @pCod ";
            SqlCommand cmd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pCod", pCod));

                        if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                            existe = true;

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return existe;
        }

        //COLUMNAS GRILLA
        private void ColumnasGirdSucursal()
        {
            if (viewSucursal.RowCount > 0)
            {
                viewSucursal.Columns[0].Caption = "N°";
                viewSucursal.Columns[0].AppearanceCell.FontStyleDelta = FontStyle.Bold;
                viewSucursal.Columns[0].Width = 30;

                viewSucursal.Columns[1].Caption = "Descripcion";
                viewSucursal.Columns[1].Width = 200;
                //DATO01
                viewSucursal.Columns[2].Width = 30;
                viewSucursal.Columns[2].Caption = "Dato N°1";
                //DATO02
                viewSucursal.Columns[3].Width = 30;
                viewSucursal.Columns[3].Caption = "Dato N°2";
            }
        }

        //LIMPIAR CAMPOS SUCURSAL
        private void LimpiarSucursal()
        {
            txtCodigoSucursal.ReadOnly = false;
            txtCodigoSucursal.Text = "";
            txtCodigoSucursal.Focus();
            txtNombreSucursal.Text = "";
            updateSucursal = false;
            lblSucursal.Visible = false;
            txtDato1Sucursal.Text = "";
            txtDato1Sucursal.Text = "";
        }

        //CARGAR DATOS DESDE GRILLA
        private void CargarSucursal(int? pos = -1)
        {
            if (viewSucursal.RowCount > 0)
            {
                if (pos != -1) viewSucursal.FocusedRowHandle = (int)pos;

                txtCodigoSucursal.Text = viewSucursal.GetFocusedDataRow()["codsucursal"].ToString();
                txtNombreSucursal.Text = viewSucursal.GetFocusedDataRow()["descSucursal"].ToString();
                txtDato1Sucursal.Text = viewSucursal.GetFocusedDataRow()["dato01"].ToString();
                txtDato2Sucursal.Text = viewSucursal.GetFocusedDataRow()["dato02"].ToString();

                btnEliminarSucursal.Enabled = true;
                updateSucursal = true;
                txtCodigoSucursal.ReadOnly = true;

                op.Cancela = false;
                op.SetButtonProperties(btnNuevaSucursal, 1);

            }
            else
            {
                LimpiarSucursal();
            }
        }

        //COMPARA VALORES SUCURSAL
        private bool SucursalSinGuardar(string pCod, string pNombre, string pDato01, string pDato02, int pCodbd)
        {
            string sql = "SELECT codSucursal, descSucursal, dato01, dato02 FROM SUCURSAL " +
                "WHERE codSucursal = @pCod";
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pCod", pCodbd));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                if (rd["codSucursal"].ToString() != pCod) return true;
                                if (pNombre != rd["descSucursal"].ToString()) return true;
                                if (pDato01 != rd["dato01"].ToString()) return true;
                                if (pDato02 != rd["dato02"].ToString()) return true;
                            }
                        }

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                        rd.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return false;
        }

        //SUCURSAL EN USO
        private bool SucursalEnUso(int pCod)
        {
            bool EnUso = false;
            string sql = "SELECT count(*) FROM trabajador WHERE sucursal=@pCod";
            SqlCommand cmd;
            int count = 0;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pCod", pCod));

                        count = Convert.ToInt32(cmd.ExecuteScalar());
                        if (count > 0)
                            EnUso = true;

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();                       
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return EnUso;
        }

        #region "LOG SUCURSAL"
        //OBTENER DATOS DESDE BD
        private Hashtable GetDataSucursal(int pCod)
        {
            Hashtable data = new Hashtable();
            string sql = "SELECT codSucursal, descSucursal, dato01, dato02 FROM sucursal " +
                "WHERE codSucursal=@pCod";
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pCod", pCod));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //CARGAMOS hashtable
                                data.Add("codSucursal", Convert.ToInt32(rd["codSucursal"]));
                                data.Add("descSucursal", (string)rd["descSucursal"]);
                                data.Add("dato01", (string)rd["dato01"]);
                                data.Add("dato02", (string)rd["dato02"]);
                            }
                        }
                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                        rd.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return data;
        }

        //COMPARAR E INGRESAR EN LOG
        private void ComparaDatosSucur(Hashtable Data, int pCod, string pNombre, string pDato01, string pDato02)
        {
            if (Data.Count > 0)
            {
                if (Convert.ToInt32(Data["codSucursal"]) != pCod)
                {
                    logRegistro log = new logRegistro(User.getUser(), $"SE MODIFICA CAMPO CODIGO SUCURSAL '{Data["descSucursal"]}'", "SUCURSAL", Data["codSucursal"].ToString(), pCod + "", "MODIFICAR");
                    log.Log();
                }
                if (Data["descSucursal"].ToString() != pNombre)
                {
                    logRegistro log = new logRegistro(User.getUser(), $"SE MODIFICA CAMPO DESCRIPCION SUCURSAL '{Data["descSucursal"]}'", "SUCURSAL", Data["descSucursal"].ToString(), pNombre, "MODIFICAR");
                    log.Log();
                }
                if (Data["dato01"].ToString() != pDato01)
                {
                    logRegistro log = new logRegistro(User.getUser(), $"SE MODIFICA CAMPO DATO01 SUCURSAL '{Data["descSucursal"]}'", "SUCURSAL", Data["dato01"].ToString(), pDato01, "MODIFICA");
                    log.Log();
                }
                if (Data["dato02"].ToString() != pDato02)
                {
                    logRegistro log = new logRegistro(User.getUser(), $"SE MODIFICA CAMPO DATO02 SUCURSAL '{Data["descSucursal"]}'", "SUCURSAL", Data["dato02"].ToString(), pDato02, "MODIFICA");
                    log.Log();
                }
            }
        }

        #endregion
        #endregion

        #region "CAUSAL TERMINO"
        //NUEVO REGISTRO
        private void fnNuevaCausal(TextEdit pCod, TextEdit pDesc, TextEdit pJustificacion, CheckEdit pAviso, CheckEdit pServicio)
        {
            string sql = "INSERT INTO causaltermino(codCausal, descCausal, justificacion, indAviso, indServicio) values(@pcod, @pDesc, @pJustificacion, @pAviso, @pServicio)";
            SqlCommand cmd;
            int res = 0;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pcod", Convert.ToInt32(pCod.Text)));
                        cmd.Parameters.Add(new SqlParameter("@pDesc", pDesc.Text));
                        cmd.Parameters.Add(new SqlParameter("@pJustificacion", pJustificacion.Text));
                        cmd.Parameters.Add(new SqlParameter("@pAviso", pAviso.Checked == false ? 0:1));
                        cmd.Parameters.Add(new SqlParameter("@pServicio", pServicio.Checked == false ? 0 : 1));

                        //Ejecutar consulta
                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            XtraMessageBox.Show("Registro guardado", "Guardar", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            //REGISTRAMOS EVENTO EN LOG
                            logRegistro log = new logRegistro(User.getUser(), "INGRESA NUEVA CAUSAL TERMINO", "CAUSALTERMINO", "0", pDesc.Text, "INGRESAR");
                            log.Log();                            
                            
                            fnSistema.spllenaGridView(gridCausal, SqlCusalTermino);
                            fnCargarCamposCausal(0);

                            if (opener != null)
                            {
                                //cargamos combobox
                                opener.RecargarComboCausal();
                            }
                        }
                        else
                        {
                            XtraMessageBox.Show("Error la intentar guardar", "Guardar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    //LIBERAR RECURSOS
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                }
                else
                {
                    XtraMessageBox.Show("No hay conexion con la base de datos", "Base de datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

        }

        //MODIFICAR REGISTRO
        private void fnModificarCausal(TextEdit pDesc, int pCodBd, TextEdit pJustificacion, CheckEdit pAviso, CheckEdit pServicio)
        {
            string sql = "UPDATE causaltermino SET descCausal=@pDesc, justificacion=@pJustificacion, indAviso=@pAviso, indServicio=@pServicio WHERE codCausal=@pCod";
            SqlCommand cmd;
            int res = 0;
            //HASH PRECARGA DATOS
            Hashtable datosCausal = PrecargaCausal(pCodBd);

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS 
                        cmd.Parameters.Add(new SqlParameter("@pCod", pCodBd));
                        cmd.Parameters.Add(new SqlParameter("@pDesc", pDesc.Text));
                        cmd.Parameters.Add(new SqlParameter("@pJustificacion", pJustificacion.Text));
                        cmd.Parameters.Add(new SqlParameter("@pAviso", pAviso.Checked == false ? 0: 1));
                        cmd.Parameters.Add(new SqlParameter("@pServicio", pServicio.Checked == false ? 0 : 1));

                        //EJECUTAR CONSULTA
                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            XtraMessageBox.Show("Registro Actualizado", "Actualizar", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            //SI HAY CAMBIOS GUARDA VALORES EN LOG
                            ComparaValorCausal(pCodBd, pDesc.Text, pJustificacion.Text, pAviso.Checked == false ? 0 : 1, pServicio.Checked == false ? 0 : 1, datosCausal);                            
                            
                            fnSistema.spllenaGridView(gridCausal, SqlCusalTermino);

                            fnCargarCamposCausal(0);

                            if (opener != null)
                            {
                                //cargamos combobox
                                opener.RecargarComboCausal();
                            }
                        }
                        else
                        {
                            XtraMessageBox.Show("Error al intentar actualizar", "Actualizar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    //LIBERAR RECURSOS
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();

                }
                else
                {
                    XtraMessageBox.Show("No hay conexion con la base de datos", "Base de datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        //ELIMINAR REGISTRO
        private void fnEliminarCausal(int pCod)
        {
            string sql = "DELETE FROM causaltermino WHERE codCausal=@pCod";
            SqlCommand cmd;
            int res = 0;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROA
                        cmd.Parameters.Add(new SqlParameter("@pCod", pCod));

                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            XtraMessageBox.Show("Registro eliminado", "Eliminar", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            //REGISTRAR EVENTO EN LOG
                            logRegistro log = new logRegistro(User.getUser(), "SE ELIMINA CAUSAL DE TERMINO ", "CAUSALTERMINO", pCod.ToString(), "0", "ELIMINAR");
                            log.Log();
                            
                            fnSistema.spllenaGridView(gridCausal, SqlCusalTermino);

                            fnCargarCamposCausal(0);

                            if (opener != null)
                            {
                                //cargamos combobox
                                opener.RecargarComboCausal();
                            }
                        }
                        else
                        {
                            XtraMessageBox.Show("Error al intentar eliminar", "Eliminar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    //LIBERAR RECURSOS
                    cmd.Dispose();
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
        }

        //CARGAR CAMPOS DESDE GRILLA
        private void fnCargarCamposCausal(int? pos = -1)
        {
            if (viewCausal.RowCount > 0)
            {
                if (pos != -1) viewCausal.FocusedRowHandle = (int)pos;

                //UPDATE EN TRUE PARA ACTUALIZACION
                UpdateCausal = true;

                txtCodCausal.ReadOnly = true;

                //CARGAMOS CAMPOS
                txtCodCausal.Text = viewCausal.GetFocusedDataRow()["codCausal"].ToString();
                txtDescCausal.Text = viewCausal.GetFocusedDataRow()["descCausal"].ToString();
                txtJustificacion.Text = viewCausal.GetFocusedDataRow()["justificacion"].ToString();
                cbAnioServ.Checked = Convert.ToInt16(viewCausal.GetFocusedDataRow()["indServicio"]) == 0 ? false : true;
                cbAviso.Checked = Convert.ToInt16(viewCausal.GetFocusedDataRow()["indAviso"]) == 0 ? false: true;

                //RESET BOTON NUEVO
                op.Cancela = false;
                op.SetButtonProperties(btnNuevoCausal, 1);
                txtCodCausal.Properties.Appearance.BackColor = Color.White;
                lblmsgCausal.Visible = false;
            }
            else
            {
                fnLimpiarCamposCausal();
            }
        }

        //LIMPIAR CAMPOS
        private void fnLimpiarCamposCausal()
        {
            txtCodCausal.Text = "";
            txtCodCausal.Focus();
            txtDescCausal.Text = "";
            txtJustificacion.Text = "";
            cbAnioServ.Checked = false;
            cbAviso.Checked = false;
            UpdateCausal = false;
            lblmsgCausal.Visible = false;
            txtCodCausal.ReadOnly = false;
            op.SetButtonProperties(btnNuevoCausal, 2);
            op.Cancela = true;
            txtCodCausal.Properties.Appearance.BackColor = Color.LightGoldenrodYellow;

        }

        //VERIFICAR SI EXISTE CAUSAL
        private bool ExisteCausal(int pCod)
        {
            bool existe = false;
            string sql = "SELECT count(*) FROM causaltermino WHERE codCausal=@pCod";
            SqlCommand cmd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pCod", pCod));

                        if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                            existe = true;

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return existe;
        }

        //VERIFICAR SI LA CAUSAL ESTA SIENDO USADA POR UN TRABAJADOR
        private bool fnCausalTrabajador(int pCod)
        {
            string sql = "SELECT contrato, causal FROM trabajador " +
                "INNER JOIN causaltermino ON causaltermino.codcausal=trabajador.causal " +
                "WHERE trabajador.causal=@pCod";

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
                        cmd.Parameters.Add(new SqlParameter("@pCod", pCod));

                        //EJECUTAR CONSULTA
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //EXISTE RELACION
                            existe = true;
                        }
                        else
                        {
                            existe = false;
                        }
                    }
                }
                else
                {
                    XtraMessageBox.Show("No hay conexion con la base de datos", "Base de datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return existe;
        }

        //COLUMNAS GRILLA
        private void fnColumnasCausal()
        {
            viewCausal.Columns[0].Caption = "Codigo";
            viewCausal.Columns[0].Width = 10;
            viewCausal.Columns[0].AppearanceCell.FontStyleDelta = FontStyle.Bold;

            viewCausal.Columns[1].Caption = "Descripcion";
            viewCausal.Columns[1].Width = 200;

            viewCausal.Columns[2].Visible = false;
            viewCausal.Columns[3].Visible = false;
            viewCausal.Columns[4].Visible = false;
        }

        //CAMBIOS SIN GUARDAR
        private bool CambiosSinGuardarCausal(int pId, string pDesc, string pJustificacion, int pAviso, int pServicio)
        {
            string sql = "SELECT descCausal, justificacion, indAviso, indServicio FROM causaltermino WHERE codCausal=@pCausal";
            SqlCommand cmd;
            SqlConnection cn;
            SqlDataReader rd;
            bool distinto = false;
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
                            cmd.Parameters.Add(new SqlParameter("@pCausal", pId));

                            rd = cmd.ExecuteReader();
                            if (rd.HasRows)
                            {
                                while (rd.Read())
                                {
                                    //COMPARAMOS
                                    if (pDesc.ToLower() != ((string)rd["descCausal"]).ToLower()) { distinto = true; break; };
                                    if (pJustificacion.ToLower() != ((string)rd["justificacion"]).ToLower()) { distinto = true; break; }
                                    if (pAviso != Convert.ToInt16(rd["indAviso"])) { distinto = true; break; }
                                    if (pServicio != Convert.ToInt16(rd["indServicio"])) { distinto = true; break; }
                                }
                            }
                        }
                        rd.Close();
                        cmd.Dispose();
                    }
                }
            }
            catch (SqlException ex)
            {
               //ERROR SQL
            }

            return distinto;
        }

        #region "LOG CAUSAL TERMINO"
        //TABLA HASH PARA RECARGA
        private Hashtable PrecargaCausal(int codigo)
        {
            Hashtable datos = new Hashtable();
            string sql = "SELECT codcausal, desccausal, justificacion, indAviso, indServicio FROM CAUSALTERMINO WHERE codcausal=@cod";
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@cod", codigo));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //LLENAMOS TABLA HASH
                                datos.Add("codcausal", (int)rd["codcausal"]);
                                datos.Add("desccausal", (string)rd["desccausal"]);
                                datos.Add("justificacion", (string)rd["justificacion"]);
                                datos.Add("aviso", Convert.ToInt16(rd["indAviso"]));
                                datos.Add("servicio", Convert.ToInt16(rd["indServicio"]));
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

        //COMPARA VALORES
        private void ComparaValorCausal(int codigo, string descripcion, string pJustificacion, int Aviso, int Servicio, Hashtable Datos)
        {
            if (Datos.Count > 0)
            {
                //COMPARA SI SE A CAMBIADO ALGUN VALOR!
                //GUARDAMOS EN LOG DE SER ASI!!!!!
                if ((string)Datos["descripcion"] != descripcion)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA DESCRIPCION CAUSAL " + codigo, "CAUSALTERMINO", (string)Datos["descripcion"] + "", descripcion, "MODIFICA");
                    log.Log();
                }
                if ((string)Datos["justificacion"] != pJustificacion)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA JUSTIFICACION CAUSAL " + codigo, "CAUSALTERMINO", (string)Datos["justificacion"] + "", pJustificacion, "MODIFICA");
                    log.Log();
                }
                if (Convert.ToInt16(Datos["aviso"]) != Aviso)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA INDAVISO CAUSAL " + codigo, "CAUSALTERMINO", Convert.ToInt16(Datos["aviso"]) + "", Aviso.ToString(), "MODIFICA");
                    log.Log();
                }
                if (Convert.ToInt16(Datos["servicio"]) != Servicio)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA INDSERVICIO CAUSAL " + codigo, "CAUSALTERMINO", Convert.ToInt16(Datos["servicio"]) + "", Servicio.ToString(), "MODIFICA");
                    log.Log();
                }

            }
        }

        #endregion

        #endregion

        #region "ESCOLARIDAD"
        /// <summary>
        /// Permite dejar en blanco las cajas de texto para un nuevo registro.
        /// </summary>
        /// <param name="pCod">Caja de texto correspondiente al codigo.</param>
        /// <param name="pDesc">Caja de texto de descripcion</param>
        /// <param name="pMessage">Label para mostrar mensajes.</param>
        public void LimpiarCajasEsco()
        {
            lblMsgEsco.Text = "";
            lblMsgEsco.Visible = false;
            txtCodEsco.Focus();
            txtCodEsco.Text = "";
            txtCodEsco.ReadOnly = false;
            txtDescEsco.Text = "";
            btnEliminarEsco.Enabled = false;
            UpdateEscolaridad = false;
        }

        /// <summary>
        /// Carga en las cajas de texto correspondiente la informacion de la fila seleccionada.
        /// </summary>
        /// <param name="pGrid">Grilla de datos.</param>
        /// <param name="pCod">Caja de texto para codigo.</param>
        /// <param name="pDesc">Caja de texto para descripcion.</param>
        public void CargarInfoEsco(int? pos = -1)
        {
            if (viewEsco.RowCount > 0)
            {
                if (pos != -1) viewEsco.FocusedRowHandle = Convert.ToInt32(pos);

                txtCodEsco.Text = Convert.ToInt32(viewEsco.GetFocusedDataRow()["codesc"]) + "";
                txtDescEsco.Text = viewEsco.GetFocusedDataRow()["descesc"].ToString();

                txtCodEsco.ReadOnly = true;
                btnEliminarEsco.Enabled = true;
                UpdateEscolaridad = true;
            }
            else
            {
                LimpiarCajasEsco();
            }
        }

        #endregion

        #region "HORARIO"
        private void CargaCamposHorario(int? pos = -1)
        {
            if (viewHorario.RowCount > 0)
            {
                if (pos != -1) viewHorario.FocusedRowHandle = Convert.ToInt32(pos);

                txtCodHorario.Text = Convert.ToInt32(viewHorario.GetFocusedDataRow()["id"]) + "";
                txtDescHorario.Text = (string)viewHorario.GetFocusedDataRow()["deschor"];
                txtMemoHorario.Text = (string)viewHorario.GetFocusedDataRow()["detalle"];
                //txtAdicionHorario.Text = (string)viewHorario.GetFocusedDataRow()["valor4"];

                UpdateHorario = true;
                txtCodHorario.ReadOnly = true;
                lblMsgHorario.Visible = false;
            }
        }

        private void LimpiaHorario()
        {
            txtCodHorario.ReadOnly = false;
            txtCodHorario.Text = "";
            txtCodHorario.Focus();
            txtDescHorario.Text = "";
            UpdateHorario = false;
            lblMsgHorario.Visible = false;
            txtMemoHorario.Text = "";
        }

        private void ColumnasGridhorario()
        {
            if (viewHorario.RowCount > 0)
            {
                viewHorario.Columns[0].Caption = "Codigo";
                viewHorario.Columns[0].Width = 20;

                viewHorario.Columns[1].Caption = "Descripción";
                viewHorario.Columns[1].Width = 100;

                viewHorario.Columns[2].Visible = false;
                //viewHorario.Columns[3].Visible = false;
            }
        }

        private bool CambiosSinHorario(TextEdit pDesc)
        {
            string Desc = "";
            if (viewHorario.RowCount > 0)
            {
                Desc = (string)viewHorario.GetFocusedDataRow()["desc"];

                if (Desc.ToLower() != pDesc.Text.ToLower())
                {
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region "TipoCuenta"
        /// <summary>
        /// Permite cargar loa datos de la fila seleccionada en los campos correspondientes.
        /// </summary>
        /// <param name="pOption"></param>
        private void CargarTipoCuenta(int? pOption = 0)
        {
            if (viewTipoCuenta.RowCount > 0)
            {
                if (Convert.ToInt32(pOption) != 0) viewTipoCuenta.FocusedRowHandle = Convert.ToInt32(pOption);
                
                txtDescTipoCuenta.Text = (string)viewTipoCuenta.GetFocusedDataRow()["nombre"];
                txtDato1Cuenta.Text = (string)viewTipoCuenta.GetFocusedDataRow()["dato01"];
                txtDato2Cuenta.Text = (string)viewTipoCuenta.GetFocusedDataRow()["dato02"];

                lblMsgTipoCuenta.Visible = false;
                //UpdateTipoCuenta = true;
                //btnEliminarTipoCuenta.Enabled = true;
                //op.SetButtonProperties(btnNuevoTipoCuenta, 1);
                //op.Cancela = false;
            }            
        }
        /// <summary>
        /// Dejar propiedades por defecto.
        /// </summary>
        private void LimpiarTipoCuenta()
        {
            UpdateTipoCuenta = false;
            btnEliminarTipoCuenta.Enabled = false;
            lblMsgTipoCuenta.Visible = false;
            txtDescTipoCuenta.Text = "";
            txtDescTipoCuenta.Focus();
        }
        /// <summary>
        /// Verificar si hay cambios sin guardar.
        /// </summary>
        /// <param name="pDesc"></param>
        /// <returns></returns>
        private bool CambiosSinTipo(string pDesc)
        {
            string descDb = "";
            bool cambios = false;
            if (viewTipoCuenta.RowCount > 0)
            {
                descDb = (string)viewTipoCuenta.GetFocusedDataRow()["nombre"];

                if (pDesc != descDb)
                { cambios = true; }
            }

            return cambios;
        }

        private void ColumnasCuenta()
        {
            if (viewTipoCuenta.RowCount > 0)
            {
                viewTipoCuenta.Columns[0].Caption = "N°";
                viewTipoCuenta.Columns[0].Width = 20;

                viewTipoCuenta.Columns[1].Caption = "Nombre";
                viewTipoCuenta.Columns[1].Width = 150;

                viewTipoCuenta.Columns[2].Width = 20;
                viewTipoCuenta.Columns[3].Width = 20;
            }
        }

        #endregion

        #region "SINDICATO"
        private void CargaSind()
        {
            if (viewSindicato.RowCount > 0)
            {
                txtCodSin.Text = Convert.ToInt32(viewSindicato.GetFocusedDataRow()["id"]).ToString();
                txtDescSin.Text = viewSindicato.GetFocusedDataRow()["descsin"].ToString();
                txtCodSin.ReadOnly = true;

                UpdateSindicato = true;
            }
            else
            {
                LimpiarSind();
            }          
        }

        private void LimpiarSind()
        {
            txtCodSin.Text = "";
            txtCodSin.ReadOnly = false;
            txtCodSin.Focus();
            txtDescSin.Text = "";
            UpdateSindicato = false;
        }

        private void ColumnasSindicato()
        {
            if (viewSindicato.RowCount > 0)
            {
                viewSindicato.Columns[0].Caption = "Código";
                viewSindicato.Columns[0].Width = 60;
                viewSindicato.Columns[0].AppearanceCell.FontStyleDelta = FontStyle.Bold;
                viewSindicato.Columns[1].Caption = "Descripcion";
            }
        }

        private bool CambiosSind()
        {
            bool cambio = false;
            if (viewSindicato.RowCount > 0)
            {
                if (txtDescSin.Text != (string)viewSindicato.GetFocusedDataRow()["descsin"])
                    cambio = true;
            }

            return cambio;
        }
        #endregion


        //NO MOSTRAR PESTAÑA A LA QUE NO SE TIENE PRIVILEGIOS
        private void OcultaTab()
        {
            //OBTENEMOS COLECCION DE PESTAÑAS DESDE EL CONTROL
            XtraTabPageCollection tabs = tabMain.TabPages;

            string Name = "";

            //ITERAMOS...
            for (int i = 0; i < tabs.Count; i++)
            {
                Name = tabs[i].Name.ToLower();

                if (Name == "tabarea")
                {
                    if (objeto.ValidaAcceso(User.GetUserGroup(), "frmarea") == false)
                    {
                        tabs[i].PageEnabled = false;
                    }
                        
                }
                else if (Name == "tabbanco")
                {
                    if (objeto.ValidaAcceso(User.GetUserGroup(), "frmbanco") == false)
                    {
                        tabs[i].PageEnabled = false;
                    }
                        
                }
                else if (Name == "tabcargo")
                {
                    if (objeto.ValidaAcceso(User.GetUserGroup(), "frmcargo") == false)
                    {
                        tabs[i].PageEnabled = false;
                    }                        
                }
                else if (Name == "tabcosto")
                {
                    if (objeto.ValidaAcceso(User.GetUserGroup(), "frmccosto") == false)
                    {
                        tabs[i].PageEnabled = false;
                    }                        
                }
                else if (Name == "tabformapago")
                {
                    if (objeto.ValidaAcceso(User.GetUserGroup(), "frmformapago") == false)
                    {
                        tabs[i].PageEnabled = false;
                    }                        
                }
                else if (Name == "tabnacion")
                {
                    if (objeto.ValidaAcceso(User.GetUserGroup(), "frmnacion") == false)
                    {
                        tabs[i].PageEnabled = false;                        
                    }
                }
                else if (Name == "tabcajacompensacion")
                {
                    if (objeto.ValidaAcceso(User.GetUserGroup(), "frmcajacompensacion") == false)
                    {
                        tabs[i].PageEnabled = false;
                    }
                }         
                    
            }          
        }

        private void tabMain_Deselecting(object sender, DevExpress.XtraTab.TabPageCancelEventArgs e)
        {
            //NUEVA ACTIVIDAD   
            Sesion.NuevaActividad();

            bool cambio = false;
            string busqueda = "";
            //OBTENER LA PAGINA ACTUAL
            string page = e.Page.Text;

            if (page == "Area")
            {
                busqueda = "SELECT id, nombre, dato01, dato02 FROM area WHERE id=@pId";
                cambio = fnCambios(txtidArea, txtNombreArea, txtdato1Area, txtdato2Area, busqueda);
                //SI CAMBIO ES TRUE SE CAMBIO ALGUNA CAJA SIN GUARDAR
                if (cambio)
                {
                    //SI CAMBIO ES TRUE, ES PORQUE SE MODIFICO ALGUNA CAJA DE TEXTO
                    DialogResult dialogo = XtraMessageBox.Show("¿Desea cambiar de pestaña de todas maneras?", "Cambios sin guardar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogo == DialogResult.No)
                    {
                        //NO CAMBIAMOS DE PESTAÑA
                        e.Cancel = true;
                    }

                }
            }
            else if (page == "Banco")
            {
                busqueda = "SELECT id, nombre, dato01, dato02 FROM banco WHERE id=@pId";
                cambio = fnCambios(txtIdBanco, txtNombreBanco, txtDato1Banco, txtDato2Banco, busqueda);
                //SI CAMBIO ES TRUE SE CAMBIO ALGUNA CAJA SIN GUARDAR                                
                if (cambio)
                {
                    //SI CAMBIO ES TRUE, ES PORQUE SE MODIFICO ALGUNA CAJA DE TEXTO
                    DialogResult dialogo = XtraMessageBox.Show("¿Desea cambiar de pestaña de todas maneras?", "Cambios sin guardar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogo == DialogResult.No)
                    {
                        //NO CAMBIAMOS DE PESTAÑA
                        e.Cancel = true;
                    }

                }
            }
            else if (page == "Cargo")
            {
                busqueda = "SELECT id, nombre, dato01, dato02 FROM cargo WHERE id=@pId";
                cambio = fnCambios(txtidCargo, txtnombreCargo, txtdato1Cargo, txtdato2Cargo, busqueda);
                //SI CAMBIO ES TRUE SE CAMBIO ALGUNA CAJA SIN GUARDAR                
                if (cambio)
                {
                    //SI CAMBIO ES TRUE, ES PORQUE SE MODIFICO ALGUNA CAJA DE TEXTO
                    DialogResult dialogo = XtraMessageBox.Show("¿Desea cambiar de pestaña de todas maneras?", "Cambios sin guardar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogo == DialogResult.No)
                    {
                        //NO CAMBIAMOS DE PESTAÑA
                        e.Cancel = true;
                    }

                }
            }
            else if (page == "Centro Costo")
            {
                busqueda = "SELECT id, nombre, dato01, dato02 FROM ccosto WHERE id=@pId";
                cambio = fnCambios(txtidCosto, txtnombreCosto, txtdato1Costo, txtdato2Costo, busqueda);
                //SI CAMBIO ES TRUE SE CAMBIO ALGUNA CAJA SIN GUARDAR

                if (cambio)
                {
                    //SI CAMBIO ES TRUE, ES PORQUE SE MODIFICO ALGUNA CAJA DE TEXTO
                    DialogResult dialogo = XtraMessageBox.Show("¿Desea cambiar de pestaña de todas maneras?", "Cambios sin guardar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogo == DialogResult.No)
                    {
                        //NO CAMBIAMOS DE PESTAÑA
                        e.Cancel = true;
                    }

                }
            }
            else if (page == "Forma de Pago")
            {
                busqueda = "SELECT id, nombre, dato01, dato02 FROM formaPago WHERE id=@pId";
                cambio = fnCambiosFormaPago(txtidPago, txtnombrePago, cbxTipoFormaPago, txtdato2Pago, busqueda);
                //SI CAMBIO ES TRUE SE CAMBIO ALGUNA CAJA SIN GUARDAR              

                if (cambio)
                {
                    //SI CAMBIO ES TRUE, ES PORQUE SE MODIFICO ALGUNA CAJA DE TEXTO
                    DialogResult dialogo = XtraMessageBox.Show("¿Desea cambiar de pestaña de todas maneras?", "Cambios sin guardar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogo == DialogResult.No)
                    {
                        //NO CAMBIAMOS DE PESTAÑA
                        e.Cancel = true;
                    }

                }
            }
            else if (page == "Nacionalidad")
            {
                //NACIONALIDAD
                busqueda = "SELECT id, nombre, dato01, dato02 FROM nacion WHERE id=@pId";
                cambio = fnCambios(txtidNacion, txtnombreNacion, txtdato1Nacion, txtdato2Nacion, busqueda);
                //SI CAMBIO ES TRUE SE CAMBIO ALGUNA CAJA SIN GUARDAR                
                if (cambio)
                {
                    //SI CAMBIO ES TRUE, ES PORQUE SE MODIFICO ALGUNA CAJA DE TEXTO
                    DialogResult dialogo = XtraMessageBox.Show("¿Desea cambiar de pestaña de todas maneras?", "Cambios sin guardar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogo == DialogResult.No)
                    {
                        //NO CAMBIAMOS DE PESTAÑA
                        e.Cancel = true;
                    }

                }
            }
            else if (page == "Sucursal")
            {
                if (viewSucursal.RowCount > 0)
                {
                    int cod = Convert.ToInt32(viewSucursal.GetFocusedDataRow()["codSucursal"]);
                    if (SucursalSinGuardar(txtCodigoSucursal.Text, txtNombreSucursal.Text, txtDato1Sucursal.Text, txtDato2Sucursal.Text, cod))
                    {
                        DialogResult dialogo = XtraMessageBox.Show("¿Desea cambiar de pestaña de todas maneras?", "Cambios sin guardar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dialogo == DialogResult.No)
                        {
                            e.Cancel = true;
                        }
                    }
                }
            }
            else if (page == "Caja compensacion")
            {
                int id = 0;
                if (viewCajacompensacion.RowCount > 0)
                {
                    id = Convert.ToInt32(viewCajacompensacion.GetFocusedDataRow()["id"]);
                    if (CambiosSinGuardar(id, txtcodPrevired.Text, txtNombrecaja.Text))
                    {
                        DialogResult dialogo = XtraMessageBox.Show("¿Desea cambiar de pestaña de todas formas?", "Cambios sin guardar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dialogo == DialogResult.No)
                            e.Cancel = true;
                    }
                }
            }
            else if (page == "Causal")
            {
                int id = 0;
                if (viewCausal.RowCount > 0)
                {
                    id = Convert.ToInt32(viewCausal.GetFocusedDataRow()["codCausal"]);
                    if (id > 0)
                    {
                        if (CambiosSinGuardarCausal(id, txtDescCausal.Text, txtJustificacion.Text, cbAviso.Checked == false ? 0 : 1, cbAnioServ.Checked == false ? 0: 1))
                        {
                            DialogResult dialogo = XtraMessageBox.Show("¿Desea cambiar de pestaña de todas formas?", "Cambios sin guardar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (dialogo == DialogResult.No)
                                e.Cancel = true;
                        }
                    }                    
                }
            }
            else if (page == "Escolaridad")
            {
                int cod = 0;
                if (viewEsco.RowCount > 0)
                {
                    Escolaridad escolar = new Escolaridad();

                    cod = Convert.ToInt32(viewEsco.GetFocusedDataRow()["codesc"]);
                    if (escolar.ExisteCodigo(cod))
                    {
                        if (escolar.CambiosSinGuardar(viewEsco, txtCodEsco, txtDescEsco))
                        {
                            DialogResult advertencia = XtraMessageBox.Show("¿Desea cambiar de pestaña de todas formas?", "Cambios sin guardar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (advertencia == DialogResult.No)
                                e.Cancel = true;
                        }
                    }
                }
            }
            else if (page == "horario")
            {
                if (CambiosSinHorario(txtDescHorario))
                {
                    DialogResult Advertencia = XtraMessageBox.Show("¿Desea cambiar de pestaña de todas formas?", "Cambios sin guardar", MessageBoxButtons.YesNo, MessageBoxIcon.Stop);
                    if (Advertencia == DialogResult.No)
                        e.Cancel = true;
                }
            }
            else if (page == "Sindicato")
            {
                if (CambiosSind())
                {
                    DialogResult Advertencia = XtraMessageBox.Show("¿Desea cambiar de pestaña de todas formas?", "Cambios Sin Guardar", MessageBoxButtons.YesNo, MessageBoxIcon.Stop);
                    if (Advertencia == DialogResult.Yes)
                        e.Cancel = true;
                }
            }
          
        }

        private void txtNombreArea_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void txtNombreBanco_KeyPress(object sender, KeyPressEventArgs e)
        {
         
        }

        private void txtnombreCargo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) || e.KeyChar == (char)46 || e.KeyChar == (char)44 || e.KeyChar == (char)47)
            {
                e.Handled = false;
            }
            else if (char.IsLetter(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (char.IsSeparator(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void txtnombreCosto_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void txtnombrePago_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) || e.KeyChar == (char)46 || e.KeyChar == (char)44 || e.KeyChar == (char)47)
            {
                e.Handled = false;
            }
            else if (char.IsLetter(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (char.IsSeparator(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void txtnombreNacion_KeyPress(object sender, KeyPressEventArgs e)
        {
           
        }

        //SOLO NUMEROS
        private void txtdato1Area_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtdato2Area_KeyPress(object sender, KeyPressEventArgs e)
        {
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

        private void txtDato1Banco_KeyPress(object sender, KeyPressEventArgs e)
        {
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

        private void txtDato2Banco_KeyPress(object sender, KeyPressEventArgs e)
        {
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

        private void txtdato1Cargo_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtdato2Cargo_KeyPress(object sender, KeyPressEventArgs e)
        {
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

        private void txtdato1Costo_KeyPress(object sender, KeyPressEventArgs e)
        {
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

        private void txtdato1Pago_KeyPress(object sender, KeyPressEventArgs e)
        {
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

        private void txtdato2Pago_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtdato1Nacion_KeyPress(object sender, KeyPressEventArgs e)
        {
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

        private void txtdato2Nacion_KeyPress(object sender, KeyPressEventArgs e)
        {
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

        private void txtdato2Costo_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtidArea_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtNombreArea_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtdato1Area_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtdato2Area_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtIdBanco_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtNombreBanco_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtDato1Banco_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtDato2Banco_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtidCargo_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtnombreCargo_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtdato1Cargo_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtdato2Cargo_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtidCosto_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtnombreCosto_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtdato1Costo_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtdato2Costo_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtidPago_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtnombrePago_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtdato1Pago_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtdato2Pago_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtidNacion_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtnombreNacion_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtdato1Nacion_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtdato2Nacion_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void btnSalirArea_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            string busqueda = "";
            int id = 0;

            busqueda = "SELECT id, nombre, dato01, dato02 FROM area WHERE id=@pId";

            if (viewArea.RowCount > 0)
            {
                id = Convert.ToInt32(viewArea.GetFocusedDataRow()["id"]);

                if (fnCambios(txtidArea, txtNombreArea, txtdato1Area, txtdato2Area, busqueda))
                {
                    DialogResult adv = XtraMessageBox.Show("Hay cambios sin guardar, ¿deseas cerrar de todas formas?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (adv == DialogResult.Yes)
                        Close();
                }
                else
                    Close();
            }
            else
                Close();

        }

        private void btnSalirBanco_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            string busqueda = "";
            int id = 0;

            busqueda = "SELECT id, nombre, dato01, dato02 FROM banco WHERE id=@pId";

            if (viewBanco.RowCount > 0)
            {
                id = Convert.ToInt32(viewBanco.GetFocusedDataRow()["id"]);

                if (fnCambios(txtIdBanco, txtNombreBanco, txtDato1Banco, txtDato2Banco, busqueda))
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

        private void btnSalirCargo_Click(object sender, EventArgs e)
        {
            //SESION ACTIVIDAD
            Sesion.NuevaActividad();

            string busqueda = "";
            int id = 0;

            busqueda = "SELECT id, nombre, dato01, dato02 FROM cargo WHERE id=@pId";

            if (viewCargo.RowCount > 0)
            {
                id = Convert.ToInt32(viewCargo.GetFocusedDataRow()["id"]);
                if (fnCambios(txtidCargo, txtnombreCargo, txtdato1Cargo, txtdato2Cargo, busqueda))
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

        private void btnSalirCosto_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            string busqueda = "";
            int id = 0;

            busqueda = "SELECT id, nombre, dato01, dato02 FROM ccosto WHERE id=@pId";
            if (viewCosto.RowCount > 0)
            {
                id = Convert.ToInt32(viewCosto.GetFocusedDataRow()["id"]);
                if (fnCambios(txtidCosto, txtnombreCosto, txtdato1Costo, txtdato2Costo, busqueda))
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

        private void btnSalirPago_Click(object sender, EventArgs e)
        {
            //SESION ACTIVIDAD
            Sesion.NuevaActividad();

            string busqueda = "";
            int id = 0;

            busqueda = "SELECT id, nombre, dato01, dato02 FROM formaPago WHERE id=@pId";
            if (viewPago.RowCount > 0)
            {
                id = Convert.ToInt32(viewPago.GetFocusedDataRow()["id"]);
                if (fnCambiosFormaPago(txtidPago, txtnombrePago, cbxTipoFormaPago, txtdato2Pago, busqueda))
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

        private void btnSalirNacion_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            string busqueda = "";
            int id = 0;

            busqueda = "SELECT id, nombre, dato01, dato02 FROM nacion WHERE id=@pId";

            if (viewNacion.RowCount > 0)
            {
                id = Convert.ToInt32(viewNacion.GetFocusedDataRow()["id"]);
                if (fnCambios(txtidNacion, txtnombreNacion, txtdato1Nacion, txtdato2Nacion, busqueda))
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

        //PARA CAJA DE COMPENSACION
        private void btnNuevoCaja_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            if (op.Cancela == false)
            {
                //SE HIZO CLICK 
                LimpiarCajaCompensacion();
                op.SetButtonProperties(btnNuevoCaja, 2);
                op.Cancela = true;

            }
            else
            {
                //RESET 
                CargarDataFromGrid();
            }
        }

        //PARA CAJA DE COMPENSACION
        private void btnGuardacaja_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            if (txtNombrecaja.Text == "")
            { lblmsgCaja.Visible = true; lblmsgCaja.Text = "Por favor ingresa un nombre"; return; }

            int id = 0;
            string nameDb = "";
            //UPDATE
            if (ModificaCaja)
            {
                if (viewCajacompensacion.RowCount > 0)
                {
                    id = Convert.ToInt32(viewCajacompensacion.GetFocusedDataRow()["id"]);
                    nameDb = (string)viewCajacompensacion.GetFocusedDataRow()["desccaja"];

                    //SI SON DISTINTOS PERO EL NOMBRE YA EXISTE MOSTRAMOS MENSAJE
                    if (nameDb.ToLower() != txtNombrecaja.Text.ToLower())
                    {
                        if (ExisteCaja(txtNombrecaja.Text))
                        {
                            lblmsgCaja.Visible = true;
                            lblmsgCaja.Text = "Nombre caja ya existe";
                            return;
                        }
                    }

                    //MODIFICAMOS
                    ModificarCajaCompensacion(id, txtNombrecaja, txtcodPrevired);
                }                    
            }
            else
            {
                if (ExisteCaja(txtNombrecaja.Text))
                { lblmsgCaja.Visible = true; lblmsgCaja.Text = "Caja ya existe"; return; }

                //INSERT
                NuevaCajaCompensacion(txtNombrecaja, txtcodPrevired);
            }
            
        }

        private void btnEliminarcaja_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            if (viewCajacompensacion.RowCount == 0)
            { lblmsgCaja.Visible = true; lblmsgCaja.Text = "Registro no valido"; return; }

            int id = 0;
            string caja = "";
            if (viewCajacompensacion.RowCount > 0)
            {
                id = Convert.ToInt32(viewCajacompensacion.GetFocusedDataRow()["id"]);
                caja = (string)viewCajacompensacion.GetFocusedDataRow()["desccaja"];

                //ELIMINAMOS REGISTRO
                EliminarCajaCompensacion(id, caja);
            }
        }

        private void gridCajacompensacion_Click(object sender, EventArgs e)
        {
            //NUEVA SESION
            Sesion.NuevaActividad();

            if (viewCajacompensacion.RowCount>0)
            {
                CargarDataFromGrid();
            }
        }

        private void gridCajacompensacion_KeyUp(object sender, KeyEventArgs e)
        {
            //NUEVA SESION
            Sesion.NuevaActividad();

            if (viewCajacompensacion.RowCount>0)
            {
                CargarDataFromGrid();
            }
        }

        private void btnSalirCaja_Click(object sender, EventArgs e)
        {
            //SESION NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            int id = 0;
            if (viewCajacompensacion.RowCount>0)
            {
                id = Convert.ToInt32(viewCajacompensacion.GetFocusedDataRow()["id"]);
                if (CambiosSinGuardar(id, txtcodPrevired.Text, txtNombrecaja.Text))
                {
                    DialogResult advertencia = XtraMessageBox.Show("Hay cambios sin guardar, ¿deseas realmente salir?", "Cambios", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (advertencia == DialogResult.Yes)
                    {
                        Close();
                    }
                }
                else { Close(); }
            }
            else
            {
                Close();
            }            
        }

        private void txtCodigoSucursal_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void txtNombreSucursal_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void btnNuevaSucursal_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            if (op.Cancela == false)
            {
                //LIMPIAMOS CAMPOS
                LimpiarSucursal();

                op.SetButtonProperties(btnNuevaSucursal, 2);
                op.Cancela = true;

                txtCodigoSucursal.Properties.Appearance.BackColor = Color.LightGoldenrodYellow;
            }
            else
            {
                //CARGAMOS SI HAY REGISTROS
                CargarSucursal(viewSucursal.FocusedRowHandle);

                op.Cancela = false;
                op.SetButtonProperties(btnNuevaSucursal, 1);
                txtCodigoSucursal.Properties.Appearance.BackColor = Color.White;

            }
        }

        private void txtDato1Sucursal_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtDato2Sucursal_KeyPress(object sender, KeyPressEventArgs e)
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

        private void gridSucursal_Click(object sender, EventArgs e)
        {
            //NUEVA SESION
            Sesion.NuevaActividad();

            if (viewSucursal.RowCount > 0)
            {
                CargarSucursal();
            }
        }

        private void gridSucursal_KeyUp(object sender, KeyEventArgs e)
        {
            //NUEVA SESION
            Sesion.NuevaActividad();

            if (viewSucursal.RowCount > 0)
            {
                CargarSucursal();
            }
        }

        private void btnEliminarSucursal_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD   
            Sesion.NuevaActividad();

            if (txtCodigoSucursal.Text == "")
            { XtraMessageBox.Show("La sucursal que intentas eliminar no es valida", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (ExisteSucursal(Convert.ToInt32(txtCodigoSucursal.Text)) == false)
            { XtraMessageBox.Show("La sucursal que intentas eliminar no es valida", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
            

            int codBd = 0;
            if (viewSucursal.RowCount > 0)
            {
                codBd = Convert.ToInt32(viewSucursal.GetFocusedDataRow()["codSucursal"]);

                if (codBd != Convert.ToInt32(txtCodigoSucursal.Text))
                { XtraMessageBox.Show("La sucursal que intentas eliminar no es valida", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                if (SucursalEnUso(codBd))
                { XtraMessageBox.Show("No puedes eliminar este registro porque está en uso", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                //ELIMINAMOS
                DialogResult advertencia = XtraMessageBox.Show($"¿Realmente deseas eliminar la sucursal N° {codBd}?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (advertencia == DialogResult.Yes)
                {
                    EliminarSucursal(codBd);
                }
            }
        }

        private void btnGuardarSucursal_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            if (txtCodigoSucursal.Text == "")
            { lblSucursal.Visible = true;lblSucursal.Text = "Por favor ingresa codigo sucursal"; txtCodigoSucursal.Focus(); return; }

            if (txtNombreSucursal.Text == "")
            { lblSucursal.Visible = true; lblSucursal.Text = "Por favor ingresa el nombre de la sucursal"; txtNombreSucursal.Focus(); return; }

            lblSucursal.Visible = false;
            int codBd = 0;

            if (updateSucursal)
            {
                //HACEMOS UPDATE
                if (viewSucursal.RowCount == 0)
                { XtraMessageBox.Show("Registro no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

                codBd = Convert.ToInt32(viewSucursal.GetFocusedDataRow()["codSucursal"]);               

                if (ExisteSucursal(codBd) == false)
                { lblSucursal.Visible = true; lblSucursal.Text = "Registro no valido"; return; }

                lblSucursal.Visible = false;

                if (codBd != Convert.ToInt32(txtCodigoSucursal.Text))
                {
                    if (ExisteSucursal(Convert.ToInt32(txtCodigoSucursal.Text)))
                    { lblSucursal.Visible = true; lblSucursal.Text = "Ya existe una sucursal con ese codigo"; txtCodigoSucursal.Focus(); return; }

                    lblSucursal.Visible = false;

                    if (SucursalEnUso(codBd))
                    { XtraMessageBox.Show("No puedes modificar el codigo de una sucursal en uso", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                    //HACEMOS UPDATE SIN PROBLEMAS
                    ModificaSucursal(txtCodigoSucursal, txtNombreSucursal, txtDato1Sucursal, txtDato2Sucursal, codBd);
                }
                else
                {
                    ModificaSucursal(txtCodigoSucursal, txtNombreSucursal, txtDato1Sucursal, txtDato2Sucursal, codBd);
                }               
            }
            else
            {
                //HACEMOS INSERT
                if (ExisteSucursal(Convert.ToInt32(txtCodigoSucursal.Text)))
                { lblSucursal.Visible = true; lblSucursal.Text = "Ya existe una sucursal con ese codigo"; txtCodigoSucursal.Focus(); return; }

                IngresarSucursal(txtCodigoSucursal, txtNombreSucursal, txtDato1Sucursal, txtDato2Sucursal);
            }
        }

        private void btnSalirSucursal_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            if (viewSucursal.RowCount > 0)
            {
                int pCod = 0;
                pCod = Convert.ToInt32(viewSucursal.GetFocusedDataRow()["codSucursal"]);
                if (SucursalSinGuardar(txtCodigoSucursal.Text, txtNombreSucursal.Text, txtDato1Sucursal.Text, txtDato2Sucursal.Text, pCod))
                {
                    DialogResult advertencia = XtraMessageBox.Show("Hay cambios sin guardar, ¿Deseas salir de todas maneras?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (advertencia == DialogResult.Yes)
                        Close();
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

        private void gridBanco_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void gridSucursal_ChangeUICues(object sender, UICuesEventArgs e)
        {

        }

        private void txtcodPrevired_KeyPress(object sender, KeyPressEventArgs e)
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

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnGuardarCausal_Click(object sender, EventArgs e)
        {
            if (txtCodCausal.Text == "") { lblmsgCausal.Visible = true; lblmsgCausal.Text = "Por favor ingrese un codigo"; return; }
            if (txtDescCausal.Text == "") { lblmsgCausal.Visible = true; lblmsgCausal.Text = "Por favor ingrese una descripcion"; return; }
            
            int codBd = 0;
            if (UpdateCausal == false)
            {
                //INSERT
                //VERIFICAR QUE EL CODIGO QUE SE QUIERE INGRESAR NO EXISTE

                if (ExisteCausal(Convert.ToInt32(txtCodCausal.Text)))
                {
                    lblmsgCausal.Visible = true;
                    lblmsgCausal.Text = "Codigo de causal ya existe";
                }
                else
                {
                    lblmsgCausal.Visible = false;
                    //HACEMOS INSERT
                    fnNuevaCausal(txtCodCausal, txtDescCausal, txtJustificacion, cbAviso, cbAnioServ);
                }
            }
            else
            {
                //UPDATE             
                if (viewCausal.RowCount > 0)
                {
                    //CODIGO DESDE BD
                    codBd = Convert.ToInt32(viewCausal.GetFocusedDataRow()["codCausal"]);

                    if (ExisteCausal(codBd) == false)
                    { lblmsgCausal.Visible = true; lblmsgCausal.Text = "Registro no valido"; return; }

                    lblmsgCausal.Visible = false;

                    //HACEMOS UPDATE
                    fnModificarCausal(txtDescCausal, codBd, txtJustificacion, cbAviso, cbAnioServ);
                }
            }
        }

        private void btnEliminarCausal_Click(object sender, EventArgs e)
        {
            if (txtCodCausal.Text == "") { lblmsgCausal.Visible = true; lblmsgCausal.Text = "Registro no valido"; return; }
            lblmsgCausal.Visible = false;

            int code = 0;

            //OBTENEMOS EL CODIGO DEL REGISTRO
            if (viewCausal.RowCount > 0)
            {
                code = Convert.ToInt32(viewCausal.GetFocusedDataRow()["codcausal"]);
            }
            else
            {
                XtraMessageBox.Show("Registro no válido", "Registro", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            //VERIFICAR SI LA CAUSAL ESTA SIENDO USADA POR UN TRABAJADOR
            bool causalUsada = fnCausalTrabajador(code);
            if (causalUsada)
            {
                //ESTA SIENDO USADA 
                DialogResult adver = XtraMessageBox.Show("Esta causal esta asociada a un trabajador, por lo que no es posible realizar esta operacion", "Denegado", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            else
            {
                DialogResult dialogo = XtraMessageBox.Show("¿Desea realmente eliminar la causal de termino " + code + " ?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogo == DialogResult.Yes)
                {
                    fnEliminarCausal(code);
                }
            }
        }

        private void btnNuevoCausal_Click(object sender, EventArgs e)
        {           

            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            if (op.Cancela == false)
            {
                //LIMPIAMOS CAMPOS
                fnLimpiarCamposCausal();
               
            }
            else
            {
                //CARGAMOS SI HAY REGISTROS
                fnCargarCamposCausal();

                /*op.Cancela = false;
                op.SetButtonProperties(btnNuevoCausal, 1);
                txtCodCausal.Properties.Appearance.BackColor = Color.White;*/

            }
        }

        private void gridCausal_Click(object sender, EventArgs e)
        {
            fnCargarCamposCausal();
        }

        private void gridCausal_KeyUp(object sender, KeyEventArgs e)
        {
            fnCargarCamposCausal();
        }

        private void txtCodCausal_KeyPress(object sender, KeyPressEventArgs e)
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

        private void btnSalirCausal_Click(object sender, EventArgs e)
        {
            int id = 0;
            if (viewCausal.RowCount > 0)
            {
                id = Convert.ToInt32(viewCausal.GetFocusedDataRow()["codcausal"]);
                if (id > 0)
                {
                    if (CambiosSinGuardarCausal(id, txtDescCausal.Text, txtJustificacion.Text, cbAviso.Checked == false ? 0 : 1, cbAnioServ.Checked == false ? 0 : 1))
                    {
                        DialogResult Advertencia = XtraMessageBox.Show("¿Deseas realmente cerrar?", "Cambios sin guardar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (Advertencia == DialogResult.Yes)
                            Close();
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
            else
            {
                Close();
            }
        }

        private void btnNuevoEsco_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            //NUEVO REGISTRO
            if (op.Cancela == false)
            {
                
                //LIMPIAMOS CAMPOS
                LimpiarCajasEsco();

                op.Cancela = true;
                op.SetButtonProperties(btnNuevoEsco, 2);
            }
            else
            {
               
                //CARGAMOS SI HAY REGISTROS
                CargarInfoEsco();
                op.Cancela = false;
                op.SetButtonProperties(btnNuevoEsco, 1);
                btnEliminarEsco.Enabled = true;
            }
        }

        private void btnSalirEsco_Click(object sender, EventArgs e)
        {
            int cod = 0;
            Escolaridad escolar = new Escolaridad();
            if (viewEsco.RowCount > 0)
            {
                cod = Convert.ToInt32(viewEsco.GetFocusedDataRow()["codesc"]);
                if (escolar.ExisteCodigo(cod))
                {
                    if (escolar.CambiosSinGuardar(viewEsco, txtCodEsco, txtDescEsco))
                    {
                        DialogResult advertencia = XtraMessageBox.Show("¿Realmente deseas cerrar esta ventana?", "Cambios sin guardar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (advertencia == DialogResult.Yes)
                            this.Close();
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
            else
            {
                Close();
            }
        }

        private void btnGuardarEsco_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            if (txtCodEsco.Text == "") { XtraMessageBox.Show("Por favor ingresa un código válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtCodEsco.Focus(); return; }
            if (txtDescEsco.Text == "") { XtraMessageBox.Show("Por favor ingresa una descripcion", "Descripción", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtDescEsco.Focus(); return; }

            Escolaridad escolar = new Escolaridad();
            bool correcto = false;
            int codBd = 0;

            //INSERT
            if (UpdateEscolaridad == false)
            {
                //EXISTE REGISTRO?
                if (escolar.ExisteCodigo(Convert.ToInt32(txtCodEsco.Text)))
                { XtraMessageBox.Show("Código ingresado ya existe", "Código", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtCodEsco.Focus(); return; }

                correcto = escolar.NuevaEscolaridad(txtCodEsco, txtDescEsco);
                if (correcto)
                {
                    XtraMessageBox.Show("Registro ingresado correctamente", "Registro", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    logRegistro log = new logRegistro(User.getUser(), $"SE INGRESA NUEVA ESCOLARIDAD CON CÓDIGO {txtCodEsco.Text}", "ESCOLARIDAD", "", txtDescEsco.Text, "INGRESAR");
                    log.Log();

                    fnSistema.spllenaGridView(gridEsco, SqlEscolaridad);
                    fnSistema.spOpcionesGrilla(viewEsco);
                    escolar.OpcionesGrilla(viewEsco);
                    CargarInfoEsco();                   
                    op.Cancela = false;
                    op.SetButtonProperties(btnNuevoEsco, 1);

                }
                else
                {
                    XtraMessageBox.Show("Registro no se pudo ingresar", "Registro", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }

            }
            //UPDATE
            else
            {
                if (viewEsco.RowCount == 0) { XtraMessageBox.Show("Registro no válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                if (escolar.ExisteCodigo(Convert.ToInt32(txtCodEsco.Text)) == false)
                { XtraMessageBox.Show("Registro no válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                if (viewEsco.RowCount > 0)
                {
                    codBd = Convert.ToInt32(viewEsco.GetFocusedDataRow()["codesc"]);

                    if (escolar.ExisteCodigo(codBd))
                    {
                        correcto = escolar.ActualizarEscolaridad(codBd, txtDescEsco);

                        if (correcto)
                        {
                            XtraMessageBox.Show("Registro actualizado correctamente", "Registro", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            fnSistema.spllenaGridView(gridEsco, SqlEscolaridad);
                            fnSistema.spOpcionesGrilla(viewEsco);
                            escolar.OpcionesGrilla(viewEsco);
                            CargarInfoEsco();                        
                            op.Cancela = false;
                            op.SetButtonProperties(btnNuevoEsco, 1);
                        }
                        else
                        {
                            XtraMessageBox.Show("No se pudo actualizar registro", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            return;
                        }
                    }

                }
            }


        }

        private void btnEliminarEsco_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();
            int pCod = 0;
            string DescDb = "";
            Escolaridad escolar = new Escolaridad();
            bool correcto = false;

            if (viewEsco.RowCount == 0) { XtraMessageBox.Show("Registro no válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (viewEsco.RowCount > 0)
            {
                pCod = Convert.ToInt32(viewEsco.GetFocusedDataRow()["codesc"]);
                DescDb = viewEsco.GetFocusedDataRow()["descesc"].ToString();

                if (escolar.ExisteCodigo(pCod))
                {
                    //USADO POR TRABAJADOR?
                    if (escolar.EscolarEnUso(pCod))
                    { XtraMessageBox.Show("No puedes eliminar este registro porque está asociado a un trabajador", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                    DialogResult Advertencia = XtraMessageBox.Show($"¿Realmente deseas eliminar el registro '{DescDb}'?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (Advertencia == DialogResult.Yes)
                    {
                        correcto = escolar.EliminaEscolaridad(pCod);

                        if (correcto)
                        {
                            XtraMessageBox.Show("Registro eliminado correctamente", "Registro", MessageBoxButtons.OK, MessageBoxIcon.Stop);

                            logRegistro log = new logRegistro(User.getUser(), "SE ELIMINAR ESCOLARIDAD CODIGO: " + pCod, "ESCOLARIDAD", DescDb, "", "ELIMINAR");

                            fnSistema.spllenaGridView(gridEsco, SqlEscolaridad);
                            fnSistema.spOpcionesGrilla(viewEsco);
                            escolar.OpcionesGrilla(viewEsco);
                            CargarInfoEsco();                          
                            op.Cancela = false;
                            op.SetButtonProperties(btnNuevoEsco, 1);
                        }
                        else
                        {
                            XtraMessageBox.Show("No se pudo eliminar registro", "Registro", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        }   
                    }                  
                }
                else
                {
                    XtraMessageBox.Show("Registro no válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
        }

        private void txtCodEsco_KeyPress(object sender, KeyPressEventArgs e)
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

        private void gridEsco_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();
     

            if (viewEsco.RowCount > 0)
            {
                CargarInfoEsco();
                op.Cancela = false;
                op.SetButtonProperties(btnNuevoEsco, 1);
            }
        }      

        private void gridEsco_KeyUp(object sender, KeyEventArgs e)
        {
            Sesion.NuevaActividad();

            if (viewEsco.RowCount > 0)
            {
                CargarInfoEsco();
                op.Cancela = false;
                op.SetButtonProperties(btnNuevoEsco, 1);
            }
        }

        private void txtCodHorario_KeyPress(object sender, KeyPressEventArgs e)
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

        private void btnNuevoHorario_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            if (op.Cancela == false)
            {
                //LIMPIAMOS CAMPOS
                LimpiaHorario();

                op.Cancela = true;
                op.SetButtonProperties(btnNuevoHorario, 2);                
            }
            else
            {
                //CARGAMOS datos
                CargaCamposHorario();

                op.Cancela = false;
                op.SetButtonProperties(btnNuevoHorario, 1);
            }
        }

        private void btnGuardarHorario_Click(object sender, EventArgs e)
        {
            if (txtCodHorario.Text.Length == 0)
            { XtraMessageBox.Show("Por favor ingresa un codigo válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtCodHorario.Focus(); return; }

            if (txtDescHorario.Text.Length == 0)
            { XtraMessageBox.Show("Por favor ingresa un descripcion válida", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtDescHorario.Focus(); return; }

            Horario hor = new Horario();
            bool correcto = false;
            int Cod = 0;

            //INSERT
            if (UpdateHorario == false)
            {
                if (hor.ExisteRegistro(Convert.ToInt32(txtCodHorario.Text)))
                { XtraMessageBox.Show("Código ingresado ya existe", "información", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtCodHorario.Focus(); return; }

                correcto = hor.NuevoHorario(txtCodHorario, txtDescHorario, txtMemoHorario);
                if (correcto)
                {
                    XtraMessageBox.Show("Nuevo horario ingresado correctamente", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    fnSistema.spllenaGridView(gridHorario, SqlHorario);
                    fnSistema.spOpcionesGrilla(viewHorario);
                    ColumnasGridhorario();
                    op.Cancela = false;
                    op.SetButtonProperties(btnNuevoHorario, 1);
                }
                else
                {
                    XtraMessageBox.Show("No se pudo ingresar nuevo horario", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
            else
            {
                //UPDATE
                if (viewHorario.RowCount > 0)
                {
                    Cod = Convert.ToInt32(viewHorario.GetFocusedDataRow()["id"]);

                    if (hor.ExisteRegistro(Cod))
                    {
                        correcto = hor.ActualizaHorario(Cod, txtDescHorario, txtMemoHorario);
                        if (correcto)
                        {
                            XtraMessageBox.Show("horario actualizado correctamente", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            fnSistema.spllenaGridView(gridHorario, SqlHorario);
                            fnSistema.spOpcionesGrilla(viewHorario);
                            ColumnasGridhorario();
                            CargaCamposHorario();

                            op.Cancela = false;
                            op.SetButtonProperties(btnNuevoHorario, 1);
                        }
                    }
                    else
                    {
                        XtraMessageBox.Show("Registro no válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                }
                else
                {
                    XtraMessageBox.Show("Registro no válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
        }

        private void btnEliminarHorario_Click(object sender, EventArgs e)
        {
            int Cod = 0;
            Horario hor = new Horario();
            bool correcto = false;

            if (viewHorario.RowCount > 0)
            {
                Cod = Convert.ToInt32(viewHorario.GetFocusedDataRow()["id"]);
                if (hor.ExisteRegistro(Cod))
                {
                    if (hor.RegistroEnUso(Cod))
                    { XtraMessageBox.Show("No puedes eliminar este registro porque está asociado a un trabajador.", "Registro", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                    DialogResult Advertencia = XtraMessageBox.Show("¿Estás seguro de elimninar el registro " + viewHorario.GetFocusedDataRow()["deschor"] + "?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Stop);
                    if (Advertencia == DialogResult.No)
                        return;

                    correcto = hor.EliminarHorario(Cod, (string)viewHorario.GetFocusedDataRow()["deschor"]);
                    if (correcto)
                    {
                        XtraMessageBox.Show("Registro " + (string)viewHorario.GetFocusedDataRow()["deschor"] + " eliminado correctamente", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        fnSistema.spllenaGridView(gridHorario, SqlHorario);
                        fnSistema.spOpcionesGrilla(viewHorario);
                        ColumnasGridhorario();
                        CargaCamposHorario();
                    }
                    else
                    {
                        XtraMessageBox.Show("No se pudo eliminar registro", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                }
                else
                {
                    XtraMessageBox.Show("Registro no válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
            else
            {
                XtraMessageBox.Show("Registro no válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void gridHorario_Click(object sender, EventArgs e)
        {
            if (viewHorario.RowCount > 0)
            {
                CargaCamposHorario();
                op.Cancela = false;
                op.SetButtonProperties(btnNuevoHorario, 1);
            }
        }

        private void gridHorario_KeyUp(object sender, KeyEventArgs e)
        {
            if (viewHorario.RowCount > 0)
            {
                CargaCamposHorario();
                op.Cancela = false;
                op.SetButtonProperties(btnNuevoHorario, 1);
            }
        }

        private void btnSalirHorario_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void txtCodigoSucursal_KeyPress_1(object sender, KeyPressEventArgs e)
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

        private void btnNuevoTipoCuenta_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();
            
            if (op.Cancela == false)
            {
                LimpiarTipoCuenta();
                op.SetButtonProperties(btnNuevoTipoCuenta, 2);
                op.Cancela = true;
                txtDescTipoCuenta.Properties.Appearance.BackColor = Color.LightGoldenrodYellow;
            }
            else
            {
                lblMsgTipoCuenta.Visible = false;
                op.SetButtonProperties(btnNuevoTipoCuenta, 1);
                op.Cancela = false;
                CargarTipoCuenta();
                txtDescTipoCuenta.Properties.Appearance.BackColor = Color.White;
            }
        }

        private void btnGuardarTipoCuenta_Click(object sender, EventArgs e)
        {
            TipoCuenta Cuenta = new TipoCuenta();

            //Nuevo ingreso
            if (UpdateTipoCuenta == false)
            {                

                if (txtDescTipoCuenta.Text == "")
                { lblMsgTipoCuenta.Visible = true; lblMsgTipoCuenta.Text = "Por favor ingresa una nombre para el tipo de cuenta"; txtDescTipoCuenta.Focus(); return; }                

                lblMsgTipoCuenta.Visible = false;

                if (Cuenta.NuevaCuenta(txtDescTipoCuenta, txtDato1Cuenta, txtDato2Cuenta))
                {
                    XtraMessageBox.Show($"Tipo de cuenta {txtDescTipoCuenta.Text} registrado correctamente", "Tipo Cuenta", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    fnSistema.spllenaGridView(gridTipoCuenta, SqlTipoCuenta);
                    fnSistema.spOpcionesGrilla(viewTipoCuenta);
                    CargarTipoCuenta();
                    ColumnasCuenta();
                }
                else
                {
                    XtraMessageBox.Show($"No se pudo registrar tipo de cuenta {txtDescTipoCuenta.Text}", "Tipo Cuenta", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }

            }
            //ACTUALIZACION
            else
            {
                lblMsgTipoCuenta.Visible = false;

                if (viewTipoCuenta.RowCount == 0)
                { XtraMessageBox.Show("Registro no válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }               

                if (viewTipoCuenta.RowCount > 0)
                {
                    if (Cuenta.ExisteCodigo(Convert.ToInt32(viewTipoCuenta.GetFocusedDataRow()["id"])))
                    {                        

                        if (Cuenta.ModificarCuenta(Convert.ToInt32(viewTipoCuenta.GetFocusedDataRow()["id"]), txtDescTipoCuenta, txtDato1Cuenta, txtDato2Cuenta))
                        {
                            XtraMessageBox.Show("Registro actualizado correctamente", "Tipo Cuenta", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            fnSistema.spllenaGridView(gridTipoCuenta, SqlTipoCuenta);
                            fnSistema.spOpcionesGrilla(viewTipoCuenta);
                            CargarTipoCuenta();
                            ColumnasCuenta();
                        }
                        else
                        {
                            XtraMessageBox.Show("No se pudo actualizar registro", "Tipo Cuenta", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        }
                    }
                    else
                    {
                        XtraMessageBox.Show("Registro no válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    } 
                }
            }
        }

        private void btnEliminarTipoCuenta_Click(object sender, EventArgs e)
        {
            int pId = 0;
            string Desc = "";
            TipoCuenta Cuenta = new TipoCuenta();

            if (viewTipoCuenta.RowCount > 0)
            {
                pId = Convert.ToInt32(viewTipoCuenta.GetFocusedDataRow()["id"]);
                Desc = (string)viewTipoCuenta.GetFocusedDataRow()["nombre"];
                if (Cuenta.ExisteCodigo(pId))
                {
                    if (Cuenta.RegistroUsado(pId))
                    { lblMsgTipoCuenta.Visible = true; lblMsgTipoCuenta.Text = "No puedes eliminar este registro porque está siendo usado por un trabajador."; return; }

                    lblMsgTipoCuenta.Visible = false;

                    DialogResult Advertencia = XtraMessageBox.Show($"¿Estás seguro de eliminar el registro {Desc}?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (Advertencia == DialogResult.Yes)
                    {
                        if (Cuenta.EliminarCuenta(pId, Desc))
                        {
                            XtraMessageBox.Show($"Tipo de cuenta {Desc} elimnado correctamente", "Tipo Cuenta", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            fnSistema.spllenaGridView(gridTipoCuenta, SqlTipoCuenta);
                            fnSistema.spOpcionesGrilla(viewTipoCuenta);
                            CargarTipoCuenta();
                            ColumnasCuenta();
                        }
                        else
                        {
                            XtraMessageBox.Show("No se pudo eliminar cuenta", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        }
                    }

                }
            }
            else
            {
                XtraMessageBox.Show("Registro no válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void gridTipoCuenta_Click(object sender, EventArgs e)
        {
            if (viewTipoCuenta.RowCount > 0)
            {
              
                CargarTipoCuenta();
            }
        }

        private void gridTipoCuenta_KeyUp(object sender, KeyEventArgs e)
        {
            if (viewTipoCuenta.RowCount > 0)
            {
                
                CargarTipoCuenta();
            }
        }

        private void btnSalirTipoCuenta_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void txtCodTipoCuenta_KeyPress(object sender, KeyPressEventArgs e)
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

        private void btnNuevoSin_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            if (op.Cancela == false)
            {
                //LIMPIAMOS CAMPOS
                LimpiarSind();

                op.Cancela = true;
                op.SetButtonProperties(btnNuevoSin, 2);
            }
            else
            {
                //CARGAMOS datos
                CargaSind();

                op.Cancela = false;
                op.SetButtonProperties(btnNuevoSin, 1);
            }
        }

        private void btnGuardarSin_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            Sindicato sin = new Sindicato();

            //INGRESO
            if (UpdateSindicato == false)
            {
                if (txtCodSin.Text == "")
                { XtraMessageBox.Show("Por favor ingresa un codigo válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtCodSin.Focus(); return; }

                if (fnSistema.IsNumeric(txtCodSin.Text) == false)
                { XtraMessageBox.Show("Por favor ingresa un código válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtCodSin.Focus(); return; }
                
                if (sin.Existe(Convert.ToInt32(txtCodSin.Text)))
                { XtraMessageBox.Show("Código ingresado ya existe", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtCodSin.Focus(); return; }

                //INGRESAMOS NUEVO REGISTRO
                sin.Nuevo(Convert.ToInt32(txtCodSin.Text), txtDescSin.Text);

                fnSistema.spllenaGridView(gridSindicato, SqlSindicato);
                fnSistema.spOpcionesGrilla(viewSindicato);
                CargaSind();
                op.Cancela = false;
                op.SetButtonProperties(btnNuevoSin, 1);
            }
            //MODIFICAR
            else
            {
                if (viewSindicato.RowCount == 0)
                { XtraMessageBox.Show("Registro no válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                if (viewSindicato.RowCount > 0)
                {
                    //Actualizamos registro.
                    sin.Actualizar(Convert.ToInt32(viewSindicato.GetFocusedDataRow()["id"]), txtDescSin.Text);

                    fnSistema.spllenaGridView(gridSindicato, SqlSindicato);
                    fnSistema.spOpcionesGrilla(viewSindicato);
                    CargaSind();
                    op.Cancela = false;
                    op.SetButtonProperties(btnNuevoSin, 1);
                }
            }


        }

        private void btnEliminarSin_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            Sindicato sin = new Sindicato();

            if (viewSindicato.RowCount > 0)
            {
                if (sin.RegistroUsado(Convert.ToInt32(viewSindicato.GetFocusedDataRow()["id"])))
                { XtraMessageBox.Show("No puedes eliminar este registro porque está siendo usado por un trabajador", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                DialogResult advertencia = XtraMessageBox.Show("¿Realmente deseas eliminar este registro?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (advertencia == DialogResult.Yes)
                {
                    //ELIMINAR REGISTRO
                    sin.Eliminar(Convert.ToInt32(viewSindicato.GetFocusedDataRow()["id"]));

                    fnSistema.spllenaGridView(gridSindicato, SqlSindicato);
                    fnSistema.spOpcionesGrilla(viewSindicato);
                    CargaSind(); 
                }
            }
            else
            {
                XtraMessageBox.Show("Registro no válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void gridSindicato_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            if (viewSindicato.RowCount > 0)
            {
                CargaSind();
            }
        }

        private void gridSindicato_KeyUp(object sender, KeyEventArgs e)
        {
            Sesion.NuevaActividad();

            if (viewSindicato.RowCount > 0)
            {
                CargaSind();
            }
        }

        private void btnSalirSin_Click(object sender, EventArgs e)
        {
            if (viewSindicato.RowCount > 0)
            {
                if (CambiosSind())
                {
                    DialogResult Advertencia = XtraMessageBox.Show("¿Estás seguro de cerrar?", "Cambios Sin Guardar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (Advertencia == DialogResult.Yes)
                        Close();

                }
                else
                    Close();
            }
            else
                Close();
        }

        private void separatorControl10_Click(object sender, EventArgs e)
        {

        }
    }
}