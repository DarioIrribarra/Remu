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

namespace Labour
{
    public partial class frmParametroFormula : DevExpress.XtraEditors.XtraForm
    {
        /// <summary>
        /// Para cargar grilla.
        /// </summary>
        private string SqlConsulta = "SELECT codPar, descPar, valPar FROM parfor " +
                                     "ORDER BY SUBSTRING(codpar, 1, 1), CONVERT(INT, SUBSTRING(codPar, 2, LEN(codPar)))";

        public frmParametroFormula()
        {
            InitializeComponent();
        }

        private void frmParametroFormula_Load(object sender, EventArgs e)
        {
            fnSistema.spllenaGridView(gridParametro, SqlConsulta);
            fnSistema.spOpcionesGrilla(viewParametro);
            ColumnasGrilla();
            Cargainfo(0);
            
        }

        #region "MANEJO DE DATOS"
        private void ColumnasGrilla()
        {
            if (viewParametro.RowCount > 0)
            {
                viewParametro.Columns[0].Caption = "Codigo";
                viewParametro.Columns[0].Width = 30;
                viewParametro.Columns[0].AppearanceCell.FontStyleDelta = FontStyle.Bold;

                viewParametro.Columns[1].Caption = "Descripcion";
                viewParametro.Columns[1].Width = 200;

                viewParametro.Columns[2].Caption = "Valor";
                viewParametro.Columns[2].Width = 40;
                viewParametro.Columns[2].DisplayFormat.FormatString = "n0";
                viewParametro.Columns[2].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;

            }
        }

        private void Cargainfo(int? pPos = -1)
        {
            if (viewParametro.RowCount > 0)
            {
                if (pPos != -1)
                    viewParametro.FocusedRowHandle = Convert.ToInt32(pPos);

                txtDesc.Text = (string)viewParametro.GetFocusedDataRow()["descPar"];
                txtValor.Text = Convert.ToDouble(viewParametro.GetFocusedDataRow()["valPar"]).ToString();
            }
        }

        //VERIFICAR NUMERO DECIMAL CORRECTO
        private bool fnDecimal(string cadena)
        {
            if (cadena.Length == 0) return false;

            //recorrer cadena y verificar que tenga solo una coma
            int coma = 0;
            for (int position = 0; position < cadena.Length; position++)
            {
                if (cadena[position] == ',') coma++;
            }

            if (coma > 1) return false;

            string[] subcadena = new string[2];
            if (coma == 1)
            {
                subcadena = cadena.Split(',');

                //SI DESPUES DE LA CADENA TIENE MAS DE DOS DIGITOS NO ES CORRECTO
                if (subcadena[1].Length > 2) return false;
                if (subcadena[1].Length == 0) return false;
                if (subcadena[0].Length == 0) return false;

                return true;
            }
            else
            {
                //SI NO TIENE COMAS SOLO ES UN NUMERO
                if (cadena.Length > 7) return false;
                return true;
            }

        }
        #endregion

        private void btnGrabar_Click(object sender, EventArgs e)
        {
            if (viewParametro.RowCount == 0)
            { XtraMessageBox.Show("Registro no válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (fnDecimal(txtValor.Text) == false)
            { XtraMessageBox.Show("Por favor verifica el valor ingresado", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtValor.Focus(); return; }

            ParametroFormula Form = new ParametroFormula();

            if (viewParametro.RowCount > 0)
            {
                //Actualizamos información.
                Form.Actualizar((string)viewParametro.GetFocusedDataRow()["codpar"], txtDesc.Text, Convert.ToDouble(txtValor.Text));

                fnSistema.spllenaGridView(gridParametro, SqlConsulta);
                Cargainfo();                
            }
        }

        private void gridParametro_Click(object sender, EventArgs e)
        {
            if (viewParametro.RowCount > 0)
            {
                Cargainfo();
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void gridParametro_EditorKeyUp(object sender, KeyEventArgs e)
        {

        }

        private void txtValor_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar) || e.KeyChar == (char)44)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void gridParametro_KeyUp(object sender, KeyEventArgs e)
        {
            if (viewParametro.RowCount > 0)
            {
                Cargainfo();
            }
        }

        private void textEdit1_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtValor_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }
    }
}