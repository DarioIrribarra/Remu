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
    public partial class frmIndiceMensual : DevExpress.XtraEditors.XtraForm
    {
        [DllImport("user32")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32")]
        static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        const int MF_BYCOMMAND = 0;
        const int MF_DISABLED = 2;
        const int SC_CLOSE = 0xF060;

        //VARIABLE BOOLEANA PARA SABER SI ES UPDATE O INSERT
        private bool update = false;

        //PARA BOTON NUEVO
        private bool cancela = false;

        string SqlIndices = "SELECT anomes, uf, utm, ingresominimo, sis, topeAFP, topeSEC, sanna, topeIps, topeMesApv, " +
                    " ufMesAnt, factorimp FROM valoresMes WHERE anomes=@pPeriodo";

        string SqlHistorico = "SELECT anomes, uf, utm, ingresominimo, sis, topeAFP, topeSEC, sanna, topeIps, topeMesApv, " +
                   " ufMesAnt FROM valoresMes ORDER BY anomes DESC";

        //BOTON NUEVO
        Operacion op;

        public frmIndiceMensual()
        {
            InitializeComponent();
        }

        private void frmIndiceMensual_Load(object sender, EventArgs e)
        {
            var sm = GetSystemMenu(Handle, false);
            EnableMenuItem(sm, SC_CLOSE, MF_BYCOMMAND | MF_DISABLED);

            fnDefaultProperties();
            
            //CARGAMOS DATOS EN CAJAS
            fnCargarPeriodoEspecifico(Calculo.PeriodoObservado);

            op = new Operacion();

            //CARGAR GRILLA HISTORICO            
            fnSistema.spllenaGridView(gridIndiceMensual, SqlHistorico);
            fnSistema.spOpcionesGrilla(viewIndiceMensual);           

            if (viewIndiceMensual.RowCount > 0)
            {
                fnColumnasGrilla();

                int filas = 0, x = 0;
                int periodo = 0;
                filas = viewIndiceMensual.RowCount;

                while (x < filas)
                {
                    viewIndiceMensual.FocusedRowHandle = x;
                    periodo = Convert.ToInt32(viewIndiceMensual.GetFocusedDataRow()["anomes"]);
                    if (periodo == Calculo.PeriodoObservado)
                    {
                        break;
                    }
                    x++;
                }
            }
        }

        #region "MANEJO DE DATOS"
        //INGRESO NUEVO INDICE MENSUAL
        /*
         * PARAMETROS DE ENTRADA
         * ANOMES
         * UF
         * UTM
         * INGRESO MINIMO (IMM)
         * SIS
         * TOPE AFP
         * TOPE SEC
         */
        private void fnNuevoMensual(TextEdit pAnomes, TextEdit pUf, TextEdit pUtm, TextEdit pIMM,
            TextEdit psis, TextEdit pTopeAfp, TextEdit pTopeSec, TextEdit pTopeIps, TextEdit pUfAnt, 
            TextEdit pTopeMesApv)
        {
            //SQL INSERT
            string sql = "INSERT INTO valoresMes(anomes, UF, UTM, ingresominimo, sis, topeAFP, topeSEC, " +
                "topeIPS, topeMesApv, ufMesAnt) " +
                "VALUES(@pAnomes, @pUf, @pUtm, @pIMM, @psis, @pTopeAfp, @pTopeSec, @pTopeIps, @pTopeMesApv, " +
                "@pUfMesAnt)";

            SqlCommand cmd;
            int res = 0;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pAnomes", int.Parse(pAnomes.Text)));
                        cmd.Parameters.Add(new SqlParameter("@pUf", decimal.Parse(pUf.Text)));
                        cmd.Parameters.Add(new SqlParameter("@pUtm", decimal.Parse(pUtm.Text)));
                        cmd.Parameters.Add(new SqlParameter("@pIMM", decimal.Parse(pIMM.Text)));
                        cmd.Parameters.Add(new SqlParameter("@psis", decimal.Parse(psis.Text)));
                        cmd.Parameters.Add(new SqlParameter("@pTopeAfp", decimal.Parse(pTopeAfp.Text)));
                        cmd.Parameters.Add(new SqlParameter("@pTopeSec", decimal.Parse(pTopeSec.Text)));
                        cmd.Parameters.Add(new SqlParameter("@pTopeIps", Convert.ToDecimal(pTopeIps.Text)));
                        cmd.Parameters.Add(new SqlParameter("@pTopeMesApv", Convert.ToDecimal(pTopeMesApv.Text)));
                        cmd.Parameters.Add(new SqlParameter("@pUfMesAnt", Convert.ToDecimal(pUfAnt.Text)));

                        //EJECUTAR CONSULTA
                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            XtraMessageBox.Show("Ingreso correcto", "Guardar", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            //GUARDAR EVENTO EN LOG
                            logRegistro log = new logRegistro(User.getUser(), "SE INGRESA NUEVO VALOR MENSUAL", "VALORESMES", "0", pAnomes.Text, "INGRESAR");
                            log.Log();

                            //CARGAR DATO ACTUAL 
                            fnCargarPeriodoEspecifico(int.Parse(pAnomes.Text));
                            //RECARGAR GRILLA
                            string grilla = "SELECT anomes, uf, utm, ingresominimo, sis, topeAFP, topeSEC FROM valoresMes ORDER BY anomes desc";
                            fnSistema.spllenaGridView(gridIndiceMensual, grilla);
                            fnColumnasGrilla();

                        }
                        else
                        {
                            XtraMessageBox.Show("Error el intentar guardar", "Guardar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    //LIBERAMOS RECURSOS
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

        //MODIFICACION NUEVO INDICE
        //PARAMETROS DE ENTRADA LOS MISMO DE ANTERIOR
        //LOS MISMOS DE ARRIBA
        private void fnModificarMensual(TextEdit pAnomes, TextEdit pUf, TextEdit pUtm, TextEdit pIMM,
            TextEdit psis, TextEdit pTopeAfp, TextEdit pTopeSec, TextEdit pSanna, TextEdit pTopeIps, 
            TextEdit pUfAnt, TextEdit pTopeMesApv, TextEdit pFactorImpuesto)
        {
            //UPDATE SQL
            string sql = "UPDATE valoresMes set UF=@pUf, UTM=@pUtm, ingresominimo=@pIMM, " +
                "sis=@pSis, topeAfp=@ptopeAfp, topeSEC=@ptopeSec, sanna=@pSana, topeIPS=@pTopeIps, " +
                "topeMesApv=@pTopeMesApv, ufMesAnt=@pUfAnt, factorimp=@pFactorImpuesto " +
                "WHERE anomes=@pAnomes";

            SqlCommand cmd;
            int res = 0;

            //TABLA HASH PARA LOG REGISTRO
            Hashtable datosIndice = new Hashtable();
            datosIndice = PrecargaIndiceMensual(Convert.ToInt32(pAnomes.Text));

            //VALIDAR VALOR UF
            if (Convert.ToDouble(pUf.Text) > 500000)
            { XtraMessageBox.Show("Por favor verifica el valor de la uf", "Valor uf", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtUf.Focus(); return; }

            if (Convert.ToDouble(pUfAnt.Text) > 500000)
            { XtraMessageBox.Show("Por favor verifica el valor de la uf Mes anterior.", "Valor uf mes Anterior", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtUf.Focus(); return; }

            //VALIDAR VALOR UTM
            if (Convert.ToDouble(pUtm.Text) > 500000)
            { XtraMessageBox.Show("Por favor verifica el valor de utm", "Valor utm", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtUtm.Focus(); return; }

            //VALIDAR VALOR IMM
            if (Convert.ToDouble(pIMM.Text) > 1000000)
            { XtraMessageBox.Show("Por favor verifica el valor del IMM", "Valor IMM", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtIMM.Focus(); return; }

            //VALOR SIS (MAXIMO DOS DIGITOS ENTEROS)
            if (Convert.ToDouble(psis.Text) > 99)
            { XtraMessageBox.Show("Por favor verifica el valor para sis", "Valor sis", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtSis.Focus(); return; }

            //VALOR TOPE AFP MAXIMO 3 ENTEROS
            if (Convert.ToDouble(pTopeAfp.Text) > 999)
            { XtraMessageBox.Show("Por favaor verifica el valor para Tope Afp", "Valor Tope Afp", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtTopeAfp.Focus(); return; }

            //VALOR TOPE SEC MAXIMO 3 ENTEROS
            if (Convert.ToDouble(pTopeSec.Text) > 999)
            { XtraMessageBox.Show("Por favor verifica el valor para Tope Seguro Cesantía", "Seguro de Cesantía", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtTopeSec.Focus(); return; }

            if (Convert.ToDouble(pTopeIps.Text) > 999)
            { XtraMessageBox.Show("Por favor verifica el valor para Tope Ips", "Tope Ips", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtTopeIps.Focus(); return; }

            if (Convert.ToDouble(pTopeMesApv.Text) > 999)
            { XtraMessageBox.Show("Por favor verifica el valor para Tope Mensual Ahorro previsional voluntario", "Tope Apv", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtTopeAfp.Focus(); return; }            

            //VALOR SANNA MAXIMO 2 ENTEROS
            if (Convert.ToDouble(pSanna.Text) > 99)
            { XtraMessageBox.Show("Por favor verifica el valor para cotizacion sanna", "Sanna", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtSanna.Focus(); return; }

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pAnomes", int.Parse(pAnomes.Text)));
                        cmd.Parameters.Add(new SqlParameter("@pUf",decimal.Parse(pUf.Text)));
                        cmd.Parameters.Add(new SqlParameter("@pUtm", decimal.Parse(pUtm.Text)));
                        cmd.Parameters.Add(new SqlParameter("@pIMM", decimal.Parse(pIMM.Text)));
                        cmd.Parameters.Add(new SqlParameter("@psis", decimal.Parse(psis.Text)));
                        cmd.Parameters.Add(new SqlParameter("@pTopeAfp", decimal.Parse(pTopeAfp.Text)));
                        cmd.Parameters.Add(new SqlParameter("@pTopeSec", decimal.Parse(pTopeSec.Text)));
                        cmd.Parameters.Add(new SqlParameter("@pSana", decimal.Parse(pSanna.Text)));
                        cmd.Parameters.Add(new SqlParameter("@pTopeIps", Convert.ToDecimal(pTopeIps.Text)));
                        cmd.Parameters.Add(new SqlParameter("@pTopeMesApv", Convert.ToDecimal(pTopeMesApv.Text)));
                        cmd.Parameters.Add(new SqlParameter("@pUfAnt", Convert.ToDecimal(pUfAnt.Text)));
                        cmd.Parameters.Add(new SqlParameter("@pFactorImpuesto", Convert.ToDouble(pFactorImpuesto.Text)));

                        //EJECUTAMOS CONSULTA
                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            XtraMessageBox.Show("Actualizacion correcta", "Actualizar", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            //SI HAY CAMBIOS GUARDAMOS EN LOG
                            ComparaValorIndice(datosIndice, int.Parse(pAnomes.Text),decimal.Parse(pUf.Text), 
                                decimal.Parse(pUtm.Text), decimal.Parse(pIMM.Text), decimal.Parse(psis.Text), 
                                decimal.Parse(pTopeAfp.Text), decimal.Parse(pTopeSec.Text), decimal.Parse(pSanna.Text), 
                                Convert.ToDecimal(pTopeIps.Text), Convert.ToDecimal(pTopeMesApv.Text),
                                Convert.ToDecimal(pUfAnt.Text), Convert.ToDouble(pFactorImpuesto.Text));
                            //limpiar tabla hash
                            datosIndice.Clear();                           
                            
                            fnSistema.spllenaGridView(gridIndiceMensual, SqlHistorico);
                            fnColumnasGrilla();
                            //VOLVER A ACTUALIZAR LOS DATOS PARA ESTE REGISTRO
                            fnCargarPeriodoEspecifico(int.Parse(pAnomes.Text));
                        }
                        else
                        {
                            XtraMessageBox.Show("Error al intentar Actualizar", "Actualizar", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        //LIMPIAR CAMPOS
        private void fnLimpiarCampos()
        {
            int periodoActual = Calculo.PeriodoEvaluar();
            txtPeriodo.Text = periodoActual.ToString();

            txtIMM.Text = "";
            txtSis.Text = "";
            txtTopeAfp.Text = "";
            txtTopeSec.Text = "";
            txtUf.Text = "";
            txtUtm.Text = "";            

            lblerror.Visible = false;

            update = false;
        }

        //DEFAULT PROPERTIES
        private void fnDefaultProperties()
        {
            txtUf.Properties.MaxLength = 9;
            txtUtm.Properties.MaxLength = 9;
            txtIMM.Properties.MaxLength = 10;
            txtSis.Properties.MaxLength = 4;
            txtTopeAfp.Properties.MaxLength = 5;
            txtTopeSec.Properties.MaxLength = 5;

            separador1.TabStop = false;       
            btnGuardar.TabStop = false;

            int periodoActual = Calculo.PeriodoObservado;
            txtPeriodo.Text = periodoActual.ToString();
            txtPeriodo.ReadOnly = true;

            gridIndiceMensual.TabStop = false;
        }

        //OBTENER EL MAYOR PERIODO REGISTRADO
        //RETORNA EL ULTIMO PERIODO ENCONTRADO (EL MAYOR)
        private int fnMayorPeriodo()
        {
            //STRING SQL
            string sql = "SELECT MAX(anomes) as mayor FROM valoresMes";
            SqlCommand cmd;
            SqlDataReader rd;

            int periodo = 0;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //SI ENCUENTRA REGISTROS GUARDARMOS EL DATO
                            while (rd.Read())
                            {
                                periodo = (int)rd["anomes"];
                            }
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

            return periodo;
        }

        //CARGAR EL PERIODO ACTUAL
        private void fnCargarPeriodoActual()
        {
            //OBTENER EL PERIODO ACTUAL
            int Actual = 0;
            Actual = Calculo.PeriodoObservado;

            //SQL CARGA
            string sql = "SELECT anomes, uf, utm, ingresominimo, sis, topeAFP, topeSEC FROM valoresMes WHERE anomes=@pActual";
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql , fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pActual", Actual));

                        //EJECUTAR CONSULTA
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //SI TIENE REGISTROS RECORREMOS Y CARGAMOS CAMPOS CORRESPONDIENTES
                            while (rd.Read())
                            {
                                txtPeriodo.Text = (string)rd["anomes"];
                                txtUf.Text = (string)rd["uf"];
                                txtUtm.Text = (string)rd["utm"];
                                txtIMM.Text = (string)rd["ingresominimo"];
                                txtSis.Text = (string)rd["sis"];
                                txtTopeAfp.Text = (string)rd["topeAFP"];
                                txtTopeSec.Text = (string)rd["topeSEC"];                                  
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
        }

        //CARGAR PERIODO ESPECIFICO
        //PARAMETRO DE ENTRADA : PERIODO
        private void fnCargarPeriodoEspecifico(int periodo)
        {
            
            //SQL CARGA
            //string sql = "SELECT anomes, uf, utm, ingresominimo, sis, topeAFP, topeSEC, sanna, topeIps, topeMesApv, " +
            //             " ufMesAnt FROM valoresMes WHERE anomes=@pPeriodo";
            SqlCommand cmd;
            SqlDataReader rd;

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(SqlIndices, fnSistema.sqlConn))
                    {
                        //PARAMETROS 
                        cmd.Parameters.Add(new SqlParameter("@pPeriodo", periodo));

                        //EJECUTAR CONSULTA
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //RECORREMOS Y CARGAMOS CAMPOS
                            while (rd.Read())
                            {
                                //DEJAMOS COMO SOLO LECTURA EL PERIODO
                                txtPeriodo.ReadOnly = true;

                                //COLOCAMOS VARIABLE UPDATE EN TRUE
                                update = true;

                                txtPeriodo.Text = ((int)rd["anomes"]).ToString();
                                txtUf.Text = ((decimal)rd["uf"]).ToString();
                                txtUtm.Text = ((decimal)rd["utm"]).ToString();
                                txtIMM.Text = ((decimal)rd["ingresominimo"]).ToString();
                                txtSis.Text = ((decimal)rd["sis"]).ToString();
                                txtTopeAfp.Text = ((decimal)rd["topeAFP"]).ToString();
                                txtTopeSec.Text = ((decimal)rd["topeSEC"]).ToString();
                                txtSanna.Text = ((decimal)rd["sanna"]).ToString();
                                txtTopeIps.Text = Convert.ToDecimal(rd["topeIps"]).ToString();
                                txtMesApv.Text = Convert.ToDecimal(rd["topeMesApv"]).ToString();
                                txtUfAnt.Text = Convert.ToDecimal(rd["ufMesAnt"]).ToString();
                                txtfactorImp.Text = Convert.ToDouble(rd["factorimp"]).ToString();
                            }
                        }
                        else
                        {
                            
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
        }

        //OBTENER TODOS LOS PERIODOS
        //GUARDA TODO LOS PERIODOS EN UNA LISTA
        //retorn lista
        private List<int> fnAllPeriodos()
        {
            //STRING SQL
            string sql = "SELECT anomes FROM valoresMes";
            SqlCommand cmd;
            SqlDataReader rd;

            //LISTA PARA GUARDAR PERIODOS
            List<int> Listado = new List<int>();
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
                                //LLENAMOS LISTA
                                Listado.Add((int)rd["anomes"]);
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

            //RETORNAMOS LISTA
            return Listado;
        }

        //VALIDAR SI EXISTE EL PERIODO INGRESADO
        private bool fnPeriodoExiste(int periodo)
        {
            string sql = "SELECT anomes FROM valoresMes WHERE anomes=@pAnomes";
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
                        cmd.Parameters.Add(new SqlParameter("@pAnomes", periodo));

                        //EJECUTAMOS
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //SI TIENE FILAS ES PORQUE EL PERIODO EXISTE
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

        //COLUMNAS GRILLA
        private void fnColumnasGrilla()
        {
            //SELECT ANOMES, UF, UTM, INGRESOMINIMO, SIS, TOPEAFP, TOPESEC from VALORESMES
            viewIndiceMensual.Columns[0].Caption = "Periodo";
            viewIndiceMensual.Columns[0].DisplayFormat.Format = new FormatCustom();
            viewIndiceMensual.Columns[0].DisplayFormat.FormatString = "periodo";
            viewIndiceMensual.Columns[0].AppearanceCell.FontStyleDelta = FontStyle.Bold;

            viewIndiceMensual.Columns[1].Caption = "UF";

            viewIndiceMensual.Columns[2].Caption = "UTM";

            viewIndiceMensual.Columns[3].Caption = "IMM";

            viewIndiceMensual.Columns[4].Caption = "Sis (%)";

            viewIndiceMensual.Columns[5].Caption = "Tope Afp (UF)";

            viewIndiceMensual.Columns[6].Caption = "Tope Sec (UF)";

            viewIndiceMensual.Columns[7].Caption = "Sanna";

            viewIndiceMensual.Columns[8].Caption = "Tope Ips";

            viewIndiceMensual.Columns[9].Caption = "Tope Mes Apv";
            viewIndiceMensual.Columns[10].Caption = "Uf Mes Anterior";
        }

        //VERIFICAR NUMERO DECIMAL CORRECTO
        private bool fnDecimal(string cadena, int pMax)
        {
            if (cadena.Length == 0) return false;

            //RECORRER CADENA Y VERIFICAR QUE SOLO TENGA UNA COMA
            int coma = 0;
            for (int position = 0; position < cadena.Length; position++)
            {
                if (cadena[position] == ',') coma++;
            }

            //SI TIENE MAS DE UNA COMA NO ES VÁLIDO
            if (coma > 1) return false;

            string[] subcadena = new string[2];
            //SI TIENE UNA COMA SEPARAMOS POR COMA
            if (coma == 1)
            {
                subcadena = cadena.Split(',');

                //SI DESPUES DE LA CADENA TIENE MAS DE DOS DIGITOS NO ES CORRECTO
                if (subcadena[1].Length > 2) return false;
                //SI NO HAY PARTE DECIMAL NO ES VÁLIDA
                if (subcadena[1].Length == 0) return false;
                //SI NO HAY PARTE ENTERA NO ES VÁLIDO
                if (subcadena[0].Length == 0) return false;
                //SI TIENE MAS DE 7 NUMEROS NO ES VALIDO
                if (subcadena[0].Length > pMax) return false;
            }
            else
            {
                //SI NO TIENE COMAS SOLO ES UN NUMERO

                //SI PARTE ENTERA ES MAYOR A 7 ENTEROS NO ES VALIDA
                 if(cadena.Length>pMax)
                    return false;

                return true;
            }
            return true;
        }

        //VERIFICAR DECIMAL PARA SANNA
        //VERIFICAR NUMERO DECIMAL CORRECTO
        private bool fnDecimalSanna(string cadena)
        {
            if (cadena.Length == 0) return false;

            //SI CADENA SOLO ES UNA , NO ES VALIDA
            if (cadena.Length == 0 && cadena == ",")
                return false;

            //RECORRER CADENA Y VERIFICAR QUE SOLO TENGA UNA COMA
            int coma = 0;
            for (int position = 0; position < cadena.Length; position++)
            {
                if (cadena[position] == ',') coma++;
            }

            //SI TIENE MAS DE UNA COMA NO ES VÁLIDO
            if (coma > 1) return false;

            string[] subcadena = new string[2];
            //SI TIENE UNA COMA SEPARAMOS POR COMA
            if (coma == 1)
            {
                subcadena = cadena.Split(',');

                //SI DESPUES DE LA CADENA TIENE MAS DE DOS DIGITOS NO ES CORRECTO
                if (subcadena[1].Length > 3) return false;
                //SI NO HAY PARTE DECIMAL NO ES VÁLIDA
                if (subcadena[1].Length == 0) return false;
                //SI NO HAY PARTE ENTERA NO ES VÁLIDO
                if (subcadena[0].Length == 0) return false;
                //SI TIENE MAS DE 1 NUMEROS NO ES VALIDO EN PARTE ENTERA NO ES VALIDO
                if (subcadena[0].Length > 1) return false;
            }
            else
            {
                //SI NO TIENE COMAS SOLO ES UN NUMERO

                //SI PARTE ENTERA ES MAYOR A 1 ENTEROS NO ES VALIDA
                if (cadena.Length > 1)
                    return false;

                return true;
            }
            return true;
        }

        //PARA MANIPULAR LA TECLA TAB
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Tab)
            {
                if (txtUf.ContainsFocus)
                {
                    if (txtUf.Text == "")
                    {
                        lblerror.Visible = true;
                        lblerror.Text = "Debes ingresar unidad de fomento";
                    }
                    else
                    {
                        lblerror.Visible = false;
                        //VALIDAR LA ESTRUCTURA CORRECTA EN CASO DE DECIMAL
                        bool decimalCorrecto = fnDecimal(txtUf.Text, 7);
                        if (decimalCorrecto == false)
                        {
                            lblerror.Visible = true;
                            lblerror.Text = "Valor no valido como uf";
                        }
                        else
                        {
                            lblerror.Visible = false;
                        }
                    }
                }
                else if (txtUtm.ContainsFocus)
                {
                    if (txtUtm.Text == "")
                    {
                        lblerror.Visible = true;
                        lblerror.Text = "Debes ingresar utm";
                    }
                    else
                    {
                        lblerror.Visible = false;
                        //VALIDAR LA ESTRUCTURA CORRECTA EN CASO DE DECIMAL
                        bool decimalCorrecto = fnDecimal(txtUtm.Text, 7);
                        if (decimalCorrecto == false)
                        {
                            lblerror.Visible = true;
                            lblerror.Text = "Valor no valido como utm";
                        }
                        else
                        {
                            lblerror.Visible = false;
                        }
                    }
                }
                else if (txtIMM.ContainsFocus)
                {
                    if (txtIMM.Text == "")
                    {
                        lblerror.Visible = true;
                        lblerror.Text = "Ingreso minimo mensual no valido";
                    }
                    else
                    {
                        lblerror.Visible = false;
                        //VALIDAR LA ESTRUCTURA CORRECTA EN CASO DE DECIMAL
                        bool decimalCorrecto = fnDecimal(txtIMM.Text, 7);
                        if (decimalCorrecto == false)
                        {
                            lblerror.Visible = true;
                            lblerror.Text = "Ingreso minimo mensual no valido";
                        }
                        else
                        {
                            lblerror.Visible = false;
                        }
                    }
                }
                else if (txtSis.ContainsFocus)
                {
                    if (txtSis.Text == "")
                    {
                        lblerror.Visible = true;
                        lblerror.Text = "Debes llenar el campo sis";
                    }
                    else
                    {
                        lblerror.Visible = false;
                        //VALIDAR LA ESTRUCTURA CORRECTA EN CASO DE DECIMAL
                        bool decimalCorrecto = fnDecimal(txtSis.Text, 2);
                        if (decimalCorrecto == false)
                        {
                            lblerror.Visible = true;
                            lblerror.Text = "Valor sis no valido";
                        }
                        else
                        {
                            lblerror.Visible = false;
                        }
                    }
                }
                else if (txtTopeAfp.ContainsFocus)
                {
                    if (txtTopeAfp.Text == "")
                    {
                        lblerror.Visible = true;
                        lblerror.Text = "Debes llenar el campo Tope Afp";
                    }
                    else
                    {
                        lblerror.Visible = false;
                        //VALIDAR LA ESTRUCTURA CORRECTA EN CASO DE DECIMAL
                        bool decimalCorrecto = fnDecimal(txtTopeAfp.Text, 3);
                        if (decimalCorrecto == false)
                        {
                            lblerror.Visible = true;
                            lblerror.Text = "Valor no vlaido para cmapo Tope Afp";
                        }
                        else
                        {
                            lblerror.Visible = false;
                        }
                    }
                }
                else if (txtTopeSec.ContainsFocus)
                {
                    if (txtTopeSec.Text == "")
                    {
                        lblerror.Visible = true;
                        lblerror.Text = "Debes ingresar Tope Sec";
                    }
                    else
                    {
                        lblerror.Visible = false;
                        //VALIDAR LA ESTRUCTURA CORRECTA EN CASO DE DECIMAL
                        bool decimalCorrecto = fnDecimal(txtTopeSec.Text, 3);
                        if (decimalCorrecto == false)
                        {
                            lblerror.Visible = true;
                            lblerror.Text = "Valor no valido para Tope Sec";
                        }
                        else
                        {
                            lblerror.Visible = false;
                        }
                    }
                }
                else if (txtTopeIps.ContainsFocus)
                {
                    if (txtTopeIps.Text == "")
                    {
                        lblerror.Visible = true;
                        lblerror.Text = "Debes ingresar Tope Ips";
                    }
                    else
                    {
                        lblerror.Visible = false;
                        //VALIDAR LA ESTRUCTURA CORRECTA EN CASO DE DECIMAL
                        bool decimalCorrecto = fnDecimal(txtTopeIps.Text, 3);
                        if (decimalCorrecto == false)
                        {
                            lblerror.Visible = true;
                            lblerror.Text = "Valor no valido para Tope Ips";
                        }
                        else
                        {
                            lblerror.Visible = false;
                        }
                    }
                }
                else if (txtMesApv.ContainsFocus)
                {
                    if (txtMesApv.Text == "")
                    {
                        lblerror.Visible = true;
                        lblerror.Text = "Debes ingresar Tope Mensual Ahorro previsional voluntario";
                    }
                    else
                    {
                        lblerror.Visible = false;
                        //VALIDAR LA ESTRUCTURA CORRECTA EN CASO DE DECIMAL
                        bool decimalCorrecto = fnDecimal(txtMesApv.Text, 3);
                        if (decimalCorrecto == false)
                        {
                            lblerror.Visible = true;
                            lblerror.Text = "Valor no valido para Tope Mensual Ahorro previsional voluntario";
                        }
                        else
                        {
                            lblerror.Visible = false;
                        }
                    }
                }
                else if (txtUfAnt.ContainsFocus)
                {
                    if (txtUfAnt.Text == "")
                    {
                        lblerror.Visible = true;
                        lblerror.Text = "Debes ingresar uf mes anterior";
                    }
                    else
                    {
                        lblerror.Visible = false;
                        //VALIDAR LA ESTRUCTURA CORRECTA EN CASO DE DECIMAL
                        bool decimalCorrecto = fnDecimal(txtUfAnt.Text, 7);
                        if (decimalCorrecto == false)
                        {
                            lblerror.Visible = true;
                            lblerror.Text = "Valor no valido como uf mes anterior";
                        }
                        else
                        {
                            lblerror.Visible = false;
                        }
                    }
                }
            }

            return base.ProcessDialogKey(keyData);
        }

        //PARA SABER SI YA SE INGRESO DATA PARA EL PERIODO EN CURSO
        private bool ExisteDataPeriodo()
        {
            string sql = "select UF, utm from valoresmes WHERE anomes = @periodo";
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
                        cmd.Parameters.Add(new SqlParameter("@periodo", Calculo.PeriodoObservado));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //SI HAY REGISTROS ES PORQUE YA EXISTE DATA PARA PERIODO EN CURSO...
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

        //VERIFICAR SI HAY CAMBIOS ANTES DE CERRAR
        private bool CambiosSinGuardar()
        {
            string sql = "SELECT anomes, uf, utm, ingresominimo, sis, topeafp, topesec, topeIps, topeMesApv, ufMesAnt " +
                        " FROM valoresmes WHERE anomes=@pActual";
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pActual", Calculo.PeriodoObservado));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //RECORREMOS Y COMPARAMOS
                            while (rd.Read())
                            {
                                try
                                {
                                    if (fnSistema.IsNumeric(txtUf.Text))
                                    {
                                        if (Convert.ToDouble(txtUf.Text) != Convert.ToDouble(rd["uf"])) return true; 
                                    }
                                    if (fnSistema.IsNumeric(txtUtm.Text))
                                    {
                                        if (Convert.ToDouble(txtUtm.Text) != Convert.ToDouble(rd["utm"])) return true; 
                                    }
                                    if (fnSistema.IsNumeric(txtIMM.Text))
                                    {
                                        if (Convert.ToDouble(txtIMM.Text) != Convert.ToDouble(rd["ingresominimo"])) return true; 
                                    }
                                    if (fnSistema.IsNumeric(txtSis.Text))
                                    {
                                        if (Convert.ToDouble(txtSis.Text) != Convert.ToDouble(rd["sis"])) return true; 
                                    }
                                    if (fnSistema.IsNumeric(txtTopeAfp.Text))
                                    {
                                        if (Convert.ToDouble(txtTopeAfp.Text) != Convert.ToDouble(rd["topeafp"])) return true; 
                                    }
                                    if (fnSistema.IsNumeric(txtTopeSec.Text))
                                    {
                                        if (Convert.ToDouble(txtTopeSec.Text) != Convert.ToDouble(rd["topesec"])) return true; 
                                    }
                                    if (fnSistema.IsNumeric(txtTopeIps.Text))
                                    {
                                        if (Convert.ToDouble(txtTopeIps.Text) != Convert.ToDouble(rd["topeips"])) return true;
                                    }
                                    if (fnSistema.IsNumeric(txtMesApv.Text))
                                    {
                                        if (Convert.ToDouble(txtMesApv.Text) != Convert.ToDouble(rd["topemesapv"])) return true;
                                    }
                                    if (fnSistema.IsNumeric(txtUfAnt.Text))
                                    {
                                        if (Convert.ToDouble(txtUfAnt.Text) != Convert.ToDouble(rd["ufMesAnt"])) return true;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    XtraMessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
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

            return false;
        }

        #region "LOG INDICE MENSUAL"
        //TABLA HASH CON TODOS LOS VALORES
        private Hashtable PrecargaIndiceMensual(int Periodo)
        {
            Hashtable datos = new Hashtable();
            string sql = "SELECT uf, utm, ingresominimo, sis, topeafp, topesec, sanna, topeIps, topeMesApv, " +
                "ufMesAnt, factorimp FROM valoresmes WHERE anomes=@periodo";
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@periodo", Periodo));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //LLENAMOS TABLA HASH
                                datos.Add("uf", (decimal)rd["uf"]);
                                datos.Add("utm",(decimal)rd["utm"]);
                                datos.Add("ingresominimo", (decimal)rd["ingresominimo"]);
                                datos.Add("sis", (decimal)rd["sis"]);
                                datos.Add("topeafp", (decimal)rd["topeafp"]);
                                datos.Add("topesec", (decimal)rd["topesec"]);
                                datos.Add("sanna", (decimal)rd["sanna"]);
                                datos.Add("topeips", Convert.ToDecimal(rd["topeIps"]));
                                datos.Add("topemesapv", Convert.ToDecimal(rd["topeMesApv"]));
                                datos.Add("ufmesanterior", Convert.ToDecimal(rd["ufMesAnt"]));
                                datos.Add("factorimpuesto", Convert.ToDouble(rd["factorimp"]));
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
        private void ComparaValorIndice(Hashtable Datos, int periodo, decimal uf, decimal utm, decimal IngresoMinimo, 
            decimal sis, decimal topeAfp, decimal topeSec, decimal sanna, decimal pTopeIps, decimal pTopeMesApv, 
            decimal pUfAnterior, double pFactorImpuesto)
        {
            if (Datos.Count > 0)
            {
                if ((decimal)Datos["uf"] != uf)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA VALOR UF INDICE MENSUAL PERIODO " + periodo, "VALORESMES", uf + "", (decimal)Datos["uf"] + "", "MODIFICAR");
                    log.Log();
                }
                if ((decimal)Datos["utm"] != utm)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA VALOR UTM INDICE MENSUAL PERIODO " + periodo, "VALORESMES", utm + "", (decimal)Datos["utm"] + "", "MODIFICAR");
                    log.Log();
                }
                if ((decimal)Datos["ingresominimo"] != IngresoMinimo)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA VALOR INGRESO MINIMO INDICE MENSUAL PERIODO " + periodo, "VALORESMES", IngresoMinimo + "", (decimal)Datos["ingresominimo"] + "", "MODIFICAR");
                    log.Log();
                }
                if ((decimal)Datos["sis"] != sis)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA VALOR SIS INDICE MENSUAL PERIODO " + periodo, "VALORESMES", sis + "", (decimal)Datos["sis"] + "", "MODIFICAR");
                    log.Log();
                }
                if ((decimal)Datos["topeafp"] != topeAfp)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA TOPE AFP INDICE MENSUAL PERIODO " + periodo, "VALORESMES", topeAfp + "", (decimal)Datos["topeafp"] + "", "MODIFICAR");
                    log.Log();
                }
                if ((decimal)Datos["topesec"] != topeSec)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA TOPE SEGURO INDICE MENSUAL PERIODO " + periodo, "VALORESMES", topeSec + "", (decimal)Datos["topesec"] + "", "MODIFICAR");
                    log.Log();
                }
                if ((decimal)Datos["sanna"] != sanna)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA PORCENTAJE SANNA INDICE MENSUAL PERIODO " + periodo, "VALORESMES", sanna + "", (decimal)Datos["sanna"] + "", "MODIFICAR");
                    log.Log();
                }
                if ((decimal)Datos["topeips"] != pTopeIps)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA TOPE IPS INDICE MENSUAL PERIODO " + periodo, "VALORESMES", pTopeIps + "", (decimal)Datos["topeips"] + "", "MODIFICAR");
                    log.Log();
                }
                if ((decimal)Datos["topemesapv"] != pTopeMesApv)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA TOPE MES AHORRO PREVISIONAL VOLUNTARIO INDICE MENSUAL PERIODO " + periodo, "VALORESMES", pTopeMesApv + "", (decimal)Datos["topemesapv"] + "", "MODIFICAR");
                    log.Log();
                }
                if ((decimal)Datos["ufmesanterior"] != pUfAnterior)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA UF MES ANTERIOR INDICE MENSUAL PERIODO " + periodo, "VALORESMES", pUfAnterior + "", (decimal)Datos["ufmesanterior"] + "", "MODIFICAR");
                    log.Log();
                }
                if ((double)Datos["factorimpuesto"] != pFactorImpuesto)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA FACTOR IMPUESTO INDICE MENSUAL PERIODO " + periodo, "VALORESMES", (double)Datos["factorimpuesto"] + "", pFactorImpuesto + "", "MODIFICAR");
                    log.Log();
                }

            }
        }

        #endregion
        #endregion

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            //SESION NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            bool decimalValido = false;
            if (txtPeriodo.Text == "") { lblerror.Visible = true; lblerror.Text = "El periodo no es valido";return;}
            if (txtUf.Text == "")
            {
                lblerror.Visible = true;
                lblerror.Text = "Debes llenar campo UF";
                txtUf.Focus();
                return;
            }
            else
            {
                //VALIDAR QUE UF ES VALIDA
                decimalValido = fnDecimal(txtUf.Text, 7);
                if (decimalValido == false)
                {
                    lblerror.Visible = true;
                    lblerror.Text = "Valor no valido como uf";
                    txtUf.Focus();
                    return;
                }
                else
                {
                    lblerror.Visible = false;
                }
            }

            if (txtUtm.Text == "")
            { lblerror.Visible = true; lblerror.Text = "Debes llenar campo UTM";txtUtm.Focus(); return; }
            else
            {
                //VALIDAR QUE utm ES VALIDA
                decimalValido = fnDecimal(txtUtm.Text, 7);
                if (decimalValido == false)
                {
                    lblerror.Visible = true;
                    lblerror.Text = "Valor no valido como utm";
                    txtUtm.Focus();
                    return;
                }
                else
                {
                    lblerror.Visible = false;
                }
            }

            if (txtIMM.Text == "")
            { lblerror.Visible = true; lblerror.Text = "Debes llenar el campo Ingreso Minimo Mensual ";txtIMM.Focus(); return; }
            else
            {
                //VALIDAR QUE IMM ES VALIDA
                decimalValido = fnDecimal(txtIMM.Text, 7);
               
                if (decimalValido == false)
                {
                    lblerror.Visible = true;
                    lblerror.Text = "Valor no valido como ingreso minimo mensual";
                    txtIMM.Focus();
                    return;
                }
                else
                {
                    lblerror.Visible = false;
                }
            }

            if (txtSis.Text == "")
            { lblerror.Visible = true; lblerror.Text = "Debes llenar el campo Sis";txtSis.Focus(); return; }
            else
            {
                //VALIDAR QUE SIS ES VALIDA
                decimalValido = fnDecimal(txtSis.Text, 2);
                if (decimalValido == false)
                {
                    lblerror.Visible = true;
                    lblerror.Text = "Valor no valido como Sis";
                    txtSis.Focus();
                    return;
                }
                else
                {
                    if (txtSis.Text.Contains(",") == false)
                    {
                        if (txtSis.Text.Length > 2)
                        {
                            lblerror.Visible = true;
                            lblerror.Text = "Valor no valido como Sis";
                            txtSis.Focus();
                            return;
                        }
                    }

                    lblerror.Visible = false;
                }
            }

            if (txtTopeAfp.Text == "") { lblerror.Visible = true; lblerror.Text = "Debes llenar el campo Tope Afp";txtTopeAfp.Focus(); return; }
            else
            {
                //VALIDAR QUE SIS ES VALIDA
                decimalValido = fnDecimal(txtTopeAfp.Text, 3);
                if (decimalValido == false)
                {
                    lblerror.Visible = true;
                    lblerror.Text = "Valor no valido como Tope Afp";
                    txtTopeAfp.Focus();
                    return;
                }
                else
                {
                    lblerror.Visible = false;
                }
            }

            if (txtTopeSec.Text == "")
            { lblerror.Visible = true; lblerror.Text = "Debes llenar campo Tope Sec";txtTopeSec.Focus(); return; }
            else
            {
                //VALIDAR QUE SIS ES VALIDA
                decimalValido = fnDecimal(txtTopeSec.Text, 3);
                if (decimalValido == false)
                {
                    lblerror.Visible = true;
                    lblerror.Text = "Valor no valido como Tope Sec";
                    txtTopeSec.Focus();
                    return;
                }
                else
                {
                    lblerror.Visible = false;
                }
            }

            if (txtSanna.Text == "")
            { lblerror.Visible = true; lblerror.Text = "Debes llenar campo sanna"; txtSanna.Focus(); return; }
            else {
                if (fnDecimalSanna(txtSanna.Text) == false)
                { lblerror.Visible = true; lblerror.Text = "Ingresa un valor válido para campo sanna."; txtSanna.Focus(); return; }
            }

            if (txtTopeIps.Text == "")
            { lblerror.Visible = true; lblerror.Text = "Ingresa tope ips."; txtTopeIps.Focus(); return; }
            else
            {
                if (fnDecimal(txtTopeIps.Text, 3) == false)
                {
                    lblerror.Visible = true;
                    lblerror.Text = "Valor no válido para Tope ips";
                    txtTopeIps.Focus();
                    return;
                }

                lblerror.Visible = false;
            }

            if (txtMesApv.Text == "")
            { lblerror.Visible = true; lblerror.Text = "Ingresa tope ahorro previsional voluntario."; txtMesApv.Focus(); return; }
            else
            {
                if (fnDecimal(txtMesApv.Text, 3) == false)
                {
                    lblerror.Visible = true;
                    lblerror.Text = "Valor no válido para Tope Ahorro previsional voluntario";
                    txtMesApv.Focus();
                    return;
                }

                lblerror.Visible = false;
            }

            if (txtUfAnt.Text == "")
            { lblerror.Visible = true; lblerror.Text = "Ingresa valor uf mes anterior"; txtUfAnt.Focus(); return; }
            else
            {
                if (fnDecimal(txtUfAnt.Text, 7) == false)
                {
                    lblerror.Visible = true;
                    lblerror.Text = "Valor no válido para Uf mes anterior";
                    txtUfAnt.Focus();
                    return;
                }

                lblerror.Visible = false;
            }

            if (txtfactorImp.Text == "")
            { XtraMessageBox.Show("Por favor ingresa un valor válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtfactorImp.Focus(); return; }

            lblerror.Visible = false;
            bool PeriodoExiste = false;
 
                //ES UPDATE
            if (txtPeriodo.Text != "")
                PeriodoExiste = fnPeriodoExiste(int.Parse(txtPeriodo.Text));

            //SI LA VARIABLE PERIODOEXISTE ES TRUE ES PORQUE EL PERIODO EXISTE PODEMOS REALIZAR UPDATE
            if (PeriodoExiste)
               {
                  fnModificarMensual(txtPeriodo, txtUf, txtUtm, txtIMM, txtSis, txtTopeAfp, txtTopeSec, txtSanna, txtTopeIps, txtUfAnt, txtMesApv, txtfactorImp);
               }
            else
               {
                  lblerror.Visible = false;
                  lblerror.Text = "Parece ser que el periodo no es valido";
                  return;
               }                        
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            
        }

        //SOLO NUMEROS Y COMA
        private void txtUf_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (e.KeyChar == (char)44)
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

        private void txtUtm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (e.KeyChar == (char)44)
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

        private void txtIMM_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (e.KeyChar == (char)44)
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

        private void txtSis_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (e.KeyChar == (char)44)
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

        private void txtTopeAfp_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (e.KeyChar == (char)44)
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

        private void txtTopeSec_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (e.KeyChar == (char)44)
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

        private void txtPeriodo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void txtUf_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (txtUf.Text == "")
                {
                    lblerror.Visible = true;
                    lblerror.Text = "Debes ingresar unidad de fomento";
                }
                else
                {
                    lblerror.Visible = false;
                    //VALIDAR LA ESTRUCTURA CORRECTA EN CASO DE DECIMAL
                    bool decimalCorrecto = fnDecimal(txtUf.Text, 6);
                    if (decimalCorrecto == false)
                    {
                        lblerror.Visible = true;
                        lblerror.Text = "Valor no valido como uf";
                    }
                    else
                    {
                        lblerror.Visible = false;
                    }
                }
            }
        }

        private void txtUtm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (txtUtm.Text == "")
                {
                    lblerror.Visible = true;
                    lblerror.Text = "Debes ingresar utm";
                }
                else
                {
                    lblerror.Visible = false;
                    //VALIDAR LA ESTRUCTURA CORRECTA EN CASO DE DECIMAL
                    bool decimalCorrecto = fnDecimal(txtUtm.Text, 6);
                    if (decimalCorrecto == false)
                    {
                        lblerror.Visible = true;
                        lblerror.Text = "Valor no valido como utm";
                    }
                    else
                    {
                        lblerror.Visible = false;
                    }
                }
            }
        }

        private void txtIMM_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (txtIMM.Text == "")
                {
                    lblerror.Visible = true;
                    lblerror.Text = "Ingreso minimo mensual no valido";
                }
                else
                {
                    lblerror.Visible = false;
                    //VALIDAR LA ESTRUCTURA CORRECTA EN CASO DE DECIMAL
                    bool decimalCorrecto = fnDecimal(txtIMM.Text, 7);
                    if (decimalCorrecto == false)
                    {
                        lblerror.Visible = true;
                        lblerror.Text = "Ingreso minimo mensual no valido";
                    }
                    else
                    {
                        lblerror.Visible = false;
                    }
                }
            }
        }

        private void txtSis_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (txtSis.Text == "")
                {
                    lblerror.Visible = true;
                    lblerror.Text = "Debes llenar el campo sis";
                }
                else
                {
                    lblerror.Visible = false;
                    //VALIDAR LA ESTRUCTURA CORRECTA EN CASO DE DECIMAL
                    bool decimalCorrecto = fnDecimal(txtSis.Text, 2);
                    if (decimalCorrecto == false)
                    {
                        lblerror.Visible = true;
                        lblerror.Text = "Valor sis no valido";
                    }
                    else
                    {
                        lblerror.Visible = false;
                    }
                }
            }
        }

        private void txtTopeAfp_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (txtTopeAfp.Text == "")
                {
                    lblerror.Visible = true;
                    lblerror.Text = "Debes llenar el campo Tope Afp";
                }
                else
                {
                    lblerror.Visible = false;
                    //VALIDAR LA ESTRUCTURA CORRECTA EN CASO DE DECIMAL
                    bool decimalCorrecto = fnDecimal(txtTopeAfp.Text, 3);
                    if (decimalCorrecto == false)
                    {
                        lblerror.Visible = true;
                        lblerror.Text = "Valor no vlaido para cmapo Tope Afp";
                    }
                    else
                    {
                        lblerror.Visible = false;
                    }
                }
            }
        }

        private void txtTopeSec_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (txtTopeSec.Text == "")
                {
                    lblerror.Visible = true;
                    lblerror.Text = "Debes ingresar Tope Sec";
                }
                else
                {
                    lblerror.Visible = false;
                    //VALIDAR LA ESTRUCTURA CORRECTA EN CASO DE DECIMAL
                    bool decimalCorrecto = fnDecimal(txtTopeSec.Text, 3);
                    if (decimalCorrecto == false)
                    {
                        lblerror.Visible = true;
                        lblerror.Text = "Valor no valido para Tope Sec";
                    }
                    else
                    {
                        lblerror.Visible = false;
                    }
                }
            }
        }

        //private void brnPeriodo_Click(object sender, EventArgs e)
        //{
        //    //NUEVA ACTIVIDAD DE SESION
        //    Sesion.NuevaActividad();

        //    //RECARGAR CAMPOS CON EL PERIODO ACTUAL
        //    fnCargarPeriodoEspecifico(Calculo.PeriodoObservado);
        //}

        private void txtPeriodo_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtUf_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtUtm_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtIMM_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtSis_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtTopeAfp_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtTopeSec_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            //NUEVA SESION
            Sesion.NuevaActividad();

            if (viewIndiceMensual.RowCount > 0)
            {
                if (CambiosSinGuardar())
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

        private void txtSanna_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (e.KeyChar == (char)44)
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

        private void txtSanna_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (txtSanna.Text == "")
                { lblerror.Visible = true; lblerror.Text = "Por favor ingresa un valor para campo sanna"; return; }
                else
                {
                    //VALIDAR QUE TENGA EL FORMATO CORRECTO
                    if (fnDecimalSanna(txtSanna.Text) == false)
                    { lblerror.Visible = true; lblerror.Text = "Por favor ingresa un valor correcto en campo sanna"; return; }
                }

                lblerror.Visible = false;
            }
        }

        private void txtTopeIps_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (e.KeyChar == (char)44)
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

        private void txtMesApv_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (e.KeyChar == (char)44)
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

        private void brnPeriodo_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            //RECARGAR CAMPOS CON EL PERIODO ACTUAL
            fnCargarPeriodoEspecifico(Calculo.PeriodoObservado);
        }
    }
}
