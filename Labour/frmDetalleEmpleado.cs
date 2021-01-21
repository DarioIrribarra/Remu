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

namespace Labour
{
    public partial class frmDetalleEmpleado : DevExpress.XtraEditors.XtraForm
    {
        public frmDetalleEmpleado()
        {
            InitializeComponent();
        }

        private string ContratoFicha = "";
        private int PeriodoFicha = 0;
        private bool Historica = false;

        /// <summary>
        /// Constructor para cargar datos de acuerdo a ficha principal seleccionada
        /// </summary>
        /// <param name="pContrato">N° de contrato del trabajador</param>
        /// <param name="pPeriodo">Periodo de la ficha</param>
        /// <param name="Historica">Indica si es o no una ficha histórica (Si es histórica no se puede editar)</param>
        public frmDetalleEmpleado(string pContrato, int pPeriodo, bool Historica)
        {
            InitializeComponent();
            this.ContratoFicha = pContrato;
            this.PeriodoFicha = pPeriodo;
            this.Historica = Historica;
        }

        private string SqlCombo = "SELECT codigo, descrip FROM cpcausaslegales ORDER BY codigo";

        private void frmDetalleEmpleado_Load(object sender, EventArgs e)
        {
            //Cargamos combobox
            datoCombobox.spllenaComboBox(SqlCombo, txtCausas, "codigo", "descrip");
            LoadData();

        }

