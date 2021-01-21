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
using System.IO;

namespace Labour
{
    public partial class frmParametros : DevExpress.XtraEditors.XtraForm
    {
        public frmParametros()
        {
            InitializeComponent();
        }

        private void frmParametros_Load(object sender, EventArgs e)
        {
            fnSistema.fnComboPorcentajeSuspension(txtPorcentaje);

            CargarDatos();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void labelControl5_Click(object sender, EventArgs e)
        {

        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (txtRutaRespaldo.Text.Length == 0)
            { XtraMessageBox.Show("Por favor ingresa una ruta válida", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (txtRutaContratos.Text.Length > 0 &&  File.Exists(txtRutaContratos.Text) == false)
            { XtraMessageBox.Show("Por favor verifique que la ruta ingresada para contratos sea válida o exista", "Archivo o Directorio no válido", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtRutaContratos.Focus(); return; }

            if (txtRutaFiniquito.Text.Length > 0 && File.Exists(txtRutaFiniquito.Text) == false)
            { XtraMessageBox.Show("Por favor verifique que la ruta ingresada para finiquitos sea válida o exista", "Archivo o Directorio no válido", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtRutaFiniquito.Focus(); return; }

            if (txtRutaCartas.Text.Length > 0 &&  File.Exists(txtRutaCartas.Text) == false)
            { XtraMessageBox.Show("Por favor verifique que la ruta ingresada para cartas de aviso sea válida o exista", "Archivo o Directorio no válido", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtRutaCartas.Focus(); return; }

            if (txtPorcentaje.Properties.DataSource == null || txtPorcentaje.EditValue == null)
            { XtraMessageBox.Show("Por favor seleccione un porcentaje de calculo para suspension válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            Configuracion conf = new Configuracion();
            bool correcto = false;
            try
            {
                if (conf.Existe())
                {
                    correcto = conf.Modifica(txtRutaRespaldo.Text, txtRutaFiniquito.Text, txtRutaContratos.Text, txtRutaCartas.Text, Convert.ToInt32(txtPorcentaje.EditValue));
                    if (correcto)
                    {
                        XtraMessageBox.Show("Datos guardados correctamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        Configuracion.SetGlobalConfiguration();
                      
                       
                    }
                    else
                    {
                        XtraMessageBox.Show("No se pudieron guardar los datos", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    correcto = conf.Nuevo(txtRutaRespaldo.Text, txtRutaFiniquito.Text, txtRutaContratos.Text, txtRutaCartas.Text, Convert.ToInt32(txtPorcentaje.EditValue));
                    if (correcto)
                    {
                        XtraMessageBox.Show("Datos guardados correctamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Configuracion.SetGlobalConfiguration();
                    }
                    else
                    {
                        XtraMessageBox.Show("No se pudieron guardar los datos", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }   
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("No se pudo guardar la información", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void CargarDatos()
        {
            try
            {
                Configuracion conf = new Configuracion();
                conf.SetConfiguracion();

                txtRutaRespaldo.Text = conf.RutaRespaldo;
                txtRutaFiniquito.Text = conf.RutaPlantillaFiniquito;
                txtRutaContratos.Text = conf.RutaPlantillaContrato;
                txtRutaCartas.Text = conf.RutaPlantillaAviso;
                txtPorcentaje.EditValue = conf.PorcentajeSuspension;
            }
            catch (Exception ex)
            {
               //ERROR...
            }
        }

        private void btnFiniquito_Click(object sender, EventArgs e)
        {
            OpenFileDialog di = new OpenFileDialog();
            di.Filter = "Word 2007 Documents (*.docx)|*.docx|Word 97-2003 Documents (*.doc)|*.doc";
            di.Title = "Selecciona una plantilla";
            if (di.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(di.FileName))
                {
                    txtRutaFiniquito.Text = di.FileName;
                }
            }
        }

        private void btnContrato_Click(object sender, EventArgs e)
        {
            OpenFileDialog di = new OpenFileDialog();
            di.Filter = "Word 2007 Documents (*.docx)|*.docx|Word 97-2003 Documents (*.doc)|*.doc";
            di.Title = "Selecciona una plantilla";
            if (di.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(di.FileName))
                {
                    txtRutaContratos.Text = di.FileName;
                }
            }
        }

        private void btnCarta_Click(object sender, EventArgs e)
        {
            OpenFileDialog di = new OpenFileDialog();
            di.Filter = "Word 2007 Documents (*.docx)|*.docx|Word 97-2003 Documents (*.doc)|*.doc";
            di.Title = "Selecciona una plantilla";
            if (di.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(di.FileName))
                {
                    txtRutaCartas.Text = di.FileName;
                }
            }
        }

        private void pictureEdit1_PopupMenuShowing(object sender, DevExpress.XtraEditors.Events.PopupMenuShowingEventArgs e)
        {
            e.PopupMenu.Items.Clear();
        }
    }
}