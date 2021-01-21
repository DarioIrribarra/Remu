using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Labour
{
    /// <summary>
    /// Clase para monitorear procesos activos.
    /// </summary>
    class Monitor
    {
        /// <summary>
        /// Indica si una tarea esta activa.
        /// </summary>
        /// <param name="pCod"></param>
        public static Tarea GetProcesoActivo(string pCod)
        {
            string sql = "SELECT activo, [desc], cod, usr FROM proceso WHERE cod=@pCod";
            SqlCommand cmd;
            SqlConnection cn;
            SqlDataReader rd;

            Tarea tr = new Tarea();

            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            cmd.Parameters.Add(new SqlParameter("@pCod", pCod));

                            rd = cmd.ExecuteReader();
                            if (rd.HasRows)
                            {
                                while (rd.Read())
                                {
                                    tr.Activo = Convert.ToInt16(rd["activo"]);
                                    tr.Cod = (string)rd["cod"];
                                    tr.DescTarea = (string)rd["desc"];
                                    tr.User = (string)rd["usr"];
                                }
                            }

                            rd.Close();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
              //ERROR...
            }

            return tr;
        }

        /// <summary>
        /// Indica si hay algun proceso activo para el usuario que inicia sesion.        
        /// </summary>
        public static bool ProcesosActivosUsuario(string pUser)
        {
            string sql = "SELECT count(*) FROM proceso WHERE activo=1 AND usr=@pUser";
            SqlCommand cmd;
            SqlConnection cn;
            bool Activos = false;

            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                                Activos = true;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                //ERROR...
            }

            return Activos;
        }

        /// <summary>
        /// Limpia todos los procesos que hayan quedado en 'espera' para el usuario actual
        /// <para>Retorna true, si el proceso se ejecutó correctamente.</para>
        /// </summary>
        public static bool CleanTask(string pUser)
        {
            string sql = "UPDATE proceso SET activo=0 WHERE usr=@pUser";
            SqlCommand cmd;
            SqlConnection cn;
            int count = 0;
            bool correcto = false;

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
                            cmd.Parameters.Add(new SqlParameter("@pUser", pUser));

                            count = cmd.ExecuteNonQuery();
                            if (count > 0)
                                correcto = true;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                //ERROR...
            }

            return correcto;
        }

        /// <summary>
        /// Libera las tareas que hayan quedado 'colgadas'.
        /// </summary>
        /// <returns></returns>
        public static bool CleanTaskColgadas()
        {
            bool correcto = false;
            List<string> usersLog = Sesion.GetListUsers(fnSistema.pgDatabase);
            List<string> usersTask = ListTareasUsuarios();
            int count = 0, cproc = 0;
            string sql = "UPDATE proceso SET activo=0";
            SqlConnection cn;
            SqlCommand cmd;            

            if (usersLog.Count > 0 && usersTask.Count > 0)
            {
                foreach (var item in usersLog)
                {
                    //ALGUN USUARIO LOGUEADO TIENE UNA TAREA ACTIVA???
                    if (usersTask.Find(x => x.ToLower().Equals(item.ToLower())) != null)
                        count++;                    
                }

                //SI COUNT ES MAYOR QUE 0, SIGNIFICA QUE ALGUN USUARIO LOGUEADO TIENE UN PROCESO ACTIVO (COD=1)
                //si count es 0, SIGNIFICA QUE NO HAY NINGUN USUARIO LOGUEADO QUE TENGA UN PROCESO ACTIVO
                if (count == 0)
                {
                    //LIMPIAMOS PROCESOS ACTIVOS
                    try
                    {
                        cn = fnSistema.OpenConnection();
                        if (cn != null)
                        {
                            using (cn)
                            {
                                using (cmd = new SqlCommand(sql, cn))
                                {
                                    cproc = cmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        //ERROR...
                    }   
                }
            }

           

            return correcto;
        }

        /// <summary>
        /// Entrega un listado de usuarios que tienen tareas activas (1)
        /// </summary>
        private static List<string> ListTareasUsuarios()
        {
            string sql = "SELECT usr FROM proceso WHERE activo=1";
            SqlCommand cmd;
            SqlConnection cn;
            SqlDataReader rd;
            List<string> data = new List<string>();

            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            rd = cmd.ExecuteReader();
                            if (rd.HasRows)
                            {
                                while (rd.Read())
                                {
                                    data.Add((string)rd["usr"]);
                                }
                            }
                            rd.Close();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
              //ERROR...
            }

            return data;
        }

    }

    /// <summary>
    /// Representa una tarea en ejecución.
    /// </summary>
    class Tarea
    {
        /// <summary>
        /// Indica si la tarea esta activa o no.
        /// </summary>
        public Int16 Activo { get; set; }
        /// <summary>
        /// Descripcion de la tarea.
        /// </summary>
        public string DescTarea { get; set; }
        /// <summary>
        /// Codigo interno de la tarea.
        /// </summary>
        public string Cod { get; set; }

        /// <summary>
        /// Usuario que inicia la tarea
        /// </summary>
        public string User { get; set; }
    }
}
