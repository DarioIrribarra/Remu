using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.XtraEditors;
using DevExpress.Skins;
using System.Windows.Forms;

namespace Labour
{
    public partial class frmDatos : DevExpress.XtraEditors.XtraForm
    {

      
        public frmDatos()
        {
            InitializeComponent();
        }

        private void frmDatos_Load(object sender, EventArgs e)
        {

            SkinManager.EnableFormSkins() ;
            SkinManager.EnableMdiFormSkins();
            //CANTIDAD MAXIMA DE CARACTERES 20
            txtBaseDatos.Properties.MaxLength = 50;
            txtNombre.Properties.MaxLength = 50;
            txtPassword.Properties.MaxLength = 50;
            txtServidor.Properties.MaxLength = 200;
            txtUsuario.Properties.MaxLength = 50;
            
        }  

        private void fnLimpiar()
        {
            txtServidor.Focus();
            txtBaseDatos.Text = "";
            txtNombre.Text = "";
            txtPassword.Text = "";
            txtServidor.Text = "";
            txtUsuario.Text = "";
        }

        private void btnLimpiar_Click_1(object sender, EventArgs e)
        {
            fnLimpiar();
        }

        private void btnGuardar_Click_1(object sender, EventArgs e)
        {
            //Guardar en BD el nuevo registro
            if (txtBaseDatos.Text != "" && txtNombre.Text != "" && txtPassword.Text != "" && txtServidor.Text != "" && txtUsuario.Text != "")
            {
                //ANTES DE GUARDAR EL DATO PROBAMOS LA CONEXION CON ESOS DATOS
                string bd = txtBaseDatos.Text;
                string name = txtNombre.Text;
                string pass = txtPassword.Text;
                string servidor = txtServidor.Text;
                string user = txtUsuario.Text;

                fnSistema.pgDatabase = bd;
                fnSistema.pgPass = pass;
                fnSistema.pgServer = servidor;
                fnSistema.pgUser = user;

                //SI LA CONEXION ES CORRECTA GUARDAMOS LA CONEXION
                //MOSTRAR VENTANA DE PROCESANDO
                IGrilla spl = this.Owner as IGrilla;
                if (spl!=null)
                {
                    spl.CargarWait();
                }

                if (fnSistema.ConectarSQLServer())
                {
                    //CERRAR VENTANA PROCESANDO
                    if (spl!=null)
                    {
                        spl.CerrarWait();
                    }
                    XtraMessageBox.Show("Conexion Exitosa", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //LLAMAMOS AL METODO PARA INGRESAR
                    SqlLite.fnIngresoRegistros(txtServidor, txtBaseDatos, txtUsuario, txtPassword, txtNombre);
                    //recargamos combobox
                    IGrilla gr = this.Owner as IGrilla;
                    if (gr != null)
                    {
                        gr.CargarGrid();
                    }
                    //limpiar campos
                    fnLimpiar();

                    this.Close();

                }
                else {
                    //CERRAR VENTANA PROCESANDO
                    if (spl!=null)
                    {
                        spl.CerrarWait();
                    }
                    XtraMessageBox.Show("NO se pudo Realizar la conexion, Compruebe los parametros y vuelva a intentar", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtServidor.Focus();
                }
                
            }//IF EXTERIOR
            else
            {
                XtraMessageBox.Show("Debes llenar los campos en blanco", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtServidor.Focus();

            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            //NUEVA SESION
            //Sesion.NuevaActividad();

            Close();
        }
    }
}
