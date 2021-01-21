using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace Labour
{
    /// <summary>
    /// Permite la la creacion de finiquitos.
    /// </summary>
    class Finiquito
    {
        /// <summary>
        /// Representa la informacion de una persona.
        /// </summary>
        private Persona Trabajador;
        /// <summary>
        /// Corresponde a la fecha en que se procesa el finiquito.
        /// </summary>
        private DateTime FechaFiniquito = DateTime.Now.Date;
        /// <summary>
        /// Indica si de dio o no aviso de termino de contrato.
        /// </summary>
        private bool AvisaTermino = false;
        /// <summary>
        /// Indica si se descuenta o no prestamos asociados al trabajador.
        /// </summary>
        private bool DescuentaPrestamo = false;
        /// <summary>
        /// Valor de descuento por seguro de cesantía.
        /// </summary>
        private double SeguroCesantia;

        /// <summary>
        /// Para cargar grilla con mapa de calculos.
        /// </summary>
        private List<PrevFiniquito> ListadoCalculo = new List<PrevFiniquito>();

        /// <summary>
        /// Nos indica si el trabajador tiene una remuneración que es pagada en horas.
        /// </summary>
        private bool PagoHoras { get; set; } = false;

        public Finiquito()
        {

        }

        public Finiquito(Persona pPersona, DateTime pFechaFiniquito, bool pAviso, bool pDescuenta, double pSeguro, bool Horas)
        {
            Trabajador = pPersona;
            FechaFiniquito = pFechaFiniquito;
            AvisaTermino = pAviso;
            DescuentaPrestamo = pDescuenta;
            SeguroCesantia = pSeguro;
            PagoHoras = Horas;
            
        }

        /// <summary>
        /// Calcula finiquito trabajador.
        /// </summary>
        public DataSet CalculaFiniquito()
        {
            double Total = 0, diasVac = 0, totalVac = 0, Anios = 0, Aviso = 0, remun = 0, servicio = 0, AdeudadoMes = 0, TopeIndem = 0;
            double TotalAdeudado = 0, Totalhaberes = 0;
            string Aletras = "", dctos = "";
            int PerAntDespido = 0;
            double RemVac = 0, dc = 0;
            DataSet DataSource = new DataSet();          
                
            if (Trabajador != null)
            {
                if (Trabajador.Ingreso <= Trabajador.Salida)
                {
                    //---------------------------------------------------------------
                    //SI ES ARTICULO 160 O 159 NO HAY DERECHO A INDEMNIZACION.      |
                    //---------------------------------------------------------------
                    //------------------------------------------------------------------------------------------------
                    //NO CONSIDERAR EN EL CALCULO DE LAS REMUNERACIONES:
                    // - Horas Extras.
                    // - Asignaciones Familiares.
                    // - Beneficios o asignaciones que se otorgan en forma esporádica o por una sola vez en el año.
                    // - Considerar solo la gratificacion si esta se paga mensualmente.
                    // - No se debe celebrar un finiquito en el caso de que el contrato no tenga una duracion superio a 30 dias.
                    // - Para el calculo del feriado proporcional no se debe usar como base de calculo la gratificacion.
                    //------------------------------------------------------------------------------------------------

                    //OBTENEMOS INFORMACION REFERENTE A LA CAUSAL DE TERMINO ASOCIADA AL TRABAJADOR.
                    Causal ca = new Causal(Trabajador.codCausal);
                    ca.GetInfo();

                    //PERIODO ANTERIOR A LA FECHA DE TERMINO DE CONTRATO
                    PerAntDespido = fnSistema.fnObtenerPeriodoAnterior(fnSistema.PeriodoFromDate(Trabajador.Salida));

                    //BASE REMUNERACION
                    remun = GetRemuneracion();
                    ListadoCalculo.Add(new PrevFiniquito() { ItemF = "Remuneracion Base", Orden = 1, ValueF = Convert.ToDouble(remun) });

                    //SOLO PARA VACACIONES, NO CONSIDERA COMO BASE DE CALCULO LA GRATIFICACION
                    RemVac = GetRemuneracion(true);
                    ListadoCalculo.Add(new PrevFiniquito() { ItemF = "Remuneración Base para vacacaciones", Orden = 2, ValueF = Convert.ToDouble(RemVac) });

                    //TOPE 90 UF
                    TopeIndem = ValorUfTope(FechaFiniquito);

                    if (remun > TopeIndem && TopeIndem != 0)
                        remun = TopeIndem;

                    if (RemVac > TopeIndem && TopeIndem != 0)
                        RemVac = TopeIndem;

                    ListadoCalculo.Add(new PrevFiniquito() { ItemF = "Remuneración Base (Tope 90 UF)", Orden = 3, ValueF = Convert.ToDouble(RemVac) });

                    //1-PAGO DIAS TRABAJADOS PROPORCIONALMENTE MES DESPIDO
                    //AdeudadoMes = RemuneracionAdeudada();

                    //2-INDEMNIZACION SUSTITUTIVA DEL AVISO PREVIO (SOLO SI ES POR NECESIDADES DE LA EMPRESA ART. 161)
                    if (ca.Aviso == 1 && AvisaTermino == false)
                    {
                        Aviso = remun;
                    }

                    ListadoCalculo.Add(new PrevFiniquito() { ItemF = "Indemnización Sustitutiva del aviso previo", Orden = 4, ValueF = Convert.ToDouble(Aviso) });

                    //3-INDEMNIZACION POR AÑOS DE SERVICIO (NECESIDADES DE LA EMPRESA ART 161)
                    if (ca.Servicio == 1 && Trabajador.Tipocontrato == 0)
                    {
                        Anios = AniosServicio(Trabajador.Ingreso, Trabajador.Salida);
                        //MULTIPLICAMOS LOS AÑOS DE SERVICIO POR EL VALOR DE BASE.
                        servicio = Math.Round(Anios * remun);
                    }
                    ListadoCalculo.Add(new PrevFiniquito() { ItemF = "Años de servicio", Orden = 5, ValueF = Convert.ToDouble(Anios) });
                    ListadoCalculo.Add(new PrevFiniquito() { ItemF = "Indemnización por años de servicio ", Orden = 6, ValueF = Convert.ToDouble(servicio) });

                    //4-INDEMNIZACION POR VACACIONES (PROPORCIONALES Y PROGRESIVAS)
                    diasVac = VacacionesAdeudadas();
                    totalVac = TotalVacaciones(diasVac, RemVac);

                    ListadoCalculo.Add(new PrevFiniquito() { ItemF = $"Indemnizacion por vacaciones ({diasVac} días)", Orden = 7, ValueF = Convert.ToDouble(servicio) });

                    //DESCUENTA PRESTAMOS ADEUDADOS?
                    if (DescuentaPrestamo)
                    {
                        TotalAdeudado = Calculo.TotalDescuentosPendientes(Trabajador.Contrato, Trabajador.PeriodoPersona);
                        dctos = Calculo.GetParametrosDescuentos(Trabajador.Contrato, Trabajador.PeriodoPersona);
                        dc = dctos == "" ? 0 : Convert.ToDouble(dctos);
                        dctos = dctos + $"'{(TotalAdeudado + SeguroCesantia).ToString("N0")}' as TotalDescuentos, ";

                        ListadoCalculo.Add(new PrevFiniquito() { ItemF = "Adeudado por prestamos", Orden = 8, ValueF = Convert.ToDouble(TotalAdeudado) });

                    }

                    ListadoCalculo.Add(new PrevFiniquito() { ItemF = "Seguro de Cesantía", Orden = 9, ValueF = Convert.ToDouble(SeguroCesantia) });

                    //TOTAL
                    Totalhaberes = (AdeudadoMes + Aviso + servicio + totalVac);
                    Total = (AdeudadoMes + Aviso + servicio + totalVac) - TotalAdeudado - SeguroCesantia;

                    if (Total < 0)
                        Total = 0;

                    ListadoCalculo.Add(new PrevFiniquito() { ItemF = "Total Haberes", Orden = 10, ValueF = Convert.ToDouble(Totalhaberes) });
                    ListadoCalculo.Add(new PrevFiniquito() { ItemF = "Total Descuentos", Orden = 11, ValueF = dc });
                    ListadoCalculo.Add(new PrevFiniquito() { ItemF = "Total a pagar", Orden = 12, ValueF = Convert.ToDouble(Total) });

                    Aletras = Conversores.NumeroALetras(Convert.ToDecimal(Total));
                    DataSource = Persona.GetDataFiniquito(Trabajador.Contrato, Trabajador.PeriodoPersona, GetParametros(totalVac.ToString("N0"), Aviso.ToString("N0"), servicio.ToString("N0"), AdeudadoMes.ToString("N0"), Total.ToString("N0"), Aletras, diasVac.ToString("N2"), Anios.ToString("N0"), SeguroCesantia.ToString("N0"), Totalhaberes.ToString("N0")), dctos);              
                    
                }
            }

            return DataSource;
        }

        /// <summary>
        /// Calculo de finiquito por horas
        /// </summary>
        public DataSet CalculaFiniquitoHoras()
        {
            DataSet data = new DataSet();
            string CadenaItems = "";

            if (Trabajador != null)
            {
                if (Trabajador.Ingreso <= Trabajador.Salida)
                {
                    /*SE LE PAGAN COMO FINIQUITO LOS MISMOS ITEMS DE SU LIQUIDACIÓN*/
                    ListadoCalculo.Clear();
                    ListadoCalculo = GetListFiniquitoHoras(Trabajador.Contrato, Trabajador.PeriodoPersona);                    

                    CadenaItems = GetCadRemplazo(ListadoCalculo);
                    data = Persona.GetDataFiniquito(Trabajador.Contrato, Trabajador.PeriodoPersona, CadenaItems, "");
                }
            }

            return data;
        }

        /// <summary>
        /// Nos indica el total adeudado al trabajador por 
        /// los dias trabajados en el mes que se realizó el despido.        
        /// <para>Si fue despedido el 12-12-2018 Se deben pagar los 12 días trabajados en ese ultimo mes.</para>
        /// </summary>
        private double RemuneracionAdeudada()
        {
            double ValorPago = 0;
            if (Trabajador != null)
            {
                ValorPago = Calculo.GetValueFromLiquidacionHistorica(Trabajador.Contrato, Trabajador.PeriodoPersona, "pago");
            }

            return ValorPago;
        }

        /// <summary>
        /// Nos indica la cantidad de años por concepto de años de indemnizacion.
        /// <para>Solo cuando el motido de despido es por necesidad de la empresa o desahucio articulo 161.</para>        
        /// <para>Es legal descontar el 1.6% de seguro de cesantia al monto total por lo que paga la empresa a la cuenta individual.</para>
        /// </summary>
        /// <param name="inicio">Corresponde a la fecha de inicio de contrato</param>
        /// <param name="termino">Corresponde a la fecha de despido</param>
        private int AniosServicio(DateTime inicio, DateTime termino)
        {
            int years = 0;
            DateTime Ultimo = DateTime.Now.Date;
            if (termino > inicio)
            {
                while (inicio <= termino)
                {
                    //AGREGAMOS UN AÑO A LA FECHA
                    inicio = inicio.AddYears(1);

                    //SI FECHA NUEVA (+1 AÑO) sigue siendo menor a la fecha de termino
                    if ((inicio <= termino))
                    {
                        years++;
                        Ultimo = inicio;
                        //ULTIMO AÑO ENCONTRADO
                       // Console.WriteLine(inicio);
                    }
                }

                //SI TODAVÍA QUEDA UNA DIFERENCIA EN MESES MAYOR O IGUAL A 6 SE CONSIDERA OTRO AÑO
                if (Ultimo <= termino)
                {
                    if (GetMeses(termino, Ultimo) >=6)
                        years++;
                }

                //MAXIMO TOPE 11 AÑOS
                if (years > 11)
                    years = 11;             
            }            

            return years;
        }

        /// <summary>
        /// Entrega la cantidad de meses que hay entre dos fechas
        /// </summary>
        /// <param name="pFechaTope">Corresponde a la fecha de termino de contrato</param>
        /// <param name="pUltimoMesEncontrato">Corresponde a la fecha del ultimo año encontrado</param>
        /// <returns></returns>
        private int GetMeses(DateTime pFechaTope, DateTime pUltimoMesEncontrato)
        {
            int meses = 0;
            if (pUltimoMesEncontrato < pFechaTope)
            {
                while (pUltimoMesEncontrato <= pFechaTope)
                {
                    pUltimoMesEncontrato = pUltimoMesEncontrato.AddMonths(1);
                    meses++;
                }
            }

            return (meses - 1);

        }

        /// <summary>
        /// Nos indica el total adeudado por no entregar carta aviso mes anterior despido.
        /// <para>Corresponde a la ultima liquidacion a 30 días</para>  
        /// </summary>
        private double RemuneracionAviso()
        {
            double total = 0;
            if (Trabajador != null)
            {
                //VARIABLE
                if (Trabajador.Regimen == 0)
                {
                    total = Calculo.RemVariableFin(Trabajador.Contrato, Trabajador.Salida);
                }
                //FIJO
                else if (Trabajador.Regimen == 1)
                {
                    total = Calculo.RemFijoFin(Trabajador.Contrato, Trabajador.Salida);
                }
            }

            return total;
        }
        /// <summary>
        /// Nos indica cuantos dias de vacaciones debemos pagarle a una persona.
        /// </summary>        
        private double VacacionesAdeudadas()
        {
            //SOLO SE DEBEN CONDIDERAR LOS DIAS HABILES
            //EJ: SI A UN TRABAJADOR LE QUEDAN 3 DIAS DE VACACIONES Y ES DESPEDIDO EL DIA JUEVES
            //SE LE DEBEN PAGAR 5 DIAS DEVACACIONES POR QUE SOLO SE CONSIDERAN DIAS HABILES
            //ES DECIR --> JUEVES - VIERNES - SABADO - DOMINGO - LUNES
            //CONSIDERAR DATOS DE TABLA FERIADOS Y TABLA VACACION      
            double DiasProporcionales = 0, DiasProgresivos = 0, DiasRestantes = 0, DiasAcum = 0;
            int diasNoHab = 0;
            Hashtable dUsados = new Hashtable();
            DateTime yearProgresivo = DateTime.Now.Date;
            bool ConsideraProgresivo = false;
            DateTime FechaBase = DateTime.Now.Date;
            double CotFaltante = 0, CotReq = 120;

            if (Trabajador != null)
            {
                DiasProporcionales = vacaciones.FeriadosProp(Trabajador.Ingreso, Trabajador.Salida);

                //@ CASO1: NO TIENE COTIZACIONES Y TAMPOCO TIENE FECHA DE PROGRESIVO
                if (Trabajador.AnosProgresivos == 0 || Trabajador.FechaProgresivo > Trabajador.Salida)
                {
                    //NO CONSIDERAMOS DIAS DE COTIZACIONES SI LA FECHA ES 01-01-3000
                    ConsideraProgresivo = false;
                    FechaBase = Trabajador.Ingreso.AddYears(10);
                    //SI FECHA BASE ES MAYOR A LA FECHA EN QUE CUMPLE AÑO (LE CORRESPONDE ANUALIDAD VACACION LO CORREMOS OTRO AÑO)
                }
                //@CASO2: TIENE FECHA DE PROGRESIVO, CONSIDERAMOS MESES DE COTIZACIONES
                else
                {
                    ConsideraProgresivo = true;
                    //DEBEMOS OBTERNER LA FECHA EN LA CUAL SE CUMPLEN LOS 10 AÑOS (SI TIENE MENOS DE 120 COTIZACIONES)
                    if (Trabajador.AnosProgresivos < 120)
                    {
                        //OBTENEMOS LA CANTIDAD DE MESES FALTANTES PARA OBTENER LOS 10 AÑOS DE BASE
                        CotFaltante = CotReq - Trabajador.AnosProgresivos;

                        //SI HAY MESES FALTANTES PARA LOS 10 AÑOS DE BASE OBTENEMOS LA FECHA FINAL (FECHA EN QUE CUMPLIRÍA LOS 10 AÑOS)
                        if (CotFaltante > 0)
                            FechaBase = Trabajador.FechaVacacion.AddMonths(Convert.ToInt32(CotFaltante));
                        //SI NO CONSIDERAMOS LA FECHA INICIAL FECHA VACACIONES
                        else
                            FechaBase = Trabajador.FechaVacacion;
                    }
                    else
                    {
                        //CUMPLE CON LAS 120 COTIZACIONES
                        FechaBase = Trabajador.FechaVacacion;
                    }
                }

                //OBTENEMOS LOS DIAS PROGRESIVOS COMO PUNTO DE PARTIDA LA FECHA BASE OBTENIDA...
                DiasProgresivos = vacaciones.Progresivos(FechaBase, Trabajador.FechaProgresivo, ConsideraProgresivo);

                //yearProgresivo = Trabajador.FechaProgresivo.AddYears(10);                

                //DiasProgresivos = (Trabajador.AnosProgresivos == 10 || yearProgresivo < DateTime.Now)? vacaciones.Progresivos(Trabajador.FechaProgresivo) : 0;                

                DiasAcum = DiasProporcionales + DiasProgresivos;
                

                //DIAS YA USADOS
                dUsados = vacaciones.diasUsados(Trabajador.Contrato);              

                double propUsados = Convert.ToDouble(dUsados["proporcional"]);

                if (dUsados.Count > 0)
                {
                    //AL TOTAL DE DIAS LE RESTAMOS LOS DIAS YA USADOS
                    DiasRestantes = (DiasAcum) - (Convert.ToDouble(dUsados["proporcional"]) + Convert.ToDouble(dUsados["progresivo"]));
                }

                //DIAS NO HABILES DESDE EL DIA SIGUIENTE AL LA FECHA DE TERMINO DE CONTRATO
                diasNoHab = vacaciones.DiasNoHabiles(Trabajador.Salida.AddDays(1), DiasRestantes);

                //TOTAL FINAL
                DiasRestantes = DiasRestantes + diasNoHab;
            }

            return DiasRestantes;
        }
        /// <summary>
        /// Obtiene el total de vacacaciones de por dia de sueldo.
        /// </summary>
        /// <returns></returns>
        private double TotalVacaciones(double pCantidad, double pBase)
        {
            double total = 0;
            double diario = 0;

            //OBTENEMOS EL VALOR X DIA
            diario = Math.Round(pBase / 30);

            //MULTIPLICAMOS CADA DIA DE VACACIONES POR EL VALOR BASE DIARIO.
            total = Math.Round(pCantidad * diario);

            return total;
        }

        /// <summary>
        /// Obtiene la remuneracion para calculos de aviso previo y anios de servicio.
        /// <para>Tope de 90 Uf</para>
        /// </summary>
        /// <returns></returns>
        private double GetRemuneracion(bool? AplicaVac = false)
        {
            double remun = 0;           

            if (Trabajador != null)
            {
                //VARIABLE
                if (Trabajador.RegimenSalario == 0)
                {
                    remun = Calculo.RemVariableFin(Trabajador.Contrato, Trabajador.Salida, AplicaVac);
                }
                //FIJO
                else if (Trabajador.RegimenSalario == 1)
                {
                    remun = Calculo.RemFijoFin(Trabajador.Contrato, Trabajador.Salida, AplicaVac);
                }
            }

            return remun;
        }
        /// <summary>
        /// Genera una subcadena sql para insertar en la sql datasource principal.
        /// </summary>
        /// <param name="pVacaciones">Total pago vacaciones.</param>
        /// <param name="pAviso">Total pago por mes de aviso.</param>
        /// <param name="pServicio">Total pago por años de servicio.</param>
        /// <param name="Adeudado">Total por dias trabajados adeudados.</param>
        /// <param name="pTotal">Total a pagar.</param>
        /// <param name="pPalabras">Total a pagar expresado en palabras.</param>
        /// <returns></returns>
        private string GetParametros(string pVacaciones, string pAviso, string pServicio, string Adeudado, string pTotal, string pPalabras, string pDiasVac, string pAnios, string pSeguro, string pTotalHaberes)
        {
            string sql = "";

            sql = $" '{pVacaciones}' as Vacaciones, '{pAviso}' as AvisoPrevio, '{pServicio}' as Servicio, '{Adeudado}' as Adeudado, '{pTotal}' as Total, '{pPalabras}' as Palabras, '{pDiasVac}' as di, '{pAnios}' as an , '{pSeguro}' as Seguro, '{pTotalHaberes}' as TotalHaberes ";

            return sql;
        }
        /// <summary>
        /// Nos indica el tope de 90 UF para indemnizaciones.
        /// <para>Se considera el mes anterior al mes en que se realiza el finiquito.</para>
        /// </summary>
        /// <param name="pFechaFiniquito"></param>
        /// <returns></returns>
        private double ValorUfTope(DateTime pFechaFiniquito)
        {
            double Total = 0, UfMes = 0;
            int MesAnterior = 0;

            //MES ANTERIOR AL MES DE LA FECHA DEL FINIQUITO
            MesAnterior = fnSistema.fnObtenerPeriodoAnterior(fnSistema.PeriodoFromDate(pFechaFiniquito));
            UfMes = Calculo.GetValueIndice("uf", MesAnterior);

            //TOPE 90 UF
            Total = UfMes * 90;

            return Total;
        }

        /// <summary>
        /// Retorna listado con items para previsualizacion
        /// </summary>
        public List<PrevFiniquito> GetListPrevisualiza()
        {
            return this.ListadoCalculo;
        }


        /// <summary>
        /// Entrega todos los items para finiquito contratos por hora
        /// </summary>
        /// <returns></returns>
        private List<PrevFiniquito> GetListFiniquitoHoras(string pContrato, int pPeriodo)
        {
            List<PrevFiniquito> Listado = new List<PrevFiniquito>();
            string sql = "DECLARE @Haberes decimal(10) \n" +
                         "DECLARE @Dctos  decimal(10) \n" +                      
                         "SET @Haberes = (SELECT sum(valorcalculado) FROM itemtrabajador \n" +
                         "WHERE contrato = @contrato AND anomes = @periodo AND(tipo = 1 OR tipo = 2) AND suspendido=0 \n" +
                         ") \n" +
                         "SET @Dctos = (SELECT SUM(valorcalculado) FROM itemtrabajador \n" +
                         "WHERE contrato = @contrato AND anomes = @periodo AND(tipo = 4 AND coditem <> 'SCEMPRE' and coditem <> 'seginv') AND suspendido = 0) \n" +
                         "SELECT item.tipo, item.orden, item.descripcion as Item, valorcalculado as monto, '0' as Palabras from itemtrabajador \n" +
                         "INNER JOIN item ON item.coditem = itemtrabajador.coditem \n" +
                         "where contrato = @contrato AND anomes = @periodo AND valorcalculado<> 0 \n" +
                         "AND(itemtrabajador.tipo = 1 OR itemtrabajador.tipo = 2 OR itemtrabajador.tipo = 4) \n" +
                         "and(itemtrabajador.coditem <> 'SCEMPRE' AND itemtrabajador.coditem <> 'SEGINV') AND suspendido = 0 \n" +
                         "UNION \n" +
                         "select 10000, 10000, 'Total a pagar' AS Total, ISNULL(@Haberes -@Dctos, 0), dbo.fn_NumeroALetras(ISNULL(@Haberes - @Dctos, 0)) + ' PESOS' \n" +
                         "ORDER BY item.tipo, item.orden";

            SqlConnection cn;
            SqlCommand cmd;
            SqlDataReader rd;
            int count = 1;

            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cmd = new SqlCommand(sql, cn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@contrato", pContrato));
                        cmd.Parameters.Add(new SqlParameter("@periodo", pPeriodo));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                double monto = 0;
                                monto = (rd["monto"] != DBNull.Value) ? Convert.ToDouble(rd["monto"]) : 0;

                                //LLenamos listado
                                Listado.Add(new PrevFiniquito()
                                {
                                    ItemF = (string)rd["Item"],
                                    Orden = count,
                                    ValueF = monto,
                                    Letras = Convert.ToString(rd["Palabras"])
                                });

                                count++;
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

            return Listado;
        }

        /// <summary>
        /// Genera cadena de remplazo para archivo word de creación de finiquito
        /// </summary>
        /// <param name="pListado"></param>
        /// <returns></returns>
        public string GetCadRemplazo(List<PrevFiniquito> pListado)
        {
            StringBuilder str = new StringBuilder();
            string ValorLetras = "";
            try
            {
                if (pListado.Count > 0)
                {
                    str.Append("'");                    
                    foreach (PrevFiniquito x in pListado)
                    {                        
                        str.Append($"{fnSistema.GetFormat(x.ItemF, 100, 2)}\t$ {x.ValueF.ToString("n0")}\n");

                        //Para guardar el monto en letras
                        if (x.Letras != "0")
                            ValorLetras = x.Letras;
                        
                    }
                }
            }
            catch (Exception ex)
            {
                //ERROR...
            }

            str.Append("' as data");
            str.Append($", '{ValorLetras}' as Palabras");
            return str.ToString();
        }
    }

    /// <summary>
    /// Clase para representar todo el calculo que se realiza del finiquito y mostrarlo en una grilla
    /// </summary>
    class PrevFiniquito
    {
        /// <summary>
        /// Nombre del item
        /// </summary>
        public string ItemF { get; set; }
        /// <summary>
        /// Valor del item
        /// </summary>
        public double ValueF { get; set; }

        /// <summary>
        /// Solo para ordenar en grilla
        /// </summary>
        public int Orden { get; set; }

        public string Letras { get; set; } 
    }

    class Causal
    {
        /// <summary>
        /// Codigo interno registro
        /// </summary>
        public int Codigo { get; set; } = -1;
        /// <summary>
        /// Descripcion causal.
        /// </summary>
        public string Descripcion { get; set; }
        /// <summary>
        /// Justificacion de la causal.
        /// </summary>
        public string Justificacion { get; set; }
        /// <summary>
        /// Nos indica si causal aplica indemnizacion por mes de aviso.
        /// </summary>
        public Int16 Aviso { get; set; }
        /// <summary>
        /// Nos indica si causal aplica indemnizacion por años de servicio.
        /// </summary>
        public Int16 Servicio { get; set; }

        public Causal() { }

        public Causal(int pCodigo) {
            Codigo = pCodigo;
        }

        /// <summary>
        /// Obtiene toda la informacion de una causal de termino
        /// </summary>
        public void GetInfo()
        {            
            if (Codigo != -1)
            {
                string sql = "SELECT descCausal, justificacion, indAviso, indServicio FROM causaltermino WHERE codCausal=@pCod";
                SqlCommand cmd;
                SqlConnection cn;
                SqlDataReader rd;
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
                                cmd.Parameters.Add(new SqlParameter("@pCod", Codigo));

                                rd = cmd.ExecuteReader();
                                if (rd.HasRows)
                                {
                                    while (rd.Read())
                                    {
                                        Descripcion = rd["desccausal"].ToString();
                                        Justificacion = rd["justificacion"].ToString();
                                        Aviso = Convert.ToInt16(rd["indAviso"]);
                                        Servicio = Convert.ToInt16(rd["indServicio"]);
                                    }
                                }
                                cmd.Dispose();
                                rd.Close();
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    //ERROR...
                }
            }
        }

        /// <summary>
        /// Genera informacion de nacionalidades
        /// </summary>
        /// <returns></returns>
        public DataTable GetInfoPlantilla()
        {
            DataTable dt = new DataTable();
            string sql = "SELECT codCausal, descCausal, justificacion FROM causalTermino ORDER BY codCausal";
            SqlCommand cmd;
            SqlConnection cn;
            DataSet ds = new DataSet();
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
                            ad.Fill(ds, "Causal");

                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                dt = ds.Tables[0];
                            }

                            ad.Dispose();
                            ds.Dispose();
                            cmd.Dispose();
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

    }
    
}
