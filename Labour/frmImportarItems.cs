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
using System.IO;

namespace Labour
{
    public partial class frmImportarItems : DevExpress.XtraEditors.XtraForm
    {

        private DataTable informacionArchivo = null;
        public frmImportarItems()
        {
            InitializeComponent();
        }

        private void panelControl1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void frmImportarItems_Load(object sender, EventArgs e)
        {

        }

        private void btnImportar_Click(object sender, EventArgs e)
        {
            //SESION NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            List<string> columnas = new List<string>();
            List<string> items = new List<string>();
            columnas = ListaColumnasValidas();
            items = ListaItemsValidos();
            string extension = "";

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm|CSV file (*.csv)|*.csv";
            //dialog.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";

            DataTable data = new DataTable();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (FileExcel.ExcelAbierto(dialog.FileName)) { XtraMessageBox.Show("Por favor cierra el archivo antes de continuar", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);return; }

                txtRuta.Text = dialog.FileName;
                //GUARDAMOS EXTENSION ARCHIVO
                extension = Path.GetExtension(dialog.FileName);

                if (extension == ".xls" || extension == ".xlsx")
                    data = FileExcel.ReadExcelDev(dialog.FileName);
                else
                    data = FileExcel.ReadCsv(dialog.FileName); 

                if (data != null)
                {
                    //VERIFICAMOS QUE LAS COLUMNAS SEAN VALIDAS
                    if (ColumnasValidas(columnas, data) == true) { btnCargaInformacion.Enabled = false; lblResult.Visible = true; lblResult.Text = "Archivo con errores"; return; }

                    //VALIDA QUE LAS FILAS TENGAN LOS DATOS CORRECTOS
                    if (ValidaInformacion(data, items) == true) { btnCargaInformacion.Enabled = false; lblResult.Visible = true; lblResult.Text = "Archivo con errores"; return; }

                    //VALIDA QUE NO HAYA REGISTROS REPETIDOS (CONTRATOS)
                    if (ContratosRepetidos(data)) { btnCargaInformacion.Enabled = false; lblResult.Visible = true; lblResult.Text = "Archivo con errores"; return; }

                    informacionArchivo = data;
                    lblResult.Visible = true;
                    lblResult.Text = "Ok.";
                    btnCargaInformacion.Enabled = true;

                    //SI TODO ESTA CORRECTO REALIZAMOS INSERT
                    //if (IngresarItems(data) == true)
                    //{ XtraMessageBox.Show("Ingreso realizado correctamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); }
                }
                else
                {
                    XtraMessageBox.Show("No se encontró informacion", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    lblResult.Visible = false;
                }
            }
        }

        private void btnSalirArea_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            if (txtRuta.Text != "")
            {
                DialogResult dialogo = XtraMessageBox.Show("Tienes pendiente la carga de un archivo, ¿Deseas cerrar de todas maneras?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogo == DialogResult.Yes)
                    Close();                
            }
            else
                Close();
        }

        #region "MANEJO DE DATOS"
        //VER SI LAS COLUMNAS INGRESADAS SON COLUMNAS VALIDAS
        private List<string> ListaColumnasValidas()
        {
            List<string> lista = new List<string>();
            lista.Add("coditem");            
            lista.Add("contrato");
            lista.Add("valor");
            lista.Add("valorcalculado");            
            lista.Add("permanente");

            return lista;
        }

        //LISTADO CODIGOS DE ITEM VALIDOS
        private List<string> ListaItemsValidos()
        {
            List<string> lista = new List<string>();
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
                                //LLENAMOS LISTADO CON ITEMS
                                lista.Add(((string)rd["coditem"]).ToLower());
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

            return lista;
        }

