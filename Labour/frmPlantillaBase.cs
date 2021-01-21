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
    public partial class frmPlantillaBase : DevExpress.XtraEditors.XtraForm
    {
        public IPlantillaFormula opener { get; set; }
        public frmPlantillaBase()
        {
            InitializeComponent();
        }

        private void frmPlantillaBase_Load(object sender, EventArgs e)
        {
            fnComboBase(txtBase);
            txtBase.ItemIndex = 0;
        }


        private void fnComboBase(LookUpEdit pBase)
        {
            List<datoCombobox> datos = new List<datoCombobox>();

            datos.Add(new datoCombobox() { KeyInfo =1, descInfo="IF"});
            datos.Add(new datoCombobox() { KeyInfo =2, descInfo= "FUNCION MIN"});
            datos.Add(new datoCombobox() { KeyInfo =3, descInfo = "FUNCION MAX" });
            //PROPIEDADES COMBOBOX
            pBase.Properties.DataSource = datos.ToList();
            pBase.Properties.ValueMember = "KeyInfo";
            pBase.Properties.DisplayMember = "descInfo";

            pBase.Properties.PopulateColumns();
            //SI OCULTAR KEY ES TRUE NO SE DEBE MOSTRAR LA COLUMNA KEY
            
            pBase.Properties.Columns[0].Visible = false;
            
            pBase.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
            pBase.Properties.AutoSearchColumnIndex = 1;
            pBase.Properties.ShowHeader = false;
        }

        private void btnBase_Click(object sender, EventArgs e)
        {
           
            if (txtBase.EditValue.ToString() == "1")
            {
            
                //CARGAR MEMOEDIT CON CADENA BASE
                string bas = "IF[exp>exp](verdadero,falso)";
                this.opener.CargarPlantilla(bas);
                this.Close();
            }
            else if (txtBase.EditValue.ToString() == "2")
            {
                //CARGAR MEMOEDIT CON CADENA BASE
                string bas = "MIN(EXPRESION1;EXPRESION2)";
                this.opener.CargarPlantilla(bas);
                this.Close();
            }
            else if (txtBase.EditValue.ToString() == "3")
            {
                //CARGAR MEMOEDIT CON CADENA BASE
                string bas = "MAX(EXPRESION1;EXPRESION2)";
                this.opener.CargarPlantilla(bas);
                this.Close();
            }
        }
    }
}