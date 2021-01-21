using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Data;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Labour
{
    public partial class XtraReportDemo : DevExpress.XtraReports.UI.XtraReport
    {
        private double totalImp = 0;
        private double totalNoimp = 0;
        private double totalFam = 0;
        private double totalLeyes = 0;
        private double totalDesc = 0;
        private double totalApor = 0;

        public XtraReportDemo()
        {
            InitializeComponent();
        }



        #region TABLAS ANTERIORES
        //public List<XRTable> CreateTable()
        //{            
        //    XRTable tabla = new XRTable();
        //    List<XRTable> listaTablas = new List<XRTable>();

        //    //CANTIDAD DE CELDAS QUE TENDRÁ LA FILA        
        //    if (Libro.TablaLibroRemuneraciones.Rows.Count>0 && Libro.ListHeaderRow.Count>0)
        //    {
        //        XRTable GroupTable = new XRTable();
        //        int count = 0, countHeader = 0;           

        //        GroupTable.Borders = DevExpress.XtraPrinting.BorderSide.All;
        //        GroupTable.BeginInit();

        //        //AGREGAMOS DATA                
        //        foreach (DataRow row in Libro.TablaLibroRemuneraciones.Rows)
        //        {
        //            //PARA CONTAR LAS COLUMNAS
        //            count = 0;

        //            //AGREGAMOS CABECERA CON NOMBRE Y CONTRATO
        //            GroupTable.Rows.Add(GetRowHeader(Libro.ListHeaderRow[countHeader].NameHeader, Libro.ListHeaderRow[countHeader].ContratoHeader, Libro.ListHeaderRow[countHeader].DiasHeader));

        //            //FILA CON DATA
        //            XRTableRow Fila = new XRTableRow();      
        //            foreach (DataColumn column in Libro.TablaLibroRemuneraciones.Columns)
        //            {
        //               count++;

        //               GetRowData(column, row, Fila);

        //               //SI ES MULTIPLO DE 8 GUARDAMOS FILA
        //               if (count % 6 == 0)
        //               {
        //                  GroupTable.Rows.Add(Fila);
        //                  Fila = new XRTableRow();
        //               }

        //               //ELEMENTOS RESTANTES --> CANTIDADE DE COLUMNAS - count
        //               if (Libro.TablaLibroRemuneraciones.Columns.Count - count < 6 && count == Libro.TablaLibroRemuneraciones.Columns.Count)
        //               {
        //                  GroupTable.Rows.Add(Fila);
        //               }
        //            }                  

        //            //COLUMNA DE SUMATORIAS
        //            //GroupTable.Rows.Add(getResultRow(totalImp, totalNoimp, totalFam, totalLeyes, totalDesc, totalApor));

        //            countHeader++;

        //            //AGREGAMOS TABLA A LISTA DE TABLAS
        //            GroupTable.BeforePrint += new System.Drawing.Printing.PrintEventHandler(tabla_BeforePrint);
        //            //GroupTable.AdjustSize();              
        //            GroupTable.EndInit();
        //            totalImp = 0;
        //            totalApor = 0;
        //            totalDesc = 0;
        //            totalFam = 0;
        //            totalLeyes = 0;
        //            totalNoimp = 0;
        //            listaTablas.Add(GroupTable);
        //        }                
        //    }

        //    return listaTablas;
        //}

        //private XRTable GetTablaImponible()
        //{
        //    XRTable tabla = new XRTable();
        //    tabla.BeginInit();
        //    if (Libro.TablaLibroRemuneraciones.Rows.Count>0)
        //    {
        //        //RECORREMOS TABLA
        //        int count = 0, countHeader = 0;

        //        foreach (DataRow row in Libro.TablaLibroRemuneraciones.Rows)
        //        {
        //            //FILA
        //            XRTableRow fila = new XRTableRow();

        //            foreach (DataColumn column in Libro.TablaLibroRemuneraciones.Columns)
        //            {
        //                count++;
        //                XRTableCell celda = new XRTableCell();
        //                celda.Text = column.ColumnName.ToLower();             
        //                fila.Cells.Add(celda);                        

        //                /*if (count % 2 == 0)
        //                {
        //                    tabla.Rows.Add(fila);
        //                    fila = new XRTableRow();
        //                }*/

        //                /*if (Libro.TablaLibroRemuneraciones.Columns.Count - countHeader < 2 && count == Libro.TablaLibroRemuneraciones.Columns.Count)
        //                {
        //                    tabla.Rows.Add(fila);
        //                }*/

        //            }

        //            tabla.Rows.Add(fila);                    
        //            countHeader++;

        //            /*XRTableCell total = new XRTableCell() { Text = "125000"};
        //            XRTableRow rowtotal = new XRTableRow();
        //            rowtotal.Cells.Add(total);
        //            tabla.Rows.Add(rowtotal);*/                 

        //        }
        //        tabla.BeforePrint += new System.Drawing.Printing.PrintEventHandler(tabla_BeforePrint);
        //        tabla.AdjustSize();
        //        //tabla.WidthF = panel.WidthF;
        //        tabla.EndInit();
        //    }

        //    return tabla;
        //}

        //private XRTable GetTablaNoImp(XRPanel panel, string pTitle)
        //{
        //    XRTable tabla = new XRTable();
        //    tabla.BeginInit();

        //    if (Libro.TablaLibroRemuneraciones.Rows.Count > 0)
        //    {
        //        //RECORREMOS TABLA
        //        int count = 0, countHeader = 0;
        //        XRTableRow rowHeader = new XRTableRow();
        //        rowHeader.Cells.Add(new XRTableCell() { Text = pTitle});
        //        tabla.Rows.Add(rowHeader);

        //        foreach (DataRow row in Libro.TablaLibroRemuneraciones.Rows)
        //        {
        //            //FILA
        //            XRTableRow fila = new XRTableRow();
        //            foreach (DataColumn column in Libro.TablaLibroRemuneraciones.Columns)
        //            {
        //                count++;
        //                XRTableCell celda = new XRTableCell();
        //                if (Libro.GetTypeItem(column.ColumnName) == 2 || Libro.GetTypeItem(column.ColumnName) == 3)
        //                {
        //                    celda.Text = column.ColumnName.ToLower() + ":" + (Convert.ToDouble(row[column.ColumnName])).ToString("#,0");
        //                    fila.Cells.Add(celda);
        //                }                            
        //                if (count % 2 == 0)
        //                {
        //                    tabla.Rows.Add(fila);
        //                    fila = new XRTableRow();
        //                }

        //                if (Libro.TablaLibroRemuneraciones.Columns.Count - countHeader < 2 && count == Libro.TablaLibroRemuneraciones.Columns.Count)
        //                {
        //                    tabla.Rows.Add(fila);
        //                }
        //            }
        //            countHeader++;                    

        //            tabla.BeforePrint += new System.Drawing.Printing.PrintEventHandler(tabla_BeforePrint);
        //            tabla.EndInit();
        //            tabla.WidthF = panel.WidthF;

        //        }
        //    }

        //    return tabla;
        //}

        //private void tabla_BeforePrint(object sender, PrintEventArgs e)
        //{
        //    XRTable table = ((XRTable)sender);
        //    table.LocationF = new DevExpress.Utils.PointFloat(0F, 0F);
        //    table.WidthF = this.PageWidth - this.Margins.Left - this.Margins.Right;
        //}

        ////COLUMN HEADER 
        //private XRTableRow GetRowHeader(string pName, string pContrato, string pDias)
        //{
        //    XRTableRow row = new XRTableRow();
        //    XRTableCell cellName = new XRTableCell();
        //    XRTableCell cellContrato = new XRTableCell();
        //    XRTableCell cellDias = new XRTableCell();

        //    cellName.Text = pName;
        //    cellContrato.Text = "N° " + pContrato;
        //    //cellContrato.WidthF = 15;
        //    cellDias.Text = "Dias trab: " + pDias;

        //    row.Cells.Add(cellContrato);
        //    row.Cells.Add(cellName);
        //    row.Cells.Add(cellDias);

        //    return row;            
        //}

        ////GENERA ROW PARA DATA
        //private void GetRowData(DataColumn column, DataRow rowTable, XRTableRow row)
        //{           

        //    //CELDA QUE REPRESENTA EL CAMPO Y EL DATO
        //    XRTableCell CellName = new XRTableCell();
        //    XRTableCell CellData = new XRTableCell();

        //    CellName.Text = column.ColumnName.ToLower();
        //    CellName.Borders = DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom;
        //    CellName.HeightF = 4;
        //    CellData.Text = "$" + (Convert.ToDouble(rowTable[column.ColumnName])).ToString("#,0");
        //    CellData.Borders = DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom | DevExpress.XtraPrinting.BorderSide.Right;
        //    CellData.HeightF = 4;
        //    if (Libro.AplicaEstilo("negrita", column.ColumnName) && Libro.AplicaEstilo("cursiva", column.ColumnName))
        //        CellData.Font = new Font("Arial", 8, FontStyle.Bold | FontStyle.Italic);
        //    else if (Libro.AplicaEstilo("negrita", column.ColumnName))
        //        CellData.Font = new Font("Arial", 8, FontStyle.Bold);
        //    else if (Libro.AplicaEstilo("cursiva", column.ColumnName))
        //        CellData.Font = new Font("Arial", 8, FontStyle.Italic);

        //    if (Libro.GetTypeItem(column.ColumnName) == 1)
        //        totalImp = totalImp + Convert.ToDouble(rowTable[column.ColumnName]);
        //    if (Libro.GetTypeItem(column.ColumnName) == 2)
        //        totalNoimp = totalNoimp + Convert.ToDouble(rowTable[column.ColumnName]);
        //    if (Libro.GetTypeItem(column.ColumnName) == 3)
        //        totalFam = totalFam + Convert.ToDouble(rowTable[column.ColumnName]);
        //    if (Libro.GetTypeItem(column.ColumnName) == 4 && column.ColumnName != "SCEMPRE" && column.ColumnName != "SEGINV")
        //        totalLeyes = totalLeyes + Convert.ToDouble(rowTable[column.ColumnName]);
        //    if (Libro.GetTypeItem(column.ColumnName) == 5)
        //        totalDesc = totalDesc + Convert.ToDouble(rowTable[column.ColumnName]);
        //    if (column.ColumnName == "SCEMPRE" || column.ColumnName == "SEGINV")
        //        totalApor = totalApor + Convert.ToDouble(rowTable[column.ColumnName]);

        //    //AGREGAMOS CELDAS A FILA
        //    row.Cells.Add(CellName);
        //    row.Cells.Add(CellData);
        //}

        ////GENERA TABLA CON SUMATORIA TOTAL
        //private XRTableRow getResultRow(double pTotalImp, double pHaberes, double pDesctos, double pLiquido, double pPago)
        //{
        //    XRTableRow Fila = new XRTableRow();
        //    //Fila.BorderDashStyle = DevExpress.XtraPrinting.BorderDashStyle.Double;
        //    XRTableCell cellImp = new XRTableCell();
        //    XRTableCell cellHaberes = new XRTableCell();
        //    XRTableCell cellDctos = new XRTableCell();
        //    XRTableCell cellPago = new XRTableCell();
        //    XRTableCell cellLiquido = new XRTableCell();

        //    cellImp.Text = " Imponibles:" + "$" + pTotalImp.ToString("#,0");
        //    cellHaberes.Text = " Haberes: $" + pHaberes.ToString("#,0");
        //    cellDctos.Text = " Dctos: $" + pDesctos.ToString("#,0");
        //    cellLiquido.Text = " Liquido: $" + pLiquido.ToString("#,0");
        //    cellPago.Text = " Pago: $" + pPago.ToString("#,0");

        //    cellImp.Font = new Font("Time New Romans", 9);
        //    cellImp.Borders = DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom;
        //    cellImp.TextAlignment = DevExpress.XtraPrinting.TextAlignment.BottomCenter;
        //    cellImp.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 4);

        //    cellHaberes.Font = new Font("Time New Romans", 9);
        //    cellHaberes.Borders = DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom;
        //    cellHaberes.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
        //    cellHaberes.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 4);

        //    cellDctos.Font = new Font("Time New Romans", 9);
        //    cellDctos.Borders = DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom;
        //    cellDctos.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
        //    cellDctos.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 4);

        //    cellLiquido.Font = new Font("Time New Romans", 9);
        //    cellLiquido.Borders = DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom;
        //    cellLiquido.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
        //    cellLiquido.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 4);

        //    cellPago.Font = new Font("Time New Romans", 9, FontStyle.Bold);
        //    cellPago.Borders = DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Right;
        //    cellPago.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
        //    cellPago.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 4);

        //    Fila.Cells.Add(cellImp);
        //    Fila.Cells.Add(cellHaberes);
        //    Fila.Cells.Add(cellDctos);
        //    Fila.Cells.Add(cellLiquido);
        //    Fila.Cells.Add(cellPago);            

        //    return Fila;
        //}

        ////SUBTABLA SOLO ITEMS IMPONIBLES
        //private void SetDataRowType(DataRow rowTabla, DataColumn column, int type, XRTableRow Fila)
        //{
        //    XRTableCell celda = new XRTableCell();            
        //    if (Libro.GetTypeItem(column.ColumnName) == type)
        //    {                
        //        celda.Text = column.ColumnName.ToLower() + ":" +  (Convert.ToDouble(rowTabla[column.ColumnName])).ToString("#,0");
        //        celda.Borders = DevExpress.XtraPrinting.BorderSide.All;
        //        Fila.Cells.Add(celda);
        //    }                  
        //}

        //private void Demo()
        //{
        //    this.DataSource = GetData();
        //    this.DataMember = ((DataSet)this.DataSource).Tables[0].TableName;
        //    DataSet ds = ((DataSet)this.DataSource);

        //    InitBands();

        //    //CANTIDAD DE COLUMNAS
        //    int Colcount = ds.Tables[0].Columns.Count;
        //    int colWidth = (this.PageWidth - (this.Margins.Left + this.Margins.Right) / Colcount);

        //    XRTable TableHeader = new XRTable();
        //    TableHeader.HeightF = 20;
        //    TableHeader.WidthF = this.PageWidth - (this.Margins.Left + this.Margins.Right);
        //    XRTableRow headerRow = new XRTableRow();
        //    headerRow.WidthF = TableHeader.WidthF;
        //    TableHeader.Rows.Add(headerRow);

        //    TableHeader.BeginInit();

        //    //TABLA PARA MOSTRAR LA INFORMACION
        //    XRTable TableDetail = new XRTable();
        //    TableDetail.HeightF = 20;
        //    TableDetail.WidthF = (this.PageWidth - (this.Margins.Left + this.Margins.Right));
        //    XRTableRow detailRow = new XRTableRow();
        //    detailRow.WidthF = TableDetail.WidthF;
        //    TableDetail.Rows.Add(detailRow);
        //    TableDetail.EvenStyleName = "EvenStyle";
        //    TableDetail.OddStyleName = "OddStyle";

        //    TableDetail.BeginInit();

        //    for (int i = 0; i < Colcount; i++)
        //    {
        //        XRTableCell headerCell = new XRTableCell();
        //        headerCell.WidthF = colWidth;
        //        headerCell.Text = ds.Tables[0].Columns[i].Caption;

        //        XRTableCell detailCell = new XRTableCell();
        //        detailCell.WidthF = colWidth;
        //        detailCell.DataBindings.Add("Text", null, ds.Tables[0].Columns[i].Caption);
        //        if (i == 0)
        //        {
        //            headerCell.Borders = DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom;
        //            detailCell.Borders = DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom;
        //        }
        //        else
        //        {
        //            headerCell.Borders = DevExpress.XtraPrinting.BorderSide.All;
        //            detailCell.Borders = DevExpress.XtraPrinting.BorderSide.All;
        //        }

        //        headerRow.Cells.Add(headerCell);
        //        detailRow.Cells.Add(detailCell);
        //    }

        //    TableHeader.EndInit();
        //    TableHeader.EndInit();

        //    this.Bands[BandKind.PageHeader].Controls.Add(TableHeader);
        //    this.Bands[BandKind.Detail].Controls.Add(TableDetail);
        //}

        //private DataSet GetData()
        //{
        //    DataSet ds = new DataSet();
        //    ds.DataSetName = "DataReport";
        //    DataTable tabla = new DataTable();            

        //    if (Libro.TablaLibroRemuneraciones.Rows.Count>0)
        //    {
        //        tabla = Libro.TablaLibroRemuneraciones;
        //        tabla.TableName = "Objetos";

        //        ds.Tables.Add(tabla);                 
        //    }

        //    return ds;
        //}

        //private void InitBands()
        //{
        //    DetailBand detail = new DetailBand();
        //    PageHeaderBand pageHeader = new PageHeaderBand();
        //    ReportFooterBand reportFooter = new ReportFooterBand();
        //    detail.HeightF = 20;
        //    reportFooter.HeightF = 380;
        //    pageHeader.HeightF = 20;

        //    this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] { detail, pageHeader, reportFooter});

        //}

        //private void TablaData(XRTable pTabla)
        //{
        //    pTabla.Rows.Clear();

        //    if (Libro.TablaLibroRemuneraciones.Rows.Count>0 && Libro.ListadoLiquidos.Count>0)
        //    {
        //        //TOTAL DE COLUMNAS DE LA TABLA
        //        int colcount = Libro.TablaLibroRemuneraciones.Columns.Count;
        //        //ANCHO DE LA COLUMNA
        //        //int wodthCol = (this.PageWidth - (this.Margins.Left + this.Margins.Right))/colcount;
        //        int factor = 6;
        //        double Imp = 0, Dcto = 0, dato = 0;
        //        double Hab = 0, Liq = 0, Pago = 0;

        //        pTabla.BeginInit();           
        //        pTabla.HeightF = 20;
        //        //pTabla.KeepTogether = true;
        //        //pTabla.WidthF = (this.PageWidth - (this.Margins.Left - this.Margins.Right));

        //        XRTableRow detailRow = new XRTableRow();
        //        detailRow.WidthF = pTabla.WidthF;

        //        int elemento = 0, posicion = 0, countHeader = 0;      
        //        foreach (DataRow row in Libro.TablaLibroRemuneraciones.Rows)
        //        {
        //            elemento = 0;
        //            detailRow = new XRTableRow();
        //            posicion = 0;

        //            //TOTALES
        //            Imp = 0;
        //            Hab = 0;
        //            Dcto = 0;
        //            Liq = 0;
        //            Pago = 0;

        //            XRTableRow Informacion = new XRTableRow();
        //            //Informacion.Borders = DevExpress.XtraPrinting.BorderSide.All;

        //            //CABECERA 
        //            //CONTRATO
        //            XRTableCell celdaContrato = new XRTableCell();
        //            celdaContrato.WidthF = 40;
        //            celdaContrato.Text = "Rut " + fnSistema.fFormatearRut2(Persona.GetRutPersona(Libro.ListHeaderRow[countHeader].ContratoHeader, Libro.ListHeaderRow[countHeader].PeriodoHeader));
        //            celdaContrato.Font = new Font("Times new Romans", 9);
        //            celdaContrato.TextAlignment = DevExpress.XtraPrinting.TextAlignment.BottomCenter;
        //            celdaContrato.Borders = DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Bottom;
        //            celdaContrato.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 4);

        //            //NOMBRE TRABAJADOR
        //            XRTableCell celNombre = new XRTableCell();
        //            celNombre.Text = "Trabajador: " + fnSistema.PrimerMayuscula((Libro.ListHeaderRow[countHeader].NameHeader).ToLower());
        //            celNombre.Font = new Font("Times new Romans", 9, FontStyle.Bold);
        //            celNombre.TextAlignment = DevExpress.XtraPrinting.TextAlignment.BottomCenter;
        //            celNombre.Borders = DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom;
        //            celNombre.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 4);

        //            //RUT   
        //            XRTableCell celdaDias = new XRTableCell();
        //            celdaDias.WidthF = 40;
        //            celdaDias.Text = "Dias trabajados: " + Libro.ListHeaderRow[countHeader].DiasHeader;
        //            celdaDias.Font = new Font("Times new Romans", 9);
        //            celdaDias.TextAlignment = DevExpress.XtraPrinting.TextAlignment.BottomCenter;
        //            celdaDias.Borders = DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom;
        //            celdaDias.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 4);

        //            //DIAS LICENCIAS
        //            XRTableCell celdaLicencias = new XRTableCell();
        //            celdaLicencias.WidthF = 40;
        //            celdaLicencias.Text = "Dias Licencia: " + Libro.ListHeaderRow[countHeader].DiasLicHeader;
        //            celdaLicencias.Font = new Font("Times new Romans", 9);
        //            celdaLicencias.TextAlignment = DevExpress.XtraPrinting.TextAlignment.BottomCenter;
        //            celdaLicencias.Borders = DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom;
        //            celdaLicencias.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 4);

        //            //IMPONIBLE
        //            Imp = Calculo.GetValueFromCalculoMensaul(Libro.ListHeaderRow[countHeader].ContratoHeader, Libro.ListHeaderRow[countHeader].PeriodoHeader, "systimp");
        //            //HABERES
        //            Hab = Calculo.GetValueFromCalculoMensaul(Libro.ListHeaderRow[countHeader].ContratoHeader, Libro.ListHeaderRow[countHeader].PeriodoHeader, "systhab");
        //            //DESCUENTOS
        //            Dcto = Calculo.GetValueFromCalculoMensaul(Libro.ListHeaderRow[countHeader].ContratoHeader, Libro.ListHeaderRow[countHeader].PeriodoHeader, "systdctos");
        //            //LIQUIDO
        //            Liq = Calculo.GetValueFromCalculoMensaul(Libro.ListHeaderRow[countHeader].ContratoHeader, Libro.ListHeaderRow[countHeader].PeriodoHeader, "sysliq");
        //            //PAGO
        //            Pago = Calculo.GetValueFromCalculoMensaul(Libro.ListHeaderRow[countHeader].ContratoHeader, Libro.ListHeaderRow[countHeader].PeriodoHeader, "syspago");
        //            XRTableRow rowNombre = new XRTableRow();
        //            XRTableRow rowDias = new XRTableRow();
        //            //HACER UAN
        //            XRTable tablaPrueba1 = new XRTable();
        //            Informacion.Cells.Add(celdaContrato);
        //            //Informacion.Cells.Add(celNombre);
        //            //Informacion.Cells.Add(celdaDias);
        //            rowNombre.Cells.Add(celNombre);
        //            rowDias.Cells.Add(celdaDias);

        //            //AGREGAMOS HEADER CON DATA DEL EMPLEADO
        //            pTabla.Rows.Add(Informacion);
        //            pTabla.Rows.Add(rowNombre);
        //            pTabla.Rows.Add(rowDias);

        //            foreach (DataColumn column in Libro.TablaLibroRemuneraciones.Columns)
        //            {
        //                //NOS MOVEMOS UNA POSICION
        //                posicion++;
        //                elemento++;

        //                //GUARDAMOS LA DATA EN UNA CELDA
        //                XRTableCell celda = new XRTableCell();                        
        //                dato = Convert.ToDouble(row[column]);
        //                celda.Text = Libro.AplicaAlias(column.Caption.ToLower()) == false ? (fnSistema.PrimerMayuscula(column.Caption.ToLower()) + ": $ " + dato.ToString("#,0")) : (Libro.GetAlias(fnSistema.PrimerMayuscula(column.Caption.ToLower())) + ": $" + dato.ToString("#,0"));
        //                celda.WidthF = pTabla.WidthF / factor;
        //                celda.TextAlignment = DevExpress.XtraPrinting.TextAlignment.BottomJustify;                        
        //                celda.RowSpan = 3;
        //                celda.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 2, 2);

        //                //VERIFICAR SI APLICA ESTILO EN CELDA
        //                if (Libro.AplicaEstilo("negrita", column.Caption))
        //                    celda.Font = new Font("Times New Romans", 9, FontStyle.Bold);
        //                if (Libro.AplicaEstilo("cursiva", column.Caption))
        //                    celda.Font = new Font("Times New Romans", 9, FontStyle.Italic);
        //                if (Libro.AplicaEstilo("negrita", column.Caption) && Libro.AplicaEstilo("cursiva", column.Caption))
        //                    celda.Font = new Font("Times new Romans", 9, FontStyle.Italic | FontStyle.Bold);



        //                //SI ESTAMOS EN LA PRIMERA POSICION AGREGAMOS BORDE IZQUIERDO
        //                if (posicion == 1)                   
        //                   celda.Borders = DevExpress.XtraPrinting.BorderSide.Left;                                             

        //                //SI ESTAMOS EN LA ULTIMA POSICION (ULTIMO ELEMENTO DE LA FILA)
        //                if (posicion == factor)
        //                    celda.Borders = DevExpress.XtraPrinting.BorderSide.Right;

        //                //AGREGAMOS CELDA A FILA (HASTA QUE SE COMPLETE LA FILA CON LA CANTIDAD DE COLUMNAS ESTABLECIDAS)
        //                detailRow.Cells.Add(celda);                                        

        //                //SI LA POSICION LLEGA HASTA EL FINAL DE LA FILA 
        //                //AGREGAMOS FILA A LA TABLA
        //                if (posicion == factor)
        //                {
        //                    pTabla.Rows.Add(detailRow);
        //                    //LIMPIAMOS LA FILA
        //                    detailRow = new XRTableRow();

        //                    //RESET POSICION AL PRIMER ELEMENTO
        //                    posicion = 0;

        //                }
        //                else
        //                {
        //                    //SI ESTAMOS EN EL ULTIMO ELEMENTO AGREGAMOS FILA
        //                    if (elemento == colcount)
        //                    {
        //                        //PREGUNTAR SI POSICION ES MENOR A FACTOR (HAY MENOS COLUMNAS)
        //                        //COMPLETAMOS CELDAS FICTICIAS
        //                        if (posicion < factor)
        //                        {
        //                            int x = 1;
        //                            int CeldasFaltantes = factor - posicion;
        //                            while (x <= CeldasFaltantes)
        //                            {
        //                                XRTableCell cel = new XRTableCell();
        //                                cel.Text = "";

        //                                //SI ES LA ULTIMA CELDA
        //                                if (x == CeldasFaltantes)
        //                                    cel.Borders = DevExpress.XtraPrinting.BorderSide.Right;

        //                                detailRow.Cells.Add(cel);                                      

        //                                x++;
        //                            }

        //                            //UNA VEZ TERMINADO AGREGAMOS FILA A TABLA
        //                            pTabla.Rows.Add(detailRow);
        //                            //LIMPIAMOS LA FILA
        //                            detailRow = new XRTableRow();
        //                            //RESET POSICION
        //                            posicion = 0;
        //                        }
        //                        else
        //                        {
        //                            pTabla.Rows.Add(detailRow);
        //                            //LIMPIAMOS LA FILA
        //                            detailRow = new XRTableRow();
        //                            //RESET POSICION
        //                            posicion = 0;
        //                        }                             
        //                    }
        //                }
        //            }

        //            //AGREGAMOS FILA DE RESULTADO
        //            pTabla.Rows.Add(getResultRow(Imp, Hab, Dcto, Libro.ListadoLiquidos[countHeader], Pago));

        //            countHeader++;
        //        }
        //    }

        //    //pTabla.AdjustSize();
        //    pTabla.EndInit();           

        //}

        //private void TablaHeader(XRTable pTabla)
        //{
        //    pTabla.Rows.Clear();
        //    if (Libro.TablaLibroRemuneraciones.Rows.Count > 0)
        //    {
        //        //TOTAL DE COLUMNAS DE LA TABLA
        //        int colcount = Libro.TablaLibroRemuneraciones.Columns.Count;
        //        //ANCHO DE LA COLUMNA
        //        //int widthCol = (int)((pTabla.WidthF - (this.Margins.Left + this.Margins.Right)) / colcount);
        //        int factor = 6, res = 0, CantidadFilas = 0, contadorFactor = 1;

        //        //CANTIDAD DE FILAS EN BASE A FACTOR
        //        CantidadFilas = colcount / factor;
        //        res = colcount % factor;

        //        pTabla.BeginInit();
        //        pTabla.KeepTogether = true;
        //        pTabla.HeightF = 20;
        //        //pTabla.WidthF = (this.PageWidth - (this.Margins.Left - this.Margins.Right));

        //        XRTableRow detailRow = new XRTableRow();
        //        detailRow.WidthF = pTabla.WidthF;

        //        int count = 0;

        //            foreach (DataColumn column in Libro.TablaLibroRemuneraciones.Columns)
        //            {
        //                //CONTAR CANTIDAD DE COLUMNAS
        //                count++;

        //                //GUARDAMOS LA DATA EN UNA CELDA
        //                XRTableCell celda = new XRTableCell();
        //                celda.WidthF = pTabla.WidthF/factor;
        //                celda.Text = column.Caption;                        
        //                celda.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
        //                celda.Borders = DevExpress.XtraPrinting.BorderSide.All;

        //                //AGREGAMOS CELDA A FILA
        //                detailRow.Cells.Add(celda);

        //                if (colcount % factor == 0)
        //                {
        //                    if (count % factor == 0)
        //                    {
        //                        pTabla.Rows.Add(detailRow);

        //                        detailRow = new XRTableRow();
        //                        contadorFactor++;
        //                    }

        //                }
        //                else
        //                {
        //                    //LA CANTIDAD DE FILAS NO SE PUEDE SEPARAR UNIFORMEMENTE
        //                    //HAY COLUMNAS RESTANTES

        //                    if (colcount <= factor)
        //                    {
        //                        if (count == colcount)
        //                            pTabla.Rows.Add(detailRow);
        //                    }
        //                    else
        //                    {
        //                    //OBTENEMOS LA CANTIDAD DE FILAS QUE SE PUEDEN GENERAR EN BASE A FACTOR
        //                    //PREGUNTA SI SE GENERÓ UNA FILA 

        //                        if (count % factor == 0)
        //                        {                                   
        //                            //GUARDAMOS FILA
        //                            pTabla.Rows.Add(detailRow);
        //                            detailRow = new XRTableRow();
        //                            contadorFactor++;
        //                        }
        //                        //SI EL LA FILA FINAL
        //                        if (count == colcount)
        //                        {
        //                            pTabla.Rows.Add(detailRow);
        //                        }
        //                    }
        //                }
        //            }

        //    }

        //    //pTabla.AdjustSize();
        //    pTabla.EndInit();
        //}
        #endregion


        private void PictureEmpleado_BeforePrint(object sender, PrintEventArgs e)
        {
            Image img;
            if (Imagen.GetLogoFromBd() != null)
            {
                img = Imagen.GetLogoFromBd();
                PictureEmpleado.Image = img;
                PictureEmpleado.Sizing = DevExpress.XtraPrinting.ImageSizeMode.StretchImage;
            }
        }

        private void xrLabel1_BeforePrint(object sender, PrintEventArgs e)
        {

        }


        #region CeldasCreadasPrueba
        //private void xrTable2_BeforePrint(object sender, PrintEventArgs e)
        //{
        //    xrTable2.Rows.FirstRow.Visible = false;

        //    int numerosContratos = Libro.GetNumeroContratos(this.Parameters["periodo"].Value.ToString());
        //    int numeroFila = 0;
        //    int numeroCelda = 0;
        //    if (numerosContratos != 0)
        //    {
        //        //  CREAR 4 FILAS POR CADA CONTRATO
        //        for (int i = 0; i < numerosContratos; i++)
        //        {
        //            //CREAR FILAS
        //            for (int j = 0; j < 4; j++)
        //            {

        //                XRTableRow filaNueva = new XRTableRow();
        //                filaNueva.Name = "n" + numeroFila.ToString();
        //                filaNueva.Borders = DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right;
        //                //filaNueva.CanGrow = false;
        //                //filaNueva.CanShrink = false;
        //                //LLENAR CELDAS
        //                for (int k = 0; k <= 10; k++)
        //                {
        //                    XRTableCell nuevaCelda = new XRTableCell();
        //                    nuevaCelda.Name = "nc" + numeroCelda;
        //                    nuevaCelda.WidthF = xrTable1.Rows[0].Cells[k].WidthF;
        //                    float ancho = nuevaCelda.WidthF;
        //                    nuevaCelda.HeightF = xrTable1.Rows[0].Cells[k].HeightF;
        //                    //nuevaCelda.CanGrow = false;
        //                    //nuevaCelda.CanShrink = false;
        //                    nuevaCelda.TextTrimming = StringTrimming.None;
        //                    nuevaCelda.WordWrap = false;
        //                    nuevaCelda.Text = "prueba";
        //                    numeroCelda++;
        //                    filaNueva.Cells.Add(nuevaCelda);
        //                }
        //                numeroFila++;
        //                //ÚLTIMA FILA
        //                if (j == 3)
        //                {
        //                    filaNueva.Borders = DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Bottom | DevExpress.XtraPrinting.BorderSide.Right;
        //                }
        //                xrTable2.Rows.Add(filaNueva);
        //            }
        //        }
        //    }
        //}
        #endregion
    }
}
