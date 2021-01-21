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
using DevExpress.Utils.Menu;
using System.Data.SqlClient;
using System.Collections;

namespace Labour
{
    public partial class frmCuentaContable : DevExpress.XtraEditors.XtraForm
    {
        /// <summary>
        /// indica si se actualiza un registro o es un registro nuevo.
        /// </summary>
        public bool UpdateReg { get; set; } = false;

        /// <summary>
        /// indica si se actualiza un registro o es un registro nuevo.
        /// </summary>
        public bool UpdateEsquema { get; set; }

        //VARIABLE PARA BOTON NUEVO
        private bool cancela = false;
        private bool CancelEsq = false;

        /// <summary>
        /// indica el codigo del esquema con el cual se está trabajando.
        /// </summary>
       // public int Esquema { get; set; } = 1;

        /// <summary>
        /// Para llenar la grilla
        /// </summary>
        public string SqlConsulta { get; set; } = "SELECT codCuenta, descCuenta, agrut FROM Cuenta ORDER BY codCuenta";

        /// <summary>
        /// Para grilla esquema
        /// </summary>
        public string SqlGrillaEsquema { get; set; } = "SELECT cod, descEsq, col, formato, separador FROM esquema ORDER BY cod";

        public int EsquemaSeleccionado { get; set; } = 0;
        public string NombreEsquemaSeleccionado { get; set; } = "";

        public frmCuentaContable()
        {
            InitializeComponent();
        }



        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();
            string sqlCuenta = $"SELECT codCuenta, descCuenta, agrut FROM Cuenta WHERE codEs={EsquemaSeleccionado} ORDER BY codCuenta";
            int CodBd = 0;
            Cuenta cu = new Cuenta();

            Cursor.Current = Cursors.WaitCursor;

