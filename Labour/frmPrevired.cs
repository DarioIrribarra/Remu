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
using DevExpress.XtraReports.UI;
using System.Collections;

namespace Labour
{
    public partial class frmPrevired : DevExpress.XtraEditors.XtraForm, IConjuntosCondicionales
    {
        #region "INTERFAZ CONJUNTOS CONDICIONALES"
        public void CargarCodigoConjunto(string code)
        {
            this.txtConjunto.Text = code;
        }
        #endregion
        IConjuntosCondicionales opener { set; get; }

        //DIRECTORIO DONDE GUARDAREMOS LOS ARCHIVOS
        private string directory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        //PARA GUARDAR EL FORMATO DE SALIDA
        private string FormatoSalida = "";

        //PARA GUARDAR EL FILTRO DEL USUARIO LOGUEADO
        private string FiltroUsuario = "";

        //USUARIO PUEDE VER FICHAS PRIVADAS
        private bool ShowPrivados { get; set; } = User.ShowPrivadas();

        List<ReportePrevired> PreviredData = new List<ReportePrevired>();

        public frmPrevired()
        {
            InitializeComponent();
        }

        private void frmPrevired_Load(object sender, EventArgs e)
        {
            datoCombobox.spLlenaPeriodos("SELECT anomes FROM parametro ORDER BY anomes DESC", txtComboPeriodo, "anomes", "anomes", true);
            
            FiltroUsuario = User.GetUserFilter();
            cbTodos.Checked = true;
            txtConjunto.Text = "";
            txtConjunto.Enabled = false;
            btnConjunto.Enabled = false;

            if (txtComboPeriodo.Properties.DataSource != null)
                txtComboPeriodo.ItemIndex = 0;
        }

        #region "MANEJO DE DATOS"
        //LISTADO DE CONTRATO Y RUT 
        //MOSTRAMOS TODOS LOS RUT DEL PERIODO SELECCIONADO PERO NO CONSIDERAMOS LOS RUT REPETIDOS 
        //(AUNQUE SEAN CONTRATOS DISTINTOS)
        private List<string> ListadoPersona(int periodo)
        {
            string filtro = "";
            
            if (FiltroUsuario != "0")
            {
                filtro = string.Format("SELECT DISTINCT rut FROM trabajador WHERE status=1 AND anomes={0} AND {1}",
                    periodo, Conjunto.GetCondicionFromCode(FiltroUsuario));
            }
            else
                filtro = string.Format("SELECT DISTINCT rut FROM trabajador WHERE status=1 AND anomes={0}", periodo);

            SqlCommand cmd;
            SqlDataReader rd;
            List<string> lista = new List<string>();

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(filtro, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        //cmd.Parameters.Add(new SqlParameter("@pPeriodo", periodo));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //GARDARMOS EN LISTA
                                lista.Add((string)rd["rut"]);
                            }
                        }
                        else
                            lista = null;
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

            return lista;

        }

