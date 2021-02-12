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
using DevExpress.XtraReports.UI;
using System.Data.SqlClient;
using System.Reflection;
using DevExpress.Data.XtraReports.Wizard;
using DevExpress.XtraPrinting.Preview;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Base;
using System.Collections;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.Utils;
using System.IO;

namespace Labour
{
    public partial class frmPrevLiquidacion : DevExpress.XtraEditors.XtraForm
    {
        //PARA GUARDAR EL RUT DEL EMPLEADO
        string contrato = "";
        //PARA GUARDAR EL PERIODO DEL EMPLEADO
        int PeriodoEmpleado = 0;

        NavegacionT nav;

        //DATOS TRABAJADOR
        Persona Trabajador;
        

        public frmPrevLiquidacion(string contrato, int periodo)
        {
            InitializeComponent();
            this.contrato = contrato;
            PeriodoEmpleado = periodo;            
        }
        public frmPrevLiquidacion()
        {
            InitializeComponent();
        }

        private void frmPrevLiquidacion_Load(object sender, EventArgs e)
        {

            Cursor.Current = Cursors.WaitCursor;

            if (contrato != "" && PeriodoEmpleado != 0)
            {
                nav = new NavegacionT(contrato, PeriodoEmpleado);
                
                double pago = 0;

                Trabajador = new Persona();
                Trabajador = Persona.GetInfo(contrato, PeriodoEmpleado);

                Haberes hab = new Haberes(contrato, PeriodoEmpleado);
                pago = Calculo.GetValueFromCalculoMensaul(contrato, PeriodoEmpleado, "syspago");

                //SOLO RECALCULAMOS PARA EL PERIODO ACTUAL   
                //if (PeriodoEmpleado == Calculo.PeriodoObservado)
                //   hab.CalculoLiquidacion();                   

                /*if (pago <= 0)
                {
                    XtraMessageBox.Show("El sueldo a pagar no puede ser igual o inferior a cero, por favor verifica la informacion proporcionada e intentelo de nuevo", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Dispose();
                    this.Close();
                    return;
                }*/

                if (PeriodoEmpleado == Calculo.PeriodoObservado)
                {
                    hab.CalculoLiquidacion();
                    hab.CalculoLiquidacion();                 

                    ////VERIFICAR SI TIENE EL ITEM SUELDO BASE
                    //if (TieneItemBase(contrato, PeriodoEmpleado, "SUBASE") == false)
                    //{
                    //    XtraMessageBox.Show("No haz ingresado sueldo base", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //    this.Dispose();
                    //    this.Close();
                    //    return;
                    //}

                    //if (TieneItemBase(contrato, PeriodoEmpleado, "PREVISI") == false)
                    //{
                    //    XtraMessageBox.Show("No haz ingresado Afp", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //    this.Dispose();
                    //    this.Close();
                    //    return;
                    //}

                    //if (TieneItemBase(contrato, PeriodoEmpleado, "SALUD") == false)
                    //{
                    //    XtraMessageBox.Show("No haz ingresado Salud", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //    this.Dispose();
                    //    this.Close();
                    //    return;
                    //}

                    //if (TieneItemBase(contrato, PeriodoEmpleado, "GRATIFI") == false)
                    //{
                    //    XtraMessageBox.Show("No haz ingresado gratificación", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //    this.Dispose();
                    //    this.Close();
                    //    return;
                    //} 
                }


                lbldias.Text = Calculo.GetValueFromCalculoMensaul(contrato, PeriodoEmpleado, "sysdiastr").ToString();

                //SETAMOS DATA TRABAJADOR
                lblNombre.Text = "Nombre: " + Trabajador.NombreCompleto;
                lblRut.Text = "Rut: " + fnSistema.fFormatearRut2(Trabajador.Rut);
                lblIngreso.Text = "Fecha Ingreso: " + fnSistema.FechaFormato(Trabajador.Ingreso);
                lblCargo.Text = "Cargo: " + Trabajador.Cargo;
                lblCentroCosto.Text = "Centro Costo: " + Trabajador.centro;
                lblSucursal.Text = "Sucursal: " + Trabajador.sucursal;
                //ConsultaDatosEmpleado(contrato, PeriodoEmpleado);

                lblLic.Text = varSistema.ObtenerValorLista("sysdiaslic") + "";
                lblContrato.Text = "Contrato:" + contrato;
                lblEvaluado.Text = "Periodo: " + fnSistema.PrimerMayuscula(fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(PeriodoEmpleado)));
                //CargarGrid();
                
                CargarListadoAnteriores();

                //if (PeriodoEmpleado != Calculo.PeriodoObservado)
                //    CargarListadoAnteriores();
                //else
                    //CargarListado();
            }
        }

