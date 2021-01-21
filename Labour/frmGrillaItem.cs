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
using DevExpress.Utils.Menu;

namespace Labour
{
    public partial class frmGrillaItem : DevExpress.XtraEditors.XtraForm
    {
       public IFormItemInformacion opener { get; set; }

        /// <summary>
        /// Consulta base.
        /// </summary>
        string sqlConsulta = "SELECT coditem, descripcion FROM " +
                             "(SELECT coditem, descripcion FROM ITEM " +
                             " UNION " +
                             "SELECT 'TPAGO', 'TOTAL PAGO') t";

        public frmGrillaItem()
        {
            InitializeComponent();
        }  

        private void frmGrillaItem_Load(object sender, EventArgs e)
        {
            fnSistema.spllenaGridView(gridItems, sqlConsulta);
            fnSistema.spOpcionesGrilla(viewItems);
            ColumnasGrilla();
        }

        #region "MANEJO DE DATOS"
        //COLUMNAS GRILLA
        private void ColumnasGrilla()
        {
            viewItems.Columns[0].Caption = "CODIGO";
            viewItems.Columns[0].Width = 20;

            viewItems.Columns[1].Caption = "DESCRIPCION";
        }

        //PROPIEDADES
        private void propiedadesDefault()
        {
            btnGuardar.AllowFocus = false;
            txtCodigo.Properties.MaxLength = 30;
            panelControl1.TabStop = false;
            gridItems.TabStop = false;
            btnrefresh.AllowFocus = false;
        }

        //METODO PARA FILTRAR
        private void BusquedaFiltro(string cadena)
        {
            string sql = "";

            if (cadena.Length != 0)
                sql = sqlConsulta + $" WHERE coditem LIKE '%{cadena}%' OR descripcion LIKE '%{cadena}%'";
            else
                sql = sqlConsulta;
            
            SqlCommand cmd;
            SqlDataAdapter ad = new SqlDataAdapter();
            DataSet ds = new DataSet();
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        ad.SelectCommand = cmd;
                        ad.Fill(ds);
                        cmd.Dispose();
                        ad.Dispose();
                        fnSistema.sqlConn.Close();

                        if (ds.Tables[0].Rows.Count>0)
                        {
                            //DATASOURCE A GRILLA
                            gridItems.DataSource = ds.Tables[0];
                        }
                        else
                        {
                            XtraMessageBox.Show("No se encontraron registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtCodigo.Focus();
                            gridItems.DataSource = null;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Tab)
            {
                if (txtCodigo.Text == "")
                {
                    XtraMessageBox.Show("Si deseas realizar una busqueda por favor llena el campo busqueda", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCodigo.Focus();
                }
                else
                {
                    BusquedaFiltro(txtCodigo.Text);
                }
            }

            return base.ProcessDialogKey(keyData);
        }
        #endregion

        private void gridItems_DoubleClick(object sender, EventArgs e)
        {
            if (viewItems.RowCount>0)
            {
                string code = "";
                code = (string)viewItems.GetFocusedDataRow()["coditem"];

                //CARGAMOS CAJA DE ACUERDO A CODIGO SELECCIONADO
                if (opener != null)
                {
                    opener.CargarCodigoItem(code);
                    opener.RecargaGrilla();
                    this.Close();
                }
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            BusquedaFiltro(txtCodigo.Text);
        }

        private void btnrefresh_Click(object sender, EventArgs e)
        {
            fnSistema.spllenaGridView(gridItems, sqlConsulta);
            fnSistema.spOpcionesGrilla(viewItems);
            ColumnasGrilla();
            txtCodigo.Text = "";
            txtCodigo.Focus();
        }

        private void txtCodigo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                BusquedaFiltro(txtCodigo.Text);
              
            }
        }

        private void frmGrillaItem_Shown(object sender, EventArgs e)
        {
            txtCodigo.Focus();
        }

        private void viewItems_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            DXPopupMenu p = e.Menu;
            if (p != null)
            {
                p.Items.Clear();
                DXMenuItem menu = new DXMenuItem("Seleccionar", new EventHandler(seleccion_click));
                menu.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("images/actions/show_16x16.png");

                p.Items.Add(menu);

            }
        }
        private void seleccion_click(object sender, EventArgs e)
        {
            if (viewItems.RowCount > 0)
            {
                string code = "";
                code = (string)viewItems.GetFocusedDataRow()["coditem"];

                //CARGAMOS CAJA DE ACUERDO A CODIGO SELECCIONADO
                if (opener != null)
                {
                    opener.CargarCodigoItem(code);
                    opener.RecargaGrilla();
                    this.Close();
                }
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}