using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace ReportesExternos
{
    public partial class rptPlanillaCaja : DevExpress.XtraReports.UI.XtraReport
    {
        public rptPlanillaCaja()
        {
            InitializeComponent();
        }

        private void xrLabel10_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //ReplaceValue("ValorIsapre", xrLabel10);
        }

        private void ReplaceValue(string pColumnName, XRLabel pField)
        {
            int value = 0;
            value = Convert.ToInt32(this.GetCurrentColumnValue(pColumnName));

            if (value == 0)
            {
                pField.Text = "";
            }
        }

        private void xrLabel11_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //ReplaceValue("ValorSinIsapre", xrLabel11);
        }

        private void xrLabel12_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //ReplaceValue("CotizacionCaja", xrLabel12);
        }

        private void xrLabel17_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //ReplaceValue("AsignacionFamiliar", xrLabel17);
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

        private void xrLabel18_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //ReplaceValue("Tramo", xrLabel18);
        }

        private void xrLabel13_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //ReplaceValue("DiasTrabajados", xrLabel13);
        }

        private void xrLabel14_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //ReplaceValue("Simples", xrLabel14);
        }

        private void xrLabel15_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //ReplaceValue("invalidas", xrLabel15);
        }

        private void xrLabel16_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //ReplaceValue("Maternal", xrLabel16);
        }

        private void xrLabel19_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //ReplaceValue("CodMovimiento", xrLabel19);
        }

        private void xrTableCell30_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //ReplaceParameter(xrTableCell30);
        }

        private void xrTableCell31_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //ReplaceParameter(xrTableCell31);
        }

        private void xrTableCell36_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //ReplaceParameter(xrTableCell36);
        }

        private void xrTableCell39_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //ReplaceParameter(xrTableCell39);
        }

        private void xrTableCell45_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //ReplaceParameter(xrTableCell45);
        }

        private void xrTableCell33_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //ReplaceParameter(xrTableCell33);
        }

        private void xrTableCell34_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //ReplaceParameter(xrTableCell34);
        }

        private void xrTableCell37_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //ReplaceParameter(xrTableCell37);
        }

        private void xrTableCell40_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //ReplaceParameter(xrTableCell40);
        }

        private void xrTableCell46_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //ReplaceParameter(xrTableCell46);
        }

        private void xrTableCell48_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //ReplaceParameter(xrTableCell48);
        }

        private void xrTableCell49_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //ReplaceParameter(xrTableCell49);
        }

        private void xrTableCell50_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //ReplaceParameter(xrTableCell50);
        }

        private void xrTableCell51_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //ReplaceParameter(xrTableCell51);
        }

        private void xrTableCell53_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //ReplaceParameter(xrTableCell53);
        }

        private void xrTableCell62_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
           // ReplaceParameter(xrTableCell62);
        }

        private void xrTableCell63_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
          //  ReplaceParameter(xrTableCell63);
        }

        private void xrTableCell64_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
           // ReplaceParameter(xrTableCell64);
        }

        private void xrTableCell65_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
           // ReplaceParameter(xrTableCell65);
        }

        private void xrTableCell66_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
           // ReplaceParameter(xrTableCell66);
        }

        private void xrTableCell67_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //ReplaceParameter(xrTableCell67);
        }

        private void xrTableCell69_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
           // ReplaceParameter(xrTableCell69);
        }

        private void xrTableCell70_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //ReplaceParameter(xrTableCell70);
        }

        private void xrTableCell71_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //ReplaceParameter(xrTableCell71);
        }

        private void xrTableCell72_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //ReplaceParameter(xrTableCell72);
        }

        private void xrTableCell73_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //ReplaceParameter(xrTableCell73);
        }

        private void xrTableCell74_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
           // ReplaceParameter(xrTableCell74);
        }

        private void xrTableCell76_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
           // ReplaceParameter(xrTableCell76);
        }

        private void xrTableCell77_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
           // ReplaceParameter(xrTableCell77);
        }

        private void xrTableCell78_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
           // ReplaceParameter(xrTableCell78);
        }

        private void xrTableCell79_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
           // ReplaceParameter(xrTableCell79);
        }

        private void xrTableCell80_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
           // ReplaceParameter(xrTableCell80);
        }

        private void xrTableCell81_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
           // ReplaceParameter(xrTableCell81);
        }

        private void xrTableCell83_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
           // ReplaceParameter(xrTableCell83);
        }

        private void xrTableCell84_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
           // ReplaceParameter(xrTableCell84);
        }

        private void xrTableCell85_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
          //  ReplaceParameter(xrTableCell85);
        }

        private void xrTableCell86_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
           // ReplaceParameter(xrTableCell86);
        }

        private void xrTableCell87_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
           // ReplaceParameter(xrTableCell87);
        }

        private void xrTableCell88_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
           // ReplaceParameter(xrTableCell88);
        }

        private void xrTableCell90_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
           // ReplaceParameter(xrTableCell90);
        }

        private void xrTableCell91_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
          //  ReplaceParameter(xrTableCell91);
        }

        private void xrTableCell92_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
           // ReplaceParameter(xrTableCell92);
        }

        private void xrTableCell93_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
           // ReplaceParameter(xrTableCell93);
        }

        private void xrTableCell95_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
           // ReplaceParameter(xrTableCell95);
        }

        private void xrTableCell97_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
          //  ReplaceParameter(xrTableCell97);
        }

        private void xrTableCell98_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
           // ReplaceParameter(xrTableCell98);
        }

        private void xrTableCell99_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
           // ReplaceParameter(xrTableCell99);
        }

        private void xrTableCell100_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
           // ReplaceParameter(xrTableCell100);
        }

        private void xrTableCell101_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
          //  ReplaceParameter(xrTableCell101);
        }

        private void xrTableCell102_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
           // ReplaceParameter(xrTableCell102);
        }
    }
}
