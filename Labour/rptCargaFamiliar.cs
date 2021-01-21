using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace Labour
{
    public partial class rptCargaFamiliar : DevExpress.XtraReports.UI.XtraReport
    {
        public rptCargaFamiliar()
        {
            InitializeComponent();
        }

        private void rptCargaFamiliar_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            
               
        }

        private void xrLabel8_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            string value = GetCurrentColumnValue("sexo").ToString();

            if (value == "0")
                xrLabel8.Text = "Masculino";
            else if (value == "1")
                xrLabel8.Text = "Femenino";

        }

        private void xrLabel10_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            string value = GetCurrentColumnValue("parentesco").ToString();

            if (value == "0")
            {
                xrLabel10.Text = "Conyuge";
            }
            else if (value == "1")
            {
                xrLabel10.Text = "Hijo(A)";
            }
            else if (value == "2")
            {
                xrLabel10.Text = "Madre";
            }
            else if (value == "3")
            {
                xrLabel10.Text = "Padre";
            }
        }

        private void xrLabel11_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            string value = GetCurrentColumnValue("relacionlegal").ToString();

            if (value == "0")
            {
                xrLabel11.Text = "Conyuge";
            }
            else if (value == "1")
            {
                xrLabel11.Text = "Hijo(A)";
            }
            else if (value == "2")
            {
                xrLabel11.Text = "Adherente";
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
                Imagen.SinImagen(imgLogo);
            }
        }

        private void ReportHeader_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {

        }

        private void xrLabel15_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //guardamos valor de la celda
            string celda = GetCurrentColumnValue("rutTrabajador") + "";
            xrLabel15.Text = fnSistema.fFormatearRut2(celda);
        }

    }
}
