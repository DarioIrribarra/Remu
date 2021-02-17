using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace ReportesExternos
{
    public partial class rptDetalleItemv2 : DevExpress.XtraReports.UI.XtraReport
    {
        public rptDetalleItemv2()
        {
            InitializeComponent();
        }

        private void xrPictureBox1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            Image img = Imagen.GetLogoFromBd();

            if (img != null)
            {
                xrPictureBox1.Image = img;
                xrPictureBox1.Sizing = DevExpress.XtraPrinting.ImageSizeMode.StretchImage;
            }
            else
            {
                xrPictureBox1.Image = null;
            }


        }

        private void RptDetalleItemv2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //Detail.SortFields.Add(new GroupField("nombre", XRColumnSortOrder.Ascending));
        }
    }
}
