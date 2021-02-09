using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;
using System.Data;
using System.IO;
using System.Runtime.InteropServices;
using DevExpress.Spreadsheet;
using DevExpress.Docs;
using DevExpress.Office;
using DevExpress.XtraGrid;
using System.Windows.Forms;
using DevExpress.Spreadsheet.Export;
using System.Data.SqlClient;

namespace Labour
{
    class FileExcel
    {
        public FileExcel()
        {
            //CONSTRUCTOR...
        }

        //CREAR UN ARCHIVO EXCEL
        //RECIBE COMO PARAMETRO DE ENTRADA UN DATATABLE Y LA RUTA DEL ARCHIVO
        public static bool CrearArchivoExcel(DataTable tabla, string FilePath)
        {
            if (FilePath != "" && tabla.Rows.Count > 0)
            {
                //GENERAMOS APLICACION EXCEL
                Excel._Application Programa = new Excel.Application();
                //OBTENER LA COLECCION DE LIBROS QUE TIENE EL ARCHIVO
                Excel.Workbooks Libros = Programa.Workbooks;
                //CREAMOS UN NUEVO LIBRO
                Libros.Add(Type.Missing);

                //OBTENER EL LIBRO ACTIVO
                Excel._Workbook LibroActivo = Libros[1];

                //OBTENER LA HOJA ACTIVA?
                Excel._Worksheet HojaActiva = LibroActivo.Worksheets[1];

                //OBTENER LAS COLUMNAS DEL DATATABLE
                DataColumnCollection columnas = tabla.Columns;

                //A TRAVÉS DE LAS COLUMNAS DE DATATABLE CREAMOS LOS HEADER PARA EL EXCEL
                for (int columna = 0; columna < columnas.Count; columna++)
                {
                    HojaActiva.Cells[1, columna + 1].Value = columnas[columna].ToString();
                }

                //RECORREMOS TODOS LE DATASET Y LLENAMOS EXCEL
                for (int fila = 1; fila <= tabla.Rows.Count; fila++)
                {
                    for (int columna = 1; columna <= tabla.Columns.Count; columna++)
                    {
                        //AGREGAMOS...
                        HojaActiva.Cells[fila + 1, columna].Value = tabla.Rows[fila - 1][columna - 1];
                    }
                }

                //PARA OBTENER EL RANGO USADO (QUE CONTIENE DATA)
                Excel.Range rango = HojaActiva.UsedRange;

                Excel.Borders borde = rango.Borders;
                borde.LineStyle = Excel.XlLineStyle.xlContinuous;

                //AJUSTAR COLUMNAS
                rango.Columns.AutoFit();

                LibroActivo.SaveCopyAs(FilePath);
                LibroActivo.Saved = true;

                //LIBERAMOS TODOS LOS OBJETOS COM...
                LimpiaTodo(LibroActivo, Libros, HojaActiva, Programa, rango);
                LimpiarBasura(borde);

                GC.Collect();
                GC.WaitForPendingFinalizers();

                //PREGUNTAMOS SI EL ARCHIVO EXISTE
                if (File.Exists(FilePath))
                {
                    //EL ARCHIVO SE CREO CORRECTAMENTE...
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Abre diálogo de directorios con un nombre pre establecido para el archivo
        /// </summary>
        /// <param name="tabla"></param>
        /// <param name="pNameArchivo"></param>
        /// <returns></returns>
        public static string OpenDialogExcel(DataTable tabla, int pPeriodo, string pPrefijoNombreArchivo)
        {

            //OPEN DIALOG PARA LA RUTA
            SaveFileDialog save = new SaveFileDialog();
            save.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            save.Filter = "Archivos Excel(*.xlsx)|*.xlsx";
            save.FileName = $"{pPrefijoNombreArchivo}_{fnSistema.FechaFormatoSoloMes(fnSistema.FechaPeriodo(pPeriodo))}";
            DialogResult dialogo = save.ShowDialog();
            if (dialogo == DialogResult.OK)
            {

                return save.FileName;
            }

            return null;
        }

        //CREAR ARCHIVO EXCEL USANDO DEVEXPRESS
        public static bool CrearArchivoExcelDev(DataTable tabla, string PathFile)
        {
            if (tabla.Rows.Count > 0)
            {
                try
                {
                    //CREAMOS LIBRO
                    Workbook Libro = new Workbook();

                    //OBTENER HOJA DESDE LIBRO  
                    Worksheet Hoja = Libro.Worksheets[0];
                    Hoja.Name = "Data";

                    //IMPORTAR A EXCEL DESDE DATATABLE
                    Hoja.Import(tabla, true, 0, 0);

                    //int[,] ar = new int[3, 3];
                    //Hoja.Import(ar, 0, 0, false);

                    //OBTENER EL RANGO USADO
                    Range rango = Hoja.GetUsedRange();

                    //AGREGAMOS TABLA A HOJA
                    Table table = Hoja.Tables.Add(rango, true);
                    table.Style = Libro.TableStyles[BuiltInTableStyleId.TableStyleLight10];
                    table.HeaderRowRange.Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
                    ColumnCollection columnas = Hoja.Columns;
                    columnas.AutoFit(0, rango.ColumnCount);

                    //GUARDAMOS LIBRO
                    Libro.SaveDocument(PathFile, DocumentFormat.Xlsx);
                    Libro.Dispose();

                    if (File.Exists(PathFile))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        //CREAR ARCHIVO EXCEL USANDO DEVEXPRESS
        public static bool CrearArchivoExcelDevArray(string[,] tabla, string PathFile)
        {
            if (tabla.Length > 0)
            {
                try
                {
                    //CREAMOS LIBRO
                    Workbook Libro = new Workbook();

                    //OBTENER HOJA DESDE LIBRO  
                    Worksheet Hoja = Libro.Worksheets[0];
                    Hoja.Name = "Data";

                    //IMPORTAR A EXCEL DESDE DATATABLE
                    //Hoja.Import(tabla, true, 0, 0);        
                    Hoja.Import(tabla, 0, 0);

                    //OBTENER EL RANGO USADO
                    Range rango = Hoja.GetUsedRange();

                    //AGREGAMOS TABLA A HOJA
                    Table table = Hoja.Tables.Add(rango, false);
                    table.Style = Libro.TableStyles[BuiltInTableStyleId.TableStyleLight10];
                    table.HeaderRowRange.Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
                    ColumnCollection columnas = Hoja.Columns;
                    columnas.AutoFit(0, rango.ColumnCount);

                    //GUARDAMOS LIBRO
                    Libro.SaveDocument(PathFile, DocumentFormat.Xlsx);
                    Libro.Dispose();

                    if (File.Exists(PathFile))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static void CrearArchivoPlantilla(DataTable tabla, string PathFile)
        {
            DataTable TablaNacion = new DataTable();
            DataTable TablaECivil = new DataTable();
            DataTable TablaCiudad = new DataTable();
            DataTable TablaArea = new DataTable();
            DataTable TablaAfp = new DataTable();
            DataTable TablaSalud = new DataTable();
            DataTable TablaCentro = new DataTable();
            DataTable TablaCargo = new DataTable();
            DataTable TablaRegimen = new DataTable();
            DataTable TablaCaja = new DataTable();
            DataTable TablaBanco = new DataTable();
            DataTable TablaFormaPago = new DataTable();
            DataTable TablaTipoCuenta = new DataTable();
            DataTable TablaClase = new DataTable();
            DataTable TablaCausal = new DataTable();
            DataTable TablaSucursal = new DataTable();
            DataTable TablaEscolaridad = new DataTable();
            DataTable TablaHorario = new DataTable();
            DataTable TablaJubilado = new DataTable();
            DataTable TablaTipoContrato = new DataTable();
            DataTable TablaSexo = new DataTable();
            DataTable TablaJornada = new DataTable();
            DataTable TablaRegimenSal = new DataTable();
            DataTable TablaSindicato = new DataTable();
            DataTable TablaComunas = new DataTable();

            try
            {
                //CREAMOS LIBRO
                Workbook Libro = new Workbook();              

                //OBTENER HOJA DESDE LIBRO  
                Worksheet Hoja = Libro.Worksheets[0];
                Hoja.Name = "Data";            

                #region "DATA"
                //AGREGAR UNA NUEVA HOJA
                Libro.Worksheets.Add();
                Libro.Worksheets.Add();
                Libro.Worksheets.Add();
                Libro.Worksheets.Add();
                Libro.Worksheets.Add();
                Libro.Worksheets.Add();
                Libro.Worksheets.Add();
                Libro.Worksheets.Add();
                Libro.Worksheets.Add();
                Libro.Worksheets.Add();
                Libro.Worksheets.Add();
                Libro.Worksheets.Add();
                Libro.Worksheets.Add();
                Libro.Worksheets.Add();
                Libro.Worksheets.Add();
                Libro.Worksheets.Add();
                Libro.Worksheets.Add();
                Libro.Worksheets.Add();
                Libro.Worksheets.Add();
                Libro.Worksheets.Add();
                Libro.Worksheets.Add();
                Libro.Worksheets.Add();
                Libro.Worksheets.Add();
                Libro.Worksheets.Add();
                Libro.Worksheets.Add();
                Libro.Worksheets.Add();

                Libro.Worksheets[1].Name = "Nacionalidades";
                Libro.Worksheets[2].Name = "Estado Civil";
                Libro.Worksheets[3].Name = "Ciudades";
                Libro.Worksheets[4].Name = "Area";
                Libro.Worksheets[5].Name = "Afp";
                Libro.Worksheets[6].Name = "Instituciones de Salud";
                Libro.Worksheets[7].Name = "Centro Costo";
                Libro.Worksheets[8].Name = "Cargo";
                Libro.Worksheets[9].Name = "Regimen";
                Libro.Worksheets[10].Name = "Caja Prevision";
                Libro.Worksheets[11].Name = "Banco";
                Libro.Worksheets[12].Name = "Forma pago";
                Libro.Worksheets[13].Name = "Tipo cuenta";
                Libro.Worksheets[14].Name = "Clase";
                Libro.Worksheets[15].Name = "Causal termino";
                Libro.Worksheets[16].Name = "Sucursal";
                Libro.Worksheets[17].Name = "Escolaridad";
                Libro.Worksheets[18].Name = "Horario";
                Libro.Worksheets[19].Name = "Jubilado";
                Libro.Worksheets[20].Name = "Tipo Contrato";
                Libro.Worksheets[21].Name = "Sexo";
                Libro.Worksheets[22].Name = "Jornada Laboral";
                Libro.Worksheets[23].Name = "Regimen Salarial";
                Libro.Worksheets[24].Name = "Sindicatos";
                Libro.Worksheets[25].Name = "Comunas";
                Libro.Worksheets[26].Name = "Suspension Laboral";

                //DATO PARA NACIONALIDADES
                Nacionalidad nac = new Nacionalidad();
                TablaNacion = nac.GetInfo();

                EstadoCivil estado = new EstadoCivil();
                TablaECivil = estado.GetInfo();

                Ciudad Ci = new Ciudad();
                TablaCiudad = Ci.GetInfo();

                Area ar = new Area();
                TablaArea = ar.GetInfo();

                AseguradoraFondoPension Afp = new AseguradoraFondoPension();
                TablaAfp = Afp.GetInfo();

                Isapre isa = new Isapre();
                TablaSalud = isa.GetInfo();

                CentroCosto costo = new CentroCosto();
                TablaCentro = costo.GetInfo();

                Cargo carg = new Cargo();
                TablaCargo = carg.GetInfo();

                Regimen reg = new Regimen();
                TablaRegimen = reg.GetInfo();

                CajaPrevision caja = new CajaPrevision();
                TablaCaja = caja.GetInfo();

                Banco bank = new Banco();
                TablaBanco = bank.GetInfo();

                FormaPago form = new FormaPago();
                TablaFormaPago = form.GetInfo();

                TipoCuenta tipo = new TipoCuenta();
                TablaTipoCuenta = tipo.GetInfo();

                ClaseRemuneracion cl = new ClaseRemuneracion();
                TablaClase = cl.GetInfo();

                Causal cas = new Causal();
                TablaCausal = cas.GetInfoPlantilla();

                Sucursal suc = new Sucursal();
                TablaSucursal = suc.GetInfo();

                Escolaridad esc = new Escolaridad();
                TablaEscolaridad = esc.Getinfo();

                Horario hor = new Horario();
                TablaHorario = hor.GetInfo();

                Jubilado Jub = new Jubilado();
                TablaJubilado = Jub.GetInfo();

                TipoContrato con = new TipoContrato();
                TablaTipoContrato = con.GetInfo();

                Sexo sex = new Sexo();
                TablaSexo = sex.GetInfo();

                JornadaLaboral jor = new JornadaLaboral();
                TablaJornada = jor.GetInfo();

                RegimenSalarial sal = new RegimenSalarial();
                TablaRegimenSal = sal.GetInfo();

                Sindicato sin = new Sindicato();
                TablaSindicato = sin.GetDataSource().Tables[0];

                TablaComunas = fnSistema.GetComunas();

                DataTable Suspension = new DataTable();
                Suspension.Columns.Add("id", typeof(int));
                Suspension.Columns.Add("Nombre", typeof(string));

                DataRow r1 = Suspension.NewRow();
                r1["id"] = 0;
                r1["Nombre"] = "No Aplica";
                DataRow r2 = Suspension.NewRow();
                r2["id"] = 13;
                r2["Nombre"] = "Suspension por acto de autoridad";
                DataRow r3 = Suspension.NewRow();
                r3["id"] = 14;
                r3["Nombre"] = "Suspension por pacto";
                DataRow r4 = Suspension.NewRow();
                r4["id"] = 15;
                r4["Nombre"] = "Reduccion de jornada laboral";

                //add rows to datatable
                Suspension.Rows.Add(r1);
                Suspension.Rows.Add(r2);
                Suspension.Rows.Add(r3);
                Suspension.Rows.Add(r4);


                if (TablaNacion.Rows.Count > 0)
                {
                    Libro.Worksheets[1].Import(TablaNacion, true, 0, 0);
                    Range rango1 = Libro.Worksheets[1].GetUsedRange();
                    ColumnCollection col = Libro.Worksheets[1].Columns;
                    col.AutoFit(0, rango1.ColumnCount);
                }

                if (TablaECivil.Rows.Count > 0)
                {
                    Libro.Worksheets[2].Import(TablaECivil, true, 0, 0);
                    Range rango1 = Libro.Worksheets[2].GetUsedRange();
                    ColumnCollection col = Libro.Worksheets[2].Columns;
                    col.AutoFit(0, rango1.ColumnCount);
                }

                if (TablaCiudad.Rows.Count > 0)
                {
                    Libro.Worksheets[3].Import(TablaCiudad, true, 0, 0);
                    Range rango1 = Libro.Worksheets[3].GetUsedRange();
                    ColumnCollection col = Libro.Worksheets[3].Columns;
                    col.AutoFit(0, rango1.ColumnCount);
                }

                if (TablaArea.Rows.Count > 0)
                {
                    Libro.Worksheets[4].Import(TablaArea, true, 0, 0);
                    Range rango1 = Libro.Worksheets[4].GetUsedRange();
                    ColumnCollection col = Libro.Worksheets[4].Columns;
                    col.AutoFit(0, rango1.ColumnCount);
                }

                if (TablaAfp.Rows.Count > 0)
                {
                    Libro.Worksheets[5].Import(TablaAfp, true, 0, 0);
                    Range rango1 = Libro.Worksheets[5].GetUsedRange();
                    ColumnCollection col = Libro.Worksheets[5].Columns;
                    col.AutoFit(0, rango1.ColumnCount);
                }

                if (TablaSalud.Rows.Count > 0)
                {
                    Libro.Worksheets[6].Import(TablaSalud, true, 0, 0);
                    Range rango1 = Libro.Worksheets[6].GetUsedRange();
                    ColumnCollection col = Libro.Worksheets[6].Columns;
                    col.AutoFit(0, rango1.ColumnCount);
                }

                if (TablaCentro.Rows.Count > 0)
                {
                    Libro.Worksheets[7].Import(TablaCentro, true, 0, 0);
                    Range rango1 = Libro.Worksheets[7].GetUsedRange();
                    ColumnCollection col = Libro.Worksheets[7].Columns;
                    col.AutoFit(0, rango1.ColumnCount);
                }

                if (TablaCargo.Rows.Count > 0)
                {
                    Libro.Worksheets[8].Import(TablaCargo, true, 0, 0);
                    Range rango1 = Libro.Worksheets[8].GetUsedRange();
                    ColumnCollection col = Libro.Worksheets[8].Columns;
                    col.AutoFit(0, rango1.ColumnCount);
                }

                if (TablaRegimen.Rows.Count > 0)
                {
                    Libro.Worksheets[9].Import(TablaRegimen, true, 0, 0);
                    Range rango1 = Libro.Worksheets[9].GetUsedRange();
                    ColumnCollection col = Libro.Worksheets[9].Columns;
                    col.AutoFit(0, rango1.ColumnCount);
                }

                if (TablaCaja.Rows.Count > 0)
                {
                    Libro.Worksheets[10].Import(TablaCaja, true, 0, 0);
                    Range rango1 = Libro.Worksheets[10].GetUsedRange();
                    ColumnCollection col = Libro.Worksheets[10].Columns;
                    col.AutoFit(0, rango1.ColumnCount);
                }

                if (TablaBanco.Rows.Count > 0)
                {
                    Libro.Worksheets[11].Import(TablaBanco, true, 0, 0);
                    Range rango1 = Libro.Worksheets[11].GetUsedRange();
                    ColumnCollection col = Libro.Worksheets[11].Columns;
                    col.AutoFit(0, rango1.ColumnCount);
                }

                if (TablaFormaPago.Rows.Count > 0)
                {
                    Libro.Worksheets[12].Import(TablaFormaPago, true, 0, 0);
                    Range rango1 = Libro.Worksheets[12].GetUsedRange();
                    ColumnCollection col = Libro.Worksheets[12].Columns;
                    col.AutoFit(0, rango1.ColumnCount);
                }

                if (TablaTipoCuenta.Rows.Count > 0)
                {
                    Libro.Worksheets[13].Import(TablaTipoCuenta, true, 0, 0);
                    Range rango1 = Libro.Worksheets[13].GetUsedRange();
                    ColumnCollection col = Libro.Worksheets[13].Columns;
                    col.AutoFit(0, rango1.ColumnCount);
                }

                if (TablaClase.Rows.Count > 0)
                {
                    Libro.Worksheets[14].Import(TablaClase, true, 0, 0);
                    Range rango1 = Libro.Worksheets[14].GetUsedRange();
                    ColumnCollection col = Libro.Worksheets[14].Columns;
                    col.AutoFit(0, rango1.ColumnCount);
                }

                if (TablaCausal.Rows.Count > 0)
                {
                    Libro.Worksheets[15].Import(TablaCausal, true, 0, 0);
                    Range rango1 = Libro.Worksheets[15].GetUsedRange();
                    ColumnCollection col = Libro.Worksheets[15].Columns;
                    col.AutoFit(0, rango1.ColumnCount);
                }

                if (TablaSucursal.Rows.Count > 0)
                {
                    Libro.Worksheets[16].Import(TablaSucursal, true, 0, 0);
                    Range rango1 = Libro.Worksheets[16].GetUsedRange();
                    ColumnCollection col = Libro.Worksheets[16].Columns;
                    col.AutoFit(0, rango1.ColumnCount);
                }

                if (TablaEscolaridad.Rows.Count > 0)
                {
                    Libro.Worksheets[17].Import(TablaEscolaridad, true, 0, 0);
                    Range rango1 = Libro.Worksheets[17].GetUsedRange();
                    ColumnCollection col = Libro.Worksheets[17].Columns;
                    col.AutoFit(0, rango1.ColumnCount);
                }

                if (TablaHorario.Rows.Count > 0)
                {
                    Libro.Worksheets[18].Import(TablaHorario, true, 0, 0);
                    Range rango1 = Libro.Worksheets[18].GetUsedRange();
                    ColumnCollection col = Libro.Worksheets[18].Columns;
                    col.AutoFit(0, rango1.ColumnCount);
                }

                if (TablaJubilado.Rows.Count > 0)
                {
                    Libro.Worksheets[19].Import(TablaJubilado, true, 0, 0);
                    Range rango1 = Libro.Worksheets[19].GetUsedRange();
                    ColumnCollection col = Libro.Worksheets[19].Columns;
                    col.AutoFit(0, rango1.ColumnCount);
                }

                if (TablaTipoContrato.Rows.Count > 0)
                {
                    Libro.Worksheets[20].Import(TablaTipoContrato, true, 0, 0);
                    Range rango1 = Libro.Worksheets[20].GetUsedRange();
                    ColumnCollection col = Libro.Worksheets[20].Columns;
                    col.AutoFit(0, rango1.ColumnCount);
                }

                if (TablaSexo.Rows.Count > 0)
                {
                    Libro.Worksheets[21].Import(TablaSexo, true, 0, 0);
                    Range rango1 = Libro.Worksheets[21].GetUsedRange();
                    ColumnCollection col = Libro.Worksheets[21].Columns;
                    col.AutoFit(0, rango1.ColumnCount);
                }

                if (TablaJornada.Rows.Count > 0)
                {
                    Libro.Worksheets[22].Import(TablaJornada, true, 0, 0);
                    Range rango1 = Libro.Worksheets[22].GetUsedRange();
                    ColumnCollection col = Libro.Worksheets[22].Columns;
                    col.AutoFit(0, rango1.ColumnCount);
                }

                if (TablaRegimenSal.Rows.Count > 0)
                {
                    Libro.Worksheets[23].Import(TablaRegimenSal, true, 0, 0);
                    Range rango1 = Libro.Worksheets[23].GetUsedRange();
                    ColumnCollection col = Libro.Worksheets[23].Columns;
                    col.AutoFit(0, rango1.ColumnCount);
                }
                if (TablaSindicato.Rows.Count > 0)
                {
                    Libro.Worksheets[24].Import(TablaSindicato, true, 0, 0);
                    Range rango1 = Libro.Worksheets[24].GetUsedRange();
                    ColumnCollection col = Libro.Worksheets[24].Columns;
                    col.AutoFit(0, rango1.ColumnCount);
                }

                if (TablaComunas.Rows.Count > 0)
                {
                    Libro.Worksheets[25].Import(TablaComunas, true, 0, 0);
                    Range rango1 = Libro.Worksheets[25].GetUsedRange();
                    ColumnCollection col = Libro.Worksheets[25].Columns;
                    col.AutoFit(0, rango1.ColumnCount);
                }

                //Suspension laboral
                Libro.Worksheets[26].Import(Suspension, true, 0, 0);
                Range rango2 = Libro.Worksheets[6].GetUsedRange();
                ColumnCollection col2 = Libro.Worksheets[26].Columns;
                col2.AutoFit(0, rango2.ColumnCount);

                #endregion

                Libro.Worksheets.ActiveWorksheet = Hoja;

                //IMPORTAR A EXCEL DESDE DATATABLE
                Hoja.Import(tabla, true, 0, 0);

                //OBTENER EL RANGO USADO
                Range rango = Hoja.GetUsedRange();

                //AGREGAMOS TABLA A HOJA
                Table table = Hoja.Tables.Add(rango, true);
                table.Style = Libro.TableStyles[BuiltInTableStyleId.TableStyleLight10];
                table.HeaderRowRange.Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
                ColumnCollection columnas = Hoja.Columns;
                columnas.AutoFit(0, rango.ColumnCount);                

                SaveFileDialog save = new SaveFileDialog();
                save.Filter = "Excel Files|*.xlsx;*.xls;*.xlsm";
                save.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                if (save.ShowDialog() == DialogResult.OK)
                {
                    //GUARDAMOS LIBRO
                    Libro.SaveDocument(save.FileName, DocumentFormat.Xlsx);
                    Libro.Dispose();
                }

                if (File.Exists(save.FileName))
                {
                    DialogResult adv = MessageBox.Show("Archivo creado correctamente.\n¿Deseas ver archivo?", "Archivo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (adv == DialogResult.Yes)
                        System.Diagnostics.Process.Start(save.FileName);
                }
                else
                {
                    MessageBox.Show("No se pudo generar documento", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
              
            }
            catch (Exception ex)
            {
              //ERROR..
            }         
         
        }

        //CREAR ARCHIVO EXCEL DESDE GRIDCONTROL
        public static bool CrearArchivoExcelFromGrid(GridControl grilla, string PathFile)
        {
            DataTable tablaFromGrilla = new DataTable();

            if (grilla.DataSource != null && PathFile != "")
            {
                tablaFromGrilla = (DataTable)grilla.DataSource;

                if (CrearArchivoExcelDev(tablaFromGrilla, PathFile))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        //CREAR ARCHIVO EXCEL AGREGANDO FILA SUMA TOTAL 
        public static bool CrearArchivoExcelConSum(DataTable tabla, string PathFile, string ColumName)
        {
            if (tabla.Rows.Count > 0)
            {
                try
                {
                    //CREAMOS LIBRO
                    Workbook Libro = new Workbook();

                    //OBTENER HOJA DESDE LIBRO  
                    Worksheet Hoja = Libro.Worksheets[0];
                    Hoja.Name = "Data";

                    //IMPORTAR A EXCEL DESDE DATATABLE
                    Hoja.Import(tabla, true, 0, 0);

                    //OBTENER EL RANGO USADO
                    Range rango = Hoja.GetUsedRange();

                    //AGREGAMOS TABLA A HOJA
                    Table table = Hoja.Tables.Add(rango, true);
                    table.Style = Libro.TableStyles[BuiltInTableStyleId.TableStyleLight10];
                    table.HeaderRowRange.Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
                    ColumnCollection columnas = Hoja.Columns;
                    columnas.AutoFit(0, rango.ColumnCount);

                    TableColumnCollection coleccion = table.Columns;

                    //MOSTRAR FILA TOTAL
                    table.ShowTotals = true;

                    foreach (TableColumn columna in coleccion)
                    {
                        if (columna.Name == ColumName)
                        {

                            //SI ES IGUAL SETEAMOS SUS PROPIEDADES
                            //NOMBRE QUE SE VA A MOSTRAR                            
                            columna.TotalRowFunction = TotalRowFunction.Sum;
                            columna.Range.NumberFormat = "#,##0.00";

                            break;
                        }
                    }

                    //GUARDAMOS LIBRO
                    Libro.SaveDocument(PathFile, DocumentFormat.Xlsx);
                    Libro.Dispose();
                    if (File.Exists(PathFile))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        //ABRIR ARCHIVO EXCEL
        public static void AbrirExcel(string PathFile)
        {
            if (PathFile != "")
            {
                System.Diagnostics.Process.Start(PathFile);
            }
        }

        //MOSTRAR ARCHIVO EXCEL
        public static void VerExcel(string File)
        {
            Excel.Application excel = new Excel.Application();
            Excel.Workbooks libros = excel.Workbooks;
            Excel.Workbook libro = libros.Open(File);
            Excel.Worksheet wx = excel.ActiveSheet as Excel.Worksheet;
            excel.Visible = true;

            //LIMPIAR BASURA
            LimpiarBasura(libros);
            LimpiarBasura(libro);
            LimpiarBasura(wx);
            LimpiarBasura(excel);

        }

        //LIBERAR TODOS LOS OBJETOS COM
        private static void LimpiarBasura(object obj)
        {
            if (obj != null)
            {
                try
                {
                    while (Marshal.ReleaseComObject(obj) > 0)
                    {
                        //...
                    }
                }
                catch { }
                finally { obj = null; }
            }
        }

        //LIMPIAR TODOS LOS OBJETOS COM
        private static void LimpiaTodo(Excel._Workbook libro, Excel.Workbooks libros, Excel._Worksheet hoja, Excel._Application Programa, Excel.Range rango)
        {
            libro.Close(false);
            LimpiarBasura(libro);
            LimpiarBasura(libros);
            LimpiarBasura(hoja);
            LimpiarBasura(rango);
            Programa.Quit();
            LimpiarBasura(Programa);
        }

        //SABER SI UN EXCEL ESTA ABIERTO
        //RECIBE COMO PARAMETRO EL NOMBRE DEL LIBRO (WORKBOOK)
        public static bool ExcelAbierto(string FileName)
        {
            bool isOpen = false;

            try
            {
                Stream s = File.Open(FileName, FileMode.Open, FileAccess.Read, FileShare.None);
                s.Close();
            }
            catch (Exception ex)
            {
                isOpen = true;
            }

            return isOpen;
        }        

        //SABER SI ESTA INSTALADO EXCEL EN EL EQUIPO
        public static bool IsExcelInstalled()
        {
            //...
            bool instalado = false;
            Type officeType = Type.GetTypeFromProgID("Excel.Application");
            if (officeType == null)
            {
                //NO ESTA INSTALLADO!!!
                instalado = false;
            }
            else
            {
                instalado = true;
                //ESTA INSTALADO
            }

            return instalado;
        }

        //LO MISMO PARA WORD
        public static bool IsWordInstalled()
        {
            //...
            bool instalado = false;
            Type officeType = Type.GetTypeFromProgID("Word.Application");
            if (officeType == null)
            {
                //NO ESTA INSTALLADO!!!
                instalado = false;
            }
            else
            {
                instalado = true;
                //ESTA INSTALADO
            }

            return instalado;
        }

        //ABRIR EXCEL Y LEER SU DATA
        //DEVUELVE UN DATATABLE
        public static DataTable ReadExcelDev(string PathFile)
        {
            DataTable data = new DataTable();

            //CREAMOS LIBRO
            Workbook Libro = new Workbook();

            //PREGUNTAMOS SI EL ARCHIVO EXISTE
            if (File.Exists(PathFile))
            {
                //CARGAMOS DOCUMENTO
                try
                {
                    Libro.LoadDocument(PathFile, DocumentFormat.Xlsx);

                    //OBTENEMOS LA HOJA DESDE ARCHIVO
                    Worksheet Hoja = Libro.Worksheets[0];                   

                    //TableCollection tablas = Hoja.Tables;
                    //if (tablas == null || tablas.Count == 0)
                      //  return null;

                    //OBTENEMOS EL RANGO USADO DE LA HOJA
                    //Range rango = Hoja.Tables[0].Range;
                    Range rango = Hoja.GetUsedRange();
                  

                    //LLAMAMOS AL METODO CREATE DATA TABLE
                    //SI EL SEGUNDO PARAMETRO ES TRUE DECIMOS QUE USAREMOS LA PRIMERA FILA COMO LAS COLUMNAS DEL DATATABLE
                    data = Hoja.CreateDataTable(rango, true);

                    foreach (DataColumn columna in data.Columns)
                    {
                        if (columna.ColumnName.Trim().ToLower() == "contrato")
                            columna.DataType = typeof(string);
                        if (columna.ColumnName.Trim().ToLower() == "rut")
                            columna.DataType = typeof(string);
                    }

                    DataTableExporter exporter = Hoja.CreateDataTableExporter(rango, data, true);

                    //ExcelConverter converter = new ExcelConverter();
                    //converter.EmptyCellValue = "N/A";
                    //exporter.Options.ConvertEmptyCells = true;
                    //exporter.Options.DefaultCellValueToColumnTypeConverter.SkipErrorValues = true;

                    exporter.CellValueConversionError += Exporter_CellValueConversionError;

                    //exporter.Options.DefaultCellValueToColumnTypeConverter.SkipErrorValues = false;
                    exporter.Export();
                    Libro.Dispose();
                    

                    //QUITAMOS ESPACIOS EN LOS NOMBRES DE LAS COLUMNAS
                    foreach (DataColumn column in data.Columns)
                    {
                        column.ColumnName = QuitarEspacios(column.ColumnName);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return null;
                }
            }

            return data;
        }

        private static void Exporter_CellValueConversionError(object sender, CellValueConversionErrorEventArgs e)
        {
            //e.DataTableValue = "";           
            e.Action = DataTableExporterAction.Continue;
            //e.Cell.Value = e.CellValue;
            
        }

        //LECTURA DE ARCHIVO CSV
        public static DataTable ReadCsv(string PathFile)
        {
            DataTable tabla = new DataTable();
            List<DataColumn> columnas = new List<DataColumn>();
            string Linea = "";
            string[] data;
            int count = 0;

            if (File.Exists(PathFile) == false) return null;
            
            StreamReader Lector = new StreamReader(PathFile);
            try
            {
                while (!Lector.EndOfStream)
                {
                    //LEEMOS CADA LINEA
                    Linea = Lector.ReadLine();                    

                    //SEPARAMOS LINEA POR ;
                    if (Linea != "")
                    {
                        if (ContieneSeparador(Linea) == false) return null;

                        data = SeparaLinea(Linea);

                        //PRIMERA FILA COLUMNAS
                        if (count == 0)
                        {
                            columnas = ListColumns(data);
                            AddColumns(tabla, columnas);
                        }

                        //GENERAMOS UNA FILA
                        DataRow row = tabla.NewRow();

                        if (count > 0 && count < data.Length - 1)
                            tabla.Rows.Add(AddRows(data, tabla.Columns.Count));
                    }
                    else
                    {
                        //SI LA LINEA ESTA VACÍA
                        return null;
                    }
                    count++;
                }
                Lector.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }            

            return tabla;
        }

        /*SEPARA LINEA EN PORCIONES EN BASE A SEPARADOR ;*/
        private static string[] SeparaLinea(string pLine)
        {
            string[] data;
            data = pLine.Split(';');
            return data;
        }

        /*CREAR UNA NUEVA FILA Y LA AGREGA AL DATATABLE*/
        private static object[] AddRows(string[] pData, int pCount)
        {
            object[] row = new object[pCount];
            /*ASUMIR QUE LA CANTIDAD DE CADENAS QUE TIENE PDATA ES IGUAL A LA CANTIDAD DE COLUMNAS DEL DATATABLE*/
            if (pData.Length > 0)
            {
                for (int i = 0; i < pData.Length - 1; i++)
                {
                    row[i] = pData[i];
                }
            }

            return row;
        }

        /*GENERA COLUMNAS*/
        private static List<DataColumn> ListColumns(string[] pData)
        {
            List<DataColumn> listado = new List<DataColumn>();
            for (int i = 0; i < pData.Length - 1; i++)
            {
                listado.Add(new DataColumn(pData[i]));
            }

            return listado;
        }

        /*AGREGAR COLUMNAS AL DATATABLE */
        private static void AddColumns(DataTable pTabla, List<DataColumn> columnas)
        {
            foreach (DataColumn column in columnas)
            {
                pTabla.Columns.Add(column);
            }
        }

        /*VERIFICAR QUE LINEA CONTENGA ;*/
        private static bool ContieneSeparador(string pCadena)
        {
            if (pCadena.Contains(";")) return true;
            else return false;
        }

        //QUITAR ESPACIOS EN CADENA 
        private static string QuitarEspacios(string cadena)
        {
            string cad = "";

            if (cadena.Length > 0)
            {
                for (int i = 0; i < cadena.Length; i++)
                {
                    if (cadena[i].ToString() != " ")
                        cad = cad + cadena[i];
                }
            }

            return cad;
        }

        /*ITERAR RANGO SELECCIONADO*/
        private void RecorreRango(Range rango)
        {
            string cellvalue = "";
            if (rango.RowCount>0)
            {
                for (int row = 0; row < rango.RowCount; row++)
                {
                    for (int col = 0; col < rango.ColumnCount; col++)
                    {
                        Cell celda = rango[row, col];
                        cellvalue = celda.Value.ToString();
                    }
                }
            }
        }
    }
}
