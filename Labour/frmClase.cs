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
using System.Collections;
using System.Runtime.InteropServices;

namespace Labour
{
    public partial class frmClase : DevExpress.XtraEditors.XtraForm
    {
        [DllImport("user32")]
        extern static IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32")]
        extern static bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        const int MF_BYCOMMAND = 0;
        const int MF_DISABLED = 2;
        const int SC_CLOSED = 0xF060;

        //PARA COMUNICACION CON FORMULARIO EMPLEADO
        public IEmpleadoClase opener { get; set; }

        //VARIABLE BOOLEANA PARA SABER SI ES UPDATE O INSERT
        private bool updateItemClase = false;

        //VARIABLE BOOLEANA PARA SABER SI ES UPDATE O INSERT EN EL CASO DE LA CLASE
        private bool updateClase = false;

        //OBTENER FILA ACTUAL
        private int FilaActual = 0;

        //OBTERNER CODIGO CLASE SELECCIONADA
        private int CodigoClase = 0;

        //PARA GUARDAR RUT Y CONTRATO DEL EMPLEADO EN CASO DE QUE SE ABRA DESDE FORM EMPLEADO
        private string rut = "";
        private string contrato = "";

        //BOTON NUEVO
        private bool CancelaClase = true;

        //BOTON NUEVO ITEMCLASE
        private bool CancelaItemClase = true;

        //BOTON NUEVO
        Operacion opClase;
        Operacion opItemClase;

        public frmClase()
        {
            InitializeComponent();
        }

        //CONSTRUCTOR PARAMETRIZADO EN CASO DE QUE EL FORMULARIO SE ABRA DESDE FORM EMPLEADO
        public frmClase(string rut, string contrato)
        {
            InitializeComponent();
            this.rut = rut;
            this.contrato = contrato;
        }

        private void frmClase_Load(object sender, EventArgs e)
        {
            var sm = GetSystemMenu(Handle, false);
            EnableMenuItem(sm, SC_CLOSED, MF_BYCOMMAND | MF_DISABLED);
            
            fnDefaultProperties();
            defaultClase();

            opClase = new Operacion();
            opItemClase = new Operacion();

            //CARGAR GRILLA CLASE ITEM
            string grillaClase = "SELECT codClase, descClase FROM CLASE";
            fnSistema.spllenaGridView(gridClase, grillaClase);
            fnSistema.spOpcionesGrilla(viewClase);
            fnColumnasClase();                   

            //CARGAR CAMPOS CLASE
            CargarCamposClase(0);

            //CARGAR COMBOS
            fnComboItem("SELECT coditem, descripcion FROM item", txtitem, "coditem", "descripcion", true);

            //CARGAR GRILLA CLASE ITEM
            string grilla = "";
            if (CodigoClase != 0)
            {                
                grilla = string.Format("SELECT clase, numitem, item, formula from itemClase INNER JOIN clase on clase.codClase = itemClase.clase WHERE itemclase.clase={0} ORDER BY numitem", CodigoClase);
               
                fnSistema.spllenaGridView(griditemClase, grilla);
                fnSistema.spOpcionesGrilla(viewitemClase);
                fnColumnasItem();

                //CARGAR CAMPOS ITEM CLASE
                fnCargarCampos(0);               
            }          

            //MOSTRAR CLASE SELECCIONADA (EN LABEL FORMULARIO ITEM CLASE)
            if (CodigoClase != 0)
                groupItemClase.Text = "ITEM CLASE: " + fnDescripcionClase(CodigoClase);
            else
                groupItemClase.Text = "NO SE HA SELECCIONADO CLASE";
        }

        #region "MANEJO DE DATOS ITEM CLASE"
       
        
        //NUEVO REGISTRO
        /*
         * DATOS DE ENTRADA:
         * CODIGO 
         * DESCRIPCION
         * ITEM (VIENE DE TABLA ITEM)
         * FORMULA (VIENE DE TABLA FORMULA)
         * NUMERO ITEM
         */