        /// <summary>
        /// Carga informacion correspondiente a la ficha
        /// </summary>
        private void LoadData()
        {
            string sql = "SELECT jefe, numtar, causalegalcontratoest as causa FROM trabajador where contrato=@pContrato AND anomes=@pPeriodo";
            SqlCommand cmd;
            SqlConnection cn;
            SqlDataReader rd;

            if (Persona.ExisteContrato(this.ContratoFicha, this.PeriodoFicha))
            {
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
                                cmd.Parameters.Add(new SqlParameter("@pContrato", this.ContratoFicha));
                                cmd.Parameters.Add(new SqlParameter("@pPeriodo", this.PeriodoFicha));

                                rd = cmd.ExecuteReader();
                                if (rd.HasRows)
                                {
                                    while (rd.Read())
                                    {
                                        //Cargamos datos en campos corresponidientes
                                        txtCausas.EditValue = Convert.ToInt32(rd["causa"]);
                                        txtJefe.Text = Convert.ToString(rd["jefe"]);
                                        txtNTarj.Text = Convert.ToString(rd["numtar"]);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                   //Error...
                }
            }
            else
            {
                XtraMessageBox.Show("N° de contrato no válido", "No se encontró información", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        /// <summary>
        /// Data para log
        /// </summary>
        /// <param name="pContrato"></param>
        /// <param name="pPeriodo"></param>
        /// <returns></returns>
        private Hashtable GetDataLog(string pContrato, int pPeriodo)
        {
            string sql = "SELECT jefe, numtar, causalegalcontratoest as causal FROM trabajador where contrato=@pContrato AND anomes=@pPeriodo";
            SqlCommand cmd;
            SqlConnection cn;
            SqlDataReader rd;
            Hashtable s = new Hashtable();

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
                            cmd.Parameters.Add(new SqlParameter("@pContrato", pContrato));
                            cmd.Parameters.Add(new SqlParameter("@pPeriodo", pPeriodo));

                            rd = cmd.ExecuteReader();
                            if (rd.HasRows)
                            {
                                while (rd.Read())
                                {
                                    s.Add("jefe", Convert.ToString(rd["jefe"]));
                                    s.Add("tarjeta", Convert.ToString(rd["numtar"]));
                                    s.Add("causal", Convert.ToInt32(rd["causal"]));
                                }
                            }

                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                //error...
            }

            return s;
        }


        /// <summary>
        /// Registra cambios en log
        /// </summary>
        /// <param name="pData"></param>
        /// <param name="pJefe"></param>
        /// <param name="pNumTar"></param>
        /// <param name="pCausal"></param>
        private void UpdateLog(Hashtable pData, string pJefe, string pNumTar, int pCausal)
        {
            if (pData.Count > 0)
            {
                if (pData["jefe"].ToString() != pJefe)
                {
                    logRegistro lg = new logRegistro(User.getUser(), "SE MODIFICA CAMPO <<JEFE>> EN TABLA TRABAJADOR N° " + this.ContratoFicha, "TRABAJADOR", Convert.ToString(pData["jefe"]), pJefe, "MODIFICAR");
                    lg.Log();
                }
                if (pData["tarjeta"].ToString() != pNumTar)
                {
                    logRegistro lg = new logRegistro(User.getUser(), "SE MODIFICA CAMPO <<NUMTAR>> EN TABLA TRABAJADOR N° " + this.ContratoFicha, "TRABAJADOR", Convert.ToString(pData["tarjeta"]), pNumTar, "MODIFICAR");
                    lg.Log();
                }
                if (pData["causal"].ToString() != pCausal.ToString())
                {
                    logRegistro lg = new logRegistro(User.getUser(), "SE MODIFICA CAMPO <<CAUSALEGALCONTRATOEST>> EN TABLA TRABAJADOR N° " + this.ContratoFicha, "TRABAJADOR", Convert.ToString(pData["causal"]), pCausal.ToString(), "MODIFICAR");
                    lg.Log();
                }
            }
        }

        /// <summary>
        /// Permite actualizar informacion del trabajador
        /// </summary>
        /// <param name="pContrato">N° de contrato del trabajador</param>
        /// <param name="pPeriodo">Periodo al que corresponde la ficha a editar</param>
        private void UpdateInfo(string pContrato, int pPeriodo, string pJefe, string pNumTar, int pCausal)
        {
            string sql = "UPDATE trabajador SET jefe=@pJefe, numtar=@pNumtar, causalegalcontratoest=@pCausal " +
                        " WHERE contrato=@pContrato AND anomes=@pPeriodo";
            SqlCommand cmd;
            SqlConnection cn;
            int count = 0;
            Hashtable data = new Hashtable();
            data = GetDataLog(pContrato, pPeriodo);

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
                            cmd.Parameters.Add(new SqlParameter("@pContrato", pContrato));
                            cmd.Parameters.Add(new SqlParameter("@pPeriodo", pPeriodo));
                            cmd.Parameters.Add(new SqlParameter("@pJefe", pJefe));
                            cmd.Parameters.Add(new SqlParameter("@pNumTar", pNumTar));
                            cmd.Parameters.Add(new SqlParameter("@pCausal", pCausal));

                            count = cmd.ExecuteNonQuery();
                            if (count > 0)
                            {
                                XtraMessageBox.Show("Información actualizada correctamente", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                UpdateLog(data, pJefe, pNumTar, pCausal);
                                LoadData();
                            }
                            else
                            {
                                XtraMessageBox.Show("No se pudo actualizar informacion del trabajador", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                //error...
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {

            ///Si es true indica que esta ficha es de meses pasados (historica)
            ///Solo lectura
            if (this.Historica)
            {
                XtraMessageBox.Show("No puedes hacer cambios en este ficha", "Ficha histórica", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            if (txtCausas.Properties.DataSource == null || txtCausas.EditValue == null)
            {
                XtraMessageBox.Show("Por favor selecciona una causa legal de contrato válida", "Información", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                txtCausas.Focus();
                return;
            }

            //Hacemos update
            UpdateInfo(this.ContratoFicha, this.PeriodoFicha, txtJefe.Text, txtNTarj.Text, Convert.ToInt32(txtCausas.EditValue));


        }

        private void txtCausas_EditValueChanged(object sender, EventArgs e)
        {
            if (txtCausas.Properties.DataSource != null && txtCausas.EditValue != null)
            {
                LookUpEdit Causal = sender as LookUpEdit;
                CausaLegal legal = new CausaLegal();
                legal.SetInfo(Convert.ToInt32(Causal.EditValue));

                //Cargamos datos correspondientes
                txtDescCausal.Text = legal.Descripcion;
                txtDetalleCausal.Text = legal.Detalle;
               
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}