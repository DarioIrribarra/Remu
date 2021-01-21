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
using System.Runtime.InteropServices;
using System.Data.SqlClient;
using DevExpress.Utils.Menu;

namespace Labour
{
    public partial class FrmModificarInfoEmpleado : DevExpress.XtraEditors.XtraForm, IConjuntosCondicionales
    {
        [DllImport("user32")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32")]
        static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        const int MF_BYCOMMAND = 0;
        const int MF_DISABLED = 2;
        const int SC_CLOSED = 0xF060;

        #region "CONUNTOS CONDICIONALES"
        public void CargarCodigoConjunto(string code)
        {
            txtConjunto.Text = code;
        }
        #endregion

        //PARA GUARDAR EL FILTRO DEL USUARIO LOGUEADO
        private string FiltroUsuario { get; set; } = User.GetUserFilter();

        //USUARIO PUEDE VER FICHAS PRIVADAS
        private bool ShowPrivadas { get; set; } = User.ShowPrivadas();

        public FrmModificarInfoEmpleado()
        {
            InitializeComponent();
        }

        private void FrmModificarInfoEmpleado_Load(object sender, EventArgs e)
        {
            var sm = GetSystemMenu(Handle, false);
            EnableMenuItem(sm, SC_CLOSED, MF_BYCOMMAND | MF_DISABLED);

            txtPeriodo.Text = Calculo.PeriodoObservado.ToString();
            txtNombrePeriodo.Text = fnSistema.PrimerMayuscula(fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(Calculo.PeriodoObservado)));
            CargarCombo();
            btnConjunto.Enabled = false;
        }

        #region "MANEJO DE DATOS"
        //LISTADO DE CAMPOS 
        private List<formula> ListadoCampos()
        {
            List<formula> lista = new List<formula>();
            lista.Add(new formula() { key = "area", desc = "AREA" });
            lista.Add(new formula() { key = "ccosto", desc = "CENTRO COSTO" });
            lista.Add(new formula() { key = "nacion", desc = "NACIONALIDAD" });
            lista.Add(new formula() { key = "ecivil", desc = "ESTADO CIVIL" });
            lista.Add(new formula() { key = "cargo", desc = "CARGO" });
            lista.Add(new formula() { key = "tipocontrato", desc = "TIPO CONTRATO" });
            lista.Add(new formula() { key = "jubilado", desc = "JUBILADO" });
            lista.Add(new formula() { key = "regimensalario", desc = "REGIMEN SALARIO" });
            lista.Add(new formula() { key = "regimen", desc = "REGIMEN" });
            lista.Add(new formula() { key = "afp", desc = "AFP" });
            lista.Add(new formula() { key = "salud", desc = "SALUD" });
            lista.Add(new formula() { key = "tramo", desc = "TRAMO" });
            lista.Add(new formula() { key = "formapago", desc = "FORMA PAGO" });
            lista.Add(new formula() { key = "banco", desc = "BANCO" });
            lista.Add(new formula() { key = "tipocuenta", desc = "TIPO CUENTA" });
            lista.Add(new formula() { key = "causal", desc = "CAUSAL" });
            lista.Add(new formula() { key = "privado", desc = "PRIVADO"});
            lista.Add(new formula() { key = "esco", desc = "ESCOLARIDAD"});
            lista.Add(new formula() { key = "horario", desc = "HORARIO"});
            lista.Add(new formula() { key = "jornada", desc = "JORNADA" });
            lista.Add(new formula() { key = "sindicato", desc = "SINDICATO"});
            lista.Add(new formula() { key = "comuna", desc = "COMUNA" });
            lista.Add(new formula() { key = "suslab", desc = "SUSPENSION LABORAL" });

            return lista;
        }

