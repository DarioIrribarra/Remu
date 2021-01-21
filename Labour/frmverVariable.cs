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
    public partial class frmverVariable : DevExpress.XtraEditors.XtraForm, IVariable
    {
        //VARIABLE INTERFAZ
        public IVarFormula opener;

        //VARIABLE PARA GUARDAR LA POSICION DEL MEMOEDIT DEL FORMULARIO FORMULA
        private int CursorPos = 0;

        //VARIABLE PARA DISTINGUIR QUE SE INVOCO EL FORMULARIO DESDE VER VARIABLE (PARA CUANDO SE QUIERE AGREGAR VARIABLES)
        private bool CargaVariable = false;

        #region "INTERFAZ"
        public void RecargarGrilla()
        {
            fnSistema.spllenaGridView(gridVariable, "SELECT codvariable, descvariable, valor FROM variableSistema");
        }
        #endregion
        public frmverVariable()
        {
            InitializeComponent();
        }
        //CONSTRUCTOR PARAMETRIZADO PARA USO CUANDO SE DESEE CARGAR UNA VARIABLE EN MEMOEDIT
        public frmverVariable(int Cursor, bool cargavariable)
        {
            InitializeComponent();
            CursorPos = Cursor;
            this.CargaVariable = cargavariable;

        }

        private void frmverVariable_Load(object sender, EventArgs e)
        {
            //CARGAR GRILLA
            //string grilla = "SELECT codvariable, descvariable FROM variableSistema ORDER BY descvariable";
            // fnSistema.spllenaGridView(gridVariable, grilla);
            CargarGird();
           fnSistema.spOpcionesGrilla(viewVariable);
           fnColumnasGrilla();
        }

        #region "MANEJO DE DATOS"
        //OPCIONES COLUMNAS GRILLA
        private void fnColumnasGrilla()
        {
            viewVariable.Columns[0].Caption = "Codigo";            
            viewVariable.Columns[0].AppearanceCell.FontStyleDelta = FontStyle.Bold;
            viewVariable.Columns[0].Width = 30;

            viewVariable.Columns[1].Caption = "Descripcion";
        }

        //LISTA PARA GUARDAR TODAS LAS VARIABLES DE SISTEMA 
        private List<Variable> ListadoVariables()
        {
            List<Variable> listado = new List<Variable>();
            string sql = "SELECT codvariable, descvariable FROM variablesistema ORDER BY descvariable";
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
                                //LLENAMOS LISTA
                                listado.Add(new Variable(){codigo = (string)rd["codvariable"], descripcion = (string)rd["descvariable"]});
                            }
                        }                        
                    }//LIBERAR RECURSSO
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                    rd.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
            return listado;
        }

        //LISTADO DE CODIGO DE ITEM
        private List<Variable> ListadoItems()
        {
            List<Variable> va = new List<Variable>();
            string sql = "SELECT coditem, descripcion FROM item";
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
                                //LLENAMOS LISTA DE OBJETOS
                                va.Add(new Variable() {codigo = ((string)rd["coditem"]).ToLower(), descripcion = (string)rd["descripcion"]});
                                    
                            }
                        }
                    }
                    //LIBERAR RECURSOS
                    cmd.Dispose();
                    rd.Close();
                    fnSistema.sqlConn.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            //RETORNAMOS LISTADO DE ITEMS
            return va;
        }

        //LISTADO VARIABLES TABLA VALORESMES
        private List<Variable> ListadoMes()
        {
            List<Variable> listado = new List<Variable>();
            string sql = "select c.name as nombre FROM sys.columns c join sys.tables t " +
                        " ON c.object_id = t.object_id " +
                        " WHERE t.name = 'valoresmes' AND c.name <> 'topeAFP' and c.name <> 'topeSEC'";
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
                                listado.Add(new Variable() { codigo = (string)rd["nombre"], descripcion =""});
                            }
                        }
                    }
                    //LIBERAR RECURSOS
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

        //LISTADO VARIABLES
        private List<Variable> ListadoTotal()
        {
            ParametroFormula Par = new ParametroFormula();
            List<ParametroFormula> ListadoParametros = new List<ParametroFormula>();

            //OBTENER LISTADO DE VARIABLES TIPO SYS
            List<Variable> listado1 = new List<Variable>();
            listado1 = ListadoVariables();

            //OBTENER LISTADO VARIABLES ITEM
            List<Variable> listado2 = new List<Variable>();
            listado2 = ListadoItems();

            //LISTADO DE PARAMETROS
            ListadoParametros = Par.GetListado();

            List<Variable> total = new List<Variable>();

            //AGREGAMOS TODAS LAS VARIABLES DE LAS LISTA ANTERIORES A LA LISTA TOTAL
            //RECORREMOS LISTADO1
            foreach (var item in listado1)
            {
                //AGREGAMOS
                total.Add(new Variable() { codigo = item.codigo, descripcion = item.descripcion});
            }

            //RECORREMOS LISTADO 2
            foreach (var item in listado2)
            {
                total.Add(new Variable() { codigo = item.codigo, descripcion = item.descripcion});
            }

            //AGREGAMOS VARIABLES FINALES
            total.Add(new Variable() { codigo = "UF", descripcion = "UNIDAD DE FOMENTO"});
            total.Add(new Variable() {codigo = "UTM", descripcion = "UNIDAD TRIBUTARIA MENSUAL"});//utm
            total.Add(new Variable() {codigo = "IMM", descripcion = "INGRESO MINIMO MENSUAL"}); //IMM
            total.Add(new Variable() { codigo = "SIS", descripcion = "SEGURO INVALIDEZ"}); //sis
            total.Add(new Variable() { codigo = "UFA", descripcion = "UF MES ANTERIOR"});

            //AGRGAMOS PARAMETROS 
            if (ListadoParametros.Count > 0)
            {
                foreach (ParametroFormula x in ListadoParametros)
                {
                    total.Add(new Variable() { codigo = x.codPar, descripcion = x.descPar});
                }
            }

            return total;
        }

        //CARGAR GRID USANDO COMO DATASOURCE LA LISTA GENERADA EN EL METODO LISTADOTOTAL
        private void CargarGird()
        {
            List<Variable> listado = new List<Variable>();
            listado = ListadoTotal();
            gridVariable.DataSource = listado;
        }

        //VERIFICAR SI LA VARIABLE ES UN ITEM
        private bool Esitem(string cadena)
        {
            List<Variable> listado = new List<Variable>();
            listado = ListadoItems();
            bool valido = false;
            if (listado.Count > 0 && cadena.Length > 0)
            {
                foreach (var item in listado)
                {
                    string elemento = item.codigo.ToLower();
                 
                    if (elemento == cadena.ToLower())
                    {
                        valido = true;
                    }                             
                }
            }

            return valido;
        }
        #endregion

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            //MOSTRAR FORMULARIO PARA AGREGAR VARIABLES
            frmIngresarvariable ingresar = new frmIngresarvariable();
            ingresar.opener = this;
            ingresar.Show();
        }

        private void gridVariable_DoubleClick(object sender, EventArgs e)
        {
            if (CursorPos >= 0 && CargaVariable)
            {
                //CARGAR VARIABLE SELECCIONADA
                if (viewVariable.RowCount > 0)
                {
                    int pos = 0;
                    pos = viewVariable.FocusedRowHandle;
                    string value = viewVariable.GetRowCellValue(pos, "codigo").ToString();

                    //PREGUNTAR SI EL VALUE ES ITEM
                    bool ItemValido = false;
                    ItemValido = Esitem(value);
                    
                    if (ItemValido)
                    {
                        //PREGUNTAMOS...
                        DialogResult original = XtraMessageBox.Show("¿Desea utilizar el valor original del item?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (original == DialogResult.Yes)
                        {
                            value = "O" + value;

                            //CARGAR CODIGO EN MEMOEDIT
                            if (opener != null)
                            {
                                opener.CargarVariableMemo(CursorPos, value);
                                Close();
                            }
                        }
                        else
                        {
                            DialogResult calculado = XtraMessageBox.Show("¿Desea utilizar el valor calculado del item?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (calculado == DialogResult.Yes)
                            {
                                value = "C" + value;

                                //CARGAR CODIGO EN MEMOEDIT
                                if (opener != null)
                                {
                                    opener.CargarVariableMemo(CursorPos, value);
                                    Close();
                                }
                            }
                            else
                            {
                                //EL USUARIO RECHAZÓ LAS DOS PETICIONES
                                XtraMessageBox.Show("Debes seleccionar alguna de las dos opciones", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                    else
                    {
                        //CARGAR CODIGO EN MEMOEDIT
                        if (opener != null)
                        {
                            opener.CargarVariableMemo(CursorPos, value);
                            Close();
                        }
                    }
                   
                }
            }
        }

        private void gridVariable_ProcessGridKey(object sender, KeyEventArgs e)
        {
           /* if (e.KeyData == Keys.Enter)
            {
                //MOSTRAR FORMULARIO PARA MODIFICAR LOS DATOS DE LA VARIABLE MODIFICADA
                if (viewVariable.RowCount > 0)
                {
                    string code = "";
                    code = viewVariable.GetFocusedDataRow()["codvariable"].ToString();

                    //UNA VEZ OBTENIDO EL CODIGO MOSTRAMOS EL FORMULARIO
                    frmIngresarvariable ingresar = new frmIngresarvariable(code);
                    ingresar.opener = this;
                    ingresar.Show();
                }
            }*/
        }

        class Variable
        {
            public string codigo { get; set; }
            public string descripcion { get; set; }
        }

        private void gridVariable_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (CursorPos >= 0)
                {
                    //CARGAR VARIABLE SELECCIONADA
                    string codeVar = "";
                    if (viewVariable.RowCount > 0)
                    {
                        int pos = 0;
                        pos = viewVariable.FocusedRowHandle;
                        string value = viewVariable.GetRowCellValue(pos, "codigo").ToString();

                        //codeVar = viewVariable.GetFocusedDataRow()[1].ToString();


                        //CARGAR CODIGO EN MEMOEDIT
                        if (opener != null)
                        {
                            opener.CargarVariableMemo(CursorPos, value);
                            Close();
                        }
                    }
                }
            }
        }
    }
}