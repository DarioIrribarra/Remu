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
using Excel = Microsoft.Office.Interop.Excel;

namespace Labour
{
    public partial class frmCargaFamiliar : DevExpress.XtraEditors.XtraForm
    {
        [DllImport("user32")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32")]
        static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uMenu);

        const int MF_BYCOMMAND = 0;
        const int MF_DISABLED = 2;
        const int SC_CLOSED = 0xF060;

        //PARA GUARDAR EL NUMERO DE CONTRATO DEL TRABAJADOR 
        private string contratoTitular = "";

        //PARA UPDATED
        private bool Updated = false;

        //PARA BOTON NUEVO REGISTRO
        private bool cancela = false;

        //BOTON NUEVO
        Operacion op;

        //CONSTRUCTOR PARAMETRIZADO (PARA CONTRATO)
        public frmCargaFamiliar(string contrato)
        {
            InitializeComponent();
            contratoTitular = contrato;
        }
        public frmCargaFamiliar()
        {
            InitializeComponent();
        }
        private void panelControl1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void frmCargaFamiliar_Load(object sender, EventArgs e)
        {           
            var sm = GetSystemMenu(Handle, false);
            EnableMenuItem(sm, SC_CLOSED, MF_BYCOMMAND | MF_DISABLED);

            if (contratoTitular != "")
            {
                lblCargasFamiliares.Text = "CONTRATO N° " + contratoTitular + " - " + fnSistema.GetNombreTrabajador(contratoTitular);
                string grilla = string.Format("select rutCarga, nombre, sexo, fechaNac, parentesco, relacionLegal, fechaIngreso," +
                            " fechaTermino, invalido, maternal FROM CargaFamiliar WHERE contrato ='{0}'", contratoTitular);

                op = new Operacion();                

                fnSistema.spllenaGridView(gridFamiliares, grilla);
                if (viewFamiliares.RowCount > 0)
                {
                    fnSistema.spOpcionesGrilla(viewFamiliares);
                    fnColumnasGrilla();
                }          

                //COMBOS
                fnComboParentesco(txtParentesco);
                fnComboRelacionLegal(txtRelacionLegal);
                fnComboSexo(txtSexo);
                Propiedades();

                CargarCampos(0);
            }               
        }

