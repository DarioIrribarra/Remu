using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using DevExpress.LookAndFeel;
using DevExpress.XtraPrinting;
using System.Collections.Generic;
using DevExpress.XtraReports.UI.PivotGrid;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Globalization;

namespace BancoItau
{
    public partial class rptBancoItau : DevExpress.XtraReports.UI.XtraReport
    {

        /// <summary>
        /// CADENA DE CONEXIÓN SQL
        /// </summary>
        public string StringConnection { get; set; }

        ///// <summary>
        ///// Listado de personas que viene desde formulario principal
        ///// </summary>
        public List<Cuerpo> ListadoRpt { get; set; }

        /// <summary>
        /// DATOS PARA HACER REPORTE
        /// </summary>
        public string dataSql { get; set; }

        public rptBancoItau()
        {
            InitializeComponent();
        }

        public rptBancoItau(string pStringConnection, string ConsultaSql)
        {
            InitializeComponent();
            StringConnection = pStringConnection;
            dataSql = ConsultaSql;
        }

        private void imgLogo_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            Image img = DatosReporte.GetLogoFromBd(StringConnection);

            if (img != null)
            {
                imgLogo.Image = img;
                imgLogo.Sizing = DevExpress.XtraPrinting.ImageSizeMode.StretchImage;
            }
            else
            {
                DatosReporte.SinImagen(imgLogo);
            }
        }

