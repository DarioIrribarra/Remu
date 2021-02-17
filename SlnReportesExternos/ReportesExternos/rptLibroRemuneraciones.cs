using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Globalization;
using DevExpress.Utils;
using System.Data.SqlClient;
using System.Data;
using DevExpress.DataAccess.Sql;
using System.Drawing.Printing;

namespace ReportesExternos
{
    public partial class rptLibroRemuneraciones : DevExpress.XtraReports.UI.XtraReport
    {
        //Font f = new Font();
        public rptLibroRemuneraciones()
        {
            InitializeComponent();
        }

        private void PictureEmpleado_BeforePrint(object sender, PrintEventArgs e)
        {
           /* Image img;
            if (Imagen.GetLogoFromBd() != null)
            {
                img = Imagen.GetLogoFromBd();
                PictureEmpleado.Image = img;
                PictureEmpleado.Sizing = DevExpress.XtraPrinting.ImageSizeMode.StretchImage;
            }*/
        }

        private void xrTable2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {

        }

        private void xrTable1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
           
        }

        private void rptLibroRemuneracionesFormaNueva_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //string value = GetCurrentColumnValue("negrita1").ToString();
            //XtraReportBase xtra = new XtraReportBase();
            //xtra
            //DevExpress.DataAccess.Sql.DataApi.IColumn columnaPrueba;
            #region MANEJO DE DATOS 1
            Font f = xrTable1.Font;
            //int contador = 0;
            ////  negrita
            //for (int i = 0; i < RowCount; i++)
            //{
            //    if (GetCurrentColumnValue("negrita" + (i + 1)).ToString() == "True")
            //    {
            //        for (int j = 0; j < xrTable1.Rows.Count; j++)
            //        {
            //            for (int k = 0; k < xrTable1.Rows[i].Cells.Count; k++)
            //            {
            //                if (xrTable1.Rows[i].Cells[k].Name == "r" + (i + 1))
            //                {
            //                    xrTable1.Rows[i].Cells[k].Font = new Font(f.FontFamily, f.Size, FontStyle.Bold);
            //                }

            //            }
            //            //for (int k = 0; k < xrTable1.Rows[i].Cells.Count; k++)
            //            //{
            //            //    if (xrTable1.Rows[i].Cells[k].Name == "r"+(i+1))
            //            //    {
            //            //        xrTable1.Rows[i].Cells[k].Font = new Font(f.FontFamily, f.Size, FontStyle.Bold);
            //            //    }
            //            //}
            //        }
            //    }
            //}

