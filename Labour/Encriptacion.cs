using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
/*para usar SHA1*/
///////////////////////////////////
using System.Security.Cryptography;
//////////////////////////////////
using System.Text;
using System.Threading.Tasks;
//PARA USAR FUNCIONES DE VISUAL BASIC
using Microsoft.VisualBasic;

namespace Labour
{
    class Encriptacion
    {
        //clase para generar una cadena encriptada

        //VARIABLE PARA GUARDAR CADENA DE ENTRADA
        private string Cadena = "";

        //CONSTRUCTOR
        public Encriptacion(string Cadena)
        {
            this.Cadena = Cadena;
        }

        //CONSTRUCTOR SIN PARAMETROS
        public Encriptacion() { }

        //LLAMAR AL METODO ENCRIPTAR
        public string getHas()
        {
            string Hash = "";
            if (Cadena != "")
            {
                Hash = EncodePassword(Cadena);
            }
            return Hash;
        }

        /*METODO PARA ENCRIPTAR USANDO SHA1*/
        /*Genera un hash el cual una vez creado no se puede desencriptar... :-(*/
        public string EncodePassword(string cadena)
        {
            SHA1 sha1 = new SHA1CryptoServiceProvider();

            byte[] inputBytes = (new UnicodeEncoding()).GetBytes(cadena);
            byte[] hash = sha1.ComputeHash(inputBytes);
            return Convert.ToBase64String(hash);
        }       
    }

    class Cifrado
    {
        //CLAVE PARA CIFRAR Y DESCIFRAR
        private string Key { get; set; } = "%ô&*@#?A";
        //LLAVE PARA ENCRIPTAR SESIONES
        public string KeySesion { get; set; } = "~]/@#$~*-¬";
        //PREFIJO PARA SABER SI UNA CADENA TIENE CIFRADO
        //SI LA CADENA CONTIENE ESTOS CARACTERES AL PRINCIPIO ES PORQUE SE LE APLICÓ UN CIFRADO CON EL METODO DE ABAJO
        private string Prefix { get; set; } = "!!=?¡x0";

        //LLAVE PARA GENERAR UNA LICENCIA TRIAL DE PRUEBA PARA LA PRIMERA VEZ QUE SE INGRESA AL SISTEMA

        //CIFRAR CADENA
        public string Encripta(string pData)
        {
            string final = "", Caracter = "", Codigo = "", hexa = "";
            int asc = 0, asc1 = 0, j = 0;

            for (int i = 0; i < pData.Length; i++)
            {
                //EXTRAEMOS CARACTER DE ACUERDO A INDICE I
                Caracter = pData.Substring(i, 1);
                Codigo = Key.Substring(j, 1);
                //GENERA CODIGO ASCII DE VALOR EN VARIABLE CODIGO           
                asc = Strings.Asc(Caracter);
                asc1 = Strings.Asc(Codigo);                

                hexa = ("0" + ConvertHex(Strings.Asc(Codigo) ^ Strings.Asc(Caracter)));

                //GENERA CODIGO EXADECIMAL
                hexa = hexa.Substring(hexa.Length - 2);

                final = final + hexa;

                //PARA RESETEAR BUSQUEDA CARACTER EN CADENA KEY
                if (j == Key.Length - 1)
                    j = 0;
                else
                    j++;

            }

            return final;
        }

        //DECIFRAR CADENA
        public string Desencripta(string pData)
        {
            string final = "", Codigo = "", Caracter = "";
            int Number = 0, asc = 0;
            char c;
            int j = 0;

            for (int i = 0; i < pData.Length; i = i + 2)
            {
                //OBTENEMOS PAR DE CARACTERES
                Caracter = pData.Substring(i, 2);
                //EXTRAEMOS CARACTER DESDE CADENA 'CLAVE'
                Codigo = Key.Substring(j, 1);
                Number = Convert.ToInt32(Conversion.Val("&h" + Caracter));
                asc = Strings.Asc(Codigo);
                c = Strings.Chr(asc ^ Number);

                final = final + c;
                if (j == Key.Length - 1)
                    j = 0;
                else
                    j++;
            }

            return final;
        }

