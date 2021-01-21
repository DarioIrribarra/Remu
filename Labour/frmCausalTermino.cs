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
using System.Collections;
using System.Runtime.InteropServices;

namespace Labour
{
    public partial class frmCausalTermino : DevExpress.XtraEditors.XtraForm
    {
        [DllImport("user32")]
        extern static IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32")]
        extern static bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        const int MF_BYCOMMAND = 0;
        const int MF_DISABLED = 2;
        const int SC_CLOSED = 0xF060;

        //VARIABLE PARA SABER SI ES UPDATE O INSERT
        private bool Update = false;

        //PARA COMUNICACION DESDE FORM EMPLEADO
        public ICausalTermino opener;

        //VARIABLE PARA BOTON NUEVO
        private bool cancela = false;

        public frmCausalTermino()
        {
            InitializeComponent();
        }

        private void frmCausalTermino_Load(object sender, EventArgs e)
        {
            var sm = GetSystemMenu(Handle, false);
            EnableMenuItem(sm, SC_CLOSED, MF_BYCOMMAND | MF_DISABLED);

            fndefaultProperties();

            //CARGAR GRILLA
            string grilla = "";
            grilla = "SELECT codCausal, descCausal, justificacion FROM causaltermino";
            fnSistema.spllenaGridView(gridcausal, grilla);
            fnSistema.spOpcionesGrilla(viewcausal);
            fnColumnas();

            //CARGAR CAMPOS
            fnCargarCampos(0);
        }

