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
using System.Data.SqlClient;
using DevExpress.Spreadsheet;
using System.Collections;
using DevExpress.XtraGrid;
using DevExpress.XtraReports.ReportGeneration;
using DevExpress.XtraReports.UI;

namespace Labour
{
    public partial class frmCargaMasivaItems : DevExpress.XtraEditors.XtraForm, ISeleccionMultiple
    {
        //PARA GUARDAR SQL DINAMICO
        Hashtable Query = new Hashtable();

        //PARA SABER LA CANTIDAD DE REGISTROS
        int registros = 0;

        /// <summary>
        /// Listado de items de trabajador.
        /// </summary>
        List<ItemTrabajador> ListadoItemsArchivos = new List<ItemTrabajador>();

        /// <summary>
        /// Liastado de contrato no repetidos.
        /// </summary>
        List<string> ListadoContratosNoRepetidos = new List<string>();
        List<string> ListadoItemsNoRpetidos = new List<string>();

        /// <summary>
        /// Listado para guardar los nuevos valores desde excel
        /// </summary>
        List<ItemTrabajador> ListadoNuevosValores = new List<ItemTrabajador>();

        public IMainChildInterface CambioEstadoOpen { get; set; }

        public string SqlQuery { get; set; } = "";

        #region "INTERFAZ SELECCION MULTIPLE"
        public void CargaListado(string pSql)
        {
            SqlQuery = pSql;
        }
        #endregion

        public frmCargaMasivaItems()
        {
            InitializeComponent();
        }

        private void btnImportar_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            OpenFileDialog abrir = new OpenFileDialog();
            abrir.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm|CSV file (*.csv)|*.csv";
            string rutaArchivo = "", extension = "";
            bool correcto = false;

            if (CambioEstadoOpen != null)
                CambioEstadoOpen.ChangeStatus("<color=Green>Leyendo...</color>");

            if (abrir.ShowDialog() == DialogResult.OK)
            {
                //PARA GUARDAR LOS DATOS
                DataTable Tabla = new DataTable();

                rutaArchivo = abrir.FileName;
                if (File.Exists(rutaArchivo) == false)
                {
                    XtraMessageBox.Show("Ruta archivo no existe", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    if (CambioEstadoOpen != null)
                        CambioEstadoOpen.ChangeStatus("");
                    return;
                }

                txtRuta.Text = abrir.FileName;
                lblResult.Visible = true;
                lblResult.Text = "Leyendo...";
                extension = Path.GetExtension(abrir.FileName);
                splashScreenManager1.ShowWaitForm();

                if (extension == ".csv")
                    //Tabla = FileExcel.ReadCsv(abrir.FileName);
                    correcto = LecturaCsv(abrir.FileName);
                else
                    // Tabla = FileExcel.ReadExcelDev(abrir.FileName);
                    correcto = LecturaExcel(abrir.FileName);

                if (correcto == false)
                {
                    XtraMessageBox.Show("Hay errores en el archivo", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); lblResult.Visible = false; if (splashScreenManager1.IsSplashFormVisible) splashScreenManager1.CloseWaitForm(); btnRefresh.Enabled = true; btnCargaInformacion.Enabled = false;
                    if (CambioEstadoOpen != null)
                        CambioEstadoOpen.ChangeStatus("");
                    return; }

        
                if (correcto)
                {
                    splashScreenManager1.CloseWaitForm();
                    lblResult.Text = "Ok.";
                    btnCargaInformacion.Enabled = true;
                    btnRefresh.Enabled = false;                    
                    //Query = GeneraSql(Tabla);

                    lblRegistros.Text = "Registros encontrados # " + registros;

                    ColumnasGrilla();
                }

               
            }

            if (CambioEstadoOpen != null)
                CambioEstadoOpen.ChangeStatus("");
        }

