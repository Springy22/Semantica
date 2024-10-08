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
                throw new Error("Sintaxis: se espera un " + espera + " (" + Contenido + ")" + "en la linea: " + linea, log);
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
                throw new Error("Sintaxis: se espera un " + espera + " (" + Contenido + ")" + "en la linea: " + linea, log);
            }
        }

        /*
        private int contadorLinea(){
            int laine = new Lexico().linea12;
            laine=laine-10;
            int diferencia;
            if((laine-13)!=0){
                diferencia=(laine-13)/2;
                laine=laine+(-diferencia);
            }
            return laine;
        }
        */
    }
}