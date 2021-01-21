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
    public partial class frmconfLibro : DevExpress.XtraEditors.XtraForm
    {
        private string SqlGrid = "SELECT orden, coditem, visible, negrita, cursiva, alias, tipo FROM libro ORDER BY orden";
        private string SqlCombo = "SELECT coditem, descripcion FROM item ORDER BY coditem";
        private string SqlComboSistema = "SELECT codVariable, descVariable FROM variableSistema ORDER BY descVariable";

        private bool Update = false;
        private bool cancela = false;

        Operacion op;

        public frmconfLibro()
        {
            InitializeComponent();
        }

        private void btnGrabar_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            if (txtItem.Properties.DataSource == null) { XtraMessageBox.Show("Item no valido", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
            if (txtItem.EditValue == "") { XtraMessageBox.Show("Item no valido", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
            if (txtNum.Text == "") { XtraMessageBox.Show("Por favor ingresa un numero de orden", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
            if (txtAlias.Text == "") { XtraMessageBox.Show("Por favor ingresa una descripción", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtAlias.Focus(); return; }
            if (itemEnTablaCalculoMensual(txtItem.EditValue.ToString(), int.Parse(cbxTipoItem.EditValue.ToString())) == false)
            {
                XtraMessageBox.Show("Item no contiene valor. Seleccione otro item.", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); cbxTipoItem.Focus(); return;
            }
            int totalRegistros = CantidadItems();
            string itemDb = "";
            //ACTUALIZAR
            if (Update)
            {
                if (txtNum.Text == "0") { XtraMessageBox.Show("El numero de orden no puede ser 0", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                //OBTENER ITEM SELECCIONADO
                if (viewLibro.RowCount > 0)
                {
                    itemDb = (string)viewLibro.GetFocusedDataRow()["coditem"];
                    if (itemDb != txtItem.EditValue.ToString())
                    {
                        if (ExisteItem(txtItem.EditValue.ToString()))
                        { XtraMessageBox.Show("Item ya existe", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                    }
                }

                ActualizarItemLibro(txtItem, txtNum, cbVisible, cbNegrita, cbCursiva, itemDb, txtAlias, cbxTipoItem);
            }
            else
            {
                //INSERT
                if (ExisteItem(txtItem.EditValue.ToString()))
                { XtraMessageBox.Show("Item ya ingresado", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); op.Cancela = false;op.SetButtonProperties(btnNuevo, 1); btnEliminar.Enabled = true; return; }

                if (txtNum.Text == "0") { XtraMessageBox.Show("El numero de orden no puede ser 0", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }               

                IngresoItemLibro(txtItem, txtNum, cbVisible, cbNegrita, cbCursiva, txtAlias);
            }
        }

        private void frmconfLibro_Load(object sender, EventArgs e)
        {

            op = new Operacion();            
            
            fnComboItem(SqlCombo, txtItem, "coditem", "descripcion", true);
            txtItem.ItemIndex = 0;
            fnSistema.spllenaGridView(gridLibro, SqlGrid);
            fnSistema.spOpcionesGrilla(viewLibro);
            llenaComboboxTipo(cbxTipoItem);
            //LLENA CBX TIPO
            cbxTipoItem.ItemIndex = 0;

            ColumnasGrilla();

            CargarCampos(0);
            viewLibro.Columns["tipo"].Visible = false;
        }

        //METODO PARA CARGAR COMBO tramo
        //TIENE CUATRO VALORES 1, 2, 3, 4
        private void llenaComboboxTipo(LookUpEdit pCombo)
        {
            //instanciamos a la clase combo
            List<datoCombobox> lista = new List<datoCombobox>();
            lista.Add(new datoCombobox() { descInfo = "ITEM TABLA LIBRO", KeyInfo = 0 });
            lista.Add(new datoCombobox() { descInfo = "VARIABLE SISTEMA", KeyInfo = 1 });
            //lista.Add(new datoCombobox() { descInfo = "FORMULA", KeyInfo = 2 });

            //PROPIEDADES COMBOBOX
            pCombo.Properties.DataSource = lista.ToList();
            pCombo.Properties.ValueMember = "KeyInfo";
            pCombo.Properties.DisplayMember = "descInfo";

            pCombo.Properties.PopulateColumns();

            //ocultamos la columan key
            pCombo.Properties.Columns[0].Visible = false;
            //pCombo.Properties.Columns[1].Visible = false;

            pCombo.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
            pCombo.Properties.AutoSearchColumnIndex = 1;
            pCombo.Properties.ShowHeader = false;
        }

        #region "MANEJO DE DATOS"
        //INGRESO
        private void IngresoItemLibro(LookUpEdit pItem, TextEdit pNum, CheckEdit pVisible, CheckEdit pNegrita, CheckEdit pCursiva, TextEdit pAlias)
        {
            //INGRESO
            string sqlinsert = "INSERT INTO libro(orden, coditem, visible, negrita, cursiva, alias) VALUES(@pNum, @pItem, " +
                "@pVisible, @pNegrita, @pCursiva, @pAlias)";
            SqlCommand cmd;
            int count = 0;

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sqlinsert, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pNum", Convert.ToInt32(pNum.Text)));
                        cmd.Parameters.Add(new SqlParameter("@pItem", pItem.EditValue));
                        cmd.Parameters.Add(new SqlParameter("@pVisible", pVisible.Checked));
                        cmd.Parameters.Add(new SqlParameter("@pNegrita", pNegrita.Checked));
                        cmd.Parameters.Add(new SqlParameter("@pCursiva", pCursiva.Checked));
                        cmd.Parameters.Add(new SqlParameter("@pAlias", pAlias.Text));

                        count = cmd.ExecuteNonQuery();
                        if (count > 0)
                        {
                            XtraMessageBox.Show("Ingreso correcto", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            fnSistema.spllenaGridView(gridLibro, SqlGrid);
                            fnSistema.spOpcionesGrilla(viewLibro);
                            ColumnasGrilla();
                            CargarCampos(viewLibro.FocusedRowHandle);                            
                        }
                        else
                        {
                            XtraMessageBox.Show("Error al ingresar", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        }

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        //UPDATE
        private void ActualizarItemLibro(LookUpEdit pItem, TextEdit pNum, CheckEdit pVisible, CheckEdit pNegrita, CheckEdit pCursiva, string Itemdb, TextEdit pAlias, LookUpEdit pTipo)
        {
            string sql = "UPDATE libro SET coditem=@pItem, visible=@pVisible, negrita=@pNegrita, " +
                "cursiva=@pCursiva, alias=@pAlias, tipo=@pTipo WHERE orden=@pNum ";

            SqlCommand cmd;
            int count = 0;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {

                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pItem", pItem.EditValue));
                        cmd.Parameters.Add(new SqlParameter("@pNum", Convert.ToInt32(pNum.Text)));
                        cmd.Parameters.Add(new SqlParameter("@pVisible", pVisible.Checked));
                        cmd.Parameters.Add(new SqlParameter("@pNegrita", pNegrita.Checked));
                        cmd.Parameters.Add(new SqlParameter("@pCursiva", pCursiva.Checked));
                        cmd.Parameters.Add(new SqlParameter("@pItemdb", Itemdb));
                        cmd.Parameters.Add(new SqlParameter("@pAlias", pAlias.Text.ToUpper()));
                        cmd.Parameters.Add(new SqlParameter("@pTipo", pTipo.EditValue));

                        count = cmd.ExecuteNonQuery();
                        if (count > 0)
                        {
                            XtraMessageBox.Show("Modificacion corecta", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            fnSistema.spllenaGridView(gridLibro, SqlGrid);
                            fnSistema.spOpcionesGrilla(viewLibro);
                            ColumnasGrilla();
                            CargarCampos(viewLibro.FocusedRowHandle);

                        }
                        else
                        {
                            XtraMessageBox.Show("Error al modificar", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        //ELIMINAR 
        private void EliminarItemLibro(string pCoditem, int pOrden)
        {
            string sql = "DELETE FROM libro WHERE coditem=@pItem AND orden=@pOrden";
            SqlCommand cmd;
            int res = 0;
            DialogResult advertencia = XtraMessageBox.Show($"¿Estás seguro de eliminar el item {pCoditem}?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (advertencia == DialogResult.Yes)
            {
                try
                {
                    if (fnSistema.ConectarSQLServer())
                    {
                        using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@pItem", pCoditem));
                            cmd.Parameters.Add(new SqlParameter("@pOrden", pOrden));

                            res = cmd.ExecuteNonQuery();
                            if (res > 0)
                            {
                                XtraMessageBox.Show($"Item {pCoditem} eliminado correctamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                fnSistema.spllenaGridView(gridLibro, SqlGrid);
                                fnSistema.spOpcionesGrilla(viewLibro);
                                ColumnasGrilla();
                                CargarCampos(viewLibro.FocusedRowHandle);
                                btnNuevo.Text = "Nuevo";
                                btnNuevo.ToolTip = "Nuevo registro";
                                cancela = false;
                            }
                            else
                            {
                                XtraMessageBox.Show($"No se pudo eliminar el item {pCoditem}", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }

                            cmd.Dispose();
                            fnSistema.sqlConn.Close();
                        }
                    }
                }
                catch (SqlException ex)
                {
                    XtraMessageBox.Show(ex.Message);
                }
            }
          
        }

        //CANTIDAD DE ELEMENTOS ITEM
        private int CantidadItems()
        {
            int count = 0;
            string sql = "SELECT count(*) FROM item";
            SqlCommand cmd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        count = Convert.ToInt32(cmd.ExecuteScalar());

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();                        
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return count;
        }

        //VERIFICAR SI ITEM YA EXISTE
        private bool ExisteItem(string pItem)
        {
            bool existe = false;
            int count = 0;
            string sql = "SELECT count(*) FROM libro WHERE coditem=@pItem";
            SqlCommand cmd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMRTROS
                        cmd.Parameters.Add(new SqlParameter("@pItem", pItem));
                        count = Convert.ToInt32(cmd.ExecuteScalar());
                        if (count > 0)
                            existe = true;
                    }

                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
            return existe;
        }

        //CARGAR COMBO CODIGO ITEM
        private void fnComboItem(string pSql, LookUpEdit pCombo, string pCampoKey, string pCampoDesc, bool? pOcultarKey = false)
        {
            List<Combos> lista = new List<Combos>();
            SqlCommand cmd;
            SqlDataReader rd;

            if (fnSistema.ConectarSQLServer())
            {
                using (cmd = new SqlCommand(pSql, fnSistema.sqlConn))
                {
                    rd = cmd.ExecuteReader();
                    if (rd.HasRows)
                    {
                        while (rd.Read())
                        {
                            //AGREGAMOS VALORES A LA LISTA
                            lista.Add(new Combos() { key = rd[pCampoKey].ToString().ToUpper(), desc = (string)rd[pCampoDesc] });
                        }
                    }
                }
                //LIBERAR RECURSOS
                cmd.Dispose();
                rd.Close();
                fnSistema.sqlConn.Close();
            }

            pCombo.Properties.DataSource = lista.ToList();
            pCombo.Properties.ValueMember = "key";
            pCombo.Properties.DisplayMember = "desc";

            pCombo.Properties.PopulateColumns();
            //SI OCULTAR KEY ES TRUE NO SE DEBE MOSTRAR LA COLUMNA KEY
            if (pOcultarKey == true)
            {
                pCombo.Properties.Columns[0].Visible = false;
            }
            pCombo.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
            pCombo.Properties.AutoSearchColumnIndex = 1;
            pCombo.Properties.ShowHeader = false;
        }

        //LIMPIAR CAMPOS
        private void LimpiarCampos()
        {
            op.SetButtonProperties(btnNuevo, 2);
            op.Cancela = true;
            txtItem.ItemIndex = 0;
            txtNum.Text = "1";
            cbVisible.Checked = true;
            cbCursiva.Checked = false;
            cbNegrita.Checked = false;
            Update = false;
            btnEliminar.Enabled = false;

            //CAMBIAR COLOR COMBOBOX
            txtItem.Properties.Appearance.BackColor = Color.LightYellow;
            txtItem.Focus();

            
        }

        //CARGAR CAMPOS
        private void CargarCampos(int? pos = -1)
        {
            if (viewLibro.RowCount>0)
            {
                if (pos != -1) viewLibro.FocusedRowHandle = (int)pos;

                Update = true;
                btnEliminar.Enabled = true;
                //var prueba = (int)viewLibro.GetFocusedDataRow()["tipo"];
                cbxTipoItem.EditValue = Convert.ToInt32(viewLibro.GetFocusedDataRow()["tipo"].ToString());
                txtItem.EditValue = (string)viewLibro.GetFocusedDataRow()["coditem"];
                txtNum.Text = Convert.ToInt32(viewLibro.GetFocusedDataRow()["orden"]).ToString();
                cbVisible.Checked = (bool)viewLibro.GetFocusedDataRow()["visible"];
                cbNegrita.Checked = (bool)viewLibro.GetFocusedDataRow()["negrita"];
                cbCursiva.Checked = (bool)viewLibro.GetFocusedDataRow()["cursiva"];
                txtAlias.Text = (string)viewLibro.GetFocusedDataRow()["alias"];

                op.Cancela = false;
                op.SetButtonProperties(btnNuevo, 1);

                //REESTABLECER COLOR COMBOBOX
                txtItem.Properties.Appearance.BackColor = Color.White;
            }
        }

        //PROPIEDADES GRILLA
        private void ColumnasGrilla()
        {
            viewLibro.Columns[0].Caption = "N°";
            viewLibro.Columns[0].Width = 20;

            viewLibro.Columns[1].Caption = "Item";
            viewLibro.Columns[1].Width = 100;            
        }                

        #endregion

        private void txtNum_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            //SI CANCELA ES TRUE ES PORQUE YA SE HIZO CLICK
            if (op.Cancela == false)
            {               
                LimpiarCampos();
            }
            else
            {             
                
                CargarCampos();
            }
        }

        private void gridLibro_Click(object sender, EventArgs e)
        {
            if (viewLibro.RowCount>0)
            {
                CargarCampos();
            }
        }

        private void gridLibro_KeyUp(object sender, KeyEventArgs e)
        {
            if (viewLibro.RowCount>0)
            {
                CargarCampos();
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            string cod = "";
            int Orden = 0;

            if(viewLibro.RowCount == 0)
            { XtraMessageBox.Show("Registro no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (viewLibro.RowCount > 0)
            {
                cod = (string)viewLibro.GetFocusedDataRow()["coditem"];
                Orden = Convert.ToInt32(viewLibro.GetFocusedDataRow()["orden"]);

                EliminarItemLibro(cod, Orden);
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void cbxTipoItem_EditValueChanged(object sender, EventArgs e)
        {

            if (cbxTipoItem.EditValue.ToString() == "0")
            {
                fnComboItem(SqlCombo, txtItem, "coditem", "descripcion", true);
                txtItem.ItemIndex = 0;
            }
            if (cbxTipoItem.EditValue.ToString() == "1")
            {
                fnComboItem(SqlComboSistema, txtItem, "codVariable", "descVariable", true);
                txtItem.ItemIndex = 0;
            }
        }

        private bool itemEnTablaCalculoMensual(string pItem, int pTipo)
        {
            string sql = "";
            bool existe = false;
            SqlCommand sqlcmd;
            string count = "";
            //SI EL ITEM ES DEL ITEM DE LIBRO
            if (pTipo == 0)
            {
                sql = $"SELECT coditem FROM item WHERE coditem = '{pItem}'";
                if (fnSistema.ConectarSQLServer())
                {
                    using (sqlcmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        try
                        {
                            count = sqlcmd.ExecuteScalar().ToString();
                            if (count != "")
                                existe = true;
                        }
                        catch (Exception)
                        {

                            existe = false;
                        }
                        
                    }
                }
            }

            //SI EL ITEM ES DE VARIABLE DE SISTEMA
            if (pTipo == 1)
            {
                sql = $"SELECT TOP 1 {pItem} FROM calculoMensual";
                if (fnSistema.ConectarSQLServer())
                {
                    using (sqlcmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        try
                        {
                            count = sqlcmd.ExecuteScalar().ToString();
                            if (count != "")
                                existe = true;
                        }
                        catch (Exception)
                        {

                            existe = false;
                        }
                        
                    }
                }
            }
            return existe;
        }
    }
}