        //CARGAR COMBOBOX
        private void CargarCombo()
        {
            List<formula> lista = new List<formula>();
            lista = ListadoCampos();
            if (lista.Count > 0)
            {
                //PROPIEDADES COMBOBOX
                txtCampo.Properties.DataSource = lista.ToList();
                txtCampo.Properties.ValueMember = "key";
                txtCampo.Properties.DisplayMember = "desc";

                txtCampo.Properties.PopulateColumns();                
                txtCampo.Properties.Columns[0].Visible = false;
              
                txtCampo.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
                txtCampo.Properties.AutoSearchColumnIndex = 1;
                txtCampo.Properties.ShowHeader = false;

                txtCampo.ItemIndex = 0;
            }
        }

        //PARA CARGAR COMBO DE ACUERDO A SELECCION
        private void CargaData(string seleccion)
        {
            switch (seleccion)
            {
                case "AREA":
                    datoCombobox.spllenaComboBox("SELECT id, nombre FROM area", txtCambio, "id", "nombre", true);
                    txtCambio.ItemIndex = 0;
                    break;
                case "CENTRO COSTO":
                    datoCombobox.spllenaComboBox("SELECT id, nombre FROM ccosto", txtCambio, "id", "nombre", true);
                    txtCambio.ItemIndex = 0;
                    break;
                case "NACIONALIDAD":
                    datoCombobox.spllenaComboBox("SELECT id, nombre FROM nacion", txtCambio, "id", "nombre", true);
                    txtCambio.ItemIndex = 0;
                    break;
                case "ESTADO CIVIL":
                    datoCombobox.spllenaComboBox("SELECT id, nombre FROM ecivil", txtCambio, "id", "nombre", true);
                    txtCambio.ItemIndex = 0;
                    break;
                case "CARGO":
                    datoCombobox.spllenaComboBox("SELECT id, nombre FROM cargo", txtCambio, "id", "nombre", true);
                    txtCambio.ItemIndex = 0;
                    break;
                case "TIPO CONTRATO":
                    fnComboTipoContrato(txtCambio);
                    txtCambio.ItemIndex = 0;
                    break;
                case "JUBILADO":
                    fnComboJubilado(txtCambio);
                    txtCambio.ItemIndex = 0;
                    break;
                case "REGIMEN SALARIO":
                    fnComboRegimenSalario(txtCambio);
                    txtCambio.ItemIndex = 0;
                    break;
                case "REGIMEN":
                    datoCombobox.spllenaComboBox("SELECT id, nombre FROM regimen", txtCambio, "id", "nombre", true);
                    txtCambio.ItemIndex = 0;
                    break;
                case "AFP":
                    datoCombobox.spllenaComboBox("SELECT id, nombre FROM afp", txtCambio, "id", "nombre", true);
                    txtCambio.ItemIndex = 0;
                    break;
                case "SALUD":
                    datoCombobox.spllenaComboBox("SELECT id, nombre FROM isapre", txtCambio, "id", "nombre", true);
                    txtCambio.ItemIndex = 0;
                    break;
                case "TRAMO":
                    fnComboTramo(txtCambio);
                    txtCambio.ItemIndex = 0;
                    break;
                case "FORMA PAGO":
                    datoCombobox.spllenaComboBox("SELECT id, nombre FROM formapago", txtCambio, "id", "nombre", true);
                    txtCambio.ItemIndex = 0;
                    break;
                case "BANCO":
                    datoCombobox.spllenaComboBox("SELECT id, nombre FROM banco", txtCambio, "id", "nombre", true);
                    txtCambio.ItemIndex = 0;
                    break;
                case "TIPO CUENTA":
                    datoCombobox.spllenaComboBox("SELECT id, nombre FROM tipocuenta", txtCambio, "id", "nombre", true);
                    txtCambio.ItemIndex = 0;
                    break;
                case "CAUSAL":
                    datoCombobox.spllenaComboBox("SELECT codCausal, descCausal FROM causaltermino ORDER BY desccausal", txtCambio, "codCausal", "descCausal", true);
                    txtCambio.ItemIndex = 0;
                    break;
                case "PRIVADO":
                    fnComboPrivado(txtCambio);
                    txtCambio.ItemIndex = 0;
                    break;

                case "ESCOLARIDAD":
                    datoCombobox.spllenaComboBox("SELECT codesc, descesc FROM escolaridad ORDER BY descesc", txtCambio, "codesc", "descesc", true);
                    break;
                case "HORARIO":
                    datoCombobox.spllenaComboBox("SELECT id, deschor FROM horario ORDER BY deschor", txtCambio, "id", "deschor", true);
                    break;

                case "JORNADA":
                    fnSistema.fnComboJornada(txtCambio);
                    break;

                case "SINDICATO":
                    datoCombobox.spllenaComboBox("SELECT id, descSin FROM sindicato ORDER BY descSin", txtCambio, "id", "descSin", true);
                    break;

                case "COMUNA":
                    datoCombobox.spllenaComboBox("SELECT codComuna, descComuna FROM comuna ORDER BY region, descComuna", txtCambio, "codComuna", "descComuna", true);
                    break;

                case "SUSPENSION LABORAL":
                    fnSistema.fnComboSupensionLaboral(txtCambio);
                    break;

                default:
                    break;
            }
        }

