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
    public partial class frmIngresarvariable : DevExpress.XtraEditors.XtraForm
    {
        //VARIABLE PARA COMUNICACION CON FORMULARIO VERVARIABLE
        public IVariable opener;

        //variable para saberla posicion del arreglo
        private int posicion = 0;

        //VARIABLE PARA SABER SI ES UPDATE O INSERT
        private bool update = false;

        //VARIABLE PARA CONSTRUCTOR PARAMETRIZAD
        //GUARDAR EL VALOR DEL CODIGO ASOCIADO A UNA VARIABLE
        public string CodeVar = "";
        public frmIngresarvariable()
        {
            InitializeComponent();
        }

        //CONSTRUCTOR PARAMETRIZADO SOLO ENCASO DE QUE SE ABRA EL FORM DESDE LA GRILLA DEL FORMULARIO VERVARIABLES
        public frmIngresarvariable(string codigo)
        {
            InitializeComponent();
            CodeVar = codigo;            
        }

        private void frmVariable_Load(object sender, EventArgs e)
        {
            if (CodeVar != "")
            {
                //SE INVOCO EL FORM DESDE GRILLA USANDO CONSTRUCTOR PARAMETRIZADO
                //CARGAR LOS DATOS EN CAJA DE TEXTO 
                fnCargarCampos(CodeVar);
            }

            fnDefaultProperties();
        }

        #region "MANEJO DE DATOS"

        //nueva variable
        /*
         * DATOS DE ENTRADA:
         * codigo --> string
         * descripcion --> string
         * valor --> decimal
         */
        private void fnNuevaVariable(TextEdit pCod, TextEdit pDesc, TextEdit pValor)
        {
            //INSERT SQL
            string sql = "INSERT into variableSistema(codvariable, descvariable, valor) VALUES(@pCod, @pDesc, @pValor)";
            SqlCommand cmd;
            int res = 0;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pCod", pCod.Text));
                        cmd.Parameters.Add(new SqlParameter("@pDesc", pDesc.Text));
                        cmd.Parameters.Add(new SqlParameter("@pValor", decimal.Parse(pValor.Text)));

                        //EJECUTAR CONSULTA
                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            XtraMessageBox.Show("Registro correcto", "Guardar", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            if (opener != null)
                                opener.RecargarGrilla();

                            //CERRAR FORMULARIO
                            Close();
                        }
                        else
                        {
                            XtraMessageBox.Show("Error al intentar guardar", "Guardar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                    }
                    //LIBERAR RECURSOS
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                }
                else
                {
                    XtraMessageBox.Show("No hay conexion con la base de datos", "Base de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        //MODIFICAR VARIABLE
        /*
         * DATOS DE ENTRADA:
         * CODIGO
         * DESCRIPCION
         * VALOR
         */

        private void fnModificarVariable(TextEdit pCod, TextEdit pDesc, TextEdit pValor)
        {
            //STRING SQL
            string sql = "UPDATE variableSistema set codvariable=@pCod, descvariable=@pDesc, valor=@pValor" +
                " WHERE codvariable=@pCod";
            SqlCommand cmd;
            int res = 0;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pCod", pCod.Text));
                        cmd.Parameters.Add(new SqlParameter("@pDesc", pDesc.Text));
                        cmd.Parameters.Add(new SqlParameter("@pValor", decimal.Parse(pValor.Text)));

                        //EJECUTAR CONSULTA
                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            XtraMessageBox.Show("Actualizacion correcta", "Actualizar", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            if (opener != null)
                                opener.RecargarGrilla();

                            //cerrar formulario
                            Close();
                        }
                        else
                        {
                            XtraMessageBox.Show("Error al intentar Actualizar", "Actualizar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                    }
                    //LIBERAR RECURSOS
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                }
                else
                {
                    XtraMessageBox.Show("No hay conexion con la base de datos", "Base de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        //LIMPIAR CAMPOS
        private void fnLimpiarCampos()
        {
            txtCodigo.Text = "";
            txtDesc.Text = "";
            txtValor.Text = "";
            txtCodigo.Focus();

            lblerror.Visible = false;
            txtCodigo.ReadOnly = false;
            update = false;
        }

        //DEFAULT PROPERTIES
        private void fnDefaultProperties()
        {
            txtCodigo.Properties.MaxLength = 7;
            txtDesc.Properties.MaxLength = 100;
            txtValor.Properties.MaxLength = 10;
            panelVariable.TabStop = false;
            btnGuardar.TabStop = false;
            btnNuevo.TabStop = false;        
        }

        //VERIFICAR SI EXISTE EL CODIGO EN BD
        //RETORNA TRUE SI EXISTE EL CODIGO
        private bool fnExisteCodigo(string pCod)
        {
            //STRING SEARCH
            string sql = "SELECT codvariable FROM variableSistema WHERE codvariable=@pCod";
            SqlCommand cmd;
            SqlDataReader rd;

            bool existe = false;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pCod", pCod));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //SI ENCUENTRA RESULTADO ES PORQUE EL CODIGO YA EXISTE EN BD
                            existe = true;
                        }
                        else
                        {
                            existe = false;
                        }
                    }
                    //LIBERAR RECURSOS
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                }
                else
                {
                    XtraMessageBox.Show("No hay conexion con la base de datos", "Base de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
            return existe;
        }

        //OBTENER TODOS LOS CODIGO DE VARIABLES
        //DEVUELVE UNA LISTA CON TODOS LOS CODIGOS
        private List<string> fnAllCodigos()
        {
            List<string> listado = new List<string>();
            string sql = "SELECT codvariable FROM variableSistema";
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
                            //RECORREMOS Y CARGAMOS LISTA CON CODIGOS
                            while (rd.Read())
                            {
                                listado.Add((string)rd["codvariable"]);
                            }
                        }
                        
                    }
                    //LIBERAR RECURSOS
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                }
                else
                {
                    XtraMessageBox.Show("No hay conexion con la base de datos", "Base de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
            //RETORNAMOS LA LISTA
            return listado;
        }

        //CARGAR VALORES EN CAMPOS
        private void fnCargarCampos(string pCod)
        {
            string sql = "SELECT codvariable, descvariable, valor FROM variableSistema WHERE codvariable=@pCod";
            SqlCommand cmd;
            SqlDataReader rd;

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pCod", pCod));

                        //EJECUTAMOS CONSULTA
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //RECORREMOS Y CARGAMOS CAMPOS
                            while (rd.Read())
                            {
                                update = true;

                                txtCodigo.ReadOnly = true;

                                txtCodigo.Text = (string)rd["codvariable"];
                                txtDesc.Text = (string)rd["descvariable"];
                                txtValor.Text = ((decimal)rd["valor"]).ToString();
                            }
                        }
                    }
                    //LIBERAR RECURSO
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                }
                else
                {
                    XtraMessageBox.Show("No hay conexion con base de datos", "Base de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        //PARA MANEJAR LA TECLA TAB!
        protected override bool ProcessDialogKey(Keys keyData)
        {
            bool existeCodigo = false;
            if (keyData == Keys.Tab)
            {
                if (txtCodigo.ContainsFocus)
                {
                    if (update == false)
                    {
                        if (txtCodigo.Text == "")
                        {
                            lblerror.Visible = true;
                            lblerror.Text = "Debes ingresar un codigo";
                            return false;
                        }
                        else
                        {
                            lblerror.Visible = false;
                            //VERIFICAR SI EL CODIGO YA EXISTE EN BD
                            existeCodigo = fnExisteCodigo(txtCodigo.Text);
                            if (existeCodigo)
                            {
                                lblerror.Visible = true;
                                lblerror.Text = "Ya existe una variable con ese codigo";
                                return false;
                            }
                            else
                            {
                                lblerror.Visible = false;
                            }
                        }
                    }
                }
                else if (txtDesc.ContainsFocus)
                {
                    if (txtDesc.Text == "")
                    {
                        lblerror.Visible = true;
                        lblerror.Text = "Debes ingresar una descripcion";
                        return false;
                    }
                    else
                    {
                        lblerror.Visible = false;
                    }
                }
                else if (txtValor.ContainsFocus)
                {
                    if (txtValor.Text == "")
                    {
                        lblerror.Visible = true;
                        lblerror.Text = "Debes ingresar un valor";
                        return false;
                    }
                    else
                    {
                        //VERIFICAR QUE DECIMAL ESTE CONSTRUIDO CORRECTAMENTE
                        bool decimalValido = false;
                        decimalValido = fnDecimal(txtValor.Text);
                        if (decimalValido == false)
                        {
                            lblerror.Visible = true;
                            lblerror.Text = "Valor no valido para campo valor";
                        }
                        else
                        {
                            lblerror.Visible = false;
                        }
                                             
                    }
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
                if (subcadena[1].Length > 2) return false;
                if (subcadena[1].Length == 0) return false;
                if (subcadena[0].Length == 0) return false;
            }
            else
            {
                //SI NO TIENE COMAS SOLO ES UN NUMERO
                return true;
            }
            return true;
        }        
        #endregion

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            //LIMPIAR CAMPOS
            fnLimpiarCampos();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (txtCodigo.Text == "") { lblerror.Visible = true;lblerror.Text = "Codigo no valido"; return;}
            if (txtDesc.Text == "") { lblerror.Visible = true; lblerror.Text = "Ingrese una descripcion";return;}
            if (txtValor.Text == "") { lblerror.Visible = true; lblerror.Text = "Ingrese un valor";return;}

            lblerror.Visible = false;
            bool existeCodigo = false;
            bool valorDecimal = false;
            if (update == false)
            {
                //INSERT
                //VERIFICAR QUE EL CODIGO DE VARIBLE NO EXISTE EN BD
                existeCodigo = fnExisteCodigo(txtCodigo.Text);
                if (existeCodigo)
                {
                    //EL CODIGO EXISTE
                    lblerror.Visible = true;
                    lblerror.Text = "Ya existe una variable con ese codigo";
                }
                else
                {
                    lblerror.Visible = false;
                    valorDecimal = fnDecimal(txtValor.Text);
                    if (valorDecimal == false)
                    {
                        lblerror.Visible = true;
                        lblerror.Text = "Valor no valido para campo Valor";
                    }
                    else
                    {
                        lblerror.Visible = false;
                        //PODEMOS REALIZAR INSERT
                        fnNuevaVariable(txtCodigo, txtDesc, txtValor);
                    }                    
                }
            }
            else
            {
                //UPDATE
                //VERIFICAR QUE EL CODIGO DE VARIABLE EXISTE EN BD
                existeCodigo = fnExisteCodigo(txtCodigo.Text);
                if (existeCodigo)
                {
                    //SI EXISTE CODIGO REALIZAMOS UPDATE EN BASE A ESE CODIGO

                    //VALIDAR DECIMAL VALIDO
                    valorDecimal = fnDecimal(txtValor.Text);
                    if (valorDecimal == false)
                    {
                        lblerror.Visible = true;
                        lblerror.Text = "Valor no valido para campo Valor";
                    }
                    else
                    {
                        lblerror.Visible = false;
                        fnModificarVariable(txtCodigo, txtDesc, txtValor);
                    }                    
                }
                else
                {
                    lblerror.Visible = true;
                    lblerror.Text = "Codigo no valido";
                }
            }
        }

        private void txtCodigo_KeyDown(object sender, KeyEventArgs e)
        {
            bool existeCodigo = false;
            if (e.KeyData == Keys.Enter)
            {
                //SI SE PRESIONA LA TECLA ENTER VALIDAMOS QUE EL CODIGO SEA VALIDO
                if (update == false && txtCodigo.ReadOnly == false)
                {
                    //CASO INSERT
                    if (txtCodigo.Text == "")
                    {
                        lblerror.Visible = true;
                        lblerror.Text = "Debes ingresar un codigo";
                    }
                    else
                    {
                        lblerror.Visible = false;
                        //VALIDAR QUE EL CODIGO NO EXISTA EN BD
                        existeCodigo = fnExisteCodigo(txtCodigo.Text);
                        if (existeCodigo)
                        {
                            lblerror.Visible = true;
                            lblerror.Text = "Ya existe una variable con ese codigo";
                        }
                        else
                        {
                            //VALIDO...
                            lblerror.Visible = false;
                        }
                    }
                }
            }
        }

        private void txtDesc_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (txtDesc.Text == "")
                {
                    lblerror.Visible = true;
                    lblerror.Text = "Debes ingresar una descripcion";
                }
                else
                {
                    lblerror.Visible = false;
                }
            }
        }

        private void txtValor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (txtValor.Text == "")
                {
                    lblerror.Visible = true;
                    lblerror.Text = "Debes ingresar un valor valido";
                }
                else
                {
                    //VERIFICAR QUE EL NUMERO TENGA UNA CONSTRUCCION CORRECTA
                    bool decimalValido = false;
                    decimalValido = fnDecimal(txtValor.Text);
                    if (decimalValido == false)
                    {
                        lblerror.Visible = true;
                        lblerror.Text = "Valor no valido para campo valor";
                        return;
                    }
                    else
                    {
                        lblerror.Visible = false;
                    }                   
                }
            }
        }

        private void txtValor_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (e.KeyChar == (char)44)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void txtCodigo_KeyPress(object sender, KeyPressEventArgs e)
        {
            
            if (char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (char.IsLetter(e.KeyChar))
            {
                e.Handled = false;
            }
        }

        private void txtDesc_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsSeparator(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (char.IsLetter(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }


    }
}