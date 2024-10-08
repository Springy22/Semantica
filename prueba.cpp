using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

static void Main(string[] args)
{
    int altura,i,j;
    float y=10, z=2;
    char c;

    // c = (100+200);
    c = (char) (100+200);

    Console.Write("Valor de altura = ");
    altura = Console.ReadLine();
    i=3;
    while(i<altura){
        Console.WriteLine("while");
        i++;
    }
}