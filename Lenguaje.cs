using System;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Semantica;

/*
    1. Colocar el numero de linea en errores lexicos y sintaxis
    2. Cambiar la clase token para atributos publicos (get,set)
    3. Cambiar los constructores de la clase lexico usando parámetros por default
*/

namespace Semantica
{
    public class Lenguaje : Sintaxis
    {
        List<Variable> listaVariables;
        private Stack<float> S;
        public Lenguaje()
        {
            listaVariables = new List<Variable>();
            S = new Stack<float>();
        }

        public Lenguaje(String nombre) : base(nombre)
        {
            listaVariables = new List<Variable>();
            S = new Stack<float>();
        }

        //Programa  -> Librerias? Variables? Main
        public void Programa()
        {
            if (getContenido() == "using")
            {
                Librerias();
            }
            Main();
            //imprimeVariables();
            log.WriteLine(linea12);
        }

        //Librerias -> using ListaLibrerias; Librerias?
        private void Librerias()
        {
            match("using");
            ListaLibrerias();
            match(";");
            if (getContenido() == "using")
            {
                Librerias();
            }
        }

        //ListaLibrerias -> identificador (.ListaLibrerias)?
        private void ListaLibrerias()
        {
            match(Tipos.Identificador);
            if (getContenido() == ".")
            {
                match(".");
                ListaLibrerias();
            }
        }

        Variable.TipoDato getTipo(string TipoDato)
        {
            Variable.TipoDato tipo = Variable.TipoDato.Char;
            switch (getContenido())
            {
                case "int": tipo = Variable.TipoDato.Int; break;
                case "float": tipo = Variable.TipoDato.Float; break;
            }
            return tipo;
        }
        //Variables -> tipo_dato Lista_identificadores; Variables?
        private void Variables()
        {
            Variable.TipoDato tipo = getTipo(getContenido());
            match(Tipos.TipoDato);
            ListaIdentificadores(tipo);
            match(";");
        }

        private void imprimeVariables()
        {
            foreach (Variable v in listaVariables)
            {
                log.WriteLine(v.getNombre() + " (" + v.getTipo() + ") = " + v.getValor());
            }
        }

        //ListaIdentificadores -> identificador (,ListaIdentificadores)?
        private void ListaIdentificadores(Variable.TipoDato t)
        {
            listaVariables.Add(new Variable(getContenido(), t));
            match(Tipos.Identificador);
            if (getContenido() == ",")
            {
                match(",");
                ListaIdentificadores(t);
            }
        }

        //BloqueInstrucciones -> { listaIntrucciones? }
        private void BloqueInstrucciones()
        {
            match("{");
            if (getContenido() != "}")
            {
                ListaInstrucciones();
            }
            match("}");
        }

        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones()
        {
            Instruccion();
            if (getContenido() != "}")
            {
                ListaInstrucciones();
            }
        }

        //Instruccion -> Console | If | While | do | For | Variables | Asignacion
        private void Instruccion()
        {
            if (getContenido() == "Console")
            {
                Console();
            }
            else if (getContenido() == "if")
            {
                If();
            }
            else if (getContenido() == "while")
            {
                While();
            }
            else if (getContenido() == "do")
            {
                Do();
            }
            else if (getContenido() == "for")
            {
                For();
            }
            if (getClasificacion() == Tipos.TipoDato)
            {
                Variables();
            }
            else
            {
                Asignacion();
            }
        }

        //Asignacion -> Identificador = Expresion;
        private void Asignacion()
        {
            match(Tipos.Identificador);
            switch (getContenido())
            {
                case "=":
                    {
                        match("=");
                        Expresion();
                        match(Tipos.FinSentencia);
                        //imprimeStack();
                        //log.WriteLine(variable + " = " + S.Pop());
                    }
                    break;
                case "++":
                    {
                        match("++");
                        match(Tipos.FinSentencia);
                        //log.WriteLine(variable + "++");
                    }
                    break;
                case "--":
                    {
                        match("--");
                        match(Tipos.FinSentencia);
                        //log.WriteLine(variable + "--");
                    }
                    break;
                case "+=":
                    {
                        match("+=");
                        Expresion();
                        match(Tipos.FinSentencia);
                        //imprimeStack();
                        //log.WriteLine(variable + " = " + variable + " + " + S.Pop());
                    }
                    break;
                case "-=":
                    {
                        match("-=");
                        Expresion();
                        match(Tipos.FinSentencia);
                        //imprimeStack();
                        //log.WriteLine(variable + " = " + variable + " - " + S.Pop());
                    }
                    break;
                case "*=":
                    {
                        match("*=");
                        Expresion();
                        match(Tipos.FinSentencia);
                        //imprimeStack();
                        //log.WriteLine(variable + " = " + variable + " * " + S.Pop());
                    }
                    break;
                case "/=":
                    {
                        match("/=");
                        Expresion();
                        match(Tipos.FinSentencia);
                        //imprimeStack();
                        //log.WriteLine(variable + " = " + variable + " / " + S.Pop());
                    }
                    break;
                case "%=":
                    {
                        match("%=");
                        Expresion();
                        match(Tipos.FinSentencia);
                        //imprimeStack();
                        //log.WriteLine(variable + " = " + variable + " % " + S.Pop());
                    }
                    break;
            }
        }

