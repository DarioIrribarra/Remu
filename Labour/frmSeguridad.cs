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
    public partial class frmSeguridad : DevExpress.XtraEditors.XtraForm
    {

        //PARA SABER SI ES UPDATE
        private bool Update = false;
        public frmSeguridad()
        {
            InitializeComponent();
        }

        private void frmSeguridad_Load(object sender, EventArgs e)
        {
            CargarGrilla();
            CargaUsuario();

            if (viewUsuario.RowCount > 0)
            {
                //GENERAMOS LISTADO DE OBJETOS...
                List<objeto> listado = new List<objeto>();
                listado = CargarDatos();

                if (listado.Count > 0)
                {
                    //REALIZAMOS SELECCION FILAS               
                    SetSelection(listado);
                }
                else
                {
                    //MOSTRAMOS GRILLA SIN SELECCIONAR...
                    viewObjeto.ClearSelection();
                }
            }
            else
            {
                Update = false;
            }
        }

        #region "MANEJO DATOS"
        //INGRESO TABLA AUTORIZACION
        private bool IngresoAutorizacion(string pUsuario, List<objeto> pLista)
        {
            string sql = "INSERT INTO autorizacion(usuario, objeto, acceso) VALUES(@pUsuario, @pObjeto, @pAcceso)";
            SqlCommand cmd;
            SqlTransaction tr;
            bool transaccionCorrecta = false;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    tr = fnSistema.sqlConn.BeginTransaction();
                    try
                    {
                        //RECORREMOS LISTADO
                        foreach (var item in pLista)
                        {
                            using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                            {
                                //PARAMETROS
                                cmd.Parameters.Add(new SqlParameter("@pUsuario", pUsuario));
                                cmd.Parameters.Add(new SqlParameter("@pObjeto", item.Codigo));
                                cmd.Parameters.Add(new SqlParameter("@pAcceso", item.Acceso));

                                //AGREGAMOS A TRANSACCION
                                cmd.Transaction = tr;

                                //EJECUTAMOS
                                cmd.ExecuteNonQuery();

                                //LIMPIAMOS PARAMETROS
                                cmd.Parameters.Clear();
                            }
                        }

                        transaccionCorrecta = true;
                    }
                    catch (SqlException ex)
                    {
                        //SI HAY ALGUN ERROR HACEMOS ROLLBACK
                        tr.Rollback();
                        transaccionCorrecta = false;
                    }

                    if (transaccionCorrecta)
                    {
                        tr.Commit();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return transaccionCorrecta;
        }

        //MODIFICAR TABLA AUTORIZACION
        private bool ModificaAutorizacion(string pUsuario, List<objeto> pLista)
        {
            /*VERIFICAR SI OBJETO EXISTE*/
            /*SI OBJETO EXISTE HACEMOS UPDATE*/
            /*SI OBJETO NO EXISTE HACEMOS INSERT*/
            bool transaccionCorrecta = false;
            string sqlUpdate = "UPDATE autorizacion SET acceso=@pAcceso WHERE usuario=@pUser AND objeto=@pObjeto";
            string sqlInsert = "INSERT INTO autorizacion(usuario, objeto, acceso) VALUES(@pUser, @pObjeto, @pAcceso)";
            string sqlObjetoExiste = "SELECT count(*) FROM AUTORIZACION WHERE usuario=@pUser AND objeto=@pObjeto";
            int count;
            
            SqlCommand cmd;
            SqlTransaction tr;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    tr = fnSistema.sqlConn.BeginTransaction();
                    //RECORREMOS LISTADO
                    try
                    {
                        foreach (var item in pLista)
                        {
                            count = 0;

                            //PREGUNTAMOS SI OBJETO EXISTE
                            using (cmd = new SqlCommand(sqlObjetoExiste, fnSistema.sqlConn))
                            {
                                //PARAMETROS
                                cmd.Parameters.Add(new SqlParameter("@pUser", pUsuario));
                                cmd.Parameters.Add(new SqlParameter("@pObjeto", item.Codigo));

                                cmd.Transaction = tr;
                                count = Convert.ToInt32(cmd.ExecuteScalar());

                                cmd.Parameters.Clear();                            
                            }

                            //EXISTE OBJETO
                            if (count>0)
                            {
                                //UPDATE
                                using (cmd = new SqlCommand(sqlUpdate, fnSistema.sqlConn))
                                {
                                    //PARAMETROS
                                    cmd.Parameters.Add(new SqlParameter("@pUser", pUsuario));
                                    cmd.Parameters.Add(new SqlParameter("@pObjeto", item.Codigo));
                                    cmd.Parameters.Add(new SqlParameter("@pAcceso", item.Acceso));

                                    //AGREGAMOS A TRANSACCION
                                    cmd.Transaction = tr;

                                    //EJECUTAMOS SENTENCIA
                                    cmd.ExecuteNonQuery();

                                    cmd.Parameters.Clear();
                                }
                            }
                            else
                            {
                                //INSERT
                                using (cmd = new SqlCommand(sqlInsert, fnSistema.sqlConn))
                                {
                                    //PARAMETROS
                                    cmd.Parameters.Add(new SqlParameter("@pUser", pUsuario));
                                    cmd.Parameters.Add(new SqlParameter("@pObjeto", item.Codigo));
                                    cmd.Parameters.Add(new SqlParameter("@pAcceso", item.Acceso));

                                    //AGREGAMOS A TRANSACCION
                                    cmd.Transaction = tr;

                                    //EJECUTAMOS SENTENCIA
                                    cmd.ExecuteNonQuery();

                                    cmd.Parameters.Clear();
                                }
                            }                           
                        }

                        transaccionCorrecta = true;
                    }
                    catch (SqlException ex)
                    {
                        transaccionCorrecta = false;
                        //SI SE PRODUCE UN ERROR HACEMOS ROLLBACK
                        tr.Rollback();
                    }

                    if (transaccionCorrecta)
                    {
                        tr.Commit();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return transaccionCorrecta;
        }

        //EXISTE OBJETO
        private bool ExisteObjeto(string pUsuario, string pObjeto)
        {
            bool existe = false;
            string sql = "SELECT count(*) FROM AUTORIZACION WHERE usuario=@pUsuario AND objeto=@pObjeto";
            int count = 0;
            SqlCommand cmd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pUsuario", pUsuario));
                        cmd.Parameters.Add(new SqlParameter("@pObjeto", pObjeto));

                        count = Convert.ToInt32(cmd.ExecuteNonQuery());

                        if (count > 0)
                            existe = true;

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }


            return existe;
        }

        private List<objeto> FilasGrilla()
        {
            List<objeto> lista = new List<objeto>();
            if (viewObjeto.RowCount > 0)
            {
                //OBTENER TODAS LAS FILAS
                //ITERAR CADA FINAL DEL GRID
                for (int i = 0; i < viewObjeto.DataRowCount; i++)
                {
                    //PREGUNTAMOS SI LA FILA ESTA SELECCIONADA
                    if (viewObjeto.IsRowSelected(i))
                    {
                        //GUARDAMOS CON ACCESO 1 (SI ESTA SELECCIONADA)
                        lista.Add(new objeto() {
                            Codigo = (string)viewObjeto.GetRowCellValue(i, "codobjeto"),
                            Acceso = 1
                        });
                    }
                    else
                    {
                        lista.Add(new objeto() { Codigo = (string)viewObjeto.GetRowCellValue(i, "codobjeto"),
                            Acceso = 0
                        });
                    }
                }

                //RETORNAMOS LISTA
                return lista;
            }
            else
            {
                return null;
            }
        }

        //CARGAR GRILLA
        private void CargarGrilla()
        {
            string sql = "SELECT codobjeto, descobjeto FROM objeto";
            SqlCommand cmd;
            SqlDataAdapter ad = new SqlDataAdapter();
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

                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            //LLENAMOS GRILLA
                            gridObjeto.DataSource = ds.Tables[0];
                            OpcionesGrilla();
                            Columnas();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        //PROPIEDADES COLUMNA
        private void OpcionesGrilla()
        {
            viewObjeto.OptionsSelection.EnableAppearanceHideSelection = false;
            viewObjeto.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFullFocus;
            viewObjeto.OptionsBehavior.Editable = false;
            viewObjeto.OptionsSelection.EnableAppearanceFocusedCell = false;
            //PARA LA SELECCION USANDO CHECKBOX
            viewObjeto.OptionsSelection.MultiSelect = true;
            viewObjeto.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;

            //deshabilitar menus contextuales
            viewObjeto.OptionsMenu.EnableColumnMenu = false;
            viewObjeto.OptionsMenu.EnableFooterMenu = false;
            viewObjeto.OptionsMenu.EnableGroupPanelMenu = false;

            //evitar filtrar por columnas y Ordenar por Columnas
            viewObjeto.OptionsCustomization.AllowFilter = false;
            viewObjeto.OptionsCustomization.AllowGroup = false;
            viewObjeto.OptionsCustomization.AllowSort = false;
            viewObjeto.OptionsCustomization.AllowColumnResizing = false;
            viewObjeto.OptionsCustomization.AllowColumnMoving = false;

            //deshabilitar cabezera de la tabla
            viewObjeto.OptionsView.ShowGroupPanel = false;
        }

        private void Columnas()
        {
            viewObjeto.Columns[0].Caption = "Codigo";
            viewObjeto.Columns[0].Width = 100;
            viewObjeto.Columns[1].Caption = "Descripcion";
            viewObjeto.Columns[1].Width = 150;
        }

        private void ColumnasUsuario()
        {
            viewUsuario.Columns[0].Caption = "Usuario";
            viewUsuario.Columns[0].Width = 70;
            viewUsuario.Columns[1].Caption = "Grupo";
            viewUsuario.Columns[1].Width = 150;
        }

        //CARGAR GRILLA USUARIO
        private void CargaUsuario()
        {
            string sql = "select usuario, grupo.descripcion from usuario " +
                         "INNER JOIN grupo ON grupo.id = usuario.grupo";
            SqlCommand cmd;
            SqlDataAdapter ad = new SqlDataAdapter();
            DataSet ds = new DataSet();
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        ad.SelectCommand = cmd;
                        ad.Fill(ds, "data");

                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            //CARGAR GRILLA
                            gridUsuario.DataSource = ds.Tables[0];
                            ColumnasUsuario();
                            fnSistema.spOpcionesGrilla(viewUsuario);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        //LISTA DE REGISTROS ASOCIADOS A USUARIO DESDE BD
        private List<objeto> CargarDatos()
        {
            List<objeto> listado = new List<objeto>();
            if (viewUsuario.RowCount > 0 && viewObjeto.RowCount > 0)
            {
                //OBTENER EL USUARIO SELECCIONADO
                string user = (string)viewUsuario.GetFocusedDataRow()["usuario"];

                string sql = "SELECT objeto, acceso FROM autorizacion WHERE usuario=@pUsuario";
                SqlCommand cmd;
                SqlDataReader rd;
                try
                {
                    if (fnSistema.ConectarSQLServer())
                    {
                        using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@pUsuario", user));
                            rd = cmd.ExecuteReader();
                            if (rd.HasRows)
                            {
                                while (rd.Read())
                                {
                                    //LLENAMOS LISTA
                                    listado.Add(new objeto() { Codigo = (string)rd["objeto"], Acceso = Convert.ToInt16(rd["acceso"]) });

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

                //RETORNAMOS LISTA
                return listado;
            }
            else
            {
                return null;
            }
        }

        //SELECCIONAR CHECKBOXS GRIDVIEW EN BASE A LISTADO
        private void SetSelection(List<objeto> pLista)
        {
            if (pLista.Count > 0 && viewObjeto.RowCount > 0)
            {
                Update = true;
                int count = 0;
                //RECORREMOS GRILLA 
                for (int i = 0; i < viewObjeto.DataRowCount; i++)
                {
                    //OBTENER EL CODIGO DEL OBJETO
                    string code = ((string)viewObjeto.GetRowCellValue(i, "codobjeto")).ToLower();

                    //RECORREMOS LISTA
                    foreach (var objeto in pLista)
                    {
                        if ((code == objeto.Codigo.ToLower()) && objeto.Acceso == 1)
                            viewObjeto.SelectRow(i);
                        else if ((code == objeto.Codigo.ToLower()) && objeto.Acceso == 0)
                            viewObjeto.UnselectRow(i);
                    }

                    //if (count == pLista.Count)
                      //  viewObjeto.ClearSelection();
                }
            }
        }

        //DATOS FROM BD
        private void DatosFromBd()
        {
            if (viewUsuario.RowCount > 0)
            {
                //GENERAMOS LISTADO DE OBJETOS...
                List<objeto> listado = new List<objeto>();
                listado = CargarDatos();

                if (listado.Count > 0)
                {
                    //REALIZAMOS SELECCION FILAS               
                    SetSelection(listado);
                }
                else
                {
                    //MOSTRAMOS GRILLA SIN SELECCIONAR...
                    viewObjeto.ClearSelection();
                }
            }
            else
            {
                Update = false;
            }
        }

        //VERIFICAR SI HAY CAMBIOS
        private bool CambiosSinGuardar()
        {
            bool singuardar = false;
            if (viewUsuario.RowCount>0)
            {
                //OBTENER LISTADO FROM BD
                List<objeto> lista = new List<objeto>();
                string user = (string)viewUsuario.GetFocusedDataRow()["usuario"];

                lista = CargarDatos();

                //ITERAR FILAS GRIDVIEW
                int x = 0;
                string code = "";
                for (int i = 0; i < viewObjeto.DataRowCount; i++)
                {
                    //COMPARAMOS SI LA FILA ESTA SELECCIONADA Y ES MISMO
                    code = (string)viewObjeto.GetRowCellValue(i, "codobjeto");
                    foreach (var item in lista)
                    {
                        //SI LA FILA DEL GRIDVIEW ESTA SELECCIONADA                         
                        if (viewObjeto.IsRowSelected(i) && (item.Codigo == code))
                        {
                            //SI EL VALOR ES CERO SON DISTINTOS
                            if (item.Acceso == 0)
                                x++;
                        }
                    }
                }

                if (x > 0)
                    singuardar = true;

            }

            return singuardar;
        }

        //GENERAR LISTADO DE OBJETOS DESDE TABLA OBJETO
        private List<objeto> ListaObjetos()
        {
            List<objeto> lista = new List<objeto>();
            string sql = "SELECT codobjeto FROM objeto";
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //LLENAMOS LISTADO
                                lista.Add(new objeto() { Codigo = (string)rd["codobjeto"], Acceso = 0});
                            }
                        }
                        else
                            lista = null;
                    }

                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                    rd.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return lista;
        }

        /*COMPARAR LISTADO ANTERIOR CON DATA DESDE GRILLA*/
        private List<objeto> ListaReemplazo()
        {
            //COMPARAMOS DATOS DE LISTA GENERADO DE TABLA OBJETO CON LISTA GENERADA DESDE SELECCION GRILLA

            /*GENERAMOS LISTA DE OBJETOS DESDE TABLA OBJETO*/
            List<objeto> ListaBd = new List<objeto>();
            /*GENERAMOS LISTA DESDE SELECCION GRILLA*/
            List<objeto> ListaGrilla = new List<objeto>();

            ListaBd = ListaObjetos();
            ListaGrilla = FilasGrilla();

            //CAMBIAMOS PROPIEDAD ACCESO EN LISTA OBJETOS DE ACUERDO A FILAS GRILLA
            foreach (var objeto in ListaBd)
            {
                foreach (var seleccion in ListaGrilla)
                {
                    //COMPARAMOS OBJETOS
                    if (seleccion.Codigo == objeto.Codigo)
                        objeto.Acceso = seleccion.Acceso;
                }
            }

            /*RETORNAMOS LISTA BD MODIFICADA*/
            foreach (var item in ListaBd)
            {
                XtraMessageBox.Show(item.Codigo);
            }
            return ListaBd;
        }        

        #endregion

        private void btnGrabar_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            if (viewObjeto.RowCount == 0 || viewUsuario.RowCount == 0)
            { XtraMessageBox.Show("No hay informacion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);return; }

            List<objeto> lista = new List<objeto>();
            lista = FilasGrilla();

            if (Update)
            {
                //UPDATE
                if (lista.Count>0)
                {
                    string usuario = (string)viewUsuario.GetFocusedDataRow()["usuario"];
                    //GUARDAMOS DATOS
                    if (ModificaAutorizacion(usuario, lista))
                    {
                        XtraMessageBox.Show("Modificacion correcta", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        DatosFromBd();
                    }
                    else
                    {
                        XtraMessageBox.Show("Error al modificar", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            else
            {
                //FILAS SELECCIONADAS               

                if (lista.Count > 0)
                {
                    string usuario = (string)viewUsuario.GetFocusedDataRow()["usuario"];
                    //GUARDAMOS DATOS
                    if (IngresoAutorizacion(usuario, lista))
                    {
                        XtraMessageBox.Show("Ingreso correcto", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        DatosFromBd();
                    }
                    else
                    {
                        XtraMessageBox.Show("Error al ingresar", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    XtraMessageBox.Show("No hay informacion disponible", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }           
        }

        private void gridUsuario_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            DatosFromBd();
        }

        private void gridUsuario_KeyUp(object sender, KeyEventArgs e)
        {
            DatosFromBd();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD  DE SESION
            Sesion.NuevaActividad();

            if (viewUsuario.RowCount>0 && viewObjeto.RowCount>0)
            {
                if (CambiosSinGuardar())
                {
                    DialogResult advertencia = XtraMessageBox.Show("Hay cambios sin guardar, ¿Deseas cerrar de todas formas?", "Informacion", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (advertencia == DialogResult.Yes)
                        Close();
                }
                else
                    Close();
            }
            else
            {
                Close();
            }
            
        }
    }
}