using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace Labour
{
    public partial class rptSubDescuentosLiq : DevExpress.XtraReports.UI.XtraReport
    {
        public rptSubDescuentosLiq()
        {
            InitializeComponent();
        }

        private void rptSubDescuentosLiq_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
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

            this.Detail.FormattingRules.Add(regla);
        }

        private void xrLabel4_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {

        }

        private void xrLabel6_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //string value = xrLabel6.Text;
            //if (value == "0")
            //    xrLabel6.Visible = false;
        }

       
    }
}
