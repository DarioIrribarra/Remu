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
using DevExpress.XtraEditors.Registrator;
using System.Data.SqlClient;
using System.Globalization;

namespace Labour
{
    public partial class frmElementosContables : DevExpress.XtraEditors.XtraForm
    {
        /// <summary>
        /// Codigo de la cuenta maestra a la cual se quieren agregar los elementos.
        /// </summary>
        public int CuentaMaestra { get; set; }
        public string NombreCuenta { get; set; } = "";       
        /// <summary>
        /// Cantidad de columnas que tiene el esquema contable al que pertenecen los elementos.
        /// </summary>
        public int ColumnCount { get; set; } = 0;
        public bool IsNumber { get; set; } = false;
        public bool IsText { get; set; } = false;

        public int EsquemaAsociado { get; set; } = 0;

        /// <summary>
        /// Generar sql para insert de elementos.
        /// </summary>
        public string SqlInsert { get; set; } = "";

        //public string MyProperty { get; set; } = "SELECT column_name FROM INFORMATION_SCHEMA.COLUMNS " +
        //                                         "WHERE table_name = @pTableName ORDER BY column_name";

        private string Fecha1 = "";
        private string Fecha2 = "";
        private string Fecha3 = "";
        private string Fecha4 = "";
        private string Fecha5 = "";
        private string Fecha6 = "";
        private string Fecha7 = "";
        private string Fecha8 = "";
        private string Fecha9 = "";
        private string Fecha10 = "";
        private string Fecha11 = "";
        private string Fecha12 = "";
        private string Fecha13 = "";
        private string Fecha14 = "";
        private string Fecha15 = "";
        private string Fecha16 = "";
        private string Fecha17 = "";
        private string Fecha18 = "";
        private string Fecha19 = "";
        private string Fecha20 = "";

        public frmElementosContables()
        {
            InitializeComponent();
        }

        public frmElementosContables(int pCuenta, string pNombreCuenta, Esquema pEsquema)
        {
            InitializeComponent();
            CuentaMaestra = pCuenta;
            NombreCuenta = pNombreCuenta;
            ColumnCount = pEsquema.Col;
            this.Text = $"Elementos Maestro {pNombreCuenta}";
            groupColumnas.Text = $"Columnas Disponibles: {ColumnCount}";
            lblTitle.Text = $"CONFIGURE LOS ELEMENTOS PARA LA CUENTA '{NombreCuenta}'";
            EsquemaAsociado = pEsquema.Cod;
           
        }
        private void frmElementosContables_Load(object sender, EventArgs e)
        {            
            //Obtenemos todos los elementos que tiene nuestro groupbox
            Control.ControlCollection con = groupColumnas.Controls;
            //ColumnCount = groupColumnas.Controls.Count;
            string sql = $"SELECT codElemento, descElemento, tipoSel, subTipoSel, valorSel, subsubtiposel FROM detallecuenta WHERE cuenta={CuentaMaestra} ORDER BY codElemento";
            fnSistema.spllenaGridView(gridElementos, sql);            
            if (viewElementos.RowCount > 0)
            {
                fnSistema.spOpcionesGrilla(viewElementos);
                viewElementos.OptionsCustomization.AllowSort = false;
                ColumnasGrilla();
            }

            fnSeleccion(ComboOption1);
            if (ComboOption1.Properties.DataSource != null)
                ComboOption1.ItemIndex = 0;
            fnSeleccion(ComboOption2);
            if (ComboOption2.Properties.DataSource != null)
                ComboOption2.ItemIndex = 0;
            fnSeleccion(ComboOption3);
            if (ComboOption3.Properties.DataSource != null)
                ComboOption3.ItemIndex = 0;
            fnSeleccion(ComboOption4);
            if (ComboOption4.Properties.DataSource != null)
                ComboOption4.ItemIndex = 0;
            fnSeleccion(ComboOption5);
            if (ComboOption5.Properties.DataSource != null)
                ComboOption5.ItemIndex = 0;
            fnSeleccion(ComboOption6);
            if (ComboOption6.Properties.DataSource != null)
                ComboOption6.ItemIndex = 0;
            fnSeleccion(ComboOption7);
            if (ComboOption7.Properties.DataSource != null)
                ComboOption7.ItemIndex = 0;
            fnSeleccion(ComboOption8);
            if (ComboOption8.Properties.DataSource != null)
                ComboOption8.ItemIndex = 0;
            fnSeleccion(ComboOption9);
            if (ComboOption9.Properties.DataSource != null)
                ComboOption9.ItemIndex = 0;
            fnSeleccion(ComboOption10);
            if (ComboOption10.Properties.DataSource != null)
                ComboOption10.ItemIndex = 0;
            fnSeleccion(ComboOption11);
            if (ComboOption11.Properties.DataSource != null)
                ComboOption11.ItemIndex = 0;
            fnSeleccion(ComboOption12);
            if (ComboOption12.Properties.DataSource != null)
                ComboOption12.ItemIndex = 0;
            fnSeleccion(ComboOption13);
            if (ComboOption13.Properties.DataSource != null)
                ComboOption13.ItemIndex = 0;
            fnSeleccion(ComboOption14);
            if (ComboOption14.Properties.DataSource != null)
                ComboOption14.ItemIndex = 0;
            fnSeleccion(ComboOption15);
            if (ComboOption15.Properties.DataSource != null)
                ComboOption15.ItemIndex = 0;
            fnSeleccion(ComboOption16);
            if (ComboOption16.Properties.DataSource != null)
                ComboOption16.ItemIndex = 0;
            fnSeleccion(ComboOption17);
            if (ComboOption17.Properties.DataSource != null)
                ComboOption17.ItemIndex = 0;
            fnSeleccion(ComboOption18);
            if (ComboOption18.Properties.DataSource != null)
                ComboOption18.ItemIndex = 0;
            fnSeleccion(ComboOption19);
            if (ComboOption19.Properties.DataSource != null)
                ComboOption19.ItemIndex = 0;
            fnSeleccion(ComboOption20);
            if (ComboOption20.Properties.DataSource != null)
                ComboOption20.ItemIndex = 0;
            fnSeleccion(ComboOption21);
            if (ComboOption21.Properties.DataSource != null)
                ComboOption21.ItemIndex = 0;
            fnSeleccion(ComboOption22);
            if (ComboOption22.Properties.DataSource != null)
                ComboOption22.ItemIndex = 0;
            fnSeleccion(ComboOption23);
            if (ComboOption23.Properties.DataSource != null)
                ComboOption23.ItemIndex = 0;
            fnSeleccion(ComboOption24);
            if (ComboOption24.Properties.DataSource != null)
                ComboOption24.ItemIndex = 0;
            fnSeleccion(ComboOption25);
            if (ComboOption25.Properties.DataSource != null)
                ComboOption25.ItemIndex = 0;

            //EnableColumns();
            EnableColumnsByTab();

            CargaDatos();

            //if (ComboSubOption1.Properties.DataSource != null)
            //    ComboSubOption1.ItemIndex = 0;

            //if (ComboSubSubOption1.Properties.DataSource != null)
            //    ComboSubSubOption1.ItemIndex = 0;
        }

        /// <summary>
        /// Permite cambiar el tipo de control
        /// </summary>
        /// <param name="EditorTypeName"></param>
        /// <param name="sourceEditor"></param>
        private void ConvertEditor(string EditorTypeName, LookUpEdit sourceEditor, int pOption)
        {
            //EditorClassInfo info = EditorRegistrationInfo.Default.Editors[EditorTypeName];
            //if (info == null) return ;

            //BaseEdit edit = info.CreateEditor();
            //edit.Location = sourceEditor.Location;
            //edit.Size = sourceEditor.Size;
            //edit.Parent = sourceEditor.Parent;
            //edit.TabIndex = sourceEditor.TabIndex;
            //edit.Name = sourceEditor.Name;
            //edit.Properties.Assign(sourceEditor.Properties);

            ////sourceEditor.Dispose();            
            ////sourceEditor = null;
            //edit.BringToFront();
            //edit.Focus();
            Relacion rel = new Relacion();

            try
            {
                //sourceEditor = new LookUpEdit();
                sourceEditor.Properties.DataSource = null;
                sourceEditor.Properties.ValueMember = "";
                sourceEditor.Properties.DisplayMember = "";
                sourceEditor.Properties.Columns.Clear();

                //DATO FIJO (TEXTO - NUMERO - FECHA)
                if (pOption == 1)
                {
                    List<datoCombobox> Listado = new List<datoCombobox>();
                    Listado.Add(new datoCombobox() { KeyInfo = 1, descInfo = "Texto" });
                    Listado.Add(new datoCombobox() { KeyInfo = 2, descInfo = "Numero" });
                    Listado.Add(new datoCombobox() { KeyInfo = 3, descInfo = "Fecha" });

                    if (Listado.Count > 0)
                    {
                        sourceEditor.Properties.DataSource = Listado.ToList();
                        SetPropertiesCombo(sourceEditor, "KeyInfo", "descInfo");
                    }

                    //ComboSubOptions(sourceEditor, Listado);
                    
                }
                //DATOS DEL EMPLEADO
                else if (pOption == 2)
                {
                    List<formula> List = new List<formula>();
                    List = GetColumnsEmpleado(sourceEditor, "trabajador");
                    if (List.Count > 0)
                    {
                        sourceEditor.Properties.DataSource = List.ToList();                        

                        SetPropertiesCombo(sourceEditor, "key", "desc");
                    }
                }
                //DATOS DEL ITEM (PUEDE VENIR DE TABLA ITEM - GRUPO CONTABLE - ITEMTRABAJADOR)
                else if (pOption == 3)
                {
                    List<formula> List = new List<formula>();
                    List = rel.GetColumns("item", "grupocontable", "itemtrabajador");
                    if (List.Count > 0)
                    {
                        sourceEditor.Properties.DataSource = List.ToList();
                        SetPropertiesCombo(sourceEditor, "key", "desc");
                    }
                }
                //DATO DE ENTIDAD
                else if (pOption == 4)
                {
                    List<datoCombobox> Listado = new List<datoCombobox>();
                    Listado.Add(new datoCombobox() { KeyInfo = 1, descInfo = "ISAPRE" });
                    Listado.Add(new datoCombobox() { KeyInfo = 2, descInfo = "AFP" });
                    Listado.Add(new datoCombobox() { KeyInfo = 3, descInfo = "MUTUAL" });
                    Listado.Add(new datoCombobox() { KeyInfo = 4, descInfo = "CAJA" });

                    if (Listado.Count > 0)
                    {
                        sourceEditor.Properties.DataSource = Listado.ToList();
                        SetPropertiesCombo(sourceEditor, "KeyInfo", "descInfo");
                    }

                    //ComboSubOptions(sourceEditor, Listado);
                }
            }
            catch (Exception ex)
            {
                // error..
            }
        }

        /// <summary>
        /// Para el segundo tercer combobox (DE ARRIBA A HACIA ABAJO)
        /// </summary>
        /// <param name="EditorTypeName"></param>
        /// <param name="sourceEditor"></param>
        /// <param name="pOption"></param>
        private void ConvertEditorCalendar(string EditorTypeName, BaseEdit sourceEditor, string pType)
        {            
            EditorClassInfo info = EditorRegistrationInfo.Default.Editors[EditorTypeName];
            if (info == null) return;

            BaseEdit edit = info.CreateEditor();
            edit.Location = sourceEditor.Location;
            edit.Size = sourceEditor.Size;
            edit.Parent = sourceEditor.Parent;
            edit.TabIndex = sourceEditor.TabIndex;
            edit.Name = sourceEditor.Name;
            edit.Properties.Assign(sourceEditor.Properties);            

            //sourceEditor.Dispose();            
            //sourceEditor = null;
            edit.BringToFront();
            edit.Focus();

            CultureInfo Culture = new CultureInfo("en-US");
            DateEdit dt = new DateEdit();
            dt = (DateEdit)edit;
            dt.Properties.Mask.Culture = Culture;
            dt.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTime;
            if(pType == "dd-mm-yyyy")
                dt.Properties.Mask.EditMask = "dd-MM-yyyy";
            else
                dt.Properties.Mask.EditMask = "yyyy-MM-dd";
            dt.Properties.Mask.UseMaskAsDisplayFormat = true;

            DateOption(dt.Name, dt.Text);
            dt.SelectionChanged += Dt_SelectionChanged;            
            
        }

        /// <summary>
        /// Evento que se lanza si se cambia la fecha en el calendario
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Dt_SelectionChanged(object sender, EventArgs e)
        {
            //Para saber desde que control se está invocando el evento.
            DateEdit control = sender as DateEdit;
            string name = control.Name;
            string dataoriginal = "";
            dataoriginal = control.Text;
            DateOption(name, dataoriginal);
           
        }

