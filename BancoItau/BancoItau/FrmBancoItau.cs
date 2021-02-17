using DevExpress.XtraEditors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BancoItau
{
    public partial class FrmBancoItau : DevExpress.XtraEditors.XtraForm
    {
        Query query = new Query();
        /// <summary>
        /// Objeto interfaz solo para saber si este formulario se invocó como hijo de otro formulario.
        /// </summary>
        public InterfazItau opener { get; set; }
        ///<summary>
        ///OBJETO QUE TIENE EL LISTADO
        ///</summary>
        //Data listadoBanco = new Data();
        ///// <summary>
        ///// Listado de personas que viene desde formulario principal
        ///// </summary>
        public List<Cuerpo> Listado { get; set; }
        /// <summary>
        /// Representa la cabecera del informe
        /// </summary>
        //public Header Cabecera { get; set; }
        public Hashtable HashHeader { get; set; }
        /// <summary>
        /// RUT EMPRESA PARA ACTUALIZACIÓN DE CUENTA
        /// </summary>
        public string RutEmpresa { get; set; }
        /// <summary>
        /// NRO DE CUENTA CUANDO ES CUENTA CORRIENTE
        /// </summary>
        public string NumeroCuenta { get; set; }

        /// <summary>
        /// DIRECCION DE RUTA DONDE SE GUARDA EL ARCHIVO
        /// </summary>
        public string RutaArchivo { get; set; }

        /// <summary>
        /// CADENA DE CONEXIÓN A SQLSERVER
        /// </summary>
        public string StringConnection { get; set; }

        public string ConsultaSQL { get; set; }

        public FrmBancoItau()
        {
            InitializeComponent();
        }

        public FrmBancoItau(List<Cuerpo> pListado, Hashtable pHashHeader, string pStringConnection, string dataSql)
        {
            InitializeComponent();
            Listado = pListado;
            HashHeader = pHashHeader;
            RutEmpresa = HashHeader["rutempresa"].ToString();
            //verifico que exista la empresa
            query.SelectEmpresa(RutEmpresa);
            StringConnection = pStringConnection;
            Configuracion.ConnectionString = StringConnection;
            ConsultaSQL = dataSql;
        }

        private void FrmBancoItau_Load(object sender, EventArgs e)
        {
            labelControl3.Visible = false;
            cbxFormatoArchivo.Visible = false;
            chkCambiarCuenta.Visible = false;
            //CARGO COMBOBOX METODO DE PAGO
            query.MediosRespaldo(cbxCargarMetodopago);
            query.FormatoArchivos(cbxFormatoArchivo);

            //CARGO NUMEROS DE CUENTA
            NumeroCuenta = query.CargarNumeroCuenta(txtNumeroCuenta, RutEmpresa);
            txtNumeroCuenta.Text = NumeroCuenta;
            txtNumeroCuenta.ReadOnly = false;
            //chkCambiarCuenta.Checked = true;

            //CARGO LA RUTA POR DEFECTO
            RutaArchivo = query.CargarRutaArchivo(txtRuta, RutEmpresa);
            txtRuta.Text = RutaArchivo;

            //CARGAMOS BOTÓN DE REPORTE DESHABILITADO
            btnReporte.Enabled = false;
        }

        private void txtRut_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void textEdit1_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {

            if (txtNumeroCuenta.Text.Length == 0)
            { XtraMessageBox.Show("Por favor ingresa un número de cuenta o cheque", "Número Medio Respaldo", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtNumeroCuenta.Focus(); return; }

            if (txtRuta.Text.Length == 0)
            { XtraMessageBox.Show("Por favor ingresa una ruta válida", "Ruta", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtRuta.Focus(); return; }

            if (Directory.Exists(txtRuta.Text) == false)
            { XtraMessageBox.Show("Directorio ingresado no existe", "Directorio", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtRuta.Focus(); return; }


            string FileName = txtRuta.Text;
            if (Listado.Count > 0)
            {
                DialogResult advertencia = XtraMessageBox.Show("¿Realmente deseas crear archivo?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (advertencia == DialogResult.No)
                    return;

                //GENERAMOS ARCHIVO FINAL
                FileName = FileName + @"\NominaBanco" + ".txt";

                #region DATOS A ENVIAR PARA CREACION 

                DataBancoItau banco = new DataBancoItau();
                banco.CrearArchivoItau(FileName, Listado, HashHeader, cbxCargarMetodopago.EditValue.ToString(), 
                    txtNumeroCuenta.Text, txtDescripcion.Text, cbxFormatoArchivo.EditValue.ToString(), txtDescripcion.TextLength);


                //CAMBIAR NUMERO DE CUENTA PREDEFINIDO SOLO CUANDO ES TRANSFERENCIA
                if (cbxCargarMetodopago.ItemIndex == 0)
                {
                    if (txtNumeroCuenta.Text != NumeroCuenta)
                    {
                        DialogResult grabarNumeroCuenta = XtraMessageBox.Show("¿Desea reemplazar el número de cuenta predeterminada por la ingresada?", "Número Cuenta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (grabarNumeroCuenta == DialogResult.No)
                        {

                        }
                        else
                        {
                            if (query.UpdateNumeroCuenta(txtNumeroCuenta.Text) == "1")
                            {
                                XtraMessageBox.Show("Cuenta actualizada exitosamente", "Actualización", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                XtraMessageBox.Show("No se pudo actualizar el número de cuenta", "Actualización", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            }
                        }

                    }
                }

                if (File.Exists(FileName))
                {
                    XtraMessageBox.Show($"Archivo creado en {FileName}", "Archivo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //VALIDACIÓN SI SE REQUIERE GUARDAR ESA RUTA COMO LA NUEVA
                    if (txtRuta.Text != RutaArchivo)
                    {
                        DialogResult grabarRuta = XtraMessageBox.Show("¿Desea reemplazar la ruta predeterminada por la ingresada?", "Ruta Archivo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (grabarRuta == DialogResult.No)
                        {

                        }
                        else
                        {
                            if (query.UpdateRutaArchivo(txtRuta.Text) == "1")
                            {
                                XtraMessageBox.Show("Ruta predeterminada de archivo actualizada exitosamente", "Actualización", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                XtraMessageBox.Show("No se pudo actualizar la ruta predeterminada del archivo", "Actualización", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            }
                        }
                    }
                }
                else
                {
                    XtraMessageBox.Show("No se pudo generar el archivo. Cambie la ruta a una existente", "Ruta Archivo", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }

                DialogResult pregunta = XtraMessageBox.Show($"¿Deseas abrir archivo?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (pregunta == DialogResult.Yes)
                    Process.Start(FileName);
                #endregion


                btnReporte.Enabled = true;
                //Close();
            }
            else
            {
                XtraMessageBox.Show("No se pudo generar archivo final", "Archivo", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
        }

        private void btnRutaArchivo_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            if (folder.ShowDialog() == DialogResult.OK)
            {
                if (Directory.Exists(folder.SelectedPath))
                {
                    txtRuta.Text = folder.SelectedPath;
                }
                else
                {
                    XtraMessageBox.Show("El directorio seleccionado no existe", "Directorio", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
        }

        private void cbxCargarMetodopago_EditValueChanged(object sender, EventArgs e)
        {
            if (cbxCargarMetodopago.ItemIndex == 0)
            {
                //txtNumeroCuenta.ReadOnly = true;
                //chkCambiarCuenta.Enabled = true;
                lblMedioRespaldo.Text = "Número de cuenta:";
                txtNumeroCuenta.Text = NumeroCuenta;
            }
            else
            {
                //txtNumeroCuenta.ReadOnly = false;
                //chkCambiarCuenta.Enabled = false;
                lblMedioRespaldo.Text = "Número de cheque:";
                txtNumeroCuenta.Text = "";
            }
        }

        private void chkCambiarCuenta_CheckedChanged(object sender, EventArgs e)
        {
            //if (chkCambiarCuenta.Checked)
            //{
            //    txtNumeroCuenta.ReadOnly = false;
            //    txtNumeroCuenta.Text = "";
            //}
            //else
            //{
            //    txtNumeroCuenta.ReadOnly = true;
            //    txtNumeroCuenta.Text = NumeroCuenta;

            //}
        }

        private void btnReporte_Click(object sender, EventArgs e)
        {
            //COMENZAMOS LA CREACIÓN DEL REPORTE 
            rptBancoItau rptBancoItau = new rptBancoItau(StringConnection, ConsultaSQL);
            rptBancoItau.LoadLayoutFromXml(Path.Combine(Configuracion.RutaCarpetaReportesExterno, "rptBancoItau.repx"));
            DatosReporte datosReporte = new DatosReporte();

            foreach (DevExpress.XtraReports.Parameters.Parameter parametro in rptBancoItau.Parameters)
            {
                parametro.Visible = false;
            }

            rptBancoItau.Parameters["rutEmpresa"].Value = datosReporte.fFormatearRut2(HashHeader["rutempresa"].ToString());
            rptBancoItau.Parameters["nombreEmpresa"].Value = HashHeader["nombreEmpresa"];
            rptBancoItau.Parameters["direccionEmpresa"].Value = HashHeader["direccionEmpresa"];
            rptBancoItau.Parameters["ciudadEmpresa"].Value = HashHeader["ciudadEmpresa"];
            rptBancoItau.ListadoRpt = setListReporte(StringConnection, ConsultaSQL);
            rptBancoItau.dataSql = ConsultaSQL;
            //rptBancoItau.SaveLayoutToXml(Path.Combine(Configuracion.RutaCarpetaReportesExterno, "rptBancoItau.repx"));
            datosReporte.ShowDocument(rptBancoItau);
        }

        private List<Cuerpo> setListReporte(string StringConnection, string ConsultaSQL)
        {
            DatosReporte datosReporte = new DatosReporte();
            Connection con = new Connection(StringConnection);
            SqlConnection sqlcon = con.OpenConnection();
            List<Cuerpo> listaReporte = new List<Cuerpo>();
            if (sqlcon != null)
            {
                using (sqlcon)
                {
                    using (SqlCommand sqlcmd = new SqlCommand(ConsultaSQL, sqlcon))
                    {
                        SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                        while (sqlrdr.Read())
                        {
                            listaReporte.Add(new Cuerpo()
                            {
                                Rut = sqlrdr["rut"].ToString(),
                                Nombre = sqlrdr["nombre"].ToString(),
                                NroBanco = int.Parse(sqlrdr["banco"].ToString()),
                                NombreBanco = sqlrdr["nombreBanco"].ToString(),
                                TipoDeCuenta = sqlrdr["NombreTipoCuenta"].ToString(),
                                NroCuenta = sqlrdr["cuenta"].ToString(),
                                Monto = Convert.ToDouble(sqlrdr["valor"].ToString())
                            });
                        }
                    }
                }
            }
            //ORDENO LA LISTA EN BASE AL NUMERO DE BANCO
            //listaReporte.Sort((x, y) => x.NroBanco.CompareTo(y.NroBanco));

            listaReporte= listaReporte.OrderBy(x => x.NroBanco).ThenBy(x => x.Nombre).ToList<Cuerpo>();
            return listaReporte;
        }

        private void cbxFormatoArchivo_EditValueChanged(object sender, EventArgs e)
        {
            if (cbxFormatoArchivo.ItemIndex == 0)
            {
                cbxCargarMetodopago.ItemIndex = 0;
                cbxCargarMetodopago.ReadOnly = false;
                txtNumeroCuenta.Text = NumeroCuenta;
            }
            else
            {
                cbxCargarMetodopago.ItemIndex = 0;
                cbxCargarMetodopago.ReadOnly = true;
                txtNumeroCuenta.Text = NumeroCuenta;
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnEditarReporte_Click(object sender, EventArgs e)
        {
            //COMENZAMOS LA CREACIÓN DEL REPORTE 
            rptBancoItau rptBancoItau = new rptBancoItau(StringConnection, ConsultaSQL);
            rptBancoItau.LoadLayoutFromXml(Path.Combine(Configuracion.RutaCarpetaReportesExterno, "rptBancoItau.repx"));
            DatosReporte datosReporte = new DatosReporte();

            foreach (DevExpress.XtraReports.Parameters.Parameter parametro in rptBancoItau.Parameters)
            {
                parametro.Visible = false;
            }

            rptBancoItau.Parameters["rutEmpresa"].Value = datosReporte.fFormatearRut2(HashHeader["rutempresa"].ToString());
            rptBancoItau.Parameters["nombreEmpresa"].Value = HashHeader["nombreEmpresa"];
            rptBancoItau.Parameters["direccionEmpresa"].Value = HashHeader["direccionEmpresa"];
            rptBancoItau.Parameters["ciudadEmpresa"].Value = HashHeader["ciudadEmpresa"];
            rptBancoItau.ListadoRpt = setListReporte(StringConnection, ConsultaSQL);
            rptBancoItau.dataSql = ConsultaSQL;
            
            splashScreenManager1.ShowWaitForm();

            DiseñadorReportes.MostrarEditorLimitado(rptBancoItau, "rptBancoItau.repx", splashScreenManager1);

        }
    }
}
