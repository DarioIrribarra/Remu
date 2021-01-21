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
    public partial class frmCargarTrabajador : DevExpress.XtraEditors.XtraForm
    {
        //PARA NOMBRE COMPLETO
        private string PrimerNombre = "";
        private string SegundoNombre = "";

        private string rut = "";
        private string contrato = "";

        //PARA OBTENER EL FILTRO DEL USUARIO LOGUEADO
        private string FiltroUsuario { get; set; } = User.GetUserFilter();

        //SE PUEDEN VISUALIZAR LOS CONTRATOS INACTIVOS
        public bool ShowInactivos { get; set; }

        //SE PUEDEN VISUALIZAR LOS CONTRATOS ACTIVOS
        public bool ShowActivos { get; set; }

        //USUARIO PUEDE VER FICHAS PRIVADAS
        private bool ShowPrivados { get; set; } = User.ShowPrivadas();

        /// <summary>
        /// Consulta base
        /// </summary>
        string sqlBase = "SELECT rut, CONCAT(apepaterno, ' ', apematerno, ' ', nombre) " +
                     $"as nombre, MAX(anomes) as anomes,contrato, IIF(MAX(anomes)<{Calculo.PeriodoObservado},0,status) as status FROM trabajador "; 
        
        string sqlBaseSimple = "SELECT rut, CONCAT(apepaterno, ' ', apematerno, ' ', nombre) " +
                     "as nombre, anomes, contrato, status FROM trabajador ";

        string StatusSql = "";

        public ICopiaEmp Opener;

        public frmCargarTrabajador()
        {
            InitializeComponent();
        }

        private void frmCargarTrabajador_Load(object sender, EventArgs e)
        {            
            fnBuscarTrabajador("");

            //fnSistema.spllenaGridView(gridTrabajador, sqlGrilla);
            //propiedades grilla
            if (viewTrabajador.RowCount > 0)
            {
                fnSistema.spOpcionesGrilla(viewTrabajador);
                fnColumnasGrilla();
            }
            
            fnDefaultProperties();            
        }

        #region "MANEJO DE DATOS"
        private void fnColumnasGrilla()
        {           

            //MOSTRAR RUT CON FORMATO 12.444.528-K
            viewTrabajador.Columns[0].Caption = "Rut";
            viewTrabajador.Columns[0].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            viewTrabajador.Columns[0].DisplayFormat.FormatString = "Rut";
            viewTrabajador.Columns[0].DisplayFormat.Format = new FormatCustom();
            viewTrabajador.Columns[0].Width = 80;

            //NOMBRE COMPLETO
            viewTrabajador.Columns[1].Caption = "Trabajador";
            viewTrabajador.Columns[1].Width = 200;
            //viewTrabajador.Columns[1].AppearanceCell.FontStyleDelta = FontStyle.Bold;

            //PERIODO
            viewTrabajador.Columns[2].Visible = false;

            //CONTRATO
            viewTrabajador.Columns[3].Caption = "Contrato";
            viewTrabajador.Columns[3].Width = 120;

            //ESTADO       
            viewTrabajador.Columns[4].Caption = "Estado";
            viewTrabajador.Columns[4].DisplayFormat.Format = new FormatCustom();
            viewTrabajador.Columns[4].DisplayFormat.FormatString = "estado";
            viewTrabajador.Columns[4].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            
            
        }

        //METODO PARA BUSQUEDA DE TRABAJADOR
        private void fnBuscarTrabajador(string pBusqueda)
        {
            //VARIABLE PARA GUARDAR EL PERIODO EN EVALUACION

            string sqlFiltro = "";
            string sql = "";
            string SqlLike = $" AND (nombre LIKE '%{pBusqueda}%' OR apepaterno LIKE '%{pBusqueda}%' OR apematerno LIKE '%{pBusqueda}%' OR contrato LIKE '%{pBusqueda}%')";

            sqlFiltro = Calculo.GetSqlFiltro(FiltroUsuario, "", ShowPrivados);

            if (pBusqueda.Length > 0)
            {
                if (ShowInactivos && ShowActivos)
                {
                    sql = sqlBase + $" WHERE ((contrato " +
                                    $"NOT IN (SELECT contrato FROM trabajador GROUP by contrato HAVING MAX(anomes) = {Calculo.PeriodoObservado})) " +
                                    $"OR (status = 0 AND anomes={Calculo.PeriodoObservado}) OR (status = 1 AND anomes={Calculo.PeriodoObservado}))  {sqlFiltro} {SqlLike}" +
                                    $"GROUP BY contrato, rut, nombre, apepaterno, apematerno, status " +
                                    $"ORDER BY apepaterno ";
                }
                else if (ShowInactivos)
                {
                    sql = sqlBase + $" WHERE contrato " +
                                    $"NOT IN (SELECT contrato FROM trabajador GROUP BY contrato HAVING MAX(anomes) = {Calculo.PeriodoObservado}) OR (status=0 AND anomes={Calculo.PeriodoObservado}) {sqlFiltro} {SqlLike}" +
                                    $"GROUP BY contrato, rut, nombre, apepaterno, apematerno, status " +
                                    $" ORDER BY apepaterno";
                }
                else
                {
                    sql = sqlBaseSimple + $" WHERE status = 1 AND anomes = {Calculo.PeriodoObservado} {sqlFiltro} {SqlLike} ORDER BY apepaterno ";                        
                }
            }
            else
            {
                if (ShowInactivos && ShowActivos)
                {
                    sql = sqlBase + $" WHERE (contrato " +
                                    $"NOT IN (SELECT contrato FROM trabajador GROUP by contrato HAVING MAX(anomes) = {Calculo.PeriodoObservado})) " +
                                    $"OR (status = 0 AND anomes={Calculo.PeriodoObservado}) OR (status=1 AND anomes={Calculo.PeriodoObservado}) {sqlFiltro}" +
                                    $"GROUP BY contrato, rut, nombre, apepaterno, apematerno, status " +
                                    $"ORDER BY apepaterno ";
                }
                else if (ShowInactivos)
                {
                    sql = sqlBase + $" WHERE contrato " +
                                    $"NOT IN (SELECT contrato FROM trabajador GROUP BY contrato HAVING MAX(anomes) = {Calculo.PeriodoObservado}) OR (status=0 AND anomes={Calculo.PeriodoObservado}) {sqlFiltro}" +
                                    $"GROUP BY contrato, rut, nombre, apepaterno, apematerno, status " +
                                    $" ORDER BY apepaterno";
                }
                else
                {
                    sql = sqlBaseSimple + $" WHERE status = 1 AND anomes = {Calculo.PeriodoObservado} {sqlFiltro} ORDER BY apepaterno ";
                }
            }

            fnRecargarGrilla(gridTrabajador, sql);
            fnColumnasGrilla();
            //ChangeValueCell();
        }

        private void fnRecargarGrilla(DevExpress.XtraGrid.GridControl pGrid, string pSql)
        {
            SqlConnection cn;
            SqlCommand cmd;

            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter();
                        DataSet ds = new DataSet();

                        using (cmd = new SqlCommand(pSql, cn))
                        {
                            //parametros
                            //cmd.Parameters.Add(new SqlParameter(pCampo, pDato));
                            adapter.SelectCommand = cmd;
                            adapter.Fill(ds);
                            adapter.Dispose();
                            cmd.Dispose();

                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                pGrid.DataSource = ds.Tables[0];
                                fnLimpiarCampos();
                                int filas = ds.Tables[0].Rows.Count;
                                //lblresultado.Text = filas + " coincidencias";
                            }
                            else
                            {
                                XtraMessageBox.Show("No se encontraron resultados", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                txtBusqueda.Text = "";
                                txtBusqueda.Focus();
                            }
                        }                       
                     
                    }
                }

            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

        }

        //METODO PARA LIMPIAR CAMPOS
        private void fnLimpiarCampos()
        {
            txtBusqueda.Text = "";
            txtBusqueda.Focus();
        }

        /// <summary>
        /// Cambia status
        /// </summary>
        private void ChangeValueCell()
        {
            string contratoRow = "";
            int perRow = 0, statusCell = 0;

            try
            {
                if (viewTrabajador.RowCount > 0)
                {
                    for (int i = 0; i < viewTrabajador.DataRowCount; i++)
                    {
                        contratoRow = (string)viewTrabajador.GetRowCellValue(i, "contrato");
                        perRow = Convert.ToInt32(viewTrabajador.GetRowCellValue(i, "anomes"));
                        statusCell = Convert.ToInt32(viewTrabajador.GetRowCellValue(i, "status"));
                        if (contratoRow.Length > 0 && Calculo.PeriodoValido(perRow))
                        {
                            if (perRow < Calculo.PeriodoObservado)
                            {
                                if (statusCell == 1)
                                    viewTrabajador.SetRowCellValue(i, "status", 0);
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                //ERROR...               
            }
        }

        //PROPIEDADES POR DEFECTO
        private void fnDefaultProperties()
        {
            txtNombre.Focus();
            txtContrato.Properties.MaxLength = 15;
            txtNombre.Properties.MaxLength = 40;        
            txtRut.Properties.MaxLength = 12;          
            BtnBuscar.TabStop = false;       
        }

        //OBTENER EL NOMBRE Y EL APELLIDO DESDE LA CAJA DE TEXTO
        //ASUMIMOS QUE SON LOS NOMBRES Y LUEGO LOS APELLIDOS
        private void fnObtenerNombre(string Completo)
        {
            if (Completo.Length == 0) return;
          
            int cantidad = 0;         
            string[] separador = Completo.Split(' ');
            cantidad = separador.Length;
            int x = 0;
            
            if (cantidad>0)
            {
                //TIENE ELEMENTOS
                string[] cadenas = new string[cantidad];
                string cad = "";
                for (int i = 0; i < Completo.Length; i++)
                {
                    if (Completo[i].ToString() != " ")
                    {
                        cad = cad + Completo[i];
                    }
                    else
                    {                 
                        //SI ES UN ESPACIOS GUARDAMOS CADENAS CONCATENADA HASTA EL MOMENTO EN ARREGLO
                        cadenas[x] = cad;                        
                        cad = "";
                        x++;
                    }

                    //SI ES LA ULTIMA POSICION
                    if (i == (Completo.Length - 1))
                        cadenas[x] = cad;
                }               

                if (cadenas.Length>0)
                {
                    //PRIMER ELEMENTO --> PRIMER NOMBRE
                    //SEGUNDO NOMBRE --> ELEMENTOS RESTANTES DEL ARREGLO
                    PrimerNombre = cadenas[0];                    
                    for (int i = 1; i < cadenas.Length; i++)
                    {
                        SegundoNombre = SegundoNombre + cadenas[i] + " ";
                    }
                }
            }
            else
            {
                PrimerNombre = Completo;
                SegundoNombre = Completo;
            }
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            string cadena = "";
            if (keyData == Keys.Tab)
            {
                if (txtNombre.ContainsFocus)
                {
                    if (txtNombre.Text == "")
                    {
                        lblError.Visible = true;
                        lblError.Text = "Por favor ingrese nombres del trabajador";
                        return false;
                    }
                    else
                    {
                       // lblError.Visible = false;
                        int espacios = 0;
                        espacios = CantidadEspacios(txtNombre.Text);                       
                        if (espacios > 3)
                        { lblError.Visible = true; lblError.Text = "Parece ser que el nombre que ingresaste no es valido"; return false; }

                        lblError.Visible = false;
                    }
                }
                if (txtRut.ContainsFocus)
                {
                    ValidaRut();
                }
                if (txtContrato.ContainsFocus)
                {
                    if (txtContrato.Text == "")
                    {
                        lblError.Visible = true;
                        lblError.Text = "Por favor ingresa el numero de contrato";
                        txtContrato.Focus();
                    }
                    else
                    {
                        lblError.Visible = false;
                        int periodo = 0;
                        periodo = Calculo.PeriodoObservado;
                        //VALIDAR QUE CONTRATO NO ESTA REGISTRADO EN BD
                        bool existeContrato = Persona.ExisteContrato(txtContrato.Text, periodo);

                        if (existeContrato)
                        {
                            lblError.Visible = true;
                            lblError.Text = "Contrato ya existe para este periodo";
                            return false;
                        }
                        //else
                        //{
                        //    lblError.Visible = false;
                        //    if (txtRut.Text != "" && txtNombre.Text != "" && txtContrato.Text != "")
                        //    {
                              
                        //        BtnBuscar.Enabled = true;
                        //        txtBusqueda.Enabled = true;
                        //        gridTrabajador.Visible = true;
                        //        this.Width = 618;
                        //        this.Height = 674;
                        //    }                          
                        //}
                    }
                }
                if (txtApeMaterno.ContainsFocus)
                {
                    if (txtApeMaterno.Text == "")
                    { lblError.Visible = true;lblError.Text = "Por favor ingresa apellido materno";return false;}

                    lblError.Visible = false;
                }
                if (txtApepaterno.ContainsFocus)
                {
                    if (txtApepaterno.Text == "")
                    { lblError.Visible = true;lblError.Text = "Por favor ingresar apellido materno";return false;}
                    lblError.Visible = false;
                }
            }
            return base.ProcessDialogKey(keyData);
        }

        //CONTAR LA CANTIDAD DE ESPACIOS QUE HAY EN UNA CADENA
        private int CantidadEspacios(string cadena)
        {
            int separador = 0;
            if (cadena.Length>0)
            {
                separador = 0;
                for (int i = 0; i < cadena.Length; i++)
                {
                    if (cadena[i].ToString() == " ")
                        separador++;
                }
            }

            return separador;
        }

        private void ValidaRut()
        {
            string cadena = "";

            if (txtRut.Text == "") { lblError.Visible = true; lblError.Text = "Rut no valido"; return ; }
            if (txtRut.Text.Contains(".") || txtRut.Text.Contains("-"))
            {
                cadena = fnSistema.fnExtraerCaracteres(txtRut.Text);
            }
            else
            {
                cadena = txtRut.Text;
            }

            if (cadena.Length < 8 || cadena.Length > 9)
            {
                //CADENA NO VALIDA
                lblError.Visible = true;
                lblError.Text = "Rut no valido";
                //FORMATEAR
                txtRut.Text = fnSistema.fFormatearRut2(cadena);
                return ;
            }
            else
            {
                //validar rut
                cadena = fnSistema.fEnmascaraRut(cadena);
                bool rutValida = fnSistema.fValidaRut(cadena);
                if (rutValida == false)
                {
                    lblError.Visible = true;
                    lblError.Text = "Rut no valido";
                    //FORMATEAR
                    txtRut.Text = fnSistema.fFormatearRut2(cadena);
                    return ;
                }
                else
                {
                    lblError.Visible = false;

                    txtRut.Text = fnSistema.fFormatearRut2(cadena);

                    txtContrato.Text = Calculo.GetNuevoContrato(cadena);

                    //if (txtRut.Text.Contains(".") || txtRut.Text.Contains("-"))
                    //    txtContrato.Text = fnSistema.fnExtraerCaracteres(txtRut.Text);
                    //else
                    //    txtContrato.Text = txtRut.Text;
                }
            }
        }

        #endregion

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            fnBuscarTrabajador(txtBusqueda.Text);
        }

        private void txtBusqueda_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
               // if (txtBusqueda.Text == "") { XtraMessageBox.Show("Busqueda no valida", "Busqueda", MessageBoxButtons.OK, MessageBoxIcon.Error); txtBusqueda.Focus(); return; }
                fnBuscarTrabajador(txtBusqueda.Text);
                //if(txtBusqueda.Text == "")
                  //  fnSistema.spllenaGridView(gridTrabajador, "SELECT contrato,  CONCAT(nombre , ' ' , apepaterno , ' ' ,apematerno) as nombre, rut, contrato FROM trabajador ORDER BY nombre");
            }            
        }

        private void gridTrabajador_DoubleClick(object sender, EventArgs e)
        {
            string rut = "", contrato = "";
            int PeriodoPersona = 0;

            if (txtNombre.Text.Length == 0)
            { XtraMessageBox.Show("Por favor ingresa nombres del trabajador", "Nombre", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtNombre.Focus(); return; }

            if (txtApeMaterno.Text.Length == 0)
            { XtraMessageBox.Show("Por favor ingresa apellido materno", "Nombre", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtApeMaterno.Focus(); return; }

            if (txtApepaterno.Text.Length == 0)
            { XtraMessageBox.Show("Por favor ingresa apellido paterno", "Nombre", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtApepaterno.Focus(); return; }

            if (txtRut.Text.Length == 0)
            { XtraMessageBox.Show("Por favor ingresa rut nuevo trabajador", "Rut", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtRut.Focus(); return; }

            if (txtContrato.Text.Length == 0)
            { XtraMessageBox.Show("Por favor ingresa numero de contrato", "Contrato", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtContrato.Focus(); return; }

            if (Persona.ExisteContrato(txtContrato.Text, Calculo.PeriodoObservado))
            { XtraMessageBox.Show("Por favor verifica que el numero de contrato ingresado no exista", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtContrato.Focus(); return; }

            if (txtRut.Text.Contains(".") || txtRut.Text.Contains("-"))
            {
                if (fnSistema.fValidaRut(fnSistema.fnExtraerCaracteres(txtRut.Text)) == false)
                { XtraMessageBox.Show("Por favor verifica el rut ingresado", "Rut", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtRut.Focus(); return; }
            }
            else
            {
                if (fnSistema.fValidaRut(txtRut.Text) == false)
                { XtraMessageBox.Show("Por favor verifica el rut ingresado", "Rut", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtRut.Focus(); return; }
            }                

            //SI ES ASI CARGAMOS LOS DATOS DEL EMPLEADOS EN EL NUEVO FORMULARIO
            if (viewTrabajador.RowCount>0)
                {
                    rut = viewTrabajador.GetFocusedDataRow()["rut"].ToString();
                    contrato = viewTrabajador.GetFocusedDataRow()["contrato"].ToString();
                    PeriodoPersona = Convert.ToInt32(viewTrabajador.GetFocusedDataRow()["anomes"]);                    

                    //PARA OBTENER EL NOMBRE Y LOS APELLIDOS                
                    fnObtenerNombre(txtNombre.Text);

                    if (PrimerNombre.Length == 0)
                    {
                       XtraMessageBox.Show("Por favor ingresa el primer nombre del trabajador", "Error copia", MessageBoxButtons.OK, MessageBoxIcon.Stop); return;
                    }

                    if (SegundoNombre.Length == 0)
                    {
                        XtraMessageBox.Show("Por favor ingresa el segundo nombre del trabajador", "Error copia", MessageBoxButtons.OK, MessageBoxIcon.Stop); return;
                    }

                    //CARGAR INFORMACION EN FORMULARIO EMPLEADO.
                    if (Opener != null)
                    {
                        Opener.CargarCopia(txtRut.Text, contrato, PeriodoPersona, PrimerNombre, SegundoNombre, txtApepaterno.Text, txtApeMaterno.Text, txtContrato.Text);
                        this.Close();
                    }
                    else
                    {
                        XtraMessageBox.Show("No se pudo cargar la información", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }                    

                    //this.Dispose();
                    //this.Owner.Close();
                    //this.Close();                
                }              
        }

        private void frmCargarTrabajador_Shown(object sender, EventArgs e)
        {
            txtNombre.Focus();
        }
        private void gridTrabajador_KeyDown(object sender, KeyEventArgs e)
        {
   
        }

        private void txtContrato_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) || e.KeyChar == (char)45)
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
            else
            {
                e.Handled = true;
            }
        }

        private void txtContrato_Enter(object sender, EventArgs e)
        {
                     
        }

        private void txtRut_Enter(object sender, EventArgs e)
        {
            /*if (txtContrato.Text == "" || txtNombre.Text == "")
            {
                lblError.Visible = true;
                lblError.Text = "Debes llenar los campos obligatorios";
                txtBusqueda.Enabled = false;
                BtnBuscar.Enabled = false;
            }*/
        }

        private void txtNombre_Enter(object sender, EventArgs e)
        {
            /*if (txtContrato.Text == "" || txtRut.Text == "")
            {
                lblError.Visible = true;
                lblError.Text = "Debes llenar los campos obligatorios";
                txtBusqueda.Enabled = false;
                BtnBuscar.Enabled = false;
            }*/
        }

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
            else if (e.KeyChar == (char)75)
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
          
        }

        private void txtContrato_KeyDown(object sender, KeyEventArgs e)
        {
            
            if (e.KeyData == Keys.Enter)
            {
                ValidaContrato();                 
            }
        }

        private void ValidaContrato()
        {
            bool valido = false;

            if (txtContrato.Text == "")
            {
                lblError.Visible = true;
                lblError.Text = "Por favor ingresa un numero de contrato";
            }
            else
            {
                lblError.Visible = false;
                int periodo = 0;
                periodo = Calculo.PeriodoObservado;

                //VALIDAR QUE EL CONTRATO NO EXISTA PARA EL PERIODO
                valido = Persona.ExisteContrato(txtContrato.Text, periodo);
                if (valido)
                {
                    lblError.Visible = true;
                    lblError.Text = "El contrato ingresado ya existe para este periodo";
                }
                else
                {
                    //NO EXISTE EL CONTRATO PARA ESTE PERIODO
                    lblError.Visible = false;

                    //PASAMOS EL FOCO AL TEXTO DE BUSQUEDA
                    txtBusqueda.Focus();
                }
            }
        }

        private void txtBusqueda_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void txtNombre_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
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

        private void txtRut_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void txtApepaterno_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsLetter(e.KeyChar) || char.IsControl(e.KeyChar) || e.KeyChar == (char)32)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void txtApeMaterno_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) || char.IsLetter(e.KeyChar) || e.KeyChar == (char)32)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void txtContrato_EnabledChanged(object sender, EventArgs e)
        {

        }

        private void txtBusqueda_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void btnSalirArea_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            Close();
        }

        private void txtRut_Leave(object sender, EventArgs e)
        {
            ValidaRut();
        }

        private void txtContrato_Leave(object sender, EventArgs e)
        {
            ValidaContrato();
        }
    }
}