        /// <summary>
        /// Para guardar la fecha correspondiente del calendario
        /// </summary>
        /// <param name="pNameControl"></param>
        /// <param name="pValue"></param>
        private void DateOption(string pNameControl, string pValue)
        {
            if (pValue == null) return;

            switch (pNameControl)
            {
                case "caja1":
                    Fecha1 = pValue;
                    break;
                case "caja2":
                    Fecha2 = pValue;
                    break;
                case "caja3":
                    Fecha3 = pValue;
                    break;
                case "caja4":
                    Fecha4 = pValue;
                    break;
                case "caja5":
                    Fecha5 = pValue;
                    break;
                case "caja6":
                    Fecha6 = pValue;
                    break;
                case "caja7":
                    Fecha7 = pValue;
                    break;
                case "caja8":
                    Fecha8 = pValue;
                    break;
                case "caja9":
                    Fecha9 = pValue;
                    break;
                case "caja10":
                    Fecha10 = pValue;
                    break;
                case "caja11":
                    Fecha11 = pValue;
                    break;
                case "caja12":
                    Fecha12 = pValue;
                    break;
                case "caja13":
                    Fecha13 = pValue;
                    break;
                case "caja14":
                    Fecha14 = pValue;
                    break;
                case "caja15":
                    Fecha15 = pValue;
                    break;
                case "caja16":
                    Fecha16 = pValue;
                    break;
                case "caja17":
                    Fecha17 = pValue;
                    break;
                case "caja18":
                    Fecha18 = pValue;
                    break;
                case "caja19":
                    Fecha19 = pValue;
                    break;
                case "caja20":
                    Fecha20 = pValue;
                    break;

                default:
                    break;
            }
        }

        private void ConvertTextEdit(string EditorTypeName, BaseEdit sourceEditor)
        {
            EditorClassInfo info = EditorRegistrationInfo.Default.Editors[EditorTypeName];
            if (info == null) return;

            BaseEdit edit = info.CreateEditor();
            edit.Location = sourceEditor.Location;
            edit.Size = sourceEditor.Size;
            edit.Parent = sourceEditor.Parent;
            edit.TabIndex = sourceEditor.TabIndex;
            edit.Name = sourceEditor.Name;
            edit.Properties.Assign(sourceEditor.Properties);
        
            edit.BringToFront();
            edit.Focus();
        }

        /// <summary>
        /// Combo para gargar el numero de columnas disponibles
        /// </summary>
        /// <param name="pCombo"></param>
        private void fnColumnas(LookUpEdit pCombo)
        {
            //instanciamos a la clase combo
            List<datoCombobox> lista = new List<datoCombobox>();
            int i = 1;
            while (i <= 8)
            {
                lista.Add(new datoCombobox() { descInfo = "" + i, KeyInfo = i });
                i++;
            }
            //PROPIEDADES COMBOBOX
            pCombo.Properties.DataSource = lista.ToList();
            pCombo.Properties.ValueMember = "KeyInfo";
            pCombo.Properties.DisplayMember = "descInfo";

            pCombo.Properties.PopulateColumns();

            //ocultamos la columan key
            pCombo.Properties.Columns[0].Visible = false;

            pCombo.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
            pCombo.Properties.AutoSearchColumnIndex = 1;
            pCombo.Properties.ShowHeader = false;

            if (pCombo.Properties.DataSource != null)
                pCombo.ItemIndex = 0;
        }

        private void ColumnasGrilla()
        {
            if (viewElementos.RowCount > 0)
            {
                viewElementos.Columns[0].Caption = "#";
                viewElementos.Columns[0].Width = 10;
                
                viewElementos.Columns[1].Caption = "Descripción";
                //TIPO SELECCIONADO
                viewElementos.Columns[2].Caption = "SubDato";
                //SUB TIPO SELECCIONADO
                viewElementos.Columns[3].Caption = "subTipoSel";
                //VALOR SELECCIONADO               
                viewElementos.Columns[4].Caption = "Dato";

                //tipoSel, subTipoSel, valorSel
            }
        }

        /// <summary>
        /// Combo para gargar el numero de columnas disponibles
        /// </summary>
        /// <param name="pCombo"></param>
        private void fnSeleccion(LookUpEdit pCombo)
        {
            //instanciamos a la clase combo
            List<datoCombobox> lista = new List<datoCombobox>();
           
            lista.Add(new datoCombobox() { descInfo = "Dato Fijo", KeyInfo = 1 });
            lista.Add(new datoCombobox() { descInfo = "Dato Empleado", KeyInfo = 2 });
            lista.Add(new datoCombobox() { descInfo = "Dato Item", KeyInfo = 3 });
            lista.Add(new datoCombobox() { descInfo = "Dato Entidad", KeyInfo = 4 });

            //PROPIEDADES COMBOBOX
            pCombo.Properties.DataSource = lista.ToList();
            pCombo.Properties.ValueMember = "KeyInfo";
            pCombo.Properties.DisplayMember = "descInfo";

            pCombo.Properties.PopulateColumns();

            //ocultamos la columan key
            pCombo.Properties.Columns[0].Visible = false;

            pCombo.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
            pCombo.Properties.AutoSearchColumnIndex = 1;
            pCombo.Properties.ShowHeader = false;

            //if (pCombo.Properties.DataSource != null)
            //    pCombo.ItemIndex = 0;
        }

        /// <summary>
        ///indica como se debe comportar el combobox dependiente del tipo de seleccion realizada
        ///1- Dato fijo; Puede ser una fecha, un numero, un texto.
        ///2- Datos del Empleado: Puede ser cualquier campo de la tabla trabajador
        ///Si se selecciona un campo que viene de otra tabla debemos desplegar un segundo combo seleccion 
        ///de acuerdo a subseleccion realizada.
        ///3- Dato item: puede ser cualquier dato de la tabla item.
        ///4- Dato Entidad: Mostrar subcombo seleccion con las siguientes alternativas: ISAPRE, 
        ///AFP (PUEDE SER CAJA PREVISION O AFP), MUTUA, CAJA COMPENSACION.        
        /// </summary>
        //private void Opciones(int pCodOption, BaseEdit pControl)
        //{
        //    List<datoCombobox> Listado = new List<datoCombobox>();

        //    switch (pCodOption)
        //    {
        //        //DATO FIJO???
        //        case 1:
        //            //CAMBIAR CAJA DE TEXTO A LOOOKUPEDIT
        //            ConvertEditor("LookUpEdit", pControl);             

        //            break;
        //        //DATO EMPLEADO??
        //        case 2:
        //            //Generamos listado con nombre de las columnas de la tabla empleados.
        //            break;
        //        case 3:
        //            break;
        //        case 4:
        //            break;

        //        default:
        //            break;
        //    }
        //}

        /// <summary>
        /// Generamos listado con todas las columnas de tabla empleado (Para comboseleccion)
        /// </summary>
        private List<formula> GetColumnsEmpleado(LookUpEdit pCombo, string pTableName)
        {
            string sql = "SELECT column_name FROM INFORMATION_SCHEMA.COLUMNS " +
                         "WHERE table_name = @pTableName ORDER BY column_name";

            List<formula> fr = new List<formula>();
            SqlCommand cmd;
            SqlConnection cn;
            SqlDataReader rd;

            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            cmd.Parameters.Add(new SqlParameter("@pTableName", pTableName));

                            rd = cmd.ExecuteReader();
                            if (rd.HasRows)
                            {
                                while (rd.Read())
                                {
                                    //LLENAMOS LISTADO
                                    fr.Add(new formula() {key = (string)rd["column_name"], desc = (string)rd["column_name"] });
                                }
                            }
                            cmd.Dispose();
                            rd.Close();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {                 
                //ERROR...
            }

            return fr;

            
        }

        /// <summary>
        /// Llena un lookupedit de acuerdo a un listado
        /// </summary>
        /// <param name="pCombo"></param>
        /// <param name="pListado"></param>
        private void ComboSubOptions(LookUpEdit pCombo, List<datoCombobox> pListado)
        {
            
            //PROPIEDADES COMBOBOX
            pCombo.Properties.DataSource = pListado.ToList();
            pCombo.Properties.ValueMember = "KeyInfo";
            pCombo.Properties.DisplayMember = "descInfo";

            pCombo.Properties.PopulateColumns();

            //ocultamos la columan key
            pCombo.Properties.Columns[0].Visible = false;

            pCombo.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
            pCombo.Properties.AutoSearchColumnIndex = 1;
            pCombo.Properties.ShowHeader = false;

            if (pCombo.Properties.DataSource != null)
                pCombo.ItemIndex = 0;
        }

        private void SetPropertiesCombo(LookUpEdit pCombo, string pValueMember, string pDisplayMember)
        {
            //PROPIEDADES COMBOBOX            
            pCombo.Properties.ValueMember = pValueMember;
            pCombo.Properties.DisplayMember = pDisplayMember;

            pCombo.Properties.PopulateColumns();

            //ocultamos la columan key
            pCombo.Properties.Columns[0].Visible = false;

            pCombo.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
            pCombo.Properties.AutoSearchColumnIndex = 1;
            pCombo.Properties.ShowHeader = false;

            //if (pCombo.Properties.DataSource != null)
            //    pCombo.ItemIndex = 0;
        }

        private void EnableColumns()
        {
            int col = 0, TotalCol = 0;
            //Representa cada grupo de 3 elementos (columnas)
            col = (ColumnCount * 5);           

            //Obtenemos la cantidad de controles que estan agrupados dentro del groupbox
            Control.ControlCollection con = groupColumnas.Controls;

            if (con.Count > 0)
            {
                //TotalCol = ColumnCount;

                //Recorremos cada control dentro del groupBox (a la inversa)
                for (int i = con.Count - 1; i >= 0; i--)
                {
                    //Habilitamos control
                    if (col > 0)
                    {
                        con[i].Enabled = true;
                    }
                    //Deshabilitamos control
                    else
                    {
                        con[i].Enabled = false;
                    }

                    col--;
                }
            }
        }

        /// <summary>
        /// Permite deshabilitar campos del groupbox de acuerdo a su tabindex
        /// </summary>
        private void EnableColumnsByTab()
        {
            int col = 0, TotalCol = 0;
            //Representa cada grupo de 3 elementos (columnas)
            col = (ColumnCount * 6);

            //Obtenemos la cantidad de controles que estan agrupados dentro del groupbox
            Control.ControlCollection con = groupColumnas.Controls;

            if (con.Count > 0)
            {
                //TotalCol = ColumnCount;

                //Recorremos cada control dentro del groupBox (a la inversa)
                for (int i = 0; i < con.Count; i++)
                {
                    int tab = con[i].TabIndex;

                    if (tab <= col)
                        con[i].Visible = true;
                    else
                        con[i].Visible = false;                                     
                }
            }
        }

