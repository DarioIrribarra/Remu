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
    public partial class frmSeleccionItemElimina : DevExpress.XtraEditors.XtraForm
    {

        //PARA OBTENER EL FIRLTRO DEL USUARIO LOGUEADO
        private string FiltroUsuario { get; set; } = User.GetUserFilter();

        //PARA GUARDAR EL ITEM QUE SE DESEA MANIPULAR
        private string ItemTrabajador = "";

        //PARA SABER SI ES ELIMINACION O MODIFICACION
        private string tipo = "";

        //CONJUNTO BUSQUEDA
        private string ConjuntoBusqueda = "";

        //PARA COMUNICACION CON FORMULARIO PADRE
        public ISeleccionItemElimina opener { get; set; }

        public frmSeleccionItemElimina()
        {
            InitializeComponent();
        }

        public frmSeleccionItemElimina(string Item, string tipo, string pConjunto)
        {
            InitializeComponent();
            this.ItemTrabajador = Item;
            this.tipo = tipo;
            ConjuntoBusqueda = pConjunto;
        }            

        private void frmSeleccionItemElimina_Load(object sender, EventArgs e)
        {
            if (ItemTrabajador != "")
            {
                if (tipo == "elimina") lblTitulo.Text = "Seleccione items a eliminar";
                else if (tipo == "modifica") lblTitulo.Text = "Seleccione items a modificar";

                CargarGrid(ItemTrabajador, Calculo.PeriodoObservado, ConjuntoBusqueda);
            }
            else
            {
                XtraMessageBox.Show("Item seleccionado no valido", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        #region "MANEJO DATOS"

        private string PreparaCondicion(string condicion)
        {
            if (condicion != "")
            {
                condicion = condicion.ToLower();
                if (condicion.Contains("contrato"))
                    condicion = condicion.Replace("contrato", "itemtrabajador.contrato");
                if (condicion.Contains("rut"))
                    condicion = condicion.Replace("rut", "itemtrabajador.rut");
                if (condicion.Contains("anomes"))
                    condicion = condicion.Replace("anomes", "itemtrabajador.anomes");

                return condicion;
            }
            else
                return "";
        }

        //CARGAR GRID
        private void CargarGrid(string pItem, int periodo, string pCodeConjunto)
        {
            string sql = "";
            string condUser = "";

            if (FiltroUsuario == "0")
            {                
                if(pCodeConjunto == "")
                    sql = string.Format("select itemTrabajador.contrato, concat(nombre, ' ', apepaterno, ' ', apematerno) as nombre," +
                       " coditem, valor, valorcalculado, numitem " +
                       " FROM itemtrabajador INNER JOIN trabajador ON trabajador.contrato = itemTrabajador.contrato" +
                       " AND trabajador.anomes = itemTrabajador.anomes " +
                       " WHERE itemtrabajador.anomes={0} AND coditem ='{1}'", periodo, pItem);
                else
                    sql = string.Format("select itemTrabajador.contrato, concat(nombre, ' ', apepaterno, ' ', apematerno) as nombre," +
                      " coditem, valor, valorcalculado, numitem " +
                      " FROM itemtrabajador INNER JOIN trabajador ON trabajador.contrato = itemTrabajador.contrato" +
                      " AND trabajador.anomes = itemTrabajador.anomes " +
                      " WHERE itemtrabajador.anomes={0} AND coditem ='{1}' AND itemTrabajador.contrato IN (SELECT contrato FROM trabajador WHERE {2})", periodo, pItem, Conjunto.GetCondicionFromCode(pCodeConjunto));

            }               
            else
            {
                condUser = Conjunto.GetCondicionFromCode(condUser);
                
                if(pCodeConjunto != "")
                sql = string.Format("select itemTrabajador.contrato, concat(nombre, ' ', apepaterno, ' ', apematerno) as nombre," +
                        " coditem, valor, valorcalculado, numitem " +
                        " FROM itemtrabajador INNER JOIN trabajador ON trabajador.contrato = itemTrabajador.contrato" +
                        " AND trabajador.anomes = itemTrabajador.anomes " +
                        " WHERE itemtrabajador.anomes={0} AND coditem ='{1}' AND {2} AND itemTrabajador.contrato IN (SELECT contrato FROM trabajador WHERE {3})", periodo, pItem, PreparaCondicion(condUser), Conjunto.GetCondicionFromCode(pCodeConjunto));
                else
                    sql = string.Format("select itemTrabajador.contrato, concat(nombre, ' ', apepaterno, ' ', apematerno) as nombre," +
                        " coditem, valor, valorcalculado, numitem " +
                        " FROM itemtrabajador INNER JOIN trabajador ON trabajador.contrato = itemTrabajador.contrato" +
                        " AND trabajador.anomes = itemTrabajador.anomes " +
                        " WHERE itemtrabajador.anomes={0} AND coditem ='{1}' AND {2}", periodo, pItem, PreparaCondicion(condUser));
            }                

            SqlDataAdapter ad = new SqlDataAdapter();
            SqlCommand cmd;
            DataSet ds = new DataSet();

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        ad.SelectCommand = cmd;
                        ad.Fill(ds, "data");

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                        ad.Dispose();

                        if (ds.Tables[0].Rows.Count>0)
                        {
                            //LLENAMOS GRILLA
                            gridSeleccion.DataSource = ds.Tables[0];                     
                            //gridSeleccion.DataMember = "data";                            
                            opcionesGrid();
                            opcionesColumnas();
                           
                        }
                        
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        //PROPIEDADES GRID
        private void opcionesGrid()
        {
            //setear propiedades de la grilla
            viewSeleccion.OptionsSelection.EnableAppearanceHideSelection = false;
            viewSeleccion.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFullFocus;
            viewSeleccion.OptionsBehavior.Editable = false;
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

            //viewSeleccion.Appearance.FocusedRow.BackColor = Color.DodgerBlue;
            //viewSeleccion.Appearance.FocusedRow.ForeColor = Color.White;
            //viewSeleccion.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        }
        //PROPIEDADES COLUMNAS
        private void opcionesColumnas()
        {
            viewSeleccion.Columns[0].Visible = false;
            viewSeleccion.Columns[1].Caption = "Trabajador";
            viewSeleccion.Columns[1].Width = 100;

            viewSeleccion.Columns[2].Caption = "Item";
            viewSeleccion.Columns[2].Width = 30;

            viewSeleccion.Columns[3].Caption = "Original";
            viewSeleccion.Columns[3].DisplayFormat.FormatString = "n0";
            viewSeleccion.Columns[3].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            viewSeleccion.Columns[3].Width = 30;

            viewSeleccion.Columns[4].Caption = "valor Calculado";
            viewSeleccion.Columns[4].DisplayFormat.FormatString = "n0";
            viewSeleccion.Columns[4].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            viewSeleccion.Columns[4].Width = 30;

            viewSeleccion.Columns[5].Caption = "N°";           
            viewSeleccion.Columns[5].Width = 30;
        }

        //OBTENER LAS FILAS SELECCIONADAS
        private int[] FilasSeleccionadas()
        {
            if (viewSeleccion.RowCount>0)
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
        private List<ItemTrabajador> getidRegistros(int[] rows)
        {
            //List<int> registros = new List<int>();
            List<ItemTrabajador> regs = new List<ItemTrabajador>();
            
            if (rows.Length > 0)
            {
                //RECORREMOS ARRAY
                for (int i = 0; i < rows.Length; i++)
                {
                    //  registros.Add(Convert.ToInt32(viewSeleccion.GetRowCellValue(rows[i], "id")));
                    
                    regs.Add(new ItemTrabajador()
                    {
                        NumeroItem = Convert.ToInt32(viewSeleccion.GetRowCellValue(rows[i], "numitem")),
                        item = (string)viewSeleccion.GetRowCellValue(rows[i], "coditem"),
                        contrato = (string)viewSeleccion.GetRowCellValue(rows[i], "contrato")
                    });
                }

                if (regs.Count > 0) return regs;
                else
                    return null;
            }
            else
                return null;
        }

        #endregion

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            if (viewSeleccion.RowCount>0)
            {
                int[] rows = FilasSeleccionadas();

                if (rows.Length == 0) { XtraMessageBox.Show("No haz selecionado filas", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);return; }

                //OBTENEMOS LOS DATOS DE LAS FILAS SELECIONADAS...
                List<ItemTrabajador> registros = new List<ItemTrabajador>();
                registros = getidRegistros(rows);

                if (registros.Count == 0) { XtraMessageBox.Show("No se encontraron registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                //ENVAMOS LISTADO DE REGISTROS...
                if (opener != null)
                {
                    opener.CargarSeleccion(registros);
                    XtraMessageBox.Show("Seleccion realizada correctamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Close();
                }                   

            }
            else
            {
                XtraMessageBox.Show("No se encontraron registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}