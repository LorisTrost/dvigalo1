using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


