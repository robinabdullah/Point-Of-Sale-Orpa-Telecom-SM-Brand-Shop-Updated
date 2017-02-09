using System;
using System.Collections.Generic;
using System.IO;
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
    public partial class Login : Window
    {
        private Settings setting;
        private MainWindow main = new MainWindow();
        private Reports reports;
        private ProductsWindow products;
        
        public Login()
        {
            InitializeComponent();
            password.Focus();
            //DB.resetConnString();
        }
        public Login(Settings setting)
        {
            InitializeComponent();
            this.setting = setting;
            DB.resetConnString();
        }

        public Login(Reports reports)
        {
            InitializeComponent();
            this.reports = reports;
            DB.resetConnString();
        }

        public Login(ProductsWindow products)
        {
            InitializeComponent();
            this.products = products;
            DB.resetConnString();
        }

        private int hasSubscription()
        {
            DateTime date1 = DB.subscriptionDatetime;
            DateTime date2 = DateTime.Now;
            int result = DateTime.Compare(DB.subscriptionDatetime, DateTime.Now);
            TimeSpan span = date1.Subtract(date2);
            //Console.WriteLine(span.TotalDays);
            return (int) span.TotalDays;

        }
        private void goButton_Click(object sender, RoutedEventArgs e)
        {
            int subscription = hasSubscription();
            //checking subscription till a date
            if (subscription <= 7 && subscription >= 0)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Your subscription will be expired withing " + subscription + " days. Please contact with the Developer as soon as possible.", "Trial Subscription expired", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (subscription < 0)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Your trial subscription has expired. Please contact with the Developer.", "Trial Subscription expired", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            try
            {
                DB.TestDBConnection();
            }
            catch(Exception ex)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message);
                return;
            }

            if (Login_TableData.verifyLogin(userName.Text, password.Password) == true)
            {
                if (setting != null)
                {
                    this.Close();
                    setting.ShowDialog();
                }
                else if (reports != null)
                {
                    this.Close();
                    reports.ShowDialog();
                }
                else if (products != null)
                {
                    this.Close();
                    products.ShowDialog();
                }
                else
                {
                    this.Close();
                    main.Show();
                }
            }
            else
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Wrong ID or Password. Please try again.", "Invalid", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        private void password_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            password.SelectAll();
        }

        private void userName_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            userName.SelectAll();
        }
    }
}