        //METODO PARA CARGAR COMBO JUBILADO
        //TIENE LOS SIGUIENTES VALORES:
        // 0 --> NO
        /* 1 --> SI, NO COTIZA
         * 2 --> SI, COTIZA 
         */
        private void fnComboJubilado(LookUpEdit pCombo)
        {
            List<PruebaCombo> lista = new List<PruebaCombo>();
            Int16 i = 0;
            while (i <= 2)
            {
                //agregamos objetos a la lista
                if (i == 0) lista.Add(new PruebaCombo() { key = i, desc = "No" });
                if (i == 1) lista.Add(new PruebaCombo() { key = i, desc = "Si, No Cotiza" });
                if (i == 2) lista.Add(new PruebaCombo() { key = i, desc = "Si, Cotiza" });
                i++;
            }

            //PROPIEDADES COMBOBOX
            pCombo.Properties.DataSource = lista.ToList();
            pCombo.Properties.ValueMember = "key";
            pCombo.Properties.DisplayMember = "desc";

            pCombo.Properties.PopulateColumns();

            //ocultamos la columna key
            pCombo.Properties.Columns[0].Visible = false;

            pCombo.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
            pCombo.Properties.AutoSearchColumnIndex = 1;
            pCombo.Properties.ShowHeader = false;
        }

        //METODO PARA CARGAR COMBO REGIMEN SALARIO
        //TIENE DOS VALORES 
        /*
         * 0 --> VARIABLE
         * 1 --> FIJO
         */
        private void fnComboRegimenSalario(LookUpEdit pCombo)
        {
            List<datoCombobox> lista = new List<datoCombobox>();
            int i = 0;
            while (i <= 1)
            {
                //agregamos valores a la lista
                if (i == 0) lista.Add(new datoCombobox() { KeyInfo = i, descInfo = "Variable" });
                if (i == 1) lista.Add(new datoCombobox() { KeyInfo = i, descInfo = "Fijo" });
                i++;
            }
            //PROPIEDADES COMBOBOX
            pCombo.Properties.DataSource = lista.ToList();
            pCombo.Properties.ValueMember = "KeyInfo";
            pCombo.Properties.DisplayMember = "descInfo";

            pCombo.Properties.PopulateColumns();

            //ocultamos la columan key
            pCombo.Properties.Columns[0].Visible = false;

            pCombo.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
            pCombo.Properties.AutoSearchColumnIndex = 1;
            pCombo.Properties.ShowHeader = false;
        }

