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
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Columns;
using System.Collections;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace Labour
{
    public partial class frmSeleccionMultiple : DevExpress.XtraEditors.XtraForm
    {
        public ISeleccionMultiple Opener { get; set; }
        /// <summary>
        /// listado de contratos para mostrar en seleccion grilla.
        /// </summary>
        public List<ItemTrabajador> ListadoSeleccion { get; set; } = new List<ItemTrabajador>();

        /// <summary>
        /// Listado con nuevos valores a ingresar.
        /// </summary>
        public List<ItemTrabajador> ListadoNuevosValores { get; set; } = new List<ItemTrabajador>();

       RepositoryItemLookUpEdit ComboL = new RepositoryItemLookUpEdit();

        Dictionary<int, RepositoryItemLookUpEdit> Combos = new Dictionary<int, RepositoryItemLookUpEdit>();
        private int CountKey = 0;

        /// <summary>
        /// Indica que se esta trabajando desde el formulario de carga extendida
        /// </summary>
        private bool CargaExtend = false;

        public frmSeleccionMultiple(List<ItemTrabajador> pListado, List<ItemTrabajador> pListadoValores, bool Extendida)
        {
            InitializeComponent();
            ListadoSeleccion = pListado;
            ListadoNuevosValores = pListadoValores;
            CargaExtend = Extendida;
        }

        public frmSeleccionMultiple(List<ItemTrabajador> pListado, List<ItemTrabajador> pListadoValores)
        {
            InitializeComponent();
            ListadoSeleccion = pListado;
            ListadoNuevosValores = pListadoValores;
        }
        public frmSeleccionMultiple()
        {
            InitializeComponent();
        }

        private void frmSeleccionMultiple_Load(object sender, EventArgs e)
        {
            DataTable Tabla = new DataTable();
            if (ListadoSeleccion.Count > 0)
            {
                //CARGAMOS GRILLA EN BASE A LISTADO PROPORCIONADO POR CONSTRUCTOR DE CLASE               
                opcionesGrid();             
                gridSeleccion.DataSource = ListadoSeleccion;
                //Permite editar cierta columnas del gridcontrol
                AllowEdit(CargaExtend);
                //Visible o no columns
                opcionesColumnas(CargaExtend);

                CountKey = 0;
                //SetComboIndex(0);

            }
        }

        #region "DATOS"
        /// <summary>
        /// Genera un DataTable para cargar un GridControl.
        /// </summary>
        /// <param name="pContratos">Listado de contratos</param>
        /// <param name="pItems">Listado de Items</param>
        /// <param name="pPeriodo">Periodo de busqueda.</param>
        private DataTable QuerySeleccion(List<ItemTrabajador> pListado, int pPeriodo)
        {
            string sql = "select numitem, itemtrabajador.contrato,  CONCAT(nombre, ' ', apepaterno, ' ', apematerno) as nombre, " +
                         "coditem, valor, valorcalculado \n" +
                         "FROM itemtrabajadOR \n" +
                         "INNER JOIN trabajador On trabajador.contrato = itemtrabajador.contrato \n" +
                         "AND trabajador.anomes = itemtrabajador.anomes \n" +
                         "WHERE itemtrabajador.anomes = @pPeriodo {items} {contratos} \n" +
                         "ORDER BY apepaterno, contrato \n";
            string sqlItems = "AND (", sqlContratos = " AND (";
            int count = 0;

            if (pListado.Count > 0)
            {
                //ITEMS
                foreach (var x in pListado)
                {                    

                    if(count < pListado.Count)
                        sqlItems = sqlItems + $"coditem='{x.item}' OR ";
                    else
                        sqlItems = sqlItems + $"coditem='{x.item}' ";

                    count++;
                }

                sqlItems = sqlItems + ")";
                sql = sql.Replace("{items}", sqlItems);

                count = 0;
                //CONSTRATOS
                foreach (var x in pListado)
                {
                    if (count < pListado.Count)
                        sqlContratos = sqlContratos + $"contrato='{x.contrato}' OR \n";
                    else
                        sqlContratos = sqlContratos + $"contrato='{x.contrato}' \n";

                    count++;
                }

                sqlContratos = sqlContratos + ")";
                sql = sql.Replace("{contratos" +
                    "}", sqlContratos);
            }

            SqlConnection cn;
            SqlCommand cmd;
            SqlDataAdapter ad = new SqlDataAdapter();
            DataSet ds = new DataSet();
            DataTable tabla = new DataTable();
            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            cmd.Parameters.Add(new SqlParameter("@pPeriodo", pPeriodo));
                            ad.SelectCommand = cmd;

                            ad.Fill(ds, "data");

                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                tabla = ds.Tables[0];
                            }

                            cmd.Dispose();
                            ds.Dispose();
                            ad.Dispose();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
              //ERROR...
            }

            return tabla;
        }

        //PROPIEDADES GRID
        private void opcionesGrid()
        {
            //setear propiedades de la grilla
            viewSeleccion.OptionsSelection.EnableAppearanceHideSelection = false;
            //viewSeleccion.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFullFocus;
            //viewSeleccion.OptionsBehavior.Editable = true;
            viewSeleccion.OptionsSelection.EnableAppearanceFocusedCell = false;
            //PARA LA SELECCION USANDO CHECKBOX
            viewSeleccion.OptionsSelection.MultiSelect = true;
            viewSeleccion.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;

            //deshabilitar menus contextuales
            viewSeleccion.OptionsMenu.EnableColumnMenu = false;
            viewSeleccion.OptionsMenu.EnableFooterMenu = false;
            viewSeleccion.OptionsMenu.EnableGroupPanelMenu = false;

            //evitar filtrar por columnas y Ordenar por Columnas
            viewSeleccion.OptionsCustomization.AllowFilter = false;
            viewSeleccion.OptionsCustomization.AllowGroup = false;
            viewSeleccion.OptionsCustomization.AllowSort = false;
            viewSeleccion.OptionsCustomization.AllowColumnResizing = false;
            viewSeleccion.OptionsCustomization.AllowColumnMoving = false;

            //deshabilitar cabezera de la tabla
            viewSeleccion.OptionsView.ShowGroupPanel = false;          
        }

        private void AddRepo()
        {
            List<Opcion> listado = new List<Opcion>();

            int count = 0;                
            gridSeleccion.DataSource = ListadoSeleccion;
            
          
        }

        //PROPIEDADES COLUMNAS
        private void opcionesColumnas(bool CargaExtendidad)
        {
            try
            {
                //Numero de item
                viewSeleccion.Columns[0].Caption = "N°";
                viewSeleccion.Columns[0].Width = 50;
                
                //Rut
                viewSeleccion.Columns[1].Visible = false;

                //Contrato
                viewSeleccion.Columns[2].Caption = "Contrato";
                //Orden
                viewSeleccion.Columns[3].Visible = false;

                //ITEM
                viewSeleccion.Columns[4].Caption = "Item";
                viewSeleccion.Columns[5].Visible = false;

                //descripcion
                viewSeleccion.Columns[6].Visible = false;

                //Valor original
                viewSeleccion.Columns[7].Caption = "Valor";

                //formula
                viewSeleccion.Columns[8].Visible = CargaExtendidad == true ? true: false;
                //esclase
                viewSeleccion.Columns[9].Visible = false;
                //proporcional
                viewSeleccion.Columns[10].Visible = CargaExtendidad == true ? true : false;
                //permanenete
                viewSeleccion.Columns[11].Visible = CargaExtendidad == true ? true : false;
                //contope
                viewSeleccion.Columns[12].Visible = false;
                //aplicauf
                viewSeleccion.Columns[13].Visible = CargaExtendidad == true ? true : false;
                //aplica pesos
                viewSeleccion.Columns[14].Visible = CargaExtendidad == true ? true : false;
                //Porcentaje
                viewSeleccion.Columns[15].Visible = CargaExtendidad == true ? true : false;
                //periodo
                viewSeleccion.Columns[16].Visible = false;
                //VALOR CALCULADO
                viewSeleccion.Columns[17].Caption = "Valor calculado";
                //adicional
                viewSeleccion.Columns[18].Caption = "Nuevo Valor";
                //Usa formula
                viewSeleccion.Columns[19].Visible = false;
                //Expresion Formula
                viewSeleccion.Columns[20].Visible = false;
                //Imponible Anterior
                viewSeleccion.Columns[21].Visible = false;
                //Informativo
                viewSeleccion.Columns[22].Visible = false;
                //Existe sobregiro anterior
                viewSeleccion.Columns[23].Visible = false;
                //Monto sobregiro anterior
                viewSeleccion.Columns[24].Visible = false;
                //Existe sobregiro actual
                viewSeleccion.Columns[25].Visible = false;
                //Last number item
                viewSeleccion.Columns[26].Visible = false;
                //Monto afc
                viewSeleccion.Columns[27].Visible = false;
                //cuota
                viewSeleccion.Columns[28].Visible = CargaExtendidad == true ? true : false;
             

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }        

            
        }

        //OBTENER LAS FILAS SELECCIONADAS
        private int[] FilasSeleccionadas()
        {
            if (viewSeleccion.RowCount > 0)
            {
                //OBTENEMOS LAS FILAS SELECIONADAS
                int[] selecionadas = viewSeleccion.GetSelectedRows();

                return selecionadas;
            }
            else
            {
                return null;
            }
        }

        //OBTENER DATOS ITEM DE LAS FILAS SELECIONADAS
        private List<ItemTrabajador> getidRegistros(int[] rows, bool Extendida)
        {
            //List<int> registros = new List<int>();
            List<ItemTrabajador> regs = new List<ItemTrabajador>();

            if (rows.Length > 0)
            {
                //RECORREMOS ARRAY
                for (int i = 0; i < rows.Length; i++)
                {
                    //  registros.Add(Convert.ToInt32(viewSeleccion.GetRowCellValue(rows[i], "id")));

                    if (!Extendida)
                    {
                        regs.Add(new ItemTrabajador()
                        {
                            NumeroItem = Convert.ToInt32(viewSeleccion.GetRowCellValue(rows[i], "NumeroItem")),
                            item = (string)viewSeleccion.GetRowCellValue(rows[i], "item"),
                            contrato = (string)viewSeleccion.GetRowCellValue(rows[i], "contrato"),
                            Rut = (string)viewSeleccion.GetRowCellValue(rows[i], "Rut"),
                            //Adicional corresponde al nuevo valor seleccionado del combo box
                            Adicional = Convert.ToDouble(viewSeleccion.GetRowCellDisplayText(rows[i], "Adicional"))


                        });
                    }
                    else
                    {
                        regs.Add(new ItemTrabajador()
                        {
                            NumeroItem = Convert.ToInt32(viewSeleccion.GetRowCellValue(rows[i], "NumeroItem")),
                            item = (string)viewSeleccion.GetRowCellValue(rows[i], "item"),
                            contrato = (string)viewSeleccion.GetRowCellValue(rows[i], "contrato"),
                            Rut = (string)viewSeleccion.GetRowCellValue(rows[i], "Rut"),
                            //Adicional corresponde al nuevo valor seleccionado del combo box
                            Adicional = Convert.ToDouble(viewSeleccion.GetRowCellDisplayText(rows[i], "Adicional")), 
                            proporcional = Convert.ToBoolean(viewSeleccion.GetRowCellDisplayText(rows[i], "proporcional")),
                            AplicaUf = Convert.ToBoolean(viewSeleccion.GetRowCellDisplayText(rows[i], "uf")),
                            AplicaPesos = Convert.ToBoolean(viewSeleccion.GetRowCellDisplayText(rows[i], "pesos")),
                            AplicaPorcentaje = Convert.ToBoolean(viewSeleccion.GetRowCellDisplayText(rows[i], "porc")),
                            formula = Convert.ToString(viewSeleccion.GetRowCellDisplayText(rows[i], "formula")),
                            permanente = Convert.ToBoolean(viewSeleccion.GetRowCellDisplayText(rows[i], "permanente"))                          //Adicionar--> 

                        });
                    }
                    
                }

                if (regs.Count > 0) return regs;
                else
                    return null;
            }
            else
                return null;
        }

        private item GetInfoItem(string pCoditem)
        {
            item obj = new item();
            string sql = "SELECT formula, tipo, orden FROM item WHERE coditem=@pCoditem";
            SqlDataReader rd;
            SqlCommand cmd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pCoditem", pCoditem));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //GUARDAMOS DATOS EN OBJETO
                                obj.formula = (string)rd["formula"];
                                obj.tipo = Convert.ToInt32(rd["tipo"]);
                                obj.orden = Convert.ToInt32(rd["orden"]);
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

            return obj;
        }

        /// <summary>
        /// Genera dos consultas (INSERT Y UPDATE)
        /// </summary>
        private string GetSql(List<ItemTrabajador> pListado, bool CargaExterna)
        {
            string sqlUpdate = "";
            item Objeto = new item();
            string value = "";
            if (pListado.Count > 0)
            {
                if (!CargaExterna)
                {
                    foreach (var x in pListado)
                    {
                        if (x.Adicional.ToString().Contains(","))
                            value = x.Adicional.ToString().Replace(",", ".");
                        else
                            value = x.Adicional.ToString();

                        sqlUpdate = sqlUpdate + $"UPDATE ITEMTRABAJADOR SET valor={value} WHERE contrato='{x.contrato}' AND " +
                                                  $"anomes={Calculo.PeriodoObservado} AND coditem='{x.item}'" +
                                                  $" AND numitem={x.NumeroItem}\n";
                    }
                }
                else
                {
                    //Agregamos los otros campos, porque es formulario extendido
                    foreach (var x in pListado)
                    {
                        if (x.Adicional.ToString().Contains(","))
                            value = x.Adicional.ToString().Replace(",", ".");
                        else
                            value = x.Adicional.ToString();                        

                        sqlUpdate = sqlUpdate + $"UPDATE ITEMTRABAJADOR SET valor={value}, proporcional={x.proporcional}, permanente={x.permanente}, " +
                                                $"uf={x.AplicaUf}, pesos={x.AplicaPesos}, porc={x.AplicaPorcentaje}, formula={x.formula} " +
                                                 $" WHERE contrato='{x.contrato}' AND " +
                                                  $"anomes={Calculo.PeriodoObservado} AND coditem='{x.item}'" +
                                                  $" AND numitem={x.NumeroItem}\n";
                    }
                }
               
            }

            return sqlUpdate;
        }

        /// <summary>
        /// Deja seleccionado un item de acuerdo a posicion seleccionada
        /// </summary>
        /// <param name="pIndex"></param>
        private void SetComboIndex(int pIndex)
        {
            if (viewSeleccion.RowCount > 0)
            {

                
                for (int i = 0; i < viewSeleccion.RowCount; i++)
                {
                    for (int j = 0; j < viewSeleccion.Columns.Count; j++)
                    {
                        //GridColumn column = new GridColumn();
                        GridColumnCollection collection = viewSeleccion.Columns;
                        if (viewSeleccion.Columns[j].FieldName == "Adicional")
                        {
                            //DEJAMOS SELECCIONADO ITEMS DEL COMBOBOX
                            int c = viewSeleccion.ViewRepository.Views.Count;
                            object data = viewSeleccion.Columns[j].RealColumnEdit;
                            GridViewInfo info = (GridViewInfo)viewSeleccion.GetViewInfo();
                            int pos = viewSeleccion.Columns[j].AbsoluteIndex;
                            GridColumn col = collection.ColumnByFieldName(viewSeleccion.Columns[j].FieldName);

                            //GridDataRowInfo rr = info.RowsInfo.GetInfoByHandle(i) as GridDataRowInfo;
                            //info.CreateCellInfo(rr, info.ColumnsInfo[col]);
                            //GridCellInfo celda = info.GetGridCellInfo(i, col);
                            viewSeleccion.SetRowCellValue(i, viewSeleccion.Columns["Adicional"],  ComboL.GetDataSourceValue(ComboL.ValueMember, pIndex));

                           
                        }

                        if (CargaExtend)
                        {
                            if(viewSeleccion.Columns[j].FieldName.ToLower() == "cuota")
                                viewSeleccion.SetRowCellValue(i, viewSeleccion.Columns["Cuota"], ComboL.GetDataSourceValue(ComboL.ValueMember, pIndex));
                            if (viewSeleccion.Columns[j].FieldName.ToLower() == "formula")
                                viewSeleccion.SetRowCellValue(i, viewSeleccion.Columns["Formula"], ComboL.GetDataSourceValue(ComboL.ValueMember, pIndex));
                        }
                    }
                }
            }
        }

        private void AllowEdit(bool CargaExtend)
        {

            if (viewSeleccion.RowCount > 0)
            {
                for (int i = 0; i < viewSeleccion.RowCount; i++)
                {
                    for (int j = 0; j < viewSeleccion.Columns.Count; j++)
                    {
                        if (viewSeleccion.Columns[j].FieldName.ToLower() == "adicional")
                            viewSeleccion.Columns[j].OptionsColumn.AllowEdit = true;
                        else
                            viewSeleccion.Columns[j].OptionsColumn.AllowEdit = false;


                        //Habilitar edicion para 
                        //Formula, proporcional, uf, pesos, porcentaje, permanente, cuota
                        if (CargaExtend)
                        {
                            viewSeleccion.Columns[j].OptionsColumn.AllowEdit = false;

                            if (viewSeleccion.Columns[j].FieldName.ToLower() == "formula")
                                viewSeleccion.Columns[j].OptionsColumn.AllowEdit = true;
                            if (viewSeleccion.Columns[j].FieldName.ToLower() == "proporcional")
                                viewSeleccion.Columns[j].OptionsColumn.AllowEdit = true;
                            if (viewSeleccion.Columns[j].FieldName.ToLower() == "aplicauf")
                                viewSeleccion.Columns[j].OptionsColumn.AllowEdit = true;
                            if (viewSeleccion.Columns[j].FieldName.ToLower() == "aplicapesos")
                                viewSeleccion.Columns[j].OptionsColumn.AllowEdit = true;
                            if (viewSeleccion.Columns[j].FieldName.ToLower() == "aplicaporcentaje")
                                viewSeleccion.Columns[j].OptionsColumn.AllowEdit = true;
                            if (viewSeleccion.Columns[j].FieldName.ToLower() == "permanente")
                                viewSeleccion.Columns[j].OptionsColumn.AllowEdit = true;
                            if (viewSeleccion.Columns[j].FieldName.ToLower() == "cuota")
                                viewSeleccion.Columns[j].OptionsColumn.AllowEdit = true;
                            if (viewSeleccion.Columns[j].FieldName.ToLower() == "adicional")
                                viewSeleccion.Columns[j].OptionsColumn.AllowEdit = true;
                        }

                    }
                }
            }
        }

        #endregion

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            List<ItemTrabajador> ListadoSeleccionado = new List<ItemTrabajador>();
            string Data = "";
            if (viewSeleccion.RowCount > 0)
            {
                //generamos listado de items de acuerdo a filas seleccionadas
                ListadoSeleccionado = getidRegistros(FilasSeleccionadas(), CargaExtend);

                if (ListadoSeleccionado == null)
                { XtraMessageBox.Show("No seleccionaste ninguna fila", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Warning); }

                if (ListadoSeleccionado != null)
                {
                    if (ListadoSeleccionado.Count > 0)
                    {
                        Data = GetSql(ListadoSeleccionado, CargaExtend);

                        //PASAMOS SELECCION DE VUELTA AL FORMULARIO QUE INVOCÓ LA SELECCION...
                        if (Opener != null)
                        {
                            Opener.CargaListado(Data);
                            Close();
                        }
                        else
                        {
                            XtraMessageBox.Show("No se pudo guardar seleccion", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            Close();
                        }
                    }
                    else
                    {
                        XtraMessageBox.Show("Error al seleccionar filas", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    } 
                }
            }
            else
            {
                XtraMessageBox.Show("Seleccion no válida", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void PopulateCombo(RepositoryItemLookUpEdit pCombo, int pRowHandle, List<ItemTrabajador> pListado, string pContrato, string pOption)
        {
            List<Opcion> Listado = new List<Opcion>();
            int count = 0;

            if (pOption == "adicional")
            {
                foreach (var item in pListado)
                {
                    if (item.contrato == pContrato)
                    {
                        Listado.Add(new Opcion() { Id = count++, Descripcion = item.Adicional.ToString() });
                    }
                }

                pCombo.DataSource = Listado.ToList();
                pCombo.ValueMember = "Id";
                pCombo.DisplayMember = "Descripcion";
                pCombo.KeyMember = "Id";
                
            }

            if (pOption == "formula")
            {
                foreach (var item in pListado)
                {
                    if (item.contrato == pContrato)
                    {
                        Listado.Add(new Opcion() { Id = count++, Descripcion = item.formula.ToString() });
                    }
                }

                pCombo.DataSource = Listado.ToList();
                pCombo.ValueMember = "Id";
                pCombo.DisplayMember = "Descripcion";
                pCombo.KeyMember = "Id";
            }

            if (pOption == "cuota")
            {
                foreach (var item in pListado)
                {
                    if (item.contrato == pContrato)
                    {
                        Listado.Add(new Opcion() { Id = count++, Descripcion = item.Cuota.ToString() });
                    }
                }

                pCombo.DataSource = Listado.ToList();
                pCombo.ValueMember = "Id";
                pCombo.DisplayMember = "Descripcion";
                pCombo.KeyMember = "Id";
            }



        }



        /// <summary>
        /// Para agregar los combos a la grilla
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void viewSeleccion_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            string contrato = "";
            contrato = viewSeleccion.GetRowCellValue(e.RowHandle, "contrato").ToString();
           

            if (!CargaExtend)
            {
                if (e.Column.FieldName.ToLower() == "adicional")
                {
                    if (Combos.ContainsKey(e.RowHandle))
                    {
                        e.RepositoryItem = Combos[e.RowHandle];
                    }
                    else
                    {
                        RepositoryItemLookUpEdit NewCombo = new RepositoryItemLookUpEdit();
                        PopulateCombo(NewCombo, e.RowHandle, ListadoNuevosValores, contrato, "adicional");
                        Combos.Add(e.RowHandle, NewCombo);
                        e.RepositoryItem = NewCombo;
                    }

                    //viewSeleccion.SetRowCellValue(e.RowHandle, viewSeleccion.Columns["Adicional"], ComboL.GetDataSourceValue(ComboL.ValueMember, e.RowHandle));
                }
            }
            else
            {
                if (e.Column.FieldName.ToLower() == "adicional")
                {

                    RepositoryItemLookUpEdit NewCombo = new RepositoryItemLookUpEdit();
                    PopulateCombo(NewCombo, e.RowHandle, ListadoNuevosValores, contrato, "adicional");
                    Combos.Add(CountKey, NewCombo);
                    e.RepositoryItem = NewCombo;
                    CountKey++;
                    //SetComboIndex(0);
                    

                    //viewSeleccion.SetRowCellValue(e.RowHandle, viewSeleccion.Columns["Adicional"], ComboL.GetDataSourceValue(ComboL.ValueMember, e.RowHandle));
                }

                if (e.Column.FieldName.ToLower() == "formula")
                {

                    RepositoryItemLookUpEdit NewCombo = new RepositoryItemLookUpEdit();
                    PopulateCombo(NewCombo, e.RowHandle, ListadoNuevosValores, contrato, "formula");
                    Combos.Add(CountKey, NewCombo);
                    e.RepositoryItem = NewCombo;
                    CountKey++;                 


                    //viewSeleccion.SetRowCellValue(e.RowHandle, viewSeleccion.Columns["Adicional"], ComboL.GetDataSourceValue(ComboL.ValueMember, e.RowHandle));
                }

                if (e.Column.FieldName.ToLower() == "cuota")
                {

                    RepositoryItemLookUpEdit NewCombo = new RepositoryItemLookUpEdit();
                    PopulateCombo(NewCombo, e.RowHandle, ListadoNuevosValores, contrato, "cuota");
                    Combos.Add(CountKey, NewCombo);
                    e.RepositoryItem = NewCombo;
                    CountKey++;


                    //viewSeleccion.SetRowCellValue(e.RowHandle, viewSeleccion.Columns["Adicional"], ComboL.GetDataSourceValue(ComboL.ValueMember, e.RowHandle));
                }

               
            }
           

            //}

            //NO DEJAR EDITAR LAS CELDAS A EXCEPCION DEL COMBO
            //GridColumnCollection cols = viewSeleccion.Columns;
            //foreach (GridColumn item in cols)
            //{
            //    if (item.FieldName != "Adicional")
            //        item.OptionsColumn.AllowEdit = false;
            //}
        }

        private void gridSeleccion_Load(object sender, EventArgs e)
        {

        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void gridSeleccion_Load_1(object sender, EventArgs e)
        {
          //  SetComboIndex(0);
        }

        private void viewSeleccion_Layout(object sender, EventArgs e)
        {
            
        }
    }
}