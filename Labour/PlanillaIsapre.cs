using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labour
{

    //CLASE PARA GENERAR PLANILLA CON LOS PAGOS POR CONCEPTO DE ISAPRE PARA LAS PERSONAS AFILIADAS
    class PlanillaIsapre
    {
        //CONTRATO TRABAJO
        private string contrato = "";
        //RUT TRABAJADOR
        private string rut = "";
        //NUMERO DE FUN  
        private string fun = "";
        //NOMBRE DEL TRABAJADOR
        private string nombre = "";        
        //TOTAL IMPONIBLE
        private double imponible = 0;
        // 7% COTIZACION SALUD
        private double saludObligatorio = 0;
        //COTIZACION PACTADA CON ISAPRE
        private double pagoIsapre = 0;
        //COTIZACION FINAL
        private double pagoFinal = 0;
        //EXCEDENTE
        private double excedente = 0;
        private int codMov = 0;
        private string inicioMov = "";
        private string terminoMov = "";

        //Centro de costo
        private string centro = "";
        //Sucursal
        private string sucursal = "";
        //Area
        private string area = "";
        //Cargo
        private string cargo = "";

        public string Contrato {
            get { return this.contrato; }
            set { this.contrato = value; }
        }
        public string Rut
        {
            get { return this.rut; }
            set { this.rut = value; }
        }
        public string Fun
        {
            get { return this.fun; }
            set { this.fun = value; }
        }
        public string NombreTrabajador {
            get { return this.nombre; }
            set { this.nombre = value; }
        }        
        public double Imponible {
            get { return this.imponible; }
            set { this.imponible = value; }
        }
        public double SaludObligatorio {
            get { return this.saludObligatorio; }
            set { this.saludObligatorio = value; }
        }
        public double Exedente
        {
            get { return this.excedente; }
            set { this.excedente = value; }
        }
        public double PagoFinal
        {
            get { return this.pagoFinal; }
            set { this.pagoFinal = value; }
        }
        public double PagoIsapre {
            get { return this.pagoIsapre; }
            set { this.pagoIsapre = value; }
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

        /*CENTRO DE COSTO*/
        public string CentroCosto
        {
            get { return this.centro; }
            set { this.centro = value; }
        }

        /*Sucursal*/
        public string Sucursal
        {
            get{ return this.sucursal; }
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
