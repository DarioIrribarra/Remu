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
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Menu;

namespace Labour
{
    public partial class frmOrdenBuilder : DevExpress.XtraEditors.XtraForm
    {
        /// <summary>
        /// Para guardar listado de campos que se generaron en la consulta sql.
        /// </summary>
        private List<formula> ListadoCampos = new List<formula>();

        /// <summary>
        /// Corresponde a listado para genera orderby que pasamos a través de interfaz.
        /// </summary>
        private List<Orden> ListadoOrderBy = new List<Orden>();

        /// <summary>
        /// Corresponde a todos los datos que usuario ya ordenó
        /// </summary>
        private List<Orden> ListOrderUsuario = new List<Orden>();

        public IGenerador Open { get; set; }

        public frmOrdenBuilder()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pFields">Listado de campos de la consulta</param>
        /// <param name="pListUsers">Listado ordenado por el usuario</param>
        public frmOrdenBuilder(List<formula> pFields, List<Orden> pListUsers)
        {
            InitializeComponent();
            this.ListadoCampos = pFields;
            this.ListOrderUsuario = pListUsers;
        }

        private void frmOrdenBuilder_Load(object sender, EventArgs e)
        {
            LoadGridControl(gridDatos);
        }

        /// <summary>
        /// En base al listado creamos un listado de objetos de la clase orden.
        /// </summary>
        private List<Orden> GeneraDataSource()
        {
            int count = 0;
            List<Orden> Data = new List<Orden>();
            if (this.ListadoCampos.Count > 0)
            {
                foreach (var field in this.ListadoCampos)
                {
                    count++;
                    Data.Add(new Orden() {Campo = field.key, Posicion = count});
                }
            }
            return Data;
        }

        /// <summary>
        /// Permite cargar el datasource correspondiente a un control gridcontrol.
        /// </summary>
        /// <param name="pGridControl"></param>
        private void LoadGridControl(DevExpress.XtraGrid.GridControl pGridControl)
        {
            List<Orden> DataSource = new List<Orden>();

            try
            {
                //Si listado tiene elementos es porque usuario realizó algun ordenamiento
                if (this.ListOrderUsuario.Count > 0)
                {
                    pGridControl.DataSource = this.ListOrderUsuario;
                    fnSistema.spOpcionesGrilla(viewDatos, true);
                    ColumnsProperties();
                }
                //CARGAMOS DATASOURCE POR DEFECTO
                else
                {
                    DataSource = GeneraDataSource();
                    if (DataSource.Count > 0)
                    {
                        pGridControl.DataSource = DataSource;
                        fnSistema.spOpcionesGrilla(viewDatos, true);
                        ColumnsProperties();
                    }
                }             
            }
            catch (Exception ex)
            {
                //ERROR...
            }
        }

        /// <summary>
        /// Genera listado para combobox de posiciones.
        /// </summary>
        /// <param name="pCombo"></param>
        /// <returns></returns>
        private List<PositionC> LoadComboPositions()
        {
            List<PositionC> listado = new List<PositionC>();
            if (this.ListadoCampos.Count > 0)
            {
                int max = 0, ini = 1;
                max = ListadoCampos.Count;

                while (ini <= max)
                {
                    listado.Add(new PositionC() { id = ini, number = ini});
                    ini++;
                }
            }

            return listado;
        }

        /// <summary>
        /// Datasource para combo ascendente, en el caso de usar algun campo como order by.
        /// </summary>
        /// <returns></returns>
        private List<formula> LoadComboType()
        {
            List<formula> listado = new List<formula>();

            listado.Add(new formula() { desc = "Descendente", key = "DESC"});
            listado.Add(new formula() { desc = "Ascendente", key = "ASC"});

            return listado;
        }       

        /// <summary>
        /// Agregamos Repositorio lookupedit y dejamos editables columna posicion, alias y visibilidad
        /// </summary>
        private void ColumnsProperties()
        {
            if (gridDatos.DataSource != null)
            {              
                RepositoryItemLookUpEdit Repo1 = new RepositoryItemLookUpEdit();
                RepositoryItemLookUpEdit Repo2 = new RepositoryItemLookUpEdit();

                //PARA COMBO ORDENAMIENTO
                Repo1.DataSource = LoadComboPositions();
                Repo1.ValueMember = "id";
                Repo1.DisplayMember = "number";
                Repo1.PopulateColumns();

                //PARA COMBO TYPE
                Repo2.DataSource = LoadComboType();                                
                Repo2.ValueMember = "key";
                Repo2.DisplayMember = "desc";
                Repo2.PopulateColumns();               

                Repo1.Columns[0].Visible = false;
                Repo2.Columns[0].Visible = false;

                gridDatos.RepositoryItems.Add(Repo1);
                gridDatos.RepositoryItems.Add(Repo2);

                viewDatos.Columns[1].ColumnEdit = Repo1;
                viewDatos.Columns[5].ColumnEdit = Repo2;

                viewDatos.Columns[0].OptionsColumn.AllowEdit = false;

                SetDefaultValue();
            }
        }

