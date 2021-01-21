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
using System.Collections;
using System.Data.SQLite;

namespace Labour
{
    public partial class frmEditaConfiguracion : DevExpress.XtraEditors.XtraForm
    {
        //PARA GUARDAR EL NOMBRE DE LA CONEXION
        private int IdConexion = 0;

        public IUpdateConfig open { get; set; }

        public frmEditaConfiguracion()
        {
            InitializeComponent();
        }

        public frmEditaConfiguracion(int IdCn)
        {
            InitializeComponent();
            this.IdConexion = IdCn;
        }
        private void frmEditaConfiguracion_Load(object sender, EventArgs e)
        {
           
            if (IdConexion != 0)
            {
                //CARRGAR DATOS
                DefaultProperties();
                Hashtable data = new Hashtable();
                data = DatosConexion(IdConexion);
                
                CargarDatosCampos(data);
            }
        }


        #region "MANEJO DE DATOS"
        //TRAER TODOS LOS DATOS DE CONEXION DE ACUERDO NOMBRE DE CONEXION
        private Hashtable DatosConexion(int IdConexion)
        {
            Hashtable data = new Hashtable();
            string sql = "SELECT id, nombre, servidor, database, usuario, password FROM datos " +
                        "WHERE id=@IdConexion";
            SQLiteCommand cmd;
            SQLiteDataReader rd;
            try
            {
                if (SqlLite.NuevaConexion())
                {
                    using (cmd = new SQLiteCommand(sql, SqlLite.Sqlconn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SQLiteParameter("@IdConexion", IdConexion));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                
                                //GUARDAMOS DATOS EN TABLA HASH
                                
                                data.Add("nombre", (string)rd["nombre"]);                                
                                data.Add("servidor", (string)rd["servidor"]);                               
                                data.Add("database", (string)rd["database"]);                         
                                data.Add("usuario", (string)rd["usuario"]);                             
                                data.Add("password", (string)rd["password"]);                                
                                data.Add("id", (Int64)rd["id"]);                            
                            }
                        }                        
                        cmd.Dispose();
                        rd.Close();
                        SqlLite.Sqlconn.Close();
                    }
                }
                else
                {
                    XtraMessageBox.Show("No hay conexion con la base de datos");
                }
            }
            catch (SQLiteException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return data;
        }

        //CARGAR DATOS EN CAMPOS DE ACUERDO A SELECCION
        private void CargarDatosCampos(Hashtable datos)
        {
            if (datos.Count > 0)
            {
                //CARGAMOS CAJAS
                txtNombre.Text = (string)datos["nombre"];
                txtBd.Text = (string)datos["database"];
                txtUser.Text = (string)datos["usuario"];
                txtServer.Text = (string)datos["servidor"];
                txtPass.Text = (string)datos["password"];
                txtPassConf.Text = (string)datos["password"];
            }
        }

        private void DefaultProperties()
        {
            cbPassword.Checked = false;
            txtPass.Enabled = false;
            txtPassConf.Enabled = false;
            txtNombre.Focus();
        }

        //TEST CONEXION
        private bool TestConexion(string bd, string user, string password, string server)
        {
            splashScreenManager1.ShowWaitForm();
            bool exito = false;
            fnSistema.pgUser = user;
            fnSistema.pgDatabase = bd;
            fnSistema.pgPass = password;
            fnSistema.pgServer = server;

            if (fnSistema.ConectarSQLServer())
            {
                exito = true;
            }

            splashScreenManager1.CloseWaitForm();
            return exito;
        }

