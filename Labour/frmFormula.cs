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
using DevExpress.Utils.Menu;
using System.Collections;
using System.Runtime.InteropServices;

namespace Labour
{
    public partial class frmFormula : DevExpress.XtraEditors.XtraForm, IPlantillaFormula, IVarFormula, IFormBusqueda
    {
        [DllImport("user32")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32")]
        static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        const int MF_BYCOMMAND = 0;
        const int MF_DISABLED = 2;
        const int SC_CLOSED = 0xF060;

        //PARA COMUNICACION DESACOPLADA CON FORM ITEM
        public IFormulaItem opener;

        //variable para recargar grilla de acuerdo al tipo(haber, descuento, familiar, etc)
        public int Tipo { get; set; }

        //VARIABLE PARA EL CODIGO DE FORMULA
        public string CodigoFormula { get; set; } = "";

        //BOTON NUEVO
        Operacion op;

        #region "interfaz"
        public void CargarPlantilla(string cadena)
        {           
            txtExpresion.Text = cadena;
        }
        #endregion

        #region "INTERFAZ FORMULA VARIABLE"
        public void CargarVariableMemo(int position, string formula)
        {
            txtExpresion.Text = txtExpresion.Text.Insert(position, formula);
            txtExpresion.Select(position, formula.Length);
            txtExpresion.ScrollToCaret();
        }
        #endregion

        #region "INTERFAZ FORMULA BUSQUEDA"
        public void CargarDatos(string codigo)
        {
            fnCargarBusqueda(codigo);
        }
        #endregion

        public frmFormula()
        {
            InitializeComponent();
        }

        //CONSTRUCTOR PARAMETRIZADO SOLO EN CASO DE QUE SE INVOQUE EL FORMULARIO DESDE EL FORMULARIO ITEM
        public frmFormula(int Tipo, string CodigoFormula)
        {
            InitializeComponent();
            this.Tipo = Tipo;
            this.CodigoFormula = CodigoFormula;
        }

        //PARA MANEJAR LA POSICION EN LA LISTA DE CODIGOS
        private int posicion = 0;

        private void frmFormula_Load(object sender, EventArgs e)
        {
            var sm = GetSystemMenu(Handle, false);
            EnableMenuItem(sm, SC_CLOSED, MF_BYCOMMAND | MF_DISABLED);

            fnDefaultProperties();

            op = new Operacion();

            //PARA CARGAR GRILLA BUSQUEDA...
            string grilla = "SELECT codFormula, descFormula, sistema, valor FROM formula WHERE codFormula <> '0'";
            fnSistema.spllenaGridView(gridBusqueda, grilla);
            fnSistema.spOpcionesGrilla(viewBusqueda);
            fnColumnas();

            CargarDataFromGrilla();
        }
        #region "MANEJO DE DATOS"
        //INGRESO FORMULA EN BD
        //VALOR DE ENTRADA --> 
        /*
         * CODIGO FORMULA
         * DESCRIPCION FORMULA
         * VALOR (CADENA DE EXPRESION CON LA FORMULA COMPLETA)
         */
        private void fnIngresoFormula(TextEdit pCod, TextEdit pDesc, MemoEdit pExpresion, CheckEdit pSistema)
        {
            //CODE SQL
            string sql = "INSERT INTO formula(codFormula, descFormula, valor, sistema) values(@pCod, @pDesc, @pExpresion, @pSistema)";

            //FALTA AGREGAR CAMPO SISTEMA
            /*--------------------------------------------------------------------------------------------------
             * EL CAMPO SISTEMA SIGUE LA SIGUIENTE LOGICA:                                                      |
             * @CASE1 - ES VARIABLE DE SISTEMA POR LO TANTO NO DEBE SER EDITABLE POR EL USUARIO (VALUE 1)       |
             * @CASE2 - NO ES VARIABLE DE SISTEMA POR LO TANTO SI SE PUEDE EDITAR POR EL USUARIO (VALUE 0)      |
             * -------------------------------------------------------------------------------------------------
             */

            /*
             * CADA FORMULA (SIEMPRE Y CUANDO NO SEA DE SISTEMA) SERA VISUALIZADA COMO COMBOBOX EN EL FORMULARIO ITEMS
             */

            SqlCommand cmd;
            int res = 0;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //parametros
                        //EL CAMPO CODIGO DEBE ESTAR PRECEDIDO DE LA LETRA F
                        //EJEMPLO: F + NOMBRE FORMULA
                        cmd.Parameters.Add(new SqlParameter("@pCod", "F" + pCod.Text.ToUpper()));
                        cmd.Parameters.Add(new SqlParameter("@pDesc", pDesc.Text.ToUpper()));
                        cmd.Parameters.Add(new SqlParameter("@pExpresion", pExpresion.Text));
                        cmd.Parameters.Add(new SqlParameter("@pSistema", pSistema.EditValue));

                        //ejecutar consulta
                        res = cmd.ExecuteNonQuery();
                        if (res >0)
                        {                         
                            XtraMessageBox.Show("Ingreso correcto", "Ingreso", MessageBoxButtons.OK,MessageBoxIcon.Information);

                            //GUARDAR EVENTO EN LOG
                            logRegistro log = new logRegistro(User.getUser(), "SE INGRESA NUEVA FORMULA CON CODIGO " + "F" + pCod.Text.ToUpper(), "FORMULA", "0", "F"+pCod.Text.ToUpper(), "INGRESO");
                            log.Log();

                            //RECARGAR GRILLA
                            string grilla = "SELECT codFormula, descFormula, sistema, valor FROM formula WHERE codFormula <> '0'";
                            fnSistema.spllenaGridView(gridBusqueda, grilla);
                            fnSistema.spOpcionesGrilla(viewBusqueda);
                            fnColumnas();

                            btnPlantilla.Enabled = false;

                            CargarDataFromGrilla();                           
                        }
                        else
                        {
                            XtraMessageBox.Show("Error al intentar guardar", "Ingreso", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        //MODIFICACION FORMULA
        /*
         * PARAMETROS DE ENTRADA:
         * CODIGO FORMULA
         * DESCRIPCION FORMULA
         * VALOR (CADENA DE EXPRESION CON LA FORMULA COMPLETA)
         */
        private void fnModificarFormula(TextEdit pCod, TextEdit pDesc, MemoEdit pExpresion, CheckEdit pSistema)
        {
            //sql update
            string sql = "UPDATE formula set descFormula=@pDesc, valor=@pExpresion, sistema=@pSistema WHERE codFormula=@pCod";
            SqlCommand cmd;
            int res = 0;

            //TABLA HASH PARA LOG
            Hashtable datosFormula = new Hashtable();
            datosFormula = PrecargaDatosFormula(pCod.Text.ToUpper());

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS                      
                        cmd.Parameters.Add(new SqlParameter("@pCod", pCod.Text.ToUpper()));
                        cmd.Parameters.Add(new SqlParameter("@pDesc", pDesc.Text.ToUpper()));
                        cmd.Parameters.Add(new SqlParameter("@pExpresion", pExpresion.Text));
                        cmd.Parameters.Add(new SqlParameter("@pSistema", pSistema.EditValue));
                        //EJECUTAMOS CONSULTA
                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {                         
                            XtraMessageBox.Show("Actualizacion correcta", "Actualizar", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            //SI HAY CAMBIOS GUARDAMOS EVENTOS EN LOG
                            ComparaValorFormula(datosFormula, pDesc.Text, pExpresion.Text, (bool)pSistema.EditValue, pCod.Text.ToUpper());

                            string grilla = "SELECT codFormula, descFormula, sistema, valor FROM formula WHERE codFormula <> '0'";
                            fnSistema.spllenaGridView(gridBusqueda, grilla);
                            fnSistema.spOpcionesGrilla(viewBusqueda);
                            fnColumnas();
                            btnPlantilla.Enabled = false;

                            CargarDataFromGrilla();
                        }
                        else
                        {
                            XtraMessageBox.Show("Error al intentar actualizar", "Actualizar", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        //ELIMINAR UNA FORMULA
        /*
         * PARAMETROS DE ENTRADA:
         * CODIGO FORMULA
         */
        private void fnEliminarFormula(TextEdit pCod)
        {
            //SQL DELETE
            string sql = "DELETE FROM formula where codFormula=@pCod";
            SqlCommand cmd;
            int res = 0;

            //SI UNA FORMULA ES DE SISTEMA NO SE PUEDE ELIMINAR...
            if (EsFormulaSistema(pCod.Text)) { XtraMessageBox.Show("No puedes eliminar una formula de sistema", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (fnFormulaenUso(pCod.Text))
            { XtraMessageBox.Show("No puedes eliminar esta formula porque está en uso", "Formula", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (fnFormulaUsadaTrabajador(pCod.Text))
            { XtraMessageBox.Show("No puedes eliminar esta formula porque está en uso", "Formula", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            //Formula en uso???

            //ELIMINAMOS...
            DialogResult adv = XtraMessageBox.Show($"¿Deseas eliminar la formula {pCod.Text}?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (adv == DialogResult.Yes)
            {
                //ELIMINAMOS
                try
                {
                    if (fnSistema.ConectarSQLServer())
                    {
                        using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@pCod", pCod.Text));
                            //EJECUTAR CONSULTA
                            res = cmd.ExecuteNonQuery();
                            if (res > 0)
                            {
                                XtraMessageBox.Show("Eliminacion correcta", "Eliminar", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                //GUARDAR REGISTRO EN LOG
                                logRegistro log = new logRegistro(User.getUser(), "REGISTRO ELIMINADO CON CODIGO " + pCod.Text, "FORMULA", pCod.Text, "", "ELIMINAR");
                                log.Log();

                                string grilla = "SELECT codFormula, descFormula, sistema, valor FROM formula WHERE codFormula <> '0'";
                                fnSistema.spllenaGridView(gridBusqueda, grilla);
                                fnSistema.spOpcionesGrilla(viewBusqueda);
                                fnColumnas();

                                CargarDataFromGrilla();
                                btnPlantilla.Enabled = false;

                            }
                            else
                            {
                                XtraMessageBox.Show("Error al intentar eliminar", "Eliminar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        //LIBERAR RECURSOS
                        cmd.Dispose();
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
            }
        }

        //ELIMINAR ITEMS ASOCIADO A FORMULA EN CASO DE QUE SE DESEE ELIMINAR ALGUN CODIGO DESDE FORMULA
        //PARAMETRO DE ENTRADA: NECESITA EL CODIGO DE LA FORMULA
        //RETORNA TRUE SI SE ELIMINARON CORRECTAMENTE LOS REGISTROS
        //RETORNA FALSE CASO CONTRARIO
        private bool fnEliminarItemsFormula(string pCodAsociado)
        {
            //DELETE ITEMS
            string sql = "DELETE FROM item WHERE formula=@pCodAsociado";
            SqlCommand cmd;
            int res = 0;

            bool eliminado = false;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS 
                        cmd.Parameters.Add(new SqlParameter("@pCodAsociado", pCodAsociado));

                        //EJECUTAMOS CONSULTA
                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            //SE ELIMINO CORRECTAMENTE
                            //RETORNAMOS TRUE
                            eliminado = true;
                        }
                        else
                        {
                            eliminado = false;
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

            return eliminado;
        }

        //MODIFICAR ITEM QUE TENGAN ASOCIADO ESE CODIGO DE FORMULA
        private bool fnModificarItemAsociado(string pCodAsociado)
        {
            string sql = "UPDATE item SET formula='0' WHERE formula=@pCodAsociado";            
            SqlCommand cmd;
            int res = 0;
            bool mod = false;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {                       
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pCodAsociado", pCodAsociado));

                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            mod = true;
                        }
                        else
                        {
                            mod = false;
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

            return mod;
        }

        //GUARDAR TODOS LOS COD DE FORMULAS EN ARRAY
        //PARAMETROS DE ENTRADA: NOTHING
        //RETORNA UNA LISTA CON LOS CODIGOS
        private List<string> fnCodigoFormulas()
        {
            //SQL SEARCH
            string sql = "SELECT codFormula FROM formula WHERE codFormula <> '0'";
            SqlCommand cmd;
            SqlDataReader rd;
            //LISTA PARA GUARDAR LOS CODIGOS
            List<string> codigos = new List<string>();
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //SI HAY FILAS CARGAMOS la lista
                            while (rd.Read())
                            {
                                codigos.Add(rd["codFormula"].ToString());
                            }
                        }
                        else
                        {
                            XtraMessageBox.Show("No se encontraron resultados", "Busqueda", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            return codigos;
        }

        //CARGAR CAMPOS DESDE BD USANDO LOS CODIGOS OBTENIDOS DESDE EL METODO ANTERIOR
        //PARAMETROS DE ENTRADA CODIGO FORMULA(VIENE DE LA LISTA PRECARGADA CON TODOS LOS CODIGOS EXISTENTEN EN BD)
        private void fnCargarCampos(string pCod)
        {
            //SQL SEARCH
            string sql = "SELECT codFormula, descFormula, valor, sistema FROM formula WHERE codFormula=@pCod";
            SqlCommand cmd;
            SqlDataReader rd;
            btnPlantilla.Enabled = false;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PAREMTROS
                        cmd.Parameters.Add(new SqlParameter("@pCod", pCod));
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //cargamos campos
                            while (rd.Read())
                            {
                                txtcodigo.ReadOnly = true;
                                txtcodigo.Text = (string)rd["codFormula"];
                                txtDescripcion.Text = (string)rd["descFormula"];
                                txtExpresion.Text = (string)rd["valor"];
                                chSistema.EditValue = (bool)rd["sistema"];

                                if ((bool)rd["sistema"])
                                {
                                    btnEliminar.Enabled = false;
                                    btnGuardar.Enabled = false;
                                }                                    
                                else
                                {
                                    btnGuardar.Enabled = true;
                                    btnEliminar.Enabled = true;
                                }
                                    
                            }
                        }
                        else
                        {
                            XtraMessageBox.Show("No se encontraron registros", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        }

        //VERIFICAR CODIGO FORMULA EXISTE EN BD
        //PARAMETROS DE ENTRADA: CODIGO FORMULA
        //RETORNA TRUE EN CASO DE QUE EL CODIGO EXISTA EN BD
        private bool fnVerificarCodigo(string pCod)
        {
            //SQL 
            string sql = "SELECT * FROM formula WHERE codFormula=@pCod";
            SqlCommand cmd;
            SqlDataReader rd;
            bool encontrado = false;
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
                            //SI TIENE FILAS RETORNAMOS TRUE
                            encontrado = true;
                        }
                        else
                        {
                            //SI NO TIENE RETUR FALSE
                            encontrado = false;
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
            return encontrado;
        }

        //COMPARA VALORES CON LOS DE LA BASE DE DATOS PARA VER SI SE HAN HECHO CAMBIOS SIN GUARDAR
        //RETORNA TRUE SI SE HA MODIFICADO ALGUN CAMPOS
        private bool fnVerificarCambios(string pCod)
        {
            //string sql
            string sql = "SELECT codFormula, descFormula, valor FROM formula WHERE codFormula=@pCod";
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
                                //COMPARAMOS VALORES                                
                                if (txtcodigo.Text != rd["codFormula"].ToString()) return true;
                                if (txtDescripcion.Text != rd["descFormula"].ToString()) return true;
                                if (txtExpresion.Text != rd["valor"].ToString()) return true;
                            }
                        }
                        else
                        {
                            XtraMessageBox.Show("No hay registros", "Registros", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            return false;
        }

        //COMPARA CAMPOS VACIOS
        private bool fnCamposVacios()
        {
            if (txtcodigo.Text != "") return true;
            if (txtDescripcion.Text != "") return true;
            if (txtExpresion.Text != "") return true;

            return false;
        }

        //DEFAULT PROPERTIES
        private void fnDefaultProperties()
        {
            txtcodigo.Properties.MaxLength = 6;
            txtDescripcion.Properties.MaxLength = 50;
            txtExpresion.Properties.MaxLength = 300;
            btnEliminar.TabStop = false;
            btnNuevo.TabStop = false;
            btnGuardar.TabStop = false;         
            btnHelp.TabStop = false;
            //btnVariables.TabStop = false;
            txtExpresion.TabStop = false;
            chSistema.TabStop = false;
            //txtExpresion.Properties.ScrollBars = ScrollBars.None;
        }

        //LIMPIAR CAMPOS
        private void fnLimpiarCampos()
        {
            txtcodigo.Focus();
            txtcodigo.Text = "";                        
            txtcodigo.ReadOnly = false;
            txtDescripcion.Text = "";
            txtExpresion.Text = "";
            lblerror.Visible = false;
            btnPlantilla.Enabled = true;
            btnGuardar.Enabled = true;            
        }

        //VERIFICAR QUE ALGUNA FORMULA NO ESTA EN USO EN ALGUN ITEM
        //RETORNA TRUE EN CASO DE ENCONTRAR ALGUN REGISTRO EN ITEM
        //RETORNA FALSE SI NO ENCUENTRA NADA
        private bool fnFormulaenUso(string pCodFormula)
        {
            //SQL CONSULTA
            string sql = "SELECT * FROM item WHERE formula=@pCodFormula";
            SqlCommand cmd;
            SqlDataReader rd;
            bool usado = false;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pCodFormula", pCodFormula));

                        //EJCUTAMOS
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //SI ENCONTRO RESULTADO, LA FORMULA SE ESTA UTILIZANDO
                            usado = true;
                        }
                        else
                        {
                            usado = false;
                        }
                    }
                    //LIBERAMOS RECURSOS
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
            return usado;
        }

        //VERIFICAR SI FORMULA ESTA SIENDO USADA POR ITEM TRABAJADOR
        private bool fnFormulaUsadaTrabajador(string pCodFormula)
        {
            string sql = "SELECT itemTrabajador.formula FROM itemTrabajador " +
                         " WHERE itemTrabajador.formula = @formula ";
            bool usada = false;
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@formula", pCodFormula));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //SI TIENE FILA ESTA EN USO
                            usada = true;
                        }
                        else
                        {
                            usada = false;
                        }
                    }
                    //LIBERAR RECURSOS
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                    rd.Close();                   
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
            return usada;
        }

        //ACTUALIZAR CAMPO VALOR EN ITEM TRABAJADOR (PARA EL CASO DE QUE EXISTE LA RELACION)
        private bool fnActualizarValorItem(double value, string formula)
        {
            string sql = "UPDATE itemtrabajador SET valor=@pvalor WHERE formula=@pformula AND formula <> '0'";
            SqlCommand cmd;
            bool exito = false;
            int res = 0;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pformula", formula));
                        cmd.Parameters.Add(new SqlParameter("@pvalor", value));

                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            exito = true;
                        }
                        else
                        {
                            exito = false;
                        }
                    }
                    //LIBERAR RECURSOIS
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();                   
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return exito;
        }

        //CARGAR FORMULA DESDE FORM BUSQUEDA FORMULA
        private void fnCargarBusqueda(string codigo)
        {
            string sql = "SELECT codFormula, descFormula, valor, sistema FROM formula WHERE codFormula=@pcodigo";
            SqlCommand cmd;
            SqlDataReader rd;
            btnPlantilla.Enabled = false;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pcodigo", codigo));

                        //EJECUTAR 
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //CARGARMOS CAMPOS
                                txtcodigo.ReadOnly = true;
                                txtcodigo.Text = (string)rd["codFormula"];
                                txtDescripcion.Text = (string)rd["descFormula"];
                                txtExpresion.Text = (string)rd["valor"];
                                chSistema.Checked = (bool)rd["sistema"];                                
                            }
                        }
                        else
                        {
                            XtraMessageBox.Show("No se encontraron registros", "Registros", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    //LIBERAR RECURSO
                    cmd.Dispose();
                    rd.Close();
                    fnSistema.sqlConn.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        //VERIFICAR SI EL REGISTROS ES UNA FORMULA DE SISTEMA
        private bool EsFormulaSistema(string code)
        {
            if (code.Length>0)
            {
                string sql = "SELECT sistema FROM formula WHERE codformula=@code";
                SqlCommand cmd;
                bool sis = false;

                try
                {
                    if (fnSistema.ConectarSQLServer())
                    {
                        using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@code", code));
                            sis = (bool)cmd.ExecuteScalar();

                            cmd.Dispose();
                            fnSistema.sqlConn.Close();

                            if (sis)
                                return true;
                            else
                                return false;
                        }
                    }
                }
                catch (SqlException ex)
                {
                    XtraMessageBox.Show(ex.Message);
                }
            }

            return false;
        }

        #region "MANEJO DE DATOS BUSQUEDA FORMULA"
        //METODO PARA REALIZAR BUSQUEDA
        private void fnBusquedaFormula(TextEdit pBusqueda)
        {
            string sql = string.Format("SELECT codFormula, descFormula, sistema, valor FROM formula WHERE codFormula <> '0' AND (codFormula LIKE '%{0}%' OR descFormula LIKE '%{1}%')", pBusqueda.Text, pBusqueda.Text);
            fnRecargarGrilla(gridBusqueda, sql);

        }

        private void fnRecargarGrilla(DevExpress.XtraGrid.GridControl pGrid, string pSql)
        {
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    DataSet ds = new DataSet();

                    SqlCommand cmd = new SqlCommand(pSql, fnSistema.sqlConn);
                    //parametros
                    //cmd.Parameters.Add(new SqlParameter(pCampo, pDato));
                    adapter.SelectCommand = cmd;
                    adapter.Fill(ds);
                    adapter.Dispose();
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        pGrid.DataSource = ds.Tables[0];
                        fnLimpiarBusqueda();
                        int filas = ds.Tables[0].Rows.Count;

                        //RECARGAR GRILLA
                        CargarDataFromGrilla();
                    }
                    else
                    {
                        XtraMessageBox.Show("No se encontraron resultados", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txtbusqueda.Text = "";
                        txtbusqueda.Focus();
                        fnSistema.spllenaGridView(gridBusqueda, "SELECT codFormula, descFormula, sistema, valor FROM formula WHERE codFormula <> '0'");
                        fnSistema.spOpcionesGrilla(viewBusqueda);
                        fnColumnas();

                        //CARGAR DATOS EN FORMULA PRINCIPAL
                        CargarDataFromGrilla();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

        }

        //LIMPIAR CAMPO
        private void fnLimpiarBusqueda()
        {
            txtbusqueda.Text = "";
            txtbusqueda.Focus();
            //lblmsg.Visible = false;

        }
        //COLUMNAS GRILLA
        private void fnColumnas()
        {
            //SELECT codformula, descformula FROM formula
            viewBusqueda.Columns[0].Caption = "Codigo";
            viewBusqueda.Columns[0].Width = 10;
            viewBusqueda.Columns[0].AppearanceCell.FontStyleDelta = FontStyle.Bold;

            viewBusqueda.Columns[1].Caption = "Descripcion";
            viewBusqueda.Columns[2].Visible = false;
            viewBusqueda.Columns[3].Visible = false;
        }

        //DEFAULTPROPERTIES
        private void fnDefaultPropertiesBusqueda()
        {
            btnBuscar.AllowFocus = false;
            btnLimpiar.AllowFocus = false;
            gridBusqueda.TabStop = false;
            txtbusqueda.Properties.MaxLength = 100;
        }

        //CARGAR DATOS DESDE GRILLA EN CAJA DE TEXTO
        private void CargarDataFromGrilla()
        {
            if (viewBusqueda.RowCount>0)
            {
                txtcodigo.ReadOnly = true;

                txtcodigo.Text = (string)viewBusqueda.GetFocusedDataRow()["codformula"];
                txtDescripcion.Text = (string)viewBusqueda.GetFocusedDataRow()["descformula"];
                bool sistema = (bool)viewBusqueda.GetFocusedDataRow()["sistema"];

                if (sistema)
                {
                    chSistema.Checked = true;
                    btnGuardar.Enabled = false;
                    btnEliminar.Enabled = false;
                }
                else
                {
                    chSistema.Checked = false;
                    btnGuardar.Enabled = true;
                    btnEliminar.Enabled = true;
                }                    

                txtExpresion.Text = (string)viewBusqueda.GetFocusedDataRow()["valor"];

                op.Cancela = false;
                op.SetButtonProperties(btnNuevo, 1);

            }
        }    

        #endregion

        #region "PARA LOG FORMULA"
        //TABLA HASH FORMULA
        private Hashtable PrecargaDatosFormula(string pCodFormula)
        {
            string sql = "SELECT codFormula, descformula, valor, sistema FROM FORMULA WHERE codFormula=@code";
            Hashtable datos = new Hashtable();
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@code", pCodFormula));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //AGREGAMOS DATOS A LA TABLA HASH
                                datos.Add("codformula", (string)rd["codformula"]);
                                datos.Add("descformula", (string)rd["descformula"]); 
                                datos.Add("valor", (string)rd["valor"]);
                                datos.Add("sistema", (bool)rd["sistema"]);
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

        //COMPARA VALORES 
        private void ComparaValorFormula(Hashtable listado, string desc, string valor, bool sistema, string codFormula)
        {
            if (listado.Count > 0)
            {
                //SI LOS VALORES SON DISTINTOS GUARDAMOS EVENTO EN REGISTRO LOG"!

                if ((string)listado["descformula"] != desc)
                {          
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA DESCRIPCION FORMULA "+ codFormula, "FORMULA", (string)listado["descformula"], desc, "MODIFICAR");
                    log.Log();
                }
                if ((string)listado["valor"] != valor)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA EXPRESION DE FORMULA " +codFormula, "FORMULA", (string)listado["valor"], valor, "MODIFICAR");
                    log.Log();
                }
                if ((bool)listado["sistema"] != sistema)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA OPCION SISTEMA DE FORMULA " + codFormula, "FORMULA", (bool)listado["sistema"] + "", sistema + "", "MODIFICAR");
                    log.Log();
                }
            }
        }
        #endregion

        #endregion

        //EVITAR CARACTERES RAROS
        private void txtcodigo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
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

        private void txtExpresion_KeyPress(object sender, KeyPressEventArgs e)
        {
            /*CARACTERES VALIDOS */
            //--> '(,),[,],+,-,*,/,^,=,>,<', '.'
            //NUMEROS Y LETRAS 
            if (char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (e.KeyChar == (char)40)
            {//PARENTESIS ABERTURA
                e.Handled = false;

            }
            else if (e.KeyChar == (char)41)
            {//PARENTESIS CIERRE
                e.Handled = false;
            }
            else if (e.KeyChar == (char)91)
            {
                e.Handled = false;
            }
            else if (e.KeyChar == (char)93)
            {
                e.Handled = false;
            }
            else if (e.KeyChar == (char)47)
            {
                e.Handled = false;
            }
            else if (e.KeyChar == (char)43)
            {
                e.Handled = false;
            }
            else if (e.KeyChar == (char)45)
            {
                e.Handled = false;
            }
            else if (e.KeyChar == (char)42)
            {
                e.Handled = false;
            }
            else if (e.KeyChar == (char)94)
            {
                e.Handled = false;
            }
            else if (e.KeyChar == (char)61)
            {
                e.Handled = false;
            }
            else if (e.KeyChar == (char)60)
            {
                e.Handled = false;
            }
            else if (e.KeyChar == (char)62)
            {
                e.Handled = false;
            }
            else if (e.KeyChar == (char)44)
            {
                e.Handled = false;
            }
            else if (e.KeyChar == (char)46)
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
            else if (e.KeyChar == (char)59)
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
            Sesion.NuevaActividad();

            if (txtcodigo.Text == "")
            {
                XtraMessageBox.Show("codigo Formula no valido", "Codigo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (txtExpresion.Text == "")
            {
                XtraMessageBox.Show("Valor para formula no valido", "Error Expresion" ,MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //PARA METODOS CALCULO VERDAD Y CALCULO FALSO
            bool  cadenaValida = false;

            if (txtcodigo.ReadOnly)
            {
                //UPDATE
                //VALIDAR PREVIAMENTE LA SINTAXIS DE LA EXPRESION 

                try
                {
                    Expresion ep = new Expresion(txtExpresion.Text);
                    //LIMPIAR CADENA
                    // string limpia = ep.LimpiarExpresion();
                    cadenaValida = ep.ValidacionSuprema();

                    if (cadenaValida)
                    {
                        //HACEMOS UPDATE
                        fnModificarFormula(txtcodigo, txtDescripcion, txtExpresion, chSistema);

                        //SI VARIABLE OPENER ES DISTINTA DE NULL ES PORQUE EL FORM SE INVOCO DESDE EL FORM ITEM
                        if (opener != null)
                            opener.RecargarComboFormula();
                    }
                    else
                    {
                        XtraMessageBox.Show("Expresion no válida", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show("Ha ocurrido un error al intentar guardar los datos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }           
            }
            else
            {
                //INSERT
                //VERIFICAR QUE EL CODIGO A INGRESAR NO EXISTA PREVIAMENTE EN BD
                try
                {
                    bool existe = fnVerificarCodigo("F" + txtcodigo.Text);
                    //SI RETORNA TRUE EL CODIGO NO ES VALIDO
                    if (existe) { lblerror.Visible = true; lblerror.Text = "Codigo ya existe en base de datos"; return; }

                    //HACER TODA LA PREVIA CON RESPECTO A LA EXPRESION...
                    //VALIDAR QUE CUMPLA CON TODAS LAS REGLAS
                    Expresion ep = new Expresion(txtExpresion.Text);
                    cadenaValida = ep.ValidacionSuprema();

                    if (cadenaValida)
                    {
                        //LA CADENA ES VALIDA, VERICAMOS SI ESTA CHECKEADO OPCION SISTEMA
                        if (chSistema.Checked)
                        {
                            DialogResult advertencia = XtraMessageBox.Show("Haz seleccionado la opción sistema por lo que una vez que guardes esta formula ya no podrás editarla, ¿Realmente desea realizar esta operación?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (advertencia == DialogResult.Yes)
                            {
                                //RELIZAR INGRESO
                                fnIngresoFormula(txtcodigo, txtDescripcion, txtExpresion, chSistema);
                            }
                        }
                        else
                        {
                            //SI NO ESTA CHEQUEADO NO HACEMOS ADVERTENCIA
                            //RELIZAR INGRESO
                            fnIngresoFormula(txtcodigo, txtDescripcion, txtExpresion, chSistema);
                        }

                        //SI VARIABLE OPENER ES DISTINTA DE NULL ES PORQUE EL FORM SE INVOCO DESDE EL FORM ITEM
                        if (opener != null)
                            opener.RecargarComboFormula();
                    }
                    else
                    {
                        XtraMessageBox.Show("Expresion no válida", "Expresion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show("ha ocurrido un error al intentar guardar los datos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                          
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            fnLimpiarCampos();
            if (op.Cancela == false)
            {
                op.Cancela = true;
                op.SetButtonProperties(btnNuevo, 2);
            }
            else
            {
                op.SetButtonProperties(btnNuevo, 1);
                op.Cancela = false;
                CargarDataFromGrilla();             
            }
        }
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            if (txtcodigo.Text == "") { XtraMessageBox.Show("Codigo no valido", "Eliminar", MessageBoxButtons.OK, MessageBoxIcon.Error);return;}

            //verificar que el codigo a eliminar exista en la base de datos
            bool existe = fnVerificarCodigo(txtcodigo.Text);
            if (existe)
            {
                fnEliminarFormula(txtcodigo);

                //SI VARIABLE OPENER ES DISTINTA DE NULL ES PORQUE EL FORM SE INVOCO DESDE EL FORM ITEM
                if (opener != null)
                {
                    opener.RecargarComboFormula();
                    //RECARGAR GRILLA EN FORMULARIO ITEM
                    if(Tipo.ToString() != "")
                    opener.RecargarGrillaTipo(Tipo);
                }                    
            }
            else
            {
                XtraMessageBox.Show("Codigo no valido");
                return;
            }            
        }

        private void txtcodigo_KeyDown(object sender, KeyEventArgs e)
        {
            bool existe = false;
            if (e.KeyCode == Keys.Enter)
            {
                //SI SE PRESIONA LA TECLA ENTER VALIDAMOS QUE EL CODIGO NO ESTE VACIO O SEA UN CODIGO EXISTENTE
                if (txtcodigo.Text == "")
                {
                    lblerror.Visible = true;
                    lblerror.Text = "codigo no valido";
                }
                else
                {
                    if (txtcodigo.ReadOnly == false)
                    {
                        existe = fnVerificarCodigo("F" + txtcodigo.Text);
                        if (existe)
                        {
                            //SI ES TRUE NO ES VALIDO
                            lblerror.Visible = true;
                            lblerror.Text = "codigo ya existe en base de datos";
                        }
                        else
                        {
                            lblerror.Visible = false;
                        }
                    }
                }
            }
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            bool existe = false;
            if (keyData == Keys.Tab)
            {
                if (txtcodigo.ContainsFocus)
                {
                    //VERIFICAMOS QUE EL CODIGO ES UN CODIGO VALIDO
                    if (txtcodigo.Text == "")
                    {
                        lblerror.Visible = true;
                        lblerror.Text = "Codigo no valido";
                    }
                    else
                    {
                        if (txtcodigo.ReadOnly == false)
                        {
                            //VERIFICAR SI CODIGO EXISTE EN BD
                            existe = fnVerificarCodigo("F" + txtcodigo.Text);
                            if (existe)
                            {
                                lblerror.Visible = true;
                                lblerror.Text = "Codigo ya existe en la base de datos";
                            }
                            else
                            {
                                lblerror.Visible = false;
                            }
                        }                     
                    }
                }
                else if (txtExpresion.ContainsFocus)
                {
                    return false;
                }
            }           

            return base.ProcessDialogKey(keyData);
        } 

        private void btnHelp_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            //MOSTRAR FORMULARIO DE AYUDA
            frmHelp hp = new frmHelp();
            hp.StartPosition = FormStartPosition.CenterScreen;
            hp.ShowDialog();
        }

        private void btnPlantilla_Click(object sender, EventArgs e)
        {
            //SESION 
            Sesion.NuevaActividad();

            frmPlantillaBase fpl = new frmPlantillaBase();            
            fpl.StartPosition = FormStartPosition.CenterScreen;
            fpl.opener = this;
            fpl.Show();
        }

        private void btnLimpiarExpresion_Click(object sender, EventArgs e)
        {
            //LIMPIAR MEMO EXPRESION
            txtExpresion.Text = "";
        }

        private void btnVariables_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            //ABRIR FORMULARIO PARA VISUALIZAR LAS VARIABLES DE SISTEMA ACTUALES
            frmverVariable variables = new frmverVariable();
            variables.ShowDialog();
        }

        private void txtExpresion_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            DXPopupMenu menu = e.Menu;
           
            if (menu != null)
            {
                //DXMenuItemCollection items = menu.Items;
                //DXMenuItem menuItem = TestMenuitem();
                DXMenuItem submenu = new DXMenuItem("Agregar Variable", new EventHandler(AgregarFormula_Click));
                submenu.ImageOptions.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("images/richedit/insertequationcaption_16x16.png");

                menu.Items.Clear();
                menu.Items.Add(submenu);
            }
        }

        private void AgregarFormula_Click(object sender, EventArgs e)
        {
            //POSICION DEL CURSOR
            int posicion = 0;
            posicion = txtExpresion.SelectionStart;

            if (txtcodigo.ReadOnly && chSistema.Checked)
            {
                XtraMessageBox.Show("No puedes editar una formula de sistema", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                frmverVariable vari = new frmverVariable(posicion, true);
                vari.opener = this;
                vari.ShowDialog();
            }

            //AGREGAR TEXTO EN LA POSICION SELECCIONADA
           
            //txtExpresion.Text = txtExpresion.Text.Insert(posicion, prueba);
        }
     
        private DXPopupMenu CreateMenu()
        {
            DXPopupMenu menu = new DXPopupMenu();
            //sub items
            DXMenuItem item1 = new DXMenuItem("Menu item1");
            DXMenuItem item2 = new DXMenuItem("Menu item2");
            //main item
            DXSubMenuItem subItem = new DXSubMenuItem("Items");
            subItem.Items.Add(item1);
            subItem.Items.Add(item2);
            menu.Items.Add(menu);
            return menu;              
        }

        private void txtExpresion_Click(object sender, EventArgs e)
        {
            string cadena = txtExpresion.Text;
            if (cadena != "")
            {
                txtExpresion.Properties.Appearance.ForeColor = Color.Brown;
            }
        }

        private void txtExpresion_Leave(object sender, EventArgs e)
        {
            string cadena = txtExpresion.Text;
            if (cadena != "")
            {
                txtExpresion.Properties.Appearance.ForeColor = Color.Black;
            }
        }

        private void txtExpresion_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            if ((e.KeyData == (Keys.Control | Keys.C))  || (e.KeyData == (Keys.Control | Keys.V)) || (e.KeyData == (Keys.Control | Keys.X)))
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
          
        }

        private void btnBuscarFormula_Click(object sender, EventArgs e)
        {
            frmBuscarFormula buscar = new frmBuscarFormula();
            buscar.opener = this;
            buscar.ShowDialog();
        }

        private void txtcodigo_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtDescripcion_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            if (viewBusqueda.RowCount > 0)
            {
                string code = (string)viewBusqueda.GetFocusedDataRow()["codformula"];
                if (fnVerificarCambios(code) && EsFormulaSistema(code) == false)
                {
                    DialogResult advertencia = XtraMessageBox.Show("Hay cambios sin guardar, ¿Desea cerrar ventana de todas maneras?", "Cambios sin guardar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (advertencia == DialogResult.Yes)
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
                Close();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            if (txtbusqueda.Text == "")
            {
                XtraMessageBox.Show("Por favor ingresar una busqueda valida", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtbusqueda.Focus();
                return;
            }
            else
            {
                fnBusquedaFormula(txtbusqueda);
            }
        }

        private void txtbusqueda_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || char.IsLetter(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            fnLimpiarBusqueda();
        }

        private void gridBusqueda_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            CargarDataFromGrilla();

            btnPlantilla.Enabled = false;
        }

        private void gridBusqueda_KeyUp(object sender, KeyEventArgs e)
        {
            Sesion.NuevaActividad();
            CargarDataFromGrilla();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            string grilla = "SELECT codFormula, descFormula, sistema, valor FROM formula WHERE codFormula <> '0'";
            fnSistema.spllenaGridView(gridBusqueda, grilla);
            fnSistema.spOpcionesGrilla(viewBusqueda);
            fnColumnas();

            //RECARGAR FORMULA DESDE GRILLA
            CargarDataFromGrilla();
            btnPlantilla.Enabled = false;
        }

        private void btnLast_Click_1(object sender, EventArgs e)
        {

        }

        private void btnnext_Click_1(object sender, EventArgs e)
        {

        }

        private void btnPrev_Click_1(object sender, EventArgs e)
        {

        }

        private void btnFirst_Click_1(object sender, EventArgs e)
        {

        }

        private void groupControl1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnParametros_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmparametroformula") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta función", "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            frmParametroFormula parametros = new frmParametroFormula();
            parametros.StartPosition = FormStartPosition.CenterParent;
            parametros.ShowDialog();
        }
    }
}