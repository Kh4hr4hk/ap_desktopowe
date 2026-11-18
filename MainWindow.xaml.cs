using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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

namespace WpfApp2
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string vLogin { get; set; }
        public string TxValue { get; set; }
        public string vPassword { get; set; }
        public int My_int_value { get; set; }
        public ObservableCollection<int> My_collection { get; set; }
        public MainWindow()
        {
            InitializeComponent();

            this.My_Collection = new ObservableCollection<int>();
            DataContext = this;

        }

        private void Btn_login(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txLogin.Text) || string.IsNullOrWhiteSpace(txPass.Text))
            {
                MessageBox.Show("Wypelnij pola ");
                return;
            }
            else
            {
                vLogin = txLogin.Text;
                vPassword = txPass.Text;
                MessageBox.Show("Twój login to: " + vLogin);

            }
        }

        private void Btn_add(object sender, RoutedEventArgs e)
        {
            if (validate_number(this.txValue)) this.My_collection.Add(this.My_int_value);

        }

        private bool validate_number(string text)
        {
            var isNumber = int.TryParse(text, out var number);
            this.My_int_value = number;
            return isNumber;
        }
    }
}

