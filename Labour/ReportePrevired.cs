using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labour
{
    class ReportePrevired
    {
        /// <summary>
        /// Afp, salud, etc
        /// </summary>
        public string Entidad { get; set; } = "";
        public string EntidadFonasa { get; set; } = "";
        public string EntidadAfp { get; set; } = "";
        public string EntidadIsapre { get; set; } = "";
        /// <summary>
        /// Cotizacion entidad
        /// </summary>
        public double CotizacionFonasa { get; set; }
        public double CotizacionAfp { get; set; }
        public double CotizacionIsapre { get; set; }
        public double CotizacionIps { get; set; }
        public double Cotizacion { get; set; }
        /// <summary>
        /// Seguro de invalidez
        /// </summary>
        public double SegInv { get; set; }
       /// <summary>
       /// Monto ahorro voluntario
       /// </summary>
        public double AhorroPrevisional { get; set; }
        /// <summary>
        /// Monto seguro cesantia trabajador
        /// </summary>
        public double SegAfiliado { get; set; }
        /// <summary>
        /// Monto seguro cesantia empresa
        /// </summary>
        public double SegEmpresa { get; set; }
        /// <summary>
        /// Asignacion familiar
        /// </summary>
        public double AsignacionFam { get; set; }
        /// <summary>
        /// Adicional isapre
        /// </summary>
        public double AdicionalIsapre { get; set; }
        /// <summary>
        /// Tipo (AFP - ISAPRE - APORTE)
        /// </summary>
        public string Tipo { get; set; }
        public string CentroCosto { get; set; } = "";
        public string Area { get; set; } = "";
        public string Sucursal { get; set; } = "";
        public string Cargo { get; set; } = "";
        public double AhorroVoluntario { get; set; }
        public double Mutal { get; set; } 
        public double Caja { get; set; }
        public double Sanna { get; set; }
        public bool Suspesion { get; set; } = false;

        //AHORRO CAJA
        public double Leasing { get; set; }
        //Prestamos CAJA
        public double Prestamo { get; set; }
        //SEGURO DE VIDA CAJA
        public double SeguroVida { get; set; }
    }
}
