using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.XtraEditors;

namespace Labour
{
    class IForm
    {
    }

    //DEFINICION DE INTERFAZ
    public interface Iform
    {
        //definimos el metodo que nos servira para cargar los datos en los combobox
        //LOS TRES PARAMETROS DE ENTRADA SIMBOLIZAN LA CAJA USER, CAJA PASSWORD Y EL COMBOBOX JUEGO DATOS
         void CargarCajas(string txt1, string txt2, string txt3);

        //METODO PARA CARGAR EL COMBO
        void CargarCombo();
    }

    public interface IGrilla
    {
        //metodo para cargar grilla
        void CargarGrid();

        //METODO PARA USAR SPLASH SCREEN
        void CargarWait();

        //METODO PARA CERRAR SPLASH SCREEEN
        void CerrarWait();
    }


    public interface IMenu
    {
        //METODO PARA MOSTRAR EL MENU
        void ShowMenu();

        //CARGAR CAMPO CON NOMBRE DE USUARIO
        void CargarUser(string user, string DataBase, string Periodo);

        /*REABRIR FORMULARIO ACCESO*/
        void OpenForm();
    }


    //INTERFAZ PARA FORM BUSQUEDA TRABAJADOR
    public interface IBuscarTrabajador
    {
        //METODO 
        void CargarBusqueda(string pContrato, int periodo);        
    }

    //INTERFAZ PARA FORM PLANTILLA BASE FORMULARIO FORMULAS
    public interface IPlantillaFormula
    {
        //metodo
        void CargarPlantilla(string bas);
    }

    //INTERFAZ PARA COMUNICAR FORMULARIO TRABAJADOR CON FORMULARIOS TABLAS Y LEYES SOCIALES
    public interface ITrabajadorCombo
    {
        //RECARGAR COMBOBOX SALUD
        void RecargarComboSalud();

        //RECARGAR COMBOBOX AFP
        void RecargarComboAfp();

        //RECARGAR COMBOBOX CAJA COMPENSACION
        void RecargarComboCaja();

        //RECARGAR COMBOBOX NACIONALIDAD
        void RecargarComboNacionalidad();

        //RECARGAR COMBOBOX PAGO
        void RecargarComboPago();

        //RECARGAR COMBOBOX BANCO
        void RecargarComboBanco();

        //RECARGAR COMBOBOX CARGO
        void RecargarComboCargo();

        //RECARGAR COMBOX AREA
        void RecargarComboArea();

        //RECARGAR COMBOBOX CENTRO COSTO
        void RecargarCombocCosto();

        //RECARGAR COMBO SUCURSAL
        void RecargarComboSucursal();

        //RECARGAR COMBO CAUSAL TERMINO
        void RecargarComboCausal();

    }


    //INTERFAZ PARA COMUNICAR FORMULARIO ITEM CON FORMULARIO FORMULA
    public interface IFormulaItem
    {
        //METODO PARA RECARGAR COMBO FORMULA EN FORMULARIO ITEM
        void RecargarComboFormula();

        //METODO PARA RECARGAR GRILLA DE ACUERDO A REGISTRO ELIMINADO (DEPENDE DEL TIPO)
        void RecargarGrillaTipo(int type);
    }

    //INTERFAZ PARA COMUNICACION ENTRE FORMULARIO AGREGAR VARIABLES Y FORMULARIO VER VARIABLES
    public interface IVariable
    {
        void RecargarGrilla();
    }

    //INTERFAZ PARA COMUNICAR FORMULARIO TRABAJADOR CON FORMULARIO CLASE
    public interface IEmpleadoClase
    {
        //METODO PARA RECARGAR DATOS DESDE CLASE A ITEMTRABAJADOR
        void RecargarDatosClase();
    }


    //INTERFAZ PARA COMUNICACION ENTRE FORMULARIO AUSENTISMO Y FORMULARIO BUSCARCONTRATO
    public interface IBusquedaContrato
    {
        //METODO PARA CARGO CAJA DE TEXTO DE ACUERDO A CONTRATO SELECCIONADO
        void CargarContrato(string pContrato);
    }

    //INTERFAZ PARA COMUNICAR FORMULARIO EMPLEADO Y FORMULARIO CAUSAL DE TERMINO
    public interface ICausalTermino
    {
        void CargarComboCausal();
    }

    //INTERFAZ DE COMUNICACION ENTRE FORMULARIO VER VARIABLES Y FORMULARIO FORMULA
    public interface IVarFormula
    {
        void CargarVariableMemo(int position, string formula);
    }

    //INTERFAX PARA COMUNICACION ENTRE FORMULARIO FORMULA Y BUSQUEDA FORMULA
    public interface IFormBusqueda
    {
        //METODO PARA CARGAR DATOS DE ACUERDO A CODIGO
        void CargarDatos(string codigo);
    }

    //INTERFAZ PARA COMUNICACION ENTRE FORMULARIO GRILLAITEM Y BUSQUEDA ITEM INFORMACION xD
    public interface IFormItemInformacion
    {
        //METODO PARA CARGAR DATO DE GRILLA A CAJA DE TEXTO
        void CargarCodigoItem(string item);

        //METODO PARA RECARGAR GRILLA EN FORMULARIO ITEM
        void RecargaGrilla();

    }

    public interface IUpdateConfig
    {
        //METODO PARA ACTUALIZAR GRILLA CUANDO SE EDITE UN REGISTRO
        void ActualizarGrilla();
    }