        /// <summary>
        /// Genera un listado de acuerdo a orden que dio el usuario a los datos y otras propiedades.
        /// </summary>
        private List<Orden> GetOrderData()
        {
            List<Orden> Ordenados = new List<Orden>();
            List<Orden> Reorden = new List<Orden>();
            if (gridDatos.DataSource != null)
            {
                if (viewDatos.RowCount > 0)
                {
                    //ORDENAR DATOS POR POSICION
                    try
                    {
                        //DE MENOR A MAYOR
                       // viewDatos.Columns[1].SortOrder = DevExpress.Data.ColumnSortOrder.Descending;

                        //Obtenemos un nuevo listado, esta vez ordenamos
                        for (int i = 0; i < viewDatos.DataRowCount; i++)
                        {
                            Ordenados.Add(new Orden()
                            {
                                visible = (bool)viewDatos.GetRowCellValue(i, "visible"),
                                Alias = viewDatos.GetRowCellValue(i, "Alias") == null ? "":(string)viewDatos.GetRowCellValue(i, "Alias"),
                                Campo = (string)viewDatos.GetRowCellValue(i, "Campo"),
                                Posicion = Convert.ToInt32(viewDatos.GetRowCellValue(i, "Posicion")),
                                OrderBy = (bool)viewDatos.GetRowCellValue(i, "OrderBy"),
                                OrderType = (string)viewDatos.GetRowCellValue(i, "OrderType")
                            });
                        }

                        //Ordena los elementos de menor a mayor.
                        Reorden = (from x in Ordenados
                                                  orderby x.Posicion
                                                  select x).ToList();

                        //Guardamos data ordenada en listado
                        ListadoOrderBy = Reorden;

                    }
                    catch (Exception ex)
                    {
                        //ERROR...
                    }
                }
                 
            }

            return Reorden;
        }

        /// <summary>
        /// valor por defecto para columna ordertype
        /// </summary>
        private void SetDefaultValue()
        {
            if (viewDatos.RowCount > 0)
            {
                try
                {
                    //DE MENOR A MAYOR
                    // viewDatos.Columns[1].SortOrder = DevExpress.Data.ColumnSortOrder.Descending;

                    //Obtenemos un nuevo listado, esta vez ordenamos
                    for (int i = 0; i < viewDatos.DataRowCount; i++)
                    {
                        viewDatos.SetRowCellValue(i, "OrderType", "DESC");
                    }              

                }
                catch (Exception ex)
                {
                    //ERROR...
                }
            }
        }

        /// <summary>
        /// Genera cadena final con campos ordenados.
        /// <para>Esta cadena la reemplazaremos en sql dinamico.</para>
        /// </summary>
        /// <returns></returns>
        private string GeneraCadenaOrden()
        {
            List<Orden> Ordenados = new List<Orden>();
            Ordenados = GetOrderData();
            StringBuilder buil = new StringBuilder();
            
            try
            {
                if (Ordenados.Count > 0)
                {
                    //Guardamos en lista para devolver por la interfaz.
                    ListadoOrderBy = Ordenados;

                    foreach (Orden x in Ordenados)
                    {
                        if (x.Alias.Length > 0)
                        {
                            if (x.visible)
                            {
                                buil.AppendLine($"{x.Campo} as '{x.Alias}',");
                            }
                        }
                        else
                        {
                            if(x.visible)
                                buil.AppendLine(x.Campo + ",");                            
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //ERROR...
            }

            return buil.ToString().Substring(0, buil.ToString().Length - 3);
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string data = "";
            List<Orden> ordenados = new List<Orden>();
            ordenados = GetOrderData();
            data = Orden.GeneraCadenaOrden(ordenados);
            if (data.Length > 0)
            {
                if (this.Open != null)
                {
                    Open.ReordenSql(data, ListadoOrderBy);
                    XtraMessageBox.Show("Ordenamiento guardado correctamente", "Orden", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Close();
                }
                else
                {
                    XtraMessageBox.Show("No se pudo guardar orden", "Orden", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
            else
            {
                XtraMessageBox.Show("No se pudo guardar orden", "Orden", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void viewDatos_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
         
           
            
            
        }
    }
}