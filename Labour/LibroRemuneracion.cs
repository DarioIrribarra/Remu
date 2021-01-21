using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labour
{
    class LibroRemuneracion
    {
        private string nombre = "";
        private string contrato = "";
        private string Sbase = "0";
        private string gratificacion ="0";
        private string hextras = "0";
        private string bono = "0";
        private string tImponible = "0";
        private string movilizacion = "0";
        private string colacion = "0";
        private string tExentos = "0";
        private string familia = "0";
        private string afp = "0";
        private string salud = "0";
        private string segtrab = "0";
        private string segemp = "0";
        private string impuesto = "0";
        private string anticipo = "0";
        private string prestamo = "0";
        private string tDescuento = "0";
        private string tLiquido = "0";
        private string tPago = "0";

        //CLASE QUE USAREMOS COMO DATASOURCE PARA GENERAR LOS DATOS EN GRILLA
        public string Nombre {
            get { return this.nombre; }
            set { this.nombre = value; }
        }
        public string Contrato
        {
            get { return contrato; }
            set { this.contrato = value; }
        }
        //SUELDO BASE
        public string Base
        {
            get { return Sbase; }
            set { this.Sbase = value; }
        }
        //GRATIFICACION
        public string Gratificacion
        {
            get { return this.gratificacion; }
            set { this.gratificacion = value; }
        }
        //HORAS EXTRAS
        public string HorasExtras
        {
            get { return this.hextras; }
            set { this.hextras = value; }
        }
        //BONOS
        public string Bonos
        {
            get { return this.bono; }
            set { this.bono = value; }
        }
        //TOTAL IMPONIBLES
        public string TotalImponible
        {
            get { return this.tImponible; }
            set { this.tImponible = value; }
        }
        //HABERES NO IMPONIBLES
        public string Movilizacion
        {
            get { return this.movilizacion; }
            set { this.movilizacion = value; }
        }
        public string Colacion
        {
            get { return this.colacion; }
            set { this.colacion = value; }
        }
        //TOTAL
        public string TotalExentos
        {
            get { return this.tExentos; }
            set { this.tExentos = value; }
        }
        //CARGAS FAMILIARES
        public string CargasFamiliares
        {
            get { return this.familia; }
            set { this.familia = value; }
        }
        //LEYES SOCIALES
        public string Afp
        {
            get { return this.afp; }
            set { this.afp = value; }
        }
        public string Salud
        {
            get { return this.salud; }
            set { this.salud = value; }
        }
        public string SeguroTrabajador
        {
            get { return this.segtrab; }
            set { this.segtrab = value; }
        }
        public string SeguroEmpresa
        {
            get { return this.segemp; }
            set { this.segemp = value; }
        }
        public string ImpuestoTrab
        {
            get { return this.impuesto; }
            set { this.impuesto = value; }
        }
        public string Anticipo
        {
            get { return this.anticipo; }
            set { this.anticipo = value; }
        }
        public string Prestamo
        {
            get { return this.prestamo; }
            set { this.prestamo = value; }
        }
        //TOTAL DESCUENTOS
        public string TotalDescuentos
        {
            get { return this.tDescuento; }
            set { this.tDescuento = value; }
        }
        //TOTAL LIQUIDO
        public string TotalLiquido
        {
            get { return this.tLiquido; }
            set { this.tLiquido = value; }
        }
        //TOTAL PAGO
        public string TotalPago
        {
            get { return this.tPago; }
            set { this.tPago = value; }
        }
    }
}
