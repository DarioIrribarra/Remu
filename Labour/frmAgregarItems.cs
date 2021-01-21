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
using DevExpress.Utils.Menu;
using System.Collections;

namespace Labour
{
    public partial class frmAgregarItems : DevExpress.XtraEditors.XtraForm, IConjuntosCondicionales
    {
        [DllImport("user32")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32")]
        static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        const int MF_BYCOMMAND = 0;
        const int MF_DISABLED = 2;
        const int SC_CLOSED = 0xF060;

        //LISTA CON TODOS LOS EMPLEADOS QUE YA TIENEN EL CODIGO DE ITEM QUE SE INTENTA INSERTAR
        List<Employe> TrabajadoresConItem = new List<Employe>();

        //PARA GUARDAR FILTRO USUARIO
        private string FiltroUsuario { get; set; } = User.GetUserFilter();

        //VER FICHAS PRIVADAS
        private bool ShowPrivate { get; set; } = User.ShowPrivadas();

        //PARA SABER QUE OPCION SELECCIONA (NUEVO ITEMS - MODIFICA ITEMS)

        //PARA COMUNICACION INTERFAZ
        #region "CONJUNTOS CONDICIONALES"
        public void CargarCodigoConjunto(string code)
        {
            txtConjunto.Text = code;
        }
        #endregion

        public frmAgregarItems()
        {
            InitializeComponent();
        }

        private void frmAgregarItems_Load(object sender, EventArgs e)
        {
            var sm = GetSystemMenu(Handle, false);
            EnableMenuItem(sm, SC_CLOSED, MF_BYCOMMAND | MF_DISABLED);

            txtPeriodo.Text = Calculo.PeriodoObservado.ToString();
            CargarCombo();
            btnConjunto.Enabled = false;

            CargarFormulas(ComboFormula);
            if (ComboFormula.Properties.DataSource != null)
                ComboFormula.ItemIndex = 0;
        }

        #region "MANEJO DE DATOS"
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
                                lista.Add(new ComboItem() { id = x, code = (string)rd["coditem"]});
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
            if (codes.Count>0)
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

        private void CargarFormulas(LookUpEdit pCombo)
        {
            string sql = "SELECT codFormula, descFormula FROM formula ORDER BY descFormula";
            SqlCommand cmd;
            SqlConnection cn;
            SqlDataReader rd;
            
            List<formula> formulas = new List<formula>();

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
                                    formulas.Add(new formula() { key = (string)rd["codFormula"], desc=(string)rd["descFormula"]});
                                }
                            }
                            cmd.Dispose();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                //ERROR...
            }

