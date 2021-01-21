using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Labour
{
    public partial class frmUsuario : DevExpress.XtraEditors.XtraForm, IConjuntosCondicionales
    {

        #region "INTERFAZ CONJUNTOS CONDICIONALES"
        public void CargarCodigoConjunto(string code)
        {
            txtCondicion.Text = code;
        }
        #endregion

        public IConjuntosCondicionales opener { get; set; }

        //VARIABLE PARA SABER SI ES UPDATE O NO
        private bool UpdateUsuario = false;

        //UPDATE GRUPO
        private bool UpdateGrupo = false;

        //PARA MANEJAR EL BOTON NUEVO
        private bool Cancelar = false;

        //PARA GUARDAR EL ID DEL GRUPO SELECCIONADO
        private int GrupoSeleccion = 0;
        //PARA GUARDAR EL NOMBRE DEL GRUPO SELECCIONADO
        private string NombreGrupoSeleccion = "";

        //PARA MANIPULAR BOTON CANCELAR GRUPO
        private bool CancelaGrupo = false;




        public frmUsuario()
        {
            InitializeComponent();
            fnDefaultProperties();
        }

        private void frmUsuario_Load(object sender, EventArgs e)
        {
            string sqlGrupo = "";
            
            //CARGAR COMBO      
            datoCombobox.spllenaComboBox("SELECT id, descripcion FROM grupo", txtGrupo, "id", "descripcion", true);

            sqlGrupo = "SELECT id, descripcion FROM grupo";
            fnSistema.spllenaGridView(gridGrupo, sqlGrupo);
            fnSistema.spOpcionesGrilla(viewGrupo);
            ColumnasGrupo();
            CancelaGrupo = true;
            CargarGrupo();                        
        }

        #region "MANEJO DE DATOS"
        //NUEVO USUARIO
        /*
         * PARAMETROS DE ENTRADA:
         * NOMBRE 
         * USUARIO(SISTEMA)
         * PASSWORD
         * FILTRO
         * PERFIL
         */
        private void fnNuevoUsuario(TextEdit pNombre, TextEdit pUser, TextEdit pPass, LookUpEdit pGrupo, MemoEdit pCondicion)
        {
            string sql = "INSERT INTO usuario(nombre, usuario, password, grupo, filtro) VALUES(@pNombre, @pUser, @pPass, @pGrupo, @pCondicion)";
            SqlCommand cmd;
            int res = 0;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pNombre", pNombre.Text));                        
                        cmd.Parameters.Add(new SqlParameter("@pUser", pUser.Text));
                        //GUARDAR FRASE PASSWORD ENCRIPTADA
                        Encriptacion enc = new Encriptacion();                    
                        cmd.Parameters.Add(new SqlParameter("@pPass", enc.EncodePassword(pPass.Text)));
                        cmd.Parameters.Add(new SqlParameter("@pGrupo", Convert.ToInt32(pGrupo.EditValue)));
                        cmd.Parameters.Add(new SqlParameter("@pCondicion", pCondicion.Text == "" ? "0" : pCondicion.Text));

                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            XtraMessageBox.Show("Registro guardado", "Guardar", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            //GUARDAR EVENTO EN LOG
                             logRegistro log = new logRegistro(User.getUser(), "SE HA CREADO UN NUEVO USUARIO CON NOMBRE " + pUser.Text, "USUARIO", "0", pUser.Text, "INGRESAR");
                             log.Log();

                            fnSistema.spllenaGridView(gridUsuarios, string.Format("SELECT nombre, usuario, descripcion, id, filtro, conf FROM usuario INNER JOIN grupo ON " +
                            "usuario.grupo = grupo.id WHERE grupo.id ={0}", Convert.ToInt32(pGrupo.EditValue)));
                            fnSistema.spOpcionesGrilla(viewUsuarios);
                            fnOpcionesGrilla();
                            btnNuevo.Text = "Nuevo";
                            Cancelar = false;

                            // fnLimpiarCampos();       
                            CambiarFocoFila(Convert.ToInt32(pGrupo.EditValue));
                        }
                        else
                        {
                            XtraMessageBox.Show("Error al intentar guardar", "Guardar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
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
        }

        /*NUEVO USUARIO USANDO TRANSACCION*/
        private bool NuevoUsuarioTransaccion(TextEdit pNombre, TextEdit pUser, TextEdit pPass, LookUpEdit pGrupo, TextEdit pCondicion, 
            TextEdit pClaveMaestra, CheckEdit pPrivado, CheckEdit pBloqueo)
        {
            bool transaccionCorrecta = false;
            //SQL INSERTAR
            string sqlUser = "INSERT INTO usuario(nombre, usuario, password, grupo, filtro, llavemaestra, conf) VALUES(@pNombre, @pUser, @pPass, @pGrupo, @pCondicion, @pLlavemaestra, @pPrivado)";
            string sqlBloqueos = "UPDATE usuario SET bloqueo=1, usrbloq=@pUser WHERE usuario <> @pUser AND usuario <> 'SUPER'";
            //SQL INGRESO EN TABLA AUTORIZACION
            //string sqlAutorizacion = "INSERT INTO autorizacion(usuario, objeto, acceso) VALUES(@pUser, @pObjeto, @pAcceso)";            

            SqlCommand cmd;
            SqlTransaction tr;

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    //INICIAMOS TRANSACCION...
                    tr = fnSistema.sqlConn.BeginTransaction();
                    try
                    {
                        //INGRESAMOS USUARIO
                        using (cmd = new SqlCommand(sqlUser, fnSistema.sqlConn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@pNombre", pNombre.Text));
                            cmd.Parameters.Add(new SqlParameter("@pUser", pUser.Text));
                            //PRIMERO DEBEMOS GENERAR EL HASH DEL PASSWORD
                            Encriptacion enc = new Encriptacion();
                            cmd.Parameters.Add(new SqlParameter("@pPass", enc.EncodePassword(pPass.Text)));
                            cmd.Parameters.Add(new SqlParameter("@pGrupo", Convert.ToInt32(pGrupo.EditValue)));
                            cmd.Parameters.Add(new SqlParameter("@pCondicion", pCondicion.Text == "" ? "0" : pCondicion.Text));
                            cmd.Parameters.Add(new SqlParameter("@pLlavemaestra", cbReabreMes.Checked ? enc.EncodePassword(pClaveMaestra.Text): "0"));
                            cmd.Parameters.Add(new SqlParameter("@pPrivado", pPrivado.Checked));                            

                            //AGREGAMOS A TRANSACCION
                            cmd.Transaction = tr;

                            //EJECUTAMOS 
                            cmd.ExecuteNonQuery();

                            cmd.Parameters.Clear();
                        }

                        //BLOQUEAR USUARIOS
                        if (cbBloqueaUsuarios.Checked)
                        {
                            using (cmd = new SqlCommand(sqlBloqueos, fnSistema.sqlConn))
                            {
                                //PARAMETROS    
                                cmd.Parameters.Add(new SqlParameter("@pUser", pUser.Text));                           

                                //AGREGAMOS A TRANSACCION   
                                cmd.Transaction = tr;
                                //EJECUTAMOS
                                cmd.ExecuteNonQuery();
                                //LIMPIAMOS PARAMETROS
                                cmd.Parameters.Clear();
                            }
                        }

                        //LLEGADOS ESTE PUNTO HACEMOS COMMIT
                        tr.Commit();
                        transaccionCorrecta = true;
                        fnSistema.sqlConn.Close();
                    }
                    catch (SqlException ex)
                    {
                        XtraMessageBox.Show(ex.Message);
                        //SI OCURRE ALGUN ERROR HACEMOS ROLLBACK
                        tr.Rollback();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return transaccionCorrecta;
        }

        //MODIFICAR USUARIO
        private void fnModificarUsuario(TextEdit pNombre, TextEdit pUser, LookUpEdit pGrupo, 
            string pUsuarioAntiguo, TextEdit pCondicion, TextEdit pClavemaestra, CheckEdit pPrivado, CheckEdit pBloquea)
        {
            string sql = "UPDATE usuario SET nombre=@pNombre, usuario=@pUsuario, grupo=@pGrupo, filtro=@pCondicion, llavemaestra=@pLlavemaestra, conf=@pPrivado WHERE" +
                " usuario=@pUsuarioAntiguo";

            string SqlBloqueaUsuarios = "UPDATE usuario SET bloqueo = 1, usrbloq=@pUser WHERE (usuario <> @pUser AND usuario <> 'SUPER')";
            string sqlDesbloq = "UPDATE usuario SET bloqueo = 0, usrbloq='0' WHERE usuario=@pUser";
            string sqlDesb = "UPDATE usuario SET bloqueo = 0, usrbloq='0' WHERE usrbloq=@pUser";

            if (fnUsuarioExiste(pUser.Text) && pUser.Text != pUsuarioAntiguo)
            { XtraMessageBox.Show("Usuario ingresado ya existe", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);return; }

            SqlCommand cmd;
            SqlTransaction tr;
            int res = 0;
            //PARA GUARDAR LLAVE MAESTRA DESDE BD
            string llaveMaestrabd = User.GetLlaveMaestra(pUser.Text);
            string key = "";
            bool TransaccionCorrecta = false;
            bool UsuarioBloquea = false;

            //ENCRIPTAR CLAVE   
            Encriptacion enc = new Encriptacion();

            //VER SI USUARIO BLOQUEABA A OTROS
            UsuarioBloquea = User.UsuarioBloquea(pUser.Text);

            //PARA OBTENER LOS DATOS DESDE BD ANTES DE HACER UPDATE
            Hashtable dataUser = new Hashtable();
            dataUser = DataUsuario(pUsuarioAntiguo);

            if (cbReabreMes.Checked)
            {
                //COMPARAMOS CADENA DE BD CON CADENA EN CAJA DE TEXTO
                if (llaveMaestrabd == txtClaveMaestra.Text)
                    key = llaveMaestrabd;
                else
                    key = enc.EncodePassword(txtClaveMaestra.Text);
            }
            else
                key = "0";

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    tr = fnSistema.sqlConn.BeginTransaction();

                    try
                    {                      
                        //MODIFICAR DATOS EMPLEADO
                        using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@pNombre", pNombre.Text));
                            cmd.Parameters.Add(new SqlParameter("@pUsuario", pUser.Text));

                            //cmd.Parameters.Add(new SqlParameter("@pClave", enc.EncodePassword(pPassword.Text)));
                            cmd.Parameters.Add(new SqlParameter("@pUsuarioAntiguo", pUsuarioAntiguo));
                            cmd.Parameters.Add(new SqlParameter("@pGrupo", Convert.ToInt32(pGrupo.EditValue)));
                            cmd.Parameters.Add(new SqlParameter("@pCondicion", pCondicion.Text == "" ? "0" : pCondicion.Text));
                            cmd.Parameters.Add(new SqlParameter("@pLlavemaestra", key));
                            cmd.Parameters.Add(new SqlParameter("@pPrivado", pPrivado.Checked));

                            //AGREGAMOS A TRANSACCION
                            cmd.Transaction = tr;

                            cmd.ExecuteNonQuery();

                            cmd.Parameters.Clear();
                        }

                        /*ACTUALIZAR VALORES DE BLOQUEO SI SE SELECCIONÓ OPCION*/
                        if (cbBloqueaUsuarios.Checked)
                        {
                            //BLOQUEAMOS A TODOS LOS USUARIO MENOS EL USUARIO QUE BLOQUEA
                            using (cmd = new SqlCommand(SqlBloqueaUsuarios, fnSistema.sqlConn))
                            {
                                //PARAMETROS
                                cmd.Parameters.Add(new SqlParameter("@pUser", pUser.Text));

                                //AGREGAMOS A TRANSACCION
                                cmd.Transaction = tr;

                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                            }

                            /*DESBLOQUEAR USUARIO QUE BLOQUEA*/
                            using (cmd = new SqlCommand(sqlDesbloq, fnSistema.sqlConn))
                            {
                                //PARAMETROS
                                cmd.Parameters.Add(new SqlParameter("@pUser", pUser.Text));

                                //AGREGAMOS A TRANSACCION
                                cmd.Transaction = tr;

                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                            }
                        }
                        else
                        {
                            /*SINO ESTÁ SELECCIONADO DESBLOQUEAMOS A LOS USUARIOS QUE HAYAN SIDO BLOQUEADO POR*/
                            /*ESTE USUARIO*/
                            /*DESBLOQUEAR USUARIO QUE BLOQUEA*/
                            using (cmd = new SqlCommand(sqlDesb, fnSistema.sqlConn))
                            {
                                //PARAMETROS
                                cmd.Parameters.Add(new SqlParameter("@pUser", pUser.Text));

                                //AGREGAMOS A TRANSACCION
                                cmd.Transaction = tr;

                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                            }
                        }                  

                        tr.Commit();
                        TransaccionCorrecta = true;
                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                    }
                    catch (SqlException ex)
                    {
                        //ERROR EN TRANSACCION
                        tr.Rollback();
                        TransaccionCorrecta = false;
                    }
            
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            if (TransaccionCorrecta)
            {
                //SI USUARIO ESTABA BLOQUEADO LO DESBLOQUEAMOS
                //User.Desbloquear(pUser.Text);

                XtraMessageBox.Show("Actualizacion realizada correctamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //RECARGAMOS GRILLA        
                fnSistema.spllenaGridView(gridUsuarios, string.Format("SELECT nombre, usuario, descripcion, id, filtro, llavemaestra, conf FROM usuario INNER JOIN grupo ON " +
                "usuario.grupo = grupo.id WHERE grupo.id={0}", Convert.ToInt32(pGrupo.EditValue)));

                //VERIFICAMOS SI HAY CAMBIOS
                ComparaValoresUsuario(dataUser, pNombre.Text, pUser.Text, "", pCondicion.Text == "" ? "0" : pCondicion.Text, Convert.ToInt32(pGrupo.EditValue), "");

                //SI DESACTIVA LA OPCION PRIVADO DEBEMOS VERIFICAR QUE EL LASTVIEW 
                //NO SEA UNA FICHA PRIVADA
                bool priv = Persona.Esprivado(User.GetLastView(User.getUser()), Convert.ToInt32(User.GetLastView(User.getUser(), 1)));

                if (priv && pPrivado.Checked == false)
                { User.UltimoRegistroVisto("0", User.getUser()); }

                fnOpcionesGrilla();
                CargarCampos(0);
                btnNuevo.Text = "Nuevo";
                Cancelar = false;

                //GUARDAMOS EN LOG
                //logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA USUARIO " + pUsuarioAntiguo, "USUARIO", pUsuarioAntiguo, pUser.Text, "MODIFICAR");
                //log.Log();

                CambiarFocoFila(Convert.ToInt32(pGrupo.EditValue));
            }
            else
            {
                XtraMessageBox.Show("No pudo actualizar el registro", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //ELIMINAR USUARIO
        private void fnEliminarUsuario(string pUser)
        {
            if (pUser.Length>0)
            {
                DialogResult pregunta = XtraMessageBox.Show("¿Realmente deseas eliminar el usuario " + pUser + "?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (pregunta == DialogResult.Yes)
                {
                    string sql = "DELETE FROM usuario WHERE usuario=@pUser";
                    SqlCommand cmd;
                    int res = 0;
                    try
                    {
                        if (fnSistema.ConectarSQLServer())
                        {
                            using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                            {
                                //PARAMETROS
                                cmd.Parameters.Add(new SqlParameter("@pUser", pUser));

                                res = cmd.ExecuteNonQuery();
                                if (res > 0)
                                {
                                    XtraMessageBox.Show("Usuario eliminado correctamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    //RECARGAR GRILLA
                                    fnSistema.spllenaGridView(gridUsuarios, string.Format("SELECT nombre, usuario, descripcion, id, filtro, llavemaestra FROM usuario INNER JOIN grupo ON " +
                                    "usuario.grupo = grupo.id WHERE grupo.id={0}", GrupoSeleccion));
                                    fnOpcionesGrilla();

                                    CargarCampos(0);

                                    logRegistro log = new logRegistro(User.getUser(), "SE ELIMINA USUARIO " + pUser, "USUARIO", pUser, "0", "ELIMINAR");
                                    log.Log();
                                }
                                else
                                {
                                    XtraMessageBox.Show("Error al intentar eliminar usuario", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            }
        }

        //ELIMINAR DATOS DE USUARIO DESDE AUTORIZACION
        private bool EliminarTransaccion(string pUser)
        {            
            bool transaccionCorrecta = false;
            bool UsuarioBloquea = false;
            string sqlUser = "DELETE FROM usuario WHERE usuario=@pUser";
            string sqlBloquea = "UPDATE usuario SET bloqueo = 0, usrbloq='0' WHERE usrbloq=@pUser";

            SqlCommand cmd;
            SqlTransaction tr;

            //VERIFICAR SI EL USUARIO QUE SE DESEA ELIMINAR ESTÁ BLOQUEANDO A OTROS USUARIO
            UsuarioBloquea = User.UsuarioBloquea(pUser);

            DialogResult dialogo = XtraMessageBox.Show("¿Realmente deseas eliminar el usuario " + pUser + "?", "Informacion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogo == DialogResult.Yes)
            {
                try
                {
                    if (fnSistema.ConectarSQLServer())
                    {
                        //INICIAMOS TRANSACCION...
                        tr = fnSistema.sqlConn.BeginTransaction();
                        try
                        {
                            //ELIMINAMOS USUARIO DE TABLA USUARIO
                            using (cmd = new SqlCommand(sqlUser, fnSistema.sqlConn))
                            {
                                //PARAMETROS
                                cmd.Parameters.Add(new SqlParameter("@pUser", pUser));

                                //AGREGAMOS A TRANSACCION
                                cmd.Transaction = tr;

                                cmd.ExecuteNonQuery();

                                cmd.Parameters.Clear();
                            }

                            //SI USUARIO BLOQUEA A OTROS USUARIOS...
                            if (UsuarioBloquea)
                            {
                                using (cmd = new SqlCommand(sqlBloquea, fnSistema.sqlConn))
                                {
                                    //PARAMETROS
                                    cmd.Parameters.Add(new SqlParameter("@pUser", pUser));

                                    //AGREGAMOS A TRANSACCION
                                    cmd.Transaction = tr;

                                    cmd.ExecuteNonQuery();
                                    cmd.Parameters.Clear();
                                }
                            }

                            //SI LLEGA A ESTE PUNTO HACEMOS COMMIT
                            tr.Commit();
                            fnSistema.sqlConn.Close();
                            transaccionCorrecta = true;

                        }
                        catch (SqlException ex)
                        {
                            //SI HAY UN ERROR HACEMOS ROLLBACK
                            tr.Rollback();
                        }
                    }
                }
                catch (SqlException ex)
                {
                    XtraMessageBox.Show(ex.Message);
                }
            }
            

            return transaccionCorrecta;
        }

        private bool IngresoObjetosUsuario(string pUser)
        {
            bool transaccionCorrecta = false;
            SqlCommand cmd;
            SqlTransaction tr;
            List<objeto> listado = new List<objeto>();
            listado = objeto.GetListObjetos();

            string sql = "INSERT INTO autorizacion(usuario, objeto, acceso) VALUES(@pUser, @pObjeto, @pAcceso)";

            if (listado.Count>0)
            {
                //INSERTAMOS
                try
                {
                    if (fnSistema.ConectarSQLServer())
                    {
                        //INICIAMOS TRANSACCION
                        tr = fnSistema.sqlConn.BeginTransaction();
                        try
                        {
                            //RECORREMOS LISTADO
                            foreach (var item in listado)
                            {
                                using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                                {
                                    //PARAMETROS
                                    cmd.Parameters.Add(new SqlParameter("@pUser", pUser));
                                    cmd.Parameters.Add(new SqlParameter("@pObjeto", item.Codigo));
                                    cmd.Parameters.Add(new SqlParameter("@pAcceso", item.Acceso));

                                    //AGREGAMOS A TRANSACCION
                                    cmd.Transaction = tr;

                                    //EJECUTAMOS CONSULTA
                                    cmd.ExecuteNonQuery();

                                    cmd.Parameters.Clear();
                                }
                            }

                            //LLEGADO ESTE PUNTO TODO SE REALIZO DE FORMA CORRECTA
                            tr.Commit();
                            transaccionCorrecta = true;
                            fnSistema.sqlConn.Close();                            
                        }
                        catch (SqlException ex)
                        {
                            //SI HAY ERROR HACEMOS ROLLBACK
                            transaccionCorrecta = false;
                            tr.Rollback();
                        }
                    }
                }
                catch (SqlException ex)
                {
                    XtraMessageBox.Show(ex.Message);
                }
            }

            return transaccionCorrecta;
        }

        //LIMPIAR CAMPOS
        private void fnLimpiarCampos()
        {
            txtNombre.Text = "";
            txtNombre.Focus();
            txtPassword.Text = "";
            txtUsuario.Text = "";
            lblerror.Visible = false;
            txtPassword.Enabled = true;            
            txtConfirmaClave.Enabled = true;
            txtConfirmaClave.Text = "";
            UpdateUsuario = false;
            cbAplicaFiltro.Checked = false;
            txtCondicion.Enabled = false;
            txtCondicion.Text = "";
            txtGrupo.ItemIndex = txtGrupo.Properties.GetDataSourceRowIndex("descInfo", NombreGrupoSeleccion);
            btnEliminarUsuario.Enabled = false;
            cbReabreMes.Enabled = true;
            txtClaveMaestra.Text = "";
            txtClaveMaestraConfirma.Text = "";
        }

        //DEFAULT PROPERTIES
        private void fnDefaultProperties()
        {          
            btnNuevo.AllowFocus = false;          
        }

        //MANIPULAR TECLA TAB
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Tab)
            {
                if (txtNombre.ContainsFocus)
                {
                    if (txtNombre.Text == "")
                    {
                        lblerror.Visible = true;
                        lblerror.Text = "Por favor ingrese su nombre";
                        return false;
                    }
                    else
                    {
                        lblerror.Visible = false;
                    }
                }
                else if (txtPassword.ContainsFocus)
                {
                    if (txtPassword.Text == "")
                    {
                        lblerror.Visible = true;
                        lblerror.Text = "Por favor ingrese su contraseña";
                        return false;
                    }
                    else
                    {
                        lblerror.Visible = false;                        
                    }
                }
                else if (txtUsuario.ContainsFocus)
                {
                    if (txtUsuario.Text == "")
                    {
                        lblerror.Visible = true;
                        lblerror.Text = "Debes ingresar un nombre de usuario";
                        return false;
                    }
                    else
                    {
                        if (UpdateUsuario == false)
                        {
                            lblerror.Visible = false;
                            //VALIDAR QUE EL NOMBRE DE USUARIO NO EXISTE EN BD
                            bool existe = false;
                            existe = fnUsuarioExiste(txtUsuario.Text);
                            if (existe)
                            {
                                lblerror.Visible = true;
                                lblerror.Text = "Nombre de usuario ya registrado";
                                return false;
                            }
                            else
                            {
                                lblerror.Visible = false;
                            }
                        }
                        else
                        {
                            string userBD = "";
                            if (viewUsuarios.RowCount > 0)
                            {
                                userBD = (string)viewUsuarios.GetFocusedDataRow()["usuario"];

                                if (txtUsuario.Text == "") { lblerror.Visible = true; lblerror.Text = "Por favor ingresa un usuario"; return false; }

                                if (txtUsuario.Text.ToLower() != userBD.ToLower())
                                {
                                    if (fnUsuarioExiste(txtUsuario.Text))
                                    {
                                        lblerror.Visible = true;
                                        lblerror.Text = "Usuario ingresado ya existe";
                                        return false;
                                    }
                                }

                                lblerror.Visible = false;
                            }
                        }
                    }
                }
                else if (txtClaveMaestra.ContainsFocus)
                {
                    if (txtClaveMaestra.Text == "")
                    { lblerror.Visible = true; lblerror.Text = "Por favor ingresa clave maestra"; txtClaveMaestra.Focus(); return false; }

                    lblerror.Visible = false;
                }
                else if (txtClaveMaestraConfirma.ContainsFocus)
                {
                    if (txtClaveMaestraConfirma.Text == "")
                    { lblerror.Visible = true; lblerror.Text = "Por favor confirma clave maestra"; txtClaveMaestraConfirma.Focus(); return false; }

                    lblerror.Visible = false;

                    if (User.ComparaContraseñas(txtClaveMaestra.Text, txtClaveMaestraConfirma.Text) == false)
                    { lblerror.Visible = true; lblerror.Text = "Las claves maestras no coinciden"; txtClaveMaestra.Focus(); return false; }

                    lblerror.Visible = false;
                }
            }

            return base.ProcessDialogKey(keyData);
        }

        //VALIDAR QUE EL USUARIO A GUARDAR NO ESTE EN USO
        private bool fnUsuarioExiste(string pUser)
        {
            string sql = "SELECT usuario FROM usuario WHERE usuario=@pUser";
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
                        cmd.Parameters.Add(new SqlParameter("@pUser", pUser));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //SI RETORNA REGISTROS ES PORQUE EXISTE
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
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
            return existe;
        }

        //OPCIONES GRILLA
        private void fnOpcionesGrilla()
        {
            if (viewUsuarios.RowCount > 0)
            {
                viewUsuarios.Columns[0].Caption = "Nombre";
                viewUsuarios.Columns[0].Width = 100;

                viewUsuarios.Columns[1].Caption = "Usuario";
                viewUsuarios.Columns[2].Caption = "Grupo";
                viewUsuarios.Columns[3].Visible = false;
                viewUsuarios.Columns[4].Visible = false;
                //viewUsuarios.Columns[5].Caption = "Sesion abierta";
                //viewUsuarios.Columns[5].DisplayFormat.FormatString = "online";
                //viewUsuarios.Columns[5].DisplayFormat.Format = new FormatCustom();
                viewUsuarios.Columns[5].Visible = false;
                viewUsuarios.Columns[6].Visible = false;
                viewUsuarios.Columns[7].Visible = false;
            }

            
        }

        //CARGAR DATOS EN CAMPOS DESDE GRILLA
        private void CargarCampos(int? pos = -1)
        {
            if (viewUsuarios.RowCount>0)
            {
                if (pos == 0) viewUsuarios.FocusedRowHandle = 0;

                string filtro = "", llavemaestra = "";
                bool Bloquea = false;

                //ACTUALIZA
                UpdateUsuario = true;

                btnEliminarUsuario.Enabled = true;                       

                //CARGAMOS CAMPOS
                txtNombre.Text = (string)viewUsuarios.GetFocusedDataRow()["nombre"];
                txtUsuario.Text = (string)viewUsuarios.GetFocusedDataRow()["usuario"];
                filtro = (string)viewUsuarios.GetFocusedDataRow()["filtro"];
                txtGrupo.EditValue = Convert.ToInt32(viewUsuarios.GetFocusedDataRow()["id"]);
                llavemaestra = (string)viewUsuarios.GetFocusedDataRow()["llavemaestra"];
                cbPrivada.Checked = (bool)viewUsuarios.GetFocusedDataRow()["conf"];

                //VERIFICAMOS SI USUARIO BLOQUEA A OTROS USUARIOS
                if (User.UsuarioBloquea((string)viewUsuarios.GetFocusedDataRow()["usuario"]))
                    cbBloqueaUsuarios.Checked = true;
                else
                    cbBloqueaUsuarios.Checked = false;

                if (llavemaestra == "0")
                {
                    cbReabreMes.Checked = false;
                    txtClaveMaestra.Enabled = false;
                    txtClaveMaestra.Text = "";
                    txtClaveMaestraConfirma.Enabled = false;
                    txtClaveMaestraConfirma.Text = "";
                }
                else
                {
                    cbReabreMes.Checked = true;
                    txtClaveMaestra.Enabled = true;
                    txtClaveMaestra.Text = llavemaestra;
                    txtClaveMaestraConfirma.Enabled = true;
                    txtClaveMaestraConfirma.Text = llavemaestra;
                }

                if (filtro == "0")
                {
                    cbAplicaFiltro.Checked = false;
                    txtCondicion.Enabled = false;
                }
                else
                {
                    cbAplicaFiltro.Checked = true;
                    txtCondicion.Enabled = true;
                    txtCondicion.Text = filtro;
                }                

                txtPassword.Enabled = false;
                txtPassword.Text = "";
                txtConfirmaClave.Enabled = false;
                txtConfirmaClave.Text = "";

                Cancelar = false;
                btnNuevo.Text = "Nuevo";

                if (lblerror.Visible)
                    lblerror.Visible = false;
            }
            else
            {
                txtNombre.Text = "";
                txtPassword.Text = "";
                txtUsuario.Text = "";
                lblerror.Visible = false;
                txtPassword.Enabled = true;
                txtConfirmaClave.Enabled = true;
                txtConfirmaClave.Text = "";
                UpdateUsuario = false;
                cbAplicaFiltro.Checked = false;
                txtCondicion.Enabled = false;
                txtCondicion.Text = "";
                cbReabreMes.Checked = false;
                txtClaveMaestra.Text = "";
                txtClaveMaestraConfirma.Text = "";
            }
        }

        //LISTADO PARA BUSQUEDA VALIDA
        private List<string> ListaCampos()
        {
            List<string> lista = new List<string>();
            lista.Add("AFP");
            lista.Add("SALUD");
            lista.Add("TRAMO");
            lista.Add("BANCO");
            lista.Add("FORMAPAGO");
            lista.Add("TIPOCUENTA");
            lista.Add("CLASE");
            lista.Add("CAUSAL");
            lista.Add("SEXO");
            lista.Add("CONTRATO");
            lista.Add("RUT");
            lista.Add("NOMBRE");
            lista.Add("APEPATERNO");
            lista.Add("APEMATERNO");
            lista.Add("CIUDAD");
            lista.Add("AREA");
            lista.Add("CCOSTO");
            lista.Add("NACION");
            lista.Add("ECIVIL");
            lista.Add("INGRESO");
            lista.Add("SALIDA");
            lista.Add("CARGO");
            lista.Add("TIPOCONTRATO");

            return lista;
        }

        //...
        private List<string> ListaLogicos()
        {
            List<string> data = new List<string>();
            data.Add("AND");
            data.Add("OR");
            data.Add("LIKE");
            data.Add("BETWEEN");
            data.Add(">");
            data.Add("<");
            data.Add("=");
            data.Add(">=");
            data.Add("<=");

            return data;
        }

        //GENERA COMNSULTA DE PRUEBA PARA VER SI LA SINTAXIS ES CORRECTA
        private bool PruebaFiltro(string condicion)
        {
            if (condicion.Length>0)
            {
                string sql = string.Format("SELECT contrato FROM trabajador WHERE {0}", condicion);
                SqlCommand cmd;
                SqlDataReader rd;
                try
                {
                    if (fnSistema.ConectarSQLServer())
                    {
                        using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                        {
                            //PARAMETROS
                            //cmd.Parameters.Add(new SqlParameter("@pCondicion", condicion));

                            rd = cmd.ExecuteReader();

                            cmd.Dispose();
                            fnSistema.sqlConn.Close();
                            rd.Close();
                            return true;
                        }                       
                       
                    }
                }
                catch (SqlException ex)
                {
                    //XtraMessageBox.Show(ex.Message);
                    return false;
                }
            }

            return false;
        }

        //VERIFICAR SI HAY CAMBIOS SIN GUARDAR
        private bool CambiosSinGuardar(string pUser)
        {
            string sql = "SELECT nombre, usuario, filtro, grupo FROM usuario WHERE usuario=@pUser";
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pUser", pUser));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //COMPARAR
                                if (txtNombre.Text != (string)rd["nombre"]) return true;
                                if (txtUsuario.Text != (string)rd["usuario"]) return true;
                                if ((string)rd["filtro"] == "0" && txtCondicion.Text != "") return true;
                                if ((string)rd["filtro"] != "0" && txtCondicion.Text == "") return true;                               
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

        //CAMBIAR FOCO FILA SELECCIONADA SI EL USUARIO SE CAMBIA DE GRUPO
        private void CambiarFocoFila(int pIdGroup)
        {
            int idRow = 0;
            if (viewGrupo.RowCount>0)
            {
                for (int i = 0; i < viewGrupo.DataRowCount; i++)
                {
                    idRow = Convert.ToInt32(viewGrupo.GetRowCellValue(i, "id"));
                    
                    if (idRow == pIdGroup)
                        viewGrupo.FocusedRowHandle = i;
                }
            }
        }

        //VERIFICAR SI EL USUARIO ESTA ACTUALMENTE LOGUEADO
        private bool UsuarioLogueado(string pUser)
        {
            if (pUser.Length>0)
            {
               
                if (pUser.ToLower() == User.getUser().ToLower())
                    return true;
                else
                    return false;
            }

            return false;            
        }

        #region "LOG USUARIO"
        //OBTENER DATOS DEL USUARIO DESDE BD
        private Hashtable DataUsuario(string pUser)
        {
            Hashtable data = new Hashtable();
            string sql = "SELECT nombre, usuario, password, filtro, grupo, llavemaestra FROM usuario" +
                " WHERE usuario=@pUser";
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pUser", pUser));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //GUARDAMOS DATOS EN HASTABLE
                                data.Add("nombre", (string)rd["nombre"]);
                                data.Add("usuario", (string)rd["usuario"]);
                                data.Add("password", (string)rd["usuario"]);
                                data.Add("filtro", (string)rd["filtro"]);
                                data.Add("grupo", (int)rd["grupo"]);
                                data.Add("llavemaestra", (string)rd["llavemaestra"]);
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

        //COMPARAR VALORES
        private void ComparaValoresUsuario(Hashtable pData, string pNombre, string pUsuario, string pPasswd, 
            string pFiltro, int pGrupo, string pLlaveMaestra)
        {
            if (pData.Count>0)
            {
                if ((string)pData["nombre"] != pNombre)
                {
                    logRegistro log = new logRegistro(User.getUser(), $"Se modifica nombre usuario {pData["nombre"]}", "USUARIO", (string)pData["nombre"], pNombre, "MODIFICAR");
                    log.Log();
                }
                if ((string)pData["usuario"] != pUsuario)
                {
                    logRegistro log = new logRegistro(User.getUser(), $"Se modifica nombre usuario {pData["usuario"]}", "USUARIO", (string)pData["usuario"], pUsuario, "MODIFICAR");
                    log.Log();
                }
                if ((string)pData["password"] != pPasswd)
                {
                    logRegistro log = new logRegistro(User.getUser(), $"Se modifica password usuario {pData["usuario"]}", "USUARIO", "", "", "MODIFICAR");
                    log.Log();
                }
                if ((string)pData["filtro"] != pFiltro)
                {
                    logRegistro log = new logRegistro(User.getUser(), $"Se modifica filtro usuario {pData["usuario"]}", "USUARIO", (string)pData["filtro"], pFiltro, "MODIFICAR");
                    log.Log();
                }
                if ((int)pData["grupo"] != pGrupo)
                {
                    logRegistro log = new logRegistro(User.getUser(), $"Se modifica grupo usuario {pData["nombre"]}", "USUARIO", Convert.ToInt32(pData["grupo"]).ToString(), pGrupo.ToString(), "MODIFICAR");
                    log.Log();
                }
                if ((string)pData["llavemaestra"] != pLlaveMaestra)
                {
                    logRegistro log = new logRegistro(User.getUser(), $"Se modifica llave maestra usuario {pData["usuario"]}", "USUARIO", "", "", "MODIFICAR");
                    log.Log();
                }
            }
        }

        #endregion
        #endregion

        #region "MANEJO GRUPO"
        //OPCIONES COLUMNAS
        private void ColumnasGrupo()
        {
            viewGrupo.Columns[0].Visible = false;
            viewGrupo.Columns[1].Caption = "Nombre";
            viewGrupo.Columns[1].Width = 150;
        }

        //INGRESAR NUEVO GRUPO
        private void NuevoGrupo(TextEdit pName)
        {
            string sql = "INSERT INTO grupo(descripcion) values(@pName)";
            string sqlAutorizacion = "DECLARE @HayData AS INTEGER " +
                                      "SET @HayData = (SELECT count(*) FROM autorizacion WHERE grupo = @@Identity) " +
                                      "IF(@HayData = 0) " +
                                           "BEGIN " +
                                               "SELECT @@Identity as grupo, codobjeto, 0 as acceso, 0 as lectura, 0 as escritura, 0 as borra  INTO #autori " +
                                               "FROM objeto " +
                                               "INSERT INTO autorizacion " +
                                               "SELECT grupo, codobjeto, acceso, lectura, escritura, borra FROM #autori " +
                                            "END";
            SqlCommand cmd;
            SqlTransaction tr;
            bool tranCorrecta = false;
            int res = 0;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    tr = fnSistema.sqlConn.BeginTransaction();
                    try
                    {
                        //CREAR GRUPO
                        using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@pName", pName.Text));

                            cmd.Transaction = tr;
                            res = cmd.ExecuteNonQuery();

                            cmd.Parameters.Clear();
                            cmd.Dispose();
                        }

                        //AGREGAR DATOS A TABLA TAUTORIZACION
                        using (cmd = new SqlCommand(sqlAutorizacion, fnSistema.sqlConn))
                        {
                            cmd.Transaction = tr;
                            cmd.ExecuteNonQuery();

                            cmd.Dispose();
                        }

                        //HACEMOS COMMIT
                        tr.Commit();
                        tranCorrecta = true;
                    }
                    catch (SqlException ex)
                    {
                        tranCorrecta = false;
                        //ERROR
                        tr.Rollback();                       
                    }                                   
                }

                fnSistema.sqlConn.Close();
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);                
            }

            if (tranCorrecta)
            {
                XtraMessageBox.Show("Grupo guardado correctamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);

                logRegistro log = new logRegistro(User.getUser(), "SE INGRESA NUEVO GRUPO", "GRUPO", "0", pName.Text, "INGRESAR");
                log.Log();

                //CARGAMOS GRILLA
                fnSistema.spllenaGridView(gridGrupo, "SELECT id, descripcion FROM grupo");
                fnSistema.spOpcionesGrilla(viewGrupo);
                ColumnasGrupo();
                lblGrupo.Visible = false;
                btnNuevoGrupo.Text = "Nuevo";
                CancelaGrupo = true;

                //ACTULIZAR COMBOBOX 
                datoCombobox.spllenaComboBox("SELECT id, descripcion FROM grupo", txtGrupo, "id", "descripcion", true);

                //DEJAR EL FOCO EN EL NUEVO GRUPO INGRESADO
                FocoUltimoGrupo(pName.Text);

                CargarGrupo();

                txtGrupo.EditValue = GrupoSeleccion;
            }
            else
            {
                XtraMessageBox.Show("No se pudo guardar grupo", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        //MODIFICAR GRUPO
        private void ModificarGrupo(TextEdit pName, string pOldName, int pId)
        {
            string sql = "UPDATE grupo SET descripcion=@pName WHERE id=@pId";
            SqlCommand cmd;
            int res = 0;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pName", pName.Text));
                        cmd.Parameters.Add(new SqlParameter("@pId", pId));

                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            XtraMessageBox.Show("Modificacion correcta", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA DESCRIPCION GRUPO", "GRUPO", pOldName, pName.Text, "MODIFICAR");
                            log.Log();

                            //CARGAR GRILLA
                            fnSistema.spllenaGridView(gridGrupo, "SELECT id, descripcion FROM grupo");
                            fnSistema.spOpcionesGrilla(viewGrupo);
                            ColumnasGrupo();
                            lblGrupo.Visible = false;
                            btnNuevoGrupo.Text = "Nuevo";
                            CancelaGrupo = true;

                            //ACTUALIZAR COMBOBOX
                            datoCombobox.spllenaComboBox("SELECT id, descripcion FROM grupo", txtGrupo, "id", "descripcion", true);
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

        //LISTADO DE USUARIOS ASOCIADO AL GRUPO
        private List<string> ListUser(int pGrupo)
        {
            List<string> usuarios = new List<string>();

            string sql = "SELECT usuario FROM usuario WHERE grupo=@pGrupo";
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pGrupo", pGrupo));
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //INGRESAMOS EN LISTA
                                usuarios.Add((string)rd["usuario"]);
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

            return usuarios;
        }

        //ELIMINAR GRUPOS Y USUARIOS USANDO TRANSACCION
        private bool EliminarGroupTransaccion(int pIdGroup, string pNameGroup, List<string> listaUsers, bool? tieneUsuarios = false)
        {
            string sqlGrupo = "DELETE from grupo WHERE id=@pId";
            string sqlUsuario = "DELETE from usuario WHERE grupo=@pGrupo";
            string sqlUsuarioData = "DELETE FROM autorizacion WHERE grupo=@pGrupo";
            string sqlUpdateBloq = "UPDATE USUARIO set bloqueo=0, usrbloq='0' " +
                                    "FROM usuario " +
                                    "INNER JOIN grupo ON grupo.id = usuario.grupo " +
                                    "WHERE usrbloq IN(SELECT usuario FROM usuario WHERE grupo = @pGrupo)";
            
            SqlCommand cmd;
            SqlTransaction tr;
            bool transaccionCorrecta = false;

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    //INICIAMOS LA TRANSACCION...
                    tr = fnSistema.sqlConn.BeginTransaction();
                    try
                    {
                        /*ACTUALIZAMOS DATOS DE USUARIOS SI ALGUNO DE LOS USUARIOS DEL GRUPO ESTABA REALIZANDO BLOQUEO*/
                        using (cmd = new SqlCommand(sqlUpdateBloq, fnSistema.sqlConn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@pGrupo", pIdGroup));

                            //AGREGAMOS A TRANSACCION
                            cmd.Transaction = tr;

                            //EJECUTAMOS
                            cmd.ExecuteNonQuery();

                            cmd.Parameters.Clear();
                        }

                        /*ELIMINAR USUARIO ASOCIADOS A GRUPO SI HAY*/
                        if ((bool)tieneUsuarios)
                        {
                            //ELIMINAMOS USUARIOS ASOCIADOS A GRUPO
                            using (cmd = new SqlCommand(sqlUsuario, fnSistema.sqlConn))
                            {
                                //PARAMETROS
                                cmd.Parameters.Add(new SqlParameter("@pGrupo", pIdGroup));

                                //AGREGAMOS A TRANSACCION
                                cmd.Transaction = tr;

                                //EJECUTAMOS
                                cmd.ExecuteNonQuery();

                                cmd.Parameters.Clear();
                            }                                              
                        }

                        //ELIMINAMOS GRUPO DE TABLA AUTORIZACION
                        using (cmd = new SqlCommand(sqlUsuarioData, fnSistema.sqlConn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@pGrupo", pIdGroup));

                            cmd.Transaction = tr;
                            cmd.ExecuteNonQuery();

                            cmd.Parameters.Clear();
                        }

                        //ELIMINAMOS GRUPO
                        using (cmd = new SqlCommand(sqlGrupo, fnSistema.sqlConn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@pId", pIdGroup));

                            //AGREGAMOS A TRANSACCION
                            cmd.Transaction = tr;

                            //EJECUTAMOS
                            cmd.ExecuteNonQuery();

                            cmd.Parameters.Clear();
                        }

                        //SI LLEGA A ESTE PUNTO TODO SE HIZO DE FORMA CORRECTA
                        tr.Commit();
                        transaccionCorrecta = true;
                        cmd.Dispose();
                        fnSistema.sqlConn.Close();

                    }
                    catch (SqlException ex)
                    {
                        //SI HAY ALGUN ERROR REALIZAMOS ROLLBACK
                        transaccionCorrecta = false;
                        tr.Rollback();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return transaccionCorrecta;
        }

        //VERIFICAR GRUPO
        private bool ExisteGrupo(string pName)
        {
            bool existe = false;
            if (pName.Length>0)
            {
                string sql = "SELECT descripcion FROM grupo WHERE descripcion=@pName";
                SqlCommand cmd;
                SqlDataReader rd;
                try
                {
                    if (fnSistema.ConectarSQLServer())
                    {
                        using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@pName", pName));
                            rd = cmd.ExecuteReader();
                            if (rd.HasRows)
                            {
                                //EXISTE
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
            }

            return existe;
        }

        //VERIFICAR SI EL GRUPO TIENE USUARIO ASOCIADOS
        private bool GrupoTieneUsuarios(int pId)
        {
            bool tiene = false;
            string sql = "SELECT usuario FROM usuario WHERE grupo=@pId";
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
                            //EXISTEN USUARIOS ASOCIADOS A ESE GRUPO...
                            tiene = true;
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

            return tiene;
        }         

        //CARGAR GRUPO
        private void CargarGrupo()
        {
            int idGrupo = 0;            
            //CARGAR CAMPOS
            UpdateGrupo = true;
            string grupo = (string)viewGrupo.GetFocusedDataRow()["descripcion"];
            idGrupo = Convert.ToInt32(viewGrupo.GetFocusedDataRow()["id"]);
            //CARGAMOS GRUPO EN CAJA
            txtNombreGrupo.Text = grupo;
            //CAMBIAR EL NOMBRE DEL GROUPCONTROL DE ACUERDO A GRUPO SELECCIONADO
            groupUsuariosGrupo.Text = $"Usuarios grupo {grupo}";
            groupPermisosGrupo.Text = $"Permisos grupo {grupo}";
            
            string Sqlusuarios = $"SELECT nombre, usuario, descripcion, id, filtro, llavemaestra, conf, bloqueo FROM usuario INNER JOIN grupo ON usuario.grupo = grupo.id AND descripcion='{grupo}'";
            //CARGAR PERMISOS
            string Sqlpermisos = $"SELECT objeto.descObjeto as objeto, objeto.codObjeto FROM autorizacion " +
                                 "INNER JOIN objeto ON objeto.codObjeto = autorizacion.objeto " +
                                 $"WHERE grupo = {idGrupo}";

            fnSistema.spllenaGridView(gridPermisos, Sqlpermisos);
            fnSistema.spllenaGridView(gridUsuarios, Sqlusuarios);
            fnSistema.spOpcionesGrilla(viewUsuarios);
            ColumnasPermiso();
            PropGridPermisos(viewPermisos);
            fnOpcionesGrilla();
            lblGrupo.Visible = false;
            btnEliminar.Enabled = true;
            GrupoSeleccion = idGrupo;
            NombreGrupoSeleccion = grupo;
            txtGrupo.ItemIndex = txtGrupo.Properties.GetDataSourceRowIndex("descInfo", grupo);

            Cancelar = false;
            btnNuevo.Text = "Nuevo";

            UpdateUsuario = false;
            CargarCampos();

            //CARGAR PERMISOS ACORDES AL GRUPO SELECCIONADO!
            List<objeto> ventanas = new List<objeto>();
            ventanas = CargarDatos(idGrupo);

            //SELECCIONAMOS CHECKBOX EN GRILLA (SI TIENE ACCESO 1)
            if (ventanas.Count > 0)
                SetSelection(ventanas);
        }

        //CARGAR EN GRILLA NUEVO GRUPO INGRESADO
        private void FocoUltimoGrupo(string pName)
        {
            string name = "";
            //RECORREMOS GRILLA GRUPO
            if (viewGrupo.RowCount>0)
            {
                for (int i = 0; i < viewGrupo.DataRowCount; i++)
                {
                    name = (string)viewGrupo.GetRowCellValue(i, "descripcion");
                    if (name == pName)
                        viewGrupo.FocusedRowHandle = i;
                }
            }
        }

        /*CARGAR TODOS LOS OBJETOS DENTRO DE TABLA AUTORIZACION JUNTO CON EL NOMBRE DEL GRUPO*/
        private void CargarAutorizacion(int pGroup)
        {
            string sql = "DECLARE @grupo AS INTEGER " +
                         "DECLARE @HayData AS INTEGER " +
                         "SET @HayData = (SELECT count(*) FROM autorizacion WHERE grupo = @grupo) " +
                         "IF(@HayData = 0) " +
                            "BEGIN " +
                                "SELECT @grupo as grupo, codobjeto, 0 as acceso INTO #autori " +
                                "FROM objeto " +
                                "INSERT INTO autorizacion " +
                                "SELECT grupo, codobjeto, acceso FROM #autori " +
                             "END";

            SqlCommand cmd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@grupo", pGroup));

                        cmd.ExecuteNonQuery();
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

        #endregion


        #region "PERMISOS GRUPO"

        //PROPIEDADES GRILLA PERMISOS
        private void PropGridPermisos(DevExpress.XtraGrid.Views.Grid.GridView pGridView)
        {
            pGridView.OptionsSelection.EnableAppearanceHideSelection = false;
            pGridView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFullFocus;
            pGridView.OptionsBehavior.Editable = false;
            pGridView.OptionsSelection.EnableAppearanceFocusedCell = false;
            //PARA LA SELECCION USANDO CHECKBOX
            pGridView.OptionsSelection.MultiSelect = true;
            pGridView.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;

            //deshabilitar menus contextuales
            pGridView.OptionsMenu.EnableColumnMenu = false;
            pGridView.OptionsMenu.EnableFooterMenu = false;
            pGridView.OptionsMenu.EnableGroupPanelMenu = false;

            //evitar filtrar por columnas y Ordenar por Columnas
            pGridView.OptionsCustomization.AllowFilter = false;
            pGridView.OptionsCustomization.AllowGroup = false;
            pGridView.OptionsCustomization.AllowSort = false;
            pGridView.OptionsCustomization.AllowColumnResizing = false;
            pGridView.OptionsCustomization.AllowColumnMoving = false;

            //deshabilitar cabezera de la tabla
            pGridView.OptionsView.ShowGroupPanel = false;
        }

        //COLUMNAS GRILLA
        //GRUPO - OBJETO - ACCESO
        private void ColumnasPermiso()
        {
            if (viewPermisos.RowCount > 0)
            {
                viewPermisos.Columns[0].Caption = "Ventana";
                viewPermisos.Columns[1].Visible = false;
            }            
        }

        //LISTA DE REGISTROS ASOCIADOS A GRUPO DESDE BD
        private List<objeto> CargarDatos(int pGrupo)
        {
            List<objeto> listado = new List<objeto>();
            if (viewGrupo.RowCount > 0)
            {
                string sql = "SELECT objeto.descObjeto as objeto, acceso FROM autorizacion " +
                            "INNER JOIN objeto ON objeto.codObjeto = autorizacion.objeto " +
                            "WHERE grupo=@pGrupo";
                SqlCommand cmd;
                SqlDataReader rd;
                try
                {
                    if (fnSistema.ConectarSQLServer())
                    {
                        using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@pGrupo", pGrupo));
                            rd = cmd.ExecuteReader();
                            if (rd.HasRows)
                            {
                                while (rd.Read())
                                {
                                    //LLENAMOS LISTA
                                    listado.Add(new objeto() { Codigo = (string)rd["objeto"], Acceso = Convert.ToInt16(rd["acceso"]) });
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

                //RETORNAMOS LISTA
                return listado;
            }
            else
            {
                return null;
            }
        }

        //SELECCIONAR CHECKBOXS GRIDVIEW EN BASE A LISTADO
        private void SetSelection(List<objeto> pLista)
        {
            if (pLista.Count > 0 && viewPermisos.RowCount > 0)
            {
                //RECORREMOS GRILLA 
                for (int i = 0; i < viewPermisos.DataRowCount; i++)
                {
                    //OBTENER EL CODIGO DEL OBJETO
                    string code = ((string)viewPermisos.GetRowCellValue(i, "objeto")).ToLower();

                    //RECORREMOS LISTA
                    foreach (var objeto in pLista)
                    {
                        if ((code == objeto.Codigo.ToLower()) && objeto.Acceso == 1)
                            viewPermisos.SelectRow(i);
                        else if ((code == objeto.Codigo.ToLower()) && objeto.Acceso == 0)
                            viewPermisos.UnselectRow(i);
                    }
                }
            }
        }

        //MODIFICAR TABLA AUTORIZACION
        private bool ModificaAutorizacion(int pGrupo, List<objeto> pLista)
        {
            /*VERIFICAR SI OBJETO EXISTE*/
            /*SI OBJETO EXISTE HACEMOS UPDATE*/
            /*SI OBJETO NO EXISTE HACEMOS INSERT*/
            bool transaccionCorrecta = false;
            string sqlUpdate = "UPDATE autorizacion SET acceso=@pAcceso WHERE Grupo=@pGrupo AND objeto=@pObjeto";
            string sqlInsert = "INSERT INTO autorizacion(grupo, objeto, acceso) VALUES(@pGrupo, @pObjeto, @pAcceso)";
            string sqlObjetoExiste = "SELECT count(*) FROM AUTORIZACION WHERE grupo=@pGrupo AND objeto=@pObjeto";
            int count;

            SqlCommand cmd;
            SqlTransaction tr;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    tr = fnSistema.sqlConn.BeginTransaction();
                    //RECORREMOS LISTADO
                    try
                    {
                        foreach (var item in pLista)
                        {
                            count = 0;

                            //PREGUNTAMOS SI OBJETO EXISTE
                            using (cmd = new SqlCommand(sqlObjetoExiste, fnSistema.sqlConn))
                            {
                                //PARAMETROS
                                cmd.Parameters.Add(new SqlParameter("@pGrupo", pGrupo));
                                cmd.Parameters.Add(new SqlParameter("@pObjeto", item.Codigo));

                                cmd.Transaction = tr;
                                count = Convert.ToInt32(cmd.ExecuteScalar());

                                cmd.Parameters.Clear();
                            }

                            //EXISTE OBJETO
                            if (count > 0)
                            {
                                //UPDATE
                                using (cmd = new SqlCommand(sqlUpdate, fnSistema.sqlConn))
                                {
                                    //PARAMETROS
                                    cmd.Parameters.Add(new SqlParameter("@pGrupo", pGrupo));
                                    cmd.Parameters.Add(new SqlParameter("@pObjeto", item.Codigo));
                                    cmd.Parameters.Add(new SqlParameter("@pAcceso", item.Acceso));

                                    //AGREGAMOS A TRANSACCION
                                    cmd.Transaction = tr;

                                    //EJECUTAMOS SENTENCIA
                                    cmd.ExecuteNonQuery();

                                    cmd.Parameters.Clear();
                                }
                            }
                            else
                            {
                                //INSERT
                                using (cmd = new SqlCommand(sqlInsert, fnSistema.sqlConn))
                                {
                                    //PARAMETROS
                                    cmd.Parameters.Add(new SqlParameter("@pUser", pGrupo));
                                    cmd.Parameters.Add(new SqlParameter("@pObjeto", item.Codigo));
                                    cmd.Parameters.Add(new SqlParameter("@pAcceso", item.Acceso));

                                    //AGREGAMOS A TRANSACCION
                                    cmd.Transaction = tr;

                                    //EJECUTAMOS SENTENCIA
                                    cmd.ExecuteNonQuery();

                                    cmd.Parameters.Clear();
                                }
                            }
                        }

                        transaccionCorrecta = true;
                    }
                    catch (SqlException ex)
                    {
                        transaccionCorrecta = false;
                        //SI SE PRODUCE UN ERROR HACEMOS ROLLBACK
                        tr.Rollback();
                    }

                    if (transaccionCorrecta)
                    {
                        tr.Commit();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return transaccionCorrecta;
        }

        //EXTRAER DE LA GRILLA LAS FILAS SELECCIONADAS
        private List<objeto> FilasSeleccionadas()
        {
            List<objeto> lista = new List<objeto>();
            if (viewPermisos.RowCount > 0)
            {
                //OBTENER TODAS LAS FILAS
                //ITERAR CADA FINAL DEL GRID
                for (int i = 0; i < viewPermisos.DataRowCount; i++)
                {
                    //PREGUNTAMOS SI LA FILA ESTA SELECCIONADA
                    if (viewPermisos.IsRowSelected(i))
                    {
                        //GUARDAMOS CON ACCESO 1 (SI ESTA SELECCIONADA)
                        lista.Add(new objeto()
                        {
                            Codigo = (string)viewPermisos.GetRowCellValue(i, "codObjeto"),
                            Acceso = 1
                        });
                    }
                    else
                    {
                        lista.Add(new objeto()
                        {
                            Codigo = (string)viewPermisos.GetRowCellValue(i, "codObjeto"),
                            Acceso = 0
                        });
                    }
                }

                //RETORNAMOS LISTA
                return lista;
            }
            else
            {
                return null;
            }
        }

        #endregion

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            if (Cancelar)
            {
                //SI CANCELAR ES TRUE ES PORQUE HICE CLICK EN EL BOTON NUEVO
                //YA SE LIMPIARON LOS CAMPOS...
                CargarCampos(0);
                btnNuevo.Text = "Nuevo";
                Cancelar = false;
            }
            else
            {
                //SI ES FALSE ES PORQUE NO SE HAN CARGADO DATOS
                fnLimpiarCampos();

                btnNuevo.Text = "Cancelar";
                Cancelar = true;
            }            
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
            else if (char.IsSeparator(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void txtUsuario_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsLetter(e.KeyChar))
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
            else if (e.KeyChar == (char)46)
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
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            if (txtNombre.Text == "")
            {
                lblerror.Visible = true;
                lblerror.Text = "Por favor ingrese su nombre";
                return;
            }

            if (txtPassword.Text == "" && UpdateUsuario == false)
            {
                lblerror.Visible = true;
                lblerror.Text = "Por favor ingrese una contraseña";
                return;
            }

            if (txtConfirmaClave.Text == "" && UpdateUsuario == false)
            {
                lblerror.Visible = true;
                lblerror.Text = "Por favor ingrese una contraseña";
                return;
            }

            if (txtUsuario.Text == "")
            {
                lblerror.Visible = true;
                lblerror.Text = "Por favor ingrese un nombre de usuario";
                return;
            }

            if (txtCondicion.Text == "" && cbAplicaFiltro.Checked)
            { lblerror.Visible = true; lblerror.Text = "Por favor ingresa una condicion"; return; }

            if (txtGrupo.EditValue == null)
            { lblerror.Visible = true; lblerror.Text = "Por favor selecciona un grupo"; return; }

            //VALIDAR QUE EL NOMBRE DE USUARIO INGRESADO NO EXISTA EN BD
            bool existe = false;

            if (GrupoSeleccion == 0) { lblerror.Visible = true; lblerror.Text = "Grupo seleccionado no valido";return;}

            lblerror.Visible = false;

            if (UpdateUsuario)
            {
                //ACTUALIZAR
                string UserBd = "";
                if (viewUsuarios.RowCount == 0) { lblerror.Visible = true;lblerror.Text = "Registro no valido"; return;}

                if (Conjunto.ExisteConjunto(txtCondicion.Text) == false && cbAplicaFiltro.Checked)
                { XtraMessageBox.Show("Filtro seleccionado no existe", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);return; }

                if (cbReabreMes.Checked)
                {
                    if (txtClaveMaestra.Text == "")
                    { lblerror.Visible = true; lblerror.Text = "Por favor ingresa clave maestra"; txtClaveMaestra.Focus(); return; }

                    if (txtClaveMaestraConfirma.Text == "")
                    { lblerror.Visible = true; lblerror.Text = "Por favor confirma clave maestra"; txtClaveMaestra.Focus(); return; }

                    if (User.ComparaContraseñas(txtClaveMaestra.Text, txtClaveMaestraConfirma.Text) == false)
                    { lblerror.Visible = true; lblerror.Text = "Las claves maestras no coinciden"; txtClaveMaestra.Focus(); return; }
                }

                //PREGUNTAMOS SI EXISTE USUARIO
                UserBd = (string)viewUsuarios.GetFocusedDataRow()["usuario"];
                if (fnUsuarioExiste(UserBd))
                {
                    //MODIFICAMOS...
                    fnModificarUsuario(txtNombre, txtUsuario, txtGrupo, UserBd, txtCondicion, txtClaveMaestra, cbPrivada, cbBloqueaUsuarios);
                }
                else { lblerror.Visible = true; lblerror.Text = "Usuario no valido"; return; }
            }
            else
            {
                //NUEVO REGISTRO
                existe = fnUsuarioExiste(txtUsuario.Text);
                if (existe)
                {
                    lblerror.Visible = true;
                    lblerror.Text = "Nombre de usuario ya registrado";
                    return;
                }
                else
                {
                    //COMPARA PASSWORD
                    if (txtPassword.Text != txtConfirmaClave.Text)
                    { lblerror.Visible = true; lblerror.Text = "Las contraseñas no coinciden"; return; }

                    if (Conjunto.ExisteConjunto(txtCondicion.Text) == false && cbAplicaFiltro.Checked)
                    { XtraMessageBox.Show("Filtro seleccionado no existe", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                    //LISTADO OBJETOS TABLA OBJETO
                    List<objeto> lista = new List<objeto>();
                    lista = objeto.GetListObjetos();

                    if (lista.Count == 0)
                    { XtraMessageBox.Show("Error al obtener informacion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                    //ES VALIDO, PODEMOS REALIZAR INSERT
                    // fnNuevoUsuario(txtNombre, txtUsuario, txtPassword, txtGrupo, txtCondicion);

                    if (cbReabreMes.Checked)
                    {
                        if (txtClaveMaestra.Text == "")
                        { lblerror.Visible = true; lblerror.Text = "Por favor ingresa clave maestra"; txtClaveMaestra.Focus(); return; }

                        if (txtClaveMaestraConfirma.Text == "")
                        { lblerror.Visible = true; lblerror.Text = "Por favor confirma clave maestra"; txtClaveMaestra.Focus(); return; }

                        if (User.ComparaContraseñas(txtClaveMaestra.Text, txtClaveMaestraConfirma.Text) == false)
                        { lblerror.Visible = true; lblerror.Text = "Las claves maestras no coinciden"; txtClaveMaestra.Focus(); return; }
                    }

                    if (NuevoUsuarioTransaccion(txtNombre, txtUsuario, txtPassword, txtGrupo, txtCondicion, txtClaveMaestra, cbPrivada, cbBloqueaUsuarios))
                    {
                        XtraMessageBox.Show("Ingreso realizado correctamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        //GUARDAR EVENTO EN LOG
                        logRegistro log = new logRegistro(User.getUser(), "SE HA CREADO UN NUEVO USUARIO CON NOMBRE " + txtUsuario.Text, "USUARIO", "0", txtUsuario.Text, "INGRESAR");
                        log.Log();

                        fnSistema.spllenaGridView(gridUsuarios, string.Format("SELECT nombre, usuario, descripcion, id, filtro, llavemaestra, conf, bloqueo FROM usuario INNER JOIN grupo ON " +
                        "usuario.grupo = grupo.id WHERE grupo.id ={0}", Convert.ToInt32(txtGrupo.EditValue)));
                        fnSistema.spOpcionesGrilla(viewUsuarios);
                        fnOpcionesGrilla();
                        btnNuevo.Text = "Nuevo";
                        Cancelar = false;                       

                        // fnLimpiarCampos();       
                        CambiarFocoFila(Convert.ToInt32(txtGrupo.EditValue));
                        CargarCampos(0);
                    }
                    else
                    {
                        XtraMessageBox.Show("Error al ingresar usuario", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }       
        }

        private void txtNombre_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (txtNombre.Text == "")
                {
                    lblerror.Visible = true;
                    lblerror.Text = "Por favor ingrese su nombre";
                    return;
                }
                else
                {
                    lblerror.Visible = false;
                }
            }
        }

        private void txtUsuario_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (txtUsuario.Text == "")
                {
                    lblerror.Visible = true;
                    lblerror.Text = "Por favor ingrese un nombre de usuario";
                    return;
                }
                else
                {
                    lblerror.Visible = false;
                    if (UpdateUsuario == false)
                    {
                        //VALIDAR QUE EL NOMBRE DE USUARIO NO EXISTE PREVIAMENTE EN BD
                        bool existe = false;
                        existe = fnUsuarioExiste(txtUsuario.Text);
                        if (existe)
                        {
                            lblerror.Visible = true;
                            lblerror.Text = "Usuario ingresado ya existe";
                            return;
                        }
                        else
                        {
                            lblerror.Visible = false;
                        }
                    }
                    else
                    {
                        string userBD = "";
                        if (viewUsuarios.RowCount>0)
                        {
                            userBD = (string)viewUsuarios.GetFocusedDataRow()["usuario"];

                            if (txtUsuario.Text == "") { lblerror.Visible = true; lblerror.Text = "Por favor ingresa un usuario";return; }

                            if (txtUsuario.Text.ToLower() != userBD.ToLower())
                            {
                                if (fnUsuarioExiste(txtUsuario.Text))
                                {
                                    lblerror.Visible = true;
                                    lblerror.Text = "Usuario ingresado ya existe";
                                    return;
                                }
                            }

                            lblerror.Visible = false;
                        }
                    }
                   
                }
            }
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (txtPassword.Text == "")
                {
                    lblerror.Visible = true;
                    lblerror.Text = "Por favor ingrese una contraseña";
                }
                else
                {
                    lblerror.Visible = false;
                }
            }
        }

        private void btnUsuarios_Click(object sender, EventArgs e)
        {
            frmAdmUsuario administrar = new frmAdmUsuario();
            administrar.ShowDialog();
        }

        private void txtNombre_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtUsuario_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtPassword_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void gridUsuarios_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            CargarCampos();
        }

        private void gridUsuarios_KeyUp(object sender, KeyEventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION

            CargarCampos();
        }

        private void viewUsuarios_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            DXPopupMenu menu = e.Menu;

            if (menu != null)
            {
                DXMenuItem sub = new DXMenuItem("Cambiar contraseña", new EventHandler(Cambiarpassword_click));
                sub.ImageOptions.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("images/actions/editname_16x16.png");
                DXMenuItem CambiaMaestra = new DXMenuItem("Cambiar contraseña maestra", new EventHandler(CambiarPasswordMaestra_click));
                CambiaMaestra.ImageOptions.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("images/actions/editname_16x16.png");
                DXMenuItem eliminar = new DXMenuItem("Eliminar usuario", new EventHandler(EliminarUsuario_click));
                eliminar.ImageOptions.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("images/edit/delete_16x16.png");
                DXMenuItem CerrarSesion = new DXMenuItem("Cerrar Sesion", new EventHandler(CerrarSesion_Click));
                CerrarSesion.ImageOptions.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("images/actions/reset_16x16.png");
                
                menu.Items.Clear();
                menu.Items.Add(sub);
                menu.Items.Add(CambiaMaestra);
                menu.Items.Add(eliminar);
                menu.Items.Add(CerrarSesion);                
            }
        }

        private void CerrarSesion_Click(object sender, EventArgs e)
        {
            string user = "";
            if (viewUsuarios.RowCount > 0)
            {
                user = (string)viewUsuarios.GetFocusedDataRow()["usuario"];

                if (user.ToLower() == User.getUser().ToLower())
                { XtraMessageBox.Show("No puedes cerrar tu propia sesion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                if (Sesion.Abierta(user.ToLower(), fnSistema.pgDatabase) == false)
                { XtraMessageBox.Show("Este usuario no tiene sesiones abiertas", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
                else
                {
                    DialogResult Advertencia = XtraMessageBox.Show("Si cierra la sesion de este usuario, toda la \n informacion no guardada se perderá, \n ¿Deseas continuar de todas maneras?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (Advertencia == DialogResult.Yes)
                    {
                        //CERRAMOS SESION USUARIO
                        if (Sesion.DeleteAccess(user.ToLower()))
                        { XtraMessageBox.Show("Sesion cerrada correctamente", "Sesion", MessageBoxButtons.OK, MessageBoxIcon.Information); }
                        else
                        { XtraMessageBox.Show("Error al intentar cerrar la sesion", "Sesion", MessageBoxButtons.OK, MessageBoxIcon.Stop); }
                    }
                }
            }
        }

        private void CambiarPasswordMaestra_click(object sender, EventArgs e)
        {
            //MOSTRAMOS FORM CAMBIO DE CONTRASEÑA
            if (viewUsuarios.RowCount > 0)
            {
                string MasterKey = "";                           

                //OBTENEMOS EL NOMBRE DEL USUARIO
                string user = (string)viewUsuarios.GetFocusedDataRow()["usuario"];

                MasterKey = User.GetLlaveMaestra(user);

                if (MasterKey == "0")
                { XtraMessageBox.Show("Este usuario no tiene permisos para reabrir mes", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                frmClave fr = new frmClave(true, user, "maestra");
                fr.StartPosition = FormStartPosition.CenterParent;
                fr.ShowDialog();
            }
        }

        private void EliminarUsuario_click(object sender, EventArgs e)
        {
            if (viewUsuarios.RowCount>0)
            {
                string userName = "";
                userName = (string)viewUsuarios.GetFocusedDataRow()["usuario"];

                if (Sesion.Abierta(userName, fnSistema.pgDatabase))
                { XtraMessageBox.Show("Usuario con sesion abierta", "Denegado", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
               
                //ELIMINAR
                if (EliminarTransaccion(userName))
                {
                    XtraMessageBox.Show("Usuario eliminado correctamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //RECARGAR GRILLA
                    fnSistema.spllenaGridView(gridUsuarios, string.Format("SELECT nombre, usuario, descripcion, id, filtro, llavemaestra, conf FROM usuario INNER JOIN grupo ON " +
                    "usuario.grupo = grupo.id WHERE grupo.id={0}", GrupoSeleccion));
                    fnOpcionesGrilla();

                    CargarCampos(0);

                    logRegistro log = new logRegistro(User.getUser(), "SE ELIMINA USUARIO " + userName, "USUARIO", userName, "0", "ELIMINAR");
                    log.Log();
                }
                else
                {
                    XtraMessageBox.Show("Error al intentar eliminar usuario", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                
                //fnEliminarUsuario(userName);
            }
            else
            {
                XtraMessageBox.Show("No hay registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void Cambiarpassword_click(object sender, EventArgs e)
        {
            if (viewUsuarios.RowCount>0)
            {
                //OBTENEMOS EL NOMBRE DEL USUARIO
                string user = (string)viewUsuarios.GetFocusedDataRow()["usuario"];

                frmClave fr = new frmClave(true, user, "normal");
                fr.StartPosition = FormStartPosition.CenterParent;
                fr.ShowDialog();
            }
        }

        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsSeparator(e.KeyChar))
            {
                e.Handled = true;
            }
           
        }

        private void txtConfirmaClave_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsSeparator(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void cbAplicaFiltro_CheckedChanged(object sender, EventArgs e)
        {
            if (cbAplicaFiltro.Checked)
            {
                txtCondicion.Enabled = true;
                txtCondicion.Text = "";
            }
            else
            {
                txtCondicion.Enabled = false;
                txtCondicion.Text = "";
            }
        }

        private void txtCondicion_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            DXPopupMenu menu = e.Menu;

            if (menu != null)
            {
                menu.Items.Clear();

                DXSubMenuItem Campos = new DXSubMenuItem("Campos");
                DXSubMenuItem Logicos = new DXSubMenuItem("Logicos");

                //CAMPOS
                List<string> Listacampos = new List<string>();
                Listacampos = ListaCampos();

                List<string> Listalogica = new List<string>();
                Listalogica = ListaLogicos();

                foreach (var item in Listacampos)
                {
                    DXMenuItem elemento = new DXMenuItem(item, new EventHandler(item_click));
                    Campos.Items.Add(elemento);
                }

                foreach (var x in Listalogica)
                {
                    DXMenuItem log = new DXMenuItem(x, new EventHandler(x_click));
                    Logicos.Items.Add(log);
                }

                menu.Items.Add(Campos);
                menu.Items.Add(Logicos);
                
            }
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

        private void item_click(object sender, EventArgs e)
        {
            DXMenuItem item = sender as DXMenuItem;

            int position = txtCondicion.SelectionStart;

            txtCondicion.Text = txtCondicion.Text.Insert(position, " ");
            txtCondicion.Text = txtCondicion.Text.Insert(position + 1, item.Caption);
            txtCondicion.Select((position + item.Caption.Length) + 1, 0);
            txtCondicion.ScrollToCaret();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            string user = "";
            if (viewUsuarios.RowCount > 0)
            {
                user = (string)viewUsuarios.GetFocusedDataRow()["usuario"];

                if (CambiosSinGuardar(user))
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

        private void txtNombreGrupo_KeyPress(object sender, KeyPressEventArgs e)
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

        private void gridGrupo_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            //CARGAR USUARIO EN BASE A GRUPO SELECCIONADO
            if (viewGrupo.RowCount>0)
            {
                CargarGrupo();
            }
        }

        private void gridGrupo_KeyUp(object sender, KeyEventArgs e)
        {
            int idGrupo = 0;
            //CARGAR USUARIO EN BASE A GRUPO SELECCIONADO
            if (viewGrupo.RowCount > 0)
            {
                CargarGrupo();
            }
        }

        private void btnNuevoGrupo_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            if (CancelaGrupo)
            {
                //SE PRESIONO UNA VEZ...
                btnNuevoGrupo.Text = "Cancelar";
                CancelaGrupo = false;
                btnEliminar.Enabled = false;
                lblGrupo.Visible = false;
                UpdateGrupo = false;
                txtNombreGrupo.Text = "";
                txtNombreGrupo.Focus();
            }
            else
            {
                btnNuevoGrupo.Text = "Nuevo";
                CancelaGrupo = true;
                CargarGrupo();
            }
        }

        private void btnGuardarGrupo_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            string oldName = "";
            int id = 0;
            if (txtNombreGrupo.Text == "")
            { lblGrupo.Visible = true; lblGrupo.Text = "Por favor ingresa un grupo"; return; }

            if (UpdateGrupo == false)
            {
                //INSERT
                if (ExisteGrupo(txtNombreGrupo.Text))
                { lblGrupo.Visible = true; lblGrupo.Text = "Grupo ingresado ya existe"; return; }

                lblGrupo.Visible = false;

                NuevoGrupo(txtNombreGrupo);
            }
            else
            {
                //UPDATE
                if (viewGrupo.RowCount > 0)
                {
                    oldName = (string)viewGrupo.GetFocusedDataRow()["descripcion"];
                    id = Convert.ToInt32(viewGrupo.GetFocusedDataRow()["id"]);
                    if (oldName != txtNombreGrupo.Text)
                    {
                        if (ExisteGrupo(txtNombreGrupo.Text))
                        { lblGrupo.Visible = true; lblGrupo.Text = "Grupo ingresado ya existe"; return; }

                        lblGrupo.Visible = true;

                        DialogResult dialogo = XtraMessageBox.Show("Estás a punto de cambiar el nombre del grupo " + oldName + ", ¿Realmente deseas modificar?", "Informacion", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                        if (dialogo == DialogResult.Yes)
                        {
                            ModificarGrupo(txtNombreGrupo, oldName, id);
                        }
                    }
                    else
                    {
                        ModificarGrupo(txtNombreGrupo, oldName, id);
                    }
                        
                }                
            }            
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            int id = 0;
            string grupo = "";
            //LISTA PARA GUARDAR LISTADO DE USUARIOS ASOCIADOS A ESE GRUPO
            List<string> ListaUsuarios = new List<string>();
            if (viewGrupo.RowCount > 0)
            {                
                //ID
                id = Convert.ToInt32(viewGrupo.GetFocusedDataRow()["id"]);
                grupo = (string)viewGrupo.GetFocusedDataRow()["descripcion"];

                //PREGUNTAR SI ALGUN USUARIO DE ESTE GRUPO TIENE UNA SESION ABIERTA
                if (Sesion.GrupoAbierta(id))
                { XtraMessageBox.Show("No puedes eliminar este grupo porque hay usuarios con sesion abierta", "informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                if (GrupoTieneUsuarios(id))
                {
                    ListaUsuarios = ListUser(id);

                    if (ListaUsuarios.Count == 0)
                    { XtraMessageBox.Show("Error al consultar informacion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                    DialogResult adv = XtraMessageBox.Show("Este grupo tiene usuarios asociados, ¿Deseas eliminar de todas maneras?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (adv == DialogResult.Yes)
                    {
                        /*ELIMINAMOS PRIMERAMENTE USUARIO*/
                        if (EliminarGroupTransaccion(id, grupo, ListaUsuarios ,true))
                        {
                            XtraMessageBox.Show("Registro eliminado correctamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            logRegistro log = new logRegistro(User.getUser(), "SE ELIMINA GRUPO " + grupo, "GRUPO", grupo, "0", "ELIMINAR");
                            log.Log();

                            //CARGAR GRILLA GRUPO
                            fnSistema.spllenaGridView(gridGrupo, "SELECT id, descripcion FROM grupo");
                            fnSistema.spOpcionesGrilla(viewGrupo);
                            ColumnasGrupo();
                            lblGrupo.Visible = false;

                            CargarGrupo();

                            //CARGAR COMBO BOX                            
                            datoCombobox.spllenaComboBox("SELECT id, descripcion FROM grupo", txtGrupo, "id", "descripcion", true);

                        }
                        else
                            XtraMessageBox.Show("No se pudo eliminar registro", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    DialogResult dialogo = XtraMessageBox.Show("¿Realmente desea eliminar grupo " + grupo + "?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogo == DialogResult.Yes)
                    {
                        if (EliminarGroupTransaccion(id, grupo, null, true))
                        {
                            XtraMessageBox.Show("Registro eliminado correctamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            logRegistro log = new logRegistro(User.getUser(), "SE ELIMINA GRUPO " + grupo, "GRUPO", grupo, "0", "ELIMINAR");
                            log.Log();

                            //CARGAR GRILLA
                            fnSistema.spllenaGridView(gridGrupo, "SELECT id, descripcion FROM grupo");
                            fnSistema.spOpcionesGrilla(viewGrupo);
                            ColumnasGrupo();
                            lblGrupo.Visible = false;

                            //CARGAR COMBO BOX                            
                            datoCombobox.spllenaComboBox("SELECT id, descripcion FROM grupo", txtGrupo, "id", "descripcion", true);
                        }
                        else
                            XtraMessageBox.Show("No se pudo eliminar registro", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }                           
            }
            else
                XtraMessageBox.Show("Registro no valido", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void btnEliminarUsuario_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            if (viewUsuarios.RowCount>0)
            {
                string user = (string)viewUsuarios.GetFocusedDataRow()["usuario"];
                //fnEliminarUsuario(user);

                if (Sesion.Abierta(user, fnSistema.pgDatabase))
                { XtraMessageBox.Show("Usuario con sesion abierta", "Denegado", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                if (EliminarTransaccion(user))
                {              
                    XtraMessageBox.Show("Usuario eliminado correctamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //RECARGAR GRILLA
                    fnSistema.spllenaGridView(gridUsuarios, string.Format("SELECT nombre, usuario, descripcion, id, filtro, llavemaestra, conf, bloqueo FROM usuario INNER JOIN grupo ON " +
                    "usuario.grupo = grupo.id WHERE grupo.id={0}", GrupoSeleccion));
                    fnOpcionesGrilla();

                    CargarCampos(0);

                    logRegistro log = new logRegistro(User.getUser(), "SE ELIMINA USUARIO " + user, "USUARIO", user, "0", "ELIMINAR");
                    log.Log();
                }
                else
                {
                    XtraMessageBox.Show("Error al intentar eliminar", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                XtraMessageBox.Show("Registro no valido", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void panelUser_Paint(object sender, PaintEventArgs e)
        {

        }

        private void txtCondicion_DoubleClick(object sender, EventArgs e)
        {
            //AL HACER DOBLE CLICK LANZAMOS EL FORMULARIO DE EXPRESION 
            if (cbAplicaFiltro.Checked)
            {
                FrmFiltroBusqueda filtro = new FrmFiltroBusqueda(true);
                filtro.StartPosition = FormStartPosition.CenterParent;
                filtro.opener = this;
                filtro.ShowDialog();
            }
        }

        private void txtCondicion_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar) || char.IsLetter(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
                    
        }

        private void txtClaveMaestra_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsSeparator(e.KeyChar))
            {
                e.Handled = true;
            }
            
        }

        private void txtClaveMaestraConfirma_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsSeparator(e.KeyChar))
            {
                e.Handled = true;
            }
         
        }

        private void cbReabreMes_CheckedChanged(object sender, EventArgs e)
        {
            //SI ESTA SELECCIONADO HABILITAMOS CAJAS MAESTRA
            if (cbReabreMes.Checked)
            {
                txtClaveMaestra.Enabled = true;
                txtClaveMaestraConfirma.Enabled = true;
                txtClaveMaestra.Focus();
                txtClaveMaestraConfirma.Text = "";
                txtClaveMaestra.Text = "";
                lblerror.Visible = false;
                
            }
            else
            {
                txtClaveMaestra.Text = "";
                txtClaveMaestraConfirma.Text = "";
                txtClaveMaestraConfirma.Enabled = false;
                txtClaveMaestra.Enabled = false;
                lblerror.Visible = false;
            }
        }

        private void txtClaveMaestra_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (txtClaveMaestra.Text == "") { lblerror.Visible = true; lblerror.Text = "Por favor ingresa clave maestra";txtClaveMaestra.Focus(); return; }

                lblerror.Visible = false;
            }
        }

        private void txtClaveMaestraConfirma_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (txtClaveMaestraConfirma.Text == "")
                { lblerror.Visible = true; lblerror.Text = "Por favor confirma clave maestra";txtClaveMaestraConfirma.Focus(); return; }

                lblerror.Visible = false;

                if (User.ComparaContraseñas(txtClaveMaestra.Text, txtClaveMaestraConfirma.Text) == false)
                { lblerror.Visible = true; lblerror.Text = "Las clave maestras no coinciden"; txtClaveMaestra.Focus(); return; }

                lblerror.Visible = false;
            }
        }

        private void txtCondicion_Properties_BeforeShowMenu_1(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void btnGuardarPermisos_Click(object sender, EventArgs e)
        {
            if (viewGrupo.RowCount == 0)
            { XtraMessageBox.Show("No haz seleccionado ningún grupo", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            int IdGrupo = 0;
            IdGrupo = Convert.ToInt32(viewGrupo.GetFocusedDataRow()["id"]);
            
            List<objeto> ventanas = new List<objeto>();
            ventanas = FilasSeleccionadas();

            if (ventanas.Count == 0)
            { XtraMessageBox.Show("No se encontró información", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (ModificaAutorizacion(IdGrupo, ventanas))
            {
                XtraMessageBox.Show("Permisos guardados correctamente", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);

                ventanas = CargarDatos(IdGrupo);
                SetSelection(ventanas);
            }
            else
            {
                XtraMessageBox.Show("No se pudo guardar los cambios", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnConjunto_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            //AL HACER DOBLE CLICK LANZAMOS EL FORMULARIO DE EXPRESION 
            if (cbAplicaFiltro.Checked)
            {
                FrmFiltroBusqueda filtro = new FrmFiltroBusqueda(true);
                filtro.StartPosition = FormStartPosition.CenterParent;
                filtro.opener = this;
                filtro.ShowDialog();
            }
        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }
    }
}