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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using System.Reflection;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.Utils.DragDrop;
using DevExpress.Utils.Menu;
using DevExpress.XtraRichEdit.API.Native;
using System.Threading;
using System.Text.RegularExpressions;

namespace Labour
{
    public partial class frmGenerador : DevExpress.XtraEditors.XtraForm, IGenerador
    {
        Point p = Point.Empty;
        int IndexSeleccion = 0;

        private Generador Gen;
        private bool Edita = false;
        private JuegoDato juego = new JuegoDato();

        private Alerta Al { get; set; }

        /// <summary>
        /// Para guardar orden generado por usuario.
        /// </summary>
        private List<Orden> OrdenByUser = new List<Orden>();

       

        /// <summary>
        ///Para guardar las secciones generadas.
        /// </summary>
        private string SeccionField = "";
        private string SeccionFilter = "";
        private string SeccionOrderBy = "";
        private string SqlBase = "";
        private string SeccionRelation = "";
        /// <summary>
        /// Seleccion filtro de usuario
        /// </summary>
        private string SeccionUser = "";

        /// <summary>
        /// Nos permite realizar distintas operatorias con los reportes (en el caso de edicion).
        /// </summary>
        private ReportBuilder Reporte = new ReportBuilder();

        /// <summary>
        /// Array con condiciones para extraer la condion de una condicion
        /// </summary>
        private string[] conditions = new string[] { "=", "<>", ">=", "<=", "LIKE"};

        /// <summary>
        /// Guarda las tablas que se encuentran en la consulta guardada en base de datos.
        /// </summary>
        private List<string> TablesNameEdit = new List<string>();

        /// <summary>
        /// Para combo con campos de consulta en el caso de editar una consulta
        /// </summary>
        private List<formula> ListadoComboEditField = new List<formula>();

        /// <summary>
        /// Para guardar los filtros encontrados en consulta guardada en bd
        /// </summary>
        private List<Filtro> ListadoFiltrosEdit = new List<Filtro>();


        #region "INTERFAZ"
        /// <summary>
        /// Agrega los filtros seleccionados por el usuario a la consulta sql.
        /// </summary>
        /// <param name="pFiltros"></param>
        public void AgregaFiltros()
        {
            string CadFiltro = "";        

            //Limpiamos filtros que puedan existir
            //CleanFilter(richEditControl1.Text);
            if (richEditControl1.Text.Length > 0)
            {                
                CadFiltro = GeneraFiltro(richEditControl1.Text);

                //Guardamos cadena generada en cadena
                SeccionFilter = CadFiltro;

                OrdenarSecciones(SeccionField, SeccionFilter, SeccionOrderBy, SeccionRelation);

                Colores();
            }
        }

        public void ReordenSql(string pNewOrden, List<Orden> OrderbyList)
        {
            //REEMPLAZAMOS NUEVO ORDEN EN SQL GENERADO
            if (pNewOrden.Length == 0) return;
            StringBuilder bl = new StringBuilder();

            string OrderBySection = "";

            if (richEditControl1.Text.Length > 0)
            {
                //Guardamos nuevos ordenamiento de fields
                SeccionField = "SELECT \n" + pNewOrden;

                //ORDER BY SECTION
                OrderBySection = Orden.CreateListOrderBy(OrderbyList);
                OrdenByUser = OrderbyList;

                //Guardamos seccion order by si aplica
                SeccionOrderBy = OrderBySection;

                OrdenarSecciones(SeccionField, SeccionFilter, SeccionOrderBy, SeccionRelation);

                Colores();
            }
        }

        /// <summary>
        /// Carga y edicion desde base de datos
        /// </summary>
        /// <param name="pReport"></param>
        public void CargarSql(ReportBuilder pReport)
        {
            Reporte = pReport;
            richEditControl1.Document.Text = pReport.Sql;
            SetListCampos();
        }

        #endregion

        public frmGenerador()
        {
            InitializeComponent();          
            
        }

        private void ShowAlertaBackground()
        {
            Al = new Alerta();
            Al.Formulario = this;
            Al.AControl = new DevExpress.XtraBars.Alerter.AlertControl();
            Al.Mensaje = "Generando tablas...";
            Al.ShowMessage();
        }

        /// <summary>
        /// Para primera carga de tablas desde otro hilo
        /// </summary>
        private void CargaTablas()
        {
            juego = new JuegoDato();

            if (juego.Componentes != null)
            {
                if (juego.Componentes.Count > 0)
                {
                    foreach (Componente x in juego.Componentes)
                    {
                        imageListBoxControl1.Items.Add(x.Nombre);
                    }

                    SetImagesList();
                }
            }

            CreateColumns(treeList1);
            //CreateNodes(treeList1, juego.Componentes);
            treeList1.AfterCheckNode += TreeList1_AfterCheckNode;
        }

        private void frmGenerador_Load(object sender, EventArgs e)
        {
            treeList1.OptionsSelection.SelectNodesOnRightClick = true;
            //genera tablas y columnas con informacion del juego dedatos
            //Esto incluye nombre de tablas sus columnas y relaciones entre ellas
            juego = new JuegoDato();     

            if (juego.Componentes != null)
            {
                if (juego.Componentes.Count > 0)
                {
                    foreach (Componente x in juego.Componentes)
                    {                    
                        imageListBoxControl1.Items.Add(x.Nombre);                        
                    }

                    SetImagesList();
                }
            }

            CreateColumns(treeList1);
            //CreateNodes(treeList1, juego.Componentes);
            treeList1.AfterCheckNode += TreeList1_AfterCheckNode;
        }

        /// <summary>
        /// Evento que se lanza una vez se terminan las selecciones de campos.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeList1_AfterCheckNode(object sender, NodeEventArgs e)
        {
            ReloadSqlText();                
        }

        private void SetImagesList()
        {
            if (imageListBoxControl1.Items.Count > 0)
            {
                ImageListBoxItemCollection c = imageListBoxControl1.Items;
                for (int i = 0; i < c.Count; i++)
                {
                    c[i].ImageOptions.Image = Properties.Resources.Grid_16x16;
                }
            }
        }
     
        /// <summary>
        /// Lo usamos para generar el context menu (Menu Click derecho)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void imageListBoxControl1_MouseDown(object sender, MouseEventArgs e)
        {
            ImageListBoxControl c = sender as ImageListBoxControl;
            p = new Point(e.X, e.Y);
            /*Localizacion del item*/
            int selectedIndex = c.IndexFromPoint(p);
            if (selectedIndex == -1)
                p = Point.Empty;

            if (e.Button == MouseButtons.Right)
            {
                /*Obtenemos la posicion donde se hace click y dejamos seleccionado el item correspondiente en el listcontrol*/
                IndexSeleccion = imageListBoxControl1.IndexFromPoint(e.Location);
                SelectedItem(IndexSeleccion);       
                /*Mostramos context menu*/
                ShowPopUpDemo(e.Location, IndexSeleccion);
            }
        }  

        private void imageListBoxControl1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                if ((p != Point.Empty) && ((Math.Abs(e.X - p.X) > SystemInformation.DragSize.Width) || (Math.Abs(e.Y - p.Y) > SystemInformation.DragSize.Height)))
                    imageListBoxControl1.DoDragDrop(sender, DragDropEffects.Move);


        }

        private void imageListBoxControl2_DragDrop(object sender, DragEventArgs e)
        {
            ImageListBoxControl listBox = sender as ImageListBoxControl;
            Point newPoint = new Point(e.X, e.Y);
            newPoint = listBox.PointToClient(newPoint);
            int selectedIndex = listBox.IndexFromPoint(newPoint);
            //DataRowView row = imageListBoxControl1.SelectedItem as DataRowView;
            //(listBox.DataSource as DataTable).Rows.Add(new object[] { row[0], row[1]});

            object item = imageListBoxControl1.Items[imageListBoxControl1.IndexFromPoint(p)];
            //if (selectedIndex == -1)
            //    imageListBoxControl2.Items.Add(item);
            //else
            //    listBox.Items.Insert(selectedIndex, item);
        }

        private void imageListBoxControl2_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void CreateColumns(TreeList Tr)
        {
            Tr.BeginUpdate();
            TreeListColumn col1 = Tr.Columns.Add();
            col1.Caption = "Nombre";
            col1.VisibleIndex = 0;
            TreeListColumn col2 = Tr.Columns.Add();
            col2.Caption = "Primaria";
            col2.VisibleIndex = 1;            
            TreeListColumn col3 = Tr.Columns.Add();
            col3.Caption = "Foranea";
            col3.VisibleIndex = 2;
            Tr.EndUpdate();
        }


