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
//PARA IMPORT DLL
using System.Runtime.InteropServices;
using DevExpress.XtraEditors.Controls;

namespace Labour
{
    public partial class frmItem : DevExpress.XtraEditors.XtraForm, IFormulaItem
    {
        [DllImport("user32")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32")]
        static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        const int MF_BYCOMMAND = 0;
        const int MF_DISABLED = 2;
        const int SC_CLOSE = 0xF060;

        #region "INTERFAZ COMUNICACION FORM FORMULA Y FORM ITEM"
        public void RecargarComboFormula()
        {
            fnComboFormula(txtformula);
        }
        public void RecargarGrillaTipo(int type)
        {
            fnBotonClick(type);
        }
        #endregion
        //VARIABLE PARA SABER EL TIPO DE ITEM QUE SE ESTA TRABAJANDO
        int TipoActual = 0;

        //VARIABLE PARA GUARDAR CODIGO DE FORMULA
        private string CodigoFormula = "";

        //BOTON NUEVO
        Operacion op;

        //PARA SABER SI ES ITEM DE SISTEM
        public bool ItemSistema { get; set; } = false;

        /// <summary>
        /// Codigo esquema seleccionado par acombo debito
        /// </summary>
        public int CodEsqDebito { get; set; }
        /// <summary>
        /// Codigo esquema seleccionado para combo credito
        /// </summary>
        public int CodEsqCredito { get; set; }
        /// <summary>
        /// Codigo cuenta seleccionada para debito
        /// </summary>
        public int CodCuentaDebito { get; set; }
        /// <summary>
        /// Codigo cuenta seleccionada para credito.
        /// </summary>
        public int CodCuentaCredito { get; set; }
        public frmItem()
        {
            InitializeComponent();
        }

        private void frmItem_Load(object sender, EventArgs e)
        {
            var sm = GetSystemMenu(Handle, false);
            EnableMenuItem(sm, SC_CLOSE, MF_BYCOMMAND | MF_DISABLED);

            op = new Operacion();
            //CARGAR COMBO
            fnComboTipo(txttipo);
            //PROPIEDADES POR DEFECTO
            fnDefaultProperties();

            ItemTrabajador.ComboModalidad(txtModalidad);

            //CARGAR COMBO FORMULA
            fnComboFormula(txtformula);

            TipoActual = 1;
            fnBotonClick(TipoActual);
            fnHaberActivo();

            //cbContabilidad.Checked = false;
            //DisableControl(false);
            ComboContabilidad();

        }
        #region "MANEJO DE DATOS"
        //INGRESO NUEVO ITEM
        /*
         * CAMPOS
         * ------------------
         * CODIGO
         * DESCRIPCION
         * TIPO
         * FORMULA
         * ORDEN
         * IMPRIME
         * INFORMACION
         * IMPRIME BASE
         * ------------------
         */
        private void fnIngresoItem(TextEdit pCod, TextEdit pDesc, LookUpEdit pTipo, LookUpEdit pFormula,
            TextEdit pOrden, CheckEdit pImprime, CheckEdit pInfo, CheckEdit pImpBase, int pTipoActual,
            CheckEdit cbSistema, CheckEdit cbPrevisualizar, CheckEdit cbTope, LookUpEdit pModalidad,
            LookUpEdit pComboDebito, LookUpEdit pComboCredito, TextEdit pDato1Credito, TextEdit pDato2Credito, 
            TextEdit pDato3Credito, TextEdit pDato4Credito, TextEdit pDato1Debito, TextEdit pDato2Debito, 
            TextEdit pDato3Debito, TextEdit pDato4Debito, CheckEdit pCheckContab)
        {
            //sql insert
            string sql = "INSERT INTO item(coditem, descripcion, tipo, formula, orden, imprime, " +
                "informacion, imprimeBase, proporcional, sistema, previsualizar, tope, modalidad)" +
                " VALUES(@pCod, @pDesc, @pTipo, @pFormula, @pOrden, @pImprime, @pInfo, @pImpBase, " +
                "@proporcional, @psistema, @pPrevisualizar, @pTope, @pModalidad)";

            string sqlContable = "INSERT INTO GrupoContable(codEs, coditem, tipocon, codCuenta, dato1, dato2, dato3, dato4) " +
                         "VALUES(@pCodEs, @pCodItem, @pTipo, @pCodCuenta, @pDato1, @pDato2, @pDato3, @pDato4)";


            SqlCommand cmd;
            SqlConnection cn;
            SqlTransaction tr;

            ///Para llenar grilla de acuerdo a tipo .
            string SqlGrilla = "SELECT coditem, descripcion, tipo, formula, " +
                                "orden, imprime, informacion, imprimeBase, proporcional, " +
                                "sistema, previsualizar, tope, modalidad " +
                                "FROM item WHERE tipo=" + pTipoActual + " ORDER BY orden, coditem";

            bool TransaccionCorrecta = false;

            int res = 0, count = 0;      

            string formula = "0";
            if (pFormula.EditValue.ToString() != "0")
                formula = pFormula.EditValue.ToString();

            if (cbSistema.Checked)
            {
                DialogResult advertencia = XtraMessageBox.Show("Si guardas el item como item de sistema no podrás eliminar el item posteriormente, ¿Deseas continuar de todas maneras?", "Informacion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (advertencia == DialogResult.No)
                    return;
            }

            if (pCheckContab.Checked)
            {
                if (pComboCredito.EditValue == null || pComboCredito.Properties.DataSource == null)
                { XtraMessageBox.Show("Por favor selecciona una cuenta contable para Credito", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                if (pComboDebito.EditValue == null || pComboDebito.Properties.DataSource == null)
                { XtraMessageBox.Show("Por favor selecciona una cuenta contable para Debito", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
            }


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
                            //INGRESAR ITEM
                            using (cmd = new SqlCommand(sql, cn))
                            {
                                cmd.Parameters.Add(new SqlParameter("@pCod", pCod.Text.ToUpper()));
                                cmd.Parameters.Add(new SqlParameter("@pDesc", pDesc.Text.ToUpper()));
                                cmd.Parameters.Add(new SqlParameter("@pTipo", pTipo.EditValue));
                                cmd.Parameters.Add(new SqlParameter("@pFormula", formula));
                                cmd.Parameters.Add(new SqlParameter("@pOrden", pOrden.Text));
                                cmd.Parameters.Add(new SqlParameter("@pImprime", pImprime.Checked));
                                cmd.Parameters.Add(new SqlParameter("@pInfo", pInfo.Checked));
                                cmd.Parameters.Add(new SqlParameter("@pImpBase", pImpBase.Checked));
                                cmd.Parameters.Add(new SqlParameter("@proporcional", cbproporcional.Checked));
                                cmd.Parameters.Add(new SqlParameter("@psistema", cbSistema.Checked));
                                cmd.Parameters.Add(new SqlParameter("@pPrevisualizar", cbPrevisualizar.Checked));
                                cmd.Parameters.Add(new SqlParameter("@pTope", cbTope.Checked));
                                cmd.Parameters.Add(new SqlParameter("@pModalidad", Convert.ToInt16(pModalidad.EditValue)));

                                cmd.Transaction = tr;
                                count = cmd.ExecuteNonQuery();

                                if (count > 0)
                                {
                                    //GUARDAR EVENTO EN LOG
                                    logRegistro log = new logRegistro(User.getUser(), "INGRESA NUEVO ITEM " + pCod.Text, "ITEM", "0", pCod.Text, "INGRESAR");
                                    log.Log();
                                }
                            }

                            //INGRESO DATOS CONTABLES
                            //SOLO SI CHECK ES TRUE
                            if (pCheckContab.Checked)
                            {
                                if (Convert.ToInt32(pComboDebito.EditValue) != 0)
                                {
                                    //PARA DEBITO
                                    using (cmd = new SqlCommand(sqlContable, cn))
                                    {
                                        //PARAMETROS
                                        cmd.Parameters.Add(new SqlParameter("@pCodEs", CodEsqDebito));
                                        cmd.Parameters.Add(new SqlParameter("@pCodItem", pCod.Text));
                                        cmd.Parameters.Add(new SqlParameter("@pTipo", 1));
                                        cmd.Parameters.Add(new SqlParameter("@pCodCuenta", CodCuentaDebito));
                                        cmd.Parameters.Add(new SqlParameter("@pDato1", pDato1Debito.Text));
                                        cmd.Parameters.Add(new SqlParameter("@pDato2", pDato2Debito.Text));
                                        cmd.Parameters.Add(new SqlParameter("@pDato3", pDato3Debito.Text));
                                        cmd.Parameters.Add(new SqlParameter("@pDato4", pDato4Debito.Text));

                                        cmd.Transaction = tr;
                                        count = cmd.ExecuteNonQuery();
                                        if (count > 0)
                                        {
                                            logRegistro Log = new logRegistro(User.getUser(), $"SE INGRESA ITEM A GRUPOCONTABLE CON CODIGO {pCod.Text} Y TIPO {pTipo}", "GRUPOCONTABLE", "", pCod.Text, "INGRESAR");
                                            Log.Log();
                                        }
                                    }
                                }

                                if (Convert.ToInt32(pComboCredito.EditValue) != 0)
                                {
                                    //PARA CREDITO
                                    using (cmd = new SqlCommand(sqlContable, cn))
                                    {
                                        //PARAMETROS
                                        cmd.Parameters.Add(new SqlParameter("@pCodEs", CodEsqCredito));
                                        cmd.Parameters.Add(new SqlParameter("@pCodItem", pCod.Text));
                                        cmd.Parameters.Add(new SqlParameter("@pTipo", 2));
                                        cmd.Parameters.Add(new SqlParameter("@pCodCuenta", CodCuentaCredito));
                                        cmd.Parameters.Add(new SqlParameter("@pDato1", pDato1Credito.Text));
                                        cmd.Parameters.Add(new SqlParameter("@pDato2", pDato2Credito.Text));
                                        cmd.Parameters.Add(new SqlParameter("@pDato3", pDato3Credito.Text));
                                        cmd.Parameters.Add(new SqlParameter("@pDato4", pDato4Credito.Text));

                                        cmd.Transaction = tr;
                                        count = cmd.ExecuteNonQuery();
                                        if (count > 0)
                                        {
                                            logRegistro Log = new logRegistro(User.getUser(), $"SE INGRESA ITEM A GRUPOCONTABLE CON CODIGO {pCod.Text} Y TIPO {pTipo}", "GRUPOCONTABLE", "", pCod.Text, "INGRESAR");
                                            Log.Log();
                                        }
                                    }
                                }                             
                            }

                            tr.Commit();
                            TransaccionCorrecta = true;
                        }
                        catch (SqlException ex)
                        {
                            //ERROR...
                            tr.Rollback();
                            TransaccionCorrecta = false;
                        }
                    }
                }

                //if (fnSistema.ConectarSQLServer())
                //{
                //    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                //    {
                //        //PARAMETROS
                //        cmd.Parameters.Add(new SqlParameter("@pCod", pCod.Text.ToUpper()));
                //        cmd.Parameters.Add(new SqlParameter("@pDesc", pDesc.Text.ToUpper()));
                //        cmd.Parameters.Add(new SqlParameter("@pTipo", pTipo.EditValue));
                //        cmd.Parameters.Add(new SqlParameter("@pFormula", formula));
                //        cmd.Parameters.Add(new SqlParameter("@pOrden", pOrden.Text));
                //        cmd.Parameters.Add(new SqlParameter("@pImprime", v1));
                //        cmd.Parameters.Add(new SqlParameter("@pInfo", v2));
                //        cmd.Parameters.Add(new SqlParameter("@pImpBase", v3));
                //        cmd.Parameters.Add(new SqlParameter("@proporcional", proporcional));
                //        cmd.Parameters.Add(new SqlParameter("@psistema", cbSistema.Checked));
                //        cmd.Parameters.Add(new SqlParameter("@pPrevisualizar", cbPrevisualizar.Checked));
                //        cmd.Parameters.Add(new SqlParameter("@pTope", cbTope.Checked));
                //        cmd.Parameters.Add(new SqlParameter("@pModalidad", Convert.ToInt16(pModalidad.EditValue)));
                //        cmd.Parameters.Add(new SqlParameter("@pDato1", pDato1.Text));
                //        cmd.Parameters.Add(new SqlParameter("@pDato2", pDato2.Text));

                //        //EJECUTAR CONSULT
                //        res = cmd.ExecuteNonQuery();
                //        if (res > 0)
                //        {
                //            XtraMessageBox.Show("Ingreso correcto", "Guardar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //            //GUARDAR EVENTO EN LOG
                //            logRegistro log = new logRegistro(User.getUser(), "INGRESA NUEVO ITEM " + pCod.Text, "ITEM", "0", pCod.Text, "INGRESAR");
                //            log.Log();

                //            fnSistema.spllenaGridView(gridItem, "SELECT coditem, descripcion, tipo, formula, " +
                //                "orden, imprime, informacion, imprimeBase, proporcional, " +
                //                "sistema, previsualizar, tope, modalidad, dato1, dato2 " +
                //                "FROM item WHERE tipo=" + pTipoActual + " ORDER BY orden, coditem");
                //            //fnCargarCampos(viewItem, 0);
                           
                //        }
                //        else
                //        {
                //            XtraMessageBox.Show("Error al intentar guardar", "Guardar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //        }
                //    }

                //    //LIBERAMOS MEMORIA
                //    cmd.Dispose();
                //    fnSistema.sqlConn.Close();
                //}
                //else
                //{
                //    XtraMessageBox.Show("No hay conexion con base de datos", "Base de Datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //}
            }
            catch (SqlException ex)
            {
                //XtraMessageBox.Show(ex.Message);
                TransaccionCorrecta = false;
            }

            //Transaccion correcta???
            if (TransaccionCorrecta)
            {
                XtraMessageBox.Show($"Item {pCod.Text} ingresado correctamente", "Item", MessageBoxButtons.OK, MessageBoxIcon.Information);

                fnSistema.spllenaGridView(gridItem, SqlGrilla);
                if (viewItem.RowCount > 0)
                {
                    fnSistema.spOpcionesGrilla(viewItem);
                    fnCargarCampos(viewItem, 0);
                }
            }
            else
            {
                XtraMessageBox.Show("No se pudo guardar item", "Item", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        //METODO PARA MODIFICAR UN ITEM
        private void fnModificarItem(TextEdit pCod, TextEdit pDesc, LookUpEdit pTipo, LookUpEdit pFormula,
            TextEdit pOrden, CheckEdit pImprime, CheckEdit pInfo, CheckEdit pImpBase, int pTipoActual,
            CheckEdit cbSistema, CheckEdit pPrevisualizar, CheckEdit pTope, LookUpEdit pModalidad,            
             LookUpEdit pComboDebito, LookUpEdit pComboCredito, TextEdit pDato1Credito, TextEdit pDato2Credito,
            TextEdit pDato3Credito, TextEdit pDato4Credito, TextEdit pDato1Debito, TextEdit pDato2Debito,
            TextEdit pDato3Debito, TextEdit pDato4Debito, CheckEdit pCheckContab)
        {
            string sql = "UPDATE item SET descripcion=@pDesc, tipo=@pTipo, formula=@pFormula," +
                "orden=@pOrden, imprime=@pImprime, informacion=@pInfo, imprimeBase=@pImpBase, " +
                "proporcional=@proporcional, sistema=@pSistema, previsualizar=@pPrevisualizar, " +
                "tope=@pTope, modalidad=@pModalidad " +
                "WHERE coditem=@pCod";


            string sqlContable = "INSERT INTO GrupoContable(codEs, coditem, tipocon, codCuenta, dato1, dato2, dato3, dato4) " +
                         "VALUES(@pCodEs, @pCodItem, @pTipo, @pCodCuenta, @pDato1, @pDato2, @pDato3, @pDato4)";

            string SqlGrilla = "SELECT coditem, descripcion, tipo, formula, orden, imprime," +
                                " informacion, imprimeBase, proporcional, sistema, previsualizar, " +
                                "tope, modalidad FROM item WHERE tipo=" + pTipoActual + " ORDER BY orden, coditem";

            string SqlExiste = "SELECT count(*) FROM GrupoContable WHERE coditem = @pCodItem";
            string SqlDelContable = "DELETE FROM grupoContable where coditem=@pCodItem";

            string sqlItemTrab = "UPDATE itemtrabajador SET tipo=@pTipo, orden=@pOrden " +
                "WHERE coditem=@pItem AND anomes=@pPeriodo";

            //string sqlItemTrab = "UPDATE itemtrabajador SET tipo=@pTipo, orden=@pOrden, formula=@pFormula " +
            //    "WHERE coditem=@pItem AND anomes=@pPeriodo";

            SqlCommand cmd;
            SqlConnection cn;
            SqlTransaction tran;
            bool TransaccionCorrecta = false, ExisteDebito = false, ExisteCredito = false;
            bool DelCorrecto = false, Existe = false;
            int count = 0;

            string formula = "0";
            if (pFormula.EditValue.ToString() != "0")
                formula = pFormula.EditValue.ToString();

            if (cbSistema.Checked)
            {
                DialogResult advertencia = XtraMessageBox.Show("Si guardas el item como item de sistema no podrás realizar nuevas modificaciones, ¿Deseas continuar de todas maneras?", "Informacion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (advertencia == DialogResult.No)
                    return;
            }

            //TABLA HASH PARA ITEM
            Hashtable DatosItem = PrecargaItem(pCod.Text);

            GrupoContable contable = new GrupoContable();

            if (pCheckContab.Checked)
            {
                if (pComboCredito.EditValue == null || pComboCredito.Properties.DataSource == null)
                { XtraMessageBox.Show("Por favor selecciona una cuenta contable para Credito", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                if (pComboDebito.EditValue == null || pComboDebito.Properties.DataSource == null)
                { XtraMessageBox.Show("Por favor selecciona una cuenta contable para Debito", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
            }

            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        tran = cn.BeginTransaction();
                        try
                        {  
                            //ACTUALIZAR ITEM
                            using (cmd = new SqlCommand(sql, cn))
                            {
                                cmd.Parameters.Add(new SqlParameter("@pCod", pCod.Text.ToUpper()));
                                cmd.Parameters.Add(new SqlParameter("@pDesc", pDesc.Text.ToUpper()));
                                cmd.Parameters.Add(new SqlParameter("@pTipo", pTipo.EditValue));
                                cmd.Parameters.Add(new SqlParameter("@pFormula", formula));
                                cmd.Parameters.Add(new SqlParameter("@pOrden", pOrden.Text));
                                cmd.Parameters.Add(new SqlParameter("@pImprime", pImprime.Checked == false ? 0:1));
                                cmd.Parameters.Add(new SqlParameter("@pInfo", pInfo.Checked == false ? 0:1));
                                cmd.Parameters.Add(new SqlParameter("@pImpBase", pImpBase.Checked == false ? 0:1));
                                cmd.Parameters.Add(new SqlParameter("@proporcional", cbproporcional.Checked == false ?0:1));
                                cmd.Parameters.Add(new SqlParameter("@pSistema", cbSistema.Checked));
                                cmd.Parameters.Add(new SqlParameter("@pPrevisualizar", pPrevisualizar.Checked));
                                cmd.Parameters.Add(new SqlParameter("@pTope", pTope.Checked));
                                cmd.Parameters.Add(new SqlParameter("@pModalidad", Convert.ToInt16(pModalidad.EditValue)));

                                //Agregamos a transaccion...
                                cmd.Transaction = tran;
                                count = cmd.ExecuteNonQuery();
                                if (count > 0)
                                {
                                    //Comparamos valores para Log.
                                    ComparaValorItem(pCod.Text, pDesc.Text, Convert.ToInt32(pTipo.EditValue), formula, int.Parse(pOrden.Text),
                                    pImprime.Checked, pInfo.Checked, pImpBase.Checked, cbproporcional.Checked, cbSistema.Checked,
                                    pPrevisualizar.Checked, cbTope.Checked, Convert.ToInt16(txtModalidad.EditValue),
                                    DatosItem);
                                }
                            }

                            if (cbContabilidad.Checked)
                            {
                                //PREGUNTAMOS SI HAY ELEMENTOS EN TABLA
                                using (cmd = new SqlCommand(SqlExiste, cn))
                                {
                                    cmd.Parameters.Add(new SqlParameter("@pCodItem", pCod.Text));

                                    cmd.Transaction = tran;
                                    if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                                        Existe = true;

                                }

                                if (Existe)
                                {
                                    //ELIMINAMOS DATOS EXISTENTES
                                    using (cmd = new SqlCommand(SqlDelContable, cn))
                                    {
                                        cmd.Parameters.Add(new SqlParameter("@pCodItem", pCod.Text));

                                        cmd.Transaction = tran;
                                        count = cmd.ExecuteNonQuery();
                                        if (count > 0)
                                            DelCorrecto = true;
                                    }
                                }
                                else
                                {
                                    DelCorrecto = true;
                                }

                                if (DelCorrecto)
                                {
                                    if (Convert.ToInt32(pComboDebito.EditValue) != 0)
                                    {
                                        //ACTUALIZAR DATOS GRUPO CONTABLE
                                        using (cmd = new SqlCommand(sqlContable, cn))
                                        {
                                            cmd.Parameters.Add(new SqlParameter("@pCodEs", CodEsqDebito));
                                            cmd.Parameters.Add(new SqlParameter("@pCodItem", pCod.Text));
                                            cmd.Parameters.Add(new SqlParameter("@pTipo", 1));
                                            cmd.Parameters.Add(new SqlParameter("@pCodCuenta", CodCuentaDebito));
                                            cmd.Parameters.Add(new SqlParameter("@pDato1", pDato1Debito.Text));
                                            cmd.Parameters.Add(new SqlParameter("@pDato2", pDato2Debito.Text));
                                            cmd.Parameters.Add(new SqlParameter("@pDato3", pDato3Debito.Text));
                                            cmd.Parameters.Add(new SqlParameter("@pDato4", pDato4Debito.Text));

                                            cmd.Transaction = tran;
                                            count = cmd.ExecuteNonQuery();

                                           //Guardar datos en log
                                        }
                                    }

                                    if (Convert.ToInt32(pComboCredito.EditValue) != 0)
                                    {
                                        using (cmd = new SqlCommand(sqlContable, cn))
                                        {
                                            cmd.Parameters.Add(new SqlParameter("@pCodEs", CodEsqCredito));
                                            cmd.Parameters.Add(new SqlParameter("@pCodItem", pCod.Text));
                                            cmd.Parameters.Add(new SqlParameter("@pTipo", 2));
                                            cmd.Parameters.Add(new SqlParameter("@pCodCuenta", CodCuentaCredito));
                                            cmd.Parameters.Add(new SqlParameter("@pDato1", pDato1Credito.Text));
                                            cmd.Parameters.Add(new SqlParameter("@pDato2", pDato2Credito.Text));
                                            cmd.Parameters.Add(new SqlParameter("@pDato3", pDato3Credito.Text));
                                            cmd.Parameters.Add(new SqlParameter("@pDato4", pDato4Credito.Text));

                                            cmd.Transaction = tran;
                                            count = cmd.ExecuteNonQuery();

                                            //Guardar datos en log.
                                          
                                        }
                                    }                                   
                                }                               
                            }
                            else
                            {
                                //ELIMINAMOS DATOS EXISTENTES
                                using (cmd = new SqlCommand(SqlDelContable, cn))
                                {
                                    cmd.Parameters.Add(new SqlParameter("@pCodItem", pCod.Text));

                                    cmd.Transaction = tran;
                                    count = cmd.ExecuteNonQuery();
                                    if (count > 0)
                                        DelCorrecto = true;
                                }
                            }

                            //Actualizar información en itemtrabajador (Si cambia orden o tipo o formula)
                            using (cmd = new SqlCommand(sqlItemTrab, cn))
                            {
                                //PARAMETROS
                                
                                cmd.Parameters.Add(new SqlParameter("@pTipo", Convert.ToInt32(pTipo.EditValue)));
                                cmd.Parameters.Add(new SqlParameter("@pOrden", Convert.ToInt32(pOrden.Text)));
                                cmd.Parameters.Add(new SqlParameter("@pItem", pCod.Text));                                
                                cmd.Parameters.Add(new SqlParameter("@pPeriodo", Calculo.PeriodoObservado));
                                //cmd.Parameters.Add(new SqlParameter("@pFormula", pFormula.EditValue.ToString()));

                                cmd.Transaction = tran;
                                count = cmd.ExecuteNonQuery();
                            }

                            tran.Commit();
                            TransaccionCorrecta = true;
                        }
                        catch (SqlException ex)
                        {
                            tran.Rollback();
                            TransaccionCorrecta = true;
                        }                      
                    }
                }


                //if (fnSistema.ConectarSQLServer())
                //{
                //    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                //    {
                //        //PARAMETROS
                //        //PARAMETROS
                //        cmd.Parameters.Add(new SqlParameter("@pCod", pCod.Text.ToUpper()));
                //        cmd.Parameters.Add(new SqlParameter("@pDesc", pDesc.Text.ToUpper()));
                //        cmd.Parameters.Add(new SqlParameter("@pTipo", pTipo.EditValue));
                //        cmd.Parameters.Add(new SqlParameter("@pFormula", formula));
                //        cmd.Parameters.Add(new SqlParameter("@pOrden", pOrden.Text));
                //        cmd.Parameters.Add(new SqlParameter("@pImprime", v1));
                //        cmd.Parameters.Add(new SqlParameter("@pInfo", v2));
                //        cmd.Parameters.Add(new SqlParameter("@pImpBase", v3));
                //        cmd.Parameters.Add(new SqlParameter("@proporcional", proporcional));
                //        cmd.Parameters.Add(new SqlParameter("@pSistema", cbSistema.Checked));
                //        cmd.Parameters.Add(new SqlParameter("@pPrevisualizar", pPrevisualizar.Checked));
                //        cmd.Parameters.Add(new SqlParameter("@pTope", pTope.Checked));
                //        cmd.Parameters.Add(new SqlParameter("@pModalidad", Convert.ToInt16(pModalidad.EditValue)));
                //        cmd.Parameters.Add(new SqlParameter("@pDato1", pDato1.Text));
                //        cmd.Parameters.Add(new SqlParameter("@pDato2", pDato2.Text));

                //        //EJECUTAR CONSULTA
                //        res = cmd.ExecuteNonQuery();
                //        if (res > 0)
                //        {
                //            XtraMessageBox.Show("Actualizacion correcta", "Actualizar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //            //GUARDAR EVENTO EN LOG                           

                //            //COMPARA SI HAY CAMBIOS Y LOS GUARDA EN LOG REGISTROS
                //            ComparaValorItem(pCod.Text, pDesc.Text, Convert.ToInt32(pTipo.EditValue), formula, int.Parse(pOrden.Text),
                //              pImprime.Checked, pInfo.Checked, pImpBase.Checked, cbproporcional.Checked, cbSistema.Checked,
                //              pPrevisualizar.Checked, cbTope.Checked, Convert.ToInt16(txtModalidad.EditValue),
                //              DatosItem, pDato1.Text, pDato2.Text);

                //            //MODIFICAR VALORES EN ITEM DE TRABAJADOR
                //            ModificarItemTrabajador(Convert.ToInt32(pOrden.Text), formula, Convert.ToInt32(pTipo.EditValue), pCod.Text);

                //            fnSistema.spllenaGridView(gridItem, "SELECT coditem, descripcion, tipo, formula, orden, imprime," +
                //                " informacion, imprimeBase, proporcional, sistema, previsualizar, " +
                //                "tope, modalidad, dato1, dato2 FROM item WHERE tipo=" + pTipoActual + " ORDER BY orden, coditem");
                //            fnCargarCampos(viewItem, 0);
                //        }
                //        else
                //        {
                //            XtraMessageBox.Show("Error al intentar Actualizar", "Actualizar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //        }
                //    }

                //    //LIBERAR MEMORIA
                //    cmd.Dispose();
                //    fnSistema.sqlConn.Close();
                //}
                //else
                //{
                //    XtraMessageBox.Show("No hay conexion con Base de datos", "Base de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //}
            }
            catch (SqlException ex)
            {
                //XtraMessageBox.Show(ex.Message);
                TransaccionCorrecta = false;
            }

            if (TransaccionCorrecta)
            {
                XtraMessageBox.Show($"Registro {pCod.Text} actualizado correctamente", "Item", MessageBoxButtons.OK, MessageBoxIcon.Information);

                fnSistema.spllenaGridView(gridItem, SqlGrilla);
                if (viewItem.RowCount > 0)
                {
                    fnSistema.spOpcionesGrilla(viewItem);
                    fnCargarCampos(viewItem, 0);
                }
            }
            else
            {
                XtraMessageBox.Show("No se pudo realizar actualizacion", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }

        }

        //SOLO MODIFICAR ITEM 
        private void fnModImprime(CheckEdit pImprime, CheckEdit pImpBase, CheckEdit pInfo, CheckEdit pProp, TextEdit pCod, int pType, CheckEdit pPrevisualizar, CheckEdit pTope, LookUpEdit pModalidad)
        {
            string sql = "UPDATE ITEM SET imprime=@pImprime, informacion=@pInformacion, " +
                         "imprimebase=@pImprimeBase, proporcional=@pProp, previsualizar=@pPrevisualizar, tope=@pTope, modalidad=@pModalidad WHERE " +
                         "coditem = @pItem";
            SqlCommand cmd;
            SqlConnection cn;
            int count = 0;

            //TABLA HASH PARA ITEM
            Hashtable DatosItem = PrecargaOpciones(pCod.Text);

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
                            cmd.Parameters.Add(new SqlParameter("@pItem", pCod.Text));
                            cmd.Parameters.Add(new SqlParameter("@pImprime", pImprime.Checked));
                            cmd.Parameters.Add(new SqlParameter("@pInformacion", pInfo.Checked));
                            cmd.Parameters.Add(new SqlParameter("@pImprimeBase", pImpBase.Checked));
                            cmd.Parameters.Add(new SqlParameter("@pProp", pProp.Checked));
                            cmd.Parameters.Add(new SqlParameter("@pPrevisualizar", pPrevisualizar.Checked));
                            cmd.Parameters.Add(new SqlParameter("@pTope", pTope.Checked));
                            cmd.Parameters.Add(new SqlParameter("@pModalidad", Convert.ToInt16(pModalidad.EditValue)));

                            count = cmd.ExecuteNonQuery();
                            if (count > 0)
                            {
                                XtraMessageBox.Show("Actualizacion realizada correctamente", "Actualizar", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                //COMPARAR SOLO PARA LOG
                                ComparaOpciones(pCod.Text, pImprime.Checked, pProp.Checked, pInfo.Checked, pImpBase.Checked, pPrevisualizar.Checked, cbTope.Checked, Convert.ToInt16(pModalidad.EditValue), DatosItem);

                                fnSistema.spllenaGridView(gridItem, "SELECT coditem, descripcion, tipo, formula, orden, imprime, informacion, imprimeBase, proporcional, sistema, previsualizar, tope, modalidad FROM item WHERE tipo=" + pType + " ORDER BY orden");
                                fnCargarCampos(viewItem, 0);
                            }
                            else
                            {
                                XtraMessageBox.Show($"No se pudo actualizar item {pCod.Text}", "Actualizar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                        cmd.Dispose();
                    }
                }
            }
            catch (SqlException ex)
            {
                //ERROR
            }
        }


        //VERSION 2 DEL METODO PARA ELIMINAR (USANDO TRANSACCIONSQL)
        private bool EliminarItemTransaccion(TextEdit pCod, int pTipoActual)
        {
            //PARA VERIFICAR SI ESTA ASOCIADO A OTROS REGISTROS 
            bool usaClase = false, usaTrab = false, usaLib = false;
            bool tranCorrecta = false;

            usaClase = fnitemClase(pCod.Text);
            usaTrab = Persona.ItemUsado(pCod.Text);
            usaLib = Libro.ItemUsado(pCod.Text);

            //SI ESTA SIENDO USADO COMO ITEM DE CLASE NO SE PUEDE ELIMINAR
            if (usaClase)
            { XtraMessageBox.Show("Este item esta siendo utilizado en otro proceso por lo que no es posible eliminar", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); return false; }

            //SI EL ITEM ESTA SIENDO USADO POR UN TRABAJADOR NO SE PUEDE ELIMINAR
            if (usaTrab)
            { XtraMessageBox.Show("Este item esta siendo utilizado por un trabajador por lo que no es posible eliminar", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); return false; }

            //ITEM EN TABLA LIBRO
            if (usaLib)
            { XtraMessageBox.Show("Item en uso", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

            //SQL PARA ELIMINAR ITEMTRABAJADOR
            string sqlTrabajador = "DELETE FROM itemtrabajador WHERE coditem=@pItem AND anomes=@periodo";

            //SQL PARA ELIMINAR ITEMCLASE
            string sqlClase = "DELETE FROM ITEMCLASE WHERE item=@pItem";

            //Para eliminar de tabla grupocontable
            string sqlDelContabilidad = "DELETE FROM grupocontable WHERE coditem=@pItem";

            //QUERY DELETE
            string sqlItem = "DELETE FROM item WHERE coditem=@pCod";

            SqlCommand cmd;
            SqlTransaction tran;

            DialogResult Advertencia = XtraMessageBox.Show("¿Está seguro que desea eliminar este item? Esta operacion puede afectar a otros procesos", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (Advertencia == DialogResult.Yes)
            {
                try
                {
                    if (fnSistema.ConectarSQLServer())
                    {
                        tran = fnSistema.sqlConn.BeginTransaction();

                        try
                        {
                            //SI EL ITEM ESTA ASOCIADO A ITEM DE TRABAJADOR ELIMINAMOS
                            if (usaTrab)
                            {
                                cmd = new SqlCommand(sqlTrabajador, fnSistema.sqlConn);

                                //PARAMETROS
                                cmd.Parameters.Add(new SqlParameter("@pItem", pCod.Text));
                                cmd.Parameters.Add(new SqlParameter("@periodo", Calculo.PeriodoObservado));

                                //TRABAJANDO COMO UNA TRANSACCION
                                cmd.Transaction = tran;

                                //EJECUTAMOS LA CONSULTA
                                cmd.ExecuteNonQuery();

                                cmd.Dispose();
                            }
                            //SI EL ITEM ESTA SIENDO USADO COMO ITEMCLASE ELIMINAMOS DESDE TABLA ITEMCLASE
                            if (usaClase)
                            {
                                cmd = new SqlCommand(sqlClase, fnSistema.sqlConn);

                                //PARAMETROS
                                cmd.Parameters.Add(new SqlParameter("@pItem", pCod.Text));

                                cmd.Transaction = tran;

                                //EJECUTAMOS
                                cmd.ExecuteNonQuery();

                                cmd.Dispose();
                            }

                            ///Eliminamos datos de tabla grupocontable
                            using (cmd = new SqlCommand(sqlDelContabilidad, fnSistema.sqlConn))
                            {
                                cmd.Parameters.Add(new SqlParameter("@pItem", pCod.Text));
                                cmd.Transaction = tran;
                                cmd.ExecuteNonQuery();
                            }

                            //ELIMINAMOS DE TABLA ITEM
                            using (cmd = new SqlCommand(sqlItem, fnSistema.sqlConn))
                            {
                                //PARAMETROS
                                cmd.Parameters.Add(new SqlParameter("@pCod", pCod.Text));

                                //AGREGAMOS A TRANSACCION
                                cmd.Transaction = tran;

                                cmd.ExecuteNonQuery();

                                cmd.Dispose();
                            }

                            //LLEGAMOS A ESTE PUNTO HACEMOS COMMIT
                            tran.Commit();

                            fnSistema.sqlConn.Close();
                            tranCorrecta = true;

                        }
                        catch (Exception ex)
                        {
                            //SI HAY ALGUNA EXCEPCION HACEMOS ROLL BACK
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

            //SI LA TRANSACCION SE HIZO DE MANERA CORRECTA GUARDAMOS REGISTRO EN LOG Y ACTUALIZAMOS GRILLA
            if (tranCorrecta)
            {
                logRegistro log = new logRegistro(User.getUser(), "SE HA ELIMINADO ITEM " + pCod.Text, "ITEM", pCod.Text, "0", "ELIMINAR");
                log.Log();

                fnSistema.spllenaGridView(gridItem, "SELECT coditem, descripcion, tipo, formula," +
                    " orden, imprime, informacion, imprimeBase, proporcional, sistema," +
                    " previsualizar, tope, modalidad FROM item WHERE tipo=" + pTipoActual + " ORDER BY orden, coditem");
                fnCargarCampos(viewItem, 0);
            }

            return tranCorrecta;
        }

        //BUSCAR CODIGO ITEM EN BD
        //RETORNA TRUE SI ENCUENTRA EL REGISTRO
        //RETORNA FALSE SI NO ENCUENTRA NADA
        private bool fnBuscarItem(TextEdit pCod)
        {
            //SQL SEARCH
            string sql = "SELECT coditem FROM item WHERE coditem=@pCod";
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
                        cmd.Parameters.Add(new SqlParameter("@pCod", pCod.Text));
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //SI HAY FILAS ES PORQUE EL CODIGO EXISTE EN BD
                            //RETORNAMOS TRUE
                            encontrado = true;
                        }
                        else
                        {
                            encontrado = false;
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
            return encontrado;
        }

        //COLUMNAS GRILLA
        private void fnColumnas(DevExpress.XtraGrid.Views.Grid.GridView pGrid)
        {
            //SELECT coditem, descripcion, tipo, formula, orden, imprime, informacion, imprimeBase
            //CODIGO
            if (pGrid.RowCount > 0)
            {
                pGrid.Columns[0].Caption = "Codigo";
                pGrid.Columns[0].AppearanceCell.FontStyleDelta = FontStyle.Bold;
                pGrid.Columns[0].Width = 50;
                pGrid.Columns[1].Caption = "Descripcion";
                pGrid.Columns[1].Width = 200;

                pGrid.Columns[2].Caption = "Tipo";
                pGrid.Columns[2].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                pGrid.Columns[2].DisplayFormat.FormatString = "Tipo";
                pGrid.Columns[2].DisplayFormat.Format = new FormatCustom();

                pGrid.Columns[3].Caption = "Formula";

                pGrid.Columns[4].Caption = "Orden";

                //COLUMNAS OCULTAS SOLO PARA CARGAR CAMPOS 
                pGrid.Columns[5].Visible = false;
                pGrid.Columns[6].Visible = false;
                pGrid.Columns[7].Visible = false;
                pGrid.Columns[8].Visible = false;
                pGrid.Columns[9].Visible = false;
                pGrid.Columns[10].Visible = false;
                pGrid.Columns[11].Visible = false;
                pGrid.Columns[12].Visible = false;

            }

        }

        //OPCIONES FORMULARIO
        private void fnDefaultProperties()
        {
            txtcodigo.Properties.MaxLength = 7;
            txtdescripcion.Properties.MaxLength = 50;
            txtorden.Properties.MaxLength = 3;
            btnFormula.TabStop = false;
            btnNuevo.AllowFocus = false;
            btnGuardar.AllowFocus = false;
            btnEliminar.AllowFocus = false;
            btnContribuciones.AllowFocus = false;
            btnDescuento.AllowFocus = false;
            btnExento.AllowFocus = false;
            btnFamiliar.AllowFocus = false;
            btnHaber.AllowFocus = false;
            btnLegal.AllowFocus = false;
            btnTipoTotales.AllowFocus = false;
            separador1.TabStop = false;
            separador2.TabStop = false;
            gridItem.TabStop = false;
            panelGrid.TabStop = false;

            //PARA LOS CHECKBOX
            //POR DEFECTO 
            //IMPRIME SELECCIONADO SI
            //INFORMACION SELECCIONADO NO
            //IMPRIME BASE SELECCIONADO NO
            cbimprime.Checked = false;
            cbinfo.Checked = false;
            cbbase.Checked = false;
            cbimprime.TabStop = false;
            cbbase.TabStop = false;
            cbinfo.TabStop = false;
            txttipo.ItemIndex = 0;
        }

        //CARGAR CAMPOS
        //AL SELECCIONAR UNA FILA DE LA GRILLA CARGAR DATOS EN FORMULARIO (para update)
        private void fnCargarCampos(DevExpress.XtraGrid.Views.Grid.GridView pGrid, int? pos = -1)
        {
            //PREGUNTAMOS SI LA GRILLA TIENE REGISTROS
            if (pGrid.RowCount > 0)
            {
                string sqlGrillaContable = "";
                bool ExisteDebito = false, ExisteCredito = false;
                GrupoContable contable = new GrupoContable();
                List<GrupoContable> Grupos = new List<GrupoContable>();

                //HABILITAMOS BOTON ELIMINAR SI ES QUE ESTA DISABLED
                //btnEliminar.Enabled = true;
                //SI LA VARIABLE OPCIONAL TIENE VALOR 0 MANIPULAMOS POR DEFAULT LA PRIMERA FILA DE LA GRILLA
                if (pos == 0) { pGrid.FocusedRowHandle = 0; }

                //CARGAMOS CAMPOS
                txtcodigo.ReadOnly = true;
                txtcodigo.Text = pGrid.GetFocusedDataRow()["coditem"].ToString();
                txtcodigo.Properties.Appearance.FontStyleDelta = FontStyle.Bold;
                txtdescripcion.Text = pGrid.GetFocusedDataRow()["descripcion"].ToString();
                txttipo.EditValue = Int16.Parse(pGrid.GetFocusedDataRow()["tipo"].ToString());
                txtformula.EditValue = pGrid.GetFocusedDataRow()["formula"].ToString();
                txtorden.Text = pGrid.GetFocusedDataRow()["orden"].ToString();
                txtModalidad.EditValue = Convert.ToInt16(pGrid.GetFocusedDataRow()["modalidad"]);

                //checkbox
                bool value = false, itemSistema = false;

                value = bool.Parse(pGrid.GetFocusedDataRow()["imprime"].ToString());
                if (value) { cbimprime.Checked = true; }
                else cbimprime.Checked = false;

                value = bool.Parse(pGrid.GetFocusedDataRow()["informacion"].ToString());
                if (value == false) { cbinfo.Checked = false; }
                else cbinfo.Checked = true;

                value = bool.Parse(pGrid.GetFocusedDataRow()["imprimeBase"].ToString());
                if (value == false) { cbbase.Checked = false; }
                else cbbase.Checked = true;

                value = bool.Parse(pGrid.GetFocusedDataRow()["proporcional"].ToString());
                if (value)
                {
                    cbproporcional.Checked = true;
                }
                else
                {
                    cbproporcional.Checked = false;
                }

                cbPrevisualizar.Checked = (bool)pGrid.GetFocusedDataRow()["previsualizar"];
                cbTope.Checked = (bool)pGrid.GetFocusedDataRow()["tope"];

                //SI ES UN ITEM DE SISTEMA INABILITAMOS EL BOTON ELIMINAR
                itemSistema = (bool)pGrid.GetFocusedDataRow()["sistema"];

                if (itemSistema)
                {
                    btnEliminar.Enabled = false;
                    btnGuardar.Enabled = true;
                    cbSistema.Checked = true;
                    ItemSistema = true;
                }
                else
                {
                    btnEliminar.Enabled = true;
                    btnGuardar.Enabled = true;
                    cbSistema.Checked = false;
                    ItemSistema = false;
                }

                //RESTABLECER CAPTION DE BOTON NUEVO
                op.Cancela = false;
                op.SetButtonProperties(btnNuevo, 1);

                //CARGAMOS SUB CONSULTA PARA MOSTRAR DATOS DE 
                Grupos = contable.GetinformationItem(pGrid.GetFocusedDataRow()["coditem"].ToString());
                if (Grupos.Count > 0)
                {
                    //SETEAMOS VALORES EN CAJAS
                    cbContabilidad.Checked = true;
                    DisableControl(true);
                    foreach (GrupoContable x in Grupos)
                    {
                        //DEBITO
                        if (x.Tipo == 1)
                        {
                            txtCuentaDebito.EditValue = x.Cuenta;
                            txtDato1Debito.Text = x.Dato1;
                            txtDato2Debito.Text = x.Dato2;
                            txtDato3Debito.Text = x.Dato3;
                            txtDato4Debito.Text = x.Dato4;

                            ExisteDebito = true;
                        }
                        //CREDITO
                        if (x.Tipo == 2)
                        {
                            txtCuentaCredito.EditValue = x.Cuenta;
                            txtDato1Credito.Text = x.Dato1;
                            txtDato2Credito.Text = x.Dato2;
                            txtDato3Credito.Text = x.Dato3;
                            txtDato4Credito.Text = x.Dato4;

                            ExisteCredito = true;
                        }
                    }

                    if (ExisteDebito == false)
                    {
                        txtCuentaDebito.EditValue = 0;
                    }

                    if (ExisteCredito == false)
                    {
                        txtCuentaCredito.EditValue = 0;
                    }
                }
                else
                {
                    cbContabilidad.Checked = false;
                    DisableControl(false);
                }
            }
            else
            {
                //SI NO TIENE DATOS LIMPIAMOS CAMPOS SI ES QUE SE CARGARON DATOS DESDE OTRO BOTON
                fnLimpiar();
                //DESHABILITAMOS BOTON ELIMINAR
                btnEliminar.Enabled = false;
                ItemSistema = false;
            }
        }

        //CARGAR COMBO TIPO
        /*ESTRUCTURA
         * --------------------------
         * 1 --> HABER              |
         * 2 --> HABER EXENTO       |
         * 3 --> FAMILIAR           |
         * 4 --> DESCUENTO LEGAL    |
         * 5 --> DESCUENTO          |
         * 6 --> CONTRIBUICIONES    |
         * --------------------------
         */
        private void fnComboTipo(LookUpEdit pCombo)
        {
            List<PruebaCombo> lista1 = new List<PruebaCombo>();

            /*  lista.Add(new datoCombobox() { KeyInfo = 1, descInfo = "HABER"});
              lista.Add(new datoCombobox() { KeyInfo = 2, descInfo = "HABER EXENTO" });
              lista.Add(new datoCombobox() { KeyInfo = 3, descInfo = "DESCUENTO" });
              lista.Add(new datoCombobox() { KeyInfo = 4, descInfo = "DESCUENTO FINAL" });
              lista.Add(new datoCombobox() { KeyInfo = 5, descInfo = "FAMILIA" });
              lista.Add(new datoCombobox() { KeyInfo = 6, descInfo = "CONTRIBUCIONES" });*/

            lista1.Add(new PruebaCombo() { key = 1, desc = "HABER" });
            lista1.Add(new PruebaCombo() { key = 2, desc = "HABER EXENTO" });
            lista1.Add(new PruebaCombo() { key = 3, desc = "FAMILIAR" });
            lista1.Add(new PruebaCombo() { key = 4, desc = "DESCUENTO LEGAL" });
            lista1.Add(new PruebaCombo() { key = 5, desc = "DESCUENTO" });
            lista1.Add(new PruebaCombo() { key = 6, desc = "CONTRIBUCIONES" });
            lista1.Add(new PruebaCombo() { key = 7, desc = "TOTALES" });
            //lista1.Add(new PruebaCombo() { key = 3, desc = "DESCUENTO" });
            //lista1.Add(new PruebaCombo() { key = 4, desc = "DESCUENTO LEGAL" });
            //lista1.Add(new PruebaCombo() { key = 5, desc = "FAMILIAR" });


            pCombo.Properties.DataSource = lista1.ToList();
            pCombo.Properties.ValueMember = "key";
            pCombo.Properties.DisplayMember = "desc";

            pCombo.Properties.PopulateColumns();

            //NO MOSTRAR EL CAMPO KEY
            pCombo.Properties.Columns[0].Visible = false;
            pCombo.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
            pCombo.Properties.AutoSearchColumnIndex = 1;
            pCombo.Properties.ShowHeader = false;
        }

        //LIMPIAR CAMPOS
        private void fnLimpiar()
        {
            txtcodigo.ReadOnly = false;
            txtcodigo.Text = "";
            txtcodigo.Focus();
            txtdescripcion.Text = "";
            txtorden.Text = "50";
            //DEJAR COMBOBOX EN EL TIPO ACTUAL            
            txttipo.ItemIndex = TipoActual - 1;
            cbimprime.Checked = true;
            cbbase.Checked = false;
            cbinfo.Checked = false;
            lblCodigo.Visible = false;
            btnEliminar.Enabled = false;
            btnGuardar.Enabled = true;
            //fnHabilitarBotones();
            txtformula.EditValue = "0";
            cbSistema.Checked = false;
            cbContabilidad.Checked = false;
            DisableControl(false);
        }

        private void fnHabilitarBotones()
        {
            if (btnContribuciones.Enabled == false) btnContribuciones.Enabled = true;
            if (btnDescuento.Enabled == false) btnDescuento.Enabled = true;
            if (btnEliminar.Enabled == false) btnEliminar.Enabled = true;
            if (btnExento.Enabled == false) btnExento.Enabled = true;
            if (btnFamiliar.Enabled == false) btnFamiliar.Enabled = true;
            if (btnHaber.Enabled == false) btnHaber.Enabled = true;
            if (btnLegal.Enabled == false) btnLegal.Enabled = true;
        }

        //FUNCION PARA MOSTRAR TIPO
        //RETORNA STRING CON NOMBRE TIPO
        private string fnMostrarTipo(int pType)
        {
            string t = "";
            if (pType == 1)
            {
                t = "HABER";
            }
            else if (pType == 2)
            {
                t = "HABER EXENTO";
            }
            else if (pType == 3)
            {
                t = "FAMILIAR";
            }
            else if (pType == 4)
            {
                t = "DESCUENTO LEGAL";
            }
            else if (pType == 5)
            {
                t = "DESCUENTO";
            }
            else if(pType == 6)
            {
                t = "CONTRIBUCIONES";
            }
            else
            {
                t = "TOTALES";
            }

            return t;
        }

        //MANIPULAR LA TECLA TAB EN CAJA DE TEXTO CODIGO
        protected override bool ProcessDialogKey(Keys keyData)
        {
            bool cod = false;
            if (keyData == Keys.Tab)
            {
                if (txtcodigo.ContainsFocus && txtcodigo.Text != "" && txtcodigo.ReadOnly == false)
                {
                    //EVERYTHING HERE...
                    cod = fnBuscarItem(txtcodigo);
                    //SI RETORNA TRUE ES PORQUE EL CODIGO YA EXISTE EN BD
                    if (cod)
                    {
                        lblCodigo.Visible = true;
                        lblCodigo.Text = "Codigo ingresado ya existe";
                        txtcodigo.EnterMoveNextControl = false;
                    }
                    else
                    {
                        //PREGUNTAR SI EL CODIGO YA SE USA COMO VARIABLE
                        bool codVar = false;
                        codVar = fnItemVariable(txtcodigo.Text);
                        if (codVar)
                        {
                            lblCodigo.Visible = true;
                            lblCodigo.Text = "Codigo ingresado ya esta en uso";
                        }
                        else
                        {
                            lblCodigo.Visible = false;
                            txtcodigo.EnterMoveNextControl = true;
                        }

                    }
                }
                if (txtorden.ContainsFocus)
                {

                    bool valido = fnOrdenNumber(txtorden);
                    if (valido == false)
                    {
                        lblOrden.Visible = true;
                        lblOrden.Text = "Numero fuera de rango, valores entre 1 y 200";
                    }
                    else
                    {
                        lblOrden.Visible = false;
                    }
                }
            }

            return base.ProcessDialogKey(keyData);
        }

        //METODO PARA VERIFICAR QUE HAY CAMBIOS SIN GUARDAR
        //RETORNA TRUE SI EXISTEN DIFERENCIAS
        private bool fnVerificarCambios(string pCod)
        {
            //QUERY
            string sql = "SELECT coditem, descripcion, tipo, formula, orden, imprime, informacion, imprimeBase FROM item" +
                " WHERE coditem=@pCod";
            SqlCommand cmd;
            SqlDataReader rd;
            if (txtcodigo.ReadOnly == false) return false;
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
                            //recorremos registro
                            while (rd.Read())
                            {
                                //COMPARAMOS
                                if (txtcodigo.Text != rd["coditem"].ToString()) return true;
                                if (txtdescripcion.Text != rd["descripcion"].ToString()) return true;
                                if (txttipo.EditValue.ToString() != rd["tipo"].ToString()) return true;
                                if (txtformula.EditValue.ToString() != rd["formula"].ToString()) return true;
                                if (txtorden.Text != rd["orden"].ToString()) return true;

                                if (cbimprime.Checked != (bool)rd["imprime"]) return true;
                                if (cbinfo.Checked != (bool)rd["informacion"]) return true;
                                if (cbbase.Checked != (bool)rd["imprimeBase"]) return true;

                            }
                        }
                    }
                    //LIBERAR RECURSOS
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                    rd.Close();
                }
                else
                {
                    XtraMessageBox.Show("No hay conexino con la base de datos", "base de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }


            return false;
        }

        //CAMBIAR EL COLOR DEL BOTON QUE ESTÁ ACTIVO
        private void fnHaberActivo()
        {
            btnHaber.Appearance.BackColor = Color.FromArgb(147, 188, 220);
            //btnHaber.Appearance.BackColor = Color.FromArgb(216, 214, 213);
            btnFamiliar.Appearance.BackColor = Color.FromArgb(236, 236, 239);
            btnLegal.Appearance.BackColor = Color.FromArgb(236, 236, 239);
            btnDescuento.Appearance.BackColor = Color.FromArgb(236, 236, 239);
            btnContribuciones.Appearance.BackColor = Color.FromArgb(236, 236, 239);
            btnExento.Appearance.BackColor = Color.FromArgb(236, 236, 239);
            btnTipoTotales.Appearance.BackColor = Color.FromArgb(236, 236, 239);
        }

        private void fnExentoActivo()
        {
            btnHaber.Appearance.BackColor = Color.FromArgb(236, 236, 239);
            btnFamiliar.Appearance.BackColor = Color.FromArgb(236, 236, 239);
            btnLegal.Appearance.BackColor = Color.FromArgb(236, 236, 239);
            btnDescuento.Appearance.BackColor = Color.FromArgb(236, 236, 239);
            btnContribuciones.Appearance.BackColor = Color.FromArgb(236, 236, 239);
            btnExento.Appearance.BackColor = Color.FromArgb(147, 188, 220);
            btnTipoTotales.Appearance.BackColor = Color.FromArgb(236, 236, 239);
        }

        private void fnDescuentoActivo()
        {
            btnHaber.Appearance.BackColor = Color.FromArgb(236, 236, 239);
            btnFamiliar.Appearance.BackColor = Color.FromArgb(236, 236, 239);
            btnLegal.Appearance.BackColor = Color.FromArgb(236, 236, 239);
            btnDescuento.Appearance.BackColor = Color.FromArgb(147, 188, 220);
            btnContribuciones.Appearance.BackColor = Color.FromArgb(236, 236, 239);
            btnExento.Appearance.BackColor = Color.FromArgb(236, 236, 239);
            btnTipoTotales.Appearance.BackColor = Color.FromArgb(236, 236, 239);
        }

        private void fnLegalActivo()
        {
            btnHaber.Appearance.BackColor = Color.FromArgb(236, 236, 239);
            btnFamiliar.Appearance.BackColor = Color.FromArgb(236, 236, 239);
            btnLegal.Appearance.BackColor = Color.FromArgb(147, 188, 220);
            btnDescuento.Appearance.BackColor = Color.FromArgb(236, 236, 239);
            btnContribuciones.Appearance.BackColor = Color.FromArgb(236, 236, 239);
            btnExento.Appearance.BackColor = Color.FromArgb(236, 236, 239);
            btnTipoTotales.Appearance.BackColor = Color.FromArgb(236, 236, 239);
        }

        private void fnFormulaActivo()
        {
            btnHaber.Appearance.BackColor = Color.FromArgb(236, 236, 239);
            btnFamiliar.Appearance.BackColor = Color.FromArgb(147, 188, 220);
            btnLegal.Appearance.BackColor = Color.FromArgb(236, 236, 239);
            btnDescuento.Appearance.BackColor = Color.FromArgb(236, 236, 239);
            btnContribuciones.Appearance.BackColor = Color.FromArgb(236, 236, 239);
            btnExento.Appearance.BackColor = Color.FromArgb(236, 236, 239);
            btnTipoTotales.Appearance.BackColor = Color.FromArgb(236, 236, 239);
        }

        private void fnTotalesActivo()
        {
            btnHaber.Appearance.BackColor = Color.FromArgb(236, 236, 239);
            btnFamiliar.Appearance.BackColor = Color.FromArgb(236, 236, 239);
            btnLegal.Appearance.BackColor = Color.FromArgb(236, 236, 239);
            btnDescuento.Appearance.BackColor = Color.FromArgb(236, 236, 239);
            btnContribuciones.Appearance.BackColor = Color.FromArgb(236, 236, 239);
            btnExento.Appearance.BackColor = Color.FromArgb(236, 236, 239);
            btnTipoTotales.Appearance.BackColor = Color.FromArgb(147, 188, 220);

        }

        private void fnContActivo()
        {
            btnHaber.Appearance.BackColor = Color.FromArgb(236, 236, 239);
            btnFamiliar.Appearance.BackColor = Color.FromArgb(236, 236, 239);
            btnLegal.Appearance.BackColor = Color.FromArgb(236, 236, 239);
            btnDescuento.Appearance.BackColor = Color.FromArgb(236, 236, 239);
            btnContribuciones.Appearance.BackColor = Color.FromArgb(147, 188, 220);
            btnExento.Appearance.BackColor = Color.FromArgb(236, 236, 239);
            btnTipoTotales.Appearance.BackColor = Color.FromArgb(236, 236, 239);
        }

        //PARA OPERATORIA CON LOS BOTON
        //CARGAR GRILLA 
        private void fnBotonClick(int type)
        {
            //SQL PARA CARGAR GRILLA
            string sql = string.Format("SELECT coditem, descripcion, tipo, formula, orden, imprime, " +
                "informacion, imprimeBase, proporcional, sistema, previsualizar, tope, modalidad " +
                " FROM item WHERE tipo={0} ORDER BY orden, coditem", type);

            //CARGAR GRILLA
            fnSistema.spllenaGridView(gridItem, sql);
            fnSistema.spOpcionesGrilla(viewItem);
            fnColumnas(viewItem);
            //CARGAR PRIMERA FILA DE LA GRILLA
            fnCargarCampos(viewItem);

            //CAMBIAR EL LABEL DE ACUERDO AL TYPE
            lblTipo.Text = "REGISTROS: " + fnMostrarTipo(type);
        }

        //SOLO NUMEROS ENTRE 0 Y 200 en campo orden
        //retorna false si no es valido
        private bool fnOrdenNumber(TextEdit pOrden)
        {
            if (pOrden.Text.Length == 0) return false;
            int number = int.Parse(pOrden.Text);
            List<Int32> lista = new List<int>();
            int encontrado = 0;
            for (int i = 0; i <= 200; i++)
            {
                lista.Add(i);
            }

            foreach (var item in lista)
            {
                if (number == item)
                {
                    //valido
                    encontrado = 1;
                    break;
                }
            }

            if (encontrado == 1) { return true; }
            else return false;
        }

        //ocultar label de error
        private void fnOcultar()
        {
            if (lblOrden.Visible) lblOrden.Visible = false;
            if (lblCodigo.Visible) lblCodigo.Visible = false;
        }

        //CARGAR COMBOBOX FORMULAS
        private void fnComboFormula(LookUpEdit pCombo)
        {
            List<test> lista = new List<test>();

            //string consulta
            string sql = "SELECT codFormula, descFormula FROM formula";
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
                            //SI ENCUENTRA REGISTROS RECORREMOS...
                            while (rd.Read())
                            {
                                //LLENAMOS LISTA CON DATOS
                                lista.Add(new test() { key = (string)rd["codFormula"], desc = (string)rd["descFormula"] });
                            }
                        }
                    }
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();

                    //PROPIEDADES COMBOBOX
                    pCombo.Properties.DataSource = lista.ToList();
                    pCombo.Properties.ValueMember = "key";
                    pCombo.Properties.DisplayMember = "desc";

                    pCombo.Properties.PopulateColumns();

                    pCombo.Properties.Columns[0].Visible = false;

                    pCombo.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
                    pCombo.Properties.AutoSearchColumnIndex = 1;
                    pCombo.Properties.ShowHeader = false;
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

        //VERIFICAR SI EL CODIGO DE ITEM YA EXISTE COMO CODIGO DE VARIABLE
        //RETORNA TRUE SI YA ESTA EN USO
        //RETORNA FALSE EN OTRO CASO
        private bool fnItemVariable(string pCod)
        {
            string sql = "SELECT codvariable FROM variablesistema WHERE codvariable=@pCod";
            SqlCommand cmd;
            SqlDataReader rd;

            bool EnUso = false;
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
                            //SI ENCUENTRA REGISTROS ES PORQUE EL CODIGO YA SE ESTA USANDO COMO CODIGO DE VARIABLE
                            EnUso = true;
                        }
                        else
                        {
                            EnUso = false;
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

            return EnUso;
        }

        //OBTENER EL RUT Y EL NOMBRE DEL TRABAJADOR EN CASO DE QUE UN ITEM ESTE SIENDO USADO POR UN TRABAJADOR
        private string fnDatosTrabajador(string pCod)
        {

            string sql = "select id from itemtrabajador JOIN item " +
                "on itemtrabajador.coditem = item.coditem AND itemtrabajador.coditem = @pCod";

            SqlCommand cmd;
            SqlDataReader rd;

            string datos = "";

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
                            while (rd.Read())
                            {
                                //GUARDAMOS DATOS                               
                                datos = ((int)rd["id"]).ToString();
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

            //RETORNAMOS ARREGLO
            return datos;
        }

        //ELIMINAR ITEM TRABAJADOR
        //PARAMETRO DE ENTRADA: CODIGO FORMULA USADA
        //RETORNA TRUE SI ELIMNA CORRECTAMENTE
        private bool fnEliminarItemTrabajador(string pItem)
        {
            string sql = "DELETE FROM itemtrabajador WHERE coditem=@pItem";
            SqlCommand cmd;
            bool exito = false;
            int res = 0;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pItem", pItem));

                        //EJECUTAR CONSULTA
                        res = cmd.ExecuteNonQuery();
                        if (res > 0)
                        {
                            exito = true;
                            //GUARDAMOS EVENTO EN LOG
                            logRegistro log = new logRegistro(User.getUser(), "SE ELIMINAR ITEM TRABAJADOR " + pItem, "ITEMTRABAJADOR", pItem, "0", "ELIMINAR");
                            log.Log();
                        }
                        else
                        {
                            exito = false;
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
            return exito;
        }

        //ACTUALIZAR FORMULA EN CAMPO CLASE SI ES QUE ESTE ES MODIFICADO
        private bool fnModificarFormulaClase(string pItem, string pFormula)
        {
            string sql = "UPDATE itemclase SET formula=@pFormula WHERE item=@pItem";
            SqlCommand cmd;
            int res = 0;

            bool actualizado = false;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pFormula", pFormula));
                        cmd.Parameters.Add(new SqlParameter("@pItem", pItem));

                        //EJECUTAR CONSULTA
                        res = cmd.ExecuteNonQuery();

                        if (res > 0)
                        {
                            actualizado = true;
                        }
                        else
                        {
                            actualizado = false;
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
            return actualizado;
        }

        //MODIFICAR FORMULA EN ITEM TRABAJADOR SI ES QUE SE MODIFICO FORMULA EN TABLA ITEM
        //NECESITAMOS EL CODIGO ITEM Y EL VALOR PARA LA NUEVA FORMULA
        private void fnModificarItemTrabajador(string pitem, string pFormula)
        {
            string sql = "UPDATE itemtrabajador SET formula=@pFormula WHERE coditem=@pItem";
            SqlCommand cmd;

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pFormula", pFormula));
                        cmd.Parameters.Add(new SqlParameter("@pItem", pitem));

                        //RESULTADOS
                        cmd.ExecuteNonQuery();
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
        }

        //VERIFICAR SI EXISTE RELACION DE UN ITEM CON UN REGISTRO DE TABLA itemclase
        private bool fnitemClase(string pItem)
        {
            string sql = " SELECT itemclase.item from itemClase " +
                         " INNER JOIN item ON item.coditem = itemclase.item " +
                         " WHERE itemclase.item = @pItem";

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
                        cmd.Parameters.Add(new SqlParameter("@pItem", pItem));

                        //EJECUTAR CONSULTA
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //SI ENCONTRO RESULTADOS ES PORQUE EXISTE ESE ITEM EN TABLA ITEMCLASE
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

        //ELIMINAR ITEMCLASE SI EXISTE LA RELACION
        private bool fnEliminarItemClase(string pItem)
        {
            string sql = "DELETE FROM itemclase WHERE item=@pItem";
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
                        cmd.Parameters.Add(new SqlParameter("@pItem", pItem));

                        //EJECUTAR CONSULTA
                        res = cmd.ExecuteNonQuery();

                        if (res > 0)
                        {
                            eliminado = true;
                            //GUARDAMOS EVENTO EN LOG
                            logRegistro log = new logRegistro(User.getUser(), "SE ELIMINA ITEM " + pItem, "ITEMCLASE", pItem, "0", "ELIMINAR");
                            log.Log();
                        }
                        else
                        {
                            eliminado = false;
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

            return eliminado;
        }

        //VERIFICAR SI UN ITEM ES HABER
        //PREGUNTAR SI EL ITEM SELECCIONADO ES UN HABER (TIPO 1)
        private bool esHaber(string item)
        {
            bool haber = false;
            string sql = "SELECT tipo FROM item WHERE coditem=@pitem";
            SqlCommand cmd;
            SqlDataReader rd;
            int tipo = 0;

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pitem", item));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                tipo = (int)rd["tipo"];
                            }
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

            if (tipo == 1)
            {
                haber = true;
            }
            else
            {
                haber = false;
            }
            return haber;
        }

        //OBTENER EL TIPO AL QUE CORRESPONDE EL ITEM
        private int TipoItem(string item)
        {
            int tipo = 0;
            string sql = "SELECT TIPO FROM item WHERE coditem=@item";
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
                                tipo = (int)rd["tipo"];
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
            return tipo;
        }

        //LISTADO ITEMS QUE NO SE PUEDEN ELIMINAR
        private bool ItemsIntocables(string item)
        {
            bool intocable = false;

            List<string> lista = new List<string>();
            lista.Add("SUBASE");
            lista.Add("GRATIFI");
            lista.Add("COLACIO");
            lista.Add("MOVILIZ");
            lista.Add("ASIGFAM");
            lista.Add("PREVISI");
            lista.Add("SALUD");
            lista.Add("SCEMPLE");
            lista.Add("IMPUEST");
            lista.Add("SCEMPRE");
            lista.Add("SEGINV");
            lista.Add("APREVOL");
            lista.Add("PRESTAM");
            lista.Add("ANTSUEL");
            lista.Add("SGANDES");
            lista.Add("MUTUALI");
            lista.Add("CAJACOM");
            lista.Add("ASIFAR");
            lista.Add("ASIGMAT");
            lista.Add("ASIGINV");
            lista.Add("AFPAHO");
            lista.Add("SANNA");
            lista.Add("SBGIRO");
            lista.Add("SBGIRA");

            foreach (var data in lista)
            {
                if (data == item)
                {
                    intocable = true;
                    break;
                }
            }

            return intocable;
        }

        //VERIFICAR SI EL ITEM ES UN ITEM DE SISTEMA
        private bool ItemDeSistema(string pitem)
        {
            string sql = "SELECT sistema FROM item WHERE coditem=@pItem";
            SqlCommand cmd;
            bool desistema = false;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pItem", pitem));

                        if (Convert.ToInt32(cmd.ExecuteScalar()) == 1)
                            desistema = true;

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return desistema;
        }

        //MODIFICAR NUMERO DE ORDEN, FORMULA EN ITEMTRABAJADOR
        private void ModificarItemTrabajador(int pOrden, string pCodFormula, int pTipo, string pItem)
        {
            string sql = "UPDATE itemtrabajador SET tipo=@pTipo, orden=@pOrden " +
                "WHERE coditem=@pItem AND anomes=@pPeriodo";
            SqlCommand cmd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETRO
                        cmd.Parameters.Add(new SqlParameter("@pTipo", pTipo));
                        cmd.Parameters.Add(new SqlParameter("@pOrden", pOrden));
                        //cmd.Parameters.Add(new SqlParameter("@pFormula", pCodFormula));
                        cmd.Parameters.Add(new SqlParameter("@pItem", pItem));
                        cmd.Parameters.Add(new SqlParameter("@pPeriodo", Calculo.PeriodoObservado));

                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        private void ComboContabilidad()
        {
            Cuenta cuen = new Cuenta();
            List<Cuenta> Cuentas = new List<Cuenta>();
            Cuentas = cuen.GetList();

            if (Cuentas.Count > 0)
            {
                try
                {
                    Cuentas.Add(new Cuenta() { AgRut = 0, CodCuenta = 0, CodEs = 0, DescCuenta = "No Aplica"});

                    txtCuentaCredito.Properties.DataSource = Cuentas.ToList();
                    txtCuentaDebito.Properties.DataSource = Cuentas.ToList();

                    txtCuentaCredito.Properties.ValueMember = "CodCuenta";
                    txtCuentaCredito.Properties.DisplayMember = "DescCuenta";
                    txtCuentaDebito.Properties.ValueMember = "CodCuenta";
                    txtCuentaDebito.Properties.DisplayMember = "DescCuenta";

                    if (txtCuentaCredito.Properties.DataSource != null)
                    {
                        txtCuentaCredito.Properties.PopulateColumns();
                        //Codigo cuenta
                        txtCuentaCredito.Properties.Columns[0].Visible = false;
                        //Codigo de esquema
                        txtCuentaCredito.Properties.Columns[2].Visible = false;
                        //Agrupa rut
                        txtCuentaCredito.Properties.Columns[3].Visible = false;

                        txtCuentaCredito.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
                        txtCuentaCredito.Properties.AutoSearchColumnIndex = 1;
                        txtCuentaCredito.Properties.ShowHeader = false;
                        txtCuentaCredito.ItemIndex = 0;

                    }

                    if (txtCuentaDebito.Properties.DataSource != null)
                    {
                        txtCuentaDebito.Properties.PopulateColumns(); 
                        //Codigo cuenta
                        txtCuentaDebito.Properties.Columns[0].Visible = false;
                        //Codigo de esquema
                        txtCuentaDebito.Properties.Columns[2].Visible = false;
                        //Agrupa rut
                        txtCuentaDebito.Properties.Columns[3].Visible = false;                  

                        txtCuentaDebito.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
                        txtCuentaDebito.Properties.AutoSearchColumnIndex = 1;
                        txtCuentaDebito.Properties.ShowHeader = false;

                        txtCuentaDebito.ItemIndex = 0;
                    }
                }
                catch (Exception ex)
                {
                    string error = ex.Message;
                }
                    
            }
        }

        #region "LOG ITEM"
        //TABLA HASH PRECARGA DATOSITEM
        private Hashtable PrecargaItem(string codItem)
        {
            Hashtable datos = new Hashtable();
            SqlCommand cmd;
            string sql = "SELECT coditem, descripcion, tipo, formula, orden, imprime, informacion, imprimebase, " +
                "proporcional, sistema, previsualizar, tope, modalidad FROM item WHERE coditem=@item";
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@item", codItem));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //LLENAMOS TABLA DATOS
                                datos.Add("coditem", (string)rd["coditem"]);
                                datos.Add("descripcion", (string)rd["descripcion"]);
                                datos.Add("tipo", (int)rd["tipo"]);
                                datos.Add("formula", (string)rd["formula"]);
                                datos.Add("orden", (int)rd["orden"]);
                                datos.Add("imprime", (bool)rd["imprime"]);
                                datos.Add("informacion", (bool)rd["informacion"]);
                                datos.Add("imprimebase", (bool)rd["imprimebase"]);
                                datos.Add("proporcional", (bool)rd["proporcional"]);
                                datos.Add("sistema", (bool)rd["sistema"]);
                                datos.Add("previsualizar", (bool)rd["previsualizar"]);
                                datos.Add("tope", (bool)rd["tope"]);
                                datos.Add("modalidad", Convert.ToInt32(rd["modalidad"]));
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

            //RETORNAMOS HASH
            return datos;
        }

        //PRECARGA ITEM SOLO PARA OPCIONES DE IMPRESION
        private Hashtable PrecargaOpciones(string codItem)
        {
            Hashtable data = new Hashtable();
            string sql = "SELECT imprime, informacion, imprimebase, proporcional, previsualizar, " +
                "tope, modalidad FROM item WHERE coditem=@pItem";
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
                            cmd.Parameters.Add(new SqlParameter("@pItem", codItem));

                            rd = cmd.ExecuteReader();
                            if (rd.HasRows)
                            {
                                while (rd.Read())
                                {
                                    //LLENAMOS TABLAHASH
                                    data.Add("imprime", (bool)rd["imprime"]);
                                    data.Add("informacion", (bool)rd["informacion"]);
                                    data.Add("imprimebase", (bool)rd["imprimebase"]);
                                    data.Add("proporcional", (bool)rd["proporcional"]);
                                    data.Add("previsualizar", (bool)rd["previsualizar"]);
                                    data.Add("tope", (bool)rd["tope"]);
                                    data.Add("modalidad", Convert.ToInt16(rd["modalidad"]));
                                }
                            }

                            cmd.Dispose();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return data;
        }

        //COMPARA VALORES OPCIONES
        private void ComparaOpciones(string pItem, bool pImprime, bool pProporcional, bool pInfo, bool pImpBase, bool pPrevisualizar, bool pTope, Int16 pModalidad, Hashtable pTabla)
        {
            if (pTabla.Count > 0)
            {
                if ((bool)pTabla["imprime"] != pImprime)
                {
                    logRegistro log = new logRegistro(User.getUser(), $"SE MODIFICA OPCION IMPRIME EN ITEM {pItem}", "ITEM", (bool)pTabla["imprime"] + "", pImprime + "", "MODIFICAR");
                    log.Log();
                }
                if ((bool)pTabla["informacion"] != pInfo)
                {
                    logRegistro log = new logRegistro(User.getUser(), $"SE MODIFICA OPCION IMPRIME EN ITEM {pItem}", "ITEM", (bool)pTabla["informacion"] + "", pInfo + "", "MODIFICAR");
                    log.Log();
                }
                if ((bool)pTabla["imprimebase"] != pImpBase)
                {
                    logRegistro log = new logRegistro(User.getUser(), $"SE MODIFICA OPCION IMPRIME BASE EN ITEM {pItem}", "ITEM", (bool)pTabla["imprimebase"] + "", pImpBase + "", "MODIFICAR");
                    log.Log();
                }
                if ((bool)pTabla["proporcional"] != pInfo)
                {
                    logRegistro log = new logRegistro(User.getUser(), $"SE MODIFICA OPCION PROPORCIONAL EN ITEM {pItem}", "ITEM", (bool)pTabla["proporcional"] + "", pProporcional + "", "MODIFICAR");
                    log.Log();
                }
                if ((bool)pTabla["previsualizar"] != pPrevisualizar)
                {
                    logRegistro log = new logRegistro(User.getUser(), $"SE MODIFICA OPCION PREVISUALIZAR EN ITEM {pItem}", "ITEM", (bool)pTabla["previsualizar"] + "", pPrevisualizar + "", "MODIFICAR");
                    log.Log();
                }
                if ((bool)pTabla["tope"] != pTope)
                {
                    logRegistro log = new logRegistro(User.getUser(), $"SE MODIFICA OPCION TOPE 15% EN ITEM {pItem}", "ITEM", (bool)pTabla["tope"] + "", pTope + "", "MODIFICAR");
                    log.Log();
                }
                if (Convert.ToInt16(pTabla["modalidad"]) != pModalidad)
                {
                    logRegistro log = new logRegistro(User.getUser(), $"SE MODIFICA OPCION MODALIDAD EN ITEM {pItem}", "ITEM", Convert.ToInt16(pTabla["modalidad"]) + "", pModalidad + "", "MODIFICAR");
                    log.Log();
                }
            }
        }

        //COMPARAMOS VALORES
        private void ComparaValorItem(string item, string descripcion, int tipo, string formula, int orden, bool imprime,
            bool informacion, bool imprimeBase, bool propocional, bool sistema, bool pPrevisualizar, bool pTope, Int16 pModalidad,
            Hashtable Datos)
        {
            if (Datos.Count > 0)
            {
                if ((string)Datos["descripcion"] != descripcion)
                {
                    logRegistro log = new logRegistro(User.getUser(), "MODIFICA DESCRIPCION EN ITEM " + item, "ITEM", (string)Datos["descripcion"], descripcion, "MODIFICAR");
                    log.Log();
                }
                if ((int)Datos["tipo"] != tipo)
                {
                    logRegistro log = new logRegistro(User.getUser(), "MODIFICA TIPO EN ITEM " + item, "ITEM", (int)Datos["tipo"] + "", tipo + "", "MODIFICAR");
                    log.Log();
                }
                if ((string)Datos["formula"] != formula)
                {
                    logRegistro log = new logRegistro(User.getUser(), "MODIFICA FORMULA EN ITEM " + formula, "ITEM", (string)Datos["formula"], formula, "MODIFICAR");
                    log.Log();
                }
                if ((int)Datos["orden"] != orden)
                {
                    logRegistro log = new logRegistro(User.getUser(), "MODIFICA NUMERO DE ORDEN EN ITEM " + item, "ITEM", (int)Datos["orden"] + "", orden + "", "MODIFICAR");
                    log.Log();
                }
                if ((bool)Datos["imprime"] != imprime)
                {
                    logRegistro log = new logRegistro(User.getUser(), "MODIFICA OPCION IMPRIME EN ITEM " + item, "ITEM", (bool)Datos["imprime"] + "", imprime + "", "MODIFICAR");
                    log.Log();
                }
                if ((bool)Datos["informacion"] != informacion)
                {
                    logRegistro log = new logRegistro(User.getUser(), "MODIFICA OPCION INFORMACION EN ITEM " + item, "ITEM", (bool)Datos["informacion"] + "", informacion + "", "MODIFICAR");
                    log.Log();
                }
                if ((bool)Datos["imprimebase"] != imprimeBase)
                {
                    logRegistro log = new logRegistro(User.getUser(), "MODIFICA OPCION IMPRIMEBASE EN ITEM " + item, "ITEM", (bool)Datos["imprimebase"] + "", imprimeBase + "", "MODIFICAR");
                    log.Log();
                }
                if ((bool)Datos["proporcional"] != propocional)
                {
                    logRegistro log = new logRegistro(User.getUser(), "MODIFICA OPCION PROPORCIONAL EN ITEM " + item, "ITEM", (bool)Datos["proporcional"] + "", propocional + "", "MODIFICAR");
                    log.Log();
                }
                if ((bool)Datos["sistema"] != sistema)
                {
                    logRegistro log = new logRegistro(User.getUser(), "MODIFICA OPCION SISTEMA EN ITEM " + item, "ITEM", (bool)Datos["sistema"] + "", sistema + "", "MODIFICAR");
                    log.Log();
                }
                if ((bool)Datos["previsualizar"] != pPrevisualizar)
                {
                    logRegistro log = new logRegistro(User.getUser(), $"SE MODIFICA OPCION PREVISUALIZAR EN ITEM {item}", "ITEM", (bool)Datos["previsualizar"] + "", pPrevisualizar + "", "MODIFICAR");
                    log.Log();
                }
                if ((bool)Datos["tope"] != pTope)
                {
                    logRegistro log = new logRegistro(User.getUser(), $"SE MODIFICA OPCION TOPE 15% EN ITEM {item}", "ITEM", (bool)Datos["tope"] + "", pTope + "", "MODIFICAR");
                    log.Log();
                }
                if (Convert.ToInt16(Datos["modalidad"]) != pModalidad)
                {
                    logRegistro log = new logRegistro(User.getUser(), $"SE MODIFICA OPCION MODALIDAD  EN ITEM {item}", "ITEM", Convert.ToInt16(Datos["modalidad"]) + "", pModalidad + "", "MODIFICAR");
                    log.Log();
                }       
            }
        }

        #endregion

        #endregion

        #region "CONTROLES FORMULARIO"
        //EVERYTHING HERE...

        //SOLO NUMEROS EN CAMPO CODIGO
        private void txtcodigo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (char.IsLetter(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void txtformula_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtorden_KeyPress(object sender, KeyPressEventArgs e)
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

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            //LIMPIAR CAMPOS
            fnLimpiar();

            if (op.Cancela == false)
            {
                op.SetButtonProperties(btnNuevo, 2);
                op.Cancela = true;

                //DEJAMOS VARIABLE ITEMSISTEMA EN FALSE
                ItemSistema = false;
            }
            else
            {
                op.SetButtonProperties(btnNuevo, 1);
                op.Cancela = false;
                fnBotonClick(TipoActual);

            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            bool cod = false;
            bool valida = false;
            if (txtcodigo.Text == "") { XtraMessageBox.Show("Codigo no valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtcodigo.Focus(); return; }

            if (txtcodigo.ReadOnly)
            {
                //if (txtcodigo.Text.ToLower() == "sbgiro" || txtcodigo.Text.ToLower() == "impuest" || txtcodigo.Text.ToLower() == "sbgira")
                //{ XtraMessageBox.Show("No puedes modificar este item", "Item", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                //VALIDAR QUE CAMPO ORDEN ESTE ENTRE EL RANGO 0 Y 200
                valida = fnOrdenNumber(txtorden);
                if (valida)
                {
                    if (txtCuentaCredito.Properties.DataSource != null)
                    {
                        CodEsqCredito = Convert.ToInt32(txtCuentaCredito.GetColumnValue("CodEs"));
                        CodCuentaCredito = Convert.ToInt32(txtCuentaCredito.GetColumnValue("CodCuenta"));
                    }

                    if (txtCuentaDebito.Properties.DataSource != null)
                    {
                        CodEsqDebito = Convert.ToInt32(txtCuentaDebito.GetColumnValue("CodEs"));
                        CodCuentaDebito = Convert.ToInt32(txtCuentaDebito.GetColumnValue("CodCuenta"));
                    }

                    if (Persona.ItemUsado(txtcodigo.Text))
                    {
                        DialogResult advertencia = XtraMessageBox.Show("Este item esta siendo usado por un trabajador, ¿Desear cambiar la informacion de todas maneras?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (advertencia == DialogResult.Yes)
                        {
                            lblOrden.Visible = false;
                            fnModificarItem(txtcodigo, txtdescripcion, txttipo, txtformula, txtorden, cbimprime, cbinfo, cbbase, TipoActual, cbSistema, cbPrevisualizar, cbTope, txtModalidad,
                                txtCuentaDebito, txtCuentaCredito, txtDato1Credito, txtDato2Credito, txtDato3Credito, txtDato4Credito,
                        txtDato1Debito, txtDato2Debito, txtDato3Debito, txtDato4Debito, cbContabilidad);
                        }
                    }
                    else
                    {
                        lblOrden.Visible = false;
                        fnModificarItem(txtcodigo, txtdescripcion, txttipo, txtformula, txtorden, cbimprime, cbinfo, cbbase, TipoActual, cbSistema, cbPrevisualizar, cbTope, txtModalidad,
                            txtCuentaDebito, txtCuentaCredito, txtDato1Credito, txtDato2Credito, txtDato3Credito, txtDato4Credito,
                        txtDato1Debito, txtDato2Debito, txtDato3Debito, txtDato4Debito, cbContabilidad);
                    }
                }
                else
                {
                    lblOrden.Visible = true;
                    lblOrden.Text = "Numero fuera de rango, valores entre 1 y 200";
                    return;
                }
            }
            else
            {
                //INSERT
                //code...       
                //verificar que el codigo ingresado no exista en la base de datos
                cod = fnBuscarItem(txtcodigo);
                if (cod) { XtraMessageBox.Show("Codigo ingresado ya existe", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtcodigo.Focus(); return; }

                //VERIFICAR QUE EL CODIGO NO SE ESTE USANDO COMO VARIABLE 
                bool codVar = false;
                codVar = fnItemVariable(txtcodigo.Text);
                if (codVar) { XtraMessageBox.Show("Codigo ingresado ya esta en uso", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); txtcodigo.Focus(); return; }

                //VALIDAR QUE CAMPO ORDEN ESTE ENTRE EL RANGO 0 Y 200
                valida = fnOrdenNumber(txtorden);
                if (valida)
                {
                    lblOrden.Visible = false;                    
                    if (txtCuentaCredito.Properties.DataSource != null)
                    {
                        CodEsqCredito = Convert.ToInt32(txtCuentaCredito.GetColumnValue("CodEs"));
                        CodCuentaCredito = Convert.ToInt32(txtCuentaCredito.GetColumnValue("CodCuenta"));
                    }

                    if (txtCuentaDebito.Properties.DataSource != null)
                    {
                        CodEsqDebito = Convert.ToInt32(txtCuentaDebito.GetColumnValue("CodEs"));
                        CodCuentaDebito = Convert.ToInt32(txtCuentaDebito.GetColumnValue("CodCuenta"));
                    }


                    fnIngresoItem(txtcodigo, txtdescripcion, txttipo, txtformula, txtorden, cbimprime, cbinfo, cbbase, TipoActual, cbSistema, cbPrevisualizar, cbTope, txtModalidad, 
                        txtCuentaDebito, txtCuentaCredito, txtDato1Credito, txtDato2Credito, txtDato3Credito, txtDato4Credito, 
                        txtDato1Debito, txtDato2Debito, txtDato3Debito, txtDato4Debito, cbContabilidad);
                }
                else
                {
                    lblOrden.Visible = true;
                    lblOrden.Text = "Numero fuera de rango, valores entre 1 y 200";
                    return;
                }
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            if (txtcodigo.Text == "") { XtraMessageBox.Show("Codigo Erroneo", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            if (ItemDeSistema(txtcodigo.Text))
            { XtraMessageBox.Show("No puedes eliminar un item de sistema", "Denegado", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (ItemsIntocables(txtcodigo.Text))
            { XtraMessageBox.Show("No puedes eliminar este item", "Item de sistema", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            bool transaccionCorrecta = false;

            transaccionCorrecta = EliminarItemTransaccion(txtcodigo, TipoActual);
            if (transaccionCorrecta)
            { XtraMessageBox.Show("Item Eliminado correctamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); }
            else
            { XtraMessageBox.Show("Error al intentar eliminar item", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); }
            //fnEliminarItem(txtcodigo, TipoActual);
        }
        private void btnHaber_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            //CAMBIO DE COLOR DEL BOTON SOLO SE DEBE REALIZAR CUANDO EL USUARIO ACEPTA CAMBIAR DE TIPO
            bool distinto = false;
            TipoActual = 1;
            if (txtcodigo.Text != "")
            {
                distinto = fnVerificarCambios(txtcodigo.Text);
                if (distinto)
                {
                    DialogResult dialogo = XtraMessageBox.Show("Hay cambios sin guardar, ¿deseas continuar de todas maneras?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogo == DialogResult.Yes)
                    {
                        fnBotonClick(TipoActual);
                        fnOcultar();
                        fnHaberActivo();
                    }
                    else
                    {
                        //NO SE DEBE CAMBIAR COLOR DEL BOTON

                    }
                }
                else
                {
                    fnBotonClick(TipoActual);
                    fnOcultar();
                    fnHaberActivo();
                }
            }
            else
            {
                fnBotonClick(TipoActual);
                fnOcultar();
                fnHaberActivo();
            }



        }
        private void btnExento_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            bool distinto = false;
            TipoActual = 2;
            if (txtcodigo.Text != "")
            {
                distinto = fnVerificarCambios(txtcodigo.Text);
                if (distinto)
                {
                    DialogResult dialogo = XtraMessageBox.Show("Hay cambios sin guardar, ¿deseas continuar de todas maneras?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogo == DialogResult.Yes)
                    {
                        fnBotonClick(TipoActual);
                        //cambiar color boton
                        fnExentoActivo();
                        fnOcultar();
                    }
                }
                else
                {
                    fnBotonClick(TipoActual);
                    fnExentoActivo();
                    fnOcultar();
                }
            }
            else
            {
                fnBotonClick(TipoActual);
                fnExentoActivo();
                fnOcultar();
            }



        }
        private void btnDescuento_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            bool distinto = false;
            TipoActual = 3;
            if (txtcodigo.Text != "")
            {
                distinto = fnVerificarCambios(txtcodigo.Text);
                if (distinto)
                {
                    DialogResult dialogo = XtraMessageBox.Show("Hay cambios sin guardar, ¿deseas continuar de todas maneras?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogo == DialogResult.Yes)
                    {
                        //SI ES YES CAMBIAMOS GRILLA Y TODO LO DEMAS
                        fnBotonClick(TipoActual);

                        //CAMBIAR COLOR BOTON
                        fnDescuentoActivo();
                        fnOcultar();
                    }
                }
                else
                {
                    //SI ES FALSE SOLO CAMBIAMOS SIN PREGUNTAR
                    fnBotonClick(TipoActual);
                    fnDescuentoActivo();
                    fnOcultar();
                }
            }
            else
            {
                //MOSTRAR GRILLA CON TIPO --> 3 DESCUENTO)
                fnBotonClick(TipoActual);
                fnDescuentoActivo();
                fnOcultar();
            }


        }
        private void btnLegal_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            bool distinto = false;
            TipoActual = 4;
            if (txtcodigo.Text != "")
            {
                distinto = fnVerificarCambios(txtcodigo.Text);
                if (distinto)
                {
                    DialogResult dialogo = XtraMessageBox.Show("Hay cambios sin guardar, ¿deseas continuar de todas maneras?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogo == DialogResult.Yes)
                    {
                        //SI ES YES CAMBIAMOS GRILLA Y TODO LO DEMAS
                        fnBotonClick(TipoActual);
                        //CAMBIAMOS COLOR BOTON
                        fnLegalActivo();
                        fnOcultar();
                    }
                }
                else
                {
                    //SI ES FALSE SOLO CAMBIAMOS SIN PREGUNTAR
                    fnBotonClick(TipoActual);
                    fnLegalActivo();
                    fnOcultar();
                }
            }
            else
            {
                //MOSTRAR GRILLA CON TIPO --> 4 (DESCUENTO LEGAL)
                fnBotonClick(TipoActual);
                fnLegalActivo();
                fnOcultar();

            }


        }
        private void btnContribuciones_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            bool distinto = false;
            TipoActual = 6;
            if (txtcodigo.Text != "")
            {
                distinto = fnVerificarCambios(txtcodigo.Text);
                if (distinto)
                {
                    DialogResult dialogo = XtraMessageBox.Show("Hay cambios sin guardar, ¿deseas continuar de todas maneras?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogo == DialogResult.Yes)
                    {
                        //SI ES YES CAMBIAMOS GRILLA Y TODO LO DEMAS
                        fnBotonClick(TipoActual);
                        //CAMBIAR COLOR BOTON
                        fnContActivo();
                        fnOcultar();
                    }
                }
                else
                {
                    //SI ES FALSE SOLO CAMBIAMOS SIN PREGUNTAR
                    fnBotonClick(TipoActual);
                    fnContActivo();
                    fnOcultar();
                }
            }
            else
            {
                //MOSTRAR GRILLA CON TIPO --> 6 (CONTRIBUCIONES)
                fnBotonClick(TipoActual);
                fnContActivo();
                fnOcultar();
            }


        }
        private void btnFamiliar_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            bool distinto = false;
            TipoActual = 5;
            if (txtcodigo.Text != "")
            {
                distinto = fnVerificarCambios(txtcodigo.Text);
                if (distinto)
                {
                    DialogResult dialogo = XtraMessageBox.Show("Hay cambios sin guardar, ¿deseas continuar de todas maneras?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogo == DialogResult.Yes)
                    {
                        //SI ES YES CAMBIAMOS GRILLA Y TODO LO DEMAS
                        fnBotonClick(TipoActual);
                        fnFormulaActivo();
                        fnOcultar();
                    }
                }
                else
                {
                    //SI ES FALSE SOLO CAMBIAMOS SIN PREGUNTAR
                    fnBotonClick(TipoActual);
                    //cambiar color boton
                    fnFormulaActivo();
                    fnOcultar();
                }
            }
            else
            {
                //MOSTRAR GRILLA CON TIPO --> 5 (FORMULA)
                fnBotonClick(TipoActual);
                fnFormulaActivo();
                fnOcultar();
            }
        }
        private void gridItem_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            //CARGAR CAMPOS AL HACER CLICK EN UN FILA DE LA GRILLA
            fnCargarCampos(viewItem);
        }
        private void gridItem_KeyUp(object sender, KeyEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            //CARGAR CAMPOS AL HACER CLICK EN UN FILA DE LA GRILLA
            fnCargarCampos(viewItem);
        }
        private void txtorden_KeyPress_1(object sender, KeyPressEventArgs e)
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
        private void txtcodigo_KeyDown(object sender, KeyEventArgs e)
        {
            //MANIPULAR TECLA ENTER
            //VERICAR SI LO QUE ESTA ESCRITO ES UN CODIGO VALIDO
            bool cod = false;
            if (e.KeyData == Keys.Enter)
            {
                if (txtcodigo.Text != "" && txtcodigo.ReadOnly == false)
                {
                    cod = fnBuscarItem(txtcodigo);
                    //SI RETORNA TRUE ES PORQUE EL CODIGO YA EXISTE EN BD
                    if (cod)
                    {
                        lblCodigo.Visible = true;
                        lblCodigo.Text = "Codigo ingresado ya existe";
                        txtcodigo.EnterMoveNextControl = false;
                    }
                    else
                    {
                        //VALIDAR QUE EL CODIGO DE ITEM NO SEA CODIGO DE VARIABLE
                        bool codVar = false;
                        codVar = fnItemVariable(txtcodigo.Text);
                        if (codVar)
                        {
                            lblCodigo.Visible = true;
                            lblCodigo.Text = "Codigo ingresado ya esta en uso como variable";
                        }
                        else
                        {
                            lblCodigo.Visible = false;
                            txtcodigo.EnterMoveNextControl = true;
                        }

                    }
                }
            }

        }

        #endregion

        private void txttipo_EditValueChanged(object sender, EventArgs e)
        {
            //HABERES
            if (txttipo.EditValue.ToString() == "1")
            {
                cbproporcional.Enabled = true;
                cbimprime.Enabled = true;
                cbPrevisualizar.Enabled = true;
                cbinfo.Enabled = true;
                cbbase.Enabled = true;
                cbTope.Enabled = false;
                txtModalidad.ReadOnly = false;
            }
            //HABERES EXENTOS
            else if (txttipo.EditValue.ToString() == "2")
            {
                cbproporcional.Enabled = true;
                cbimprime.Enabled = true;
                cbPrevisualizar.Enabled = true;
                cbinfo.Enabled = true;
                cbbase.Enabled = true;
                cbTope.Enabled = false;
                txtModalidad.ReadOnly = false;
            }
            //ASIGNACIONES FAMILIARES
            else if (txttipo.EditValue.ToString() == "3")
            {
                cbproporcional.Enabled = true;
                cbimprime.Enabled = true;
                cbPrevisualizar.Enabled = true;
                cbinfo.Enabled = true;
                cbbase.Enabled = true;
                cbTope.Enabled = false;
                txtModalidad.ReadOnly = true;
            }
            //LLSS
            else if (txttipo.EditValue.ToString() == "4")
            {
                cbproporcional.Enabled = true;
                cbimprime.Enabled = true;
                cbPrevisualizar.Enabled = true;
                cbinfo.Enabled = true;
                cbbase.Enabled = true;
                cbTope.Enabled = false;
                txtModalidad.ReadOnly = true;
            }
            //DESCUENTOS
            else if (txttipo.EditValue.ToString() == "5")
            {
                cbproporcional.Enabled = true;
                cbimprime.Enabled = true;
                cbPrevisualizar.Enabled = true;
                cbinfo.Enabled = true;
                cbbase.Enabled = true;
                cbTope.Enabled = true;
                txtModalidad.ReadOnly = true;
            }
            //APORTES
            else if (txttipo.EditValue.ToString() == "6")
            {
                cbproporcional.Enabled = true;
                cbimprime.Enabled = true;
                cbPrevisualizar.Enabled = true;
                cbinfo.Enabled = true;
                cbbase.Enabled = true;
                cbTope.Enabled = false;
                txtModalidad.ReadOnly = true;
            }
            //CONTRIBUCIONES
            else if (txttipo.EditValue.ToString() == "6")
            {
                cbproporcional.Enabled = true;
                cbimprime.Enabled = true;
                cbPrevisualizar.Enabled = true;
                cbinfo.Enabled = true;
                cbbase.Enabled = true;
                cbTope.Enabled = false;
                txtModalidad.ReadOnly = true;
            }
        }

        private void txtorden_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                //SI SE PRESIONA ENTER VALIDAMOS VALOR ESCRITO EN CAMPO
                bool valido = fnOrdenNumber(txtorden);
                if (valido == false)
                {
                    lblOrden.Visible = true;
                    lblOrden.Text = "Numero fuera de rango, valores entre 1 y 200";
                }
                else
                {
                    lblOrden.Visible = false;
                }
            }
        }

        private void txtformula_EditValueChanged(object sender, EventArgs e)
        {
            CodigoFormula = txtformula.EditValue.ToString();
        }

        private void btnFormula_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            //RECARGAR COMBOBOX FORMULA
            fnComboFormula(txtformula);
        }

        private void txtformula_DoubleClick(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmformula"))
            {
                //MOSTRAR FORMULARIO FORMULA
                frmFormula formula = new frmFormula(TipoActual, CodigoFormula);
                formula.opener = this;
                formula.ShowDialog();
            }
        }

        private void txtdescripcion_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void txtcodigo_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtdescripcion_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txttipo_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtformula_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtorden_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            if (viewItem.RowCount > 0)
            {
                string codItem = (string)viewItem.GetFocusedDataRow()["coditem"];
                if (codItem != "")
                {
                    if (fnVerificarCambios(codItem))
                    {
                        DialogResult adv = XtraMessageBox.Show("Hay cambios sin guardar, ¿Deseas cerrar de todas maneras?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
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
            {
                Close();
            }

        }

        private void separador1_Click(object sender, EventArgs e)
        {

        }

        private void cbTope_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void txtAbrevia_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || char.IsLetter(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void cbContabilidad_CheckedChanged(object sender, EventArgs e)
        {
            if (cbContabilidad.Checked)
                DisableControl(true);
            else
                DisableControl(false);
        }

        private void DisableControl(bool Option)
        {

                txtCuentaCredito.Enabled = Option;
                txtCuentaDebito.Enabled = Option;
                txtDato1Credito.Enabled = Option;
                txtDato1Credito.Text = "";
                txtDato2Credito.Enabled = Option;
                txtDato2Credito.Text = "";
                txtDato3Credito.Enabled = Option;
                txtDato3Credito.Text = "";
                txtDato4Credito.Enabled = Option;
                txtDato4Credito.Text = "";
                txtDato1Debito.Enabled = Option;
                txtDato1Debito.Text = "";
                txtDato2Debito.Enabled = Option;
                txtDato2Debito.Text = "";
                txtDato3Debito.Enabled = Option;
                txtDato3Debito.Text = "";
                txtDato4Debito.Enabled = Option;
                txtDato4Debito.Text = "";

        }

        private void txtCuentaCredito_EditValueChanged(object sender, EventArgs e)
        {
            LookUpEdit Combo = sender as LookUpEdit;                    
            if (Combo.Properties.DataSource != null)
            {
                CodEsqCredito = Convert.ToInt32(Combo.GetColumnValue("CodEs"));
                CodCuentaCredito = Convert.ToInt32(Combo.GetColumnValue("CodCuenta"));
            }                       
        }

        private void txtCuentaDebito_EditValueChanged(object sender, EventArgs e)
        {
            LookUpEdit Combo = sender as LookUpEdit;
            if (Combo.Properties.DataSource != null)
            {
                CodEsqDebito = Convert.ToInt32(Combo.GetColumnValue("CodEs"));
                CodCuentaDebito = Convert.ToInt32(Combo.GetColumnValue("CodCuenta"));
            }
        }

        private void btnTipoTotales_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            TipoActual = 7;

            //SI ES YES CAMBIAMOS GRILLA Y TODO LO DEMAS
            fnBotonClick(TipoActual);
            //CAMBIAR COLOR BOTON
            fnTotalesActivo();
            fnOcultar();
        }
    }

    class test
    {
        public string key{ get; set; }
        public string desc { get; set; }
    }
}