        //ACTUALIZAR DATOS
        private void ActualizarData(TextEdit pNombre, TextEdit pBd, TextEdit pUser, TextEdit pServer, string password, int Id)
        {
            string sql = "UPDATE datos SET nombre=@nombre, servidor=@server, database=@db, usuario=@user, " +
                "password=@password WHERE id=@pId";
            SQLiteCommand cmd;
            int res = 0;
            
            try
            {
                if (SqlLite.NuevaConexion())
                {
                    using (cmd = new SQLiteCommand(sql, SqlLite.Sqlconn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SQLiteParameter("@nombre", pNombre.Text));
                        cmd.Parameters.Add(new SQLiteParameter("@server", pServer.Text));
                        cmd.Parameters.Add(new SQLiteParameter("@db", pBd.Text));
                        cmd.Parameters.Add(new SQLiteParameter("@user", pUser.Text));
                        cmd.Parameters.Add(new SQLiteParameter("@password", password));
                        cmd.Parameters.Add(new SQLiteParameter("@pId", Id));

                        res = cmd.ExecuteNonQuery();

                        cmd.Dispose();
                        SqlLite.Sqlconn.Close();
                        if (res > 0)
                        {
                            XtraMessageBox.Show("Actualizacion realizada con exito", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            if (open != null)
                                open.ActualizarGrilla();

                            Close();
                        }
                        else
                        {
                            XtraMessageBox.Show("Error al intentar actualizar", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtNombre.Focus();
                        }
                    }                    
                }
            }
            catch (SQLiteException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        #endregion

        private void cbPassword_CheckedChanged(object sender, EventArgs e)
        {
            if (cbPassword.Checked)
            {
                txtPass.Enabled = true;
                txtPassConf.Enabled = true;
                txtPass.Focus();
            }
            else
            {
                txtPass.Enabled = false;
                txtPassConf.Enabled = false;
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            if (txtNombre.Text == "")
            { lblError.Visible = true; lblError.Text = "Por favor ingresa el nombre de la conexion";txtNombre.Focus(); return; }
            if (txtServer.Text == "")
            { lblError.Visible = true; lblError.Text = "Por favor ingresa el nombre del servidor";txtServer.Focus(); return;}
            if (txtBd.Text == "")
            { lblError.Visible = true; lblError.Text = "Por favor ingresa el nombre de la base de datos";txtBd.Focus(); return; }
            if (txtUser.Text == "")
            { lblError.Visible = true; lblError.Text = "Por favor ingresa usuario para la conexion";txtUser.Focus(); return; }

            Cifrado cif = new Cifrado();
            if (cbPassword.Checked)
            {
                if (txtPass.Text == "")
                { lblError.Visible = true; lblError.Text = "Por favor ingresa la contraseña";txtPass.Focus(); return; }
                if (txtPassConf.Text == "")
                { lblError.Visible = true; lblError.Text = "Por favor ingresa la contraseña de confirmacion";txtPassConf.Focus(); return;}

                lblError.Visible = false;
                string pass1 = "", pass2 = "";
                pass1 = txtPass.Text;
                pass2 = txtPassConf.Text;
              

                if (pass1 != pass2)
                { lblError.Visible = true;lblError.Text = "Las contraseñas no coinciden";txtPass.Focus(); return; }
                else
                {
                    //LAS CONTRASEÑAS COINCIDEN                    
                    lblError.Visible = false;
                    //VERIFICAMOS QUE HAYA CONEXION...
                    if (TestConexion(txtBd.Text, txtUser.Text, pass1, txtServer.Text))
                    {
                        //SI LA CONEXION ES EXITOSA PODEMOS ACTUALIZAR
                        ActualizarData(txtNombre, txtBd, txtUser, txtServer, cif.EncriptaTripleDesc(pass1), IdConexion);
                    }
                    else
                    {
                        XtraMessageBox.Show("No se pudo establecer conexion con el servidor", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtNombre.Focus();
                    }
                }
            }
            else
            {
                string pass1 = "";
                pass1 = txtPass.Text;

                //VERIFICAMOS QUE HAYA CONEXION...
                if (TestConexion(txtBd.Text, txtUser.Text, cif.DesencriptaTripleDesc(pass1), txtServer.Text))
                {
                    //SI LA CONEXION ES EXITOSA PODEMOS ACTUALIZAR
                    ActualizarData(txtNombre, txtBd, txtUser, txtServer, pass1, IdConexion);
                }
                else
                {
                    XtraMessageBox.Show("No se pudo establecer conexion con el servidor", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtNombre.Focus();
                }

            }
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            if (txtNombre.Text == "")
            { lblError.Visible = true; lblError.Text = "Por favor ingresa el nombre de la conexion"; txtNombre.Focus(); return; }
            if (txtServer.Text == "")
            { lblError.Visible = true; lblError.Text = "Por favor ingresa el nombre del servidor";txtServer.Focus(); return; }
            if (txtBd.Text == "")
            { lblError.Visible = true; lblError.Text = "Por favor ingresa el nombre de la base de datos";txtBd.Focus(); return; }
            if (txtUser.Text == "")
            { lblError.Visible = true; lblError.Text = "Por favor ingresa usuario para la conexion";txtUser.Focus(); return; }

            Cifrado cif = new Cifrado();

            if (cbPassword.Checked)
            {
                if (txtPass.Text == "")
                { lblError.Visible = true; lblError.Text = "Por favor ingresa la contraseña";txtPass.Focus(); return; }
                if (txtPassConf.Text == "")
                { lblError.Visible = true; lblError.Text = "Por favor ingresa la contraseña de confirmacion";txtPassConf.Focus(); return; }

                lblError.Visible = false;
                string pass1 = "", pass2 = "";
                pass1 = txtPass.Text;
                pass2 = txtPassConf.Text;

                if (pass1 != pass2)
                { lblError.Visible = true; lblError.Text = "Las contraseñas no coinciden";txtPass.Focus(); return; }
                else
                {
                    //LAS CONTRASEÑAS COINCIDEN                    
                    lblError.Visible = false;
                    //VERIFICAMOS QUE HAYA CONEXION...
                    if (TestConexion(txtBd.Text, txtUser.Text, pass1, txtServer.Text))
                    {
                        //SI LA CONEXION ES EXITOSA PODEMOS ACTUALIZAR
                        // ActualizarData(txtNombre, txtBd, txtUser, txtServer, pass1, IdConexion);
                        XtraMessageBox.Show("Conexion realizada con exito", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txtNombre.Focus();
                    }
                    else
                    {
                        XtraMessageBox.Show("No se pudo establecer conexion con el servidor", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtNombre.Focus();
                    }
                }
            }
            else
            {
                string pass1 = "";
                pass1 = txtPass.Text;

                //VERIFICAMOS QUE HAYA CONEXION...
                if (TestConexion(txtBd.Text, txtUser.Text, cif.DesencriptaTripleDesc(pass1), txtServer.Text))
                {
                    //SI LA CONEXION ES EXITOSA PODEMOS ACTUALIZAR
                    XtraMessageBox.Show("Conexion realizada con exito", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtNombre.Focus();
                }
                else
                {
                    XtraMessageBox.Show("No se pudo establecer conexion con el servidor", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtNombre.Focus();
                }
            }
        }

        private void txtNombre_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) || char.IsLetter(e.KeyChar) || char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void txtServer_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void txtBd_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void txtUser_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            //Sesion.NuevaActividad();

            Close();
        }
    }
}