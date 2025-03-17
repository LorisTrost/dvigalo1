using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



// Razred ElevatorSystem


class Program
{
    static void Main()
    {
        SistemDvigal sistem = new SistemDvigal(3, 10);

        sistem.ZahtevajDvigalo(5, 3);
        sistem.ZahtevajDvigalo(7, 4);
        sistem.ZahtevajDvigalo(2, 2);
        sistem.ZahtevajDvigalo(9, 5);
        sistem.ZahtevajDvigalo(0, 6); 

        for (int i = 0; i < 15; i++)
        {
            sistem.KorakSimulacije();
            sistem.IzpisiStatus();
            Console.WriteLine("--------------------");
        }
    }
}
