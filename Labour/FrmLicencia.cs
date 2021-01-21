using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Data.SqlClient;

namespace Labour
{
    public partial class FrmLicencia : XtraForm 
    {
        public FrmLicencia()
        {
            InitializeComponent();
        }

        private void FrmLicencia_Load(object sender, EventArgs e)
        {
            Cifrado cif = new Cifrado();
            string codigo = Licencia.GetLicencia();
            DateTime Expira = DateTime.Now.Date;
            Expira = Licencia.GetExpiration();            
            txtCodigo.Text = codigo;
            Hashtable dataSesion = Sesion.GetInformationUserSession(User.getUser(), fnSistema.pgDatabase);

            lblUsers.Text = "Licencia válida para " + cif.GetUserCount(codigo) + " dispositivos en simultaneo.";
            lblSesiones.Text = "Sesiones activas : " + (Sesion.SesionesAbiertas(fnSistema.pgDatabase) == 0?"No hay sesiones Abiertas en este momento.":(Sesion.SesionesAbiertas(fnSistema.pgDatabase)).ToString());
            lblFooter.Text = "Sopytec S.A Todos los derechos reservados " + DateTime.Now.Date.ToString("yyyy");
            lblDireccion.Text = "Direccion Ip: " + User.GetIpUser();
            lblServerinfo.Text = fnSistema.GetInfoServer();
            lblExpira.Text = Expira.ToShortDateString();
            TimeSpan d = Expira.Subtract(DateTime.Now);
            if (d.Days <= 30)
            {
                lblDiasExp.ForeColor = Color.Red;
                lblDiasExp.Text = $"{d.Days} días (Tu licencia está próxima a vencer)";
            }
            else if (d.Days >30 && d.Days <90)
            {
                lblDiasExp.ForeColor = Color.Orange;
                lblDiasExp.Text = $"{d.Days} días";
            }
            else
            {
                lblDiasExp.ForeColor = Color.Green;
                lblDiasExp.Text = $"{d.Days} días";
            }        
         
        }

        private void memoEdit1_Properties_BeforeShowMenu(object sender, DevExpress.XtraEditors.Controls.BeforeShowMenuEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void txtCodigo_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyData == (Keys.Control | Keys.C)) || (e.KeyData == (Keys.Control | Keys.V)))
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            //NUEVA ACTIVIDAD DE SESION
            Sesion.NuevaActividad();

            Close();
        }

        private void pictureEdit1_Properties_PopupMenuShowing(object sender, DevExpress.XtraEditors.Events.PopupMenuShowingEventArgs e)
        {
            e.PopupMenu.Items.Clear();
        }
    }
}
