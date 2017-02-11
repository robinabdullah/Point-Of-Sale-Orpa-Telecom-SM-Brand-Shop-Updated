
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
using Point_Of_Sale.DAL;

namespace Point_Of_Sale.PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
        }

        private void sellProduct_Click(object sender, RoutedEventArgs e)
        {
            Billing bl = new Billing();
            bl.ShowDialog();
        }

        private void purchaseProduct_Click(object sender, RoutedEventArgs e)
        {
            Stock st = new Stock();
            st.ShowDialog();
        }

        private void searchProduct_Click(object sender, RoutedEventArgs e)
        {
            Search_Product ss = new Search_Product();
            ss.ShowDialog();
        }

        private void products_Click(object sender, RoutedEventArgs e)
        {
            ProductsWindow products = new ProductsWindow();
            //ss.ShowDialog();    

            Login loginInterface = new Login(products);
            loginInterface.userName.Text = "PRODUCT ADMIN";
            loginInterface.userName.IsEnabled = false;
            loginInterface.password.Clear();
            loginInterface.password.Focus();
            loginInterface.ShowDialog();
        }

        private void settings_Click(object sender, RoutedEventArgs e)
        {
            Settings  setting = new Settings();
            //ss.ShowDialog();

            Login loginInterface = new Login(setting);
            loginInterface.userName.Text = "SETTING ADMIN";
            loginInterface.userName.IsEnabled = false;
            loginInterface.password.Clear();
            loginInterface.password.Focus();
            loginInterface.ShowDialog();
        }

        private void reports_Click(object sender, RoutedEventArgs e)
        {
            Reports reports = new Reports();
            //reports.ShowDialog();

            Login loginInterface = new Login(reports);
            loginInterface.userName.Text = "REPORT ADMIN";
            loginInterface.userName.IsEnabled = false;
            loginInterface.password.Focus();
            loginInterface.password.Clear();
            loginInterface.ShowDialog();
        }

        
    }
}
