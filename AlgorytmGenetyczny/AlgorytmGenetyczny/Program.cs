//
// 13K3 Trzop Artur (c) 2012 Politechnika Krakowska, Wydział Mechaniczny
//
// Wyznaczyć maximum dla funkcji f(x)=x^2 przy użyciu algorytmu genetycznego.
// Dziedzina <0;10>
// Dokładność: 0.01
// Podzielić <0;10> na 1024 podprzedziały
// Uznajemy, że osiągnięto cel jeżeli następne rozwiązanie będzie różniło się o mniej niż 0.01
// Genotyp składa się z 10 genów. np. 00 0000 0000    
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace AlgorytmGenetyczny
{  
    class Program
    {
        const double WAGA = 0.01;

        static void Main(string[] args)
        {
            Console.WriteLine("13K3 Trzop Artur (c) 2012 Politechnika Krakowska, Wydział Mechaniczny\n");
            Console.WriteLine("=====================================================================");

            // Każdy chromosom składa się z 10 genów.
            // Budujemy słownik chromosomów.
            // Klucz słownika to kolejne liczby naturalne od zera, przyporządkowane do argumentów na osi x, 
            // które to argumenty wyrażone są wartościami od 0, 0.01, 0.02, itd. aż do 10.
            Dictionary<int, Chromosom> ArrChrom = new Dictionary<int, Chromosom>();

            
            // Warunek do 10.24 ponieważ chcemy mieć wszystkie możliwe do zapisania genotypy od 00 0000 0000 do 11 1111 1111 
            int key = 0;
            for (double x = 0; x <= 10.24; x += WAGA)
            {
                ArrChrom.Add(key, new Chromosom()
                {
                    // wagaDoubleToBin zwraca string, który np. dla x=0.01 wyniesie 00 0000 0001
                    genotyp = wagaDoubleToBin(x),
                    
                    // Wartość funkcji f dla argumentu x będącego kolejnym krokiem o długości 0.01*n, gdzie n to numer kroku pętli for
                    fenotyp = f(x) 
                });
                
                // Drukuj dodany do słownika chromosom
                // Console.WriteLine("key: " + key + " (" + x + ") => { \n\tgenotyp: " + ArrChrom[key].genotyp + "\n\tfenotyp: " + ArrChrom[key].fenotyp + "\n}");
                
                key++;
            }

            // Testowe sprawdzenie poprawności działania metod wagaBinToDouble i wagaDoubleToBin
            // Console.WriteLine(wagaBinToDouble("0000000001") + " = " + wagaDoubleToBin(0.01));


            Random random = new Random();
            int r; // przechowuje wylosowaną liczbę int
            double rd; // przechowuje wylosowaną liczbę double 


            // przechowuje populację
            List<Chromosom> Populacja = new List<Chromosom>();
            
            // przechowuje najlepszy chromosom jaki udało nam się dotychczas uzyskać ze wszystkich generacji rozwijających się populacji
            Chromosom NajlepszyChromosom = new Chromosom();
            
            // kontrolny fenotyp przed ostatniego chromosomu do porównywania czy znaleźliśmy lepsze wyniki
            double? PrzedOstatniNajlepszyFenotyp = null;

            int LiczebnoscPoczatkowejPopulacji = 20;

            // ustawienie warunku STOP na true spowoduje przerwanie tworzenia nowych generacji rozwijających się populacji
            bool STOP = false;
            while (!STOP) // int generacja=0; generacja < 10; generacja++ // alternatywne sztywne ustalenie ile generacji chcemy stworzyć
            {
                // czyścimy populacje przed rozpoczęciem nowej generacji 
                Populacja.Clear(); 

                // startujemy w okolicy x=0.5 czyli np. wylosujemy liczbę 50 * 0.01 = 0,5
                //r = random.Next(30, 70);
                r = random.Next(0, 1024 - LiczebnoscPoczatkowejPopulacji);
                rd = r * WAGA;
                key = r;

                // wybieramy 20 kolejnych po sobie osobników do populacji startowej                
                //double warunek = rd + (LiczebnoscPoczatkowejPopulacji * WAGA);
                //for (double x = rd; x < warunek; x += WAGA)
                //{
                //    //Console.WriteLine(x);
                //    Populacja.Add(ArrChrom[key]);
                //    Console.WriteLine("key: " + key + " (" + x + ") => { \n\tgenotyp: " + ArrChrom[key].genotyp + "\n\tfenotyp: " + ArrChrom[key].fenotyp + "\n}");
                //    key++;
                //}


                // Wybieramy osobników w sposób losowy do populacji startowej.
                // Daje to lepsze wyniki niż sposób kilka linijek wyżej, który pobiera 20 kolejnych po sobie osobników.
                for (int i = 0; i < 20; i++)
                {
                    key = random.Next(0, 1024); // wylosowana zostanie liczba od 0 do 1023 włącznie                    
                    Populacja.Add(ArrChrom[key]);
                    Console.WriteLine("key: " + key + " => { \n\tgenotyp: " + ArrChrom[key].genotyp + "\n\tfenotyp: " + ArrChrom[key].fenotyp + "\n}");                    
                }



                int whileWarunek = 0;
                while(whileWarunek < 500) // 500 na sztywno liczba możliwych pętli do przeprowadzenia 
                {
                    whileWarunek++;

                    // szukanie najlepszych osobników  
                    Console.WriteLine("\nSzukanie najlepszych osobników w danej populacji:");

                    // Przy pomocy LINQ budujemy zapytanie wybierające z populacji chromosomy posortowane od największej wartości fenotypu.
                    IEnumerable query = Populacja.OrderByDescending(chrom => chrom.fenotyp);
                    
                    // przygotowanie zmiennych przechowujących genotyp rodzica
                    string rodzic1Genotyp = null;
                    string rodzic2Genotyp = null;
            
                    var l = 0;
                    foreach(Chromosom i in query)
                    {
                        //Console.WriteLine(i.fenotyp);
                        if (rodzic1Genotyp == null)
                        {
                            rodzic1Genotyp = i.genotyp;
                            NajlepszyChromosom = i; // zapisujemy ten genotyp bo ma najwyższą wartość f(x)
                            if (PrzedOstatniNajlepszyFenotyp != null)
                            {
                                if ((PrzedOstatniNajlepszyFenotyp - i.fenotyp) % WAGA < WAGA) 
                                { 
                                    // możemy zakończyć poszukiwania ponieważ kolejny wynik różni się z dokładnością mniejszą niż 0.01
                                    STOP = true;
                                }
                            } 
                            PrzedOstatniNajlepszyFenotyp = i.fenotyp;            
                            Console.WriteLine("Rodzic1 => { genotyp: " + i.genotyp + ", fenotyp: " + i.fenotyp + " }");
                        }
                        else
                        {
                            rodzic2Genotyp = i.genotyp;
                            Console.WriteLine("Rodzic2 => { genotyp: " + i.genotyp + ", fenotyp: " + i.fenotyp + " }");
                        }

                        // zatrzymujemy foreach po pobraniu dwóch genotypów
                        if (l++ > 0)
                            break;                
                    }


                    // krzyżowanie rodziców
                    Console.WriteLine("\nKrzyżowanie rodziców:");

                    string dziecko1Genotyp = rodzic1Genotyp.Substring(0, 5) + rodzic2Genotyp.Substring(5, 5);
                    string dziecko2Genotyp = rodzic2Genotyp.Substring(0, 5) + rodzic1Genotyp.Substring(5, 5);

                    Console.WriteLine("Dziecko1: " + dziecko1Genotyp);
                    Console.WriteLine("Dziecko2: " + dziecko2Genotyp);

            
                    // mutacja z pewnym prawdopodobieństwem zadanym w metodzie mutacja()
                    string mutacja1 = mutacja(dziecko1Genotyp);
                    if (mutacja1 != null)
                    {
                        dziecko1Genotyp = mutacja1;
                        Console.WriteLine("Mutacja Dziecka1: " + dziecko1Genotyp);
                    }
            
                    string mutacja2 = mutacja(dziecko2Genotyp);
                    if (mutacja2 != null)
                    {
                        dziecko2Genotyp = mutacja2;
                        Console.WriteLine("Mutacja Dziecka2: " + dziecko2Genotyp);
                    }
                    

                    // znajdujemy obiekty dzieci w słowniku ArrChrom
                    query = from p in Populacja where p.genotyp.Equals(dziecko1Genotyp) || p.genotyp.Equals(dziecko2Genotyp) select p;
                    int dzieckoWarunekStopu = 0;
                    foreach (Chromosom current in query)
                    {
                        // jeśli dany chromosom jeszcze nie istnieje w populacji to możemy go dodać
                        if (!Populacja.Any(chromosom => chromosom.fenotyp == current.fenotyp))
                        {
                            Populacja.Add(current);
                            Console.WriteLine("+ Dodano chromosom dziecko " + current.genotyp + " do populacji.");
                        }
                        else
                        {
                            Console.WriteLine("- Nie dodano chromosomu dziecka " + current.genotyp + " do populacji ponieważ już w niej istnieje.");
                            dzieckoWarunekStopu++;
                        }
                    }

                    if (dzieckoWarunekStopu == 2)
                    {
                        // Od komentować poniższe 3 linie w /* takim komentarzu */ jeżeli chcemy zatrzymać pętlę w przypadku pojawienia się dzieci o takich 
                        // samych genotypach jak osobniki już w populacji. 
                        //
                        // Gdy dzieci mają takie same genotypy jak osobniki już istniejące w populacji to nadal ci sami rodzice będą 
                        // dominować w populacji. Czynnikiem, który może spowodować pojawienie się potomków nie dublujących się jest:
                        // a) populacja składa się z losowych chromosomów przez co rzadko będą dublować się otrzymane dzieci z osobnikami w populacji.
                        // b) wprowadzono mutacje u dzieci dzięki temu istnieje szansa na pojawienie się dziecka o odmiennym genotypie (to rozwiązanie przyjęto w tym programie).
                        // 
                        /*
                        Console.WriteLine("Koniec tej populacji. Rodzice są dominujący i nie pojawiają się dzieci o lepszych genach.");
                        Console.WriteLine("----------------------------------");
                        break;
                        */
                    }
                    Console.WriteLine("----------------------------------");

                } // koniec while

            } // koniec while generacji
            

            // Wyświetlamy najlepszy chromosom dotychczas znaleziony.            
            Console.WriteLine("\n\nNajlepszy chromosom => { \n\tgenotyp: " + NajlepszyChromosom.genotyp + "\n\tfenotyp: " + NajlepszyChromosom.fenotyp + "\n}");

            Console.WriteLine("\nKONIEC");
            Console.ReadKey();
        }

        public struct Chromosom
        {
            // geneotyp składa się z 10 genów będących bitami 
            public string genotyp;

            // interpretacja genotypu w postaci liczby double
            public double fenotyp;
        }

        // Funkcja oceny albo inaczej funkcja przystosowania.
        static double f(double x)
        {
            // nasza funkcja f(x)=x^2        
            return x*x;
        }

        /*
         * Zamienia genotyp: 00 0000 0000 na jego odpowiednik w systemie dziesiętnym, a następnie mnoży przez odpowiednią wagę.        
         * Tak uzyskany wynik można podać jako argument do funkcji f(x)        
         * Przykład: 
         *      00 0000 0000 * 0.01 = 0
         *      00 0000 0001 * 0.01 = 0.01
         *      00 0000 0010 * 0.01 = 0.02
         */
        static double wagaBinToDouble(string genotyp)
        {
            // zamieniamy string na int a następnie mnożymy przez wagę
            return Int32.Parse(genotyp) * WAGA; // waga 0.01
        }

        // np. liczbę d=0.02 mnożymy przez 100 aby uzyskać int, a następnie zapisać ją binarnie
        static string wagaDoubleToBin(double d)
        {
            // mnożymy przez odwrotność wagi
            double tmp = d * 100;
           
            // konwersja na postać binarną, dopisujemy brakujące zera            
            return Convert.ToString((int)tmp, 2).PadLeft(10, '0');
        }
        
        static string mutacja(string dziecko)
        {
            // mutacja z prawdopodobieństwem 10%
            Random random = new Random();
            int r = random.Next(0, 10);
            string tmpstr;

            if (r == 0)
            {
                r = random.Next(0, 9); // wylosowana liczba z przedziału <0;8>
                r = 9;
                tmpstr = dziecko.Substring(r, 1);

                // zamiana znaku
                if (tmpstr == "1")
                    tmpstr = "0";
                else
                    tmpstr = "1";

                if (r == 0)
                {
                    dziecko = tmpstr + dziecko.Substring(1, 9);
                }
                else if (r == 9)
                {
                    dziecko = dziecko.Substring(0, 9) + tmpstr;
                }
                else
                {
                    dziecko = dziecko.Substring(0, r) + tmpstr + dziecko.Substring(r + 1, 9 - r);
                }

                return dziecko;
            }
            else
            {
                return null;
            }
        }
    }
}
