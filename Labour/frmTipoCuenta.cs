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

namespace Labour
{
    public partial class frmTipoCuenta : DevExpress.XtraEditors.XtraForm
    {
        [DllImport("user32")]
        extern static IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32")]
        extern static bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        const int MF_BYCOMMAND = 0;
        const int MF_DISABLED = 2;
        const int SC_CLOSED = 0xF060;

        //VARIABLE PARA SABER SI ES UPDATE O INSERT
        private bool Update = false;

        public frmTipoCuenta()
        {
            InitializeComponent();
        }

        private void frmTipoCuenta_Load(object sender, EventArgs e)
        {
            var sm = GetSystemMenu(Handle, false);
            EnableMenuItem(sm, SC_CLOSED, MF_BYCOMMAND | MF_DISABLED);

            fnDefaultProperties();

            string grilla = "SELECT id, nombre FROM tipocuenta ORDER BY nombre";
            fnSistema.spllenaGridView(gridTipoCuenta, grilla);
            fnSistema.spOpcionesGrilla(viewTipoCuenta);
            fnColumnas();

            fnCargarCampos();            
        }

        #region "MANEJO DE DATOS"
        //INGRESO NUEVO TIPO DE CUENTA
        /*
         * PARAMETROS DE ENTRADA
         * ID (AUTOINCREMENTAL)
         * NOMBRE CUENTA
         */
        private void fnNuevaCuenta(TextEdit pNombre)
        {
            string sql = "INSERT INTO tipocuenta(nombre) VALUES(@pnombre)";

            SqlCommand cmd;
            int res = 0;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {

                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pnombre", pNombre.Text));

                        //EJECUTAR CONSULTA
                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            XtraMessageBox.Show("Registro guardado", "Guardar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            //RECARGAR GRILLA
                            fnSistema.spllenaGridView(gridTipoCuenta,"SELECT id, nombre FROM tipocuenta ORDER BY nombre");
                            fnCargarCampos(0);
                        }
                        else
                        {
                            XtraMessageBox.Show("Error al intentar guardar", "Guardar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        //MODIFICAR TIPO DE CUENTA
        /*
         * PARAMETROS DE ENTRADA:
         * ID
         * NOMBRE CUENTA
         */
        private void fnModificarCuenta(TextEdit pNombre, int pId)
        {
            string sql = "UPDATE TIPOCUENTA SET nombre=@pNombre WHERE id=@pId";

            SqlCommand cmd;
            int res = 0;
            Hashtable data = new Hashtable();
            data = PrecargaDatos(pId);

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pNombre", pNombre.Text));
                        cmd.Parameters.Add(new SqlParameter("@pId", pId));

                        //EJECUTAR CONSULTA
                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            XtraMessageBox.Show("Registro Actualizado", "Actualizar", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            //SI HAY CAMBIOS GUARDAMOS EN LOG
                            ModificaData(data, pNombre.Text);

                            //RECARGAR GRILLA
                            fnSistema.spllenaGridView(gridTipoCuenta, "SELECT id, nombre FROM tipocuenta ORDER BY nombre");
                            fnCargarCampos(0);
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

        //ELIMINAR UN TIPO DE CUENTA
        //DATO DE ENTRADA: ID
        private void fnEliminarCuenta(int pId)
        {
            string sql = "DELETE FROM TIPOCUENTA WHERE ID=@pId";
            SqlCommand cmd;
            int res = 0;

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
                                XtraMessageBox.Show("Registro Eliminado", "Eliminar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            //RECARGAR GRILLA
                            fnSistema.spllenaGridView(gridTipoCuenta, "SELECT id, nombre FROM tipocuenta ORDER BY nombre");
                            fnCargarCampos(0);
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
                        XtraMessageBox.Show("No hay conexion con la base de datos", "Base de datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (SqlException ex)
                {
                    XtraMessageBox.Show(ex.Message);
                }
         
        }

        //LIMPIAR CAMPOS
        private void fnLimpiarCampos()
        {
            Update = false;
            txtNombre.Focus();
            txtNombre.Text = "";
            lblError.Visible = false;

            btnEliminar.Enabled = false;
        }

        //PROPIEDADES POR DEFECTO
        private void fnDefaultProperties()
        {
            btnNuevo.TabStop = false;
            btnEliminar.TabStop = false;
            btnGuardar.TabStop = false;
            txtNombre.Properties.MaxLength = 100;
            separador1.TabStop = false;
            gridTipoCuenta.TabStop = false;            
        }

        //COLUMNAS GRILLA
        private void fnColumnas()
        {
            //SELECT ID, NOMBRE FROM TIPOCUENTA

            //NO MOSTRAR ID
            viewTipoCuenta.Columns[0].Caption = "Id";
            viewTipoCuenta.Columns[0].Width = 20;

            viewTipoCuenta.Columns[1].Caption = "Nombre";
            viewTipoCuenta.Columns[1].AppearanceCell.FontStyleDelta = FontStyle.Bold;            
        }

        //VERIFICAR QUE EL TIPO DE CUENTA QUE SE QUIERE INGRESAR NO EXISTA
        private bool fnExisteCuenta(string pNombre)
        {
            string sql = "SELECT * FROM tipocuenta WHERE nombre = @pNombre";
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
                        cmd.Parameters.Add(new SqlParameter("@pNombre", pNombre));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //SI ENCUENTRA REGISTROS ES PORQUE YA EXISTE ESE NOMBRE 
                            existe = true;
                        }
                        else
                        {
                            existe = false;
                        }
                    }
                    //LIBERAR RECURSOS
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                    rd.Close();
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

        //CARGAR CAMPOS
        private void fnCargarCampos(int? pos = -1)
        {
            if (viewTipoCuenta.RowCount > 0)
            {
                if (pos != -1) viewTipoCuenta.FocusedRowHandle = (int)pos;

                //PONEMOS UPDATE EN TRUE
                Update = true;

                btnEliminar.Enabled = true;

                lblError.Visible = false;

                txtNombre.Text = viewTipoCuenta.GetFocusedDataRow()["nombre"].ToString();
            }
        }

        //VERIFICAR SI EL ID A ELIMINAR REALMENTE EXISTE EN BD
        private bool fnExisteIdEliminar(int pId)
        {
            string sql = "SELECT id FROM tipocuenta WHERE id=@pId";
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
                        cmd.Parameters.Add(new SqlParameter("@pId", pId));

                        //EJECUATAR CONSULTA
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //ENCONTRO EL ID
                            existe = true;
                        }
                        else
                        {
                            existe = false;
                        }
                    }
                    //LIBERAR RECURSOS
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                    rd.Close();                    
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

        //VERIFICAR SI EL TIPO DE CUENTA ESTA SIENDO UTILIZADO POR UN EMPLEADO
        private bool fnCuentaUsada(string pCuenta)
        {
            string sql = "SELECT rut, contrato, trabajador.tipoCuenta FROM trabajador" +
                         " INNER JOIN tipoCuenta ON tipocuenta.id = trabajador.tipoCuenta " +
                         " WHERE tipoCuenta.nombre = @pCuenta";

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
                        cmd.Parameters.Add(new SqlParameter("@pCuenta", pCuenta));

                        rd = cmd.ExecuteReader();

                        if (rd.HasRows)
                        {
                            //SI ENCONTRO FILAS ES PORQUE EL TIPO DE CUENTA ESTA SIENDO UTILIZADO POR UN EMPLEADO
                            existe = true;
                        }
                        else
                        {
                            existe = false;
                        }
                    }
                    //LIBERAR RECURSOS
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                    rd.Close();
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

        protected override bool ProcessDialogKey(Keys keyData)
        {
            string nombreAntiguo = "";
            if (keyData == Keys.Tab)
            {
                if (txtNombre.ContainsFocus)
                {
                    if (txtNombre.Text == "") { lblError.Visible = true;lblError.Text = "Por favor ingresa un nombre";return false; }

                    if (Update == false)
                    {
                        if (ExisteCuenta(fnSistema.QuitarEspacios(txtNombre.Text)))
                        {
                            lblError.Visible = true;
                            lblError.Text = "Tipo de cuenta ya ingresada";
                            return false;
                        }
                        else
                        {
                            lblError.Visible = false;
                        }
                    }
                    else
                    {
                        if (viewTipoCuenta.RowCount>0)
                        {
                            nombreAntiguo = viewTipoCuenta.GetFocusedDataRow()["nombre"].ToString();
                            if (nombreAntiguo != txtNombre.Text)
                            {
                                if (ExisteCuenta(fnSistema.QuitarEspacios(txtNombre.Text)))
                                { lblError.Visible = true; lblError.Text = "Tipo de cuenta ya ingresada"; return false;}
                            }
                            else
                            {
                                lblError.Visible = false;
                            }
                        }
                    }
                 
                }
            }

            return base.ProcessDialogKey(keyData);  
        }

        //VERIFICAR SI EXISTE TIPO DE CUENTA
        private bool ExisteCuenta(string pTipoCuenta)
        {
            bool existe = false;
            string sql = "SELECT nombre FROM tipocuenta";
            List<string> lista = new List<string>();
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
                                if (((string)rd["nombre"]).Length > 0)
                                    lista.Add((fnSistema.QuitarEspacios((string)rd["nombre"])).ToLower());
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


            if (lista.Count>0)
            {
                foreach (var item in lista)
                {
                    if (pTipoCuenta.ToLower() == item)
                    { existe = true; break; }
                    else
                        existe = false;
                }
            }

            return existe;
        }

        //PARA LOG MODIFICAR
        private Hashtable PrecargaDatos(int pId)
        {
            string sql = "SELECT id, nombre FROM tipocuenta WHERE id=@pId";
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
                                tabla.Add("nombre", (string)rd["nombre"]);
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

            return tabla;
        }

        //PREGUNTAR SI SON DISTINTOS (GUARDAMOS EN LOG)
        private void ModificaData(Hashtable data, string pNombre)
        {
            if (data.Count>0)
            {
                if ((string)data["nombre"] != pNombre)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA TIPO CUENTA " + (string)data["nombre"], "TIPOCUENTA", (string)data["nombre"], pNombre, "MODIFICAR");
                    log.Log();
                }
            }
        }

        //CAMBIOS ANTES DE CERRAR
        private bool CambiosSinGuardar(int pId)
        {
            string sql = "SELECT nombre FROM tipocuenta WHERE id=@pId";
            SqlCommand cmd;
            string nombre = "";
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pId", pId));
                        nombre = (string)cmd.ExecuteScalar();

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();

                        if (nombre != txtNombre.Text) return true;
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return false;
        }
        #endregion

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
            else if (char.IsNumber(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            fnLimpiarCampos();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (txtNombre.Text == "") { lblError.Visible = true; lblError.Text = "Ingrese un nombre para la cuenta";return;}

            lblError.Visible = false;
            int id = 0;
            string nombreAntiguo = "";

            if (Update)
            {
                //ES UPDATE
                if (viewTipoCuenta.RowCount> 0)
                {
                    id = int.Parse(viewTipoCuenta.GetFocusedDataRow()["id"].ToString());
                    nombreAntiguo = (string)viewTipoCuenta.GetFocusedDataRow()["nombre"];

                    if (nombreAntiguo != txtNombre.Text)
                    {
                        if (ExisteCuenta(txtNombre.Text))
                        { lblError.Visible = true; lblError.Text = "Tipo de cuenta ya ingresada"; return; }
                        else{
                            //SI NO EXISTE ADVERTIMOS SOBRE CAMBIO
                            DialogResult dialogo = XtraMessageBox.Show("¿Realmente deseas modificar el tipo de cuenta " + nombreAntiguo + "?", "Informacion", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                            if (dialogo == DialogResult.Yes)
                            {
                                //MODIFICAMOS
                                fnModificarCuenta(txtNombre, id);
                            }
                        }
                    }
                    else
                    {
                        //SOLO MODIFICAMOS
                        fnModificarCuenta(txtNombre, id);
                    }                                      
                }
                else
                {
                    lblError.Visible = true;
                    lblError.Text = "Registro no valido";                        
                }
            }
            else
            {
                //ES INSERT
                //VERIFICAR SI TIPO DE CUENTA QUE SE QUIERE INGRESAR YA EXISTE
                string cuenta = fnSistema.QuitarEspacios(txtNombre.Text);
                bool existeCuenta = ExisteCuenta(cuenta);
                if (existeCuenta)
                {
                    lblError.Visible = true;
                    lblError.Text = "Tipo de cuenta ya existe";
                }
                else
                {
                    lblError.Visible = false;
                    fnNuevaCuenta(txtNombre);
                }
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (viewTipoCuenta.RowCount > 0)
            {
                int id = int.Parse(viewTipoCuenta.GetFocusedDataRow()["id"].ToString());
                //VERIFICAR QUE EL ID EXISTE EN BD
                string name = viewTipoCuenta.GetFocusedDataRow()["nombre"].ToString();

                bool existeId = fnExisteIdEliminar(id);
                if (existeId)
                {
                    lblError.Visible = false;
                    //VERIFICAR QUE EL TIPO DE CUENTA NO ESTE SIENDO USADO POR UN EMPLEADO
                    bool EmpleadoUsa = fnCuentaUsada(name);
                    if (EmpleadoUsa)
                    {
                        XtraMessageBox.Show("La cuenta " + name + " no se puede eliminar, porque esta siendo utilizada por un trabajador", "Denegado", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }
                    else
                    {
                        DialogResult dialogo = XtraMessageBox.Show("¿Realmente desea eliminar la cuenta " + name  + "?", "Eliminar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (dialogo == DialogResult.Yes)
                        {
                            fnEliminarCuenta(id);
                        }                        
                    }
                }
                else
                {
                    lblError.Visible = true;
                    lblError.Text = "Registro no valido";
                }             
            }
        }

        private void gridTipoCuenta_Click(object sender, EventArgs e)
        {
            fnCargarCampos();
        }

        private void gridTipoCuenta_KeyUp(object sender, KeyEventArgs e)
        {
            fnCargarCampos();
        }

        private void txtNombre_KeyDown(object sender, KeyEventArgs e)
        {
            string nombreAntiguo = "";
            if (e.KeyData == Keys.Enter)
            {
                if (txtNombre.Text == "") { lblError.Visible = true; lblError.Text = "Por favor ingresa un nombre"; return; }

                if (Update == false)
                {
                    if (ExisteCuenta(fnSistema.QuitarEspacios(txtNombre.Text)))
                    {
                        lblError.Visible = true;
                        lblError.Text = "Tipo de cuenta ya ingresada";
                        return;
                    }
                    else
                    {
                        lblError.Visible = false;
                    }
                }
                else
                {
                    if (viewTipoCuenta.RowCount > 0)
                    {
                        nombreAntiguo = viewTipoCuenta.GetFocusedDataRow()["nombre"].ToString();
                        if (nombreAntiguo != txtNombre.Text)
                        {
                            if (ExisteCuenta(fnSistema.QuitarEspacios(txtNombre.Text)))
                            { lblError.Visible = true; lblError.Text = "Tipo de cuenta ya ingresada"; return; }
                        }
                        else
                        {
                            lblError.Visible = false;
                        }
                    }                   
                }
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            int id = 0;
            if (viewTipoCuenta.RowCount > 0)
            {
                id = Convert.ToInt32(viewTipoCuenta.GetFocusedDataRow()["id"]);
                if (CambiosSinGuardar(id))
                {
                    DialogResult adv = XtraMessageBox.Show("Hay cambios sin guardar, ¿Deseas cerrar de todas maneras?", "Informacion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (adv == DialogResult.Yes)
                        Close();
                }
                else
                    Close();
            }
            else
                Close();


        }
    }
}