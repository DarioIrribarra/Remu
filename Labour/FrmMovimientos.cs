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
    public partial class FrmMovimientos : DevExpress.XtraEditors.XtraForm
    {
        public FrmMovimientos()
        {
            InitializeComponent();
        }

        private void FrmMovimientos_Load(object sender, EventArgs e)
        {
            datoCombobox.spLlenaPeriodos("SELECT anomes FROM parametro ORDER BY anomes", txtPeriodo, "anomes", "anomes", true);

        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;

            if (txtPeriodo.Properties.DataSource == null || txtPeriodo.EditValue == null)
            {
                XtraMessageBox.Show("Por favor selecciona un periodo válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            GetDataSource(Convert.ToInt32(txtPeriodo.EditValue));
            Cursor = Cursors.Default;
        }

        private void GetDataSource(int pPeriodo)
        {
            SqlCommand cmd;
            SqlConnection cn;
            DataSet ds = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            string sql = "SELECT DISTINCT CONCAT(Item, ' - ' , descripcion) as item, SUM(valorcalculado) as Total, " +  
                         "(select IIF(LEN(CONVERT(varchar, tipocon)) > 0, 'X', '') FROM grupocontable WHERE coditem = item AND tipocon = 1) as Debito, " +
                         "(select IIF(LEN(CONVERT(varchar, tipocon)) > 0, 'X', '') FROM grupocontable WHERE coditem = item AND tipocon = 2) as Credito, " +
                         "count(*) as Movimientos,  " +
                         "IIF(item = 'SEGINV' OR item = 'SCEMPRE', 6, (select tipo from item WHERE item.coditem = item)) as tipo " +
                         "FROM " +
                         "(SELECT it.coditem as item, item.descripcion, valorcalculado " +
                         "FROM itemtrabajador it " +
                        "INNER JOIN trabajador t on t.contrato = it.contrato " +
                        "AND t.anomes = it.anomes " +
                        "INNER JOIN item on item.coditem = it.coditem " +
                        "WHERE t.status = 1 AND t.anomes = @pPeriodo AND it.suspendido = 0) as t " +
                        "GROUP BY item, descripcion ";

            RptMovimiento re = new RptMovimiento();
            Empresa em = new Empresa();
            em.SetInfo();

            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cmd = new SqlCommand(sql, cn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pPeriodo", pPeriodo));
                        ad.SelectCommand = cmd;
                        ad.Fill(ds, "data");

                        if (ds.Tables.Count > 0)
                        {
                            re.DataSource = ds.Tables[0];
                            re.DataMember = "data";

                            re.Parameters["Periodo"].Value = fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(pPeriodo));
                            re.Parameters["Periodo"].Visible = false;
                            re.Parameters["empresa"].Visible = false;
                            re.Parameters["empresa"].Value = em.Razon;

                            Documento doc = new Documento("", 0);
                            doc.ShowDocument(re);
                        }
                        else
                        {
                            XtraMessageBox.Show("No se pudo generar la información", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show("No se pudo generar la información", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }
    }
}