        #region "MANEJO DE DATOS"
        //CONSULTA DATOS EMPLEADO
        private void ConsultaDatosEmpleado(string contrato, int Periodo)
        {     
            string sql = "SELECT concat(trabajador.nombre, ' ', apepaterno, ' ', apematerno) as name, " +
                        "cargo.nombre as cargo, rut, ingreso, ccosto.nombre as ccosto, sucursal.descSucursal as nombreSucursal" +
                        " FROM trabajador" +
                        " INNER JOIN cargo ON cargo.id = trabajador.cargo" +
                        " INNER JOIN ccosto ON trabajador.ccosto = ccosto.id" +
                        " INNER JOIN sucursal ON trabajador.sucursal = sucursal.codSucursal" +
                        " WHERE contrato=@pcontrato AND anomes=@periodo";

            SqlCommand cmd;
            SqlDataReader rd;
            DateTime ingr = DateTime.Now.Date;

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pcontrato", contrato));
                        cmd.Parameters.Add(new SqlParameter("@periodo", Periodo));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {                              
                                lblCargo.Text = "Cargo: " + (string)rd["cargo"];                             
                                lblNombre.Text = "Nombre: " + (string)rd["name"];
                                lblRut.Text = "Rut: " + fnSistema.fFormatearRut2((string)rd["rut"]);
                                lblCentroCosto.Text = "Centro Costo: " + (string)rd["ccosto"];
                                lblSucursal.Text = "Sucursal: " + (string)rd["nombreSucursal"];
                                ingr = (DateTime)rd["ingreso"];
                                lblIngreso.Text = "Fecha ingreso: " + fnSistema.FechaFormato(ingr);
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
        }

        //COLUMNAS GRILLA
        private void ColumnasGrilla()
        {
            gridliquidacion.ToolTipController = toolTipGrilla;
            viewliquidacion.Columns[0].Caption = "Detalle";
            viewliquidacion.Columns[0].Width = 220;

            viewliquidacion.Columns[1].Caption = "v.o";
            viewliquidacion.Columns[1].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            viewliquidacion.Columns[1].DisplayFormat.FormatString = "n2";
            viewliquidacion.Columns[1].Width = 70;
            //viewliquidacion.Columns[1].AppearanceCell.Font = new Font("Arial", 9, FontStyle.Bold);

            viewliquidacion.Columns[2].Caption = "Haberes";
            viewliquidacion.Columns[2].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            viewliquidacion.Columns[2].DisplayFormat.FormatString = "n0";
            viewliquidacion.Columns[2].Width = 70;

            viewliquidacion.Columns[3].Caption = "Descuentos";
            viewliquidacion.Columns[3].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            viewliquidacion.Columns[3].DisplayFormat.FormatString = "n0";
            viewliquidacion.Columns[3].Width = 70;
        }

        //CARGAR GRID VIEW
        private void CargarGrid()
        {
            SqlDataAdapter ad = new SqlDataAdapter();
            DataSet ds = new DataSet();
            string sql = "SELECT item, original, calculado FROM calculo WHERE contrato='156081825'";
            try
            {
                if (fnSistema.ConectarSQLServer())
                {

                    SqlCommand cmd = new SqlCommand(sql, fnSistema.sqlConn);
                    ad.SelectCommand = cmd;
                    ad.Fill(ds);
                    fnSistema.sqlConn.Close();

                    if (ds.Tables[0].Rows.Count>0)
                    {
                        gridliquidacion.DataSource = ds.Tables[0];
                    }
                    else
                    {
                        gridliquidacion.DataSource = null;
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

        }

        //CARGAR PRELISTA (PARA GRILLA)
        private void CargarListado(bool? muestraAnterior = false)
        {
            List<tablaLiquidacion> listado = new List<tablaLiquidacion>();       

            string sqlCalculo = "SELECT calculo.contrato, calculo.orden, calculo.item, original, calculado, " +
                          "calculo.numitem, tipo, cuota, uf, pesos, porc from calculo " +
                            "INNER JOIN itemtrabajador ON itemTrabajador.contrato = calculo.contrato " +
                            "AND itemTrabajador.anomes = calculo.anomes AND itemTrabajador.coditem = calculo.item " +
                            " AND itemtrabajador.numitem = calculo.numitem " +
                            "WHERE calculo.contrato = @contrato AND calculo.anomes = @periodo " +
                            "ORDER BY tipo, orden";

            int tipo = 0, cAportes = 0;
            string[] arreglo = new string[2];            
            double original = 0, calculado = 0, syscicese = 0, sysfscese = 0;
            string item = "", cadImpuesto = "", cadAfp = "", cuotas = "", cadCuotas = "";
            SqlCommand cmd;
            SqlDataReader rd;

            tablaLiquidacion d1 = new tablaLiquidacion();
            d1.Item = "";
            tablaLiquidacion d2 = new tablaLiquidacion();
            d2.Item = "";
            tablaLiquidacion d3 = new tablaLiquidacion();
            d3.Item = "";

            //OBTENER CADENA PARA IMPUESTO
            cadImpuesto = ImpuestoEmpleado();
        
            //CADENA PARA AFP
            //cadAfp = PorcentajeAfp(true, true);

            syscicese = Calculo.GetValueFromCalculoMensaul(contrato, PeriodoEmpleado, "syscicese");
            sysfscese = Calculo.GetValueFromCalculoMensaul(contrato, PeriodoEmpleado, "sysfscese");

            double Licencias = Calculo.GetValueFromCalculoMensaul(contrato, PeriodoEmpleado, "sysdiasLic");
            lblLic.Text = Licencias + "";
            
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sqlCalculo, fnSistema.sqlConn))
                    {
                        cmd.Parameters.Add(new SqlParameter("@periodo", PeriodoEmpleado));
                        cmd.Parameters.Add(new SqlParameter("@contrato", contrato));
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //PRIMER ELEMENTO ES HABERES IMPONIBLES                           
                            while (rd.Read())
                            {                                

                                arreglo = tipoItem((string)rd["item"]);
                                tipo = Convert.ToInt32(arreglo[0]);
                                //cuotas = NumeroCuotas((string)rd["item"], (int)rd["numitem"],fnSistema.sqlConn);                                
                                item = arreglo[1];
                                original = Convert.ToDouble((decimal)rd["original"]);                             
                                calculado = Convert.ToDouble((decimal)rd["calculado"]);                               

                                if (tipo == 1)
                                {
                                    //HABERES IMPONIBLES
                                   
                                    if ((string)rd["cuota"] != "0")
                                        item = item + " " + ItemTrabajador.GetCadenaCuota((string)rd["cuota"]);

                                    listado.Add(new tablaLiquidacion() { Item = item, vo = original, valorHaber = calculado });
                                                                        
                                }
                                else if (tipo == 2)
                                {                                   
                                    //HABERES EXENTOS
                                    listado.Add(new tablaLiquidacion() { Item = item, vo = original , valorHaber = calculado});
                                  
                                }
                                else if (tipo == 3)
                                {
                                   //listado.Add(new tablaLiquidacion() { Item = "ASIGNACIONES FAMILIARES" });

                                    //FAMILIARES
                                    listado.Add(new tablaLiquidacion() { Item = item, vo = original, valorHaber = calculado });
                                    
                                }
                                else if (tipo == 4 && (rd["item"].ToString() != "SCEMPRE" && rd["item"].ToString() != "SEGINV"))
                                {                                    
                                    //DESCUENTOS LEGALES (LEYES SOCIALES)
                                    original = 0;

                                    if (rd["item"].ToString() == "PREVISI")
                                        item = cadAfp;
                                    if (rd["item"].ToString() == "IMPUEST" && calculado != 0)
                                        item = cadImpuesto;
                                    if (rd["item"].ToString() == "IMPUEST" && calculado == 0)
                                    { item = "EXENTO DE IMPUESTO"; }
                                    if (rd["item"].ToString() == "SALUD")
                                    {                                        
                                        //item = SaludEmpleado(Convert.ToDouble((decimal)rd["original"]));
                                        //item = GetCadenaSalud((bool)rd["porc"], (bool)rd["uf"], (bool)rd["pesos"], Convert.ToDouble(rd["original"]), Licencias, Data);                                    
                                    }
                                    
                                    if(rd["item"].ToString() == "IMPUEST")
                                        listado.Add(new tablaLiquidacion() { Item = item, valorDescuento = calculado });                                  
                                    else if(calculado != 0)
                                        listado.Add(new tablaLiquidacion() { Item = item, valorDescuento = calculado });
                                }
                                else if (tipo == 5)
                                {
                                    //DESCUENTOS          
                                    if ((string)rd["cuota"] != "0")                               
                                        cadCuotas = item + " " + ItemTrabajador.GetCadenaCuota((string)rd["cuota"]);                                                              
                                    else                             
                                       cadCuotas = item;                                
                                                                
                                   listado.Add(new tablaLiquidacion() { Item = cadCuotas, valorDescuento = calculado });                                    
                                }
                                else if (tipo == 6)
                                {
                                    //CONTRIBUCIONES
                                    //..                           
                        
                                    listado.Add(new tablaLiquidacion() { Item = item, vo = calculado});                                    
                                }
                                else
                                {
                                    //CEMPRE Y SIS
                                    // listado.Add(new tablaLiquidacion() {Item = item, vo = calculado });
                                    if (rd["item"].ToString() == "SCEMPRE")
                                    {                                     
                                        if(syscicese != 0)
                                            d1.Item = item;d1.vo = syscicese;

                                        d2.Item = item;d2.vo = sysfscese;                                      
                                    }
                                    if (rd["item"].ToString() == "SEGINV")
                                    {
                                        d3.Item = item;d3.vo = calculado;
                                    }
                                    cAportes++;
                                }                              
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
         
            if(d1.Item != "")
            listado.Add(d1);
            if(d2.Item != "")
            listado.Add(d2);
            if(d3.Item != "")
            listado.Add(d3);

            gridliquidacion.DataSource = listado;
            viewliquidacion.OptionsCustomization.AllowFilter = false;
            viewliquidacion.OptionsCustomization.AllowSort = false;
            viewliquidacion.OptionsMenu.EnableFooterMenu = false;
            viewliquidacion.OptionsMenu.EnableGroupPanelMenu = false;
            viewliquidacion.OptionsMenu.EnableColumnMenu = false;
            viewliquidacion.OptionsCustomization.AllowColumnMoving = false;

            ColumnasGrilla();

           // txtHaberes.Text = varSistema.ObtenerValorLista("systhab").ToString("N0");
           // txtDescuentos.Text = varSistema.ObtenerValorLista("systdctos").ToString("N0");
           // txtImponible.Text = varSistema.ObtenerValorLista("systimp").ToString("N0");
           // txtLiquido.Text = varSistema.ObtenerValorLista("sysliq").ToString("N0");
           // txtPago.Text = varSistema.ObtenerValorLista("syspago").ToString("N0");

            /*lblHab.Text = "$" + varSistema.ObtenerValorLista("systhab").ToString("N0");
            lblDctos.Text ="$" + varSistema.ObtenerValorLista("systdctos").ToString("N0");
            lblImp.Text = "$" + varSistema.ObtenerValorLista("systimp").ToString("N0");
            lblLiq.Text = "$" + varSistema.ObtenerValorLista("sysliq").ToString("N0");
            lblPago.Text ="$" + varSistema.ObtenerValorLista("syspago").ToString("N0");*/

            lblHab.Text = "$" + Calculo.GetValueFromCalculoMensaul(contrato, PeriodoEmpleado, "systhab").ToString("N0");
            lblDctos.Text = "$" + Calculo.GetValueFromCalculoMensaul(contrato, PeriodoEmpleado, "systdctos").ToString("N0");
            lblImp.Text = "$" + Calculo.GetValueFromCalculoMensaul(contrato, PeriodoEmpleado, "systimp").ToString("N0");
            lblLiq.Text = "$" + Calculo.GetValueFromCalculoMensaul(contrato, PeriodoEmpleado, "sysliq").ToString("N0");
            lblPago.Text = "$" + Calculo.GetValueFromCalculoMensaul(contrato, PeriodoEmpleado, "syspago").ToString("N0");            

        }

        //PARA PERIODOS ANTERIORES
        private void CargarListadoAnteriores()
        {
            string sqlItem = "SELECT contrato, itemTrabajador.orden, itemTrabajador.coditem as item, " +
                            "valor as original, valorcalculado as calculado,  " +
                            "numitem, itemTrabajador.tipo, descripcion, cuota, porc, uf, pesos, imprimebase, " +
                             "splab13, splab14, " +
                            "ISNULL((SELECT imponible FROM liquidacionHistorico liq where liq.contrato = itemtrabajador.contrato AND " +
                            "liq.anomes=@pAnterior), 0) as impanterior, " +
                            " (select systhab FROM calculomensual c WHERE c.contrato = itemtrabajador.contrato AND c.anomes=itemtrabajador.anomes) as systhab, " + 
                            " (select systimp FROM calculomensual c WHERE c.contrato = itemtrabajador.contrato AND c.anomes = itemtrabajador.anomes) as systimp, " +
                            " (select systdctos FROM calculomensual c WHERE c.contrato = itemtrabajador.contrato AND c.anomes = itemtrabajador.anomes) as systdctos, " + 
                            " (select sysliq FROM calculomensual c WHERE c.contrato = itemtrabajador.contrato AND c.anomes = itemtrabajador.anomes) as sysliq, " +
                            " (select syspago FROM calculomensual c WHERE c.contrato = itemtrabajador.contrato AND c.anomes = itemtrabajador.anomes) as syspago, " +
                            "(select sysfactorimpto FROM calculomensual c WHERE c.contrato = itemtrabajador.contrato AND c.anomes=itemtrabajador.anomes) as sysfactorimpto, " +
                            "(select systributo FROM calculomensual c WHERE c.contrato = itemtrabajador.contrato AND c.anomes = itemtrabajador.anomes) as systributo, " +
                            "(select sysrebimpto FROM calculomensual c WHERE c.contrato = itemtrabajador.contrato AND c.anomes = itemtrabajador.anomes) as sysrebimpto, " +
                            "(select syscicese FROM calculomensual c WHERE c.contrato = itemtrabajador.contrato AND c.anomes=itemtrabajador.anomes) as syscicese, " +
                            "(select sysfscese FROM calculomensual c WHERE c.contrato = itemtrabajador.contrato AND c.anomes = itemtrabajador.anomes) as sysfscese, " +
                            "(select syscicest FROM calculomensual c WHERE c.contrato = itemtrabajador.contrato AND c.anomes = itemtrabajador.anomes) as syscicest,  " +
                            "(select sysdiaslic FROM calculomensual c WHERE c.contrato = itemtrabajador.contrato AND c.anomes = itemtrabajador.anomes) as sysdiaslic, " +
                            " (select ROUND(systopeafp, 0) FROM calculomensual c WHERE c.contrato = itemtrabajador.contrato AND c.anomes=itemtrabajador.anomes) as systopeafp, " +
                            "(select sysporcadmafp FROM calculomensual c WHERE c.contrato = itemtrabajador.contrato AND c.anomes=itemtrabajador.anomes) as sysporcadmafp, " +
                            "(select sysdiastr FROM calculomensual c WHERE c.contrato = itemtrabajador.contrato AND c.anomes=itemtrabajador.anomes) as sysdiastr, " +
                            "(select isa.nombre FROM trabajador t inner join isapre isa on t.salud = isa.id WHERE t.contrato = itemTrabajador.contrato AND itemtrabajador.anomes = t.anomes) as Salud, " +
                            "(select isa.id FROM trabajador t inner join isapre isa on t.salud = isa.id WHERE t.contrato = itemTrabajador.contrato AND itemtrabajador.anomes = t.anomes) as CodSalud, " +
                            "(select sysdiassp13 FROM calculomensual c WHERE c.contrato = itemtrabajador.contrato AND c.anomes=itemtrabajador.anomes) as sysdiassp13,  " +
                            " (select sysdiassp14 FROM calculomensual c WHERE c.contrato = itemtrabajador.contrato AND c.anomes = itemtrabajador.anomes) as sysdiassp14  " +
                            " FROM itemtrabajador " +                            
                            "INNER JOIN item ON item.coditem = itemTrabajador.coditem " +
                            "WHERE contrato = @contrato AND anomes = @periodo " +
                            "AND previsualizar=1 AND informacion=0 AND suspendido = 0 " +
                            "ORDER by tipo, orden ";
            
            SqlCommand cmd;
            SqlDataReader rd;
            SqlConnection cn;
            Hashtable data = new Hashtable();
            string item = "", cadImpuesto = "", cadAfp = "", cadAfpSuspension="", cuotas = "", cadCuotas = "";
            double original = 0, calculado = 0, SegIndividualEmp = 0, SegFondoEmp = 0, Licencias = 0, SegCiTrab = 0, sanna = 0;
            int tipo = 0, count = 0;
            double systimp = 0, systhab = 0, systdctos = 0, syspago = 0, sysliq = 0, sysfactorimpto = 0;
            double systributo = 0, sysrebimpto = 0, syscicese = 0, sysfscese = 0, syscicest = 0, sysdiaslic = 0;
            double TopeAfpLiq = 0, sysporcadmafp = 0, sysdiastr = 0, ImpAnterior = 0, sysdiassp13 = 0, sysdiassp14 = 0;
            string CadenaSalud = "";
            bool esSuspension = false, sp13 = false, sp14 = false;

            List<tablaLiquidacion> listado = new List<tablaLiquidacion>();

            tablaLiquidacion d1 = new tablaLiquidacion();
            d1.Item = "";
            tablaLiquidacion d2 = new tablaLiquidacion();
            d2.Item = "";
            tablaLiquidacion d3 = new tablaLiquidacion();
            d3.Item = "";
            //suspension seginv
            tablaLiquidacion d4 = new tablaLiquidacion();
            d4.Item = "";
            tablaLiquidacion d5 = new tablaLiquidacion();
            d5.Item = "";
            tablaLiquidacion d6 = new tablaLiquidacion();
            d6.Item = "";
            tablaLiquidacion d7 = new tablaLiquidacion();
            d7.Item = "";

            //OBTENER CADENA PARA IMPUESTO
            //cadImpuesto = ImpuestoEmpleado();
            //cadImpuesto = "Impto " + Calculo.GetValueFromCalculoMensaul(contrato, PeriodoEmpleado, "sysfactorimpto") * 100 + "%" +
            //    " de: " + Calculo.GetValueFromCalculoMensaul(contrato, PeriodoEmpleado, "systributo").ToString("N0") +
            //    " -Rebaja:" + Calculo.GetValueFromCalculoMensaul(contrato, PeriodoEmpleado, "sysrebimpto").ToString("N2");

            //CADENA PARA AFP
            //cadAfp = PorcentajeAfp((Trabajador.Regimen == 4 || Trabajador.Regimen == 3 || Trabajador.Regimen == 5)? true : false, true);        

            //SEGURO DE INVALIDEZ CUENTA INDIVIDUAL EMPRESA
            //SegIndividualEmp = Calculo.GetValueFromCalculoMensaul(contrato, PeriodoEmpleado, "syscicese");

            //SEGURO DE INVALIDEZ FONDO SOLIDARIO EMPRESA
            //SegFondoEmp = Calculo.GetValueFromCalculoMensaul(contrato, PeriodoEmpleado, "sysfscese");

            //SEGURO CESANTIA CUENTA INDIVIDUAL TRABAJADOR
            //SegCiTrab = Calculo.GetValueFromCalculoMensaul(contrato, PeriodoEmpleado, "syscicest");

            //LICENCIAS
            //Licencias = Calculo.GetValueFromCalculoMensaul(contrato, PeriodoEmpleado, "sysdiaslic");

            //VALOR SANNA
            //sanna = Calculo.GetValueFromCalculoMensaul(contrato, PeriodoEmpleado, "sysvalsanna");



            //data = SaludEmpleadoAnterior();

            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sqlItem, cn))
                        {
                            cmd.Parameters.Add(new SqlParameter("@periodo", PeriodoEmpleado));
                            cmd.Parameters.Add(new SqlParameter("@contrato", contrato));
                            cmd.Parameters.Add(new SqlParameter("@pAnterior", fnSistema.fnObtenerPeriodoAnterior(PeriodoEmpleado)));
                            rd = cmd.ExecuteReader();
                            if (rd.HasRows)
                            {
                                //PRIMER ELEMENTO ES HABERES IMPONIBLES                    
                                while (rd.Read())
                                {
                                    count++;
                                    //Solo para guardar los valores totales
                                    if (count == 1)
                                    {
                                        systimp = Convert.ToDouble(rd["systimp"]);
                                        systhab = Convert.ToDouble(rd["systhab"]);
                                        systdctos = Convert.ToDouble(rd["systdctos"]);
                                        sysliq = Convert.ToDouble(rd["sysliq"]);
                                        syspago = Convert.ToDouble(rd["syspago"]);
                                        TopeAfpLiq = Convert.ToDouble(rd["systopeafp"]);

                                        //Para cadena Impuesto
                                        sysfactorimpto = Convert.ToDouble(rd["sysfactorimpto"]);
                                        systributo = Convert.ToDouble(rd["systributo"]);
                                        sysrebimpto = Convert.ToDouble(rd["sysrebimpto"]);

                                        syscicese = Convert.ToDouble(rd["syscicese"]);
                                        sysfscese = Convert.ToDouble(rd["sysfscese"]);
                                        syscicest = Convert.ToDouble(rd["syscicest"]);

                                        sysdiaslic = Convert.ToDouble(rd["sysdiaslic"]);
                                        sysdiastr = Convert.ToDouble(rd["sysdiastr"]);
                                        sysporcadmafp = Convert.ToDouble(rd["sysporcadmafp"]);
                                        sysdiassp13 = Convert.ToDouble(rd["sysdiassp13"]);
                                        sysdiassp14 = Convert.ToDouble(rd["sysdiassp14"]);                                      

                                        ImpAnterior = Convert.ToDouble(rd["impanterior"]);

                                        //Cadena para impuesto
                                        cadImpuesto = "Impto " + sysfactorimpto * 100 + "%" +
                                         " de: " + systributo.ToString("N0") +
                                        " -Rebaja:" + sysrebimpto.ToString("N2");

                                        //SEGURO DE INVALIDEZ CUENTA INDIVIDUAL EMPRESA
                                        SegIndividualEmp = syscicese;
                                        //SEGURO DE INVALIDEZ FONDO SOLIDARIO EMPRESA
                                        SegFondoEmp = sysfscese;
                                        //SEGURO CESANTIA CUENTA INDIVIDUAL TRABAJADOR
                                        SegCiTrab = syscicest;
                                        //LICENCIAS
                                        Licencias = sysdiaslic;

                                    }

                                    esSuspension = (Convert.ToBoolean(rd["splab13"]) == true || Convert.ToBoolean(rd["splab14"]) == true) ? true : false;
                                    sp13 = Convert.ToBoolean(rd["splab13"]);
                                    sp14 = Convert.ToBoolean(rd["splab14"]);

                                    tipo = Convert.ToInt32(rd["tipo"]);
                                    //cuotas = NumeroCuotas((string)rd["item"], (int)rd["numitem"], fnSistema.sqlConn);
                                    item = (string)rd["descripcion"];

                                    original = Convert.ToDouble((decimal)rd["original"]);
                                    calculado = Convert.ToDouble((decimal)rd["calculado"]);

                                    if (tipo == 1)
                                    {
                                        //HABERES IMPONIBLES                                   
                                        if ((string)rd["cuota"] != "0")
                                            item = item + " " + ItemTrabajador.GetCadenaCuota((string)rd["cuota"]);

                                        if(Convert.ToBoolean(rd["imprimebase"]))
                                            listado.Add(new tablaLiquidacion() { Item = item, vo = original, valorHaber = calculado });
                                        else
                                            listado.Add(new tablaLiquidacion() { Item = item, vo = 0, valorHaber = calculado });

                                    }
                                    else if (tipo == 2)
                                    {
                                        //HABERES EXENTOS           

                                        listado.Add(new tablaLiquidacion() { Item = item, vo = original, valorHaber = calculado });

                                    }
                                    else if (tipo == 3)
                                    {
                                        //listado.Add(new tablaLiquidacion() { Item = "ASIGNACIONES FAMILIARES" });

                                        //FAMILIARES

                                        listado.Add(new tablaLiquidacion() { Item = item, vo = original, valorHaber = calculado });

                                    }
                                    else if (tipo == 4 && (rd["item"].ToString() != "SCEMPRE" && rd["item"].ToString() != "SEGINV"))
                                    {
                                        //DESCUENTOS LEGALES (LEYES SOCIALES)
                                        original = 0;

                                        if (rd["item"].ToString() == "PREVISI")
                                        {
                                            if (esSuspension)
                                            {
                                                cadAfp = fnSistema.PorcentajeAfp(Trabajador.Contrato, Trabajador.PeriodoPersona, (Trabajador.Regimen == 4 || Trabajador.Regimen == 3 || Trabajador.Regimen == 5) ? true : false, systimp, TopeAfpLiq, sysdiaslic, sysdiastr, sysporcadmafp, ImpAnterior, true, true, sp13, sp14);
                                            }
                                            else
                                            {
                                                cadAfp = fnSistema.PorcentajeAfp(Trabajador.Contrato, Trabajador.PeriodoPersona,(Trabajador.Regimen == 4 || Trabajador.Regimen == 3 || Trabajador.Regimen == 5) ? true : false, systimp, TopeAfpLiq, sysdiaslic, sysdiastr, sysporcadmafp, ImpAnterior, true);
                                            }

                                            item = cadAfp;
                                        }                                           
                                        if (rd["item"].ToString() == "IMPUEST" && calculado != 0)
                                            item = cadImpuesto;
                                        if (rd["item"].ToString() == "IMPUEST" && calculado == 0)
                                        { item = "EXENTO DE IMPUESTO"; }
                                        if (rd["item"].ToString() == "SALUD")
                                        {
                                            if (esSuspension)
                                            {
                                                CadenaSalud =  GetCadenaSalud((bool)rd["porc"], (bool)rd["uf"], (bool)rd["pesos"], Convert.ToDouble(rd["original"]), Licencias, Convert.ToString(rd["Salud"]), Convert.ToInt32(rd["CodSalud"]), ImpAnterior, sp13, sp14, true);
                                            }
                                            else
                                            {
                                                CadenaSalud = GetCadenaSalud((bool)rd["porc"], (bool)rd["uf"], (bool)rd["pesos"], Convert.ToDouble(rd["original"]), Licencias, Convert.ToString(rd["Salud"]), Convert.ToInt32(rd["CodSalud"]), ImpAnterior);
                                            }
                                            item = CadenaSalud;
                                        }

                                        if (rd["item"].ToString() == "SCEMPLE" && SegCiTrab != 0)
                                        {
                                            if (esSuspension)
                                            {
                                                if(sp13)
                                                    listado.Add(new tablaLiquidacion() { Item = item + " * Suspension Autoridad", valorDescuento = calculado });
                                                if(sp14)
                                                    listado.Add(new tablaLiquidacion() { Item = item + " * Suspension Pacto", valorDescuento = calculado });
                                            }
                                            else
                                            {
                                                listado.Add(new tablaLiquidacion() { Item = item, valorDescuento = calculado });
                                            }

                                        }                                        
                                        else if (rd["item"].ToString() == "IMPUEST")
                                            listado.Add(new tablaLiquidacion() { Item = item, valorDescuento = calculado });
                                        else if (calculado != 0)
                                            listado.Add(new tablaLiquidacion() { Item = item, valorDescuento = calculado });
                                    }
                                    else if (tipo == 5)
                                    {
                                        //DESCUENTOS          

                                        if ((string)rd["cuota"] != "0")
                                            cadCuotas = item + " " + ItemTrabajador.GetCadenaCuota((string)rd["cuota"]);
                                        else
                                            cadCuotas = item;
                                        if (Convert.ToBoolean(rd["imprimebase"]))
                                            listado.Add(new tablaLiquidacion() { Item = cadCuotas, valorDescuento = calculado, vo = original });
                                        else
                                            listado.Add(new tablaLiquidacion() { Item = cadCuotas, valorDescuento = calculado });
                                    }
                                    else if (tipo == 6)
                                    {
                                        //CONTRIBUCIONES
                                        //..          
                                        if ((string)rd["item"] == "MUTUALI")
                                            listado.Add(new tablaLiquidacion() { Item = item, vo = Math.Round(calculado) });
                                        else
                                        {
                                            if (esSuspension)
                                            {
                                                if(sp13)
                                                    listado.Add(new tablaLiquidacion() { Item = item + " * Suspension Autoridad", vo = calculado });
                                                if(sp14)
                                                    listado.Add(new tablaLiquidacion() { Item = item + " * Suspension Pacto", vo = calculado });
                                            }

                                            else
                                                listado.Add(new tablaLiquidacion() { Item = item, vo = calculado });
                                        }
                                          
                                    }
                                    else
                                    {
                                        //CEMPRE Y SIS
                                        // listado.Add(new tablaLiquidacion() {Item = item, vo = calculado });
                                        if (rd["item"].ToString() == "SCEMPRE")
                                        {
                                            if (esSuspension)
                                            {
                                                if (sp13)
                                                {
                                                    d6.Item = "Seguro Empresa * Suspension Autoridad";
                                                    d6.vo = calculado;
                                                }
                                                if (sp14)
                                                {
                                                    d7.Item = "Seguro Empresa * Suspension Pacto";
                                                    d7.vo = calculado;
                                                }

                                            }
                                            else
                                            {
                                                if (SegIndividualEmp != 0)
                                                    d1.Item = "Seg. Ces. Empresa Individual"; d1.vo = SegIndividualEmp;

                                                d2.Item = "Seg. Ces. Empresa Solidario"; d2.vo = SegFondoEmp;
                                            }
                                           
                                        }
                                        if (rd["item"].ToString() == "SEGINV")
                                        {
                                            if (esSuspension)
                                            {
                                                if (calculado != 0)
                                                {
                                                    if(sp13)
                                                        d4.Item = "Invalidez * Suspension Autoridad"; d4.vo = calculado;
                                                    if (sp14)
                                                        d5.Item = "Invalidez * Suspension Pacto"; d5.vo = calculado;
                                                }
                                            }
                                            else
                                            {
                                                d3.Item = item; d3.vo = calculado;
                                            }
                                            
                                                
                                           
                                        }
                                    }
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
                XtraMessageBox.Show(ex.Message);
            }
          
            if (d1.Item != "")
                listado.Add(d1);
            if (d2.Item != "")
                listado.Add(d2);
            if (d3.Item != "")
                listado.Add(d3);
            if (d4.Item != "")
                listado.Add(d4);
            if (d5.Item != "")
                listado.Add(d5);
            if (d6.Item != "")
                listado.Add(d6);
            if (d7.Item != "")
                listado.Add(d7);


            lblLic.Text = Licencias + "";

            gridliquidacion.DataSource = listado;
            viewliquidacion.OptionsCustomization.AllowFilter = false;
            viewliquidacion.OptionsCustomization.AllowSort = false;
            viewliquidacion.OptionsMenu.EnableFooterMenu = false;
            viewliquidacion.OptionsMenu.EnableGroupPanelMenu = false;
            viewliquidacion.OptionsMenu.EnableColumnMenu = false;
            viewliquidacion.OptionsCustomization.AllowColumnMoving = false;

            ColumnasGrilla();

            //lblHab.Text = "$" + Calculo.GetValueFromCalculoMensaul(contrato, PeriodoEmpleado, "systhab").ToString("N0");
            //lblDctos.Text = "$" + Calculo.GetValueFromCalculoMensaul(contrato, PeriodoEmpleado, "systdctos").ToString("N0");
            //lblImp.Text = "$" + Calculo.GetValueFromCalculoMensaul(contrato, PeriodoEmpleado, "systimp").ToString("N0");
            //lblLiq.Text = "$" + Calculo.GetValueFromCalculoMensaul(contrato, PeriodoEmpleado, "sysliq").ToString("N0");
            //lblPago.Text = "$" + Calculo.GetValueFromCalculoMensaul(contrato, PeriodoEmpleado, "syspago").ToString("N0");

            lblHab.Text = "$" + systhab.ToString("N0");
            lblDctos.Text = "$" + systdctos.ToString("N0");
            lblImp.Text = "$" + systimp.ToString("N0");
            lblLiq.Text = "$" + sysliq.ToString("N0");
            lblPago.Text = "$" + syspago.ToString("N0");

        }

        //OBTENER EL TIPO DE ACUERDO A CODIGO ITEM
        private string[] tipoItem(string item)
        {
            string sql = "SELECT tipo, descripcion FROM item WHERE coditem = @item";
            string[] datos = new string[2];
            SqlCommand cmd2;
            SqlDataReader rd2;
            try
            {               
                    using (cmd2 = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd2.Parameters.Add(new SqlParameter("@item", item));

                        rd2 = cmd2.ExecuteReader();
                        if (rd2.HasRows)
                        {
                            while (rd2.Read())
                            {
                            datos[0] = (int)rd2["tipo"] + "";
                            datos[1] = (string)rd2["descripcion"];
                            }
                        }
                    }
                cmd2.Dispose();
               
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return datos;
        }

        //PARA AFP (NOMBRE AFP Y PORCENTAJE TOTAL A PAGAR)
        private string PorcentajeAfp(bool RegimenAntiguo, double pImponible, double pTopeAfp, double pLic, double pDiastr, double pPorcentajeAdmin, double pImpAnterior,  bool? showPreview = false, bool? EsSuspension = false, bool? Es13 = false, bool? Es14 = false)
        {
            CajaPrevision caja = new CajaPrevision();
            AseguradoraFondoPension Afp = new AseguradoraFondoPension();

            string info = "";
            double showValue = 0, porcPrevision = 0;
            double porcAdmin = 0;
            double ImponibleSuspension = 0;

            //imp = Calculo.GetValueFromCalculoMensaul(contrato, PeriodoEmpleado, "systimp");
            //topeAfp = Math.Round(varSistema.ObtenerValorLista("systopeafp"));
            //topeAfp = Calculo.GetValueFromCalculoMensaul(contrato, PeriodoEmpleado, "systopeafp");

            //Mostramos el tope
            if (EsSuspension == false)
            {
                if (pImponible > pTopeAfp)
                    showValue = pTopeAfp;
                else if (pTopeAfp > pImponible)
                    showValue = pImponible;
                else
                    showValue = pImponible;

                //TIENE LICENCIAS
                //double Lic = 0, diasTr = 0;
                //Lic = Calculo.GetValueFromCalculoMensaul(contrato, PeriodoEmpleado, "sysdiaslic");
                //diasTr = Calculo.GetValueFromCalculoMensaul(contrato, PeriodoEmpleado, "sysdiastr");

                if (pLic > 0)
                {
                    if (pImponible > pTopeAfp)
                        showValue = (showValue / 30) * pDiastr;
                }

                if (RegimenAntiguo)
                {
                    //Consultamos datos cajaprevision...
                    //porcPrevision = Calculo.GetValueFromCalculoMensaul(contrato, PeriodoEmpleado, "sysporcadmafp");
                    porcPrevision = pPorcentajeAdmin;
                    caja.SetInfo(contrato, PeriodoEmpleado);
                    info = porcPrevision + "% Cotiz. " + caja.Nombre + " Sobre:" + showValue.ToString("N0");
                }
                else
                {
                    //Consultamos datos afp...
                    Afp = Persona.GetAfp(contrato, PeriodoEmpleado);
                    //porcAdmin = Calculo.GetValueFromCalculoMensaul(contrato, PeriodoEmpleado, "sysporcadmafp");
                    porcAdmin = pPorcentajeAdmin;
                    info = (porcAdmin + Afp.porcFondo) + "% Cotiz. " + Afp.nombre + " Sobre:" + showValue.ToString("N0");
                }
            }
            else
            {
                if (pImpAnterior > pTopeAfp)
                    ImponibleSuspension = pTopeAfp;
                else
                    ImponibleSuspension = pImpAnterior;

                //ImponibleSuspension = (pImpAnterior / 30) * pDiastr;
                string cad = "";
                if (Es13 == true)
                    cad = " * Suspension autoridad";
                if (Es14 == true)
                    cad = " * Suspension pacto";

                if (RegimenAntiguo)
                {
                    //Consultamos datos cajaprevision...
                    //porcPrevision = Calculo.GetValueFromCalculoMensaul(contrato, PeriodoEmpleado, "sysporcadmafp");
                    porcPrevision = pPorcentajeAdmin;
                    caja.SetInfo(contrato, PeriodoEmpleado);
                    info = porcPrevision + "% Cotiz. " + caja.Nombre + " Sobre:" + ImponibleSuspension.ToString("N0") + cad;
                }
                else
                {
                    //Consultamos datos afp...
                    Afp = Persona.GetAfp(contrato, PeriodoEmpleado);
                    //porcAdmin = Calculo.GetValueFromCalculoMensaul(contrato, PeriodoEmpleado, "sysporcadmafp");
                    porcAdmin = pPorcentajeAdmin;
                    info = (porcAdmin + Afp.porcFondo) + "% Cotiz. " + Afp.nombre + " Sobre:" + ImponibleSuspension.ToString("N0") + cad;
                }

            }



            return info;
        }

        //PARA SALUD (TIPO SALUD)
        private string SaludEmpleado(double original)
        {
            string sql = "select isapre.id as identificador, isapre.nombre as nombre from trabajador " +
                        "INNER JOIN isapre ON isapre.id = trabajador.salud " +
                        "WHERE contrato = @contrato AND anomes = @periodo";

            SqlCommand cmd;
            SqlDataReader rd;
            string info = "";
            int id = 0;         
            try
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
                                id = (int)rd["identificador"];                              
                                if (id == 1)
                                {
                                    if (varSistema.ObtenerValorLista("sysdiaslic") != 0)
                                        //FONASA
                                        info = "7% FONASA (-Lic)";
                                    else
                                        info = "7% FONASA";
                                }
                                else
                                {
                                    //ISAPRE                               
                                    if (varSistema.ObtenerValorLista("sysdiaslic") != 0)
                                    {
                                        info = "UF " + original + " " + (string)rd["nombre"] + "(-Lic)";
                                    }
                                    else {
                                        info ="UF " +  original + " " + (string)rd["nombre"];
                                    }
                                }                               
                            }
                        }                        
                    }
                cmd.Dispose();
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return info;
        }

        private string GetCadenaSalud(bool AplicaPorc, bool AplicaUf, bool AplicaPesos, double pValue, double Lic, string pIsapre, int pCode, double ImpAnterior, bool? es13 = false, bool? es14 = false, bool? EsSuspension = false)
        {        
            string cad = "", Isapre = "";
            int code = 0;

            code = pCode;
            Isapre = pIsapre;

            if (EsSuspension == true)
            {
                //FONASA
                if (code == 1)
                {
                    if (es13 == true)
                        cad = "Fonasa * Suspension autoridad";
                    if (es14 == true)
                        cad = "Fonasa * Suspension pacto";
                    
                }
                //ISAPRE
                else if (code >= 2)
                {
                    if (es13 == true)
                        cad = $"{Isapre} * Suspension autoridad";
                    if (es14 == true)
                        cad = $"{Isapre} * Suspension pacto";
                  
                }
            }
            else
            {
                //FONASA
                if (code == 1)
                {
                    if (AplicaPorc)
                    {
                        if (Lic != 0)
                            cad = $"{pValue}% FONASA (-Lic)";
                        else
                            cad = $"{pValue}% FONASA";
                    }
                    else if (AplicaUf)
                    {
                        if (Lic != 0)
                            cad = $"{pValue} UF FONASA (-Lic)";
                        else
                            cad = $"{pValue} UF FONASA";
                    }
                    else if (AplicaPesos)
                    {
                        if (Lic != 0)
                            cad = $"{pValue} Pesos FONASA (-Lic)";
                        else
                            cad = $"{pValue} Pesos FONASA";
                    }
                    else
                    {
                        if (Lic != 0)
                            //FONASA
                            cad = "7% FONASA (-Lic)";
                        else
                            cad = "7% FONASA";
                    }
                }
                //ISAPRE
                else if (code >= 2)
                {
                    //SI APLICA PORCENTAJE
                    if (AplicaPorc)
                    {
                        if (Lic != 0)
                            cad = $"{pValue}% {Isapre} (-Lic)";
                        else
                            cad = $"{pValue}% {Isapre}";
                    }
                    else if (AplicaUf)
                    {
                        if (Lic != 0)
                            cad = $"{pValue} UF {Isapre} (-Lic)";
                        else
                            cad = $"{pValue} UF {Isapre}";
                    }
                    else if (AplicaPesos)
                    {
                        if (Lic != 0)
                            cad = $"DESCUENTO {Isapre} (-Lic)";
                        else
                            cad = $"DESCUENTO {Isapre}";
                    }
                    else
                        cad = "DESCUENTO SALUD";
                }
            }
                  

            return cad;
        }

        //SOLO PARA PERIODOS ANTERIORES
        private Hashtable SaludEmpleadoAnterior()
        {
            string sql = "select isapre.id as identificador, isapre.nombre as nombre from trabajador " +
                        "INNER JOIN isapre ON isapre.id = trabajador.salud " +
                        "WHERE contrato = @contrato AND anomes = @periodo";
            SqlCommand cmd;
            SqlDataReader rd;
            SqlConnection cn;
            Hashtable data = new Hashtable();       

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
                                cmd.Parameters.Add(new SqlParameter("@contrato", contrato));
                                cmd.Parameters.Add(new SqlParameter("@periodo", PeriodoEmpleado));

                                rd = cmd.ExecuteReader();
                                if (rd.HasRows)
                                {
                                    while (rd.Read())
                                    {
                                        data.Add("code", Convert.ToInt32(rd["identificador"]));
                                        data.Add("nombre", (string)rd["nombre"]);
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
                    XtraMessageBox.Show(ex.Message);
                }
          
            return data;
        }

        //IMPUESTO
        private string ImpuestoEmpleado()
        {
            string info = "";           
                //NO TIENE LICENCIAS ASOCIADAS
                info = "Impto " + varSistema.ObtenerValorLista("sysfactorimpto") * 100 + "% " +
                "de: " + (varSistema.ObtenerValorLista("systributo")).ToString("N0") + " -Rebaja:" + (varSistema.ObtenerValorLista("sysrebimpto")).ToString("N2");

            return info;
        }

        //OBTENER LA CANTIDA DE CUOTAS QUE TIENE EL EMPLEADO
        private string NumeroCuotas(string item, int numitem, SqlConnection cn)
        {
            string cadena = "", cuot = "";
            string sql = "SELECT cuota FROM itemtrabajador WHERE contrato=@contrato " +
                        " AND anomes=@periodo AND coditem=@item AND numitem=@numitem";
            SqlCommand cmd3;
            SqlDataReader rd3;
            string[] separa = new string[2];
            try
            {              
              using (cmd3 = new SqlCommand(sql, cn))
                    {
                        //PARAMETROS
                        cmd3.Parameters.Add(new SqlParameter("@contrato", contrato));
                        cmd3.Parameters.Add(new SqlParameter("@periodo", PeriodoEmpleado));
                        cmd3.Parameters.Add(new SqlParameter("@item", item));
                        cmd3.Parameters.Add(new SqlParameter("@numitem", numitem));

                        rd3 = cmd3.ExecuteReader();
                        if (rd3.HasRows)
                        {
                            while (rd3.Read())
                            {
                                cuot = (string)rd3["cuota"];
                                if (cuot != "0")
                                {
                                    if (cuot.Contains("/"))
                                    {
                                        separa = cuot.Split('/');
                                        cadena = "cuota n° " + separa[0] + " de " + separa[1];
                                    }
                                }
                                else
                                {
                                    cadena = "0";
                                }
                            }                            
                        }                        
                    }              
              rd3.Close();
              cmd3.Dispose();                  
             
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
         
            return cadena;
        }

        //GENERAR UN DATATABLE CON PAR [CODIGOITEM, CUOTA]
        private DataTable TablaCuotas(string item)
        {
            DataTable data = new DataTable();
            string sql = "SELECT coditem, cuota FROM itemtrabajador WHERE contrato=@contrato AND anomes=@periodo AND coditem=@item";
            SqlCommand cmd;
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@contrato", contrato));
                        cmd.Parameters.Add(new SqlParameter("@periodo", PeriodoEmpleado));
                        cmd.Parameters.Add(new SqlParameter("@item", item));

                        ad.SelectCommand = cmd;
                        ad.Fill(data);

                        fnSistema.sqlConn.Close();
                        ad.Dispose();
                        cmd.Dispose();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }


            return data;
        }

        //OBTENER NOMBRE COMPLETO, CARGO, RUT, FECHA INGRESO
        private List<string> CabeceraEmpleado(string contrato, int periodo)
        {
            string sql = "SELECT concat(apepaterno, ' ', apematerno,' ', trabajador.nombre) as nombre, " +
                "rut, cargo.nombre as cargo, ingreso FROM trabajador " +
                "INNER JOIN cargo ON cargo.id = trabajador.cargo " +
                "WHERE contrato = @contrato AND anomes = @periodo";

            List<string> lista = new List<string>();
            string d = "";
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
                        cmd.Parameters.Add(new SqlParameter("@contrato", contrato));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //GUARDAMOS LOS DATOS
                                lista.Add((string)rd["nombre"]);
                                lista.Add(fnSistema.fFormatearRut2((string)rd["rut"]));
                                lista.Add((string)rd["cargo"]);
                                d = fnSistema.FechaFormato((DateTime)rd["ingreso"]);
                                lista.Add(d);

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

            //RETORNAMOS LA LISTA
            return lista;
        }

        //CARGAR PRELISTA (PARA MOSTRAR DOCUMENTO PDF)
        private void CargarListadoPdf()
        {
            List<Data> listado = new List<Data>();
            Data d1 = new Data();
            Data d2 = new Data();

            string sql = "select item, original, calculado " +
                        " from calculo " +
                        " WHERE contrato = @contrato AND anomes = @periodo";

            int tipo = 0, cExentos = 0, cFam = 0, cLeyes = 0, cDesc = 0, cAportes = 0;
            string[] arreglo = new string[2];
            double original = 0, calculado = 0;
            string item = "", cadImpuesto = "", cadAfp = "";
            SqlCommand cmd;
            SqlDataReader rd;

            //OBTENER CADENA PARA IMPUESTO
            cadImpuesto = ImpuestoEmpleado();

            //CADENA PARA AFP
            //cadAfp = PorcentajeAfp(true, true);

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        cmd.Parameters.Add(new SqlParameter("@periodo", PeriodoEmpleado));
                        cmd.Parameters.Add(new SqlParameter("@contrato", contrato));
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //PRIMER ELEMENTO ES HABERES IMPONIBLES
                            listado.Add(new Data() { Item = "HABERES" });
                            while (rd.Read())
                            {
                                arreglo = tipoItem((string)rd["item"]);
                                tipo = Convert.ToInt32(arreglo[0]);
                                item = arreglo[1];                         
                              
                                original = Convert.ToDouble((decimal)rd["original"]);
                                calculado = Math.Round(Convert.ToDouble((decimal)rd["calculado"]));

                                if (tipo == 1)
                                {
                                    
                                    //HABERES IMPONIBLES
                                    listado.Add(new Data() { Item = item.ToLower(), Vo = original.ToString("N2"), Haber = calculado.ToString("N0")});
                                    
                                }
                                else if (tipo == 2)
                                {
                                    if (cExentos == 0)
                                     listado.Add(new Data() { Item = "OTROS HABERES" });

                                    //HABERES EXENTOS
                                    listado.Add(new Data() { Item = item.ToLower(), Vo = "", Haber = calculado.ToString("N0")});
                                  
                                    cExentos++;
                                }
                                else if (tipo == 3)
                                {
                                    if (cFam == 0)
                                        listado.Add(new Data() { Item = "ASIGNACIONES FAMILIARES" });

                                    //FAMILIARES
                                    listado.Add(new Data() { Item = item.ToLower(), Vo = original.ToString("N2"), Haber = calculado.ToString("N0")});

                                    cFam++;
                                }
                                else if (tipo == 4 && (rd["item"].ToString() != "SCEMPRE" && rd["item"].ToString() != "SEGINV"))
                                {
                                    if (cLeyes == 0)
                                        listado.Add(new Data() { Item = "DESCUENTOS LEGALES" });

                                    //DESCUENTOS LEGALES (LEYES SOCIALES)
                                    original = 0;

                                    if (rd["item"].ToString() == "PREVISI")
                                        item = cadAfp;
                                    if (rd["item"].ToString() == "IMPUEST" && calculado != 0)
                                        item = cadImpuesto;
                                    if (rd["item"].ToString() == "IMPUEST" && calculado == 0)
                                    { item = "EXENTO DE IMPUESTO"; }
                                    if (rd["item"].ToString() == "SALUD")
                                        item = SaludEmpleado(Convert.ToDouble((decimal)rd["original"]));

                                    listado.Add(new Data() { Item = item.ToLower(), Descuento = calculado.ToString("N0")});

                                    cLeyes++;
                                }
                                else if (tipo == 5)
                                {
                                    //DESCUENTOS
                                    if (cDesc == 0)
                                        listado.Add(new Data() { Item = "DESCUENTOS" });

                                    listado.Add(new Data() { Item = item.ToLower(), Descuento = calculado.ToString("N0")});

                                    cDesc++;
                                }
                                else if (tipo == 6)
                                {
                                    //CONTRIBUCIONES
                                    //..
                                }
                                else
                                {
                                    //CEMPRE Y SIS
                                    //listado.Add(new Data() {Item = item.ToLower(), Vo = calculado + "" });
                                    if (cAportes == 0)
                                    { d1.Item = item.ToLower(); d1.Vo = calculado.ToString("N2"); }
                                    if (cAportes == 1)
                                    { d2.Item = item.ToLower(); d2.Vo = calculado.ToString("N2"); }

                                    cAportes++;
                                }
                            }
                        }
                    }
                    cmd.Dispose();
                    rd.Close();
                    fnSistema.sqlConn.Close();

                    //AGREGAMOS D1 Y D2 A LA LISTA
                    listado.Add(new Data() { Item = "APORTES" });
                    listado.Add(d1);
                    listado.Add(d2);

                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            List<string> datosEmpleado = new List<string>();
            datosEmpleado = CabeceraEmpleado(contrato, PeriodoEmpleado);
            List<string> datosEmpresa = new List<string>();
            datosEmpresa = ConsultaDatosEmpresa();

            //PASAR COMO DATASOURCE A REPORTE
            //RptLiquidacion reporte = new RptLiquidacion();
            //Report externo
            ReportesExternos.rptLiquidacion reporte = new ReportesExternos.rptLiquidacion();
            reporte.DataSource = listado;

            //SETEAR PARAMETROS...
            reporte.Parameters["imagen"].Value = Imagen.GetLogoFromBd();
            reporte.Parameters["imponible"].Value = "$" + (Math.Round(varSistema.ObtenerValorLista("systimp"))).ToString("N0");
            reporte.Parameters["descuentos"].Value =(Math.Round(varSistema.ObtenerValorLista("systdctos"))).ToString("N0");
            reporte.Parameters["haberes"].Value =(Math.Round(varSistema.ObtenerValorLista("systhab"))).ToString("N0");
            reporte.Parameters["liquido"].Value ="$" +  (Math.Round(varSistema.ObtenerValorLista("sysliq"))).ToString("N0");
            reporte.Parameters["pago"].Value = "$" + (Math.Round(varSistema.ObtenerValorLista("syspago"))).ToString("N0");
            reporte.Parameters["dias"].Value = varSistema.ObtenerValorLista("sysdiastr");
            reporte.Parameters["cargo"].Value = datosEmpleado[2];
            reporte.Parameters["nombre"].Value = datosEmpleado[0];
            reporte.Parameters["rut"].Value = datosEmpleado[1];
            reporte.Parameters["ingreso"].Value = datosEmpleado[3];
            reporte.Parameters["rutempresa"].Value =fnSistema.fFormatearRut2(datosEmpresa[0]);
            reporte.Parameters["giro"].Value = datosEmpresa[1];
            reporte.Parameters["direccion"].Value = datosEmpresa[2];

            foreach (DevExpress.XtraReports.Parameters.Parameter parametro in reporte.Parameters)
            {
                parametro.Visible = false;
            }



            fnSistema.ShowDocument(reporte);
        }

        //OBTENER GIRO EMPRESA, DIRECCION Y RUT
        private List<string> ConsultaDatosEmpresa()
        {
            List<string> listado = new List<string>();
            string sql = "SELECT rutemp, giro, direccion FROM empresa";
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
                                listado.Add((string)rd["rutemp"]);
                                listado.Add((string)rd["giro"]);
                                listado.Add((string)rd["direccion"]);
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

            return listado;
        }

        //VERIFICAR SI TIENE SUBASE O SEGINV
        private bool TieneItemBase(string contrato, int periodo, string item)
        {
            bool existe = false;
            string sql = "SELECT coditem from itemtrabajador where anomes = @periodo " +
                         "AND contrato = @contrato AND coditem = @item";
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
                        cmd.Parameters.Add(new SqlParameter("@periodo", periodo));
                        cmd.Parameters.Add(new SqlParameter("@item", item));
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            existe = true;
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

        #endregion

        private void panelControl1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void viewliquidacion_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "valorDescuento" || e.Column.FieldName == "valorHaber" || e.Column.FieldName == "vo")
            {
                if (Convert.ToDouble(e.Value) == 0)
                {
                    e.DisplayText = "";
                }
            }
        }

