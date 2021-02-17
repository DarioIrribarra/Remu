using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReportesExternos
{
    public class ReporteContable
    {
        //#region VARIABLES CONEXIÓN
        //public string pgServer { get; set; }
        //public string pgUser { get; set; }
        //public string pgPassword { get; set; }
        //public string pgDatabase { get; set; }
        //#endregion
        public string c1 { get; set; }
        public string c2 { get; set; }
        public string c3 { get; set; }
        public string c4 { get; set; }
        public string c5 { get; set; }
        public string c6 { get; set; }
        public string c7 { get; set; }
        public string c8 { get; set; }
        public string n1 { get; set; }
        public string n2 { get; set; }
        public string n3 { get; set; }
        public string n4 { get; set; }
        public string n5 { get; set; }
        public string n6 { get; set; }
        public string n7 { get; set; }
        public string n8 { get; set; }
        public string s1 { get; set; }
        public string s2 { get; set; }
        public string s3 { get; set; }
        public string s4 { get; set; }
        public string s5 { get; set; }
        public string s6 { get; set; }
        public string s7 { get; set; }
        public string s8 { get; set; }
        public int num1 { get; set; }
        public int num2 { get; set; }
        public int num3 { get; set; }
        public int num4 { get; set; }
        public int num5 { get; set; }
        public int num6 { get; set; }
        public int num7 { get; set; }
        public int num8 { get; set; }
        public int or1 { get; set; }
        public int or2 { get; set; }
        public int or3 { get; set; }
        public int or4 { get; set; }
        public int or5 { get; set; }
        public int or6 { get; set; }
        public int or7 { get; set; }
        public int or8 { get; set; }

        public ReporteContable() 
        {

        }

        /*public ReporteContable(string pgServer, string pgUser, string pgPassword, string pgDatabase)
        {
            this.pgServer = pgServer;
            this.pgUser = pgUser;
            this.pgPassword = pgPassword;
            this.pgDatabase = pgDatabase;
        }
        */

        /// <summary>
        /// INGRESA LOS DATOS PARA CONFIGURACION DEL REPORTE CONTABLE
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <param name="c3"></param>
        /// <param name="c4"></param>
        /// <param name="c5"></param>
        /// <param name="c6"></param>
        /// <param name="c7"></param>
        /// <param name="c8"></param>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <param name="n3"></param>
        /// <param name="n4"></param>
        /// <param name="n5"></param>
        /// <param name="n6"></param>
        /// <param name="n7"></param>
        /// <param name="n8"></param>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <param name="s3"></param>
        /// <param name="s4"></param>
        /// <param name="s5"></param>
        /// <param name="s6"></param>
        /// <param name="s7"></param>
        /// <param name="s8"></param>
        public void Insert(string c1, string c2, string c3, string c4, string c5, string c6,
            string c7, string c8, string n1, string n2, string n3, string n4, string n5,
             string n6, string n7, string n8, string s1, string s2, string s3, string s4,
             string s5, string s6, string s7, string s8, int num1, int num2, int num3,
             int num4, int num5, int num6, int num7, int num8, int or1, int or2, int or3,
             int or4, int or5, int or6, int or7, int or8
            )
        {
            string sql = "INSERT INTO repcontable (c1, c2, c3, c4, c5, c6, c7, " +
                "c8, n1, n2, n3, n4, n5, n6, n7, n8, s1, s2, s3, s4, s5, s6, s7, s8, " +
                "num1, num2, num3, num4, num5, num6, num7, num8, " +
                "or1, or2, or3, or4, or5, or6, or7, or8) " +
                "VALUES(@c1, @c2, @c3, @c4, @c5, @c6, @c7, @c8, @n1, @n2, @n3, @n4, " +
                "@n5, @n6, @n7,@n8, @s1, @s2, @s3, @s4, @s5, @s6, @s7, @s8, " +
                "@num1, @num2, @num3, @num4, @num5, @num6, @num7, @num8," +
                "@or1, @or2, @or3, @or4, @or5, @or6, @or7, @or8)";

            string sqldel = "DELETE FROM repcontable";

            SqlConnection cn;
            SqlCommand cmd;
            SqlTransaction tr;
            int count = 0;
            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        tr = cn.BeginTransaction();
                        try
                        {
                            //Eliminar registros previos
                            using (cmd = new SqlCommand(sqldel, cn))
                            {
                                cmd.Transaction = tr;
                                cmd.ExecuteNonQuery();
                            }

                            //Ingresar nuevos registros
                            using (cmd = new SqlCommand(sql, cn))
                            {
                                //parametros
                                cmd.Parameters.Add(new SqlParameter("@c1", c1));
                                cmd.Parameters.Add(new SqlParameter("@c2", c2));
                                cmd.Parameters.Add(new SqlParameter("@c3", c3));
                                cmd.Parameters.Add(new SqlParameter("@c4", c4));
                                cmd.Parameters.Add(new SqlParameter("@c5", c5));
                                cmd.Parameters.Add(new SqlParameter("@c6", c6));
                                cmd.Parameters.Add(new SqlParameter("@c7", c7));
                                cmd.Parameters.Add(new SqlParameter("@c8", c8));
                                cmd.Parameters.Add(new SqlParameter("@n1", n1));
                                cmd.Parameters.Add(new SqlParameter("@n2", n2));
                                cmd.Parameters.Add(new SqlParameter("@n3", n3));
                                cmd.Parameters.Add(new SqlParameter("@n4", n4));
                                cmd.Parameters.Add(new SqlParameter("@n5", n5));
                                cmd.Parameters.Add(new SqlParameter("@n6", n6));
                                cmd.Parameters.Add(new SqlParameter("@n7", n7));
                                cmd.Parameters.Add(new SqlParameter("@n8", n8));
                                cmd.Parameters.Add(new SqlParameter("@s1", s1));
                                cmd.Parameters.Add(new SqlParameter("@s2", s2));
                                cmd.Parameters.Add(new SqlParameter("@s3", s3));
                                cmd.Parameters.Add(new SqlParameter("@s4", s4));
                                cmd.Parameters.Add(new SqlParameter("@s5", s5));
                                cmd.Parameters.Add(new SqlParameter("@s6", s6));
                                cmd.Parameters.Add(new SqlParameter("@s7", s7));
                                cmd.Parameters.Add(new SqlParameter("@s8", s8));
                                cmd.Parameters.Add(new SqlParameter("@num1", num1));
                                cmd.Parameters.Add(new SqlParameter("@num2", num2));
                                cmd.Parameters.Add(new SqlParameter("@num3", num3));
                                cmd.Parameters.Add(new SqlParameter("@num4", num4));
                                cmd.Parameters.Add(new SqlParameter("@num5", num5));
                                cmd.Parameters.Add(new SqlParameter("@num6", num6));
                                cmd.Parameters.Add(new SqlParameter("@num7", num7));
                                cmd.Parameters.Add(new SqlParameter("@num8", num8));
                                cmd.Parameters.Add(new SqlParameter("@or1", or1));
                                cmd.Parameters.Add(new SqlParameter("@or2", or2));
                                cmd.Parameters.Add(new SqlParameter("@or3", or3));
                                cmd.Parameters.Add(new SqlParameter("@or4", or4));
                                cmd.Parameters.Add(new SqlParameter("@or5", or5));
                                cmd.Parameters.Add(new SqlParameter("@or6", or6));
                                cmd.Parameters.Add(new SqlParameter("@or7", or7));
                                cmd.Parameters.Add(new SqlParameter("@or8", or8));

                                cmd.Transaction = tr;
                                count = cmd.ExecuteNonQuery();
                                if (count > 0)
                                {
                                    XtraMessageBox.Show("Datos guardados correctamente", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                else
                                {
                                    XtraMessageBox.Show("No se pudieron guardar los datos", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                            }

                            tr.Commit();
                        }
                        catch (Exception ex)
                        {
                            tr.Rollback();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                //error...
                XtraMessageBox.Show("No se pudo realizar opeación", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }



        /// <summary>
        /// Trae la informacion almacenada en la base de datos
        /// </summary>
        public void SetInfo()
        {
            string sql = "SELECT * FROM repcontable";
            SqlCommand cmd;
            SqlConnection cn;
            SqlDataReader rd;

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

                                    bool x = Convert.ToBoolean(rd["num1"]);
                                    c1 = Convert.ToString(rd["c1"]);
                                    c2 = Convert.ToString(rd["c2"]);
                                    c3 = Convert.ToString(rd["c3"]);
                                    c4 = Convert.ToString(rd["c4"]);
                                    c5 = Convert.ToString(rd["c5"]);
                                    c6 = Convert.ToString(rd["c6"]);
                                    c7 = Convert.ToString(rd["c7"]);
                                    c8 = Convert.ToString(rd["c8"]);
                                    n1 = Convert.ToString(rd["n1"]);
                                    n2 = Convert.ToString(rd["n2"]);
                                    n3 = Convert.ToString(rd["n3"]);
                                    n4 = Convert.ToString(rd["n4"]);
                                    n5 = Convert.ToString(rd["n5"]);
                                    n6 = Convert.ToString(rd["n6"]);
                                    n7 = Convert.ToString(rd["n7"]);
                                    n8 = Convert.ToString(rd["n8"]);
                                    s1 = Convert.ToString(rd["s1"]);
                                    s2 = Convert.ToString(rd["s2"]);
                                    s3 = Convert.ToString(rd["s3"]);
                                    s4 = Convert.ToString(rd["s4"]);
                                    s5 = Convert.ToString(rd["s5"]);
                                    s6 = Convert.ToString(rd["s6"]);
                                    s7 = Convert.ToString(rd["s7"]);
                                    s8 = Convert.ToString(rd["s8"]);
                                    num1 = Convert.ToInt32(rd["num1"]);
                                    num2 = Convert.ToInt32(rd["num2"]);
                                    num3 = Convert.ToInt32(rd["num3"]);
                                    num4 = Convert.ToInt32(rd["num4"]);
                                    num5 = Convert.ToInt32(rd["num5"]);
                                    num6 = Convert.ToInt32(rd["num6"]);
                                    num7 = Convert.ToInt32(rd["num7"]);
                                    num8 = Convert.ToInt32(rd["num8"]);
                                    or1 = Convert.ToInt32(rd["or1"]);
                                    or2 = Convert.ToInt32(rd["or2"]);
                                    or3 = Convert.ToInt32(rd["or3"]);
                                    or4 = Convert.ToInt32(rd["or4"]);
                                    or5 = Convert.ToInt32(rd["or5"]);
                                    or6 = Convert.ToInt32(rd["or6"]);
                                    or7 = Convert.ToInt32(rd["or7"]);
                                    or8 = Convert.ToInt32(rd["or8"]);
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
        }
    }
}
