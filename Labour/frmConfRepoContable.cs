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
using DevExpress.Utils.Menu;

namespace Labour
{
    public partial class frmConfRepoContable : DevExpress.XtraEditors.XtraForm
    {
        public frmConfRepoContable()
        {
            InitializeComponent();
        }

        

        //indica si es actualizacion o nuevo registro
        private bool UpdateReg = false;
        
        private void frmConfRepoContable_Load(object sender, EventArgs e)
        {
            Esquema es = new Esquema();
            es.SetInfo(es.GetPredeterminado());

            LoadCombo(c1, es.Col);
            LoadCombo(c2, es.Col);
            LoadCombo(c3, es.Col);
            LoadCombo(c4, es.Col);
            LoadCombo(c5, es.Col);
            LoadCombo(c6, es.Col);
            LoadCombo(c7, es.Col);
            LoadCombo(c8, es.Col);
            LoadComboNumber(num1);
            LoadComboNumber(num2);
            LoadComboNumber(num3);
            LoadComboNumber(num4);
            LoadComboNumber(num5);
            LoadComboNumber(num6);
            LoadComboNumber(num7);
            LoadComboNumber(num8);
            LoadCombo(or1, es.Col);
            LoadCombo(or2, es.Col);
            LoadCombo(or3, es.Col);
            LoadCombo(or4, es.Col);
            LoadCombo(or5, es.Col);
            LoadCombo(or6, es.Col);
            LoadCombo(or7, es.Col);
            LoadCombo(or8, es.Col);

            Cargarinfo();

        }

        #region "DATO"
        private void LoadCombo(LookUpEdit pComboBox, int pMaxColumn)
        {
            List<datoCombobox> Listado = new List<datoCombobox>();

            Listado.Add(new datoCombobox() { descInfo = "No Aplica", KeyInfo = 0});

            for (int i = 1; i <=pMaxColumn ; i++)
            {
                Listado.Add(new datoCombobox() { KeyInfo = i, descInfo = i.ToString() });
            }

            Listado.Add(new datoCombobox() { KeyInfo = pMaxColumn + 1, descInfo = (pMaxColumn +1).ToString()});

            //AGRGAMOS COLUMNA ADICIONAL (POR QUE COLUMNA MONTO (DEBE - CREDITO) 
            //genera una columna adicional 

            pComboBox.Properties.DataSource = Listado.ToList();
            pComboBox.Properties.ValueMember = "KeyInfo";
            pComboBox.Properties.DisplayMember = "descInfo";

            pComboBox.Properties.PopulateColumns();
            //SI OCULTAR KEY ES TRUE NO SE DEBE MOSTRAR LA COLUMNA KEY

            pComboBox.Properties.Columns[0].Visible = false;
            pComboBox.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
            pComboBox.Properties.AutoSearchColumnIndex = 1;
            pComboBox.Properties.ShowHeader = false;

            if (pComboBox.Properties.DataSource != null)
            {
                pComboBox.ItemIndex = 0;
            }

        }

        private void LoadComboNumber(LookUpEdit pComboBox)
        {
            List<datoCombobox> Listado = new List<datoCombobox>();

            Listado.Add(new datoCombobox() { descInfo = "No", KeyInfo = 0 });
            Listado.Add(new datoCombobox() { descInfo = "Si", KeyInfo = 1});            

            pComboBox.Properties.DataSource = Listado.ToList();
            pComboBox.Properties.ValueMember = "KeyInfo";
            pComboBox.Properties.DisplayMember = "descInfo";

            pComboBox.Properties.PopulateColumns();
            //SI OCULTAR KEY ES TRUE NO SE DEBE MOSTRAR LA COLUMNA KEY

            pComboBox.Properties.Columns[0].Visible = false;
            pComboBox.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
            pComboBox.Properties.AutoSearchColumnIndex = 1;
            pComboBox.Properties.ShowHeader = false;

            if (pComboBox.Properties.DataSource != null)
            {
                pComboBox.ItemIndex = 0;
            }

        }

