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
using System.Transactions;
using DevExpress.XtraReports.UI;
using System.Threading;

namespace Labour
{
    public partial class frmVacacionesColectivas : DevExpress.XtraEditors.XtraForm, IConjuntosCondicionales
    {
        /// <summary>
        /// Sql para eliminar los datos en caso de revertir los cambios.
        /// </summary>
        public string SqlDelRoll { get; set; } = "";

        /// <summary>
        /// Para reporte final.
        /// </summary>
        private XtraReport ReporteUnido = new XtraReport();

        /// <summary>
        /// Para guardar listado seleccionado.
        /// </summary>
        private List<LiquidacionHistorico> ListadoHilo = new List<LiquidacionHistorico>();
        /// <summary>
        /// Representa la fecha de salida de vacaciones.
        /// </summary>
        private DateTime SalidaVac = DateTime.Now.Date;
        /// <summary>
        /// Representa la fecha de termino de las vacaciones.
        /// </summary>
        private DateTime FinalizaVac = DateTime.Now.Date;
        /// <summary>
        /// No sindica si se genera o no reporte comprobante.
        /// </summary>
        private bool GeneraReporte = false;

        /// <summary>
        /// Para manipular la barra de progreso
        /// </summary>
        BarraProgreso Barra;

        /// <summary>
        /// Para mostrar mensaje desde hilo...
        /// </summary>
        /// <param name="pLabel"></param>
        /// <param name="pText"></param>
        delegate void Title(LabelControl pLabel, string pText, bool pVisible);
        /// <summary>
        /// Para manipular botones desde otro hilo.
        /// </summary>
        /// <param name="pButton"></param>
        /// <param name="pEnable"></param>
        delegate void BStop(SimpleButton pButton, bool pEnable);

        /// <summary>
        /// Para obtener informacion de vacaciones para cada trabajador.
        /// </summary>
        private List<vacaciones> ListadoInfoVacaciones = new List<vacaciones>();

        #region "CONDICIONES"
        public void CargarCodigoConjunto(string pConjunto)
        {
            txtConjunto.Text = pConjunto;
        }
        #endregion

        public frmVacacionesColectivas()
        {
            InitializeComponent();
        }


        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void frmVacacionesColectivas_Load(object sender, EventArgs e)
        {
            datoCombobox.spLlenaPeriodos("SELECT anomes FROM parametro ORDER BY anomes DESC", txtComboPeriodo, "anomes", "anomes", true);
            ComboTipo(txtTipo);

            cbTodos.Checked = true;
            btnConjunto.Enabled = false;
            txtConjunto.Enabled = false;
            dtSalida.DateTime = DateTime.Now.Date;
            dtFinaliza.DateTime = DateTime.Now.Date;
            dtRetornoTrabajo.DateTime = DateTime.Now.Date;
            txtTipo.ItemIndex = 0;
            txtComboPeriodo.ReadOnly = true;
            Barra = new BarraProgreso(BarraCalculo, 1, true, true, this);

            //SETEAMOS INFORMACION DE PERSONAS
            string Filtro = Calculo.GetSqlFiltro(User.GetUserFilter(), "", User.ShowPrivadas());
            List<LiquidacionHistorico> ListadoPersonas = Getlistado(Filtro, Convert.ToInt32(txtComboPeriodo.EditValue));
            SetInfoVac(ListadoPersonas);
        }

