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
    public partial class Registration : Window
    {
        private void Register_Click(object sender, RoutedEventArgs e)
        {
            string storedMac = FileManagement.getProductReg();
            string decryptedMac = RegistrationApp.Decrypt(storedMac, productKey.Password);
            string encryptMac;
            if (FileManagement.getProductReg() == "")
            {
                MessageBox.Show("This copy of product is not registered. Please Enter product key to register the product.");

            }
            else if (decryptedMac == RegistrationApp.getMac())
            {

            }
        }

        private void userName_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {

        }

        private void password_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {

        }
    }
}
