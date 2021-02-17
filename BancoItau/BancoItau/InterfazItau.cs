using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BancoItau
{
    /// <summary>
    /// Permite la comunicacion desacoplada entre dos formularios
    /// </summary>
    public interface InterfazItau
    {
        /// <summary>
        /// Permite cerrar este formulario desde el formulario padre que lo invocó.
        /// </summary>
        void CerrarVentanaExterna();

    }
}