        #region "DATOS"
        //COMBO TIPO
        //1--> PROPORCIONAL
        //2--> PROGRESIVO
        private void ComboTipo(LookUpEdit ComboTipo)
        {
            List<PruebaCombo> lista = new List<PruebaCombo>();

            //agregamos objetos a la lista
            lista.Add(new PruebaCombo() { key = 1, desc = "PROPORCIONAL" });
            lista.Add(new PruebaCombo() { key = 2, desc = "PROGRESIVO" });

            //PROPIEDADES COMBOBOX
            ComboTipo.Properties.DataSource = lista.ToList();
            ComboTipo.Properties.ValueMember = "key";
            ComboTipo.Properties.DisplayMember = "desc";

            ComboTipo.Properties.PopulateColumns();

            //ocultamos la columna key
            ComboTipo.Properties.Columns[0].Visible = false;

            ComboTipo.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
            ComboTipo.Properties.AutoSearchColumnIndex = 1;
            ComboTipo.Properties.ShowHeader = false;
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
                if (subcadena[1].Length > 2) return false;
                if (subcadena[1] != "50" && subcadena[1] != "00" && subcadena[1] != "5") return false;
                if (subcadena[1].Length == 0) return false;
                if (subcadena[0].Length == 0) return false;
            }
            else
            {
                //SI NO TIENE COMAS SOLO ES UN NUMERO

                return true;
            }
            return true;
        }

        /// <summary>
        /// Genera un listado de contratos.
        /// </summary>
        /// <param name="pSql">Consulta sql</param>
        /// <returns></returns>
        private List<LiquidacionHistorico> Getlistado(string pFiltro, int pPeriodo)
        {
            List<LiquidacionHistorico> Listado = new List<LiquidacionHistorico>();            
            
            string sql = "SELECT contrato, anomes FROM trabajador WHERE anomes=@pPeriodo " + pFiltro;
            SqlConnection cn;
            SqlCommand cmd;
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
                            cmd.Parameters.Add(new SqlParameter("@pPeriodo", pPeriodo));

                            rd = cmd.ExecuteReader();
                            if (rd.HasRows)
                            {
                                while (rd.Read())
                                {
                                    if(Convert.ToInt32(rd["anomes"]) == pPeriodo)
                                        Listado.Add(new LiquidacionHistorico() { Contrato = (string)rd["contrato"],
                                        Periodo = Convert.ToInt32(rd["anomes"])});
                                }
                            }
                            rd.Close();
                            cmd.Dispose();
                        }
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
        /// Indica si la fecha de vacaciones es válida para un conjunto de trabajadores
        /// <para>Verificamos si las fechas son mayor o iguales a las fechas de inicio de vacaciones de cada trabajador</para>
        /// </summary>
        /// <param name="pListado">Listado de contratos conjunto.</param>
        /// <returns></returns>
        private bool FechaNoValida(List<LiquidacionHistorico> pListado, DateTime pSalida, DateTime pFinaliza, DateTime pRetorno)
        {
            bool valida = false;

            Persona person = new Persona();

            if (pListado.Count == 0)
                return true;

            if (pListado.Count > 0)
            {
                //RECORREMOS CADA PERSONA.
                foreach (LiquidacionHistorico item in pListado)
                {
                    person = Persona.GetInfo(item.Contrato, item.Periodo);

                    if (pSalida < person.FechaVacacion || pFinaliza < person.FechaVacacion || pRetorno < person.FechaVacacion)
                    { valida = true; break; }
                }
            }

            return valida;
        }