        private void Detail_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            
        }

        private void xrTable1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //SE HACE INVISIBLE LA PRIMERA FILA
            xrTable1.Rows.FirstRow.Visible = false;
            Font f = xrTable1.Font;
            DatosReporte datosReporte = new DatosReporte();
            Connection con = new Connection(StringConnection);
            SqlConnection sqlcon = con.OpenConnection();
            string query = dataSql;
            int contador = 0;
            double suma = 0;
            double sumaTotal = 0;
            var decimales = CultureInfo.CreateSpecificCulture("da-DK");
            //OBTENER LOS BANCOS
            int[] idBanco = new int[datosReporte.CountBanco(StringConnection)];
            idBanco = datosReporte.GetBancos(StringConnection);

            //RECORRO CADA PERSONA DEL LISTADO POR CADA BANCO EN LA BASE DE DATOS
            //SE VERIFICA QUE NO SEA NULL Y SE DA UN VALOR DUMMY
            for (int i = 0; i < idBanco.Length; i++)
            {
                for (int j = 0; j < (ListadoRpt == null ? 5 : ListadoRpt.Count); j++)
                {
                    if ((ListadoRpt == null ? 4 : ListadoRpt[j].NroBanco) == idBanco[i])
                    {
                        //SE CREAN FILA Y CELDAS PARA AÑADIR
                        XRTableRow fila = new XRTableRow();
                        XRTableCell cellRut = new XRTableCell();
                        XRTableCell cellNombre = new XRTableCell();
                        XRTableCell cellBanco = new XRTableCell();
                        XRTableCell cellTipoCuenta = new XRTableCell();
                        XRTableCell cellNCuenta = new XRTableCell();
                        XRTableCell cellMonto = new XRTableCell();
                        //AÑADIMOS PARÁMETROS
                        cellRut.WidthF = 30F;
                        cellTipoCuenta.WidthF = 35F;
                        cellNCuenta.WidthF = 30F;
                        cellBanco.WidthF = 35f;
                        cellMonto.WidthF = 23F;
                        
                        cellRut.TextFitMode = TextFitMode.ShrinkAndGrow;
                        cellNombre.TextFitMode = TextFitMode.ShrinkAndGrow;
                        cellTipoCuenta.TextFitMode = TextFitMode.ShrinkAndGrow;
                        cellNCuenta.TextFitMode = TextFitMode.ShrinkAndGrow;
                        cellBanco.TextFitMode = TextFitMode.ShrinkAndGrow;
                        cellMonto.TextFitMode = TextFitMode.ShrinkAndGrow;
                        

                        cellRut.TextAlignment = TextAlignment.TopLeft;
                        cellNombre.TextAlignment = TextAlignment.TopLeft;
                        cellTipoCuenta.TextAlignment = TextAlignment.TopCenter;
                        cellNCuenta.TextAlignment = TextAlignment.TopLeft;
                        cellBanco.TextAlignment = TextAlignment.TopCenter;
                        cellMonto.TextAlignment = TextAlignment.TopRight;

                        cellRut.Text = datosReporte.fFormatearRut2((ListadoRpt == null ? "111111111" : ListadoRpt[j].Rut.ToString()));
                        cellNombre.Text = (ListadoRpt == null ? "Nombre Completo" : ListadoRpt[j].Nombre.ToString());
                        cellBanco.Text = (ListadoRpt == null ? "Nombre Banco" : ListadoRpt[j].NombreBanco.ToString());
                        cellTipoCuenta.Text = (ListadoRpt == null ? "Tipo Cuenta" : ListadoRpt[j].TipoDeCuenta.ToString());
                        cellNCuenta.Text = (ListadoRpt == null ? "111111111" : ListadoRpt[j].NroCuenta.ToString());
                        cellMonto.Text = Convert.ToDouble((ListadoRpt == null ? 0.0 : ListadoRpt[j].Monto)).ToString("N0", decimales);
                        //AÑADO CELDAS A FILA
                        fila.Cells.Add(cellRut);
                        fila.Cells.Add(cellNombre);
                        fila.Cells.Add(cellBanco);
                        fila.Cells.Add(cellTipoCuenta);
                        fila.Cells.Add(cellNCuenta);
                        fila.Cells.Add(cellMonto);
                        
                        //AÑADO FILA
                        xrTable1.Rows.Add(fila);
                        fila.Borders = BorderSide.Left | BorderSide.Bottom | BorderSide.Right;
                        ////TAMAÑO DE CELDAS
                        xrTableCell7.WidthF = cellRut.WidthF;
                        xrTableCell8.WidthF = cellNombre.WidthF;
                        xrTableCell9.WidthF = cellBanco.WidthF;
                        xrTableCell10.WidthF = cellTipoCuenta.WidthF;
                        xrTableCell11.WidthF = cellNCuenta.WidthF;
                        xrTableCell12.WidthF = cellMonto.WidthF;
                        //CUENTO LAS FILAS QUE SE HACEN
                        contador++;
                        suma = suma + Convert.ToDouble((ListadoRpt == null ? 0.0 : ListadoRpt[j].Monto));
                        sumaTotal = sumaTotal + Convert.ToDouble((ListadoRpt == null ? 0.0 : ListadoRpt[j].Monto));
                    }
                }
                //SI ES EL ULTIMO REGISTRO CREO LA TABLA
                if (contador > 0)
                {
                    XRTableRow filaMontoFinal = new XRTableRow();
                    XRTableCell celdaMontofinal = new XRTableCell();
                    XRTableCell celdaTituloMontoFinal = new XRTableCell();

                    //ESTILOS DE LAS CELDAS
                    celdaTituloMontoFinal.Text = "MONTO FINAL POR BANCO: ";
                    celdaTituloMontoFinal.TextAlignment = TextAlignment.MiddleRight;
                    celdaTituloMontoFinal.Font = new Font(f.FontFamily, f.Size, FontStyle.Bold);

                    //celdaMontofinal.Borders = BorderSide.None;
                    celdaMontofinal.TextAlignment = TextAlignment.MiddleRight;
                    celdaMontofinal.Font = new Font(f.FontFamily, f.Size, FontStyle.Bold);
                    celdaMontofinal.WidthF = 17F;
                    celdaMontofinal.Text = suma.ToString("N0", decimales);

                    //AÑADO CELDAS Y FILAS
                    filaMontoFinal.Borders = BorderSide.Bottom;
                    filaMontoFinal.Cells.Add(celdaTituloMontoFinal);
                    filaMontoFinal.Cells.Add(celdaMontofinal);
                    filaMontoFinal.Padding = new PaddingInfo(2, 2, 2, 10);
                    xrTable1.Rows.Add(filaMontoFinal);
                    //REINICIO CONTADOR
                    contador = 0;
                    suma = 0;
                }
            }
            XRTableRow filaTituloTotalRegistros = new XRTableRow();
            XRTableCell celdaTituloTotalRegistros = new XRTableCell();
            XRTableCell celdaTotalRegistros = new XRTableCell();
            XRTableCell celdaTituloTotalGeneral = new XRTableCell();
            XRTableCell celdaTotalGeneral = new XRTableCell();
            Font font = celdaTituloTotalRegistros.Font;

            //ESTILOS DE LAS CELDAS
            celdaTituloTotalRegistros.Text = "TOTAL DE REGISTROS: ";
            celdaTituloTotalRegistros.Borders = BorderSide.None;
            celdaTituloTotalRegistros.TextAlignment = TextAlignment.MiddleRight;
            celdaTituloTotalRegistros.Font = new Font(font.FontFamily, font.Size, FontStyle.Bold);

            celdaTotalRegistros.Text = (ListadoRpt == null ? "5" : ListadoRpt.Count.ToString());
            celdaTotalRegistros.Borders = BorderSide.None;
            celdaTotalRegistros.TextAlignment = TextAlignment.MiddleRight;
            //celdaTotalRegistros.Font = new Font(font.FontFamily, font.Size, FontStyle.Bold);

            celdaTituloTotalGeneral.Text = "TOTAL GENERAL: ";
            celdaTituloTotalGeneral.Borders = BorderSide.None;
            celdaTituloTotalGeneral.TextAlignment = TextAlignment.MiddleRight;
            celdaTituloTotalGeneral.Font = new Font(font.FontFamily, font.Size, FontStyle.Bold);

            celdaTotalGeneral.Text = sumaTotal.ToString("N0", decimales);
            celdaTotalGeneral.Borders = BorderSide.None;
            celdaTotalGeneral.TextAlignment = TextAlignment.MiddleRight;
            //celdaTotalGeneral.Font = new Font(font.FontFamily, font.Size, FontStyle.Bold);

            //AÑADO CELDAS Y FILAS
            filaTituloTotalRegistros.Borders = BorderSide.All;
            filaTituloTotalRegistros.Cells.Add(celdaTituloTotalRegistros);
            filaTituloTotalRegistros.Cells.Add(celdaTotalRegistros);
            filaTituloTotalRegistros.Cells.Add(celdaTituloTotalGeneral);
            filaTituloTotalRegistros.Cells.Add(celdaTotalGeneral);
            filaTituloTotalRegistros.Padding = new PaddingInfo(2, 2, 2, 2);
            xrTable1.Rows.Add(filaTituloTotalRegistros);
        }
    }
}
