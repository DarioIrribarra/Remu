using DevExpress.XtraEditors;
using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReportesExternos
{
    public class fnSistema
    {
        /// <summary>
        /// Nombre del servidor.
        /// </summary>
        public static string pgServer;
        /// <summary>
        /// Nombre de la base de datos.
        /// </summary>
        public static string pgDatabase;
        /// <summary>
        /// Nombre de usuario.
        /// </summary>
        public static string pgUser;
        /// <summary>
        /// Contraseña usuario base de datos.
        /// </summary>
        public static string pgPass;
        /// <summary>
        /// Agrega puntos y guion a una cadena de entrada que representa un rut.
        /// <para>Ej: 174536007 -> 17.453.600-7</para>
        /// </summary>
        /// <param name="pRut">Cadena a evaluar.</param>
        /// <returns></returns>
        /// 
        public static string fFormatearRut2(string pRut)
        {

            if (pRut == "") return "";
            //RECORRER CADENA Y DEJAR EN FORMATO XXX.XXX.XXX-X
            //CADENA TIENE COMO MAXIMO 12 CARACTERES (INCLUIDO PUNTOS Y GUION)
            //123 456 789 123
            //REGLAS
            /*
             *@1 ANTES DEL ULTIMO ELEMENTO SIEMPRE VA UN GUION
             *@2 CADA 3 ELEMENTOS VA UN PUNTO DESDE DERECHA A IZQUIERDA Y SIN CONTAR EL GUION             
             */
            int original = pRut.Length;
            string digito = pRut.Substring(original - 1, 1);
            //EXTRAEMOS CADENA SIN EL ULTIMO DIGITO (REPRESENTA A EL DIGITO VERIFICADOR)
            string subcadena = pRut.Substring(0, pRut.Length - 1);
            string[] invertir = new string[] { };
            //guardamos el largo de la subcadena
            int largo = subcadena.Length;
            string cad = "";
            string cadFinal = "";
            string res = "";
            int multiplo = 0;
            //RECORREMOS CADENA DE FORMA INVERSA
            for (int i = largo - 1; i >= 0; i--)
            {
                cad = cad + subcadena[i];
            }

            //RECORREMOS NUEVAMENTE LA CADENA PARA ENCONTRAR EL MAYOR MULTIPLO DE 3
            for (int i = 0; i < cad.Length; i++)
            {
                //SI ES MULTIPLO DE 3
                if (i % 3 == 0)
                {
                    multiplo = i;
                }
            }

            for (int i = 0; i < cad.Length; i++)
            {
                if (i % 3 == 0)
                {
                    if (i < multiplo)
                    {
                        cadFinal = cadFinal + cad.Substring(i, 3) + ".";
                    }
                    else
                    {

                        cadFinal = cadFinal + cad.Substring(i, (cad.Length) - i);
                    }
                }
            }

            for (int i = (cadFinal.Length) - 1; i >= 0; i--)
            {
                res = res + cadFinal[i];
            }
            //agregamos digito verificador 
            res = res + "-" + digito;

            return res;
        }

        /// <summary>
        /// Verifica si una cadena de entrada es un numero decimal.
        /// </summary>
        /// <param name="input">Cadena a evaluar.</param>
        /// <returns></returns>
        public static bool IsDecimal(string input)
        {
            double test;
            return double.TryParse(input, out test);
        }

        /// <summary>
        /// Abre una conexion a la base de datos y devuelve un objeto sqlconnection.
        /// <para>Util para procesos en otros hilos de ejecución.</para>
        /// </summary>
        /// <returns></returns>
        public static SqlConnection OpenConnection()
        {
            string StringCon = String.Format("Server={0};Database={1};User Id={2};Password={3}; MultipleActiveResultSets=True;Application Name='Remu'", pgServer, pgDatabase, pgUser, pgPass);
            SqlConnection con = new SqlConnection();
            try
            {
                con.ConnectionString = StringCon;
                con.Open();
            }
            catch (SqlException ex)
            {
                //ERROR...
                con = null;
            }

            return con;
        }

    }

    /// <summary>
    /// Clase para manipular imagen ingresada en ficha y reportes.
    /// </summary>
    public class Imagen
    {
        public static Image imagen;
        private static Size OriginalSizeImage;
        public static string PathFile = "";

        /// <summary>
        /// Permite al usuario cargar una imagen.
        /// <para>Nota: Retorna un objeto Image.</para>
        /// </summary>
        /// <returns></returns>
        public static Image GenerarImagenFromUser()
        {
            OpenFileDialog ventana = new OpenFileDialog();
            /*FILTRO FORMATO*/
            ventana.Filter = "*.bmp;*.gif;*.jpg;*.png|*.bmp;*.gif;*.jpg;*.png";
            /*TITULO VENTANA*/
            ventana.Title = "Seleccione una imagen";

            if (ventana.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    //GENERAMOS IMAGEN EN BASE A IMAGEN CARGADA
                    imagen = Image.FromFile(ventana.FileName);

                    //GUARDAMOS RUTA DE ARCHIVO
                    PathFile = ventana.FileName;

                    //RETORNAMOS IMAGEN...
                    return imagen;
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return null;

                }

            }

            return null;
        }

        /*CARGAR IMAGEN EN PICTURE BOX*/
        /// <summary>
        /// Carga una imagen en un control PictureBox.
        /// </summary>
        /// <param name="picture">Representa la imagen.</param>
        /// <param name="pControl">Control picturebox.</param>
        public static void CargarImagen(Image picture, PictureEdit pControl)
        {
            if (picture != null)
            {
                int Ancho = 0, Alto = 0;

                Ancho = picture.Width;
                Alto = picture.Width;

                //CARGAMOS IMAGEN EN CONTROL...
                pControl.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;
                pControl.Image = picture;
                OriginalSizeImage = new Size(Ancho, Alto);
            }
        }

        /*MIMETYPE*/
        /// <summary>
        /// Obtiene el mime correspondiente a una imagen de acuerdo a su extension.
        /// </summary>
        /// <param name="ext">Extension archivo.</param>
        /// <returns></returns>
        private static string GetMimeType(string ext)
        {
            switch (ext.ToLower())
            {
                case ".bpm":
                case ".dib":
                case ".rle":
                    return "ima/bmp";

                case ".jpg":
                case ".jpeg":
                case ".jpe":
                case ".fif":
                    return "image/jpeg";

                case "gif":
                    return "image/gif";

                case ".tif":
                case ".tiff":
                    return "image/tiff";

                case "png":
                    return "image/png";
                default:
                    return "image/jpeg";
            }
        }

        /*OBTENER EXTENSION IMAGEN*/
        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageDecoders();

            ImageCodecInfo encoder = (from enc in encoders where enc.MimeType == mimeType select enc).First();
            return encoder;
        }

        /*COMPRIMIR IMAGEN*/
        /// <summary>
        /// Comprime una imagen y la almacena de forma temporal en el disco local.
        /// </summary>
        /// <param name="InputFile">Ruta del archivo.</param>
        /// <param name="OutputFile">Ruta donde se guardará la imagen comprimida.</param>
        /// <param name="compression">Factor de compresion</param>
        /// <param name="Ancho">Ancho imagen.</param>
        /// <param name="Largo">Largo imagen.</param>
        public static void ComprimirImagen(string InputFile, string OutputFile, long compression, int Ancho, int Largo)
        {
            string mimeType = "";

            //GENERAMOS UN BITMAP DE ACUERDO A ARCHIVO DE ENTRADA
            Bitmap b = new Bitmap(InputFile);
            Bitmap resize = new Bitmap(b, new Size(Ancho, Largo));

            EncoderParameters eps = new EncoderParameters(1);

            eps.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, compression);
            mimeType = GetMimeType(new FileInfo(InputFile).Extension);
            ImageCodecInfo ici = GetEncoderInfo(mimeType);

            //GUARDAMOS BITMAP 
            //OUTPUT FILE REPRESENTA LA RUTA 
            resize.Save(OutputFile, ici, eps);

            //LIBERAMOS RECURSOS
            resize.Dispose();
            b.Dispose();
        }

        /*CROP IMAGEN*/
        /// <summary>
        /// Permite recortar una imagen.
        /// </summary>
        /// <param name="pControl"></param>
        /// <param name="crop"></param>
        /// <param name="PathFile"></param>
        public static void CropImagen(PictureEdit pControl, Rectangle crop, string PathFile)
        {
            try
            {
                Bitmap ImagenOriginal = new Bitmap(pControl.Image, pControl.Width, pControl.Height);
                //CROP REPRESENTA LA IMAGEN RECORTADA
                Bitmap ImagenRecortada = new Bitmap(crop.Width, crop.Height);

                /*PARA DIBUJAR DENTRO DEL CONTROL*/
                Graphics g = Graphics.FromImage(ImagenRecortada);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                //DIBUJAMOS...
                g.DrawImage(ImagenOriginal, 0, 0, crop, GraphicsUnit.Pixel);

                //GUARDAMOS IMAGEN RECORTADA EN DISCO 
                ImagenRecortada.Save(PathFile + "empleado.bmp", ImageFormat.Bmp);

                pControl.Image = ImagenRecortada;
                pControl.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;

                ComprimirImagen(PathFile + "empleado.bmp", PathFile + "empleado_resize.jpg", 90, 300, 300);

                ImagenOriginal.Dispose();
            }
            catch (Exception)
            {
                //ERROR...
            }
        }

        /*OBTENER IMAGEN DESDE BD*/
        /// <summary>
        /// Devuelve el logo de la empresa almacenado en bd.
        /// </summary>
        /// <returns></returns>
        public static Image GetLogoFromBd()
        {
            Image img = null;

            string sql = "SELECT logo FROM empresa";
            SqlCommand cmd;
            SqlDataReader rd;
            SqlConnection cn;
            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            rd = cmd.ExecuteReader();
                            if (rd.HasRows)
                            {
                                while (rd.Read())
                                {
                                    if (rd["logo"] as byte[] != null)
                                        img = GetImageFromStream((byte[])rd["logo"]);
                                }
                            }
                        }
                        cmd.Dispose();
                        rd.Close();
                    }
                }

            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return img;
        }


        /// <summary>
        /// Genera una imagen en blanco en el caso de que no se haya ingresado logo empresa.
        /// </summary>
        /// <param name="picture">Control XRPictureBox reporte.</param>
        public static void SinImagen(XRPictureBox picture)
        {
            picture.Image = null;
            picture.Sizing = DevExpress.XtraPrinting.ImageSizeMode.StretchImage;
        }

        /*GENERAR BYTE DESDE IMAGEN*/
        /// <summary>
        /// Genera un arreglo byte[] desde un imagen.
        /// </summary>
        /// <param name="pControl">PictureBox control donde está cargada la imagen.</param>
        /// <returns></returns>
        public static byte[] GetStreamFromImage(PictureBox pControl)
        {
            Image img = pControl.Image;

            MemoryStream memory = new MemoryStream();

            img.Save(memory, ImageFormat.Png);

            //GUARDAMOS EN ARREGLO
            byte[] imageBt = memory.ToArray();
            return imageBt;
        }

        /*GUARDAR IMAGEN EN BD*/
        /// <summary>
        /// Guarda en bd la representacion en binario de una imagen.
        /// </summary>
        /// <param name="pathFle">Ruta del archivo.</param>
        /// <returns></returns>
        public static byte[] GuardarImagenBd(string pathFile)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                FileStream fs = new FileStream(pathFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                ms.SetLength(fs.Length);
                fs.Read(ms.GetBuffer(), 0, (int)fs.Length);

                byte[] arrImg = ms.GetBuffer();
                ms.Flush();
                fs.Close();

                return arrImg;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //GENERAR IMAGEN DESDE BD
        /// <summary>
        /// Genera la correspondiente imagen desde su representacion en binario.
        /// </summary>
        /// <param name="img">Imagen.</param>
        /// <returns></returns>
        public static Image GetImageFromStream(byte[] img)
        {
            MemoryStream memo = new MemoryStream(img);
            Image imagenFinal = Image.FromStream(memo);

            return imagenFinal;
        }

        /*LIMPIAR DIBUJO EN PICTURE EDIT*/
        /// <summary>
        /// Limpia una figura pintada encima de un control PictureBox.
        /// </summary>
        /// <param name="draw">Objeto Rectangulo.</param>
        public static void CleanDraw(Rectangle draw)
        {
            draw.Width = 0;
            draw.Height = 0;
        }

        //ELIMINAR IMAGENES
        public static void CleanImage(string pathFile)
        {
            if (File.Exists(pathFile + "empleado.bmp"))
            {
                File.Delete(pathFile + "empleado.bmp");
            }
            if (File.Exists(pathFile + "empleado_resize.jpg"))
            {
                File.Delete(pathFile + "empleado_resize.jpg");
            }
            if (File.Exists(pathFile + "ConvertImg.jpg"))
            {
                File.Delete(pathFile + "ConvertImg.jpg");
            }
        }

        /*GENERAR IMAGEN TRABAJADOR DESDE BD*/
        /// <summary>
        /// Devuelve la imagen almacenada del trabajador.
        /// </summary>
        /// <param name="pContrato">Numero de contrato asociado al trabajador.</param>
        /// <returns></returns>
        public static Image GetImagenTrabajador(string pContrato)
        {
            Image image = null;
            string sql = "select top 1 rutafoto FROM trabajador WHERE contrato = @pContrato AND rutafoto is not null";

            SqlCommand cmd;
            SqlDataReader rd;
            SqlConnection cn;
            try
            {
                cn = fnSistema.OpenConnection();
                if (cn != null)
                {
                    using (cn)
                    {
                        using (cmd = new SqlCommand(sql, cn))
                        {
                            //PARAMETROS
                            cmd.Parameters.Add(new SqlParameter("@pContrato", pContrato));

                            rd = cmd.ExecuteReader();
                            if (rd.HasRows)
                            {
                                while (rd.Read())
                                {
                                    if (rd["rutafoto"] as byte[] != null)
                                        image = GetImageFromStream((byte[])rd["rutafoto"]);
                                }
                            }
                            cmd.Dispose();
                            rd.Close();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            return image;
        }
    }
}
