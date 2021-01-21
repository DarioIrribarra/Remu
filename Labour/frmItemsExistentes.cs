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

namespace Labour
{
    public partial class frmItemsExistentes : DevExpress.XtraEditors.XtraForm
    {
        public int Anomes { get; set; } = 0;
        public string Item { get; set; } = "";

        public frmItemsExistentes()
        {
            InitializeComponent();
        }

        public frmItemsExistentes(int anomes, string item)
        {
            InitializeComponent();
            Anomes = anomes;
            Item = item;
        }

        private void frmItemsExistentescs_Load(object sender, EventArgs e)
        {
            if (Anomes != 0 && Item != "")
            {
                lblTitulo.Text = "Trabajadores que actualmente tienen el item: '" + Item + "'";

                string sql = string.Format("select itemTrabajador.rut, concat(nombre, ' ', apepaterno, ' ', apematerno) " +
                    "as nombre, coditem, numitem FROM itemtrabajador " +
                    "INNER JOIN trabajador ON trabajador.contrato = itemTrabajador.contrato " +
                    "AND trabajador.anomes = itemTrabajador.anomes " +
                    "where itemTrabajador.anomes = {0} AND coditem = '{1}'", Anomes, Item);

                fnSistema.spllenaGridView(gridItem, sql);
                fnSistema.spOpcionesGrilla(viewItem);
                ColumnasGrilla();
            }
        }

        #region "MANEJO DE DATOS"
        private void ColumnasGrilla()
        {
            viewItem.Columns[0].Caption = "Rut";
            viewItem.Columns[0].DisplayFormat.FormatString = "Rut";
            viewItem.Columns[0].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            viewItem.Columns[0].DisplayFormat.Format = new FormatCustom();

            viewItem.Columns[1].Caption = "Trabajador";
            viewItem.Columns[1].Width = 170;

            viewItem.Columns[2].Caption = "Item";

            viewItem.Columns[3].Caption = "N°";
            viewItem.Columns[3].Width = 20;
        }
        #endregion

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}