        /// <summary>
        /// Agrega nodos raiz y nodos hijos a un treelist
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="pComponentes"></param>
        private void CreateNodes(TreeList tr, List<Componente> pComponentes, string pTableName)
        {
            tr.BeginUnboundLoad();

            Componente comp1 = new Componente();          

            //Nodo raiz
            if (pComponentes.Count > 0)
            {                
                comp1 = pComponentes.Find(x => x.Nombre.ToLower().Equals(pTableName));               
                if (comp1 != null)
                {
                    TreeListNode parentForRootNodes = null;
                    TreeListNode RootNode = null;

                    //EL NODO RAIZ VA A SER EL NOMBRE DE LA TABLA
                    RootNode = tr.AppendNode(new object[] { comp1.Nombre}, parentForRootNodes);                         

                    //Nodo Hijo
                    for (int i = 0; i < comp1.Columnas.Count; i++)
                    {
                        object[] data = new object[] {
                            comp1.Columnas[i].Nombre,
                            comp1.Columnas[i].PrimaryKey == true? "*":"",
                        comp1.Columnas[i].ForeignKey == true? "*":""};
                        tr.AppendNode(data, RootNode);
                    }
                    
                    RootNode.ExpandAll();
                    NoExpandir(RootNode);                    
                }
            }

            /*Solo dejamos visible la primer columna (La de los nombre de los campos)*/
            treeList1.Columns[1].Visible = false;
            treeList1.Columns[2].Visible = false;
            
          
            //TreeListNode rootNode = tr.AppendNode(new object[] { "Alfreds Futterkiste", "Germany, Obere Str. 57", "030-0074321"}, parentForRootNodes);
            //Create child of the rootNode
            //tr.AppendNode(new object[] { "Suyama, Michael", "Obere Str. 55", "030-0074263"}, rootNode);

            tr.EndUnboundLoad();

        }    

        private void NoExpandir(TreeListNode pNode)
        {
            if (treeList1.Nodes.Count > 0)
            {
                foreach (TreeListNode x in treeList1.Nodes)
                {
                    if (x.GetValue(0).ToString().ToLower() != (pNode.GetValue(0).ToString().ToLower()))
                    {
                        x.Collapse();
                    }
                }
            }          
        }

        /// <summary>
        /// Elimina un nodo raiz y todos los nodos hijos de un control Treelist
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="pNode"></param>
        private void DeleteNode(TreeList tr, TreeListNode pNode)
        {           

            if (tr != null && pNode != null)
            {               
              tr.Nodes.Remove(pNode);
            }
        }

        private void treeList1_DragDrop(object sender, DragEventArgs e)
        {
          
        }

        private void SelectedItem(int itemIndex)
        {
            imageListBoxControl1.SelectedIndex = itemIndex;
        }

  

        private void ShowPopUpDemo(Point location, int itemIndex)
        {
            //Para obtener el item seleccionado
            ImageListBoxItem elemento = imageListBoxControl1.Items[itemIndex];
         
            ContextMenuStrip ContextStripImageList = new ContextMenuStrip();
            ContextStripImageList.Cursor = Cursors.Hand;
            ToolStripMenuItem StripItemImageList = new ToolStripMenuItem("Agregar " + elemento.Value.ToString(), Properties.Resources.Grid_16x16);
            StripItemImageList.Click += StripItemImageList_Click;            

            ContextStripImageList.Items.Add(StripItemImageList);            

            imageListBoxControl1.ContextMenuStrip = ContextStripImageList;
            imageListBoxControl1.ContextMenuStrip.Show(imageListBoxControl1, location);

        }

