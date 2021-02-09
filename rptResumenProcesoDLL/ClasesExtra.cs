using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace rptResumenProcesoDLL
{
    class ClasesExtra
    {
        /// <summary>
        /// Nombre del servidor.
        /// </summary>
        public static string pgServer;
        /// <summary>
        /// Nombre de la base de datos.
        /// </summary>
        public static string pgDatabase;
        /// <summary>
        /// Nombre de usuario.
        /// </summary>
        public static string pgUser;
        /// <summary>
        /// Contraseña usuario base de datos.
        /// </summary>
        public static string pgPass;


        #region "BASE DE DATOS"
        public static string StringCon = "";
        public static SqlConnection sqlConn;
        public static DataSet ds;
        public static DataTable sqlData;
        public static SqlDataAdapter sqlDataAdapter;
        public static SqlCommandBuilder sqlDa;
        public static SqlCommand sqlCmd;
        public static SqlDataReader sqlRe;
        public static SqlDataReader sqlDr;

        /// <summary>
        /// Abre una conexion con la bae de datos
        /// </summary>
        /// <returns></returns>
        public static bool ConectarSQLServer()
        {
            if (sqlConn != null)
                sqlConn.Dispose();

            StringCon = String.Format("Server={0};Database={1};User Id={2};Password={3}; MultipleActiveResultSets=True;Application Name='Remu'", pgServer, pgDatabase, pgUser, pgPass);

            try
            {
                //CREO UNA NUEVA INSTANCIA DE LA CLASE
                sqlConn = new SqlConnection(StringCon);

                sqlConn.Open();

                return true;
            }
            catch (SqlException ex)
            {
                return false;
            }
        }
        #endregion
        /// <summary>
        /// Abre una conexion a la base de datos y devuelve un objeto sqlconnection.
        /// <para>Util para procesos en otros hilos de ejecución.</para>
        /// </summary>
        /// <returns></returns>
        public static SqlConnection OpenConnection()
        {
            StringCon = String.Format("Server={0};Database={1};User Id={2};Password={3}; MultipleActiveResultSets=True;Application Name='Remu'", pgServer, pgDatabase, pgUser, pgPass);
            SqlConnection con = new SqlConnection();
            try
            {
                con.ConnectionString = StringCon;
                con.Open();
            }
            catch (SqlException ex)
            {
                //ERROR...
                con = null;
            }

            return con;
        }
    }


}
