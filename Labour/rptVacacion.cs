using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Windows.Forms;
using System.Data;

namespace Labour
{
    public partial class rptVacacion : DevExpress.XtraReports.UI.XtraReport
    {
        public rptVacacion()
        {
            InitializeComponent();
        }

        private void rptVacacion_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            
        }

        private void xrLabel1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
     
        }

        private void xrTableCell8_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            string data = GetCurrentColumnValue("tipo") + "";
            if (data == "1")
            {
                xrTableCell8.Text = "Normal";
            }
            if (data == "2")
            {
                xrTableCell8.Text = "Progresivo";
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

        private void xrLabel29_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //guardamos valor de la celda
            string celda = GetCurrentColumnValue("rut") + "";
            xrLabel29.Text = fnSistema.fFormatearRut2(celda);
        }

        //SUMO TODOS LOS PROPORCIONALES ANMUALES Y LOS RESTANTES PARA OBTENER EL DÍA DE VACACIONES NORMALES ACUMULADOS
        private void xrTableCell32_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            decimal diasTotalVacacionesAcumuladas = Convert.ToDecimal(Parameters["propanual"].Value);
            decimal diasTotalVacacionesRestantes = Convert.ToDecimal(Parameters["proprestante"].Value);
            xrTableCell32.Text = (diasTotalVacacionesAcumuladas + diasTotalVacacionesRestantes).ToString();
        }
    }
}
