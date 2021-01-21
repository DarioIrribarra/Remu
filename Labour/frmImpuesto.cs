using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using DevExpress.XtraEditors;
using System.Collections;
//PARA IMPORTAR DLL PARA DESHABILITAR BOTON DE CIERRE DE LA VENTANA
using System.Runtime.InteropServices;

namespace Labour
{
    public partial class frmImpuesto : DevExpress.XtraEditors.XtraForm
    {
        [DllImport("user32")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32")]
        static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        const int MF_BYCOMMAND = 0;
        const int MF_DISABLED = 2;
        const int SC_CLOSE = 0xF060;

        //VARIABLE PARA SABER SI ES UPDATE O INSERT
        private bool Update = false;

        //OBTENER LA POSICION ACTUAL DE LA GRILLA
        private int PositionGrid = 0;

        Operacion op;

        public frmImpuesto()
        {
            InitializeComponent();
        }

        private void frmImpuesto_Load(object sender, EventArgs e)
        {
            var sm = GetSystemMenu(Handle, false);
            EnableMenuItem(sm, SC_CLOSE, MF_BYCOMMAND | MF_DISABLED);

            op = new Operacion();

            fnDefaultProperties();

            string grilla = "SELECT idImptoUnico, tramo, tope, factor, rebaja, inicio, hasta, dato01, dato02 FROM impUnico ORDER by tramo";
            fnSistema.spllenaGridView(GridImpuesto, grilla);
            fnSistema.spOpcionesGrilla(viewImpuesto);
            fnColumnas();

            //CARGAR COMBO TRAMO
            fnComboTramo(txtTramo);

            fnCargarCampos(0);           

        }

        #region "MANEJO DE DATOS"
        //INGRESO NUEVO IMPUESTO
        /*
         * DATOS DE ENTRADA:
         * id
         * tramo
         * tope
         * factor
         * rebaja
         * inicio
         * hasta
         */

        private void fnNuevoImpuesto(TextEdit pId, LookUpEdit pTramo, TextEdit pTope, TextEdit pFactor, TextEdit pRebaja,
            TextEdit pInicio, TextEdit pHasta, TextEdit pDato1, TextEdit pDato2)
        {
            string sql = "INSERT INTO ImpUnico(idImptoUnico, tramo, tope, factor, rebaja, inicio, hasta, dato01, dato02) VALUES" +
                "(@pId, @pTramo, @pTope, @pFactor, @pRebaja, @pInicio, @pHasta, @pDato1, @pDato2)";

            SqlCommand cmd;
            int res = 0;

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pId", pId.Text));
                        cmd.Parameters.Add(new SqlParameter("@pTramo", pTramo.EditValue));
                        cmd.Parameters.Add(new SqlParameter("@pTope", decimal.Parse(pTope.Text))); //decimal
                        cmd.Parameters.Add(new SqlParameter("@pFactor", decimal.Parse(pFactor.Text))); //decimal
                        cmd.Parameters.Add(new SqlParameter("@pRebaja", decimal.Parse(pRebaja.Text)));//decimal
                        cmd.Parameters.Add(new SqlParameter("@pInicio", pInicio.Text));
                        cmd.Parameters.Add(new SqlParameter("@pHasta", pHasta.Text));
                        cmd.Parameters.Add(new SqlParameter("@pDato1", pDato1.Text));
                        cmd.Parameters.Add(new SqlParameter("@pDato2", pDato2.Text));

                        //EJECUTAR CONSULTA
                        res = cmd.ExecuteNonQuery();

                        if (res > 0)
                        {
                            XtraMessageBox.Show("Ingreso correcto", "Guardar", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            //GUARDAR EVENTO EN LOG
                            logRegistro log = new logRegistro(User.getUser(), "SE INGRESA NUEVO IMPUESTO UNICO ID:" + pId.Text, "IMPUNICO", "0", "TRAMO:" + pTramo.EditValue, "INGRESAR");
                            log.Log();

                            string grilla = "SELECT idImptoUnico, tramo, tope, factor, rebaja, inicio, hasta, dato01, dato02 FROM impUnico ORDER by tramo";
                            fnSistema.spllenaGridView(GridImpuesto, grilla);
                            //RECARGAR CAMPOS
                            fnCargarCampos(PositionGrid);                           
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

        //MODIFICAR IMPUESTO
        private void fnModificarImpuesto(TextEdit pId, LookUpEdit pTramo, TextEdit pTope, TextEdit pFactor, TextEdit pRebaja,
            TextEdit pInicio, TextEdit pHasta, int TramoAntiguo, TextEdit pDato1, TextEdit pDato2)
        {
            string sql = "UPDATE ImpUnico SET tramo=@pTramo, tope=@pTope, factor=@pFactor, rebaja=@pRebaja, " +
                "inicio=@pInicio, hasta=@pHasta, dato01=@pDato1, dato02=@pDato2 " +
                "WHERE idImptoUnico=@pId AND tramo=@pTramoAntiguo";

            SqlCommand cmd;
            int res = 0;

            //TABLA HASH PARA LOG IMPUESTO
            Hashtable datosImpuesto = new Hashtable();
            datosImpuesto = PrecargaImpuesto(int.Parse(pId.Text), TramoAntiguo);

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pId", pId.Text));
                        cmd.Parameters.Add(new SqlParameter("@pTramo", pTramo.EditValue));
                        cmd.Parameters.Add(new SqlParameter("@pTope", decimal.Parse(pTope.Text)));
                        cmd.Parameters.Add(new SqlParameter("@pFactor", decimal.Parse(pFactor.Text)));
                        cmd.Parameters.Add(new SqlParameter("@pRebaja", decimal.Parse(pRebaja.Text)));
                        cmd.Parameters.Add(new SqlParameter("@pInicio", pInicio.Text));
                        cmd.Parameters.Add(new SqlParameter("@pHasta", pHasta.Text));
                        cmd.Parameters.Add(new SqlParameter("@pDato1", pDato1.Text));
                        cmd.Parameters.Add(new SqlParameter("@pDato2", pDato2.Text));
                        cmd.Parameters.Add(new SqlParameter("@pTramoAntiguo", TramoAntiguo));

                        //EJECUTAR CONSULTA
                        res = cmd.ExecuteNonQuery();

                        if (res > 0)
                        {
                            XtraMessageBox.Show("Registro Actualizado", "Actualizar", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            //SI HAY CAMBIOS GUARDAMOS EN LOG
                            ComparaValorImpuesto(int.Parse(pId.Text), datosImpuesto, Convert.ToInt32(pTramo.EditValue), 
                                decimal.Parse(pTope.Text), decimal.Parse(pFactor.Text), decimal.Parse(pRebaja.Text), 
                                int.Parse(pInicio.Text), int.Parse(pHasta.Text), txtdato1.Text, txtdato2.Text);

                            string grilla = "SELECT idImptoUnico, tramo, tope, factor, rebaja, inicio, hasta, dato01, dato02 FROM impUnico ORDER by tramo";
                            fnSistema.spllenaGridView(GridImpuesto, grilla);
                            //RECARGAR CAMPOS
                            fnCargarCampos(PositionGrid);                            
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
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        //ELIMINAR IMPUESTO
        /*
         * PARAMETRO DE ENTRADA:
         * ID
         */
        private void fnEliminarImpuesto(int pId, int pTramo)
        {
            string sql = "DELETE FROM IMPUNICO WHERE idImptoUnico=@pId AND tramo=@pTramo";

            SqlCommand cmd;
            int res = 0;

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pId", pId));
                        cmd.Parameters.Add(new SqlParameter("@pTramo", pTramo));

                        //EJECUTAR CONSULTA
                        res = cmd.ExecuteNonQuery();

                        if (res > 0)
                        {
                            XtraMessageBox.Show("Registro Eliminado", "Eliminar", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            //GUARDAR EVENTO EN LOG
                            logRegistro log = new logRegistro(User.getUser(), "SE HA ELIMINADO IMPUESTO CON ID " + pId, "IMPUNICO", "0", "0", "ELIMINAR");
                            log.Log();

                            string grilla = "SELECT idImptoUnico, tramo, tope, factor, rebaja, inicio, hasta, dato01, dato02 FROM impUnico ORDER by tramo";
                            fnSistema.spllenaGridView(GridImpuesto, grilla);
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
                    XtraMessageBox.Show("No hay conexion con la base de datos", "Base de datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        //DEFAULT PROPERTIES
        private void fnDefaultProperties()
        {
            btnNuevo.TabStop = false;
            btnGuardar.TabStop = false;
            btnEliminar.TabStop = false;
            GridImpuesto.TabStop = false;
            separador1.TabStop = false;

            txtTope.Properties.MaxLength = 8;
            txtFactor.Properties.MaxLength = 8;
            txtRebaja.Properties.MaxLength = 8;
            txtInicio.Properties.MaxLength = 6; //201712
            txtInicio.Text = (Calculo.PeriodoObservado).ToString();
            txtHasta.Properties.MaxLength = 6;
            txtHasta.Text = "300001";
            txtdato1.Properties.MaxLength = 10;
            txtdato2.Properties.MaxLength = 10;

            txtIdImpuesto.Focus();
            lblError.Visible = false;

            Update = false;
        }

        //LIMPIAR CAMPOS
        private void fnLimpiarCampos()
        {
            txtFactor.Text = "0";
            txtHasta.Text = "300001";
            txtIdImpuesto.Text = "";
            txtInicio.Text = (Calculo.PeriodoObservado).ToString();
            txtRebaja.Text = "0";
            txtTope.Text = "0";     
            txtIdImpuesto.ReadOnly = false;
            txtdato1.Text = "";
            txtdato2.Text = "";

            txtIdImpuesto.Text = (fnItemDisponibles()).ToString();

            lblError.Visible = false;
            Update = false;
            btnEliminar.Enabled = false;
        }

        //VERIFICAR SI ID y TRAMO YA EXISTEN
        private bool fnExisteIdTramo(int pId, int pTramo)
        {
            string sql = "SELECT idImptoUnico FROM impUnico WHERE idImptoUnico=@pId AND tramo=@pTramo";

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
                        cmd.Parameters.Add(new SqlParameter("@pId", pId));
                        cmd.Parameters.Add(new SqlParameter("@pTramo", pTramo));

                        //EJECUTAR CONSULTA
                        rd = cmd.ExecuteReader();

                        if (rd.HasRows)
                        {
                            //EXISTE EL REGISTRO
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
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return existe;
        }

        //CONFIGURAR COLUMNAS GRILLA
        private void fnColumnas()
        {
            //NO MOSTRAR ID
            viewImpuesto.Columns[0].Visible = false;
            viewImpuesto.Columns[0].Caption = "Id";
            viewImpuesto.Columns[0].AppearanceCell.FontStyleDelta = FontStyle.Bold;
            viewImpuesto.Columns[0].Width = 10;

            viewImpuesto.Columns[1].Caption = "Tramo";
            viewImpuesto.Columns[1].Width = 40;

            viewImpuesto.Columns[2].Caption = "Tope (UTM)";

            viewImpuesto.Columns[3].Caption = "Factor (%)";

            viewImpuesto.Columns[4].Caption = "Rebaja (UTM)";

            viewImpuesto.Columns[5].Caption = "Inicio";

            viewImpuesto.Columns[6].Caption = "Hasta";

            //COLUMNAS DATO1 Y DATO2
            viewImpuesto.Columns[7].Visible = false;
            viewImpuesto.Columns[8].Visible = false;           
            
        }

        //METODO PARA CARGAR COMBO tramo
        //TIENE CUATRO VALORES 1, 2, 3, 4
        private void fnComboTramo(LookUpEdit pCombo)
        {
            //instanciamos a la clase combo
            List<datoCombobox> lista = new List<datoCombobox>();
            int i = 1;
            while (i <= 7)
            {
                lista.Add(new datoCombobox() { descInfo = "" + i, KeyInfo = i });
                i++;
            }
            //PROPIEDADES COMBOBOX
            pCombo.Properties.DataSource = lista.ToList();
            pCombo.Properties.ValueMember = "KeyInfo";
            pCombo.Properties.DisplayMember = "descInfo";

            pCombo.Properties.PopulateColumns();

            //ocultamos la columan key
            pCombo.Properties.Columns[0].Visible = false;

            pCombo.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
            pCombo.Properties.AutoSearchColumnIndex = 1;
            pCombo.Properties.ShowHeader = false;
        }

        //CARGAR CAMPOS DESDE GRILLA
        private void fnCargarCampos(int? pos = -1)
        {
            if (viewImpuesto.RowCount > 0)
            {
                if (pos != -1) viewImpuesto.FocusedRowHandle = (int)pos;

                //DEJAR VARIABLE UPDATE EN TRUE
                Update = true;

                //DEJAR COMO SOLO LECTURA EL ID
                txtIdImpuesto.ReadOnly = true;

                lblError.Visible = false;

                btnEliminar.Enabled = true;

                txtIdImpuesto.Text = viewImpuesto.GetFocusedDataRow()["idImptoUnico"].ToString();
                txtFactor.Text = viewImpuesto.GetFocusedDataRow()["factor"].ToString();
                txtHasta.Text = viewImpuesto.GetFocusedDataRow()["hasta"].ToString();
                txtInicio.Text = viewImpuesto.GetFocusedDataRow()["inicio"].ToString();
                txtRebaja.Text = viewImpuesto.GetFocusedDataRow()["rebaja"].ToString();
                txtTope.Text = viewImpuesto.GetFocusedDataRow()["tope"].ToString();
                int tram = int.Parse(viewImpuesto.GetFocusedDataRow()["tramo"].ToString());
                txtTramo.EditValue = tram;
                txtdato1.Text = (string)viewImpuesto.GetFocusedDataRow()["dato01"];
                txtdato2.Text = (string)viewImpuesto.GetFocusedDataRow()["dato02"];

                //RESET BOTON NUEVO (CAPTION)
                op.SetButtonProperties(btnNuevo, 1);
                op.Cancela = false;
               
            }
        }

        //MANEJAR TECLA TAB
        protected override bool ProcessDialogKey(Keys keyData)
        {
            bool formatoCorrecto = false;
            if (keyData == Keys.Tab)
            {
                if (txtIdImpuesto.ContainsFocus)
                {
                    if (txtIdImpuesto.Text == "")
                    {
                        lblError.Visible = true;
                        lblError.Text = "Por favor ingrese un identificador";
                        return false;
                    }
                    else
                    {
                        lblError.Visible = false;
                    }
                }
                else if (txtFactor.ContainsFocus)
                {
                    if (txtFactor.Text == "")
                    {
                        lblError.Visible = true;
                        lblError.Text = "Por favor llene campo factor";
                        return false;
                    }
                    else
                    {
                        lblError.Visible = false;
                        //VERIFICAR QUE TENGA UNA COMPOSICION CORRECTA (FORMATO DECIMAL)
                        formatoCorrecto = fnDecimal(txtFactor.Text);
                        if (formatoCorrecto)
                        {
                            //VALIDO
                            lblError.Visible = false;
                        }
                        else
                        {
                            lblError.Visible = true;
                            lblError.Text = "Valor erroneo para campo factor";
                            return false;
                        }
                        
                    }
                }
                else if (txtHasta.ContainsFocus)
                {
                    if (txtHasta.Text == "")
                    {
                        lblError.Visible = true;
                        lblError.Text = "Por favor llene campo Hasta";
                    }
                    else
                    {
                        lblError.Visible = false;
                    }
                }
                else if (txtInicio.ContainsFocus)
                {
                    if (txtInicio.Text == "")
                    {
                        lblError.Visible = true;
                        lblError.Text = "Por favor llene campo Inicio";
                        return false;
                    }
                    else
                    {
                        lblError.Visible = false;                        
                    }
                }
                else if (txtRebaja.ContainsFocus)
                {
                    if (txtRebaja.Text == "")
                    {
                        lblError.Visible = true;
                        lblError.Text = "Por favor llene campo Rebaja";
                        return false;
                    }
                    else
                    {
                        lblError.Visible = false;
                        //VALIDAR QUE TENGA FORMATO CORRECTO
                        formatoCorrecto = fnDecimal(txtRebaja.Text);
                        if (formatoCorrecto)
                        {
                            //VALIDO
                        }
                        else
                        {
                            lblError.Visible = true;
                            lblError.Text = "Valor erroneo para campo rebaja";
                            return false;
                        }
                    }
                }
                else if (txtTope.ContainsFocus)
                {
                    if (txtTope.Text == "")
                    {
                        lblError.Visible = true;
                        lblError.Text = "Por favor llene campo Tope";
                        return false;
                    }
                    else
                    {
                        lblError.Visible = false;
                        //VALIDAR FORMATO CORRECTO DECIMAL
                        formatoCorrecto = fnDecimal(txtTope.Text);
                        if (formatoCorrecto)
                        {
                            lblError.Visible = false;
                        }
                        else
                        {
                            lblError.Visible = true;
                            lblError.Text = "Valor erroneo para campo Tope";
                            return false;
                        }
                    }
                }                
            }

            return base.ProcessDialogKey(keyData);
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

        //VERIFICAR SI HAY CAMBIOS ANTES DE SALIR DEL FORMULARIO
        private bool HayCambios(int id, int tramo)
        {
            string sql = "SELECT tramo, tope, factor, rebaja, inicio, hasta, dato01, dato02 FROM impunico WHERE " +
                        " idimptounico=@pId AND tramo=@pTramo";
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pId", id));
                        cmd.Parameters.Add(new SqlParameter("@pTramo", tramo));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //COMPARAMOS
                                if (Convert.ToInt32(txtTramo.EditValue) != Convert.ToInt32(rd["tramo"])) return true;
                                if (Convert.ToDouble(txtTope.Text) != Convert.ToDouble(rd["tope"])) return true;
                                if (Convert.ToDouble(txtFactor.Text) != Convert.ToDouble(rd["factor"])) return true;
                                if (Convert.ToDouble(txtRebaja.Text) != Convert.ToDouble(rd["rebaja"])) return true;
                                if (Convert.ToInt32(txtInicio.Text) != Convert.ToInt32(rd["inicio"])) return true;
                                if (Convert.ToInt32(txtHasta.Text) != Convert.ToInt32(rd["hasta"])) return true;
                                if (txtdato1.Text != (string)rd["dato01"]) return true;
                                if (txtdato2.Text != (string)rd["dato02"]) return true;
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

            return false;
        }

        #region "LOG IMPUESTO"
        //TABLA HASH PARA GUARDA LOS DATOS DEL REGISTRO EN EVALUACION
        private Hashtable PrecargaImpuesto(int pId, int pTramo)
        {
            string sql = "SELECT idImptoUnico, tramo, tope, factor, rebaja, inicio, hasta, dato01, dato02 FROM impunico WHERE" +
                " idimptounico=@pId AND tramo=@pTramo";
            Hashtable datos = new Hashtable();
            SqlCommand cmd;
            SqlDataReader rd;
      
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pId", pId));
                        cmd.Parameters.Add(new SqlParameter("@pTramo", pTramo));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {                            
                                //GUARDAMOS DATOS EN HASH
                                datos.Add("tramo", (int)rd["tramo"]);                                
                                datos.Add("tope", (decimal)rd["tope"]);
                                datos.Add("factor", (decimal)rd["factor"]);
                                datos.Add("rebaja", (decimal)rd["rebaja"]);
                                datos.Add("inicio", (int)rd["inicio"]);
                                datos.Add("hasta", (int)rd["hasta"]);
                                datos.Add("dato01", (string)rd["dato01"]);
                                datos.Add("dato02", (string)rd["dato02"]);
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
        private void ComparaValorImpuesto(int Pid, Hashtable Datos, int pTramo, decimal pTope, decimal pFactor, decimal pRebaja,
            int pInicio, int pHasta, string pDato1, string pDato2)
        {
            
            if (Datos.Count > 0)
            {
              
                if ((int)Datos["tramo"] != pTramo)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA TRAMO ID" + Pid, "IMPUNICO", (int)Datos["tramo"]+"", pTramo+"", "MODIFICAR");
                    log.Log();
                }
                if ((decimal)Datos["tope"] != pTope)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA TOPE ID" + Pid, "IMPUNICO", (decimal)Datos["tope"] + "", pTope + "", "MODIFICAR");
                    log.Log();
                }
                if ((decimal)Datos["factor"] != pFactor)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA FACTOR ID" + Pid, "IMPUNICO", (decimal)Datos["factor"] + "", pFactor + "", "MODIFICAR");
                    log.Log();
                }
                if ((decimal)Datos["rebaja"] != pRebaja)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA REBAJA ID" + Pid, "IMPUNICO", (decimal)Datos["rebaja"] + "", pRebaja + "", "MODIFICAR");
                    log.Log();
                }
                if ((int)Datos["inicio"] != pInicio)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA TOPE ID:" + Pid, "IMPUNICO", (int)Datos["inicio"] + "", pInicio + "", "MODIFICAR");
                    log.Log();
                }
                if ((int)Datos["hasta"] != pHasta)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA CAMPO HASTA ID:" + Pid, "IMPUNICO", (int)Datos["hasta"] + "", pHasta + "", "MODIFICAR");
                    log.Log();
                }
                if ((string)Datos["datos01"] != pDato1)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA CAMPO DATO01:" + pDato1, "IMPUNICO", (string)Datos["dato01"], pDato1, "MODIFICAR");
                    log.Log();
                }
                if ((string)Datos["datos02"] != pDato2)
                {
                    logRegistro log = new logRegistro(User.getUser(), "SE MODIFICA CAMPO DATO02:" + pDato2, "IMPUNICO", (string)Datos["dato02"], pDato2, "MODIFICAR");
                    log.Log();
                }
            }
        }
        #endregion

        #endregion

        #region "MANEJO NUMERO ID"
        //TRAER TODOS LOS NUMEROS DE ITEM DESDE LA BASE DE DATOS
        private int[] fnAllitem()
        {
            int total = 0;
            total = fnCantidad();
            int[] numeros = new int[total];

            int posicion = 0;      

            //sql consulta
            string sql = "SELECT idImptoUnico FROM impUnico";
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
                                numeros[posicion] = (int)rd["idImptoUnico"];
                                posicion++;
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
            return numeros;

        }

        //OBTENER LA CANTIDAD DE ELEMENTOS 
        private int fnCantidad()
        {
         
            string sql = "SELECT count(*) as cantidad FROM impUnico";
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
                        devuelve = fnUltimoNumero() + 1;

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

        //CONSULTAR POR EL ULTIMO ITEM INGRESADO
        private int fnUltimoNumero()
        {
            string sql = "SELECT max(idImptoUnico) as maximo FROM impUnico";
            SqlCommand cmd;
            SqlDataReader rd;
            int num = 0;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {                  

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

        //TRAER TODOS LOS TRAMOS DISPONIBLES PARA ESE CODIGO ID
        //DEVUELVE LISTA CON LOS TRAMOS ACTUALMENTE USADOS POR ESE IDIMPUESTO
        private List<int> fnAllTramos(int pId)
        {
            string sql = "SELECT tramo FROM impUnico WHERE idimptounico=@pId";
            SqlCommand cmd;
            SqlDataReader rd;
            List<int> tramos = new List<int>();
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pId", pId));

                        //EJECUTAR
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            tramos.Add((int)rd["tramo"]);
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
            return tramos;
        }

        //VERIFICAR SI ESE ID TIENE ALGUN TRAMO PARA UTILIZAR (LIBRE)
        private int fnTramoLibre(List<int> tramos)
        {
            //RECORREMOS LISTA
            int disponible = 0;
            if (tramos.Count > 0)
            {
                //FOR PARA RECORRE CADA ITEM DE LA LISTA TRAMO
                for (int i = 0; i < tramos.Count; i++)
                {
                    //FOR PARA COMPARAR...
                    for (int tram = 1; tram <= 4; tram++)
                    {
                        if (tramos[i] != tram)
                        {
                            //SI SON DISINTOS SIGNIGICA QUE ESTE TRAMO (NMERO) ESTA DISPONIBLE PARA ESE ID
                            //RETORNAMOS NUMERO DISPONIBLE
                            disponible = tram;
                            break;
                        }
                    }                    
                }
            }
            else
            {
                //SI LA LISTA ESTA VACIA ES PORQUE NO SE HAN USADO NINGUN NUMERO DE TRAMO PARA ESE ID
                //RETORNAMOS EL PRIMERO DISPONIBLE (1-2-3-4)
                return 1;
            }
           return disponible;
        }

        #endregion

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            fnLimpiarCampos();
            
            if (op.Cancela == false)
            {
                op.SetButtonProperties(btnNuevo, 2);
                op.Cancela = true;                
            }
            else
            {
                op.Cancela = false;
                op.SetButtonProperties(btnNuevo, 1);
                //CARGAR CAMPO
                fnCargarCampos(0);

              
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            if (txtIdImpuesto.Text == "") { lblError.Visible = true;lblError.Text = "Por favor ingrese un identificador"; return; }
            if (txtFactor.Text == "") { lblError.Visible = true; lblError.Text = "Por favor llene campo factor";return;}
            if (txtHasta.Text == "") { lblError.Visible = true; lblError.Text = "Por favor llene campo Hasta";return;}
            if (txtInicio.Text == "") { lblError.Visible = true;lblError.Text = "Por favor llene campo Inicio";return;}
            if (txtRebaja.Text == "") { lblError.Visible = true;lblError.Text = "Por favor llene campo Rebaja";return;}
            if (txtTope.Text == "") { lblError.Visible = true;lblError.Text = "Por favor llene campo Tope";return;}

            lblError.Visible = false;
            bool TopeCorrecto = false, FactorCorrecto = false, RebajaCorrecto = false;

            if (Update == false)
            {
                //INSERT
                //VERIFICAR QUE ID INGRESADO Y TRAMO NO EXISTAN COMO REGISTRO EN BD
                bool ExisteRegistro = false;
                ExisteRegistro = fnExisteIdTramo(int.Parse(txtIdImpuesto.Text), int.Parse(txtTramo.EditValue.ToString()));
                if (ExisteRegistro)
                {
                    //SI EXISTE REGISTRO NO PODEMOS DEJAR INGRESAR
                    lblError.Visible = true;
                    lblError.Text = "Registro ya existe";
                    return;
                }
                else
                {
                    lblError.Visible = false;
                    //VALIDO!!!
                    //VERIFICAR QUE CAMPOS TOPE, REBAJA, FACTO TENGAN FORMATO CORRECTO                    

                    TopeCorrecto = fnDecimal(txtTope.Text);
                    FactorCorrecto = fnDecimal(txtFactor.Text);
                    RebajaCorrecto = fnDecimal(txtRebaja.Text);

                    if (TopeCorrecto == false) { lblError.Visible = true;lblError.Text = "Valor erroneo para campo Tope"; return;}
                    if (FactorCorrecto == false) { lblError.Visible = true; lblError.Text = "Valor erroneo para campo Factor"; return;}
                    if (RebajaCorrecto == false) { lblError.Visible = true; lblError.Text = "Valor erroneo para campo Rebaja"; return;}

                    lblError.Visible = false;

                    fnNuevoImpuesto(txtIdImpuesto, txtTramo, txtTope, txtFactor, txtRebaja, txtInicio, txtHasta, txtdato1, txtdato2);
                }
            }
            else
            {
                //ES UPDATE
              
                TopeCorrecto = fnDecimal(txtTope.Text);
                FactorCorrecto = fnDecimal(txtFactor.Text);
                RebajaCorrecto = fnDecimal(txtRebaja.Text);

                if (TopeCorrecto == false) { lblError.Visible = true; lblError.Text = "Valor erroneo para campo Tope"; return; }
                if (FactorCorrecto == false) { lblError.Visible = true; lblError.Text = "Valor erroneo para campo Factor"; return; }
                if (RebajaCorrecto == false) { lblError.Visible = true; lblError.Text = "Valor erroneo para campo Rebaja"; return; }

                //OBTENER VALOR TRAMO DESDE GRILLA SELECCIONADA
                int tramo = 0;
                if (viewImpuesto.RowCount > 0)
                    tramo = Convert.ToInt32(viewImpuesto.GetFocusedDataRow()["tramo"]);

                fnModificarImpuesto(txtIdImpuesto, txtTramo, txtTope, txtFactor, txtRebaja, txtInicio, txtHasta, tramo, txtdato1, txtdato2);
            }
                 
        }

        private void GridImpuesto_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            fnCargarCampos();

            //GUARDAR LA POSICION DE LA FILA SELECCIONADA!
            PositionGrid = viewImpuesto.FocusedRowHandle;
        }

        private void GridImpuesto_KeyUp(object sender, KeyEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            fnCargarCampos();

            //GUARDAR LA POSICION DE LA FILA SELECCIONADA!
            PositionGrid = viewImpuesto.FocusedRowHandle;
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            if (txtIdImpuesto.Text == "" || txtTramo.EditValue.ToString() == "")
            {
                lblError.Visible = true;
                lblError.Text = "Registro no valido";
                return;
            }

            lblError.Visible = false;

            DialogResult dialogo = XtraMessageBox.Show("¿Desea realmente eliminar?", "Eliminacion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogo == DialogResult.Yes)
            {
                if (viewImpuesto.RowCount > 0)
                {
                    int id = int.Parse(viewImpuesto.GetFocusedDataRow()["idImptoUnico"].ToString());
                    int tramo = int.Parse(viewImpuesto.GetFocusedDataRow()["tramo"].ToString());
                    fnEliminarImpuesto(id, tramo);
                }
            }
        }

        private void txtTope_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (char.IsControl(e.KeyChar))
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

        private void txtFactor_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (char.IsControl(e.KeyChar))
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

        private void txtRebaja_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (char.IsControl(e.KeyChar))
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

        private void txtInicio_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtHasta_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtIdImpuesto_KeyDown(object sender, KeyEventArgs e)
        {
         
        }

        private void txtTope_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (txtTope.Text == "")
                {
                    lblError.Visible = true;
                    lblError.Text = "Por favor llenar campo Tope";
                    return;
                }
                else
                {
                    lblError.Visible = false;
                    //VALIDAR FORMATO VALIDO
                    bool formatoCorrecto = fnDecimal(txtTope.Text);
                    if (formatoCorrecto)
                    {
                        lblError.Visible = true;
                    }
                    else
                    {
                        lblError.Visible = false;
                        lblError.Text = "Valor erroneo para campo Tope";
                        return;
                    }
                }
            }
        }

        private void txtFactor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (txtFactor.Text == "")
                {
                    lblError.Visible = true;
                    lblError.Text = "Por favor llenar campo Factor";
                    return;
                }
                else
                {
                    lblError.Visible = false;
                    //VALIDAR FORMATO VALIDO
                    bool formatoCorrecto = fnDecimal(txtFactor.Text);
                    if (formatoCorrecto)
                    {
                        lblError.Visible = true;
                    }
                    else
                    {
                        lblError.Visible = false;
                        lblError.Text = "Valor erroneo para campo Factor";
                        return;
                    }
                }
            }
        }

        private void txtRebaja_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (txtRebaja.Text == "")
                {
                    lblError.Visible = true;
                    lblError.Text = "Por favor llenar campo Rebaja";
                    return;
                }
                else
                {
                    lblError.Visible = false;
                    //VALIDAR FORMATO VALIDO
                    bool formatoCorrecto = fnDecimal(txtRebaja.Text);
                    if (formatoCorrecto)
                    {
                        lblError.Visible = true;
                    }
                    else
                    {
                        lblError.Visible = false;
                        lblError.Text = "Valor erroneo para campo Rebaja";
                        return;
                    }
                }
            }
        }

        private void txtTramo_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtTope_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtFactor_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtRebaja_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtInicio_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtHasta_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            if (viewImpuesto.RowCount>0)
            {
                int tramo = Convert.ToInt32(viewImpuesto.GetFocusedDataRow()["tramo"]);
                int id = Convert.ToInt32(viewImpuesto.GetFocusedDataRow()["idimptounico"]);

                if (HayCambios(id, tramo))
                {
                    DialogResult adv = XtraMessageBox.Show("Hay datos sin guardar, ¿Deseas cerrar de todas formas?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (adv == DialogResult.Yes)
                    {
                        Close();
                    }
                }
                else
                    Close();
            }
            else
            {
                Close();
            }
        }

        private void txtdato1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar) || char.IsLetter(e.KeyChar))
            {
                e.Handled = false;
            }
            else
                e.Handled = true;
        }

        private void txtdato2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar) || char.IsLetter(e.KeyChar))
            {
                e.Handled = false;
            }
            else
                e.Handled = true;
        }

        private void txtTramo_EditValueChanged(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();
        }
    }
}