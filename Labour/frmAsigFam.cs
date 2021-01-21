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
    public partial class frmAsigFam : DevExpress.XtraEditors.XtraForm
    {
        public frmAsigFam()
        {
            InitializeComponent();
        }

        private void panelControl1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            Close();
        }

        private void frmAsigFam_Load(object sender, EventArgs e)
        {
            CargarData();
        }

        #region "MANEJO DE DATOS"
        //ACTUALIZAR VALOR TRAMOS
        private void ActualizarValoresTramos(double pTramoA, double pTramoB, double pTramoC, double pTramoD)
        {
            string sql = "BEGIN TRY " +
                         "BEGIN TRANSACTION " +    
                             "UPDATE asignacionFamiliar SET valor = @pValorA WHERE tramo = '1' " +
                             "UPDATE asignacionFamiliar SET valor = @pValorB WHERE tramo = '2' " +
                             "UPDATE asignacionFamiliar SET valor = @pValorC WHERE tramo = '3' " +
                             "UPDATE asignacionFamiliar SET valor = @pValorD WHERE tramo = '4' " +
                             "COMMIT TRANSACTION " +
                         "END TRY " +
                         "BEGIN CATCH " +
                            "IF @@TRANCOUNT > 0 " +
                                "ROLLBACK " +
                        "END CATCH";
            SqlCommand cmd;
            Hashtable Data = new Hashtable();
            Data = PrecargaData();
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    if (fnSistema.ConectarSQLServer())
                    {
                        using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@pValorA", pTramoA));
                            cmd.Parameters.Add(new SqlParameter("@pValorB", pTramoB));
                            cmd.Parameters.Add(new SqlParameter("@pValorC", pTramoC));
                            cmd.Parameters.Add(new SqlParameter("@pValorD", pTramoD));

                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                XtraMessageBox.Show("Actualizacion realizada correctamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                ComparaData(Data, pTramoA, pTramoB, pTramoC, pTramoD);
                                CargarData();
                            }                                
                            else
                                XtraMessageBox.Show("Error al intentar actualizar", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            cmd.Dispose();
                            fnSistema.sqlConn.Close();
                            
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        //ACTUALIZAR CAJAS DE TEXTO CON VALORES DESDE BD
        private void CargarData()
        {
            string sql = "SELECT tramo, valor FROM AsignacionFamiliar";
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //CARGAMOS DATA
                                if((string)rd["tramo"] == "1")
                                    txtA.Text = Convert.ToDouble(rd["valor"]) + "";
                                if((string)rd["tramo"] == "2")
                                    txtB.Text = Convert.ToDouble(rd["valor"]) + "";
                                if((string)rd["tramo"] == "3")
                                    txtC.Text = Convert.ToDouble(rd["valor"]) + "";
                                if((string)rd["tramo"] == "4")
                                    txtD.Text = Convert.ToDouble(rd["valor"]) + "";
                            }
                        }                        
                    }
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                    rd.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }


        /*PARA LOG*/
        /*PRECARGA DE DATOS*/
        private Hashtable PrecargaData()
        {
            Hashtable data = new Hashtable();
            string sql = "SELECT tramo, valor FROM asignacionFamiliar ORDER BY tramo";
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                data.Add(Convert.ToInt32(rd["tramo"]) + "", Convert.ToDouble(rd["valor"]));                                
                            }
                        }
                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                        rd.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return data;
        }

        /*COMPARA VALORES*/
        private void ComparaData(Hashtable Data, double pValue1, double pValue2, 
            double pValue3, double pValue4)
        {
            if (Data.Count > 0)
            {
                if (Convert.ToDouble(Data["1"]) != pValue1)
                {
                    logRegistro Log = new logRegistro(User.getUser(), "SE MODIFICA VALOR CARGA FAMILIAR TRAMO A", "ASIGNACIONFAMILIAR", Convert.ToDouble(Data["1"]).ToString(), pValue1 + "", "MODIFICAR");
                    Log.Log();
                }
                if (Convert.ToDouble(Data["2"]) != pValue2)
                {
                    logRegistro Log = new logRegistro(User.getUser(), "SE MODIFICA VALOR CARGA FAMILIAR TRAMO B", "ASIGNACIONFAMILIAR", Convert.ToDouble(Data["2"]).ToString(), pValue2 + "", "MODIFICAR");
                    Log.Log();
                }
                if (Convert.ToDouble(Data["3"]) != pValue3)
                {
                    logRegistro Log = new logRegistro(User.getUser(), "SE MODIFICA VALOR CARGA FAMILIAR TRAMO C", "ASIGNACIONFAMILIAR", Convert.ToDouble(Data["3"]).ToString(), pValue3 + "", "MODIFICAR");
                    Log.Log();
                }
                if (Convert.ToDouble(Data["4"]) != pValue4)
                {
                    logRegistro Log = new logRegistro(User.getUser(), "SE MODIFICA VALOR CARGA FAMILIAR TRAMO D", "ASIGNACIONFAMILIAR", Convert.ToDouble(Data["4"]).ToString(), pValue4 + "", "MODIFICAR");
                    Log.Log();
                }
            }
        }
        #endregion

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            if (txtA.Text == "")
            { XtraMessageBox.Show("Por favor ingresa un valor valido para el tramo A", "informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtA.Focus(); return; }
            if (txtB.Text == "")
            { XtraMessageBox.Show("Por favor ingresa un valor valido para el tramo B", "informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtB.Focus(); return; }
            if (txtC.Text == "")
            { XtraMessageBox.Show("Por favor ingresa un valor valido para el tramo C", "informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtC.Focus(); return; }
            if (txtD.Text == "")
            { XtraMessageBox.Show("Por favor ingresa un valor valido para el tramo D", "informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtD.Focus(); return; }

            ActualizarValoresTramos(Convert.ToDouble(txtA.Text), Convert.ToDouble(txtB.Text), 
                Convert.ToDouble(txtC.Text), Convert.ToDouble(txtD.Text));
        }

        private void txtA_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else
                e.Handled = true;
        }

        private void txtB_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else
                e.Handled = true;
        }

        private void txtC_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else
                e.Handled = true;
        }

        private void txtD_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else
                e.Handled = true;
        }
    }
}