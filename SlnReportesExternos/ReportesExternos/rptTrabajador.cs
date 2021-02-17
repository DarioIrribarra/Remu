using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace ReportesExternos
{
    public partial class rptTrabajador : DevExpress.XtraReports.UI.XtraReport
    {
        public rptTrabajador()
        {
            InitializeComponent();
        }

        private void xrLabel6_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            string data = GetCurrentColumnValue("sexo").ToString();
            if (data == "0")
            {
                xrLabel6.Text = "MASCULINO";
            }
            else if (data == "1")
            {
                xrLabel6.Text = "FEMENINO";
            }
        }

        private void xrLabel41_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            string data = GetCurrentColumnValue("caja").ToString();
            if (data == "")
            {
                xrLabel41.Text = "No Aplica";
            }
        }

        private void xrLabel27_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            string data = GetCurrentColumnValue("tipocontrato").ToString();
            if (data == "0")
            {
                xrLabel27.Text = "Indefinido";
            }
            else if (data == "1")
            {
                xrLabel27.Text = "Plazo Fijo";
            }
            else if (data == "2")
            {
                xrLabel27.Text = "Faena";
            }
        }

        private void xrLabel20_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            string data = GetCurrentColumnValue("rut").ToString();
            if (data != "")
            {
               xrLabel20.Text = fnSistema.fFormatearRut2(data);
            }
        }

        private void xrLabel55_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            DateTime nacimiento = (DateTime)GetCurrentColumnValue("fechanac");
            DateTime today = DateTime.Now.Date;

            int edad = 0;
            edad = today.Year - nacimiento.Year;
            xrLabel55.Text = edad + " Años";
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

        private void PictureEmpleado_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            Image img = Imagen.GetImagenTrabajador(xrLabel53.Text);
            if (img != null)
            {
                PictureEmpleado.Image = img;
                PictureEmpleado.Sizing = DevExpress.XtraPrinting.ImageSizeMode.StretchImage;
            }
        }
    }
}
