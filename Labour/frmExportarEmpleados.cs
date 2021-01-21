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
using Excel = Microsoft.Office.Interop.Excel;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.IO;
using DevExpress.Utils.Menu;

namespace Labour
{
    public partial class frmExportarEmpleados : DevExpress.XtraEditors.XtraForm, IConjuntosCondicionales, IColumnasData
    {
        [DllImport("user32")]
        extern static IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32")]
        extern static bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        const int MF_BYCOMMAND = 0;
        const int MF_DISABLED = 2;
        const int SC_CLOSED = 0xF060;

        //PARA GUARDAR LISTA EN CASO DE QUERER MANIPULAR LOS CAMPOS A MOSTRAR EN QUERY
        private List<string> ListadoCampos = new List<string>();

        //PARA GUARDAR EL PERFIL DEL TRABAJADOR
        private string FiltroUsuario { get; set; } = User.GetUserFilter();

        #region "CONJUNTOS CONDICIONALES"
        public void CargarCodigoConjunto(string codigo)
        {
            txtConjunto.Text = codigo;
        }
        #endregion

        #region "COLUMNAS DATA"
        public void CargarLista(List<string> lista)
        {
            if (lista.Count > 0)
            {
                ListadoCampos.Clear();
                foreach (var item in lista)
                {
                    ListadoCampos.Add(item);
                }
            }
            else
            {
                ListadoCampos.Clear();
            }
        }
        #endregion

        public frmExportarEmpleados()
        {
            InitializeComponent();
        }

        private void frmExportarEmpleados_Load(object sender, EventArgs e)
        {
            var sm = GetSystemMenu(Handle, false);
            EnableMenuItem(sm, SC_CLOSED, MF_BYCOMMAND | MF_DISABLED);

            cbPeriodo.Checked = true; 
            txtPeriodo.Text = Calculo.PeriodoObservado.ToString();
            txtPeriodo.ReadOnly = true;
            cbTodos.Checked = true;
        }

        #region "MANEJO DE DATOS"

        //SI EL ARCHIVO ESTA ABIERTO ADVERTIR
        //VERIFICAR QUE EL PC TENGA INSTALDO MICROSOFT OFFICE

        private string PreparaCondicion(string condicion)
        {
            if (condicion != "")
            {
                condicion = condicion.ToLower();
                if (condicion.Contains("contrato"))
                    condicion = condicion.Replace("contrato", "trabajador.contrato");
                if (condicion.Contains("rut"))
                    condicion = condicion.Replace("rut", "trabajador.rut");
                if (condicion.Contains("anomes"))
                    condicion = condicion.Replace("anomes", "trabajador.anomes");

                return condicion;
            }
            else
                return "";
        }

