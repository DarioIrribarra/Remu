using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.UI.Sql;
using DevExpress.DataAccess.UI.Wizard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using DevExpress.Data.Controls.ExpressionEditor;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraEditors;

namespace Labour
{
    /// <summary>
    /// Clase para manipular query sql desde el editor devexpress
    /// </summary>
    class BuilderSql
    {
        /// <summary>
        /// Para guardar consulta generada en el editor.
        /// </summary>
        public string CustomSql { get; set; } = "";       

        /// <summary>
        /// Permite configuar y mostrar el editor de sql.
        /// </summary>
        /// <param name="pServer">Server para conexion</param>
        /// <param name="pBase">Base de datos de conexion</param>
        /// <param name="pUserName">Nombre de usuario base de datos.</param>
        /// <param name="pPassword">Password base de datos.</param>
        public void ShowSqlBuilder(string pServer, string pBase, string pUserName, string pPassword, string pFiltroUsuario)
        {
            try
            {
                StringBuilder SqlFinal = new StringBuilder();
                SelectQuery query = new SelectQuery();
                SelectQuery q = new SelectQuery();
                string relacionTabla = "";
                //Parametros de conexion para la base de datos.
                MsSqlConnectionParameters parameters = new MsSqlConnectionParameters(pServer, pBase, pUserName, pPassword, MsSqlAuthorizationType.SqlServer);

                SqlDataConnection data = new SqlDataConnection("name", parameters);
                SqlDataSource ds = new SqlDataSource(parameters);

                //SqlWizardSettings settings = new SqlWizardSettings();
                //settings.EnableCustomSql = true;
                //settings.QueryBuilderLight = false;
                //settings.DisableNewConnections = true;
                //settings.QueryBuilderDiagramView = false;                

                ExpressionEditorContext x = new ExpressionEditorContext();                                      

                QueryBuilderEditQueryContext context = new QueryBuilderEditQueryContext();
                context.QueryBuilderLight = true;
                context.ExpressionEditorContext = x;
                
                //EditQueryContext context = new EditQueryContext();
                //context.DBSchemaProviderEx = new DBSchemaProviderEx();
                //context.Options = settings.ToSqlWizardOptions();
               
                query.Name = "mainQuery";
                ds.Queries.Add(query);

                query.FilterString = "anomes=201901";

                DevExpress.Data.Filtering.CriteriaOperator op = query.FilterString;
                
                string da = DevExpress.Data.Filtering.CriteriaToWhereClauseHelper.GetMsSqlWhere(op);
                //query.FilterString;
                //ds.Connection.Open();
                //string QueryFromSource = (ds.Queries["mainQuery"] as SelectQuery).GetSql(ds.Connection.GetDBSchema());                     

                //if (pFiltroUsuario.Length > 0)
                //{
                //    q = (SelectQuery)ds.Queries[0];
                //    relacionTabla = GetRelation(q.GetSql(ds.DBSchema));
                //}

                //Con este codigo lanzamos el generador de consultas.
                SqlDataSourceUIHelper.EditQueryWithQueryBuilder(query, context);              
               
                ds.Fill();                    

                q = new SelectQuery();
                q = (SelectQuery)ds.Queries[0];
                
                //Obtenemos el sql final
                CustomSql = q.GetSql(ds.DBSchema);
                string xx = q.FilterString;
                //AGREGAR A CUSTOM QUERY FILTRO QUE TENGA USUARIO
                relacionTabla = GetRelation(CustomSql);
                if (CustomSql.ToLower().Contains("where"))
                {
                    SqlFinal.Append(CustomSql);
                    SqlFinal.Append($" AND {relacionTabla} IN (SELECT contrato FROM trabajador {pFiltroUsuario})");

                    CustomSql = SqlFinal.ToString();
                }
                else
                {
                    SqlFinal.Append(CustomSql);
                    SqlFinal.Append("WHERE ");
                    SqlFinal.Append($" {relacionTabla} IN (SELECT contrato FROM trabajador {pFiltroUsuario})");

                    CustomSql = SqlFinal.ToString();
                }
            }
            catch (Exception ex)
            {
                //ERROR...
            }
        }

