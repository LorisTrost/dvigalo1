using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main()
    {
        SistemDvigal sistem = new SistemDvigal(3, 10);

        sistem.ZahtevajDvigalo(5, 8, 3);  
        sistem.ZahtevajDvigalo(7, 2, 4); 
        sistem.ZahtevajDvigalo(2, 9, 2);  
        sistem.ZahtevajDvigalo(9, 0, 5);  
        sistem.ZahtevajDvigalo(0, 4, 6); 

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

    public void ZahtevajDvigalo(int izNadstropja, int vNadstropje, int steviloPotnikov)
    {
        var naVoljo = Dvigala.Where(d => d.LahkoSprejmePotnike(steviloPotnikov)).ToList();
        if (naVoljo.Count == 0)
        {
            Console.WriteLine("Vsa dvigala so polna!");
            return;
        }

        Dvigalo najblizjeDvigalo = naVoljo
            .OrderBy(d => Math.Abs(d.TrenutnoNadstropje - izNadstropja))  
            .ThenBy(d => d.Zahteve.Count)
            .First();

        najblizjeDvigalo.ZahtevajNadstropje(vNadstropje, izNadstropja, steviloPotnikov);
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
            Console.WriteLine($"Dvigalo ID: {dvigalo.Id}, Trenutno nadstropje: {dvigalo.TrenutnoNadstropje}, " +
                              $"Premika se gor: {dvigalo.PremikaSeGor}, Potniki: {dvigalo.TrenutnaObremenitev}/{dvigalo.Kapaciteta}");
        }
    }
}

public class Dvigalo
{
    public int Id { get; }
    public int TrenutnoNadstropje { get; private set; }
    public bool PremikaSeGor { get; private set; }
    public Queue<(int ciljnoNadstropje, int izNadstropja, int steviloPotnikov)> Zahteve { get; }  
    public int Kapaciteta { get; }
    public int TrenutnaObremenitev { get; private set; }

    public Dvigalo(int id)
    {
        Id = id;
        TrenutnoNadstropje = new Random().Next(0, 10);  
        Zahteve = new Queue<(int, int, int)>();
        Kapaciteta = 10;
        TrenutnaObremenitev = 0;
    }

    public void ZahtevajNadstropje(int vNadstropje, int izNadstropja, int steviloPotnikov)
    {
        if (!Zahteve.Contains((vNadstropje, izNadstropja, steviloPotnikov)))
        {
            Zahteve.Enqueue((vNadstropje, izNadstropja, steviloPotnikov)); 
        }
    }

    public void Premik()
    {
        if (Zahteve.Count > 0)
        {
            var (ciljnoNadstropje, izNadstropja, steviloPotnikov) = Zahteve.Peek();

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

            if (TrenutnoNadstropje == izNadstropja)
            {
                TrenutnaObremenitev += steviloPotnikov;
                Console.WriteLine($"Dvigalo {Id} pobira {steviloPotnikov} potnikov iz nadstropja {izNadstropja}.");
            }

            if (TrenutnoNadstropje == ciljnoNadstropje)
            {
                Zahteve.Dequeue();  // Odstrani to zahtevo
                TrenutnaObremenitev = Math.Max(0, TrenutnaObremenitev - steviloPotnikov);  // Izstop potnikov
                Console.WriteLine($"Dvigalo {Id} je pripeljalo potnike v nadstropje {ciljnoNadstropje}.");
            }
        }
    }


    public bool LahkoSprejmePotnike(int stevilo)
    {
        return (TrenutnaObremenitev + stevilo) <= Kapaciteta;
    }
}
