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
            
            validateRegistration();
            
            
            userName.Focus();
            //DB.resetConnString();
        }
        public void validateRegistration()
        {
            int count = FileManagement.getProductReg().Count();
            string storedEncryptedMac, storedEncryptedOrg, storedEncryptedDate;
            
            if (count != 0)/// if the Product Reg.dat is not empty
            {
                storedEncryptedOrg = FileManagement.getProductReg().ElementAt(0); ///stored in file
                storedEncryptedMac = FileManagement.getProductReg().ElementAt(1); ///stored in file
                storedEncryptedDate = FileManagement.getProductReg().ElementAt(2); ///stored in file
                try
                {
                    Register.OrgName = Register.Decrypt(storedEncryptedOrg, Register.ProductKey);
                    Register.Mac = Register.Decrypt(storedEncryptedMac, Register.ProductKey);
                    Register.SubscriptionDateString = Register.Decrypt(storedEncryptedDate, Register.ProductKey);
                    
                    Console.WriteLine(Register.OrgName + " " + Register.Mac + " " + Register.SubscriptionDateString + "HI");
                }
                catch
                {
                    MessageBoxResult res = Xceed.Wpf.Toolkit.MessageBox.Show("Product registration failed because of incorrect binding with Product Key. Please Register again.", "Registration Failed", MessageBoxButton.OKCancel, MessageBoxImage.Stop, MessageBoxResult.Cancel);
                    if (res == MessageBoxResult.Cancel)
                        Environment.Exit(0);///exit
                    this.Close();
                    Registration rr = new Registration();
                    rr.Show();
                    return;
                }
            }

            ///Console.WriteLine(decryptedMac);
            if (count == 0) /// if the file empty
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("This copy of product is not registered. Press OK to Enter the details and register the product.");
                this.Close();
                Registration rr = new Registration();
                rr.Show();
            }
            else if (Register.Mac != Register.getPcMac()) ///matching pc mac with stored encrypted mac 
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Please contact with the developer.");
                
            }
            
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

        private void checkSubscription()
        {
            DateTime date1 = Register.SubscriptionDateEnd;
            DateTime date2 = DateTime.Now;
            int result = DateTime.Compare(Register.SubscriptionDateEnd, DateTime.Now.Date);
            TimeSpan span = date1.Subtract(date2);
            //Console.WriteLine(span.TotalDays);
            int subscription = (int)span.TotalDays;
            if (subscription <= 7 && subscription >= 0)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Your subscription will be expired withing " + subscription + " days. Please contact with the Developer as soon as possible.", " Subscription", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (subscription < 0)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Your trial subscription has expired. Please contact with the Developer.", "Trial Subscription expired", MessageBoxButton.OK, MessageBoxImage.Stop);
                Environment.Exit(0);
            }

        }
        private void goButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DB.TestDBConnection();
            }
            catch (Exception ex)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message);
                return;
            }

            if (Login_TableData.verifyLogin(userName.Text, password.Password) == true)
            {
                //verify subscription
                checkSubscription();

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
                    main.welcome.Content = "WELCOME TO " + Register.OrgName;///saving org name in dsply
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
