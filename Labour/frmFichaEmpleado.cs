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
using System.Runtime.InteropServices;

namespace Labour
{
    public partial class frmFichaEmpleado : DevExpress.XtraEditors.XtraForm
    {
        //PARA DESHABILITAR BOTON CERRAR
        [DllImport("user32")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32")]
        static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        const int MF_BYCOMMAND = 0;
        const int MF_DISABLED = 2;
        const int SC_CLOSE = 0xF060;

        //PARA GUARDAR NUMERO DE CONTRATO
        string NumeroContrato = "";

        //PARA GUARDAR PERIODO
        int Periodo = 0;

        public frmFichaEmpleado()
        {
            InitializeComponent();
        }

        //CONSTRUCTOR PARAMETRIZADO
        public frmFichaEmpleado(string Contrato, int Periodo)
        {
            InitializeComponent();
            NumeroContrato = Contrato;
            this.Periodo = Periodo;
        }

        private void frmFichaEmpleado_Load(object sender, EventArgs e)
        {            

            var sm = GetSystemMenu(Handle, false);
            EnableMenuItem(sm, SC_CLOSE, MF_BYCOMMAND | MF_DISABLED);

            if (NumeroContrato != "" && Periodo != 0)
            {                
                CargaFicha(NumeroContrato, Periodo);
            }           
        }

        #region "MANEJO DE DATOS"
        //CARGA DATOS EN FORMULARIO (FICHA)
        private void CargaFicha(string contrato, int periodo)
        {
            Persona person = new Persona();
            person = Persona.GetInfo(contrato, periodo);

            lblNombre.Text = person.NombreCompleto;
            lblRut.Text = fnSistema.fFormatearRut2(person.Rut);
            lblNacimiento.Text = person.Nacimiento.ToShortDateString();
            lblSexo.Text = person.Sexo == 0 ? "Masculino" : "Femenino";
            lblEstadoCivil.Text = person.EstadoCivil;
            lblNacionalidad.Text = person.Nacionalidad;
            lblDireccion.Text = person.direccion;
            lblCiudad.Text = person.ciudad;
            lblFono.Text = person.Telefono;
            lblInicioContrato.Text = person.Ingreso.ToShortDateString();
            lblTerminoContrato.Text = person.Salida.ToShortDateString();
            lblArea.Text = person.NombreArea;

            if (person.Tipocontrato == 0)
                lblTipoContrato.Text = "Indefinido";
            else if (person.Tipocontrato == 1)
            {
                lblTipoContrato.Text = "Plazo Fijo";
            }
            else
            {
                lblTipoContrato.Text = "Obra o faena";
            }

            lblCargo.Text = person.Cargo;
            lblCcosto.Text = person.centro;
            lblCausalTermino.Text = person.ArticuloCausal;
            lblSalud.Text = person.NombreSalud;
            lblAfp.Text = person.NombreAfp;
            lblNumberCuenta.Text = person.NumCuenta;
            lblTipoCuenta.Text = person.NombreTipoCuenta;
            lblFormaPago.Text = person.NombreFormaPago;
            lblBanco.Text = person.NombreBanco;
            lblCajaPrevision.Text = person.NombreCajaPrev;
        }

        //FICHA EMPLEADO ARCHIVO PDF
        private void FichaEmpleado(string contrato, int periodo, bool? ImpresionRapida = false, bool? GeneraPdf = false)
        {

            Empresa emp = new Empresa();
            emp.SetInfo();
            DataSet ds = new DataSet();

            ds = Persona.GetInfoDataset(contrato, periodo);
            if (ds.Tables[0].Rows.Count > 0)
            {
                //PASAMOS COMO DATASOURCE EL DATASET A REPORTE
                rptTrabajador ficha = new rptTrabajador();
                ficha.DataSource = ds.Tables[0];
                ficha.DataMember = "ficha";

                //NO MOSTRAR LOS PARAMETROS
                foreach (DevExpress.XtraReports.Parameters.Parameter parametro in ficha.Parameters)
                {
                    parametro.Visible = false;
                }

                ficha.Parameters["empresa"].Value = emp.Razon;
                //ficha.Parameters["empresa"].Visible = false;

                Documento d = new Documento("", 0);

                if ((bool)ImpresionRapida)
                    d.PrintDocument(ficha);
                else if ((bool)GeneraPdf)
                    d.ExportToPdf(ficha, $"Ficha_{contrato}");
                else
                    d.ShowDocument(ficha);
            }

        }


        #endregion

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            if (objeto.ValidaAcceso(User.GetUserGroup(), "rptfichatrabajador") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            FichaEmpleado(NumeroContrato, Periodo);
        }

        private void btnImpresionRapida_Click(object sender, EventArgs e)
        {
            if (objeto.ValidaAcceso(User.GetUserGroup(), "rptfichatrabajador") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            FichaEmpleado(NumeroContrato, Periodo, true);
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnPdf_Click(object sender, EventArgs e)
        {
            if (objeto.ValidaAcceso(User.GetUserGroup(), "rptfichatrabajador") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            FichaEmpleado(NumeroContrato, Periodo, false, true);
        }
    }
}