        private void fnNuevaitemClase(int pcod, LookUpEdit pitem, LookUpEdit pformula, TextEdit pnum)
        {
            //INSERT SQL
            string sql = "INSERT INTO ITEMCLASE(clase, item, formula, numitem) VALUES " +
                "(@pcod, @pitem, @pformula, @pnum)";

            SqlCommand cmd;
            int res = 0;

            bool valido = false;

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pcod", pcod));                 
                        cmd.Parameters.Add(new SqlParameter("@pitem", pitem.EditValue));
                        cmd.Parameters.Add(new SqlParameter("@pformula", pformula.EditValue));
                        cmd.Parameters.Add(new SqlParameter("@pnum", pnum.Text));

                        //EJECUTAR CONSULTA
                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            XtraMessageBox.Show("Registro guardado", "Guardar", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            logRegistro log = new logRegistro(User.getUser(), "SE INGRESA NUEVO ITEM EN CLASE " + pcod, "ITEMCLASE", "0", (string)pitem.EditValue, "INGRESAR");
                            log.Log();

                            //CARGAR NUEVO ITEM CLASE EN ITEMTRABAJADOR QUE USEN LA CLASE
                            valido = AgregaElementoTrabajador(pcod, Calculo.PeriodoEvaluar());

                            if (valido)
                            {
                                ActualizarNumeroitem(pcod, (string)pitem.EditValue);
                            }

                            //RECARGAR GRILLA
                            string grilla = "";
                            if (CodigoClase != 0)
                                grilla = string.Format("select clase, item, formula, numitem from itemClase INNER JOIN clase on clase.codClase = itemClase.clase WHERE itemclase.clase={0} ORDER BY numitem", CodigoClase);
                            
                            fnSistema.spllenaGridView(griditemClase, grilla);

                            //CARGAR CAMPOS
                            fnCargarCampos(0);

                            if (opener != null)
                                opener.RecargarDatosClase();                           
                        }
                        else
                        {
                            XtraMessageBox.Show("Error al intentar guardar", "Guardar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    //LIBERAR RECURSOS
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                }
                else
                {
                    XtraMessageBox.Show("No hay conexion con la base de datos", "Base de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        //AGREGAR NUEVO ITEM CLASE (VERSION TRANSACCION)
        private bool NuevaItemClase(int pcod, LookUpEdit pitem, LookUpEdit pformula, TextEdit pnum)
        {
            Cursor.Current = Cursors.WaitCursor;

            bool transaccionCorrecta = false;
            int AgregaItem = 0;

            List<string> Listado = new List<string>();
            //Listado = ListaContratos(Calculo.PeriodoObservado);
            Listado = ListadoContratosUsaClase(Calculo.PeriodoObservado, pcod);
            

            //INSERT SQL
            string sql = "INSERT INTO ITEMCLASE(clase, item, formula, numitem) VALUES " +
                "(@pcod, @pitem, @pformula, @pnum)";

            //SQL INSERT PARA ITEM TRABAJADOR (SI NO EXISTE )
            string sqlAgregaItem = "declare @num INT " +
                                  "select @num = MAX(numitem) + 1 FROM itemtrabajador WHERE contrato = @pcontrato AND anomes = @periodo " +
                                 "INSERT INTO itemtrabajador(rut, contrato, anomes, coditem, formula, tipo, orden, numitem) " +
                                 "SELECT rut, contrato, anomes, itemclase.item, itemclase.formula, tipo, orden, row_number() OVER(ORDER BY numitem) + @num FROM( " +
                                        "SELECT rut, contrato, anomes, itemClase.item, itemClase.formula, tipo, orden, numitem " +
                                                "FROM trabajador inner JOIN clase on clase.codClase = trabajador.clase " +
                                                "INNER JOIN itemclase on itemClase.clase = clase.codClase " +
                                                "INNER JOIN item on item.coditem = itemClase.item " +
                                                "WHERE contrato = @pcontrato AND clase.codClase = @pclase AND trabajador.anomes = @periodo " +
                                                "EXCEPT " +
                                                "SELECT itemTrabajador.rut, itemTrabajador.contrato, trabajador.anomes, " +
                                                "itemTrabajador.coditem, itemTrabajador.formula, itemTrabajador.tipo, itemtrabajador.orden, itemclase.numitem " +
                                                "FROM itemTrabajador INNER JOIN trabajador on trabajador.contrato = itemTrabajador.contrato " +
                                                "INNER JOIN clase on clase.codClase = trabajador.clase " +
                                                "INNER JOIN itemClase on itemClase.clase = clase.codClase " +
                                                "WHERE itemTrabajador.contrato = @pcontrato " +
                                               "AND clase.codClase = @pclase AND itemtrabajador.anomes = @periodo " +
                                   ")itemclase";

            //PARA SABER SI EL ITEM EXISTE EN ITEMTRABAJADOR...
            bool existeItem = false;
            existeItem = ExisteCodItem((string)pitem.EditValue, pcod);

            //SOLO AGREGAMOS ITEMS SI ALGUN TRABAJADOR TIENE ASOCIADA LA CLASE
            bool ClaseOcupada = false;
            ClaseOcupada = ExisteClaseEnTrabajador(pcod, Calculo.PeriodoObservado);

            SqlCommand cmd;
            SqlTransaction tran;

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    //INICIAMOS LA TRANSACCION
                    tran = fnSistema.sqlConn.BeginTransaction();
                    try
                    {
                        //INGRESAMOS NUEVO ITEM
                        using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@pcod", pcod));
                            cmd.Parameters.Add(new SqlParameter("@pitem", pitem.EditValue));
                            cmd.Parameters.Add(new SqlParameter("@pformula", pformula.EditValue));
                            cmd.Parameters.Add(new SqlParameter("@pnum", pnum.Text));

                            //AGREGAMOS A TRANSACCION
                            cmd.Transaction = tran;

                            //EJECUTAMOS CONSULTA
                            cmd.ExecuteNonQuery();

                            cmd.Dispose();
                        }

                        /*Agregamos items a trabajores que tengan asociada la clase*/
                        if (ClaseOcupada)
                        {
                            if (Listado.Count > 0)
                            {
                                foreach (var contrato in Listado)
                                {
                                    using (cmd = new SqlCommand(sqlAgregaItem, fnSistema.sqlConn))
                                    {
                                        //PARAMETROS
                                        cmd.Parameters.Add(new SqlParameter("@pclase", pcod));
                                        cmd.Parameters.Add(new SqlParameter("@periodo", Calculo.PeriodoObservado));
                                        cmd.Parameters.Add(new SqlParameter("@pcontrato", contrato));

                                        //AGREGAMOS A TRANSACCION
                                        cmd.Transaction = tran;

                                        AgregaItem = cmd.ExecuteNonQuery();
                                        cmd.Dispose();
                                    }

                                } 
                            }
                        }                      

                        //SI TODO SALIO BIEN GUARDAMOS LOS CAMBIOS
                        tran.Commit();
                        transaccionCorrecta = true;
                    }
                    catch (Exception ex)
                    {
                        //SI SE PRODUCE ALGUNA EXCEPCION DESHACEMOS TODOS LOS CAMBIOS
                        transaccionCorrecta = false;
                        tran.Rollback();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            //SI LA TRANSACCION FUE EXITOSA GUARDAMOS EVENTO EN LOG
            if (transaccionCorrecta)
            {
                logRegistro log = new logRegistro(User.getUser(), "SE INGRESA NUEVO ITEM EN CLASE " + pcod, "ITEMCLASE", "0", (string)pitem.EditValue, "INGRESAR");
                log.Log();

                //ACTUALIZAR NUMERO DE ITEM SI ES QUE SE AGREGA A ITEMTRABAJADOR
               // if (AgregaItem > 0)
                 //   ActualizarNumeroitem(pcod, (string)pitem.EditValue);

                //RECARGAR GRILLA
                string grilla = "";
                if (CodigoClase != 0)
                    grilla = string.Format("select clase, item, formula, numitem from itemClase INNER JOIN clase on clase.codClase = itemClase.clase WHERE itemclase.clase={0} ORDER BY numitem", CodigoClase);

                fnSistema.spllenaGridView(griditemClase, grilla);

                //CARGAR CAMPOS
                fnCargarCampos(0);

                opItemClase.Cancela = false;
                opItemClase.SetButtonProperties(btnnuevoitem, 1);

                if (opClase.Cancela)
                {
                    opClase.Cancela = false;
                    opClase.SetButtonProperties(btnNuevaClase, 1);
                }


               // if (opener != null)
                 //   opener.RecargarDatosClase();
            }

            return transaccionCorrecta;
        }

        //ACTUALIZAR CLASE
        //PARAMETROS DE ENTRADA LOS MISMOS DE ARRIBA
        private void fnModificaritemClase(LookUpEdit pitem, LookUpEdit pformula, TextEdit pnum, 
            int pClase, string pOriginalItem, int pOriginalNum)
        {
            //OBTENER TODOS ID ASOCIADOS A ESE ITEM (SUPONIENDO QUE SE REALIZARA UN UPDATE CORRECTO)
            List<int> listado = new List<int>();
            //listado = IdAsociadoUpdate(pClase, pOriginalItem);

            //SQL UPDATE
            string sql = "UPDATE ITEMCLASE SET item=@pitem, formula=@pformula, numitem=@pnum " +
                " WHERE clase=@pClase AND item=@pOriginalItem AND numitem=@pOriginalNum";
            SqlCommand cmd;
            int res = 0;

            //OBTENER DE ACUERDO A ITEM EL TIPO Y EL NUMERO DE ORDEN
            List<int> infoitem = new List<int>();
            infoitem = DatosItem((string)pitem.EditValue);            

            //TABLA HASH PARA LOG
            Hashtable datosItemClase = new Hashtable();
            datosItemClase = PrecargaDatosItemClase(pClase, pOriginalItem, pOriginalNum);
         
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pClase", pClase));
                        cmd.Parameters.Add(new SqlParameter("@pOriginalItem", pOriginalItem));
                        cmd.Parameters.Add(new SqlParameter("@pOriginalNum", pOriginalNum));
                        cmd.Parameters.Add(new SqlParameter("@pitem", pitem.EditValue));
                        cmd.Parameters.Add(new SqlParameter("@pformula", pformula.EditValue));
                        cmd.Parameters.Add(new SqlParameter("@pnum", pnum.Text));

                        //EJECUTAR CONSULTA
                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            XtraMessageBox.Show("Actualizacion correcta", "Actualizar", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            //SI HAY CAMBIOS GUARDAMOS VALORES EN LOG
                            ComparaValorItemClase(datosItemClase, pClase, (string)pitem.EditValue, (string)pformula.EditValue, int.Parse(pnum.Text));
                            //LIMPIAR DATOS HASH
                            datosItemClase.Clear();

                            //SI SE ACTUALIZO CORRECTAMENTE ACTUALIZAMOS VALOR EN TABLA ITEM TRABAJADOR
                            //SIEMPRE Y CUANDO EXISTAN REGISTROS DE ITEM

                            if(listado.Count>0)
                            fnActualizarDatoItem(pitem.EditValue.ToString(), pformula.EditValue.ToString(), listado, infoitem[1], infoitem[0]);

                            //RECARGAR GRILLA
                            string grilla = "";
                            if (CodigoClase != 0)
                                grilla = string.Format("select clase, item, formula, numitem from itemClase INNER JOIN clase on clase.codClase = itemClase.clase WHERE itemclase.clase={0} ORDER BY numitem", CodigoClase);

                            fnSistema.spllenaGridView(griditemClase, grilla);
                            fnCargarCampos(0);
                        }
                        else
                        {
                            XtraMessageBox.Show("Error al intentar actualizar", "Actualizar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }

                    //LIBERAR RECURSOS
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                    
                }
                else
                {
                    XtraMessageBox.Show("No hay conexion con la base de datos", "Base de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        //METODO PARA MODIFICACION V2 (USANDO TRANSACCION)
        private bool ModificarItemClase(LookUpEdit pitem, LookUpEdit pFormula, TextEdit pnum, 
            int pClase, string pOriginalItem, int pOriginalNum)
        {
            Cursor.Current = Cursors.WaitCursor;

            bool transaccionCorrecta = false, existeItem = false;
            List<string> listaContratos = new List<string>();
            //PARA SABER SI SE AGREGO NUEVO ITEM
            int AgregaItem = 0;

            //SI LOS ITEM SON DISTINTOS, VERIFICAMOS SI EL CODIGO DE ITEM YA EXISTE EN TABLA ITEM TRABAJADOR...
            if (pOriginalItem != (string)pitem.EditValue)
                existeItem = ExisteItemTrabajador(pClase, (string)pitem.EditValue);

            //OBTENER TODOS LOS CONTRATOS DE TABLA ITEMTRABAJADOR QUE TIENEN ASOCIADO EL CODIGO DE ITEM
            List<ItemActualizable> listado = new List<ItemActualizable>();
            listado = RegistroAsociadoUpdate(pClase, pOriginalItem);

            ///Listado de contratos que tienen asociada la clase
            listaContratos = ListaContratos(Calculo.PeriodoObservado, pClase);

            //SQL UPDATE
            string sql = "UPDATE ITEMCLASE SET item=@pitem, formula=@pformula, numitem=@pnum " +
                " WHERE clase=@pClase AND item=@pOriginalItem AND numitem=@pOriginalNum";

            //PARA ITEM TRABAJADOR ASOCIADOS
            string sqlUpdateAsociados = " UPDATE itemTrabajador SET coditem = @pitem, " +
                                        "formula = @pformula, tipo=@tipo, orden=@orden " +
                                        " WHERE contrato=@pcontrato AND coditem=@pItemAntiguo AND numitem=@pnum AND anomes=@periodo";

            //QUERY PARA AGREGAR NUEVO ELEMENTO A ITEM TRABAJADOR EN CASO DE QUE SE CAMBIO CODIGO ITEM Y 
            //ESTE NO ESTE EN ITEMTRABAJADOR
            string sqlAgregaItem = "declare @num INT " +
                                  "select @num = MAX(numitem) + 1 FROM itemtrabajador WHERE contrato = @pcontrato AND anomes = @periodo " +
                                 "INSERT INTO itemtrabajador(rut, contrato, anomes, coditem, formula, tipo, orden, numitem) " +
                                 "SELECT rut, contrato, anomes, itemclase.item, itemclase.formula, tipo, orden, row_number() OVER(ORDER BY numitem) + @num FROM( " +
                                        "SELECT rut, contrato, anomes, itemClase.item, itemClase.formula, tipo, orden, numitem " +
                                                "FROM trabajador inner JOIN clase on clase.codClase = trabajador.clase " +
                                                "INNER JOIN itemclase on itemClase.clase = clase.codClase " +
                                                "INNER JOIN item on item.coditem = itemClase.item " +
                                                "WHERE contrato = @pcontrato AND clase.codClase = @pclase AND trabajador.anomes = @periodo " +
                                                "EXCEPT " +
                                                "SELECT itemTrabajador.rut, itemTrabajador.contrato, trabajador.anomes, " +
                                                "itemTrabajador.coditem, itemTrabajador.formula, itemTrabajador.tipo, itemtrabajador.orden, itemclase.numitem " +
                                                "FROM itemTrabajador INNER JOIN trabajador on trabajador.contrato = itemTrabajador.contrato " +
                                                "INNER JOIN clase on clase.codClase = trabajador.clase " +
                                                "INNER JOIN itemClase on itemClase.clase = clase.codClase " +
                                                "WHERE itemTrabajador.contrato = @pcontrato " +
                                               "AND clase.codClase = @pclase AND itemtrabajador.anomes = @periodo " +
                                   ")itemclase";

            //SQL UPDATE PARA ACTUALIZAR LOS ITEM DE ITEMTRABAJADOR ASOCIADOS (CAMBIAMOS CAMPO ES CLASE A 0)
            string sqlUpdateItemTrabajador = "UPDATE itemtrabajador SET esclase = 0 FROM itemtrabajador " +
                                             "INNER JOIN trabajador ON trabajador.contrato = itemtrabajador.contrato " +
                                              "WHERE trabajador.clase = @pClase AND itemTrabajador.anomes = @periodo AND coditem = @pcodItem";            

            //OBTENER EL TIPO Y EL NUMERO DE ORDEN DEL ITEM
            List<int> infoitem = new List<int>();
            infoitem = DatosItem((string)pitem.EditValue);

            //TABLA HASH PARA LOG   
            Hashtable datosItemClase = new Hashtable();
            datosItemClase = PrecargaDatosItemClase(pClase, pOriginalItem, pOriginalNum);

            SqlCommand cmd;
            SqlTransaction tran;

            //POS PARA BUCLE WHILE
            int pos = 0;

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    //INICIAMOS LA TRANSACCION...
                    tran = fnSistema.sqlConn.BeginTransaction();
                    try
                    {
                        //UPDATE ITEMCLASE
                        using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@pClase", pClase));
                            cmd.Parameters.Add(new SqlParameter("@pOriginalItem", pOriginalItem));
                            cmd.Parameters.Add(new SqlParameter("@pOriginalNum", pOriginalNum));
                            cmd.Parameters.Add(new SqlParameter("@pitem", pitem.EditValue));
                            cmd.Parameters.Add(new SqlParameter("@pformula", pFormula.EditValue));
                            cmd.Parameters.Add(new SqlParameter("@pnum", pnum.Text));

                            //AGREGAMOS A TRANSACCION
                            cmd.Transaction = tran;

                            cmd.ExecuteNonQuery();
                        }

                        //HACEMOS UPDATE A ITEM ASOCIADOS (SI ES QUE HAY ITEM ASOCIADOS EN TABLA ITEMTRABAJADOR...)
                        //SI EL CODIGO DE ITEM NO SE HA MODIFICADO PODEMOS HACER UPDATE SIN PROBLEMA...
                        if (listado.Count>0 && ((string)pitem.EditValue == pOriginalItem))
                        {
                            //RECORREMOS TODOS LOS ID QUE SE ENCUENTRAN EN LA LISTA
                            while (pos < listado.Count)
                            {
                                using (cmd = new SqlCommand(sqlUpdateAsociados, fnSistema.sqlConn))
                                {
                                    //PARAMETROS
                                    cmd.Parameters.Add(new SqlParameter("@pitem", (string)pitem.EditValue));
                                    cmd.Parameters.Add(new SqlParameter("@pformula", (string)pFormula.EditValue));                            
                                    cmd.Parameters.Add(new SqlParameter("@periodo", Calculo.PeriodoObservado));
                                    cmd.Parameters.Add(new SqlParameter("@orden", infoitem[1]));
                                    cmd.Parameters.Add(new SqlParameter("@tipo", infoitem[0]));
                                    cmd.Parameters.Add(new SqlParameter("@pcontrato", listado[pos].contrato));
                                    cmd.Parameters.Add(new SqlParameter("@pItemAntiguo", listado[pos].item));
                                    cmd.Parameters.Add(new SqlParameter("@pnum", listado[pos].numitem));

                                    //AGREGAMOS A TRANSACCION
                                    cmd.Transaction = tran;

                                    //EJECUTAMOS 
                                    cmd.ExecuteNonQuery();

                                    cmd.Dispose();
                                }
                                pos++;
                            }
                        }
                        if (listado.Count>0 && ((string)pitem.EditValue != pOriginalItem))
                        {
                            //SI EL CODIGO DE ITEM CAMBIÓ SOLO CAMBIAMOS VALOR DE CAMPO ES CLASE A 0
                            using (cmd = new SqlCommand(sqlUpdateItemTrabajador, fnSistema.sqlConn))
                            {
                                //PARAMETROS
                                cmd.Parameters.Add(new SqlParameter("@pClase", pClase));
                                cmd.Parameters.Add(new SqlParameter("@periodo", Calculo.PeriodoObservado));
                                cmd.Parameters.Add(new SqlParameter("@pcodItem", pOriginalItem));

                                //AGREGAMOS A TRANSACCION
                                cmd.Transaction = tran;

                                cmd.ExecuteNonQuery();
                                cmd.Dispose();
                            } 
                           
                        }
                        if (((string)pitem.EditValue != pOriginalItem) && existeItem == false)
                        {
                            foreach (var contrato in listaContratos)
                            {
                                //SI EL CODIGO DE ITEM CAMBIO Y ESTE ITEM NO EXISTE EN ITEMTRABAJADOR LO AGREGAMOS..
                                using (cmd = new SqlCommand(sqlAgregaItem, fnSistema.sqlConn))
                                {
                                    //PARAMETROS
                                    cmd.Parameters.Add(new SqlParameter("@pclase", pClase));
                                    cmd.Parameters.Add(new SqlParameter("@periodo", Calculo.PeriodoObservado));
                                    cmd.Parameters.Add(new SqlParameter("@pcontrato", contrato));

                                    //AGREGAMOS A TRANSACCION
                                    cmd.Transaction = tran;
                                    AgregaItem = cmd.ExecuteNonQuery();
                                    cmd.Dispose();
                                }
                            }                           
                         

                            //SI SE AGREGA NUEVO ITEM ACTUALIZAMOS NUMERO DE ITEM...
                        }
                        
                        //SI TODO HA SALIDO BIEN CONFIRMAMOS LOS CAMBIOS
                        tran.Commit();

                        //TRANSACCION CORRECTA!
                        transaccionCorrecta = true;
                    }
                    catch (Exception ex)
                    {
                        //SI SE PRODUCE ALGUNA EXCEPCION DESHACEMOS LOS CAMBIOS
                        transaccionCorrecta = false;
                        tran.Rollback();
                    }
                }
                //CERRAMOS CONEXION
                fnSistema.sqlConn.Close();
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            //SI LA VARIABLE TRANSACCIONCORRECTA ES TRUE ES PORQUE TODO EL PROCESO SE REALIZO DE FORMA CORRECTA!
            if (transaccionCorrecta)
            {
                //SI HAY CAMBIOS GUARDAMOS VALORES EN LOG
                ComparaValorItemClase(datosItemClase, pClase, (string)pitem.EditValue, (string)pFormula.EditValue, int.Parse(pnum.Text));
                //LIMPIAR DATOS HASH
                datosItemClase.Clear();

                //ACTUALIZAMOS NUMERO DE ITEN EN ITEMTRABAJADOR
               // if(AgregaItem > 0)
                 //   ActualizarNumeroitem(pClase, (string)pitem.EditValue);

                //RECARGAR GRILLA
                string grilla = "";
                if (CodigoClase != 0)
                    grilla = string.Format("select clase, item, formula, numitem from itemClase INNER JOIN clase on clase.codClase = itemClase.clase WHERE itemclase.clase={0} ORDER BY numitem", CodigoClase);

                fnSistema.spllenaGridView(griditemClase, grilla);
                fnCargarCampos(0);

                if (opClase.Cancela)
                {
                    opClase.Cancela = false;
                    opClase.SetButtonProperties(btnNuevaClase, 1);
                }

            }

            return transaccionCorrecta;
        }

        //ELIMINAR CLASE
        //PARAMETROS DE ENTRADA:
        //CODIGO
        //CODITEM
        //NUMERO ITEM
        private void fnEliminaritemClase(int pcod, string pitem,int  pnum)
        {
            //SQL DELETE
            string sql = "DELETE FROM ITEMCLASE WHERE clase=@pcod AND item=@pitem AND numitem=@pnum";
            SqlCommand cmd;

            int res = 0;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pcod", pcod));
                        cmd.Parameters.Add(new SqlParameter("@pitem", pitem));
                        cmd.Parameters.Add(new SqlParameter("@pnum", pnum));

                        //EJECUTAR CONSULTA
                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            XtraMessageBox.Show("Registro Eliminado", "Eliminar", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            logRegistro log = new logRegistro(User.getUser(), "SE ELIMINA ITEM " + pitem + " Numero " + pnum + " DESDE CLASE " + pcod,"ITEMCLASE", pitem, "0", "ELIMINAR");
                            log.Log();

                            //RECARGAR GRILLA
                            string grilla = "";
                            if (CodigoClase != 0)
                                grilla = string.Format("select clase, item, formula, numitem from itemClase INNER JOIN clase on clase.codClase = itemClase.clase WHERE itemclase.clase={0} ORDER BY numitem", CodigoClase);

                            fnSistema.spllenaGridView(griditemClase, grilla);
                            fnCargarCampos(0);                         
                        }
                        else
                        {
                            XtraMessageBox.Show("Error al intentar eliminar", "Eliminar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    //LIBERAR RECURSOS
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                }
                else
                {
                    XtraMessageBox.Show("No hay conexion con la base de datos", "Base de datos", MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        //ELIMINAR ITEM CLASE (USANDO TRANSACCION)
        //Version 2
        private bool EliminarItemClaseTransaccion(string item, int codClase, int numItem)
        {
            //@1 ELIMINAR DESDE TABLA ITEMTRABAJADOR EL ITEM ASOCIADO (QUE CORRESPONDA AL ITEMCLASE)
            //@2 ELIMINAR ITEM CLASE DESDE TABLA ITEMCLASE
            bool existeItemTrabajador = false, transaccionCorrecta = false;
            existeItemTrabajador = fnVerificarLigado(item, codClase);            

            //SQL PARA ELIMINACION ITEM TRABAJADOR (QUE ES ITEMCLASE)
            //string sqlDeleteItem = "DELETE itemtrabajador FROM itemTrabajador " +
            //            " INNER JOIN trabajador ON trabajador.contrato = itemtrabajador.contrato " +
            //            " INNER JOIN itemclase ON itemclase.clase = trabajador.clase " +
            //            " WHERE itemclase.clase = @pClase AND itemtrabajador.coditem = @pcodItem " +
            //            " AND itemtrabajador.esclase=1  AND itemtrabajador.anomes=@periodo";

            string sqlDeleteItem = "DELETE itemtrabajador FROM itemtrabajador " +
                                   "INNER JOIN trabajador On trabajador.contrato = itemtrabajador.contrato " +
                                   "AND trabajador.anomes = itemtrabajador.anomes " +
                                   "WHERE trabajador.clase = @pClase " +
                                   "AND coditem = @pcodItem AND itemtrabajador.anomes = @pPeriodo";

            //SQL PARA ELIMINAR ITEMCLASE
            string sqlDeleteItemClase = "DELETE FROM ITEMCLASE WHERE clase=@pClase AND item=@pcodItem AND numitem=@pnum";

            //SQL UPDATE PARA ACTUALIZAR LOS ITEM DE ITEMTRABJAADOR ASOCIADOS (CAMBIAMOS CAMPO ES CLASE A 0)
            string sqlUpdateItemTrabajador = "UPDATE itemtrabajador SET esclase = 0 FROM itemtrabajador " +
                                             "INNER JOIN trabajador ON trabajador.contrato = itemtrabajador.contrato "+
                                              "WHERE trabajador.clase = @pClase AND itemTrabajador.anomes = @periodo AND coditem = @pcodItem";

            //USAREMOS EL PERIODO EN CURSO COMO PARAMETRO
            int periodoCurso = 0;
            periodoCurso = Calculo.PeriodoObservado;

            //PARA TRANSACCION
            SqlTransaction tran;
            SqlCommand cmd;

            string pregunta = "";
            if (existeItemTrabajador)
                pregunta = "Este item esta asociado a un trabajador, ¿Realmente desea eliminar?";
            else
                pregunta = "¿Realmente desea eliminar este item?";

            DialogResult advertencia = XtraMessageBox.Show(pregunta, "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (advertencia == DialogResult.Yes)
            {
                try
                {
                    if (fnSistema.ConectarSQLServer())
                    {
                        tran = fnSistema.sqlConn.BeginTransaction();
                        try
                        {                            
                            //ELIMINAMOS ITEMCLASE
                            using (cmd = new SqlCommand(sqlDeleteItemClase, fnSistema.sqlConn))
                            {
                                //PARAMETROS
                                cmd.Parameters.Add(new SqlParameter("@pClase", codClase));
                                cmd.Parameters.Add(new SqlParameter("@pcodItem", item));
                                cmd.Parameters.Add(new SqlParameter("@pnum", numItem));

                                cmd.Transaction = tran;

                                cmd.ExecuteNonQuery();
                                cmd.Dispose();
                            }

                            //SI EXISTE ITEM ASOCIADO A ITEM TRABAJADOR HACEMOS UPDATE DE CAMPO ESCLASE (DEJAMOS EN 0)
                            if (existeItemTrabajador)
                            {
                                using (cmd = new SqlCommand(sqlDeleteItem, fnSistema.sqlConn))
                                {
                                    //PARAMETROS
                                    cmd.Parameters.Add(new SqlParameter("@pClase", codClase));
                                    cmd.Parameters.Add(new SqlParameter("@pcodItem", item));
                                    cmd.Parameters.Add(new SqlParameter("@pPeriodo", periodoCurso));

                                    cmd.Transaction = tran;

                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();
                                }
                            }

                            //SI TODO SALIÓ BIEN REALIZAMOS COMMIT (PARA QUE SE GUARDEN LOS CAMBIOS)
                            tran.Commit();
                            transaccionCorrecta = true;
                        }
                        catch (Exception ex)
                        {
                            //SI HUBO ALGUN PROBLEMA HACEMOS ROOLBACK (DESHACEMOS CAMBIOS)
                            transaccionCorrecta = false;
                            tran.Rollback();                            
                        }
                    }
                }
                catch (SqlException ex)
                {
                    XtraMessageBox.Show(ex.Message);
                }
            }
            return transaccionCorrecta;
        }

        //LIMPIAR CAMPOS
        private void fnlimpiarCampos()
        {
            int num = 0;
            txtitem.ItemIndex = 0;
            txtitem.Properties.Appearance.BackColor = Color.Orange;
            txtitem.Focus();
            txtformula.ItemIndex = 0;
            updateItemClase = false;

            //CARGAR NUMERO ITEM DISPONIBLE

            if (HayDatosClase())
            {
                num = 1;
                num = fnItemDisponibles();

                txtnumitem.Text = num.ToString();
            }
            else
            {
                //PONEMOS FOCO EN CODIGO CLASE
                txtcod.Focus();
            }   
            
        }

        //DEFAULT PROPERTIES
        private void fnDefaultProperties()
        {
            txtnumitem.Properties.MaxLength = 2;
            txtformula.ItemIndex = 0;
            txtitem.ItemIndex = 0;  
            separador1.TabStop = false;
            btnnuevoitem.TabStop = false;
            btnguardarItemClase.TabStop = false;
            btnEliminaritemClase.TabStop = false;
            griditemClase.TabStop = false;
            
        }

        //CARGAR COMBO ITEM
        private void fnComboItem(string pSql, LookUpEdit pCombo, string pCampoKey, string pCampoDesc, bool? pOcultarKey = false)
        {
            List<formula> lista = new List<formula>();

            SqlCommand cmd;
            SqlDataReader rd;

            if (fnSistema.ConectarSQLServer())
            {
                using (cmd = new SqlCommand(pSql, fnSistema.sqlConn))
                {
                    // lista.Add(new formula() { key="0", desc = "SIN FORMULA"});
                    rd = cmd.ExecuteReader();
                    if (rd.HasRows)
                    {
                        while (rd.Read())
                        {
                            //AGREGAMOS VALORES A LA LISTA                                    
                            lista.Add(new formula() { key = (string)rd[pCampoKey], desc = (string)rd[pCampoDesc] });
                        }
                    }
                }
                //LIBERAR RECURSOS
                cmd.Dispose();
                rd.Close();
                fnSistema.sqlConn.Close();
            }

            pCombo.Properties.DataSource = lista.ToList();
            pCombo.Properties.ValueMember = "key";
            pCombo.Properties.DisplayMember = "desc";

            if (lista.Count > 0)
            {
                //SI ENCONTRO RESULTADOS DEJAMOS SELECCIONADO EL SEGUNDO ELEMENTO DE LA LISTA
                pCombo.ItemIndex = 1;
            }
            pCombo.Properties.PopulateColumns();
            //SI OCULTAR KEY ES TRUE NO SE DEBE MOSTRAR LA COLUMNA KEY
            if (pOcultarKey == true)
            {
                pCombo.Properties.Columns[0].Visible = false;
            }
            pCombo.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
            pCombo.Properties.AutoSearchColumnIndex = 1;
            pCombo.Properties.ShowHeader = false;
        }

        //CARGAR COMBO FORMULA
        private void fnComboFormula(string pSql, LookUpEdit pCombo, string pCampoKey, string pCampoDesc, bool? pOcultarKey = false, bool? sinFormula = false)
        {
            List<formula> lista = new List<formula>();

            SqlCommand cmd;
            SqlDataReader rd;

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(pSql, fnSistema.sqlConn))
                    {
                        lista.Add(new formula() { key="0", desc = "SIN FORMULA"});
                        //SI LA VARIABLE SIN FORMULA ES TRUE ES PORQUE EL ITEM YA TIENE COMO REGISTRO SIN FORMULA
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                string code = (string)rd[pCampoKey];
                                if (code != "0")
                                {
                                    //AGREGAMOS VALORES A LA LISTA                                    
                                    lista.Add(new formula() { key = (string)rd[pCampoKey], desc = (string)rd[pCampoDesc] });
                                }                               
                            }
                        }
                    }
                    //LIBERAR RECURSOS
                    cmd.Dispose();
                    rd.Close();
                    fnSistema.sqlConn.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
           

            pCombo.Properties.DataSource = lista.ToList();
            pCombo.Properties.ValueMember = "key";
            pCombo.Properties.DisplayMember = "desc";

            if (lista.Count > 0)
            {
                //SI ENCONTRO RESULTADOS DEJAMOS SELECCIONADO EL SEGUNDO ELEMENTO DE LA LISTA
                pCombo.ItemIndex = 1;
            }
            pCombo.Properties.PopulateColumns();
            //SI OCULTAR KEY ES TRUE NO SE DEBE MOSTRAR LA COLUMNA KEY
            if (pOcultarKey == true)
            {
                pCombo.Properties.Columns[0].Visible = false;
            }
            pCombo.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
            pCombo.Properties.AutoSearchColumnIndex = 1;
            pCombo.Properties.ShowHeader = false;
        }

        //OPCIONES GRILLA
        private void fnColumnasItem()
        {
            //select clase, numitem,  item, formula from itemClase 
            //INNER JOIN clase on clase.codClase = itemClase.clase
            
                //NO MOSTRAR CODIGO CLASE
                viewitemClase.Columns[0].Visible = false;

                viewitemClase.Columns[1].Width = 10;
                viewitemClase.Columns[1].Caption = "N°";
                viewitemClase.Columns[1].AppearanceCell.FontStyleDelta = FontStyle.Bold;

                viewitemClase.Columns[2].Caption = "Item";

                viewitemClase.Columns[3].Caption = "Formula";                
        }

        //CARGAR CAMPOS
        private void fnCargarCampos(int? pos = -1)
        {
            //select clase, item, formula, numitem from itemClase 
            //INNER JOIN clase on clase.codClase = itemClase.clase ORDER BY numitem
            if (viewitemClase.RowCount > 0)
            {
                if (pos != 0) viewitemClase.FocusedRowHandle = (int)pos;

                lblerror.Visible = false;                

                //DEJAMOS COMO TRUE VARIABLE UPDATE
                updateItemClase = true;

                //CARGAMOS CAMPOS                                                
                txtformula.EditValue = viewitemClase.GetFocusedDataRow()["formula"].ToString();

                txtitem.EditValue = viewitemClase.GetFocusedDataRow()["item"].ToString();
                txtnumitem.Text = viewitemClase.GetFocusedDataRow()["numitem"].ToString();

                txtitem.Properties.Appearance.BackColor = Color.LightGoldenrodYellow;             
                
            }
            else
            {
                updateItemClase = false;
                fnlimpiarCampos();
            }
        }

        //VERIFICAR SI REGISTRO EXISTE
        //RETORNA TRUE SI EL REGISTRO YA EXISTE
        private bool fnRegistroExiste(string pcod, string pitem, string pnum)
        {
            string sql = "SELECT * FROM ITEMCLASE WHERE clase=@pcod AND item=@pitem AND numitem=@pnum";
            SqlCommand cmd;
            SqlDataReader rd;
            bool encontrado = false;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pcod", pcod));
                        cmd.Parameters.Add(new SqlParameter("@pitem", pitem));
                        cmd.Parameters.Add(new SqlParameter("@pnum", pnum));

                        //EJECUTAR CONSULTA
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //SI ENCUENTRA FILAS EL REGISTRO YA EXISTE
                            encontrado = true;
                        }
                        else
                        {
                            encontrado = false;
                        }
                    }
                }
                else
                {
                    XtraMessageBox.Show("No hay conexion con la base de datos", "Base de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            //RETURN VAR
            return encontrado;
        }

        //VERIFICAR SI EL CODIGO INGRESADO EXISTE EN BD(solo para obtener la descripcion)
        //RETORNA TRUE SI EXISTE
        private bool fnExisteCodigo(string pCod)
        {
            string sql = "SELECT codClase FROM clase WHERE codClase=@pCod";
            SqlCommand cmd;
            SqlDataReader rd;
            bool existe = false;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pCod", pCod));

                        //EJECUTAR CONSULTA
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //SI RETORNA FILAS ES PORQUE EL CODIGO YA EXISTE
                            existe = true;
                        }
                        else
                        {
                            existe = false;
                        }
                    }
                    //LIBERAR RECURSOS
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                }
                else
                {
                    XtraMessageBox.Show("No hay conexion con la base de datos", "Base de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
            return existe;
        }

        //OBTENER LA DESCRIPCION DE ACUERDO AL CODIGO CLASE
        //RETORNA UN STRING CORRESPONDIENTE A LA DESCRIPCION
        private string fnDescripcionClase(int pCod)
        {
            string sql = "SELECT descClase FROM CLASE WHERE codClase=@pCod";
            SqlCommand cmd;
            SqlDataReader rd;
            string desc = "";
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PAREMeTROS
                        cmd.Parameters.Add(new SqlParameter("@pCod", pCod));

                        //EJECUTAR CONSULTA
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //RECORREMOS
                            while (rd.Read())
                            {
                                desc = (string)rd["descClase"];
                            }
                        }
                    }
                    //LIBERAR RECURSOS
                    cmd.Dispose();
                    rd.Close();
                    fnSistema.sqlConn.Close();
                }
                else
                {
                    XtraMessageBox.Show("No hay conexion con la base de datos", "Base de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
            //RETURN DESCRIPCION
            return desc;
        }

        //ACTUALIZAR REGISTRO EN TABLA ITEMTRABAJADOR SI SE CAMBIA UN DATO DE UN REGISTRO EN TABLA CLASE
        //SIEMPRE Y CUANDO EL REGISTRO EXISTA

        private void fnActualizarDatoItem(string pitemCambio, string pformulaCambio, List<int> listadoId, int orden, int tipo)
        {
            string sql = " UPDATE itemTrabajador SET coditem = @pcoditem, formula = @pformula, tipo=@tipo, orden=@orden " +
                         " WHERE id = @pid AND anomes=@periodo";

            //RECORRER LISTADO ID (HACER UPDATE EN CADA UNO DE ELLOS)
            int cantidad = listadoId.Count;
            int pos = 0, periodo = 0;
            periodo = Calculo.PeriodoEvaluar();
            SqlCommand cmd;

            try
            {
                while (pos < cantidad)
                {
                    if (fnSistema.ConectarSQLServer())
                    {
                        using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@pcoditem", pitemCambio));
                            cmd.Parameters.Add(new SqlParameter("@pformula", pformulaCambio));
                            cmd.Parameters.Add(new SqlParameter("@pid", listadoId[pos]));
                            cmd.Parameters.Add(new SqlParameter("@periodo", periodo));
                            cmd.Parameters.Add(new SqlParameter("@orden", orden));
                            cmd.Parameters.Add(new SqlParameter("@tipo", tipo));

                            //EJECUTAR CONSULTA
                            cmd.ExecuteNonQuery();
                        }
                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                    }
                    pos++;
                }                
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        //VERIFICAR SI UN REGISTRO QUE SE QUIERE ELIMINAR ESTA LIGADO COMO ITEM A ITEM TRABAJADOR
        //RETORNA TRUE SI EL VALOR EXISTE
        private bool fnVerificarLigado(string pcodItem, int pClase)
        {
            string sql = "SELECT nombre, clase, coditem FROM itemtrabajador " +
                        "INNER JOIN trabajador ON trabajador.contrato = itemTrabajador.contrato " +
                        "WHERE trabajador.clase = @pClase AND itemTrabajador.coditem = @pcodItem";

            SqlCommand cmd;
            SqlDataReader rd;
            bool existe = false;

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pcodItem", pcodItem));
                        cmd.Parameters.Add(new SqlParameter("@pClase", pClase));

                        //EJECUTAR CONSULTAS
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //SI ENCUENTRA FILAS ES PORQUE EXISTE EL REGISTRO EN TABLA ITEMTRABAJADOR
                            existe = true;
                        }
                        else
                        {
                            existe = false;
                        }
                    }
                    //LIBERAR RECURSOS
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            //RETORNAR VARIABLE EXISTE
            return existe;
        }

        //ELIMINAR REGISTRO DESDE TABLA ITEMTRABAJADOR
        private bool fnEliminarItemTrabajador(string pcodItem, int pClase)
        {
            string sql = " DELETE itemtrabajador FROM itemTrabajador " +
                        " INNER JOIN trabajador ON trabajador.contrato = itemtrabajador.contrato " +
                        " INNER JOIN itemclase ON itemclase.clase = trabajador.clase " +
                        " WHERE itemclase.clase = @pClase AND itemtrabajador.coditem = @pcodItem " +
                        " AND itemtrabajador.esclase=1  AND itemtrabajador.anomes=@periodo";      

            SqlCommand cmd;
            int res = 0, periodo = 0;
            double calculado = 0;
            periodo = Calculo.PeriodoEvaluar();
            bool eliminado = false;

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pClase", pClase));
                        cmd.Parameters.Add(new SqlParameter("@pcodItem", pcodItem));
                        cmd.Parameters.Add(new SqlParameter("@periodo", periodo));

                        //EJECUTAR CONSULTA
                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            
                            //LOG REGISTRO
                            //logRegistro log = new logRegistro(User.getUser(), "SE ELIMINA REGISTRO ITEMTRABAJADOR " + pcodItem, "ITEMTRABAJADOR", pcodItem, "0", "ELIMINAR");
                            //log.Log();
                            eliminado = true;
                        }
                        else
                        {
                            //XtraMessageBox.Show("No se pudieron eliminar los item asociados");
                            eliminado = false;
                        }
                    }
                    //LIBERAR RECURSOS
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                }
                else
                {
                    //NO HAY CONEXION CON LA BASE DE DATOS
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return eliminado;
        }

        //VERIFICAR SI ITEM A INGRESAR YA EXISTE (NUMERO )
        private bool fnExisteItemIngresar(int pnumitem, int pClase)
        {
            string sql = "SELECT numitem FROM itemclase WHERE numitem=@pnumitem AND clase=@pClase";
            SqlCommand cmd;
            SqlDataReader rd;
            bool existe = false;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pnumitem", pnumitem));
                        cmd.Parameters.Add(new SqlParameter("@pClase", pClase));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //SI EXISTE REGISTROS ES PORQUE EXISTE ESE NUMERO
                            existe = true;
                        }
                        else
                        {
                            existe = false;
                        }

                    }
                    //LIBERAR RECURSOS
                    cmd.Dispose();
                    rd.Close();
                    fnSistema.sqlConn.Close();
                }
                else
                {
                    XtraMessageBox.Show("No hay conexion con la base de datos", "Base de datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
            return existe;
        }

        //OBTENER TODOS LOS ID RELACIONES CON ITEMTRABAJADOR QUE SEAN POSIBLES UPDATE
        private List<ItemActualizable> RegistroAsociadoUpdate(int pClase, string pcoditem)
        {
            //OBTENER PERIODO EN EVALUACION
            int periodoEval = 0;
            periodoEval = Calculo.PeriodoObservado;

            List<ItemActualizable> listadoItems = new List<ItemActualizable>();
            
            string sql = "SELECT itemtrabajador.contrato, itemTrabajador.anomes, coditem," +
                " numitem " +
                "FROM itemTrabajador " +
                "INNER JOIN trabajador ON trabajador.contrato = itemTrabajador.contrato AND " +
                "trabajador.anomes = itemtrabajador.anomes " +
                "WHERE trabajador.clase = @pclase AND itemTrabajador.esclase = 1 AND coditem=@pcoditem " +
                " AND itemTrabajador.anomes = @periodo";

            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pclase", pClase));
                        cmd.Parameters.Add(new SqlParameter("@pcoditem", pcoditem));
                        cmd.Parameters.Add(new SqlParameter("@periodo", periodoEval));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //RECORREMOS registros
                            while (rd.Read())
                            {
                             //   listado.Add((int)rd["id"]);
                                listadoItems.Add(new ItemActualizable() { contrato = (string)rd["contrato"],
                                item = (string)rd["coditem"], numitem = Convert.ToInt32(rd["numitem"]),
                                periodo = Convert.ToInt32(rd["anomes"])});
                            }
                        }
                    }
                    //LIBERAR RECURSOS
                    cmd.Dispose();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
            return listadoItems;
        }

