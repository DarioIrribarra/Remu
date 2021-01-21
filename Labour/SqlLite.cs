/*----------------------------------*/
/* @Date: 29/08/2017                */
/* @desarrollador: Daniel Sepulveda */
/* @Empresa: Soporte y Tecnologia   */
/*----------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors;
using System.Data;
/*para usar sqlite*/
using System.Data.SQLite;
using System.Windows.Forms;
using System.Drawing;

namespace Labour
{
    //clase para gestionar las conexion con el archivo que guarda los juegos de datos 
    //tipo de conexion SqLite
    //file: 'configdb.sqlite'
    class  SqlLite 
    {
        /*NOMBRE ARCHIVO DE CONEXION*/
        public static string NombreArchivoBd = "";

        public static SQLiteConnection Sqlconn { get; set; }
        public static string Cadena { get; set; }
        public static SQLiteCommand cmd;
        public static SQLiteDataAdapter data = null;
        public static SQLiteDataReader read = null;

        /*-------------------------------------------*/
        /*METODO PARA ABRIR CONEXION CON EL ARCHIVO */
        /*------------------------------------------*/
        /*devuelve true en caso de ser conexion exitosa y false en caso de algun error*/
        public static bool NuevaConexion()
        {            
            try
            {
                Cadena = "Data Source=configdb.sqlite;Version=3;New=False;Compress=True;";
                
                Sqlconn = new SQLiteConnection(Cadena);
                //PARA COLOCAR UNA CONTRASEÑA A LA BASE DE DATOS
                
                //PARA CAMBIAR O BORRAR LA CONTRASEÑ
                //Sqlconn.ChangePassword("MyNewPassword");
                //Sqlconn.ChangePassword(String.Empty);
                //abrimos la conexion
                Sqlconn.Open();
                return true;
            }
            catch (SQLiteException ex)
            {
                //error en la conexion o excepcion
                return false;
            }            
        }

        /// <summary>
        /// Crea una conexion con sql lite y devuelve el objeto sql
        /// </summary>
        /// <returns></returns>
        public static SQLiteConnection OpenConexion()
        {
            System.Data.SQLite.SQLiteConnection cn;

            string AbsolutePath = System.Environment.CurrentDirectory + "\\configdb.sqlite";

            try
            {
                Cadena = $"Data Source={AbsolutePath};Version=3;New=False";

                cn = new SQLiteConnection(Cadena);
                //PARA COLOCAR UNA CONTRASEÑA A LA BASE DE DATOS

                //PARA CAMBIAR O BORRAR LA CONTRASEÑ
                //Sqlconn.ChangePassword("MyNewPassword");
                //Sqlconn.ChangePassword(String.Empty);
                //abrimos la conexion
                cn.Open();
            }
            catch (SQLiteException ex)
            {
                //error en la conexion o excepcion
                //XtraMessageBox.Show(ex.Message);
                cn = null;
            }

            return cn;
        }

        /*------------------------------- */
        /*METODO PARA CARGAR GRIDOCONTROL */
        /*--------------------------------*/
        public static void CargarGrilla(DevExpress.XtraGrid.GridControl grid, string sql, DevExpress.XtraGrid.Views.Grid.GridView view)
       {
            RepositoryItemTextEdit repos = new RepositoryItemTextEdit();
            repos.MaxLength = 20;

            SQLiteConnection cn = new SQLiteConnection();
            SQLiteCommand cmd;
            
            try
            {
                cn = OpenConexion();
                if (cn != null)
                {
                    using (cn)
                    {
                        SQLiteDataAdapter db = new SQLiteDataAdapter(sql, cn);

                        DataSet ds = new DataSet();
                        ds.Reset();

                        DataTable dt = new DataTable();
                        db.Fill(ds);

                        dt = ds.Tables[0];

                        //le pasamos los datos al grid
                        grid.DataSource = dt;

                        //editar gridview
                        view.OptionsSelection.EnableAppearanceHideSelection = false;
                        view.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFullFocus;
                        view.OptionsBehavior.Editable = false;
                        view.OptionsSelection.EnableAppearanceFocusedCell = false;
                        view.OptionsSelection.MultiSelect = false;
                        view.Appearance.FocusedRow.BackColor = Color.DodgerBlue;
                        view.Appearance.FocusedRow.ForeColor = Color.White;
                        view.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        //al activar esta opcion se activa un formulario de edicion al doble click en row seleccionada 
                        //view.OptionsBehavior.EditingMode = DevExpress.XtraGrid.Views.Grid.GridEditingMode.Default;

                        //editar las columnas
                        //columna id
                        view.Columns[0].Caption = "IdDato";
                        view.Columns[0].Visible = false;

                        view.Columns[1].Caption = "Nombre Conexion";
                        view.Columns[1].ColumnEdit = repos;
                        view.Columns[1].AppearanceCell.FontStyleDelta = System.Drawing.FontStyle.Bold;
                        //columna servidor (ip o nombre)
                        view.Columns[2].Caption = "Servidor";
                        view.Columns[2].ColumnEdit = repos;
                        //view.Columns[1].OptionsColumn.AllowEdit = false;
                        view.Columns[3].Caption = "Base de Datos";
                        view.Columns[3].ColumnEdit = repos;

                        view.Columns[4].Caption = "Usuario";
                        view.Columns[4].ColumnEdit = repos;

                        view.Columns[5].Visible = false;                   

                    }
                }
            }
            catch (SQLiteException ex)
            {
               //ERROR...
            }        
        }

