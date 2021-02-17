using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Windows.Forms;

namespace ReportesExternos
{
    public partial class rptResumenProcesov2 : DevExpress.XtraReports.UI.XtraReport
    {
        public rptResumenProcesov2()
        {
            InitializeComponent();
            
        }      

        private void CeldaTotal_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell celda = sender as XRTableCell;
            string detalle = GetCurrentColumnValue("detalle").ToString();

            if (detalle == "titulo")
            {
                celda.Text = "";
            }           
        }

        private void celdaDetalle_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            
        }      

        private void RptResumenProcesov2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FormattingRule regla = new FormattingRule();
            FormattingRule reglaDatos = new FormattingRule();

            this.FormattingRuleSheet.Add(regla);
            this.FormattingRuleSheet.Add(reglaDatos);

            regla.DataMember = this.DataMember;
            regla.DataSource = this.DataSource;
            reglaDatos.DataMember = this.DataMember;
            reglaDatos.DataSource = this.DataSource;

            regla.Condition = "[detalle] == 'titulo'";
            regla.Formatting.BackColor = Color.Gray;
            reglaDatos.Condition = "[detalle] == 'dato'";

            this.Detail.FormattingRules.Add(regla);
            this.Detail.FormattingRules.Add(reglaDatos);

            //ExpressionBinding exp = new ExpressionBinding();
            //exp.PropertyName = "Bold";
            //exp.Expression = "Iif([detalle] == 'subtotal', true, false)";
            //exp.EventName = "BeforePrint";
            ////celdaDescripcion.ExpressionBindings.Clear();            
            //celdaDescripcion.ExpressionBindings.Add(exp);     

            //reglaDatos.Formatting.Padding = new DevExpress.XtraPrinting.PaddingInfo(25, 0, 0, 0, 100F);
            //reglaDatos.Formatting.Padding = new DevExpress.XtraPrinting.PaddingInfo(25, 0, 0, 0, 100F);

           
        }

        private void PictureLogo_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            Image img = Imagen.GetLogoFromBd();

            if (img != null)
            {
                PictureLogo.Image = img;
                PictureLogo.Sizing = DevExpress.XtraPrinting.ImageSizeMode.StretchImage;
            }
            else
            {
                PictureLogo.Image = null;
            }
        }

        private void xrTableCell1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTableCell celda = sender as XRTableCell;
            string detalle = GetCurrentColumnValue("detalle").ToString();

            if (detalle == "titulo")
            {
                celda.Text = "N°";
            }
            else if (detalle == "subtotal")
            {
                celda.Text = ">";
            }
            else if (detalle == "totalizado")
            {
                celda.Text = ">";
            }
        }
    }
}
