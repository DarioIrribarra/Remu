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
    public partial class frmBuscarFormula : DevExpress.XtraEditors.XtraForm
    {
        //PARA COMUNICACION CON FORMULA FORMULA
        public IFormBusqueda opener;
        public frmBuscarFormula()
        {
            InitializeComponent();
        }

        #region "MANEJO DE DATOS"
        //METODO PARA REALIZAR BUSQUEDA
        private void fnBusquedaFormula(TextEdit pBusqueda)
        {
           string sql = string.Format("SELECT codFormula, descFormula FROM formula WHERE codFormula <> '0' AND (codFormula LIKE '%{0}%' OR descFormula LIKE '%{1}%')", pBusqueda.Text, pBusqueda.Text);            
           fnRecargarGrilla(gridBusqueda, sql);          
            
        }

        private void fnRecargarGrilla(DevExpress.XtraGrid.GridControl pGrid, string pSql)
        {
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    DataSet ds = new DataSet();

                    SqlCommand cmd = new SqlCommand(pSql, fnSistema.sqlConn);
                    //parametros
                    //cmd.Parameters.Add(new SqlParameter(pCampo, pDato));
                    adapter.SelectCommand = cmd;
                    adapter.Fill(ds);
                    adapter.Dispose();
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        pGrid.DataSource = ds.Tables[0];
                        fnLimpiar();
                        int filas = ds.Tables[0].Rows.Count;
                        lblresultados.Text = filas + " coincidencias";
                    }
                    else
                    {
                        XtraMessageBox.Show("No se encontraron resultados", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txtbusqueda.Text = "";
                        txtbusqueda.Focus();
                        lblresultados.Text = "Registros actuales";
                        fnSistema.spllenaGridView(gridBusqueda, "SELECT codFormula, descFormula FROM formula WHERE codFormula <> '0'");
                        fnSistema.spOpcionesGrilla(viewBusqueda);
                        fnColumnas();                            
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

        }

        //LIMPIAR CAMPO
        private void fnLimpiar()
        {
            txtbusqueda.Text = "";
            txtbusqueda.Focus();
            lblresultados.Text = "REGISTROS ACTUALES";
            lblmsg.Visible = false;

        }
        //COLUMNAS GRILLA
        private void fnColumnas()
        {
            //SELECT codformula, descformula FROM formula
            viewBusqueda.Columns[0].Caption = "Codigo";
            viewBusqueda.Columns[0].Width = 10;
            viewBusqueda.Columns[0].AppearanceCell.FontStyleDelta = FontStyle.Bold;

            viewBusqueda.Columns[1].Caption = "Descripcion";
        }

        //DEFAULTPROPERTIES
        private void fnDefaultProperties()
        {
            btnBuscar.AllowFocus = false;
            btnLimpiar.AllowFocus = false;
            gridBusqueda.TabStop = false;
            panelControl1.TabStop = false;
            txtbusqueda.Properties.MaxLength = 100;
        }      

        #endregion

        private void txtbusqueda_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (char.IsLetter(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void frmBuscarFormula_Load(object sender, EventArgs e)
        {
            fnDefaultProperties();

            //CARGAR GRILLA
            
            string grilla = "SELECT codFormula, descFormula FROM formula WHERE codFormula <> '0'";
            fnSistema.spllenaGridView(gridBusqueda, grilla);
            fnSistema.spOpcionesGrilla(viewBusqueda);
            fnColumnas();

        }

        private void txtbusqueda_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (txtbusqueda.Text == "")
                {
                    lblmsg.Visible = true;
                    lblmsg.Text = "Por ingresa una busqueda";
                    txtbusqueda.Focus();
                }
                else
                {
                    //REALIZAR BUSQUEDA
                    lblmsg.Visible = false;
                    fnBusquedaFormula(txtbusqueda);
                }
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            fnLimpiar();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            if (txtbusqueda.Text == "")
            {
                lblmsg.Visible = true;
                lblmsg.Text = "Por favor ingresa una busqueda";
            }
            else
            {
                lblmsg.Visible = false;
                //BUSCAR
                fnBusquedaFormula(txtbusqueda);
            }
        }

        private void gridBusqueda_DoubleClick(object sender, EventArgs e)
        {
            if (viewBusqueda.RowCount > 0)
            {
                //OBTENEMOS EL CODIGO DE LA FORMULA
                string code = viewBusqueda.GetFocusedDataRow()["codFormula"].ToString();

                if (opener != null)
                {
                    opener.CargarDatos(code);
                    Close();
                }
            }
        }

        private void viewBusqueda_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            DXPopupMenu menu = e.Menu;
            if (menu != null)
            {
                DXMenuItem submenu = new DXMenuItem("Cargar Formula", new EventHandler(CargarFormula_Click));
                submenu.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("images/actions/show_16x16.png");
                menu.Items.Clear();
                menu.Items.Add(submenu);
            }
        }

        private void CargarFormula_Click(object sender, EventArgs e)
        {
            if (viewBusqueda.RowCount > 0)
            {
                //OBTENEMOS EL CODIGO DE LA FORMULA
                string code = viewBusqueda.GetFocusedDataRow()["codFormula"].ToString();

                if (opener != null)
                {
                    opener.CargarDatos(code);
                    Close();
                }
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            Close();
        }
    }
}