        private void Cargarinfo()
        {
            ReporteContable rep = new ReporteContable();
            rep.SetInfo();

            c1.EditValue = Convert.ToInt32(rep.c1);
            c2.EditValue = Convert.ToInt32(rep.c2);
            c3.EditValue = Convert.ToInt32(rep.c3);
            c4.EditValue = Convert.ToInt32(rep.c4);
            c5.EditValue = Convert.ToInt32(rep.c5);
            c6.EditValue = Convert.ToInt32(rep.c6);
            c7.EditValue = Convert.ToInt32(rep.c7);
            c8.EditValue = Convert.ToInt32(rep.c8);
            n1.Text = rep.n1;
            n2.Text = rep.n2;
            n3.Text = rep.n3;
            n4.Text = rep.n4;
            n5.Text = rep.n5;
            n6.Text = rep.n6;
            n7.Text = rep.n7;
            n8.Text = rep.n8;
            s1.Text = rep.s1;
            s2.Text = rep.s2;
            s3.Text = rep.s3;
            s4.Text = rep.s4;
            s5.Text = rep.s5;
            s6.Text = rep.s6;
            s7.Text = rep.s7;
            s8.Text = rep.s8;
            num1.EditValue = Convert.ToInt32(rep.num1);
            num2.EditValue = Convert.ToInt32(rep.num2);
            num3.EditValue = Convert.ToInt32(rep.num3);
            num4.EditValue = Convert.ToInt32(rep.num4);
            num5.EditValue = Convert.ToInt32(rep.num5);
            num6.EditValue = Convert.ToInt32(rep.num6);
            num7.EditValue = Convert.ToInt32(rep.num7);
            num8.EditValue = Convert.ToInt32(rep.num8);
            or1.EditValue = Convert.ToInt32(rep.or1);
            or2.EditValue = Convert.ToInt32(rep.or2);
            or3.EditValue = Convert.ToInt32(rep.or3);
            or4.EditValue = Convert.ToInt32(rep.or4);
            or5.EditValue = Convert.ToInt32(rep.or5);
            or6.EditValue = Convert.ToInt32(rep.or6);
            or7.EditValue = Convert.ToInt32(rep.or7);
            or8.EditValue = Convert.ToInt32(rep.or8);

        }
      

        #endregion

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            ReporteContable rep = new ReporteContable();

            if (c1.Properties.DataSource == null || c2.Properties.DataSource == null ||
               c3.Properties.DataSource == null || c4.Properties.DataSource == null ||
               c5.Properties.DataSource == null || c6.Properties.DataSource == null ||
               c7.Properties.DataSource == null || c8.Properties.DataSource == null)
            {
                XtraMessageBox.Show("Por favor selecciona una columna válida", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //Error...
            rep.Insert(c1.EditValue.ToString(), c2.EditValue.ToString(), c3.EditValue.ToString(), 
                c4.EditValue.ToString(), c5.EditValue.ToString(), c6.EditValue.ToString(), 
                c7.EditValue.ToString(), c8.EditValue.ToString(), n1.Text, n2.Text, n3.Text, 
                n4.Text, n5.Text, n6.Text, n7.Text, n8.Text, s1.Text, s2.Text, s3.Text, s4.Text, 
                s5.Text, s6.Text, s7.Text, s8.Text, Convert.ToInt32(num1.EditValue),
                Convert.ToInt32(num2.EditValue), Convert.ToInt32(num3.EditValue),
                Convert.ToInt32(num4.EditValue), Convert.ToInt32(num5.EditValue),
                Convert.ToInt32(num6.EditValue), Convert.ToInt32(num7.EditValue),
                Convert.ToInt32(num8.EditValue), Convert.ToInt32(or1.EditValue),
                Convert.ToInt32(or2.EditValue), Convert.ToInt32(or3.EditValue),
                Convert.ToInt32(or4.EditValue), Convert.ToInt32(or5.EditValue),
                Convert.ToInt32(or6.EditValue), Convert.ToInt32(or7.EditValue),
                Convert.ToInt32(or8.EditValue)
                );

            Cargarinfo();
        }       

        
        private void viewReporte_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad(); 

            DXPopupMenu menu = e.Menu;
            string nombre = "";
            if (menu != null)
            {
                //if (viewMaestro.RowCount > 0) nombre = (string)viewMaestro.GetFocusedDataRow()["descCuenta"];

                DXMenuItem submenu = new DXMenuItem("Configurar Columnas", new EventHandler(ConfiguraCol_Click));
                submenu.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("images/actions/show_16x16.png");
                menu.Items.Clear();
                menu.Items.Add(submenu);
            }
        }

        private void ConfiguraCol_Click(object sender, EventArgs e)
        {
            //Abrimos formulario correspondiente
            Sesion.NuevaActividad();
          

        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            Close();
        }

        private void labelControl7_Click(object sender, EventArgs e)
        {

        }
    }
}