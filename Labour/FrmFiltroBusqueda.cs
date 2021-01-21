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
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using System.Collections;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using DevExpress.Utils.Menu;
using System.Runtime.InteropServices;

namespace Labour
{
    public partial class FrmFiltroBusqueda : DevExpress.XtraEditors.XtraForm
    {
        [DllImport("user32")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32")]
        static extern bool EnableMenuItem(IntPtr hMenu, uint IDEnableItem, uint uEnable);

        const int MF_BYCOMMAND = 0;
        const int MF_DISABLED = 2;
        const int SC_CLOSED = 0xF060;

        //PARA SABER SI ES MODIFICACION O INGRESO NUEVO
        private bool Update = false;

        //PARA SABER SI EL FORMULARIO ES INVOCADO DESDE ALGUN FORMULARIO DE BUSQUEDA COMO DETALLE ITEMS
        private bool Invocado = false;

        private bool cancela = true;

        /// <summary>
        /// Condicion para grilla.
        /// </summary>
        string sqlGrilla = "SELECT codigo, descripcion, cadena FROM conjunto";

        public IConjuntosCondicionales opener { get; set; }

        public FrmFiltroBusqueda()
        {
            InitializeComponent();
        }
        public FrmFiltroBusqueda(bool Invocado)
        {
            InitializeComponent();
            this.Invocado = Invocado;
        }
        private void FrmFiltroBusqueda_Load(object sender, EventArgs e)
        {
            var sm = GetSystemMenu(Handle, false);
            EnableMenuItem(sm, SC_CLOSED, MF_BYCOMMAND | MF_DISABLED);

            if (Invocado)
                btnSeleccionar.Enabled = true;
            else
                btnSeleccionar.Enabled = false;

            //CARGAMOS TODOS LOS DATOS EXISTENTES EN TABLA CONJUNTO
            fnSistema.spllenaGridView(gridExpresiones, sqlGrilla);            
            fnSistema.spOpcionesGrilla(viewExpresiones);


            if (viewExpresiones.RowCount > 0)
            {
                ColumnasExpresiones();
                CargarCampos(0);
            }                
            else
            {
                Update = false;
                btnEliminar.Enabled = false;
            }                
        }

        #region "MANEJO DE DATOS"
        //INGRESAR UN NUEVO CONJUNTO
        private void NuevoConjunto(TextEdit pCode, TextEdit pDesc, TextEdit pExpresion)
        {
            string sql = "INSERT INTO conjunto(codigo, descripcion, cadena) VALUES(@code, @desc, @cadena)";
            SqlCommand cmd;
            int res = 0;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@code", pCode.Text));
                        cmd.Parameters.Add(new SqlParameter("@desc", pDesc.Text));
                        cmd.Parameters.Add(new SqlParameter("@cadena", pExpresion.Text));                      

                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            XtraMessageBox.Show("Ingreso correcto", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            //GUARDAMOS EN LOG
                            logRegistro log = new logRegistro(User.getUser(), "SE INGRESA NUEVA CONDICION CONJUNTO CODIGO " + pCode.Text, "CONJUNTO", "0", pExpresion.Text, "INGRESAR");
                            log.Log();                            

                            fnSistema.spllenaGridView(gridExpresiones, sqlGrilla);
                            fnSistema.spOpcionesGrilla(viewExpresiones);
                            ColumnasExpresiones();

                            CargarCampos(0);

                            cancela = true;
                            btnNuevo.Text = "Nuevo";
                            btnNuevo.ToolTip = "Nuevo";

                        }
                        else
                        {
                            XtraMessageBox.Show("Error al intentar guardar", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        //MODIFICAR UN CONJUNTO
        private void ModificarConjunto(TextEdit pCode, TextEdit pDesc, TextEdit pExpresion, string codigoDb)
        {
            string sql = "UPDATE conjunto SET codigo=@code,  descripcion=@desc, cadena=@cadena " +
                " WHERE codigo=@codeDb";
            SqlCommand cmd;
            int res = 0;

            Hashtable data = new Hashtable();
            data = PrecargaData(codigoDb);

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@desc", pDesc.Text));
                        cmd.Parameters.Add(new SqlParameter("@cadena", pExpresion.Text));
                        cmd.Parameters.Add(new SqlParameter("@code", pCode.Text));
                        cmd.Parameters.Add(new SqlParameter("@codeDb", codigoDb));                        

                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            XtraMessageBox.Show("Actualizacion correcta", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);                          

                            //COMPARAMOS
                            ComparaData(data, pCode.Text, pDesc.Text, pExpresion.Text);

                            //CARGAMOS GRILLA
                            fnSistema.spllenaGridView(gridExpresiones, sqlGrilla);
                            fnSistema.spOpcionesGrilla(viewExpresiones);
                            CargarCampos(0);

                            cancela = true;
                            btnNuevo.Text = "Nuevo";
                            btnNuevo.ToolTip = "Nuevo registro";
                        }
                        else
                        {
                            XtraMessageBox.Show("Error al intentar actualizar", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        //ELIMINAR CONJUNTO
        private void EliminarConjunto(string pCode)
        {
            string sql = "DELETE from conjunto WHERE codigo=@codigo";
            SqlCommand cmd;
            int res = 0;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@codigo", pCode));
                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            XtraMessageBox.Show("Registro eliminado", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            //GUARDAR EN LOG
                            logRegistro log = new logRegistro(User.getUser(), "SE HA ELIMINADO REGISTRO CON CODIGO " + pCode, "CONJUNTO", pCode, "0", "ELIMINAR");
                            log.Log();

                            //RECARGAR GRILLA
                            fnSistema.spllenaGridView(gridExpresiones, sqlGrilla);
                            fnSistema.spOpcionesGrilla(viewExpresiones);

                            CargarCampos(0);

                            cancela = true;
                            btnNuevo.Text = "Nuevo";
                            btnNuevo.ToolTip = "Nuevo registro";
                        }
                        else
                        {
                            XtraMessageBox.Show("Error al intentar eliminar", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        //PROBAR LA EXPRESION
        private bool PruebaCondicion(string cadena)
        {
            bool valida = false;
            if (cadena != "")
            {
                string sql = String.Format("SELECT contrato FROM trabajador where {0} AND anomes=@pPeriodo", cadena);
                //sql = $"SELECT MAX(anomes) FROM trabajador WHERE {cadena} GROUP BY contrato";
             
                SqlCommand cmd;
                SqlDataReader rd;
                try
                {
                    if (fnSistema.ConectarSQLServer())
                    {
                        using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                        {
                            cmd.Parameters.Add(new SqlParameter("@pPeriodo", Calculo.PeriodoObservado));

                            rd = cmd.ExecuteReader();
                            if (rd.HasRows)
                            {
                                valida = true;
                            }
                            else
                            {
                                valida = false;
                                XtraMessageBox.Show("La busqueda no trajo resultados", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                    }
                }
                catch (SqlException ex)
                {
                    XtraMessageBox.Show("Parece ser que tienes un error de sintaxis", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    valida = false;
                }
            }
            return valida;
        }

        //LIMPIAR LOS CAMPOS
        private void LimpiarCampos()
        {
            txtcodigo.Text = "";
            txtdesc.Text = "";
            txtCondicion.Text = "";
            Update = false;
            btnEliminar.Enabled = false;
            gridResultado.DataSource = null;
            txtcodigo.Focus();
            lblmsg.Visible = false;

            txtQuery.Text = "[QUERY] =>";
        }

        //COLUMNAS GRILLA EXPRESIONES
        private void ColumnasExpresiones()
        {
            viewExpresiones.Columns[0].Caption = "Codigo";
            viewExpresiones.Columns[0].Width = 30;
            viewExpresiones.Columns[0].AppearanceCell.FontStyleDelta = FontStyle.Bold;
            viewExpresiones.Columns[1].Caption = "Descripcion";
            viewExpresiones.Columns[1].Width = 200;
            viewExpresiones.Columns[2].Visible = false;
        }

        //CARGAR DATOS CON BUSQUEDA     
        private void CargarDataBusqueda(string condicion)
        {
            string sql = "SELECT rut, contrato, CONCAT(nombre, ' ',apepaterno,  ' ', apematerno) as nombre " +
                $"FROM trabajador WHERE {condicion} AND anomes={Calculo.PeriodoObservado}";
            SqlCommand cmd;
            SqlDataAdapter ad = new SqlDataAdapter();
            DataSet ds = new DataSet();

            txtQuery.Text = "[QUERY] => " + sql;

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        

                        ad.SelectCommand = cmd;
                        ad.Fill(ds, "data");

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                        ad.Dispose();
                        if (ds.Tables[0].Rows.Count>0)
                        {
                            //CARGAR GRIDVIEW                            
                            gridResultado.DataSource = ds.Tables[0];                            
                            fnSistema.spOpcionesGrilla(viewResultado);
                            ColumnasBusqueda();
                            viewResultado.Appearance.FocusedRow.ForeColor = Color.Empty;
                            viewResultado.Appearance.FocusedRow.BackColor = Color.Empty;
                            viewResultado.OptionsSelection.EnableAppearanceFocusedRow = false;
                            lblCount.Text = $"Registros:{ds.Tables[0].Rows.Count}";
                        }
                        else
                        {
                            gridResultado.DataSource = null;
                            lblCount.Text = "Registros:0";
                            fnSistema.spOpcionesGrilla(viewResultado);                            
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        //COLUMNAS GRILLA BUSQUEDA
        private void ColumnasBusqueda()
        {
            viewResultado.Columns[0].Caption = "rut";
            viewResultado.Columns[0].Width = 40;
            viewResultado.Columns[0].DisplayFormat.FormatString = "Rut";
            viewResultado.Columns[0].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            viewResultado.Columns[0].DisplayFormat.Format = new FormatCustom();

            viewResultado.Columns[1].Caption = "contrato";
            viewResultado.Columns[1].Width = 40;

            viewResultado.Columns[2].Caption = "nombre";
            viewResultado.Columns[2].Width = 200;
        }

        //CARGAR CMAPOS DE ACUERDO A FILA SELECCIONADA DE LA GRILLA 
        private void CargarCampos(int? pos = -1)
        {
            if (viewExpresiones.RowCount>0)
            {
                if (pos != -1) viewExpresiones.FocusedRowHandle = (int)pos;

                Update = true;
                btnEliminar.Enabled = true;
                lblmsg.Visible = false;

                txtcodigo.Text = (string)viewExpresiones.GetFocusedDataRow()["codigo"];               
                txtdesc.Text = (string)viewExpresiones.GetFocusedDataRow()["descripcion"];
                txtCondicion.Text = (string)viewExpresiones.GetFocusedDataRow()["cadena"];               

                //CARGAR GRILLA DE ACUERDO A CONSULTA
                CargarDataBusqueda((string)viewExpresiones.GetFocusedDataRow()["cadena"]);

                cancela = true;
                btnNuevo.Text = "Nuevo";
                btnNuevo.ToolTip = "Nuevo registro";
            }
            else
            {
                LimpiarCampos();
            }
        }

        //VERIFICAR SI CODIGO EXISTE EN BD
        private bool ExisteCondicion(string codigo)
        {
            bool valida = false;
            string sql = "SELECT codigo FROM conjunto WHERE codigo=@codigo";
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@codigo", codigo));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            valida = true;
                        }
                        else
                        {
                            valida = false;
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

            return valida;
        }

        //PARA LOG
        //PRECARGA
        private Hashtable PrecargaData(string codigo)
        {
            Hashtable data = new Hashtable();
            string sql = "SELECT codigo, descripcion, cadena FROM conjunto WHERE codigo=@codigo";
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@codigo", codigo));
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                data.Add("codigo", (string)rd["codigo"]);
                                data.Add("descripcion", (string)rd["descripcion"]);
                                data.Add("cadena", (string)rd["cadena"]);
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
            return data;
        }

        //COMPARA VALORES
        private void ComparaData(Hashtable data, string codigo, string descripcion, string cadena)
        {
            if ((string)data["codigo"] != codigo)
            {
                logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA CODIGO CONDICION CONJUNTO", "CONJUNTO", (string)data["codigo"], codigo, "MODIFICAR");
                log.Log();
            }
            if ((string)data["descripcion"] != descripcion)
            {
                logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA DESCRIPCION CONDICION CONJUNTO " + (string)data["codigo"], "CONJUNTO", (string)data["descripcion"], descripcion, "MODIFICAR");
                log.Log();
            }
            if ((string)data["cadena"] != cadena)
            {
                logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA CONDICION EN CONJUNTO CODIGO " + (string)data["codigo"], "CONJUNTO", (string)data["cadena"], cadena, "MODIFICAR");
                log.Log();
            }
        }

        //MANEJO TECLA TAB
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Tab)
            {
                if (txtcodigo.ContainsFocus)
                {
                    if (Update == false)
                    {
                        //INSERT
                        //VERIFICAR QUE EL CODIGO NO EXISTA EN BD
                        if (ExisteCondicion(txtcodigo.Text)) { lblmsg.Visible = true; lblmsg.Text = "codigo ingresado ya existe"; return false; }

                        lblmsg.Visible = false;
                    }
                    else
                    {
                        //UPDATE
                        if (viewExpresiones.RowCount>0)
                        {
                            string code = (string)viewExpresiones.GetFocusedDataRow()["codigo"];
                            //COMPARAMOS
                            if (code != txtcodigo.Text)
                            {
                                //SI SON DISTINTOS VERIFICAR QUE EL NUEVO CODIGO NO EXISTA EN BD
                                if (ExisteCondicion(txtcodigo.Text))
                                { lblmsg.Visible = true; lblmsg.Text = "Codigo ingresado ya existe"; return false; }
                            }
                            else
                            {
                                lblmsg.Visible = false;
                            }
                        }
                    }
                }
            }
            return base.ProcessDialogKey(keyData);
        }

        //VER SI HAY CAMPOS SIN GUARDAR
        private bool Cambiosinguardar(string code)
        {
            if (code.Length>0)
            {
                string sql = "SELECT codigo, descripcion, cadena FROM conjunto WHERE codigo=@pCode";
                SqlCommand cmd;
                SqlDataReader rd;
                try
                {
                    if (fnSistema.ConectarSQLServer())
                    {
                        using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@pCode", code));

                            rd = cmd.ExecuteReader();
                            if (rd.HasRows)
                            {
                                //RECORREMOS
                                while (rd.Read())
                                {
                                    if (txtcodigo.Text != (string)rd["codigo"]) return true;
                                    if (txtdesc.Text != (string)rd["descripcion"]) return true;
                                    if (txtCondicion.Text != (string)rd["cadena"]) return true;
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
            }

            return false;
        }

        //VERIFICAR SI HAY USUARIO USANDO FILTRO
        private bool UsuarioUsaExpresion(string pCod)
        {
            bool usa = false;
            string sql = "SELECT count(*) FROM usuario WHERE filtro=@pCod";
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
                            usa = true;

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return usa;
        }

        #endregion

        private void txtcodigo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar) || char.IsLetter(e.KeyChar))
            {
                e.Handled = false;
            }
            else
                e.Handled = true;
        }

        private void txtCondicion_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar) || char.IsLetter(e.KeyChar) || char.IsSeparator(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (e.KeyChar == (char)39 || e.KeyChar == (char)40 || e.KeyChar == (char)41)
            {
                e.Handled = false;
            }
            else if (e.KeyChar == (char)42 || e.KeyChar == (char)43 || e.KeyChar == (char)60 || e.KeyChar == (char)61)
            {
                e.Handled = false;
            }
            else if (e.KeyChar == (char)62 || e.KeyChar == (char)45 || e.KeyChar == (char)47)
            {
                e.Handled = false;
            }
            else if (e.KeyChar == (char)37)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void txtCondicion_Click(object sender, EventArgs e)
        {
            if (txtCondicion.Text != "")
            {
                txtCondicion.Properties.Appearance.ForeColor = Color.Brown;
            }
        }

        private void txtCondicion_Leave(object sender, EventArgs e)
        {
            if (txtCondicion.Text != "")
            {
                txtCondicion.Properties.Appearance.ForeColor = Color.Black;
            }
        }

        private void txtCondicion_KeyDown(object sender, KeyEventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                if (txtCondicion.Text == "") { XtraMessageBox.Show("Por favor ingresa una condicion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);return; }               

                //VALIDAR QUE LA EXPRESION TENGA SINTAXIS CORRECTA
                if (PruebaCondicion(txtCondicion.Text))
                    CargarDataBusqueda(txtCondicion.Text);
            }
            if ((e.KeyData == (Keys.Control | Keys.C)) || (e.KeyData == (Keys.Control | Keys.V)) || (e.KeyData == (Keys.Control | Keys.X)))
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            if (txtcodigo.Text == "") { XtraMessageBox.Show("Por favor ingresa un codigo", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);txtcodigo.Focus();return; }
            if (txtdesc.Text == "") { XtraMessageBox.Show("Por favor ingresa una descripcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);txtdesc.Focus();return; }
            if (txtCondicion.Text == "") { XtraMessageBox.Show("Por favor ingresa una expresion a evaluar", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);txtCondicion.Focus();return;}            
            
            if (Update == false)
            {
                //INSERT
                //VERIFICAR QUE EL CODIGO QUE SE INTENTA REGISTRAR NO ESTE EN BD
                string code = txtcodigo.Text;                
                if (ExisteCondicion(code)) { XtraMessageBox.Show("codigo ingresado ya existe", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information) ;return; }

                //VALIDAR QUE LA EXPRESION QUE SE INTENTA GUARDAR SEA CORRECTA
                if (PruebaCondicion(txtCondicion.Text))
                {
                    NuevoConjunto(txtcodigo, txtdesc, txtCondicion);
                }
            }
            else
            {
                //UPDATE
                //VALIDAR LA EXPRESION
                if (viewExpresiones.RowCount>0)
                {
                    string code = (string)viewExpresiones.GetFocusedDataRow()["codigo"];
                    if (code != txtcodigo.Text)
                    {
                        DialogResult pregunta= XtraMessageBox.Show("Estás a punto de modificar el registro con codigo " + code + ", ¿estás seguro de realizar esta operacion?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (pregunta == DialogResult.Yes)
                        {
                            if (PruebaCondicion(txtCondicion.Text))
                            {
                                ModificarConjunto(txtcodigo, txtdesc, txtCondicion, code);
                            }
                        }
                    }
                    else
                    {
                        //SOLO ELIMNAMOS
                        if (PruebaCondicion(txtCondicion.Text))
                        {
                            ModificarConjunto(txtcodigo, txtdesc, txtCondicion, code);
                        }
                    }                                      
                }
                else
                {
                    XtraMessageBox.Show("No podemos actualizar este registro", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }               
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            if (cancela)
            {
                btnNuevo.Text = "Cancelar";
                btnNuevo.ToolTip = "Cancelar operacion";
                cancela = false;
                txtcodigo.Properties.Appearance.BackColor = Color.LightGoldenrodYellow;
                LimpiarCampos();
            }
            else
            {
                btnNuevo.Text = "Nuevo";
                btnNuevo.ToolTip = "Nuevo registro";      
                cancela = true;
                txtcodigo.Properties.Appearance.BackColor = Color.White;
                CargarCampos();
            }            
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            if (viewExpresiones.RowCount>0)
            {
                string code = (string)viewExpresiones.GetFocusedDataRow()["codigo"];

                if (UsuarioUsaExpresion(code))
                { XtraMessageBox.Show("No puedes eliminar registro usado por usuario", "Denegado", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                DialogResult dialogo = XtraMessageBox.Show("¿Seguro deseas eliminar el conjunto " + code + "?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogo == DialogResult.Yes)
                {
                    if (ExisteCondicion(txtcodigo.Text))
                    {
                        //BORRAMOS...
                        EliminarConjunto(code);
                    }
                    else
                    {
                        XtraMessageBox.Show("Registro no valido", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }              
            }
            else
            {
                XtraMessageBox.Show("Registro no valido", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void FrmFiltroBusqueda_Shown(object sender, EventArgs e)
        {
            txtcodigo.Focus();
        }

        private void gridExpresiones_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            if (viewExpresiones.RowCount>0)
            {
                CargarCampos();
            }
        }

        private void gridExpresiones_KeyUp(object sender, KeyEventArgs e)
        {
            if (viewExpresiones.RowCount>0)
            {
                CargarCampos();
            }
        }

        private void txtCondicion_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            DXPopupMenu menu = e.Menu;
            if (menu != null)
            {
                menu.Items.Clear();
                DXSubMenuItem variables = new DXSubMenuItem("campos");
                DXSubMenuItem logico = new DXSubMenuItem("logico");

                List<string> operadoresLogicos = new List<string>();
                operadoresLogicos = Logicos();

                foreach (var x in operadoresLogicos)
                {
                    DXMenuItem d = new DXMenuItem(x, new EventHandler(x_click));
                    logico.Items.Add(d);
                }

                List<string> data = new List<string>();
                data = ListadoCampos();

                foreach (var elemento in data)
                {
                    DXMenuItem item = new DXMenuItem(elemento, new EventHandler(elemento_click));
                    variables.Items.Add(item);
                }

                menu.Items.Add(variables);
                menu.Items.Add(logico);                      
            }
        }

        private void elemento_click(object sender, EventArgs e)
        {
            DXMenuItem item = sender as DXMenuItem;

            int position = txtCondicion.SelectionStart;

            txtCondicion.Text = txtCondicion.Text.Insert(position, " ");
            txtCondicion.Text = txtCondicion.Text.Insert(position + 1, item.Caption);
            txtCondicion.Select((position + item.Caption.Length) + 1, 0);
            txtCondicion.ScrollToCaret();
        }

        private void x_click(object sender, EventArgs e)
        {
            DXMenuItem item = sender as DXMenuItem;

            //OBTENER LA POSICION DEL CURSOR
            int position = txtCondicion.SelectionStart;
            string cadena = item.Caption;

            if (cadena == "BETWEEN")
                cadena = "CAMPO BETWEEN 'X' AND 'Y'";

            txtCondicion.Text = txtCondicion.Text.Insert(position, " ");
            txtCondicion.Text = txtCondicion.Text.Insert(position + 1, cadena);
            txtCondicion.Select((position + cadena.Length) + 1, 0);
            txtCondicion.ScrollToCaret();
        }

        private List<string> ListadoCampos()
        {
            string sql = "SELECT UPPER(COLUMN_NAME) as campo from INFORMATION_SCHEMA.columns " +
                         "WHERE TABLE_NAME = 'trabajador' AND(column_name <> 'rutafoto' AND column_name <> 'id' AND column_name <> 'anomes')";
            SqlCommand cmd;
            SqlDataReader rd;
            List<string> data = new List<string>();

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
                                data.Add((string)rd["campo"]);
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

            return data;
        }

        private List<string> Logicos()
        {
            List<string> data = new List<string>();            
            data.Add("AND");
            data.Add("OR");
            data.Add("LIKE");
            data.Add("BETWEEN");
            data.Add("<>");
            data.Add(">");
            data.Add("<");
            data.Add("=");
            data.Add(">=");
            data.Add("<=");                      

            return data;
        }

        private void viewExpresiones_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (Invocado)
            {
                DXPopupMenu menu = e.Menu;
                if (menu != null)
                {
                    menu.Items.Clear();

                    DXMenuItem submenu = new DXMenuItem("Seleccionar", new EventHandler(seleccion_click));
                    submenu.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("images/actions/show_16x16.png");

                    menu.Items.Add(submenu);
                }
            }            
        }

        private void seleccion_click(object sender, EventArgs e)
        {
            if (Invocado)
            {
                if (viewExpresiones.RowCount>0)
                {
                    //CODE PARA TRASPASAR EL CODIGO DE SELECCION A FORMULARIO QUE LO NECESITA
                    string code = (string)viewExpresiones.GetFocusedDataRow()["codigo"];

                    if (opener != null)
                    {
                        opener.CargarCodigoConjunto(code);
                        Close();
                    }
                }
                else
                {
                    XtraMessageBox.Show("No hay conjuntos disponibles para seleccion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void memoEdit1_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            string code = "";
            if (viewExpresiones.RowCount > 0)
            {
                code = (string)viewExpresiones.GetFocusedDataRow()["codigo"];
                if (Cambiosinguardar(code))
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

        private void txtcodigo_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtdesc_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void btnSeleccionar_Click(object sender, EventArgs e)
        {
            if (Invocado)
            {
                Sesion.NuevaActividad();

                if (viewExpresiones.RowCount > 0)
                {
                    //CODE PARA TRASPASAR EL CODIGO DE SELECCION A FORMULARIO QUE LO NECESITA
                    string code = (string)viewExpresiones.GetFocusedDataRow()["codigo"];                    

                    if (opener != null)
                    {
                        opener.CargarCodigoConjunto(code);
                        Close();
                    }
                }
                else
                {
                    XtraMessageBox.Show("No hay conjuntos disponibles para seleccion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void gridExpresiones_DoubleClick(object sender, EventArgs e)
        {
            if (Invocado)
            {
                if (viewExpresiones.RowCount > 0)
                {
                    //CODE PARA TRASPASAR EL CODIGO DE SELECCION A FORMULARIO QUE LO NECESITA
                    string code = (string)viewExpresiones.GetFocusedDataRow()["codigo"];

                    if (opener != null)
                    {
                        opener.CargarCodigoConjunto(code);
                        Close();
                    }
                }
                else
                {
                    XtraMessageBox.Show("No hay conjuntos disponibles para seleccion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }


        private void btnPrevisualizar_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            if (txtCondicion.Text == "") { XtraMessageBox.Show("Por favor ingresa una condicion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }            

            //VALIDAR QUE LA EXPRESION TENGA SINTAXIS CORRECTA
            if (PruebaCondicion(txtCondicion.Text))
                CargarDataBusqueda(txtCondicion.Text);           
       
        }    
    }
}