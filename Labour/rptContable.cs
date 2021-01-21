using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Collections.Generic;
using System.Data;

namespace Labour
{
    public partial class rptContable : DevExpress.XtraReports.UI.XtraReport
    {
        public DataTable pDataSource { get; set; } = new DataTable();

        public rptContable()
        {
            InitializeComponent();
        }

        public rptContable(DataTable pData)
        {
            InitializeComponent();
            pDataSource = pData;
        }


        private void xrLabel8_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {

        }

        private void xrLabel7_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
          
        }

        private void xrPictureBox1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            try
            {
                Image img = Imagen.GetLogoFromBd();

                if (img != null)
                {
                    xrPictureBox1.Image = img;
                    xrPictureBox1.Sizing = DevExpress.XtraPrinting.ImageSizeMode.StretchImage;
                }
            }
            catch (Exception ex)
            {
               //ERROR
            }
        }

        private void LoadData(XRTableCell pCelda, int pColumnNumber)
        {
            List<ReporteContableDetalle> Columnas = new List<ReporteContableDetalle>();
            string Data = "";
            if (pDataSource.Rows.Count > 0)
            {
                try
                {                 

                    ReporteContableDetalle detalle = new ReporteContableDetalle();
                    Columnas = detalle.GetDetail();

                    if (Columnas.Count > 0 && pDataSource != null)
                    {
                        foreach (DataRow row in pDataSource.Rows)
                        {
                            foreach (ReporteContableDetalle col in Columnas)
                            {
                                //Columnas iguales???
                                if (col.Column == pColumnNumber)
                                {
                                    pCelda.Text = row["col" + col.ColumnValue].ToString();
                                    break;
                                }
                            }
                            
                        }
                    }
                    
                }
                catch (Exception ex)
                {
                    //ERROR...
                }
            }
        }



        private void rptContable_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //LoadData();
            
        }

        private void xrTable1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            
            
        }

        private void xrTableCell1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //XRTableCell celda = sender as XRTableCell;
            //LoadData(celda, 1);
        }

        private void xrTableCell2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //XRTableCell celda = sender as XRTableCell;
            //LoadData(celda, 2);
        }

        private void xrTableCell3_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //XRTableCell celda = sender as XRTableCell;
            //LoadData(celda, 3);
        }

        private void xrTableCell4_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //XRTableCell celda = sender as XRTableCell;
            //LoadData(celda, 4);
        }

        private void xrTableCell5_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //XRTableCell celda = sender as XRTableCell;
            //LoadData(celda, 5);
        }

        private void xrTableCell6_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
           
        }

        
     

        private void xrTableCell9_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //XRTableCell celda = sender as XRTableCell;
            //LoadData(celda, 9);
        }

        private void xrTableCell10_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //XRTableCell celda = sender as XRTableCell;
            //LoadData(celda, 10);
        }

        private void xrTableCell11_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //XRTableCell celda = sender as XRTableCell;
            //LoadData(celda, 11);
        }

        private void xrLabel8_BeforePrint_1(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //XRTableCell celda = sender as XRTableCell;
            //LoadData(celda, 6);
            string value = GetCurrentColumnValue("col7").ToString();
            try
            {
                if (fnSistema.IsDecimal(value))
                {
                    if (Convert.ToDouble(value) == 0)
                        xrLabel8.Text = "";
                }
            }
            catch (Exception ex)
            {

            }
          
        }

      

        private void xrLabel7_BeforePrint_2(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell celda = sender as XRTableCell;
            //LoadData(celda, 6);

            string value = GetCurrentColumnValue("col6").ToString();
            try
            {
                if (fnSistema.IsDecimal(value))
                {
                    if (Convert.ToDouble(value) == 0)
                        xrLabel7.Text = "";
                }
            }
            catch (Exception ex)
            {

            }

        }

        private void xrLabel6_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell celda = sender as XRTableCell;
            //LoadData(celda, 6);

            string value = GetCurrentColumnValue("col9").ToString();
            try
            {
                if (fnSistema.IsDecimal(value))
                {
                    if (Convert.ToDouble(value) == 0)
                        xrLabel6.Text = "";
                }
            }
            catch (Exception ex)
            {

            }

        }

        private void xrLabel1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {

        }
    }
}