        //METODO PARA CARGAR COMBO TIPO CONTRATO
        //TIENE LOS SIGUIENTES VALORES
        /*
         * 0 --> INDEFINIDO
         * 1 --> PLAZO FIJO
         * 2 --> OBRA O FAENA
         */
        private void fnComboTipoContrato(LookUpEdit pCombo)
        {
            List<datoCombobox> lista = new List<datoCombobox>();
            int i = 0;
            while (i <= 2)
            {
                if (i == 0) lista.Add(new datoCombobox() { KeyInfo = i, descInfo = "INDEFINIDO" });
                if (i == 1) lista.Add(new datoCombobox() { KeyInfo = i, descInfo = "PLAZO FIJO" });
                if (i == 2) lista.Add(new datoCombobox() { KeyInfo = i, descInfo = "OBRA O FAENA" });
                i++;
            }
            //PROPIEDADES COMBOBOX
            pCombo.Properties.DataSource = lista.ToList();
            pCombo.Properties.ValueMember = "KeyInfo";
            pCombo.Properties.DisplayMember = "descInfo";

            pCombo.Properties.PopulateColumns();

            //ocultamos la columan key
            pCombo.Properties.Columns[0].Visible = false;

            pCombo.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
            pCombo.Properties.AutoSearchColumnIndex = 1;
            pCombo.Properties.ShowHeader = false;

        }

        //METODO PARA CARGAR COMBO tramo
        //TIENE CUATRO VALORES 1, 2, 3, 4
        private void fnComboTramo(LookUpEdit pCombo)
        {
            //instanciamos a la clase combo
            List<datoCombobox> lista = new List<datoCombobox>();
            int i = 1;
            while (i <= 4)
            {
                lista.Add(new datoCombobox() { descInfo = "" + i, KeyInfo = i });
                i++;
            }
            //PROPIEDADES COMBOBOX
            pCombo.Properties.DataSource = lista.ToList();
            pCombo.Properties.ValueMember = "KeyInfo";
            pCombo.Properties.DisplayMember = "descInfo";

            pCombo.Properties.PopulateColumns();

            //ocultamos la columan key
            pCombo.Properties.Columns[0].Visible = false;

            pCombo.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
            pCombo.Properties.AutoSearchColumnIndex = 1;
            pCombo.Properties.ShowHeader = false;
        }

        //COMBO PRIVADO
        //0 --> NO ES PRIVADO
        //1 --> ES PRIVADO
        private void fnComboPrivado(LookUpEdit pCombo)
        {
            //instanciamos a la clase combo
            List<datoCombobox> lista = new List<datoCombobox>();
           
             lista.Add(new datoCombobox() { KeyInfo = 1, descInfo = "SI"});
            lista.Add(new datoCombobox() { KeyInfo = 0, descInfo = "NO" });

            //PROPIEDADES COMBOBOX
            pCombo.Properties.DataSource = lista.ToList();
            pCombo.Properties.ValueMember = "KeyInfo";
            pCombo.Properties.DisplayMember = "descInfo";

            pCombo.Properties.PopulateColumns();

            //ocultamos la columan key
            pCombo.Properties.Columns[0].Visible = false;

            pCombo.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
            pCombo.Properties.AutoSearchColumnIndex = 1;
            pCombo.Properties.ShowHeader = false;
        }

