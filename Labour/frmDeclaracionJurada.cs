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
using System.IO;
using System.Collections;

namespace Labour
{
    public partial class frmDeclaracionJurada : DevExpress.XtraEditors.XtraForm
    {
        Empresa emp = new Empresa();

        /// <summary>
        /// Columnas validas para archivo de entrada formulario 1879
        /// </summary>
        List<string> ColumnasArchivo = new List<string>() { "rut", "nombre", "impuesto", "renta35", "renta10", "periodo" };

        public frmDeclaracionJurada()
        {
            InitializeComponent();
        }

        private void frmDeclaracionJurada_Load(object sender, EventArgs e)
        {
            LlenaComboBox(txtPeriodo, GetListYears());
            LlenaComboBox(txtTipoDeclaración, GetDataSourceTipo());
            LlenaComboBox(txtTipoFormulario, GetDataSourceFormulario());
            emp.SetInfo();            
            txtFolioBase.Text = "51" + fnSistema.GetDigRutFolio(emp.Rut);
            txtFolioAntBase.Text = "51" + fnSistema.GetDigRutFolio(emp.Rut);
        }


        #region "DATA"
        private void LlenaComboBox(LookUpEdit pCombo, List<formula> pDataSource)
        {
            if (pDataSource != null)
            {
                if (pDataSource.Count > 0)
                {
                    //PROPIEDADES COMBOBOX
                    pCombo.Properties.DataSource = pDataSource.ToList();
                    pCombo.Properties.ValueMember = "key";
                    pCombo.Properties.DisplayMember = "desc";

                    pCombo.Properties.PopulateColumns();

                    //ocultamos la columan key
                    pCombo.Properties.Columns[0].Visible = false;

                    pCombo.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
                    pCombo.Properties.AutoSearchColumnIndex = 1;
                    pCombo.Properties.ShowHeader = false;

                    if (pCombo.Properties.DataSource != null)
                        pCombo.ItemIndex = 0;
                }
            }
        }

        private List<formula> GetDataSourceTipo()
        {
            List<formula> Data = new List<formula>();

            Data.Add(new formula() { key = "O", desc = "Original"});
            Data.Add(new formula() { key = "R", desc = "Rectificatoria" });
            Data.Add(new formula() { key = "A", desc = "Anula" });

            return Data;
        }
        private List<formula> GetDataSourceFormulario()
        {
            List<formula> Data = new List<formula>();

            Data.Add(new formula() { key = "1887", desc = "Formulario 1887" });
            Data.Add(new formula() { key = "1879", desc = "Formulario 1879" });

            return Data;
        }

        /// <summary>
        /// Retorna todos los años disponibles para consultar.
        /// </summary>
        /// <returns></returns>
        private List<formula> GetListYears()
        {
            string sql = "SELECT DISTINCT CAST(CAST(SUBSTRING(CAST(anomes AS VARCHAR(6)), 1, 4) as integer) + 1 as varchar(6)) as y from parametro " +
                         "ORDER BY y DESC";
            SqlCommand cmd;
            SqlConnection cn;
            SqlDataReader rd;

            List<formula> listado = new List<formula>();

            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            rd = cmd.ExecuteReader();
                            if (rd.HasRows)
                            {
                                while (rd.Read())
                                {
                                    listado.Add(new formula() { key = (string)rd["y"], desc = (string)rd["y"] });
                                }
                            }

                            cmd.Dispose();
                            rd.Close();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                //ERROR
            }

            return listado;
        }

