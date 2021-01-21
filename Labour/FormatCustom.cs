using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Labour
{
    class FormatCustom: IFormatProvider, ICustomFormatter
    {
        public object GetFormat(System.Type Type)
        {
            return this;
        }

        //FUNCION PARA REALIZAR EL FORMATEO
        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            string formatValue = arg.ToString();
            string RutFormat = "";
            string cadena = "";

            Cifrado cipher = new Cifrado();

            if (format == "Rut")
            {
                if (formatValue.Length > 0)
                {
                    //logica para formatear la cadena como rut
                    RutFormat = fnSistema.fFormatearRut2(formatValue);
                }
               
                //retornamos la cadena formatedaa
                return RutFormat;
            }

            //PARA MOSTRAR NOMBRE EN VEZ DE UN NUMERO EN CAMPO TIPO DE LA TABLA ITEMS
            if (format == "Tipo")
            {
                if (formatValue == "1")
                {
                    cadena = "Haber";                    
                }
                if (formatValue == "2")
                {
                    cadena = "Haber Exento";
                }
                if (formatValue == "3")
                {
                    cadena = "Familiar";
                }
                if (formatValue == "4")
                {
                    cadena = "Descuento legal";
                }
                if (formatValue == "5")
                {
                    cadena = "Descuento";
                }
                if (formatValue == "6")
                {
                    cadena = "Contribuciones";
                }
                if (formatValue == "7")
                {
                    cadena = "Totales";
                }

                return cadena;
            }


            if (format == "formula")
            {
                if (formatValue == "0")
                {                   
                    return "NO USA";
                }
                else
                {
                    return formatValue;
                }
            }

            if (format == "motivo")
            {
                string mo = "";
                if (formatValue == "1")
                {
                    mo = "MEDICA";
                }
                else if (formatValue == "2")
                {
                    mo = "MATERNAL";
                }
                else if (formatValue == "3")
                {
                    mo = "PERMISO";
                }
                else if (formatValue == "4")
                {
                    mo = "INASISTENCIA";
                }
                else if (formatValue == "5")
                {
                    mo = "ACCIDENTE";
                }
                else if (formatValue == "13")
                {
                    mo = "SUSPENSION POR AUTORIDAD";
                }
                else if (formatValue == "14")
                {
                    mo = "SUSPENSION POR PACTO";
                }
                else if (formatValue == "15")
                {
                    mo = "REDUCCION JORNADA";
                }
                return mo;
            }

            if (format == "vacacion")
            {
                string vac = "";
                if (formatValue == "1")
                {
                    vac = "Proporcional";
                }
                if (formatValue == "2")
                {
                    vac = "Progresivo";
                }
                return vac;
            }

            if (format == "periodo")
            {
                int data = 0;
                data = Convert.ToInt32(formatValue);
                DateTime dateFromPeriodo = DateTime.Now.Date;
                dateFromPeriodo = fnSistema.FechaPeriodo(data);

                //GENERAMOS NOMBRE DESDE PERIODO
                return (fnSistema.PrimerMayuscula(fnSistema.GetNameDate(dateFromPeriodo)));
            }

            if (format == "online")
            {
                if (formatValue == "1")
                    return "Si";
                else
                    return "No";
            }

            if (format == "estado")
            {
                if (formatValue == "1")
                    return "A";
                else
                    return "I";
            }

            /*PARA SESIONES...*/
            if (format == "usuario")
                return cipher.DesencriptaTripleDesc(formatValue);
            if (format == "equipo")
                return cipher.DesencriptaTripleDesc(formatValue);
            if (format == "ip")
                return cipher.DesencriptaTripleDesc(formatValue);
            if (format == "db")
                return cipher.DesencriptaTripleDesc(formatValue);
            if (format == "hostname")
                return cipher.DesencriptaTripleDesc(formatValue);

            return "";
        }

      
    }
}
