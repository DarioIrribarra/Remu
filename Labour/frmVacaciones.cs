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
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors.Calendar;
using DevExpress.Utils;
using DevExpress.XtraScheduler;
using System.IO;

namespace Labour
{
    public partial class frmVacaciones : DevExpress.XtraEditors.XtraForm
    {
        [DllImport("user32")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32")]
        static extern double EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        const int MF_BYCOMMAND = 0;
        const int MF_DISABLED = 2;
        const int SC_CLOSED = 0xF060;

        //VARIABLE PARA GUARDAR EL NUMERO DE CONTRATO ASOCIADO
        private string contrato = "";

        //PERIODO REGISTRO
        private int PeriodoRegistro = 0;

        //VARIABLE PARA UPDATES Y INSERT
        private bool Update = false;

        //VARIABLE PARA FECHA PROGRESIVO
        private DateTime FechaProgresivo = DateTime.Now.Date;

        //REPRESENTA AL TRABAJADOR
        Persona Trabajador;

        Operacion op;

        /// <summary>
        /// Data para parametros de reporte...
        /// </summary>
        Hashtable DataReporte = new Hashtable();

        vacaciones DatoVacaciones = new vacaciones();

        public frmVacaciones(string contrato, int pPeriodo)
        {
            InitializeComponent();
            this.contrato = contrato;
            this.PeriodoRegistro = pPeriodo;
        }

        private void frmVacaciones_Load(object sender, EventArgs e)
        {                   
            var ms = GetSystemMenu(Handle, false);
            EnableMenuItem(ms, SC_CLOSED, MF_BYCOMMAND | MF_DISABLED);

            //VARIABLES
            bool existeRegistro = false;
            string grilla = "";
            calendarioVacacion.ToolTipController = toolTipController1;
            //vacaciones Vac = new vacaciones();

            ComboTipo(txtTipo);

            //PROPIEDADES 
            DefaultProperties();

            if (contrato != "" && PeriodoRegistro !=0)
            {
                //Seteamos las propiedades de la persona.
                Trabajador = new Persona();
                Trabajador = Persona.GetInfo(contrato, PeriodoRegistro);

                lblNombre.Text = $"Contrato: {Trabajador.Contrato}  - {Trabajador.NombreCompleto}";
                groupDetalleView.Text ="DETALLE VACACIONES: " +  Trabajador.NombreCompleto;
                FechaProgresivo = Trabajador.FechaProgresivo;         
                //EXISTE CONTRATO EN TABLA VACACIONES
                existeRegistro = ExisteRegistroInfo(contrato);

                op = new Operacion();

                //SI VARIABLE EXISTE REGISTRO RETORNA TRUE ES PORQUE EL REGISTRO DE INFORMACION DE VACACIONES YA EXISTE
                //EN ESTE CASO SOLO DEBEMOS ACTUALIZAR
                //CASO CONTRARIO INGRESAMOS EL REGISTRO POR PRIMERA VEZ!

                if (Trabajador.PeriodoPersona == Calculo.PeriodoObservado)
                {
                    //SOLO GUARDAMOS POR PRIMERA VEZ TODOS LOS REGISTROS
                    //GUARDAR REGISTRO
                    //PrimerIngreso(FechaProgresivo, existeRegistro);
                    DatoVacaciones = vacaciones.PrimerIngreso(Trabajador, DataReporte);
                    if (DatoVacaciones != null)
                    {
                        if (existeRegistro)
                            vacaciones.ModificarInformacionVacacion(DatoVacaciones, Trabajador.Contrato);
                        else
                            vacaciones.NuevaInformacionVacacion(DatoVacaciones, Trabajador.Contrato);                            
                    }

                    //CARGAMOS DATOS EN CAJAS DE TEXTO
                    CargaInfoVacacion(contrato);                  
                }

                //MOSTRAR GRILLA CON DATOS
                grilla = string.Format("SELECT salida, finaliza, dias, tipo, retorna FROM vacaciondetalle WHERE contrato='{0}' ORDER BY salida desc ", contrato);
              
                fnSistema.spllenaGridView(gridVacaciones, grilla);
                fnSistema.spOpcionesGrilla(viewVacaciones);
                if (viewVacaciones.RowCount > 0)
                {
                    ColumnasGrilla();
                    CargarCamposFromGrid(0);
                }                
            }      
        }

        #region "MANEJO DE DATOS"
        //INGRESAR NUEVO REGISTRO VACACIONES
        private void NuevaInformacionVacacion(decimal diaspropAnual, decimal diaspropRestantes, decimal diaspropTomados,
            decimal totalProp, decimal diasProg, decimal diasProgTomados, decimal totalProg, decimal totalDias, decimal diasanRestantes)
        {
            string sql = "INSERT INTO vacacion(contrato, diaspropanual, diasproprestantes, diasproptomados, totalprop, " +
                "diasprog, diasprogtomados, totalprog, totaldias, diaspropanrestantes) VALUES(@contrato, @diaspropanual, @diasproprestantes, " +
                "@diasproptomados, @totalprop, @diasprog, @diasprogtomados, @totalprog, @totaldias, @diasanrestantes)";
            SqlCommand cmd;
            int res = 0;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@contrato", contrato));
                        cmd.Parameters.Add(new SqlParameter("@diaspropanual", diaspropAnual));
                        cmd.Parameters.Add(new SqlParameter("@diasproprestantes", diaspropRestantes));
                        cmd.Parameters.Add(new SqlParameter("@diasproptomados", diaspropTomados));
                        cmd.Parameters.Add(new SqlParameter("@totalprop", totalProp));
                        cmd.Parameters.Add(new SqlParameter("@diasprog", diasProg));
                        cmd.Parameters.Add(new SqlParameter("@diasprogtomados", diasProgTomados));
                        cmd.Parameters.Add(new SqlParameter("@totalprog", totalProg));
                        cmd.Parameters.Add(new SqlParameter("@totaldias", totalDias));
                        cmd.Parameters.Add(new SqlParameter("@diasanrestantes", diasanRestantes));

                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            //INGRESO CORRECTO
                        }
                        else
                        {
                            //SE PRODUJO ALGUN ERROR
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
        }

        //MODIFICAR REGISTRO VACACIONES
        private void ModificarInformacionVacacion(decimal diaspropAnual, decimal diaspropRestantes, decimal diaspropTomados,
            decimal totalProp, decimal diasProg, decimal diasProgTomados, decimal totalProg, decimal totalDias, decimal diasanRestantes)
        {
            string sql = "UPDATE VACACION SET diaspropanual=@diaspropanual, diasproprestantes=@diasproprestantes, " +
                "diasproptomados=@diasproptomados, totalprop=@totalprop, diasprog=@diasprog, diasprogtomados=@diasprogtomados, " +
                " totalprog=@totalprog, totaldias=@totaldias, diaspropanrestantes=@diaspropanrestantes WHERE contrato=@contrato";
            SqlCommand cmd;
            int res = 0;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@contrato", contrato));
                        cmd.Parameters.Add(new SqlParameter("@diaspropanual", diaspropAnual));
                        cmd.Parameters.Add(new SqlParameter("@diasproprestantes", diaspropRestantes));
                        cmd.Parameters.Add(new SqlParameter("@diasproptomados", diaspropTomados));
                        cmd.Parameters.Add(new SqlParameter("@totalprop", totalProp));
                        cmd.Parameters.Add(new SqlParameter("@diasprog", diasProg));
                        cmd.Parameters.Add(new SqlParameter("@diasprogtomados", diasProgTomados));
                        cmd.Parameters.Add(new SqlParameter("@totalprog", totalProg));
                        cmd.Parameters.Add(new SqlParameter("@totaldias", totalDias));
                        cmd.Parameters.Add(new SqlParameter("@diaspropanrestantes", diasanRestantes));

                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            //ACTUALIZACION CORRECTA!
                        }
                        else
                        {
                            //ERROR AL INTENTAR ACTUALIZAR
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
        }

        //PARA INGRESO DE REGISTRO VACACIONES
        private void IngresoVacacion(Persona pTrabajador, DateEdit Salida, DateEdit Finaliza, TextEdit dias, LookUpEdit Tipo, DateEdit RetornoTrabajo)
        {
            string sql = "INSERT INTO VACACIONDETALLE(contrato, salida, finaliza, dias, tipo, retorna, folio, pervac) " +
                "VALUES(@contrato, @salida, @finaliza, @dias, @tipo, @retorna, @pFolio, @perVac)";
            SqlCommand cmd;
            string grilla = "", PerUsados = "";
            int res = 0, LastFolio = 0;

            //RptComprobanteVacacion Comp = new RptComprobanteVacacion();
            //Reporte externo
            ReportesExternos.rptComprobanteVacacion Comp = new ReportesExternos.rptComprobanteVacacion();
            Comp.LoadLayoutFromXml(Path.Combine(fnSistema.RutaCarpetaReportesExterno, "rptComprobanteVacacion.repx"));

            //Comp.Parameters["imagen"].Value = Imagen.GetLogoFromBd();
            vacaciones Vac = new vacaciones();
            Documento Doc = new Documento("", 0);

            Hashtable dataCalculo = new Hashtable();
            LastFolio = UltimoFolio(pTrabajador.Contrato);

            //CALCULAMOS LA FECHA DE FIN DE VACACIONES (ULTIMO DIA DE VACACIONES)
            DateTime FinalVacaciones = vacaciones.AgregarDiasFinSemana(Salida.DateTime, Math.Round(Convert.ToDouble(dias.Text)));
            //FECHA EN LA QUE REGRESA AL TRABAJO
            DateTime finalRetornoTrabajo = vacaciones.DiaRetornoTrabajo(Salida.DateTime, Math.Round(Convert.ToDouble(dias.Text)), pTrabajador.Jornada);

            if ((Convert.ToDateTime(Salida.EditValue) == Convert.ToDateTime(Finaliza.EditValue)) && Convert.ToDouble(dias.Text) > 1)
            { XtraMessageBox.Show("Por favor verifica las fechas", "Fechas incorrectas", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            //PROPORCIONALES?
            if (Convert.ToInt32(Tipo.EditValue) == 1)
               PerUsados = vacaciones.PeriodoDiasTomados(pTrabajador.FechaVacacion, DatoVacaciones.FechaLimiteVacacionesLegales, DatoVacaciones.DiasPropTomados, Convert.ToDouble(dias.Text));
            else
            //PROGRESIVOS?
               PerUsados = vacaciones.MesProgresivoUsado(DatoVacaciones.FechaBaseProgresivos, pTrabajador.FechaProgresivo, DatoVacaciones.diasProgTomados, Convert.ToInt32(dias.Text), true);

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@contrato", pTrabajador.Contrato));
                        cmd.Parameters.Add(new SqlParameter("@salida", Convert.ToDateTime(Salida.EditValue)));
                        cmd.Parameters.Add(new SqlParameter("@finaliza", Convert.ToDateTime(Finaliza.EditValue)));
                        cmd.Parameters.Add(new SqlParameter("@dias", Convert.ToDecimal(dias.Text)));
                        cmd.Parameters.Add(new SqlParameter("@tipo", Convert.ToInt16(Tipo.EditValue)));
                        cmd.Parameters.Add(new SqlParameter("@retorna", Convert.ToDateTime(RetornoTrabajo.EditValue)));
                        cmd.Parameters.Add(new SqlParameter("@pFolio", LastFolio));
                        cmd.Parameters.Add(new SqlParameter("@perVac", PerUsados));

                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            //INGRESO CORRECTO
                            XtraMessageBox.Show("Ingreso Correcto", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            //GUARDAR EVENTO EN LOG REGISTRO
                            logRegistro log = new logRegistro(User.getUser(), "SE INGRESA NUEVO REGISTRO DE VACACIONES PARA CONTRATO " + pTrabajador.Contrato, "VACACIONDETALLE", "0", "[" + (DateTime)Salida.EditValue + ";" + (DateTime)Finaliza.EditValue + "]", "INGRESAR");
                            log.Log();

                            //RECARGAR GRILLA
                            grilla = string.Format("SELECT salida, finaliza, dias, tipo, retorna FROM vacaciondetalle WHERE contrato='{0}' ORDER BY salida desc", pTrabajador.Contrato);
                            fnSistema.spllenaGridView(gridVacaciones, grilla);
                            ColumnasGrilla();

                            //RECALCULAR DATOS INFORMACION VACACIONES
                            //RecalculoData(FechaProgresivo);

                            //PrimerIngreso(FechaProgresivo, true, true, Convert.ToDateTime(Finaliza.EditValue));
                            DatoVacaciones = vacaciones.PrimerIngreso(pTrabajador, DataReporte, true, Convert.ToDateTime(Finaliza.EditValue));
                            if (DatoVacaciones != null)
                            {
                                vacaciones.ModificarInformacionVacacion(DatoVacaciones, pTrabajador.Contrato);
                            }

                            //RECARGAMOS DATOS EN CAJAS DE TEXTO
                            CargaInfoVacacion(pTrabajador.Contrato);

                            //PREGUNTAMOS SI DESEA IMPRIMIR COMPROBANTE DE VACACIONES
                            DialogResult pregunta = XtraMessageBox.Show("¿Desea generar comprobante de vacaciones?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (pregunta == DialogResult.Yes)
                            {
                                //dataCalculo = CalculoDias(FechaProgresivo, finalRetornoTrabajo);
                               Comp = (ReportesExternos.rptComprobanteVacacion)vacaciones.GeneraComprobante(Convert.ToDateTime(Salida.EditValue), Convert.ToDateTime(Finaliza.EditValue), pTrabajador.Contrato, DataReporte);
                                if (Comp != null)
                                    Doc.ShowDocument(Comp);
                            }

                            CargarCamposFromGrid(0);
                            op.Cancela = false;
                            op.SetButtonProperties(btnNuevo, 1);
                        }
                        else
                        {
                            //ERROR!!!!
                            XtraMessageBox.Show("Error al intentar Guardar", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        }

        //PARA MODIFICAR REGISTRO VACACIONES
        private void ModificarVacacion(Persona pTrabajador, DateEdit Salida, DateEdit Finaliza, 
            TextEdit dias, LookUpEdit Tipo, DateTime salidadb, DateTime regresodb, DateEdit RetornoTrabajo, double pDiasBd)
        {
            string sql = "UPDATE VACACIONDETALLE SET salida=@salida, finaliza=@finaliza, dias=@dias, " +
                        "tipo=@tipo, retorna=@retorna, pervac=@perVac " +
                        "WHERE contrato=@contrato AND salida=@salidadb AND finaliza=@regresodb";
            SqlCommand cmd;
            string grilla = "", PerUsados = "";
            int res = 0;
            double DiasTomFecha = 0;

            vacaciones Vac = new vacaciones();
            //RptComprobanteVacacion Comp = new RptComprobanteVacacion();
            //Reporte externo
            ReportesExternos.rptComprobanteVacacion Comp = new ReportesExternos.rptComprobanteVacacion();
            Comp.LoadLayoutFromXml(Path.Combine(fnSistema.RutaCarpetaReportesExterno, "rptComprobanteVacacion.repx"));

            //Comp.Parameters["imagen"].Value = Imagen.GetLogoFromBd();
            Documento doc = new Documento("", 0);

            //TABLA HASH PRECARGA
            Hashtable Data = new Hashtable();
            Data = precargaVacaciones(pTrabajador.Contrato, salidadb, regresodb);

            Hashtable dataCalculo = new Hashtable();

            //OBTENEMOS LOS DIAS TOMADOS HASTA LA FECHA ANTERIOR AL REGISTRO A EDITAR...
            if(Convert.ToInt32(Tipo.EditValue) == 1)
                DiasTomFecha = vacaciones.DiasTomadosAnt(pTrabajador.Contrato, salidadb, 1);
            else
                DiasTomFecha = vacaciones.DiasTomadosAnt(pTrabajador.Contrato, salidadb, 2);

            //CALCULAMOS LA FECHA DE FIN DE VACACIONES (ULTIMO DIA DE VACACIONES)
            DateTime FinalVacaciones = vacaciones.AgregarDiasFinSemana(Salida.DateTime, Math.Round(Convert.ToDouble(dias.Text)));
            //FECHA EN LA QUE REGRESA AL TRABAJO
            DateTime Retorno = vacaciones.DiaRetornoTrabajo(Salida.DateTime, Math.Round(Convert.ToDouble(dias.Text)), pTrabajador.Jornada);

            if ((Convert.ToDateTime(Salida.EditValue) == Convert.ToDateTime(Finaliza.EditValue)) && Convert.ToDouble(dias.Text) > 1)
            { XtraMessageBox.Show("Por favor verifica las fechas", "Fechas incorrectas", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

           //PROPORCIONALES?
            if (Convert.ToInt32(Tipo.EditValue) == 1)
                PerUsados = vacaciones.PeriodoDiasTomados(pTrabajador.FechaVacacion, DatoVacaciones.FechaLimiteVacacionesLegales, DiasTomFecha, Convert.ToDouble(dias.Text));
            else
                //PROGRESIVOS?
                PerUsados = vacaciones.MesProgresivoUsado(DatoVacaciones.FechaBaseProgresivos, pTrabajador.FechaProgresivo, DiasTomFecha, Convert.ToDouble(dias.Text), true);

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@salida", Convert.ToDateTime(Salida.EditValue)));
                        cmd.Parameters.Add(new SqlParameter("@finaliza", Convert.ToDateTime(Finaliza.EditValue)));
                        cmd.Parameters.Add(new SqlParameter("@dias", Convert.ToDecimal(dias.Text)));
                        cmd.Parameters.Add(new SqlParameter("@tipo", Convert.ToInt16(Tipo.EditValue)));
                        cmd.Parameters.Add(new SqlParameter("@contrato", contrato));
                        cmd.Parameters.Add(new SqlParameter("@salidadb", salidadb));
                        cmd.Parameters.Add(new SqlParameter("@regresodb", regresodb));
                        cmd.Parameters.Add(new SqlParameter("@retorna", Convert.ToDateTime(RetornoTrabajo.EditValue)));
                        cmd.Parameters.Add(new SqlParameter("@perVac", PerUsados));

                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            //UPDATE CORRECTO!
                            XtraMessageBox.Show("Actualizacion Correcta", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            //VERIFICAMOS I HUBO CAMBIOS Y GUARDAMOS EN LOG REGISTRO
                            ComparaVacacion(pTrabajador.Contrato, Convert.ToDateTime(Salida.EditValue), Convert.ToDateTime(Finaliza.EditValue), 
                                Convert.ToDecimal(dias.Text), Convert.ToInt16(Tipo.EditValue), Data);

                            //RECARGAMOS GRILLA
                            grilla = string.Format("SELECT salida, finaliza, dias, tipo, retorna FROM vacaciondetalle WHERE contrato='{0}' ORDER BY salida desc", pTrabajador.Contrato);
                            fnSistema.spllenaGridView(gridVacaciones, grilla);
                            ColumnasGrilla();

                            //VOLVER A RECALCULAR LOS DATOS DE VACACIONES (PROPORCIONALES, PROGRESIVOS)
                            //RecalculoData(FechaProgresivo);
                            //PrimerIngreso(FechaProgresivo, true, true, Convert.ToDateTime(Finaliza.EditValue));
                            DatoVacaciones = vacaciones.PrimerIngreso(pTrabajador, DataReporte, true, Convert.ToDateTime(Finaliza.EditValue));

                            if (DatoVacaciones != null)
                            {
                                vacaciones.ModificarInformacionVacacion(DatoVacaciones, pTrabajador.Contrato);
                            }

                            //RECARGAMOS DATOS EN CAJAS DE TEXTO
                            CargaInfoVacacion(pTrabajador.Contrato);

                            //PREGUNTA SI SE DESEA GENERAR COMPROBANTE DE VACACIONES
                            DialogResult pregunta = XtraMessageBox.Show("¿Desea generar comprobante de vacaciones?", "Informacion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (pregunta == DialogResult.Yes)
                            {
                                //dataCalculo = CalculoDias(FechaProgresivo, FinalVacaciones);
                                Comp = (ReportesExternos.rptComprobanteVacacion)vacaciones.GeneraComprobante((DateTime)Salida.EditValue, Convert.ToDateTime(Finaliza.EditValue), pTrabajador.Contrato, DataReporte);
                                if (Comp != null)
                                    doc.ShowDocument(Comp);
                            }

                            CargarCamposFromGrid();
                        }
                        else
                        {
                            //ERROR AL INTENTAR MODIFICAR!!!!!
                            XtraMessageBox.Show("Error al intentar guardar", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
        }

        //ELIMINAR REGISTRO DE VACACIONES
        private void EliminarVacacion(Persona pTrabajador, DateTime salida, DateTime finaliza)
        {
            string sql ="DELETE FROM VACACIONDETALLE WHERE contrato=@contrato AND salida=@salida AND finaliza=@finaliza";           
            SqlCommand cmd;
            int res = 0;
            string grilla = "";
            vacaciones Vac = new vacaciones();
            DialogResult pregunta = XtraMessageBox.Show("¿Realmente desea eliminar registro conprendido entre " + salida.ToString() + " y " + finaliza.ToString() + "?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (pregunta == DialogResult.Yes)
            {
                try
                {
                    if (fnSistema.ConectarSQLServer())
                    {
                        using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@contrato", pTrabajador.Contrato));
                            cmd.Parameters.Add(new SqlParameter("@salida", salida));
                            cmd.Parameters.Add(new SqlParameter("@finaliza", finaliza));

                            res = cmd.ExecuteNonQuery();
                            if (res > 0)
                            {
                                XtraMessageBox.Show("Registro Eliminado correctamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                //GUARDAR REGISTRO EN BD
                                logRegistro log = new logRegistro(User.getUser(), "SE ELIMINA REGISTRO DE VACACIONES ASOCIADO A CONTRATO "+ pTrabajador.Contrato, "VACACIONDETALLE", "[" + salida.ToString() + ";" + finaliza.ToString() + "]", "0", "ELIMINAR");
                                log.Log();

                                //RECARGAR GRILLA...
                                grilla = string.Format("SELECT salida, finaliza, dias, tipo, retorna FROM vacaciondetalle WHERE contrato='{0}' ORDER BY salida desc", pTrabajador.Contrato);
                                fnSistema.spllenaGridView(gridVacaciones, grilla);

                                //RECALCULAR DATA 
                                //PrimerIngreso(FechaProgresivo, true, false, null);
                                DatoVacaciones = vacaciones.PrimerIngreso(pTrabajador, DataReporte);
                                if (DatoVacaciones != null)
                                {
                                    vacaciones.ModificarInformacionVacacion(DatoVacaciones, pTrabajador.Contrato);
                                }

                                //RECARGAMOS DATOS EN CAJAS DE TEXTO
                                CargaInfoVacacion(pTrabajador.Contrato);
                                CargarCamposFromGrid(0);
                            }
                            else
                            {
                                XtraMessageBox.Show("Error al intentar eliminar", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);

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
            }         
        }

        //OBTENER LA FECHA DE PROGRESIVO DEL TRABAJADOR (PARA CALCULOS)
        private DateTime FechaProgresivoTrabajador(string contrato)
        {
            //USAREMOS PARA CONSULTA EL ULTIMO PERIODO REGISTRADO EN SISTEMA
            int periodo = 0;
            periodo = Calculo.PeriodoEvaluar();

            string sql = "SELECT fechaprogresivo FROM trabajador WHERE contrato=@contrato AND anomes=@periodo";
            SqlCommand cmd;
            SqlDataReader rd;
            DateTime fecha = DateTime.Now.Date;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@contrato", contrato));
                        cmd.Parameters.Add(new SqlParameter("@periodo", periodo));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //GUARDAMOS FECHA
                                fecha = (DateTime)rd["fechaprogresivo"];
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

            return fecha;
        }

        //VERIFICAR SI EL TRABAJADOR CUMPLE CON EL REQUISITO 10 AÑOS PARA CALCULO DE PROGRESIVOS
        private bool CalculaProgresivo(string contrato)
        {
            //PARA EFECTO DE CONSULTA USAREMOS COMO PERIODO EL ULTIMO PERIODO REGISTRADO
            int periodo = 0;
            periodo = PeriodoRegistro;
            bool calcula = false;
            string sql = "SELECT anosprogresivo FROM trabajador WHERE contrato = @contrato AND anomes=@periodo";
            SqlCommand cmd;
            SqlDataReader rd;
            decimal anios = 0;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMTROS
                        cmd.Parameters.Add(new SqlParameter("@contrato", contrato));
                        cmd.Parameters.Add(new SqlParameter("@periodo", periodo));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                anios = (decimal)rd["anosprogresivo"];
                                if (anios == 10)
                                {
                                    calcula = true;
                                }
                                else
                                {
                                    //NO CUMPLE LOS REQUISITOS
                                    calcula = false;
                                }
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return calcula;
        }

        //CONSULTAR SI EXISTE REGISTRO EN TABLA VACACIONES PARE EL CONTRATO EN EVALUACION
        private bool ExisteRegistroInfo(string pContrato)
        {
            bool existe = false;
            string sql = "SELECT count(*) FROM vacacion WHERE contrato=@pContrato";
            SqlCommand cmd;
        
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETRO
                        cmd.Parameters.Add(new SqlParameter("@pContrato", pContrato));

                        if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                            existe = true;                       
                        
                    }
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return existe;
        }

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

        //PROPIEDADES DEFECTO
        private void DefaultProperties()
        {
            dtFinaliza.DateTime = DateTime.Now.Date;
            dtSalida.DateTime = DateTime.Now.Date;
            dtRetornoTrabajo.DateTime = DateTime.Now.Date;            
            txtTipo.ItemIndex = 0;
            dtSalida.Focus();
            //txtDiasVac.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            //txtDiasVac.Properties.Mask.EditMask = "#,##0.00;<<#,##0.00>>";
        }

        //LIMPIAR CAMPOS
        private void LimpiarCampos()
        {
            txtDiasVac.Text = "0";
            dtFinaliza.DateTime = DateTime.Now.Date;
            dtSalida.DateTime = DateTime.Now.Date;
            dtRetornoTrabajo.DateTime = DateTime.Now.Date;
            txtTipo.ItemIndex = 0;
            dtSalida.Focus();
            Update = false;        


            //DESHABILITAMOS BOTONES REPORTE
            btnExcel.Enabled = false;
            btnImpresionRapida.Enabled = false;
            btnImprimir.Enabled = false;
            btnPdf.Enabled = false;
            btnEliminar.Enabled = false;
            btnFeriados.Enabled = false;
        }



        //OBTENER LA CANTIDAD DE DIAS OCUPADOS POR LA PERSONA
        private Hashtable diasUsados(string contrato, DateTime? pFechaLimite = null)
        {            
            //TABLA HASH PARA GUARDAR LA CANTIDAD DE DIAS PROGRESIVOS Y LA CANTIDAD DE DIAS PROPORCIONALES USADOS
            string sql = "SELECT dias, tipo FROM VACACIONDETALLE WHERE contrato = @pContrato";

            if (pFechaLimite != null)
            {
                sql = sql + $" AND finaliza <= @pFecha";
            }

            SqlCommand cmd;
            SqlDataReader rd;
            Hashtable tablaDatos = new Hashtable();
            double sumaProp = 0, sumaProg = 0;            
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pContrato", contrato));
                        if (pFechaLimite != null)
                            cmd.Parameters.Add(new SqlParameter("@pFecha", Convert.ToDateTime(pFechaLimite)));

                        rd = cmd.ExecuteReader();

                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                if ((Int16)rd["tipo"] == 1)
                                {
                                   
                                    //SON PROPORCIONALES
                                    sumaProp = sumaProp + Convert.ToDouble((decimal)rd["dias"]);

                                }
                                else if ((Int16)rd["tipo"] == 2)
                                {
                                    //SON PROGRESIVOS
                                    sumaProg = sumaProg + Convert.ToDouble((decimal)rd["dias"]);
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

            //AGREGAMOS SUMAS A HASH
            tablaDatos.Add("proporcional", sumaProp);
            tablaDatos.Add("progresivo", sumaProg);

            return tablaDatos;
        }

        //VALIDAR QUE EL NUEVO REGISTRO NO EXISTA YA PARA CONTRATO
        private bool ExisteRegistro(DateTime salida, DateTime retorno)
        {
            bool existe = false;
            string sql = "SELECT salida, finaliza FROM vacacionDetalle WHERE contrato=@contrato";
            SqlCommand cmd;
            SqlDataReader rd;
            DateTime sal = DateTime.Now.Date;
            DateTime ret = DateTime.Now.Date;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@contrato", contrato));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                sal = (DateTime)rd["salida"];
                                ret = (DateTime)rd["finaliza"];
                            
                                //ITERO EL RANGO QUE HAY ENTRE SAL Y RET
                                while (sal<=ret)
                                {
                                    //COMPARAMOS
                                    if (salida == sal || retorno == sal)
                                        existe = true;                                    


                                    sal = sal.AddDays(1);
                                   
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

            return existe;
        }

        //VALIDACION DE FECHAS SOLO PARA MODIFICACION
        //ES IGUAL AL DE ARRIBA CON LA SALVEDAD DE QUE NO COMPARA CON EL REGISTRO QUE SE INTENTA MODIFICAR
        private bool ExisteRegistroModifica(DateTime salida, DateTime retorno, DateTime salbd, DateTime retdb)
        {
            bool existe = false;
            string sql = "SELECT salida, finaliza FROM vacacionDetalle " +
                " WHERE contrato=@contrato AND salida<>@salbd AND finaliza<>@retdb";
            SqlCommand cmd;
            SqlDataReader rd;
            DateTime sal = DateTime.Now.Date;
            DateTime ret = DateTime.Now.Date;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@contrato", contrato));
                        cmd.Parameters.Add(new SqlParameter("@salbd", salbd));
                        cmd.Parameters.Add(new SqlParameter("@retdb", retdb));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                sal = (DateTime)rd["salida"];
                                ret = (DateTime)rd["finaliza"];

                                //ITERO EL RANGO QUE HAY ENTRE SAL Y RET
                                while (sal <= ret)
                                {
                                    //COMPARAMOS
                                    if (salida == sal || retorno == sal)
                                        existe = true;


                                    sal = sal.AddDays(1);

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

            return existe;
        }

        //COLUMNAS GRILLA
        private void ColumnasGrilla()
        {
            viewVacaciones.Columns[0].Caption = "Salida";
            viewVacaciones.Columns[1].Caption = "Finaliza";
            viewVacaciones.Columns[2].Caption = "Dias";
            viewVacaciones.Columns[3].Caption = "Tipo";
            viewVacaciones.Columns[3].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            viewVacaciones.Columns[3].DisplayFormat.FormatString = "vacacion";
            viewVacaciones.Columns[3].DisplayFormat.Format = new FormatCustom();
            viewVacaciones.Columns[4].Caption = "Retorna";            
        }

        //CARGAR CAMPOS
        private void CargarCamposFromGrid(int? pos = -1)
        {
            if (viewVacaciones.RowCount>0)
            {
                if (pos != -1) viewVacaciones.FocusedRowHandle = (int)pos;

                //DEJAR UPDATE EN TRUE
                Update = true;

                btnEliminar.Enabled = true;
                btnExcel.Enabled = true;
                btnImpresionRapida.Enabled = true;
                btnImprimir.Enabled = true;
                btnPdf.Enabled = true;
                btnFeriados.Enabled = true;

                dtSalida.EditValue = (DateTime)viewVacaciones.GetFocusedDataRow()["salida"];
                dtFinaliza.EditValue = (DateTime)viewVacaciones.GetFocusedDataRow()["finaliza"];
                txtDiasVac.Text = (decimal)viewVacaciones.GetFocusedDataRow()["dias"] + "";
                txtTipo.EditValue = (Int16)viewVacaciones.GetFocusedDataRow()["tipo"];
                dtRetornoTrabajo.EditValue = (DateTime)viewVacaciones.GetFocusedDataRow()["retorna"];

                //DEJARMOS FECHA SELECCIONADA EN CALENDARIO
                calendarioVacacion.EditValue = dtSalida.EditValue;
               
            }
            else
            {
                LimpiarCampos();
            }
        } 

        //PARA PRIMER INGRESO 
        /// <summary>
        /// </summary>
        /// <param name="FechaProgresivo">Corresponde a la fecha en que se recibe el certificado de cotizaciones.</param>
        private void PrimerIngreso(DateTime FechaProgresivo, bool? Actualiza = false, bool? Reporte = false, DateTime? pFechaReporte = null, bool? pSoloReporte = false)
        {
            bool AplicaAnual = false;
            double progresivos = 0, propAnuales = 0, propRestantes = 0, propUsados = 0, progUsados = 0, propRestantesAnuales = 0;
            double totalProporcionales = 0, totalProgresivos = 0, totalDias = 0;
            bool ConsideraProgresivo = false;
            Hashtable datos = new Hashtable();
            //Solo para reporte
            Hashtable datosRepo = new Hashtable();
            DateTime FechaLimite = DateTime.Now.Date;

            DataReporte.Clear();

            double CotFaltante = 0, CotReq = 120, diasPropTotalesUsados = 0, diasProgTotalesUsados = 0;
            DateTime FechaBase = DateTime.Now.Date;
            //ULTIMO AÑO EN QUE SE ALCANZA A CUMPLIR UN AÑO (CONSIDERADO DESDE LA FECHA DE INICIO DE VACACIONES)
            DateTime UltimoAnioVacaciones = DateTime.Now.Date;

            //Si es para reporte la fecha limite será hasta la fecha del registro (ultimo dia registro)

            if (Trabajador != null)
            {
                //CALCULAMOS LA CANTIDAD DE DIAS DE VACACIONES HASTA EL DIA DE HOY
                //SI LA FECHA DE TERMINO DE CONTRATO ES MENOR AL DÍA DE HOY CALCULAMOS HASTA LA FECHA DE TERMINO DE CONTRATO
                if (Trabajador.Salida < DateTime.Now.Date)
                    FechaLimite = Trabajador.Salida;
                else if (Trabajador.Salida > DateTime.Now.Date)
                    FechaLimite = DateTime.Now.Date;
                else
                    FechaLimite = DateTime.Now.Date;

                // ------------------------------
                //  FERIADOS PROGRESIVOS        |
                //-------------------------------

                //@ DEBEMOS VERIFICAR LA CANTIDAD DE COTIZACIONES (MESES) QUE TIENE REGISTRADA LA FICHA
                //@ DEBEMOS CONSIDERAR LA FECHA EN QUE SE ENTREGÓ EL CERTIFICADO DE COTIZACIONES.

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
                progresivos = vacaciones.Progresivos(FechaBase, FechaProgresivo, ConsideraProgresivo);

                // ---------------------------------------------------------------------------------
                // FERIADOS LEGALES (15 X AÑO)                                                     |
                // ---------------------------------------------------------------------------------
                //@ PASADO UN AÑO DESDE EL INICIO DE CONTRATO, CORRESPONDE 15 DIAS DE VACACIONES.  |
                //@ POR MES = > 15/12 = 1.25                                                       |
                //@ POR DIA => 1.25/30 = 0.041667                                                  |
                //----------------------------------------------------------------------------------

                //ULTIMO AÑO DISPONIBLE...
                UltimoAnioVacaciones = vacaciones.UltimoAnioProporcional(Trabajador.FechaVacacion);

                AplicaAnual = vacaciones.AplicaProporcionalAnual(Trabajador.FechaVacacion);                                  

                //CONSIDERAMOS LA FECHA DE VACACION GUARDADA EN LA FICHA
                if (AplicaAnual)
                    propAnuales = Math.Round(vacaciones.FeriadosProp(Trabajador.FechaVacacion, UltimoAnioVacaciones), 2);
                else
                    propAnuales = 0;

                //VACACIONES RESTANTES (FECHA DESDE EL ULTIMO AÑO ENCONTRATO HASTA EL ULTIMO DIA DE CONTRATO O TODAY)

                if (AplicaAnual)
                {
                    if (UltimoAnioVacaciones < FechaLimite)
                        propRestantes = Math.Round(vacaciones.FeriadosProp(UltimoAnioVacaciones, FechaLimite), 2);
                    else
                        propRestantes = 0;
                }
                else
                {
                    propRestantes = vacaciones.FeriadosProp(Trabajador.FechaVacacion, FechaLimite);
                }

                //OBTENEMOS LA CANTIDAD DE DIAS PROPORCIONALES USADOS AL IGUAL QUE LA CANTIDAD DE DIAS PROGRESIVOS               
                datos = diasUsados(Trabajador.Contrato);

                //Guardamos el total de dias proprcionales usados y Progresivos
                diasPropTotalesUsados = Convert.ToDouble(datos["proporcional"]);
                diasProgTotalesUsados = Convert.ToDouble(datos["progresivo"]);                  

                //PROPORCIONALES ANUALES RESTANTES
                if (diasPropTotalesUsados > propAnuales)
                    propRestantesAnuales = 0;
                else
                    propRestantesAnuales = propAnuales - diasPropTotalesUsados;                            
                
                //CANTIDAD DE DIAS PROPORCIONALES DISPONIBLES (RESTAMOS LOS USADOS)
                totalProporcionales = Math.Round((propAnuales + propRestantes) - diasPropTotalesUsados, 2);

                if (diasProgTotalesUsados == progresivos)
                    totalProgresivos = 0;
                else if (diasProgTotalesUsados > progresivos)
                    totalProgresivos = 0;
                else
                    totalProgresivos = Math.Round(progresivos - diasProgTotalesUsados, 2);

                //Obtenemos los dias usados hasta la fecha del registro comprobante (Solo para reporte)
                if ((bool)Reporte && pFechaReporte != null)
                {
                    datosRepo = diasUsados(Trabajador.Contrato, Convert.ToDateTime(pFechaReporte));
                    propUsados = Convert.ToDouble(datosRepo["proporcional"]);
                    progUsados = Convert.ToDouble(datosRepo["progresivo"]);

                    DataReporte.Add("propUsados", propUsados);
                    DataReporte.Add("progUsados", progUsados);
                    DataReporte.Add("propRestante", totalProporcionales);
                    DataReporte.Add("progRestante", totalProgresivos);
                    DataReporte.Add("propUsadosTotales", diasPropTotalesUsados);
                    DataReporte.Add("progUsadosTotales", diasProgTotalesUsados);
                }             

                //totalProgresivos = Math.Round(progresivos - progUsados, 2);

                //TOTAL ENTRE DIAS PROGRESIVOS Y DIAS LEGALES
                totalDias = Math.Round(totalProporcionales + totalProgresivos, 2);

                //INSERT
                //SI SOLO REPORTE ES TRUE NO QUEREMOS GUARDAR DATOS EN BD, SOLO MOSTRAR COMPROBANTE
                if (pSoloReporte == false)
                {
                    if (Actualiza == false)
                    {
                        NuevaInformacionVacacion(Convert.ToDecimal(propAnuales), Convert.ToDecimal(propRestantes),
                                 Convert.ToDecimal(diasPropTotalesUsados), Convert.ToDecimal(totalProporcionales), Convert.ToDecimal(progresivos),
                                 Convert.ToDecimal(diasProgTotalesUsados), Convert.ToDecimal(totalProgresivos), Convert.ToDecimal(totalDias),
                                 Convert.ToDecimal(propRestantesAnuales));
                    }
                    else
                    {
                        //UPDATE
                        ModificarInformacionVacacion(Convert.ToDecimal(propAnuales), Convert.ToDecimal(propRestantes),
                                  Convert.ToDecimal(diasPropTotalesUsados), Convert.ToDecimal(totalProporcionales), Convert.ToDecimal(progresivos),
                                  Convert.ToDecimal(diasProgTotalesUsados), Convert.ToDecimal(totalProgresivos), Convert.ToDecimal(totalDias),
                                  Convert.ToDecimal(propRestantesAnuales));
                    } 
                }
              
            }
        }
        /// <summary>
        /// Carga informacion de vacaciones en cajas correspondientes.
        /// </summary>
        /// <param name="contrato">Numero de contrato del trabajador.</param>
        private void CargaInfoVacacion(string contrato)
        {
            string sql = "SELECT diaspropanual, diasproprestantes, diasproptomados, totalprop, diasprog, " +
                "diasprogtomados, totalprog, totaldias, diaspropanrestantes FROM vacacion WHERE contrato=@contrato";
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
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //CARGAMOS DATOS EN CAJAS DE TEXTO
                                txtpropAnual.Text = (decimal)rd["diaspropanual"] + "";
                                txtpropRestante.Text = (decimal)rd["diasproprestantes"] + "";
                                txtpropUsados.Text = (decimal)rd["diasproptomados"] + "";
                                txtpropTotal.Text = (decimal)rd["totalprop"] + "";
                                txtprogAcumulados.Text = (decimal)rd["diasprog"] + "";
                                txtUsadosProg.Text = (decimal)rd["diasprogtomados"] + "";
                                txtTotalProg.Text = (decimal)rd["totalprog"] + "";                       
                                lblTotalDias.Text = (decimal)rd["totaldias"] + "";
                                txtAnualesRestantes.Text = (decimal)rd["diaspropanrestantes"] + "";
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

        //OBTENER LA CANTIDAD DE DIAS PROPORCIONALES Y PROGRESIVOS DISPONIBLES
        private Hashtable datosVacacion(string contrato)
        {
            Hashtable datos = new Hashtable();
            string sql = "SELECT totalprop, totalprog FROM vacacion WHERE contrato=@contrato";
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

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                datos.Add("proporcional", Convert.ToDouble((decimal)rd["totalprop"]));
                                datos.Add("progresivo", Convert.ToDouble((decimal)rd["totalprog"]));
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

        //VERIFICAR QUE LA FECHA DE SALIDA NO SEA FIN DE SEMANA (SABADO-DOMINGO)
        private bool EsFinSemana(DateTime fecha)
        {
            bool esfin = false;
            if (fecha.DayOfWeek == DayOfWeek.Saturday || fecha.DayOfWeek == DayOfWeek.Sunday)
            {
                //SI ES SABADO O DOMINGO
                esfin = true;
            }
            else
            {
                esfin = false;
            }

            return esfin;
        }        

        //CONSULTA INFORMACION VACACION
        private Hashtable InformacionVacacion(string contrato)
        {
            string sql = "SELECT diaspropanual, diasproprestantes, diasproptomados, totalprop, diasprog, " +
                "diasprogtomados, totalprog, totaldias, diaspropanrestantes FROM vacacion WHERE contrato=@contrato";
            SqlCommand cmd;
            SqlDataReader rd;
            Hashtable datos = new Hashtable();
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@contrato", contrato));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //LLENAMOS HASHTABLA
                                datos.Add("diaspropanual", (decimal)rd["diaspropanual"]);
                                datos.Add("diasproprestantes", (decimal)rd["diasproprestantes"]);
                                datos.Add("diasproptomados", (decimal)rd["diasproptomados"]);
                                datos.Add("totalprop", (decimal)rd["totalprop"]);
                                datos.Add("diasprog", (decimal)rd["diasprog"]);
                                datos.Add("diasprogtomados", (decimal)rd["diasprogtomados"]);
                                datos.Add("totalprog", (decimal)rd["totalprog"]);
                                datos.Add("totaldias", (decimal)rd["totaldias"]);
                                datos.Add("diaspropanrestantes", (decimal)rd["diaspropanrestantes"]);
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

        //COMPARA VALORES PARA LOG
        #region "LOG REGISTRO"
        private Hashtable precargaVacaciones(string contrato, DateTime salida, DateTime regreso)
        {
            Hashtable datos = new Hashtable();
            string sql = "SELECT salida, finaliza, dias, tipo FROM vacaciondetalle " +
                "WHERE contrato=@contrato AND salida=@salida AND finaliza=@finaliza";
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
                        cmd.Parameters.Add(new SqlParameter("@salida", salida));
                        cmd.Parameters.Add(new SqlParameter("@finaliza", regreso));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //LLENAMOS HASH
                                datos.Add("salida", (DateTime)rd["salida"]);
                                datos.Add("finaliza", (DateTime)rd["finaliza"]);
                                datos.Add("dias", (decimal)rd["dias"]);
                                datos.Add("tipo", (Int16)rd["tipo"]);
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

        //COMPARA CON TABLA HASH
        private void ComparaVacacion(string contrato, DateTime salida, DateTime regreso, decimal dias, Int16 tipo, Hashtable datos)
        {
            if (datos.Count > 0)
            {
                //SI SON DISTINTOS GUARDAMOS EN LOG
                if ((DateTime)datos["salida"] != salida)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE CAMBIA FECHA DE SALIDA PARA VACACION CONTRATO " + contrato + " [" + (DateTime)datos["salida"] + ";" + (DateTime)datos["finaliza"] + "]", "VACACIONDETALLE", salida.ToShortDateString(), (DateTime)datos["salida"] + "", "MODIFICAR");
                    log.Log();
                }
                if ((DateTime)datos["finaliza"] != regreso)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE CAMBIA FECHA FINALIZA PARA VACACION CONTRATO " + contrato + " [" + (DateTime)datos["salida"] + ";" + (DateTime)datos["finaliza"] + "]", "VACACIONDETALLE", regreso.ToShortDateString(), (DateTime)datos["finaliza"] + "", "MODIFICAR");
                    log.Log();
                }
                if ((decimal)datos["dias"] != dias)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE CAMBIA CANTIDAD DE DIAS DE VACACIONES CONTRATO " + contrato + " [" + (DateTime)datos["salida"] + ";" + (DateTime)datos["finaliza"] + "]", "VACACIONDETALLE", dias + "", (decimal)datos["dias"] + "", "MODIFICAR");
                    log.Log();
                }
                if ((Int16)datos["tipo"] != tipo)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE CAMBIA TIPO DE VACACION CONTRATO " + contrato + " [" + (DateTime)datos["salida"] + ";" + (DateTime)datos["finaliza"] + "]", "VACACIONDETALLE", tipo + "", (Int16)datos["tipo"] + "", "MODIFICAR");
                    log.Log();
                }
            }
        }
        #endregion

        //METODO PARA MOSTRAR INFORME
        private void InformeVacaciones(string contrato, bool? imprime = false, bool? GeneraPdf = false, bool editar = false)
        {
            string sql = "select vacacionDetalle.contrato, concat(nombre, ' ', apepaterno, ' ', apematerno) as nombre, " +
                        " vacacionDetalle.salida, vacacionDetalle.finaliza, folio, trabajador.rut, dias, tipo, vacacionDetalle.retorna from vacaciondetalle" +
                        " INNER JOIN trabajador ON trabajador.contrato = vacacionDetalle.contrato " +
                        " WHERE vacacionDetalle.contrato = @contrato AND anomes=@pPeriodo;";

            SqlCommand cmd;
            SqlDataAdapter data = new SqlDataAdapter();
            DataSet ds = new DataSet();
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@contrato", contrato));
                        cmd.Parameters.Add(new SqlParameter("@pPeriodo", Calculo.PeriodoObservado));

                        data.SelectCommand = cmd;
                        data.Fill(ds, "vacaciondetalle");
                        cmd.Dispose();
                        data.Dispose();
                        fnSistema.sqlConn.Close();

                        if (ds.Tables[0].Rows.Count > 0 || editar)
                        {
                            //CREAMOS REPORTE
                            //rptVacacion vac = new rptVacacion();
                            ReportesExternos.rptVacacion vac = new ReportesExternos.rptVacacion();
                            vac.LoadLayoutFromXml(Path.Combine(fnSistema.RutaCarpetaReportesExterno, "rptVacacion.repx"));

                            vac.DataSource = ds.Tables[0];
                            vac.DataMember = "vacaciondetalle";

                            Empresa emp = new Empresa();
                            emp.SetInfo();

                                                      //PARAMETROS
                            Hashtable datosVacacion = new Hashtable();
                            datosVacacion = InformacionVacacion(contrato);                            

                            vac.Parameters["propanual"].Value = (decimal)datosVacacion["diaspropanual"];
                            vac.Parameters["proprestante"].Value = (decimal)datosVacacion["diasproprestantes"];
                            vac.Parameters["propusados"].Value = (decimal)datosVacacion["diasproptomados"];
                            vac.Parameters["totalprop"].Value = (decimal)datosVacacion["totalprop"];
                            vac.Parameters["progresivo"].Value = (decimal)datosVacacion["totalprog"];
                            vac.Parameters["totalProg"].Value = (decimal)datosVacacion["diasprog"];
                            vac.Parameters["progusado"].Value = (decimal)datosVacacion["diasprogtomados"];
                            vac.Parameters["progdisponible"].Value = (decimal)datosVacacion["totalprog"];
                            vac.Parameters["totaldias"].Value = (decimal)datosVacacion["totaldias"];
                            vac.Parameters["empresa"].Value = emp.Razon;
                            vac.Parameters["rutEmpresa"].Value = fnSistema.fFormatearRut2(emp.Rut);
                            vac.Parameters["proanrestante"].Value = (decimal)datosVacacion["diaspropanrestantes"];

                            //OCULTAR PARAMETROS

                            foreach (DevExpress.XtraReports.Parameters.Parameter parametro in vac.Parameters)
                            {
                                parametro.Visible = false;
                            }


                            Documento doc = new Documento("", 0);
                            //vac.SaveLayoutToXml(Path.Combine(fnSistema.RutaCarpetaReportesExterno, "rptVacacion.repx"));

                            if (editar)
                            {
                                splashScreenManager1.ShowWaitForm();
                                //Se le pasa el waitform para que se cierre una vez cargado
                                DiseñadorReportes.MostrarEditorLimitado(vac, "rptVacacion.repx", splashScreenManager1);
                            }
                            else
                            {
                                if ((bool)imprime)
                                    doc.PrintDocument(vac);
                                else if ((bool)GeneraPdf)
                                    doc.ExportToPdf(vac, $"Vacaciones_{contrato}");
                                else
                                    doc.ShowDocument(vac);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
           
        }

        //PREGUNTAR SI HAY REGISTROS DE VACACIONES
        private bool HayVacaciones(string contrato)
        {
            bool existe = false;
            string sql = "SELECT salida FROM vacaciondetalle WHERE contrato = @contrato";
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

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //SI HAY FILAS ES PORQUE HAY REGISTROS!
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

        //GENERA DOCUMENTO COMPROBANTE DE VACACIONES
        private void GeneraComprobante(DateTime salida, DateTime finaliza, string contrato, Hashtable data, bool? Imprime = false)
        {
            string sql = "select DISTINCT rut, concat(nombre, ' ', apepaterno, ' ', apematerno) as name, " +
                "vacacionDetalle.contrato, vacaciondetalle.salida, finaliza, dias, tipo, retorna, folio " +
                "from vacacionDetalle " +
                "INNER JOIN trabajador ON trabajador.contrato = vacacionDetalle.contrato " +
                "where vacacionDetalle.contrato = @contrato AND vacaciondetalle.salida = @salida " +
                "AND vacaciondetalle.finaliza = @finaliza";

            SqlCommand cmd;
            SqlDataAdapter ad = new SqlDataAdapter();
            DataSet ds = new DataSet();
            
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@contrato", contrato));
                        cmd.Parameters.Add(new SqlParameter("@salida", salida));
                        cmd.Parameters.Add(new SqlParameter("@finaliza", finaliza));

                        ad.SelectCommand = cmd;
                        ad.Fill(ds, "data");

                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            //RptComprobanteVacacion vaca = new RptComprobanteVacacion();
                            //Reporte externo
                            ReportesExternos.rptComprobanteVacacion vaca = new ReportesExternos.rptComprobanteVacacion();
                            vaca.LoadLayoutFromXml(Path.Combine(fnSistema.RutaCarpetaReportesExterno, "rptComprobanteVacacion.repx"));

                            vaca.Parameters["imagen"].Value = Imagen.GetLogoFromBd();

                            vaca.DataSource = ds.Tables[0];
                            vaca.DataMember = "data";

                            Empresa emp = new Empresa();
                            emp.SetInfo();

                            foreach (DevExpress.XtraReports.Parameters.Parameter parametro in vaca.Parameters)
                            {
                                parametro.Visible = false;
                            }

                            vaca.Parameters["empresa"].Value = emp.Razon;
                            vaca.Parameters["rutEmpresa"].Value = fnSistema.fFormatearRut2(emp.Rut);
                            vaca.Parameters["propusados"].Value = (double)data["propUsadosTotales"];
                            vaca.Parameters["progusados"].Value = (double)data["progUsadosTotales"];
                            vaca.Parameters["proprestante"].Value = (double)data["propRestante"];
                            vaca.Parameters["progrestante"].Value = Convert.ToDecimal(data["progRestante"]);
                            vaca.Parameters["propUsadosRepo"].Value = Convert.ToDouble(data["propUsados"]);
                            vaca.Parameters["progUsadosRepo"].Value = Convert.ToDouble(data["progUsados"]);

                            //DÍAS TOTALES DE VACACIONES ACUMULADAS ANUALMENTE
                            double diasVacacionesAcumuladas = (double)data["propRestante"] + (double)data["propUsadosTotales"];
                            vaca.Parameters["diasVacacionesAcumuladas"].Value = diasVacacionesAcumuladas;

                            //DÍAS TOTALES DE VACACIONES ACUMULADAS ANUALMENTE
                            double diasProgAcumulados = (double)data["progRestante"] + (double)data["progUsadosTotales"];
                            vaca.Parameters["diasProgAcumulados"].Value = diasProgAcumulados;

                            //SUB TOTAL DE DIAS SUMADOS PROGRESIVOS Y PROPRESTANTES
                           // double subTotalDiasAcumulados = diasVacacionesAcumuladas + (double)data["propRestante"] + (double)data["propRestante"];
                           //vaca.Parameters["subTotalDiasAcumulados"].Value = subTotalDiasAcumulados;

                            //TOTAL DÍAS PENDIENTES (RESTA DE LOS USADOS)
                           // double totalDiasPendientes = subTotalDiasAcumulados - ((double)data["propUsadosTotales"] + (double)data["progUsadosTotales"]);
                           // vaca.Parameters["totalDiasPendientes"].Value = totalDiasPendientes;

                            Documento d = new Documento("", 0);
                            //vaca.SaveLayoutToXml(Path.Combine(fnSistema.RutaCarpetaReportesExterno, "rptComprobanteVacacion.repx"));
                            if (Imprime == false)
                                d.ShowDocument(vaca);
                            else
                                d.PrintDocument(vaca);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

        }

        //DATOS PARA REPORTE EN EXCEL
        private DataTable TablaExcel(string contrato, int periodo)
        {
            string sql = "select vacacionDetalle.contrato, concat(nombre, ' ', apepaterno, ' ', apematerno) as nombre, " +
                    " vacacionDetalle.salida, vacacionDetalle.finaliza, dias, tipo, vacacionDetalle.retorna from vacaciondetalle" +
                    " INNER JOIN trabajador ON trabajador.contrato = vacacionDetalle.contrato " +
                    " WHERE vacacionDetalle.contrato = @contrato AND trabajador.anomes = @periodo";

            SqlCommand cmd;
            SqlDataAdapter ad = new SqlDataAdapter();
            DataSet ds = new DataSet();

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@contrato", contrato));
                        cmd.Parameters.Add(new SqlParameter("@periodo", periodo));

                        ad.SelectCommand = cmd;
                        ad.Fill(ds, "data");

                        cmd.Dispose();
                        ad.Dispose();
                        fnSistema.sqlConn.Close();
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            return ds.Tables[0];
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
                return null;
            }
        }

        //MANIPULAR DATATABLE
        private DataTable TablaFinalExcel(DataTable tabla)
        {
            DataTable tablaFinal = new DataTable();
            if (tabla.Rows.Count > 0)
            {
                tablaFinal.Columns.Add("contrato", typeof(string));
                tablaFinal.Columns.Add("nombre", typeof(string));
                tablaFinal.Columns.Add("salida", typeof(DateTime));
                tablaFinal.Columns.Add("finaliza", typeof(DateTime));
                tablaFinal.Columns.Add("tipo", typeof(string));
                tablaFinal.Columns.Add("retorna", typeof(DateTime));
                tablaFinal.Columns.Add("dias", typeof(decimal));

                string contrato = "", nombre = "", tipo = "";
                DateTime salida = DateTime.Now.Date;
                DateTime finaliza = DateTime.Now.Date;
                DateTime retorna = DateTime.Now.Date;
                decimal dias = 0;

                try
                {
                    for (int fila = 0; fila < tabla.Rows.Count; fila++)
                    {
                        for (int columna = 0; columna < tabla.Columns.Count; columna++)
                        {
                            if (tabla.Columns[columna].ToString() == "contrato")
                                contrato = (string)tabla.Rows[fila][columna];
                            if (tabla.Columns[columna].ToString() == "nombre")
                                nombre = (string)tabla.Rows[fila][columna];
                            if (tabla.Columns[columna].ToString() == "salida")
                                salida = Convert.ToDateTime(tabla.Rows[fila][columna]);
                            if (tabla.Columns[columna].ToString() == "finaliza")
                                finaliza = Convert.ToDateTime(tabla.Rows[fila][columna]);
                            if (tabla.Columns[columna].ToString() == "dias")
                                dias = (decimal)tabla.Rows[fila][columna];
                            if (tabla.Columns[columna].ToString() == "tipo")
                                tipo = GetTipoVacacion(Convert.ToInt32(tabla.Rows[fila][columna]));
                            if (tabla.Columns[columna].ToString() == "retorna")
                                retorna = Convert.ToDateTime(tabla.Rows[fila][columna]);

                        }

                        //INGRESAR FILA
                        tablaFinal.Rows.Add(contrato, nombre, salida, finaliza, tipo, retorna, dias);
                    }

                    //RETORNAMOS TABLA
                    return tablaFinal;
                }
                catch (Exception)
                {
                    XtraMessageBox.Show("Ha ocurrido un error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }

            }
            else
            {
                return null;
            }
        }

        //OBTENER TIPO DE VACACION
        private string GetTipoVacacion(int Id)
        {
            string data = "";
            if (Id == 1)
            {
                data = "PROPORCIONAL";
            }
            else if (Id == 2)
            {
                data = "PROGRESIVO";
            }

            return data;
        }

        /*VERIFICAR SI HAY CAMBIOS SIN GUARDAR*/
        private bool CambiosSinGuardar(string pContrato, DateTime pSalida, DateTime pFinaliza)
        {
            string sql = "SELECT salida, finaliza, dias, tipo, retorna FROM vacaciondetalle WHERE " +
                "contrato=@pContrato AND salida=@pSalida AND finaliza=@pFinaliza";
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
                        cmd.Parameters.Add(new SqlParameter("@pSalida", pSalida));
                        cmd.Parameters.Add(new SqlParameter("@pFinaliza", pFinaliza));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //COMPARAR
                                if (dtSalida.DateTime != Convert.ToDateTime(rd["salida"])) return true;
                                if (dtFinaliza.DateTime != Convert.ToDateTime(rd["finaliza"])) return true;
                                if (Convert.ToDouble(txtDiasVac.Text) != Convert.ToDouble(rd["dias"])) return true;
                                if (Convert.ToInt32(txtTipo.EditValue) != Convert.ToInt32(rd["tipo"])) return true;
                                if (dtRetornoTrabajo.DateTime != Convert.ToDateTime(rd["retorna"])) return true;
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
            return false;
        }

        //OBTENER TODAS LAS FECHAS DE VACACIONES PARA CALENDARIO
        private List<DateTime> ListadoVacaciones(string pContrato)
        {
            List<DateTime> Fechas = new List<DateTime>();
            string sql = "SELECT salida, finaliza FROM vacacionDetalle WHERE contrato=@pContrato";
            SqlCommand cmd;
            SqlDataReader rd;
            DateTime Salida = DateTime.Now.Date;
            DateTime Finaliza = DateTime.Now.Date;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        cmd.Parameters.Add(new SqlParameter("@pContrato", pContrato));
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                Salida = Convert.ToDateTime(rd["salida"]);
                                Finaliza = Convert.ToDateTime(rd["finaliza"]);

                                //OBTENEMOS TODAS LAS FECHAS DENTRO DEL RANGO (INCLUYENDO EXTREMOS)
                                while (Salida<=Finaliza)
                                {
                                    Fechas.Add(Salida);

                                    Salida = Salida.AddDays(1);
                                }
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

            return Fechas;
        }

        //OBTENER TODAS LAS FECHAS DE RETORNO PARA CONTRATO (PARA CALENDARIO)
        private List<DateTime> ListadoVacacionesRetorno(string pContrato)
        {
            List<DateTime> Fechas = new List<DateTime>();
            string sql = "SELECT retorna FROM vacacionDetalle WHERE contrato=@pContrato";
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        cmd.Parameters.Add(new SqlParameter("@pContrato", pContrato));
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                Fechas.Add(Convert.ToDateTime(rd["retorna"]));
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

            return Fechas;
        }

        //OBTENER TODAS LAS FECHAS FERIADOS
        private List<DateTime> ListadoFeriados()
        {
            List<DateTime> Listado = new List<DateTime>();
            string sql = "SELECT fecha FROM feriado";
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
                                //GUARDAMOS FECHAS
                                Listado.Add(Convert.ToDateTime(rd["fecha"]));
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

            return Listado;
        }

        //OBTENER DESCRIPCION FERIADO SOLO PARA TOOLTIP (PARA CALENDARIO)
        private string getDescripcionFeriado(DateTime pFecha)
        {
            string sql = "SELECT descripcion FROM feriado WHERE fecha=@pFecha";
            string desc = "";
            SqlCommand cmd;

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pFecha", pFecha));
                        desc = (string)cmd.ExecuteScalar();

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();

                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return desc;
        }

        //OBTENER EL ULTIMO NUMERO DE FOLIO
        private int UltimoFolio(string pContrato)
        {
            string sql = "SELECT ISNULL(MAX(folio), 0) FROM vacaciondetalle WHERE contrato=@pContrato";
            SqlConnection cn;
            SqlCommand cmd;
            int number = 0;
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
                            cmd.Parameters.Add(new SqlParameter("@pContrato", pContrato));

                            number = Convert.ToInt32(cmd.ExecuteScalar());

                            //AUMENTOS UN DIGITO AL NUMERO DE FOLIO
                            if (number > 0)
                                number++;
                            else
                                number = 1;
                            
                        }
                        cmd.Dispose();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return number;
        }

        #endregion

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();
          
            if (op.Cancela == false)
            {
                op.Cancela = true;
                op.SetButtonProperties(btnNuevo, 2);
                LimpiarCampos();
            }
            else
            {
                op.Cancela = false;
                op.SetButtonProperties(btnNuevo, 1);
                CargarCamposFromGrid();              
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            //NUEVA SESION
            Sesion.NuevaActividad();

            if (Trabajador.Contrato == "" || contrato == "")
            { XtraMessageBox.Show("Ficha de trabajador no válida", "Error ficha", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            //VER SI EL USUARIO ESTA BLOQUEADO 
            if (User.Bloqueado())
            { XtraMessageBox.Show("No puedes realizar modificaciones", "Información", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (Trabajador.PeriodoPersona < Calculo.PeriodoObservado)
            { XtraMessageBox.Show("No puedes editar esta ficha", "Histórico", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (dtSalida.EditValue == null) { XtraMessageBox.Show("Por favor selecciona una fecha de salida", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); dtSalida.Focus(); return; }
            if (dtFinaliza.EditValue == null) { XtraMessageBox.Show("Por favor selecciona una fecha de finalizacion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); dtFinaliza.Focus(); return; }
            if (txtDiasVac.Text == "") { XtraMessageBox.Show("Por favor ingrese la cantidad de dias", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtDiasVac.Focus(); return; }
            if (txtTipo.EditValue == null) { XtraMessageBox.Show("Por favor selecciona un tipo de vacacion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtTipo.Focus(); return; }
            if (dtRetornoTrabajo.EditValue == null) { XtraMessageBox.Show("POor favor selecciona una fecha de retorno", "Informacion",  MessageBoxButtons.OK, MessageBoxIcon.Warning); dtRetornoTrabajo.Focus();return;}

            bool existe = false;
            bool valido = false, cruceFechas = false;
            Int16 type = 0;
            Hashtable datos = new Hashtable();
            DateTime salidabd = DateTime.Now.Date;
            DateTime regresobd = DateTime.Now.Date;
            double dias = 0, diasdb = 0;
          
            if (contrato != "")
            {
                //UPDATE REGISTRO
                if (Update)
                {
                    //VERIFICAR QUE EL NUMERO INGRESADO TENGA EL FORMATO CORRECTO
                    valido = fnDecimal(txtDiasVac.Text);
                    if (valido == false) { XtraMessageBox.Show("Numero no valido para dias", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);txtDiasVac.Focus();return;}

                    if (txtDiasVac.Text == "0")
                    {
                        DialogResult pregunta = XtraMessageBox.Show("¿Seguro quieres ingresar 0 dias?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (pregunta == DialogResult.No)
                            return;
                    }

                    //VALIDAR QUE LA FECHA DE INICIO NO SEA MENOR A LA FECHA DE VACACIONES
                    if (Convert.ToDateTime(dtSalida.EditValue) < Trabajador.FechaVacacion)
                    { XtraMessageBox.Show("La fecha de salida no puede ser menor a la fecha de inicio de vacaciones de la ficha", "Vacaciones", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                    //VALIDAR QUE LA FECHA DE SALIDA NO SEA MAYOR A LA FECHA DE TERMINO
                    if ((DateTime)(dtSalida.EditValue) > (DateTime)dtFinaliza.EditValue)
                    { XtraMessageBox.Show("La fecha de salida no puede ser mayor a la fecha de retorno", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning); dtSalida.Focus(); return; }

                    //VALIDAR QUE LA FECHA DE RETORNO NO SEA MENOR A LA FECHA DE INICIO DE VACACIONES
                    if (Convert.ToDateTime(dtRetornoTrabajo.EditValue) < Convert.ToDateTime(dtSalida.EditValue))
                    { XtraMessageBox.Show("La fecha de retorno de vacaciones no puede ser menor a la fecha de inicio de vacaciones", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                    //VALIDAR QUE LA FECHA DE RETORNO NO SEA INFERIOR A LA FECHA DE TERMINO
                    if (Convert.ToDateTime(dtRetornoTrabajo.EditValue) < Convert.ToDateTime(dtFinaliza.EditValue))
                    { XtraMessageBox.Show("La fecha de retorno al trabajo no puede ser inferior a la fecha de termino de vacaciones", "Fecha Retorno", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                    //VALIDAR QUE LA FECHA QUE FINALIZA NO SEA MENOR A LA FECHA DE SALIDA (INICIO DE VAC.)
                    if (Convert.ToDateTime(dtFinaliza.EditValue) < Convert.ToDateTime(dtSalida.EditValue))
                    { XtraMessageBox.Show("La fecha de termino de vacaciones no puede ser inferior a la fecha de incio", "Fecha incorrecta", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                    //COMPARA FECHAS (SI SIGUEN SIENDO IGUALES SOLO GUARDAMOS, SI SON DISTINTAS VERIFICAMOS)
                    if (viewVacaciones.RowCount>0)
                    {
                        salidabd = (DateTime)viewVacaciones.GetFocusedDataRow()["salida"];
                        regresobd = (DateTime)viewVacaciones.GetFocusedDataRow()["finaliza"];
                        //CORRESPONDE A LOS DIAS GUARDADOS EN BASE DE DATOS
                        diasdb = Convert.ToDouble((decimal)viewVacaciones.GetFocusedDataRow()["dias"]);
                        //PROGRESIVO O NORMAL?
                        type = (Int16)viewVacaciones.GetFocusedDataRow()["tipo"];

                        //VERIFICAMOS LOS DIAS DE INGRESO SI ES QUE SON DISTINTOS
                        if (Convert.ToDouble(txtDiasVac.Text) != diasdb)
                        {
                            //VALIDAR QUE LA CANTIDAD DE DIAS NO SUPERE LOS DISPONIBLES
                            datos = datosVacacion(contrato);

                            //DEBEMOS SUMAR LOS DIAS DEL REGISTRO QUE ESTAMOS PREGUNTANDO PARA OBETENER EL VALOR REAL DE DIAS DISPONIBLES
                            //DIASDB + DIAS REGISTRO = DIAS REALES DISPONIBLES
                            if (type == 1)
                                datos["proporcional"] = Convert.ToDouble(datos["proporcional"]) + diasdb;
                            if (type == 2)
                                datos["progresivo"] = Convert.ToDouble(datos["progresivo"]) + diasdb;

                            //DIAS INGRESADOS
                            dias = Convert.ToDouble(txtDiasVac.Text);

                            //VERIFICAMOS QUE LOS DIAS INGRESADOS NO SUPEREN LOS DIAS DISPONIBLES
                            //if (Convert.ToInt32(txtTipo.EditValue) == 1 && (dias > Convert.ToDouble(datos["proporcional"])))
                            //{ XtraMessageBox.Show("Por favor verifica que los dias ingresados no superen los dias proporcionales disponibles", "Dias proporcionales", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
                            //LO MISMO DE ARRIBA PERO PARA DIAS PROGRESIVOS
                            if (Convert.ToInt32(txtTipo.EditValue) == 2 && (dias > Convert.ToDouble(datos["progresivo"])))
                            { XtraMessageBox.Show("Por favor verifica que los dias ingresados no superen los dias progresivos disponibles", "Dias progresivos", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
                        }

                        if (salidabd != (DateTime)dtSalida.EditValue && regresobd != (DateTime)dtFinaliza.EditValue)
                        {                           
                            //VERIFICAR SI LA FECHA DE SALIDA ES FIN DE SEMANA
                           // if (EsFinSemana((DateTime)dtSalida.EditValue))
                           // { XtraMessageBox.Show("La fecha de salida debe ser un dia habil", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);dtSalida.Focus();return;}

                            //SI SON DISTINTOS DEBEMOS VERIFICAR QUE LAS FECHAS NO SE CRUCEN CON LOS OTRO REGISTROS
                            cruceFechas = ExisteRegistroModifica((DateTime)dtSalida.EditValue, (DateTime)dtFinaliza.EditValue, salidabd, regresobd);
                            if (cruceFechas)
                            { XtraMessageBox.Show("La fecha que intentas registrar ya esta en uso", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }                         

                            //VALIDAR QUE LA CANTIDAD DE DIAS NO SUPERE LA CANTIDAD DISPONIBLE
                            //if (SuperaLimite(diasdb, Convert.ToDouble(txtDiasVacaciones.Text), (Int16)txtTipo.EditValue))
                            //{ XtraMessageBox.Show("Por favor verifica que la cantidad no supere la cantidad de dias disponibles", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);txtDiasVacaciones.Focus();return;}

                            ModificarVacacion(Trabajador, dtSalida, dtFinaliza, txtDiasVac, txtTipo, salidabd, regresobd, dtRetornoTrabajo, diasdb);
                        }
                        else
                        {
                            //SI SON IGUALES SOLO MODIFICAMOS
                            //VALIDAR QUE LA CANTIDAD DE DIAS NO SUPERE LA CANTIDAD DISPONIBLE
                           // if (SuperaLimite(diasdb, Convert.ToDouble(txtDiasVacaciones.Text), (Int16)txtTipo.EditValue))
                           // { XtraMessageBox.Show("Por favor verifica que la cantidad no supere la cantidad de dias disponibles", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtDiasVacaciones.Focus(); return; }

                            //VERIFICAR QUE LA FECHA DE SALIDA NO SEA FIN DE SEMANA        
                            if (type != (Int16)txtTipo.EditValue)
                            {
                                DialogResult pregunta = XtraMessageBox.Show("Haz modificado el tipo de vacacion a evaluar, ¿Deseas Continuar?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                if (pregunta == DialogResult.No)
                                {
                                    return;
                                }
                                else
                                {
                                    //ES ACTUALIZACION
                                    ModificarVacacion(Trabajador, dtSalida, dtFinaliza, txtDiasVac, txtTipo, salidabd, regresobd, dtRetornoTrabajo, diasdb);
                                }
                            }
                            else
                            {
                                //ES ACTUALIZACION
                                ModificarVacacion(Trabajador, dtSalida, dtFinaliza, txtDiasVac, txtTipo, salidabd, regresobd, dtRetornoTrabajo, diasdb);
                            }                            
                        }
                    }                  
                }
                //INSERT NUEVO REGISTRO
                else
                {
                    //VERIFICAR QUE EL NUMERO INGRESADO TENGA EL FORMATO CORRECTO
                    //valido = fnDecimal(txtDiasVac.Text);
                    //if (valido == false) { XtraMessageBox.Show("Numero no valido para dias", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtDiasVac.Focus(); return; }

                    //ES INSERT
                    existe = ExisteRegistro((DateTime)dtSalida.EditValue, (DateTime)dtFinaliza.EditValue);
                    if (existe) { XtraMessageBox.Show("Registro ya existe, por favor verifica los datos y vuelve a intentarlo", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);dtSalida.Focus();return;}

                    if (txtDiasVac.Text == "0")
                    {
                        DialogResult pregunta = XtraMessageBox.Show("¿Seguro quieres ingresar 0 dias?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (pregunta == DialogResult.No)
                            return;
                    }                    

                    //VALIDAR QUE LA CANTIDAD DE DIAS NO SUPERE LOS DISPONIBLES
                    datos = datosVacacion(contrato);

                    //DIAS INGRESADOS
                    dias = Convert.ToDouble(txtDiasVac.Text);   

                    //VERIFICAMOS QUE LOS DIAS INGRESADOS NO SUPEREN LOS DIAS DISPONIBLES
                    //if (Convert.ToInt32(txtTipo.EditValue) == 1 && (dias > Convert.ToDouble(datos["proporcional"])))
                    //{ XtraMessageBox.Show("Por favor verifica que los dias ingresados no superen los dias disponibles", "Dias", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
                    //LO MISMO DE ARRIBA PERO PARA DIAS PROGRESIVOS
                    if (Convert.ToInt32(txtTipo.EditValue) == 2 && (dias > Convert.ToDouble(datos["progresivo"])))
                    { XtraMessageBox.Show("Por favor verifica que los dias ingresados no superen los dias disponibles", "Dias", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                    //VALIDAR QUE LA FECHA DE INICIO NO SEA MENOR A LA FECHA DE VACACIONES
                    if (Convert.ToDateTime(dtSalida.EditValue) < Trabajador.FechaVacacion)
                    { XtraMessageBox.Show("La fecha de salida no puede ser menor a la fecha de inicio de vacaciones de la ficha", "Vacaciones", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }                    

                    //VALIDAR QUE LA FECHA DE SALIDA NO SEA MAYOR A LA FECHA DE TERMINO
                    if (Convert.ToDateTime(dtSalida.EditValue) > Convert.ToDateTime(dtFinaliza.EditValue))
                    { XtraMessageBox.Show("La fecha de salida no puede ser mayor a la fecha de retorno", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);dtSalida.Focus();return;}

                    //VALIDAR QUE LA FECHA QUE FINALIZA NO SEA MENOR A LA FECHA DE SALIDA (INICIO DE VAC.)
                    if (Convert.ToDateTime(dtFinaliza.EditValue) < Convert.ToDateTime(dtSalida.EditValue))
                    { XtraMessageBox.Show("La fecha de termino de vacaciones no puede ser inferior a la fecha de incio", "Fecha incorrecta", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                    //VALIDAR QUE LA FECHA DE RETORNO NO SEA MENOR A LA FECHA DE INICIO DE VACACIONES
                    if (Convert.ToDateTime(dtRetornoTrabajo.EditValue) < Convert.ToDateTime(dtSalida.EditValue))
                    { XtraMessageBox.Show("La fecha de retorno de vacaciones no puede ser menor a la fecha de inicio de vacaciones", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                    //VALIDAR QUE LA FECHA DE RETORNO NO SEA INFERIOR A LA FECHA DE TERMINO
                    if (Convert.ToDateTime(dtRetornoTrabajo.EditValue) < Convert.ToDateTime(dtFinaliza.EditValue))
                    { XtraMessageBox.Show("La fecha de retorno al trabajo no puede ser inferior a la fecha de termino de vacaciones", "Fecha Retorno", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                    //VERIFICA QUE LA FECHA DE SALIDA NO SEA FIN DE SEMANA
                   // if (EsFinSemana((DateTime)dtSalida.EditValue))
                   // { XtraMessageBox.Show("La fecha de salida debe ser un dia habil", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);dtSalida.Focus();return;}

                    //VERIFICAR QUE EL REGISTRO A GUARDAR NO EXISTA EN BD
                    IngresoVacacion(Trabajador, dtSalida, dtFinaliza, txtDiasVac, txtTipo, dtRetornoTrabajo);
                }
            }         
        }

        private void txtDiasVacaciones_KeyPress(object sender, KeyPressEventArgs e)
        {
          
        }

        private void gridVacaciones_Click(object sender, EventArgs e)
        {
            CargarCamposFromGrid();

            //RESET BTN
            op.Cancela = false;
            op.SetButtonProperties(btnNuevo, 1);
        }

        private void dtSalida_KeyDown(object sender, KeyEventArgs e)
        {
          
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            if (Trabajador.Contrato == "" || contrato == "")
            { XtraMessageBox.Show("Registro no válido", "Error ficha", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (Trabajador.PeriodoPersona < Calculo.PeriodoObservado)
            { XtraMessageBox.Show("No puedes eliminar este registro", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }            

            if (viewVacaciones.RowCount>0 && contrato != "" && Trabajador != null)
            {
                //OBTENER LA FECHA DE SALIDA Y LA FECHA DE RETORNO DESDE GRILLA
                DateTime salida = DateTime.Now.Date;
                DateTime finaliza = DateTime.Now.Date;

                salida = (DateTime)viewVacaciones.GetFocusedDataRow()["salida"];
                finaliza = (DateTime)viewVacaciones.GetFocusedDataRow()["finaliza"];
              
                EliminarVacacion(Trabajador, salida, finaliza);
            }
            else
            {
                XtraMessageBox.Show("Registro no valido", "Advertencia",MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void txtDiasVacaciones_EditValueChanged(object sender, EventArgs e)
        {

        }



        private void gridVacaciones_KeyUp(object sender, KeyEventArgs e)
        {
            CargarCamposFromGrid();
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
           Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "rptvacacion") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (op.Cancela)
            {
                op.Cancela = false;
                op.SetButtonProperties(btnNuevo, 1);
            }

            if (HayVacaciones(contrato))
            {
                InformeVacaciones(contrato);
            }
            else
            {
                XtraMessageBox.Show("No hay registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            

        }

        private void panelControl6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnFeriados_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmferiado") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

            if (op.Cancela)
            {
                op.Cancela = false;
                op.SetButtonProperties(btnNuevo, 1);
            }

            frmFeriado fer = new frmFeriado();
            fer.StartPosition = FormStartPosition.CenterScreen;
            fer.ShowDialog();         
        }

        private void btnImpresionRapida_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "rptvacacion") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (op.Cancela)
            {
                op.Cancela = false;
                op.SetButtonProperties(btnNuevo, 1);
            }

            if (HayVacaciones(contrato))
            {
                InformeVacaciones(contrato, true);
            }
            else
            {
                XtraMessageBox.Show("No hay registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void panelControl2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void txtTipo_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void dtSalida_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtDiasVacaciones_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void dtFinaliza_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void dtRetornoTrabajo_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtpropAnual_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtAnualesRestantes_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtpropRestante_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtpropUsados_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtpropTotal_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtprogAcumulados_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtUsadosProg_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtTotalProg_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            DateTime salida = DateTime.Now.Date;
            DateTime finaliza = DateTime.Now.Date;

            if (viewVacaciones.RowCount > 0)
            {
                salida = Convert.ToDateTime(viewVacaciones.GetFocusedDataRow()["salida"]);
                finaliza = Convert.ToDateTime(viewVacaciones.GetFocusedDataRow()["finaliza"]);

                if (CambiosSinGuardar(contrato, salida, finaliza))
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

        private void gridVacaciones_DoubleClick(object sender, EventArgs e)
        {            
            if (viewVacaciones.RowCount>0)
            {
                DialogResult pregunta = XtraMessageBox.Show("¿Deseas revisar comprobante de vacaciones?", "Pregunta", MessageBoxButtons.YesNo,MessageBoxIcon.Question);
                if (pregunta == DialogResult.Yes)
                {
                    DateTime salida = DateTime.Now.Date;
                    DateTime finaliza = DateTime.Now.Date;
                    Hashtable dataComprobante = new Hashtable();

                    salida = (DateTime)viewVacaciones.GetFocusedDataRow()["salida"];
                    finaliza = (DateTime)viewVacaciones.GetFocusedDataRow()["finaliza"];

                    //dataComprobante = CalculoDias(FechaProgresivo, salida);
                    PrimerIngreso(FechaProgresivo, false, true, finaliza, true);

                    GeneraComprobante(salida, finaliza, contrato, DataReporte);
                }                           
            }
        }

        private void viewVacaciones_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            DXPopupMenu menu = e.Menu;

            if (menu != null)
            {
                //CREAMOS UN SUBMENU PARA EL MENU
                DXMenuItem submenu = new DXMenuItem("Ver comprobante", new EventHandler(ShowComprobante_Click));
                DXMenuItem editar = new DXMenuItem("Editar comprobante", new EventHandler(EditarComprobante_Click));
                DXMenuItem PrintMenu = new DXMenuItem("Imprimir comprobante", new EventHandler(ImprimirComprobante_Click));
                PrintMenu.ImageOptions.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("images/print/print_16x16.png");
                editar.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("images/edit/edit_16x16.png");
                submenu.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("images/actions/show_16x16.png");                   
                e.Menu.Items.Clear();
                //AGREGAMOS SUBMENU A MENU
                menu.Items.Add(submenu);
                menu.Items.Add(editar);
                menu.Items.Add(PrintMenu);
            }
        }

        private void ImprimirComprobante_Click(object sender, EventArgs e)
        {
            //RptComprobanteVacacion Comp = new RptComprobanteVacacion();
            //Reporte externo
            ReportesExternos.rptComprobanteVacacion Comp = new ReportesExternos.rptComprobanteVacacion();
            Comp.LoadLayoutFromXml(Path.Combine(fnSistema.RutaCarpetaReportesExterno, "rptComprobanteVacacion.repx"));

            //Comp.Parameters["imagen"].Value = Imagen.GetLogoFromBd();
            Documento doc = new Documento("", 0);

            if (viewVacaciones.RowCount > 0 && Trabajador != null)
            {            
                DateTime salida = DateTime.Now.Date;
                DateTime finaliza = DateTime.Now.Date;
                //Hashtable dataComprobante = new Hashtable();

                salida = (DateTime)viewVacaciones.GetFocusedDataRow()["salida"];
                finaliza = (DateTime)viewVacaciones.GetFocusedDataRow()["finaliza"];

                //dataComprobante = CalculoDias(FechaProgresivo, salida);
                //PrimerIngreso(FechaProgresivo, false, true, finaliza, true);
                //GeneraComprobante(salida, finaliza, contrato, DataReporte, true);
                vacaciones.PrimerIngreso(Trabajador, DataReporte, true, finaliza);
                Comp = (ReportesExternos.rptComprobanteVacacion)vacaciones.GeneraComprobante(salida, finaliza, Trabajador.Contrato, DataReporte);
                //Comp.SaveLayoutToXml(Path.Combine(fnSistema.RutaCarpetaReportesExterno, "rptComprobanteVacacion.repx"));
                if (Comp != null)
                    doc.PrintDocument(Comp);
            }
        }

        //EVENTO QUE SE LANZA CUANDO SE HACE CLICK EN EL CONTEXT MENU
        private void ShowComprobante_Click(object sender, EventArgs e)
        {
            //RptComprobanteVacacion Comp = new RptComprobanteVacacion();
            //Reporte externo
            ReportesExternos.rptComprobanteVacacion Comp = new ReportesExternos.rptComprobanteVacacion();
            Comp.LoadLayoutFromXml(Path.Combine(fnSistema.RutaCarpetaReportesExterno, "rptComprobanteVacacion.repx"));

            //Comp.Parameters["imagen"].Value = Imagen.GetLogoFromBd();
            Documento doc = new Documento("", 0);

            if (viewVacaciones.RowCount>0 && Trabajador != null)
            {
                DateTime salida = DateTime.Now.Date;
                DateTime finaliza = DateTime.Now.Date;
                Hashtable data = new Hashtable();

                salida = (DateTime)viewVacaciones.GetFocusedDataRow()["salida"];
                finaliza = (DateTime)viewVacaciones.GetFocusedDataRow()["finaliza"];
                //data = CalculoDias(FechaProgresivo, salida);
                //PrimerIngreso(Trabajador.FechaProgresivo, false, true, finaliza, true);
                vacaciones.PrimerIngreso(Trabajador, DataReporte, true, finaliza);
                Comp = (ReportesExternos.rptComprobanteVacacion)vacaciones.GeneraComprobante(salida, finaliza, Trabajador.Contrato, DataReporte);
                //Comp.SaveLayoutToXml(Path.Combine(fnSistema.RutaCarpetaReportesExterno, "rptComprobanteVacacion.repx"));
                if (Comp != null)
                    doc.ShowDocument(Comp);
            }
        }

        private void EditarComprobante_Click(object sender, EventArgs e)
        {
            AbreEditorComprobanteVacaciones();
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "rptvacacion") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (op.Cancela)
            {
                op.Cancela = false;
                op.SetButtonProperties(btnNuevo, 1);
            }

            if (viewVacaciones.RowCount > 0 && contrato != "")
            {
                string PathFile = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Vacacion_" + contrato + ".xlsx";
                if (FileExcel.IsExcelInstalled())
                {
                    DataTable tabla = new DataTable();
                    tabla = TablaExcel(contrato, Calculo.PeriodoObservado);

                    if (tabla == null)
                    {
                        XtraMessageBox.Show("No se encontró informacion", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    tabla = TablaFinalExcel(tabla);

                    if (tabla == null)
                    {
                        XtraMessageBox.Show("No se encontró informacion", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (FileExcel.CrearArchivoExcelConSum(tabla, PathFile, "dias"))
                    {
                        DialogResult pregunta = XtraMessageBox.Show("Documento generado correctamente, ¿Deseas abrir el archivo?", "Informacion", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                        if (pregunta == DialogResult.Yes)
                        {
                            FileExcel.AbrirExcel(PathFile);
                        }
                    }
                    else
                    {
                        XtraMessageBox.Show("Lamentablemente el documento no se pudo generar, verifica que el archivo no este abierto", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    XtraMessageBox.Show("Parece ser que tu sistema no tiene instalado office", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void toolTipController1_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            //LISTADO DE FECHAS DE ACUERDO A CONTRATO
            List<DateTime> Fechas = new List<DateTime>();
            Fechas = ListadoVacacionesRetorno(contrato);

            //LISTADO FERIADOS
            List<DateTime> Feriados = new List<DateTime>();
            Feriados = ListadoFeriados();

            if (Fechas.Count>0)
            {
                if (e.SelectedControl == calendarioVacacion)
                {
                    CalendarHitInfo HitInfo = calendarioVacacion.GetHitInfo(e.ControlMousePosition);
                    if (HitInfo.IsInCell)
                    {
                        //RECORREMOS FECHAS
                        foreach (DateTime fecha in Fechas)
                        {
                            if (HitInfo.Cell.Date == fecha)
                            {
                                //HitInfo.Cell.Appearance.BackColor = Color.Red;
                                //HitInfo.Cell.Appearance.ForeColor = Color.Blue;

                                ToolTipControlInfo info = new ToolTipControlInfo(HitInfo.Cell, "Fecha retorno trabajo");
                                e.Info = info;
                            }
                        }

                        //PARA FERIADOS
                        foreach (DateTime fecha in Feriados)
                        {
                            if (HitInfo.Cell.Date == fecha)
                            {
                                //HitInfo.Cell.Appearance.BorderColor = Color.DarkRed;

                                ToolTipControlInfo info = new ToolTipControlInfo(HitInfo.Cell, getDescripcionFeriado(fecha));
                                e.Info = info;
                            }
                        }
                    }
                }                
            }
        }

        private void calendarioVacacion_CustomDrawDayNumberCell(object sender, DevExpress.XtraEditors.Calendar.CustomDrawDayNumberCellEventArgs e)
        {
            //LISTADO DE FECHAS DE ACUERDO A CONTRATO
            List<DateTime> Fechas = new List<DateTime>();
            //FECHAS DE RETORNO
            List<DateTime> FechasRetorno = new List<DateTime>();
            //LISTADO FERIADOS
            List<DateTime> Feriados = new List<DateTime>();
            Feriados = ListadoFeriados();

            Fechas = ListadoVacaciones(contrato);
            FechasRetorno = ListadoVacacionesRetorno(contrato);

            if (Fechas.Count > 0 && FechasRetorno.Count>0)
            {
                //RECORREMOS FECHAS
                foreach (DateTime Fecha in Fechas)
                {
                    if (e.Date == Fecha)
                    {
                        e.Style.ForeColor = Color.White;
                        e.Style.BackColor = Color.Orange;
                        e.Style.BorderColor = Color.Gray;
                    }
                }

                //RECORREMOS FECHA DE RETORNO
                foreach (DateTime Retorno in FechasRetorno)
                {
                    if (e.Date == Retorno)
                    {
                        e.Style.ForeColor = Color.White;
                        e.Style.BackColor = Color.ForestGreen;
                        e.Style.BorderColor = Color.Gray;
                    }
                }


                //FERIADOS
                foreach (DateTime fecha in Feriados)
                {
                    if (e.Date == fecha)
                    {
                        e.Style.ForeColor = Color.Red;
                        e.Style.BorderColor = Color.DarkRed;
                    }
                }
            }         
        }

        private void btnPdf_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "rptvacacion") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (op.Cancela)
            {
                op.Cancela = false;
                op.SetButtonProperties(btnNuevo, 1);
            }

            if (HayVacaciones(contrato))
            {
                InformeVacaciones(contrato, false, true);
            }
            else
            {
                XtraMessageBox.Show("No hay registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void txtDiasVacaciones_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar) || e.KeyChar == (char)44)
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
            //TextEdit dias = sender as TextEdit;

            if (e.KeyData == Keys.Enter)
            {
                ValidaDias();
            }
        }

        private void ValidaDias()
        {
            if (txtDiasVac.Text == "")
            { XtraMessageBox.Show("Por favor ingresa un día válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (Convert.ToDouble(txtDiasVac.Text) == 0)
            { XtraMessageBox.Show("Por favor ingresa un día válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            bool valido = fnDecimal(txtDiasVac.Text);
            if (valido == false) { XtraMessageBox.Show("Valor no valido para dias", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtDiasVac.Focus(); return; }

            dtFinaliza.DateTime = vacaciones.AgregarDiasFinSemana(dtSalida.DateTime, Convert.ToDouble(txtDiasVac.Text));
            dtRetornoTrabajo.DateTime = vacaciones.DiaRetornoTrabajo(dtSalida.DateTime, Convert.ToDouble(txtDiasVac.Text), Trabajador.Jornada);
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Tab)
            {
                if (txtDiasVac.ContainsFocus)
                {
                    if (txtDiasVac.Text == "")
                    { XtraMessageBox.Show("Por favor ingresa un día válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    if (Convert.ToDouble(txtDiasVac.Text) == 0)
                    { XtraMessageBox.Show("Por favor ingresa un día válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    dtFinaliza.DateTime = vacaciones.AgregarDiasFinSemana(dtSalida.DateTime, Convert.ToDouble(txtDiasVac.Text));
                    dtRetornoTrabajo.DateTime = vacaciones.DiaRetornoTrabajo(dtSalida.DateTime, Convert.ToDouble(txtDiasVac.Text), Trabajador.Jornada);
                }
            }

            return base.ProcessDialogKey(keyData);
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

        private void txtDiasVac_Leave(object sender, EventArgs e)
        {
            ValidaDias();
        }

        private void AbreEditorComprobanteVacaciones() 
        {
            //RptComprobanteVacacion Comp = new RptComprobanteVacacion();
            //Reporte externo
            ReportesExternos.rptComprobanteVacacion Comp = new ReportesExternos.rptComprobanteVacacion();
            Comp.LoadLayoutFromXml(Path.Combine(fnSistema.RutaCarpetaReportesExterno, "rptComprobanteVacacion.repx"));

            DateTime salida = viewVacaciones.GetFocusedDataRow() == null ? DateTime.Now : Convert.ToDateTime(viewVacaciones.GetFocusedDataRow()["salida"]);
            DateTime finaliza = viewVacaciones.GetFocusedDataRow() == null ? DateTime.Now : Convert.ToDateTime(viewVacaciones.GetFocusedDataRow()["finaliza"]);

            //salida = (DateTime)viewVacaciones.GetFocusedDataRow()["salida"];
            //finaliza = (DateTime)viewVacaciones.GetFocusedDataRow()["finaliza"];

            vacaciones.PrimerIngreso(Trabajador, DataReporte, true, finaliza);
            Comp = (ReportesExternos.rptComprobanteVacacion)vacaciones.GeneraComprobante(salida, finaliza, Trabajador.Contrato, DataReporte);
            splashScreenManager1.ShowWaitForm();
            DiseñadorReportes.MostrarEditorLimitado(Comp, "rptComprobanteVacacion.repx", splashScreenManager1);
        }

        private void btnEditarReporte_Click(object sender, EventArgs e)
        {
            InformeVacaciones(contrato, editar: true);
        }

        private void btnEditarComprobante_Click(object sender, EventArgs e)
        {
            AbreEditorComprobanteVacaciones();
        }
    }
}