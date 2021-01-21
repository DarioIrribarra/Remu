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
using System.IO;
using System.Data.SqlClient;
using System.Threading;

namespace Labour
{
    public partial class frmCargaItemExtendida : DevExpress.XtraEditors.XtraForm, ISeleccionMultiple
    {
        private List<string> Columnas = new List<string>() { "contrato",
            "coditem", "monto", "formula", "cuota", "proporcional", "uf", "porcentaje",
        "pesos", "permanente"};

        private List<string> ListadoInsert = new List<string>();
        /// <summary>
        /// Listado de items que se van a  actualizar
        /// </summary>
        private List<ItemTrabajador> ListadoItemsUpdate = new List<ItemTrabajador>();
        /// <summary>
        /// Corresponde a los valores que se van a actualizar
        /// Corresponde al N° de contrato, coditem y valores asociados a ese item
        /// </summary>
        private List<ItemTrabajador> ListadoValoresUpdate = new List<ItemTrabajador>();

        private DataTable TablaHilo = new DataTable();

        public string SqlQuery { get; set; } = "";

        //DELEGADO PARA IR MOSTRANDO EL NOMBRE DEL TRABAJADOR
        delegate void ShowEmploye(string pText);

        delegate void DisableBtn(SimpleButton pButton, bool pOption);

