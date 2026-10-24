using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;
using Microsoft.Data.Sqlite;
namespace ElektroBudWPF.Data
{
    public static class Database
    {
        public static void CreateDB() //korzystalam z poradnika https://www.youtube.com/watch?v=66K5Nmmc9g8
        {
            using (var connect = new SqliteConnection("Data Source=elektrobud.db")) //laczenie z baza
            {
                connect.Open();
                var comm = connect.CreateCommand();

                comm.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Produkty (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    EAN TEXT UNIQUE NOT NULL,
                    Nazwa TEXT NOT NULL,
                    Kategoria TEXT NOT NULL,
                    Sektor INTEGER NOT NULL,
                    Regal INTEGER NOT NULL,
                    Polka INTEGER NOT NULL ,
                    LiczbaSztuk INTEGER NOT NULL DEFAULT 0)";
                comm.ExecuteNonQuery(); //wykonanie kwerendy https://learn.microsoft.com/en-us/dotnet/api/microsoft.data.sqlite.sqlitecommand.executenonquery?view=msdata-sqlite-10.0.0
            }
        }
        //przyjmowanie towaru
        public static bool AddProduct(string ean, string nazwa, string kategoria, int sektor, int regal, int polka, int liczbaSztuk)
        //metoda zmieniona z void na bool za radą AI
        //mialam problem przez to że przy wywolywaniu metody pojawialy sie messageboxy o nieprawidlowych danych ale dalej wartosci wpisywaly sie do podsumowującego pola tekstowego
        {
            using (var connect = new SqliteConnection("Data Source=elektrobud.db"))
            {
                connect.Open();
                var check = connect.CreateCommand(); //https://medium.com/@bridgesquared/efcore-sqlite-case-insensitive-order-21371b256e5
                check.CommandText = @"SELECT EAN, Nazwa, kategoria, Sektor, Regal, Polka FROM Produkty WHERE EAN=$ean COLLATE NOCASE OR Nazwa=$nazwa COLLATE NOCASE LIMIT 1;";
                //collate nocase pozwala na porownanie tekstow bez znaczenia czy litery są duze czy nie
                check.Parameters.AddWithValue("$ean", ean.Trim());
                check.Parameters.AddWithValue("$nazwa", nazwa.Trim());

                string savedEan = null;
                string savedNazwa = null;
                string savedKategoria = null;
                int savedSektor = 0;
                int savedRegal = 0;
                int savedPolka = 0;

                using (var reader = check.ExecuteReader())//czyta rekordy
                {
                    //https://stackoverflow.com/questions/5794529/how-to-use-executereader-method-to-retrieve-the-value-of-just-one-cell
                    if (reader.Read()) //czy kwerenda znalazla jakis rekord
                    {
                        savedEan = reader.GetString(0); //cyferki to nr kolumnt w wyniku kwerendy
                        savedNazwa = reader.GetString(1);
                        savedKategoria = reader.GetString(2);
                        savedSektor = reader.GetInt32(3);
                        savedRegal = reader.GetInt32(4);
                        savedPolka = reader.GetInt32(5);
                    }
                }
                if (savedEan != null) //sprawdzanie poprawnosci danych
                {
                    if (savedEan != ean.Trim())
                    {
                        MessageBox.Show("Produkt " + savedNazwa + " istnieje już w bazie z innym kodem EAN: " + savedEan);
                        return false;
                    }

                    if (savedNazwa != nazwa.Trim())
                    {
                        MessageBox.Show("Ten kod EAN już jest przypisany do produktU: " + savedNazwa);
                        return false;
                    }

                    if (savedPolka != polka || savedRegal != regal || savedSektor != sektor)
                    {
                        MessageBox.Show("Nieprawidłowe miejsce. Ten produkt znajduje się w: S" + savedSektor + "R" + savedRegal + "P" + savedPolka + ".Musisz podać takie samo");
                        return false;
                    }

                    //jesli wszystko bylo poprawne to aktualizuje ilosc  wbazie danych 
                    var update = connect.CreateCommand();
                    update.CommandText = @"UPDATE Produkty SET LiczbaSztuk = LiczbaSztuk + $liczbaSztuk WHERE EAN=$ean;";
                    update.Parameters.AddWithValue("$ean", ean.Trim()); //parametry polecenia (miejusca w zapytaniu, do ktorych program przekazuje daną wartosc)
                    update.Parameters.AddWithValue("$liczbaSztuk", liczbaSztuk);
                    update.ExecuteNonQuery();

                    return true;
                }
                var comm = connect.CreateCommand();



                comm.CommandText = @" INSERT INTO Produkty (EAN, Nazwa, Kategoria, Sektor, Regal, Polka, LiczbaSztuk)
                    VALUES ($ean, $nazwa, $kategoria, $sektor, $regal, $polka, $liczbaSztuk);";

                ///https://learn.microsoft.com/pl-pl/dotnet/standard/data/sqlite/parameters
                comm.Parameters.AddWithValue("$ean", ean); //parametry sa bezpiecnza opcja  dla przekazania danych do kwerendy ze wzgledu na mozliwe bledy przy bezposrednim wpisywaniu ręcznym
                comm.Parameters.AddWithValue("$nazwa", nazwa);
                comm.Parameters.AddWithValue("$kategoria", kategoria);
                comm.Parameters.AddWithValue("$sektor", sektor);
                comm.Parameters.AddWithValue("$regal", regal);
                comm.Parameters.AddWithValue("$polka", polka);
                comm.Parameters.AddWithValue("$liczbaSztuk", liczbaSztuk);
                comm.ExecuteNonQuery();
                return true;

            }
        }


