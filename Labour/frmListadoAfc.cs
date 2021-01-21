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

namespace Labour
{
    public partial class frmListadoAfc : DevExpress.XtraEditors.XtraForm
    {
        public frmListadoAfc()
        {
            InitializeComponent();
        }

        private void frmListadoAfc_Load(object sender, EventArgs e)
        {
            LoadGridControl("");
        }

        /// <summary>
        /// Entrega data para gridview
        /// </summary>
        /// <returns></returns>
        private DataTable GetDataSource(string pSearch)
        {
            string sql = "SELECT mes as Mes, rut as Rut, monto as Monto FROM afc $search ORDER BY mes, rut";
            SqlCommand cmd;
            SqlConnection cn;
            SqlDataAdapter ad = new SqlDataAdapter();
            DataTable tabla = new DataTable();
            DataSet ds = new DataSet();

            if (pSearch.Length == 0)
                sql = sql.Replace("$search", "");
            else
                sql = sql.Replace("$search", $" WHERE rut LIKE '%{pSearch}%'");

            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            ad.SelectCommand = cmd;
                            ad.Fill(ds, "tabla");
                            if (ds.Tables.Count > 0)
                            {
                                tabla = ds.Tables[0];
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                //Error...
            }

            return tabla;
        }

        private void LoadGridControl(string pSearch)
        {
            DataTable data = new DataTable();
            data = GetDataSource(pSearch);

            if (data.Rows.Count > 0)
            {
                gridAfc.DataSource = data;
                fnSistema.spOpcionesGrilla(viewAfc);
                if (viewAfc.Columns.Count > 0)
                {
                    viewAfc.Columns[1].DisplayFormat.FormatString = "Rut";
                    viewAfc.Columns[1].DisplayFormat.Format = new FormatCustom();
                   
                }
            }
            else
            {
                XtraMessageBox.Show("No se encontró información", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void btnBusqueda_Click(object sender, EventArgs e)
        {
            LoadGridControl(txtBusqueda.Text);
        }
    }
}