        //Hacer un constructor general 


        //If -> if (Condicion) bloqueInstrucciones | instruccion
        //(else bloqueInstrucciones | instruccion)?
        private void If()
        {
            match("if");
            match("(");
            Condicion();
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }
            if (getContenido() == "else")
            {
                match("else");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones();
                }
                else
                {
                    Instruccion();
                }
            }
        }

        //Condicion -> Expresion operadorRelacional Expresion
        private void Condicion()
        {
            Expresion();
            match(Tipos.OpRelacional);
            Expresion();
        }

        //While -> while(Condicion) bloqueInstrucciones | instruccion
        private void While()
        {
            match("while");
            match("(");
            Condicion();
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }
        }

        //Do -> do 
        //bloqueInstrucciones | intruccion 
        //while(Condicion);
        private void Do()
        {
            match("do");
            if (getContenido() == "{")
            {
                BloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }
            match("while");
            match("(");
            Condicion();
            match(")");
            match(Tipos.FinSentencia);

        }

        //For -> for(Asignacion Condicion; Incremento) 
        //BloqueInstrucciones | Intruccion 
        private void For()
        {
            match("for");
            match("(");
            Asignacion();
            Condicion();
            match(";");
            Incremento();
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }
        }

        //Incremento -> Identificador ++ | --
        private void Incremento()
        {
            match(Tipos.Identificador);
            if (getContenido() == "++")
            {
                match("++");
            }
            else
            {
                match("--");
            }
        }

        //Console -> Console.(WriteLine|Write) (cadena?); |
        //Console.(Read | ReadLine) ();
        private void Console()
        {
            match("Console");
            match(".");
            if (getContenido() == "WriteLine" || getContenido() == "Write")
            {
                match(getContenido());
                match("(");
                if (getClasificacion() == Tipos.Cadena)
                {
                    match(Tipos.Cadena);
                }
                match(")");
            }
            else
            {
                if (getContenido() == "ReadLine")
                {
                    match("ReadLine");
                }
                else
                {
                    match("Read");
                }
                match("(");
                match(")");
            }
            match(";");

        }

        //Main      -> static void Main(string[] args) BloqueInstrucciones 
        private void Main()
        {
            match("static");
            match("void");
            match("Main");
            match("(");
            match("string");
            match("[");
            match("]");
            match("args");
            match(")");
            BloqueInstrucciones();
        }

        //Expresion -> Termino MasTermino
        private void Expresion()
        {
            Termino();
            MasTermino();
        }

        //MasTermino -> (OperadorTermino Termino)?
        private void MasTermino()
        {
            if (getClasificacion() == Tipos.OpTermino)
            {
                string operador = getContenido();
                match(Tipos.OpTermino);
                Termino();
                float R2 = S.Pop();
                float R1 = S.Pop();
                switch (operador)
                {
                    case "+": S.Push(R1 + R2); break;
                    case "-": S.Push(R1 - R2); break;
                }
            }
        }

        //Termino -> Factor PorFactor
        private void Termino()
        {
            Factor();
            PorFactor();
        }

        //PorFactor -> (OperadorFactor Factor)?
        private void PorFactor()
        {
            if (getClasificacion() == Tipos.OpFactor)
            {
                string operador = getContenido();
                match(Tipos.OpFactor);
                Factor();
                float R2 = S.Pop();
                float R1 = S.Pop();
                switch (operador)
                {
                    case "*": S.Push(R1 * R2); break;
                    case "/": S.Push(R1 / R2); break;
                    case "%": S.Push(R1 % R2); break;
                }
            }
        }

        private void imprimeStack()
        {
            log.WriteLine("Stack:");
            foreach (float e in S.Reverse())
            {
                log.Write(e + " ");
            }
            log.WriteLine();
        }

        //Factor -> numero | identificador | (Expresion)
        private void Factor()
        {
            if (getClasificacion() == Tipos.Numero)
            {
                S.Push(float.Parse(getContenido()));
                match(Tipos.Numero);
            }
            else if (getClasificacion() == Tipos.Identificador)
            {
                match(Tipos.Identificador);
            }
            else
            {
                match("(");
                if (getClasificacion() == Tipos.TipoDato)
                {
                    match(Tipos.TipoDato);
                    match(")");
                    match("(");
                }
                Expresion();
                match(")");
            }
        }

        private void Validacion()
        {
            Variable.TipoDato tipo = getTipo(getContenido());
            switch (tipo)
            {
                case Variable.TipoDato.Char:
                    {

                    }
                    break;

                case Variable.TipoDato.Int:
                    {

                    }
                    break;

                case Variable.TipoDato.Float:
                    {

                    }
                    break;
            }
        }
    }

}