        /// <summary>
        /// Indica si algun trabajador ya tiene ocupada ese rango de fechas.
        /// </summary>
        /// <param name="pListado">Listado de trabajadores.</param>
        /// <param name="pSalida">Fecha en que comienza el periodo de vacaciones.</param>
        /// <param name="pFinaliza">Fecha en que termina el periodo de vacaciones.</param>
        /// <returns></returns>
        private bool FechaUsada(List<LiquidacionHistorico> pListado, DateTime pSalida, DateTime pFinaliza)
        {
            bool ocupada = false;            
            if (pListado.Count > 0)
            {
                foreach (LiquidacionHistorico item in pListado)
                {                   
                    if (vacaciones.FechaOcupada(pSalida, pFinaliza, item.Contrato))
                    { ocupada = true; break; }
                }
            }

            return ocupada;
        }
        /// <summary>
        /// indica si la persona tiene los dias disponibles necesarios para tomar feriado.
        /// </summary>
        /// <param name="pListado">Listado de personas.</param>
        /// <param name="pDias">Dias de vacaciones.</param>
        /// <param name="pTipoFeriado">progresivo o proporcional.</param>
        /// <returns></returns>
        private bool Disponibles(List<LiquidacionHistorico> pListado, double pDias, int pTipoFeriado)
        {
            vacaciones vac = new vacaciones();
            bool error = false;
            if (pListado.Count > 0)
            {
                foreach (LiquidacionHistorico item in pListado)
                {
                    //DIAS DISPONIBLES                    
                    vac.SetInfo(item.Contrato);

                    //PROPORCIONALES
                    if (pTipoFeriado == 1)
                    {
                        if (pDias > vac.TotalProp)
                        { error = true; break; }
                    }
                    //PROGRESIVAS
                    if (pTipoFeriado == 2)
                    {
                        if (pDias > vac.TotalProg)
                        { error = true; break; }
                    }
                }
            }

            return error;
        }
        /// <summary>
        /// Permite el ingreso masivo de vacaciones.
        /// </summary>
        /// <param name="pListado"></param>
        /// <param name="pSalida"></param>
        /// <param name="pFinaliza"></param>
        /// <param name="pDias"></param>
        /// <param name="pTipo"></param>
        /// <param name="pRetorna"></param>
        /// <returns></returns>
        private bool IngresoMasivo(List<LiquidacionHistorico> pListado, DateTime pSalida, DateTime pFinaliza, 
            double pDias, int pTipo, DateTime pRetorna)
        {
            string sql = "INSERT INTO vacaciondetalle(contrato, salida, finaliza, dias, tipo, retorna, folio, pervac) " +
                        "VALUES(@pContrato, @pSalida, @pFinaliza, @pDias, @pTipo, @pRetorna, @pFolio, @perVac)";

            SqlConnection cn;
            SqlCommand cmd;
            SqlTransaction tr;
            int LastFolio = 0;
            bool TransaccionCorrecta = false;
            Persona person = new Persona();
            string per = "";

            if (pListado.Count > 0 && ListadoInfoVacaciones.Count > 0)
            {
                try
                {
                    cn = fnSistema.OpenConnection();
                    if (cn != null)
                    {
                        using (cn)
                        {
                            tr = cn.BeginTransaction();

                            try
                            {
                                //RECORREMOS E INGRESAMOS DATOS PARA TRABAJADOR DEL LISTADO
                                foreach (LiquidacionHistorico item in pListado)
                                {
                                    foreach (vacaciones vac in ListadoInfoVacaciones)
                                    {
                                        if (vac.contrato == item.Contrato)
                                        {
                                            person = Persona.GetInfo(item.Contrato, item.Periodo);

                                            if (pTipo == 1)
                                              per = vacaciones.PeriodoDiasTomados(person.FechaVacacion, vac.FechaLimiteVacacionesLegales, vac.DiasPropTomados, pDias);
                                            else
                                              per = vacaciones.MesProgresivoUsado(vac.FechaBaseProgresivos, person.FechaProgresivo, vac.diasProgTomados, pDias, true);

                                            using (cmd = new SqlCommand(sql, cn))
                                            {
                                                cmd.Parameters.Add(new SqlParameter("@pContrato", item.Contrato));
                                                cmd.Parameters.Add(new SqlParameter("@pSalida", pSalida));
                                                cmd.Parameters.Add(new SqlParameter("@pFinaliza", pFinaliza));
                                                cmd.Parameters.Add(new SqlParameter("@pDias", pDias));
                                                cmd.Parameters.Add(new SqlParameter("@pTipo", pTipo));
                                                cmd.Parameters.Add(new SqlParameter("@pRetorna", pRetorna));
                                                cmd.Parameters.Add(new SqlParameter("@perVac", per));
                                                //ULTIMO FOLIO
                                                LastFolio = vacaciones.GetLastFolio(item.Contrato) + 1;
                                                cmd.Parameters.Add(new SqlParameter("@pFolio", LastFolio));

                                                cmd.Transaction = tr;
                                                cmd.ExecuteNonQuery();

                                                SqlRollBack($"DELETE from VacacionDetalle WHERE contrato='{item.Contrato}' AND salida='{pSalida.ToString("yyyy-MM-dd")}' AND finaliza='{pFinaliza.ToString("yyyy-MM-dd")}'");
                                            }

                                            break;
                                        }
                                    }
                                }

                                //COMPLETAMOS OPERACION
                                tr.Commit();
                                TransaccionCorrecta = true;
                            }
                            catch (SqlException ex)
                            {
                                TransaccionCorrecta = false;
                            }
                        }
                    }
                 
                }
                catch (TransactionAbortedException ex)
                {
                    //ERROR..
                    TransaccionCorrecta = false;
                }
            }

            return TransaccionCorrecta;
        }
        /// <summary>
        /// Genera sql para borrar informacion de vacaciones en el caso de querer revetir los insert.
        /// </summary>
        /// <param name="pSql"></param>
        private void SqlRollBack(string pSql)
        {          
            SqlDelRoll = SqlDelRoll + $"{pSql}\n";
        }

