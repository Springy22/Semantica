using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Semantica
{
    public class Token
    {
        public enum Tipos
        {
            Identificador, Numero, FinSentencia, OpTermino, OpFactor,
            OpLogico, OpRelacional, OpTernario, Asignacion, IncTermino,
            IncFactor, Cadena, Inicio, Fin, Caracter, TipoDato, Ciclo,
            Condicion, Validacion, DecTermino
        };

        private string contenido;
        private Tipos clasificacion;

        public Token()
        {
            contenido = "";
        }

        public string Contenido
        {
            get => contenido;
            set => contenido = value;
        }

        public Tipos Clasificacion
        {
            get => clasificacion;
            set => clasificacion = value;
        }
    }

    public class ExtendedToken : Token
    {
        private string contenido;

        // Ocultando la propiedad Contenido de la clase base con 'new'.
        public new string Contenido
        {
            get => contenido;
            set => contenido = value + " (Extended)";
        }
    }

    class TestHiding
    {
        public static void Test()
        {
            ExtendedToken et = new ExtendedToken();

            // Propiedad de la clase derivada.
            et.Contenido = "Valor Derivado";

            // Propiedad de la clase base.
            ((Token)et).Contenido = "Valor Base";

            Console.WriteLine("Contenido en la clase derivada: {0}", et.Contenido);
            Console.WriteLine("Contenido en la clase base: {0}", ((Token)et).Contenido);
        }
    }
}