        //VERIFICAR SI EXISTE CODIGO DE ITEM A INGRESAR
        private bool ExisteCodItem(string item, int codClase)
        {
            bool existe = false;
            string sql = "SELECT item FROM itemclase WHERE item=@item AND clase=@clase";
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@item", item));
                        cmd.Parameters.Add(new SqlParameter("@clase", codClase));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            existe = true;
                        }
                        else
                        {
                            existe = false;
                        }
                    }
                    cmd.Dispose();
                    rd.Close();
                    fnSistema.sqlConn.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return existe;
        }

        /// <summary>
        /// Indica si una clase está siendo ocupada o no por un trabajador en el periodo seleccionado
        /// </summary>
        /// <param name="pContrato"></param>
        /// <param name="pClase">Codigo de la clase</param>
        /// <param name="pPeriodo"></param>
        /// <returns></returns>
        private bool ExisteClaseEnTrabajador(int pClase, int pPeriodo)
        {
            string sql = "SELECT count(*) FROM trabajador where clase=@pclase AND anomes=@pPeriodo AND status=1";
            SqlCommand cmd;
            SqlConnection cn;
            int count = 0;
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
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@pclase", pClase));                   
                            cmd.Parameters.Add(new SqlParameter("@pPeriodo", pPeriodo));

                            object data = cmd.ExecuteScalar();
                            if (data != DBNull.Value)
                            {
                                count = Convert.ToInt32(data);
                                if (count > 0)
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

        //VERIFICAR SI EXISTE EL CODIGO DE ITEM (PARA TABLE ITEM TRABAJADOR)
        private bool ExisteItemTrabajador(int pClase, string pcodItem)
        {
            string sql = "SELECT coditem, itemTrabajador.contrato FROM itemtrabajador " +
                        "INNER JOIN trabajador ON trabajador.contrato = itemtrabajador.contrato " +
                        "WHERE trabajador.clase = @pClase and itemTrabajador.anomes = @periodo " +
                        "AND itemtrabajador.coditem = @pCodItem";
            SqlCommand cmd;
            SqlDataReader rd;
            bool existe = false;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pClase", pClase));
                        cmd.Parameters.Add(new SqlParameter("@periodo", Calculo.PeriodoObservado));
                        cmd.Parameters.Add(new SqlParameter("@pCoditem", pcodItem));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //SI HAY REGISTROS ES PORQUE EL ITEM YA EXISTE
                            existe = true;
                        }
                        else
                        {
                            existe = false;
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

            return existe;
        }

        /// <summary>
        /// Listado de items que tienen asociada la clase
        /// </summary>
        /// <param name="periodo"></param>
        /// <param name="pClase"></param>
        /// <returns></returns>
        private List<string> ListaContratos(int periodo, int pClase)
        {
            List<string> Lista = new List<string>();
            string sql = "SELECT contrato FROM trabajador WHERE anomes = @periodo AND clase=@pClase AND status=1";
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@periodo", periodo));
                        cmd.Parameters.Add(new SqlParameter("@pClase", pClase));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                Lista.Add((string)rd["contrato"]);
                            }
                        }
                    }
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return Lista;
        }

        /// <summary>
        /// Entrega un listado de contratos que usan una clase determinada
        /// </summary>
        /// <param name="pPeriodo"></param>
        /// <param name="pClase"></param>
        /// <returns></returns>
        private List<string> ListadoContratosUsaClase(int pPeriodo, int pClase)
        {
            List<string> Lista = new List<string>();
            string sql = "SELECT contrato FROM trabajador WHERE anomes = @periodo AND clase=@pClase AND status=1";
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pClase", pClase));
                        cmd.Parameters.Add(new SqlParameter("@periodo", pPeriodo));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                Lista.Add((string)rd["contrato"]);
                            }
                        }
                    }
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return Lista;
        }

        #region "SOLO PARA ITEMTRABAJADOR"
        //AGREGAR NUEVO ITEMCLASE A ITEMTRABAJADOR EN CASO DE QUE SE AGREGUE UN NUEVO ITEM
        private bool AgregaElementoTrabajador(int clase, int periodo)
        {
            string sql = " INSERT INTO itemtrabajador(rut, contrato, anomes, coditem, formula, tipo, orden)" +
                         " SELECT rut, contrato, anomes, itemClase.item, itemClase.formula, tipo, orden " +
                        " FROM trabajador inner JOIN clase on clase.codClase = trabajador.clase "+
                        " INNER JOIN itemclase on itemClase.clase = clase.codClase " +
                        " INNER JOIN item on item.coditem = itemClase.item " +
                        " WHERE clase.codClase = @pClase AND trabajador.anomes = @periodo " +
                        " EXCEPT " +
                        " SELECT itemTrabajador.rut, itemTrabajador.contrato, trabajador.anomes, " +
                        " itemTrabajador.coditem, itemTrabajador.formula, item.tipo, item.orden " +
                        " FROM itemTrabajador INNER JOIN trabajador on trabajador.contrato = itemTrabajador.contrato " +
                        " INNER JOIN clase on clase.codClase = trabajador.clase " +
                        " INNER JOIN itemClase on itemClase.clase = clase.codClase " +
                        " INNER JOIN item on item.coditem = itemClase.item " +
                        " WHERE clase.codClase = @pClase AND itemtrabajador.anomes = @periodo";

            SqlCommand cmd;
            int res = 0;
            bool correcto = false;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pClase", clase));
                        cmd.Parameters.Add(new SqlParameter("@periodo", periodo));

                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            //INGRESO CORRECTO
                            correcto = true;
                        }
                        else
                        {
                            //ERROR!!
                            correcto = false;
                        }
                    }
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
            return correcto;
        }

        //OBTENER TODOS LOS CONTRATOS QUE TIENEN ASOCIADA ESA CLASE
        //SOLO PARA MANIPULAR EL NUMERO DE ITEM UNA VEZ GUARDADO EL NUEVO ITEM
        private List<ItemClaseTrabajador> ListaContratosClase(int clase)
        {   
            List<ItemClaseTrabajador> listado = new List<ItemClaseTrabajador>();

            string sql = " SELECT DISTINCT itemTrabajador.contrato, itemtrabajador.rut from itemtrabajador " +
                         " INNER JOIN trabajador ON trabajador.contrato = itemTrabajador.contrato " +
                        " INNER JOIN itemclase ON itemclase.clase = trabajador.clase " +
                        " WHERE itemclase.clase = @clase AND itemTrabajador.anomes = @periodo";
            
            int anomes = 0;
            anomes = Calculo.PeriodoEvaluar();

            SqlCommand cmd;
            SqlDataReader rd;

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@clase", clase));
                        cmd.Parameters.Add(new SqlParameter("@periodo", anomes));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //AGREGAR CONTRATOS
                                listado.Add(new ItemClaseTrabajador() { contrato = (string)rd["contrato"], rut = (string)rd["rut"]});
                            }
                        }
                    }
                    cmd.Dispose();
                    rd.Close();
                    fnSistema.sqlConn.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return listado;
        }

        //ACTUALIZAR EL NUMERO DE ITEM
        private void ActualizarNumeroitem(int clase, string CodigoItem)
        {
            //OBTENER PAR RUT Y CONTRATO 
            List<ItemClaseTrabajador> lista = new List<ItemClaseTrabajador>();
            lista = ListaContratosClase(clase);

            //LISTA DE ID (REPRESENTA EL ID DEL NUEVO ELEMENTO INGRESADO Y AL QUE SE LE DEBE ACTUALIZAR EL NUMERO DE ITEM)
            int idItem = 0;

            //PARA GUARDAR EL ITEM DISPONIBLE
            int NumItem = 0;

            //CONSULTA SQL PARA UPDATE
            string sql = " UPDATE itemTrabajador set numitem=@pNum FROM itemTrabajador " +
               " INNER JOIN item on item.coditem = itemTrabajador.coditem " +
                " INNER JOIN itemClase on itemClase.item = item.coditem AND rut = @pRut " +
                " AND contrato = @pContrato AND itemClase.clase = @pClase AND id = @pId AND itemtrabajador.anomes=@periodo";

            SqlCommand cmd;
            int periodo = 0;
            periodo = Calculo.PeriodoEvaluar();

            if (lista.Count > 0)
            {
                //RECORREMOS LISTADO Y ACTUALIZAMOS POR CADA CONTRATO EL NUMERO DE ITEM
                //...
                foreach (var elemento in lista)
                {
                    //NUMERO DE ITEM NUEVO PARA EL ITEM
                    NumItem = ItemDisponible(elemento.contrato, elemento.rut);

                    //OBTENER EL ID DEL NUEVO ITEM TRABAJADOR
                    idItem = IdNuevo(elemento.contrato, elemento.rut, CodigoItem);

                    //REALIZAMOS UPDATE POR CADA CONTRATO DE LA LISTA
                    try
                    {
                        if (fnSistema.ConectarSQLServer())
                        {
                            using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                            {
                                //PARAMETROS
                                cmd.Parameters.Add(new SqlParameter("@pNum", NumItem));
                                cmd.Parameters.Add(new SqlParameter("@pRut", elemento.rut));
                                cmd.Parameters.Add(new SqlParameter("@pContrato", elemento.contrato));
                                cmd.Parameters.Add(new SqlParameter("@pClase", clase));
                                cmd.Parameters.Add(new SqlParameter("@pId", idItem));
                                cmd.Parameters.Add(new SqlParameter("@periodo", periodo));

                                cmd.ExecuteNonQuery();
                            }
                            cmd.Dispose();
                            fnSistema.sqlConn.Close();                            
                        }
                    }
                    catch (SqlException ex)
                    {
                        XtraMessageBox.Show(ex.Message);
                    }   
                }
            }           
        }

        //TRAER TODOS LOS NUMEROS DE ITEM
        //RECIBE COMO PARAMETROS DE ENTRADA RUT Y CONTRATO
        private List<int> AllItems(string rut, string contrato)
        {
            //CONSIDERAR COMO PERIODO, EL PERIODO EN CURSO
            int periodo = 0;
            periodo = Calculo.PeriodoEvaluar();
            List<int> numeros = new List<int>();
            string sql = "SELECT numitem FROM itemtrabajador WHERE contrato=@contrato AND rut=@rut AND anomes=@periodo";
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS    
                        cmd.Parameters.Add(new SqlParameter("@contrato", contrato));
                        cmd.Parameters.Add(new SqlParameter("@rut", rut));
                        cmd.Parameters.Add(new SqlParameter("@periodo", periodo));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                numeros.Add((int)rd["numitem"]);
                            }
                        }
                    }
                    cmd.Dispose();
                    rd.Close();
                    fnSistema.sqlConn.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }


            return numeros;
        }

        //CONSULTAR POR EL ULTIMO ITEM INGRESADO
        private int fnUltimoNumero(string pcontrato, string prut)
        {
            string sql = "SELECT max(numitem) as maximo FROM itemtrabajador WHERE contrato=@pcontrato AND rut=@prut AND anomes=@periodo";
            SqlCommand cmd;
            SqlDataReader rd;
            int num = 0, periodo = 0;
            periodo = Calculo.PeriodoEvaluar();
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        cmd.Parameters.Add(new SqlParameter("@pcontrato", pcontrato));
                        cmd.Parameters.Add(new SqlParameter("@prut", prut));
                        cmd.Parameters.Add(new SqlParameter("@periodo", periodo));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //RECORREMOS SI ENCONTRO REGISTROS
                            while (rd.Read())
                            {
                                num = (int)rd["maximo"];
                            }
                        }
                        else
                        {
                            //SI NO ENCONTRO REGISTROS RETORNAMOS -1 QUE SIGNIFICA QUE NO HAY NUMEROS INGRESADOS
                            num = -1;
                        }
                    }
                }
                else
                {
                    XtraMessageBox.Show("No hay conexion con la base de datos", "Base de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return num;
        }

        //ITEM DISPONIBLE
        private int ItemDisponible(string contrato, string rut)
        {
            //LISTADO NUMERO DE ITEM
            List<int> listadoNumeros = new List<int>();
            listadoNumeros = AllItems(rut, contrato);

            int num1 = 0, num2 = 0;
            int generado = 0;
            int devuelve = 0;

            if (listadoNumeros.Count > 1)
            {
                //ORDENAR DE FORMA ASCENDENTE LOS NUMEROS DE ITEMS
                listadoNumeros.Sort();
                for (int pos = 0; pos < listadoNumeros.Count - 1; pos++)
                {
                    num1 = listadoNumeros[pos];
                    num2 = listadoNumeros[pos + 1];
                    if ((num2 - num1) > 1)
                    {
                        //HAY ELEMENTOS QUE NO ESTAN EN USO...
                        generado = num1 + 1;
                    }
                }

                //SI GENERADO SIGUE SIENDO 0 A PESAR DE RECORRER TODO EL ARREGLO
                //ES PORQUE NO HAY DIGITOS ENTRE MEDIO DISPONIBLES
                if (generado == 0)
                {
                    //RETORNAMOS EL ULTIMO ELEMENTO INGRESADO EN BD + 1
                    if (rut != "" && contrato != "")
                        devuelve = fnUltimoNumero(contrato, rut) + 1;

                }
                else
                {
                    //SI ES DISTINTO DE CERO RETORNAMOS EL NUMERO GENERADO
                    devuelve = generado;
                }

            }
            else if (listadoNumeros.Count == 1)
            {
                //SI SOLO TIENE UN ELEMENTO...
                num1 = listadoNumeros[0];
                if (num1 == 1)
                {
                    devuelve = num1 + 1;
                }
                else if (num1 > 1 && num1 != 0)
                {
                    devuelve = num1 - 1;
                }
            }
            else
            {
                //SI NO TIENE ELEMENTOS RETORNA UN 1
                devuelve = 1;
            }

            return devuelve;
        }

        //LISTADO DE IDS
        private int IdNuevo(string contrato, string rut, string coditem)
        {
            int dato = 0;
            string sql = "SELECT id FROM itemtrabajador WHERE contrato=@pContrato AND rut=@pRut " +
                " AND esclase=1 AND anomes=@periodo AND coditem=@item";

            int periodo = 0;
            periodo = Calculo.PeriodoEvaluar();
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@item", coditem));
                        cmd.Parameters.Add(new SqlParameter("@pRut", rut));
                        cmd.Parameters.Add(new SqlParameter("@pContrato", contrato));
                        cmd.Parameters.Add(new SqlParameter("@periodo", periodo));
                     

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //GUARDAMOS DATOS EN LISTA
                                dato = (int)rd["id"];
                            }
                        }
                    }
                    cmd.Dispose();
                    rd.Close();
                    fnSistema.sqlConn.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }           
            return dato;
        }

        //OBTENER EL VALOR CALCULADO DEL ITEM
        private double ItemCalculado(string contrato, int periodo, string item)
        {
            double valor = 0;
            string sql = "SELECT valorcalculado FROM itemtrabajador WHERE contrato=@contrato AND anomes=@periodo AND " +
                "coditem = @coditem";

            SqlCommand cmd;
            
            return valor;
        }

        //OBTENER EL TIPO Y EL NUMERO DE ORDEN DE UN ITEM
        private List<int> DatosItem(string item)
        {
            List<int> datos = new List<int>();
            string sql = "SELECT tipo, orden FROM item WHERE coditem=@item";
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@item", item));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                datos.Add((int)rd["tipo"]);
                                datos.Add((int)rd["orden"]);
                            }
                        }
                    }
                    cmd.Dispose();
                    rd.Close();
                    fnSistema.sqlConn.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return datos;
        }

        //VERIFICAR SI HAY CAMBIOS
        private bool CambiosItemClase(int pClase, string item, int num)
        {
            string sql = "SELECT item, formula, numitem FROM itemclase WHERE clase=@pClase AND item=@Item AND numitem=@num";
          
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pClase", pClase));
                        cmd.Parameters.Add(new SqlParameter("@Item", item));
                        cmd.Parameters.Add(new SqlParameter("@num", num));
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {

                                //COMPARAMOS                              
                                if (txtitem.EditValue.ToString() != (string)rd["item"])
                                    return true; 
                                if (txtformula.EditValue.ToString() != (string)rd["formula"])
                                    return true; 
                                if (Convert.ToInt32(txtnumitem.Text) != Convert.ToInt32(rd["numitem"]))
                                    return true; 
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

            return false;
        }

        #endregion

        #region "LOG ITEM CLASE"
        //TABLA HASH PRECARGADA CON DATOS DEL REGISTRO EN EVALUACION
        private Hashtable PrecargaDatosItemClase(int Clase, string item, int Numitem)
        {
            Hashtable datos = new Hashtable();
            string sql = "SELECT clase, item, formula, numitem FROM itemclase WHERE clase=@pclase AND item=@item" +
                " AND numitem=@num";
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pclase", Clase));
                        cmd.Parameters.Add(new SqlParameter("@item", item));
                        cmd.Parameters.Add(new SqlParameter("@num", Numitem));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //GUARDAR DATOS EN HASH
                                datos.Add("clase", (int)rd["clase"]);
                                datos.Add("item", (string)rd["item"]);
                                datos.Add("formula", (string)rd["formula"]);
                                datos.Add("numitem", (int)rd["numitem"]);
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
            return datos;
        }

        //COMPARAR VALORES
        private void ComparaValorItemClase(Hashtable Datos, int Clase, string Item, string Formula, int NumItem)
        {
            if (Datos.Count>0)
            {
                if ((string)Datos["item"] != Item)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA ITEM EN CLASE " + Clase, "ITEMCLASE", (string)Datos["item"], Item, "MODIFICAR");
                    log.Log();
                }
                if ((string)Datos["formula"] != Formula)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA FORMULA EN ITEM " + (string)Datos["item"] +" CLASE " + Clase, "ITEMCLASE", (string)Datos["formula"], Formula, "MODIFICAR");
                    log.Log();
                }
                if ((int)Datos["numitem"] != NumItem)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA NUMERO ITEM EN ITEM " + (string)Datos["item"] + " CLASE " + Clase, "ITEMCLASE", (int)Datos["numitem"] + "", NumItem + "", "MODIFICAR");
                    log.Log();
                }
            }
        }

        #endregion

        #endregion

        #region "MANEJO DE DATOS CLASE"
        //INGRESO DE CLASE 
        //DATOS DE ENTRADA
        //CODIGO CLASE
        //DESCRIPCION
        private void fnIngresoClase(TextEdit pCod, TextEdit pdesc)
        {
            //SQL INSERT
            string sql = "INSERT INTO clase(codClase, descClase) VALUES(@pcod, @pdesc)";
            SqlCommand cmd;
            int res = 0;            

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pcod", pCod.Text));
                        cmd.Parameters.Add(new SqlParameter("@pdesc", pdesc.Text));

                        //EJECUTAR CONSULTA
                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            //INSERT CORRECTO
                            XtraMessageBox.Show("Clase guardada", "Guardar",MessageBoxButtons.OK, MessageBoxIcon.Information);

                            //GUARDAR EVENTO
                            logRegistro log = new logRegistro(User.getUser(), "NUEVO REGISTRO CLASE " + pCod.Text, "CLASE", "0", pCod.Text, "INGRESAR");
                            log.Log();

                            //CARGAR GRILLA
                            string grillaClase = "SELECT codClase, descClase FROM CLASE";
                            fnSistema.spllenaGridView(gridClase, grillaClase);
                            fnColumnasClase();
                            
                            //CARGAR CAMPO 0
                            CargarCamposClase(0);

                            opClase.Cancela = false;
                            opClase.SetButtonProperties(btnNuevaClase, 1);

                            if (opItemClase.Cancela)
                            {
                                opItemClase.Cancela = false;
                                opItemClase.SetButtonProperties(btnnuevoitem, 1);
                            }                                

                        }
                        else
                        {
                            XtraMessageBox.Show("Error al intentar guardar", "Guardar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    //LIBERAR RECURSOS
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                }
                else
                {
                    XtraMessageBox.Show("No hay conexion con la base de datos", "Base de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
           
        }

        //MODIFICAR INGRESO CLASE
        private void fnModClase(TextEdit pCod, TextEdit pdesc)
        {
            string sql = "UPDATE CLASE set descclase=@pdesc WHERE codclase=@pCod";
            SqlCommand cmd;
            int res = 0;

            //TABLA HASH PARA LOG
            Hashtable datosClase = new Hashtable();
            datosClase = PrecargaDatosClase(int.Parse(pCod.Text));

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pCod", pCod.Text));
                        cmd.Parameters.Add(new SqlParameter("@pdesc", pdesc.Text));

                        res = cmd.ExecuteNonQuery();

                        if (res > 0)
                        {
                            XtraMessageBox.Show("Clase Actualizada correctamente", "Actualizar", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            //SI CAMBIAN VALORES GUARDAMOS EVENTOS EN LOG
                            ComparaValorClase(datosClase, int.Parse(pCod.Text), pdesc.Text);
                            //LIMPIAMOS HASH
                            datosClase.Clear();

                            //CARGAR GRILLA
                            string grillaClase = "SELECT codClase, descClase FROM CLASE";
                            fnSistema.spllenaGridView(gridClase, grillaClase);
                            //CARGAR CAMPO 0
                            CargarCamposClase(0);

                            if (opItemClase.Cancela)
                            {
                                opItemClase.Cancela = false;
                                opItemClase.SetButtonProperties(btnnuevoitem, 1);
                            }
                        }
                        else
                        {
                            XtraMessageBox.Show("Error al actualizar clase", "Actualizar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }


                    }
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                }
                else
                {
                    XtraMessageBox.Show("No hay conexion con la base de datos");
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        //ELIMINAR CLASE
        private void fnEliminarClase(TextEdit pCod)
        {
            string sql = "DELETE FROM clase WHERE codClase=@pCod";
            SqlCommand cmd;
            int res = 0;           
         
                try
                {
                    if (fnSistema.ConectarSQLServer())
                    {
                        using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@pCod", pCod.Text));

                            res = cmd.ExecuteNonQuery();
                            if (res > 0)
                            {
                                XtraMessageBox.Show("Clase Eliminada correctamente", "Eliminar", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            //GUARDAR EVENTO EN LOG
                            logRegistro log = new logRegistro(User.getUser(), "SE HA ELIMINADO CLASE " + pCod.Text, "CLASE", pCod.Text, "0", "ELIMINAR");
                            log.Log();

                            //CARGAR GRILLA
                            string grillaClase = "SELECT codClase, descClase FROM CLASE";
                                fnSistema.spllenaGridView(gridClase, grillaClase);
                                //CARGAR CAMPO 0
                                CargarCamposClase(0);

                            //RECARGAR GRILLA ITEM CLASE
                            string grilla = "";
                            if (CodigoClase != 0)
                                grilla = string.Format("select clase, item, formula, numitem from itemClase INNER JOIN clase on clase.codClase = itemClase.clase WHERE itemclase.clase={0} ORDER BY numitem", CodigoClase);

                            fnSistema.spllenaGridView(griditemClase, grilla);
                            fnCargarCampos(0);

                           
                        }
                            else
                            {
                                XtraMessageBox.Show("Error al intentar eliminar clase", "Eliminar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }

                        }
                        //LIBERAR RECURSOS
                        cmd.Dispose();
                        fnSistema.sqlConn.Close();

                    }
                    else
                    {
                        XtraMessageBox.Show("No hay conexion con la base de datos", "Base de datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (SqlException ex)
                {
                    XtraMessageBox.Show(ex.Message);
                }

        }

        //COLUMNAS GRILLA CLASE
        private void fnColumnasClase()
        {            
            if (viewClase.RowCount > 0)
            {
                viewClase.Columns[0].Caption = "Codigo";
                viewClase.Columns[0].AppearanceCell.FontStyleDelta = FontStyle.Bold;
                viewClase.Columns[0].Width = 10;

                viewClase.Columns[1].Caption = "Descripcion";
                

            }
        }

        //DEFAULT PROPERTIES CLASE
        private void defaultClase()
        {
            txtcod.Properties.MaxLength = 4;
            txtdesc.Properties.MaxLength = 100;
            btnEliminarClase.TabStop = false;
            btnNuevaClase.TabStop = false;
            btnGuardarClase.TabStop = false;
            updateClase = false;
            gridClase.TabStop = false;
        }

        //CARGAR CAMPOS CLASE
        private void CargarCamposClase(int? pos = -1)
        {
            if (viewClase.RowCount>0)
            {
                if (pos != -1) viewClase.FocusedRowHandle = (int)pos;
                //DEJAR EN TRUE VARIABLE UPDATE CLASE
                updateClase = true;

                //DEJAR COMO SOLO LECTURA EL CODIGO
                txtcod.ReadOnly = true;
                txtcod.Text = viewClase.GetFocusedDataRow()["codClase"].ToString();              
                CodigoClase = int.Parse(viewClase.GetFocusedDataRow()["codClase"].ToString());
                txtdesc.Text = viewClase.GetFocusedDataRow()["descClase"].ToString();               
                groupItemClase.Text = "ITEM CLASE: " + fnDescripcionClase(CodigoClase);


                //RESET BOTON NUEVO               
            }
            else
            {
                fnLimpiarClase();
            }
        }

        //LIMPIAR CAMPOS CLASE
        private void fnLimpiarClase()
        {
            txtcod.ReadOnly = false;
            txtcod.Focus();
            txtcod.Text = "";
            txtdesc.Text = "";

            updateClase = false;
        }

        //VERIFICAR QUE EL CODIGO EXISTE EN TABLA CLASE
        private bool fnExisteCodClase(int pCod)
        {
            string sql = "SELECT codClase FROM clase WHERE codClase=@pCod";
            SqlCommand cmd;
            SqlDataReader rd;
            bool existe = false;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pCod", pCod));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //SI ENCUENTRA REGISTROS ES PORQUE EL CODIGO EXISTE
                            existe = true;
                        }
                        else
                        {
                            existe = false;
                        }
                    }
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                }
                else
                {
                    XtraMessageBox.Show("No hay conexion con la base de datos", "Base de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            //RETURN VALUE
            return existe;
        }

        //VERIFICAR SI LA CLASE A ELIMINAR TIENE ITEM ASOCIADOS EN TABLA ITEMCLASE
        private bool fnClaseTieneItems(int pCod)
        {
            string sql = "select clase from clase INNER JOIN itemClase on clase.codClase=itemClase.clase WHERE clase=@pCod";
            SqlCommand cmd;
            SqlDataReader rd;
            bool existe = false;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pCod", pCod));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //EXISTEN DATOS EN TABLA ITEMCLASE RELACIONADOS CON EL CODIGO DE CLASE
                            existe = true;
                        }
                        else
                        {
                            existe = false;
                        }
                    }
                    //LIBERAR RECURSOS
                    cmd.Dispose();
                    rd.Close();
                    fnSistema.sqlConn.Close();
                }
                else
                {
                    XtraMessageBox.Show("No hay conexion con la base de datos", "Base de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
            return existe;
        }

        //ELIMINAR ITEM ASOCIADOS A CLASE
        //DEVUELVE TRUE SI ELIMINO CORRECTAMENTE
        private bool fnEliminarTodoItemClase(int pCod)
        {
            string sql = "DELETE itemclase FROM itemClase " +
            "INNER JOIN clase on clase.codClase = itemClase.clase" + 
            " WHERE itemClase.clase=@pCod";

            SqlCommand cmd;
            int res = 0;
            bool eliminado = false;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS                   
                        cmd.Parameters.Add(new SqlParameter("@pCod", pCod));

                        //EJECUTAMOS CONSULTA
                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            //elimino correctamente, retornamos true
                            eliminado = true;
                            //GUARDAMOS EN REGISTRO LOG
                            logRegistro log = new logRegistro(User.getUser(), "SE ELIMINA REGISTROS CODIGO CLASE " + pCod, "ITEMCLASE", pCod + "", "0", "ELIMINAR");
                            log.Log();
                        }
                        else
                        {
                            eliminado = false;
                        }
                    }
                }
                else
                {
                    XtraMessageBox.Show("No hay conexion con la base de datos", "Base de datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
            return eliminado;
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Tab)
            {
                if (txtcod.ContainsFocus)
                {
                    if (updateClase == false)
                    {
                        //ES INSERT NUEVO
                        if (txtcod.Text == "")
                        {
                            lblclase.Visible = true;
                            lblclase.Text = "Debes ingresar un codigo para la clase";
                            return false;
                        }
                        else
                        {
                            //VERIFICAR SI EL CODIGO INGRESAR YA EXISTE
                            bool codigo = fnExisteCodClase(int.Parse(txtcod.Text));
                            if (codigo)
                            {
                                lblclase.Visible = true;
                                lblclase.Text = "Codigo ingresado ya existe";
                                return false;
                            }
                            else
                            {
                                lblclase.Visible = false;
                            }
                        }
                    }
                }
                else if (txtnumitem.ContainsFocus)
                {
                    if (updateItemClase == false)
                    {
                        if (txtnumitem.Text == "")
                        {
                            lblerror.Visible = true;
                            lblerror.Text = "Numero de item no valido";
                            return false;
                        }
                        else
                        {
                            lblerror.Visible = false;
                            //VERIFICAR QUE NUMERO DE ITEM NO EXISTE 
                            bool existe = fnExisteItemIngresar(int.Parse(txtnumitem.Text), CodigoClase);
                            if (existe)
                            {
                                //NO ES VALIDO PORQUE YA EXISTE
                                lblerror.Visible = true;
                                lblerror.Text = "El numero ingresado ya existe para esta clase";
                                return false;
                            }
                            else
                            {
                                lblerror.Visible = false;
                            }
                        }
                    }
                }
                else if (txtdesc.ContainsFocus)
                {
                    if (txtdesc.Text == "")
                    {
                        lblclase.Visible = true;
                        lblclase.Text = "Ingrese descripcion";
                        return false;
                    }
                    else
                    {
                        lblclase.Visible = false;
                    }
                }

                
            }

            return base.ProcessDialogKey(keyData);
        }

        //VERIFICAR SI EL CODIGO DE CLASE ESTA ASOCIADO A UN TRABAJADOR
        private bool fnClaseAsociadoTrabajador(int pCod)
        {
            string sql = " SELECT clase FROM trabajador " + 
                        "INNER JOIN clase ON clase.codClase = trabajador.clase " +
                        "WHERE trabajador.clase = @pCod";
            SqlCommand cmd;
            SqlDataReader rd;
            bool existe = false;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pCod", pCod));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //SI TIENE FILAS ES PORQUE HAY RELACION
                            existe = true;
                        }
                        else
                        {
                            existe = false;
                        }

                    }
                    //LIBERAR RECURSOS
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                    rd.Close();

                }
                else
                {
                    XtraMessageBox.Show("No hay conexion con la base de datos", "Base de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return existe;
        }

        //OTRA FORMA DE VERIFICAR SI LA CLASE ESTA SIENDO USADA POR UN TRABAJADOR
        private bool TrabUsaClase(int pCod)
        {
            string sql = "SELECT contrato FROM trabajador WHERE clase=@code";
            SqlCommand cmd;
            SqlDataReader rd;
            bool usado = false;

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@code", pCod));
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //SI HAY REGISTROS ES PORQUE LA CLASE ESTA ASOCIADA A UN TRABAJADOR
                            usado = true;
                        }
                        else
                        {
                            usado = false;
                        }
                    }
                    cmd.Dispose();
                    rd.Close();
                    fnSistema.sqlConn.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return usado;
        }

        //INGRESO ITEMTRABAJADOR DE ACUERDO A SELECCION EN COMBOBOX CLASE
        private bool fnCargarClasesTrabajador(int pClase, string contrato, string rut)
        {

            /*string sql = "INSERT INTO itemtrabajador(rut, contrato, anomes, coditem, formula, tipo, orden) " +
                "SELECT rut, contrato, anomes, clase.item, clase.formula, tipo, orden " +
                "FROM trabajador inner JOIN clase on clase.codClase = trabajador.clase " +
                "INNER JOIN item on item.coditem = clase.item AND rut=@prut AND contrato=@pcontrato " +
                "AND clase.codClase=@pClase";*/

            string sql = "INSERT INTO itemtrabajador(rut, contrato, anomes, coditem, formula, tipo, orden) " +
                " SELECT rut, contrato, anomes, itemClase.item, itemClase.formula, tipo, orden " +
                " FROM trabajador inner JOIN clase on clase.codClase = trabajador.clase " +
                " INNER JOIN itemclase on itemClase.clase = clase.codClase " +
                " INNER JOIN item on item.coditem = itemClase.item " +
                " WHERE contrato = @pcontrato AND rut = @prut AND clase.codClase = @pClase " +
                " EXCEPT " +
                " SELECT itemTrabajador.rut, itemTrabajador.contrato, trabajador.anomes," +
                " itemTrabajador.coditem, itemTrabajador.formula, itemTrabajador.tipo, itemtrabajador.orden " +
                " FROM itemTrabajador INNER JOIN trabajador on trabajador.contrato = itemTrabajador.contrato " +
                " INNER JOIN clase on clase.codClase = trabajador.clase " +
                " INNER JOIN itemClase on itemClase.clase = clase.codClase " +
                " WHERE itemTrabajador.contrato = @pcontrato AND itemtrabajador.rut = @prut AND clase.codClase = @pClase";

            SqlCommand cmd;
            int res = 0;
            bool cargacorrecta = false;
            //ELIMINAR VALORES ACTUALES DE LA TABLA ITEMTRABAJADOR Y REEMPLAZAR POR LO NUEVOS
            //SI ENCUTRA CONCORDANCEAS ELIMNADA REGISTROS.
            //SI NO ENCUENTRA CONCORDANCIAS SIMPLEMENTE NO ELIMINADA
            //fnElimnarFilasTrabajador(rut, contrato, pClase);

            //----------------------
            //INSERT SQL            |
            //----------------------

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@prut", rut));
                        cmd.Parameters.Add(new SqlParameter("@pcontrato", contrato));
                        cmd.Parameters.Add(new SqlParameter("@pClase", pClase));

                        //EJECUTAR CONSULTA
                        res = cmd.ExecuteNonQuery();
                        //SI RES SIGUE SIENDO CERO ES PORQUE NO FUE NECESARIO INGRESAR LAS FILAS NUEVAMENTE
                        //YA QUE LOS REGISTROS YA ESTABAN
                        if (res > 0)
                        {
                            //INGRESO CORRECTO
                            //XtraMessageBox.Show("Carga de datos correcta!");
                            cargacorrecta = true;
                        }
                        else
                        {
                            //ERROR AL INGRESAR
                            //XtraMessageBox.Show("Error al cargar datos");
                            cargacorrecta = false;
                        }
                    }
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();

                }
                else
                {
                    XtraMessageBox.Show("No hay conexion con la base de datos", "Base de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
            return cargacorrecta;
        }

        //ELIMINAR CLASE USANDO TRANSACCION
        private bool EliminarClaseTransaccion(int pCod)
        {
            //PARA VERFICAR SI LA CLASE ESTA SIENDO USADA POR UN TRABAJADOR
            bool usaTrabajador = false;
            bool tranCorrecta = false, tieneItem = false;

            //PREGUNTAMOS SI EXISTE ALGUN REGISTROS EN LE PERIODO QUE SEA QUE ESTE HACIENDO USO DE ESTA CLASE
            //SI RETORNA TRUE NO PODEMOS ELIMINAR ESTA CLASE (INCONSISTENCIA EN BD)
            usaTrabajador = TrabUsaClase(pCod);

            if (usaTrabajador)
            { XtraMessageBox.Show("Esta clase no se puede eliminar porque esta siendo usada por un trabajador", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);return tranCorrecta; }

            tieneItem = fnClaseTieneItems(pCod);

            SqlCommand cmd;
            SqlTransaction tran;

            //SQL PARA ELIMINAR ITEM ASOCIADOS A CLASE
            string sqlItemClase = "DELETE FROM ITEMCLASE WHERE clase=@code";

            //SQL PARA ELIMINAR CLASE
            string sqlClase = "DELETE FROM CLASE WHERE codclase=@code";

            string pregunta = "";
            if (tieneItem)
                pregunta = "Esta clase tiene items asociados, ¿Desea eliminar de todas maneras?";
            else
                pregunta = "¿Realmente desea eliminar esta clase?";

            DialogResult advertencia = XtraMessageBox.Show(pregunta, "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (advertencia == DialogResult.Yes)
            {
                try
                {
                    if (fnSistema.ConectarSQLServer())
                    {
                        tran = fnSistema.sqlConn.BeginTransaction();
                        try
                        {
                            //SI TIENE ITEMS ELIMINAMOS ITEM PRIMERAMENTE
                            if (tieneItem)
                            {
                                cmd = new SqlCommand(sqlItemClase, fnSistema.sqlConn);

                                //PARAMETROS
                                cmd.Parameters.Add(new SqlParameter("@code", pCod));

                                cmd.Transaction = tran;

                                //EJECUTAMOS QUERY
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();
                            }

                            //ELIMINAMOS CLASE
                            using (cmd = new SqlCommand(sqlClase, fnSistema.sqlConn))
                            {
                                //PARAMETROS
                                cmd.Parameters.Add(new SqlParameter("@code", pCod));

                                cmd.Transaction = tran;

                                cmd.ExecuteNonQuery();
                                cmd.Dispose();
                            }

                            //LLEGADO A ESTE PUNTO GUARDAMOS LOS CAMBIOS CON COMMIT
                            tran.Commit();
                            fnSistema.sqlConn.Close();

                            tranCorrecta = true;
                        }
                        catch (Exception ex)
                        {
                            //SI SE PRODUCE ALGUNA EXCEPCION DESHACEMOS LOS CAMBIOS
                            tranCorrecta = false;
                            tran.Rollback();
                        }                        
                    }
                }
                catch (SqlException ex)
                {
                    XtraMessageBox.Show(ex.Message);
                }
            }            

            return tranCorrecta;
        }

        //VERIFICAR SI HAY CAMBIOS SIN GUARDAR
        private bool CambiosSinguardarClase(int clase)
        {
            string sql = "SELECT descclase FROM clase WHERE codClase=@pClase";
            SqlCommand cmd;
            SqlDataReader rd;

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pClase", clase));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //COMPARAMOS
                                if (txtdesc.Text != (string)rd["descclase"]) return true;
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
            return false;
        }

        #region "MANEJO LOG CLASE"
        //TABLA HASH PARA PRECARGA DE DATOS        
        private Hashtable PrecargaDatosClase(int pCodClase)
        {
            Hashtable datos = new Hashtable();
            string sql = "SELECT codclase, descclase FROM clase WHERE codclase = @pcode";
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pcode", pCodClase));
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //AGREGAMOS DATOS A TABLA HASH
                                datos.Add("codclase", (int)rd["codclase"]);
                                datos.Add("descclase", (string)rd["descclase"]);
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

            return datos;
        }

        //COMPARA VALORES
        private void ComparaValorClase(Hashtable Datos, int pCodClase, string pDesc)
        {
            if (Datos.Count>0)
            {
                if ((string)Datos["descclase"] != pDesc)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA DESCRIPCION CLASE " + pCodClase, "CLASE", (string)Datos["descclase"], pDesc, "MODIFICAR");
                    log.Log();
                }
            }
        }

        #endregion

        #endregion

        #region "MANIPULACION NUMERO ITEM"
        //CONSULTAR POR EL ULTIMO ITEM INGRESADO
        private int fnUltimoNumero(int pClase)
        {
            string sql = "select max(numitem) as maximo FROM itemClase WHERE itemClase.clase=@pClase";
            SqlCommand cmd;
            SqlDataReader rd;
            int num = 0;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        cmd.Parameters.Add(new SqlParameter("@pClase", pClase));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //RECORREMOS SI ENCONTRO REGISTROS
                            while (rd.Read())
                            {
                                num = (int)rd["maximo"];
                            }
                        }
                        else
                        {
                            //SI NO ENCONTRO REGISTROS RETORNAMOS -1 QUE SIGNIFICA QUE NO HAY NUMEROS INGRESADOS
                            num = -1;
                        }

                    }
                }
                else
                {
                    XtraMessageBox.Show("No hay conexion con la base de datos", "Base de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return num;
        }

        //TRAER TODOS LOS NUMEROS DE ITEM DESDE LA BASE DE DATOS
        private int[] fnAllitem()
        {
            int total = 0;
            total = fnCantidad();
            int[] numeros = new int[total];

            int posicion = 0;

            //sql consulta
            string sql = "";
            if(CodigoClase != 0)
                 sql = string.Format("select numitem FROM itemclase WHERE clase = {0}", CodigoClase);

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
                                //GUARDAR DATOS EN ARREGLO...
                                numeros[posicion] = (int)rd["numitem"];
                                posicion++;
                            }
                        }
                    }
                    //LIBERAR RECURSOS
                    cmd.Dispose();
                    rd.Close();
                    fnSistema.sqlConn.Close();
                }
                else
                {
                    XtraMessageBox.Show("No hay conexion con la base de datos", "Base de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
            return numeros;

        }

        //OBTENER LA CANTIDAD DE ELEMENTOS 
        private int fnCantidad()
        {
            string sql = "";

            if(CodigoClase != 0)
            sql = string.Format("select count(*) as cantidad FROM itemclase WHERE clase={0}", CodigoClase);

            SqlCommand cmd;
            SqlConnection cn;

            int total = 0;
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
                                total = Convert.ToInt32(data);

                        }
                        //LIBERAR RECURSOS
                        cmd.Dispose();                        
                    }
                }            
                else
                {
                    XtraMessageBox.Show("No hay conexion con la base de datos", "Base de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
            return total;

        }

        //OBTENER LA CANTIDAD DE ITEM DISPONIBLES DENTRO DE UN RANGO DE NUMEROS
        //ej: entre 3 y 7 hay 3 elementos disponibles {4,5,6}
        private int fnItemDisponibles()
        {
            int[] elementos = fnAllitem();
            int num1 = 0, num2 = 0;
            int generado = 0;
            int devuelve = 0;

            if (elementos.Length > 1)
            {
                //ORDENAR DE FORMA ASCENDENTE LOS NUMEROS DE ITEMS
                Array.Sort(elementos);
                for (int pos = 0; pos < elementos.Length - 1; pos++)
                {
                    num1 = elementos[pos];
                    num2 = elementos[pos + 1];
                    if ((num2 - num1) > 1)
                    {
                        //HAY ELEMENTOS QUE NO ESTAN EN USO...
                        generado = num1 + 1;
                    }
                }

                //SI GENERADO SIGUE SIENDO 0 A PESAR DE RECORRER TODO EL ARREGLO
                //ES PORQUE NO HAY DIGITOS ENTRE MEDIO DISPONIBLES
                if (generado == 0)
                {
                    //RETORNAMOS EL ULTIMO ELEMENTO INGRESADO EN BD + 1
                    if (CodigoClase != 0)
                        devuelve = fnUltimoNumero(CodigoClase) + 1;
                }
                else
                {
                    //SI ES DISTINTO DE CERO RETORNAMOS EL NUMERO GENERADO
                    devuelve = generado;
                }

            }
            else if (elementos.Length == 1)
            {
                //SI SOLO TIENE UN ELEMENTO...
                num1 = elementos[0];
                if (num1 == 1)
                {
                    devuelve = num1 + 1;
                }
                else if (num1 > 1 && num1 != 0)
                {
                    devuelve = num1 - 1;
                }
            }
            else
            {
                //SI NO TIENE ELEMENTOS RETORNA UN 1
                devuelve = 1;
            }


            return devuelve;
        }

        //VER DI HAY DATOS EN TABLA CLASE
        private bool HayDatosClase()
        {
            bool tiene = false;
            string sql = "SELECT count(*) FROM clase ";
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
                            if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                                tiene = true;
                            
                        }
                        cmd.Dispose();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
            

            return tiene;
        }

        #endregion
        private void txtitem_EditValueChanged(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD EN SESION
            Sesion.NuevaActividad();

            string formula = "";
            string seleccion = txtitem.EditValue.ToString();
            string sql = string.Format("SELECT formula, descFormula FROM formula JOIN item " +
                "on formula.codFormula = item.formula WHERE coditem='{0}'", seleccion);

            //CARGAR COMBO FORMULA
            fnComboFormula(sql, txtformula, "formula", "descFormula", true);
            if (viewitemClase.RowCount > 0)
            {
                formula = viewitemClase.GetFocusedDataRow()["formula"].ToString();
                if (formula == "0")
                {
                    txtformula.EditValue = 0;
                }                    
            }                
        }

        private void txtnumitem_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void txtdesc_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsLetter(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (char.IsSeparator(e.KeyChar))
            {
                e.Handled = false;
            }
        }

        private void txtcod_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            if (opClase.Cancela == false)
            {
                opClase.Cancela = true;
                opClase.SetButtonProperties(btnNuevaClase, 2);
                fnLimpiarClase();
                txtcod.Properties.Appearance.BackColor = Color.LightGoldenrodYellow;

                //BOTON NUEVO ITEM CLASE    
                if(opItemClase.Cancela)
                    opItemClase.Cancela = false;

                opItemClase.SetButtonProperties(btnnuevoitem, 1);
                fnCargarCampos(0);
            }
            else
            {
                opClase.Cancela = false;
                opClase.SetButtonProperties(btnNuevaClase, 1);

                //CARGAR CAMPOS
                //GUARDAR LA FILA SELECCIONADA DE LA GRILLA
                FilaActual = viewClase.FocusedRowHandle;
                CargarCamposClase();

                //CAMBIAR LABEL TITULO DE ACUERDO A CLASE SELECCIONADA
                if (CodigoClase != 0)
                {
                    groupItemClase.Text = "ITEM CLASE: " + fnDescripcionClase(CodigoClase);
                    //RECARGAR GRILLA DE ACUERDO A CODIGO SELECCIONADO
                    string grilla = "";
                    grilla = string.Format("SELECT clase, item, formula, numitem from itemClase INNER JOIN clase on clase.codClase = itemClase.clase WHERE itemclase.clase={0} ORDER BY numitem", CodigoClase);
                    fnSistema.spllenaGridView(griditemClase, grilla);

                    updateItemClase = false;

                    fnCargarCampos(0);
                    opItemClase.SetButtonProperties(btnnuevoitem, 1);
                }
                else
                {
                    groupItemClase.Text = "NO SE HA SELECCIONADO CLASE";
                }

                       
            }
            
        
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            //NUEVA SESION
            Sesion.NuevaActividad();

            if (txtcod.Text == "") { lblclase.Visible = true; lblclase.Text = "Debe ingresar un codigo"; return; }
            if (txtdesc.Text == "") { lblclase.Visible = true; lblclase.Text = "Debe ingresar descripcion"; return; }            

            bool existe = false;
            lblclase.Visible = false;
            int code = 0;
           
            if (updateClase)
            {
                //ES ACTUALIZAR
                
                if (viewClase.RowCount > 0)
                {
                    code = int.Parse(viewClase.GetFocusedDataRow()["codClase"].ToString());
                    existe = fnExisteCodClase(code);
                    //VERIFICAR QUE EL CODIGO EXISTA                                   
                    if (existe)
                    {
                        lblclase.Visible = false;
                        //SI EL REGISTRO EXISTE HACEMOS UPDATE
                        fnModClase(txtcod, txtdesc);
                    }
                    else
                    {
                        lblclase.Visible = true;
                        lblclase.Text = "Codigo no valido";
                        return;
                    }
                }     
            }
            else
            {
                //ES INSERT

                //VERIFICAR QUE EL REGISTRO A INGRESAR NO EXISTE EN BD
                code = int.Parse(txtcod.Text);
                existe = fnExisteCodClase(code);
                if (existe)
                {
                    //SI EXISTE MOSTRAMOS MENSAJE DE ADVERTENCIA
                    lblclase.Visible = true;
                    lblclase.Text = "El codigo ingresado ya existe";
                    return;
                }
                else
                {
                    
                    //ES VALIDO
                    lblclase.Visible = false;
                    fnIngresoClase(txtcod, txtdesc);
                }
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            //NUEVA SESION
            Sesion.NuevaActividad();

            if (txtcod.Text == "") { lblclase.Visible = true; lblclase.Text = "clase no valida";return;}

            lblclase.Visible = false;

            bool existe = false, correcto = false;
            //bool tieneItem = false;
            //bool AsociadoTrabajador = false;
            int code = 0;

            //VERIFICAR QUE EL REGISTRO QUE SE INTENTA ELIMINAR EXISTA REALMENTE EN BD
            existe = fnExisteCodClase(int.Parse(txtcod.Text));

            if (existe)
            {
                lblclase.Visible = false;
              
                if (viewClase.RowCount > 0)
                    code = int.Parse(viewClase.GetFocusedDataRow()["codclase"].ToString());

                correcto = EliminarClaseTransaccion(code);
                if (correcto)
                {
                    XtraMessageBox.Show("Clase eliminada correctamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //GUARDAR EVENTO EN LOG
                    logRegistro log = new logRegistro(User.getUser(), "SE HA ELIMINADO CLASE " + code, "CLASE", code.ToString(), "0", "ELIMINAR");
                    log.Log();

                    //CARGAR GRILLA
                    string grillaClase = "SELECT codClase, descClase FROM CLASE";
                    fnSistema.spllenaGridView(gridClase, grillaClase);
                    //CARGAR CAMPO 0
                    CargarCamposClase(0);

                    //RECARGAR GRILLA ITEM CLASE
                    string grilla = "";
                    if (CodigoClase != 0)
                        grilla = string.Format("select clase, item, formula, numitem from itemClase INNER JOIN clase on clase.codClase = itemClase.clase WHERE itemclase.clase={0} ORDER BY numitem", CodigoClase);

                    fnSistema.spllenaGridView(griditemClase, grilla);
                    fnCargarCampos(0);

                    btnNuevaClase.Text = "Nuevo";
                    btnNuevaClase.ToolTip = "Nuevo registro";
                    CancelaClase = true;
                }
                else
                { XtraMessageBox.Show("Error al intentar eliminar clase", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); }

                //VERIFICAR SI LA CLASE ESTA SOCIADA A ALGUN TRABAJADOR (CODIGO DE LA CLASE)
                /*AsociadoTrabajador = fnClaseAsociadoTrabajador(code);
                if (AsociadoTrabajador)
                {
                    //ES PORQUE ESTA CLASE ESTA ASOCIADA A UN ITEMTRABAJADOR
                    XtraMessageBox.Show("Esta clase no se puede eliminar porque esta siendo usada por un trabajador", "Denegado", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                else
                {
                    //VERIFICAR SI LA CLASE TIENE ITEMS (ITEMS DE CLASE)
                    tieneItem = fnClaseTieneItems(code);
                    if (tieneItem)
                    {
                        //SI TIENE ITEM ASOCIADOS DEBEMOS ADVERTIR
                        DialogResult ad = XtraMessageBox.Show("Esta clase tiene registros Asociados, ¿Realmente desea eliminar?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (ad == DialogResult.Yes)
                        {
                            //ELIMINAMOS TODOS LOS DATOS REALACIONADOS
                            bool itemEliminado = fnEliminarTodoItemClase(int.Parse(txtcod.Text));
                            if (itemEliminado)
                            {
                                //ELIMINAMOS FINALMENTE LA CLASE DE TABLA CLASE
                                fnEliminarClase(txtcod);
                            }
                        }
                    }
                    else
                    {
                        //SI NO TIENE ITEM ASOCIADO SOLO ELIMINAMOS SIN PROBLEMAS
                        DialogResult adv = XtraMessageBox.Show("¿Realmente desea eliminar?", "Eliminar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (adv == DialogResult.Yes)
                        {
                            fnEliminarClase(txtcod);
                        }
                    }
                }        */

            }
            else
            {
                lblclase.Visible = false;
                lblclase.Text = "Registro no valido";
                return;
            }            
        }

        private void gridClase_Click(object sender, EventArgs e)
        {
            //NUEVA SESION
            Sesion.NuevaActividad();

            //GUARDAR LA FILA SELECCIONADA DE LA GRILLA
            FilaActual = viewClase.FocusedRowHandle;
            CargarCamposClase();

            //CAMBIAR LABEL TITULO DE ACUERDO A CLASE SELECCIONADA
            if (CodigoClase != 0)
            {
                groupItemClase.Text = "ITEM CLASE: " + fnDescripcionClase(CodigoClase);
                groupItemClase.Text = "ITEM CLASE: " + fnDescripcionClase(CodigoClase);
                //RECARGAR GRILLA DE ACUERDO A CODIGO SELECCIONADO
                string grilla = "";
                grilla = string.Format("SELECT clase, item, formula, numitem from itemClase INNER JOIN clase on clase.codClase = itemClase.clase WHERE itemclase.clase={0} ORDER BY numitem", CodigoClase);
                fnSistema.spllenaGridView(griditemClase, grilla);

                updateItemClase = false;

                fnCargarCampos(0);
           
                opClase.Cancela = false;
                opClase.SetButtonProperties(btnNuevaClase, 1);

                //BOTON NUEVO ITEMCLASE
                opItemClase.Cancela = false;
                opItemClase.SetButtonProperties(btnnuevoitem, 1);
                

            }
            else
            {
                groupItemClase.Text = "NO SE HA SELECCIONADO CLASE";
            }
        }

        private void gridClase_ProcessGridKey(object sender, KeyEventArgs e)
        {
           
        }

        private void gridClase_KeyUp(object sender, KeyEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            CargarCamposClase();

            //CAMBIAR LABEL TITULO DE ACUERDO A CLASE SELECCIONADA
            if (CodigoClase != 0)
            {
                groupItemClase.Text = "ITEM CLASE: " + fnDescripcionClase(CodigoClase);
                //RECARGAR GRILLA DE ACUERDO A CODIGO SELECCIONADO
                string grilla = "";
                grilla = string.Format("SELECT clase, item, formula, numitem from itemClase INNER JOIN clase on clase.codClase = itemClase.clase WHERE itemclase.clase={0} ORDER BY numitem", CodigoClase);
                fnSistema.spllenaGridView(griditemClase, grilla);

                updateItemClase = false;

                fnCargarCampos(0);
            }
            else
            {
                groupItemClase.Text = "NO SE HA SELECCIONADO CLASE";
            }
        }

        private void txtformula_EditValueChanged(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();
        }

        private void btnnuevoitem_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            if (opItemClase.Cancela == false)
            {
                opItemClase.Cancela = true;
                opItemClase.SetButtonProperties(btnnuevoitem, 2);
                fnlimpiarCampos();

                //CARGAR CAMPOS
                CargarCamposClase(viewClase.FocusedRowHandle);
                txtcod.Properties.Appearance.BackColor = Color.White;
                lblclase.Visible = false;

                if (opClase.Cancela)
                    opClase.Cancela = false;

                opClase.SetButtonProperties(btnNuevaClase, 1);
            }
            else
            {
                opItemClase.Cancela = false;
                opClase.SetButtonProperties(btnnuevoitem, 1);
                fnCargarCampos(0);               
                
            }            
        }

        private void btnguardarItemClase_Click(object sender, EventArgs e)
        {
            //NUEVA SESION ACTIVIDAD
            Sesion.NuevaActividad();

            if (txtitem.EditValue.ToString() == "") { lblerror.Visible = true;lblerror.Text = "Seleccione un item";return;}
            if (txtformula.EditValue.ToString() == "") { lblerror.Visible = true; lblerror.Text = "Seleccione una formula"; return; }
            if (txtnumitem.Text == "") { lblerror.Visible = true; lblerror.Text = "Numero item ya existe";return;}

            string item = "";
            int numero = 0;
            bool existeCodigo = false;
            bool tranCorrecta = false;

            if (updateItemClase)
            {
                if (viewitemClase.RowCount > 0)
                {
                    item = viewitemClase.GetFocusedDataRow()["item"].ToString();
                    numero = int.Parse(viewitemClase.GetFocusedDataRow()["numitem"].ToString());
                }

                //VERIFICAR QUE SI EL ITEM ES DISTINTO MANDAMOS ALERTA
                if (item != txtitem.EditValue.ToString())
                {
                    DialogResult advertencia = XtraMessageBox.Show("Estas a punto de cambiar el item " + item + ", ¿Realmente quieres realizar esta operación?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (advertencia == DialogResult.Yes)
                    {
                        //VERIFCAR QUE NO SEA UN CODIGO DE ITEM YA EXISTENTE...
                        if (ExisteCodItem((string)txtitem.EditValue, CodigoClase))
                        { XtraMessageBox.Show("Ya existe un registro con ese codigo", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);txtitem.Focus();return;}

                        //SI ES TRUE ES ACTUALIZACION
                        //fnModificaritemClase(txtitem, txtformula, txtnumitem, CodigoClase, item, numero);
                        tranCorrecta = ModificarItemClase(txtitem, txtformula, txtnumitem, CodigoClase, item, numero);
                        if (tranCorrecta) { XtraMessageBox.Show("Actualizacion correcta", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); }
                        else
                        { XtraMessageBox.Show("Error al intentar actualizar", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);return; }
                    }
                }
                else
                {
                    //SOLO MODIFICAMOS
                    //fnModificaritemClase(txtitem, txtformula, txtnumitem, CodigoClase, item, numero);
                    tranCorrecta = ModificarItemClase(txtitem, txtformula, txtnumitem, CodigoClase, item, numero);
                    if (tranCorrecta) { XtraMessageBox.Show("Actualizacion correcta", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); }
                    else
                    { XtraMessageBox.Show("Error al intentar actualizar", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
                }                
            }
            else
            {
                //ES INSERT
                if (CodigoClase != 0)
                {
                    //VERIFICAR QUE NUMERO DE ITEM A INGRESAR NO EXISTA
                    bool existeNum = fnExisteItemIngresar(int.Parse(txtnumitem.Text), CodigoClase);
                    if (existeNum)
                    {
                        lblerror.Visible = true;
                        lblerror.Text = "El numero de item que intentas ingresar ya existe en esta clase";
                        return;
                    }
                    else
                    {
                        lblerror.Visible = false;
                        //VERIFICAR SI EL CODIGO DE ITEM YA EXISTE PARA ESA CLASE...
                        item = (string)txtitem.EditValue;

                        existeCodigo = ExisteCodItem(item, CodigoClase);

                        if (existeCodigo)
                        { XtraMessageBox.Show("Ya existe un registro con ese codigo", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); txtcod.Focus(); return; }

                        //SOLO INGRESAMOS NUEVO CODIGO ITEM
                        // fnNuevaitemClase(CodigoClase, txtitem, txtformula, txtnumitem);
                        tranCorrecta = NuevaItemClase(CodigoClase, txtitem, txtformula, txtnumitem);
                        if (tranCorrecta)
                        { XtraMessageBox.Show("Nuevo item ingresado correctamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); }
                        else
                        { XtraMessageBox.Show("Error al intentar ingresar item", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);return;}
                    }                    
                }
                else
                {
                    lblerror.Visible = true;
                    lblerror.Text = "Clase no es valida";
                }                
            }
        }

        private void btnEliminaritemClase_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            int num = 0;
            string item = "";
            bool correcto = false;
            if (CodigoClase != 0)
            {                
                if (viewitemClase.RowCount > 0)
                {                    
                    item = viewitemClase.GetFocusedDataRow()["item"].ToString();
                    num = Convert.ToInt32(viewitemClase.GetFocusedDataRow()["numitem"]);

                    //LLAMAMOS AL METODO QUE ELIMINA ITEM CLASE
                   correcto = EliminarItemClaseTransaccion(item, CodigoClase, num);
                    if (correcto)
                    {
                        XtraMessageBox.Show("Item de clase eliminado correctamente", "Informacion",  MessageBoxButtons.OK, MessageBoxIcon.Information);
                        logRegistro log = new logRegistro(User.getUser(), "SE ELIMINA ITEM " + item + " Numero " + num + " DESDE CLASE " + CodigoClase, "ITEMCLASE", item, "0", "ELIMINAR");
                        log.Log();

                        //RECARGAR GRILLA
                        string grilla = "";
                        if (CodigoClase != 0)
                            grilla = string.Format("select clase, item, formula, numitem from itemClase INNER JOIN clase on clase.codClase = itemClase.clase WHERE itemclase.clase={0} ORDER BY numitem", CodigoClase);

                        fnSistema.spllenaGridView(griditemClase, grilla);
                        fnCargarCampos(0);

                        CancelaItemClase = true;
                        btnnuevoitem.Text = "Nuevo";
                        btnnuevoitem.ToolTip = "Nuevo registro";
                    }
                    else
                    {
                        XtraMessageBox.Show("Se ha producido un error al intentar eliminar registro", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    
                    /*bool AsociadoTrabajador = fnVerificarLigado(item, CodigoClase);
                    //SI RETORNA TRUE ES PORQUE EL ITEM ESTA ASOCIADO A ITEM TRABAJADOR
                    if (AsociadoTrabajador)
                    {
                        DialogResult adver = XtraMessageBox.Show("Este item esta asociado a un item trabajador, ¿Desea eliminar de todas maneras?", "Advertencia" ,MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (adver == DialogResult.Yes)
                        {
                            //ELIMINAMOS REGISTRO DESDE TABLA ITEMTRABAJADOR
                            bool eliminado = fnEliminarItemTrabajador(item, CodigoClase);
                            if (eliminado)
                            {
                                //PODEMOS ELIMINAR SIN PROBLEMAS EL ITEM DESDE TABLA ITEMTABLA
                                num = int.Parse(viewitemClase.GetFocusedDataRow()["numitem"].ToString());
                                fnEliminaritemClase(CodigoClase, item, num);
                            }
                        }
                    }
                    else
                    {
                        //SI NO ESTA ASOCIADO SOLO ELIMINAMOS
                        DialogResult ad = XtraMessageBox.Show("¿Desea realmente eliminar?", "Eliminar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (ad == DialogResult.Yes)
                        {
                            item = viewitemClase.GetFocusedDataRow()["item"].ToString();
                            num = int.Parse(viewitemClase.GetFocusedDataRow()["numitem"].ToString());
                            fnEliminaritemClase(CodigoClase, item, num);
                        }
                    }*/

                }
                else
                {
                    XtraMessageBox.Show("Item no valido", "Item", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void btnNuevaItemClase_Click(object sender, EventArgs e)
        {
            fnCargarCampos();
        }

        private void btnNuevaItemClase_KeyUp(object sender, KeyEventArgs e)
        {
            fnCargarCampos();
        }

        private void txtnumitem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (updateItemClase == false)
                {
                    if (txtnumitem.Text == "")
                    {
                        lblerror.Visible = true;
                        lblerror.Text = "Numero de item no valido";
                        return;
                    }
                    else
                    {
                        lblerror.Visible = false;
                        //VERIFICAR QUE NUMERO DE ITEM NO EXISTE 
                        bool existe = fnExisteItemIngresar(int.Parse(txtnumitem.Text), CodigoClase);
                        if (existe)
                        {
                            //NO ES VALIDO PORQUE YA EXISTE
                            lblerror.Visible = true;
                            lblerror.Text = "El numero ingresado ya existe para esta clase";
                            return;
                        }
                        else
                        {
                            lblerror.Visible = false;
                        }
                    }
                }              
            }           
        }

        private void txtcod_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (updateClase == false)
                {
                    //ES INSERT NUEVO
                    if (txtcod.Text == "")
                    {
                        lblclase.Visible = true;
                        lblclase.Text = "Debes ingresar un codigo para la clase";
                        return;
                    }
                    else
                    {
                        //VERIFICAR SI EL CODIGO INGRESAR YA EXISTE
                        bool codigo = fnExisteCodClase(int.Parse(txtcod.Text));
                        if (codigo)
                        {
                            lblclase.Visible = true;
                            lblclase.Text = "Codigo ingresado ya existe";
                            return;
                        }
                        else
                        {
                            lblclase.Visible = false;
                        }
                    }
                }
               
            }
        }

        private void griditemClase_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            fnCargarCampos();
            opItemClase.Cancela = false;
            opItemClase.SetButtonProperties(btnnuevoitem, 1);

            //ACTUALIZAR BOTON NUEVO EN FORMULARIO CLASE
            opClase.Cancela = false;
            opClase.SetButtonProperties(btnNuevaClase, 1);
            
        }

        private void griditemClase_MouseUp(object sender, MouseEventArgs e)
        {
            fnCargarCampos();
        }

        private void griditemClase_KeyUp(object sender, KeyEventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            fnCargarCampos();
        }

        private void txtnumitem_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void txtcod_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtdesc_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtitem_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtformula_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtnumitem_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            int codclase = 0, numitem = 0;
            string item = "";
            if (viewClase.RowCount > 0 || viewitemClase.RowCount > 0)
            {
                if (viewClase.RowCount > 0)
                {
                    codclase = Convert.ToInt32(viewClase.GetFocusedDataRow()["codclase"]);

                    if (CambiosSinguardarClase(codclase))
                    {
                        DialogResult adv = XtraMessageBox.Show("Hay cambios sin guardar, ¿Deseas cerrar de todas formas?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (adv == DialogResult.Yes)
                            Close();
                    }
                    else if (viewitemClase.RowCount > 0)
                    {
                        numitem = Convert.ToInt32(viewitemClase.GetFocusedDataRow()["numitem"]);
                        item = (string)viewitemClase.GetFocusedDataRow()["item"];

                        if (CambiosItemClase(codclase, item, numitem))
                        {
                            DialogResult adv = XtraMessageBox.Show("Hay cambios sin guardar, ¿Deseas cerrar de todas formas?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (adv == DialogResult.Yes)
                                Close();
                        }
                        else
                            Close();
                    }
                    else
                        Close();
                }
                else
                    Close();
            }
            else
                Close();

         
        }
    }
    //CLASE SOLO PARA GENERA UNA LISTA DE CONTRATOS Y RUT (PARA ITEMTRABAJADOR)
    class ItemClaseTrabajador
    {
        public string contrato { get; set; } = "";
        public string rut { get; set; } = "";
    }

    class ItemActualizable
    {
        public string contrato { get; set; }
        public int periodo { get; set; }
        public string  item { get; set; }
        public int numitem { get; set; }
    }

  
}