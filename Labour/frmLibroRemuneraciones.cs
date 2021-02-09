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
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Configuration;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;

namespace Labour
{
    public partial class frmLibroRemuneraciones : DevExpress.XtraEditors.XtraForm, IConjuntosCondicionales
    {
        [DllImport("user32")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32")]
        static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        const int MF_BYCOMMAND = 0;
        const int MF_DISABLED = 2;
        const int SC_CLOSE = 0xF060;

        #region "CONJUNTO CONDICIONALES"
        public void CargarCodigoConjunto(string code)
        {
            //txtConjunto.Text = code;
            cbxSeleccionConjunto.EditValue= code;
        }
        #endregion

        //LISTA PARA GUARDAR DATOS 
        List<LibroRemuneracion> listado = new List<LibroRemuneracion>();

        DataTable TablaReporte = new DataTable();        

        //PARA SABER SI SE USO FILTRO DE BUSQUEDA O NO
        bool Filtro = false;

        //PARA SUMATORIAS
        double sumLiquido = 0, sumPago = 0, sumDctos = 0, sumEx = 0, sumImp = 0, sumfam = 0;

        //PARA GUARDAR PERIODO EN EVALUACION
        int periodoBusqueda = 0;

        //PARA GUARDAR EL FILTRO DEL USUARIO
        private string FiltroUsuario { get; set; } = User.GetUserFilter();

        //USUARIO PUEDE VER FICHAS PRIVADAS
        public bool ShowPrivadas { get; set; } = User.ShowPrivadas();

        public frmLibroRemuneraciones()
        {
            InitializeComponent();
        }

        private void cbActual_Load(object sender, EventArgs e)
        {
            var sm = GetSystemMenu(Handle, false);
            EnableMenuItem(sm, SC_CLOSE, MF_BYCOMMAND | MF_DISABLED);
            fnComboItem("SELECT codigo, descripcion FROM conjunto ORDER BY codigo", cbxSeleccionConjunto, "codigo", "descripcion", false, true);
            datoCombobox.spLlenaPeriodos("SELECT anomes FROM parametro ORDER BY anomes DESC", lookUpEdit1, "anomes", "anomes", true);
            int periodoActual = 0;
            periodoActual = Calculo.PeriodoEvaluar();
            //cbActual.Checked = true;
            //lookUpEdit1.ReadOnly = true;
            cbTodos.Checked = true;

            datoCombobox.AgrupaList(txtGrupo);
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;
            string field = "";
            DataTable reporteDataSet = new DataTable();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "rptlibrorem") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            ////BUSQUEDA DE TRABAJADOR DE ACUERDO AL NUMERO DE CONTRATO INGRESADO Y EL PERIODO SELECCIONADO
            //if (txtPeriodo.Text == "") { XtraMessageBox.Show("Por favor ingresa un periodo de busqueda", "Parametro", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtPeriodo.Focus(); return;}

            if (Calculo.PeriodoValido(Convert.ToInt32(lookUpEdit1.EditValue.ToString())) == false)
            { XtraMessageBox.Show("Periodo ingresado no existe", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); lookUpEdit1.Focus(); return; }

            //List<string> List = new List<string>();

            int periodo = 0;
            periodo = Convert.ToInt32(lookUpEdit1.EditValue.ToString());
            //string contrato = "";

            
            reporteDataSet = GetSQLString(periodo, cbxSeleccionConjunto.EditValue.ToString()).Tables[0];
            if (reporteDataSet.Rows.Count == 0)
            { XtraMessageBox.Show("No se encontraron registros", "Registros", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }    

            splashScreenManager1.ShowWaitForm();
            //rptLibroRemuneracionesFormaNueva reporteNuevo = new rptLibroRemuneracionesFormaNueva();
            //Reporte desde DLL
            LibroRemuneracionesExterno.rptLibroRemuneraciones reporteNuevo = new LibroRemuneracionesExterno.rptLibroRemuneraciones();
            reporteNuevo.Parameters["periodo"].Visible = false;
            reporteNuevo.Parameters["periodo"].Value = fnSistema.PrimerMayuscula(fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(periodo)));
            reporteNuevo.Parameters["condicion"].Visible = false;
            reporteNuevo.Parameters["condicion"].Value = Conjunto.GetCondicionReporte(cbxSeleccionConjunto.EditValue.ToString(), FiltroUsuario);
            splashScreenManager1.CloseWaitForm();
            reporteNuevo.DataSource = reporteDataSet;
            reporteNuevo.DataMember = "prueba";
            

            if (txtGrupo.Properties.DataSource != null)
            {
                reporteNuevo.Parameters["agrupacion"].Visible = false;
                reporteNuevo.Parameters["agrupacion"].Value = txtGrupo.Text;              

                if (txtGrupo.EditValue.ToString() != "0")
                {
                    reporteNuevo.groupFooterBand1.Visible = true;
                    //Create Group 
                    GroupHeaderBand GroupBand = new GroupHeaderBand { HeightF = 10 };
                    GroupBand.Borders = DevExpress.XtraPrinting.BorderSide.Bottom | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Left;
                    reporteNuevo.Bands.Add(GroupBand);


                    GroupFooterBand Footer = new GroupFooterBand { HeightF = 10 };
                    Footer.Borders = DevExpress.XtraPrinting.BorderSide.Bottom | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right;
                    reporteNuevo.Bands.Add(Footer);

                    GroupField groupField = new GroupField();
                    field = txtGrupo.Text.ToLower();
                    groupField.FieldName = field;                    
                    
                    //Centro de costo
                    //if (txtGrupo.EditValue.ToString() == "1")
                    //    field = "centrocosto";
                    //else if (txtGrupo.EditValue.ToString() == "2")
                    //    field = "sucursal";
                    //else if (txtGrupo.EditValue.ToString() == "3")
                    //    field = "area";
                    //else if (txtGrupo.EditValue.ToString() == "4")
                    //    field = "cargo";

                    GroupBand.GroupFields.Add(groupField);
                    

                    XRLabel labelGroup = new XRLabel { ForeColor = System.Drawing.Color.Black, WidthF = 748, TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter, /*Font = new Font(reporteNuevo.Font.FontFamily, 9, FontStyle.Underline)*/};
                    XRLabel labelDetail = new XRLabel { LocationF = new System.Drawing.PointF(30, 0) };

                    if (Settings.Default.UserDesignerOptions.DataBindingMode == DataBindingMode.Bindings)
                    {
                        labelGroup.DataBindings.Add("Text", null, $"prueba.{field}");
                    }
                    else
                    {
                        labelGroup.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", $"[prueba.{field}]"));
                    }
                    GroupBand.Controls.Add(labelGroup);
                }

          
            }         

            Documento doc = new Documento("", 0);
            doc.ShowDocument(reporteNuevo);

            //ReportPrintTool print = new ReportPrintTool(reporteNuevo);
            //print.ShowPreview();

            ////SI EL CHECKBOX TODOS ESTÁ SELECCIONADO, BUSCAMOS PARA TODOS LOS REGISTROS DEL PERIODO
            //if (cbTodos.Checked)
            //{
            //    //GENERAMOS LISTADO
            //    List = ListadoContratos(GetQuery(periodo, ""));

            //    if (List.Count == 0)
            //    { XtraMessageBox.Show("No se encontraron registros", "Registros", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            //    cargaLibro(periodo, List);
            //}
            //else
            //{
            //    //BUSCAMOS POR CONJUNTO
            //    if (txtConjunto.Text == "")
            //    { XtraMessageBox.Show("Por favor ingresa un conjunto válido", "Conjunto", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtConjunto.Focus(); return; }

            //    if(Conjunto.ExisteConjunto(txtConjunto.Text) == false)
            //    { XtraMessageBox.Show("Por favor ingresa un conjunto válido", "Conjunto", MessageBoxButtons.OK, MessageBoxIcon.Stop);txtConjunto.Focus(); return; }

            //    //GENERAMOS LISTADO
            //    List = ListadoContratos(GetQuery(periodo, txtConjunto.Text));

            //    if (List.Count == 0)
            //    { XtraMessageBox.Show("No se encontraron registros", "Registros", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            //    cargaLibro(periodo, List);

            //}
        }


