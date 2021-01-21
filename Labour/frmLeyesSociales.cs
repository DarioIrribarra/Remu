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
using DevExpress.XtraTab;

namespace Labour
{
    public partial class frmLeyesSociales : DevExpress.XtraEditors.XtraForm
    {
        [DllImport("user32")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32")]
        static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        const int MF_BYCOMMAND = 0;
        const int MF_DISABLED = 2;
        const int SC_CLOSE = 0xF060;

        public ITrabajadorCombo opener { get; set; }

        //PARA VARIABLE OPCIONES USADOS EN FORMULARIO EMPLEADO
        private string opcion;

        private bool denegado = false;

        string SqlAfp = "SELECT id, nombre, porcFondo, porcAdmin, rut, porcOtro, claveExp, dato01, dato02 FROM afp WHERE id > 0 ORDER BY nombre";
        string sqlCajaPrevision = "SELECT id, nombre, rut, porcPension, porcSalud, porcAccidente, porcOtro, claveExp, dato01, dato02 FROM cajaPrevision WHERE id > 0 ORDER BY nombre";
        string sqlSalud = "SELECT id, nombre, rut, dato01, dato02 FROM isapre WHERE id > 0 ORDER BY nombre";

        Operacion op;       

        public frmLeyesSociales()
        {
            InitializeComponent();
        }

        //CONSTRUCTOR PARAMETRIZADO SOLO PARA CUANDO SE ACTIVE ESTE FORMULARIO ATRAVÉS DE FORMULARIO TRABJADOR
        public frmLeyesSociales(string opcion)
        {
            InitializeComponent();
            this.opcion = opcion;
        }

        private void frmLeyesSociales_Load(object sender, EventArgs e)
        {
            op = new Operacion();

            //SI NO ACCESO NO MOSTRAMOS PESTAÑAS
            OcultaTab();

            var sm = GetSystemMenu(Handle, false);
            EnableMenuItem(sm, SC_CLOSE, MF_BYCOMMAND | MF_DISABLED);

            //PARA LLAMADA DESDE FORMULARIO TRABAJADOR
            if (opcion != "") fnOpciones(opcion);

            //CODE
            tabAfp.TabControl.TabStop = false;
            tabMain.TabIndex = 0;           

            fnDefaultProperties(txtId, txtNombre, txtRut, txtFondo, txtAdmin, txtOtro, txtClave, txtdato1, txtdato2);
            btnNuevo.AllowFocus = false;
            btnEliminar.AllowFocus = false;
            btnGuardar.AllowFocus = false;
            separador1Afp.TabStop = false;
            separador2Afp.TabStop = false;
            gridPrevision.TabStop = false;

            //Cargar GRILLA
            fnSistema.spllenaGridView(gridPrevision, SqlAfp);
            fnSistema.spOpcionesGrilla(viewPrevision);
            fnColumnas(viewPrevision);
            fnCargarCamposPrev(txtId, txtNombre, txtRut, txtFondo, txtAdmin, txtOtro, txtdato1, txtdato2, txtClave,
            btnEliminar, viewPrevision, 0);
        }
        private void tabMain_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            //SABER CUAL ES LA PESTAÑA SELECCIONADA
            if (tabMain.SelectedTabPage.Equals(tabAfp))
            {                
                //CODE
                fnDefaultProperties(txtId, txtNombre, txtRut, txtFondo, txtAdmin, txtOtro, txtClave, txtdato1, txtdato2);
                btnNuevo.AllowFocus = false;
                btnEliminar.AllowFocus = false;
                btnGuardar.AllowFocus = false;
                separador1Afp.TabStop = false;
                separador2Afp.TabStop = false;
                gridPrevision.TabStop = false;

                //Cargar GRILLA
                fnSistema.spllenaGridView(gridPrevision, SqlAfp);
                fnSistema.spOpcionesGrilla(viewPrevision);
                fnColumnas(viewPrevision);
                fnCargarCamposPrev(txtId, txtNombre, txtRut, txtFondo, txtAdmin, txtOtro, txtdato1, txtdato2, txtClave,
                btnEliminar, viewPrevision, 0);

                this.Text = "Afp";

            }
            else if (tabMain.SelectedTabPage.Equals(tabCajaPrevision))
            {                

                //CODE
                fnDefaultPropertiesCaja(txtIdPrev, txtNombrePrev, txtRutPrev, txtPensionPrev, txtSaludPrev,
                    txtOtroPrev, txtClavePrev, txtDato1Prev, txtDato2Prev, txtAccidente);
                btnNuevoPrev.AllowFocus = false;
                btnGuardarPrev.AllowFocus = false;
                btnEliminarPrev.AllowFocus = false;
                gridPrev.TabStop = false;
                separador1Prevision.TabStop = false;


                fnSistema.spllenaGridView(gridPrev, sqlCajaPrevision);
                fnSistema.spOpcionesGrilla(viewPrev);
                fnColumnasCaja(viewPrev);
                fnCargarCamposCaja(txtIdPrev, txtNombrePrev, txtRutPrev, txtPensionPrev, txtSaludPrev, txtOtroPrev, txtDato1Prev,
                    txtDato2Prev, txtClavePrev, txtAccidente, btnEliminarPrev, viewPrev, 0);

                this.Text = "Caja de Prevision";


            }
            else if (tabMain.SelectedTabPage.Equals(tabIsapre))
            {              

                //CODE      
                fnPropiedadesIsapre();
                fnSistema.spllenaGridView(gridIsapre, sqlSalud);
                fnSistema.spOpcionesGrilla(viewIsapre);
                fnColumnasGrilla();
                fnCargarIsapre(0);

                this.Text = "Salud";
            }
        }


        private void fnOpciones(string opcion)
        {
            switch (opcion)
            {
                case "salud":
                    tabAfp.PageVisible = false;
                    tabCajaPrevision.PageVisible = false;
                    break;
                case "caja":
                    tabIsapre.PageVisible = false;
                    tabAfp.PageVisible = false;
                    break;
                case "afp":
                    tabIsapre.PageVisible = false;
                    tabCajaPrevision.PageVisible = false;
                    break;
            }
        }