        //COMBO CAUSAL
        private void fnComboCausal(LookUpEdit pCombobox, bool ocultarkey)
        {
            List<Causal> lista = new List<Causal>();

            string sql = "SELECT codCausal, descCausal FROM causaltermino";
            SqlCommand cmd;
            SqlDataReader re;

            if (fnSistema.ConectarSQLServer())
            {
                cmd = new SqlCommand(sql, fnSistema.sqlConn);
                re = cmd.ExecuteReader();

                if (re.HasRows)
                {
                    //int i = 0;
                    while (re.Read())
                    {
                        lista.Add(new Causal() { Codigo = (int)re["codCausal"], Descripcion = (string)re["descCausal"] });
                    }
                }
                cmd.Dispose();
                re.Close();
                fnSistema.sqlConn.Close();
            }


            //PROPIEDADES COMBOBOX
            pCombobox.Properties.DataSource = lista.ToList();
            pCombobox.Properties.ValueMember = "id";
            pCombobox.Properties.DisplayMember = "desc";

            pCombobox.Properties.PopulateColumns();
            //SI OCULTAR KEY ES TRUE NO SE DEBE MOSTRAR LA COLUMNA KEY
            if (ocultarkey == true)
            {
                pCombobox.Properties.Columns[0].Visible = false;
            }
            pCombobox.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
            pCombobox.Properties.AutoSearchColumnIndex = 1;
            pCombobox.Properties.ShowHeader = false;
            pCombobox.ItemIndex = 0;

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

        //VERIFICA SI TABLA ITEM TRABAJADOR TIENE REGISTROS
        private bool PeriodoTieneItems(int periodo)
        {
            string sql = "SELECT coditem FROM itemtrabajador WHERE anomes=@periodo";
            SqlCommand cmd;
            SqlDataReader rd;
            bool existe = true;
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

        //OBTENER LISTADO CONTRATOS Y RUT DE ACUERDO A CONJUNTO SELECCIONADO
        private List<Employe> ListadoConjunto(string condicion, int periodo)
        {
            List<Employe> empleados = new List<Employe>();
            string sql = "", condUser = "";

            //SI PRIPIEDAD SHOWPRIVADOS ES FALSE, EL USUARIO NO TIENE PERMISO PARA VER FICHAS PRIVADAS

            if (FiltroUsuario == "0")
                sql = string.Format("SELECT contrato, rut, anomes FROM trabajador WHERE {0} AND anomes={1} " + (ShowPrivadas==false? " AND privado=0":""), condicion, periodo);
            else
            {
                condUser = Conjunto.GetCondicionFromCode(FiltroUsuario);
                sql = string.Format("SELECT contrato, rut, anomes FROM trabajador WHERE {0} AND anomes={1} AND {2} " + (ShowPrivadas==false? " AND privado=0":""), condicion, periodo, condUser);
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

        //TECLA TAB
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Tab)
            {
                if (txtPeriodo.ContainsFocus)
                {
                    /*if (cbPeriodo.Checked == false)
                    {
                        if (txtPeriodo.Text == "")
                        { lblMensaje.Visible = true; lblMensaje.Text = "Por favor ingresa un periodo"; return false; }

                        //VERIFICAR SI EL PERIODO REGISTRA DATOS
                        if (PeriodoTieneRegistros(Convert.ToInt32(txtPeriodo.Text)) == false)
                        { lblMensaje.Visible = true; lblMensaje.Text = "Parece ser que el periodo ingresado no tiene datos";  return false; }

                        lblMensaje.Visible = false;
                    }*/
                }
                if (txtConjunto.ContainsFocus)
                {
                    if (cbTodos.Checked == false)
                    {
                        if (txtConjunto.Text == "")
                        { lblMensaje.Visible = true; lblMensaje.Text = "Por favor ingresa o selecciona un conjunto a evaluar";return false; }

                        //VERIFICAR SI EL CODIGO INGRESADO EXISTE
                        if (CodeConjuntoExiste(txtConjunto.Text) == false)
                        { lblMensaje.Visible = true; lblMensaje.Text = "Conjunto ingresado no existe";return false; }

                        lblMensaje.Visible = false;
                    }
                }
            }

            return base.ProcessDialogKey(keyData);
        }

        //UPDATE INFORMACION PARA TODOS LOS DEL PERIODO SELECCIONADO
        private bool UpdateInformacionTodos(LookUpEdit pComboCampo, LookUpEdit pComboData, TextEdit pPeriodo, List<Employe> pListadoContratos)
        {
            string sql = "", condUser = "";

            Cursor = Cursors.WaitCursor;
            //SI PROPIEDAD SHOWPRIVADOS ES FALSE, EL USUARIO NO TIENE PERMISOS PARA VER FICHAS PRIVADAS
            if (FiltroUsuario == "0")
            {
                sql = string.Format("UPDATE trabajador SET {0}={1} WHERE anomes={2} " + (ShowPrivadas == false ? " AND privado=0" : ""),
                    pComboCampo.EditValue.ToString(), pComboData.EditValue, Convert.ToInt32(pPeriodo.Text));

                if (pComboCampo.EditValue.ToString() == "privado")
                    sql = $"UPDATE trabajador SET {pComboCampo.EditValue.ToString()}={pComboData.EditValue}  {(ShowPrivadas == false ? " WHERE privado=0":"")}";
            }                
            else
            {
                condUser = Conjunto.GetCondicionFromCode(FiltroUsuario);
                sql = string.Format("UPDATE trabajador SET {0}={1} WHERE anomes={2} AND {3} " + (ShowPrivadas==false?" AND privado=0":""),
                    pComboCampo.EditValue.ToString(), pComboData.EditValue, Convert.ToInt32(pPeriodo.Text), condUser);

                if (pComboCampo.EditValue.ToString() == "privado")
                    sql = $"UPDATE trabajador SET {pComboCampo.EditValue.ToString()}={pComboData.EditValue} WHERE {condUser} {(ShowPrivadas == false?" AND privado=0":"")}";
            }

            //SI SE CAMBIA EL CAMPO PRIVADO SE CAMBIA TODO HACIA ATRÁS

            SqlCommand cmd;
            SqlTransaction tr;
            bool transaccionCorrecta = false;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    tr = fnSistema.sqlConn.BeginTransaction();
                    try
                    {
                        using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                        {
                            //PARAMETROS
                            //cmd.Parameters.Add(new SqlParameter("@campo", (string)pComboCampo.EditValue));
                            //cmd.Parameters.Add(new SqlParameter("@data", pComboData.EditValue));
                            //cmd.Parameters.Add(new SqlParameter("@periodo", Convert.ToInt32(pPeriodo.Text)));

                            //AGREGAMOS A TRANSACION
                            cmd.Transaction = tr;
                            cmd.ExecuteNonQuery();

                            //SI NO HAY ERRORES HACEMOS ROLLBACK
                            tr.Commit();
                            cmd.Dispose();
                            fnSistema.sqlConn.Close();
                            transaccionCorrecta = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        XtraMessageBox.Show(ex.Message);
                        //SI SE PRODUCE ALGUN ERROR HACEMOS ROLLBACK
                        tr.Rollback();
                        transaccionCorrecta = false;
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }


            //if (pComboCampo.EditValue.ToString().ToLower().Equals("suslab"))
            //{

            //    if (pComboData.EditValue.ToString() == "13" || pComboData.EditValue.ToString() == "14")
            //    {
            //        if (pListadoContratos.Count > 0)
            //        {
            //            foreach (Employe item in pListadoContratos)
            //            {
            //                Calculo.UpdateItemSuspension(item.contrato, item.anomes);
            //            }
            //        }
            //    }
            //    else
            //    {
            //        if (pListadoContratos.Count > 0)
            //        {
            //            foreach (Employe item in pListadoContratos)
            //            {
            //                Calculo.UpdateItemSuspension(item.contrato, item.anomes, true);
            //            }
            //        }
            //    }
            //}
            Cursor = Cursors.Default;

            return transaccionCorrecta;
        }

        //UPDATE INFORMACION PARA CONJUNTO SELECCIONADO
        private bool UpdateInformacionConjunto(LookUpEdit pComboCampo, LookUpEdit pComboData, TextEdit pPeriodo, 
            TextEdit pConjunto, List<Employe> pListadoContratos)
        {

            Cursor = Cursors.WaitCursor;
            bool transaccionCorrecta = false;
            string sql = "",  condicion = "", Filtro = "";

            //CONDICION DE ACUERDO A CODIGO DE CONJUNTO SELECCIONADO
            condicion = CondicionConjunto(pConjunto.Text);

            //VERIRFICAR SI EL USUARIO TIENE FILTRO
            if (FiltroUsuario != "0")
            {
                Filtro = CondicionConjunto(FiltroUsuario);

                sql = $"UPDATE trabajador SET {pComboCampo.EditValue.ToString()} = {pComboData.EditValue} WHERE " +
                      $" {Filtro} AND {condicion} AND anomes={Convert.ToInt32(pPeriodo.Text)} {(ShowPrivadas == false? " AND privado=0":"")}";

                if(pComboCampo.EditValue.ToString() == "privado")
                    sql = $"UPDATE trabajador SET {pComboCampo.EditValue.ToString()} = {pComboData.EditValue} WHERE " +
                          $" {Filtro} AND {condicion} {(ShowPrivadas == false ? " AND privado=0" : "")}";
            }
            else
            {
                //NO TIENE FILTRO USUARIO
                sql = $"UPDATE trabajador SET {pComboCampo.EditValue.ToString()} = {pComboData.EditValue} WHERE " +
                      $" {condicion} AND anomes={Convert.ToInt32(pPeriodo.Text)} {(ShowPrivadas == false? " AND privado=0":"")}";

                if(pComboCampo.EditValue.ToString() == "privado")
                    sql = $"UPDATE trabajador SET {pComboCampo.EditValue.ToString()} = {pComboData.EditValue} WHERE " +
                          $" {condicion} {(ShowPrivadas == false ? " AND privado=0" : "")}";
            }

           
            SqlCommand cmd;
            SqlTransaction tr;
            int count = 0;

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        count = cmd.ExecuteNonQuery();
                        if (count > 0)
                            transaccionCorrecta = true;
                        else
                            transaccionCorrecta = false;

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }


            //if (pComboCampo.EditValue.ToString().ToLower().Equals("suslab"))
            //{

            //    if (pComboData.EditValue.ToString() == "13" || pComboData.EditValue.ToString() == "14")
            //    {
            //        if (pListadoContratos.Count > 0)
            //        {
            //            foreach (Employe item in pListadoContratos)
            //            {
            //                Calculo.UpdateItemSuspension(item.contrato, item.anomes);
            //            }
            //        }
            //    }
            //    else
            //    {
            //        if (pListadoContratos.Count > 0)
            //        {
            //            foreach (Employe item in pListadoContratos)
            //            {
            //                Calculo.UpdateItemSuspension(item.contrato, item.anomes, true);
            //            }
            //        }
            //    }
            //}


            Cursor = Cursors.Default;
            return transaccionCorrecta;
        }

        #endregion

        private void txtCampo_EditValueChanged(object sender, EventArgs e)
        {
            if (txtCampo.Properties.DataSource != null)
            {
                //CADA VEZ QUE SE CAMBIE DE ITEM CARGAMOS INFORMACION REFENTE A SELECCION
                string seleccion = "";
                seleccion = txtCampo.Text;
                CargaData(seleccion);
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
                lblMensaje.Visible = false;
                btnConjunto.Enabled = false;
            }
            else
            {
                txtConjunto.Enabled = true;
                txtConjunto.Text = "";
                txtConjunto.Focus();
                lblMensaje.Visible = false;
                btnConjunto.Enabled = true;
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (txtCampo.Properties.DataSource == null || txtCambio.Properties.DataSource == null)
            { XtraMessageBox.Show("Por favor selecciona un campo", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);return; }

            //if (txtPeriodo.Text == "" && cbPeriodo.Checked == false)
           // { XtraMessageBox.Show("Por favor ingresar un periodo", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);txtPeriodo.Focus(); return; }

            if (txtConjunto.Text == "" && cbTodos.Checked == false)
            { XtraMessageBox.Show("Por favor ingresa o selecciona un conjunto a evaluar", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);txtConjunto.Focus(); return; }

            List<Employe> lista = new List<Employe>();
            bool tranCorrecta = false;
            if (cbTodos.Checked)
            {
                //HACEMOS UPDATE PARA TODOS LOS REGISTROS DEL PERIODO SELECCIONADO
                //VERIFICAR QUE EL PERIODO TENGA REGISTROS
                if (PeriodoTieneRegistros(Convert.ToInt32(txtPeriodo.Text)) == false || PeriodoTieneItems(Convert.ToInt32(txtPeriodo.Text)) == false)
                { XtraMessageBox.Show("Periodo no registra datos", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);txtPeriodo.Focus();return; }

                string con = Calculo.GetSqlFiltro(FiltroUsuario, "", ShowPrivadas);
                con = $"SELECT contrato, anomes FROM trabajador WHERE anomes={Calculo.PeriodoObservado} " + con + " AND Status=1";
                lista = Calculo.GetListPer(con);

                DialogResult advertencia = XtraMessageBox.Show("¿Está seguro de modificar el campo " + txtCampo.Text + " con el valor " + txtCambio.EditValue.ToString() + "?", "Informacion", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (advertencia == DialogResult.Yes)
                {
                    //MODIFICAMOS
                    tranCorrecta = UpdateInformacionTodos(txtCampo, txtCambio, txtPeriodo, lista);
                    if (tranCorrecta)
                    {
                        XtraMessageBox.Show("Actualizacion correcta de campo " + txtCampo.EditValue.ToString() + " para todos los trabajadores del periodo " + txtPeriodo.Text, "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        //GUADAMOS EN LOG
                        logRegistro log = new logRegistro(User.getUser(), "SE ACTUALIZA CAMPO " + (string)txtCampo.EditValue + " PARA TODOS LOS REGISTROS DEL PERIODO " + txtPeriodo.Text, "TRABAJADOR", "NO APLICA", txtCambio.EditValue.ToString(), "MODIFICAR");
                        log.Log();
                    }
                    else
                    {
                        XtraMessageBox.Show("Error al intentar actualizar campo", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
            }
            else
            {
                //VERIFICAR QUE EL PERIODO TENGA REGISTROS
                if (PeriodoTieneRegistros(Convert.ToInt32(txtPeriodo.Text)) == false || PeriodoTieneItems(Convert.ToInt32(txtPeriodo.Text)) == false)
                { XtraMessageBox.Show("Periodo no registra datos", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); txtPeriodo.Focus(); return; }

                if (Conjunto.ExisteConjunto(txtConjunto.Text) == false)
                { XtraMessageBox.Show("Conjunto ingresado no existe", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

               
                string con = Calculo.GetSqlFiltro(FiltroUsuario, txtConjunto.Text, ShowPrivadas);
                con = $"SELECT contrato, anomes FROM trabajador WHERE anomes={Calculo.PeriodoObservado} " + con + " AND Status=1";
                lista = Calculo.GetListPer(con);

                DialogResult Advertencia = XtraMessageBox.Show("¿Está seguro que desea modificar el campo " + (string)txtCampo.EditValue + "?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (Advertencia == DialogResult.Yes)
                {
                    //MODIFICAMOS EN BASE A CONJUNTO
                    tranCorrecta = UpdateInformacionConjunto(txtCampo, txtCambio, txtPeriodo, txtConjunto, lista);
                    if (tranCorrecta)
                    {
                        XtraMessageBox.Show("Actualizacion correcta de campo " + (string)txtCampo.EditValue + " para periodo " + txtPeriodo.Text, "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        //GUARDAMOS EVENTO EN LOG
                        logRegistro log = new logRegistro(User.getUser(), "SE ACTUALIZA CAMPO " + txtCampo.EditValue.ToString() + " PARA UN CONJUNTO DE REGISTROS PERIODO " + txtPeriodo.Text, "TRABAJADOR", "NO APLICA", txtCambio.EditValue.ToString(), "MODIFICAR");
                        log.Log();

                        Close();
                    }
                    else
                    {
                        XtraMessageBox.Show("Error al intentar actualiza campo " + txtCampo.Text, "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
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

        private void txtConjunto_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtConjunto_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            DXPopupMenu p = e.Menu;
            if (p != null)
            {
                p.Items.Clear();
                DXMenuItem menu = new DXMenuItem("Agregar Conjunto", new EventHandler(AgregarConjunto_Click));
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

        private void btnSalir_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            Close();
        }

        private void txtPeriodo_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void groupControl1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void txtConjunto_Properties_Click(object sender, EventArgs e)
        {

        }

        private void btnConjunto_Click(object sender, EventArgs e)
        {
            FrmFiltroBusqueda filtro = new FrmFiltroBusqueda(true);
            filtro.opener = this;
            filtro.ShowDialog();
        }
    }
}