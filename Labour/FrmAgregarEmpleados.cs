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
using System.IO;
using DevExpress.DataAccess.Excel;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.Data.SqlClient;
using DevExpress.Spreadsheet;
using DevExpress.Spreadsheet.Export;

namespace Labour
{
    public partial class FrmAgregarEmpleados : DevExpress.XtraEditors.XtraForm
    {
        [DllImport("user32")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32")]
        static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        const int MF_BYCOMMAND = 0;
        const int MF_DISABLED = 2;
        const int SC_CLOSED = 0xF060;
        //PARA SABER SI SE CARGO CORRECTAMENTE EL ARCHIVO
        public bool CargaCorrecta { get; set; } = false;

        //Listado solo para generacion de plantilla excel
        public List<string> ListadoPlantilla { get; set; } = new List<string>(){"contrato", "rut", "nombre", "apellidopaterno",
            "apellidomaterno", "sexo", "nacimiento", "nacionalidad", "estadocivil", "direccion", "ciudad", "telefono",
            "tipocontrato", "iniciocontrato", "terminocontrato", "area", "tramo", "afp", "salud", "centrocosto",
            "cargo", "regimensalario", "jubilado", "regimen", "cajaprevision", "banco", "formapago", "numerocuenta",
            "tipocuenta", "progresivos", "fechaprogresivos", "clase", "causal", "sucursal", "fun", "mail", "escolaridad", "fonoemergencia",
            "nombreemergencia", "talla", "calzado", "horario", "jornada", "sindicato", "comuna", "laboral"};

        

        //DATATABLE
        private DataTable TablaEmpleados = null;
        
        public FrmAgregarEmpleados()
        {
            InitializeComponent();
        }

        private void btnCargarArchivo_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            bool instalado = IsExcelInstalled();
            DataTable tabla = new DataTable();
            if (instalado)
            {
                OpenFileDialog open = new OpenFileDialog();
                open.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";

                if (open.ShowDialog() == DialogResult.OK)
                {
                    if (isOpened(open.SafeFileName) == false)
                    {
                        splashScreenManager1.ShowWaitForm();
                        txtFile.Text = open.FileName;
                        lblMensaje.Visible = true;
                        lblMensaje.Text = "Procesando...";

                        //EL ARCHIVO NO ESTA ABIERTO
                        //TablaEmpleados = ReadFile(open.FileName);
                        //TablaEmpleados = ReadFileDev(open.FileName);                        
                        if(Path.GetExtension(open.FileName) == ".xls" || Path.GetExtension(open.FileName) == ".xlsx" || Path.GetExtension(open.FileName) == ".xlsm")
                            TablaEmpleados = FileExcel.ReadExcelDev(open.FileName);
                        else
                            TablaEmpleados = FileExcel.ReadCsv(open.FileName);                        
                        
                        if (TablaEmpleados != null)
                        {
                            if (ExistenColumnas(TablaEmpleados.Columns) == false)
                            { XtraMessageBox.Show("Faltan Columnas", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); splashScreenManager1.CloseWaitForm(); return; }

                            if (ValidaDataTable(TablaEmpleados))
                            {
                                //SI ES TRUE NO HAY ERRORES DE VALIDACION EN DATATABLE...
                                lblMensaje.Text = "Ok. Por favor no manipular el archivo mientras se lee la informacion";
                                CargaCorrecta = true;
                                btnCargarTrabajadores.Enabled = true;
                               // gridControl1.DataSource = TablaEmpleados;                                
                            }
                            else
                            {
                                btnCargarTrabajadores.Enabled = false;
                                lblMensaje.Text = "Parece ser que el archivo tiene errores";
                            }
                        }
                        else
                        {
                            btnCargarTrabajadores.Enabled = false;
                            lblMensaje.Text = "Parece ser que el archivo tiene errores";
                        }
                        splashScreenManager1.CloseWaitForm();
                    }
                    else
                    {
                        XtraMessageBox.Show("Por favor cierra el archivo '" + open.SafeFileName + "' y vuelve al intentarlo", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
            }
            else
            {
                XtraMessageBox.Show("Parece ser que tu sistema no tiene instalado Microsoft Office", "Informacion Importante", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        #region "MANEJO DATA"
        //CARGAR GRILLA CON PLANILLA EXCEL
        private void DataFromExcel(string FileName, DevExpress.XtraGrid.GridControl pGrid)
        {
            if (FileName != null)
            {
                ExcelDataSource dataExcel = new ExcelDataSource();
                dataExcel.FileName = FileName;
                ExcelWorksheetSettings settings = new ExcelWorksheetSettings("Hoja1");
                dataExcel.SourceOptions = new ExcelSourceOptions(settings);

                dataExcel.SourceOptions.SkipEmptyRows = true;                
                dataExcel.SourceOptions.UseFirstRowAsHeader = true;

                dataExcel.Fill();
                pGrid.DataSource = dataExcel;
            }
        }

        //LECTURA ARCHIVO EXCEL
        private DataTable ReadFile(string File)
        {
            Excel.Application ExcelAplication = new Excel.Application();
            //COLECCION DE LIBROS
            Excel.Workbooks libros = ExcelAplication.Workbooks;
            //REPRESENTA EL LIBRO
            Excel.Workbook xlWorkBook = libros.Open(File, 0, true, 5, "", "", true, Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1 , 0);
            //REPRESENTA LA HOJA
            Excel._Worksheet xlWorkSheet = xlWorkBook.Sheets[1];
            //RANGE REPRESENTA TODAS LAS CELDAS QUE CONTIENEN ALGUN VALOR
            Excel.Range range = xlWorkSheet.UsedRange;

            int r = range.Count;            

            //PARA SABER LA CANTIDAD DE HOJAS QUE TIENE EL LIBRO
            int Hojas = ExcelAplication.Sheets.Count;            
         
            //CANTIDAD DE FILAS 
            int rowCount = range.Rows.Count; //RESTAR EL ENCABEZADO
            //CANTIDAD DE COLUMNAS
            int colCount = range.Columns.Count;

            if (Hojas == 0)
            {
                XtraMessageBox.Show("Parece ser que el archivo no tiene hojas", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                LimpiaTodo(xlWorkBook, libros, xlWorkSheet, ExcelAplication, range);
                return null;
            }

            //VERIFICAR QUE LAS COLUMNAS CUMPLAN CON LOS NOMBRES ESTABLECIDOS
            for (int column = 1; column <= colCount; column++)
            {
                if (range.Cells[column].Value == null)
                {
                    XtraMessageBox.Show("El nombre de la columna no puede estar vacio", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    LimpiaTodo(xlWorkBook, libros, xlWorkSheet, ExcelAplication, range);
                    return null;
                }

                string c = range.Cells[column].Value;               

                if (ColumnaValida(range.Cells[column].Value) == false)
                {
                    XtraMessageBox.Show("El nombre de la columna \"" + range.Cells[column].Value + " \" no es valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    LimpiaTodo(xlWorkBook, libros, xlWorkSheet, ExcelAplication, range);
                    return null;
                }
            }

            DataTable tabla = new DataTable();
            tabla = CreaDataTable();
            string NombreColumna = "";
            bool valida = false;           

            try
            {
                for (int i = 2; i <= rowCount; i++)
                {
                    object[] objeto = new object[colCount];
                    
                    for (int j = 1; j <= colCount; j++)
                    {
                        NombreColumna = range.Cells[j].Value.ToString();
                        if (range.Cells[i, j].Value != null)
                        {                    
                            //VALIDAMOS VALOR ACORDE A NOMBRE DE COLUMNA
                            valida = ValidaData(range.Cells[i, j].Value.ToString(), NombreColumna);
                          
                            if (valida == false)
                            {
                                LimpiaTodo(xlWorkBook, libros, xlWorkSheet, ExcelAplication, range);
                                return null;
                            }
                            else
                            {
                                objeto[j - 1] = range.Cells[i, j].Value;
                            }
                        }
                        else
                        {
                            XtraMessageBox.Show("El campo de la columna " + NombreColumna + " no puede estar vacio", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            LimpiaTodo(xlWorkBook, libros, xlWorkSheet, ExcelAplication, range);
                            return null;
                        }                                                              
                    }
                    //LLENAR FILA
                    tabla.Rows.Add(objeto);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                LimpiaTodo(xlWorkBook, libros, xlWorkSheet, ExcelAplication, range);
                return null;
            }

            LimpiaTodo(xlWorkBook, libros, xlWorkSheet, ExcelAplication, range);
           

            return tabla;
        }

        //LIBERAR ARCHIVOS COM
        private void CleanObjectCom(Excel.Workbook libro, Excel.Application Program)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

            libro.Close();
            Marshal.ReleaseComObject(libro);

            Program.Quit();
            Marshal.ReleaseComObject(Program);

        }

        //LIMPIAR OBJETOS COM
        private void CleanComObject(object objecto)
        {
            try
            {
                while (Marshal.ReleaseComObject(objecto) > 0)
                {
                    //ITERAMOS
                }
            }
            catch { }
            finally { objecto = null; }
        }

        //LIMPIAR TODOS LOS COM
        private void LimpiaTodo(Excel.Workbook libro, Excel.Workbooks libros,Excel._Worksheet hoja, Excel.Application Programa, Excel.Range rango)
        {
            libro.Close(false);
            CleanComObject(libro);
            CleanComObject(libros);
            CleanComObject(hoja);
            CleanComObject(rango);
            Programa.Quit();
            CleanComObject(Programa);
        }

        //VALIDAR QUE LOS NOMBRES DE LAS COLUMNAS ESTEN ACORDE A CRITERIO
        private bool ColumnaValida(string name)
        {
            name = NewCadena(name);
            
            bool valida = true;
            //List<string> Columnas = new List<string>() {"contrato", "rut", "nombre", "apellidopaterno",
            //"apellidomaterno", "sexo", "nacimiento", "nacionalidad", "estadocivil", "direccion", "ciudad", "telefono",
            //"tipocontrato", "iniciocontrato", "terminocontrato", "area", "tramo", "afp", "salud", "centrocosto",
            //"cargo", "regimensalario", "jubilado", "regimen", "cajaprevision", "banco", "formapago", "numerocuenta",
            //"tipocuenta", "progresivos", "clase", "causal", "sucursal", "fun", "mail", "escolaridad", "fonoemergencia",
            //"nombreemergencia", "talla", "calzado", "horario", "jornada"};

            int i = 0;
            foreach (var x in ListadoPlantilla)
            {
                //SI EXISTE ES VALIDA
                if (x == name)
                {
                    i++;
                    break;
                }
                    
            }

            if (i == 0)
                valida = false;               


            return valida;
        }

        //VALIDAR QUE EXISTAN TODAS LAS COLUMNAS
        private bool ExistenColumnas(DataColumnCollection columnas)
        {
            //List<string> fields = new List<string>() {"contrato", "rut", "nombre", "apellidopaterno",
            //"apellidomaterno", "sexo", "nacimiento", "nacionalidad", "estadocivil", "direccion", "ciudad", "telefono",
            //"tipocontrato", "iniciocontrato", "terminocontrato", "area", "tramo", "afp", "salud", "centrocosto",
            //"cargo", "regimensalario", "jubilado", "regimen", "cajaprevision", "banco", "formapago", "numerocuenta",
            //"tipocuenta", "progresivos", "clase", "causal", "sucursal", "fun", "mail", "escolaridad", "fonoemergencia",
            //"nombreemergencia", "talla", "calzado", "horario", "jornada"};

            int count = 0;
            bool correcto = false;
            string col = "";

            foreach (DataColumn columnaTabla in columnas)
            {
                col = NewCadena(columnaTabla.ColumnName.ToLower());
              
                foreach (string item in ListadoPlantilla)
                {                   
                    if (col.Equals(item))
                    {
                        count++;
                        break;
                    }
                }
            }
            if (count == ListadoPlantilla.Count)
                correcto = true;

            return correcto;
        }

        //QUITAR ESPACIOS DE CADENA
        private string NewCadena(string Old)
        {
            string cad = "";            
            
            if (Old.Length > 0)
            {
                for (int i = 0;  i < Old.Length;  i++)
                {
                    if (Old[i].ToString() != " ")
                        cad = cad + Old[i];
                }
            }

            return cad;
        }

        //CONOCER EL NOMBRE DE LAS HOJAS DEL ARCHIVO EXCEL
        private void NombreExcel(Excel.Application aplication)
        {
            string name = "";
            foreach (Excel.Worksheet x in aplication.Sheets)
            {
                name = x.Name;
            }
        }

        //CREAR DATATABLE QUE REPRESENTA EL ARCHIVO EXCEL
        private DataTable CreaDataTable()
        {
            DataTable mitabla = new DataTable();

            //COLUMNAS
            mitabla.Columns.Add("periodo", typeof(int));
            mitabla.Columns.Add("contrato", typeof(string));
            mitabla.Columns.Add("rut", typeof(string));
            mitabla.Columns.Add("nombre", typeof(string));
            mitabla.Columns.Add("apellido paterno", typeof(string));
            mitabla.Columns.Add("apellido materno", typeof(string));
            mitabla.Columns.Add("sexo", typeof(string));
            mitabla.Columns.Add("nacimiento", typeof(DateTime));
            mitabla.Columns.Add("nacionalidad", typeof(string));
            mitabla.Columns.Add("estado civil", typeof(string));            
            mitabla.Columns.Add("direccion", typeof(string));
            mitabla.Columns.Add("ciudad", typeof(string));
            mitabla.Columns.Add("telefono", typeof(string));
            mitabla.Columns.Add("tipo contrato", typeof(string));
            mitabla.Columns.Add("inicio contrato", typeof(DateTime));
            mitabla.Columns.Add("termino contrato", typeof(DateTime));
            mitabla.Columns.Add("area", typeof(string));
            mitabla.Columns.Add("tramo", typeof(int));
            mitabla.Columns.Add("afp", typeof(string));
            mitabla.Columns.Add("salud", typeof(string));
            mitabla.Columns.Add("centro costo", typeof(int));
            mitabla.Columns.Add("cargo", typeof(string));
            mitabla.Columns.Add("regimen salario", typeof(string));
            mitabla.Columns.Add("jubilado", typeof(string));
            mitabla.Columns.Add("regimen", typeof(string));
            mitabla.Columns.Add("caja prevision", typeof(string));
            mitabla.Columns.Add("banco", typeof(string));
            mitabla.Columns.Add("forma pago", typeof(string));
            mitabla.Columns.Add("numero cuenta", typeof(string));
            mitabla.Columns.Add("tipo cuenta", typeof(string));
            mitabla.Columns.Add("progresivos", typeof(int));
            mitabla.Columns.Add("clase", typeof(string));
            mitabla.Columns.Add("causal", typeof(string));

            return mitabla;
        }

        //SABER SI UN EXCEL ESTA ABIERTO
        private bool isOpened(string wbook)
        {
            bool isOpen = true;
            Excel.Application exApp;
            
            try
            {
                exApp = (Excel.Application)System.Runtime.InteropServices.Marshal.GetActiveObject("Excel.Application");
                exApp.Workbooks.get_Item(wbook);
            }
            catch (Exception ex)
            {
                isOpen = false;
            }

            return isOpen;
        }

        //PARA VALIDAR CAMPO QUE SE INTENTA GUARDAR
        private bool ValidaData(string value, string ColumnName)
        {           
            if (ColumnName == "")
            { XtraMessageBox.Show("Campo no puede estar vacio", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information); return false; }
       
            switch (ColumnName)
            {
                case "contrato":
                    if (value == "") { XtraMessageBox.Show("N° contrato no puede estar vacío", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return false; }
                    if (value.Length > 15) { XtraMessageBox.Show("Contrato excede el largo permitido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);return false; }
                    //if (CaracterRaro(value)) { XtraMessageBox.Show("Caracter no permitido en contrato", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);return false; }                    
                    //CONTRATO EXISTE PARA PERIODO
                    break;
                case "rut":
                    string rut = value.Trim();
                
                    if (rut == "") { XtraMessageBox.Show("Rut no puede estar vacío", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return false; }
                    if (CaracterRaro(rut)) { XtraMessageBox.Show("Caracter no permitido en rut", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);return false; }
                    if (rut.Length > 9) { XtraMessageBox.Show("Rut excede el largo permitido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);return false; }
                    rut = fnSistema.fEnmascaraRut(rut);                 
                    if(fnSistema.fValidaRut(rut) == false) { XtraMessageBox.Show($"Rut {value} no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);return false; };
                    
                    break;
                case "pasaporte":
                    if (value.Length > 9) { XtraMessageBox.Show("Pasaporte excede el largo permitido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);return false; };
                    if (CaracterRaro(value)) { XtraMessageBox.Show("Caracter no permitido en pasaporte", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);return false; }
                    break;             
                case "nombre":
                    if (value == "") { XtraMessageBox.Show("Nombre no puede estar vacío", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return false; }
                    if (value.Length > 50) { XtraMessageBox.Show("Nombre excede el largo permitido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);return false; };
                    if (CaracterRaro(value)) { XtraMessageBox.Show("Caracter no permitido en nombre", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);return false; }
                    break;
                case "apellidopaterno":
                    if (value == "") { XtraMessageBox.Show("Apellido paterno no puede estar vacío", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return false; }
                    if (value.Length > 50) { XtraMessageBox.Show("Apellido paterno execede el largo permitido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);return false; };
                    if (CaracterRaro(value)) { XtraMessageBox.Show("Caracter no permitido en apellido paterno", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);return false; }
                    break;
                case "apellidomaterno":
                    if (value == "") { XtraMessageBox.Show("Apellido materno no puede estar vacío", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return false; }
                    if (value.Length > 50) { XtraMessageBox.Show("Apellido materno excede el largo permitido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);return false; };
                    if (CaracterRaro(value)) { XtraMessageBox.Show("Caracter no permitido en apellido materno", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);return false; }
                    break;
                case "direccion":                    
                    if (value.Length > 100) { XtraMessageBox.Show("Direccion excede el largo permitido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);return false; }
                    break;
                case "ciudad":

                    if (value.Length == 0)
                    { XtraMessageBox.Show("Por favor ingresa un código de ciudad válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    if (fnSistema.IsNumeric(value) == false)
                    { XtraMessageBox.Show("Por favor ingresa un codigo de ciudad", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    if (CiudadExiste(Convert.ToInt32(value)) == false) { XtraMessageBox.Show("No existe el código de ciudad " + value, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);return false; }

                    break;
                case "area":

                    if (value.Length == 0)
                    { XtraMessageBox.Show("Por favor ingresa un codigo de area válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    if (fnSistema.IsNumeric(value) == false)
                    { XtraMessageBox.Show("Por favor ingresa un código de area válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    if (existeDato(Convert.ToInt32(value), "AREA") == false)
                    { XtraMessageBox.Show($"Código de area {value} no existe", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }
                       
                    break;
                case "centrocosto":
                    if (fnSistema.IsNumeric(value) == false)
                    { XtraMessageBox.Show("Por favor ingresa un valor numérico en centro de costo", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }
                    if (ExisteCentroCosto(Convert.ToInt32(value)) == false) { XtraMessageBox.Show("No existe el centro costo " + value, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);return false; }
                    break;
                case "nacimiento":                 
                    if (value.Trim() == "") { XtraMessageBox.Show("Fecha nacimiento no puede estar vacío", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return false; }
                    if (ValidarFormatoFecha(value) == false) { XtraMessageBox.Show("Por favor verifica que la fecha de nacimiento tengo el formato correcto", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }
                    if (Convert.ToDateTime(value) > DateTime.Now.Date) { XtraMessageBox.Show($"Por favor verifica la fecha de nacimiento {value}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }
                    if (NacimientoValido(Convert.ToDateTime(value)) == false) { XtraMessageBox.Show($"Por favor verifica la fecha de nacimiento {value}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);return false; }
                    break;
                case "nacionalidad":

                    if (value.Length == 0)
                    { XtraMessageBox.Show("Por favor ingresa un codigo de nacionalidad válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    if (fnSistema.IsNumeric(value) == false)
                    { XtraMessageBox.Show("Por favor ingresa un codigo de nacionalidad válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    if (existeDato(Convert.ToInt32(value), "NACION") == false) { XtraMessageBox.Show("No existe el código de nacionalidad " + value, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);return false; }
                    break;
                case "estadocivil":

                    if (value.Length == 0)
                    { XtraMessageBox.Show("Por favor ingrese un código de estado civil válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    if (fnSistema.IsNumeric(value) == false)
                    { XtraMessageBox.Show("Por favor ingrese un código de estado civil válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }
                    
                    if (existeDato(Convert.ToInt32(value), "ECIVIL") == false) { XtraMessageBox.Show("No existe código estado civil " + value, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }
                    break;
                case "telefono":
                    if (value.Length > 50) { XtraMessageBox.Show("Telefono excede el largo permitido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);return false; };
                    
                    //if (CaracterRaro(value)) { XtraMessageBox.Show("Caracter no permitido en telefono", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);return false; }
                    break;
                case "iniciocontrato":
                    
                    if (value == "") { XtraMessageBox.Show("Fecha ingreso no puede estar vacío", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return false; }
                    if (ValidarFormatoFecha(value) == false) { XtraMessageBox.Show("Verifica que la fecha de ingreso tenga el formato correcto", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }
                    //if (Convert.ToDateTime(value) > DateTime.Now.Date) { XtraMessageBox.Show("Por favor verifica que la fecha de ingreso no sea mayor al dia de hoy", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);return false; }
                    break;
                case "terminocontrato":
                    if (value == "") { XtraMessageBox.Show("Fecha salida no puede estar vacío", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return false; }
                    if (ValidarFormatoFecha(value) == false) { XtraMessageBox.Show("Verifica que la fecha de salida tenga el forma correcto", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }
                    break;
                case "cargo":

                    if (value.Length == 0)
                    { XtraMessageBox.Show("Por favor ingresa un código de cargo válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    if (fnSistema.IsNumeric(value) == false)
                    { XtraMessageBox.Show("Por favor ingresa un código de cargo válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    if (existeDato(Convert.ToInt32(value), "CARGO") == false) { XtraMessageBox.Show("No existe cargo " + value, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }
                    break;
                case "tipocontrato":

                    if (value.Length == 0)
                    { XtraMessageBox.Show("Por favor ingresa un codigo en tipo de contrato", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    if(fnSistema.IsNumeric(value) == false)
                        if (value.Length == 0)
                        { XtraMessageBox.Show("Por favor ingresa un codigo en tipo de contrato", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    if (ExisteTipoContrato(Convert.ToInt32(value)) == false) { XtraMessageBox.Show("Tipo de contrato " + value + " no existe", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }
                    break;
                case "regimensalario":
                    if (ExisteRegimenSalario(value.ToLower()) == false) { XtraMessageBox.Show("El regimen salarial " + value + " no existe", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }
                    break;
                case "jubilado":

                    if (value.Length == 0)
                    { XtraMessageBox.Show("Por favor ingresa un código válido en columna jubilado", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    if(fnSistema.IsNumeric(value) == false)
                    { XtraMessageBox.Show("Por favor ingresa un código válido en columna jubilado", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    if (ExisteJubilado(Convert.ToInt32(value)) == false) { XtraMessageBox.Show("No existe valor " + value + " para jubilado", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }
                    break;
                case "regimen":

                    if (value.Length == 0)
                    { XtraMessageBox.Show("Por favor ingresa un código de regimen válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    if(fnSistema.IsNumeric(value) == false)
                    { XtraMessageBox.Show("Por favor ingresa un código de regimen válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    if (existeDato(Convert.ToInt32(value), "REGIMEN") == false) { XtraMessageBox.Show("No existe regimen " + value, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);return false; }
                    break;
                case "afp":

                    if (value.Length == 0)
                    { XtraMessageBox.Show("Por favor ingresa un código de afp válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    if(fnSistema.IsNumeric(value) == false)
                    { XtraMessageBox.Show("Por favor ingresa un código de afp válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    if (existeDato(Convert.ToInt32(value), "AFP") == false) { XtraMessageBox.Show("No existe afp " + value, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);return false; }
                    break;
                case "salud":

                    if (value.Length == 0)
                    { XtraMessageBox.Show("Por favor ingresa un codigo de salud válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    if(fnSistema.IsNumeric(value) == false)
                    { XtraMessageBox.Show("Por favor ingresa un codigo de salud válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    if (existeDato(Convert.ToInt32(value), "ISAPRE") == false) { XtraMessageBox.Show("No existe salud " + value, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }
                    break;
                case "cajaprevision":
                    if (value.Length == 0)
                    { XtraMessageBox.Show("Por favor ingresa un codigo de caja de prevision válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    if (fnSistema.IsNumeric(value) == false)
                    { XtraMessageBox.Show("Por favor ingresa un codigo de caja de prevision válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }
                
                    if (existeDato(Convert.ToInt32(value), "CAJAPREVISION") == false) { XtraMessageBox.Show("No existe caja prevision " + value, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }
                    break;
                case "fechasegurocesantia":
                    //..
                    break;
                case "tramo":
                    if (fnSistema.IsNumeric(value) == false)
                    { XtraMessageBox.Show("Por favor ingresa un valor numerico en tramo", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }
                    if (Convert.ToInt32(value) < 1 || Convert.ToInt32(value) > 4) { XtraMessageBox.Show("Tramo no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);return false; };
                    break;
                case "formapago":

                    if (value.Length == 0)
                    { XtraMessageBox.Show("Por favor ingresa un codigo de forma de pago válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    if(fnSistema.IsNumeric(value) == false)
                    { XtraMessageBox.Show("Por favor ingresa un codigo de forma de pago válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    if (existeDato(Convert.ToInt32(value), "FORMAPAGO") == false) { XtraMessageBox.Show("No existe la forma de pago " + value, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);return false; }
                    break;
                case "numerocuenta":
                    if (value.Length > 50) { XtraMessageBox.Show("Numero de cuenta excede el largo permitido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);return false; };
                    //if (CaracterRaro(value)) { XtraMessageBox.Show("Caracter no permitido en numero de cuenta"); return false; }
                    break;
                case "fechavacaciones":
                    //...
                    break;
                case "fechaprogresivos":
                    if (ValidarFormatoFecha(value) == false) { XtraMessageBox.Show("Por favor verifica el valor ingresado en columna fecha progresivos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }
                    break;
                case "progresivos":

                    if (value.Length == 0)
                    { XtraMessageBox.Show("Por favor ingresa un numero en campo progresivos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    if(fnSistema.IsNumeric(value) == false)
                    { XtraMessageBox.Show("Por favor ingresa un numero en campo progresivos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    if (Convert.ToDecimal(value) < 0) { XtraMessageBox.Show("Por favor verifica que los dias progresivos sean mayor o igual a 0", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);return false; }
                    if (Convert.ToDecimal(value) > 300) { XtraMessageBox.Show("Por favor verifica la cantidad de dias progresivos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }
                    break;
                case "tipocuenta":

                    if (value.Length == 0)
                    { XtraMessageBox.Show("Por favor ingresa un codigo de tipo de cuenta válido", "Error",MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    if(fnSistema.IsNumeric(value) == false)
                    { XtraMessageBox.Show("Por favor ingresa un codigo de tipo de cuenta válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    if (existeDato(Convert.ToInt32(value), "TIPOCUENTA") == false) { XtraMessageBox.Show("No existe el tipo de cuenta " + value, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);return false; }

                    break;               
                case "causal":

                    if (value.Length == 0)
                    { XtraMessageBox.Show("Por favor ingresa un codigo de causal de termino válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    if(fnSistema.IsNumeric(value) == false)
                    { XtraMessageBox.Show("Por favor ingresa un codigo de causal de termino válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    if (ExisteCausalTermino(Convert.ToInt32(value)) == false) { XtraMessageBox.Show("No existe la causal de termino " + value, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);return false; }
                    break;
                case "sexo":
                    if (value.ToLower() != "m" && value.ToLower() != "f") { XtraMessageBox.Show("Por favor verifiva valor para columna sexo", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);return false; };
                    break;
                case "clase":

                    if (value.Length == 0)
                    { XtraMessageBox.Show("Por favor ingresa un código de clase", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    if(fnSistema.IsNumeric(value) == false)
                    { XtraMessageBox.Show("Por favor ingresa un código de clase", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    if (ExisteClase(Convert.ToInt32(value)) == false) { XtraMessageBox.Show("No existe la clase " + value, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);return false; }
                    break;
                case "sucursal":
                    if (value == "")
                    { XtraMessageBox.Show("Por favor ingresa una sucursal válida", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    if(fnSistema.IsNumeric(value) == false)
                    { XtraMessageBox.Show("Por favor ingresa una sucursal válida", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    if (ExisteSucursal(Convert.ToInt32(value)) == false)
                    { XtraMessageBox.Show("Sucursal ingresada no existe", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }
                    break;
                case "banco":
                    if (value == "")
                    { XtraMessageBox.Show("Por favor ingresar un banco válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    if(fnSistema.IsNumeric(value) == false)
                    { XtraMessageBox.Show("Por favor ingresa un codigo de banco válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    if (existeDato(Convert.ToInt32(value), "banco") == false)
                    { XtraMessageBox.Show($"No existe banco {value}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }
                    break;
                case "fun":
                    if (CaracterRaro(value))
                    { XtraMessageBox.Show("Caracter no permitido en columna fun", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return false; }

                    if (fnSistema.IsNumeric(value) == false)
                    { XtraMessageBox.Show("Por favor ingresa un valor numerico en la columna fun", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }
                    break;

                case "mail":
                    if (value.Length > 150)
                    { XtraMessageBox.Show("El campo mail supera el largo permitido", "Mail", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }                    
                    break;
                case "escolaridad":
                    Escolaridad esco = new Escolaridad();

                    if (value.Length == 0)
                    { XtraMessageBox.Show("Por favor ingresa una escolaridad válida", "Escolaridad", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    if (fnSistema.IsNumeric(value) == false)
                    { XtraMessageBox.Show("Por favor ingresa un código válido de escolaridad", "Escolaridad", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    if (esco.ExisteCodigo(Convert.ToInt32(value)) == false)
                    { XtraMessageBox.Show("Por favor ingresa un código válido para escolaridad", "Escolaridad", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }
                    break;

                case "horario":

                    Horario hor = new Horario();
                    if (value.Length == 0)
                    {
                        XtraMessageBox.Show("Por favor ingresa codigo horario", "horario", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return false;
                    }

                    if (fnSistema.IsNumeric(value) == false)
                    { XtraMessageBox.Show("Por favor ingresa un código de horario válido", "horario", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    if (hor.ExisteRegistro(Convert.ToInt32(value)) == false)
                    { XtraMessageBox.Show("Por favor ingresa un código de horario válido", "horario", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    break;

                case "jornada":
                    if (value.Length == 0)
                    { XtraMessageBox.Show("Por favor ingresa un código de jornada laboral válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    if (fnSistema.IsNumeric(value) == false)
                    { XtraMessageBox.Show("Por favor ingresa un código de jornada", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    //CODIGO VALIDO 1, 2, 3
                    if (fnSistema.IsNumeric(value))
                    {
                        if (ExisteJornada(Convert.ToInt32(value)) == false)
                        { XtraMessageBox.Show($"Código {value} no es un código de jornada laboral válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }
                    }

                    break;

                case "sindicato":

                    Sindicato sin = new Sindicato();

                    if (value.Length == 0)
                    { XtraMessageBox.Show("Por favor ingresa un código válido para columna sindicato", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    if (fnSistema.IsNumeric(value) == false)
                    { XtraMessageBox.Show("Por favor ingresa un código válido para columna sindicato", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    if (sin.Existe(Convert.ToInt32(value)) == false)
                    { XtraMessageBox.Show($"código {value} no es válido en columna sindicato", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    break;

                case "comuna":                   

                    if (value.Length == 0)
                    { XtraMessageBox.Show("Por favor ingresa un código válido para columna Comuna", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    if (fnSistema.IsNumeric(value) == false)
                    { XtraMessageBox.Show("Por favor ingresa un código válido para columna Comuna", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    if (fnSistema.ExisteComuna(Convert.ToInt32(value)) == false)
                    {
                        XtraMessageBox.Show($"Codigo {value} no válido como comuna", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return false;
                    }

                    break;

                case "laboral":
                    if (value.Length == 0)
                    { XtraMessageBox.Show("Por fasvor ingresa un código válido para columna laboral", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }
        
                    if (fnSistema.IsNumeric(value) == false)
                    { XtraMessageBox.Show("Por favor ingresa un código válido para columna laboral", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    if(value.ToString() != "0" && value.ToString() != "13" && value.ToString() != "14" && value.ToString() != "15")
                    { XtraMessageBox.Show("Por favor ingresa un código válido para columna laboral", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }


                    break;

                default:
                    break;              
            }

            return true;
        }

        //VERIFICAR SI EL CONTRATO EXISTE PARA EL PERIODO EVALUADO
        private bool ExisteContratoenPeriodo(string contrato, int periodo, SqlTransaction tr)
        {
            string sql = "SELECT contrato FROM trabajador WHERE anomes=@periodo AND contrato=@contrato";            
           
            SqlCommand cmd;
            SqlDataReader rd;
            bool existe = false;
            try
            {               
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@periodo", periodo));
                        cmd.Parameters.Add(new SqlParameter("@contrato", contrato));
                        cmd.Transaction = tr;
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
                rd.Close();
              
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return existe;           
        }

        //VALIDAR CARACTERES RAROS EN CADENAS
        private bool CaracterRaro(string cadena)
        {
            string[] caracter = new string[] {"#", "(", ")", "|", "°", "¬", "!", "$", "@",
                "%", "&", "/", "?", "'", "¿", "¡","-", "*", "+", "¨", "~", "^", "`", ".",
                ",", ":", "-", ";", "_", "[", "]", "=", "{", "}", " \" ", "´", ">", "<"};

            bool Novalido = false;
            if (cadena.Length>0)
            {
                for (int i = 0; i < cadena.Length; i++)
                {
                    //RECORREMOS ARREGLO CARACTER
                    for (int c = 0; c < caracter.Length; c++)
                    {
                        if (cadena[i].ToString() == caracter[c])
                        {
                            Novalido = true;
                            break;
                        }                            
                    }
                }
            }

            return Novalido;
        }

        //METODO PARA INGRESO DE TRABAJADORES
        private bool IngresoTrabajadores(DataTable TablaEmpleados)
        {
            if (TablaEmpleados.Rows.Count == 0) return false;
            int codRegimen = 0;

            //--------------------------------------------------
            //QUERY SQL PARA INGRESAR EL NUEVO TRABAJADOR       |
            //--------------------------------------------------

            string sqlTrabajador = "INSERT INTO trabajador " +
                "(rut, nombre, apepaterno, apematerno, direccion," +
                "ciudad, area, ccosto, fechanac, nacion, ecivil, telefono, ingreso, salida, cargo," +
                " tipocontrato, regimenSalario, regimen, afp, salud, fechaSegCes, tramo, formapago, banco, " +
                "cuenta, fechavacacion, fechaprogresivo, anosprogresivo, status, rutafoto, contrato," +
                " anomes, jubilado, cajaPrevision, tipoCuenta, clase, causal, sexo, pasaporte, sucursal, " +
                "fun, mail, esco, numemer, nomemer, talla, calzado, horario, jornada, sindicato, comuna, suslab) VALUES (" +
                "@pRut, @pNombre, @pApePat, @pApeMat, @pDirec, @pCiudad, @pArea, @pcCosto, @pNac, @pNacion," +
                "@peCivil, @pFono, @pIngreso, @pSalida, @pCargo, @ptContr, @pregSal, @pReg, @pAfp, @pSalud, @psegCes," +
                "@pTramo, @pformPag, @pBanco, @pCuenta, @pVac, @pfeProg, @pAnProg, @pEstado, @pImage, " +
                "@pContrato, @pAnoMes, @pJub, @pPrev, @pTipoCuenta, @pclase, @pCausal, @psexo, @pPasa, @pSucursal, " +
                "@pFun, @pMail, @pEsco, @pNumEmer, @pNomEmer, @pTalla, @pCalzado, @pHorario, @pJornada, " +
                "@pSindicato, @pComuna, @psuslab)";

            //-------------------------------------------------------------------
            // QUERY PARA INSERT MASIVO DE ITEM DE ACUERDO A CLASE SELECCIONADA |
            //-------------------------------------------------------------------
            string sqlItemClase = "";
            //string sqlItemClase = "INSERT INTO itemtrabajador(rut, contrato, anomes, coditem, formula, tipo, orden, numitem) " +
            //                      "SELECT rut, contrato, anomes, itemclase.item, itemclase.formula, tipo, orden, row_number() OVER(ORDER BY numitem)  FROM( " +
            //                            "SELECT rut, contrato, anomes, itemClase.item, itemClase.formula, tipo, orden, numitem " +
            //                                    "FROM trabajador inner JOIN clase on clase.codClase = trabajador.clase " +
            //                                    "INNER JOIN itemclase on itemClase.clase = clase.codClase " +
            //                                    "INNER JOIN item on item.coditem = itemClase.item " +
            //                                    "WHERE contrato = @pcontrato AND clase.codClase = @pclase AND trabajador.anomes = @periodo " +
            //                                    "EXCEPT " +
            //                                    "SELECT itemTrabajador.rut, itemTrabajador.contrato, trabajador.anomes, " +
            //                                    "itemTrabajador.coditem, itemTrabajador.formula, itemTrabajador.tipo, itemtrabajador.orden, itemclase.numitem " +
            //                                    "FROM itemTrabajador INNER JOIN trabajador on trabajador.contrato = itemTrabajador.contrato " +
            //                                    "INNER JOIN clase on clase.codClase = trabajador.clase " +
            //                                    "INNER JOIN itemClase on itemClase.clase = clase.codClase " +
            //                                    "WHERE itemTrabajador.contrato = @pcontrato " +
            //                                   "AND clase.codClase = @pclase AND itemtrabajador.anomes = @periodo " +
            //                       ")itemclase";

            sqlItemClase = Calculo.GetItemClaseSql();


            SqlCommand cmd;
            SqlTransaction tr;
            bool transaccionCorrecta = false;

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    tr = fnSistema.sqlConn.BeginTransaction();

                    //RECORRER CADA FILA DEL DATATABLE POR CADA COLUMNA ASOCIAR A CAMPO DE BD
                    foreach (DataRow fila in TablaEmpleados.Rows)
                    {
                        try
                        {
                            //1@ INGRESO TRABAJADOR...
                            using (cmd = new SqlCommand(sqlTrabajador, fnSistema.sqlConn))
                            {
                                //VERIFICAR QUE LA FECHA DE INICIO DE CONTRATO NO SEA MAYOR A LA FECHA DE TERMINO DE CONTRATO
                                if (FechasContratoValidas(Convert.ToDateTime(fila["iniciocontrato"]), Convert.ToDateTime(fila["terminocontrato"])) == false)
                                { XtraMessageBox.Show("Verifica que la fecha de inicio de contrato no sea mayor a la fecha de termino de contrato para el contrato " + fila["contrato"].ToString(), "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);return false; }

                                if (Convert.ToDateTime(fila["iniciocontrato"]) > fnSistema.UltimoDiaMes(Calculo.PeriodoObservado))
                                { XtraMessageBox.Show("La fecha de inicio de contrato no puede ser mayor al periodo evaluado para contrato " + fila["contrato"].ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                                if(Convert.ToDateTime(fila["terminocontrato"]) < fnSistema.PrimerDiaMes(Calculo.PeriodoObservado))
                                { XtraMessageBox.Show("La fecha de termino de contrato no puede ser menor al periodo evaluado para contrato " + fila["contrato"].ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                                //FECHA PROGRESIVOS MENOR A FECHA DE INICIO DE CONTRATO
                                if(Convert.ToDateTime(fila["fechaprogresivos"]) < Convert.ToDateTime(fila["iniciocontrato"]))
                                { XtraMessageBox.Show("Por favor verifica que la fecha de progresivos no sea menor a la fecha de inicio de contrato", "Error Progresivos", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                                //VERIFICAR QUE EL NUMER DE CONTRATO QUE SE INTENTAR REGISTRA NO EXISTA PARA ESTE PERIODO
                                if (ExisteContratoenPeriodo(fila["contrato"].ToString(), Calculo.PeriodoObservado,tr))
                                { XtraMessageBox.Show("Contrato " + fila["contrato"].ToString() + " ya existe para el periodo seleccionado", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);return false; }

                                codRegimen = Convert.ToInt32(fila["regimen"]);

                                //PARAMETROS
                                cmd.Parameters.Add(new SqlParameter("@pRut", fnSistema.fEnmascaraRut(fila["rut"].ToString().Trim())));
                                cmd.Parameters.Add(new SqlParameter("@pNombre", (fila["nombre"].ToString().Trim()).ToUpper()));
                                cmd.Parameters.Add(new SqlParameter("@pApePat", (fila["apellidopaterno"].ToString().Trim()).ToUpper()));
                                cmd.Parameters.Add(new SqlParameter("@pApeMat", (fila["apellidomaterno"].ToString().Trim()).ToUpper()));
                                cmd.Parameters.Add(new SqlParameter("@pDirec", (fila["direccion"].ToString()).ToUpper().Trim()));
                                cmd.Parameters.Add(new SqlParameter("@pCiudad", fila["ciudad"].ToString()));
                                cmd.Parameters.Add(new SqlParameter("@pArea", fila["area"].ToString()));
                                cmd.Parameters.Add(new SqlParameter("@pcCosto", fila["centrocosto"]));
                                cmd.Parameters.Add(new SqlParameter("@pNac", fila["nacimiento"]));
                                cmd.Parameters.Add(new SqlParameter("@pNacion", fila["nacionalidad"]));
                                cmd.Parameters.Add(new SqlParameter("@peCivil", fila["estadocivil"]));
                                cmd.Parameters.Add(new SqlParameter("@pFono", fila["telefono"].ToString().Trim()));
                                cmd.Parameters.Add(new SqlParameter("@pIngreso", fila["iniciocontrato"]));
                                //SI TIPO CONTRATO ES INDEFINIDO GUARDAMOS CON 01-01-3000
                                cmd.Parameters.Add(new SqlParameter("@pSalida", Convert.ToInt32(fila["tipocontrato"]) == 0 ? Convert.ToDateTime("01-01-3000") : fila["terminocontrato"]));
                                cmd.Parameters.Add(new SqlParameter("@pCargo", fila["cargo"]));
                                cmd.Parameters.Add(new SqlParameter("@ptContr", fila["tipocontrato"]));
                                cmd.Parameters.Add(new SqlParameter("@pregSal", GetIdRegimenSalarial((fila["regimensalario"].ToString()).ToLower())));
                                cmd.Parameters.Add(new SqlParameter("@pReg", fila["regimen"]));
                                cmd.Parameters.Add(new SqlParameter("@pAfp", codRegimen == 1? fila["afp"] : 0));
                                cmd.Parameters.Add(new SqlParameter("@pSalud", (codRegimen == 1 || codRegimen == 2 || codRegimen ==4 || codRegimen == 5 )? fila["salud"] : 0));
                                cmd.Parameters.Add(new SqlParameter("@psegCes", fila["iniciocontrato"]));
                                cmd.Parameters.Add(new SqlParameter("@pTramo", fila["tramo"]));
                                cmd.Parameters.Add(new SqlParameter("@pformPag", fila["formapago"]));
                                cmd.Parameters.Add(new SqlParameter("@pBanco", fila["banco"].ToString()));
                                cmd.Parameters.Add(new SqlParameter("@pCuenta", fila["numerocuenta"].ToString().Trim()));                                
                                cmd.Parameters.Add(new SqlParameter("@pVac", fila["iniciocontrato"]));
                                cmd.Parameters.Add(new SqlParameter("@pfeProg", fila["fechaprogresivos"]));
                                cmd.Parameters.Add(new SqlParameter("@pAnProg", fila["progresivos"]));
                                cmd.Parameters.Add(new SqlParameter("@pEstado", 1));
                                cmd.Parameters.Add("@pImage", System.Data.SqlDbType.VarBinary).Value = DBNull.Value;
                                cmd.Parameters.Add(new SqlParameter("@pPasa", "0"));
                                cmd.Parameters.Add(new SqlParameter("@pContrato", fila["contrato"].ToString().Trim()));
                                cmd.Parameters.Add(new SqlParameter("@pAnoMes", Calculo.PeriodoObservado));
                                cmd.Parameters.Add(new SqlParameter("@pJub", fila["jubilado"]));
                                cmd.Parameters.Add(new SqlParameter("@pPrev", fila["cajaprevision"]));
                                cmd.Parameters.Add(new SqlParameter("@pTipoCuenta", fila["tipocuenta"]));
                                cmd.Parameters.Add(new SqlParameter("@pclase", fila["clase"]));
                                cmd.Parameters.Add(new SqlParameter("@pCausal", fila["causal"]));
                                cmd.Parameters.Add(new SqlParameter("@psexo", fila["sexo"].ToString().ToLower() == "m"? 0: 1));
                                cmd.Parameters.Add(new SqlParameter("@pSucursal", fila["sucursal"]));
                                cmd.Parameters.Add(new SqlParameter("@pFun", Convert.ToInt32(fila["salud"]) == 1? "0": fila["fun"].ToString()));                                
                                cmd.Parameters.Add(new SqlParameter("@pMail", fila["mail"] == DBNull.Value ? "": fila["mail"]));
                                cmd.Parameters.Add(new SqlParameter("@pEsco", Convert.ToInt32(fila["escolaridad"])));
                                cmd.Parameters.Add(new SqlParameter("@pNumEmer", fila["fonoemergencia"].ToString().Trim()));
                                cmd.Parameters.Add(new SqlParameter("@pNomEmer", fila["nombreemergencia"].ToString().Trim()));
                                cmd.Parameters.Add(new SqlParameter("@pTalla", fila["talla"].ToString().Trim()));
                                cmd.Parameters.Add(new SqlParameter("@pCalzado", fila["calzado"].ToString().Trim()));
                                cmd.Parameters.Add(new SqlParameter("@pHorario", Convert.ToInt32(fila["horario"])));
                                cmd.Parameters.Add(new SqlParameter("@pJornada", Convert.ToInt16(fila["jornada"])));
                                cmd.Parameters.Add(new SqlParameter("@pSindicato", Convert.ToInt32(fila["sindicato"])));
                                cmd.Parameters.Add(new SqlParameter("@pComuna", Convert.ToInt32(fila["comuna"])));
                                cmd.Parameters.Add(new SqlParameter("@psuslab", Convert.ToInt32(fila["laboral"])));

                                cmd.Transaction = tr;
                                cmd.ExecuteNonQuery();

                                cmd.Parameters.Clear();                             
                            }                        

                            //2@ INGRESO CLASE
                            using (cmd = new SqlCommand(sqlItemClase, fnSistema.sqlConn))
                            {
                                //PARAMETROS
                                cmd.Parameters.Add(new SqlParameter("@pContrato", fila["contrato"].ToString().Trim()));                       
                                cmd.Parameters.Add(new SqlParameter("@pClase", fila["clase"]));
                                cmd.Parameters.Add(new SqlParameter("@pPeriodo", Calculo.PeriodoObservado));

                                cmd.Transaction = tr;
                                cmd.ExecuteNonQuery();

                                cmd.Parameters.Clear();
                            }

                            transaccionCorrecta = true;
                        }
                        catch (SqlException ex)
                        {
                            XtraMessageBox.Show(ex.Message);
                            //SI HAY ALGUN ERROR HACEMOS ROLLBACK
                            tr.Rollback();
                            transaccionCorrecta = false;
                            return false;
                        }
                    }//END FOREACH
                    if (transaccionCorrecta)
                    {
                        tr.Commit();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return transaccionCorrecta;
        }

        //VALIDAR QUE LA FECHA TIENE EL CORRECTO FORMATO DE UNA FECHA
        private bool ValidarFormatoFecha(string cadena)
        {
            DateTime fecha = DateTime.Now.Date;
            if (cadena.Length>0)
            {               
                try
                {
                    fecha = Convert.ToDateTime(cadena);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
           
        }

        //VALIDAR CIUDAD
        private bool CiudadExiste(int ciudad)
        {
            if (ciudad == 0)
                return false;

            bool existe = false;
            if (ciudad != 0)
            {
                string sql = "SELECT count(*) FROM ciudad WHERE idCiudad=@pCiudad";
                SqlCommand cmd;
                SqlDataReader rd;
                try
                {
                    if (fnSistema.ConectarSQLServer())
                    {
                        using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@pCiudad", ciudad));

                            object data = cmd.ExecuteScalar();
                            if (data != null)
                            {
                                if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                                    existe = true;
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

            return existe;
        }

        //VALIDAR AREA
        private bool existeDato(int value, string tabla)
        {
            bool existe = false;

            string sql = string.Format("SELECT count(*) FROM {0} WHERE id=@pId", tabla, value);
            SqlCommand cmd;

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        cmd.Parameters.Add(new SqlParameter("@pId", value));

                        object data = cmd.ExecuteScalar();
                        if (data != null)
                        {
                            if (Convert.ToInt32(data) > 0)
                                existe = true;
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

            return existe;
        }

        //VALIDAR TIPO DE CONTRATO
        /*
         * 0 - INDEFINIDO
         * 1 - PLAZO FIJO
         * 2 - OBRA O FAENA
         */
        private bool ExisteTipoContrato(int value)
        {
            bool existe = false;

            switch (value)
            {
                case 0:
                    existe = true;
                    break;
                case 1:
                    existe = true;
                    break;
                case 2:
                    existe = true;
                    break;

                default:
                    existe = false;
                    break;
            }

            return existe;
        }

        //REGIMEN SALARIO
        private bool ExisteRegimenSalario(string value)
        {
            bool existe = false;
            if (value.Length == 0)
                return false;

            if (value == "variable" || value == "fijo")
                existe = true;
            else
                existe = false;

            return existe;
        }

        //VALIDAR CLASE
        private bool ExisteClase(int clase)
        {           

            bool existe = false;
            string sql = "SELECT count(*) FROM clase WHERE codClase=@pId";
            SqlCommand cmd;
          
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROD
                        cmd.Parameters.Add(new SqlParameter("@pId", clase));

                        object data = cmd.ExecuteScalar();
                        if (data != null)
                        {
                            if (Convert.ToInt32(data) > 0)
                                existe = true;
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

            return existe;
        }

        //VALIDAR JUBILADO
        /*
         * 0- NO
         * 1- SI, NO COTIZA
         * 2- SI, COTIZA
         * 
         */
        private bool ExisteJubilado(int value)
        {
            bool existe = false;

            switch (value)
            {
                case 0:
                    existe = true;
                    break;
                case 1:
                    existe = true;
                    break;
                case 2:
                    existe = true;
                    break;

                default:
                    existe = false;
                    break;
            }

            return existe;           

        }

        //EXISTE CAUSAL TERMINO
        private bool ExisteCausalTermino(int value)
        {            
            bool existe = false;
            string sql = "SELECT count(*) FROM causaltermino WHERE codcausal=@pId";
            SqlCommand cmd;
            
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pId", value));

                        object data = cmd.ExecuteScalar();

                        if (data != null)
                        {
                            if (Convert.ToInt32(data) > 0)
                                existe = true;
                        }                   

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                    }
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
               
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return existe;
        }

        //JORNADA LABORAL
        /*
         * 1- Lunes - Viernes
         * 2- Lunes - Sábado
         * 3- Turnos
         */
        private bool ExisteJornada(int pData)
        {
            bool existe = false;

            switch (pData)
            {
                case 1:
                    existe = true;
                    break;
                case 2:
                    existe = true;
                    break;
                case 3:
                    existe = true;
                    break;
                default:
                    existe = false;
                    break;
            }

            return existe;
        }

        //OBTENER ID DE ACUERDO A CADENA
        private int GetIdFromData(string value, string table, SqlTransaction tr)
        {
            string sql = string.Format("SELECT id FROM {0} WHERE nombre='{1}'", table, value);
           
            int valor = 0;
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {               
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        cmd.Transaction = tr;
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                valor = (int)rd["id"];
                            }
                        }
                    }
                    cmd.Dispose();
                    rd.Close();              
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return valor;
        }

        //id DE CIUDAD
        private int GetIdFromCiudad(string value, SqlTransaction tr)
        {
            int id = 0;
            string sql = "SELECT idciudad FROM CIUDAD WHERE descCiudad=@nombre";
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@nombre", value));
                        cmd.Transaction = tr;
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                id = (int)rd["idciudad"];
                            }
                        }
                        cmd.Dispose();
                        rd.Close();
                    }

            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return id;
        }

        //ID CAUSAL
        private int GetIdFromCausal(string value, SqlTransaction tr)
        {
            int id = 0;
            string sql = "SELECT codCausal FROM causaltermino WHERE descCausal=@pValue";
            SqlCommand cmd;
            try
            {
                using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                {
                    //PARAMETROS
                    cmd.Parameters.Add(new SqlParameter("@pValue", value));
                    cmd.Transaction = tr;
                    id = Convert.ToInt32(cmd.ExecuteScalar());

                    cmd.Dispose();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return id;
        }

        //VALIDAR CENTRO COSTO
        private bool ExisteCentroCosto(int value)
        {
            bool existe = false;
            string sql = "SELECT id FROM ccosto WHERE id=@number";
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETRO
                        cmd.Parameters.Add(new SqlParameter("@number", value));
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

        //OBTENER ID PARA TIPO CONTRATO
        private int GetIdTipoContrato(string value)
        {
            int id = 0;
            if (value.ToLower() == "indefinido")
                id = 0;
            if (value.ToLower() == "plazo fijo")
                id = 1;
            if (value.ToLower() == "obra o faena")
                id = 2;

            return id;
        }

        //ID PARA JUBILADO
        private int GetIdJubilado(string value)
        {
            int id = 0;
            if (value == "no")
                id = 0;
            if (value == "si, no cotiza")
                id = 1;
            if (value == "si, cotiza")
                id = 2;

            return id;
        }

        //CAJA PREVISION ID
        private int GetIdCajaPrevision(string value, SqlTransaction tran)
        {
            int id = 0;
            if (value == "no")
                return 0;

            string sql = "SELECT id FROM CAJAPREVISION WHERE nombre=@nombre";
            SqlCommand cmd;
            SqlDataReader rd;

            try
            {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@nombre", value));
                        cmd.Transaction = tran;
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                id = (int)rd["id"];
                            }
                        }
                        cmd.Dispose();
                        rd.Close();
                    }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return id;
        }

        //SEXO
        private int GetIdSexo(string value)
        {
            int id = 0;
            if (value.ToLower() == "m")
                id = 0;
            if (value.ToLower() == "f")
                id = 1;

            return id;
        }

        //OBTENER CODIGO SUCURSAL DESDE SU DESCRIPCION
        private bool ExisteSucursal(int pDesc)
        {
            bool existe = false;
            string sql = "SELECT count(*) FROM sucursal WHERE codSucursal=@pId";
            SqlCommand cmd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pId", pDesc));

                        object data = cmd.ExecuteScalar();
                        if (data != DBNull.Value)
                        {
                            if (Convert.ToInt32(data) > 0)
                                existe = true;
                        }

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

        //REGIMEN SALARIO ID
        private int GetIdRegimenSalarial(string value)
        {
            int id = 0;
            if (value == "variable")
                id = 0;
            if (value == "fijo")
                id = 1;

            return id;
        }

        //CODIGO DE CLASE
        private int GetIdFromClase(string value, SqlTransaction tr)
        {
            int id = 0;
            string sql = "SELECT codclase FROM clase WHERE descClase=@nombre ";
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@nombre", value));
                        cmd.Transaction = tr;
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                id = (int)rd["codclase"];
                            }
                        }
                    }
                    cmd.Dispose();
                    rd.Close();
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return id;
        }

        //CODIGO SUCURSAL
        private int GetIdFromSucursal(string value, SqlTransaction tr)
        {
            int id = 0;
            string sql = "SELECT codSucursal FROM SUCURSAL WHERE descSucursal=@nombre ";
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                {
                    //PARAMETROS
                    cmd.Parameters.Add(new SqlParameter("@nombre", value));
                    cmd.Transaction = tr;
                    rd = cmd.ExecuteReader();
                    if (rd.HasRows)
                    {
                        while (rd.Read())
                        {
                            id = (int)rd["codSucursal"];
                        }
                    }
                }
                cmd.Dispose();
                rd.Close();
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return id;
        }

        //VALIDAR FECHA DE NACIMIENTO
        private bool NacimientoValido(DateTime fecha)
        {
            DateTime today = DateTime.Now.Date;
           
            int edad = 0;
            edad = today.Year - fecha.Year;

            if (edad < 15)
                return false;
            if (edad > 90)
                return false;

            return true;            
        }

        //VALIDAR FECHAS DE CONTRATO
        private bool FechasContratoValidas(DateTime inicio, DateTime termino)
        {
            if (inicio > termino)
                return false;
            //if (inicio == termino)
              //  return false;

            return true;

        }

        //VERIFICAR SI EXISTE EL PERIODO
        private bool ExistePeriodo(int periodo)
        {
            string sql = "SELECT anomes FROM parametro WHERE anomes=@periodo";
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
                        cmd.Parameters.Add(new SqlParameter("@periodo", periodo));
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            existe = true;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return existe;
        }

        //SABER SI EL PERIODO INGRESADO ES MENOR AL PERIODO ACTUAL Y ESTE TIENE REGISTROS NO SE PUEDEN INGRESAR
        private bool PeriodoNoEditable(int periodo)
        {
            string sql = "select anomes from trabajador " +
                "where anomes < (select max(anomes) from parametro) AND anomes=@periodo";
            SqlCommand cmd;
            SqlDataReader rd;
            bool editable = true;
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
                            //REGISTRA DATOS (ASUMIMOS QUE YA SE CERRÓ)
                            editable = false;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return editable;
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

        //LECTURA DE ARCHIVO EXCEL USANDO API DE DEVEXPRESS
        private DataTable ReadFileDev(string FileName)
        {
            DataTable tabla = new DataTable();

            //CREAMOS LIBRO
            Workbook Libro = new Workbook();

            //string FileName = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Empleados.xlsx";

            if (File.Exists(FileName))
            {
                //SI EXISTE LO INTENTAMOS CARGAR EN LIBRO...
                //CARGAMOS EL DOCUMENTO EN LIBRO
                try
                {
                    Libro.LoadDocument(FileName, DocumentFormat.Xlsx);

                    //OBTENEMOS LA HOJA DESDE ARCHIVO
                    Worksheet Hoja = Libro.Worksheets[0];

                    TableCollection tablas = Hoja.Tables;
                    if (tablas == null || tablas.Count == 0)
                    { XtraMessageBox.Show("Por favor verifica que el documento tenga el formato correcto", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Error); return null; }

                    //OBTENEMOS EL RANGO USADO DE LA HOJA
                    Range rango = Hoja.Tables[0].Range;

                    //LLAMAMOS AL METODO CREATE DATA TABLE
                    //SI EL SEGUNDO PARAMETRO ES TRUE DECIMOS QUE USAREMOS LA PRIMERA FILA COMO LAS COLUMNAS DEL DATATABLE
                    tabla = Hoja.CreateDataTable(rango, true);

                    DataTableExporter exporter = Hoja.CreateDataTableExporter(rango, tabla, true);
                    //exporter.CellValueConversionError += exporter_CellValueConversionError;
                    ExcelConverter convert = new ExcelConverter();
                    convert.EmptyCellValue = "N/A";
                    exporter.Options.ConvertEmptyCells = true;

                    exporter.Options.DefaultCellValueToColumnTypeConverter.SkipErrorValues = false;
                    exporter.Export();

                    //QUITAMOS ESPACIOS EN LOS NOMBRES DE LAS COLUMNAS
                    foreach (DataColumn columna in tabla.Columns)
                    {
                        columna.ColumnName = NewCadena(columna.ColumnName);                   
                    }
                   
                    return tabla;
                }
                catch (Exception ex)
                {                    
                    XtraMessageBox.Show("Por favor verifica la informacion y vuelve a intentarlo", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
            }
            else
            {
                XtraMessageBox.Show("Archivo no existe", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;
            }

        }

        //VALIDAR DATATABLE 
        private bool ValidaDataTable(DataTable tabla)
        {
            string data = "";
            string val = "";
            if (tabla.Rows.Count > 0)
            {
                //OBTENER LAS COLUMNAS
                DataColumnCollection Columnas = tabla.Columns;
                try
                {
                    //COMPARAMOS LAS COLUMNAS PARA VER SI SON VALIDAS
                    foreach (DataColumn columna in Columnas)
                    {
                        //SI COLUMNA VALIDA RETORNA FALSE, NO ES VALIDA
                        if (ColumnaValida((columna.ToString()).ToLower()) == false)
                        {
                            XtraMessageBox.Show($"Columna {columna} no valida", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                    }

                    //RECORREMOS ELMENTOS DE DATATABLE
                    for (int i = 0; i < tabla.Rows.Count; i++)
                    {
                        for (int j = 0; j < tabla.Columns.Count; j++)
                        {
                            if (tabla.Rows[i][j].ToString() != "" || tabla.Rows[i][j] != null)
                            {                           
                                //VALIDAMOS CAMPO
                                data = tabla.Rows[i][j].ToString();
                                if (ValidaData(data, tabla.Columns[j].ToString().ToLower()) == false)
                                {
                                    return false;
                                }
                            }
                        }
                    }

                    //SI RECORRIO TODO EL DATA SET Y NO REGISTRO ERRORES RETORNAMOS TRUE
                    return true;
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show(ex.Message);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        #endregion

        private void FrmAgregarEmpleados_Load(object sender, EventArgs e)
        {
            var sm = GetSystemMenu(Handle, false);
            EnableMenuItem(sm, SC_CLOSED, MF_BYCOMMAND | MF_DISABLED);

            btnCargarTrabajadores.Enabled = false;
        }

        private void btnCargarTrabajadores_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (txtFile.Text == "")
            { XtraMessageBox.Show("Por favor carga un archivo", "Informacion",MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

            bool transaccionCorrecta = false;
            if (TablaEmpleados.Rows.Count > 0)
            {
                DialogResult advertencia = XtraMessageBox.Show("¿Realmente deseas ingresar " + TablaEmpleados.Rows.Count + " trabajadores al periodo "+ fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(Calculo.PeriodoObservado)) + "?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (advertencia == DialogResult.Yes)
                {
                    //INSERTAMOS DATA
                    lblMensaje.Visible = true;
                    lblMensaje.Text = "Procesando...";
                    transaccionCorrecta = IngresoTrabajadores(TablaEmpleados);

                    if (transaccionCorrecta)
                    {
                        XtraMessageBox.Show("Carga de trabajadores realizada correctamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        //GUARDAR REGISTRO EN LOG
                        logRegistro log = new logRegistro(User.getUser(), "SE CARGAN TRABAJADORES DESDE ARCHIVO EXTERNO", "TRABAJADOR", "0", "0", "INGRESAR");
                        log.Log();
                        lblMensaje.Visible = false;

                        Close();
                    }
                    else
                    {
                        XtraMessageBox.Show("No se pudo realizar la carga de trabajadores", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        lblMensaje.Visible = false;
                        return;
                    }
                }
              
            }
        }

        private void btnManual_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            frmManualCargaArchivo manual = new frmManualCargaArchivo();
            manual.ShowDialog();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            Close();
        }

        private void txtFile_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void groupControl1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnPlantilla_Click(object sender, EventArgs e)
        {
            //MOSTRAR PLANTILLA EXCEL CON TODAS LAS COLUMNAS.
            string Default = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\PlantillaFichas.xlsx";

            try
            {
                DataTable pTabla = new DataTable();

                foreach (string col in ListadoPlantilla)
                {                     
                    pTabla.Columns.Add(new DataColumn() { ColumnName = col });
                }               

                FileExcel.CrearArchivoPlantilla(pTabla, Default);
             
            }
            catch (Exception)
            {
                XtraMessageBox.Show("Error al crear archivo", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);                
            }
        }
    }
}