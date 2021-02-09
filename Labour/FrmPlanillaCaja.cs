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
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Configuration;

namespace Labour
{
    public partial class FrmPlanillaCaja : DevExpress.XtraEditors.XtraForm, IConjuntosCondicionales
    {
        //PARA GUARDAR EL FILTRO USUARIO
        private string FiltroUsuario { get; set; } = User.GetUserFilter();

        //LISTA DE DATOS
        List<PlanillaCaja> lista = new List<PlanillaCaja>();

        /// <summary>
        /// Descripcion condicion usada en filtro.
        /// </summary>
        public string DescripcionCondicion { get; set; } = "";

        #region "INTERFAZ"
        public void CargarCodigoConjunto(string pConjunto)
        {
            if (pConjunto.Length > 0)
            {
                txtConjunto.Text = pConjunto;
            }
        }

        #endregion

        /// <summary>
        /// Sql consulta base
        /// </summary>
        string sqlConsulta = "SELECT contrato FROM trabajador ";

        double SumaIsapre = 0, SumaSinIsapre = 0, SumaCotiz = 0, SumaSimple = 0,
         SumaMaternal = 0, SumaInvalidez = 0, SumaAsig = 0;

        int totalhombreconisapre = 0;
        int totalmujerconisapre = 0;
        int totalhombresinisapre = 0;
        int totalmujersinisapre = 0;
        double totalcotizacioncaja = 0;
        int totalcargaconisapre = 0;
        int totalcargasinisapre = 0;
        double totalremconisapre = 0;
        double totalremsinisapre = 0;
        double MontoRetroActiva = 0, TotalRetroactivo = 0;
        int SimpleTram1 = 0, InvalTram1 = 0, MatTram1 = 0, RetroTram1 = 0, totalPersonas1 = 0;
        int SimpleTram2 = 0, InvalTram2 = 0, MatTram2 = 0, RetroTram2 = 0, totalPersonas2 = 0;
        int SimpleTram3 = 0, InvalTram3 = 0, MatTram3 = 0, RetroTram3 = 0, totalPersonas3 = 0;
        int SimpleTram4 = 0, InvalTram4 = 0, MatTram4 = 0, RetroTram4 = 0, totalPersonas4 = 0;

        private void btnConjunto_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            FrmFiltroBusqueda filtro = new FrmFiltroBusqueda(true);
            filtro.opener = this;
            filtro.StartPosition = FormStartPosition.CenterParent;
            filtro.ShowDialog();
        }

        double TotalTram1 = 0, TotalTram2 = 0, TotalTram3 = 0, TotalTram4 = 0;

