using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

/*
    El proyecto generea código ASM en: nasm o masm o ... excerpto emu8086

    1. Completar la asignacion
    2. Console.Write & Console.WriteLine
    3. Console.Read & Console.ReadLine
    4. Considerar el else en el IF
    5. Programar el while
    6. Programar el for

*/
namespace Semantica
{
    public class Lenguaje : Sintaxis
    {
        private List<Variable> listaVariables;
        private List<String> bloqueCodigo;
        private int cIFs, cDos, cWhiles, cFors;
        public Lenguaje()
        {
            log.WriteLine("Analizador Sintactico");
            asm.WriteLine("; Analizador Sintactico");
            asm.WriteLine("; Analizador Semantico");
            listaVariables = new List<Variable>();
            bloqueCodigo = new List<String>();
            cIFs = cDos = cWhiles = cFors = 1;
        }
        public Lenguaje(string nombre) : base(nombre)
        {
            log.WriteLine("Analizador Sintactico");
            asm.WriteLine("; Analizador Sintactico");
            asm.WriteLine("; Analizador Semantico");
            listaVariables = new List<Variable>();
            cIFs = cDos = cWhiles = cFors = 1;
            bloqueCodigo = new List<String>();
        }
        // Programa  -> Librerias? Main
        public void Programa()
        {
            if (Contenido == "using")
            {
                Librerias();
            }
            Main();
        }
        // Librerias -> using ListaLibrerias; Librerias?
        private void Librerias()
        {
            match("using");
            listaLibrerias();
            match(";");
            if (Contenido == "using")
            {
                Librerias();
            }
        }
        // ListaLibrerias -> identificador (.ListaLibrerias)?
        private void listaLibrerias()
        {
            match(Tipos.Identificador);
            if (Contenido == ".")
            {
                match(".");
                listaLibrerias();
            }
        }
        Variable.TipoDato getTipo(string TipoDato)
        {
            Variable.TipoDato tipo = Variable.TipoDato.Char;
            switch (TipoDato)
            {
                case "int": tipo = Variable.TipoDato.Int; break;
                case "float": tipo = Variable.TipoDato.Float; break;
            }
            return tipo;
        }
        // Variables -> tipo_dato Lista_identificadores;
        private void Variables()
        {
            Variable.TipoDato tipo = getTipo(Contenido);
            match(Tipos.TipoDato);
            listaIdentificadores(tipo);
            match(";");
        }
        private void imprimeVariables()
        {
            // log.WriteLine("Lista de variables");
            asm.WriteLine("\n.data");
            foreach (Variable v in listaVariables)
            {
                // log.WriteLine(v.getNombre() + " (" + v.getTipo() + ") = " + v.getValor());
                if (v.getTipo() == Variable.TipoDato.Char)
                {
                    asm.WriteLine("\t" + v.getNombre() + " db 0");
                }
                else if (v.getTipo() == Variable.TipoDato.Int)
                {
                    asm.WriteLine("\t" + v.getNombre() + " dd 0");
                }
                else
                {
                    asm.WriteLine("\t" + v.getNombre() + " dw 0 ");
                }
            }
        }
        // ListaIdentificadores -> identificador (,ListaIdentificadores)?
        private void listaIdentificadores(Variable.TipoDato t)
        {
            listaVariables.Add(new Variable(Contenido, t));
            match(Tipos.Identificador);
            if (Contenido == "=")
            {
                match("=");
                Expresion();
            }
            if (Contenido == ",")
            {
                match(",");
                listaIdentificadores(t);
            }
        }
        // BloqueInstrucciones -> { listaIntrucciones? }
        private void bloqueInstrucciones()
        {
            match("{");
            if (Contenido != "}")
            {
                listaIntrucciones();
            }
            match("}");
        }
        // ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void listaIntrucciones()
        {
            Instruccion();
            if (Contenido != "}")
            {
                listaIntrucciones();
            }
        }
        // Instruccion -> Console | If | While | do | For | Variables | Asignacion
        private void Instruccion()
        {
            if (Contenido == "Console")
            {
                console();
            }
            else if (Contenido == "if")
            {
                If();
            }
            else if (Contenido == "while")
            {
                While();
            }
            else if (Contenido == "do")
            {
                Do();
            }
            else if (Contenido == "for")
            {
                For();
            }
            else if (Clasificacion == Tipos.TipoDato)
            {
                Variables();
            }
            else
            {
                Asignacion();
                match(";");
            }
        }
        // Asignacion -> Identificador = Expresion;
        private void Asignacion()
        {
            string variable = Contenido;
            match(Tipos.Identificador);
            bloqueCodigo.Add("; Asignacion a " + variable);
            var v = listaVariables.Find(delegate (Variable x) { return x.getNombre() == variable; });
            float nuevoValor = v.getValor();

            if (Contenido == "=")
            {
                match("=");
                if (Contenido == "Console")
                {
                    match("Console");
                    match(".");
                    if (Contenido == "Read")
                    {
                        match("Read");
                    }
                    else
                    {
                        match("ReadLine");
                    }
                    match("(");
                    match(")");
                }
                else
                {
                    Expresion();
                    bloqueCodigo.Add("\tpop eax");
                    bloqueCodigo.Add("\tmov " + variable + ", eax");
                }
            }
            else if (Contenido == "++")
            {
                match("++");
                bloqueCodigo.Add("\tinc " + variable);
                nuevoValor++;
            }
            else if (Contenido == "--")
            {
                match("--");
                bloqueCodigo.Add("\tdec " + variable);
                nuevoValor--;
            }
            else if (Contenido == "+=")
            {
                match("+=");
                Expresion();
                bloqueCodigo.Add("\tpop eax");
            }
            else if (Contenido == "-=")
            {
                match("-=");
                Expresion();
                bloqueCodigo.Add("\tpop eax");
            }
            else if (Contenido == "*=")
            {
                match("*=");
                Expresion();
                bloqueCodigo.Add("\tpop eax");
            }
            else if (Contenido == "/=")
            {
                match("/=");
                Expresion();
                bloqueCodigo.Add("\tpop eax");
            }
            else
            {
                match("%=");
                Expresion();
                bloqueCodigo.Add("\tpop eax");
            }
            // match(";");            
            v.setValor(nuevoValor);
            // log.WriteLine(variable + " = " + nuevoValor);
            bloqueCodigo.Add("; Termina asignacion a " + variable);
        }
        // If -> if (Condicion) bloqueInstrucciones | instruccion
        // (else bloqueInstrucciones | instruccion)?
        private void If()
        {
            bloqueCodigo.Add("; if " + cIFs);
            string etiqueta = "_if" + cIFs++;
            match("if");
            match("(");
            Condicion(etiqueta, "");
            match(")");
            if (Contenido == "{")
            {
                bloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }
            if (Contenido == "else")
            {
                match("else");
                if (Contenido == "{")
                {
                    bloqueInstrucciones();
                }
                else
                {
                    Instruccion();
                }
            }
            bloqueCodigo.Add(etiqueta + ":");
            // Generar una etiqueta
        }
        // Condicion -> Expresion operadorRelacional Expresion
        private void Condicion(string etiqueta, string etiquetaMenorIgual)
        {
            Expresion(); // E1
            string operador = Contenido;
            match(Tipos.OpRelacional);
            Expresion(); // E2
            bloqueCodigo.Add("\tpop eax");
            bloqueCodigo.Add("\tpop ebx");
            bloqueCodigo.Add("\tcmp ebx, eax");
            switch (operador)
            {
                case ">":
                    bloqueCodigo.Add("\tjz " + etiqueta);
                    bloqueCodigo.Add("\tjc " + etiqueta);
                    break;
                case ">=":
                    bloqueCodigo.Add("\tjc " + etiqueta);
                    break;
                case "<":
                    bloqueCodigo.Add("\tjnc " + etiqueta);
                    break;
                case "<=":
                    bloqueCodigo.Add("\tjz " + etiquetaMenorIgual);
                    bloqueCodigo.Add("\tjnc " + etiqueta);
                    bloqueCodigo.Add(etiquetaMenorIgual + ":");

                    break;
                case "==":
                    bloqueCodigo.Add("\tjnZ " + etiqueta);
                    break;
                default:
                    bloqueCodigo.Add("\tje " + etiqueta);
                    break;

            }
        }
        // While -> while(Condicion) bloqueInstrucciones | instruccion
        private void While()
        {
            bloqueCodigo.Add("; while " + cWhiles);
            string etiquetaIni = "_whileIni" + cWhiles;
            string etiquetaFin = "_whileFin" + cWhiles;
            string etiquetaMenorIgual = "_whileMenorIgual" + cWhiles++;
            match("while");
            match("(");
            bloqueCodigo.Add(etiquetaIni + ":");
            Condicion(etiquetaFin, etiquetaMenorIgual);
            match(")");
            if (Contenido == "{")
            {
                bloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }
            bloqueCodigo.Add("jmp " + etiquetaIni);
            bloqueCodigo.Add(etiquetaFin + ":");
        }
        // Do -> do 
        //          bloqueInstrucciones | intruccion 
        //       while(Condicion);
        private void Do()
        {
            bloqueCodigo.Add("; do " + cDos);
            string etiqueta = "_do" + cDos++;
            bloqueCodigo.Add(etiqueta + ":");
            match("do");
            if (Contenido == "{")
            {
                bloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }
            match("while");
            match("(");
            Condicion(etiqueta, "");
            match(")");
            match(";");
        }
        // For -> for(Asignacion Condicion; Incremento) 
        //          BloqueInstrucciones | Intruccion
        private void For()
        {
            bloqueCodigo.Add("; for " + cFors);
            string etiquetaIni="_forIni"+cFors;
            string etiquetaFin="_forFin"+cFors;
            string etiquetaMenorIgual = "_forMenorIgual" + cFors++;
            match("for");
            match("(");
            Asignacion();
            match(";");
            bloqueCodigo.Add(etiquetaIni + ":");
            Condicion(etiquetaFin, etiquetaMenorIgual);
            match(";");
            Asignacion();
            match(")");
            if (Contenido == "{")
            {
                bloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }
            bloqueCodigo.Add("jmp " + etiquetaIni);
            bloqueCodigo.Add(etiquetaFin + ":");
        }
        // Console -> Console.(WriteLine|Write) (cadena?);
        private void console()
        {
            match("Console");
            match(".");
            if (Contenido == "WriteLine")
            {
                match("WriteLine");
            }
            else
            {
                match("Write");
            }
            match("(");
            if (Clasificacion == Tipos.Cadena)
            {
                match(Tipos.Cadena);
                if (Contenido == "+")
                {
                    listaConcatenacion();
                }
            }
            match(")");
            match(";");
        }
        private string listaConcatenacion()
        {
            match("+");
            match(Tipos.Identificador); // Validar que exista la variable
            if (Contenido == "+")
            {
                listaConcatenacion();
            }
            return "";
        }
        private void asm_Main()
        {
            asm.WriteLine();
            asm.WriteLine(".model flat,stdcall");
            asm.WriteLine(".stack 4096");

        }
        private void asm_endMain()
        {
            imprimeVariables();
            asm.WriteLine("\n.code");
            asm.WriteLine("ExitProcess PROTO, dwExitCode:DWORD");
            asm.WriteLine("main proc");
            foreach (String bloque in bloqueCodigo)
            {
                asm.WriteLine(bloque);
            }
            asm.WriteLine("\n\tINVOKE ExitProcess,0");
            asm.WriteLine("main endp");
            asm.WriteLine("\nend main");
        }
        // Main      -> static void Main(string[] args) BloqueInstrucciones 
        private void Main()
        {
            asm_Main();
            match("static");
            match("void");
            match("Main");
            match("(");
            match("string");
            match("[");
            match("]");
            match("args");
            match(")");
            bloqueInstrucciones();
            asm_endMain();
        }
        // Expresion -> Termino MasTermino
        private void Expresion()
        {
            Termino();
            MasTermino();
        }
        // MasTermino -> (OperadorTermino Termino)?
        private void MasTermino()
        {
            if (Clasificacion == Tipos.OpTermino)
            {
                string operador = Contenido;
                match(Tipos.OpTermino);
                Termino();
                bloqueCodigo.Add("\tpop ebx");
                bloqueCodigo.Add("\tpop eax");
                switch (operador)
                {
                    case "+":
                        bloqueCodigo.Add("\tadd eax, ebx");
                        bloqueCodigo.Add("\tpush eax");
                        break;
                    case "-":
                        bloqueCodigo.Add("\tsub eax, ebx");
                        bloqueCodigo.Add("\tpush eax");
                        break;
                }
            }
        }
        // Termino -> Factor PorFactor
        private void Termino()
        {
            Factor();
            PorFactor();
        }
        // PorFactor -> (OperadorFactor Factor)?
        private void PorFactor()
        {
            if (Clasificacion == Tipos.OpFactor)
            {
                string operador = Contenido;
                match(Tipos.OpFactor);
                Factor();
                bloqueCodigo.Add("\tpop ebx");
                bloqueCodigo.Add("\tpop eax");
                switch (operador)
                {
                    case "*":
                        bloqueCodigo.Add("\tmul ebx");
                        bloqueCodigo.Add("\tpush eax");
                        break;
                    case "/":
                        bloqueCodigo.Add("\tdiv ebx");
                        bloqueCodigo.Add("\tpush eax");
                        break;
                    case "%":
                        bloqueCodigo.Add("\tdiv ebx");
                        bloqueCodigo.Add("\tpush edx");
                        break;
                }
            }
        }
        // Factor -> numero | identificador | (Expresion)
        private void Factor()
        {
            if (Clasificacion == Tipos.Numero)
            {
                bloqueCodigo.Add("\tmov eax, " + Contenido);
                bloqueCodigo.Add("\tpush eax");
                match(Tipos.Numero);
            }
            else if (Clasificacion == Tipos.Identificador)
            {
                var v = listaVariables.Find(delegate (Variable x) { return x.getNombre() == Contenido; });
                bloqueCodigo.Add("\tmov eax, " + Contenido);
                bloqueCodigo.Add("\tpush eax");
                match(Tipos.Identificador);
            }
            else
            {
                match("(");
                Expresion();
                match(")");
            }
        }
    }
}