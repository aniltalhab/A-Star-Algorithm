using System;
using System.Collections.Generic;

class Puzzle
{
    public int[,] Tahta;
    public int AdimSayisi;
    public int HataSayisi;
    public Puzzle Onceki;

    public Puzzle(int[,] tahta, int adimSayisi, int hataSayisi, Puzzle onceki)
    {
        Tahta = tahta;
        AdimSayisi = adimSayisi;
        HataSayisi = hataSayisi;
        Onceki = onceki;
    }

    public int ToplamMaliyet() => AdimSayisi + HataSayisi;
}

class Program
{
    static int[,] hedef = {
        {1, 2, 3},
        {4, 5, 6},
        {7, 8, 0}
    };

    static void Main()
    {
        int[,] baslangic = new int[3, 3];

        Console.WriteLine("Başlangıç durumunu girin (3x3 matris, satır satır, her rakam arasında 1 boşluk olacak ve rakam yerine boşluk için 0 kullanılacak):");
        for (int i = 0; i < 3; i++)
        {
            string[] satir = Console.ReadLine().Split();
            for (int j = 0; j < 3; j++)
            {
                baslangic[i, j] = int.Parse(satir[j]);
            }
        }

        Puzzle sonuc = AStar(baslangic);

        if (sonuc == null)
        {
            Console.WriteLine("Çözüm bulunamadı.");
        }
        else
        {
            Console.WriteLine("Çözüm Adımları:");
            YazdirCozumAdimlari(sonuc);
        }
    }

    static Puzzle AStar(int[,] baslangic)
    {
        var baslangicPuzzle = new Puzzle(baslangic, 0, HataHesapla(baslangic), null);
        var acikListe = new List<Puzzle> { baslangicPuzzle };
        var kapaliListe = new HashSet<string>();

        while (acikListe.Count > 0)
        {
            acikListe.Sort((a, b) => a.ToplamMaliyet().CompareTo(b.ToplamMaliyet()));
            var mevcut = acikListe[0];
            acikListe.RemoveAt(0);

            if (AyniMi(mevcut.Tahta, hedef))
            {
                return mevcut;
            }

            string durumKodu = TahtaKodu(mevcut.Tahta);
            if (kapaliListe.Contains(durumKodu))
                continue;

            kapaliListe.Add(durumKodu);

            foreach (var komsu in Komsular(mevcut))
            {
                acikListe.Add(komsu);
            }
        }

        return null;
    }

    static List<Puzzle> Komsular(Puzzle mevcut)
    {
        var liste = new List<Puzzle>();
        (int x, int y) = BoslukNerede(mevcut.Tahta);

        int[,] yonler = { { 0, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 } };

        for (int i = 0; i < 4; i++)
        {
            int nx = x + yonler[i, 0];
            int ny = y + yonler[i, 1];

            if (nx >= 0 && nx < 3 && ny >= 0 && ny < 3)
            {
                int[,] yeniTahta = (int[,])mevcut.Tahta.Clone();
                (yeniTahta[x, y], yeniTahta[nx, ny]) = (yeniTahta[nx, ny], yeniTahta[x, y]);

                int yeniHata = HataHesapla(yeniTahta);
                liste.Add(new Puzzle(yeniTahta, mevcut.AdimSayisi + 1, yeniHata, mevcut));
            }
        }

        return liste;
    }

    static int HataHesapla(int[,] tahta)
    {
        int hata = 0;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (tahta[i, j] != hedef[i, j] && tahta[i, j] != 0)
                {
                    hata++;
                }
            }
        }
        return hata;
    }

    static (int, int) BoslukNerede(int[,] tahta)
    {
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
                if (tahta[i, j] == 0)
                    return (i, j);
        throw new Exception("Boşluk bulunamadı!");
    }

    static string TahtaKodu(int[,] tahta)
    {
        string kod = "";
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
                kod += tahta[i, j] + ",";
        return kod;
    }

    static bool AyniMi(int[,] a, int[,] b)
    {
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
                if (a[i, j] != b[i, j])
                    return false;
        return true;
    }

    static void YazdirCozumAdimlari(Puzzle puzzle)
    {
        var adimlar = new Stack<int[,]>();
        while (puzzle != null)
        {
            adimlar.Push(puzzle.Tahta);
            puzzle = puzzle.Onceki;
        }

        while (adimlar.Count > 0)
        {
            YazdirTahta(adimlar.Pop());
            Console.WriteLine();
        }
    }

    static void YazdirTahta(int[,] tahta)
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Console.Write(tahta[i, j] + " ");
            }
            Console.WriteLine();
        }
    }
}