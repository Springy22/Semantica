using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Semantica;

/*
    1. Usar metodo find en lugar del for each
    2. Validar que no existam variables duplicadas --> Listo, falta comprobar
    3. Validar que existan las variables en las expresiones matematicas
       Asignación
    4. Asignar una expresion matematica a la variable al momento de declararla
    verificando la semantica
    5. Validar que en el ReadLine se capturen solo números e implementar una excepción
    6. ** listaConcatenacion: 30, 40, 50, 12, 0

    Console.WriteLine("Hola = "+a+b+c);

    7. Quiter comillas y considerar el write
    8. Emular el for   --- 15 pts
    9. Emular el while --- 15 pts

    En el for validar si las variables existen, y en listaIdentificadores validar que no 
    existan
    
*/

namespace Semantica
{
    public class Lenguaje : Sintaxis
    {
        List<Variable> listaVariables;
        private Stack<float> S;
        private float nuevoValor;
        private string operadorCondicion;
        private Variable.TipoDato tipoDatoExpresion;
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
            imprimeVariables();
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
            for (int i = 0; i < listaVariables.Count; i++)
            {
                Variable v = listaVariables[i];
                log.WriteLine(v.getNombre() + " " + v.getTipo() + " " + v.getValor());
            }
        }

        private Variable BuscarVariable(string nombreVariable)
        {
            var v = listaVariables.Find(v => v.getNombre() == nombreVariable);
            if (v == null)
            {
                throw new Error("Semantico, la variable " + nombreVariable + " no existe", log, linea);
            }
            return v;
        }

        //ListaIdentificadores -> identificador (,ListaIdentificadores)?
        private void ListaIdentificadores(Variable.TipoDato t)
        {
            var variableExistente = listaVariables.Find(v => v.getNombre() == Contenido);
            if (variableExistente != null)
            {
                throw new Error("Semantico, la variable " + Contenido + " ya existe", log, linea);
            }
            else
            {
                listaVariables.Add(new Variable(Contenido, t));
            }
            match(Tipos.Identificador);
            if (Contenido == "=")
            {
                match("=");
                Expresion(); // Procesar la expresión de asignación
            }
            if (Contenido == ",")
            {
                match(",");
                ListaIdentificadores(t);
            }
        }

        //BloqueInstrucciones -> { listaIntrucciones? }
        private void BloqueInstrucciones(bool ejecutar)
        {
            match("{");
            if (Contenido != "}")
            {
                ListaInstrucciones(ejecutar);
            }
            match("}");
        }

        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones(bool ejecutar)
        {
            Instruccion(ejecutar);
            if (Contenido != "}")
            {
                ListaInstrucciones(ejecutar);
            }
        }

        //Instruccion -> Console | If | While | do | For | Variables | Asignacion
        private void Instruccion(bool ejecutar)
        {
            if (Contenido == "Console")
            {
                console(ejecutar);
            }
            else if (Contenido == "if")
            {
                If(ejecutar);
            }
            else if (Contenido == "while")
            {
                While(ejecutar);
            }
            else if (Contenido == "do")
            {
                Do(ejecutar);
            }
            else if (Contenido == "for")
            {
                For(ejecutar);
            }
            if (Clasificacion == Tipos.TipoDato)
            {
                Variables();
            }
            else
            {
                Asignacion(ejecutar);
                match(";");
            }
        }