            #region ifsDeSoloNegrita
            if (GetCurrentColumnValue("negrita1").ToString() == "True")
            {
                r1.Font = new Font(f.FontFamily, f.Size, FontStyle.Bold);
                c1.Font = new Font(f.FontFamily, f.Size, FontStyle.Bold);
            }
            if (GetCurrentColumnValue("negrita2").ToString() == "True")
            {
                r2.Font = new Font(f.FontFamily, f.Size, FontStyle.Bold);
                c2.Font = new Font(f.FontFamily, f.Size, FontStyle.Bold);
            }
            if (GetCurrentColumnValue("negrita3").ToString() == "True")
            {
                r3.Font = new Font(f.FontFamily, f.Size, FontStyle.Bold);
                c3.Font = new Font(f.FontFamily, f.Size, FontStyle.Bold);
            }
            if (GetCurrentColumnValue("negrita4").ToString() == "True")
            {
                r4.Font = new Font(f.FontFamily, f.Size, FontStyle.Bold);
                c4.Font = new Font(f.FontFamily, f.Size, FontStyle.Bold);
            }
            if (GetCurrentColumnValue("negrita5").ToString() == "True")
            {
                r5.Font = new Font(f.FontFamily, f.Size, FontStyle.Bold);
                c5.Font = new Font(f.FontFamily, f.Size, FontStyle.Bold);
            }
            if (GetCurrentColumnValue("negrita6").ToString() == "True")
            {
                r6.Font = new Font(f.FontFamily, f.Size, FontStyle.Bold);
                c6.Font = new Font(f.FontFamily, f.Size, FontStyle.Bold);
            }
            if (GetCurrentColumnValue("negrita7").ToString() == "True")
            {
                r7.Font = new Font(f.FontFamily, f.Size, FontStyle.Bold);
                c7.Font = new Font(f.FontFamily, f.Size, FontStyle.Bold);
            }
            if (GetCurrentColumnValue("negrita8").ToString() == "True")
            {
                r8.Font = new Font(f.FontFamily, f.Size, FontStyle.Bold);
                c8.Font = new Font(f.FontFamily, f.Size, FontStyle.Bold);
            }
            if (GetCurrentColumnValue("negrita9").ToString() == "True")
            {
                r9.Font = new Font(f.FontFamily, f.Size, FontStyle.Bold);
                c9.Font = new Font(f.FontFamily, f.Size, FontStyle.Bold);
            }
            if (GetCurrentColumnValue("negrita10").ToString() == "True")
            {
                r10.Font = new Font(f.FontFamily, f.Size, FontStyle.Bold);
                c10.Font = new Font(f.FontFamily, f.Size, FontStyle.Bold);
            }
            if (GetCurrentColumnValue("negrita11").ToString() == "True")
            {
                r11.Font = new Font(f.FontFamily, f.Size, FontStyle.Bold);
                c11.Font = new Font(f.FontFamily, f.Size, FontStyle.Bold);
            }
            if (GetCurrentColumnValue("negrita12").ToString() == "True")
            {
                r12.Font = new Font(f.FontFamily, f.Size, FontStyle.Bold);
                c12.Font = new Font(f.FontFamily, f.Size, FontStyle.Bold);
            }
            if (GetCurrentColumnValue("negrita13").ToString() == "True")
            {
                r13.Font = new Font(f.FontFamily, f.Size, FontStyle.Bold);
                c13.Font = new Font(f.FontFamily, f.Size, FontStyle.Bold);
            }
            if (GetCurrentColumnValue("negrita14").ToString() == "True")
            {
                r14.Font = new Font(f.FontFamily, f.Size, FontStyle.Bold);
                c14.Font = new Font(f.FontFamily, f.Size, FontStyle.Bold);
            }
            if (GetCurrentColumnValue("negrita15").ToString() == "True")
            {
                r15.Font = new Font(f.FontFamily, f.Size, FontStyle.Bold);
                c15.Font = new Font(f.FontFamily, f.Size, FontStyle.Bold);
            }
            if (GetCurrentColumnValue("negrita16").ToString() == "True")
            {
                r16.Font = new Font(f.FontFamily, f.Size, FontStyle.Bold);
                c16.Font = new Font(f.FontFamily, f.Size, FontStyle.Bold);
            }
            if (GetCurrentColumnValue("negrita17").ToString() == "True")
            {
                r17.Font = new Font(f.FontFamily, f.Size, FontStyle.Bold);
                c17.Font = new Font(f.FontFamily, f.Size, FontStyle.Bold);
            }
            if (GetCurrentColumnValue("negrita18").ToString() == "True")
            {
                r18.Font = new Font(f.FontFamily, f.Size, FontStyle.Bold);
                c18.Font = new Font(f.FontFamily, f.Size, FontStyle.Bold);
            }
            if (GetCurrentColumnValue("negrita19").ToString() == "True")
            {
                r19.Font = new Font(f.FontFamily, f.Size, FontStyle.Bold);
                c19.Font = new Font(f.FontFamily, f.Size, FontStyle.Bold);
            }
            if (GetCurrentColumnValue("negrita20").ToString() == "True")
            {
                r20.Font = new Font(f.FontFamily, f.Size, FontStyle.Bold);
                c20.Font = new Font(f.FontFamily, f.Size, FontStyle.Bold);
            }
            #endregion

