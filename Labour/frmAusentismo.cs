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
using System.Globalization;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraReports.UI;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors.Controls;
using DevExpress.Skins;
using System.IO;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Menu;
using DevExpress.XtraGrid.Views.Grid;

namespace Labour
{
    public partial class frmAusentismo : DevExpress.XtraEditors.XtraForm
    {
        //PARA DESHABILITAR EL BOTON CERRAR
        [DllImport("user32")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32")]
        static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        const int MF_BYCOMMAND = 0;
        const int MF_DISABLED = 2;
        const int SC_CLOSED = 0xF060;

        //PARA GUARDAR LA FECHA DE INICIO Y TERMINO DE CONTRATO
        private DateTime InicioContrato = DateTime.Now.Date;
        private DateTime TerminoContrato = DateTime.Now.Date;

        //PARA MANEJAR LA POSICION DENTRO DEL ARREGLO (para cargar datos usando botones navegacionales)
        private int Position = 0;

        //PARA MANIPULAR EL CODIGO DE CONTRATO
        private string Contrato = "";

        //PARA MANIPULAR EL PERIODO ASOCIADO A ESE CONTRATO
        private int periodo = 0;

        //GUARDAR LA POSICION FILA SELECCIONADA EN GRILLA
        private int PosicionGrid = 0;

        //PARA SABER SI ES UPDATE O INSERT
        private bool Update = false;
       
        //BOTON NUEVO
        Operacion op;

        //INFORMACION TRABAJADOR
        Persona Trabajador;

        public frmAusentismo()
        {
            InitializeComponent();           
        }

        //CONSTRUCTOR PARA PASAR NUMERO DE CONTRATO
        public frmAusentismo(string Contrato, int periodo)
        {
            InitializeComponent();
            this.Contrato = Contrato;
            this.periodo = periodo;
            //fnCalendarios();
        }

        private void frmAusentismo_Load(object sender, EventArgs e)
        {           
            var sm = GetSystemMenu(Handle, false);
            EnableMenuItem(sm, SC_CLOSED, MF_BYCOMMAND | MF_DISABLED);            

            //CARGAR COMBOBOX MOTIVO
            datoCombobox.spllenaComboBox("SELECT id, nombre FROM motivo", txtmotivo, "id", "nombre", true);            
            
            string grilla = "";

            if (Contrato != "" && periodo != 0)
            {
                lblperiodo.Text = "Periodo: " + fnSistema.PrimerMayuscula(fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(periodo)));

                op = new Operacion();

                Trabajador = new Persona();
                Trabajador = Persona.GetInfo(Contrato, periodo);                
                
                //OBTENER NOMBRE COMPLETO EMPLEADO                
                lblcontrato.Text = Trabajador.NombreCompleto;               

                grilla = "SELECT contrato, descripcion, motivo, fechaEvento, fechaAplic, numdias, fecFin, fecFinApli, " +
                         $"rebSueldo FROM ausentismo INNER JOIN motivo ON ausentismo.motivo = motivo.id WHERE contrato = '{Contrato}' ORDER BY fechaevento desc";
              
                fnSistema.spllenaGridView(gridAusentismo, grilla);
                fnSistema.spOpcionesGrilla(viewAusentismo);
                fnDefaultProperties();

                if (viewAusentismo.RowCount > 0)
                {
                    fnColumnasGrilla();
                    fnCargarCampos(0);
                }               
            }
            //Haberes hab = new Haberes();
            //hab.ValueFormula(Contrato, "GRATIFI");
        }

