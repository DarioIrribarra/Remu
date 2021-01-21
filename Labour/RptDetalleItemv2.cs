using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace Labour
{
    public partial class RptDetalleItemv2 : DevExpress.XtraReports.UI.XtraReport
    {
        public RptDetalleItemv2()
        {
            InitializeComponent();
        }

        private void xrPictureBox1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            Image img;
            if (Imagen.GetLogoFromBd() != null)
            {
                img = Imagen.GetLogoFromBd();
                xrPictureBox1.Image = img;
                xrPictureBox1.Sizing = DevExpress.XtraPrinting.ImageSizeMode.StretchImage;
            }


        }

        private void RptDetalleItemv2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //Detail.SortFields.Add(new GroupField("nombre", XRColumnSortOrder.Ascending));
        }
    }
}