            #region ifsDeSoloCursiva
            if (GetCurrentColumnValue("cursiva1").ToString() == "True")
            {
                r1.Font = new Font(f.FontFamily, f.Size, FontStyle.Italic);
                c1.Font = new Font(f.FontFamily, f.Size, FontStyle.Italic);
            }
            if (GetCurrentColumnValue("cursiva2").ToString() == "True")
            {
                r2.Font = new Font(f.FontFamily, f.Size, FontStyle.Italic);
                c2.Font = new Font(f.FontFamily, f.Size, FontStyle.Italic);
            }
            if (GetCurrentColumnValue("cursiva3").ToString() == "True")
            {
                r3.Font = new Font(f.FontFamily, f.Size, FontStyle.Italic);
                c3.Font = new Font(f.FontFamily, f.Size, FontStyle.Italic);
            }
            if (GetCurrentColumnValue("cursiva4").ToString() == "True")
            {
                r4.Font = new Font(f.FontFamily, f.Size, FontStyle.Italic);
                c4.Font = new Font(f.FontFamily, f.Size, FontStyle.Italic);
            }
            if (GetCurrentColumnValue("cursiva5").ToString() == "True")
            {
                r5.Font = new Font(f.FontFamily, f.Size, FontStyle.Italic);
                c5.Font = new Font(f.FontFamily, f.Size, FontStyle.Italic);
            }
            if (GetCurrentColumnValue("cursiva6").ToString() == "True")
            {
                r6.Font = new Font(f.FontFamily, f.Size, FontStyle.Italic);
                c6.Font = new Font(f.FontFamily, f.Size, FontStyle.Italic);
            }
            if (GetCurrentColumnValue("cursiva7").ToString() == "True")
            {
                r7.Font = new Font(f.FontFamily, f.Size, FontStyle.Italic);
                c7.Font = new Font(f.FontFamily, f.Size, FontStyle.Italic);
            }
            if (GetCurrentColumnValue("cursiva8").ToString() == "True")
            {
                r8.Font = new Font(f.FontFamily, f.Size, FontStyle.Italic);
                c8.Font = new Font(f.FontFamily, f.Size, FontStyle.Italic);
            }
            if (GetCurrentColumnValue("cursiva9").ToString() == "True")
            {
                r9.Font = new Font(f.FontFamily, f.Size, FontStyle.Italic);
                c9.Font = new Font(f.FontFamily, f.Size, FontStyle.Italic);
            }
            if (GetCurrentColumnValue("cursiva10").ToString() == "True")
            {
                r10.Font = new Font(f.FontFamily, f.Size, FontStyle.Italic);
                c10.Font = new Font(f.FontFamily, f.Size, FontStyle.Italic);
            }
            if (GetCurrentColumnValue("cursiva11").ToString() == "True")
            {
                r11.Font = new Font(f.FontFamily, f.Size, FontStyle.Italic);
                c11.Font = new Font(f.FontFamily, f.Size, FontStyle.Italic);
            }
            if (GetCurrentColumnValue("cursiva12").ToString() == "True")
            {
                r12.Font = new Font(f.FontFamily, f.Size, FontStyle.Italic);
                c12.Font = new Font(f.FontFamily, f.Size, FontStyle.Italic);
            }
            if (GetCurrentColumnValue("cursiva13").ToString() == "True")
            {
                r13.Font = new Font(f.FontFamily, f.Size, FontStyle.Italic);
                c13.Font = new Font(f.FontFamily, f.Size, FontStyle.Italic);
            }
            if (GetCurrentColumnValue("cursiva14").ToString() == "True")
            {
                r14.Font = new Font(f.FontFamily, f.Size, FontStyle.Italic);
                c14.Font = new Font(f.FontFamily, f.Size, FontStyle.Italic);
            }
            if (GetCurrentColumnValue("cursiva15").ToString() == "True")
            {
                r15.Font = new Font(f.FontFamily, f.Size, FontStyle.Italic);
                c15.Font = new Font(f.FontFamily, f.Size, FontStyle.Italic);
            }
            if (GetCurrentColumnValue("cursiva16").ToString() == "True")
            {
                r16.Font = new Font(f.FontFamily, f.Size, FontStyle.Italic);
                c16.Font = new Font(f.FontFamily, f.Size, FontStyle.Italic);
            }
            if (GetCurrentColumnValue("cursiva17").ToString() == "True")
            {
                r17.Font = new Font(f.FontFamily, f.Size, FontStyle.Italic);
                c17.Font = new Font(f.FontFamily, f.Size, FontStyle.Italic);
            }
            if (GetCurrentColumnValue("cursiva18").ToString() == "True")
            {
                r18.Font = new Font(f.FontFamily, f.Size, FontStyle.Italic);
                c18.Font = new Font(f.FontFamily, f.Size, FontStyle.Italic);
            }
            if (GetCurrentColumnValue("cursiva19").ToString() == "True")
            {
                r19.Font = new Font(f.FontFamily, f.Size, FontStyle.Italic);
                c19.Font = new Font(f.FontFamily, f.Size, FontStyle.Italic);
            }
            if (GetCurrentColumnValue("cursiva20").ToString() == "True")
            {
                r20.Font = new Font(f.FontFamily, f.Size, FontStyle.Italic);
                c20.Font = new Font(f.FontFamily, f.Size, FontStyle.Italic);
            }
            #endregion

