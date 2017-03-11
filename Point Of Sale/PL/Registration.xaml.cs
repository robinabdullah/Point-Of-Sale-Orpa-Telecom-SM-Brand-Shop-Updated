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
using Point_Of_Sale.BL;

namespace Point_Of_Sale.PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Registration : Window
    {
        public Registration()
        {
            InitializeComponent();

            subscription.Items.Add("Trial");
            subscription.Items.Add("Full");
            subscription.SelectedIndex = 0;

            org.Focus();
            mac.Text = Register.getPcMac();
        }
        int days = 0;
        private void validateRegistration_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                if (org.Text == "" || mac.Text == "" || productKey.Password == "" || subscription.SelectedIndex == -1) ///validating fields
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show("one or more textboxes are empty", "Empty Text in Textbox");
                    return;
                }
                else if ((subscription.SelectedIndex == 0 && int.TryParse(trialDays.Text, out days) == false))
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show("Please enter valid the trial days", "Empty Text in Textbox");
                    return;
                }
                if (productKey.Password == Register.ProductKey)
                {
                    Register.SubscriptionDateEnd = DateTime.Now.AddDays(days);
                    string encryptMac = Register.Encrypt(mac.Text, productKey.Password);
                    string encryptOrg = Register.Encrypt(org.Text, productKey.Password);
                    string encryptDate = Register.Encrypt(Register.SubscriptionDateEnd.ToShortDateString(), productKey.Password);
                    //Console.WriteLine(date);
                    bool isDone = FileManagement.saveProductReg(encryptOrg, encryptMac, encryptDate);
                    if (isDone == true) /// if organization name and mac is saved
                    {
                        ///setting the subscription date
                        ///Register.SubscriptionDateEnd = DateTime.Parse(Register.SubscriptionDateString);
                        Xceed.Wpf.Toolkit.MessageBox.Show("Product is registered Successfully", "Registration", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();
                        PL.Login login = new PL.Login();
                        login.Show();
                    }
                }
                else
                {
                    MessageBox.Show("Invalid Product Key. Please try again.", "Invalid");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message + " " + ex.StackTrace);
            }
            
        }

        private void userName_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox t = sender as TextBox;

            if (t.Text != "")
                t.SelectAll();
        }

        private void password_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            PasswordBox t = sender as PasswordBox;
            
            if (t.Password != "")
                t.SelectAll();
        }

        private void subscription_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(subscription.SelectedIndex == 0)
            {
                trialDays.IsEnabled = true;
            }
            else if (subscription.SelectedIndex == 1)
            {
                trialDays.IsEnabled = false;
                days = 1095; ///3 years 
            }
        }
    }
}
