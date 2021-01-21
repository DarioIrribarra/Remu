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
    public partial class frmImportarTrabUpd : DevExpress.XtraEditors.XtraForm
    {

        private List<string> CadenasUpdate = new List<string>();

        public frmImportarTrabUpd()
        {
            InitializeComponent();
        }

        private void frmImportarTrabUpd_Load(object sender, EventArgs e)
        {

        }
        private void btnCargaInformacion_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (txtRuta.Text == "") { XtraMessageBox.Show("No hay ningun archivo cargado", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
            if (CadenasUpdate.Count == 0) { XtraMessageBox.Show("No hay informacion", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (CadenasUpdate.Count>0)
            {
                DialogResult pregunta = XtraMessageBox.Show($"¿Estás seguro de actualizar {CadenasUpdate.Count} registros?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (pregunta == DialogResult.Yes)
                {
                    if (UpdateInformacion(CadenasUpdate))
                    { XtraMessageBox.Show("Informacion actualizada correctamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);txtRuta.Text = "";lblResult.Visible = false; Close(); return; }
                    else
                    { XtraMessageBox.Show("No se pudo realizar actualizacion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
                }                
            }            
        }

        private void btnSalirArea_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            if (txtRuta.Text != "")
            {
                DialogResult advertencia = XtraMessageBox.Show("Tienes pendiente la carga de un archivo, ¿Deseas salir de todas maneras?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (advertencia == DialogResult.Yes)
                {
                    Close();
                }
            }
            else {
                Close();
            }            
        }

        #region "MANEJO DATOS"
        //LISTADO CAMPOS VALIDOS
        private List<string> ListaCamposValidos()
        {
            List<string> lista = new List<string>();
            lista.Add("contrato");
            lista.Add("status");
            lista.Add("nombre");
            lista.Add("apepaterno");
            lista.Add("apematerno");
            lista.Add("direccion");
            lista.Add("ciudad");
            lista.Add("sucursal");
            lista.Add("area");
            lista.Add("ccosto");
            lista.Add("fechanac");
            lista.Add("nacion");
            lista.Add("ecivil");
            lista.Add("telefono");
            lista.Add("ingreso");
            lista.Add("salida");
            lista.Add("cargo");
            lista.Add("tipocontrato");
            lista.Add("regimensalario");
            lista.Add("jubilado");
            lista.Add("regimen");
            lista.Add("afp");
            lista.Add("salud");
            lista.Add("cajaprevision");
            lista.Add("fechasegces");
            lista.Add("tramo");
            lista.Add("formapago");
            lista.Add("banco");
            lista.Add("cuenta");
            lista.Add("fechavacacion");
            lista.Add("fechaprogresivo");
            lista.Add("anosprogresivo");
            lista.Add("tipocuenta");
          //lista.Add("clase");
            lista.Add("causal");
            lista.Add("sexo");
            lista.Add("fun");
            lista.Add("numemer");
            lista.Add("nomemer");
            lista.Add("talla");
            lista.Add("calzado");
            lista.Add("horario");
            lista.Add("escolaridad");
            lista.Add("jornada");
            lista.Add("comuna");
            lista.Add("suslab");

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

                    if (column.ColumnName.ToLower() == "contrato")
                        count++;
                }

                if (count < 1)
                {
                    XtraMessageBox.Show("No se encontró la columna contrato", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
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

        //OBTENER FECHA DE INGRESO PARA CONTRATO
        private DateTime GetFechaIngreso(string pContrato)
        {
            string sql = "SELECT ingreso FROM trabajador WHERE contrato=@pContrato AND anomes=@pPeriodo";
            SqlCommand cmd;
            DateTime ingreso = DateTime.Now.Date;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pContrato", pContrato));
                        cmd.Parameters.Add(new SqlParameter("@pPeriodo", Calculo.PeriodoObservado));

                        ingreso = Convert.ToDateTime(cmd.ExecuteScalar());

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return ingreso;
        }

        //OBTENER FECHA DE SALIDA PARA CONTRATO
        private DateTime GetFechaSalida(string pContrato)
        {
            DateTime salida = DateTime.Now.Date;
            string sql = "SELECT salida FROM trabajador WHERE contrato=@pContrato AND anomes=@pPeriodo";
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

                        salida = Convert.ToDateTime(cmd.ExecuteScalar());
                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return salida;
        }

        //VERIFICAR SI CONTRATO EXISTE
        private bool ExisteContrato(string pContrato)
        {
            bool valido = false;
            int count = 0;
            string sql = "SELECT count(*) FROM trabajador WHERE contrato=@pContrato AND anomes=@pPeriodo";

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

                        count = Convert.ToInt32(cmd.ExecuteScalar());
                        if (count > 0)
                            valido = true;
                        else
                            valido = false;

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                        
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
            
            return valido;
        }

        //STATUS VALIDO
        private bool StatusValido(int pDato)
        {
            bool valido = false;
            if (pDato == 0 || pDato == 1)
                valido = true;

            return valido;
        }

        //OBTENER CODIGO CIUDAD DESDE SU NOMBRE
        private bool ExisteCiudad(int pId)
        {
            bool existe = false;
            string sql = "SELECT count(*) FROM ciudad where idCiudad=@pId";
            SqlCommand cmd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pId", pId));

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

        //TIPO CONTRATO
        /*
         * 0 - INDEFINIDO
         * 1 - PLAZO FIJO
         * 2 - OBRA O FAENA
         */
        private bool GetTipocontrato(int pId)
        {
            bool Existe = false;

            switch (pId)
            {
                case 0:
                    Existe = true;
                    break;

                case 1:
                    Existe = true;
                    break;

                case 2:
                    Existe = true;
                    break;

                default:
                    Existe = false;
                    break;
            }

            return Existe;
        }

        //REGIMEN SALARIO
        private int GetregimenSalario(string pName)
        {
            int cod = -1;
            if (pName.ToLower() == "fijo")
                cod = 1;
            if (pName.ToLower() == "variable")
                cod = 0;

            return cod;
        }

        //JUBILADO
        /*
         * 0 - NO
         * 1 - SI,NO COTIZA
         * 2 - SI, COTIZA
         */
        private bool GetJubilado(int pId)
        {
            bool existe = false;

            switch (pId)
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

        //OBTENER CODIGO CLASE REMJUNERACION
        private bool GetcodClase(int pId)
        {
            string sql = "SELECT count(*) FROM clase WHERE codclase=@pId";
            SqlCommand cmd;
            bool existe = false;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pId", pId));

                        object data = cmd.ExecuteScalar();
                        if (data != null)
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

        //OBTENER CODIGO AREA DESDE SU NOMBRE
        private bool GetcodFromField(int pField, string pTable)
        {
            bool existe = false;
            string sql = $"SELECT count(*) FROM {pTable} WHERE id=@pId"; 

            SqlCommand cmd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        cmd.Parameters.Add(new SqlParameter("@pId", pField));

                        object data = cmd.ExecuteScalar();
                        if (data != null)
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

        //OBTENER CODIGO SEXO
        private int GetcodSexo(string pName)
        {
            int cod = -1;
            if (pName.ToLower() == "m")
                cod = 0;
            if (pName.ToLower() == "f")
                cod = 1;

            return cod;
        }

        //OBTENER CODIGO CAUSAL TERMINO
        private bool GetcodCausal(int pId)
        {
            string sql = "SELECT count(*) FROM causaltermino WHERE codcausal=@pId";
            bool existe = false;
            SqlCommand cmd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pName", pId));

                        object data = cmd.ExecuteScalar();
                        if (data != null)
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

        //OBTENER CODIGO SUCURSAL DE ACUERDO AL NNOMBRE
        private bool ExisteSucursal(int pSucursal)
        {
           bool existe = false;

            string sql = "SELECT count(*) FROM SUCURSAL WHERE codSucursal=@pId";
            SqlCommand cmd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pId", pSucursal));

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

        //VALIDA INFORMACION
        private bool ValidaInformacion(DataTable pData, out List<string> queryUpdate)
        {
            int number = 0, regimen = 0;
            DateTime fecha = DateTime.Now.Date;
            DateTime fechaIngreso = DateTime.Now.Date;
            DateTime fechaTermino = DateTime.Now.Date;
            DateTime fechaSeguro = DateTime.Now.Date;
            string ContratoPersona = "";
            queryUpdate = new List<string>();
            bool valida = false;
            Hashtable InfoCampos = new Hashtable();
            if (pData != null)
            {
                //RECORREMOS DATATABLE
                foreach (DataRow row in pData.Rows)
                {
                    foreach (DataColumn column in pData.Columns)
                    {                     
                        if (column.ColumnName.ToLower() == "contrato")
                        {
                            if (ExisteContrato(row[column.ColumnName].ToString()) == false)
                            {
                                string c = row[column.ColumnName].ToString();
                                XtraMessageBox.Show($"Contrato n° {row[column.ColumnName]} no existe", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return false;}

                            InfoCampos.Add("contrato", $"'{row[column.ColumnName].ToString().ToUpper().Trim()}'");

                            ContratoPersona = row[column.ColumnName].ToString();
                        }
                        if (column.ColumnName.ToLower() == "status")
                        {
                            if (int.TryParse(row[column.ColumnName].ToString(), out number) == false)
                            { XtraMessageBox.Show($"Por favor ingresa un valor numerico en columna status", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (StatusValido(Convert.ToInt32(row[column.ColumnName])) == false)
                            { XtraMessageBox.Show($"Valor no valido para columna status", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return false; }

                            InfoCampos.Add("status", Convert.ToInt32(row[column.ColumnName]));
                        }
                        if (column.ColumnName.ToLower() == "nombre")
                        {
                            if (row[column.ColumnName].ToString() == "")
                            { XtraMessageBox.Show("La columna nombre no puede estar vacía", "informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);return false; }
                            if (int.TryParse(row[column.ColumnName].ToString(), out number))
                            { XtraMessageBox.Show("Por favor ingresa un valor valido en columna nombre", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            InfoCampos.Add("nombre", $"'{row[column.ColumnName].ToString().ToUpper().Trim()}'");
                        }
                        if (column.ColumnName.ToLower() == "apematerno")
                        {
                            if (row[column.ColumnName].ToString() == "")
                            { XtraMessageBox.Show("La columna apematerno no puede estar vacía", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (int.TryParse(row[column.ColumnName].ToString(), out number))
                            { XtraMessageBox.Show("Por favor ingresa un valor valido en columna apematerno", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);return false; }

                            InfoCampos.Add("apematerno", $"'{row[column.ColumnName].ToString().ToUpper().Trim()}'");
                        }
                        if (column.ColumnName.ToLower() == "apepaterno")
                        {
                            if (row[column.ColumnName].ToString() == "")
                            { XtraMessageBox.Show("La columna apepaterno no puede estar vacía", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);return false; }

                            if (int.TryParse(row[column.ColumnName].ToString(), out number))
                            { XtraMessageBox.Show("Por favor ingresa un valor valido en columna apepaterno", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            InfoCampos.Add("apepaterno", $"'{row[column.ColumnName].ToString().ToUpper().Trim()}'");
                        }
                        if (column.ColumnName.ToLower() == "direccion")
                        {
                            if (row[column.ColumnName].ToString() == "")
                            { XtraMessageBox.Show("La columna direccion no puede estar vacía", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (int.TryParse(row[column.ColumnName].ToString(), out number))
                            { XtraMessageBox.Show("Por favor ingresa un valor valido para la columna direccion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            InfoCampos.Add("direccion", $"'{row[column.ColumnName].ToString().ToUpper().Trim()}'");
                        }
                        if (column.ColumnName.ToLower() == "ciudad")
                        {
                            if (row[column.ColumnName].ToString() == "")
                            { XtraMessageBox.Show("La columna ciudad no puede estar vacía", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (fnSistema.IsNumeric(row[column.ColumnName].ToString()) == false)
                            { XtraMessageBox.Show("Por favor ingresa un valor valido en campo ciudad", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (ExisteCiudad(Convert.ToInt32(row[column.ColumnName])) == false)
                            { XtraMessageBox.Show($"Valor '{row[column.ColumnName]}' no valido para columna ciudad", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            //CAMBIAMOS VALOR SI ES VALIDO
                            row[column.ColumnName] = Convert.ToInt32(row[column.ColumnName]);

                            InfoCampos.Add("ciudad", Convert.ToInt32(row[column.ColumnName]));
                        }
                        if (column.ColumnName.ToLower() == "sucursal")
                        {
                            if (row[column.ColumnName].ToString() == "")
                            { XtraMessageBox.Show("La columna sucursal no puede estar vacía", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (fnSistema.IsNumeric(row[column.ColumnName].ToString()) == false)
                            { XtraMessageBox.Show("Por favor ingresa un valor valido en columna sucursal", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            //VALIDAR SI LA SUCURSAL EXISTE
                            if (ExisteSucursal(Convert.ToInt32(row[column.ColumnName])) == false)
                            { XtraMessageBox.Show($"Sucursal {row[column.ColumnName]} no existe", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            InfoCampos.Add("sucursal", Convert.ToInt32(row[column.ColumnName]));
                        }
                        if (column.ColumnName.ToLower() == "area")
                        {
                            if (row[column.ColumnName].ToString() == "")
                            { XtraMessageBox.Show("La columna area no puede estar vacía", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (fnSistema.IsNumeric(row[column.ColumnName].ToString()) == false)
                            { XtraMessageBox.Show("Por favor ingresa un valor valido para columna area", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (GetcodFromField(Convert.ToInt32(row[column.ColumnName]), "area") == false)
                            { XtraMessageBox.Show($"campo '{row[column.ColumnName]}' no valido para columna area", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            //SI ES VALIDO CAMBIAMOS VALOR A COLUMNA
                            row[column.ColumnName] = Convert.ToInt32(row[column.ColumnName]);

                            InfoCampos.Add("area", Convert.ToInt32(row[column.ColumnName]));
                        }
                        if (column.ColumnName.ToLower() == "ccosto")
                        {
                            if (row[column.ColumnName].ToString() == "")
                            { XtraMessageBox.Show("La columna ccosto no puede estar vacía", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (fnSistema.IsNumeric(row[column.ColumnName].ToString()) == false)
                            { XtraMessageBox.Show("Por favor ingresa un valor válido para columna ccosto", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (GetcodFromField(Convert.ToInt32(row[column.ColumnName]), "ccosto") == false)
                            { XtraMessageBox.Show($"Valor {row[column.ColumnName]} no valido", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            //SI ES VALIDO CAMBIAMOS VALOR
                            row[column.ColumnName] = Convert.ToInt32(row[column.ColumnName]);

                            InfoCampos.Add("ccosto", Convert.ToInt32(row[column.ColumnName]));
                        }
                        if (column.ColumnName.ToLower() == "fechanac")
                        {
                            if (row[column.ColumnName].ToString() == "")
                            { XtraMessageBox.Show("La columna fechanac no puede estar vacía", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (DateTime.TryParse(row[column.ColumnName].ToString(), out fecha) == false)
                            { XtraMessageBox.Show("Por favor ingresa una fecha valida en columna fechanac", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if ((DateTime.Now.Date.Year - Convert.ToDateTime(row[column.ColumnName]).Year) < 15)
                            { XtraMessageBox.Show($"Por favor verifica la fecha de nacimiento para el contrato {ContratoPersona}", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            InfoCampos.Add("fechanac", $"'{Convert.ToDateTime(row[column.ColumnName]).ToString("yyyy-MM-dd")}'");
                        }
                        if (column.ColumnName.ToLower() == "nacion")
                        {
                            if (row[column.ColumnName].ToString() == "")
                            { XtraMessageBox.Show("La columna no puede estar vacía", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (fnSistema.IsNumeric(row[column.ColumnName].ToString()) == false)
                            { XtraMessageBox.Show("Por favor ingresa un valor valido en columna nacion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (GetcodFromField(Convert.ToInt32(row[column.ColumnName]), "nacion") == false)
                            { XtraMessageBox.Show($"valor {row[column.ColumnName]} no es valido para columna nacion", "informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            //SI ES VALIDO CAMBIAMOS SU VALOR
                            row[column.ColumnName] = Convert.ToInt32(row[column.ColumnName]);

                            InfoCampos.Add("nacion", Convert.ToInt32(row[column.ColumnName]));
                        }
                        if (column.ColumnName.ToLower() == "ecivil")
                        {
                            if (row[column.ColumnName].ToString() == "")
                            { XtraMessageBox.Show("La columna ecivil no puede estar vacía", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (fnSistema.IsNumeric(row[column.ColumnName].ToString()) == false)
                            { XtraMessageBox.Show("Por favor ingresa un valor valido para columna ecivil", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (GetcodFromField(Convert.ToInt32(row[column.ColumnName]), "ecivil") == false)
                            { XtraMessageBox.Show($"valor {row[column.ColumnName]} no valido para columna ecivil", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            //SI ES VALIDO GUARDAMOS
                            row[column.ColumnName] = Convert.ToInt32(row[column.ColumnName]);

                            InfoCampos.Add("ecivil", Convert.ToInt32(row[column.ColumnName]));
                        }
                        if (column.ColumnName.ToLower() == "telefono")
                        {
                            if (row[column.ColumnName].ToString() == "")
                            { XtraMessageBox.Show("La columna telefono no puede estar vacía", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            InfoCampos.Add("telefono", $"'{row[column.ColumnName]}'");
                        }
                        if (column.ColumnName.ToLower() == "ingreso")
                        {
                            if (row[column.ColumnName].ToString() == "")
                            { XtraMessageBox.Show("La columna ingreso no puede estar vacía", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (int.TryParse(row[column.ColumnName].ToString(), out number))
                            { XtraMessageBox.Show("Valor no válido para columna ingreso", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (DateTime.TryParse(row[column.ColumnName].ToString(), out fecha) == false)
                            { XtraMessageBox.Show("Por favor ingresa una fecha valida para columna ingreso");return false; }

                            //GUARDAR TEMPORALMENTE FECHA
                            fechaIngreso = Convert.ToDateTime(row[column.ColumnName]);

                            ////SI LA FECHA DE TERMINO ES DISTINTA A LA FECHA DE HOY QUIERE DECIR QUE ESTÁ SETEADA
                            if (fechaTermino != DateTime.Now.Date)
                            {
                                if (fechaIngreso > fechaTermino)
                                {
                                    XtraMessageBox.Show($"Por favor verifica que la fecha de ingreso no sea mayor que la fecha de termino para contrato {ContratoPersona}", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                                if (fechaIngreso > fnSistema.UltimoDiaMes(Calculo.PeriodoObservado))
                                { XtraMessageBox.Show("Por favor verifica que la fecha de ingreso de contrato no sea mayor al periodo en evaluación", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }


                                //if (GetFechaIngreso(ContratoPersona) > fechaTermino)
                                //{ XtraMessageBox.Show($"Por favor verifica que la fecha de ingreso no sea mayor que la fecha de termino para contrato {ContratoPersona}", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }
                            }

                            InfoCampos.Add("ingreso", $"'{Convert.ToDateTime(row[column.ColumnName]).ToString("yyyy-MM-dd")}'");
                        }
                        if (column.ColumnName.ToLower() == "salida")
                        {
                            if (row[column.ColumnName].ToString() == "")
                            { XtraMessageBox.Show("La columna salida no puede estar vacía", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (int.TryParse(row[column.ColumnName].ToString(), out number))
                            { XtraMessageBox.Show("Valor no válido para columna salida", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (DateTime.TryParse(row[column.ColumnName].ToString(), out fecha) == false)
                            { XtraMessageBox.Show("Por favor ingresa una fecha valida para columna salida"); return false; }

                            fechaTermino = Convert.ToDateTime(row[column.ColumnName]);

                            //SI FECHA DE INGRESO ES DISTINTA A LA FECHA DE HOY ES PORQUE ESTÁ SETEADA.
                            if (fechaIngreso != DateTime.Now.Date)
                            {
                                if (fechaTermino < fechaIngreso)
                                { XtraMessageBox.Show($"Por favor verifica que la fecha de termino no sea menor a la fecha de ingreso para contrato {ContratoPersona}", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                                if (fechaTermino < fnSistema.PrimerDiaMes(Calculo.PeriodoObservado))
                                { XtraMessageBox.Show("Por favor verifica que la fecha de termino de contrato sea menor o igual al periodo en evaluación", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                                //if (GetFechaSalida(ContratoPersona) <= fechaIngreso)
                                //{ XtraMessageBox.Show($"Por favor verifica que la fecha de termino no sea menor a la fecha de ingreso para contrato {ContratoPersona}", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }
                            }

                            InfoCampos.Add("salida", $"'{Convert.ToDateTime(row[column.ColumnName]).ToString("yyyy-MM-dd")}'");
                        }
                        if (column.ColumnName.ToLower() == "tipocontrato")
                        {
                            if (row[column.ColumnName].ToString() == "")
                            { XtraMessageBox.Show("La columna tipocontrato no puede estar vacía", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (fnSistema.IsNumeric(row[column.ColumnName].ToString()) == false)
                            { XtraMessageBox.Show("Por favor ingresa un valor valido para columna tipocontrato", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (GetTipocontrato(Convert.ToInt32(row[column.ColumnName])) == false)
                            { XtraMessageBox.Show($"valor '{row[column.ColumnName]}' no valido para columna tipocontrato", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            //SI ES VALIDO CAMBIAMOS VALOR
                            //row[column.ColumnName] = GetTipocontrato(row[column.ColumnName].ToString());

                            InfoCampos.Add("tipocontrato", Convert.ToInt32(row[column.ColumnName]));
                        }
                        if (column.ColumnName.ToLower() == "regimensalario")
                        {
                            if (row[column.ColumnName].ToString() == "")
                            { XtraMessageBox.Show("La columna regimensalario no puede estar vacia", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (fnSistema.IsNumeric(row[column.ColumnName].ToString()) == false)
                            { XtraMessageBox.Show("Por favor ingresa un valor valido para columna regimensalario", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (GetregimenSalario(row[column.ColumnName].ToString()) == -1)
                            { XtraMessageBox.Show($"Valor {row[column.ColumnName]} no valdio para columna regimensalario", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            //si es valido cambiamos valor
                            row[column.ColumnName] = GetregimenSalario(row[column.ColumnName].ToString());

                            InfoCampos.Add("regimensalario", GetregimenSalario(row[column.ColumnName].ToString()));
                        }
                        if (column.ColumnName.ToLower() == "jubilado")
                        {
                            if (row[column.ColumnName].ToString() == "")
                            { XtraMessageBox.Show("La columna jubilado no puede estar vacía", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (fnSistema.IsNumeric(row[column.ColumnName].ToString()) == false)
                            { XtraMessageBox.Show("Por favor ingresa un valor valido para columna jubilado", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (GetJubilado(Convert.ToInt32(row[column.ColumnName])) == false)
                            { XtraMessageBox.Show($"valor {row[column.ColumnName]} no válido para columna jubilado", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            //SI ES VALIDO CAMBIAMOS VALOR A COLUMNA
                            //row[column.ColumnName] = GetJubilado(row[column.ColumnName].ToString());

                            InfoCampos.Add("jubilado", Convert.ToInt32(row[column.ColumnName]));

                        }
                        if (column.ColumnName.ToLower() == "regimen")
                        {
                            if (row[column.ColumnName].ToString() == "")
                            { XtraMessageBox.Show("La columna regimen no puede estar vacía", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (fnSistema.IsNumeric(row[column.ColumnName].ToString()) == false)
                            { XtraMessageBox.Show("Por favor ingresa un valor valido para columna regimen", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (GetcodFromField(Convert.ToInt32(row[column.ColumnName]), "regimen") == false)
                            { XtraMessageBox.Show($"Valor '{row[column.ColumnName]}' no valido para columna regimen", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            //SI ES VALIDO CAMBIAMOS VALOR
                            //row[column.ColumnName] = GetcodFromField(row[column.ColumnName].ToString(), "regimen");

                            //GUARDAMOS TEMPORALMENTE EL REGIMEN
                            regimen = Convert.ToInt32(row[column.ColumnName]);

                            InfoCampos.Add("regimen", Convert.ToInt32(row[column.ColumnName]));
                        }
                        if (column.ColumnName.ToLower() == "afp")
                        {
                            if (row[column.ColumnName].ToString() == "")
                            { XtraMessageBox.Show("La columna afp no puede estar vacía", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (fnSistema.IsNumeric(row[column.ColumnName].ToString()) == false)
                            { XtraMessageBox.Show("Por favor ingresa un valor valido para la columna afp", "informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (GetcodFromField(Convert.ToInt32(row[column.ColumnName]), "afp") == false)
                            { XtraMessageBox.Show($"valor {row[column.ColumnName]} no valido para columna afp", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            //SOLO APLICA AFP SI REGIMEN ES --> 1
                            //if (regimen != 0)
                            //{
                            //    if (regimen == 1)
                            //        row[column.ColumnName] = GetcodFromField(row[column.ColumnName].ToString(), "afp");
                            //    else
                            //        row[column.ColumnName] = 0;
                            //}
                            //else
                            //    row[column.ColumnName] = GetcodFromField(row[column.ColumnName].ToString(), "afp");                      

                            InfoCampos.Add("afp", Convert.ToInt32(row[column.ColumnName]));
                        }
                        if (column.ColumnName.ToLower() == "salud")
                        {
                            if (row[column.ColumnName].ToString() == "")
                            { XtraMessageBox.Show("La columna salud no puede estar vacía", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (fnSistema.IsNumeric(row[column.ColumnName].ToString()) == false)
                            { XtraMessageBox.Show("Por favor ingresa un valor valido para la columna salud", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (GetcodFromField(Convert.ToInt32(row[column.ColumnName]), "isapre") == false )
                            { XtraMessageBox.Show($"valor {row[column.ColumnName]} no valido para la columna salud", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            //SOLO APLICA SI REGIMEN ES --> 1, 2, 4, 5
                            //if (regimen != 0)
                            //{
                            //    if (regimen == 1 || regimen == 2 || regimen == 4 || regimen == 5)
                            //        row[column.ColumnName] = GetcodFromField(row[column.ColumnName].ToString(), "isapre");
                            //    else
                            //        row[column.ColumnName] = 0;
                            //}else
                            //    //SI ES VALIDO CAMBIAMOS VALOR
                            //    row[column.ColumnName] = GetcodFromField(row[column.ColumnName].ToString(), "isapre");

                            InfoCampos.Add("salud", Convert.ToInt32(row[column.ColumnName]));
                        }
                        if (column.ColumnName.ToLower() == "cajaprevision")
                        {
                            if (row[column.ColumnName].ToString() == "")
                            { XtraMessageBox.Show("La columna cajaprevision no puede estar vacía", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (fnSistema.IsNumeric(row[column.ColumnName].ToString()) == false)
                            { XtraMessageBox.Show("Por favor ingresa un valor valido para la columna cajaprevision", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (GetcodFromField(Convert.ToInt32(row[column.ColumnName]), "cajaprevision"))
                            { XtraMessageBox.Show($"Valor {row[column.ColumnName]} no valido para cajaprevision", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            //SOLO APLICA SI REGIMEN ES --> 3
                            //if (regimen != 0)
                            //{
                            //    if (regimen == 3)
                            //        row[column.ColumnName] = GetcodFromField(row[column.ColumnName].ToString(), "cajaprevision");
                            //    else
                            //        row[column.ColumnName] = 0;
                            //}else
                            //    //SI ES VALIDO GUARDAMOS VALOR
                            //    row[column.ColumnName] = GetcodFromField(row[column.ColumnName].ToString(), "cajaprevision");

                            InfoCampos.Add("cajaprevision", Convert.ToInt32(row[column.ColumnName]));
                            
                        }
                        if (column.ColumnName.ToLower() == "fechasegces")
                        {
                            if (row[column.ColumnName].ToString() == "")
                            { XtraMessageBox.Show("La columna fechasegces no puede estar vacía", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (DateTime.TryParse(row[column.ColumnName].ToString(), out fecha) == false)
                            { XtraMessageBox.Show("Por favor ingresa una fcha valida para columna fechasegces", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }                            

                            if (fechaIngreso != DateTime.Now.Date && fechaTermino != DateTime.Now.Date)
                            {
                                if (fechaSeguro < fechaIngreso)
                                { XtraMessageBox.Show("Por favor verifica que la fecha del seguro de cesantia esté dentro de las fechas de contrato", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);  return false; }

                                if (fechaSeguro > fechaTermino)
                                { XtraMessageBox.Show("Por favor verifica que la fecha de seguro cesantia esté dentro de las fechas de contrato", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); }
                            }

                            InfoCampos.Add("fechasegces", $"{Convert.ToDateTime(row[column.ColumnName]).ToString("yyyy-MM-dd")}");
                        }
                        if (column.ColumnName.ToLower() == "tramo")
                        {                            
                            if (row[column.ColumnName].ToString() == "")
                            { XtraMessageBox.Show("La columna tramo no puede estar vacía", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (int.TryParse(row[column.ColumnName].ToString(), out number) == false)
                            { XtraMessageBox.Show("Por favor ingresa un valor valido para columna tramo", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (int.TryParse(row[column.ColumnName].ToString(), out number) && (number < 1 || number > 4))
                            { XtraMessageBox.Show($"valor {row[column.ColumnName]} no valido para columna tramo", "informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            InfoCampos.Add("tramo", Convert.ToInt32(row[column.ColumnName]));
                        }
                        if (column.ColumnName.ToLower() == "formapago")
                        {
                            if (row[column.ColumnName].ToString() == "")
                            { XtraMessageBox.Show("La columna formapago no puede estar vacía", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (fnSistema.IsNumeric(row[column.ColumnName].ToString()) == false)
                            { XtraMessageBox.Show("Por favor ingresa un valor valido para la columna formapago", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (GetcodFromField(Convert.ToInt32(row[column.ColumnName]), "formapago") == false)
                            { XtraMessageBox.Show($"valor {row[column.ColumnName]} no es valido para columna formapago", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            //CAMBIAMOS VALOR
                            //row[column.ColumnName] = GetcodFromField(row[column.ColumnName].ToString(), "formapago");

                            InfoCampos.Add("formapago", Convert.ToInt32(row[column.ColumnName]));
                        }
                        if (column.ColumnName.ToLower() == "banco")
                        {
                            if (row[column.ColumnName].ToString() == "")
                            { XtraMessageBox.Show("La columna banco no puede estar vacía", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (fnSistema.IsNumeric(row[column.ColumnName].ToString()) == false)
                            { XtraMessageBox.Show("Por favor ingresa un valor valido para la columna banco", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (GetcodFromField(Convert.ToInt32(row[column.ColumnName]), "banco") == false)
                            { XtraMessageBox.Show($"valor {row[column.ColumnName]} no valido para columna banco", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            //SI ES VALIDO CAMBIAMOS
                            //row[column.ColumnName] = GetcodFromField(row[column.ColumnName].ToString(), "banco");

                            InfoCampos.Add("banco", Convert.ToInt32(row[column.ColumnName]));
                        }
                        if (column.ColumnName.ToLower() == "cuenta")
                        {
                            if (row[column.ColumnName].ToString() == "")
                            { XtraMessageBox.Show("La columna cuenta no puede estar vacía", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            //if (int.TryParse(row[column.ColumnName].ToString(), out number) == false)
                            //{ XtraMessageBox.Show("Por favor ingresa un valor valido para la columna cuenta", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            InfoCampos.Add("cuenta", row[column.ColumnName].ToString());
                        }
                        if (column.ColumnName.ToLower() == "fechavacacion")
                        {
                            if (row[column.ColumnName].ToString() == "")
                            { XtraMessageBox.Show("La columna fechavacacion no puede estar vacía", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (DateTime.TryParse(row[column.ColumnName].ToString(), out fecha) == false)
                            { XtraMessageBox.Show("Por favor ingresa una fecha valida para columna fechavacacion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            InfoCampos.Add("fechavacacion", $"{Convert.ToDateTime(row[column.ColumnName]).ToString("yyyy-MM-dd")}");
                        }
                        if (column.ColumnName.ToLower() == "fechaprogresivo")
                        {
                            if (row[column.ColumnName].ToString() == "")
                            { XtraMessageBox.Show("La columna fechaprogresivo no puede estar vacía", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (DateTime.TryParse(row[column.ColumnName].ToString(), out fecha) == false)
                            { XtraMessageBox.Show("Por favor ingresa una fecha valida para columna fechaprogresivo", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            InfoCampos.Add("fechaprogresivo", $"{Convert.ToDateTime(row[column.ColumnName]).ToString("yyyy-MM-dd")}");
                        }
                        if (column.ColumnName.ToLower() == "anosprogresivo")
                        {
                            if (row[column.ColumnName].ToString() == "")
                            { XtraMessageBox.Show("La columna anosprogresivo no puede estar vacía", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (int.TryParse(row[column.ColumnName].ToString(), out number) == false)
                            { XtraMessageBox.Show("Por favor ingresa un valor valido para la columna anosprogresivo", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);return false; }

                            InfoCampos.Add("anosprogresivos", Convert.ToDouble(row[column.ColumnName]));
                        }
                        if (column.ColumnName.ToLower() == "tipocuenta")
                        {
                            if (row[column.ColumnName].ToString() == "")
                            { XtraMessageBox.Show("La columna tipocuenta no puede estar vacía", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (fnSistema.IsNumeric(row[column.ColumnName].ToString()) == false)
                            { XtraMessageBox.Show("Por favor ingresa un valor valido para la columna tipocuenta", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (GetcodFromField(Convert.ToInt32(row[column.ColumnName]), "tipocuenta") == false)
                            { XtraMessageBox.Show($"Valor {row[column.ColumnName]} no valido para columna tipocuenta", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            //SI ES VALIDO CAMBIAMOS VALOR
                            //row[column.ColumnName] = GetcodFromField(row[column.ColumnName].ToString(), "tipocuenta");

                            InfoCampos.Add("tipocuenta", Convert.ToInt32(row[column.ColumnName]));
                        }
                        if (column.ColumnName.ToLower() == "clase")
                        {
                            if (row[column.ColumnName].ToString() == "")
                            { XtraMessageBox.Show("La columna clase no puede estar vacía", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (fnSistema.IsNumeric(row[column.ColumnName].ToString()) == false)
                            { XtraMessageBox.Show("Por favor ingresa un valor valido para columna clase", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (GetcodClase(Convert.ToInt32(row[column.ColumnName])) == false)
                            { XtraMessageBox.Show($"Valor {row[column.ColumnName]} no valido para columna clase", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            //SI E VALOR ES VALIDO CAMBIAMOS EL VALOR
                            //row[column.ColumnName] = GetcodClase(row[column.ColumnName].ToString());

                            InfoCampos.Add("clase", Convert.ToInt32(row[column.ColumnName]));

                        }
                        if (column.ColumnName.ToLower() == "causal")
                        {
                            if (row[column.ColumnName].ToString() == "")
                            { XtraMessageBox.Show("La columna causal no puede estar vacía", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (fnSistema.IsNumeric(row[column.ColumnName].ToString()) == false)
                            { XtraMessageBox.Show("Por favor ingresa un valor valido para columna causal", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (GetcodCausal(Convert.ToInt32(row[column.ColumnName])) == false)
                            { XtraMessageBox.Show($"Valor {row[column.ColumnName]} no valido para columna causal", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            //SI EL VALOR ES VALIDO CAMBIAMOS
                            //row[column.ColumnName] = GetcodCausal(row[column.ColumnName].ToString());

                            InfoCampos.Add("causal", row[column.ColumnName].ToString());
                        }
                        if (column.ColumnName.ToLower() == "sexo")
                        {
                            if (row[column.ColumnName].ToString() == "")
                            { XtraMessageBox.Show("La columna sexo no puede estar vacía", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (fnSistema.IsNumeric(row[column.ColumnName].ToString()) == false)
                            { XtraMessageBox.Show("Por favor ingresa un valor valido para la columna sexo", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (GetcodSexo(row[column.ColumnName].ToString()) == -1)
                            { XtraMessageBox.Show($"valor {row[column.ColumnName]} no valido para columna sexo", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            //SI EL VALOR ES VALIDO CAMBIAMOS VALOR
                            row[column.ColumnName] = GetcodSexo(row[column.ColumnName].ToString());

                            InfoCampos.Add("sexo", Convert.ToInt32(row[column.ColumnName]));
                        }
                        if (column.ColumnName.ToLower() == "fun")
                        {
                            if (row[column.ColumnName].ToString() == "")
                            { XtraMessageBox.Show("La columna fun no puede estar vacía", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (int.TryParse(row[column.ColumnName].ToString(), out number) == false)
                            { XtraMessageBox.Show("Por favor ingresa un valor valido para la columna fun", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            InfoCampos.Add("fun", row[column.ColumnName].ToString());
                        }
                        if (column.ColumnName.ToLower() == "cargo")
                        {
                            if (row[column.ColumnName].ToString() == "")
                            { XtraMessageBox.Show("La columna cargo no puede estar vacía", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (int.TryParse(row[column.ColumnName].ToString(), out number))
                            { XtraMessageBox.Show("Por favor ingresa un valor valido para la columna cargo", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (GetcodFromField(Convert.ToInt32(row[column.ColumnName]), "cargo") == false)
                            { XtraMessageBox.Show($"valor '{row[column.ColumnName]}' no es valido para la columna cargo", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            //SI ES VALIDO GUARDAMOS EL VALOR CORRESPONDIENTE
                            //row[column.ColumnName] = GetcodFromField(row[column.ColumnName].ToString(), "cargo");

                            InfoCampos.Add("cargo",Convert.ToInt32(row[column.ColumnName]));
                        }
                        if (column.ColumnName.ToLower() == "escolaridad")
                        {
                            Escolaridad esco = new Escolaridad();

                            if (row[column.ColumnName].ToString() == "")
                            { XtraMessageBox.Show("Por favor ingresa un código válido en columna escolaridad", "Información", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (int.TryParse(row[column.ColumnName].ToString(), out number) == false)
                            { XtraMessageBox.Show("Por favor ingresa un código válido en columna escolaridad", "información", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (esco.ExisteCodigo(Convert.ToInt32(row[column.ColumnName])) == false)
                            { XtraMessageBox.Show($"No existe escolaridad con código {row[column.ColumnName]}", "Información", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            InfoCampos.Add("esco", Convert.ToInt32(row[column.ColumnName]));
                        }
                        if (column.ColumnName.ToLower() == "horario")
                        {
                            Horario hor = new Horario();

                            if (row[column.ColumnName].ToString() == "")
                            { XtraMessageBox.Show("Por favor ingresa un código válido en columna horario", "Información", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (int.TryParse(row[column.ColumnName].ToString(), out number) == false)
                            { XtraMessageBox.Show("Por favor ingresa un código válido en columna horario", "información", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (hor.ExisteRegistro(Convert.ToInt32(row[column.ColumnName])) == false)
                            { XtraMessageBox.Show($"No existe horario con código {row[column.ColumnName]}", "Información", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            InfoCampos.Add("horario", Convert.ToInt32(row[column.ColumnName]));
                        }
                        if (column.ColumnName.ToLower() == "talla")
                        {
                            if(row[column.ColumnName].ToString().Length != 0)
                                InfoCampos.Add("talla", $"'{row[column.ColumnName].ToString()}'");
                        }
                        if (column.ColumnName.ToLower() == "calzado")
                        {
                            if (row[column.ColumnName].ToString().Length != 0)
                                InfoCampos.Add("calzado", $"'{row[column.ColumnName].ToString()}'");
                        }
                        if (column.ColumnName.ToLower() == "nomemer")
                        {
                            if (row[column.ColumnName].ToString().Length != 0)
                                InfoCampos.Add("nomemer", $"'{row[column.ColumnName].ToString()}'");
                        }
                        if (column.ColumnName.ToLower() == "numemer")
                        {
                            if (row[column.ColumnName].ToString().Length != 0)
                                InfoCampos.Add("numemer", $"'{row[column.ColumnName].ToString()}'");
                        }
                        if (column.ColumnName.ToLower() == "jornada")
                        {
                            if (row[column.ColumnName].ToString() == "")
                            { XtraMessageBox.Show("Por favor ingresa un código de jornada laboral válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (fnSistema.IsNumeric(row[column.ColumnName].ToString()) == false)
                            { XtraMessageBox.Show("Por favor ingresa un código válido para jornada laboral", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (fnSistema.ExisteJornada(Convert.ToInt32(row[column.ColumnName])) == false)
                            { XtraMessageBox.Show($"Código {row[column.ColumnName].ToString()} no es un código de jornada laboral válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            InfoCampos.Add("jornada", Convert.ToInt16(row[column.ColumnName]));
                        }

                        if (column.ColumnName.ToLower() == "comuna")
                        {
                            if (row[column.ColumnName].ToString() == "")
                            { XtraMessageBox.Show("Por favor ingresa un código de comuna válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (fnSistema.IsNumeric(row[column.ColumnName].ToString()) == false)
                            { XtraMessageBox.Show("Por favor ingresa un código válido para comuna", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (fnSistema.ExisteComuna(Convert.ToInt32(row[column.ColumnName])) == false)
                            { XtraMessageBox.Show($"Código {row[column.ColumnName].ToString()} no es un código de comuna válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            InfoCampos.Add("comuna", Convert.ToInt16(row[column.ColumnName]));
                        }


                        if (column.ColumnName.ToLower() == "suslab")
                        {
                            if (row[column.ColumnName].ToString() == "")
                            { XtraMessageBox.Show("Por favor ingresa un código de suspension laboral válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (fnSistema.IsNumeric(row[column.ColumnName].ToString()) == false)
                            { XtraMessageBox.Show("Por favor ingresa un código válido para campo suslab", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            if (Convert.ToInt32(row[column.ColumnName].ToString()) != 13 && Convert.ToInt32(row[column.ColumnName].ToString()) !=14 && Convert.ToInt32(row[column.ColumnName].ToString()) != 15 && Convert.ToInt32(row[column.ColumnName].ToString()) != 0)
                            { XtraMessageBox.Show($"Código {row[column.ColumnName].ToString()} no es un código válido para campo suslab", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                            InfoCampos.Add("suslab", Convert.ToInt32(row[column.ColumnName]));
                        }
                    }

                    //GENERAMOS CADENA PARA UPDATE Y LA GREGAMOS A LISTA
                    queryUpdate.Add(GeneraCadena(InfoCampos, ContratoPersona));

                    //Ir guardando los contratos en una lista

                    InfoCampos.Clear();
                }

                valida = true;                
            }

            return valida;
        }

        //GENERA CADENA PARA UPDATE
        private string GeneraCadena(Hashtable pdata, string pContrato)
        {
            string cadena = "UPDATE trabajador SET ";
            if (pdata.Count == 0) return null;
            //RECORREMOS HASHTABLE
            foreach (DictionaryEntry field in pdata)
            {
                if(field.Key.ToString() != "contrato")
                    cadena = cadena + $"{field.Key}={field.Value},";
            }

            cadena = cadena.Substring(0, cadena.Length - 1);
            cadena = cadena + $" WHERE contrato='{pContrato}' AND anomes={Calculo.PeriodoObservado}";

            return cadena;
        }

        //VALIDAR QUE NO HAYAN CONTRASTOS REPETIDOS
        private bool ContratosRepetidos(DataTable pData)
        {
            bool repetido = false;
            List<string> contratos = new List<string>();
            int count = 0;

            if (pData.Rows.Count>0)
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

        //ACTUALIZACION INFORMACION
        private bool UpdateInformacion(List<string> pLista)
        {
            bool transaccioncorrecta = false;
            SqlCommand cmd;
            SqlTransaction tr;
            int count = 0;

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    tr = fnSistema.sqlConn.BeginTransaction();
                    //RECORREMOS LISTA
                    foreach (var query in pLista)
                    {                        
                        try
                        {
                            using (cmd = new SqlCommand(query, fnSistema.sqlConn))
                            {
                                //AGREGAMOS A TRANSACCION
                                cmd.Transaction = tr;

                                //EJECUTAMOS CONSULTA
                                cmd.ExecuteNonQuery();

                                cmd.Parameters.Clear();
                                cmd.Dispose();
                                count++;
                            }

                            if (count == pLista.Count)
                            {
                                //COMIT
                                tr.Commit();
                                transaccioncorrecta = true;
                                fnSistema.sqlConn.Close();
                            }
                        }
                        catch (SqlException ex)
                        {
                            XtraMessageBox.Show(ex.Message);
                            //HACEMOS ROLL BACK
                            try
                            {
                                tr.Rollback();
                                transaccioncorrecta = false;
                            }
                            catch (Exception ex1)
                            {
                                XtraMessageBox.Show(ex1.Message);
                            }                            
                        }                        
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }


            return transaccioncorrecta;
        }

        #endregion

        private void btnImportar_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            List<string> columnasValidas = new List<string>();
            columnasValidas = ListaCamposValidos();
            string extension = "";

            OpenFileDialog dialogo = new OpenFileDialog();
            //dialogo.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
            dialogo.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm|CSV file (*.csv)|*.csv";

            DataTable data = new DataTable();
            if (dialogo.ShowDialog() == DialogResult.OK)
            {
                if (FileExcel.ExcelAbierto(dialogo.FileName)) { XtraMessageBox.Show("Por favor cierre el archivo antes de continuar", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                txtRuta.Text = dialogo.FileName;

                //OBTENEMOS LA EXTENSION DEL ARCHIVO
                extension = Path.GetExtension(dialogo.FileName);

                if (extension == ".xls" || extension == ".xlsx")
                    data = FileExcel.ReadExcelDev(dialogo.FileName);
                else
                    data = FileExcel.ReadCsv(dialogo.FileName);

                if (data != null)
                {
                    //VERIFICAMOS QUE LAS COLUMNAS SEAN VALIDAS
                    if (ColumnasValidas(columnasValidas, data) == true) { btnCargaInformacion.Enabled = false; lblResult.Visible = true; lblResult.Text = "Archivo con errores."; return; }

                    //VALIDA QUE LAS FILAS TENGAN LOS DATOS CORRECTOS
                    if (ValidaInformacion(data, out CadenasUpdate) == false){ btnCargaInformacion.Enabled = false; lblResult.Visible = true; lblResult.Text = "Archivo con errores."; return; }

                    if (ContratosRepetidos(data)) { btnCargaInformacion.Enabled = false; lblResult.Visible = true; lblResult.Text = "Archivo con errores."; return; }

                    btnCargaInformacion.Enabled = true;
                    lblResult.Visible = true;
                    lblResult.Text = "Ok.";
                }
                else
                {
                    XtraMessageBox.Show("No se encontró informacion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnCargaInformacion.Enabled = false;
                    lblResult.Visible = true;
                    lblResult.Text = "Archivo con errores.";
                }
            }
        }
    }
}