        /// <summary>
        /// Validacion en cascada de cada grupo columna.
        /// </summary>
        /// <param name="pComboPrincipal"></param>
        /// <param name="pComboSecundario"></param>
        /// <param name="pComboTercero"></param>
        /// <returns></returns>
        private bool Validation(LookUpEdit pComboPrincipal, TextEdit pComboSecundario, LookUpEdit pComboTercero, TextEdit pCaja, int pIndex, TextEdit pDesc)
        {
            int OptionComboPrincipal = 0, OptionSegundoCombo = 0;
            OptionComboPrincipal = Convert.ToInt32(pComboPrincipal.EditValue);

            int tipoSel = 0;
            string valorSel = "", SubTipoSel = "", NameCaja = "", subsubSel = "";

            Relacion rel = new Relacion();
            try
            {
                if (pComboPrincipal.Properties.DataSource == null || pComboPrincipal.EditValue == null)
                    return false;

                //Guardamos el tipo de seleccion del combo principal.
                tipoSel = OptionComboPrincipal;                

                //TEXTO FIJO
                if (OptionComboPrincipal == 1)
                {
                    
                    if (pComboSecundario.EditValue == null)
                        return false;

                    //NUMERO - TEXTO - FECHA
                    OptionSegundoCombo = Convert.ToInt32(pComboSecundario.EditValue);
                    if (OptionSegundoCombo == 3)
                    {
                        //FECHA
                        if (pComboTercero.Properties.DataSource == null || pComboTercero.EditValue == null)
                            return false;

                        valorSel = GetFecha(pCaja.Name);
                        SubTipoSel = pComboSecundario.EditValue + "";
                        subsubSel = pComboTercero.EditValue + "";
                    }
                    else if (OptionSegundoCombo == 2)
                    {
                        //NUMERO
                        //Numero incorrecto.
                        if (pCaja.Text != "")
                        {
                            if (fnSistema.IsDecimal(pCaja.Text) == false)
                                return false;
                        }
                       

                        //Guardamos valor
                        valorSel = (string)pCaja.Text;
                        SubTipoSel = Convert.ToInt32(pComboSecundario.EditValue) + "";
                    }
                    else if (OptionSegundoCombo == 1)
                    {
                        //TEXTO
                        //TEXTO EN BLANCO???

                        valorSel = (string)pCaja.Text;
                        SubTipoSel = Convert.ToInt32(pComboSecundario.EditValue) + "";
                    }
                }
                //DATA EMPLEADO?
                else if (OptionComboPrincipal == 2)
                {
                    //...
                    if (pComboSecundario.EditValue == null)
                        return false;

                    //DEPENDE SI ES UN CAMPO FIJO O ES UNA CLAVE FORANEA
                    valorSel = (string)pComboSecundario.EditValue;
                    List<Relacion> relaciones = rel.GetRelaciones("trabajador");
                    if (rel.EsForaneo(relaciones, valorSel))
                    {                        
                        valorSel = (string)pComboTercero.EditValue;
                    }
                    else
                    {
                        valorSel = (string)pComboSecundario.EditValue;
                    }

                    SubTipoSel = (string)pComboSecundario.EditValue;
                }
                //DATA ITEM
                else if (OptionComboPrincipal == 3)
                {
                    //...
                    if (pComboSecundario.EditValue == null)
                        return false;

                    valorSel = (string)pComboSecundario.EditValue;
                }
                //ENTIDAD
                else if (OptionComboPrincipal == 4)
                {
                    //...
                    if (pComboSecundario.EditValue == null)
                        return false;

                    SubTipoSel = pComboSecundario.EditValue + "";
                    valorSel = (string)pComboTercero.EditValue;
                }

                //GENERAMOS CADENA COMPLETA DE INSERT
                SqlInsert = SqlInsert + $"INSERT INTO detallecuenta(cuenta, codElemento, descElemento, codEs, tipoSel, valorSel, subtiposel, subsubtiposel) VALUES({CuentaMaestra}, {pIndex}, '{pDesc.Text}',{1}, {tipoSel},'{valorSel}', '{SubTipoSel}', '{subsubSel}')\n";
                
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;

        }

        private void CargaDatos()
        {
            int TipoSel = 0;
            string SubTipo = "";
            string SubSubTipo = "";
            string ValSel = "";
            string Number = "";
            string Desc = "";
            Relacion rel = new Relacion();
            string NameComboOption = "";
            LookUpEdit pComboPrincipal = new LookUpEdit(); ;
            LookUpEdit pComboSecundario = new LookUpEdit();
            LookUpEdit pComboTercero = new LookUpEdit();
            TextEdit pCaja = new TextEdit();
            //caja descripcion
            TextEdit pCajaDescripcion = new TextEdit();

            try
            {
                if (viewElementos.RowCount > 0)
                {
                    //OBTENEMOS LOS VALORES POR CADA FILA
                    for (int i = 0; i < viewElementos.RowCount; i++)
                    {
                        //PONEMOS EL FOCO EN LA FILA CORRESPONDIENTE...
                        //viewElementos.FocusedRowHandle = i;
                        //OBTENEMOS VALORES DE LA FILA
                        //TipoSel = Convert.ToInt32(viewElementos.GetFocusedDataRow()["tipoSel"]);
                        //SubTipo = (string)(viewElementos.GetFocusedDataRow()["subTipoSel"]);
                        //ValSel = (string)viewElementos.GetFocusedDataRow()["valorSel"];

                        TipoSel = Convert.ToInt32(viewElementos.GetRowCellValue(i, "tipoSel"));
                        SubTipo = (string)(viewElementos.GetRowCellValue(i, "subTipoSel"));
                        SubSubTipo = (string)(viewElementos.GetRowCellValue(i, "subsubtiposel"));
                        ValSel = (string)(viewElementos.GetRowCellValue(i, "valorSel"));
                        Desc = (string)(viewElementos.GetRowCellValue(i, "descElemento"));

                        Control.ControlCollection Controles = groupColumnas.Controls;
                        if (Controles.Count > 0)
                        {
                            pComboPrincipal = Controles.Find("ComboOption" + (i+1), true).FirstOrDefault() as LookUpEdit;
                            pComboSecundario = Controles.Find("ComboSubOption" + (i+1), true).FirstOrDefault() as LookUpEdit;
                            pComboTercero = Controles.Find("ComboSubSubOption" + (i+1), true).FirstOrDefault() as LookUpEdit;
                            pCaja = Controles.Find("caja" + (i+1), true).FirstOrDefault() as TextEdit;
                            pCajaDescripcion = Controles.Find("txtDesc" + (i+1), true).FirstOrDefault() as TextEdit;

                            //SETEAMOS DESCRIPCION
                            pCajaDescripcion.Text = Desc;

                            //SE SELECCIONÓ VALOR FIJO
                            if (TipoSel == 1)
                            {
                                //SELECCIONAMOS EL COMBOBOX 
                                pComboPrincipal.EditValue = TipoSel;

                                //SUBTIPO SELECCIONADO?
                                pComboSecundario.EditValue = Convert.ToInt32(SubTipo);

                                //Texto
                                if (SubTipo == "1")
                                {
                                    pCaja.Text = ValSel;
                                }
                                //numero
                                else if (SubTipo == "2")
                                {
                                    pCaja.Text = ValSel;
                                }
                                //FECHA
                                else if (SubTipo == "3")
                                {
                                    //pComboSecundario.EditValue = "3";
                                    pComboTercero.EditValue = SubSubTipo;
                                    pCaja.BringToFront();
                                    pCaja.Text = ValSel;
                                }

                            }
                            //DATOS DE EMPLEADO
                            else if (TipoSel == 2)
                            {
                                pComboPrincipal.EditValue = TipoSel;

                                //SUBTIPO SELECCIONADO?
                                pComboSecundario.EditValue = SubTipo;

                                //VALOR ES FORANEO
                                List<Relacion> relaciones = rel.GetRelaciones("trabajador");
                                if (relaciones.Count > 0)
                                {
                                    //SI ES FORANEA CARGAMOS TERCER COMBO
                                    if (rel.EsForaneo(relaciones, SubTipo))
                                        pComboTercero.EditValue = ValSel;

                                }
                            }
                            //DATOS DE ITEM
                            else if (TipoSel == 3)
                            {
                                pComboPrincipal.EditValue = TipoSel;
                                pComboSecundario.EditValue = ValSel;

                                //SUBTIPO SELECCIONADO?
                                //pComboTercero.EditValue = SubTipo;
                            }
                            //DATOS DE ENTIDAD
                            else if (TipoSel == 4)
                            {
                                
                                pComboPrincipal.EditValue = TipoSel;

                                //SUBTIPO SELECCIONADO?
                                pComboSecundario.EditValue = Convert.ToInt32(SubTipo);

                                pComboTercero.EditValue = ValSel;
                            }
                        }                       
                    }

                    
                }          

            }
            catch (Exception ex)
            {
                return;
            }
        }

        private string GetFecha(string pNameControl)
        {
            string value = "";
            switch (pNameControl)
            {
                case "caja1":
                    value = Fecha1;
                    break;
                case "caja2":
                    value = Fecha2;
                    break;
                case "caja3":
                    value = Fecha3;
                    break;
                case "caja4":
                    value = Fecha4;
                    break;
                case "caja5":
                    value = Fecha5;
                    break;
                case "caja6":
                    value = Fecha6;
                    break;
                case "caja7":
                    value = Fecha7;
                    break;
                case "caja8":
                    value = Fecha8;
                    break;
                case "caja9":
                    value = Fecha9;
                    break;
                case "caja10":
                    value = Fecha10;
                    break;
                case "caja11":
                    value = Fecha11;
                    break;
                case "caja12":
                    value = Fecha12;
                    break;
                case "caja13":
                    value = Fecha13;
                    break;
                case "caja14":
                    value = Fecha14;
                    break;
                case "caja15":
                    value = Fecha15;
                    break;
                case "caja16":
                    value = Fecha16;
                    break;
                case "caja17":
                    value = Fecha17;
                    break;
                case "caja18":
                    value = Fecha18;
                    break;
                case "caja19":
                    value = Fecha19;
                    break;
                case "caja20":
                    value = Fecha20;
                    break;

                default:
                    break;
            }

            return value;
        }

        private void SaveData(string pSql)
        {
            string sqlGrilla = $"SELECT codElemento, descElemento, tipoSel, subTipoSel, valorSel, subsubtiposel FROM detallecuenta WHERE cuenta={CuentaMaestra} ORDER BY codElemento";

            string sql2 = "BEGIN TRY " +
                       "BEGIN TRANSACTION " +
                           //AQUI VA NUESTRO SQL DINAMICO...                            
                           pSql +
                        "COMMIT TRANSACTION " +
                       "END TRY " +
                           "BEGIN CATCH " +
                       "IF @@TRANCOUNT > 0 " +
                            "ROLLBACK " +
                       "END CATCH";

            SqlCommand cmd;
            SqlConnection cn;
            int count = 0;

            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql2, cn))
                        {
                            count = cmd.ExecuteNonQuery();
                            if (count > 0)
                            {
                                XtraMessageBox.Show("Configuración guardada correctamente", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                //CARGAR GRILLA CON ELEMENTOS...
                                fnSistema.spllenaGridView(gridElementos, sqlGrilla);
                                if (viewElementos.RowCount > 0)
                                {
                                    fnSistema.spOpcionesGrilla(viewElementos);
                                    ColumnasGrilla();
                                }
                            }
                            else
                            {
                                XtraMessageBox.Show("No se pudo guardar configuración", "Información", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                //ERROR...
            }
        }

        private void OpcionesComboPrincipal(int pOption, TextEdit pCaja, LookUpEdit pCombo)
        {
            //ES DATO FIJO
            if (pOption == 1)
            {
                pCombo.Enabled = false;
                pCaja.Enabled = true;
                pCaja.Text = "";
                pCaja.Focus();
            }
            //ES DATA EMPLEADO?
            else if (pOption == 2)
            {
                pCombo.Enabled = true;
                pCaja.Text = "";
                pCaja.Enabled = false;
            }
            //ES ITEM
            else if (pOption == 3)
            {
                pCombo.Enabled = false;
                pCaja.Text = "";
                pCaja.Enabled = false;
            }
            //ENTIDAD
            else if (pOption == 4)
            {
                pCombo.Enabled = true;
                pCaja.Text = "";
                pCaja.Enabled = false;
            }
        }

        private void SeleccionEntidad(int pOption, LookUpEdit pCombo, string pValueMember, string pDisplayMember)
        {
            Relacion rel = new Relacion();
            List<formula> Listado = new List<formula>();
            
            switch (pOption)
            {
                case 1:
                    Listado = rel.GetColumnsTable("isapre");
                    if (Listado.Count > 0)
                    {
                        pCombo.Properties.DataSource = Listado.ToList();
                        SetPropertiesCombo(pCombo, pValueMember, pDisplayMember);
                    }                     
                    break;

                case 2:
                    Listado = rel.GetColumnsTable("afp");
                    if (Listado.Count > 0)
                    {
                        pCombo.Properties.DataSource = Listado.ToList();
                        SetPropertiesCombo(pCombo, pValueMember, pDisplayMember);
                    }
                    break;

                case 3:
                    Listado = rel.GetColumnsTable("mutual");
                    if (Listado.Count > 0)
                    {
                        pCombo.Properties.DataSource = Listado.ToList();
                        SetPropertiesCombo(pCombo, pValueMember, pDisplayMember);
                    }
                    break;

                case 4:
                    Listado = rel.GetColumnsTable("cajacompensacion");
                    if (Listado.Count > 0)
                    {
                        pCombo.Properties.DataSource = Listado.ToList();
                        SetPropertiesCombo(pCombo, pValueMember, pDisplayMember);
                    }
                    break;

                default:
                    break;
            }
        }   
        private void btnGuardarElemento_Click(object sender, EventArgs e)
        {
            //Obtener todos los controles menores o iguales a columnCount.
            //Comparar cada control de la lista que contenga 
            SqlInsert = "";
            DetalleCuenta det = new DetalleCuenta();            

            if (ColumnCount > 0)
            {
                #region "VALIDATION"
                if (ComboOption1.Name.Length > 0)
                {
                    string d = ComboOption1.Name.Substring(ComboOption1.Name.Length - 1, 1);
                    //SI EL NUMERO QUE ACOMPAÑA A EL NOMBRE ES MENOR A LA CANTIDAD DE COLUMNAS DISPONIBLES VALIDAMOS
                    if (Convert.ToInt32(ComboOption1.Name.Substring(ComboOption1.Name.Length - 1, 1)) <= ColumnCount)
                    {
                        if (Validation(ComboOption1, ComboSubOption1, ComboSubSubOption1, caja1, Convert.ToInt32(d), txtDesc1) == false)
                        { XtraMessageBox.Show("Verifica la información de la columna n°1", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
                    }
                }

                if (ComboOption2.Name.Length > 0)
                {
                    string d = ComboOption2.Name.Substring(ComboOption2.Name.Length - 1, 1);
                    //SI EL NUMERO QUE ACOMPAÑA A EL NOMBRE ES MENOR A LA CANTIDAD DE COLUMNAS DISPONIBLES VALIDAMOS
                    if (Convert.ToInt32(ComboOption2.Name.Substring(ComboOption2.Name.Length - 1, 1)) <= ColumnCount)
                    {
                        if (Validation(ComboOption2, ComboSubOption2, ComboSubSubOption2, caja2, Convert.ToInt32(d), txtDesc2) == false)
                        { XtraMessageBox.Show("Verifica la información de la columna n°2", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
                    }
                }

                if (ComboOption3.Name.Length > 0)
                {
                    string d = ComboOption3.Name.Substring(ComboOption3.Name.Length - 1, 1);
                    //SI EL NUMERO QUE ACOMPAÑA A EL NOMBRE ES MENOR A LA CANTIDAD DE COLUMNAS DISPONIBLES VALIDAMOS
                    if (Convert.ToInt32(ComboOption3.Name.Substring(ComboOption3.Name.Length - 1, 1)) <= ColumnCount)
                    {
                        if (Validation(ComboOption3, ComboSubOption3, ComboSubSubOption3, caja3, Convert.ToInt32(d), txtDesc3) == false)
                        { XtraMessageBox.Show("Verifica la información de la columna n°3", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
                    }
                }

                if (ComboOption4.Name.Length > 0)
                {
                    string d = ComboOption4.Name.Substring(ComboOption4.Name.Length - 1, 1);
                    //SI EL NUMERO QUE ACOMPAÑA A EL NOMBRE ES MENOR A LA CANTIDAD DE COLUMNAS DISPONIBLES VALIDAMOS
                    if (Convert.ToInt32(ComboOption4.Name.Substring(ComboOption4.Name.Length - 1, 1)) <= ColumnCount)
                    {
                        if (Validation(ComboOption4, ComboSubOption4, ComboSubSubOption4, caja4, Convert.ToInt32(d), txtDesc4) == false)
                        { XtraMessageBox.Show("Verifica la información de la columna n°2", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
                    }
                }

                if (ComboOption5.Name.Length > 0)
                {
                    string d = ComboOption5.Name.Substring(ComboOption5.Name.Length - 1, 1);
                    //SI EL NUMERO QUE ACOMPAÑA A EL NOMBRE ES MENOR A LA CANTIDAD DE COLUMNAS DISPONIBLES VALIDAMOS
                    if (Convert.ToInt32(ComboOption5.Name.Substring(ComboOption5.Name.Length - 1, 1)) <= ColumnCount)
                    {
                        if (Validation(ComboOption5, ComboSubOption5, ComboSubSubOption5, caja5, Convert.ToInt32(d), txtDesc5) == false)
                        { XtraMessageBox.Show("Verifica la información de la columna n°5", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
                    }
                }

                if (ComboOption6.Name.Length > 0)
                {
                    string d = ComboOption6.Name.Substring(ComboOption6.Name.Length - 1, 1);
                    //SI EL NUMERO QUE ACOMPAÑA A EL NOMBRE ES MENOR A LA CANTIDAD DE COLUMNAS DISPONIBLES VALIDAMOS
                    if (Convert.ToInt32(ComboOption6.Name.Substring(ComboOption6.Name.Length - 1, 1)) <= ColumnCount)
                    {
                        if (Validation(ComboOption6, ComboSubOption6, ComboSubSubOption6, caja6, Convert.ToInt32(d), txtDesc6) == false)
                        { XtraMessageBox.Show("Verifica la información de la columna n°6", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
                    }
                }

                if (ComboOption7.Name.Length > 0)
                {
                    string d = ComboOption7.Name.Substring(ComboOption7.Name.Length - 1, 1);
                    //SI EL NUMERO QUE ACOMPAÑA A EL NOMBRE ES MENOR A LA CANTIDAD DE COLUMNAS DISPONIBLES VALIDAMOS
                    if (Convert.ToInt32(ComboOption7.Name.Substring(ComboOption7.Name.Length - 1, 1)) <= ColumnCount)
                    {
                        if (Validation(ComboOption7, ComboSubOption7, ComboSubSubOption7, caja7, Convert.ToInt32(d), txtDesc7) == false)
                        { XtraMessageBox.Show("Verifica la información de la columna n°7", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
                    }
                }

                if (ComboOption8.Name.Length > 0)
                {
                    string d = ComboOption8.Name.Substring(ComboOption8.Name.Length - 1, 1);
                    //SI EL NUMERO QUE ACOMPAÑA A EL NOMBRE ES MENOR A LA CANTIDAD DE COLUMNAS DISPONIBLES VALIDAMOS
                    if (Convert.ToInt32(ComboOption8.Name.Substring(ComboOption8.Name.Length - 1, 1)) <= ColumnCount)
                    {
                        if (Validation(ComboOption8, ComboSubOption8, ComboSubSubOption8, caja8, Convert.ToInt32(d), txtDesc8) == false)
                        { XtraMessageBox.Show("Verifica la información de la columna n°8", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
                    }
                }

                if (ComboOption9.Name.Length > 0)
                {
                    string d = ComboOption9.Name.Substring(ComboOption9.Name.Length - 1, 1);
                    //SI EL NUMERO QUE ACOMPAÑA A EL NOMBRE ES MENOR A LA CANTIDAD DE COLUMNAS DISPONIBLES VALIDAMOS
                    if (Convert.ToInt32(ComboOption9.Name.Substring(ComboOption9.Name.Length - 1, 1)) <= ColumnCount)
                    {
                        if (Validation(ComboOption9, ComboSubOption9, ComboSubSubOption9, caja9, Convert.ToInt32(d), txtDesc9) == false)
                        { XtraMessageBox.Show("Verifica la información de la columna n°9", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
                    }
                }

                if (ComboOption10.Name.Length > 0)
                {
                    string d = ComboOption10.Name.Substring(ComboOption10.Name.Length - 2, 2);
                    //SI EL NUMERO QUE ACOMPAÑA A EL NOMBRE ES MENOR A LA CANTIDAD DE COLUMNAS DISPONIBLES VALIDAMOS
                    if (Convert.ToInt32(ComboOption10.Name.Substring(ComboOption10.Name.Length - 2, 2)) <= ColumnCount)
                    {
                        if (Validation(ComboOption10, ComboSubOption10, ComboSubSubOption10, caja10, Convert.ToInt32(d), txtDesc10) == false)
                        { XtraMessageBox.Show("Verifica la información de la columna n°10", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
                    }
                }

                if (ComboOption11.Name.Length > 0)
                {
                    string d = ComboOption11.Name.Substring(ComboOption11.Name.Length - 2, 2);
                    //SI EL NUMERO QUE ACOMPAÑA A EL NOMBRE ES MENOR A LA CANTIDAD DE COLUMNAS DISPONIBLES VALIDAMOS
                    if (Convert.ToInt32(ComboOption11.Name.Substring(ComboOption11.Name.Length - 2, 2)) <= ColumnCount)
                    {
                        if (Validation(ComboOption11, ComboSubOption11, ComboSubSubOption11, caja11, Convert.ToInt32(d), txtDesc11) == false)
                        { XtraMessageBox.Show("Verifica la información de la columna n°11", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
                    }
                }

                if (ComboOption12.Name.Length > 0)
                {
                    string d = ComboOption12.Name.Substring(ComboOption12.Name.Length - 2, 2);
                    //SI EL NUMERO QUE ACOMPAÑA A EL NOMBRE ES MENOR A LA CANTIDAD DE COLUMNAS DISPONIBLES VALIDAMOS
                    if (Convert.ToInt32(ComboOption12.Name.Substring(ComboOption12.Name.Length - 2, 2)) <= ColumnCount)
                    {
                        if (Validation(ComboOption12, ComboSubOption12, ComboSubSubOption12, caja12, Convert.ToInt32(d), txtDesc12) == false)
                        { XtraMessageBox.Show("Verifica la información de la columna n°12", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
                    }
                }

                if (ComboOption13.Name.Length > 0)
                {
                    string d = ComboOption13.Name.Substring(ComboOption13.Name.Length - 2, 2);
                    //SI EL NUMERO QUE ACOMPAÑA A EL NOMBRE ES MENOR A LA CANTIDAD DE COLUMNAS DISPONIBLES VALIDAMOS
                    if (Convert.ToInt32(ComboOption13.Name.Substring(ComboOption13.Name.Length - 2, 2)) <= ColumnCount)
                    {
                        if (Validation(ComboOption13, ComboSubOption13, ComboSubSubOption13, caja13, Convert.ToInt32(d), txtDesc13) == false)
                        { XtraMessageBox.Show("Verifica la información de la columna n°13", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
                    }
                }

                if (ComboOption14.Name.Length > 0)
                {
                    string d = ComboOption14.Name.Substring(ComboOption14.Name.Length - 2, 2);
                    //SI EL NUMERO QUE ACOMPAÑA A EL NOMBRE ES MENOR A LA CANTIDAD DE COLUMNAS DISPONIBLES VALIDAMOS
                    if (Convert.ToInt32(ComboOption14.Name.Substring(ComboOption14.Name.Length - 2, 2)) <= ColumnCount)
                    {
                        if (Validation(ComboOption14, ComboSubOption14, ComboSubSubOption14, caja14, Convert.ToInt32(d), txtDesc14) == false)
                        { XtraMessageBox.Show("Verifica la información de la columna n°2", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
                    }
                }

                if (ComboOption15.Name.Length > 0)
                {
                    string d = ComboOption15.Name.Substring(ComboOption15.Name.Length - 2, 2);
                    //SI EL NUMERO QUE ACOMPAÑA A EL NOMBRE ES MENOR A LA CANTIDAD DE COLUMNAS DISPONIBLES VALIDAMOS
                    if (Convert.ToInt32(ComboOption15.Name.Substring(ComboOption15.Name.Length - 2, 2)) <= ColumnCount)
                    {
                        if (Validation(ComboOption15, ComboSubOption15, ComboSubSubOption15, caja15, Convert.ToInt32(d), txtDesc15) == false)
                        { XtraMessageBox.Show("Verifica la información de la columna n°15", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
                    }
                }

                if (ComboOption16.Name.Length > 0)
                {
                    string d = ComboOption16.Name.Substring(ComboOption16.Name.Length - 2, 2);
                    //SI EL NUMERO QUE ACOMPAÑA A EL NOMBRE ES MENOR A LA CANTIDAD DE COLUMNAS DISPONIBLES VALIDAMOS
                    if (Convert.ToInt32(ComboOption16.Name.Substring(ComboOption16.Name.Length - 2, 2)) <= ColumnCount)
                    {
                        if (Validation(ComboOption16, ComboSubOption16, ComboSubSubOption16, caja16, Convert.ToInt32(d), txtDesc16) == false)
                        { XtraMessageBox.Show("Verifica la información de la columna n°16", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
                    }
                }

                if (ComboOption17.Name.Length > 0)
                {
                    string d = ComboOption17.Name.Substring(ComboOption17.Name.Length - 2, 2);
                    //SI EL NUMERO QUE ACOMPAÑA A EL NOMBRE ES MENOR A LA CANTIDAD DE COLUMNAS DISPONIBLES VALIDAMOS
                    if (Convert.ToInt32(ComboOption17.Name.Substring(ComboOption17.Name.Length - 2, 2)) <= ColumnCount)
                    {
                        if (Validation(ComboOption17, ComboSubOption17, ComboSubSubOption17, caja17, Convert.ToInt32(d), txtDesc17) == false)
                        { XtraMessageBox.Show("Verifica la información de la columna n°17", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
                    }
                }

                if (ComboOption18.Name.Length > 0)
                {
                    string d = ComboOption18.Name.Substring(ComboOption18.Name.Length - 2, 2);
                    //SI EL NUMERO QUE ACOMPAÑA A EL NOMBRE ES MENOR A LA CANTIDAD DE COLUMNAS DISPONIBLES VALIDAMOS
                    if (Convert.ToInt32(ComboOption18.Name.Substring(ComboOption18.Name.Length - 2, 2)) <= ColumnCount)
                    {
                        if (Validation(ComboOption18, ComboSubOption18, ComboSubSubOption18, caja18, Convert.ToInt32(d), txtDesc18) == false)
                        { XtraMessageBox.Show("Verifica la información de la columna n°18", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
                    }
                }

                if (ComboOption19.Name.Length > 0)
                {
                    string d = ComboOption19.Name.Substring(ComboOption19.Name.Length - 2, 2);
                    //SI EL NUMERO QUE ACOMPAÑA A EL NOMBRE ES MENOR A LA CANTIDAD DE COLUMNAS DISPONIBLES VALIDAMOS
                    if (Convert.ToInt32(ComboOption19.Name.Substring(ComboOption19.Name.Length - 2, 2)) <= ColumnCount)
                    {
                        if (Validation(ComboOption19, ComboSubOption19, ComboSubSubOption19, caja19, Convert.ToInt32(d), txtDesc19) == false)
                        { XtraMessageBox.Show("Verifica la información de la columna n°19", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
                    }
                }

                if (ComboOption20.Name.Length > 0)
                {
                    string d = ComboOption20.Name.Substring(ComboOption20.Name.Length - 2, 2);
                    //SI EL NUMERO QUE ACOMPAÑA A EL NOMBRE ES MENOR A LA CANTIDAD DE COLUMNAS DISPONIBLES VALIDAMOS
                    if (Convert.ToInt32(ComboOption20.Name.Substring(ComboOption20.Name.Length - 2, 2)) <= ColumnCount)
                    {
                        if (Validation(ComboOption20, ComboSubOption20, ComboSubSubOption20, caja20, Convert.ToInt32(d), txtDesc20) == false)
                        { XtraMessageBox.Show("Verifica la información de la columna n°20", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
                    }
                }

                if (ComboOption21.Name.Length > 0)
                {
                    string d = ComboOption21.Name.Substring(ComboOption21.Name.Length - 2, 2);
                    //SI EL NUMERO QUE ACOMPAÑA A EL NOMBRE ES MENOR A LA CANTIDAD DE COLUMNAS DISPONIBLES VALIDAMOS
                    if (Convert.ToInt32(ComboOption21.Name.Substring(ComboOption21.Name.Length - 2, 2)) <= ColumnCount)
                    {
                        if (Validation(ComboOption21, ComboSubOption21, ComboSubSubOption21, Caja21, Convert.ToInt32(d), txtDesc21) == false)
                        { XtraMessageBox.Show("Verifica la información de la columna n°21", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
                    }
                }

                if (ComboOption22.Name.Length > 0)
                {
                    string d = ComboOption22.Name.Substring(ComboOption22.Name.Length - 2, 2);
                    //SI EL NUMERO QUE ACOMPAÑA A EL NOMBRE ES MENOR A LA CANTIDAD DE COLUMNAS DISPONIBLES VALIDAMOS
                    if (Convert.ToInt32(ComboOption22.Name.Substring(ComboOption22.Name.Length - 2, 2)) <= ColumnCount)
                    {
                        if (Validation(ComboOption22, ComboSubOption22, ComboSubSubOption22, Caja22, Convert.ToInt32(d), txtDesc22) == false)
                        { XtraMessageBox.Show("Verifica la información de la columna n°22", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
                    }
                }

                if (ComboOption23.Name.Length > 0)
                {
                    string d = ComboOption23.Name.Substring(ComboOption23.Name.Length - 2, 2);
                    //SI EL NUMERO QUE ACOMPAÑA A EL NOMBRE ES MENOR A LA CANTIDAD DE COLUMNAS DISPONIBLES VALIDAMOS
                    if (Convert.ToInt32(ComboOption23.Name.Substring(ComboOption23.Name.Length - 2, 2)) <= ColumnCount)
                    {
                        if (Validation(ComboOption23, ComboSubOption23, ComboSubSubOption23, Caja23, Convert.ToInt32(d), txtDesc23) == false)
                        { XtraMessageBox.Show("Verifica la información de la columna n°23", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
                    }
                }

                if (ComboOption24.Name.Length > 0)
                {
                    string d = ComboOption24.Name.Substring(ComboOption24.Name.Length - 2, 2);
                    //SI EL NUMERO QUE ACOMPAÑA A EL NOMBRE ES MENOR A LA CANTIDAD DE COLUMNAS DISPONIBLES VALIDAMOS
                    if (Convert.ToInt32(ComboOption24.Name.Substring(ComboOption24.Name.Length - 2, 2)) <= ColumnCount)
                    {
                        if (Validation(ComboOption24, ComboSubOption24, ComboSubSubOption24, Caja24, Convert.ToInt32(d), txtDesc24) == false)
                        { XtraMessageBox.Show("Verifica la información de la columna n°24", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
                    }
                }

                if (ComboOption25.Name.Length > 0)
                {
                    string d = ComboOption25.Name.Substring(ComboOption25.Name.Length - 2, 2);
                    //SI EL NUMERO QUE ACOMPAÑA A EL NOMBRE ES MENOR A LA CANTIDAD DE COLUMNAS DISPONIBLES VALIDAMOS
                    if (Convert.ToInt32(ComboOption25.Name.Substring(ComboOption25.Name.Length - 2, 2)) <= ColumnCount)
                    {
                        if (Validation(ComboOption25, ComboSubOption25, ComboSubSubOption25, Caja25, Convert.ToInt32(d), txtDesc25) == false)
                        { XtraMessageBox.Show("Verifica la información de la columna n°25", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
                    }
                }
                #endregion

                //SI TODO VA BIEN GUARDAMOS INFORMACION
                try
                {
                    if (SqlInsert.Length > 0)
                    {
                        if (det.ExistenElementos(CuentaMaestra))
                        {
                            //Eliminar todos los registros asociados a la cuenta y luego volver a ingresar todos.                    
                            if (det.EliminarTodos(CuentaMaestra, EsquemaAsociado))
                            {
                                //INGRESAMOS DATOS                           
                                SaveData(SqlInsert);
                            }
                            else
                            {
                                XtraMessageBox.Show("No se pudo realizar proceso", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            }
                        }
                        else
                        {
                            //SOLO INGRESAMOS
                            SaveData(SqlInsert);
                        }                      
                    }
                    else
                    {
                        XtraMessageBox.Show("No se pudo realizar proceso", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show("No se pudo realizar proceso", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }


            }
            else
            {
                XtraMessageBox.Show("Error al procesar operacion, no se pudieron guardar los cambios", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }        
                     
        }


        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ComboOption1_EditValueChanged(object sender, EventArgs e)
        {
            int pOption = 0;
            pOption = Convert.ToInt32(ComboOption1.EditValue);

            ConvertEditor("LookUpEdit", ComboSubOption1, pOption);
            OpcionesComboPrincipal(pOption, caja1, ComboSubSubOption1);

            if (ComboSubOption1.Properties.DataSource != null)
                ComboSubOption1.ItemIndex = 0;

            if (ComboSubSubOption1.Properties.DataSource != null)
                ComboSubSubOption1.ItemIndex = 0;         
            
        }

        private void ComboOption2_EditValueChanged(object sender, EventArgs e)
        {
            int pOption = 0;
            pOption = Convert.ToInt32(ComboOption2.EditValue);

            ConvertEditor("LookUpEdit", ComboSubOption2, pOption);

            OpcionesComboPrincipal(pOption, caja2, ComboSubSubOption2);

            if (ComboSubOption2.Properties.DataSource != null)
                ComboSubOption2.ItemIndex = 0;

            if (ComboSubSubOption2.Properties.DataSource != null)
                ComboSubSubOption2.ItemIndex = 0;
        }

        private void ComboOption3_EditValueChanged(object sender, EventArgs e)
        {
            int pOption = 0;
            pOption = Convert.ToInt32(ComboOption3.EditValue);

            ConvertEditor("LookUpEdit", ComboSubOption3, pOption);

            OpcionesComboPrincipal(pOption, caja3, ComboSubSubOption3);

            if (ComboSubOption3.Properties.DataSource != null)
                ComboSubOption3.ItemIndex = 0;

            if (ComboSubSubOption3.Properties.DataSource != null)
                ComboSubSubOption3.ItemIndex = 0;
        }

        private void ComboOption4_EditValueChanged(object sender, EventArgs e)
        {
            int pOption = 0;
            pOption = Convert.ToInt32(ComboOption4.EditValue);

            ConvertEditor("LookUpEdit", ComboSubOption4, pOption);

            OpcionesComboPrincipal(pOption, caja4, ComboSubSubOption4);

            if (ComboSubOption4.Properties.DataSource != null)
                ComboSubOption4.ItemIndex = 0;

            if (ComboSubSubOption4.Properties.DataSource != null)
                ComboSubSubOption4.ItemIndex = 0;
        }

        private void ComboOption5_EditValueChanged(object sender, EventArgs e)
        {
            int pOption = 0;
            pOption = Convert.ToInt32(ComboOption5.EditValue);

            ConvertEditor("LookUpEdit", ComboSubOption5, pOption);

            OpcionesComboPrincipal(pOption, caja5, ComboSubSubOption5);

            if (ComboSubOption5.Properties.DataSource != null)
                ComboSubOption5.ItemIndex = 0;

            if (ComboSubSubOption5.Properties.DataSource != null)
                ComboSubSubOption5.ItemIndex = 0;
        }

        private void ComboOption6_EditValueChanged(object sender, EventArgs e)
        {
            int pOption = 0;
            pOption = Convert.ToInt32(ComboOption6.EditValue);

            ConvertEditor("LookUpEdit", ComboSubOption6, pOption);
            OpcionesComboPrincipal(pOption, caja6, ComboSubSubOption6);

            if (ComboSubOption6.Properties.DataSource != null)
                ComboSubOption6.ItemIndex = 0;

            if (ComboSubSubOption6.Properties.DataSource != null)
                ComboSubSubOption6.ItemIndex = 0;
        }

        private void ComboOption7_EditValueChanged(object sender, EventArgs e)
        {
            int pOption = 0;
            pOption = Convert.ToInt32(ComboOption7.EditValue);

            ConvertEditor("LookUpEdit", ComboSubOption7, pOption);

            OpcionesComboPrincipal(pOption, caja7, ComboSubSubOption7);

            if (ComboSubOption7.Properties.DataSource != null)
                ComboSubOption7.ItemIndex = 0;

            if (ComboSubSubOption7.Properties.DataSource != null)
                ComboSubSubOption7.ItemIndex = 0;
        }

        private void ComboOption8_EditValueChanged(object sender, EventArgs e)
        {
            int pOption = 0;
            pOption = Convert.ToInt32(ComboOption8.EditValue);

            ConvertEditor("LookUpEdit", ComboSubOption8, pOption);
            OpcionesComboPrincipal(pOption, caja8, ComboSubSubOption8);

            if (ComboSubOption8.Properties.DataSource != null)
                ComboSubOption8.ItemIndex = 0;

            if (ComboSubSubOption8.Properties.DataSource != null)
                ComboSubSubOption8.ItemIndex = 0;
        }

        private void ComboOption9_EditValueChanged(object sender, EventArgs e)
        {
            int pOption = 0;
            pOption = Convert.ToInt32(ComboOption9.EditValue);

            ConvertEditor("LookUpEdit", ComboSubOption9, pOption);

            OpcionesComboPrincipal(pOption, caja9, ComboSubSubOption9);

            if (ComboSubOption9.Properties.DataSource != null)
                ComboSubOption9.ItemIndex = 0;

            if (ComboSubSubOption9.Properties.DataSource != null)
                ComboSubSubOption9.ItemIndex = 0;
        }

        private void ComboOption10_EditValueChanged(object sender, EventArgs e)
        {
            int pOption = 0;
            pOption = Convert.ToInt32(ComboOption10.EditValue);

            ConvertEditor("LookUpEdit", ComboSubOption10, pOption);

            OpcionesComboPrincipal(pOption, caja10, ComboSubSubOption10);

            if (ComboSubOption10.Properties.DataSource != null)
                ComboSubOption10.ItemIndex = 0;

            if (ComboSubSubOption10.Properties.DataSource != null)
                ComboSubSubOption10.ItemIndex = 0;
        }

        private void ComboOption11_EditValueChanged(object sender, EventArgs e)
        {
            int pOption = 0;
            pOption = Convert.ToInt32(ComboOption11.EditValue);

            ConvertEditor("LookUpEdit", ComboSubOption11, pOption);
            OpcionesComboPrincipal(pOption, caja11, ComboSubSubOption11);

            if (ComboSubOption11.Properties.DataSource != null)
                ComboSubOption11.ItemIndex = 0;

            if (ComboSubSubOption11.Properties.DataSource != null)
                ComboSubSubOption11.ItemIndex = 0;
        }

        private void ComboOption12_EditValueChanged(object sender, EventArgs e)
        {
            int pOption = 0;
            pOption = Convert.ToInt32(ComboOption12.EditValue);

            ConvertEditor("LookUpEdit", ComboSubOption12, pOption);

            OpcionesComboPrincipal(pOption, caja12, ComboSubSubOption12);

            if (ComboSubOption12.Properties.DataSource != null)
                ComboSubOption12.ItemIndex = 0;

            if (ComboSubSubOption12.Properties.DataSource != null)
                ComboSubSubOption12.ItemIndex = 0;
        }

        private void ComboOption13_EditValueChanged(object sender, EventArgs e)
        {
            int pOption = 0;
            pOption = Convert.ToInt32(ComboOption13.EditValue);

            ConvertEditor("LookUpEdit", ComboSubOption13, pOption);

            OpcionesComboPrincipal(pOption, caja13, ComboSubSubOption13);

            if (ComboSubOption13.Properties.DataSource != null)
                ComboSubOption13.ItemIndex = 0;

            if (ComboSubSubOption13.Properties.DataSource != null)
                ComboSubSubOption13.ItemIndex = 0;
        }

        private void ComboOption14_EditValueChanged(object sender, EventArgs e)
        {
            int pOption = 0;
            pOption = Convert.ToInt32(ComboOption14.EditValue);

            ConvertEditor("LookUpEdit", ComboSubOption14, pOption);

            OpcionesComboPrincipal(pOption, caja14, ComboSubSubOption14);

            if (ComboSubOption14.Properties.DataSource != null)
                ComboSubOption14.ItemIndex = 0;

            if (ComboSubSubOption14.Properties.DataSource != null)
                ComboSubSubOption14.ItemIndex = 0;
        }

        private void ComboOption15_EditValueChanged(object sender, EventArgs e)
        {
            int pOption = 0;
            pOption = Convert.ToInt32(ComboOption15.EditValue);

            ConvertEditor("LookUpEdit", ComboSubOption15, pOption);

            OpcionesComboPrincipal(pOption, caja15, ComboSubSubOption15);

            if (ComboSubOption15.Properties.DataSource != null)
                ComboSubOption15.ItemIndex = 0;

            if (ComboSubSubOption15.Properties.DataSource != null)
                ComboSubSubOption15.ItemIndex = 0;
        }

        private void ComboOption16_EditValueChanged(object sender, EventArgs e)
        {
            int pOption = 0;
            pOption = Convert.ToInt32(ComboOption16.EditValue);

            ConvertEditor("LookUpEdit", ComboSubOption16, pOption);

            OpcionesComboPrincipal(pOption, caja16, ComboSubSubOption16);

            if (ComboSubOption16.Properties.DataSource != null)
                ComboSubOption16.ItemIndex = 0;

            if (ComboSubSubOption16.Properties.DataSource != null)
                ComboSubSubOption16.ItemIndex = 0;
        }

        private void groupColumnas_Enter(object sender, EventArgs e)
        {

        }

        private void ComboOption17_EditValueChanged(object sender, EventArgs e)
        {
            int pOption = 0;
            pOption = Convert.ToInt32(ComboOption17.EditValue);

            ConvertEditor("LookUpEdit", ComboSubOption17, pOption);
            OpcionesComboPrincipal(pOption, caja17, ComboSubSubOption17);

            if (ComboSubOption17.Properties.DataSource != null)
                ComboSubOption17.ItemIndex = 0;

            if (ComboSubSubOption17.Properties.DataSource != null)
                ComboSubSubOption17.ItemIndex = 0;
        }

        private void ComboOption18_EditValueChanged(object sender, EventArgs e)
        {
            int pOption = 0;
            pOption = Convert.ToInt32(ComboOption18.EditValue);

            ConvertEditor("LookUpEdit", ComboSubOption18, pOption);

            OpcionesComboPrincipal(pOption, caja18, ComboSubSubOption18);

            if (ComboSubOption18.Properties.DataSource != null)
                ComboSubOption18.ItemIndex = 0;

            if (ComboSubSubOption18.Properties.DataSource != null)
                ComboSubSubOption18.ItemIndex = 0;
        }

        private void ComboOption19_EditValueChanged(object sender, EventArgs e)
        {
            int pOption = 0;
            pOption = Convert.ToInt32(ComboOption19.EditValue);

            ConvertEditor("LookUpEdit", ComboSubOption19, pOption);

            OpcionesComboPrincipal(pOption, caja19, ComboSubSubOption19);

            if (ComboSubOption19.Properties.DataSource != null)
                ComboSubOption19.ItemIndex = 0;

            if (ComboSubSubOption19.Properties.DataSource != null)
                ComboSubSubOption19.ItemIndex = 0;
        }

        private void ComboOption20_EditValueChanged(object sender, EventArgs e)
        {
            int pOption = 0;
            pOption = Convert.ToInt32(ComboOption20.EditValue);

            ConvertEditor("LookUpEdit", ComboSubOption20, pOption);

            OpcionesComboPrincipal(pOption, caja20, ComboSubSubOption20);

            if (ComboSubOption20.Properties.DataSource != null)
                ComboSubOption20.ItemIndex = 0;

            if (ComboSubSubOption20.Properties.DataSource != null)
                ComboSubSubOption20.ItemIndex = 0;
        }

        private void ComboSubOption1_EditValueChanged(object sender, EventArgs e)
        {
            LookUpEdit Combo = sender as LookUpEdit;

            ChangeValueCombo(Combo, ComboSubSubOption1, ComboOption1, caja1);            
        }

        private void ChangeValueCombo(LookUpEdit pSenderCombo, LookUpEdit pComboSubSubOption, LookUpEdit pComboOption, TextEdit pCaja)
        {
            int FirstSelec = 0;
            string Field = "";
            Relacion rel = new Relacion();
            List<Relacion> Relaciones = new List<Relacion>();
            List<formula> Listado = new List<formula>();
            List<formula> ListadoFecha = new List<formula>();

            pComboSubSubOption.Properties.DataSource = null;
            //caja1.Enabled = false;
            if (pSenderCombo.Properties.DataSource != null && pComboOption.Properties.DataSource != null)
            {
                //OBTENEMOS LA SELECCION DEL COMBO PRINCIPAL (EL DE LOS TIPOS DE OPCIONES)
                FirstSelec = Convert.ToInt32(pComboOption.EditValue);

                IsNumber = false;
                IsText = false;

                //ES TIPO 1 (TEXT FIJO) ??
                if (FirstSelec == 1)
                {

                    pComboSubSubOption.Enabled = false;

                    /*Dato fijo - FECHA*/
                    if (Convert.ToInt32(pSenderCombo.EditValue) == 3)
                    {

                        //Habilitamos tercer combobox
                        pComboSubSubOption.Enabled = true;
                        //Habilitamos caja
                        pCaja.BringToFront();
                        pCaja.ReadOnly = false;
                        pCaja.Focus();

                        ListadoFecha.Add(new formula() { key = "dd-mm-yyyy", desc = "dd-mm-yyyy" });
                        ListadoFecha.Add(new formula() { key = "yyyy-mm-dd", desc = "yyyy-mm-dd" });
                        pComboSubSubOption.Properties.DataSource = ListadoFecha.ToList();
                        SetPropertiesCombo(pComboSubSubOption, "key", "desc");

                        ConvertEditorCalendar("DateEdit", pCaja, "dd-mm-yyyy");
                        //CARGAR COMBOBOX CALENDARIO
                        //CONVERTIR TEXT EDIT EN DATEEDIT
                    }
                    /*Dato fijo - NUMERO*/
                    else if (Convert.ToInt32(pSenderCombo.EditValue) == 2)
                    {
                        pCaja.BringToFront();
                        pCaja.ReadOnly = false;
                        pCaja.Focus();

                        IsNumber = true;
                        pCaja.Text = "";
                        pCaja.Properties.Mask.EditMask = @"\d{0,6}(.\d{0,4})?";
                        pCaja.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
                    }
                    /*Dato fijo --> TEXTO*/
                    else if (Convert.ToInt32(pSenderCombo.EditValue) == 1)
                    {
                        pCaja.BringToFront();
                        pCaja.ReadOnly = false;
                        pCaja.Focus();

                        IsText = true;
                        pCaja.Text = "";
                        pCaja.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;

                    }
                }
                //ES dato de empleado???
                else if (FirstSelec == 2)
                {
                    //caja1.BringToFront();
                    //caja1.SendToBack();
                    //caja1.Enabled = false;
                    pCaja.SendToBack();
                    pCaja.ReadOnly = true;
                    //OBTENEMOS EL DATO SELECCIONADO QUE REPRESENTA UN CAMPO DE LA TABLA EMPLEADO
                    Field = (string)pSenderCombo.EditValue;

                    //ESTA CAMPO VIENE DE OTRA TABLA ??
                    Relaciones = rel.GetRelaciones("trabajador");

                    if (Relaciones.Count > 0)
                    {
                        if (rel.EsForaneo(Relaciones, Field))
                        {
                            pComboSubSubOption.Enabled = true;

                            //MOSTRAMOS TERCER COMBO BOX CON CAMPOS DE LA TABLA
                            Listado = rel.GetColumnsTable(rel.GetInfo(Relaciones, Field).TablaRef);
                            if (Listado.Count > 0)
                            {
                                pComboSubSubOption.Properties.DataSource = Listado.ToList();
                                SetPropertiesCombo(pComboSubSubOption, "key", "desc");
                            }
                        }
                        else
                        {
                            pComboSubSubOption.Enabled = false;
                        }
                    }
                    else
                    {
                        pComboSubSubOption.Enabled = false;
                    }
                }
                else if (FirstSelec == 3)
                {
                    //caja1.BringToFront();
                    pCaja.SendToBack();
                    pCaja.ReadOnly = true;
                    pComboSubSubOption.Enabled = false;
                }
                //ENTIDAD ???
                else if (FirstSelec == 4)
                {
                    pCaja.SendToBack();
                    pCaja.ReadOnly = true;
                    int SecondOption = Convert.ToInt32(pSenderCombo.EditValue);

                    SeleccionEntidad(SecondOption, pComboSubSubOption, "key", "desc");
                }

                if (pComboSubSubOption.Properties.DataSource != null)
                    pComboSubSubOption.ItemIndex = 0;

            }
        }

        private void ComboSubOption2_EditValueChanged(object sender, EventArgs e)
        {
            LookUpEdit Combo = sender as LookUpEdit;

            ChangeValueCombo(Combo, ComboSubSubOption2, ComboOption2, caja2);

        }

        private void ComboSubOption3_EditValueChanged(object sender, EventArgs e)
        {
            LookUpEdit Combo = sender as LookUpEdit;

            ChangeValueCombo(Combo, ComboSubSubOption3, ComboOption3, caja3);
        }

        private void ComboSubOption4_EditValueChanged(object sender, EventArgs e)
        {
            LookUpEdit Combo = sender as LookUpEdit;

            ChangeValueCombo(Combo, ComboSubSubOption4, ComboOption4, caja4);
        }

        private void ComboSubOption5_EditValueChanged(object sender, EventArgs e)
        {
            LookUpEdit Combo = sender as LookUpEdit;

            ChangeValueCombo(Combo, ComboSubSubOption5, ComboOption5, caja5);
        }

        private void ComboSubOption6_EditValueChanged(object sender, EventArgs e)
        {
            LookUpEdit Combo = sender as LookUpEdit;

            ChangeValueCombo(Combo, ComboSubSubOption6, ComboOption6, caja6);
        }

        private void ComboSubOption7_EditValueChanged(object sender, EventArgs e)
        {
            LookUpEdit Combo = sender as LookUpEdit;

            ChangeValueCombo(Combo, ComboSubSubOption7, ComboOption7, caja7);
        }

        private void ComboSubOption8_EditValueChanged(object sender, EventArgs e)
        {
            LookUpEdit Combo = sender as LookUpEdit;

            ChangeValueCombo(Combo, ComboSubSubOption8, ComboOption8, caja8);
        }

        private void ComboSubOption9_EditValueChanged(object sender, EventArgs e)
        {
            LookUpEdit Combo = sender as LookUpEdit;

            ChangeValueCombo(Combo, ComboSubSubOption9, ComboOption9, caja9);
        }

        private void ComboSubOption10_EditValueChanged(object sender, EventArgs e)
        {
            LookUpEdit Combo = sender as LookUpEdit;

            ChangeValueCombo(Combo, ComboSubSubOption10, ComboOption10, caja10);
        }

        private void ComboSubOption11_EditValueChanged(object sender, EventArgs e)
        {
            LookUpEdit Combo = sender as LookUpEdit;

            ChangeValueCombo(Combo, ComboSubSubOption11, ComboOption11, caja11);
        }

        private void ComboSubOption12_EditValueChanged(object sender, EventArgs e)
        {
            LookUpEdit Combo = sender as LookUpEdit;

            ChangeValueCombo(Combo, ComboSubSubOption12, ComboOption12, caja12);
        }

        private void ComboSubOption13_EditValueChanged(object sender, EventArgs e)
        {
            LookUpEdit Combo = sender as LookUpEdit;

            ChangeValueCombo(Combo, ComboSubSubOption13, ComboOption13, caja13);
        }

        private void ComboSubOption14_EditValueChanged(object sender, EventArgs e)
        {
            LookUpEdit Combo = sender as LookUpEdit;

            ChangeValueCombo(Combo, ComboSubSubOption14, ComboOption14, caja14);
        }

        private void ComboSubOption15_EditValueChanged(object sender, EventArgs e)
        {
            LookUpEdit Combo = sender as LookUpEdit;

            ChangeValueCombo(Combo, ComboSubSubOption15, ComboOption15, caja15);
        }

        private void ComboSubOption16_EditValueChanged(object sender, EventArgs e)
        {
            LookUpEdit Combo = sender as LookUpEdit;

            ChangeValueCombo(Combo, ComboSubSubOption16, ComboOption16, caja16);
        }

        private void ComboSubOption17_EditValueChanged(object sender, EventArgs e)
        {
            LookUpEdit Combo = sender as LookUpEdit;

            ChangeValueCombo(Combo, ComboSubSubOption17, ComboOption17, caja17);
        }

        private void ComboSubOption18_EditValueChanged(object sender, EventArgs e)
        {
            LookUpEdit Combo = sender as LookUpEdit;

            ChangeValueCombo(Combo, ComboSubSubOption18, ComboOption18, caja18);
        }

        private void ComboSubOption19_EditValueChanged(object sender, EventArgs e)
        {
            LookUpEdit Combo = sender as LookUpEdit;

            ChangeValueCombo(Combo, ComboSubSubOption19, ComboOption19, caja19);
        }

        private void ComboSubOption20_EditValueChanged(object sender, EventArgs e)
        {
            LookUpEdit Combo = sender as LookUpEdit;

            ChangeValueCombo(Combo, ComboSubSubOption20, ComboOption20, caja20);
        }

        private void gridElementos_Click(object sender, EventArgs e)
        {
            CargaDatos();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            CargaDatos();
        }

        private void ComboSubSubOption5_EditValueChanged(object sender, EventArgs e)
        {
            if (ComboSubSubOption5.Properties.DataSource != null)
            {
                if (ComboSubSubOption5.EditValue != null)
                {
                    if (ComboSubSubOption5.EditValue.ToString() == "dd-mm-yyyy" || ComboSubSubOption5.EditValue.ToString() == "yyyy-mm-dd")
                    {
                        //CONVERTIR ULTIMA CAJA DE TEXTO EN DATEEDIT
                        ConvertEditorCalendar("DateEdit", caja5, ComboSubSubOption5.EditValue.ToString());
                    }
                    else
                    {
                        ConvertTextEdit("TextEdit", caja5);
                    }
                }
            }
            else
            {
                ConvertTextEdit("TextEdit", caja5);
            }
        }

        private void ComboSubSubOption1_EditValueChanged(object sender, EventArgs e)
        {            
            if (ComboSubSubOption1.Properties.DataSource != null)
            {               

                if (ComboSubSubOption1.EditValue != null)
                {
                    if (ComboSubSubOption1.EditValue.ToString() == "dd-mm-yyyy" || ComboSubSubOption1.EditValue.ToString() == "yyyy-mm-dd")
                    {
                        //CONVERTIR ULTIMA CAJA DE TEXTO EN DATEEDIT
                        ConvertEditorCalendar("DateEdit", caja1, ComboSubSubOption1.EditValue.ToString());
                    }
                    else
                    {
                        ConvertTextEdit("TextEdit", caja1);
                    }
                }
            }
            else
            {
                ConvertTextEdit("TextEdit", caja1);
            }
        }

        private void ComboSubSubOption2_EditValueChanged(object sender, EventArgs e)
        {
            if (ComboSubSubOption2.Properties.DataSource != null)
            {
                if (ComboSubSubOption2.EditValue != null)
                {
                    if (ComboSubSubOption2.EditValue.ToString() == "dd-mm-yyyy" || ComboSubSubOption2.EditValue.ToString() == "yyyy-mm-dd")
                    {
                        //CONVERTIR ULTIMA CAJA DE TEXTO EN DATEEDIT
                        ConvertEditorCalendar("DateEdit", caja2, ComboSubSubOption2.EditValue.ToString());
                    }
                    else
                    {
                        ConvertTextEdit("TextEdit", caja2);
                    }
                }
            }
            else
            {
                ConvertTextEdit("TextEdit", caja2);
            }
        }

        private void ComboSubSubOption3_EditValueChanged(object sender, EventArgs e)
        {
            if (ComboSubSubOption3.Properties.DataSource != null)
            {
                if (ComboSubSubOption3.EditValue != null)
                {
                    if (ComboSubSubOption3.EditValue.ToString() == "dd-mm-yyyy" || ComboSubSubOption3.EditValue.ToString() == "yyyy-mm-dd")
                    {
                        //CONVERTIR ULTIMA CAJA DE TEXTO EN DATEEDIT
                        ConvertEditorCalendar("DateEdit", caja3, ComboSubSubOption3.EditValue.ToString());
                    }
                    else
                    {
                        ConvertTextEdit("TextEdit", caja3);
                    }
                }
            }
            else
            {
                ConvertTextEdit("TextEdit", caja3);
            }
        }

        private void ComboSubSubOption4_EditValueChanged(object sender, EventArgs e)
        {
            if (ComboSubSubOption4.Properties.DataSource != null)
            {
                if (ComboSubSubOption4.EditValue != null)
                {
                    if (ComboSubSubOption4.EditValue.ToString() == "dd-mm-yyyy" || ComboSubSubOption4.EditValue.ToString() == "yyyy-mm-dd")
                    {
                        //CONVERTIR ULTIMA CAJA DE TEXTO EN DATEEDIT
                        ConvertEditorCalendar("DateEdit", caja4, ComboSubSubOption4.EditValue.ToString());
                    }
                    else
                    {
                        ConvertTextEdit("TextEdit", caja4);
                    }
                }
            }
            else
            {
                ConvertTextEdit("TextEdit", caja4);
            }
        }

        private void ComboSubSubOption6_EditValueChanged(object sender, EventArgs e)
        {
            if (ComboSubSubOption6.Properties.DataSource != null)
            {
                if (ComboSubSubOption6.EditValue != null)
                {
                    if (ComboSubSubOption6.EditValue.ToString() == "dd-mm-yyyy" || ComboSubSubOption6.EditValue.ToString() == "yyyy-mm-dd")
                    {
                        //CONVERTIR ULTIMA CAJA DE TEXTO EN DATEEDIT
                        ConvertEditorCalendar("DateEdit", caja6, ComboSubSubOption6.EditValue.ToString());
                    }
                    else
                    {
                        ConvertTextEdit("TextEdit", caja6);
                    }
                }
            }
            else
            {
                ConvertTextEdit("TextEdit", caja6);
            }
        }

        private void ComboSubSubOption7_EditValueChanged(object sender, EventArgs e)
        {
            if (ComboSubSubOption7.Properties.DataSource != null)
            {
                if (ComboSubSubOption7.EditValue != null)
                {
                    if (ComboSubSubOption7.EditValue.ToString() == "dd-mm-yyyy" || ComboSubSubOption7.EditValue.ToString() == "yyyy-mm-dd")
                    {
                        //CONVERTIR ULTIMA CAJA DE TEXTO EN DATEEDIT
                        ConvertEditorCalendar("DateEdit", caja7, ComboSubSubOption7.EditValue.ToString());
                    }
                    else
                    {
                        ConvertTextEdit("TextEdit", caja7);
                    }
                }
            }
            else
            {
                ConvertTextEdit("TextEdit", caja7);
            }
        }

        private void ComboSubSubOption8_EditValueChanged(object sender, EventArgs e)
        {
            if (ComboSubSubOption8.Properties.DataSource != null)
            {
                if (ComboSubSubOption8.EditValue != null)
                {
                    if (ComboSubSubOption8.EditValue.ToString() == "dd-mm-yyyy" || ComboSubSubOption8.EditValue.ToString() == "yyyy-mm-dd")
                    {
                        //CONVERTIR ULTIMA CAJA DE TEXTO EN DATEEDIT
                        ConvertEditorCalendar("DateEdit", caja8, ComboSubSubOption8.EditValue.ToString());
                    }
                    else
                    {
                        ConvertTextEdit("TextEdit", caja8);
                    }
                }
            }
            else
            {
                ConvertTextEdit("TextEdit", caja8);
            }
        }

        private void ComboSubSubOption9_EditValueChanged(object sender, EventArgs e)
        {
            if (ComboSubSubOption9.Properties.DataSource != null)
            {
                if (ComboSubSubOption9.EditValue != null)
                {
                    if (ComboSubSubOption9.EditValue.ToString() == "dd-mm-yyyy" || ComboSubSubOption9.EditValue.ToString() == "yyyy-mm-dd")
                    {
                        //CONVERTIR ULTIMA CAJA DE TEXTO EN DATEEDIT
                        ConvertEditorCalendar("DateEdit", caja9, ComboSubSubOption9.EditValue.ToString());
                    }
                    else
                    {
                        ConvertTextEdit("TextEdit", caja9);
                    }
                }
            }
            else
            {
                ConvertTextEdit("TextEdit", caja9);
            }
        }

        private void ComboSubSubOption10_EditValueChanged(object sender, EventArgs e)
        {
            if (ComboSubSubOption10.Properties.DataSource != null)
            {
                if (ComboSubSubOption10.EditValue != null)
                {
                    if (ComboSubSubOption10.EditValue.ToString() == "dd-mm-yyyy" || ComboSubSubOption10.EditValue.ToString() == "yyyy-mm-dd")
                    {
                        //CONVERTIR ULTIMA CAJA DE TEXTO EN DATEEDIT
                        ConvertEditorCalendar("DateEdit", caja10, ComboSubSubOption10.EditValue.ToString());
                    }
                    else
                    {
                        ConvertTextEdit("TextEdit", caja10);
                    }
                }
            }
            else
            {
                ConvertTextEdit("TextEdit", caja10);
            }
        }

        private void ComboSubSubOption11_EditValueChanged(object sender, EventArgs e)
        {
            if (ComboSubSubOption11.Properties.DataSource != null)
            {
                if (ComboSubSubOption11.EditValue != null)
                {
                    if (ComboSubSubOption11.EditValue.ToString() == "dd-mm-yyyy" || ComboSubSubOption11.EditValue.ToString() == "yyyy-mm-dd")
                    {
                        //CONVERTIR ULTIMA CAJA DE TEXTO EN DATEEDIT
                        ConvertEditorCalendar("DateEdit", caja11, ComboSubSubOption11.EditValue.ToString());
                    }
                    else
                    {
                        ConvertTextEdit("TextEdit", caja11);
                    }
                }
            }
            else
            {
                ConvertTextEdit("TextEdit", caja11);
            }
        }

        private void ComboSubSubOption12_EditValueChanged(object sender, EventArgs e)
        {
            if (ComboSubSubOption12.Properties.DataSource != null)
            {
                if (ComboSubSubOption12.EditValue != null)
                {
                    if (ComboSubSubOption12.EditValue.ToString() == "dd-mm-yyyy" || ComboSubSubOption12.EditValue.ToString() == "yyyy-mm-dd")
                    {
                        //CONVERTIR ULTIMA CAJA DE TEXTO EN DATEEDIT
                        ConvertEditorCalendar("DateEdit", caja12, ComboSubSubOption12.EditValue.ToString());
                    }
                    else
                    {
                        ConvertTextEdit("TextEdit", caja12);
                    }
                }
            }
            else
            {
                ConvertTextEdit("TextEdit", caja12);
            }
        }

        private void ComboSubSubOption13_EditValueChanged(object sender, EventArgs e)
        {
            if (ComboSubSubOption13.Properties.DataSource != null)
            {
                if (ComboSubSubOption13.EditValue != null)
                {
                    if (ComboSubSubOption13.EditValue.ToString() == "dd-mm-yyyy" || ComboSubSubOption13.EditValue.ToString() == "yyyy-mm-dd")
                    {
                        //CONVERTIR ULTIMA CAJA DE TEXTO EN DATEEDIT
                        ConvertEditorCalendar("DateEdit", caja13, ComboSubSubOption13.EditValue.ToString());
                    }
                    else
                    {
                        ConvertTextEdit("TextEdit", caja13);
                    }
                }
            }
            else
            {
                ConvertTextEdit("TextEdit", caja13);
            }
        }

        private void ComboSubSubOption14_EditValueChanged(object sender, EventArgs e)
        {
            if (ComboSubSubOption14.Properties.DataSource != null)
            {
                if (ComboSubSubOption14.EditValue != null)
                {
                    if (ComboSubSubOption14.EditValue.ToString() == "dd-mm-yyyy" || ComboSubSubOption14.EditValue.ToString() == "yyyy-mm-dd")
                    {
                        //CONVERTIR ULTIMA CAJA DE TEXTO EN DATEEDIT
                        ConvertEditorCalendar("DateEdit", caja14, ComboSubSubOption14.EditValue.ToString());
                    }
                    else
                    {
                        ConvertTextEdit("TextEdit", caja14);
                    }
                }
            }
            else
            {
                ConvertTextEdit("TextEdit", caja14);
            }
        }

        private void ComboSubSubOption15_EditValueChanged(object sender, EventArgs e)
        {
            if (ComboSubSubOption15.Properties.DataSource != null)
            {
                if (ComboSubSubOption15.EditValue != null)
                {
                    if (ComboSubSubOption15.EditValue.ToString() == "dd-mm-yyyy" || ComboSubSubOption15.EditValue.ToString() == "yyyy-mm-dd")
                    {
                        //CONVERTIR ULTIMA CAJA DE TEXTO EN DATEEDIT
                        ConvertEditorCalendar("DateEdit", caja15, ComboSubSubOption15.EditValue.ToString());
                    }
                    else
                    {
                        ConvertTextEdit("TextEdit", caja15);
                    }
                }
            }
            else
            {
                ConvertTextEdit("TextEdit", caja15);
            }
        }

        private void ComboSubSubOption16_EditValueChanged(object sender, EventArgs e)
        {
            if (ComboSubSubOption16.Properties.DataSource != null)
            {
                if (ComboSubSubOption16.EditValue != null)
                {
                    if (ComboSubSubOption16.EditValue.ToString() == "dd-mm-yyyy" || ComboSubSubOption16.EditValue.ToString() == "yyyy-mm-dd")
                    {
                        //CONVERTIR ULTIMA CAJA DE TEXTO EN DATEEDIT
                        ConvertEditorCalendar("DateEdit", caja16, ComboSubSubOption16.EditValue.ToString());
                    }
                    else
                    {
                        ConvertTextEdit("TextEdit", caja16);
                    }
                }
            }
            else
            {
                ConvertTextEdit("TextEdit", caja16);
            }
        }

        private void ComboSubSubOption17_EditValueChanged(object sender, EventArgs e)
        {
            if (ComboSubSubOption17.Properties.DataSource != null)
            {
                if (ComboSubSubOption17.EditValue != null)
                {
                    if (ComboSubSubOption17.EditValue.ToString() == "dd-mm-yyyy" || ComboSubSubOption17.EditValue.ToString() == "yyyy-mm-dd")
                    {
                        //CONVERTIR ULTIMA CAJA DE TEXTO EN DATEEDIT
                        ConvertEditorCalendar("DateEdit", caja17, ComboSubSubOption17.EditValue.ToString());
                    }
                    else
                    {
                        ConvertTextEdit("TextEdit", caja17);
                    }
                }
            }
            else
            {
                ConvertTextEdit("TextEdit", caja17);
            }
        }

        private void ComboSubSubOption18_EditValueChanged(object sender, EventArgs e)
        {
            if (ComboSubSubOption18.Properties.DataSource != null)
            {
                if (ComboSubSubOption18.EditValue != null)
                {
                    if (ComboSubSubOption18.EditValue.ToString() == "dd-mm-yyyy" || ComboSubSubOption18.EditValue.ToString() == "yyyy-mm-dd")
                    {
                        //CONVERTIR ULTIMA CAJA DE TEXTO EN DATEEDIT
                        ConvertEditorCalendar("DateEdit", caja18, ComboSubSubOption18.EditValue.ToString());
                    }
                    else
                    {
                        ConvertTextEdit("TextEdit", caja18);
                    }
                }
            }
            else
            {
                ConvertTextEdit("TextEdit", caja18);
            }
        }

        private void ComboSubSubOption19_EditValueChanged(object sender, EventArgs e)
        {
            if (ComboSubSubOption19.Properties.DataSource != null)
            {
                if (ComboSubSubOption19.EditValue != null)
                {
                    if (ComboSubSubOption19.EditValue.ToString() == "dd-mm-yyyy" || ComboSubSubOption19.EditValue.ToString() == "yyyy-mm-dd")
                    {
                        //CONVERTIR ULTIMA CAJA DE TEXTO EN DATEEDIT
                        ConvertEditorCalendar("DateEdit", caja19, ComboSubSubOption19.EditValue.ToString());
                    }
                    else
                    {
                        ConvertTextEdit("TextEdit", caja19);
                    }
                }
            }
            else
            {
                ConvertTextEdit("TextEdit", caja19);
            }
        }

        private void ComboSubSubOption20_EditValueChanged(object sender, EventArgs e)
        {
            if (ComboSubSubOption20.Properties.DataSource != null)
            {
                if (ComboSubSubOption20.EditValue != null)
                {
                    if (ComboSubSubOption20.EditValue.ToString() == "dd-mm-yyyy" || ComboSubSubOption20.EditValue.ToString() == "yyyy-mm-dd")
                    {
                        //CONVERTIR ULTIMA CAJA DE TEXTO EN DATEEDIT
                        ConvertEditorCalendar("DateEdit", caja20, ComboSubSubOption20.EditValue.ToString());
                    }
                    else
                    {
                        ConvertTextEdit("TextEdit", caja20);
                    }
                }
            }
            else
            {
                ConvertTextEdit("TextEdit", caja20);
            }
        }

        private void caja1_EditValueChanged(object sender, EventArgs e)
        {
           
        }

        private void ComboOption21_EditValueChanged(object sender, EventArgs e)
        {
            int pOption = 0;
            pOption = Convert.ToInt32(ComboOption21.EditValue);

            ConvertEditor("LookUpEdit", ComboSubOption21, pOption);

            OpcionesComboPrincipal(pOption, Caja21, ComboSubSubOption21);

            if (ComboSubOption21.Properties.DataSource != null)
                ComboSubOption21.ItemIndex = 0;

            if (ComboSubSubOption21.Properties.DataSource != null)
                ComboSubSubOption21.ItemIndex = 0;
        }

        private void ComboOption22_EditValueChanged(object sender, EventArgs e)
        {
            int pOption = 0;
            pOption = Convert.ToInt32(ComboOption22.EditValue);

            ConvertEditor("LookUpEdit", ComboSubOption22, pOption);

            OpcionesComboPrincipal(pOption, Caja22, ComboSubSubOption22);

            if (ComboSubOption22.Properties.DataSource != null)
                ComboSubOption22.ItemIndex = 0;

            if (ComboSubSubOption22.Properties.DataSource != null)
                ComboSubSubOption22.ItemIndex = 0;
        }

        private void ComboOption23_EditValueChanged(object sender, EventArgs e)
        {
            int pOption = 0;
            pOption = Convert.ToInt32(ComboOption23.EditValue);

            ConvertEditor("LookUpEdit", ComboSubOption23, pOption);

            OpcionesComboPrincipal(pOption, Caja23, ComboSubSubOption23);

            if (ComboSubOption23.Properties.DataSource != null)
                ComboSubOption23.ItemIndex = 0;

            if (ComboSubSubOption23.Properties.DataSource != null)
                ComboSubSubOption23.ItemIndex = 0;
        }

        private void ComboOption24_EditValueChanged(object sender, EventArgs e)
        {
            int pOption = 0;
            pOption = Convert.ToInt32(ComboOption24.EditValue);

            ConvertEditor("LookUpEdit", ComboSubOption24, pOption);

            OpcionesComboPrincipal(pOption, Caja24, ComboSubSubOption16);

            if (ComboSubOption24.Properties.DataSource != null)
                ComboSubOption24.ItemIndex = 0;

            if (ComboSubSubOption24.Properties.DataSource != null)
                ComboSubSubOption24.ItemIndex = 0;
        }

        private void ComboOption25_EditValueChanged(object sender, EventArgs e)
        {
            int pOption = 0;
            pOption = Convert.ToInt32(ComboOption25.EditValue);

            ConvertEditor("LookUpEdit", ComboSubOption25, pOption);

            OpcionesComboPrincipal(pOption, Caja25, ComboSubSubOption25);

            if (ComboSubOption25.Properties.DataSource != null)
                ComboSubOption25.ItemIndex = 0;

            if (ComboSubSubOption25.Properties.DataSource != null)
                ComboSubSubOption25.ItemIndex = 0;
        }

        private void ComboSubOption21_EditValueChanged(object sender, EventArgs e)
        {
            LookUpEdit Combo = sender as LookUpEdit;

            ChangeValueCombo(Combo, ComboSubSubOption21, ComboOption21, Caja21);
        }

        private void ComboSubOption22_EditValueChanged(object sender, EventArgs e)
        {
            LookUpEdit Combo = sender as LookUpEdit;

            ChangeValueCombo(Combo, ComboSubSubOption22, ComboOption22, Caja22);
        }

        private void ComboSubOption23_EditValueChanged(object sender, EventArgs e)
        {
            LookUpEdit Combo = sender as LookUpEdit;

            ChangeValueCombo(Combo, ComboSubSubOption23, ComboOption23, Caja23);
        }

        private void ComboSubOption24_EditValueChanged(object sender, EventArgs e)
        {
            LookUpEdit Combo = sender as LookUpEdit;

            ChangeValueCombo(Combo, ComboSubSubOption24, ComboOption24, Caja24);
        }

        private void ComboSubOption25_EditValueChanged(object sender, EventArgs e)
        {
            LookUpEdit Combo = sender as LookUpEdit;

            ChangeValueCombo(Combo, ComboSubSubOption25, ComboOption25, Caja25);
        }

        private void ComboSubSubOption21_EditValueChanged(object sender, EventArgs e)
        {
            if (ComboSubSubOption21.Properties.DataSource != null)
            {
                if (ComboSubSubOption21.EditValue != null)
                {
                    if (ComboSubSubOption21.EditValue.ToString() == "dd-mm-yyyy" || ComboSubSubOption21.EditValue.ToString() == "yyyy-mm-dd")
                    {
                        //CONVERTIR ULTIMA CAJA DE TEXTO EN DATEEDIT
                        ConvertEditorCalendar("DateEdit", Caja21, ComboSubSubOption21.EditValue.ToString());
                    }
                    else
                    {
                        ConvertTextEdit("TextEdit", Caja21);
                    }
                }
            }
            else
            {
                ConvertTextEdit("TextEdit", Caja21);
            }
        }

        private void ComboSubSubOption22_EditValueChanged(object sender, EventArgs e)
        {
            if (ComboSubSubOption22.Properties.DataSource != null)
            {
                if (ComboSubSubOption22.EditValue != null)
                {
                    if (ComboSubSubOption22.EditValue.ToString() == "dd-mm-yyyy" || ComboSubSubOption22.EditValue.ToString() == "yyyy-mm-dd")
                    {
                        //CONVERTIR ULTIMA CAJA DE TEXTO EN DATEEDIT
                        ConvertEditorCalendar("DateEdit", Caja22, ComboSubSubOption22.EditValue.ToString());
                    }
                    else
                    {
                        ConvertTextEdit("TextEdit", Caja22);
                    }
                }
            }
            else
            {
                ConvertTextEdit("TextEdit", Caja22);
            }
        }

        private void ComboSubSubOption23_EditValueChanged(object sender, EventArgs e)
        {
            if (ComboSubSubOption23.Properties.DataSource != null)
            {
                if (ComboSubSubOption23.EditValue != null)
                {
                    if (ComboSubSubOption23.EditValue.ToString() == "dd-mm-yyyy" || ComboSubSubOption23.EditValue.ToString() == "yyyy-mm-dd")
                    {
                        //CONVERTIR ULTIMA CAJA DE TEXTO EN DATEEDIT
                        ConvertEditorCalendar("DateEdit", Caja23, ComboSubSubOption23.EditValue.ToString());
                    }
                    else
                    {
                        ConvertTextEdit("TextEdit", Caja23);
                    }
                }
            }
            else
            {
                ConvertTextEdit("TextEdit", Caja23);
            }
        }

        private void ComboSubSubOption24_EditValueChanged(object sender, EventArgs e)
        {
            if (ComboSubSubOption24.Properties.DataSource != null)
            {
                if (ComboSubSubOption24.EditValue != null)
                {
                    if (ComboSubSubOption24.EditValue.ToString() == "dd-mm-yyyy" || ComboSubSubOption24.EditValue.ToString() == "yyyy-mm-dd")
                    {
                        //CONVERTIR ULTIMA CAJA DE TEXTO EN DATEEDIT
                        ConvertEditorCalendar("DateEdit", Caja24, ComboSubSubOption24.EditValue.ToString());
                    }
                    else
                    {
                        ConvertTextEdit("TextEdit", Caja24);
                    }
                }
            }
            else
            {
                ConvertTextEdit("TextEdit", Caja24);
            }
        }

        private void ComboSubSubOption25_EditValueChanged(object sender, EventArgs e)
        {
            if (ComboSubSubOption25.Properties.DataSource != null)
            {
                if (ComboSubSubOption25.EditValue != null)
                {
                    if (ComboSubSubOption25.EditValue.ToString() == "dd-mm-yyyy" || ComboSubSubOption25.EditValue.ToString() == "yyyy-mm-dd")
                    {
                        //CONVERTIR ULTIMA CAJA DE TEXTO EN DATEEDIT
                        ConvertEditorCalendar("DateEdit", Caja25, ComboSubSubOption25.EditValue.ToString());
                    }
                    else
                    {
                        ConvertTextEdit("TextEdit", Caja25);
                    }
                }
            }
            else
            {
                ConvertTextEdit("TextEdit", Caja25);
            }
        }
    }
}