using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.XtraEditors.Repository;
using DevExpress.Skins;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors.Controls;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Data.SQLite;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.Utils.Menu;

namespace Labour
{
    public partial class frmDatosUpdate : DevExpress.XtraEditors.XtraForm, IGrilla, IUpdateConfig
    {
        string ValorCelda = "";

        public IUpdateComboDatos OpenerUpdateJuegoDatos { get; set; }
        public Iform Opener { get; set; }
        #region "InterfazGrilla"

        #region "INTERFAZ ACTUALIZACION REGISTRO"
        public void ActualizarGrilla()
        {
            SqlLite.CargarGrilla(gridDatos, "select id, nombre, servidor, database, usuario, password from datos", viewDatos);
            //ACTUALIZAR COMBO
            if (OpenerUpdateJuegoDatos != null)
                OpenerUpdateJuegoDatos.RecargarCombo();
        }
        #endregion
        public void CargarGrid()
        {
            //recargar la grilla
            SqlLite.CargarGrilla(gridDatos, "SELECT * FROM datos", viewDatos);
        }
        public void CargarWait()
        {
            splashScreenManager1.ShowWaitForm();
        }

        public void CerrarWait()
        {
            splashScreenManager1.CloseWaitForm();
        }
        #endregion

        //DEFINICION METODO INTERFAZ