        /// <summary>
        /// Genera datasource para reporte año tributario
        /// </summary>
        /// <param name="pPeriodo"></param>
        private DataTable GetDataSourceReport(int pPeriodo)
        {
            SqlCommand cmd;
            SqlConnection cn;
            SqlDataAdapter ad = new SqlDataAdapter();
            DataSet ds = new DataSet();
            DataTable tb = new DataTable();
            string Inicio = "", Termino = "";
            Inicio = pPeriodo.ToString() + "01";
            Termino = pPeriodo.ToString() + "12";

            #region "SQL QUERY"
            string sql = "SELECT rut, SUM(Imponible) as Imponible, SUM(RentaTotalNetaSinAct) as 'RentaTotalNetaSinAct', SUM(ImpuestoUnicoSinAct) as 'ImpuestoUnicoSinAct',  " +
                     "SUM(MayorRetencion) as 'MayorRetencion', SUM(RentaNoGravadaSinAct) as 'RentaNoGravadaSinAct', SUM(RentaTotalExentaSinAct) as 'RentaTotalExentaSinAct', " +
                     "SUM(RebajasZonaExtremaSinAct) as 'RebajasZonaExtremaSinAct', SUM(RentaAfectaIMptoReajustada) as 'RentaAfectaIMptoReajustada', " +
                     "SUM(ImpoReajustado) as 'ImpoReajustado', SUM(MayorRetdelimptoAct) as 'MayorRetdelimptoAct', SUM(RentaTotalNoGravadaAct) as 'RentaTotalNoGravadaAct', " +
                     "SUM(RentaTotalExentaAct) as 'RentaTotalExentaAct', SUM(RebajasExtremaAct) as 'RebajasExtremaAct'," +
                     " IIF(SUM(enero) =1, 'C', '') as Enero, IIF(SUM(febrero) =1, 'C', '') as Febrero, " +
                     "IIF(SUM(Marzo) =1, 'C', '') as 'Marzo', IIF(SUM(Abril) =1, 'C', '') as 'Abril', IIF(SUM(Mayo) =1, 'C', '') as 'Mayo', " +
                     "IIF(Sum(Junio) =1, 'C', '') as 'Junio', IIF(SUM(julio) =1, 'C', '') as 'Julio', IIF(SUM(agosto) =1, 'C', '') as 'Agosto', " +
                     "IIF(SUM(Septiembre) =1, 'C', '') as 'Septiembre', IIF(SUM(octubre) =1, 'C', '') as 'Octubre', " +
                     "IIF(SUM(noviembre) =1, 'C', '') as 'Noviembre', IIF(SUM(diciembre) =1, 'C', '') as 'Diciembre', " +
                     "row_number() OVER(ORDER BY rut) as 'NumCert' " +
                     "FROM " +
                     "(SELECT dbo.fnformatearut(trabajador.rut) as rut, " +
                     "syspago as 'RentaTotalNetaSinAct', " +
                     "systimp as 'Imponible', " +
                     "(SELECT SUM(valorcalculado) FROM itemtrabajador WHERE coditem = 'IMPUEST' and suspendido = 0 AND rut = trabajador.rut AND itemtrabajador.anomes = trabajador.anomes) as 'ImpuestoUnicoSinAct', " +
                     "0 as MayorRetencion, 0 as RentaNoGravadaSinAct, 0 as RentaTotalExentaSinAct, 0 as RebajasZonaExtremaSinAct,  " +
                     "dbo.fnTributableAjustable(rut, trabajador.anomes) as 'RentaAfectaImptoReajustada', " +
                     "dbo.fnValorIMpuesto(dbo.fntributableAjustable(rut, trabajador.anomes), trabajador.anomes) as 'ImpoReajustado', " +
                     "0 as'MayorRetdelimptoAct', 0 as 'RentaTotalNoGravadaAct', 0 as 'RentaTotalExentaAct', 0 as 'RebajasExtremaAct',  " +
                     "IIF(trabajador.anomes = 201901, 1, 0) as Enero, " +
                     "IIF(trabajador.anomes = 201902, 1, 0) as Febrero, " +
                     "IIF(trabajador.anomes = 201903, 1, 0) as Marzo, " +
                     "IIF(trabajador.anomes = 201904, 1, 0) as Abril, " +
                     "IIF(trabajador.anomes = 201905, 1, 0) as Mayo, " +
                     "IIF(trabajador.anomes = 201906, 1, 0) as Junio, " +
                     "IIF(trabajador.anomes = 201907, 1, 0) as Julio, " +
                     "IIF(trabajador.anomes = 201908, 1, 0) as Agosto, " +
                     "IIF(trabajador.anomes = 201909, 1, 0) as Septiembre, " +
                     "IIF(trabajador.anomes = 201910, 1, 0) as Octubre, " +
                     "IIF(trabajador.anomes = 201911, 1, 0) as Noviembre, " +
                     "IIF(trabajador.anomes = 201912, 1, 0) as Diciembre " +
                     "FROM trabajador " +
                     "INNER JOIN calculomensual ON calculomensual.contrato = trabajador.contrato AND calculomensual.anomes = trabajador.anomes " +
                     "WHERE status = 1 AND (trabajador.anomes >=@pInicio AND trabajador.anomes <=@pTermino)  ) as source " +
                     "GROUP BY rut";
            #endregion

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
                            cmd.Parameters.Add(new SqlParameter("@pInicio",Inicio));
                            cmd.Parameters.Add(new SqlParameter("@pTermino", Termino));

                            ad.SelectCommand = cmd;
                            ad.Fill(ds, "data");

                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                tb = ds.Tables[0];
                            }

