using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraReports.UI;

namespace Labour
{
    public partial class frmAntiguedad : XtraForm
    {
        Persona Trabajador;
        //PARA GUARDAR NUMERO DE CONTRATO
        string NumeroContrato = "";
        //PARA GUARDAR PERIODO
        int Periodo = 0;
        public frmAntiguedad()
        {
            InitializeComponent();
        }

        //CARGAR DATOS EN EL FORMULARIO
        public frmAntiguedad(string Contrato, int periodo)
        {
            InitializeComponent();
            NumeroContrato = Contrato;
            this.Periodo = periodo;
        }

        private void frmAntiguedad_Load(object sender, EventArgs e)
        {
            Trabajador = new Persona();
            if (NumeroContrato != "" && Periodo != 0)
            {
                CargaDatos(NumeroContrato, Periodo);
            }
        }

        #region "MANEJO DE DATOS"
        //CARGA DATOS EN FORMULARIO
        private void CargaDatos(string Contrato, int periodo)
        {
            //LLAMAR A LA CLASE PERSONA PARA LOS DATOS DEL TRABAJADOR
            Trabajador = Persona.GetInfo(Contrato, periodo);

            //LLAMAR A LA CLASE EMPRESA PARA LOS DATOS DE LA EMPRESA
            Empresa emp = new Empresa();
            emp.SetInfo();
            lblNombreTrabajador.Text = Trabajador.NombreCompleto;
            lblRut.Text = fnSistema.fFormatearRut2(Trabajador.Rut);
            lblNombreEmpresa.Text = emp.Razon;
            lblFechaContrato.Text = Trabajador.Ingreso.ToLongDateString().ToUpper();
            lblCargo.Text = Trabajador.Cargo;

            if (Trabajador.Tipocontrato == 0)
                lblTipoContrato.Text = "INDEFINIDO";
            else if (Trabajador.Tipocontrato == 1)
            {
                lblTipoContrato.Text = "PLAZO FIJO";
            }
            else
            {
                lblTipoContrato.Text = "OBRA O FAENA";
            }
        }

        //FICHA EMPLEADO ARCHIVO PDF
        private void AntiguedadEmpleado(string contrato, int periodo, bool? ImpresionRapida = false, bool? GeneraPdf = false)
        {
            string tipoContrato = "";
            //LLAMAR A LA CLASE EMPRESA PARA LOS DATOS DE LA EMPRESA
            Empresa emp = new Empresa();
            emp.SetInfo();

            DataSet ds = new DataSet();
            //string dia = Trabajador.Ingreso.ToString("dd");

            ds = Persona.GetInfoDataset(contrato, periodo);
            if (ds.Tables[0].Rows.Count > 0)
            {
                //PASAMOS COMO DATASOURCE EL DATASET A REPORTE
                //rptAntiguedad antiguedad = new rptAntiguedad();
                //Reporte externo
                //ReportesExternos.rptAntiguedad antiguedad = new ReportesExternos.rptAntiguedad();
                XtraReport antiguedad = new XtraReport();
                antiguedad.LoadLayout(Path.Combine(fnSistema.RutaCarpetaReportesExterno, "rptAntiguedad.repx"));

                antiguedad.DataSource = ds.Tables[0];
                antiguedad.DataMember = "antiguedad";

                //NO MOSTRAR LOS PARAMETROS
                
                foreach (DevExpress.XtraReports.Parameters.Parameter parametro in antiguedad.Parameters)
                {
                    parametro.Visible = false;
                }
                

                
                antiguedad.Parameters["rutTrabajador"].Value = fnSistema.fFormatearRut2(Trabajador.Rut);
                antiguedad.Parameters["antiguedad"].Value = Trabajador.Ingreso.ToString("dd 'de' MMMM 'de' yyyy");
                antiguedad.Parameters["cargo"].Value = Trabajador.Cargo;
                antiguedad.Parameters["imagen"].Value = Imagen.GetLogoFromBd();
                antiguedad.Parameters["nombreTrabajador2"].Value = Trabajador.NombreCompleto;

                if (Trabajador.Tipocontrato == 0)
                    tipoContrato = "INDEFINIDO";
                else if (Trabajador.Tipocontrato == 1)
                {
                    tipoContrato = "PLAZO FIJO";
                }
                else
                {
                    tipoContrato = "OBRA O FAENA";
                }
                antiguedad.Parameters["tipoContratoTexto"].Value = tipoContrato;
                //DATOS EMPRESA
                antiguedad.Parameters["empresa"].Value = emp.Razon;
                antiguedad.Parameters["rutEmpresa"].Value = fnSistema.fFormatearRut2(emp.Rut);
                antiguedad.Parameters["direccion"].Value = emp.Direccion;

                SqlCommand cmd;
                string sql = "SELECT descCiudad FROM ciudad WHERE idCiudad = @ciudad";
                string resultado = "";
                try
                {
                    if (fnSistema.ConectarSQLServer())
                    {
                        
                        using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                        {
                            cmd.Parameters.Add(new SqlParameter("@ciudad", emp.Ciudad));
                            resultado = cmd.ExecuteScalar().ToString();
                        }
                        cmd.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    //ERROR
                    XtraMessageBox.Show($"Error al cargar Ciudad {ex.ToString()}","Error");
                    throw;
                }
                antiguedad.Parameters["ciudad"].Value = resultado.ToUpper();

                Documento d = new Documento("", 0);

                //antiguedad.SaveLayoutToXml(Path.Combine(fnSistema.RutaCarpetaReportesExterno, "rptAntiguedad.repx"));

                if ((bool)ImpresionRapida)
                    d.PrintDocument(antiguedad);
                else if ((bool)GeneraPdf)
                    d.ExportToPdf(antiguedad, $"Antiguedad_{contrato}");
                else
                    d.ShowDocument(antiguedad);
            }

        }

        #endregion

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            if (objeto.ValidaAcceso(User.GetUserGroup(), "rptAntiguedad") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            AntiguedadEmpleado(NumeroContrato, Periodo);
        }

        private void btnImpresionRapida_Click(object sender, EventArgs e)
        {
            if (objeto.ValidaAcceso(User.GetUserGroup(), "rptAntiguedad") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            AntiguedadEmpleado(NumeroContrato, Periodo, true);
        }

        private void btnPdf_Click(object sender, EventArgs e)
        {
            if (objeto.ValidaAcceso(User.GetUserGroup(), "rptAntiguedad") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            AntiguedadEmpleado(NumeroContrato, Periodo, false, true);
        }

        private void panelControl5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnEditarReporte_Click(object sender, EventArgs e)
        {
            splashScreenManager1.ShowWaitForm();
            //Prueba de edición de certificado
            XtraReport reporte = new XtraReport();
            reporte.LoadLayoutFromXml(Path.Combine(fnSistema.RutaCarpetaReportesExterno, "rptAntiguedad.repx"));

            //Se le pasa el waitform para que se cierre una vez cargado
            DiseñadorReportes.MostrarEditorLimitado(reporte, "rptAntiguedad.repx", splashScreenManager1);

        }
    }
}
