using System;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Semantica;

/*
    1. Usar metodo find en lugar del for each
    2. Validar que no existam variables duplicadas
    3. Validar que existan las variables en las expresiones matematicas
       Asignación
    4. char + char = char
       int + int = int
    5. Meter el valor de la variable al stack
    6. Asignar una expresión matematica a la variable al momento de declararla
    7. Emular el if
    8. Validar que en el ReadLine se capturen solo números e implementar una excepción
    9. Emular el do
    10. Emular el for
    11. Emular el while
*/

namespace Semantica
{
    public class Lenguaje : Sintaxis
    {
        List<Variable> listaVariables;
        private Stack<float> S;
        public Lenguaje()
        {
            log.WriteLine("Analizador Sintactico");
            listaVariables = new List<Variable>();
            S = new Stack<float>();
        }

        public Lenguaje(String nombre) : base(nombre)
        {
            log.WriteLine("Analizador Sintactico");
            listaVariables = new List<Variable>();
            S = new Stack<float>();
        }

        //Programa  -> Librerias? Variables? Main
        public void Programa()
        {
            if (Contenido == "using")
            {
                Librerias();
            }
            Main();
            //imprimeVariables();            
        }

        //Librerias -> using ListaLibrerias; Librerias?
        private void Librerias()
        {
            match("using");
            ListaLibrerias();
            match(";");
            if (Contenido == "using")
            {
                Librerias();
            }
        }

        //ListaLibrerias -> identificador (.ListaLibrerias)?
        private void ListaLibrerias()
        {
            match(Tipos.Identificador);
            if (Contenido == ".")
            {
                match(".");
                ListaLibrerias();
            }
        }

        Variable.TipoDato getTipo(string TipoDato)
        {
            Variable.TipoDato tipo = Variable.TipoDato.Char;
            switch (Contenido)
            {
                case "int": tipo = Variable.TipoDato.Int; break;
                case "float": tipo = Variable.TipoDato.Float; break;
            }
            return tipo;
        }
        //Variables -> tipo_dato Lista_identificadores; Variables?
        private void Variables()
        {
            Variable.TipoDato tipo = getTipo(Contenido);
            match(Tipos.TipoDato);
            ListaIdentificadores(tipo);
            match(";");
        }

        private void imprimeVariables()
        {
            log.WriteLine("Lista de variables");
            foreach (Variable v in listaVariables)
            {
                log.WriteLine(v.getNombre() + " (" + v.getTipo() + ") = " + v.getValor());
            }
        }

        //ListaIdentificadores -> identificador (,ListaIdentificadores)?
        private void ListaIdentificadores(Variable.TipoDato t)
        {
            listaVariables.Add(new Variable(Contenido, t));
            match(Tipos.Identificador);
            if (Contenido == ",")
            {
                match(",");
                ListaIdentificadores(t);
            }
        }

        //BloqueInstrucciones -> { listaIntrucciones? }
        private void BloqueInstrucciones()
        {
            match("{");
            if (Contenido != "}")
            {
                ListaInstrucciones();
            }
            match("}");
        }

        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones()
        {
            Instruccion();
            if (Contenido != "}")
            {
                ListaInstrucciones();
            }
        }

