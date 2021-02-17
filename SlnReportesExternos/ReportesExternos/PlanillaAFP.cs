using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportesExternos
{
    class PlanillaAfp
    {
        //CLASE PARA PLANILLA DE PREVISIONES DE LOS EMPLEADOS
        private string rut = "";
        private string nombre = "";
        private double imponible = 0;
        private double cotizacion = 0;
        private double sis = 0;
        //SEGURO EMPRESA COMPRENDE LA SUMA DEL FONDO SOLIDARIO Y CUENTA INDIVIDUAL
        private double segempresa = 0;
        private double segtrabajador = 0;
        private int codMov = 0;
        private string inicioMov = "";
        private string terminoMov = "";
        private string rutPagadora = "";
        private double cuentaAhor = 0;
        //Centro de costo
        private string centro = "";
        //Sucursal
        private string sucursal = "";
        //Cargo
        private string cargo = "";
        //Area
        private string area = "";

        public string Rut
        {
            get { return this.rut; }
            set { this.rut = value; }
        }
        public string Nombre
        {
            get { return this.nombre; }
            set { this.nombre = value; }
        }
        public double Imponible
        {
            get { return this.imponible; }
            set { this.imponible = value; }
        }
        public double Cotizacion
        {
            get { return this.cotizacion; }
            set { this.cotizacion = value; }
        }
        public double Sis
        {
            get { return this.sis; }
            set { this.sis = value; }
        }
        public double SegEmpresa
        {
            get { return this.segempresa; }
            set { this.segempresa = value; }
        }
        public double SegTrabajador
        {
            get { return this.segtrabajador; }
            set { this.segtrabajador = value; }
        }

        /*CODIGO MOVIMIENTO DE PERSONAL*/
        public int CodMovimiento
        {
            get { return this.codMov; }
            set { this.codMov = value; }
        }

        /*FECHA INICIO MOVIMIENTO DE PERSONAL*/
        public string InicioMovimiento
        {
            get { return this.inicioMov; }
            set { this.inicioMov = value; }
        }

        /*FECHA TERMINO MOVIMIENTO PERSONAL*/
        public string TerminoMovimiento
        {
            get { return this.terminoMov; }
            set { this.terminoMov = value; }
        }

        /*RUT PAGADORA SUBSIDIO*/
        public string RutPagadoraSubsidio
        {
            get { return this.rutPagadora; }
            set { this.rutPagadora = value; }

        }

        public double CuentaAhorro
        {
            get { return this.cuentaAhor; }
            set { this.cuentaAhor = value; }

        }

        /*Centro de costo*/
        public string CentroCosto
        {
            get { return this.centro; }
            set { this.centro = value; }
        }

        /*Sucursal*/
        public string Sucursal
        {
            get { return this.sucursal; }
            set { this.sucursal = value; }
        }

        /*Area*/
        public string Area
        {
            get { return this.area; }
            set { this.area = value; }
        }

        /*Cargo*/
        public string Cargo
        {
            get { return this.cargo; }
            set { this.cargo = value; }
        }
    }
}