        //para guardar el id de la fila seleccionada
        public string idRow = "";
        public frmDatosUpdate()
        {
            InitializeComponent();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {

        }

        private void frmDatosUpdate_Load(object sender, EventArgs e)
        {

            SkinManager.EnableFormSkins();
            SkinManager.EnableMdiFormSkins();
            
            layoutControl1.AllowCustomization = false;
            //cargar grilla cuando se cargue el formulario
            SqlLite.CargarGrilla(gridDatos, "select id, nombre, servidor, database, usuario, password from datos", viewDatos);
            
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            //LimpiarCampos();
            frmDatos datos = new frmDatos();
            datos.ShowDialog(this);
        }

        //limpiar campos  
        private void btnGrabar_Click(object sender, EventArgs e)
        {           

            
        }

        private void viewDatos_RowUpdated(object sender, DevExpress.XtraGrid.Views.Base.RowObjectEventArgs e)
        {
            //MessageBox.Show("valor actualizado!");            
        }

        private void viewDatos_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            int id = 0;            
            string value = "";
            string campo = "";
            string celda = "";
            string parametro = "";
            if (ValorCelda!= "")
            {
                parametro = ValorCelda;
            }
           
            DialogResult pregunta = XtraMessageBox.Show("¿Desea Guardar los cambios en '" + parametro + "' ?", "Informacion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            //METODO PARA SABER SI SE CAMBIO EL VALOR DE UNA CELDA
            ColumnView view = gridDatos.FocusedView as ColumnView;
            celda = view.FocusedValue.ToString();
            //PREGUNTAMOS SI SE EDITO EL VALOR DE LA CELDA
            if (view.UpdateCurrentRow())
            {
                if (pregunta == DialogResult.Yes)
                {
                    if (view.FocusedValue.ToString() != "")
                    {
                        //OBTENEMOS EL ID DE LA FILA SELECCIONADA
                        id = int.Parse(view.GetFocusedDataRow()["id"].ToString());
                        //OBTENER EL VALOR NUEVO DE LA CELDA
                        value = view.GetFocusedValue().ToString();
                        //OBTENER NOMBRE DE LA COLUMNA EN BD
                        campo = view.FocusedColumn.FieldName;
                        //LLAMAR A LA FUNCION UPDATE PARA ACTUALIZAR EL VALOR EN BD
                        SqlLite.UpdateRegistro(value, campo, id);
                        //recargar la grilla
                        SqlLite.CargarGrilla(gridDatos, "select * from datos", viewDatos);
                        //recargar usando interfaz el combo con nuevo valor
                        Iform inter = this.Owner as Iform;
                        if (inter!=null)
                        {
                            inter.CargarCombo();
                        }
                    }
                    else
                    {
                        XtraMessageBox.Show("Valor no Válido", "informacion", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        //REFRESCAR GRILLA
                        SqlLite.CargarGrilla(gridDatos, "select * from datos", viewDatos);
                    }
                }
                else {
                    //REFRESCAR GRILLA
                    SqlLite.CargarGrilla(gridDatos, "select * from datos", viewDatos);
                }
              
            
            }
        }     

        private void viewDatos_ValidatingEditor(object sender,BaseContainerValidateEditorEventArgs e)
        {
           
        }

        private void viewDatos_InvalidValueException(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e)
        {
            //e.ExceptionMode = ExceptionMode.NoAction;
            //mostrar el mensaje con el error
            MessageBox.Show(e.ErrorText, "Valor invalido", MessageBoxButtons.OK,MessageBoxIcon.Error);
        }

        private void viewDatos_ShowingEditor(object sender, CancelEventArgs e)
        {
            
        }
        private void btnSeleccionar_Click(object sender, EventArgs e)
        {
            //guardar el id de la fila seleccionada y pasar la variable desde este formulario al formulario acceso
            if (viewDatos.RowCount>0)
            {
                //OBTENEMOS LOS VALORES DE LA GRILLA EN EL FOCO ACTUAL
                idRow = viewDatos.GetFocusedDataRow()["id"].ToString();
                string user = viewDatos.GetFocusedDataRow()["usuario"].ToString();
                string pass = viewDatos.GetFocusedDataRow()["password"].ToString();
                string nameConf = viewDatos.GetFocusedDataRow()["nombre"].ToString();
                //XtraMessageBox.Show(idRow, "Caption", MessageBoxButtons.OK, MessageBoxIcon.Error);

                //USAMOS LA INTERFAZ IFORM
                //SI NO ES NULA USAMOS EL METODO CREADO EN LA INTERFAZ
                this.Opener.CargarCajas(user, pass, nameConf);

                 this.Dispose();                
                //this.Close();
            }
        }

        private void gridDatos_DoubleClick(object sender, EventArgs e)
        {          

            //Cargar en combobox el elemento seleccionado en la grilla (la conexion seleccionada)
            //guardar el id de la fila seleccionada y pasar la variable desde este formulario al formulario acceso
            if (viewDatos.RowCount > 0)
            {
                //OBTENEMOS LOS VALORES DE LA GRILLA EN EL FOCO ACTUAL
                idRow = viewDatos.GetFocusedDataRow()["id"].ToString();
                string user = viewDatos.GetFocusedDataRow()["usuario"].ToString();
                string pass = viewDatos.GetFocusedDataRow()["password"].ToString();
                string nameConf = viewDatos.GetFocusedDataRow()["nombre"].ToString();
                //XtraMessageBox.Show(idRow, "Caption", MessageBoxButtons.OK, MessageBoxIcon.Error);

                //USAMOS LA INTERFAZ IFORM
                //SI NO ES NULA USAMOS EL METODO CREADO EN LA INTERFAZ
                this.Opener.CargarCombo();
                this.Opener.CargarCajas(user, pass, nameConf);

                this.Dispose();
                //this.Close();
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (viewDatos.RowCount>0)
            {
                //OBTENEMOS EL ID DE LA FILA SELECCIONADA
                int id = int.Parse(viewDatos.GetFocusedDataRow()["id"].ToString());
                //OBTENER EL NOMBRE DE LA CONEXION
                string name = viewDatos.GetFocusedDataRow()["nombre"].ToString();
                //LLAMAMOS A LA FUNCION ELIMINAR
                fnEliminar(id, name);
            }
        }

        //METODO PARA ELIMINAR UN ELEMENTO DE BD
        /*
         * PARAMETRO ENTRADA
         * @-> ID
         * @ -> NOMBRE CONEXION
         */
        private void fnEliminar(int id, string nombre)
        {
            string sql = "DELETE FROM DATOS WHERE id=@pid";
            int res = 0;
            DialogResult pregunta = XtraMessageBox.Show("¿Seguro Desea Eliminar la Conexion '" + nombre + "'?", "Informacion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            //PREGUNTAR SI QUIERE ELIMINAR EL REGISTRO
            if (pregunta == DialogResult.Yes)
            {
                if (SqlLite.NuevaConexion())
                {
                    using (SqlLite.Sqlconn)
                    {
                        SQLiteCommand cmd = new SQLiteCommand(sql, SqlLite.Sqlconn);
                        //PARAMETROS
                        cmd.Parameters.Add(new SQLiteParameter("@pid", id));
                        //EJECUTAMOS LA CONSULTA
                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            //SE ELIMINO CORRECTAMENTE
                            XtraMessageBox.Show("Eliminado Correctamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            //VOLVEMOS A CARGAR LA GRILLA
                            SqlLite.CargarGrilla(gridDatos, "select * from datos", viewDatos);

                            if (OpenerUpdateJuegoDatos != null)
                                OpenerUpdateJuegoDatos.RecargarCombo();
                        }
                        else
                        {
                            XtraMessageBox.Show("Error al Intentar Eliminar", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        //LIBERAMOS CMD 
                        cmd.Dispose();
                        //CERRAMOS LA CONEXION
                       // SqlLite.Sqlconn.Close();
                    }
                }
                else
                {
                    XtraMessageBox.Show("No hay conexion con la base de datos!", "Error Conexion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else {
                XtraMessageBox.Show("El Usuario a Cancelado la Operación", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            //REFRESCAR GRILLA
            SqlLite.CargarGrilla(gridDatos, "select * from datos", viewDatos);
        }

        private void frmDatosUpdate_FormClosing(object sender, FormClosingEventArgs e)
        {
            //volver a llenar el combobox cuando se cierre el formulario
            this.Opener.CargarCombo();
        }

        private void btnProbarConexion_Click(object sender, EventArgs e)
        {
            //PROBAR CONEXION CORRECTA A BASE DE DATOS!
                   
            string user = "";
            string pass = "";
            string db = "";
            string server = "";
            Cifrado cif = new Cifrado();
            
            if (viewDatos.RowCount>0)
            {
                //OBTENER LOS DATOS DESDE LA GRILLA 
                user = viewDatos.GetFocusedDataRow()["usuario"].ToString();
                pass = cif.DesencriptaTripleDesc(viewDatos.GetFocusedDataRow()["password"].ToString());
                db = viewDatos.GetFocusedDataRow()["database"].ToString();
                server = viewDatos.GetFocusedDataRow()["servidor"].ToString();
                //LLAMAMOS AL METODO PARA REALIZAR LA CONEXION
                fnSistema.pgUser = user;
                fnSistema.pgPass = pass;
                fnSistema.pgDatabase = db;
                fnSistema.pgServer = server;
                
                splashScreenManager1.ShowWaitForm();
                //SI RETORNA TRUE LA CONEXION ES CORRECTA
                if (fnSistema.ConectarSQLServer())
                {
                    splashScreenManager1.CloseWaitForm();
                    XtraMessageBox.Show("Conexion Establecida Correctamente", "MENSAJE", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else {
                    splashScreenManager1.CloseWaitForm();
                    XtraMessageBox.Show("Error de Conexion", "MENSAJE", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void viewDatos_CellValueChanging(object sender, CellValueChangedEventArgs e)
        {
            //OBTENER EL VALOR DE LA CELDA
            if (viewDatos.RowCount>0)
            {
                ValorCelda = viewDatos.GetRowCellValue(e.RowHandle, "nombre").ToString();
            }
        }        
       
        private void viewDatos_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            DXPopupMenu menu = e.Menu;

            if (menu != null)
            {
                DXMenuItem submenu = new DXMenuItem("Editar Configuracion", new EventHandler(EditConfiguration));
                submenu.Image = Labour.Properties.Resources.editdatasource_16x16;
                menu.Items.Clear();
                menu.Items.Add(submenu);
            }
        }

        private void EditConfiguration(object sender, EventArgs e)
        {
            //AL HACER CLICK MOSTRAMOS EL FORMULARIO PARA EDITAR LAS CONFIGURACIONES
            int idRegistro = 0;
            if (viewDatos.RowCount>0)
            {
                idRegistro = Convert.ToInt32(viewDatos.GetFocusedDataRow()["id"]);

                frmEditaConfiguracion edita = new frmEditaConfiguracion(idRegistro);
                edita.open = this;
                edita.StartPosition = FormStartPosition.CenterParent;                
                edita.ShowDialog();
            }
        }

        private void viewDatos_ShownEditor(object sender, EventArgs e)
        {
          //ContextMenu con = new ContextMenu();         

            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            //con.MenuItems.Add(con.MenuItems.Count, new MenuItem("Editar"));
            //AGREGAR EVENTO CLICK            

            //view.ActiveEditor.ContextMenu = con;            

            BaseEdit inplaceEditor = ((GridView)sender).ActiveEditor;
            inplaceEditor.DoubleClick += inplaceEditor_DoubleClick;    
        }        

        private void inplaceEditor_DoubleClick(object sender, EventArgs e)
        {
            BaseEdit editor = (BaseEdit)sender;
            GridControl grid = (GridControl)editor.Parent;
            Point pt = grid.PointToClient(Control.MousePosition);
            GridView view = (GridView)grid.FocusedView;
            GridHitInfo info = view.CalcHitInfo(pt);
            if ((info.InRow || info.InRowCell) && info.Column != null)
            {
                //Cargar en combobox el elemento seleccionado en la grilla (la conexion seleccionada)
                //guardar el id de la fila seleccionada y pasar la variable desde este formulario al formulario acceso
                if (viewDatos.RowCount > 0)
                {
                    //OBTENEMOS LOS VALORES DE LA GRILLA EN EL FOCO ACTUAL
                    idRow = viewDatos.GetFocusedDataRow()["id"].ToString();
                    string user = viewDatos.GetFocusedDataRow()["usuario"].ToString();
                    string pass = viewDatos.GetFocusedDataRow()["password"].ToString();
                    string nameConf = viewDatos.GetFocusedDataRow()["nombre"].ToString();
                    //XtraMessageBox.Show(idRow, "Caption", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    //USAMOS LA INTERFAZ IFORM
                    //SI NO ES NULA USAMOS EL METODO CREADO EN LA INTERFAZ
                    this.Opener.CargarCajas(user, pass, nameConf);

                    this.Dispose();
                    //this.Close();
                }
            }
        }

        private void gridDatos_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void gridDatos_ProcessGridKey(object sender, KeyEventArgs e)
        {
           
        }

        private void gridDatos_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void gridDatos_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            

            
        }

        private void viewDatos_DoubleClick(object sender, EventArgs e)
        {
            GridView view = (GridView)sender;
            Point pt = view.GridControl.PointToClient(MousePosition);
            GridHitInfo info = view.CalcHitInfo(pt);
            if ((info.InRow || info.InRowCell) && info.Column != null)
            {
                //Cargar en combobox el elemento seleccionado en la grilla (la conexion seleccionada)
                //guardar el id de la fila seleccionada y pasar la variable desde este formulario al formulario acceso
                if (viewDatos.RowCount > 0)
                {
                    //OBTENEMOS LOS VALORES DE LA GRILLA EN EL FOCO ACTUAL
                    idRow = viewDatos.GetFocusedDataRow()["id"].ToString();
                    string user = viewDatos.GetFocusedDataRow()["usuario"].ToString();
                    string pass = viewDatos.GetFocusedDataRow()["password"].ToString();
                    string nameConf = viewDatos.GetFocusedDataRow()["nombre"].ToString();
                    //XtraMessageBox.Show(idRow, "Caption", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    //USAMOS LA INTERFAZ IFORM
                    //SI NO ES NULA USAMOS EL METODO CREADO EN LA INTERFAZ
                    this.Opener.CargarCajas(user, pass, nameConf);

                    this.Dispose();
                    //this.Close();
                }
            }
            
        }

        private void viewDatos_HiddenEditor(object sender, EventArgs e)
        {
            if (sender.GetType() == typeof(BaseEdit))
            {
                BaseEdit editor = (BaseEdit) sender;
                editor.DoubleClick -= inplaceEditor_DoubleClick;
            }
        }

        private void btnSalirAfp_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            //Sesion.NuevaActividad();

            Close();
        }
    }

    class RowInfo
    {
        public int RowHandle;
        public GridView View;

        public RowInfo(GridView view, int rowHandle)
        {
            this.RowHandle = rowHandle;
            this.View = view;
        }
    }
    
}
