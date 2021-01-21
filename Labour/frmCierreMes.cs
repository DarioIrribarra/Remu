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
using System.Threading;

namespace Labour
{
    public partial class frmCierreMes : DevExpress.XtraEditors.XtraForm
    {
        public IMenu Opener { get; set; }

        public frmCierreMes()
        {
            InitializeComponent();           
        }

        private void panelControl2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void frmCierreMes_Load(object sender, EventArgs e)
        {
            if (Persona.ExistenRegistros(Calculo.PeriodoObservado) == false)
            {                
                XtraMessageBox.Show("Hemos detectado que no hay registros para el periodo evaluado, por lo que no es posible realizar un nuevo cierre de mes", "Informacion Importante", MessageBoxButtons.OK, MessageBoxIcon.Warning);             
                BtnCerrarMes.Enabled = false;
                this.Close();
            }
            else
                BtnCerrarMes.Enabled = true;            

            txtPeriodoActual.Focus();
            txtPeriodoActual.Text = Calculo.PeriodoObservado + "";
            txtActual.Text = fnSistema.PrimerMayuscula(fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(Calculo.PeriodoObservado)));
            //BtnCerrarMes.Text = "Cerrar Mes " + Calculo.PeriodoObservado;
            txtNuevoPeriodo.Text = fnSistema.PeriodoSiguiente(Calculo.PeriodoObservado) + "";
            txtNuevo.Text = fnSistema.PrimerMayuscula(fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(fnSistema.PeriodoSiguiente(Calculo.PeriodoObservado))));
        }

        private void BtnCerrarMes_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            //VER SI EL USUARIO ESTA BLOQUEADO 
            if (User.Bloqueado())
            { XtraMessageBox.Show("No puedes realizar esta operación", "Información", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            //Coleccion de todos los formularios Abiertos!!
            // FormCollection formularios = Application.OpenForms;               
            if (txtPeriodoActual.Text == "") { XtraMessageBox.Show("Periodo no valido", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
            if (txtNuevoPeriodo.Text == "") { XtraMessageBox.Show("Nuevo Periodo no valido", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (ExistenCalculos() == false) { XtraMessageBox.Show($"No se detectaron calculos de remuneraciones para el periodo {fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(Calculo.PeriodoObservado))}", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
            if (HayRegistros() == false) { XtraMessageBox.Show($"No se detectaron registros para el periodo {fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(Calculo.PeriodoObservado))}", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
            if (Existenitems() == false) { XtraMessageBox.Show($"No se detectaron registros para el periodo {fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(Calculo.PeriodoObservado))}", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            DialogResult advertencia = XtraMessageBox.Show($"Estás seguro de cerrar el mes {fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(Calculo.PeriodoObservado))}?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (advertencia == DialogResult.Yes)
            {
                //CierreMes();
                CierreMesv2();
                //Thread.Sleep(2000);
                Close();
            } 
                             
        }

        #region "MANEJO DE DATOS"  

        //VERIFICAR SI HAY REGISTROS DE TRABAJADORES
        private bool HayRegistros()
        {
            //PERIODO EN CURSO
            int periodo = 0;
            bool existen = false;

            periodo = Calculo.PeriodoObservado;
            string sql = "SELECT contrato FROM TRABAJADOR WHERE anomes=@periodo";
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

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            existen = true;
                        }
                        else
                        {
                            existen = false;
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

            return existen;
        }

        private void test(ref FormCollection fr)
        {
            foreach (Form x in fr)
            {
                if (x != null)
                {
                    if (x != this && x.Name != "frmMain")
                    {
                        x.Close();
                    }
                }
            }
        }

        //VERIFICAR SI HAY FORMULARIOS DISTINTOS A ESTE ABIERTOS
        private void CloseForm(string name)
        {
            //NAME REPRESENTARIA EL FORM ACTUAL
            FormCollection fc = Application.OpenForms;

            foreach (Form frm in fc)
            {
                if (frm.Text != name)
                {
                    //CERRAMOS TODOS EL QUE SEA DISTINTO
                    XtraMessageBox.Show(frm.Text);
                }
            }
        }

        //OTRA OPCION PARA CERRAR TODOS LOS FORMULARIOS ABIERTOS EXCEPTO EL MAIN
        private void CierraTodo()
        {
            FormCollection formularios = Application.OpenForms;

            foreach (Form formulario in formularios)
            {
                if (formulario.Name != "FrmMain" && formulario.Name != "FrmCierreMes")
                        formulario.Close();
            }            
        }

        private bool TrabajadorSigueActivo(string pContrato, int pPeriodo)
        {
            string sql = "SELECT salida FROM trabajador WHERE contrato=@pContrato";
            SqlCommand cmd;
            DateTime fecha = DateTime.Now.Date;
            DateTime PeriodoAbierto = fnSistema.UltimoDiaMes(pPeriodo);
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pContrato", pContrato));

                        fecha = Convert.ToDateTime(cmd.ExecuteReader());

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            //SI LA FECHA DE SALIDA DEL CONTRATO ES MAYOR A EL ULTIMO DIA DEL PERIODO EVALUADO, 
            //EL TRABAJADOR SIGUE ACTIVO
            //SI LA FECHA ES MENOR O IGUAL AL ULTIMO DIA DEL PERIODO EVALUADO EL TRABAJADOR QUEDA INACTIVO
            if (fecha <= PeriodoAbierto)
                return false;
            else
                return true;

        }

        /// <summary>
        /// Realiza cierre de mes
        /// </summary>
        private void CierreMes()
        {
            //DEJAMOS PROCESO ACTIVO
            Calculo.ChangeStatus(1, "002");

            SqlCommand cmd;
            int count = 0;
            string sqlTransaction = "BEGIN TRY " +
                                        "BEGIN TRANSACTION " +
                                        "declare @inactivo INT " +                                     
                                            "set @inactivo = (SELECT count(*) FROM trabajador " +
                                            "WHERE anomes = @pActual AND salida <= @pFecha)  " +
                                             "INSERT INTO PARAMETRO(anomes) values(@pSiguiente) " +
                                                    "SELECT * INTO #indices FROM valoresMes " +
                                                    "WHERE anomes = @pActual " +
                                                "update #indices SET anomes = @pSiguiente, uf = 0, utm = 0  " +
                                                    "INSERT INTO valoresMes " +
                                                    "SELECT * FROM #indices " +
                                        "IF @inactivo<>0 " +
                                            "BEGIN " +
                                                    "UPDATE trabajador SET status = 0 WHERE salida <= @pFecha OR anomes <=@pActual " +
                                                    "SELECT * INTO #copia1 FROM trabajador WHERE  " +
                                                    "anomes = @pActual AND salida > @pFecha " +
                                                    "UPDATE #copia1 set anomes=@pSiguiente  " +
                                                    "UPDATE #copia1 set status=1 " +
                                                    "INSERT INTO trabajador " +
                                                        "SELECT * FROM #copia1  " +
                                                    "SELECT itemTrabajador.rut, itemTrabajador.contrato, coditem, numitem, formula,  " +
                                                    "tipo, orden, esclase, valor, valorcalculado, proporcional, contope, cuota, " +
                                                    "permanente, itemTrabajador.anomes, porc, uf, pesos, modalidad " +
                                                    "INTO #permanentes1 FROM ITEMTRABAJADOR  " +
                                                    "INNER JOIN trabajador ON trabajador.contrato = itemTrabajador.contrato " +
                                                    "AND trabajador.anomes = itemTrabajador.anomes " +
                                                    "WHERE itemTrabajador.anomes = @pActual AND (permanente = 1 OR esclase=1) AND salida > @pFecha " +
                                                        "UPDATE #permanentes1 SET anomes = @pSiguiente " +
                                                    "INSERT INTO ITEMTRABAJADOR(rut, contrato, coditem, numitem, formula, tipo, " +
                                                    "orden, esclase, valor, valorcalculado, proporcional, contope, cuota, permanente, anomes, porc, uf, pesos, modalidad) " +
                                                    "SELECT rut, contrato, coditem, numitem, formula, tipo, orden, esclase, valor, " +
                                                    "valorcalculado, proporcional, contope, cuota, permanente, anomes, porc, uf, pesos, modalidad FROM #permanentes1 " +
                                                "END" +
                                        " ELSE " +
                                            "BEGIN " +
                                                "UPDATE trabajador SET status = 0 WHERE salida <= @pFecha OR anomes <=@pActual " +
                                                "SELECT * INTO #copia FROM trabajador WHERE  " +
                                                "anomes = @pActual AND salida > @pFecha " +
                                                "UPDATE #copia set anomes=@pSiguiente  " +
                                                "UPDATE #copia set status=1 " +
                                                "INSERT INTO trabajador " +
                                                    "SELECT * FROM #copia  " +
                                                "SELECT itemTrabajador.rut, itemTrabajador.contrato, coditem, numitem, formula,  " +
                                                "tipo, orden, esclase, valor, valorcalculado, proporcional, contope, cuota, " +
                                                "permanente, itemTrabajador.anomes, porc, uf, pesos, modalidad " +
                                                "INTO #permanentes FROM ITEMTRABAJADOR  " +
                                                "INNER JOIN trabajador ON trabajador.contrato = itemTrabajador.contrato " +
                                                "AND trabajador.anomes = itemTrabajador.anomes " +
                                                "WHERE itemTrabajador.anomes = @pActual AND (permanente = 1 OR esclase=1) AND salida > @pFecha " +
                                                    "UPDATE #permanentes SET anomes = @pSiguiente " +
                                                "INSERT INTO ITEMTRABAJADOR(rut, contrato, coditem, numitem, formula, tipo, " +
                                                "orden, esclase, valor, valorcalculado, proporcional, contope, cuota, permanente, anomes, porc, uf, pesos, modalidad) " +
                                                "SELECT rut, contrato, coditem, numitem, formula, tipo, orden, esclase, valor, " +
                                                "valorcalculado, proporcional, contope, cuota, permanente, anomes, porc, uf, pesos, modalidad FROM #permanentes " +                                                
                                             "END " +                                               
                                        "COMMIT TRANSACTION " +
                                        " END TRY " +
                                        "BEGIN CATCH " +
                                            "IF @@TRANCOUNT > 0 " +
                                                "BEGIN " +
                                                    "ROLLBACK " +
                                                "END " +
                                        "END CATCH";

/*INGRESAR NUEVO PERIODO EN TABLA PARAMETROS*/
/*INSERT INTO parametros(anomes, version) values(201806, '1.0')*/
/*INGRESAR NUEVO PERIODO EN VALORES MES*/
/*INSERT INTO valoresMes(anomes, uf, utm, ingresominimo, sis, topeafp, topesec) VALUES(201806, 0, 0, 0, 0, 0, 0)*/
//COMMIT TRANSACTION";

            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sqlTransaction, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pActual", Calculo.PeriodoObservado));                        
                        cmd.Parameters.Add(new SqlParameter("@pFecha", fnSistema.UltimoDiaMes(Calculo.PeriodoObservado)));                        
                        cmd.Parameters.Add(new SqlParameter("@pSiguiente", fnSistema.PeriodoSiguiente(Calculo.PeriodoObservado)));                                                
                    
                        count = cmd.ExecuteNonQuery();

                        if (count > 0)
                        {
                            XtraMessageBox.Show("Cierre de mes realizado correctamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            //ACTUALIZAMOS VARIABLE PERIODO OBSERVADO
                            Calculo.PeriodoObservado = Calculo.PeriodoEvaluar();
                            BtnCerrarMes.Enabled = false;
                            Calculo.ChangeStatus(0, "002");
                            User.CleanLastView();
                            //ACTUALIZAR PERIODO EN BARRA INFERIOR PRINCIPAL        
                            if (Opener != null)
                                Opener.CargarUser($"User: {User.NombreCompleto()}", $"BD: {fnSistema.pgDatabase}", $"{fnSistema.PrimerMayuscula(fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(Calculo.PeriodoObservado)))}");
                        }
                        else
                        {
                            XtraMessageBox.Show("Error al intentar realizar cierre", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            Calculo.ChangeStatus(0, "002");
                        }

                        //Calculo.ChangeStatus(0, "002");
                        cmd.Dispose();
                        fnSistema.sqlConn.Close();

                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
                Calculo.ChangeStatus(0, "002");
            }
          
        }

        /// <summary>
        /// Realiza cierre de mes v2
        /// <para>Agrega items que tienen cuotas</para>
        /// </summary>
        private void CierreMesv2()
        {
            #region "SQL"
            string sql = "BEGIN TRY  " +
                            "BEGIN TRANSACTION " +
                             " INSERT INTO PARAMETRO(anomes) values(@pSiguiente) " +
                                  "SELECT * INTO #indices FROM valoresMes " +
                                  "WHERE anomes = @pActual " +
                                  " UPDATE #indices SET anomes = @pSiguiente, uf = 0, utm = 0, ufMesAnt=0  " +
                             " INSERT INTO valoresMes " +
                                 "SELECT * FROM #indices " +
                                 //"  UPDATE trabajador SET status = 0 WHERE salida <= @pFecha OR anomes <= @pActual " +
                                 " SELECT * INTO #copia1 FROM trabajador WHERE " +
                                 "anomes = @pActual AND salida > @pFecha AND status= 1 " +
                                 "UPDATE #copia1 set anomes=@pSiguiente " +
                                 "UPDATE #copia1 set status=1 " +
                                 "INSERT INTO trabajador " +
                                    "SELECT * FROM #copia1 " +
                             "SELECT itemTrabajador.rut, itemTrabajador.contrato, coditem, " +
                                    "row_number() OVER(PARTITION BY itemtrabajador.CONTRATO ORDER by itemtrabajador.contrato) as numitem, formula,  " +
                                     "tipo, orden, esclase, valor, valorcalculado, proporcional, contope, cuota,  " +
                                     "permanente, itemTrabajador.anomes, porc, uf, pesos, suspendido " +
                                  "INTO #permanentes1 FROM ITEMTRABAJADOR " +
                                     "INNER JOIN trabajador ON trabajador.contrato = itemTrabajador.contrato " +
                                     "AND trabajador.anomes = itemTrabajador.anomes " +
                                     "WHERE itemTrabajador.anomes = @pActual AND(permanente = 1 OR esclase = 1 OR suspendido=1) AND salida > @pFecha AND cuota = '0' AND status=1 " +
                             "UPDATE #permanentes1 SET anomes = @pSiguiente  " +
                             "INSERT INTO ITEMTRABAJADOR(rut, contrato, coditem, numitem, formula, tipo, " +
                                     "orden, esclase, valor, valorcalculado, proporcional, contope, cuota, permanente, anomes, porc, uf, pesos, suspendido) " +
                                     "SELECT rut, contrato, coditem, numitem, formula, tipo, orden, esclase, valor, " +
                                     "valorcalculado, proporcional, contope, cuota, permanente, anomes, porc, uf, pesos, suspendido FROM #permanentes1  " +
                                     "SELECT itemTrabajador.rut, itemTrabajador.contrato, coditem, numitem, formula,  " +
                                     "tipo, orden, esclase, valor, valorcalculado, proporcional, contope,  " +
                                     " CAST(CAST(SUBSTRING(cuota, 0, charindex('/', cuota)) AS int) + 1 AS VARCHAR) + '/' + RIGHT(cuota, LEN(cuota) - CHARINDEX('/', cuota)) as cuota, " +
                                     "permanente, itemTrabajador.anomes, porc, uf, pesos, suspendido " +
                             "INTO #prestamos FROM ITEMTRABAJADOR " +
                                     "INNER JOIN trabajador ON trabajador.contrato = itemTrabajador.contrato " +
                                     "AND trabajador.anomes = itemTrabajador.anomes " +
                                     "WHERE itemTrabajador.anomes = @pActual " +
                                     "AND(cuota <> '0' AND CAST(SUBSTRING(cuota, 0, charindex('/', cuota)) AS INT) < CAST(RIGHT(cuota, LEN(cuota) - CHARINDEX('/', cuota)) AS INTEGER)) " +
                                     "AND salida > @pFecha " +
                                   "UPDATE #prestamos SET anomes = @pSiguiente " +
                                     "INSERT INTO itemtrabajador(rut, contrato, coditem, numitem, formula, tipo, orden, esclase, valor, valorcalculado, " +
                                     "proporcional, contope, cuota, permanente, anomes, porc, uf, pesos, suspendido) " +
                                     "SELECT rut, contrato, coditem, COALESCE(num.maxs, 0) +row_number() OVER(PARTITION BY CONTRATO ORDER by contrato) as numitem, " +
                                     "formula, tipo, orden, esclase, valor, valorcalculado, proporcional, contope, cuota, permanente, anomes, porc, uf, pesos, suspendido " +
                                     "FROM #prestamos " +
                                     "CROSS JOIN " +
                                     "(SELECT MAX(numitem)as maxs FROM #permanentes1 WHERE anomes = @pSiguiente) as num " +
                              "COMMIT TRANSACTION " +
                          "END TRY " +
                                    "BEGIN CATCH " +
                                        "IF @@TRANCOUNT > 0 " +
                                            "BEGIN " +
                                                "ROLLBACK " +
                                            "END " +
                                    "END CATCH";
            #endregion

            //DEJAMOS PROCESO ACTIVO
            Calculo.ChangeStatus(1, "002");

            SqlCommand cmd;
            SqlConnection cn;
            int count = 0;

            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            DateTime f = fnSistema.UltimoDiaMes(Calculo.PeriodoObservado);
                            int f1 = fnSistema.PeriodoSiguiente(Calculo.PeriodoObservado);

                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@pActual", Calculo.PeriodoObservado));
                            cmd.Parameters.Add(new SqlParameter("@pFecha", fnSistema.UltimoDiaMes(Calculo.PeriodoObservado)));
                            cmd.Parameters.Add(new SqlParameter("@pSiguiente", fnSistema.PeriodoSiguiente(Calculo.PeriodoObservado)));

                            count = cmd.ExecuteNonQuery();

                            if (count > 0)
                            {
                                XtraMessageBox.Show("Cierre de mes realizado correctamente", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                logRegistro log = new logRegistro(User.getUser(), "SE CIERRA MES " + fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(Calculo.PeriodoObservado)), "", fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(Calculo.PeriodoObservado)), fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(fnSistema.PeriodoSiguiente(Calculo.PeriodoObservado))), "INGRESAR");
                                log.Log();

                                //ACTUALIZAMOS VARIABLE PERIODO OBSERVADO
                                Calculo.PeriodoObservado = Calculo.PeriodoEvaluar();
                                BtnCerrarMes.Enabled = false;
                                Calculo.ChangeStatus(0, "002");
                                User.CleanLastView();
                                //ACTUALIZAR PERIODO EN BARRA INFERIOR PRINCIPAL        
                                if (Opener != null)
                                    Opener.CargarUser($"User: {User.NombreCompleto()}", $"BD: {fnSistema.pgDatabase}", $"{fnSistema.PrimerMayuscula(fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(Calculo.PeriodoObservado)))}");
                            }
                            else
                            {
                                XtraMessageBox.Show("Error al intentar realizar cierre", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                Calculo.ChangeStatus(0, "002");
                            }

                            //Calculo.ChangeStatus(0, "002");
                            cmd.Dispose();
                            fnSistema.sqlConn.Close();

                        }
                    }
                }               
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
                Calculo.ChangeStatus(0, "002");
            }

        }

        private void Test()
        {
            string sql = "declare @number INT set @number = 5 " +
                " UPDATE itemtrabajador SET @number = numitem = @number + 1 " +
                " WHERE anomes = 201806 AND contrato = '125291112'";

            int count = 0;
            SqlCommand cmd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        count = cmd.ExecuteNonQuery();
                        if (count > 0)
                            XtraMessageBox.Show("Correcto!");
                        else
                            XtraMessageBox.Show("Incorrecto!");
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

        //1- VERIFICAR SI EL CONTRATO SIGUE ACTIVO DE ACUERDO A FECHA DE TERMINO DE CONTRATO
        //2- SI ESTA ACTIVO SE GENERA NUEVO REGISTRO CON LOS MISMOS DATOS DEL TRABAJADOR PERO CON NUEVO PERIODO EVALUADO
        //3- TAMBIEN SE DEBEN GENERAR EN TABLA ITEMTRABAJADOR LOS ITEMS ASOCIADO DE ACUERDO A NUEVO PERIODO Y A CLASE REMUNERACION DE TRABAJADOR
        //4- SI EL CONTRATO TERMINA EN ESTE PERIODO, DEJAR REGISTRO COMO INACTIVO

        private bool ExistenCalculos()
        {
            string sql = "SELECT count(*) FROM liquidacionHistorico WHERE anomes = @periodo";
            SqlCommand cmd;
            int count = 0;
            bool existen = false;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@periodo", Calculo.PeriodoObservado));

                        count = Convert.ToInt32(cmd.ExecuteScalar());

                        if (count > 0)
                            existen = true;
                    }
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return existen;
        }

        private bool Existenitems()
        {
            string sql = "SELECT count(*) FROM itemtrabajador WHERE anomes=@periodo";
            SqlCommand cmd;
            int count = 0;
            bool existe = false;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@periodo", Calculo.PeriodoObservado));

                        count = Convert.ToInt32(cmd.ExecuteScalar());

                        if (count > 0)
                            existe = true;

                        cmd.Dispose();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return existe;
        }
        #endregion

        private void txtPeriodoActual_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtNuevoPeriodo_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}