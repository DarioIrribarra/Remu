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
using DevExpress.Utils.Menu;
using DevExpress.Images;

namespace Labour
{
    public partial class frmSesion : DevExpress.XtraEditors.XtraForm
    {
        Cifrado cif = new Cifrado();
        private string BaseDatosCifrada = "";

        public frmSesion()
        {
            InitializeComponent();
        }

        private void ColumnasGrilla()
        {
            if (viewSesiones.RowCount > 0)
            {
                viewSesiones.Columns[0].Caption = "Usuario";
                viewSesiones.Columns[0].DisplayFormat.FormatString = "usuario";
                viewSesiones.Columns[0].DisplayFormat.Format = new FormatCustom();

                viewSesiones.Columns[1].Caption = "Registrado";
                viewSesiones.Columns[2].Caption = "Equipo";
                viewSesiones.Columns[2].Width = 100;
                viewSesiones.Columns[2].DisplayFormat.FormatString = "equipo";
                viewSesiones.Columns[2].DisplayFormat.Format = new FormatCustom();

                viewSesiones.Columns[3].Caption = "Ip";
                viewSesiones.Columns[3].DisplayFormat.FormatString = "ip";
                viewSesiones.Columns[3].DisplayFormat.Format = new FormatCustom();

                viewSesiones.Columns[4].Caption = "Juego Datos";
                viewSesiones.Columns[4].Width = 60;
                viewSesiones.Columns[4].DisplayFormat.FormatString = "db";
                viewSesiones.Columns[4].DisplayFormat.Format = new FormatCustom();

                viewSesiones.Columns[5].Caption = "HostName";
                viewSesiones.Columns[5].DisplayFormat.FormatString = "hostname";
                viewSesiones.Columns[5].DisplayFormat.Format = new FormatCustom();

                viewSesiones.Columns[6].Caption = "Ultima Actividad";
            }
        }


        private void frmSesion_Load(object sender, EventArgs e)
        {
            BaseDatosCifrada = cif.EncriptaTripleDesc(fnSistema.pgDatabase);

            Sesion.ShowSesion(gridSesiones, BaseDatosCifrada);
            fnSistema.spOpcionesGrilla(viewSesiones);
            ColumnasGrilla();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            BaseDatosCifrada = cif.EncriptaTripleDesc(fnSistema.pgDatabase);

            Sesion.ShowSesion(gridSesiones, BaseDatosCifrada);
            fnSistema.spOpcionesGrilla(viewSesiones);
            ColumnasGrilla();
        }

        private void viewSesiones_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            DXPopupMenu menu = e.Menu;

            if (menu != null)
            {
                DXMenuItem CerrarSesion = new DXMenuItem("Cerrar Sesion", new EventHandler(CerrarSesion_Click));
                CerrarSesion.ImageOptions.Image = ImageResourceCache.Default.GetImage("images/edit/delete_16x16.png");

                menu.Items.Clear();
                menu.Items.Add(CerrarSesion);
            }
        }

        /// <summary>
        /// Permite cerrar una sesion de usuario.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CerrarSesion_Click(object sender, EventArgs e)
        {
            if (viewSesiones.RowCount > 0)
            {
                string usuario = viewSesiones.GetFocusedDataRow()["usr"].ToString();
                usuario = cif.DesencriptaTripleDesc(usuario);
                string db = viewSesiones.GetFocusedDataRow()["bd"].ToString();

                if (usuario.Length > 0 && db.Length > 0)
                {
                    if (usuario.Equals(User.getUser()))
                    { XtraMessageBox.Show("No puedes cerrar tu propia sesión", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                    DialogResult advertencia = XtraMessageBox.Show("¿Realmente deseas cerrar la sesion del usuario " + usuario + "?", "Sesion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (advertencia == DialogResult.Yes)
                    {
                        //CERRAR...
                        if (Sesion.DeleteAccess(usuario))
                        {
                            XtraMessageBox.Show("Sesion cerrada correctamente", "Sesion", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            BaseDatosCifrada = cif.EncriptaTripleDesc(fnSistema.pgDatabase);

                            gridSesiones.DataSource = null;
                            Sesion.ShowSesion(gridSesiones, BaseDatosCifrada);
                            fnSistema.spOpcionesGrilla(viewSesiones);
                            return;
                        }
                        else
                        {
                            XtraMessageBox.Show("No se pudo cerrar la sesion", "Sesion", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        }
                    }
                }
                else
                {
                    XtraMessageBox.Show("No se pudo cerrar sesion", "Sesion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}