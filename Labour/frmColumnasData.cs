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
using System.Runtime.InteropServices;

namespace Labour
{
    public partial class frmColumnasData : DevExpress.XtraEditors.XtraForm
    {
        [DllImport("user32")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32")]
        static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        const int MF_BYCOMMAND = 0;
        const int MF_DISABLED = 2;
        const int SC_CLOSED = 0xF060;

        //PARA SABER SI SE ABRIO DESDE OTRO FORMULARIO
        private bool AdministraColumna = false;

        //PARA LISTA FROM FORMULARIO EXPORTAR EMPLEADOS
        private List<string> ListadoCamposUsados = null;

        public IColumnasData opener { get; set; }
        public frmColumnasData()
        {
            InitializeComponent();
        }

        public frmColumnasData(bool Columnas, List<string> Lista)
        {
            InitializeComponent();
            AdministraColumna = Columnas;
            ListadoCamposUsados = GeneraListaFromOther(Lista);
            
        }

        private void frmColumnasData_Load(object sender, EventArgs e)
        {
            var sm = GetSystemMenu(Handle, false);
            EnableMenuItem(sm, SC_CLOSED, MF_BYCOMMAND | MF_DISABLED);

            CargarListBox(ListBoxData);
            //SI LISTA QUE VIENE DE FORMULARIO EXPORTAR EMPLEADOS ES DISTINTAS DE NULL LLENAMOS LIST BOX
            if (ListadoCamposUsados != null)
            {
                AgregaElementos(ListadoCamposUsados, ListBoxResultante);
                //QUITAR DE LISTBOXDATA LOS ELEMENTOS QUE ESTAN EN LISTBOX RESULTANTE
                EliminarDataList(ListadoCamposUsados, ListBoxData);

                lblNumPrimary.Text = ListBoxData.Items.Count.ToString();
                lblNumResultante.Text = ListBoxResultante.Items.Count.ToString();
            }
                  
        }


        #region "MANEJO DE DATOS"
        //CARGAR LISTBOX ENTRANTE
        private void CargarListBox(ListBoxControl ListControl)
        {
            string sql = "SELECT LOWER(COLUMN_NAME) as columna FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME ='TRABAJADOR' ";
                        

            //string sql = "SELECT contrato, nombre, direccion, telefono FROM trabajador where anomes = 201804 AND contrato = '125291112'";

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
                                string col = (string)rd["columna"];
                                if(ElementoExiste(col, ListControl) == false && ColumnaNoValida(col.ToLower()) == false)
                                    ListControl.Items.Add(col);                                                  
                            }
                        }
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
            ListControl.SelectedIndex = 0;
            lblNumPrimary.Text = ListControl.Items.Count.ToString();
            lblNumResultante.Text = "0";
            
        }

        //COLUMNAS NO VALIDA PARA MOSTRAR
        private bool ColumnaNoValida(string columna)
        {
            List<string> Columnas = new List<string>() {"fechasegces", "fechaprogresivo", "rutafoto", "empresa",
            "fechavacacion", "status", "pasaporte", "id"};
            bool valida = false;

            foreach (var item in Columnas)
            {
                if (item == columna)
                {
                    //SI SON IGUALES NO ES VALIDA
                    valida = true;
                    break;
                }
            }

            return valida;
        }
        //METODO PARA SABER SI UN ELEMENTO EXISTE EN UNA LISTA
        private bool ElementoExiste(string name, ListBoxControl Lista)
        {
            bool existe = false;
            if (Lista.Items.Count > 0)
            {
                //RECORREMOS LISTA DE ITEMS
                foreach (var item in Lista.Items)
                {
                    if (item.ToString() == name)
                    {
                        existe = true;
                        break;
                    }
                }
            }

            return existe;
        }