        /*METODO PARA INGRESAR UN NUEVO REGISTRO EN LA TABLA 'DATOS' */
        /*
         * Parametros de entrada: servidor (ip o nombre), database, usuario, password, nombre
         */
        public static string NuevoRegistro(DevExpress.XtraEditors.TextEdit id, DevExpress.XtraEditors.TextEdit server, DevExpress.XtraEditors.TextEdit bd,
            DevExpress.XtraEditors.TextEdit user, DevExpress.XtraEditors.TextEdit pass, DevExpress.XtraEditors.TextEdit name)
        {
            string insert = "INSERT INTO datos(servidor, database, usuario, password, nombre)" +
                " values(@pserv, @pdb, @puser, @pass, @pname)";

            string update = "UPDATE datos set id=@pid, servidor=@pserv, database=@pdb, usuario=@puser, password=@pass, nombre=@pname where id=@pid";

            string hash = "";            

            //para usar funcion hash
            Encriptacion enc = new Encriptacion();
            
            string message = "";
            int res = 0;

            SQLiteConnection cn;
            SQLiteCommand cmd;

            //si id es null es porque esta en blanco, debemos realizar insert
            if (id.Text == "-1")
            {
                //generamos hash de la contraseña ingresada
                hash = enc.EncodePassword(pass.Text);
                try
                {
                    cn = OpenConexion();
                    if (cn != null)
                    {
                        using (cn)
                        {
                            //si es true es porque hay conexion con la bd
                            using (Sqlconn)
                            {
                                using (cmd = new SQLiteCommand(insert, Sqlconn))
                                {                                   
                                    //parametros
                                    cmd.Parameters.Add(new SQLiteParameter("@pserv", server.Text));
                                    cmd.Parameters.Add(new SQLiteParameter("@pdb", bd.Text));
                                    cmd.Parameters.Add(new SQLiteParameter("@puser", user.Text));
                                    //generar hash para guardar el hash de la contraseña
                                    cmd.Parameters.Add(new SQLiteParameter("@pass", hash));
                                    cmd.Parameters.Add(new SQLiteParameter("@pname", name.Text));
                                    //ejecutamos la consulta
                                    res = cmd.ExecuteNonQuery();
                                    if (res > 0)
                                    {
                                        //insert correcto
                                        message = "Registro Guardado Correctamente";
                                    }
                                    else
                                    {
                                        //error al intentar insertar el nuevo registro
                                        message = "No se pudo Guardar!";
                                    }
                                }

                            }
                        }
                    }
                    else
                    {
                        //error con la conexion
                        message = "No hay conexion a la base de datos!!";
                    }
                }             
                catch (SQLiteException e)
                {
                    message = e.Message;
                }               
            }
            else
            {
                
                //verificamos si el hash es igual al hash que esta en bd
                string hashBd = buscarHash(id.Text);
                //comparamos hashBd con Hash de textbox
                if (hashBd.Equals(pass.Text))
                {
                    //si son iguales, guardamos el mismo hash
                    hash = hashBd;
                }
                else {
                    //generamos nuevo hash para la nueva cadena
                    hash = enc.EncodePassword(pass.Text);
                }

                cn = OpenConexion();

                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SQLiteCommand(update, Sqlconn))
                        {
                            try
                            {

                                //parametros
                                cmd.Parameters.Add(new SQLiteParameter("@pserv", server.Text));
                                cmd.Parameters.Add(new SQLiteParameter("@pdb", bd.Text));
                                cmd.Parameters.Add(new SQLiteParameter("@puser", user.Text));
                                cmd.Parameters.Add(new SQLiteParameter("@pass", hash));
                                cmd.Parameters.Add(new SQLiteParameter("@pname", name.Text));
                                cmd.Parameters.Add(new SQLiteParameter("@pid", id.Text));
                                //ejecutamos la consulta
                                res = cmd.ExecuteNonQuery();
                                if (res > 0)
                                {
                                    //insert correcto
                                    message = "Registro Actualizado Correctamente!";
                                }
                                else
                                {
                                    //error al intentar insertar el nuevo registro
                                    message = "Error al actualizar registro!";
                                }
                            }
                            catch (SQLiteException e)
                            {
                                message = e.Message;
                            }
                        }

                    }
                }         
                else
                {
                    //no hay conexion a la base de datos!
                    message = "No hay conexion con la base de datos!";
                }

            }
            return message;
        }//end metodo

        /*compara hash que esta en bd con el hash que esta en la caja de texto, si son iguales es porque no se 
         ha cambiado la cadena...
         Caso contrario debemos generar un hash nuevo para la nueva cadena
         */

        public static string buscarHash(string id)
        {
            string query = "select password from datos where id=@pid";
            string pass = "";
            if (NuevaConexion())
            {
                using (Sqlconn)
                {
                    using (cmd)
                    {
                        cmd = new SQLiteCommand(query, Sqlconn);
                        cmd.Parameters.Add(new SQLiteParameter("@pid", id));
                        read = cmd.ExecuteReader();
                        //preguntamos si retorno resultados
                        if (read.HasRows)
                        {
                            //si hay registros los recorremos
                            while (read.Read())
                            {
                                //guardamos el hash que retorna la consulta
                                pass = (string)read["password"];
                                
                            }
                        }
                    }

                }
            }
            return pass;
        }


        /*METODO PARA REALIZAR UPDATE TABLA DATOS*
         * PARAMETROS ENTRADA
         * @-> NEW VALUE
         */
        public static void UpdateRegistro(string value, string campo, int id)
        {
            // SQL UPDATE
            //string update = "UPDATE datos set @pCampo='@pValor' where id=@pId";
           string update = string.Format("UPDATE datos set {0} = '{1}' WHERE id={2}", campo, value, id);          
          
            int res = 0;
            if (NuevaConexion())
            {
                using (Sqlconn)
                {
                    try
                    {
                        cmd = new SQLiteCommand(update, Sqlconn);                       
                        
                        //EJECUTAMOS LA CONSULTA
                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            XtraMessageBox.Show("Registro Actualizado", "informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            XtraMessageBox.Show("Error al Actualizar", "informacion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        cmd.Dispose();
                        //CERRAR CONEXION
                        Sqlconn.Close();
                    }
                    catch (SQLiteException ex)
                    {
                        XtraMessageBox.Show(ex.Message);
                    }                   
                }               
            }
            else {
                XtraMessageBox.Show("No hay conexion a Base de datos!");
            }
        }

        /*METODO PARA CERRAR CONEXION*/
        public static void Cerrar()
        {
           // if (Sqlconn.State==ConnectionState.Open)
            //{
            //    Sqlconn.Close();
            //}
        }

        /*METODO PARA INGRESO @2*/
        public static void fnIngresoRegistros(DevExpress.XtraEditors.TextEdit server, DevExpress.XtraEditors.TextEdit bd,
            DevExpress.XtraEditors.TextEdit user, DevExpress.XtraEditors.TextEdit pass, DevExpress.XtraEditors.TextEdit name)
        {
            string insert = "INSERT INTO datos(servidor, database, usuario, password, nombre)" +
                " values(@pserv, @pdb, @puser, @pass, @pname)";

            string hash = "";
            int res = 0;
            //para usar funcion hash
            //Encriptacion enc = new Encriptacion();
            Cifrado cif = new Cifrado();
            //generamos hash de la contraseña ingresada
            //hash = enc.EncodePassword(pass.Text);
            try
            {
                if (NuevaConexion())
                {
                    using (Sqlconn)
                    {
                        cmd = new SQLiteCommand(insert, Sqlconn);
                        //parametros
                        cmd.Parameters.Add(new SQLiteParameter("@pserv", server.Text));
                        cmd.Parameters.Add(new SQLiteParameter("@pdb", bd.Text));
                        cmd.Parameters.Add(new SQLiteParameter("@puser", user.Text));
                        //generar hash para guardar el hash de la contraseña                        
                        cmd.Parameters.Add(new SQLiteParameter("@pass", cif.EncriptaTripleDesc(pass.Text)));
                        cmd.Parameters.Add(new SQLiteParameter("@pname", name.Text));
                        //ejecutamos la consulta
                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            //insert correcto
                            XtraMessageBox.Show("Registro Guardado", "informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            //error al intentar insertar el nuevo registro
                            XtraMessageBox.Show("No se pudo Guardar", "informacion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    XtraMessageBox.Show("No hay conexion con Base de Datos", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (SQLiteException ex)
            {

                XtraMessageBox.Show(ex.Message);
            }
           


        }

        /*BUSCAR DATOS DE ACUERDO A LO SELECCIONADO EN EL COMBO*/
        /*ESTOS DATOS SE USARAN PARA SABER A CUAL SERVIDOR HAY QUE APUNTAR LAS CONEXIONES*/
        public static string[] fnBuscarConfig(string nombre)
        {
            //retornar arreglo con datos
            string sql = "SELECT servidor, database, usuario, password FROM DATOS WHERE UPPER(nombre) =UPPER(@nombre)";            
            string[] datos = new string[4];
            if (NuevaConexion())
            {
                using (cmd = new SQLiteCommand(sql, Sqlconn))
                {
                    //parametros
                    cmd.Parameters.Add(new SQLiteParameter("@nombre", nombre));
                    read = cmd.ExecuteReader();
                    if (read.HasRows)
                    {
                        //guardamos los valores de la consulta
                        while (read.Read())
                        {
                            datos[0] = (string)read["servidor"];
                            datos[1] = (string)read["database"];
                            datos[2] = (string)read["usuario"];
                            datos[3] = (string)read["password"]; 
                        }
                    }
                    else {
                        //NO ENCONTRO REGISTROS!
                        datos[0] = "";
                        datos[1] = "";
                        datos[2] = "";
                        datos[3] = "";
                    }
                    //CERRAMOS LA CONEXION
                    Sqlconn.Clone();
                    cmd.Dispose();
                }
            }
            return datos;
        }

        public static string[] fnBuscarConfigDb(string nombre)
        {
            //retornar arreglo con datos
            string sql = "SELECT servidor, database, usuario, password FROM DATOS WHERE database=@nombre";
            string[] datos = new string[4];
            if (NuevaConexion())
            {
                using (cmd = new SQLiteCommand(sql, Sqlconn))
                {
                    //parametros
                    cmd.Parameters.Add(new SQLiteParameter("@nombre", nombre));
                    read = cmd.ExecuteReader();
                    if (read.HasRows)
                    {
                        //guardamos los valores de la consulta
                        while (read.Read())
                        {
                            datos[0] = (string)read["servidor"];
                            datos[1] = (string)read["database"];
                            datos[2] = (string)read["usuario"];
                            datos[3] = (string)read["password"];
                        }
                    }
                    else
                    {
                        //NO ENCONTRO REGISTROS!
                        datos[0] = "";
                        datos[1] = "";
                        datos[2] = "";
                        datos[3] = "";
                    }
                    //CERRAMOS LA CONEXION
                    Sqlconn.Clone();
                    cmd.Dispose();
                }
            }
            return datos;
        }

        /// <summary>
        /// Solo para buscar datos para licencia
        /// </summary>
        /// <param name="nombre">Nombre base de datos.</param>
        /// <returns></returns>
        public static string[] fnBuscarConfigLicencia(string nombre)
        {
            //retornar arreglo con datos
            string sql = "SELECT servidor, database, usuario, password FROM DATOS WHERE UPPER(database) =UPPER(@nombre)";
            string[] datos = new string[4];
            if (NuevaConexion())
            {
                using (cmd = new SQLiteCommand(sql, Sqlconn))
                {
                    //parametros
                    cmd.Parameters.Add(new SQLiteParameter("@nombre", nombre));
                    read = cmd.ExecuteReader();
                    if (read.HasRows)
                    {
                        //guardamos los valores de la consulta
                        while (read.Read())
                        {
                            datos[0] = (string)read["servidor"];
                            datos[1] = (string)read["database"];
                            datos[2] = (string)read["usuario"];
                            datos[3] = (string)read["password"];
                        }
                    }
                    else
                    {
                        //NO ENCONTRO REGISTROS!
                        datos[0] = "";
                        datos[1] = "";
                        datos[2] = "";
                        datos[3] = "";
                    }
                    //CERRAMOS LA CONEXION
                    Sqlconn.Clone();
                    cmd.Dispose();
                }
            }
            return datos;
        }

    }//end class
    
    class ComboLite
    {
        //private string idDatos;
        //private string NameDatos;
        public int KeyInfo { get; set; }
        public string descInfo { get; set; }

        /*---------------------------*/
        /*METODO PARA CARGAR COMBOBOX*/
        /*----------------------------*/

        public static string CargarCombo(DevExpress.XtraEditors.ComboBoxEdit combo, string sql)
        {
            combo.Properties.Items.Clear();
            SQLiteDataReader read;
            //creamos una lista de combobox
            List<ComboLite> lista = new List<ComboLite>();
            string msg = "";

            SQLiteConnection cn = new SQLiteConnection();
            SQLiteCommand cmd;
            SQLiteDataReader rd;
            
                try
                {
                    cn = SqlLite.OpenConexion();
                    if (cn != null)
                    {
                        using (cn)
                        {
                            using (cmd = new SQLiteCommand(sql, cn))
                            {
                                read = cmd.ExecuteReader();
                                if (read.HasRows)
                                {
                                    while (read.Read())
                                    {
                                        //LLENAMOS COMBOBOX
                                        combo.Properties.Items.Add(read.GetString(0));
                                    }
                                }
                                // SqlLite.Sqlconn.Close();
                                cmd.Dispose();
                                read.Close();
                                
                            }
                        //cn.Close();
                        cn.Dispose();
                        }
                    }                   
                                     
                }
                catch (SQLiteException e)
                {
                    msg = e.Message;
                XtraMessageBox.Show(msg);
                }
        
            //seleccionar el primer elemento del combo
            combo.SelectedIndex = 0;
            //en caso de excepcion retornamos mensaje
            return msg;
        }

        public static void fnCargarLook(DevExpress.XtraEditors.LookUpEdit pCombo, string sql, string pCampoKey, string pCampoInfo, bool? ocultarKey=false, bool? opcionTodos=false)
        {
            List<ComboLite> listacombo = new List<ComboLite>();
            SQLiteCommand cmd;
            SQLiteDataReader re;
            SQLiteConnection cn;

            try
            {
                cn = SqlLite.OpenConexion();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SQLiteCommand(sql, cn))
                        {
                            re = cmd.ExecuteReader();
                            if (re.HasRows)
                            {
                                while (re.Read())
                                {
                                    ComboLite combo = new ComboLite();
                                    combo.KeyInfo = Convert.ToInt32(re[pCampoKey]);
                                    combo.descInfo = (string)re[pCampoInfo];
                                    listacombo.Add(combo);
                                }
                            }
                            re.Close();
                        }
                    }
                }
            }
            catch (SQLiteException ex)
            {
              //ERROR...
            }

     
                pCombo.Properties.DataSource = listacombo.ToList();
                pCombo.Properties.ValueMember = "KeyInfo";
                pCombo.Properties.DisplayMember = "descInfo";
                pCombo.Properties.PopulateColumns();
                if (ocultarKey == true)
                {
                    pCombo.Properties.Columns[0].Visible = false;
                }
                pCombo.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
                pCombo.Properties.AutoSearchColumnIndex = 1;
                pCombo.Properties.ShowHeader = false;

            if (pCombo.Properties.DataSource != null)
                pCombo.ItemIndex = 0;
        }
    }


    interface ISqlServer
    {
        //FORMATO: tipo dato que retorna, nombre metodo, parametros
        //no se define visibilidad (public, private, etc)

        //Conexion Sql server
        bool NuevaConexion();
    }
}