        //LISTADO DE EMPLEADOS DE ACUERDO A CONJUNTO SELECCIONADO
        private List<string> ListadoPersonasConjunto(int periodo, string codeConjunto)
        {
            List<string> listado = new List<string>();
            string filtro = "";
           // string sql = "SELECT rut, contrato, nombre FROM trabajador WHERE anomes=@pPeriodo";

            
            if (FiltroUsuario == "0")
                filtro = string.Format("SELECT DISTINCT rut FROM trabajador WHERE status=1 AND anomes={0} AND {1} ",
                    periodo, Conjunto.GetCondicionFromCode(codeConjunto));
            else
                filtro = string.Format("SELECT DISTINCT rut FROM trabajador WHERE status=1 AND anomes={0} AND {1} AND {2}", 
                    periodo, Conjunto.GetCondicionFromCode(codeConjunto), Conjunto.GetCondicionFromCode(FiltroUsuario));

            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(filtro, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //AGREGAMOS A LISTA
                                listado.Add((string)rd["rut"]);
                            }
                        }
                    }
                    rd.Close();
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return listado;
        }
        
        #endregion

        private void btnGenerar_Click(object sender, EventArgs e)
        {

            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            PreviredData.Clear();

            if (cbCsv.Checked == false && cbTxt.Checked == false)
            { XtraMessageBox.Show("Debes seleccionar al menos un formato de salida", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);return; }

            if (cbCsv.Checked && cbTxt.Checked)
            { XtraMessageBox.Show("Solo puedes seleccionar un formato de salida", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);return; }

            List<string> dataResultante =  new List<string>();
            string  pathFile = "", rut = "";
            string type = "";

            if (txtComboPeriodo.Properties.DataSource == null)
            { XtraMessageBox.Show("Por favor ingresa un periodo", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtComboPeriodo.Focus(); return; }

            if (Calculo.PeriodoValido(Convert.ToInt32(txtComboPeriodo.EditValue)) == false)
            { XtraMessageBox.Show("Periodo ingresado no valido", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtComboPeriodo.Focus(); return; }

            //GENERAMOS LISTADO DE EMPLEADOS PARA EL PERIODO SELECCIONADO
            List<string> listado = new List<string>();

            //GUARDAMOS EL TIPO DE SALIDA SELEECIONADA
            type = cbCsv.Checked ? ".csv" : ".txt";
            string today = DateTime.Now.Date.ToString("yyyyMMdd");

            pathFile = directory + @"\previred_"+ today + type;

            if (File.Exists(pathFile) && FileExcel.ExcelAbierto(pathFile))
            { XtraMessageBox.Show("Por favor verificar que el archivo no esté  abierto", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (cbTodos.Checked == false)
            {
                if (txtConjunto.Text == "") { XtraMessageBox.Show("Por favor ingresa un conjunto a evaluar", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                if (Conjunto.ExisteConjunto(txtConjunto.Text) == false) { XtraMessageBox.Show("Parece ser que el conjunto ingresado no existe", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                //BUSCAMOS CON CONDICION INGRESADA
                listado = ListadoPersonasConjunto(Convert.ToInt32(txtComboPeriodo.EditValue), txtConjunto.Text);

                string condicion = "";
                condicion = Conjunto.GetCondicionFromCode(txtConjunto.Text);

                if (listado == null)
                { XtraMessageBox.Show("No se encontro informacion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                if (listado.Count == 0)
                { XtraMessageBox.Show("No se encontro informacion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                splashScreenManager1.ShowWaitForm();
                //GENERAMOS ARCHIVO
                if (Archivo.CrearArchivo(pathFile))
                {
                    //RECORREMOS LISTADO
                    if (listado.Count > 0)
                    {
                        foreach (var rutPersona in listado)
                        {
                            rut = fnSistema.fnDesenmascararRut(rutPersona);
                            Previred prev = new Previred(rut, Convert.ToInt32(txtComboPeriodo.EditValue));
                            prev.Condicion = condicion;

                            dataResultante = prev.GenerarData(PreviredData);

                            if (dataResultante == null)
                            { XtraMessageBox.Show("Error al generar informacion", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }                            

                            //AGREGAR NUEVA LINEA A ARCHIVO
                            //RECORREMOS DATA RESULTANTE
                            foreach (var cadena in dataResultante)
                            {
                                Archivo.EscribirArchivo(pathFile, cadena);
                            }
                        }
                        splashScreenManager1.CloseWaitForm();
                        DialogResult advertencia = XtraMessageBox.Show($"Archivo {pathFile} generado correctamente, ¿desea ver archivo?", "Informacion", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                        if (advertencia == DialogResult.Yes)
                            FileExcel.AbrirExcel(pathFile);

                        btnResumen.Enabled = true;                        
                        btnEditarReporte.Enabled = true;                        
                    }
                    else
                    {
                        XtraMessageBox.Show("No se encontrato informacion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        splashScreenManager1.CloseWaitForm();
                        btnResumen.Enabled = false;
                        btnEditarReporte.Enabled = false;
                    }
                }
                else
                {
                    XtraMessageBox.Show("Archivo no se pudo generar", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    splashScreenManager1.CloseWaitForm();
                    btnResumen.Enabled = false;
                    btnEditarReporte.Enabled = false;
                }                  
            }
            else
            {
                //GENERAMOS LISTADO
                listado = ListadoPersona(Convert.ToInt32(txtComboPeriodo.EditValue));
           

                if(listado == null)
                { XtraMessageBox.Show("No se encontro informacion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                if (listado.Count == 0)
                { XtraMessageBox.Show("No se encontro informacion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                splashScreenManager1.ShowWaitForm();
                //GENERAMOS ARCHIVO
                if (Archivo.CrearArchivo(pathFile))
                {
                    //RECORREMOS Y GENERAMOS CADENA CORRESPONDIENTE...
                    if (listado.Count > 0)
                    {
                        //RECORREMOS LISTADO
                        foreach (var rutPersona in listado)
                        {                            
                            rut = fnSistema.fnDesenmascararRut(rutPersona);
                            Previred prev = new Previred(rut, Convert.ToInt32(txtComboPeriodo.EditValue));
                            

                            dataResultante = prev.GenerarData(PreviredData);
                            if (dataResultante == null)
                            { XtraMessageBox.Show("Error al generar informacion", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }                           

                            //AGREGAR NUEVA LINEA A ARCHIVO
                            //RECORREMOS E INGRESAMOS CADA LINEA AL ARCHIVO
                            foreach (var cadena in dataResultante)
                            {
                                Archivo.EscribirArchivo(pathFile, cadena);
                            }                           
                        }

                        splashScreenManager1.CloseWaitForm();

                        DialogResult advertencia = XtraMessageBox.Show($"Archivo {pathFile} generado correctamente, ¿desea abrir archivo?", "Informacion", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                        if (advertencia == DialogResult.Yes)
                            FileExcel.AbrirExcel(pathFile);

                        //HABILITAMOS BOTON
                        btnResumen.Enabled = true;
                        btnEditarReporte.Enabled = true;
                       
                    }
                    else
                    {
                        XtraMessageBox.Show("No se encontrato informacion", "informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        splashScreenManager1.CloseWaitForm();
                        btnResumen.Enabled = false;
                        btnEditarReporte.Enabled = false;
                    }
                }
                else
                {
                    XtraMessageBox.Show("Error al generar el archivo", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    splashScreenManager1.CloseWaitForm();
                    btnResumen.Enabled = false;
                    btnEditarReporte.Enabled = false;

                }
            }                     
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            Close();
        }

        private void panelControl1_Paint(object sender, PaintEventArgs e)
        {

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

        private void txtConjunto_DoubleClick(object sender, EventArgs e)
        {
            FrmFiltroBusqueda filtro = new FrmFiltroBusqueda(true);
            filtro.opener = this;
            filtro.StartPosition = FormStartPosition.CenterParent;
            filtro.Show();
            
        }

        private void cbConjunto_CheckedChanged(object sender, EventArgs e)
        {
            if (cbTodos.Checked)
            {
                txtConjunto.Text = "";
                txtConjunto.Enabled = false;
                btnConjunto.Enabled = false;
            }
            else
            {
                txtConjunto.Enabled = true;
                txtConjunto.Focus();
                txtConjunto.Text = "";
                btnConjunto.Enabled = true;

            }
        }

        private void btnResumen_Click(object sender, EventArgs e)
        {
            MostrarReporte();
        }

        private void DataSourceResumen(int pPeriodo, XtraReport reporte, string pCodeConjunto)
        {
            string sql = "";

            //sql = GetSqlFinal(pCodeConjunto);
            sql = GetSqlDataSource(pCodeConjunto);
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
                        cmd.Parameters.Add(new SqlParameter("@pPeriodo", pPeriodo));

                        ad.SelectCommand = cmd;                       
                        ad.Fill(ds);
                        ds.DataSetName = "data";

                        if (ds.Tables[0].Rows.Count == 0)
                        { XtraMessageBox.Show("No se encontró informacion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

                        if (ds.Tables[0].Rows.Count>0)
                        {
                            reporte.DataSource = ds.Tables[0];
                            reporte.DataMember = "data";
                        }

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                        ad.Dispose();
                    }
                }
            }
            catch (SqlException ex)
            {
             
                XtraMessageBox.Show(ex.Message);
            }

             
            Documento d = new Documento("", 0);
            d.ShowDocument(reporte);
           
            
        }

        private void SqlReportePrev(int pPeriodo, string pConjunto)
        {
            string sql = "DECLARE @TopeSalud as decimal " +
                         "DECLARE @Caja as decimal " +
                         "SET @TopeSalud = (SELECT ROUND(uf*topeAFP, 0) FROM valoresmes WHERE anomes=@MES) " +
                         "SET @Caja = (SELECT codafiliacion FROM empresa) " +
                         " " + 
                         "SELECT entidad, sum(cotizacion) as cotizacion, SUM(SeguroInv) as SeguroInv, SUM(Ahorro) as Ahorro, SUM(Sub) as Sub, " +
                         "SUM(SegEmpleador) as SeguroEmpleador, SUM(SegAfiliado) as SeguroAfiliado, SUM(Adicional) as Adicional, Sum(Familia) as Familia, Indice FROM " +
                         "(select tb.rut, tb.nombre, tb.apepaterno, a.nombre as entidad,  " +
                         "IIF(coditem = 'PREVISI' AND tb.cajaPrevision = 0, valorcalculado, 0) as cotizacion, IIF(coditem = 'SEGINV', valorcalculado, 0) as 'SeguroInv',  " +
                         "IIF(coditem = 'AFPAHO', valorcalculado, 0) as Ahorro, 0 as Sub, IIF(coditem = 'SCEMPRE', valorcalculado, 0) as 'SegEmpleador', " +
                         "IIF(coditem = 'SCEMPLE', valorcalculado, 0) as 'SegAfiliado', 0 as adicional, 0 as Familia, '1-Afp' as Indice " +
                         "FROM itemtrabajador it " +
                         "INNER JOIN trabajador tb ON tb.contrato = it.contrato AND tb.anomes = it.anomes " +
                         "INNER JOIN afp a on a.id = tb.afp " +
                         "WHERE tb.anomes = @MES AND suspendido = 0 AND(coditem = 'PREVISI' OR coditem = 'SEGINV' OR coditem = 'AFPAHO' OR coditem = 'SCEMPRE' OR coditem = 'SCEMPLE') AND tb.status = 1 AND tb.afp <> 0 {condition}" +
                       "UNION " +
                         "select tb.rut, tb.nombre,tb.apepaterno, a.nombre as entidad, valorcalculado, 0, 0, 0, 0, 0, 0, 0, '2-Ahorro Previsional' FROM itemtrabajador it " +
                         "INNER JOIN trabajador tb ON tb.contrato = it.contrato AND tb.anomes = it.anomes " +
                         "INNER JOIN afp a on a.id = tb.afp " +
                         "WHERE tb.anomes = @MES AND suspendido = 0 AND coditem = 'APREVOL' AND tb.status = 1 {condition}" +
                      "UNION " +
                         "select tb.rut, tb.nombre,tb.apepaterno, isa.nombre as entidad, IIF(tb.salud=1, IIF(@caja = 2 OR @caja=3 ,ROUND((valorcalculado * 0.064)/0.07, 0), valorcalculado), IIF(valorcalculado>ROUND(0.07*@TopeSalud, 0), ROUND(0.07*@TopeSalud, 0), valorcalculado)), 0, 0, 0, 0, 0, " +
                         "IIF(tb.salud > 1,IIF(valorcalculado > ROUND(0.07*@TopeSalud,0), valorcalculado - ROUND(0.07*@TopeSalud,0), 0), 0), 0, '3-Isapre' FROM itemtrabajador it " +
                         "INNER JOIN trabajador tb ON tb.contrato = it.contrato AND tb.anomes = it.anomes " +
                         "INNER JOIN isapre isa on isa.id = tb.salud " +
                         "WHERE tb.anomes = @MES AND suspendido = 0 AND coditem = 'SALUD' AND tb.status = 1 AND tb.cajaPrevision = 0 AND tb.salud <> 0 {condition}" +
                      "UNION " +
                         "select tb.rut, tb.nombre,tb.apepaterno, 'INP', valorcalculado, 0, 0, 0, 0, 0, 0, 0, '4-Inp' FROM itemtrabajador it " +
                         "INNER JOIN trabajador tb ON tb.contrato = it.contrato AND tb.anomes = it.anomes " +
                         "INNER JOIN cajaprevision caj on caj.id = tb.cajaprevision " +
                         "WHERE tb.anomes = @MES AND suspendido = 0 AND coditem = 'previsi' AND tb.status = 1 AND tb.cajaPrevision > 0 {condition}" +
                      "UNION " +
                         "select tb.rut, tb.nombre,tb.apepaterno, 'INP', 0, 0, 0, 0, 0, 0, 0, IIF(@Caja <> 2 AND @Caja <> 3, valorcalculado, 0), '4-Inp' FROM itemtrabajador it  " +
                         "INNER JOIN trabajador tb ON tb.contrato = it.contrato AND tb.anomes = it.anomes " +
                         "WHERE tb.anomes = @MES AND suspendido = 0 AND(coditem = 'CAJACOM' OR coditem = 'ASIGFAM' OR coditem = 'ASIFAR' OR coditem = 'ASIGMAT' OR coditem = 'ASIGINV') AND tb.status = 1 {condition}" +
                      "UNION " +
                         "select tb.rut, tb.nombre,tb.apepaterno, 'Caja de compensacion', IIF(coditem='CAJACOM', valorcalculado, 0), 0, 0, 0, 0, 0, 0, IIF(@Caja=2 OR @Caja=3, IIF(coditem = 'ASIGFAM' OR coditem = 'ASIFAR' OR coditem = 'ASIGMAT' OR coditem = 'ASIGINV', valorcalculado, 0), 0), '5-Caja Compensacion' FROM itemtrabajador it " +
                         "INNER JOIN trabajador tb ON tb.contrato = it.contrato AND tb.anomes = it.anomes " +
                         "WHERE tb.anomes = @MES AND suspendido = 0 AND(coditem = 'CAJACOM' OR coditem = 'ASIGFAM' OR coditem = 'ASIFAR' OR coditem = 'ASIGMAT' OR coditem = 'ASIGINV') AND tb.status = 1 {condition}" +
                      "UNION " +
                         "select tb.rut, tb.nombre,tb.apepaterno, 'Mutual (*Incluye LeySanna)', valorcalculado, 0, 0, 0, 0, 0, 0, 0, '6-Mutual' FROM itemtrabajador it " +
                         "INNER JOIN trabajador tb ON tb.contrato = it.contrato AND tb.anomes = it.anomes " +
                         "WHERE tb.anomes = @MES AND suspendido = 0 AND (coditem = 'MUTUALI' OR coditem='SANNA') AND tb.status = 1 {condition}) as tabla " +
                          "GROUP BY entidad, indice";

            SqlConnection cn;
            SqlCommand cmd;
            SqlDataAdapter ad = new SqlDataAdapter();
            DataSet ds = new DataSet();
            //RptResumenPreviredV2 resumen = new RptResumenPreviredV2();
            ReportesExternos.RptResumenPreviredV2 resumen = new ReportesExternos.RptResumenPreviredV2();
            resumen.LoadLayoutFromXml(Path.Combine(fnSistema.RutaCarpetaReportesExterno, "RptResumenPreviredV2.repx"));
            string Cond = "", Filtro = "";
            Empresa emp = new Empresa();
            emp.SetInfo();
            string d = User.GetUserFilter();

            if (pConjunto.Length > 0 && User.GetUserFilter().Length > 0)
                Cond = Conjunto.GetCondicionFromCode(User.GetUserFilter()) + ";" + Conjunto.GetCondicionFromCode(pConjunto);
            else if (User.GetUserFilter().Length > 0)
                Cond = Conjunto.GetCondicionFromCode(User.GetUserFilter());
            else if (User.GetUserFilter().Length == 0)
                Cond = "";
            
            Filtro = Calculo.GetSqlFiltro(User.GetUserFilter(), pConjunto, User.ShowPrivadas());
            if (Filtro.ToLower().Contains(" contrato"))
            {                
                
                Filtro = Filtro.ToLower().Replace("contrato", "tb.contrato");
            }
                
            
                sql = sql.Replace("{condition}", Filtro);

            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            //parametros
                            cmd.Parameters.Add(new SqlParameter("@MES", pPeriodo));

                            ad.SelectCommand = cmd;
                            ad.Fill(ds, "data");

                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                resumen.DataSource = ds.Tables[0];
                                resumen.DataMember = "data";

                                resumen.Parameters["periodo"].Value = fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(pPeriodo));
                                resumen.Parameters["condicion"].Value = Cond;
                                resumen.Parameters["empresa"].Value = emp.Razon;

                                resumen.Parameters["periodo"].Visible = false;
                                resumen.Parameters["condicion"].Visible = false;
                                resumen.Parameters["empresa"].Visible = false;

                                Documento doc = new Documento("", 0);
                                //resumen.SaveLayoutToXml(Path.Combine(fnSistema.RutaCarpetaReportesExterno, "RptResumenPreviredV2.repx"));
                                doc.ShowDocument(resumen);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                //ERROR...
            }
        }

        //PARA TOTAL AFP
        private double TotalAfp(int pPeriodo)
        {
            string sql = "SELECT sum(valorcalculado) FROM itemtrabajador " +
                        "WHERE anomes = @pPeriodo AND coditem = 'PREVISI'";

            double valor = 0;
            SqlCommand cmd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pPeriodo", pPeriodo));
                        valor = Convert.ToDouble(cmd.ExecuteScalar());

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return valor;
        }

        //PARA TOTAL SALUD
        private double TotalSalud(int pPeriodo)
        {
            string sql = "SELECT sum(valorcalculado) FROM itemtrabajador " +
                        "WHERE anomes = @pPeriodo AND coditem = 'SALUD'";

            double value = 0;
            SqlCommand cmd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pPeriodo", pPeriodo));

                        value = Convert.ToDouble(cmd.ExecuteScalar());

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return value;
        }

        //PARA TOTAL SEGUROS
        private double TotalSeguros(int pPeriodo)
        {
            string sql = "select sum(valorcalculado) as suma FROM itemtrabajador " +
                        "where anomes = 201808 AND(coditem = 'SCEMPLE' OR coditem = 'SCEMPRE' OR coditem = 'SEGINV')";
            SqlCommand cmd;
            double value = 0;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pPeriodo", pPeriodo));

                        value = Convert.ToDouble(cmd.ExecuteScalar());

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return value;
        }

        //PREPARA SQL
        private string GetSqlFinal(string pCodeConjunto)
        {           
            string sql = "";
            string cadenaConjunto = "";
            //USUARIO NO USA FILTRO
            if (FiltroUsuario == "0")
            {
                //SI SELECCIONA CONJUNTO
                if (pCodeConjunto != "")
                {
                    cadenaConjunto = Conjunto.GetCondicionFromCode(pCodeConjunto);
                    sql = "SELECT nombre, SUM(cotizacion) as cotizacion , SUM(seguroinvalidez) as seguroInvalidez, SUM(segurotrabajador) as segurotrabajador, SUM(seguroempresa) as seguroempresa, SUM(asignacion) as familiar, SUM(total) as total, orden, detalle from " +
                            "( " +
                                "select afp.nombre as nombre, SUM(valorcalculado) as cotizacion, 0 as seguroinvalidez , 0 as segurotrabajador , 0 as seguroempresa , 0 as asignacion , 0 as total , 1 as orden, 'dato' as detalle  FROM itemtrabajador " +
                                "INNER JOIN trabajador On trabajador.contrato = itemTrabajador.contrato and trabajador.anomes = itemTrabajador.anomes " +
                                "INNER JOIN afp ON afp.id = trabajador.afp " +
                                $"WHERE itemTrabajador.anomes = @pPeriodo AND coditem = 'PREVISI' AND itemTrabajador.contrato IN(SELECT contrato FROM trabajador WHERE {cadenaConjunto}) " +
                                "GROUP BY afp.nombre " +
                             "UNION " +
                                "select afp.nombre, 0, SUM(valorcalculado) , 0 , 0, 0, 0, 1, 'dato' from itemtrabajador " +
                                 "INNER JOIN trabajador On trabajador.contrato = itemTrabajador.contrato and trabajador.anomes = itemTrabajador.anomes " +
                                "INNER JOIN afp ON afp.id = trabajador.afp " +
                                $"WHERE itemTrabajador.anomes = @pPeriodo AND coditem = 'SEGINV' AND itemTrabajador.contrato IN(SELECT contrato FROM trabajador WHERE {cadenaConjunto}) " +
                                "GROUP BY afp.nombre " +
                            "UNION " +
                                "select afp.nombre, 0, 0, SUM(valorcalculado), 0, 0, 0, 1, 'dato' from itemtrabajador " +
                                "INNER JOIN trabajador On trabajador.contrato = itemTrabajador.contrato and trabajador.anomes = itemTrabajador.anomes " +
                                "INNER JOIN afp ON afp.id = trabajador.afp " +
                                $"WHERE itemTrabajador.anomes = @pPeriodo AND coditem = 'SCEMPLE' AND itemTrabajador.contrato IN(SELECT contrato FROM trabajador WHERE {cadenaConjunto}) " +
                                "GROUP BY afp.nombre " +
                             "UNION " +
                                "select afp.nombre, 0, 0, 0, SUM(valorcalculado), 0, 0, 1, 'dato' from itemtrabajador " +
                                "INNER JOIN trabajador On trabajador.contrato = itemTrabajador.contrato and trabajador.anomes = itemTrabajador.anomes " +
                                "INNER JOIN afp ON afp.id = trabajador.afp " +
                                $"WHERE itemTrabajador.anomes = @pPeriodo AND coditem = 'SCEMPRE' AND itemTrabajador.contrato IN(SELECT contrato FROM trabajador WHERE {cadenaConjunto}) " +
                                "GROUP BY afp.nombre " +
                            "UNION " +
                                "SELECT afp.nombre, 0, 0, 0, 0, 0, SUM(valorcalculado), 1, 'dato' FROM itemTrabajador " +
                                "INNER JOIN trabajador On trabajador.contrato = itemtrabajador.contrato AND itemtrabajador.anomes = trabajador.anomes " +
                                "INNER JOIN afp ON afp.id = trabajador.afp " +
                                "WHERE itemTrabajador.anomes = @pPeriodo AND(coditem = 'SEGINV' OR coditem = 'SCEMPLE' OR coditem = 'SCEMPRE' OR coditem = 'PREVISI') " +
                                $"AND itemTrabajador.contrato IN(SELECT contrato FROM trabajador WHERE {cadenaConjunto}) " +
                                "GROUP BY afp.nombre " +
                            "UNION " +
                                "SELECT isapre.nombre, SUM(valorcalculado), 0, 0, 0, 0, SUM(valorcalculado), 4, 'dato' FROM itemTrabajador " +
                                "INNER JOIN trabajador ON trabajador.anomes = itemtrabajador.anomes AND itemtrabajador.contrato = trabajador.contrato " +
                                "INNER JOIN isapre ON isapre.id = trabajador.salud " +
                                "WHERE itemTrabajador.anomes = @pPeriodo AND coditem = 'SALUD' AND isapre.id > 1 " +
                                $"AND itemtrabajador.contrato IN(SELECT contrato FROM trabajador WHERE {cadenaConjunto}) " +
                                "GROUP BY isapre.nombre " +
                            "UNION " +
                                "SELECT 'FONASA', fonasa.name - caja.name, 0, 0, 0, 0, fonasa.name - caja.name, 4, 'dato' FROM " +
                                "( " +
                                    "SELECT SUM(valorcalculado) as name FROM itemTrabajador " +
                                    "INNER JOIN trabajador ON itemTrabajador.anomes = trabajador.anomes AND itemTrabajador.contrato = trabajador.contrato " +
                                    "INNER JOIN isapre ON isapre.id = trabajador.salud " +
                                    "WHERE itemTrabajador.anomes = @pPeriodo AND coditem = 'SALUD' AND isapre.id = 1 " +
                                    $"AND itemTrabajador.contrato IN(SELECT contrato FROM trabajador WHERE {cadenaConjunto}) " +
                                    "GROUP BY isapre.nombre " + 
                                ") as fonasa, " +
	                            "( " +
                                    "SELECT SUM(valorcalculado) as name FROM itemTrabajador " +
                                    "WHERE anomes = @pPeriodo AND coditem = 'CAJACOM' " +
                                    $"AND contrato IN(SELECT contrato FROM trabajador WHERE {cadenaConjunto}) " +
	                             ") as caja " +
                            "UNION " +
                                 "SELECT 'CAJA', c1.suma, 0, 0, 0, ISNULL(c2.suma, 0), c1.suma - ISNULL(c2.suma, 0), 8, 'dato' FROM " +
                                    $"(SELECT SUM(valorcalculado) as suma FROM itemtrabajador WHERE anomes = @pPeriodo AND coditem = 'CAJACOM' AND contrato IN(SELECT contrato FROM trabajador WHERE {cadenaConjunto})) as c1, " +
	                                $"(SELECT SUM(valorcalculado) as suma FROM itemtrabajador WHERE anomes = @pPeriodo AND tipo = 3 AND contrato IN(SELECT contrato FROM trabajador WHERE {cadenaConjunto})) as c2 " +
                            "UNION " +
                                   "SELECT 'MUTUAL', ISNULL(SUM(sysmutual + sysvalsanna), 0), 0, 0, 0, 0, ISNULL(SUM(sysmutual + sysvalsanna), 0), 9, 'dato'  " +
                                   "FROM calculomensual WHERE anomes = @pPeriodo " +
                                   $"AND contrato IN(SELECT contrato FROM trabajador WHERE {cadenaConjunto}) " + 
                        ") as tabla " +
                        "GROUP BY nombre, orden, detalle " +
                        "ORDER BY orden";
                }
                else {
                    sql = "SELECT nombre, SUM(cotizacion) as cotizacion , SUM(seguroinvalidez) as seguroInvalidez, SUM(segurotrabajador) as segurotrabajador, SUM(seguroempresa) as seguroempresa, SUM(asignacion) as familiar, SUM(total) as total, orden, detalle from " +
                            "( " +
                                "select afp.nombre as nombre, SUM(valorcalculado) as cotizacion, 0 as seguroinvalidez , 0 as segurotrabajador , 0 as seguroempresa , 0 as asignacion , 0 as total , 1 as orden, 'dato' as detalle  FROM itemtrabajador " +
                                "INNER JOIN trabajador On trabajador.contrato = itemTrabajador.contrato and trabajador.anomes = itemTrabajador.anomes " +
                                "INNER JOIN afp ON afp.id = trabajador.afp " +
                                "WHERE itemTrabajador.anomes = @pPeriodo AND coditem = 'PREVISI'  " +
                                "GROUP BY afp.nombre " +
                             "UNION " +
                                "select afp.nombre, 0, SUM(valorcalculado) , 0 , 0, 0, 0, 1, 'dato' from itemtrabajador " +
                                 "INNER JOIN trabajador On trabajador.contrato = itemTrabajador.contrato and trabajador.anomes = itemTrabajador.anomes " +
                                "INNER JOIN afp ON afp.id = trabajador.afp " +
                                "WHERE itemTrabajador.anomes = @pPeriodo AND coditem = 'SEGINV'  " +
                                "GROUP BY afp.nombre " +
                            "UNION " +
                                "select afp.nombre, 0, 0, SUM(valorcalculado), 0, 0, 0, 1, 'dato' from itemtrabajador " +
                                "INNER JOIN trabajador On trabajador.contrato = itemTrabajador.contrato and trabajador.anomes = itemTrabajador.anomes " +
                                "INNER JOIN afp ON afp.id = trabajador.afp " +
                                "WHERE itemTrabajador.anomes = @pPeriodo AND coditem = 'SCEMPLE'  " +
                                "GROUP BY afp.nombre " +
                             "UNION " +
                                "select afp.nombre, 0, 0, 0, SUM(valorcalculado), 0, 0, 1, 'dato' from itemtrabajador " +
                                "INNER JOIN trabajador On trabajador.contrato = itemTrabajador.contrato and trabajador.anomes = itemTrabajador.anomes " +
                                "INNER JOIN afp ON afp.id = trabajador.afp " +
                                "WHERE itemTrabajador.anomes = @pPeriodo AND coditem = 'SCEMPRE'  " +
                                "GROUP BY afp.nombre " +
                            "UNION " +
                                "SELECT afp.nombre, 0, 0, 0, 0, 0, SUM(valorcalculado), 1, 'dato' FROM itemTrabajador " +
                                "INNER JOIN trabajador On trabajador.contrato = itemtrabajador.contrato AND itemtrabajador.anomes = trabajador.anomes " +
                                "INNER JOIN afp ON afp.id = trabajador.afp " +
                                "WHERE itemTrabajador.anomes = @pPeriodo AND(coditem = 'SEGINV' OR coditem = 'SCEMPLE' OR coditem = 'SCEMPRE' OR coditem = 'PREVISI') " +                              
                                "GROUP BY afp.nombre " +
                            "UNION " +
                                "SELECT isapre.nombre, SUM(valorcalculado), 0, 0, 0, 0, SUM(valorcalculado), 4, 'dato' FROM itemTrabajador " +
                                "INNER JOIN trabajador ON trabajador.anomes = itemtrabajador.anomes AND itemtrabajador.contrato = trabajador.contrato " +
                                "INNER JOIN isapre ON isapre.id = trabajador.salud " +
                                "WHERE itemTrabajador.anomes = @pPeriodo AND coditem = 'SALUD' AND isapre.id > 1 " +                                
                                "GROUP BY isapre.nombre " +
                            "UNION " +
                                "SELECT 'FONASA', fonasa.name - caja.name, 0, 0, 0, 0, fonasa.name - caja.name, 4, 'dato' FROM " +
                                "( " +
                                    "SELECT SUM(valorcalculado) as name FROM itemTrabajador " +
                                    "INNER JOIN trabajador ON itemTrabajador.anomes = trabajador.anomes AND itemTrabajador.contrato = trabajador.contrato " +
                                    "INNER JOIN isapre ON isapre.id = trabajador.salud " +
                                    "WHERE itemTrabajador.anomes = @pPeriodo AND coditem = 'SALUD' AND isapre.id = 1 " +                                  
                                    "GROUP BY isapre.nombre " +
                                ") as fonasa, " +
                                "( " +
                                    "SELECT SUM(valorcalculado) as name FROM itemTrabajador " +
                                    "WHERE anomes = @pPeriodo AND coditem = 'CAJACOM' " +                               
                                 ") as caja " +
                            "UNION " +
                                 "SELECT 'CAJA', c1.suma, 0, 0, 0, ISNULL(c2.suma, 0), c1.suma - ISNULL(c2.suma, 0), 8, 'dato' FROM " +
                                    "(SELECT SUM(valorcalculado) as suma FROM itemtrabajador WHERE anomes = @pPeriodo AND coditem = 'CAJACOM' ) as c1, " +
                                    "(SELECT SUM(valorcalculado) as suma FROM itemtrabajador WHERE anomes = @pPeriodo AND tipo = 3 ) as c2 " +
                            "UNION " +
                                    "SELECT 'MUTUAL', ISNULL(SUM(sysmutual + sysvalsanna), 0), 0, 0, 0, 0, ISNULL(SUM(sysmutual + sysvalsanna), 0), 9, 'dato'  " +
                                   "FROM calculomensual WHERE anomes = @pPeriodo " +                                   
                        ") as tabla " +
                        "GROUP BY nombre, orden, detalle " +
                        "ORDER BY orden";
                }
            }
            else
            {
               
                //SI USA FILTRO
                //SI SELECCIONA CONJUNTTO
                if (pCodeConjunto != "")
                {
                    FiltroUsuario = Conjunto.GetCondicionFromCode(FiltroUsuario);
                    cadenaConjunto = Conjunto.GetCondicionFromCode(pCodeConjunto);
                    sql = "SELECT nombre, SUM(cotizacion) as cotizacion , SUM(seguroinvalidez) as seguroInvalidez, SUM(segurotrabajador) as segurotrabajador, SUM(seguroempresa) as seguroempresa, SUM(asignacion) as familiar, SUM(total) as total, orden, detalle from " +
                        "( " +
                            "select afp.nombre as nombre, SUM(valorcalculado) as cotizacion, 0 as seguroinvalidez , 0 as segurotrabajador , 0 as seguroempresa , 0 as asignacion , 0 as total , 1 as orden, 'dato' as detalle  FROM itemtrabajador " +
                            "INNER JOIN trabajador On trabajador.contrato = itemTrabajador.contrato and trabajador.anomes = itemTrabajador.anomes " +
                            "INNER JOIN afp ON afp.id = trabajador.afp " +
                            $"WHERE itemTrabajador.anomes = @pPeriodo AND coditem = 'PREVISI' AND itemTrabajador.contrato IN(SELECT contrato FROM trabajador WHERE {FiltroUsuario} AND {cadenaConjunto}) " +
                            "GROUP BY afp.nombre " +
                         "UNION " +
                            "select afp.nombre, 0, SUM(valorcalculado) , 0 , 0, 0, 0, 1, 'dato' from itemtrabajador " +
                             "INNER JOIN trabajador On trabajador.contrato = itemTrabajador.contrato and trabajador.anomes = itemTrabajador.anomes " +
                            "INNER JOIN afp ON afp.id = trabajador.afp " +
                            $"WHERE itemTrabajador.anomes = @pPeriodo AND coditem = 'SEGINV' AND itemTrabajador.contrato IN(SELECT contrato FROM trabajador WHERE {FiltroUsuario} AND {cadenaConjunto}) " +
                            "GROUP BY afp.nombre " +
                        "UNION " +
                            "select afp.nombre, 0, 0, SUM(valorcalculado), 0, 0, 0, 1, 'dato' from itemtrabajador " +
                            "INNER JOIN trabajador On trabajador.contrato = itemTrabajador.contrato and trabajador.anomes = itemTrabajador.anomes " +
                            "INNER JOIN afp ON afp.id = trabajador.afp " +
                            $"WHERE itemTrabajador.anomes = @pPeriodo AND coditem = 'SCEMPLE' AND itemTrabajador.contrato IN(SELECT contrato FROM trabajador WHERE {FiltroUsuario} AND {cadenaConjunto}) " +
                            "GROUP BY afp.nombre " +
                         "UNION " +
                            "select afp.nombre, 0, 0, 0, SUM(valorcalculado), 0, 0, 1, 'dato' from itemtrabajador " +
                            "INNER JOIN trabajador On trabajador.contrato = itemTrabajador.contrato and trabajador.anomes = itemTrabajador.anomes " +
                            "INNER JOIN afp ON afp.id = trabajador.afp " +
                            $"WHERE itemTrabajador.anomes = @pPeriodo AND coditem = 'SCEMPRE' AND itemTrabajador.contrato IN(SELECT contrato FROM trabajador WHERE {FiltroUsuario} AND {cadenaConjunto}) " +
                            "GROUP BY afp.nombre " +
                        "UNION " +
                            "SELECT afp.nombre, 0, 0, 0, 0, 0, SUM(valorcalculado), 1, 'dato' FROM itemTrabajador " +
                            "INNER JOIN trabajador On trabajador.contrato = itemtrabajador.contrato AND itemtrabajador.anomes = trabajador.anomes " +
                            "INNER JOIN afp ON afp.id = trabajador.afp " +
                            "WHERE itemTrabajador.anomes = @pPeriodo AND(coditem = 'SEGINV' OR coditem = 'SCEMPLE' OR coditem = 'SCEMPRE' OR coditem = 'PREVISI') " +
                            $"AND itemTrabajador.contrato IN(SELECT contrato FROM trabajador WHERE {FiltroUsuario} AND {cadenaConjunto}) " +
                            "GROUP BY afp.nombre " +
                        "UNION " +
                            "SELECT isapre.nombre, SUM(valorcalculado), 0, 0, 0, 0, SUM(valorcalculado), 4, 'dato' FROM itemTrabajador " +
                            "INNER JOIN trabajador ON trabajador.anomes = itemtrabajador.anomes AND itemtrabajador.contrato = trabajador.contrato " +
                            "INNER JOIN isapre ON isapre.id = trabajador.salud " +
                            "WHERE itemTrabajador.anomes = @pPeriodo AND coditem = 'SALUD' AND isapre.id > 1 " +
                            $"AND itemtrabajador.contrato IN(SELECT contrato FROM trabajador WHERE {FiltroUsuario} AND {cadenaConjunto}) " +
                            "GROUP BY isapre.nombre " +
                        "UNION " +
                            "SELECT 'FONASA', fonasa.name - caja.name, 0, 0, 0, 0, fonasa.name - caja.name, 4, 'dato' FROM " +
                            "( " +
                                "SELECT SUM(valorcalculado) as name FROM itemTrabajador " +
                                "INNER JOIN trabajador ON itemTrabajador.anomes = trabajador.anomes AND itemTrabajador.contrato = trabajador.contrato " +
                                "INNER JOIN isapre ON isapre.id = trabajador.salud " +
                                "WHERE itemTrabajador.anomes = @pPeriodo AND coditem = 'SALUD' AND isapre.id = 1 " +
                                $"AND itemTrabajador.contrato IN(SELECT contrato FROM trabajador WHERE {FiltroUsuario} AND {cadenaConjunto}) " +
                                "GROUP BY isapre.nombre " +
                            ") as fonasa, " +
                            "( " +
                                "SELECT SUM(valorcalculado) as name FROM itemTrabajador " +
                                "WHERE anomes = @pPeriodo AND coditem = 'CAJACOM' " +
                                $"AND contrato IN(SELECT contrato FROM trabajador WHERE {FiltroUsuario} AND {cadenaConjunto}) " +
                             ") as caja " +
                        "UNION " +
                             "SELECT 'CAJA', c1.suma, 0, 0, 0, ISNULL(c2.suma, 0), c1.suma - ISNULL(c2.suma, 0), 8, 'dato' FROM " +
                                $"(SELECT SUM(valorcalculado) as suma FROM itemtrabajador WHERE anomes = @pPeriodo AND coditem = 'CAJACOM' AND contrato IN(SELECT contrato FROM trabajador WHERE {FiltroUsuario} AND {cadenaConjunto})) as c1, " +
                                $"(SELECT SUM(valorcalculado) as suma FROM itemtrabajador WHERE anomes = @pPeriodo AND tipo = 3 AND contrato IN(SELECT contrato FROM trabajador WHERE {FiltroUsuario} AND {cadenaConjunto})) as c2 " +
                        "UNION " +
                                "SELECT 'MUTUAL', ISNULL(SUM(sysmutual + sysvalsanna), 0), 0, 0, 0, 0, ISNULL(SUM(sysmutual + sysvalsanna), 0), 9, 'dato'  " +
                                   "FROM calculomensual WHERE anomes = @pPeriodo " +
                                   $"AND contrato IN(SELECT contrato FROM trabajador WHERE {FiltroUsuario} AND {cadenaConjunto}) " +
                    ") as tabla " +
                    "GROUP BY nombre, orden, detalle " +
                    "ORDER BY orden";
                }
                else
                {
                    FiltroUsuario = Conjunto.GetCondicionFromCode(FiltroUsuario);
                    sql = "SELECT nombre, SUM(cotizacion) as cotizacion , SUM(seguroinvalidez) as seguroInvalidez, SUM(segurotrabajador) as segurotrabajador, SUM(seguroempresa) as seguroempresa, SUM(asignacion) as familiar, SUM(total) as total, orden, detalle from " +
                                 "( " +
                                     "select afp.nombre as nombre, SUM(valorcalculado) as cotizacion, 0 as seguroinvalidez , 0 as segurotrabajador , 0 as seguroempresa , 0 as asignacion , 0 as total , 1 as orden, 'dato' as detalle  FROM itemtrabajador " +
                                     "INNER JOIN trabajador On trabajador.contrato = itemTrabajador.contrato and trabajador.anomes = itemTrabajador.anomes " +
                                     "INNER JOIN afp ON afp.id = trabajador.afp " +
                                     $"WHERE itemTrabajador.anomes = @pPeriodo AND coditem = 'PREVISI' AND itemTrabajador.contrato IN(SELECT contrato FROM trabajador WHERE {FiltroUsuario}) " +
                                     "GROUP BY afp.nombre " +
                                  "UNION " +
                                     "select afp.nombre, 0, SUM(valorcalculado) , 0 , 0, 0, 0, 1, 'dato' from itemtrabajador " +
                                      "INNER JOIN trabajador On trabajador.contrato = itemTrabajador.contrato and trabajador.anomes = itemTrabajador.anomes " +
                                     "INNER JOIN afp ON afp.id = trabajador.afp " +
                                     $"WHERE itemTrabajador.anomes = @pPeriodo AND coditem = 'SEGINV' AND itemTrabajador.contrato IN(SELECT contrato FROM trabajador WHERE {FiltroUsuario} ) " +
                                     "GROUP BY afp.nombre " +
                                 "UNION " +
                                     "select afp.nombre, 0, 0, SUM(valorcalculado), 0, 0, 0, 1, 'dato' from itemtrabajador " +
                                     "INNER JOIN trabajador On trabajador.contrato = itemTrabajador.contrato and trabajador.anomes = itemTrabajador.anomes " +
                                     "INNER JOIN afp ON afp.id = trabajador.afp " +
                                     $"WHERE itemTrabajador.anomes = @pPeriodo AND coditem = 'SCEMPLE' AND itemTrabajador.contrato IN(SELECT contrato FROM trabajador WHERE {FiltroUsuario} ) " +
                                     "GROUP BY afp.nombre " +
                                  "UNION " +
                                     "select afp.nombre, 0, 0, 0, SUM(valorcalculado), 0, 0, 1, 'dato' from itemtrabajador " +
                                     "INNER JOIN trabajador On trabajador.contrato = itemTrabajador.contrato and trabajador.anomes = itemTrabajador.anomes " +
                                     "INNER JOIN afp ON afp.id = trabajador.afp " +
                                     $"WHERE itemTrabajador.anomes = @pPeriodo AND coditem = 'SCEMPRE' AND itemTrabajador.contrato IN(SELECT contrato FROM trabajador WHERE {FiltroUsuario} ) " +
                                     "GROUP BY afp.nombre " +
                                 "UNION " +
                                     "SELECT afp.nombre, 0, 0, 0, 0, 0, SUM(valorcalculado), 1, 'dato' FROM itemTrabajador " +
                                     "INNER JOIN trabajador On trabajador.contrato = itemtrabajador.contrato AND itemtrabajador.anomes = trabajador.anomes " +
                                     "INNER JOIN afp ON afp.id = trabajador.afp " +
                                     "WHERE itemTrabajador.anomes = @pPeriodo AND(coditem = 'SEGINV' OR coditem = 'SCEMPLE' OR coditem = 'SCEMPRE' OR coditem = 'PREVISI') " +
                                     $"AND itemTrabajador.contrato IN(SELECT contrato FROM trabajador WHERE {FiltroUsuario} ) " +
                                     "GROUP BY afp.nombre " +
                                 "UNION " +
                                     "SELECT isapre.nombre, SUM(valorcalculado), 0, 0, 0, 0, SUM(valorcalculado), 4, 'dato' FROM itemTrabajador " +
                                     "INNER JOIN trabajador ON trabajador.anomes = itemtrabajador.anomes AND itemtrabajador.contrato = trabajador.contrato " +
                                     "INNER JOIN isapre ON isapre.id = trabajador.salud " +
                                     "WHERE itemTrabajador.anomes = @pPeriodo AND coditem = 'SALUD' AND isapre.id > 1 " +
                                     $"AND itemtrabajador.contrato IN(SELECT contrato FROM trabajador WHERE {FiltroUsuario} ) " +
                                     "GROUP BY isapre.nombre " +
                                 "UNION " +
                                     "SELECT 'FONASA', fonasa.name - caja.name, 0, 0, 0, 0, fonasa.name - caja.name, 4, 'dato' FROM " +
                                     "( " +
                                         "SELECT SUM(valorcalculado) as name FROM itemTrabajador " +
                                         "INNER JOIN trabajador ON itemTrabajador.anomes = trabajador.anomes AND itemTrabajador.contrato = trabajador.contrato " +
                                         "INNER JOIN isapre ON isapre.id = trabajador.salud " +
                                         "WHERE itemTrabajador.anomes = @pPeriodo AND coditem = 'SALUD' AND isapre.id = 1 " +
                                         $"AND itemTrabajador.contrato IN(SELECT contrato FROM trabajador WHERE {FiltroUsuario} ) " +
                                         "GROUP BY isapre.nombre " +
                                     ") as fonasa, " +
                                     "( " +
                                         "SELECT SUM(valorcalculado) as name FROM itemTrabajador " +
                                         "WHERE anomes = @pPeriodo AND coditem = 'CAJACOM' " +
                                         $"AND contrato IN(SELECT contrato FROM trabajador WHERE {FiltroUsuario} ) " +
                                      ") as caja " +
                                 "UNION " +
                                      "SELECT 'CAJA', c1.suma, 0, 0, 0, ISNULL(c2.suma, 0), c1.suma - ISNULL(c2.suma, 0), 8, 'dato' FROM " +
                                         $"(SELECT SUM(valorcalculado) as suma FROM itemtrabajador WHERE anomes = @pPeriodo AND coditem = 'CAJACOM' AND contrato IN(SELECT contrato FROM trabajador WHERE {FiltroUsuario} )) as c1, " +
                                         $"(SELECT SUM(valorcalculado) as suma FROM itemtrabajador WHERE anomes = @pPeriodo AND tipo = 3 AND contrato IN(SELECT contrato FROM trabajador WHERE {FiltroUsuario} )) as c2 " +
                                 "UNION " +
                                         "SELECT 'MUTUAL', ISNULL(SUM(sysmutual + sysvalsanna), 0), 0, 0, 0, 0, ISNULL(SUM(sysmutual + sysvalsanna), 0), 9, 'dato'  " +
                                   "FROM calculomensual WHERE anomes = @pPeriodo " +
                                   $"AND contrato IN(SELECT contrato FROM trabajador WHERE {FiltroUsuario}) " +
                             ") as tabla " +
                             "GROUP BY nombre, orden, detalle " +
                             "ORDER BY orden";
                }
            }                  

            return sql;
        }

        /// <summary>
        /// Obtenemos cadena con condicion adicional para generar reporte
        /// <para>Hace referencia a si el usuario tiene filtro, no puede ver fichas privadas, etc.</para>
        /// </summary>
        /// <returns></returns>
        private string GetCondition(string pConjunto)
        {
            string cadena = "";

            //USUARIO TIENE FILTRO?
            if (FiltroUsuario != "0")
            {
                //LA BUSQUEDA TIENE FILTROS?
                if (pConjunto != "")
                {
                    //PUEDE VER FICHAS privadas?
                    if (ShowPrivados == false)
                    {
                        cadena = $"AND itemtrabajador.contrato IN (SELECT contrato FROM trabajador WHERE {Conjunto.GetCondicionFromCode(FiltroUsuario)} AND {Conjunto.GetCondicionFromCode(pConjunto)} AND privado=0)";
                    }
                    else
                    {
                        cadena = $"AND itemtrabajador.contrato IN (SELECT contrato FROM trabajador WHERE {Conjunto.GetCondicionFromCode(FiltroUsuario)} AND {Conjunto.GetCondicionFromCode(pConjunto)})";
                    }
                }
                else
                {
                    //NO HAY BUSQUEDA CONDICIONADA...
                    if (ShowPrivados == false)
                        cadena = $"AND itemtrabajador.contrato IN (SELECT contrato FROM trabajador WHERE {Conjunto.GetCondicionFromCode(FiltroUsuario)} AND privado=0)";
                    else
                        cadena = $"AND itemtrabajador.contrato IN (SELECT contrato FROM trabajador WHERE {Conjunto.GetCondicionFromCode(FiltroUsuario)})";
                }
            }
            else
            {
                //USUARIO NO TIENE FILTROS
                //LA BUSQUEDA TIENE FILTROS?
                if (pConjunto != "")
                {
                    //PUEDE VER FICHAS privadas?
                    if (ShowPrivados == false)
                    {
                        cadena = $"AND itemtrabajador.contrato IN (SELECT contrato FROM trabajador WHERE {Conjunto.GetCondicionFromCode(pConjunto)} AND privado=0)";
                    }
                    else
                    {
                        cadena = $"AND itemtrabajador.contrato IN (SELECT contrato FROM trabajador WHERE {Conjunto.GetCondicionFromCode(pConjunto)})";
                    }
                }
                else
                {
                    //NO HAY BUSQUEDA CONDICIONADA...
                    if (ShowPrivados == false)
                        cadena = $"AND itemtrabajador.contrato IN (SELECT contrato FROM trabajador WHERE privado=0)";
                    else
                        cadena = "";
                }
            }

            return cadena;
        }
        /// <summary>
        /// Entrega consulta sql para reporte.
        /// </summary>
        /// <param name="pCodeConjunto"></param>
        /// <returns></returns>
        private string GetSqlDataSource(string pCodeConjunto)
        {
            //OBTENEMOS CONDICION EXTRA.
            string condition = GetCondition(pCodeConjunto);

            #region "QUERY"
            string sql = " DECLARE @FONASA  AS DECIMAL \n" +
                 "SET @FONASA = (SELECT SUM(valorcalculado) as name FROM itemTrabajador \n" +
                     "INNER JOIN trabajador ON itemTrabajador.anomes = trabajador.anomes AND itemTrabajador.contrato = trabajador.contrato \n" +
                     "INNER JOIN isapre ON isapre.id = trabajador.salud \n" +
                     "WHERE itemTrabajador.anomes = @pPeriodo AND coditem = 'SALUD' AND isapre.id = 1 \n" +
                     " AND suspendido = 0 {condition} \n" +
                    "GROUP BY isapre.nombre ) \n" +
                " " +
                "DECLARE @CAJA AS DECIMAL \n" +
                "SET @CAJA = (SELECT SUM(valorcalculado) as name FROM itemTrabajador \n" +
                    "WHERE anomes = @pPeriodo AND coditem = 'CAJACOM' \n" +
                    " AND suspendido = 0 {condition} ) \n" +
                " " +
                "DECLARE @ASIGNACIONES AS DECIMAL \n" +
                    "SET @ASIGNACIONES = (SELECT SUM(valorcalculado) as suma FROM itemtrabajador WHERE anomes = @pPeriodo AND tipo = 3 AND suspendido=0 {condition}) \n" +
                " " +
                "SELECT nombre, SUM(cotizacion) as Cotizacion , SUM(ahorro) as Ahorro, SUM(seguroinvalidez) as 'Seguro Invalidez', SUM(segurotrabajador) as 'Seguro Afiliado', SUM(seguroempresa) as 'Seguro Empresa', SUM(asignacion) as 'Asignaciones Familiares', SUM(total) as Total, orden, detalle from \n" +
                   " ( \n" +
                       " Select afp.nombre as nombre, SUM(valorcalculado) as cotizacion, 0 as Ahorro,  0 as seguroinvalidez , 0 as segurotrabajador , 0 as seguroempresa , 0 as asignacion , 0 as total , 1 as orden, 'dato' as detalle  FROM itemtrabajador \n" +
                       " INNER JOIN trabajador On trabajador.contrato = itemTrabajador.contrato and trabajador.anomes = itemTrabajador.anomes \n" +
                       " INNER JOIN afp ON afp.id = trabajador.afp \n" +
                       " WHERE itemTrabajador.anomes = @pPeriodo AND coditem = 'PREVISI' \n" +
                       " AND suspendido=0 {condition}  \n" +
                       " GROUP BY afp.nombre \n" +
                     "UNION \n" +
                       " SELECT afp.nombre, 0, SUM(valorcalculado), 0, 0, 0, 0, 0, 1, 'dato'  FROM itemTrabajador \n" +
                       "  INNER JOIN trabajador \n" +
                       " On trabajador.contrato = itemtrabajador.contrato AND trabajador.anomes = itemTrabajador.anomes \n" +
                       " INNER JOIN afp ON afp.id = trabajador.afp \n" +
                       " WHERE itemTrabajador.anomes = @pPeriodo AND(coditem = 'AFPAHO' OR coditem = 'APREVOL') \n" +
                       " AND suspendido=0 {condition} \n" +
                       " Group by afp.nombre \n" +
                     "UNION \n" +
                       " select afp.nombre, 0, 0,SUM(valorcalculado) , 0 , 0, 0, 0, 1, 'dato' from itemtrabajador \n" +
                       " INNER JOIN trabajador On trabajador.contrato = itemTrabajador.contrato and trabajador.anomes = itemTrabajador.anomes \n" +
                       " INNER JOIN afp ON afp.id = trabajador.afp \n" +
                       " WHERE itemTrabajador.anomes = @pPeriodo AND coditem = 'SEGINV'" +
                       " AND suspendido=0 {condition}  \n" +
                       " GROUP BY afp.nombre \n" +
                    "UNION \n" +
                       " Select afp.nombre, 0, 0, 0, SUM(valorcalculado), 0, 0, 0, 1, 'dato' from itemtrabajador \n" +
                       "INNER JOIN trabajador On trabajador.contrato = itemTrabajador.contrato and trabajador.anomes = itemTrabajador.anomes \n" +
                        "INNER JOIN afp ON afp.id = trabajador.afp \n" +
                        "WHERE itemTrabajador.anomes = @pPeriodo AND coditem = 'SCEMPLE' \n" +
                        " AND suspendido=0 {condition}  \n" +
                        "GROUP BY afp.nombre " +
                     "UNION \n" +
                        "Select afp.nombre, 0, 0, 0, 0, SUM(valorcalculado), 0, 0, 1, 'dato' from itemtrabajador \n" +
                        "INNER JOIN trabajador On trabajador.contrato = itemTrabajador.contrato and trabajador.anomes = itemTrabajador.anomes \n" +
                        "INNER JOIN afp ON afp.id = trabajador.afp \n" +
                        "WHERE itemTrabajador.anomes = @pPeriodo AND coditem = 'SCEMPRE' \n" +
                        "AND suspendido=0 {condition} \n" +
                        "GROUP BY afp.nombre \n" +
                    "UNION \n" +
                        "SELECT afp.nombre, 0, 0, 0, 0, 0, 0, SUM(valorcalculado), 1, 'dato' FROM itemTrabajador \n" +
                        "INNER JOIN trabajador On trabajador.contrato = itemtrabajador.contrato AND itemtrabajador.anomes = trabajador.anomes \n" +
                        "INNER JOIN afp ON afp.id = trabajador.afp \n" +
                        "WHERE itemTrabajador.anomes = @pPeriodo AND(coditem = 'SEGINV' OR coditem = 'SCEMPLE' OR coditem = 'SCEMPRE' OR coditem = 'PREVISI' OR coditem = 'AFPAHO' OR coditem = 'APREVOL') \n" +
                        "AND suspendido = 0 {condition}  \n" +
                        "GROUP BY afp.nombre \n" +
                    "UNION \n" +
                       "SELECT isapre.nombre, SUM(valorcalculado), 0, 0, 0, 0,0, SUM(valorcalculado), 4, 'dato' FROM itemTrabajador \n" +
                       " INNER JOIN trabajador ON trabajador.anomes = itemtrabajador.anomes AND itemtrabajador.contrato = trabajador.contrato \n" +
                       " INNER JOIN isapre ON isapre.id = trabajador.salud \n" +
                       " WHERE itemTrabajador.anomes = @pPeriodo AND coditem = 'SALUD' AND isapre.id > 1 \n" +
                       " AND suspendido = 0 {condition}  \n" +
                       "GROUP BY isapre.nombre \n" +
                    "UNION \n" +
                       "SELECT 'FONASA', @Fonasa - @Caja, 0, 0, 0, 0, 0,@Fonasa - @Caja, 4, 'dato' \n" +
                    "UNION \n" +
                       " SELECT 'CAJA', @Caja, 0, 0, 0,0, ISNULL(@Asignaciones, 0), @Caja - ISNULL(@Asignaciones, 0), 8, 'dato' \n" +
                    "UNION \n" +
                       " SELECT 'MUTUAL', ISNULL(SUM(sysmutual + sysvalsanna), 0), 0, 0, 0, 0,0, ISNULL(SUM(sysmutual + sysvalsanna), 0), 9, 'dato' \n" +
                       " FROM calculomensual WHERE anomes = @pPeriodo \n" +
                       " {mutual} \n" +
                ") as tabla \n" +
                "GROUP BY nombre, orden, detalle \n" +
                "ORDER BY orden\n";
            #endregion

            sql = sql.Replace("{condition}", condition);
            if (condition.Contains("itemtrabajador.contrato"))
                condition = condition.Replace("itemtrabajador.contrato", "contrato");

            sql = sql.Replace("{mutual}", condition);

            return sql;
        }

        //OBTENER TOTALES
        private Hashtable GetTotales(string pCodeConjunto, int pPeriodo)
        {
            string sql = "";
            string condicionConjunto = "";
            SqlCommand cmd;
            Hashtable data = new Hashtable();
            SqlDataReader rd;

            #region "PREPARACION QUERY"
            if (FiltroUsuario != "0")
            {
                if (pCodeConjunto != "")
                {
                    condicionConjunto = Conjunto.GetCondicionFromCode(pCodeConjunto);
                    //USA LAS DOS COSAS
                    sql = "SELECT ISNULL((SELECT sum(valorcalculado)), 0) as sumaAfp, " +
                        $"ISNULL((SELECT sum(valorcalculado) FROM itemtrabajador WHERE anomes = @pPeriodo AND coditem = 'SALUD' AND contrato IN(SELECT contrato FROM trabajador WHERE {condicionConjunto} AND {FiltroUsuario})), 0)  " +
                        $"-ISNULL((SELECT sum(valorcalculado) FROM itemtrabajador WHERE anomes = @pPeriodo AND coditem = 'CAJACOM' AND contrato IN(SELECT contrato FROM trabajador WHERE {condicionConjunto} AND {FiltroUsuario})), 0) as sumaSalud, " +
                        $"ISNULL((SELECT sum(valorcalculado) FROM itemtrabajador WHERE anomes = @pPeriodo AND(coditem = 'SCEMPLE' OR coditem = 'SCEMPRE' OR coditem = 'SEGINV') AND contrato IN(select contrato FROM trabajador WHERE {condicionConjunto} AND {FiltroUsuario})), 0) as sumaSeguros, " +
                        $"ISNULL((SELECT sum(valorcalculado) FROM itemtrabajador WHERE anomes = @pPeriodo AND coditem = 'ASIGFAM' AND contrato IN(SELECT contrato FROM trabajador WHERE {condicionConjunto} AND {FiltroUsuario})), 0) as sumaFamiliares, " +
                        $"ISNULL((SELECT sum(valorcalculado) FROM itemtrabajador WHERE anomes = @pPeriodo AND coditem = 'CAJACOM' AND contrato IN(SELECT contrato FROM trabajador WHERE {condicionConjunto} AND {FiltroUsuario})), 0) - " +
                        $"ISNULL((SELECT sum(valorcalculado) FROM itemtrabajador WHERE anomes = @pPeriodo AND coditem = 'ASIGFAM' AND contrato IN(select contrato FROM trabajador WHERE {condicionConjunto} AND {FiltroUsuario})), 0) as SumaCaja, " +
                        $"ISNULL((SELECT sum(valorcalculado) FROM itemtrabajador WHERE anomes = @pPeriodo AND coditem = 'MUTUALI' AND contrato IN(select contrato FROM trabajador WHERE {condicionConjunto} AND {FiltroUsuario})), 0) as SumaMutualidad, " +
                        $"ISNULL((SELECT sum(valorcalculado) FROM itemtrabajador WHERE anomes = @pPeriodo AND coditem = 'SALUD' AND contrato IN(select contrato FROM trabajador WHERE {condicionConjunto} AND {FiltroUsuario})), 0) - " +
                        $"ISNULL((SELECT sum(valorcalculado) FROM itemtrabajador WHERE anomes = @pPeriodo AND coditem = 'CAJACOM' AND contrato IN(select contrato FROM trabajador WHERE {condicionConjunto} AND {FiltroUsuario})), 0) + " +
                        $"ISNULL((SELECT sum(valorcalculado) FROM itemtrabajador WHERE anomes = @pPeriodo AND(coditem = 'SCEMPLE' OR coditem = 'SCEMPRE' OR coditem = 'SEGINV') AND contrato IN(select contrato FROM trabajador WHERE {condicionConjunto} AND {FiltroUsuario})), 0) + " +
                        $"ISNULL((SELECT sum(valorcalculado) FROM itemtrabajador WHERE anomes = @pPeriodo AND coditem = 'CAJACOM' AND contrato IN(select contrato FROM trabajador WHERE {condicionConjunto} AND {FiltroUsuario})),0) - " +
                        $"ISNULL((SELECT sum(valorcalculado) FROM itemtrabajador WHERE anomes = @pPeriodo AND coditem = 'ASIGFAM' AND contrato IN(select contrato FROM trabajador WHERE {condicionConjunto} AND {FiltroUsuario})), 0) + " +
                        $"ISNULL((SELECT sum(valorcalculado) FROM itemtrabajador WHERE anomes = @pPeriodo AND coditem = 'MUTUALI' AND contrato IN(select contrato FROM trabajador WHERE {condicionConjunto} AND {FiltroUsuario})), 0) + " +
                        $"ISNULL((SELECT sum(valorcalculado) FROM itemtrabajador WHERE anomes = @pPeriodo AND coditem = 'PREVISI' AND contrato IN(select contrato FROM trabajador WHERE {condicionConjunto} AND {FiltroUsuario})), 0) as total  " +
                        "FROM itemtrabajador " +
                        $"WHERE anomes = @pPeriodo AND coditem = 'PREVISI' AND contrato IN(SELECT contrato FROM trabajador {condicionConjunto} AND {FiltroUsuario})";
                }
                else
                {
                    sql = "SELECT ISNULL((SELECT sum(valorcalculado)), 0) as sumaAfp, " +
                         $"ISNULL((SELECT sum(valorcalculado) FROM itemtrabajador WHERE anomes = @pPeriodo AND coditem = 'SALUD' AND contrato IN(SELECT contrato FROM trabajador WHERE {FiltroUsuario})), 0)  " +
                         $"-ISNULL((SELECT sum(valorcalculado) FROM itemtrabajador WHERE anomes = @pPeriodo AND coditem = 'CAJACOM' AND contrato IN(SELECT contrato FROM trabajador WHERE {FiltroUsuario})), 0) as sumaSalud, " +
                         $"ISNULL((SELECT sum(valorcalculado) FROM itemtrabajador WHERE anomes = @pPeriodo AND(coditem = 'SCEMPLE' OR coditem = 'SCEMPRE' OR coditem = 'SEGINV') AND contrato IN(select contrato FROM trabajador WHERE {FiltroUsuario})), 0) as sumaSeguros, " +
                         $"ISNULL((SELECT sum(valorcalculado) FROM itemtrabajador WHERE anomes = @pPeriodo AND coditem = 'ASIGFAM' AND contrato IN(SELECT contrato FROM trabajador WHERE {FiltroUsuario})), 0) as sumaFamiliares, " +
                         $"ISNULL((SELECT sum(valorcalculado) FROM itemtrabajador WHERE anomes = @pPeriodo AND coditem = 'CAJACOM' AND contrato IN(SELECT contrato FROM trabajador WHERE {FiltroUsuario})), 0) - " +
                         $"ISNULL((SELECT sum(valorcalculado) FROM itemtrabajador WHERE anomes = @pPeriodo AND coditem = 'ASIGFAM' AND contrato IN(select contrato FROM trabajador WHERE {FiltroUsuario})), 0) as SumaCaja, " +
                         $"ISNULL((SELECT sum(valorcalculado) FROM itemtrabajador WHERE anomes = @pPeriodo AND coditem = 'MUTUALI' AND contrato IN(select contrato FROM trabajador WHERE {FiltroUsuario})), 0) as SumaMutualidad, " +
                         $"ISNULL((SELECT sum(valorcalculado) FROM itemtrabajador WHERE anomes = @pPeriodo AND coditem = 'SALUD' AND contrato IN(select contrato FROM trabajador WHERE   {FiltroUsuario})), 0) - " +
                         $"ISNULL((SELECT sum(valorcalculado) FROM itemtrabajador WHERE anomes = @pPeriodo AND coditem = 'CAJACOM' AND contrato IN(select contrato FROM trabajador WHERE {FiltroUsuario})), 0) + " +
                         $"ISNULL((SELECT sum(valorcalculado) FROM itemtrabajador WHERE anomes = @pPeriodo AND(coditem = 'SCEMPLE' OR coditem = 'SCEMPRE' OR coditem = 'SEGINV') AND contrato IN(select contrato FROM trabajador WHERE {FiltroUsuario})), 0) + " +
                         $"ISNULL((SELECT sum(valorcalculado) FROM itemtrabajador WHERE anomes = @pPeriodo AND coditem = 'CAJACOM' AND contrato IN(select contrato FROM trabajador WHERE {FiltroUsuario})),0) - " +
                         $"ISNULL((SELECT sum(valorcalculado) FROM itemtrabajador WHERE anomes = @pPeriodo AND coditem = 'ASIGFAM' AND contrato IN(select contrato FROM trabajador WHERE {FiltroUsuario})), 0) + " +
                         $"ISNULL((SELECT sum(valorcalculado) FROM itemtrabajador WHERE anomes = @pPeriodo AND coditem = 'MUTUALI' AND contrato IN(select contrato FROM trabajador WHERE {FiltroUsuario})), 0) + " +
                         $"ISNULL((SELECT sum(valorcalculado) FROM itemtrabajador WHERE anomes = @pPeriodo AND coditem = 'PREVISI' AND contrato IN(select contrato FROM trabajador WHERE {FiltroUsuario})), 0) as total  " +
                         "FROM itemtrabajador " +
                         $"WHERE anomes = @pPeriodo AND coditem = 'PREVISI' AND contrato IN (SELECT contrato FROM trabajador WHERE {FiltroUsuario})";
                }

            }
            else
            {
                //NO USA FILTRO USUAIO
                if (pCodeConjunto != "")
                {
                    condicionConjunto = Conjunto.GetCondicionFromCode(pCodeConjunto);
                    //USA CONJUNTO
                    sql = "SELECT ISNULL((SELECT sum(valorcalculado)), 0) as sumaAfp, " +
                      $"ISNULL((SELECT sum(valorcalculado) FROM itemtrabajador WHERE anomes = @pPeriodo AND coditem = 'SALUD' AND contrato IN(SELECT contrato FROM trabajador WHERE {condicionConjunto})), 0)  " +
                      $"-ISNULL((SELECT sum(valorcalculado) FROM itemtrabajador WHERE anomes = @pPeriodo AND coditem = 'CAJACOM' AND contrato IN(SELECT contrato FROM trabajador WHERE {condicionConjunto} )), 0) as sumaSalud, " +
                      $"ISNULL((SELECT sum(valorcalculado) FROM itemtrabajador WHERE anomes = @pPeriodo AND(coditem = 'SCEMPLE' OR coditem = 'SCEMPRE' OR coditem = 'SEGINV') AND contrato IN(select contrato FROM trabajador WHERE {condicionConjunto} )), 0) as sumaSeguros, " +
                      $"ISNULL((SELECT sum(valorcalculado) FROM itemtrabajador WHERE anomes = @pPeriodo AND coditem = 'ASIGFAM' AND contrato IN(SELECT contrato FROM trabajador WHERE {condicionConjunto} )), 0) as sumaFamiliares, " +
                      $"ISNULL((SELECT sum(valorcalculado) FROM itemtrabajador WHERE anomes = @pPeriodo AND coditem = 'CAJACOM' AND contrato IN(SELECT contrato FROM trabajador WHERE {condicionConjunto} )), 0) - " +
                      $"ISNULL((SELECT sum(valorcalculado) FROM itemtrabajador WHERE anomes = @pPeriodo AND coditem = 'ASIGFAM' AND contrato IN(select contrato FROM trabajador WHERE {condicionConjunto} )), 0) as SumaCaja, " +
                      $"ISNULL((SELECT sum(valorcalculado) FROM itemtrabajador WHERE anomes = @pPeriodo AND coditem = 'MUTUALI' AND contrato IN(select contrato FROM trabajador WHERE {condicionConjunto} )), 0) as SumaMutualidad, " +
                      $"ISNULL((SELECT sum(valorcalculado) FROM itemtrabajador WHERE anomes = @pPeriodo AND coditem = 'SALUD' AND contrato IN(select contrato FROM trabajador WHERE {condicionConjunto} )), 0) - " +
                      $"ISNULL((SELECT sum(valorcalculado) FROM itemtrabajador WHERE anomes = @pPeriodo AND coditem = 'CAJACOM' AND contrato IN(select contrato FROM trabajador WHERE {condicionConjunto} )), 0) + " +
                      $"ISNULL((SELECT sum(valorcalculado) FROM itemtrabajador WHERE anomes = @pPeriodo AND(coditem = 'SCEMPLE' OR coditem = 'SCEMPRE' OR coditem = 'SEGINV') AND contrato IN(select contrato FROM trabajador WHERE {condicionConjunto} )), 0) + " +
                      $"ISNULL((SELECT sum(valorcalculado) FROM itemtrabajador WHERE anomes = @pPeriodo AND coditem = 'CAJACOM' AND contrato IN(select contrato FROM trabajador WHERE {condicionConjunto} )),0) - " +
                      $"ISNULL((SELECT sum(valorcalculado) FROM itemtrabajador WHERE anomes = @pPeriodo AND coditem = 'ASIGFAM' AND contrato IN(select contrato FROM trabajador WHERE {condicionConjunto} )), 0) + " +
                      $"ISNULL((SELECT sum(valorcalculado) FROM itemtrabajador WHERE anomes = @pPeriodo AND coditem = 'MUTUALI' AND contrato IN(select contrato FROM trabajador WHERE {condicionConjunto} )), 0) + " +
                      $"ISNULL((SELECT sum(valorcalculado) FROM itemtrabajador WHERE anomes = @pPeriodo AND coditem = 'PREVISI' AND contrato IN(select contrato FROM trabajador WHERE {condicionConjunto} )), 0) as total  " +
                      "FROM itemtrabajador " +
                      $"WHERE anomes = @pPeriodo AND coditem = 'PREVISI' AND contrato IN (SELECT contrato FROM trabajador WHERE {condicionConjunto})";
                }
                else
                {
                    sql = "SELECT ISNULL((SELECT sum(valorcalculado)), 0) as sumaAfp, " +
                        "ISNULL((SELECT sum(valorcalculado) FROM itemtrabajador WHERE anomes = @pPeriodo AND coditem = 'SALUD'), 0)  " +
                        "-ISNULL((SELECT sum(valorcalculado) FROM itemtrabajador WHERE anomes = @pPeriodo AND coditem = 'CAJACOM'), 0) as sumaSalud, " +
                        "ISNULL((SELECT sum(valorcalculado) FROM itemtrabajador WHERE anomes = @pPeriodo AND(coditem = 'SCEMPLE' OR coditem = 'SCEMPRE' OR coditem = 'SEGINV') ), 0) as sumaSeguros, " +
                        "ISNULL((SELECT sum(valorcalculado) FROM itemtrabajador WHERE anomes = @pPeriodo AND coditem = 'ASIGFAM'), 0) as sumaFamiliares, " +
                        "ISNULL((SELECT sum(valorcalculado) FROM itemtrabajador WHERE anomes = @pPeriodo AND coditem = 'CAJACOM'), 0) - " +
                        "ISNULL((SELECT sum(valorcalculado) FROM itemtrabajador WHERE anomes = @pPeriodo AND coditem = 'ASIGFAM' ), 0) as SumaCaja, " +
                        "ISNULL((SELECT sum(valorcalculado) FROM itemtrabajador WHERE anomes = @pPeriodo AND coditem = 'MUTUALI' ), 0) as SumaMutualidad, " +
                        "ISNULL((SELECT sum(valorcalculado) FROM itemtrabajador WHERE anomes = @pPeriodo AND coditem = 'SALUD' ), 0) - " +
                        "ISNULL((SELECT sum(valorcalculado) FROM itemtrabajador WHERE anomes = @pPeriodo AND coditem = 'CAJACOM'), 0) + " +
                        "ISNULL((SELECT sum(valorcalculado) FROM itemtrabajador WHERE anomes = @pPeriodo AND(coditem = 'SCEMPLE' OR coditem = 'SCEMPRE' OR coditem = 'SEGINV')), 0) + " +
                        "ISNULL((SELECT sum(valorcalculado) FROM itemtrabajador WHERE anomes = @pPeriodo AND coditem = 'CAJACOM'),0) - " +
                        "ISNULL((SELECT sum(valorcalculado) FROM itemtrabajador WHERE anomes = @pPeriodo AND coditem = 'ASIGFAM' ), 0) + " +
                        "ISNULL((SELECT sum(valorcalculado) FROM itemtrabajador WHERE anomes = @pPeriodo AND coditem = 'MUTUALI'), 0) + " +
                        "ISNULL((SELECT sum(valorcalculado) FROM itemtrabajador WHERE anomes = @pPeriodo AND coditem = 'PREVISI'), 0) as total  " +
                        "FROM itemtrabajador " +
                        "WHERE anomes = @pPeriodo AND coditem = 'PREVISI'";
                }
            }
            #endregion
            
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pPeriodo", pPeriodo));
                        Console.WriteLine(sql);
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //GUARDAMOS VALORES EN TABLA HASH
                                data.Add("SumaAfp", Convert.ToDouble(rd["sumaafp"]));
                                data.Add("SumaSalud", Convert.ToDouble(rd["sumaSalud"]));
                                data.Add("SumaSeguros", Convert.ToDouble(rd["sumaSeguros"]));
                                data.Add("SumaFamiliares", Convert.ToDouble(rd["sumaFamiliares"]));
                                data.Add("SumaCaja", Convert.ToDouble(rd["SumaCaja"]));
                                data.Add("SumaMutualidad", Convert.ToDouble(rd["SumaMutualidad"]));
                                data.Add("Total", Convert.ToDouble(rd["total"]));
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

            return data;
        }

        private void txtPeriodo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) || char.IsNumber(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void btnConjunto_Click(object sender, EventArgs e)
        {
            FrmFiltroBusqueda filtro = new FrmFiltroBusqueda(true);
            filtro.opener = this;
            filtro.StartPosition = FormStartPosition.CenterParent;
            filtro.Show();
        }

        private void groupControl1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void MostrarReporte(bool editar = false) 
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            if (txtComboPeriodo.Properties.DataSource != null)
            {
                if (Calculo.PeriodoValido(Convert.ToInt32(txtComboPeriodo.EditValue)) == false)
                { XtraMessageBox.Show("Periodo ingresado no valido", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtComboPeriodo.Focus(); return; }

                if (ListadoPersona(Convert.ToInt32(txtComboPeriodo.EditValue)) == null)
                { XtraMessageBox.Show("No se encontró información", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                Empresa emp = new Empresa();
                emp.SetInfo();

                List<ReportePrevired> DataSum = new List<ReportePrevired>();
                string Cond = "";

                Cursor = Cursors.WaitCursor;

                if (PreviredData.Count == 0)
                { XtraMessageBox.Show("No se encontró información", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                if (PreviredData.Count > 0)
                {
                    DataSum = PreviredData.GroupBy(x => x.Entidad).
                        Select(x => new ReportePrevired
                        {
                            Cotizacion = x.Sum(y => y.Cotizacion),
                            CotizacionAfp = x.Sum(y => y.CotizacionAfp),
                            CotizacionIps = x.Sum(y => y.CotizacionIps),
                            CotizacionFonasa = x.Sum(y => y.CotizacionFonasa),
                            CotizacionIsapre = x.Sum(y => y.CotizacionIsapre),
                            AdicionalIsapre = x.Sum(y => y.AdicionalIsapre),
                            AhorroPrevisional = x.Sum(y => y.AhorroPrevisional),
                            AhorroVoluntario = x.Sum(y => y.AhorroVoluntario),
                            Prestamo = x.Sum(y => y.Prestamo),
                            SeguroVida = x.Sum(y => y.SeguroVida),
                            Leasing = x.Sum(y => y.Leasing),
                            Entidad = x.First().Entidad,
                            Caja = x.Sum(y => y.Caja),
                            AsignacionFam = x.Sum(y => y.AsignacionFam),
                            Mutal = x.Sum(y => y.Mutal),
                            SegAfiliado = x.Sum(y => y.SegAfiliado),
                            SegEmpresa = x.Sum(y => y.SegEmpresa),
                            SegInv = x.Sum(y => y.SegInv),
                            Tipo = x.First().Tipo,
                            Area = x.First().Area,
                            Cargo = x.First().Cargo,
                            CentroCosto = x.First().CentroCosto,
                            Sucursal = x.First().Sucursal


                        }).ToList();

                }

                Cursor = Cursors.Default;
                if (DataSum.Count == 0)
                { XtraMessageBox.Show("No se encontró información", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                if (txtConjunto.Text.Length > 0 && User.GetUserFilter().Length > 0)
                    //Cond = Conjunto.GetCondicionFromCode(User.GetUserFilter()) + ";" + Conjunto.GetCondicionFromCode(txtConjunto.Text);
                    Cond = Labour.Conjunto.GetDescConjunto(txtConjunto.Text);
                else if (User.GetUserFilter().Length > 0)
                    //Cond = Conjunto.GetCondicionFromCode(User.GetUserFilter());
                    Cond = Labour.Conjunto.GetDescConjunto(User.GetUserFilter());
                else if (User.GetUserFilter().Length == 0)
                    Cond = "";

                //RptPreviredRes r = new RptPreviredRes();
                //Reporte externo
                ReportesExternos.rptPreviredRes r = new ReportesExternos.rptPreviredRes();
                r.LoadLayoutFromXml(Path.Combine(fnSistema.RutaCarpetaReportesExterno, "rptPreviredRes.repx"));
                foreach (DevExpress.XtraReports.Parameters.Parameter parametro in r.Parameters)
                {
                    parametro.Visible = false;
                }

                r.Parameters["periodo"].Value = txtComboPeriodo.Text;
                r.Parameters["condicion"].Value = Cond;
                r.Parameters["empresa"].Value = emp.Razon;

                r.DataSource = DataSum;
                Documento doc = new Documento("", 0);
                //r.SaveLayoutToXml(Path.Combine(fnSistema.RutaCarpetaReportesExterno, "rptPreviredRes.repx"));
                if (editar)
                {
                    splashScreenManager1.ShowWaitForm();
                    //Se le pasa el waitform para que se cierre una vez cargado
                    DiseñadorReportes.MostrarEditorLimitado(r, "rptPreviredRes.repx", splashScreenManager1);
                }
                else 
                {
                    doc.ShowDocument(r);
                }
                

                //Hashtable data = new Hashtable();
                //RptResumenPrevired resumen = new RptResumenPrevired();
                //if (cbTodos.Checked == false)
                //{
                //    //SE CALCULA EN BASE A CONJUNTO (SUPONER QUE EL CONJUNTO INGRESADO EXISTE)
                //    if (txtConjunto.Text != "")
                //    {
                //        if (Conjunto.ExisteConjunto(txtConjunto.Text) == false)
                //        { XtraMessageBox.Show("Conjunto ingresado no existe", "informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); txtConjunto.Focus(); return; }

                //        //resumen.Parameters["periodo"].Value = txtComboPeriodo.Text;
                //        //resumen.Parameters["empresa"].Value = emp.Razon;
                //        //resumen.Parameters["empresa"].Visible = false;
                //        //resumen.Parameters["periodo"].Visible = false;                

                //        //DataSourceResumen(Convert.ToInt32(txtComboPeriodo.EditValue), resumen, txtConjunto.Text);
                //        SqlReportePrev(Convert.ToInt32(txtComboPeriodo.EditValue), txtConjunto.Text);
                //    }
                //    else
                //    {
                //        XtraMessageBox.Show("Por favor selecciona un conjunto valido", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //        txtConjunto.Focus();                     
                //        return;
                //    }
                //}
                //else
                //{
                //    resumen.Parameters["periodo"].Value = fnSistema.PrimerMayuscula(fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(Convert.ToInt32(txtComboPeriodo.EditValue))));
                //    resumen.Parameters["empresa"].Value = emp.Razon;
                //    resumen.Parameters["empresa"].Visible = false;
                //    resumen.Parameters["periodo"].Visible = false;

                //    //DataSourceResumen(Convert.ToInt32(txtComboPeriodo.EditValue), resumen, "");
                //    SqlReportePrev(Convert.ToInt32(txtComboPeriodo.EditValue), "");
                //}                              
            }
            else
            {
                XtraMessageBox.Show("Ingresa un periodo valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
        }

        private void btnEditarReporte_Click(object sender, EventArgs e)
        {
            MostrarReporte(editar:true);
        }
    }
}