        #region "AFP"
        //INSERT NUEVO REGISTRO TABLA AFP
        private bool fnNuevoAfp(TextEdit pId, TextEdit pNombre, TextEdit pFondo, TextEdit pAdmin, TextEdit pOtro,
            TextEdit pClave, string pRut, TextEdit pDato1, TextEdit pDato2, string pSql)
        {
            //SQL
            string sql = "INSERT INTO AFP(id, nombre, dato01, dato02, porcFondo, porcAdmin, rut, porcOtro, claveExp)" +
                " VALUES(@pId, @pNombre, @pDato1, @pDato2, @pFondo, @pAdmin, @pRut, @pOtro, @pClave)";
            SqlCommand cmd;
            int res = 0;
            decimal otro = 0;
            if (pOtro.Text != "")
            {

                //validamos la cadena
                bool valido = fnValidarPorcentajes(pOtro.Text);
                //SI RETORNA FALSE NO ES VALIDO
                if (valido == false) { XtraMessageBox.Show("Campo Otro no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return false; }
                //CASO CONTRARIO GUARDARMOS EN VARIALBLE OTRO
                otro = decimal.Parse(pOtro.Text);
            }

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(pSql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pId", pId.Text));
                        cmd.Parameters.Add(new SqlParameter("@pNombre", pNombre.Text));
                        cmd.Parameters.Add(new SqlParameter("@pDato1", pDato1.Text));
                        cmd.Parameters.Add(new SqlParameter("@pDato2", pDato2.Text));
                        cmd.Parameters.Add(new SqlParameter("@pFondo", decimal.Parse(pFondo.Text)));
                        cmd.Parameters.Add(new SqlParameter("@pAdmin", decimal.Parse(pAdmin.Text)));
                        cmd.Parameters.Add(new SqlParameter("@pRut", pRut));
                        cmd.Parameters.Add(new SqlParameter("@pOtro", otro));
                        cmd.Parameters.Add(new SqlParameter("@pClave", pClave.Text));
                        //EJCUTAR CONSULTA
                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            XtraMessageBox.Show("Registro Guardado", "Guardar", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            return true;

                        }
                        else
                        {
                            XtraMessageBox.Show("Error al intentar Guardar", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

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

        //METODO PARA MODIFICAR REGISTRO AFP
        private bool fnModificarAfp(TextEdit pId, TextEdit pNombre, TextEdit pFondo, TextEdit pAdmin, TextEdit pOtro,
            TextEdit pClave, string pRut, TextEdit pDato1, TextEdit pDato2, string pSql)
        {
            //SQL
            string sql = "UPDATE afp SET nombre=@pNombre, dato01=@pDato1, dato02=@pDato2, porcFondo=@pFondo," +
                "porcAdmin=@pAdmin, rut=@pRut, porcOtro=@pOtro, claveExp=@pClave WHERE Id=@pId";
            SqlCommand cmd;
            int res = 0;
            decimal otro = 0;
            if (pOtro.Text != "")
            {

                //validamos la cadena
                bool valido = fnValidarPorcentajes(pOtro.Text);
                //SI RETORNA FALSE NO ES VALIDO
                if (valido == false) { XtraMessageBox.Show("Campo Otro no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return false; }
                //CASO CONTRARIO GUARDARMOS EN VARIALBLE OTRO
                otro = decimal.Parse(pOtro.Text);
            }

            //PARA LOG, EXTRAEMOS DATOS DESDE BASE ANTES DE QUE SE REALIZE EL UPDATE (PARA COMPARAR)
            Hashtable tablaAfp = new Hashtable();
            tablaAfp = DatosAfp(int.Parse(pId.Text));
           
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(pSql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pId", pId.Text));
                        cmd.Parameters.Add(new SqlParameter("@pNombre", pNombre.Text));
                        cmd.Parameters.Add(new SqlParameter("@pDato1", pDato1.Text));
                        cmd.Parameters.Add(new SqlParameter("@pDato2", pDato2.Text));
                        cmd.Parameters.Add(new SqlParameter("@pFondo", decimal.Parse(pFondo.Text)));
                        cmd.Parameters.Add(new SqlParameter("@pAdmin", decimal.Parse(pAdmin.Text)));
                        cmd.Parameters.Add(new SqlParameter("@pRut", pRut));
                        cmd.Parameters.Add(new SqlParameter("@pOtro", otro));
                        cmd.Parameters.Add(new SqlParameter("@pClave", pClave.Text));
                        //EJECUTAMOS CONSULTA
                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            XtraMessageBox.Show("Registro Actualizado", "Actualizar", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            //SI LA ACTUALIZACION ES CORRECTA REGISTRAMOS EN LOG
                            CambiaValorAfp(pNombre.Text, pDato1.Text, pDato2.Text, decimal.Parse(pFondo.Text), decimal.Parse(pAdmin.Text), pRut, otro, pClave.Text, tablaAfp);

                            return true;
                        }
                        else
                        {
                            XtraMessageBox.Show("Error al intentar Actualizar", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        }
                    }
                    //LIBERAR RECURSOS
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                }
                else
                {
                    XtraMessageBox.Show("No hay conexion a Base de datos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
            return false;
        }

        //METODO PARA ELIMINAR REGISTRO AFP
        //PARAMETRO DE ENTRADA : ID
        private bool fnEliminarAfp(int pId, string pSql)
        {
            //Sql
            String sql = "DELETE FROM afp WHERE id=@pId";
            SqlCommand cmd;
            int res = 0;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(pSql, fnSistema.sqlConn))
                    {
                        cmd.Parameters.Add(new SqlParameter("@pId", pId));
                        //EJECUTAMOS CONSULTA
                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            XtraMessageBox.Show("Registro Eliminado", "Eliminar", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            return true;
                        }
                        else
                        {
                            XtraMessageBox.Show("Error al intentar Elmiminar", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    //LIBERAMOS RECURSOS
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

        //COLUMNAS GRILLA
        private void fnColumnas(DevExpress.XtraGrid.Views.Grid.GridView pGrid)
        {
            pGrid.Columns[0].Visible = true;
            pGrid.Columns[0].Width = 50;
            pGrid.Columns[0].Caption = "N°";

            pGrid.Columns[1].Caption = "Nombre";
            pGrid.Columns[1].Width = 200;
            pGrid.Columns[1].AppearanceCell.FontStyleDelta = FontStyle.Bold;
            pGrid.Columns[2].Caption = "Fondo";
            pGrid.Columns[3].Caption = "Administrado";

            pGrid.Columns[4].Caption = "Rut";
            pGrid.Columns[4].Width = 100;
            pGrid.Columns[4].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            pGrid.Columns[4].DisplayFormat.FormatString = "Rut";
            pGrid.Columns[4].DisplayFormat.Format = new FormatCustom();

            pGrid.Columns[5].Caption = "Otro";
            pGrid.Columns[6].Caption = "Clave";
            pGrid.Columns[7].Caption = "Dato1";
            pGrid.Columns[8].Caption = "Dato2";

        }

        //CARGAR CAMPOS DESDE GRILLA
        private void fnCargarCamposPrev(TextEdit pId, TextEdit pNombre, TextEdit pRut, TextEdit pFondo, TextEdit pAdmin,
            TextEdit pOtro, TextEdit pDato1, TextEdit pDato2, TextEdit pClave, SimpleButton pButton,
            DevExpress.XtraGrid.Views.Grid.GridView pGrid, int? pos = -1)
        {
            //CARGAR TEXTBOX
            if (pGrid.RowCount > 0)
            {
                if (pos == 0) { pGrid.FocusedRowHandle = 0; }
                //HABILITAMOS EL BOTON
                pButton.Enabled = true;
                pId.ReadOnly = true;
                pId.Text = pGrid.GetFocusedDataRow()["id"].ToString();
                pNombre.Text = pGrid.GetFocusedDataRow()["nombre"].ToString();
                //DESENMASCARAR RUT
                string rut = fnSistema.fnDesenmascararRut(pGrid.GetFocusedDataRow()["rut"].ToString());
                pRut.Text = fnSistema.fFormatearRut2(rut);
                pFondo.Text = pGrid.GetFocusedDataRow()["porcFondo"].ToString();
                pAdmin.Text = pGrid.GetFocusedDataRow()["porcAdmin"].ToString();
                pOtro.Text = pGrid.GetFocusedDataRow()["porcOtro"].ToString();
                pDato1.Text = pGrid.GetFocusedDataRow()["dato01"].ToString();
                pClave.Text = pGrid.GetFocusedDataRow()["claveExp"].ToString();
                pDato2.Text = pGrid.GetFocusedDataRow()["dato02"].ToString();

                //RESET BOTON NUEVO (CAPTION)
                op.Cancela = false;
                op.SetButtonProperties(btnNuevo, 1);
                lblrutAfp.Visible = false;
            }
        }

        //VERIFICAR SI EXISTE RUT EN BASE DE DATOS
        private bool fnVerificarId(int pId, string pSql)
        {
            //sql
            string sql = "SELECT * FROM afp WHERE id=@pId";
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
                    //Liberar recursos
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                }
                else
                {
                    XtraMessageBox.Show("No hay conexion con la base de datos", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
            return false;
        }

        //LIMPIAR CAMPOS
        private void fnLimpiar(TextEdit pId, TextEdit pNombre, TextEdit pRut, TextEdit pAdmin, TextEdit pFondo,
            TextEdit pOtro, TextEdit pClave, TextEdit pDato1, TextEdit pDato2, SimpleButton pButton, string pTableName)
        {
            pId.ReadOnly = false;
            //pId.Text = "";
            pNombre.Text = "";
            pNombre.Focus();
            pRut.Text = "";
            pAdmin.Text = "0";
            pFondo.Text = "10";
            pOtro.Text = "0";
            pClave.Text = "";
            pDato1.Text = "";
            pDato2.Text = "";

            //DESHABILITAR BOTON ELIMINAR
            pButton.Enabled = false;

            //OBTENER NUMERO ID DISPONIBLE
            int num = fnItemDisponibles(pTableName);
            txtId.Text = num.ToString();

        }

        //VALIDAR CAMPOS DE %
        private bool fnValidarPorcentajes(string pCadena)
        {
            //recorrer cadena y buscar los '.' y comas
            int c = 0;
            for (int i = 0; i < pCadena.Length; i++)
            {
                if (pCadena[i].ToString() == ",") c++;
            }
            //SI HAY MAS DE UNA COMA NO ES VALIDO                      
            if (c > 1) return false;
            if (pCadena.Length == 1 && pCadena == ",") return false;

            //SEPARAMOS LA CADENA POR COMA
            char[] separador = new char[] { ',' };
            string[] cadenas = pCadena.Split(separador);
            //123,2 --> NO VALIDO
            if (cadenas[0].Length > 2) return false;
            //100 --> valido            
            //en otro caso es valido y retornamos true
            return true;
        }

        //PROPIEDADES POR DEFECTO
        private void fnDefaultProperties(TextEdit pId, TextEdit pNombre, TextEdit pRut, TextEdit pFondo, TextEdit pAdmin,
            TextEdit pOtro, TextEdit pClave, TextEdit pDato1, TextEdit pDato2)
        {
            pId.Properties.MaxLength = 5;
            pNombre.Properties.MaxLength = 30;
            pRut.Properties.MaxLength = 9;
            lblrutAfp.Visible = false;
            pFondo.Properties.MaxLength = 4;
            pAdmin.Properties.MaxLength = 4;
            pOtro.Properties.MaxLength = 4;
            pClave.Properties.MaxLength = 20;
            pDato1.Properties.MaxLength = 10;
            pDato2.Properties.MaxLength = 10;
            pNombre.Properties.Appearance.FontStyleDelta = FontStyle.Bold;
        }

        //VERIFICAR SI HAY CAMBIOS SIN GUARDAR
        private bool fnCambiosPrev(TextEdit pId, TextEdit pNombre, TextEdit pRut, TextEdit pFondo, TextEdit pAdmin,
            TextEdit pOtro, TextEdit pClave, TextEdit pDato1, TextEdit pDato2, string pSql)
        {
            //EJEMPLO SQL
            //SELECT id, nombre, rut, fondo, admin, otro, clave, dato01, dato02 FROM tabla WHERE id=@Id
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(pSql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pId", pId.Text));
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //COMPARAMOS
                                //ANTES DE COMPARAR EL RUT DEBEMOS DESENMASCARAR Y QUITAR LOS PUNTOS Y GUION

                                string rut = fnSistema.fnExtraerCaracteres(pRut.Text);
                                rut = fnSistema.fEnmascaraRut(rut);

                                if (rd["nombre"].ToString() != pNombre.Text) { return true; }
                                if (rd["rut"].ToString() != rut) { return true; }
                                if (rd["porcFondo"].ToString() != pFondo.Text) { return true; }
                                if (rd["porcAdmin"].ToString() != pAdmin.Text) { return true; }
                                if (rd["porcOtro"].ToString() != pOtro.Text) { return true; }
                                if (rd["claveExp"].ToString() != pClave.Text) { return true; }
                                if (rd["dato01"].ToString() != pDato1.Text) { return true; }
                                if (rd["dato02"].ToString() != pDato2.Text) { return true; }
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

        //VERIFICAR ANTES DE ELIMINAR UN REGISTRO AFP SI ESTE ESTA SIENDO USADO POR UN EMPLEADO
        private bool fnAfpEmpleado(string pId)
        {
            //CONSULTA SQL
            string sql = "SELECT afp FROM trabajador WHERE afp=@pId";
            SqlCommand cmd;
            SqlDataReader rd;

            bool utilizado = false;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pId", pId));

                        //EJECUTAR CONSULTA
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //SI ENCUENTRA FILAS ES PORQUE ESE CODIGO AF ESTA SIENDO USADO POR UN EMPLEADO
                            utilizado = true;
                        }
                        else
                        {
                            utilizado = false;
                        }
                    }
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

            return utilizado;
        }

        //VALIDAR QUE NOMBRE DE AFP NO EXISTA
        private bool ExistePrevision(string nombre)
        {
            bool existe = false;
            string sql = "SELECT nombre FROM Afp where nombre=@nombre";
            SqlCommand cmd;
            SqlDataReader rd;

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@nombre", nombre));
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
                    fnSistema.sqlConn.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return existe;
        }

        //VALIDAR QUE RUT A INGRESAR NO EXISTA EN BD
        private bool ExisteRutPrevision(string pRut)
        {
            bool existe = false;
            string sql = "SELECT rut FROM afp WHERE rut =@pRut";
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
                            //SI HAY REGISTROS ES PORQUE EL RUT YA EXISTE
                            existe = true;
                        }
                        else
                        {
                            existe = false;
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
        
        #region "LOG AFP"
        //VERIFICAR SI SE CAMBIO ALGUN REGISTRO AFP
        //PARAMETROS DE ENTRADA:ID AFP
        private void CambiaValorAfp(string pNombre, string dato01, string dato02, decimal porcfondo, 
            decimal porcAdmin, string rut, decimal porcOtro, string claveExp, Hashtable Tabla)
        {
            try
            {                   
                 //COMPARAR VALOR EN BASE DE DATOS CON EL VALOR A CAMBIAR
                 //SI SON DISTINTAS REGISTRAMOS CAMBIO EN LOG
                 if ((string)Tabla["nombre"] != pNombre)
                 {                    
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA NOMBRE AFP " + (string)Tabla["nombre"], "AFP", (string)Tabla["nombre"], pNombre, "MODIFICAR");
                    log.Log();
                 }
                 if ((string)Tabla["dato01"] != dato01)
                 {
                    
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA DATO 01 AFP " + (string)Tabla["nombre"], "AFP", (string)Tabla["dato01"], dato01, "MODIFICAR");
                    log.Log();
                 }
                 if ((string)Tabla["dato02"] != dato02)
                 {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA DATO 02 AFP " + (string)Tabla["nombre"], "AFP", (string)Tabla["dato02"], dato02, "MODIFICAR");
                    log.Log();
                 }
                 if ((decimal)Tabla["porcFondo"] != porcfondo)
                 {                 
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA PORCENTAJE FONDO AFP " + (string)Tabla["nombre"], "AFP", (decimal)Tabla["porcFondo"] + "", porcfondo + "", "MODIFICAR");
                    log.Log();
                 }
                 if ((decimal)Tabla["porcAdmin"] != porcAdmin)
                 {          
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA PORCENTAJE ADMINISTRADO AFP "+ (string)Tabla["nombre"], "AFP", (decimal)Tabla["porcAdmin"] + "", porcAdmin + "", "MODIFICAR");
                    log.Log();
                 }
                 if ((string)Tabla["rut"] != rut)
                 {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA RUT AFP " + (string)Tabla["nombre"], "AFP", (string)Tabla["rut"], rut, "MODIFICAR");
                    log.Log();
                 }
                 if ((decimal)Tabla["porcOtro"] != porcOtro)
                 {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA CAMPO PORCOTRO EN AFP " + (string)Tabla["nombre"], "AFP", (decimal)Tabla["porcOtro"] + "", porcOtro + "", "MODIFICAR");
                    log.Log();
                 }
                 if ((string)Tabla["claveExp"] != claveExp)
                 {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA CLAVE EXP AFP " + (string)Tabla["nombre"], "AFP", (string)Tabla["claveExp"], claveExp, "MODIFICAR");
                    log.Log();
                 }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        //LISTADO HASH TABLE CON LOS DATOS DESDE BD
        private Hashtable DatosAfp(int pId)
        {
            string sql = "SELECT nombre, dato01, dato02, porcFondo, porcAdmin, rut, porcOtro, claveExp" +
                " FROM AFP  WHERE id=@pId";
            SqlCommand cmd;
            SqlDataReader rd;
            Hashtable tabla = new Hashtable();
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
                                //LLENAMOS TABLA HASH
                                tabla.Add("nombre", (string)rd["nombre"]);
                                tabla.Add("dato01", (string)rd["dato01"]);
                                tabla.Add("dato02", (string)rd["dato02"]);
                                tabla.Add("porcFondo", (decimal)rd["porcFondo"]);
                                tabla.Add("porcAdmin", (decimal)rd["porcAdmin"]);
                                tabla.Add("rut", (string)rd["rut"]);
                                tabla.Add("porcOtro", (decimal)rd["porcOtro"]);
                                tabla.Add("claveExp", (string)rd["claveExp"]);
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
            //RETORNAR TABLA HASH
            return tabla;
        }
        #endregion

        #endregion

        #region "DATOS CAJA PREVISION"
        //INSERT NUEVO REGISTRO TABLA CAJA PREVISION
        private bool fnNuevoCaja(TextEdit pId, TextEdit pNombre, TextEdit pPension, TextEdit pSalud, TextEdit pOtro,
            TextEdit pClave, string pRut, TextEdit pDato1, TextEdit pDato2, TextEdit pAccidente,  string pSql)
        {
            //SQL
            string sql = "INSERT INTO cajaPrevision(id, nombre, dato01, dato02, porcPension, porcSalud, rut, porcOtro, claveExp, porcAccidente)" +
                " VALUES(@pId, @pNombre, @pDato1, @pDato2, @pPension, @pSalud, @pRut, @pOtro, @pClave, @pAccidente)";

            SqlCommand cmd;
            int res = 0;
            decimal otro = 0;
            if (pOtro.Text != "")
            {

                //validamos la cadena
                bool valido = fnValidarPorcentajes(pOtro.Text);
                //SI RETORNA FALSE NO ES VALIDO
                if (valido == false) { XtraMessageBox.Show("Campo Otro no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return false; }
                //CASO CONTRARIO GUARDARMOS EN VARIALBLE OTRO
                otro = decimal.Parse(pOtro.Text);
            }

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(pSql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pId", pId.Text));
                        cmd.Parameters.Add(new SqlParameter("@pNombre", pNombre.Text));
                        cmd.Parameters.Add(new SqlParameter("@pDato1", pDato1.Text));
                        cmd.Parameters.Add(new SqlParameter("@pDato2", pDato2.Text));
                        cmd.Parameters.Add(new SqlParameter("@pPension", decimal.Parse(pPension.Text)));
                        cmd.Parameters.Add(new SqlParameter("@pSalud", decimal.Parse(pSalud.Text)));
                        cmd.Parameters.Add(new SqlParameter("@pRut", pRut));
                        cmd.Parameters.Add(new SqlParameter("@pOtro", otro));
                        cmd.Parameters.Add(new SqlParameter("@pClave", pClave.Text));
                        cmd.Parameters.Add(new SqlParameter("@pAccidente", decimal.Parse(pAccidente.Text)));
                        //EJCUTAR CONSULTA
                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            XtraMessageBox.Show("Registro Guardado", "Guardar", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            return true;

                        }
                        else
                        {
                            XtraMessageBox.Show("Error al intentar Guardar", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

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

        //METODO PARA MODIFICAR REGISTRO AFP
        private bool fnModificarCaja(TextEdit pId, TextEdit pNombre, TextEdit pPension, TextEdit pSalud, TextEdit pOtro,
            TextEdit pClave, string pRut, TextEdit pDato1, TextEdit pDato2, TextEdit pAccidente,  string pSql)
        {
            //SQL
            string sql = "UPDATE cajaPrevision SET nombre=@pNombre, dato01=@pDato1, dato02=@pDato2, porcPension=@pPension," +
                "porcSalud=@pSalud, rut=@pRut, porcOtro=@pOtro, claveExp=@pClave, porcAccidente=@pAccidente WHERE Id=@pId";
            SqlCommand cmd;
            int res = 0;
            decimal otro = 0;
            if (pOtro.Text != "")
            {

                //validamos la cadena
                bool valido = fnValidarPorcentajes(pOtro.Text);
                //SI RETORNA FALSE NO ES VALIDO
                if (valido == false) { XtraMessageBox.Show("Campo Otro no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return false; }
                //CASO CONTRARIO GUARDARMOS EN VARIALBLE OTRO
                otro = decimal.Parse(pOtro.Text);
            }

            //TABLA HASH PARA LOG
            Hashtable tablaCaja = new Hashtable();
            tablaCaja = DatosCaja(int.Parse(pId.Text));

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(pSql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pId", pId.Text));
                        cmd.Parameters.Add(new SqlParameter("@pNombre", pNombre.Text));
                        cmd.Parameters.Add(new SqlParameter("@pDato1", pDato1.Text));
                        cmd.Parameters.Add(new SqlParameter("@pDato2", pDato2.Text));
                        cmd.Parameters.Add(new SqlParameter("@pPension", decimal.Parse(pPension.Text)));
                        cmd.Parameters.Add(new SqlParameter("@pSalud", decimal.Parse(pSalud.Text)));
                        cmd.Parameters.Add(new SqlParameter("@pRut", pRut));
                        cmd.Parameters.Add(new SqlParameter("@pOtro", otro));
                        cmd.Parameters.Add(new SqlParameter("@pClave", pClave.Text));
                        cmd.Parameters.Add(new SqlParameter("@pAccidente", decimal.Parse(pAccidente.Text)));
                        //EJECUTAMOS CONSULTA 
                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            XtraMessageBox.Show("Registro Actualizado", "Actualizar", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            //COMPARAMOS VALORES Y GUARDAMOS SI ES QUE HAY CAMBIOS EN LOGREGISTRO
                            CambiaValorCaja(tablaCaja, pNombre.Text, pDato1.Text, pDato2.Text, decimal.Parse(pPension.Text), decimal.Parse(pSalud.Text), pRut, otro, pClave.Text, decimal.Parse(pAccidente.Text));

                            return true;
                        }
                        else
                        {
                            XtraMessageBox.Show("Error al intentar Actualizar", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        }
                    }
                    //LIBERAR RECURSOS
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                }
                else
                {
                    XtraMessageBox.Show("No hay conexion a Base de datos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
            return false;
        }

        //METODO PARA ELIMINAR REGISTRO AFP
        //PARAMETRO DE ENTRADA : ID
        private bool fnEliminarCaja(int pId, string pSql)
        {
            //Sql
            String sql = "DELETE FROM cajaPrevision WHERE id=@pId";
            SqlCommand cmd;
            int res = 0;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(pSql, fnSistema.sqlConn))
                    {
                        cmd.Parameters.Add(new SqlParameter("@pId", pId));
                        //EJECUTAMOS CONSULTA
                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            XtraMessageBox.Show("Registro Eliminado", "Eliminar", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            return true;
                        }
                        else
                        {
                            XtraMessageBox.Show("Error al intentar Elmiminar", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    //LIBERAMOS RECURSOS
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

        //COLUMNAS GRILLA
        private void fnColumnasCaja(DevExpress.XtraGrid.Views.Grid.GridView pGrid)
        {
            //fnSistema.spllenaGridView(gridPrev, "SELECT id, nombre, rut, porcPension, porcSalud, " +
            //"porcAccidente, porcOtro, claveExp, dato01, dato02 FROM cajaPrevision ORDER BY nombre");

            //OCULTAR CAMPO ID
            pGrid.Columns[0].Visible = true;
            pGrid.Columns[0].Width = 50;
            pGrid.Columns[0].Caption = "N°";

            pGrid.Columns[1].Caption = "Nombre";
            pGrid.Columns[1].Width = 200;
            pGrid.Columns[1].AppearanceCell.FontStyleDelta = FontStyle.Bold;

            //FORMATEAMOS EL CAMPO RUT DE ACUERDO A INTERFAZ CREADA PARA TAL EFECTO
            pGrid.Columns[2].Caption = "Rut";
            pGrid.Columns[2].Width = 100;
            pGrid.Columns[2].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            pGrid.Columns[2].DisplayFormat.FormatString = "Rut";
            pGrid.Columns[2].DisplayFormat.Format = new FormatCustom();

            pGrid.Columns[3].Caption = "IPS";

            pGrid.Columns[4].Caption = "Fonasa";           

            pGrid.Columns[5].Caption = "Accidentes";
            pGrid.Columns[6].Caption = "Otro";
            pGrid.Columns[7].Caption = "Clave";
            pGrid.Columns[8].Caption = "dato1";
            pGrid.Columns[9].Caption = "dato2";

        }

        //CARGAR CAMPOS DESDE GRILLA
        private void fnCargarCamposCaja(TextEdit pId, TextEdit pNombre, TextEdit pRut, TextEdit pPension, TextEdit pSalud,
            TextEdit pOtro, TextEdit pDato1, TextEdit pDato2, TextEdit pClave, TextEdit pAccidente, SimpleButton pButton,
            DevExpress.XtraGrid.Views.Grid.GridView pGrid, int? pos = -1)
        {
            //CARGAR TEXTBOX
            if (pGrid.RowCount > 0)
            {
                if (pos == 0) { pGrid.FocusedRowHandle = 0; }
                //HABILITAMOS EL BOTON
                pButton.Enabled = true;
                pId.ReadOnly = true;
                pId.Text = pGrid.GetFocusedDataRow()["id"].ToString();
                pNombre.Text = pGrid.GetFocusedDataRow()["nombre"].ToString();
                //DESENMASCARAR RUT Y FORMATEAR
                string rut = fnSistema.fnDesenmascararRut(pGrid.GetFocusedDataRow()["rut"].ToString());
                rut = fnSistema.fFormatearRut2(rut);
                pRut.Text = rut;
                pPension.Text = pGrid.GetFocusedDataRow()["porcPension"].ToString();
                pSalud.Text = pGrid.GetFocusedDataRow()["porcSalud"].ToString();
                pOtro.Text = pGrid.GetFocusedDataRow()["porcOtro"].ToString();
                pDato1.Text = pGrid.GetFocusedDataRow()["dato01"].ToString();
                pClave.Text = pGrid.GetFocusedDataRow()["claveExp"].ToString();
                pDato2.Text = pGrid.GetFocusedDataRow()["dato02"].ToString();
                pAccidente.Text = pGrid.GetFocusedDataRow()["porcAccidente"].ToString();

                //RESET BOTON NUEVO
                op.Cancela = false;
                op.SetButtonProperties(btnNuevoPrev, 1);

                lblrutPrevision.Visible = false;
            }
        }

        //VERIFICAR SI EXISTE RUT EN BASE DE DATOS
        private bool fnVerificarIdCaja(int pId, string pSql)
        {
            //sql
            string sql = "SELECT * FROM cajaPrevision WHERE id=@pId";
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
                    //Liberar recursos
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                }
                else
                {
                    XtraMessageBox.Show("No hay conexion con la base de datos", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
            return false;
        }

        //LIMPIAR CAMPOS
        private void fnLimpiarCaja(TextEdit pId, TextEdit pNombre, TextEdit pRut, TextEdit pSalud, TextEdit pPension,
            TextEdit pOtro, TextEdit pClave, TextEdit pDato1, TextEdit pDato2,TextEdit pAccidente, SimpleButton pButton)
        {
            pId.ReadOnly = false;         
            //pId.Text = "";
            pNombre.Text = "";
            pNombre.Focus();
            pRut.Text = "";
            pSalud.Text = "7";
            pPension.Text = "0";
            pOtro.Text = "0";
            pClave.Text = "";
            pDato1.Text = "";
            pDato2.Text = "";
            pAccidente.Text = "0";
            //DESHABILITAR BOTON ELIMINAR
            pButton.Enabled = false;

            //OBTENER ID DISPONIBLE 
            int num = fnItemDisponibles("cajaprevision");
            pId.Text = num.ToString();

        }

        //VALIDAR CAMPOS DE %
       /* private bool fnValidarPorcentajes(string pCadena)
        {
            //recorrer cadena y buscar los '.' y comas
            int c = 0;
            for (int i = 0; i < pCadena.Length; i++)
            {
                if (pCadena[i].ToString() == ",") c++;
            }
            //SI HAY MAS DE UNA COMA NO ES VALIDO                      
            if (c > 1) return false;
            if (pCadena.Length == 1 && pCadena == ",") return false;

            //SEPARAMOS LA CADENA POR COMA
            char[] separador = new char[] { ',' };
            string[] cadenas = pCadena.Split(separador);
            //123,2 --> NO VALIDO
            if (cadenas[0].Length > 2) return false;
            //100 --> valido            
            //en otro caso es valido y retornamos true
            return true;
        }*/

        //PROPIEDADES POR DEFECTO
        private void fnDefaultPropertiesCaja(TextEdit pId, TextEdit pNombre, TextEdit pRut, TextEdit pPension, TextEdit pSalud,
            TextEdit pOtro, TextEdit pClave, TextEdit pDato1, TextEdit pDato2, TextEdit pAccidente)
        {
            pId.Properties.MaxLength = 5;
            pNombre.Properties.MaxLength = 30;
            pRut.Properties.MaxLength = 9;
            lblrutPrevision.Visible = false;
            pPension.Properties.MaxLength = 5;
            pSalud.Properties.MaxLength = 5;
            pOtro.Properties.MaxLength = 5;
            pClave.Properties.MaxLength = 20;
            pDato1.Properties.MaxLength = 10;
            pDato2.Properties.MaxLength = 10;
            pNombre.Properties.Appearance.FontStyleDelta = FontStyle.Bold;
            pAccidente.Properties.MaxLength = 4;
        }

        //VERIFICAR SI HAY CAMBIOS SIN GUARDAR
        private bool fnCambiosPrevCaja(TextEdit pId, TextEdit pNombre, TextEdit pRut, TextEdit pPension, TextEdit pSalud,
            TextEdit pOtro, TextEdit pClave, TextEdit pDato1, TextEdit pDato2, TextEdit pAccidente, string pSql)
        {
            //EJEMPLO SQL
            //SELECT id, nombre, rut, fondo, admin, otro, clave, dato01, dato02 FROM tabla WHERE id=@Id
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(pSql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pId", pId.Text));
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //COMPARAMOS
                                //ANTES DE COMPARAR RUT DEBEMOS QUITAR . Y - Y ENMASCARAR                                
                                string rut = fnSistema.fnExtraerCaracteres(pRut.Text);
                                rut = fnSistema.fEnmascaraRut(rut);
                                if (rd["nombre"].ToString() != pNombre.Text) { return true; }
                                if (rd["rut"].ToString() != rut) { return true; }
                                if (rd["porcPension"].ToString() != pPension.Text) { return true; }
                                if (rd["porcSalud"].ToString() != pSalud.Text) { return true; }
                                if (rd["porcOtro"].ToString() != pOtro.Text) { return true; }
                                if (rd["claveExp"].ToString() != pClave.Text) { return true; }
                                if (rd["dato01"].ToString() != pDato1.Text) { return true; }
                                if (rd["dato02"].ToString() != pDato2.Text) { return true; }
                                if (rd["porcAccidente"].ToString() != pAccidente.Text) { return true; }
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

        //VERIFICAR SI ALGUN TRABAJADOR ESTA USANDO ALGUN REGISTRO DE LA TABLA CAJA PREVISION
        //RETORNA TRUE SI EL REGISTRO ESTA EN USO
        private bool fnCajaEmpleado(string pId)
        {
            //CONSULTAR SQL
            string sql = "SELECT cajaPrevision FROM trabajador WHERE cajaPrevision=@pId";
            SqlCommand cmd;
            SqlDataReader rd;

            bool utilizado = false;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pId", pId));

                        //EJECUTAR CONSULTA
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //SI ENCUENTRA REGISTROS ES PORQUE SE ESTA USANDO EL REGISTRO POR UN EMPLEADO
                            utilizado = true;
                        }
                        else
                        {
                            utilizado = false;
                        }
                    }
                    //LIBERAR RECURSOS
                    cmd.Dispose();
                    rd.Close();
                    fnSistema.sqlConn.Close();
                }
                else
                {
                    XtraMessageBox.Show("No hay conexion con la base de datos");
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return utilizado;
        }

        //VERIFICAR SI EL NOMBRE EXISTE EN BD
        private bool ExisteNombreCaja(string pNombre)
        {
            bool existe = false;
            string sql = "SELECT nombre FROM cajaprevision WHERE nombre=@pNombre";
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pNombre", pNombre));
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //SIGNIFICA QUE EL NOMBRE DE LA CAJA DE PREVISION YA EXISTE
                            existe = true;
                        }
                        else
                        {
                            existe = false;
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

        //EXISTE RUT CAJA 
        private bool ExisteRutCaja(string pRut)
        {
            string sql = "SELECT rut FROM cajaprevision WHERE rut=@pRut";
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
                        cmd.Parameters.Add(new SqlParameter("@pRut", pRut));
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //EXISTE RUT
                            existe = true;
                        }
                        else
                        {
                            existe = false;
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

        #region "LOG CAJA PREVISION"
        //VERIFICAR SI SE CAMBIO ALGUN VALOR
        private void CambiaValorCaja(Hashtable tabla, string pNombre, string dato01, string dato02, decimal porcPension,
            decimal porcSalud, string rut, decimal porcOtro, string claveExp, decimal porcAccidente)
        {
            if (tabla.Count > 0)
            {
                try
                {
                    //COMPARA CADA VALOR DE LA TABLA HASH CON EL NUEVO 
                    //SI SON DISTINTOS GUARDAMOS REGISTRO EN LOG
                    if ((string)tabla["nombre"] != pNombre)
                    {
                        logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA NOMBRE CAJA COMPENSACION " + (string)tabla["nombre"], "CAJAPREVISION", (string)tabla["nombre"], pNombre, "MODIFICAR");
                        log.Log();
                    }
                    if ((string)tabla["dato01"] != dato01)
                    {
                        logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA DATO 01 CAJA COMPENSACION " + (string)tabla["nombre"], "CAJAPREVISION", (string)tabla["dato01"], dato01, "MODIFICAR");
                        log.Log();
                    }
                    if ((string)tabla["dato02"] != dato02)
                    {
                        logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA DATO 02 CAJA COMPENSACION " + (string)tabla["nombre"], "CAJAPREVISION", (string)tabla["dato02"], dato02, "MODIFICAR");
                        log.Log();
                    }
                    if ((decimal)tabla["porcPension"] != porcPension)
                    {
                        logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA PORCENTAJE PENSION CAJA COMPENSACION " + (string)tabla["nombre"] + (string)tabla["nombre"], "CAJAPREVISION", (decimal)tabla["porcPension"] + "", porcPension + "", "MODIFICAR");
                        log.Log();
                    }
                    if ((decimal)tabla["porcSalud"] != porcSalud)
                    {
                        logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA PORCENTAJE SALUD CAJA COMPENSACION " + (string)tabla["nombre"], "CAJAPREVISION", (decimal)tabla["porcPension"]+"", porcSalud + "", "MODIFICAR");
                        log.Log();
                    }
                    if ((string)tabla["rut"] != rut)
                    {
                        logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA RUT CAJA COMPENSACION " + (string)tabla["nombre"], "CAJAPREVISION", (string)tabla["rut"], rut, "MODIFICAR");
                        log.Log();
                    }
                    if ((decimal)tabla["porcOtro"] != porcOtro)
                    {
                        logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA PORC OTRO CAJA COMPENSACION " + (string)tabla["nombre"], "CAJAPREVISION", (decimal)tabla["porcOtro"]+"", porcOtro+"", "MODIFICAR");
                        log.Log();
                    }
                    if ((string)tabla["claveExp"] != claveExp)
                    {
                        logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA CLAVE EXP CAJA COMPENSACION " + (string)tabla["nombre"], "CAJAPREVISION", (string)tabla["claveExp"], claveExp, "MODIFICAR");
                        log.Log();
                    }
                    if ((decimal)tabla["porcAccidente"] != porcAccidente)
                    {
                        logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA PORCENTAJE ACCIDENTE CAJA COMPENSACION " + (string)tabla["nombre"], "CAJAPREVISION", (decimal)tabla["porcAccidente"]+"", porcAccidente+"", "MODIFICAR");
                        log.Log();
                    }
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show(ex.Message);
                }
            }
        }
        //GENERA TABLA HASH CON LOS DATOS DESDE BD
        private Hashtable DatosCaja(int pId)
        {
            Hashtable lista = new Hashtable();
            string sql = "SELECT nombre, dato01, dato02, porcPension, porcSalud, rut, porcOtro, claveExp, " +
                "porcAccidente FROM CAJAPREVISION WHERE id=@pId";
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
                                //LLENAMOS TABLA HASH
                                lista.Add("nombre", (string)rd["nombre"]);
                                lista.Add("dato01", (string)rd["dato01"]);
                                lista.Add("dato02", (string)rd["dato02"]);
                                lista.Add("porcPension", (decimal)rd["porcPension"]);
                                lista.Add("porcSalud", (decimal)rd["porcSalud"]);
                                lista.Add("rut", (string)rd["rut"]);
                                lista.Add("porcOtro", (decimal)rd["porcOtro"]);
                                lista.Add("claveExp", (string)rd["claveExp"]);
                                lista.Add("porcAccidente", (decimal)rd["porcAccidente"]);
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

            return lista;
        }
        #endregion
        #endregion

        #region "DATOS ISAPRE"
        //INGRESAR NUEVA ISAPRE
        private void fnNuevoIsapre(TextEdit pId, TextEdit pNombre, string pRut, TextEdit pDato1, TextEdit pDato2)
        {
            //SQL
            string sql = "INSERT INTO isapre(id, nombre, rut, dato01, dato02) VALUES(@pId, @pNombre, @pRut, @pDato1" +
                ", @pDato2)";
            int res = 0;
            SqlCommand cmd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pId", pId.Text));
                        cmd.Parameters.Add(new SqlParameter("@pNombre", pNombre.Text));
                        cmd.Parameters.Add(new SqlParameter("@pRut", pRut));
                        cmd.Parameters.Add(new SqlParameter("@pDato1", pDato1.Text));
                        cmd.Parameters.Add(new SqlParameter("@pDato2", pDato2.Text));
                        //EJECUTAMOS SQL
                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            XtraMessageBox.Show("Registro Guardado", "Guardar", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            //GUARDAR EVENTO EN LOG
                            logRegistro log = new logRegistro(User.getUser(),"SE INGRESA NUEVA ISAPRE", "ISAPRE", "0", pNombre.Text, "INGRESAR");
                            log.Log();

                            //VOLVER A CARGAR GRILLA
                            fnSistema.spllenaGridView(gridIsapre, sqlSalud);
                            fnCargarIsapre(0);                            
                        }
                        else
                        {
                            XtraMessageBox.Show("Error al intentar guardar", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    //LIBERAMOS RECURSOS
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                }
                else
                {
                    XtraMessageBox.Show("No hay conexion a base de datos", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

        }
        //MODIFICAR REGISTRO ISAPRE
        private void fnModificarIsapre(TextEdit pId, TextEdit pNombre, string pRut, TextEdit pDato1, TextEdit pDato2)
        {
            //SQL
            string sql = "UPDATE isapre SET id=@pId, nombre=@pNombre, rut=@pRut, dato01=@pDato1, dato02=@pDato2 " +
                "WHERE id=@pId";
            SqlCommand cmd;
            int res = 0;

            if (pId.Text == "1")
            { XtraMessageBox.Show("No puedes modificar este registro", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            //TABLA HASH PARA LOG
            Hashtable tablaIsapre = new Hashtable();
            tablaIsapre = DatosPrecargados(int.Parse(pId.Text));         

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pId", pId.Text));
                        cmd.Parameters.Add(new SqlParameter("@pNombre", pNombre.Text));
                        cmd.Parameters.Add(new SqlParameter("@pRut", pRut));
                        cmd.Parameters.Add(new SqlParameter("@pDato1", pDato1.Text));
                        cmd.Parameters.Add(new SqlParameter("@pDato2", pDato2.Text));
                        //EJECUTAR CONSULTA
                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            XtraMessageBox.Show("Registro Actualizado", "Actualizar", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            //SI LA MODIFICACION FUE EXITOSA COMPARAMOS VALORES Y GUARDAMOS EN LOG SI HAY CAMBIOS!
                            CambiaValorIsapre(pNombre.Text, pDato1.Text, pDato2.Text, pRut, tablaIsapre);

                            //VOLVER A CARGAR LA GRILLA
                            fnSistema.spllenaGridView(gridIsapre, sqlSalud);
                            fnCargarIsapre(0);

                           
                        }
                        else
                        {
                            XtraMessageBox.Show("Error al intentar actualizar", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
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
        }
        //METODO PARA ELMINAR UNA ISAPRE
        private void fnEliminar(int pId, string nombre)
        {
            //SQL
            string sql = "DELETE FROM isapre WHERE id=@pId";
            int res = 0;
            SqlCommand cmd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pId", pId));
                        //EJECUTAR CONSULTA
                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            XtraMessageBox.Show("Registro Eliminado", "Eliminar", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            //GUARDAR EVENTO EN LOG
                            logRegistro log = new logRegistro(User.getUser(), "ELIMINA ISAPRE", "ISAPRE", nombre, "0", "ELIMINAR");
                            log.Log();

                            //VOLVEMOS A CARGAR LA GRILLA
                            fnSistema.spllenaGridView(gridIsapre, sqlSalud);
                            fnCargarIsapre(0);
                           
                        }
                        else
                        {
                            XtraMessageBox.Show("Error al intentar eliminar", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    //LIBERAMOS RECURSOS
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                }
                else
                {
                    XtraMessageBox.Show("No hay conexion con base de datos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        //COLUMNAS GRILLA
        private void fnColumnasGrilla()
        {
            //ocultar ID
            viewIsapre.Columns[0].Visible = true;
            viewIsapre.Columns[0].Width = 50;
            viewIsapre.Columns[0].Caption = "N°";

            viewIsapre.Columns[1].Caption = "Nombre";
            viewIsapre.Columns[1].Width = 200;
            viewIsapre.Columns[1].AppearanceCell.FontStyleDelta = FontStyle.Bold;

            viewIsapre.Columns[2].Caption = "Rut";
            viewIsapre.Columns[2].Width = 100;
            viewIsapre.Columns[2].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            viewIsapre.Columns[2].DisplayFormat.FormatString = "Rut";
            viewIsapre.Columns[2].DisplayFormat.Format = new FormatCustom();

            viewIsapre.Columns[3].Caption = "Dato1";
            viewIsapre.Columns[4].Caption = "Dato2";
        }
        //Cargar campos desde grilla
        private void fnCargarIsapre(int? pos = -1)
        {
            if (viewIsapre.RowCount > 0)
            {
                if (pos == 0) viewIsapre.FocusedRowHandle = 0;
                //CARGARMOS CAMPOS
                //HABILITAR BOTON ELIMINAR
                btnEliminarIsapre.Enabled = true;
                txtidIsapre.ReadOnly = true;
                txtidIsapre.Text = viewIsapre.GetFocusedDataRow()["id"].ToString();
                txtnombreIsapre.Text = viewIsapre.GetFocusedDataRow()["nombre"].ToString();
                //FORMATEAR RUT
                string rut = fnSistema.fnDesenmascararRut(viewIsapre.GetFocusedDataRow()["rut"].ToString());
                rut = fnSistema.fFormatearRut2(rut);
                txtrutIsapre.Text = rut;
                txtdato1Isapre.Text = viewIsapre.GetFocusedDataRow()["dato01"].ToString();
                txtdato2Isapre.Text = viewIsapre.GetFocusedDataRow()["dato02"].ToString();

                //RESET BOTON NUEVO
                op.SetButtonProperties(btnNuevoIsapre, 1);
                op.Cancela = false;
             

                lblrutIsapre.Visible = false;
            }
        }
        //VERIFICAR ID EN BD
        private bool fnVerificarId(int pId)
        {
            //SQL
            string sql = "SELECT * FROM isapre WHERE id=@pId";
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //parametros
                        cmd.Parameters.Add(new SqlParameter("@pId", pId));
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //SI ENCONTROL EL ID RETORNAMOS TRUE
                            return true;
                        }
                    }
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
        //VERIFICAR NOMBRE ISAPRE EN BD
        private bool fnVerificarNombreIsapre(string pNombre)
        {
            //SQL
            string sql = "SELECT * FROM isapre WHERE nombre=@pNombre";
            SqlCommand cmd;
            SqlDataReader rd;
            bool existe = false;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //parametros
                        cmd.Parameters.Add(new SqlParameter("@pNombre", pNombre));
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
                    fnSistema.sqlConn.Close();
                    rd.Close();
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
            return existe;
        }

        //VERIFICAR QUE RUT PARA ISAPRE YA ESTA INGRESADO
        private bool ExisteRutIsapre(string pRut)
        {
            bool existe = false;
            string sql = "SELECT rut FROM isapre WHERE rut=@pRut";
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
                            //SI ENCUENTRA REGISTROS ES PORQUE EL RUT YA EXISTE EN TABLA
                            existe = true;
                        }
                        else
                        {
                            existe = false;
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

        //LIMPIAR CAMPOS ISAPRE
        private void fnlmpIsapre()
        {
            txtidIsapre.Text = "";
            txtidIsapre.ReadOnly = false;
            //txtidIsapre.Focus();
            txtnombreIsapre.Text = "";
            txtnombreIsapre.Focus();
            txtrutIsapre.Text = "";
            txtdato1Isapre.Text = "";
            txtdato2Isapre.Text = "";
            //DESHABILITAR BOTON ELIMINAR
            btnEliminarIsapre.Enabled = false;

            //OBTENER NUMERO ID DISPONIBLE
            int num = fnItemDisponibles("isapre");
            txtidIsapre.Text = num.ToString();
        }

        private void fnPropiedadesIsapre()
        {
            txtidIsapre.Properties.MaxLength = 5;
            txtnombreIsapre.Properties.MaxLength = 30;
            txtdato1Isapre.Properties.MaxLength = 10;
            txtdato2Isapre.Properties.MaxLength = 10;
            txtrutIsapre.Properties.MaxLength = 9;
            lblrutIsapre.Visible = false;
            btnNuevoIsapre.AllowFocus = false;
            btnEliminarIsapre.AllowFocus = false;
            btnGuardarIsapre.AllowFocus = false;
            gridIsapre.TabStop = false;
            separador1Isapre.TabStop = false;
            separador2Isapre.TabStop = false;
            txtnombreIsapre.Properties.Appearance.FontStyleDelta = FontStyle.Bold;
            //txtrutIsapre.Properties.Appearance.FontStyleDelta = FontStyle.Bold;
        }

        //METODO PARA SABER SI SE CAMBIO EL VALOR EN ALGUNA CAJA
        private bool fnCambiosIsapre(TextEdit pId, TextEdit pNombre, TextEdit pDato1, TextEdit pDato2, TextEdit pRut,
            string pSql)
        {
            //EJEMPLO --> select id, nombre, dato01, dato02, rut from TABLA WHERE id=@pId
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(pSql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pId", pId.Text));
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //COMPARAMOS
                                //PRIMERO QUITAMOS PUNTO Y GUION Y LUEGO ENMASCARAMOS
                                string rut = fnSistema.fnExtraerCaracteres(pRut.Text);
                                rut = fnSistema.fEnmascaraRut(rut);
                                if (rd["nombre"].ToString() != pNombre.Text) return true;
                                if (rd["rut"].ToString() != rut) return true;
                                if (rd["dato01"].ToString() != pDato1.Text) return true;
                                if (rd["dato02"].ToString() != pDato2.Text) return true;
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

        //VERIFICAR SI ALGUN REGISTRO DE ISAPRE ESTA EN USO POR UN TRABAJADOR
        private bool fnIsapreEmpleado(string pId)
        {
            //SQL CONSULTA
            string sql = "SELECT salud FROM trabajador WHERE salud=@pId";
            SqlCommand cmd;
            SqlDataReader rd;

            bool utilizado = false;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pId", pId));

                        //EJECUTAR CONSULTA
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //SI TIENE REGISTROS ES PORQUE EL REGISTRO ESTA SIENDO USADO POR UN TRABAJADOR
                            utilizado = true;
                        }
                        else
                        {
                            utilizado = false;
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

            return utilizado;
        }

        #region "LOG ISAPRE"
        //VERIFICAR SE A MODIFICADO ALGUN CAMPO
        private void CambiaValorIsapre(string pNombre, string dato01, string dato02, string rut, Hashtable ListaClaves)
        {
            try
            {                       
             //COMPARA VALORES SI SON DISTINTOS GUARDAMOS EN LOG REGISTRO
             if ((string)ListaClaves["nombre"] != pNombre)
             {
                logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA NOMBRE ISAPRE " + (string)ListaClaves["nombre"], "ISAPRE", (string)ListaClaves["nombre"], pNombre, "MODIFICAR");
                log.Log();
             }
             if ((string)ListaClaves["dato01"] != dato01)
             {
                logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA DATO 01 ISAPRE " + (string)ListaClaves["nombre"], "ISAPRE", (string)ListaClaves["dato01"], dato01, "MODIFICAR");
                log.Log();
             }
             if ((string)ListaClaves["dato02"] != dato02)
             {
                logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA DATO 02 ISAPRE " + (string)ListaClaves["nombre"], "ISAPRE", (string)ListaClaves["dato02"], dato02, "MODIFICAR");
                log.Log();
             }
             if ((string)ListaClaves["rut"] != rut)
             {
                logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA RUT ISAPRE " + (string)ListaClaves["nombre"], "ISAPRE", (string)ListaClaves["rut"], rut, "MODIFICAR");
                log.Log();
             }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        //GENERA UNA TABLA HASH DE TODOS LOS DATOS PARA EL REGISTRO EN EVALUACION
        //NECESITA COMO PARAMETRO DE ENTRADA EL ID
        private Hashtable DatosPrecargados(int pId)
        {            
            string sql = "SELECT nombre, dato01, dato02, rut FROM ISAPRE WHERE id=@pId";
            SqlCommand cmd;
            SqlDataReader rd;
            Hashtable tabla = new Hashtable();
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
                                //AGREGAMOS DATOS A 
                                tabla.Add("nombre",(string)rd["nombre"]);
                                tabla.Add("dato01", (string)rd["dato01"]);
                                tabla.Add("dato02", (string)rd["dato02"]);
                                tabla.Add("rut", (string)rd["rut"]);
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
            
            //RETORNAMOS LA TABLA HASH
            return tabla;
        }
        #endregion

        #endregion

        #region "CONTROLS AFP"    
        private void btnNuevo_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            fnLimpiar(txtId, txtNombre, txtRut, txtAdmin, txtFondo, txtOtro, txtClave, txtdato1, txtdato2, btnEliminar, "afp");

            if (op.Cancela == false)
            {
                op.SetButtonProperties(btnNuevo, 2);
                op.Cancela = true;
                txtNombre.Properties.Appearance.BackColor = Color.LightGoldenrodYellow;
            }
            else
            {
                op.SetButtonProperties(btnNuevo, 1);
                op.Cancela = false;                     
                fnCargarCamposPrev(txtId, txtNombre, txtRut, txtFondo, txtAdmin, txtOtro, txtdato1, txtdato2, txtClave,
                btnEliminar, viewPrevision, 0);
                txtNombre.Properties.Appearance.BackColor = Color.White;            
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD   
            Sesion.NuevaActividad();

            //CONSULTA SQL PARA INSERT
            string sqlInsert = "INSERT INTO AFP(id, nombre, dato01, dato02, porcFondo, porcAdmin, rut, porcOtro, claveExp)" +
               " VALUES(@pId, @pNombre, @pDato1, @pDato2, @pFondo, @pAdmin, @pRut, @pOtro, @pClave)";

            //QUERY UPDATE
            string sqlUpdate = "UPDATE afp SET nombre=@pNombre, dato01=@pDato1, dato02=@pDato2, porcFondo=@pFondo," +
                "porcAdmin=@pAdmin, rut=@pRut, porcOtro=@pOtro, claveExp=@pClave WHERE Id=@pId";

            //PARA VERIFICAR ID EN BD
            string busqueda = "SELECT * FROM afp WHERE id=@pId";

            //QUERY PARA GRILLA
            //string grilla = "SELECT id, nombre, porcFondo, porcAdmin, rut, porcOtro, claveExp, dato01, dato02 FROM afp ORDER BY nombre";

            if (txtId.Text == "") { XtraMessageBox.Show("Id no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtId.Focus(); return; }
            if (txtNombre.Text == "") { XtraMessageBox.Show("Debes ingresar un Nombre"); txtNombre.Focus(); return; }
            if (txtRut.Text == "") { XtraMessageBox.Show("Debes ingresar un rut valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtRut.Focus(); return; }
            if (txtAdmin.Text == "") { XtraMessageBox.Show("Porcentaje Adminstrado no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtAdmin.Focus(); return; }
            if (txtFondo.Text == "") { XtraMessageBox.Show("Porcentaje Fondo no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtFondo.Focus(); return; }

            string rut = "", rutAntiguo = "";
            string NombreAntiguo = "";
            //DEBEMOS VALIDAR RUT EN CASO DE QUE SEA UN REGISTRO NUEVO
            if (txtId.ReadOnly)
            {
                //UPDATE
                //PRIMERAMENTE QUITAR LOS PUNTOS Y GUION AL RUT
                if (txtRut.Text.Contains(".") || txtRut.Text.Contains("-"))
                    rut = fnSistema.fnExtraerCaracteres(txtRut.Text);
                else
                    rut = txtRut.Text;

                if (rut.Length < 8 || rut.Length > 9) { XtraMessageBox.Show("rut no valido", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);return;}                

                //VOLVER A VERIFICAR RUT
                bool rutValido = fnSistema.fValidaRut(rut);
                //si rutvlaido es true es un rut valido
                if (rutValido == false) { XtraMessageBox.Show("Rut no valido", "Rut", MessageBoxButtons.OK, MessageBoxIcon.Error); txtRut.Focus(); return; }                

                //VALIDAR CAMPOS PORCENTAJES
                bool porc1 = fnValidarPorcentajes(txtFondo.Text);
                //SI RETORNA FALSE NO ES VALIDO
                if (porc1 == false) { XtraMessageBox.Show("Valor Campo Fondo no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtFondo.Focus(); return; }
                bool porc2 = fnValidarPorcentajes(txtAdmin.Text);
                //SI RETORNA FALSE NO ES VALIDO
                if (porc2 == false) { XtraMessageBox.Show("Valor Campo Admin no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtAdmin.Focus(); return; }

                //VALIDAR SI NOMBRE DE AFP YA EXISTE (SOLO SI SE DETECTA QUE EL NOMBRE A CAMBIADO)
                if (viewPrevision.RowCount > 0)
                {
                    NombreAntiguo = viewPrevision.GetFocusedDataRow()["nombre"].ToString();
                    rutAntiguo = viewPrevision.GetFocusedDataRow()["rut"].ToString();
                    if (NombreAntiguo != txtNombre.Text)
                    {
                        //VERIFICAMOS QUE NOMBRE NO EXISTA
                        if (ExistePrevision(txtNombre.Text))
                        { XtraMessageBox.Show("Nombre Ingresado ya existe", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);return;}
                    }

                    if (rutAntiguo != rut)
                    {                            
                        if (ExisteRutPrevision(rut))
                        { XtraMessageBox.Show("Rut ingresado ya existe", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);return;}
                    }
                }

                //ANTES DE GUARDAR ENMASCARAMOS EL RUT
                rut = fnSistema.fEnmascaraRut(rut);

                bool mod = fnModificarAfp(txtId, txtNombre, txtFondo, txtAdmin, txtOtro, txtClave, rut, txtdato1, txtdato2, sqlUpdate);

                //SI MOD ES TRUE SE MODIFICO CORRECTAMENTE
                if (mod)
                {
                    //GUARDAR EVENTO EN REGISTRO LOG
                   // logRegistro log = new logRegistro(User.getUser(), "SE HA MODIFICADO REGISTRO CON ID " + txtId.Text, fnSistema.pgDatabase, "AFP");
                   // log.Log();

                    //RECARGAR GRILLA
                    fnSistema.spllenaGridView(gridPrevision, SqlAfp);
                    fnCargarCamposPrev(txtId, txtNombre, txtRut, txtFondo, txtAdmin, txtOtro, txtdato1, txtdato2,
             txtClave, btnEliminar, viewPrevision, 0);                   

                    //SI LA VARIABLE OPENER ES DISTINTA DE NULL ES PORQUE EL FORM SE INVOCO DESDE EL FORM EMPLEADO
                    if (opener != null)
                        opener.RecargarComboAfp();
                }
            }
            else
            {
                //INSERT NUEVO
                //PRIMERAMENTE QUITAR LOS PUNTOS Y GUION AL RUT
                if (txtRut.Text.Contains(".") || txtRut.Text.Contains("-"))
                    rut = fnSistema.fnExtraerCaracteres(txtRut.Text);
                else
                    rut = txtRut.Text;

                if (rut.Length < 8 || rut.Length > 9) { XtraMessageBox.Show("rut no valido", "error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

                bool rutValido = fnSistema.fValidaRut(rut);
                //si rutvlaido es true es un rut valido
                if (rutValido == false) { XtraMessageBox.Show("Rut no valido", "Rut", MessageBoxButtons.OK, MessageBoxIcon.Error); txtRut.Focus(); return; }

                //VALIDAR QUE RUT NO EXISTE 
                if (ExisteRutPrevision(rut))
                { XtraMessageBox.Show("Rut ya existe", "Rut", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

                //VERIFICAR QUE NOMBRE NO EXISTA
                if (ExistePrevision(txtNombre.Text))
                { XtraMessageBox.Show("Nombre ya existe, por favor verifica la informacion y vuelve a intentarlo", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);return;}

                //if rut => true continue...
                bool existe = fnVerificarId(int.Parse(txtId.Text), busqueda);
                //SI EXISTE ES TRUE ES PORQUE ID YA ESTA EN BD
                if (existe) { XtraMessageBox.Show("El id ingresado ya existe", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                //VALIDAR CAMPOS PORCENTAJES
                bool porc1 = fnValidarPorcentajes(txtFondo.Text);
                //SI RETORNA FALSE NO ES VALIDO
                if (porc1 == false) { XtraMessageBox.Show("Valor Campo Fondo no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtFondo.Focus(); return; }
                bool porc2 = fnValidarPorcentajes(txtAdmin.Text);
                //SI RETORNA FALSE NO ES VALIDO
                if (porc2 == false) { XtraMessageBox.Show("Valor Campo Admin no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtAdmin.Focus(); return; }
                //INSERTAR BD

                //ENMASCARAMOS rut
                rut = fnSistema.fEnmascaraRut(rut);
                bool nuevo = fnNuevoAfp(txtId, txtNombre, txtFondo, txtAdmin, txtOtro, txtClave, rut, txtdato1, txtdato2, sqlInsert);

                //SI NUEVO ES TRUE ES PORQUE SE REALIZO CORRECTAMENTE EL INSERT
                if (nuevo)
                {
                    //GUARDAR EVENTO EN LOG
                    logRegistro log = new logRegistro(User.getUser(), "SE INGRESA NUEVA ASEGURADORA DE PENSIONES", "AFP", "0", txtNombre.Text, "INGRESAR");
                    log.Log();

                    //RECARGAR GRILLA
                    fnSistema.spllenaGridView(gridPrevision, SqlAfp);
                    fnCargarCamposPrev(txtId, txtNombre, txtRut, txtFondo, txtAdmin, txtOtro, txtdato1, txtdato2, txtClave,
                        btnEliminar, viewPrevision, 0);                    

                    //SI LA VARIABLE OPENER ES DISTINTA DE NULL ES PORQUE EL FORM SE INVOCO DESDE EL FORM EMPLEADO
                    if(opener != null)
                        opener.RecargarComboAfp();
                }
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            //PARA VERIFICAR SI REGISTRO ESTA EN USO POR UN EMPLEADO
            bool RegistroEnUso = false;

            //QUERY PARA DELETE
            string sqlDelete = "DELETE FROM afp WHERE id=@pId";
            //PARA VERIFICAR ID EN BD
            string busqueda = "SELECT * FROM afp WHERE id=@pId";

            //QUERY PARA GRILLA
            //string grilla = "SELECT id, nombre, porcFondo, porcAdmin, rut, porcOtro, claveExp, dato01, dato02 FROM afp ORDER BY nombre";

            string afp = "";
            bool existe;

            if (txtId.Text == "") { XtraMessageBox.Show("Id no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            existe = fnVerificarId(int.Parse(txtId.Text), busqueda);

            if (existe == false) { XtraMessageBox.Show("Registro no Valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            //VERIFICAR SI EL REGISTRO ESTA EN USO POR UN EMPLEADO
            
            if (viewPrevision.RowCount > 0)
            {               
                RegistroEnUso = fnAfpEmpleado(viewPrevision.GetFocusedDataRow()["id"].ToString());
                string name = viewPrevision.GetFocusedDataRow()["nombre"].ToString();
                
                if (RegistroEnUso)
                {
                    XtraMessageBox.Show(name + " esta siendo utilizado por un empleado, por lo que no es posible eliminarlo", "Denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }           

            if (viewPrevision.RowCount > 0) afp = viewPrevision.GetFocusedDataRow()["nombre"].ToString();

            DialogResult dialogo = XtraMessageBox.Show("¿Realmente desea eliminar a " + afp + " ?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogo == DialogResult.Yes)
            {
                if (viewPrevision.RowCount > 0)
                {
                    //ELIMINAMOS FILA SELECCIONADA
                    bool del = fnEliminarAfp(int.Parse(viewPrevision.GetFocusedDataRow()["id"].ToString()), sqlDelete);
                    //SI DEL ES TRUE ES PORQUE EL REGISTRO SE ELIMINO CORRECTAMENTE
                    if (del)
                    {
                        //GUARDAMOS EVENTO EN LOG
                        logRegistro log = new logRegistro(User.getUser(), "SE ELIMINA ASEGURADORA DE PENSIONES", "AFP", afp, "0", "ELIMINAR");
                        log.Log();

                        //RECARGAR GRILLA
                        fnSistema.spllenaGridView(gridPrevision, SqlAfp);
                        fnCargarCamposPrev(txtId, txtNombre, txtRut, txtFondo, txtAdmin, txtOtro, txtdato1, txtdato2, txtClave,
                            btnEliminar, viewPrevision, 0);                       

                        //SI LA VARIABLE OPENER ES DISTINTA DE NULL ES PORQUE EL FORM SE INVOCO DESDE EL FORM EMPLEADO
                        if (opener != null)
                            opener.RecargarComboAfp();
                    }
                }
            }
        }

        private void txtId_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtRut_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (e.KeyChar == (char)107)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void gridPrevision_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            fnCargarCamposPrev(txtId, txtNombre, txtRut, txtFondo, txtAdmin, txtOtro, txtdato1, txtdato2, txtClave,
              btnEliminar, viewPrevision);
        }

        private void gridPrevision_KeyUp(object sender, KeyEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            fnCargarCamposPrev(txtId, txtNombre, txtRut, txtFondo, txtAdmin, txtOtro, txtdato1, txtdato2, txtClave,
              btnEliminar, viewPrevision);
        }

        private void txtFondo_KeyPress(object sender, KeyPressEventArgs e)
        {
            //CARACTER 44 -> ',' (COMA)
            //CARACTER 46-> '.' (PUNTO)
            if (e.KeyChar == (char)44)
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
            else
            {
                e.Handled = true;
            }
        }

        private void txtAdmin_KeyPress(object sender, KeyPressEventArgs e)
        {
            //CARACTER 44 -> ',' (COMA)
            //CARACTER 46-> '.' (PUNTO)
            if (e.KeyChar == (char)44)
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
            else
            {
                e.Handled = true;
            }
        }

        private void txtOtro_KeyPress(object sender, KeyPressEventArgs e)
        {
            //CARACTER 44 -> ',' (COMA)
            //CARACTER 46-> '.' (PUNTO)
            if (e.KeyChar == (char)44)
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
            else
            {
                e.Handled = true;
            }
        }
        #endregion

        #region "CONTROLES CAJAPREVISION"        

        private void btnNuevoPrev_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD   
            Sesion.NuevaActividad();

            fnLimpiarCaja(txtIdPrev, txtNombrePrev, txtRutPrev, txtSaludPrev, txtPensionPrev, txtOtroPrev, txtClavePrev,
               txtDato1Prev, txtDato2Prev,txtAccidente, btnEliminarPrev);
            if (op.Cancela == false)
            {
                op.SetButtonProperties(btnNuevoPrev, 2);
                op.Cancela = true;           
                txtNombrePrev.Properties.Appearance.BackColor = Color.LightGoldenrodYellow;
            }
            else
            {
                op.SetButtonProperties(btnNuevoPrev, 1);
                op.Cancela = false;
                fnCargarCamposCaja(txtIdPrev, txtNombrePrev, txtRutPrev, txtPensionPrev, txtSaludPrev, txtOtroPrev, txtDato1Prev,
                  txtDato2Prev, txtClavePrev, txtAccidente, btnEliminarPrev, viewPrev, 0);
                txtNombrePrev.Properties.Appearance.BackColor = Color.White;            
            }
        }

        private void btnGuardarPrev_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD   
            Sesion.NuevaActividad();
            
            //CONSULTA SQL PARA INSERT
            string sqlInsert = "INSERT INTO cajaPrevision(id, nombre, dato01, dato02, porcPension, porcSalud, rut, porcOtro, claveExp, porcAccidente)" +
            " VALUES(@pId, @pNombre, @pDato1, @pDato2, @pPension, @pSalud, @pRut, @pOtro, @pClave, @pAccidente)";

            //QUERY UPDATE
            string sqlUpdate = "UPDATE cajaPrevision SET nombre=@pNombre, dato01=@pDato1, dato02=@pDato2, porcPension=@pPension," +
        "porcSalud=@pSalud, rut=@pRut, porcOtro=@pOtro, claveExp=@pClave, porcAccidente=@pAccidente WHERE Id=@pId";

            //PARA VERIFICAR ID EN BD
            string busqueda = "SELECT * FROM cajaPrevision WHERE id=@pId";

            //QUERY PARA GRILLA
            //string grilla = "SELECT id, nombre, rut, porcPension, porcSalud, porcAccidente, porcOtro, claveExp, dato01, dato02 FROM cajaPrevision ORDER BY nombre";

            if (txtIdPrev.Text == "") { XtraMessageBox.Show("Id no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtIdPrev.Focus(); return; }
            if (txtNombrePrev.Text == "") { XtraMessageBox.Show("Debes ingresar un Nombre"); txtNombrePrev.Focus(); return; }
            if (txtRutPrev.Text == "") { XtraMessageBox.Show("Debes ingresar un rut valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtRutPrev.Focus(); return; }
            if (txtSaludPrev.Text == "") { XtraMessageBox.Show("Porcentaje Salud no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtSaludPrev.Focus(); return; }
            if (txtPensionPrev.Text == "") { XtraMessageBox.Show("Porcentaje Pension IPS no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtPensionPrev.Focus(); return; }
            if (txtAccidente.Text == "") { XtraMessageBox.Show("Porcentaje Accidentes del trabajo no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtAccidente.Focus(); return; }

            string rut = "", rutAntiguo = "", NombreAntiguo = "";
            //DEBEMOS VALIRDAR RUT EN CASO DE QUE SEA UN REGISTRO NUEVO
            if (txtIdPrev.ReadOnly)
            {
                //UPDATE
                //PRIMERAMENTE QUITAR LOS PUNTOS Y GUION AL RUT
                if (txtRutPrev.Text.Contains(".") || txtRutPrev.Text.Contains("-"))
                    rut = fnSistema.fnExtraerCaracteres(txtRutPrev.Text);
                else
                    rut = txtRutPrev.Text;

                if (rut.Length < 8 || rut.Length > 9) { XtraMessageBox.Show("rut no valido", "error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

                //VOLVER A VERIFICAR RUT
                bool rutValido = fnSistema.fValidaRut(rut);
                //si rutvlaido es true es un rut valido
                if (rutValido == false) { XtraMessageBox.Show("Rut no valido", "Rut", MessageBoxButtons.OK, MessageBoxIcon.Error); txtRutPrev.Focus(); return; }              

                //VALIDAR CAMPOS PORCENTAJES
                bool porc1 = fnValidarPorcentajes(txtPensionPrev.Text);
                //SI RETORNA FALSE NO ES VALIDO
                if (porc1 == false) { XtraMessageBox.Show("Valor Campo Pension IPS no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtPensionPrev.Focus(); return; }
                bool porc2 = fnValidarPorcentajes(txtSaludPrev.Text);
                //SI RETORNA FALSE NO ES VALIDO
                if (porc2 == false) { XtraMessageBox.Show("Valor Campo Fonasa no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtSaludPrev.Focus(); return; }

                bool porc3 = fnValidarPorcentajes(txtAccidente.Text);
                //SI RETORNA FALSE NO ES VALIDO
                if (porc3 == false) { XtraMessageBox.Show("Valor Campo Accidentes del trabajo no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtAccidente.Focus(); return; }              

                if (viewPrev.RowCount>0)
                {
                    rutAntiguo = viewPrev.GetFocusedDataRow()["rut"].ToString();
                    NombreAntiguo = viewPrev.GetFocusedDataRow()["nombre"].ToString();
                    if (NombreAntiguo != txtNombrePrev.Text)
                    {
                        if (ExisteNombreCaja(txtNombrePrev.Text))
                        { XtraMessageBox.Show("Nombre ingresado ya existe", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);txtNombrePrev.Focus();return;}
                    }

                    if (rutAntiguo != rut)
                    {
                        if (ExisteRutCaja(rut))
                        { XtraMessageBox.Show("Rut ingresado ya existe", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);txtRutPrev.Focus(); return;}
                    }
                }

                //ENMASCARAR RUT
                rut = fnSistema.fEnmascaraRut(rut);
                bool mod = fnModificarCaja(txtIdPrev, txtNombrePrev, txtPensionPrev, txtSaludPrev, txtOtroPrev, txtClavePrev, rut, txtDato1Prev, txtDato2Prev,txtAccidente, sqlUpdate);

                //SI MOD ES TRUE SE MODIFICO CORRECTAMENTE                
                if (mod)
                {
                    //GUARDA EVENTO EN LOG
                    //logRegistro log = new logRegistro(User.getUser(), "REGISTRO MODIFICADO CON ID " + txtIdPrev.Text, fnSistema.pgDatabase, "CAJAPREVISION");
                    //log.Log();

                    //RECARGAR GRILLA
                    fnSistema.spllenaGridView(gridPrev, sqlCajaPrevision);
                    fnCargarCamposCaja(txtIdPrev, txtNombrePrev, txtRutPrev, txtPensionPrev, txtSaludPrev, txtOtroPrev, txtDato1Prev,
                      txtDato2Prev, txtClavePrev, txtAccidente, btnEliminarPrev, viewPrev, 0);
                  

                    //SI LA VARIABLE OPENER ES DISTINTA DE NULL ES PORQUE EL FORM SE INVOCO DESDE FORM EMPLEADO
                    if (opener != null)
                        opener.RecargarComboCaja();
                }
            }
            else
            {
                //INSERT NUEVO

                //PRIMERAMENTE QUITAR LOS PUNTOS Y GUION AL RUT
                if (txtRutPrev.Text.Contains(".") || txtRutPrev.Text.Contains("-"))
                    rut = fnSistema.fnExtraerCaracteres(txtRutPrev.Text);
                else
                    rut = txtRutPrev.Text;

                if (rut.Length < 8 || rut.Length > 9) { XtraMessageBox.Show("rut no valido", "error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

                bool rutValido = fnSistema.fValidaRut(rut);
                //si rutvlaido es true es un rut valido
                if (rutValido == false) { XtraMessageBox.Show("Rut no valido", "Rut", MessageBoxButtons.OK, MessageBoxIcon.Error); txtRutPrev.Focus(); return; }

                //VALIDAR QUE RUT NO EXISTE
                if (ExisteRutCaja(rut))
                { XtraMessageBox.Show("Rut ingresado ya existe", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

                //if rut => true continue...
                bool existe = fnVerificarId(int.Parse(txtIdPrev.Text), busqueda);
                //SI EXISTE ES TRUE ES PORQUE ID YA ESTA EN BD
                if (existe) { XtraMessageBox.Show("El id ingresado ya existe", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                //VALIDAR CAMPOS PORCENTAJES
                bool porc1 = fnValidarPorcentajes(txtPensionPrev.Text);
                //SI RETORNA FALSE NO ES VALIDO
                if (porc1 == false) { XtraMessageBox.Show("Valor Campo Pension no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtPensionPrev.Focus(); return; }
                bool porc2 = fnValidarPorcentajes(txtSaludPrev.Text);
                //SI RETORNA FALSE NO ES VALIDO
                if (porc2 == false) { XtraMessageBox.Show("Valor Campo Fonasa no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtSaludPrev.Focus(); return; }

                bool porc3 = fnValidarPorcentajes(txtAccidente.Text);
                //SI RETORNA FALSE NO ES VALIDO
                if (porc3 == false) { XtraMessageBox.Show("Valor Campo Accidentes del trabajo no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtAccidente.Focus(); return; }

                //VALIDAR QUE NOMBRE NO EXISTA
                if (ExisteNombreCaja(txtNombrePrev.Text))
                { XtraMessageBox.Show("Nombre ya existe", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);return;}

                //INSERTAR BD
                //ENMASCARAR RUT
                rut = fnSistema.fEnmascaraRut(rut);
                bool nuevo = fnNuevoCaja(txtIdPrev, txtNombrePrev, txtPensionPrev, txtSaludPrev, txtOtroPrev, txtClavePrev, rut, txtDato1Prev, txtDato2Prev, txtAccidente,sqlInsert);

                //SI NUEVO ES TRUE ES PORQUE SE REALIZO CORRECTAMENTE EL INSERT
                if (nuevo)
                {
                    //GUARDAR EVENTO EN LOG
                    logRegistro log = new logRegistro(User.getUser(), "SE INGRESA NUEVA CAJA DE PREVISION", "CAJAPREVISION", "0", txtNombrePrev.Text, "INGRESO");
                    log.Log();

                    //RECARGAR GRILLA
                    fnSistema.spllenaGridView(gridPrev, sqlCajaPrevision);
                    fnCargarCamposCaja(txtIdPrev, txtNombrePrev, txtRutPrev, txtPensionPrev, txtSaludPrev, txtOtroPrev, txtDato1Prev,
                     txtDato2Prev, txtClavePrev, txtAccidente, btnEliminarPrev, viewPrev, 0);
                   

                    //SI LA VARIABLE OPENER ES DISTINTA DE NULL ES PORQUE EL FORM SE INVOCO DESDE FORM EMPLEADO
                    if (opener != null)
                        opener.RecargarComboCaja();
                }

            }
        }

        private void btnEliminarPrev_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD   
            Sesion.NuevaActividad();

            bool utilizado = false;

            //QUERY PARA DELETE
            string sqlDelete = "DELETE FROM cajaPrevision WHERE id=@pId";

            //PARA VERIFICAR ID EN BD
            string busqueda = "SELECT * FROM cajaPrevision WHERE id=@pId";

            //QUERY PARA GRILLA
            //string grilla = "SELECT id, nombre, rut, porcPension, porcSalud, porcAccidente, porcOtro, claveExp, dato01, dato02 FROM cajaPrevision ORDER BY nombre";

            string afp = "";
            bool existe;
            if (txtIdPrev.Text == "") { XtraMessageBox.Show("Id no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            existe = fnVerificarIdCaja(int.Parse(txtIdPrev.Text), busqueda);
            if (existe == false) { XtraMessageBox.Show("Registro no Valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            //VERIFICAR SI LA CAJA DE PREVISION ESTA SIENDO USADA POR UN EMPLEADO
            if (viewPrev.RowCount>0)
            {
                utilizado = fnCajaEmpleado(viewPrev.GetFocusedDataRow()["id"].ToString());
                string name = viewPrev.GetFocusedDataRow()["nombre"].ToString();
                if (utilizado)
                {
                    XtraMessageBox.Show(name + " esta siendo usado por un empleado, por lo que no es posible eliminarlo", "Denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            if (viewPrev.RowCount > 0) afp = viewPrev.GetFocusedDataRow()["nombre"].ToString();
            DialogResult dialogo = XtraMessageBox.Show("¿Realmente desea eliminar a " + afp + " ?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogo == DialogResult.Yes)
            {
                if (viewPrev.RowCount > 0)
                {
                    //ELIMINAMOS FILA SELECCIONADA
                    bool del = fnEliminarCaja(int.Parse(viewPrev.GetFocusedDataRow()["id"].ToString()), sqlDelete);
                    //SI DEL ES TRUE ES PORQUE EL REGISTRO SE ELIMINO CORRECTAMENTE
                    if (del)
                    {
                        //GUARDAR EVENTO EN LOG
                        logRegistro log = new logRegistro(User.getUser(), "SE ELIMINA CAJA PREVISION", "CAJAPREVISION", afp, "0", "ELIMINAR");
                        log.Log();

                        //RECARGAR GRILLA
                        fnSistema.spllenaGridView(gridPrev, sqlCajaPrevision);
                        fnCargarCamposCaja(txtIdPrev, txtNombrePrev, txtRutPrev, txtPensionPrev, txtSaludPrev, txtOtroPrev, txtDato1Prev,
                      txtDato2Prev, txtClavePrev, txtAccidente, btnEliminarPrev, viewPrev, 0);                        

                        //SI LA VARIABLE OPENER ES DISTINTA DE NULL ES PORQUE EL FORM SE INVOCO DESDE FORM EMPLEADO
                        if (opener != null)
                            opener.RecargarComboCaja();
                    }
                }
            }
        }

        private void txtIdPrev_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtRutPrev_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (e.KeyChar == (char)107)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void txtPensionPrev_KeyPress(object sender, KeyPressEventArgs e)
        {
            //CARACTER 44 -> ',' (COMA)
            //CARACTER 46-> '.' (PUNTO)
            if (e.KeyChar == (char)44)
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
            else
            {
                e.Handled = true;
            }
        }

        private void txtSaludPrev_KeyPress(object sender, KeyPressEventArgs e)
        {
            //CARACTER 44 -> ',' (COMA)
            //CARACTER 46-> '.' (PUNTO)
            if (e.KeyChar == (char)44)
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
            else
            {
                e.Handled = true;
            }
        }

        private void txtAccidente_KeyPress(object sender, KeyPressEventArgs e)
        {

            //CARACTER 44 -> ',' (COMA)
            //CARACTER 46-> '.' (PUNTO)
            if (e.KeyChar == (char)44)
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
            else
            {
                e.Handled = true;
            }
        }
        private void txtOtroPrev_KeyPress(object sender, KeyPressEventArgs e)
        {
            //CARACTER 44 -> ',' (COMA)
            //CARACTER 46-> '.' (PUNTO)
            if (e.KeyChar == (char)44)
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
            else
            {
                e.Handled = true;
            }
        }

        private void gridPrev_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD   
            Sesion.NuevaActividad();

            fnCargarCamposCaja(txtIdPrev, txtNombrePrev, txtRutPrev, txtPensionPrev, txtSaludPrev, txtOtroPrev, txtDato1Prev,
                    txtDato2Prev, txtClavePrev, txtAccidente, btnEliminarPrev, viewPrev);
        }

        private void gridPrev_KeyUp(object sender, KeyEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            fnCargarCamposCaja(txtIdPrev, txtNombrePrev, txtRutPrev, txtPensionPrev, txtSaludPrev, txtOtroPrev, txtDato1Prev,
                    txtDato2Prev, txtClavePrev, txtAccidente, btnEliminarPrev, viewPrev);
        }

        #endregion

        #region "CONTROLES ISAPRE"

       

        private void btnNuevoIsapre_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD   
            Sesion.NuevaActividad();

            fnlmpIsapre();
            if (op.Cancela == false)
            {
                op.SetButtonProperties(btnNuevoIsapre, 2);
                op.Cancela = true;
                txtnombreIsapre.Properties.Appearance.BackColor = Color.LightGoldenrodYellow;
            }
            else
            {
                op.SetButtonProperties(btnNuevoIsapre, 1);
                op.Cancela = false;             
                fnCargarIsapre(0);
                txtnombreIsapre.Properties.Appearance.BackColor = Color.White;      
            }
        }

        private void btnGuardarIsapre_Click(object sender, EventArgs e)
        {
            //NUEVA SESION
            Sesion.NuevaActividad();

            if (txtidIsapre.Text == "") { XtraMessageBox.Show("id no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtidIsapre.Focus(); return; }
            if (txtnombreIsapre.Text == "") { XtraMessageBox.Show("Nombre no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtnombreIsapre.Focus(); return; }
            if (txtrutIsapre.Text == "") { XtraMessageBox.Show("Rut no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtrutIsapre.Focus(); return; }

            string rut = "", rutAntiguo = "", NombreAntiguo = "";
            if (txtidIsapre.ReadOnly)
            {
                //UPDATE
                //PRIMERAMENTE QUITAR LOS PUNTOS Y GUION AL RUT
                if (txtrutIsapre.Text.Contains(".") || txtrutIsapre.Text.Contains("-"))
                    rut = fnSistema.fnExtraerCaracteres(txtrutIsapre.Text);
                else
                    rut = txtrutIsapre.Text;

                if (rut.Length < 8 || rut.Length > 9) { XtraMessageBox.Show("rut no valido", "error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

                //verificar nuevamente rut
                bool rutValido = fnSistema.fValidaRut(rut);               

                if (rutValido == false) { XtraMessageBox.Show("Rut no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtrutIsapre.Focus(); return; }
                if (viewIsapre.RowCount > 0)
                {
                    NombreAntiguo = viewIsapre.GetFocusedDataRow()["nombre"].ToString();
                    rutAntiguo = viewIsapre.GetFocusedDataRow()["rut"].ToString();
                    if (NombreAntiguo != txtnombreIsapre.Text)
                    {
                        if (fnVerificarNombreIsapre(txtnombreIsapre.Text))
                        { XtraMessageBox.Show("Nombre ya registrado", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
                    }
                    if (rutAntiguo != rut)
                    {
                        if (ExisteRutIsapre(rut))
                        { XtraMessageBox.Show("Rut ingresado ya existe", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
                    }
                }

                rut = fnSistema.fEnmascaraRut(rut);            
                fnModificarIsapre(txtidIsapre, txtnombreIsapre, rut, txtdato1Isapre, txtdato2Isapre);

                //RECARGAR COMBO SALUD FORMULARIO EMPLEADO
                //SI LA VARIABLE OPENER ES DISTINTA DE NULL, SIGNIFICA QUE EL FORMULARIO SE ABRIO DESDE FORM EMPLEADO
                if (opener != null)
                {
                    opener.RecargarComboSalud();
                }
            }
            else
            {
                //INSERT

                //PRIMERAMENTE QUITAR LOS PUNTOS Y GUION AL RUT
                if (txtrutIsapre.Text.Contains(".") || txtrutIsapre.Text.Contains("-"))
                    rut = fnSistema.fnExtraerCaracteres(txtrutIsapre.Text);
                else
                    rut = txtrutIsapre.Text;

                if (rut.Length < 8 || rut.Length > 9) { XtraMessageBox.Show("rut no valido", "error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

                //validamos rut
                bool rutValido = fnSistema.fValidaRut(rut);
                if (rutValido == false) { XtraMessageBox.Show("Rut no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtrutIsapre.Focus(); return; }

                //VERIFICAR QUE EL RUT NO EXISTE
                if (ExisteRutIsapre(rut))
                { XtraMessageBox.Show("Rut ya ingresado", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);return;}

                //validar ID que no exista previamente en BD
                bool existe = fnVerificarId(int.Parse(txtidIsapre.Text));
                //SI ES TRUE --> NO VALIDO
                if (existe) { XtraMessageBox.Show("Id ingresado ya existe", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtidIsapre.Focus(); return; }
                //validar nombre
                bool nombre = fnVerificarNombreIsapre(txtnombreIsapre.Text);
                //SI ESTRUE NOMBRE YA EXISTE
                if (nombre) { XtraMessageBox.Show("Nombre ingresado ya existe", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtnombreIsapre.Focus(); return; }
                //INSERTAMOS NUEVO REGISTRO

                //enmascarar rut
                rut = fnSistema.fEnmascaraRut(rut);
                fnNuevoIsapre(txtidIsapre, txtnombreIsapre, rut, txtdato1Isapre, txtdato2Isapre);

                //RECARGAR COMBO SALUD FORMULARIO EMPLEADO
                //SI LA VARIABLE OPENER ES DISTINTA DE NULL, SIGNIFICA QUE EL FORMULARIO SE ABRIO DESDE FORM EMPLEADO
                if (opener != null)
                {
                    opener.RecargarComboSalud();
                }
            }
        }

        private void btnEliminarIsapre_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD   
            Sesion.NuevaActividad();

            bool existe = false, utilizado = false;
            string isapre = "";
            if (txtidIsapre.Text == "") { XtraMessageBox.Show("id no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtidIsapre.Focus(); return; }
            existe = fnVerificarId(int.Parse(txtidIsapre.Text));
            //SI ES TRUE EXISTE SI NO NO EXISTE
            if (existe == false) { XtraMessageBox.Show("Registro no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtidIsapre.Focus(); return; }

            //VERIFICAR SI ESTE REGISTRO ESTA SIENDO UTILIZADO POR UN EMPLEADO
            if (viewIsapre.RowCount>0)
            {
                utilizado = fnIsapreEmpleado(viewIsapre.GetFocusedDataRow()["id"].ToString());
                string name = viewIsapre.GetFocusedDataRow()["nombre"].ToString();
                if (utilizado)
                {
                    XtraMessageBox.Show(name + " esta siendo utilizado por un empleado, por lo que no se puede eliminar", "Denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (viewIsapre.GetFocusedDataRow()["id"].ToString() == "1")
                { XtraMessageBox.Show("No puedes eliminar este registro", "Eliminar", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
            }

            if (viewIsapre.RowCount > 0) isapre = viewIsapre.GetFocusedDataRow()["nombre"].ToString();
            DialogResult dialogo = XtraMessageBox.Show("¿Desea realmente eliminar a " + isapre + "?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogo == DialogResult.Yes)
            {
                //ELIMINAMOS
                if (viewIsapre.RowCount > 0)
                {
                    int id = int.Parse(viewIsapre.GetFocusedDataRow()["id"].ToString());
                    fnEliminar(id, isapre);

                    //RECARGAR COMBO SALUD FORMULARIO EMPLEADO
                    //SI LA VARIABLE OPENER ES DISTINTA DE NULL, SIGNIFICA QUE EL FORMULARIO SE ABRIO DESDE FORM EMPLEADO
                    if (opener != null)
                    {
                        opener.RecargarComboSalud();
                    }
                }
            }
        }

        private void txtidIsapre_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtrutIsapre_KeyPress(object sender, KeyPressEventArgs e)
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

        private void gridIsapre_Click(object sender, EventArgs e)
        {
            //NUEVA SESION ACTIVIDAD
            Sesion.NuevaActividad();

            fnCargarIsapre();
        }
        private void gridIsapre_KeyUp(object sender, KeyEventArgs e)
        {
            //NUEVA SESION ACTIVIDAD
            Sesion.NuevaActividad();

            fnCargarIsapre();
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


        //PARA OCULTAR PESTAÑA SI NO TIENE ACCESO
        private void OcultaTab()
        {
            //OBTENEMOS LA COLECION DE PESTAÑAS QUE TIENE EL CONTROL
            XtraTabPageCollection tabs = tabMain.TabPages;
            string TabName = "";
            for (int i = 0; i < tabMain.TabPages.Count; i++)
            {
                /*RECORREMOS Y OBTENEMOS EL NOMBRE DE LA PESTAÑA*/
                TabName = tabMain.TabPages[i].Name.ToLower();               

                if (TabName == "tabafp")
                {
                    if (objeto.ValidaAcceso(User.GetUserGroup(), "frmafp") == false)
                    {
                        //tabMain.TabPages[i].PageVisible = false;
                        tabMain.TabPages[i].PageEnabled = false;
                    }                        
                }
                else if (TabName == "tabcajaprevision")
                {
                    if (objeto.ValidaAcceso(User.GetUserGroup(), "frmcaja") == false)
                    {
                        //tabMain.TabPages[i].PageVisible = false;
                        tabMain.TabPages[i].PageEnabled = false;
                    }
                        
                }
                else if (TabName == "tabisapre")
                {
                    if (objeto.ValidaAcceso(User.GetUserGroup(), "frmsalud") == false)
                    {
                        //tabMain.TabPages[i].PageVisible = false;
                        tabMain.TabPages[i].PageEnabled = false;
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
            if (page == "Afp")
            {
                busqueda = "SELECT id, nombre, rut, porcFondo, porcAdmin, porcOtro, claveExp, dato01, dato02 FROM afp WHERE id=@pId";
                cambio = fnCambiosPrev(txtId, txtNombre, txtRut, txtFondo, txtAdmin, txtOtro, txtClave,
                    txtdato1, txtdato2, busqueda);
                //SI CAMBIO ES TRUE SE CAMBIO ALGUN DATO
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
            else if (page == "Caja Prevision")
            {
                busqueda = "SELECT id, nombre, rut, porcPension, porcSalud, porcOtro, claveExp, dato01, dato02, porcAccidente FROM cajaPrevision WHERE id=@pId";
                cambio = fnCambiosPrevCaja(txtIdPrev, txtNombrePrev, txtRutPrev, txtPensionPrev, txtSaludPrev, txtOtroPrev, txtClavePrev,
                    txtDato1Prev, txtDato2Prev,txtAccidente, busqueda);
                //SI CAMBIO ES TRUE SE CAMBIO ALGUN DATO
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
            else if (page == "Salud")
            {
                busqueda = "SELECT id, nombre, dato01, dato02, rut FROM isapre WHERE id=@pId";
                cambio = fnCambiosIsapre(txtidIsapre, txtnombreIsapre, txtdato1Isapre, txtdato2Isapre,
                    txtrutIsapre, busqueda);
                //SI CAMBIO ES TRUE ES PORQUE SE CAMBIO ALGUN DATO EN ALGUN CAMPO
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
        }

        private void groupPrevision_Paint(object sender, PaintEventArgs e)
        {

        }

        //PARA MANEJAR LA TECLA TAB CUANDO SE PRESIONE MIENTRAS TIENE EL FOCO ALGUN TEXTEDIT
        protected override bool ProcessDialogKey(Keys keyData)
        {
            //PREGUNTAMOS SI EL CAMPO RUT DE LA PESTAÑA AFP TIENE EL FOCO
            string cadena = "", rutAntiguo = "";
            bool validaRut;
            if (keyData == Keys.Tab)
            {
                //RUT PREVISION AFP
                if (txtRut.ContainsFocus)
                {
                    //REALIZAMOS CODE DE VALIDACION
                    if (txtRut.Text == "") { lblrutAfp.Visible = true;lblrutAfp.Text = "rut no valido";return false;}
                    if (txtRut.Text.Contains(".") || txtRut.Text.Contains("-"))
                    {
                        //CADENA LE QUITAMOS LOS PUNTOS Y EL GUION
                        cadena = fnSistema.fnExtraerCaracteres(txtRut.Text);
                    }
                    else
                    {
                        //CADENA LA DEJAMOS TAL CUAL
                        cadena = txtRut.Text;
                    }

                    if (cadena.Length<8 || cadena.Length>9)
                    {
                        //CADENA NO VALIDA
                        lblrutAfp.Visible = true;
                        lblrutAfp.Text = "rut no valido";
                        //FORMATEAMOS DE TODAS MANERAS
                        txtRut.Text = fnSistema.fFormatearRut2(cadena);
                    }
                    else
                    {
                        //CADENA VALIDA
                        //VALIDAMOS RUT
                        validaRut = fnSistema.fValidaRut(cadena);
                        if (validaRut == false)
                        {
                            lblrutAfp.Visible = true;
                            lblrutAfp.Text = "rut no valido";
                            //FORMATEAMOS
                            txtRut.Text = fnSistema.fFormatearRut2(cadena);
                        }
                        else
                        {
                            //TODO CORRECTO
                            lblrutAfp.Visible = false;
                            txtRut.Text = fnSistema.fFormatearRut2(cadena);

                            if (txtId.ReadOnly == false)
                            {
                                //VERIFICAR SI EL RUT YA ESTA INGRESADO
                                if (ExisteRutPrevision(cadena))
                                { lblrutAfp.Visible = true; lblrutAfp.Text = "Rut ya ingresado"; return false; }
                                else                             
                                    lblrutAfp.Visible = false;
                            }
                            else
                            {
                                if (viewPrevision.RowCount>0)
                                {
                                    rutAntiguo = viewPrevision.GetFocusedDataRow()["rut"].ToString();
                                    if (rutAntiguo != cadena)
                                    {
                                        if (ExisteRutPrevision(cadena))
                                        { lblrutAfp.Visible = true; ; lblrutAfp.Text = "Rut ya ingresado"; return false; }
                                    }
                                    else
                                        lblrutAfp.Visible = false;
                                }
                            }                          
                        }
                    }
                }

                //RUT CAJA PREVISION
                if (txtRutPrev.ContainsFocus)
                {
                    //REALIZAMOS CODE DE VALIDACION
                    if (txtRutPrev.Text == "") { lblrutPrevision.Visible = true; lblrutPrevision.Text = "rut no valido"; return false; }
                    if (txtRutPrev.Text.Contains(".") || txtRutPrev.Text.Contains("-"))
                    {
                        //CADENA LE QUITAMOS LOS PUNTOS Y EL GUION
                        cadena = fnSistema.fnExtraerCaracteres(txtRutPrev.Text);
                    }
                    else
                    {
                        //CADENA LA DEJAMOS TAL CUAL
                        cadena = txtRutPrev.Text;
                    }

                    if (cadena.Length < 8 || cadena.Length > 9)
                    {
                        //CADENA NO VALIDA
                        lblrutPrevision.Visible = true;
                        lblrutPrevision.Text = "rut no valido";
                        //FORMATEAMOS DE TODAS MANERAS
                        txtRutPrev.Text = fnSistema.fFormatearRut2(cadena);
                    }
                    else
                    {
                        //CADENA VALIDA
                        //VALIDAMOS RUT
                        validaRut = fnSistema.fValidaRut(cadena);
                        if (validaRut == false)
                        {
                            lblrutPrevision.Visible = true;
                            lblrutPrevision.Text = "rut no valido";
                            //FORMATEAMOS
                            txtRutPrev.Text = fnSistema.fFormatearRut2(cadena);
                        }
                        else
                        {
                            //TODO CORRECTO
                            lblrutPrevision.Visible = false;
                            txtRutPrev.Text = fnSistema.fFormatearRut2(cadena);

                            if (txtIdPrev.ReadOnly == false)
                            {
                                //VERIFICAR SI RUT YA EXISTE
                                if (ExisteRutPrevision(cadena))
                                { lblrutPrevision.Visible = true; lblrutPrevision.Text = "Rut ingresado ya existe"; return false; }
                                else
                                { lblrutPrevision.Visible = false; }
                            }
                            else
                            {
                                if (viewPrev.RowCount>0)
                                {
                                    rutAntiguo = viewPrev.GetFocusedDataRow()["rut"].ToString();
                                    if (rutAntiguo != cadena)
                                    {
                                        if (ExisteRutCaja(cadena))
                                        { lblrutPrevision.Visible = true; lblrutPrevision.Text = "Rut ingresado ya existe"; return false; }

                                    }
                                    else
                                        lblrutPrevision.Visible = false;
                                }
                            }                           
                        }
                    }
                }

                //RUT ISAPRE
                if (txtrutIsapre.ContainsFocus)
                {
                    //REALIZAMOS CODE DE VALIDACION
                    if (txtrutIsapre.Text == "") { lblrutIsapre.Visible = true; lblrutIsapre.Text = "rut no valido"; return false; }
                    if (txtrutIsapre.Text.Contains(".") || txtrutIsapre.Text.Contains("-"))
                    {
                        //CADENA LE QUITAMOS LOS PUNTOS Y EL GUION
                        cadena = fnSistema.fnExtraerCaracteres(txtrutIsapre.Text);
                    }
                    else
                    {
                        //CADENA LA DEJAMOS TAL CUAL
                        cadena = txtrutIsapre.Text;
                    }

                    if (cadena.Length < 8 || cadena.Length > 9)
                    {
                        //CADENA NO VALIDA
                        lblrutIsapre.Visible = true;
                        lblrutIsapre.Text = "rut no valido";
                        //FORMATEAMOS DE TODAS MANERAS
                        txtrutIsapre.Text = fnSistema.fFormatearRut2(cadena);
                    }
                    else
                    {
                        //CADENA VALIDA
                        //VALIDAMOS RUT
                        validaRut = fnSistema.fValidaRut(cadena);
                        if (validaRut == false)
                        {
                            lblrutIsapre.Visible = true;
                            lblrutIsapre.Text = "rut no valido";
                            //FORMATEAMOS
                            txtrutIsapre.Text = fnSistema.fFormatearRut2(cadena);
                        }
                        else
                        {
                            //TODO CORRECTO
                            lblrutIsapre.Visible = false;
                            txtrutIsapre.Text = fnSistema.fFormatearRut2(cadena);

                            if (txtidIsapre.ReadOnly == false)
                            {
                                //VERIFICAR SI EXISTE EL RUT EN TABLA
                                if (ExisteRutIsapre(cadena))
                                { lblrutIsapre.Visible = true; lblrutIsapre.Text = "Rut ingresado ya existe"; return false; }
                                else
                                { lblrutIsapre.Visible = false; }
                            }
                            else
                            {
                                if (viewIsapre.RowCount>0)
                                {
                                    rutAntiguo = viewIsapre.GetFocusedDataRow()["rut"].ToString();
                                    if (rutAntiguo != cadena)
                                    {
                                        if (ExisteRutIsapre(cadena))
                                        { lblrutIsapre.Visible = true; lblrutIsapre.Text = "Rut ingresado ya existe"; txtrutIsapre.Focus(); return false; }
                                    }
                                    else
                                        lblrutIsapre.Visible = false;
                                }
                            }                           
                        }
                    }
                }

                //NOMBRE AFP
                if (txtNombre.ContainsFocus)
                {
                    cadena = txtNombre.Text;
                    string nombreAntiguo = "";
                    if (txtId.ReadOnly == false)
                    {
                        if (ExistePrevision(cadena))
                        { XtraMessageBox.Show("Ya existe afp con ese nombre, por favor verifica la informacion y vuelve a intentarlo", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); return false; }
                    }
                    else
                    {
                        if (viewPrevision.RowCount > 0)
                        {
                            nombreAntiguo = viewPrevision.GetFocusedDataRow()["nombre"].ToString();
                            if (nombreAntiguo != cadena)
                            {
                                if(ExistePrevision(cadena))
                                { XtraMessageBox.Show("Ya existe afp con ese nombre, por favor verifica la informacion y vuelve a intentarlo", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); return false; }
                            }
                        }
                    }                    
                }

                //NOMBRE ISAPRE
                if (txtnombreIsapre.ContainsFocus)
                {
                   if (txtnombreIsapre.Text == "") { XtraMessageBox.Show("Por favor ingresa un nombre", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);return false;}
                    string nombreAntiguo = "";
                    if (txtidIsapre.ReadOnly == false)
                    {
                        cadena = txtnombreIsapre.Text;
                        if (fnVerificarNombreIsapre(cadena))
                        { XtraMessageBox.Show("Nombre ya existe", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); return false; }
                    }
                    else
                    {
                        if (viewIsapre.RowCount>0)
                        {
                            nombreAntiguo = viewIsapre.GetFocusedDataRow()["nombre"].ToString();
                            if (nombreAntiguo != txtnombreIsapre.Text)
                            {
                                if (fnVerificarNombreIsapre(txtnombreIsapre.Text))
                                { XtraMessageBox.Show("Nombre ya existe", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);txtnombreIsapre.Focus();return false; }
                            }
                        }
                    }                    
                }

                if (txtNombrePrev.ContainsFocus)
                {                   
                    if (txtNombrePrev.Text == "") { XtraMessageBox.Show("Por favor ingresa un nombre", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);return false; }
                    string NombreAntiguo = "";
                    if (txtIdPrev.ReadOnly == false)
                    {
                        cadena = txtNombrePrev.Text;
                        if (ExisteNombreCaja(cadena))
                        { XtraMessageBox.Show("Nombre ya registrado", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); return false; }
                    }
                    else
                    {
                        if (viewPrev.RowCount>0)
                        {
                            NombreAntiguo = viewPrev.GetFocusedDataRow()["nombre"].ToString();
                            if (NombreAntiguo != txtNombrePrev.Text)
                            {
                                if (ExisteNombreCaja(txtNombrePrev.Text))
                                { XtraMessageBox.Show("Nombre ingresado ya existe", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);return false;}
                            }
                        }
                    }                    
                }
            }
            return base.ProcessDialogKey(keyData);
        }

        private void txtRut_KeyDown(object sender, KeyEventArgs e)
        {
           
            //VALIDAR RUT CUANDO SE PRESIONE LA TECLA ENTER
            if (e.KeyData == Keys.Enter)
            {
                ValidaCajaRut();              
                
            }
        }

        private void ValidaCajaRut()
        {
            string cadena = "";
            bool rutValida = false;

            if (txtRut.Text == "")
            {
                lblrutAfp.Visible = true;
                lblrutAfp.Text = "rut no valido";
                //FORMATEAMOS
                txtRut.Text = fnSistema.fFormatearRut2(txtRut.Text);
                return;
            }
            if (txtRut.Text.Contains(".") || txtRut.Text.Contains("-"))
            {
                //quitamos punto y guion
                cadena = fnSistema.fnExtraerCaracteres(txtRut.Text);

            }
            else
            {
                cadena = txtRut.Text;
            }

            if (cadena.Length < 8 || cadena.Length > 9) { lblrutAfp.Visible = true; lblrutAfp.Text = "rut no valido"; return; }
            //validamos rut
            rutValida = fnSistema.fValidaRut(cadena);
            if (rutValida == false)
            {
                lblrutAfp.Visible = true;
                lblrutAfp.Text = "rut no valido";
                //formateamos
                txtRut.Text = fnSistema.fFormatearRut2(cadena);
            }
            else
            {
                //RUT VALIDO
                lblrutAfp.Visible = false;
                txtRut.Text = fnSistema.fFormatearRut2(cadena);

            }
        }

        private void txtRutPrev_KeyDown(object sender, KeyEventArgs e)
        {        
            //VALIDAR RUT CUANDO SE PRESIONE LA TECLA ENTER
            if (e.KeyData == Keys.Enter)
            {
                RutPrevValida();
            }
        }

        private void RutPrevValida()
        {
            string cadena = "", rutAntiguo = "";
            bool rutValida;

            if (txtRutPrev.Text == "")
            {
                lblrutPrevision.Visible = true;
                lblrutPrevision.Text = "rut no valido";
                //FORMATEAMOS
                txtRutPrev.Text = fnSistema.fFormatearRut2(txtRutPrev.Text);
                return;
            }
            if (txtRutPrev.Text.Contains(".") || txtRutPrev.Text.Contains("-"))
            {
                //quitamos punto y guion
                cadena = fnSistema.fnExtraerCaracteres(txtRutPrev.Text);
            }
            else
            {
                cadena = txtRutPrev.Text;
            }

            if (cadena.Length < 8 || cadena.Length > 9) { lblrutPrevision.Visible = true; lblrutPrevision.Text = "rut no valido"; return; }
            //validamos rut
            rutValida = fnSistema.fValidaRut(cadena);
            if (rutValida == false)
            {
                lblrutPrevision.Visible = true;
                lblrutPrevision.Text = "rut no valido";
                //formateamos
                txtRutPrev.Text = fnSistema.fFormatearRut2(cadena);
            }
            else
            {
                //RUT VALIDO
                // lblrutPrevision.Visible = false;
                txtRutPrev.Text = fnSistema.fFormatearRut2(cadena);

                if (txtIdPrev.ReadOnly == false)
                {
                    //VERIFICAR SI RUT YA EXISTE
                    if (ExisteRutPrevision(cadena))
                    { lblrutPrevision.Visible = true; lblrutPrevision.Text = "Rut ya ingresado"; return; }
                    else
                    { lblrutPrevision.Visible = false; }
                }
                else
                {
                    if (viewPrev.RowCount > 0)
                    {
                        rutAntiguo = viewPrev.GetFocusedDataRow()["rut"].ToString();
                        if (rutAntiguo != cadena)
                        {
                            if (ExisteRutCaja(cadena))
                            { lblrutPrevision.Visible = true; lblrutPrevision.Text = "Rut ingresado ya existe"; return; }

                        }
                        else
                            lblrutPrevision.Visible = false;
                    }
                }
            }
        }

        private void txtrutIsapre_KeyDown(object sender, KeyEventArgs e)
        {
            //VALIDAR RUT CUANDO SE PRESIONE LA TECLA ENTER
            if (e.KeyData == Keys.Enter)
            {
                ValidaRutIsapre();
            }
        }

        private void ValidaRutIsapre()
        {
            string cadena = "";
            bool rutValida;

            if (txtrutIsapre.Text == "")
            {
                lblrutIsapre.Visible = true;
                lblrutIsapre.Text = "rut no valido";
                //FORMATEAMOS
                txtrutIsapre.Text = fnSistema.fFormatearRut2(txtrutIsapre.Text);
                return;
            }
            if (txtrutIsapre.Text.Contains(".") || txtrutIsapre.Text.Contains("-"))
            {
                //quitamos punto y guion
                cadena = fnSistema.fnExtraerCaracteres(txtrutIsapre.Text);

            }
            else
            {
                cadena = txtrutIsapre.Text;
            }

            if (cadena.Length < 8 || cadena.Length > 9) { lblrutIsapre.Visible = true; lblrutIsapre.Text = "rut no valido"; return; }
            //validamos rut
            rutValida = fnSistema.fValidaRut(cadena);
            if (rutValida == false)
            {
                lblrutIsapre.Visible = true;
                lblrutIsapre.Text = "rut no valido";
                //formateamos
                txtrutIsapre.Text = fnSistema.fFormatearRut2(cadena);
            }
            else
            {
                //RUT VALIDO
                lblrutIsapre.Visible = false;
                txtrutIsapre.Text = fnSistema.fFormatearRut2(cadena);

            }
        }

        private void txtNombre_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) || char.IsSeparator(e.KeyChar))
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

        private void txtnombreIsapre_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
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

        private void txtNombrePrev_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void txtdato1_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtdato2_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtdato1Isapre_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtDato1Prev_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtDato2Prev_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void txtdato2Isapre_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtId_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtNombre_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtRut_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtFondo_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtAdmin_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtOtro_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtClave_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtdato1_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtdato2_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtidIsapre_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtnombreIsapre_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtrutIsapre_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtdato1Isapre_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtdato2Isapre_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtIdPrev_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtNombrePrev_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtRutPrev_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtPensionPrev_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtSaludPrev_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtAccidente_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtOtroPrev_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtClavePrev_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtDato1Prev_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtDato2Prev_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            string busqueda = "";
            int id = 0;
            busqueda = "SELECT id, nombre, rut, porcFondo, porcAdmin, porcOtro, claveExp, dato01, dato02 FROM afp WHERE id=@pId";
            if (viewPrevision.RowCount > 0)
            {
                id = Convert.ToInt32(viewPrevision.GetFocusedDataRow()["id"]);

                if (fnCambiosPrev(txtId, txtNombre, txtRut, txtFondo, txtAdmin, txtOtro, txtClave, txtdato1, txtdato2, busqueda))
                {
                    DialogResult adv = XtraMessageBox.Show("Hay cambios sin guardar, ¿Deseas cerrar de todas maneras?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (adv == DialogResult.Yes)
                        Close();
                }
                else
                    Close();
            }
            else
                Close();

         
        }

        private void btnSalirSalud_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD   
            Sesion.NuevaActividad();

            string busqueda = "";
            int id = 0;
            busqueda = "SELECT id, nombre, dato01, dato02, rut FROM isapre WHERE id=@pId";

            if (viewIsapre.RowCount > 0)
            {
                id = Convert.ToInt32(viewIsapre.GetFocusedDataRow()["id"]);

                if (fnCambiosIsapre(txtidIsapre, txtnombreIsapre, txtdato1Isapre, txtdato2Isapre,
                txtrutIsapre, busqueda))
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

        private void btnSalirPrevision_Click(object sender, EventArgs e)
        {
            //NUEVA SESION
            Sesion.NuevaActividad();

            string busqueda = "";
            int id = 0;

            busqueda = "SELECT id, nombre, rut, porcPension, porcSalud, porcOtro, claveExp, dato01, dato02, porcAccidente FROM cajaPrevision WHERE id=@pId";

            if (viewPrevision.RowCount > 0)
            {
                id = Convert.ToInt32(viewPrevision.GetFocusedDataRow()["id"]);

                if (fnCambiosPrevCaja(txtIdPrev, txtNombrePrev, txtRutPrev, txtPensionPrev, txtSaludPrev, txtOtroPrev, txtClavePrev,
                txtDato1Prev, txtDato2Prev, txtAccidente, busqueda))
                {
                    DialogResult adv = XtraMessageBox.Show("Hay cambios sin guardar, ¿Deseas cerrar de todas formas?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (adv == DialogResult.Yes)
                    {
                        Close();
                    }
                }
                else
                    Close();
            }
            else
                Close();

        }

        private void txtNombre_KeyDown(object sender, KeyEventArgs e)
        {
            string name = "", nombreAntiguo = "";
            //VALIDAR QUE NOMBRE QUE SE INTENTAR INGRESAR NO EXISTE PARA OTRO REGISTRO
            if (e.KeyData == Keys.Enter)
            {
                if (txtNombre.Text == "") { XtraMessageBox.Show("Por favor ingresar un nombre", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);return; }

                if (txtId.ReadOnly == false)
                {
                    name = txtNombre.Text;
                    //VERIFICAMOS...
                    if (ExistePrevision(name))
                    { XtraMessageBox.Show("Ya existe afp con ese nombre, verificar la informacion y vuelve a intentarlo", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
                }
                else
                {
                    if (viewPrevision.RowCount>0)
                    {
                        nombreAntiguo = viewPrevision.GetFocusedDataRow()["nombre"].ToString();
                        if (nombreAntiguo != txtNombre.Text)
                        {
                            if (ExistePrevision(txtNombre.Text))
                            { XtraMessageBox.Show("Nombre ingresado ya existe", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);return;}
                        }
                    }
                }
            }
        }

        private void txtnombreIsapre_KeyDown(object sender, KeyEventArgs e)
        {
            string name = "", nombreAntiguo = "";
            /*if (e.KeyData == Keys.Enter)
            {
                if (txtnombreIsapre.Text == "") { XtraMessageBox.Show("Por favor ingresa un nombre", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);txtnombreIsapre.Focus(); return;}

                if (txtidIsapre.ReadOnly == false)
                {
                    name = txtnombreIsapre.Text;

                    if (fnVerificarNombreIsapre(name))
                    { XtraMessageBox.Show("Nombre ya registrado, por favor verificar la informacion y vuelve a intentarlo", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);txtnombreIsapre.Focus(); return; }
                }
                else
                {
                    if (viewIsapre.RowCount>0)
                    {
                        nombreAntiguo = viewIsapre.GetFocusedDataRow()["nombre"].ToString();
                        if (nombreAntiguo != txtnombreIsapre.Text)
                        {
                            if (fnVerificarNombreIsapre(txtnombreIsapre.Text))
                            { XtraMessageBox.Show("Nombre ya registrado, por favor verifica la informacion y vuelve a intentarlo", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);txtnombreIsapre.Focus();return; }
                        }
                    }
                }               
            }*/
        }

        private void txtRutPrev_InvalidValue(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e)
        {

        }

        private void groupAfp_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tabMain_Selected(object sender, DevExpress.XtraTab.TabPageEventArgs e)
        {
            
        }

        private void tabCajaPrevision_Paint(object sender, PaintEventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void txtRut_Leave(object sender, EventArgs e)
        {
            ValidaCajaRut();
        }

        private void txtrutIsapre_Leave(object sender, EventArgs e)
        {
            ValidaRutIsapre();
        }

        private void txtRutPrev_Leave(object sender, EventArgs e)
        {
            RutPrevValida();
        }
    }
}