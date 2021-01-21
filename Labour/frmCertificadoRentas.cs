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
using System.Threading;
using DevExpress.XtraReports.UI;
using System.IO;

namespace Labour
{
    public partial class frmCertificadoRentas : DevExpress.XtraEditors.XtraForm, IConjuntosCondicionales
    {
        #region "INTERFAZ"
        public void CargarCodigoConjunto(string code)
        {
            //txtConjunto.Text = code;
            txtCondicion.Text = code;
        }
        #endregion

        private string SqlBase = "SELECT distinct rut from trabajador ";
        private bool Privadas = User.ShowPrivadas();
        private string FiltroUsuario = User.GetUserFilter();
        private List<LiquidacionHistorico> ListadoRuts = new List<LiquidacionHistorico>();

        BarraProgreso Barra;
        delegate void ShowEmploye(string pNombre);
        private string PeriodoConsultado = "";

        public frmCertificadoRentas()
        {
            InitializeComponent();
        }

        private void frmCertificadoRentas_Load(object sender, EventArgs e)
        {
            CargaCombo(txtPeriodo);
          
            Barra = new BarraProgreso(BarraCalculo, 1, true, true, this);
        }

        private void cbTodos_CheckedChanged(object sender, EventArgs e)
        {
            if (cbTodos.Checked)
            {
                txtCondicion.Text = "";
                txtCondicion.Enabled = false;
                btnCondicion.Enabled = false;
            }
            else
            {
                txtCondicion.Text = "";
                txtCondicion.Enabled = true;
                txtCondicion.Focus();
                btnCondicion.Enabled = true;
            }
        }

        private void btnCondicion_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();
            if (cbTodos.Checked == false)
            {
                FrmFiltroBusqueda filtro = new FrmFiltroBusqueda(true);
                filtro.StartPosition = FormStartPosition.CenterParent;
                filtro.opener = this;
                filtro.ShowDialog();
            }
        }

        #region "DATOS"
        /// <summary>
        /// Retorna todos los años disponibles para consultar.
        /// </summary>
        /// <returns></returns>
        private List<formula> GetListYears()
        {
            string sql = "SELECT DISTINCT SUBSTRING(CAST(anomes AS VARCHAR(6)), 1, 4) as y from parametro ORDER BY y";
            SqlCommand cmd;
            SqlConnection cn;
            SqlDataReader rd;

            List<formula> listado = new List<formula>();

            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            rd = cmd.ExecuteReader();
                            if (rd.HasRows)
                            {
                                while (rd.Read())
                                {
                                    listado.Add(new formula() { key = (string)rd["y"], desc = (string)rd["y"] });
                                }
                            }

                            cmd.Dispose();
                            rd.Close();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                //ERROR
            }