        //AGREGAR TODOS LOS ITEMS DE UNA LISTA A LA OTRA
        private void AgregaMasivo(ListBoxControl Primary, ListBoxControl Resultante)
        {
            if (Primary.Items.Count > 0)
            {
                //DEBEMOS VERIFICAR CADA ITEMS ANTES DE INGRESAR
                if (Resultante.Items.Count == 0)
                {
                    foreach (var item in Primary.Items)
                    {
                        Resultante.Items.Add(item);
                    }
                    Primary.Items.Clear();
                }
                else
                {
                    //USAREMOS UNA LISTA AUXILIAR PARR IR GUARDANDO LOS ITEMS PARA DESPUES ELIMINARLOS
                    List<string> Auxiliar = new List<string>();

                    //RECORREMOS ITEMS DE PRIMARY                    
                    foreach (var elemento in Primary.Items)
                    {
                        if (ElementoExiste(elemento.ToString(), Resultante) == false)
                        {
                            //SI EXISTE AGREGAMOS
                            Resultante.Items.Add(elemento);
                            //AGREGAMOS ITEMS A LISTA AUXILIAR
                            Auxiliar.Add(elemento.ToString());
                        }
                    }

                    //RECORREMOS LISTA AUXILIAR
                    //ELIMINAMOS LOS ITEMS QUE COINCIDAN
                    foreach (var x in Auxiliar)
                    {
                        Primary.Items.Remove(x);
                    }
                }
            }
        }

        //ELIMINAR TODOS LOS ITEMS DE UNA LISTA
        private void EliminaMasivo(ListBoxControl Primary, ListBoxControl Resultante)
        {
            if (Resultante.Items.Count > 0)
            {
                //QUITAR TODOS LOS ELEMENTOS DE RESULTANTE Y AGREGAR A LA VEZ A LISTA PRIMARY
                if (Primary.ItemCount == 0)
                {
                    foreach (var item in Resultante.Items)
                    {
                        Primary.Items.Add(item);
                    }
                    //LIMPIAMOS RESULTANTE
                    Resultante.Items.Clear();
                }
                else
                {
                    List<string> Auxiliar = new List<string>();

                    //RECORREMOS RESULTANTE
                    foreach (var item in Resultante.Items)
                    {
                        if (ElementoExiste(item.ToString(), Primary) == false)
                        {
                            Primary.Items.Add(item);
                            //AGREGAMOS ITEMS A LISTA AUXILIAR
                            Auxiliar.Add(item.ToString());
                        }
                    }

                    //RECORREMOS LISTA AUXILIAR Y ELIMINAMOS
                    foreach (var x in Auxiliar)
                    {
                        Resultante.Items.Remove(x);
                    }
                }

            }
        }

        //GENERA LISTA EN BASE A ELEMENTO DE LISTBOX RESULTANTES
        private List<string> GeneraLista(ListBoxControl Data)
        {
            List<string> listado = new List<string>();
            if (Data.Items.Count > 0)
            {
                foreach (var item in Data.Items)
                {
                    listado.Add(item.ToString());
                }

                return listado;
            }
            else
            {
                return null;
            }
        }

        //LLENAR LISTA EN BASE A OTRA
        private List<string> GeneraListaFromOther(List<string> myList)
        {
            List<string> resultante = new List<string>();
            if (myList.Count>0)
            {
                foreach (var item in myList)
                {
                    resultante.Add(item);
                }
            }
            else
            {
                resultante = null;
            }

            return resultante;
        }

        //AGREGAR ELEMENTOS GENERADOS FROM FORMULARIO EXPORTAR A LISTBOX RESULTANTE
        private void AgregaElementos(List<string> MyList, ListBoxControl list)
        {
            if (MyList.Count>0)
            {
                foreach (var field in MyList)
                {
                    //AGREGAMOS CAMPOS A LISTBOX
                    list.Items.Add(field);
                }

                lblNumResultante.Text = list.Items.Count.ToString();
                lblNumPrimary.Text = ListBoxData.Items.Count.ToString();

            }            
        }

        //ELIMINAR ITEM DESDE LISTBOX DATA
        private void EliminarDataList(List<string> Listado, ListBoxControl List)
        {
            //LISTADO CONTIENE LOS CAMPOS USADOS
            if (Listado.Count > 0 && List.Items.Count>0)
            {
                //RECORRREMOS LSITADO
                foreach (var item in Listado)
                {
                    //SI EL CAMPO USADO COINCIDE CON EL CAMPO DEL LIST BOX LO ELIMINAMOS
                    if (ElementoExiste(item, List))
                        List.Items.Remove(item);
                }
            }
        }

