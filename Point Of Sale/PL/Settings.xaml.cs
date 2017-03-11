using Microsoft.WindowsAPICodePack.Dialogs;
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
using Point_Of_Sale.BL;

namespace Point_Of_Sale.PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();

            organization.Text = Register.OrgName;

            try
            {
                FileManagement.checkReceiptSavingLocation();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            receiptSavingLocationTextbox.Text = FileManagement.ReceiptSavingPath;

            //for changing password
            try
            {
                var userList = Login_TableData.getAllUsers();
                List<string> usernameList = new List<string>();

                foreach (DAL.Login item in userList) usernameList.Add(item.Username);

                usernameCombo.ItemsSource = usernameList;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void changeReceiptSavingLocation_Click(object sender, RoutedEventArgs e)
        {
            var openFolder = new CommonOpenFileDialog();
            //openFolder.AllowNonFileSystemItems = true;
            //openFolder.Multiselect = true;
            openFolder.IsFolderPicker = true;
            openFolder.Title = "Select Receipt Saving Location";

            if (openFolder.ShowDialog() != CommonFileDialogResult.Ok)
            {
                MessageBox.Show("No Folder selected", "Warning");
                return;
            }
            FileManagement.saveNewSavingPathtoDatFile(openFolder.FileName);
            FileManagement.ReceiptSavingPath = openFolder.FileName;
            receiptSavingLocationTextbox.Text = FileManagement.ReceiptSavingPath;
        }

        private void change_PassButton_Click(object sender, RoutedEventArgs e)
        {
            if(usernameCombo.SelectedIndex != -1 && currentPassBox.Password != "" && newPasswordBox.Password != "" && confirmpasswordBox.Password != "" )
            {
                if(Login_TableData.verifyLogin(usernameCombo.Text, currentPassBox.Password) != true)
                {
                    MessageBox.Show("Password is not matched with the username.", "Warning");
                    return;
                }

                if (newPasswordBox.Password == confirmpasswordBox.Password)
                {
                    Login_TableData.changePassword(usernameCombo.Text, newPasswordBox.Password);
                    MessageBox.Show("Password changed successfully.", "Change Passoword");
                    currentPassBox.Clear();
                    newPasswordBox.Clear();
                    confirmpasswordBox.Clear();
                }
                else
                    MessageBox.Show("New Password and Confirm Password doesn't match.", "Warning");
            }
            else
            {
                MessageBox.Show("Can not change password because some fields are empty.", "Warning");
                return;
            }
        }
    }
}