            if (formulas.Count > 0)
            {
                pCombo.Properties.DataSource = formulas.ToList();
                pCombo.Properties.ValueMember = "key";
                pCombo.Properties.DisplayMember = "desc";

                pCombo.Properties.PopulateColumns();

                pCombo.Properties.Columns[0].Visible = false;
                pCombo.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
                pCombo.Properties.AutoSearchColumnIndex = 1;
                pCombo.Properties.ShowHeader = false;

                pCombo.ItemIndex = 0;
            }
        }


        //OBTENER NUMERO DE ORDEN, FORMULA Y tipo para cargar en las cajas
        //VER NUMERO DE ITEM DISPONIBLE PARA CAJA CASO (N° DE CONTRATO)
        //RECARCULAR TODOS LOS ITEM??? PARA PARA CADA TRABAJADOR

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

        //OBTENER LISTADO DE CONTRATOS Y RUT (PARA EL CASO DE QUE SEAN TODOS)
        private List<Employe> TodosContratos(int Periodo)
        {
            //CONDICION ANTIGUA
            //string sql = "select DISTINCT contrato, rut, anomes FROM itemtrabajador where anomes = @periodo";
            string sql = "";
            string condicion = "";

            //SI PROPIEDAD SHOWPRIVATE ES FALSA ES PORQUE EL USUARIO NO TIENE PERMISO PARA VER FICHAS PRIVADAS

            /*USUARIO SIN FILTRO*/
            if (FiltroUsuario == "0")
                sql = string.Format("SELECT contrato, rut, anomes FROM trabajador where " + (ShowPrivate == false? "privado=0 AND ": "") + " anomes={0}", Periodo);
            else
            {
                condicion = Conjunto.GetCondicionFromCode(FiltroUsuario);
                sql = string.Format("select contrato, rut, anomes FROM trabajador " +
                " WHERE {0} " + (ShowPrivate == false? " AND privado=0":"") + " AND anomes={1}", condicion, Periodo);
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
                                contratos.Add(new Employe() {contrato = (string)rd["contrato"], rut = (string)rd["rut"], anomes = (int)rd["anomes"]});
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

        //VERIFICAR QUE EL CONTRATO TENGA REGISTROS EN ITEM DE TRABAJADOR (AL MENOS UN ITEM)
        private bool ContratoTieneRegistros(string contrato, int periodo)
        {
            bool tiene = false;
            string sql = "SELECT coditem FROM itemtrabajador WHERE contrato=@contrato AND anomes=@periodo";
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@contrato", contrato));
                        cmd.Parameters.Add(new SqlParameter("@periodo", periodo));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //TIENE REGISTROS
                            tiene = true;
                        }
                        else
                        {
                            //NO TIENE REGISTROS
                            tiene = false;
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

            return tiene;
        }

        //VERIFICAR SI ALGUNO DE LOS CONTRATOS DE LA LISTA TIENE EL ITEM QUE SE QUIERE INGRESAR
        private int ItemExisteContrato(string coditem, List<Employe> personas)
        {
            string sql = "SELECT coditem FROM itemtrabajador " +
                        "WHERE contrato=@contrato AND anomes=@periodo AND coditem=@item";
            SqlCommand cmd;
            SqlDataReader rd;
        
            int contador = 0;
            if (personas.Count>0)
            {
                foreach (var persona in personas)
                {
                    try
                    {
                        if (fnSistema.ConectarSQLServer())
                        {
                            using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                            {
                                //PARAMETROS
                                cmd.Parameters.Add(new SqlParameter("@periodo", persona.anomes));
                                cmd.Parameters.Add(new SqlParameter("@contrato", persona.contrato));
                                cmd.Parameters.Add(new SqlParameter("@item", coditem));

                                rd = cmd.ExecuteReader();
                                if (rd.HasRows)
                                {
                                    //SI EL CODIGO DE ITEM EXISTE AUMENTAMOS EL CONTADOR
                                    //GUARDAR EMPLEADO EN LISTADO
                                    TrabajadoresConItem.Add(persona);
                                    contador++;
                                }
                               
                                cmd.Parameters.Clear();
                                rd.Close();
                            }
                            
                        }
                    }
                    catch (SqlException ex)
                    {
                        XtraMessageBox.Show(ex.Message);
                    }
                }

                //CERRAMOS LA CONEXION
                fnSistema.sqlConn.Close();
            }
            return contador;
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
            string sql = "", cond = "";

            //NO HAY FILTRO DE USUARIO
            if (FiltroUsuario == "0")
            {
                //SI CONDICION ES VACIA ES PORQUE NO HAY CONJUNTO                
                sql = string.Format("SELECT contrato, rut, anomes FROM trabajador WHERE {0} AND anomes={1} " + (ShowPrivate == false? " AND privado=0":""),
                    condicion, periodo);
            }                
            else
            {
                //OBTENEMOS EL FILTRO QUE TIENE APLICADO EL TRABAJADOR
                cond = Conjunto.GetCondicionFromCode(FiltroUsuario);
                sql = string.Format("SELECT contrato, rut, anomes FROM trabajador WHERE {0} AND anomes={1} AND {2} " + (ShowPrivate == false ? " AND privado=0" : ""),
                    condicion, periodo, cond);
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
                                empleados.Add(new Employe() { contrato = (string)rd["contrato"],
                                    rut = (string)rd["rut"], anomes = (int)rd["anomes"]});
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

        //INSERT MASIVO DE ITEM DE ACUERDO A CONJUNTO SELECCIONADO O A TODOS
        //TODOS
        private bool IngresoItemsTodos(LookUpEdit pItem, TextEdit Periodo, string pFormula, int pTipo, 
            int pOrden, TextEdit ValorOriginal, bool Proporcional, bool Permanente, bool Contope, 
            TextEdit NumberCuota, TextEdit TotalCuota, CheckEdit AplicaCuota, CheckEdit pUf,
            CheckEdit pPesos, CheckEdit pPorc)
        {
            //SUPONEMOS QUE SON TODOS LOS DEL PERIODO
            string sql = "INSERT INTO itemtrabajador(coditem, anomes, contrato, rut, numitem, formula, tipo, orden, " +
                "esclase, valor, valorCalculado, proporcional, permanente, contope, cuota, pesos, uf, porc) VALUES(@item, " +
                "@anomes, @contrato, @rut, @numitem, @formula, @tipo, @orden, @esclase, @valor, @calculado, " +
                "@proporcional, @permanente, @contope, @cuota, @pesos, @uf, @porc)";

            //PARA OBTENER EL ULTIMO NUMERO DE ITEM IGRESADO 
            string sqlNumitem = "select max(numitem) as numero from itemtrabajador " +
                                "where contrato = @contrato and anomes = @anomes";

            //SABER SI EL CONTRATO TIENE REGISTROS EN TABLA ITEMTRABAJADOR
            string sqlTieneRegistros = "SELECT coditem FROM itemtrabajador WHERE contrato=@contrato AND anomes=@anomes";

            SqlCommand cmd;
            SqlTransaction tr;
            SqlDataReader rd;
            int lastNumber = 0;
            bool transaccionCorrecta = false, tieneRegistros = false;
            string cuota = AplicaCuota.Checked ? (NumberCuota.Text + "/" + TotalCuota.Text) : "0";
            //OBTENER EL RUT Y NUMERO CONTRATO DE TODOS LOS ITEMS TRABAJADOR PARA EL PERIODO SELECCIONADO
            List<Employe> empleados = new List<Employe>();
            empleados = TodosContratos(Convert.ToInt32(Periodo.Text));

            if (pItem.Text == "SALUD")
            {
                if (pPorc.Checked && pPesos.Checked && pUf.Checked)
                { XtraMessageBox.Show("Por favor selecciona solo una opcion para item salud", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }
                if (pPorc.Checked == false && pPesos.Checked == false && pUf.Checked == false)
                { XtraMessageBox.Show("Por favor selecciona al menos una opction para item salud", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }
                if (pPorc.Checked)
                {
                    if (Convert.ToDouble(txtValor.Text) > 100 || Convert.ToDouble(txtValor.Text) < 7)
                    { XtraMessageBox.Show("Por favor ingresa un porcentaje valido para item salud", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }
                }
                if (pUf.Checked)
                {
                    if (fnDecimalSalud(Convert.ToDouble(txtValor.Text)) == false)
                    { XtraMessageBox.Show("Por favor verificar el valor en uf para item salud", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    if (Convert.ToDouble(txtValor.Text) > 10)
                    { XtraMessageBox.Show("Por favor ingresa un valor uf valido para item salud", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }
                }
            }

            if (empleados.Count>0)
            {
                //RECORREMOS Y GUARDAMOS POR CADA REGISTRO              
                    try
                    {
                        if (fnSistema.ConectarSQLServer())
                        {
                            tr = fnSistema.sqlConn.BeginTransaction();
                            try
                            {
                                foreach (var persona in empleados)
                                {
                                    //VERIFICAR SI EL CONTRATO TIENE REGISTROS EN ITEMTRABAJADOR                               

                                    using (cmd = new SqlCommand(sqlTieneRegistros, fnSistema.sqlConn))
                                    {
                                        //PARAMETROS
                                        cmd.Parameters.Add(new SqlParameter("@contrato", persona.contrato));
                                        cmd.Parameters.Add(new SqlParameter("@anomes", Convert.ToInt32(Periodo.Text)));

                                        cmd.Transaction = tr;
                                        rd = cmd.ExecuteReader();
                                        if (rd.HasRows == false)
                                        {

                                            rd.Close();
                                            cmd.Parameters.Clear();
                                            lastNumber = 1;
                                            tieneRegistros = false;
                                        }
                                        else
                                        {
                                            tieneRegistros = true;
                                        }

                                        cmd.Parameters.Clear();
                                        rd.Close();
                                    }

                                    if (tieneRegistros)
                                    {
                                        //OBTENER EL ULTIMO NUMERO DE ITEM INGRESADO PARA EL CONTRATO
                                        using (cmd = new SqlCommand(sqlNumitem, fnSistema.sqlConn))
                                        {
                                            //PARAMETROS
                                            cmd.Parameters.Add(new SqlParameter("@contrato", persona.contrato));
                                            cmd.Parameters.Add(new SqlParameter("@anomes", Convert.ToInt32(Periodo.Text)));

                                            cmd.Transaction = tr;

                                            rd = cmd.ExecuteReader();
                                            if (rd.HasRows)
                                            {
                                                while (rd.Read())
                                                {
                                                    lastNumber = (int)rd["numero"];
                                                    //GUARDAMOS EL ULTIMO NUMERO INGRESADO MAS 1
                                                    lastNumber = lastNumber + 1;
                                                }
                                            }
                                            else
                                            {
                                                //SI NO TIENE REGISTROS GUARDAMOS COMO 1
                                                lastNumber = 1;
                                            }

                                            cmd.Parameters.Clear();
                                            rd.Close();
                                        }
                                    }

                                    //REALIZAR INSERT EN TABLA ITEMTRABAJADOR
                                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                                    {
                                        //PARAMETROS
                                        cmd.Parameters.Add(new SqlParameter("@item", pItem.Text));
                                        cmd.Parameters.Add(new SqlParameter("@anomes", Convert.ToInt32(Periodo.Text)));
                                        cmd.Parameters.Add(new SqlParameter("@contrato", persona.contrato));
                                        cmd.Parameters.Add(new SqlParameter("@rut", persona.rut));
                                        cmd.Parameters.Add(new SqlParameter("@numitem", lastNumber));
                                        cmd.Parameters.Add(new SqlParameter("@formula", pFormula));
                                        cmd.Parameters.Add(new SqlParameter("@tipo", pTipo));
                                        cmd.Parameters.Add(new SqlParameter("@orden", pOrden));
                                        cmd.Parameters.Add(new SqlParameter("@esclase", false));
                                        cmd.Parameters.Add(new SqlParameter("@valor", Convert.ToDecimal(ValorOriginal.Text)));
                                        cmd.Parameters.Add(new SqlParameter("@calculado", Convert.ToDecimal(0)));
                                        cmd.Parameters.Add(new SqlParameter("@proporcional", Proporcional == false ? 0 : 1));
                                        cmd.Parameters.Add(new SqlParameter("@permanente", Permanente == false ? 0 : 1));
                                        cmd.Parameters.Add(new SqlParameter("@contope", Contope == false ? 0 : 1));
                                        cmd.Parameters.Add(new SqlParameter("@cuota", cuota));
                                        cmd.Parameters.Add(new SqlParameter("@pesos", pPesos.Checked));
                                        cmd.Parameters.Add(new SqlParameter("@uf", pUf.Checked));
                                        cmd.Parameters.Add(new SqlParameter("@porc", pPorc.Checked));                                        

                                        cmd.Transaction = tr;
                                        cmd.ExecuteNonQuery();

                                        cmd.Parameters.Clear();
                                    }

                                }//END FOREACH

                            //UNA VEZ RECORRIDO TODOS LOS ELEMENTOS DE LA LISTA REALIZAMOS COMMIT
                            tr.Commit();
                            transaccionCorrecta = true;
                            }
                            catch (Exception ex)
                            {
                            //SI HAY ERRORES HACEMOS ROOLBACK
                            XtraMessageBox.Show(ex.Message);
                                tr.Rollback();
                                transaccionCorrecta = false;                         
                            }                         

                        }//EN IF EXTERNO
                    }
                    catch (SqlException ex)
                    {
                        XtraMessageBox.Show(ex.Message);
                        transaccionCorrecta = false;
                    }
            }
            else
            {
                transaccionCorrecta = false;
            }

            return transaccionCorrecta;
        }

        //INSERT MASICO DE ITEM DE ACUERDO A CONJUNTO SELECCIONADO 
        private bool IngresoItemsConjunto(LookUpEdit pItem, TextEdit Periodo, string pFormula, int pTipo,
            int pOrden, TextEdit ValorOriginal, bool Proporcional, bool Permanente, bool Contope,
            TextEdit NumberCuota, TextEdit TotalCuota, CheckEdit AplicaCuota, TextEdit pConjunto, CheckEdit pUf, 
            CheckEdit pPesos, CheckEdit pPorc)
        {
            //SUPONEMOS QUE SON TODOS LOS DEL PERIODO
            string sql = "INSERT INTO itemtrabajador(coditem, anomes, contrato, rut, numitem, formula, tipo, orden, " +
                "esclase, valor, valorCalculado, proporcional, permanente, contope, cuota, pesos, uf, porc) VALUES(@item, " +
                "@anomes, @contrato, @rut, @numitem, @formula, @tipo, @orden, @esclase, @valor, @calculado, " +
                "@proporcional, @permanente, @contope, @cuota, @pesos, @uf, @porc)";

            //PARA OBTENER EL ULTIMO NUMERO DE ITEM IGRESADO 
            string sqlNumitem = "select max(numitem) as numero from itemtrabajador " +
                                "WHERE contrato = @contrato and anomes = @anomes";

            string sqlTieneRegistros = "SELECT coditem FROM itemtrabajador WHERE contrato=@contrato AND anomes=@anomes";
           
            SqlCommand cmd;
            SqlTransaction tr;
            SqlDataReader rd;
            int lastNumber = 0;
            decimal c = 0;
            bool transaccionCorrecta = false, tieneRegistros = false;
            string cuota = AplicaCuota.Checked ? (NumberCuota.Text + "/" + TotalCuota.Text) : "0";
            string condicion = "";
            //OBTENER LISTADO DE EMPLEADOS DE ACUERDO A CONDICION DE CONJUNTO SELECCIONADO!!!
            List<Employe> empleados = new List<Employe>();
            condicion = CondicionConjunto(pConjunto.Text);
            empleados = ListadoConjunto(condicion, Convert.ToInt32(Periodo.Text));

            if (pItem.Text == "SALUD")
            {
                if (pPorc.Checked && pPesos.Checked && pUf.Checked)
                { XtraMessageBox.Show("Por favor selecciona solo una opcion para item salud", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);lblMensaje.Visible = false; return false; }
                if (pPorc.Checked == false && pPesos.Checked == false && pUf.Checked == false)
                { XtraMessageBox.Show("Por favor selecciona al menos una opction para item salud", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);lblMensaje.Visible = false; return false; }
                if (pPorc.Checked)
                {
                    if (Convert.ToDouble(txtValor.Text) > 100 || Convert.ToDouble(txtValor.Text) < 7)
                    { XtraMessageBox.Show("Por favor ingresa un porcentaje valido para item salud", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);lblMensaje.Visible = false; return false; }
                }
                if (pUf.Checked)
                {
                    if (fnDecimalSalud(Convert.ToDouble(txtValor.Text)) == false)
                    { XtraMessageBox.Show("Por favor verificar el valor en uf para item salud", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    if (Convert.ToDouble(txtValor.Text) > 10)
                    { XtraMessageBox.Show("Por favor ingresa un valor uf valido para item salud", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);lblMensaje.Visible = false; return false; }
                }
            }

            if (empleados.Count>0)
            {
                //ABRIMOS CONECCION CON LA BASE DE DATOS
                try
                {
                    if (fnSistema.ConectarSQLServer())
                    {
                        //INICIALIZAMOS LA TRANSACCION
                        tr = fnSistema.sqlConn.BeginTransaction();
                        try
                        {
                            //ITERAMOS CONTRATOS ENCONTRADOS
                            foreach (var persona in empleados)
                            {
                               
                                    //SI EL CONTRATO TIENE REGISTROS REALIZAMOS LOS PASOS SIGUIENTES
                                    using (cmd = new SqlCommand(sqlTieneRegistros, fnSistema.sqlConn))
                                    {
                                        //PARAMETROS
                                        cmd.Parameters.Add(new SqlParameter("@contrato", persona.contrato));
                                        cmd.Parameters.Add(new SqlParameter("@anomes", Convert.ToInt32(Periodo.Text)));

                                        cmd.Transaction = tr;
                                        rd = cmd.ExecuteReader();
                                        if (rd.HasRows == false)
                                        {
                                            //SALTAMOS AL SIGUIENTE REGISTRO SI NO HAY REGISTROS
                                            rd.Close();
                                            cmd.Parameters.Clear();
                                            lastNumber = 1;
                                            tieneRegistros = false;

                                        }
                                        else
                                        {
                                            tieneRegistros = true;
                                        }
                                        cmd.Parameters.Clear();
                                        rd.Close();
                                    }

                                    if (tieneRegistros)
                                    {
                                        //OBTENER EL ULTIMO NUMERO DE ITEM INGRESADO PARA EL CONTRATO
                                        using (cmd = new SqlCommand(sqlNumitem, fnSistema.sqlConn))
                                        {
                                            //PARAMETROS
                                            cmd.Parameters.Add(new SqlParameter("@contrato", persona.contrato));
                                            cmd.Parameters.Add(new SqlParameter("@anomes", Convert.ToInt32(Periodo.Text)));

                                            cmd.Transaction = tr;

                                            rd = cmd.ExecuteReader();
                                            if (rd.HasRows)
                                            {
                                                while (rd.Read())
                                                {
                                                    lastNumber = (int)rd["numero"];
                                                    //GUARDAMOS EL ULTIMO NUMERO INGRESADO MAS 1
                                                    lastNumber = lastNumber + 1;
                                                }
                                            }
                                            else
                                            {
                                                //SI NO TIENE REGISTROS GUARDAMOS COMO 1
                                                lastNumber = 1;
                                            }

                                            cmd.Parameters.Clear();
                                            rd.Close();
                                        }
                                    }

                                    //REALIZAR INSERT EN TABLA ITEMTRABAJADOR
                                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                                    {
                                        //PARAMETROS
                                        cmd.Parameters.Add(new SqlParameter("@item", pItem.Text));
                                        cmd.Parameters.Add(new SqlParameter("@anomes", Convert.ToInt32(Periodo.Text)));
                                        cmd.Parameters.Add(new SqlParameter("@contrato", persona.contrato));
                                        cmd.Parameters.Add(new SqlParameter("@rut", persona.rut));
                                        cmd.Parameters.Add(new SqlParameter("@numitem", lastNumber));
                                        cmd.Parameters.Add(new SqlParameter("@formula", pFormula));
                                        cmd.Parameters.Add(new SqlParameter("@tipo", pTipo));
                                        cmd.Parameters.Add(new SqlParameter("@orden", pOrden));
                                        cmd.Parameters.Add(new SqlParameter("@esclase", false));
                                        cmd.Parameters.Add(new SqlParameter("@valor", Convert.ToDecimal(ValorOriginal.Text)));
                                        cmd.Parameters.Add(new SqlParameter("@calculado", c));
                                        cmd.Parameters.Add(new SqlParameter("@proporcional", Proporcional == false ? 0 : 1));
                                        cmd.Parameters.Add(new SqlParameter("@permanente", Permanente == false ? 0 : 1));
                                        cmd.Parameters.Add(new SqlParameter("@contope", Contope == false ? 0 : 1));
                                        cmd.Parameters.Add(new SqlParameter("@cuota", cuota));
                                        cmd.Parameters.Add(new SqlParameter("@pesos", pPesos.Checked));
                                        cmd.Parameters.Add(new SqlParameter("@uf", pUf.Checked));
                                        cmd.Parameters.Add(new SqlParameter("@porc", pPorc.Checked));                                        

                                        cmd.Transaction = tr;
                                        cmd.ExecuteNonQuery();

                                        cmd.Parameters.Clear();
                                    }                                                        
                            }

                            //SI TERMINO DE ITERAR TODO LOS CONTRATOS Y NO HUBO PROBLEMAS REALIZAMOS COMMIT()
                            tr.Commit();
                            transaccionCorrecta = true;
                        }
                        catch (Exception ex)
                        {
                            tr.Rollback();
                            transaccionCorrecta = false;
                            XtraMessageBox.Show(ex.Message);
                        }                     
                    }

                    //CERRAMOS LA CONEXION
                    fnSistema.sqlConn.Close();
                }
                catch (SqlException ex)
                {
                    XtraMessageBox.Show(ex.Message);
                }
            }
            else
            {
                transaccionCorrecta = false;
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

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Tab)
            {
                if (txtPeriodo.ContainsFocus)
                {
                    /*if (cbPeriodo.Checked == false)
                    {
                        if (txtPeriodo.Text == "")
                        { lblMensaje.Visible = true; lblMensaje.Text = "Por favor ingresa un periodo" ;return false;}

                        if (PeriodoTieneRegistros(Convert.ToInt32(txtPeriodo.Text)) == false)
                        { lblMensaje.Visible = true; lblMensaje.Text = "Parece ser que el periodo ingresado no registra datos"; return false; }

                        lblMensaje.Visible = false;
                    }*/
                }
                if (txtConjunto.ContainsFocus)
                {
                    if (cbTodos.Checked == false)
                    {
                        if (txtConjunto.Text == "")
                        { lblMensaje.Visible = true; lblMensaje.Text = "Por favor selecciona un conjunto a evaluar"; return false; }

                        //VALIDAR QUE EL CODIGO DE CONJUNTO INGRESADO SEA VAIDO (EXISTA)
                        if (CodeConjuntoExiste(txtConjunto.Text) == false)
                        { lblMensaje.Visible = true; lblMensaje.Text = "Conjunto ingresado no es valido"; return false; }

                        lblMensaje.Visible = false;
                    }
                }
                if (txtNumCuota.ContainsFocus)
                {
                    if (cbCuota.Checked)
                    {
                        if (txtNumCuota.Text == "")
                        { lblMensaje.Visible = true;lblMensaje.Text = "Por favor ingresa el numero de cuota";return false; }

                        if (txtTotalCuotas.Text != "" && (Convert.ToInt32(txtNumCuota.Text) > Convert.ToInt32(txtTotalCuotas.Text)))
                        { lblMensaje.Visible = true;lblMensaje.Text = "Por favor verifica que el n° de cuotas no supere el total de cuotas"; return false; }

                        lblMensaje.Visible = false;
                    }
                }
                if (txtTotalCuotas.ContainsFocus)
                {
                    if (cbCuota.Checked)
                    {
                        if (txtTotalCuotas.Text == "")
                        { lblMensaje.Visible = true; lblMensaje.Text = "Por favor ingresa el total de cuotas"; return false;}

                        if (txtNumCuota.Text != "" && (Convert.ToInt32(txtTotalCuotas.Text) < Convert.ToInt32(txtNumCuota.Text)))
                        { lblMensaje.Visible = true; lblMensaje.Text = "Por favor verifica que el total de cuotas no sea menor al n° de cuota";return false; }

                        lblMensaje.Visible = false;
                    }
                }
                if (txtValor.ContainsFocus)
                {
                    if (txtValor.Text == "0" || txtValor.Text == "")
                    { lblMensaje.Visible = true; lblMensaje.Text = "Por favor ingresa el monto";return false;}

                    lblMensaje.Visible = false;
                }
            }
            return base.ProcessDialogKey(keyData);
        }

        //VERIFICAR NUMERO DECIMAL CORRECTO
        private bool fnDecimal(string cadena)
        {
            if (cadena.Length == 0) return false;

            //recorrer cadena y verificar que tenga solo una coma
            int coma = 0;
            for (int position = 0; position < cadena.Length; position++)
            {
                if (cadena[position] == ',') coma++;
            }

            if (coma > 1) return false;

            string[] subcadena = new string[2];
            if (coma == 1)
            {
                subcadena = cadena.Split(',');

                //SI DESPUES DE LA CADENA TIENE MAS DE DOS DIGITOS NO ES CORRECTO
                if (subcadena[1].Length > 4) return false;
                if (subcadena[1].Length == 0) return false;
                if (subcadena[0].Length == 0) return false;

                return true;
            }
            else
            {
                //SI NO TIENE COMAS SOLO ES UN NUMERO
                if (cadena.Length > 8) return false;
                return true;
            }

        }

        //VERIFICAR DECIMAL SOLO PARA SALUD
        private bool fnDecimalSalud(double value)
        {
            string cadena = "";
            int number = 0;
            cadena = value.ToString();
            if (cadena.Length == 0) return false;

            //recorrer cadena y verificar que tenga solo una coma
            int coma = 0;
            for (int position = 0; position < cadena.Length; position++)
            {
                if (cadena[position] == ',') coma++;
            }

            if (coma > 1) return false;

            string[] subcadena = new string[2];
            if (coma == 1)
            {
                subcadena = cadena.Split(',');
                number = Convert.ToInt32(subcadena[0]);

                //SI DESPUES DE LA CADENA TIENE MAS DE DOS DIGITOS NO ES CORRECTO
                if (subcadena[1].Length > 4) return false;
                if (subcadena[0].Length > 2) return false;
                //if (number > 10) return false;
                if (subcadena[1].Length == 0) return false;
                if (subcadena[0].Length == 0) return false;

                return true;
            }
            else
            {
                //SI NO TIENE COMAS SOLO ES UN NUMERO
                if (cadena.Length > 2) return false;

                //  number = Convert.ToInt32(cadena);
                //  if (number > 10) return false;

                return true;
            }

        }

        #endregion

        private void txtItem_EditValueChanged(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            if (txtItem.Properties.DataSource != null)
            {
                lblMensaje.Visible = false;
                Hashtable data = new Hashtable();
                int tipo = 0;
                data = DataItem(txtItem.Text);
                txtDescripcion.Text = (string)data["descripcion"];
                //txtFormula.Text = (string)data["formula"] == "0" ? "NO USA": (string)data["formula"];
                txtOrden.Text = ((int)data["orden"]).ToString();
                tipo = (int)data["tipo"];

                if (tipo == 1)
                { txtTipo.Text = "HABER IMPONIBLE"; cbCuota.Enabled = false; txtNumCuota.Enabled = false; txtTotalCuotas.Enabled = false; cbProporcional.Enabled = true; cbTope.Enabled = false; cbPorcentaje.Enabled = false; cbUf.Enabled = false; cbPesos.Enabled = false; cbPorcentaje.Checked = false; cbUf.Checked = false; cbPesos.Checked = false; }
                else if (tipo == 2)
                { txtTipo.Text = "HABER NO IMPONIBLE"; cbCuota.Enabled = false; txtNumCuota.Enabled = false; txtTotalCuotas.Enabled = false; cbProporcional.Enabled = true; cbTope.Enabled = false; cbPorcentaje.Enabled = false; cbUf.Enabled = false; cbPesos.Enabled = false; cbPorcentaje.Checked = false; cbUf.Checked = false; cbPesos.Checked = false; }
                else if (tipo == 3)
                { txtTipo.Text = "ASIGNACION FAMILIAR"; cbCuota.Enabled = false; txtNumCuota.Enabled = false; txtTotalCuotas.Enabled = false; cbProporcional.Enabled = false; cbTope.Enabled = false; cbPorcentaje.Enabled = false; cbUf.Enabled = false; cbPesos.Enabled = false; cbPorcentaje.Checked = false; cbUf.Checked = false; cbPesos.Checked = false; }
                else if (tipo == 4)
                { txtTipo.Text = "LEYES SOCIALES"; cbCuota.Enabled = false; txtNumCuota.Enabled = false; txtTotalCuotas.Enabled = false; cbProporcional.Enabled = false; cbTope.Enabled = true; cbPorcentaje.Enabled = false; cbUf.Enabled = false; cbPesos.Enabled = false; cbPorcentaje.Checked = false; cbUf.Checked = false; cbPesos.Checked = false; }
                else if (tipo == 5)
                { txtTipo.Text = "DESCUENTOS"; cbCuota.Enabled = true; cbProporcional.Enabled = false; cbTope.Enabled = true; cbPorcentaje.Enabled = false; cbUf.Enabled = false; cbPesos.Enabled = false; cbPorcentaje.Checked = false; cbUf.Checked = false; cbPesos.Checked = false; }
                else if (tipo == 6)
                { txtTipo.Text = "APORTES"; cbCuota.Enabled = false; txtTotalCuotas.Enabled = false; txtNumCuota.Enabled = false; cbProporcional.Enabled = false; cbTope.Enabled = false; cbPorcentaje.Enabled = false; cbUf.Enabled = false; cbPesos.Enabled = false; cbPorcentaje.Enabled = false; cbUf.Enabled = false; cbPesos.Enabled = false; cbPorcentaje.Checked = false; cbUf.Checked = false; cbPesos.Checked = false; }

                if (txtItem.Text == "SALUD")
                { cbPorcentaje.Enabled = true; cbPesos.Enabled = true; cbUf.Enabled = true; cbCuota.Enabled = false; cbProporcional.Enabled = false; cbTope.Enabled = false; cbPorcentaje.Checked = false; cbUf.Checked = false; cbPesos.Checked = false; }
                
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
            if (char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar) || char.IsLetter(e.KeyChar) || char.IsSeparator(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void textEdit1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar) || e.KeyChar == (char)44)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void panelControl1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textEdit2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) || char.IsSeparator(e.KeyChar) || char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void textEdit3_KeyPress(object sender, KeyPressEventArgs e)
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

        private void cbPeriodo_CheckedChanged(object sender, EventArgs e)
        {
          
        }

        private void cbTodos_CheckedChanged(object sender, EventArgs e)
        {
            if (cbTodos.Checked)
            {
                txtConjunto.Enabled = false;
                txtConjunto.Text = "";
                lblMensaje.Text = "";
                btnConjunto.Enabled = false;
            }
            else
            {
                txtConjunto.Enabled = true;
                txtConjunto.Text = "";
                txtConjunto.Focus();
                lblMensaje.Text = "";
                btnConjunto.Enabled = true;
            }
        }

        private void cbCuota_CheckedChanged(object sender, EventArgs e)
        {
            if (cbCuota.Checked)
            {
                txtNumCuota.Enabled = true;
                txtNumCuota.Text = "";
                txtNumCuota.Focus();
                txtTotalCuotas.Enabled = true;
                txtTotalCuotas.Text = "";
            }
            else
            {
                txtNumCuota.Text = "";
                txtNumCuota.Enabled = false;
                txtTotalCuotas.Text = "";
                txtTotalCuotas.Enabled = false;
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            Close();
        }

        private void txtConjunto_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            DXPopupMenu p = e.Menu;
            if (p != null)
            {
                p.Items.Clear();
                DXMenuItem menu = new DXMenuItem("Agregar Formula", new EventHandler(AgregarFormula_Click));
                menu.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("images/actions/show_16x16.png");

                p.Items.Add(menu);
            }
        }

        private void AgregarFormula_Click(object sender, EventArgs e)
        {
            //ABRIMOS EL FORMULARIO CONJUNTOS
            FrmFiltroBusqueda filtro = new FrmFiltroBusqueda(true);
            filtro.opener = this;
            filtro.ShowDialog();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (txtItem.EditValue == null)
            { lblMensaje.Visible = true;lblMensaje.Text = "Por favor selecciona un item" ;txtItem.Focus(); return;}

            //if (txtPeriodo.Text == "" && cbPeriodo.Checked == false)
            //{ lblMensaje.Visible = true;lblMensaje.Text = "Por favor ingresa un periodo" ;txtPeriodo.Focus(); return; }

            if (txtValor.Text == "0")
            { lblMensaje.Visible = true; lblMensaje.Text = "Por favor ingresa el monto del item"; txtValor.Focus();return; }

            if (fnDecimal(txtValor.Text) == false)
            { lblMensaje.Visible = true; lblMensaje.Text = "Por favor verifica el monto ingresado"; txtValor.Focus(); return;  }
            
            if (txtConjunto.Text == "" && cbTodos.Checked == false)
            { lblMensaje.Visible = true;lblMensaje.Text = "Por favor ingresa un conjunto a evaluar" ;txtConjunto.Focus(); return; }

            if (txtNumCuota.Text == "" && cbCuota.Checked)
            { lblMensaje.Visible = true; lblMensaje.Text = "Por favor ingresa el numero de la cuota";txtNumCuota.Focus(); return; }

            if (txtTotalCuotas.Text == "" && cbCuota.Checked)
            { lblMensaje.Visible = true; lblMensaje.Text = "Por favor ingresa el total de cuotas";txtTotalCuotas.Focus(); return; }

            if (ComboFormula.Properties.DataSource == null || ComboFormula.EditValue == null)
            { lblMensaje.Visible = true; lblMensaje.Text = "Por favor selecciona una formula"; ComboFormula.Focus();return; }

            Hashtable data = new Hashtable();
            data = DataItem(txtItem.Text);
            
            bool procesoCorrecto = false;
            List<Employe> Empleados = new List<Employe>();
            if (cbTodos.Checked)
            {
                //VERIFICAR SI ALGUN CONTRATO YA TIENE INGRESADO DENTRO DE SUS REGISTROS EN CODIGO DE ITEM
                Empleados = TodosContratos(Convert.ToInt32(txtPeriodo.Text));

                if (Empleados.Count == 0)
                { XtraMessageBox.Show("No se encontraron registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);return; }

                if (ItemExisteContrato(txtItem.Text, Empleados) > 0)
                {
                    //SI ES MAYOR QUE CERO ES PORQUE ALGUN TRABAJADOR YA TIENE INGRESADO ESTE CODIGO
                    //ADVERTIMOS
                    DialogResult advertencia = XtraMessageBox.Show("Hemos detectado que algunos trabajadores ya tienen este item, ¿Desea realizar el ingreso de todas maneras?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (advertencia == DialogResult.Yes)
                    {
                        //REALIZAMOS INSERT PARA TODOS LOS CONTRATOS DEL PERIODO SELECCIONADO!
                        procesoCorrecto = IngresoItemsTodos(txtItem, txtPeriodo, ComboFormula.EditValue.ToString(), (int)data["tipo"],
                            (int)data["orden"], txtValor, cbProporcional.Checked, cbPermanente.Checked, cbTope.Checked,
                            txtNumCuota, txtTotalCuotas, cbCuota, cbUf, cbPesos, cbPorcentaje);

                        if (procesoCorrecto)
                        {
                            XtraMessageBox.Show("Ingreso de items realizado correctamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            lblMensaje.Visible = false;

                            //GUARDAMOS EVENTO EN LOG
                            logRegistro log = new logRegistro(User.getUser(), "SE INGRESA ITEM " + txtItem.Text + " PARA TODOS LOS REGISTROS DEL PERIODO " + txtPeriodo.Text, "ITEMTRABAJADOR", "0", txtItem.Text, "INGRESAR");
                            log.Log();
                        }
                        else
                        { XtraMessageBox.Show("Ingreso de items no se pudo realizar", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
                    }
                    else
                    {
                        lblMensaje.Visible = false;
                        DialogResult VerData = XtraMessageBox.Show("¿Deseas revisar que trabajadores tienen actualmente el item " + txtItem.Text + "?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (VerData == DialogResult.Yes)
                        {
                            frmItemsExistentes formItem = new frmItemsExistentes(Convert.ToInt32(txtPeriodo.Text), txtItem.Text);
                            formItem.ShowDialog();
                        }
                    }
                }
                else
                {
                    DialogResult pregunta = XtraMessageBox.Show("¿Está seguro de agregar este item?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (pregunta == DialogResult.Yes)
                    {
                        //PODEMOS INGRESAR SIN PROBLEMAS...
                        //REALIZAMOS INSERT PARA TODOS LOS CONTRATOS DEL PERIODO SELECCIONADO!
                        procesoCorrecto = IngresoItemsTodos(txtItem, txtPeriodo, ComboFormula.EditValue.ToString(), (int)data["tipo"],
                            (int)data["orden"], txtValor, cbProporcional.Checked, cbPermanente.Checked, cbTope.Checked,
                            txtNumCuota, txtTotalCuotas, cbCuota, cbUf, cbPesos, cbPorcentaje);

                        if (procesoCorrecto)
                        {
                            XtraMessageBox.Show("Ingreso de items realizado correctamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            lblMensaje.Visible = false;

                            //GUARDAMOS EVENTO EN LOG
                            logRegistro log = new logRegistro(User.getUser(), "SE INGRESA ITEM " + txtItem.Text + " PARA TODOS LOS REGISTROS DEL PERIODO " + txtPeriodo.Text, "ITEMTRABAJADOR", "0", txtItem.Text, "INGRESAR");
                            log.Log();
                        }
                        else
                        { XtraMessageBox.Show("Ingreso de items no se pudo realizar", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); lblMensaje.Visible = false; return; }
                    }                    
                }             
            }
            else
            {
                //VERIFICAR QUE ALGUN CONTRATO TENGA ESTE ITEM PREVIAMENTE
                //ADVERTIMOS
                string condicion = CondicionConjunto(txtConjunto.Text);
                if (condicion == "")
                { lblMensaje.Visible = true; lblMensaje.Text = "Conjunto no existe"; txtConjunto.Focus(); return; }
                
                Empleados = ListadoConjunto(condicion, Convert.ToInt32(txtPeriodo.Text));

                if (Empleados.Count == 0)
                { XtraMessageBox.Show("No se encontraron registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);return;}

                if (ItemExisteContrato(txtItem.Text, Empleados) > 0)
                {
                    DialogResult advertencia = XtraMessageBox.Show("Hemos detectado que algunos trabajadores ya tiene este item, ¿Desea realizar el ingreso de todas maneras?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (advertencia == DialogResult.Yes)
                    {
                        //BUSCAMOS DE ACUERDO A CONDICION DEL CONJUNTO SELECIONADO
                        procesoCorrecto = IngresoItemsConjunto(txtItem, txtPeriodo, ComboFormula.EditValue.ToString(), (int)data["tipo"],
                             (int)data["orden"], txtValor, cbProporcional.Checked, cbPermanente.Checked, cbTope.Checked,
                             txtNumCuota, txtTotalCuotas, cbCuota, txtConjunto, cbUf, cbPesos, cbPorcentaje);

                        if (procesoCorrecto)
                        {
                            XtraMessageBox.Show("Ingreso de items realizado correctamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            lblMensaje.Visible = false;

                            //GUARDAMOS EVENTO EN LOG
                            logRegistro log = new logRegistro(User.getUser(), "SE INGRESA ITEM " + txtItem.Text + " PERIODO " + txtPeriodo.Text, "ITEMTRABAJADOR", "0", txtItem.Text, "INGRESAR");
                            log.Log();
                        }
                        else
                        { XtraMessageBox.Show("Ingreso de items no se pudo realizar", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
                    }
                }
                else
                {
                    DialogResult pregunta = XtraMessageBox.Show("¿Está seguro de agregar este item?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (pregunta == DialogResult.Yes)
                    {
                        //BUSCAMOS DE ACUERDO A CONDICION DEL CONJUNTO SELECIONADO
                        procesoCorrecto = IngresoItemsConjunto(txtItem, txtPeriodo, ComboFormula.EditValue.ToString(), (int)data["tipo"],
                             (int)data["orden"], txtValor, cbProporcional.Checked, cbPermanente.Checked, cbTope.Checked,
                             txtNumCuota, txtTotalCuotas, cbCuota, txtConjunto, cbUf, cbPesos, cbPorcentaje);

                        if (procesoCorrecto)
                        {
                            XtraMessageBox.Show("Ingreso de items realizado correctamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            //GUARDAMOS EVENTO EN LOG
                            logRegistro log = new logRegistro(User.getUser(), "SE INGRESA ITEM " + txtItem.Text + " PERIODO " + txtPeriodo.Text, "ITEMTRABAJADOR", "0", txtItem.Text, "INGRESAR");
                            log.Log();

                            lblMensaje.Visible = false;
                        }
                        else
                        { XtraMessageBox.Show("Ingreso de items no se pudo realizar", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);lblMensaje.Visible = false; return; }
                    }                    
                }                
            }
        }

        private void txtDescripcion_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtPeriodo_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtValor_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtNumCuota_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtTotalCuotas_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtFormula_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtOrden_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtTipo_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void labelControl2_Click(object sender, EventArgs e)
        {

        }

        private void txtConjunto_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void cbUf_CheckedChanged(object sender, EventArgs e)
        {
            if (cbUf.Checked)
            {
                cbPorcentaje.Checked = false;
                cbPesos.Checked = false;
            }
            
        }

        private void cbPorcentaje_CheckedChanged(object sender, EventArgs e)
        {
            if (cbPorcentaje.Checked)
            {
                cbPesos.Checked = false;
                cbUf.Checked = false;
            }
        }

        private void cbPesos_CheckedChanged(object sender, EventArgs e)
        {
            if (cbPesos.Checked)
            {
                cbPorcentaje.Checked = false;
                cbUf.Checked = false;
            }
        }

        private void txtDescripcion_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void groupControl1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void btnConjunto_Click(object sender, EventArgs e)
        {
            FrmFiltroBusqueda filtro = new FrmFiltroBusqueda(true);
            filtro.StartPosition = FormStartPosition.CenterParent;
            filtro.opener = this;
            filtro.ShowDialog();
        }
    }
    class ComboItem
    {
        public int id { get; set; }
        public string code { get; set; }
    }
}