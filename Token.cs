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

    class TestHiding
    {
        public static void Test()
        {
            Token t = new Token();

            // Asignando y mostrando la propiedad de la clase base.
            t.Contenido = "Valor Base";

            Console.WriteLine("Contenido en la clase base: {0}", t.Contenido);
        }
    }
}
