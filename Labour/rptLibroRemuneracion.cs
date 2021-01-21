using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Drawing.Printing;
using System.Data;
using System.Windows.Forms;

namespace Labour
{
    public partial class rptLibroRemuneracion : DevExpress.XtraReports.UI.XtraReport
    {
        public rptLibroRemuneracion()
        {
            InitializeComponent();
        }    

        private void imgLogo_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            Image img = Imagen.GetLogoFromBd();

            if (img != null)
            {
                imgLogo.Image = img;
                imgLogo.Sizing = DevExpress.XtraPrinting.ImageSizeMode.StretchImage;
            }
            else
            {
                Imagen.SinImagen(imgLogo);
            }
        }

        private void xrLabel2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
        }

        private void Detail_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
           /* DetailBand detail = sender as DetailBand;
            detail.Band.Controls.Clear();
            detail.HeightF = 0;

            //AGREGAMOS TABLA
            XRTable table = new XRTable();
            detail.Band.Controls.Add(table);

            table.BeginInit();

            //TODOS LOS BORDERS
            table.Borders = DevExpress.XtraPrinting.BorderSide.All;
            //CENTRADA
            table.LocationF = new PointF(0, 0);
            table.WidthF = this.PageWidth - this.Margins.Left - this.Margins.Right;

            //CREAMOS UNA FILA
            XRTableRow row = new XRTableRow();
            table.Rows.Add(row);

            //CAMPOS QUE VAMOS A INGRESAR
            string[] fields = new string[] {"Afp", "anticipo", "base"};

            XRTableCell cell = new XRTableCell();*/       
          

            /*foreach (string fieldname in fields)
            {
                if (GetCurrentColumnValue(fieldname).ToString() != string.Empty)
                {
                    XRTableCell cell = new XRTableCell();
                    row.Cells.Add(cell);
                    cell.DataBindings.Add("Text",null, fieldname);
                }
            }*/

            //table.EndInit();

        }

        private void rptLibroRemuneracion_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            this.Landscape = true;
            this.Detail.Controls.Add(CreateXRtable());
            MessageBox.Show("asd");
        }

        public XRTable CreateXRtable()
        {
            XRTable table = new XRTable();

            //CANTIDAD DE CELDAS QUE TENDRÁ LA FILA
            if (Libro.TablaLibroRemuneraciones.Rows.Count>0)
            {
                int cellsInRow = Libro.TablaLibroRemuneraciones.Columns.Count;
                int rowsCount = Libro.TablaLibroRemuneraciones.Rows.Count;
                float rowHeight = 25f;                
           
                table.Borders = DevExpress.XtraPrinting.BorderSide.All;
                table.BeginInit();

                //RECORREMOS COLUMNAS Y GENERAMOS HEADERS
                XRTableRow rowHeader = new XRTableRow();
                rowHeader.HeightF = rowHeight;

                /*foreach (DataColumn column in Libro.TablaLibroRemuneraciones.Columns)
                {
                    if (column.ColumnName != "contrato")
                    {
                        XRTableCell cellHeader = new XRTableCell();
                        cellHeader.Text = column.ColumnName;
                        cellHeader.Font = new Font("Arial", 11, FontStyle.Bold);
                        rowHeader.Cells.Add(cellHeader);
                    }                    
                }*/

                
                //table.Rows.Add(rowHeader);

                //AGREGAMOS DATA
                foreach (DataRow row in Libro.TablaLibroRemuneraciones.Rows)
                {
                    //CREAMOS UN XRTABLEROW
                    XRTableRow FilaContrato = new XRTableRow();
                    FilaContrato.Cells.Add(new XRTableCell() { Text = "Contrato"});
                    FilaContrato.Cells.Add(new XRTableCell() { Text = row["contrato"].ToString() });                 

                    table.Rows.Add(FilaContrato);

                    XRTableRow FilaHeader = new XRTableRow();
                    XRTableRow Fila = new XRTableRow();
                    foreach (DataColumn column in Libro.TablaLibroRemuneraciones.Columns)
                    {
                        if (column.ColumnName != "contrato")
                        {
                            XRTableCell CellHeader = new XRTableCell();
                            CellHeader.Text = column.ColumnName;
                            CellHeader.Font = new Font("Arial", 10, FontStyle.Bold);
                            FilaHeader.Cells.Add(CellHeader);
                        }

                        if (column.ColumnName != "contrato")
                        {
                            XRTableCell Celda = new XRTableCell();
                            Celda.Text = row[column].ToString();
                            Fila.Cells.Add(Celda);
                        }                        
                    }
                    table.Rows.Add(FilaHeader);
                    table.Rows.Add(Fila);                  
                }

                table.BeforePrint += new System.Drawing.Printing.PrintEventHandler(table_beforeprint);
                table.AdjustSize();
                table.EndInit();
            }           
            return table;
        }

        private void table_beforeprint(object sender, PrintEventArgs e)
        {
            XRTable table = ((XRTable)sender);
            table.LocationF = new DevExpress.Utils.PointFloat(0F, 0F);
            table.WidthF = this.PageWidth - this.Margins.Left - this.Margins.Right;
        }

    }
}
