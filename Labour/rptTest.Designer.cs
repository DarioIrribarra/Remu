namespace Labour
{
    partial class rptTest
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            DevExpress.DataAccess.Sql.CustomSqlQuery customSqlQuery1 = new DevExpress.DataAccess.Sql.CustomSqlQuery();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(rptTest));
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.sqlDataSource1 = new DevExpress.DataAccess.Sql.SqlDataSource(this.components);
            this.xrPivotGrid1 = new DevExpress.XtraReports.UI.XRPivotGrid();
            this.fieldrut1 = new DevExpress.XtraReports.UI.PivotGrid.XRPivotGridField();
            this.fieldnombre1 = new DevExpress.XtraReports.UI.PivotGrid.XRPivotGridField();
            this.fieldcoditem1 = new DevExpress.XtraReports.UI.PivotGrid.XRPivotGridField();
            this.fieldvalorcalculado1 = new DevExpress.XtraReports.UI.PivotGrid.XRPivotGridField();
            this.fieldorden1 = new DevExpress.XtraReports.UI.PivotGrid.XRPivotGridField();
            this.PageHeader = new DevExpress.XtraReports.UI.PageHeaderBand();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // TopMargin
            // 
            this.TopMargin.HeightF = 135F;
            this.TopMargin.Name = "TopMargin";
            this.TopMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.TopMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // BottomMargin
            // 
            this.BottomMargin.Name = "BottomMargin";
            this.BottomMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // sqlDataSource1
            // 
            this.sqlDataSource1.ConnectionName = "localhost_EST_Connection";
            this.sqlDataSource1.Name = "sqlDataSource1";
            customSqlQuery1.Name = "Query";
            customSqlQuery1.Sql = resources.GetString("customSqlQuery1.Sql");
            this.sqlDataSource1.Queries.AddRange(new DevExpress.DataAccess.Sql.SqlQuery[] {
            customSqlQuery1});
            this.sqlDataSource1.ResultSchemaSerializable = resources.GetString("sqlDataSource1.ResultSchemaSerializable");
            // 
            // xrPivotGrid1
            // 
            this.xrPivotGrid1.Appearance.Cell.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.xrPivotGrid1.Appearance.CustomTotalCell.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.xrPivotGrid1.Appearance.FieldHeader.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.xrPivotGrid1.Appearance.FieldValue.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.xrPivotGrid1.Appearance.FieldValueGrandTotal.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.xrPivotGrid1.Appearance.FieldValueTotal.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.xrPivotGrid1.Appearance.GrandTotalCell.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.xrPivotGrid1.Appearance.Lines.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.xrPivotGrid1.Appearance.TotalCell.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.xrPivotGrid1.Fields.AddRange(new DevExpress.XtraReports.UI.PivotGrid.XRPivotGridField[] {
            this.fieldrut1,
            this.fieldnombre1,
            this.fieldcoditem1,
            this.fieldvalorcalculado1,
            this.fieldorden1});
            this.xrPivotGrid1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrPivotGrid1.Name = "xrPivotGrid1";
            this.xrPivotGrid1.OptionsPrint.FilterSeparatorBarPadding = 3;
            this.xrPivotGrid1.SizeF = new System.Drawing.SizeF(650F, 50F);
            // 
            // fieldrut1
            // 
            this.fieldrut1.Area = DevExpress.XtraPivotGrid.PivotArea.ColumnArea;
            this.fieldrut1.AreaIndex = 1;
            this.fieldrut1.Caption = "rut";
            this.fieldrut1.FieldName = "rut";
            this.fieldrut1.Name = "fieldrut1";
            this.fieldrut1.Options.ShowInFilter = true;
            // 
            // fieldnombre1
            // 
            this.fieldnombre1.Area = DevExpress.XtraPivotGrid.PivotArea.ColumnArea;
            this.fieldnombre1.AreaIndex = 0;
            this.fieldnombre1.Caption = "nombre";
            this.fieldnombre1.FieldName = "nombre";
            this.fieldnombre1.Name = "fieldnombre1";
            this.fieldnombre1.Options.ShowInFilter = true;
            // 
            // fieldcoditem1
            // 
            this.fieldcoditem1.Area = DevExpress.XtraPivotGrid.PivotArea.ColumnArea;
            this.fieldcoditem1.AreaIndex = 2;
            this.fieldcoditem1.Caption = "coditem";
            this.fieldcoditem1.FieldName = "coditem";
            this.fieldcoditem1.Name = "fieldcoditem1";
            this.fieldcoditem1.Options.ShowInFilter = true;
            // 
            // fieldvalorcalculado1
            // 
            this.fieldvalorcalculado1.Area = DevExpress.XtraPivotGrid.PivotArea.ColumnArea;
            this.fieldvalorcalculado1.AreaIndex = 3;
            this.fieldvalorcalculado1.Caption = "valorcalculado";
            this.fieldvalorcalculado1.FieldName = "valorcalculado";
            this.fieldvalorcalculado1.Name = "fieldvalorcalculado1";
            this.fieldvalorcalculado1.Options.ShowInFilter = true;
            // 
            // fieldorden1
            // 
            this.fieldorden1.AreaIndex = 0;
            this.fieldorden1.Caption = "orden";
            this.fieldorden1.FieldName = "orden";
            this.fieldorden1.Name = "fieldorden1";
            this.fieldorden1.Options.ShowInFilter = true;
            this.fieldorden1.Width = 50;
            // 
            // PageHeader
            // 
            this.PageHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrPivotGrid1});
            this.PageHeader.Name = "PageHeader";
            // 
            // rptTest
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin,
            this.PageHeader});
            this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[] {
            this.sqlDataSource1});
            this.DataMember = "Query";
            this.DataSource = this.sqlDataSource1;
            this.Landscape = true;
            this.Margins = new System.Drawing.Printing.Margins(100, 100, 135, 100);
            this.PageHeight = 850;
            this.PageWidth = 1100;
            this.Version = "18.1";
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.DataAccess.Sql.SqlDataSource sqlDataSource1;
        private DevExpress.XtraReports.UI.XRPivotGrid xrPivotGrid1;
        private DevExpress.XtraReports.UI.PivotGrid.XRPivotGridField fieldrut1;
        private DevExpress.XtraReports.UI.PivotGrid.XRPivotGridField fieldnombre1;
        private DevExpress.XtraReports.UI.PivotGrid.XRPivotGridField fieldcoditem1;
        private DevExpress.XtraReports.UI.PivotGrid.XRPivotGridField fieldvalorcalculado1;
        private DevExpress.XtraReports.UI.PivotGrid.XRPivotGridField fieldorden1;
        private DevExpress.XtraReports.UI.PageHeaderBand PageHeader;
    }
}