                            ad.Dispose();
                            ds.Dispose();
                            cmd.Dispose();
                        }
                    }
                }

            }
            catch (SqlException ex)
            {
                //ERROR...
            }

            return tb;

        }

        /// <summary>
        /// Verifica si para un determinado rut hay meses repetidos
        /// </summary>
        private bool MesesRepetidos(DataTable pTabla)
        {
            string Mes = "", Rut = "";
            int En = 0, Feb = 0, Mar = 0, Ab = 0, May = 0, Jun = 0, Jul = 0, Ago = 0;
            int Sep = 0, Oct = 0, Nov = 0, Dic = 0;
            bool rep = false;
            if (pTabla.Rows.Count > 0)
            {
                List<string> RutEncontratos = pTabla.AsEnumerable()
                                           .Select(row => row.Field<string>("rut")).Distinct().ToList();

                try
                {
                    foreach (var r in RutEncontratos)
                    {
                        ///Recorrimos todos los meses asociados a al rut del for exterior
                        foreach (var row in pTabla.Select($"rut='{r}'", "periodo asc"))
                        {
                            string periodo = row["periodo"].ToString();

                            Mes = row["periodo"].ToString().Substring(4, 2);

                            switch (Mes)
                            {
                                case "01":
                                    En++;
                                    break;
                                case "02":
                                    Feb++;
                                    break;
                                case "03":
                                    Mar++;
                                    break;
                                case "04":
                                    Ab++;
                                    break;
                                case "05":
                                    May++;
                                    break;
                                case "06":
                                    Jun++;
                                    break;
                                case "07":
                                    Jul++;
                                    break;
                                case "08":
                                    Ago++;
                                    break;
                                case "09":
                                    Sep++;
                                    break;
                                case "10":
                                    Oct++;
                                    break;
                                case "11":
                                    Nov++;
                                    break;
                                case "12":
                                    Dic++;
                                    break;

                                default:
                                    break;
                            }

                            //Mes repetido???
                            //Break ciclo
                            if (En > 1 || Feb > 1 || Mar > 1 || Ab > 1 || May > 1 || Jun > 1 || Jul > 1 || Ago > 1 || Sep > 1 || Oct > 1 || Nov > 1 || Dic > 1)
                            {
                                rep = true;
                                break;
                            }
                        }
                        //Hay meses repetidos???
                        //Break ciclo
                        if (rep)
                        {
                            break;
                        }

                        En = 0;
                        Feb = 0;
                        Mar = 0;
                        Ab = 0;
                        May = 0;
                        Jun = 0;
                        Jul = 0;
                        Ago = 0;
                        Sep = 0;
                        Oct = 0;
                        Nov = 0;
                        Dic = 0;
                    }
                }
                catch (Exception ex)
                {
                    //ERROR
                    string d = ex.Message;
                }
            }

            return rep;
        }

        /// <summary>
        /// Lectura archivo de entrada para formulario 1879
        /// </summary>
        private DataTable ReadData(string pRuta)
        {
            DataTable Data = new DataTable();
            bool Error = false;
            try
            {
                Data = FileExcel.ReadExcelDev(pRuta);
                if (Data != null)
                {
                    if (Data.Rows.Count > 0)
                    {
                        //VERIFICAMOS QUE ESTEN TODAS LAS COLUMNAS
                        DataColumnCollection Columnas = Data.Columns;
                        foreach (DataColumn col in Columnas)
                        {
                            if (ExisteColumna(col.ColumnName) == false)
                            { XtraMessageBox.Show($"La columna {col.ColumnName} no es válida", "Nombre columna", MessageBoxButtons.OK, MessageBoxIcon.Stop); return null; }
                        }                    

                        //Recorremos filas
                        foreach (DataRow row in Data.Rows)
                        {
                            foreach (DataColumn col in Data.Columns)
                            {
                                if (ValidaData(row[col].ToString(), col.ColumnName) == false)
                                { Error = true; break; }
                            }
                            if (Error)
                                break;
                        }

                        #region "MESES REPETIDOS O MAS DE 12 REGISTROS POR RUT"
                        //Contar la cantidad de veces que existe un rut en la planilla
                        var contados = (from x in Data.AsEnumerable()
                                        group x by x.Field<string>("rut")
                                        into tabla
                                        let count = tabla.Count()
                                        select new
                                        {
                                            Rut = tabla.Key,
                                            Count = count
                                        });

                        //Nos indica en base a <<contados>> si hay algun rut que tenga mas de 12 registros (NO válido)
                        if (contados.Count(x => x.Count > 12) > 0)
                        {
                            //Entrega un registro que cumpla la condicion
                            var registro = contados.First(x => x.Count > 12);
                            XtraMessageBox.Show($"No puede haber un rut con mas de 12 meses registrados\nRut:{registro.Rut}, Meses:{registro.Count}", "Error meses", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            return null;
                        }

                        bool rep = MesesRepetidos(Data);
                        if (rep)
                        { XtraMessageBox.Show("Hay meses repetidos para el mismo rut", "Meses", MessageBoxButtons.OK, MessageBoxIcon.Stop); return null; }
                        #endregion
                    }
                }
            } 
            catch (Exception ex)
            {
                Data = null;
            }

            return Data;
        }

        /// <summary>
        /// Verifica si existe una columnna o si tiene el nombre de columna válido
        /// </summary>
        /// <param name="pColumnName">Nombre de la columna a buscar</param>
        /// <returns></returns>
        private bool ExisteColumna(string pColumnName)
        {
            bool Existe = false;
            try
            {
                if (ColumnasArchivo.Count > 0 && pColumnName.Length > 0)
                {
                    //Si retorna un valor mayor a 0 es porque existe coincidencias
                    if (ColumnasArchivo.Count(x => x.ToLower().Equals(pColumnName.ToLower())) > 0)
                        Existe = true;                    
                }
            }
            catch (Exception ex)
            {
                Existe = false;
            }

            return Existe;
        }

        /// <summary>
        /// Nos indica si hay algun error de validacion en la informacion del archivo externo.
        /// </summary>
        /// <param name="pValue"></param>
        /// <param name="pColumnaName"></param>
        private bool ValidaData(string pValue, string pColumnaName)
        {           
            switch (pColumnaName.ToLower())
            {
                case "rut":
                    if (pValue.Length == 0)
                    { XtraMessageBox.Show("El campo rut no puede estar vacío", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }
                    if (fnSistema.fValidaRut(pValue) == false)
                    { XtraMessageBox.Show($"Valor {pValue} no es un rut válido", "Modulo 11", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }                                        
                    break;

                case "nombre":
                    if (pValue.Length == 0)
                    { XtraMessageBox.Show("El campo <<nombre>> no puede estar vacío", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }                    
                    break;

                case "periodo":
                    if (pValue.Length == 0)
                    { XtraMessageBox.Show("El campo <<periodo>> no puede estar vacío", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    if (fnSistema.IsNumeric(pValue) == false)
                    { XtraMessageBox.Show("Por favor ingresa un valor numerico para campo <<periodo>>", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    if (Calculo.ValidaFormatoPeriodo(pValue) == false)
                    { XtraMessageBox.Show("Por favor verifica el formato para campo <<periodo>>", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    break;
                case "renta10":
                    if (pValue.Length == 0)
                    { XtraMessageBox.Show("El campo <<renta10>> no puede estar vacío", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    if (fnSistema.IsDecimal(pValue) == false)
                    { XtraMessageBox.Show("Por favor ingresa un valor numerico para campo <<renta10>>", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }
                    
                    break;
                case "renta35":
                    if (pValue.Length == 0)
                    { XtraMessageBox.Show("El campo <<renta35>> no puede estar vacío", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    if (fnSistema.IsDecimal(pValue) == false)
                    { XtraMessageBox.Show("Por favor ingresa un valor numerico para campo <<renta35>>", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }
                    
                    break;
                //case "bruto":
                //    if (pValue.Length == 0)
                //    { XtraMessageBox.Show("El campo <<bruto>> no puede estar vacío", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }
                //
                //    if (fnSistema.IsNumeric(pValue) == false)
                //    { XtraMessageBox.Show("Por favor ingresa un valor numerico en campo <<bruto>>", "Error formato", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                  //  break;
                case "impuesto":
                    if (pValue.Length == 0)
                    { XtraMessageBox.Show("El campo <<impuesto>> no puede estar vacío", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    if (fnSistema.IsDecimal(pValue) == false)
                    { XtraMessageBox.Show("Por favor ingresa un valor numerico en campo <<impuesto>>", "Error formato", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }
                    break;

                default:
                    break;
            }

            return true;
        }

      
        #endregion

        private void btnSalida_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            folder.RootFolder = Environment.SpecialFolder.Desktop;

            if (folder.ShowDialog() == DialogResult.OK)
            {
                txtSalida.Text = folder.SelectedPath;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;
            int LineasBase = 0;

            if (Directory.Exists(txtSalida.Text) == false)
            { XtraMessageBox.Show("Directorio de salida no válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);txtSalida.Focus(); return; }

            if (txtPeriodo.Properties.DataSource == null || txtPeriodo.EditValue == null)
            { XtraMessageBox.Show("Por favor selecciona un periodo válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtPeriodo.Focus(); return; }

            if (txtTipoDeclaración.Properties.DataSource == null || txtTipoDeclaración.EditValue == null)
            { XtraMessageBox.Show("Por favor selecciona el tipo de declaración", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);txtTipoDeclaración.Focus(); return; }

            if (txtTipoFormulario.Properties.DataSource == null || txtTipoFormulario.EditValue == null)
            { XtraMessageBox.Show("Por favor selecciona un tipo de formulario a declarar", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtTipoFormulario.Focus(); return; }

            if (txtTipoDeclaración.EditValue.ToString().ToLower().Equals("r") || txtTipoDeclaración.EditValue.ToString().ToLower().Equals("a"))
            {
                if (txtFolioAnt.Text == "")
                { XtraMessageBox.Show("Debes proporcionar el numero de folio que deseas rectificar o anular", "Error Folio", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtFolioAnt.Focus(); return; }

                if(txtFolioAnt.Text == "0" || txtFolioAnt.Text == "00" || txtFolioAnt.Text == "99")
                { XtraMessageBox.Show("Debes proporcionar el numero de folio que deseas rectificar o anular", "Error Folio", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtFolioAnt.Focus(); return; }

                if (txtFolioAnt.Text == txtFolioOriginal.Text)
                { XtraMessageBox.Show("El numero de folio a rectificar no puede ser igual al nuevo numero de folio", "Folios", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
            }

            if (txtFolioOriginal.Text == "")
            { XtraMessageBox.Show("Por favor ingresa un numero de folio", "Error folio", MessageBoxButtons.OK, MessageBoxIcon.Stop); txtFolioOriginal.Focus(); return; }

            try
            {
                string s0 = "", s1 = "", s2 = "", s3 = "", Lines = "", FileName = "";
                int PeriodoBusqueda = 0;
                Hashtable Declarantes = new Hashtable();

                if (txtTipoFormulario.EditValue.ToString() == "1887")
                {
                    PeriodoBusqueda = Convert.ToInt32(txtPeriodo.EditValue) - 1;

                    Seccion2 sec2 = new Seccion2();
                    Declarantes = sec2.GetInfoYear(PeriodoBusqueda, txtTipoFormulario.EditValue.ToString(), txtFolioBase.Text + txtFolioOriginal.Text, txtTipoDeclaración.EditValue.ToString(), emp);

                    if (Declarantes.Count > 0)
                    {
                        Seccion0 sec0 = new Seccion0();
                        if (txtTipoDeclaración.EditValue.ToString().ToLower().Equals("a"))
                            LineasBase = 6;
                        else
                            LineasBase = 6;

                        sec0.SetSection(emp, Convert.ToInt32(txtPeriodo.EditValue), Convert.ToInt32(txtTipoFormulario.EditValue), (Convert.ToInt32(Declarantes["registros"]) + LineasBase) + "", txtFolioBase.Text + txtFolioOriginal.Text, txtTipoDeclaración.EditValue.ToString(), txtCodEmpresa.Text, txtVersion.Text);

                        s0 = sec0.GetLine() + Environment.NewLine;

                        Seccion1 sec1 = new Seccion1();
                        sec1.SetSections(emp, Convert.ToInt32(txtPeriodo.EditValue), txtTipoFormulario.EditValue.ToString(), txtFolioBase.Text + txtFolioOriginal.Text, txtTipoDeclaración.EditValue.ToString(), "", "", txtCodEmpresa.Text, txtFolioAntBase.Text + txtFolioAnt.Text);

                        s1 = sec1.GetLine() + Environment.NewLine;
                        s2 = Declarantes["section"].ToString();

                        if (txtTipoDeclaración.EditValue.ToString().ToLower().Equals("o") || txtTipoDeclaración.EditValue.ToString().ToLower().Equals("r"))
                        {
                            Seccion3 sec3 = new Seccion3();
                            s3 = sec3.GetInfoYear(PeriodoBusqueda, emp, txtTipoFormulario.EditValue.ToString(), txtFolioBase.Text + txtFolioOriginal.Text, Declarantes);

                            Lines = s0 + s1 + s2 + s3;
                        }
                        else
                        {
                            //if (s2.Length > 0)
                            //    s2 = s2.Substring(0, s2.Length - 2);
                            Seccion3 sec3 = new Seccion3();
                            s3 = sec3.GetInfoYear(PeriodoBusqueda, emp, txtTipoFormulario.EditValue.ToString(), txtFolioBase.Text + txtFolioOriginal.Text, Declarantes);

                            Lines = s0 + s1 + s2 + s3;
                        }

                        FileName = txtSalida.Text + @"\" + $"{fnSistema.GetRutsindv(emp.Rut)}.887";

                        Archivo.CrearArchivo(FileName);
                        Archivo.EscribirArchivo(FileName, Lines);

                        if (File.Exists(FileName))
                        {
                            DialogResult ad = XtraMessageBox.Show("Archivo creado correctamente.\n¿Deseas ver archivo?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (ad == DialogResult.Yes)
                                System.Diagnostics.Process.Start(FileName);
                        }
                        else
                        {
                            XtraMessageBox.Show("No se pudo generar archivo", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        XtraMessageBox.Show("No se pudo generar archivo", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                }
                else if (txtTipoFormulario.EditValue.ToString() == "1879")
                {
                    Seccion0 sec0 = new Seccion0();                   

                    Seccion1 sec1 = new Seccion1();
                    s1 =sec1.GetSections1879(emp, Convert.ToInt32(txtPeriodo.EditValue), txtTipoFormulario.EditValue.ToString(), txtFolioBase.Text + txtFolioOriginal.Text, txtTipoDeclaración.EditValue.ToString(), txtCodEmpresa.Text, txtFolioAntBase.Text + txtFolioAnt.Text);                    

                    if (File.Exists(txtArchivo79.Text) == false)
                    { XtraMessageBox.Show("Por favor verifique que la ruta del archivo para formulario sea correcta", "Error archivo", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

                    DataTable dt = ReadData(txtArchivo79.Text);

                    if (dt == null || dt.Rows.Count == 0)
                    {
                        XtraMessageBox.Show("No se encontró información en archivo", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }

                    Hashtable ht = new Hashtable();
                    if (dt != null)
                    {
                        Seccion2 sec2 = new Seccion2();
                        if (txtTipoDeclaración.EditValue.ToString().ToLower().Equals("a"))
                            LineasBase = 3;
                        else
                            LineasBase = 3;

                        ht = sec2.GetData1879(dt, txtTipoFormulario.EditValue.ToString(), txtFolioBase.Text + txtFolioOriginal.Text, emp, txtTipoDeclaración.EditValue.ToString());
                        if (ht.Count > 0)
                        {
                            sec0.SetSection(emp, Convert.ToInt32(txtPeriodo.EditValue), Convert.ToInt32(txtTipoFormulario.EditValue), (Convert.ToInt32(ht["casos"]) + LineasBase) + "", txtFolioBase.Text + txtFolioOriginal.Text, txtTipoDeclaración.EditValue.ToString(), txtCodEmpresa.Text, txtVersion.Text);
                            s0 = sec0.GetLine() + Environment.NewLine;
                            
                            s2 = ht["section"].ToString();

                            //Final
                            if (txtTipoDeclaración.EditValue.ToString().ToLower().Equals("o") || txtTipoDeclaración.EditValue.ToString().ToLower().Equals("r"))
                            {
                                Seccion3 sec3 = new Seccion3();
                                s3 = sec3.GetDetail1879(ht, emp, txtTipoFormulario.EditValue.ToString(), txtFolioBase.Text + txtFolioOriginal.Text);                                
                                Lines = s0 + s1 + s2 + s3;
                            }
                            else
                            {
                                //  if(s2.Length > 0)
                                //     s2 = s2.Substring(0, s2.Length-2);

                                Seccion3 sec3 = new Seccion3();
                                s3 = sec3.GetDetail1879(ht, emp, txtTipoFormulario.EditValue.ToString(), txtFolioBase.Text + txtFolioOriginal.Text);

                                Lines = s0 + s1 + s2 + s3;
                            }
                                                        

                            FileName = txtSalida.Text + @"\" + $"{fnSistema.GetRutsindv(emp.Rut)}.879";

                            Archivo.CrearArchivo(FileName);
                            Archivo.EscribirArchivo(FileName, Lines);

                            if (File.Exists(FileName))
                            {
                                DialogResult ad = XtraMessageBox.Show("Archivo creado correctamente.\n¿Deseas ver archivo?", "Pregunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (ad == DialogResult.Yes)
                                    System.Diagnostics.Process.Start(FileName);
                            }
                            else
                            {
                                XtraMessageBox.Show("No se pudo generar archivo", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }

                        }
                        else
                        {
                            XtraMessageBox.Show("No se encontró información", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            return;
                        }
                    }                   
                }                           
            }
            catch (Exception ex)
            {
                //ERROR...
                XtraMessageBox.Show("No se pudo generar archivo porque ocurrió un error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void txtTipoDeclaración_EditValueChanged(object sender, EventArgs e)
        {
            string Option = "";
            Option = txtTipoDeclaración.EditValue.ToString();
            if (Option.ToLower().Equals("r") || Option.ToLower().Equals("a"))
            {
                txtFolioAnt.Text = "";
                txtFolioAnt.Focus();
                txtFolioAnt.Enabled = true;
            }
            else
            {
                txtFolioAnt.Text = "";
                txtFolioAnt.Enabled = false;
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            DialogResult adv = XtraMessageBox.Show("¿Estás seguro de salir?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (adv == DialogResult.Yes)
                Close();

            
        }

        private void txtFolioAnterior_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void txtFolio_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void txtFolioAnt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void txtFolioOriginal_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void txtFolioAnt_Leave(object sender, EventArgs e)
        {
            string value = "", sub = "";
            value = txtFolioAnt.Text;
            try
            {
                if (value.Length > 0)
                {
                    if (fnSistema.IsNumeric(value))
                    {
                        sub = "0" + (Convert.ToInt32(value) + 1).ToString();
                        txtFolioOriginal.Text = fnSistema.fnRightString(sub, 2);                        
                    }
                }
            }
            catch (Exception ex)
            {
                //error...
            }
        }

        private void btnReporte_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            Cursor.Current = Cursors.WaitCursor;

            if (txtPeriodo.Properties.DataSource == null || txtPeriodo.EditValue == null)
            { XtraMessageBox.Show("Por favor selecciona un periodo válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }

            DataTable tabla = new DataTable();
            int PeriodoBusqueda = 0;
            Empresa emp = new Empresa();
            emp.SetInfo();
            try
            {
                PeriodoBusqueda = Convert.ToInt32(txtPeriodo.EditValue) - 1;
                tabla = GetDataSourceReport(PeriodoBusqueda);

                if (tabla.Rows.Count > 0)
                {
                    Documento doc = new Documento("", 0);

                    rptDeclaracionJurada dec = new rptDeclaracionJurada();
                    dec.DataSource = tabla;
                    dec.DataMember = "data";

                    dec.Parameters["folio"].Value = txtFolioBase.Text + txtFolioOriginal.Text;
                    dec.Parameters["casos"].Value = tabla.Rows.Count;
                    dec.Parameters["comuna"].Value = emp.Comuna;
                    dec.Parameters["correo"].Value = emp.EmailEmp;
                    dec.Parameters["direccion"].Value = emp.Direccion;
                    dec.Parameters["fax"].Value = emp.CodigoPais + emp.CodigoArea + emp.Telefono;
                    dec.Parameters["razon"].Value = emp.Razon;
                    dec.Parameters["rutemp"].Value = fnSistema.fFormatearRut2(emp.Rut);
                    dec.Parameters["rutrepresentante"].Value = fnSistema.fFormatearRut2(emp.RutRep);
                    dec.Parameters["telefono"].Value = emp.CodigoArea + emp.Telefono;

                    //PARA QUE NO APARESCA EL FORMULARIO DE INGRESO DE VALORES PARA PARAMETROS
                    foreach (DevExpress.XtraReports.Parameters.Parameter item in dec.Parameters)
                    {
                        item.Visible = false;
                    }

                    doc.ShowDocument(dec);
                }

            }
            catch (SqlException ex)
            {
                //ERROR...
                XtraMessageBox.Show("No se pudo generar reporte", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void txtCodEmpresa_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void txtVersion_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void txtTipoFormulario_EditValueChanged(object sender, EventArgs e)
        {
            if (txtTipoFormulario.Properties.DataSource != null)
            {
                if (Convert.ToInt32(txtTipoFormulario.EditValue) == 1887)
                {
                    txtArchivo79.Text = "";
                    txtArchivo79.Enabled = false;
                    btn79.Enabled = false;
                }
                else
                {
                    txtArchivo79.Text = "";
                    txtArchivo79.Enabled = true;
                    btn79.Enabled = true;
                }
            }
        }

        private void btn79_Click(object sender, EventArgs e)
        {
            Sesion.NuevaActividad();

            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Excel Files|*.xls;*.xlsx";
            if (open.ShowDialog() == DialogResult.OK)
            {
                Cursor.Current = Cursors.WaitCursor;

                txtArchivo79.Text = open.FileName;

                DataTable dt = ReadData(open.FileName);
                Hashtable ht = new Hashtable();
                if (dt == null)
                {
                    XtraMessageBox.Show("Archivo con errores", "Errores", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
        }
    }
}