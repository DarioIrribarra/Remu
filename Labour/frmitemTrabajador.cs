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
using System.Threading;
using System.Collections;
using System.Runtime.InteropServices;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.Utils;

namespace Labour
{
    public partial class frmitemTrabajador : DevExpress.XtraEditors.XtraForm
    {
        [DllImport("user32")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32")]
        static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        const int MF_BYCOMMAND = 0;
        const int MF_DISABLED = 2;
        const int SC_CLOSED = 0xF060;

        //VARIABLES PARA GUARDAR LOS DATOS QUE VIENEN DESDE EL FORMULARIO EMPLEADO
        //RUT, CONTRATO
        private string contrato = "";
        private string rut = "";
        private int PeriodoEmpleado = 0;
        //BANDERA PARA SABER SI SE DESEA INGRESO NUEVO O MODIFICACION
        private bool update = false;
        //VARIABLE PARA OBTENER LA FILA SELECCIONADA DE LA GRILLA
        private int filaSeleccionada = 0;
        //PARA QUITAR FORMATEO AL RUT
        private string rutFormatear = "";      

        //VARIABLE PARA GUARDAR LA SUMATORIA DE HABERES IMPONIBLES
        private double SumaHaberes = 0;

        //BOTON NUEVO
        Operacion op;

        //DATOS DEL TRABAJADOR
        Persona trabajador = new Persona();        
        

        //CONSTRUCTOR FORMULARIO
        public frmitemTrabajador(string contrato, string rut, int periodo)
        {
            InitializeComponent();
            this.contrato = contrato;
            this.rut = rut;
            PeriodoEmpleado = periodo;
        }

        //GET PARA CONTRATO Y RUT   
        private string getContrato()
        {
            if (contrato != "")
                return contrato;
            else
                return "";
        }

        private string getRut()
        {
            if (rut == "") return "";

            return rut;
        }

        private void frmitemHis_Load(object sender, EventArgs e)
        {           
            var sm = GetSystemMenu(Handle, false);
            EnableMenuItem(sm, SC_CLOSED, MF_BYCOMMAND | MF_DISABLED);

            int ContratoTipo = 0;
            bool existeItem = false;

            //PARA BOTON NUEVO
            op = new Operacion();

            //CARGAR COMBO FORMULA
            fnComboFormula("SELECT codformula, descformula FROM formula", txtFormula, "codformula", "descformula", true);

            //cargar grilla
            if (getRut() != "" && getContrato() != "")
            {
                //DATOS DEL TRABAJADOR                
                trabajador = Persona.GetInfo(getContrato(), PeriodoEmpleado);

                lblNombre.Text = $"Contrato: {getContrato()}  - {trabajador.NombreCompleto}";

                //CARGAR COMBO BOX ITEM
                fnComboItem("SELECT coditem, descripcion FROM item", txtItem, "coditem", "descripcion", true);               

                //RECALCULAMOS LIQUIDACION
                Haberes hab = new Haberes(getContrato(), PeriodoEmpleado);
                hab.CalculoLiquidacion();

                //TIPO CONTRATO PERSONA
                ContratoTipo = trabajador.Tipocontrato;
               
                //PREGUNTAR SI EL CONTRATO ES FIJO
                if (ContratoTipo == 1)
                {
                    //SI ES FIJO VERIFICAR SI EXISTE ITEM SEGURO CESANTIA TRABAJADOR                                        
                    existeItem = ItemTrabajador.Existe("SCEMPLE", getContrato(), PeriodoEmpleado);
                    if (existeItem)
                    {
                        //SI EXISTE ITEM LO ELIMINAMOS YA QUE EN CONTRATO TIPO FIJO NO SE CALCULA SEGURO CESANTIA POR PARTE DEL EMPLEADO                        
                        ItemTrabajador.Eliminar("SCEMPLE", getContrato(), PeriodoEmpleado);
                    }
                }
              
                //CARGAR EL PERIODO DEL REGISTRO
                lblAnomes.Text = lblAnomes.Text + " " + fnSistema.PrimerMayuscula(fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(PeriodoEmpleado)));               

                rutFormatear = fnSistema.fnExtraerCaracteres(getRut());

                //MOSTRAR NOMBRE EMPLEADO
                groupItems.Text = "ITEMS ASOCIADOS A: " + trabajador.NombreCompleto;
              
                string grilla = string.Format("SELECT coditem, formula," +
                    " valor, valorCalculado, esclase, proporcional, tipo, numitem, permanente, contope, cuota, uf, pesos, porc, suspendido FROM itemtrabajador " +                  
                    " WHERE rut ='{0}' AND contrato = '{1}' AND anomes={2} ORDER BY tipo, orden", fnSistema.fnExtraerCaracteres(getRut()), getContrato(), PeriodoEmpleado);               

                fnSistema.spllenaGridView(gridHis, grilla);
                fnSistema.spOpcionesGrilla(viewHis);
                fnDefaultProperties();
                fnColumnasGrilla();               
            }            
            //CARGAR DATOS
            fnCargarCampos(0);            
        }

