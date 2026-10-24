using ElektroBudWPF.Data;
using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Logika interakcji dla klasy PrzyjmowanieTowaru.xaml
    /// </summary>
    public partial class PrzyjmowanieTowaru : Window
    {
        public PrzyjmowanieTowaru()
        {
            InitializeComponent();
        }

        private void PrzyjmijGuzikClick(object sender, RoutedEventArgs e)
        {
            int sztuki;
            ComboBoxItem wybranaKategoria = kategoriaBox.SelectedItem as ComboBoxItem;

            if (string.IsNullOrWhiteSpace(eanBox.Text) || string.IsNullOrWhiteSpace(nazwaBox.Text) || string.IsNullOrWhiteSpace(pracownikBox.Text) || string.IsNullOrWhiteSpace(sztukiBox.Text) || string.IsNullOrWhiteSpace(sektorBox.Text) || string.IsNullOrWhiteSpace(regalBox.Text) || string.IsNullOrWhiteSpace(polkaBox.Text) || string.IsNullOrWhiteSpace(dataBox.Text))
            {
                MessageBox.Show("Nie wszystkie pola są wypełnione");
                return;

            }
            else
            {
                if (!int.TryParse(sztukiBox.Text, out sztuki) || sztuki <= 0)
                {
                    MessageBox.Show("Trzeba podać dodatnią ilość sztuk");
                    return;
                }

                string ean = eanBox.Text;
                string nazwa = nazwaBox.Text;
                string pracownik = pracownikBox.Text;
                string sektor = sektorBox.Text;
                string regal = regalBox.Text;
                string data = dataBox.Text;
                string polka = polkaBox.Text;
                string kategoria = wybranaKategoria.Content.ToString();

                listaBox.Text += "EAN: " + ean + "; nazwa: " + nazwa + "; Kategoria: " + kategoria + "; Ilość: " + sztuki + " sztuk; Miejsce: S" + sektor + "R" + regal + "P" + polka + "; Data: " + data + "; ID pracownika: " + pracownik + "\n";
                Database.AddProduct(ean, nazwa, kategoria, int.Parse(sektor), int.Parse(regal), int.Parse(polka), sztuki);
            }




        }
        private void Wyszukiwanie(object sender, RoutedEventArgs e)
        {

            this.Close();
        }

    }
}
