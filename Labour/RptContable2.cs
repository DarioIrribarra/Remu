using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace Labour
{
    public partial class RptContable2 : DevExpress.XtraReports.UI.XtraReport
    {
        ReporteContable Rep = new ReporteContable();


        public RptContable2()
        {
            InitializeComponent();
        }

        private void RptContable2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            Rep.SetInfo();                  
          
        }

        //Columna name 1
        private void xrTableCell1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRLabel lbl = sender as XRLabel;
            lbl.WidthF = (float)(Convert.ToDouble(Rep.s1));
            if (Rep.c1 != "0")
            {
                lbl.Text = Rep.n1;
                if (Rep.num1 == 1)
                    lbl.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            }           
        }

        private void xrTableCell2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRLabel lbl = sender as XRLabel;
            lbl.WidthF = (float)(Convert.ToDouble(Rep.s2));
            if (Rep.c2 != "0")
            {
                lbl.Text = Rep.n2;
                if (Rep.num2 == 1)
                    lbl.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            }
         
        }

        private void xrTableCell3_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRLabel lbl = sender as XRLabel;
            lbl.WidthF = (float)(Convert.ToDouble(Rep.s3));
            if (Rep.c3 != "0")
            {
                lbl.Text = Rep.n3;
                if (Rep.num3 == 1)
                    lbl.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            }
         
        }

        private void xrTableCell4_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRLabel lbl = sender as XRLabel;
            lbl.WidthF = (float)(Convert.ToDouble(Rep.s4));
            if (Rep.c4 != "0")
            {
                lbl.Text = Rep.n4;
                if (Rep.num4 == 1)
                    lbl.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            }
           
        }

        private void xrTableCell5_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRLabel lbl = sender as XRLabel;
            lbl.WidthF = (float)(Convert.ToDouble(Rep.s5));
            if (Rep.c5 != "0")
            {
                lbl.Text = Rep.n5;
                if (Rep.num5 == 1)
                    lbl.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            }
           
        }

        private void xrTableCell6_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRLabel lbl = sender as XRLabel;
            lbl.WidthF = (float)(Convert.ToDouble(Rep.s6));
            if (Rep.c6 != "0")
            {
                lbl.Text = Rep.n6;
                if (Rep.num6 == 1)
                    lbl.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            }
            
        }

        private void xrTableCell7_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRLabel lbl = sender as XRLabel;
            lbl.WidthF = (float)(Convert.ToDouble(Rep.s7));
            if (Rep.c7 != "0")
            {
                lbl.Text = Rep.n7;
                if (Rep.num7 == 1)
                    lbl.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            }
           
        }

        private void xrTableCell8_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRLabel lbl = sender as XRLabel;
            lbl.WidthF = (float)(Convert.ToDouble(Rep.s8));
            if (Rep.c8 != "0")
            {
                lbl.Text = Rep.n8;
                if (Rep.num8 == 1)
                    lbl.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            }
            
        }

        //Dato col1
        private void xrTableCell9_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRLabel lbl = sender as XRLabel;
            lbl.WidthF = (float)(Convert.ToDouble(Rep.s1));
            double value = 0;
            lbl.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            if (Rep.num1 == 1)
            {
                if (fnSistema.IsDecimal(GetCurrentColumnValue("Col1").ToString()))
                {
                    value = Convert.ToDouble(GetCurrentColumnValue("Col1"));
                    lbl.Text = string.Format("{0:#,#}", value);
                    lbl.TextAlignment = DevExpress.XtraPrinting.TextAlignment.BottomRight;

                }
            }
            

            if (Rep.c1 == "0" || GetCurrentColumnValue("Col1").ToString() == "0")
            {
                lbl.Text = "";
            }
        }

        private void xrTableCell10_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRLabel lbl = sender as XRLabel;
            lbl.WidthF = (float)(Convert.ToDouble(Rep.s2));
            double value = 0;
            lbl.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            if (Rep.num2 == 1)
            {
                if (fnSistema.IsDecimal(GetCurrentColumnValue("Col2").ToString()))
                {
                    value = Convert.ToDouble(GetCurrentColumnValue("Col2"));
                    lbl.Text = string.Format("{0:#,#}", value);
                    lbl.TextAlignment = DevExpress.XtraPrinting.TextAlignment.BottomRight;

                }
            }
            

            if (Rep.c2 == "0" || GetCurrentColumnValue("Col2").ToString() == "0")
            {
                lbl.Text = "";
            }
        }

        private void xrTableCell11_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRLabel lbl = sender as XRLabel;
            lbl.WidthF = (float)(Convert.ToDouble(Rep.s3));
            double value = 0;
            lbl.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;

            if (Rep.num3 == 1)
            {
                if (fnSistema.IsDecimal(GetCurrentColumnValue("Col3").ToString()))
                {
                    value = Convert.ToDouble(GetCurrentColumnValue("Col3"));
                    lbl.Text = string.Format("{0:#,#}", value);
                    lbl.TextAlignment = DevExpress.XtraPrinting.TextAlignment.BottomRight;
                }
            }            

            if (Rep.c3 == "0" || GetCurrentColumnValue("Col3").ToString() == "0")
            {
                lbl.Text = "";
            }
        }

        private void xrTableCell12_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRLabel lbl = sender as XRLabel;
            lbl.WidthF = (float)(Convert.ToDouble(Rep.s4));
            double value = 0;

            lbl.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;

            if (Rep.num4 == 1)
            {
                if (fnSistema.IsDecimal(GetCurrentColumnValue("Col4").ToString()))
                {
                    value = Convert.ToDouble(GetCurrentColumnValue("Col4"));
                    lbl.Text = string.Format("{0:#,#}", value);
                    lbl.TextAlignment = DevExpress.XtraPrinting.TextAlignment.BottomRight;
                }
            }
           
            if (Rep.c4 == "0" || GetCurrentColumnValue("Col4").ToString() == "0")
            {
                lbl.Text = "";
            }
        }

        private void xrTableCell13_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRLabel lbl = sender as XRLabel;
            lbl.WidthF = (float)(Convert.ToDouble(Rep.s5));
            double value = 0;

            lbl.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            if (Rep.num5 == 1)
            {
                if (fnSistema.IsDecimal(GetCurrentColumnValue("Col5").ToString()))
                {
                    value = Convert.ToDouble(GetCurrentColumnValue("Col5"));
                    lbl.Text = string.Format("{0:#,#}", value);
                    lbl.TextAlignment = DevExpress.XtraPrinting.TextAlignment.BottomRight;
                }
                
            }

            if (Rep.c5 == "0" || GetCurrentColumnValue("Col5").ToString() == "0")
            {
                lbl.Text = "";
            }
        }

        private void xrTableCell14_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRLabel lbl = sender as XRLabel;
            lbl.WidthF = (float)(Convert.ToDouble(Rep.s6));
            double value = 0;

            lbl.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;

            if (Rep.num6 == 1)
            {
                if (fnSistema.IsDecimal(GetCurrentColumnValue("Col6").ToString()))
                {
                    value = Convert.ToDouble(GetCurrentColumnValue("Col6"));
                    lbl.Text = string.Format("{0:#,#}", value);
                    lbl.TextAlignment = DevExpress.XtraPrinting.TextAlignment.BottomRight;
                }
            }

            if (Rep.c6 == "0" || GetCurrentColumnValue("Col6").ToString() == "0")
            {
                lbl.Text = "";
            }
        }

        private void xrTableCell15_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRLabel lbl = sender as XRLabel;
            lbl.WidthF = (float)(Convert.ToDouble(Rep.s7));
            double value = 0;

            lbl.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;

            if (Rep.num7 == 1)
            {
                if (fnSistema.IsDecimal(GetCurrentColumnValue("Col7").ToString()))
                {
                    value = Convert.ToDouble(GetCurrentColumnValue("Col7"));
                    lbl.Text = string.Format("{0:#,#}", value);
                    lbl.TextAlignment = DevExpress.XtraPrinting.TextAlignment.BottomRight;
                }
            }

            if (Rep.c7 == "0" || GetCurrentColumnValue("Col7").ToString() == "0")
            {
                lbl.Text = "";
            }
        }

        private void xrTableCell16_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRLabel lbl = sender as XRLabel;
            lbl.WidthF = (float)(Convert.ToDouble(Rep.s8));
            double value = 0;

            lbl.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            if (Rep.num8 == 1)
            {
                if (fnSistema.IsDecimal(GetCurrentColumnValue("Col8").ToString()))
                {
                    value = Convert.ToDouble(GetCurrentColumnValue("Col8"));
                    lbl.Text = string.Format("{0:#,#}", value);
                    lbl.TextAlignment = DevExpress.XtraPrinting.TextAlignment.BottomRight;
                }
            }

            if (Rep.c8 == "0" || GetCurrentColumnValue("Col8").ToString() == "0")
            {
                lbl.Text = "";
            }
        }

        //total col 1
        private void xrTableCell17_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRLabel lbl = sender as XRLabel;
            lbl.WidthF = (float)(Convert.ToDouble(Rep.s1));           

            if (Rep.c1 == "0" || Rep.num1 == 0)
                lbl.Visible = false;
        }

        private void xrTableCell18_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRLabel lbl = sender as XRLabel;
            lbl.WidthF = (float)(Convert.ToDouble(Rep.s2));

            if (Rep.c2 == "0" || Rep.num2 == 0)
                lbl.Visible = false;
        }

        private void xrTableCell19_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRLabel lbl = sender as XRLabel;
            lbl.WidthF = (float)(Convert.ToDouble(Rep.s3));

            if (Rep.c3== "0" || Rep.num3 == 0)
                lbl.Visible = false;
        }

        private void xrTableCell20_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRLabel lbl = sender as XRLabel;
            lbl.WidthF = (float)(Convert.ToDouble(Rep.s4));

            if (Rep.c4 == "0" || Rep.num4 == 0)
                lbl.Visible = false;
        }

        private void xrTableCell21_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRLabel lbl = sender as XRLabel;
            lbl.WidthF = (float)(Convert.ToDouble(Rep.s5));

            if (Rep.c5 == "0" || Rep.num5 == 0)
                lbl.Visible = false;
        }

        private void xrTableCell22_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRLabel lbl = sender as XRLabel;
            lbl.WidthF = (float)(Convert.ToDouble(Rep.s6));

            if (Rep.c6 == "0" || Rep.num6 == 0)
                lbl.Visible = false;
        }

        private void xrTableCell23_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRLabel lbl = sender as XRLabel;
            lbl.WidthF = (float)(Convert.ToDouble(Rep.s7));

            if (Rep.c7 == "0" || Rep.num7 == 0)
                lbl.Visible = false;
        }

        private void xrTableCell24_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRLabel lbl = sender as XRLabel;
            lbl.WidthF = (float)(Convert.ToDouble(Rep.s8));

            if (Rep.c8 == "0" || Rep.num8 == 0)
                lbl.Visible = false;
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
    }
}
