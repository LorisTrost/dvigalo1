using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main()
    {
        SistemDvigal sistem = new SistemDvigal(3, 10);

        // Dodamo nekaj zahtev za dvigala
        sistem.ZahtevajDvigalo(2, 7, 4);
        sistem.ZahtevajDvigalo(5, 1, 3);
        sistem.ZahtevajDvigalo(8, 0, 5);
        sistem.ZahtevajDvigalo(3, 9, 2);
        sistem.ZahtevajDvigalo(6, 4, 6);

        for (int i = 0; i < 20; i++)  // Več korakov za testiranje
        {
            Console.WriteLine($"Korak {i + 1}:");
            sistem.KorakSimulacije();
            sistem.IzpisiStatus();
            Console.WriteLine("--------------------");
        }
    }
}

public class SistemDvigal
{
    private List<Dvigalo> Dvigala;

    public SistemDvigal(int steviloDvigal, int skupnoNadstropij)
    {
        Dvigala = new List<Dvigalo>();
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

        najblizjeDvigalo.ZahtevajNadstropje(izNadstropja, vNadstropje, steviloPotnikov);
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
    public Queue<(int izNadstropja, int ciljnoNadstropje, int steviloPotnikov)> Zahteve { get; }
    public int Kapaciteta { get; }
    public int TrenutnaObremenitev { get; private set; }
    private bool pobralPotnike = false;
    private int ciljPotnikov = -1;

    public Dvigalo(int id)
    {
        Id = id;
        TrenutnoNadstropje = 0; // Vsa dvigala so na začetku v pritličju
        Zahteve = new Queue<(int, int, int)>();
        Kapaciteta = 10;
        TrenutnaObremenitev = 0;
    }

    public void ZahtevajNadstropje(int izNadstropja, int ciljnoNadstropje, int steviloPotnikov)
    {
        Zahteve.Enqueue((izNadstropja, ciljnoNadstropje, steviloPotnikov));
    }

    public void Premik()
    {
        if (Zahteve.Count == 0)
            return;

        var (izNadstropja, ciljnoNadstropje, steviloPotnikov) = Zahteve.Peek();

        // 1. Premik proti potnikom
        if (!pobralPotnike)
        {
            if (TrenutnoNadstropje < izNadstropja)
            {
                TrenutnoNadstropje++;
                PremikaSeGor = true;
            }
            else if (TrenutnoNadstropje > izNadstropja)
            {
                TrenutnoNadstropje--;
                PremikaSeGor = false;
            }
            else
            {
                // Pobere potnike
                if (LahkoSprejmePotnike(steviloPotnikov))
                {
                    TrenutnaObremenitev += steviloPotnikov;
                    ciljPotnikov = ciljnoNadstropje;
                    Console.WriteLine($"Dvigalo {Id} pobralo {steviloPotnikov} potnikov v nadstropju {izNadstropja}.");
                    pobralPotnike = true;
                }
            }
        }

        // 2. Premik proti ciljnemu nadstropju
        if (pobralPotnike)
        {
            if (TrenutnoNadstropje < ciljPotnikov)
            {
                TrenutnoNadstropje++;
                PremikaSeGor = true;
            }
            else if (TrenutnoNadstropje > ciljPotnikov)
            {
                TrenutnoNadstropje--;
                PremikaSeGor = false;
            }
            else
            {
                // Spusti potnike
                TrenutnaObremenitev -= steviloPotnikov;
                Console.WriteLine($"Dvigalo {Id} izpustilo potnike v nadstropju {ciljPotnikov}.");
                Zahteve.Dequeue();
                pobralPotnike = false;
                ciljPotnikov = -1; // Resetiramo cilj
            }
        }

        // 3. Če so še potniki, nadaljuje s prejšnjimi zahtevami
        if (TrenutnaObremenitev > 0 && Zahteve.Count > 0)
        {
            var naslednjaZahteva = Zahteve.Peek();
            ciljPotnikov = naslednjaZahteva.ciljnoNadstropje;
        }
    }

    public bool LahkoSprejmePotnike(int stevilo)
    {
        return (TrenutnaObremenitev + stevilo) <= Kapaciteta;
    }
}
