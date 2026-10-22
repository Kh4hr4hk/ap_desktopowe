using ElektroBudWPF.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ElektroBudWPF
{
    /// <summary>
    /// Logika interakcji dla klasy WydawanieTowaru.xaml
    /// </summary>
    public partial class WydawanieTowaru : Window
    {
        public WydawanieTowaru()
        {
            InitializeComponent();
        }

        private void WydajGuzikClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(eanBox.Text))
            {
                MessageBox.Show("Podaj kod EAN produktu do wydania");
                return;
            }

            DataTable wynik = Database.SearchProduct(eanBox.Text);

            if (wynik.Rows.Count == 0)
            {
                MessageBox.Show("Brak produktu w bazie, sprawdź poprawność wprowadzonych danych");
                return;
            }

            string szukane = eanBox.Text;
            int sztuki;

            if (!int.TryParse(sztukiBox.Text, out sztuki) || sztuki <= 0)
            {
                MessageBox.Show("Podaj prawidłową liczbę sztuk");
                return;
            }

            if (string.IsNullOrWhiteSpace(pracownikBox.Text) || string.IsNullOrWhiteSpace(dataBox.Text))
            {
                MessageBox.Show("WYpełnij wszystkie dane");
                return;

            }

            string pracownik = pracownikBox.Text;
            string data = dataBox.Text;

            bool mozliwoscWydania = Database.ReleaseProduct(szukane, sztuki);
            if (!mozliwoscWydania)
            {
                MessageBox.Show("Wydawanie towaru się nie powiodło");
                return;
            }
            if (!string.IsNullOrWhiteSpace(pracownikBox.Text) || !string.IsNullOrWhiteSpace(dataBox.Text) || !string.IsNullOrWhiteSpace(eanBox.Text))
                if (int.TryParse(sztukiBox.Text, out sztuki) && sztuki > 0 && mozliwoscWydania == true)
                {
                    listaBox.Text += "EAN produktu: " + szukane + "; Liczba wydanych sztuk: " + sztuki + "; Pracownik ID: " + pracownik + "; Data: " + data + "\n";
                }
        }

        private void eanOrNameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string szukane = eanBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(szukane))
            {
                dostepneBox.Text = "-";
                return;
            }

            // ? bo dostepne moze byc null albo liczba calkowitą
            int? dostepne = Database.AvailableItems(szukane);
            if (dostepne.HasValue)
            {
                dostepneBox.Text = dostepne.Value.ToString();
            }
            else
            {
                dostepneBox.Text = "-";
                //MessageBox.Show("Nie znaleziono");
            }
        }

        private void Wyszukiwanie(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