        #region "MANEJO DE DATOS"
        //INGRESO NUEVO AUSENTISMO
        /*
         * DATOS DE ENTRADA:
         * contrato
         * Fecha Evento
         * fecha aplicacion
         * numero dias
         * fecha fin
         * fecha fin aplicacion
         * motivo
         * descripcion
         * rebaja sueldo
         */
        private void fnNuevoAusentismo(string pContrato, DateEdit pEvento, DateEdit pApli, TextEdit pDias,
            DateEdit pFin, DateEdit pFinApli, LookUpEdit pMotivo, TextEdit pDesc, CheckEdit pReb)
        {
            //SQL PARA INSERT
            string sql = "INSERT INTO ausentismo(contrato, fechaEvento, fechaAplic, numdias, fecFin, fecFinApli, " +
                "motivo, descripcion, rebSueldo, periodoAnterior, diasAnterior, periodoSiguiente, diasSiguiente, folio) " +
                "VALUES(@pContrato, @pEvento, @pApli, @pDias, @pFin, @pFinApli, @pMotivo, @pDesc, @pReb, @periodoAnterior, @pdiasAnterior," +
                "@periodoSiguiente, @pdiasSiguiente, @pFolio)";

            // SON LICENCIAS --> LICENCIA MEDICA, LICENCIA MATERNAL, ACCIDENTE
            // --> CODES => 1, 2, 5
            // SON AUSENTISMO --> INASISTENCIA
            // --> CODES => 4

            //PARA OBTENER EL PERIODO DE ACUERDO A FECHA DE EVENTO
            //EJ: 27-02-2018 => 201802
            int PeriodoEvento = 0, lastFolio = 0;

            double number = Convert.ToDouble(pDias.Text);
            double[] numeros = new double[2];

            //OBTENER LA CANTIDAD DE DIAS QUE CORRESPONDE A LOS ACTUALES Y AL PERIODO SIGUIENTE...
            numeros = fnDiasPeriodo(DateTime.Parse(pApli.EditValue.ToString()),DateTime.Parse(pFinApli.EditValue.ToString()), number);
            double DiasActual = 0, DiasPosterior = 0;

            //DIAS QUE REPRESENTAN DIAS DEL PERIODO OBSERVADO
            DiasActual = numeros[0];
            //DIAS QUE REPRESENTAN DIAS QUE PASAN PARA EL DIAS PERIODO SIGUIENTE
            DiasPosterior = numeros[1];

            //bool f5 = false, f6 = false;
            int tipo = 0;            

            string d1 = "", d2 = "";
            d1 = DiasActual + "";
            d2 = DiasPosterior + "";

            if (d1.Contains(","))
                d1 = d1.Replace(",", ".");

            if (d2.Contains(","))
                d2 = d2.Replace(",", ".");

            //OBTENER EL PERIODO ACTUAL
            int Actual = periodo;

            //OBTENEMOS EL PERIODO DESDE LA FECHA DE EVENTO
            PeriodoEvento = fnSistema.PeriodoFromDate((DateTime)pApli.EditValue);
            Actual = PeriodoEvento;

            //OBTENER PERIODO SIGUIENTE EN BASE A FECHA FIN DE AUSENTISMO
            int Siguiente = 0;
            Siguiente = fnSistema.PeriodoFromDate(pFinApli.DateTime);

            //ULTIMO FOLIO  
            lastFolio = UltimoFolio(pContrato);

            //VALIDAR QUE LA FECHA QUE SE INTENTA INGRESAR NO ESTE OCUPADA COMO VACACION
            if (vacaciones.FechaOcupada(Convert.ToDateTime(pEvento.EditValue), Convert.ToDateTime(pFin.EditValue), pContrato))
            { XtraMessageBox.Show("No puedes ingresar una fecha que está ocupada como vacación", "Fecha no disponible", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (pApli.DateTime < pEvento.DateTime)
            { XtraMessageBox.Show("Por favor verifica que la fecha de aplicacion no sea menor a la fecha de evento", "Fecha", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            //FECHA TERMINO APLICACION
            fnFechaFinal(pDias, pFinApli, pApli);
            //FECHA TERMINO REAL
            fnFechaFinal(pDias, pFin, pEvento);         

            SqlCommand cmd;
            int res = 0;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pContrato", pContrato));
                        cmd.Parameters.Add(new SqlParameter("@pEvento", pEvento.EditValue));
                        cmd.Parameters.Add(new SqlParameter("@pApli", pApli.EditValue));
                        cmd.Parameters.Add(new SqlParameter("@pDias", Convert.ToDecimal(pDias.Text)));
                        cmd.Parameters.Add(new SqlParameter("@pFin", pFin.EditValue));
                        cmd.Parameters.Add(new SqlParameter("@pFinApli", pFinApli.EditValue));
                        cmd.Parameters.Add(new SqlParameter("@pMotivo", pMotivo.EditValue));
                        cmd.Parameters.Add(new SqlParameter("@pDesc", pDesc.Text));
                        cmd.Parameters.Add(new SqlParameter("@pReb", pReb.EditValue));
                        cmd.Parameters.Add(new SqlParameter("@periodoAnterior", Actual));
                        cmd.Parameters.Add(new SqlParameter("@pdiasAnterior", d1));
                        cmd.Parameters.Add(new SqlParameter("@periodoSiguiente", Siguiente));
                        cmd.Parameters.Add(new SqlParameter("@pdiasSiguiente", d2));
                        cmd.Parameters.Add(new SqlParameter("@pFolio", lastFolio));
                        // cmd.Parameters.Add(new SqlParameter("@pausencia", ausencia));
                        //cmd.Parameters.Add(new SqlParameter("@plicencia", licencia));

                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            XtraMessageBox.Show("Ingreso correcto", "Guardar", MessageBoxButtons.OK, MessageBoxIcon.Information);                            

                            //GUARDAMOS EN LOG
                            logRegistro log = new logRegistro(User.getUser(), "SE INGRESA NUEVO AUSENTISMO ASOCIADO A CONTRATO " + pContrato, "AUSENTISMO", "0", "EVENTO: " +pEvento.EditValue, "INGRESAR");
                            log.Log();

                            //calendarioAusentismo.CustomDrawDayNumberCell += CalendarioAusentismo_CustomDrawDayNumberCell;

                            //CARGAMOS GRILLA
                            //string grilla = string.Format("SELECT periodoanterior ,contrato, fechaEvento, fechaAplic, numdias, fecFin, fecFinApli, motivo, " +
                            //"descripcion, rebSueldo FROM ausentismo WHERE contrato='{0}' ORDER BY periodoanterior desc", Contrato);

                            string grilla = "SELECT contrato, descripcion, motivo, fechaEvento, fechaAplic, numdias, fecFin, fecFinApli, " +
                            $"rebSueldo FROM ausentismo WHERE contrato = '{Contrato}' ORDER BY fechaevento desc";

                            fnSistema.spllenaGridView(gridAusentismo, grilla);
                            fnColumnasGrilla();

                            fnCargarCampos(0);

                            op.Cancela = false;
                            op.SetButtonProperties(btnNuevo, 1);                            
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
                    XtraMessageBox.Show("No hay conexion con la base de datos", "Base de datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        private void CalendarioAusentismo_CustomDrawDayNumberCell(object sender, DevExpress.XtraEditors.Calendar.CustomDrawDayNumberCellEventArgs e)
        {
            //throw new NotImplementedException();
            //LISTADO DE FECHAS
            List<DateTime> Ausentismos = new List<DateTime>();
            Ausentismos = ListadoAusentismos(Contrato);

            if (Ausentismos.Count > 0)
            {
                foreach (DateTime fecha in Ausentismos)
                {
                    if (e.Date == fecha)
                    {
                        e.Style.ForeColor = Color.White;
                        e.Style.BackColor = Color.Orange;
                        e.Style.BorderColor = Color.Gray;
                    }
                }
            }
        }

        //MODIFICAR UN REGISTRO
        private void fnModificarRegistro(string pContrato, DateEdit pEvento, DateEdit pApli, TextEdit pDias,
            DateEdit pFin, DateEdit pFinApli, LookUpEdit pMotivo, TextEdit pDesc, CheckEdit pReb, DateTime pEventoAntiguo)
        {
            string sql = "UPDATE ausentismo SET fechaEvento=@pEvento,  fechaAplic=@pApli, numdias=@pDias, fecFin=@pFin, fecFinApli=@pFinApli," +
                "motivo=@pMotivo, descripcion=@pDesc, rebSueldo=@pReb, periodoAnterior=@periodoAnterior, diasAnterior=@pdiasAnterior, " +
                "periodoSiguiente=@periodoSiguiente, diasSiguiente=@pdiasSiguiente " +
                "WHERE contrato=@pContrato AND fechaEvento=@pEventoAntiguo";
            SqlCommand cmd;
            int res = 0;

            //SON LICENCIAS --> LICENCIA MEDICA, LICENCIA MATERNAL, ACCIDENTE
            // --> CODES => 1, 2, 5
            //SON AUSENTISMO --> INASISTENCIA
            // --> CODES => 4

            //PARA OBTENER EL DESDE LA FECHA DE EVENTO
            int PeriodoEvento = 0;

            double number = Convert.ToDouble(pDias.Text);
            double[] numeros = new double[2];
            //OBTENEMOS LA CANTIDAD DE DIAS PARA EL MES Y LA CANTIDAD DE DIAS PARA EL MES SIGUIENTE...
            numeros = fnDiasPeriodo(DateTime.Parse(pApli.EditValue.ToString()), DateTime.Parse(pFinApli.EditValue.ToString()), number);
            double DiasActual = 0, DiasPosterior = 0;
            bool f5 = false, f6 = false;
            int tipo = 0;

            //CORRESPONDE A LOS DIAS DEL MES (DE ACUERDO A FECHA EVENTO)
            DiasActual = numeros[0];
            //CORRESPONDE A LOS DIAS QUE SON DEL MES SIGUIENTE...
            DiasPosterior = numeros[1];

            string d1 = "", d2 = "";
            d1 = DiasActual + "";
            d2 = DiasPosterior + "";

            if (d1.Contains(","))
                d1 = d1.Replace(",", ".");

            if (d2.Contains(","))
                d2 = d2.Replace(",", ".");          

            //OBTENER EL PERIODO ACTUAL
            int Actual = periodo;

            //PERIODO ACTUAL SERÁ PERIODO DESDE FECHA EVENTO
            PeriodoEvento = fnSistema.PeriodoFromDate(pApli.DateTime);
            Actual = PeriodoEvento;

            //PERIODO SIGUIENTE LO TOMAMOS DE LA FECHA DE FIN
            int siguiente = fnSistema.PeriodoFromDate(pFinApli.DateTime);

            //VALIDAR QUE LA FECHA QUE SE INTENTA INGRESAR NO ESTE OCUPADA COMO VACACION
            if (vacaciones.FechaOcupada(Convert.ToDateTime(pEvento.EditValue), Convert.ToDateTime(pFin.EditValue), pContrato))
            { XtraMessageBox.Show("No puedes ingresar una fecha que está ocupada como vacación", "Fecha no disponible", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (pApli.DateTime < pEvento.DateTime)
            {
                XtraMessageBox.Show("Por favor verifica que la fecha de aplicacion no sea menor a fecha de evento", "Fecha", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            //Fecha termino aplicacion
            fnFechaFinal(pDias, pFinApli, pEvento);
            //Fecha termino aplicacion
            fnFechaFinal(pDias, pFin, pEvento);
   

            //TABLA HASH PARA DATOS
            Hashtable datosAusentismo = new Hashtable();
            datosAusentismo = PrecargaAusentismo(pContrato, periodo, pEventoAntiguo);

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pContrato", pContrato));
                        cmd.Parameters.Add(new SqlParameter("@pEvento", pEvento.EditValue));
                        cmd.Parameters.Add(new SqlParameter("@pApli", pApli.EditValue));
                        cmd.Parameters.Add(new SqlParameter("@pDias", Convert.ToDecimal(pDias.Text)));
                        cmd.Parameters.Add(new SqlParameter("@pFin", pFin.EditValue));
                        cmd.Parameters.Add(new SqlParameter("@pFinApli", pFinApli.EditValue));
                        cmd.Parameters.Add(new SqlParameter("@pMotivo", pMotivo.EditValue));
                        cmd.Parameters.Add(new SqlParameter("@pDesc", pDesc.Text));
                        cmd.Parameters.Add(new SqlParameter("@pReb", pReb.EditValue));
                        cmd.Parameters.Add(new SqlParameter("@pEventoAntiguo", pEventoAntiguo));
                        cmd.Parameters.Add(new SqlParameter("@periodoAnterior", Actual));
                        cmd.Parameters.Add(new SqlParameter("@pdiasAnterior", d1));
                        cmd.Parameters.Add(new SqlParameter("@periodoSiguiente", siguiente));
                        cmd.Parameters.Add(new SqlParameter("@pdiasSiguiente",d2));
                        cmd.Parameters.Add(new SqlParameter("@periodo", Actual));
                        //cmd.Parameters.Add(new SqlParameter("@pausencia", ausencia));
                        //cmd.Parameters.Add(new SqlParameter("@plicencia", licencia));

                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            XtraMessageBox.Show("Registro Actualizado", "Actualizar", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            //calendarioAusentismo.CustomDrawDayNumberCell += CalendarioAusentismo_CustomDrawDayNumberCell1;
                           
                            //SI HAY CAMBIOS REGISTRAMOS EVENTO EN LOG
                            ComparaValorAusentismo(datosAusentismo, (DateTime)pEvento.EditValue,(DateTime)pApli.EditValue, pContrato, (DateTime)pFin.EditValue,
                                (DateTime)pFinApli.EditValue, Convert.ToInt32(pMotivo.EditValue), pDesc.Text, (bool)pReb.EditValue, decimal.Parse(pDias.Text));

                            //string grilla = string.Format("SELECT periodoanterior ,contrato, fechaEvento, fechaAplic, numdias, fecFin, fecFinApli, motivo, " +
                            //"descripcion, rebSueldo FROM ausentismo WHERE contrato='{0}' ORDER BY periodoanterior desc", Contrato);

                            string grilla = "SELECT contrato, descripcion, motivo, fechaEvento, fechaAplic, numdias, fecFin, fecFinApli, " +
                         $"rebSueldo FROM ausentismo WHERE contrato = '{Contrato}' ORDER BY fechaevento desc";

                            fnSistema.spllenaGridView(gridAusentismo, grilla);

                            fnCargarCampos(PosicionGrid);                           
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
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        private void CalendarioAusentismo_CustomDrawDayNumberCell1(object sender, DevExpress.XtraEditors.Calendar.CustomDrawDayNumberCellEventArgs e)
        {
            //LISTADO DE FECHAS
            List<DateTime> Ausentismos = new List<DateTime>();
            Ausentismos = ListadoAusentismos(Contrato);

            if (Ausentismos.Count > 0)
            {
                foreach (DateTime fecha in Ausentismos)
                {
                    if (e.Date == fecha)
                    {
                        e.Style.ForeColor = Color.White;
                        e.Style.BackColor = Color.Orange;
                        e.Style.BorderColor = Color.Gray;
                    }
                }
            }
        }

        //ELMIMINAR UN REGISTRO
        private void fnEliminarRegistro(string pContrato, DateTime pEvento)
        {
            string sql = "DELETE FROM ausentismo WHERE contrato=@pContrato AND fechaEvento=@pEvento";
            SqlCommand cmd;
            int res = 0;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pContrato", pContrato));
                        cmd.Parameters.Add(new SqlParameter("@pEvento", pEvento));
                        //EJECUTAR CONSULTA
                        res = cmd.ExecuteNonQuery();

                        if (res > 0)
                        {
                            XtraMessageBox.Show("Registro Eliminado", "Eliminar", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            //GUARDAR EVENTO LICENCIA
                            logRegistro log = new logRegistro(User.getUser(), "SE ELIMINA AUSENTISMO ASOCIADO A CONTRATO " + pContrato + " CON FECHA " + pEvento , "AUSENTISMO", "0", "0", "ELIMINAR");
                            log.Log();

                            //string grilla = string.Format("SELECT periodoanterior ,contrato, fechaEvento, fechaAplic, numdias, fecFin, fecFinApli, motivo, " +
                            // "descripcion, rebSueldo FROM ausentismo WHERE contrato='{0}' ORDER BY periodoanterior desc", Contrato);

                            string grilla = "SELECT contrato, descripcion, motivo, fechaEvento, fechaAplic, numdias, fecFin, fecFinApli, " +
                            $"rebSueldo FROM ausentismo WHERE contrato = '{Contrato}' ORDER BY fechaevento desc";

                            fnSistema.spllenaGridView(gridAusentismo, grilla);

                            fnCargarCampos(0);
                            
                        }
                        else
                        {
                            XtraMessageBox.Show("Error al intentar eliminar", "Eliminar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        //LIMPIAR CAMPOS
        private void fnLimpiarCampos()
        {
            txtdesc.Text = "";
            txtdias.Text = "1";
            txtmotivo.ItemIndex = 1;
            dtAplicacion.DateTime = fnSistema.PrimerDiaMes(Calculo.PeriodoObservado);
            dtEvento.DateTime = fnSistema.PrimerDiaMes(Calculo.PeriodoObservado);
            dtFin.DateTime = fnSistema.PrimerDiaMes(Calculo.PeriodoObservado);
            dtfinApli.DateTime = fnSistema.PrimerDiaMes(Calculo.PeriodoObservado);
            Update = false;
            btnEliminar.Enabled = false;
            lblError.Visible = false;

            txtdesc.ReadOnly = false;
            txtdias.ReadOnly = false;
            dtAplicacion.ReadOnly = false;
            dtEvento.ReadOnly = false;
            txtmotivo.ReadOnly = false;
            cbRebaja.ReadOnly = false;

            dtEvento.Focus();

            btnGuardar.Enabled = true;

            //DESHABILITAMOS LOS BOTONES
            btnExcel.Enabled = false;
            btnImpresionRapida.Enabled = false;
            btnImprimir.Enabled = false;
            btnPdf.Enabled = false;
            btnEliminar.Enabled = false;

           
        }
        
        //PROPIEDADES POR DEFECTO
        private void fnDefaultProperties()
        {
            dtEvento.DateTime = DateTime.Now;
            dtAplicacion.DateTime = DateTime.Now;
            dtFin.DateTime = DateTime.Now;
            dtfinApli.DateTime = DateTime.Now;
      
            txtmotivo.ItemIndex = 0;
            Update = false;
        }

    
        //CARGAR COLUMNAS GRILLA
        private void fnColumnasGrilla()
        {
            viewAusentismo.Columns[0].Visible = false;
            viewAusentismo.Columns[1].Caption = "Descripcion";
            viewAusentismo.Columns[1].Width = 100;

            viewAusentismo.Columns[2].Caption = "Motivo";
            viewAusentismo.Columns[2].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            viewAusentismo.Columns[2].DisplayFormat.FormatString = "motivo";
            viewAusentismo.Columns[2].DisplayFormat.Format = new FormatCustom();
            viewAusentismo.Columns[2].AppearanceCell.FontStyleDelta = FontStyle.Bold;
            viewAusentismo.Columns[2].Width = 50;

            viewAusentismo.Columns[3].Caption = "Inicio";
            viewAusentismo.Columns[3].AppearanceCell.FontStyleDelta = FontStyle.Bold;
            viewAusentismo.Columns[3].Width = 50;

            //viewAusentismo.Columns[4].Caption = "Fecha Aplicacion";
            viewAusentismo.Columns[4].Visible = false;
            //viewAusentismo.Columns[5].AppearanceCell.FontStyleDelta = FontStyle.Bold;

            viewAusentismo.Columns[5].Caption = "Dias";
            viewAusentismo.Columns[5].Width = 20;

            viewAusentismo.Columns[6].Caption = "Termino";
            viewAusentismo.Columns[6].AppearanceCell.FontStyleDelta = FontStyle.Bold;
            viewAusentismo.Columns[6].Width = 50;

            //viewAusentismo.Columns[7].Caption = "Fin Aplicacion";
            viewAusentismo.Columns[7].Visible = false;

            viewAusentismo.Columns[8].Visible = false;
        }

        //CARGAR CAMPOS
        private void fnCargarCampos(int? pos = -1)
        {
            if (viewAusentismo.RowCount > 0)
            {               
                if (pos != -1) viewAusentismo.FocusedRowHandle = (int)pos;

                DateTime FechaFinRegistro = DateTime.Now.Date;                

                //PARA ACTUALIZAR DEJAR EN TRUE
                Update = true;

                btnExcel.Enabled = true;
                btnImpresionRapida.Enabled = true;
                btnImprimir.Enabled = true;
                btnPdf.Enabled = true;

                //CARGAMOS CAMPOS               
                txtdesc.Text = viewAusentismo.GetFocusedDataRow()["descripcion"].ToString();
                txtdias.Text = viewAusentismo.GetFocusedDataRow()["numdias"].ToString();
                txtmotivo.EditValue = int.Parse(viewAusentismo.GetFocusedDataRow()["motivo"].ToString());
                dtAplicacion.EditValue = viewAusentismo.GetFocusedDataRow()["fechaAplic"];
                dtEvento.EditValue = viewAusentismo.GetFocusedDataRow()["fechaEvento"];
                dtFin.EditValue = viewAusentismo.GetFocusedDataRow()["fecFin"];
                dtfinApli.EditValue = viewAusentismo.GetFocusedDataRow()["fecFinApli"];         

                FechaFinRegistro = Convert.ToDateTime(viewAusentismo.GetFocusedDataRow()["fecFin"]);

                //SELECCIONAMOS FECHA EN EL CALENDARIO
                calendarioAusentismo.EditValue = dtEvento.EditValue;

                //SI LAS FECHAS SON MENORES AL PERIODO ABIERTO DESHABILITAMOS BOTONES
                if (FechaFinRegistro < fnSistema.PrimerDiaMes(Calculo.PeriodoObservado))
                {
                    txtdesc.ReadOnly = true;
                    txtdias.ReadOnly = true;
                    txtmotivo.ReadOnly = true;
                    dtAplicacion.ReadOnly = true;
                    dtEvento.ReadOnly = true;
                    dtFin.ReadOnly = true;
                    dtfinApli.ReadOnly = true;
                    cbRebaja.ReadOnly = true;
                    btnGuardar.Enabled = false;
                    btnEliminar.Enabled = false;
                }
                else
                {
                    //SI CORRESPONDE AL PERIODO EN EVALAUCION DEJAMOS QUE SE PUEDA EDITAR
                    txtdesc.ReadOnly = false;
                    txtdias.ReadOnly = false;
                    txtmotivo.ReadOnly = false;
                    dtAplicacion.ReadOnly = false;
                    dtEvento.ReadOnly = false;
                    dtFin.ReadOnly = false;
                    dtfinApli.ReadOnly = false;
                    cbRebaja.ReadOnly = false;
                    btnGuardar.Enabled = true;
                    btnEliminar.Enabled = true;
                }

                cbRebaja.Checked = Convert.ToBoolean(viewAusentismo.GetFocusedDataRow()["rebSueldo"]);                
            }
            else
            { 
                fnLimpiarCampos();
            }
        }

        //TRAER Y GUADAR EN LISTA TODOS LOS CONTRATOS ENCONTRADOS EN TABLA AUSENTISMO
        private List<string> fnListaContratos()
        {
            string sql = "SELECT DISTINCT contrato FROM ausentismo";
            SqlCommand cmd;
            SqlDataReader rd;
            List<string> contratos = new List<string>();

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //RECORREMOS Y LLENAMOS LISTA CON LOS CONTRATOS
                            while (rd.Read())
                            {
                                contratos.Add((string)rd["contrato"]);
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
                    XtraMessageBox.Show("No hay conexion con la base de datos", "Base de datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
            //RETORNAMOS LISTADO CON LOS CONTRATOS ENCONTRADOS
            return contratos;
        }

        //CARGAR DATOS EN CAMPO RECORRIENDO LISTADO CON CONTRATOS ENCONTRADOS
        private void fnCargarContrato(string pContrato)
        {
            string sql = "SELECT contrato, fechaevento, fechaAplic, numdias, fecFin, fecFinApli, motivo, " +
                "descripcion, rebSueldo FROM ausentismo WHERE contrato = @pContrato";
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

                        //EJECUTAR CONSULTA Y CARGAR DATOS
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                Update = true;

                                //guardar el nombre del contrato en variable contrato
                                Contrato = (string)rd["contrato"];
                                txtdesc.Text = (string)rd["descripcion"];
                                txtdias.Text = ((int)rd["numdias"]).ToString();
                                txtmotivo.EditValue = (int)rd["motivo"];
                                dtAplicacion.DateTime = (DateTime)rd["fechaAplic"];
                                dtEvento.DateTime = (DateTime)rd["fechaevento"];
                                dtFin.DateTime = (DateTime)rd["fecFin"];
                                dtfinApli.DateTime = (DateTime)rd["fecFinApli"];
                                cbRebaja.Checked = (bool)rd["rebSueldo"];
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
                    XtraMessageBox.Show("No hay conexion con la base de datos", "Base de datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            //MANEJO TECLA TAB
            if (keyData == Keys.Tab)
            {
                if (txtdias.ContainsFocus)
                {
                    //if (txtdias.Text == "")
                    //{
                    //    lblError.Visible = true;
                    //    lblError.Text = "Por favor ingresar la cantidad de dias";
                    //    return false;
                    //}
                    //else
                    //{
                    //    lblError.Visible = false;
                    //    //CALCULAR LA FECHA FIN Y FINAPLICACION
                    //    if (txtdias.Text == "0")
                    //    {
                    //        lblError.Visible = true;
                    //        lblError.Text = "Por favor ingrese un dia valido";
                    //        return false;
                    //    }
                    //    else
                    //    {
                    //        //VALIDAR DECIMAL VALIDO
                    //        bool decimalValido = fnDecimal(txtdias.Text);
                    //        if (decimalValido == false)
                    //        {
                    //            lblError.Visible = true;
                    //            lblError.Text = "Numero dia no valido";
                    //            return false;
                    //        }
                    //        else
                    //        {
                    //            lblError.Visible = false;
                    //            //FECHA TERMINO REAL
                    //            fnFechaFinal(txtdias, dtFin, dtEvento);
                    //            //FECHA TERMINO APLICACION
                    //            fnFechaFinal(txtdias, dtfinApli, dtAplicacion);
                    //        }
                    //    }
                    //}

                    ValidaDias();
                }
                else if (txtdesc.ContainsFocus)
                {
                    if (txtdesc.Text == "")
                    {
                        lblError.Visible = true;
                        lblError.Text = "Por favor ingrese una descripcion";
                        return false;
                    }
                    else
                    {
                        lblError.Visible = false;
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

        //COMBOBOX MOTIVO
        /*
         * 1 --> LICENCIA MEDICA
         * 2 --> LICENCIA MATERNAL
         * 3 --> PERMISO
         * 4 --> INASISTENCIA
         * 5 --> ACCIDENTE
         */
        private void fnComboMotivo(LookUpEdit pComboBox, bool? ocultarKey = false)
        {
            List<datoCombobox> lista = new List<datoCombobox>();

            lista.Add(new datoCombobox() { KeyInfo = 1, descInfo = "LICENCIA MEDICA" });
            lista.Add(new datoCombobox() { KeyInfo = 2, descInfo = "LICENCIA MATERNAL" });
            lista.Add(new datoCombobox() { KeyInfo = 3, descInfo = "PERMISO" });
            lista.Add(new datoCombobox() { KeyInfo = 4, descInfo = "INASISTENCIA" });
            lista.Add(new datoCombobox() { KeyInfo = 5, descInfo = "ACCIDENTE" });

            //PROPIEDADES COMBOBOX
            pComboBox.Properties.DataSource = lista.ToList();
            pComboBox.Properties.ValueMember = "KeyInfo";
            pComboBox.Properties.DisplayMember = "descInfo";

            pComboBox.Properties.PopulateColumns();
            //SI OCULTAR KEY ES TRUE NO SE DEBE MOSTRAR LA COLUMNA KEY
            if (ocultarKey == true)
            {
                pComboBox.Properties.Columns[0].Visible = false;
            }
            pComboBox.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
            pComboBox.Properties.AutoSearchColumnIndex = 1;
            pComboBox.Properties.ShowHeader = false;
        }

        //VERIFICAR SI REGISTRO YA EXISTE
        //PARAMETROS DE ENTRADA: CONTRATO Y FECHA EVENTO
        private bool fnRegistroExiste(string pContrato, DateTime pEvento, DateTime pAplic, DateTime pfecAplic, DateTime pFin)
        {
            string sql = "SELECT contrato, fechaEvento FROM ausentismo WHERE contrato=@pContrato AND fechaEvento=@pEvento AND periodoanterior=@periodo";

            string sql2 = "SELECT fechaevento FROM ausentismo WHERE contrato=@pContrato " +
                "AND (fechaEvento=@pEvento OR fechaAplic = @pAplic) AND " +
                "(fecFinApli = @pfecAplic OR fecFin = @pfecFin ) AND periodoanterior=@periodo ";

            string sql3 = "SELECT * FROM ausentismo WHERE contrato='c001122' " +
                        " AND((fechaEvento >= @pEvento AND fechaEvento <= @pfecFin)" + /* fecha evento  <--> fecha fin*/
                        " OR(fechaAplic >= @pAplic AND fechaAplic <= @pfecFin)" + /*fecha aplicacion <--> fecha fin*/
                        " OR(fechaEvento >= @pEvento AND fechaEvento <= @pfecAplic) " + /*fecha evento <--> fecha fin aplicacion*/
                        " OR(fechaAplic >= @pAplic AND fechaAplic <= @pfecAplic)) " +/*fecha aplicacion <--> fecha fin aplicacion*/
                        " AND periodoanterior = 201710 ";
            SqlCommand cmd;
            SqlDataReader rd;

            bool existe = false;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql2, fnSistema.sqlConn))
                    {
                        //PARAMETROS 
                        cmd.Parameters.Add(new SqlParameter("@pContrato", pContrato));
                        cmd.Parameters.Add(new SqlParameter("@pEvento", pEvento));
                        cmd.Parameters.Add(new SqlParameter("@periodo", periodo));
                        cmd.Parameters.Add(new SqlParameter("@pAplic", pAplic));
                        cmd.Parameters.Add(new SqlParameter("@pfecAplic", pfecAplic));
                        cmd.Parameters.Add(new SqlParameter("@pfecFin", pFin));

                        //EJECUTAR CONSULTA 
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //SI ENCUENTRA REGISTROS ES PORQUE EL REGISTRO YA EXISTE
                            //RETORNAMOS TRUE
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
                    XtraMessageBox.Show("No hay conexio con la base de datos", "Base de datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
            return existe;
        }

        //VERIFICAR SI SE HA CAMBIADO ALGUN VALOR DE LAS CAJAS DE TEXTO
        private bool fnCambioCampos(string pContrato, DateTime pEvento, int periodo)
        {
            string sql = "SELECT contrato, fechaEvento, fechaAplic, numdias, fecFin, fecFinApli, motivo, " +
                "descripcion, rebSueldo FROM ausentismo WHERE contrato=@pContrato AND fechaEvento=@pEvento AND periodoanterior=@periodo";
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
                        cmd.Parameters.Add(new SqlParameter("@pEvento", pEvento));
                        cmd.Parameters.Add(new SqlParameter("@periodo", periodo));

                        //EJECUTAR CONSULTA
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //RECORREMOS Y COMPARAMOS
                            while (rd.Read())
                            {

                                if (((DateTime)rd["fechaEvento"]).ToString() != dtEvento.EditValue.ToString()) return true;
                                if (((DateTime)rd["fechaAplic"]).ToString() != dtAplicacion.EditValue.ToString()) return true;
                                if (((int)rd["numdias"]).ToString() != txtdias.Text) return true;
                                if (((DateTime)rd["fecFin"]).ToString() != dtFin.EditValue.ToString()) return true;
                                if (((DateTime)rd["fecFinApli"]).ToString() != dtfinApli.EditValue.ToString()) return true;
                                if (rd["descripcion"].ToString() != txtdesc.Text) return true;
                                if (((bool)rd["rebSueldo"]).ToString() != cbRebaja.Checked.ToString()) return true;
                            }
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
            return false;
        }

        //VER SI SE MODIFICO LA FECHA DE EVENTO (SOLO PARA UPDATE)
        private bool fnModificaEvento(DateTime pEventoOriginal, DateTime pEventoNuevo)
        {
            if (pEventoOriginal != pEventoNuevo)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //VALIDAR QUE LAS FECHAS INGRESADAS NO SEAN MENOR A LA FECHA DE INGRESO DEL TRABAJADOR
        //SI RETORNA TRUE ES UNA FECHA VALIDA
        private bool fnValidarFechas(DateTime fechaIngreso, DateEdit pFecha)
        {
            int compara = 0;
            compara = DateTime.Compare(DateTime.Parse(pFecha.EditValue.ToString()), fechaIngreso);

            if (compara < 0)
            {
                //FECHA ES ANTERIOR A FECHA DE INGRESO NO VALIDO
                return false;
            }
            else if (compara == 0)
            {
                //SON FECHAS IGUALES
                return true;
            }
            else if (compara > 0)
            {
                //FECHA EVENTO ES POSTERIOR A FECHA DE INGRESO (VALIDO!)
                return true;
            }
            return false;
        }

        //VALIDAR QUE LA FECHA DE APLICACION NO SEA MENOR A LA FECHA DE EVENTO
        private bool fnValFechaAplicacion(DateTime pEvento, DateTime pAplicacion)
        {
            int compara = 0;

            compara = DateTime.Compare(pAplicacion, pEvento);
            if (compara < 0)
            {
                //FECHA APLICACION ES MENOR A LA FECHA DE EVENTO (INCORRECTO)
                return false;
            }
            else if (compara == 0)
            {
                //LAS FECHAS SON IGUALES
                return true;
            }
            else if (compara > 0)
            {
                //LA FECHA DE APLICACION ES MAYOR A  LA FECHA DE EVENTO (CORRECTO)
                return true;
            }
            return false;
        }

        //VALIDAR QUE LAS FECHAS DE FIN APLICACION Y FECHA FIN NO SEAN MENORES A LAS FECHAS DE APLICACION
        private bool fnfechaMenor(DateTime pAplicacion, DateEdit pFecha)
        {
            int compara = 0;
            compara = DateTime.Compare(DateTime.Parse(pFecha.EditValue.ToString()), pAplicacion);
            if (compara < 0)
            {
                //FECHA FIN ES MENOR A LA FECHA DE APLICACION
                return false;
            }
            else if (compara == 0)
            {
                //LA FECHA FIN ES IGUAL A LA FECHA APLICACION
                return true;
            }
            else if (compara > 0)
            {
                //LA FECHA FIN ES MAYOR A LA FECHA DE APLICACION
                return true;
            }
            return false;
        }

        //VALIDAR QUE LA FECHA DE EVENTO NO SEA MAYOR A LA FECHA DE FIN
        private bool fnValEventoFin(DateTime pEvento, DateTime pFin)
        {
            int compara = 0;
            compara = DateTime.Compare(pEvento, pFin);
            if (compara > 0 )
            {
                //LA FECHA DE EVENTO ES SUPERIOR A LA FECHA DE FIN
                return false;
            }
            else if (compara == 0)
            {
                //LA FECHA DE EVENTO ES IGUAL A LA FECHA DE FIN
                return true;
            }
            else if (compara < 0)
            {
                //LA FECHA DE EVENTO ES INFERIOR A LA FECHA DE FIN
                return true;
            }

            return false;
        }

        //VALIDAR QUE LA FECHA DE FIN NO SEA INFERIOR A LA FECHA DE EVENTO
        private bool fnValFinEvento(DateTime pFin, DateTime pEvento)
        {
            int compara = 0;
            compara = DateTime.Compare(pFin, pEvento);
            if (compara > 0)
            {
                //LA FECHA DE FIN ES SUPERIOR A LA FECHA DE EVENTO
                return true;
            }
            else if (compara == 0)
            {
                //LA FECHA DE EVENTO Y LA FECHA DE FIN SON IGUALES
                return true;
            }
            else if (compara < 0)
            {
                //LA FECHA DE FIN ES MENOR A LA FECHA DE EVENTO
                return false;
            }

            return false;
        }

        //OBTENER LA CANTIDAD DE DIAS ENTRE FECHA EVENTO Y FECHA FIN
        private decimal fnNumeroDias(DateEdit pEvento, DateEdit pFin)
        {
            TimeSpan dias = DateTime.Parse(pFin.EditValue.ToString()).Subtract(DateTime.Parse(pEvento.EditValue.ToString()));
            decimal numero = 0;
            numero = (dias.Days) + 1;
            return numero;
        }

        //DEJAR SELECCIONADO CHECKBOX DE ACUERDO A MOTIVO SELECCIONADO
        private void fnTieneRebaja(int pId)
        {
            string sql = "SELECT licencia, ausencia FROM motivo WHERE id=@pId";
            SqlCommand cmd;
            SqlDataReader rd;
            bool ausencia = false, licencia = false;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pId", pId));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                ausencia = (bool)rd["ausencia"];
                                licencia = (bool)rd["licencia"];
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

            if (licencia == true && ausencia == false)
            {
                cbRebaja.Checked = true;
            }
            else if (ausencia == true && licencia == false)
            {
                cbRebaja.Checked = true;
            }
            else if (ausencia == false && licencia == false)
            {
                cbRebaja.Checked = false;
            }
        }

        //OBTENER LA CANTIDAD DE DIAS QUE TIENE DE LICENCIA EN EL PERIODO DE REGISTRO
        private double[] fnDiasPeriodo(DateTime pEvento, DateTime pFin, double NumDias)
        {
            //List<DateTime> fechas = new List<DateTime>();
            //fechas = fechasContractuales(Contrato);
            double[] Numeros = new double[2];

            int yearAnterior = 0, yearSiguiente = 0, mesAnterior = 0, mesSiguiente = 0;
            double cantidadDias = 0;
            //Meses y años desde fechas.
            yearAnterior = pEvento.Year;
            yearSiguiente = pFin.Year;
            mesAnterior = pEvento.Month;
            mesSiguiente = pFin.Month;           

            //OBTENER LA CANTIDAD DE DIAS QUE TIENE ESE PERIODO(ULTIMO DIA)
            int num = fnSistema.DiasMes(pEvento.Month, pEvento.Year);

            //FECHA LIMITE (Ultimo dia del mes)
            DateTime Limite = DateTime.Parse(yearAnterior + "-" + mesAnterior + "-" + num);

            if (mesAnterior != mesSiguiente)
            {
                //SON DISTINTOS MESES
                //DEBEMOS CALCULAR LA CANTIDAD DE DIAS QUE HAY HASTA EL ULTIMO DIA DEL MES
                 TimeSpan dias = Limite.Subtract(pEvento);
                cantidadDias = (dias.Days) + 1;
                Numeros[0] = cantidadDias;
                //GUARDAR EN POS1 LA CANTIDAD DE DIAS RESTANTES
                Numeros[1] = NumDias - cantidadDias;
            }
            else
            {
                //SON MESES IGUALES
                //SI ES EL MISMO MES SOLO CALCULAMOS LA DIFERENCIA DE DIAS ENTRE LA FECHA DE EVENTO Y LA FECHA DE FIN
                if (pFin.Month == pEvento.Month)
                {
                    Numeros[0] = NumDias;
                    Numeros[1] = 0;
                }
                else
                {
                    TimeSpan dias = pFin.Subtract(pEvento);
                    cantidadDias = (dias.Days) + 1;
                    Numeros[0] = cantidadDias;
                    Numeros[1] = NumDias - cantidadDias;
                }               
            }

            //RETORNAR ARREGLO CON NUMEROS
            //POS 0 REPRESENTA LA CANTIDAD DE DIAS PARA EL PERIODO ACTUAL
            //POS 1 REPRESENTA LA CANTIDADAD RESTANTE DE DIAS(DIAS PARA EL PERIODO SIGUIENTE)
            return Numeros;
        }

        //OBTENER LA FECHA FIN Y FIN APLICACION DE ACUERDO AL NUMERO DE DIAS
        private void fnFechaFinal(TextEdit pNumDias, DateEdit pFinal, DateEdit pEvento)
        {
            double d = double.Parse(pNumDias.Text);
            string desc = d.ToString();
            string desc1 = "", desc2 = "";

            string[] spl = new string[] {};
            if (desc.Contains(","))
                spl = desc.Split(',');

            DateTime evento = DateTime.Parse(pEvento.EditValue.ToString());
            //evento = evento.AddDays(d - 1);            
            if (d < 1)
            {
                pFinal.EditValue = pEvento.EditValue;
            }
            else
            {
                //Si hay decimales
                if (spl.Length > 0)
                {
                    desc1 = spl[0];
                    desc2 = spl[1];
                    if (fnSistema.IsNumeric(desc1) && fnSistema.IsNumeric(desc2))
                    {
                        //Si la parte decimal es mayor o igual 1 agregamos u dia adicional
                        if (Convert.ToInt32(desc2) >= 1)
                        {
                            pFinal.EditValue = evento.AddDays((Convert.ToInt32(desc1)) - 1);
                            pFinal.EditValue = Convert.ToDateTime(pFinal.EditValue).AddDays(1);
                        }
                        else
                        {
                            pFinal.EditValue = evento.AddDays((Convert.ToInt32(desc1)) - 1);
                        }
                    }
                    else
                    {
                        pFinal.EditValue = evento.AddDays(d - 1);
                    }
                }
                else
                {
                    pFinal.EditValue = evento.AddDays(d - 1);
                }              
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
                if (subcadena[1].Length > 1) return false;
                if (subcadena[1].Length == 0) return false;
                if (subcadena[0].Length == 0) return false;

                return true;
            }
            else
            {
                //SI NO TIENE COMAS SOLO ES UN NUMERO
                if (cadena.Length > 0)
                {
                    string c = cadena[0].ToString();
                    if (c == "0")
                        return false;
                }

                return true;
            }           
        }

        //VALIDAR QUE LA CANTIDAD DE DIAS DE AUSENTISMO NO SUPERE LOS 30
        private bool fnValidaMaxDias(TextEdit pNumDias)
        {
            decimal num = Convert.ToDecimal(pNumDias.Text);

            if (num > 30)
                return false;
            else
                return true;
        }  

        //VALIDAR QUE LAS FECHAS NO SE CRUCEN CON FECHAS YA INGRESADAS (O QUE ESTEN DENTRO DEL RANGO DE OTRAS)
        private bool ValidarFechasIngreso(string contrato, DateTime Fecha)
        {
            string sql = "SELECT fechaEvento, fechaAplic, fecFin, fecFinApli FROM AUSENTISMO " +
                "WHERE contrato=@pcontrato";

            DateTime evento = DateTime.Now.Date;
            DateTime aplicacion = DateTime.Now.Date;
            DateTime fecFin = DateTime.Now.Date;
            DateTime fecFinApli = DateTime.Now.Date;
            int NumeroRegistros = 0, c = 0;

            //OBTENER LA CANTIDAD DE REGISTROS QUE TIENE 
            NumeroRegistros = CantidadRegistros(contrato);
            SqlCommand cmd;
            SqlDataReader rd;

            //CASO NUEVO INSERT

                try
                {
                    if (fnSistema.ConectarSQLServer())
                    {
                        using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@pcontrato", contrato));
                            // cmd.Parameters.Add(new SqlParameter("@periodo", periodo));
                            //cmd.Parameters.Add(new SqlParameter("@fecha", FechaEvento));

                            rd = cmd.ExecuteReader();
                            if (rd.HasRows)
                            {
                                while (rd.Read())
                                {
                                    //ITERAMOS
                                    evento = (DateTime)rd["fechaEvento"];
                                    aplicacion = (DateTime)rd["fechaAplic"];
                                    fecFin = (DateTime)rd["fecFin"];
                                    fecFinApli = (DateTime)rd["fecFinApli"];

                                    //COMPARA RANGO [EVENTO <--> FECHA FIN]
                                    while (evento <= fecFin)
                                    {
                                        //RECORREMOS TODO EL RANGO DE FECHAS ENTRE LA FECHA DE EVENTO Y LA FECHA FIN
                                        if (Fecha == evento)
                                        {
                                            //SI ALGUNA FECHA COINCIDE CON LA FECHA NO ES VALIDO
                                            return false;
                                        }

                                        //AGREGAMOS UN DIA A LA FECHA
                                        evento = evento.AddDays(1);                                            
                                    }

                                    //COMPARA RANGO[EVENTO <--> FECHA FIN APLICACION]
                                    while (evento <= fecFinApli)
                                    {
                                        //RECORREMOS TODO EL RANGO DE FECHAS
                                        if (Fecha == evento)
                                        {
                                            //SI ALGUNA FECHA COINCIDE CON LA FECHA NO ES VALIDO
                                            return false;
                                        }

                                        //AGREGAMOS UN DIA A LA FECHA
                                        evento = evento.AddDays(1);
                                    }

                                    //COMPARA RANGO [APLICACION <--> FECHA FIN]
                                    while (aplicacion <= fecFin)
                                    {
                                        if (Fecha == aplicacion)
                                        {
                                            //NO VALIDO
                                            return false;
                                        }

                                        //AUMENTAMOS UN DIA A LA FECHA
                                        aplicacion = aplicacion.AddDays(1);
                                    }

                                    //COMPARA RANGO [APLICACION <--> FECHA FIN APLICACION]
                                    while (aplicacion <= fecFinApli)
                                    {
                                        if (Fecha == aplicacion)
                                        {
                                            //NO VALIDO
                                            return false;
                                        }
                                        //AUMENTAMOS UN DIA A LA FECHA
                                        aplicacion = aplicacion.AddDays(1);
                                    }

                                    c++;
                                }
                            }

                        }
                        //LIBERAR 
                        cmd.Dispose();
                        rd.Close();
                        fnSistema.sqlConn.Close();
                    }
                }
                catch (SqlException ex)
                {
                    XtraMessageBox.Show(ex.Message);

                }

                //SI LA VARIABLE C TIENE LA MISMA CANTIDAD QUE NUMERO DE REGISTROS ES VALIDO
                if (c == NumeroRegistros)
                {
                    return true;
                }
                else
                {
                    return false;
                }
        }

        //VALIDAR QUE EL RANGO DE FECHAS QUE SE INTENTA INGRESAR NO EXISTE EN BD (CUALQUIER FECHA DEL RANGO)
        private bool ValidarRangoIngreso(string contrato, DateTime FechaInicioRango, DateTime FechaTerminoRango)
        {
            string sql = "SELECT fechaEvento, fechaAplic, fecFin, fecFinApli FROM AUSENTISMO " +
               "WHERE contrato=@pcontrato";

            DateTime evento = DateTime.Now.Date;
            DateTime aplicacion = DateTime.Now.Date;
            DateTime fecFin = DateTime.Now.Date;
            DateTime fecFinApli = DateTime.Now.Date;
            DateTime AuxFechaInicio = DateTime.Now.Date;
            int NumeroRegistros = 0, c = 0;

            //OBTENER LA CANTIDAD DE REGISTROS QUE TIENE 
            NumeroRegistros = CantidadRegistros(contrato);
            SqlCommand cmd;
            SqlDataReader rd;

            //CASO NUEVO INSERT
            AuxFechaInicio = FechaInicioRango;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pcontrato", contrato));
                        // cmd.Parameters.Add(new SqlParameter("@periodo", periodo));
                        //cmd.Parameters.Add(new SqlParameter("@fecha", FechaEvento));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //ITERAMOS
                                evento = (DateTime)rd["fechaEvento"];
                                aplicacion = (DateTime)rd["fechaAplic"];
                                fecFin = (DateTime)rd["fecFin"];
                                fecFinApli = (DateTime)rd["fecFinApli"];

                                //RECORREMOS TODO EL RANGO DE FECHAS QUE HAY ENTRE LA FECHA DE VENTO Y AL FECHA FIN
                                //NECESITAMOS RECORRER TAMBIEN TODAS LAS FECHAS QUE HAY ENTRE EL RANGO QUE SE INTENTA
                                //INGRESAR SI HAY ALGUNA COINCIDENCIA NO ES VALIDO!
                                while (FechaInicioRango <= FechaTerminoRango)
                                {                              
                                    while (evento <= fecFin)
                                    {
                                       
                                        //RECORREMOS TODO EL RANGO DE FECHAS ENTRE LA FECHA DE EVENTO Y LA FECHA FIN
                                        if (FechaInicioRango == evento)
                                        {
                                            //SI ALGUNA FECHA COINCIDE CON LA FECHA NO ES VALIDO
                                            return false;
                                        }                                       
                                        //AGREGAMOS UN DIA A LA FECHA
                                        evento = evento.AddDays(1);                                 
                                    }

                                    //COMPARA RANGO[EVENTO <--> FECHA FIN APLICACION]
                                    while (evento <= fecFinApli)
                                    {
                                        //RECORREMOS TODO EL RANGO DE FECHAS
                                        if (FechaInicioRango == evento)
                                        {
                                            //SI ALGUNA FECHA COINCIDE CON LA FECHA NO ES VALIDO
                                            return false;
                                        }

                                        //AGREGAMOS UN DIA A LA FECHA
                                        evento = evento.AddDays(1);
                                    }

                                    //COMPARA RANGO [APLICACION <--> FECHA FIN]
                                    while (aplicacion <= fecFin)
                                    {
                                        if (FechaInicioRango == aplicacion)
                                        {
                                            //NO VALIDO
                                            return false;
                                        }

                                        //AUMENTAMOS UN DIA A LA FECHA
                                        aplicacion = aplicacion.AddDays(1);
                                    }

                                    //COMPARA RANGO [APLICACION <--> FECHA FIN APLICACION]
                                    while (aplicacion <= fecFinApli)
                                    {
                                        if (FechaInicioRango == aplicacion)
                                        {
                                            //NO VALIDO
                                            return false;
                                        }
                                        //AUMENTAMOS UN DIA A LA FECHA
                                        aplicacion = aplicacion.AddDays(1);
                                    }

                                    //AUMENTAMOS EN UN DIA LA FECHA DE INICIO DE RANGO...
                                    FechaInicioRango = FechaInicioRango.AddDays(1);

                                    //RESET FECHAS
                                    evento = (DateTime)rd["fechaEvento"];
                                    aplicacion = (DateTime)rd["fechaAplic"];
                                    fecFin = (DateTime)rd["fecFin"];
                                    fecFinApli = (DateTime)rd["fecFinApli"];
                                }

                                //RESET FECHA INICIO RANGO
                                FechaInicioRango = AuxFechaInicio;

                                c++;
                            }
                        }
                    }
                    //LIBERAR 
                    cmd.Dispose();
                    rd.Close();
                    fnSistema.sqlConn.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);

            }

            //SI LA VARIABLE C TIENE LA MISMA CANTIDAD QUE NUMERO DE REGISTROS ES VALIDO
            if (c == NumeroRegistros)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //VALIDAR FECHAS INGRESO PARA EL CASO DE UPDATE
        private bool ValidarFechasIngresoUpdate(string contrato, DateTime DateEventoNueva, 
            DateTime DateAplicNueva, DateTime DateFinNueva, DateTime DateFinAplicNueva, DateTime EventoAntigua, 
            DateTime ApliAntigua, DateTime finAntigua, DateTime finApliAntigua)
        {
            string sql = "SELECT * FROM ausentismo WHERE contrato=@pcontrato " +
            " AND((fechaEvento >= @pEvento AND fechaEvento <= @pDateFin)" + /* fecha evento  <--> fecha fin*/
            " OR(fechaAplic >= @pDateAplic AND fechaAplic <= @pDateFin)" + /*fecha aplicacion <--> fecha fin*/
            " OR(fechaEvento >= @pEvento AND fechaEvento <= @pDateFinAplic)" + /*fecha evento <--> fecha fin aplicacion*/
            " OR(fechaAplic >= @pDateAplic AND fechaAplic <= @pDateFinAplic)" +/*fecha aplicacion <--> fecha fin aplicacion*/
            " OR(fecFin >= @pEvento AND fecFin <= @pDateFin) " + /*fecha fin [con fecha evento] <--> fecha fin*/
            " OR(fecFinApli >= @pEvento AND fecFinApli <= @pDateFinAplic))" + /*fecha fin apli [con fecha evento] <--> fecha fin*/
            " AND fechaEvento <> @pFecha1 AND fechaAplic <> @pFecha2 AND fecFin <> @pFecha3 " +
            " AND fecFinApli <> @pFecha4";            

            //string p = string.Format("SELECT * FROM ausentismo WHERE contrato='{0}' " +
            //" AND((fechaEvento >= '{1}' AND fechaEvento <= '{2}')" + /* fecha evento  <--> fecha fin*/
            //" OR(fechaAplic >= '{3}' AND fechaAplic <= '{4}')" + /*fecha aplicacion <--> fecha fin*/
            //" OR(fechaEvento >= '{5}' AND fechaEvento <= '{6}')" + /*fecha evento <--> fecha fin aplicacion*/
            //" OR(fechaAplic >= '{7}' AND fechaAplic <= '{8}'))" +/*fecha aplicacion <--> fecha fin aplicacion*/
           // " OR(fecFin >= '{9}' AND fecFin <= '{10}') " + /*fecha fin [con fecha evento] <--> fecha fin*/
           // " OR(fecFinApli >= '{11}' AND fecFinApli <= '{12}')" + /*fecha fin apli [con fecha evento] <--> fecha fin*/
            //" AND fechaEvento <> '{13}' AND fechaAplic <> '{14}' AND fecFin <> '{15}' " +
           // " AND fecFinApli <> '{16}'" +
           // " AND periodoanterior = {17}", contrato, DateEventoNueva, DateFinNueva, DateAplicNueva, DateFinNueva,
           // DateEventoNueva, DateFinAplicNueva, DateAplicNueva, DateFinAplicNueva, DateEventoNueva, DateFinNueva, 
           // DateEventoNueva, DateFinNueva, EventoAntigua, ApliAntigua, finAntigua, finApliAntigua);
               
            //XtraMessageBox.Show(p);

            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pcontrato", contrato));
                        cmd.Parameters.Add(new SqlParameter("@pEvento", DateEventoNueva));
                        cmd.Parameters.Add(new SqlParameter("@pDateFin", DateFinNueva));
                        cmd.Parameters.Add(new SqlParameter("@pDateAplic", DateAplicNueva));
                        cmd.Parameters.Add(new SqlParameter("@pDateFinAplic",  DateFinAplicNueva));
                        //cmd.Parameters.Add(new SqlParameter("@periodo", Periodo));
                        cmd.Parameters.Add(new SqlParameter("@pFecha1", EventoAntigua));
                        cmd.Parameters.Add(new SqlParameter("@pFecha2", finAntigua));
                        cmd.Parameters.Add(new SqlParameter("@pFecha3", finAntigua));
                        cmd.Parameters.Add(new SqlParameter("@pFecha4", finApliAntigua));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //SI ENCUENTRA REGISTROS NO ES VALIDA LA FECHA
                           /* while (rd.Read())
                            {
                               // XtraMessageBox.Show(((DateTime)rd["fechaEvento"]).ToString());
                               // XtraMessageBox.Show(((DateTime)rd["fecfinapli"]).ToString());
                            }*/
                            return false;
                        }
                        else
                        {
                            return true;
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

        //VALIDAR FECHAS INGRESO PARA EL CASO DE UPDATE (V2)
        private bool ValidarFechasUpdate(string contrato, DateTime EventoAntigua, DateTime FechaInicioRango, 
            DateTime FechaTerminoRango)
        {
            string sql = "SELECT fechaEvento, fechaAplic, fecFin, fecFinApli FROM AUSENTISMO " +
           "WHERE (contrato=@pcontrato AND fechaEvento<>@Fecha)";

            DateTime evento = DateTime.Now.Date;
            DateTime aplicacion = DateTime.Now.Date;
            DateTime fecFin = DateTime.Now.Date;
            DateTime fecFinApli = DateTime.Now.Date;
            DateTime AuxFechaInicio = DateTime.Now.Date;
            int NumeroRegistros = 0, c = 0;

            //OBTENER LA CANTIDAD DE REGISTROS QUE TIENE 
            NumeroRegistros = (CantidadRegistros(contrato))-1;
         
            SqlCommand cmd;
            SqlDataReader rd;

            //CASO NUEVO INSERT
            AuxFechaInicio = FechaInicioRango;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pcontrato", contrato));
                        cmd.Parameters.Add(new SqlParameter("@Fecha", EventoAntigua));
                        // cmd.Parameters.Add(new SqlParameter("@periodo", periodo));
                        //cmd.Parameters.Add(new SqlParameter("@fecha", FechaEvento));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //ITERAMOS
                                evento = (DateTime)rd["fechaEvento"];
                                aplicacion = (DateTime)rd["fechaAplic"];
                                fecFin = (DateTime)rd["fecFin"];
                                fecFinApli = (DateTime)rd["fecFinApli"];
                               
                                //RECORREMOS TODO EL RANGO DE FECHAS QUE HAY ENTRE LA FECHA DE VENTO Y AL FECHA FIN
                                //NECESITAMOS RECORRER TAMBIEN TODAS LAS FECHAS QUE HAY ENTRE EL RANGO QUE SE INTENTA
                                //INGRESAR SI HAY ALGUNA COINCIDENCIA NO ES VALIDO!
                                while (FechaInicioRango <= FechaTerminoRango)
                                {
                                    while (evento <= fecFin)
                                    {                                        
                                        //RECORREMOS TODO EL RANGO DE FECHAS ENTRE LA FECHA DE EVENTO Y LA FECHA FIN
                                        if (FechaInicioRango == evento)
                                        {
                                            //SI ALGUNA FECHA COINCIDE CON LA FECHA NO ES VALIDO                                 
                                            return false;
                                        }
                                        //AGREGAMOS UN DIA A LA FECHA
                                        evento = evento.AddDays(1);
                                    }

                                    //COMPARA RANGO[EVENTO <--> FECHA FIN APLICACION]
                                    while (evento <= fecFinApli)
                                    {
                                        //RECORREMOS TODO EL RANGO DE FECHAS
                                        if (FechaInicioRango == evento)
                                        {
                                            //SI ALGUNA FECHA COINCIDE CON LA FECHA NO ES VALIDO
                                            return false;
                                        }

                                        //AGREGAMOS UN DIA A LA FECHA
                                        evento = evento.AddDays(1);
                                    }

                                    //COMPARA RANGO [APLICACION <--> FECHA FIN]
                                    while (aplicacion <= fecFin)
                                    {
                                        if (FechaInicioRango == aplicacion)
                                        {
                                            //NO VALIDO
                                            return false;
                                        }

                                        //AUMENTAMOS UN DIA A LA FECHA
                                        aplicacion = aplicacion.AddDays(1);
                                    }

                                    //COMPARA RANGO [APLICACION <--> FECHA FIN APLICACION]
                                    while (aplicacion <= fecFinApli)
                                    {
                                        if (FechaInicioRango == aplicacion)
                                        {
                                            //NO VALIDO
                                            return false;
                                        }
                                        //AUMENTAMOS UN DIA A LA FECHA
                                        aplicacion = aplicacion.AddDays(1);
                                    }

                                    //AUMENTAMOS EN UN DIA LA FECHA DE INICIO DE RANGO...
                                    FechaInicioRango = FechaInicioRango.AddDays(1);

                                    //RESET FECHAS
                                    evento = (DateTime)rd["fechaEvento"];
                                    aplicacion = (DateTime)rd["fechaAplic"];
                                    fecFin = (DateTime)rd["fecFin"];
                                    fecFinApli = (DateTime)rd["fecFinApli"];                                   
                                }

                                //RESET A LA FECHA INICIAL 
                                FechaInicioRango = AuxFechaInicio;
                                
                                c++;
                            }
                        }
                    }
                    //LIBERAR 
                    cmd.Dispose();
                    rd.Close();
                    fnSistema.sqlConn.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            //SI LA VARIABLE C TIENE LA MISMA CANTIDAD QUE NUMERO DE REGISTROS ES VALIDO
            if (c == NumeroRegistros)
            {
                return true;
            }
            else
            {
                return false;
            }           
        }

        //OBTENER LA CANTIDAD DE REGISTROS DE AUSENTISMOS PARA UN DETERMINADO CONTRATO Y PERIODO
        private int CantidadRegistros(string contrato)
        {
            int registros = 0;
            string sql = "SELECT count(*) as numero FROM AUSENTISMO WHERE contrato=@pcontrato";
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pcontrato", contrato));
                       // cmd.Parameters.Add(new SqlParameter("@periodo", periodo));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                registros = (int)rd["numero"];
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
            return registros;
        }

        //VALIDAR QUE LA FECHAS DE EVENTO O FECHAS DE APLICACION ESTEN DENTRO DEL PERIODO EN CURSO
        private bool FechaDentroPeriodo(DateTime fecha, int periodo, int tipoContrato, string contrato)
        {
            //OBTENER EL ULTIMO DIA DEL MES
            DateTime ultimoDia = DateTime.Now.Date;
            ultimoDia = fnSistema.UltimoDiaMes(periodo);
            
            //FECHAS CONTRACTUALES EMPLEADO
            //List<DateTime> listado = new List<DateTime>();
            //listado = fechasContractuales(contrato);

            DateTime ingreso = DateTime.Now.Date;
            DateTime termino = DateTime.Now.Date;

            if (tipoContrato == 0)
            {
                //INDEFINIDO
                //SI ES INDEFINIDO ASUMIMOS COMO FINAL EL ULTIMO DIA DEL MES EN CURSO
                if (fecha <= ultimoDia)
                {
                    //VALIDO
                    return true;
                }
                else
                {
                    //NO VALIDO
                    return false;
                }
            }
            else if (tipoContrato == 1)
            {
                //FIJO
                //OBTENER LAS FECHAS CONTRACTUALES DEL EMPLEADO
                ingreso = Trabajador.Ingreso;
                termino = Trabajador.Salida;
                
                //MESES IGUALES Y AÑOS IGUALES
                if ((ingreso.Month == termino.Month) && (ingreso.Year == termino.Year))
                {
                    //CONSIDERAMOS COMO TERMINO LA FECHA DE TERMINO ORIGINAL
                    ultimoDia = Trabajador.Salida;
                  
                    if (fecha <= ultimoDia)
                    {
                        //VALIDO
                        return true;
                    }
                    else
                    {
                        //NO VALIDO
                        return false;
                    }
                }
                else if ((ingreso.Month == termino.Month) && (ingreso.Year != termino.Year))
                {
                    //MESES IGUALES AÑOS DISTINTOS
                    //CONSIDERAMOS COMO FECHA TOPE EL ULTIMO DIA DEL MES EN CURSO
                    if (fecha <= ultimoDia)
                    {
                        //VALIDO
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if ((ingreso.Month != termino.Month))
                {
                    
                    //MESES DISTINTOS
                    //CONSIDERAMOS COMO TERMINO ULTIMO DIA DEL MES EN CURSO
                    if (fecha <= ultimoDia)
                    {
                        //VALIDO
                        return true;
                    }
                    else
                    {
                        //NO VALIDO
                        return false;
                    }
                }
               
            }
            else if (tipoContrato == 2)
            {
                //FAENA
                ingreso = Trabajador.Ingreso;
                termino = Trabajador.Salida;

                //MESES IGUALES Y AÑOS IGUALES
                if ((ingreso.Month == termino.Month) && (ingreso.Year == termino.Year))
                {
                    //CONSIDERAMOS COMO TERMINO LA FECHA DE TERMINO ORIGINAL
                    ultimoDia = Trabajador.Salida;
                    if (fecha <= ultimoDia)
                    {
                        //VALIDO
                        return true;
                    }
                    else
                    {
                        //NO VALIDO
                        return false;
                    }
                }
                else if ((ingreso.Month == termino.Month) && (ingreso.Year != termino.Year))
                {
                    //MESES IGUALES AÑOS DISTINTOS
                    //CONSIDERAMOS COMO FECHA TOPE EL ULTIMO DIA DEL MES EN CURSO
                    if (fecha <= ultimoDia)
                    {
                        //VALIDO
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if ((ingreso.Month != termino.Month))
                {
                    //MESES DISTINTOS
                    //CONSIDERAMOS COMO TERMINO ULTIMO DIA DEL MES EN CURSO
                    if (fecha <= ultimoDia)
                    {
                        //VALIDO
                        return true;
                    }
                    else
                    {
                        //NO VALIDO
                        return false;
                    }
                }
            }

            return false;
        }

        //VERIFICAR QUE LAS FECHAS QUE SE INTENTAN MODIFICAR NO EXISTAN EN BD
        private bool DiferentDate(DateTime evento, DateTime aplicacion, DateTime fin, DateTime finaplicacion, string contrato, int periodo)
        {
            bool valida = false;
            string sql = "SELECT * FROM AUSENTISMO WHERE contrato=@pcontrato AND periodoAnterior=@periodo AND " +
                "fechaEvento=@evento AND fechaAplic=@fechaAplic AND fecFin=@fin AND fecfinapli=@finapli";
            SqlCommand cmd;
            SqlDataReader rd;
            
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pcontrato", contrato));
                        cmd.Parameters.Add(new SqlParameter("@periodo", periodo));
                        cmd.Parameters.Add(new SqlParameter("@evento", evento));
                        cmd.Parameters.Add(new SqlParameter("@fechaAplic", aplicacion));
                        cmd.Parameters.Add(new SqlParameter("@fin", fin));
                        cmd.Parameters.Add(new SqlParameter("@finapli", finaplicacion));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //SI RETORNA FILAS ES PORQUE EL REGISTRO EXISTE
                            valida = false;
                        }
                        else
                        {
                            valida = true;
                        }
                    }
                    //LIBERAR RECURSOS
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                    rd.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return valida;
        }

        //IMPRIME DOCUMENTO
        private void ImprimeDocument(string contrato, bool? ImpresionRapida = false, bool? GeneraPdf = false, bool editar = false)
        {
            if (viewAusentismo.RowCount>0 || editar)
            {
                string sql = "SELECT contrato, motivo.nombre as descripcion, fechaEvento as 'Fecha Inicio', fecFin as 'Fecha Termino', numdias as dias " +
                             "FROM ausentismo INNER JOIN motivo ON motivo.id = ausentismo.motivo " +
                             "WHERE contrato = @contrato ";
                double totalAus = 0, totalLic = 0, totalPermi = 0; 
                SqlCommand cmd;
                DataSet ds = new DataSet();
                SqlDataAdapter ad = new SqlDataAdapter();
                try
                {
                    if (fnSistema.ConectarSQLServer())
                    {
                        using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@contrato", contrato));

                            ad.SelectCommand = cmd;
                            ad.Fill(ds, "ausentismos");

                            fnSistema.sqlConn.Close();
                            ad.Dispose();
                            cmd.Dispose();

                            if (ds.Tables[0].Rows.Count > 0 || editar)
                            {
                                //PASAMOS DATASET COMO DATASOURCE A REPORTE
                                //rptAusentismo aus = new rptAusentismo();
                                //Reporte externo
                                ReportesExternos.rptAusentismo aus = new ReportesExternos.rptAusentismo();
                                aus.LoadLayoutFromXml(Path.Combine(fnSistema.RutaCarpetaReportesExterno, "rptAusentismo.repx"));
                                aus.DataSource = ds.Tables[0];
                                aus.DataMember = "ausentismos";

                                Empresa emp = new Empresa();
                                emp.SetInfo();

                                //PARAMETROS
                                aus.Parameters["nombre"].Value = Trabajador.NombreCompleto;
                                aus.Parameters["rutTrabajador"].Value = fnSistema.fFormatearRut2(Trabajador.Rut);
                                aus.Parameters["empresa"].Value = emp.Razon;
                                aus.Parameters["rutEmpresa"].Value = fnSistema.fFormatearRut2(emp.Rut);

                                foreach (DevExpress.XtraReports.Parameters.Parameter parametro in aus.Parameters)
                                    parametro.Visible = false;                                

                                totalLic = DiasAusentismo(contrato);
                                totalAus = DiasAusentismo(contrato, 1);
                                totalPermi = DiasAusentismo(contrato, 2);

                                aus.Parameters["totallic"].Value = totalLic;
                                aus.Parameters["totalaus"].Value = totalAus;                               
                                aus.Parameters["totalPermi"].Value = totalPermi;                               

                                Documento d = new Documento("", 0);

                                if (editar)
                                {
                                    splashScreenManager1.ShowWaitForm();
                                    //Se le pasa el waitform para que se cierre una vez cargado
                                    DiseñadorReportes.MostrarEditorLimitado(aus, "rptAusentismo.repx", splashScreenManager1);
                                }
                                else
                                {
                                    if ((bool)ImpresionRapida)
                                        d.PrintDocument(aus);
                                    else if ((bool)GeneraPdf)
                                        d.ExportToPdf(aus, $"Ausentismo_{contrato}");
                                    else
                                        d.ShowDocument(aus);
                                }

                                
                            }
                            else
                            {
                                XtraMessageBox.Show("No hay registros");
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    XtraMessageBox.Show(ex.Message);
                }
            }
            else
            {
                XtraMessageBox.Show("No se encontraron ausentismos", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }            
        }

        //OBTENER LA CANTIDAD DE DIAS QUE SON LICENCIAS (TOTAL)
        private double DiasAusentismo(string contrato, int? tipo = 0)
        {
            string sqlLicencias = "SELECT ISNULL(SUM(numdias), 0) as dias " +
                                  "FROM ausentismo " +
                                  "INNER JOIN motivo ON motivo.id = ausentismo.motivo " +
                                  "where contrato = @contrato AND motivo.licencia = 1";

            string sqlAusencias = "SELECT ISNULL(SUM(numdias), 0) as dias  " +
                                  "FROM ausentismo " +
                                  "INNER JOIN motivo ON motivo.id = ausentismo.motivo " +
                                  "where contrato = @contrato AND motivo.ausencia = 1";

            string sqlPermisos = "SELECT ISNULL(SUM(numdias), 0) as dias  " +
                                  "FROM ausentismo " +
                                  "INNER JOIN motivo ON motivo.id = ausentismo.motivo " +
                                  "where contrato = @contrato AND motivo.ausencia = 0 AND motivo.licencia = 0";

            string usa = "";
            double total = 0;

            if (tipo == 0)
                usa = sqlLicencias;
            if (tipo == 1)
                usa = sqlAusencias;
            if (tipo == 2)
                usa = sqlPermisos;

            SqlCommand cmd;

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(usa, fnSistema.sqlConn))
                    {
                        //PARAMETRO
                        cmd.Parameters.Add(new SqlParameter("@contrato", contrato));

                        total = Convert.ToDouble(cmd.ExecuteScalar());                       
                    }
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return total;
        }

        //MANIPULAR DATATABLE DESDE GRIDCONTROL
        private DataTable CrearDatatableFinal(DevExpress.XtraGrid.GridControl Grid)
        {
            DataTable tabla = new DataTable();
            DataTable tablaFinal = new DataTable();
            if (Grid.DataSource != null)
            {
                string contrato = "", motivo = "", descripcion = "", rebsueldo = "";
                DateTime fechaEvento = DateTime.Now.Date;
                DateTime fechaAplic = DateTime.Now.Date;
                DateTime fechaFin = DateTime.Now.Date;
                DateTime fechaFinApli = DateTime.Now.Date;
                decimal numdias = 0;
                int periodo = 0;

                tabla = (DataTable)Grid.DataSource;

                //tablaFinal.Columns.Add("periodo", typeof(int));
                tablaFinal.Columns.Add("contrato", typeof(string));
                tablaFinal.Columns.Add("motivo", typeof(string));
                tablaFinal.Columns.Add("descripcion", typeof(string));
                tablaFinal.Columns.Add("fechaEvento", typeof(DateTime));
                tablaFinal.Columns.Add("fechaAplic", typeof(DateTime));
                tablaFinal.Columns.Add("fecFin", typeof(DateTime));
                tablaFinal.Columns.Add("fecFinApli", typeof(DateTime));
                tablaFinal.Columns.Add("rebSueldo", typeof(string));
                tablaFinal.Columns.Add("numdias", typeof(decimal));

                //RECORREMOS DATATABLE
                if (tabla.Rows.Count > 0)
                {
                    try
                    {
                        for (int fila = 0; fila < tabla.Rows.Count; fila++)
                        {
                            for (int columna = 0; columna < tabla.Columns.Count; columna++)
                            {
                                if (tabla.Columns[columna].ToString() == "motivo")
                                    motivo = GetMotivo(Convert.ToInt32(tabla.Rows[fila][columna]));
                                //if (tabla.Columns[columna].ToString() == "periodoanterior")
                                //    periodo = (int)tabla.Rows[fila][columna];
                                if (tabla.Columns[columna].ToString() == "contrato")
                                    contrato = (string)tabla.Rows[fila][columna];
                                if (tabla.Columns[columna].ToString() == "descripcion")
                                    descripcion = (string)tabla.Rows[fila][columna];
                                if (tabla.Columns[columna].ToString() == "fechaEvento")
                                    fechaEvento = Convert.ToDateTime(tabla.Rows[fila][columna]);
                                if (tabla.Columns[columna].ToString() == "fechaAplic")
                                    fechaAplic = Convert.ToDateTime(tabla.Rows[fila][columna]);
                                if (tabla.Columns[columna].ToString() == "fecFin")
                                    fechaFin = Convert.ToDateTime(tabla.Rows[fila][columna]);
                                if (tabla.Columns[columna].ToString() == "fecFinApli")
                                    fechaFinApli = Convert.ToDateTime(tabla.Rows[fila][columna]);
                                if (tabla.Columns[columna].ToString() == "rebSueldo")
                                    rebsueldo = GetRebSueldo((bool)tabla.Rows[fila][columna]);
                                if (tabla.Columns[columna].ToString() == "numdias")
                                    numdias = Convert.ToDecimal(tabla.Rows[fila][columna]);
                            }

                            //AGREGAMOS FILA
                            tablaFinal.Rows.Add(contrato, motivo, descripcion, fechaEvento, fechaAplic,
                                fechaFin, fechaFinApli, rebsueldo, numdias);

                        }

                        //RETORNAMOS LA TABLA
                        return tablaFinal;
                    }
                    catch (Exception ex)
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
            else
            {
                return null;
            }
        }

        //MANIPULAR MOTIVO
        private string GetMotivo(int Id)
        {
            string data = "";
            if (Id == 1)
            {
                data = "LICENCIA MEDICA";
            }
            else if (Id == 2)
            {
                data = "LICENCIA MATERNAL";
            }
            else if (Id == 3)
            {
                data = "PERMISO";
            }
            else if (Id == 4)
            {
                data = "INASISTENCIA";
            }
            else if (Id == 4)
            {
                data = "ACCIDENTE";
            }

            return data;
        }

        //MANIPULAR REBAJA SUELDO
        private string GetRebSueldo(bool value)
        {
            string data = "";
            if (value == true)
            {
                data = "SI";
            }
            else
            {
                data = "NO";
            }
            return data;
        }

        //VERIFICAR SI HAY CAMBIOS ANTES DE CERRAR
        private bool Cambiossinguardar(DateTime pEvento, DateTime pFin, string pContrato, int periodo)
        {
            string sql = "SELECT fechaevento, fechaaplic, fecfin, fecfinapli, motivo, descripcion, " +
                "rebsueldo, numdias FROM ausentismo WHERE fechaevento=@pEvento AND fecfin=@pFin AND " +
                "contrato=@pContrato AND periodoAnterior=@pPeriodo";
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pEvento", pEvento));
                        cmd.Parameters.Add(new SqlParameter("@pFin", pFin));
                        cmd.Parameters.Add(new SqlParameter("@pContrato", pContrato));
                        cmd.Parameters.Add(new SqlParameter("@pPeriodo", periodo));

                        //...
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                string descripcion = rd["descripcion"].ToString();
                                //COMPARAMOS
                                if (dtEvento.DateTime != Convert.ToDateTime(rd["fechaevento"])) return true;
                                if (dtAplicacion.DateTime != Convert.ToDateTime(rd["fechaaplic"])) return true;
                                if (dtFin.DateTime != Convert.ToDateTime(rd["fecfin"])) return true;
                                if (dtfinApli.DateTime != Convert.ToDateTime(rd["fecfinapli"])) return true;
                                if (Convert.ToInt32(txtmotivo.EditValue) != Convert.ToInt32(rd["motivo"])) return true;
                                if (txtdesc.Text.ToLower() != rd["descripcion"].ToString().ToLower()) return true;
                                if (cbRebaja.Checked != (bool)rd["rebsueldo"]) return true;
                                if (Convert.ToDouble(txtdias.Text) != Convert.ToDouble(rd["numdias"])) return true;
                            }
                        }

                        cmd.Dispose();
                        rd.Close();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            
            }
            return false;
        }

        //LISTADO CON TODAS LAS FECHAS DE ACUERDO A CONTRATO
        private List<DateTime> ListadoAusentismos(string pContrato)
        {
            List<DateTime> Listado = new List<DateTime>();
            string sql = "SELECT fechaevento, fecfinapli FROM ausentismo WHERE contrato=@pContrato";
            SqlCommand cmd;
            SqlDataReader rd;
            DateTime Evento = DateTime.Now.Date;
            DateTime Finaliza = DateTime.Now.Date;

            try
            {
                if(fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pContrato", pContrato));
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                Evento = Convert.ToDateTime(rd["fechaevento"]);
                                Finaliza = Convert.ToDateTime(rd["fecfinapli"]);

                                while (Evento <= Finaliza)
                                {
                                    //AGREGAMOS A LISTADO
                                    Listado.Add(Evento);
                                    Evento = Evento.AddDays(1);
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

            return Listado;
        }

        //ULITMO FOLIO ASOCIADO A CONTRATO
        private int UltimoFolio(string pContrato)
        {
            int number = 0;
            string sql = "SELECT ISNULL(MAX(folio), 0) FROM ausentismo WHERE contrato=@pContrato";
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
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@pContrato", pContrato));

                            number = Convert.ToInt32(cmd.ExecuteScalar());
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

        //GENERA COMPROBANTE AUSENTISMO
        private XtraReport GeneraComprobante(string pContrato, DateTime pFechaInicio, DateTime pFechaTermino)
        {
            string sql = "SELECT DISTINCT ausentismo.contrato, CONCAT(trabajador.nombre, ' ', apepaterno, ' ', apematerno, ' ') as nombre, fechaevento,  " +
                         "fecfin, motivo.nombre as motivo, descripcion, rebsueldo, diasAnterior, numdias, folio, trabajador.rut as rutTrabajador " +
                         "FROM ausentismo " +
                         "INNER JOIN trabajador on trabajador.contrato = ausentismo.contrato " +
                         "INNER JOIN motivo ON motivo.id = ausentismo.motivo " +
                         "WHERE ausentismo.contrato = @pContrato and fechaevento = @pInicio AND fecfin = @pFin";

            SqlCommand cmd;
            SqlConnection cn;
            SqlDataAdapter ad = new SqlDataAdapter();
            DataSet ds = new DataSet();
            //rptComprobanteAusentismo repo = new rptComprobanteAusentismo();
            //Reporte externo
            ReportesExternos.rptComprobanteAusentismo repo = new ReportesExternos.rptComprobanteAusentismo();
            repo.LoadLayoutFromXml(Path.Combine(fnSistema.RutaCarpetaReportesExterno, "rptComprobanteAusentismo.repx"));

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
                            cmd.Parameters.Add(new SqlParameter("@pInicio", pFechaInicio));
                            cmd.Parameters.Add(new SqlParameter("@pFin", pFechaTermino));                           

                            ad.SelectCommand = cmd;
                            ad.Fill(ds, "data");
                            ad.Dispose();

                            if (ds.Tables[0].Rows.Count> 0)
                            {                          
                                repo.DataSource = ds.Tables[0];
                                repo.DataMember = "data";                                
                            }

                            Empresa emp = new Empresa();
                            emp.SetInfo();

                            foreach (DevExpress.XtraReports.Parameters.Parameter parametro in repo.Parameters)
                            {
                                parametro.Visible = false;
                            }

                            repo.Parameters["empresa"].Value = emp.Razon;
                            repo.Parameters["rutEmpresa"].Value = fnSistema.fFormatearRut2(emp.Rut);
                            repo.Parameters["imagen"].Value = Imagen.GetLogoFromBd();
                        }
                        cmd.Dispose();
                    }
                }
                //repo.SaveLayoutToXml(Path.Combine(fnSistema.RutaCarpetaReportesExterno, "rptComprobanteAusentismo.repx"));
            }

            catch (SqlException ex)
            {
                //ERROR
            }

            return repo;

        }

        #region "LOG AUSENTISMO"
        //TABLA HASH PARA GUARDAR LOS DATOS DE AUSENTIMOS
        private Hashtable PrecargaAusentismo(string contrato, int periodo, DateTime FechaEvento)
        {
            string sql = "SELECT contrato, fechaEvento, fechaAplic, fecFin, fecFinApli, motivo, descripcion, " +
                "rebsueldo, diasAnterior, periodoSiguiente, diasSiguiente, numdias FROM AUSENTISMO WHERE " +
                "contrato=@contrato AND periodoAnterior=@periodo And fechaEvento =@Fecha";
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
                        cmd.Parameters.Add(new SqlParameter("@periodo", periodo));
                        cmd.Parameters.Add(new SqlParameter("@Fecha", FechaEvento));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //AGREGAMOS DATOS A TABLA HASH
                                datos.Add("contrato", (string)rd["contrato"]);
                                datos.Add("fechaevento", (DateTime)rd["fechaevento"]);
                                datos.Add("fechaaplic", (DateTime)rd["fechaaplic"]);
                                datos.Add("fecfin", (DateTime)rd["fecfin"]);
                                datos.Add("fecfinapli", (DateTime)rd["fecfinapli"]);
                                datos.Add("rebsueldo", (bool)rd["rebsueldo"]);
                                datos.Add("motivo", (int)rd["motivo"]);
                                datos.Add("descripcion", (string)rd["descripcion"]);
                                datos.Add("diasanterior", (decimal)rd["diasanterior"]);
                                datos.Add("numdias", (decimal)rd["numdias"]);
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

        //COMPARA VALORES
        private void ComparaValorAusentismo(Hashtable Datos, DateTime FechaEvento, DateTime FechaAplic, string Contrato, 
            DateTime FecFin, DateTime FechaFinApli, int Motivo, string Descripcion, bool RebSueldo, decimal NumDias)
        {
            if (Datos.Count>0)
            {
                if ((DateTime)Datos["fechaevento"] != FechaEvento)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA FECHA EVENTO CONTRATO " + Contrato, "AUSENTISMO", (DateTime)Datos["fechaevento"]+"", FechaEvento+"", "MODIFICAR");
                    log.Log();
                }
                if ((DateTime)Datos["fechaaplic"] != FechaAplic)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA FECHA APLICACION CONTRATO " + Contrato, "AUSENTISMO", (DateTime)Datos["fechaaplic"] + "", FechaAplic + "", "MODIFICAR");
                    log.Log();
                }
                if ((DateTime)Datos["fecfin"] != FecFin)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA FECHA FIN AUSENTIMO CONTRATO " + Contrato, "AUSENTISMO", (DateTime)Datos["fecfin"] + "", FecFin + "", "MODIFICAR");
                    log.Log();
                }
                if ((DateTime)Datos["fecfinapli"] != FechaFinApli)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA FECHA FIN APLICACION CONTRATO " + Contrato, "AUSENTISMO", (DateTime)Datos["fecfinapli"] + "", FechaFinApli + "", "MODIFICAR");
                    log.Log();
                }
                if ((int)Datos["motivo"] != Motivo)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA MOTIVO CONTRATO " + Motivo, "AUSENTISMO", (int)Datos["motivo"] + "", Motivo +"", "MODIFICAR");
                    log.Log();
                }
                if ((string)Datos["descripcion"] != Descripcion)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA DESCRIPCION CONTRATO " + Contrato, "AUSENTISMO", (string)Datos["descripcion"], Descripcion, "MODIFICAR");
                    log.Log();
                }
                if ((bool)Datos["rebsueldo"] != RebSueldo)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA OPCION REBAJA SUELDO AUSENTISMO CONTRATO " + Contrato, "AUSENTISMO", (bool)Datos["rebsueldo"] + "", RebSueldo + "", "MODIFICAR");
                    log.Log();
                }
                if ((decimal)Datos["numdias"] != NumDias)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA NUMERO DIAS AUSENTISMO CONTRATO " + Contrato, "AUSENTISMO", (decimal)Datos["numdias"] + "", NumDias + "", "MODIFICAR");
                    log.Log();
                }
            }
        }
        #endregion

        #endregion

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

             //SOLO LIMPIAMOS TODOS LOS CAMPOS
             
            if (op.Cancela == false)
            {
                op.Cancela = true;
                op.SetButtonProperties(btnNuevo, 2);
                fnLimpiarCampos();                        
            }
            else
            {
                op.Cancela = false;
                op.SetButtonProperties(btnNuevo, 1);
                fnCargarCampos();                
            }
        }
        private void txtdias_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtdesc_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (char.IsLetter(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (char.IsSeparator(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void gridAusentismo_Click(object sender, EventArgs e)
        {
            if(viewAusentismo.RowCount > 0)
            fnCargarCampos();

            //GUARDAR LA POSICION FILA SELECCIONADA EN GRILLA
            if(viewAusentismo.RowCount > 0)
            PosicionGrid = viewAusentismo.FocusedRowHandle;

            lblError.Visible = false;

            op.Cancela = false;
            op.SetButtonProperties(btnNuevo, 1);

        }

        private void gridAusentismo_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void gridAusentismo_KeyUp(object sender, KeyEventArgs e)
        {
            fnCargarCampos();

            //GUARDAMOS LA POSICION DE LA FILA SELECCIONADA EN GRILLA
            PosicionGrid = viewAusentismo.FocusedRowHandle;

            lblError.Visible = false;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            //NUEVA SESION
            Sesion.NuevaActividad();

            //VER SI EL USUARIO ESTA BLOQUEADO 
            if (User.Bloqueado())
            { XtraMessageBox.Show("No puedes realizar modificaciones", "Información", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (txtdesc.Text == "") { lblError.Visible = true;lblError.Text = "Por favor ingrese una descripcion";txtdesc.Focus();return;}
            if (txtdias.Text == "") { lblError.Visible = true; lblError.Text = "Por favor ingrese la cantidad de dias"; txtdesc.Focus(); return; }
            if (txtmotivo.EditValue.ToString() == "") { lblError.Visible = true; lblError.Text = "Por favor ingrese la cantidad de dias"; txtdesc.Focus(); return; }

            if (Contrato.Length == 0)
            { XtraMessageBox.Show("Numero contrato no válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (Persona.ExisteContrato(Contrato, periodo) == false)
            { XtraMessageBox.Show("Numero de contrato no existe", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            lblError.Visible = false;
            //VALIDAR QUE LA FECHA DE EVENTO NO SEA MENOR A LA FECHA DE INGRESO          

            //VALIDAR FECHAS
            bool FApli = false, fFin = false, finApli = false, FechaEventoValida = false;
            bool ev = false, apl = false;
            bool FechaEventoExiste = false, FechaApliExiste = false;         
           
            //VALIDAR QUE LAS FECHAS TENGA CORRECTO SENTIDO LOGICO
            FechaEventoValida = fnValidarFechas(Trabajador.Ingreso, dtEvento);
            FApli = fnValidarFechas(Trabajador.Ingreso, dtAplicacion);
            fFin = fnValidarFechas(Trabajador.Ingreso, dtFin);
            finApli = fnValidarFechas(Trabajador.Ingreso, dtfinApli);

            if (Update == false)
            {
                //ES INSERT
                lblError.Visible = false;
                //VERIFICAR QUE LAS FECHAS INGRESADAS NO ESTAN EN USO
                DateTime evento = DateTime.Parse(dtEvento.EditValue.ToString());

                //registroExiste = fnRegistroExiste(Contrato, evento, dtAplicacion.DateTime, dtfinApli.DateTime, dtFin.DateTime);
                ev = ValidarFechasIngreso(Contrato, evento);
                apl = ValidarFechasIngreso(Contrato, dtAplicacion.DateTime);

                //VALIDAR LAS FECHAS QUE SE INTENTAN INGRESAR (QUE NO ESTEN YA REGISTRADAS)
                FechaEventoExiste = ValidarRangoIngreso(Contrato, evento, dtFin.DateTime);
                FechaApliExiste = ValidarRangoIngreso(Contrato, dtAplicacion.DateTime, dtFin.DateTime);

                if (ev == false || apl == false || FechaEventoExiste == false || FechaApliExiste == false)
                {
                    //SI EV Y APL RETORNAN FALSE ES PORQUE LAS FECHAS NO SON VALIDAS
                    if (ev == false)
                    {
                        lblError.Visible = true;
                        lblError.Text = "La fecha de evento ya esta en uso";
                    }
                    if (apl == false)
                    {
                        lblError.Visible = true;
                        lblError.Text = "La fecha de aplicacion ya esta en uso";
                    }
                    if (FechaEventoExiste == false)
                    {
                        lblError.Visible = true;
                        lblError.Text = "Por favor verifica que la fecha ingresada no este en uso";
                    }
                    if (FechaApliExiste == false)
                    {
                        lblError.Visible = true;
                        lblError.Text = "Por favor verifica que la fecha ingresada no este en uso";
                    }
                }
                else
                {
                    //VALIDAR QUE LA FECHA DE EVENTO NO SEA MAYOR AL PERIODO EN EVALUACION
                    int periodoEvento = 0;
                    periodoEvento = fnSistema.PeriodoFromDate((DateTime)dtEvento.EditValue);

                    if (periodoEvento > Calculo.PeriodoObservado)
                    {
                        //lblError.Visible = true;
                        //lblError.Text = "Por favor verifica que la fecha no sea mayor al periodo en evaluacion";
                        DialogResult Advertencia = XtraMessageBox.Show("Estás intentado ingresar una fecha superior al periodo en evaluación, ¿Deseas continuar de todas maneras?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (Advertencia == DialogResult.No)
                            return;
                    }

                    lblError.Visible = false;
                    //HACEMOS INSERT                        
                    fnNuevoAusentismo(Contrato, dtEvento, dtAplicacion, txtdias, dtFin, dtfinApli, txtmotivo, txtdesc, cbRebaja);
                }
            }
            else
            {
                //ES UPDATE

                if (viewAusentismo.RowCount == 0)
                { XtraMessageBox.Show("Registro no válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return;}
                  
                DateTime EventoAntiguo = DateTime.Now.Date;
                DateTime ApliAntiguo = DateTime.Now.Date;
                DateTime FinApli = DateTime.Now.Date;
                DateTime Fin = DateTime.Now.Date;

                if (viewAusentismo.RowCount > 0)
                {
                    EventoAntiguo = DateTime.Parse(viewAusentismo.GetFocusedDataRow()["fechaevento"].ToString());
                    ApliAntiguo = DateTime.Parse(viewAusentismo.GetFocusedDataRow()["fechaAplic"].ToString());
                    FinApli = DateTime.Parse(viewAusentismo.GetFocusedDataRow()["fecFinApli"].ToString());
                    Fin = DateTime.Parse(viewAusentismo.GetFocusedDataRow()["fecFin"].ToString());
                }

                //VERIFICAR SI LA FECHA DE EVENTO SE HA MODIFICADO
                bool EventoMod = false, apliMod = false, fecfinMod = false, finApliMod = false;
                EventoMod = fnModificaEvento(EventoAntiguo, DateTime.Parse(dtEvento.EditValue.ToString()));
                apliMod = fnModificaEvento(ApliAntiguo, dtAplicacion.DateTime);
                fecfinMod = fnModificaEvento(Fin, dtFin.DateTime);
                finApliMod = fnModificaEvento(FinApli, dtfinApli.DateTime);

                lblError.Visible = false;

                if (EventoMod || apliMod || fecfinMod || finApliMod)
                {
                    DialogResult pregunta = XtraMessageBox.Show("Estas a punto de modificar registro con fecha " + EventoAntiguo.ToShortDateString() + ", ¿Estas seguro de realizar esta opeción?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (pregunta == DialogResult.Yes)
                    {
                        //VERIFICAMOS QUE REGISTRO NO EXISTA (DUPLA)                        
                        ev = ValidarFechasUpdate(Contrato, EventoAntiguo, (DateTime)dtEvento.EditValue, (DateTime)dtFin.EditValue);

                        if (ev == false)
                        {
                            lblError.Visible = true;
                            lblError.Text = "Verifica que la fecha ingresada no está en uso.";
                        }
                        else
                        {
                            lblError.Visible = false;
                            //PODEMOS MODIFICAR SIN PROBLEMAS
                            fnModificarRegistro(Contrato, dtEvento, dtAplicacion, txtdias, dtFin, dtfinApli, txtmotivo, txtdesc, cbRebaja, EventoAntiguo);
                        }
                    }
                }
                else
                {

                    //SIMPLEMENTE MODIFICAMOS
                    fnModificarRegistro(Contrato, dtEvento, dtAplicacion, txtdias, dtFin, dtfinApli, txtmotivo, txtdesc, cbRebaja, EventoAntiguo);
                }

            }            
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            if (Contrato == "") { lblError.Visible = true; lblError.Text = "Registro no valido";return; }
            if (viewAusentismo.RowCount == 0) { lblError.Visible = true; lblError.Text = "Registro no valido"; return; }

            lblError.Visible = false;
            DateTime Fechaevento = DateTime.Now.Date;
          
            if (viewAusentismo.RowCount > 0)
                Fechaevento = (DateTime)viewAusentismo.GetFocusedDataRow()["fechaevento"];

            DialogResult dialogo = XtraMessageBox.Show("¿Realmente desea eliminar el evento con fecha " + Fechaevento.ToString("dd/MM/yyyy") + "?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogo == DialogResult.Yes)
            {
                if (viewAusentismo.RowCount > 0)
                {
                    string contrato = viewAusentismo.GetFocusedDataRow()["contrato"].ToString();                    
                    DateTime evento = DateTime.Parse(viewAusentismo.GetFocusedDataRow()["fechaEvento"].ToString());
                   
                    fnEliminarRegistro(contrato, evento);  
                }
            }
        }       

        private void txtdias_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                ValidaDias();
            }
        }

        private void ValidaDias()
        {
            if (txtdias.Text == "")
            {
                lblError.Visible = true;
                lblError.Text = "Por favor ingresar la cantidad de dias";
                return;
            }
            else
            {
                lblError.Visible = false;
                if (txtdias.Text == "0")
                {
                    lblError.Visible = true;
                    lblError.Text = "Por favor ingrese un dia valido";
                    return;
                }
                else
                {
                    lblError.Visible = false;
                    //CALCULAR FECHA DE TERMINO DE ACUERDO A NUMERO DE DIAS
                    //VALIDAR DECIMAL VALIDO
                    bool decimalValido = fnDecimal(txtdias.Text);

                    if (decimalValido == false)
                    {
                        lblError.Visible = true;
                        lblError.Text = "Dia no valido";
                        return;
                    }
                    else
                    {
                        lblError.Visible = false;
                        //TERMINO REAL
                        fnFechaFinal(txtdias, dtFin, dtEvento);
                        //TERMINO APLICACION
                        fnFechaFinal(txtdias, dtfinApli, dtAplicacion);

                        if (decimal.Parse(txtdias.Text) < 1)
                            cbRebaja.Checked = false;
                        else
                            cbRebaja.Checked = true;
                    }
                }
            }
        }

        private void txtdesc_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (txtdesc.Text == "")
                {
                    lblError.Visible = true;
                    lblError.Text = "Por favor ingresar una descripcion";
                    return;
                }
                else
                {
                    lblError.Visible = false;
                }
            }
        }

        private void btnDiasTrabajados_Click(object sender, EventArgs e)
        {
            //LLAMAR A LA CLASE REMUNERACION
            if (Contrato != "")
            {
                //DiasEmpleado rem = new DiasEmpleado(Contrato);
                //decimal total = rem.Calculo();               
            }            
        }

        private void dtFin_EditValueChanged(object sender, EventArgs e)
        {
            ValidadtFin();         
        }

        private void ValidadtFin()
        {
            int dias = 0;

            bool fechaValida = fnValidarFechas(Trabajador.Ingreso, dtFin);
            bool menor = false;
            if (fechaValida == false)
            {
                lblError.Visible = true;
                lblError.Text = "La fecha de fin no puede ser inferior a la fecha de ingreso del trabajador";
                return;
            }
            else
            {
                lblError.Visible = false;
                //VALIDAR QUE LA FECHA FIN NO SEA MENOR DE LA FECHA DE APLICACION
                menor = fnfechaMenor(DateTime.Parse(dtAplicacion.EditValue.ToString()), dtFin);
                if (menor == false)
                {
                    lblError.Visible = true;
                    lblError.Text = "La fecha de fin debe ser mayor a la fecha de aplicacion";
                }
                else
                {
                    lblError.Visible = false;
                    //VALIDAR QUE LA FECHA DE FIN NO SEA INFERIOR A LA FECHA DE EVENTO
                    bool MenorEvento = fnValFinEvento(DateTime.Parse(dtFin.EditValue.ToString()), DateTime.Parse(dtEvento.EditValue.ToString()));
                    if (MenorEvento == false)
                    {
                        lblError.Visible = true;
                        lblError.Text = "La fecha de fin no puede ser inferior a la fecha de evento";
                        return;
                    }
                    else
                    {
                        lblError.Visible = false;
                        //CALCULAR LA CANTIDAD DE DIAS QUE HAY ENTRE LA FECHA DE EVENTO Y LA FECHA DE FIN
                        //dias = fnNumeroDias(dtEvento, dtFin);

                        // txtdias.Text = dias.ToString();
                    }
                }
            }
        }
        private void dtAplicacion_EditValueChanged(object sender, EventArgs e)
        {
            ValidaAplicacion();
        }

        private void ValidaAplicacion()
        {
            //Si retorna false la fecha no es válida.
            bool fechaValida = fnValidarFechas(Trabajador.Ingreso, dtAplicacion);
            bool menor = false;
            if (fechaValida == false)
            {
                lblError.Visible = true;
                lblError.Text = "La fecha de aplicacion no puede ser inferior a la fecha de ingreso del trabajador";
                return;
            }
            else
            {
                lblError.Visible = false;
                //VALIDAR QUE LA FECHA DE APLICACION NO SEA MENOR A LA FECHA DE EVENTO
                menor = fnValFechaAplicacion(DateTime.Parse(dtEvento.EditValue.ToString()), DateTime.Parse(dtAplicacion.EditValue.ToString()));
                if (menor == false)
                {
                    lblError.Visible = true;
                    lblError.Text = "La fecha de aplicacion no puede ser menor a la fecha de evento";
                    return;
                }
                else
                {
                    lblError.Visible = false;
                }
            }
        }

        private void dtEvento_EditValueChanged(object sender, EventArgs e)
        {
            ValidaEvento();
        }

        private void ValidaEvento()
        {
            bool fechaValida = fnValidarFechas(Trabajador.Ingreso, dtEvento);

            //Si es menor a fecha de inicio de contrato
            if (fechaValida == false)
            { lblError.Visible = true; lblError.Text = "Verifica que la fecha ingresada no sea menor a la fecha de inicio de contrato."; return; }

            lblError.Visible = false;

            //Obtenemos la fecha final de acuerdo a fecha de evento
            //fnFechaFinal(txtdias, dtFin, dtEvento);
            //fnFechaFinal(txtdias, dtfinApli, dtEvento);

            dtAplicacion.EditValue = dtEvento.EditValue;
        }

        private void dtfinApli_EditValueChanged(object sender, EventArgs e)
        {
            ValidaFinAplicacion();

            
        }

        private void ValidaFinAplicacion()
        {
            //VERIFICAR QUE LA FECHA DE APLICACION NO SEA MENOR A LA FECHA DE INGRESO DEL TRABAJADOR
            //List<DateTime> fechas = new List<DateTime>();
            //fechas = fechasContractuales(Contrato);

            bool fechaValida = fnValidarFechas(Trabajador.Ingreso, dtfinApli);
            bool Menor = false;
            if (fechaValida == false)
            {
                lblError.Visible = true;
                lblError.Text = "La fecha fin aplicacion no puede ser inferior a la fecha de ingreso del trabajador";
                return;
            }
            else
            {
                lblError.Visible = false;
                //VALIDAR QUE LA FECHA FIN APLICACION NO SEA MENOR A LA FECHA DE APLICACION
                Menor = fnfechaMenor(DateTime.Parse(dtAplicacion.EditValue.ToString()), dtfinApli);
                if (Menor == false)
                {
                    lblError.Visible = true;
                    lblError.Text = "La fecha fin Aplicacion debe ser mayor a la fecha de aplicacion";
                    return;
                }
                else
                {
                    lblError.Visible = false;

                }
            }
        }

        private void txtmotivo_EditValueChanged(object sender, EventArgs e)
        {
            int val =int.Parse(txtmotivo.EditValue.ToString());
            
            //HABILITAR CHECK REBAJASUELDO DE ACUERDO A TIPO DE MOTIVO SELECCIONADO
            fnTieneRebaja(val);
        }

        private void txtdias_EditValueChanged(object sender, EventArgs e)
        {

        }    

        #region "DESHABILITAR CONTEXT MENU"
        //PARA QUE NO APARESCA EL CONTEXT MENU AL HACER CLICK DERECHO EN LA CAJA DE TEXTO
        private void txtdias_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtdesc_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }
        #endregion

        private void dtEvento_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void dtAplicacion_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void dtFin_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void dtfinApli_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtmotivo_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            DateTime evento = DateTime.Now.Date;
            DateTime fin = DateTime.Now.Date;

            if (viewAusentismo.RowCount > 0)
            {
                evento = Convert.ToDateTime(viewAusentismo.GetFocusedDataRow()["fechaevento"]);
                fin = Convert.ToDateTime(viewAusentismo.GetFocusedDataRow()["fecfin"]);
                if (Cambiossinguardar(evento, fin, Contrato, Calculo.PeriodoObservado))
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

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "rptausentismo") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (viewAusentismo.RowCount>0)
            {
                ImprimeDocument(Contrato);
            }
            else
            {
                XtraMessageBox.Show("No hay registros", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            //SI PRESIONARON EL BOTON NUEVO
            if (op.Cancela)
            {
                op.Cancela = false;
                op.SetButtonProperties(btnNuevo, 1);
                if (viewAusentismo.RowCount > 0)
                    fnCargarCampos();
            }

        }

        private void btnImpresionRapida_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "rptausentismo") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (viewAusentismo.RowCount == 0)
            { XtraMessageBox.Show("No se encontraron registros", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            ImprimeDocument(Contrato, true);

            //SI PRESIONARON EL BOTON NUEVO
            if (op.Cancela)
            {
                op.Cancela = false;
                op.SetButtonProperties(btnNuevo, 1);
                if (viewAusentismo.RowCount > 0)
                    fnCargarCampos();
            }
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "rptausentismo") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (viewAusentismo.RowCount > 0 && Contrato != "")
            {
                DataTable tabla = new DataTable();
                tabla = CrearDatatableFinal(gridAusentismo);

                if (tabla == null)
                { XtraMessageBox.Show("No se pudo generar la informacion", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

                string PathFile = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Ausentismos_" + Contrato + ".xlsx";

                if (FileExcel.IsExcelInstalled())
                {
                    if (FileExcel.CrearArchivoExcelConSum(tabla, PathFile, "numdias"))
                    {
                        DialogResult pregunta = XtraMessageBox.Show("Archivo " + PathFile + " generado correctamente, ¿Deseas ver archivo?", "Informacion", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                        if (pregunta == DialogResult.Yes)
                        {
                            FileExcel.AbrirExcel(PathFile);
                        }                        
                    }
                    else
                    {
                        XtraMessageBox.Show("No se pudo generar el documento", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    XtraMessageBox.Show("Parece ser que tu sistema no tiene instalado office", "Informacion Importante", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                //SI PRESIONARON EL BOTON NUEVO
                if (op.Cancela)
                {
                    op.Cancela = false;
                    op.SetButtonProperties(btnNuevo, 1);
                    if(viewAusentismo.RowCount>0)
                        fnCargarCampos();
                }               

            }
            else
            {
                XtraMessageBox.Show("No hay informacion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }     

    

        private void calendarioAusentismo_CustomDrawDayNumberCell(object sender, DevExpress.XtraEditors.Calendar.CustomDrawDayNumberCellEventArgs e)
        {
            //LISTADO DE FECHAS
            List<DateTime> Ausentismos = new List<DateTime>();
            Ausentismos = ListadoAusentismos(Contrato);

            //Color foreColor = e.Style.ForeColor;
            //Color BackColor = e.Style.BackColor;
            //Color BorderColor = e.Style.BorderColor;

            if (Ausentismos.Count > 0)
            {
                foreach (DateTime fecha in Ausentismos)
                {
                    if (e.Date == fecha)
                    {
                        e.Style.ForeColor = Color.White;
                        e.Style.BackColor = Color.Orange;
                        e.Style.BorderColor = Color.Gray;
                    }
                }
            }
            else
            {

            }
        }

        private void cbRebaja_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btnPdf_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "rptausentismo") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (viewAusentismo.RowCount == 0)
            { XtraMessageBox.Show("No hay registros", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if(Persona.ExisteContrato(Contrato, periodo))
                ImprimeDocument(Contrato, false, true);

            //SI PRESIONARON EL BOTON NUEVO
            if (op.Cancela)
            {
                op.Cancela = false;
                op.SetButtonProperties(btnNuevo, 1);
                if (viewAusentismo.RowCount > 0)
                    fnCargarCampos();
            }
        }

        private void viewAusentismo_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            DXPopupMenu menu = e.Menu;

            //CREAMOS UN SUBMENU PARA EL MENU
            DXMenuItem submenu = new DXMenuItem("Ver comprobante", new EventHandler(ShowComprobante_Click));
            DXMenuItem editar = new DXMenuItem("Editar comprobante", new EventHandler(EditarComprobante_Click));
            DXMenuItem PrintMenu = new DXMenuItem("Imprimir comprobante", new EventHandler(ImprimirComprobante_Click));

            //PReferencias de menú
            PrintMenu.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("images/print/print_16x16.png");
            submenu.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("images/actions/show_16x16.png");
            editar.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("images/edit/edit_16x16.png");

            if (menu != null)
            {
                menu.Items.Clear();
                //AGREGAMOS SUBMENU A MENU
                menu.Items.Add(submenu);
                menu.Items.Add(editar);
                menu.Items.Add(PrintMenu);
            }
        }

        private void ImprimirComprobante_Click(object sender, EventArgs e)
        {
            if (viewAusentismo.RowCount > 0)
            {
                //OBTENEMOS LAS FECHAS DESDE LA GRILLA
                DateTime Inicio = Convert.ToDateTime(viewAusentismo.GetFocusedDataRow()["fechaevento"]);
                DateTime Termino = Convert.ToDateTime(viewAusentismo.GetFocusedDataRow()["Fecfin"]);
                XtraReport reporte = new XtraReport();

                if (Inicio != null && Termino != null)
                {
                    reporte = GeneraComprobante(Contrato, Inicio, Termino);

                    if (reporte != null)
                    {
                        //MOSTRAMOS COMPROBANTE
                        ReportPrintTool print = new ReportPrintTool(reporte);
                        print.PrintDialog();
                    }
                    else
                    {
                        XtraMessageBox.Show("No se pudo generar el comprobante", "Comprobante", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }
                }
            }
        }

        private void EditarComprobante_Click(object sender, EventArgs e)
        {
            AbreEditorComprobanteAusentismo();
        }

        private void ShowComprobante_Click(object sender, EventArgs e)
        {
            if (viewAusentismo.RowCount > 0)
            {
                //OBTENEMOS LAS FECHAS DESDE LA GRILLA
                DateTime Inicio = Convert.ToDateTime(viewAusentismo.GetFocusedDataRow()["fechaevento"]);
                DateTime Termino = Convert.ToDateTime(viewAusentismo.GetFocusedDataRow()["Fecfin"]);
                XtraReport reporte = new XtraReport();

                if (Inicio != null && Termino != null)
                {
                    reporte = GeneraComprobante(Contrato, Inicio, Termino);

                    if (reporte != null)
                    {
                        //MOSTRAMOS COMPROBANTE
                        ReportPrintTool print = new ReportPrintTool(reporte);
                        print.ShowPreview();
                    }
                    else
                    {
                        XtraMessageBox.Show("No se pudo generar el comprobante", "Comprobante", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }
                }
            }
        }

        private void txtdias_Leave(object sender, EventArgs e)
        {
            ValidaDias();
        }

        private void dtEvento_Leave(object sender, EventArgs e)
        {
            ValidaEvento();
        }

        private void dtAplicacion_Leave(object sender, EventArgs e)
        {
            ValidaAplicacion();
        }

        private void dtfinApli_Leave(object sender, EventArgs e)
        {
            ValidaFinAplicacion();
        }

        private void dtFin_Leave(object sender, EventArgs e)
        {
            ValidadtFin();
        }

        private void btnEditarReporte_Click(object sender, EventArgs e)
        {
            ImprimeDocument(Contrato, editar:true);
        }

        private void btnEditarComprobante_Click(object sender, EventArgs e)
        {
            AbreEditorComprobanteAusentismo();
        }

        private void AbreEditorComprobanteAusentismo() 
        {
            //OBTENEMOS LAS FECHAS DESDE LA GRILLA
            DateTime Inicio = viewAusentismo.GetFocusedDataRow() == null ? DateTime.Now : Convert.ToDateTime(viewAusentismo.GetFocusedDataRow()["fechaevento"]);
            DateTime Termino = viewAusentismo.GetFocusedDataRow() == null ? DateTime.Now : Convert.ToDateTime(viewAusentismo.GetFocusedDataRow()["Fecfin"]);

            splashScreenManager1.ShowWaitForm();
            ReportesExternos.rptComprobanteAusentismo reporte = (ReportesExternos.rptComprobanteAusentismo)GeneraComprobante(Contrato, Inicio, Termino);
            DiseñadorReportes.MostrarEditorLimitado(reporte, "rptComprobanteAusentismo.repx", splashScreenManager1);
        }
    }
}