            #region ifsDeSoloVisible
            if (GetCurrentColumnValue("visible1").ToString() == "False")
            {
                r1.Text = "";
                c1.Text = "prueba";
            }
            if (GetCurrentColumnValue("visible2").ToString() == "False")
            {
                r2.Text = "";
                c2.Text = "";
            }
            if (GetCurrentColumnValue("visible3").ToString() == "False")
            {
                r3.Text = "";
                c3.Text = "";
            }
            if (GetCurrentColumnValue("visible4").ToString() == "False")
            {
                r4.Text = "";
                c4.Text = "";
            }
            if (GetCurrentColumnValue("visible5").ToString() == "False")
            {
                r5.Text = "";
                c5.Text = "";
            }
            if (GetCurrentColumnValue("visible6").ToString() == "False")
            {
                r6.Text = "";
                c6.Text = "";
            }
            if (GetCurrentColumnValue("visible7").ToString() == "False")
            {
                r7.Text = "";
                c7.Text = "";
            }
            if (GetCurrentColumnValue("visible8").ToString() == "False")
            {
                r8.Text = "";
                c8.Text = "";
            }
            if (GetCurrentColumnValue("visible9").ToString() == "False")
            {
                r9.Text = "";
                c9.Text = "";
            }
            if (GetCurrentColumnValue("visible10").ToString() == "False")
            {
                r10.Text = "";
                c10.Text = "";
            }
            if (GetCurrentColumnValue("visible11").ToString() == "False")
            {
                r11.Text = "";
                c11.Text = "";
            }
            if (GetCurrentColumnValue("visible12").ToString() == "False")
            {
                r12.Text = "";
                c12.Text = "";
            }
            if (GetCurrentColumnValue("visible13").ToString() == "False")
            {
                r13.Text = "";
                c13.Text = "";
            }
            if (GetCurrentColumnValue("visible14").ToString() == "False")
            {
                r14.Text = "";
                c14.Text = "";
            }
            if (GetCurrentColumnValue("visible15").ToString() == "False")
            {
                r15.Text = "";
                c15.Text = "";
            }
            if (GetCurrentColumnValue("visible16").ToString() == "False")
            {
                r16.Text = "";
                c16.Text = "";
            }
            if (GetCurrentColumnValue("visible17").ToString() == "False")
            {
                r17.Text = "";
                c17.Text = "";
            }
            if (GetCurrentColumnValue("visible18").ToString() == "False")
            {
                r18.Text = "";
                c18.Text = "";
            }
            if (GetCurrentColumnValue("visible19").ToString() == "False")
            {
                r19.Text = "";
                c19.Text = "";
            }
            if (GetCurrentColumnValue("visible20").ToString() == "False")
            {
                r20.Text = "";
                c20.Text = "";
            }
            #endregion

            //for (int i = 0; i < RowCount; i++)
            //{
            //    string prueba = GetCurrentColumnValue("negrita" + (i + 1)).ToString();
            //    //VER SI APLICA NEGRITA
            //    if (GetCurrentColumnValue("negrita" + (i + 1)).ToString() == "True")
            //    {

