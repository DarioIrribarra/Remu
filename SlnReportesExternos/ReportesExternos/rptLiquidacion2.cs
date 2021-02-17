using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace ReportesExternos
{
    public partial class rptLiquidacion2 : DevExpress.XtraReports.UI.XtraReport
    {
        public rptLiquidacion2()
        {
            InitializeComponent();
        }

        private void rptLiquidacion2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //CREANDO UNA REGLA DE FORMATO PARA EL REPORTE
            //INSTANCIAMOS A LA CLASE FORMATINNGRULE
            FormattingRule regla = new FormattingRule();
            FormattingRule regla2 = new FormattingRule();
            //AGREGAMOS LA REGLA
            this.FormattingRuleSheet.Add(regla);

            regla.DataMember = this.DataMember;
            regla.DataSource = this.DataSource;
            //Iif(EXPRESION, verdadero  ,  falso)
            regla.Condition = "[item] == 'IMPONIBLES' Or [item] == 'LEGALES' Or [item] == 'OTROS' Or [item] == 'NO IMPONIBLES' Or [item] == 'ASIGNACIONES FAMILIARES' Or [item] == 'DESCUENTOS'";
            regla.Formatting.Font = new Font("Times New Romans", 9, FontStyle.Bold);

            //regla2.DataMember = this.DataMember;
            //regla2.DataSource = this.DataSource;
            //regla2.Condition = "[item] <> 'IMPONIBLES' Or [item] <> 'LEGALES' Or [item] <> 'OTROS' Or [item] <> 'NO IMPONIBLES' Or [item] <> 'ASIGNACIONES FAMILIARES' Or [item] <> 'DESCUENTOS'";
            //regla2.Formatting.

            this.Detail.FormattingRules.Add(regla);

        }

        private void xrSubreport1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            
        }

        private void xrLabel35_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            string value = xrLabel35.Text;
            if (value == "0")
                xrLabel35.Visible = false;
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
                imgLogo1.Image = null;
            }
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
    }
}