        /// <summary>
        /// Genera datasource en base a CustomSql
        /// </summary>
        public DataTable GetDataSource(MemoEdit pMemo)
        {
            SqlCommand cmd;
            SqlConnection cn;
            SqlDataAdapter ad;
            DataSet ds = new DataSet();
            DataTable Tabla = new DataTable();
            

            if (CustomSql.Length > 0)
            {
                try
                {
                    cn = fnSistema.OpenConnection();
                    if (cn != null)
                    {
                        using (cn)
                        {
                            using (cmd = new SqlCommand(CustomSql, cn))
                            {
                                ad = new SqlDataAdapter();
                                ad.SelectCommand = cmd;
                                ad.Fill(ds, "data");

                                if (ds.Tables[0].Rows.Count > 0)
                                {
                                    Tabla = ds.Tables[0];
                                    pMemo.Text = "Consulta realizada correctamente.";
                                }                                    
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    //ERROR...
                    pMemo.Text = ex.Message;
                }
            }

            return Tabla;
        }

        /// <summary>
        /// Genera excel en base al datasource seleccionado
        /// </summary>
        //public bool GetExcel(string pPathFile)
        //{
        //    DataTable pTabla = new DataTable();
        //    pTabla = GetDataSource();
        //    bool creado = false;
        //    try
        //    {
        //        if (pTabla.Rows.Count > 0 && pPathFile.Length > 0)
        //        {
        //            if (FileExcel.CrearArchivoExcel(pTabla, pPathFile))
        //            {
        //                creado = true;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //ERROR...
        //    }

        //    return creado;
        //}

        /// <summary>
        /// Genera consulta de acuerdo a tablas seleccionadas, solo para filtro.
        /// </summary>
        /// <param name="pQuery"></param>
        /// <returns></returns>
        private string GetTableSelectQuery(SelectQuery pQuery)
        {
            StringBuilder bl = new StringBuilder();
            if (pQuery.Tables.Count > 0)
            {
                foreach (var item in pQuery.Tables)
                {
                    //BUSCAMOS TABLAS
                    if (FieldExistTable(item.Name, "contrato"))
                    {
                        bl.Append(item.Name);
                        bl.Append(".");
                        bl.Append("contrato");

                        break;
                    }
                }
            }

            return bl.ToString();
        }

        /// <summary>
        /// Verifica si existe el campo contrato en la tabla especificada.
        /// </summary>
        /// <param name="pTableName"></param>
        /// <returns></returns>
        private bool FieldExistTable(string pTableName, string pField)
        {
            string sql = " SELECT count(*)  FROM INFORMATION_SCHEMA.COLUMNS " +
                         $"WHERE TABLE_NAME = {pTableName} " +
                         $"AND COLUMN_NAME = {pField} ";
            SqlCommand cmd;
            SqlConnection cn;
            bool existe = false;

            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            object data = cmd.ExecuteScalar();
                            if (data != null)
                            {
                                if (Convert.ToInt32(data) > 0)
                                    existe = true;
                            }
                            cmd.Dispose();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                //ERROR...
            }

            return existe;
        }

        /// <summary>
        /// Genera relacion para filtro.
        /// </summary>
        /// <param name="pCustomSql"></param>
        /// <returns></returns>
        private string GetRelation(string pCustomSql)
        {
            StringBuilder buil = new StringBuilder();
            if (pCustomSql.Length > 0)
            {
                if (pCustomSql.ToLower().Contains("trabajador"))
                {
                    buil.Append("trabajador");
                    buil.Append(".");
                    buil.Append("contrato");
                }
                else if (pCustomSql.ToLower().Contains("itemtrabajador"))
                {
                    buil.Append("itemtrabajador");
                    buil.Append(".");
                    buil.Append("contrato");
                }
                else if (pCustomSql.ToLower().Contains("ausentismo"))
                {
                    buil.Append("ausentismo");
                    buil.Append(".");
                    buil.Append("contrato");
                }
                else if (pCustomSql.ToLower().Contains("vacacion"))
                {
                    buil.Append("vacaciones");
                    buil.Append(".");
                    buil.Append("contrato");
                }
                else if (pCustomSql.ToLower().Contains("vacaciondetalle"))
                {
                    buil.Append("vacaciondetalle");
                    buil.Append(".");
                    buil.Append("contrato");
                }
                else if (pCustomSql.ToLower().Contains("calculomensual"))
                {
                    buil.Append("calculomensual");
                    buil.Append(".");
                    buil.Append("contrato");
                }
                else if (pCustomSql.ToLower().Contains("cargafamiliar"))
                {
                    buil.Append("cargafamiliar");
                    buil.Append(".");
                    buil.Append("contrato");
                }
                else if (pCustomSql.ToLower().Contains("datoscalculo"))
                {
                    buil.Append("datoscalculo");
                    buil.Append(".");
                    buil.Append("contrato");
                }
                else if (pCustomSql.ToLower().Contains("liquidacionhistorico"))
                {
                    buil.Append("liquidacionhistorico");
                    buil.Append(".");
                    buil.Append("contrato");
                }
                else
                {
                    buil.Append("");
                }

            }

            return buil.ToString();
        }
    }

    /// <summary>
    /// Representa a todos los elementos de una base de datos
    /// </summary>
    class JuegoDato
    {
        /// <summary>
        /// Representa a todos los componentes que tiene un juego de datos.
        /// </summary>
        public List<Componente> Componentes { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public JuegoDato()
        {
            this.Componentes = GetElementos(GetTableName());
        }

        /// <summary>
        /// Obtiene cada uno de los elementos correspondientes a un juego de datos (Tablas que componen la base de datos)
        /// </summary>
        private List<string> GetTableName()
        {
            //Nombre de las tablas
            string sql = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' " +
                         "AND (table_name = 'afp' " +
                         "OR table_name = 'area' OR table_name = 'ausentismo' OR table_name = 'banco' " +
                         "OR table_name = 'calculomensual' OR table_name = 'cargo' OR table_name='ccosto' " +
                         "OR table_name = 'ciudad' OR table_name = 'clase' OR table_name = 'escolaridad' " +
                         "OR table_name = 'ecivil' OR table_name = 'formpago' OR table_name = 'formula'" +
                         "OR table_name = 'horario' OR table_name = 'isapre' OR table_name = 'item' " +
                         "OR table_name = 'itemclase' OR table_name = 'itemtrabajador' " +
                         "OR table_name = 'liquidacionhistorico' OR table_name = 'motivo' " +
                         "OR table_name = 'nacion' OR table_name = 'regimen' OR table_name = 'sucursal' " +
                         "OR table_name = 'tipocuenta' OR table_name = 'trabajador' " +
                         "OR table_name = 'vacacion' OR table_name = 'vacaciondetalle' " +
                         "OR table_name = 'valoresmes' OR table_name = 'sindicato'" +
                         "OR table_name = 'cajaprevision') " +
                         "order by table_name";

            SqlCommand cmd;
            SqlConnection cn;
            SqlDataReader rd;
            List<string> Tables = new List<string>();

            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {                        
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            rd = cmd.ExecuteReader();
                            if (rd.HasRows)
                            {
                                while (rd.Read())
                                {
                                    Tables.Add((string)rd["TABLE_NAME"]);
                                }
                            }
                        }
                        cmd.Dispose();
                        rd.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                //ERROR...
            }

            return Tables;
        }    

        /// <summary>
        /// Obtiene cada una de las columnas asociadas a una tabla.
        /// </summary>
        private List<Columna> GetTableColumns(string pTableName)
        {

            string sqlCol = "SELECT c.name 'Column_Name', ISNULL(i.is_primary_key, 0) 'Primary_Key', " +
                            "c.object_id FROM sys.columns c INNER JOIN sys.types t ON c.user_type_id = t.user_type_id " +
                            "LEFT OUTER JOIN " +
                            "sys.index_columns ic ON ic.object_id = c.object_id AND ic.column_id = c.column_id " +
                            "LEFT OUTER JOIN " +
                            "sys.indexes i ON ic.object_id = i.object_id AND ic.index_id = i.index_id " +
                            "WHERE " +
                            "c.object_id = OBJECT_ID(@pTableName) ";

            SqlCommand cmd;
            SqlConnection cn;
            SqlDataReader rd;

            List<Columna> Columnas = new List<Columna>();

            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sqlCol, cn))
                        {
                            //PARAMETROS    
                            cmd.Parameters.Add(new SqlParameter("@pTableName", pTableName));

                            rd = cmd.ExecuteReader();
                            if (rd.HasRows)
                            {
                                while (rd.Read())
                                {
                                    
                                    Columnas.Add(new Columna() { Nombre = (string)rd["Column_Name"],
                                        PrimaryKey = Convert.ToInt32(rd["Primary_Key"]) == 1?true:false });
                                }
                            }
                        }
                        cmd.Dispose();
                        rd.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                //ERROR...
            }

