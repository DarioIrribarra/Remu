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
using System.IO;

namespace Labour
{
    public partial class frmMontoAnual : DevExpress.XtraEditors.XtraForm
    {
        public frmMontoAnual()
        {
            InitializeComponent();
        }

        private void frmMontoAnual_Load(object sender, EventArgs e)
        {
            CargaCombo(txtYear);
        }

        private void btnSalirArea_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            Close();
        }

        #region "MANEJO DE DATOS"
        /// <summary>
        /// Retorna todos los años disponibles para consultar.
        /// </summary>
        /// <returns></returns>
        private List<formula> GetListYears()
        {
            string sql = "SELECT DISTINCT SUBSTRING(CAST(anomes AS VARCHAR(6)), 1, 4) as y from parametro ORDER BY y";
            SqlCommand cmd;
            SqlConnection cn;
            SqlDataReader rd;

            List<formula> listado = new List<formula>();

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
                                    listado.Add(new formula() { key = (string)rd["y"], desc =(string)rd["y"]});
                                }
                            }

                            cmd.Dispose();
                            rd.Close();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                //ERROR
            }

            return listado;
        }

        private void CargaCombo(LookUpEdit pCombo)
        {
            List<formula> Listado = new List<formula>();
            Listado = GetListYears();

            if (Listado.Count > 0)
            {
                pCombo.Properties.DataSource = Listado.ToList();
                pCombo.Properties.PopulateColumns();
                
                pCombo.Properties.ValueMember = "key";
                pCombo.Properties.DisplayMember = "desc";
                pCombo.Properties.Columns[0].Visible = false;

                pCombo.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
                pCombo.Properties.AutoSearchColumnIndex = 1;
                pCombo.Properties.ShowHeader = false;

                if (pCombo.Properties.DataSource != null)
                    pCombo.ItemIndex = 0;
            }
        }


        #endregion

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            SaveFileDialog dir = new SaveFileDialog();
            dir.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            dir.Filter = "Excel Files|*.xlsx;";
            //FolderBrowserDialog dir = new FolderBrowserDialog();

            if (dir.ShowDialog() == DialogResult.OK)
            {
                txtRuta.Text = dir.FileName;                
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtYear.Properties.DataSource == null || txtYear.EditValue == null)
            { XtraMessageBox.Show("Por favor selecciona un periodo válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtYear.Focus(); return; }

            Declaracion dec = new Declaracion();
            DataTable Tabla = new DataTable();

            try
            {
                //Para realizar pruebas se da un máximo de 95 ruts
                if (chkPruebasMax95Ruts.Checked)
                {
                    Tabla = dec.GetInformationYear(Convert.ToInt32(txtYear.EditValue.ToString()), true);
                }
                else { 
                    Tabla = dec.GetInformationYear(Convert.ToInt32(txtYear.EditValue.ToString()));

                }



                if (Tabla == null)
                { XtraMessageBox.Show("No se encontraron resultados", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                FileExcel.CrearArchivoExcelDev(Tabla, txtRuta.Text);
                if (File.Exists(txtRuta.Text))
                {
                    DialogResult adv = XtraMessageBox.Show("Archivo creado correctamente.\n¿Deseas ver archivo?", "Archivo", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (adv == DialogResult.Yes)
                        System.Diagnostics.Process.Start(txtRuta.Text);
                }
                else
                {
                    XtraMessageBox.Show("Archivo no se pudo generar", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Archivo no se pudo generar", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }
    }
}