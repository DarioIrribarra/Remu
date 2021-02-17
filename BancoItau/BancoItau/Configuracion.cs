using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BancoItau
{
    public class Configuracion
    {
        /// <summary>
        /// Direccion ip o nombre del servidor.
        /// </summary>
        private string Server { get; set; }
        /// <summary>
        /// Nombre de la base de datos.
        /// </summary>
        private string Database { get; set; }
        /// <summary>
        /// Usuario para conexion con base de datos.
        /// </summary>
        private string User { get; set; }
        /// <summary>
        /// Password para conexion con base de datos.
        /// </summary>
        private string Pass { get; set; }
        /// <summary>
        /// Direccion local de la bd SQLite
        /// </summary>
        private string DireccionSQLite { get; set; }

        /// <summary>
        /// Ruta para reporte externo
        /// </summary>
        public static string RutaCarpetaReportesExterno = "";

        /// <summary>
        /// String de conexión permamnente
        /// </summary>
        public static string ConnectionString = "";

        public Configuracion( string pDireccionSQLite)
        {
            DireccionSQLite = pDireccionSQLite;
        }

        public Configuracion(string pServer, string pUser, string pPassword, string pDatabase)
        {
            Server = pServer;
            User = pUser;
            Pass = pPassword;
            Database = pDatabase;
        }

        /// <summary>
        /// Retorna la cadena de conexion necesaria para abrir una conexion con una base de datos.
        /// </summary>
        /// <returns></returns>
        public string GetConnectionString()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

            if (Server.Length > 0 && Database.Length > 0 && User.Length > 0 && Pass.Length > 0)
            {
                builder.DataSource = Server;
                builder.InitialCatalog = Database;
                builder.UserID = User;
                builder.Password = Pass;
            }

            return builder.ConnectionString;
        }
        /// <summary>
        /// Retorna la cadena de conexion necesaria para abrir una conexion con una base de datos SQLITE.
        /// </summary>
        public string sQLiteGetConnectionString()
        {
            SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder();
            if (DireccionSQLite.Length > 0)
            {
                builder.DataSource = DireccionSQLite;
            }
            return builder.ConnectionString;
        }

    }

    /// <summary>
    /// Permite abrir una conexion con la base de datos.
    /// </summary>
    public class Connection
    {
        /// <summary>
        /// Representa la cadena conexion necesaria para abrir una conexion con una base de datos SQLITE.
        /// </summary>
        //private string sQLiteConnectionString = BancoItau.Properties.Resources.Culture.;
        //private string sQLiteConnectionString = $"Data Source = {BancoItau.}";

        private string sQLiteConnectionString = $"Data Source = {(Directory.GetCurrentDirectory() + "\\BancoItau.sqlite")}";
        /// <summary>
        /// Representa la cadena conexion necesaria para abrir una conexion con una base de datos.
        /// </summary>
        private string ConnectionString { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pConfig">Representa la cadena de conexion.</param>
        public Connection(string pConfig)
        {
            ConnectionString = pConfig;
        }

        public Connection()
        {
            
        }

        //public void sQLiteConnection(string pConfig)
        //{
        //    sQLiteConnectionString = pConfig;
        //}

        /// <summary>
        /// Abre una conexion con la base de datos
        /// <para>Retorna una objeto sqlconnection</para>
        /// </summary>
        public SqlConnection OpenConnection()
        {
            SqlConnection cn;

            //Conexión estática para reporte
            string stringConexion = "";

            if (Configuracion.ConnectionString.Length > 0)
                stringConexion = Configuracion.ConnectionString;
            else
                stringConexion = ConnectionString;

            if (stringConexion.Length > 0)
            {
                try
                {
                    cn = new SqlConnection(stringConexion);
                    cn.Open();
                    return cn;
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }

            return null;
        }

        /// <summary>
        /// Abre una conexion con la base de datos SQLITE
        /// <para>Retorna una objeto sqlconnection</para>
        /// </summary>
        public SQLiteConnection sQliteOpenConnection()
        {
            SQLiteConnection sqlcn;
            if (sQLiteConnectionString.Length > 0)
            {
                try
                {
                    sqlcn = new SQLiteConnection(sQLiteConnectionString);
                    sqlcn.Open();
                    return sqlcn;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }
            return null;
        }
    }
}
