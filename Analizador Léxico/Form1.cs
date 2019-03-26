﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using Analizador_Léxico.Clases;
using System.Diagnostics;


namespace Analizador_Léxico
{
    public partial class Form1 : Form
    {

        string strEntrada;
        char[] strCaracteres;
        bool BanderaCaracteres=false;
        int ContadorArreglo = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnleertodo_Click(object sender, EventArgs e)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            //Anlizador Lexico
            rtxtcodigointermedio.Text = "";
            string strEntrada = rtxtentrada.Text;
            string[] strLineas = strEntrada.Split('\n');
            foreach (string Linea in strLineas)
            {
                List<string> tokens = new List<string>();
                MetodosAL.ObtenerToken(Linea, ref tokens);
                if (Linea != "")
                {
                    foreach (string token in tokens) rtxtcodigointermedio.Text += token + " ";
                    rtxtcodigointermedio.Text += "\n";
                }
            }
            MostrarIdentificadoresConstantes();
            Depurar();

            stopwatch.Stop();
            MessageBox.Show(stopwatch.Elapsed.ToString() + "ms", "Analizador léxico", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Depurar()
        {
            MetodosAL.Identificadores.Clear();
            MetodosAL.ConstantesNumericasEnteras.Clear();
            MetodosAL.ConstantesNumericasReales.Clear();
            MetodosAL.ConstantesNumericasExponenciales.Clear();
            MetodosAL.ConstantesNumericasExpReales.Clear();
        }

        private void MostrarIdentificadoresConstantes()
        {
            dgvIDE.Rows.Clear();
            dgvConstatesNumericas.Rows.Clear();
            dgvConstantesExpo.Rows.Clear();
            foreach (Identificador IDE in MetodosAL.Identificadores)
                dgvIDE.Rows.Add("ID"+IDE.Index, IDE.Nombre, "", "");
            foreach (NumericoEntero Num in MetodosAL.ConstantesNumericasEnteras)
                dgvConstatesNumericas.Rows.Add("CNE" + Num.Index, Num.Contenido);
            foreach (NumericoReal Real in MetodosAL.ConstantesNumericasReales)
                dgvConstatesNumericas.Rows.Add("CNR" + Real.Index, Real.Contenido);
            foreach (NumericoExponencial expo in MetodosAL.ConstantesNumericasExponenciales)
                dgvConstantesExpo.Rows.Add("CNEE" + expo.Index, expo.Contenido, expo.Exponencial);
            foreach (NumericoExpReal exporeal in MetodosAL.ConstantesNumericasExpReales)
                dgvConstantesExpo.Rows.Add("CNRE" + exporeal.Index, exporeal.Contenido, exporeal.Exponencial);
        }

        private void btnCaracterXCaracter_Click(object sender, EventArgs e)
        {
            if(BanderaCaracteres==false)
            {
                ContadorArreglo = 0;
                txtEstadoAnt.Text = "";
                string strEntrada = rtxtentrada.Text;
                strCaracteres =rtxtentrada.Text.ToCharArray();
                BanderaCaracteres = true;
                txtCaracter.Text = strCaracteres[ContadorArreglo].ToString();
                txtEstadoActual.Text = MetodosAL.CaracterPorCaracter(strCaracteres[ContadorArreglo], 0).ToString();
                ContadorArreglo++;
            }
            else
            {
                if(ContadorArreglo == strCaracteres.Length)
                {
                    txtEstadoAnt.Text = txtEstadoActual.Text;
                    txtEstadoActual.Text=MetodosAL.CaracterPorCaracter(' ', int.Parse(txtEstadoActual.Text)).ToString();
                    txtCaracter.Text = ' '+"del";
                    rtxtcodigointermedio.Text+= MetodosAL.ObtenerToken(int.Parse(txtEstadoActual.Text))+'\n';
                    txtEstadoAnt.Text = txtEstadoActual.Text;
                    txtEstadoActual.Text = "0";
                    BanderaCaracteres = false;
                    MostrarIdentificadoresConstantes();
                    return;
                }
                if(txtEstadoActual.Text == "196")
                {
                    txtEstadoAnt.Text = txtEstadoActual.Text;
                    txtEstadoActual.Text = MetodosAL.CaracterPorCaracter('/', int.Parse(txtEstadoActual.Text)).ToString();
                    txtCaracter.Text = '/' + "del";
                    rtxtcodigointermedio.Text += MetodosAL.ObtenerToken(int.Parse(txtEstadoActual.Text)) + '\n';
                    txtEstadoAnt.Text = txtEstadoActual.Text;
                    if (ContadorArreglo < strCaracteres.Length)
                    {
                        txtEstadoActual.Text = "0";
                        ContadorArreglo+=2;
                        return;
                    }
                    else
                    {
                        txtEstadoActual.Text = "0";
                        BanderaCaracteres = false;
                        MostrarIdentificadoresConstantes();
                        return;
                    }
                }
                if((strCaracteres[ContadorArreglo]==' '&&txtEstadoActual.Text!="195"))
                {
                    txtEstadoAnt.Text = txtEstadoActual.Text;
                    txtEstadoActual.Text = MetodosAL.CaracterPorCaracter(' ', int.Parse(txtEstadoActual.Text)).ToString();
                    txtCaracter.Text = ' ' + "del";
                    rtxtcodigointermedio.Text += MetodosAL.ObtenerToken(int.Parse(txtEstadoActual.Text)) + ' ';
                    txtEstadoAnt.Text = txtEstadoActual.Text;                   
                    ContadorArreglo++;
                    txtEstadoActual.Text = MetodosAL.CaracterPorCaracter(strCaracteres[ContadorArreglo], int.Parse(txtEstadoActual.Text)).ToString();
                    MostrarIdentificadoresConstantes();
                    return;
                }
                if(strCaracteres[ContadorArreglo]=='\n')
                {
                   
                    txtEstadoAnt.Text = txtEstadoActual.Text;
                    txtEstadoActual.Text = MetodosAL.CaracterPorCaracter(' ', int.Parse(txtEstadoActual.Text)).ToString();
                    txtCaracter.Text = ' ' + "del";
                    rtxtcodigointermedio.Text += MetodosAL.ObtenerToken(int.Parse(txtEstadoActual.Text)) + '\n';
                    txtEstadoAnt.Text = txtEstadoActual.Text;
                    ContadorArreglo++;
                    txtEstadoActual.Text = MetodosAL.CaracterPorCaracter(strCaracteres[ContadorArreglo], int.Parse(txtEstadoActual.Text)).ToString();
                    MostrarIdentificadoresConstantes();
                }
                if (MetodosAL.ObtenerToken(int.Parse(txtEstadoActual.Text))!="")
                {
                    MetodosAL.ObtenerToken(int.Parse(txtEstadoActual.Text));
                    txtEstadoAnt.Text = txtEstadoActual.Text;
                    txtEstadoActual.Text = "0";
                }
                else
                {
                    txtEstadoAnt.Text = txtEstadoActual.Text;
                    txtCaracter.Text = strCaracteres[ContadorArreglo].ToString();
                    txtEstadoActual.Text = MetodosAL.CaracterPorCaracter(strCaracteres[ContadorArreglo], int.Parse(txtEstadoActual.Text)).ToString();
                    ContadorArreglo++;
                }
            }
            
        }

        int intContadorPalabras = 0;
        int intCantidadPalabras = 0;
        int intLinea = 1;
        string[] strPalabras;

        private void btnleersiguiente_Click(object sender, EventArgs e)
        {
            if (intContadorPalabras <= intCantidadPalabras)
            {
                if (intContadorPalabras == 0)
                {
                    rtxtcodigointermedio.Text = "";
                    string strEntrada = rtxtentrada.Text;
                    strPalabras = strEntrada.Split(' ');
                    intCantidadPalabras = strPalabras.Length - 1;
                }

                List<string> tokens = new List<string>();
                string strPalabra = strPalabras[intContadorPalabras];
                if (!strPalabras[intContadorPalabras].Contains("\n"))
                {                    
                    MetodosAL.ObtenerToken(strPalabras[intContadorPalabras], ref tokens);
                    foreach (string token in tokens)
                    {
                        rtxtcodigointermedio.Text += token + " ";
                        txttoken.Text = token;
                    }
                    rtxtcodigointermedio.Text += " ";
                    MostrarIdentificadoresConstantes();
                }
                else
                {
                    string strPalabraEspaciada = strPalabras[intContadorPalabras];
                    if (strPalabras[intContadorPalabras] != "\n")
                    {                        
                        strPalabraEspaciada = strPalabraEspaciada.Replace("\n","");
                    }
                    MetodosAL.ObtenerToken(strPalabraEspaciada, ref tokens);
                    foreach (string token in tokens)
                    {
                        rtxtcodigointermedio.Text += token + " ";
                        txttoken.Text = token;
                    }
                    rtxtcodigointermedio.Text += " ";
                    rtxtcodigointermedio.Text += " \n";
                    MostrarIdentificadoresConstantes();
                    intLinea++;                    
                }
                txtnumrenglon.Text = intLinea.ToString();
                intContadorPalabras++;
            }
            else
            {
                intContadorPalabras = 0;
                intCantidadPalabras = 0;
                intLinea = 1;
                Depurar();
            }
        }
    }

}