        #region "MANEJO DE DATOS"
        //INGRESO NUEVO REGISTRO
        private void NuevaCargaFamiliar(string contrato, TextEdit rutCarga, TextEdit nombre, LookUpEdit sexo, DateEdit FechaNac,
            LookUpEdit parentesco, LookUpEdit relacionLegal, DateEdit fechaIngreso, DateEdit fechaTermino, 
            CheckEdit pMaternal, CheckEdit pInvalido)
        {
            string sql = "INSERT INTO CARGAFAMILIAR(contrato, rutCarga, nombre, sexo, fechanac, parentesco, " +
                "relacionLegal, fechaIngreso, fechaTermino, invalido, maternal) VALUES(@contrato, @rutCarga, @nombre, @sexo, @fechanac, " +
                "@parentesco, @relacionLegal, @fechaIngreso, @fechaTermino, @pMaternal, @pInvalido)";

            string grilla = "";
            SqlCommand cmd;
            int res = 0;

            if (FechaNac.DateTime > DateTime.Now)
            {
                XtraMessageBox.Show("Por favor verifica la fecha de nacimiento", "Fecha de nacimiento", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            if (fechaIngreso.DateTime > DateTime.Now)
            {
                XtraMessageBox.Show("Por favor verifica la fecha de ingreso de la carga", "Ingreso", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            if (fechaIngreso.DateTime > fechaTermino.DateTime)
            {
                XtraMessageBox.Show("Por favor verifica que la fecha de ingreso de la carga familiar no sea mayor a la fecha de termino", "Fechas incorrectas", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@contrato", contrato));
                        cmd.Parameters.Add(new SqlParameter("@rutCarga", (rutCarga.Text.Contains(".") || rutCarga.Text.Contains("-"))?fnSistema.fnExtraerCaracteres(rutCarga.Text):rutCarga.Text));
                        cmd.Parameters.Add(new SqlParameter("@nombre", nombre.Text));
                        cmd.Parameters.Add(new SqlParameter("@sexo", Convert.ToInt16(sexo.EditValue)));
                        cmd.Parameters.Add(new SqlParameter("@fechanac", Convert.ToDateTime(FechaNac.EditValue)));
                        cmd.Parameters.Add(new SqlParameter("@parentesco", Convert.ToInt16(parentesco.EditValue)));
                        cmd.Parameters.Add(new SqlParameter("@relacionLegal", Convert.ToInt16(relacionLegal.EditValue)));
                        cmd.Parameters.Add(new SqlParameter("@fechaIngreso", Convert.ToDateTime(fechaIngreso.EditValue)));
                        cmd.Parameters.Add(new SqlParameter("@fechaTermino", Convert.ToDateTime(fechaTermino.EditValue)));
                        cmd.Parameters.Add(new SqlParameter("@pMaternal", pMaternal.Checked ? 1: 0));
                        cmd.Parameters.Add(new SqlParameter("@pInvalido", pInvalido.Checked ? 1: 0));

                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            XtraMessageBox.Show("Registro Guardado correctamente", "Carga Familiar", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            //GUARDAMOS EVENTO EN LOG
                            logRegistro log = new logRegistro(User.getUser(), "INGRESA NUEVA CARGA FAMILIAR ASOCIADA A CONTRATO " + contrato, "CARGAFAMILIAR", "0", nombre.Text, "INGRESAR");
                            log.Log();

                            grilla = string.Format("select rutCarga, nombre, sexo, fechaNac, parentesco, relacionLegal, fechaIngreso," +
                           " fechaTermino, invalido, maternal FROM CargaFamiliar WHERE contrato ='{0}'", contratoTitular);

                            fnSistema.spllenaGridView(gridFamiliares, grilla);
                            fnSistema.spOpcionesGrilla(viewFamiliares);
                            fnColumnasGrilla();
                            txtRut.Properties.Appearance.BackColor = Color.White;
                            CargarCampos();

                            op.Cancela = false;
                            op.SetButtonProperties(btnNuevo, 1);

                        }
                        else
                        {
                            XtraMessageBox.Show("Error al intentar Guardar", "Carga Familiar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

        }

        //MODIFICAR REGISTRO
        private void ModificarCargaFamiliar(string contrato, TextEdit rutCarga, TextEdit nombre, LookUpEdit sexo, DateEdit FechaNac,
            LookUpEdit parentesco, LookUpEdit relacionLegal, DateEdit fechaIngreso, DateEdit fechaTermino, 
            CheckEdit pMaternal, CheckEdit pInvalido)
        {
            string sql = "UPDATE CARGAFAMILIAR SET nombre=@nombre, sexo=@sexo, fechanac=@fechanac," +
                "parentesco=@parentesco, relacionLegal=@relacionLegal, fechaIngreso=@fechaIngreso, " +
                "fechaTermino=@fechaTermino, invalido=@pInvalido, maternal=@pMaternal" +
                " WHERE contrato=@contrato AND rutCarga=@rutCarga";

            string grilla = "";
            bool HijoMayor = false;
            SqlCommand cmd;
            int res = 0;

            if (FechaNac.DateTime > DateTime.Now)
            {
                XtraMessageBox.Show("Por favor verifica la fecha de nacimiento", "Fecha de nacimiento", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            if (fechaIngreso.DateTime > DateTime.Now)
            {
                XtraMessageBox.Show("Por favor verifica la fecha de ingreso de la carga", "Ingreso", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            if (fechaIngreso.DateTime > fechaTermino.DateTime)
            {
                XtraMessageBox.Show("Por favor verifica que la fecha de ingreso de la carga familiar no sea mayor a la fecha de termino", "Fechas incorrectas", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            //TABLA HASH
            Hashtable DatosCarga = new Hashtable();
            DatosCarga = PrecargaFamiliar(contrato, fnSistema.fnExtraerCaracteres(rutCarga.Text));

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@nombre", nombre.Text));
                        cmd.Parameters.Add(new SqlParameter("@sexo", Convert.ToInt16(sexo.EditValue)));
                        cmd.Parameters.Add(new SqlParameter("@fechanac", Convert.ToDateTime(FechaNac.EditValue)));
                        cmd.Parameters.Add(new SqlParameter("@parentesco", Convert.ToInt16(parentesco.EditValue)));
                        cmd.Parameters.Add(new SqlParameter("@relacionLegal", Convert.ToInt16(relacionLegal.EditValue)));
                        cmd.Parameters.Add(new SqlParameter("@fechaIngreso", Convert.ToDateTime(fechaIngreso.EditValue)));
                        cmd.Parameters.Add(new SqlParameter("@fechaTermino", Convert.ToDateTime(fechaTermino.EditValue)));
                        cmd.Parameters.Add(new SqlParameter("@contrato", contrato));
                        cmd.Parameters.Add(new SqlParameter("@rutCarga", fnSistema.fnExtraerCaracteres(rutCarga.Text)));
                        cmd.Parameters.Add(new SqlParameter("@pMaternal", pMaternal.Checked ? 1:0));
                        cmd.Parameters.Add(new SqlParameter("@pInvalido", pInvalido.Checked ? 1:0));

                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            XtraMessageBox.Show("Registro Actualizado Correctamente", "Carga Familiar", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            //VERIFICAMOS SI HAY CAMBIOS EN ALGUN DATO, GUARDAMOS EVENTO EN LOG
                            ComparaFamiliar(contrato, fnSistema.fnExtraerCaracteres(rutCarga.Text), nombre.Text, Convert.ToInt16(sexo.EditValue), Convert.ToDateTime(FechaNac.EditValue),
                                Convert.ToInt16(parentesco.EditValue), Convert.ToInt16(relacionLegal.EditValue), Convert.ToDateTime(fechaIngreso.EditValue), 
                                Convert.ToDateTime(fechaTermino.EditValue), DatosCarga, pMaternal.Checked ? 1:0, pInvalido.Checked ? 1:0);

                            grilla = string.Format("select rutCarga, nombre, sexo, fechaNac, parentesco, relacionLegal, fechaIngreso," +
                            " fechaTermino, invalido, maternal FROM CargaFamiliar WHERE contrato ='{0}'", contratoTitular);

                            fnSistema.spllenaGridView(gridFamiliares, grilla);
                            fnColumnasGrilla();
                            txtRut.Properties.Appearance.BackColor = Color.White;
                            CargarCampos();
                        }
                        else
                        {
                            XtraMessageBox.Show("Error al intentar Actualizar", "Carga Familiar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        //ELIMINAR REGISTRO 
        private void EliminarCargaFamiliar(string contrato, string rutCarga)
        {
            string sql = "DELETE FROM CARGAFAMILIAR WHERE contrato=@contrato AND rutCarga=@rutCarga";
            string grilla = "";
            SqlCommand cmd;
            int res = 0;
            DialogResult dialogo = XtraMessageBox.Show("¿Realmente desea eliminar el registro con rut " + fnSistema.fFormatearRut2(rutCarga) + "?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogo == DialogResult.Yes)
            {
                try
                {
                    if (fnSistema.ConectarSQLServer())
                    {
                        using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@contrato", contrato));
                            cmd.Parameters.Add(new SqlParameter("@rutCarga", rutCarga));

                            res = cmd.ExecuteNonQuery();
                            if (res > 0)
                            {
                                XtraMessageBox.Show("Eliminado correctamente", "Carga Familiar", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                //GUARDAR EVENTO EN LOG
                                logRegistro log = new logRegistro(User.getUser(), "ELIMINA CARGA FAMILIAR RUT " + rutCarga + " ASOCIADA A CONTRATO " + contrato, "CARGAFAMILIAR", rutCarga, "0", "ELIMINA");
                                log.Log();

                                grilla = string.Format("select rutCarga, nombre, sexo, fechaNac, parentesco, relacionLegal, fechaIngreso," +
                               " fechaTermino, invalido, maternal FROM CargaFamiliar WHERE contrato ='{0}'", contratoTitular);

                                fnSistema.spllenaGridView(gridFamiliares, grilla);
                                fnLimpiarCampos();
                                fnColumnasGrilla();
                                txtRut.Properties.Appearance.BackColor = Color.White;
                                CargarCampos(0);
                            }
                            else
                            {
                                XtraMessageBox.Show("Error al intentar Eliminar", "Carga Familiar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                    }
                }
                catch (SqlException ex)
                {
                    XtraMessageBox.Show(ex.Message);
                }
            }
            
        }

        //COLUMNAS GRILLA
        private void fnColumnasGrilla()
        {
            if (viewFamiliares.RowCount > 0)
            {
                viewFamiliares.Columns[0].Caption = "Rut";
                viewFamiliares.Columns[0].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                viewFamiliares.Columns[0].DisplayFormat.FormatString = "Rut";
                viewFamiliares.Columns[0].DisplayFormat.Format = new FormatCustom();

                viewFamiliares.Columns[1].Caption = "Nombre";
                viewFamiliares.Columns[1].Width = 200;

                viewFamiliares.Columns[2].Caption = "Sexo";
                viewFamiliares.Columns[2].Visible = false;

                viewFamiliares.Columns[3].Caption = "Nacimiento";
                viewFamiliares.Columns[4].Caption = "Parentesco";
                viewFamiliares.Columns[4].Visible = false;

                viewFamiliares.Columns[5].Caption = "Relacion";
                viewFamiliares.Columns[5].Visible = false;

                viewFamiliares.Columns[6].Caption = "Ingreso";
                viewFamiliares.Columns[7].Caption = "Termino";

                //MATERNAL E INVALIDO
                viewFamiliares.Columns[8].Visible = false;
                viewFamiliares.Columns[9].Visible = false;
            }            
        }

        //COMBO SEXO
        //0--> M
        //1--> F
        private void fnComboSexo(LookUpEdit pCombo)
        {
            List<PruebaCombo> listado = new List<PruebaCombo>();

            listado.Add(new PruebaCombo() { key = 0, desc = "M" });
            listado.Add(new PruebaCombo() { key = 1, desc = "F" });

            //PROPIEDADES COMBOBOX
            pCombo.Properties.DataSource = listado.ToList();
            pCombo.Properties.ValueMember = "key";
            pCombo.Properties.DisplayMember = "desc";

            pCombo.Properties.PopulateColumns();

            //ocultamos la columan key
            pCombo.Properties.Columns[0].Visible = false;

            pCombo.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
            pCombo.Properties.AutoSearchColumnIndex = 1;
            pCombo.Properties.ShowHeader = false;
        }

        //COMBO PARENTESCO
        private void fnComboParentesco(LookUpEdit pCombo)
        {
            List<PruebaCombo> listado = new List<PruebaCombo>();

            listado.Add(new PruebaCombo() { key = 0, desc = "CONYUGE" });
            listado.Add(new PruebaCombo() { key = 1, desc = "HIJO(A)" });
            listado.Add(new PruebaCombo() { key = 2, desc = "MADRE" });
            listado.Add(new PruebaCombo() { key = 3, desc = "PADRE" });

            //PROPIEDADES COMBOBOX
            pCombo.Properties.DataSource = listado.ToList();
            pCombo.Properties.ValueMember = "key";
            pCombo.Properties.DisplayMember = "desc";

            pCombo.Properties.PopulateColumns();

            //ocultamos la columan key
            pCombo.Properties.Columns[0].Visible = false;

            pCombo.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
            pCombo.Properties.AutoSearchColumnIndex = 1;
            pCombo.Properties.ShowHeader = false;
        }

        //COMBO RELACION LEGAL
        private void fnComboRelacionLegal(LookUpEdit pCombo)
        {
            List<PruebaCombo> listado = new List<PruebaCombo>();

            listado.Add(new PruebaCombo() { key = 0, desc = "CONYUGE/CONVIVIENTE" });
            listado.Add(new PruebaCombo() { key = 1, desc = "HIJO(A)" });
            listado.Add(new PruebaCombo() { key = 2, desc = "ADHERENTE" });            

            //PROPIEDADES COMBOBOX
            pCombo.Properties.DataSource = listado.ToList();
            pCombo.Properties.ValueMember = "key";
            pCombo.Properties.DisplayMember = "desc";

            pCombo.Properties.PopulateColumns();

            //ocultamos la columan key
            pCombo.Properties.Columns[0].Visible = false;

            pCombo.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
            pCombo.Properties.AutoSearchColumnIndex = 1;
            pCombo.Properties.ShowHeader = false;
        }

        //MANIPULAR LA TECLA TAB
        protected override bool ProcessDialogKey(Keys keyData)
        {
            bool valido = false;
            bool Existe = false;
            string cadena = "";
            if (keyData == Keys.Tab)
            {
                if (txtRut.ContainsFocus)
                {
                    ValidaCajaRut();
                }
                else if (txtNombre.ContainsFocus)
                {
                    if (txtNombre.Text == "")
                    {
                        lblmsg.Visible = true;
                        lblmsg.Text = "Por favor ingresa un nombre";
                        txtNombre.Focus();
                        return false;
                    }
                    else
                    {
                        lblmsg.Visible = false;
                    }                    
                }
                else if (dtIngreso.ContainsFocus)
                {
                    //SUMAMOS 18 A LA FECHA DE INGRESO
                    DateTime fechaFinal = DateTime.Now.Date;
                    fechaFinal = dtIngreso.DateTime.AddYears(18);
                    dtTermino.DateTime = fechaFinal;
                }
                else if (dtNacimiento.ContainsFocus)
                {
                    dtIngreso.DateTime = dtNacimiento.DateTime;
                    
                }
            }
            return base.ProcessDialogKey(keyData);
        }

        //VERIFICAR SI EXISTE REGISTRO CON ESE RUT
        private bool fnExisteRegistro(string contrato, string RutCarga)
        {
            string sql = "SELECT rutCarga FROM CARGAFAMILIAR WHERE contrato=@contrato AND rutCarga=@rutCarga";
            bool existe = false;
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
                        cmd.Parameters.Add(new SqlParameter("@rutCarga", RutCarga));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //YA EXISTE
                            existe = true;
                        }
                        else
                        {
                            existe = false;
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
            return existe;
        }

        //PROPIEDADES
        private void Propiedades()
        {
            dtTermino.EditValue = DateTime.Now.Date;            
            dtIngreso.EditValue = DateTime.Now.Date;
            dtNacimiento.EditValue = DateTime.Now.Date;
            dtIngreso.Properties.MaxValue = DateTime.Now.Date;
            dtNacimiento.Properties.MaxValue = DateTime.Now.Date;
            txtParentesco.ItemIndex = 0;
            txtRelacionLegal.ItemIndex = 0;
            txtSexo.ItemIndex = 0;           
            cbInvalido.Checked = false;
            cbMaternal.Checked = false;
        }

        //LIMPIAR CAMPOS
        private void fnLimpiarCampos()
        {
            txtRut.Text = "";
            txtRut.Focus();
            txtNombre.Text = "";
            txtParentesco.ItemIndex = 0;
            txtRelacionLegal.ItemIndex = 0;
            txtSexo.ItemIndex = 0;
            dtTermino.EditValue = DateTime.Now;
            dtIngreso.EditValue = DateTime.Now;
            dtNacimiento.EditValue = DateTime.Now;
            Updated = false;
            txtRut.ReadOnly = false;
            cbNoAplica.Checked = false;
            cbNoAplica.ReadOnly = false;
            lblmsg.Visible = false;
            cbInvalido.Checked = false;
            cbMaternal.Checked = false;
        }

        //VALIDACION COMPLETA RUT
        private bool ValidacionCompletaRut(string rut)
        {
            bool valido = false;            
         
            if (rut == "")
            {
                return false;
            }
            else
            {                
                if (rut.Length < 8 || rut.Length > 9)
                {
                    return false;
                }
                else
                {
                    if (rut.Contains(".") || rut.Contains("-"))
                    {
                        rut = fnSistema.fnExtraerCaracteres(rut);
                    }
                    
                    //ENMASCARAMOS
                    rut = fnSistema.fEnmascaraRut(rut);                 
                    valido = fnSistema.fValidaRut(rut);                
                    if (valido == false)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }

                }
            }
        }

        //CARGAR CAMPOS
        private void CargarCampos(int? fila = -1)
        {
            int maternal = 0, invalido = 0;
            if (viewFamiliares.RowCount > 0)
            {
                if (fila != -1)
                    viewFamiliares.FocusedRowHandle = (int)fila;

                lblmsg.Visible = false;

                txtRut.ReadOnly = true;
                Updated = true;
                cbNoAplica.ReadOnly = true;

                btnImprimeExcel.Enabled = true;
                btnImprimirPdf.Enabled = true;
                btnPdf.Enabled = true;
                btnImpresionRapida.Enabled = true;
                btnEliminar.Enabled = true;

                txtRut.Text = fnSistema.fFormatearRut2(viewFamiliares.GetFocusedDataRow()["rutcarga"] + "");
                txtNombre.Text = viewFamiliares.GetFocusedDataRow()["nombre"] + "";
                txtSexo.EditValue = viewFamiliares.GetFocusedDataRow()["sexo"];
                txtParentesco.EditValue = viewFamiliares.GetFocusedDataRow()["parentesco"];
                txtRelacionLegal.EditValue = viewFamiliares.GetFocusedDataRow()["relacionlegal"];
                dtTermino.EditValue = viewFamiliares.GetFocusedDataRow()["fechaTermino"];                
                dtIngreso.EditValue = viewFamiliares.GetFocusedDataRow()["fechaIngreso"];
                dtNacimiento.EditValue = viewFamiliares.GetFocusedDataRow()["fechanac"];
                maternal = Convert.ToInt16(viewFamiliares.GetFocusedDataRow()["maternal"]);
                invalido = Convert.ToInt16(viewFamiliares.GetFocusedDataRow()["invalido"]);

                if (maternal == 0)
                    cbMaternal.Checked = false;
                else
                    cbMaternal.Checked = true;

                if (invalido == 0)
                    cbInvalido.Checked = false;
                else
                    cbInvalido.Checked = true;
            }
            else
            {
                fnLimpiarCampos();
            }
        }

        //HIJO MAYOR DE 24 AÑOS
        private bool HijoMayor24(DateTime nacimiento, Int16 parentesco)
        {
            int edad = 0;
            edad = DateTime.Now.Year - nacimiento.Year;
            //1 --> HIJO(A)
            if (parentesco == 1)
            {
                if (edad>24)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        //MOSTRAR DOCUMENTO PDF
        private void ImprimirPdf(string contrato, bool? ImpresionRapida = false, bool? GeneraPdf = false)
        {
            string sql = "select DISTINCT CONCAT(trabajador.nombre, ' ', apepaterno, ' ', apematerno) as trabajador," +
                " cargafamiliar.contrato, rutcarga, CargaFamiliar.nombre, CargaFamiliar.sexo, cargafamiliar.fechanac," +
                " parentesco, relacionlegal, fechaingreso, fechatermino, trabajador.rut as rutTrabajador " +
                " FROM cargafamiliar INNER JOIN trabajador ON trabajador.contrato = cargafamiliar.contrato" +
                " where CargaFamiliar.contrato = @contrato";

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
                        ad.SelectCommand = cmd;
                        ad.Fill(ds, "data");

                        ad.Dispose();
                        cmd.Dispose();
                        fnSistema.sqlConn.Close();                        

                        if (ds.Tables[0].Rows.Count>0)
                        {                            
                            //RECORREMOS DATATABLE Y FORMATEAMOS CON PUNTO Y GUION EL RUT
                            foreach (DataRow fila in ds.Tables[0].Rows)
                            {
                                foreach (DataColumn columna in ds.Tables[0].Columns)
                                {
                                    if (columna.ToString() == "rutcarga")
                                        fila[columna] = fnSistema.fFormatearRut2(fila[columna].ToString());                                   
                                }    
                            }

                            //rptCargaFamiliar reporte = new rptCargaFamiliar();
                            //Reporte externo
                            ReportesExternos.rptCargaFamiliar reporte = new ReportesExternos.rptCargaFamiliar();
                            reporte.DataSource = ds.Tables[0];
                            reporte.DataMember = "data";

                            Empresa emp = new Empresa();
                            emp.SetInfo();

                            foreach (DevExpress.XtraReports.Parameters.Parameter parametro in reporte.Parameters)
                            {
                                parametro.Visible = false;
                            }

                            reporte.Parameters["empresa"].Value = emp.Razon;
                            reporte.Parameters["rutEmpresa"].Value = fnSistema.fFormatearRut2(emp.Rut);
                            reporte.Parameters["imagen"].Value = Imagen.GetLogoFromBd();

                            Documento docu = new Documento("", 0);

                            if ((bool)ImpresionRapida)
                                docu.PrintDocument(reporte);
                            else if ((bool)GeneraPdf)
                                docu.ExportToPdf(reporte, $"Cargas_{contrato}");
                            else
                                docu.ShowDocument(reporte);

                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        //GENERA DATATABLE PARA CLASE EXCEL
        private DataTable GeneraTablaExcel(string contrato)
        {
            string sql = "select DISTINCT CONCAT(trabajador.nombre, ' ', apepaterno, ' ', apematerno) as trabajador," +
           " cargafamiliar.contrato, rutcarga, CargaFamiliar.nombre, CargaFamiliar.sexo, cargafamiliar.fechanac," +
           " parentesco, relacionlegal, fechaingreso, fechatermino " +
           " FROM cargafamiliar INNER JOIN trabajador ON trabajador.contrato = cargafamiliar.contrato" +
           " where CargaFamiliar.contrato = @contrato";

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

                        ad.SelectCommand = cmd;
                        ad.Fill(ds, "data");
                        ad.Dispose();
                        cmd.Dispose();
                        fnSistema.sqlConn.Close();

                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            return ds.Tables[0];
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return null;
        }

        //GENERA INFORMA EN EXCEL
        private void GeneraExcelCargas(string contrato)
        {
            string sql = "select DISTINCT CONCAT(trabajador.nombre, ' ', apepaterno, ' ', apematerno) as trabajador," +
             " cargafamiliar.contrato, rutcarga, CargaFamiliar.nombre, CargaFamiliar.sexo, cargafamiliar.fechanac," +
             " parentesco, relacionlegal, fechaingreso, fechatermino " +
             " FROM cargafamiliar INNER JOIN trabajador ON trabajador.contrato = cargafamiliar.contrato" +
             " where CargaFamiliar.contrato = @contrato";

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
                        ad.SelectCommand = cmd;
                        ad.Fill(ds, "data");

                        ad.Dispose();
                        cmd.Dispose();
                        fnSistema.sqlConn.Close();

                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            //PARA EXCEL
                            Excel._Application app = new Excel.Application();
                            //OBTENEMOS LOS LIBROS ACTUALES DEL EXCEL
                            Excel.Workbooks libros = app.Workbooks;
                            //CREAMOS UN NUEVO LIBRO
                            libros.Add(Type.Missing);

                            //ACCEDEMOS AL LIBRO
                            Excel._Workbook Libro = libros[0];
                            //GUARDAR LA HOJA
                            Excel._Worksheet Hoja = app.Worksheets[1];

                            //PARA OBTENER EL RANGO USADO
                            Excel.Range rango = Hoja.UsedRange;

                            //AUTO AJUSTAR LAS COLUMNAS
                            rango.Columns.AutoFit();

                            //RUTA DONDE SE GUARDARA EL ARCHIVO
                            string PathFile = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\CargaFamiliar_" + contrato + ".xlsx" ;

                            //OBTENER LAS COLUMNAS 
                            DataColumnCollection columnas = ds.Tables[0].Columns;

                            //CREAMOS CABECERAS PARA EXCEL DE ACUERDO A COLECCION DE COLUMNAS DESDE EL DATASET
                            for (int columna = 0; columna < columnas.Count; columna++)
                            {
                                app.Cells[1, columna + 1].Value = columnas[columna].ToString();
                            }

                            //RECORREMOS TODO EL DATASET Y AGREGAMOS TODO A LA HOJA DE EXCEL
                            for (int fila = 1; fila <= ds.Tables[0].Rows.Count; fila++)
                            {
                                for (int columna = 1; columna <= ds.Tables[0].Columns.Count; columna++)
                                {
                                    app.Cells[fila, columna].Value = ds.Tables[0].Rows[fila - 1][columna - 1].ToString();
                                }
                            }
                            
                            Libro.SaveCopyAs(PathFile);
                            Libro.Saved = true;

                            //LIMPIAMOS OBJETOS COM...

                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        private DataTable TablaFinal(DataTable miTabla)
        {
            DataTable tabla = new DataTable();

            tabla.Columns.Add("contrato", typeof(string));
            tabla.Columns.Add("rut", typeof(string));
            tabla.Columns.Add("nombre", typeof(string));
            tabla.Columns.Add("sexo", typeof(string));
            tabla.Columns.Add("nacimiento", typeof(DateTime));
            tabla.Columns.Add("parentesco", typeof(string));
            tabla.Columns.Add("relacionlegal", typeof(string));
            tabla.Columns.Add("fechaingreso", typeof(DateTime));
            tabla.Columns.Add("fechatermino", typeof(DateTime));

            //FILAS...
            if (miTabla.Rows.Count > 0)
            {
                string data = "";
                MyData m = new MyData();

                try
                {
                    //RECORREMOS TABLA
                    for (int i = 0; i < miTabla.Rows.Count; i++)
                    {
                        for (int x = 0; x < miTabla.Columns.Count; x++)
                        {
                            if (miTabla.Columns[x].ToString() == "sexo")
                            {
                                data = GetSexo(Convert.ToInt32(miTabla.Rows[i][x]));
                                m.sexo = data;
                            }
                            if (miTabla.Columns[x].ToString() == "parentesco")
                            {
                                data = GetParentesco(Convert.ToInt32(miTabla.Rows[i][x]));
                                m.parentesco = data;
                            }
                            if (miTabla.Columns[x].ToString() == "relacionlegal")
                            {
                                data = GetRelacion(Convert.ToInt32(miTabla.Rows[i][x]));
                                m.relacionlegal = data;
                            }
                            if (miTabla.Columns[x].ToString() == "fechanac")
                            {
                                m.nacimiento = (DateTime)miTabla.Rows[i][x];
                            }
                            if (miTabla.Columns[x].ToString() == "contrato")
                            {
                                m.contrato = (string)miTabla.Rows[i][x];
                            }
                            if (miTabla.Columns[x].ToString() == "rutcarga")
                            {
                                m.rut = (string)miTabla.Rows[i][x];
                            }
                            if (miTabla.Columns[x].ToString() == "nombre")
                            {
                                m.nombre = (string)miTabla.Rows[i][x];
                            }
                            if (miTabla.Columns[x].ToString() == "fechaingreso")
                            {
                                m.fechaingreso = (DateTime)miTabla.Rows[i][x];
                            }
                            if (miTabla.Columns[x].ToString() == "fechatermino")
                            {
                                m.fechatermino = (DateTime)miTabla.Rows[i][x];
                            }
                        }

                        //AGREGAMOS FILA
                        tabla.Rows.Add(m.contrato, m.rut, m.nombre, m.sexo, m.nacimiento, m.parentesco, m.relacionlegal,
                          m.fechaingreso, m.fechatermino);

                        //tabla.Rows.Add(m);
                    }

                    return tabla;
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show("ha ocurrido un error " + ex.Message, "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        //MANIPULAR SEXO
        private string GetSexo(int Sexo)
        {
            string data = "";
            if (Sexo == 0)
            {
                data = "Masculino";
            }
            else if (Sexo == 1)
            {
                data = "Femenino";
            }

            return data;
        }

        //MANIPULAR RELACION
        private string GetRelacion(int relacion)
        {
            string data = "";
            if (relacion == 0)
            {
                data = "Conyuge";
            }
            else if (relacion == 1)
            {
                data = "Hijo(A)";
            }
            else if (relacion == 2)
            {
                data = "Adherente";
            }

            return data;
        }

        //MANIPULAR PARENTESCO
        private string GetParentesco(int parentesco)
        {
            string data = "";
            if (parentesco == 0)
            {
                data = "Conyuge";
            }
            else if (parentesco == 1)
            {
                data = "Hijo(A)";
            }
            else if (parentesco == 2)
            {
                data = "Madre";
            }
            else if (parentesco == 3)
            {
                data = "Padre";
            }

            return data;
        }

        struct MyData
        {
            public string contrato { get; set; }
            public string rut { get; set; }
            public string nombre { get; set; }
            public string sexo { get; set; }
            public DateTime nacimiento { get; set; }
            public string parentesco { get; set; }
            public string relacionlegal { get; set; }
            public DateTime fechaingreso { get; set; }
            public DateTime fechatermino { get; set; }

        }
        //VERIFICAR SI HAY CAMBIOS ANTES DE CERRAR
        private bool CambiosSinGuardar(string pContrato, string pRutCarga)
        {
            string sql = "SELECT nombre, sexo, fechaNac, parentesco, relacionlegal, fechaingreso, fechatermino, maternal, invalido " +
                "FROM cargafamiliar WHERE contrato=@pContrato AND rutcarga=@pRutCarga";
            SqlCommand cmd;
            SqlDataReader rd;
            int mat = 0, invalido = 0;
            if (cbMaternal.Checked)
                mat = 1;
            else
                mat = 0;

            if (cbInvalido.Checked)
                invalido = 1;
            else
                invalido = 0;

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETRO
                        cmd.Parameters.Add(new SqlParameter("@pContrato", pContrato));
                        cmd.Parameters.Add(new SqlParameter("@pRutCarga", pRutCarga));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //COMPARAMOS
                                if (txtNombre.Text != (string)rd["nombre"]) return true;
                                if (Convert.ToInt16(txtSexo.EditValue) != Convert.ToInt16(rd["sexo"])) return true;
                                if (Convert.ToDateTime(dtNacimiento.DateTime) != Convert.ToDateTime(rd["fechaNac"])) return true;
                                if (Convert.ToInt32(txtParentesco.EditValue) != Convert.ToInt32(rd["parentesco"])) return true;
                                if (Convert.ToInt32(txtRelacionLegal.EditValue) != Convert.ToInt32(rd["relacionlegal"])) return true;
                                if (Convert.ToDateTime(dtIngreso.DateTime) != Convert.ToDateTime(rd["fechaingreso"])) return true;
                                if (Convert.ToDateTime(dtTermino.DateTime) != Convert.ToDateTime(rd["fechatermino"])) return true;
                                if (mat != Convert.ToInt16(rd["maternal"])) return true;
                                if (invalido != Convert.ToInt16(rd["invalido"])) return true;
                            }
                        }
                        cmd.Dispose();
                        rd.Close();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return false;
        }       
        

        #region "LOG FAMILIARES"
        //TABLA HASH PARA PRECARGA
        private Hashtable PrecargaFamiliar(string contrato, string rutCarga)
        {
            Hashtable tabla = new Hashtable();
            string sql = "SELECT nombre, sexo, fechaNac, parentesco, relacionLegal, fechaIngreso, fechaTermino, " +
                " invalido, maternal FROM " +
                "cargafamiliar WHERE contrato=@contrato AND rutCarga=@rutCarga";
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@rutCarga", rutCarga));
                        cmd.Parameters.Add(new SqlParameter("@contrato", contrato));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //AGREGAMOS DATOS A LA TABLA HASHA
                                tabla.Add("nombre", (string)rd["nombre"]);
                                tabla.Add("sexo", (Int16)rd["sexo"]);
                                tabla.Add("fechanac", (DateTime)rd["fechanac"]);
                                tabla.Add("parentesco", (Int16)rd["parentesco"]);
                                tabla.Add("relacionlegal", (Int16)rd["relacionlegal"]);
                                tabla.Add("fechaingreso", (DateTime)rd["fechaingreso"]);
                                tabla.Add("fechatermino", (DateTime)rd["fechatermino"]);
                                tabla.Add("invalido", Convert.ToInt16(rd["invalido"]));
                                tabla.Add("maternal", Convert.ToInt16(rd["maternal"]));
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

            return tabla;
        }

        //COMPARA VALORES
        private void ComparaFamiliar(string contrato, string rutCarga, string nombre, Int16 sexo, DateTime fechanac, 
            Int16 parentesco, Int16 relacionlegal, DateTime fechaingreso, DateTime fechatermino, Hashtable Datos, 
            int invalido, int maternal)
        {        
            if (Datos.Count > 0)
            {
                if ((string)Datos["nombre"] != nombre)
                {
                    logRegistro log = new logRegistro(User.getUser(), "MODIFICAR NOMBRE CARGA FAMILIAR ASOCIADA A CONTRATO "+ contrato, "CARGAFAMILIAR", (string)Datos["nombre"], nombre, "MODIFICAR");
                    log.Log();
                }
                if ((Int16)Datos["sexo"] != sexo)
                {
                    logRegistro log = new logRegistro(User.getUser(), "MODIFICA SEXO CARGA FAMILIAR ASOCIADO A " + contrato, "CARGAFAMILIAR", (Int16)Datos["sexo"] + "", sexo+"", "MODIFICAR");
                    log.Log();
                }
                if ((DateTime)Datos["fechanac"] != fechanac)
                {
                    logRegistro log = new logRegistro(User.getUser(), "MODIFICA FECHA NACIMIENTO CARGA FAMILIAR ASOCIADO A CONTRATO " + contrato, "CARGAFAMILIAR", (DateTime)Datos["fechanac"] + "", fechanac + "", "MODIFICAR");
                    log.Log();
                }
                if ((Int16)Datos["parentesco"] != parentesco)
                {
                    logRegistro log = new logRegistro(User.getUser(), "MODIFICA PARENTESCO CARGA FAMILIAR ASOCIADO A CONTRATO " + contrato, "CARGAFAMILIAR", (Int16)Datos["parentesco"] + "", parentesco + "", "MODIFICAR");
                    log.Log();
                }
                if ((Int16)Datos["relacionlegal"] != relacionlegal)
                {
                    logRegistro log = new logRegistro(User.getUser(), "MODIFICA RELACION LEGAL CARGA FAMILIAR ASOCIADO A CONTRATO " + contrato, "CARGAFAMILIAR", (Int16)Datos["relacionlegal"]+"", relacionlegal+"", "MODIFICAR");
                    log.Log();
                }
                if ((DateTime)Datos["fechaingreso"] != fechaingreso)
                {
                    logRegistro log = new logRegistro(User.getUser(), "MODIFICA FECHA INGRESO CARGA FAMILIAR ASOCIADO A CONTRATO " + contrato, "CARGAFAMILIAR", (DateTime)Datos["fechaingreso"] + "", fechaingreso + "", "MODIFICAR");
                    log.Log();
                }
                if ((DateTime)Datos["fechatermino"] != fechatermino)
                {
                    logRegistro log = new logRegistro(User.getUser(), "MODIFICA FECHA TERMINO CARGA FAMILIAR ASOCIADO A CONTRATO " + contrato, "CARGAFAMILIAR", (DateTime)Datos["fechatermino"] + "", fechatermino + "", "MODIFICAR");
                    log.Log();
                }
                if ((Int16)Datos["invalido"] != invalido)
                {
                    logRegistro log = new logRegistro(User.getUser(), "MODIFICA CAMPO INVALIDO CARGA FAMILIAR ASOCIADO A CONTRATO " + contrato, "CARGAFAMILIAR", (Int16)Datos["invalido"] + "", invalido + "", "MODIFICAR");
                    log.Log();
                }
                if ((Int16)Datos["maternal"] != maternal)
                {
                    logRegistro log = new logRegistro(User.getUser(), "MODIFICA CAMPO MATERNAL CARGA FAMILIAR ASOCIADO A CONTRATO " + contrato, "CARGAFAMILIAR", (Int16)Datos["maternal"] + "", maternal + "", "MODIFICAR");
                    log.Log();
                }
            }
        }


        #endregion
        #endregion

        private void txtRut_KeyPress(object sender, KeyPressEventArgs e)
        {
            //CARACTER 45-> '-'
            //CARACTER 46-> '.'

            if (e.KeyChar == (char)45)
            {
                e.Handled = false;
            }
            else if (char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (e.KeyChar == (char)46)
            {
                e.Handled = false;
            }
            else if (e.KeyChar == (char)107)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void txtNombre_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (char.IsSeparator(e.KeyChar))
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

        private void txtRut_KeyDown(object sender, KeyEventArgs e)
        {
         
            if (e.KeyData == Keys.Enter)
            {
                ValidaCajaRut();
            }
           
        }

        private void ValidaCajaRut()
        {
            string cadena = "";
            bool Existe = false, valido = false;

            if (cbNoAplica.Checked == false)
            {
                //VALIDAMOS RUT
                if (txtRut.ReadOnly == false)
                {
                    if (txtRut.Text == "")
                    {
                        lblmsg.Visible = true;
                        lblmsg.Text = "Por favor ingresar un rut valido";
                        txtRut.Focus();
                        return;
                    }
                    else
                    {

                        if (txtRut.Text.Contains(".") || txtRut.Text.Contains("-"))
                        {
                            cadena = fnSistema.fnExtraerCaracteres(txtRut.Text);
                        }
                        else
                        {
                            cadena = txtRut.Text;
                        }

                        Existe = fnExisteRegistro(contratoTitular, cadena);
                        if (Existe)
                        {
                            lblmsg.Visible = true;
                            lblmsg.Text = "Rut ya registrado";
                            txtRut.Focus();
                            return;
                        }


                        //VALIDAR RUT
                        if (cadena.Length < 8 || cadena.Length > 9)
                        {
                            //CADENA NO VALIDA
                            lblmsg.Visible = true;
                            lblmsg.Text = "Rut no valido";

                            //FORMATEAR
                            txtRut.Text = fnSistema.fFormatearRut2(cadena);
                            txtRut.Focus();
                            return;
                        }
                        else
                        {
                            lblmsg.Visible = false;
                            //ENMASCARAR RUT ANTES DE VALIDAR
                            cadena = fnSistema.fEnmascaraRut(cadena);
                            valido = fnSistema.fValidaRut(cadena);
                            if (valido == false)
                            {
                                lblmsg.Visible = true;
                                lblmsg.Text = "Rut no valido";
                                txtRut.Text = fnSistema.fFormatearRut2(cadena);
                                txtRut.Focus();
                                return;
                            }
                            else
                            {
                                lblmsg.Visible = false;
                                txtRut.Text = fnSistema.fFormatearRut2(cadena);

                            }
                        }
                    }
                }

            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            //SESION
            Sesion.NuevaActividad();

            fnLimpiarCampos();
            if (op.Cancela == false)
            {
                op.Cancela = true;
                op.SetButtonProperties(btnNuevo, 2);
                txtRut.Properties.Appearance.BackColor = Color.LightGoldenrodYellow;
                //DESHABILITAMOS BOTONES REPORTE
                btnImprimeExcel.Enabled = false;
                btnImprimirPdf.Enabled = false;
                btnPdf.Enabled = false;
                btnImpresionRapida.Enabled = false;
                btnEliminar.Enabled = false;
            }
            else
            {
                op.Cancela = false;
                op.SetButtonProperties(btnNuevo, 1);
                CargarCampos();
                txtRut.Properties.Appearance.BackColor = Color.White;

               
            }
        }

        private void btnGrabar_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            //VER SI EL USUARIO ESTÁ BLOQUEADO 
            if (User.Bloqueado())
            { XtraMessageBox.Show("No puedes realizar modificaciones", "Información", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (txtRut.Text == "") { lblmsg.Visible = true;lblmsg.Text = "Por favor ingresa un rut";txtRut.Focus();return; }
            if (txtNombre.Text == "") { lblmsg.Visible = true;lblmsg.Text = "Por favor ingresa un nombre";txtNombre.Focus();return; }

            if (cbInvalido.Checked && cbMaternal.Checked)
            { XtraMessageBox.Show("Debes seleccionar solo una opcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            lblmsg.Visible = false;
            string cad = "";
            bool existe = false;
            bool valido = false;
            if (Updated == false)
            {
                //INSERT                
                if (txtRut.Text.Contains(".") || txtRut.Text.Contains("-"))
                    cad = fnSistema.fnExtraerCaracteres(txtRut.Text);
                else
                    cad = txtRut.Text;
                //VALIDAR QUE RUT NO EXISTE COMO REGISTRO                 
                existe = fnExisteRegistro(contratoTitular, cad);
                if (existe)
                {
                    lblmsg.Visible = true;
                    lblmsg.Text = "Rut ya ingresado";
                    txtRut.Focus();
                    return;
                }
                else
                {                 
                    cad = fnSistema.fEnmascaraRut(cad);                
                    valido = fnSistema.fValidaRut(cad);
                    if (valido == false)
                    {
                        lblmsg.Visible = true;
                        lblmsg.Text = "Por favor verifica rut";
                        txtRut.Focus();
                        return;
                    }
                    else
                    {
                        lblmsg.Visible = false;
                        NuevaCargaFamiliar(contratoTitular, txtRut, txtNombre, txtSexo, dtNacimiento, txtParentesco, txtRelacionLegal, dtIngreso, dtTermino, cbMaternal, cbInvalido);
                    }                                     
                }
            }
            else
            {
                //UPDATE
                ModificarCargaFamiliar(contratoTitular, txtRut, txtNombre, txtSexo, dtNacimiento, txtParentesco, txtRelacionLegal, dtIngreso, dtTermino, cbMaternal, cbInvalido);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            if (txtRut.Text == "") { lblmsg.Visible = true;lblmsg.Text = "Registro no valido"; return;}

            if (viewFamiliares.RowCount>0)
            {
                lblmsg.Visible = false;
                //ELIMINAR REGISTRO!!
                EliminarCargaFamiliar(contratoTitular,viewFamiliares.GetFocusedDataRow()["rutCarga"] + "");
            }
            else
            {
                lblmsg.Visible = true;
                lblmsg.Text = "Registro no valido";
                return;
            }

        }

        private void cbNoAplica_CheckedChanged(object sender, EventArgs e)
        {
            if (cbNoAplica.Checked)
            {
                txtRut.Text = "111111111";
                txtRut.ReadOnly = true;
                txtNombre.Focus();
            }
            else
            {
                txtRut.Text = "";
                txtRut.Focus();
                txtRut.ReadOnly = false;
            }
        }

        private void gridFamiliares_Click(object sender, EventArgs e)
        {
            CargarCampos();

            //RESET BOTON NUEVO CAPTION
            op.Cancela = false;
            op.SetButtonProperties(btnNuevo, 1);
        }

        private void txtRut_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void txtNombre_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (txtNombre.Text == "")
                {
                    lblmsg.Visible = true;
                    lblmsg.Text = "Por favor ingresa un nombre";
                    txtNombre.Focus();
                }
                else
                {
                    lblmsg.Visible = false;
                }
            }
        }

        private void dtIngreso_EditValueChanged(object sender, EventArgs e)
        {
            //SUMAR 18 AÑOS A LA FECHA DE INGRESO
            //DateTime fechaFinal = DateTime.Now.Date;
            //fechaFinal = dtIngreso.DateTime.AddYears(18);
            //dtTermino.DateTime = fechaFinal;
        }

        private void txtRut_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtNombre_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtSexo_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void dtNacimiento_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtParentesco_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtRelacionLegal_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void dtIngreso_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void dtTermino_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            string rutCarga = "";

            if (viewFamiliares.RowCount > 0)
            {
                rutCarga = (string)viewFamiliares.GetFocusedDataRow()["rutcarga"];
                if (CambiosSinGuardar(contratoTitular, rutCarga))
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

        private void btnImprimirPdf_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();


            if (objeto.ValidaAcceso(User.GetUserGroup(), "rptcargas") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }           

            if (viewFamiliares.RowCount > 0)
            {
                if (contratoTitular != "")
                {
                    ImprimirPdf(contratoTitular, false, false);
                }
            }
            else
            {
                XtraMessageBox.Show("No hay informacion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            if (op.Cancela)
            {
                op.SetButtonProperties(btnNuevo, 1);
                op.Cancela = false;
                if (viewFamiliares.RowCount > 0)
                {
                    CargarCampos();
                    txtRut.Properties.Appearance.BackColor = Color.White;
                }
            }
        }

        private void btnImpresionRapida_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "rptcargas") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (viewFamiliares.RowCount > 0)
            {
                if (contratoTitular != "")
                {
                    ImprimirPdf(contratoTitular, true, false);
                }
            }
            else
            {
                XtraMessageBox.Show("No hay informacion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            if (op.Cancela)
            {
                op.SetButtonProperties(btnNuevo, 1);
                op.Cancela = false;
                if (viewFamiliares.RowCount > 0)
                {
                    CargarCampos();
                    txtRut.Properties.Appearance.BackColor = Color.White;
                }
            }

        }

        private void btnImprimeExcel_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "rptcargas") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }


            if (contratoTitular != null && viewFamiliares.RowCount > 0)
            {
                DataTable tablaData = new DataTable();
                tablaData = GeneraTablaExcel(contratoTitular);

                tablaData = TablaFinal(tablaData);

                if (tablaData == null)
                { XtraMessageBox.Show("No se encontraron datos", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

                string fileName = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\CargaFamiliar_" + contratoTitular + ".xlsx";

                if (tablaData.Rows.Count > 0)
                {
                    if (FileExcel.IsExcelInstalled() == false)
                    { XtraMessageBox.Show("Parece ser que tu sistema no tiene instalado Excel", "Informacion Importante", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                    //if (FileExcel.ExcelAbierto(fileName))
                    //{ XtraMessageBox.Show("Cierra el archivo antes de continuar", "Informacion Importante", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                    if (FileExcel.CrearArchivoExcelDev(tablaData, fileName))
                    {
                        //CORRECTO , PREGUNTA SI DESEA VER EL ARCHIVO CREADO...
                        DialogResult pregunta = XtraMessageBox.Show("Archivo " + fileName + " generado correctamente, ¿Desea ver el archivo?", "Informacion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (pregunta == DialogResult.Yes)
                        {
                            FileExcel.AbrirExcel(fileName);
                        }
                    }
                    else
                    {
                        XtraMessageBox.Show("No se pudo generar el archivo", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    XtraMessageBox.Show("No hay resultados", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                XtraMessageBox.Show("No hay informacion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }


            if (op.Cancela)
            {
                op.SetButtonProperties(btnNuevo, 1);
                op.Cancela = false;
                if (viewFamiliares.RowCount > 0)
                {
                    CargarCampos();
                    txtRut.Properties.Appearance.BackColor = Color.White;
                }
            }
        }

        private void gridFamiliares_KeyUp(object sender, KeyEventArgs e)
        {
            CargarCampos();

            //RESET BOTON NUEVO CAPTION
            btnNuevo.Text = "Nuevo";
            btnNuevo.ToolTip = "Nuevo Registro";
            cancela = false;
        }

        private void dtNacimiento_EditValueChanged(object sender, EventArgs e)
        {
            //FECHA TERMINO VA A SER EL EQUIVALENTE A LA FECHA DE NACIMIENTO MAS 18 AÑOS
            if(cancela)
            dtTermino.DateTime = dtNacimiento.DateTime.AddYears(18);
        }

        private void dtTermino_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void btnPdf_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "rptcargas") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }


            if (viewFamiliares.RowCount > 0)
            {
                if (contratoTitular != "")
                {
                    ImprimirPdf(contratoTitular, false, true);
                }
            }
            else
            {
                XtraMessageBox.Show("No hay informacion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }


            if (op.Cancela)
            {
                op.SetButtonProperties(btnNuevo, 1);
                op.Cancela = false;
                if (viewFamiliares.RowCount > 0)
                {
                    CargarCampos();
                    txtRut.Properties.Appearance.BackColor = Color.White;
                }
            }
        }

        private void txtParentesco_EditValueChanged(object sender, EventArgs e)
        {
            int data = -1;
            //Solo en insert
            try
            {
                if (Updated == false)
                {
                    if (txtParentesco.Properties.DataSource != null)
                    {
                        data = Convert.ToInt32(txtParentesco.EditValue);
                        if (data == 0)
                        {
                            dtTermino.DateTime = Convert.ToDateTime("01-01-3000");
                        }
                        else
                        {
                            dtTermino.DateTime = fnSistema.UltimoDiaMes(Calculo.PeriodoObservado);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                //ERROR...
            }
        }

        private void txtRut_Leave(object sender, EventArgs e)
        {
            ValidaCajaRut();
        }
    }
}