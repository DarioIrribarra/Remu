using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportesExternos
{
    class PlanillaCaja
    {
        //RUT TRABAJADOR
        private string rut = "";
        private string nombre = "";
        /*VALOR PARA EL CASO DE QUE EL TRABAJADOR ESTÉ EN ISAPRE*/
        private double valorisapre = 0;
        /*VALOR PARA EL CASO DE QUE EL TRABAJADOR NO ESTÉ EN ISAPRE*/
        private double valorsinisapre = 0;
        /*COTIZACION 6% (VALOR CAJA)*/
        private double cotizacioncaja = 0;
        private double diastrab = 0;
        /*CANTIDAD CARGAS FAMILIARES SIMPLES O NORMALES*/
        private int simple = 0;
        private int invalida = 0;
        private int maternal = 0;
        private double asigfam = 0;
        private int tramo = 0;
        private int codMov = 0;
        private string inicioMov = "";
        //Centro de costo
        private string centrocosto = "";
        private string sucursal = "";
        private string cargo = "";
        private string area = "";

        public string Rut
        {
            get { return this.rut; }
            set { this.rut = value; }
        }

        public string NombreTrabajador
        {
            get { return this.nombre; }
            set { this.nombre = value; }
        }

        public double ValorIsapre
        {
            get { return this.valorisapre; }
            set { this.valorisapre = value; }
        }

        public double ValorSinIsapre
        {
            get { return this.valorsinisapre; }
            set { this.valorsinisapre = value; }
        }

        public double CotizacionCaja
        {
            get { return this.cotizacioncaja; }
            set { this.cotizacioncaja = value; }
        }
        public double DiasTrabajados
        {
            get { return this.diastrab; }
            set { this.diastrab = value; }
        }

        public int Simples
        {
            get { return this.simple; }
            set { this.simple = value; }
        }
        public int Invalidas
        {
            get { return this.invalida; }
            set { this.invalida = value; }
        }
        public int Maternal
        {
            get { return this.maternal; }
            set { this.maternal = value; }
        }
        public double AsignacionFamiliar
        {
            get { return this.asigfam; }
            set { this.asigfam = value; }
        }
        public int Tramo
        {
            get { return this.tramo; }
            set { this.tramo = value; }
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

        public string CentroCosto
        {
            get { return this.centrocosto; }
            set { this.centrocosto = value; }
        }

        public string Sucursal
        {
            get { return this.sucursal; }
            set { this.sucursal = value; }
        }

        public string Cargo
        {
            get { return this.cargo; }
            set { this.cargo = value; }
        }

        public string Area
        {
            get { return this.area; }
            set { this.area = value; }
        }

    }
}
