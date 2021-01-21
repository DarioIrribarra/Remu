using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace Labour
{
    public partial class rptPlanillaIsapre : DevExpress.XtraReports.UI.XtraReport
    {
        public rptPlanillaIsapre()
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
                imgLogo.Image = null;
            }
        }

        private void xrLabel7_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
           
        }

        private void ReplaceValue(string pColumnName, XRLabel pField)
        {
            int value = 0;
            try
            {
                object data = this.GetCurrentColumnValue(pColumnName);
                if (data != null)
                    value = Convert.ToInt32(data);

                if (value == 0)
                {
                    pField.Text = "";
                }
            }
            catch (Exception ex)
            {
                //ERROR
                pField.Text = "";
            }
        }

        private void ReplaceParameter(XRLabel pField)
        {
            string value = pField.Text;

            if (value.Length > 0)
            {
                if (value.Contains("."))
                    value = value.Replace(".", "");

                if (Convert.ToInt32(value) == 0)
                    pField.Text = "";
            }
        }

        private void xrLabel8_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
         
        }

        private void xrLabel9_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
           
        }

        private void xrLabel10_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
           
        }

        private void xrLabel11_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
          
        }

        private void xrLabel26_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
           
        }

        private void xrLabel24_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
          
        }

        private void xrLabel16_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
          
        }

        private void xrLabel17_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            
        }

        private void xrLabel18_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            
        }

        private void xrLabel19_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            
        }

        private void xrLabel20_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            
        }
    }
}
