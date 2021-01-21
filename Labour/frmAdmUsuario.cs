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

namespace Labour
{
    public partial class frmAdmUsuario : DevExpress.XtraEditors.XtraForm
    {
        public frmAdmUsuario()
        {
            InitializeComponent();
        }

        private void frmAdmUsuario_Load(object sender, EventArgs e)
        {
            //CARGAR LISTADO DE USUARIOS
            string sql = "SELECT nombre, usuario, descripcion FROM usuario INNER JOIN grupo " +
                "ON usuario.grupo = grupo.id";
            fnSistema.spllenaGridView(gridUsuario, sql);
            fnSistema.spOpcionesGrilla(viewUsuario);

            fnColumnas();
            
        }

        #region "MANEJO DE DATOS"
        //COLUMNAS GRILLA
        private void fnColumnas()
        {
            viewUsuario.Columns[0].Caption = "Nombre";
            viewUsuario.Columns[0].Width = 120;

            viewUsuario.Columns[1].Caption = "Usuario";

            viewUsuario.Columns[2].Caption = "Grupo";

            //viewUsuario.Columns[2].Caption = "Password";
        }

        //GENERACION DE NUMERO ALEATORIO
        private void NumberRandom()
        {
            int numero = 0;


        }
        #endregion
    }
}