        #region "MANEJO DATOS"
        //VALIDAR SI CODIGO DE ITEM EXISTE
        private bool ExisteCodigo(string pItem)
        {
            bool existe = false;
            string sql = "SELECT count(*) FROM item WHERE coditem=@pItem";
            SqlCommand cmd;
            int c = 0;

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pItem", pItem));

                        c = Convert.ToInt32(cmd.ExecuteScalar());

                        if (c > 0)
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

        //VALIDAR SI CONTRATO EXISTE
        private bool ExisteContrato(string pContrato)
        {
            string sql = "SELECT count(*) FROM trabajador WHERE contrato=@pContrato AND anomes=@pPeriodo";
            SqlCommand cmd;
            bool existe = false;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pContrato", pContrato));
                        cmd.Parameters.Add(new SqlParameter("@pPeriodo", Calculo.PeriodoObservado));

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

        //VALIDAR DATA
        private bool ValidaData(DataTable Datos)
        {
            if (Datos.Rows.Count>0)
            {
                DataColumnCollection columnas = Datos.Columns;
                List<string> contratos = new List<string>();
                List<string> items = new List<string>();
                List<ResumenItemMasivo> resumen = new List<ResumenItemMasivo>();            

                //RECORREMOS COLUMNAS
                if (columnas[0].ColumnName.ToLower() != "contrato")
                {
                   // XtraMessageBox.Show(columnas[0].ColumnName);
                    XtraMessageBox.Show($"Columna {columnas[0].ColumnName} no valida", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }

                //RECORREMOS DESDE LA SEGUNDA COLUMNA (SUPONEMOS QUE LA PRIMERA COLUMNA ES CONTRATO)
                for (int i = 1; i < columnas.Count; i++)
                {
                    if (ExisteCodigo(columnas[i].ColumnName) == false)
                    {
                        XtraMessageBox.Show($"Item {columnas[i].ColumnName} no existe", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return false;
                    }
                }

                //SOLO PARA VER SI HAY UN ITEM REPETIDO COMO COLUMNA
                foreach (DataColumn Columna in Datos.Columns)
                {
                    items.Add(Columna.ColumnName);
                    if(Columna.ColumnName.ToLower() != "contrato")
                        resumen.Add(new ResumenItemMasivo() { key = ResumenItemMasivo.getNameItem(Columna.ColumnName), value = 0});
                }

                //SI LLEGAMOS A ESTE PUNTO ES PORQUE LAS COLUMNAS SON VALIDAS...

                //DEBEMOS VALIDAR QUE LOS DATOS DE LA FILA SON CORRECTOS
                foreach (DataRow Fila in Datos.Rows)
                {
                    foreach (DataColumn Columna in Datos.Columns)
                    {
                        if (Columna.ColumnName == "contrato")
                        {
                            if (Fila[Columna].ToString() == "")
                            {
                                XtraMessageBox.Show($"Por favor verifica el valor para columna contrato", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return false;
                            }

                            if (ExisteContrato(Fila[Columna].ToString()) == false)
                            { XtraMessageBox.Show($"No existe contrato {Fila[Columna]}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return false; }

                            contratos.Add(Fila[Columna].ToString());
                        }
                        else
                        {
                            //SUPONES QUE EL RESTO SON ITEMS validos
                            if (Fila[Columna].ToString() != "")
                            {
                                if (fnSistema.IsNumeric(Fila[Columna].ToString()) == false)
                                {
                                    XtraMessageBox.Show($"Por favor verifica el valor ingresado para la columna {Columna.ColumnName}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                    return false;
                                }
                            }                           

                            //SI ES BLANCO LO CAMBIAMOS A CERO
                           /* if (Fila[Columna].ToString() == "")
                            {
                                Fila[Columna] = 0;
                            }*/
                            if (Columna.ColumnName.ToLower() != "contrato" && Fila[Columna].ToString() != "")
                                ResumenItemMasivo.Actualizar(resumen, Convert.ToDouble(Fila[Columna]), ResumenItemMasivo.getNameItem(Columna.ColumnName));
                        }                       

                    }
                }//END FOREACH  

                if (ElementoRepetido(contratos) == true)
                { XtraMessageBox.Show("Verificar que el n° de contrato no esté repetido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }
                if (ElementoRepetido(items) == true)
                { XtraMessageBox.Show("Verificar que no hayan items repetidos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                //LLEGADO ESTE PUNTO PODEMOS DECIR QUE LA DATA INGRESADA ES VALIDA
                //cargamos grilla
                if (resumen.Count > 0)
                {                    
                    gridResumen.DataSource = resumen;                   
                    fnSistema.spOpcionesGrilla(viewResumen);                   
                }                    

                return true;
            }

            return false;
        }

        //OBTENER EL RUT EN BASE AL N° de contrato
        private string GetRut(string pContrato)
        {
            string sql = "SELECT rut FROM trabajador WHERE contrato = @pContrato AND anomes=@pPeriodo";
            string rut = "";
            SqlCommand cmd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pContrato", pContrato));
                        cmd.Parameters.Add(new SqlParameter("@pPeriodo", Calculo.PeriodoObservado));

                        rut = (string)cmd.ExecuteScalar();

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return rut;
        }
        //OBTENER FORMULA, TIPO, ORDEN DESDE TABLA ITEM 
        private item GetInfoItem(string pCoditem)
        {
            item obj = new item();
            string sql = "SELECT formula, tipo, orden FROM item WHERE coditem=@pCoditem";
            SqlDataReader rd;
            SqlCommand cmd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pCoditem", pCoditem));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //GUARDAMOS DATOS EN OBJETO
                                obj.formula = (string)rd["formula"];
                                obj.tipo = Convert.ToInt32(rd["tipo"]);
                                obj.orden = Convert.ToInt32(rd["orden"]);
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

            return obj;
        }

        //PREGUNTR SI EL ITEM YA EXISTE
        private bool ExisteItem(string pCoditem, string pContrato)
        {
            bool existe = false;
            string sql = "SELECT count(*) FROM itemtrabajador WHERE coditem=@pCoditem AND contrato=@pContrato " +
                "AND anomes=@pPeriodo";
            SqlCommand cmd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pCoditem", pCoditem));
                        cmd.Parameters.Add(new SqlParameter("@pContrato", pContrato));
                        cmd.Parameters.Add(new SqlParameter("@pPeriodo", Calculo.PeriodoObservado));

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

        //VERIFICAR SI EL ELEMENTO DE UNA LISTA ESTA MAS DE UNA VEZ
        private bool ElementoRepetido(List<string> listado)
        {
            bool repetido = false;
            int count = 0;
            if (listado.Count > 0)
            {
                foreach (string elemento in listado)
                {
                    count = 0;
                    foreach (string x in listado)
                    {
                        if (x == elemento)
                            count++;
                    }

                    if (count > 1)
                        return true;
                }
            }

            return repetido;
        }

        //OBTENER LISTADO DE ITEMS
        private List<string> ListadoItems()
        {
            List<string> items = new List<string>();
            string sql = "SELECT coditem FROM item";
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
                                items.Add((string)rd["coditem"]);
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
            return items;
        }

        //VER SI UN ELEMENTO EXISTE EN UNA LISTA
        private bool ExisteLista(List<string> listado, string pCod)
        {
            int count = 0;
            if (listado.Count>0)
            {
                foreach (string item in listado)
                {
                    if (item.ToLower() == pCod.ToLower())
                        count++;
                }

                if (count > 0)
                    return true;
            }
            return false;
        }

        //GENERAR SQL INSERT PARA LOS ITEMS
        private string GeneraSql(DataTable Datos)
        {
            string query = "";

            string Rut = "";
            item obj = new item();
            bool existe = false;
            int lastItem = 0;

            if (Datos.Rows.Count>0)
            {
                registros = 0;
                foreach (DataRow Fila in Datos.Rows)
                {
                    foreach (DataColumn Columna in Datos.Columns)
                    {
                        if (Columna.ColumnName.ToLower() == "contrato")
                        {
                            Rut = GetRut(Fila[Columna].ToString());
                            //OBTENEMOS EL N° DEL ULTIMO ITEM INGRESADO
                            lastItem = LastNumberItem(Fila[Columna].ToString());
                        }                         
                        else
                        {
                            obj = GetInfoItem(Columna.ColumnName);

                            //PREGUNTAMOS SI EXISTE ITEM PARA CONTRATO
                            existe = ExisteItem(Columna.ColumnName, Fila["contrato"].ToString());
                            if (existe)
                            {
                                if(Fila[Columna].ToString() != "")
                                query = query + $"UPDATE ITEMTRABAJADOR SET valor={Convert.ToDouble(Fila[Columna.ColumnName])} WHERE contrato='{Fila["contrato"]}' AND " +
                                        $"anomes={Calculo.PeriodoObservado} AND coditem='{Columna.ColumnName}'\n";
                            }
                            else {

                                if(Fila[Columna].ToString() != "")
                                query = query + "INSERT INTO ITEMTRABAJADOR(contrato, rut, anomes, coditem, formula, tipo, orden, numitem, valor, valorcalculado, esclase) " +
                                $"VALUES('{Fila["contrato"]}', '{Rut}', {Calculo.PeriodoObservado}, " +
                                $"'{Columna.ColumnName.ToUpper()}', {obj.formula}, {obj.tipo}, {obj.orden}, {lastItem}," +
                                $"{Convert.ToDouble(Fila[Columna.ColumnName])}, {Convert.ToDouble(Fila[Columna.ColumnName])}, 0)\n";

                                lastItem++;
                            }                           
                            

                            //GENERAMOS SQL INSERT POR CADA COLUMNA
                            /*query = query + "INSERT INTO #data(contrato, rut, anomes, coditem, formula, tipo, orden, numitem, valor, valorcalculado) " +
                                $"VALUES('{Fila["contrato"]}', '{Rut}', {Calculo.PeriodoObservado}, " +
                                $"'{Columna.ColumnName.ToUpper()}', {obj.formula}, {obj.tipo}, {obj.orden}, 1," +
                                $"{Convert.ToDouble(Fila[Columna.ColumnName])}, {Convert.ToDouble(Fila[Columna.ColumnName])})\n";*/

                            registros++;
                        }                        
                    }                   
                }            
            }

            

            return query;
        }

        //METODO QUE REALIZA EL INSERT EN BD
        private void IngresarDatos(Hashtable pQuery, int pOption)
        {
            #region "OldQuery"
            string sql = "BEGIN TRY " +
                    "BEGIN TRANSACTION " +
                        "DECLARE @maximo INT " +
                        "CREATE TABLE #data( " +
                            "contrato varchar(15), " +
                            "rut varchar(9), " +
                            "anomes int, " +
                            "coditem varchar(7), " +
                            "formula varchar(7), " +
                            "tipo int, " +
                            "orden int, " +
                            "numitem int, " +
                            "esclase bit DEFAULT 0, " +
                            "valor decimal(12, 4), " +
                            "valorcalculado decimal(12, 4), " +
                            "proporcional bit DEFAULT 0, " +
                            "permanente bit DEFAULT 0,  " +
                            "contope bit DEFAULT 0, " +
                            "cuota varchar(7) DEFAULT 0 " +
                        ") " +
                        //AQUI VA NUESTRO SQL DINAMICO...
                        Query +
                        " SET @maximo = (ISNULL((SELECT MAX(numitem) FROM itemtrabajador WHERE anomes = @pPeriodo), 0)) " +
                            "UPDATE #data " +
                            "SET @maximo = numitem = @maximo + 1 " +
                        "INSERT INTO itemtrabajador(contrato, rut, anomes, coditem, formula, tipo, orden, numitem, valor, valorcalculado, esclase, proporcional, permanente, contope, cuota) " +
                        "SELECT contrato, rut, anomes, coditem, formula, tipo, orden, numitem, valor, valorcalculado, esclase, proporcional, permanente, contope, cuota FROM #data " +
                     "COMMIT TRANSACTION " +
                "END TRY " +
                    "BEGIN CATCH " +
                "IF @@TRANCOUNT > 0 " +
                     "ROLLBACK " +
                "END CATCH";
            #endregion

            if (CambioEstadoOpen != null)
                CambioEstadoOpen.ChangeStatus("<color=Green>Procesando...</color>");

            if (pQuery.Count == 0)
            { XtraMessageBox.Show("No se pudo realizar operación", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            string SqlOption = "";
            //SE DESEA REALIZAR UN INSERT
            if (pOption == 1)
                SqlOption = pQuery["insert"].ToString();
            else if(pOption == 2)
            //SE DESEA REALIZAR UN UPDATE
                SqlOption = pQuery["update"].ToString();

            string sql2 = "BEGIN TRY " +
                        "BEGIN TRANSACTION " +                           
                            //AQUI VA NUESTRO SQL DINAMICO...                            
                            SqlOption +                
                         "COMMIT TRANSACTION " +
                        "END TRY " +
                            "BEGIN CATCH " +
                        "IF @@TRANCOUNT > 0 " +
                             "ROLLBACK " +
                        "END CATCH";            

            SqlCommand cmd;
            int count = 0;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql2, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        //cmd.Parameters.Add(new SqlParameter("@pPeriodo", Calculo.PeriodoObservado));

                        count = Convert.ToInt32(cmd.ExecuteNonQuery());
                        if (count > 0)
                        {
                            XtraMessageBox.Show("items ingresados correctamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Close();
                        }
                        else
                        {
                            XtraMessageBox.Show("No se pudo guardar la informacion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

            if (CambioEstadoOpen != null)
                CambioEstadoOpen.ChangeStatus("");
        }

        //COLUMNAS GRILLA
        private void ColumnasGrilla()
        {
            if (viewResumen.RowCount > 0)
            {
                viewResumen.Columns[0].Caption = "Item";        
                viewResumen.Columns[1].Caption = "Total";
                viewResumen.Columns[1].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                viewResumen.Columns[1].DisplayFormat.FormatString = "n1";

                //COLUMNAS SUMA
                viewResumen.Columns[1].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                viewResumen.Columns[1].SummaryItem.DisplayFormat = "Total: {0:n1}";
                
                viewResumen.OptionsView.ShowFooter = true;
            }
        }

        //EL ULTIMO NUMERO DE ITEM INGRESADO PARA UN CONTRATO EN PARTICULAR
        private int LastNumberItem(string pContrato)
        {
            int number = 0;
            string sql = "SELECT ISNULL(MAX(numitem), 0) FROM itemtrabajador WHERE contrato=@pContrato " +
                "AND anomes=@pPeriodo";
            SqlCommand cmd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pContrato", pContrato));
                        cmd.Parameters.Add(new SqlParameter("@pPeriodo", Calculo.PeriodoObservado));

                        number = Convert.ToInt32(cmd.ExecuteScalar());

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return (number + 1);
        }

        //LECTURA DE EXCEL (PARA ITEMS REPETIDOS)
        private bool LecturaExcel(string PathFile)
        {
            //CREAMOS LIBRO
            Workbook Libro = new Workbook();

            ListadoItemsArchivos.Clear();

            string name = "";

            //PREGUNTAMOS SI EL ARCHIVO EXISTE
            if (File.Exists(PathFile))
            {
                //CARGAMOS DOCUMENTO
                try
                {
                    if (PathFile.ToLower().Contains("xlsx"))
                        Libro.LoadDocument(PathFile, DocumentFormat.Xlsx);
                    else if (PathFile.ToLower().Contains("xls"))
                        Libro.LoadDocument(PathFile, DocumentFormat.Xls);

                    //PARA LISTADO DE ITEMS
                    List<string> items = new List<string>();
                    //PARA LISTADO DE CONTRATOS DEL PERIODO
                    List<string> contratos = new List<string>();
                    List<ResumenItemMasivo> resumen = new List<ResumenItemMasivo>();

                    //OBTENEMOS LA HOJA DESDE ARCHIVO
                    Worksheet Hoja = Libro.Worksheets[0];                    
                    Range rango = Hoja.GetUsedRange();                    
                    

                    int Filas = rango.RowCount;                  
                    int Columnas = rango.ColumnCount;
                 
                    string NombreColumna = "", value = "";

                    if (Filas == 0)
                    { XtraMessageBox.Show("No se encontró informacion en el archivo", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); if (splashScreenManager1.IsSplashFormVisible) splashScreenManager1.CloseWaitForm();  return false; }

                    //LISTADO ITEMS
                    items = ListadoItems();                    

                    if (items.Count == 0)
                    { XtraMessageBox.Show("No se encontró informacion", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); if (splashScreenManager1.IsSplashFormVisible) splashScreenManager1.CloseWaitForm(); return false; }                    

                    //RECORREMOS COLUMNAS (DESDE LA POS 1)
                    for (int col  = 0; col < Columnas; col ++)
                    {                        
                        if (rango[0, 0].Value.ToString().ToLower().Trim() != "contrato")
                        { XtraMessageBox.Show("Verifica que la primera columna sea contrato", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return false; }

                        NombreColumna = rango[0, col].Value.ToString().Trim();

                        if (NombreColumna.Length == 0)
                        { XtraMessageBox.Show($"Por favor verifica que los nombres de la columnas no estén vacíos columna {col+1}\nNumero de columnas encontradas:{rango.ColumnCount}\nSi hay columnas con espacios en blanco y no desea agregarlas, elimínelas.", "Columna vacía", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                        if (ExisteCodigo(NombreColumna) == false && col > 0)
                        { XtraMessageBox.Show($"No existe item {NombreColumna}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                        if(col > 0)   
                            if(ResumenItemMasivo.ExisteItem(resumen, ResumenItemMasivo.getNameItem(rango[0, col].Value.ToString().Trim())) == false)
                                resumen.Add(new ResumenItemMasivo() { key = ResumenItemMasivo.getNameItem(rango[0, col].Value.ToString().Trim()), value = 0 });
                    }

                    //VALIDAMOS TODAS LAS CELDAS
                    for (int row = 1; row < Filas; row++)
                    {                        
                        for (int col = 0; col < Columnas; col++)
                        {                         
                            if (!rango[row, col].Value.IsEmpty)
                            {
                                value = rango[row, col].Value.ToString().Trim();
                            }
                            else
                            {
                                value = "0";
                            }

                            name = rango[0, col].Value.ToString();

                            if (col == 0)
                            {                              
                                if (value == "")
                                {
                                    XtraMessageBox.Show($"Por favor verifica el valor para columna contrato celda [{row},{col}]", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return false;
                                }

                                if (ExisteContrato(value) == false)
                                { XtraMessageBox.Show($"No existe contrato {value} en celda [{row},{col}]", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return false; }

                                //GUARDAMOS CONTRATOS
                                contratos.Add(value);
                            }
                            else
                            {
                                //SUPONES QUE EL RESTO SON ITEMS validos
                                if (value != "")
                                {
                                    if (fnSistema.IsDecimal(value) == false)
                                    {
                                        XtraMessageBox.Show($"Por favor verifica el valor ingresado para la columna [{row},{col}] \n Valor:{value}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                        return false;
                                    }
                                }
                             
                                if (value != "")
                                {                                   
                                    ResumenItemMasivo.Actualizar(resumen, Convert.ToDouble(value), ResumenItemMasivo.getNameItem(rango[0, col].Value.ToString().Trim()));
                                }
                                    
                            }
                        }
                    }

                    //VERIFICAMOS QUE NO HAYAN CONTRATOS REPETIDOS
                    if (ElementoRepetido(contratos) == true)
                    { XtraMessageBox.Show("Verificar que el n° de contrato no esté repetido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); if (splashScreenManager1.IsSplashFormVisible) splashScreenManager1.CloseWaitForm(); return false; }
                    // if (ElementoRepetido(items) == true)
                    // { XtraMessageBox.Show("Verificar que no hayan items repetidos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return ; }
                
                    if (resumen.Count > 0)
                    {
                        gridResumen.DataSource = resumen;
                        fnSistema.spOpcionesGrilla(viewResumen);

                         Query = GetSqlFromFile(rango);
                     
                    }

                    return true;                   
                }
                catch (Exception ex)
                {
                    if (splashScreenManager1.IsSplashFormVisible) splashScreenManager1.CloseWaitForm();
                    MessageBox.Show(ex.Message);    
                    
                }
            }

            return false;
        }

        //LECTURA CSV (PARA ITEMS REPETIDOS)
        private bool LecturaCsv(string PathFile)
        {
            Query.Clear();

            bool correcto = false, existe = false;
            string Linea = "", cellValue = "", contrato = "", Rut = "", query = "", queryUpdate = "";
            string[] data;
            int count = 0, lastItem = 0;
            List<string> contratos = new List<string>();
            List<string> items = new List<string>();
            List<string> columnas = new List<string>();
            item obj = new item();
            List<ResumenItemMasivo> resumen = new List<ResumenItemMasivo>();

            ItemTrabajador dataFila = new ItemTrabajador();
            ItemTrabajador NuevoVal = new ItemTrabajador();
            ListadoContratosNoRepetidos.Clear();
            ListadoItemsNoRpetidos.Clear();
            ListadoNuevosValores.Clear();
            ListadoItemsArchivos.Clear();

            if (File.Exists(PathFile) == false) return false;

            StreamReader Lector = new StreamReader(PathFile);
            try
            {
                while (!Lector.EndOfStream)
                {
                    //LEEMOS CADA LINEA
                    Linea = Lector.ReadLine();

                    //SEPARAMOS LINEA POR ;
                    if (Linea != "")
                    {
                        if (Linea.Contains(";") == false) return false;

                        //PRIMERA LINEA SON COLUMNAS   
                        data = Linea.Split(';');                    

                        //SOLO SI ES LA PRIMERA FILA (VALIDAMOS HEADERS)
                        if (count == 0)
                        {
                            //RECORREMOS DATA
                            for (int i = 0; i < data.Length; i++)
                            {                                
                                if (data[i] == "")
                                {                                  
                                    XtraMessageBox.Show("Nombre de columna no puede estar vacío", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); if (splashScreenManager1.IsSplashFormVisible) splashScreenManager1.CloseWaitForm(); return false; }

                                //CONTRATO
                                if (i == 0)
                                {
                                    if (data[i].ToLower().Trim() != "contrato")
                                    {
                                        XtraMessageBox.Show("Verifica que la primera columna sea contrato", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                        if (splashScreenManager1.IsSplashFormVisible)
                                            splashScreenManager1.CloseWaitForm();

                                        return false;
                                    }
                                    
                                }
                                else
                                {

                                    //VERIFICAR SI EXISTE EL ITEM...
                                    if (ExisteCodigo(data[i].Trim()) == false)
                                    { XtraMessageBox.Show($"No existe item {data[i].ToString()}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); if (splashScreenManager1.IsSplashFormVisible) splashScreenManager1.CloseWaitForm(); return false; }

                                    //GUARDAMOS NOMBRE DE COLUMNAS
                                    columnas.Add(data[i].Trim());

                                    //GUARDAMOS ITEMS EXCEPTUANDO LOS REPETIDOS
                                    if (i > 0)
                                        if (ResumenItemMasivo.ExisteItem(resumen, ResumenItemMasivo.getNameItem(data[i].Trim())) == false)
                                            resumen.Add(new ResumenItemMasivo() { key = ResumenItemMasivo.getNameItem(data[i].Trim()), value = 0 });
                                }
                            }
                        }
                        else
                        {
                           
                            //SEGUNDA LINEA -->
                            for (int i = 0; i < data.Length; i++)
                            {
                                dataFila = new ItemTrabajador();
                                NuevoVal = new ItemTrabajador();

                                cellValue = data[i].Trim();
                              
                                //VERIFICAR SI CONTRATO EXISTE
                                if (i == 0)
                                {
                                    if (cellValue == "")
                                    { XtraMessageBox.Show("Contrato no puede estar vacío", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                                    if (ExisteContrato(cellValue) == false)
                                    { XtraMessageBox.Show($"contrato {cellValue} no existe", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                                    //GUARDAMOS CONTRATOS EN LISTA...
                                    contratos.Add(cellValue);
                                    contrato = cellValue;
                                    lastItem = LastNumberItem(contrato);
                                    //OBTENER RUT DESDE CONTRATO
                                    Rut = GetRut(contrato);

                                    //GUARDAMOS CONTRATOS EN LISTADO DE CONTRATOS NO REPETIDA
                                    ListadoContratosNoRepetidos.Add(contrato);
                                }
                                else
                                {
                                    //VALIDAMOS ITEMS
                                    if (cellValue != "")
                                    {
                                        if (fnSistema.IsDecimal(cellValue) == false)
                                        { XtraMessageBox.Show($"Por favor verifica valor {cellValue}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);if (splashScreenManager1.IsSplashFormVisible) splashScreenManager1.CloseWaitForm(); return false; }

                                        dataFila.contrato = contrato;
                                        dataFila.Rut = Rut;

                                        NuevoVal.contrato = contrato;
                                        NuevoVal.Rut = Rut;

                                        //GENERAR SQL..
                                        obj = GetInfoItem(columnas[i-1]);                                        

                                        if (cellValue.Contains(","))
                                            cellValue = cellValue.Replace(",", ".");

                                        //GUARDAR SUMATORIAS
                                        ResumenItemMasivo.Actualizar(resumen, Convert.ToDouble(cellValue), ResumenItemMasivo.getNameItem(columnas[i-1]));

                                        queryUpdate = queryUpdate + $"UPDATE ITEMTRABAJADOR SET valor={cellValue} WHERE contrato='{contrato}' AND " +
                                                        $"anomes={Calculo.PeriodoObservado} AND coditem='{columnas[i - 1]}'\n";

                                        query = query + "INSERT INTO ITEMTRABAJADOR(contrato, rut, anomes, coditem, formula, tipo, orden, numitem, valor, valorcalculado, esclase) " +
                                              $"VALUES('{contrato}', '{Rut}', {Calculo.PeriodoObservado}, " +
                                              $"'{columnas[i - 1].ToUpper()}', '{obj.formula}', {obj.tipo}, {obj.orden}, {lastItem}," +
                                              $"{cellValue}, {cellValue}, 0)\n";

                                        lastItem++;
                                        
                                      
                                        registros++;
                                        //GUARDAMOS VALORES CORRESPONDIENTES EN LISTA                           

                                        dataFila.item = columnas[i - 1];
                                        NuevoVal.item = columnas[i - 1];
                                        NuevoVal.Adicional = Convert.ToDouble(cellValue);

                                        ListadoItemsArchivos.Add(dataFila);
                                        //GUARDAMOS EN LISTADO LOS NUEVO VALORES
                                        ListadoNuevosValores.Add(NuevoVal);
                                        //Guardamos items en listado de items no repetidos
                                        if (ExisteItemListado(columnas[i - 1], ListadoItemsNoRpetidos) == false)
                                            ListadoItemsNoRpetidos.Add(columnas[i - 1]);
                                    }
                                }

                             
                            }
                        }                        
                    }
                    else
                    {
                        //SI LA LINEA ESTA VACÍA
                        return false;
                    }
                    count++;
                }

                //VERIFICAR QUE EL CONTRATO NO SE REPITA
                if (ElementoRepetido(contratos))
                { XtraMessageBox.Show("Verifica que no hayan contratos repetidos", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);if (splashScreenManager1.IsSplashFormVisible) splashScreenManager1.CloseWaitForm(); return false; }

                if (resumen.Count > 0)
                {
                    gridResumen.DataSource = resumen;
                    fnSistema.spOpcionesGrilla(viewResumen);
                    Query.Add("insert", query);
                    Query.Add("update", queryUpdate);
                    correcto = true;
                }

               
                Lector.Close();
            }
            catch (Exception ex)
            {
                if (splashScreenManager1.IsSplashFormVisible)
                    splashScreenManager1.CloseWaitForm();

                MessageBox.Show(ex.Message);
            }

            return correcto;
        }

        //GENERA SQL DESDE RANGO
        private Hashtable GetSqlFromFile(Range rango)
        {
            Query.Clear();

            string query = "", queryUpdate = "";
            string ColumnName = "", rut = "", cellValue = "", contrato = "";
            int Filas = 0, lastItem = 0;
            int Columnas = 0;
            bool existe = false;
            item obj = new item();

            Hashtable SqlProc = new Hashtable();
            ItemTrabajador data = new ItemTrabajador();
            ItemTrabajador NuevoVal = new ItemTrabajador();
            ListadoItemsNoRpetidos.Clear();
            ListadoContratosNoRepetidos.Clear();

            Filas = rango.RowCount;
            Columnas = rango.ColumnCount;

            if (Filas > 0 && Columnas > 0)
            {
                registros = 0;
                //RECORREMOS
                for (int row = 1; row < Filas; row++)
                {
                    for (int col = 0; col < Columnas; col++)
                    {                       
                        ColumnName = rango[0, col].Value.ToString().Trim();                        
                        cellValue = rango[row, col].Value.ToString().Trim();

                        if (cellValue == "") continue;

                        data = new ItemTrabajador();
                        NuevoVal = new ItemTrabajador();
                        
                       //COLUMNA CONTRATO
                       if (col == 0)
                       {
                            //OBTENEMOS EL RUT Y EL N° DEL ULTIMO ITEM INGRESADO
                            contrato = rango[row, 0].Value.ToString().Trim();
                            rut = GetRut(contrato);
                            //retorna el ulitmo numero de item registrado + 1
                            lastItem = LastNumberItem(contrato);

                            //GUARDAMOS CONTRATO EN LISTADO DE NO REPETIDOS
                            ListadoContratosNoRepetidos.Add(contrato);                           
                        }
                        else
                        {
                            //SOLO INGRESAMOS SI LA CELDA ES DISTINTO DE VACÍO
                            //SI ESTÁ VACÍA NO LA CONSIDERAMOS...
                            if (cellValue != "" && fnSistema.IsDecimal(cellValue))
                            {
                                data.contrato = contrato;
                                data.Rut = rut;

                                NuevoVal.contrato = contrato;
                                NuevoVal.Rut = rut;

                                //SON ITEMS
                                obj = GetInfoItem(ColumnName);

                                if (cellValue.Contains(","))
                                    cellValue = cellValue.Replace(",", ".");                                
                                
                                //SQL PARA INSERT
                                query = query + "INSERT INTO ITEMTRABAJADOR(contrato, rut, anomes, coditem, formula, tipo, orden, numitem, valor, valorcalculado, esclase) " +
                                $"VALUES('{contrato}', '{rut}', {Calculo.PeriodoObservado}, " +
                                $"'{ColumnName.ToUpper()}', '{obj.formula}', {obj.tipo}, {obj.orden}, {lastItem}," +
                                $"{cellValue}, {cellValue}, 0)\n";

                                //SQL PARA UPDATE
                                queryUpdate = queryUpdate + $"UPDATE ITEMTRABAJADOR SET valor={cellValue} WHERE contrato='{contrato}' AND " +
                                               $"anomes={Calculo.PeriodoObservado} AND coditem='{ColumnName}'\n";

                               //PARA NUMERO DE ITEM
                               lastItem++;                          
                               
                                registros++;

                                data.item = ColumnName;
                                NuevoVal.item = ColumnName;
                                if (cellValue.Contains("."))
                                    cellValue = cellValue.Replace(".", ",");

                                NuevoVal.Adicional = Convert.ToDouble(cellValue);

                                ListadoItemsArchivos.Add(data);
                                //GUARDAMOS EN LISTADO LOS NUEVO VALORES
                                ListadoNuevosValores.Add(NuevoVal);
                                //Guardamos items en listado de items no repetidos
                                if (ExisteItemListado(ColumnName, ListadoItemsNoRpetidos) == false)
                                    ListadoItemsNoRpetidos.Add(ColumnName);
                              
                            }                          
                        }
                    }
                }
            }

            //GUARDAMOS DATOS EN HASHTABLA
            SqlProc.Add("insert", query);
            SqlProc.Add("update", queryUpdate);

            //RETORNAMOS HASHTABLE
            return SqlProc;
        }

        /// <summary>
        /// indica si un contrato tiene mas de una vez un item.
        /// </summary>
        /// <param name="Items"></param>
        /// <param name="pContratos"></param>
        /// <param name="pPeriodo"></param>
        /// <returns></returns>
        private bool RecorreItems(List<ItemTrabajador> pListado,int pPeriodo)
        {
            bool repetido = false;
            //List<ItemTrabajador> ListadoRepetidos = new List<ItemTrabajador>();

            if (pListado.Count > 0)
            {
                //RECORREMOS CADA CONTRATO Y PREGUNTAMOS POR CADA ITEMS DEL LISTADO
                foreach (var elemento in pListado)
                {
                    if (ItemRepetido(elemento.contrato, pPeriodo, elemento.item))
                    {
                        repetido = true;
                        //guardamos en campo adicional el nuevo valor que se quiere ingresar                       
                        //ListadoRepetidos.Add(elemento);
                        break;   
                    }

                    if (repetido)
                        break;
                }
            }

            return repetido;
        }

        //VERIFICAR SI HAY ALGUN CONTRATO QUE TIENE REPETIDO EL ITEM
        private bool ItemRepetido(string pContrato, int pPeriodo, string pCoditem)
        {
            bool repetido = false;
            string sql = "SELECT count(*) FROM itemtrabajador WHERE contrato=@pContrato AND coditem=@pItem AND anomes=@pPeriodo";
            SqlCommand cmd;
            SqlConnection cn;
            
            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@pContrato", pContrato));
                            cmd.Parameters.Add(new SqlParameter("@pItem", pCoditem));
                            cmd.Parameters.Add(new SqlParameter("@pPeriodo", pPeriodo));

                            object data = cmd.ExecuteScalar();
                            if (data != null)
                            {
                                if (Convert.ToInt32(data) >= 1)
                                    repetido = true;
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

            return repetido;
        }

        private List<ItemTrabajador> GetListadoRepetidos(int pPeriodo, List<string> pListadoExcel, List<string> pListadoItems)
        {
            string sql = "SELECT rut, contrato, numitem, coditem, valor, valorcalculado, 0 as 'Nuevo' FROM " +
                         "(select rut, contrato, numitem, coditem, count(*) OVER(PARTITION BY coditem, contrato) as Count, valor, valorcalculado " +
                         "FROM itemtrabajador " +
                         "WHERE anomes = @pPeriodo " +
                         ") " +
                         "as t " +
                        "WHERE count > 0 " +
                        "ORDER BY contrato, coditem";

            SqlCommand cmd;
            SqlConnection cn;
            SqlDataReader rd;
            List<ItemTrabajador> ListadoBd = new List<ItemTrabajador>();
            List<ItemTrabajador> pListadoSeleccion = new List<ItemTrabajador>();
            List<ItemTrabajador> pListadoFinal = new List<ItemTrabajador>();
            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@pPeriodo", pPeriodo));

                            rd = cmd.ExecuteReader();

                            if (rd.HasRows)
                            {
                                while (rd.Read())
                                {
                                    //GUARDAR DATOS EN LISTADO
                                    ListadoBd.Add(new ItemTrabajador()
                                    {
                                        contrato = (string)rd["contrato"],
                                        Rut = (string)rd["rut"],
                                        NumeroItem = Convert.ToInt32(rd["numitem"]),
                                        item = (string)rd["coditem"],
                                        valorOriginal = Convert.ToDouble(rd["valor"]),
                                        calculado = Convert.ToDouble(rd["valorcalculado"])
                                    });
                                }
                            }

                            cmd.Dispose();
                            rd.Close();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
              //ERROR...
            }

            string contrato = "";

            if (ListadoBd.Count > 0 && pListadoExcel.Count > 0)
            {
                //COMPARAMOS LISTADO Y SOLO DEJAMOS EN LISTADO FINAL LOS CONTRATOS QUE SON IGUALES
                //CONTRATO DE EXCEL EXISTE EN LISTADO BD?              

                for (int i = 0; i < pListadoExcel.Count; i++)
                {
                    for (int j = 0; j < ListadoBd.Count; j++)
                    {
                        if (pListadoExcel[i] == ListadoBd[j].contrato)
                            pListadoSeleccion.Add(ListadoBd[j]);
                    }
                }

                if (pListadoItems.Count > 0)
                {
                    foreach (var item in pListadoItems)
                    {
                        foreach (var x in pListadoSeleccion)
                        {
                            if (item.ToLower() == x.item.ToLower())
                                pListadoFinal.Add(x);
                        }
                    }
                }
            }

            return pListadoFinal;
        }

        private bool ExisteItemListado(string pItem, List<string> pListado)
        {
            bool existe = false;
            if (pListado.Count > 0)
            {
                foreach (var item in pListado)
                {
                    if (item.ToLower() == pItem.ToLower())
                    { existe = true; break; }
                }
            }

            return existe;
        }
        #endregion

        private void btnCargaInformacion_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (txtRuta.Text == "" || File.Exists(txtRuta.Text) == false)
            { XtraMessageBox.Show("Ruta de archivo no valida", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (File.Exists(txtRuta.Text) == false)
            { XtraMessageBox.Show("Archivo no valida", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (txtOperacion.Properties.DataSource == null || txtOperacion.EditValue == null)
            { XtraMessageBox.Show("Por favor selecciona una opción", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            string Textoption = "";
            if (Convert.ToInt32(txtOperacion.EditValue) == 1)
                Textoption = "Ingresar";
            else
                Textoption = "Modificar";

            if (Query.Count > 0)
            {
                //ES UPDATE????
                if (Convert.ToInt32(txtOperacion.EditValue) == 2)
                {
                    if (ListadoContratosNoRepetidos.Count > 0 && ListadoItemsArchivos.Count > 0 && ListadoNuevosValores.Count > 0)
                    {
                        //VERIFICAMOS SI HAY ITEMS REPETIDOS
                        if (RecorreItems(ListadoItemsArchivos, Calculo.PeriodoObservado))
                        {
                            DialogResult Adv = XtraMessageBox.Show("Se detectaron trabajadores que tienen mas de una vez un item.\n¿Deseas seleccionar items?", "Mensaje", MessageBoxButtons.YesNo, MessageBoxIcon.Stop);
                            if (Adv == DialogResult.Yes)
                            {
                                List<ItemTrabajador> ListadoSelec = new List<ItemTrabajador>();

                                ListadoSelec = GetListadoRepetidos(Calculo.PeriodoObservado, ListadoContratosNoRepetidos, ListadoItemsNoRpetidos);
                                if (ListadoSelec.Count > 0)
                                {
                                    //MOSTRAMOS FORMULARIO CORRESPONDIENTES
                                    frmSeleccionMultiple Multiple = new frmSeleccionMultiple(ListadoSelec, ListadoNuevosValores);
                                    Multiple.Opener = this;
                                    Multiple.StartPosition = FormStartPosition.CenterParent;
                                    Multiple.ShowDialog();

                                    if (SqlQuery.Length > 0)
                                    {
                                        //Ejecutamos consuLTA
                                        DialogResult Ingreso = XtraMessageBox.Show("¿Estás seguro de realizar modificación?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                        if (Ingreso == DialogResult.Yes)
                                        {
                                            Query.Clear();
                                            Query.Add("update", SqlQuery);

                                            IngresarDatos(Query, 2);
                                        }
                                    }
                                    else
                                    {
                                        XtraMessageBox.Show("No se pudo realizar operación", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                    }
                                }
                                else
                                {
                                    XtraMessageBox.Show("No se pudo realizar operación", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                }
                            }
                        }
                    }
                }              
                else
                {
                    //SOLO INGRESAMOS O ACTUALZAMOS DATOS
                    //GUARDAMOS DATA EN DB
                    DialogResult advertencia = XtraMessageBox.Show($"¿Estás seguro de {Textoption} {registros} registros?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (advertencia == DialogResult.Yes)
                        IngresarDatos(Query, Convert.ToInt32(txtOperacion.EditValue));
                }
               
            }
            else
            {
                XtraMessageBox.Show("Error al intentar guardar ", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void btnSalirArea_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            Close();
        }

        private void frmCargaMasivaItems_Load(object sender, EventArgs e)
        {
            Opcion op = new Opcion();
            op.CargarCombo(txtOperacion);
        }

        private void txtRuta_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void btnReporte_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            if (viewResumen.RowCount>0)
            {
                XtraReport report = ReportGenerator.GenerateReport(viewResumen);                
                
                report.ShowPreview();
                
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            if (txtRuta.Text != "")
            {
                DataTable Tabla = new DataTable();
                string extension = "";
                bool correcto = false;
                
                lblResult.Visible = true;
                lblResult.Text = "Leyendo...";
                extension = Path.GetExtension(txtRuta.Text);
                splashScreenManager1.ShowWaitForm();

                if (extension == ".csv")
                    //Tabla = FileExcel.ReadCsv(abrir.FileName);
                    correcto = LecturaCsv(txtRuta.Text);
                else
                    // Tabla = FileExcel.ReadExcelDev(abrir.FileName);
                    correcto = LecturaExcel(txtRuta.Text);

                if (correcto == false)
                { XtraMessageBox.Show("Hay errores en el archivo", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); lblResult.Visible = false; if (splashScreenManager1.IsSplashFormVisible) splashScreenManager1.CloseWaitForm(); btnRefresh.Enabled = true; btnCargaInformacion.Enabled = false; return; }

                if (correcto)
                {
                    splashScreenManager1.CloseWaitForm();
                    lblResult.Text = "Ok.";
                    btnCargaInformacion.Enabled = true;
                    btnRefresh.Enabled = false;
                    //Query = GeneraSql(Tabla);

                    lblRegistros.Text = "Registros encontrados # " + registros;

                    ColumnasGrilla();
                }
            }

        }

        private void panelControl1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}