        //CONVERTIR A HEXADECIMAL
        public static string ConvertHex(int pNumber)
        {
            return pNumber.ToString("X");
        }

        //
        public string EncryptBase64(int pNumber)
        {
            string res = "";
            try
            {
                byte[] encrypted = System.Text.Encoding.Unicode.GetBytes(pNumber.ToString());
                res = Convert.ToBase64String(encrypted);
            }
            catch (Exception ex)
            {
                //ERROR   
            }
           
            return res;
        }
        public string DecryptBase64(string pCad)
        {
            string res = "";

            if (pCad.Length == 0)
                return "0";
            try
            {
                byte[] decrypted = Convert.FromBase64String(pCad);
                res = System.Text.Encoding.Unicode.GetString(decrypted);
            }
            catch (Exception ex)
            {
                res = "";
            }
            

            return res;
        }

        //EXTRAER EL NUMERO DE USUARIOS DESDE LA LICENCIA
        public int GetUserCount(string pCodigo)
        {
            int numb = 0;
            string decrypt = "";
            if (pCodigo.Length > 0)
            {
                try
                {
                    //USAMO SPLIT DE ACUERDO A CARACTER '&'
                    string[] codes = pCodigo.Split('@');
                    if (codes.Length > 0)
                    {
                        //MANIPULAMOS EL ULTIMO ELEMENTO DEL ARREGLO
                        decrypt = DecryptBase64(codes[codes.Length - 1].ToString());
                        numb = Convert.ToInt32(decrypt);
                    }
                }
                catch (Exception ex)
                {
                    //ERROR
                    numb = 0;
                }
               
            }

            return numb;
        }

        //ENCRIPTAR ARCHIVO 
        public void EncriptarArchivo(string InputFile, string OutputFile, string KeyEncrypt)
        {
            //PARA LECTURA DE ARCHIVO
            FileStream streamInput = new FileStream(InputFile, FileMode.Open, FileAccess.Read);
            //ARCHIVO ENCRIPTADO
            FileStream streamEncrypted = new FileStream(OutputFile, FileMode.Create, FileAccess.Write);

            //ALGORITMO ENCRIPTACION
            DESCryptoServiceProvider Des = new DESCryptoServiceProvider();

            //GENERAMOS CLAVE Y VECTOR DE INICIALIZACION...
            Des.Key = ASCIIEncoding.ASCII.GetBytes(KeyEncrypt);
            Des.IV = ASCIIEncoding.ASCII.GetBytes(KeyEncrypt);

            //CREAMOS CIFRADOS
            ICryptoTransform desencrypt = Des.CreateEncryptor();

            //ENCRIPTAMOS STREAM
            CryptoStream cryptostream = new CryptoStream(streamEncrypted, desencrypt, CryptoStreamMode.Write);

            //LEER EL TEXTO DEL ARCHIVO EN LA MATRIZ DE BYTES
            byte[] bytearrayinput = new byte[streamInput.Length - 1];
            streamInput.Read(bytearrayinput, 0, bytearrayinput.Length);

            //ESCRIBIR EL ARCHIVO CIFRADO CON DES
            cryptostream.Write(bytearrayinput, 0, bytearrayinput.Length);
            cryptostream.Close();
            streamInput.Close();
            streamEncrypted.Close();
        }

