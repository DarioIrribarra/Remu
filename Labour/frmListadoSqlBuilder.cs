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
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.Utils.Menu;

namespace Labour
{
    public partial class frmListadoSqlBuilder : DevExpress.XtraEditors.XtraForm, IEditaBuilder
    {
        /// <summary>
        /// Para manipular reportes.
        /// </summary>
       private ReportBuilder Reporte = new ReportBuilder();

       public IGenerador iGenOpen { get; set; }

        #region "INTERFAZ"
        public void ReloadGridControl()
        {
            fnSistema.spllenaGridView(gridConsultas, "SELECT id, nombre, data FROM reporte ORDER BY id");
            if (viewConsultas.RowCount > 0)
            {
                fnSistema.spOpcionesGrilla(viewConsultas);
                ColumnsGridView();
            }
        }
        #endregion

        public frmListadoSqlBuilder()
        {
            InitializeComponent();
        }

        private void frmListadoSqlBuilder_Load(object sender, EventArgs e)
        {
            fnSistema.spllenaGridView(gridConsultas, "SELECT id, nombre, data FROM reporte ORDER BY id");
            if (viewConsultas.RowCount > 0)
            {
                fnSistema.spOpcionesGrilla(viewConsultas);
                ColumnsGridView();
            }                
        }