        //Instruccion -> Console | If | While | do | For | Variables | Asignacion
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
            if (Clasificacion == Tipos.TipoDato)
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
            string variable = Contenido;
            match(Tipos.Identificador);
            var v = listaVariables.Find(delegate (Variable x) { return x.getNombre() == variable; });
            float nuevoValor = v.getValor();
            switch (Contenido)
            {
                case "=":
                    {
                        Token var = new Token();
                        var.Contenido = Contenido;
                        match("=");
                        if (Contenido == "Console")
                        {
                            match("Console");
                            match(".");
                            if (Contenido == "Read")
                            {
                                match("Read");
                                float valor = Console.Read();
                            }
                            else
                            {
                                match("ReadLine");
                                float valor = float.Parse("" + Console.ReadLine());
                                // 8
                            }
                        }
                        else
                        {
                            Expresion();
                            Validacion(var);
                            match(Tipos.FinSentencia);
                            nuevoValor = S.Pop();
                            //imprimeStack();
                            //log.WriteLine(variable + " = " + S.Pop());
                        }

                    }
                    break;
                case "++":
                    {
                        match("++");
                        match(Tipos.FinSentencia);
                        nuevoValor++;

                        //log.WriteLine(variable + "++");
                    }
                    break;
                case "--":
                    {
                        match("--");
                        match(Tipos.FinSentencia);
                        nuevoValor--;
                        //log.WriteLine(variable + "--");
                    }
                    break;
                case "+=":
                    {
                        match("+=");
                        Expresion();
                        match(Tipos.FinSentencia);
                        nuevoValor += S.Pop();
                        //imprimeStack();
                        //log.WriteLine(variable + " = " + variable + " + " + S.Pop());
                    }
                    break;
                case "-=":
                    {
                        match("-=");
                        Expresion();
                        match(Tipos.FinSentencia);
                        nuevoValor -= S.Pop();
                        //imprimeStack();
                        //log.WriteLine(variable + " = " + variable + " - " + S.Pop());
                    }
                    break;
                case "*=":
                    {
                        match("*=");
                        Expresion();
                        match(Tipos.FinSentencia);
                        nuevoValor *= S.Pop();
                        //imprimeStack();
                        //log.WriteLine(variable + " = " + variable + " * " + S.Pop());
                    }
                    break;
                case "/=":
                    {
                        match("/=");
                        Expresion();
                        match(Tipos.FinSentencia);
                        nuevoValor /= S.Pop();
                        //imprimeStack();
                        //log.WriteLine(variable + " = " + variable + " / " + S.Pop());
                    }
                    break;
                case "%=":
                    {
                        match("%=");
                        Expresion();
                        match(Tipos.FinSentencia);
                        nuevoValor %= S.Pop();
                        //imprimeStack();
                        //log.WriteLine(variable + " = " + variable + " % " + S.Pop());
                    }
                    break;
            }
            if (analisisSemantico(v, nuevoValor))
            {
                v.setValor(nuevoValor);
            }
            else
            {
                throw new Error(" Semantico: No puedo asignar un " + valorToTipo(nuevoValor) + " a un " + v.getTipo(), log, linea);
            }
            log.WriteLine(variable + "=" + nuevoValor);
            /*if(nuevoValor > rangoTipoDato()){
                throw new Error("Semantico: ",log,linea12);
            }*/
        }

        private Variable.TipoDato valorToTipo(float valor)
        {
            if (valor % 1 != 0)
            {
                return Variable.TipoDato.Float;
            }
            else if (valor <= 255)
            {
                return Variable.TipoDato.Char;
            }
            else if (valor <= 65535)
            {
                return Variable.TipoDato.Int;
            }
            return Variable.TipoDato.Float;
        }

        bool analisisSemantico(Variable v, float valor)
        {
            if (valor % 1 == 0)
            {
                if (v.getTipo() == Variable.TipoDato.Char)
                {
                    if (valor <= 255)
                    {
                        return true;
                    }
                    return false;
                }
                else if (v.getTipo() == Variable.TipoDato.Int)
                {
                    if (valor <= 65535)
                    {
                        return true;
                    }
                    return false;
                }
            }
            else
            {
                if (v.getTipo() == Variable.TipoDato.Char || v.getTipo() == Variable.TipoDato.Int)
                {
                    return false;
                }
            }
            return true;
        }