            if (txtAgrupaCombo.Properties.DataSource == null || txtAgrupaCombo.EditValue == null)
            { XtraMessageBox.Show("Por favor selecciona opcion en agrupa rut", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            //ACTUALIZAR
            if (UpdateReg)
            {
                if (viewMaestro.RowCount == 0)
                { XtraMessageBox.Show("Registro no válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                if (viewMaestro.RowCount > 0)
                {
                    CodBd = Convert.ToInt32(viewMaestro.GetFocusedDataRow()["codCuenta"]);
                    if (cu.ExisteCuenta(CodBd, EsquemaSeleccionado) == false)
                    { XtraMessageBox.Show("Registro seleccionado no existe", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                    if (cu.ModificarCuenta(CodBd, txtDescripcion.Text, EsquemaSeleccionado, Convert.ToInt32(txtAgrupaCombo.EditValue)))
                    {
                        XtraMessageBox.Show("Registro actualizado correctamente", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
                        fnSistema.spllenaGridView(gridMaestro, sqlCuenta);
                        if (viewMaestro.RowCount > 0)
                        {
                            fnSistema.spOpcionesGrilla(viewMaestro);
                            Columnas();
                            CargarCampos();
                        }                       

                    }
                    else
                    {
                        XtraMessageBox.Show("Registro no se pudo actualizar", "Información", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                }
            }
            else
            {
                //INGRESAR
                if (txtCodigo.Text == "")
                { XtraMessageBox.Show("Por favor ingresa un codigo de registro válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);txtCodigo.Focus(); return; }

                if (cu.ExisteCuenta(Convert.ToInt32(txtCodigo.Text), EsquemaSeleccionado))
                { XtraMessageBox.Show("Por favor verifica que el código ingresado no exista", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtCodigo.Focus(); return; }

                if (cu.IngresarCuenta(Convert.ToInt32(txtCodigo.Text), txtDescripcion.Text, EsquemaSeleccionado, Convert.ToInt32(txtAgrupaCombo.EditValue)))
                {
                    XtraMessageBox.Show("Ingreso realizado correctamente", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    fnSistema.spllenaGridView(gridMaestro, sqlCuenta);
                    if (viewMaestro.RowCount > 0)
                    {
                        fnSistema.spOpcionesGrilla(viewMaestro);
                        Columnas();
                        CargarCampos();
                    }                   

                    btnNuevo.Text = "Nuevo";
                    btnNuevo.ToolTip = "Nuevo registro";
                    cancela = false;
                }
                else
                {
                    XtraMessageBox.Show("No se pudo realiza ingreso de registro", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            string sqlCuenta = $"SELECT codCuenta, descCuenta, agrut FROM Cuenta WHERE codEs={EsquemaSeleccionado} ORDER BY codCuenta";
            int codBd = 0;
            string Desc = "";
            Cuenta cu = new Cuenta();
            DetalleCuenta Det = new DetalleCuenta();            

            if (viewMaestro.RowCount > 0)
            {
                codBd = Convert.ToInt32(viewMaestro.GetFocusedDataRow()["codCuenta"]);
                Desc = (string)viewMaestro.GetFocusedDataRow()["descCuenta"];
                if (cu.ExisteCuenta(codBd, EsquemaSeleccionado))
                {
                    //CUENTA USADA POR UN ITEM?
                    //CUENTA TIENE ELEMENTOS??
                    if (cu.CuentaUsada(codBd, EsquemaSeleccionado) || cu.CuentaVacia(codBd))
                    {
                        DialogResult Adv = XtraMessageBox.Show($"la cuenta {Desc} está en uso, ¿Deseas eliminar de todas maneras?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (Adv == DialogResult.Yes)
                        {
                            if (cu.EliminarCuenta(codBd, EsquemaSeleccionado))
                            {
                                XtraMessageBox.Show($"Cuenta {Desc} eliminada correctamente", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                fnSistema.spllenaGridView(gridMaestro, sqlCuenta);
                                fnSistema.spOpcionesGrilla(viewMaestro);
                                Columnas();
                                CargarCampos();
                            }
                            else
                            {
                                XtraMessageBox.Show($"No se pudo eliminar cuenta {Desc}", "Información", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            }
                        }
                    }
                    else
                    {
                        //SOLO ELIMINAMOS
                        DialogResult Adv = XtraMessageBox.Show($"¿Realmente deseas eliminar la cuenta {Desc}?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Stop);
                        if (Adv == DialogResult.Yes)
                        {
                            if (cu.EliminarCuenta(codBd, EsquemaSeleccionado))
                            {
                                XtraMessageBox.Show($"Cuenta {Desc} eliminada correctamente", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                fnSistema.spllenaGridView(gridMaestro, SqlConsulta);
                                fnSistema.spOpcionesGrilla(viewMaestro);
                                Columnas();
                                CargarCampos();
                            }
                            else
                            {
                                XtraMessageBox.Show($"No se pudo eliminar cuenta {Desc}", "Información", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            }
                        }
                    }                    
                }
            }
            else
            {
                XtraMessageBox.Show("Registro no válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {            
            if (cancela)
            {
                btnNuevo.Text = "Nuevo";
                btnNuevo.ToolTip = "Nuevo registro";
                cancela = false;
                CargarCampos();

                DeshabilitarControlesEsquema(false);
            }
            else
            {
                btnNuevo.Text = "Cancelar";
                btnNuevo.ToolTip = "Cancelar operacion";
                cancela = true;
                LimpiarCampo();

                DeshabilitarControlesEsquema(true);
            }
        }

        #region "Datos"
        /// <summary>
        /// Cargar campos grilla cuentas 
        /// </summary>
        private void CargarCampos()
        {
            if (viewMaestro.RowCount > 0)
            {
                btnEliminar.Enabled = true;
                UpdateReg = true;

                txtCodigo.Text = Convert.ToInt32(viewMaestro.GetFocusedDataRow()["codCuenta"]) + "";
                txtDescripcion.Text = (string)viewMaestro.GetFocusedDataRow()["descCuenta"];
                txtAgrupaCombo.EditValue = Convert.ToInt32(viewMaestro.GetFocusedDataRow()["agrut"]);
                lblMessage.Visible = false;

                txtCodigo.ReadOnly = true;
                DeshabilitarControlesEsquema(false);

            }
            else
            {
                LimpiarCampo();
            }
        }

        private void LimpiarCampo()
        {
            txtCodigo.Text = "";
            txtCodigo.Focus();
            txtDescripcion.Text = "";
            btnEliminar.Enabled = false;
            UpdateReg = false;
            lblMessage.Visible = false;
            txtCodigo.ReadOnly = false;
        }

        /// <summary>
        /// Columnas gridview cuentas
        /// </summary>
        private void Columnas()
        {
            if (viewMaestro.RowCount > 0)
            {
                viewMaestro.Columns[0].Caption = "N°";
                viewMaestro.Columns[0].Width = 10;
                viewMaestro.Columns[1].Caption = "Descripcion";
                viewMaestro.Columns[2].Visible = false;
            }
        }
        /// <summary>
        /// Colunas grilla esquema
        /// </summary>
        private void ColumnasEsquema()
        {
            if (viewEsquema.RowCount > 0)
            {
                viewEsquema.Columns[0].Caption = "#";
                viewEsquema.Columns[0].Width = 10;
                viewEsquema.Columns[1].Caption = "Descripcion";
                viewEsquema.Columns[2].Caption = "Columnas";
                viewEsquema.Columns[3].Caption = "Formato";
                viewEsquema.Columns[4].Caption = "Separador";
            }
        }

        /// <summary>
        /// Setear cajas de texto de acuerdo a fila seleccionada de grilla Esquema
        /// </summary>
        private void CargarCamposEsq()
        {
            if (viewEsquema.RowCount > 0)
            {
                txtCodEsquema.Text = Convert.ToInt32(viewEsquema.GetFocusedDataRow()["cod"]) + "";
                txtDescEsquema.Text = (string)(viewEsquema.GetFocusedDataRow()["descEsq"]);
                txtColEsquema.EditValue = Convert.ToInt32(viewEsquema.GetFocusedDataRow()["col"]);
                txtFormato.EditValue = Convert.ToInt32(viewEsquema.GetFocusedDataRow()["formato"]);
                txtSeparador.EditValue = Convert.ToInt32(viewEsquema.GetFocusedDataRow()["separador"]);

                txtCodEsquema.ReadOnly = true;

                UpdateEsquema = true;
                btnEliminarEsquema.Enabled = true;

                //Guardamos el registro seleccionado
                EsquemaSeleccionado = Convert.ToInt32(viewEsquema.GetFocusedDataRow()["cod"]);
                NombreEsquemaSeleccionado = (string)viewEsquema.GetFocusedDataRow()["descEsq"];
                groupCuentas.Text = $"Cuentas Esquema '{NombreEsquemaSeleccionado}'";

                DeshabilitarControles(false);

                //RECARGAR GRILLA MAESTROS
                string sqlCuenta = $"SELECT codCuenta, descCuenta, agrut FROM Cuenta WHERE codEs={EsquemaSeleccionado}";
                fnSistema.spllenaGridView(gridMaestro, sqlCuenta);
                if (viewMaestro.RowCount > 0)
                {
                    fnSistema.spOpcionesGrilla(viewMaestro);
                    Columnas();
                    CargarCampos();
                }
                else
                {
                    LimpiarCampo();
                }
            }
            else
            {
                LimpiarCampo();
                DeshabilitarControles(true);
            }
        }

        private void ComboOpcion(LookUpEdit pCombo)
        {
            //instanciamos a la clase combo
            List<datoCombobox> lista = new List<datoCombobox>();

            lista.Add(new datoCombobox() { descInfo = "No", KeyInfo = 1 });
            lista.Add(new datoCombobox() { descInfo = "Si", KeyInfo = 2 });

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

            if (pCombo.Properties.DataSource != null)
                pCombo.ItemIndex = 0;
        }

        /// <summary>
        /// Combo con seleccion de numero de columnas
        /// </summary>
        private void ComboColumnas(LookUpEdit pCombo, string pValueMember, string pDisplayMember)
        {
            List<datoCombobox> Listado = new List<datoCombobox>();
            int count = 1;

            while (count <=25)
            {
                Listado.Add(new datoCombobox() { KeyInfo = count, descInfo = count + "" });
                count++;
            }

            pCombo.Properties.DataSource = Listado.ToList();
            SetPropertiesCombo(pCombo, pValueMember, pDisplayMember);
        }

        /// <summary>
        /// Combo seleccion formatos de salida (Excel, texto, etc)
        /// </summary>
        /// <param name="pCombo"></param>
        /// <param name="pValueMember"></param>
        /// <param name="pDisplayMember"></param>
        private void ComboFormato(LookUpEdit pCombo, string pValueMember, string pDisplayMember)
        {
            List<datoCombobox> Listado = new List<datoCombobox>();

            Listado.Add(new datoCombobox() { KeyInfo = 1, descInfo = "Texto"});
            Listado.Add(new datoCombobox() { KeyInfo = 2, descInfo = "Excel" });

            pCombo.Properties.DataSource = Listado.ToList();
            SetPropertiesCombo(pCombo, pValueMember, pDisplayMember);
        }

        /// <summary>
        /// Combo seleccion para separadores (tabulador, ;, etc.)
        /// </summary>
        /// <param name="pCombo"></param>
        /// <param name="pValueMember"></param>
        /// <param name="pDisplayMember"></param>
        private void ComboSeparador(LookUpEdit pCombo, string pValueMember, string pDisplayMember)
        {
            List<datoCombobox> Listado = new List<datoCombobox>();

            Listado.Add(new datoCombobox() { KeyInfo = 1, descInfo = "Tabulador" });
            Listado.Add(new datoCombobox() { KeyInfo = 2, descInfo = "Punto y Coma" });
            Listado.Add(new datoCombobox() { KeyInfo = 3, descInfo = "Coma" });

            pCombo.Properties.DataSource = Listado.ToList();
            SetPropertiesCombo(pCombo, pValueMember, pDisplayMember);
        }

        /// <summary>
        /// Setea las propiedades de un LookUpEdit
        /// </summary>
        /// <param name="pCombo"></param>
        /// <param name="pValueMember"></param>
        /// <param name="pDisplayMember"></param>
        private void SetPropertiesCombo(LookUpEdit pCombo, string pValueMember, string pDisplayMember)
        {
            //PROPIEDADES COMBOBOX            
            pCombo.Properties.ValueMember = pValueMember;
            pCombo.Properties.DisplayMember = pDisplayMember;

            pCombo.Properties.PopulateColumns();

            //ocultamos la columan key
            pCombo.Properties.Columns[0].Visible = false;

            pCombo.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
            pCombo.Properties.AutoSearchColumnIndex = 1;
            pCombo.Properties.ShowHeader = false;

            //if (pCombo.Properties.DataSource != null)
            //    pCombo.ItemIndex = 0;
        }

        private void DeshabilitarControles(bool pOption)
        {
            //DESHABILITAR
            if (pOption)
            {
                btnGuardar.Enabled = false;
                btnEliminar.Enabled = false;
                btnNuevo.Enabled = false;
                txtCodigo.Enabled = false;
                txtDescripcion.Enabled = false;
                txtAgrupaCombo.Enabled = false;
                gridMaestro.Enabled = false;
                
            }
            else
            {
                btnGuardar.Enabled = true;
                btnEliminar.Enabled = true;
                btnNuevo.Enabled = true;
                txtCodigo.Enabled = true;
                txtDescripcion.Enabled = true;
                gridMaestro.Enabled = true;
                txtAgrupaCombo.Enabled = true;
            }

        }

        private void DeshabilitarControlesEsquema(bool pOption)
        {
            //DESHABILITAR
            if (pOption)
            {
                btnGuardarEsquema.Enabled = false;
                btnEliminarEsquema.Enabled = false;
                btnNuevoEsquema.Enabled = false;
                txtCodEsquema.Enabled = false;
                txtDescEsquema.Enabled = false;                
                gridEsquema.Enabled = false;
                txtFormato.Enabled = false;
                txtSeparador.Enabled = false;
                txtColEsquema.Enabled = false;
            }
            else
            {
                btnGuardarEsquema.Enabled = true;
                btnEliminarEsquema.Enabled = true;
                btnNuevoEsquema.Enabled = true;
                txtCodEsquema.Enabled = true;
                txtDescEsquema.Enabled = true;
                gridEsquema.Enabled = true;
                txtFormato.Enabled = true;
                txtSeparador.Enabled = true;
                txtColEsquema.Enabled = true;
            }
        }

        //Solo para cuando se cambian la cantidad de columnas
        private bool ModificaTransaccion(int pCodEsquema, string pDesc, int pCol, int pFormato, int pSeparador)
        {
            string sqlUpdate = "UPDATE esquema SET descEsq=@pDesc, col=@pCol, formato=@pFormato, separador=@pSeparador" +
                         " WHERE cod=@pCod";
            string sqlDelete = "DELETE FROM DetalleCuenta WHERE codes=@pCodEs";

            //string sqlusado = "SELECT count(*) FROM DetalleCuenta WHERE codEs=@pCodEs";
            SqlCommand cmd;
            SqlTransaction tr;
            SqlConnection cn;
            int Count = 0;

            Esquema Es = new Esquema();

            Hashtable pData = new Hashtable();
            pData = Es.Precarga(pCodEsquema);
            bool TransaccionCorrecta = false;

            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        tr = cn.BeginTransaction();
                        try
                        {
                            //ELIMINAR DETALLE CUENTA ASOCIADO A CUENTA
                            using (cmd = new SqlCommand(sqlDelete, cn))
                            {
                                cmd.Parameters.Add(new SqlParameter("@pCodEs", pCodEsquema));
                                cmd.Transaction = tr;
                                Count = cmd.ExecuteNonQuery();
                                if (Count > 0)
                                {
                                    logRegistro log = new logRegistro(User.getUser(), $"SE ELIMINA DETALLECUENTA PARA ESQUEMA {pCodEsquema}", "DETALLECUENTA", pCodEsquema + "", "", "ELIMINAR");
                                    log.Log();
                                }
                            }

                            //Modificamos dato
                            using (cmd = new SqlCommand(sqlUpdate, cn))
                            {
                                cmd.Parameters.Add(new SqlParameter("@pCod", pCodEsquema));
                                cmd.Parameters.Add(new SqlParameter("@pDesc", pDesc));
                                cmd.Parameters.Add(new SqlParameter("@pCol", pCol));
                                cmd.Parameters.Add(new SqlParameter("@pFormato", pFormato));
                                cmd.Parameters.Add(new SqlParameter("@pSeparador", pSeparador));

                                cmd.Transaction = tr;
                                Count = cmd.ExecuteNonQuery();

                                if (Count > 0)
                                {
                                    //Escribimos en Log.
                                    Es.WriteEvento(pData, pCodEsquema, pDesc, pFormato, pSeparador, pCol);
                                }
                            }

                            tr.Commit();
                            TransaccionCorrecta = true;
                        }
                        catch (SqlException ex)
                        {
                            TransaccionCorrecta = false;
                            tr.Rollback();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                //error...
                TransaccionCorrecta = false;
            }

            return TransaccionCorrecta;

        }

        private void LimpiarEsq()
        {
            txtCodEsquema.Text = "";
            txtCodEsquema.Focus();
            txtDescEsquema.Text = "";
            if(txtColEsquema.Properties.DataSource != null)
                txtColEsquema.ItemIndex = 0;
            if (txtFormato.Properties.DataSource != null)
                txtFormato.ItemIndex = 0;
            if (txtSeparador.Properties.DataSource != null)
                txtSeparador.ItemIndex = 0;
            UpdateEsquema = false;
            btnEliminarEsquema.Enabled = false;
            txtCodEsquema.ReadOnly = false;
            
        }
        #endregion

        private void gridMaestro_Click(object sender, EventArgs e)
        {
            if (viewMaestro.RowCount > 0)
            {
                CargarCampos();
                btnNuevo.Text = "Nuevo";
                btnNuevo.ToolTip = "Nuevo registro";
                cancela = false;

                DeshabilitarControlesEsquema(false);
            }
        }

        private void gridMaestro_KeyUp(object sender, KeyEventArgs e)
        {
            if (viewMaestro.RowCount > 0)
            {
                CargarCampos();
            }
        }

        private void frmCuentaContable_Load(object sender, EventArgs e)
        {
            string sqlCuenta = "";
           
            fnSistema.spllenaGridView(gridEsquema, SqlGrillaEsquema);       
            if (viewEsquema.RowCount > 0)
            {
                fnSistema.spOpcionesGrilla(viewEsquema);
                ColumnasEsquema();
                CargarCamposEsq();
            }
            else
            {
                LimpiarCampo();
                DeshabilitarControles(true);
            }

            sqlCuenta = $"SELECT codCuenta, descCuenta, agrut FROM Cuenta WHERE codEs={EsquemaSeleccionado} ORDER BY codCuenta";
            fnSistema.spllenaGridView(gridMaestro, sqlCuenta);
            if (viewMaestro.RowCount > 0)
            {
                fnSistema.spOpcionesGrilla(viewMaestro);
                Columnas();
                CargarCampos();
            }

            ComboOpcion(txtAgrupaCombo);
            ComboColumnas(txtColEsquema, "KeyInfo", "descInfo");
            ComboFormato(txtFormato, "KeyInfo", "descInfo");
            ComboSeparador(txtSeparador, "KeyInfo", "descInfo");
        }

        private void txtCodigo_KeyPress(object sender, KeyPressEventArgs e)
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

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Tab)
            {
                if (txtCodigo.ContainsFocus)
                {
                    Cuenta cu = new Cuenta();

                    if (txtCodigo.Text == "")
                    { lblMessage.Visible = true; lblMessage.Text = "Por favor ingresa un código"; return false; }

                    if (cu.ExisteCuenta(Convert.ToInt32(txtCodigo.Text), EsquemaSeleccionado))
                    {
                        lblMessage.Visible = true; lblMessage.Text = "Por favor verifica que el codigo ingresado no exista";
                        return false;
                    }

                    lblMessage.Visible = false;
                }
            }

            return base.ProcessDialogKey(keyData);
        }

        private void gridMaestro_DoubleClick(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();
            DeshabilitarControlesEsquema(false);

            Cuenta cu = new Cuenta();
            Esquema esq = new Esquema();
            int codCuenta = 0;
            string Name = "";
            if (viewMaestro.RowCount > 0)
            {
                codCuenta = Convert.ToInt32(viewMaestro.GetFocusedDataRow()["codCuenta"]);
                Name = (string)viewMaestro.GetFocusedDataRow()["descCuenta"];
                esq.SetInfo(EsquemaSeleccionado);
                if (cu.ExisteCuenta(codCuenta, EsquemaSeleccionado))
                {
                    frmElementosContables ele = new frmElementosContables(codCuenta, Name, esq);
                    ele.StartPosition = FormStartPosition.CenterScreen;
                    ele.ShowDialog();
                }
                else
                {
                    XtraMessageBox.Show("Registro no válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }

            }
            else
            {
                XtraMessageBox.Show("No se encontraron registros", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }

           
        }

        private void textEdit2_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void btnGuardarEsquema_Click(object sender, EventArgs e)
        {
            Esquema Esq = new Esquema();
            int cod = 0, columnas = 0;
            DetalleCuenta detalle = new DetalleCuenta();

            Cursor.Current = Cursors.WaitCursor;

            if (txtColEsquema.EditValue == null || txtColEsquema.Properties.DataSource == null)
            { XtraMessageBox.Show("Por favor ingresar la cantidad de columnas", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtColEsquema.Focus(); return; }

            if (txtFormato.EditValue == null || txtFormato.Properties.DataSource == null)
            { XtraMessageBox.Show("Por favor selecciona un formato", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtFormato.Focus(); return; }

            if (txtSeparador.EditValue == null || txtSeparador.Properties.DataSource == null)
            { XtraMessageBox.Show("Por favor selecciona un tipo de separador", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtSeparador.Focus(); return; }

            //Actualizar
            if (UpdateEsquema)
            {
                if (viewEsquema.RowCount == 0)
                { XtraMessageBox.Show("Registro no válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                if (viewEsquema.RowCount > 0)
                {
                    cod = Convert.ToInt32(viewEsquema.GetFocusedDataRow()["cod"]);
                    columnas = Convert.ToInt32(viewEsquema.GetFocusedDataRow()["col"]);
                    if (Esq.Existe(cod) == false)
                    { XtraMessageBox.Show("Registro no válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                    if (columnas != Convert.ToInt32(txtColEsquema.EditValue))
                    {
                        DialogResult Adv = XtraMessageBox.Show("haz cambiado la cantidad de columnas del esquema.\nSi continuas las configuraciones serán reestablecidas, ¿Deseas continuar de todas maneras?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (Adv == DialogResult.Yes)
                        {
                            //HACEMOS MODIFICACION
                            if (ModificaTransaccion(cod, txtDescEsquema.Text, Convert.ToInt32(txtColEsquema.EditValue), Convert.ToInt32(txtFormato.EditValue), Convert.ToInt32(txtSeparador.EditValue)))
                            {
                                XtraMessageBox.Show($"Esquema {txtDescEsquema.Text} registrado correctamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                fnSistema.spllenaGridView(gridEsquema, SqlGrillaEsquema);
                                if (viewEsquema.RowCount > 0)
                                {
                                    fnSistema.spOpcionesGrilla(viewEsquema);
                                    ColumnasEsquema();
                                    CargarCamposEsq();
                                    btnNuevoEsquema.Text = "Nuevo";
                                    btnNuevoEsquema.ToolTip = "Nuevo registro";
                                    CancelEsq = false;
                                }
                            }
                            else
                            {
                                XtraMessageBox.Show("No se pudo guardar registro", "información", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            }
                        }
                    }
                    else
                    {
                        //SOLO ACTUALIZAMOS
                        if (Esq.Modificar(cod, txtDescEsquema.Text, Convert.ToInt32(txtColEsquema.EditValue), Convert.ToInt32(txtFormato.EditValue), Convert.ToInt32(txtSeparador.EditValue)))
                        {
                            XtraMessageBox.Show($"Esquema {txtDescEsquema.Text} registrado correctamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            fnSistema.spllenaGridView(gridEsquema, SqlGrillaEsquema);
                            if (viewEsquema.RowCount > 0)
                            {
                                fnSistema.spOpcionesGrilla(viewEsquema);
                                ColumnasEsquema();
                                CargarCamposEsq();
                                btnNuevoEsquema.Text = "Nuevo";
                                btnNuevoEsquema.ToolTip = "Nuevo registro";
                                CancelEsq = false;
                            }
                        }
                        else
                        {
                            XtraMessageBox.Show("No se pudo guardar registro", "información", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        }
                    }                    
                }
            }
            else
            {
                if (txtCodEsquema.Text == "")
                { XtraMessageBox.Show("Por favor ingresa un codigo válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtCodEsquema.Focus(); return; }

                if (Esq.Existe(Convert.ToInt32(txtCodEsquema.Text)))
                { XtraMessageBox.Show($"Ya existe un registro con codigo {txtCodEsquema.Text}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtCodEsquema.Focus(); return; }

                if (Esq.Ingresar(Convert.ToInt32(txtCodEsquema.EditValue), txtDescEsquema.Text, Convert.ToInt32(txtColEsquema.EditValue), Convert.ToInt32(txtFormato.EditValue),Convert.ToInt32(txtSeparador.EditValue)))
                {
                    XtraMessageBox.Show("Registro ingresado correctamente", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    //Cargamos grilla
                    fnSistema.spllenaGridView(gridEsquema, SqlGrillaEsquema);
                    if (viewEsquema.RowCount > 0)
                    {
                        fnSistema.spOpcionesGrilla(viewEsquema);
                        ColumnasEsquema();
                        CargarCamposEsq();
                        btnNuevoEsquema.Text = "Nuevo";
                        btnNuevoEsquema.ToolTip = "Nuevo registro";
                        CancelEsq = false;
                    }
                }
                else
                {
                    XtraMessageBox.Show("No se pudo realizar registro", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }            
        }

        private void btnEliminarEsquema_Click(object sender, EventArgs e)
        {
            int cod = 0;
            string Desc = "";
            Esquema esq = new Esquema();
            if (viewEsquema.RowCount > 0)
            {
                cod = Convert.ToInt32(viewEsquema.GetFocusedDataRow()["cod"]);
                Desc = (string)viewEsquema.GetFocusedDataRow()["descEsq"];
                if (esq.Existe(cod) == false)
                { XtraMessageBox.Show("Registro no válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); }

                if (esq.Usado(cod))
                {
                    DialogResult Adv = XtraMessageBox.Show("Este registro está siendo usado. ¿Deseas eliminar de todas maneras?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (Adv == DialogResult.Yes)
                    {
                        //ELIMINAMOS...
                        if (esq.Eliminar(cod, Desc))
                        {
                            XtraMessageBox.Show("Registro eliminado correctamente", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            //CARGAMOS GRILLA...
                            fnSistema.spllenaGridView(gridEsquema, SqlGrillaEsquema);
                            if (viewEsquema.RowCount > 0)
                            {
                                fnSistema.spOpcionesGrilla(viewEsquema);
                                ColumnasEsquema();
                                CargarCamposEsq();
                            }
                            else
                            {
                                DeshabilitarControles(true);
                                LimpiarEsq();
                            }
                        }
                        else
                        {
                            XtraMessageBox.Show("Error al intentar eliminar registro", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                else
                {
                    //SOLO ELIMINAMOS
                    DialogResult Adv = XtraMessageBox.Show("¿Estás seguro de eliminar este registro?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (Adv == DialogResult.Yes)
                    {
                        //ELIMINAMOS...
                        if (esq.Eliminar(cod, Desc))
                        {
                            XtraMessageBox.Show("Registro eliminado correctamente", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            //CARGAMOS GRILLA...
                            fnSistema.spllenaGridView(gridEsquema, SqlGrillaEsquema);
                            if (viewEsquema.RowCount > 0)
                            {
                                fnSistema.spOpcionesGrilla(viewEsquema);
                                ColumnasEsquema();
                                CargarCamposEsq();
                            }
                            else
                            {
                                DeshabilitarControles(true);
                                LimpiarEsq();
                            }
                        }
                        else
                        {
                            XtraMessageBox.Show("Error al intentar eliminar registro", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            else
            {
                XtraMessageBox.Show("Registro no válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void btnNuevoEsquema_Click(object sender, EventArgs e)
        {
            if (CancelEsq)
            {
                btnNuevoEsquema.Text = "Nuevo";
                btnNuevoEsquema.ToolTip = "Nuevo registro";
                CancelEsq = false;
                CargarCamposEsq();
                //DeshabilitarControles(false);
            }
            else
            {
                //INGRESAR UN ELEMENTO NUEVO

                btnNuevoEsquema.Text = "Cancelar";
                btnNuevoEsquema.ToolTip = "Cancelar operacion";
                CancelEsq = true;
                LimpiarEsq();
                DeshabilitarControles(true);                      
            }
        }

        private void gridEsquema_Click(object sender, EventArgs e)
        {
            if (viewEsquema.RowCount > 0)
            {
                CargarCamposEsq();
                btnNuevoEsquema.Text = "Nuevo";
                btnNuevoEsquema.ToolTip = "Nuevo registro";
                CancelEsq = false;
            }
        }

        private void viewMaestro_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();
            DeshabilitarControlesEsquema(false);

            DXPopupMenu menu = e.Menu;
            string nombre = "";
            if (menu != null)
            {
                if (viewMaestro.RowCount > 0) nombre = (string)viewMaestro.GetFocusedDataRow()["descCuenta"];

                DXMenuItem submenu = new DXMenuItem($"Configurar cuenta {nombre}", new EventHandler(ConfigurarCuenta_Click));
                submenu.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("images/actions/show_16x16.png");
                menu.Items.Clear();
                menu.Items.Add(submenu);
            }
        }

        private void ConfigurarCuenta_Click(object sender, EventArgs e)
        {
            Cuenta cu = new Cuenta();
            Esquema esq = new Esquema();
            int codCuenta = 0;
            string Name = "";
            if (viewMaestro.RowCount > 0)
            {
                codCuenta = Convert.ToInt32(viewMaestro.GetFocusedDataRow()["codCuenta"]);
                Name = (string)viewMaestro.GetFocusedDataRow()["descCuenta"];
                esq.SetInfo(EsquemaSeleccionado);
                if (cu.ExisteCuenta(codCuenta, EsquemaSeleccionado))
                {
                    frmElementosContables ele = new frmElementosContables(codCuenta, Name, esq);
                    ele.StartPosition = FormStartPosition.CenterScreen;
                    ele.ShowDialog();
                }
                else
                {
                    XtraMessageBox.Show("Registro no válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }

            }
            else
            {
                XtraMessageBox.Show("No se encontraron registros", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }
    }
}