        //DESENCRIPTAR ARCHIVO
        public void DesencriptarArchivo(string InputFile, string OutputFile, string KeyEncrypt)
        {
            DESCryptoServiceProvider Des = new DESCryptoServiceProvider();

            //ESTABLECER LA CLAVE Y VECTOR DE INICIALIZACION
            Des.Key = ASCIIEncoding.ASCII.GetBytes(KeyEncrypt);
            Des.IV = ASCIIEncoding.ASCII.GetBytes(KeyEncrypt);

            FileStream fsRead = new FileStream(InputFile, FileMode.Open, FileAccess.Read);

            //CREAMOS DESCRIPTOR
            ICryptoTransform desdecrypt = Des.CreateDecryptor();

            CryptoStream cryptostreamDecr = new CryptoStream(fsRead, desdecrypt, CryptoStreamMode.Read);
            //IMPRIMIR EL CONTENIDO DE ARCHIVO DESCIFRADO
            StreamWriter fsDecrypted = new StreamWriter(OutputFile);

            //LECTURA STREAM
            using (StreamReader reader = new StreamReader(cryptostreamDecr))
            {
                string Line = reader.ReadLine();
                while ((Line = reader.ReadLine()) != null)
                {
                    //GUARDAMOS LINEA EN FSDECRYPTed
                    fsDecrypted.Write(Line);
                }
            }

            fsDecrypted.Flush();
            fsDecrypted.Close();
            fsRead.Close();
        }

        //ENCRIPTAR USANDO TripleDesCryptoServiceProvider
        public string EncriptaTripleDesc(string pCadena)
        {
            string enc = "";

            if (pCadena.Length == 0) return "";

            byte[] llaveCifrado;
            byte[] arreglo = UTF8Encoding.UTF8.GetBytes(pCadena);

            try
            {
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                llaveCifrado = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(this.Key));
                md5.Clear();

                TripleDESCryptoServiceProvider triple = new TripleDESCryptoServiceProvider();
                triple.Key = llaveCifrado;
                triple.Mode = CipherMode.ECB;
                triple.Padding = PaddingMode.PKCS7;
                //INICIAMOS LA CONVERSION DE LA CADENA
                ICryptoTransform convertir = triple.CreateEncryptor();

                //ARREGLO DONDE GUARDAREMOS LA CADENA CIFRADA
                byte[] resultado = convertir.TransformFinalBlock(arreglo, 0, arreglo.Length);

                triple.Clear();

                //CONVERTIMOS A BASE 64
                enc = Convert.ToBase64String(resultado, 0, resultado.Length);

                //AGREGAMOS PREFIJO
            }
            catch (Exception ex)
            {
                //ERROR        
                enc = "";
            }
            
            return enc;
        }

        //DESENCRIPTAR USANDO TRIPLEDESC
        public string DesencriptaTripleDesc(string pCadena)
        {
            string res = "";

            byte[] llave;

            if (pCadena.Length == 0) return "";           

            try
            {
                //CONVERTIMOS ARRAY DESDE LA CADENA QUE VIENE EN BASE 64
                byte[] arreglo = Convert.FromBase64String(pCadena);
                
                //CIFRAMOS CON MD5
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();                
                llave = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(this.Key));
                md5.Clear();

                TripleDESCryptoServiceProvider triple = new TripleDESCryptoServiceProvider();
                triple.Key = llave;
                triple.Mode = CipherMode.ECB;
                triple.Padding = PaddingMode.PKCS7;
                ICryptoTransform convertir = triple.CreateDecryptor();
                byte[] resultado = convertir.TransformFinalBlock(arreglo, 0, arreglo.Length);
                triple.Clear();

                //PARA SABER SI EL TAMAÑO ESPECIFADO DE LA CADENA ES VALIDO PARA EL ALGORITMO ACTUAL
                //SI RETORNA TRUE ES VALIDO
                //if(triple.ValidKeySize(llave.Length))

                res = UTF8Encoding.UTF8.GetString(resultado);
            }
            catch (Exception ex)
            {
                //ERROR...
                res = "";
            }          

            return res;
        }

        //QUITAR PREFIX
        public string RemovePrefix(string pCadena)
        {
            string cad = "";
            if (pCadena.Length > 0)
            {
                //PREGUNTAMOS SI LA CADENA COMIENZA CON EL PREFIJO
                if (pCadena.StartsWith(this.Prefix))
                {
                    //REEMPLAZAMOS PREFIX POR ""
                    pCadena = (pCadena.Replace(this.Prefix, String.Empty)).Trim();
                }
                else
                {
                    //DEJAMOS LA CADENA TAL CUAL
                    cad = pCadena;
                }
            }

            return cad;
        }
    }
}
