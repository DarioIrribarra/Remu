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
    public partial class frmFiltroGenerador : DevExpress.XtraEditors.XtraForm
    {
        /// <summary>
        /// Guada todos los campos de la query creada.
        /// </summary>
        private List<formula> FieldsList { get; set; } = new List<formula>();

        /// <summary>
        /// Nos indica cuantas filas han sido mostradas
        /// </summary>
        private int Rows { get; set; } = 0;

        /// <summary>
        /// Indica la cantidad de elementos que tiene el grupocontrol
        /// </summary>
        private int CountGroup { get; set; } 

        public IGenerador OpenGen { get; set; }

        public frmFiltroGenerador()
        {
            InitializeComponent();
        }

        public frmFiltroGenerador(List<formula> pFields)
        {
            InitializeComponent();
            this.FieldsList = pFields;
        }

        private void frmFiltroGenerador_Load(object sender, EventArgs e)
        {
            CountGroup = groupFiltros.Controls.Count;
            HideTab();

            LoadCombos();

            DefaultFilters();
        }

        /// <summary>
        /// Permite deshabilitar campos del groupbox de acuerdo a su tabindex
        /// </summary>
        private void EnableColumnsByTab()
        {
            //una fila equivale a 8 elementos.
            int FilaMuestra = 0, totalFila = 8;
            int r = 0;

            FilaMuestra = totalFila * Rows;
            
            //Obtenemos la cantidad de controles que estan agrupados dentro del groupbox
            Control.ControlCollection con = groupFiltros.Controls;

            //ORDENAMOS CONTROLES DE MENOR A MAYOR POR TAB
            List<Control> controles =  Reordenar(con);

            if (controles.Count > 0)
            {
                //TotalCol = ColumnCount;

                //Recorremos cada control dentro del groupBox (a la inversa)
                for (int i = 0; i < controles.Count; i++)
                {
                    if (Rows > 1)
                        r = 1;

                    int tab = controles[i].TabIndex;
                    string Controlname = controles[i].Name;

                    if (tab <= FilaMuestra)
                    {
                        //LA FILA QUE CORRESPONDE A ROWS (ULTIMA FILA) NO DEBE MOSTRAR EL COMBO AND OR
                        if (tab <= (totalFila * Rows) && tab >= ((totalFila * Rows) - totalFila) + 1)
                        {
                            //ES CONTROL COMBOUNIR
                            if (Controlname.ToLower().Contains("combounir"))
                            {
                                controles[i].Enabled = false;
                                controles[i].Visible = true;
                            }
                            else
                                controles[i].Visible = true;

                        }
                        else
                        {
                            controles[i].Visible = true;
                            if (controles[i].Enabled == false)
                                controles[i].Enabled = true;
                        }                            
                    }
                    else
                    {
                        controles[i].Visible = false;
                        if (controles[i].Enabled == false)
                            controles[i].Enabled = true;
                    }
                        
                }
            }
        }       

        /// <summary>
        /// Oculta todos los elementos del groupcontrol
        /// </summary>
        private void HideTab()
        {
            
            //DEBEMOS MOSTRAR CUATRO ELEMENTOS
            //Obtenemos la cantidad de controles que estan agrupados dentro del groupbox
            Control.ControlCollection con = groupFiltros.Controls;

            if (con.Count > 0)
            {
                //TotalCol = ColumnCount;

                //Recorremos cada control dentro del groupBox (a la inversa)
                for (int i = 0; i < con.Count; i++)
                {
                    con[i].Visible = false;                    
                }
            }
        }

        /// <summary>
        /// Genera cadena de acuerdo a opciones.
        /// </summary>
        private string GetDataSelected()
        {
            //Obtenemos la cantidad de controles que estan agrupados dentro del groupbox
            Control.ControlCollection con = groupFiltros.Controls;            
            StringBuilder filtroSql = new StringBuilder();
            List<string> Condiciones = new List<string>();
            LookUpEdit ConvertLook = new LookUpEdit();
            TextEdit ConvertText = new TextEdit();
            string cond = "", campo = "", valor = "", union = "";
            bool EntraCond = false, EntraField = false, EntraText = false, EntraUnion = false;
            int Fila = 1;
            int ElxFilas = 8;

            Filtro fil = new Filtro();

            //Limpiamos coleccion de filtros que teniamos en memoria
            Filtro.FilterStack.Clear();

            if (con.Count > 0)
            {
                //REORDENAR CONTROLES POR TAB
                List<Control> Ordenados = new List<Control>();
                Ordenados = Reordenar(con);


                //TotalCol = ColumnCount;

                //Recorremos cada control dentro del groupBox (a la inversa)
                if (Ordenados.Count > 0)
                {
                    for (int i = 0; i < Ordenados.Count; i++)
                    {
                        //Fila = 1;

                        int tab = Ordenados[i].TabIndex;
                        string nameControl = Ordenados[i].Name;
                        string indexFromName = nameControl.Substring(nameControl.Length - 1, 1);

                        //A QUE FILA PERTENECE EL CONTROL???
                        if (Ordenados[i].Visible)
                        {
                            if (tab <= (ElxFilas * Fila) && tab >= ((ElxFilas * Fila) - ElxFilas) + 1)
                            {                            

                                //Estoy dentro de la fila
                                //condicion
                                if (nameControl.ToLower().Contains("combocond"))
                                {
                                    ConvertLook = (LookUpEdit)Ordenados[i];
                                    cond = ConvertLook.EditValue.ToString();
                                    fil.Condicion = cond;
                                    EntraCond = true;
                                    
                                }
                                else if (nameControl.ToLower().Contains("combofield"))
                                {
                                    ConvertLook = (LookUpEdit)Ordenados[i];                                    
                                    campo = ConvertLook.EditValue.ToString();
                                    EntraField = true;
                                    fil.Nombre = campo;
                                }
                                else if (nameControl.ToLower().Contains("textedit"))
                                {
                                    ConvertText = (TextEdit)Ordenados[i];
                                    valor = ConvertText.Text;
                                    EntraText = true;
                                    fil.Valor = valor;
                                }
                                else if (nameControl.ToLower().Contains("combounir"))
                                {                                   
                                    ConvertLook = (LookUpEdit)Ordenados[i];
                                    union = Ordenados[i].Enabled ? ConvertLook.EditValue.ToString() : "";
                                    fil.Union = Ordenados[i].Enabled ? ConvertLook.EditValue.ToString() : "";
                                    EntraUnion = true;

                                    if (Ordenados[i].Enabled)
                                    {
                                        if (ConvertLook.EditValue.ToString() == "")
                                        {
                                            XtraMessageBox.Show("Por favor selecciona un tipo de union", "OR / AND", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                            Ordenados[i].Focus();
                                            return "";
                                        }

                                    }
                                }

                                if (EntraField && EntraCond && EntraText && EntraUnion)
                                {
                                    //Agregamos datos a builder
                                    filtroSql.AppendLine($"{campo} {cond} {valor} {union}");
                                    //Condiciones.Add($"{campo} {cond} {valor}");
                                    EntraCond = false;
                                    EntraField = false;
                                    EntraText = false;
                                    EntraUnion = false;

                                    //Agregamos filtro a coleccion
                                    Filtro.AddFilter(fil);

                                    fil = new Filtro();
                                    
                                }
                            }
                            else
                            {
                                Fila++;
                            }
                        }
                        else
                        {
                            break;
                        }
                    } 
                }             
              
            }

            return filtroSql.ToString();
        }

        /// <summary>
        /// Carga los filtros que esten guardados en memoria
        /// </summary>
        private void DefaultFilters()
        {
            Control.ControlCollection con = groupFiltros.Controls;
            StringBuilder filtroSql = new StringBuilder();
            List<string> Condiciones = new List<string>();
            LookUpEdit ConvertLook = new LookUpEdit();
            TextEdit ConvertText = new TextEdit();
            string cond = "", campo = "", valor = "", union = "";
            bool EntraCond = false, EntraField = false, EntraText = false, EntraUnion = false;
            int Fila = 1;
            int ElxFilas = 8;
            int FilterCount = 0;

            FilterCount = Filtro.FilterStack.Count;

            if (FilterCount == 0)
            {
                btnQuiita.Enabled = false;
                return;
            }

            btnQuiita.Enabled = true;

            //ACTUALIZAMOS VALOR DE VARIABLE ROWS
            Rows = FilterCount;

            if (con.Count > 0)
            {
                //REORDENAR CONTROLES POR TAB
                List<Control> Ordenados = new List<Control>();
                Ordenados = Reordenar(con);

                //TotalCol = ColumnCount;

                //Recorremos cada control dentro del groupBox (a la inversa)
                if (Ordenados.Count > 0)
                {
                    for (int i = 0; i < Ordenados.Count; i++)
                    {
                        //Fila = 1;

                        int tab = Ordenados[i].TabIndex;
                        string nameControl = Ordenados[i].Name;
                        string indexFromName = nameControl.Substring(nameControl.Length - 1, 1);

                        if (tab > (Fila * ElxFilas))
                            Fila++;

                        //Las filas visibles serán igual a la cantidad de filtro guardados en listado.

                        if (tab <= (FilterCount * ElxFilas))
                        {
                            //OBTENEMOS EL REGISTRO CORRESPONDIENTE DE ACUERDO A LA FILA EN LA QUE ESTAMOS
                            if (tab <= (ElxFilas * Fila) && tab >= ((ElxFilas * Fila) - ElxFilas) + 1)
                            {

                                //ESTAMOS EN LA ULTIMA FILA
                                if (tab <= (ElxFilas * FilterCount) && tab >= ((ElxFilas * FilterCount) - ElxFilas) + 1)
                                {
                                    //HABILITAMOS CONTROL
                                    if (nameControl.ToLower().Contains("combounir"))
                                    {
                                        Ordenados[i].Enabled = false;
                                        Ordenados[i].Visible = true;
                                    }
                                    else
                                        Ordenados[i].Visible = true;
                                }
                                else
                                {
                                    Ordenados[i].Visible = true;
                                }                          
 

                                Filtro fil = new Filtro();
                                fil = Filtro.GetFilterFromId(Fila - 1);                                

                                //Estoy dentro de la fila
                                //condicion
                                if (nameControl.ToLower().Contains("combocond"))
                                {                                    
                                    ConvertLook = (LookUpEdit)Ordenados[i];                                    
                                    ConvertLook.EditValue = fil.Condicion;
                                    cond = ConvertLook.EditValue.ToString();

                                }
                                else if (nameControl.ToLower().Contains("combofield"))
                                {
                                    ConvertLook = (LookUpEdit)Ordenados[i];
                                    ConvertLook.EditValue = fil.Nombre;
                                    campo = ConvertLook.EditValue.ToString();
                                }
                                else if (nameControl.ToLower().Contains("textedit"))
                                {
                                    ConvertText = (TextEdit)Ordenados[i];
                                    ConvertText.Text = fil.Valor;
                                    valor = ConvertText.Text;
                                }
                                else if (nameControl.ToLower().Contains("combounir"))
                                {
                                    ConvertLook = (LookUpEdit)Ordenados[i];
                                    ConvertLook.EditValue = fil.Union;
                                    union = Ordenados[i].Enabled ? ConvertLook.EditValue.ToString() : "";
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                                        
                    }
                }
            }
        }
       
        /// <summary>
        /// Entrega los controles ordenados de menor a mayor por tab.
        /// </summary>
        /// <param name="pControls"></param>
        /// <returns></returns>
        private List<Control> Reordenar(Control.ControlCollection pControls)
        {
            var ordenados = from x in pControls.Cast<Control>()
                            orderby x.TabIndex
                            select x;

            return ordenados.ToList();

        }

        /// <summary>
        /// Permite cargar combos con campos consulta sql
        /// </summary>
        private void CargarCombo(LookUpEdit pCombo, List<formula> pDataSource)
        {
            if (this.FieldsList.Count > 0)
            {
                pCombo.Properties.DataSource = pDataSource;

                //PROPIEDADES COMBOBOX
                //pCombo.Properties.DataSource = lista.ToList();
                pCombo.Properties.ValueMember = "key";
                pCombo.Properties.DisplayMember = "desc";

                pCombo.Properties.PopulateColumns();

                //ocultamos la columan key
                pCombo.Properties.Columns[0].Visible = false;

                pCombo.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
                pCombo.Properties.AutoSearchColumnIndex = 1;
                pCombo.Properties.ShowHeader = false;
            }
        }

        private void btnAgrega_Click(object sender, EventArgs e)
        {
            if (Rows < (CountGroup / 8))
            {
                Rows++;
                if (btnQuiita.Enabled == false)
                    btnQuiita.Enabled = true;
            }                
            EnableColumnsByTab();


        }

        private void btnQuiita_Click(object sender, EventArgs e)
        {
            if(Rows >0)
                Rows--;
            EnableColumnsByTab();

            if (Rows == 0)
            {
                Filtro.FilterStack.Clear();
                btnQuiita.Enabled = false;
            }
            else
            {
                btnQuiita.Enabled = true;
            }
                


        }

        /// <summary>
        /// Carga todos los combobox
        /// </summary>
        private void LoadCombos()
        {
            //Fields
            CargarCombo(ComboField1, FieldsList);            
            CargarCombo(ComboField2, FieldsList);
            CargarCombo(ComboField3, FieldsList);
            CargarCombo(ComboField4, FieldsList);
            CargarCombo(ComboField5, FieldsList);
            CargarCombo(ComboField6, FieldsList);
            CargarCombo(ComboField7, FieldsList);
            CargarCombo(ComboField8, FieldsList);
            CargarCombo(ComboField9, FieldsList);
            CargarCombo(ComboField10, FieldsList);

            if (ComboField1.Properties.DataSource != null)
                ComboField1.ItemIndex = 0;
            if (ComboField2.Properties.DataSource != null)
                ComboField2.ItemIndex = 0;
            if (ComboField3.Properties.DataSource != null)
                ComboField3.ItemIndex = 0;
            if (ComboField4.Properties.DataSource != null)
                ComboField4.ItemIndex = 0;
            if (ComboField5.Properties.DataSource != null)
                ComboField5.ItemIndex = 0;
            if (ComboField6.Properties.DataSource != null)
                ComboField6.ItemIndex = 0;
            if (ComboField7.Properties.DataSource != null)
                ComboField7.ItemIndex = 0;
            if (ComboField8.Properties.DataSource != null)
                ComboField8.ItemIndex = 0;
            if (ComboField9.Properties.DataSource != null)
                ComboField9.ItemIndex = 0;
            if (ComboField10.Properties.DataSource != null)
                ComboField10.ItemIndex = 0;

            //Condiciones
            CargarCombo(ComboCond1, Options());
            CargarCombo(ComboCond2, Options());
            CargarCombo(ComboCond3, Options());
            CargarCombo(ComboCond4, Options());
            CargarCombo(ComboCond5, Options());
            CargarCombo(ComboCond6, Options());
            CargarCombo(ComboCond7, Options());
            CargarCombo(ComboCond8, Options());
            CargarCombo(ComboCond9, Options());
            CargarCombo(ComboCond10, Options());

            if (ComboCond1.Properties.DataSource != null)
                ComboCond1.ItemIndex = 0;
            if (ComboCond2.Properties.DataSource != null)
                ComboCond2.ItemIndex = 0;
            if (ComboCond3.Properties.DataSource != null)
                ComboCond3.ItemIndex = 0;
            if (ComboCond4.Properties.DataSource != null)
                ComboCond4.ItemIndex = 0;
            if (ComboCond5.Properties.DataSource != null)
                ComboCond5.ItemIndex = 0;
            if (ComboCond6.Properties.DataSource != null)
                ComboCond6.ItemIndex = 0;
            if (ComboCond7.Properties.DataSource != null)
                ComboCond7.ItemIndex = 0;
            if (ComboCond8.Properties.DataSource != null)
                ComboCond8.ItemIndex = 0;
            if (ComboCond9.Properties.DataSource != null)
                ComboCond9.ItemIndex = 0;
            if (ComboCond10.Properties.DataSource != null)
                ComboCond10.ItemIndex = 0;

            //UNIONES
            CargarCombo(ComboUnir1, OptionsUnion());
            CargarCombo(ComboUnir2, OptionsUnion());
            CargarCombo(ComboUnir3, OptionsUnion());
            CargarCombo(ComboUnir4, OptionsUnion());
            CargarCombo(ComboUnir5, OptionsUnion());
            CargarCombo(ComboUnir6, OptionsUnion());
            CargarCombo(ComboUnir7, OptionsUnion());
            CargarCombo(ComboUnir8, OptionsUnion());
            CargarCombo(ComboUnir9, OptionsUnion());

            if (ComboUnir1.Properties.DataSource != null)
                ComboUnir1.ItemIndex = 0;
            if (ComboUnir2.Properties.DataSource != null)
                ComboUnir2.ItemIndex = 0;
            if (ComboUnir3.Properties.DataSource != null)
                ComboUnir3.ItemIndex = 0;
            if (ComboUnir4.Properties.DataSource != null)
                ComboUnir4.ItemIndex = 0;
            if (ComboUnir5.Properties.DataSource != null)
                ComboUnir5.ItemIndex = 0;
            if (ComboUnir6.Properties.DataSource != null)
                ComboUnir6.ItemIndex = 0;
            if (ComboUnir7.Properties.DataSource != null)
                ComboUnir7.ItemIndex = 0;
            if (ComboUnir8.Properties.DataSource != null)
                ComboUnir8.ItemIndex = 0;
            if (ComboUnir9.Properties.DataSource != null)
                ComboUnir9.ItemIndex = 0;
        }

        /// <summary>
        /// Opciones para combo condiciones.
        /// </summary>
        /// <returns></returns>
        private List<formula> Options()
        {
            List<formula> Opciones = new List<formula>() {
                new formula(){ key = "=", desc = "="},
                new formula(){ key = ">", desc = ">"},
                new formula(){ key = "<", desc = "<"},
                new formula(){ key = ">=", desc = ">="},
                new formula(){ key = "<=", desc = "<="},
                new formula(){ key = "<>", desc = "<>"},
                new formula(){ key = "LIKE", desc = "LIKE"}         
            };

            return Opciones;
        }

        /// <summary>
        /// Para combo union.
        /// </summary>
        /// <returns></returns>
        private List<formula> OptionsUnion()
        {
            List<formula> Opciones = new List<formula>() {
                new formula(){ key = "AND", desc = "AND"},
                new formula(){ key = "OR", desc = "OR"}
            };

            return Opciones;
        }

        private void btnGuardarCambios_Click(object sender, EventArgs e)
        {
            GetDataSelected();          

            if (Filtro.FilterStack.Count > 0)
            {
                if (OpenGen != null)
                {
                    OpenGen.AgregaFiltros();
                    XtraMessageBox.Show("Filtros Agregados correctamente", "Filtro", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Close();
                }
                else
                {
                    XtraMessageBox.Show("No se pudieron guardar los filtros", "Filtro", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    Filtro.FilterStack.Clear();
                }
            }
            else
            {
                XtraMessageBox.Show("No se pudieron guardar los filtros", "Filtro", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                if (OpenGen != null)
                    OpenGen.AgregaFiltros();

               
            }          
        }

        private void groupFiltros_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}