        private void btnTablasExcel_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;
            DataTable reporteDataSet = new DataTable();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "rptlibrorem") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }


            if (Calculo.PeriodoValido(Convert.ToInt32(lookUpEdit1.EditValue.ToString())) == false)
            { XtraMessageBox.Show("Periodo ingresado no existe", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); lookUpEdit1.Focus(); return; }

            int periodo = 0;
            periodo = Convert.ToInt32(lookUpEdit1.EditValue.ToString());

            //Tabla de datos
            reporteDataSet = GetSQLStringExcel(periodo, cbxSeleccionConjunto.EditValue.ToString(), txtGrupo.Text.ToLower() == "sin agrupar" ? "" : txtGrupo.Text.ToLower()).Tables[0];

            fnSistema.SetNombreColumnasMayúsculas(reporteDataSet);

            if (reporteDataSet.Rows.Count == 0)
            { XtraMessageBox.Show("No se encontraron registros", "Registros", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (reporteDataSet == null)
            { XtraMessageBox.Show("No se encontraron resultados", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            //OBTIENE RUTA
            string rutaExcel = FileExcel.OpenDialogExcel(reporteDataSet, periodo, "LibroRemuneraciones");

            if (rutaExcel != null) 
            {
                splashScreenManager1.ShowWaitForm();
                try
                {
                    //CREA ARCHIVO EXCEL CON DATOS DE TABLA
                    if (FileExcel.CrearArchivoExcelDev(reporteDataSet, rutaExcel))
                    {
                        if (File.Exists(rutaExcel))
                        {
                            splashScreenManager1.CloseWaitForm();
                            XtraMessageBox.Show($"Archivo creado correctamente en {rutaExcel}", "Documento Pdf", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            DialogResult Pregunta = XtraMessageBox.Show("¿Deseas ver el documento?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (Pregunta == DialogResult.Yes)
                                System.Diagnostics.Process.Start(rutaExcel);
                        }
                    }
                    else
                    {
                        splashScreenManager1.CloseWaitForm();
                        { XtraMessageBox.Show("Error al crear tablas de datos. Asegúrese de no estar utilizando el archivo", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
                    }
                }
                catch (Exception ex)
                {
                    splashScreenManager1.CloseWaitForm();
                    { XtraMessageBox.Show("Error al crear tablas de datos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
                }
                
            }
        }

        #region "TABLAS ANTIGUAS"

        //private void testReporte()
        //{
        //    string sql = " " +
        //        "SELECT * FROM " +
        //        "( " +
        //        "SELECT contrato, itemTrabajador.coditem as item, valorcalculado as valor, libro.orden FROM itemtrabajador " +
        //        "INNER JOIN libro ON libro.coditem = itemTrabajador.coditem " +
        //        "INNER JOIN item ON item.coditem = itemTrabajador.coditem " +
        //        "WHERE itemTrabajador.anomes = 201808 AND visible = 1 AND itemTrabajador.coditem <> 'SCEMPRE' " +
        //        "UNION " +
        //        "SELECT itemTrabajador.contrato, itemTrabajador.coditem, syscicese, libro.orden " +
        //        "FROM itemtrabajador INNER JOIN libro ON libro.coditem = itemTrabajador.coditem " +
        //        "INNER JOIN calculoMensual ON calculoMensual.contrato = itemtrabajador.contrato AND calculoMensual.anomes = itemTrabajador.anomes " +
        //        "INNER JOIN item ON item.coditem = itemTrabajador.coditem " +
        //        "WHERE itemTrabajador.anomes = 201808 AND itemTrabajador.contrato = '125291112' AND itemTrabajador.coditem = 'SCEMPRE' " +
        //        "AND visible = 1 " +
        //        "UNION " +
        //        "SELECT itemTrabajador.contrato, itemtrabajador.coditem, sysfscese, libro.orden " +
        //        "FROM itemtrabajador INNER JOIN libro ON libro.coditem = itemTrabajador.coditem " +
        //        "INNER JOIN calculoMensual ON calculoMensual.contrato = itemtrabajador.contrato AND calculoMensual.anomes = itemTrabajador.anomes " +
        //        " INNER JOIN item ON itemTrabajador.coditem = item.coditem " +
        //        "WHERE itemTrabajador.anomes = 201808 AND itemTrabajador.contrato = '125291112' AND itemTrabajador.coditem = 'SCEMPRE' " +
        //        "AND visible = 1 " +
        //        "UNION " +
        //        "SELECT contrato, 'TOTAL HABERES', SUM(valorcalculado), 1111 FROM itemtrabajador WHERE anomes = 201808  AND tipo = 1 " +
        //        "GROUP BY contrato " +
        //        "UNION " +
        //        "SELECT contrato, 'TOTAL HABERES NO IMPONIBLES', SUM(valorcalculado), 1112 FROM itemtrabajador WHERE anomes = 201808 AND tipo = 2 " +
        //        "GROUP BY contrato " +
        //        "UNION " +
        //        " SELECT contrato, 'TOTAL DESCUENTOS', SUM(valorcalculado), 11132 " +
        //        "FROM itemtrabajador WHERE anomes = 201808  AND(tipo = 4 OR tipo = 5) AND(itemTrabajador.coditem <> 'SCEMPRE' AND itemTrabajador.coditem <> 'SEGINV') " +
        //        "GROUP BY contrato " +
        //        "UNION " +
        //        "SELECT contrato, 'TOTAL APORTES EMPRESA', SUM(valorcalculado), 1114 FROM itemtrabajador " +
        //        "WHERE anomes = 201808 AND(coditem = 'SEGINV' OR  coditem = 'SCEMPRE') " +
        //        "GROUP BY contrato " +
        //        "UNION " +
        //        "SELECT contrato, 'TOTAL PAGO', syspago, 1115 FROM calculomensual WHERE anomes = 201808 " +
        //        ") libro " +
        //        "ORDER BY contrato, orden";

        //    SqlCommand cmd;
        //    SqlDataAdapter ad = new SqlDataAdapter();
        //    DataSet ds = new DataSet();
        //    try
        //    {
        //        if (fnSistema.ConectarSQLServer())
        //        {
        //            using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
        //            {
        //                ad.SelectCommand = cmd;
        //                ad.Fill(ds, "data");

        //                if (ds.Tables[0].Rows.Count>0)
        //                {
        //                    RptLibroRemuneracionesv2 libro = new RptLibroRemuneracionesv2();
        //                    libro.DataSource = ds.Tables[0];
        //                    libro.DataMember = "data";

        //                    Documento d = new Documento("", 0);
        //                    d.ShowDocument(libro);
        //                }
        //            }

        //            cmd.Dispose();
        //            fnSistema.sqlConn.Close();
        //            ad.Dispose();
        //        }
        //    }
        //    catch (SqlException ex)
        //    {
        //        XtraMessageBox.Show(ex.Message);
        //    }
        //}

        #region "MANEJO DE DATOS"
        //BUSQUEDA DE DATOS A TRAVÉS DE NUMERO DE CONTRATO
        //PARAMETROS DE ENTRADA: PERIODO Y CONTRATO     
        //private void BuscarDatosTrabajador(int periodo, string contrato)
        //{
        //    //LISTADO CONTRATOS CON FILTRO USUARIO
        //    double syscicest = 0, syscicese = 0, sysfscese, diasTr = 0, systimp = 0, sysliq = 0, diaslic = 0, sanna = 0;
        //    List<string> ListaFiltrada = new List<string>();
        //    ListaFiltrada = ListadoContratosFiltro(periodo);

        //    Libro.ListadoLiquidos.Clear();
        //    Libro.ListHeaderRow.Clear();
        //    Libro.TotalImponibleLibro = 0;
        //    if (ListaFiltrada.Count == 0) { XtraMessageBox.Show("Ha ocurrido un error al procesar la informacion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);return; }

        //    if (ExisteContrato(contrato, periodo) && ExisteContratoFiltro(ListaFiltrada, contrato))
        //    {
        //        TablaReporte.Clear();

        //        syscicest = Calculo.GetValueFromCalculoMensaul(contrato, periodo, "syscicest");
        //        syscicese = Calculo.GetValueFromCalculoMensaul(contrato, periodo, "syscicese");
        //        sysfscese = Calculo.GetValueFromCalculoMensaul(contrato, periodo, "sysfscese");
        //        diasTr = Calculo.GetValueFromCalculoMensaul(contrato, periodo, "sysdiastr");
        //        systimp = Calculo.GetValueFromCalculoMensaul(contrato, periodo, "systimp");
        //        sysliq = Calculo.GetValueFromCalculoMensaul(contrato, periodo, "sysliq");
        //        diaslic = Calculo.GetValueFromCalculoMensaul(contrato, periodo, "sysdiaslic");
        //        //sanna = Calculo.GetValueFromCalculoMensaul(contrato, periodo, "sysvalsanna");

        //        periodoBusqueda = periodo;

        //        //VALORES DESDE TABLA HASH

        //        Hashtable tablaDatos = new Hashtable();
        //        tablaDatos = DataEmpleado(contrato, periodo);

        //        //TABLA CONFIGURACION LIBRO                
        //        TablaReporte = Libro.GetTablaFromLibro();

        //        DataRow row = TablaReporte.NewRow();
        //        //RECORREMOS TABLA CONFIGURACION
        //        foreach (DataColumn column in TablaReporte.Columns)
        //        {
        //            if (TieneItem(tablaDatos, column.ColumnName.ToLower()))
        //            {
        //                row[column.ColumnName] = tablaDatos[column.ColumnName.ToLower()];

        //                //SEGURO EMPLEADO
        //                if (column.ColumnName.ToLower() == "scemple")
        //                {
        //                    row[column.ColumnName] = syscicest;
        //                }

        //                //SEGURO EMPRESA
        //                if (column.ColumnName.ToLower() == "scempre")
        //                {
        //                    row[column.ColumnName] = syscicese + sysfscese;
        //                }                       

        //            }
        //            else
        //            {
        //                row[column.ColumnName] = "0";

        //                if (column.Caption == "contrato")
        //                    row[column.ColumnName] = contrato;
        //                if (column.Caption == "nombre")
        //                    row[column.ColumnName] = NombreTrabajador(contrato, periodoBusqueda);
        //                if (column.Caption == "diastrabajados")
        //                    row[column.ColumnName] = diasTr;
        //            }                       
        //        }

        //        //OBTENER EL NOMBRE DEL TRABAJADOR
        //        ListadoLibroHeader(NombreTrabajador(contrato, periodoBusqueda), contrato, diasTr, periodo, diaslic);

        //        Libro.TotalImponibleLibro = Libro.TotalImponibleLibro + systimp;

        //        Libro.ListadoLiquidos.Add(sysliq);

        //        //AGREGAMOS FILA AL DATATABLE
        //        TablaReporte.Rows.Add(row);

        //        Libro.TablaLibroRemuneraciones = TablaReporte;

        //        //splashScreenManager1.CloseWaitForm();
        //        //mostramos reporte
        //        XtraReportDemo reporte = new XtraReportDemo();

        //        reporte.Parameters["periodo"].Visible = false;
        //        reporte.Parameters["periodo"].Value = fnSistema.PrimerMayuscula(fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(periodo)));


        //    }
        //    else
        //    {
        //        XtraMessageBox.Show("Contrato no existe", "Busqueda", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        //txtContrato.Focus();
        //    }           
        //}

        //VERSION 2
        //private void cargaLibro(int Periodo, List<string> pListado)
        //{
        //    splashScreenManager1.ShowWaitForm();
        //    TablaReporte.Clear();
        //    string sql = "", condUser = "";
        //    double syscicest = 0, syscicese = 0, sysfscese = 0, sysdiastr = 0, sysdiaslic = 0, sanna = 0;

        //    Hashtable tablaDatos = new Hashtable();

        //    Libro.ListHeaderRow.Clear();
        //    Libro.ListadoLiquidos.Clear();
        //    Libro.TotalImponibleLibro = 0;

        //    //GENERA LAS COLUMNAS EN BASE A LOS NOMBRES DE LOS CODIGO DE ITEMS
        //    TablaReporte = Libro.GetTablaFromLibro();            

        //    //RECORREMOS CONTRATO
        //    foreach (var contrato in pListado)
        //    {
        //        tablaDatos = DataEmpleado(contrato, Periodo);
        //        sysdiastr = Calculo.GetValueFromCalculoMensaul(contrato, Periodo, "sysdiastr");
        //        sysdiaslic = Calculo.GetValueFromCalculoMensaul(contrato, Periodo, "sysdiaslic");
        //        //sanna = Calculo.GetValueFromCalculoMensaul(contrato, Periodo, "sysvalsanna");

        //        //CREAMOS UNA FILA
        //        DataRow row = TablaReporte.NewRow();

        //        //RECORREMOS COLUMNAS
        //        foreach (DataColumn column in TablaReporte.Columns)
        //        {
        //            //PREGUNTAMOS SI EXISTE ITEM EN TABLA HASH
        //            if (TieneItem(tablaDatos, column.ColumnName.ToLower()))
        //            {

        //                //SI EXISTE REEMPLAZAMOS POR EL VALOR
        //                row[column.ColumnName] = tablaDatos[column.ColumnName.ToLower()];
        //                //SEGURO EMPLEADO
        //                if (column.ColumnName.ToLower() == "scemple")
        //                {                           
        //                    //row[column.ColumnName] = varSistema.ObtenerValorLista("syscicest");
        //                    row[column.ColumnName] = Calculo.GetValueFromCalculoMensaul(contrato, Periodo, "syscicest");
        //                }

        //                //SEGURO EMPRESA
        //                if (column.ColumnName.ToLower() == "scempre")
        //                {
        //                    syscicese = Calculo.GetValueFromCalculoMensaul(contrato, Periodo, "syscicese");
        //                    sysfscese = Calculo.GetValueFromCalculoMensaul(contrato, Periodo, "sysfscese");
        //                    row[column.ColumnName] = syscicese + sysfscese;
        //                }
        //            }
        //            else
        //            {                        
        //                row[column.ColumnName] = "0";

        //                if (column.Caption == "contrato")
        //                    row[column.ColumnName] = contrato;
        //                if (column.Caption == "nombre")
        //                    row[column.ColumnName] = NombreTrabajador(contrato, Periodo);
        //                if (column.Caption == "diastrabajados")
        //                    row[column.ColumnName] = sysdiastr;
        //            }
        //        }                

        //        //OBTENER EL NOMBRE DEL TRABAJADOR
        //        ListadoLibroHeader(NombreTrabajador(contrato, Periodo), contrato, sysdiastr, Periodo, sysdiaslic);

        //        Libro.TotalImponibleLibro = Libro.TotalImponibleLibro + Calculo.GetValueFromCalculoMensaul(contrato, Periodo, "systimp");

        //        Libro.ListadoLiquidos.Add(Calculo.GetValueFromCalculoMensaul(contrato, Periodo, "sysliq"));
        //        //Libro.ListadoLiquidos.Add(varSistema.ObtenerValorLista("sysliq"));

        //        //AGREGAMOS FILA AL DATATABLE
        //        TablaReporte.Rows.Add(row);
        //    }//END FOREACH  


        //    //gridControl1.DataSource = GeneraTabla(Convert.ToInt32(txtPeriodo.Text));
        //    Libro.TablaLibroRemuneraciones = TablaReporte;


        //    //mostramos reporte
        //    //XtraReportDemo reporte = new XtraReportDemo();
        //    splashScreenManager1.CloseWaitForm();
        //    rptLibroRemuneracionesFormaNueva reporteNuevo = new rptLibroRemuneracionesFormaNueva();
        //    reporteNuevo.Parameters["periodo"].Visible = false;
        //    reporteNuevo.Parameters["periodo"].Value = fnSistema.PrimerMayuscula(fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(Periodo)));
        //    reporteNuevo.DataSource = GetSQLString(Periodo, txtConjunto.Text).Tables[0];
        //    reporteNuevo.DataMember = "prueba";
        //    ReportPrintTool print = new ReportPrintTool(reporteNuevo);
        //    print.ShowPreview();

        //    //reporte.Parameters["periodo"].Value = Periodo;


        //    //reporte.DataSource = FillDataset(TablaReporte);
        //    //reporte.DataMember = ((DataSet)reporte.DataSource).Tables[0].TableName;

        //    //InitBands(reporte);

        //    //CreateTable(reporte);


        //}

        //LISTADO CON TODOS LOS CONTRATOS
        //private List<string> ListadoContratos(string pSql)
        //{
        //    SqlCommand cmd;
        //    SqlDataReader rd;
        //    List<string> lista = new List<string>();
        //    try
        //    {
        //        if (fnSistema.ConectarSQLServer())
        //        {
        //            using (cmd = new SqlCommand(pSql, fnSistema.sqlConn))
        //            {
        //                rd = cmd.ExecuteReader();
        //                if (rd.HasRows)
        //                {
        //                    while (rd.Read())
        //                    {
        //                        lista.Add((string)rd["contrato"]);
        //                    }
        //                }

        //                cmd.Dispose();
        //                rd.Close();
        //                fnSistema.sqlConn.Close();
        //            }
        //        }
        //    }
        //    catch (SqlException ex)
        //    {
        //        XtraMessageBox.Show(ex.Message);
        //    }

        //    return lista;
        //}

        //OBTENER VALORES DE CALCULO DESDE TABLA CALCULO
        //private Hashtable DataEmpleado(string contrato, int periodo)
        //{
        //    Hashtable data = new Hashtable();
        //    string sql = "select coditem as item, SUM(valorcalculado) as calculado FROM itemtrabajador " +
        //            "WHERE contrato = @contrato AND anomes=@periodo GROUP BY coditem ";

        //    SqlCommand cmd;
        //    SqlDataReader rd;
        //    try
        //    {
        //        if (fnSistema.ConectarSQLServer())
        //        {
        //            using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
        //            {
        //                //PARAMETROS
        //                cmd.Parameters.Add(new SqlParameter("@contrato", contrato));
        //                cmd.Parameters.Add(new SqlParameter("@periodo", periodo));

        //                rd = cmd.ExecuteReader();
        //                if (rd.HasRows)
        //                {
        //                    while (rd.Read())
        //                    {
        //                        //AGREGAMOS EL ITEM COMO NOMBRE DEL DICCIONARIO JUNTO CON SU VALOR
        //                        data.Add(((string)rd["item"]).ToLower(), (decimal)rd["calculado"]);
        //                        //data.Add("contratoTrabajadorReporteItem", (string)rd["contratoTrabajador"]);
        //                    }
        //                }
        //                else
        //                {
        //                    XtraMessageBox.Show("No hay registros!");
        //                }
        //            }
        //            cmd.Dispose();
        //            fnSistema.sqlConn.Close();
        //            rd.Close();
        //        }
        //    }
        //    catch (SqlException ex)
        //    {
        //        XtraMessageBox.Show(ex.Message);
        //    }

        //    //RETORNAMOS LA TABLA HASH
        //    return data;
        //}

        ////OBTENER VALORES DE CALCULO DESDE TABLA CALCULO
        //private Hashtable DataEmpleado(string contrato, int periodo)
        //{
        //    Hashtable data = new Hashtable();
        //    string sql = "select coditem as item, SUM(valorcalculado) as calculado FROM itemtrabajador " +
        //            "WHERE contrato = @contrato AND anomes=@periodo GROUP BY coditem ";            

        //    SqlCommand cmd;
        //    SqlDataReader rd;
        //    try
        //    {
        //        if (fnSistema.ConectarSQLServer())
        //        {
        //            using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
        //            {
        //                //PARAMETROS
        //                cmd.Parameters.Add(new SqlParameter("@contrato", contrato));
        //                cmd.Parameters.Add(new SqlParameter("@periodo", periodo));

        //                rd = cmd.ExecuteReader();
        //                if (rd.HasRows)
        //                {  
        //                    while (rd.Read())
        //                    {                             
        //                     //AGREGAMOS EL ITEM COMO NOMBRE DEL DICCIONARIO JUNTO CON SU VALOR
        //                     data.Add(((string)rd["item"]).ToLower(), (decimal)rd["calculado"]);                           
        //                    }                           
        //                }
        //                else
        //                {
        //                    XtraMessageBox.Show("No hay registros!");
        //                }
        //            }
        //            cmd.Dispose();
        //            fnSistema.sqlConn.Close();
        //            rd.Close();
        //        }
        //    }
        //    catch (SqlException ex)
        //    {
        //        XtraMessageBox.Show(ex.Message);
        //    }

        //    //RETORNAMOS LA TABLA HASH
        //    return data;
        //}

        //VERIFICAR QUE EL TRABAJADOR TIENE ESE ITEM (BUSCAMOS EN TABLA HASH)





        //NOMBRE COMPLETO TRABAJADOR
        //private string NombreTrabajador(string contrato, int periodo)
        //{
        //    string nombre = "";
        //    string sql = "SELECT CONCAT(nombre, ' ', apepaterno, ' ', apematerno) as name FROM trabajador " +
        //        "WHERE contrato=@contrato AND anomes=@periodo";
        //    SqlCommand cmd;
        //    SqlDataReader rd;
        //    try
        //    {
        //        if (fnSistema.ConectarSQLServer())
        //        {
        //            using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
        //            {
        //                //PARAMETROS
        //                cmd.Parameters.Add(new SqlParameter("@contrato", contrato));
        //                cmd.Parameters.Add(new SqlParameter("@periodo", periodo));

        //                rd = cmd.ExecuteReader();
        //                if (rd.HasRows)
        //                {
        //                    while (rd.Read())
        //                    {
        //                        nombre = (string)rd["name"];
        //                    }
        //                }
        //                else
        //                {
        //                    XtraMessageBox.Show("No hay registros!");
        //                }
        //            }
        //            cmd.Dispose();
        //            fnSistema.sqlConn.Close();
        //            rd.Close();
        //        }
        //    }
        //    catch (SqlException ex)
        //    {
        //        XtraMessageBox.Show(ex.Message);
        //    }
        //    return nombre;
        //}

        //GENERA LISTADO CON NOMBRE Y N° CONTRATO
        //private void ListadoLibroHeader(string pNombre, string pContrato, double Diastr, int Periodo, double DiasLic)
        //{
        //    Libro.ListHeaderRow.Add(new Libro() { ContratoHeader = pContrato, NameHeader = pNombre, DiasHeader = Diastr.ToString("n1"), PeriodoHeader = Periodo, DiasLicHeader = DiasLic.ToString("n1")});
        //}

        //EXISTE CONTRATO EN ESE PERIODO
        //private bool ExisteContrato(string contrato, int periodo)
        //{
        //    string sql = "SELECT contrato FROM trabajador WHERE contrato=@contrato AND anomes=@periodo";
        //    SqlCommand cmd;
        //    SqlDataReader rd;
        //    bool existe = false;
        //    try
        //    {
        //        if (fnSistema.ConectarSQLServer())
        //        {
        //            using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
        //            {
        //                //PARAMETROS
        //                cmd.Parameters.Add(new SqlParameter("@contrato", contrato));
        //                cmd.Parameters.Add(new SqlParameter("@periodo", periodo));

        //                rd = cmd.ExecuteReader();
        //                if (rd.HasRows)
        //                {
        //                    //EXISTE
        //                    existe = true;
        //                }
        //                else
        //                {
        //                    existe = false;
        //                }
        //            }
        //            cmd.Dispose();
        //            fnSistema.sqlConn.Close();
        //            rd.Close();
        //        }
        //    }
        //    catch (SqlException ex)
        //    {
        //        XtraMessageBox.Show(ex.Message);
        //    }

        //    return existe;           
        //}        

        //TRAER LISTADO DE CONTRATOS DE ACUERDO A FILTRO USUARIO
        //private List<string> ListadoContratosFiltro(int periodo)
        //{
        //    List<string> lista = new List<string>();
        //    string sql = "", condUser = "";

        //    //SI PROPIEDAD SHOWPRIVADAS ES FALSE, USUARIO NO TIENE PERMISO PARA VER FICHAS PRIVADAS
        //    if (FiltroUsuario == "0")
        //        sql = string.Format("SELECT contrato FROM trabajador WHERE anomes={0} " + (ShowPrivadas==false?" AND privado=0":"") + " ORDER BY apepaterno", periodo);
        //    else
        //    {
        //        condUser = Conjunto.GetCondicionFromCode(FiltroUsuario);
        //        sql = string.Format("SELECT contrato FROM trabajador WHERE anomes={0} AND {1} " + (ShowPrivadas==false?" AND privado=0":"") + " ORDER BY apepaterno", periodo, condUser);
        //    }                

        //    SqlCommand cmd;
        //    SqlDataReader rd;

        //    try
        //    {
        //        if (fnSistema.ConectarSQLServer())
        //        {
        //            using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
        //            {
        //                rd = cmd.ExecuteReader();
        //                if (rd.HasRows)
        //                {
        //                    while (rd.Read())
        //                    {
        //                        //GUARDAMOS DATOS EN LISTA
        //                        lista.Add((string)rd["contrato"]);
        //                    }
        //                }

        //                cmd.Dispose();
        //                fnSistema.sqlConn.Close();
        //                rd.Close();
        //            }
        //        }
        //    }
        //    catch (SqlException ex)
        //    {
        //        XtraMessageBox.Show(ex.Message);
        //    }

        //    return lista;
        //}

        //VERIFICAR QUE CONTRATO ESTE DENTRO DE LISTADO DE CONTRATOS FILTRO
        //private bool ExisteContratoFiltro(List<string> Lista, string contrato)
        //{
        //    if (Lista.Count>0)
        //    {
        //        foreach (var item in Lista)
        //        {
        //            if (item == contrato)
        //            {
        //                return true;
        //            }
        //        }

        //        return false;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        //SUMATORIAS BONO
        //private double GetBonos(string pContrato, int Pperiodo)
        //{
        //    double value = 0;
        //    string formula = FormulaSistema.GetValueFormula("FBONREM");

        //    Expresion ex = new Expresion(formula);
        //    ex.setContrato(pContrato);
        //    ex.setPeriodoEmpleado(Pperiodo);

        //    value = ex.CalculoSupremo();

        //    return value;
        //}

        //GENERA DATATABLE 
        //private DataTable GeneraTabla(int pPeriodo)
        //{
        //    string sql = "", sqldata = "", FieldName = "", contrato = "";
        //    double FieldValue = 0;
        //    SqlCommand cmd;
        //    SqlDataReader rd;

        //    #region "OBTENER CONTRATOS"
        //    if (FiltroUsuario != "0")
        //    {
        //        FiltroUsuario = Conjunto.GetCondicionFromCode(FiltroUsuario);
        //        sql = $"SELECT contrato FROM trabajador WHERE anomes={pPeriodo} AND {FiltroUsuario}";
        //    }
        //    else
        //    {
        //        sql = $"SELECT contrato FROM trabajador WHERE anomes={pPeriodo}";
        //    }

        //    List<string> Listado = new List<string>();

        //    /*OBTENER LISTADO DE CONTRATOS*/
        //    try
        //    {
        //        if (fnSistema.ConectarSQLServer())
        //        {
        //            using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
        //            {
        //                rd = cmd.ExecuteReader();
        //                if (rd.HasRows)
        //                {
        //                    while (rd.Read())
        //                    {
        //                        //CONTRATOS
        //                        Listado.Add(rd["contrato"].ToString());
        //                    }
        //                }

        //                cmd.Dispose();
        //                fnSistema.sqlConn.Close();
        //                rd.Close();
        //            }
        //        }
        //    }
        //    catch (SqlException ex)
        //    {
        //        XtraMessageBox.Show(ex.Message);
        //    }

        //    #endregion

        //    //OBTENEMOS ESTRUCTURA DATATABLE (SOLO COLUMNAS)
        //    DataTable tablaSinData = Libro.GetColumnsTable();

        //    sqldata = "SELECT contrato, itemTrabajador.coditem, SUM(valorcalculado) as valorcalculado, libro.orden FROM itemtrabajador " +
        //              " INNER JOIN libro ON libro.coditem = itemTrabajador.coditem " +
        //              " WHERE anomes = @pPeriodo AND visible = 1 AND contrato = @pContrato " +
        //              " GROUP BY itemTrabajador.coditem, contrato, libro.orden " +
        //              " UNION " +
        //              " SELECT contrato, 'LIQPAGO', syspago, 0 " +
        //              " FROM calculoMensual WHERE anomes = @pPeriodo AND contrato = @pContrato ";

        //    //RECORRER LISTADO DE CONTRATOS
        //    if (Listado.Count>0)
        //    {
        //        foreach (string data in Listado)
        //        {
        //            //POR CADA CONTRATO VAMOS A OBTENER LOS VALORES DE ITEMS CORRESPONDIENTES
        //            try
        //            {
        //                if (fnSistema.ConectarSQLServer())
        //                {
        //                    using (cmd = new SqlCommand(sqldata, fnSistema.sqlConn))
        //                    {
        //                        //PARAMETROS
        //                        cmd.Parameters.Add(new SqlParameter("@pPeriodo", pPeriodo));
        //                        cmd.Parameters.Add(new SqlParameter("@pContrato", data));

        //                        rd = cmd.ExecuteReader();
        //                        if (rd.HasRows)
        //                        {
        //                            DataRow Fila = tablaSinData.NewRow();
        //                            while (rd.Read())
        //                            {
        //                                FieldName = rd["coditem"].ToString().ToLower();
        //                                FieldValue = Convert.ToDouble(rd["valorcalculado"]);

        //                                /*ITERAMOS COLUMNAS*/
        //                                foreach (DataColumn column in tablaSinData.Columns)
        //                                {
        //                                    if (column.ColumnName.ToLower() == FieldName)
        //                                        Fila[column.ColumnName] = FieldValue;
        //                                }
        //                            }

        //                            /*ITERADOS TODOS LOS ITEMS GUARDAMOS EN TABLA LA NUEVA FILA*/
        //                            tablaSinData.Rows.Add(Fila);
        //                        }
        //                    }
        //                    cmd.Dispose();
        //                    fnSistema.sqlConn.Close();
        //                    rd.Close();
        //                }
        //            }
        //            catch (SqlException ex)
        //            {
        //                XtraMessageBox.Show(ex.Message);
        //            }
        //        }
        //    }

        //    return tablaSinData;                    
        //}

        //GET QUERY
        //PARA LISTADO DE CONTRATOS
        //private string GetQuery(int pPeriodo, string pConjunto)
        //{
        //    string sql = "";

        //    //BUSCAMOS POR TODOS LOS TRABAJADORES
        //    if (cbTodos.Checked)
        //    {                
        //        sql = $"SELECT contrato FROM trabajador WHERE anomes={pPeriodo} " +
        //              $"{(FiltroUsuario != "0"? ($"AND {Conjunto.GetCondicionFromCode(FiltroUsuario)}"):"")} " +
        //              $"{(ShowPrivadas == false? "AND privado=0":"")} " +
        //              $" ORDER BY contrato";
        //    }
        //    else
        //    {
        //        //CONJUNTO DE BUSQUEDA
        //        sql = $"SELECT contrato FROM trabajador WHERE anomes={pPeriodo} " +
        //             $"{(FiltroUsuario != "0" ? ($"AND {Conjunto.GetCondicionFromCode(FiltroUsuario)}") : "")} " +
        //             $"{(ShowPrivadas == false ? "AND privado=0" : "")} " +
        //             $"{(pConjunto == ""? "":($" AND {Conjunto.GetCondicionFromCode(pConjunto)}"))}" +
        //             $" ORDER BY contrato";
        //    }

        //    return sql;
        //}



        #endregion

        #region "REPORTE"

        private void CreateTable(XtraReport Report)
        {
            DataSet ds = ((DataSet)Report.DataSource);
            int colCount = ds.Tables[0].Columns.Count;
            int colWidth = (Report.PageWidth - (Report.Margins.Left + Report.Margins.Right)) / colCount;
            int factor = 10, res = 0, CantidadFilas = 0, contadorFactor = 1, ini = 0, fin = 0;
            int rangoInicio = 0, n = 0;
            double Imp = 0, Nopimp = 0, Leyes = 0, Dcto = 0, Aporte = 0; 

            //TABLA HEADER
            XRTable tableHeader = new XRTable();           
            
            tableHeader.HeightF = 20;
            tableHeader.WidthF = (Report.PageWidth - (Report.Margins.Left + Report.Margins.Right));
            XRTableRow headerRow = new XRTableRow();
            headerRow.WidthF = tableHeader.WidthF;           
            //tableHeader.Rows.Add(headerRow);

            CantidadFilas = colCount / factor;
            res = colCount % factor;

            //RANGO INDICE INICIO ULTIMA FILA
            rangoInicio = ((CantidadFilas - 1) * factor);

            tableHeader.BeginInit();

            //TABLA PARA DATA
            XRTable tableDetail = new XRTable();
            tableDetail.KeepTogether = true;
            tableDetail.HeightF = 20;
            tableDetail.WidthF = (Report.PageWidth - (Report.Margins.Left - Report.Margins.Right));
            XRTableRow detailRow = new XRTableRow();
            detailRow.WidthF = tableDetail.WidthF;         

            tableDetail.BeginInit();
            ini = factor * n;
            fin = (ini + factor) - 1;
            
            //CREAMOS CELDAS Y GUARDAMOS DATA
            int count = 0;
            for (int i = 0; i < colCount; i++)
            {
                count++;
   
                XRTableCell headerCell = new XRTableCell();
                headerCell.WidthF = Report.PageWidth/colCount;
                headerCell.Text = ds.Tables[0].Columns[i].Caption;
                headerCell.Borders = DevExpress.XtraPrinting.BorderSide.All;
                headerCell.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;

                XRTableCell detailCell = new XRTableCell();
                detailCell.WidthF = Report.PageWidth / colCount;
                detailCell.DataBindings.Add("Text", null, ds.Tables[0].Columns[i].Caption);
                detailCell.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleJustify;

                detailCell.BeforePrint += new PrintEventHandler(detailcell_BeforePrint);
                
                //detailCell.Borders = DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right;                

                //AGREGAMOS CELDAS A FILA
                headerRow.Cells.Add(headerCell);
                detailRow.Cells.Add(detailCell);

                if (colCount % factor == 0)
                {
                    //SE PUEDEN GENERAR COLUMNAS IGUALES EN BASE A FACTOR Y NO HAY COLUMNAS RESTANTES
                    //GENERAMOS NUEVA FALIDA DE ACUERDO A FACTOR
                    if (count % factor == 0)
                    {
                        tableHeader.Rows.Add(headerRow);
                        tableDetail.Rows.Add(detailRow);
                       
                        headerRow = new XRTableRow();
                        detailRow = new XRTableRow();                        

                        contadorFactor++;
                        //INICIO RANGO FILA
                        n++;
                        ini = factor * n;
                        fin = (ini + factor) - 1;
                    }

                    //SI ES LA ULTIMA FILA GENERAMOS BORDERS BOTTOM
                    //RANGO INDICE ULTIMA FILA
                    if (i >= rangoInicio && i <= colCount - 1)
                        detailCell.Borders = DevExpress.XtraPrinting.BorderSide.Bottom;                   

                    if (i == ini)
                        detailCell.Borders = DevExpress.XtraPrinting.BorderSide.Left;
                    if (i == fin)
                        detailCell.Borders = DevExpress.XtraPrinting.BorderSide.Right;                       
                }
                else
                {
                    //LA CANTIDAD DE FILAS NO SE PUEDE SEPARAR UNIFORMEMENTE
                    //HAY COLUMNAS RESTANTES

                    //@CASE1: LA CANTIDAD DE COLUMNAS ES MENOR AL FACTOR(SE GENERA SOLO UNA FILA)
                    if (colCount <= factor)
                    {
                        //SI LLEGAMOS AL FINAL GUARDAMOS FILA
                        if (count == colCount - 1)
                        {
                            tableHeader.Rows.Add(headerRow);
                            tableDetail.Rows.Add(detailRow);
                        }

                        //POR CADA COLUMNA GENERAMOS BORDERS BOTTOM
                        //if (i <= colCount - 1)
                        detailCell.Borders = DevExpress.XtraPrinting.BorderSide.All;
                    }
                    //@CASE2: HAY MAS DE UNA FILA 
                    else
                    {
                        if (i < (CantidadFilas * factor))
                        {
                            if (i == ini)
                                detailCell.Borders = DevExpress.XtraPrinting.BorderSide.Left;
                            if (i == fin)
                                detailCell.Borders = DevExpress.XtraPrinting.BorderSide.Right;
                        }
                        else
                        {
                            //RANGO INICIO
                            ini = factor * CantidadFilas;

                            if (i >= ini && i <= colCount - 1)
                                detailCell.Borders = DevExpress.XtraPrinting.BorderSide.Bottom;

                            if (i == ini)
                                detailCell.Borders = DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Bottom;
                            if (i == colCount - 1)
                                detailCell.Borders = DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom;
                        }

                        //OBTENEMOS LA CANTIDAD DE FILAS QUE SE PUEDEN GENERAR EN BASE A FACTOR
                        //PREGUNTA SI SE GENERÓ UNA FILA 
                        if (count % factor == 0)
                        {                           
                            //GUARDAMOS FILA
                            tableHeader.Rows.Add(headerRow);
                            tableDetail.Rows.Add(detailRow);
                            
                            headerRow = new XRTableRow();
                            detailRow = new XRTableRow();
                            contadorFactor++;

                            //RECALCULAMOS FIN Y TERMINO
                            n++;
                            ini = factor * n;
                            fin = (ini + factor) - 1;                           
                        }
                        //SI EL LA FILA FINAL
                        if (count == colCount - 1)
                        {                            
                            tableHeader.Rows.Add(headerRow);
                            tableDetail.Rows.Add(detailRow);
                        }                       
                    }                   
                }

                //IMPONIBLES
                if (i < colCount - 1)
                {
                    if (Libro.GetTypeItem(ds.Tables[0].Columns[i].Caption) == 1)
                        Imp = Imp + Convert.ToDouble(ds.Tables[0].Rows[i][i]);
                    if (Libro.GetTypeItem(ds.Tables[0].Columns[i].Caption) == 2 || Libro.GetTypeItem(ds.Tables[0].Columns[i].Caption) == 3)
                        Nopimp = Nopimp + Convert.ToDouble(ds.Tables[0].Rows[i][i]);
                    if (Libro.GetTypeItem(ds.Tables[0].Columns[i].Caption) == 4 && ds.Tables[0].Columns[i].Caption != "SCEMPRE" && ds.Tables[0].Columns[i].Caption != "SEGINV")
                        Leyes = Leyes + Convert.ToDouble(ds.Tables[0].Rows[i][i]);
                    if (Libro.GetTypeItem(ds.Tables[0].Columns[i].Caption) == 5)
                        Dcto = Dcto + Convert.ToDouble(ds.Tables[0].Rows[i][i]);
                    if (ds.Tables[0].Columns[i].Caption == "SCEMPRE" || ds.Tables[0].Columns[i].Caption == "SEGINV")
                        Aporte = Aporte + Convert.ToDouble(ds.Tables[0].Rows[i][i]);
                }       

                if(i == colCount - 1)
                    tableDetail.Rows.Add(RowTotales(Imp, Nopimp, Leyes, Dcto, Aporte, 0));                
            }

            tableHeader.AdjustSize();
            tableDetail.AdjustSize();
            tableHeader.EndInit();
            tableDetail.EndInit();

            Report.Bands[BandKind.PageHeader].Controls.Add(tableHeader);
            Report.Bands[BandKind.Detail].Controls.Add(tableDetail);
                 
        }    

        private void detailcell_BeforePrint(object sender, PrintEventArgs e)
        {
            XRTableCell celda = sender as XRTableCell;
            //double value = Convert.ToDouble(celda.Text);
            //celda.Text = "$" + value.ToString("#,0");

        }

        public DataSet FillDataset(DataTable pTabla)
        {
            DataSet dataset = new DataSet();
            dataset.DataSetName = "dataset";

            dataset.Tables.Add(pTabla);
            pTabla.TableName = "Objetos";
            return dataset;
        }

        private void InitBands(XtraReport Report)
        {
            DetailBand detail = new DetailBand();
            PageHeaderBand pageHeader = new PageHeaderBand();
            ReportFooterBand reportFooter = new ReportFooterBand();
            detail.HeightF = 20;
            reportFooter.HeightF = 380;
            pageHeader.HeightF = 20;

            Report.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] { detail, pageHeader, reportFooter });

        }

        //ROWS TOTALES
        private XRTableRow RowTotales(double pImp, double pNoImp, double pLeyes, double pDecto, double pAporte, double pLiq)
        {
            XRTableRow Fila = new XRTableRow();
            Fila.Borders = DevExpress.XtraPrinting.BorderSide.All;
            XRTableCell totalImp = new XRTableCell(); //{ WidthF = 80};
            XRTableCell totalNoImp = new XRTableCell();// { WidthF =80};
            XRTableCell totalLeyes = new XRTableCell();// { WidthF = 80};
            XRTableCell totalDecto = new XRTableCell(); //{ WidthF = 80};            
            XRTableCell totalAportes = new XRTableCell();// { WidthF = 80};
            XRTableCell totalLiquido = new XRTableCell(); //{ WidthF = 80};            

            totalImp.Text = "Imp:$" + pImp.ToString("#,0");
            totalNoImp.Text = "No imp:$" + pNoImp.ToString("#,0");
            totalLeyes.Text = "LL SS:$" + pLeyes.ToString("#,0");
            totalDecto.Text = "Dctos:$" + pDecto.ToString("#,0");
            totalAportes.Text = "Aportes:$" + pAporte.ToString("#,0");
            totalLiquido.Text = "Liquido: $" + pLiq.ToString("#,0");

            Fila.Cells.Add(totalImp);
            Fila.Cells.Add(totalNoImp);
            Fila.Cells.Add(totalLeyes);
            Fila.Cells.Add(totalDecto);
            Fila.Cells.Add(totalAportes);
            Fila.Cells.Add(totalLiquido);

            return Fila;
        }

        private XRTableRow RowInformacion(string pNombre, string pContrato, string pDias)
        {
            XRTableRow Fila = new XRTableRow();            
            XRTableCell celdaNombre = new XRTableCell();
            celdaNombre.Borders = DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Bottom;
            XRTableCell celdaContrato = new XRTableCell();
            celdaContrato.Borders = DevExpress.XtraPrinting.BorderSide.Bottom;
            XRTableCell celdaDias = new XRTableCell();
            celdaDias.Borders = DevExpress.XtraPrinting.BorderSide.Bottom | DevExpress.XtraPrinting.BorderSide.Right;

            celdaNombre.Text = pNombre;
            celdaContrato.Text = "N° " + pContrato;
            celdaDias.Text = "Dias trab: " + pDias;

            Fila.Cells.Add(celdaNombre);
            Fila.Cells.Add(celdaContrato);
            Fila.Cells.Add(celdaDias);



            return Fila;
        }



        #endregion

        #endregion

        private bool TieneItem(Hashtable tabla, string item)
        {
            bool tiene = false;
            foreach (DictionaryEntry data in tabla)
            {
                //SI SON IGUALES EXISTE EL ITEM

                if (data.Key.ToString() == item)
                {
                    tiene = true;
                }
            }
            return tiene;
        }

        //LIMPIAR campos
        private void LimpiarCampos()
        {
            //cbActual.Checked = true;
            datoCombobox.spLlenaPeriodos("SELECT anomes FROM parametro ORDER BY anomes DESC", lookUpEdit1, "anomes", "anomes", true);
            cbTodos.Checked = false;
            cbxSeleccionConjunto.ItemIndex = -1;
            //txtContrato.Text = "";
            Filtro = false;

            periodoBusqueda = 0;
        }

        private DataSet GetSQLString(int pPeriodo, string pConjunto, bool pTablasExcel = false)
        {
            SqlTransaction sqltran;
            string sql = "SELECT * FROM libro ORDER BY orden";
            if (pTablasExcel)
                sql = "SELECT * FROM libro WHERE visible != 0 ORDER BY orden";
            string cadena = "";
            string header = "";
            string negrita = "";
            string cursiva = "";
            string visible = "";
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            SqlCommand sqlcmd;

            if (fnSistema.ConectarSQLServer())
            {

                sqltran = fnSistema.sqlConn.BeginTransaction();
                //1
                try
                {
                    /*Carga informacion de tabla libro*/
                    using (sqlcmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        sqlcmd.Transaction = sqltran;
                        var datareader = sqlcmd.ExecuteReader();
                        dt.Load(datareader);
                    }

                    //2
                    //FOR PARA RECORRER LA TABLA LIBRO Y SABER QUE ITEMS AGREGAR
                    /*
                     * 0 --> VIENE DE TABLE ITEM (CODITEM)
                     * 1 --> ITEM O VARIABLE SYS DE SISTEMA.
                     */
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        int variableSistema = dt.Rows[i].Field<int>("tipo");
                        string item = dt.Rows[i].Field<string>("coditem");
                        header = header + $"(SELECT alias FROM libro WHERE orden = {i + 1}) as headercampo{i + 1}, \n";
                        negrita = negrita + $"(SELECT negrita FROM libro WHERE orden = {i + 1}) as negrita{i + 1}, \n";
                        cursiva = cursiva + $"(SELECT cursiva FROM libro WHERE orden = {i + 1}) as cursiva{i + 1}, \n";
                        visible = visible + $"(SELECT visible FROM libro WHERE orden = {i + 1}) as visible{i + 1}, \n";
                        //ITEM DE BD
                        if (variableSistema == 0)
                        {
                            cadena = cadena + $"(ISNULL((SELECT SUM(valorcalculado) from itemtrabajador WHERE coditem='{item}' AND itemTrabajador.contrato=trabajador.contrato AND itemTrabajador.anomes=trabajador.anomes AND suspendido=0 ),0)) as campo{i + 1},\n";
                        }
                        //VARIABLE DE SISTEMA
                        if (variableSistema == 1)

                        {
                            cadena = cadena + $"(ISNULL((SELECT SUM({item}) from calculomensual WHERE calculomensual.contrato=trabajador.contrato AND calculomensual.anomes=trabajador.anomes ),0)) as campo{i + 1},\n";
                        }
                        //FORMULA
                        if (variableSistema == 2)

                        {
                            //cadena = cadena + $"(ISNULL((SELECT SUM('{item}') from calculomensual WHERE calculomensual.contrato=trabajador.contrato AND calculomensual.anomes=trabajador.anomes ),0)) as campo{i + 1}, ";
                        }
                    }
                    //string cadenaSinComaFinal = cadena.Remove(cadena.Length - 2, 1);
                    string cadenaSinComaFinal = cadena.Substring(0, cadena.Length-2);
                    if (pConjunto == "")
                    {
                        sql = $"SELECT rut, contrato, concat(trabajador.nombre, ' ', trabajador.apepaterno, ' ', trabajador.apematerno) as nombre, anomes, \n" +
                          $"(SELECT SUM(sysdiastr) from calculomensual WHERE calculomensual.contrato=trabajador.contrato AND calculomensual.anomes=trabajador.anomes) as diasTrabajados, \n" +
                          $"cargo.nombre as cargo, " +
                          $"ccosto.nombre as centrocosto, area.nombre as area, sucursal.descSucursal as sucursal," +
                          $"{header} {negrita} {cursiva} {visible} {cadenaSinComaFinal} \n" +
                          $"FROM trabajador \n" +
                          $"INNER JOIN cargo ON trabajador.cargo = cargo.id \n" +
                          "INNER JOIN area ON trabajador.area = area.id \n" +
                          "INNER JOIN sucursal ON sucursal.codSucursal = trabajador.sucursal \n" +
                          "INNER JOIN ccosto ON ccosto.id = trabajador.ccosto \n" +
                          $"WHERE ANOMES = {pPeriodo} \n" +
                          $"AND contrato IN (SELECT contrato FROM trabajador WHERE{(FiltroUsuario != "0" ? ($" {Conjunto.GetCondicionFromCode(FiltroUsuario) + " AND"}") : "")} \n" +
                          $"{(ShowPrivadas == false ? " privado=0 AND" : "")}  status = 1 AND anomes={pPeriodo})\n" +
                          $" ORDER BY apepaterno\n";
                    }
                    else
                    {
                        sql = $"SELECT rut, contrato, concat(trabajador.nombre, ' ', trabajador.apepaterno, ' ', trabajador.apematerno) as nombre, anomes, \n" +
                        $"(SELECT SUM(sysdiastr) from calculomensual WHERE calculomensual.contrato=trabajador.contrato AND calculomensual.anomes=trabajador.anomes) as diasTrabajados, \n" +
                        $"cargo.nombre as cargo, " +
                        $"ccosto.nombre as centrocosto, area.nombre as area, sucursal.descSucursal as sucursal," +
                        $"{header} {negrita} {cursiva} {visible} {cadenaSinComaFinal} \n" +
                        $"FROM trabajador \n" +
                        $"INNER JOIN cargo ON trabajador.cargo = cargo.id \n" +
                        "INNER JOIN area ON trabajador.area = area.id \n" +
                        "INNER JOIN sucursal ON sucursal.codSucursal = trabajador.sucursal \n" +
                        "INNER JOIN ccosto ON ccosto.id = trabajador.ccosto \n" +
                        $"WHERE ANOMES = {pPeriodo} \n" +
                        $"AND contrato IN (SELECT contrato FROM trabajador WHERE{(FiltroUsuario != "0" ? ($" {Conjunto.GetCondicionFromCode(FiltroUsuario) + " AND"}") : "")} \n" +
                        $"{(ShowPrivadas == false ? " privado=0 AND" : "")} \n" +
                        $"{(pConjunto == "" ? "" : ($"  {Conjunto.GetCondicionFromCode(pConjunto)}"))} AND status = 1 AND anomes={pPeriodo})\n" +
                        $" ORDER BY apepaterno\n";
                    }

                    using (sqlcmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        sqlcmd.Transaction = sqltran;

                        SqlDataAdapter sqldata = new SqlDataAdapter();
                        sqldata.SelectCommand = sqlcmd;
                        sqldata.Fill(ds, "prueba");
                    }
                    sqltran.Commit();
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show(ex.ToString());
                    sqltran.Rollback();
                }
            }
            return ds;

        }

        private DataSet GetSQLStringExcel(int pPeriodo, string pConjunto, string pOrdenPor)
        {
            SqlTransaction sqltran;
            string sql = "SELECT * FROM libro WHERE visible != 0 AND coditem != '' ORDER BY orden";
            string cadena = "";
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            SqlCommand sqlcmd;

            if (fnSistema.ConectarSQLServer())
            {

                sqltran = fnSistema.sqlConn.BeginTransaction();
                //1
                try
                {
                    /*Carga informacion de tabla libro*/
                    using (sqlcmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        sqlcmd.Transaction = sqltran;
                        var datareader = sqlcmd.ExecuteReader();
                        dt.Load(datareader);
                    }

                    //2
                    //FOR PARA RECORRER LA TABLA LIBRO Y SABER QUE ITEMS AGREGAR
                    /*
                     * 0 --> VIENE DE TABLE ITEM (CODITEM)
                     * 1 --> ITEM O VARIABLE SYS DE SISTEMA.
                     */
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        int variableSistema = dt.Rows[i].Field<int>("tipo");
                        string item = dt.Rows[i].Field<string>("coditem");
                        string alias = dt.Rows[i].Field<string>("alias");
                        //ITEM DE BD
                        if (variableSistema == 0)
                        {
                            cadena = cadena + $"(ISNULL((SELECT SUM(valorcalculado) from itemtrabajador WHERE coditem='{item}' AND itemTrabajador.contrato=trabajador.contrato AND itemTrabajador.anomes=trabajador.anomes AND suspendido=0 ),0)) as '{alias}',\n";
                        }
                        //VARIABLE DE SISTEMA
                        if (variableSistema == 1)

                        {
                            cadena = cadena + $"(ISNULL((SELECT SUM({item}) from calculomensual WHERE calculomensual.contrato=trabajador.contrato AND calculomensual.anomes=trabajador.anomes ),0)) as '{alias}',\n";
                        }
                        //FORMULA
                        if (variableSistema == 2)

                        {
                            //cadena = cadena + $"(ISNULL((SELECT SUM('{item}') from calculomensual WHERE calculomensual.contrato=trabajador.contrato AND calculomensual.anomes=trabajador.anomes ),0)) as campo{i + 1}, ";
                        }
                    }
                    //string cadenaSinComaFinal = cadena.Remove(cadena.Length - 2, 1);
                    string cadenaSinComaFinal = cadena.Substring(0, cadena.Length - 2);

                    //Orden por agrupación
                    string orderBy = " ORDER BY apepaterno, apematerno, nombre";
                    if (pOrdenPor != "")
                        orderBy = $" ORDER BY {pOrdenPor}, apepaterno, apematerno, nombre";

                    if (pConjunto == "")
                    {
                        sql = $"SELECT rut, contrato, concat(trabajador.apepaterno, ' ', trabajador.apematerno, ' ', trabajador.nombre) as nombre, anomes, \n" +
                          $"(SELECT SUM(sysdiastr) from calculomensual WHERE calculomensual.contrato=trabajador.contrato AND calculomensual.anomes=trabajador.anomes) as diasTrabajados, \n" +
                          $"cargo.nombre as cargo, " +
                          $"ccosto.nombre as centrocosto, area.nombre as area, sucursal.descSucursal as sucursal," +
                          $"{cadenaSinComaFinal} \n" +
                          $"FROM trabajador \n" +
                          $"INNER JOIN cargo ON trabajador.cargo = cargo.id \n" +
                          "INNER JOIN area ON trabajador.area = area.id \n" +
                          "INNER JOIN sucursal ON sucursal.codSucursal = trabajador.sucursal \n" +
                          "INNER JOIN ccosto ON ccosto.id = trabajador.ccosto \n" +
                          $"WHERE ANOMES = {pPeriodo} \n" +
                          $"AND contrato IN (SELECT contrato FROM trabajador WHERE{(FiltroUsuario != "0" ? ($" {Conjunto.GetCondicionFromCode(FiltroUsuario) + " AND"}") : "")} \n" +
                          $"{(ShowPrivadas == false ? " privado=0 AND" : "")}  status = 1 AND anomes={pPeriodo})\n" +
                          $"{orderBy}\n";
                    }
                    else
                    {
                        sql = $"SELECT rut, contrato, concat(trabajador.apepaterno, ' ', trabajador.apematerno, ' ', trabajador.nombre) as nombre, anomes, \n" +
                        $"(SELECT SUM(sysdiastr) from calculomensual WHERE calculomensual.contrato=trabajador.contrato AND calculomensual.anomes=trabajador.anomes) as diasTrabajados, \n" +
                        $"cargo.nombre as cargo, " +
                        $"ccosto.nombre as centrocosto, area.nombre as area, sucursal.descSucursal as sucursal," +
                        $"{cadenaSinComaFinal} \n" +
                        $"FROM trabajador \n" +
                        $"INNER JOIN cargo ON trabajador.cargo = cargo.id \n" +
                        "INNER JOIN area ON trabajador.area = area.id \n" +
                        "INNER JOIN sucursal ON sucursal.codSucursal = trabajador.sucursal \n" +
                        "INNER JOIN ccosto ON ccosto.id = trabajador.ccosto \n" +
                        $"WHERE ANOMES = {pPeriodo} \n" +
                        $"AND contrato IN (SELECT contrato FROM trabajador WHERE{(FiltroUsuario != "0" ? ($" {Conjunto.GetCondicionFromCode(FiltroUsuario) + " AND"}") : "")} \n" +
                        $"{(ShowPrivadas == false ? " privado=0 AND" : "")} \n" +
                        $"{(pConjunto == "" ? "" : ($"  {Conjunto.GetCondicionFromCode(pConjunto)}"))} AND status = 1 AND anomes={pPeriodo})\n" +
                        $"{orderBy}\n";
                    }

                    using (sqlcmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        sqlcmd.Transaction = sqltran;

                        SqlDataAdapter sqldata = new SqlDataAdapter();
                        sqldata.SelectCommand = sqlcmd;
                        sqldata.Fill(ds, "prueba");
                    }
                    sqltran.Commit();
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show(ex.ToString());
                    sqltran.Rollback();
                }
            }
            return ds;

        }

        private void fnComboItem(string pSql, LookUpEdit pCombo, string pCampoKey, string pCampoDesc, bool? pOcultarKey = false, bool? pMostrarValorKeyAlSeleccionar = false)
        {
            List<Combos> lista = new List<Combos>();
            SqlCommand cmd;
            SqlDataReader rd;

            if (fnSistema.ConectarSQLServer())
            {
                using (cmd = new SqlCommand(pSql, fnSistema.sqlConn))
                {
                    rd = cmd.ExecuteReader();
                    if (rd.HasRows)
                    {
                        while (rd.Read())
                        {
                            //AGREGAMOS VALORES A LA LISTA
                            lista.Add(new Combos() { key = (string)rd[pCampoKey], desc = (string)rd[pCampoDesc] });
                        }
                    }
                }
                //LIBERAR RECURSOS
                cmd.Dispose();
                rd.Close();
                fnSistema.sqlConn.Close();
            }


            if (pMostrarValorKeyAlSeleccionar == true)
            {
                pCombo.Properties.DataSource = lista.ToList();
                pCombo.Properties.ValueMember = "key";
                pCombo.Properties.DisplayMember = "key";

            }
            else
            {
                //AGREGAMOS TOTAL PAGO
                lista.Add(new Combos() { key = "TPAGO", desc = "TOTAL PAGO" });
                pCombo.Properties.DataSource = lista.ToList();
                pCombo.Properties.ValueMember = "key";
                pCombo.Properties.DisplayMember = "desc";
            }

            pCombo.Properties.PopulateColumns();
            //SI OCULTAR KEY ES TRUE NO SE DEBE MOSTRAR LA COLUMNA KEY
            if (pOcultarKey == true)
            {
                pCombo.Properties.Columns[1].Visible = false;
            }
            pCombo.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
            pCombo.Properties.AutoSearchColumnIndex = 1;
            pCombo.Properties.ShowHeader = false;
        }

        private void cbTodos_CheckedChanged(object sender, EventArgs e)
        {
            if (cbTodos.Checked == true)
            {
                //txtConjunto.Text = "";
                //txtConjunto.Enabled = false;
                cbxSeleccionConjunto.EditValue = "";
                cbxSeleccionConjunto.Enabled = false;
                btnConjunto.Enabled = false;

                //txtContrato.Enabled = false;
                //txtContrato.Text = "";
            }
            else
            {
                //txtConjunto.Text = "";
                //txtConjunto.Enabled = true;
                cbxSeleccionConjunto.EditValue = "";
                cbxSeleccionConjunto.Enabled = true;
                cbxSeleccionConjunto.Focus();
                cbxSeleccionConjunto.Enabled = true;
                if (cbxSeleccionConjunto.Properties.DataSource != null)
                    cbxSeleccionConjunto.ItemIndex = 0;
                btnConjunto.Enabled = true;
                //txtContrato.Enabled = true;
                // txtContrato.Focus();
            }
        }

        //private void cbActual_CheckedChanged(object sender, EventArgs e)
        //{         
        //    if (cbActual.Checked == true)
        //    {
        //        datoCombobox.spLlenaPeriodos("SELECT anomes FROM parametro ORDER BY anomes DESC", lookUpEdit1, "anomes", "anomes", true);
        //        //txtContrato.Focus();
        //        lookUpEdit1.ReadOnly = true;
        //    }
        //    else
        //    {
        //        lookUpEdit1.Focus();
        //        lookUpEdit1.ReadOnly = false;
        //    }
        //}

        private void frmLibroRemuneraciones_Shown(object sender, EventArgs e)
        {
            //txtContrato.Focus();
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private void txtContrato_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar) || e.KeyChar == (char)45)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

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

        private void gridRemuneraciones_Click(object sender, EventArgs e)
        {
         
        }

        private void gridRemuneraciones_KeyUp(object sender, KeyEventArgs e)
        {
           
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
           
        }

        private void gridRemuneraciones_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                DialogResult pregunta = XtraMessageBox.Show("¿Desea revisar libro para este trabajador?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (pregunta == DialogResult.Yes)
                {
                //    ImprimeDocumentoIndividual();
                }
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            Close();
        }

        private void btnConfLibro_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            frmconfLibro conf = new frmconfLibro();
            conf.StartPosition = FormStartPosition.CenterParent;
            conf.ShowDialog();
        }

        private void btnConjunto_Click(object sender, EventArgs e)
        {
            if (cbTodos.Checked == false)
            {
                FrmFiltroBusqueda filtro = new FrmFiltroBusqueda(true);
                filtro.opener = this;
                filtro.ShowDialog();

            }
        }


        private void textEdit1_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtPeriodo_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void gridControl1_Click(object sender, EventArgs e)
        {

        }

        private void txtContrato_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void gridRemuneraciones_DoubleClick(object sender, EventArgs e)
        {
            DialogResult pregunta = XtraMessageBox.Show("¿Desea revisar libro para este trabajador?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (pregunta == DialogResult.Yes)
            {
               // ImprimeDocumentoIndividual();
            }
        }


    }
   
}