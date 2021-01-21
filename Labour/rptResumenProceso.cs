using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;


namespace Labour
{
    public partial class rptResumenProceso : DevExpress.XtraReports.UI.XtraReport
    {
        public rptResumenProceso()
        {
            InitializeComponent();
        }

        private void rptResumenProceso_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {        
                
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
    }
}