            //    }
            //}
            #endregion

        }

        //MÉTODO PARA EL LLENADO DE CELDAS
        private void fontDeCeldas(XRTableCell pCelda)
        {
            if (pCelda.Name.StartsWith("c"))
            {
                string numerocelda = pCelda.Name.Remove(0, 1);
                Font f = xrTable2.Font;
                if (GetCurrentColumnValue("visible" + numerocelda).ToString() == "True")
                {
                    if (GetCurrentColumnValue("negrita" + numerocelda).ToString() == "True")
                    {
                        pCelda.Font = new Font(f.FontFamily, f.Size, FontStyle.Bold);

                        if (GetCurrentColumnValue("cursiva" + numerocelda).ToString() == "True")
                        {
                            pCelda.Font = new Font(f.FontFamily, f.Size, FontStyle.Bold | FontStyle.Italic);
                        }
                        return;
                    }

                    if (GetCurrentColumnValue("cursiva" + numerocelda).ToString() == "True")
                    {
                        pCelda.Font = new Font(f.FontFamily, f.Size, FontStyle.Italic);
                    }
                }
                else
                {
                    pCelda.Text = "";
                }
            }
            if (pCelda.Name.StartsWith("r"))
            {
                string numerocelda = pCelda.Name.Remove(0, 1);
                Font f = xrTable1.Font;
                if (GetCurrentColumnValue("visible" + numerocelda).ToString() == "True")
                {
                    if (GetCurrentColumnValue("negrita" + numerocelda).ToString() == "True")
                    {
                        pCelda.Font = new Font(f.FontFamily, f.Size, FontStyle.Bold);

                        if (GetCurrentColumnValue("cursiva" + numerocelda).ToString() == "True")
                        {
                            pCelda.Font = new Font(f.FontFamily, f.Size, FontStyle.Bold | FontStyle.Italic);
                        }
                        return;
                    }

                    if (GetCurrentColumnValue("cursiva" + numerocelda).ToString() == "True")
                    {
                        pCelda.Font = new Font(f.FontFamily, f.Size, FontStyle.Italic);
                    }
                }
                else
                {
                    pCelda.Text = "";
                }
            }

            
        }

        #region beforePrint de Cabeceras

        private void c1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            fontDeCeldas(c1);
        }

        private void c2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            fontDeCeldas(c2);
        }

        private void c3_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            fontDeCeldas(c3);
        }

        private void c4_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            fontDeCeldas(c4);
        }

        private void c5_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            fontDeCeldas(c5);
        }

        private void c6_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            fontDeCeldas(c6);
        }

        private void c7_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            fontDeCeldas(c7);
        }

        private void c8_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            fontDeCeldas(c8);
        }

        private void c9_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            fontDeCeldas(c9);
        }

        private void c10_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            fontDeCeldas(c10);
        }

        private void c11_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            fontDeCeldas(c11);
        }

        private void c12_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            fontDeCeldas(c12);
        }

        private void c13_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            fontDeCeldas(c13);
        }

        private void c14_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            fontDeCeldas(c14);
        }

        private void c15_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            fontDeCeldas(c15);
        }

        private void c16_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            fontDeCeldas(c16);
        }

        private void c17_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            fontDeCeldas(c17);
        }

        private void c18_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            fontDeCeldas(c18);
        }

        private void c19_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            fontDeCeldas(c19);
        }

        private void c20_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            fontDeCeldas(c20);
        }
        #endregion

        #region beforePrint de Celdas
        //RUT
        private void xrTableCell7_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //GetCurrentColumnValue("rut");
            string rut = fnSistema.fFormatearRut2(GetCurrentColumnValue("rut").ToString());
            if (rut.StartsWith("0"))
            {
                rut = rut.Remove(0, 1);
            }
            xrTableCell7.Text = rut;

        }

        private void r1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            fontDeCeldas(r1);
        }

        private void r2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            fontDeCeldas(r2);
        }

        private void r3_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            fontDeCeldas(r3);
        }

        private void r4_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            fontDeCeldas(r4);
        }

        private void r5_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            fontDeCeldas(r5);
        }

        private void r6_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            fontDeCeldas(r6);
        }

        private void r7_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            fontDeCeldas(r7);
        }

        private void r8_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            fontDeCeldas(r8);
        }

        private void r9_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            fontDeCeldas(r9);
        }

        private void r10_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            fontDeCeldas(r10);
        }

        private void r11_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            fontDeCeldas(r11);
        }

        private void r12_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            fontDeCeldas(r12);
        }

        private void r13_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            fontDeCeldas(r13);
        }

        private void r14_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            fontDeCeldas(r14);
        }

        private void r15_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            fontDeCeldas(r15);
        }

        private void r16_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            fontDeCeldas(r16);
        }

        private void r17_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            fontDeCeldas(r17);
        }

        private void r18_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            fontDeCeldas(r18);
        }

        private void r19_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            fontDeCeldas(r19);
        }

        private void r20_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            fontDeCeldas(r20);
        }
        #endregion

        
    }
}
