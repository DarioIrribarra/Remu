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
using System.Collections;
using System.Runtime.InteropServices;

namespace Labour
{
    public partial class FrmEliminarItems : DevExpress.XtraEditors.XtraForm, IConjuntosCondicionales, ISeleccionItemElimina
    {
        [DllImport("user32")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32")]
        static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        const int MF_BYCOMMAND = 0;
        const int MF_DISABLED = 2;
        const int SC_CLOSED = 0xF060;

        //PARA GUARDAR FILTRO USUARIO
        private string FiltroUsuario { get; set; } = User.GetUserFilter();

        //PARA GUARDAR LISTADO DESDE FORMULARIO HIJO (SI SE PERMITE SELECCION DE NUMERO ITEM A ELIMINAR
        List<ItemTrabajador> ListaSeleccionItems = new List<ItemTrabajador>();

        //USUARIO PUEDE VER FICHAS PRIVADAS
        public bool ShowPrivados { get; set; } = User.ShowPrivadas();

        #region "CONJUNTOS CONDICIONALES"
        public void CargarCodigoConjunto(string code)
        {
            txtConjunto.Text = code;
        }
        #endregion

        #region "INTERFAZ SELECCION ITEM ELIMINACION"
        public void CargarSeleccion(List<ItemTrabajador> registros)
        {
            foreach (var item in registros)
            {
                ListaSeleccionItems.Add(item);
            }
        }
        #endregion
        public FrmEliminarItems()
        {
            InitializeComponent();
        }

        private void FrmEliminarItems_Load(object sender, EventArgs e)
        {
            var sm = GetSystemMenu(Handle, false);
            EnableMenuItem(sm, SC_CLOSED, MF_BYCOMMAND | MF_DISABLED);

            txtPeriodo.Text = Calculo.PeriodoObservado.ToString();
            btnConjunto.Enabled = false;
            CargarCombo();
        }

        #region "MANEJO DE DATOS"
        private string PreparaCondicion(string condicion)
        {
            if (condicion != "")
            {
                condicion = condicion.ToLower();
                if (condicion.Contains("contrato"))
                    condicion = condicion.Replace("contrato", "itemtrabajador.contrato");
                if (condicion.Contains("rut"))
                    condicion = condicion.Replace("rut", "itemtrabajador.rut");
                if (condicion.Contains("anomes"))
                    condicion = condicion.Replace("anomes", "itemtrabajador.anomes");

                return condicion;
            }
            else
                return "";
        }
        //LISTA CON TODOS LOS CODIGOS DE ITEMS
        private List<ComboItem> ListadoItems()
        {
            List<ComboItem> lista = new List<ComboItem>();
            string sql = "SELECT coditem FROM item ORDER BY coditem";
            SqlCommand cmd;
            SqlDataReader rd;
            int x = 0;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //CARGAMOS DATA EN LISTA
                                lista.Add(new ComboItem() { id = x, code = (string)rd["coditem"] });
                                x++;
                            }
                        }
                    }
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                    rd.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return lista;
        }

        //CARGAR COMBOBOS
        private void CargarCombo()
        {
            List<ComboItem> codes = new List<ComboItem>();
            codes = ListadoItems();
            if (codes.Count > 0)
            {
                txtItem.Properties.DataSource = codes.ToList();
                txtItem.Properties.ValueMember = "id";
                txtItem.Properties.DisplayMember = "code";

                txtItem.Properties.PopulateColumns();

                txtItem.Properties.Columns[0].Visible = false;
                txtItem.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
                txtItem.Properties.AutoSearchColumnIndex = 1;
                txtItem.Properties.ShowHeader = false;

                txtItem.ItemIndex = 0;
            }
        }

        //OBTENER EL TIPO, NUMERO DE ORDEN, CODIGO FORMULA
        private Hashtable DataItem(string coditem)
        {
            Hashtable data = new Hashtable();
            string sql = "SELECT descripcion, tipo, formula, orden FROM item WHERE coditem = @codigo";
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@codigo", coditem));
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //GUARDAMOS EN TABLA HASH
                                data.Add("tipo", (int)rd["tipo"]);
                                data.Add("formula", (string)rd["formula"]);
                                data.Add("orden", (int)rd["orden"]);
                                data.Add("descripcion", (string)rd["descripcion"]);
                            }
                        }
                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                        rd.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
            return data;
        }

        //EXTRAER CONDICION DE ACUERDO A CODIGO DE CONJUNTO SELECCIONADO
        private string CondicionConjunto(string code)
        {
            string sql = "SELECT cadena FROM conjunto WHERE codigo=@code";
            SqlCommand cmd;
            SqlDataReader rd;
            string condicion = "";
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@code", code));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                condicion = (string)rd["cadena"];
                            }
                        }
                    }
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                    rd.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
            return condicion;
        }

        //OBTENER LISTADO DE CONTRATOS Y RUT (PARA EL CASO DE QUE SEAN TODOS)
        private List<Employe> TodosContratos(int Periodo, string pItem)
        {
            string sql = "", condicion = "";

            //CONSULTA ANTIGUA...
            //sql = string.Format("select DISTINCT contrato, rut, anomes FROM itemtrabajador " +
            //"where anomes={0}", Periodo);

            //SI LA PROPIEDAD SHOOWPRIVADOS ES FALSE, EL USUARIO NO TIENE PERMISO PARA VER LAS FICHAS PRIVADAS

            if (FiltroUsuario == "0")
                sql = string.Format("select DISTINCT itemtrabajador.contrato, itemTrabajador.rut, " +
                    " itemTrabajador.anomes from itemtrabajador INNER JOIN trabajador ON " +
                    " trabajador.contrato = itemtrabajador.contrato AND itemTrabajador.anomes = trabajador.anomes" +
                    " WHERE itemTrabajador.anomes={0} AND coditem='{1}' " + (ShowPrivados==false? " AND privado=0":""), Periodo, pItem);
            else
            {
                condicion = Conjunto.GetCondicionFromCode(FiltroUsuario);
                sql = string.Format("select DISTINCT itemtrabajador.contrato, itemTrabajador.rut, " +
                    " itemTrabajador.anomes from itemtrabajador INNER JOIN trabajador ON " +
                    " trabajador.contrato = itemtrabajador.contrato AND itemTrabajador.anomes = trabajador.anomes" +
                    " WHERE coditem='{0}' AND itemTrabajador.anomes={1} AND {2} " + (ShowPrivados==false? " AND privado=0": ""), pItem, Periodo, PreparaCondicion(condicion));
            }                

            List<Employe> contratos = new List<Employe>();
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        //cmd.Parameters.Add(new SqlParameter("@periodo", Periodo));
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //AGREGAMOS CONTRATOS A LA LISTA
                                contratos.Add(new Employe() { contrato = (string)rd["contrato"], rut = (string)rd["rut"], anomes = (int)rd["anomes"] });
                            }
                        }
                    }
                    cmd.Dispose();
                    rd.Close();
                    fnSistema.sqlConn.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return contratos;
        }

        //OBTENER LISTADO CONTRATOS Y RUT DE ACUERDO A CONJUNTO SELECCIONADO
        private List<Employe> ListadoConjunto(string condicion, int periodo)
        {
            List<Employe> empleados = new List<Employe>();
            string sql = "", condUser = "";

            if (FiltroUsuario == "0")
                sql = string.Format("SELECT contrato, rut, anomes FROM trabajador WHERE {0} " +
                    "AND anomes={1} " + (ShowPrivados==false? " AND privado=0":""), condicion, periodo);
            else
            {
                condUser = Conjunto.GetCondicionFromCode(FiltroUsuario);
                sql = string.Format("SELECT contrato, rut, anomes FROM trabajador WHERE {0} " +
                    "AND anomes={1} AND {2} " + (ShowPrivados==false? " AND privado=0":""), condicion, periodo, condUser);
            }
                

            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //AGREGAMOS DATOS A LISTA
                                empleados.Add(new Employe()
                                {
                                    contrato = (string)rd["contrato"],
                                    rut = (string)rd["rut"],
                                    anomes = (int)rd["anomes"]
                                });
                            }
                        }
                    }
                    cmd.Dispose();
                    rd.Close();
                    fnSistema.sqlConn.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return empleados;
        }

        //ELIMINAR ITEM TRABAJADOR 
        //ELIMINAR TODOS
        private bool ElimiminarTodos(LookUpEdit pItem, TextEdit pPeriodo)
        {
            string sql = "";

            if (FiltroUsuario == "0")
                sql = string.Format("DELETE itemtrabajador " +
                                    "FROM itemTrabajador " +
                                    "INNER JOIN trabajador " +
                                    "ON trabajador.contrato = itemtrabajador.contrato AND trabajador.anomes = itemtrabajador.anomes " +
                                    "WHERE coditem = '{0}' AND itemtrabajador.anomes = {1} AND esclase=0 " + (ShowPrivados==false?" AND privado=0":""),
                                    pItem.Text, Convert.ToInt32(pPeriodo.Text));
            else
                sql = string.Format("DELETE itemtrabajador " +
                                    "FROM itemTrabajador  " +
                                    "INNER JOIN trabajador " +
                                    "ON trabajador.contrato = itemtrabajador.contrato AND trabajador.anomes = itemtrabajador.anomes " +
                                    "WHERE {0} AND coditem = '{1}' AND itemtrabajador.anomes = {2} AND esclase=0 " + (ShowPrivados == false ? " AND privado=0" : ""),
                    PreparaCondicion(Conjunto.GetCondicionFromCode(FiltroUsuario)), pItem.Text, Convert.ToInt32(pPeriodo.Text));
            

            SqlCommand cmd;
            SqlTransaction tran;
            bool transaccionCorrecta = false;
            int count = 0;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    //COMENZAMOS LA TRANSACCION
                    tran = fnSistema.sqlConn.BeginTransaction();
                    try
                    {
                        using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                        {
                            //PARAMETROS
                            //cmd.Parameters.Add(new SqlParameter("@item", pItem.Text));
                            //cmd.Parameters.Add(new SqlParameter("@periodo", Convert.ToInt32(pPeriodo.Text)));

                            //AGREGAMOS PROCESO A TRANSACCION
                            cmd.Transaction = tran;
                            count =  cmd.ExecuteNonQuery();

                            //SI TODO SALIO BIEN HACEMOS COMMIT
                            tran.Commit();

                            if (count > 0)
                                transaccionCorrecta = true;

                            //LIBERAMOS RECURSOS...
                            cmd.Dispose();
                            fnSistema.sqlConn.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        //EN CASO DE ERROR DESHACEMOS CAMBIOS
                        tran.Rollback();
                        transaccionCorrecta = false;
                    }                    
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return transaccionCorrecta;
        }

        //ELIMINAMOS DE ACUERDO A CONJUNTO SELECCIONADO
        private bool EliminarConjunto(LookUpEdit pitem, TextEdit pPeriodo, TextEdit pCodConjunto, List<Employe> pLista)
        {
            bool transaccionCorrecta = false;
            string sqlDelete = "DELETE FROM ITEMTRABAJADOR WHERE coditem=@item " +
                                "AND anomes=@periodo AND contrato=@contrato AND esclase=0";
            SqlCommand cmd;
            SqlTransaction tran;
            string condicion = "";
            List<Employe> Empleados = new List<Employe>();

            //OBTENER LA CONDICION DEL CONJUNTO A EVALUAR
            condicion = CondicionConjunto(pCodConjunto.Text);

            //OBTENER LISTADO DE TRABAJADORES DE ACUERDO A CONDICION
            //Empleados = ListadoConjunto(condicion, Convert.ToInt32(pPeriodo.Text));

            if (pLista.Count>0)
            {
                //ABRIMOS LA CONEXION
                try
                {
                    if (fnSistema.ConectarSQLServer())
                    {
                        tran = fnSistema.sqlConn.BeginTransaction();
                        //RECORRES LISTADO
                        foreach (var persona in pLista)
                        {
                            try
                            {
                                using (cmd = new SqlCommand(sqlDelete, fnSistema.sqlConn))
                                {
                                    //PARAMETROS
                                    cmd.Parameters.Add(new SqlParameter("@periodo", Convert.ToInt32(pPeriodo.Text)));
                                    cmd.Parameters.Add(new SqlParameter("@contrato", persona.contrato));
                                    cmd.Parameters.Add(new SqlParameter("@item", pitem.Text));

                                    //AGREGAMOS A TRANSACCION
                                    cmd.Transaction = tran;

                                    cmd.ExecuteNonQuery();

                                    cmd.Parameters.Clear();                                    
                                }
                            }
                            catch (Exception ex)
                            {
                                XtraMessageBox.Show(ex.Message);
                                //SI OCURRE UN ERROR HACEMOS ROLLBACK
                                tran.Rollback();
                                transaccionCorrecta = false;
                            }
                        }
                        //SI TERMINO DE ITERAR SIN PROBLEMAS HACEMOS COMMIT
                        tran.Commit();
                        fnSistema.sqlConn.Close();
                        transaccionCorrecta = true;
                    }
                }
                catch (SqlException ex)
                {
                    XtraMessageBox.Show(ex.Message);
                }
            }

            return transaccionCorrecta;
        }

        //VERIFICAR SI EL PERIODO TIENE REGISTROS 
        private bool PeriodoTieneRegistros(int periodo)
        {
            string sql = "SELECT contrato FROM trabajador where anomes=@periodo";
            SqlCommand cmd;
            SqlDataReader rd;
            bool tiene = false;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@periodo", periodo));
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //SI HAY FILAS ES PORQUE TIENE REGISTROS
                            tiene = true;
                        }
                        else
                        {
                            tiene = false;
                        }
                    }
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                    rd.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return tiene;
        }

        //VALIDAR QUE EL CODIGO DE CONJUNTO INGRESADO EXISTA EN BD
        private bool CodeConjuntoExiste(string code)
        {
            bool existe = false;
            string sql = "SELECT codigo FROM conjunto WHERE codigo=@code";
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@code", code));
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //EXISTE CODIGO
                            existe = true;
                        }
                        else
                        {
                            existe = false;
                        }
                    }
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                    rd.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return existe;
        }

        //MANEJO DE TECLA TAB
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Tab)
            {
                if (txtPeriodo.ContainsFocus)
                {
                    /*if (cbPeriodo.Checked == false)
                    {
                        if (txtPeriodo.Text == "")
                        { lblMensaje.Visible = true;lblMensaje.Text = "Por favor ingresa un periodo";return false; }

                        //VERIFICAR QUE EL PERIODO TIENE REGISTROS
                        if (PeriodoTieneRegistros(Convert.ToInt32(txtPeriodo.Text)) == false)
                        { lblMensaje.Visible = true; lblMensaje.Text = "Parece ser que el periodo no registra datos";return false; }

                        lblMensaje.Visible = false;
                    }        */          
                }
                if (txtConjunto.ContainsFocus)
                {
                    if (cbTodos.Checked == false)
                    {
                        if (txtConjunto.Text == "")
                        { lblMensaje.Visible = true; lblMensaje.Text = "Por favor ingresa o selecciona un conjunto a evaluar";return false; }

                        //VERIFICAR SI EL CONJUNTO INGRESADO ES VALIDO
                        if (CodeConjuntoExiste(txtConjunto.Text) == false)
                        { lblMensaje.Visible = true; lblMensaje.Text = "El codigo de conjunto ingresado no existe";return false; }

                        lblMensaje.Visible = false;
                    }
                }
            }

            return base.ProcessDialogKey(keyData);
        }

        //VER SI UN CONTRATO TIENE MAS DE UNA VEZ UN ITEM (REPETIDO)
        private bool ItemRepetido(string item, int periodo)
        {
            if (item != "" && periodo != 0)
            {
                //CONTAMOS CUANTAS VECES ESTA UN DETERMINADO ITEM AGRUPADO POR CONTRATO
                string sql = "";
                if(FiltroUsuario == "0")
                    sql = string.Format("SELECT count(itemTrabajador.contrato) as contar FROM itemtrabajador " +
                      " INNER JOIN trabajador ON trabajador.contrato = itemTrabajador.contrato AND " +
                      " trabajador.anomes = itemTrabajador.anomes " +
                      " WHERE coditem='{0}' AND itemtrabajador.anomes={1} AND esclase=0 " + (ShowPrivados==false? " AND privado=0":"") +
                      " group by itemTrabajador.contrato", item, periodo);
                else
                    sql = string.Format("SELECT count(itemTrabajador.contrato) as contar FROM itemtrabajador " +
                      " INNER JOIN trabajador ON trabajador.contrato = itemTrabajador.contrato AND " +
                      " trabajador.anomes = itemTrabajador.anomes " +
                      " WHERE coditem='{0}' AND itemtrabajador.anomes={1} AND esclase=0 AND {2} " + (ShowPrivados==false? " AND privado=0":"") +
                      " group by itemTrabajador.contrato", item, periodo, PreparaCondicion(Conjunto.GetCondicionFromCode(FiltroUsuario)));

                SqlCommand cmd;
                SqlDataReader rd;
                bool existe = false;

                try
                {
                    if (fnSistema.ConectarSQLServer())
                    {
                        using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                        {
                            rd = cmd.ExecuteReader();
                            if (rd.HasRows)
                            {
                                //RECORREMOS FILAS
                                while (rd.Read())
                                {
                                    int number = Convert.ToInt32(rd["contar"]);
                                    if (number > 1) existe = true;
                                }

                                if (existe) return true;
                                else
                                    return false;
                            }  
                        }
                    }
                }
                catch (SqlException ex)
                {
                    XtraMessageBox.Show(ex.Message);
                }
            }

            return false;
        }

        //METODO PARA ELIMINAR REGISTROS A TRAVÉS DE SU ID(PARA EL CASO DE SELECCION EN ITEMS REPETIDOS)
        private bool EliminarItemsSeleccionados(List<ItemTrabajador> registros, int periodo)
        {
            bool transactionCorrecta = false;
            if (registros.Count>0)
            {
                //string sql = "DELETE FROM ITEMTRABAJADOR WHERE id=@pId and anomes=@pPeriodo";
                string sql = "DELETE FROM ITEMTRABAJADOR WHERE contrato=@pContrato AND coditem=@pCoditem " +
                    "AND anomes=@pPeriodo AND numitem=@pNum AND esclase=0";
                SqlCommand cmd;
                SqlTransaction tr;
                try
                {
                    if (fnSistema.ConectarSQLServer())
                    {
                        tr = fnSistema.sqlConn.BeginTransaction();
                        //RECORREMOS LISTA
                        foreach (var item in registros)
                        {
                            try
                            {
                                using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                                {
                                    //PARAMETROS
                                    cmd.Parameters.Add(new SqlParameter("@pContrato", item.contrato));
                                    cmd.Parameters.Add(new SqlParameter("@pPeriodo", periodo));
                                    cmd.Parameters.Add(new SqlParameter("@pCoditem", item.item));
                                    cmd.Parameters.Add(new SqlParameter("@pNum", item.NumeroItem));

                                    //AGREGAMOS A TRANSACCION
                                    cmd.Transaction = tr;
                                    //EJECUTAMOS...
                                    cmd.ExecuteNonQuery();

                                    cmd.Parameters.Clear();
                                }
                            }
                            catch (Exception ex)
                            {
                                //SI OCURRE UN ERROR HACEMOS ROLBACK
                                tr.Rollback();                               
                                return false;
                            }                           
                        }

                        //SI LLEGA ACA ES PORQUE NO HUBO EXCEPCIONES...
                        tr.Commit();
                        transactionCorrecta = true;
                        fnSistema.sqlConn.Close();                    
                        
                    }
                }
                catch (SqlException ex)
                {
                    XtraMessageBox.Show(ex.Message);
                }
            }
            else
            {
                XtraMessageBox.Show("No se encontraron registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return transactionCorrecta;
        }

        #endregion

        private void txtConjunto_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            DXPopupMenu p = e.Menu;
            if (p != null)
            {
                p.Items.Clear();

                DXMenuItem menu = new DXMenuItem("Agregar conjunto", new EventHandler(AgregarConjunto_Click));
                menu.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("images/actions/show_16x16.png");

                p.Items.Add(menu);
            }
        }

        private void AgregarConjunto_Click(object sender, EventArgs e)
        {
            FrmFiltroBusqueda filtro = new FrmFiltroBusqueda(true);
            filtro.opener = this;
            filtro.ShowDialog();
        }

        private void txtPeriodo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar) || char.IsSeparator(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void txtConjunto_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar) || char.IsSeparator(e.KeyChar) || char.IsLetter(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void cbPeriodo_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void cbTodos_CheckedChanged(object sender, EventArgs e)
        {
            if (cbTodos.Checked)
            {
                txtConjunto.Enabled = false;
                txtConjunto.Text = "";
                btnConjunto.Enabled = false;
            }
            else
            {
                txtConjunto.Enabled = true;
                txtConjunto.Text = "";
                txtConjunto.Focus();
                btnConjunto.Enabled = true;
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            // if (txtPeriodo.Text == "" && cbPeriodo.Checked == false)
            //{ XtraMessageBox.Show("Por favor ingresa un periodo a evaluar", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

            if (txtConjunto.Text == "" && cbTodos.Checked == false)
            { XtraMessageBox.Show("Por favor ingresa conjunto a evaluar", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);return; }

            List<Employe> listado = new List<Employe>();
            string condicion = "";
            bool tranCorrecta = false;
            if (cbTodos.Checked)
            {
                //REALIZAMOS OPERACION PARA TODOS LOS REGISTROS DEL PERIODO EN EVALUACION
                listado = TodosContratos(Convert.ToInt32(txtPeriodo.Text), txtItem.Text);
                if (listado.Count == 0)
                { XtraMessageBox.Show("No se encontraron registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                //VER SI ALGUN TRABAJADOR TIENE REPETIDO EL ITEM...
                if (ItemRepetido(txtItem.Text, Convert.ToInt32(txtPeriodo.Text)))
                {
                    DialogResult seleccion = XtraMessageBox.Show("Hemos detectado que algunos trabajadores tienen mas de una vez este item, ¿Desea seleccionar item?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (seleccion == DialogResult.Yes)
                    {
                        //ABRIMOS FORMULARIO SELECCION...
                        frmSeleccionItemElimina frmsel = new frmSeleccionItemElimina(txtItem.Text, "elimina", "");
                        frmsel.StartPosition = FormStartPosition.CenterParent;
                        frmsel.opener = this;
                        frmsel.ShowDialog();

                        if (ListaSeleccionItems.Count == 0)
                        { XtraMessageBox.Show("No se selecionaron items", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                        //ELIMINAMOS...
                        DialogResult pre = XtraMessageBox.Show("¿Está seguro de eliminar item seleccionado?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (pre == DialogResult.Yes)
                        {
                            if (EliminarItemsSeleccionados(ListaSeleccionItems, Convert.ToInt32(txtPeriodo.Text)))
                            {
                                XtraMessageBox.Show($"Item {txtItem.Text} eliminado correctamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                //GUARDAMOS EN EL LOG
                                logRegistro log = new logRegistro(User.getUser(), "SE ELIMINA ITEM " + txtItem.Text + " PARA TODOS LOS REGISTROS DEL PERIODO " + txtPeriodo.Text, "ITEMTRABAJADOR", txtItem.Text, "0", "ELIMINAR");
                                log.Log();

                                return;
                            }
                            else
                            {
                                XtraMessageBox.Show("Error al intentar eliminar", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                return;
                            }
                        }
                        else
                            return;
                    }
                    else
                        return;

                }               

                //SI HAY REGISTROS REALIZAMOS DELETE 
                DialogResult Advertencia = XtraMessageBox.Show("¿Está seguro de eliminar el item " + txtItem.Text + " de todos los trabajadores del periodo " + txtPeriodo.Text + "?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (Advertencia == DialogResult.Yes)
                {
                    //ELIMINAMOS
                    tranCorrecta = ElimiminarTodos(txtItem, txtPeriodo);
                    if (tranCorrecta)
                    {
                        XtraMessageBox.Show("Item eliminado correctamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        //GUARDAMOS REGISTRO EN LOG
                        logRegistro log = new logRegistro(User.getUser(), "SE ELIMINA ITEM " + txtItem.Text+ " PARA TODOS LOS REGISTROS DEL PERIODO " + txtPeriodo.Text, "ITEMTRABAJADOR", txtItem.Text, "0", "ELIMINAR");
                        log.Log();

                        lblMensaje.Visible = false;
                    }
                    else
                    {
                        XtraMessageBox.Show("Error al intentar eliminar item", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
            }
            else
            {
                condicion = CondicionConjunto(txtConjunto.Text);
                if (condicion == "")
                { XtraMessageBox.Show("Conjunto no existe", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);txtConjunto.Focus(); return; }

                //BUSCAMOS PARA EL CONJUNTO SELECCIONADO
                listado = ListadoConjunto(condicion, Convert.ToInt32(txtPeriodo.Text));
                if (listado.Count == 0)
                { XtraMessageBox.Show("No se encontraron registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);return; }

                //VERIFICAR SI HAY ITEMS REPETIDOS
                if (ItemRepetido(txtItem.Text, Convert.ToInt32(txtPeriodo.Text)))
                {
                    DialogResult seleccion = XtraMessageBox.Show("Hemos detectado que algunos trabajadores tienen mas de una vez este item, ¿Desea seleccionar item?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (seleccion == DialogResult.Yes)
                    {
                        //ABRIMOS FORMULARIO SELECCION...
                        frmSeleccionItemElimina frmsel = new frmSeleccionItemElimina(txtItem.Text, "elimina", txtConjunto.Text);
                        frmsel.StartPosition = FormStartPosition.CenterParent;
                        frmsel.opener = this;
                        frmsel.ShowDialog();

                        if (ListaSeleccionItems.Count == 0)
                        { XtraMessageBox.Show("No se selecionaron items", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                        //ELIMINAMOS...
                        DialogResult pre = XtraMessageBox.Show("¿Esta seguro eliminar item selecionado?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (pre == DialogResult.Yes)
                        {
                            if (EliminarItemsSeleccionados(ListaSeleccionItems, Convert.ToInt32(txtPeriodo.Text)))
                            {
                                XtraMessageBox.Show($"Item {txtItem.Text} eliminado correctamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                //GUARDAMOS EN EL LOG
                                logRegistro log = new logRegistro(User.getUser(), "SE ELIMINA ITEM " + txtItem.Text + " DE FORMA MASIVA PERIODO " + txtPeriodo.Text, "ITEMTRABAJADOR", txtItem.Text, "0", "ELIMINAR");
                                log.Log();

                                return;
                            }
                            else
                            {
                                XtraMessageBox.Show("Error al intentar eliminar", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                return;
                            }
                        }
                        else
                            return;
                       
                    }
                    else
                        return;
                }

                //ELIMINAMOS PARA EL CONJUNTO
                //SI HAY REGISTROS REALIZAMOS DELETE 
                DialogResult Advertencia = XtraMessageBox.Show("¿Está seguro de eliminar el item " + txtItem.Text + "?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (Advertencia == DialogResult.Yes)
                {
                    //ELIMINAMOS
                    tranCorrecta = EliminarConjunto(txtItem, txtPeriodo, txtConjunto, listado);

                    if (tranCorrecta)
                    {
                        XtraMessageBox.Show("Item eliminado correctamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        //GUARDAMOS EVENTO EN LOG
                        logRegistro log = new logRegistro(User.getUser(), "SE ELIMINA ITEM " + txtItem.Text + " DE FORMA MASIVA PERIODO " +txtPeriodo.Text, "ITEMTRABAJADOR", txtItem.Text, "0", "ELIMINAR");
                        log.Log();

                        lblMensaje.Visible = false;
                    }
                    else
                    {
                        XtraMessageBox.Show("Error al intentar eliminar", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
            }
        }

        private void txtItem_EditValueChanged(object sender, EventArgs e)
        {
            if (txtItem.Properties.DataSource != null)
            {
                Hashtable info = new Hashtable();
                info = DataItem(txtItem.Text);

                txtDescripcion.Text = (string)info["descripcion"];
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            Close();
        }

        private void txtConjunto_DoubleClick(object sender, EventArgs e)
        {
            FrmFiltroBusqueda filtro = new FrmFiltroBusqueda(true);
            filtro.opener = this;
            filtro.ShowDialog();
        }

        private void txtDescripcion_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
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