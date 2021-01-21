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
using System.Collections;
using DevExpress.Utils.Menu;

namespace Labour
{
    public partial class FrmModificaItems : DevExpress.XtraEditors.XtraForm, IConjuntosCondicionales, ISeleccionItemElimina
    {
        [DllImport("user32")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32")]
        static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        const int MF_BYCOMMAND = 0;
        const int MF_DISABLED = 2;
        const int SC_CLOSED = 0xF060;

        //PARA COMUNICACION CON FORMULARIO PADRE
        public ISeleccionItemElimina opener { get; set; }

        //PARA GUARDAR FILTRO USUARIO
        private string FiltroUsuario { get; set; } = User.GetUserFilter();

        //PARA GUARDAR LISTADO DESDE FORMULARIO HIJO (SI SE PERMITE SELECCION DE NUMERO ITEM A ELIMINAR
        List<ItemTrabajador> ListaSeleccionItems = new List<ItemTrabajador>();

        //USUARIO PUEDE VER FICHAS PRIVADAS
        public bool ShowPrivadas { get; set; } = User.ShowPrivadas();

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

        public FrmModificaItems()
        {
            InitializeComponent();
        }

        private void txtDescripcion_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtValor_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtTotalCuotas_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtNumCuota_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void btnModifica_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (txtItem.Properties.DataSource == null)
            { XtraMessageBox.Show("Selecciona un item valido", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

            if (ComboFormula.Properties.DataSource == null || ComboFormula.EditValue == null)
            { XtraMessageBox.Show("Selecciona una fórmula válida", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            List<Employe> Listado = new List<Employe>();

            if (cbTodos.Checked)
            {
                //REALIZAMOS UPDATE PARA TODOS LOS CONTRATOS DEL PERIODO
                if (Calculo.PeriodoValido(Convert.ToInt32(txtPeriodo.Text)) == false)
                { XtraMessageBox.Show("Periodo no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                if (cbPropYmonto.Checked)
                {
                    if (txtValor.Text == "0" || txtValor.Text == "")
                    { XtraMessageBox.Show("Por favor ingresa un monto valido", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtValor.Focus(); return; }

                    if (fnDecimal(txtValor.Text) == false)
                    { XtraMessageBox.Show("Por favor verifica el monto ingresado", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; } 
                }

                if (cbCuota.Checked)
                {
                    if (txtNumCuota.Text == "0" || txtNumCuota.Text == "")
                    { XtraMessageBox.Show("Por favor ingresa el numero de cuota", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                    if (txtTotalCuotas.Text == "0" || txtTotalCuotas.Text == "")
                    { XtraMessageBox.Show("Por favor ingresa el total de cuotas", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                    if (Convert.ToInt32(txtNumCuota.Text) > Convert.ToInt32(txtTotalCuotas.Text))
                    { XtraMessageBox.Show("El numero de cuota no puede ser mayor al total de cuotas", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);return; }                    
                }

                //OBTENER LISTADO CONTRATOS
                Listado = ListadoContratosModifica(Convert.ToInt32(txtPeriodo.Text));

                if (Listado.Count == 0)
                { XtraMessageBox.Show("No se encontraron registros", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }                

                //VERIFICAR SI HAY ITEMS REPETIDOS PARA UN MISMO CONTRATO
                if (ItemRepetido(txtItem.Text, Convert.ToInt32(txtPeriodo.Text)))
                {
                    DialogResult advertencia = XtraMessageBox.Show($"Hemos detectado que algunos trabajadores tiene mas de un vez el item {txtItem.Text}, ¿Deseas seleccionar items?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (advertencia == DialogResult.Yes)
                    {
                        //MOSTRAMOS FORM PARA SELECCION
                        frmSeleccionItemElimina frmsel = new frmSeleccionItemElimina(txtItem.Text, "modifica", "");
                        frmsel.StartPosition = FormStartPosition.CenterParent;
                        frmsel.opener = this;
                        frmsel.ShowDialog();

                        if (ListaSeleccionItems.Count == 0)
                        { XtraMessageBox.Show("No se selecionaron items", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                        DialogResult pregunta = XtraMessageBox.Show("Estás seguro de realizar modificacion", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (pregunta == DialogResult.Yes)
                        {
                            //REALIZAMOS MODIFICACION...                         
                            if (ModificaItems(Listado, txtItem.Text, txtValor, cbPermanente,
                                cbTope, cbCuota, cbProporcional, txtNumCuota, txtTotalCuotas, ListaSeleccionItems,
                                Convert.ToInt32(txtPeriodo.Text), cbPesos, cbUf, cbPorcentaje, ComboFormula.EditValue.ToString(), cbSoloProp.Checked, cbPropYmonto.Checked, true))
                            {
                                XtraMessageBox.Show("Modificacion realizada correctamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                logRegistro log = new logRegistro(User.getUser(), $"Se modifica de forma masiva items {txtItem.Text}", "ITEMTRABAJADOR", "0", txtValor.Text, "MODIFICAR");
                                log.Log();
                            }
                            else
                            {
                                XtraMessageBox.Show("Error al intentar modificar", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                }
                else
                {
                    //NO HAY ITEMS REPETIDOS
                    DialogResult advertencia = XtraMessageBox.Show($"¿Deseas realmente modificar item {txtItem.Text}?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (advertencia == DialogResult.Yes)
                    {
                        //HACEMOS UPDATE
                        if (ModificaItems(Listado, txtItem.Text, txtValor, cbPermanente,
                            cbTope, cbCuota, cbProporcional, txtNumCuota, txtTotalCuotas, null, Convert.ToInt32(txtPeriodo.Text), cbPesos, cbUf, cbPorcentaje, ComboFormula.EditValue.ToString(), cbSoloProp.Checked, cbPropYmonto.Checked))
                        {
                            XtraMessageBox.Show("Modificacion realizada correctamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            logRegistro log = new logRegistro(User.getUser(), $"Se modifica de forma masiva items {txtItem.Text}", "ITEMTRABAJADOR", "0", txtValor.Text, "MODIFICAR");
                            log.Log();
                        }
                        else
                        {
                            XtraMessageBox.Show("Error al intentar modificar", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }                    
                }
            }
            else
            {
                //SELECCIONAMOS CONJUNTO
                if (txtConjunto.Text == "") { XtraMessageBox.Show("Por favor ingresa un conjunto valido", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtValor.Focus(); return; }

                if (Conjunto.ExisteConjunto(txtConjunto.Text) == false)
                { XtraMessageBox.Show("Conjunto ingresado no existe", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                if (cbPropYmonto.Checked)
                {
                    if (txtValor.Text == "0" || txtValor.Text == "")
                    { XtraMessageBox.Show("Por favor ingresa un monto valido", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                    if (fnDecimal(txtValor.Text) == false)
                    { XtraMessageBox.Show("Por favor verifica el monto ingresado", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; } 
                }

                if (cbCuota.Checked)
                {
                    if (txtNumCuota.Text == "0" || txtNumCuota.Text == "")
                    { XtraMessageBox.Show("Por favor ingresa el numero de cuota", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                    if (txtTotalCuotas.Text == "0" || txtTotalCuotas.Text == "")
                    { XtraMessageBox.Show("Por favor ingresa el total de cuotas", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                    if (Convert.ToInt32(txtNumCuota.Text) > Convert.ToInt32(txtTotalCuotas.Text))
                    { XtraMessageBox.Show("El numero de cuota no puede ser mayor al total de cuotas", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                }

                //OBTENER CONTRATOS
                Listado = ListadoContratosModificaConjunto(Convert.ToInt32(txtPeriodo.Text), Conjunto.GetCondicionFromCode(txtConjunto.Text));

                if (Listado.Count == 0)
                { XtraMessageBox.Show("No se encontraron registros", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                //VERIFICAR SI HAY ITEMS REPETIDOS PARA UN MISMO CONTRATO
                if (ItemRepetido(txtItem.Text, Convert.ToInt32(txtPeriodo.Text)))
                {
                    DialogResult advertencia = XtraMessageBox.Show($"Hemos detectado que algunos trabajadores tiene mas de un vez el item {txtItem.Text}, ¿Deseas seleccionar items?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (advertencia == DialogResult.Yes)
                    {
                        //MOSTRAMOS FORM PARA SELECCION
                        frmSeleccionItemElimina frmsel = new frmSeleccionItemElimina(txtItem.Text, "modifica", txtConjunto.Text);
                        frmsel.StartPosition = FormStartPosition.CenterParent;
                        frmsel.opener = this;
                        frmsel.ShowDialog();

                        if (ListaSeleccionItems.Count == 0)
                        { XtraMessageBox.Show("No se selecionaron items", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                        DialogResult pregunta = XtraMessageBox.Show("Estás seguro de realizar modificacion", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (pregunta == DialogResult.Yes)
                        {
                            //REALIZAMOS MODIFICACION...
                            if (ModificaItems(Listado, txtItem.Text, txtValor, cbPermanente,
                                cbTope, cbCuota, cbProporcional, txtNumCuota, txtTotalCuotas, ListaSeleccionItems,
                                Convert.ToInt32(txtPeriodo.Text), cbPesos, cbUf, cbPorcentaje, ComboFormula.EditValue.ToString(), cbSoloProp.Checked, cbPropYmonto.Checked, true))
                            {
                                XtraMessageBox.Show("Modificacion realizada correctamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                logRegistro log = new logRegistro(User.getUser(), $"Se modifica de forma masiva items {txtItem.Text}", "ITEMTRABAJADOR", "0", txtValor.Text, "MODIFICAR");
                                log.Log();
                            }
                            else
                            {
                                XtraMessageBox.Show("Error al intentar modificar", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                }
                else
                {
                    //NO HAY ITEMS REPETIDOS
                    DialogResult advertencia = XtraMessageBox.Show($"¿Deseas realmente modificar item {txtItem.Text}?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (advertencia == DialogResult.Yes)
                    {
                        //HACEMOS UPDATE                        
                        if (ModificaItems(Listado, txtItem.Text, txtValor, cbPermanente,
                            cbTope, cbCuota, cbProporcional, txtNumCuota, txtTotalCuotas, null, Convert.ToInt32(txtPeriodo.Text), cbPesos, cbUf, cbPorcentaje, ComboFormula.EditValue.ToString(), cbSoloProp.Checked, cbPropYmonto.Checked))
                        {
                            XtraMessageBox.Show("Modificacion realizada correctamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            logRegistro log = new logRegistro(User.getUser(), $"Se modifica de forma masiva items {txtItem.Text}", "ITEMTRABAJADOR", "0", txtValor.Text, "MODIFICAR");
                            log.Log();
                        }
                        else
                        {
                            XtraMessageBox.Show("Error al intentar modificar", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
        }

        private void FrmModificaItems_Load(object sender, EventArgs e)
        {
            txtPeriodo.Text = Calculo.PeriodoObservado.ToString();
            btnConjunto.Enabled = false;
            CargarCombo();
            CargarFormulas(ComboFormula);
            cbSoloProp.Checked = true;
        }


        #region "MANEJO DATOS"
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

        //OBTENER LISTADO DE CONTRATOS QUE TIENEN EL CODITEM DE ITEM QUE SE QUIERE MODIFICAR
        private List<Employe> ListadoContratosModifica(int Periodo)
        {
            string sql = "";
            string condicion = "";

            //SI PROPIEDAD SHOWPRIVADAS ES FALSE, EL USUARIO NO TIENE PERMISO PARA VER FICHAS PRIVADAS

            if (FiltroUsuario == "0")
                sql = string.Format("SELECT DISTINCT contrato, rut, anomes FROM trabajador where anomes={0} " + (ShowPrivadas == false? " AND privado=0":""), Periodo);
            else
            {
                condicion = Conjunto.GetCondicionFromCode(FiltroUsuario);
                sql = string.Format("select DISTINCT contrato, rut, anomes FROM trabajador where " +
                "contrato IN(SELECT contrato FROM trabajador WHERE {0}) AND anomes={1} " + (ShowPrivadas == false? " AND privado=0":""), condicion, Periodo);
            }

            List<Employe> listado = new List<Employe>();
            SqlCommand cmd;
            SqlDataReader rd;
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
                                //LLENAMOS LISTADO
                                listado.Add(new Employe()
                                {
                                    anomes = Periodo,
                                    contrato = (string)rd["contrato"],
                                    rut = (string)rd["rut"]
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

            return listado;
        }

        //OBTENER LISTADO DE CONTRATOS SI ELIGE CONJUNTO
        private List<Employe> ListadoContratosModificaConjunto(int Periodo, string condicion)
        {
            string sql = "", cond = "";

            if (FiltroUsuario == "0")
                sql = string.Format("SELECT contrato, rut, anomes FROM trabajador WHERE {0} AND anomes={1} " + (ShowPrivadas==false? " AND privado=0":""),
                    condicion, Periodo);
            else
            {
                cond = Conjunto.GetCondicionFromCode(FiltroUsuario);
                sql = string.Format("SELECT contrato, rut, anomes FROM trabajador WHERE {0} AND anomes={1} AND {2} " + (ShowPrivadas == false? " AND privado=0": ""),
                    condicion, Periodo, cond);
            }

            List<Employe> listado = new List<Employe>();
            SqlCommand cmd;
            SqlDataReader rd;
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
                                //LLENAMOS LISTADO
                                listado.Add(new Employe()
                                {
                                    anomes = Periodo,
                                    contrato = (string)rd["contrato"],
                                    rut = (string)rd["rut"]
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

            return listado;
        }

        //DE ACUERDO A LISTADO HACEMOS UPDATE
        private bool ModificaItems(List<Employe> Listado, string pCodItem, TextEdit pValor,
            CheckEdit pPermanente, CheckEdit pTope, CheckEdit pCuota, CheckEdit pProporcional, 
            TextEdit pNumberCuota, TextEdit pTotalCuota, List<ItemTrabajador> Listaitems,
            int pPeriodo, CheckEdit pPesos, CheckEdit pUf, CheckEdit pPorc, 
            string pFormula, bool pSoloProp, bool pPropyMonto, bool? repite = false)
        {
            bool tranCorrecta = false;

            //SIN CONSIDERAR QUE PUEDE EXISTIR MAS DE UNA VEZ EL MISMO ITEM PARA UN CONTRATO
            string sql = "UPDATE itemtrabajador SET valor=@pValor, proporcional=@pProporcional, " +
                "permanente=@pPermanente, contope=@pTope, cuota=@pCuota, pesos=@pPesos, uf=@pUf, " +
                "porc=@pPorc, formula=@pFormula " +
                "  WHERE contrato=@pContrato AND " +
                "anomes=@pPeriodo AND coditem=@pItem";

            string sqlRepite = "UPDATE itemtrabajador SET valor=@pValor, proporcional=@pProporcional, " +
                "permanente=@pPermanente, contope=@pTope, cuota=@pCuota, formula=@pFormula" +
                " WHERE contrato=@pContrato AND " +
                "anomes=@pPeriodo AND coditem=@pItem AND numitem=@pNum";

            string sqlRepiteProp = "UPDATE itemtrabajador SET proporcional=@pProporcional, " +
               "permanente=@pPermanente, contope=@pTope, cuota=@pCuota, formula=@pFormula" +
               " WHERE contrato=@pContrato AND " +
               "anomes=@pPeriodo AND coditem=@pItem AND numitem=@pNum";

            //SOLO MODIFICAR PROPIEDADES
            string sqlProp = "UPDATE itemtrabajador SET proporcional=@pProporcional, " +
                            "permanente=@pPermanente, contope=@pTope, cuota=@pCuota, pesos=@pPesos, uf=@pUf, " +
                            "porc=@pPorc, formula=@pFormula " +
                            "  WHERE contrato=@pContrato AND " +
                            "anomes=@pPeriodo AND coditem=@pItem";

            SqlCommand cmd;
            SqlTransaction tr;

            string cuota = pCuota.Checked ? (pNumberCuota.Text + "/" + pTotalCuota.Text) : "0";
            string sqlFinal = "";

            if (Listado.Count == 0)
            { XtraMessageBox.Show("No se encontraron registros", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

            if (pCodItem == "SALUD")
            {
                if (pUf.Checked && pPesos.Checked && pPorc.Checked)
                { XtraMessageBox.Show("Por favor selecciona solo una opcion para item salud", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }
                if (pUf.Checked == false && pPesos.Checked == false && pPorc.Checked == false)
                { XtraMessageBox.Show("Por favor selecciona al menos una opcion para item salud", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }
                if (pPorc.Checked)
                {
                    if (Convert.ToDouble(txtValor.Text) > 100 || Convert.ToDouble(txtValor.Text) < 7)
                    { XtraMessageBox.Show("Por favor ingresa un porcentaje valido para item salud", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }
                }
                if (pUf.Checked)
                {
                    if (fnDecimalSalud(Convert.ToDouble(txtValor.Text)) == false)
                    { XtraMessageBox.Show("Por favor ingresa un valor en uf valido para item salud", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    if (Convert.ToDouble(txtValor.Text) > 10)
                    { XtraMessageBox.Show("Por favor ingresa un valor en uf valido para item salud", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                }
                if (pPesos.Checked)
                {
                    if (Convert.ToDouble(txtValor.Text) > 300000)
                    { XtraMessageBox.Show("Por favor ingresa un valor valido para item salud", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }
                }
            }

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    tr = fnSistema.sqlConn.BeginTransaction();
                    try
                    {
                        //RECORREMOS LISTADO DE CONTRATOS
                        if (repite == false)
                        {

                            //SOLO PROPIEDADES??
                            if (cbSoloProp.Checked)
                                sqlFinal = sqlProp;
                            else if (cbPropYmonto.Checked)
                                sqlFinal = sql;
                            
                            foreach (var people in Listado)
                            {
                                using (cmd = new SqlCommand(sqlFinal, fnSistema.sqlConn))
                                {
                                    //PARAMETROS
                                    if(cbPropYmonto.Checked)
                                        cmd.Parameters.Add(new SqlParameter("@pValor", Convert.ToDouble(pValor.Text)));
                                    cmd.Parameters.Add(new SqlParameter("@pProporcional", pProporcional.Checked ? true : false));
                                    cmd.Parameters.Add(new SqlParameter("@pPermanente", pPermanente.Checked ? true : false));
                                    cmd.Parameters.Add(new SqlParameter("@pTope", pTope.Checked ? true : false));
                                    cmd.Parameters.Add(new SqlParameter("@pCuota", cuota));
                                    cmd.Parameters.Add(new SqlParameter("@pContrato", people.contrato));
                                    cmd.Parameters.Add(new SqlParameter("@pPeriodo", people.anomes));
                                    cmd.Parameters.Add(new SqlParameter("@pItem", pCodItem));
                                    cmd.Parameters.Add(new SqlParameter("@pPesos", pPesos.Checked));
                                    cmd.Parameters.Add(new SqlParameter("@pUf", pUf.Checked));
                                    cmd.Parameters.Add(new SqlParameter("@pPorc", pPorc.Checked));
                                    cmd.Parameters.Add(new SqlParameter("@pFormula", pFormula));
                                    //cmd.Parameters.Add(new SqlParameter("@pNumitem", pNumitem));

                                    //AGREGAMOS A TRANSACCION
                                    cmd.Transaction = tr;
                                    cmd.ExecuteNonQuery();

                                    cmd.Parameters.Clear();
                                }
                            }
                        }
                        else
                        {

                            if (cbSoloProp.Checked)
                                sqlFinal = sqlProp;
                            else if (cbPropYmonto.Checked)
                                sqlFinal = sqlRepiteProp;

                            foreach (var item in Listaitems)
                            {                              
                                using (cmd = new SqlCommand(sqlFinal, fnSistema.sqlConn))
                                {
                                    //PARAMETROS
                                    if(cbPropYmonto.Checked)
                                        cmd.Parameters.Add(new SqlParameter("@pValor", Convert.ToDouble(pValor.Text)));

                                    cmd.Parameters.Add(new SqlParameter("@pProporcional", pProporcional.Checked ? true : false));
                                    cmd.Parameters.Add(new SqlParameter("@pPermanente", pPermanente.Checked ? true : false));
                                    cmd.Parameters.Add(new SqlParameter("@pTope", pTope.Checked ? true : false));
                                    cmd.Parameters.Add(new SqlParameter("@pCuota", cuota));
                                    cmd.Parameters.Add(new SqlParameter("@pContrato", item.contrato));
                                    cmd.Parameters.Add(new SqlParameter("@pPeriodo", pPeriodo));
                                    cmd.Parameters.Add(new SqlParameter("@pItem", pCodItem));
                                    cmd.Parameters.Add(new SqlParameter("@pNum", item.NumeroItem));
                                    cmd.Parameters.Add(new SqlParameter("@pPesos", pPesos.Checked));
                                    cmd.Parameters.Add(new SqlParameter("@pUf", pUf.Checked));
                                    cmd.Parameters.Add(new SqlParameter("@pPorc", pPorc.Checked));

                                 
                                    cmd.Parameters.Add(new SqlParameter("@pFormula", pFormula));

                                    //AGREGAMOS A TRANSACCION
                                    cmd.Transaction = tr;
                                    cmd.ExecuteNonQuery();

                                    cmd.Parameters.Clear();
                                }
                            }
                        }
                       

                        //LLEGADO ESTE PUNTO HACEMOS COMMIT
                        tr.Commit();
                        tranCorrecta = true;
                    }
                    catch (SqlException ex)
                    {
                        tranCorrecta = false;
                        tr.Rollback();
                    }                        
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return tranCorrecta;
        }

        //VER SI UN CONTRATO TIENE MAS DE UNA VEZ UN ITEM (REPETIDO)
        private bool ItemRepetido(string item, int periodo)
        {
            if (item != "" && periodo != 0)
            {
                //CONTAMOS CUANTAS VECES ESTA UN DETERMINADO ITEM AGRUPADO POR CONTRATO
                string sql = "";
                if (FiltroUsuario == "0")
                    sql = string.Format("SELECT count(itemTrabajador.contrato) as contar FROM itemtrabajador " +
                      " INNER JOIN trabajador ON trabajador.contrato = itemTrabajador.contrato AND " +
                      " trabajador.anomes = itemTrabajador.anomes " +
                      " WHERE coditem='{0}' AND itemtrabajador.anomes={1}" +
                      " group by itemTrabajador.contrato", item, periodo);
                else
                    sql = string.Format("SELECT count(itemTrabajador.contrato) as contar FROM itemtrabajador " +
                      " INNER JOIN trabajador ON trabajador.contrato = itemTrabajador.contrato AND " +
                      " trabajador.anomes = itemTrabajador.anomes " +
                      " WHERE coditem='{0}' AND itemtrabajador.anomes={1} AND {2} " +
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
                                    formulas.Add(new formula() { key = (string)rd["codFormula"], desc = (string)rd["descFormula"] });
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

        private void panelControl1_Paint(object sender, PaintEventArgs e)
        {

        }

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

        private void txtItem_EditValueChanged(object sender, EventArgs e)
        {
            if (txtItem.Properties.DataSource != null)
            {
                //OBTENER INFORMACION DE ITEM SELECCIONADO
                Hashtable informacionItem = new Hashtable();                
                informacionItem = DataItem(txtItem.Text);
            
                //COLOCAMOS LA DESCRIPCION LARGA DEL ITEM SELECCIONADO
                txtDescripcion.Text = (string)informacionItem["descripcion"];

                if (informacionItem.Count > 0)
                {
                    //OBTENEMOS EL TIPO
                    if (Convert.ToInt32(informacionItem["tipo"]) == 1)
                    { cbCuota.Enabled = false; txtNumCuota.Enabled = false; txtTotalCuotas.Enabled = false; cbProporcional.Enabled = true; cbTope.Enabled = false; cbPesos.Enabled = false; cbUf.Enabled = false; cbPorcentaje.Enabled = false; cbUf.Checked = false; cbPesos.Checked = false; cbPorcentaje.Checked = false; }
                    else if (Convert.ToInt32(informacionItem["tipo"]) == 2)
                    { cbCuota.Enabled = false; txtNumCuota.Enabled = false; txtTotalCuotas.Enabled = false; cbProporcional.Enabled = true; cbTope.Enabled = false; cbPesos.Enabled = false; cbUf.Enabled = false; cbPorcentaje.Enabled = false; cbUf.Checked = false; cbPesos.Checked = false; cbPorcentaje.Checked = false; }
                    else if (Convert.ToInt32(informacionItem["tipo"]) == 3)
                    { cbCuota.Enabled = false; txtNumCuota.Enabled = false; txtTotalCuotas.Enabled = false; cbProporcional.Enabled = false; cbTope.Enabled = false; cbPesos.Enabled = false; cbUf.Enabled = false; cbPorcentaje.Enabled = false; cbUf.Checked = false; cbPesos.Checked = false; cbPorcentaje.Checked = false; }
                    else if (Convert.ToInt32(informacionItem["tipo"]) == 4)
                    { cbCuota.Enabled = false; txtNumCuota.Enabled = false; txtTotalCuotas.Enabled = false; cbProporcional.Enabled = false; cbTope.Enabled = true; cbPesos.Enabled = false; cbUf.Enabled = false; cbPorcentaje.Enabled = false; cbUf.Checked = false; cbPesos.Checked = false; cbPorcentaje.Checked = false; }
                    else if (Convert.ToInt32(informacionItem["tipo"]) == 5)
                    { cbCuota.Enabled = true; cbProporcional.Enabled = false; cbTope.Enabled = true; cbPesos.Enabled = false; cbUf.Enabled = false; cbPorcentaje.Enabled = false; cbUf.Checked = false; cbPesos.Checked = false; cbPorcentaje.Checked = false; }
                    else if (Convert.ToInt32(informacionItem["tipo"]) == 6)
                    { cbCuota.Enabled = false; txtTotalCuotas.Enabled = false; txtNumCuota.Enabled = false; cbProporcional.Enabled = false; cbTope.Enabled = false; cbPesos.Enabled = false; cbUf.Enabled = false; cbPorcentaje.Enabled = false; cbUf.Checked = false; cbPesos.Checked = false; cbPorcentaje.Checked = false; }

                    if (txtItem.Text == "SALUD")
                    { cbCuota.Enabled = false; txtTotalCuotas.Enabled = false; txtNumCuota.Enabled = false; cbProporcional.Enabled = false; cbTope.Enabled = false; cbPesos.Enabled = true; cbUf.Enabled = true; cbPorcentaje.Enabled = true; }
                }
            }            
        }

        private void txtValor_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtNumCuota_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtTotalCuotas_KeyPress(object sender, KeyPressEventArgs e)
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

        private void btnSalir_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            Close();
        }

        private void cbCuota_CheckedChanged(object sender, EventArgs e)
        {
            if (cbCuota.Checked)
            {
                txtNumCuota.Enabled = true;
                txtNumCuota.Focus();
                txtNumCuota.Text = "";
                txtTotalCuotas.Enabled = true;
                txtTotalCuotas.Text = "";
            }
            else
            {
                txtNumCuota.Enabled = false;                
                txtNumCuota.Text = "";
                txtTotalCuotas.Enabled = false;
                txtTotalCuotas.Text = "";
            }
        }

        private void txtConjunto_DoubleClick(object sender, EventArgs e)
        {
            FrmFiltroBusqueda filtro = new FrmFiltroBusqueda(true);
            filtro.opener = this;
            filtro.ShowDialog();
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
                cbUf.Checked = false;
                cbPorcentaje.Checked = false;
            }
        }

        private void btnConjunto_Click(object sender, EventArgs e)
        {
            FrmFiltroBusqueda filtro = new FrmFiltroBusqueda(true);
            filtro.opener = this;
            filtro.ShowDialog();
        }

        private void cbSoloProp_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSoloProp.Checked)
            {
                cbPropYmonto.Checked = false;
            }
            else
            {
                cbPropYmonto.Checked = false;
            }
        }

        private void cbPropYmonto_CheckedChanged(object sender, EventArgs e)
        {
            if (cbPropYmonto.Checked)
            {
                cbSoloProp.Checked = false;
            }
            else
            {
                cbSoloProp.Checked = true;
            }
        }
    }
}