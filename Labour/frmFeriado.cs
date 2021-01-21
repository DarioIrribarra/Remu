using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using DevExpress.XtraEditors;
using System.Collections;
using System.Runtime.InteropServices;
using DevExpress.XtraEditors.Calendar;
using DevExpress.Utils;

namespace Labour
{
    public partial class frmFeriado : DevExpress.XtraEditors.XtraForm
    {
        [DllImport("user32")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32")]
        static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        const int MF_BYCOMMAND = 0;
        const int MF_DISABLED = 2;
        const int SC_CLOSED = 0xF060;

        //VARIABLE PARA SABER SI ES ACTUALIZACION O INGRESO
        private bool Updated = false;

        //PARA EL BOTON NUEVO
        private bool cancela = false;

        public frmFeriado()
        {
            InitializeComponent();
        }

        private void frmFeriado_Load(object sender, EventArgs e)
        {
            var sm = GetSystemMenu(Handle, false);
            EnableMenuItem(sm, SC_CLOSED, MF_BYCOMMAND | MF_DISABLED);

            DefaultProperties();
            calendarioFeriados.ToolTipController = toolTipController1;

            string feriados = "";
            feriados = "SELECT fecha, descripcion FROM feriado ORDER BY fecha DESC";
            fnSistema.spllenaGridView(gridFeriado, feriados);
            fnSistema.spOpcionesGrilla(viewFeriado);
            ColumnasGrilla();
            CargarCampos(0);
            SeleccionarFechaAnio();
        }

        #region "MANEJO DE DATOS"
        //INGRESAR UN NUEVO FERIADO
        private void IngresarFeriado(DateEdit Fecha, TextEdit Descripcion)
        {
            string sql = "INSERT INTO FERIADO(fecha, descripcion) VALUES (@fecha, @descripcion)";
            SqlCommand cmd;
            int res = 0;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@fecha", Convert.ToDateTime(Fecha.EditValue)));
                        cmd.Parameters.Add(new SqlParameter("@descripcion", Descripcion.Text));

                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            XtraMessageBox.Show("Feriado ingresado correctamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            //GUARDAMOS EVENTO EN LOG
                            logRegistro log = new logRegistro(User.getUser(), "SE INGRESA NUEVO FERIADO CON FECHA " + Convert.ToDateTime(Fecha.EditValue), "FERIADO", "0", Convert.ToDateTime(Fecha.EditValue).ToShortDateString(), "INGRESAR");
                            log.Log();

                            //RECARGAMOS GRILLA
                            string grilla = "SELECT fecha, descripcion FROM feriado order by fecha";
                            fnSistema.spllenaGridView(gridFeriado, grilla);

                            CargarCampos();
                            btnNuevo.Text = "Nuevo";
                            btnNuevo.ToolTip = "Nuevo Registro";
                            cancela = false;

                        }
                        else
                        {
                            XtraMessageBox.Show("Error al ingresar registro", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        //MODIFICAR REGISTRO FERIADO
        private void ModificarFeriado(DateEdit Fecha, TextEdit Descripcion, DateTime FechaBd)
        {
            string sql = "UPDATE FERIADO SET fecha=@fecha, descripcion=@descripcion WHERE fecha=@fechabd";
            SqlCommand cmd;
            int res = 0, position = 0;

            //TABLA HASH PARA LOG
            Hashtable datosFeriado = new Hashtable();
            datosFeriado = PrecargaFeriado(FechaBd);

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@fecha", Convert.ToDateTime(Fecha.EditValue)));
                        cmd.Parameters.Add(new SqlParameter("@descripcion", Descripcion.Text));
                        cmd.Parameters.Add(new SqlParameter("@fechabd", FechaBd));

                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            XtraMessageBox.Show("Modificacion correcta", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            //SI ES CORRECTO COMPARAMOS SI HAY CAMBIOS
                            ComparaFeriado(datosFeriado, Convert.ToDateTime(Fecha.EditValue), Descripcion.Text);

                            position = viewFeriado.FocusedRowHandle;
                            //RECARGAMOS GRILLA
                            string grilla = "SELECT fecha, descripcion FROM feriado order by fecha";
                            fnSistema.spllenaGridView(gridFeriado, grilla);

                            CargarCampos(position);

                            btnNuevo.Text = "Nuevo";
                            btnNuevo.ToolTip = "Nuevo Registro";
                            cancela = false;

                        }
                        else
                        {
                            XtraMessageBox.Show("Error al intentar modificar", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        //ELIMINAR FERIADO
        private void EliminarFeriado(DateTime Fecha)
        {
            string sql = "DELETE FROM FERIADO WHERE fecha=@fecha";
            SqlCommand cmd;
            int res = 0;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@fecha", Fecha));

                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            XtraMessageBox.Show("Registro Eliminado correctamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            //GUARDAR EVENTO EN LOG
                            logRegistro log = new logRegistro(User.getUser(), "SE ELIMINAR FERIADO CON FECHA " + Fecha, "FERIADO", Fecha.ToShortDateString(), "0", "ELIMINAR");
                            log.Log();

                            //RECARGAMOS GRILLA
                            string grilla = "SELECT fecha, descripcion FROM FERIADO order by fecha";
                            fnSistema.spllenaGridView(gridFeriado, grilla);
                        }
                        else
                        {
                            XtraMessageBox.Show("Error al intentar eliminar registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        //LLENAR COMBO PERIODO
        private void ComboPeriodos(LookUpEdit Combo)
        {
            string sql = "SELECT anomes FROM periodo WHERE anomes<@periodo";
            List<datoCombobox> lista = new List<datoCombobox>();
            SqlCommand cmd;
            SqlDataReader rd;
            int PeriodoEnCurso = 0;
            PeriodoEnCurso = Calculo.PeriodoEvaluar();
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@periodo", PeriodoEnCurso));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //AGREGAR DATOS A LISTA
                                lista.Add(new datoCombobox() { KeyInfo = (int)rd["anomes"], descInfo = (int)rd["anomes"] + ""});
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
       
            //PROPIEDADES COMBOBOX
            Combo.Properties.DataSource = lista.ToList();
            Combo.Properties.ValueMember = "KeyInfo";
            Combo.Properties.DisplayMember = "descInfo";

            Combo.Properties.PopulateColumns();
            //SI OCULTAR KEY ES TRUE NO SE DEBE MOSTRAR LA COLUMNA KEY
            
            Combo.Properties.Columns[0].Visible = false;
           
            Combo.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
            Combo.Properties.AutoSearchColumnIndex = 1;
            Combo.Properties.ShowHeader = false;
        }

        //LIMPIAR CAMPOS
        private void LimpiarCampos()
        {
            dtFecha.Focus();
            txtdescripcion.Text = "";
            dtFecha.EditValue = DateTime.Now.Date;
            cbCopiaPeriodo.Checked = false;
            Updated = false;
            btnEliminar.Enabled = false;
            //txtPeriodoGraba.Text = DateTime.Now.Date.Year + "";
            //txtYearCopia.Text = DateTime.Now.Date.AddYears(-1).Year + "";
            btnEliminar.Enabled = true;
            btnGuardar.Enabled = true;
            cbCopiaPeriodo.Enabled = true;
            txtPeriodoGraba.Text = "";
            txtYearCopia.Text = "";            
        }

        //METODO PARA MANEJAR HABILITACION Y DESHABILITACION DE PANELES
        private void HandlePanel(int opcion)
        {
            //OPCION 1 --> HABILITA PANEL PERIODO
            //OPCION 2 --> HABILITA PANEL INGRESO
            switch (opcion)
            {
                case 1:
                    //HABILITAMOS PANEL CON COMBO PERIODO
                    txtYearCopia.Focus();
                    PanelPeriodo.Enabled = true;
                    txtYearCopia.Enabled = true;

                    //DESHABILITAMOS PANEL DE INGRESO
                    PanelIngreso.Enabled = false;
                    txtdescripcion.Enabled = false;
                    dtFecha.Enabled = false;

                    break;
                case 2:
                    PanelPeriodo.Enabled = false;
                    txtYearCopia.Enabled = false;

                    //HABILITAMOS PANEL PARA INGRESO            
                    txtdescripcion.Enabled = true;
                    dtFecha.Enabled = true;
                    PanelIngreso.Enabled = true;

                    break;
            }
        }

        //COLUMNAS PARA GRILLA
        private void ColumnasGrilla()
        {
            viewFeriado.Columns[0].Caption = "Fecha";
            viewFeriado.Columns[0].Width = 20;

            viewFeriado.Columns[1].Caption = "Descripcion";
        }

        //PROPIEDADES POR DEFECTO
        private void DefaultProperties()
        {
            dtFecha.EditValue = DateTime.Now.Date;
            dtFecha.Properties.ShowClear = false;
            Updated = false;
        }

        //CARGAR CAMPOS
        private void CargarCampos(int? pos = 0)
        {
            if (viewFeriado.RowCount > 0)
            {
                if (pos != 0)
                    viewFeriado.FocusedRowHandle = (int)pos;                

                Updated = true;
                btnEliminar.Enabled = true;
                cbCopiaPeriodo.Checked = false;
                cbCopiaPeriodo.Enabled = false;
                HandlePanel(2);

                dtFecha.EditValue = (DateTime)viewFeriado.GetFocusedDataRow()["fecha"];
                txtdescripcion.Text = (string)viewFeriado.GetFocusedDataRow()["descripcion"];

                //CARGAMOS EN CALENDARIO FECHA
                calendarioFeriados.EditValue = dtFecha.EditValue;

                //SI FECHA ES MENOR AL PERIODO EN EVALUACION NO DEJAMOS QUE SE PUEDA ELIMINAR
                if (PermiteEliminarFeriado((DateTime)viewFeriado.GetFocusedDataRow()["fecha"], Calculo.PeriodoObservado) == false)
                {
                    //DESHABILITAR BOTON ELIMINAR
                    btnEliminar.Enabled = false;
                    btnGuardar.Enabled = false;
                }
                else
                {
                    btnEliminar.Enabled = true;
                    btnGuardar.Enabled = true;
                }                
            }
            else
            {
                LimpiarCampos();
            }

        }

        //VALIDAR QUE LA FECHA INGRESADA NO EXISTA EN BD
        private bool ExisteFeriado(DateTime fecha)
        {
            bool existe = false;
            string sql = "SELECT fecha FROM feriado WHERE fecha=@fecha";
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@fecha", fecha));

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

        //INGRESO MASIVO PARA EL CASO DE QUE SE SELECCIONE COPIA DE PERIODO
        private void IngresoMasivo(int year, int nuevoyear)
        {
            string sql = "INSERT INTO FERIADO(fecha, descripcion) " +
                "SELECT DATEADD(YEAR, @number, fecha), descripcion FROM FERIADO WHERE fecha BETWEEN @inicio AND @termino";

            //PARA OBTENER LA DIFERENCIA DE AÑOS
            int diferencia = 0;
            List<DateTime> fechas = new List<DateTime>();
            DateTime inicio = DateTime.Now.Date;
            DateTime termino = DateTime.Now.Date;

            diferencia = nuevoyear - year;
            inicio = DateTime.Parse("01-01-" + year);
            termino = DateTime.Parse("31-12-" + year);

            SqlCommand cmd;
            int res = 0;            

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@inicio", inicio));
                        cmd.Parameters.Add(new SqlParameter("@termino", termino));
                        cmd.Parameters.Add(new SqlParameter("@number", diferencia));

                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            XtraMessageBox.Show("Ingreso Correcto", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            string grilla = "SELECT fecha, descripcion FROM feriado order by fecha";
                            fnSistema.spllenaGridView(gridFeriado, grilla);

                            logRegistro log = new logRegistro(User.getUser(), "SE INGRESAN REGISTROS MASIVOS PARA AÑO " + inicio.Year, "FERIADO", "0", inicio + " - " + termino, "INGRESAR");
                            log.Log();

                            CargarCampos();
                            btnNuevo.Text = "Nuevo";
                            btnNuevo.ToolTip = "Nuevo Registro";
                            cancela = false;

                        }
                        else
                        {
                            XtraMessageBox.Show("Error al intentar ingresar datos", "Informacion",MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        //VALIDAR QUE EXISTA AÑO QUE SE INTENTA COPIAR
        private bool ExisteYearCopia(int year)
        {
            DateTime inicio = DateTime.Now.Date;
            DateTime termino = DateTime.Now.Date;

            inicio = DateTime.Parse("01-01-" + year);
            termino = DateTime.Parse("31-12-" + year);

            bool existecopia = false;
            string sql = "SELECT FECHA FROM FERIADO WHERE FECHA BETWEEN @inicio AND @termino";
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        cmd.Parameters.Add(new SqlParameter("@inicio", inicio));
                        cmd.Parameters.Add(new SqlParameter("@termino", termino));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //SI TIENE FILAS ES PORQUE HAY REGISTROS PARA ESE AÑO
                            existecopia = true;
                        }
                        else
                        {
                            existecopia = false;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return existecopia;
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            bool yearexiste = false;
            if (keyData == Keys.Tab)
            {
                if (txtYearCopia.ContainsFocus)
                {
                    if (txtYearCopia.Text == "")
                    {
                        XtraMessageBox.Show("Por favor ingresa el año a copiar", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                    else
                    {
                        //VERIFICAR QUE AÑO EXISTA PARA COPIA
                        yearexiste = ExisteYearCopia(Convert.ToInt32(txtYearCopia.Text));
                        if (yearexiste == false) { XtraMessageBox.Show("Año no registra datos", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);txtYearCopia.Focus();return false;}

                    }
                }
                else if (txtPeriodoGraba.ContainsFocus)
                {
                    if (txtPeriodoGraba.Text == "")
                    {
                        XtraMessageBox.Show("Por favor ingresar año", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }                    
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        //VERIFICAR QUE EL PERIOdo QUE SE INTENTAR REGISTRAR YA EXISTA
        private bool ExisteAnio(int Year)
        {
            bool existe = false;
            DateTime inicio = DateTime.Now.Date;
            DateTime termino = DateTime.Now.Date;

            string sql = "SELECT fecha FROM feriado where fecha between @inicio AND @termino";
            SqlCommand cmd;
            SqlDataReader rd;

            inicio = DateTime.Parse("01-01-" + Year);
            termino = DateTime.Parse("31-12-"+ Year);

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@inicio", inicio));
                        cmd.Parameters.Add(new SqlParameter("@termino", termino));

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

        //OBTENER EL ULTIMO YEAR INGRESADO EN FERIADOS
        private int UltimoAnioIngresado()
        {
            string sql = "SELECT Max(fecha) as fecha FROM feriado";
            SqlCommand cmd;
            SqlDataReader rd;
            DateTime fecha = DateTime.Now.Date;
            int year = 0;

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                fecha = (DateTime)rd["fecha"];
                                year = fecha.Year;
                            }
                        }
                        else
                        {
                            year = -1;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
            return year;
        }

        //AÑO SIGUIENTE
        private int NextYear(int pLastYear)
        {
            int Next = 0;
            DateTime fecha = DateTime.Now.Date;
            if (pLastYear != 0)
            {
                try
                {
                    fecha = Convert.ToDateTime($"01-01-{pLastYear}");
                    //SI LA FECHA SE CONVIRTIO CORRECTAMENTE OBTENEMOS EL AÑO SIGUIENTE
                    Next = fecha.AddYears(1).Year;
                }
                catch (FormatException)
                {
                   //
                }                
            }

            return Next;
        }

        //NO DEJAR ELEMINAR FERIADOS QUE SEAN DE PERIODOS ANTERIORES
        private bool PermiteEliminarFeriado(DateTime Fecha, int PeriodoEvaluado)
        {
            bool permite = false;

            DateTime registro = DateTime.Now.Date;
            DateTime PrimerDiaMes = DateTime.Now.Date;
            DateTime UltimoDiaMes = DateTime.Now.Date;

            //OBTENER PRIMER Y ULTIMO DIA DE ACUERDO A PERIODO EN EVALUACION
            PrimerDiaMes = fnSistema.PrimerDiaMes(PeriodoEvaluado);
            UltimoDiaMes = fnSistema.UltimoDiaMes(PeriodoEvaluado);

            //SI FECHA ES MENOR AL PRIMER DIA DEL PERIODO
            //NO DEBEMOS DEJAR QUE SE ELIMINE EL FERIADO PORQUE PUEDE AFECTAR A CALCULO PREVIOS DE VACACIONES...
            if (Fecha < PrimerDiaMes)
            {
                permite = false;
            }
            else
            {
                //SI ES IGUAL O MAYOR PODRIAMOS DEJAR QUE SE ELIMINE
                permite = true;
            }

            return permite;
        }

        private void SeleccionarFechaAnio()
        {
            DateTime fecha = DateTime.Now.Date;
            DateTime PrimerDiaAño = DateTime.Now.Date;
            PrimerDiaAño = new DateTime(DateTime.Now.Date.Year, 1, 1);

            if (viewFeriado.RowCount>0)
            {
                //RECORRER TODOS LOS ELEMENTOS DE LA GRILLA Y ENCONTRAR EL ELEMENTO QUE SEA EL PRIMERO DEL ANIO EN CURSO
                int totalFilas = viewFeriado.RowCount;
                int pos = 0;
                while (pos < totalFilas)
                {                       
                    //OBTENER LA FECHA
                    fecha = (DateTime)viewFeriado.GetFocusedDataRow()["fecha"];
                    
                    if (fecha == PrimerDiaAño)
                    {
                        viewFeriado.FocusedRowHandle = pos;
                        viewFeriado.TopRowIndex = pos;
                        CargarCampos(pos);

                        break;
                    }
                    else
                        viewFeriado.FocusedRowHandle = 0;

                    pos++;
                    viewFeriado.FocusedRowHandle = pos;
                }
            }
        }

        //VER SI HAY CAMBIOS SIN GUARDAR
        private bool CambiosSinGuardar(DateTime pFecha)
        {
            string sql = "SELECT fecha, descripcion FROM feriado WHERE fecha = @pFecha";
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pFecha", pFecha));
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //COMPARAMOS
                                if (dtFecha.DateTime != Convert.ToDateTime(rd["fecha"])) return true;
                                if (txtdescripcion.Text != (string)rd["descripcion"]) return true;                                
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

        //LISTADO DE FECHAS DESDE TABLA FERIADOS
        private List<DateTime> Feriados()
        {
            List<DateTime> Listado = new List<DateTime>();
            string sql = "SELECT fecha FROM feriado";

            SqlDataReader rd;
            SqlCommand cmd;
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
                                //LLENAMOS LISTADO
                                Listado.Add(Convert.ToDateTime(rd["fecha"]));
                            }
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

            return Listado;
        }

        //OBTENER DESCRIPCION DESDE TABLA FERIADO (ACORDE A FECHA)
        private string getDescripcionFecha(DateTime pFecha)
        {
            string desc = "";
            string sql = "SELECT descripcion FROM feriado WHERE fecha=@pFecha";
            SqlCommand cmd;            
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pFecha", pFecha));
                        desc = (string)cmd.ExecuteScalar();

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return desc;
        }

        #region "LOG FERIADO"

        //PRECARGA DATOS HASH
        private Hashtable PrecargaFeriado(DateTime fechaBd)
        {
            Hashtable datos = new Hashtable();
            string sql = "SELECT fecha, descripcion FROM feriado WHERE fecha=@fechabd";
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        cmd.Parameters.Add(new SqlParameter("@fechabd", fechaBd));

                        rd = cmd.ExecuteReader();

                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //CARGAR DATOS HASHTABLE
                                datos.Add("fecha", (DateTime)rd["fecha"]);
                                datos.Add("descripcion", (string)rd["descripcion"]);
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

            return datos;
        }

        //COMPARA VALORES
        private void ComparaFeriado(Hashtable data, DateTime fecha, string descripcion)
        {
            if (data.Count>0)
            {
                if ((DateTime)data["fecha"] != fecha)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE HA CAMBIADO FECHA FERIADO " + (DateTime)data["fecha"], "FERIADO", (DateTime)data["fecha"] + "", fecha.ToShortDateString(), "MODIFICAR");
                    log.Log();
                }
                if ((string)data["descripcion"] != descripcion)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE HA CAMBIADO DESCRIPCION FERIADO " + (DateTime)data["fecha"], "FERIADO", (string)data["descripcion"], descripcion, "MODIFICAR");
                    log.Log();
                }
            }
        }

        #endregion

        #endregion

        private void cbCopiaPeriodo_CheckedChanged(object sender, EventArgs e)
        {
            if (cbCopiaPeriodo.Checked)
            {
                //ULTIMO AÑO INGRESADO 
                int LastYear = UltimoAnioIngresado();         
                txtYearCopia.Text = LastYear.ToString();
                //OBTENEMOS EL AÑO SIGUIENTE
                txtPeriodoGraba.Text = NextYear(LastYear).ToString();
                HandlePanel(1);     
            }
            else
            {
                txtYearCopia.Text = "";
                txtPeriodoGraba.Text = "";
                HandlePanel(2);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;


            bool existeFeriado = false;
            int UltimoYear = 0;
            DateTime fecha = DateTime.Now.Date;

            //SI ESTA SELECCIONADO EL CHECKBOX ASUMIMOS QUE SE QUIERE REALIZAR COPIA DE PERIODO ANTERIOR
            if (cbCopiaPeriodo.Checked)
            {
                UltimoYear = UltimoAnioIngresado();               

                //REALIZAMOS INSERT CON PERIODO NUEVO (COPIA MASIVA)
                if (txtYearCopia.Text == "") { XtraMessageBox.Show("Por favor ingresa un año", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtYearCopia.Focus();return;}
                if (txtPeriodoGraba.Text == "") { XtraMessageBox.Show("Por favor ingresar un año", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtPeriodoGraba.Focus();return;}

                if (ExisteYearCopia(Convert.ToInt32(txtYearCopia.Text)) == false) { XtraMessageBox.Show("Periodo no registra datos", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);txtYearCopia.Focus();return;}
                if (ExisteAnio(Convert.ToInt32(txtPeriodoGraba.Text))) { XtraMessageBox.Show("Ya existen registros para ese año", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);txtPeriodoGraba.Focus();return;}

                //VERIFICAR QUE PERIODO QUE SE INTENTA COPIAR EXISTA REALMENTE EN TABAL FERIADOS
                existeFeriado = ExisteYearCopia(Convert.ToInt32(txtYearCopia.Text));
                if (existeFeriado == false) { XtraMessageBox.Show("Periodo no registra datos", "informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);txtYearCopia.Focus();return;}

                DialogResult Advertencia = XtraMessageBox.Show("¿Estás seguro de ingresar registros para el año " + txtPeriodoGraba.Text + "?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (Advertencia == DialogResult.Yes)
                    IngresoMasivo(Convert.ToInt32(txtYearCopia.Text), Convert.ToInt32(txtPeriodoGraba.Text));
                else {
                    CargarCampos();
                    btnNuevo.Text = "Nuevo";
                    btnNuevo.ToolTip = "Nuevo registro";
                    cancela = false;
                }
            }
            else
            {
                if (txtdescripcion.Text == "") { XtraMessageBox.Show("Por favor ingresa una descripcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);txtdescripcion.Focus();return;}
                if (dtFecha.DateTime == null) { XtraMessageBox.Show("Por favor selecciona una fecha", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);dtFecha.Focus();return;}

                //MANIPULAMOS NORMALMENTE COMO SI FUERA UN INGRESO NORMAL O MODIFICACION NORMAL
                if (Updated)
                {
                    //ACTUALIZACION
                    //OBTENER LA FECHA DEL REGISTRO ACTUAL DESDE GRILLA
                    if (viewFeriado.RowCount > 0)
                    {
                        fecha = (DateTime)viewFeriado.GetFocusedDataRow()["fecha"];
                        //VERIFICAR QUE LA FECHA QUE SE INTENTA MODIFICAR NO EXISTE EN BD
                        ModificarFeriado(dtFecha, txtdescripcion, fecha);
                    }
                    else
                    {
                        XtraMessageBox.Show("Error al intentar actualizar", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                else
                {
                    //VERIFICAR SI LA FECHA QUE SE INTENTA INGRESAR YA EXISTE EN TABLA FERIADOS
                    existeFeriado = ExisteFeriado((DateTime)dtFecha.EditValue);
                    if (existeFeriado)
                    { XtraMessageBox.Show("Ya existe un registro con esa fecha", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);return;}

                    //MODIFICAMOS
                    IngresarFeriado(dtFecha, txtdescripcion);
                }
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            LimpiarCampos();
            if (cancela)
            {
                btnNuevo.Text = "Nuevo";
                btnNuevo.ToolTip = "Nuevo Registro";
                cancela = false;
                CargarCampos();
                cbCopiaPeriodo.Enabled = false;
            }
            else
            {
                btnNuevo.Text = "Cancelar";
                btnNuevo.ToolTip = "Cancelar registro";
                cancela = true;
            }           
        }

        private void txtPeriodoGraba_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtdescripcion_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) || char.IsLetter(e.KeyChar) || char.IsSeparator(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void txtYearCopia_KeyPress(object sender, KeyPressEventArgs e)
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

        private void gridFeriado_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            CargarCampos();

            //RESET BOTON NUEVO
            btnNuevo.Text = "Nuevo";
            btnNuevo.ToolTip = "Nuevo registro";
            cancela = false;
        }

        private void gridFeriado_KeyUp(object sender, KeyEventArgs e)
        {
            CargarCampos();
            //RESET BOTON NUEVO
            btnNuevo.Text = "Nuevo";
            btnNuevo.ToolTip = "Nuevo registro";
            cancela = false;
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            DateTime fecha = DateTime.Now.Date;
            if (viewFeriado.RowCount > 0)
            {
                //OBTENER LA FECHA DESDE GRILLA
                fecha = (DateTime)viewFeriado.GetFocusedDataRow()["fecha"];

                DialogResult pregunta = XtraMessageBox.Show("¿Esta seguro de eliminar el feriado " + fecha.ToShortDateString() + " ?", "Informacion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (pregunta ==DialogResult.Yes)
                {
                    EliminarFeriado(fecha);
                }
            }
            else
            {
                XtraMessageBox.Show("Registro no valido", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void txtYearCopia_KeyDown(object sender, KeyEventArgs e)
        {
            bool existeYear = false;
            if (e.KeyData == Keys.Enter)
            {
                if (txtYearCopia.Text == "" || txtPeriodoGraba.Text == "")
                {
                    XtraMessageBox.Show("Por favor llena los campos en blanco", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else
                {
                    //VERIFICAR QUE EL AÑO A COPIAR REALMENTE EXISTE EN BD
                    existeYear = ExisteYearCopia(Convert.ToInt32(txtYearCopia.Text));
                    if (existeYear == false) { XtraMessageBox.Show("No existe informacion para ese año", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);txtYearCopia.Focus(); return;}                   

                }
            }
        }

        private void txtYearCopia_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void txtPeriodoGraba_KeyDown(object sender, KeyEventArgs e)
        {
            bool existeYear = false;
            if (e.KeyData == Keys.Enter)
            {
                if (txtPeriodoGraba.Text == "")
                {
                    XtraMessageBox.Show("Por favor ingresa un año", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else
                {
                    existeYear = ExisteAnio(Convert.ToInt32(txtPeriodoGraba.Text));
                    if (existeYear)
                    {
                        XtraMessageBox.Show("Ya existen registros para este año", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    btnGuardar.Focus();
                }
            }
        }

        private void txtYearCopia_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtPeriodoGraba_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void dtFecha_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtdescripcion_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            DateTime fecha = DateTime.Now.Date;
            if (viewFeriado.RowCount > 0)
            {
                fecha = Convert.ToDateTime(viewFeriado.GetFocusedDataRow()["fecha"]);
                if (CambiosSinGuardar(fecha))
                {
                    DialogResult adv = XtraMessageBox.Show("Hay cambios sin guardar, ¿Deseas cerrar de todas maneras?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (adv == DialogResult.Yes)
                        Close();
                }
                else
                    Close();
            }
            else
                Close();
        }

        private void calendarioFeriados_CustomDrawDayNumberCell(object sender, DevExpress.XtraEditors.Calendar.CustomDrawDayNumberCellEventArgs e)
        {
            //LISTADO DE FERIADOS
            List<DateTime> Fechas = new List<DateTime>();
            Fechas = Feriados();

            if (Fechas.Count>0)
            {
                foreach (DateTime date in Fechas)
                {
                    if (e.Date == date)
                    {
                        e.Style.ForeColor = Color.White;
                        e.Style.BackColor = Color.Red;
                        e.Style.BorderColor = Color.Gray;
                    }
                }
            }
        }

        private void toolTipController1_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            List<DateTime> ListadoFechas = new List<DateTime>();
            ListadoFechas = Feriados();

            if (ListadoFechas.Count>0)
            {
                if (e.SelectedControl == calendarioFeriados)
                {
                    CalendarHitInfo HitInfo = calendarioFeriados.GetHitInfo(e.ControlMousePosition);
                    if (HitInfo.IsInCell)
                    {
                        //COMPARAMOS FECHA CON LISTADO
                        foreach (DateTime fecha in ListadoFechas)
                        {
                            if (HitInfo.Cell.Date == fecha)
                            {
                                //OBTENER DESCRIPCION DESDE TABLA
                               // HitInfo.Cell.Appearance.BackColor = Color.Red;
                               // HitInfo.Cell.Appearance.ForeColor = Color.Blue;

                                ToolTipControlInfo info = new ToolTipControlInfo(HitInfo.Cell, getDescripcionFecha(fecha));
                                e.Info = info;
                            }
                        }                        
                    }
                }
            }           
        }
    }
}