            return Columnas;

        }

        /// <summary>
        /// Retorna un listado con todos los elementos de un juego de datos.
        /// </summary>
        /// <param name="pTables">Listado de todas las tablas de la base de datos</param>
        /// <returns></returns>
        private List<Componente> GetElementos(List<string> pTables)
        {
            List<Componente> Listado = new List<Componente>();
            List<Columna> Columns = new List<Columna>();
            List<Relacion> Relaciones = new List<Relacion>();
            Relacion rel = new Relacion();

            try
            {
                if (pTables.Count > 0)
                {
                    foreach (string Table in pTables)
                    {
                        //Obtenemos columnas asociadas a la tabla
                        Columns = GetTableColumns(Table);

                        //Alguno de los elementos de la lista es foranea???
                        Relaciones = rel.GetRelaciones(Table);

                        //CAMBIAMOS VALOR DE FORANEA A TRUE, SI EXISTE EN LISTADO
                        foreach (Relacion x in Relaciones)
                        {
                            if (Columns.Find(p => p.Nombre.ToLower().Equals(x.ColumnaOrigen.ToLower())) != null)
                            {
                                //Si cumple la condicion cambiamos las propiedades de la columna encontrada
                                Columns.Find(p => p.Nombre.ToLower().Equals(x.ColumnaOrigen.ToLower())).ForeignKey = true;
                                Columns.Find(p => p.Nombre.ToLower().Equals(x.ColumnaOrigen.ToLower())).TablaForanea = x.TablaRef;
                                Columns.Find(p => p.Nombre.ToLower().Equals(x.ColumnaOrigen.ToLower())).TablaOrigen = x.TablaOrigen;
                                Columns.Find(p => p.Nombre.ToLower().Equals(x.ColumnaOrigen.ToLower())).CampoRef = x.ColumnaRef;

                            }
                        }

                        //Agregamos Tabla y columnas
                        Listado.Add(new Componente() { Nombre = Table, Columnas = Columns });
                    }
                }
            }
            catch (Exception ex)
            {
              //ERROR...
            }

            return Listado;
        }
    }

    /// <summary>
    /// Representa a un elemento de un juego de datos
    /// </summary>
    class Componente
    {
        /// <summary>
        /// Nombre del componente.
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Cantidad de elementos (Columnas) que tiene el componente
        /// </summary>
        public List<Columna> Columnas { get; set; }

    }

    /// <summary>
    /// Representa a una columna de un componente
    /// </summary>
    class Columna
    {
        /// <summary>
        /// Nombre de la columna
        /// </summary>
        public string Nombre { get; set; }
        /// <summary>
        /// Indica si es clave primaria
        /// </summary>
        public bool PrimaryKey { get; set; }
        /// <summary>
        /// Indica si es clave foranea
        /// </summary>
        public bool ForeignKey { get; set; }
        /// <summary>
        /// Table de la cual viene (si es foranea)
        /// </summary>
        public string TablaForanea { get; set; }
        /// <summary>
        /// Tabla de origen de columna.
        /// </summary>

        public string TablaOrigen { get; set; }

        /// <summary>
        /// Representa el campo de la tabla a la cual hace referencia una columna.
        /// </summary>
        public string CampoRef { get; set; }
    }

    /// <summary>
    /// Clase que nos permite genera codigo sql dinamico
    /// </summary>
    class Generador
    {
        /// <summary>
        /// Representa un listado de Nodos Raiz junto con sus nodos hijos correspondientes.
        /// </summary>
        private List<TreeListNode> DataSource { get; set; }

        public List<Componente> Componentes { get; set; }

        /// <summary>
        /// Solo para guardar todos las columnas que participan en la consulta sql.
        /// </summary>
        private List<formula> FieldsList = new List<formula>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Data">Listado de nodos seleccionados</param>
        /// <param name="pComp">Listado de componentes que tiene el juego de datos</param>
        public Generador(List<TreeListNode> Data, List<Componente> pComp)
        {
            this.DataSource = Data;
            this.Componentes = pComp;
        }

        /// <summary>
        /// Indica si existen o no relaciones entre tablas
        /// </summary>
        /// <returns>Retorna true si no encuentra relaciones</returns>
        public bool NoHayRelacionTablas()
        {
            bool ConRel = false;
            bool Existen = false;
            List<Relacion> ListadoTodasRelaciones = new List<Relacion>();

            if (this.DataSource != null && this.Componentes.Count > 0)
            {
                //Hay mas de una tabla seleccionada
                //Verificamos que exista al menos alguna relacion entre tablas
                if (this.DataSource.Count > 1)
                {
                    /*Generamos from para query */
                    AddRelations(ConRel, this.DataSource, ListadoTodasRelaciones);

                    /*Encontró relaciones??*/
                    if (ListadoTodasRelaciones.Count == 0)
                        Existen = true;                    

                }
            }

            return Existen;
        }

        /// <summary>
        /// Genera una cadena en base a todos los elementos del listado de componentes.
        /// </summary>
        public string GetSql()
        {
            StringBuilder cadena = new StringBuilder();
            string data = "";
            try
            {
                if (DataSource != null)
                {
                    if (DataSource.Count > 0)
                    {
                        data = CreateManyQuery(DataSource);
                    
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
        /// Genera una cadena para sql.
        /// </summary>
        /// <param name="pTableName"></param>
        /// <param name="ChildsNodes"></param>
        /// <returns></returns>
        public string CreateQuery(string pTableName, TreeListNodes ChildsNodes)
        {
            StringBuilder buil = new StringBuilder();
            string pk = "", fk = "";

            Columna c = new Columna();

            if (ChildsNodes.Count > 0)
            {
                //Header                
                buil.AppendLine("SELECT ");
                for (int i = 0; i < ChildsNodes.Count; i++)
                {
                    //Primary Key
                    pk = ChildsNodes[i].GetValue(1).ToString();
                    fk = ChildsNodes[i].GetValue(2).ToString();

                    if (fk == "*")
                    {
                        //El nodo es clave foranea.
                        //Si es fk extraemos del componente del juego la columnas correspondiente
                        //c = (Componentes.Find(x => x.Nombre.ToLower().Equals(pTableName))).Columnas.Find(y => y.Nombre.ToLower().Equals(ChildsNodes[i].GetValue(0).ToString()));
                    }
                    
                    buil.AppendLine($"{pTableName}.{ChildsNodes[i].GetValue(0).ToString()},");
                }

                buil.AppendLine($"FROM {pTableName}");
            }

            return buil.ToString();
        }

        /// <summary>
        /// Genera consulta sql.
        /// </summary>
        /// <param name="pNodosRaiz">Listado de nodos seleccionados</param>
        /// <returns></returns>
        public string CreateManyQuery(List<TreeListNode> pNodosRaiz)
        {
            StringBuilder buil = new StringBuilder();
            string pk = "", fk = "";

            Columna c = new Columna();
            List<Columna> columnasForaneas = new List<Columna>();
            string sub = "", cadena = "";

            buil.AppendLine("--<field>");
            buil.AppendLine("SELECT ");
            try
            {
                if (pNodosRaiz.Count > 0)
                {
                    //ITERAMOS TODOS LOS DATOS
                    foreach (var x in pNodosRaiz)
                    {
                        string rtable = x.GetValue(0).ToString();
                        //Extrae todos los nodos hijos que sean claves foraneas (por cada nodo raíz).
                        IEnumerable<TreeListNode> en = x.Nodes.Where(y => y.GetValue(2).ToString().Equals("*") && y.Checked == true);
                        
                        //LAS CLAVES PRIMARIAS (NODOS HIJOS)
                        //IEnumerable<TreeListNode> pks = x.Nodes.Where(y => y.Checked && y.GetValue(1).ToString().Equals("*"));
                        //int coun = pks.Count();                        

                        //RESTO DE HIJOS
                        IEnumerable<TreeListNode> chNormales = x.Nodes.Where(y => y.GetValue(2).ToString().Equals("") && y.Checked);

                        //DE QUE TABLA ES FORANEA???
                        foreach (TreeListNode ch in en)
                        {
                            //Trae el componente correspondiente a la columna (trae los datos de la columna)
                            string col = ch.GetValue(0).ToString();
                            c = (Componentes.Find(p => p.Nombre.ToLower().Equals(x.GetValue(0).ToString().ToLower()))).Columnas.Find(y => y.Nombre.ToLower().Equals(ch.GetValue(0).ToString().ToLower()));
                            if (c != null)
                            {
                                string TablaForanea = c.TablaForanea;
                                columnasForaneas.Add(c);
                            }

                            cadena = $"{x.GetValue(0)}.{ch.GetValue(0).ToString()},";
                            //Guardamos el campo
                            FieldsList.Add(new formula() { desc= $"{x.GetValue(0)}.{ch.GetValue(0).ToString()}" , key = $"{x.GetValue(0)}.{ch.GetValue(0).ToString()}" });

                            //AGREGAMOS CADA ELEMENTO AL STRINGBUILDER
                            buil.AppendLine(cadena);
                        }

                        //ELEMENTOS QUE NO SON FORANEAS (COLUMNAS RESTANTES SELECCIONADAS)
                        foreach (TreeListNode chn in chNormales)
                        {
                            cadena = $"{x.GetValue(0)}.{chn.GetValue(0).ToString()},";
                            //Guardamos el campo
                            FieldsList.Add(new formula() { desc= $"{x.GetValue(0)}.{chn.GetValue(0).ToString()}" , key= $"{x.GetValue(0)}.{chn.GetValue(0).ToString()}" });
                            buil.AppendLine(cadena);
                        }
                    }

                    sub = buil.ToString().Substring(0, buil.Length - 3);
                    buil.Clear();
                    buil.AppendLine(sub);
                    buil.AppendLine("--</field>");

                    buil.AppendLine("--<relation>");
                    buil.AppendLine("FROM");
                    buil.Append(GeneraFrom(columnasForaneas, pNodosRaiz));
                    buil.AppendLine("--</relation>");
                }
            }
            catch (Exception ex)
            {
              //ERROR...
            }

            return buil.ToString();
        }

        /// <summary>
        /// Genera toda la seccion del from de una consulta sql
        /// </summary>
        /// <param name="Foraneas"></param>
        /// <param name="pRootNodes"></param>
        /// <returns></returns>
        private string GeneraFrom(List<Columna> Foraneas, List<TreeListNode> pRootNodes)
        {
            StringBuilder buil = new StringBuilder();
            string tori = "", tref = "";
            int c = 0, i = 0;
            bool ConRel = false;

            Relacion rel = new Relacion();
            List<Relacion> relaciones = new List<Relacion>();

            /*Este listado nos sirve para ir guardando todas las relaciones encontradas
             * en el caso de que no se seleccionen campos con claves foraneas
             */
            List<Relacion> ListadoTodasRelaciones = new List<Relacion>();

            //ROOT NODES TIENE SOLO UNA TABLA??
            if (pRootNodes.Count >0)
            {
                //SOLO CALCULAMOS FROM CON FORANEAS PARA EL CASO DE QUE HAYA MAS DE UNA TABLA INVOLUCRADA
                if (pRootNodes.Count > 1)
                {
                    /*Generamos from para query */
                    AddRelations(ConRel, pRootNodes, ListadoTodasRelaciones);

                    /*Encontró relaciones*/
                    if (ListadoTodasRelaciones.Count > 0)
                    {
                        string data = GetQueryFrom(ListadoTodasRelaciones);
                        buil.AppendLine(data);
                    }
                    


                    //if (Foraneas.Count > 0)
                    //{
                    //    foreach (Columna cc in Foraneas)
                    //    {
                    //        //TABLA DE ORIGEN DE LA COLUMNA?
                    //        string TablaOrigen = cc.TablaOrigen;
                    //        string ColumnaOrigen = cc.Nombre;
                    //        string ColumnaRefer = cc.CampoRef;
                    //        string TablaRef = cc.TablaForanea;                 

                    //        if (pRootNodes.Find(x => x.GetValue(0).ToString().ToLower().Equals(TablaRef.ToLower())) != null)
                    //        {
                    //            //SOLO AGREGAMOS UNA VEZ HASTA QUE LAS TABLAS CAMBIEN
                    //            if (c == 0)
                    //            {
                    //                if (tori != TablaOrigen || tref != TablaRef)
                    //                {
                    //                    buil.AppendLine($"{TablaOrigen} INNER JOIN {TablaRef} ON ");
                    //                    ConRel = true;
                    //                }                                     
                    //            }
                    //            else
                    //            {
                    //                if (tori != TablaOrigen || tref != TablaRef)
                    //                {
                    //                    if (buil.ToString().Contains(TablaRef) == false)
                    //                    {
                    //                        buil.AppendLine($"INNER JOIN {TablaRef} ON ");
                    //                        ConRel = true;
                    //                    }
                    //                    else
                    //                    {
                    //                        buil.AppendLine($"INNER JOIN {TablaOrigen} ON ");
                    //                        ConRel = true;
                    //                    }                                            
                    //                }
                                     
                    //            }

                    //            //ENTREGA EL ELEMENTO SIGUIENTE DE LA LISTA.
                    //            if (Foraneas.IndexOf(cc) + 1 < Foraneas.Count)
                    //            {
                    //                Columna col = Foraneas[Foraneas.IndexOf(cc) + 1];
                    //                if (TablaOrigen == col.TablaOrigen && col.TablaForanea == TablaRef)
                    //                {
                    //                    buil.AppendLine($"{TablaOrigen}.{ColumnaOrigen}={TablaRef}.{ColumnaRefer} AND ");
                    //                    ConRel = true;
                    //                }
                    //                else
                    //                {
                    //                    buil.AppendLine($"{TablaOrigen}.{ColumnaOrigen}={TablaRef}.{ColumnaRefer} ");
                    //                    ConRel = true;
                    //                }                                        
                    //            }
                    //            else
                    //            {
                    //                buil.AppendLine($"{TablaOrigen}.{ColumnaOrigen}={TablaRef}.{ColumnaRefer} ");
                    //                ConRel = true;
                    //            }

                    //            tori = TablaOrigen;
                    //            tref = TablaRef;

                    //            c++;
                    //        }

                    //        //SOLO PARA SABER LA POSICION DEL LISTADO.
                    //        i++;
                    //    }

                    //    /*Conrel?? false (No encontró ninguna relacion o no aplica)*/
                    //    if (ConRel == false)
                    //    {
                    //        AddRelations(ConRel, pRootNodes, ListadoTodasRelaciones);

                    //        /*Encontró relaciones*/
                    //        if (ListadoTodasRelaciones.Count > 0)
                    //        {
                    //            GetQueryFrom(ListadoTodasRelaciones);
                    //        }
                    //    }
                    //}                 
                    
                }
                else
                {
                    string pTableName = "";
                    pTableName = pRootNodes[0].GetValue(0).ToString();

                    buil.AppendLine(pTableName);
                }
            }
          

            return buil.ToString();
        }

        /// <summary>
        /// Agrega relaciones adicionales si es que no hay campos seleccionados que se relaciones, 
        /// pero que de igual forma las tablas tengan alguna relacion entre ellas.
        /// </summary>
        /// <param name="ExistenRelaciones"></param>
        /// <param name="pRootNodes"></param>
        private void AddRelations(bool ExistenRelaciones, List<TreeListNode> pRootNodes, List<Relacion> pSaveList)
        {
            List<Relacion> relaciones = new List<Relacion>();
            List<Relacion> extraeList = new List<Relacion>();
            Relacion rel = new Relacion();
            StringBuilder buil = new StringBuilder();
       
            if (ExistenRelaciones == false)
            {
                /*No se encontraron campos foraneos seleccionados y que se relacionen entre los nodos raices*/
                //Recorrer nodos raices y ver si hay relaciones entre las tablas          
                foreach (TreeListNode x in pRootNodes)
                {
                    string TableName = "";
                    //Obtenemos nombre de la tabla
                    TableName = x.GetValue(0).ToString();

                    /*Todas las relaciones que tiene la tabla */
                    relaciones = rel.GetRelaciones(TableName);

                    /*Encuentra todas las relaciones que hay entre tablas*/
                    extraeList = ExisteRelacion(pRootNodes, relaciones, TableName);

                    if (extraeList.Count > 0)
                    {
                        foreach (Relacion r in extraeList)
                        {
                            Relacion rr = new Relacion();
                            //Solo para saber si ya existe elemento en listado pSaveList, sino existe la agregamos (codigo de abajo)
                            rr = pSaveList.Find(p => p.ColumnaOrigen.Equals(r.ColumnaOrigen) && p.ColumnaRef.Equals(r.ColumnaRef) && p.TablaOrigen.Equals(r.TablaOrigen) && p.TablaRef.Equals(r.TablaRef));

                            if(rr == null)
                                pSaveList.Add(r);
                        }
                    }
                }
            }
           
        }

        /// <summary>
        /// Verifica si hay relacion entre tablas, de acuerdo a un listado de relaciones.
        /// </summary>
        /// <param name="pRootNodes"></param>
        /// <param name="pRelaciones"></param>
        private List<Relacion> ExisteRelacion(List<TreeListNode> pRootNodes, List<Relacion> pRelaciones, string pTableObserva)
        {
            List<Relacion> relaciones = new List<Relacion>();
            Relacion rel = new Relacion();
            Relacion elemento = new Relacion();
            List<Relacion> ExtraeList = new List<Relacion>();
            List<Relacion> Totallist = new List<Relacion>();
            
            List<TreeListNode> SinObservadoList = new List<TreeListNode>();           

            /*LINQ SIN TABLA OBSERVA DENTRO DEL LISTADO*/
            if (pRootNodes.Count > 0 && pRelaciones.Count > 0 )
            {
                /*Nodos sin la tabla observada*/
                SinObservadoList = (from el in pRootNodes
                                    where el.GetValue(0).ToString() != pTableObserva
                                    select el).ToList();
                if (SinObservadoList.Count > 0)
                {

                    foreach (TreeListNode node in SinObservadoList)
                    {
                        string TableName = node.GetValue(0).ToString();

                        relaciones = rel.GetRelaciones(TableName);

                        /*Existe relacion entre tablas*/
                        if (relaciones.Count(x => x.TablaRef.Equals(pTableObserva)) > 0)
                        {
                            //elemento = relaciones.Find(y => y.TablaRef.Equals(pTableObserva));
                            /*Extrae todos los elementos de la la lista que coincidadn las tablas*/
                            ExtraeList = (from n in relaciones
                                          where n.TablaRef.Equals(pTableObserva)
                                          select n).ToList();

                            //Agregamos al total
                            if (ExtraeList.Count > 0)
                            {
                                foreach (var item in ExtraeList)
                                {
                                    Totallist.Add(item);
                                }
                            }
                        }
                        else
                        {
                            //LA TABLA DE ORIGEN CONTIENE DENTRO DE LAS RELACIONES A TABLENAME
                            if (pRelaciones.Count(x => x.TablaRef.Equals(TableName)) > 0)
                            {
                                Relacion r = new Relacion();
                                r = pRelaciones.Find(y => y.TablaRef.Equals(TableName));                                
                                
                                
                                Totallist.Add(r);
                            }
                        }     
                        
                    }
                  
                }
            }

            return Totallist;
        }

        /// <summary>
        /// Genera cadena final para query bulder (Seccion FROM)
        /// </summary>
        /// <param name="pRelaciones"></param>
        private string GetQueryFrom(List<Relacion> pRelaciones)
        {
            StringBuilder buil = new StringBuilder();
            string ChangeTableRef = "", TablaOrigenjoin = "", TablaRefjoin = "";            
            if (pRelaciones.Count > 0)
            {
                /*Ordenar listado por nombre de tabla*/
                pRelaciones = (from x in pRelaciones
                              orderby x.TablaRef
                              select x).ToList();

                try
                {
                    if (pRelaciones.Count > 0)
                    {
                        buil.AppendLine(pRelaciones[0].TablaOrigen + " INNER JOIN " + pRelaciones[0].TablaRef + " ON ");
                        TablaOrigenjoin = pRelaciones[0].TablaOrigen;
                        TablaRefjoin = pRelaciones[0].TablaRef;
                        ChangeTableRef = pRelaciones[0].TablaOrigen;

                        for (int i = 0; i < pRelaciones.Count; i++)
                        {
                            buil.AppendLine($"{pRelaciones[i].TablaOrigen}.{pRelaciones[i].ColumnaOrigen} = {pRelaciones[i].TablaRef}.{pRelaciones[i].ColumnaRef}");
                            //Agrega and??
                            if (i < pRelaciones.Count - 1)
                            {                                

                                //next !=
                                if (pRelaciones[i + 1].TablaRef == pRelaciones[i].TablaRef)
                                {
                                    /*Referencia a la misma tabla pero la tabla de origen que hace referencia cambia*/
                                    if (pRelaciones[i+1].TablaOrigen == pRelaciones[i].TablaOrigen)
                                    {
                                        buil.AppendLine(" AND ");
                                    }
                                    else
                                    {
                                        /*Referencia a la misma tabla pero con otra tabla de origen*/
                                        buil.AppendLine(" INNER JOIN ");
                                        buil.AppendLine(pRelaciones[i + 1].TablaOrigen + " ON ");

                                        TablaOrigenjoin = pRelaciones[i + 1].TablaOrigen;
                                        TablaRefjoin = pRelaciones[i + 1].TablaRef; 
                                    }
                                }
                                else
                                {
                                    /*Cambia la tabla de referencia*/
                                    buil.AppendLine(" INNER JOIN ");
                                    if(buil.ToString().Contains(pRelaciones[i+1].TablaOrigen))
                                        buil.AppendLine(pRelaciones[i+1].TablaRef + " ON ");
                                    else
                                        buil.AppendLine(pRelaciones[i + 1].TablaOrigen + " ON ");

                                    TablaOrigenjoin = pRelaciones[i + 1].TablaOrigen;
                                    TablaRefjoin = pRelaciones[i + 1].TablaRef;

                                }
                            }
                        }


                    }
                }
                catch (Exception ex)
                {
                   //ERROR

                }
             
            }

            return buil.ToString();
        }

        /// <summary>
        /// Retorna una lista con todos los campos que participan dentro de la consulta sql.
        /// </summary>
        /// <returns></returns>
        public List<formula> GetFieldList()
        {
            if (this.FieldsList.Count > 0)
            {
                return this.FieldsList;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Permite setear listado
        /// </summary>
        /// <param name="pList"></param>
        public void SetFieldList(List<formula> pList)
        {
            this.FieldsList = pList;
        }
        
       
    }

    /// <summary>
    /// Representa una condicion en una consulta sql
    /// </summary>
    class Filtro
    {
        /// <summary>
        /// Refiere a campo de tabla.
        /// <para>Puede tener el formato TableName.Field</para>
        /// </summary>
        public string Nombre { get; set; }
        /// <summary>
        /// Condicion usada en el filtro
        /// <para>Puede ser Like, >, =, etc </para>
        /// </summary>
        public string Condicion { get; set; }
        /// <summary>
        /// Valor que tendrá la condicion, ingresado por el usuario que crea el filtro.
        /// </summary>
        public string Valor { get; set; }
        /// <summary>
        /// Elemento que se usará para unir una o mas condiciones, puede ser OR o AND.
        /// </summary>
        public string Union { get; set; }

        /// <summary>
        /// Permite ir guardando en memoria uno o varios filtros.
        /// </summary>
        public static List<Filtro> FilterStack = new List<Filtro>();

        /// <summary>
        /// Permite agrega un nuevo filtro a la lista.
        /// </summary>
        public static void AddFilter(Filtro pFilter)
        {
            FilterStack.Add(pFilter);
        }

        /// <summary>
        /// Permite obtener un filtro desde listado en memoria.
        /// </summary>
        public void GetFilter(string pFilterName)
        {
            Filtro f = new Filtro();
            try
            {
                if (FilterStack.Count > 0)
                {
                    //Si no encuentra nada retorna null.
                    f = FilterStack.Find(x => x.Nombre.ToLower().Equals(pFilterName));
                    
                }
            }
            catch (Exception ex)
            {
              //ERROR...
            }
        }

        /// <summary>
        /// Encuentra un filtro a través de su Id
        /// </summary>
        /// <param name="pIndex"></param>
        /// <returns></returns>
        public static Filtro GetFilterFromId(int pIndex)
        {
            Filtro f = new Filtro();
            try
            {
                if (FilterStack.Count > 0)
                {
                    for (int i = 0; i < FilterStack.Count; i++)
                    {
                        if (pIndex == i)
                        {
                            f = FilterStack[i];
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
              //ERROR...
            }

            return f;
        }
    }

    /// <summary>
    /// Representa el orden que tendrá cada elemento de una consulta sql (field)
    /// </summary>
    public class Orden
    {
        /// <summary>
        /// Nombre del campo de la consulta sql
        /// </summary>
        public string Campo { get; set; } = "";
        /// <summary>
        /// Posicion que tendrá dentro de la consulta
        /// <para>Valor maximo equivale a la cantidad total de elementos de la consulta. (Fields)</para>
        /// </summary>
        public int Posicion { get; set; }
        /// <summary>
        /// Nombre opcional que se le quiera dar al campo.
        /// </summary>
        public string Alias { get; set; }
        /// <summary>
        /// Indica si el campo se muestra o no en el resultado final.
        /// <para>Por defecto es true (visible)</para>
        /// </summary>
        public bool visible { get; set; } = true;
        /// <summary>
        /// Indica si se ocupa dentro de la clausula order by
        /// </summary>
        public bool OrderBy { get; set; } = false;
        /// <summary>
        /// Indica si el orden es ascendente o descendente
        /// </summary>
        public string OrderType { get; set; } = "";


        /// <summary>
        /// Entrega un elemento a través de su nombre (Field)
        /// </summary>
        /// <returns></returns>
        public Orden FindFromName(string pField, List<Orden> pListado)
        {
            Orden or = new Orden();

            try
            {
                if (pListado.Count > 0)
                {
                    //Si no encuentra nada retorna null...
                    or = pListado.Find(x => x.Campo.ToLower().Contains(pField.ToLower()));
                }
            }
            catch (Exception ex)
            {
                 //ERROR...
            }

            return or;
        }

        /// <summary>
        /// Entrega un elemento de la lista a través de un Id        
        /// </summary>
        public Orden FindByIndex(int pIndex, List<Orden> pListado)
        {
            Orden or = new Orden();
            try
            {
                if (pListado.Count > 0)
                {
                    for (int i = 0; i < pListado.Count; i++)
                    {
                        if (i == pIndex)
                        {
                            or = pListado[i];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //ERROR...
            }

            return or;
        }

        /// <summary>
        /// Genera el listado para la seccion order by
        /// </summary>
        public static string CreateListOrderBy(List<Orden> pListado)
        {
            StringBuilder builder = new StringBuilder();            

            if (pListado.Count > 0)
            {
                foreach (Orden x in pListado)
                {
                    if(x.OrderBy)
                        builder.AppendLine($"{x.Campo} {x.OrderType},");
                }
            }

            if (builder.ToString().Length > 0)
                return builder.ToString().Substring(0, builder.ToString().Length - 3);
            else
                return "";
        }

        /// <summary>
        /// Genera cadena final con campos ordenados.
        /// <para>Esta cadena la reemplazaremos en sql dinamico.</para>
        /// </summary>
        /// <returns></returns>
        public static string GeneraCadenaOrden(List<Orden> Ordenados)
        {
            //List<Orden> Ordenados = new List<Orden>();
            //Ordenados = GetOrderData();
            StringBuilder buil = new StringBuilder();            

            try
            {
                if (Ordenados.Count > 0)
                {
                    //Guardamos en lista para devolver por la interfaz.
                    //ListadoOrderBy = Ordenados;

                    foreach (Orden x in Ordenados)
                    {
                        if (x.Alias.Length > 0)
                        {
                            if (x.visible)
                            {
                                x.Alias = x.Alias.Replace("\'", "");
                                buil.AppendLine($"{x.Campo} as '{x.Alias}',");
                            }
                        }
                        else
                        {
                            if (x.visible)
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


    }

    /// <summary>
    /// Solo para cargar un lookupeditcontrol
    /// </summary>
    class PositionC
    {
        public int id { get; set; }
        public int number { get; set; }
    }

    /// <summary>
    /// Para manipular los reporte que se guardan en base de datos.
    /// </summary>
    public class ReportBuilder
    {
        /// <summary>
        /// Representa al id del elemento
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Nombre del reporte
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Contenido del reporte
        /// </summary>
        public string Sql { get; set; }

        /// <summary>
        /// Entrega un id disponible para un nuevo ingreso en tabla.
        /// </summary>
        private int GetId()
        {
            string sql = "SELECT ISNULL(MAX(id), 0) FROM reporte";
            SqlCommand cmd;
            SqlConnection cn;
            int x = 0;

            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            x = (Convert.ToInt32(cmd.ExecuteScalar()) + 1);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                x = 0;
            }

            return x;
        }

        /// <summary>
        /// Agrega una nueva consulta.
        /// </summary>
        /// <returns></returns>
        public bool AddReport(string pConsulta, string pName)
        {
            string sql = "INSERT INTO reporte(id, data, nombre) VALUES(@pId, @pData, @pName)";
            SqlCommand cmd;
            SqlConnection cn;

            int count = 0, id = 0;
            bool correcto = false;

            //Id disponible
            id = GetId();

            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@pId", id));
                            cmd.Parameters.Add(new SqlParameter("@pData", pConsulta));
                            cmd.Parameters.Add(new SqlParameter("@pName", pName));

                            count = cmd.ExecuteNonQuery();
                            if (count > 0)
                            {
                                correcto = true;                                
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                //ERROR...
                correcto = false;
            }

            return correcto;
        }

        /// <summary>
        /// Modifica una consulta
        /// </summary>
        /// <returns></returns>
        public bool ModReport(int pId, string pConsulta, string pName)
        {
            string sql = "UPDATE reporte SET data=@pData, nombre=@pName WHERE id=@pId";
            SqlCommand cmd;
            SqlConnection cn;
            bool correcto = false;
            int count = 0;
            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@pData", pConsulta));
                            cmd.Parameters.Add(new SqlParameter("@pId", pId));
                            cmd.Parameters.Add(new SqlParameter("@pName", pName));

                            count = cmd.ExecuteNonQuery();
                            if (count > 0)
                            {
                                correcto = true;
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                //ERROR...
                correcto = false;
            }            

            return correcto;
        }

        /// <summary>
        /// Elimina una consulta.
        /// </summary>
        /// <param name="pId"></param>
        /// <param name="pConsulta"></param>
        /// <returns></returns>
        public bool DelReport(int pId, string pConsulta)
        {
            string sql = "DELETE FROM reporte WHERE id=@pId";
            SqlCommand cmd;
            SqlConnection cn;
            bool correcto = false;
            int count = 0;

            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            //parametros
                            cmd.Parameters.Add(new SqlParameter("@pId", pId));

                            count = cmd.ExecuteNonQuery();
                            if (count > 0)
                            {
                                correcto = true;
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                //ERROR...
            }

            return correcto;
        }

        /// <summary>
        /// Entrega la consulta de acuerdo a un id.
        /// </summary>
        /// <param name="pId"></param>
        public void GetReportBuilder(int pId)
        {
            string sql = "SELECT data FROM reporte WHERE id=@pId";
            SqlCommand cmd;
            SqlConnection cn;
            string data = "";

            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@pId", pId));

                            data = (string)cmd.ExecuteScalar();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                //ERROR...
            }
        }

        /// <summary>
        /// Entrega todos los datos guardados en base de datos para cargar en grilla
        /// </summary>
        public DataTable GetDataSource()
        {
            string sql = "SELECT id, nombre, data FROM reporte ORDER BY Id";
            SqlCommand cmd;
            SqlConnection cn;
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            ad.SelectCommand = cmd;
                            ad.Fill(ds, "data");

                            if (ds.Tables.Count > 0)
                            {
                                dt = ds.Tables[0];
                            }
                            else
                            {
                                dt = null;
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                //ERROR...
                dt = null;
            }

            return dt;
        }

        /// <summary>
        /// Nos indica si hay reporte en base de datos
        /// </summary>
        public int Count()
        {
            int c = 0;
            string sql = "SELECT count(*) FROM reporte";
            SqlCommand cmd;
            SqlConnection cn;

            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            c = Convert.ToInt32(cmd.ExecuteScalar());
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                //ERROR...
                
            }

            return c;
        }

        /// <summary>
        /// Genera datasource para gridcontrol en base a consulta sql.
        /// </summary>
        /// <param name="pData"></param>
        public DataTable GetDataFromReport(string pData)
        {
            SqlConnection cn;
            SqlCommand cmd;
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(pData, cn))
                        {
                            ad.SelectCommand = cmd;
                            ad.Fill(ds, "data");

                            if (ds.Tables.Count > 0)
                            {
                                dt = ds.Tables[0];
                            }
                            else
                            {
                                dt = null;
                            }

                            ad.Dispose();
                            ds.Dispose();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                //ERROR...
            }

            return dt;
        }

        /// <summary>
        /// Limpia los datos del reporte.
        /// </summary>
        public void Clear()
        {
            this.Id = 0;
            this.Name = "";
            this.Sql = "";
        }

    }

}
