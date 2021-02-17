using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace ReportesExternos
{
    public partial class rptComprobanteAusentismo : DevExpress.XtraReports.UI.XtraReport
    {
        public rptComprobanteAusentismo()
        {
            InitializeComponent();
        }

        //IMÁGENES
        private void imgLogo1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {

            Image img = Imagen.GetLogoFromBd();

            if (img != null)
            {
                imgLogo1.Image = img;
                imgLogo1.Sizing = DevExpress.XtraPrinting.ImageSizeMode.StretchImage;
            }
            else
            {
                imgLogo1.Image = null;
            }
        }

        private void imgLogo2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            Image img = Imagen.GetLogoFromBd();

            if (img != null)
            {
                imgLogo2.Image = img;
                imgLogo2.Sizing = DevExpress.XtraPrinting.ImageSizeMode.StretchImage;
            }
            else
            {
                imgLogo2.Image = null;
            }
        }

        private void xrLabel19_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            
            
        }

        private void xrLabel22_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //guardamos valor de la celda
            string celda = GetCurrentColumnValue("rutTrabajador") == null ? "11.111.111-1" : GetCurrentColumnValue("rutTrabajador").ToString();
            xrLabel22.Text = fnSistema.fFormatearRut2(celda);
            xrLabel22.WidthF = xrLabel22.Text.Length * 10;
        }

        private void xrLabel42_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //guardamos valor de la celda
            string celda = GetCurrentColumnValue("rutTrabajador") == null ? "11.111.111-1" : GetCurrentColumnValue("rutTrabajador").ToString();
            xrLabel42.Text = fnSistema.fFormatearRut2(celda);
            xrLabel42.WidthF = xrLabel42.Text.Length * 10;
        }

        private void rptComprobanteAusentismo_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {

        }

        //NUMERO A LA DERECHA
        private void xrTableCell59_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            int lengthTexto = xrTableCell59.Text.Length;
            xrTableCell59.WidthF = lengthTexto * 10;
            xrTable5.LocationF = new DevExpress.Utils.PointFloat(xrTable5.LocationF.X - (lengthTexto + 15), xrTable5.LocationF.Y);
        }

        private void xrTableCell3_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            int lengthTexto = xrTableCell3.Text.Length;
            xrTableCell3.WidthF = lengthTexto * 10;
            xrTable1.LocationF = new DevExpress.Utils.PointFloat(xrTable1.LocationF.X - (lengthTexto + 15), xrTable1.LocationF.Y);
        }

        private void xrLabel7_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            
            string cadena = GetCurrentColumnValue("nombre") == null ? "NOMBRE COMPLETO TRABAJADOR ASOCIADO" : GetCurrentColumnValue("nombre").ToString();
            xrLabel7.WidthF = (cadena.Length * 10) + 1;
            xrLabel7.Text = cadena + ",";

        }

        private void xrLabel34_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            string cadena = GetCurrentColumnValue("nombre") == null ? "NOMBRE COMPLETO TRABAJADOR ASOCIADO" : GetCurrentColumnValue("nombre").ToString();
            xrLabel34.WidthF = (cadena.Length * 10) + 1;
            xrLabel34.Text = cadena + ",";
        }

        private void xrLabel9_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            string cadena = GetCurrentColumnValue("motivo") == null ? "MOTIVO AUSENTISMO JUSTIFICADO" : GetCurrentColumnValue("motivo").ToString();
            xrLabel9.WidthF = (cadena.Length * 10) + 1;
            xrLabel9.Text = cadena + ",";
        }

        private void xrLabel32_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            string cadena = GetCurrentColumnValue("motivo") == null ? "MOTIVO AUSENTISMO JUSTIFICADO" : GetCurrentColumnValue("motivo").ToString();
            xrLabel32.WidthF = (cadena.Length * 10) + 1;
            xrLabel32.Text = cadena + ",";
        }
    }
}
