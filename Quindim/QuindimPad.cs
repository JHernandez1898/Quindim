﻿using Quindim.Clases;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Quindim
{
    public partial class QuindimPad : Form
    {
        public QuindimPad()
        {
            InitializeComponent();
        }

        private void QuindimPad_Load(object sender, EventArgs e)
        {
            EstablecerConexion();
        }

        #region Conexión
        public void EstablecerConexion()
        {
            //MessageBox.Show("Capture una instancia para la conexion", "Analizador Sintactico", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //btnleertodo.Enabled = false;
            //gbLexico.Enabled = false;
           //gbSintax.Enabled = false;
            //btnleertodo.Enabled = false;
            /*lblServidor.Text = "Servidor: " + System.Environment.MachineName;
            lblconexion.BackColor = Color.Red;
            txtServer.Focus();*/
        }

        /*private void BtnConectar_Click(object sender, EventArgs e)
        {
            try
            {
                if (ConexionMatriz.ProbarConexion(txtServer.Text))
                {
                    MessageBox.Show("Conectado al servidor", "Lexico", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    tabControl1.Enabled = true;
                    //btnleertodo.Enabled = true;
                    MetodosAL.Servidor = txtServer.Text;
                    lblconexion.BackColor = Color.Green;
                    MetodosAS.CrearMatriz();
                    MetodosSe.CrearMatriz();
                    //btnleertodo.Enabled = true;
                }
                else
                {
                    MessageBox.Show("Conexion fallida", "Error de conexion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnCaracterxCarter.Enabled = false;
                    //btnleertodo.Enabled = false;
                    lblconexion.BackColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error de conexion", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }*/

        #endregion  

        public string[] RellenarArreglo()
        {
            string[] ArregloLineas = new string[LineasTokens.Count];
            int i = 0;
            foreach (string strLinea in LineasTokens)
            {
                ArregloLineas[i] = strLinea;
                i++;
            }
            return ArregloLineas;
        }

        private void Btnleertodo_Click(object sender, EventArgs e)
        {
            
        }

        private void MostrarIdentificadoresConstantes()
        {
            dgvIDE.Rows.Clear();
            dgvConstatesNumericasEnteras.Rows.Clear();
            dgvConstatesNumericasReales.Rows.Clear();
            dgvConstantesExpo.Rows.Clear();
            foreach (Identificador IDE in MetodosAL.Identificadores)
                dgvIDE.Rows.Add("ID" + IDE.Index, IDE.Nombre, IDE.Tipo, "");
            foreach (NumericoEntero Num in MetodosAL.ConstantesNumericasEnteras)
                dgvConstatesNumericasEnteras.Rows.Add("CNE" + Num.Index, Num.Contenido);
            foreach (NumericoReal Real in MetodosAL.ConstantesNumericasReales)
                dgvConstatesNumericasReales.Rows.Add("CNR" + Real.Index, Real.Contenido);
            foreach (NumericoExponencial expo in MetodosAL.ConstantesNumericasExponenciales)
                dgvConstantesExpo.Rows.Add("CNEE" + expo.Index, expo.Contenido, expo.Exponencial);
            foreach (NumericoExpReal exporeal in MetodosAL.ConstantesNumericasExpReales)
                dgvConstantesExpo.Rows.Add("CNRE" + exporeal.Index, exporeal.Contenido, exporeal.Exponencial);
            dgvIDE.CurrentCell = null;
            dgvConstatesNumericasEnteras.CurrentCell = null;
            dgvConstatesNumericasReales.CurrentCell = null;
            dgvConstantesExpo.CurrentCell = null;
        }

        #region Léxico

        static bool principio = true;
        static bool linea = true;
        static List<string> LineasTokens = new List<string>();
        static int nLinea = 0;
        static string strActual = "";
        static int temp = 1;

        private void btnCaracterxCarter_Click(object sender, EventArgs e)
        {

            LineasTokens = Lexico.AnalizadorLexico(rtxtentrada.Text);
            string[] ArregloLineas = RellenarArreglo();
            if (principio)
            {
                principio = false;
                rtxtcodigointermediolexico.Text = "";
                rtxSintaxLineaxLinea.Text = "";
            }
            if (linea)
            {
                linea = false;
                strActual = RellenarArreglo()[nLinea];
                strActual = strActual.Substring(0, strActual.Length - 1);
                rtxtcodigointermediolexico.Text += ArregloLineas[nLinea] + "\n";
                txtcadenatokens.Text = ArregloLineas[nLinea];

                temp = strActual.Split(' ').Length;
            }
            txtTemporal.Text = temp.ToString();
            if (temp == 0) { MessageBox.Show("Error de sintaxis en línea " + (nLinea + 1)); nLinea = 0; principio = true; rtxtcodigointermediolexico.Text = " "; }
            else
            {
                string[] strSubcadenas = Sintaxis.CrearCombinaciones(temp, strActual);
                if (MetodosAS.DisminuirTemp(strSubcadenas, temp)) { txtTemporal.Text = temp.ToString(); temp--; }

                else
                {
                    foreach (string str in strSubcadenas)
                    {
                        string strCambio = "";

                        strCambio = Sintaxis.NormalizarCadena(str, temp);
                        strActual = strActual.Replace(str, MetodosAS.ObtenerConversion(strCambio));

                    }
                    rtxtcodigointermediolexico.Text += strActual + "\n";
                    temp = strActual.Split(' ').Length;

                }
                txtcadenatokens.Text = strActual;

                if (strActual == "S") { nLinea++; linea = true; rtxSintaxLineaxLinea.Text += "Línea " + nLinea.ToString() + ":S" + "\n"; }
            }
            if (LineasTokens.Count <= nLinea) { nLinea = 0; principio = true; }

        }



        static int indx = 0;
        static int palabra = 0;
        static int intEstadoActual = 0;
        static int lineax = 1;
        static List<char> caracteres = new List<char>();


        private void BtnCaracterxCarter_Click_1(object sender, EventArgs e)
        {
            try
            {
                string strEntrada = rtxtentrada.Text;
                if (indx == 0)
                {
                    Lexico.Depurar();
                    rtxtcodigointermediolexico.Text = "";
                }
                strEntrada = strEntrada.Replace('\n', ' ');
                string[] strPalabras = strEntrada.Split(' ');
                strEntrada = rtxtentrada.Text;
                List<string> tokens = new List<string>();
                txtSubcadena.Text = strPalabras[palabra];//Mostrar la subcadena actual
                string palabraActual = strPalabras[palabra];
                bool bandera = false;
                if (strEntrada.Length > indx)
                {
                    char c = strEntrada[indx];
                    caracteres.Add(c);
                    if (c != '\n')
                    {
                        txtEstadoAnt.Text = intEstadoActual.ToString();
                        txtCaracter.Text = c.ToString();
                        txtnumrenglon.Text = linea.ToString();
                        intEstadoActual = MetodosAL.NuevoEstado(c, intEstadoActual, ref bandera);
                        if (bandera)
                        {
                            string tokn = MetodosAL.ObtenerToken(intEstadoActual, caracteres);
                            tokens.Add(tokn);
                            txttoken.Text = tokn;
                            foreach (string tkn in tokens) txtcadenatokens.Text += tkn + " "; //Muestro la cadena de tokens
                            intEstadoActual = 0;
                            palabra++; //Avanzo a la siguiente palabra
                            bandera = false;
                            caracteres.Clear();
                        }
                        txtEstadoActual.Text = intEstadoActual.ToString();
                    }
                    else
                    {
                        lineax++;
                        CambiarEstado(' ', ref bandera, ref tokens);
                        palabra++;
                    }
                    indx++;
                }
                else
                {
                    CambiarEstado(' ', ref bandera, ref tokens);
                    indx = 0;
                    palabra = 0;
                    lineax = 1;
                }
                MostrarIdentificadoresConstantes();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + lineax + ".\nVerifique el uso apropiado del léxico, y el caracter actual.", "Error de analizador léxico", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void CambiarEstado(char c, ref bool bandera, ref List<string> tokens)
        {
            txtEstadoAnt.Text = intEstadoActual.ToString();
            intEstadoActual = MetodosAL.NuevoEstado(' ', intEstadoActual, ref bandera);
            txtEstadoActual.Text = intEstadoActual.ToString();
            string tokn = MetodosAL.ObtenerToken(intEstadoActual, caracteres);
            tokens.Add(tokn);
            txttoken.Text = tokn;
            foreach (string tkn in tokens) txtcadenatokens.Text += tkn + " "; //Muestro la cadena de tokens
            rtxtcodigointermediolexico.Text += txtcadenatokens.Text + "\n";
            txtcadenatokens.Text = "";
            caracteres.Clear();
            intEstadoActual = 0;
        }

        #endregion

        #region Sintáctico
        private void BtnLineaxLinea_Click(object sender, EventArgs e)
        {
            rtxtcodigointermediolexico.Text = "";
            LineasTokens = Lexico.AnalizadorLexico(rtxtentrada.Text);
            foreach (String token in LineasTokens)
            {
                rtxtcodigointermediolexico.Text += token + " ";
                rtxtcodigointermediolexico.Text += "\n";
            }
            MostrarIdentificadoresConstantes();
            string[] ArregloLineas = RellenarArreglo();
            if (principio)
            {
                principio = false;
                rtxtcodigointermediosintax.Text = "";
                rtxSintaxLineaxLinea.Text = "";
            }
            if (linea)
            {
                linea = false;
                strActual = RellenarArreglo()[nLinea];
                strActual = strActual.Substring(0, strActual.Length - 1);
                rtxtcodigointermediosintax.Text += ArregloLineas[nLinea] + "\n";
                tokenSintax.Text = ArregloLineas[nLinea];

                temp = strActual.Split(' ').Length;
            }
            txtTemporal.Text = temp.ToString();
            if (temp == 0) { MessageBox.Show("Error de sintaxis en línea " + (nLinea + 1)); nLinea = 0; principio = true; rtxtcodigointermediosintax.Text = " "; }
            else
            {
                string[] strSubcadenas = Sintaxis.CrearCombinaciones(temp, strActual);
                if (MetodosAS.DisminuirTemp(strSubcadenas, temp)) { txtTemporal.Text = temp.ToString(); temp--; }

                else
                {
                    foreach (string str in strSubcadenas)
                    {
                        string strCambio = "";

                        strCambio = Sintaxis.NormalizarCadena(str, temp);
                        strActual = strActual.Replace(str, MetodosAS.ObtenerConversion(strCambio));

                    }
                    rtxtcodigointermediosintax.Text += strActual + "\n";
                    temp = strActual.Split(' ').Length;

                }
                tokenSintax.Text = strActual;

                if (strActual == "S") { nLinea++; linea = true; rtxSintaxLineaxLinea.Text += "Línea " + nLinea.ToString() + ":S" + "\n"; }
            }
            if (LineasTokens.Count <= nLinea) { nLinea = 0; principio = true; }
        }
        #endregion

        #region Semántico

        public string[] RellenarArregloSemantica()
        {
            string[] ArregloLineas = new string[LineasTokensSmt.Count];
            int i = 0;
            foreach (string strLinea in LineasTokensSmt)
            {
                ArregloLineas[i] = strLinea;
                i++;
            }
            return ArregloLineas;
        }

        private void btnPrimeraPasada_Click(object sender, EventArgs e)
        {
            rchSemantica.Text = "";
            rchtxtSemantic.Text = "";
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            List<string> LineasTokens = Lexico.AnalizadorLexico(rtxtentrada.Text);
            try
            {
                foreach (string cadena in LineasTokens)
                {
                    strActual = cadena;
                    strActual = strActual.Substring(0, strActual.Length - 1);
                    string[] combinacionesde2 = Sintaxis.CrearCombinaciones(2, strActual);
                    foreach (string str in combinacionesde2)
                    {
                        string[] arreglo1 = str.Split(' ');
                        string aver = arreglo1[0].Substring(0, 2);
                        string aver2 = arreglo1[1].Substring(0, 1);
                        if (arreglo1[0].Substring(0, 3) == "TDD" && arreglo1[1].Substring(0, 2) == "ID")
                        {
                            string strIndex1 = arreglo1[1];
                            int index = int.Parse(strIndex1.Replace("ID", ""));
                            Identificador elemento = MetodosAL.Identificadores.Find(x => x.Index == index);
                            MetodosAL.Identificadores.Remove(elemento);
                            switch (arreglo1[0])
                            {
                                case "TDD1":
                                    elemento.Tipo = "INTE";
                                    break;
                                case "TDD2":
                                    elemento.Tipo = "DBLE";
                                    break;
                                case "TDD3":
                                    elemento.Tipo = "STRG";
                                    break;
                                case "TDD4":
                                    elemento.Tipo = "CHAR";
                                    break;
                                default:
                                    elemento.Tipo = "BOOL";
                                    break;
                            }
                            MetodosAL.Identificadores.Add(elemento);

                        }

                    }
                }
                foreach (string d in LineasTokens)
                {
                    rchSemantica.Text += MetodosSe.ObtenerArchivoTemporal(d) + "\n";
                }
                MostrarIdentificadoresConstantes();
                stopwatch.Stop();
                MessageBox.Show(stopwatch.Elapsed.ToString() + "ms", "Primera Pasada", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SegundaPasada_Click(object sender, EventArgs e)
        {
            rchSemantica.Text = "";
            rchtxtSemantic.Text = "";
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            List<string> LineasTokens = Lexico.AnalizadorLexico(rtxtentrada.Text);
            List<string> LineasSemantica = MetodosSe.PrimeraPasada(LineasTokens);
            int linea = 1;
            string strCambio;
            string strActual;
            int temp;
            MetodosSe.CrearMatriz();
            try
            {
                foreach (string cadena in LineasSemantica)
                {
                    strActual = cadena;
                    strActual = strActual.Substring(0, strActual.Length - 1);
                    rchSemantica.Text += cadena + "\n";
                    temp = strActual.Split(' ').Length;

                    while (temp > 0)
                    {
                        string[] strSubcadenas = MetodosSe.CrearCombinaciones(temp, strActual);
                        if (MetodosSe.DisminuirTemp(strSubcadenas, temp)) { temp--; }
                        else
                        {
                            foreach (string str in strSubcadenas)
                            {
                                strCambio = MetodosSe.NormalizarCadena(str, temp);
                                strActual = strActual.Replace(str, MetodosSe.ObtenerConversion(strCambio));
                            }
                            rchSemantica.Text += strActual + "\n";
                            temp = strActual.Split(' ').Length;
                        }
                        if (strActual == "S") { rchtxtSemantic.Text += "Línea " + linea.ToString() + ":S" + "\n"; temp = 0; linea++; }
                    }
                    if (strActual != "S") { rchtxtSemantic.Text += "Línea " + linea.ToString() + ":ERROR" + "\n"; MessageBox.Show("Semantica incorrecta en la línea: " + linea); linea++; }
                }

            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }


        }

        private void BtnTerceraPasada_Click(object sender, EventArgs e)
        {
            rchSemantica.Text = "";
            rchtxtSemantic.Text = "";
            List<string> LineasTokens = Lexico.AnalizadorLexico(rtxtentrada.Text);
            List<string> LineasSemantica = MetodosSe.PrimeraPasada(LineasTokens);
            int linea = 1;
            string strCambio;
            string strActual;
            int begins = 0;
            int ends = 0;
            int temp;
            MetodosSe.CrearMatriz();
            try
            {
                foreach (string cadena in LineasSemantica)
                {
                    strActual = cadena;
                    strActual = strActual.Substring(0, strActual.Length - 1);
                    rchSemantica.Text += cadena + "\n";
                    temp = strActual.Split(' ').Length;

                    while (temp > 0)
                    {
                        string[] strSubcadenas = MetodosSe.CrearCombinaciones(temp, strActual);
                        if (MetodosSe.DisminuirTemp(strSubcadenas, temp)) { temp--; }
                        else
                        {
                            foreach (string str in strSubcadenas)
                            {
                                strCambio = MetodosSe.NormalizarCadena(str, temp);
                                strActual = strActual.Replace(str, MetodosSe.ObtenerConversion(strCambio));
                            }
                            rchSemantica.Text += strActual + "\n";
                            temp = strActual.Split(' ').Length;
                        }
                        if (strActual == "S")
                        {
                            if (
                                (cadena.Contains("PR04") && cadena.Contains("PR20")) |
                                cadena.Contains("PR04") |
                                cadena.Contains("PR05") |
                                cadena.Contains("PR06") |
                                cadena.Contains("PR07") |
                                cadena.Contains("PR08") |
                                cadena.Contains("PR11") |
                                cadena.Contains("PR18") |
                                cadena.Contains("PR20")
                            ) begins++;
                            else
                            {
                                if (cadena.Contains("PR21")) ends++;
                            }
                            rchtxtSemantic.Text += "Línea " + linea.ToString() + ":S" + "\n";
                            temp = 0;
                            linea++;
                        }
                    }
                    if (strActual != "S") { rchtxtSemantic.Text += "Línea " + linea.ToString() + ":ERROR" + "\n"; MessageBox.Show("Semantica incorrecta en la línea: " + linea); linea++; }
                }
                if (begins - ends == 0) rchtxtSemantic.Text += "Bloque valido";
                else rchtxtSemantic.Text += "Bloque invalido";

            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }


        static bool principioSmt = true;
        static bool lineaSmt = true;
        static int nLineaSmt = 0;
        static string strActualSmt = "";
        static int tempSmt = 1;
        static List<char> caracteresSmt = new List<char>();
        List<string> LineasTokensSmt = new List<string>();

        private void LineaLineaSemantico_Click(object sender, EventArgs e)
        {
            LineasTokens = Lexico.AnalizadorLexico(rtxtentrada.Text);
            LineasTokensSmt = MetodosSe.PrimeraPasada(LineasTokens);
            MostrarIdentificadoresConstantes();
            string[] ArregloLineas = RellenarArregloSemantica();
            if (principioSmt)
            {
                principioSmt = false;
                rchSemantica.Text = "";
                rchtxtSemantic.Text = "";
            }
            if (lineaSmt)
            {
                lineaSmt = false;
                strActualSmt = RellenarArregloSemantica()[nLineaSmt];
                strActualSmt = strActualSmt.Substring(0, strActualSmt.Length - 1);
                rchSemantica.Text += ArregloLineas[nLineaSmt] + "\n";
                tokenSemantica.Text = ArregloLineas[nLineaSmt];
                tempSmt = strActualSmt.Split(' ').Length;
            }
            txtTemporalSemantica.Text = tempSmt.ToString();
            if (tempSmt == 0) { MessageBox.Show("Error de semántica en línea " + (nLineaSmt + 1)); nLineaSmt = 0; principioSmt = true; rchSemantica.Text = " "; }
            else
            {
                string[] strSubcadenas = MetodosSe.CrearCombinaciones(tempSmt, strActualSmt);
                if (MetodosSe.DisminuirTemp(strSubcadenas, tempSmt)) { txtTemporalSemantica.Text = tempSmt.ToString(); tempSmt--; }
                else
                {
                    foreach (string str in strSubcadenas)
                    {
                        string strCambio = "";
                        strCambio = MetodosSe.NormalizarCadena(str, tempSmt);
                        strActualSmt = strActualSmt.Replace(str, MetodosSe.ObtenerConversion(strCambio));
                    }
                    rchSemantica.Text += strActualSmt + "\n";
                    tempSmt = strActualSmt.Split(' ').Length;
                }
                tokenSemantica.Text = strActualSmt;
                if (strActualSmt == "S") { nLineaSmt++; lineaSmt = true; rchtxtSemantic.Text += "Línea " + nLineaSmt.ToString() + ":S" + "\n"; }
            }
            if (LineasTokensSmt.Count <= nLineaSmt) { nLineaSmt = 0; principioSmt = true; }
        }
        #endregion

        #region Código intermedio

        List<string> postFijo(string strTokens)
        {
            List<string> loQueSeRegresa = new List<string>();
            var lineas = strTokens.Split('\n');
            string tempLinea = "";
            foreach (string linea in lineas)
            {
                var Tokens = linea.Split(' ');
                bool banderaNumero = false;
                bool banderaIdentificador = false;
                bool bandera = false;
                string tokenIdentificador = "";
                foreach (string token in Tokens)
                {
                    if (bandera)
                        tempLinea += token + ' ';
                    else if (banderaIdentificador)
                    {
                        if (token.Contains("OP"))
                        {
                            if (token.Contains("OPA") || token.Contains("OPR") || token.Contains("OL0"))
                            {
                                tempLinea += tokenIdentificador + ' ' + token + ' ';
                                bandera = true;
                            }
                            else
                                banderaIdentificador = false;
                        }
                        else
                            banderaIdentificador = false;
                    }
                    else if (banderaNumero)
                    {
                        if (token.Contains("OPA") || token.Contains("OPR") || token.Contains("OL0"))
                        {
                            tempLinea += token + ' ';
                            bandera = true;
                        }
                    }
                    else if (token.Contains("CNE") || token.Contains("CNR"))
                    {
                        banderaNumero = true;
                        tempLinea += token + ' ';
                    }
                    else if (token.Contains("ID")||token.Contains("CN"))
                    {
                        banderaIdentificador = true;
                        tokenIdentificador = token;
                    }
                }
                if (tempLinea != "")
                {
                    tempLinea.Remove(tempLinea.Length - 1);
                    bandera = false;
                    banderaNumero = false;
                    banderaIdentificador = false;
                    loQueSeRegresa.Add(Reordenar(tempLinea));
                }
                tempLinea = "";
            }
            return loQueSeRegresa;
        }
        
        int jerarquiaOperador(String operador)
        {
            switch (operador)
            {                
                case "OPA3": // ^
                    return 8;  
                case "OPA1":// *                                                  
                case "OPA2":// /
                    return 7;                
                case "OPA4": // +                                              
                case "OPA5": // -
                    return 6;
                case "OPR4": // <=                 
                case "OPR3": // <                    
                case "OPR2": // >=                    
                case "OPR1": // >
                    return 5;                                
                case "OL03": // !
                    return 4;                                
                case "OL01": // &
                    return 3;                                
                case "OL02": // |
                    return 2;                                
                case "OPA6": // =
                    return 1;                
                default:
                    return 0;
            }
        }

        string Reordenar(string strCadenaTokens)
        {
            Stack<string> pilaTokens = new Stack<string>();

            string[] cadenaTokens = strCadenaTokens.Split(' ');
            string strNumeritos = "";            
            int operador1 = 0;
            int operador2 = 0;
            int contadorParentesis = 0;
            string subCadenaParentesis = "";
            bool banderaParentesis = false;

            foreach (string token in cadenaTokens)
            {
                if (banderaParentesis)
                {  
                    if (token.Contains("PAR2"))
                    {
                        contadorParentesis--;
                        if (contadorParentesis == 0)
                        {
                            subCadenaParentesis = subCadenaParentesis.Remove(subCadenaParentesis.Length - 1);
                            strNumeritos += Reordenar(subCadenaParentesis)+ " ";
                            subCadenaParentesis = "";
                            banderaParentesis = false;
                        }
                    }
                    else if (token.Contains("PAR1"))                    
                        contadorParentesis++;                    
                    else                    
                        subCadenaParentesis += token + ' ';                    
                }
                else
                {
                    if (token.Contains("CNE") || token.Contains("CNR") || token.Contains("ID"))
                        strNumeritos += token + " ";
                    if (token.Contains("OPA") || token.Contains("OPR") || token.Contains("OL0"))
                        if (operador1 == 0)
                        {
                            operador1 = jerarquiaOperador(token);
                            pilaTokens.Push(token);
                        }
                        else
                        {
                            operador2 = jerarquiaOperador(token);
                            if (operador1 < operador2)                            
                                pilaTokens.Push(token);                            
                            else if (operador2 < operador1)
                            {
                                string tokenDePila = pilaTokens.Pop();
                                operador1 = operador2;
                                pilaTokens.Push(token);
                                strNumeritos += tokenDePila + " ";
                            }
                            else if (operador2 == operador1)
                            {
                                strNumeritos += pilaTokens.Pop() + " ";
                                pilaTokens.Push(token);
                            }
                        }
                    if (token.Contains("PAR1"))
                    {
                        contadorParentesis++;
                        banderaParentesis = true;
                    }
                }
            }
            foreach (string operadorEnPila in pilaTokens)
            {
                strNumeritos += operadorEnPila + ' ';
            }
            return strNumeritos.Remove(strNumeritos.Length - 1);
        }

        #endregion

        private void Rtxtentrada_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                LeerTodo();
            }
        }

        public void LeerTodo() {
            MetodosAL.Depurar();
            rtxtcodigointermediolexico.Text = "";
            rtxtcodigointermediosintax.Text = "";
            rtxSintaxLineaxLinea.Text = "";

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                //LEXICO
                List<string> LineasTokens;
                LineasTokens = Lexico.AnalizadorLexico(rtxtentrada.Text);
                foreach (String token in LineasTokens)
                {
                    rtxtcodigointermediolexico.Text += token + " ";
                    rtxtcodigointermediolexico.Text += "\n";
                }

                //SINTAXIS
                List<string> SintaxResult = Sintaxis.AnalisisSintactico(LineasTokens);
                rtxtcodigointermediosintax.Text = SintaxResult[0];
                rtxSintaxLineaxLinea.Text = SintaxResult[1];


                //SEMANTICA
                List<string> LineasSemantica = MetodosSe.PrimeraPasada(LineasTokens);
                List<string> bottomupSemantica = MetodosSe.SegundaPasada(LineasSemantica);
                rchSemantica.Text = "";
                rchtxtSemantic.Text = "";
                rchSemantica.Text = bottomupSemantica[0];
                rchtxtSemantic.Text = bottomupSemantica[1];



            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            stopwatch.Stop();
            MessageBox.Show(stopwatch.Elapsed.ToString() + "ms", " Compilacion ", MessageBoxButtons.OK, MessageBoxIcon.Information);

            MostrarIdentificadoresConstantes();
            //PostFijo
            List<string> cadenasPostFijo = postFijo(rtxtcodigointermediolexico.Text);
            MostrarPostFijos(cadenasPostFijo);
        }

        private void LeerTodoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LeerTodo();
        }

        private void InstanciasSQLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings settings = new Settings();
            this.Hide();
            settings.ShowDialog();
            this.Close();
        }

        private void RUNToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void AbriToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void CargarEntradaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rtxtentrada.Text = "int num\nread num\nint res = Calcfact ( num )\nprint ( res )\nfunction int Calcfact ( int num )\nif ( num == 0 )\nreturn ( 1 )\nend\nelse\nfor ( int x = num to num > 1 step x = x - 1 )\nint R = R * x\nend\nend\nreturn ( R )\nend";
        }

        void MostrarPostFijos(List<string> lineas)
        {
            rtxtPostFijos.Text = "";
            foreach (string linea in lineas)
            {
                rtxtPostFijos.Text += linea + "\n";
            }
        }
     
    }
    
}