        public static void ResetID()
        { 
            //stworzone do testow
           
            using (var connect = new SqliteConnection("Data Source=elektrobud.db"))
            {
                connect.Open();

                var comm = connect.CreateCommand();
                //tu pomoglo mi AI bo nie rozumiałam jak moge usunąc jakis rekord żeby pola z autoinkrementacją sie nie psuly
                comm.CommandText = @"DELETE FROM sqlite_sequence WHERE name = 'Produkty';"; //usuwanie zapamietanych numerow z autoinkrementacja (specjalma tabela sqlite)

                comm.ExecuteNonQuery();
            }
        }

        public static System.Data.DataTable GetProducts()
        {  //tworzy nowa tebelke i nizej wpisuje ja w zmienna
            var table = new System.Data.DataTable();
            using (var connect = new SqliteConnection("Data Source=elektrobud.db"))
            {
                connect.Open();

                var comm = connect.CreateCommand();
                comm.CommandText = @"SELECT ID, EAN, Nazwa, Kategoria, Sektor, Regal, Polka, LiczbaSztuk FROM Produkty;";

                using (var reader = comm.ExecuteReader()) //ma w sobie wyniki zapytania
                {
                    table.Load(reader); //ładuje odczytane dane do tabeli w pamięci progamu
                }
            }
            return table;
        }

        public static System.Data.DataTable SearchProduct(string tekst)
        {
            var table = new System.Data.DataTable();
            using (var connect = new SqliteConnection("Data Source=elektrobud.db"))
            {
                connect.Open();

                var comm = connect.CreateCommand();
                comm.CommandText = @"SELECT ID, EAN, Nazwa, Kategoria, Sektor, Regal, Polka, LiczbaSztuk FROM Produkty WHERE EAN LIKE $tekst OR Nazwa LIKE $tekst;";

                comm.Parameters.AddWithValue("$tekst", "%" + tekst + "%"); //zeby poszukac ciag znakow tez w srodku nazwy/kodu

                using (var reader = comm.ExecuteReader())
                {
                    table.Load(reader);
                }
            }
            return table;

        }