            return listado;
        }

        /// <summary>
        /// Carga combobox con años
        /// </summary>
        /// <param name="pCombo"></param>
        private void CargaCombo(LookUpEdit pCombo)
        {
            List<formula> Listado = new List<formula>();
            Listado = GetListYears();

            if (Listado.Count > 0)
            {
                pCombo.Properties.DataSource = Listado.ToList();
                pCombo.Properties.PopulateColumns();

                pCombo.Properties.ValueMember = "key";
                pCombo.Properties.DisplayMember = "desc";
                pCombo.Properties.Columns[0].Visible = false;

                pCombo.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
                pCombo.Properties.AutoSearchColumnIndex = 1;
                pCombo.Properties.ShowHeader = false;

                if (pCombo.Properties.DataSource != null)
                    pCombo.ItemIndex = 0;
            }
        }

        /// <summary>
        /// Entrega un listado de rut dentro de un año especifico
        /// </summary>
        /// <param name="pConjunto"></param>
        /// <param name="pPeriodo"></param>
        /// <returns></returns>
        private List<LiquidacionHistorico> GetListado(string pConjunto, int pPeriodo)
        {
            List<LiquidacionHistorico> Listado = new List<LiquidacionHistorico>();
            string filter = "", sql = "";
            filter = Calculo.GetSqlFiltro(FiltroUsuario, pConjunto, Privadas);

            string Primer = "", ultimo = "";
            Primer = pPeriodo.ToString() + "01";
            ultimo = pPeriodo.ToString() + "12";

            //sql = SqlBase + $" WHERE (anomes>={Primer} AND anomes <={ultimo}) {filter} ORDER BY rut, anomes";
            sql = SqlBase + $" WHERE (anomes>={Primer} AND anomes <={ultimo}) {filter} ORDER BY rut";
            SqlCommand cmd;
            SqlConnection cn;
            SqlDataReader rd;

            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            rd = cmd.ExecuteReader();
                            if (rd.HasRows)
                            {
                                while (rd.Read())
                                {
                                    //LLENAMOS LISTADO
                                    Listado.Add(new LiquidacionHistorico() { Rut = (string)rd["rut"]});
                                }
                            }

                            cmd.Dispose();
                            rd.Close();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                //ERROR.
            }


            return Listado;
        }

        /// <summary>
        /// Genera informacion de renta para cada rut en el periodo específico.
        /// </summary>
        private DataTable GeneraData(string pRut, string pPeriodo)
        {
            string sql = "SELECT dbo.fnGetMesEstaticos(trabajador.anomes) as anomes, " +
                         "(SELECT SUM(valorcalculado) FROM itemtrabajador WHERE tipo = 1 AND suspendido = 0 AND itemtrabajador.anomes = trabajador.anomes AND rut = @rut) as 'SueldoBruto', " +
                         "(SELECT SUM(valorcalculado) FROM itemtrabajador WHERE(coditem = 'SCEMPLE' OR coditem = 'SALUD' OR coditem = 'PREVISI') " +
                         "AND suspendido = 0 AND itemtrabajador.anomes = trabajador.anomes AND itemtrabajador.rut = @rut) as 'PrevisionTrabajador', " +
                         "(SELECT SUM(systributo) FROM calculomensual WHERE calculoMensual.anomes = trabajador.anomes AND calculomensual.contrato = trabajador.contrato) as 'RentaImpAfectaImpto', " +
                         "(SELECT SUM(valorcalculado) FROM itemtrabajador WHERE coditem = 'IMPUEST' and suspendido = 0 AND rut = @rut AND itemtrabajador.anomes = trabajador.anomes) as 'ImpuestoUnico', " +
                         "0 as MayorRenta, 0 as RentaNoGravada, 0 as RentaTotalExenta, 0 as RebajasZonaExtrema,  " +
                         "(SELECT factorimp FROM valoresmes WHERE anomes = trabajador.anomes) as 'FactorActualizacion', " +
                         "dbo.fnTributableAjustable(rut, trabajador.anomes) as 'RentaAfectaImptoReajustada', " +
                         "dbo.fnValorIMpuesto(dbo.fntributableAjustable(rut, trabajador.anomes), trabajador.anomes) as 'ImpoReajustado', " +
                         "0 as'MayorRetdelimpto', 0 as 'RentaTotalNoGravada', 0 as 'RentaTotalExenta', 0 as 'RebajasExtrema'," +
                         "CONCAT(nombre, ' ', apepaterno, ' ', apematerno) as Trabajador, " +
                         "dbo.fnformatearut(trabajador.rut) as rut " +
                         $"FROM trabajador WHERE rut = @rut AND status = 1 AND trabajador.anomes LIKE '%{pPeriodo}%'";

            SqlCommand cmd;
            SqlConnection cn;
            DataSet ds = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            DataTable tabla = new DataTable();

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
                            cmd.Parameters.Add(new SqlParameter("@rut", pRut));
                            ad.SelectCommand = cmd;
                            ad.Fill(ds, "data").ToString();

                            if (ds.Tables[0].Rows.Count > 0)
                                tabla = ds.Tables[0];

                            ad.Dispose();
                            ds.Dispose();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                //ERROR ...
            }

            return tabla;
        }


        private void Calcular(bool pdf)
        {
            DataTable data = new DataTable();
            Empresa emp = new Empresa();
            emp.SetInfo();

            if (ListadoRuts.Count > 0)
            {
                Barra.Maximum = ListadoRuts.Count;
                Barra.ShowControl = true;
                Barra.Begin();

                XtraReport reporteAux = new XtraReport();
                reporteAux.CreateDocument();

                foreach (LiquidacionHistorico x in ListadoRuts)
                {
                    data = GeneraData(x.Rut, txtPeriodo.EditValue.ToString());
                    if (data.Rows.Count > 0)
                    {
                        //Obtener nombre a través del rut
                        
                        RptCertificadoRentas crt = new RptCertificadoRentas();
                        crt.DataSource = data;
                        crt.DataMember = "data";

                        SetParameters(crt, emp);                                          

                        crt.CreateDocument();

                        //MostrarTrabajador(per.ApellidoNombre);
                        Barra.Increase();

                        reporteAux.Pages.AddRange(crt.Pages);
                    }
                    else
                    {
                        XtraMessageBox.Show("No se encontró información", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        break;
                    }
                }

                //Guardar archivo
                Documento doc = new Documento("", 0);
                if (reporteAux.Pages.Count > 0) {
                    if (pdf)
                        doc.ExportToPdf(reporteAux, $"Certificado_Rentas");
                    else
                        doc.ShowDocument(reporteAux);
                }
                    
            }
        }


        private void SetParameters(XtraReport pReport, Empresa emp)
        {
            pReport.Parameters["direccion"].Value = emp.Direccion;
            pReport.Parameters["Empresa"].Value = emp.Razon;
            pReport.Parameters["Representante"].Value = emp.NombreRep;
            pReport.Parameters["RutEmpresa"].Value = fnSistema.fFormatearRut2(emp.Rut);
            pReport.Parameters["RutRepresentante"].Value = "Rut: " + fnSistema.fFormatearRut2(emp.RutRep);
            pReport.Parameters["periodo"].Value = PeriodoConsultado;
            pReport.Parameters["ciudad"].Value = emp.NombreCiudad;

            pReport.Parameters["direccion"].Visible = false;
            pReport.Parameters["Empresa"].Visible = false;
            pReport.Parameters["Representante"].Visible = false;
            pReport.Parameters["RutEmpresa"].Visible = false;
            pReport.Parameters["RutRepresentante"].Visible = Visible;
            pReport.Parameters["periodo"].Visible = false;
        }

        //MOSTRAR NOMBRE DEL TRABAJADOR QUE SE ESTÁ PROCESANDO
        private void MostrarTrabajador(string pText)
        {
            if (this.InvokeRequired)
            {
                ShowEmploye emp = new ShowEmploye(MostrarTrabajador);

                //PARAMETROS
                object[] parameters = new object[] { pText };

                this.Invoke(emp, parameters);
            }
            else
            {
                lblNombre.Visible = true;
                lblNombre.Text = pText;
            }
        }
        #endregion

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            Close();
        }     

        private void btnSalida_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            FolderBrowserDialog di = new FolderBrowserDialog();
            //di.Filter = "Word 2007 Documents (*.docx)|*.docx|Word 97-2003 Documents (*.doc)|*.doc";
            //di.Title = "Guardar en...";            

            if (di.ShowDialog() == DialogResult.OK)
            {
                txtSalida.Text = di.SelectedPath;
            }
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            Guardar(false);
        }

        private void guardarPdf_Click(object sender, EventArgs e)
        {
            Guardar(true);
        }

        /// <summary>
        /// Se realiza proceso de guardado y se entrega valor para exportar a pdf o excel
        /// </summary>
        /// <param name="pdf"></param>
        private void Guardar(bool pdf) {
            Sesion.NuevaActividad();

            if (txtPeriodo.Properties.DataSource == null || txtPeriodo.EditValue == null)
            { XtraMessageBox.Show("Por favor ingresa un periodo válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtPeriodo.Focus(); return; }

            if (Directory.Exists(txtSalida.Text) == false)
            { XtraMessageBox.Show("Por favor verifica el directorio de salid", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (cbTodos.Checked)
            {
                ListadoRuts = GetListado("", Convert.ToInt32(txtPeriodo.EditValue));
            }
            else
            {
                if (Conjunto.ExisteConjunto(txtCondicion.Text) == false)
                { XtraMessageBox.Show("Por favor ingresa una condición válida", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtCondicion.Focus(); return; }

                ListadoRuts = GetListado(txtCondicion.Text, Convert.ToInt32(txtPeriodo.EditValue));
            }

            if (ListadoRuts.Count == 0)
            { XtraMessageBox.Show("No se encontró información para el periodo seleccionado", "Información", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            PeriodoConsultado = txtPeriodo.EditValue.ToString();
            ThreadStart del = new ThreadStart(() => Calcular(pdf));
            Thread hilo = new Thread(del);
            hilo.Name = "Hilo";
            //PARA SOLUCIONAR ERROR AL EXPORTAR DESDE EL REPORTE
            hilo.SetApartmentState(ApartmentState.STA);
            hilo.Start();
        }

        
    }
}