    //INTERFAZ DE COMUNICACION ENTRE FORMULARIO Y FORMULARIO DE ADMINISTRACION DE CONJUNTOS CONDICIONALES
    public interface IConjuntosCondicionales
    {
        //CARGAR CODIGO DE CONJUNTO
        void CargarCodigoConjunto(string code);
    }

    //INTERFAZ PARA COMUNICACION ENTRE FORMULARIO COLUMNAS DATA Y OTRO FORMULARIO
    public interface IColumnasData
    {
        //GENERA LISTA DE STRING EN BASE CAMPOS SELECCIONADOS PARA QUERY
        void CargarLista(List<string> Lista);
    }

    //INTERFAZ PARA COMUNICAR FORMULARIO DE ELIMINACION DE ITEMS MASIVOS CON FORMULARIO DE SELECCION NUM ITEM
    public interface ISeleccionItemElimina
    {
        //METODO PARA CARGAR LISTADO DE REGISTROS SELECCIONADOS
        void CargarSeleccion(List<ItemTrabajador> registros);
    }

    //INTERFAZ PARA COMUNICACION ENTRE FORM LICENCIA Y FORM ACCESO
    public interface ILicenciaAcceso
    {
        //CERRAR FORMULARIOS
        void CloseForms();
    }

    //INTERFAZ DE COMUNICACION ENTRE FORMULARIO DE INGRESO DE JUEGOS DE DATOS Y FORMULARIO ACCESO
    public interface IUpdateComboDatos
    {
        //ACTUALIZAR COMBO DATOS
        void RecargarCombo();
    }

    //INTERFAZ DE COMUNICACION ENTRE FORMULARIO CLAVE MAESTRA Y FORMULARIO REABRE MES
    public interface IReabreMes
    {
        //ACTUALIZAR VARIABLE
        void SetVariable(string pData);

        //CERRAR FORMULARIO CLAVE MAESTRA
        void CloseKeyMaster();
    }
    /// <summary>
    /// Permite la comunicacion entre formulario de copia de formulario empleado.
    /// </summary>
    public interface ICopiaEmp
    {
        /// <summary>
        /// Permite cargar informacion de ficha en formulario trabajador.
        /// </summary>
        /// <param name="pContratoCopia">Numero de contrato de ficha a copiar.</param>
        /// <param name="pPeriodoFichaCopia">Periodo ficha.</param>
        /// <param name="pPrimerNombre">Primer nombre nuevo trabajador.</param>
        /// <param name="pSegundoNombre">Segundo nombre nuevo trabajador.</param>
        /// <param name="pApepaterno">Apellido paterno nuevo trabajador.</param>
        /// <param name="pApematerno">Apellido materno nuevo trabajador.</param>
        /// <param name="pNuevoContrato">Numero de contrato para nuevo trabajador</param>
        /// <param name="pRutNuevo">Rut nuevo trabajador.</param>
        void CargarCopia(string pRutNuevo, string pContratoCopia, int pPeriodoFichaCopia, string pPrimerNombre, string pSegundoNombre, string pApepaterno, string pApematerno, string pNuevoContrato);
    }

    /// <summary>
    /// Interfaz de comunicacion entre un formulario y el formulario de seleccion multiple
    /// <para>Carrga de items desde archivo excel o csv.</para>
    /// </summary>
    public interface ISeleccionMultiple
    {
        /// <summary>
        /// Genera sql para update (en formulario que invoca el evento.)
        /// </summary>
        /// <param name="pSql">Consulta sql</param>
        void CargaListado(string pSql);
    }

    /// <summary>
    /// Permite la comunicacion entre formulario main y algun formulario que sea invocado desde main
    /// </summary>
    public interface IMainChildInterface
    {
        /// <summary>
        /// Cambia el estado (caption) de barra de acuerdo a algun proceso activo.
        /// Ej: Proceso de calculo de liquidaciones...
        /// </summary>
        /// <param name="pNewStatus"></param>
        void ChangeStatus(string pNewStatus);

        /// <summary>
        /// Permite indicar si hay un usuario bloqueado en barra de tareas.
        /// </summary>
        void ShowBloqueo(string pNewBloqueo, bool ShowWarning);
    }
    /// <summary>
    /// Permite la comunicacion entre formulario Main y formulario Acceso
    /// </summary>
    public interface IMonitor
    {
        /// <summary>
        /// Intenta detener un timer creado en la ventana de acceso (Notifacaciones)
        /// </summary>
        /// <param name="pTimer"></param>
        void StopTimer(System.Windows.Forms.Timer pTimer);
    }

    /// <summary>
    /// Permite la comunicacion entre formulario generador y formulario filtro (sql builder)
    /// </summary>
    public interface IGenerador
    {
        /// <summary>
        /// Agrega filtros a consulta generada por el sql builder.
        /// </summary>
        void AgregaFiltros();

        /// <summary>
        /// Reordena los campos de la consulta sql de acuerdo a orden generado por el usuario.
        /// </summary>
        /// <param name="pNewOrder"></param>
        void ReordenSql(string pNewOrder,List<Orden> pOrderbyList);

        /// <summary>
        /// nos permite cargar y editar un sql guardado en base de datos
        /// </summary>
        void CargarSql(ReportBuilder pReport);

    }

    /// <summary>
    /// Permite la comunicacion entre formulario saveregistro y listado registros para sql builder
    /// </summary>
    public interface IEditaBuilder
    {
        void ReloadGridControl();
        
    }

}
