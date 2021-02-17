using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportesExternos
{
    class PlanillaMutual
    {
        //PLANILLA PARA COTIZACIONES MUTUAL
        private string rut = "";
        private string nombre = "";
        private double dias = 0;
        private DateTime nacimiento = DateTime.Now.Date;
        private string sexo = "";
        private double imponible = 0;
        //Centro de costo
        private string centrocosto = "";
        private string sucursal = "";
        private string area = "";
        private string cargo = "";


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
        public string Sexo
        {
            get { return this.sexo; }
            set { this.sexo = value; }
        }
        public DateTime Nacimiento
        {
            get { return this.nacimiento; }
            set { this.nacimiento = value; }
        }
        public double Dias
        {
            get { return this.dias; }
            set { this.dias = value; }
        }

        public double Imponible
        {
            get { return this.imponible; }
            set { this.imponible = value; }
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