        //Asignacion -> Identificador = Expresion;
        private void Asignacion(bool ejecutar)
        {
            string variable = Contenido;
            match(Tipos.Identificador);

            var v = BuscarVariable(variable);

            float nuevoValor = v.getValor();

            tipoDatoExpresion = Variable.TipoDato.Char;

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
                        if (ejecutar)
                        {
                            float valor = Console.Read();
                        }
                        // 8
                    }
                    else
                    {
                        match("ReadLine");
                        try
                        {
                            nuevoValor = float.Parse(Console.ReadLine());  // Intentamos convertir el valor a float
                        }
                        catch (FormatException)
                        {
                            throw new Error("Error: Solo se permiten números", log, linea);  // Lanza una excepción si el valor no es numérico
                        }
                    }
                    match("(");
                    match(")");
                }
                else
                {
                    Expresion();
                    nuevoValor = S.Pop();
                }
            }
            else if (Contenido == "++")
            {
                operadorCondicion = Contenido;
                match("++");
                nuevoValor++;
            }
            else if (Contenido == "--")
            {
                operadorCondicion = Contenido;
                match("--");
                nuevoValor--;
            }
            else if (Contenido == "+=")
            {
                match("+=");
                Expresion();
                nuevoValor += S.Pop();
            }
            else if (Contenido == "-=")
            {
                match("-=");
                Expresion();
                nuevoValor -= S.Pop();
            }
            else if (Contenido == "*=")
            {
                match("*=");
                Expresion();
                nuevoValor *= S.Pop();
            }
            else if (Contenido == "/=")
            {
                match("/=");
                Expresion();
                nuevoValor /= S.Pop();
            }
            else
            {
                match("%=");
                Expresion();
                nuevoValor %= S.Pop();
            }
            // match(";");
            if (analisisSemantico(v, nuevoValor))
            {
                if (ejecutar)
                    v.setValor(nuevoValor);
            }
            else
            {
                // tipoDatoExpresion = 
                throw new Error("Semantico, no puedo asignar un " + tipoDatoExpresion +
                                " a un " + v.getTipo(), log, linea);
            }
            log.WriteLine(variable + " = " + nuevoValor);
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
            if (tipoDatoExpresion > v.getTipo())
            {
                return false;
            }
            else if (valor % 1 == 0)
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
        private void If(bool ejecutar)
        {
            match("if");
            match("(");
            bool resultado = Condicion();
            match(")");
            if (Contenido == "{")
            {
                BloqueInstrucciones(resultado && ejecutar);
            }
            else
            {
                Instruccion(resultado && ejecutar);
            }
            if (Contenido == "else")
            {
                match("else");
                if (Contenido == "{")
                {
                    BloqueInstrucciones(!resultado && ejecutar);
                }
                else
                {
                    Instruccion(!resultado && ejecutar);
                }
            }
        }

        //Condicion -> Expresion operadorRelacional Expresion
        private bool Condicion()
        {
            Expresion(); //E1
            string operador = Contenido;
            operadorCondicion = operador;
            match(Tipos.OpRelacional);
            Expresion(); //E2
            float R2 = S.Pop();
            float R1 = S.Pop();
            switch (operador)
            {
                case ">": return R1 > R2;
                case ">=": return R1 >= R2;
                case "<": return R1 < R2;
                case "<=": return R1 <= R2;
                case "==": return R1 == R2;
                default: return R1 != R2;
            }
        }

        //While -> while(Condicion) bloqueInstrucciones | instruccion
        private void While(bool ejecutar)
        {
            int cTemp = caracter - 6;
            int lTemp = linea;
            bool resultado = false;
            match("while");
            match("(");
            resultado = Condicion() && ejecutar;
            match(")");
            while (resultado)
            {
                if (Contenido == "{")
                {
                    BloqueInstrucciones(ejecutar);
                }
                else
                {
                    Instruccion(ejecutar);
                }

                if (resultado)
                {
                    caracter = cTemp;
                    linea = lTemp;
                    archivo.DiscardBufferedData();
                    archivo.BaseStream.Seek(cTemp, System.IO.SeekOrigin.Begin);
                    nextToken();

                    match("while");
                    match("(");
                    resultado = Condicion() && ejecutar;
                    match(")");
                }
            }
        }

        //Do -> do 
        //bloqueInstrucciones | intruccion 
        //while(Condicion);
        private void Do(bool ejecutar)
        {
            int cTemp = caracter - 3;
            int ltemp = linea;
            bool resultado = false;
            do
            {
                match("do");
                if (Contenido == "{")
                {
                    BloqueInstrucciones(ejecutar);
                }
                else
                {
                    Instruccion(ejecutar);
                }
                match("while");
                match("(");
                resultado = Condicion() && ejecutar;
                match(")");
                match(";");

                if (resultado)
                {
                    caracter = cTemp;
                    linea = ltemp;
                    archivo.DiscardBufferedData();
                    archivo.BaseStream.Seek(cTemp, System.IO.SeekOrigin.Begin);
                    nextToken();
                }
            } while (resultado);
        }

        //For -> for(Asignacion Condicion; Incremento) 
        //BloqueInstrucciones | Intruccion 
        // For -> for (Asignacion; Condicion; Asignacion) BloqueInstrucciones | Instruccion
        private void For(bool ejecutar)
        {
            // Guardar la posición actual del archivo para poder regresar en cada iteración
            int posCaracterInicio = caracter;
            int posLineaInicio = linea;

            match("for");
            match("(");

            // Procesar la inicialización
            Asignacion(ejecutar);
            match(";");

            // Evaluar la condición
            bool condicion = Condicion() && ejecutar;
            match(";");      // Reconocer ';'

            // Procesar el incremento
            string operadorIncremento = operadorCondicion;
            Asignacion(ejecutar);

            match(")");

            // Determinar si es un bloque de instrucciones o una única instrucción
            if (Contenido == "{")
            {
                BloqueInstrucciones(condicion);
            }
            else
            {
                Instruccion(condicion);
            }

            // Mientras la condición sea verdadera y se deba ejecutar, repetir el ciclo
            while (condicion)
            {
                // Procesar el incremento
                operadorIncremento = operadorCondicion;
                Asignacion(ejecutar);

                // Reiniciar la posición del archivo para reevaluar la condición
                caracter = posCaracterInicio;
                linea = posLineaInicio;
                archivo.DiscardBufferedData();
                archivo.BaseStream.Seek(posCaracterInicio, System.IO.SeekOrigin.Begin);
                nextToken(); // Obtener el siguiente token desde la posición reiniciada

                match("for");
                match("(");
                Asignacion(ejecutar); // Inicialización (puede omitirse si no es necesaria)
                match(";");
                condicion = Condicion() && ejecutar;
                match(";");

                operadorIncremento = operadorCondicion;
                Asignacion(ejecutar);
                match(")");

                if (Contenido == "{")
                {
                    BloqueInstrucciones(condicion);
                }
                else
                {
                    Instruccion(condicion);
                }
            }
        }

        //Incremento -> Identificador ++ | --
        //Console -> Console.(WriteLine|Write) (cadena?); |
        private void console(bool ejecutar)
        {
            match("Console");
            match(".");
            bool esWriteLine = false;

            if (Contenido == "WriteLine")
            {
                match("WriteLine");
                esWriteLine = true;
            }
            else
            {
                match("Write");
            }
            match("(");
            if (Clasificacion == Tipos.Cadena)
            {
                string contenido = Contenido;
                contenido = contenido.Trim('"');
                if (ejecutar)
                {
                    if (esWriteLine)
                    {
                        Console.WriteLine(contenido);
                    }
                    else
                    {
                        Console.Write(contenido);
                    }
                }
                match(Tipos.Cadena);
                if (Contenido == "+")
                {
                    listaConcatenacion(ejecutar, esWriteLine);
                }
            }
            match(")");
            match(";");
        }

        /*
        string listaConcatenacion()
        {
            // Inicializa una lista para construir la concatenación
            List<string> elementos = new List<string>();

            // Elimina las comillas y agrega el valor inicial a la lista
            elementos.Add(nuevoValor.Trim('"'));
            match("+");

            // Procesa el siguiente valor dependiendo de su tipo
            switch (Clasificacion)
            {
                case Tipos.Identificador:
                    match(Tipos.Identificador);
                    elementos.Add(nuevoValor);  // Añade el identificador directamente a la lista
                    break;
                default:
                    string content = Contenido.Replace("\"", "");  // Elimina comillas del contenido
                    match(Contenido);
                    elementos.Add(content);  // Añade el contenido a la lista
                    break;
            }

            // Verifica si hay más concatenaciones
            while (Contenido == "+")
            {
                elementos.Add(listaConcatenacion());  // Recursión para concatenar más elementos
            }

            // Devuelve la concatenación de todos los elementos de la lista
            return string.Join("", elementos);
        }*/


        string listaConcatenacion(bool ejecutar, bool esWriteLine)
        {
            // Verificación de que el próximo elemento sea un "+"
            match("+");

            if (Clasificacion == Tipos.Cadena)
            {
                // Eliminar las comillas de la cadena
                string contenido = Contenido.Trim('"');

                // Si se debe ejecutar, se imprime el contenido
                if (ejecutar)
                {
                    if (esWriteLine)
                    {
                        Console.WriteLine(contenido);
                    }
                    else
                    {
                        Console.Write(contenido);
                    }
                }
                match(Tipos.Cadena); // Avanzar en la cadena
            }
            else if (Clasificacion == Tipos.Identificador)
            {
                // Buscar el valor de la variable identificada
                var v = BuscarVariable(Contenido);
                tipoDatoExpresion = v.getTipo();
                float valor = v.getValor();

                // Si se debe ejecutar, se imprime el valor de la variable
                if (ejecutar)
                {
                    if (esWriteLine)
                    {
                        Console.WriteLine(valor);
                    }
                    else
                    {
                        Console.Write(valor);
                    }
                }
                match(Tipos.Identificador); // Avanzar en el identificador
            }

            // Si el próximo elemento es un "+", llamamos recursivamente
            if (Contenido == "+")
            {
                listaConcatenacion(ejecutar, esWriteLine);
            }

            return "";
        }
        /*
        string listaConcatenacion(bool ejecutar, bool esWriteLine)
        {
            string resultado = "";  // Acumulador para la concatenación

            match("+");

            // Procesa cadenas
            if (Clasificacion == Tipos.Cadena)
            {
                string contenido = Contenido.Trim('"');

                if (ejecutar)
                {
                    resultado += contenido;
                }
                match(Tipos.Cadena);
            }
            // Procesa identificadores
            else if (Clasificacion == Tipos.Identificador)
            {
                var v = BuscarVariable(Contenido);
                S.Push(v.getValor());
                tipoDatoExpresion = v.getTipo();

                float valor = v.getValor();

                if (ejecutar)
                {
                    resultado += valor.ToString();  // Convierte el valor a string y lo agrega al resultado
                }
                match(Tipos.Identificador);
            }

            // Si hay más concatenaciones
            if (Contenido == "+")
            {
                //match("+");
                resultado += listaConcatenacion(ejecutar, esWriteLine);  // Recursión
            }

            // Si es la última concatenación, imprime el resultado
            if (ejecutar && esWriteLine)
            {
                Console.WriteLine(resultado);  // Imprime todo el resultado acumulado
            }
            else if (ejecutar)
            {
                Console.Write(resultado);  // Imprime sin salto de línea
            }
            return resultado;  // Devuelve el resultado acumulado
        }*/

        /*string listaConcatenacion(bool ejecutar, bool esWriteLine)
        {
            match("+");
            if (Clasificacion == Tipos.Cadena)
            {
                string contenido = Contenido.Trim('"');

                if (ejecutar)
                {
                    if (esWriteLine)
                    {
                        Console.WriteLine(contenido);
                    }
                    else
                    {
                        Console.Write(contenido);
                    }
                }
                match(Tipos.Cadena);
            }
            else if (Clasificacion == Tipos.Identificador)
            {
                var v = BuscarVariable(Contenido);
                S.Push(v.getValor());
                tipoDatoExpresion = v.getTipo();

                float valor = v.getValor();

                if (ejecutar)
                {
                    if (esWriteLine)
                    {
                        Console.WriteLine(valor);
                    }
                    else
                    {
                        Console.Write(valor);
                    }
                }
                match(Tipos.Identificador);
            }

            if (Contenido == "+")
            {
                listaConcatenacion(ejecutar, esWriteLine);
            }
            return "";

        }*/

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
            BloqueInstrucciones(true);
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
                if (tipoDatoExpresion < valorToTipo(float.Parse(Contenido)))
                {
                    tipoDatoExpresion = valorToTipo(float.Parse(Contenido));
                }
                match(Tipos.Numero);
            }
            else if (Clasificacion == Tipos.Identificador)
            {
                var v = listaVariables.Find(delegate (Variable x) { return x.getNombre() == Contenido; });
                S.Push(v.getValor());
                if (tipoDatoExpresion < v.getTipo())
                {
                    tipoDatoExpresion = v.getTipo();
                }
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
                    tipoDatoExpresion = aCastear;
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
        /*private void Validacion(Token variable)
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
        */
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