        /// <summary>
        /// Cuando se hace click en la tabla seleccionada del imagelistcontrol
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StripItemImageList_Click(object sender, EventArgs e)
        {
            int co = 0;
            /*Llenar treelist de acuerdo a tabla seleccionada*/
            DXMenuItem elemento = sender as DXMenuItem;
            //Obtenemos el valor del item selecionado
            string value = (string)imageListBoxControl1.Items[imageListBoxControl1.SelectedIndex].Value.ToString();

            //EXISTE ITEM EN TREELIST
            //LOS NODOS RAIZ
            co = treeList1.Nodes.Count(x => x.GetValue(0).ToString().ToLower().Equals(value.ToLower()));
            if (co > 0)
            {
                XtraMessageBox.Show("Tabla ya agregada", "Información", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            //Agregamos nodos de acuerdo a tabla seleccionada...
            if (juego.Componentes.Count > 0)
            {
                CreateNodes(treeList1, juego.Componentes, value.ToLower());
            }
        }

    

        private void treeList1_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            e.Menu.Items.Clear();

            TreeList tree = sender as TreeList;

            TreeListNode node = tree.GetNodeAt(e.Point.X, e.Point.Y);
            
            tree.SelectNode(node);
            if (tree.IsRootNode(node))
            {
                DXMenuItem item = new DXMenuItem();                
                item.Caption = "Eliminar tabla";
                item.ImageOptions.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("images/edit/delete_16x16.png");
                item.Click += Item_Click;

                e.Menu.Items.Add(item);
                
            }
        }

        private void Item_Click(object sender, EventArgs e)
        {
            /*Eliminamos nodo seleccionado*/
            TreeListNode nd =  treeList1.FocusedNode;
            string NameNode = "";

            NameNode = nd.GetValue(0).ToString();
            DialogResult adv = XtraMessageBox.Show($"¿Estás seguro de eliminar la tabla {NameNode}?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (adv == DialogResult.No)
                return;

            DeleteNode(treeList1, nd);

            //Limpiamos lista de ordenamiento
            OrdenByUser.Clear();

            if (treeList1.Nodes.Count == 0)
            {
                //no hay nodos raiz
                //Limpiamos filtros
                SeccionField = "";
                SeccionOrderBy = "";
                SeccionFilter = "";
                SeccionRelation = "";

                Filtro.FilterStack.Clear();
            }

            ReloadSqlText();

            //SeccionRelation = ExtraeSeccion("--<relation>", "--</relation>");
            //SqlBase = richEditControl1.Text;
            //SeccionField = ExtraeSeccion("--<field>", "--</field>");

            //OrdenarSecciones(SeccionField, SeccionFilter, SeccionOrderBy, SeccionRelation);
        }

        /// <summary>
        /// Permite obtener todos los elementos seleccionados del treelist
        /// <para>Retorna un listado de nodos.</para>
        /// </summary>
        private List<TreeListNode> CheckSeleccion()
        {
            List<TreeListNode> RootNodes = new List<TreeListNode>();
            TreeListNodes ChildNodes = null;

            try
            {
                List<TreeListNode> nodes = new List<TreeListNode>();                 

                foreach (TreeListNode root in treeList1.Nodes)
                {
                    ChildNodes = root.Nodes;
                    if (ChildNodes.Count > 0)
                    {
                        
                        int count = ChildNodes.Count(x => x.Checked);
                        if (count > 0)
                        {
                            RootNodes.Add(root);
                        }                                                 
                    }
                }               
               
            }
            catch (Exception ex)
            {
              //ERROR...
            }

            return RootNodes;
        }

        private List<TreeListNode> CheckSelectionEdit(List<Orden> pFields)
        {
            List<TreeListNode> RootNodes = new List<TreeListNode>();
            TreeListNodes ChildNodes = null;
            string ChildName = "", TableNameRoot = "";

            try
            {
                List<TreeListNode> nodes = new List<TreeListNode>();

                //Recorremos todos los nodos raiz que existan en el treeelist
                foreach (TreeListNode root in treeList1.Nodes)
                {
                    TableNameRoot = root.GetValue(0).ToString();
                    //Obtenemos todos los nodos hijos del nodo raiz.
                    ChildNodes = root.Nodes;
                    if (ChildNodes.Count > 0)
                    {
                        foreach (TreeListNode child in ChildNodes)
                        {
                            //Obtenemos el nombre de la columna
                            ChildName = child.GetValue(0).ToString();

                            if (pFields.Count(x => x.Campo.Equals($"{TableNameRoot}.{ChildName}")) > 0)
                            {
                                child.Checked = true;
                            }

                            //if (CheckedChild(TableNameRoot, ChildName, pFields))
                            //{
                            //    child.Checked = true;
                            //}
                        }
                        
                    }
                }

            }
            catch (Exception ex)
            {
                //ERROR...
            }

            return RootNodes;
        }

        /// <summary>
        /// Nos indica si debemos o no seleccionar un campo que sea hijo de un nodo raiz.
        /// </summary>
        /// <param name="pTableName"></param>
        /// <param name="pChildName"></param>
        /// <param name="pFields"></param>
        private bool CheckedChild(string pTableName, string pChildName, List<Orden> pFields)
        {
            bool check = false;
            if (pFields.Count > 0)
            {
                //La lista contiene el nombre de la tabla???
                if (pFields.Count(x => x.Campo.ToLower().TrimStart().TrimEnd().Contains(pTableName)) > 0)
                {
                    //Algun elemento de la lista contiene el nombre del campo
                    if (pFields.Count(y => y.Campo.ToLower().TrimStart().TrimEnd().TrimStart().Contains(pChildName)) > 0)
                    {
                        check = true;
                    }
                }
            }

            return check;
        }

        private void treeList1_MouseDown(object sender, MouseEventArgs e)
        {
          
        }

        /// <summary>
        /// Monitorea la selecciona o deseleccionde un node en el treelist (equivalente al check)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeList1_NodeChanged(object sender, NodeChangedEventArgs e)
        {
            TreeList tree = sender as TreeList;

            if (e.ChangeType == NodeChangeTypeEnum.CheckedState)
            {
                /*para monitorear si se han seleccionado nodos*/
                if (tree.IsRootNode(e.Node))
                {
                    if (e.Node.Checked)
                    {
                        //SELECCIONAMOS TODOS LOS HIJOS
                        if (e.Node.HasChildren)
                        {
                            e.Node.CheckAll();
                        }
                    }
                    else
                    {
                        if (e.Node.HasChildren)
                        {
                            e.Node.UncheckAll();
                        }
                    }
                }

                //Actualizamos sql text
                //ReloadSqlText();

            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            if (treeList1.Nodes.Count == 0)
            { XtraMessageBox.Show("No hay tablas seleccionadas", "Tablas", MessageBoxButtons.OK, MessageBoxIcon.Warning);return; }

            ReloadSqlText();

            //SeccionRelation = ExtraeSeccion("--<relation>", "--</relation>");
            //SqlBase = richEditControl1.Text;       
            //SeccionField = ExtraeSeccion("--<field>", "--</field>");

            //OrdenarSecciones(SeccionField, SeccionFilter, SeccionOrderBy, SeccionRelation);
        }

        /// <summary>
        /// Permite cambiar el color a una palabra dentro del control.
        /// </summary>
        /// <param name="pWord">Palabra a buscar</param>
        /// <param name="pFontColor">Nuevo color.</param>
        private void ChangeColorText(string pWord, Color pFontColor)
        {                      
            DocumentRange rg = richEditControl1.Document.Range;

            DocumentRange[] words = richEditControl1.Document.FindAll(pWord, SearchOptions.CaseSensitive, rg);

            if (words.Length > 0)
            {
                foreach (var wd in words)
                {
                   
                    CharacterProperties cp = richEditControl1.Document.BeginUpdateCharacters(wd);
                    cp.ForeColor = pFontColor;

                    richEditControl1.Document.EndUpdateCharacters(cp);
                    //richEditControl1.Document.EndUpdateCharacters(cp);
                }
            }
        }

        /// <summary>
        /// Elimina una cadena dentro del richEditControl.
        /// </summary>
        /// <param name="pWord"></param>
        /// <param name="pFontColor"></param>
        private void RemoveWord(string pWord)
        {
            if (pWord.Length == 0) return;

            try
            {
                DocumentRange rg = richEditControl1.Document.Range;

                DocumentRange[] words = richEditControl1.Document.FindAll(pWord, SearchOptions.CaseSensitive, rg);

                if (words.Length > 0)
                {
                    foreach (var wd in words)
                    {
                        richEditControl1.Document.Delete(wd);

                    }
                }
            }
            catch (Exception ex)
            {
                //ERROR...
            }
        }

        /// <summary>
        /// Oculta una cadena dentro del sql generado.
        /// </summary>
        /// <param name="pWord"></param>
        private void HideText(string pWord, bool? ShowText = false)
        {
            DocumentRange rg = richEditControl1.Document.Range;

            DocumentRange[] words = richEditControl1.Document.FindAll(pWord, SearchOptions.CaseSensitive, rg);

            if (words.Length > 0)
            {
                foreach (var wd in words)
                {
                    CharacterProperties cp = richEditControl1.Document.BeginUpdateCharacters(wd);
                    if (ShowText == true)
                        cp.Hidden = false;
                    else
                        cp.Hidden = true;                    

                    richEditControl1.Document.EndUpdateCharacters(cp);
                }
            }
        }

        /// <summary>
        /// Extrae una seccion del sql
        /// </summary>
        private void ExtraeyReemplazaSeccion(string pSectionStart, string pSectionEnd, string pNewSection, bool? AgregaSelect = false)
        {
            DocumentRange rg = richEditControl1.Document.Range;

            try
            {
                DocumentRange[] Comienza = richEditControl1.Document.FindAll(pSectionStart, SearchOptions.CaseSensitive, rg);
                DocumentRange[] Termina = richEditControl1.Document.FindAll(pSectionEnd, SearchOptions.CaseSensitive, rg);              
        
                int s = 0, e = 0;
                int t1 = 0, t2 = 0, t3 = 0, t4 = 0;

                t1 = Comienza[0].Start.ToInt();
                t2 = Comienza[0].End.ToInt();
                t3 = Termina[0].End.ToInt();
                t4 = Termina[0].Start.ToInt();

                e = Termina[0].Start.ToInt();

                StringBuilder buil = new StringBuilder();

                //genera la posicion de inicio y de termino dentro un tag especifico (No considera los tag propiamente tal)
                DocumentRange r1 = richEditControl1.Document.CreateRange(t2, t4 - t2);
                //Considera los tag
                //DocumentRange r2 = richEditControl1.Document.CreateRange(t1, t3 - t1);
                if (r1.Length > 0)
                {                    
                    //ELIMINAMOS TEXTO
                    richEditControl1.Document.Delete(r1);

                    //REPLACE NUEVO TEXTO
                    DocumentPosition pos = richEditControl1.Document.CreatePosition(r1.Start.ToInt());                    
                    if(AgregaSelect == true)
                        buil.AppendLine("\nSELECT");
                    
                    buil.AppendLine(pNewSection);

                    richEditControl1.Document.InsertText(pos, buil.ToString());

                    ChangeColorText("SELECT", Color.Blue);
                    ChangeColorText("FROM", Color.Red);
                    ChangeColorText("INNER", Color.Red);
                    ChangeColorText("JOIN", Color.Red);
                    ChangeColorText("AND", Color.Red);
                    ChangeColorText("ON", Color.Red);
                    ChangeColorText("OR", Color.Red);
                    ChangeColorText("WHERE", Color.Red);
                    ChangeColorText("BETWEEN", Color.Red);
                    ChangeColorText("LIKE", Color.Red);
                }
            }
            catch (Exception ex)
            {
                //ERROR...                
            }
            
        }

        /// <summary>
        /// Extrae una seccion dentro del sql.
        /// </summary>
        /// <param name="pSectionStart"></param>
        /// <param name="pSectionEnd"></param>
        private string ExtraeSeccion(string pSectionStart, string pSectionEnd, bool IncluyeTag = true)
        {
            DocumentRange rg = richEditControl1.Document.Range;
            string extrae = "";

            try
            {
                DocumentRange[] Comienza = richEditControl1.Document.FindAll(pSectionStart, SearchOptions.CaseSensitive, rg);
                DocumentRange[] Termina = richEditControl1.Document.FindAll(pSectionEnd, SearchOptions.CaseSensitive, rg);
                DocumentRange r2 = null;

                int s = 0, e = 0;
                int t1 = 0, t2 = 0, t3 = 0, t4 = 0;

                t1 = Comienza[0].Start.ToInt();
                t2 = Comienza[0].End.ToInt();
                t3 = Termina[0].End.ToInt();
                t4 = Termina[0].Start.ToInt();

                e = Termina[0].Start.ToInt();

                StringBuilder buil = new StringBuilder();

                //genera la posicion de inicio y de termino dentro un tag especifico (No considera los tag propiamente tal)
                //DocumentRange r1 = richEditControl1.Document.CreateRange(t2, t4 - t2);
                //Considera los tag
                if (IncluyeTag)
                    r2 = richEditControl1.Document.CreateRange(t1, t3 - t1);
                else
                    r2 = richEditControl1.Document.CreateRange(t2, t4 - t2);

                if (r2.Length > 0)
                {
                    //REPLACE NUEVO TEXTO
                    //DocumentPosition pos = richEditControl1.Document.CreatePosition(r2.Start.ToInt());                
                    //extrae = richEditControl1.Text.Substring(r2.Start.ToInt(), richEditControl1.Text.Length);

                    extrae = richEditControl1.Document.GetText(r2);
                }
            }
            catch (Exception ex)
            {
                //ERROR...                
            }

            return extrae;
        }

        /// <summary>
        /// Entrega la posicion de inicio o termino de una palabra dentro de richcontrol
        /// </summary>
        /// <param name="pFIndText">Palabra consultada</param>
        /// <param name="Option">Opciones (inicio o termino)</param>
        /// <returns></returns>
        private int GetPositionText(string pFIndText, int Option)
        {
            DocumentRange rg = richEditControl1.Document.Range;
            int Position = 0;


            try
            {
                DocumentRange[] RangoPalabra = richEditControl1.Document.FindAll(pFIndText, SearchOptions.CaseSensitive, rg);
                if (RangoPalabra.Length > 0)
                {
                    //Posicion inicial y final de la cadena encontrada
                    if (Option == 1)
                        Position = RangoPalabra[0].Start.ToInt();
                    if (Option == 2)
                        Position = RangoPalabra[0].End.ToInt();

                }

            }
            catch (Exception ex)
            {
                //error..
            }

            return Position;
            

        }

        /// <summary>
        /// Extraer toda la informacion desde el sql guardado en base de datos.
        /// <para>Genera cada parte correspondiente de una cadena sql; campos, filtros, relaciones, etc.</para>
        /// </summary>
        private void SetListCampos()
        {
            //SECCION CAMPOS
            string secFields = ExtraeSeccion("--<field>", "--</field>", false);
            string secOrderBy = ExtraeSeccion("--<orderby>", "--</orderby>", false);
            string secFilter = ExtraeSeccion("--<filter>", "--</filter>", false);
            string secRelation = ExtraeSeccion("--<relation>", "--</relation>", false);
            string Alias = "";

            /*Guardamos cada seccion encontrada en las variables globales*/
            SeccionField = ExtraeSeccion("--<field>", "--</field>");
            SeccionFilter = ExtraeSeccion("--<filter>", "--</filter>");
            SeccionOrderBy = ExtraeSeccion("--<orderby>", "--</orderby>");
            SeccionRelation = ExtraeSeccion("--<relation>", "--</relation>");

            treeList1.ClearNodes();
            TablesNameEdit.Clear();

            /*Listado que nos permite obtener el listado de campos, su orden, si se ocupan en order by etc.*/
            OrdenByUser.Clear();

            //Para combobox con listado de campos.
            ListadoComboEditField.Clear();

            List<Orden> fields = new List<Orden>();
            /*Listado que nos permite tener todos los filtros que existen en la consulta sql.*/
            
            List<Filtro> Filtros = new List<Filtro>();

            //List<formula> FieldsCombo = new List<formula>();

            //Extraer campos
            if (secFields.Length > 0)
            {
                secFields = secFields.Replace("\n", "");
                secFields = secFields.Replace("\r", "");
                secFields = secFields.Replace("\"", "");
                secFields = secFields.Replace("SELECT", "");

                secOrderBy = secOrderBy.Replace("\n", "");
                secOrderBy = secOrderBy.Replace("\r", "");
                secOrderBy = secOrderBy.Replace("\"", "");
                secOrderBy = secOrderBy.Replace("ORDER BY", "");

                string[] dataFields = secFields.Split(',');
                string[] dataOrder = secOrderBy.Split(',');

                if (dataFields.Length > 0 && dataOrder.Length > 0)
                {                    

                    for (int i = 0; i < dataFields.Length; i++)
                    {
                        formula fm = new formula();

                        Orden ord = new Orden();
                        Alias = ExtraeAlias(dataFields[i]);

                        ord.Campo = ExtraerCampo(dataFields[i]);
                        ord.Posicion = i + 1;
                        ord.visible = true;
                        ord.Alias = Alias;                        

                        fm.key = ord.Campo;
                        fm.desc = ord.Campo;

                        //Solo para combobox
                        //FieldsCombo.Add(fm);
                        ListadoComboEditField.Add(fm);

                        //Extraemos nombre de tabla
                        ExtraeTablas(ord.Campo);

                        //RECORRER dataorder 
                        if (dataOrder.Length > 0)
                        {
                            //EXISTE O COINCIDE EL CAMPO CON ALGUN CAMPO ENCONTRATO EN LA SECCION DE ORDERBY??
                            if (dataOrder.Count(x => x.Contains(ord.Campo)) > 0)
                            {
                                ord.OrderBy = true;
                                if (OrderType(ord.Campo, dataOrder, " DESC"))
                                    ord.OrderType = "DESC";
                                if (OrderType(ord.Campo, dataOrder, " ASC"))
                                    ord.OrderType = "ASC";
                            }
                            else
                            {
                                ord.OrderBy = false;
                                ord.OrderType = "DESC";
                            }
                        }

                        //Agregamos data a listado
                        //fields.Add(ord);                       
                        fields.Add(ord);

                        //if(ord.OrderBy)
                            OrdenByUser.Add(ord);
                    }
                }
            }

            //Extrae filtros 
            if (secFilter.Length > 0)
            {
                secFilter = secFilter.Replace("\r", "");
                secFilter = secFilter.Replace("\n", "");

                //string[] separa = secFilter.Split(new string[] {"AND", "OR"}, StringSplitOptions.None);
                string[] separa = Regex.Split(secFilter, @"(?<=AND|OR)");
                //Guardamos filtros en listado correspondiente
                if (separa.Length > 0)
                {
                    for (int i = 0; i < separa.Length; i++)
                    {
                        Filtro fil = new Filtro();
                        fil.Nombre = ExtraeNombreFiltro(separa[i]);
                        fil.Condicion = ExtraeOperadorFiltro(separa[i]);
                        fil.Valor = ExtraeValorFiltro(separa[i]);
                        if (separa[i].Contains("AND"))
                            fil.Union = "AND";
                        else
                            fil.Union = "OR";

                        //Agregamos objeto a listado
                        Filtros.Add(fil);                        
                    }
                }
            }

            //Algun elemento del listado de filtros no existe en el listado para campos de la consulta
            //List<Filtro> NoEncontradosList = new List<Filtro>();
            FindNoFilterInField(Filtros, fields);
            FindNoFilterInField(Filtros, OrdenByUser);
            AddToListField(Filtros, ListadoComboEditField);

            //SETEAR LISTADO ESTATICO DE CLASE DE ACUERDO A FILTROS ENCONTRADOS
            Filtro.FilterStack = Filtros;            

            //Creamos nodos correspondientes en treelist
            foreach (var tabl in TablesNameEdit)
            {
                CreateNodes(treeList1, juego.Componentes, tabl.ToLower().TrimStart().TrimEnd());
            }

            ///Seleccionamos campos correspondientes.
            CheckSelectionEdit(fields);

            List<TreeListNode> nodos = new List<TreeListNode>();
            nodos = CheckSeleccion();
            Gen = new Generador(nodos, juego.Componentes);
            Gen.SetFieldList(ListadoComboEditField);
            Colores();

        }

        private void SetListCamposPersonalizados(string pField)
        {
            List<Orden> Listado = new List<Orden>();

            if (pField.Length > 0)
            {
                string[] campos = pField.Split(',');
                if (campos.Length > 0)
                {
                    for (int i = 0; i < campos.Length; i++)
                    {
                        Listado.Add(new Orden() {
                            Campo = ExtraerCampo(campos[i]), 
                            Posicion = i + 1, 
                            visible = true, 
                            Alias = ExtraeAlias(campos[i])
                        });
                    }
                }
            }
        }

        /// <summary>
        /// Agrega los campos que no estén dentro de los encontrados en la lista pero que si son parte de los filtros o que pueden estar no visibles
        /// </summary>
        /// <param name="Filtros"></param>
        /// <param name="CamposSql"></param>
        private void FindNoFilterInField(List<Filtro> Filtros, List<Orden> CamposSql)
        {
            int c = 0;
            if (CamposSql.Count > 0 && Filtros.Count > 0)
            {
                //Algun elemento del listado de filtros no existe en el listado para campos de la consulta???
                foreach (Filtro fil in Filtros)
                {
                    //Buscar en camposSql
                    //Si devuelve 0, es porque no encontró el campo dentro de la lista de campos
                    if (CamposSql.Count(x => x.Campo.Contains(fil.Nombre)) == 0)
                    {
                        c = CamposSql.Count + 1;
                        //Agregamos elemento a camposSql
                        CamposSql.Add(new Orden() { Alias = "", Campo = fil.Nombre, OrderBy = false, OrderType = "ASC", Posicion = c, visible = false});
                    }
                }
            }
        }

        private void AddToListField(List<Filtro> Filtros, List<formula> CamposSql)
        {          
            if (CamposSql.Count > 0 && Filtros.Count > 0)
            {
                //Algun elemento del listado de filtros no existe en el listado para campos de la consulta???
                foreach (Filtro fil in Filtros)
                {
                    //Buscar en camposSql
                    //Si devuelve 0, es porque no encontró el campo dentro de la lista de campos
                    if (CamposSql.Count(x => x.desc.Contains(fil.Nombre)) == 0)
                    {
                        //Agregamos elemento a camposSql
                        CamposSql.Add(new formula() { desc = fil.Nombre, key = fil.Nombre});
                    }
                }
            }
        }

        /// <summary>
        /// Extrae nombre de tabla desde campos (para generacion de treelist nodes)
        /// <para>Ejemplo: NombreTabla.CampoTabla</para>
        /// </summary>
        private void ExtraeTablas(string pCampo)
        {
            int pos = 0;
            string Table = "";
            try
            {
                if (pCampo.Length > 0)
                {
                    //Obtenemos la posicion del punto
                    pos = pCampo.IndexOf(".");
                    if (pos > 0)
                    {
                        Table = pCampo.Substring(0, pos);

                        //Existe registro en listado??
                        //Si no existe lo agregamos
                        if (TablesNameEdit.Count(x => x.Contains(Table)) == 0)
                            TablesNameEdit.Add(Table);
                    }
                }
            }
            catch (Exception ex)
            {
                //ERROR...
            }
        } 

        /// <summary>
        /// Extrae el alias del field.
        /// </summary>
        /// <param name="pCampo"></param>
        private string ExtraeAlias(string pCampo)
        {
            string al = "";
            int pos = 0;
            try
            {
                if (pCampo.Length > 0)
                {
                    if (pCampo.Contains(" as "))
                    {
                        pos = pCampo.IndexOf(" as ");
                        if (pos != 0)
                        {
                            al = pCampo.Substring(pos + 4, (pCampo.Length) - (pos + 4));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //ERROR...
            }

            return al;
        }

        /// <summary>
        /// Extrae solo nombre del campo sin considerar el alias, si existe.
        /// </summary>
        /// <param name="pCampo"></param>
        /// <returns></returns>
        private string ExtraerCampo(string pCampo)
        {
            string al = "";
            int pos = 0;
            try
            {
                if (pCampo.Length > 0)
                {
                    if (pCampo.Contains(" as "))
                    {
                        pos = pCampo.IndexOf(" as ");
                        if (pos != 0)
                        {
                            al = pCampo.Substring(0, pos);
                        }
                    }
                    else
                    {
                        al = pCampo;
                    }

                    if (al.Length > 0)
                        al = al.TrimStart().TrimEnd();
                }
            }
            catch (Exception ex)
            {
                //ERROR...
            }

            return al;
        }

        /// <summary>
        /// Extrae porcion de cadena que indica si es un orden ascendente o descendente.
        /// </summary>
        /// <param name="pCampo"></param>
        /// <returns></returns>
        private string ExtraerOrderBy(string pCampo, string[] pData)
        {
            string data = "";
            int pos = 0;
            string Element = "";
            try
            {
                if (pData.Length > 0)
                {
                    //Extraemos elemento de lista que contenga la palabra de la variable pcampo
                    Element = pData.First(x => x.Contains(pCampo));

                    if (Element.Length > 0)
                    {
                        if (Element.Contains(" ASC"))
                        {
                            pos = Element.IndexOf(" ASC");
                            data = Element.Substring(pos + 4, Element.Length - (pos + 4));
                        }
                        else if (Element.Contains(" DESC"))
                        {
                            pos = Element.IndexOf(" DESC");
                            data = Element.Substring(pos + 5, Element.Length - (pos + 5));
                        }
                    }
                }

              
            }
            catch (Exception ex)
            {
                //ERROR...
            }

            return data;
        }

        /// <summary>
        /// Extrae de cadena la parte que nos indica si el orden es ascendente o descendente.
        /// </summary>
        /// <param name="pCampo"></param>
        /// <param name="pData"></param>
        /// <param name="pType"></param>
        /// <returns></returns>
        private bool OrderType(string pCampo, string[] pData, string pType)
        {
            string Element = "";
            bool correcto = false;
            try
            {
                if (pData.Length > 0)
                {
                    //Extraemos elemento de lista que contenga la palabra de la variable pcampo
                    Element = pData.First(x => x.Contains(pCampo));

                    if (Element.Length > 0)
                    {
                        if (Element.Contains(pType))
                        {
                            correcto = true;   
                        }                     
                    }
                }

            }
            catch (Exception ex)
            {
                //ERROR...
            }

            return correcto;
        }

        /// <summary>
        /// Extrae solo el campo del filtro
        /// </summary>
        private string ExtraeNombreFiltro(string Filtro)
        {
            int pos = 0;
            string sub = "";
            try
            {
                if (Filtro.Length > 0)
                {

                    //OPTENER POSICION DEL OPERADOR DE CONDICION
                    for (int i = 0; i < conditions.Length; i++)
                    {
                        //La cadena contiene el operador
                        if (Filtro.Contains(conditions[i]))
                        {
                            //Posicion???
                            pos = Filtro.IndexOf(conditions[i]);
                            break;
                        }
                    }


                    if (pos > 0)
                    {
                        sub = Filtro.Substring(0, pos);
                        if (sub.Length > 0)
                            sub = sub.TrimStart().TrimEnd();
                    }

                }
            }
            catch (Exception ex)
            {
                //ERROR
                sub = "";
            }

            return sub;
        }

        /// <summary>
        /// Extrae solo el operador de un filtro
        /// </summary>
        /// <param name="Filtro"></param>
        /// <returns></returns>
        private string ExtraeOperadorFiltro(string Filtro)
        {           
            string sub = "";
            try
            {
                if (Filtro.Length > 0)
                {
                    
                    //OPTENER POSICION DEL OPERADOR DE CONDICION
                    for (int i = 0; i < conditions.Length; i++)
                    {
                        //La cadena contiene el operador
                        if (Filtro.Contains(conditions[i]))
                        {
                            //Posicion???
                            sub = conditions[i];
                            break;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                //ERROR
                sub = "";
            }

            return sub;
        }

        /// <summary>
        /// Extrae el valor de la condicion de filtro.
        /// </summary>
        /// <param name="Filtro"></param>
        /// <returns></returns>
        private string ExtraeValorFiltro(string Filtro)
        {
            int pos = 0, posUnion = 0;
            string sub = "";

            try
            {
                if (Filtro.Length > 0)
                {
                    //OBTENER POSICION DEL OPERADOR DE CONDICION
                    for (int i = 0; i < conditions.Length; i++)
                    {
                        //La cadena contiene el operador
                        if (Filtro.Contains(conditions[i]))
                        {
                            //Posicion???
                            pos = Filtro.IndexOf(conditions[i]);
                            break;
                        }
                    }

                    if (pos > 0)
                    {
                        sub = Filtro.Substring(pos + 1, Filtro.Length - (pos + 1));

                        if (sub.Contains(" AND"))
                        {
                            posUnion = sub.IndexOf(" AND");
                        }
                        else if (sub.Contains(" OR"))
                        {
                            posUnion = sub.IndexOf(" OR");
                        }

                        //QUITAMOS AND 
                        if(posUnion > 0)
                            sub = sub.Substring(0, posUnion);
                    }
                }
            }
            catch (Exception ex)
            {
                //ERROR
                sub = "";
            }

            return sub;
        }

        private void SearchText()
        {
            Document doc = richEditControl1.Document;
            Paragraph par = doc.Paragraphs.Get(richEditControl1.Document.CaretPosition);
            string text = doc.GetText(par.Range);

            ISearchResult search = richEditControl1.Document.StartSearch("");

            //get current caret position
            DocumentPosition pos = richEditControl1.Document.CaretPosition;

            while (search.FindNext())
            {

                if (search.CurrentResult.Contains(pos))
                {
                    search.Replace("");
                    doc.CaretPosition = search.CurrentResult.Start;
                }
            }

        }

        /// <summary>
        /// Permite ordenar las secciones de acuerdo a sql.
        /// </summary>
        private void OrdenarSecciones(string pNewField, string pNewFilter, string pNewOrder, string pRelations)
        {
            /*
             * 1- SECCION SELECT 
             * 2- SECCION WHERE CON CONDICIONES
             * 3- SECCION ORDER BY
             */
            string fuser = "";

            StringBuilder Sections = new StringBuilder();

            if (richEditControl1.Text.Length > 0)
            {
                richEditControl1.Text = "";

                //SECCION CAMPOS
                if (pNewField.Length > 0)
                {
                    if (pNewField.Contains("--<field>") == false)
                        Sections.AppendLine("--<field>");
                    Sections.AppendLine(pNewField);
                    if (pNewField.Contains("--</field>") == false)
                        Sections.AppendLine("--</field>");                  
                }

                //SECCION RELACIONES
                if (pRelations.Length > 0)
                {                   
                    Sections.AppendLine(pRelations);                 
                }

                //SECCION FILTROS
                if (pNewFilter.Length > 0)
                {
                    if (richEditControl1.Text.Contains("WHERE"))
                    {
                        if(pNewFilter.Contains("--<filter>") == false)
                            Sections.AppendLine("--<filter>");
                        Sections.AppendLine(pNewFilter);
                        if(pNewFilter.Contains("--</filter>") == false)
                            Sections.AppendLine("--</filter>");                      
                    }
                    else
                    {
                        Sections.AppendLine("WHERE");
                        if (pNewFilter.Contains("--<filter>") == false)
                            Sections.AppendLine("--<filter>");
                        Sections.AppendLine(pNewFilter);
                        if (pNewFilter.Contains("--</filter>") == false)
                            Sections.AppendLine("--</filter>");
                 
                    }

                    /*Seccion Filtros de usuario*/
                    if (Calculo.GetSqlFiltro(User.GetUserFilter(), "", User.ShowPrivadas(), true) != "")
                    {
                        Sections.AppendLine("--<user>");
                        Sections.AppendLine($"AND {AgregaTablaUserFilter(pNewField)} IN (SELECT contrato FROM trabajador  {Calculo.GetSqlFiltro(User.GetUserFilter(), "", User.ShowPrivadas(), true)})");
                        Sections.AppendLine("--</user>");
                    }                    
                }
                else
                {
                    /*Agregamos solo el filtro de usuario si es que existe*/
                    /*Seccion Filtros de usuario*/
                    fuser = Calculo.GetSqlFiltro(User.GetUserFilter(), "", User.ShowPrivadas(), true);

                    

                    //Solo agregamos filtro si dentro de la relacion hay alguna tabla que tenga la columna contrato
                    if (fuser != "")
                    {
                        Sections.AppendLine("--<user>");                                                
                        Sections.AppendLine($"WHERE {AgregaTablaUserFilter(pNewField)} IN (SELECT contrato FROM trabajador  {Calculo.GetSqlFiltro(User.GetUserFilter(), "", User.ShowPrivadas(), true)})");
                        Sections.AppendLine("--</user>");
                    }
                }

                //SECCION ORDEN
                if (pNewOrder.Length > 0)
                {
                    if(pNewOrder.Contains("--<orderby>") == false)
                        Sections.AppendLine("--<orderby>");
                    if(pNewOrder.Contains("ORDER BY ") == false)
                        Sections.AppendLine("ORDER BY ");
                    Sections.AppendLine(pNewOrder);
                    if(pNewOrder.Contains("--</orderby>") == false)
                        Sections.AppendLine("--</orderby>");
                }

                richEditControl1.Text = Sections.ToString();

                //Change color text.
                Colores();

            }
        }

        /// <summary>
        /// Agrega tabla de acuerdo a tablas encontradas en fields
        /// </summary>
        private string AgregaTablaUserFilter(string pSectionFields)
        {
            string data = "";
            if (pSectionFields.Length > 0)
            {
                if (pSectionFields.ToLower().Contains("itemtrabajador."))
                {
                    data = "itemtrabajador.contrato";
                }
                else if (pSectionFields.ToLower().Contains("liquidacionhistorico."))
                {
                    data = "liquidacionhistorico.contrato";
                }
                else if (pSectionFields.ToLower().Contains("calculomensual."))
                {
                    data = "calculomensual.contrato";
                }
                else if (pSectionFields.ToLower().Contains("ausentismo."))
                {
                    data = "ausentismo.contrato";
                }
                else if (pSectionFields.ToLower().Contains("vacacion."))
                {
                    data = "vacacion.contrato";
                }
                else if (pSectionFields.ToLower().Contains("vacaciondetalle."))
                {
                    data = "vacaciondetalle.contrato";
                }
                else if (pSectionFields.ToLower().Contains("cargafamiliar."))
                {
                    data = "cargafamiliar.contrato";
                }
                else if (pSectionFields.ToLower().Contains("trabajador."))
                {
                    data = "trabajador.contrato";
                }
            }

            return data;
        }

        /// <summary>
        /// Solo para saber si aplica
        /// </summary>
        /// <param name="pFields"></param>
        /// <returns></returns>
        private bool AplicaFiltroContrato(string pFields)
        {
            bool aplica = false;

            if (pFields.ToLower().Contains("trabajador") || pFields.ToLower().Contains("itemtrabajador") ||
                pFields.ToLower().Contains("vacacion") || pFields.ToLower().Contains("vacaciondetalle") ||
                pFields.ToLower().Contains("ausentismo") || pFields.ToLower().Contains("calculomensual") || 
                pFields.ToLower().Contains("liquidacionhistorico"))
                aplica = true;

            return aplica;
        }

        private void richEditControl1_PopupMenuShowing(object sender, DevExpress.XtraRichEdit.PopupMenuShowingEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        /// <summary>
        /// Cambia el color a cadenas
        /// </summary>
        private void Colores()
        {
            //VOLVER A COLOCAR LOS COLORES AL TEXTO
            ChangeColorText("SELECT", Color.Blue);
            ChangeColorText("FROM", Color.Red);
            ChangeColorText("INNER", Color.Red);
            ChangeColorText("JOIN", Color.Red);
            ChangeColorText("AND", Color.Red);
            ChangeColorText("ON", Color.Red);
            ChangeColorText("OR", Color.Red);
            ChangeColorText("WHERE", Color.Red);
            ChangeColorText("BETWEEN", Color.Red);
            ChangeColorText("LIKE", Color.Red);
            ChangeColorText("ORDER BY", Color.Green);
        }

        /// <summary>
        /// Mostramos datos en ventana auxiliar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDatos_Click(object sender, EventArgs e)
        {           

            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            BuilderSql builder = new BuilderSql();
            DataTable data = new DataTable();

            if (treeList1.Nodes.Count == 0)
            { XtraMessageBox.Show("Selecciona al menos una tabla", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            if (richEditControl1.Text.Length > 0)
            {
                builder.CustomSql = richEditControl1.Text;
                data = builder.GetDataSource(txtSalidaLog);
                if (data.Rows.Count > 0)
                {
                    //PASAMOS DATA PARA FORMULARIO CORRESPONDIENTE
                    frmDataGeneador fd = new frmDataGeneador(data);
                    fd.StartPosition = FormStartPosition.CenterParent;
                    fd.ShowDialog();
                }
                else
                {
                    XtraMessageBox.Show("No se pudo generar la información", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                XtraMessageBox.Show("No se pudo generar información", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            if (richEditControl1.Text.Length > 0)
            {
                DialogResult adv = XtraMessageBox.Show("¿Estás seguro que deseas salir?\nLos cambios no guardados se perderán.", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (adv == DialogResult.No)
                    return;
            }           

            Filtro.FilterStack.Clear();
            Close();
        }
      

        private void btnExcel_Click(object sender, EventArgs e)
        {

        }     

        private void btnFiltro_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (treeList1.Nodes.Count == 0)
            { XtraMessageBox.Show("Selecciona al menos una tabla", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);return; }

            List<formula> Fields = new List<formula>();
            if (Gen != null)
            {
                Fields = Gen.GetFieldList();
                if (Fields.Count > 0)
                {
                    //LO PASAMOS AL FORMULARIO DE FILTROS A TRAVES DE SU CONSTRUCTOR.
                    frmFiltroGenerador fil = new frmFiltroGenerador(Fields);
                    fil.OpenGen = this;
                    fil.StartPosition = FormStartPosition.CenterParent;
                    fil.ShowDialog();
                }
            }       
      
        }

        /// <summary>
        /// Crea el sql en el control richeditcontrol
        /// </summary>
        private void ReloadSqlText()
        {
            List<TreeListNode> Nodos = new List<TreeListNode>();
            //Todos los nodos raiz que tienen el 'check'
            Nodos = CheckSeleccion();

            string sql = "", ReplaceSection = "";     

            if (Nodos.Count > 0)
            {
                //Mostrar mensaje cuando las tablas seleccionadas no tengan relacion entre si
                //o no haya manera de hacer una realacion entre tablas

                Gen = new Generador(Nodos, juego.Componentes);
                if (Nodos.Count > 1)
                {
                    if (Gen.NoHayRelacionTablas())
                    {
                        XtraMessageBox.Show("No se encontró relacion entre tablas", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }
                }
                   
                sql = Gen.GetSql();
                
                if (sql.Length > 0)
                {
                    //1- Hay order by generados por el usuario??
                    //2- Hay Alias generado por el usuario??                   

                    SqlBase = sql;

                    richEditControl1.Text = sql;

                    //Replace seccion campos (si aplica), por data ordenada y con alias.
                    if (OrdenByUser.Count > 0)
                    {
                        //Agrega algun elemento a la lista "OrdenByuser" que no exista
                        AddNewField(Gen.GetFieldList(), OrdenByUser);
                        //Genera nuevamente cadena pero esta vez en base a orden del usuario.
                        ReplaceSection = Orden.GeneraCadenaOrden(OrdenByUser);
                        //Reemplazar en sql y luego agregar a richcontrol
                        //SeccionField = ReplaceSection;

                        ExtraeyReemplazaSeccion("--<field>", "--</field>", ReplaceSection, true);
                    }                   

                    SeccionRelation = ExtraeSeccion("--<relation>", "--</relation>");
                    SqlBase = richEditControl1.Text;
                    SeccionField = ExtraeSeccion("--<field>", "--</field>");                              

                    OrdenarSecciones(SeccionField, SeccionFilter, SeccionOrderBy, SeccionRelation);                                                            
                }
            }
            else
            {
                richEditControl1.Text = "";
                //Limpiamos el listado de filtros
                Filtro.FilterStack.Clear();
            }
        }

       

        /// <summary>
        /// Agrega un campo a la lista de orden si es que este no existe
        /// </summary>
        private void AddNewField(List<formula> pFiels, List<Orden> pOrderlist)
        {
            if (pFiels.Count > 0 && pOrderlist.Count > 0)
            {

                List<Orden> lisAux = new List<Orden>();
                Orden delete = new Orden();
                int LastPos = 0;
                LastPos = pOrderlist.Count;

                //Field esta contenido dentro de orderlist
                foreach (formula f in pFiels)
                {
                    if (pOrderlist.Count(x => x.Campo.Contains(f.desc)) == 0)
                    {
                        LastPos++;
                        lisAux.Add(new Orden() { Campo = f.desc, Alias = "", OrderBy = false, OrderType = "DESC", visible = true, Posicion = LastPos });
                    }                        
                }

                //Se deja de seleccionar un elemento del treelist (No aparece en el fieldList, pero si aparece en el orderlist)??
                //@ - Eliminar del orderlist  
                
                foreach (Orden y in pOrderlist)
                {
                    if (pFiels.Count(x => x.desc.Contains(y.Campo)) == 0)
                    {
                        //Eliminamos
                        delete = y;
                        break;
                    }
                }

                //remove 
                if(delete.Campo != "")
                    pOrderlist.Remove(delete);

                //Agregamos datos encontrados a orderlist
                if (lisAux.Count > 0)
                {
                    foreach (Orden item in lisAux)
                    {
                        pOrderlist.Add(item);
                    } 
                }                               
                
            }
        }

        /// <summary>
        /// Genera filtro en base listado de clase FILTRO
        /// </summary>
        private string GeneraFiltro(string pSql)
        {
            if (pSql.Length == 0) return "";

            string cad = "";
            StringBuilder buil = new StringBuilder();       
            if (Filtro.FilterStack.Count > 0)
            {
                foreach (var fil in Filtro.FilterStack)
                {
                    //SI ALGUNA CADENA YA EXISTE NO LA AGREGAMOS
                    cad = $"{fil.Nombre} {fil.Condicion} {fil.Valor} {fil.Union}";
                    buil.AppendLine(cad); 
                }
            }   

            return buil.ToString();
        }

        /// <summary>
        /// Limpiar el filtro que pueda existir en la cadena sql generada.
        /// </summary>
        private void CleanFilter(string pSql)
        {
            string cad = "";
            StringBuilder buil = new StringBuilder();
            if (Filtro.FilterStack.Count > 0)
            {
                foreach (var fil in Filtro.FilterStack)
                {
                    //SI ALGUNA CADENA YA EXISTE NO LA AGREGAMOS
                    cad = $"{fil.Nombre} {fil.Condicion} {fil.Valor} {fil.Union}";
                    if (pSql.Contains(cad))
                    {
                        //Obtener la posicion de la cadena.
                        //Inicio y Fin del tramo que desea eliminar.
                        RemoveWord(cad);                        
                    }
                }
            }
        }

        private void btnOrdenar_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (treeList1.Nodes.Count == 0)
            { XtraMessageBox.Show("Selecciona al menos una tabla", "Error",  MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            List<formula> Fields = new List<formula>();
            if (Gen != null)
            {
                Fields = Gen.GetFieldList();
                if (Fields.Count > 0)
                {
                    //LO PASAMOS AL FORMULARIO DE FILTROS A TRAVES DE SU CONSTRUCTOR.
                    frmOrdenBuilder fil = new frmOrdenBuilder(Fields, OrdenByUser);
                    fil.Open = this;
                    fil.StartPosition = FormStartPosition.CenterParent;
                    fil.ShowDialog();
                }
            }
 
        }

        private void btnListarConsultas_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (Reporte.Count() == 0)
            { XtraMessageBox.Show("No hay registros guardados", "Información", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            frmListadoSqlBuilder list = new frmListadoSqlBuilder();
            list.iGenOpen = this;
            list.StartPosition = FormStartPosition.CenterParent;
            list.ShowDialog();
        }

        private void btnGuardarConsulta_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (treeList1.Nodes.Count == 0)
            {
                XtraMessageBox.Show("Primero debes generar una consulta", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (richEditControl1.Text.Length == 0)
            {
                XtraMessageBox.Show("Primera debes generar una consulta", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (Reporte.Id != 0)
            {
                /*Si reporte no es nulo quiere decir que estamos trabajando en la edicion de un reporte*/
                /*No es un reporte nuevo*/
                DialogResult adv = XtraMessageBox.Show("Estás editando una consulta.\n¿Estás seguro de guardar los cambios?", "Edición", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (adv == DialogResult.Yes)
                {
                    //Update registro.
                    if (Reporte.ModReport(Reporte.Id, richEditControl1.Text, Reporte.Name))
                    {
                        XtraMessageBox.Show("Consulta actualizada correctamente", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        XtraMessageBox.Show("No se pudo actualizar consulta", "Información", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                }
            }
            else
            {
                //Mostramos el formulario para que puedan color el nombre al reporte y despues guardar
                DialogResult adv = XtraMessageBox.Show("¿Estás seguro que deseas guardar el reporte?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (adv == DialogResult.Yes)
                {
                    frmSaveReportBuilder save = new frmSaveReportBuilder(richEditControl1.Text);
                    save.StartPosition = FormStartPosition.CenterParent;
                    save.ShowDialog();
                }
            }           
        }

        private void btnNuevaConsulta_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            if (richEditControl1.Text.Length > 0)
            {
                DialogResult adv = XtraMessageBox.Show("¿Estás seguro de que quieres crear una nueva consulta?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (adv == DialogResult.No)
                    return;
            }

            richEditControl1.Text = "";
            SeccionField = "";
            SeccionFilter = "";
            SeccionOrderBy = "";
            SeccionRelation = "";

            Reporte.Clear();

            Filtro.FilterStack.Clear();
            OrdenByUser.Clear();

            treeList1.Nodes.Clear();
            
        }

        private void treeList1_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void treeList1_DragDrop_1(object sender, DragEventArgs e)
        {
            TreeList tree = sender as TreeList;

            Point np = new Point(e.X, e.Y);
            np = tree.PointToClient(np);

            int selectedIndex = imageListBoxControl1.IndexFromPoint(np);
            object item = imageListBoxControl1.Items[imageListBoxControl1.IndexFromPoint(p)];

            if (treeList1.Nodes.Count > 0)
            {
                if (treeList1.Nodes.Count(x => x.GetValue(0).ToString().ToLower().Equals(item.ToString().ToLower())) > 0)
                {
                    XtraMessageBox.Show("Ya agregaste la tabla " + item.ToString() + ".\nIntenta con otra.", "Tabla", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                    
                CreateNodes(treeList1, juego.Componentes, item.ToString().ToLower());
            }
            else
                CreateNodes(treeList1, juego.Componentes, item.ToString().ToLower());


        }

        private void cbTodos_CheckedChanged(object sender, EventArgs e)
        {
            if (cbTodos.Checked)
            {
                if (treeList1.Nodes.Count == 0)
                { XtraMessageBox.Show("Agrega al menos una tabla", "No hay tablas", MessageBoxButtons.OK, MessageBoxIcon.Stop); cbTodos.Checked = false; return; }

                if (treeList1.Nodes.Count > 0)
                {
                    treeList1.CheckAll();
                    ReloadSqlText();
                }
                   
            }
            else {
                treeList1.UncheckAll();
                ReloadSqlText();
            }
        }

        private void checkEdit1_CheckedChanged(object sender, EventArgs e)
        {
            CheckEdit c = sender as CheckEdit;
            if (c.Checked)
            {
                DialogResult ad = XtraMessageBox.Show("¿Estás seguro de querer modificar el código?\nSi no sabes lo que estás haciendo, cancela esta operación.", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (ad == DialogResult.Yes)
                {
                    richEditControl1.ReadOnly = false;
                    Edita = true;

                    //1- Actualizar los campos acorde al codigo realizado
                    //2- Si se eliminar un campos se debe reflejar en los nodos de las tablas que supuestamente están relacionadas

                }
                else
                {
                    c.Checked = false;
                    Edita = false;
                }
                           
            }
            else
            {
                richEditControl1.ReadOnly = true;
                Edita = false;
            }
            
        }

      

        private void richEditControl1_LocationChanged(object sender, EventArgs e)
        {
            int pos = richEditControl1.Document.CaretPosition.ToInt();
        }

        private void richEditControl1_KeyDown(object sender, KeyEventArgs e)
        {
            int pos = richEditControl1.Document.CaretPosition.ToInt();
            int PosFindTextStart = 0, PosFindTextEnd = 0;
            
            if (Edita)
            {
                //Consultamos por la posicion donde está el curso actualmente
                PosFindTextStart = GetPositionText("--<relation>", 1);
                PosFindTextEnd = GetPositionText("--</relation>", 2);

                //Está dentro de esta seccion
                if (pos >= PosFindTextStart && pos <= PosFindTextEnd)
                {
                    XtraMessageBox.Show("No puedes editar esta seccion", "No permitido", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    richEditControl1.Document.CaretPosition = richEditControl1.Document.Range.Start;
                   
                }

                //Consultamos por la posicion donde está el curso actualmente
                PosFindTextStart = GetPositionText("--<filter>", 1);
                PosFindTextEnd = GetPositionText("--</filter>", 2);

                //Está dentro de esta seccion
                if (pos >= PosFindTextStart && pos <= PosFindTextEnd)
                {
                    XtraMessageBox.Show("No puedes editar esta seccion", "No permitido", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    richEditControl1.Document.CaretPosition = richEditControl1.Document.Range.Start;
                   
                }

                //Consultamos por la posicion donde está el curso actualmente
                PosFindTextStart = GetPositionText("--<user>", 1);
                PosFindTextEnd = GetPositionText("--</user>", 2);

                //Está dentro de esta seccion
                if (pos >= PosFindTextStart && pos <= PosFindTextEnd)
                {
                    XtraMessageBox.Show("No puedes editar esta seccion", "No permitido", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    richEditControl1.Document.CaretPosition = richEditControl1.Document.Range.Start;
                    
                }

                //Consultamos por la posicion donde está el curso actualmente
                PosFindTextStart = GetPositionText("--<orderby>", 1);
                PosFindTextEnd = GetPositionText("--</orderby>", 2);

                //Está dentro de esta seccion
                if (pos >= PosFindTextStart && pos <= PosFindTextEnd)
                {
                    XtraMessageBox.Show("No puedes editar esta seccion", "No permitido", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    richEditControl1.Document.CaretPosition = richEditControl1.Document.Range.Start;
                    
                }

                //Si llega a este punto es porque está dentro de una zona valida
                //Está en la seccion de campos
                PosFindTextStart = GetPositionText("--<field>", 1);
                PosFindTextEnd = GetPositionText("--</field>", 2);

                if (pos >= PosFindTextStart && pos <= PosFindTextEnd)
                {
                    //Zona de creacion de campos nuevos
                    //Campos custom????
                }

            }

        }

        private void richEditControl1_Click(object sender, EventArgs e)
        {
            int pos = richEditControl1.Document.CaretPosition.ToInt();
            int PosFindTextStart = 0, PosFindTextEnd = 0;

            if (Edita)
            {
                //Consultamos por la posicion donde está el curso actualmente
                PosFindTextStart = GetPositionText("--<relation>", 1);
                PosFindTextEnd = GetPositionText("--</relation>", 2);

                //Está dentro de esta seccion
                if (pos >= PosFindTextStart && pos <= PosFindTextEnd)
                {
                    XtraMessageBox.Show("No puedes editar esta seccion", "No permitido", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    richEditControl1.Document.CaretPosition = richEditControl1.Document.Range.Start;

                }

                //Consultamos por la posicion donde está el curso actualmente
                PosFindTextStart = GetPositionText("--<filter>", 1);
                PosFindTextEnd = GetPositionText("--</filter>", 2);

                //Está dentro de esta seccion
                if (pos >= PosFindTextStart && pos <= PosFindTextEnd)
                {
                    XtraMessageBox.Show("No puedes editar esta seccion", "No permitido", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    richEditControl1.Document.CaretPosition = richEditControl1.Document.Range.Start;

                }

                //Consultamos por la posicion donde está el curso actualmente
                PosFindTextStart = GetPositionText("--<user>", 1);
                PosFindTextEnd = GetPositionText("--</user>", 2);

                //Está dentro de esta seccion
                if (pos >= PosFindTextStart && pos <= PosFindTextEnd)
                {
                    XtraMessageBox.Show("No puedes editar esta seccion", "No permitido", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    richEditControl1.Document.CaretPosition = richEditControl1.Document.Range.Start;

                }

                //Consultamos por la posicion donde está el curso actualmente
                PosFindTextStart = GetPositionText("--<orderby>", 1);
                PosFindTextEnd = GetPositionText("--</orderby>", 2);

                //Está dentro de esta seccion
                if (pos >= PosFindTextStart && pos <= PosFindTextEnd)
                {
                    XtraMessageBox.Show("No puedes editar esta seccion", "No permitido", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    richEditControl1.Document.CaretPosition = richEditControl1.Document.Range.Start;

                }


            }
        }

        private void btnCampoPersonalizado_Click(object sender, EventArgs e)
        {
            if (treeList1.Nodes.Count > 0)
            {
                frmCampoPersonalizadoBuilder campo = new frmCampoPersonalizadoBuilder();
                campo.StartPosition = FormStartPosition.CenterParent;
                campo.ShowDialog();
            }
            else
            {
                XtraMessageBox.Show("Debes seleccionar al menos una tabla", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        private void SetCustomFields()
        {
            List<TreeListNode> Nodos = new List<TreeListNode>();
            DocumentRange rango = richEditControl1.Document.Range;
            string d = richEditControl1.Document.GetText(rango);
            if (d.Length > 0)
            {
                string seccionFields = ExtraeSeccion("--<field>", "--</field>");
                if (seccionFields.Length > 0)
                {
                    //Quitamos select y otros
                    seccionFields = seccionFields.Replace("\n", "");
                    seccionFields = seccionFields.Replace("\r", "");
                    seccionFields = seccionFields.Replace("\"", "");
                    seccionFields = seccionFields.Replace("SELECT", "");

                    //Ahora podemos extraer cada campo del listado
                    Nodos = CheckSeleccion();
                    if (Nodos.Count > 0)
                    {
                        TreeListNode tree = Nodos.FindLast(x => x.Checked);
                        if (tree != null)
                        {
                            object[] NodeData = new object[] { "CustomColumn", "", "" };
                            tree.Nodes.Add(NodeData);
                        }
                    }
                }
            }
        }
    }
}