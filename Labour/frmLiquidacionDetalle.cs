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
using DevExpress.XtraGrid.Columns;

namespace Labour
{
    public partial class frmLiquidacionDetalle : DevExpress.XtraEditors.XtraForm
    {
        public string Contrato { get; set; } = "";
        public int Periodo { get; set; } = 0;
        public frmLiquidacionDetalle()
        {
            InitializeComponent();
        }

        public frmLiquidacionDetalle(string contrato, int periodo)
        {
            InitializeComponent();
            this.Contrato = contrato;
            this.Periodo = periodo;
        }
        private void frmLiquidacionDetalle_Load(object sender, EventArgs e)
        {
            if (Contrato != "" && Periodo != 0)
            {
                CargarTabla(Periodo, Contrato);
               // fnSistema.spOpcionesGrilla(gridView1);
            }
        }

        #region "MANEJO DE DATOS"
        private void CargarTabla(int pPeriodo, string pContrato)
        {
            #region "SQL"
            string sql = "DECLARE @Simples INT " +
                         "DECLARE @Invalidas INT " +
                         "DECLARE @Maternal INT " +
                         "DECLARE @Isapre INT " +                  
                         "SET @Simples = (SELECT count(*) FROM CargaFamiliar WHERE contrato = @pContrato AND(invalido = 0 AND maternal = 0)) " +
                         "SET @Invalidas = (SELECT count(*) FROM CargaFamiliar WHERE contrato = @pContrato AND invalido = 1) " +
                         "SET @Maternal = (SELECT count(*) FROM CargaFamiliar WHERE contrato = @pContrato AND maternal = 1) " +
                         "SET @Isapre = (SELECT salud FROM trabajador WHERE contrato = @pContrato AND anomes = @pPeriodo) " +
                             "SELECT CONCAT(nombre, ' ', apepaterno, ' ', apematerno) as item, sysdiastr as 'Dias Tr' , sysdiaslic as 'Dias Lic', 0 as 'N° cuota', 0 as 'Cuotas',  " +
                             "0 as 'N° Carga', 0 as 'val tramo',  0 as '% Afp', 0 as 'Uf', 0 as 'Factor % Impto', 0 as 'Reb. Impto', 0 as 'C. Inv.', 0 as 'Fdo Sold.', 0 as '% caja', 0 as 'Total', 0 as tipo " +
                             "FROM itemtrabajador " +
                            "INNER JOIN trabajador ON trabajador.contrato = itemtrabajador.contrato AND itemtrabajador.anomes = trabajador.anomes " +
                            "INNER JOIN calculoMensual ON calculoMensual.contrato = itemTrabajador.contrato AND itemTrabajador.anomes = calculomensual.anomes " +
                            "WHERE itemTrabajador.anomes = @pPeriodo AND itemtrabajador.contrato = @pContrato " +
                        "UNION " +
                            "SELECT descripcion , 0, 0, 0 , 0 , 0, 0 , 0 , 0 , 0 ,  " +
                            "0 , 0 , 0, 0, valorcalculado , itemTrabajador.tipo " +
                            "FROM itemtrabajador " +
                            "INNER JOIN item ON item.coditem = itemTrabajador.coditem " +
                            "WHERE anomes = @pPeriodo AND contrato = @pContrato " +
                            "AND(itemtrabajador.tipo = 1 OR itemTrabajador.tipo = 2) " +
                        "UNION " +
                            "SELECT descripcion,0, 0, 0, 0, 0, 0, sysporcadmafp + 10, 0, 0, 0, 0, 0, 0, valor, itemTrabajador.tipo FROM itemtrabajador " +
                            "INNER JOIN calculoMensual ON calculoMensual.contrato = itemTrabajador.contrato AND calculoMensual.anomes = itemtrabajador.anomes " +
                            "INNER JOIN item ON item.coditem = itemtrabajador.coditem " +
                            "WHERE itemTrabajador.anomes = @pPeriodo AND itemTrabajador.contrato = @pContrato AND itemTrabajador.coditem = 'PREVISI' " +
                        "UNION " +
                            "SELECT isapre.nombre, 0, 0, 0, 0, 0,0,  0, valor, 0, 0, 0, 0, 0, valor, itemTrabajador.tipo FROM itemtrabajador " +
                            "INNER JOIN trabajador ON trabajador.contrato = itemtrabajador.contrato AND trabajador.anomes = itemtrabajador.anomes " +
                            "INNER JOIN isapre ON isapre.id = trabajador.salud " +
                            "WHERE itemTrabajador.anomes = @pPeriodo AND itemTrabajador.contrato = @pContrato AND itemTrabajador.coditem = 'SALUD' AND @Isapre<> 1 " +
                        "UNION " +
                            "SELECT isapre.nombre, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, valor, itemTrabajador.tipo FROM itemtrabajador " +
                            "INNER JOIN trabajador ON trabajador.contrato = itemtrabajador.contrato AND trabajador.anomes = itemtrabajador.anomes " +
                            "INNER JOIN isapre ON isapre.id = trabajador.salud " +
                            "WHERE itemTrabajador.anomes = @pPeriodo AND itemTrabajador.contrato = @pContrato AND itemTrabajador.coditem = 'SALUD' AND @Isapre = 1 " +
                        "UNION " +
                            "SELECT descripcion,0, 0, 0, 0, 0, 0, 0, 0, sysfactorimpto * 100, sysrebimpto, 0, 0, 0, valor, itemtrabajador.tipo " +
                            "FROM itemtrabajador " +
                            "INNER JOIN calculomensual ON calculoMensual.contrato = itemtrabajador.contrato AND itemtrabajador.anomes = calculoMensual.anomes " +
                            "INNER JOIN item ON item.coditem = itemTrabajador.coditem " +
                            "INNER JOIN trabajador ON trabajador.contrato = itemtrabajador.contrato AND trabajador.anomes = itemTrabajador.anomes " +
                            "WHERE itemTrabajador.anomes = @pPeriodo AND itemTrabajador.contrato = @pContrato AND itemTrabajador.coditem = 'IMPUEST' " +
                        "UNION " +
                            "SELECT descripcion, 0, 0, LEFT(cuota, CHARINDEX('/', cuota) - 1),RIGHT(cuota, LEN(cuota) - CHARINDEX('/', cuota)), 0, 0, 0, 0, 0, 0, 0, 0, 0, valor, itemTrabajador.tipo " +
                            "FROM itemtrabajador " +
                            "INNER JOIN item ON item.coditem = itemTrabajador.coditem " +
                            "WHERE anomes = @pPeriodo AND itemTrabajador.coditem = 'PRESTAM' and contrato = @pContrato " +
                        "UNION " +
                            "SELECT descripcion, 0, 0, 0, 0, 0,0, 0, 0, 0, 0, 0, 0, 0, valor, itemTrabajador.tipo " +
                            "FROM itemtrabajador " +
                            "INNER JOIN item ON item.coditem = itemTrabajador.coditem " +
                            "WHERE anomes = @pPeriodo AND contrato = @pContrato AND itemtrabajador.tipo = 4 AND " +
                            "(itemTrabajador.coditem <> 'PREVISI' AND itemTrabajador.coditem <> 'SCEMPRE' AND itemTrabajador.coditem <> 'SCEMPLE' AND itemtrabajador.coditem <> 'SALUD' AND itemtrabajador.coditem <> 'IMPUEST') " +
                        "UNION " +
                            "SELECT descripcion, 0, 0, 0, 0, @Simples, asignacionFamiliar.valor,0, 0, 0, 0, 0, 0, 0, valorcalculado, itemtrabajador.tipo " +
                            "FROM itemtrabajador " +
                            "INNER JOIN trabajador ON trabajador.contrato = itemTrabajador.contrato AND trabajador.anomes = itemtrabajador.anomes " +
                            "INNER JOIN item ON item.coditem = itemTrabajador.coditem " +
                            "INNER JOIN asignacionFamiliar ON asignacionFamiliar.tramo = trabajador.tramo " +
                            "WHERE itemTrabajador.anomes = @pPeriodo AND itemTrabajador.contrato = @pContrato AND itemTrabajador.coditem = 'ASIGFAM' " +
                        "UNION " +
                            "SELECT descripcion, 0, 0, 0, 0, @Invalidas, asignacionFamiliar.valor, 0, 0, 0, 0, 0, 0, 0, valorcalculado, itemtrabajador.tipo " +
                            "FROM itemtrabajador " +
                            "INNER JOIN trabajador ON trabajador.contrato = itemTrabajador.contrato AND trabajador.anomes = itemtrabajador.anomes " +
                            "INNER JOIN item ON item.coditem = itemTrabajador.coditem " +
                            "INNER JOIN asignacionFamiliar ON asignacionFamiliar.tramo = trabajador.tramo " +
                            "WHERE itemTrabajador.anomes = @pPeriodo AND itemTrabajador.contrato = @pContrato AND itemTrabajador.coditem = 'ASIGINV' " +
                        "UNION " +
                            "SELECT descripcion, 0, 0, 0, 0, @Maternal, asignacionFamiliar.valor,  0, 0, 0, 0, 0, 0, 0, valorcalculado, itemtrabajador.tipo " +
                            "FROM itemtrabajador " +
                            "INNER JOIN trabajador ON trabajador.contrato = itemTrabajador.contrato AND trabajador.anomes = itemtrabajador.anomes " +
                            "INNER JOIN item ON item.coditem = itemTrabajador.coditem " +
                            "INNER JOIN asignacionFamiliar ON asignacionFamiliar.tramo = trabajador.tramo " +
                            "WHERE itemTrabajador.anomes = @pPeriodo AND itemTrabajador.contrato = @pContrato AND itemTrabajador.coditem = 'ASIGMAT' " +
                        "UNION " +
                            "SELECT descripcion, 0, 0, 0, 0, @Simples, asignacionFamiliar.valor,  0, 0, 0, 0, 0, 0, 0, valorcalculado, itemtrabajador.tipo " +
                            "FROM itemtrabajador " +
                            "INNER JOIN trabajador ON trabajador.contrato = itemTrabajador.contrato AND trabajador.anomes = itemtrabajador.anomes " +
                            "INNER JOIN item ON item.coditem = itemTrabajador.coditem " +
                            "INNER JOIN asignacionFamiliar ON asignacionFamiliar.tramo = trabajador.tramo " +
                            "WHERE itemTrabajador.anomes = @pPeriodo AND itemTrabajador.contrato = @pContrato AND itemTrabajador.coditem = 'ASIFAR' " +
                        "UNION " +
                            "SELECT descripcion, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, valor, itemTrabajador.tipo " +
                            "FROM itemtrabajador " +
                            "INNER JOIN item ON item.coditem = itemTrabajador.coditem " +
                            "WHERE anomes = @pPeriodo AND contrato = @pContrato AND itemtrabajador.tipo = 5 AND(itemTrabajador.coditem <> 'PRESTAM') " +
                            "UNION " +
                        "SELECT descripcion, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, sysfscese, syscicese, 0, valor, itemtrabajador.tipo " +
                            "FROM itemtrabajador " +
                            "INNER JOIN calculomensual ON calculoMensual.contrato = itemtrabajador.contrato AND itemtrabajador.anomes = calculoMensual.anomes " +
                            "INNER JOIN item ON item.coditem = itemTrabajador.coditem " +
                            "INNER JOIN trabajador ON trabajador.contrato = itemtrabajador.contrato AND trabajador.anomes = itemTrabajador.anomes " +
                            "WHERE itemTrabajador.anomes = @pPeriodo AND itemTrabajador.contrato = @pContrato AND itemTrabajador.coditem = 'SCEMPRE' " +
                        "UNION " +
                            "SELECT descripcion, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, sysfscest, syscicest, 0, valor, itemtrabajador.tipo " +
                            "FROM itemtrabajador " +
                            "INNER JOIN calculomensual ON calculoMensual.contrato = itemtrabajador.contrato AND itemtrabajador.anomes = calculoMensual.anomes " +
                            "INNER JOIN item ON item.coditem = itemTrabajador.coditem " +
                            "INNER JOIN trabajador ON trabajador.contrato = itemtrabajador.contrato AND trabajador.anomes = itemTrabajador.anomes " +
                            "WHERE itemTrabajador.anomes = @pPeriodo AND itemTrabajador.contrato = @pContrato AND itemTrabajador.coditem = 'SCEMPLE' " +
                        "UNION " +
                        "SELECT descripcion, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, valor, itemtrabajador.tipo " +
                        "FROM itemtrabajador " +
                        "INNER JOIN item ON item.coditem = itemTrabajador.coditem " +
                        "WHERE anomes = @pPeriodo AND itemTrabajador.contrato = @pContrato AND itemTrabajador.tipo = 6 " +
                        "order by tipo";
            #endregion

            SqlCommand cmd;
            SqlDataAdapter ad = new SqlDataAdapter();
            DataSet ds = new DataSet();
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pPeriodo", pPeriodo));
                        cmd.Parameters.Add(new SqlParameter("@pContrato", pContrato));

                        ad.SelectCommand = cmd;
                        ad.Fill(ds);

                        if (ds.Tables[0].Rows.Count>0)
                        {
                            //LLENAMOS GRID
                            gridControl1.DataSource = ds.Tables[0];

                            gridView1.OptionsCustomization.AllowFilter = false;
                            gridView1.OptionsCustomization.AllowSort = false;
                            gridView1.OptionsMenu.EnableFooterMenu = false;
                            gridView1.OptionsMenu.EnableGroupPanelMenu = false;
                            gridView1.OptionsMenu.EnableColumnMenu = false;
                            gridView1.OptionsCustomization.AllowColumnMoving = false;

                            GridColumnCollection columnas = gridView1.Columns;
                            foreach (GridColumn item in columnas)
                            {
                                //gridView1.Columns[item.FieldName].Width = gridView1.Columns[item.FieldName].GetBestWidth();                                
                                if (item.FieldName == "item")
                                    gridView1.Columns[item.FieldName].Width = 120;

                                if (item.FieldName != "item")
                                {
                                    gridView1.Columns[item.FieldName].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                                    gridView1.Columns[item.FieldName].DisplayFormat.FormatString = "n2";
                                }                                       

                                if (item.FieldName == "tipo")
                                    gridView1.Columns[item.FieldName].Visible = false;

                                if (item.FieldName == "% caja")
                                    gridView1.Columns[item.FieldName].Visible = false;

                            }                            
                        }

                        cmd.Dispose();
                        ad.Dispose();
                        fnSistema.sqlConn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }
        #endregion

        private void gridView1_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "N° cuota" || e.Column.FieldName == "Cuotas" || e.Column.FieldName == "N° Carga" || e.Column.FieldName == "% Afp"
                || e.Column.FieldName == "Uf" || e.Column.FieldName == "Factor % Impto" || e.Column.FieldName == "Reb. Impto" || e.Column.FieldName == "C. Inv."
                || e.Column.FieldName == "Fdo Sold." || e.Column.FieldName == "Total" || e.Column.FieldName == "val tramo" || e.Column.FieldName == "Dias Lic" 
                || e.Column.FieldName == "Dias Tr")
            {
                if (Convert.ToDouble(e.Value) == 0)
                {
                    e.DisplayText = "";
                }
                else
                {
                    e.Column.AppearanceCell.ForeColor = Color.Green;
                }
            }
        }
    }
}