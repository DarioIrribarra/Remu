using DevExpress.XtraEditors;
using DevExpress.XtraReports;
using DevExpress.XtraReports.Configuration;
using DevExpress.XtraReports.Extensions;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UserDesigner;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Labour
{
    /// <summary>
    /// Clase que maneja el uso de ReportDesigner para que el usuario pueda modificar reportes
    /// </summary>
    public class DiseñadorReportes
    {

        public static string rutaReportes = fnSistema.RutaCarpetaReportesExterno;
        private const string ExtensionName = "Custom";

        /// <summary>
        /// Precarga las dependencias del designer con un reporte vacío
        /// </summary>
        public static void PreCargarDependenciasDiseñador()
        {
            XtraReport reporte = new XtraReport();
            ReportDesignTool diseñador = new ReportDesignTool(reporte);
            //Se crea un objeto que permite editar la visibilidad de los controles existentes en Ribbon
            IDesignForm designForm = diseñador.DesignRibbonForm;
            XRDesignMdiController mdiController = designForm.DesignMdiController;
        }

        /// <summary>
        /// Muestra el ReportDesigner limitando algunas opciones como el añadir parámetros o manipular consultas y conexiones a bd
        /// </summary>
        /// <param name="reporte"></param>
        /// <param name="reportName"></param>
        /// <param name="popUpWait"></param>
        public static void MostrarEditorLimitado(XtraReport reporte, string reportName, DevExpress.XtraSplashScreen.SplashScreenManager popUpWait = null) 
        {
            //Ruta por defecto para guardar reportes
            //DevExpress.XtraReports.Configuration.Settings.Default.StorageOptions.RootDirectory = rutaReportes;

            /*
            // The following code is required to support serialization of multiple custom objects.
            TypeDescriptor.AddAttributes(typeof(DataSet), new ReportAssociatedComponentAttribute());
            TypeDescriptor.AddAttributes(typeof(OleDbDataAdapter), new ReportAssociatedComponentAttribute());

            // The following code is required to serialize custom objects.
            ReportExtension.RegisterExtensionGlobal(new ReportExtension());
            ReportDesignExtension.RegisterExtension(new DesignExtension(), ExtensionName);
            */

            
            //Oculta parámetros
            foreach (DevExpress.XtraReports.Parameters.Parameter parametro in reporte.Parameters)
            {
                parametro.Visible = false;
            }

            ReportDesignTool diseñador = new ReportDesignTool(reporte);
            //Se crea un objeto que permite editar la visibilidad de los controles existentes en Ribbon
            IDesignForm designForm = diseñador.DesignRibbonForm;
            XRDesignMdiController mdiController = designForm.DesignMdiController;

            #region Manejo Evento Guardado
            //Se usa el controlador de reportes para asignar evento de diseño
            mdiController.DesignPanelLoaded += new DesignerLoadedEventHandler(mdiController_DesignPanelLoaded);

            //Se asigna el evento load
            //mdiController.DesignPanelLoaded += OnDesignPanelLoaded;

            //Método que le pasa el panel de diseño a la clase que maneja el evento de guardado
            void mdiController_DesignPanelLoaded(object sender, DesignerLoadedEventArgs e)
            {
                XRDesignPanel panel = (XRDesignPanel)sender;
                panel.AddCommandHandler(new SaveCommandHandler(panel, reportName));
            }

            //Evento al terminar de cargar
            //void OnDesignPanelLoaded(object sender, DevExpress.XtraReports.UserDesigner.DesignerLoadedEventArgs e)
            //{
            //    //ReportDesignExtension.AssociateReportWithExtension((XtraReport)e.DesignerHost.RootComponent, ExtensionName);
            //    //Cierra popup

            //    //if (!e.DesignerHost.Loading) 
            //    //{
            //    //    if (popUpWait != null)
            //    //        popUpWait.CloseWaitForm();
            //    //}
            //}

            #endregion

            //se ocultan los controles
            ReportCommand[] controles = new ReportCommand[]
            {
                ReportCommand.AddNewDataSource,
                ReportCommand.AddParameter,
                ReportCommand.NewReport,
                ReportCommand.NewReportWizard,
                ReportCommand.OpenFile,
                ReportCommand.OpenRemoteReport,
                ReportCommand.OpenSubreport,
                ReportCommand.PivotGridAddDataSource,
                ReportCommand.PivotGridAddField,
                ReportCommand.PivotGridRemoveField,
                ReportCommand.SaveAll,
                ReportCommand.SaveFileAs,
                ReportCommand.UploadNewRemoteReport,
                ReportCommand.VerbImport,
                ReportCommand.VerbLoadReportTemplate,
                ReportCommand.VerbReportWizard,
                ReportCommand.VerbRtfLoadFile,
                
            };

            mdiController.SetCommandVisibility(controles, CommandVisibility.None);

            //Se bloquean popups referentes a transformación de Bindings a expresiones
            Settings.Default.UserDesignerOptions.ConvertBindingsToExpressions = DevExpress.XtraReports.UI.PromptBoolean.False;

            //Visibilidad de los paneles con información referente a sql
            ReportExplorerDockPanel reportExplorer =
                (ReportExplorerDockPanel)designForm.DesignDockManager[DesignDockPanelType.ReportExplorer];
            reportExplorer.Hide();

            FieldListDockPanel fieldList =
                (FieldListDockPanel)designForm.DesignDockManager[DesignDockPanelType.FieldList];
            fieldList.Hide();

            //Visibilidad de los paneles innecesarios
            GroupAndSortDockPanel groupAndSort =
                (GroupAndSortDockPanel)designForm.DesignDockManager[DesignDockPanelType.GroupAndSort];
            groupAndSort.Hide();

            ErrorListDockPanel errorList =
                (ErrorListDockPanel)designForm.DesignDockManager[DesignDockPanelType.ErrorList];
            errorList.Hide();

            if (popUpWait != null)
                popUpWait.CloseWaitForm();

                //Abre el diseñador
                diseñador.ShowRibbonDesignerDialog();
        }

        /// <summary>
        /// Se muestra editor completo con sentencias sql y parámetros
        /// </summary>
        /// <param name="reporte"></param>
        /// <param name="reportName"></param>
        /// <param name="popUpWait"></param>
        public static void MostrarEditorCompleto(XtraReport reporte, string reportName, DevExpress.XtraSplashScreen.SplashScreenManager popUpWait = null)
        {
            //DevExpress.XtraReports.Configuration.Settings.Default.StorageOptions.RootDirectory = rutaReportes;
            
            foreach (DevExpress.XtraReports.Parameters.Parameter parametro in reporte.Parameters)
            {
                parametro.Visible = false;
            }

            ReportDesignTool diseñador = new ReportDesignTool(reporte);

            //Se crea un objeto que permite editar la visibilidad de los controles existentes en Ribbon
            IDesignForm designForm = diseñador.DesignRibbonForm;
            XRDesignMdiController mdiController = designForm.DesignMdiController;


            #region Manejo Evento Guardado
            //Se usa el controlador de reportes para asignar evento de diseño
            mdiController.DesignPanelLoaded +=
                new DesignerLoadedEventHandler(mdiController_DesignPanelLoaded);

            //Método que le pasa el panel de diseño a la clase que maneja el evento de guardado
            void mdiController_DesignPanelLoaded(object sender, DesignerLoadedEventArgs e)
            {
                XRDesignPanel panel = (XRDesignPanel)sender;
                panel.AddCommandHandler(new SaveCommandHandler(panel, reportName));
            }

            #endregion

            //Cierra popup
            if (popUpWait != null)
                popUpWait.CloseWaitForm();

            //Abre el diseñador
            diseñador.ShowRibbonDesignerDialog();
        }

    }
    #region Prueba Exportacion Estructura SQL
    /*
    class ReportExtension : ReportStorageExtension
    {
        public override void SetData(XtraReport report, Stream stream)
        {
            report.SaveLayoutToXml(stream);
        }
    }

    class DesignExtension : ReportDesignExtension
    {
        protected override bool CanSerialize(object data)
        {
            return data is DataSet || data is OleDbDataAdapter;
        }
        protected override string SerializeData(object data, XtraReport report)
        {
            if (data is DataSet)
                return (data as DataSet).GetXmlSchema();
            if (data is OleDbDataAdapter)
            {
                OleDbDataAdapter adapter = data as OleDbDataAdapter;
                return adapter.SelectCommand.Connection.ConnectionString +
                    "\r\n" + adapter.SelectCommand.CommandText;
            }

            return base.SerializeData(data, report);
        }

        protected override bool CanDeserialize(string value, string typeName)
        {
            return typeof(DataSet).FullName ==
                typeName || typeof(OleDbDataAdapter).FullName == typeName;
        }
        protected override object DeserializeData(string value, string typeName, XtraReport report)
        {
            if (typeof(DataSet).FullName == typeName)
            {
                DataSet dataSet = new DataSet();
                dataSet.ReadXmlSchema(new StringReader(value));
                return dataSet;
            }
            if (typeof(OleDbDataAdapter).FullName == typeName)
            {
                OleDbDataAdapter adapter = new OleDbDataAdapter();
                string[] values = value.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                adapter.SelectCommand = new OleDbCommand(values[1], new OleDbConnection(values[0]));
                return adapter;
            }
            return base.DeserializeData(value, typeName, report);
        }
    }
    */
    #endregion
    /// <summary>
    /// Clase que maneja eventos propios del ReportDesigner
    /// </summary>
    public class SaveCommandHandler : ICommandHandler
    {
        XRDesignPanel panel;
        string reportName;

        public SaveCommandHandler(XRDesignPanel panel, string reportName)
        {
            this.panel = panel;
            this.reportName = reportName;
        }

        //Método que maneja el evento de guardado directamente
        public void HandleCommand(ReportCommand command, object[] args)
        {
            // Guarda el reporte
            Save();
        }

        //Función que verifica si activar el evento de guardado denpiendo de que ReportCommand reciba
        public bool CanHandleCommand(ReportCommand command, ref bool useNextHandler)
        {
            useNextHandler = !(command == ReportCommand.SaveFile || command == ReportCommand.SaveFileAs);
            return !useNextHandler;
        }

        //Método de guardado
        void Save()
        {
            try
            {
                //Se guarda sin mostrar Dialog de ruta
                panel.Report.SaveLayoutToXml(Path.Combine(DiseñadorReportes.rutaReportes, reportName));

                { XtraMessageBox.Show("Cambios guardados correctamente", "Edición", MessageBoxButtons.OK, MessageBoxIcon.Information);}

                // Evita mostrar mensaje de Reporte es distinto o ha cambiado
                panel.ReportState = ReportState.Saved;

            }
            catch (Exception ex)
            {
                { XtraMessageBox.Show("Ha ocurrido un error de guardado. Verifique la ruta exista y que el archivo no esté siendo utilizado", "Edición", MessageBoxButtons.OK, MessageBoxIcon.Stop); return; }
            }
            
        }
    }
}
