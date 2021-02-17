using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace ReportesExternos
{
    public partial class rptMovimiento : DevExpress.XtraReports.UI.XtraReport
    {
        public rptMovimiento()
        {
            InitializeComponent();
        }

        private void xrLabel2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRLabel lab = sender as XRLabel;
            string value = "";
            value = GetCurrentColumnValue("tipo").ToString();

            lab.Text = GetNombreTipo(value);
        }

        private string GetNombreTipo(string pType)
        {
            string final = "";

            switch (pType)
            {
                case "1":
                    final = "Haberes Imponibles";
                    break;
                case "2":
                    final = "Haberes No Imponibles";
                    break;
                case "3":
                    final = "Haberes No Imponibles";
                    break;
                case "4":
                    final = "Leyes Sociales";
                    break;
                case "5":
                    final = "Otros Descuentos";
                    break;
                case "6":
                    final = "Aportes";
                    break;
                case "7":
                    final = "Totales";
                    break;
                case "99":
                    final = "Informativos";
                    break;

                default:
                    break;
            }

            return final;
        }

        private void xrPictureBox1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            Image img = Imagen.GetLogoFromBd();

            if (img != null)
            {
                xrPictureBox1.Image = img;
                xrPictureBox1.Sizing = DevExpress.XtraPrinting.ImageSizeMode.StretchImage;
            }
        }
    }
}
