using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Data;

namespace Labour
{
    public partial class RptResumenPrevired : DevExpress.XtraReports.UI.XtraReport
    {

        public RptResumenPrevired()
        {
            InitializeComponent();

           // ValorCajaCompensacion = SumaCaja(Convert.ToInt32(this.Parameters["periodoCalculo"].Value));

        }

        private void xrTable2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
          

        }

   

        private void xrTableCell6_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
          
        }

        private void xrTableCell7_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
           
          
                    

        }

        private void RptResumenPrevired_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            
        }

        private void xrLabel4_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {            
            
            
        }

        private void xrLabel7_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //RESTAMOS EL VALOR DE CAJA
            
           /* double valorCelda = 0;
            valorCelda = Convert.ToDouble(xrLabel7.Value);
            ValorCajaCompensacion = SumaCaja(Convert.ToInt32(this.Parameters["periodoCalculo"].Value));
            valorCelda = Math.Round(valorCelda - ValorCajaCompensacion);
            xrLabel7.Text = string.Format("{0:#,000.00}", valorCelda);*/
            
        }

        private void xrTableCell12_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {            
            
        }

        private void pictureLogo_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            Image img = Imagen.GetLogoFromBd();

            if (img != null)
            {
                pictureLogo.Image = img;
                pictureLogo.Sizing = DevExpress.XtraPrinting.ImageSizeMode.StretchImage;
            }
            else
            {
                pictureLogo.Image = null;
            }
        }

        private void xrLabel10_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            Show(xrLabel10);
            
        }

        private void Show(XRLabel pControl)
        {

            try
            {
                string value = pControl.Value.ToString();
                if (value == "0")
                    pControl.Value = "";
            }
            catch (Exception ex)
            {
               //ERROR...
            }
        }
    }
}