        //If -> if (Condicion) bloqueInstrucciones | instruccion
        //(else bloqueInstrucciones | instruccion)?
        private void If()
        {
            match("if");
            match("(");
            Condicion();
            match(")");
            if (Contenido == "{")
            {
                BloqueInstrucciones();
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
            if (Contenido == "{")
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
            if (Contenido == "{")
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
            if (Contenido == "{")
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
            if (Contenido == "++")
            {
                match("++");
            }
            else
            {
                match("--");
            }
        }

        //Console -> Console.(WriteLine|Write) (cadena?); |
        private void console()
        {
            match("Console");
            match(".");
            if (Contenido == "WriteLine")
            {
                match("WriteLine");
                match("(");
                if (Clasificacion == Tipos.Cadena)
                {
                    match(Tipos.Cadena);
                    if(Contenido == "+"){
                        listaConcatenacion();
                    }
                }
                else{

                }
                match(")");
            }
            else{
                match("Write");
                match("(");
                if (Clasificacion == Tipos.Cadena)
                {
                    match(Tipos.Cadena);
                }
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
            if (Clasificacion == Tipos.OpTermino)
            {
                string operador = Contenido;
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
            if (Clasificacion == Tipos.OpFactor)
            {
                string operador = Contenido;
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
            if (Clasificacion == Tipos.Numero)
            {
                S.Push(float.Parse(Contenido));
                match(Tipos.Numero);
            }
            else if (Clasificacion == Tipos.Identificador)
            {
                //5.
                match(Tipos.Identificador);
            }
            else
            {
                bool huboCast = false;
                Variable.TipoDato aCastear = Variable.TipoDato.Char;
                match("(");
                if (Clasificacion == Tipos.TipoDato)
                {
                    huboCast = true;
                    aCastear = getTipo(Contenido);
                    match(Tipos.TipoDato);
                    match(")");
                    match("(");
                    //12
                    //Sacar un elemento del stack
                    //Castearlo
                    //Meterlo casteado
                }
                Expresion();
                match(")");
                if (huboCast && aCastear != Variable.TipoDato.Float)
                {
                    float valor = S.Pop();
                    //Castearlo
                    if (aCastear == Variable.TipoDato.Char)
                    {
                        valor %= 256;
                    }
                    else
                    {
                        valor %= 65536;
                    }
                    S.Push(valor);
                }
            }
        }

        private void Validacion(Token variable)
        {
            string contenido = Contenido;
            switch (variable.Contenido)
            {
                case "char":
                    {
                        if (contenido.Length == 1)
                        {
                            log.WriteLine("Es un char valido " + contenido);
                        }
                        else
                        {
                            log.WriteLine("Error: No es un char valido " + contenido);
                        }
                    }
                    break;

                case "int":
                    {
                        int numero;
                        if (int.TryParse(contenido, out numero))
                        {
                            if (numero >= int.MinValue && numero <= int.MaxValue)
                            {
                                log.WriteLine("Es un int valido y " + contenido + " esta dentro de rango");
                            }
                            else
                            {
                                log.WriteLine("Error: No es un int valido y " + contenido + " esta fuera de rango");
                            }
                        }
                        else
                        {
                            log.WriteLine("Error: No es un int valido");
                        }
                    }
                    break;

                case "float":
                    {
                        float numero;
                        if (float.TryParse(contenido, out numero))
                        {
                            if (numero >= float.MinValue && numero <= float.MaxValue)
                            {
                                log.WriteLine("Es un float valido y " + contenido + " esta dentro de rango");
                            }
                            else
                            {
                                log.WriteLine("Error: No es un float valido y " + contenido + " esta fuera de rango");
                            }
                        }
                        else
                        {
                            log.WriteLine("Error: No es un float valido");
                        }
                    }
                    break;
            }
        }

        /*public int contadorLinea(){
            linea12=linea12-10;
            int diferencia;
            if((linea12-13)!=0){
                diferencia=(linea12-13)/2;
                linea12=linea12+(-diferencia);
            }
            log.WriteLine(linea12);
            return linea12;
        }
        */
    }

}