        private void viewliquidacion_ShowingEditor(object sender, CancelEventArgs e)
        {
            //NO PERMITIR EDITAR LAS CELDAS
            e.Cancel = true;
        }

        private void panelControl3_Paint(object sender, PaintEventArgs e)
        {

        }
        private void btnImprimir_Click(object sender, EventArgs e)
        {
            //if (objeto.ValidaAcceso(User.GetUserGroup(), "rptLiquidacion") == false)
            //{ XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            //string Nombre = "";            
            
            //Documento docu = new Documento(contrato, PeriodoEmpleado);
            //XtraReport reporte = new XtraReport();

            //Nombre = contrato + "_" + fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(PeriodoEmpleado)) + "_" + DateTime.Now.Date.ToShortDateString();
            
            //if (Calculo.PeriodoObservado != PeriodoEmpleado)
            //    reporte = docu.SoloHaberesAnteriores();
            //else
            //    reporte = docu.SoloHaberes();

            //docu.ExportToPdf(reporte, Nombre);
            //docu.ShowDocument(reporte);            
        }

        private void btnSavePdf_Click(object sender, EventArgs e)
        {
                 
        }

        private void gridliquidacion_Click(object sender, EventArgs e)
        {
           
        }
        private void viewliquidacion_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {

        }

        private void viewliquidacion_CustomRowFilter(object sender, DevExpress.XtraGrid.Views.Base.RowFilterEventArgs e)
        {
            
        }

