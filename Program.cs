using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Hangman
{
    class Program
    {
        static Random random = new Random();
        static List<string> capitalsAndCounties; //lista par panstw i stolic

        static void Main(string[] args)
        {
            loadCapitalsAndCountries(); //wczytanie panstw i stolic z pliku
            start(); //rozpoczecie nowej gry
        }

        static void loadCapitalsAndCountries()
        {
            //wczytanie wszystkich linii pliku do listy stringow
            capitalsAndCounties = new List<string>(File.ReadAllLines("..\\..\\..\\countries_and_capitals.txt"));
        }

        static string drawCapitalCountryPair()
        {
            //losowanie pary panstwa i stolicy
            return capitalsAndCounties[random.Next(capitalsAndCounties.Count)];
        }

        static void start()
        {
            int lifePoints = 5; //pozostale punkty zdrowia
            int tries = 0; //liczba prob w tym podejsciu
            List<char> notInWord = new List<char>(); //lista podanych znakow, ktore nie pasuja
            string capitalAndCountryPair = drawCapitalCountryPair();
            //rozdzielenie panstwa i stolicy o formacie Panstwo | Stolica
            string country = capitalAndCountryPair.Split(" | ")[0];
            string capital = capitalAndCountryPair.Split(" | ")[1];
            StringBuilder guessed = new StringBuilder(); //odgadniete haslo
            guessed.Append('_', capital.Length); //na poczatku odgadniete haslo to same podkreslenia
            bool finished = false; //czy gra zostala zakonczona
            bool win = false; //czy gracz wygral
            char choice;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start(); //poczatek pomiaru czasu

            while(!finished) //dopoki gracz nie odgadl hasla, ani nie wyczerpal punktow zdrowia
            {
                Console.Clear();
                Console.WriteLine("The Hangman Game\n");
                Console.WriteLine(guessed.ToString() + "\n"); //wyswietlenie odgadnietego do tego momentu hasla np. Wa_s_awa

                if(lifePoints < 3) //wyswietlenie podpowiedzi jezeli liczba punktow zdrowia spadla ponizej 3
                {
                    Console.WriteLine("The capital of " + country + "\n");
                }

                Console.WriteLine("Life points: " + lifePoints + "\n"); //wyswietlnie aktualnej liczby punktow zdrowia
                Console.WriteLine("Not in word: " + string.Join(", ", notInWord) + "\n"); //wyswietlnie podanych znakow, ktore nie pasuja oddzielonych przecinkiem np. a, h, o
                Console.WriteLine("1. Guess a letter.");
                Console.WriteLine("2. Guess whole word(s).");

                char letter;
                string word;

                do
                {
                    choice = Console.ReadKey().KeyChar; //uzytkownik wybiera, czy chce podac litere, czy cale haslo
                    tries++; //za kazdym razem zwiekszamy liczbe prob o jeden
                    switch(choice)
                    {
                        case '1': //zgaduje litere
                        {
                            Console.WriteLine();
                            Console.Write("Letter: ");
                            letter = char.ToLower(Console.ReadKey().KeyChar); //uzytkownik podaje litere

                            if(capital.ToLower().Contains(letter)) //czy litera znajduje sie w nazwie stolicy (bez wzgledu na wielkosc liter)
                            {
                                for(int i = 0; i < capital.Length; i++) //wypelnienie wszystkich miejsc z podana litera
                                {
                                    if(capital.ToLower()[i] == letter)
                                    {
                                        guessed[i] = capital[i];
                                    }
                                }
                            }
                            else //jezeli podano nieprawidlowa odpowiedz
                            {
                                lifePoints--; //zostaje odjety 1 punkt
                                notInWord.Add(letter); //podana litera zostaje dodana do listy niepasujacych
                            }

                            break;
                        }
                        case '2': //zgaduje haslo
                        {
                            Console.WriteLine();
                            Console.Write("Word: ");
                            word = Console.ReadLine(); //uzytkownik podaje haslo

                            if(word.ToLower() == capital.ToLower())
                            {
                                guessed.Clear();
                                guessed.Append(capital);
                            }
                            else //jezeli podano nieprawidlowa odpowiedz
                            {
                                lifePoints -= 2; //zostaja odjete 2 punkty
                            }

                            break;
                        }
                        default:
                        {
                            Console.WriteLine("\nPlease choose correct option.");
                            break;
                        }
                    }
                } while(!(choice == '1' || choice == '2')); //jezeli podana nieprawidlowa wartosc (inna niz 1 lub 2) - uzytkownik wybiera jeszcze raz

                if(lifePoints < 1 || guessed.ToString() == capital) //jezeli liczba punktow zdrowia jest mniejsza od 1 lub uzytkownik odgadl cale haslo
                {
                    finished = true; //koniec gry
                }

                if(guessed.ToString() == capital) //jezeli w momencie zakonczenia gry odgadniete haslo jest rowne nazwie stolicy - uzytkownik wygral
                {
                    win = true; //zwyciestwo
                }
            }

            stopwatch.Stop(); //koniec pomiaru czasu
            Console.Clear();
            Console.WriteLine("\nGame over! You " + (win ? "won!" : "lost.")); //ogloszenie wyniku

            if(win) //jezeli uzytkownik wygral
            {
                Console.WriteLine("What's your name?");
                string name = Console.ReadLine(); //uzytkowik podaje imie
                //zapis do pliku highscore.txt                                     imie | data i czas | czas zgadywania w sekundach | liczba prob | odgadnieta stolica
                File.WriteAllText("..\\..\\..\\highscore.txt", string.Join(" | ", name, DateTime.Now.ToString(), stopwatch.Elapsed.Seconds + " s", tries, capital));
            }

            //zapytanie, czy uzytkownik chce zagrac ponownie
            Console.WriteLine("Would you like to play again?");
            Console.WriteLine("1. Yes.");
            Console.WriteLine("2. No.");

            do
            {
                choice = Console.ReadKey().KeyChar;

                switch(choice)
                {
                    case '1': //nowa gra
                    {
                        start();
                        break;
                    }
                    case '2': //wyjscie
                    {
                        Environment.Exit(0);
                        break;
                    }
                    default:
                    {
                        break;
                    }
                }
            } while(!(choice == '1' || choice == '2')); //jezeli podana nieprawidlowa wartosc (inna niz 1 lub 2) - uzytkownik wybiera jeszcze raz
        }
    }
}
