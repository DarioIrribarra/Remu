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

namespace Labour
{
    public partial class frmCargaMontoAfc : DevExpress.XtraEditors.XtraForm
    {
        private List<string> Columnas = new List<string>() { "rut",
            "monto", "mes" };

        private List<string> ListadoInsert = new List<string>();
        public frmCargaMontoAfc()
        {
            InitializeComponent();
        }

        private void btnCargar_Click(object sender, EventArgs e)
        {
            OpenFileDialog abrir = new OpenFileDialog();
            abrir.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
            string rutaArchivo = "", extension = "";
            bool correcto = false;

            Cursor = Cursors.WaitCursor;

            if (txtOperacion.Properties.DataSource == null || txtOperacion.EditValue == null)
            { XtraMessageBox.Show("Por favor selecciona una opción válida", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); Cursor = Cursors.Default; return; }

            if (abrir.ShowDialog() == DialogResult.OK)
            {
                //PARA GUARDAR LOS DATOS
                DataTable Tabla = new DataTable();

                rutaArchivo = abrir.FileName;
                if (File.Exists(rutaArchivo) == false)
                {
                    XtraMessageBox.Show("Ruta archivo no existe", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Cursor = Cursors.Default;
                    return;
                }

                txtRuta.Text = abrir.FileName;
                extension = Path.GetExtension(abrir.FileName);

                Tabla = FileExcel.ReadExcelDev(abrir.FileName);
                if (Tabla.Rows.Count == 0)
                { XtraMessageBox.Show("No se encontró información en archivo", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); Cursor = Cursors.Default; return; }


                ValidateData(Tabla);

                Cursor = Cursors.Default;

            }

            Cursor = Cursors.Default;
        }

        /// <summary>
        /// Validacion de columnas 
        /// </summary>
        /// <param name="pDataSource"></param>
        private void ValidateData(DataTable pDataSource)
        {
            int ColumnasValidas = 0;
            bool Error = false;
            StringBuilder str = new StringBuilder();
            //1--> Ingresar; 2--> Modificar
            int Selection = 0;
            Selection = Convert.ToInt32(txtOperacion.EditValue);

            ListadoInsert.Clear();
            
            if (pDataSource.Rows.Count > 0)
            {
                DataColumnCollection Columnas = pDataSource.Columns;
                if (Columnas.Count > 0)
                {
                    ColumnasValidas = ColumnValidation(Columnas);
                    if (ColumnasValidas < Columnas.Count)
                    {
                        XtraMessageBox.Show("Por favor verifique el nombre de las columnas", "Columna no válida", MessageBoxButtons.OK, MessageBoxIcon.Stop); Cursor = Cursors.Default; return;
                    }                    
                }

                string Filter = "Convert([rut], 'System.String') is null OR Convert([rut], 'System.String')='' " +
                    "OR Convert([monto], 'System.String') is null OR Convert([monto], 'System.String')='' " ;
                    
                DataRow[] FilasEncontradas = pDataSource.Select(Filter);

                if (FilasEncontradas.Length > 0)
                { XtraMessageBox.Show("Por favor verifique columnas con informacion en blanco", "Datos en blanco", MessageBoxButtons.OK, MessageBoxIcon.Stop); Cursor = Cursors.Default; return; }
           
                foreach (DataRow Row in pDataSource.Rows)
                {
                    string insert = $"INSERT INTO afc(mes, rut, monto) VALUES({Calculo.PeriodoObservado}, $rut, $monto)";
                    string update = $"UPDATE afc SET monto=$monto WHERE rut=$rut AND mes={Calculo.PeriodoObservado}";
                    foreach (DataColumn Col in pDataSource.Columns)
                    {
                        string ColumnName = Col.Caption.ToLower().Trim();
                        string data = Row[Col].ToString().Trim();

                        if (ColumnName.Equals("rut"))
                        {
                            if (fnSistema.fValidaRut(data) == false)
                            {
                                XtraMessageBox.Show($"Rut {data} no es un rut válido", "Rut", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                Cursor = Cursors.Default;
                                Error = true;
                                break;
                            }

                            //Verificar si el rut existe en el mes
                            if (Calculo.ExisteRutMes(data, Calculo.PeriodoObservado) == false)
                            {
                                XtraMessageBox.Show($"Rut {data} no existe en periodo evaluado", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                Cursor = Cursors.Default;
                                Error = true;
                                break;
                            }

                            if (ExisteAfc(data, Calculo.PeriodoObservado))
                            {
                                if (Selection == 1)
                                {
                                    XtraMessageBox.Show($"Ya existe un monto ingresado para el rut {data}\nSi lo que deseas hacer es actualizar el monto\npor favor selecciona la opcion actualizar", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                    Cursor = Cursors.Default;
                                    Error = true;
                                    break;
                                }
                            }

                            insert = insert.Replace("$rut", $"'{data.Trim()}'");
                            update = update.Replace("$rut", $"'{data.Trim()}'");
                        }                  
                       

                        if (ColumnName.Equals("monto"))
                        {
                            if (fnSistema.IsDecimal(data) == false)
                            {
                                XtraMessageBox.Show($"Monto {data} no es un valor válido", "Monto Erroneo", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                Cursor = Cursors.Default;
                                Error = true;
                                break;
                            }
                            else
                            {
                                insert = insert.Replace("$monto", data.Trim());
                                update = update.Replace("$monto", data.Trim());
                            }
                        }
                        
                    }

                    if (Error)
                    {
                        ListadoInsert.Clear();
                        break;

                    }                        

                    if(Selection == 1)
                        ListadoInsert.Add(insert);
                    if (Selection == 2)
                        ListadoInsert.Add(update);

                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool Insert()
        {
            SqlCommand cmd;
            SqlConnection cn;
            SqlTransaction tr;
            bool correcto = false;
            if (ListadoInsert.Count > 0)
            {
                try
                {
                    cn = fnSistema.OpenConnection();
                    if (cn != null)
                    {
                        tr = cn.BeginTransaction();
                        try
                        {
                           //Recorremos listado de queries
                            foreach (string query in ListadoInsert)
                            {
                                using (cmd = new SqlCommand(query, cn))
                                {
                                    cmd.Transaction = tr;
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            //Si no hubo error, hacemos elcommit
                            tr.Commit();
                            correcto = true;
                        }
                        catch (Exception ex)
                        {
                            tr.Rollback();
                            correcto = false;
                        }
                    }
                }
                catch (SqlException ex)
                {
                    //Error
                    correcto = false;
                }
            }

            return correcto;
        }


        /// <summary>
        /// Existe si ya existe un monto ingresado para un rut
        /// </summary>
        /// <param name="pRut"></param>
        private bool ExisteAfc(string pRut, int pPeriodo)
        {
            string sql = "SELECT count(*) FROM afc WHERE rut=@pRut AND mes=@pPeriodo";
            SqlCommand cmd;
            SqlConnection cn;
            bool existe = false;

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
                            cmd.Parameters.Add(new SqlParameter("@pRut", pRut));
                            cmd.Parameters.Add(new SqlParameter("@pPeriodo", pPeriodo));

                            object data = cmd.ExecuteScalar();
                            if (data != DBNull.Value)
                            {
                                if (Convert.ToInt32(data) > 0)
                                    existe = true;
                            }
                        }
                        cmd.Dispose();
                    }
                }
            }
            catch (SqlException ex )
            {
                //Error...
            }

            return existe;

        }
  

        /// <summary>
        /// Indica si todas las columnas del excel son columnas válidas...
        /// </summary>
        /// <param name="pColumns"></param>
        /// <returns>Un numero entero que indica la cantidad de coincidencias encontradas</returns>
        private int ColumnValidation(DataColumnCollection pColumns)
        {
            int count = 0;

            if (pColumns.Count > 0)
            {
                
                foreach (DataColumn col in pColumns)
                {
                    string ColumnName = col.Caption.ToLower().Trim();
                    if (ColumnName.Length == 0)
                    { XtraMessageBox.Show("Los nombres de las columnas no pueden estar vacios", "Columnas", MessageBoxButtons.OK, MessageBoxIcon.Stop); return -1; }
                    if (Columnas.Count(x => x.ToLower().Equals(ColumnName)) > 0)
                        count++;
                    else
                    {
                        XtraMessageBox.Show($"Columna {ColumnName} no es un nombre de columna válida", "Columnas", MessageBoxButtons.OK, MessageBoxIcon.Stop); return -1; 
                    }
                }

                //Si count == a cantidad de columnas de la tabla
                //quiere decir que todas las columnas existen en nuestra lista de 
                //columnas validas
               
            }

            return count;
        }

       

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void frmCargaItemExtendida_Load(object sender, EventArgs e)
        {
            Opcion op = new Opcion();
            op.CargarCombo(txtOperacion);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtOperacion.Properties.DataSource == null || txtOperacion.EditValue == null)
            { XtraMessageBox.Show("Por favor selecciona una operacion válida", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (File.Exists(txtRuta.Text))
            {
                //Hacemos el insert O UPDATE SEGUN CORRESPONDA
                if (ListadoInsert.Count == 0)
                {
                    XtraMessageBox.Show("No se pudo realizar operacion", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }                

                if (Insert())
                {
                    XtraMessageBox.Show("Registros guardados correctamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                }
                else
                {
                    XtraMessageBox.Show("No se pudo ingresar datos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }

                Close();


            }
            else
            {
                XtraMessageBox.Show("Ruta de archivo no válida", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void txtOperacion_EditValueChanged(object sender, EventArgs e)
        {
            txtRuta.Text = "";
        }

        private void btnList_Click(object sender, EventArgs e)
        {
            frmListadoAfc list = new frmListadoAfc();
            list.StartPosition = FormStartPosition.CenterParent;
            list.ShowDialog();

        }
    }
}