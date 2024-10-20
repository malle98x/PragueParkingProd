using System;
namespace PragueParking
{
    class Program
    {
        static string[] p = new string[100];
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("=== Prague Parking ===\n1. Parkera ett fordon\n2. Flytta ett fordon\n3. Ta bort ett fordon\n4. Sök efter ett fordon\n5. Visa parkeringsplatser\n6. Avsluta\nVälj ett alternativ: ");
                string c = Console.ReadLine(); Console.Clear();
                if (c == "1") Park(); else if (c == "2") Move(); else if (c == "3") Remove(); else if (c == "4") Search(); else if (c == "5") Display(); else if (c == "6") { Console.WriteLine("Tack för att du använde Prague Parking. Hejdå!"); break; } else Console.WriteLine("Ogiltigt val. Välj ett alternativ mellan 1 och 6.");
                Console.WriteLine("\nTryck på Enter för att fortsätta..."); Console.ReadLine(); Console.Clear();
            }
        }
        static void Park()
        {
            Console.Write("Ange fordonstyp (CAR/MC): "); string t = Console.ReadLine().ToUpper();
            if (t != "CAR" && t != "MC") { Console.WriteLine("Ogiltig fordonstyp."); return; }
            Console.Write("Ange registreringsnummer (max 10 tecken): "); string r = Console.ReadLine().ToUpper();
            if (string.IsNullOrWhiteSpace(r) || r.Length > 10) { Console.WriteLine("Ogiltigt registreringsnummer."); return; }
            if (Find(r, out int i)) { Console.WriteLine($"Fordon med registreringsnummer {r} är redan parkerat på plats {i + 1}."); return; }
            if (t == "CAR")
            {
                int s = FindEmptySpotForCar();
                if (s != -1) { p[s] = $"CAR#{r}"; Console.WriteLine($"Bil parkerad på plats {s + 1}."); }
                else Console.WriteLine("Inga lediga parkeringsplatser för bilar.");
            }
            else
            {
                int s = FindSpotForMC();
                if (s != -1)
                {
                    if (string.IsNullOrEmpty(p[s])) p[s] = $"MC#{r}";
                    else p[s] += $"|MC#{r}";
                    Console.WriteLine($"Motorcykel parkerad på plats {s + 1}.");
                }
                else Console.WriteLine("Inga lediga parkeringsplatser för motorcyklar.");
            }
        }
        static void Move()
        {
            Console.Write("Ange registreringsnumret på fordonet som ska flyttas: "); string r = Console.ReadLine().ToUpper();
            if (!Find(r, out int c)) { Console.WriteLine("Fordonet hittades inte."); return; }
            Console.Write("Ange nytt parkeringsplatsnummer (1-100): ");
            if (!int.TryParse(Console.ReadLine(), out int n) || n < 1 || n > 100) { Console.WriteLine("Ogiltigt parkeringsplatsnummer."); return; }
            n -= 1;
            string v = Extract(r, c);
            if (v.StartsWith("CAR#"))
            {
                if (string.IsNullOrEmpty(p[n]))
                {
                    p[n] = v;
                    Console.WriteLine($"Bil flyttad till plats {n + 1}.");
                    RemoveData(r, c);
                }
                else Console.WriteLine("Den nya platsen är inte tillgänglig för en bil.");
            }
            else if (v.StartsWith("MC#"))
            {
                if (CanParkMC(n))
                {
                    if (string.IsNullOrEmpty(p[n])) p[n] = v;
                    else p[n] += $"|{v}";
                    Console.WriteLine($"Motorcykel flyttad till plats {n + 1}.");
                    RemoveData(r, c);
                }
                else Console.WriteLine("Den nya platsen är inte tillgänglig för en motorcykel.");
            }
        }
        static void Remove()
        {
            Console.Write("Ange registreringsnumret på fordonet som ska tas bort: "); string r = Console.ReadLine().ToUpper();
            if (Find(r, out int i)) { RemoveData(r, i); Console.WriteLine("Fordonet har tagits bort från parkeringsplatsen."); }
            else Console.WriteLine("Fordonet hittades inte.");
        }
        static void Search()
        {
            Console.Write("Ange registreringsnummer att söka efter: "); string r = Console.ReadLine().ToUpper();
            if (Find(r, out int i)) Console.WriteLine($"Fordon med registreringsnummer {r} är parkerat på plats {i + 1}.");
            else Console.WriteLine("Fordonet hittades inte.");
        }
        static void Display()
        {
            Console.WriteLine("=== Parkeringsplatsstatus ===");
            for (int i = 0; i < p.Length; i++) Console.WriteLine($"Plats {i + 1}: {(string.IsNullOrEmpty(p[i]) ? "Tom" : p[i])}");
        }
        static bool Find(string r, out int s)
        {
            for (int i = 0; i < p.Length; i++)
                if (!string.IsNullOrEmpty(p[i]) && p[i].Contains($"#{r}")) { s = i; return true; }
            s = -1; return false;
        }
        static int FindEmptySpotForCar()
        {
            for (int i = 0; i < p.Length; i++)
                if (string.IsNullOrEmpty(p[i])) return i;
            return -1;
        }
        static int FindSpotForMC()
        {
            for (int i = 0; i < p.Length; i++)
                if (CanParkMC(i)) return i;
            return -1;
        }
        static bool CanParkMC(int i)
        {
            if (string.IsNullOrEmpty(p[i])) return true;
            string[] v = p[i].Split('|');
            if (v.Length == 1 && v[0].StartsWith("MC#")) return true;
            return false;
        }
        static string Extract(string r, int i)
        {
            string[] v = p[i].Split('|');
            foreach (var ve in v)
                if (ve.Contains($"#{r}")) return ve;
            return null;
        }
        static void RemoveData(string r, int i)
        {
            string[] v = p[i].Split('|');
            v = Array.FindAll(v, ve => !ve.Contains($"#{r}"));
            if (v.Length == 0) p[i] = null;
            else p[i] = string.Join("|", v);
        }
    }
}