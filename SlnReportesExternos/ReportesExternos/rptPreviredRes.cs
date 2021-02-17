using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace ReportesExternos
{
    public partial class rptPreviredRes : DevExpress.XtraReports.UI.XtraReport
    {
        private double GlobalSum = 0;
        private double GlobalSubTotal = 0;

        public rptPreviredRes()
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

        /// <summary>
        /// Total por entidad
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void xrTableCell20_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell celda = sender as XRTableCell;
            double CotizacionCaja = 0;
            double AsignacionFam = 0;
            double Prestamos = 0;
            double SeguroVida = 0;
            double Leasing = 0;
                      
          
            try
            {
                
                string tipo = Convert.ToString(GetCurrentColumnValue("Tipo"));
                if (tipo.Trim().Equals("6 - CAJA COMPENSACION"))
                {
                    CotizacionCaja = Convert.ToDouble(GetCurrentColumnValue("Cotizacion"));
                    AsignacionFam = Convert.ToDouble(GetCurrentColumnValue("AsignacionFam"));
                    Prestamos = Convert.ToDouble(GetCurrentColumnValue("Prestamo"));
                    SeguroVida = Convert.ToDouble(GetCurrentColumnValue("SeguroVida"));
                    Leasing = Convert.ToDouble(GetCurrentColumnValue("Leasing"));

                    
                    //Subtotal = Convert.ToDouble(GetCurrentColumnValue(""));

                    ////Case 1: Solo restamos a total si (caja - asigfam) es positivo
                    if ((CotizacionCaja - AsignacionFam) < 0)
                    {
                        celda.Text = (Prestamos + Leasing + SeguroVida).ToString("n0");
                    }
                    else
                    {
                        //CASE2: (suma de rebajas por asignaciones + otros)
                       celda.Text = ((CotizacionCaja - AsignacionFam) + Prestamos + SeguroVida + Leasing).ToString("n0");
                    }
                }

            }
            catch (Exception ex)
            {
               //error...
            }
        }

        /// <summary>
        /// Subtotal
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void xrLabel8_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRLabel label = sender as XRLabel;            

            try
            {
                string value = label.Summary.GetResult().ToString();
                if (fnSistema.IsDecimal(value))
                    GlobalSum = GlobalSum + Convert.ToDouble(value);
              
            }
            catch (Exception ex)
            {
                //error..
            }
        }

        /// <summary>
        /// Subtotal  summary
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void xrLabel8_SummaryGetResult(object sender, SummaryGetResultEventArgs e)
        {
            
            double CotizacionCaja = 0;
            double AsignacionFam = 0;
            double Prestamos = 0;
            double SeguroVida = 0;
            double Leasing = 0;
            

            try
            {
              

                string tipo = Convert.ToString(GetCurrentColumnValue("Tipo"));
                if (tipo.Trim().Equals("6 - CAJA COMPENSACION"))
                {
                    CotizacionCaja = Convert.ToDouble(GetCurrentColumnValue("Cotizacion"));
                    AsignacionFam = Convert.ToDouble(GetCurrentColumnValue("AsignacionFam"));
                    Prestamos = Convert.ToDouble(GetCurrentColumnValue("Prestamo"));
                    SeguroVida = Convert.ToDouble(GetCurrentColumnValue("SeguroVida"));
                    Leasing = Convert.ToDouble(GetCurrentColumnValue("Leasing"));

                    //Case 1: Solo hay caja y asignacion familiar
                    if ((CotizacionCaja - AsignacionFam) < 0)
                    {
                        e.Result = (Prestamos + SeguroVida + Leasing).ToString("n0");
                        e.Handled = true;
                    }
                    else
                    {
                        //CASE2: Hay Prestamos y otros
                        
                        e.Result = ((CotizacionCaja - AsignacionFam) + Prestamos + SeguroVida + Leasing).ToString("n0");
                        e.Handled = true;
                    }

                }



            }
            catch (Exception ex)
            {
                //error..
            }
        }


        /// <summary>
        /// Sumatoria global
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void xrLabel4_SummaryGetResult(object sender, SummaryGetResultEventArgs e)
        {
           

            e.Result = GlobalSum;
            e.Handled = true;
        }
    }
}
