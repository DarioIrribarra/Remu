using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.Skins;
using System.Data.SQLite;
using System.Data.SqlClient;
using DevExpress.LookAndFeel;
using DevExpress.XtraBars;
using System.IO;
using System.Globalization;

        private void barItemResumenProceso_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmresproceso"))
            {
                //VE FICHAS PRIVADAS
                ShowPrivadas = User.ShowPrivadas();

                if (Calculo.GetTotalRegistros(ShowPrivadas) == 0)
                { XtraMessageBox.Show("No se encontraron registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                Form frm = this.MdiChildren.FirstOrDefault(x => x is frmResumenProceso);

                if (frm != null)
                {
                    frm.BringToFront();
                    return;
                }

                //SI NO EXISTE CREAMOS EL FORM
                frm = new frmResumenProceso();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
            else
                XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        private void baritemFeriados_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmferiado"))
            {
                Form frm = this.MdiChildren.FirstOrDefault(x => x is frmFeriado);

                if (frm != null)
                {
                    frm.BringToFront();
                    return;
                }

                //SI NO EXISTE CREAMOS EL FORM
                frm = new frmFeriado();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
            else
                XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        private void baritemLiqHistorica_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmliqhistorico"))
            {
                //VE FICHAS PRIVADAS
                ShowPrivadas = User.ShowPrivadas();

                if (Calculo.GetTotalRegistros(ShowPrivadas) == 0)
                { XtraMessageBox.Show("No se encontraron registros", "Informacion importante", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                frmLiquidacionHistorica liq = new frmLiquidacionHistorica();
                liq.StartPosition = FormStartPosition.CenterScreen;
                liq.ShowDialog();
            }
            else
                XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        private void baritemCierreMes_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmcierremes"))
            {
                //VE FICHAS PRIVADAS
                ShowPrivadas = User.ShowPrivadas();

                if (Calculo.GetTotalRegistros(ShowPrivadas) == 0)
                { XtraMessageBox.Show("No se encontraron registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                //VERIFICAMOS SI HAY ALGUN PROCESO CORRIENDO
                if (Calculo.ViewStatus())
                { XtraMessageBox.Show("Hay un proceso en ejecucion", "Proceso activo", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }                

                frmCierreMes cierre = new frmCierreMes();
                cierre.StartPosition = FormStartPosition.CenterScreen;
                cierre.Opener = this;
                cierre.ShowDialog();
            }
            else
                XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        private void baritemDatosEmpleado_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmfichaempleado"))
            {
                //VE FICHAS PRIVADAS
                ShowPrivadas = User.ShowPrivadas();

                if (Calculo.GetTotalRegistros(ShowPrivadas) == 0)
                { XtraMessageBox.Show("No hay registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                Form frm = this.MdiChildren.FirstOrDefault(x => x is frmDatosEmpleado);

                if (frm != null)
                {
                    frm.BringToFront();
                    return;
                }

                //SI NO EXISTE CREAMOS EL FORM
                frm = new frmDatosEmpleado();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
            else
                XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        private void batitemLibroRemuneraciones_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmlibroremun"))
            {
                //VE FICHAS PRIVADAS
                ShowPrivadas = User.ShowPrivadas();

                if (Calculo.GetTotalRegistros(ShowPrivadas) == 0)
                { XtraMessageBox.Show("No se encontraron registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                Form frm = this.MdiChildren.FirstOrDefault(x => x is frmLibroRemuneraciones);

                if (frm != null)
                {
                    frm.BringToFront();
                    return;
                }

                //SI NO EXISTE CREAMOS EL FORM
                frm = new frmLibroRemuneraciones();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
            else
                XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
           
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmplanillasalud"))
            {
                //VE FICHAS PRIVADAS
                ShowPrivadas = User.ShowPrivadas();

                if (Calculo.GetTotalRegistros(ShowPrivadas) == 0)
                { XtraMessageBox.Show("No se encontraron registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                Form frm = this.MdiChildren.FirstOrDefault(x => x is frmPlanillaIsapre);

                if (frm != null)
                {
                    frm.BringToFront();
                    return;
                }

                //SI NO EXISTE CREAMOS EL FORM
                frm = new frmPlanillaIsapre();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
            else
                XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        private void baritemAfp_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmplanillaafp"))
            {
                //VE FICHAS PRIVADAS
                ShowPrivadas = User.ShowPrivadas();

                if (Calculo.GetTotalRegistros(ShowPrivadas) == 0)
                { XtraMessageBox.Show("No se encontraron registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                Form frm = this.MdiChildren.FirstOrDefault(x => x is frmPlanillaAfp);

                if (frm != null)
                {
                    frm.BringToFront();
                    return;
                }

                //SI NO EXISTE CREAMOS EL FORM
                frm = new frmPlanillaAfp();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
            else
                XtraMessageBox.Show("No tienes privilegios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);

        }

        private void barbtnPagoMutual_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmplanillamutual"))
            {
                //VE FICHAS PRIVADAS
                ShowPrivadas = User.ShowPrivadas();

                if (Calculo.GetTotalRegistros(ShowPrivadas) == 0)
                { XtraMessageBox.Show("No se encontraron registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }                

                Form frm = this.MdiChildren.FirstOrDefault(x => x is frmPlanillaMutual);

                if (frm != null)
                {
                    frm.BringToFront();
                    return;
                }

                //SI NO EXISTE CREAMOS EL FORM
                frm = new frmPlanillaMutual();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
            else
                XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmconjuntobusqueda"))
            {
                FrmFiltroBusqueda filtro = new FrmFiltroBusqueda();
                filtro.StartPosition = FormStartPosition.CenterParent;
                filtro.ShowDialog();
            }
            else
                XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);                     
        }

        private void barsubitemInformacion_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frminfoitem"))
            {
                Form frm = this.MdiChildren.FirstOrDefault(x => x is frmDetalleItem);

                if (frm != null)
                {
                    frm.BringToFront();
                    return;
                }

                //SI NO EXISTE CREAMOS EL FORM
                frm = new frmDetalleItem();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
            else
                XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
           
        }

        private void barsubitemAgrega_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmadditemmasivo"))
            {                
                Form frm = this.MdiChildren.FirstOrDefault(x => x is frmAgregarItems);

                if (frm != null)
                {
                    frm.BringToFront();
                    return;
                }

                //SI NO EXISTE CREAMOS EL FORM
                frm = new frmAgregarItems();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
            else
                XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        
        }

        private void barsubitemEliminar_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmeliminaitemmasivo"))
            {

                Form frm = this.MdiChildren.FirstOrDefault(x => x is FrmEliminarItems);

                if (frm != null)
                {
                    frm.BringToFront();
                    return;
                }

                //SI NO EXISTE CREAMOS EL FORM
                frm = new FrmEliminarItems();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
            else
                XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        private void baritemModificarInformacion_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmeditinfoempleado"))
            {
                Form frm = this.MdiChildren.FirstOrDefault(x => x is FrmModificarInfoEmpleado);

                if (frm != null)
                {
                    frm.BringToFront();
                    return;
                }

                //SI NO EXISTE CREAMOS EL FORM
                frm = new FrmModificarInfoEmpleado();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
            else
                XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        private void baritemAgregaInformacion_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmimportdata"))
            {
                Form frm = this.MdiChildren.FirstOrDefault(x => x is FrmAgregarEmpleados);

                if (frm != null)
                {
                    frm.BringToFront();
                    return;
                }

                //SI NO EXISTE CREAMOS EL FORM
                frm = new FrmAgregarEmpleados();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
            else
                XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);               
        }

        private void barsubExportarArchivo_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmexportdata"))
            {

                Form frm = this.MdiChildren.FirstOrDefault(x => x is frmExportarEmpleados);

                if (frm != null)
                {
                    frm.BringToFront();
                    return;
                }

                //SI NO EXISTE CREAMOS EL FORM
                frm = new frmExportarEmpleados();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
            else
                XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //frmSeleccionItemElimina sel = new frmSeleccionItemElimina("PRESTAM");
            //sel.StartPosition = FormStartPosition.CenterParent;
            //sel.Show();
        }

        private void barSuperior_DockChanged(object sender, EventArgs e)
        {

        }

        private void barbtnSeguridad_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //SESION NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmseguridad") == false)
            { XtraMessageBox.Show("No tienes los permisos necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            Form frm = this.MdiChildren.FirstOrDefault(x => x is frmSeguridad);

            if (frm != null)
            {
                frm.BringToFront();
                return;
            }

            //SI NO EXISTE CREAMOS EL FORM
            frm = new frmSeguridad();
            frm.MdiParent = this;
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.Show();
        }

        private void barbtnarchivoprevired_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDA EN SESION
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmprevired") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            //VE FICHAS PRIVADAS
            ShowPrivadas = User.ShowPrivadas();

            if (Calculo.GetTotalRegistros(ShowPrivadas) == 0)
            { XtraMessageBox.Show("No se encontraron registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            frmPrevired prev = new frmPrevired();
            prev.StartPosition = FormStartPosition.CenterParent;
            prev.ShowDialog();
        }

        private void barButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
          
        }

        private void barimportaritems_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmimportaritems") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            frmImportarItems items = new frmImportarItems();
            items.StartPosition = FormStartPosition.CenterScreen;
            items.ShowDialog();
        }

        private void barUpdDesdeArchivo_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //SESION ACTIVIDAD
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmimportaupdtrab") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            frmImportarTrabUpd update = new frmImportarTrabUpd();
            update.StartPosition = FormStartPosition.CenterScreen;
            update.ShowDialog();
        }

        private void baritemReabrirMes_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //NUEVA SESION ACTIVIDAD    
            Sesion.NuevaActividad();

            //VER SI EL USUARIO ESTA BLOQUEADO 
            if (User.Bloqueado())
            { XtraMessageBox.Show("No puedes realizar modificaciones", "Información", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmreabreperiodo") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (User.GetLlaveMaestra(User.getUser()) == "0")
            { XtraMessageBox.Show("Solicita a soporte la llave maestra para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

            if (Calculo.ViewStatus())
            { XtraMessageBox.Show("Hay un proceso de calculo en curso, por favor espera un momento", $"Proceso activo por {User.getUser()}", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (Calculo.ExistePeriodoAnterior(fnSistema.fnObtenerPeriodoAnterior(Calculo.PeriodoObservado)) == false)
            { XtraMessageBox.Show("No existe ningun periodo que se pueda reabrir", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

            frmMaestra master = new frmMaestra();
            master.Opener = this;
            master.StartPosition = FormStartPosition.CenterParent;
            master.ShowDialog();
        }

        private void barsubitemModificaritems_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmmodificaitemmasivo") == false)
            { XtraMessageBox.Show("No tienes los provilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            Form frm = this.MdiChildren.FirstOrDefault(x => x is FrmModificaItems);

            if (frm != null)
            {
                frm.BringToFront();
                return;
            }

            //SI NO EXISTE CREAMOS EL FORM
            frm = new FrmModificaItems();
            frm.MdiParent = this;
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.Show();
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {

                //OCURRIÓ ALGUN ERROR Y CERRAMOS EL PROGRAMA SIN ADVERTIR
                //POR EJEMPLO SI LA LICENCIA NO ES VALIDA O LA VERSION ESTÁ OBSOLETA...
                if (Licencia.ForzarCierre)
                    return;

                var result = XtraMessageBox.Show("¿Estás seguro de cerrar la aplicación?", "Informacion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                {                    
                    e.Cancel = true;
                }                    
                else
                {
                    e.Cancel = false;                 
                    //ELIMINAMOS SESSION
                    Sesion.DeleteAccess(User.getUser().ToLower());

                    //VERIFICAR SI QUERDÓ ALGUN PROCESO NO CERRADO CORRECTAMENTE...
                    Monitor.CleanTask(User.getUser());

                }            
        }

        private void frmMain_KeyDown(object sender, KeyEventArgs e)
        {
            Alt_F4 = (e.KeyCode.Equals(Keys.F4) && e.Alt == true);
        }

        private void barItemAignacionFamiliar_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDADA
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmfamtramo") == false)
            { XtraMessageBox.Show("No tienes los provilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            Form frm = this.MdiChildren.FirstOrDefault(x => x is frmAsigFam);

            if (frm != null)
            {
                frm.BringToFront();
                return;
            }

            //SI NO EXISTE CREAMOS EL FORM
            frm = new frmAsigFam();
            frm.MdiParent = this;
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.Show();
        }

        private void baritemCargaMasivaitems_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //ACTIVIDAD
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmimportaritemmasivo") == false)
            { XtraMessageBox.Show("No tienes los provilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            Form frm = this.MdiChildren.FirstOrDefault(x => x is frmCargaMasivaItems);

            if (frm != null)
            {
                frm.BringToFront();
                return;
            }

            //SI NO EXISTE CREAMOS EL FORM
            frmCargaMasivaItems itemsmas = new frmCargaMasivaItems();
            itemsmas.StartPosition = FormStartPosition.CenterScreen;
            itemsmas.CambioEstadoOpen = this;
            itemsmas.Show();
            //frm = new frmCargaMasivaItems();
            //frm.MdiParent = this;
            //frm.StartPosition = FormStartPosition.CenterScreen;
            //frm.Show();
        }

        private void barButtonItem7_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void btnSubitemLicencia_ItemClick(object sender, ItemClickEventArgs e)
        {
            Sesion.NuevaActividad();

            //FORMULARIO SOLO PARA MOSTRAR LA LICENCIA DEL SISTEMA
            FrmLicencia lic = new FrmLicencia();            
            lic.StartPosition = FormStartPosition.CenterParent;
            lic.ShowDialog();
        }

        private void btnItemCaja_ItemClick(object sender, ItemClickEventArgs e)
        {
            //ACTIVIDAD
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmPlanillaCaja") == false)
            { XtraMessageBox.Show("No tienes los provilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            //VE FICHAS PRIVADAS
            ShowPrivadas = User.ShowPrivadas();

            if (Calculo.GetTotalRegistros(ShowPrivadas) == 0)
            { XtraMessageBox.Show("No se encontraron registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            Form frm = this.MdiChildren.FirstOrDefault(x => x is FrmPlanillaCaja);

            if (frm != null)
            {
                frm.BringToFront();
                return;
            }

            Empresa emp = new Empresa();
            emp.SetInfo();

            if (emp.NombreCCAF == 1)
            { XtraMessageBox.Show("Esta empresa no tiene asociada una caja de compensación", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

            //SI NO EXISTE CREAMOS EL FORM
            frm = new FrmPlanillaCaja();
            frm.MdiParent = this;
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.Show();
        }

        private void barBtnCorreo_ItemClick(object sender, ItemClickEventArgs e)
        {
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmconfcorreo") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta función", "Acceso", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            Form frm = this.MdiChildren.FirstOrDefault(x => x is frmConfCorreo);

            if (frm != null)
            {
                frm.BringToFront();
                return;
            }

            //SI NO EXISTE CREAMOS EL FORM
            frm = new frmConfCorreo();
            frm.MdiParent = this;
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.Show();
        }

        private void barBtnCorreoMasivo_ItemClick(object sender, ItemClickEventArgs e)
        {
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmenviocorreo") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta función", "Acceso", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            Form frm = this.MdiChildren.FirstOrDefault(x => x is frmCorreoMasivo);

            if (frm != null)
            {
                frm.BringToFront();
                return;
            }
            
            //SI NO EXISTE CREAMOS EL FORM
            frm = new frmCorreoMasivo();
            frm.MdiParent = this;
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.Show();
        }

        private void barBtnPlanillasSueldo_ItemClick(object sender, ItemClickEventArgs e)
        {
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmplanillasueldos") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta función", "Acceso", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            Form frm = this.MdiChildren.FirstOrDefault(x => x is FrmPlanillaSueldos);

            if (frm != null)
            {
                frm.BringToFront();
                return;
            }

            //SI NO EXISTE CREAMOS EL FORM
            FrmPlanillaSueldos sueldo = new FrmPlanillaSueldos();
            sueldo.CambioEstadoOpen = this;
            sueldo.StartPosition = FormStartPosition.CenterParent;
            sueldo.Show();

            //frm = new FrmPlanillaSueldos();
            //frm.MdiParent = this;
            //frm.StartPosition = FormStartPosition.CenterScreen;
            //frm.Show();
        }

        private void barBtnNominasBanco_ItemClick(object sender, ItemClickEventArgs e)
        {
            Sesion.NuevaActividad();

            //RUTA DLL
            //string RutaDll = Environment.CurrentDirectory + "\\Nominas.dll";
            //string BciDll = Environment.CurrentDirectory + "\\BancoBci.dll";
            string ItauDll = Environment.CurrentDirectory + "\\BancoItau.dll";
            if (File.Exists(ItauDll) == false)
            { XtraMessageBox.Show("No se encuentra archivo de configuración", "Archivo no encontrado", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmnominasbanco") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta función", "Acceso", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            Form frm = this.MdiChildren.FirstOrDefault(x => x is frmNominasBanco);

            if (frm != null)
            {
                frm.BringToFront();
                return;
            }

            //SI NO EXISTE CREAMOS EL FORM
            frm = new frmNominasBanco();
            frm.MdiParent = this;
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.Show();
        }

        private void barButtonItem12_ItemClick(object sender, ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDADA
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmcartaaviso") == false)
            { XtraMessageBox.Show("No tienes los provilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            Form frm = this.MdiChildren.FirstOrDefault(x => x is frmCartaAviso);

            if (frm != null)
            {
                frm.BringToFront();
                return;
            }

            //SI NO EXISTE CREAMOS EL FORM
            frm = new frmCartaAviso();
            frm.MdiParent = this;
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.Show();
        }

        private void barBtnContable_ItemClick(object sender, ItemClickEventArgs e)
        {
            Sesion.NuevaActividad();        

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmcomprobantecontable") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta función", "Acceso", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
            
            frmComprobanteContable comp = new frmComprobanteContable();
            comp.StartPosition = FormStartPosition.CenterParent;
            comp.ShowDialog();
        }   

        private void barbtnListadoItems_ItemClick_1(object sender, ItemClickEventArgs e)
        {
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmlistadoitems") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta función", "Acceso", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            frmListadoItems lista = new frmListadoItems();
            lista.StartPosition = FormStartPosition.CenterParent;
            lista.ShowDialog();
        }

        private void barBtnVacColectivas_ItemClick(object sender, ItemClickEventArgs e)
        {
            Sesion.NuevaActividad();

            if(objeto.ValidaAcceso(User.GetUserGroup(), "frmvacacionescolectivas") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta función", "Acceso", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            frmVacacionesColectivas colec = new frmVacacionesColectivas();
            colec.StartPosition = FormStartPosition.CenterParent;
            colec.ShowDialog();
        }

        private void barSubEmpresa_ItemClick(object sender, ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmempresa"))
            {

                Form frm = this.MdiChildren.FirstOrDefault(x => x is frmEmpresa);

                if (frm != null)
                {
                    frm.BringToFront();
                    return;
                }

                //SI NO EXISTE LA CREAMOS
                frm = new frmEmpresa();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
            else
            {
                XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void barSubInfoitems_ItemClick(object sender, ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frminfoitem"))
            {
                Form frm = this.MdiChildren.FirstOrDefault(x => x is frmDetalleItem);

                if (frm != null)
                {
                    frm.BringToFront();
                    return;
                }

                //SI NO EXISTE CREAMOS EL FORM
                frm = new frmDetalleItem();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
            else
                XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        private void barSubitems_ItemClick(object sender, ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmitem"))
            {

                //FORMULARIO EMPRESA
                Form frm = this.MdiChildren.FirstOrDefault(x => x is frmItem);

                if (frm != null)
                {
                    frm.BringToFront();
                    return;
                }

                //SI NO EXISTE CREAMOS EL FORM
                frm = new frmItem();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
            else
                XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        private void barSubIndices_ItemClick(object sender, ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmindice"))
            {

                Form frm = this.MdiChildren.FirstOrDefault(x => x is frmIndiceMensual);

                if (frm != null)
                {
                    frm.BringToFront();
                    return;
                }

                //SI NO EXISTE CREAMOS EL FORM
                frm = new frmIndiceMensual();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
            else
            {
                XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void barSubCargasTramos_ItemClick(object sender, ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDADA
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmfamtramo") == false)
            { XtraMessageBox.Show("No tienes los provilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            Form frm = this.MdiChildren.FirstOrDefault(x => x is frmAsigFam);

            if (frm != null)
            {
                frm.BringToFront();
                return;
            }

            //SI NO EXISTE CREAMOS EL FORM
            frm = new frmAsigFam();
            frm.MdiParent = this;
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.Show();
        }

        private void barSubFichasEmp_ItemClick(object sender, ItemClickEventArgs e)
        {
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmficha"))
            {
                //NO DEJAR ABRIR ESTE FORMULARIO SI NO SE HAN INGRESADO LOS DATOS DE LA EMPRESA
                //Empresa emp = new Empresa();
                //if (!Licencia.HayDatos)
                //{ XtraMessageBox.Show("Por favor ingresa los datos de tu empresa antes de continuar", "Informacion Importante", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }                

                //VER SI EL FORM YA ESTA ABIERTO Y EVITAR QUE SE VUELVA A ABRIR
                Form frm = this.MdiChildren.FirstOrDefault(x => x is frmEmpleado);

                if (frm != null)
                {
                    frm.BringToFront();
                    return;
                }

                //SI EXISTE CREAMOS LA INSTANCIA
                frm = new frmEmpleado();
                frm.MdiParent = this;
                frm.Show();
            }
            else
                XtraMessageBox.Show("No tienes los provilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        private void barSubLiquidaciones_ItemClick(object sender, ItemClickEventArgs e)
        {
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmplanillasueldos") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta función", "Acceso", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            Form frm = this.MdiChildren.FirstOrDefault(x => x is FrmPlanillaSueldos);

            if (frm != null)
            {
                frm.BringToFront();
                return;
            }

            //SI NO EXISTE CREAMOS EL FORM
            frm = new FrmPlanillaSueldos();
            frm.MdiParent = this;
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.Show();
        }

        private void barSubLiqHistoricas_ItemClick(object sender, ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmliqhistorico"))
            {
                //VE FICHAS PRIVADAS
                ShowPrivadas = User.ShowPrivadas();

                if (Calculo.GetTotalRegistros(ShowPrivadas) == 0)
                { XtraMessageBox.Show("No se encontraron registros", "Informacion importante", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                frmLiquidacionHistorica liq = new frmLiquidacionHistorica();
                liq.StartPosition = FormStartPosition.CenterScreen;
                liq.ShowDialog();
            }
            else
                XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        private void barSubCondiciones_ItemClick(object sender, ItemClickEventArgs e)
        {

            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmconjuntobusqueda"))
            {
                FrmFiltroBusqueda filtro = new FrmFiltroBusqueda();
                filtro.StartPosition = FormStartPosition.CenterParent;
                filtro.ShowDialog();
            }
            else
                XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        private void barSubCalculoLiq_ItemClick(object sender, ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD SESION
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmcalremun"))
            {
                //VE FICHAS PRIVADAS
                ShowPrivadas = User.ShowPrivadas();

                if (Calculo.GetTotalRegistros(ShowPrivadas) == 0)
                { XtraMessageBox.Show("No se encontraron registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                if (Calculo.ViewStatus())
                { XtraMessageBox.Show("Ya se está realizando un proceso de calculo, intentelo mas tarde", "Proceso activo usuario " + User.getUser(), MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                Form frm = this.MdiChildren.FirstOrDefault(x => x is frmCalculoRemuneracion);

                if (frm != null)
                {
                    frm.BringToFront();
                    return;
                }

                frmCalculoRemuneracion calc = new frmCalculoRemuneracion();
                calc.CambioEstadoOpen = this;
                calc.StartPosition = FormStartPosition.CenterParent;
                calc.ShowDialog();
                ////SI NO EXISTE CREAMOS EL FORM
                //frm = new frmCalculoRemuneracion();                
                //frm.ShowDialog();
            }
            else
                XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        private void barSubResumenProc_ItemClick(object sender, ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDAD
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmresproceso"))
            {
                //VE FICHAS PRIVADAS
                ShowPrivadas = User.ShowPrivadas();

                if (Calculo.GetTotalRegistros(ShowPrivadas) == 0)
                { XtraMessageBox.Show("No se encontraron registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                Form frm = this.MdiChildren.FirstOrDefault(x => x is frmResumenProceso);

                if (frm != null)
                {
                    frm.BringToFront();
                    return;
                }

                //SI NO EXISTE CREAMOS EL FORM
                frm = new frmResumenProceso();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
            else
                XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        private void barSubPrevired_ItemClick(object sender, ItemClickEventArgs e)
        {
            //NUEVA ACTIVIDA EN SESION
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmprevired") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta funcion", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            //VE FICHAS PRIVADAS
            ShowPrivadas = User.ShowPrivadas();

            if (Calculo.GetTotalRegistros(ShowPrivadas) == 0)
            { XtraMessageBox.Show("No se encontraron registros", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            frmPrevired prev = new frmPrevired();
            prev.StartPosition = FormStartPosition.CenterParent;
            prev.ShowDialog();
        }

        private void barSubBanco_ItemClick(object sender, ItemClickEventArgs e)
        {
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            //RUTA DLL
            //string RutaDll = Environment.CurrentDirectory + "\\Nominas.dll";
            //string BciDll = Environment.CurrentDirectory + "\\BancoBci.dll";
            string ItauDll = Environment.CurrentDirectory + "\\BancoItau.dll";
            if (File.Exists(ItauDll) == false)
            { XtraMessageBox.Show("No se encuentra archivo de configuración", "Archivo no encontrado", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmnominasbanco") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta función", "Acceso", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            Form frm = this.MdiChildren.FirstOrDefault(x => x is frmNominasBanco);

            if (frm != null)
            {
                frm.BringToFront();
                return;
            }

            //SI NO EXISTE CREAMOS EL FORM
            frm = new frmNominasBanco();
            frm.MdiParent = this;
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.Show();
        }

        private void barBtnMaestroContable_ItemClick(object sender, ItemClickEventArgs e)
        {
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmmaestrocontable") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utlizar esta función", "Acceso", MessageBoxButtons.OK, MessageBoxIcon.Stop);return; }

            Form frm = this.MdiChildren.FirstOrDefault(x => x is frmNominasBanco);

            if (frm != null)
            {
                frm.BringToFront();
                return;
            }

            frmCuentaContable cuenta = new frmCuentaContable();
            cuenta.StartPosition = FormStartPosition.CenterParent;
            cuenta.MdiParent = this;
            cuenta.Show();
        }

        private void barButtonItem18_ItemClick(object sender, ItemClickEventArgs e)
        {
            Sesion.NuevaActividad();

            //string FiltroSql = "", Path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Query.xlsx";
            //FiltroSql = Calculo.GetSqlFiltro(User.GetUserFilter(), "", User.ShowPrivadas(), true);

            //BuilderSql b = new BuilderSql();
            //b.ShowSqlBuilder(fnSistema.pgServer, fnSistema.pgDatabase, fnSistema.pgUser, fnSistema.pgPass, FiltroSql);

            //if (b.CustomSql.Length > 0)
            //{
            //    if (b.GetExcel(Path))
            //    {
            //        DialogResult d = XtraMessageBox.Show($"Archivo {Path} generado correctamete.¿Deseas revisar archivo?", "Informacion", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            //        if (d == DialogResult.Yes)
            //        {
            //            System.Diagnostics.Process.Start(Path);
            //        }
            //        else
            //        {
            //            XtraMessageBox.Show("No se pudo generar documento", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            //        }
            //    }
            //    else
            //    {
            //        XtraMessageBox.Show("No se pudo generar documento", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            //    }
            //}
            //else
            //{
            //    XtraMessageBox.Show("No se pudo generar información", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            //}
        }

        private void batbtnSesiones_ItemClick(object sender, ItemClickEventArgs e)
        {
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmsesiones") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utlizar esta función", "Acceso", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            Form frm = this.MdiChildren.FirstOrDefault(x => x is frmSesion);

            if (frm != null)
            {
                frm.BringToFront();
                return;
            }

            frmSesion cuenta = new frmSesion();
            cuenta.StartPosition = FormStartPosition.CenterParent;
            cuenta.MdiParent = this;
            cuenta.Show();
        }

        private void barbtnRespaldoBase_ItemClick(object sender, ItemClickEventArgs e)
        {
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmrespaldo") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta función", "Acceso", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            frmRespaldo resp = new frmRespaldo();
            resp.StartPosition = FormStartPosition.CenterParent;
            resp.ShowDialog();
        }

        private void barButtonItem20_ItemClick(object sender, ItemClickEventArgs e)
        {
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            // XtraMessageBox.Show("Módulo en construcción", "información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmquerybuilder") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta función", "Acceso", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }          

            frmGenerador gen = new frmGenerador();
            gen.StartPosition = FormStartPosition.CenterParent;
            gen.ShowDialog();
        }

        private void barbtnReporteContable_ItemClick(object sender, ItemClickEventArgs e)
        {
            Sesion.NuevaActividad();

            // XtraMessageBox.Show("Módulo en construcción", "información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //if (objeto.ValidaAcceso(User.GetUserGroup(), "frmquerybuilder") == false)
            //{ XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta función", "Acceso", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            frmConfRepoContable gen = new frmConfRepoContable();
            gen.StartPosition = FormStartPosition.CenterParent;
            gen.ShowDialog();
        }

        private void barBtnMontoAnual_ItemClick(object sender, ItemClickEventArgs e)
        {
            Sesion.NuevaActividad();

            frmMontoAnual mo = new frmMontoAnual();
            mo.MdiParent = this;
            mo.StartPosition = FormStartPosition.CenterParent;
            mo.Show();
        }

        private void barbtnfiniquitomasivo_ItemClick(object sender, ItemClickEventArgs e)
        {
            Sesion.NuevaActividad();

            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmfiniquitomasivo") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta función", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            frmFiniquitosMasivos mas = new frmFiniquitosMasivos();
            mas.StartPosition = FormStartPosition.CenterParent;
            mas.ShowDialog();
        }

        private void barbtnCertificadoRentas_ItemClick(object sender, ItemClickEventArgs e)
        {
            Sesion.NuevaActividad();

            frmCertificadoRentas cert = new frmCertificadoRentas();
            cert.StartPosition = FormStartPosition.CenterParent;
            cert.ShowDialog();

        }

        private void barButtonItem22_ItemClick(object sender, ItemClickEventArgs e)
        {
            Sesion.NuevaActividad();

            frmDeclaracionJurada dec = new frmDeclaracionJurada();
            dec.StartPosition = FormStartPosition.CenterParent;
            dec.ShowDialog();
        }

        private void barbtnMovimientos_ItemClick(object sender, ItemClickEventArgs e)
        {
            FrmMovimientos mov = new FrmMovimientos();
            mov.StartPosition = FormStartPosition.CenterScreen;
            mov.ShowDialog();
        }

        private void barResumenVac_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (objeto.ValidaAcceso(User.GetUserGroup(), "frmvacacionest") == false)
            { XtraMessageBox.Show("No tienes los privilegios necesarios para utilizar esta función", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }


            FrmVacacionesEst est = new FrmVacacionesEst();
            est.StartPosition = FormStartPosition.CenterScreen;
            est.ShowDialog();
        }

        private void barItemCargaMasivaItem_ItemClick(object sender, ItemClickEventArgs e)
        {
            frmCargaMontoAfc ext = new frmCargaMontoAfc();
            ext.StartPosition = FormStartPosition.CenterScreen;
            ext.ShowDialog();
        }

        private void barbtnItemExtendido_ItemClick(object sender, ItemClickEventArgs e)
        {
            frmCargaItemExtendida fext = new frmCargaItemExtendida();
            fext.StartPosition = FormStartPosition.CenterScreen;
            fext.ShowDialog();
        }
    }
}