        private void btnSiguiente_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            string contratoAux = "";
            int periodoAux = 0;
            if (contrato != "" && PeriodoEmpleado != 0)
            {
                nav.Siguiente();
                
                //PeriodoEmpleado = nav.Listado[nav.Inicio].Periodo;
                periodoAux = nav.Listado[nav.Inicio].Periodo;
                contratoAux = nav.Listado[nav.Inicio].Contrato;

                if (contratoAux == contrato && PeriodoEmpleado == periodoAux)
                {
                    nav.Siguiente();
                    PeriodoEmpleado = nav.Listado[nav.Inicio].Periodo;
                    contrato = nav.Listado[nav.Inicio].Contrato;
                }
                else
                {                  
                    contrato = contratoAux;
                    PeriodoEmpleado = periodoAux;
                }

          
                lblEvaluado.Text = "Periodo: " + fnSistema.PrimerMayuscula(fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(nav.Listado[nav.Inicio].Periodo)));
                lblContrato.Text = "Contrato: " + contrato;
                //DIAS TRABAJADOS
                //DiasEmpleado dias = new DiasEmpleado(contrato, PeriodoEmpleado);
                lbldias.Text = Calculo.GetValueFromCalculoMensaul(contrato, PeriodoEmpleado, "sysdiastr") + "";               

