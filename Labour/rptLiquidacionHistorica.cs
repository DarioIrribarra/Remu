using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace Labour
{
    public partial class rptLiquidacionHistorica : DevExpress.XtraReports.UI.XtraReport
    {
        public rptLiquidacionHistorica()
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
    }
}
