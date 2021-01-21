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
using System.Runtime.InteropServices;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraEditors.Repository;
using DevExpress.Utils.Menu;

namespace Labour
{
    public partial class frmDatosEmpleado : DevExpress.XtraEditors.XtraForm, IConjuntosCondicionales
    {
        [DllImport("user32")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32")]
        static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        const int MF_BYCOMMAND = 0;
        const int MF_DISABLED = 2;
        const int SC_CLOSED = 0xF060;

        #region "CONJUNTOS CONDICIONALES"
        public void CargarCodigoConjunto(string code)
        {
            txtConjunto.Text = code;
        }
        #endregion

        //PARA GUARDAR EL FILTRO DEL USUARIO LOGUEADO
        private string FiltroUsuario { get; set; } = User.GetUserFilter();

        //USUARIO PUEDE VER FICHAS PRIVADAS
        private bool ShowPrivados { get; set; } = User.ShowPrivadas();

        /// <summary>
        /// Consulta base.
        /// </summary>
        string sqlConsulta = "SELECT anomes, contrato,  " +
                             "CONCAT(nombre, ' ', apepaterno, ' ', apematerno) as nombre FROM TRABAJADOR ";

        public frmDatosEmpleado()
        {
            InitializeComponent();
        }

        private void frmDatosEmpleado_Load(object sender, EventArgs e)
        {
            var sm = GetSystemMenu(Handle, false);
            EnableMenuItem(sm, SC_CLOSED, MF_BYCOMMAND | MF_DISABLED);

            datoCombobox.spLlenaPeriodos("SELECT anomes FROM parametro ORDER BY anomes DESC", txtComboPeriodo, "anomes", "anomes", true);           
            


            if (txtComboPeriodo.Properties.DataSource != null)
            {
                txtComboPeriodo.ItemIndex = 0;
                CargaTotal(Convert.ToInt32(txtComboPeriodo.EditValue), "", txtTrabajador.Text);
            }
         
        }

        #region "MANEJO DE DATOS"
        //CARGA DE DATOS EN GRILLA
        private void CargaTotal(int pPeriodo, string pConjunto, string pBusqueda)
        {
            string sql = "", condUser = "";
            string sqlFiltro = "";

            sqlFiltro = Calculo.GetSqlFiltro(FiltroUsuario, pConjunto, ShowPrivados);

            if (pBusqueda.Length > 0)
            {
                if(fnSistema.IsNumeric(pBusqueda))
                    sqlFiltro = sqlConsulta + $" WHERE anomes={pPeriodo} AND contrato LIKE '%{pBusqueda}%' {sqlFiltro} ORDER BY apepaterno";
                else
                    sqlFiltro = sqlConsulta + $" WHERE anomes={pPeriodo} AND (nombre LIKE '%{pBusqueda}%' OR apepaterno LIKE '%{pBusqueda}%' OR apematerno LIKE '%{pBusqueda}%') {sqlFiltro} ORDER BY apepaterno";
            }                
            else
                sqlFiltro = sqlConsulta + $" WHERE anomes={pPeriodo} {sqlFiltro} ORDER BY apepaterno";            

            fnSistema.spllenaGridView(gridTrabajador, sqlFiltro);
            fnSistema.spOpcionesGrilla(viewTrabajador);
            ColumnasGrilla();
        }

        //OPCIONES COLUMNAS
        private void ColumnasGrilla()
        {
            //CONTRATO Y NOMBRE
            if (viewTrabajador.RowCount>0)
            {
                viewTrabajador.Columns[0].Visible = false;
                viewTrabajador.Columns[1].Caption = "Contrato";
                viewTrabajador.Columns[1].Width = 20;
                viewTrabajador.Columns[2].Caption = "Nombre";
            }

        }

        //FICHA EMPLEADO
        private void FichaEmpleado(string contrato, int periodo)
        {
            string sql = " select contrato, trabajador.rut, CONCAT(trabajador.nombre, ' ', apepaterno, ' ', apematerno)as name, fechanac,sexo, direccion, telefono, ingreso, salida, tipocontrato, " +
                        " isapre.nombre as salud, area.nombre as area, banco.nombre as banco, cuenta,  cajaPrevision.nombre as caja, cargo.nombre as cargo, causalTermino.descCausal, " +
                        " ccosto.nombre as costo, ciudad.descCiudad as ciudad, ecivil.nombre as civil, formapago.nombre as formapago, nacion.nombre as pais, tipocuenta.nombre as tipocuenta, afp.nombre as afp" +
                        " FROM trabajador" +
                        " LEFT JOIN isapre ON isapre.id = trabajador.salud " +
                        " LEFT JOIN area on area.id = trabajador.area " +
                        " LEFT JOIN banco on banco.id = trabajador.banco " +
                        " LEFT JOIN cajaPrevision ON cajaPrevision.id = trabajador.cajaPrevision " +
                        " LEFT JOIN cargo ON cargo.id = trabajador.cargo " +
                        " LEFT JOIN causalTermino ON causalTermino.codCausal = trabajador.causal " +
                        " LEFT JOIN ccosto ON ccosto.id = trabajador.ccosto " +
                        " LEFT JOIN ciudad ON ciudad.idCiudad = trabajador.ciudad " +
                        " LEFT JOIN ecivil ON ecivil.id = trabajador.ecivil " +
                        " LEFT JOIN formaPago ON formapago.id = trabajador.formapago " +
                        " LEFT JOIN nacion ON nacion.id = trabajador.nacion " +
                        " LEFT JOIN tipoCuenta ON tipoCuenta.id = trabajador.tipoCuenta " +
                        " LEFT JOIN afp on afp.id = trabajador.afp " +
                        "WHERE trabajador.contrato =@contrato AND trabajador.anomes=@periodo";

            Empresa emp = new Empresa();
            emp.SetInfo();

            SqlCommand cmd;
            SqlDataAdapter ad = new SqlDataAdapter();
            DataSet ds = new DataSet();
                 
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@contrato", contrato));
                        cmd.Parameters.Add(new SqlParameter("@periodo", periodo));

                        ad.SelectCommand = cmd;
                        ad.Fill(ds, "ficha");
                        cmd.Dispose();
                        ad.Dispose();
                        fnSistema.sqlConn.Close();
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            //PASAMOS COMO DATASOURCE EL DATASET A REPORTE
                            rptTrabajador ficha = new rptTrabajador();
                            ficha.DataSource = ds.Tables[0];
                            ficha.DataMember = "ficha";

                            foreach (DevExpress.XtraReports.Parameters.Parameter parametro in ficha.Parameters)
                            {
                                parametro.Visible = false;
                            }

                            ficha.Parameters["empresa"].Value = emp.Razon;


                            Documento d = new Documento("", 0);
                            d.ShowDocument(ficha);
                            
                        }                        
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        //MANEJO DE TECLA TAB
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Tab)
            {
                if (cbTodos.Checked == false)
                {
                    if (txtConjunto.Text == "")
                    { XtraMessageBox.Show("Por favor ingresa un conjunto a evaluar", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); txtConjunto.Focus(); return false; }

                    //VERIFICA SI EL CONJUNTO EXISTE
                    if (Conjunto.ExisteConjunto(txtConjunto.Text) == false)
                    { XtraMessageBox.Show("Parece ser que el conjunto ingresado no existe", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); txtConjunto.Focus(); return false; }
                }
            
            }