        //GENERAR DATATABLE FROM SQL PARA TODOS LOS EMPLEADOS DEL PERIODO SELECCIONADO
        private DataTable TablaFromDb(int periodo)
        {
            DataTable tabla = new DataTable();
            SqlDataAdapter ad = new SqlDataAdapter();
            DataSet ds = new DataSet();
            string sql = "", condicion = "";

            if (FiltroUsuario == "0")
                sql = string.Format("SELECT anomes, contrato, trabajador.rut, trabajador.nombre, apepaterno," +
                    "  apematerno, sexo, fechanac, " +
                    " nacion.nombre as nacion, ecivil.nombre as 'ecivil', direccion, ciudad.descCiudad as ciudad, telefono, " +
                    " tipocontrato, ingreso, salida, area.nombre as area, tramo, afp.nombre as afp, isapre.nombre as salud,  " +
                    " ccosto as 'ccosto', cargo.nombre as cargo, regimensalario, jubilado, " +
                    " regimen.nombre as regimen, cajaprevision.nombre as 'cajaprevision', banco.nombre as banco, formapago.nombre as 'formapago', " +
                    " cuenta, tipocuenta.nombre as 'tipocuenta', anosprogresivo, clase, causalTermino.descCausal as causal, " +
                    " sucursal.descSucursal as sucursal, fun, mail, escolaridad.descesc as esco, jornada FROM trabajador " +
                    " INNER JOIN nacion ON nacion.id = trabajador.nacion" +
                    " INNER JOIN ecivil ON ecivil.id = trabajador.ecivil " +
                    " INNER JOIN ciudad ON ciudad.idCiudad = trabajador.ciudad" +
                    " INNER JOIN area ON area.id = trabajador.area" +
                    " LEFT JOIN afp ON afp.id = trabajador.afp " +
                    " LEFT JOIN isapre ON isapre.id = trabajador.salud " +
                    " INNER JOIN ccosto ON ccosto.id = trabajador.ccosto " +
                    " INNER JOIN cargo ON cargo.id = trabajador.cargo " +
                    " INNER JOIN regimen ON regimen.id = trabajador.regimen " +
                    " LEFT JOIN cajaPrevision ON cajaPrevision.id = trabajador.cajaPrevision " +
                    " INNER JOIN banco ON banco.id = trabajador.banco " +
                    " INNER JOIN formaPago ON formapago.id = trabajador.formapago " +
                    " INNER JOIN tipoCuenta ON tipoCuenta.id = trabajador.tipoCuenta " +
                    " INNER JOIN sucursal ON sucursal.codSucursal = trabajador.sucursal " +
                    " INNER JOIN causalTermino ON causalTermino.codCausal = trabajador.causal " +
                    " INNER JOIN escolaridad ON escolaridad.codesc = trabajador.esco " +
                    " WHERE ANOMES={0} " +
                    "ORDER BY anomes, nombre", periodo);
            else
            {
                condicion = Conjunto.GetCondicionFromCode(FiltroUsuario);
                sql = string.Format("SELECT anomes, contrato, trabajador.rut, trabajador.nombre, apepaterno," +
                        "  apematerno, sexo, fechanac, " +
                        " nacion.nombre as nacion, ecivil.nombre as 'ecivil', direccion, ciudad.descCiudad as ciudad, telefono, " +
                        " tipocontrato, ingreso, salida, area.nombre as area, tramo, afp.nombre as afp, isapre.nombre as salud,  " +
                        " ccosto as 'ccosto', cargo.nombre as cargo, regimensalario, jubilado, " +
                        " regimen.nombre as regimen, cajaprevision.nombre as 'cajaprevision', banco.nombre as banco, formapago.nombre as 'formapago', " +
                        " cuenta, tipocuenta.nombre as 'tipocuenta', anosprogresivo, clase, causalTermino.descCausal as causal, " +
                        " sucursal.descSucursal as sucursal, fun, mail, escolaridad.descesc as esco, jornada FROM trabajador " +
                        " INNER JOIN nacion ON nacion.id = trabajador.nacion" +
                        " INNER JOIN ecivil ON ecivil.id = trabajador.ecivil " +
                        " INNER JOIN ciudad ON ciudad.idCiudad = trabajador.ciudad" +
                        " INNER JOIN area ON area.id = trabajador.area" +
                        " LEFT JOIN afp ON afp.id = trabajador.afp " +
                        " LEFT JOIN isapre ON isapre.id = trabajador.salud " +
                        " INNER JOIN ccosto ON ccosto.id = trabajador.ccosto " +
                        " INNER JOIN cargo ON cargo.id = trabajador.cargo " +
                        " INNER JOIN regimen ON regimen.id = trabajador.regimen " +
                        " LEFT JOIN cajaPrevision ON cajaPrevision.id = trabajador.cajaPrevision " +
                        " INNER JOIN banco ON banco.id = trabajador.banco " +
                        " INNER JOIN formaPago ON formapago.id = trabajador.formapago " +
                        " INNER JOIN tipoCuenta ON tipoCuenta.id = trabajador.tipoCuenta " +
                        " INNER JOIN sucursal ON sucursal.codSucursal = trabajador.sucursal " +
                        " INNER JOIN causalTermino ON causalTermino.codCausal = trabajador.causal " +
                        " INNER JOIN escolaridad ON escolaridad.codesc = trabajador.esco " + 
                        " WHERE ANOMES={0} AND {1} " +
                        " ORDER BY anomes, nombre", periodo, PreparaCondicion(condicion));
            }
                

            SqlCommand cmd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@periodo", periodo));
                        ad.SelectCommand = cmd;
                        ad.Fill(ds, "data");

                        ad.Dispose();
                        cmd.Dispose();
                        fnSistema.sqlConn.Close();

                        if (ds.Tables[0].Rows.Count>0)
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

        //GENERAR DATATABLE FROM SQL PARA CONJUNTO SELECCIONADO
        private DataTable TablaConjuntoFromDb(int periodo, string codeConjunto)
        {
            DataTable tabla = new DataTable();
            SqlDataAdapter ad = new SqlDataAdapter();
            string condicion = Conjunto.GetCondicionFromCode(codeConjunto);
            string sql = "", condUser = "";

            if (FiltroUsuario == "0")
                sql = string.Format("SELECT anomes, contrato, trabajador.rut, trabajador.nombre, apepaterno," +
                        "  apematerno, sexo, fechanac, " +
                        " nacion.nombre as nacion, ecivil.nombre as 'ecivil', direccion, ciudad.descCiudad as ciudad, telefono, " +
                        " tipocontrato, ingreso, salida, area.nombre as area, tramo, afp.nombre as afp, isapre.nombre as salud,  " +
                        " ccosto as 'ccosto', cargo.nombre as cargo, regimensalario, jubilado, " +
                        " regimen.nombre as regimen, cajaprevision.nombre as 'cajaprevision', banco.nombre as banco, formapago.nombre as 'formapago', " +
                        " cuenta, tipocuenta.nombre as 'tipocuenta', anosprogresivo, clase, causalTermino.descCausal as causal, sucursal.descSucursal as sucursal, " +
                        " fun, mail, escolaridad.descesc as esco, jornada FROM trabajador " +
                        " INNER JOIN nacion ON nacion.id = trabajador.nacion" +
                        " INNER JOIN ecivil ON ecivil.id = trabajador.ecivil " +
                        " INNER JOIN ciudad ON ciudad.idCiudad = trabajador.ciudad" +
                        " INNER JOIN area ON area.id = trabajador.area" +
                        " LEFT JOIN afp ON afp.id = trabajador.afp " +
                        " LEFT JOIN isapre ON isapre.id = trabajador.salud " +
                        " INNER JOIN ccosto ON ccosto.id = trabajador.ccosto " +
                        " INNER JOIN cargo ON cargo.id = trabajador.cargo " +
                        " INNER JOIN regimen ON regimen.id = trabajador.regimen " +
                        " LEFT JOIN cajaPrevision ON cajaPrevision.id = trabajador.cajaPrevision " +
                        " INNER JOIN banco ON banco.id = trabajador.banco " +
                        " INNER JOIN formaPago ON formapago.id = trabajador.formapago " +
                        " INNER JOIN tipoCuenta ON tipoCuenta.id = trabajador.tipoCuenta " +
                        " INNER JOIN sucursal ON sucursal.codSucursal = trabajador.sucursal " +
                        " INNER JOIN causalTermino ON causalTermino.codCausal = trabajador.causal " +
                        " INNER JOIN escolaridad On escolaridad.codesc = trabajador.esco " + 
                        " WHERE contrato IN (SELECT contrato FROM trabajador where {0}) " +
                        " AND anomes={1} ORDER BY anomes, nombre", condicion, periodo);
            else
            {
                condUser = Conjunto.GetCondicionFromCode(FiltroUsuario);

                sql = string.Format("SELECT anomes, contrato, trabajador.rut, trabajador.nombre, apepaterno," +
                      "  apematerno, sexo, fechanac, " +
                      " nacion.nombre as nacion, ecivil.nombre as 'ecivil', direccion, ciudad.descCiudad as ciudad, telefono, " +
                      " tipocontrato, ingreso, salida, area.nombre as area, tramo, afp.nombre as afp, isapre.nombre as salud,  " +
                      " ccosto as 'ccosto', cargo.nombre as cargo, regimensalario, jubilado, " +
                      " regimen.nombre as regimen, cajaprevision.nombre as 'cajaprevision', banco.nombre as banco, formapago.nombre as 'formapago', " +
                      " cuenta, tipocuenta.nombre as 'tipocuenta', anosprogresivo, clase, causalTermino.descCausal as causal, " +
                      " sucursal.descSucursal as sucursal, fun, mail, escolaridad.descesc as esco, jornada FROM trabajador " +
                      " INNER JOIN nacion ON nacion.id = trabajador.nacion" +
                      " INNER JOIN ecivil ON ecivil.id = trabajador.ecivil " +
                      " INNER JOIN ciudad ON ciudad.idCiudad = trabajador.ciudad" +
                      " INNER JOIN area ON area.id = trabajador.area" +
                      " LEFT JOIN afp ON afp.id = trabajador.afp " +
                      " LEFT JOIN isapre ON isapre.id = trabajador.salud " +
                      " INNER JOIN ccosto ON ccosto.id = trabajador.ccosto " +
                      " INNER JOIN cargo ON cargo.id = trabajador.cargo " +
                      " INNER JOIN regimen ON regimen.id = trabajador.regimen " +
                      " LEFT JOIN cajaPrevision ON cajaPrevision.id = trabajador.cajaPrevision " +
                      " INNER JOIN banco ON banco.id = trabajador.banco " +
                      " INNER JOIN formaPago ON formapago.id = trabajador.formapago " +
                      " INNER JOIN tipoCuenta ON tipoCuenta.id = trabajador.tipoCuenta " +
                      " INNER JOIN sucursal ON sucursal.codSucursal = trabajador.sucursal " +
                      " INNER JOIN causalTermino ON causalTermino.codCausal = trabajador.causal " +
                      " INNER JOIN escolaridad ON escolaridad.codesc = trabajador.esco " +
                      " WHERE contrato IN (SELECT contrato FROM trabajador where {0}) " +
                      " AND anomes={1} AND {2} ORDER BY anomes, nombre", condicion, periodo, PreparaCondicion(condUser));
            }                

            SqlCommand cmd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@periodo", periodo));
                        ad.SelectCommand = cmd;
                        ad.Fill(tabla);

                        ad.Dispose();
                        cmd.Dispose();
                        fnSistema.sqlConn.Close();

                        if (tabla.Rows.Count > 0)
                        {
                            return tabla;
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

        //GENERA EXCEL
        private void CrearArchivo(DataTable tabla)
        {
            //RUTA ESCRITORIO
            string ruta = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string file = ruta + @"\Empleados.xlsx";

            if (tabla.Rows.Count>0)
            {
                DataColumnCollection colecion = tabla.Columns;
                Excel._Application ExcelApp = new Excel.Application();
                Excel.Workbooks Libros = ExcelApp.Workbooks;
                //ExcelApp.Application.Workbooks.Add(Type.Missing);
                Libros.Add(Type.Missing);
                Excel._Worksheet hoja = ExcelApp.Worksheets[1];
                Excel._Workbook libro = ExcelApp.Workbooks[1];

                //AGREGAR COLUMNAS
                for (int columna = 0; columna < colecion.Count; columna++)
                {
                    ExcelApp.Cells[1, columna + 1] = colecion[columna].ToString();                    
                }              
                
                ExcelApp.Cells[1, 1].EntireRow.Font.Bold = true;                

                //LLENAMOS LA PLANILLA CON LOS DATOS
                for (int i = 1; i <= tabla.Rows.Count; i++)
                {
                    for (int j = 1; j <= tabla.Columns.Count; j++)
                    {
                        ExcelApp.Cells[i + 1, j] = tabla.Rows[i - 1][j - 1];
                    }
                }

                //PARA QUE LAS COLUMNAS SE AUTOAJUSTEN AL TAMANO DE LA CADENA
                Excel.Range rango = hoja.UsedRange;
                Excel.Range r1 = hoja.UsedRange;
                r1 = r1.Cells[1, 1];
                r1 = r1.EntireRow;
                Excel.Font fuente = r1.Font;                
                fuente.Bold = true;
                //PARA CAMBIAR EL COLOR DEL TEXTO
                //fuente.Color = ColorTranslator.ToOle(Color.DarkOrange);               
                //PARA CAMBIAR EL COLOR DE FONDO DE LA CELDA
               // Excel.Interior back = r1.Interior;
               // back.Color = ColorTranslator.ToOle(Color.AliceBlue);

                Excel.Borders border = rango.Borders;                
                border.LineStyle = Excel.XlLineStyle.xlContinuous;                
                
                rango.Columns.AutoFit();

                //GUARDAMOS ARCHIVO EN EL ESCRITORIO...
                //ExcelApp.ActiveWorkbook.SaveCopyAs(file);
                //ExcelApp.ActiveWorkbook.Saved = true;
                libro.SaveCopyAs(file);
                libro.Saved = true;

                if (File.Exists(file))
                {
                    XtraMessageBox.Show(file + " creado correctamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DialogResult pregunta = XtraMessageBox.Show("¿Deseas abrir el archivo?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (pregunta == DialogResult.Yes)
                    {
                        //OPEN...
                        OpenFile(file);                        
                    }
                }
                else
                {
                    XtraMessageBox.Show("Parece ser que el archivo no se pudo crear", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                //LIMPIAMOS OBJETOS COM
                CleanComObject(hoja);
                libro.Close(false);
                CleanComObject(libro);
                CleanComObject(Libros);
                CleanComObject(r1);
                CleanComObject(fuente);
                CleanComObject(border);
                //CleanComObject(back);
                ExcelApp.Quit();
                CleanComObject(ExcelApp);

                GC.Collect();
                GC.WaitForPendingFinalizers();               
            }
        }

        //LIMPIAR OBJETOS COM
        private void CleanComObject(object objecto)
        {
            try
            {
                while (Marshal.ReleaseComObject(objecto) > 0 )
                {
                    //ITERAMOS
                }
            }
            catch { }
            finally { objecto = null; }
        }

        //periodo existe
        private bool ExistePeriodo(int periodo)
        {
            string sql = "SELECT anomes FROM parametro WHERE anomes=@anomes";
            SqlCommand cmd;
            SqlDataReader rd;
            bool existe = false;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETRO
                        cmd.Parameters.Add(new SqlParameter("@anomes", periodo));
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

        //PERIODO REGISTRA EVENTOS
        private bool PeriodoTieneData(int periodo)
        {
            string sql = "SELECT contrato FROM trabajador WHERE anomes=@periodo";
            SqlCommand cmd;
            SqlDataReader rd;
            bool contieneInfo = false;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMERO
                        cmd.Parameters.Add(new SqlParameter("@periodo", periodo));
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //TIENE REGISTROS
                            contieneInfo = true;
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
            return contieneInfo;
        }

        //MANEJO DE TECLA TAB
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Tab)
            {
                if (txtPeriodo.ContainsFocus)
                {
                    if (cbPeriodo.Checked == false)
                    {
                        if (txtPeriodo.Text == "") { XtraMessageBox.Show("Por favor ingresa un periodo", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);return false; }

                        //VERIFICAR QUE EL PERIODO EXISTE O TIENE REGISTROS
                        if (ExistePeriodo(Convert.ToInt32(txtPeriodo.Text)) == false)
                        { XtraMessageBox.Show("Periodo ingresado no existe", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); return false; }

                        if (PeriodoTieneData(Convert.ToInt32(txtPeriodo.Text)) == false)
                        { XtraMessageBox.Show("Periodo ingresado no contiene registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); return false; }
                    }
                }
                if (txtConjunto.ContainsFocus)
                {
                    if (cbTodos.Checked == false)
                    {
                        if (txtConjunto.Text == "")
                        { XtraMessageBox.Show("Por favor ingresa un conjunto a evaluar", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); return false; }

                        if (Conjunto.ExisteConjunto(txtConjunto.Text) == false)
                        { XtraMessageBox.Show("Parece ser que el conjunto ingresado no existe", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); return false; }
                    }
                }
            }

            return base.ProcessDialogKey(keyData);
        }

        //ABRIR EXCEL
        private void OpenFile(string file)
        {
            Excel.Application excel = new Excel.Application();
            Excel.Workbooks libros = excel.Workbooks;
            Excel.Workbook libro = libros.Open(file);
            Excel.Worksheet wx = excel.ActiveSheet as Excel.Worksheet;
            excel.Visible = true;

            CleanComObject(wx);
            CleanComObject(libro);                   
            CleanComObject(libros);            
            CleanComObject(excel);

            GC.Collect();
            GC.WaitForPendingFinalizers();
           // libro.Close(true, Type.Missing, Type.Missing);
            //excel.Quit();
        }

        //SABER SI UN EXCEL ESTA ABIERTO
        private bool isOpened(string FileName)
        {
            bool isOpen = false;         

            try
            {
                Stream s = File.Open(FileName, FileMode.Open, FileAccess.Read, FileShare.None);
                s.Close();
            }
            catch (Exception ex)
            {              
                isOpen = true;
            }

            return isOpen;
        }

        //GENERAR QUERY DINAMICA EN CASO DE ADMINISTRAR COLUMNAS
        private DataTable CrearTablaDinamica(DataTable tabla)
        {
            if (tabla.Rows.Count > 0 && ListadoCampos.Count>0)
            {
                //CREAR DATATABLE AUXILIAR CON LAS COLUMNAS SELECCIONADAS
                //COLUMNAS
                DataTable auxiliar = new DataTable();
                foreach (var columna in tabla.Columns)
                {
                    if (ExisteColumna(columna.ToString(), ListadoCampos))
                    {                        
                        auxiliar.Columns.Add(columna.ToString(), tabla.Columns[columna.ToString()].DataType);                      
                    }                        
                }

                //UNA VEZ AGREGADAS LAS COLUMNAS
                //IMPORTAMOS LAS FILAS A LA TABLA AUXILIAR
                foreach (DataRow fila in tabla.Rows)
                {
                    auxiliar.ImportRow(fila);
                }


                //RETORNAMOS TABLA AUXILIAR
                return auxiliar;
            }
            else
            {
                return null;
            }
        }

        //TABLA FINAL CON MODIFICACIONES
        private DataTable TablaFinal(DataTable Tabla)
        {
            DataTable Intermedia = new DataTable();
            if (Tabla.Rows.Count>0)
            {
                try
                {
                    //CLONAMOS LA ESTRUCTURA DE LA TABLA
                    Intermedia = Tabla.Clone();

                    //RECORREMOS LAS COLUMNAS Y CAMBIAMOS EL TIPO DE DATO
                    foreach (DataColumn columna in Intermedia.Columns)
                    {
                        if (columna.ColumnName == "sexo")
                            columna.DataType = typeof(string);
                        if (columna.ColumnName == "jubilado")
                            columna.DataType = typeof(string);
                        if (columna.ColumnName == "tipocontrato")
                            columna.DataType = typeof(string);
                        if (columna.ColumnName == "regimensalario")
                            columna.DataType = typeof(string);
                        if (columna.ColumnName == "cajaprevision")
                            columna.DataType = typeof(string);
                        if (columna.ColumnName == "clase")
                            columna.DataType = typeof(string);
                    }

                    //RECORREMOS FILAS Y AGREGAMOS A TABLA
                    foreach (DataRow fila in Tabla.Rows)
                    {
                        Intermedia.ImportRow(fila);
                    }

                    //RECORREMOS TABLA INTERMEDIA
                    for (int fila = 0; fila < Intermedia.Rows.Count; fila++)
                    {
                        for (int columna = 0; columna < Intermedia.Columns.Count; columna++)
                        {
                            if (Intermedia.Columns[columna].ColumnName == "sexo")
                                Intermedia.Rows[fila][columna] = GetSexo(Convert.ToInt32(Intermedia.Rows[fila][columna]));
                            else if (Intermedia.Columns[columna].ColumnName == "regimensalario")
                                Intermedia.Rows[fila][columna] = GetRegimenSalario(Convert.ToInt32(Intermedia.Rows[fila][columna]));
                            else if (Intermedia.Columns[columna].ColumnName == "tipocontrato")
                                Intermedia.Rows[fila][columna] = GetTipoContrato(Convert.ToInt32(Intermedia.Rows[fila][columna]));
                            else if (Intermedia.Columns[columna].ColumnName == "jubilado")
                                Intermedia.Rows[fila][columna] = getJubilado(Convert.ToInt32(Intermedia.Rows[fila][columna]));
                            else if (Intermedia.Columns[columna].ColumnName == "cajaprevision")
                                Intermedia.Rows[fila][columna] = string.IsNullOrEmpty(Intermedia.Rows[fila][columna].ToString()) ? "NO" : getCajaPrevision((string)Intermedia.Rows[fila][columna]);
                            else if (Intermedia.Columns[columna].ColumnName == "ecivil")
                                Intermedia.Columns[columna].ColumnName = "estado civil";
                            else if (Intermedia.Columns[columna].ColumnName == "nacion")
                                Intermedia.Columns[columna].ColumnName = "nacionalidad";
                            else if (Intermedia.Columns[columna].ColumnName == "apepaterno")
                                Intermedia.Columns[columna].ColumnName = "apellido paterno";
                            else if (Intermedia.Columns[columna].ColumnName == "apematerno")
                                Intermedia.Columns[columna].ColumnName = "apellido materno";
                            else if (Intermedia.Columns[columna].ColumnName == "anomes")
                                Intermedia.Columns[columna].ColumnName = "periodo";
                            else if (Intermedia.Columns[columna].ColumnName == "ingreso")
                                Intermedia.Columns[columna].ColumnName = "inicio contrato";
                            else if (Intermedia.Columns[columna].ColumnName == "salida")
                                Intermedia.Columns[columna].ColumnName = "termino contrato";
                            else if (Intermedia.Columns[columna].ColumnName == "ccosto")
                                Intermedia.Columns[columna].ColumnName = "centro costo";
                            else if (Intermedia.Columns[columna].ColumnName == "cuenta")
                                Intermedia.Columns[columna].ColumnName = "numero cuenta";
                            else if (Intermedia.Columns[columna].ColumnName == "anosprogresivo")
                                Intermedia.Columns[columna].ColumnName = "progresivos";
                            else if (Intermedia.Columns[columna].ColumnName == "fechanac")
                                Intermedia.Columns[columna].ColumnName = "nacimiento";
                            else if (Intermedia.Columns[columna].ColumnName == "clase")
                                Intermedia.Rows[fila][columna] = GetClase(Convert.ToInt32(Intermedia.Rows[fila][columna]));
                        }
                    }//END FOR EXTERNO

                    return Intermedia;
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show("Ha ocurrido un error " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }               
            }
            else
            {
                return null;
            }         
        }

        //VER SI COLUMNAS EXISTE EN LISTA
        private bool ExisteColumna(string name, List<string> Lista)
        {
            bool existe = false;
            if (name != "" && Lista.Count>0)
            {
                foreach (var item in Lista)
                {
                    if (name == item)
                    { existe = true; break; }
                }
            }

            return existe;
        }

        //PARA SABER SI EXCEL ESTA INSTALADO
        private bool IsExcelInstalled()
        {
            //...
            bool instalado = false;
            Type officeType = Type.GetTypeFromProgID("Excel.Application");
            if (officeType == null)
            {
                //NO ESTA INSTALLADO!!!
                instalado = false;
            }
            else
            {
                instalado = true;
                //ESTA INSTALADO
            }

            return instalado;
        }

        //PARA MANIPULAR SEXO
        private string GetSexo(int Id)
        {
            string data = "";

            if (Id == 0)
                data = "m";
            else if (Id == 1)
                data = "f";

            return data;
        }

        //JUBILADO
        private string GetRegimenSalario(int Id)
        {
            string data = "";
            if (Id == 0)
            {
                data = "Variable";
            }
            else if (Id == 1)
            {
                data = "Fijo";
            }

            return data;
        }

        //TIPO CONTRATO
        private string GetTipoContrato(int Id)
        {
            string data = "";
            if (Id == 0)
                data = "INDEFINIDO";
            else if (Id == 1)
                data = "FIJO";
            else if (Id == 2)
                data = "OBRA O FAENA";

            return data;
        }

        //JUBILADO
        private string getJubilado(int Id)
        {
            string data = "";
            if (Id == 0)
                data = "NO";
            else if (Id == 1)
                data = "SI, NO COTIZA";
            else if (Id == 2)
                data = "SI, COTIZA";

            return data;
        }

        //CAJA PREVISION
        private string getCajaPrevision(string value)
        {
            string data = "";
            if (value == "")
                return "NO";
            string sql = "SELECT nombre FROM cajaprevision WHERE id=@code";
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETRO
                        cmd.Parameters.Add(new SqlParameter("@code", value));
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                data = (string)rd["nombre"];
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

            return data;
        }

        //DESCRIPCION CLASE
        private string GetClase(int Id)
        {
            string data = "";
            string sql = "SELECT descClase FROM clase WHERE codClase=@codigo";
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@codigo", Id));
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                data = (string)rd["descClase"];
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
            return data;
        }

        #endregion

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

        private void cbPeriodo_CheckedChanged(object sender, EventArgs e)
        {
            if (cbPeriodo.Checked)
            {
                txtPeriodo.Text = Calculo.PeriodoObservado.ToString();
                txtPeriodo.ReadOnly = true;
            }
            else
            {
                txtPeriodo.Text = "";
                txtPeriodo.ReadOnly = false;
                txtPeriodo.Focus();
            }
        }

        private void cbTodos_CheckedChanged(object sender, EventArgs e)
        {
            if (cbTodos.Checked)
            {
                txtConjunto.Enabled = false;
                txtConjunto.Text = "";
            }
            else
            {
                txtConjunto.Enabled = true;
                txtConjunto.Text = "";
                txtConjunto.Focus();
            }
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();           

            //Cursor.Current = Cursors.WaitCursor;

            if (IsExcelInstalled() == false)
            { XtraMessageBox.Show("Parece ser que tu sistema no tiene instalado Microsoft Office", "Informacion Importante", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

            string file = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Empleados.xlsx";
            if (File.Exists(file) && isOpened(file))
            { XtraMessageBox.Show("Por favor cierra el archivo antes de continuar", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);return; }                            

            if (txtPeriodo.Text == "") { XtraMessageBox.Show("Por favor ingresa un periodo a evaluar", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtPeriodo.Focus(); return; }
            if (txtConjunto.Text == "" && cbTodos.Checked == false) { XtraMessageBox.Show("Por favor ingresa un conjunto a evaludar", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);return; }

            DataTable data = new DataTable();
            if (cbTodos.Checked)
            {
                //MOSTRAMOS TODOS LOS REGISTROS PARA EL PERIODO SELECCIONADO
                //GENERAMOS DATATABLE                

                data = TablaFromDb(Convert.ToInt32(txtPeriodo.Text));

                if (data == null) { XtraMessageBox.Show("No se encontraron registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

                //SI LISTADO CAMPOS ES DISTINTO DE CERO ES PORQUE SE DESEA ADMINISTRAR LAS COLUMNAS A MOSTRAR
                if (ListadoCampos.Count > 0)
                    data = CrearTablaDinamica(data);

                data = TablaFinal(data);
                if (data == null) { XtraMessageBox.Show("No se encontraron registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

                DialogResult pregunta = XtraMessageBox.Show("Se detectaron " + data.Rows.Count + "  trabajadores, ¿Estas seguro de generar el archivo?", "Informacion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (pregunta == DialogResult.Yes)
                {
                    Cursor.Current = Cursors.WaitCursor;
                    //CrearArchivo(data);
                    if(FileExcel.CrearArchivoExcelDev(data, file))
                    {
                        DialogResult mensaje =  XtraMessageBox.Show("Archivo " + file + " creado correctamente, ¿Deseas abrir el archivo?", "Informacion", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                        if (mensaje == DialogResult.Yes)
                        {
                            FileExcel.AbrirExcel(file);
                        }
                    }
                }              
            }
            else
            {
                if (Conjunto.ExisteConjunto(txtConjunto.Text) == false) { XtraMessageBox.Show("Parece ser que el conjunto ingresado no existe", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
                
                //BUSCAMOS POR CONJUNTO 
                data = TablaConjuntoFromDb(Convert.ToInt32(txtPeriodo.Text), txtConjunto.Text);

                if (data == null) { XtraMessageBox.Show("No se encontraron registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

                //SI LISTADO CAMPOS ES DISTINTO DE CERO ES PORQUE SE DESEA ADMINISTRAR LAS COLUMNAS A MOSTRAR
                if (ListadoCampos.Count > 0)
                    data = CrearTablaDinamica(data);

                data = TablaFinal(data);
                if (data == null) { XtraMessageBox.Show("No se encontraron registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

                DialogResult pregunta = XtraMessageBox.Show("Se detectaton " + data.Rows.Count + " trabajadores, ¿Estás seguro de generar el archivo?", "Informacion", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (pregunta == DialogResult.Yes)
                {
                    //CrearArchivo(data);
                    if (FileExcel.CrearArchivoExcelDev(data, file))
                    {
                        DialogResult mensaje = XtraMessageBox.Show("Archivo " + file + " creado correctamente, ¿Deseas abrir el archivo?", "Informacion", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                        if (mensaje == DialogResult.Yes)
                        {
                            FileExcel.AbrirExcel(file);
                        }
                    }
                }
            }
        }

        private void textEdit2_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            DXPopupMenu me = e.Menu;
            if (me != null)
            {
                me.Items.Clear();
                DXMenuItem menu = new DXMenuItem("Agregar conjunto", new EventHandler(AgregarConjunto_Click));
                menu.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("images/actions/show_16x16.png");
                me.Items.Add(menu);
            }
        }

        private void AgregarConjunto_Click(object sender, EventArgs e)
        {
            FrmFiltroBusqueda filtro = new FrmFiltroBusqueda(true);
            filtro.opener = this;
            filtro.ShowDialog();
        }

        private void txtConjunto_DoubleClick(object sender, EventArgs e)
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

        private void btnColumnas_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            frmColumnasData colum = new frmColumnasData(true, ListadoCampos);
            colum.opener = this;
            colum.ShowDialog();            
        }

        private void frmExportarEmpleados_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void txtPeriodo_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void groupControl1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}