                CargarListadoAnteriores();
                ConsultaDatosEmpleado(contrato, PeriodoEmpleado);
            }            
        }

        private void btnAnterior_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            int Anterior = 0;
            string contratoAnterior = "";
            if (contrato != "" && PeriodoEmpleado != 0)
            {
                if (nav.Universo > 0)
                {
                    nav.Anterior();
                    Anterior = nav.Listado[nav.Inicio].Periodo;
                    contratoAnterior = nav.Listado[nav.Inicio].Contrato;

                    //PeriodoEmpleado = nav.Listado[nav.Inicio];
                    //SI AL RETROCEDER UN PERIODO ESTE YA SE ESTÁ VISUALIZANDO RETROCEDEMOS OTRO MAS
                    if (contratoAnterior == contrato && PeriodoEmpleado == Anterior)
                    {
                        nav.Anterior();
                        PeriodoEmpleado = nav.Listado[nav.Inicio].Periodo;
                        contrato = nav.Listado[nav.Inicio].Contrato;
                    }
                    else
                    {
                        PeriodoEmpleado = Anterior;
                        contrato = contratoAnterior;
                    }                        

                    lblEvaluado.Text = "Periodo: " + fnSistema.PrimerMayuscula(fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(nav.Listado[nav.Inicio].Periodo)));
                    lblContrato.Text = "Contrato: " + contrato;

                    //DIAS TRABAJADOS
                    //DiasEmpleado dias = new DiasEmpleado(contrato, PeriodoEmpleado);
                    lbldias.Text = Calculo.GetValueFromCalculoMensaul(contrato, PeriodoEmpleado, "sysdiastr") + "";
                    ConsultaDatosEmpleado(contrato, PeriodoEmpleado);
                 
                    CargarListadoAnteriores();
                 
                }                
            }
        }

        private void btnPrimero_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            if (contrato != "" && PeriodoEmpleado != 0)
            {
                if (nav.Universo > 0)
                {
                    nav.Primer();
                    PeriodoEmpleado = nav.Listado[nav.Inicio].Periodo;
                    contrato = nav.Listado[nav.Inicio].Contrato;
                    lblEvaluado.Text = "Periodo: " + fnSistema.PrimerMayuscula(fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(nav.Listado[nav.Inicio].Periodo)));
                    lblContrato.Text = "Contrato: " + contrato;

                    //DIAS TRABAJADOS
                    //DiasEmpleado dias = new DiasEmpleado(contrato, PeriodoEmpleado);
                    lbldias.Text = Calculo.GetValueFromCalculoMensaul(contrato, PeriodoEmpleado, "sysdiastr") + "";
                    ConsultaDatosEmpleado(contrato, PeriodoEmpleado);                   
                    CargarListadoAnteriores();                                   
                }              
            }
        }

        private void btnUltimo_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            if (contrato != "" && PeriodoEmpleado != 0)
            {
                if (nav.Universo > 0)
                {
                    nav.Ultimo();
                    PeriodoEmpleado = nav.Listado[nav.Inicio].Periodo;
                    contrato = nav.Listado[nav.Inicio].Contrato;
                    lblEvaluado.Text = "Periodo: " + fnSistema.PrimerMayuscula(fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(nav.Listado[nav.Inicio].Periodo)));
                    lblContrato.Text = "Contrato: " + contrato;
                    //DIAS TRABAJADOS
                    //DiasEmpleado dias = new DiasEmpleado(contrato, PeriodoEmpleado);
                    lbldias.Text = Calculo.GetValueFromCalculoMensaul(contrato, PeriodoEmpleado, "sysdiastr") + "";
                    ConsultaDatosEmpleado(contrato, PeriodoEmpleado);
                    CargarListadoAnteriores();                

                    
                }               
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            if (contrato != "" && PeriodoEmpleado != 0)
            {
                if (nav.Universo > 0)
                {                  
                    nav.Ultimo();
                    PeriodoEmpleado = nav.Listado[nav.Inicio].Periodo;
                    contrato = nav.Listado[nav.Inicio].Contrato;
                    lblContrato.Text = "Contrato: " + contrato;
                   // Haberes hab = new Haberes(contrato, PeriodoEmpleado);
                   // hab.CalculoGenericoItemTrabajador(null);
                    lblEvaluado.Text = "Periodo: " + fnSistema.PrimerMayuscula(fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(nav.Listado[nav.Inicio].Periodo)));
                    lblLic.Text = Calculo.GetValueFromCalculoMensaul(contrato, PeriodoEmpleado, "sysdiaslic") + "";
                    lbldias.Text = Calculo.GetValueFromCalculoMensaul(contrato, PeriodoEmpleado, "sysdiastr") + "";
                    ConsultaDatosEmpleado(contrato, PeriodoEmpleado);
                   
                    CargarListadoAnteriores();                
                }                             
            }
        }

        private void btnSavePdf_Click_1(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "rptLiquidacion") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            string Nombre = "";

            Documento docu = new Documento(contrato, PeriodoEmpleado);
            XtraReport reporte = new XtraReport();

            Nombre = contrato + "_" + fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(PeriodoEmpleado)) + "_" + DateTime.Now.Date.ToShortDateString();
          
            reporte = docu.SoloHaberesAnteriores();         

            docu.ExportToPdf(reporte, Nombre);
        }

        private void btnImprimir_Click_1(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "rptLiquidacion") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            Documento docu = new Documento(contrato, PeriodoEmpleado);
            XtraReport reporte = new XtraReport();

            reporte = docu.SoloHaberesAnteriores();
            //reporte.SaveLayoutToXml(Path.Combine(fnSistema.RutaCarpetaReportesExterno, "rptLiquidacion2.repx"));
            docu.ShowDocument(reporte);
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "rptLiquidacion") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            string Nombre = "";

            Documento docu = new Documento(contrato, PeriodoEmpleado);
            XtraReport reporte = new XtraReport();

            //Nombre = contrato + "_" + fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(PeriodoEmpleado)) + "_" + DateTime.Now.Date.ToShortDateString();
            reporte = docu.SoloHaberesAnteriores();

            //CREAR PREVIAMENTE EL ARCHVO PDF EN EL DISCO
            string PathPdf = $"Liq_{contrato}-{fnSistema.PrimerMayuscula(fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(PeriodoEmpleado)))}.pdf";
            reporte.ExportToPdf(PathPdf);
            
            docu.ExportToZip(reporte, "123", PathPdf, false);
        }

        private void panelControl1_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void toolTipGrilla_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            //TOOLTIP PARA FILA GRIDCONTROL 
            if (e.SelectedControl != gridliquidacion)
                return;

            GridHitInfo hitInfo = viewliquidacion.CalcHitInfo(e.ControlMousePosition);

            if (hitInfo.InRow == false)
                return;

            SuperToolTipSetupArgs toolTipArgs = new SuperToolTipSetupArgs();

            DataRow drCurrentRow = viewliquidacion.GetDataRow(hitInfo.RowHandle);
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

        private void btnEditarReporte_Click(object sender, EventArgs e)
        {
            splashScreenManager1.ShowWaitForm();
            //Prueba de edición de certificado
            Documento docu = new Documento(contrato, PeriodoEmpleado);
            ReportesExternos.rptLiquidacion2 reporte = new ReportesExternos.rptLiquidacion2();

            reporte = (ReportesExternos.rptLiquidacion2)docu.SoloHaberesAnteriores();

            //Se le pasa el waitform para que se cierre una vez cargado
            DiseñadorReportes.MostrarEditorLimitado(reporte, "rptLiquidacion2.repx", splashScreenManager1);
        }
    }

    class tablaLiquidacion
    {
        //ITEM
        public string Item { get; set; }
        //VALOR ORIGINAL
        public double vo { get; set; }
        //VALOR HABER
        public double valorHaber { get; set; }
        public double valorDescuento { get; set; }

        List<tablaLiquidacion> ListadoLiquidacion = new List<tablaLiquidacion>();
        //CARGALISTADO
    }   
}