using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Data.SqlClient;
using System.Collections;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Configuration;
using System.IO;

namespace Labour
{
    public partial class FrmVacacionesEst : DevExpress.XtraEditors.XtraForm, IConjuntosCondicionales
    {
        #region "INTERFAZ"
        public void CargarCodigoConjunto(string code)
        {
            txtConjunto.Text = code;
        }
        #endregion

        public FrmVacacionesEst()
        {
            InitializeComponent();
        }

        private void FrmVacacionesEst_Load(object sender, EventArgs e)
        {
            dtFecha.Properties.MinValue = DateTime.Now.Date;
            dtFecha.DateTime = DateTime.Now.Date;
            cbtodos.Checked = true;
            datoCombobox.AgrupaList(txtAgrupa);
        }

        private void cbtodos_CheckedChanged(object sender, EventArgs e)
        {
            CheckEdit cb = sender as CheckEdit;
            if (cb.Checked)
            {
                btnConjunto.Enabled = false;
                txtConjunto.Text = "";
                txtConjunto.Enabled = false;
            }
            else
            {
                btnConjunto.Enabled = true;
                txtConjunto.Text = "";
                txtConjunto.Enabled = true;
                txtConjunto.Focus();
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnConjunto_Click(object sender, EventArgs e)
        {
            if (txtConjunto.Enabled)
            {
                FrmFiltroBusqueda busqueda = new FrmFiltroBusqueda(true);
                busqueda.StartPosition = FormStartPosition.CenterParent;
                busqueda.opener = this;
                busqueda.ShowDialog();
            }
        }

        private void btnProcesar_Click(object sender, EventArgs e)
        {
            if (dtFecha.EditValue == null)
            { XtraMessageBox.Show("Por favor selecciona una fecha válida", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            Cursor = Cursors.WaitCursor;
            if (cbtodos.Checked)
            {
                //Consultamos para todos los registros del periodo
                Busqueda(dtFecha.DateTime, "");
            }
            else
            {
                //Buscamos en base a condicion
                if (txtConjunto.Text == "" || Conjunto.ExisteConjunto(txtConjunto.Text) == false)
                { XtraMessageBox.Show("Por favor selecciona un conjunto válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtConjunto.Focus(); return; }

                Busqueda(dtFecha.DateTime, txtConjunto.Text);

            }
        }

        private void Busqueda(DateTime pFechaTope, string pCond, bool editar = false)
        {
            string cond = "";
            cond = Calculo.GetSqlFiltro(User.GetUserFilter(), pCond, User.ShowPrivadas());

            List<string> Contratos = new List<string>();
            Contratos = Listado(cond);
            Persona Trabajador = new Persona();

            double DiasProporcionales = 0, DiasProgresivos = 0, DiasRestantes = 0, DiasAcum = 0;
            int diasNoHab = 0;
            Hashtable dUsados = new Hashtable();
            DateTime yearProgresivo = DateTime.Now.Date;
            bool ConsideraProgresivo = false;
            DateTime FechaBase = DateTime.Now.Date;
            double CotFaltante = 0, CotReq = 120;
            List<VacEst> DataSource = new List<VacEst>();
            Hashtable Usados = new Hashtable();
            string field = "";

            if (Contratos.Count == 0)
            { XtraMessageBox.Show("No se encontró información", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            try
            {
                if (Contratos != null)
                {
                    if (Contratos.Count > 0)
                    {
                        //Recorremos y buscamos info
                        foreach (string con in Contratos)
                        {
                            Trabajador = Persona.GetInfo(con, Calculo.PeriodoObservado);
                            if (Trabajador != null)
                            {
                                //Buscamos info de vacaciones hasta la fecha de tope
                                DiasProporcionales = vacaciones.FeriadosProp(Trabajador.Ingreso, pFechaTope);

                                //@ CASO1: NO TIENE COTIZACIONES Y TAMPOCO TIENE FECHA DE PROGRESIVO
                                if (Trabajador.AnosProgresivos == 0 || Trabajador.FechaProgresivo > Trabajador.Salida)
                                {
                                    //NO CONSIDERAMOS DIAS DE COTIZACIONES SI LA FECHA ES 01-01-3000
                                    ConsideraProgresivo = false;
                                    FechaBase = Trabajador.Ingreso.AddYears(10);
                                    //SI FECHA BASE ES MAYOR A LA FECHA EN QUE CUMPLE AÑO (LE CORRESPONDE ANUALIDAD VACACION LO CORREMOS OTRO AÑO)
                                }
                                //@CASO2: TIENE FECHA DE PROGRESIVO, CONSIDERAMOS MESES DE COTIZACIONES
                                else
                                {
                                    ConsideraProgresivo = true;
                                    //DEBEMOS OBTERNER LA FECHA EN LA CUAL SE CUMPLEN LOS 10 AÑOS (SI TIENE MENOS DE 120 COTIZACIONES)
                                    if (Trabajador.AnosProgresivos < 120)
                                    {
                                        //OBTENEMOS LA CANTIDAD DE MESES FALTANTES PARA OBTENER LOS 10 AÑOS DE BASE
                                        CotFaltante = CotReq - Trabajador.AnosProgresivos;

                                        //SI HAY MESES FALTANTES PARA LOS 10 AÑOS DE BASE OBTENEMOS LA FECHA FINAL (FECHA EN QUE CUMPLIRÍA LOS 10 AÑOS)
                                        if (CotFaltante > 0)
                                            FechaBase = Trabajador.FechaVacacion.AddMonths(Convert.ToInt32(CotFaltante));
                                        //SI NO CONSIDERAMOS LA FECHA INICIAL FECHA VACACIONES
                                        else
                                            FechaBase = Trabajador.FechaVacacion;
                                    }
                                    else
                                    {
                                        //CUMPLE CON LAS 120 COTIZACIONES
                                        FechaBase = Trabajador.FechaVacacion;
                                    }
                                }

                                //OBTENEMOS LOS DIAS PROGRESIVOS COMO PUNTO DE PARTIDA LA FECHA BASE OBTENIDA...
                                DiasProgresivos = vacaciones.ProgresivosEstimados(FechaBase, Trabajador.FechaProgresivo, pFechaTope, ConsideraProgresivo);

                                Usados = vacaciones.diasUsados(con);

                                DataSource.Add(new VacEst() {
                                    Area = Trabajador.NombreArea, 
                                    Cargo = Trabajador.Cargo, 
                                    CentroCosto = Trabajador.centro, 
                                    Sucursal = Trabajador.sucursal, 
                                    Nombre = Trabajador.ApellidoNombre, 
                                    Rut = fnSistema.fFormatearRut2(Trabajador.Rut), 
                                    FechaTope = pFechaTope, 
                                    DiasProgresivos = DiasProgresivos, 
                                    DiasPropporcionales = DiasProporcionales, 
                                    ProgUsados = Convert.ToDouble(Usados["progresivo"]), 
                                    PropUsados = Convert.ToDouble(Usados["proporcional"]),
                                    Cotiz = Convert.ToDouble(Trabajador.AnosProgresivos), 
                                    FecVac = Trabajador.FechaVacacion
                                });

                                DataSource = DataSource.OrderBy(x => x.Nombre).ToList();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //No se encontró información
            }


            //Pasamos cono dartasource a reporte
            //RptVacEst va = new RptVacEst();
            ReportesExternos.rptVacEst va = new ReportesExternos.rptVacEst();
            va.LoadLayoutFromXml(Path.Combine(fnSistema.RutaCarpetaReportesExterno, "rptVacEst.repx"));

            va.DataSource = DataSource;

            va.Parameters["condicion"].Visible = false;
            va.Parameters["condicion"].Value = Conjunto.GetCondicionReporte(pCond, User.GetUserFilter());
            va.Parameters["agrupacion"].Visible = false;
           

            if (txtAgrupa.Properties.DataSource != null)
            {
                //rpt.Parameters["agrupacion"].Visible = false;
                //rpt.Parameters["agrupacion"].Value = txtAgrupa.Text;

                if (txtAgrupa.EditValue.ToString() != "0")
                {
                    va.groupFooterBand1.Visible = true;

                    va.Parameters["agrupacion"].Value = txtAgrupa.Text;

                    va.groupHeaderBand2.GroupFields.Clear();
                    GroupField groupField = new GroupField();
                    field = txtAgrupa.Text.ToLower();
                    groupField.FieldName = field;
                    va.groupHeaderBand2.GroupFields.Add(groupField);

                    XRLabel labelGroup = new XRLabel { ForeColor = System.Drawing.Color.Black, WidthF = 998, TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft };
                    labelGroup.Font = new Font("Arial", 9, FontStyle.Italic | FontStyle.Bold);
                    labelGroup.Borders = DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Bottom | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Top;
                    labelGroup.BorderDashStyle = DevExpress.XtraPrinting.BorderDashStyle.Solid;

                    if (Settings.Default.UserDesignerOptions.DataBindingMode == DataBindingMode.Bindings)
                    {
                        labelGroup.DataBindings.Add("Text", null, $"{field}");
                    }
                    else
                    {
                        labelGroup.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", $"[{field}]"));
                    }
                    va.groupHeaderBand2.Controls.Add(labelGroup);


                }
                else
                {
                    va.groupFooterBand1.Visible = false;
                    
                  
                }
                   
            }

            Cursor = Cursors.Default;
            Documento doc = new Documento("", 0);
            //va.SaveLayoutToXml(Path.Combine(fnSistema.RutaCarpetaReportesExterno, "rptVacEst.repx"));
            if (editar)
            {
                splashScreenManager1.ShowWaitForm();
                //Se le pasa el waitform para que se cierre una vez cargado
                DiseñadorReportes.MostrarEditorLimitado(va, "rptVacEst.repx", splashScreenManager1);
            }
            else 
            {
                doc.ShowDocument(va);
            }
            


        }

        /// <summary>
        /// Entrega un listado de contrato en base a condiciones implementadas por el usuario...
        /// </summary>
        /// <param name="pCondicion"></param>
        /// <returns></returns>
        private List<string> Listado(string pCondicion)
        {
            List<string> List = new List<string>();
            string sql = "SELECT contrato FROM trabajador WHERE status=1 AND anomes=@pPeriodo {condition} ";

            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader rd;

            if (pCondicion.Length > 0)
                sql = sql.Replace("{condition}", pCondicion);
            else
                sql = sql.Replace("{condition}", "");

            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@pPeriodo", Calculo.PeriodoObservado));

                            rd = cmd.ExecuteReader();
                            if (rd.HasRows)
                            {
                                while (rd.Read())
                                {
                                    //Llenamos listado
                                    List.Add(Convert.ToString(rd["contrato"]));
                                }
                            }
                            rd.Close();
                            cmd.Dispose();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                //Error...
                List = null;
            }


            return List;
        }

        private void btnEditarReporte_Click(object sender, EventArgs e)
        {
            if (dtFecha.EditValue == null)
            { XtraMessageBox.Show("Por favor selecciona una fecha válida", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            Cursor = Cursors.WaitCursor;
            if (cbtodos.Checked)
            {
                //Consultamos para todos los registros del periodo
                Busqueda(dtFecha.DateTime, "", editar: true);
            }
            else
            {
                //Buscamos en base a condicion
                if (txtConjunto.Text == "" || Conjunto.ExisteConjunto(txtConjunto.Text) == false)
                { XtraMessageBox.Show("Por favor selecciona un conjunto válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtConjunto.Focus(); return; }

                Busqueda(dtFecha.DateTime, txtConjunto.Text, editar:true);

            }
        }
    }
}