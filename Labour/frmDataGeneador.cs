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
using DevExpress.XtraPrinting;
using DevExpress.XtraPrintingLinks;
using DevExpress.Export.Xl;
using DevExpress.XtraGrid.Columns;

namespace Labour
{
    public partial class frmDataGeneador : DevExpress.XtraEditors.XtraForm
    {
        private DataTable DataSource { get; set; }

        /*Nombre del reporte que se quiere generar*/
        private string NombreReporte = "";
        private int ColumnIndexFocusMenu = 0;
        //Indica si se dio o no formato rut a una columna
        private bool FormatoRut = false;
        public frmDataGeneador()
        {
            InitializeComponent();
        }

        public frmDataGeneador(DataTable pData)
        {
            InitializeComponent();
            this.DataSource = pData;
        }

        private void frmDataGeneador_Load(object sender, EventArgs e)
        {
            viewData.OptionsPrint.PrintVertLines = false;
            viewData.OptionsPrint.PrintHorzLines = false;            

            try
            {
                if (DataSource != null)
                {
                    if (DataSource.Rows.Count > 0)
                    {
                        gridata.DataSource = this.DataSource;                        
                        //gridata.DataMember = "data";
                        fnSistema.spOpcionesGrilla(viewData);
                        viewData.OptionsCustomization.AllowColumnMoving = true;
                        viewData.OptionsCustomization.AllowFilter = true;
                        viewData.OptionsMenu.EnableColumnMenu = true;
                        viewData.BestFitColumns();

                        viewData.PrintInitialize += ViewData_PrintInitialize;
                    }
                }
            }
            catch (Exception ex)
            {
                //ERROR...
            }
        }

       

        private void ViewData_PrintInitialize(object sender, DevExpress.XtraGrid.Views.Base.PrintInitializeEventArgs e)
        {
            (e.PrintingSystem as PrintingSystemBase).PageSettings.Landscape = true;
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            try
            {
                if (gridata.DataSource != null)
                {
                    if (viewData.RowCount > 0)
                    {
                        SaveFileDialog di = new SaveFileDialog();
                        //di.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
                        di.Filter = "Excel Files|*.xlsx";
                        di.Title = "Guardar en...";
                        di.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                        if (di.ShowDialog() == DialogResult.OK)
                        {
                            //ExportOptions(di.FileName);
                            viewData.BestFitColumns();
                            XlsxExportOptions op = new XlsxExportOptions();
                            if (FormatoRut == false)
                                op.TextExportMode = TextExportMode.Value;
                            else
                                op.TextExportMode = TextExportMode.Text;
                            gridata.ExportToXlsx(di.FileName, op);
                            XtraMessageBox.Show("Archivo generado en " + di.FileName, "Archivo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            if (File.Exists(di.FileName))
                            {
                                DialogResult open = XtraMessageBox.Show("¿Deseas ver archivo?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                                if (open == DialogResult.Yes)
                                    System.Diagnostics.Process.Start(di.FileName);
                            }
                        }
                    }
                    else
                    {
                        XtraMessageBox.Show("No se pudo exportar la información", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                }
                else
                {
                    XtraMessageBox.Show("No se pudo exportar la información", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
            catch (Exception ex)
            {
                //ERROR...
                XtraMessageBox.Show("No se pudo exportar la información", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void btnExportPdf_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            try
            {
                if (gridata.DataSource != null)
                {
                    if (viewData.RowCount > 0)
                    {
                        SaveFileDialog di = new SaveFileDialog();
                        di.Filter = "Pdf Files|*.pdf";
                        di.Title = "Guardar en...";
                        di.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                        if (di.ShowDialog() == DialogResult.OK)
                        {                           
                            gridata.ExportToPdf(di.FileName);
                            XtraMessageBox.Show("Archivo generado en " + di.FileName, "Archivo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            //Options();

                            if (File.Exists(di.FileName))
                            {
                                DialogResult open = XtraMessageBox.Show("¿Deseas ver archivo", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                                if (open == DialogResult.Yes)
                                    System.Diagnostics.Process.Start(di.FileName);
                            }
                        }
                    }
                    else
                    {
                        XtraMessageBox.Show("No se pudo exportar la información", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                }
                else
                {
                    XtraMessageBox.Show("No se pudo exportar la información", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
            catch (Exception ex)
            {
                //ERROR...
                XtraMessageBox.Show("No se pudo exportar la información", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void btnExportWord_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            try
            {
                if (gridata.DataSource != null)
                {
                    if (viewData.RowCount > 0)
                    {
                        SaveFileDialog di = new SaveFileDialog();
                        di.Filter = "Word Documents|*.docx";
                        di.Title = "Guardar en...";
                        di.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                        if (di.ShowDialog() == DialogResult.OK)
                        {
                            gridata.ExportToDocx(di.FileName);
                            XtraMessageBox.Show("Archivo generado en " + di.FileName, "Archivo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            if (File.Exists(di.FileName))
                            {
                                DialogResult open = XtraMessageBox.Show("¿Deseas ver archivo", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                                if (open == DialogResult.Yes)
                                    System.Diagnostics.Process.Start(di.FileName);
                            }
                        }
                    }
                    else
                    {
                        XtraMessageBox.Show("No se pudo exportar la información", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                }
                else
                {
                    XtraMessageBox.Show("No se pudo exportar la información", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
            catch (Exception ex)
            {
                //ERROR...
                XtraMessageBox.Show("No se pudo exportar la información", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void ExportOptions(string FilePath)
        {
            XlsxExportOptionsEx Options = new XlsxExportOptionsEx();
            Options.CustomizeSheetHeader += Options_CustomizeSheetHeader;

            viewData.ExportToXlsx(FilePath, Options);
        }

        private void Options_CustomizeSheetHeader(DevExpress.Export.ContextEventArgs e)
        {
            e.ExportContext.AddRow(new object[] {"Todos los items subase del mes"});
            //e.ExportContext.InsertImage(Properties.Resources.accountx16, new XlCellRange(new XlCellPosition(0, 0), new XlCellPosition(0,0)));
            e.ExportContext.MergeCells(new XlCellRange(new XlCellPosition(0, 0), new XlCellPosition(viewData.VisibleColumns.Count - 1, 0)));

        }

        private void viewData_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.MenuType == DevExpress.XtraGrid.Views.Grid.GridMenuType.Column)
            {
                DevExpress.Utils.Menu.DXMenuItem item = new DevExpress.Utils.Menu.DXMenuItem();
                item.Caption = "Dar formato Rut";
                ColumnIndexFocusMenu = e.HitInfo.Column.AbsoluteIndex;
                item.Click += Item_Click;
                
                e.Menu.Items.Add(item);
            }          
    
        }

        private void Item_Click(object sender, EventArgs e)
        {
            if (viewData.RowCount > 0)
            {
                GridColumnCollection columns =  viewData.Columns;
                if (columns.Count > 0)
                {
                    foreach (GridColumn cl in columns)
                    {
                        if (cl.AbsoluteIndex == ColumnIndexFocusMenu)
                        {
                            cl.DisplayFormat.Format = new FormatCustom();
                            cl.DisplayFormat.FormatString = "Rut";
                            FormatoRut = true;                           


                        }
                    }
                }
            }
        }
    }
}