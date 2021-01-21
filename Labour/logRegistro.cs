using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using DevExpress.XtraEditors;

namespace Labour
{
    class logRegistro
    {
        //VARIABLES
        private string User = "";
        private string descripcion = "";
        private string tabla = "";
        private string antiguo = ""; //VALOR ANTIGUO CAMPO
        private string nuevo = ""; //VALOR NUEVO CAMPO
        private string tipo = ""; //INGRESA - MODIFICA - ELIMINA

        //CONSTRUCTOR PARAMETRIZADO
        public logRegistro(string user, string descripcion, string tabla, string antiguo, string nuevo, string tipo)
        {
            User = user;
            this.descripcion = descripcion;
            this.tabla = tabla;
            this.antiguo = antiguo;
            this.nuevo = nuevo;
            this.tipo = tipo;
        }

        //CONSTRUCTOR SIN PARAMETROS
        public logRegistro()
        {

        }

        //GET Y SET
        #region "GET - SET"
        public void setUser(string user)
        {
            User = user;
        }

        public void setDescripcion(string descripcion)
        {
            this.descripcion = descripcion;
        }      

        public void setTabla(string tabla)
        {
            this.tabla = tabla;
        }

        public string getUser()
        {
            return User;
        }

        public string getDescripcion()
        {
            return descripcion;
        }


        public string getTabla()
        {
            return tabla;
        }

        public string getAntiguo()
        {
            return this.antiguo;
        }

        public void setAntiguo(string Antiguo)
        {
            this.antiguo = Antiguo;
        }

        public void setNuevo(string nuevo)
        {
            this.nuevo = nuevo;
        }
        public string getNuevo()
        {
            return this.nuevo;
        }

        public void setTipo(string Tipo)
        {
            this.tipo = Tipo;
        }
        public string getTipo()
        {
            return this.tipo;
        }
        #endregion

        //CARGAR LOG
        public void Log()
        {          
           fnNuevoLog(User, descripcion, tabla, antiguo, nuevo, tipo);           
        }

        public void LogEvento(SqlConnection cn)
        {
            NuevoEvento(User, descripcion, tabla, antiguo, nuevo, tipo, cn);
        }
        //PARA GUARDAR LOG DE REGISTRO DE CAMBIOS REALIZADOS EN SISTEMA
        //INGRESAR NUEVO REGISTRO
        protected bool fnNuevoLog(string user, string descripcion, string tabla, string antiguo, string nuevo, string tipo)        
        {
            string sql = "INSERT INTO logCambios(usuario, descripcion, tabla, valorAntiguo, valorActual, tipo) " +
                        "VALUES(@pusuario, @pdesc, @tabla, @valorAntiguo, @valorActual, @tipo)";
            SqlCommand cmd;
            SqlConnection cn;
            int res = 0;
            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            cmd.Parameters.Add(new SqlParameter("@pusuario", user));
                            cmd.Parameters.Add(new SqlParameter("@pdesc", descripcion));
                            cmd.Parameters.Add(new SqlParameter("@tabla", tabla));
                            cmd.Parameters.Add(new SqlParameter("@valorAntiguo", antiguo));
                            cmd.Parameters.Add(new SqlParameter("@valorActual", nuevo));
                            cmd.Parameters.Add(new SqlParameter("@tipo", tipo));

                            //EJECUTAR CONSULTA
                            res = cmd.ExecuteNonQuery();
                            if (res > 0)
                            {
                                //XtraMessageBox.Show("LOG CORRECTO!");
                                //INGRESO CORRECTO
                            }
                            else
                            {
                                //   XtraMessageBox.Show("LOG INCORRECTO!");

                            }
                        }
                        cmd.Dispose();
                    }
                }             
            }
            catch (SqlException ex)
            {
             //SQL EXCEPTION...
            }

            return false;
        }

        //METODO PARA INGRESO SIN CERRAR LA CONEXION
        protected bool NuevoEvento(string user, string descripcion, string tabla, string antiguo, string nuevo, string tipo, SqlConnection cn)
        {
            string sql = "INSERT INTO logCambios(usuario, descripcion, tabla, valorAntiguo, valorActual, tipo) " +
                        "VALUES(@pusuario, @pdesc, @tabla, @valorAntiguo, @valorActual, @tipo)";
            SqlCommand cmd;
            int res = 0;
            try
            {               
                    using (cmd = new SqlCommand(sql, cn))
                    {
                        cmd.Parameters.Add(new SqlParameter("@pusuario", user));
                        cmd.Parameters.Add(new SqlParameter("@pdesc", descripcion));
                        cmd.Parameters.Add(new SqlParameter("@tabla", tabla));
                        cmd.Parameters.Add(new SqlParameter("@valorAntiguo", antiguo));
                        cmd.Parameters.Add(new SqlParameter("@valorActual", nuevo));
                        cmd.Parameters.Add(new SqlParameter("@tipo", tipo));

                        //EJECUTAR CONSULTA
                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            //XtraMessageBox.Show("LOG CORRECTO!");
                            //INGRESO CORRECTO
                        }
                        else
                        {
                            //   XtraMessageBox.Show("LOG INCORRECTO!");

                        }
                    }
                    cmd.Dispose();
                    //fnSistema.sqlConn.Close();
               
             
            }
            catch (SqlException ex)
            {
                //SQL EXCEPTION...
            }

            return false;
        }
        //DESTRUCTOR
       /* ~logRegistro()
        {
            this.User = "";
            this.tabla = "";
            this.descripcion = "";
            this.bd = "";
        }*/
    }

    class Registro
    {
        //SABER SI UN CAMPO SE MODIFICO
        public static bool CambioRegistro(string ConsultaSql, string Campo, string valorCompara, string tipo)
        {
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(ConsultaSql, fnSistema.sqlConn))
                    {
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                if ((string)rd[Campo].ToString() == valorCompara)
                                {
                                    //NO SE MODIFICA
                                    return true;
                                }
                                else
                                {
                                    //SE MODIFICA EL VALOR
                                    return false;
                                }
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
            return false;
        }
    }
}
