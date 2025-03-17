using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Dvigalo
{
    public int Id { get; }
    public int TrenutnoNadstropje { get; private set; }
    public bool PremikaSeGor { get; private set; }
    public Queue<int> Zahteve { get; }
    public int Kapaciteta { get; }
    public int TrenutnaObremenitev { get; private set; }

    public Dvigalo(int id)
    {
        Id = id;
        TrenutnoNadstropje = 0;
        Zahteve = new Queue<int>();
        Kapaciteta = 10;
        TrenutnaObremenitev = 0;
    }

    public void ZahtevajNadstropje(int nadstropje)
    {
        if (!Zahteve.Contains(nadstropje))
        {
            Zahteve.Enqueue(nadstropje);
        }
    }

    public void Premik()
    {
        if (Zahteve.Count > 0)
        {
            int ciljnoNadstropje = Zahteve.Peek();
            if (TrenutnoNadstropje < ciljnoNadstropje)
            {
                TrenutnoNadstropje++;
                PremikaSeGor = true;
            }
            else if (TrenutnoNadstropje > ciljnoNadstropje)
            {
                TrenutnoNadstropje--;
                PremikaSeGor = false;
            }
            else
            {
                Zahteve.Dequeue();
                TrenutnaObremenitev = Math.Max(0, TrenutnaObremenitev - new Random().Next(1, 4)); // Izstopijo potniki
            }
        }
    }

    public bool LahkoSprejmePotnike(int stevilo)
    {
        return (TrenutnaObremenitev + stevilo) <= Kapaciteta;
    }

    public void DodajPotnike(int stevilo)
    {
        TrenutnaObremenitev = Math.Min(Kapaciteta, TrenutnaObremenitev + stevilo);
    }
}