        private void btnPdf_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            if (txtComboPeriodo.Properties.DataSource != null)
            {
                if (Calculo.PeriodoValido(Convert.ToInt32(txtComboPeriodo.EditValue)) == false)
                { XtraMessageBox.Show("Por favor ingresa un periodo válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                ImprimeDocumento(Convert.ToInt32(txtComboPeriodo.EditValue), false, true);
            }
        }

        //USUARIO PUEDE VER FICHAS PRIVADAS
        private bool ShowPrivados { get; set; } = User.ShowPrivadas();

        public FrmPlanillaCaja()
        {
            InitializeComponent();
        }        

        #region "MANEJO DE DATOS"
        //LISTADO DE CONTRATOS DE ACUERDO A PERIODO PARAMETRO
        private List<string> ListadoContratos(int pPeriodo, string pConjunto, string pBusqueda)
        {
            List<string> lista = new List<string>();
            SqlCommand cmd;
            SqlDataReader rd;
            string sql = "";
            string sqlFiltro = "";

            sqlFiltro = Calculo.GetSqlFiltro(FiltroUsuario, pConjunto, ShowPrivados);

            DescripcionCondicion = Conjunto.GetCondicionReporte(pConjunto, FiltroUsuario);

            if (pBusqueda.Length > 0)
            {
                sql = sqlConsulta + $" WHERE anomes=@pPeriodo AND status=1 AND (nombre LIKE '%{pBusqueda}%' " +
                    $"OR apepaterno LIKE '%{pBusqueda}%' OR apematerno LIKE '%{pBusqueda}%' OR contrato LIKE '%{pBusqueda}%') {sqlFiltro} \n ORDER BY apepaterno";
            }
            else
            {
                sql = sqlConsulta + $" WHERE status=1 AND anomes=@pPeriodo {sqlFiltro} \n ORDER BY apepaterno";
            }

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pPeriodo", pPeriodo));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //LLENAMOS LA LIST
                                lista.Add((string)rd["contrato"]);
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
            return lista;
        }

        //LISTA FILTRADA POR COINCIDENCIAS DE CONTRATO
        //PARAMETRO DE ENTRADA: NUMERO DE CONTRATO
        private List<string> ListaContratoFiltrada(string busqueda, int periodo, int option)
        {
            List<string> registros = new List<string>();
            string sqlContrato = "", sqlNombre = "", condUser = "";

            //SI PROPIEDAD SHOWPRIVADOS ES FALSE, EL USUARIO PUEDE VER FICHAS PRIVADAS

            if (FiltroUsuario == "0")
                sqlContrato = string.Format("SELECT contrato FROM trabajador WHERE contrato LIKE '%{0}%' AND anomes={1} " + (ShowPrivados==false?" AND privado=0":"") +" ORDER BY apepaterno", busqueda, periodo);
            else
            {
                condUser = Conjunto.GetCondicionFromCode(FiltroUsuario);
                sqlContrato = string.Format("SELECT contrato FROM trabajador WHERE contrato LIKE '%{0}%' AND anomes={1} AND {2} " + (ShowPrivados == false ? " AND privado=0":"") + " ORDER BY apepaterno", busqueda, periodo, condUser);
            }

            if (FiltroUsuario == "0")
                sqlNombre = string.Format("SELECT contrato FROM trabajador WHERE (apepaterno LIKE '%{0}%' " +
                "OR apematerno LIKE '%{1}%' OR apematerno LIKE '%{2}%' OR nombre LIKE '%{3}%') AND anomes={4} " + (ShowPrivados==false?" AND privado=0":"") +" ORDER BY apepaterno", busqueda, busqueda, busqueda, busqueda, periodo);
            else
            {
                condUser = Conjunto.GetCondicionFromCode(FiltroUsuario);
                sqlNombre = string.Format("SELECT contrato FROM trabajador WHERE (apepaterno LIKE '%{0}%' " +
                "OR apematerno LIKE '%{1}%' OR apematerno LIKE '%{2}%' OR nombre LIKE '%{3}%') AND anomes={4} AND {5} " + (ShowPrivados == false ? " AND privado=0" : "") + " ORDER BY apepaterno", busqueda, busqueda, busqueda, busqueda, periodo, condUser);
            }

            string uso = "";

            //OPTION 1 --> BUQUEDA POR NUMERO DE CONTRATO
            //OPTIN 2 --> BUSQUEDA POR NOMBRE
            if (option == 1)
                uso = sqlContrato;
            else if (option == 2)
                uso = sqlNombre;

            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(uso, fnSistema.sqlConn))
                    {
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                registros.Add((string)rd["contrato"]);
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
            return registros;
        }

        //EN BASE A LA LISTA DE CONTRATOS DEBEMOS OBTENER LOS VALORES DE SALUD ASOCIADOS
        private void CalculoDatos(List<string> listado, int periodo)
        {
            double Imponible = 0, valorCaja = 0, diasTrab = 0, asigfam = 0, Conisapre = 0, SinIsapre = 0, TopeSalud = 0;
            MontoRetroActiva = 0; TotalRetroactivo = 0;
            SumaAsig = 0; SumaCotiz = 0; SumaInvalidez = 0; SumaIsapre = 0; SumaMaternal = 0; SumaSinIsapre = 0;
            SumaSimple = 0;
            int simples = 0, maternales, invalidas = 0, tramo = 0, retroactivas = 0;
            string IniMov = "";

            totalhombreconisapre = 0;totalmujerconisapre = 0;totalhombresinisapre = 0;totalmujersinisapre = 0;
            totalcotizacioncaja = 0; totalcargaconisapre = 0; totalcargasinisapre = 0;totalremconisapre = 0;
            totalremsinisapre = 0;

            //TOTAL POR TRAMO   
            SimpleTram1 = 0; InvalTram1 = 0; MatTram1 = 0; RetroTram1 = 0; totalPersonas1 = 0;
            SimpleTram2 = 0; InvalTram2 = 0; MatTram2 = 0; RetroTram2 = 0; totalPersonas2 = 0;
            SimpleTram3 = 0; InvalTram3 = 0; MatTram3 = 0; RetroTram3 = 0; totalPersonas3 = 0;
            SimpleTram4 = 0; InvalTram4 = 0; MatTram4 = 0; RetroTram4 = 0; totalPersonas4 = 0;

            TotalTram1 = 0; TotalTram2 = 0; TotalTram3 = 0; TotalTram4 = 0;

            //List<PlanillaIsapre> lista = new List<PlanillaIsapre>();
            lista.Clear();
            
            if (listado.Count > 0 && periodo != 0)
            {
                splashScreenManager1.ShowWaitForm();
                foreach (var elemento in listado)
                {                   
                    SinIsapre = 0; Conisapre = 0;tramo = 0;

                    //VALOR IMPONIBLE CONTRATO
                    Imponible = Calculo.GetValueFromCalculoMensaul(elemento, periodo, "systimp");
                    //TOPE SALUD
                    TopeSalud = Math.Round(Calculo.GetValueFromCalculoMensaul(elemento, periodo, "systopesalud"));
                    //VALOR COTIZACION CAJA
                    valorCaja = Calculo.GetValueFromCalculoMensaul(elemento, periodo, "syscaja");
                    //DIAS TRABAJADOS
                    diasTrab = Calculo.GetValueFromCalculoMensaul(elemento, periodo, "sysdiastr");
                    Familiar fam = new Familiar(elemento, periodo);
                    //CANTIDAD CARGAS FAMILIARES SIMPLES
                    simples = fam.GetNumCargasSimplesV2();
                    //CANTIDAD CARGAS FAMILIARES INVALIDAS
                    invalidas = fam.GetNumCargasInvalidezV2();
                    //CANTIDAD CARGAS FAMILIARES MATERNALES
                    maternales = fam.GetNumCargasMaternal();                    
                    //MONTO ASIGNACION FAMILIAR
                    asigfam = Calculo.GetValueFromCalculoMensaul(elemento, periodo, "systfam");
                    //ASIGNACION RETROACTIVA
                    MontoRetroActiva = fam.GetTotalAsignacionesRetroc();

                    //MOVIMIENTOS DE PERSONAL
                    Previred prev = new Previred(elemento, periodo);
                    List<MovimientoPersonal> Movimientos = prev.GetMovimientoPersonal(periodo, elemento);

                    Persona p = new Persona();
                    p = Persona.GetInfo(elemento, periodo);

                    //SI EL IMPONIBLE ES MAYOR AL TOPE CONSIDERAMOS EL TOPE 
                    if (Imponible > TopeSalud)
                        Imponible = TopeSalud;

                    //ISAPRE
                    if (p.codSalud != 1)
                        Conisapre = Imponible;
                    else
                        //FONASA
                        SinIsapre = Imponible;

                    if (simples > 0 || invalidas > 0 || maternales > 0 || asigfam > 0)
                        tramo = p.Tramo;

                    if (Movimientos.Count > 0)
                    {
                        for (int i = 0; i < Movimientos.Count; i++)
                        {
                            //SOLO EN ESTOS CASOS MOSTRAMOS FECHA INICIO MOVIMIENTO
                            if (Movimientos[i].codMovimiento == "1" || Movimientos[i].codMovimiento == "3" ||
                                Movimientos[i].codMovimiento == "4" || Movimientos[i].codMovimiento == "5" ||
                                Movimientos[i].codMovimiento == "6" || Movimientos[i].codMovimiento == "7" ||
                                Movimientos[i].codMovimiento == "8" || Movimientos[i].codMovimiento == "11")
                                IniMov = Movimientos[i].inicioMovimiento.ToString("dd-MM-yyyy");                        

                            //SEGUNDA LINEA
                            if (i > 0)
                            {
                                AddLista(lista, fnSistema.fFormatearRut2(fnSistema.fnDesenmascararRut(p.Rut)), p.ApellidoNombre, 
                                    0, 0, 0, 0, 0, 0, 0, 0, 0, 
                                    Convert.ToInt32(Movimientos[i].codMovimiento), IniMov, p.centro, 
                                    p.NombreArea, p.Cargo, p.sucursal);
                            }
                            else
                            {
                                //PRIMERA LINEA
                                AddLista(lista, fnSistema.fFormatearRut2(fnSistema.fnDesenmascararRut(p.Rut)), p.ApellidoNombre
                                     ,Conisapre, SinIsapre, valorCaja, diasTrab, simples, invalidas, maternales, 
                                     asigfam, tramo,
                                     Convert.ToInt32(Movimientos[i].codMovimiento), IniMov, p.centro, 
                                     p.NombreArea, p.Cargo, p.sucursal);                                
                            }
                        }
                    }
                    else
                    {
                        AddLista(lista, fnSistema.fFormatearRut2(fnSistema.fnDesenmascararRut(p.Rut)), p.ApellidoNombre
                                    , Conisapre, SinIsapre, valorCaja, diasTrab, simples, invalidas, maternales,
                                    asigfam, tramo, 0, "", p.centro, p.NombreArea, p.Cargo, p.sucursal);
                    }

                    SumaAsig = SumaAsig + asigfam;
                    SumaCotiz = SumaCotiz + valorCaja;
                    SumaInvalidez = SumaInvalidez + invalidas;
                    SumaIsapre = SumaIsapre + Conisapre;
                    SumaMaternal = SumaMaternal + maternales;
                    SumaSimple = SumaSimple + simples;
                    SumaSinIsapre = SumaSinIsapre + SinIsapre;

                    //TOTAL HOMBRES CON ISAPRE
                    if (Conisapre > 0 && p.Sexo == 0)
                        totalhombreconisapre++;
                    //TOTAL MUJERES CON ISAPRE
                    if (Conisapre > 0 && p.Sexo == 1)
                        totalmujerconisapre++;
                    //TOTAL HOMBRES SIN ISAPRE
                    if (SinIsapre > 0 && p.Sexo == 0)
                        totalhombresinisapre++;
                    //TOTAL MUJERES SIN ISAPRE
                    if (SinIsapre > 0 && p.Sexo == 1)
                        totalmujersinisapre++;

                    //TOTAL COTIZACION CAJA
                    totalcotizacioncaja = totalcotizacioncaja + Math.Round(valorCaja);                   
                    //TOTLA REMUNERACION CON ISAPRE
                    totalremconisapre = totalremconisapre + Conisapre;
                    //TOTAL REMUNERACION SIN ISAPRE
                    totalremsinisapre = totalremsinisapre + SinIsapre;

                    //CANTIDAD DE CARGAS SIMPLES PARA EL TRAMO 1
                    if (tramo == 1 && simples > 0)
                    { SimpleTram1 = SimpleTram1 + simples; totalPersonas1++; }
                    //CANTIDAD DE CARGAS MATERNALES PARA EL TRAMO 1
                    if (tramo == 1 && maternales > 0)
                    { MatTram1 = MatTram1 + maternales; totalPersonas1++; }
                    //CANTIDAD DE CARGAS INVALIDAZ PARA EL TRAMO 1
                    if (tramo == 1 && invalidas > 0)
                    { InvalTram1 = InvalTram1 + invalidas; totalPersonas1++; }

                    //CANTIDAD DE CARGAS SIMPLES PARA EL TRAMO 1
                    if (tramo == 2 && simples > 0)
                    { SimpleTram2 = SimpleTram2 + simples; totalPersonas2++; }
                    //CANTIDAD DE CARGAS MATERNALES PARA EL TRAMO 1
                    if (tramo == 2 && maternales > 0)
                    { MatTram2 = MatTram2 + maternales; totalPersonas2++; }
                    //CANTIDAD DE CARGAS INVALIDAZ PARA EL TRAMO 1
                    if (tramo == 2 && invalidas > 0)
                    { InvalTram2 = InvalTram2 + invalidas; totalPersonas2++; }

                    //CANTIDAD DE CARGAS SIMPLES PARA EL TRAMO 1
                    if (tramo == 3 && simples > 0)
                    { SimpleTram3 = SimpleTram3 + simples; totalPersonas3++; }
                    //CANTIDAD DE CARGAS MATERNALES PARA EL TRAMO 1
                    if (tramo == 3 && maternales > 0)
                    { MatTram3 = MatTram3 + maternales; totalPersonas3++; }
                    //CANTIDAD DE CARGAS INVALIDAZ PARA EL TRAMO 1
                    if (tramo == 3 && invalidas > 0)
                    { InvalTram3 = InvalTram3 + invalidas; totalPersonas3++; }

                    //CANTIDAD DE CARGAS SIMPLES PARA EL TRAMO 1
                    if (tramo == 4 && simples > 0)
                    { SimpleTram4 = SimpleTram4 + simples; totalPersonas4++; }
                    //CANTIDAD DE CARGAS MATERNALES PARA EL TRAMO 1
                    if (tramo == 4 && maternales > 0)
                    { MatTram4 = MatTram4 + maternales; totalPersonas4++; }
                    //CANTIDAD DE CARGAS INVALIDAZ PARA EL TRAMO 1
                    if (tramo == 4 && invalidas > 0)
                    { InvalTram4 = InvalTram4 + invalidas; totalPersonas4++; }

                    //TOTAL SIMPLES
                    if (tramo == 1)
                        TotalTram1 = TotalTram1 + asigfam;
                    if (tramo == 2)
                        TotalTram2 = TotalTram2 + asigfam;
                    if (tramo == 3)
                        TotalTram3 = TotalTram3 + asigfam;
                    if (tramo == 4)
                        TotalTram4 = TotalTram4 + asigfam;
                        
                    //TOTAL CARGAS CON ISAPRE
                    if (Conisapre > 0)
                        totalcargaconisapre = totalcargaconisapre + (simples + invalidas + maternales);
                    //TOTAL CARGAS SIN ISAPRE
                    if(SinIsapre > 0)
                        totalcargasinisapre = totalcargasinisapre + (simples + invalidas + maternales);

                    //TOTAL PAGO RETROACTIVO
                    TotalRetroactivo = TotalRetroactivo + MontoRetroActiva;
                }

                if (lista.Count > 0)
                {
                    //AGREGAMOS LISTA COMO DATASOURCE A GRILLA
                    

                    btnImprimir.Enabled = true;
                    btnImpresionRapida.Enabled = true;
                    btnPdf.Enabled = true;
                    XtraMessageBox.Show($"{lista.Count} registros encontrados", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    
                }
                else
                {

                    btnImprimir.Enabled = false;
                    btnImpresionRapida.Enabled = false;
                    btnPdf.Enabled = false;
                    XtraMessageBox.Show("No se encontró información", "Información", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
               

                splashScreenManager1.CloseWaitForm();
            }
            else
            {
                XtraMessageBox.Show("No se a podido realizar la operacion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void AddLista(List<PlanillaCaja> pList, string pRut, string pNombre, double pConIsapre,
        double pSinIsapre, double pCotizacionCaja, double pDiastrab, int pSimples,
        int pInvalidas, int pMaternales, double pMontoAsignacion, int pTramo,
        int pCodMov, string pIniMov, string pCentroCosto, string pArea, string pCargo, 
        string pSucursal)
        {
            pList.Add(new PlanillaCaja()
            {                
                Rut = pRut,
                NombreTrabajador = pNombre,
                ValorIsapre = pConIsapre,
                ValorSinIsapre = pSinIsapre,
                CotizacionCaja = pCotizacionCaja,
                DiasTrabajados = pDiastrab,
                Simples = pSimples,
                Invalidas = pInvalidas,
                Maternal = pMaternales,
                AsignacionFamiliar = pMontoAsignacion,
                Tramo = pTramo,
                CodMovimiento = pCodMov,
                InicioMovimiento = pIniMov, 
                Area = pArea,
                Cargo = pCargo,
                CentroCosto = pCentroCosto,
                Sucursal = pSucursal
            });
        }

     

        //IMPRIME
        private void ImprimeDocumento(int periodo, bool? impresionRapida = false, bool? GeneraPdf = false)
        {
            Empresa emp = new Empresa();
            emp.SetInfo();
            string field = "";
            if (lista.Count > 0)
            {
                //RptPlanillaCaja reporte = new RptPlanillaCaja();
                //Reporte externo
                Planillas_PagoCajaExterno.RptPlanillaCaja reporte = new Planillas_PagoCajaExterno.RptPlanillaCaja();
                reporte.DataSource = lista;

                //PARAMETROS
                reporte.Parameters["rutempresa"].Value = fnSistema.fFormatearRut2(emp.Rut);
                reporte.Parameters["empresa"].Value = emp.Razon;
                reporte.Parameters["periodo"].Value = fnSistema.PrimerMayuscula(fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(periodo)));
                reporte.Parameters["sumasinisapre"].Value = SumaSinIsapre;
                reporte.Parameters["sumcoti"].Value = SumaCotiz;
                reporte.Parameters["sumInval"].Value = SumaInvalidez;
                reporte.Parameters["sumisapre"].Value = SumaIsapre;
                reporte.Parameters["sumMat"].Value = SumaMaternal;
                reporte.Parameters["sumSimples"].Value = SumaSimple;
                reporte.Parameters["sumAsig"].Value = SumaAsig;
                reporte.Parameters["caja"].Value = txtCaja.Text;
                reporte.Parameters["imagen"].Value = Imagen.GetLogoFromBd();
                //PARA TABLA RESUMEN
                reporte.Parameters["TotalHombreConIsapre"].Value = totalhombreconisapre;
                reporte.Parameters["TotalMujerConIsapre"].Value = totalmujerconisapre;
                reporte.Parameters["TotalCargaConIsapre"].Value = totalcargaconisapre;
                reporte.Parameters["TotalRemConIsapre"].Value = totalremconisapre;
                reporte.Parameters["TotalCotConIsapre"].Value = 0;
                reporte.Parameters["TotalHombreSinIsapre"].Value = totalhombresinisapre;
                reporte.Parameters["TotalMujerSinIsapre"].Value = totalmujersinisapre;
                reporte.Parameters["TotalCargasSinIsapre"].Value = totalcargasinisapre;
                reporte.Parameters["TotalRemSinIsapre"].Value = totalremsinisapre;
                reporte.Parameters["TotalCotSinIsapre"].Value = totalcotizacioncaja;
                reporte.Parameters["TotalHombres"].Value = totalhombreconisapre + totalhombresinisapre;
                reporte.Parameters["TotalMujeres"].Value = totalmujerconisapre + totalmujersinisapre;
                reporte.Parameters["TotalCargas"].Value = totalcargaconisapre + totalcargasinisapre;
                reporte.Parameters["TotalRemu"].Value = Math.Round(totalremconisapre + totalremsinisapre);
                reporte.Parameters["TotalCot"].Value = totalcotizacioncaja;

                //SEGUNDA TABLA
                reporte.Parameters["SimpleTram1"].Value = SimpleTram1;
                reporte.Parameters["SimpleTram2"].Value = SimpleTram2;
                reporte.Parameters["SimpleTram3"].Value = SimpleTram3;
                reporte.Parameters["SimpleTram4"].Value = SimpleTram4;
                reporte.Parameters["InvTram1"].Value = InvalTram1;
                reporte.Parameters["InvTram2"].Value = InvalTram2;
                reporte.Parameters["InvTram3"].Value = InvalTram3;
                reporte.Parameters["InvTram4"].Value = InvalTram4;
                reporte.Parameters["MatTram1"].Value = MatTram1;
                reporte.Parameters["MatTram2"].Value = MatTram2;
                reporte.Parameters["MatTram3"].Value = MatTram3;
                reporte.Parameters["MatTram4"].Value = MatTram4;
                reporte.Parameters["RetTram1"].Value = RetroTram1;
                reporte.Parameters["RetTram2"].Value = RetroTram2;
                reporte.Parameters["RetTram3"].Value = RetroTram3;
                reporte.Parameters["RetTram4"].Value = RetroTram4;
                reporte.Parameters["PersTram1"].Value = totalPersonas1;
                reporte.Parameters["PersTram2"].Value = totalPersonas2;
                reporte.Parameters["PersTram3"].Value = totalPersonas3;
                reporte.Parameters["PersTram4"].Value = totalPersonas4;
                reporte.Parameters["MontoTr1"].Value = TotalTram1;
                reporte.Parameters["MontoTr2"].Value = TotalTram2;
                reporte.Parameters["MontoTr3"].Value = TotalTram3;
                reporte.Parameters["MontoTr4"].Value = TotalTram4;
                reporte.Parameters["RetSimple"].Value = 0;
                reporte.Parameters["RetInv"].Value = 0;
                reporte.Parameters["RetMat"].Value = 0;
                reporte.Parameters["RetRetro"].Value = 0;
                reporte.Parameters["RetMonto"].Value = 0;
                reporte.Parameters["SumRetro"].Value = 0;
                reporte.Parameters["TotalPersonas"].Value = totalPersonas1 + totalPersonas2 + totalPersonas3 + totalPersonas4;

                reporte.Parameters["condicion"].Value = DescripcionCondicion;
                reporte.Parameters["agrupacion"].Value = txtAgrupa.Text;

                foreach (DevExpress.XtraReports.Parameters.Parameter parametro in reporte.Parameters)
                {
                    parametro.Visible = false;
                }

                if (txtAgrupa.EditValue.ToString() != "0")
                {
                    reporte.groupFooterBand2.Visible = true;

                    reporte.groupHeaderBand1.GroupFields.Clear();
                    GroupField groupField = new GroupField();
                    field = txtAgrupa.Text.ToLower();
                    groupField.FieldName = field;
                    reporte.groupHeaderBand1.GroupFields.Add(groupField);

                    XRLabel labelGroup = new XRLabel { ForeColor = System.Drawing.Color.Black, WidthF = 1048, TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft };
                    labelGroup.Font = new Font("Arial", 9, FontStyle.Italic | FontStyle.Bold);
                    labelGroup.Borders = DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Bottom | DevExpress.XtraPrinting.BorderSide.Right;

                    if (Settings.Default.UserDesignerOptions.DataBindingMode == DataBindingMode.Bindings)
                    {
                        labelGroup.DataBindings.Add("Text", null, $"{field}");
                    }
                    else
                    {
                        labelGroup.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", $"[{field}]"));
                    }
                    reporte.groupHeaderBand1.Controls.Add(labelGroup);

                }
                else
                    reporte.groupFooterBand2.Visible = false;

                Documento d = new Documento("", 0);
                if ((bool)impresionRapida)
                    d.PrintDocument(reporte);
                else if ((bool)GeneraPdf)
                    d.ExportToPdf(reporte, $"Planilla{fnSistema.PrimerMayuscula(txtCaja.Text.ToLower())}_{fnSistema.PrimerMayuscula(fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(periodo)))}");
                else
                    d.ShowDocument(reporte);

            }
        }


        #endregion

        private void txtAfp_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtContrato_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtPeriodo_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }  

        private void cbTodos_CheckedChanged(object sender, EventArgs e)
        {
            if (cbTodos.Checked == true)
            {
                txtConjunto.Text = "";
                txtConjunto.Enabled = false;
                btnConjunto.Enabled = false;
            }
            else
            {
                txtConjunto.Text = "";
                txtConjunto.Focus();
                txtConjunto.Enabled = true;
                btnConjunto.Enabled = true;
            }
        }

        private void FrmPlanillaCaja_Load(object sender, EventArgs e)
        {
            Empresa emp = new Empresa();
            txtCaja.Text = emp.CajaCompensacion();

            datoCombobox.spLlenaPeriodos("SELECT anomes FROM parametro ORDER BY anomes DESC", txtComboPeriodo, "anomes", "anomes", true);
            btnImprimir.Enabled = false;
            btnImpresionRapida.Enabled = false;
            btnPdf.Enabled = false;

            if (txtComboPeriodo.Properties.DataSource != null)
                txtComboPeriodo.ItemIndex = 0;

            datoCombobox.AgrupaList(txtAgrupa);
        }

   

        private void viewCaja_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "Tramo" || e.Column.FieldName == "Simples" || 
                e.Column.FieldName == "Maternal" || e.Column.FieldName == "Invalidas" || 
                e.Column.FieldName == "AsignacionFamiliar" || e.Column.FieldName == "CotizacionCaja" ||
                e.Column.FieldName == "ValorSinIsapre" || e.Column.FieldName == "ValorIsapre" ||
                e.Column.FieldName == "DiasTrabajados")
            {
                if (Convert.ToDouble(e.Value) == 0)
                {
                    e.DisplayText = string.Empty;
                }
            }
        }

        private void txtPeriodo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) || char.IsNumber(e.KeyChar))
            {
                e.Handled = false;
            }
            else
                e.Handled = true;
        }

        private void txtContrato_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar) || char.IsLetter(e.KeyChar) || e.KeyChar == (char)45)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            Close();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            List<string> ListaContratos = new List<string>();

            if (txtComboPeriodo.Properties.DataSource == null)
            { XtraMessageBox.Show("Por favor ingresa un periodo válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }            

            if(Calculo.PeriodoValido(Convert.ToInt32(txtComboPeriodo.EditValue)) == false)
            { XtraMessageBox.Show("Por favor ingresa un periodo válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (cbTodos.Checked)
            {              
                ListaContratos = ListadoContratos(Convert.ToInt32(txtComboPeriodo.EditValue), "", "");

                if (ListaContratos.Count == 0)
                { XtraMessageBox.Show("No se encontraron registros", "Registros", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                //CALCULO
                CalculoDatos(ListaContratos, Convert.ToInt32(txtComboPeriodo.EditValue));                
            }
            else
            {

                if (txtConjunto.Text == "")
                { XtraMessageBox.Show("Por favor Ingresa una condición válida", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                if (Conjunto.ExisteConjunto(txtConjunto.Text) == false)
                { XtraMessageBox.Show("Por favor ingresa una condición válida", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                ListaContratos = ListadoContratos(Convert.ToInt32(txtComboPeriodo.EditValue), txtConjunto.Text, "");

                if (ListaContratos.Count == 0)
                { XtraMessageBox.Show("No se encontraron registros", "Registros", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                //CALCULO
                CalculoDatos(ListaContratos, Convert.ToInt32(txtComboPeriodo.EditValue));

            }
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            if (txtComboPeriodo.Properties.DataSource != null)
            {
                if(Calculo.PeriodoValido(Convert.ToInt32(txtComboPeriodo.EditValue)) == false)
                { XtraMessageBox.Show("Por favor ingresa un periodo válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                ImprimeDocumento(Convert.ToInt32(txtComboPeriodo.EditValue));               
            }
        }

        private void btnImpresionRapida_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            if (txtComboPeriodo.Properties.DataSource != null)
            {
                if (Calculo.PeriodoValido(Convert.ToInt32(txtComboPeriodo)))
                {
                    ImprimeDocumento(Convert.ToInt32(txtComboPeriodo.EditValue), true);  
                }
            }
        }
    }
}