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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ElektroBudWPF
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window

    {//ZRODLA learn.microsoft.com, youtube
        
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                Database.CreateDB();
                //MessageBox.Show("UDALO SIE TWORZENIE :) ");

                //Database.AddProduct("5901974527897", "Gniazdko podwójne", "Gniazdka", 1, 2, 7, 18);
                //MessageBox.Show("DODALO SUE");


                //Database.DeleteAllProducts();
                //Database.ResetID(); 
                produktyTabela.ItemsSource = Database.GetProducts().DefaultView; //tu zwraca dane jako DataTable ale dzieki DefaultView mozna przypisac widok do tabelki WPF
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Problem z baza danych");
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e) //to sie uruchamia przy kazdej zmianie w polu wyszkukiwarki
        {
            produktyTabela.ItemsSource = Database.SearchProduct(wyszukiwarka.Text).DefaultView;
        }

        private void Przyjmowanie(object sender, RoutedEventArgs e)
        {
            PrzyjmowanieTowaru przyjmowanieOkno = new PrzyjmowanieTowaru();
            przyjmowanieOkno.Closed += (s, args) => //przy zamykanih okna z przyjnowanymi produktami odswiezana jest tabela w glownym oknie
            {
                produktyTabela.ItemsSource = Database.GetProducts().DefaultView;
            };

            przyjmowanieOkno.Show();
        }

        private void Wydawanie(object sender, RoutedEventArgs e)
        {
            WydawanieTowaru wydawanieOkno = new WydawanieTowaru();
            wydawanieOkno.Closed += (s, args) =>
            {
                produktyTabela.ItemsSource = Database.GetProducts().DefaultView;
            };
            wydawanieOkno.Show();

        }
    }
}