        //wydawanie produktu
        public static bool ReleaseProduct(string szukane, int sztuki)
        {

            if (string.IsNullOrWhiteSpace(szukane) || sztuki <= 0)
            {
                return false;
            }
            using (var connect = new SqliteConnection("Data Source=elektrobud.db"))
            {
                connect.Open();
                //uzylam transakcji na wypadek jakby 2 pracownikow chciałowykonać operacje na tych samych produktach
                //jej zadanie polega na tym że albo przechodzi CALOSC albo NIC wiec w razie W baza jest bezpieczna
                using (var trans = connect.BeginTransaction()) // rozpoczyna transakcje https://www.sqlite.org/lang_transaction.html
                {
                    try
                    {
                        var comm = connect.CreateCommand();
                        comm.Transaction = trans;
                        //comm.CommandText = @"UPDATE Produkty SET LiczbaSztuk = LiczbaSztuk-$sztuki WHERE EAN = $ean OR Nazwa = $Nazwa AND $sztuki>0 AND LiczbaSztuk>=$sztuki";


                        if (!string.IsNullOrWhiteSpace(szukane) && sztuki > 0)
                        {
                            //zmienia sie liczba sztuk ale jest to widoczne dla jednej osoby naraz. Inny pracownik widzi nadal starą wersje
                            comm.CommandText = @"UPDATE Produkty SET LiczbaSztuk = LiczbaSztuk-$sztuki WHERE EAN = $ean AND $sztuki>0 AND LiczbaSztuk>=$sztuki";
                            comm.Parameters.AddWithValue("$szukane", szukane.Trim());

                            //comm.CommandText = @"UPDATE Produkty SET LiczbaSztuk = LiczbaSztuk-$sztuki WHERE EAN = $ean AND $sztuki>0 AND LiczbaSztuk>=$sztuki";
                            //comm.Parameters.AddWithValue("$ean", ean);
                            //} else if (!string.IsNullOrWhiteSpace(nazwa))
                            //{
                            //comm.CommandText = @"UPDATE Produkty SET LiczbaSztuk = LiczbaSztuk-$sztuki WHERE Nazwa = $nazwa AND $sztuki>0 AND LiczbaSztuk>=$sztuki";
                            //comm.Parameters.AddWithValue("$nazwa", nazwa);
                        }
                        else
                        {
                            trans.Rollback(); //cofanie zmian w transakcji gdy nie zostaly zatwierdzone
                            return false;
                        }
                        comm.Parameters.AddWithValue("$ean", szukane.Trim());
                        comm.Parameters.AddWithValue("$sztuki", sztuki);

                        int zmianaWiersz = comm.ExecuteNonQuery(); //wykonuje sie kwerenda i zwraca ile wierszy sie zmienilo po niej

                        if (zmianaWiersz != 1)//np jak pracownik zle podał kod ean wiec nie dokonala sie zmiana
                        {
                            trans.Rollback(); //baza wraca do stanu pierwotnego
                            return false;
                        }

                        var usun = connect.CreateCommand();
                        usun.Transaction = trans;

                        usun.CommandText = @"DELETE FROM Produkty WHERE EAN = $szukane AND LiczbaSztuk = 0;";
                        usun.Parameters.AddWithValue("$szukane", szukane.Trim());
                        usun.ExecuteNonQuery();
                        //dopoki nie wykona sie commit to w przypadku problemow baza jest bezpieczna i wykona sie rollback 
                        trans.Commit(); //daje zatwierdzenie na zmiany (wydaje towar)
                        return true;
                    }
                    catch (SqliteException)
                    {
                        trans.Rollback(); //tu tez cofa niezatwierdzone zmiany w przypadku bledu
                        return false;
                    }
                }
            }
        }


        public static int? AvailableItems(string szukane)
        {
            if (string.IsNullOrWhiteSpace(szukane))
            {
                return null;
            }

            using (var connect = new SqliteConnection("Data Source=elektrobud.db"))
            {
                connect.Open();
                var comm = connect.CreateCommand();
                comm.CommandText = @"SELECT LiczbaSztuk FROM Produkty WHERE EAN = $szukane";
                comm.Parameters.AddWithValue("$szukane", szukane.Trim());

                //https://learn.microsoft.com/pl-pl/dotnet/api/system.data.sqlclient.sqlcommand.executescalar?view=netframework-4.8.1
                object wynik = comm.ExecuteScalar(); //to jest poberanie pierwszej kolumny z 1 wiersza  wyniku
                //jako object bo ExecuteScalar() moze rozne typy wyrzucic (null gdy nie zwroci zadnego wynoku) i wtedy wyrzuci blad

                if (wynik == null || wynik == DBNull.Value) //specjalne sprawdzenie czy zmienna nie ma przypisanego obiektu LUB czy jest brak wartosci jeszcze w bazie
                {
                    return null;
                }

                return Convert.ToInt32(wynik);//poniewaz z object nastepuje konwertowanie na int
            }
        }

        //potrzebowalam stworzyc tę metode do etapu testow :)
        public static void DeleteAllProducts()
        {
            using (var connect = new SqliteConnection("Data Source=elektrobud.db"))
            {
                connect.Open();

                var comm = connect.CreateCommand();
                comm.CommandText = "DELETE FROM Produkty;";

                comm.ExecuteNonQuery();
            }
        }

    }

}