        /// <summary>
        /// Revertir ingreso de datos.
        /// </summary>
        /// <param name="pSqlRev"></param>
        private void RevertirInsert(string pSqlRev)
        {
            string sql = "BEGIN TRY \n" +
                         "\tBEGIN TRANSACTION \n" +
                         $"{pSqlRev}\n" +
                         "\t\t COMMIT TRANSACTION \n" +
                         "END TRY\n" +
                         "\t\t\tBEGIN CATCH \n" +
                         "\t\t\t\t\tIF @@TRANCOUNT > 0\n" +
                         "\t\t\t\t\t    BEGIN\n" +
                         "\t\t\t\t\t        ROLLBACK\n" +
                         "\t\t\t\t\t    END \n" +
                         "\t\t\t\t\t  END CATCH";

            SqlConnection cn;
            SqlCommand cmd;
            int count = 0;
                        
            if (pSqlRev.Length > 0)
            {
                try
                {
                    cn = fnSistema.OpenConnection();
                    if (cn != null)
                    {
                        using (cn)
                        {
                            using (cmd = new SqlCommand(sql, cn))
                            {
                                count = cmd.ExecuteNonQuery();
                                if (count > 0)
                                {
                                    XtraMessageBox.Show("Cambios realizados correctamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                else
                                {
                                    XtraMessageBox.Show("No se pudo realizar cambios", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                }

                                cmd.Dispose();                                
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        /// <summary>
        /// Actualiza datos de vacaciones.
        /// </summary>
        /// <param name="pListado"></param>
        private void UpdateCalculo(List<LiquidacionHistorico> pListado, DateTime pSalida, DateTime pFinaliza, bool? Reporte = false)
        {
            Persona person = new Persona();
            vacaciones Vac = new vacaciones();
            SqlConnection cn;
            SqlTransaction Tr;
            Hashtable pData = new Hashtable();
            //RptComprobanteVacacion Report = new RptComprobanteVacacion();
            //Reporte externo
            ReportesExternos.rptComprobanteVacacion Report = new ReportesExternos.rptComprobanteVacacion();

            Report.Parameters["imagen"].Value = Imagen.GetLogoFromBd();
            ReporteUnido.Pages.Clear();
            ReporteUnido.CreateDocument();            
            
            if (pListado.Count > 0)
            {
                try
                {
                    cn = fnSistema.OpenConnection();
                    if (cn != null)
                    {
                        Tr = cn.BeginTransaction();
                        try
                        {                           
                            using (cn)
                            {
                                if ((bool)Reporte)
                                {
                                    Barra.ShowControl = true;
                                    Barra.Maximum = pListado.Count;
                                    Barra.Begin();
                                    ShowMessageComp(lblError, "Generando comprobantes...", true);
                                    ButtonStop(btnGenerar, false);
                                    ButtonStop(btnSalir, false);
                                }                              

                                foreach (LiquidacionHistorico item in pListado)
                                {                                  
                                    //OBTENEMOS TODOS LOS DATOS DE LA PERSONA
                                    person = Persona.GetInfo(item.Contrato, item.Periodo);                                    

                                    if (person.Contrato != "")
                                    {
                                        //OBTENEMOS LOS DATOS ACTUALIZADOS DE VACACIONES PARA PERSONA.
                                        Vac = vacaciones.PrimerIngreso(person, pData, Reporte, pFinaliza);
                                        if (Vac != null)
                                        {
                                            //CON LOS DATOS DEL OBJETO HACEMOS INSERT O UPDATE DEPENDIENDO
                                            if (vacaciones.ExistenRegistros(person.Contrato))
                                                UpdateInfo(Vac, cn, Tr);
                                            else
                                                InsertInfo(Vac, cn, Tr);

                                            //GENERAMOS REPORTE
                                            if (pData.Count > 0 && (bool)Reporte)
                                            {
                                                Report = vacaciones.GeneraComprobante(pSalida, pFinaliza, item.Contrato, pData);

                                                //MERGE REPORT
                                                UnirReportes(Report);
                                            }                                                
                                        }                                        
                                    }

                                    Barra.Increase();
                                }

                                Tr.Commit();

                                if (ReporteUnido.Pages.Count > 0 && (bool)Reporte)
                                {
                                    Documento doc = new Documento("", 0);
                                    doc.ShowDocument(ReporteUnido);

                                    Barra.ShowControl = false;
                                    Barra.ShowClose();
                                    ShowMessageComp(lblError, "Comprobante generado correctamente.", true);
                                    ButtonStop(btnGenerar, true);
                                    ButtonStop(btnSalir, true);
                                }                               
                            }
                        }
                        catch (SqlException ex)
                        {
                            //ERROR
                            Tr.Rollback();
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
        /// Ingreso detalle de vacaciones.
        /// </summary>
        /// <param name="pVacaciones">Objeto vacaciones con informacion de vacaciones.</param>
        /// <param name="pConnection"></param>
        /// <param name="pTran"></param>
        private void InsertInfo(vacaciones pVacaciones, SqlConnection pConnection, SqlTransaction pTran)
        {
            if (pVacaciones != null)
            {
                string sql = "INSERT INTO vacacion(contrato, diaspropanual, diasproprestantes, diasproptomados, totalprop, " +
               "diasprog, diasprogtomados, totalprog, totaldias, diaspropanrestantes) VALUES(@contrato, @diaspropanual, @diasproprestantes, " +
               "@diasproptomados, @totalprop, @diasprog, @diasprogtomados, @totalprog, @totaldias, @diasanrestantes)";

                SqlCommand cmd;
                try
                {
                    using (cmd = new SqlCommand(sql, pConnection))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@contrato", pVacaciones.contrato));
                        cmd.Parameters.Add(new SqlParameter("@diaspropanual", pVacaciones.DiasPropAnual));
                        cmd.Parameters.Add(new SqlParameter("@diasproprestantes", pVacaciones.DiasPropRestantes));
                        cmd.Parameters.Add(new SqlParameter("@diasanrestantes", pVacaciones.DiasPropAnRestantes));
                        cmd.Parameters.Add(new SqlParameter("@diasproptomados", pVacaciones.DiasPropTomados));
                        cmd.Parameters.Add(new SqlParameter("@totalprop", pVacaciones.TotalProp));
                        cmd.Parameters.Add(new SqlParameter("@diasprog", pVacaciones.diasProg));
                        cmd.Parameters.Add(new SqlParameter("@diasprogtomados", pVacaciones.diasProgTomados));
                        cmd.Parameters.Add(new SqlParameter("@totalprog", pVacaciones.TotalProg));
                        cmd.Parameters.Add(new SqlParameter("@totaldias", pVacaciones.TotalDias));

                        cmd.Transaction = pTran;
                        cmd.ExecuteNonQuery();
                    }

                    cmd.Dispose();
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        /// <summary>
        /// Actualiza informacion vacaciones.
        /// </summary>
        /// <param name="pVacaciones"></param>
        /// <param name="pConnection"></param>
        /// <param name="pTran"></param>
        private void UpdateInfo(vacaciones pVacaciones, SqlConnection pConnection, SqlTransaction pTran)
        {
            string sql = "UPDATE VACACION SET diaspropanual=@diaspropanual, diasproprestantes=@diasproprestantes, " +
       "diasproptomados=@diasproptomados, totalprop=@totalprop, diasprog=@diasprog, diasprogtomados=@diasprogtomados, " +
       " totalprog=@totalprog, totaldias=@totaldias, diaspropanrestantes=@diaspropanrestantes WHERE contrato=@contrato";

            SqlCommand cmd;
            int res = 0;

            try
            {
                using (cmd = new SqlCommand(sql, pConnection))
                {
                    //PARAMETROS
                    cmd.Parameters.Add(new SqlParameter("@contrato", pVacaciones.contrato));
                    cmd.Parameters.Add(new SqlParameter("@diaspropanual", pVacaciones.DiasPropAnual));
                    cmd.Parameters.Add(new SqlParameter("@diasproprestantes", pVacaciones.DiasPropRestantes));
                    cmd.Parameters.Add(new SqlParameter("@diaspropanrestantes", pVacaciones.DiasPropAnRestantes));
                    cmd.Parameters.Add(new SqlParameter("@diasproptomados", pVacaciones.DiasPropTomados));
                    cmd.Parameters.Add(new SqlParameter("@totalprop", pVacaciones.TotalProp));
                    cmd.Parameters.Add(new SqlParameter("@diasprog", pVacaciones.diasProg));
                    cmd.Parameters.Add(new SqlParameter("@diasprogtomados", pVacaciones.diasProgTomados));
                    cmd.Parameters.Add(new SqlParameter("@totalprog", pVacaciones.TotalProg));
                    cmd.Parameters.Add(new SqlParameter("@totaldias", pVacaciones.TotalDias));

                    //AGREGAMOS A TRANSACCION
                    cmd.Transaction = pTran;
                    cmd.ExecuteNonQuery();

                }
                cmd.Dispose();
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Une uno o mas reportes.
        /// </summary>
        /// <param name="pReport">Reporte</param>
        private void UnirReportes(XtraReport pReport)
        {            

            if (pReport != null)
            {
                pReport.CreateDocument();
                ReporteUnido.Pages.AddRange(pReport.Pages);
            }
        }
        /// <summary>
        /// Para ejecutar metodo UpdateCalculo desde otro hilo.
        /// </summary>
        private void StartHilo()
        {
            UpdateCalculo(ListadoHilo, SalidaVac, FinalizaVac, GeneraReporte);
        }

        private void ShowMessageComp(LabelControl pLabel, string pText, bool pVisible)
        {
            if (this.InvokeRequired)
            {
                Title t = new Title(ShowMessageComp);

                object[] parameters = new object[] { pLabel, pText, pVisible};
                this.Invoke(t, parameters);
            }
            else
            {
                pLabel.Visible = pVisible;
                pLabel.Text = pText;                
            }
        }

        private void ButtonStop(SimpleButton pButton, bool pEnable)
        {
            if (this.InvokeRequired)
            {
                BStop bt = new BStop(ButtonStop);

                object[] parameters = new object[] { pButton, pEnable };

                this.Invoke(bt, parameters);
            }
            else
            {
                pButton.Enabled = pEnable;
            }
        }
        /// <summary>
        /// Setea listado con informacion de todos los trabajadores.
        /// </summary>
        /// <param name="pListado"></param>
        private void SetInfoVac(List<LiquidacionHistorico> pListado)
        {
            ListadoInfoVacaciones.Clear();
            Persona person = new Persona();
            Hashtable pTabla = new Hashtable();
            vacaciones Vac = new vacaciones();
            if (pListado.Count > 0)
            {
                //RECORREMOS PERSONAS
                foreach (LiquidacionHistorico item in pListado)
                {
                    person = Persona.GetInfo(item.Contrato, item.Periodo);
                    if (person != null)
                    {
                        //Agregamos data a listado
                        Vac = vacaciones.PrimerIngreso(person, pTabla);
                        if(Vac != null)
                            ListadoInfoVacaciones.Add(vacaciones.PrimerIngreso(person, pTabla));
                    }
                }
            }         
        }

        #endregion

        private void cbTodos_CheckedChanged(object sender, EventArgs e)
        {
            if (cbTodos.Checked)
            {
                txtConjunto.Enabled = false;
                txtConjunto.Text = "";
                btnConjunto.Enabled = false;
            }
            else
            {
                txtConjunto.Enabled = true;
                txtConjunto.Text = "";
                txtConjunto.Focus();
                btnConjunto.Enabled = true;
            }
        }

        private void btnConjunto_Click(object sender, EventArgs e)
        {
            if (cbTodos.Checked == false)
            {
                FrmFiltroBusqueda filtro = new FrmFiltroBusqueda(true);
                filtro.opener = this;
                filtro.StartPosition = FormStartPosition.CenterParent;
                filtro.ShowDialog(); 
            }
        }

        private void txtDiasVac_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar) || e.KeyChar == (char)44)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void txtDiasVac_KeyDown(object sender, KeyEventArgs e)
        {
            TextEdit dias = sender as TextEdit;

            if (e.KeyData == Keys.Enter)
            {
                if (dias.Text == "")
                { XtraMessageBox.Show("Por favor ingresa un día válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                bool valido = fnDecimal(dias.Text);
                if (valido == false) { XtraMessageBox.Show("Valor no valido para dias", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtDiasVac.Focus(); return; }

                if (Convert.ToDouble(dias.Text) == 0)
                { XtraMessageBox.Show("Por favor ingresa un día válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }                

                dtFinaliza.DateTime = vacaciones.AgregarDiasFinSemana(dtSalida.DateTime, Convert.ToDouble(dias.Text));
                dtRetornoTrabajo.DateTime = vacaciones.DiaRetornoTrabajo(dtSalida.DateTime, Convert.ToDouble(dias.Text), 3);
            }
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Tab)
            {
                if (txtDiasVac.ContainsFocus)
                {
                    if (txtDiasVac.Text == "")
                    { XtraMessageBox.Show("Por favor ingresa un día válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    bool valido = fnDecimal(txtDiasVac.Text);
                    if (valido == false) { XtraMessageBox.Show("Valor no valido para dias", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtDiasVac.Focus(); return false; }

                    if (Convert.ToDouble(txtDiasVac.Text) == 0)
                    { XtraMessageBox.Show("Por favor ingresa un día válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }                    

                    dtFinaliza.DateTime = vacaciones.AgregarDiasFinSemana(dtSalida.DateTime, Convert.ToDouble(txtDiasVac.Text));
                    dtRetornoTrabajo.DateTime = vacaciones.DiaRetornoTrabajo(dtSalida.DateTime, Convert.ToDouble(txtDiasVac.Text), 1);
                }
            }

            return base.ProcessDialogKey(keyData);
        }

        private void btnGenerar_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            ListadoHilo.Clear();

            if (txtTipo.EditValue == null || txtTipo.Properties.DataSource == null)
            { XtraMessageBox.Show("Por favor selecciona un tipo de vacacion", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (txtComboPeriodo.Properties.DataSource == null)
            { XtraMessageBox.Show("Por favor selecciona un periodo válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            string Filtro = "";
            List<LiquidacionHistorico> ListadoPersonas = new List<LiquidacionHistorico>();

            if (fnDecimal(txtDiasVac.Text) == false)
            { XtraMessageBox.Show("Por favor ingresa un dia válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtDiasVac.Focus(); return; }

            if (cbTodos.Checked)
            {
                //TODOS LOS REGISTROS DEL PERIODO
                //GENERAR LISTADO DE CONTRATOS DE acuerdo a filtro
                Filtro = Calculo.GetSqlFiltro(User.GetUserFilter(), "", User.ShowPrivadas());
                ListadoPersonas = Getlistado(Filtro, Convert.ToInt32(txtComboPeriodo.EditValue));            
            }
            else
            {
                //CONJUNTO
                if (Conjunto.ExisteConjunto(txtConjunto.Text) == false)
                { XtraMessageBox.Show("Por favor ingresa un conjunto válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtConjunto.Focus(); return; }

                //GENERAR LISTADO DE CONTRATOS DE acuerdo a filtro
                Filtro = Calculo.GetSqlFiltro(User.GetUserFilter(), txtConjunto.Text, User.ShowPrivadas());
                ListadoPersonas = Getlistado(Filtro, Convert.ToInt32(txtComboPeriodo.EditValue));               
            }

            if (ListadoPersonas.Count == 0)
            { XtraMessageBox.Show("No se encontró información", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (ListadoPersonas.Count > 0)
            {
                //FECHA MENOR A FECHA DE INICIO DE CONTRATO??
                if (FechaNoValida(ListadoPersonas, Convert.ToDateTime(dtSalida.EditValue), Convert.ToDateTime(dtFinaliza.EditValue), Convert.ToDateTime(dtRetornoTrabajo.EditValue)))
                { XtraMessageBox.Show("Las fechas ingresadas no son válidas\nPor favor verifica la información y vuelve a intentarlo", "Vacaciones", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                if (FechaUsada(ListadoPersonas, Convert.ToDateTime(dtSalida.EditValue), Convert.ToDateTime(dtFinaliza.EditValue)))
                { XtraMessageBox.Show("Por favor verifica que las fechas no estén ya ocupadas", "Fecha ocupadas", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                if (Disponibles(ListadoPersonas, Convert.ToDouble(txtDiasVac.Text), Convert.ToInt32(txtTipo.EditValue)))
                { XtraMessageBox.Show("No hay dias disponibles", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                //SETEAMOS PROPIEDADES PARA PODER ACCEDER DESDE HILO
                FinalizaVac = Convert.ToDateTime(dtFinaliza.EditValue);
                SalidaVac = Convert.ToDateTime(dtSalida.EditValue);
                ListadoHilo = ListadoPersonas;                

                DialogResult Advertencia = XtraMessageBox.Show($"¿Estás seguro de descontar {Convert.ToDouble(txtDiasVac.Text)} días de vacaciones {(Convert.ToInt32(txtTipo.EditValue) == 1? "Legales?":"Progresivas?")}", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (Advertencia == DialogResult.Yes)
                {
                    if (IngresoMasivo(ListadoPersonas, Convert.ToDateTime(dtSalida.EditValue), Convert.ToDateTime(dtFinaliza.EditValue), Convert.ToDouble(txtDiasVac.Text), Convert.ToInt32(txtTipo.EditValue), Convert.ToDateTime(dtRetornoTrabajo.EditValue)))
                    {
                        DialogResult Ad = XtraMessageBox.Show("Registro realizado correctamente", "Vacaciones", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                        if (Ad == DialogResult.Cancel)
                        {
                            //REVERTIMOS OPERACION
                            DialogResult Adv = XtraMessageBox.Show("¿Estás seguro que deseas revertir esta operación?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (Adv == DialogResult.Yes)
                            {
                                GeneraReporte = false;
                                RevertirInsert(SqlDelRoll);                               
                            }
                        }
                        else
                        {
                            GeneraReporte = true;                            
                        }

                        ThreadStart del = new ThreadStart(StartHilo);
                        Thread Hilo = new Thread(del);
                        Hilo.Start();

                        //UpdateCalculo(ListadoPersonas, Convert.ToDateTime(dtSalida.EditValue), Convert.ToDateTime(dtFinaliza.EditValue), true);
                    }
                    else
                    {
                        XtraMessageBox.Show("No se pudo realizar operacion", "Error ingreso", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                }                   
            }
        }

        private void txtConjunto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (Conjunto.ExisteConjunto(txtConjunto.Text) == false)
                { lblError.Visible = true; lblError.Text = "Condición no válida"; txtConjunto.Focus(); return; }

                lblError.Visible = false;
            }
        }
    }
}