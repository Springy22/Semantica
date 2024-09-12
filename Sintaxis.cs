using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Semantica
{
    public class Sintaxis : Lexico
    {
        public Sintaxis()
        {
            nextToken();
        }
        public Sintaxis(string nombre) : base(nombre)
        {
            nextToken();
        }
        public void match(string espera)
        {
            if (Contenido == espera)
            {
                nextToken();
            }
            else
            {
                linea12 = linea12 - 10;
                int diferencia;
                if ((linea12 - 13) != 0)
                {
                    diferencia = (linea12 - 13) / 2;
                    linea12 = linea12 + (-diferencia);
                }
                log.WriteLine(linea12);
                throw new Error("Sintaxis: se espera un " + espera + " (" + Contenido + ")" + " en la linea: " + linea12, log);
            }
        }
        public void match(Tipos espera)
        {
            if (Clasificacion == espera)
            {
                nextToken();
            }
            else
            {
                linea12 = linea12 - 10;
                int diferencia;
                if ((linea12 - 13) != 0)
                {
                    diferencia = (linea12 - 13) / 2;
                    linea12 = linea12 + (-diferencia);
                }
                log.WriteLine(linea12);
                throw new Error("Sintaxis: se espera un " + espera + " (" + Contenido + ")" + " en la linea: " + linea12, log);
            }
        }
    }
}