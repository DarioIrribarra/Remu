using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using DevExpress.XtraEditors;
using System.Data;


namespace Labour
{
    class Expresion
    {
        //OPERADORES --> + - * / 
        //OPERADORES LOGICOS --> '=' '>' '<' '>=' '<='
        //LOGICA --> 'IF' 'ELSE'

        /*
         * ESTRUCTURA LOGICA
         * -------------------------------------------------------------------------------------
         * SI[CONDICION X](SI ES VERDAD REALIZA OPERACION1, SINO ES VERDAD REALIZA OPERACION 2)
         *        !CONDICION!           !VERDADERO!            !FALSO!
         * EJ: SI(SUELDO BASE >=270000)(LIQUIDO = 270000X0.65, LIQUIDO = 270000/2)
         * -------------------------------------------------------------------------------------
         */

        /// <summary>
        /// Representa la expresion de entrada.
        /// </summary>
        private string CadenaOriginal = "";

        private char CorcheteAbre = '[';
        private char CorcheteCierra = ']';
        //private char PatentesisAbre = '(';
        //private char ParentesisCierra = ')';
        //private char Coma = ',';

        /// <summary>
        /// Lista con operadores matemáticos válidos.
        /// </summary>
        private char[] Operadores = new char[] { '+', '-', '*', '/', '^' };
        /// <summary>
        /// Lista con operadores lógicos válidos.
        /// </summary>
        private string[] Logicos = new string[] { ">", "<", "=", ">=", "<=" };
        //OPERADORES AND Y OR
        private string[] TablaVerdad = new string[] { "AND", "OR" };
        //CADENA RESULTANTE
        private string ExpresionResultante;
        /// <summary>
        /// Representa la expresion logica para el caso de expresion que contengan una funcion IF.
        /// </summary>
        private string ExpresionLogica;
        /// <summary>
        /// Representa porcion de la funcion IF que representa la parte verdadera.
        /// </summary>
        private string ExpresionVerdadera;
        /// <summary>
        /// Representa porcion de la funcion IF que representa la parte falsa.
        /// </summary>
        private string ExpresionFalsa;
        //PARA PARTE MATEMATICA
        private string ExpresionMatematica = "";
        //VARIABLE INGRESADA
        private double Variable;
        //PARA ACUMULAR VALOR CALCULADO
        private double Acumular = 0;

        //VARIABLE PARA GUARDAR EL CALCULO FINAL DE ACUERDO A EVALUACION DE PARTE LOGICA
        private double ResultadoFormula = 0;

        /// <summary>
        /// Lista con caracteres no permitidos dentro una fórmula.
        /// </summary>
        private char[] NoValidos = new char[] { '!', '°', '"', '#', '$', '%', '&', '?', '¡', '~', '`', '-', '_' };

        /// <summary>
        /// Representa la expresion correspondiente a una funcion MINMAX
        /// </summary>
        private string CadenaMinMax = "";

        //VARIABLES PARA GUARDAR ANTERIOR Y POSTERIOR EN FUNCION MIN MAX
        private string AnteriorMinMax = "";
        private string PosteriorMinMax = "";

        //VARIABLE PARCIAL PARA ANALISIS 
        private string ParcialMinMax = "";

        //VARIABLE PARA OBTENER EL VALOR CALCULO MINMAX
        private double CalculadoMinMax = 0;

        //PARA FORMULA WHILE
        private string ExpresionWhile = "";

        //PARTE OPERATORIA WHILE
        private string OperatoriaWhile = "";

        //PARTE LOGICA WHILE
        private string LogicaWhile = "";

        /// <summary>
        /// Representa el numero de contrato al cual está asociado el item y su correspondiente formula de calculo.
        /// </summary>
        private string Contrato = "";

        /// <summary>
        /// Representa el operador lógico dentro de la expresion logica en una funcion IF.
        /// </summary>
        private string OperadorLogico = "";

        //LOGICA ANTERIOR Y POSTERIOR AL OPERADOR
        private string AntesOperador = "";
        private string DespuesOperador = "";

        /// <summary>
        /// Representa el codigo del item del trabajador que se desea evaluar.
        /// </summary>
        private string ItemTrabajador = "";

        /// <summary>
        /// Representa el valor de una funcion de sistema de tipo sys.
        /// </summary>
        private double SysValor = 0;

        /// <summary>
        /// Representa el numero del item de trabajador que se está evaluando.
        /// </summary>
        private int NumeroItem = 0;

        /// <summary>
        /// Representa el periodo al cual pertenece el item de trabajador.
        /// </summary>
        private int PeriodoEmpleado = 0;

        #region "CONSTRUCTORES"
        public Expresion()
        {
            //CONSTRUCTOR SIN PARAMETROS
        }
        public Expresion(string cadena)
        {
            CadenaOriginal = cadena;
        }
        #endregion

        //SET Y GET          
        #region "GETTER AND SETTER"

        /// <summary>
        /// Setea el valor de la expresion entrante en la variable CadenaOriginal.
        /// </summary>
        /// <param name="cadena">Expresion que representa una fórmula</param>
        private void SetCadenaOriginal(string cadena)
        {
            CadenaOriginal = cadena;
        }

        /// <summary>
        /// Devuelve la expresion que representa la fórmula.
        /// </summary>
        /// <returns></returns>
        private string GetCadenaOriginal()
        {
            return CadenaOriginal;
        }
        /// <summary>
        /// Almacena la expresion de fórmula.
        /// </summary>
        /// <param name="ExpresionResultante"></param>
        public void SetExpresionResultante(string ExpresionResultante)
        {
            this.ExpresionResultante = ExpresionResultante;
        }

        /// <summary>
        /// Retorna la expresion de fórmula.
        /// </summary>
        /// <returns></returns>
        public string GetExpresionResultante()
        {
            if (ExpresionResultante == "") return "";
            return ExpresionResultante;
        }

        /// <summary>
        /// Guarda parte de expresion de fórmula que representa la parte lógica en el caso de un expresion que incluye IF
        /// </summary>
        /// <param name="ExpresionLogica">Representa expresión lógica.</param>
        public void SetExpresionLogica(string ExpresionLogica)
        {
            this.ExpresionLogica = ExpresionLogica;
        }

        /// <summary>
        /// Devuelve el valor almacenado correspondiente a expresion lógica
        /// </summary>
        /// <returns></returns>
        public string GetExpresionLogica()
        {
            if (ExpresionLogica == "") return "";

            return ExpresionLogica;
        }

        /// <summary>
        /// Guarda porcíon verdadera para el caso de funciones que incluyan IF
        /// </summary>
        /// <param name="ExpresionVerdadera">Expresion verdadera.</param>
        public void SetExpresionVerdadera(string ExpresionVerdadera)
        {
            this.ExpresionVerdadera = ExpresionVerdadera;
        }

        /// <summary>
        /// Devuelve porción verdadera para el caso de funciones IF
        /// </summary>
        /// <returns></returns>
        public string GetExpresionVerdadera()
        {
            if (ExpresionVerdadera == "") return "";

            return ExpresionVerdadera;
        }

        /// <summary>
        /// Guarda porcion de la fórmula que representa la parte falsa, para el caso de funciones IF
        /// </summary>
        /// <param name="ExpresionFalsa">Expresion falsa.</param>
        public void SetExpresionFalsa(string ExpresionFalsa)
        {
            this.ExpresionFalsa = ExpresionFalsa;
        }

        /// <summary>
        /// Devuelve expresion que representa la parte falsa, para el caso de funciones IF
        /// </summary>
        /// <returns></returns>
        public string GetExpresionFalsa()
        {
            if (ExpresionFalsa == "") return "";

            return ExpresionFalsa;
        }

        //SETEA EL VALOR DE VARIABLE
        public void SetVariable(double Variable)
        {
            this.Variable = Variable;
        }

        //OBTENER EL VALOR DE LA VARIABLE
        public double GetVariable()
        {
            if (Variable.ToString() == "") return 0.0;
            return Variable;
        }

        /// <summary>
        /// Almacena porcion de la cadena que corresponde a las expresiones matematicas a evaluar, parte verdadera y parte false.
        /// </summary>
        /// <param name="mat">Expresion a guardar</param>
        public void SetExpresionMatematica(string mat)
        {
            ExpresionMatematica = mat;
        }

        /// <summary>
        /// Retorna porcion de la cadena correspondiente a las expresiones matematicas a evaluar, parte verdadera y parte falsa.
        /// </summary>
        /// <returns></returns>
        public string GetExpresionMatematica()
        {
            if (ExpresionMatematica.Length == 0) return "";

            return ExpresionMatematica;
        }

        //SETEA VALOR RESULTADOFORMULA
        public void SetResultadoFormula(double value)
        {
            ResultadoFormula = value;
        }

        //OBTIENE VALOR VARIABLE RESULTADOFORMULA
        public double GetResultadoFormula()
        {
            return ResultadoFormula;
        }        

        //PARA FORMULA MIN MAX
        public void SetCadenaMinMax(string cadena)
        {
            CadenaMinMax = cadena;
        }

        //PARA OBTENER LA CADENA EN CASO DE USAR FORMULA MIN MAX
        public string getCadenaMinMax()
        {
            return CadenaMinMax;
        }

        //PARA GUARDAR LA PARTE ANTES DE LA COMA EN FORMULA MIN MAX
        public void SetAnteriorMinMax(string anterior)
        {
            AnteriorMinMax = anterior;
        }

        //PARA OBTENER LA PARTE ANTES DEL ; EN FORMULA MIN MAX
        public string GetAnteriorMinMax()
        {
            return AnteriorMinMax;
        }
        //PARA SETEAR LA PARTES POSTERIOR AL ; DE LA FORMULA MIN MAX
        public void SetPosteriorMinMax(string posterior)
        {
            PosteriorMinMax = posterior;
        }
        //PARA OBTENER CADENA QUE REPRESENTA PARTE POSTERIOR MINMAX (DESPUES DE ;)
        public string GetPosteriorMinMax()
        {
            return PosteriorMinMax;
        }

        //PARA GUARDAR CADENA COMPLETA PARA LUEGO EVALUAR
        public void SetParcialMinMax(string parcial)
        {
            ParcialMinMax = parcial;
        }

        //PARA OBTENER CADENA COMPLETA 
        public string GetParcialMinMax()
        {
            return ParcialMinMax;
        }

        /// <summary>
        /// Almacena el resulatado final al evaluar la función MinMax
        /// </summary>
        /// <param name="value"></param>
        public void setCalculadoMinMax(double value)
        {
            CalculadoMinMax = value;
        }
        /// <summary>
        /// Retorna el resultado de evaluar la función MinMax
        /// </summary>
        /// <returns></returns>
        public double getCalculadoMinMax()
        {
            return CalculadoMinMax;
        }

        //GUARDAR CADENA FORMATO WHILE
        public void setExpresionWhile(string expr)
        {
            ExpresionWhile = expr;
        }

        //OBTENER VALOR VARIABLE
        public string getExpresionWhile()
        {
            return ExpresionWhile;
        }

        //OPERATORIA WHILE
        public void setOperatoriaWhile(string expr)
        {
            OperatoriaWhile = expr;
        }

        //OBTENER VALOR OPERATORIA (CADENA)
        public string getOperatoriaWhile()
        {
            return OperatoriaWhile;
        }

        //LOGICA WHILE
        public void setLogicaWhile(string Expr)
        {
            LogicaWhile = Expr;
        }

        public string getLogicaWhile()
        {
            return LogicaWhile;
        }

        /// <summary>
        /// Permite almacenar el numero de contrato asociado al trabajador.
        /// Nos permite saber a que contrato está asociado el codigo del item.
        /// </summary>
        /// <param name="contrato">Numero de contrato asociado al trabajador.</param>
        public void setContrato(string contrato)
        {
            this.Contrato = contrato;
        }
        /// <summary>
        /// Retorna el numero de contrato.
        /// </summary>
        /// <returns></returns>
        public string getContrato()
        {
            return Contrato;
        }
        //PARA GMANEJAR EL VALOR DEL OPERADOR LOGICO (>, <, >=, <=, =)
        /// <summary>
        /// Almacena el operador lógico de la expresion.
        /// <para>Ej:[x+1>=y+100]</para>
        /// <para>Extrae >= </para>
        /// </summary>
        /// <param name="Operador">Cadena que representa un operador lógico.</param>
        public void SetOperadorLogico(string Operador)
        {
            OperadorLogico = Operador;
        }
        /// <summary>
        /// Retorna el operador lógico.
        /// </summary>
        /// <returns></returns>
        public string GetOperadorLogico()
        {
            return OperadorLogico;
        }
        /// <summary>
        /// Guarda la expresion correspondiente a la parte izquierda de una expresion lógica.
        /// <para>Ej:x>y</para>
        /// <para>Extrae x</para>
        /// </summary>
        /// <param name="Antes">Cadena a guardar.</param>
        public void SetAntesOperador(string Antes)
        {
            AntesOperador = Antes;
        }
        /// <summary>
        /// Retorna la expresion correspondiente a la parte izquierda de una expresion lógica.
        /// </summary>
        /// <returns></returns>
        public string GetAntesOperador()
        {
            return AntesOperador;
        }
        /// <summary>
        /// Guarda la expresion correspondiente a la parte derecha de una expresion lógica.
        /// <para>Ej: x>y</para>
        /// <para>Extrae y</para>
        /// </summary>
        /// <param name="Despues">Cadena a guardar.</param>
        public void setDespuesOperador(string Despues)
        {
            DespuesOperador = Despues;
        }
        /// <summary>
        /// Retorna la expresion correspondiente a la parte derecha de una expresion lógica.
        /// </summary>
        /// <returns></returns>
        public string getDespuesOperador()
        {
            return DespuesOperador;
        }
        /// <summary>
        /// Permite almacenar el codigo del item del trabajador al cual está asociada la expresion de fórmula.
        /// </summary>
        /// <param name="itemtrabajador"></param>
        public void setItemTrabajador(string itemtrabajador)
        {
            ItemTrabajador = itemtrabajador;
        }
        /// <summary>
        /// Retorna el codigo del item de trabajador almacenado.
        /// </summary>
        /// <returns></returns>
        public string getItemTrabajador()
        {
            return ItemTrabajador;
        }
        //PARA MANIPULAR EL VALOR DE LA VARIABLE DE SISTEMA DE TIPO 1
        public void setSysValor(double valor)
        {
            SysValor = valor;
        }
        public double getSysValor()
        {
            return SysValor;
        }

        /// <summary>
        /// Permite guardar el numero del item el cual usará la fórmula.
        /// </summary>
        /// <param name="num"></param>
        public void setNumeroItem(int num)
        {
            NumeroItem = num;
        }

        /// <summary>
        /// Retorna el numero del item al cual está relacionado o el cual va a hacer uso de la fórmula.
        /// </summary>
        /// <returns></returns>
        public int getNumeroItem()
        {
            return NumeroItem;
        }

        /// <summary>
        /// Permite almacenar el periodo al cual pertenece el item del trabajador.
        /// </summary>
        /// <param name="periodo">Periodo item</param>
        public void setPeriodoEmpleado(int periodo)
        {
            PeriodoEmpleado = periodo;
        }

        /// <summary>
        /// Retorna el valor almacenado, correspondiente al periodo del item de trabajador.
        /// </summary>
        /// <returns></returns>
        public int getPeriodoEmpleado()
        {
            return PeriodoEmpleado;
        }
      
        #endregion

        #region "GENERACION DE CADENA PRINCIPAL"
        /*
      * METODO PARA LIMPIAR LA EXPRESION DE ESPACIOS EN BLANCO
      * RETORNA LA CADENA LIMPIA
      */
      /// <summary>
      /// Quita los espacios dentro de una cadena.
      /// </summary>
      /// <returns></returns>
        private string LimpiarExpresion()
        {
            string Expresion = "";            
            Expresion = GetCadenaOriginal();
            int largo = Expresion.Length;
            if (largo == 0) return "";
            string[] CadenaSplit = new string[] { };
            string NuevaCadena = "";
            //RECORREMOS LA CADENA
            if (Expresion.Contains(" "))
            {
                //SEPARAMOS LA CADENA CON SPLIT 
                CadenaSplit = Expresion.Split(' ');
                //RECORREMOS CadenaSplit
                for (int i = 0; i < CadenaSplit.Length; i++)
                {
                    NuevaCadena = NuevaCadena + CadenaSplit[i];
                }
            }
            else
            {
                NuevaCadena = Expresion;
            }
            //RETORNAR CADENA
            return NuevaCadena;
        }

        //PARA GENERAR LA EXPRESION NECESITAMOS PRIMERAMENTE EVALUAR EL STRING
        /*EXPRESION DE ENTRADA:
         * LA EXPRESION DE ENTRADA VIENE DADA DE LA SIGUIENTE MANERA (CONCATENADA)
         * ---------------------------------------
         * @1.CONDICION + 2@.VERDADERO + 3@.FALSO |
         * ---------------------------------------
         * EXPRESION RESULTANTE
         * IF + ['CONDICION'] + '(OPERATORIA SI ES VERDADERA, OPERATORIA SI ES FALSA)'
         * LOS [] Y LOS () DEBEN APARECER DE FORMA AUTOMATICA
         * EL IF SE GENERA DE FORMA AUTOMATICA
         * 
         * SI RETORNA TRUE SE GENERO CORRECTAMENTE LA CADENA
         * SI RETORNA FALSE SE DETECTO ALGUN ERROR DE ENTRADA (FALTA UN CARACTER - NO HAY IF, ETC.)
         */