        #region "Manejo de Datos"      
        /*
         * INGRESO NUEVO REGISTRO ITEM HISTORICO
         * PARAMETROS DE ENTRADA:
         * id -> AUTOINCREMENTAL
         * codigo item --> viene de tabla item
         * numero item --> integer correlativo
         * anomes --> fecha viene de tabla 'PARAMETROS' (POR CREAR)
         * formula --> viene de la tabla item
         * valor --> campo numerico
         * tipo --> viene de la tabla item
         * orden --> viene de la tabla item
         * rut --> rut del empleado
         * contrado --> codigo de contrato del empleado
         */
        private void fnNuevoHistorico(LookUpEdit pcodItem, LookUpEdit pFormula, TextEdit pValor, TextEdit pnumItem, 
            int panoMes, int pTipo, int pOrden, string pRut, string pContrato, string cuotas, 
            CheckEdit pUf, CheckEdit pPesos, CheckEdit pPorcentaje, CheckEdit pSuspendido)
        {
            //SQL INSERT
            string sql = "INSERT INTO itemtrabajador(contrato, rut, coditem, numitem, anomes, formula, valor, tipo, orden, " +
                        "esclase, proporcional, valorcalculado, permanente, contope, cuota, uf, pesos, porc, suspendido)" +
                        " VALUES(@pcontrato, @prut, @pcoditem, @pnumitem, @panomes, @pformula, @pvalor, @ptipo, " +
                        "@porden, @esclase, @prop,@calculado, @permanente, @tope, @cuota, @pUf, @pPesos, @pPorcentaje, @pSuspendido)";            
            

            SqlCommand cmd;
            string formula = "";
            int permanente = 0, ContratoTipo = 0;
            bool Aplicauf = false, AplicaPesos = false, AplicaPorcentaje = false;
            double calculado = 0;
            int tope = 0;

            if (pFormula.EditValue.ToString() == "SIN FORMULA")
                formula = "0";
            else
                formula = pFormula.EditValue.ToString();

            int proporcional = 0;
            if (cbProporcional.Checked)
                proporcional = 1;
            else
                proporcional = 0;

            //CAMPO PERMANENTE
            if (cbpermanente.Checked) permanente = 1;

            if (cbTope.Checked) tope = 1;
           
            ContratoTipo = Persona.GetTipoContrato(pContrato, panoMes);
            if (ContratoTipo == 1 && pcodItem.EditValue.ToString() == "SCEMPLE")
            {
                //SI CONTRATO ES FIJO Y EL ITEM QUE SE INTENTA GUARDAR ES SEGURO DE CESANTIA PARTE EMPLEADO NO SE DEBE INGRESAR
                lblAdulto.Visible = true;
                lblAdulto.Text = "Para contratos fijos el seguro de cesantia es pagado en su totalidad por el empleador";
                return;
            }
            else
                lblAdulto.Visible = false;

            int res = 0;

            //PARA ISAPRES
            if (pcodItem.EditValue.ToString() == "SALUD")
            {
                //VERIFICAMOS QUE ESTE SELECCIONADA PESOS O UF
                if (pUf.Checked == false && pPesos.Checked == false && pPorcentaje.Checked == false)
                {
                    if (trabajador.codSalud != 1)
                    {
                        XtraMessageBox.Show("Ingrese un monto en pesos, uf o porcentaje para isapres.", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return; 
                    }
                }

                if (pUf.Checked && pPesos.Checked && pPorcentaje.Checked)
                { XtraMessageBox.Show("Por favor selecciona solo una opcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                if (pUf.Checked)
                {
                    Aplicauf = true;
                    if (fnDecimalSalud(Convert.ToDouble(pValor.Text)) == false)
                    { XtraMessageBox.Show("Por favor ingresa un valor en UF", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                    if (Convert.ToDouble(pValor.Text) == 0)
                    {
                        DialogResult advertencia = XtraMessageBox.Show("¿Estás seguro de ingresar 0 uf?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (advertencia == DialogResult.No)
                            return;
                    }                  
                }

                if (pPesos.Checked)
                {
                    AplicaPesos = true;
                    if (fnDecimal(pValor.Text) == false)
                    { XtraMessageBox.Show("Por favor ingresa un valor valido", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                }

                if (pPorcentaje.Checked)
                {
                    AplicaPorcentaje = true;
                    if (Convert.ToDouble(pValor.Text) >= 30 || Convert.ToDouble(pValor.Text) < 7)
                    {
                        XtraMessageBox.Show("Por favor ingresa un porcentaje valido", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
            }

            if (cbSuspendido.Checked)
            {
                DialogResult Advertencia = XtraMessageBox.Show($"Estás a punto de suspender el item {pcodItem.EditValue}, ¿Estás seguro de realizar esta operación?", "Suspender Item", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (Advertencia == DialogResult.No)
                    return;
            }

            //PARA LOG
            DataTable dataitem = GeneraDataTable();

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //DEFINICION DE PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pcontrato", pContrato));                        
                        cmd.Parameters.Add(new SqlParameter("@prut", pRut));
                        cmd.Parameters.Add(new SqlParameter("@pcoditem", pcodItem.EditValue));
                        cmd.Parameters.Add(new SqlParameter("@pnumitem", pnumItem.Text));
                        cmd.Parameters.Add(new SqlParameter("@panomes", panoMes));
                        cmd.Parameters.Add(new SqlParameter("@pformula", formula));
                        cmd.Parameters.Add(new SqlParameter("@pvalor", Convert.ToDecimal(pValor.Text)));
                        cmd.Parameters.Add(new SqlParameter("@ptipo", pTipo));
                        cmd.Parameters.Add(new SqlParameter("@porden", pOrden));
                        cmd.Parameters.Add(new SqlParameter("@esclase", false));
                        cmd.Parameters.Add(new SqlParameter("@prop", proporcional));
                        cmd.Parameters.Add(new SqlParameter("@calculado", Convert.ToDecimal(pValor.Text)));
                        cmd.Parameters.Add(new SqlParameter("@permanente", permanente));
                        cmd.Parameters.Add(new SqlParameter("@tope", tope));
                        cmd.Parameters.Add(new SqlParameter("@cuota", cuotas));
                        cmd.Parameters.Add(new SqlParameter("@pUf", Aplicauf));
                        cmd.Parameters.Add(new SqlParameter("@pPesos", AplicaPesos));
                        cmd.Parameters.Add(new SqlParameter("@pPorcentaje", AplicaPorcentaje));
                        cmd.Parameters.Add(new SqlParameter("@pSuspendido", cbSuspendido.Checked));

                        //EJECUTAR LA CONSULTA
                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            XtraMessageBox.Show("Registro guardado correctamente", "Guardar", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            //Ejecutamos el calculo de liquidaciones.
                            Haberes hab = new Haberes(getContrato(), panoMes);
                            hab.CalculoLiquidacion();
                            hab.CalculoLiquidacion();

                            calculado = ItemTrabajador.GetValCal(pcodItem.EditValue.ToString(), pContrato, panoMes, Convert.ToInt32(pnumItem.Text));
                            
                            logRegistro log = new logRegistro(User.getUser(), "INGRESA NUEVO ITEM " + pcodItem.EditValue + " TRABAJADOR N° " + pContrato, "ITEMTRABAJADOR", "0", calculado+"" , "INGRESAR");
                            log.Log();

                            string grilla = string.Format("SELECT coditem, formula," +
                             " valor, valorCalculado, esclase, proporcional, tipo, numitem, permanente, contope, cuota, uf, pesos, porc, suspendido FROM itemtrabajador " +           
                            " WHERE rut ='{0}' AND contrato = '{1}' AND anomes={2} ORDER BY tipo, orden", fnSistema.fnExtraerCaracteres(getRut()), getContrato(), panoMes);

                            fnSistema.spllenaGridView(gridHis, grilla);
                            fnColumnasGrilla();
                            fnCargarCampos(0);

                            op.Cancela = false;
                            op.SetButtonProperties(btnNuevo, 1);                            

                            bool c = false;
                            if (pTipo == 5 && cbTope.Checked)
                            {
                                c = TopeDescuento(Convert.ToDouble(viewHis.GetFocusedDataRow()["valorcalculado"]), pContrato, panoMes);
                                if (c)
                                    XtraMessageBox.Show("Valor excede 15% descuento legal", "Tope Legal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        /*MODIFICAR REGISTRO ITEM HISTORICO*/
        private void fnModificarHistorico(LookUpEdit pcodItem, LookUpEdit pFormula, TextEdit pValor, TextEdit pnumItem,
            int panoMes, int pTipo, int pOrden, string pRut, string pContrato, string cuotas, string pItembd, int pnumItembd, 
            CheckEdit pUf, CheckEdit pPesos, CheckEdit pPorcentaje, CheckEdit pSuspendido)
        {
            //SQL UPDATE
            string sql = "UPDATE itemtrabajador set coditem=@pcoditem, numitem=@pnumitem," +
                " formula=@pformula, valor=@pvalor, tipo=@ptipo, orden=@porden, " +
                " proporcional=@prop, permanente=@permanente, contope=@tope, cuota=@cuota, uf=@pUf, " +
                " pesos=@pPesos, porc=@pPorcentaje, suspendido=@pSuspendido WHERE " +
                " contrato=@pcontrato AND anomes=@pPeriodo AND coditem=@pItem AND numitem=@pNumbd";
           
            SqlCommand cmd;
            string formula = "";
            bool valorValido = false, Aplicauf=false, AplicaPesos=false, AplicaPorcentaje = false, Esisa = false;
            int permanente = 0, tope = 0, salud = 0, ContratoTipo = 0;            
           
            if (pFormula.EditValue.ToString() == "SIN FORMULA")
            {
                formula = "0";
            }
            else
            {
                formula = pFormula.EditValue.ToString();
            }
            int res = 0;

            int proporcional = 0;
            if (cbProporcional.Checked)
            {
                proporcional = 1;
            }
            else
            {
                proporcional = 0;
            }

            if (cbpermanente.Checked) permanente = 1;

            if (cbTope.Checked) tope = 1;

            //TIPO CONTRATO
            ContratoTipo = trabajador.Tipocontrato;
            if (ContratoTipo == 1 && pcodItem.EditValue.ToString() == "SCEMPLE")
            {
                //SI CONTRATO ES FIJO Y EL ITEM QUE SE INTENTA GUARDAR ES SEGURO DE CESANTIA PARTE EMPLEADO NO SE DEBE INGRESAR
                lblAdulto.Visible = true;
                lblAdulto.Text = "Para contratos fijos el seguro de cesantia es pagado en su totalidad por el empleador";
                return;
            }
            else
            {
                lblAdulto.Visible = false;
            }

            //PARA ISAPRES
            if (pcodItem.EditValue.ToString() == "SALUD")
            {
                //VERIFICAMOS QUE ESTE SELECCIONADA PESOS O UF
                if (pUf.Checked == false && pPesos.Checked == false && pPorcentaje.Checked == false)
                {
                    if (trabajador.codSalud != 1)
                    { XtraMessageBox.Show("Ingrese un monto en pesos, uf o porcentaje para isapres ", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                }

                if (pUf.Checked && pPesos.Checked && pPorcentaje.Checked)
                { XtraMessageBox.Show("Por favor selecciona solo una opcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                if (pUf.Checked)
                {
                    Aplicauf = true;
                    if (fnDecimalSalud(Convert.ToDouble(pValor.Text)) == false)
                    { XtraMessageBox.Show("Por favor ingresa un valor en uf", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                    if (Convert.ToDouble(pValor.Text) == 0)
                    {
                        DialogResult advertencia = XtraMessageBox.Show("¿Estás seguro de ingresar 0 uf?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (advertencia == DialogResult.No)
                            return;
                    }                    
                }

                if (pPesos.Checked)
                {
                    AplicaPesos = true;
                    if (fnDecimal(pValor.Text) == false)
                    { XtraMessageBox.Show("Por favor ingresa un valor valido", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                }
                if (cbPorcentaje.Checked)
                {
                    AplicaPorcentaje = true;
                    if (Convert.ToDouble(pValor.Text) < 7 || Convert.ToDouble(pValor.Text) >= 30)
                    {
                        XtraMessageBox.Show("Por favor ingresa un porcentaje valido", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                     //   pValor.Text = "7";
                        return;
                    }
                    //SI NO SE SELECCIONA ASUMIMOS QUE ES EL 7%
                }
            }

            if (cbSuspendido.Checked)
            {
                DialogResult Advertencia = XtraMessageBox.Show($"Estás a pundo de suspender el item {pItembd}, ¿Estás seguro de realizar esta operación?", "Suspender Item", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (Advertencia == DialogResult.No)
                    return;
            }


            //TABLA HASH PARA LOG
            Hashtable tablaCodigoItem = new Hashtable();
            tablaCodigoItem = PrecargaItem(pContrato, pItembd, panoMes, pnumItembd);

            //DATATABLE
           // DataTable dataItemLog = GeneraDataTable();          

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                     
                        cmd.Parameters.Add(new SqlParameter("@pcoditem", pcodItem.EditValue));
                        cmd.Parameters.Add(new SqlParameter("@pnumitem", pnumItem.Text));
                        //cmd.Parameters.Add(new SqlParameter("@panomes", panoMes));
                        cmd.Parameters.Add(new SqlParameter("@pformula", formula));
                        cmd.Parameters.Add(new SqlParameter("@pvalor", Convert.ToDecimal(pValor.Text)));
                        //cmd.Parameters.Add(new SqlParameter("@pCalculado", Convert.ToDecimal(pValor.Text)));                                                
                        cmd.Parameters.Add(new SqlParameter("@ptipo", pTipo));
                        cmd.Parameters.Add(new SqlParameter("@porden", pOrden));
                        cmd.Parameters.Add(new SqlParameter("@prop", proporcional));
                        cmd.Parameters.Add(new SqlParameter("@permanente", permanente));
                        cmd.Parameters.Add(new SqlParameter("@tope", tope));
                        cmd.Parameters.Add(new SqlParameter("@cuota", cuotas));
                        cmd.Parameters.Add(new SqlParameter("@pcontrato", pContrato));
                        cmd.Parameters.Add(new SqlParameter("@pPeriodo", panoMes));
                        cmd.Parameters.Add(new SqlParameter("@pItem", pItembd));
                        cmd.Parameters.Add(new SqlParameter("@pNumbd", pnumItembd));
                        cmd.Parameters.Add(new SqlParameter("@pUf", Aplicauf));
                        cmd.Parameters.Add(new SqlParameter("@pPesos", AplicaPesos));
                        cmd.Parameters.Add(new SqlParameter("@pPorcentaje", AplicaPorcentaje));
                        cmd.Parameters.Add(new SqlParameter("@pSuspendido", cbSuspendido.Checked));
                        
                        //EJECUTAMOS LA CONSULTA
                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {                           
                            XtraMessageBox.Show("Registro Actualizado", "Actualizar", MessageBoxButtons.OK, MessageBoxIcon.Information);                          
             
                            Haberes hab = new Haberes(getContrato(), panoMes);
                            //hab.CalculoGenericoItemTrabajador(dataItemLog);
                            hab.CalculoLiquidacion();
                            hab.CalculoLiquidacion();

                            ComparaValorItem(tablaCodigoItem, (string)pcodItem.EditValue, pRut, int.Parse(pnumItem.Text), 
                                formula + "", pTipo, pOrden, decimal.Parse(pValor.Text), decimal.Parse(pValor.Text), 
                                Convert.ToBoolean(proporcional), Convert.ToBoolean(permanente), Convert.ToBoolean(tope));
                           
                            string grilla = string.Format("SELECT coditem, formula," +
                            " valor, valorCalculado, esclase, proporcional, tipo, numitem, permanente, contope, cuota, uf, pesos, porc, suspendido FROM itemtrabajador " +                           
                            " WHERE rut ='{0}' AND contrato = '{1}' AND anomes = {2} ORDER BY tipo, orden", fnSistema.fnExtraerCaracteres(getRut()), getContrato(), panoMes);

                            fnSistema.spllenaGridView(gridHis, grilla);

                            //RECARGAR CAMPOS
                            fnCargarCampos(filaSeleccionada);
                            //VOLVER A DEJAR EDITABLE EL CAMPO NUMERO ITEM
                            //txtnumItem.ReadOnly = false;
                            fnOcultarLabels();                            

                            bool c = false;
                            if (pTipo == 5 && cbTope.Checked)
                            {
                                c = TopeDescuento(Convert.ToDouble(viewHis.GetFocusedDataRow()["valorcalculado"]), pContrato, panoMes);
                                if (c)
                                    XtraMessageBox.Show("Valor excede 15% descuento legal", "Tope Legal", MessageBoxButtons.OK, MessageBoxIcon.Warning);                    
                            }
                        }
                        else
                        {
                            XtraMessageBox.Show("Error al intentar actualizar", "Actualizar", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        /*ELIMINAR REGISTRO ITEM HISTORICO*/
        /*
         * PARAMETROS DE ENTRADA: ID REGISTROS 
         */
        private void fnEliminarHistorico(string pDesc, int pNumdb, string pItemdb, string pContrato, int pPeriodo)
        {
            //SQL DELETE
            string sql = "DELETE FROM itemtrabajador WHERE contrato=@pContrato " +
                "AND anomes=@pPeriodo AND coditem=@pItem AND numitem=@pNum";
            SqlCommand cmd;
            int res = 0;
            double calculado = 0;

            

            //OBTENER EL VALOR CALCULADO DEL ITEM SOLO PARA LOG
            calculado = ItemTrabajador.GetValCal(pItemdb, pContrato, pPeriodo, pNumdb);            
            
            DialogResult dialogo = XtraMessageBox.Show("¿Realmente desea eliminar el item " + pDesc + "?", "Eliminar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogo == DialogResult.Yes)
            {
                try
                {
                    if (fnSistema.ConectarSQLServer())
                    {
                        using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@pContrato", pContrato));
                            cmd.Parameters.Add(new SqlParameter("@pPeriodo", pPeriodo));
                            cmd.Parameters.Add(new SqlParameter("@pItem", pItemdb));
                            cmd.Parameters.Add(new SqlParameter("@pNum", pNumdb));                           

                            //EJECUTAR CONSULTA
                            res = cmd.ExecuteNonQuery();
                            if (res > 0)
                            {
                                XtraMessageBox.Show("Registro Eliminado", "Eliminar", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                //SI SE ELIMIN CORRECTAMENTE RECALCULAMOS VALORES HABERES
                                //RecalcularItemsHaberes();                               

                                Haberes hab = new Haberes(getContrato(), pPeriodo);
                                hab.CalculoLiquidacion();
                                //hab.CalculoGenericoItemTrabajador(null);
                                //hab.CalculoGenerico();                                

                                //VOLVER A CARGAR CAMPOS
                                string grilla = string.Format("SELECT coditem, formula," +
                                " valor, valorCalculado, esclase, proporcional, tipo, numitem, permanente, contope, cuota, uf, pesos, porc, suspendido FROM itemtrabajador " +                              
                                " WHERE rut ='{0}' AND contrato = '{1}' AND anomes = {2} ORDER BY tipo, orden", fnSistema.fnExtraerCaracteres(getRut()), getContrato(), PeriodoEmpleado);

                                fnSistema.spllenaGridView(gridHis, grilla);
                                fnCargarCampos(0);
                                
                                //LOG REGISTRO
                                logRegistro log = new logRegistro(User.getUser(), "SE ELIMINA CODIGO ITEM " + pItemdb + " ASOCIADO A TRABAJADOR N°" + getContrato(), "ITEMTRABAJADOR", calculado+"", "0", "ELIMINAR");
                               log.Log();
                             
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
                        XtraMessageBox.Show("No hay conexion con la base de datos", "Base de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (SqlException ex)
                {
                    XtraMessageBox.Show(ex.Message);
                }
            }
            else
            {
                XtraMessageBox.Show("El usuario a cancelado la operacion", "Operacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            } 
        }


        //LIMPIAR CAMPOS
        private void fnLimpiar()
        {            
            txtValor.Text = "";
            txtItem.ItemIndex = 0;
            lblmsg.Visible = false;
            lblnumber.Visible = false;
            lblvalue.Visible = false;
            update = false;
            btnEliminar.Enabled = false;
            
            //CARGAR NUMERO ITEM
            //int num = fnGeneraNumero();
            int num = fnItemDisponibles();
            txtnumItem.Text = num.ToString();
            txtItem.ReadOnly = false;
            txtFormula.ReadOnly = false;            

            txtValor.Text = "0";
            cbProporcional.Checked = false;
            cbpermanente.Checked = false;
            lblAdulto.Visible = false;
            gridHis.TabStop = false;

            //DESHABILITAMOS CHECKBOX CUOTAS
            cbAplicaCuotas.Checked = false;
            txtNumCuotas.Enabled = false;
            txtTotalCuotas.Enabled = false;
            txtNumCuotas.Text = "";
            txtTotalCuotas.Text = "";

            op.Cancela = true;
            op.SetButtonProperties(btnNuevo, 2);
            txtItem.Focus();
            cbSuspendido.Checked = false;

            if (txtItem.Properties.DataSource != null)
                txtItem.ItemIndex = 0;

            txtItem.Properties.Appearance.BackColor = Color.Wheat;
     
            txtFormula.Properties.Appearance.BackColor = Color.Wheat;
           
            txtValor.Properties.Appearance.BackColor = Color.Wheat;            

            lblmsg.Visible = true;
            lblmsg.Text = "Por favor selecciona un item";
        }

        //PROPIEDADES POR DEFECTO
        private void fnDefaultProperties()
        {
            txtItem.ItemIndex = 0;
            txtnumItem.Properties.MaxLength = 3;
            txtValor.Properties.MaxLength = 10;
            txtValor.Text = "0";
            cbpermanente.EnterMoveNextControl = false;
            cbProporcional.EnterMoveNextControl = false;
            btnEliminar.TabStop = false;
            btnNuevo.TabStop = false;
            //btnGuardar.TabStop = false;
            gridHis.TabStop = false;
            btnliquidacion.TabStop = false;
            txtDesc.TabStop = false;
        }

        //PROPIEDADES GRILLA (COLUMNAS)
        private void fnColumnasGrilla()
        {
            gridHis.ToolTipController = toolTipController1;

            viewHis.Columns[0].Caption = "Item";
            viewHis.Columns[0].AppearanceCell.FontStyleDelta = FontStyle.Bold;
            viewHis.Columns[0].Width = 60;

            viewHis.Columns[1].Caption = "Formula";
            viewHis.Columns[1].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            viewHis.Columns[1].DisplayFormat.FormatString = "formula";
            viewHis.Columns[1].DisplayFormat.Format = new FormatCustom();

            viewHis.Columns[2].Caption = "Valor Inicial";
            viewHis.Columns[2].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            viewHis.Columns[2].DisplayFormat.FormatString = "n3";

            viewHis.Columns[3].Caption = "Calculado";
            viewHis.Columns[3].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            viewHis.Columns[3].DisplayFormat.FormatString = "n3";

            viewHis.Columns[4].Caption = "Item de Clase";
            viewHis.Columns[5].Caption = "Proporcional";
            viewHis.Columns[6].Visible = false;
            viewHis.Columns[7].Visible = false;
            viewHis.Columns[8].Visible = false;
            viewHis.Columns[9].Visible = false;
            viewHis.Columns[10].Visible = false;
            viewHis.Columns[11].Visible = false;
            viewHis.Columns[12].Visible = false;
            viewHis.Columns[13].Visible = false;
            viewHis.Columns[14].Visible = false;
         
           
        }

        #region "MANEJO NUMERO ITEM"
        //CONSULTAR POR EL ULTIMO ITEM INGRESADO
        private int fnUltimoNumero(string pcontrato, string prut)
        {
            string sql = "SELECT max(numitem) as maximo FROM itemtrabajador WHERE contrato=@pcontrato AND rut=@prut AND anomes=@periodo";
            SqlCommand cmd;
            SqlDataReader rd;
            int num = 0;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        cmd.Parameters.Add(new SqlParameter("@pcontrato", pcontrato));
                        cmd.Parameters.Add(new SqlParameter("@prut", fnSistema.fnExtraerCaracteres(prut)));
                        cmd.Parameters.Add(new SqlParameter("@periodo", PeriodoEmpleado));

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

        //GENERA NUMERO ITEM CORRELATIVO
        //BUSCAMOS EL ULTIMO INGRESADO Y RETORNAMOS EL MAXIMO + 1
        private int fnGeneraNumero()
        {
            int generado = 0;
            int obtenido = 0;
            string rut = "", contrato = "";
            //USAMOS LA FUNCION PARA OBTENER EL ULTIMO NUMERO INGRESADO
            if (getContrato() != "") contrato = getContrato();
            if (getRut() != "") rut = getRut();

            obtenido = fnUltimoNumero(contrato, rut);
            //SI SE OBTIENE -1 ES PORQUE NO HAY REGISTROS
            if (obtenido == -1)
            {
                generado = 1;
            }
            else
            {
                generado = obtenido + 1;
            }

            return generado;
        }

        //TRAER TODOS LOS NUMEROS DE ITEM DESDE LA BASE DE DATOS
        private int[] fnAllitem()
        {
            int total = 0;
            total = fnCantidad();
            int[] numeros = new int[total];

            int posicion = 0;
            string rut = "", contrato = "";

            if (getRut() != "") rut = getRut();
            if (getContrato() != "") contrato = getContrato();

            //sql consulta
            string sql = string.Format("SELECT numitem FROM itemtrabajador WHERE contrato='{0}' AND rut='{1}' AND anomes={2}", contrato, fnSistema.fnExtraerCaracteres(rut), PeriodoEmpleado);
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
            string rut = "", contrato = "";
            if (getRut() != "") rut = getRut();
            if (getContrato() != "") contrato = getContrato();

            string sql = string.Format("SELECT count(*) as cantidad FROM itemtrabajador WHERE contrato='{0}' AND rut='{1}' AND anomes={2}", contrato, fnSistema.fnExtraerCaracteres(rut), PeriodoEmpleado);
            SqlCommand cmd;
            SqlDataReader rd;

            int total = 0;
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
                                total = (int)rd["cantidad"];
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
                    if (getRut() != "" && getContrato() != "")
                        devuelve = fnUltimoNumero(getContrato(), getRut()) + 1;
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

        #endregion

        //CARGAR CAMPOS DESDE GRILLA A FORMULARIO
        private void fnCargarCampos(int? pos = -1)
        {
            if (viewHis.RowCount>0)
            {
                //SI SE INGRESA CERO ASUMIMOS QUE QUEREMOS MANIPULAR LA PRIMERA FILA DE LA GRILLA
                if (pos != -1) viewHis.FocusedRowHandle = (int)pos;

                //DEJAR CAMPO NUMERO ITEM COMO SOLO LECTURA
                //txtnumItem.ReadOnly = true;

               // txtItem.Properties.AppearanceFocused.BackColor = Color.DodgerBlue;
               // txtItem.Properties.AppearanceFocused.ForeColor = Color.White;
                //BANDERA EN TRUE PARA ACTUALIZAR
                update = true;

                btnliquidacion.Enabled = true;
                lblmsg.Text = "";
                lblmsg.Visible = false;

                lblvalue.Visible = false;
                lblAdulto.Visible = false;

                bool esclase = (bool)(viewHis.GetFocusedDataRow()["esclase"]);
                if (esclase)
                {
                    txtItem.ReadOnly = true;
                    txtFormula.ReadOnly = true;
                    txtnumItem.ReadOnly = true;
                    btnEliminar.Enabled = false;
                }
                else
                {
                    txtItem.ReadOnly = false;
                    txtFormula.ReadOnly = false;
                    btnEliminar.Enabled = true;
                    //txtnumItem.ReadOnly = false;
                }                

                //CARGAMOS CAMPOS
                txtItem.EditValue = viewHis.GetFocusedDataRow()["coditem"];

                if (viewHis.GetFocusedDataRow()["formula"].ToString() == "0")
                    txtFormula.EditValue = "0";
                else
                txtFormula.EditValue = viewHis.GetFocusedDataRow()["formula"].ToString();

                bool activo = (bool)viewHis.GetFocusedDataRow()["proporcional"];
                if (activo)
                {
                    cbProporcional.Checked = true;
                }
                else
                {
                    cbProporcional.Checked = false;
                }

                bool per = (bool)viewHis.GetFocusedDataRow()["permanente"];
                if (per)
                    cbpermanente.Checked = true;
                else
                    cbpermanente.Checked = false;

                if ((bool)viewHis.GetFocusedDataRow()["contope"])
                {
                    cbTope.Checked = true;
                }
                else
                {
                    cbTope.Checked = false;
                }

                if (Convert.ToInt32(viewHis.GetFocusedDataRow()["tipo"]) == 5)
                    cbTope.Enabled = true;
                else
                    cbTope.Enabled = false;

                string cuota = (string)viewHis.GetFocusedDataRow()["cuota"];
                Hashtable infoCuotas = new Hashtable();
                //SI EL ITEM ES DE TIPO DESCUENTO Y EL VALOR DE CUOTA ES DISTINTO DE 0 MOSTRAMOS LAS CUOTAS
                if (cuota != "0")
                {
                    cbAplicaCuotas.Checked = true;
                    infoCuotas = GetCuotasFromString(cuota);
                    txtNumCuotas.Enabled = true;
                    txtTotalCuotas.Enabled = true;
                    txtNumCuotas.Text = (string)infoCuotas["NumeroCuota"];
                    txtTotalCuotas.Text = (string)infoCuotas["TotalCuota"];
                    gridHis.Focus();
                }
                else
                {
                    cbAplicaCuotas.Checked = false;                    
                    txtNumCuotas.Text = "";
                    txtTotalCuotas.Text = "";
                    txtNumCuotas.Enabled = false;
                    txtTotalCuotas.Enabled = false;
                }

                if ((bool)viewHis.GetFocusedDataRow()["uf"])
                {
                    cbUf.Checked = true;
                    txtValor.Text = viewHis.GetFocusedDataRow()["valor"].ToString();
                }                    
                else
                    cbUf.Checked = false;
                if ((bool)viewHis.GetFocusedDataRow()["pesos"])
                {
                    cbPesos.Checked = true;
                    txtValor.Text = viewHis.GetFocusedDataRow()["valor"].ToString();
                }                    
                else
                    cbPesos.Checked = false;
                if ((bool)viewHis.GetFocusedDataRow()["porc"])
                {
                    cbPorcentaje.Checked = true;
                    txtValor.Text = viewHis.GetFocusedDataRow()["valor"].ToString();
                }                    
                else
                    cbPorcentaje.Checked = false;

                txtnumItem.Text = viewHis.GetFocusedDataRow()["numitem"].ToString();
                txtValor.Text = viewHis.GetFocusedDataRow()["valor"].ToString();
                cbSuspendido.Checked = Convert.ToBoolean(viewHis.GetFocusedDataRow()["suspendido"]);

                txtItem.Properties.Appearance.BackColor = Color.White;
                txtFormula.Properties.Appearance.BackColor = Color.White;
                txtValor.Properties.Appearance.BackColor = Color.White;
             
            }
            else
            {
                update = false;
                fnLimpiar();
            }
        }

        //CARGAR COMBO CODIGO ITEM
        private void fnComboItem(string pSql, LookUpEdit pCombo, string pCampoKey, string pCampoDesc, bool? pOcultarKey = false)
        {
            List<Combos> lista = new List<Combos>();            
            SqlCommand cmd;
            SqlDataReader rd;

            if (fnSistema.ConectarSQLServer())
            {
                using (cmd = new SqlCommand(pSql, fnSistema.sqlConn))
                {
                    rd = cmd.ExecuteReader();
                    if (rd.HasRows)
                    {
                        while (rd.Read())
                        {
                            //AGREGAMOS VALORES A LA LISTA
                            lista.Add(new Combos() { key = (string)rd[pCampoKey], desc = (string)rd[pCampoDesc]});
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
            pCombo.Properties.DisplayMember = "key";

            pCombo.Properties.PopulateColumns();
            //SI OCULTAR KEY ES TRUE NO SE DEBE MOSTRAR LA COLUMNA KEY
            if (pOcultarKey == true)
            {
                pCombo.Properties.Columns[1].Visible = false;
            }
            pCombo.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
            pCombo.Properties.AutoSearchColumnIndex = 1;
            pCombo.Properties.ShowHeader = false;
        }

        //CARGAR COMBO FORMULA
        private void fnComboFormula(string pSql, LookUpEdit pCombo, string pCampoKey, string pCampoDesc, bool? pOcultarKey = false)
        {
            List<formula> lista = new List<formula>();

            SqlCommand cmd;
            SqlDataReader rd;
            
            if (fnSistema.ConectarSQLServer())
            {                
                using (cmd = new SqlCommand(pSql, fnSistema.sqlConn))
                {
                   //lista.Add(new formula() { key="0", desc = "SIN FORMULA"});
                    rd = cmd.ExecuteReader();                    
                    if (rd.HasRows)
                    {
                        while (rd.Read())
                        {
                            //AGREGAMOS VALORES A LA LISTA     
                            string code = (string)rd[pCampoKey];

                            lista.Add(new formula() { key = (string)rd[pCampoKey], desc = (string)rd[pCampoDesc] });
                            //if (code != "0")
                           // {
                            
                          //  }
                            
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

        //OCULTAR LABEL MENSAJES DE ERROR
        private void fnOcultarLabels()
        {
            lblmsg.Visible = false;
            lblnumber.Visible = false;
            lblvalue.Visible = false;
        }

        //VERIFICAR NUMERO DECIMAL CORRECTO
        private bool fnDecimal(string cadena)
        {
            if (cadena.Length == 0) return false;

            //recorrer cadena y verificar que tenga solo una coma
            int coma = 0;
            for (int position = 0; position < cadena.Length; position++)
            {
                if (cadena[position] == ',') coma++;
            }

            if (coma > 1) return false;

            string[] subcadena = new string[2];
            if (coma == 1)
            {
                subcadena = cadena.Split(',');

                //SI DESPUES DE LA CADENA TIENE MAS DE DOS DIGITOS NO ES CORRECTO
                if (subcadena[1].Length > 4) return false;
                if (subcadena[1].Length == 0) return false;
                if (subcadena[0].Length == 0) return false;

                return true;
            }
            else
            {
                //SI NO TIENE COMAS SOLO ES UN NUMERO
                if (cadena.Length > 8) return false;
                return true;
            }

        }

        //VERIFICAR DECIMAL SOLO PARA SALUD
        private bool fnDecimalSalud(double value)
        {
            string cadena = "";
            int number = 0;
            cadena = value.ToString();
            if (cadena.Length == 0) return false;

            //recorrer cadena y verificar que tenga solo una coma
            int coma = 0;
            for (int position = 0; position < cadena.Length; position++)
            {
                if (cadena[position] == ',') coma++;
            }

            if (coma > 1) return false;

            string[] subcadena = new string[2];
            if (coma == 1)
            {
                subcadena = cadena.Split(',');
                number = Convert.ToInt32(subcadena[0]);

                //SI DESPUES DE LA CADENA TIENE MAS DE DOS DIGITOS NO ES CORRECTO
                if (subcadena[1].Length > 4) return false;
                if (subcadena[0].Length > 2) return false;
                //if (number > 10) return false;
                if (subcadena[1].Length == 0) return false;
                if (subcadena[0].Length == 0) return false;

                return true;
            }
            else
            {
                //SI NO TIENE COMAS SOLO ES UN NUMERO
                if (cadena.Length > 2) return false;

                //  number = Convert.ToInt32(cadena);
                //  if (number > 10) return false;

                return true;
            }

        }

        private bool fnDecimalCalculado(double value)
        {
            string cadena = "";
            int number = 0;
            cadena = value.ToString();
            if (cadena.Length == 0) return false;

            //recorrer cadena y verificar que tenga solo una coma
            int coma = 0;
            for (int position = 0; position < cadena.Length; position++)
            {
                if (cadena[position] == ',') coma++;
            }

            if (coma > 1) return false;

            string[] subcadena = new string[2];
            if (coma == 1)
            {
                subcadena = cadena.Split(',');
                number = Convert.ToInt32(subcadena[0]);

                //SI DESPUES DE LA CADENA TIENE MAS DE DOS DIGITOS NO ES CORRECTO
                if (subcadena[1].Length > 4) return false;
                if (subcadena[0].Length > 2) return false;
                if (number > 10) return false;
                if (subcadena[1].Length == 0) return false;
                if (subcadena[0].Length == 0) return false;

                return true;
            }
            else
            {
                //SI NO TIENE COMAS SOLO ES UN NUMERO
                if (cadena.Length > 2) return false;

                number = Convert.ToInt32(cadena);
                if (number > 10) return false;

                return true;
            }

        }

        //CALCULAR EDAD DEL TRABAJADOR
        private int fnCalcularEdad(DateTime Nacimiento)
        {
            int edad = 0;
            //OBTENEMOS EL AÑO ACTUAL
            int year = DateTime.Now.Year;

            //OBTENEMOS EL AÑO DE LA FECHA DE NACIMIENTO
            int yearNac = Nacimiento.Year;

            //RESTAMOS
            edad = year - yearNac;

            //RETORNAMOS LA EDAD
            return edad;
        }

        //CALCULO 15% TOPE
        private bool TopeDescuento(double valor, string pContrato, int pPeriodo)
        {
            double TotalHaber = 0;
            double tope = 0;
            TotalHaber = Calculo.GetValueFromCalculoMensaul(pContrato, pPeriodo, "systhab");
            //XtraMessageBox.Show(varSistema.ObtenerValorLista("systhab").ToString());

            tope = 0.15 * TotalHaber;

            if (valor>tope)
            {
                //SOBRE EL TOPE DE DESCUENTO
                return true;
            }

            return false;
        }

        //VERIFICAR EN CASO DE QUE SE QUIERA MODIFICAR EL ITEM (ALERTAR CON MENSAJE)
        private bool AlertaCambioItem(string item)
        {
            string itemGrilla = "";
            if (update == true)
            {
                //SE ESTA EN ESTADO DE ACTUALIZACION
                if (viewHis.RowCount > 0)
                {
                    itemGrilla = viewHis.GetFocusedDataRow()["coditem"] + "";

                    if (item != itemGrilla)
                    {
                        //SE ESTA INTENTANDO CAMBIAR EL ITEM DE UN REGISTRO YA GUARDADO
                        return true;
                    }
                    else
                    {
                        //NO HAY PROBLEMA 
                        return false;
                    }
                }
            }

            return false;
        }

        //GENERA CADENA PARA GUARDAR EN CAMPO CUOTA
        private string CadenaCuota(string Numcuota, string totalCuota)
        {
            string cuota = "";
            cuota = Numcuota + "/" + totalCuota;

            return cuota;
        }

        //EXTRAE NUMERO DE CUOTA Y TOTAL DE CUOTAS DESDE CADENA (FROM BD)
        private Hashtable GetCuotasFromString(string value)
        {
            Hashtable data = new Hashtable();
            if (value.Length>0)
            {
                if (value.Contains("/"))
                {
                    string[] separa = new string[2];
                    separa = value.Split('/');

                    data.Add("NumeroCuota", separa[0]);
                    data.Add("TotalCuota", separa[1]);                    
                }
            }

            return data;
        }

        //VALIDAR CAMPOS PARA CUOTA
        private bool ValidaCuota(string value)
        {
            if (value.Length>0)
            {
                if (value.Length == 1)
                    if (value == "0")
                        return false;

                if (value[0] == '0')
                    return false;

                return true;
            }

            return false;
        }

        //VALIDAR QUE LA CUOTA INGRESADA SEA CORRECTA DE ACUERDO A INGRESOS PREVIOS
        private bool CuotaMaximaValida(string contrato, string CuotaTope)
        {
            string sql = "select cuota from itemtrabajador where contrato = @contrato" +
                         " AND anomes < @periodo AND coditem = 'PRESTAM'";
            SqlCommand cmd;
            SqlDataReader rd;
            //PARA GUARDAR TODOS LOS TOPE DE CUOTAS (ULTIMAS CUOTA DISPONIBLE)
            List<int> CuotasMaximas = new List<int>();
            int contados = 0;

            if (CuotaTope != "")
            {
                try
                {
                    if (fnSistema.ConectarSQLServer())
                    {
                        using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@contrato", contrato));
                            cmd.Parameters.Add(new SqlParameter("@periodo", PeriodoEmpleado));

                            rd = cmd.ExecuteReader();
                            if (rd.HasRows)
                            {
                                while (rd.Read())
                                {
                                    //AGREGAMOS DATOS
                                    Hashtable data = new Hashtable();
                                    data = GetCuotasFromString((string)rd["cuota"]);

                                    CuotasMaximas.Add(Convert.ToInt32(data["TotalCuota"]));
                                }
                            }
                            else
                            {
                                return true;
                            }
                        }
                        cmd.Dispose();
                        rd.Close();
                        fnSistema.sqlConn.Close();

                        if (CuotasMaximas.Count == 0)
                            return false;


                        //RECORREMOS LISTA Y VERIFICAMOS QUE EL VALOR A INGRESAR NO SUPERE ESTOS VALORES
                        foreach (var cuota in CuotasMaximas)
                        {
                            //NO VALIDO
                            if (Convert.ToInt32(CuotaTope) != cuota)
                            { contados++; }
                        }

                        //SI LA VARIABLE CONTADOS ES IGUAL A LA CANTIDAD DE ELEMENTOS QUE TIENE LA LISTA CUOTASMAXIMAS
                        //QUIERE DECIR QUE EL NUMERO NO COINCIDE CON NINGUNO, ES DECIR, NO ES VALIDA
                        if (contados == CuotasMaximas.Count)
                            return false;

                        //SI LLEGA ACA ES VALIDO...
                        return true;

                    }
                    else
                    {
                        return false;
                    }
                }
                catch (SqlException ex)
                {
                    XtraMessageBox.Show(ex.Message);
                    return false;
                }
            }
            else
            {
                return false;
            }           
      

        }

        //VALIDAR QUE EL NUMERO DE CUOTA NO ESTÉ YA INGRESADO
        private bool NumeroCuotaLogica(string contrato, string NumeroCuota, string Tope)
        {
            string sql = "select cuota from itemtrabajador where contrato = @contrato" +
                         " AND anomes < @periodo AND coditem = 'PRESTAM'";
            SqlCommand cmd;
            SqlDataReader rd;
            //PARA GUARDAR TODOS LOS TOPE DE CUOTAS (ULTIMAS CUOTA DISPONIBLE)
            List<int> NumerosCuota = new List<int>();
            List<int> SubLista = new List<int>();
            
            if (NumeroCuota != "")
            {
                try
                {
                    if (fnSistema.ConectarSQLServer())
                    {
                        using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@contrato", contrato));
                            cmd.Parameters.Add(new SqlParameter("@periodo", PeriodoEmpleado));

                            rd = cmd.ExecuteReader();
                            if (rd.HasRows)
                            {
                                while (rd.Read())
                                {
                                    //AGREGAMOS DATOS
                                    Hashtable data = new Hashtable();
                                    data = GetCuotasFromString((string)rd["cuota"]);

                                    //SI EL TOTAL DE CUOTAS COINCIDE CON EL TOPE(PARAMETRO)
                                    //GUARDAMOS EN LISTA
                                    if (Tope == (string)data["TotalCuota"])
                                        NumerosCuota.Add(Convert.ToInt32(data["NumeroCuota"]));
                                }
                            }
                            else
                                return true;
                        }

                        cmd.Dispose();
                        rd.Close();
                        fnSistema.sqlConn.Close();

                        if (NumerosCuota.Count == 0)
                            return false;

                        //OBTENER DE LA LISTA TODOS LOS ELEMENTOS QUE TENGA UN DIGITO MENOS AL NUMERO DE CUOTA
                        for (int i = 0; i < NumerosCuota.Count; i++)
                        {
                            if (NumerosCuota[i] == (Convert.ToInt32(NumeroCuota) - 1))
                                SubLista.Add(NumerosCuota[i]);
                        }

                        //SI ENCUENTRA UN DIGITO INMEDIATAMENTE ANTERIOR AL NUMERO DE CUOTA INGRESADA
                        //ASUMIMOS QUE EL VALOR QUE SE INTENTA INGRESAR ES VALIDO...
                        if (SubLista.Count > 0)
                            return true;
                        else
                            //SI NO ENCONTRÓ NINGUN NUMERO INMEDIATAMENTE ANTERIOR AL NUMERO DE CUOTA ES PORQUE
                            //NO HAY CONTINUIDAD DE NUMEROS, NO ES VALIDO
                            return false;                      
                     
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (SqlException ex)
                {
                    XtraMessageBox.Show(ex.Message);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        //VERIFICAR SI HAY CAMBIOS ANTES DE SALIR
        private bool CambiossinGuardar(int periodo, string pcontrato, string pItem, int pNum)
        {
            string sql = "SELECT coditem, formula, valor, " +
                "proporcional, permanente, contope, cuota FROM itemtrabajador WHERE " +
                "contrato=@pcontrato AND anomes=@periodo AND coditem=@pItem AND numitem=@pnum";

            string cuota = "";
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
                        cmd.Parameters.Add(new SqlParameter("@pcontrato", contrato));
                        cmd.Parameters.Add(new SqlParameter("@pItem", pItem));
                        cmd.Parameters.Add(new SqlParameter("@pnum", pNum));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {                                
                                cuota = (string)rd["cuota"];
                            
                                if (txtItem.EditValue.ToString() != (string)rd["coditem"]) return true; 
                                if (txtFormula.EditValue.ToString() != (string)rd["formula"]) return true;
                                if (txtValor.Text != "")
                                {
                                    if (Convert.ToDouble(txtValor.Text) != Convert.ToDouble(rd["valor"])) return true;
                                }                                    
                                if (cbProporcional.Checked != (bool)rd["proporcional"]) return true;
                                if (cbpermanente.Checked != (bool)rd["permanente"]) return true;
                                if (cbTope.Checked != (bool)rd["contope"]) return true;
                                if (cbAplicaCuotas.Checked && cuota == "0") return true;
                                if (cbAplicaCuotas.Checked == false && cuota != "0") return true;                                
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


        #region "LOG REGISTRO ITEMTRABAJADOR"
        //GENERA TABLA HASH CON LOS DATOS DEL ITEM A EVALUAR
        private Hashtable PrecargaItem(string pContrato, string pItem, int pPeriodo, int pNum)
        {
            Hashtable datos = new Hashtable();
            string sql = "SELECT coditem, rut, contrato, rut, numitem, formula, tipo, orden, esclase, valor, " +
                "valorcalculado, proporcional, permanente, contope FROM ITEMTRABAJADOR WHERE " +
                "contrato = @pContrato AND anomes=@pPeriodo AND coditem=@pItem AND numitem=@pNum";
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pContrato", pContrato));
                        cmd.Parameters.Add(new SqlParameter("@pPeriodo", pPeriodo));
                        cmd.Parameters.Add(new SqlParameter("@pItem", pItem));
                        cmd.Parameters.Add(new SqlParameter("@pNum", pNum));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //CARGAMOS HASH
                                datos.Add("coditem", (string)rd["coditem"]);
                                datos.Add("rut", (string)rd["rut"]);
                                datos.Add("contrato", (string)rd["contrato"]);
                                datos.Add("numitem", (int)rd["numitem"]);
                                datos.Add("formula", (string)rd["formula"]);
                                datos.Add("tipo", (int)rd["tipo"]);
                                datos.Add("orden", (int)rd["orden"]);
                                datos.Add("esclase", (bool)rd["esclase"]);
                                //datos.Add("valor", (decimal)rd["valor"]);
                                //datos.Add("valorcalculado", (decimal)rd["valorcalculado"]);
                                datos.Add("proporcional", (bool)rd["proporcional"]);
                                datos.Add("permanente", (bool)rd["permanente"]);
                                datos.Add("contope", (bool)rd["contope"]);                                
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
        private void ComparaValorItem(Hashtable Datos, string pCoditem, string pRut, int pNumItem, string pFormula,
            int pTipo, int pOrden, decimal pValor, decimal pValorCalculado, bool pProporcional,
            bool pPermanente, bool pTope)
        {

            //SI LOS VALORES SON DISTINTOS GUARDAMOS EVENTO EN LOG
            if (Datos.Count > 0)
            {
                if ((string)Datos["coditem"] != pCoditem)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA CODIGO ITEM " + (string)Datos["coditem"] + " PARA TRABAJADOR N° " + Datos["contrato"], "ITEMTRABAJADOR", (string)Datos["coditem"], pCoditem, "MODIFICAR");
                    log.Log();
                }
                if ((string)Datos["formula"] != pFormula)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA CODIGO FORMULA ITEM " + (string)Datos["coditem"] + " TRABAJADOR N° " + Datos["contrato"], "ITEMTRABAJADOR", (string)Datos["formula"], pFormula, "MODIFICAR");
                    log.Log();
                }
                //if ((decimal)Datos["valor"] != pValor)
               // {
                 //   logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA VALOR ORIGINAL ITEM " +(string)Datos["coditem"] +" PARA TRABAJADOR N° " + Datos["contrato"], "ITEMTRABAJADOR", (decimal)Datos["valor"]+"", pValor+"", "MODIFICAR");
                   // log.Log();
               // }
                if ((bool)Datos["proporcional"] != pProporcional)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA OPCION PROPORCIONAL PARA ITEM " + (string)Datos["coditem"] + " TRABAJADOR N° " + Datos["contrato"], "ITEMTRABAJADOR", (bool)Datos["proporcional"]+"", pProporcional+"", "MODIFICAR");
                    log.Log();
                }
               // if ((decimal)Datos["valorcalculado"] != pValorCalculado)
               // {
                 //   logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA VALOR CALCULADO PARA ITEM " + (string)Datos["coditem"] + " TRABAJADOR N° " + Datos["contrato"], "ITEMTRABAJADOR", (decimal)Datos["valorcalculado"]+"", pValorCalculado+"", "MODIFICAR");
                  //  log.Log();
               // }
                if ((bool)Datos["permanente"] != pPermanente)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA OPCION PERMANENTE PARA ITEM " + (string)Datos["coditem"] + " TRABAJADOR N° " + Datos["contrato"], "ITEMTRABAJADOR", (bool)Datos["permanente"] + "", pPermanente + "", "MODIFICAR");
                    log.Log();
                }
                if ((bool)Datos["contope"] != pTope)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA OPCION TOPE PARA ITEM " + (string)Datos["coditem"] + " TRABAJADOR N° " + Datos["contrato"], "ITEMTRABAJADOR", (bool)Datos["contope"] + "", pTope + "", "MODIFICAR");
                    log.Log();

                }           
            }
        }

        //GENERA DATATABLE
        //ESTE METODO GENERA UN DATATABLE CON TODAS LOS ITEM ASOCIADOS AL CONTRATO 
        //REPRESENTA A TODOS LOS ITEM PREVIO AL RECALCULO (PARA COMPARAR EN CASO DE CAMBIOS)
        private DataTable GeneraDataTable()
        {
            string sql = string.Format("SELECT coditem, valor, valorcalculado FROM " +
                "itemtrabajador WHERE contrato='{0}' AND anomes={1}", getContrato(), PeriodoEmpleado);

            SqlDataAdapter adapter;
            DataSet ds = new DataSet("items");
            DataTable tabla = new DataTable();

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    adapter = new SqlDataAdapter(sql, fnSistema.sqlConn);
                    //PASAMOS LOS DATOS A TRAVES DE FILL AL DATASET
                    adapter.Fill(ds, "itemtrabajador");
                    tabla = ds.Tables["itemtrabajador"];

                    adapter.Dispose();

                }
                fnSistema.sqlConn.Close();

            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            //RETORNAMOS EL DATATABLE
            return tabla;
        }
        #endregion


        #endregion

        #region "Controles Formulario"       
        private void txtItem_EditValueChanged(object sender, EventArgs e)
        {
            //OBTENER EL SELECCIONADO
            string item = "";
            if(txtItem.EditValue.ToString() != "")
                item = txtItem.EditValue.ToString();

            ItemBase itemb = new ItemBase(item);
            itemb.Setinfo();
            txtFormula.EditValue = itemb.Formula;          

            //CARGAR DESCRIPCION EN CAJA DE TEXTO
            txtDesc.Text = itemb.Descripcion;

            //PREGUNTAR SI EL ITEM SELECCIONADO ES UN HABER
            if (itemb.Tipo == 1 || itemb.Tipo == 2 || itemb.Tipo == 3)
                //HABILITAMOS CHECKBOX
                cbProporcional.Enabled = true;            
            else            
                cbProporcional.Enabled = false;

            //PREGUNTAR SI EL ITEM VIENE PROPORCIONAL DE BASE
            if (update == false)
            {
                if (itemb.Proporcional)
                    cbProporcional.Checked = true;
                else
                    cbProporcional.Checked = false;
            }

            if (itemb.Tipo == 5)
            {
                cbTope.Enabled = true;
                cbTope.Checked = itemb.tope;
            }
            else
            {
                cbTope.Enabled = false;
                cbTope.Checked = false;
            }                

            //SI ES DE TIPO DE 5 HABILITAMOS CHECKBOX CUOTAS
            if (itemb.Tipo == 5 || itemb.Tipo == 1)
                cbAplicaCuotas.Enabled = true;
            else
                cbAplicaCuotas.Enabled = false;

            //PARA ITEM SALUD
            if (item.ToLower() == "salud")
            {                
                  cbPesos.Enabled = true;
                  cbUf.Enabled = true;                    
                  cbPorcentaje.Enabled = true;                                           
            }
            else
            {
                cbPesos.Enabled = false;
                cbPesos.Checked = false;
                cbUf.Enabled = false;
                cbUf.Checked = false;
                cbPorcentaje.Enabled = false;
                cbPorcentaje.Checked = false;
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();
            
            if (op.Cancela == false)
            {
                fnLimpiar();               

                //DESHABILITAMOS BOTON ELIMINAR Y PREVISUALIZACION LIQUIDACION
                btnEliminar.Enabled = false;
                btnliquidacion.Enabled = false;
            }
            else
            {
                op.Cancela = false;
                op.SetButtonProperties(btnNuevo, 1);
                fnCargarCampos();
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            bool numCorrecto = false, cuotaCorrecta = false;
            int Numdb = 0;
            int[] datosItem = new int[2];
            string rut = "", item = "", itemGrilla = "", Cuotas = "0";
            bool tienAfp = false, tieneSalud = false, tieneCaja = false;
            bool AdultoMayor = false, MenorEdad = false, adultoMujer = false;
            Int16 SexoTrabajador = -1;
            DateTime nacimiento = DateTime.Now.Date;
            int cuota = 0, totalCuota = 0;       

            if (txtItem.EditValue == null) { XtraMessageBox.Show("Debes seleccionar un item", "Item", MessageBoxButtons.OK, MessageBoxIcon.Error);return;}
            if (txtFormula.EditValue == null) { XtraMessageBox.Show("Formula no valida", "Formula", MessageBoxButtons.OK, MessageBoxIcon.Error); txtFormula.Focus(); return;}

            if (txtItem.EditValue.ToString() == "") { XtraMessageBox.Show("Item no valido", "item", MessageBoxButtons.OK, MessageBoxIcon.Error); txtItem.Focus(); return;}
            if (txtnumItem.Text == "") { XtraMessageBox.Show("Numero item no valido", "Numero item", MessageBoxButtons.OK, MessageBoxIcon.Error);txtnumItem.Focus(); return; }
            if (txtValor.Text == "") { XtraMessageBox.Show("Campo valor no valido", "valor", MessageBoxButtons.OK, MessageBoxIcon.Error);txtValor.Focus(); return;}

            if (txtnumItem.Text == "0") { XtraMessageBox.Show("Valor no valido para numero item", "Numero Item", MessageBoxButtons.OK, MessageBoxIcon.Error);return;}

            //VALIDAR LAS CUOTAS
            if (cbAplicaCuotas.Enabled && cbAplicaCuotas.Checked)
            {
                if (txtNumCuotas.Text == "") { XtraMessageBox.Show("Por favor ingresa el numero de cuota", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                if (txtTotalCuotas.Text == "") { XtraMessageBox.Show("Por favor ingresa el total de cuotas", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Error);return; }

                //VALIDAR VALOR DA CAJAS
                cuotaCorrecta = ValidaCuota(txtNumCuotas.Text);
                if (cuotaCorrecta == false)
                { XtraMessageBox.Show("Por favor ingresa un numero de cuota valida", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

                cuotaCorrecta = ValidaCuota(txtTotalCuotas.Text);
                if(cuotaCorrecta == false)
                { XtraMessageBox.Show("Por favor ingresa un numero de cuota valida", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

                cuota = Convert.ToInt32(txtNumCuotas.Text);
                totalCuota = Convert.ToInt32(txtTotalCuotas.Text);
                if (cuota > totalCuota)
                { XtraMessageBox.Show("El numero de cuota no puede ser superior al total de cuotas", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);return; }               

                //FINALMENTE GENERAMOS LA CADENA A GUARDAR!!
                Cuotas = CadenaCuota(txtNumCuotas.Text, txtTotalCuotas.Text);                
            }

            tienAfp = trabajador.codAfp == 0 ? false: true ;            
            tieneSalud = trabajador.codSalud == 0 ? false : true;
            tieneCaja = trabajador.codCajaPrev == 0 ? false : true;

            numCorrecto = fnDecimal(txtValor.Text);
            if (numCorrecto == false)
            {
                lblvalue.Visible = true;
                lblvalue.Text = "valor no valido para campo valor";
                return;
            }

            //OBTENER FECHA DE NACIMIENTO DEL EMPLEADO
            nacimiento = trabajador.Nacimiento;
            //nacimiento = fechaNacimiento(getContrato(), PeriodoEmpleado);
            AdultoMayor = DateTime.Now.Date > nacimiento.AddYears(65);           
            MenorEdad = nacimiento.AddYears(18) > DateTime.Now.Date;
            adultoMujer = DateTime.Now.Date > nacimiento.AddYears(64);
            SexoTrabajador = Convert.ToInt16(trabajador.Sexo);         

            //SI SON DISTINTOS DE VACIO PODEMOS REALIZAR INSERT
            if (update == false)
            {
                //ES UN INSERT NUEVO              
                //OBTENER LOS DATOS DE ACUERDO A ITEM SELECCIONADO EN COMBOBOX ITEM     
                ItemBase itemb = new ItemBase(txtItem.EditValue.ToString());
                itemb.Setinfo();
                    //datosItem = fnDatosItem(txtItem.EditValue.ToString());
                    item = txtItem.EditValue.ToString();

                    //GUARDAMOS EN ARREGLO EL TIPO Y EL ORDEN
                    if (getContrato() != "" && getRut() != "")
                    {
                        rut = fnSistema.fnExtraerCaracteres(getRut());                                          

                        if (MenorEdad && (item == "SCEMPLE" || item == "SCEMPRE"))
                        {
                            lblAdulto.Visible = true;
                            lblAdulto.Text = "Para trabajadores menores de edad no se realiza calculo de seguro de cesantia";
                            return;
                        }                   

                        if (AdultoMayor && item == "SEGINV" && SexoTrabajador == 0)
                        {
                            lblAdulto.Visible = true;
                            lblAdulto.Text = "Para trabajadores mayores de 65 años no se realiza calculo Seguro Invalidez";
                            return;
                        }

                        if (adultoMujer && item == "SEGINV" && SexoTrabajador == 1)
                        {
                            lblAdulto.Visible = true;
                            lblAdulto.Text = "Para trabajadores mayores de 60 años no se realiza calculo Seguro Invalidez";
                            return;
                        }

                        if (tienAfp == false && item == "PREVISI")
                        {
                            lblAdulto.Visible = true;
                            lblAdulto.Text = "Este trabajador no usa Afp";
                            return;
                        }

                        if (tieneSalud == false && item == "SALUD")
                        {
                            lblAdulto.Visible = true;
                            lblAdulto.Text = "Este trabajador no usa salud";
                            return;
                        }                        

                        lblAdulto.Visible = false;                       
                        lblmsg.Visible = false;

                        //VERIFICAR QUE ITEM QUE SE INTENTA INGRESAR NO EXISTE EN ITEMtrabajador                        
                        bool ExisteItem = ItemTrabajador.Existe(txtItem.EditValue.ToString(), getContrato(), PeriodoEmpleado);
                        //bool ExisteItem = fnExisteItem(rut, getContrato(), txtItem.EditValue.ToString(), PeriodoEmpleado);                       
                        
                        if (ExisteItem)
                        {
                            //SI ES TRUE ES PORQUE YA EXISTE UN REGISTRO CON ESE PODEMOS PREGUNTAR
                            DialogResult pregunta = XtraMessageBox.Show("Ya existe un registro con ese item, ¿Desea ingresar de todas maneras?", "Pregunta",  MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (pregunta == DialogResult.Yes)
                            {                                
                                fnNuevoHistorico(txtItem, txtFormula, txtValor, txtnumItem, PeriodoEmpleado, itemb.Tipo, itemb.Orden, rut, getContrato(), Cuotas, cbUf, cbPesos, cbPorcentaje, cbSuspendido);
                        }
                        else
                        {
                            if (op.Cancela)
                            {
                                op.Cancela = false;
                                op.SetButtonProperties(btnNuevo, 1);
                                if(viewHis.RowCount > 0)
                                    fnCargarCampos();
                            }
                        }
                        }
                        else
                        {
                            //SOLO INGRESAMOS SIN PROBLEMAS                                                
                            fnNuevoHistorico(txtItem, txtFormula, txtValor, txtnumItem, PeriodoEmpleado, itemb.Tipo, itemb.Orden, rut, getContrato(), Cuotas, cbUf, cbPesos, cbPorcentaje, cbSuspendido);
                        }
                    }
                    else
                    {
                        XtraMessageBox.Show("Hay un error con el contrato", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }                       
            }
            else
            {
                //SI ES TRUE HACEMOS UPDATE DEL REGISTRO
                //OBTENER EL ID DESDE LA GRILLA
                if (viewHis.RowCount>0)
                {
                    itemGrilla = viewHis.GetFocusedDataRow()["coditem"] + "";
                    Numdb = Convert.ToInt32(viewHis.GetFocusedDataRow()["numitem"]);
                    
                    if (AlertaCambioItem(txtItem.EditValue.ToString()))
                    {                       
                        DialogResult alerta = XtraMessageBox.Show("Estás a punto de cambiar el item " + itemGrilla + ", ¿Está seguro?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (alerta == DialogResult.No)
                        {
                            return;
                        }
                    }

                        //OBTENER VALORES TIPO Y ORDEN
                        ItemBase itemb = new ItemBase(txtItem.EditValue.ToString());
                        itemb.Setinfo();
                        //datosItem = fnDatosItem(txtItem.EditValue.ToString());  
                    
                        if (getContrato() != "" && getRut() != "")
                        {
                            item = txtItem.EditValue.ToString();                                                

                            if (AdultoMayor && item == "SEGINV" && SexoTrabajador == 0)
                            {
                                lblAdulto.Visible = true;
                                lblAdulto.Text = "Para trabajadores mayores de 65 años no se realiza calculo Seguro Invalidez";
                                return;
                            }

                            if (adultoMujer && item == "SEGINV" && SexoTrabajador == 1)
                            {
                                lblAdulto.Visible = true;
                                lblAdulto.Text = "Para trabajadores mayores de 60 años no se realiza calculo Seguro Invalidez";
                                return;
                            }

                            if (MenorEdad && (item == "SCEMPLE" || item == "SCEMPRE"))
                            {
                                lblAdulto.Visible = true;
                                lblAdulto.Text = "Para trabajadores menores de edad no se realiza calculo seguro cesantia";
                                return;
                            }

                            if (viewHis.RowCount > 0)
                            {
                                item = viewHis.GetFocusedDataRow()["coditem"].ToString();
                                if (item == "SALUD" && tieneSalud == false)
                                {
                                    lblAdulto.Visible = true;
                                    lblAdulto.Text = "Este trabajador no usa Salud";
                                    return;
                                }
                                if (tienAfp == false && item == "PREVISI")
                                {
                                    lblAdulto.Visible = true;
                                    lblAdulto.Text = "Este trabajador no usa afp";
                                    return;
                                }                             
                            }

                            lblAdulto.Visible = false;                       
                            
                            //valido
                            rut = fnSistema.fnExtraerCaracteres(getRut());                                                                
                            fnModificarHistorico(txtItem, txtFormula, txtValor, txtnumItem, PeriodoEmpleado, itemb.Tipo, itemb.Orden, rut, getContrato(), Cuotas, itemGrilla, Numdb, cbUf, cbPesos, cbPorcentaje, cbSuspendido);
                        }                   
                }
            }
        }

        private void txtnumItem_KeyDown(object sender, KeyEventArgs e)
        {            

        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            //preguntamos si se presiono la tecla tab
            if (keyData == Keys.Tab)
            {
                //PREGUNTAMOS SI EL COMBO ITEM TIENE EL FOCO
                if (txtItem.ContainsFocus)
                {
                    if (txtItem.EditValue.ToString() == "")
                    {
                        lblmsg.Visible = true;
                        lblmsg.Text = "Debes seleccionar un item";
                        return false;
                    }                   
                }

                //PREGUNTAMOS SI LA CAJA VALOR ITEM TIENE EL FOCO
                ValidaValor();

                if (cbAplicaCuotas.Enabled == true && cbAplicaCuotas.Checked)
                {
                    bool valido = false;
                    if (txtNumCuotas.ContainsFocus)
                    {
                        if (txtNumCuotas.Text == "") { lblmsg.Visible = true;lblmsg.Text = "Por favor ingresa el numero de cuota";return false; }

                        valido = ValidaCuota(txtNumCuotas.Text);
          
                        if (valido == false)
                        { lblmsg.Visible = true; lblmsg.Text = "Por favor ingresa una cuota valida";return false;}

                        lblmsg.Visible = false;
                    }
                }
                if (cbAplicaCuotas.Enabled == true && cbAplicaCuotas.Checked)
                {
                    bool valido = false;
                    if (txtTotalCuotas.ContainsFocus)
                    {
                        if (txtTotalCuotas.Text == "") { lblmsg.Visible = true;lblmsg.Text = "Por favor ingrsa el total de cuotas"; return false;}

                        valido = ValidaCuota(txtTotalCuotas.Text);
                        if (valido == false)
                        { lblmsg.Visible = true; lblmsg.Text = "Por favor ingresa un valor valido para el total de cuotas"; return false; }

                        lblmsg.Visible = false;
                    }
                }
            }

            //TECLA ESCAPE?
            if (keyData == Keys.Escape)
            {
                //SOLO SI SE ESTÁ INGRESANDO UN NUEVO REGISTRO
                if (op.Cancela)
                {
                    op.Cancela = false;
                    op.SetButtonProperties(btnNuevo, 1);
                    fnCargarCampos();
                }   
            }
            return base.ProcessDialogKey(keyData);
        }

        private void ValidaValor()
        {
            if (txtValor.ContainsFocus)
            {
                if (txtValor.Text == "")
                {
                    lblvalue.Visible = true;
                    lblvalue.Text = "Debes ingresar un valor";
                    return ;
                }
                else
                {
                    //VERIFICAMOS QUE EL NUMERO TENGA LA ESTRUCTURA CORRECTA
                    bool correcto = false;
                    correcto = fnDecimal(txtValor.Text);
                    if (correcto == false)
                    {
                        lblvalue.Visible = true;
                        lblvalue.Text = "Valor no valido para campo valor";
                        return ;
                    }
                    else
                    {
                        lblvalue.Visible = false;
                    }
                }
            }
        }

        private void ValidaValorLostFocus()
        {
            if (txtValor.Text == "")
            {
                lblvalue.Visible = true;
                lblvalue.Text = "Debes ingresar un valor";
                return;
            }
            else
            {
                //VERIFICAMOS QUE EL NUMERO TENGA LA ESTRUCTURA CORRECTA
                bool correcto = false;
                correcto = fnDecimal(txtValor.Text);
                if (correcto == false)
                {
                    lblvalue.Visible = true;
                    lblvalue.Text = "Valor no valido para campo valor";
                    return;
                }
                else
                {
                    lblvalue.Visible = false;
                }
            }
        }

        private void txtValor_KeyDown(object sender, KeyEventArgs e)
        {
            //PREGUNTAMOS SI SE PRESIONO LA LETRA ENTER
            if (e.KeyCode == Keys.Enter)
            {
                ValidaValor();
            }
        }

        private void txtItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (txtItem.EditValue.ToString() == "")
                {
                    lblmsg.Visible = true;
                    lblmsg.Text = "Debes seleccionar un item";
                }
            }
        }

        private void txtnumItem_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (char.IsNumber(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void txtValor_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (e.KeyChar == (char)44)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();         

            //TRAER CODIGO DESDE GRILLA
            int numitemdb = 0;
            string itemGrilla = "", itembd = "";
            if (viewHis.RowCount>0)
            {
                numitemdb = int.Parse(viewHis.GetFocusedDataRow()["numitem"].ToString());                
                itembd = viewHis.GetFocusedDataRow()["coditem"].ToString();

                ItemBase itemb = new ItemBase(itembd);
                itemb.Setinfo();

                itemGrilla = itemb.Descripcion;
                //VERIFICAR SI ID EXISTE EN LA BASE DE DATOS
                fnEliminarHistorico(itemGrilla, numitemdb, itembd, getContrato(), PeriodoEmpleado);
            }
            else
            {
                XtraMessageBox.Show("Registro no valido", "Eliminar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void gridHis_Click(object sender, EventArgs e)
        {
            fnCargarCampos();
            //GUARDAR LA POSICION DE LA FILA SELECCIONADA
            filaSeleccionada = viewHis.FocusedRowHandle;
            op.Cancela = false;
            op.SetButtonProperties(btnNuevo, 1);
        }

        private void gridHis_KeyUp(object sender, KeyEventArgs e)
        {
            fnCargarCampos();
          
        }

        private void txtFormula_EditValueChanged(object sender, EventArgs e)
        {
            string formula = "";
            formula = txtFormula.EditValue.ToString();
            if (update == false)
            {
                if (formula != "0")
                {
                    //double valor = fnValorFormula(formula);
                    
                    //txtValor.Text = valor.ToString();
                }
                else
                {
                    txtValor.Text = "0";
                }
            }                  
        }

        private void panelitem_Paint(object sender, PaintEventArgs e)
        {

        }

        #endregion

        private void txtFormula_DoubleClick(object sender, EventArgs e)
        {

        }

        private void btnSueldoProp_Click(object sender, EventArgs e)
        {
           
        }

        private void btnliquidacion_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmliquidacion") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            string con = "";
            int per = 0;
            if (getContrato() != "") con = getContrato();

            //VERIFICAR QUE EXISTA AL MENOS subase y seguro invalidez

            frmPrevLiquidacion prev = new frmPrevLiquidacion(con, PeriodoEmpleado);
            prev.StartPosition = FormStartPosition.CenterParent;
            prev.ShowDialog();
        }

        private void txtItem_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtFormula_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtDesc_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtnumItem_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtValor_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }
        private void btnSalir_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            string item = "";
            int num = 0;
            if (viewHis.RowCount > 0)
            {
                num = Convert.ToInt32(viewHis.GetFocusedDataRow()["numitem"]);
                item = (string)viewHis.GetFocusedDataRow()["coditem"];
                
                if (CambiossinGuardar(PeriodoEmpleado, getContrato(), item, num))
                {
                    DialogResult adv = XtraMessageBox.Show("Hay cambios sin guardar, ¿Deseas cerrar de todas maneras?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (adv == DialogResult.Yes)
                        Close();
                }
                else
                    Close();
            }
            else
                Close();
        }

        private void cbAplicaCuotas_CheckedChanged(object sender, EventArgs e)
        {
            //SI ESTA SELECCIONADO HABILITAMOS TEXTBOX CUOTAS
            if (cbAplicaCuotas.Checked)
            {
                txtNumCuotas.Enabled = true;
                txtTotalCuotas.Enabled = true;
                txtNumCuotas.Text = "";
                txtTotalCuotas.Text = "";
                txtNumCuotas.Focus();
            }
            else
            {
                txtNumCuotas.Enabled = false;
                txtTotalCuotas.Enabled = false;
                txtNumCuotas.Text = "";
                txtTotalCuotas.Text = "";
            }
        }

        private void txtNumCuotas_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void txtTotalCuotas_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void btnDetalle_Click(object sender, EventArgs e)
        {
            frmLiquidacionDetalle liq = new frmLiquidacionDetalle(getContrato(), PeriodoEmpleado);
            liq.StartPosition = FormStartPosition.CenterParent;
            liq.ShowDialog();
            
        }      

        private void btnDetalle_Click_1(object sender, EventArgs e)
        {
            frmLiquidacionDetalle det = new frmLiquidacionDetalle(contrato, PeriodoEmpleado);
            det.StartPosition = FormStartPosition.CenterParent;
            det.ShowDialog();
        }

        private void txtNumCuotas_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtTotalCuotas_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void cbPorcentaje_CheckedChanged(object sender, EventArgs e)
        {
            if (cbPorcentaje.Checked)
            {
                txtValor.Text = "7";
                txtValor.Focus();
                cbPesos.Checked = false;
                cbUf.Checked = false;
            }
            else
            {
                txtValor.Text = "0";
            }
        }

        private void cbUf_CheckedChanged(object sender, EventArgs e)
        {
            if (cbUf.Checked)
            {
                cbPesos.Checked = false;
                cbPorcentaje.Checked = false;
                txtValor.Focus();
            }
        }

        private void cbPesos_CheckedChanged(object sender, EventArgs e)
        {
            if (cbPesos.Checked)
            {
                cbPorcentaje.Checked = false;
                cbUf.Checked = false;
                txtValor.Focus();
            }
        }

        private void cbTope_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void cbProporcional_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void cbpermanente_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void toolTipController1_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            //TOOLTIP PARA FILA GRIDCONTROL 
            if (e.SelectedControl != gridHis)
                return;

            GridHitInfo hitInfo = viewHis.CalcHitInfo(e.ControlMousePosition);

            if (hitInfo.InRow == false)
                return;

            SuperToolTipSetupArgs toolTipArgs = new SuperToolTipSetupArgs();
            //toolTipArgs.Title.Text = "";

            //GET DATA FROM THIS ROW
            DataRow drCurrentRow = viewHis.GetDataRow(hitInfo.RowHandle);
            if (drCurrentRow != null)
            {
                string BodyText = $"Coditem: {drCurrentRow["coditem"] + Environment.NewLine} Original:{drCurrentRow["valor"] + Environment.NewLine} Valor Calculado:{Math.Round(Convert.ToDouble(drCurrentRow["valorcalculado"]))}";
                toolTipArgs.Contents.Text = BodyText;
            }

            e.Info = new ToolTipControlInfo();
            e.Info.Object = hitInfo.HitTest.ToString() + hitInfo.RowHandle.ToString();
            e.Info.ToolTipType = ToolTipType.SuperTip;
            e.Info.SuperTip = new SuperToolTip();
            e.Info.SuperTip.Setup(toolTipArgs);
        }

        private void viewHis_KeyDown(object sender, KeyEventArgs e)
        {
            string code = "", itemGrilla = "";
            int numitemdb = 0;
            bool Esclase = false;
            //ELIMINAR UNA FILA USANDO LA TECLA SUPRIMIR
            if (e.KeyCode == Keys.Delete)
            {
                if (viewHis.RowCount > 0)
                {
                    code = viewHis.GetFocusedDataRow()["coditem"].ToString();
                    numitemdb = Convert.ToInt32(viewHis.GetFocusedDataRow()["numitem"]);
                    Esclase = Convert.ToBoolean(viewHis.GetFocusedDataRow()["esclase"]);
                    if (!Esclase)
                    {
                        ItemBase itemb = new ItemBase(code);
                        itemb.Setinfo();

                        itemGrilla = itemb.Descripcion;
                        if (code != "")
                        {
                            fnEliminarHistorico(itemGrilla, numitemdb, code, getContrato(), PeriodoEmpleado);
                        }
                        else
                        {
                            XtraMessageBox.Show("Registro no válido", "Registro", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        }
                    }                   
                }
                else
                {
                    XtraMessageBox.Show("Registro no válido", "Registro", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
        }

        private void txtValor_Leave(object sender, EventArgs e)
        {
            ValidaValorLostFocus();
        }
    }
   
}