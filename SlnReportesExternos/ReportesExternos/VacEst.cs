using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportesExternos
{
    /// <summary>
    /// Solo como datasource para reporte de vacaciones estimadas
    /// </summary>
    class VacEst
    {
        public string Nombre { get; set; }
        public string Rut { get; set; }
        public string Cargo { get; set; }
        public string Sucursal { get; set; }
        public string Area { get; set; }
        public string CentroCosto { get; set; }
        public double DiasPropporcionales { get; set; }
        public double DiasProgresivos { get; set; }
        public DateTime FechaTope { get; set; }
        public double PropUsados { get; set; }
        public double ProgUsados { get; set; }
        public double Cotiz { get; set; }
        public DateTime FecVac { get; set; }

    }
}
