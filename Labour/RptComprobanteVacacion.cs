using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace Labour
{
    public partial class RptComprobanteVacacion : DevExpress.XtraReports.UI.XtraReport
    {
        public RptComprobanteVacacion()
        {
            InitializeComponent();
        }

        /*RUT*/
        private void xrLabel19_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //guardamos valor de la celda
            string celda = GetCurrentColumnValue("rut") + "";
            xrLabel19.Text = fnSistema.fFormatearRut2(celda);
        }

        private void xrLabel28_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //guardamos valor de la celda
            string celda = GetCurrentColumnValue("rut") + "";
            xrLabel28.Text = fnSistema.fFormatearRut2(celda);
        }

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
                Imagen.SinImagen(imgLogo1);
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
                Imagen.SinImagen(imgLogo2);
            }
        }

        /*FOLIO A LA DERECHA*/
        private void xrTableCell59_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            int lengthTexto = xrTableCell59.Text.Length;
            xrTableCell59.WidthF = lengthTexto * 10;
            xrTable5.LocationF = new DevExpress.Utils.PointFloat(xrTable5.LocationF.X - (lengthTexto + 2), xrTable5.LocationF.Y);
        }

        private void xrTableCell62_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            int lengthTexto = xrTableCell62.Text.Length;
            xrTableCell62.WidthF = lengthTexto * 10;
            xrTable6.LocationF = new DevExpress.Utils.PointFloat(xrTable6.LocationF.X - (lengthTexto + 2), xrTable6.LocationF.Y);
        }

        private void xrTableCell56_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            string data = GetCurrentColumnValue("tipo") + "";
            if (data == "1")
            {
                xrTableCell56.Text = "Normal";
            }
            if (data == "2")
            {
                xrTableCell56.Text = "Progresivo";
            }
        }

        private void xrTableCell30_BeforePrint_1(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            string data = GetCurrentColumnValue("tipo") + "";
            if (data == "1")
            {
                xrTableCell30.Text = "Normal";
            }
            if (data == "2")
            {
                xrTableCell30.Text = "Progresivo";
            }
        }

        private void xrTableCell63_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            string data = GetCurrentColumnValue("tipo") + "";
            if (data == "1")
            {
                xrTableCell63.Text = "Días normales a usar";
            }
            if (data == "2")
            {
                xrTableCell63.Text = "Días progresivos a usar";
            }
        }

        //PARÁMETROS PARA EL TEXTO
        private void xrlblDias_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            string dato = GetCurrentColumnValue("dias") + "";
            string tipoVacaciones = GetCurrentColumnValue("tipo") + "";

            xrlblDias.Text = dato;
            xrLabel17.Text = dato;

            if (double.Parse(dato) > 1.00)
            {
                xrLabel1.Text = "días feriados";
                xrLabel36.Text = "días feriados";
            }
            else
            {
                xrLabel1.Text = "día feriado";
                xrLabel36.Text = "día feriado";
            }

            if (tipoVacaciones == "1" && double.Parse(dato) > 1.00)
            {
                xrLabel16.Text = "normales";
                xrLabel4.Text = "normales";
            }
            else if (tipoVacaciones == "1" && double.Parse(dato) <= 1.00)
            {
                xrLabel16.Text = "normal";
                xrLabel4.Text = "normal";
            }
            else if (tipoVacaciones == "2" && double.Parse(dato) > 1.00)
            {
                xrLabel16.Text = "progresivos";
                xrLabel4.Text = "progresivos";
            }
            else
            {
                xrLabel16.Text = "progresivo";
                xrLabel4.Text = "progresivo";
            }

        }

        //MOSTRAR DIAS EN PROPORCIONALES
        private void xrTableCell10_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //decimal dias = Convert.ToDecimal(Parameters["propUsadosRepo"].Value);
            //string tipoVacaciones = GetCurrentColumnValue("tipo").ToString();
            //if (tipoVacaciones == "1")
            //{
            //    xrTableCell10.Text = dias.ToString("n2");
            //    xrTableCell13.Text = dias.ToString("n2");
            //}
            //else
            //{
            //    xrTableCell10.Text = "0.00";
            //    xrTableCell13.Text = "0.00";
            //}
        }

        //MOSTRAR DIAS EN PROGRESIVOS
        private void xrTableCell96_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //decimal dias = Convert.ToDecimal(Parameters["progUsadosRepo"].Value);
            //string tipoVacaciones = GetCurrentColumnValue("tipo").ToString();
            //if (tipoVacaciones == "2")
            //{
            //    xrTableCell96.Text = dias.ToString("n2");
            //    xrTableCell69.Text = dias.ToString("n2");
            //}
            //else
            //{
            //    xrTableCell96.Text = "0.00";
            //    xrTableCell69.Text = "0.00";
            //}
        }

        private void xrTableCell83_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //Dias proporcionales usados.
            double propRestantes = Convert.ToDouble(Parameters["proprestante"].Value);
            double propUsados = Convert.ToDouble(Parameters["propusados"].Value);
            double VacAcumuladas = Convert.ToDouble(Parameters["diasVacacionesAcumuladas"].Value);
            //Dias usados..
            double dias = Convert.ToDouble(GetCurrentColumnValue("dias").ToString());
            string tipoVacaciones = GetCurrentColumnValue("tipo").ToString();

            if (propUsados > 0)
            {
                string resultado = (VacAcumuladas - propUsados).ToString("N2");

                xrTableCell83.Text = resultado;
                xrTableCell35.Text = resultado;
            }
            else
            {
                string resultado = VacAcumuladas.ToString("N2");

                xrTableCell83.Text = resultado;
                xrTableCell35.Text = resultado;
            }

            //SI HAY DÍAS PROPORCIONALES, SE RESTAN DE AHÍ
            //if (tipoVacaciones == "1")
            //{
            //    //string resultado = (propRestantes - dias).ToString();
            //    string resultado = (VacAcumuladas - propUsados).ToString("N2");                
                
            //    if (resultado.Length == 1)
            //    {
            //        resultado = resultado + ".00";
            //    }
            //    else if (resultado.Length > 1 && resultado.Length < 4)
            //    {
            //        resultado = resultado + "0";
            //    }
            //    xrTableCell83.Text = resultado;
            //    xrTableCell35.Text = resultado;
            //}
            //else
            //{
            //    string resultado = propRestantes.ToString();
            //    if (resultado.Length == 1)
            //    {
            //        resultado = resultado + ".00";
            //    }
            //    else if (resultado.Length > 1 && resultado.Length < 4)
            //    {
            //        resultado = resultado + "0";
            //    }
            //    xrTableCell83.Text = resultado;
            //    xrTableCell35.Text = resultado;
            //}
        }

        private void xrTableCell99_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            double progAcumulados = Convert.ToDouble(Parameters["diasProgAcumulados"].Value);
            double ProgUsados = Convert.ToDouble(Parameters["progusados"].Value);
            decimal dias = Convert.ToDecimal(GetCurrentColumnValue("dias").ToString());
            string tipoVacaciones = GetCurrentColumnValue("tipo").ToString();

            if (ProgUsados > 0)
            {
                string resultado = (progAcumulados - ProgUsados).ToString("n2");

                xrTableCell99.Text = resultado;
                xrTableCell72.Text = resultado;
            }
            else
            {
                string resultado = progAcumulados.ToString("n2");

                xrTableCell99.Text = resultado;
                xrTableCell72.Text = resultado;
            }

            //SI HAY DÍAS PROGRESIVOS, SE RESTAN DE AHÍ
            //if (tipoVacaciones == "2")
            //{
            //    string resultado = (progAcumulados - ProgUsados).ToString();
            //    if (resultado.Length == 1)
            //    {
            //        resultado = resultado + ".00";
            //    }
            //    else if (resultado.Length >1 && resultado.Length < 4 )
            //    {
            //        resultado = resultado + "0";
            //    }

            //    xrTableCell99.Text = resultado;
            //    xrTableCell72.Text = resultado;
            //}
            //else
            //{
            //    string resultado = progAcumulados.ToString();
            //    if (resultado.Length == 1)
            //    {
            //        resultado = resultado + ".00";
            //    }
            //    else if (resultado.Length > 1 && resultado.Length < 4)
            //    {
            //        resultado = resultado + "0";
            //    }

            //    xrTableCell99.Text = resultado;
            //    xrTableCell72.Text = resultado;
            //}
        }

        private void xrTableCell65_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            double progRestantes = Convert.ToDouble(Parameters["progrestante"].Value);
            double propRestantes = Convert.ToDouble(Parameters["proprestante"].Value);
            double dias = Convert.ToDouble(GetCurrentColumnValue("dias").ToString());
            //Dias usadOS
            double propUsados = Convert.ToDouble(Parameters["propusados"].Value);
            double progUsados = Convert.ToDouble(Parameters["progusados"].Value);
            double AcumProporcional = Convert.ToDouble(Parameters["diasVacacionesAcumuladas"].Value);
            double AcumProgresivo = Convert.ToDouble(Parameters["diasProgAcumulados"].Value);
            string tipoVacaciones = GetCurrentColumnValue("tipo").ToString();
            //if (tipoVacaciones == "1")
            //{
            //    xrTableCell65.Text = ((propRestantes - dias) + progRestantes).ToString();
            //    xrTableCell78.Text = ((propRestantes - dias) + progRestantes).ToString();
            //}
            //else
            //{
            //    xrTableCell65.Text = ((progRestantes - dias) + propRestantes).ToString();
            //    xrTableCell78.Text = ((progRestantes - dias) + propRestantes).ToString();
            //}

            //TOTAL FINAL
            // (DIAS PROPORCINALES DISPONIBLES + DIAS PROGRESIVOS DISPONIBLES) - (DIAS PROP USADOS + DIAS PROGRE USADOS)
            xrTableCell65.Text = Math.Round((AcumProporcional + AcumProgresivo) - (propUsados + progUsados), 2).ToString();
            xrTableCell78.Text = Math.Round((AcumProporcional + AcumProgresivo) - (propUsados + progUsados), 2).ToString();
        }
    }
}
