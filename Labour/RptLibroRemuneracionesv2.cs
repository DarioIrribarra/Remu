using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Windows.Forms;

namespace Labour
{
    public partial class RptLibroRemuneracionesv2 : DevExpress.XtraReports.UI.XtraReport
    {
        public RptLibroRemuneracionesv2()
        {
            InitializeComponent();
        }

        private void xrLabel1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
       
        }

        private void RptLibroRemuneracionesv2_AfterPrint(object sender, EventArgs e)
        {
           
        }

        private void xrPivotGrid1_CustomGroupInterval(object sender, DevExpress.XtraReports.UI.PivotGrid.PivotCustomGroupIntervalEventArgs e)
        {
            
        }

        private void RptLibroRemuneracionesv2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            GroupField group = new GroupField("CategoryName");
            GroupHeader1.GroupFields.Add(group);

            XRLabel labelGroup = new XRLabel { ForeColor = System.Drawing.Color.Blue };
            labelGroup.DataBindings.Add("Text", null, "contrato");

            GroupHeader1.Controls.Add(labelGroup);            
            
        }


        //CREAR COLUMNAS EN BASE A SQL
    }
}