        private void ColumnsGridView()
        {
            if (viewConsultas.RowCount > 0)
            {
                //ID
                viewConsultas.Columns[0].Caption = "#";
                viewConsultas.Columns[0].Width = 10;

                viewConsultas.Columns[1].Caption = "Nombre";
                viewConsultas.Columns[2].Visible = false;
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            Close();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            if (viewConsultas.RowCount == 0)
            { XtraMessageBox.Show("No hay registros", "Información", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            int id = 0;
            string consulta = "";

            if (viewConsultas.RowCount > 0)
            {
                id = Convert.ToInt32(viewConsultas.GetFocusedDataRow()["id"]);
                consulta = (string)viewConsultas.GetFocusedDataRow()["data"];

                DialogResult ad = XtraMessageBox.Show($"¿Estás seguro de eliminar la consulta {consulta}?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (ad == DialogResult.Yes)
                {
                    //Eliminamos
                    if (Reporte.DelReport(id, consulta))
                    {
                        XtraMessageBox.Show("Registro eliminado correctamente", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);


                    }
                    else
                    {
                        XtraMessageBox.Show("Registro no se pudo eliminar", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                }
            }
        }

        private void viewConsultas_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            try
            {
                if (e.Menu != null)
                    e.Menu.Items.Clear();

                DXMenuItem viewModificar = new DXMenuItem();
                viewModificar.Caption = "Editar consulta";
                viewModificar.ImageOptions.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("images/data/editdatasource_16x16.png");
                viewModificar.Click += ViewModificar_Click;

                DXMenuItem viewChangeName = new DXMenuItem();
                viewChangeName.Caption = "Cambiar Nombre";
                viewChangeName.ImageOptions.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("images/data/renamedatasource_16x16.png");
                viewChangeName.Click += ViewChangeName_Click;

                DXMenuItem viewEliminar = new DXMenuItem();
                viewEliminar.Caption = "Eliminar consulta";
                viewEliminar.ImageOptions.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("images/actions/cancel_16x16.png");
                viewEliminar.Click += ViewEliminar_Click;

                DXMenuItem viewShowData = new DXMenuItem();
                viewShowData.Caption = "Ver reporte";
                viewShowData.ImageOptions.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("images/chart/3dcolumn_16x16.png");
                viewShowData.Click += ViewShowData_Click;

                e.Menu.Items.Add(viewModificar);
                e.Menu.Items.Add(viewChangeName);
                e.Menu.Items.Add(viewEliminar);
                e.Menu.Items.Add(viewShowData);
            }
            catch (Exception ex)
            {
                //ERROR...
            }
        }

        /// <summary>
        /// Permite ver la informacion que genera el reporte guardado.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewShowData_Click(object sender, EventArgs e)
        {
            int id = 0;
            string consulta = "";
            DataTable dt = new DataTable();
            if (viewConsultas.RowCount > 0)
            {
                id = Convert.ToInt32(viewConsultas.GetFocusedDataRow()["id"]);
                consulta = (string)viewConsultas.GetFocusedDataRow()["data"];

                if (id != 0 && consulta.Length > 0)
                {
                    dt = Reporte.GetDataFromReport(consulta);
                    if (dt != null)
                    {
                        frmDataGeneador data = new frmDataGeneador(dt);
                        data.StartPosition = FormStartPosition.CenterParent;
                        data.ShowDialog();
                    }                    
                }
            }   
        }

        /// <summary>
        /// Permite eliminar un reporte desde base de datos.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewEliminar_Click(object sender, EventArgs e)
        {
            int Id = 0;
            string consulta = "";
            string Nombre = "";
            try
            {
                if (viewConsultas.RowCount > 0)
                {
                    Id = Convert.ToInt32(viewConsultas.GetFocusedDataRow()["id"]);
                    consulta = (string)viewConsultas.GetFocusedDataRow()["data"];
                    Nombre = (string)viewConsultas.GetFocusedDataRow()["nombre"];

                    if (Id != 0 && consulta.Length > 0)
                    {
                        DialogResult adv = XtraMessageBox.Show("¿Estás seguro de eliminar reporte " + Nombre + "?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question );

                        if (adv == DialogResult.Yes)
                        {
                            if (Reporte.DelReport(Id, consulta))
                            {
                                XtraMessageBox.Show("Registro eliminado correctamente", "Información", MessageBoxButtons.OK, MessageBoxIcon.Stop);

                                fnSistema.spllenaGridView(gridConsultas, "SELECT id, nombre, data FROM reporte ORDER BY id");
                                if (viewConsultas.RowCount > 0)
                                {
                                    fnSistema.spOpcionesGrilla(viewConsultas);
                                    ColumnsGridView();
                                }
                            }
                            else
                            {
                                XtraMessageBox.Show("No se pudo eliminar registro", "Registro", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            } 
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("No se pudo eliminar registro", "Registro", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        /// <summary>
        /// Permite cambiar el nombre al reporte (Solo para descripcion).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewChangeName_Click(object sender, EventArgs e)
        {
            int id = 0;
            string consulta = "";
            string Nombre = "";
            ReportBuilder rpbuil = new ReportBuilder();
            try
            {
                if (viewConsultas.RowCount > 0)
                {
                    id = Convert.ToInt32(viewConsultas.GetFocusedDataRow()["id"]);
                    consulta = (string)viewConsultas.GetFocusedDataRow()["data"];
                    Nombre = (string)viewConsultas.GetFocusedDataRow()["nombre"];
                    if (id != 0 && consulta.Length > 0 && Nombre.Length > 0)
                    {
                        rpbuil.Id = id;
                        rpbuil.Sql = consulta;
                        rpbuil.Name = Nombre;

                        frmSaveReportBuilder save = new frmSaveReportBuilder(true, rpbuil);
                        save.IEditaOpen = this;                        
                        save.StartPosition = FormStartPosition.CenterParent;
                        save.ShowDialog();
                    }
                    else
                    {
                        XtraMessageBox.Show("No se pudo realizar la operación solicitada", "Información", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                }
            }
            catch (Exception ex)
            {
                //ERROR...
            }
        }

        /// <summary>
        /// Permite modificar un reporte.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewModificar_Click(object sender, EventArgs e)
        {
            int id = 0;
            string consulta = "", name = "";

            try
            {
                if (viewConsultas.RowCount > 0)
                {
                    id = Convert.ToInt32(viewConsultas.GetFocusedDataRow()["id"]);
                    consulta = (string)viewConsultas.GetFocusedDataRow()["data"];
                    name = (string)viewConsultas.GetFocusedDataRow()["nombre"];

                    Reporte = new ReportBuilder();
                    Reporte.Id = id;
                    Reporte.Name = name;
                    Reporte.Sql = consulta;

                    if (iGenOpen != null)
                    {
                        iGenOpen.CargarSql(Reporte);
                        Close();
                    }
                    else
                    {
                        XtraMessageBox.Show("No se pudo cargar consulta", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                }
                else
                {
                    XtraMessageBox.Show("No se pudo cargar consulta", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("No se pudo cargar consulta", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }
    }
}