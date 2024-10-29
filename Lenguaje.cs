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
        private int cIFs, cDos, cWhiles;
        public Lenguaje()
        {
            log.WriteLine("Analizador Sintactico");
            asm.WriteLine("; Analizador Sintactico");
            asm.WriteLine("; Analizador Semantico");
            listaVariables = new List<Variable>();
            bloqueCodigo = new List<String>();
            cIFs = cDos = 1;
        }
        public Lenguaje(string nombre) : base(nombre)
        {
            log.WriteLine("Analizador Sintactico");
            asm.WriteLine("; Analizador Sintactico");
            asm.WriteLine("; Analizador Semantico");
            listaVariables = new List<Variable>();
            cIFs = cDos = 1;
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
            //asm.WriteLine("; Asignacion a " + variable);
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
                    //asm.WriteLine("\tpop eax");
                    bloqueCodigo.Add("\tpop eax");
                    //asm.WriteLine("\tmov " + variable + ", eax");
                    bloqueCodigo.Add("\tmov " + variable + ", eax");
                }
            }
            else if (Contenido == "++")
            {
                match("++");
                //asm.WriteLine("\tinc " + variable);
                bloqueCodigo.Add("\tinc " + variable);
                nuevoValor++;
            }
            else if (Contenido == "--")
            {
                match("--");
                //asm.WriteLine("\tdec " + variable);
                bloqueCodigo.Add("\tdec " + variable);
                nuevoValor--;
            }
            else if (Contenido == "+=")
            {
                match("+=");
                Expresion();
                //asm.WriteLine("\tpop eax");
                bloqueCodigo.Add("\tpop eax");
            }
            else if (Contenido == "-=")
            {
                match("-=");
                Expresion();
                //asm.WriteLine("\tpop eax");
                bloqueCodigo.Add("\tpop eax");
            }
            else if (Contenido == "*=")
            {
                match("*=");
                Expresion();
                //asm.WriteLine("\tpop eax");
                bloqueCodigo.Add("\tpop eax");
            }
            else if (Contenido == "/=")
            {
                match("/=");
                Expresion();
                //asm.WriteLine("\tpop eax");
                bloqueCodigo.Add("\tpop eax");
            }
            else
            {
                match("%=");
                Expresion();
                //asm.WriteLine("\tpop eax");
                bloqueCodigo.Add("\tpop eax");
            }
            // match(";");            
            v.setValor(nuevoValor);
            // log.WriteLine(variable + " = " + nuevoValor);
            //asm.WriteLine("; Termina asignacion a " + variable);
            bloqueCodigo.Add("; Termina asignacion a " + variable);
        }
        // If -> if (Condicion) bloqueInstrucciones | instruccion
        // (else bloqueInstrucciones | instruccion)?
        private void If()
        {
            //asm.WriteLine("; if " + cIFs);
            bloqueCodigo.Add("; if " + cIFs);
            string etiqueta = "_if" + cIFs++;
            match("if");
            match("(");
            Condicion(etiqueta);
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
            //asm.WriteLine(etiqueta + ":");
            bloqueCodigo.Add(etiqueta + ":");
            // Generar una etiqueta
        }
        // Condicion -> Expresion operadorRelacional Expresion
        private void Condicion(string etiqueta)
        {
            Expresion(); // E1
            string operador = Contenido;
            match(Tipos.OpRelacional);
            Expresion(); // E2
            //asm.WriteLine("\tpop eax");
            bloqueCodigo.Add("\tpop eax");
            //asm.WriteLine("\tpop ebx");
            bloqueCodigo.Add("\tpop ebx");
            //asm.WriteLine("\tcmp eax, ebx");
            bloqueCodigo.Add("\tcmp eax, ebx");
            switch (operador)
            {
                case ">": 
                case ">=": 
                case "<": 
                case "<=": 
                case "==": //asm.WriteLine("\tjne "+etiqueta);
                            bloqueCodigo.Add("\tjne "+etiqueta);
                           break;
                default:   //asm.WriteLine("\tje "+etiqueta);
                            bloqueCodigo.Add("\tje "+etiqueta);
                           break;
                           
            }
        }
        // While -> while(Condicion) bloqueInstrucciones | instruccion
        private void While()
        {
            //asm.WriteLine("; while " + ++cWhiles);
            bloqueCodigo.Add("; while " + cWhiles);
            string etiquetaIni = "_whileIni" + cWhiles; 
            string etiquetaFin = "_whileFin" + cWhiles; 
            match("while");
            match("(");
            //asm.WriteLine(etiquetaIni + ":");
            bloqueCodigo.Add(etiquetaIni + ":");
            Condicion(etiquetaFin);
            match(")");
            if (Contenido == "{")
            {
                bloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }
            //asm.WriteLine("jmp "+etiquetaIni);
            bloqueCodigo.Add("jmp "+etiquetaIni);
            //asm.WriteLine(etiquetaFin + ":");
            bloqueCodigo.Add(etiquetaFin + ":");
        }
        // Do -> do 
        //          bloqueInstrucciones | intruccion 
        //       while(Condicion);
        private void Do()
        {
            //asm.WriteLine("; do " + cDos);
            bloqueCodigo.Add("; do " + cDos);
            string etiqueta = "_do" + cDos++;
            //asm.WriteLine(etiqueta + ":");
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
            Condicion(etiqueta);
            match(")");
            match(";");
        }
        // For -> for(Asignacion Condicion; Incremento) 
        //          BloqueInstrucciones | Intruccion
        private void For()
        {
            match("for");
            match("(");
            Asignacion();
            match(";");
            Condicion("");
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
            //asm.WriteLine("\tadd esp, 4\n");
            //asm.WriteLine("\tmov eax, 1");
            //asm.WriteLine("\txor ebx, ebx");
            //asm.WriteLine("\tint 0x80");
            imprimeVariables();
            asm.WriteLine("\n.code");
            asm.WriteLine("ExitProcess PROTO, dwExitCode:DWORD");
            asm.WriteLine("main proc");
            foreach(String bloque in bloqueCodigo){
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
                //asm.WriteLine("\tpop ebx");
                bloqueCodigo.Add("\tpop ebx");
                //asm.WriteLine("\tpop eax");
                bloqueCodigo.Add("\tpop eax");
                switch (operador)
                {
                    case "+":
                        //asm.WriteLine("\tadd eax, ebx");
                        bloqueCodigo.Add("\tadd eax, ebx");
                        //asm.WriteLine("\tpush eax");
                        bloqueCodigo.Add("\tpush eax");
                        break;
                    case "-":
                        //asm.WriteLine("\tsub eax, ebx");
                        bloqueCodigo.Add("\tsub eax, ebx");
                        //asm.WriteLine("\tpush eax");
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
                //asm.WriteLine("\tpop ebx");
                bloqueCodigo.Add("\tpop ebx");
                //asm.WriteLine("\tpop eax");
                bloqueCodigo.Add("\tpop eax");
                switch (operador)
                {
                    case "*":
                        //asm.WriteLine("\tmul ebx");
                        bloqueCodigo.Add("\tmul ebx");
                        //asm.WriteLine("\tpush eax");
                        bloqueCodigo.Add("\tpush eax");
                        break;
                    case "/":
                        //asm.WriteLine("\tdiv ebx");
                        bloqueCodigo.Add("\tdiv ebx");
                        //asm.WriteLine("\tpush eax");
                        bloqueCodigo.Add("\tpush eax");
                        break;
                    case "%":
                        //asm.WriteLine("\tdiv ebx");
                        bloqueCodigo.Add("\tdiv ebx");
                        //asm.WriteLine("\tpush edx");
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
                //asm.WriteLine("\tmov eax, " + Contenido);
                bloqueCodigo.Add("\tmov eax, " + Contenido);
                //asm.WriteLine("\tpush eax");
                bloqueCodigo.Add("\tpush eax");
                match(Tipos.Numero);
            }
            else if (Clasificacion == Tipos.Identificador)
            {
                var v = listaVariables.Find(delegate (Variable x) { return x.getNombre() == Contenido; });
                //asm.WriteLine("\tmov eax, " + Contenido);
                bloqueCodigo.Add("\tmov eax, " + Contenido);
                //asm.WriteLine("\tpush eax");
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