        #region "MANEJO DE DATOS CAUSAL"
        //NUEVO REGISTRO
        private void fnNuevaCausal(TextEdit pCod, TextEdit pDesc, TextEdit pJustificacion)
        {
            string sql = "INSERT INTO causaltermino(codCausal, descCausal, justificacion) values(@pcod, @pDesc, @pJustificacion)";
            SqlCommand cmd;
            int res = 0;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pcod", Convert.ToInt32(pCod.Text)));
                        cmd.Parameters.Add(new SqlParameter("@pDesc", pDesc.Text));
                        cmd.Parameters.Add(new SqlParameter("@pJustificacion", pJustificacion.Text));

                        //Ejecutar consulta
                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            XtraMessageBox.Show("Registro guardado", "Guardar", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            //REGISTRAMOS EVENTO EN LOG
                            logRegistro log = new logRegistro(User.getUser(), "INGRESA NUEVA CAUSAL TERMINO", "CAUSALTERMINO", "0", pDesc.Text, "INGRESAR");
                            log.Log();

                            string grilla = "";
                            grilla = "SELECT codCausal, descCausal, justificacion FROM causaltermino";
                            fnSistema.spllenaGridView(gridcausal, grilla);

                            fnCargarCampos(0);

                            if (opener != null)
                            {
                                //cargamos combobox
                                opener.CargarComboCausal();
                            }
                        }
                        else
                        {
                            XtraMessageBox.Show("Error la intentar guardar", "Guardar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    //LIBERAR RECURSOS
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                }
                else
                {
                    XtraMessageBox.Show("No hay conexion con la base de datos", "Base de datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

        }

        //MODIFICAR REGISTRO
        private void fnModificarCausal(TextEdit pDesc, int pCodBd, TextEdit pJustificacion)
        {
            string sql = "UPDATE causaltermino SET descCausal=@pDesc, justificacion=@pJustificacion WHERE codCausal=@pCod";
            SqlCommand cmd;
            int res = 0;
            //HASH PRECARGA DATOS
            Hashtable datosCausal = PrecargaCausal(pCodBd);

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS 
                        cmd.Parameters.Add(new SqlParameter("@pCod", pCodBd));
                        cmd.Parameters.Add(new SqlParameter("@pDesc", pDesc.Text));
                        cmd.Parameters.Add(new SqlParameter("@pJustificacion", pJustificacion.Text));

                        //EJECUTAR CONSULTA
                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            XtraMessageBox.Show("Registro Actualizado", "Actualizar", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            //SI HAY CAMBIOS GUARDA VALORES EN LOG
                            ComparaValorCausal(pCodBd, pDesc.Text, pJustificacion.Text, datosCausal);                            

                            string grilla = "";
                            grilla = "SELECT codCausal, descCausal, justificacion FROM causaltermino";
                            fnSistema.spllenaGridView(gridcausal, grilla);

                            fnCargarCampos(0);

                            if (opener != null)
                            {
                                //cargamos combobox
                                opener.CargarComboCausal();
                            }
                        }
                        else
                        {
                            XtraMessageBox.Show("Error al intentar actualizar", "Actualizar", MessageBoxButtons.OK,MessageBoxIcon.Warning);
                        }
                    }
                    //LIBERAR RECURSOS
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();

                }
                else
                {
                    XtraMessageBox.Show("No hay conexion con la base de datos", "Base de datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        //ELIMINAR REGISTRO
        private void fnEliminarCausal(int pCod)
        {
            string sql = "DELETE FROM causaltermino WHERE codCausal=@pCod";
            SqlCommand cmd;
            int res = 0;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROA
                        cmd.Parameters.Add(new SqlParameter("@pCod", pCod));

                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            XtraMessageBox.Show("Registro eliminado", "Eliminar", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            //REGISTRAR EVENTO EN LOG
                            logRegistro log = new logRegistro(User.getUser(), "SE ELIMINA CAUSAL DE TERMINO ", "CAUSALTERMINO", pCod.ToString(), "0", "ELIMINAR");
                            log.Log();

                            string grilla = "";
                            grilla = "SELECT codCausal, descCausal, justificacion FROM causaltermino";
                            fnSistema.spllenaGridView(gridcausal, grilla);

                            fnCargarCampos(0);

                            if (opener != null)
                            {
                                //cargamos combobox
                                opener.CargarComboCausal();
                            }
                        }
                        else
                        {
                            XtraMessageBox.Show("Error al intentar eliminar", "Eliminar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    //LIBERAR RECURSOS
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                }
                else
                {
                    XtraMessageBox.Show("No hay conexion con la base de datos" ,"Base de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        //VERIFICAR SI EL CODIGO A INGRESAR YA EXISTE
        private bool fnExisteCode(string pCod)
        {            
            string SqlLista = "SELECT codCausal FROM causaltermino";
            List<string> ListadoCodes = new List<string>();
            SqlCommand cmd;
            SqlDataReader rd;
            bool existe = false;

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(SqlLista, fnSistema.sqlConn))
                    {
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //AGREGAMOS A LISTA QUITANDO LOS ESPACIOS VACIOS
                                if(((string)rd["codCausal"]).Length>0)
                                    ListadoCodes.Add((fnSistema.QuitarEspacios((string)rd["codCausal"])).ToLower());
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

            //RECORREMOS LISTADO Y COMPARAMOS
            if (ListadoCodes.Count>0)
            {
                foreach (var elemento in ListadoCodes)
                {                    
                    if (pCod.ToLower() == elemento)
                    { existe = true; break; }
                    else
                        existe = false;
                }
            }
           
   
            return existe;
        }

        //VERIFICAR SI EXISTE CAUSAL
        private bool ExisteCausal(int pCod)
        {
            bool existe = false;
            string sql = "SELECT count(*) FROM causaltermino WHERE codCausal=@pCod";
            SqlCommand cmd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pCod", pCod));

                        if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                            existe = true;

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return existe;
        }

        //DEFAULT PROPERTIES
        private void fndefaultProperties()
        {
            btnEliminar.TabStop = false;
            btnGuardar.TabStop = false;
            btnNuevo.TabStop = false;
            
            txtcod.Properties.MaxLength = 10;
            txtdesc.Properties.MaxLength = 100;
            gridcausal.TabStop = false;
        }

        //LIMPIAR CAMPOS
        private void fnLimpiarCampos()
        {
            txtcod.Text = "";
            txtcod.Focus();
            txtdesc.Text = "";
            txtJustificacion.Text = "";
            Update = false;
            lblerror.Visible = false;
            txtcod.ReadOnly = false;

        }

        //COLUMNAS GRILLA
        private void fnColumnas()
        {
            viewcausal.Columns[0].Caption = "Codigo";
            viewcausal.Columns[0].Width = 10;
            viewcausal.Columns[0].AppearanceCell.FontStyleDelta = FontStyle.Bold;

            viewcausal.Columns[1].Caption = "Descripcion";
            viewcausal.Columns[1].Width = 200;
            viewcausal.Columns[2].Visible = false;
        }

        //CARGAR CAMPOS DESDE GRILLA
        private void fnCargarCampos(int? pos = -1)
        {
            if (viewcausal.RowCount > 0)
            {
                if (pos != -1) viewcausal.FocusedRowHandle = (int)pos;

                //UPDATE EN TRUE PARA ACTUALIZACION
                Update = true;

                txtcod.ReadOnly = true;

                //CARGAMOS CAMPOS
                txtcod.Text = viewcausal.GetFocusedDataRow()["codCausal"].ToString();
                txtdesc.Text = viewcausal.GetFocusedDataRow()["descCausal"].ToString();
                txtJustificacion.Text = viewcausal.GetFocusedDataRow()["justificacion"].ToString();
            }
            else
            {
                fnLimpiarCampos();
            }
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            string codeAntiguo = "";
            if (keyData == Keys.Tab)
            {
                if (txtcod.ContainsFocus)
                {
                    if (txtcod.Text == "")
                    {
                        lblerror.Visible = true;
                        lblerror.Text = "Por favor ingrese el codigo de la causal";
                        return false;
                    }
                    else
                    {
                        lblerror.Visible = false;
                        if (Update == false)
                        {
                            //VERIFICAR SI EL CODIGO YA EXISTE
                           // bool existe = fnExisteCode(fnSistema.QuitarEspacios(txtcod.Text));

                            if (ExisteCausal(Convert.ToInt32(txtcod.Text)))
                            {
                                lblerror.Visible = true;
                                lblerror.Text = "Codigo ingresado ya existe";
                                return false;
                            }
                            else
                            {
                                lblerror.Visible = false;
                            }                           
                        }
                                      
                    }
                }
                else if (txtdesc.ContainsFocus)
                {
                    if (txtdesc.Text == "")
                    {
                        lblerror.Visible = true;
                        lblerror.Text = "Por favor ingrese una descripcion";
                        return false;
                    }
                    else
                    {
                        lblerror.Visible = false;
                    }
                }
            }
            return base.ProcessDialogKey(keyData);  
        }

        //VERIFICAR SI YA EXISTE LA DESCRIPCION
        private bool fnDescExiste(string pDesc)
        {
            string sql = "SELECT descCausal FROM causaltermino WHERE descCausal=@pDesc";
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
                        cmd.Parameters.Add(new SqlParameter("@pDesc", pDesc));
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //EXISTE
                            existe = true;
                        }
                        else
                        {
                            existe = false;
                        }
                    }
                }
                else
                {
                    XtraMessageBox.Show("No hay conexion con la base de datos", "Base de datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
            return existe;
        }

        //VERIFICAR SI LA CAUSAL ESTA SIENDO USADA POR UN TRABAJADOR
        private bool fnCausalTrabajador(int pCod)
        {
            string sql = "SELECT contrato, causal FROM trabajador " +
                "INNER JOIN causaltermino ON causaltermino.codcausal=trabajador.causal " +
                "WHERE trabajador.causal=@pCod";

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

                        //EJECUTAR CONSULTA
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //EXISTE RELACION
                            existe = true;
                        }
                        else
                        {
                            existe = false;
                        }
                    }
                }
                else
                {
                    XtraMessageBox.Show("No hay conexion con la base de datos", "Base de datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return existe;
        }

        /*VER S HAY CAMBIOS ANTES DE CERRAR*/
        private bool CambiosSinGuardar(int pCode)
        {
            string sql = "SELECT codcausal, desccausal FROM causaltermino WHERE codcausal=@pCode";
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pCode", pCode));
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                
                                if (txtcod.Text != Convert.ToInt32(rd["codcausal"]).ToString()) return true;
                                if (txtdesc.Text.ToLower() != rd["desccausal"].ToString().ToLower()) return true;

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

            return false;
        }

        #region "LOG CAUSAL TERMINO"
        //TABLA HASH PARA RECARGA
        private Hashtable PrecargaCausal(int codigo)
        {
            Hashtable datos = new Hashtable();
            string sql = "SELECT codcausal, desccausal, justificacion FROM CAUSALTERMINO WHERE codcausal=@cod";
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@cod", codigo));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //LLENAMOS TABLA HASH
                                datos.Add("codcausal", (int)rd["codcausal"]);
                                datos.Add("desccausal", (string)rd["desccausal"]);
                                datos.Add("justificacion", (string)rd["justificacion"]);
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


            return datos;
        }

        //COMPARA VALORES
        private void ComparaValorCausal(int codigo, string descripcion, string pJustificacion, Hashtable Datos)
        {
            if (Datos.Count>0)
            {
                //COMPARA SI SE A CAMBIADO ALGUN VALOR!
                //GUARDAMOS EN LOG DE SER ASI!!!!!
                if ((string)Datos["descripcion"] != descripcion)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA DESCRIPCION CAUSAL "+ codigo, "CAUSALTERMINO", (string)Datos["descripcion"]+"", descripcion, "MODIFICA");
                    log.Log();
                }
                if ((string)Datos["justificacion"] != pJustificacion)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA JUSTIFICACION CAUSAL " + codigo, "CAUSALTERMINO", (string)Datos["justificacion"] + "", pJustificacion, "MODIFICA");
                    log.Log();
                }
               
            }
        }

        #endregion

        #endregion

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (txtcod.Text == "") { lblerror.Visible = true; lblerror.Text = "Por favor ingrese un codigo"; return;}
            if (txtdesc.Text == "") { lblerror.Visible = true; lblerror.Text = "Por favor ingrese una descripcion"; return;}

            string DescAntiguo = "";
            int codBd = 0;
            if (Update == false)
            {
                //INSERT
                //VERIFICAR QUE EL CODIGO QUE SE QUIERE INGRESAR NO EXISTE
               
                if (ExisteCausal(Convert.ToInt32(txtcod.Text)))
                {
                    lblerror.Visible = true;
                    lblerror.Text = "Codigo de causal ya existe";
                }
                else
                {
                    lblerror.Visible = false;
                    //HACEMOS INSERT
                    fnNuevaCausal(txtcod, txtdesc, txtJustificacion);
                }
            }
            else
            {
                //UPDATE             
                if (viewcausal.RowCount>0)
                {
                    //CODIGO DESDE BD
                    codBd = Convert.ToInt32(viewcausal.GetFocusedDataRow()["codCausal"]);

                    if (ExisteCausal(codBd) == false)
                    { lblerror.Visible = true; lblerror.Text = "Registro no valido"; return; }

                    lblerror.Visible = false;

                    //HACEMOS UPDATE
                    fnModificarCausal(txtdesc, codBd, txtJustificacion);                 
                }   
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (txtcod.Text == "") { lblerror.Visible = true; lblerror.Text = "Registro no valido";return;}
            lblerror.Visible = false;

            int code = 0;

            //OBTENEMOS EL CODIGO DEL REGISTRO
            if (viewcausal.RowCount > 0)
            {
                code = Convert.ToInt32(viewcausal.GetFocusedDataRow()["codcausal"]);
            }

            //VERIFICAR SI LA CAUSAL ESTA SIENDO USADA POR UN TRABAJADOR
            bool causalUsada = fnCausalTrabajador(code);
            if (causalUsada)
            {
                //ESTA SIENDO USADA 
                DialogResult adver = XtraMessageBox.Show("Esta causal esta asociada a un trabajador, por lo que no es posible realizar esta operacion", "Denegado", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            else
            {
                DialogResult dialogo = XtraMessageBox.Show("¿Desea realmente eliminar la causal de termino " + code + " ?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogo == DialogResult.Yes)
                {
                    fnEliminarCausal(code);
                }
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            fnLimpiarCampos();
            if (cancela)
            {
                btnNuevo.Text = "Nuevo";
                btnNuevo.ToolTip = "Nuevo registro";
                cancela = false;
                fnCargarCampos();
            }
            else
            {
                btnNuevo.Text = "Cancelar";
                btnNuevo.ToolTip = "Cancelar operacion";
                cancela = true;
            }
        }

        private void gridcausal_Click(object sender, EventArgs e)
        {
            fnCargarCampos();
            lblerror.Visible = false;

            //RESET BOTON
            btnNuevo.Text = "Nuevo";
            btnNuevo.ToolTip = "Nuevo registro";
            cancela = false;
        }

        private void gridcausal_KeyUp(object sender, KeyEventArgs e)
        {
            fnCargarCampos();
            lblerror.Visible = false;
        }

        private void txtcod_KeyPress(object sender, KeyPressEventArgs e)
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
        private void txtdesc_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (char.IsLetter(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (char.IsSeparator(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void txtcod_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (txtcod.Text == "")
                {
                    lblerror.Visible = true;
                    lblerror.Text = "Por favor ingrese un codigo";
                    return;
                }
                else
                {
                    lblerror.Visible = false;
                    if (Update == false)
                    {
                        //VERIFICAR SI EL CODIGO YA EXISTE
                        //bool existeCode = fnExisteCode(fnSistema.QuitarEspacios(txtcod.Text));

                        if (ExisteCausal(Convert.ToInt32(txtcod.Text)))
                        {
                            lblerror.Visible = true;
                            lblerror.Text = "codigo ya existe";
                            return;
                        }

                        lblerror.Visible = false;
                    }
                  
                }
            }
        }

        private void txtdesc_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (txtdesc.Text == "")
                {
                    lblerror.Visible = false;
                    lblerror.Text = "Por favor ingrese descripcion";
                    return;
                }
                else
                {
                    lblerror.Visible = false;
                }
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            int code = 0;
            if (viewcausal.RowCount > 0)
            {
                code = Convert.ToInt32(viewcausal.GetFocusedDataRow()["codcausal"]);
                if (CambiosSinGuardar(code))
                {
                    DialogResult adv = XtraMessageBox.Show("Hay cambios sin guardar, ¿Deseas cerrar de todas formas?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (adv == DialogResult.Yes)
                        Close();
                }
                else
                    Close();
            }
            else
                Close();
            
        }
    }
}