        #endregion

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            if (ListBoxData.Items.Count>0)
            {
                //RECORREMOS TODOS LOS ITEMS QUE TIENE LA LISTA RESULTANTE
                //SI EL ITEM QUE SE INTENTA AGREGAR YA EXISTE HACEMOS RETURN
                if (ElementoExiste(ListBoxData.SelectedItem.ToString(), ListBoxResultante) == false)
                {
                    
                    //AGREGAR ITEM A LISTBOX RESULTANTE
                    ListBoxResultante.Items.Add(ListBoxData.SelectedItem);
                    //ELIMINAR ELEMENTO SELECIONADO DE LA LISTA PRIMARIA
                    ListBoxData.Items.Remove(ListBoxData.SelectedItem);

                    lblNumPrimary.Text = ListBoxData.Items.Count.ToString();
                    lblNumResultante.Text = ListBoxResultante.Items.Count.ToString();
                }
            }            
        }

        private void btnAgregaMultiple_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            AgregaMasivo(ListBoxData, ListBoxResultante);
            lblNumPrimary.Text = ListBoxData.Items.Count.ToString();
            lblNumResultante.Text = ListBoxResultante.Items.Count.ToString();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            //HACEMOS RESET DE LAS LISTAS
            CargarListBox(ListBoxData);

            ListBoxResultante.Items.Clear();

            lblNumPrimary.Text = ListBoxData.Items.Count.ToString();
            lblNumResultante.Text = ListBoxResultante.Items.Count.ToString();
        }

        private void btnQuitar_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            if (ListBoxResultante.Items.Count>0)
            {
                //SI NO EXISTE ELEMENTO EN LIST BOX MADRE AGREGAMOS
                if (ElementoExiste(ListBoxResultante.SelectedItem.ToString(), ListBoxData) == false)
                {
                    //AGREGAMOS ITEM...
                    ListBoxData.Items.Add(ListBoxResultante.SelectedItem);
                    ListBoxResultante.Items.Remove(ListBoxResultante.SelectedItem);
                    lblNumPrimary.Text = ListBoxData.Items.Count.ToString();
                    lblNumResultante.Text = ListBoxResultante.Items.Count.ToString();
                }               
            }            
        }

        private void btnQuitarMultiple_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            //AGREGAMOS TODOS LOS ITEMS DE LIST BOX RESULTANTE A LIST BOX MADRE
            EliminaMasivo(ListBoxData, ListBoxResultante);
            lblNumPrimary.Text = ListBoxData.Items.Count.ToString();
            lblNumResultante.Text = ListBoxResultante.Items.Count.ToString();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            if (ListBoxResultante.Items.Count>0)
            {
                ListBoxData.Items.Add(ListBoxResultante.SelectedItem);
                ListBoxResultante.Items.RemoveAt(ListBoxResultante.SelectedIndex);                
                lblNumPrimary.Text = ListBoxData.Items.Count.ToString();
                lblNumResultante.Text = ListBoxResultante.Items.Count.ToString();
            }
        }

        

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            if (AdministraColumna)
            {
                if (ListBoxResultante.Items.Count > 0)
                {
                    List<string> lista = new List<string>();
                    //GENERAMOS LISTADO DE CAMPOS
                    lista = GeneraLista(ListBoxResultante);

                    if (lista.Count == 0)
                    { XtraMessageBox.Show("No se pudo procesar campos", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Error); return;}

                    if (opener != null)
                    {
                        opener.CargarLista(lista);

                        XtraMessageBox.Show(lista.Count + " campos guardados correctamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Close();
                    }
                        
                }
                else
                {
                    XtraMessageBox.Show("No hay campos seleccionados", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    List<string> lista = new List<string>();                 
                    if (opener != null)
                    {
                        opener.CargarLista(lista);
                    }
                }
            }
            
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            Close();
        }
    }
}