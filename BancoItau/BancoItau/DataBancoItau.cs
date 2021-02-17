using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BancoItau
{
    public class DataBancoItau
    {
        Configuracion conf;
        /// <summary>
        /// Direccion ip o nombre del servidor.
        /// </summary>
        private string Server { get; set; }
        /// <summary>
        /// Nombre de la base de datos.
        /// </summary>
        private string Database { get; set; }
        /// <summary>
        /// Usuario para conexion con base de datos.
        /// </summary>
        private string User { get; set; }
        /// <summary>
        /// Password para conexion con base de datos.
        /// </summary>
        private string Pass { get; set; }
        /// <summary>
        /// Representa la cadena de conexion para abrir una conexion con la base de datos.
        /// </summary>
        public string StringConnection { get; set; }
        ///<sumary>
        ///Representa el valor del atributo pConjunto
        ///</sumary>
        private string conjunto { get; set; }
        ///<sumary>
        ///Representa el valor del checkBox Todos
        ///</sumary>
        private bool cbTodos { get; set; }
        ///<sumary>
        ///Representa el valor del txtItem
        ///</sumary>
        private string txtItem { get; set; }
        ///<sumary>
        ///Representa el valor de ShowPrivados
        ///</sumary>
        private bool ShowPrivados { get; set; }
        ///<sumary>
        ///Representa el valor de Filtro
        ///</sumary>
        private string Filtro { get; set; }
        ///<sumary>
        ///Representa el valor de Filtro
        ///</sumary>
        private string GetoConditionFiltro { get; set; }
        ///<sumary>
        ///Representa el valor de código
        ///</sumary>
        private string GetoConditionCode { get; set; }
        ///<sumary>
        ///Representa el valor del período
        ///</sumary>
        private int txtPeriodo { get; set; }
        ///<sumary>
        ///Representa el valor de la fechaProceso
        ///</sumary>
        private string fechaProceso { get; set; }
        /// <summary>
        /// Corresponde a la ruta del archivo que se creará (incluye el nombre).
        /// </summary>
        private string RutaSalida { get; set; }

        ///<sumary>
        ///Representa el valor del banco
        ///</sumary>
        private int Banco { get; set; }

        /// <summary>
        /// Listado de personas que viene desde formulario principal
        /// </summary>
        public List<Cuerpo> Listado { get; set; }

        /// <summary>
        /// Codigo de banco seleccionado en formulario principal.
        /// </summary>
        private int IdBanco { get; set; }

        /// <summary>
        /// REPRESENTA LA INFORMACION DE LA CABECERA DEL ARCHIVO.
        /// Información referente a la empresa.
        /// </summary>
        public Header HeaderInfo { get; set; }

        /// <summary>
        /// REPRESENTA LA INFORMACION DEL HASTABLE QUE TRAE LOS DATOS DE LA CABECERA.
        /// Información referente a la empresa.
        /// </summary>
        public Hashtable HashHeader { get; set; }

        /// <summary>
        /// TIPO DE DOCUMENTO
        /// <para>Representa el codigo del banco.</para>
        /// </summary>
        private int DocumentoTipo { get; set; } = 0;

        /// <summary>
        /// RESUTLADO DE SQL PARA PASAR AL REPORTE
        /// </summary>
        public string dataSql { get; set; }

        public DataBancoItau()
        {
            //CONSTRUCTOR VACÍO
        }

        //PASARLE LOS VALORES A LISTADO Y PARÁMETROS DE CONEXIÓN
        public DataBancoItau(string pGetoConditionCode, bool pcbTodos, string ptxtItem, bool pShowPrivados, string pFiltro, string pGetConditionFiltro, string pfechaProceso, int ptxtPeriodo, int pBanco, Hashtable pHeader, string pServer, string pDataBase, string pUser, string pPass)
        {

            cbTodos = pcbTodos;
            txtItem = ptxtItem;
            ShowPrivados = pShowPrivados;
            Filtro = pFiltro;
            GetoConditionFiltro = pGetConditionFiltro;
            Banco = pBanco;
            Server = pServer;
            Database = pDataBase;
            User = pUser;
            Pass = pPass;
            txtPeriodo = ptxtPeriodo;
            fechaProceso = pfechaProceso;
            GetoConditionCode = pGetoConditionCode;
            HashHeader = pHeader;

            //SE CONECTA SQLSERVER
            if (Server.Length > 0 && Pass.Length > 0 && User.Length > 0 && Database.Length > 0)
            {
                conf = new Configuracion(Server, User, Pass, Database);
                StringConnection = conf.GetConnectionString();

            }

            //REALIZO LA QUERY PARA OBTENER DATA
            Query query = new Query();
            string data = query.GetSql(GetoConditionCode, cbTodos, txtItem, ShowPrivados, Filtro, GetoConditionFiltro, txtPeriodo, txtItem);

            //OBTENGO EL LISTADO
            Listado = SetListado(StringConnection, data, fechaProceso);

            if (Listado.Count > 0)
            {
                ShowForm();
            }

        }


        /// <summary>
        /// Representa la ventana que se va a abrir de acuerdo a codigo de banco. DEPENDE DEL CODIGO DEL BANCO QUE FORM SE ABRE!!
        /// </summary>
        public void ShowForm()
        {
            if (Banco > 0 && Listado.Count > 0 && HashHeader != null)
            {
                switch (Banco)
                {
                    case 39:
                        //PASAMOS COMO PARAMETRO EL LISTADO
                        FrmBancoItau Ventana = new FrmBancoItau(Listado, HashHeader, StringConnection, dataSql);
                        Ventana.StartPosition = FormStartPosition.CenterParent;
                        Ventana.ShowDialog();
                        break;

                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Genera archivo nomina para bancos bci
        /// </summary>
        /// <param name="pRutaSalida">Ruta de salida del archivo</param>
        public void CrearArchivoItau(string pRutaSalida, List<Cuerpo> pListado, Hashtable pHashHeader, string pMediosRespaldo, string pNumeroCuenta, string pDescripcion, string pFormatoArchivo, int pDescripcionLength)
        {
            Query query = new Query();
            Connection con = new Connection();
            SQLiteConnection sqlcon = con.sQliteOpenConnection();
            Listado = pListado;
            HashHeader = pHashHeader;
            string Row = "", RowHeader = "";
            string pTipoServicio = "";
            if (pRutaSalida.Length > 0 && pNumeroCuenta.Length > 0 && Listado.Count > 0)
            {
                //LIMPIAR ARCHIVO SI EXISTE
                CleanTextFile(pRutaSalida);
                pNumeroCuenta.Trim();

                //RECORREMOS LISTADO Y GENERAR FILA POR CADA FILA DE LA LISTA (CASH)
                if (pFormatoArchivo == "1")
                {
                    //INFORMACION DE CABECERA
                    if (HashHeader != null)
                    {
                        //SE RECORRE EL LISTADO PARA SUMAR LOS MONTOS
                        int suma = 0;
                        foreach (var item in Listado)
                        {
                            suma = suma + (int)item.Monto;
                        }

                        //ASIGNAMOS VALOR A CADA PARTE DEL HEADERINFO DESDE EL HASHTABLE *SE PUEDE MEJORAR*
                        RowHeader = $"{HashHeader["rutempresa"].ToString()},{Listado.Count},{suma},{query.CargarTipoServicio(1)},{pMediosRespaldo},{pNumeroCuenta},{pDescripcion}";
                        WriteFileText(pRutaSalida, RowHeader);
                    }

                    foreach (Cuerpo item in Listado)
                    {
                        /*VALORES FIJOS*/
                        item.MedioRespaldo = "CAT_CSH_CCTE";
                        item.NroMedioResp = pNumeroCuenta;
                        //SI NO ES TRANSFERENCIA EL NUMERO DE CUENTA VA VACÍO
                        if (item.MetodoPago != "CAT_CSH_TRANSFER")
                        {
                            item.NroCuenta = "";
                        }

                        Row = $"{item.Rut},{item.Nombre},{item.Email},{item.MetodoPago}," +
                              $"{item.CodigoBanco},{item.TipoDeCuenta},{item.NroCuenta},{item.FechaDePago}" +
                              $",,,{item.Monto},{item.MedioRespaldo},{item.NroMedioResp},{item.CodigoSucursal},{item.ReferenciaCliente},{item.GlosaDelPago}";
                        WriteFileText(pRutaSalida, Row);
                    }
                }
                //RECORREMOS LISTADO Y GENERAR FILA POR CADA FILA DE LA LISTA (CIRCULO PAGOS ONLINE)
                else
                {
                    //INFORMACION DE CABECERA
                    if (HashHeader != null)
                    {
                        //SE RECORRE EL LISTADO PARA SUMAR LOS MONTOS
                        int suma = 0;
                        foreach (var item in Listado)
                        {
                            suma = suma + (int)item.Monto;
                        }

                        //ASIGNAMOS VALOR A CADA PARTE DEL HEADERINFO DESDE EL HASHTABLE *SE PUEDE MEJORAR*
                        //SE ASIGNA EL TIPO DE SERICIO "TRANSFERENCIA"
                        RowHeader = $"{HashHeader["rutempresa"].ToString()},{Listado.Count},{suma},{query.CargarTipoServicio(22)},{pMediosRespaldo},{pNumeroCuenta},{pDescripcion},,";
                        WriteFileText(pRutaSalida, RowHeader);
                    }

                    foreach (Cuerpo item in Listado)
                    {
                        /*VALORES FIJOS*/
                        item.MedioRespaldo = "CAT_CSH_CCTE";
                        item.NroMedioResp = pNumeroCuenta;
                        //SI NO ES TRANSFERENCIA EL NUMERO DE CUENTA VA VACÍO
                        if (item.MetodoPago != "CAT_CSH_TRANSFER")
                        {
                            item.NroCuenta = "";
                        }

                        Row = $"{item.Rut},{item.Nombre},{item.Email},{item.MetodoPago}," +
                              $"{item.CodigoBanco},{item.TipoDeCuenta},{item.NroCuenta}," +
                              $",,,{item.Monto},{item.MedioRespaldo},{item.NroMedioResp},{item.CodigoSucursal},{item.ReferenciaCliente},{item.GlosaDelPago}";
                        WriteFileText(pRutaSalida, Row);
                    }
                }               
                

            }
        }

        /// <summary>
        /// ESCRIBE LA INFORMACION DESDE PROPIEDAD DATASOURCE EN ARCHIVO DE TEXTO
        /// </summary>
        /// <param name="pRuta">RUTA DEL ARCHIVO DE SALIDA</param>
        /// <param name="pData">INFORMACION QUE SE AGREGARÁ AL ARCHIVO</param>
        private void WriteFileText(string pRuta, string pData)
        {
            try
            {
                using (StreamWriter str = new StreamWriter(pRuta, true))
                {
                    str.WriteLine(pData);
                }
            }
            catch (Exception ex)
            {
                //ERROR 
            }
        }

        /// <summary>
        /// LIMPIA EL ARCHIVO DE TEXTO, SI ES QUE ESTE EXISTE
        /// </summary>
        /// <param name="PathFile">RUTA DEL ARCHIVO</param>
        private void CleanTextFile(string PathFile)
        {
            if (File.Exists(PathFile))
            {
                try
                {
                    using (var fs = new FileStream(PathFile, FileMode.Truncate))
                    {
                        //...
                    }
                }
                catch (Exception ex)
                {
                    //ERROR...
                }
            }
        }

        //GENERAR LISTADO DE TIPO CUERPO
        private List<Cuerpo> SetListado(string pStringConecction, string pSql, string fechaProceso)
        {
            List<Cuerpo> lista = new List<Cuerpo>();
            Connection con = new Connection(pStringConecction);
            SqlCommand cmd;
            SqlConnection cn;
            SqlDataReader rd;
            Query query = new Query();
            query.SetTiposCuentas();
            query.SetMetodoPagos();
            string pTipodeCuenta = "";
            string pMetodoPago = "";
            string pCodigoBanco = "";
            string pCodigoSucursal = "";
            string pNumeroCuenta = "";
            if (pSql.Length > 0)
            {
                try
                {
                    cn = con.OpenConnection();
                    if (cn != null)
                    {
                        using (cn)
                        {
                            using (cmd = new SqlCommand(pSql, cn))
                            {
                                ////PARAMETROS
                                //cmd.Parameters.Add(new SqlParameter("@pPeriodo", pPeriodo));
                                //cmd.Parameters.Add(new SqlParameter("@pCoditem", pCoditem));
                                dataSql = cmd.CommandText;
                                rd = cmd.ExecuteReader();
                                if (rd.HasRows)
                                {
                                    while (rd.Read())
                                    {

                                        //PAGO POR CUENTA TRANSFERENCIA
                                        if (rd["formapago"].ToString().Trim() == "2")
                                        {
                                            pMetodoPago = query.MetodoPago1;
                                            pCodigoBanco = (string)rd["dato01"];
                                            pNumeroCuenta = rd["cuenta"].ToString().Trim();
                                            if (pCodigoBanco == "039")
                                            {
                                                pCodigoSucursal = "001";
                                            }
                                            else
                                            {
                                                pCodigoSucursal = "";
                                            }
                                            //SELECCIÓN DE TIPO DE CUENTA SI NO ES CHEQUE
                                            if (rd["tipoCuenta"].ToString().Trim() == "4")
                                            {
                                                pTipodeCuenta = query.TipoCuenta1;
                                            }
                                            if (rd["tipoCuenta"].ToString().Trim() == "2")
                                            {
                                                pTipodeCuenta = query.TipoCuenta2;
                                            }
                                            if (rd["tipoCuenta"].ToString().Trim() == "1")
                                            {
                                                pTipodeCuenta = query.TipoCuenta3;
                                            }
                                            if (rd["tipoCuenta"].ToString().Trim() == "3")
                                            {
                                                pTipodeCuenta = query.TipoCuenta2;
                                            }
                                        }
                                        else
                                        {
                                            //PAGO POR CUENTA VALE VISTA VIRTUAL
                                            if (rd["formapago"].ToString().Trim() == "5")
                                            {
                                                pMetodoPago = query.MetodoPago2;

                                            }
                                            //PAGO POR CUENTA VALE VISTA IMPRESO
                                            if (rd["formapago"].ToString().Trim() == "4")
                                            {
                                                pMetodoPago = query.MetodoPago3;
                                            }
                                            //PAGO POR ORDEN DE PAGO
                                            if (rd["formapago"].ToString().Trim() == "6")
                                            {
                                                pMetodoPago = query.MetodoPago4;
                                            }
                                            pCodigoBanco = "039";
                                            pCodigoSucursal = "001";
                                            pTipodeCuenta = "";
                                            pNumeroCuenta = "";

                                        }

                                        lista.Add(new Cuerpo()
                                        {
                                            Rut = rd["rut"].ToString().Trim(),
                                            Nombre = rd["nombre"].ToString().Trim(),
                                            Email = rd["mail"].ToString().Trim(),
                                            MetodoPago = pMetodoPago,
                                            CodigoBanco = pCodigoBanco,
                                            TipoDeCuenta = pTipodeCuenta,
                                            NroCuenta = pNumeroCuenta,
                                            FechaDePago = fechaProceso,
                                            Monto = Convert.ToDouble(rd["valor"]),
                                            CodigoSucursal = pCodigoSucursal
                                        });
                                    }
                                }
                            }
                            cmd.Dispose();
                            rd.Close();
                        }
                    }
                }
                catch (SqlException ex)
                {
                    //ERROR SQL
                }
            }

            return lista;
        }
    }

    /// <summary>
    /// Permite crear y manipular consultas a la base de datos.
    /// </summary>
    public class Query
    {
        public string TipoCuenta1 { get; set; }
        public string TipoCuenta2 { get; set; }
        public string TipoCuenta3 { get; set; }
        public string TipoCuenta4 { get; set; }
        /// <summary>
        /// Método de pago mediante transferencia de fondos CAT_CSH_TRANSFER
        /// </summary>
        public string MetodoPago1 { get; set; }
        /// <summary>
        /// Método de pago mediante un vale vista virtual CAT_CSH_VIRTUAL_OFFICE_CHECK
        /// </summary>
        public string MetodoPago2 { get; set; }
        /// <summary>
        /// Método de pago mediante un vale vista impreso CAT_CSH_PRINTED_OFFICE_CHECK
        /// </summary>
        public string MetodoPago3 { get; set; }
        /// <summary>
        /// Método de pago mediante una orden de pago CAT_CSH_PAYMENT_ORDER
        /// </summary>
        public string MetodoPago4 { get; set; }
        /// <summary>
        /// NUMERO CUENTA DE EMPRESA
        /// </summary>
        public string NumeroCuenta { get; set; }

        //GENERAR SQL CONSULTA
        public string GetSql(string pgetConditionCode, bool cbTodos, string txtItem, bool ShowPrivados, string Filtro, string GetConditionFiltro, int pPeriodo, string pCoditem)
        {
            string sql = "";
            //int caer = int.Parse("k");
            if (cbTodos == true)
            {
                //SOLO PARA MOSTRAR EL TOTAL PAGO
                if (txtItem == "TPAGO")
                {
                    sql = "SELECT IIF(SUBSTRING(trabajador.rut, 1, 1) = '0', RIGHT(trabajador.rut, 8), trabajador.rut) as rut, " +
                        "CONCAT(apepaterno, ' ', apematerno, ' ', trabajador.nombre) as nombre, banco, banco.nombre as nombreBanco, " +
                        "pago as valor, banco.dato01, formapago.dato01 as formapago, tipoCuenta, tipoCuenta.nombre as NombreTipoCuenta, cuenta, mail " +
                        "FROM trabajador " +
                        "INNER JOIN liquidacionHistorico ON liquidacionHistorico.contrato = trabajador.contrato AND liquidacionHistorico.anomes = trabajador.anomes " +
                        "INNER JOIN banco ON banco.id = trabajador.banco " +
                        "INNER JOIN formaPago ON formaPago.id = trabajador.formapago " +
                        "INNER JOIN tipoCuenta ON tipoCuenta.id = trabajador.tipoCuenta " + 
                        $"WHERE trabajador.anomes = {pPeriodo} {(ShowPrivados == false ? "AND privado = 0" : "")} " + 
                        $"{(Filtro != "0" ? ($" {GetConditionFiltro}") : "")} " + 
                        "ORDER BY nombre";
                }
                else
                {
                    sql = "SELECT IIF(SUBSTRING(trabajador.rut, 1, 1) = '0', RIGHT(trabajador.rut, 8), trabajador.rut) as rut, " +
                        "CONCAT(apepaterno, ' ', apematerno, ' ', trabajador.nombre) as nombre, trabajador.banco as banco, banco.nombre as nombreBanco, " +
                        "valorcalculado as valor, banco.dato01, formapago.dato01 as formapago, trabajador.tipoCuenta as tipoCuenta, tipoCuenta.nombre as NombreTipoCuenta, cuenta, mail " +
                        "FROM trabajador " +
                        "INNER JOIN itemtrabajador ON trabajador.contrato = itemTrabajador.contrato AND itemtrabajador.anomes = trabajador.anomes " +
                        "INNER JOIN banco ON banco.id = trabajador.banco " +
                        "INNER JOIN formaPago ON formaPago.id = trabajador.formapago " +
                        "INNER JOIN tipoCuenta ON tipoCuenta.id = trabajador.tipoCuenta " + 
                        $"WHERE trabajador.anomes = {pPeriodo} AND coditem = '{pCoditem}' {(ShowPrivados == false ? "AND privado=0" : "")} " + 
                        $"{(Filtro != "0" ? ($" {GetConditionFiltro}") : "")}" + 
                        "ORDER BY nombre";
                }
            }
            else
            {
                if (txtItem == "TPAGO")
                {
                    sql = "SELECT IIF(SUBSTRING(trabajador.rut, 1, 1) = '0', RIGHT(trabajador.rut, 8), trabajador.rut) as rut, " +
                        "CONCAT(apepaterno, ' ', apematerno, ' ', trabajador.nombre) as nombre, banco, banco.nombre as nombreBanco, " +
                        "pago as valor, banco.dato01, formapago.dato01 as formapago, tipoCuenta, tipoCuenta.nombre as NombreTipoCuenta, cuenta, mail " +
                        "FROM trabajador " +
                        "INNER JOIN liquidacionHistorico ON liquidacionHistorico.contrato = trabajador.contrato AND liquidacionHistorico.anomes = trabajador.anomes " +
                        "INNER JOIN banco ON banco.id = trabajador.banco " +
                        "INNER JOIN formaPago ON formaPago.id = trabajador.formapago " +
                        "INNER JOIN tipoCuenta ON tipoCuenta.id = trabajador.tipoCuenta " + 
                        $"WHERE trabajador.anomes = {pPeriodo} {(ShowPrivados == false ? "AND privado = 0" : "")} " + 
                        $"{(Filtro != "0" ? ($"{GetConditionFiltro}") : "")} " + 
                        $"AND trabajador.contrato IN (SELECT contrato FROM trabajador WHERE {pgetConditionCode} )" + 
                        "ORDER BY nombre";
                }
                else
                {
                    
                    sql = "SELECT IIF(SUBSTRING(trabajador.rut, 1, 1) = '0', RIGHT(trabajador.rut, 8), trabajador.rut) as rut, " +
                        "CONCAT(apepaterno, ' ', apematerno, ' ', trabajador.nombre) as nombre, banco, banco.nombre as nombreBanco, " +
                        "valorcalculado as valor, banco.dato01, formapago.dato01 as formapago, tipoCuenta, tipoCuenta.nombre as NombreTipoCuenta, cuenta, mail " +
                        "FROM trabajador " +
                        "INNER JOIN itemtrabajador ON trabajador.contrato = itemTrabajador.contrato AND itemtrabajador.anomes = trabajador.anomes " +
                        "INNER JOIN banco ON banco.id = trabajador.banco " +
                        "INNER JOIN formaPago ON formaPago.id = trabajador.formapago " +
                        "INNER JOIN tipoCuenta ON tipoCuenta.id = trabajador.tipoCuenta " + 
                        $"WHERE trabajador.anomes = {pPeriodo} AND coditem = '{pCoditem}' {(ShowPrivados == false ? "AND privado=0" : "")} " + 
                        $"{(Filtro != "0" ? ($" {GetConditionFiltro}") : "")} " + 
                        $" AND trabajador.contrato IN (SELECT contrato FROM trabajador WHERE {pgetConditionCode} )" + 
                        "ORDER BY nombre";
                }
            }
            return sql;
        }

        public void FormatoArchivos(LookUpEdit pCombobox)
        {
            Connection sqlite = new Connection();
            SQLiteConnection sqlitecon = sqlite.sQliteOpenConnection();

            string query = "SELECT id, descripcion FROM FormatoArchivo";
            var datatable = new DataTable();
            try
            {
                if (sqlitecon != null)
                {
                    using (sqlitecon)
                    {
                        SQLiteCommand sqlcmd = new SQLiteCommand(query, sqlitecon);
                        var datareader = sqlcmd.ExecuteReader();
                        datatable.Load(datareader);
                    }
                }
                pCombobox.Properties.DataSource = datatable;
                pCombobox.Properties.ValueMember = "id";
                pCombobox.Properties.DisplayMember = "descripcion";
                pCombobox.Properties.PopulateColumns();
                pCombobox.Properties.Columns[0].Visible = false;
                pCombobox.Properties.ShowHeader = false;
                pCombobox.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
                pCombobox.Properties.AutoSearchColumnIndex = 1;
                pCombobox.ItemIndex = 0;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void MediosRespaldo(DevExpress.XtraEditors.LookUpEdit pCombobox)
        {
            Connection sqlite = new Connection();
            SQLiteConnection sqlcon = sqlite.sQliteOpenConnection();

            string querysql = "SELECT codigoAlfanumerico, descCombobox FROM TiposMediosRespaldo";
            var datatable = new DataTable();
            try
            {
                if (sqlcon != null)
                {
                    using (sqlcon)
                    {
                        SQLiteCommand sqlcmd = new SQLiteCommand(querysql, sqlcon);
                        var datareader = sqlcmd.ExecuteReader();
                        datatable.Load(datareader);
                    }
                }
                pCombobox.Properties.DataSource = datatable;
                pCombobox.Properties.ValueMember = "codigoAlfanumerico";
                pCombobox.Properties.DisplayMember = "descCombobox";
                pCombobox.Properties.PopulateColumns();
                pCombobox.Properties.Columns[0].Visible = false;
                pCombobox.Properties.ShowHeader = false;
                pCombobox.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
                pCombobox.Properties.AutoSearchColumnIndex = 1;
                pCombobox.ItemIndex = 0;
            }
            catch (Exception)
            {

                //ERROR AL CARGAR EL CBX
            }


        }

        public string CargarTipoServicio(int pIdservicio)
        {
            Connection sqlite = new Connection();
            SQLiteConnection sqlcon = sqlite.sQliteOpenConnection();
            string codigo = "";
            string querysql = $"SELECT codigoNumerico FROM TiposServicio WHERE id = {pIdservicio}";
            if (true)
            {
                if (sqlcon != null)
                {
                    using (sqlcon)
                    {
                        SQLiteCommand sqlcmd = new SQLiteCommand(querysql, sqlcon);
                        codigo = sqlcmd.ExecuteScalar().ToString();

                    }
                }
            }
            return codigo;
        }

        public void MetodosPago(DevExpress.XtraEditors.LookUpEdit pCombobox)
        {
            Connection sqlite = new Connection();
            SQLiteConnection sqlcon = sqlite.sQliteOpenConnection();
            string querysql = "SELECT codigoAlfanumerico, descCombobox FROM TiposMetodosPago";
            var datatable = new DataTable();
            try
            {
                if (sqlcon != null)
                {
                    using (sqlcon)
                    {
                        SQLiteCommand sqlcmd = new SQLiteCommand(querysql, sqlcon);
                        var datareader = sqlcmd.ExecuteReader();
                        datatable.Load(datareader);
                    }
                }
                pCombobox.Properties.DataSource = datatable;
                pCombobox.Properties.ValueMember = "codigoAlfanumerico";
                pCombobox.Properties.DisplayMember = "descCombobox";
                pCombobox.Properties.PopulateColumns();
                pCombobox.Properties.Columns[0].Visible = false;
                pCombobox.Properties.ShowHeader = false;
                pCombobox.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
                pCombobox.Properties.AutoSearchColumnIndex = 1;
                pCombobox.ItemIndex = 0;
            }
            catch (Exception)
            {

                //ERROR AL CARGAR EL CBX
            }
        }

        public void SetMetodoPagos()
        {
            Connection con = new Connection();
            SQLiteConnection sqlitecon = con.sQliteOpenConnection();
            string query = "SELECT codigoAlfanumerico FROM TiposMetodosPago";
            int contador = 0;
            string[] pTiposMetodosPago = new string[4];
            if (sqlitecon != null)
            {
                using (sqlitecon)
                {
                    using (SQLiteCommand sqlitecmd = new SQLiteCommand(query, sqlitecon))
                    {
                        using (SQLiteDataReader sqlitrd = sqlitecmd.ExecuteReader())
                        {
                            if (sqlitrd.HasRows)
                            {
                                while (sqlitrd.Read())
                                {
                                    pTiposMetodosPago[contador] = sqlitrd["codigoAlfanumerico"].ToString();
                                    contador++;
                                }
                            }
                        }
                    }
                }
            }
            MetodoPago1 = pTiposMetodosPago[0];
            MetodoPago2 = pTiposMetodosPago[1];
            MetodoPago3 = pTiposMetodosPago[2];
            MetodoPago4 = pTiposMetodosPago[3];
        }

        public void SetTiposCuentas()
        {
            Connection con = new Connection();
            SQLiteConnection sqlitecon = con.sQliteOpenConnection();
            string query = "SELECT codigoAlfanumerico FROM TiposCuentas";
            int contador = 0;
            string[] pTiposCuentas = new string[4];
            if (sqlitecon != null)
            {
                using (sqlitecon)
                {
                    using (SQLiteCommand sqlitecmd = new SQLiteCommand(query, sqlitecon))
                    {
                        using (SQLiteDataReader sqlitrd = sqlitecmd.ExecuteReader())
                        {
                            if (sqlitrd.HasRows)
                            {
                                while (sqlitrd.Read())
                                {
                                    pTiposCuentas[contador] = sqlitrd["codigoAlfanumerico"].ToString();
                                    contador++;
                                }
                            }
                        }
                    }
                }
            }
            TipoCuenta1 = pTiposCuentas[0];
            TipoCuenta2 = pTiposCuentas[1];
            TipoCuenta3 = pTiposCuentas[2];
            TipoCuenta4 = pTiposCuentas[3];
        }

        public string CargarNumeroCuenta(DevExpress.XtraEditors.TextEdit ptxtNroCuenta, string prutEmpresa)
        {
            Connection sqlite = new Connection();
            SQLiteConnection sqlcon = sqlite.sQliteOpenConnection();
            string querysql = "SELECT numeroCuenta FROM datosEmpresa WHERE rutEmpresa = @rutEmpresa";
            string numeroCuenta = "";
            try
            {
                if (sqlcon != null)
                {
                    using (sqlcon)
                    {
                        SQLiteCommand sqlcmd = new SQLiteCommand(querysql, sqlcon);
                        sqlcmd.Parameters.Add(new SQLiteParameter("@rutEmpresa", prutEmpresa));
                        numeroCuenta = sqlcmd.ExecuteScalar().ToString();
                    }
                }
                return numeroCuenta;

            }
            catch (Exception)
            {
                //ERROR AL CARGAR EL CBX
                return "";
            }
        }

        public string UpdateNumeroCuenta(string pNumeroCuenta)
        {
            Connection con = new Connection();
            SQLiteConnection sqlitecon = con.sQliteOpenConnection();
            string query = "UPDATE datosEmpresa SET numeroCuenta = @numeroCuenta";
            try
            {
                if (sqlitecon != null)
                {
                    using (sqlitecon)
                    {
                        using (SQLiteCommand sqlitecmd = new SQLiteCommand(query, sqlitecon))
                        {
                            sqlitecmd.Parameters.Add(new SQLiteParameter("@numeroCuenta", pNumeroCuenta));
                            sqlitecmd.ExecuteNonQuery();
                            return "1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            return "";
        }

        public string CargarRutaArchivo(DevExpress.XtraEditors.TextEdit ptxtRuta, string prutEmpresa)
        {
            Connection con = new Connection();
            SQLiteConnection sqlitecon = con.sQliteOpenConnection();
            string query = "SELECT rutaArchivo FROM datosEmpresa WHERE rutEmpresa = @rutEmpresa";
            if (sqlitecon != null)
            {
                using (sqlitecon)
                {
                    SQLiteCommand sqlitecmd = new SQLiteCommand(query, sqlitecon);
                    sqlitecmd.Parameters.Add(new SQLiteParameter("@rutEmpresa", prutEmpresa));
                    return sqlitecmd.ExecuteScalar().ToString();

                }
            }
            return "Seleccione Ruta";
        }

        public string UpdateRutaArchivo(string pRutaArchivo)
        {
            Connection con = new Connection();
            SQLiteConnection sqlitecon = con.sQliteOpenConnection();
            string query = "UPDATE datosEmpresa SET rutaArchivo = @rutaArchivo";
            if (sqlitecon != null)
            {
                using (sqlitecon)
                {
                    SQLiteCommand sqlitecmd = new SQLiteCommand(query, sqlitecon);
                    sqlitecmd.Parameters.Add(new SQLiteParameter("@rutaArchivo", pRutaArchivo));
                    sqlitecmd.ExecuteNonQuery();
                    return "1";
                }
            }
            return "No se pudo actualizar la ruta";
        }

        public void InsertEmpresa(string pRutEmpresa)
        {
            Connection con = new Connection();
            SQLiteConnection sqlitecon = con.sQliteOpenConnection();
            string query = "INSERT INTO datosEmpresa (rutEmpresa) VALUES (@rutEmpresa)";
            if (sqlitecon != null)
            {
                using (sqlitecon)
                {
                    SQLiteCommand sqlitecmd = new SQLiteCommand(query, sqlitecon);
                    sqlitecmd.Parameters.Add(new SQLiteParameter("@rutEmpresa", pRutEmpresa));
                    sqlitecmd.ExecuteNonQuery();
                }
            }
        }

        public void SelectEmpresa(string pRutEmpresa)
        {
            Connection con = new Connection();
            SQLiteConnection sqlitecon = con.sQliteOpenConnection();
            string query = "SELECT rutEmpresa FROM datosEmpresa WHERE rutEmpresa = @rutEmpresa";
            string dato = "";
            if (sqlitecon != null)
            {
                using (sqlitecon)
                {
                    try
                    {
                        SQLiteCommand sqlitecmd = new SQLiteCommand(query, sqlitecon);
                        sqlitecmd.Parameters.Add(new SQLiteParameter("@rutEmpresa", pRutEmpresa));
                        dato = sqlitecmd.ExecuteScalar().ToString();
                    }
                    catch (Exception)
                    {

                        InsertEmpresa(pRutEmpresa);
                    }

                }
            }
        }

    }
    /// <summary>
    /// Clase para LOS DATOS DE LOS REPORTES.
    /// </summary>
    public class DatosReporte
    {
        /// <summary>
        /// CUENTA LOS BANCOS EN LA BASE DE DATOS
        /// </summary>
        /// <param name="StringConnection"></param>
        /// <returns></returns>
        public int CountBanco(string StringConnection)
        {
            Connection con = new Connection(StringConnection);
            SqlConnection sqlcon = con.OpenConnection();
            string query = "SELECT COUNT(id) FROM banco";
            int cantidadBanco = 0;
            if (sqlcon != null)
            {
                using (sqlcon)
                {
                    SqlCommand sqlcmd = new SqlCommand(query, sqlcon);
                    cantidadBanco = int.Parse(sqlcmd.ExecuteScalar().ToString());
                    return cantidadBanco;
                }
            }
            return 0;
        }

        public int[] GetBancos(string StringConnection)
        {
            Connection con = new Connection(StringConnection);
            SqlConnection sqlcon = con.OpenConnection();
            string query = "SELECT id FROM banco";
            int[] idBanco = new int[CountBanco(StringConnection)];
            int contador = 0;
            if (sqlcon != null)
            {
                using (sqlcon)
                {
                    SqlCommand sqlcmd = new SqlCommand(query, sqlcon);
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    while (sqlrdr.Read())
                    {
                        idBanco[contador] = int.Parse(sqlrdr["id"].ToString());
                        contador++;
                    }
                }
            }
            return idBanco;
        }

        /// <summary>
        /// Agrega puntos y guion a una cadena de entrada que representa un rut.
        /// <para>Ej: 174536007 -> 17.453.600-7</para>
        /// </summary>
        /// <param name="pRut">Cadena a evaluar.</param>
        /// <returns></returns>
        public string fFormatearRut2(string pRut)
        {
            if (pRut == "") return "";
            //RECORRER CADENA Y DEJAR EN FORMATO XXX.XXX.XXX-X
            //CADENA TIENE COMO MAXIMO 12 CARACTERES (INCLUIDO PUNTOS Y GUION)
            //123 456 789 123
            //REGLAS
            /*
             *@1 ANTES DEL ULTIMO ELEMENTO SIEMPRE VA UN GUION
             *@2 CADA 3 ELEMENTOS VA UN PUNTO DESDE DERECHA A IZQUIERDA Y SIN CONTAR EL GUION             
             */
            int original = pRut.Length;
            string digito = pRut.Substring(original - 1, 1);
            //EXTRAEMOS CADENA SIN EL ULTIMO DIGITO (REPRESENTA A EL DIGITO VERIFICADOR)
            string subcadena = pRut.Substring(0, pRut.Length - 1);
            string[] invertir = new string[] { };
            //guardamos el largo de la subcadena
            int largo = subcadena.Length;
            string cad = "";
            string cadFinal = "";
            string res = "";
            int multiplo = 0;
            //RECORREMOS CADENA DE FORMA INVERSA
            for (int i = largo - 1; i >= 0; i--)
            {
                cad = cad + subcadena[i];
            }

            //RECORREMOS NUEVAMENTE LA CADENA PARA ENCONTRAR EL MAYOR MULTIPLO DE 3
            for (int i = 0; i < cad.Length; i++)
            {
                //SI ES MULTIPLO DE 3
                if (i % 3 == 0)
                {
                    multiplo = i;
                }
            }

            for (int i = 0; i < cad.Length; i++)
            {
                if (i % 3 == 0)
                {
                    if (i < multiplo)
                    {
                        cadFinal = cadFinal + cad.Substring(i, 3) + ".";
                    }
                    else
                    {

                        cadFinal = cadFinal + cad.Substring(i, (cad.Length) - i);
                    }
                }
            }

            for (int i = (cadFinal.Length) - 1; i >= 0; i--)
            {
                res = res + cadFinal[i];
            }
            //agregamos digito verificador 
            res = res + "-" + digito;

            return res;
        }

        /// <summary>
        /// Muestra el reporte en una ventana.
        /// </summary>
        /// <param name="report"></param>
        public void ShowDocument(XtraReport report)
        {
            ReportPrintTool print = new ReportPrintTool(report);

            UserLookAndFeel lookAndFeel = new UserLookAndFeel(this);
            lookAndFeel.UseDefaultLookAndFeel = false;
            lookAndFeel.SkinName = "Office 2016 Black";
            //print.Report.Watermark.Text = "TEST";

            //LOS SIGUIENTES ELEMENTOS REPRESENTAN LOS BOTONES DEL FORMULARIO DE PREVISUALIZACION DE UN REPORTE
            //NO LOS MOSTRAREMOS USANDO LA PROPIEDAD COMMANDVISIBILITY EN NONE (EQUIVALENTE A FALSE) 
            PrintingSystemCommand[] commands = new PrintingSystemCommand[] {
                PrintingSystemCommand.ClosePreview,
                PrintingSystemCommand.Find,
                PrintingSystemCommand.Save,
                PrintingSystemCommand.Open,
                PrintingSystemCommand.FillBackground,
                PrintingSystemCommand.SendFile,
                PrintingSystemCommand.ExportFile,
                PrintingSystemCommand.Watermark
            };
            print.PrintingSystem.SetCommandVisibility(commands, DevExpress.XtraPrinting.CommandVisibility.None);

            //print.ShowPreviewDialog(lookAndFeel);
            print.ShowRibbonPreviewDialog(lookAndFeel);
        }

        /*OBTENER IMAGEN DESDE BD*/
        /// <summary>
        /// Devuelve el logo de la empresa almacenado en bd.
        /// </summary>
        /// <returns></returns>
        public static Image GetLogoFromBd(string pStringConecction)
        {
            Image img = null;
            Connection con = new Connection(pStringConecction);
            SqlCommand cmd;
            SqlConnection cn;
            string sql = "SELECT logo FROM empresa";
            SqlDataReader rd;
            try
            {
                cn = con.OpenConnection();
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
                                    if (rd["logo"] as byte[] != null)
                                        img = GetImageFromStream((byte[])rd["logo"]);
                                }
                            }
                        }
                        cmd.Dispose();
                        rd.Close();
                    }
                }

            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return img;
        }

        /// Genera la correspondiente imagen desde su representacion en binario.
        /// </summary>
        /// <param name="img">Imagen.</param>
        /// <returns></returns>
        public static Image GetImageFromStream(byte[] img)
        {
            MemoryStream memo = new MemoryStream(img);
            Image imagenFinal = Image.FromStream(memo);

            return imagenFinal;
        }

        /// <summary>
        /// Genera una imagen en blanco en el caso de que no se haya ingresado logo empresa.
        /// </summary>
        /// <param name="picture">Control XRPictureBox reporte.</param>
        public static void SinImagen(XRPictureBox picture)
        {
            //int dato = int.Parse("k");
            picture.Image = BancoItau.Properties.Resources.logo_vacio;
            picture.Sizing = DevExpress.XtraPrinting.ImageSizeMode.StretchImage;
        }
    }

    /// <summary>
    /// CLASE AUXILIAR QUE REPRESENTA LOS CAMPOS QUE TENDRÁ EL ARCHIVO FINAL
    /// REPRESENTA CADA COLUMNA DEL ARCHIVO FINAL
    /// </summary>
    public class Cuerpo
    {
        /// <summary>
        /// RUT DEL TRABAJADOR SIN PUNTOS NI GUION
        /// </summary>
        public string Rut { get; set; }
        /// <summary>
        /// NOMBRE COMPLETO DEL TRABAJADOR
        /// </summary>
        public string Nombre { get; set; }
        /// <summary>
        /// NUERO DE BANCO CORRESPONDIENTE A LA BASE DE DATOS
        /// </summary>
        public int NroBanco { get; set; }
        /// <summary>
        /// NOMBRE DE BANCO CORRESPONDIENTE A BASE DE DATOS
        /// </summary>
        public string NombreBanco { get; set; }
        /// <summary>
        /// EMAIL DEL BENEFICIARIO
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// CODIGO DE METODO BANCO DE TABLA SQLITE
        /// </summary>
        public string MetodoPago { get; set; }
        /// <summary>
        /// CODIGO DEL BANCO, CORRESPONDE AL CODIGO SBIF ENTREGADO POR LA SUPERINTENDENCIA DE BANCOS. CODIGO DE BANCO DE DESTINO
        /// </summary>
        public string CodigoBanco { get; set; }
        ///<summary>
        ///TIPO DE CUENTA OBLIGATORIO SOLO SI EL METODO PAGO ES CAT_CSH_TRANSFER
        ///</summary>
        public string TipoDeCuenta { get; set; }
        /// <summary>
        /// NUMERO DE CUENTA DEL BENEFICIARIO OBLIGATORIO SOLO SI EL METODO PAGO ES CAT_CSH_TRANSFER
        /// </summary>
        public string NroCuenta { get; set; }
        ///<summary>
        ///FECHA DE PAGO FORMATO dd/mm/aaaa
        ///</summary>
        public string FechaDePago { get; set; }
        ///<summary>
        ///REFERENCIA 1 DESCONTINUADO
        ///</summary>
        public string Referencia1 { get; set; }
        ///<summary>
        ///REFERENCIA 2 DESCONTINUADO
        ///</summary>
        public string Referencia2 { get; set; }
        /// <summary>
        /// MONTO A PAGAR
        /// </summary>
        public double Monto { get; set; }
        ///<summary>
        ///TIPO DE CUENTA DEL MEDIO RESPALDO
        ///</summary>
        public string MedioRespaldo { get; set; }
        ///<summary>
        ///NUMERO DE CUENTA CORRIENTE DE LA EMPRESA
        ///</summary>
        public string NroMedioResp { get; set; }
        ///<summary>
        ///CODIGO DE SUCURSAL. VALOR FIJO DE 001 CUANDO EL BANCO ES ITAU. SINO ES ITAU VA EN BLANCO
        ///</summary>
        public string CodigoSucursal { get; set; }
        ///<summary>
        ///REFERENCIA DE PAGO PARA EL CLIENTE SOLO SI ES CHEQUE PONER EL DATO
        ///</summary>
        public string ReferenciaCliente { get; set; }
        ///<summary>
        ///DESCRIPCION OPCIONAL DEL PAGO
        ///</summary>
        public string GlosaDelPago { get; set; }
    }

    /// <summary>
    /// REPRESENTA EL HEADER DEL ARCHIVO, CON DATOS DE LA EMPRESA
    /// </summary>
    public class Header
    {

        /// <summary>
        /// Rut de la empresa, sin puntos ni guión.
        /// </summary>
        public string RutEmpresa { get; set; }
        /// <summary>
        /// Código de la empresa
        /// </summary>
        public string Codigo { get; set; }
        /// <summary>
        /// Contract account, POR DEFECTO ES 'CAT_CSH_CONTRACT_ACCOUNT'
        /// </summary>
        public string Account { get; set; } = "CAT_CSH_CONTRACT_ACCOUNT";
    }

    /// <summary>
    /// Clase para generación de listado de bancos. 
    /// </summary>
    public class ComboOption
    {
        /// <summary>
        /// Código opcion.
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// Nombre banco.
        /// </summary>        
        public string Entidad { get; set; }

        /// <summary>
        /// Abreviatura del banco
        /// </summary>
        // public string Alias { get; set; }

        public ComboOption()
        {
            /*Constructor...*/
        }

        /// <summary>
        /// Genera un listado de bancos
        /// </summary>
        /// <returns></returns>
        public List<ComboOption> GetList()
        {
            List<ComboOption> Listado = new List<ComboOption>();

            //LISTADO DE BANCOS            
            Listado.Add(new ComboOption() { Code = 1, Entidad = "Banco de Chile" }); 
            Listado.Add(new ComboOption() { Code = 2, Entidad = "Banco Estado" });
            Listado.Add(new ComboOption() { Code = 3, Entidad = "Banco Scotiabank Chile" });
            Listado.Add(new ComboOption() { Code = 4, Entidad = "Banco Itaú Corpbanca" });
            Listado.Add(new ComboOption() { Code = 5, Entidad = "Banco de creditos e inversiones BCI" });
            Listado.Add(new ComboOption() { Code = 6, Entidad = "Banco BICE" });
            Listado.Add(new ComboOption() { Code = 7, Entidad = "HSBC Bank Chile" });
            Listado.Add(new ComboOption() { Code = 8, Entidad = "Banco Santander Chile" });
            Listado.Add(new ComboOption() { Code = 9, Entidad = "Banco Security" });
            Listado.Add(new ComboOption() { Code = 10, Entidad = "Banco Falabella" });
            Listado.Add(new ComboOption() { Code = 11, Entidad = "Banco Ripley" });
            Listado.Add(new ComboOption() { Code = 12, Entidad = "Banco Consorcio" });
            Listado.Add(new ComboOption() { Code = 13, Entidad = "Rabobank Chile" });
            Listado.Add(new ComboOption() { Code = 14, Entidad = "Banco Penta" });
            Listado.Add(new ComboOption() { Code = 15, Entidad = "Banco BBVA" });
            Listado.Add(new ComboOption() { Code = 16, Entidad = "Banco BTG Pactual Chile" });
            Listado.Add(new ComboOption() { Code = 17, Entidad = "Banco Do Brasil" });
            Listado.Add(new ComboOption() { Code = 18, Entidad = "JP Morgan Chase Bank" });
            Listado.Add(new ComboOption() { Code = 19, Entidad = "Banco de la Nación Argentina" });
            Listado.Add(new ComboOption() { Code = 20, Entidad = "The Bank of Tokyo - Mitsubishi UFJ" });
            Listado.Add(new ComboOption() { Code = 21, Entidad = "China Construction Bank" });
            Listado.Add(new ComboOption() { Code = 22, Entidad = "BCI - Sucursal Miami" });
            Listado.Add(new ComboOption() { Code = 23, Entidad = "Banco Estado Sucursal Extranjero" });
            Listado.Add(new ComboOption() { Code = 24, Entidad = "Itaú Corpbanca New York Branch" });

            return Listado;
        }
    }
}