            return base.ProcessDialogKey(keyData);
        }
        #endregion

        private void txtTrabajador_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) || char.IsLetter(e.KeyChar) || char.IsDigit(e.KeyChar) || char.IsSeparator(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }
        private void btnConsultar_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            if (txtComboPeriodo.Properties.DataSource == null) { XtraMessageBox.Show("Por favor ingresa un periodo a evaluar", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); txtComboPeriodo.Focus(); return; }
            if (Calculo.PeriodoValido(Convert.ToInt32(txtComboPeriodo.EditValue)) == false) { XtraMessageBox.Show("Por favor ingresa un periodo válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtComboPeriodo.Focus(); return; }                        

            if (cbTodos.Checked)
            {
                //BUSCAMOS TODOS LOS REGISTROS DEL PERIODO SELECCIONADO
                CargaTotal(Convert.ToInt32(txtComboPeriodo.EditValue), "", txtTrabajador.Text);
            }
            else
            {
                if (Conjunto.ExisteConjunto(txtConjunto.Text) == false) { XtraMessageBox.Show("Parece ser que el conjunto ingresado no existe", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

                //BUSCAMOS POR CONJUNTO
                CargaTotal(Convert.ToInt32(txtComboPeriodo.EditValue), txtConjunto.Text, txtTrabajador.Text);
            }                       
            
        }

        private void txtTrabajador_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void gridTrabajador_DoubleClick(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "rptfichatrabajador") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (viewTrabajador.RowCount > 0)
            {              
                //CONTRATO Y NOMBRE
                string nombre = "", contrato = "";
                nombre = viewTrabajador.GetFocusedDataRow()["nombre"].ToString();
                contrato = viewTrabajador.GetFocusedDataRow()["contrato"].ToString();

                DialogResult pregunta = XtraMessageBox.Show("¿Desea ver ficha trabajador " + nombre + "?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (pregunta == DialogResult.Yes)
                {
                    FichaEmpleado(contrato, Convert.ToInt32(viewTrabajador.GetFocusedDataRow()["anomes"]));
                }
            }
            else
            {
                XtraMessageBox.Show("No hay registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            txtConjunto.Text = "";
            txtConjunto.Focus();
        }

        private void btnSalirAfp_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            Close();
        }

        private void viewTrabajador_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            DXPopupMenu menu = e.Menu;
            string nombre = "";
            if (viewTrabajador.RowCount > 0) nombre = viewTrabajador.GetFocusedDataRow()["nombre"] + "";                    
            if (menu != null)
            {
                DXMenuItem submenu = new DXMenuItem("Ver ficha de " + nombre, new EventHandler(verFicha_Click));
                submenu.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("images/actions/show_16x16.png");
                menu.Items.Clear();
                menu.Items.Add(submenu);
            }
        }
        private void verFicha_Click(object sender, EventArgs e)
        {
            if (objeto.ValidaAcceso(User.GetUserGroup(), "rptfichatrabajador") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (viewTrabajador.RowCount > 0)
            {
                //CONTRATO Y NOMBRE
                string nombre = "", contrato = "";
                nombre = viewTrabajador.GetFocusedDataRow()["nombre"].ToString();
                contrato = viewTrabajador.GetFocusedDataRow()["contrato"].ToString();
            
                FichaEmpleado(contrato, Convert.ToInt32(viewTrabajador.GetFocusedDataRow()["anomes"]));             
            }
            else
            {
                XtraMessageBox.Show("No hay registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

     

        private void cbTodos_CheckedChanged(object sender, EventArgs e)
        {
            if (cbTodos.Checked)
            {
                txtConjunto.Enabled = false;
                txtConjunto.Text = "";                             
                txtTrabajador.Text = "";
                btnConjunto.Enabled = false;
            }
            else
            {
                btnConjunto.Enabled = true;
                txtConjunto.Enabled = true;
                txtConjunto.Text = "";
                txtTrabajador.Text = "";
            }
        }

        private void txtPeriodo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void txtTrabajador_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar) || char.IsLetter(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void txtConjunto_DoubleClick(object sender, EventArgs e)
        {
            FrmFiltroBusqueda filtro = new FrmFiltroBusqueda(true);
            filtro.opener = this;
            filtro.ShowDialog();
        }

        private void txtConjunto_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            DXPopupMenu me = e.Menu;
            if (me != null)
            {
                me.Items.Clear();
                DXMenuItem menu = new DXMenuItem("Agregar conjunto", new EventHandler(AgregarConjunto_Click));
                menu.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("images/actions/show_16x16.png");
                me.Items.Add(menu);                    
            }
        }

        private void AgregarConjunto_Click(object sender, EventArgs e)
        {
            FrmFiltroBusqueda filtro = new FrmFiltroBusqueda(true);
            filtro.opener = this;
            filtro.ShowDialog();
        }

        private void textEdit1_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtConjunto_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void txtPeriodo_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void btnConjunto_Click(object sender, EventArgs e)
        {
            FrmFiltroBusqueda filtro = new FrmFiltroBusqueda(true);
            filtro.opener = this;
            filtro.ShowDialog();
        }
    }
}