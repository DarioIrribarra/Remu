using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace ReportesExternos
{
    public partial class rptLiquidacion : DevExpress.XtraReports.UI.XtraReport
    {
        public rptLiquidacion()
        {
            InitializeComponent();
            //this.bindingSource1.DataSource = typeof(Labour.Data);
        }

        private void RptLiquidacion_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
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
            regla.Condition = "[item] == 'HABERES' Or [item] == 'DESCUENTOS LEGALES' Or [item] == 'APORTES' Or [item] == 'HABERES NO IMPONIBLES' Or [item] == 'ASIGNACIONES FAMILIARES' Or [item] == 'DESCUENTOS'";
            regla.Formatting.Font = new Font("Times New Romans", 9, FontStyle.Bold);
            
           
            this.Detail.FormattingRules.Add(regla);
           
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
    }
}
