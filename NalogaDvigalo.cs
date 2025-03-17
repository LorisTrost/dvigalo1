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
public class SistemDvigal
{
    private List<Dvigalo> Dvigala;
    private int SkupnoNadstropij;

    public SistemDvigal(int steviloDvigal, int skupnoNadstropij)
    {
        Dvigala = new List<Dvigalo>();
        SkupnoNadstropij = skupnoNadstropij;
        for (int i = 0; i < steviloDvigal; i++)
        {
            Dvigala.Add(new Dvigalo(i));
        }
    }

    public void ZahtevajDvigalo(int nadstropje, int steviloPotnikov)
    {
        var naVoljo = Dvigala.Where(d => d.LahkoSprejmePotnike(steviloPotnikov)).ToList();
        if (naVoljo.Count == 0)
        {
            Console.WriteLine("Vsa dvigala so polna!");
            return;
        }

        Dvigalo najblizjeDvigalo;

        if (nadstropje == 0)
        {
            najblizjeDvigalo = naVoljo.OrderBy(d => d.Zahteve.Count).First();
        }
        else
        {
            najblizjeDvigalo = naVoljo
                .OrderBy(d => Math.Abs(d.TrenutnoNadstropje - nadstropje))
                .ThenBy(d => d.Zahteve.Count)
                .First();
        }

        najblizjeDvigalo.ZahtevajNadstropje(nadstropje);
        najblizjeDvigalo.DodajPotnike(steviloPotnikov);
    }

    public void KorakSimulacije()
    {
        foreach (var dvigalo in Dvigala)
        {
            dvigalo.Premik();
        }
    }

    public void IzpisiStatus()
    {
        foreach (var dvigalo in Dvigala)
        {
            Console.WriteLine($"Dvigalo ID: {dvigalo.Id}, Nadstropje: {dvigalo.TrenutnoNadstropje}, " +
                              $"Premika se gor: {dvigalo.PremikaSeGor}, Potniki: {dvigalo.TrenutnaObremenitev}/{dvigalo.Kapaciteta}");
        }
    }
}
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
                TrenutnaObremenitev = Math.Max(0, TrenutnaObremenitev - new Random().Next(1, 4));
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