        #region "INTERFAZ SELECCION MULTIPLE"
        public void CargaListado(string pSql)
        {
            SqlQuery = pSql;
        }
        #endregion
        public frmCargaItemExtendida()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool Insert()
        {
            SqlCommand cmd;
            SqlConnection cn;
            SqlTransaction tr;
            bool correcto = false, repetido = false;
            int Selection = Convert.ToInt32(txtOperacion.EditValue);
            string d = "";

            if (ListadoInsert.Count > 0)
            {

                //Es update???
                //Verificar si hay items que se desean actualizar que existen mas de una vez
                if (Selection == 2)
                {
                    var countquery = from r in ListadoItemsUpdate
                                     orderby r.contrato, r.item
                                     group r by new { r.contrato, r.item }
                                     into grupo
                                     select new { key = grupo.Key, cantidad = grupo.Count() };

                    if (countquery != null)
                    {
                        foreach (var x in countquery)
                        {
                            if (x.cantidad > 1)
                            {
                                repetido = true;
                                break;
                            }
                        }

                    }

                    if (repetido)
                    {
                        DialogResult adv = XtraMessageBox.Show("Hay mas de un items con el mismo nombre para una opersona.\nDesea seleccionar items a actualizar?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (adv == DialogResult.Yes)
                        {
                            //Abrir ventana con seleccion
                            frmSeleccionMultiple selec = new frmSeleccionMultiple(ListadoItemsUpdate, ListadoValoresUpdate, true);
                            selec.StartPosition = FormStartPosition.CenterScreen;
                            selec.ShowDialog();

                            string s = SqlQuery;
                        }
                    }
                    else
                    {
                        try
                        {
                            cn = fnSistema.OpenConnection();
                            if (cn != null)
                            {
                                tr = cn.BeginTransaction();
                                try
                                {
                                    //Recorremos listado de queries
                                    foreach (string query in ListadoInsert)
                                    {
                                        using (cmd = new SqlCommand(query, cn))
                                        {
                                            cmd.Transaction = tr;
                                            cmd.ExecuteNonQuery();
                                        }
                                    }

                                    //Si no hubo error, hacemos elcommit
                                    tr.Commit();
                                    correcto = true;
                                }
                                catch (Exception ex)
                                {
                                    tr.Rollback();
                                    correcto = false;
                                }
                            }
                        }
                        catch (SqlException ex)
                        {
                            //Error
                            correcto = false;
                        }
                    }

                }
                else
                {
                    try
                    {
                        cn = fnSistema.OpenConnection();
                        if (cn != null)
                        {
                            tr = cn.BeginTransaction();
                            try
                            {
                                //Recorremos listado de queries
                                foreach (string query in ListadoInsert)
                                {
                                    d = query;
                                    using (cmd = new SqlCommand(query, cn))
                                    {
                                        cmd.Transaction = tr;
                                        cmd.ExecuteNonQuery();
                                    }
                                }

                                //Si no hubo error, hacemos elcommit
                                tr.Commit();
                                correcto = true;
                            }
                            catch (Exception ex)
                            {
                                XtraMessageBox.Show(d);
                                tr.Rollback();
                                correcto = false;
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        //Error
                        correcto = false;
                    }
                }

               
            }

            return correcto;
        }

        /// <summary>
        /// Indica si todas las columnas del excel son columnas válidas...
        /// </summary>
        /// <param name="pColumns"></param>
        /// <returns>Un numero entero que indica la cantidad de coincidencias encontradas</returns>
        private int ColumnValidation(DataColumnCollection pColumns)
        {
            int count = 0;

            if (pColumns.Count > 0)
            {

                foreach (DataColumn col in pColumns)
                {
                    string ColumnName = col.Caption.ToLower().Trim();
                    if (ColumnName.Length == 0)
                    { XtraMessageBox.Show("Los nombres de las columnas no pueden estar vacios", "Columnas", MessageBoxButtons.OK, MessageBoxIcon.Stop); return -1; }
                    if (Columnas.Count(x => x.ToLower().Equals(ColumnName)) > 0)
                        count++;
                    else
                    {
                        XtraMessageBox.Show($"Columna {ColumnName} no es un nombre de columna válida", "Columnas", MessageBoxButtons.OK, MessageBoxIcon.Stop); return -1;
                    }
                }


                //Si count == a cantidad de columnas de la tabla
                //quiere decir que todas las columnas existen en nuestra lista de 
                //columnas validas

            }

            return count;
        }

        /// <summary>
        /// Validacion de columnas 
        /// </summary>
        /// <param name="pDataSource"></param>
        private void ValidateData()
        {
            int ColumnasValidas = 0;
            bool Error = false, repetido = false, ItemsRepetidos = false;
            StringBuilder str = new StringBuilder();
            //1--> Ingresar; 2--> Modificar
            int Selection = 0, numitem = 0, count = 0;
            Selection = Convert.ToInt32(txtOperacion.EditValue);
            List<string> ListadoContratosArchivo = new List<string>();
            ListadoItemsUpdate.Clear();
            ListadoValoresUpdate.Clear();
           // ItemBase it = new ItemBase();

            //Obtener the last numitem from itemtrabajador 
            ListadoInsert.Clear();
            string ContratoFila = "", NombreItemRep = "";
            DisableButton(btnCargar, false);
            DisableButton(btnSalir, false);
            DisableButton(btnSave, false);
            if (TablaHilo.Rows.Count > 0)
            {
                MostrarTrabajador("Iniciando lectura...");

                DataColumnCollection Columnas = TablaHilo.Columns;
                if (Columnas.Count > 0)
                {
                    ColumnasValidas = ColumnValidation(Columnas);
                    if (ColumnasValidas < Columnas.Count)
                    {
                        XtraMessageBox.Show("Por favor verifique el nombre de las columnas", "Columna no válida", MessageBoxButtons.OK, MessageBoxIcon.Stop); Cursor = Cursors.Default;
                        DisableButton(btnCargar, true);
                        DisableButton(btnSalir, true);
                        DisableButton(btnSave, true);
                        return;
                    }
                }

                string Filter = "Convert([contrato], 'System.String') is null OR Convert([contrato], 'System.String')='' " +
                    "OR Convert([monto], 'System.String') is null OR Convert([monto], 'System.String')=''" +
                    "OR Convert([coditem], 'System.String') is null OR Convert([coditem], 'System.String')=''" +
                    "OR Convert([formula], 'System.String') is null OR Convert([formula], 'System.String')=''" +
                    "OR Convert([cuota], 'System.String') is null OR Convert([cuota], 'System.String')=''" +
                    "OR Convert([proporcional], 'System.String') is null OR Convert([proporcional], 'System.String')=''" +
                    "OR Convert([uf], 'System.String') is null OR Convert([uf], 'System.String')=''" +
                    "OR Convert([porcentaje], 'System.String') is null OR Convert([porcentaje], 'System.String')=''" +
                    "OR Convert([pesos], 'System.String') is null OR Convert([pesos], 'System.String')=''" +
                    "OR Convert([permanente], 'System.String') is null OR Convert([permanente], 'System.String')=''" +
                    " ";

                string Filte2 = "";

                DataRow[] FilasEncontradas = null;

                try
                {
                    FilasEncontradas = TablaHilo.Select(Filter);
                }
                catch (Exception e)
                {
                    XtraMessageBox.Show(e.Message);
                    DisableButton(btnCargar, true);
                    DisableButton(btnSalir, true);
                    DisableButton(btnSave, true);

                    return;
                }
                

                if (FilasEncontradas.Length > 0)
                {
                    XtraMessageBox.Show("Por favor verifique columnas con informacion en blanco", "Datos en blanco", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    Cursor = Cursors.Default;
                    DisableButton(btnCargar, true);
                    DisableButton(btnSalir, true);
                    DisableButton(btnSave, true);
                    return;
                }

                DataTable pTablaOrdenada = new DataTable();
                TablaHilo.DefaultView.Sort = "contrato desc";
                pTablaOrdenada = TablaHilo.DefaultView.ToTable();

                //Cuenta la cantidad de veces que se repite un item para un determinado contrato
                 var countquery = from row in pTablaOrdenada.AsEnumerable()
                                 group row by new { contrato = row.Field<string>("contrato"), coditem = row.Field<string>("coditem") }
                                 into table
                                 select new
                                 {
                                     Name = table.Key,
                                     cantidad = table.Count()
                                 };

                //Obtener todos los items sin repetir
                //Esto es para ver si alguno de estos items que deseo actualizar existe mas de una vez para ese contrato
                var itemsSinRepetirQuery = pTablaOrdenada.AsEnumerable().
                    Select(x => x.Field<string>("coditem")).Distinct().ToList();

                int countPorc = pTablaOrdenada.Rows.Count;
                int countRow = 0;

                //Hay algun item repetido???

                foreach (var c in countquery)
                {
                    if (c.cantidad > 1)
                    {
                        ItemsRepetidos = true;
                        NombreItemRep = c.Name.coditem;
                        break;
                    }
                }

                if (ItemsRepetidos)
                {
                    DialogResult adv = XtraMessageBox.Show($"Existen items repetidos,\nDeseas continuar de todas maneras?\n Item repetido encontrado:{NombreItemRep}", "Items Repetidos", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (adv == DialogResult.No)
                    {
                        DisableButton(btnCargar, true);
                        DisableButton(btnSalir, true);
                        DisableButton(btnSave, true);
                        return;
                    }
                        
                }              

                foreach (DataRow Row in pTablaOrdenada.Rows)
                {

                    string insert = $"INSERT INTO itemtrabajador(coditem, anomes, contrato, rut, numitem, formula, tipo, orden, esclase, valor, valorcalculado, proporcional, permanente, contope, cuota, uf, pesos, porc, suspendido, splab13, splab14) " +
                        $"VALUES($coditem, {Calculo.PeriodoObservado}, $contrato, $rut, $numitem, $formula, $tipo, $orden, 0, $monto, 0, $proporcional, $permanente, 0, $cuota, $uf, $pesos, $porc, 0, 0, 0)";

                    string update = $"UPDATE itemtrabajador SET valor=$monto, formula=$formula, proporcional=$proporcional, permanente=$permanente, cuota=$cuota, uf=$uf, pesos=$pesos, porc=$porc " +
                        $"WHERE contrato=$contrato AND coditem=$coditem AND anomes={Calculo.PeriodoObservado}";

                    string c = Row["contrato"].ToString().Trim().ToLower();

                    if (c.Equals(ContratoFila.ToLower()))
                    {
                        numitem++;
                        //Contrato aparece en mas de una fila???
                        repetido = true;
                    }
                    else
                    {
                        numitem = 0;
                        repetido = false;
                    }

                    ItemTrabajador ObjItem = new ItemTrabajador();

                    countRow++;
                    //string update = $"UPDATE itemtrabajador SET coditem=$coditem, ";
                    foreach (DataColumn Col in pTablaOrdenada.Columns)
                    {
                        string ColumnName = Col.Caption.ToLower().Trim();
                        string data = Row[Col].ToString().Trim();

                        if (ColumnName.Equals("contrato"))
                        {
                            if (data.ToString().Length == 0)
                            {
                                XtraMessageBox.Show($"El numero de contrato no puede estar vacío", "Contrato", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                Cursor = Cursors.Default;
                                Error = true;
                                break;
                            }

                            //Verificar si el contrato existe en el periodo observado
                            if (Persona.ExisteContrato(data, Calculo.PeriodoObservado) == false)
                            {
                                XtraMessageBox.Show($"N° de contrato {data} no existe en periodo evaluado", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                Cursor = Cursors.Default;
                                Error = true;
                                break;
                            }
                        

                            insert = insert.Replace("$contrato", $"'{data.Trim()}'");
                            ContratoFila = data.Trim();
                            string rutPersona = Persona.GetRutFromContrato(ContratoFila, Calculo.PeriodoObservado);
                            //Si repetido es falso quiere decir que es un nuevo contrato
                            //Hay que volver a obtener el ultimo numero de item ingresado
                            if(!repetido)
                                numitem = ItemTrabajador.LastNumber(data.Trim(), Calculo.PeriodoObservado) + 1;

                            insert = insert.Replace("$numitem", numitem + "");
                            insert = insert.Replace("$rut", $"'{rutPersona}'");

                            update = update.Replace("$contrato", $"'{data.Trim()}'");

                            //Guardamos contrato en listado
                            ListadoContratosArchivo.Add(ContratoFila);
                            ObjItem.contrato = ContratoFila;

                            MostrarTrabajador($"Leyendo contrato n° {ContratoFila}... {(countRow * 100)/countPorc}%");
                            
                        }
                        if (ColumnName.Equals("monto"))
                        {
                            if (fnSistema.IsDecimal(data) == false)
                            {
                                XtraMessageBox.Show($"Monto {data} no es un valor válido", "Monto Erroneo", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                Cursor = Cursors.Default;
                                Error = true;
                                break;
                            }
                            else
                            {
                                if (data.Trim().Contains(","))
                                    data = data.Replace(",", ".");

                                insert = insert.Replace("$monto", data.Trim() + "");
                                update = update.Replace("$monto", data.Trim() + "");
                                ObjItem.Adicional = Convert.ToDouble(data.Trim());

                            }
                        }

                        if (ColumnName.Equals("coditem"))
                        {
                            if (data.Length == 0 || ItemTrabajador.ExisteItemBase(data) == false)
                            {
                                XtraMessageBox.Show("Por favor ingresa un código de item válido", $"Item no existe => {data.Trim()}", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                Cursor = Cursors.Default;
                                Error = true;
                                break;
                            }

                            //Verificar si existe el item de trabajador
                            //Solo si es update
                            if (Selection == 2)
                            {
                                if (ItemTrabajador.Existe(data.Trim(), ContratoFila, Calculo.PeriodoObservado) == false)
                                {
                                    XtraMessageBox.Show($"El item que estás tratando de actualizar no existe para el contrato {ContratoFila}.\nItem: {data.Trim()}", "Item no existe", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    Cursor = Cursors.Default;
                                    Error = true;
                                    break;
                                }
                            }

                            ItemBase it = new ItemBase(data.Trim());
                            it.Setinfo();
                            insert = insert.Replace("$coditem", $"'{data.Trim()}'");
                            insert = insert.Replace("$orden", it.Orden + "");
                            insert = insert.Replace("$tipo", it.Tipo + "");

                            update = update.Replace("$coditem", $"'{data.Trim()}'");
                            ObjItem.item = data.Trim();

                            MostrarTrabajador($"Leyendo item codigo {data.Trim()}...{(countRow * 100) / countPorc}%");
                        }

                        if (ColumnName.Equals("formula"))
                        {
                            if (data.Length == 0)
                            {
                                XtraMessageBox.Show("Por favor ingresa un codigo de formula válido", "Formula no existe", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                Cursor = Cursors.Default;
                                Error = true;
                                break;
                            }

                            if (data.Equals("0") == false)
                            {
                                if (formula.ExisteFormula(data) == false)
                                {
                                    XtraMessageBox.Show($"No existe la formula {data}\nSi no aplica dejar con valor cero", "Formula", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                    Cursor = Cursors.Default;
                                    Error = true;
                                    break;
                                }
                            }                           

                            insert = insert.Replace("$formula", $"'{data.Trim()}'");
                            update = update.Replace("$formula", $"'{data.Trim()}'");
                            ObjItem.formula = data.Trim();
                        }

                        if (ColumnName.Equals("cuota"))
                        {
                            if (data.Length == 0)
                            {
                                XtraMessageBox.Show("Por favor ingresa un numero cuota valido\nSi no aplica ingrese un cero", "Cuota", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                Error = true;
                                Cursor = Cursors.Default;
                                break;
                            }
                            //Validar formato de cuota
                            if (data != "0")
                            {
                                //validamos formato
                                if (ItemTrabajador.CuotaValida(data) == false)
                                {
                                    XtraMessageBox.Show("Por favor verificar el valor para cuota\nVerifique el numero de cuota no sea mayor al total de cuotas\n*Formato correcto: 2/10 (Cuota 2 de 10)\nSi no aplica dejar con valor cero", "Error formato cuota", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                    Cursor = Cursors.Default;
                                    Error = true;
                                    break;
                                }
                            }

                            insert = insert.Replace("$cuota", $"'{data.Trim()}'");
                            update = update.Replace("$cuota", $"'{data.Trim()}'");
                            ObjItem.Cuota = data.Trim();
                        }

                        //proporcional??
                        if (ColumnName.Equals("proporcional"))
                        {
                            if (data.Length == 0 || fnSistema.IsNumeric(data) == false)
                            {
                                XtraMessageBox.Show("Por favor ingresa un valor valido para columnas proporcional\nValores válidos:\n0 --> No Aplica\n1 --> Si aplica", "Proporcional", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                Cursor = Cursors.Default;
                                Error = true;
                                break;
                            }

                            if (data.Equals("0") == false && data.Equals("1") == false)
                            {
                                XtraMessageBox.Show("Por favor ingresa un valor valido para columnas proporcional\nValores válidos:\n0 --> No Aplica\n1 --> Si aplica", "Proporcional", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                Cursor = Cursors.Default;
                                Error = true;
                                break;
                            }

                            insert = insert.Replace("$proporcional", data.Trim());
                            update = update.Replace("$proporcional", data.Trim());


                        }
                        //Uf??
                        if (ColumnName.Equals("uf"))
                        {
                            if (data.Length == 0 || fnSistema.IsNumeric(data) == false)
                            {
                                XtraMessageBox.Show("Por favor ingresa un valor valido para columnas uf\nValores válidos:\n0 --> No Aplica\n1 --> Si aplica", "Uf", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                Cursor = Cursors.Default;
                                Error = true;
                                break;
                            }

                            if (data.Equals("0") == false && data.Equals("1") == false)
                            {
                                XtraMessageBox.Show("Por favor ingresa un valor valido para columnas uf\nValores válidos:\n0 --> No Aplica\n1 --> Si aplica", "Uf", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                Cursor = Cursors.Default;
                                Error = true;
                                break;
                            }

                            insert = insert.Replace("$uf", data.Trim());
                            update = update.Replace("$uf", data.Trim());
                        }
                        //Porcentaje??
                        if (ColumnName.Equals("porcentaje"))
                        {
                            if (data.Length == 0 || fnSistema.IsNumeric(data) == false)
                            {
                                XtraMessageBox.Show("Por favor ingresa un valor valido para columnas porcentaje\nValores válidos:\n0 --> No Aplica\n1 --> Si aplica", "Porcentaje", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                Cursor = Cursors.Default;
                                Error = true;
                                break;
                            }

                            if (data.Equals("0") == false && data.Equals("1") == false)
                            {
                                XtraMessageBox.Show("Por favor ingresa un valor valido para columnas porcentaje\nValores válidos:\n0 --> No Aplica\n1 --> Si aplica", "Porcentaje", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                Cursor = Cursors.Default;
                                Error = true;
                                break;
                            }

                            insert = insert.Replace("$porc", data.Trim());
                            update = update.Replace("$porc", data.Trim());
                        }
                        //Pesos???
                        if (ColumnName.Equals("pesos"))
                        {
                            if (data.Length == 0 || fnSistema.IsNumeric(data) == false)
                            {
                                XtraMessageBox.Show("Por favor ingresa un valor valido para columnas pesos\nValores válidos:\n0 --> No Aplica\n1 --> Si aplica", "Pesos", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                Cursor = Cursors.Default;
                                Error = true;
                                break;
                            }

                            if (data.Equals("0") == false && data.Equals("1") == false)
                            {
                                XtraMessageBox.Show("Por favor ingresa un valor valido para columnas pesos\nValores válidos:\n0 --> No Aplica\n1 --> Si aplica", "Pesos", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                Cursor = Cursors.Default;
                                Error = true;
                                break;
                            }

                            insert = insert.Replace("$pesos", data.Trim());
                            update = update.Replace("$pesos", data.Trim());
                        }

                        //Permanente???
                        if (ColumnName.Equals("permanente"))
                        {
                            if (data.Length == 0 || fnSistema.IsNumeric(data) == false)
                            {
                                XtraMessageBox.Show("Por favor ingresa un valor valido para columnas permanente\nValores válidos:\n0 --> No Aplica\n1 --> Si aplica", "Permanente", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                Cursor = Cursors.Default;
                                Error = true;
                                break;
                            }

                            if (data.Equals("0") == false && data.Equals("1") == false)
                            {
                                XtraMessageBox.Show("Por favor ingresa un valor valido para columnas permanente\nValores válidos:\n0 --> No Aplica\n1 --> Si aplica", "Permanente", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                Cursor = Cursors.Default;
                                Error = true;
                                break;
                            }

                            insert = insert.Replace("$permanente", data.Trim());
                            update = update.Replace("$permanente", data.Trim());
                        }

                    }

                    if (Error)
                    {
                        ListadoInsert.Clear();
                        break;

                    }

                    if (Selection == 1)
                        ListadoInsert.Add(insert);
                    if (Selection == 2)
                        ListadoInsert.Add(update);

                    ListadoValoresUpdate.Add(ObjItem);

                }
                
                //Solo si es update
                if (Selection == 2)
                {
                    //Del listado de contratos extraer solo contratos no repetidos
                    List<string> ContratosSinRepetir = new List<string>();
                    ContratosSinRepetir = ListadoContratosArchivo.Select(x => x).Distinct().ToList();

                    //Generar listado de todos los items que se intentan actualizar
                    //Para los contratos del listado
                    //Estos los cargaremos en la ventana de seleccion

                    ListadoItemsUpdate = GetListItemUpdate(ContratosSinRepetir, itemsSinRepetirQuery);
                }


                MostrarTrabajador("Terminado");

            }

            DisableButton(btnCargar, true);
            DisableButton(btnSalir, true);
            DisableButton(btnSave, true);
        }

        private List<ItemTrabajador> GetListItemUpdate(List<string> pContratos, List<string> pItems)
        {
            string CadenaQuery = "";
            CadenaQuery = SetSqlSearchItem(pItems, pContratos);
            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader rd;
            List<ItemTrabajador> Items = new List<ItemTrabajador>();
            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(CadenaQuery, cn))
                        {
                            rd = cmd.ExecuteReader();
                            if (rd.HasRows)
                            {
                                while (rd.Read())
                                {
                                    //LLENAMOS LISTADO
                                    Items.Add(new ItemTrabajador() { item = rd["coditem"].ToString(),
                                    contrato = rd["contrato"].ToString(),
                                    calculado = Convert.ToDouble(rd["valorcalculado"]),
                                    NumeroItem = Convert.ToInt32(rd["numitem"]),
                                    });
                                }
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                //error...
            }

            return Items;


        }

        /// <summary>
        /// Entrega query para busqueda de todos los items
        /// </summary>
        /// <param name="pItems"></param>
        /// <param name="pContratos"></param>
        private string SetSqlSearchItem(List<string> pItems, List<string> pContratos)
        {
            string sql = "SELECT contrato, coditem, valorcalculado, numitem FROM itemtrabajador where  " +
                $"anomes={Calculo.PeriodoObservado} AND $option";
            StringBuilder str = new StringBuilder();
            str.Append("(");
            int count = 0;
            foreach (string x in pItems)
            {
                count++;
                if (count < pItems.Count)
                    str.Append($"coditem='{x}' OR ");
                else
                    str.Append($"coditem='{x}'");
            }
            str.Append(")");
            str.Append(" AND ");
            str.Append("(");
            count = 0;
            foreach (string con in pContratos)
            {
                count++;
                if (count < pContratos.Count)
                    str.Append($"contrato='{con}' OR ");
                else
                    str.Append($"contrato='{con}'");
            }

            str.Append(")");
            sql = sql.Replace("$option", str.ToString());
            return sql;

        }

        //MOSTRAR NOMBRE DEL TRABAJADOR QUE SE ESTÁ PROCESANDO
        private void MostrarTrabajador(string pText)
        {
            if (this.InvokeRequired)
            {
                ShowEmploye emp = new ShowEmploye(MostrarTrabajador);

                //PARAMETROS
                object[] parameters = new object[] { pText };

                this.Invoke(emp, parameters);
            }
            else
            {
                lblName.Visible = true;
                lblName.Text = pText;
            }
        }

        private void DisableButton(SimpleButton pButton, bool pOption)
        {
            if (this.InvokeRequired)
            {
                DisableBtn emp = new DisableBtn(DisableButton);

                //PARAMETROS
                object[] parameters = new object[] { pButton, pOption };

                this.Invoke(emp, parameters);
            }
            else
            {
                pButton.Enabled = pOption;
            }
        }

        private void frmCargaItemExtendida_Load(object sender, EventArgs e)
        {
            Opcion op = new Opcion();
            op.CargarCombo(txtOperacion);
        }

        private void btnCargar_Click(object sender, EventArgs e)
        {
            OpenFileDialog abrir = new OpenFileDialog();
            abrir.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
            string rutaArchivo = "", extension = "";
            bool correcto = false;

            Cursor = Cursors.WaitCursor;

            if (txtOperacion.Properties.DataSource == null || txtOperacion.EditValue == null)
            { XtraMessageBox.Show("Por favor selecciona una opción válida", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); Cursor = Cursors.Default; return; }

            if (abrir.ShowDialog() == DialogResult.OK)
            {
                //PARA GUARDAR LOS DATOS
                DataTable Tabla = new DataTable();

                rutaArchivo = abrir.FileName;
                if (File.Exists(rutaArchivo) == false)
                {
                    XtraMessageBox.Show("Ruta archivo no existe", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Cursor = Cursors.Default;
                    return;
                }

                txtRuta.Text = abrir.FileName;
                extension = Path.GetExtension(abrir.FileName);

                Tabla = FileExcel.ReadExcelDev(abrir.FileName);
                if (Tabla.Rows.Count == 0)
                { XtraMessageBox.Show("No se encontró información en archivo", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); Cursor = Cursors.Default; return; }

                TablaHilo = Tabla;

                ThreadStart s = new ThreadStart(ValidateData);
                Thread hilo = new Thread(s);
                hilo.Start();
                //ValidateData(Tabla);

                Cursor = Cursors.Default;

            }

            Cursor = Cursors.Default;
        }

        private void RunInBackGround()
        {

        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtOperacion.Properties.DataSource == null || txtOperacion.EditValue == null)
            { XtraMessageBox.Show("Por favor selecciona una operacion válida", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (File.Exists(txtRuta.Text))
            {
                //Hacemos el insert O UPDATE SEGUN CORRESPONDA
                if (ListadoInsert.Count == 0)
                {
                    XtraMessageBox.Show("No se pudo realizar operacion", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }

                if (Insert())
                {
                    XtraMessageBox.Show("Registros guardados correctamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                else
                {
                    XtraMessageBox.Show("No se pudo ingresar datos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }

                Close();


            }
            else
            {
                XtraMessageBox.Show("Ruta de archivo no válida", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }
    }
}