        //COMPARA COLUMNAS DE DATATABLE CON LISTADO DE COLUMNAS VALIDAS
        private bool ColumnasValidas(List<string> lista, DataTable Tabla)
        {
            bool novalidas = false;
            int count = 0;
            DataColumnCollection columnasTabla;
            if (lista.Count > 0 && Tabla.Rows.Count > 0)
            {
                columnasTabla = Tabla.Columns;

                //RECORRER columnas
                foreach (DataColumn column in columnasTabla)
                {
                    if (GetColumnInvalid(column.ColumnName.ToLower(), lista) == false)
                    {
                        XtraMessageBox.Show($"La columna {column.ColumnName} no es válida", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        novalidas = true;
                        break;
                    }

                    if (column.ColumnName.ToLower() == "coditem" || column.ColumnName.ToLower() == "contrato" || column.ColumnName == "valor")
                        count++;
                }

                if (count < 3)
                {
                    XtraMessageBox.Show("Verifique que al menos existan la columnas contrato, item y valor a ingresar", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    novalidas = true;                    
                }
            }

            return novalidas;
        }

        private bool GetColumnInvalid(string ColumnName, List<string> Columns)
        {
            int flag = 0;
            foreach (string column in Columns)
            {
                if (column == ColumnName)
                { flag = 1; break; }
            }

            if (flag == 1)
                return true;
            else
                return false;
        }

        //VALIDAR QUE EL CODIGO DE ITEM EXISTE
        private bool ExisteItem(string CodItem, List<string> lista)
        {
            bool existe = false;

            if (lista.Count>0 && CodItem.Length > 0)
            {
                foreach (var elemento in lista)
                {
                    if (elemento == CodItem.ToLower())
                    { existe = true; break; }
                }
            }

            return existe;
        }

        //VALIDAR DATOS DE DATATABLE PARA CADA COLUMNA
        private bool ValidaInformacion(DataTable pTabla, List<string> Lista)
        {
            bool novalida = false;
            double number = 0;
            if (pTabla != null)
            {
                foreach (DataRow row in pTabla.Rows)
                {
                    foreach (DataColumn column in pTabla.Columns)
                    {                      
                        if (column.ColumnName.ToLower() == "contrato")
                        {
                            //VERIFICAR SI CONTRATO EXISTE PARA EL PERIODO OBSERVADO
                            if (ExisteContrato(row[column.ColumnName].ToString()) == false)
                            { XtraMessageBox.Show($"No existe el contrato {row[column.ColumnName]}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);novalida = true; return true; }
                        }
                        if (column.ColumnName.ToLower() == "coditem")
                        {
                            //VERIFICAR SI EL CODIGO DE ITEM EXISTE                            
                            if (ExisteItem(row[column.ColumnName].ToString(), Lista) == false)
                            { XtraMessageBox.Show($"No existe el item {row[column.ColumnName]}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); novalida = true; return true; }
                        }
                        if (column.ColumnName.ToLower() == "valor")
                        {
                            //VERIFICAR QUE EL VALOR INGRESADO SEA NUMERICO
                            if (double.TryParse(row[column.ColumnName].ToString(), out number) == false)
                            { XtraMessageBox.Show($"Por favor ingresa un numero valido en la columna {column.ColumnName}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); novalida = true; return true; }
                        }
                        if (column.ColumnName.ToLower() == "valorcalculado")
                        {
                            //VERIFICAR QUE EL VALOR INGRESADO SEA NUMERICO
                            if (double.TryParse(row[column.ColumnName].ToString(), out number) == false)
                            { XtraMessageBox.Show($"Por favor ingresa un numero valido en la columna {column.ColumnName}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); novalida = true; return true; }
                        }
                    }
                }
            }

            return novalida;
        }

        //VALIDAR QUE NO HAYAN CONTRASTOS REPETIDOS
        private bool ContratosRepetidos(DataTable pData)
        {
            bool repetido = false;
            List<string> contratos = new List<string>();
            int count = 0;

            if (pData.Rows.Count > 0)
            {
                foreach (DataRow row in pData.Rows)
                {
                    foreach (DataColumn column in pData.Columns)
                    {
                        if (column.ColumnName.ToString() == "contrato")
                            contratos.Add(row[column.ColumnName].ToString());
                    }
                }

                //RECORREMOS LISTA DE CONTRATOS
                foreach (var item in contratos)
                {
                    count = 0;
                    foreach (var contrato in contratos)
                    {
                        if (contrato == item)
                            count++;
                    }

                    if (count > 1)
                    {
                        XtraMessageBox.Show("Hay mas de un registro con el mismo contrato", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        repetido = true;
                        break;
                    }
                }
            }

            return repetido;
        }

        //VERIFICAR SI CONTRATO EXISTE PARA EL PERIODO SELECCIONADO
        private bool ExisteContrato(string pContrato)
        {
            bool existe = false;
            if (pContrato.Length == 0) return false;

            string sql = "SELECT count(*) FROM trabajador WHERE contrato=@pContrato AND anomes=@pPeriodo";
            SqlCommand cmd;
            int count = 0;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pContrato", pContrato));
                        cmd.Parameters.Add(new SqlParameter("@pPeriodo", Calculo.PeriodoObservado));

                        count = Convert.ToInt32(cmd.ExecuteScalar());

                        if (count > 0)
                            existe = true;
                        else
                            existe = false;

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

        //INGRESAR ITEMS
        private bool IngresarItems(DataTable pTabla)
        {
            bool transaccionCorrecta = false, cancelar = false;
            int numItem = 0, count = 0;
            string rutEmpleado = "";
            Hashtable infoitem = new Hashtable();
            //INSERT
            string sqlInsert = "INSERT INTO itemtrabajador(coditem, anomes, contrato, rut, numitem, formula, tipo, orden, esclase, valor, valorcalculado, proporcional, permanente, contope, cuota)" +
                " VALUES(@pCoditem, @pPeriodo, @pContrato, @pRut, @pNumItem, @pFormula, @pTipo, @pOrden, @pEsclase, @pValor, @pValorcalculado, @pProporcional, @pPermanente, @ptope, @pCuota); ";
            //OBTENER NUMERO ITEM PARA NUEVO REGISTRO
            string sqlNumitem = "SELECT (MAX(numitem) + 1) FROM itemtrabajador WHERE contrato=@pContrato AND anomes=@pPeriodo";
            //OBTENER LOS VALORES DEL ITEM (ORDEN, TIPO FORMULA)
            string sqlDatosItem = "select tipo, formula, orden from item WHERE coditem=@pItem";
            //OBTENER EL RUT EN BASE AL NUMERO DE CONTRATO
            string sqlRut = "SELECT rut FROM trabajador WHERE contrato=@pContrato AND anomes=@pPeriodo";
            //VER SI CONTRATO YA TIENE ESE ITEMS
            string sqlExisteItem = "SELECT count(*) FROM itemtrabajador WHERE coditem=@pItem AND contrato=@pContrato AND anomes=@pPeriodo";

            SqlCommand cmd;
            SqlTransaction tr;
            SqlDataReader rd;
            if (pTabla != null)
            {
                //RECORREMOS DATATABLE
                try
                {
                    if (fnSistema.ConectarSQLServer())
                    {
                        tr = fnSistema.sqlConn.BeginTransaction();
                       
                            //RECORREMOS FILAS
                            foreach(DataRow row in pTabla.Rows)
                            {
                                try
                                {
                                    //BUSCAR RUT EN BASE A CONTRATO
                                    using (cmd = new SqlCommand(sqlRut, fnSistema.sqlConn))
                                    {
                                        //PARAMETROS
                                        cmd.Parameters.Add(new SqlParameter("@pContrato", row["contrato"].ToString()));
                                        cmd.Parameters.Add(new SqlParameter("@pPeriodo", Calculo.PeriodoObservado));

                                        cmd.Transaction = tr;
                                        rutEmpleado = (string)cmd.ExecuteScalar();

                                        cmd.Parameters.Clear();
                                        cmd.Dispose();
                                    }

                                //VERIFICAR SI EXISTE ITEMTRABAJADOR EN CONTRATO
                                using (cmd = new SqlCommand(sqlExisteItem, fnSistema.sqlConn))
                                {
                                    //PARAMETROS
                                    cmd.Parameters.Add(new SqlParameter("@pContrato", row["contrato"].ToString()));
                                    cmd.Parameters.Add(new SqlParameter("@pPeriodo", Calculo.PeriodoObservado));
                                    cmd.Parameters.Add(new SqlParameter("@pItem", row["coditem"].ToString()));

                                    //AGREGAMOS A TRANSACCION
                                    cmd.Transaction = tr;

                                    if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                                    {
                                        DialogResult advertencia = XtraMessageBox.Show($"contrato n° {row["contrato"]} ya tiene asociado item {row["coditem"].ToString().ToUpper()}, ¿Deseas continuar de todas maneras?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                        if (advertencia == DialogResult.No)
                                        {
                                            transaccionCorrecta = false;
                                            tr.Rollback();
                                            break;
                                        }
                                    }
                                }
                                
                                    //buscar informacion item
                                    using (cmd = new SqlCommand(sqlDatosItem, fnSistema.sqlConn))
                                    {
                                        //PARAMETROS
                                        cmd.Parameters.Add(new SqlParameter("@pItem", row["coditem"]));

                                        //AGREGAMOS A TRANSACCION
                                        cmd.Transaction = tr;

                                        rd = cmd.ExecuteReader();
                                        if (rd.HasRows)
                                        {
                                            while (rd.Read())
                                            {
                                                //AGREGAMOS 
                                                infoitem.Add("tipo", Convert.ToInt32(rd["tipo"]));
                                                infoitem.Add("formula", (string)(rd["formula"]));
                                                infoitem.Add("orden", Convert.ToInt32(rd["orden"]));
                                            }
                                        }
                                        cmd.Parameters.Clear();
                                        rd.Close();
                                        cmd.Dispose();
                                    }

                                    if (infoitem.Count > 0 && rutEmpleado.Length > 0)
                                    {
                                        //BUSCAMOS NUMERO ITEM PARA REALIZAR REGISTRO
                                        using (cmd = new SqlCommand(sqlNumitem, fnSistema.sqlConn))
                                        {
                                            //PARAMETROS
                                            cmd.Parameters.Add(new SqlParameter("@pContrato", row["contrato"].ToString()));
                                            cmd.Parameters.Add(new SqlParameter("@pPeriodo", Calculo.PeriodoObservado));

                                            cmd.Transaction = tr;

                                            numItem = Convert.ToInt32(cmd.ExecuteScalar());

                                            cmd.Parameters.Clear();
                                            cmd.Dispose();
                                        }

                                        //REALIZAMOS INSERT
                                        using (cmd = new SqlCommand(sqlInsert, fnSistema.sqlConn))
                                        {

                                            //PARAMETROS
                                            cmd.Parameters.Add(new SqlParameter("@pCoditem", (row["coditem"]).ToString().ToUpper()));
                                            cmd.Parameters.Add(new SqlParameter("@pPeriodo", Calculo.PeriodoObservado));
                                            cmd.Parameters.Add(new SqlParameter("@pContrato", row["contrato"].ToString()));
                                            cmd.Parameters.Add(new SqlParameter("@pRut", rutEmpleado));
                                            cmd.Parameters.Add(new SqlParameter("@pNumItem", numItem));
                                            cmd.Parameters.Add(new SqlParameter("@pFormula", (string)infoitem["formula"]));
                                            cmd.Parameters.Add(new SqlParameter("@pTipo", Convert.ToInt32(infoitem["tipo"])));
                                            cmd.Parameters.Add(new SqlParameter("@pOrden", Convert.ToInt32(infoitem["orden"])));
                                            cmd.Parameters.Add(new SqlParameter("@pEsclase", false));
                                            cmd.Parameters.Add(new SqlParameter("@pValor", Convert.ToDouble(row["valor"])));
                                            cmd.Parameters.Add(new SqlParameter("@pValorcalculado", Convert.ToDouble(0)));
                                            cmd.Parameters.Add(new SqlParameter("@pProporcional", false));
                                            cmd.Parameters.Add(new SqlParameter("@pPermanente", false));
                                            cmd.Parameters.Add(new SqlParameter("@ptope", false));
                                            cmd.Parameters.Add(new SqlParameter("@pCuota", "0"));

                                            cmd.Transaction = tr;

                                            //EJECUTAMOS QUERY
                                            cmd.ExecuteNonQuery();                                 
                                        }
                                    }

                                    //PARA CONTAR LA CANTIDAD REGISTROS 
                                    count++;

                                    //SI LA CANTIDAD DE REGISTROS ES IGUAL A LA CANTIDAD DE ROWS, OK!
                                    if (count == pTabla.Rows.Count)
                                    {
                                        transaccionCorrecta = true;
                                        tr.Commit();
                                        fnSistema.sqlConn.Close();
                                    }
                                }
                                catch (SqlException ex)
                                {
                                    XtraMessageBox.Show(ex.Message);
                                    tr.Rollback();
                                    transaccionCorrecta = false;
                                }                           
                            }                      
                    }
                }
                catch (SqlException ex)
                {
                    XtraMessageBox.Show(ex.Message);
                }
            }

            return transaccionCorrecta;
        }

        //VERIFICAR SI ALGUN TRABAJADOR YA TIENE ITEM QUE SE INTENTA INGRESAR
        private bool TrabajadorTieneItem(string pItem, string pContrato)
        {
            bool existe = false;
            string sql = "SELECT count(*) FROM itemtrabajador WHERE itemtrabajador=@pItem AND " +
                        "contrato=@pContrato AND anomes=@pPeriodo";
            SqlCommand cmd;
            int count = 0;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pItem", pItem));
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
            return existe;
        }

        #endregion

        private void btnCargaInformacion_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            if (txtRuta.Text == "") { XtraMessageBox.Show("No se ha cargado ningun archivo", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);return; }

            if (informacionArchivo == null) { XtraMessageBox.Show("No hay informacion"); }

            DialogResult advertencia = XtraMessageBox.Show($"¿Estás seguro de ingresar {informacionArchivo.Rows.Count} registros?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (advertencia == DialogResult.Yes)
            {
                if (IngresarItems(informacionArchivo) == false) { XtraMessageBox.Show("Ha ocurrido un error", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
                else { XtraMessageBox.Show("Ingreso correcto", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); lblResult.Visible = false; txtRuta.Text = ""; }
            }            
        }

        private void labelControl1_Click(object sender, EventArgs e)
        {

        }

        private void txtRuta_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void lblResult_Click(object sender, EventArgs e)
        {

        }
    }
}