            /// <summary>
            /// Indica si la cadena de entrada tiene el formato correcto. De ser válida setea las variables correspondientes.
            /// <para>Variables:</para>
            /// <para>->ExpresionFalsa</para>
            /// <para>->ExpresionVerdadera</para>
            /// <para>->ExpresionLogica</para>
            /// </summary>
            /// <returns></returns>
        private bool GenerarCadena()
        {
            int largo = GetCadenaOriginal().Length;
            if (largo == 0) return false;
            SetExpresionResultante(GetCadenaOriginal().Trim());
            string cadena = "", cadLogico = "", cadVerdad = "", cadFalso = "", cadOperatoria = "";
            int poscAbre = 0, poscCierra = 0, pospAbre = 0, pospCierra = 0, posComa = 0;

            //PARA QUE LA CADENA SEA VALIDA DEBEN RETORNAR TRUE LOS SIGUIENTES METODOS:
            //existeif() --> DEBE COMENZAR CON UN IF
            //existecorchete() --> LA PARTE LOGICA DEBE ESTAR CERRADA CON CORCHETES
            //existeparentesis() --> LA PARTE DE OPERACIONES MATEMATICAS DEBEN ESTAR ENCERRADAS POR PARENTESIS
            //existecomas() --> DENTRO DE LA PARTE DE OPERACIONES MATEMATICAS DEBE ESTAR SEPARADAS POR UNA COMA

            if (GetExpresionResultante().Length > 0)
            {
                if (ExisteIf())
                {
                    if (ExisteCorchete())
                    {
                        if (ExisteParentesis())
                        {
                            if (ExisteComa())
                            {
                                cadena = GetExpresionResultante();
                                //MessageBox.Show("asdasd");
                                // MessageBox.Show(cadena);
                                //OBTENEMOS LAS SUBCADENAS CORRESPONDIENTES
                                //PARTE LOGICA - PARTE VERDADERO - PARTE FALSO

                                //OBTENER POSICION DEL LOS CORCHETES
                                poscAbre = cadena.IndexOf("[");
                                poscCierra = cadena.IndexOf("]");

                                //CADENA PARTE LOGICA
                                cadLogico = cadena.Substring(poscAbre + 1, (poscCierra - poscAbre) - 1);

                                //OBTENER POSICION PARENTESIS(PARTE OPERATORIAS)
                                pospAbre = poscCierra + 1;
                                pospCierra = (cadena.Length) - 1;

                                //CADENA PARTE OPERATORIAS (DESDE POSICION DE PARENTESIS DE ABERTURA)
                                cadOperatoria = cadena.Substring(pospAbre, (cadena.Length - pospAbre));

                                //EXTRAEMOS EL PRIMER Y ULTIMO ELEMENTO DE LA CADENA
                                string ope = cadOperatoria.Remove(0, 1);
                                ope = ope.Remove(ope.Length - 1, 1);

                                //OBTENEMOS PARTE VERDADERA Y PARTE FALSA
                                //OBTENEMOS LA POSICION DE LA COMA
                                posComa = ope.IndexOf(",");
                                //PARTE VERDADERA
                                cadVerdad = ope.Substring(0, posComa);

                                //PARTE FALSA
                                cadFalso = ope.Substring(posComa + 1, ((ope.Length - 1) - posComa));

                                //SETEAMOS VARIABLES CORRESPONDIENTES
                                SetExpresionFalsa(cadFalso);
                                SetExpresionVerdadera(cadVerdad);
                                SetExpresionLogica(cadLogico);
                                //SOLO PARA VERIFICAR QUE SE CREARON LAS CADENAS CORRESPONDIENTES
                                return true;
                            }
                        }
                        else
                        {
                            XtraMessageBox.Show("Falta Parentesis en la expresion", "Estructura", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else
                    {
                        XtraMessageBox.Show("Falta corchete en la expresion logica", "Estructura", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    XtraMessageBox.Show("Falta palabra clave", "Estrutura", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            return false;
        }

        //METODO MADRE PARA CUALQUIER TIPO DE CALCULO
        //CON FUNCION IF
        //CON FUNCIONES MINMAX
        //SOLO OPERACIONES MATEMATICAS
        public bool ValidacionSuprema()
        {
            bool ExpresionValida = false;

            if (GetCadenaOriginal() != "")
            {
                if (GetCadenaOriginal().Contains("IF["))
                {
                    //ES UNA FUNCION IF
                    //VALIDAMOS ESTRUCTURA IF
                    ExpresionValida = CalculoIf();
                }
                else if (GetCadenaOriginal().Contains("MIN(") || GetCadenaOriginal().Contains("MAX("))
                {
                    //CONTIENE FUNCIONES MIN MAX
                    //VALIDAR ESTRUCTURA CADENA
                    //if (ValidarMinMax(GetCadenaOriginal()) == false)
                    //{ XtraMessageBox.Show("Tienes un error en la expresión", "Error sintáxis", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }

                    ExpresionValida = EvaluarMinMax();
                }
                else
                {
                    //SOLO ES UNA EXPRESION MATEMATICA
                    ExpresionValida =  ValidaMatematica();
                }
            }            

            //SI EXPRESION VALIDA ES TRUE LA CADENA ES CORRECTA!
            if (ExpresionValida) return true;
            else
                return false;
        }

        //CALCULO SUPREMO
        /// <summary>
        /// Metodo para gestionar el calculo de una formula.        
        /// </summary>
        /// <returns></returns>
        public double CalculoSupremo()
        {
            double resultado = 0;
            //bool validaIf = false;
            if (GetCadenaOriginal() != "")
            {
                if (GetCadenaOriginal().Contains("IF["))
                {
                    //validaIf = CalculoIf();
                    //if(validaIf)
                    SetearCadenasIf();
                    resultado = EvaluaDatos();
                }
                else if (GetCadenaOriginal().Contains("MIN(") || GetCadenaOriginal().Contains("MAX("))
                {
                    resultado = CalculoFinalMinMaxV2();
                }
                else
                {
                    resultado = CalculoSimple();
                }
            }

            return resultado;
        }
        #endregion

        #region "OPERATORIAS PARA EXPRESION LOGICA"        

        //EVALUAR EXPRESION LOGICA
        /*
         * LOS VALORES PARA LA CONDICION LOGICA PUEDEN SER LOS SIGUIENTES
         * '>' MAYOR QUE
         * '<' MENOR QUE
         * '=' IGUAL A
         * '>=' MAYOR IGUAL QUE
         * '<=' MENOR IGUAL QUE
         */
        //SI RETORNA TRUE LA CONDICION ES VERDADERA
        //SI RETORNA FALSE LA CONDICION ES FALSA
        /// <summary>
        /// Valida la expresion logica.
        /// <para>Retorna true si la expresion es válida.</para>        
        /// <para>Retorna false si la expresion no es válida.</para>
        /// </summary>
        /// <returns></returns>
        private bool EvaluarLogica()
        {
            //CADENA DE ENTRADA VIENE --> 'VALOR1>VALOR2'
            //SEPARA CADENA COMO --> |VAR1| |LOGICA| |VAR2|
            int largo = GetExpresionLogica().Length;
            if (largo == 0) return false;            
            string[] Parcial = new string[] { };
            //PARA POSICION DEL OPERADOR LOGICO
            int x = 0;
            int codigoError1 = 0, codigoError2 = 0;
            string MensajeError1 = "", MensajeError2 = "";
            if (largo > 0)
            {
                //PREGUNTAMOS QUE OPERADOR LOGICO CONTIENE
                //Logicos = new string[] { ">", "<", "=", ">=", "<=" };
                if (GetExpresionLogica().Contains(Logicos[0]))
                {
                    //CODE HERE...
                    //EVALUAMOS COMO MAYOR                   
                    //OBTENER LA POSICION DONDE ESTA EL CARACTER '>'
                    x = GetExpresionLogica().IndexOf('>');

                    //SETEAMOS VARIABLE PARA GUARDAR OPERADOR LOGICO
                    SetOperadorLogico(">");

                    //DEVUELVE UN ARRAY (CADA PARTE DE LA CONDICION @antes y @despues del operador logico)
                    Parcial = CadenasLogicas(GetExpresionLogica(), x);

                    //GUARDAR PARTE ANTERIOR Y PARTE POSTERIOR
                    SetAntesOperador(Parcial[0]);
                    setDespuesOperador(Parcial[1]);

                    //VALIDAR QUE LA EXPRESION ANTES Y DESPUES DEL CARACTER LOGICO NO TENGA ERRORES MATEMATICOS
                    codigoError1 = fnCodigoError(Parcial[0]);
                    codigoError2 = fnCodigoError(Parcial[1]);
                    //SI EL CODIGO DE ERROR ES -1 ES PORQUE NO HAY ERRORES MATEMATICOS
                    MensajeError1 = fnMensajeError(codigoError1);
                    MensajeError2 = fnMensajeError(codigoError2);

                    if (codigoError1 == -1 && codigoError2 == -1)
                    {
                        //GENERA LISTA 
                        List<string> listaAnterior = new List<string>();
                        List<string> listaPosterior = new List<string>();

                        List<string> listaValidaPosterior = new List<string>();
                        List<string> listaValidaAnterior = new List<string>();

                        listaAnterior = GeneraListaCadena(Parcial[0]);
                        listaPosterior = GeneraListaCadena(Parcial[1]);

                        //GENERAMOS LISTA VALIDANDO VARIABLES (EN CASO DE EXISTIR)
                        listaValidaPosterior = GeneraListaValida(listaAnterior);
                        listaValidaAnterior = GeneraListaValida(listaPosterior);

                        if (listaValidaPosterior == null)
                        {
                            //XtraMessageBox.Show("Error en expresion logica", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                        else if (listaValidaAnterior == null)
                        {
                            //XtraMessageBox.Show("Error en expresion logica", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }

                        //SI NO ENTRA A LOS IF ES PORQUE ES UNA EXPRESION VALIDA

                        //RETORNAMOS TRUE PORQUE LA EXPRESION ANTERIOR Y POSTERIOR AL OPERADOR LOGICO ES VALIDA
                        return true;                      
                    }
                    else if (codigoError1 != -1)
                    {
                        //MOSTRAMOS ERROR
                        XtraMessageBox.Show(MensajeError1, "Error matematico", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                    else if (codigoError2 != -1)
                    {
                        XtraMessageBox.Show(MensajeError2, "Error matematico", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }

                }
                else if (GetExpresionLogica().Contains(Logicos[1]))
                {
                    //CODE HERE...
                    //EVALUAMOS COMO MENOR QUE
                    x = GetExpresionLogica().IndexOf('<');
                    //SETEAMOS VARIABLE PARA GUARDAR OPERADOR LOGICO
                    SetOperadorLogico("<");
                    Parcial = CadenasLogicas(GetExpresionLogica(), x);

                    //GUARDAR PARTE ANTERIOR Y PARTE POSTERIOR
                    SetAntesOperador(Parcial[0]);
                    setDespuesOperador(Parcial[1]);

                    //VALIDAR QUE LA EXPRESION ANTES Y DESPUES DEL CARACTER LOGICO NO TENGA ERRORES MATEMATICOS
                    codigoError1 = fnCodigoError(Parcial[0]);
                    codigoError2 = fnCodigoError(Parcial[1]);
                    //SI EL CODIGO DE ERROR ES -1 ES PORQUE NO HAY ERRORES MATEMATICOS
                    MensajeError1 = fnMensajeError(codigoError1);
                    MensajeError2 = fnMensajeError(codigoError2);


                    if (codigoError1 == -1 && codigoError2 == -1)
                    {
                        //NO CALCULAR SOLO RETORNAR LISTA VALIDADA (EN CASO DE TENER VARIABLES DE SISTEMA)

                        //GENERA LISTA 
                        List<string> listaAnterior = new List<string>();
                        List<string> listaPosterior = new List<string>();

                        List<string> listaValidaPosterior = new List<string>();
                        List<string> listaValidaAnterior = new List<string>();

                        listaAnterior = GeneraListaCadena(Parcial[0]);
                        listaPosterior = GeneraListaCadena(Parcial[1]);

                        //GENERAMOS LISTA VALIDANDO VARIABLES (EN CASO DE EXISTIR)
                        listaValidaPosterior = GeneraListaValida(listaAnterior);
                        listaValidaAnterior = GeneraListaValida(listaPosterior);

                        if (listaValidaPosterior == null)
                        {
                            XtraMessageBox.Show("Error en expresion logica", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                        else if (listaValidaAnterior == null)
                        {
                            XtraMessageBox.Show("Error en expresion logica", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                        return true;
                    }
                    else if (codigoError1 != -1)
                    {
                        //MOSTRAMOS ERROR
                        XtraMessageBox.Show(MensajeError1, "Error matematico", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                    else if (codigoError2 != -1)
                    {
                        XtraMessageBox.Show(MensajeError2, "Error matematico", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }
                else if (GetExpresionLogica().Contains(Logicos[3]))
                {
                    //CODE HERE...
                    //EVALUAMOS COMO MAYOR O IGUAL QUE
                    x = GetExpresionLogica().IndexOf(">=");
                    //SETEAMOS VARIABLE PARA GUARDAR OPERADOR LOGICO
                    SetOperadorLogico(">=");
                    Parcial = CadenasLogicas(GetExpresionLogica(), x);

                    //GUARDAR PARTE ANTERIOR Y PARTE POSTERIOR
                    SetAntesOperador(Parcial[0]);
                    setDespuesOperador(Parcial[1]);

                    //VALIDAR QUE LA EXPRESION ANTES Y DESPUES DEL CARACTER LOGICO NO TENGA ERRORES MATEMATICOS
                    codigoError1 = fnCodigoError(Parcial[0]);
                    codigoError2 = fnCodigoError(Parcial[1]);
                    //SI EL CODIGO DE ERROR ES -1 ES PORQUE NO HAY ERRORES MATEMATICOS
                    MensajeError1 = fnMensajeError(codigoError1);
                    MensajeError2 = fnMensajeError(codigoError2);


                    if (codigoError1 == -1 && codigoError2 == -1)
                    {
                        //GENERA LISTA 
                        List<string> listaAnterior = new List<string>();
                        List<string> listaPosterior = new List<string>();

                        List<string> listaValidaPosterior = new List<string>();
                        List<string> listaValidaAnterior = new List<string>();

                        listaAnterior = GeneraListaCadena(Parcial[0]);
                        listaPosterior = GeneraListaCadena(Parcial[1]);

                        //GENERAMOS LISTA VALIDANDO VARIABLES (EN CASO DE EXISTIR)
                        listaValidaPosterior = GeneraListaValida(listaAnterior);
                        listaValidaAnterior = GeneraListaValida(listaPosterior);

                        if (listaValidaPosterior == null)
                        {
                            XtraMessageBox.Show("Error en expresion logica", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                        else if (listaValidaAnterior == null)
                        {
                            XtraMessageBox.Show("Error en expresion logica", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }

                        return true;
                    }
                    else if (codigoError1 != -1)
                    {
                        //MOSTRAMOS ERROR
                        XtraMessageBox.Show(MensajeError1, "Error matematico", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                    else if (codigoError2 != -1)
                    {
                        XtraMessageBox.Show(MensajeError2, "Error matematico", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }

                }
                else if (GetExpresionLogica().Contains(Logicos[4]))
                {
                    //CODE HERE...
                    //EVALUAMOS COMO MENOR O IGUAL
                    x = GetExpresionLogica().IndexOf("<=");
                    //SETEAMOS VARIABLE PARA GUARDAR OPERADOR LOGICO
                    SetOperadorLogico("<=");
                    //DEVUELVE UN ARRAY
                    Parcial = CadenasLogicas(GetExpresionLogica(), x);

                    //GUARDAR PARTE ANTERIOR Y PARTE POSTERIOR
                    SetAntesOperador(Parcial[0]);
                    setDespuesOperador(Parcial[1]);

                    //VALIDAR QUE LA EXPRESION ANTES Y DESPUES DEL CARACTER LOGICO NO TENGA ERRORES MATEMATICOS
                    codigoError1 = fnCodigoError(Parcial[0]);
                    codigoError2 = fnCodigoError(Parcial[1]);
                    //SI EL CODIGO DE ERROR ES -1 ES PORQUE NO HAY ERRORES MATEMATICOS
                    MensajeError1 = fnMensajeError(codigoError1);
                    MensajeError2 = fnMensajeError(codigoError2);

                    if (codigoError1 == -1 && codigoError2 == -1)
                    {
                        //GENERA LISTA 
                        List<string> listaAnterior = new List<string>();
                        List<string> listaPosterior = new List<string>();

                        List<string> listaValidaPosterior = new List<string>();
                        List<string> listaValidaAnterior = new List<string>();

                        listaAnterior = GeneraListaCadena(Parcial[0]);
                        listaPosterior = GeneraListaCadena(Parcial[1]);

                        //GENERAMOS LISTA VALIDANDO VARIABLES (EN CASO DE EXISTIR)
                        listaValidaPosterior = GeneraListaValida(listaAnterior);
                        listaValidaAnterior = GeneraListaValida(listaPosterior);

                        if (listaValidaPosterior == null)
                        {
                            XtraMessageBox.Show("Error en expresion logica", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                        else if (listaValidaAnterior == null)
                        {
                            XtraMessageBox.Show("Error en expresion logica", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }

                        return true;
                    }
                    else if (codigoError1 != -1)
                    {
                        //MOSTRAMOS ERROR
                        XtraMessageBox.Show(MensajeError1, "Error matematico", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                    else if (codigoError2 != -1)
                    {
                        XtraMessageBox.Show(MensajeError2, "Error matematico", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }


                }
                else if (GetExpresionLogica().Contains(Logicos[2]))
                {
                    //CODE HERE...
                    //EVALUAMOS COMO IGUAL
                    x = GetExpresionLogica().IndexOf("=");
                    //SETEAMOS VARIABLE PARA GUARDAR OPERADOR LOGICO
                    SetOperadorLogico("=");
                    Parcial = CadenasLogicas(GetExpresionLogica(), x);

                    //GUARDAR PARTE ANTERIOR Y PARTE POSTERIOR
                    SetAntesOperador(Parcial[0]);
                    setDespuesOperador(Parcial[1]);

                    //VALIDAR QUE LA EXPRESION ANTES Y DESPUES DEL CARACTER LOGICO NO TENGA ERRORES MATEMATICOS
                    codigoError1 = fnCodigoError(Parcial[0]);
                    codigoError2 = fnCodigoError(Parcial[1]);
                    //SI EL CODIGO DE ERROR ES -1 ES PORQUE NO HAY ERRORES MATEMATICOS
                    MensajeError1 = fnMensajeError(codigoError1);
                    MensajeError2 = fnMensajeError(codigoError2);

                    if (codigoError1 == -1 && codigoError2 == -1)
                    {
                        //GENERA LISTA 
                        List<string> listaAnterior = new List<string>();
                        List<string> listaPosterior = new List<string>();

                        List<string> listaValidaPosterior = new List<string>();
                        List<string> listaValidaAnterior = new List<string>();

                        listaAnterior = GeneraListaCadena(Parcial[0]);
                        listaPosterior = GeneraListaCadena(Parcial[1]);

                        //GENERAMOS LISTA VALIDANDO VARIABLES (EN CASO DE EXISTIR)
                        listaValidaPosterior = GeneraListaValida(listaAnterior);
                        listaValidaAnterior = GeneraListaValida(listaPosterior);

                        if (listaValidaPosterior == null)
                        {
                            XtraMessageBox.Show("Error en expresion logica", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                        else if (listaValidaAnterior == null)
                        {
                            XtraMessageBox.Show("Error en expresion logica", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }

                        return true;
                    }
                    else if (codigoError1 != -1)
                    {
                        //MOSTRAMOS ERROR
                        XtraMessageBox.Show(MensajeError1, "Error matematico", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                    else if (codigoError2 != -1)
                    {
                        XtraMessageBox.Show(MensajeError2, "Error matematico", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }
                else
                {
                    //NO CONTIENE NINGUN OPERADOR LOGICO VALIDO
                    return false;
                }
            }
            return false;
        }

        //METODO PARCIAL PARA OBTENER LAS VARIABLES DE LA CONDICIONANTE
        //PARAMETRO DE ENTRADA NECESITA LA POSICION DEL OPERADOR LOGICO Y LA CADENA
        /// <summary>
        /// Extrae desde expresion logica la parte izquierda y la parte derecha.
        ///<para>Ej:[x+a>z+b]</para>
        ///<para>Extrae x+a y z+b</para>
        /// </summary>
        /// <param name="Expresion">Expresion lógica.</param>
        /// <param name="Position">Representa la posicion del operador logico dentro de la cadena.</param>
        /// <returns></returns>
        private string[] CadenasLogicas(string Expresion, int Position)
        {
            string subAnterior = "", subPosterior = "";
            string[] arreglo = new string[] { "", "" };
            for (int i = 0; i < Expresion.Length; i++)
            {
                if (i < Position)
                {
                    //SUBCADENA ANTES DEL OPERADOR LOGICO
                    subAnterior = subAnterior + Expresion[i];
                }
                else if (i > Position)
                {
                    subPosterior = subPosterior + Expresion[i];
                }
            }
            //RETORNAMOS LAS CADENAS RESULTANTES (ARRAY)

            arreglo[0] = subAnterior;
            arreglo[1] = subPosterior;
            return arreglo;
        }    

        /// <summary>
        /// Evalua y calcula la expresion en una funcion IF.
        /// </summary>
        /// <returns></returns>
        private double EvaluaDatos()
        {
            string cadena1 = "", cadena2 = "", verdadero = "", falso = "", contrato = "", itemempleado = "";
            double valor1 = 0, valor2 = 0, valVerdad = 0, valFalso = 0;
            double resultado = 0;

            if (getContrato() != "") contrato = getContrato();
            if (getItemTrabajador() != "") itemempleado = getItemTrabajador();

            //PARA CALCULO PARTE VERDADERA Y PARTE FALSA
            List<string> verdad = new List<string>();
            List<string> falsa = new List<string>();

            List<string> verdadValida = new List<string>();
            List<string> falsaValida = new List<string>();
         
            if (GetAntesOperador() != "" && getDespuesOperador() != "" && GetOperadorLogico()!= "")
            {
                //GENERA LISTA 
                List<string> listaAnterior = new List<string>();
                List<string> listaPosterior = new List<string>();

                List<string> listaValidaPosterior = new List<string>();
                List<string> listaValidaAnterior = new List<string>();

                //----------------------------------------------
                // LISTAS PARA PARTES DE EXPRESION LOGICA       |
                //----------------------------------------------

                //*************************************************************************************
                //VERIFICAR QUE ALGUNOS DE LOS FACTORES DE LA EXPRESION LOGICA ES UNA FUNCION MIN MAX
                //*************************************************************************************

                listaAnterior = GeneraListaCadena(GetAntesOperador());
                listaPosterior = GeneraListaCadena(getDespuesOperador());

                //GENERAMOS LISTA VALIDANDO VARIABLES (EN CASO DE EXISTIR)
                listaValidaAnterior = GeneraListaValida(listaAnterior);
                listaValidaPosterior = GeneraListaValida(listaPosterior);

                cadena1 = ListaCalculada(listaValidaAnterior, contrato, itemempleado);
                cadena2 = ListaCalculada(listaValidaPosterior, contrato, itemempleado);
             
                //UNA VEZ OBTENIDAS LAS CADENA DE CALCULO REALIZAMOS CALCULO
                valor1 = CalculoExpresion(cadena1);
                valor2 = CalculoExpresion(cadena2);
                
                //----------------------------------------------
                // LISTAS PARA PARTE VERDADERA Y PARTE FALSA    |
                //----------------------------------------------

                //*******************************************************************************
                // VERIFICAR SI CADENA VERDADERA O CADENA FALSA CONTINENE ALGUNA FUNCION MINMAX
                //*******************************************************************************

                //GENERACION DE LISTAS PARA PARTE VERDADERA Y FALSA
                verdad = GeneraListaCadena(GetExpresionVerdadera());
                falsa = GeneraListaCadena(GetExpresionFalsa());

                verdadValida = GeneraListaValida(verdad);
                falsaValida = GeneraListaValida(falsa);

                //OBTENEMOS CADENA PARA CALCULO FINAL
                verdadero = ListaCalculada(verdadValida, contrato, itemempleado);
                falso = ListaCalculada(falsaValida, contrato, itemempleado);                

                //REALIZAMOS CALCULO MATEMATICO
                valVerdad = CalculoExpresion(verdadero);
                valFalso = CalculoExpresion(falso);               

                if (GetOperadorLogico() == ">")
                {                                      
                    if (valor1 > valor2)
                    {
                        //RETORNAMOS CALCULO PARTE VERDADERA
                        resultado = valVerdad;
                    }
                    else if (valor2 > valor1)
                    {
                        //RETORNAMOS CALCULO PARTE FALSA
                        resultado = valFalso;
                    }
                    else if (valor1 == valor2)
                    {
                        //RETORNE PARTE FALSA
                        resultado = valFalso;
                    }
                }
                else if (GetOperadorLogico() == "<")
                {                  
                    //VALOR 1 EXPRESION IZQUIERDA DE LA EXPRESION LOGICA
                    //VALOR 2 EXPRESION DERECHA DE LA EXPRESION LOGICA
                    if (valor1 < valor2)
                    {
                        //RETORNAMOS CALCULO PARTE VERDADERA
                        resultado = valVerdad;
                    }
                    else if (valor2 < valor1)
                    {
                        //CALCULO PARTE FALSA
                        resultado = valFalso;
                    }
                    else if (valor1 == valor2)
                    {
                        //RETORNA VALOR FALSO
                        resultado = valFalso;
                    }
                }
                else if (GetOperadorLogico() == ">=")
                {
                    if (valor1 >= valor2)
                    {
                        //RETORNAMOS CALCULO PARTE VERDADERA
                        resultado = valVerdad;
                    }
                    else if (valor2 >= valor1)
                    {
                        //RETORNAMOS CALCULO PARTE FALSA
                        resultado = valFalso;
                    }
                }
                else if (GetOperadorLogico() == "<=")
                {
                    if (valor1 <= valor2)
                    {
                        //RETORNAMOS CALCULO PARTE VERDADERA
                        resultado = valVerdad;
                    }
                    else if (valor2 <= valor1)
                    {
                        //RETORNAMOS CALCULO PARTE FALSA
                        resultado = valFalso;
                    }
                }
                else if (GetOperadorLogico() == "=")
                {
                    if (valor1 == valor2)
                    {
                        //RETORNAMOS CALCULO PARTE VERDADERA
                        resultado = valVerdad;
                    }
                    else
                    {
                        //RETORNAMOS CALCULO PARTE FALSA
                        resultado = valFalso;
                    }                    
                }
                
                return Math.Round(resultado);
            }

            return -1;
        }
        #endregion

        #region "ERRORES DE ESTRUCTURA DE LA CADENA"
        //VERIFICAR QUE EXISTA LA PALABRA IF AL COMIENZO DE LA EXPRESION
        //RETORNA TRUE SI EXISTE
        /// <summary>
        /// Nos indica si la expresion incluye correctamente el caracter 'IF', para funciones IF.
        /// <para>IF[exp1>exp2](true,false)</para>
        /// </summary>
        /// <returns></returns>
        private bool ExisteIf()
        {
            int largo = GetExpresionResultante().Length;
            if (largo == 0) return false;
            string cadena = GetExpresionResultante();
            cadena = cadena.ToUpper();
            //recorremos cadena y verificamos que exista al comienzo de la expresion la palabra if
            if (cadena[0] == 'I' && cadena[1] == 'F') return true;
            if (cadena[0] == 'W' && cadena[1] == 'H' && cadena[2] == 'I' && cadena[3] == 'L' && cadena[4] == 'E') return true;
            if (cadena[0] == 'F' && cadena[1] == 'O' && cadena[2] == 'R') return true;

            return false;
        }

        //VERIFICAR QUE LA CONDICION LOGICA ESTE ENCERRADA DENTRO DE []
        //SEA CUAL SEA LA CONDICION IF, WHILE, ETC...
        //RETORNA TRUE SI EXISTE
        //RETORNA FALSE EN CASO DE ERROR
        /// <summary>
        /// Nos indica si la funcion IF incluye correctamente los corchetes de abertura y cierre para expresion lógica.
        /// Si no hay corchetes retorna false, caso contrario retorna true.
        /// <para>IF[x>y]</para>
        /// </summary>
        /// <returns></returns>
        private bool ExisteCorchete()
        {
            int largo = GetExpresionResultante().Length;
            if (largo == 0) return false;
            string cadena = GetExpresionResultante();
            string extraer = "";
            string siguiente = "";
            bool coincide = false;
            string cadenaLogica = "";
            int posAbre = 0, posCierra = 0;
            //SI NO CONTIENE CORCHETES NO VALIDA
            if (!cadena.Contains("[") && !cadena.Contains("]")) return false;
            if (cadena.Contains("[") && !cadena.Contains("]")) return false;
            if (!cadena.Contains("[") && cadena.Contains("]")) return false;
            //contar la cantidad de corchetes que tiene (SOLO UNO DE CIERRE Y UNO DE ABERTURA PERMITIDOS)              

            bool mcorches = CantidadCorchetes(cadena);
           
            if (mcorches) { XtraMessageBox.Show("Cantidad de corchetes no valida", "Corchetes", MessageBoxButtons.OK, MessageBoxIcon.Warning); return false; }
            for (int i = 0; i < largo; i++)
            {
                if (cadena[i] == CorcheteAbre)
                {
                    //corAbre++;
                    posAbre = i;
                }
                else if (cadena[i] == CorcheteCierra)
                {
                    //corCierra++;
                    posCierra = i;
                }

                //if (corAbre == 1 && corCierra == 1) break;
            }

            //VERIFICAR QUE ENTRE LOS CORCHETES HAYA ALGUN DATO
            //EXTRAEMOS CADENA QUE REPRESENTA LOS QUE ESTA ANTES DEL CORCHETE DE ABERTURA '[' 
            extraer = cadena.Substring(0, posAbre);
            //extraer los que esta despues del corchete de cierre
            if (posCierra < cadena.Length - 1)
            {
                siguiente = cadena.Substring(posCierra + 1, 1);
            }

            coincide = Claves(extraer);
            if (coincide && siguiente == "(")
            {
                //SI COINCIDE ES TRUE ES PORQUE LO QUE ESTA ANTES DEL [ ES UNA PALABRA CORRECTA (IF, WHILE, ETC)
                //SI DESPUES DEL CORCHETE HAY UN PARENTESIS DE ABERTURA ES CORRECTA LA SINTAXIS                

                //falta verificar que en la condicion logica exista algun operador de verdad (> < >= <= ==)
                cadenaLogica = cadena.Substring(posAbre, (posCierra - posAbre) + 1);
                bool cantidad = CantidadCorchetes(cadenaLogica);
                if (cantidad) { XtraMessageBox.Show("Error en cantidad de corchetes", "Corchetes", MessageBoxButtons.OK, MessageBoxIcon.Warning); return false; }

                for (int i = 0; i < cadenaLogica.Length; i++)
                {
                    if (cadenaLogica[i] == '[')
                    {
                        if (cadenaLogica[i + 1] == ']')
                        {
                            //NO ES UNA ESTRUCTURA CORRECTA
                            XtraMessageBox.Show("La condicion logica entre los corchetes no es correcta", "Corchetes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                    }
                }

                //VERIFICAR QUE EN LA CADENA LOGICA NO HAYAN CARACTERES RAROS
                // bool CaracteresRaros = CaracteresNoValidos(cadenaLogica, 1);

                //if (CaracteresRaros) {XtraMessageBox.Show("Caracteres no permitidos en cadena logica", "Caracteres", MessageBoxButtons.OK, MessageBoxIcon.Warning); return false;}
                //PREGUNTAMOS SI LA CADENA TIENE ALMENOS UN OPERADOR LOGICO
                if (cadenaLogica.Contains(">") || cadenaLogica.Contains("<") || cadenaLogica.Contains(">=") ||
                     cadenaLogica.Contains("<=") || cadenaLogica.Contains("=") || cadenaLogica.Contains("<>"))
                {

                    //VERIFICAR QUE ESTE AL MEDIO O TENGA CARACTERES ANTES Y DESPUES DENTRO DE LOS CORCHETES
                    string operador = "";
                    int veces = 0;
                    for (int i = 0; i < cadenaLogica.Length; i++)
                    {
                        if ((cadenaLogica[i] == '>' || cadenaLogica[i] == '<' || cadenaLogica[i] == '='))
                        {
                            if (cadenaLogica[i + 1] == '=' && cadenaLogica[i] != '=')
                            {
                                //EN EL CASO DE QUE SEA >= O <=
                                operador = cadenaLogica[i] + "" + cadenaLogica[i + 1];
                                i = i + 1;
                                veces++;
                            }
                            else if (cadenaLogica[i + 1] == '>' && cadenaLogica[i] == '<')
                            {
                                operador = cadenaLogica[i] + "" + cadenaLogica[i + 1];
                                //aumentar la posicion en una de la cadena
                                i = i + 1;
                                veces++;
                            }
                            else if (cadenaLogica[i] == '=')
                            {
                                //SOLO '='
                                operador = cadenaLogica[i].ToString();
                                veces++;
                            }
                            else
                            {
                                operador = cadenaLogica[i].ToString();
                                veces++;
                            }
                        }
                        else if (cadenaLogica[i] == '=' && cadenaLogica[i - 1] != '<' && cadenaLogica[i - 1] != '>')
                        {
                            //SOLO '='
                            operador = cadenaLogica[i].ToString();
                            veces++;
                        }
                    }
                    //MessageBox.Show(operador);
                    //MessageBox.Show(veces.ToString());
                    if (veces > 1) { XtraMessageBox.Show("Hay mas de un operador", "Operador", MessageBoxButtons.OK, MessageBoxIcon.Warning); return false; }
                    //OBTENER LA POSICION DONDE ESTA EL OPERADOR

                    int posOperador = cadenaLogica.IndexOf(operador);
                    if (posOperador == 1 || posOperador == cadenaLogica.Length - 2) { MessageBox.Show("Posicion operador no es correcta"); return false; }
                    //SI NO ENTRA A LOS IF DE ARRIBA ES VALIDA
                    return true;
                }
                else
                {
                    MessageBox.Show("Estructura incorrecta dentro de corchetes", "Corchetes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            return false;

        }

        //CONTAR LA CANTIDAD DE CORCHETES QUE TIENE LA CADENA LOGICA SOLO DOS PERMITIDOS
        /// <summary>
        /// Obtiene la cantidad de corchetes que tiene una cadena.
        /// </summary>
        /// <param name="cadena">Cadena a evaluar.</param>
        /// <returns></returns>
        private bool CantidadCorchetes(string cadena)
        {
            int largo = cadena.Length;
            if (largo == 0) return true;
            int ab = 0, ci = 0;
            for (int i = 0; i < largo; i++)
            {
                if (cadena[i] == '[') ab++;
                if (cadena[i] == ']') ci++;
            }            
            //si alguno es mayor a uno es porque hay mas de un corchete de cierre o abertura
            if (ab > 1 || ci > 1) return true;
            if (ab == 1 && ci == 0) return true;
            if (ab == 0 && ci == 1) return true;
            if (ab == 0 && ci == 0) return true;

            return false;
        }

        //VALIDAR QUE EN CADENA NO EXISTAN CARACTERES NO VALIDOS
        //CARACTERES--> /,.*+-^ (para cadena logica)
        /// <summary>
        /// Nos indica si una cadena tiene caracteres no válidos de acuerdo a un listado.
        /// </summary>
        /// <param name="cadena">Expresion a evaluar</param>
        /// <param name="tipo">1->Se desea evaluar un operador lógico.2->Se desea evaluar un operador matematico</param>        
        /// <returns></returns>
        private bool CaracteresNoValidos(string cadena, int tipo)
        {
            //TIPOS {1-logico, 2 matematico}
            char[] logicos = new char[] { '/', '*', '+', '-', ',', '^', '(', ')' };
            char[] matematicos = new char[] { '>', '<', '=' };
            if (tipo == 1)
            {
                for (int i = 0; i < cadena.Length; i++)
                {
                    for (int j = 0; j < logicos.Length; j++)
                    {
                        if (cadena[i] == logicos[j]) return true;
                    }
                }
            }
            if (tipo == 2)
            {
                for (int i = 0; i < cadena.Length; i++)
                {
                    for (int l = 0; l < matematicos.Length; l++)
                    {
                        if (cadena[i] == matematicos[l]) return true;
                    }
                }
            }
            return false;
        }

        //METODO PARA SABER SI LAS OPERATORIAS DESPUES DE LOS CORCHETES CONTIENEN PARENTESIS
        //RETORNA TRUE SI CONTIENE
        /// <summary>
        /// Nos permite evaluar si la expresion de verdad y falso en una funcion IF contiene la cantidad de parentesis correcta.
        /// <para>Igual cantidad de parentesis de cierre que de abertura.</para>
        /// <para>Si la cantidad de parentesis de abertura es distinta que la cantidad de parentesis de cierre retorna false.</para>
        /// <para>Si no hay parentesis retorna false.</para>
        /// </summary>
        /// <returns></returns>
        private bool ExisteParentesis()
        {
            string cadena = "";
            //string subcadena = "";
            //int posCorcheteAbre = 0;

            //OBTENERMOS LA CADENA RESULTANTE
            cadena = GetExpresionResultante();
            //SI LA CADENA NO CONTIENE PARENTESIS NO ES VALIDA
            if (!cadena.Contains("(") && !cadena.Contains(")")) return false;
            if (cadena.Contains("(") && !cadena.Contains(")")) return false;
            if (!cadena.Contains("(") && cadena.Contains(")")) return false;

            int p1 = 0, p2 = 0;
            for (int i = 0; i < cadena.Length; i++)
            {
                if (cadena[i] == '(')
                {
                    if (cadena[i - 1] == ']')
                    {
                        //guardamos posicion
                        p1 = i;
                        break;
                    }
                }
            }

            if (cadena[cadena.Length - 1] == ')') p2 = cadena.Length;
            else { XtraMessageBox.Show("Error al final de la cadena", "Cadena", MessageBoxButtons.OK, MessageBoxIcon.Warning); return false; }
            //GENERAMOS CADENA QUE REPRESENTA LA PARTE MATEMATICA
            string mat = ExtraerMatematica(cadena, p1, p2);
            //MessageBox.Show(mat);

            //VERIFICAMOS EN CASO DE TENER PARENTESIS SI LOS PARENTESIS ESTAN BALANCEADOS
            //(TIENE QUE HABER LA MISMA CANTIDAD DE PARENTESIS QUE ABREN DE APRENTESIS QUE CIERRAN)
            bool balance = BalanceParentesis(mat);
            if (balance == false)
            {
                XtraMessageBox.Show("Error en los parentesis, faltan parentesis", "Parentesis", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                //ESTARIAN LOS MISMOS PARENTESIS
                //PODEMOS VERIFICAR LOS PARENTESIS
                bool parentesis = VerificaParentesis(mat);
                //SI ES FALSA NO ES VALIDA LA ESTRUCTURA
                if (parentesis == false)
                {
                    XtraMessageBox.Show("Faltan parentesis!", "Parentesis", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    //Estructura correcta
                    //caracterers no permitidos
                    bool caracteres = CaracteresNoValidos(mat, 2);
                    if (caracteres)
                    {
                        XtraMessageBox.Show("Caracteres no validos en expresion a evaluar", "Caracteres", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                    else
                    {
                        //SETEAR VARIABLE 
                        SetExpresionMatematica(mat);
                        return true;
                    }

                }
            }
            return true;
        }
        /// <summary>
        /// Corresponde a las expresiones matemáticas que se van a evaluar dentro de la funcion IF. Corresponde a expresion verdadera y expresion falsa.
        /// </summary>
        /// <param name="cadena">Cadena a evaluar</param>
        /// <param name="pos1">Posicion inicial desde donde se desea extraer subcadena</param>
        /// <param name="pos2">Posicion final hasta donde se desea extraer subcadena</param>
        /// <returns></returns>
        private string ExtraerMatematica(string cadena, int pos1, int pos2)
        {
            string sub = "";
            int largo = cadena.Length;
            if (largo == 0) return "";

            sub = cadena.Substring(pos1, (pos2 - pos1));

            return sub;
        }

        //verificar que empieza y termina con una parentesis
        //ESTO ES PARA LA CADENA MATEMATICA COMPLETA 
        //PPARA QUE LA VERIFICACION DE PARENTESIS TENGA SENTIDO
        //DEBERIAMOS PRIMERAMENTE SABER SI LA CANTIDAD DE PARENTESIS ESTA BALANCEADA
        /// <summary>
        /// Nos indica si la cantidad de parentesis de Abertura es igual a la cantidad de parentesis de cierre dentro de una cadena.
        /// </summary>
        /// <param name="cadena">Cadena a evaluar</param>
        /// <returns></returns>
        private bool BalanceParentesis(string cadena)
        {
            int largo = cadena.Length;
            int ab = 0, ci = 0;
            for (int i = 0; i < largo; i++)
            {
                if (cadena[i] == '(')
                    ab++;
                if (cadena[i] == ')')
                    ci++;
            }

            if (ab != ci) return false;

            return true;
        }

        /// <summary>
        /// Nos indica si están presentes el parentesis de abertura y cierre en funcion if, correspondiente a las partes a evaluar.
        /// <para>Ejemplo: (True, False)</para>
        /// </summary>
        /// <param name="cadena">Cadena a evaluar</param>
        /// <returns></returns>
        private bool VerificaParentesis(string cadena)
        {
            //ESTRUCTURA --> '(verdad , falso)'
            //extraer first element
            //extraer last element
            int largo = cadena.Length;
            if (largo == 0) return false;

            string first = cadena[0].ToString();


            string last = cadena[cadena.Length - 1].ToString();


            if (first == "(" && last == ")") return true;
            if (first != "(" || last != ")") return false;
            if (first != "(" && last != ")") return false;

            return false;
        }

        /// <summary>
        /// Nos indica si existe caracter ',' de separacion para expresion a evaluar en funcion IF
        /// <para>(True,False) hace referencia al caracter ',' que separa la expresion True de la expresion False.</para>
        /// </summary>        
        /// <param name="cadena">Cadena a evaluar</param>               
        /// <returns></returns>
        private bool VerificarComa(string cadena)
        {
            //SUPONIENDO QUE SE VALIDO PREVIAMENTE EL METODO DE ARRIBA (PARENTESIS)
            //validar que antes y despues de la coma hay un numero o una variable 
            int largo = cadena.Length;
            if (largo == 0) return true;

            for (int i = 0; i < largo; i++)
            {
                if (cadena[i] == ',')
                {
                    if (cadena[i - 1] == '(') return true;
                    if (cadena[i + 1] == ')') return true;
                    if (cadena[i + 1] == ' ') return true;
                    if (cadena[i - 1] == ' ') return true;

                }
            }

            return false;
        }

        /// <summary>
        /// Indica si hay mas de una coma dentro de expresion verdadera y falsa.
        /// <para>(True,,,,False) No es válido.</para>
        /// </summary>
        /// <param name="cadena">Cadena a evaluar</param>
        /// <returns></returns>
        private bool MuschasComas(string cadena)
        {
            int largo = cadena.Length;
            if (largo == 0) return true;
            int coma = 0;
            for (int i = 0; i < largo; i++)
            {
                if (cadena[i] == ',')
                {
                    coma++;
                }
            }

            if (coma > 1) return true;

            return false;
        }

        /// <summary>
        /// Nos indica si existe caracter especial de separacion en expresion verdadera y false.
        /// <para>(TrueFalse). No sería válido.</para>
        /// </summary>
        /// <returns></returns>
        private bool ExisteComa()
        {
            string cadena = "";
            string[] partes = new string[] { };


            //OBTENEMOS LA CADENA RESULTANTE
            cadena = GetExpresionMatematica();
            //MessageBox.Show(cadena);
            //SI LA CADENA NO CONTIENE NINGUNA COMA NO ES VALIDA
            if (!cadena.Contains(",")) { XtraMessageBox.Show("Falta coma en la expresion a evaluar", "Coma", MessageBoxButtons.OK, MessageBoxIcon.Warning); return false; }

            bool existeComa = VerificarComa(cadena);
            if (existeComa)
            {
                XtraMessageBox.Show("Debes ingresar una expresion a evaluar antes y despues de la coma", "Coma", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            else
            {
                //ESTARIA CORRECTA
                //FALTA VERIFICAR QUE NO HAYAN MAS DE UNA COMA
                bool muchas = MuschasComas(cadena);
                if (muchas)
                {
                    XtraMessageBox.Show("Hay mas de una coma", "Coma", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                else
                {
                    return true;
                }
            }

        }

        #endregion

        #region "CALCULO MATEMATICA VERDADERO-FALSO"    

        /// <summary>
        /// Realiza el calculo de una expresion matematica simple.
        /// </summary>
        /// <param name="pCad">Cadena a evaluar</param>
        /// <returns></returns>
        private double CalculoMatematico(string pCad)
        {
            double ResultadoFinal = 0;
            string value = "";
            string calculado = "";

            if (pCad.Length > 0)
            {              
               try
               {
                    try
                    {
                        value = new DataTable().Compute(pCad, null).ToString();
                        calculado = string.Format("Valor calculado:{0}", value);

                        ResultadoFinal = Convert.ToDouble(value);

                        if (double.IsInfinity(ResultadoFinal) || double.IsNaN(ResultadoFinal))
                            ResultadoFinal = 0;
                    }
                    catch (DivideByZeroException ex)
                    {
                        ResultadoFinal = 0;
                    }
               
               }
               catch (OverflowException ex)
               {
                  XtraMessageBox.Show("El valor calculado es muy grande", "Valor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
               }               
            }           

            return ResultadoFinal;
        }

        //RECIBE COMO PARAMETRO LA CADENA LISTA PARA CALCULO (CON REEMPLAZO DE VARIABLES, ETC)
        /// <summary>
        /// Recibe una cadena y retorna el valor calculado.
        /// <para>Ej: (10*2)/10</para>
        /// <para>Retorna 2</para>
        /// </summary>
        /// <param name="cadena">Expresion matemática.</param>
        /// <returns></returns>
        private double CalculoExpresion(string cadena)
        {
            //XtraMessageBox.Show(cadena);
            double resultado = 0;
            string value = "";
            int posDiv = 0;
            if (cadena.Length > 0)
            {
                try
                {
                    try
                    {                        
                        value = new DataTable().Compute(cadena, null).ToString();
                        resultado = Convert.ToDouble(value);

                        if (double.IsInfinity(resultado) || double.IsNaN(resultado))
                            resultado = 0;
                    }
                    catch (DivideByZeroException ex)
                    {
                        resultado = 0;
                    }                   
                }
                catch (OverflowException ex)
                {
                    resultado = 0;
                    XtraMessageBox.Show("El valor calculado es muy grande", "Valor", MessageBoxButtons.OK, MessageBoxIcon.Warning);                       
                }
            }


            return resultado;
        }     
        #endregion

        #region "MANEJO DE CADENA MATEMATICA"   

        //GENERA LISTA DESDE LA CADENA (SE AGREGO RECONOCIMIENTO DE VARIABLES CON NUMEROS COMO --> HRSEX50)
        /// <summary>
        /// Genera un listado de elementos desde una expresion. Pueden ser numeros, variables, etc.
        /// </summary>
        /// <param name="Cadena">Cadena a evaluar.</param>
        /// <returns></returns>
        private List<string> GeneraListaCadena(string Cadena)
        {
            List<string> lista = new List<string>();
          
            string num = "", ope = "", par = "", letra = "";
            if (Cadena.Length > 0)
            {
                //recorrer cadena
                for (int i = 0; i < Cadena.Length; i++)
                {
                    //GUARDAMOS ELEMENTO DE CADENA COMO CHAR
                    char caracter = Cadena[i];
                  
                    //PREGUNTAMOS SI EL CARACTER ES UN NUMERO O UN PUNTO(PODRIA SER UN NUMERO DECIMAL)
                    if (caracter >= '0' && caracter <= '9' || caracter == '.')
                    {
                        //SI ES NUMERO O NUMERO DECIMAL         
                        //SI LETRA ES DISTINTO DE VACIO ESPORQUE SE ESTUVO ACUMULANDO LETRAS
                        //EN ESE CASO LE SUMAMOS LOS NUMEROS
                        if (letra != "")
                        {
                            letra = letra + caracter;
                        }
                        else
                        {
                            num = num + caracter;
                        }

                        if (letra != "" && i == Cadena.Length - 1)
                        {
                            lista.Add(letra);
                        }
                        if (num != "" && i == Cadena.Length - 1)
                        {
                            lista.Add(num);
                        }

                    }//PREGUNTAMOS SI ES UNA LETRA
                    else if (caracter >= 'A' && caracter <= 'Z')
                    {
                        //SI NUMERO ES DISTINTO DE VACIO ES PORQUE SE ESTUVO CONCATENANDO NUMEROS 
                        //EN ESTE CASO CONCATENAMOS LA LETRA A ESA CADENA
                        if (num != "")
                        {
                            num = num + caracter;
                        }
                        else
                        {
                            letra = letra + caracter;
                        }

                        if (letra != "" && i == Cadena.Length - 1)
                        {
                            lista.Add(letra);
                        }
                        if (num != "" && i == Cadena.Length - 1)
                        {
                            lista.Add(num);
                        }
                    }
                    else if (caracter == ')' || caracter == '(')
                    {
                        //ES PARENTESIS

                        if (letra != "")
                        {
                            lista.Add(letra);
                        
                            letra = "";
                        }

                        if (num != "")
                        {
                            lista.Add(num);
                          
                            num = "";
                        }
                        par = caracter + "";
                        lista.Add(par);
                    }
                    else if (caracter == '*' || caracter == '+' || caracter == '/' || caracter == '-' || caracter == '^')
                    {
                        //ES OPEARADOR MATEMATICO

                        if (num != "")
                        {
                            lista.Add(num);
                           
                            num = "";
                        }

                        if (letra != "")
                        {
                            lista.Add(letra);
                   
                            letra = "";
                        }

                        ope = caracter + "";
                        lista.Add(ope);
                 
                    }
                }
            }           
            return lista;
        }

        //BUSCAR Y VALIDAR LAS VARIABLES DENTRO DE LA LISTA
        //RECIBE COMO PARAMETRO LA LISTA QUE GENERA EL METODO GENERALISTA
        //RETORNA LA LISTA VALIDADA
        /// <summary>
        /// Valida que los elementos de la lista existan, como en el caso de las variables.
        /// </summary>
        /// <param name="Lista">Listado de elementos.</param>
        /// <returns></returns>
        private List<string> GeneraListaValida(List<string> Lista)
        {
            if (Lista.Count > 0)
            {
                //BUSCAR VARIABLES DENTRO DE LA LISTA
                List<string> Variables = new List<string>();
                Variables = BuscandoVariables(Lista);

                bool VariableValida = false;
                //OBTENER LAS VARIABLE DE SISTEMA
                List<string> sistema = new List<string>();
                sistema = fnListadoVariablesSistema();

                if (Variables.Count>0)
                {
                    //SE ENCONTRARON VARIABLES
                    //VALIDAMOS QUE TODAS LAS VARIABLES ENCONTRADAS EXISTAN COMO TAL EN BD
                    VariableValida = fnVariableExiste(Variables, sistema); 
                    //SI RETORNAR TRUE ES PORQUE TODAS LAS VARIABLES SON VALIDAS
                    if (VariableValida)
                    {
                        //LA LISTA ES CORRECTA PODEMOS DEVOLVERLA SIN PROBLEMAS
                        return Lista;
                    }
                    else
                    {
                        //LA VARIABLE NO EXISTE
                        //XtraMessageBox.Show("Variable no existe", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return null;
                    }
                }
                else
                {
                    //NO HAY VARAIBLE SOLO DEVOLVEMOS LA LISTA
                    return Lista;
                }
            }
            else
            {                
                return null;
            }
        }

        //PROCESAR LISTA CON VARIABLES VALIDAS 
        //SE SUPONE QUE YA SE VALIDO LA ESTRUCTURA Y SINTAXIS DE LA CADENA
        //SOLO FALTA OBTENER EL RESULTADO DE ACUERDO CONTRATO Y CODIGO DE ITEM EN CASO DE HABER UNA VARIABLE 
        //DE TABLA ITEM        
        /// <summary>
        /// Genera cadena resultante remplazando los valores de las variables, items, etc.
        /// </summary>
        /// <param name="lista">Listado de elementos.</param>
        /// <param name="contrato">Numero de contrato asociado.</param>
        /// <param name="ItemEmpleado">Codigo item trabajador.</param>
        /// <returns></returns>
        private string ListaCalculada(List<string> lista, string contrato, string ItemEmpleado)
        {
            //OBTENER LISTADO DE VARIABLE DE SISTEMA
            List<string> sistema = new List<string>();
            sistema = fnListadoVariablesSistema();

            //OBTENER VARIABLES DESDE LA CADENA
            List<string> varCadena = new List<string>();

            //PARA SABER SI HAY VARIABLES QUE SON PARAMETROS
            ParametroFormula Par = new ParametroFormula();

            string final = "";
            bool esItem = false;
            double calculo = 0;
            string valor = "0", anterior = "", siguiente = "";
            //int periodo = 0;
            //periodo = fnSistema.fnAnioMes();

            if (lista.Count > 0)
            {
                varCadena = BuscandoVariables(lista);
                
                if (varCadena.Count > 0)
                {
                    //HAY VARIABLES
                    //BUSCAMOS SU VALOR
                    foreach (var elemento in varCadena)
                    {
                        //PREGUNTAR SI VARIABLE ES UN CODIGO DE ITEM
                        string dato = elemento.ToLower();
                       
                        esItem = fnVariableEsItem(dato.ToUpper());
                        //ES ITEM ? 
                        if (esItem || dato=="vc" || dato=="vo")
                        {                                 
                            if (dato == "vo")
                            {                                
                                //CALCULAMOS EN BASE A VALOR ORIGINAL DE ITEM                               
                                valor = fnValorItem(contrato, ItemEmpleado, 0, getNumeroItem(), getPeriodoEmpleado()).ToString();
                              
                            }
                            else if (dato == "vc")
                            {
                                //CALCULAMOS EN BASE A VALOR CALCULADO DE ITEM
                                valor = fnValorItem(contrato, ItemEmpleado, 1, getNumeroItem(), getPeriodoEmpleado()).ToString();
                            }
                            else
                            {                               
                                int type = 0;
                                if (dato[0] == 'O' || dato[0] == 'o') type = 0;
                                if (dato[0] == 'C' || dato[0] == 'c') type = 1;
                               
                                //EL ELEMENTO ES UN ITEM, CALCULAMOS EN BASE AL ITEM PRESENTE EN CADENA                           
                                valor = fnValorItemGenerico(contrato, dato, type, getPeriodoEmpleado()).ToString();
                               
                                string cadena = string.Format("elemento:{0}; valor: {1}", dato, valor.ToString());                             
                            }                                                 
                        }
                        //ES UN INDICE MENSUAL?
                        else if (dato == "uf" || dato == "utm" || dato == "imm" || dato == "sis" || dato == "ufa")
                        {
                            valor = fnValorMes(getPeriodoEmpleado(), dato).ToString();
                        }
                        //ES UN PARAMETRO?
                        else if (Par.ExisteParametro(dato))
                        {
                            //OBTENEMOS EL VALOR DEL PARAMETRO.
                            valor = Par.GetValue(dato).ToString();
                        }
                        else
                        {
                            //ES VARIABLE TIPO SYS
                            //RECORRER LISTADO DE VARIABLE Y OBTENER VALOR                                      
                            valor = varSistema.ObtenerValorLista(dato).ToString();
                        }
                      
                        //REEEMPLAZAR VALOR OBTENIDO EN LISTA
                        for (int i = 0; i < lista.Count; i++)
                        {
                            if (lista[i] == elemento)
                            {
                                lista[i] = valor;
                            }
                        }
                    }

                    //UNA VEZ REEMPLAZADA TODAS LAS VARIABLES REALIZAMOS CALCULO DE POTENCIA EN CASO DE EXISTIR
                    for (int i = 0; i < lista.Count; i++)
                    {
                        if (lista[i] == "^")
                        {
                            anterior = lista[i - 1];
                            siguiente = lista[i + 1];
                            calculo = Math.Pow(double.Parse(anterior), double.Parse(siguiente));
                            //INSERTAMOS CALCULO EN lista
                            lista.Insert(i, calculo.ToString());
                            //reemplazar calculado en la cadena
                            lista.RemoveAt(i + 1);
                            lista.RemoveAt(i - 1);
                            lista.RemoveAt(i);
                        }
                    }
                }
                else
                {
                    //NO TIENE VARIABLES
                    //DEBEMOS BUSCAR DE TODAS MANERAS EN LA LISTA SI ENCUENTRA UNA POTENCIA
                    for (int i = 0; i < lista.Count; i++)
                    {
                        if (lista[i] == "^")
                        {
                            anterior = lista[i-1];
                            siguiente = lista[i + 1];
                            calculo = Math.Pow(double.Parse(anterior), double.Parse(siguiente));
                            //INSERTAMOS CALCULO EN LISTA
                            lista.Insert(i, calculo.ToString());
                            //REEMPLAZAR CALCULADO EN LA CADENA
                            lista.RemoveAt(i+1);
                            lista.RemoveAt(i-1);
                            lista.RemoveAt(i);
                        }
                    }
                }               
            }

            //REEMPLAZAMOS EN LA CADENA LAS COMAS POR PUNTOS (PARA LOS VALORES DECIMALES)
            for (int i = 0; i < lista.Count; i++)
            {
                if (lista[i].Contains(","))
                {
                    lista[i] = lista[i].Replace(",", ".");
                }
            }

            //GENERAMOS CADENA FINAL
            final = CadenaFinal(lista);
           
            return final;
        }

        //GENERAR NUEVA CADENA A PARTIR DEL METODO DE ARRIBA
        /// <summary>
        /// Genera cadena final para calculo.
        /// </summary>
        /// <param name="Lista">Listado de objetos.</param>
        /// <returns></returns>
        private string CadenaFinal(List<string> Lista)
        {
            string NuevaCadena = "";

            if (Lista != null)
            {
                foreach (var item in Lista)
                {
                    NuevaCadena = NuevaCadena + item;
                }
            }
            //retornamos la nueva cadena

            return NuevaCadena;
        }

        //VARIANTE GENERA LISTA VALIDA (USANDO METODO EXTRAEOBJETOS)
        /// <summary>
        /// Indica si los elementos de las lista, como variables, son válidos.
        /// </summary>
        /// <param name="listado">Listado de objetos.</param>
        /// <returns></returns>
        private bool ListaObjetosValida(List<TipoExpresion> listado)
        {
            bool validas = false;
            List<string> sis = new List<string>();
            List<string> variables = new List<string>();
            sis = fnListadoVariablesSistema();

            if (listado.Count > 0)
            {
                foreach (var elemento in listado)
                {
                    if (elemento.tipo == "vr")
                        variables.Add(elemento.Valor);
                }
            }

            if (variables.Count > 0)
            {
                //VERIFICAR SI LAS VARIALES SON VALIDAS
                validas = fnVariableExiste(variables, sis);
                if (validas) return true;
                else
                    return false;
            }
            else
            {
                //NO HAY VARIABLES EN LISTA
                return true;
            }

        }
       
        #endregion

        #region "MANEJO VARIABLES DE SISTEMA"

        //*****************************************
        // ORDEN DE USO DE LOS METODOS            |
        //*****************************************

        //1- 'BUSCAR VARIABLES' 
        //      @@ GENERA LISTA CON TODAS LAS VARIABLES ENCONTRADAS EN LA CADENA
        //2- 'LISTADO VARIABLES SISTEMA'
        //      @@ GENERA LISTA CON TODAS LAS VARIABLES(CODIGO) ENCONTRADAS EN BD
        //3- 'VARIABLE EXISTE'
        //      @@ VERIFICAR QUE LAS VARIABLES ENCONTRADAS EXISTAN COMO VARIABLES DE SISTEMA
        //4- 'VALOR VARIABLE'
        //      @@ OBTIENE EL VALOR DE LA VARIABLE DE ACUERDO A SU CODIGO
        //

        //BUSCAR VARIABLES EN CADENA MATEMATICA
        //RETORNA UNA LISTA CON TODAS LAS VARIABLES ENCONTRADAS EN CADENA A EVALUAR
        private List<string> BuscarVariables(List<string> Lista)
        {
            List<string> variables = new List<string>();
            if (Lista != null)
            {
                //GENERAR O EXTRAER TODAS LAS VARIABLES QUE PUEDAN HABER EN LA LISTA     
                //LAS GUARDA EN UNA LISTA DE STRING

                for (int position = 0; position < Lista.Count; position++)
                {
                    if (!IsNumeric(Lista[position]) && Lista[position] != ")" && Lista[position] != "(" &&
                        !isOperador(Lista[position]) && !Lista[position].Contains("."))
                    {
                        //  MessageBox.Show(Lista[position]);
                        variables.Add(Lista[position]);
                    }
                }
            }
            //RETORNAR LA LISTA
            return variables;
        }

        //TRAER TODAS LAS VARIABLES ACTUALES DESDE LA TABLA VARIABLESISTEMA EN BD
        //RETORNA UNA LISTA CON EL CODIGO DE VARIABLE
        /// <summary>
        /// Retorna un listado con todas las variables de sistema existentes en la base de datos.
        /// <para>Variables de sistema, Codigo Items, parametros, indices mensuales.</para>
        /// </summary>
        /// <returns></returns>
        private List<string> fnListadoVariablesSistema()
        {
            //CONSULTA SQL
            string sql = "SELECT codvariable FROM variablesistema";
            SqlCommand cmd;
            SqlDataReader rd;

            ParametroFormula Pr = new ParametroFormula();
            List<ParametroFormula> ListadoParametros = new List<ParametroFormula>();
            ListadoParametros = Pr.GetListado();

            List<string> ListadoVariables = new List<string>();
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            //SI ENCUENTRA RESULTADO RECORREMOS Y LLENAMOS LISTA CON VARIABLES
                            while (rd.Read())
                            {
                                ListadoVariables.Add((string)rd["codvariable"]);
                            }
                        }
                    }
                    //LIBERAR RECURSOS
                    cmd.Dispose();
                    rd.Close();
                    fnSistema.sqlConn.Close();
                }
                else
                {
                    //NO HAY CONEXION CON LA BD
                }
            }
            catch (SqlException ex)
            {
                //error exception sql
            }

            //RETORNAR LISTA CON VARIABLES DESDE LA BASE DE DATOS
            //AGREGAR A LA LISTA LAS VARIABLE DE TABLA VALORES MES
            ListadoVariables.Add("uf");
            ListadoVariables.Add("utm");
            ListadoVariables.Add("ingresominimo");
            ListadoVariables.Add("sis");
            //UF MES ANTERIOR
            ListadoVariables.Add("ufa");

            //OBTENER LISTADO VARIABLES DESDE TABLA ITEM
            List<string> Items = new List<string>();
            Items = ListadoItem();

            //AGREGAR LISTADO A LISTADO DE VARIABLES DE SISTEMA
            foreach (var item in Items)
            {
                ListadoVariables.Add(item);
            }

            //AGREGAR VARIABLE VALOR A LISTA DE VARIABLES DE SISTEMA
            ListadoVariables.Add("vc");
            ListadoVariables.Add("vo");

            //AGREGAR PARAMETROS A LISTADO
            if (ListadoParametros.Count > 0)
            {
                foreach (ParametroFormula pr in ListadoParametros)
                {
                    ListadoVariables.Add(pr.codPar);
                }
            }

            return ListadoVariables;
        }

        //VERIFICAR QUE LAS VARIABLES ENCONTRADAS EN LA CADENA EXISTAN COMO TAL EN BD
        //RECIBE LA LISTA DE VARIABLES COMO PARMETRO DE ENTRADA
        //RECIBE TAMBIEN LA LISTA CON TODAS LAS VARIABLES DE SISTEMA
        //RETORNA TRUE SI EXISTEN
        /// <summary>
        /// Verifica que todos los elementos de tipo variable dentro un listado existan.
        /// </summary>
        /// <param name="variables">Listado de variables.</param>
        /// <param name="VarSistema">Listado de variables desde base de datos.</param>
        /// <returns></returns>
        private bool fnVariableExiste(List<string> variables, List<string> VarSistema)
        {
            //RECORREMOS VARIABLES
            if (variables.Count > 0)
            {
                //RECORREMOS LA LISTA CON LAS VARIABLES DE SISTEMA
                //VERIFICAMOS QUE TODAS LAS VARIABLES EXISTAN 
                int cantidad = variables.Count;
                int encontradas = 0;
                string subitem = "";
                bool Esitem = false, novalido = false;
                for (int i = 0; i < cantidad; i++)
                {
                    string elemento = variables[i].ToLower();
                    //PREGUNTAR SI LA CADENA EMPIEZA CON C O CON O (CALCULADO U ORIGINAL)
                    if (elemento[0] == 'c' || elemento[0] == 'o' || elemento[0] == 'C' || elemento[0]== 'O')
                    {
                        //PREGUNTAR SI EL ELEMENTO ES ITEM
                        Esitem = fnVariableEsItem(elemento.ToUpper());
                       
                        if (Esitem)
                        {                            
                            //ES ITEM POR LO TANTO ES UNA VARIABLE VALIDA
                            subitem = elemento.Substring(1, elemento.Length-1);
                            elemento = subitem;
                        }
                    }
                    else
                    {
                        Esitem = fnVariableEsItem(elemento.ToUpper());
                        if (Esitem)
                        {
                            encontradas--;
                        }
                    }                  
                   
                    //RECORREMOS INTERNAMENTE LAS VARIABLES ENCONTRADAS EN LA CADENA
                    for (int pos = 0; pos < VarSistema.Count; pos++)
                    {
                        string sis = VarSistema[pos].ToLower();
                        if (elemento == "imm") elemento = "ingresominimo";
                        
                        if (elemento == sis)
                        {
                            
                            //SI SON IGUALES ES PORQUE LA VARIABLE EXISTE EN BD
                            //CONTAMOS 
                            encontradas++;
                        }
                    }
                }

                //SI ENCONTRADAS ES IGUAL A LA CANTIDAD DE ELEMENTOS QUE TIENE LA LISTA VARSISTEMA 
                //ES PORQUE TODAS LAS VARIABLES DE LA CADENA SON VALIDAS (EXISTEN EN BD)

                if (encontradas == cantidad )
                    return true;
                
            }
            return false;
        }

        //METODO PARA BUSCAR VARIABLES (VARIANTE PARA VARIABLES QUE CONTENGAN ALGUN NUMERO)
        //RETORNA LISTA CON TODAS LAS VARIABLES ENCONTRADAS
        //BUSCAR VARIABLES DENTRO DE LISTA
        /// <summary>
        /// Recorre un listado y retorna todas las variables encontradas.
        /// </summary>
        /// <param name="Listado">Listado de elementos.</param>
        /// <returns></returns>
        private List<string> BuscandoVariables(List<string> Listado)
        {
            List<string> variablesEncontradas = new List<string>();

            if (Listado.Count > 0)
            {
                //RECORREMOS LISTA Y BUSCAMOS VARIABLEES
                for (int i = 0; i < Listado.Count; i++)
                {
                    if (Listado[i] == "+" || Listado[i] == "*" || Listado[i] == "/" || Listado[i] == "-" || Listado[i] == "^")
                    {
                        //ES UN OPERADOR 
                        //NO ES VARIABLE
                    }
                    else if (Listado[i] == "(" || Listado[i] == ")")
                    {
                        //ES PARENTESIS
                        //NO ES VARIBLE
                    }
                    else if (Listado[i].Contains(".") || IsNumeric(Listado[i]))
                    {
                        //PODRIA SER UN NUMERO                        

                    }
                    else
                    {
                        //ES UNA VARIABLE    
                        variablesEncontradas.Add(Listado[i]);
                    }
                }
            }
            return variablesEncontradas;
        }

        //OBTENER EL VALOR DESDE TABLA VALORES MES
        /// <summary>
        /// Obtiene el valor de un indice mensual.
        /// </summary>
        /// <param name="anomes">Periodo consulta.</param>
        /// <param name="codigo">Codigo indicador mensual.</param>
        /// <returns></returns>
        private decimal fnValorMes(int anomes, string codigo)
        {
            
            //string[] codes = new string[] {"uf", "utm", "ingresominimo", "sis", "topeafp", "topesec"};
            string cod = "";
            decimal valor = 0;
            if (codigo == "imm")
                cod = "ingresominimo";
            else if (codigo == "ufa")
                cod = "UfMesAnt";
            else
                cod = codigo;
            

            string sql = string.Format("SELECT {0} as dato FROM valoresmes WHERE anomes={1}", cod, anomes);
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                     //   cmd.Parameters.Add(new SqlParameter("@panomes", anomes));                     
                     //   cmd.Parameters.Add(new SqlParameter("@pcod", cod));

                        //EJECUTAR
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                valor = (decimal)rd["dato"];
                            }
                        }
                    }
                    //LIBERAR RECURSOS
                    cmd.Dispose();
                    rd.Close();
                    fnSistema.sqlConn.Close();
                }                
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
            return valor;
        }

        //OBTENER LOS NOMBRES DE LAS COLUMNAS DE LA TABLA VALORES MES
        /// <summary>
        /// Obtiene el nombre de las columnas de la tabla valoresmes.
        /// </summary>
        /// <returns></returns>
        private List<string> ListadoVarMes()
        {
            List<string> names = new List<string>();
            string sql = "select c.name as names FROM sys.columns c join sys.tables t " +
                         " ON c.object_id = t.object_id " +
                        " WHERE t.name = 'valoresmes' AND c.name <> 'topeafp' AND c.name <> 'topesec' ";

            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                names.Add((string)rd["names"]);
                            }
                        }
                    }
                    //LIBERAR RECURSOS
                    cmd.Dispose();
                    rd.Close();
                    fnSistema.sqlConn.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
            foreach (var item in names)
            {
                XtraMessageBox.Show(item);
            }
            return names;
        }

        //LISTADO ITEM DESDE TABLA ITEM
        /// <summary>
        /// Retorna un listado de items desde bd.
        /// </summary>
        /// <returns></returns>
        private List<string> ListadoItem()
        {
            List<string> listado = new List<string>();
            string sql = "SELECT coditem FROM item";
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                //LLENAMOS LISTADO
                                listado.Add((string)rd["coditem"]);
                            }
                        }
                    }
                    //LIBERAR RECURSOS
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                    rd.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
            return listado;
        }

        //OBTENER VALOR DE ITEM EN CASO DE QUE LA VARIABLE SEA UN CODIGO DE ITEM
        /// <summary>
        /// Obtener el valor de un item de trabajador.
        /// </summary>
        /// <param name="contrato">Numero de contrato asociado.</param>
        /// <param name="codItem">Codigo de item.</param>
        /// <param name="tipo">0->Valor original. 1->ValorCalculado.</param>
        /// <param name="num">Numero de item</param>
        /// <param name="periodo">Periodo al cual pertenece el itemtrabajador.</param>
        /// <returns></returns>
        private double fnValorItem(string contrato, string codItem, int tipo, int num, int periodo)
        {
            //if (codItem != "")
            //{
            //    if (codItem[0] == 'O' || codItem[0] == 'C' || codItem[0] == 'o' || codItem[0] == 'c')
            //        codItem = codItem.Substring(1, codItem.Length - 1);
            //}

            double valor = 0, vc = 0;
            string sql = "SELECT valor, valorcalculado FROM itemtrabajador WHERE contrato=@pcontrato AND coditem=@pcoditem AND numitem=@pnumitem AND anomes=@periodo AND suspendido = 0";
            //string c = string.Format("SELECT valor, valorcalculo FROM itemtrabajador WHERE contrato={0} AND coditem={1} AND numitem={2}", contrato, codItem,num);
            //XtraMessageBox.Show(c);
       
            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pcontrato", contrato));
                        cmd.Parameters.Add(new SqlParameter("@pcoditem", codItem));
                        cmd.Parameters.Add(new SqlParameter("@pnumitem", num));
                        cmd.Parameters.Add(new SqlParameter("@periodo", periodo));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                valor = Convert.ToDouble((decimal)rd["valor"]);                              
                                vc = Convert.ToDouble((decimal)rd["valorcalculado"]);
                            }
                        }
                    }
                    //LIBERAR RECURSOS
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                    rd.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
            
            if (tipo == 0)
            {
                //RETORNO VALOR ORIGINAL
                return valor;
            }
            else if (tipo == 1)
            {
                //RETORNO CALCULADO               
                return vc;                
            }
           
            return valor;

        }

        //VERSION 2 DEL METODO DE ARRIBA PARA OBTENER EL VALOR DE ITEM
        /// <summary>
        /// Obtiene el valor de un item de trabajador. No considera el numero de item
        /// </summary>
        /// <param name="contrato">Numero de contrato asociado.</param>
        /// <param name="codItem">Codigo del item</param>
        /// <param name="tipo">0->Valor base. 1->Valor calculado.</param>
        /// <param name="periodo"></param>
        /// <returns></returns>
        private double fnValorItemGenerico(string contrato, string codItem, int tipo, int periodo)
        {
            if (codItem != "")
            {
                if (codItem[0] == 'O' || codItem[0] == 'C' || codItem[0] == 'o' || codItem[0] == 'c')
                    codItem = codItem.Substring(1, codItem.Length - 1);
            }            
            
            double valor = 0, vc = 0;
            string sql = "SELECT valor, valorcalculado FROM itemtrabajador WHERE contrato=@pcontrato AND coditem=@pcoditem AND anomes=@periodo AND suspendido = 0";            

            SqlCommand cmd;
            SqlDataReader rd;
            try
            {
                if (fnSistema.ConectarSQLServer())
                {
                    using (cmd = new SqlCommand(sql, fnSistema.sqlConn))
                    {
                        //PARAMETROS
                        cmd.Parameters.Add(new SqlParameter("@pcontrato", contrato));
                        cmd.Parameters.Add(new SqlParameter("@pcoditem", codItem));
                        cmd.Parameters.Add(new SqlParameter("@periodo", periodo));

                        rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                valor = valor +  Convert.ToDouble((decimal)rd["valor"]);
                                vc = vc + Convert.ToDouble((decimal)rd["valorcalculado"]);
                            }
                        }
                    }
                    //LIBERAR RECURSOS
                    cmd.Dispose();
                    fnSistema.sqlConn.Close();
                    rd.Close();
                }
            }
            catch (SqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
            }

            if (tipo == 0)
            {
                //RETORNO VALOR ORIGINAL
                return valor;
            }
            else if (tipo == 1)
            {
                //RETORNO CALCULADO
                return vc;
            }
            return valor;
        }

        //PREGUNTAR SI UNA VARIABLE ES UN CODIGO DE ITEM
        /// <summary>
        /// Nos indica si la variable es un item.
        ///<para>O->Valor base</para>
        ///<para>C->Valor calculado.</para>
        /// </summary>
        /// <param name="codigo">Cadena a evaluar.</param>
        /// <returns></returns>
        private bool fnVariableEsItem(string codigo)
        {           
            List<string> items = new List<string>();
            items = ListadoItem();

            if (codigo[0] == 'O' || codigo[0] == 'C' || codigo[0]=='o' || codigo[0] == 'c')
            {
                codigo = codigo.Substring(1, codigo.Length - 1);
            }           

            foreach (var elemento in items)
            {
                if (elemento == codigo)
                {
                    return true;
                }
            }

            return false;
        }            
       
        #endregion   

        #region "MANEJO LETRAS - NUMEROS"

        //IS NUMBER
        private bool IsNumber(char x)
        {
            if (x >= '0' && x <= '9') return true;

            return false;
        }

        //IS LETTER
        private bool isLetra(char x)
        {
            if ((x >= 'a' && x <= 'z') || (x >= 'A' && x <= 'Z')) return true;

            return false;

        }

        //FUNCION PARA VER SI UNA CADENA ES NUMERICA O NO
        private bool IsNumeric(string input)
        {
            int test;
            return int.TryParse(input, out test);
        }

        //NO ES CARACTER VALIDO
        private bool IsCaracter(string cad)
        {
            char[] novalidos = new char[] {'*', '-', '/', '+', ';', ',', '.', ')', '(', '[', ']', '#'};
            
            if (cad.Length > 0)
            {
                for (int x = 0; x < cad.Length; x++)
                {
                    for (int i = 0; i < novalidos.Length; i++)
                    {
                        
                        if (novalidos[i] == cad[x])
                        {
                            
                            //NO VALIDO
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        #endregion         

        #region "ERRORES OPERACIONALES"  

        //CLAVES UNICAS --> IF, WHILE, FOR, ETC
        //PARAMETRO DE ENTRADA-> CADENA STRING
        //RETORNA TRUE SI LA CADENA COINCIDE CON LGUNAS DE LAS CADENAS RESERVADAS
        /// <summary>
        /// Indica si dentro de la expresion se encuentra alguna palabra clave que representa una funcion como IF.
        /// </summary>
        /// <param name="cadena">Cadena a evaluar.</param>
        /// <returns></returns>
        private bool Claves(string cadena)
        {
            cadena = cadena.ToUpper();
            string[] reservadas = new string[] { "IF", "FOR", "WHILE" };
            //RECORRER ARRAY Y VERIFICAMOS EXISTENCIA
            //SI COINCIDE CON ALGUNA CADENA RETORNAMO TRUE
            for (int i = 0; i < reservadas.Length; i++)
            {
                if (cadena == reservadas[i]) return true;
            }
            return false;
        }          

        /// <summary>
        /// Indica si un caracter es un operador matematico.
        /// </summary>
        /// <param name="cadena">Cadena a evaluar</param>
        /// <returns></returns>
        private bool isOperador(string cadena)
        {
            if (cadena == "+" || cadena == "-" || cadena == "*" || cadena == "/" || cadena == "^")
            {
                return true;
            }

            return false;
        }
        #endregion

        #region "VALIDACION SINTAXIS"
        /// <summary>
        /// Nos entraga un codigo de error de acuerdo a la evaluacion de algún elemento de la expresion.
        /// </summary>
        /// <param name="cadena">Cadena de entrada.</param>
        /// <returns></returns>
        private int fnCodigoError(string cadena)
        {          
            
            //if (ParentesisConcordantes(cadena)) return 1;
  
            if (OperadorParentesisCierra(cadena)) return 2;
            if (ParentesisAbreOperador(cadena)) return 3;
            if (ParentesisDesbalanceados(cadena)) return 4;
            if (ParentesisVacio(cadena)) return 5;
            if (ParentesisBalanceIncorrecto(cadena)) return 6;
            if (ParentesisCierraNumero(cadena)) return 7;
            if (NumeroParentesisAbre(cadena)) return 8;
            if (ParentesisCierraVariable(cadena)) return 9;
            if (VariableluegoPunto(cadena)) return 10;
            if (PuntoluegoVariable(cadena)) return 11;
            if (NumeroAntesVariable(cadena)) return 12;
            if (VariableDespuesNumero(cadena)) return 13;
            if (ParCierraParAbre(cadena)) return 14;
            if (OperadorPunto(cadena)) return 15;
            if (ParAbrePunto(cadena)) return 16;
            if (PuntoParAbre(cadena)) return 17;
            if (ParCierraPunto(cadena)) return 18;
            if (PuntoOperador(cadena)) return 19;
            if (OperadorFinal(cadena)) return 20;
            if (OperadorPrincipio(cadena)) return 21;
            if (PuntoEmpiezaTermina(cadena)) return 22;
            if (NumeroPuntoNumero(cadena)) return 23;
            if (DosoMasOperadores(cadena)) return 24;
            if (DivisionPorCero(cadena)) return 25;

            return -1;          
        }

        /// <summary>
        /// Entrega un mensaje de acuerdo a codigo de error de entrada.
        /// </summary>
        /// <param name="opcion">Codigo de error.</param>
        /// <returns></returns>
        private string fnMensajeError(int opcion)
        {
            string msg = "";
            switch (opcion)
            {
                case 1:
                    msg = "Error: Parentesis no concuerdan";
                    break;
                case 2:
                    msg = "Error: No puede haber un operador seguido de un paréntesis que cierra";
                    break;
                case 3:
                    msg = "Error: No puede haber un paréntesis que abre seguido de un operador";
                    break;
                case 4:
                    msg = "Error: Los paréntesis están desbalanceados";
                    break;
                case 5:
                    msg = "Error: No pueden haber parentesis vacíos";
                    break;
                case 6:
                    msg = "Error: No concide el parentesis que cierra con el que abre";
                    break;
                case 7:
                    msg = "Error: No puede haber un  paréntesis que cierra y sigue un número o paréntesis que abre";
                    break;
                case 8:
                    msg = "Error: No puede haber un número seguido de un paréntesis que abre";
                    break;
                case 9:
                    msg = "Error: No puede haber un paréntesis que cierra seguido de una variable";
                    break;
                case 10:
                    msg = "Error: No puede haber una variable seguida de un punto";
                    break;
                case 11:
                    msg = "Error: no puede haber un punto seguido de una variable";
                    break;
                case 12:
                    msg = "Error: No puede haber un número antes de una variable";
                    break;
                case 13:
                    msg = "Error: No puede haber un número después de una variable";
                    break;
                case 14:
                    msg = "Error: No puede haber paréntesis que cierra seguido de paréntesis que abre";
                    break;
                case 15:
                    msg = "Error: Después de operador no puede haber un punto";
                    break;
                case 16:
                    msg = "Error: Después de paréntesis que abre no puede haber un punto";
                    break;
                case 17:
                    msg = "Error: No puede haber un punto seguido despues de un paréntesis que abre";
                    break;
                case 18:
                    msg = "Error:No puede haber un punto despues de un parentesis de cierre";
                    break;
                case 19:
                    msg = "Error: No puede haber un punto seguido despues de operador";
                    break;
                case 20:
                    msg = "Error: No puede haber un operador al final de la expresion";
                    break;
                case 21:
                    msg = "Error: No puede haber un operador al principio de la expresion";
                    break;
                case 22:
                    msg = "Error: No puede haber un punto al inicio o termino de la expresion";
                    break;
                case 23:
                    msg = "Error: Valor decimal no valido";
                    break;
                case 24:
                    msg = "Error: No puede haber dos operadores juntos";
                    break;
                case 25:
                    msg = "Error: División por cero.";
                    break;
                case -1:
                    msg = "Sin errores";                    
                    break;
            }

            return msg;
        }

        /*FUNCIONES PARA VALIDACIONES...*/

        //2. Un operador seguido de un paréntesis que cierra. Ejemplo: 2-(4+)-7
        /// <summary>
        /// Un operador seguido de un paréntesis que cierra no es válido.
        /// <para>Ejemplo: 2-(4+)-7</para>
        /// </summary>
        /// <param name="expr">Cadena a evaluar.</param>
        /// <returns></returns>
        private bool OperadorParentesisCierra(string expr)
        {
            for (int pos = 0; pos < expr.Length - 1; pos++)
            {
                char car1 = expr[pos];   //Extrae un carácter

                //Compara si el primer carácter es operador y el siguiente es paréntesis que cierra
                if (car1 == '+' || car1 == '-' || car1 == '*' || car1 == '/' || car1 == '^')
                    if (expr[pos + 1] == ')') return true;
            }
            return false; //No encontró operador seguido de un paréntesis que cierra
        }

        //3. Un paréntesis que abre seguido de un operador. Ejemplo: 2-(*3)
        /// <summary>
        /// Un paréntesis que abre seguido de un operador no es válido.
        /// <para>Ejemplo: 2-(*3)</para>
        /// </summary>
        /// <param name="expr">Cadena a evaluar.</param>
        /// <returns></returns>
        private bool ParentesisAbreOperador(string expr)
        {
            for (int pos = 0; pos < expr.Length - 1; pos++)
            {
                char car2 = expr[pos + 1]; //Extrae el siguiente carácter

                //Compara si el primer carácter es paréntesis que abre y el siguiente es operador
                if (expr[pos] == '(')
                    if (car2 == '+' || car2 == '*' || car2 == '/' || car2 == '^') return true;
            }
            return false;  //No encontró paréntesis que abre seguido de un operador
        }

        //4. Que los paréntesis estén desbalanceados. Ejemplo: 3-(2*4))
        /// <summary>
        /// Parentesis desbalanceados no son válidos.
        /// <para>Ejemplo: 3-(2*4))</para>
        /// </summary>
        /// <param name="expr">Cadena a evaluar.</param>
        /// <returns></returns>
        private bool ParentesisDesbalanceados(string expr)
        {
            int parabre = 0, parcierra = 0;
            for (int pos = 0; pos < expr.Length; pos++)
            {
                char car1 = expr[pos];
                if (car1 == '(') parabre++;
                if (car1 == ')') parcierra++;
            }
            return parabre != parcierra;
        }

        //5. Que haya paréntesis vacío. Ejemplo: 2-()*3
        /// <summary>
        /// No pueden haber parentesis vacíos.
        /// <para>Ejemplo: 2-()*3</para>
        /// </summary>
        /// <param name="expr">Cadena a evaluar.</param>
        /// <returns></returns>
        private bool ParentesisVacio(string expr)
        {
            //Compara si el primer carácter es paréntesis que abre y el siguiente es paréntesis que cierra
            for (int pos = 0; pos < expr.Length - 1; pos++)
                if (expr[pos] == '(' && expr[pos + 1] == ')') return true;
            return false;
        }

        //6. Así estén balanceados los paréntesis no corresponde el que abre con el que cierra. Ejemplo: 2+3)-2*(4
        /// <summary>
        /// Balance incorrecto de parentesis.
        /// <para>Ejemplo: 2+3)-2*(4</para>
        /// </summary>
        /// <param name="expr">Cadena a evaluar.</param>
        /// <returns></returns>
        private bool ParentesisBalanceIncorrecto(string expr)
        {
            int balance = 0;
            for (int pos = 0; pos < expr.Length; pos++)
            {
                char car1 = expr[pos];   //Extrae un carácter
                if (car1 == '(') balance++;
                if (car1 == ')') balance--;
                if (balance < 0) return true; //Si cae por debajo de cero es que el balance es erróneo
            }
            return false;
        }

        //7. Un paréntesis que cierra y sigue un número o paréntesis que abre. Ejemplo: (3-5)7-(1+2)(3/6)
        /// <summary>
        /// Un numero después de un parentesis.
        /// <para>Ejemplo: (3-5)7-(1+2)(3/6)</para>
        /// </summary>
        /// <param name="expr">Cadena a evaluar.</param>
        /// <returns></returns>
        private bool ParentesisCierraNumero(string expr)
        {
            for (int pos = 0; pos < expr.Length - 1; pos++)
            {
                char car2 = expr[pos + 1]; //Extrae el siguiente carácter

                //Compara si el primer carácter es paréntesis que cierra y el siguiente es número
                if (expr[pos] == ')')
                    if (car2 >= '0' && car2 <= '9') return true;
            }
            return false;
        }

        //8. Un número seguido de un paréntesis que abre. Ejemplo: 7-2(5-6)
        /// <summary>
        /// Un numero seguido de un paréntesis que abre.
        /// <para>Ejemplo: 7-2(5-6)</para>
        /// </summary>
        /// <param name="expr">Cadena a evaluar.</param>
        /// <returns></returns>
        private bool NumeroParentesisAbre(string expr)
        {
            for (int pos = 0; pos < expr.Length - 1; pos++)
            {
                char car1 = expr[pos];   //Extrae un carácter

                //Compara si el primer carácter es número y el siguiente es paréntesis que abre
                if (car1 >= '0' && car1 <= '9')
                    if (expr[pos + 1] == '(') return true;
            }
            return false;
        }

        //10. Un paréntesis que cierra seguido de una variable. Ejemplo: (12-4)y-1
        /// <summary>
        /// No puede haber un paréntesis que cierra seguido de una variable.
        /// <para>Ejemplo: (12-4)y-1</para>
        /// </summary>
        /// <param name="expr">Cadena a evaluar.</param>
        /// <returns></returns>
        private bool ParentesisCierraVariable(string expr)
        {
            for (int pos = 0; pos < expr.Length - 1; pos++)
                if (expr[pos] == ')') //Compara si el primer carácter es paréntesis que cierra y el siguiente es letra
                    if ((expr[pos + 1] >= 'a' && expr[pos + 1] <= 'z') || (expr[pos + 1] >= 'A' && expr[pos + 1] <='Z'))
                        return true;
            return false;
        }

        //11. Una variable seguida de un punto. Ejemplo: 4-z.1+3
        /// <summary>
        /// No puede haber una variable seguida de un punto.
        ///<para>Ejemplo: 4-z.1+3</para>
        /// </summary>
        /// <param name="expr">Cadena a evaluar.</param>
        /// <returns></returns>
        private bool VariableluegoPunto(string expr)
        {
            for (int pos = 0; pos < expr.Length - 1; pos++)
                if ((expr[pos] >= 'a' && expr[pos] <= 'z') || (expr[pos] >= 'A' && expr[pos] <= 'Z'))
                    if (expr[pos + 1] == '.') return true;
            return false;
        }

        //12. Un punto seguido de una variable. Ejemplo: 7-2.p+1
        /// <summary>
        /// No puede haber un punto seguido de una variable.
        /// <para>Ejemplo: 7-2.p+1</para>
        /// </summary>
        /// <param name="expr">Cadena a evaluar.</param>
        /// <returns></returns>
        private bool PuntoluegoVariable(string expr)
        {
            for (int pos = 0; pos < expr.Length - 1; pos++)
                if (expr[pos] == '.')
                    if ((expr[pos + 1] >= 'a' && expr[pos + 1] <= 'z') || (expr[pos + 1] >= 'A' && expr[pos + 1] <= 'Z'))
                        return true;
            return false;
        }

        //13. Un número antes de una variable. Ejemplo: 3x+1
        //Nota: Algebraicamente es aceptable 3x+1 pero entonces vuelve más complejo un evaluador porque debe saber que 3x+1 es en realidad 3*x+1
        /// <summary>
        /// No puede haber un numero antes de una variable.
        /// <para>Ejemplo: 3x+1</para>
        /// </summary>
        /// <param name="expr">Cadena a evaluar.</param>
        /// <returns></returns>
        private bool NumeroAntesVariable(string expr)
        {
             bool ec = false;

            //PREGUNTAR PRIMERO SI LAS VARIABLES EN CASO DE EXISTIR SON VARIABLES VALIDAS
            List<string> variablesSistema = new List<string>();
            variablesSistema = fnListadoVariablesSistema();

            //GENERAMOS LA LISTA
            List<string> Lista = new List<string>();
            Lista = GeneraListaCadena(expr);

            //BUSCAMOS VARIABLES DENTRO DE LISTA
            List<string> variables = new List<string>();
            variables = BuscandoVariables(Lista);

            //VALIDAR VARIABLES
            bool validas = fnVariableExiste(variables, variablesSistema);
            if (validas) return false;

            for (int pos = 0; pos < expr.Length - 1; pos++)
             {
                 char item = expr[pos];                
                 if (item >= '0' && item <= '9')
                 {                    
                     char next = expr[pos + 1];                
                     if ((next >= 'a' && next <= 'z') || (next >='A' && next <= 'Z'))
                     {
                         ec = true;                     
                     }                        
                 }                    
             }           
             return ec;
        
        }

        //14. Un número después de una variable. Ejemplo: x21+4
        /// <summary>
        /// No puede haber un numero despues de una variable.
        /// <para>Ejemplo: x21+4</para>
        /// </summary>
        /// <param name="expr">Cadena a evaluar.</param>
        /// <returns></returns>
        private bool VariableDespuesNumero(string expr)
        {
             //PREGUNTAR PRIMERO SI LAS VARIABLES EN CASO DE EXISTIR SON VARIABLES VALIDAS
             List<string> variablesSistema = new List<string>();
             variablesSistema = fnListadoVariablesSistema();

            //GENERAMOS LA LISTA
            List<string> Lista = new List<string>();
            Lista = GeneraListaCadena(expr);

            //BUSCAMOS VARIABLES DENTRO DE LISTA
             List<string> variables = new List<string>();
            variables = BuscandoVariables(Lista);

            //VALIDAR VARIABLES
            bool validas = fnVariableExiste(variables, variablesSistema);
            if (validas) return false;            

             for (int pos = 0; pos < expr.Length - 1; pos++)
                 if ((expr[pos] >= 'a' && expr[pos] <= 'z') || (expr[pos] >= 'A' && expr[pos] <= 'Z'))
                     if (expr[pos + 1] >= '0' && expr[pos + 1] <= '9')
                         return true;
             return false;            
        }

        //19. Después de paréntesis que cierra sigue paréntesis que abre. Ejemplo: (4-5)(2*x)
        /// <summary>
        /// No puede haber un parentesis que cierra seguido de un paréntesis que abre.
        /// <para>Ejemplo: (4-5)(2*x)</para>
        /// </summary>
        /// <param name="expr">Cadena a evaluar</param>
        /// <returns></returns>
        private bool ParCierraParAbre(string expr)
        {
            for (int pos = 0; pos < expr.Length - 1; pos++)
                if (expr[pos] == ')' && expr[pos + 1] == '(')
                    return true;
            return false;
        }

        //20. Después de operador sigue un punto. Ejemplo: -.3+7
        /// <summary>
        /// No puede haber un operador y despúes un punto.
        /// <para>Ejemplo: -.3+7</para>
        /// </summary>
        /// <param name="expr">Cadena a evaluar</param>
        /// <returns></returns>
        private bool OperadorPunto(string expr)
        {
            for (int pos = 0; pos < expr.Length - 1; pos++)
                if (expr[pos] == '+' || expr[pos] == '-' || expr[pos] == '*' || expr[pos] == '/' || expr[pos] == '^')
                    if (expr[pos + 1] == '.')
                        return true;
            return false;
        }

        //21. Después de paréntesis que abre sigue un punto. Ejemplo: 3*(.5+4)
        /// <summary>
        /// No puede haber un paréntesis que abre y seguido un punto.
        /// <para>Ejemplo: 3*(.5+4)</para>
        /// </summary>
        /// <param name="expr">Cadena a evaluar.</param>
        /// <returns></returns>
        private bool ParAbrePunto(string expr)
        {
            for (int pos = 0; pos < expr.Length - 1; pos++)
                if (expr[pos] == '(' && expr[pos + 1] == '.')
                    return true;
            return false;
        }

        //22. Un punto seguido de un paréntesis que abre. Ejemplo: 7+3.(2+6)
        /// <summary>
        /// No puede haber un  punto seguido de un parentesis que abre.
        /// <para>Ejemplo: 7+3.(2+6)</para>
        /// </summary>
        /// <param name="expr">Cadena a evaluar.</param>
        /// <returns></returns>
        private bool PuntoParAbre(string expr)
        {
            for (int pos = 0; pos < expr.Length - 1; pos++)
                if (expr[pos] == '.' && expr[pos + 1] == '(')
                    return true;
            return false;
        }

        //23. Paréntesis cierra y sigue punto. Ejemplo: (4+5).7-2
        /// <summary>
        /// No puede haber un parentesis que cierra y seguido un punto
        /// <para>Ejemplo: (4+5).7-2</para>
        /// </summary>
        /// <param name="expr">Cadena a evaluar.</param>
        /// <returns></returns>
        private bool ParCierraPunto(string expr)
        {
            for (int pos = 0; pos < expr.Length - 1; pos++)
                if (expr[pos] == ')' && expr[pos + 1] == '.')
                    return true;
            return false;
        }

        //24. Punto seguido de operador. Ejemplo: 5.*9+1 
        /// <summary>
        /// No puede haber un punto y seguido un operador
        /// </summary>
        /// <param name="expr">Cadena a evaluar.</param>
        /// <returns></returns>
        private bool PuntoOperador(string expr)
        {
            for (int pos = 0; pos < expr.Length - 1; pos++)
                if (expr[pos] == '.')
                    if (expr[pos + 1] == '+' || expr[pos + 1] == '-' || expr[pos + 1] == '*' || expr[pos + 1] == '/' || expr[pos + 1] == '^')
                        return true;
            return false;
        }

        //25. OPERADOR AL FINAL DE LA CADENA NO ES VALIDO. Ejemplo 10/  2*2--
        /// <summary>
        /// No puede haber un operador al final de la cadena.
        /// <para>Ejemplo 10/  2*2--</para>
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        private bool OperadorFinal(string expr)
        {
            int largo = expr.Length;
            if (largo == 0) return true;
            //extraemos el ultimo elemento
            string last = expr[expr.Length - 1].ToString();
            if (last == "+" || last == "-" || last == "/" || last == "^" || last == "*")
            {
                return true;
            }
            return false;
        }

        //26. OPERADOR AL PRINCIPO DE LA CADENA NO ES VALIDO: /2+1*3
        /// <summary>
        /// No puede haber un operador al principio de la cadena.
        /// <para>/2+1*3</para>
        /// </summary>
        /// <param name="expr">Cadena a evaluar.</param>
        /// <returns></returns>
        private bool OperadorPrincipio(string expr)
        {
            int largo = expr.Length;
            if (largo == 0) return true;

            string first = expr[0].ToString();

            if (first == "*" || first == "-" || first == "/" || first == "+" || first == "^")
            {
                return true;
            }
            return false;
        }

        //27. UN punto al final de la cadena o al principio ej: ..5+1   5^3+1..
        /// <summary>
        /// No puede haber un punto al principio o a final de la cadena.
        /// <para>5^3+1..</para>
        /// </summary>
        /// <param name="expr">Cadena a evaluar.</param>
        /// <returns></returns>
        private bool PuntoEmpiezaTermina(string expr)
        {
            int largo = expr.Length;
            if (largo == 0) return true;

            string first = "", last = "";
            first = expr[0].ToString();
            last = expr[expr.Length - 1].ToString();

            if (first == "." || last == ".")
                return true;

            return false;
        }

        //28. punto numero punto  ej: 15+2+50.2.6
        /// <summary>
        /// No puede haber un punto, un numero, luego un punto.
        /// <para>ej: 15+2+50.2.6</para>
        /// </summary>
        /// <param name="expr">Cadena a evaluar</param>
        /// <returns></returns>
        private bool NumeroPuntoNumero(string expr)
        {
            int largo = expr.Length;
            if (largo == 0) return true;            
            string numero = "";
            List<string> parciales = new List<string>();
            if (expr.Contains("."))
            {
                for (int i = 0; i < largo; i++)
                {                    
                        if (expr[i] >= '0' && expr[i] <= '9' || expr[i] == '.')
                        {
                            //GENERAMOS NUMERO CONSIDERANDO NUMERO O PUNTO
                            numero = numero + expr[i];
                        if (i + 1 <= largo - 1)
                        {
                            if (isLetra(expr[i + 1]) || isOperador(expr[i + 1].ToString()))
                            {
                                //es una letra, no debemos seguir acumulando...
                                parciales.Add(numero);
                                //MessageBox.Show(numero);
                                numero = "";
                            }
                        }

                     
                        }

                    if (i == largo - 1) parciales.Add(numero);
                    

                }

                //genera una cadena como: 1548.21254.5758 ...

                //recorro cadena numero
                int puntos = 0;
                for (int i = 0; i < parciales.Count; i++)
                {
                    string item = parciales[i];
                    puntos = 0;
                    for (int pos = 0; pos < item.Length; pos++)
                    {
                        if (item[pos] == '.') puntos++;

                        if (puntos > 1) return true;
                            
                    }
                }
            }


            return false;

        }

        //29.dos o mas operadores --> 2+5--9
        /// <summary>
        /// No puede haber dos o mas operadores juntos.
        /// <para>Ej:2+5--9</para>
        /// </summary>
        /// <param name="expr">Cadena a evaluar.</param>
        /// <returns></returns>
        private bool DosoMasOperadores(string expr)
        {
            int largo = expr.Length;
            if (largo == 0) return true;
            int operador = 0;

            for (int i = 0; i < largo; i++)
            {

                if (isOperador(expr[i].ToString()))
                {
                    if (isOperador(expr[i+1].ToString()))
                    {
                        operador++;
                        //aumentar en uno la posicion
                        break;
                    }
                }
            }

            if (operador == 1) return true;

            return false;
        }
        /// <summary>
        /// No se puede dividir por cero Ej: 10/0
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        private bool DivisionPorCero(string expr)
        {
            bool error = false;

            if (expr.Length == 0 || expr == "") error = true;

            if (expr.Length > 0)
            {
                for (int i = 0; i < expr.Length; i++)
                {
                    if (expr[i].ToString() == "/" && expr[i + 1].ToString() == "0")
                    { error = true; break; }
                }
            }

            return error;
        }

        #endregion

        #region "FORMULA MIN Y MAX"

        //EXPRESION CON FORMATO => MIN(EXPRESION MATEMATICA;EXPRESION MATEMATICA)

        //VERIFICAR SI LA EXPRESION CONTIENE LA CLAVE MIN O MAX
        private bool ContieneMinMax(string Expresion)
        {
            //CADENA VACÍA
            if (Expresion.Length == 0)
                return false;

            //VERIFICAR QUE LA CADENA CONTENGA LA CLAVE MIN O MAX
            if (Expresion.Contains("MIN") || Expresion.Contains("MAX"))
            {
                //SI LA PRIMERA POSICION NO ES UNA M NO ES VÁLIDOS
                if (Expresion[0] != 'M')
                    return false;

                //EL SEGUNDO CARACTER DE LA CADENA NO ES I NI A
                if (Expresion[1] != 'I' && Expresion[1] != 'A')
                    return false;

                //EL TERCER CARACTER NO ES X NI N
                if (Expresion[2] != 'X' && Expresion[2] != 'N')
                    return false;

                //DESPUES DE PALABRA CLAVE NO CONTIENE PARENTESIS DE ABERTURA
                if (Expresion[3] != '(')
                    return false;

                //NO CONTIENE PARENTESIS DE CIERRE NI DE ABERTURA
                if (Expresion.Contains("(") == false || Expresion.Contains(")") == false)
                    return false;
            }               

            return false;
        }

        //VALIDAR CORRECTA SINTAXIS
        private bool ValidarMinMax(string Expresion)
        {
            bool tieneMaxMin = false;           
            tieneMaxMin = ContieneMinMax(Expresion);
           
            if (tieneMaxMin)
            {
                //EXISTE LA PALABRA CLAVE MIN MAX
                //ESTO DEBE APARECER EN LA PARTE LOGICA            
                string cadena = "";
                cadena = Expresion;
               
                //FORMATO --> MIN(EXPR1; EXPR2)
                //FORMATO --> MAX(EXPR1; EXPR2)

                int largo = cadena.Length;
                if (largo > 0)
                {
                    //VERIFICAR QUE DESPUES DE LA PALABRA MIN O MAX VENGA UN PARENTESIS DE ABERTURA
                    string subcadena = "";                   
                    subcadena = cadena.Substring(3,(largo-3));                
                    if (subcadena[0] != '(')
                    {
                        XtraMessageBox.Show("Falta parentesis de abertura", "Parentesis", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                    else
                    {
                        //VERIFICAR QUE EL ULTIMO ELEMENTO DE LA CADENA SEA UN PARENTESIS DE CIERRE
                        if (subcadena[subcadena.Length - 1] != ')')
                        {
                            XtraMessageBox.Show("Falta parentesis de cierre", "Parentesis", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                        else
                        {
                            //SEPARAR EXPRESION CON SPLIT SUPONIENDO QUE HAY UN ';'
                            //VERIFICAR QUE LA CADENA SOLO TIENE UN ';'
                            int puntocoma = 0, coma = 0;
                            for (int i = 0; i < subcadena.Length; i++)
                            {
                                if (subcadena[i] == ';')
                                {
                                    puntocoma++;
                                }
                                if (subcadena[i] == ',') coma++;
                            }

                            if (coma > 0) return false;
                            //SI ENCUENTRA MAS DE 1 NO ES VALIDO
                            if (puntocoma > 1) return false;
                            if (puntocoma == 0) return false;

                            //QUITAR DE LA CADENA EL ULTIMO Y EL PRIMER ELEMENTOS (SE SUPONE QUE SON LOS PARENTESIS DE PRIMER NIVEL)
                            subcadena = subcadena.Substring(1, subcadena.Length-2);                        

                            string[] separados = new string[2];
                            separados = subcadena.Split(';');

                            //VERIFICAR QUE LA CADENA ANTERIOR Y POSTERIOR NO ESTEN VACIAS (NO VALIDO)
                            string anterior = "", posterior = "";
                            anterior = separados[0];                                                    
                            posterior = separados[1];                      
                            if (anterior.Length == 0) return false;
                            if (posterior.Length == 0) return false;

                            //RETURN TRUE;
                            //HASTA ESTE PUNTO SE VALIDO CORRECTAMENTE LA ESTRUCTURA 
                            //FALTARIA REVISAR MATEMATICAMENTE LO QUE ESTA ANTES Y DESPUES DEL ;

                            //SETEAMOS VARIABLE CORRESPONDIENTE
                            SetCadenaMinMax(Expresion);
                            SetAnteriorMinMax(anterior);
                            SetPosteriorMinMax(posterior);
                            return true;
                        }
                    }
                }
                else
                {
                    //NO VALIDA
                    return false;
                }
            }
            else
            {
                return false;
            }  
        }

        /// <summary>
        /// Evalua funcion MinMax
        /// <para>Retorna true si la sintaxis es correcta.</para>
        /// <para>Retorna false si la sintaxis no es correcta.</para>
        /// </summary>
        /// <returns></returns>
        private bool EvaluarMinMax()
        {
            bool valido = false, validaFunciones = false, validarVariables = false, correcto = false;
            bool existefn = false, existevr = false;
            string parcial = "";
            List<TipoExpresion> objetos = new List<TipoExpresion>();
            if (GetCadenaOriginal() != "")
            {
                parcial = GetCadenaOriginal();
                
                //LIMPIAMOS EXPRESION
                parcial = LimpiarExpresion();

                //VALIDAMOS ESTRUCTURA CORRECTA
                objetos = ExtraeObjetos(parcial);               
                //VALIDAR SINTAXIS PARA FUNCIONES Y VARIABLES

                existefn = TieneFunciones(objetos);
                existevr = Tienevr(objetos);        
                
                if (existefn && existevr)
                {
                    //TIENE FUNCIONES Y VARIABLES
                    validarVariables = SintaxisVariables(objetos);
                    validaFunciones = SintaxisObjetos(objetos);
                    if (validarVariables && validaFunciones)
                    {
                        //SI AMBOS SON TRUE ES PORQUE LAS VARIABLES SON CORRECTAS Y LAS FUNCIONES TAMBIEN SON CORRECTAS (ESTRUCTURA)
                        //VALIDAR LA CADENA COMPLETA
                       valido =  ReemplazoFunciones(objetos);
                        if (valido)
                        {
                            //LA CADENA EN SU TOTALIDAD ES CORRECTA!!
                            correcto = true;
                        }
                        else
                        {
                            //error...
                            correcto = false;
                        }
                    }
                    else
                    {
                        correcto = false;
                    }
                }
                else if (existefn && existevr == false)
                {
                    //SOLO TIENE FUNCIONES
                    validaFunciones = SintaxisObjetos(objetos);
                    if (validaFunciones)
                    {
                        //VALIDAMOS LA CADENA EN SU TOTALIDAD
                        valido = ReemplazoFunciones(objetos);
                        if (valido)
                        {
                            //LA CADENA EN SU TOTALIDAD ES CORRECTA!!
                            correcto = true;
                        }
                        else
                        {
                            //error...
                            correcto = false;
                        }
                    }
                    else
                    {
                        correcto = false;
                    }
                }
                else if (existevr && existefn == false)
                {
                    //SOLO TIENE FUNCIONES
                    validarVariables = SintaxisVariables(objetos);
                    if (validarVariables)
                    {
                        //VALIDAMOS LA CADENA EN SU TOTALIDAD
                        valido = ReemplazoFunciones(objetos);
                        if (valido)
                        {
                            //LA CADENA EN SU TOTALIDAD ES CORRECTA!!
                            correcto = true;
                        }
                        else
                        {
                            //error...
                            correcto = false;
                        }
                    }
                }
                else
                {
                    //NO TIENE FUNCIONES NI variables
                }

                if (correcto == false)
                {                    
                    XtraMessageBox.Show("Error de sintaxis", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return correcto;
                }
                else
                {
                    return correcto;
                }               
          
            }
            else
            {
                return false;
            }
        }

        //UNA VEZ SETEADAS LAS VARIABLES CORRESPONDIENTES SOLO NOS FALTA EVALUAR CADA EXTREMO DE LA EXPRESION
        private bool EvaluarExpresionAnterior()
        {
            List<string> lista = new List<string>();
            List<string> listaValida = new List<string>();

            int error1 = 0;
            string ErrorMessage1 = "";

            //ESTRUCTURA CORRECTA...                         
                //VALIDAR EXTRUCTURA CORRECTA
                error1 = fnCodigoError(GetAnteriorMinMax());

                //OBTENER MENSAJE DE ERROR
                ErrorMessage1 = fnMensajeError(error1);

                //SI DEVUELVE -1 NO HAY ERROR (EXPRESION MATEMATICA CORRECTA)
                if (error1 == -1)
                {
                //NO HAY ERROR SE SINTAXIS                

                lista = GeneraListaCadena(GetAnteriorMinMax());               
                listaValida = GeneraListaValida(lista);                
              
                if (listaValida == null)
                {
                    //HUBO UN ERROR AL GENERAR LA CADENA
                    return false;
                }
                else
                {
                    //VALIDA
                    return true;
                }          
                    
                }
                else if (error1 != -1)
                {
                    XtraMessageBox.Show(ErrorMessage1, "Error Matematico", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            return false;             
        }

        //UNA VEZ SETEADAS LAS VARIABLES CORRESPONDIENTES SOLO NOS FALTA EVALUAR CADA EXTREMO DE LA EXPRESION
        private bool EvaluarExpresionPosterior()
        {
            List<string> lista = new List<string>();
            List<string> listaValida = new List<string>();
            int error1 = 0;
            string ErrorMessage1 = "";
                       
                //ESTRUCTURA CORRECTA...              

                //VALIDAR EXTRUCTURA CORRECTA
                error1 = fnCodigoError(GetPosteriorMinMax());

                //OBTENER MENSAJE DE ERROR
                ErrorMessage1 = fnMensajeError(error1);

                //SI DEVUELVE -1 NO HAY ERROR (EXPRESION MATEMATICA CORRECTA)
                if (error1 == -1)
                {
                
                lista = GeneraListaCadena(GetPosteriorMinMax());
                listaValida = GeneraListaValida(lista);

                if (listaValida == null)
                {
                    //HUBO UN ERROR AL GENERAR LA CADENA
                    return false;
                }
                else
                {
                    //LISTA VALIDA, RETORNAMOS TRUE
                    return true;   
                }
          
                }
                else if (error1 != -1)
                {
                    XtraMessageBox.Show(ErrorMessage1, "Error Sintaxis", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }        
            return false;
        }

        //CALCULO FINAL MINMAX V2
        /// <summary>
        /// Realiza calculo de funcion MinMax
        /// </summary>
        /// <returns></returns>
        private double CalculoFinalMinMaxV2()
        {           
            double calculo = 0;
            string contrato = "", itemtrabajador = "", ExpAnterior = "", ExpPosterior = "", cadenaFinal = "";
            double prev = 0, next = 0;
            double valor1 = 0, valor2 = 0, final = 0, potencia = 0;
            bool esItem = false;
            int periodo = 0;
            periodo = getPeriodoEmpleado();

            List<TipoExpresion> objetos = new List<TipoExpresion>();
            List<string> ListaInternaAnterior = new List<string>();
            List<string> ListaInternaPosterior = new List<string>();
            List<string> anterior = new List<string>();
            List<string> posterior = new List<string>();

            if (getContrato() != "") contrato = getContrato();
            if (getItemTrabajador() != "") itemtrabajador = getItemTrabajador();
          
            //GENERAR OBJETOS DESDE CADENA
            if (GetCadenaOriginal() != "")
            {
                objetos = ExtraeObjetos(GetCadenaOriginal());
                
                //RECORREMOS LA CADENA Y PREGUNTAMOS POR LOS OBJETOS QUE SON FUNCION
                foreach (var item in objetos)
               {
                    if (item.tipo == "fn")
                    {
                        //ES FUNCION (DEBEMOS OBTENER EL VALOR)
                        GenerarAnteriorPosterior(item.Valor);
                   
                        //UNA VEZ EXTRAIDAS LA PARTE ANTERIOR Y LA PARTE POSTERIOR PROCEDEMOS A REALIZAR EL CALCULO
                        if (GetAnteriorMinMax() != "" && GetPosteriorMinMax() != "")
                        {                           
                            //GENERAMOS LISTA DE CADA CADENA (ANTERIOR Y POSTERIOR)
                            ListaInternaAnterior = GeneraListaCadena(GetAnteriorMinMax());
                            ListaInternaPosterior = GeneraListaCadena(GetPosteriorMinMax());

                            //GENERAMOS LISTA VALIDA
                           // anterior = GeneraListaValida(ListaInternaAnterior);
                           // posterior = GeneraListaValida(ListaInternaPosterior);

                            //UNA VEZ GENERADA LA LISTA EVALUAMOS CADA LISTA (DEVUELVE UNA CADENA CON VAR Y FUN YA EVALUADAS)
                            ExpAnterior = ListaCalculada(ListaInternaAnterior, contrato, itemtrabajador);
                            ExpPosterior = ListaCalculada(ListaInternaPosterior, contrato, itemtrabajador);
                            
                            //CALCULO MATEMATICO
                            valor1 = CalculoExpresion(ExpAnterior);
                            valor2 = CalculoExpresion(ExpPosterior);

                            //PREGUNTAR SI ES MAX O MIN
                            if (item.Valor.Contains("MIN"))
                            {
                                if (valor1 < valor2)
                                {
                                    final = valor1;
                                }
                                else if (valor2 < valor1)
                                {
                                    final = valor2;
                                }
                                else
                                {
                                    final = valor1;
                                }
                            }
                            else if (item.Valor.Contains("MAX"))
                            {
                                if (valor1 > valor2)
                                {
                                    final = valor1;
                                }
                                else if (valor2 > valor1)
                                {
                                    final = valor2;
                                }
                                else
                                {
                                    final = valor1;
                                }
                            }

                            //UNA VEZ OBTENIDO EL RESULTADO FINAL PARA ESA FUNCION
                            //REEMPLAZAMOS LA FUNCION POR EL VALOR                            
                            item.Valor = final.ToString();                          
                        }
                    }
                    else if (item.tipo == "vr")
                    {                      
                        //ES VARIABLE (HAY QUE OBTENER EL VALOR Y REEMPLAZAR ITEM POR VALOR)
                        esItem = fnVariableEsItem(item.Valor.ToUpper());
                        if (esItem || item.Valor.ToLower() == "vc" || item.Valor.ToLower() == "vo")
                        {
                            if (item.Valor.ToLower() == "vo")
                            {
                                //CALCULAMOS EN BASE A VALOR ORIGINAL DE ITEM                               
                                calculo = fnValorItem(contrato, itemtrabajador, 0, getNumeroItem(), getPeriodoEmpleado());
                            }
                            else if (item.Valor.ToLower() == "vc")
                            {
                                //CALCULAMOS EN BASE A VALOR CALCULADO DE ITEM
                                calculo = fnValorItem(contrato, itemtrabajador, 1, getNumeroItem(), getPeriodoEmpleado());
                            }
                            else
                            {
                                int type = 0;
                                if (item.Valor[0] == 'O' || item.Valor[0] == 'o') type = 0;
                                if (item.Valor[0] == 'C' || item.Valor[0] == 'c') type = 1;

                                //EL ELEMENTO ES UN ITEM, CALCULAMOS EN BASE AL ITEM PRESENTE EN CADENA                           
                                calculo = fnValorItemGenerico(contrato, item.Valor, type, getPeriodoEmpleado());                                
                            }
                        }
                        else
                        {
                            //NO ES ITEM
                            
                            //PODRIA SER VARIABLE DE TABLA VALORESMES
                            if (item.Valor.ToLower() == "uf" || item.Valor.ToLower() == "utm" || item.Valor.ToLower() == "imm" || item.Valor.ToLower() == "sis")
                            {                              
                                calculo = Convert.ToDouble(fnValorMes(getPeriodoEmpleado(), item.Valor));
                            }
                            else
                            {
                                //ES VARIABLE TIPO SYS
                                //RECORRER LISTADO DE VARIABLE Y OBTENER VALOR                                   
                                calculo = varSistema.ObtenerValorLista(item.Valor.ToLower());
                               
                            }
                        }
                        
                        //UNA VEZ OBTENIDO EL VALOR FINAL REEMPLAZAMOS
                        item.Valor = calculo.ToString();
                    }                   
                }

                //AHORA QUE YA REEMPLAZAMOS LAS FUNCIONES Y VARIABLES REALIZAMOS EL CALCULO FINAL
                //VERIFICAR SI HAY ^ CALCULAR VALOR
               for (int i = 0; i < objetos.Count; i++)
                {
                    if (objetos[i].Valor == "^")
                    {
                        prev = Convert.ToDouble(objetos[i - 1].Valor);
                        next = Convert.ToDouble(objetos[i + 1].Valor);
                        potencia = Math.Pow(prev, next);

                        //ELIMINAR DE LA LISTA PREV Y NEXT Y SOLO DEJAR EL CALCULO DE LA POTENCIA
                        TipoExpresion ex = new TipoExpresion();
                        ex.tipo = "op";
                        ex.Valor = potencia.ToString();
                        objetos.Insert(i, ex);
                        //ELIMINAR SOBRANTES
                        objetos.RemoveAt(i+1);
                        objetos.RemoveAt(i - 1);
                        objetos.RemoveAt(i);
                    }
                }
                
                foreach (var item in objetos)
                {
                    if (item.Valor.Contains(",")) item.Valor = item.Valor.Replace(",", ".");                    
                    cadenaFinal = cadenaFinal + item.Valor;
                }

                //VALOR FINAL
                final = 0;              
                final = CalculoExpresion(cadenaFinal);
                final = Math.Round(final, 0, MidpointRounding.AwayFromZero);
                
            }
            return final;
        }

        //EXTRAER FUNCIONES MIN MAX DESDE CADENA 
        //ES UN METODO AMPLIADO DE GENERAR LISTA EL CUAL AHORA TAMBIEN RECONOCE DENTRO DE LA CADENA FUNCIONES MIN MAX
        /// <summary>
        /// Extrae desde una cadena funciones, expresiones matematicas y expresiones lógicas.
        /// Genera una lista con cada expresion.
        /// <para>Genera un listado de objetos de tipo *TipoExpresion* </para>
        /// <para>Tipos:</para>
        /// <para>* vr: Corresponde a una variable. Puede ser un valor de item, una variable tipo sys. </para>
        /// <para>* nm: Numero.</para>
        /// <para>* fn: Funcion.</para>
        /// <para>* op: Operador matematico. </para>
        /// <para>* pr: Representa un caracter de parentesis ') ('</para>
        /// </summary>
        /// <param name="cadena"></param>
        /// <returns></returns>
        private List<TipoExpresion> ExtraeObjetos(string cadena)
        {
            string cad = "", numero = "", operador = "", parentesis = "", letra = "";
            int abierto = 0, cerrado = 0, posab = -1, posci = -1, pos = 0, posf1 = -1, posf2 = -1, posf3 = -1;
            List<string> funciones = new List<string>();
            List<TipoExpresion> listado = new List<TipoExpresion>();
            if (cadena.Length > 0)
            {               
                    for (int i = 0; i < cadena.Length; i++)
                    {
                        //ES FUNCION MAX O MIN?
                        if ((cadena[i] == 'M' && cadena[i + 1] == 'I' && cadena[i + 2] == 'N') || (cadena[i] == 'M' && cadena[i + 1] == 'A' && cadena[i + 2] == 'X'))
                        {
                            //OBTENEMOS POSICION                            
                            pos = i + 3;
                            cad = "";
                            //AGREAMOS LAS TRES PRIMERAS LETRAS A LA CADENA 'cad'
                            cad = cad + cadena[i] + cadena[i + 1] + cadena[i + 2];
                            //SI EL CARACTER SIGUIENTE ES UNA LETRA DEBEMOS CONSIDERAR QUE NO ES UNA FUNCION SINO PUEDE
                            //SER UNA VARIABLE
                            if (pos <= cadena.Length - 1)
                            {
                                if (cadena[pos] >= 'A' && cadena[pos] <= 'Z')
                                {
                                    if (numero != "")
                                    {
                                        numero = numero + cadena[i];
                                    }
                                    else
                                    {
                                        letra = letra + cadena[i];
                                    }
                                    continue;
                                }

                                if (cadena[pos] >= '0' && cadena[pos] <= '9')
                                {
                                    if (letra != "")
                                    {
                                        letra = letra + cadena[i];
                                    }
                                    else
                                    {
                                        numero = numero + cadena[i];
                                    }
                                    continue;
                                }
                                if (cadena[pos] == '+' || cadena[pos] == '-' || cadena[pos] == '*' || cadena[pos] == '/' || cadena[pos] == '^')
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                if (numero != "")
                                {
                                    numero = numero + cadena[i] + cadena[i + 1] + cadena[i + 2];
                                    funciones.Add(numero);

                                    if (ContieneLetras(numero))
                                        listado.Add(new TipoExpresion() { Valor = numero, tipo = "vr" });
                                    else
                                        listado.Add(new TipoExpresion() { Valor = numero, tipo = "nm" });
                                    numero = "";
                                    cad = "";

                                }
                                if (letra != "")
                                {
                                    letra = letra + cadena[i] + cadena[i + 1] + cadena[i + 2];
                                    funciones.Add(letra);
                                    listado.Add(new TipoExpresion() { Valor = letra, tipo = "vr" });
                                    letra = "";
                                    cad = "";
                                }
                            }

                            //PARA GUARDAR LAS POSICIONES DE LAS LETRAS PARA QUE NO SE GUARDEN COMO VARIABLES
                            posf1 = i;
                            posf2 = i + 1;
                            posf3 = i + 2;

                            for (int j = pos; j < cadena.Length; j++)
                            {
                                cad = cad + cadena[j];
                                //RECORREMOS Y VERIFICAMOS PARENTESIS DE CIERRE Y ABERTURA
                                if (cadena[j] == '(')
                                {
                                    abierto = abierto + 1;
                                    posab = j;
                                }
                                if (cadena[j] == ')')
                                {
                                    cerrado++;
                                    posci = j;
                                }

                                if (abierto == cerrado)
                                {
                                    funciones.Add(cad);
                                    listado.Add(new TipoExpresion() { Valor = cad, tipo = "fn" });
                                    cad = "";
                                    i = posci;
                                    break;
                                }
                            }
                        }
                        //ES NUMERO O DECIMAL?
                        else if (cadena[i] >= '0' && cadena[i] <= '9' || cadena[i] == '.')
                        {
                            if (operador != "")
                            {
                                funciones.Add(operador);
                                listado.Add(new TipoExpresion() { Valor = operador, tipo = "op" });
                                operador = "";
                            }
                            if (letra != "")
                            {
                                letra = letra + cadena[i];
                            }
                            else
                            {
                                numero = numero + cadena[i];
                            }
                            if (i == cadena.Length - 1)
                            {
                                if (numero != "")
                                {
                                    funciones.Add(numero);

                                    if (ContieneLetras(numero))
                                        listado.Add(new TipoExpresion() { Valor = numero, tipo = "vr" });
                                    else
                                        listado.Add(new TipoExpresion() { Valor = numero, tipo = "nm" });
                                }
                                if (letra != "")
                                {
                                    funciones.Add(letra);
                                    listado.Add(new TipoExpresion() { Valor = letra, tipo = "vr" });
                                }
                            }
                        }
                        //ES UN OPERADOR MATEMATICO?
                        else if (cadena[i] == '*' || cadena[i] == '/' || cadena[i] == '-' || cadena[i] == '+' || cadena[i] == '^')
                        {
                            if (numero != "")
                            {
                                funciones.Add(numero);

                                if (ContieneLetras(numero))
                                    listado.Add(new TipoExpresion() { Valor = numero, tipo = "vr" });
                                else
                                    listado.Add(new TipoExpresion() { Valor = numero, tipo = "nm" });

                                numero = "";
                            }
                            if (letra != "")
                            {
                                funciones.Add(letra);
                                listado.Add(new TipoExpresion() { Valor = letra, tipo = "vr" });
                                letra = "";
                            }

                            operador = operador + cadena[i];
                            funciones.Add(operador);
                            listado.Add(new TipoExpresion() { Valor = operador, tipo = "op" });
                            operador = "";
                        }
                        //ES UN PARENTESIS ?
                        else if (cadena[i] == '(' || cadena[i] == ')')
                        {
                            if (operador != "")
                            {
                                funciones.Add(operador);
                                listado.Add(new TipoExpresion() { Valor = operador, tipo = "op" });
                                operador = "";
                            }
                            if (numero != "")
                            {
                                funciones.Add(numero);

                                if (ContieneLetras(numero))
                                    listado.Add(new TipoExpresion() { Valor = numero, tipo = "vr" });
                                else
                                    listado.Add(new TipoExpresion() { Valor = numero, tipo = "nm" });

                                numero = "";
                            }
                            if (letra != "")
                            {
                                funciones.Add(letra);
                                listado.Add(new TipoExpresion() { Valor = letra, tipo = "vr" });
                                letra = "";
                            }

                            if (i == cadena.Length - 1)
                            {
                                //   funciones.Add(cadena[i].ToString());
                            }

                            if (i != posab && i != posci)
                            {
                                funciones.Add(cadena[i].ToString());
                                listado.Add(new TipoExpresion() { Valor = cadena[i].ToString(), tipo = "pr" });
                            }

                        }
                        //ES UNA LETRA (VARIABLE)
                        else if (cadena[i] >= 'A' && cadena[i] <= 'Z')
                        {
                            if (operador != "")
                            {
                                funciones.Add(operador);
                                listado.Add(new TipoExpresion() { Valor = operador, tipo = "op" });
                                operador = "";
                            }
                            if (numero != "")
                            {
                                numero = numero + cadena[i];
                            }
                            else
                            {

                                if (i != posf1 && i != posf2 && i != posf3)
                                {
                                    letra = letra + cadena[i];
                                }
                            }

                            if (i == cadena.Length - 1)
                            {
                                if (letra != "")
                                {
                                    funciones.Add(letra);
                                    listado.Add(new TipoExpresion() { Valor = letra, tipo = "vr" });
                                }

                                if (numero != "")
                                {
                                    funciones.Add(numero);
                                    if (ContieneLetras(numero))
                                        listado.Add(new TipoExpresion() { Valor = numero, tipo = "vr" });
                                    else
                                        listado.Add(new TipoExpresion() { Valor = numero, tipo = "nm" });
                                }
                            }
                        }
                    }            
            }

            //TIPOS DE OBJETOS
            // vr --> VARIABLE
            // fn --> FUNCIONES (COMO MIN MAX)
            // op --> OPERADOR
            // nm --> NUMERO
            // pr --> PARENTESIS
            return listado;
        }

        //RECORRER CADENA DE OBJETOS Y VALIDAR LA SINTAXIS PARA EL CASO DE LAS FUNCIONES MIN MAX O EN CASO DE SER VARIABLES
        /// <summary>
        /// Indica si las funciones encontradas dentro del listado son válidas.
        /// </summary>
        /// <param name="listado">Listado de objetos.</param>
        /// <returns></returns>
        private bool SintaxisObjetos(List<TipoExpresion> listado)
        {            
            bool valFun = false, valAnterior = false, valPosterior;
            bool Interna = false;
            int correcto = 0;

            //PARA VARIABLES DE TIPO FN
            List<string> funciones = new List<string>();

            //EXTRAER TODAS LAS FUNCIONES DESDE LISTADO (FUNCIONES MINMAX)
            if (listado.Count > 0)
            {
                foreach (var item in listado)
                {
                    if (item.tipo == "fn")
                        funciones.Add(item.Valor);
                }

                //VALIDAR FUNCIONES
                foreach (var elemento in funciones)
                {
                    valFun = ValidarFunciones(elemento);
                    if (valFun)
                    {
                        //SI ES TRUE ES PORQUE LA FUNCION ES CORRECTA                        
                        valAnterior = EvaluarExpresionAnterior();
                        valPosterior = EvaluarExpresionPosterior();
                        if (valAnterior && valPosterior)
                        {
                            //LAS FUNCIONES INTERNAS DE LA FUNCION COMO OPERACIONES MATEMATICAS SON CORRECTAS!!
                            correcto++;
                        }                        
                    }
                    else
                    {
                        //HAY ERRORES
                        XtraMessageBox.Show("Hay un error en la funcion", "FUNCION", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }

                //SI CORRECTO ES IGUAL A LA CANTIDAD DE FUNCIONES
                //TODAS LAS FUNCIONES QUE TIENE LA CADENA TIENEN LA ESTRUCTURA CORRECTA

                if (correcto == funciones.Count)
                {
                    return true;
                }
                else
                {
                    XtraMessageBox.Show("Hay un error en la funcion", "FUNCION", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }                   
            }
            return false;
        }

        /// <summary>
        /// Nos indica si dentro del listado de objetos las variables existen y son válidas.
        /// </summary>
        /// <param name="listado">Listado de objetos.</param>
        /// <returns></returns>
        private bool SintaxisVariables(List<TipoExpresion> listado)
        {
            bool validas = false;
            List<string> variables = new List<string>();
            List<string> varsistema = new List<string>();
            varsistema = fnListadoVariablesSistema();
            if (listado.Count > 0)
            {
                //RECORREMOS Y OBTENEMOS LAS VARIABLES DESDE LISTADO
                foreach (var item in listado)
                {
                    if (item.tipo == "vr")
                        variables.Add(item.Valor);
                }

                //VALIDAR QUE LAS VARIABLES EXISTAN
                validas = fnVariableExiste(variables, varsistema);
                if (validas)
                {
                    return true;
                }
                else
                {
                    XtraMessageBox.Show("Variable no existe", "Variable", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            return false;
        }

        //VALIDAR FUNCIONES TIPO MIN-MAX
        private bool ValidarFunciones(string cadena)
        {
            string subcadena = "", Anterior = "", Posterior = "";
            string[] separar = new string[2];
            int largo = 0, puntoComa = 0;
            bool valido = false, tienecoma = false;
            if (cadena.Length > 0)
            {
                //RECORREMOS CADENA
                for (int i = 0; i < cadena.Length; i++)
                {                   
                    //@DEBE COMENZAR CON MIN O CON MAX
                    if ((cadena[i] == 'M' && cadena[i+1] == 'I' && cadena[i+2] == 'N') || (cadena[i] == 'M' && cadena[i+1] == 'A' && cadena[i+2] == 'X'))
                    {
                        //@VERIFICAR QUE DESPUES SEA UN PARENTESIS DE ABERTURA
                        subcadena = cadena.Substring(i+3, cadena.Length - (i+3));                        
                       
                        if (subcadena[0] == '(')
                        {
                            //@VERIFICAR QUE EL ULTIMO ELEMENTO DE LA FUNCION SEA UN PARENTESIS DE CIERRE
                            largo = subcadena.Length;
                            if (subcadena[largo-1] == ')')
                            {
                                //@VERIFICAR QUE EXISTE SOLO 1 ';' (PUNTO Y COMA)
                                for (int x = 0; x < largo; x++)
                                {
                                    if (subcadena[x] == ';')
                                        puntoComa++;
                                }

                                if (puntoComa > 1 || puntoComa == 0)
                                {              
                                    
                                    return false;                                    
                                }
                                else
                                {
                                    //SI NO ENTRA A NINGUNO DE LOS IF ES PORQUE SOLO EXISTE UN ';'
                                    //SETEAMOS VARIABLES CORRESPONDIENTES
                                    separar = subcadena.Split(';');

                                    //VERIFICAR QUE ANTES Y DESPUES DEL ; NO DEBEN ESTAR VACIOS
                                    if (separar[0].Length == 0 || separar[1].Length == 0)
                                    {
                                        return false;
                                    }
                                    else
                                    {
                                        //VALIDAR QUE NO HAYAN ',' ANTES Y DESPUES                                        
                                        Anterior = separar[0];
                                        Anterior = Anterior.Substring(1, Anterior.Length - 1);
                                        Posterior = separar[1];
                                        Posterior = Posterior.Substring(0, Posterior.Length - 1);
                                        
                                        tienecoma = ContieneComa(Anterior);
                                       
                                        if (tienecoma) return false;

                                        tienecoma = ContieneComa(Posterior);
                                       
                                        if (tienecoma) return false;

                                        //SETEAMOS VARIABLE CADENAMINMAX
//                                        SetCadenaMinMax(cadena);
                                        SetAnteriorMinMax(Anterior);
                                        SetPosteriorMinMax(Posterior);

                                        return true;
                                    }
                                   
                                }                          
                            }
                            else
                            {
                                //ESTRUCTURA INCORRECTA
                                valido = false;
                            }
                        }
                        else
                        {
                            //ESTRUCTURA INCORRECTA
                            valido = false;
                        }
                    }
                    else
                    {
                        //ESTRUCTURA INCORRECT
                        valido = false;
                    }
                }
            }

            return valido;
        }

        //VALIDAR EXPRESIONES MATEMATICAS INTERNAS (PARA EL CASO DE LAS FUNCIONES MIN MAX)
        private bool ValidarFuncionesInternas()
        {
            int error1 = 0, error2 = 0;
            string anterior = "", posterior = "", errorMsg = "";
            
            if (GetAnteriorMinMax() != "") anterior = GetAnteriorMinMax();
            if (GetPosteriorMinMax() != "") posterior = GetPosteriorMinMax();

            if (anterior.Length > 0 && posterior.Length > 0)
            {
                //PARA CADENA ANTERIOR
                //VALIDAR EXTRUCTURA MATEMATICA CORRECTA
                error1 = fnCodigoError(anterior);
                error2 = fnCodigoError(posterior);

                //SI LA FUNCION RETORNA -1 ES PORQUE NO HAY ERRORES DE SINTAXIS
                if (error1 != -1)
                {
                    errorMsg = fnMensajeError(error1);
                    XtraMessageBox.Show(errorMsg, "Error matematico", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
                else if (error2 != -1)
                {
                    errorMsg = fnMensajeError(error2);
                    XtraMessageBox.Show(errorMsg, "Error matematico", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (error1 == -1 && error2 == -1)
                {
                    //NO HAY ERRORES MATEMATICOS
                    return true;
                } 
            }
            return false;
        }
        
        //VER SI UNA PALABRA CONTIENE ALGUN NUMERO (PARA EL CASO DE SABER SI ES UNA VARIABLE)
        /// <summary>
        /// Indica si la cadena de entrada contiene letras.
        /// </summary>
        /// <param name="cadena">Cadena a evaluar</param>
        /// <returns></returns>
        private bool ContieneLetras(string cadena)
        {
            int c = 0;
            for (int i = 0; i < cadena.Length; i++)
            {
                if (cadena[i] >= 'A' && cadena[i] <= 'Z')
                {
                    c++;
                }
            }

            if (c > 0)
                return true;
            else
                return false;
        }

       /// <summary>
       /// Nos indica si una cadena contiene el caracter ','.
       /// Los numeros deben tener el caracter '.' si son decimales y no el caracter ','.
       /// </summary>
       /// <param name="cadena"></param>
       /// <returns></returns>
        private bool ContieneComa(string cadena)
        {
            int coma = 0;
            bool valido = false;
            if (cadena.Length > 0)
            {
                for (int i = 0; i < cadena.Length; i++)
                {
                    if (cadena[i] == ',')
                    {
                        coma++;
                    }                        
                }

                if (coma > 0)
                {
                    valido = true;   
                }
                else
                {                    
                    valido = false;
                }                   
            }
            return valido;
        }

        /// <summary>
        /// Indica si dentro de un listado de objetos TipoExpresion hay variables.
        /// </summary>
        /// <param name="listado">Listado de objetos</param>
        /// <returns></returns>
        private bool Tienevr(List<TipoExpresion> listado)
        {
            if (listado.Count > 0)
            {
                //RECORREMOS
                foreach (var item in listado)
                {
                    if (item.tipo == "vr")
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Indica si dentro de un listado de objeto TipoExpresion hay funciones.
        /// </summary>
        /// <param name="listado">Listado de objetos</param>
        /// <returns></returns>
        private bool TieneFunciones(List<TipoExpresion> listado)
        {
            if (listado.Count > 0)
            {
                //RECORREMOS
                foreach (var item in listado)
                {
                    if (item.tipo == "fn")
                        return true;
                }
            }

            return false;
        }

        //REEMPLAZAR TODAS LAS APARICIONES DE VARIABLES Y FUNCIONES POR CERO
        //SI RETORNA TRUE LA ESTRUCTURA ES VALIDA
        /// <summary>
        /// Setea en cero el valor de todas las funciones y variables en listado.
        /// </summary>
        /// <param name="listado">Listado de objetos.</param>
        /// <returns></returns>
        private bool ReemplazoFunciones(List<TipoExpresion> listado)
        {
            string cadena = "";
            if (listado.Count > 0)
            {
                foreach (var item in listado)
                {
                    if (item.tipo == "fn" || item.tipo == "vr")
                        item.Valor = "0";
                }

                //RECORREMOS NUEVAMENTE LA LISTA PARA GENERAR SU EQUIVALENTE EN FORMATO STRING
                foreach (var item in listado)
                {
                    cadena = cadena + item.Valor;
                }

                //SI LA CADENA TIENE EL SIGUIENTE FORMATO: MIN(12.5*10;2^3)+10
                //AL REEMPLAZAR QUEDARIA ASI: 0 + 10 (LO CUAL ES MAS FACIL VALIDAR SINTAXIS)

                int error = 0;
                //string msger = "";
                error = fnCodigoError(cadena);

                if (error != -1)
                {
                    //HAY ERROR
                    
                    return false;
                }
                else
                {
                    return true;
                }

            }

            return false;
        }

        //GENERAR CADENA ANTERIOR Y CADENA POSTERIOR EN CASO DE SER FUNCION
        /// <summary>
        /// Genera cadena anterior y posterior de una expresion MINMAX
        /// </summary>
        /// <param name="funcion">Expresion a evaluar.</param>
        private void GenerarAnteriorPosterior(string funcion)
        {
            string anterior = "", posterior = "";
            int position = 0;
            if (funcion.Length > 0)
            {
                //OBTENGO LA POSICION DEL ;
                position = funcion.IndexOf(";");
                anterior = funcion.Substring(4, position - 4);
                posterior = funcion.Substring(position + 1, (funcion.Length - (position + 1)) - 1);

                //SETEAMOS VARIABLES
                SetAnteriorMinMax(anterior);
                SetPosteriorMinMax(posterior);
            }
        }

        #endregion

        #region "CALCULO FUNCION IF"
        /// <summary>
        /// Realiza calculo de formula para funciones IF.
        /// </summary>
        /// <returns></returns>
        public bool CalculoIf()
        {
            string cadenaLimpia = "", cadenaVerdadera = "", cadenaFalsa = "", cadenaLogina = "", msgVerdad = "", msgFalso = "", verdadFinal = "", falsoFinal = "";
            bool cadenaCorrecta = false, valido = false;
            bool existeFn = false, existeVr = false, ValidarFunciones = false, ValidarVariables = false;
            //PARA GUARDAR CODIGOS DE ERROR
            int errorVerdad = 0, errorFalso = 0;

            //SOLO EN EL CASO DE QUE ALGUNA PARTE DE LA CADENA CONTENGA ALGUNA FUNCION MIN O MAX            
            List<TipoExpresion> objetos = new List<TipoExpresion>();

            //GENERAR LISTA PARA VERDAD Y FALSO
            List<string> listaVerdad = new List<string>();
            List<string> listaFalsa = new List<string>();

            //GENERA LISTA VALIDANDO VARIABLES
            List<string> listaValidaVerdad = new List<string>();
            List<string> listaValidadFalsa = new List<string>();

            //LIMPIAR CADENA
            cadenaLimpia = LimpiarExpresion();
            //GENERAR CADENA (SETEA VARIABLES LOGICA, VERDADERO, FALSO)
            cadenaCorrecta = GenerarCadena();
            if (cadenaCorrecta)
            {
                //SI CADENA SE GENERA CORRECTAMENTE PROSEGUIMOS...
                //OBTENEMOS PARTE VERDADERA Y PARTE FALSA
                objetos = ExtraeObjetos(GetCadenaOriginal());
                cadenaVerdadera = GetExpresionVerdadera();
                
                cadenaFalsa = GetExpresionFalsa();
                
                cadenaLogina = GetExpresionLogica();

                //.........................................................................
                //PREGUNTAR SI CADENA VERDADERA O CADENA FALSA CONTIENE UNA FUNCION MIN MAX
                //.........................................................................
                if (cadenaVerdadera.Contains("MIN") || cadenaVerdadera.Contains("MAX"))
                {
                    //VALIDAR FUNCION MIN
                    objetos = ExtraeObjetos(cadenaVerdadera);

                    /*PREGUNTAMOS SI LA CADENA CONTIENE FUNCIONES O VARIABLES Y VALIDAMOS QUE EXISTAN*/
                    if (objetos.Count > 0)
                    {
                        existeFn = TieneFunciones(objetos);
                        existeVr = Tienevr(objetos);

                        //EXISTEN LOS DOS...
                        if (existeFn && existeVr)
                        {
                            //VALIDAMOS VARIABLES Y FUNCIONES...
                            ValidarVariables = SintaxisVariables(objetos);
                            ValidarFunciones = SintaxisObjetos(objetos);
                            if (ValidarVariables && ValidarFunciones)
                            {
                                //VARIABLES Y FUNCIONES CORRECTAS
                                //REEMPLAZAMOS VARIABLES POR VALOR 0
                                valido = ReemplazoFunciones(objetos);
                                if (valido == false)
                                { XtraMessageBox.Show("Tienes un error de sintaxis en parte verdadera", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }
                            }
                            else
                            {
                                XtraMessageBox.Show("Variable no existe en parte verdadera", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                return false;
                            }
                        }
                        /*SOLO TIENE FUNCIONES*/
                        else if (existeFn && existeVr == false)
                        {
                            ValidarFunciones = SintaxisObjetos(objetos);
                            //FUNCION CORRECTA  
                            if (ValidarFunciones)
                            {
                                valido = ReemplazoFunciones(objetos);
                                if (valido == false)
                                { XtraMessageBox.Show("Tienes un error en parte verdadera", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }
                            }
                            else
                            { XtraMessageBox.Show("Funcion no existe en parte verdadera", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }
                        }
                        /*SOLO TIENE VARIABLES*/
                        else if (existeVr && existeFn == false)
                        {
                            ValidarVariables = SintaxisVariables(objetos);
                            if (ValidarVariables)
                            {
                                valido = ReemplazoFunciones(objetos);
                                if (valido == false)
                                { XtraMessageBox.Show("Error de sintaxis en parte verdadera", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }
                            }
                            else
                            { XtraMessageBox.Show("No existe variable en parte verdadera", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }
                        }
                    }
                }
                if (cadenaFalsa.Contains("MIN") || cadenaFalsa.Contains("MAX"))
                {
                    //VALIDAR FUNCION MIN MAX...

                    objetos = ExtraeObjetos(cadenaFalsa);

                    /*PREGUNTAMOS SI LA CADENA CONTIENE FUNCIONES O VARIABLES Y VALDAMOS QUE EXISTAN*/
                    if (objetos.Count > 0)
                    {
                        existeFn = TieneFunciones(objetos);
                        existeVr = Tienevr(objetos);

                        //EXISTEN LOS DOS...
                        if (existeFn && existeVr)
                        {
                            //VALIDAMOS VARIABLES Y FUNCIONES...
                            ValidarVariables = SintaxisVariables(objetos);
                            ValidarFunciones = SintaxisObjetos(objetos);
                            if (ValidarVariables && ValidarFunciones)
                            {
                                //VARIABLES Y FUNCIONES CORRECTAS
                                //REEMPLAZAMOS VARIABLES POR VALOR 0
                                valido = ReemplazoFunciones(objetos);
                                if (valido == false)
                                { XtraMessageBox.Show("Tienes un error de sintaxis en parte Falsa", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }
                            }
                            else
                            {
                                XtraMessageBox.Show("Variable no existe en parte Falsa", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                return false;
                            }
                        }
                        /*SOLO TIENE FUNCIONES*/
                        else if (existeFn && existeVr == false)
                        {
                            ValidarFunciones = SintaxisObjetos(objetos);
                            //FUNCION CORRECTA  
                            if (ValidarFunciones)
                            {
                                valido = ReemplazoFunciones(objetos);
                                if (valido == false)
                                { XtraMessageBox.Show("Tienes un error en parte Falsa", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }
                            }
                            else
                            { XtraMessageBox.Show("Funcion no existe en parte Falsa", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }
                        }
                        /*SOLO TIENE VARIABLES*/
                        else if (existeVr && existeFn == false)
                        {
                            ValidarVariables = SintaxisVariables(objetos);
                            if (ValidarVariables)
                            {
                                valido = ReemplazoFunciones(objetos);
                                if (valido == false)
                                { XtraMessageBox.Show("Error de sintaxis en parte Falsa", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }
                            }
                            else
                            { XtraMessageBox.Show("No existe variable en parte Falsa", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); return false; }
                        }
                    }
                }

                //SI NINGUNA DE LAS PARTES TIENE FUNCION MINMAX ENTONCES VALIDAMOS DE LA FORMA NORMAL...
                if (cadenaVerdadera.Contains("MIN") == false && cadenaFalsa.Contains("MAX") == false)
                {
                    errorVerdad = fnCodigoError(cadenaVerdadera);
                    errorFalso = fnCodigoError(cadenaFalsa);

                    msgVerdad = fnMensajeError(errorVerdad);
                    msgFalso = fnMensajeError(errorFalso);

                    listaVerdad = GeneraListaCadena(cadenaVerdadera);
                    listaFalsa = GeneraListaCadena(cadenaFalsa);

                    listaValidaVerdad = GeneraListaValida(listaVerdad);
                    listaValidadFalsa = GeneraListaValida(listaFalsa);

                    if (listaValidaVerdad == null)
                    {
                        XtraMessageBox.Show("Error en expresion verdadera", "Verdadera", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                    else if (listaValidadFalsa == null)
                    {
                        XtraMessageBox.Show("Error en expresion falsa", "Falsa", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }

                    if (errorVerdad == -1 && errorFalso == -1)
                    {
                        //NO HAY ERRORES MATEMATICOS
                        //VALIDAMOS VARIABLES EXISTAN...
                        //METODO GENERALISTAVARIANTE TWO NOS DEVUELVE LA LISTA FINAL PARA LUEGO REALIZAR EL CALCULO
                        //verdadFinal = GeneraListaVarianteTwo(cadenaVerdadera);          
                        //falsoFinal = GeneraListaVarianteTwo(cadenaFalsa);                

                        //SI NO ENTRA A LOS IF ES PORQUE LAS LISTAS GENERADAS SON VALIDAS Y TIENE VARIABLES VALIDAS
                        //VALIDAR QUE LA EXPRESION LOGICA TAMBIEN SEA CORRECTA
                        bool LogicaValida = EvaluarLogica();
                        if (LogicaValida == false)
                        {
                            XtraMessageBox.Show("Error en expresion logica", "Logica", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else if (errorVerdad != -1)
                    {
                        //ERROR EN PARTE VERDADERA
                        XtraMessageBox.Show(msgVerdad, "Error matematico", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                    else if (errorFalso != -1)
                    {
                        //ERROR EN PARTE FALSA
                        XtraMessageBox.Show(msgFalso, "Error matematico", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }

            }
            else
            {
                return false;
            }
            return false;
        }

        //METODO PARA SETEAR VARIABLES COMO GETANTESOPERADOR, GETDESPUESOPERADOR ETC
        //ESTO PARA NO VOLVER A LLAMAR  AL FUNCION CALCULO IF AL REALIZAZR EL CALCULO (CALCULO IF SOLO VALIDA)
        //YA SE VALIDO LA CADENA SOLO NECESITAMOS OBTENER LE VALOR CALCULADO
        /// <summary>
        /// Setea todas las variables de entrada necesarias para realizar el calculo.
        /// </summary>
        public void SetearCadenasIf()
        {
            //--> IF[exp>exp](expresion;expresion)
            string subcadenaLogica = "", cadenaCalculo = "";
            string[] partesLogicas = new string[2];
            string[] partesCalculo = new string[2];
            int posCierre = 0, posAbre = 0 ,largo = 0, posOpe = 0;
            if (GetCadenaOriginal() != "")
            {
                largo = GetCadenaOriginal().Length;
                //@EXTRAER PARTE LOGICA
                //OBTENEMOS LA POSICION DEL CORCHE DE CIERRE Y DE ABERTURA
                posAbre = GetCadenaOriginal().IndexOf('[');
                posCierre = GetCadenaOriginal().IndexOf(']');
                
                //OBTENEMOS SUBCADENA QUE REPRESENTA PARTE LOGICA 
                subcadenaLogica = GetCadenaOriginal().Substring(posAbre + 1, (posCierre - posAbre) - 1);
                SetExpresionLogica(subcadenaLogica);

                //EXTRAER PARTE ANTERIOR Y PARTE POSTERIOR DE CADENA LOGICA
                if (subcadenaLogica.Contains(">"))
                {
                    posOpe = subcadenaLogica.IndexOf(">");
                    SetOperadorLogico(">");
                }
                else if (subcadenaLogica.Contains("<"))
                {
                    posOpe = subcadenaLogica.IndexOf("<");
                    SetOperadorLogico("<");
                }
                else if (subcadenaLogica.Contains(">="))
                {
                    posOpe = subcadenaLogica.IndexOf("<>");
                    SetOperadorLogico("<>");
                }
                else if (subcadenaLogica.Contains("<="))
                {
                    posOpe = subcadenaLogica.IndexOf("<=");
                    SetOperadorLogico("<=");
                }

                //UNA VEZ OBTENIDO LA POSICION DEL OPERADOR EXTRAER CADA PARTE
                //LLAMAMOS A LA FUNCION PARTESLOGICAS LA CUAL DEVUELVE UN ARREGLO
                partesLogicas = CadenasLogicas(subcadenaLogica, posOpe);

                //SETEAMOS VARIABLES
                SetAntesOperador(partesLogicas[0]);
                setDespuesOperador(partesLogicas[1]);

                //SUPONEMOS QUE LO QUE ESTA DESPUES DEL CORCHETE DE CIERRE ES LA PARTE VERDAD Y FALSO
                cadenaCalculo = GetCadenaOriginal().Substring(posCierre + 2, (largo - (posCierre + 2))-1);
              
                //OBTENEMOS PARTE VERDADERA Y PARTE FALSA              
                partesCalculo = cadenaCalculo.Split(',');

                //SETEAMOS VARIABLES
                SetExpresionVerdadera(partesCalculo[0]);
                SetExpresionFalsa(partesCalculo[1]);             

            }
        }

        #endregion

        #region "EXPRESION MATEMATICA SIMPLE"
        //ASUMIMOS QUE YA SE SABE PREVIAMENTE QUE ES SOLO UNA EXPRESION MATEMATICA SIMPLE
        //VALIDAR SINTAXIS 
        private bool ValidaMatematica()
        {
            int error = 0;
            string mensajeError = "";
            bool validas = false;

            if (GetCadenaOriginal()!= "")
            {
                // VALIDAR CARACTER RAROS
                validas = CaracteresNoValidos(GetCadenaOriginal());
                if (validas == false) { XtraMessageBox.Show("Caracter no valido", "Caracteres", MessageBoxButtons.OK, MessageBoxIcon.Warning); return false; }

                List<TipoExpresion> objetos = new List<TipoExpresion>();
                objetos = ExtraeObjetos(GetCadenaOriginal());

                error = fnCodigoError(GetCadenaOriginal());
                if (error != -1)
                {
                    mensajeError = fnMensajeError(error);
                    XtraMessageBox.Show(mensajeError, "Error de Sintaxis", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                else
                {
                    //VALIDAR QUE LAS VARAIBLES (SI EXISTEN) SEAN VALIDAS
                    validas = ListaObjetosValida(objetos);
                    if (validas)
                    {
                        //SON VARIABLES VALIDAS
                        return true;
                    }
                    else
                    {
                        XtraMessageBox.Show("Variable no existe", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }
            }
            return false;                  
        }

        //NO SON VALIDAS , ; []
        /// <summary>
        /// Indica si hay algun caracter no válido dentro de la cadena de entrada.
        /// <para>No se permiten caracteres como ',' '[' ';' '>' etc.</para>
        /// </summary>
        /// <param name="cadena">Cadena a evaluar.</param>
        /// <returns></returns>
        private bool CaracteresNoValidos(string cadena)
        {
            if (cadena.Length>0)
            {
                if (cadena.Contains(",")) return false;
                if (cadena.Contains("[") || cadena.Contains("]")) return false;
                if (cadena.Contains(";")) return false;
                if (cadena.Contains(">") || cadena.Contains("<") || cadena.Contains("=")) return false;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Calcula una expresion matemática simple.
        /// </summary>
        /// <returns></returns>
        private double CalculoSimple()
        {           
            List<TipoExpresion> objetos = new List<TipoExpresion>();
            List<string> ListFromObject = new List<string>();
            string CadenaCalculo = "";
            double final = 0;         
            if (GetCadenaOriginal() != "")
            {
                //GENERAMOS LISTA DE OBJETOS A PARTIR DE LA CADENA                
                objetos = ExtraeObjetos(GetCadenaOriginal());
                //EXTRAEMOS LOS ELEMENTOS Y PASAMOS A LA LISTA SIMPLE
                foreach (var item in objetos)
                {                   
                    ListFromObject.Add(item.Valor);
                }

                //GENERAMOS LISTA CALCULADA
                CadenaCalculo = ListaCalculada(ListFromObject, getContrato(), getItemTrabajador());
                final = CalculoMatematico(CadenaCalculo);
            }            

            return final;
        }
        #endregion

        #region "FUNCION WHILE"

        //ESTRUCTURA PARA WHILE 
        //      WHILE(CONDICION)[EXPRESION MATEMATICA;CONTADOR]

        //PREGUNTAR SI EXISTE LA PALABRA WHILE DENTRO DE LA CADENA
        private bool fnExisteWhile()
        {
            string Expresion = "";
            if (getExpresionWhile() != "")
                Expresion = getExpresionWhile();

            int largo = 0;
            bool valido = false;
            largo = Expresion.Length;
            if (largo > 0)
            {
                if (Expresion[0] == 'W' && Expresion[1] == 'H' && Expresion[2] == 'I' && Expresion[3] == 'L' && Expresion[4] == 'E')
                {
                    //ES VALIDO!
                    valido = true;
                }
                else
                {
                    valido = false;
                }
            }
            else
            {
                valido = false;
            }

            return valido;

        }

        //PREGUNTAR SI DESPUES DEL WHILE VIENE UN PARENTESIS DE ABERTURA (PARTE LOGICA)
        private bool fnValidaLogicaWhile()
        {
            string cadena = "";
            if (getExpresionWhile() != "")
            {
                cadena = getExpresionWhile();
            }

            bool ExisteWhile = fnExisteWhile();
            if (ExisteWhile)
            {
                //VERIFICAR QUE DESPUES DE LA PALABRA WHILE VIENE UN PARENTESIS DE ABERTURA
                string sub = cadena.Substring(5, cadena.Length - 5);

                //SI NO CONTIENE PARENTESIS NO ES VALIDO
                if (!sub.Contains("(")) return false;
                if (!sub.Contains(")")) return false;

                if (sub[0] == '(')
                {
                    //CORRECTO
                    //PREGUNTAMOS SI CONTIENE PARENTESIS DE CIERRE ')'
                    if (sub.Contains(")"))
                    {
                        //EXTRAEMOS CADENA HASTA PARENTESIS DE CIERRE
                        int position = sub.IndexOf(")");
                        string parte = sub.Substring(0, position);

                        //VERIFICAMOS QUE DENTRO DE PARENTESIS NO ESTE VACIO
                        if (parte[0] == '(' && parte[1] == ')') return false;

                        //VALIDAR QUE DENTRO EXISTE ALGUN OPERADOR DE VERDAD VALIDO (> < >= <= <>)
                        int veces = 0;
                        string operador = "";
                        if (parte.Contains(">") || parte.Contains("<") || parte.Contains(">=") || parte.Contains("<=") || parte.Contains("<>"))
                        {
                            //RECORREMOS CADENA PARA VALIDAR QUE SOLO HAY UN TIPO DE OPERADOR (NO PUEDE HABER MAS DE UNO)
                            for (int i = 0; i < parte.Length; i++)
                            {
                                if ((parte[i] == '>' || parte[i] == '<' || parte[i] == '='))
                                {
                                    if (parte[i + 1] == '=' && parte[i] != '=')
                                    {
                                        //EN EL CASO DE QUE SEA >= O <=
                                        operador = parte[i] + "" + parte[i + 1];
                                        i = i + 1;
                                        veces++;
                                    }
                                    else if (parte[i + 1] == '>' && parte[i] == '<')
                                    {
                                        operador = parte[i] + "" + parte[i + 1];
                                        //aumentar la posicion en una de la cadena
                                        i = i + 1;
                                        veces++;
                                    }
                                    else if (parte[i] == '=')
                                    {
                                        //SOLO '='
                                        operador = parte[i].ToString();
                                        veces++;
                                    }
                                    else
                                    {
                                        operador = parte[i].ToString();
                                        veces++;
                                    }
                                }
                                else if (parte[i] == '=' && parte[i - 1] != '<' && parte[i - 1] != '>')
                                {
                                    //SOLO '='
                                    operador = parte[i].ToString();
                                    veces++;
                                }
                            }

                            //SI VECES ES MAYOR A UNO ES PORQUE HAY MAS DE UN OPERADOR DE VERDAD, NO VALIDO
                            if (veces > 1) { XtraMessageBox.Show("Hay mas de un operador", "Operador", MessageBoxButtons.OK, MessageBoxIcon.Warning); return false; }

                            //OBTENER LA POSICION DONDE ESTA EL OPERADOR
                            int posOperador = parte.IndexOf(operador);
                            if (posOperador == 1 || posOperador == parte.Length - 2) { MessageBox.Show("Posicion operador no es correcta"); return false; }

                            //SERIA VALIDA
                            //SETEAMOS VARIABLE CORRESPONDIENTE
                            setLogicaWhile(parte);

                            return true;
                        }
                        else
                        {
                            return false;
                        }

                      
                    }
                }
                else
                {
                    //ERROR
                    return false;
                }
            }
            else
            {
                return false;
            }
            return false;
        }

        //VALIDAR CADENA PARTE OPERATORIA (DESPUES DE LA PARTE LOGICA)
        private bool fnValidaOperatoriaWhile()
        {
            bool logica = false;
            string operatoria = "";
            int inicio = 0;
            logica = fnValidaLogicaWhile();
            if (logica)
            {
                //SI ES VERDAD ES PORQUE LA PARTE LOGICA ESTA CORRECTA
                if (getExpresionWhile() != "")
                {
                    operatoria = getExpresionWhile();
                    //EXTRAEMOS DE CADENA ORIGINAL LA PARTE OPERACIONAL
                    if (operatoria.Contains("[") && operatoria.Contains("]"))
                    {
                        inicio = operatoria.IndexOf("[");
                        operatoria = operatoria.Substring(inicio, operatoria.Length - inicio);

                        //SI AL FINAL DE LA CADENA NO HAY UN CORCHETE DE CIERRE NO ES CORRECTO!
                        if (operatoria[operatoria.Length] != ']') return false;

                        //VALIDAR QUE LOS CORCHETES NO ESTEN VACIOS
                        if (operatoria[0] == '[' && operatoria[1] == ']') return false;

                        //VALIDAR QUE SOLO HAYA UN CORCHETE DE ABERTURA Y UN CORCHETE DE CIERRE
                        int ab = 0, cie = 0;
                        for (int i = 0; i < operatoria.Length; i++)
                        {
                            if (operatoria[i] == '[') ab++;
                            if (operatoria[i] == ']') cie++;
                        }

                        if (ab == 0) return false;
                        if (cie == 0) return false;
                        if (ab == 1 && cie == 0) return false;
                        if (cie == 1 && ab == 0) return false;
                        if (ab > 1) return false;
                        if (cie > 1) return false;

                        //VALIDAR QUE EXISTE UN ';'
                        if (!operatoria.Contains(";")) return false;

                        //VALIDAR QUE SOLO HAYA UN ;
                        int coma = 0;
                        for (int i = 0; i < operatoria.Length; i++)
                        {
                            if (operatoria[i] == ';') coma++;
                        }

                        if (coma == 0) return false;
                        if (coma > 1) return false;

                        //SETEAR VARIABLE CORRESPONDIENTE
                        setOperatoriaWhile(operatoria);

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        //VALIDAR EXPRESIONES LOGICA Y EXPRESION OPERATORIA
        private void calculoWhile()
        {
            string logica = "", operatoria = "";
            bool cadenaValida = false;
            cadenaValida = fnValidaOperatoriaWhile();

            if (cadenaValida)
            {
                if (getLogicaWhile() != "") logica = getLogicaWhile();
                if (getOperatoriaWhile() != "") operatoria = getOperatoriaWhile();

                

            }
        }

        #endregion
    }

    /// <summary>
    /// Clase que nos permite representar los distintos elementos que existen en una expresion de fórmula.
    /// </summary>
    class TipoExpresion
    {
        /// <summary>
        /// Corresponde al valor del objeto o elemento.
        /// Puede ser un numero, una cadena, una variable, una funcion, etc.
        /// </summary>
        public string Valor { get; set; }
        /// <summary>
        /// Representa al tipo de objeto dentro de la expresion de fórmula.
        /// <para>Tipos:</para>
        /// <para>* vr: Corresponde a una variable. Puede ser un valor de item, una variable tipo sys. </para>
        /// <para>* nm: Numero.</para>
        /// <para>* fn: Funcion.</para>
        /// <para>* op: Operador matematico. </para>
        /// <para>* pr: Representa un caracter de parentesis ') ('</para>
        /// </summary>
        public string tipo { get; set; }
    }
}
