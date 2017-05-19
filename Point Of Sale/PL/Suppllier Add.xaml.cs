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
using System.Collections.ObjectModel;
using System.Windows.Shapes;
using Point_Of_Sale.DAL;
using Point_Of_Sale.BL;

namespace Point_Of_Sale.PL
{
    /// <summary>
    /// Interaction logic for Product_Type_Add.xaml
    /// </summary>
    public partial class Supplier_Add : Window
    {
        private ComboBox supplier;

        public Supplier_Add()
        {
            InitializeComponent();
            companyName.Focus();
        }

        public Supplier_Add(ComboBox supplier): this()
        {
            this.supplier = supplier;
        }

        private void SelectAll_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox t;

            try
            {
                t = sender as TextBox;
                t.SelectAll();
            }
            catch
            {
                
            }
        }

        private void Grid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var uie = e.OriginalSource as UIElement;

            Button button = new Button();
            TextBox textbox = new TextBox();

            try { button = e.Source as Button; } catch { }
            try { textbox = e.Source as TextBox; } catch { }

            if (e.Key == Key.Enter)
            {
                //if (textbox != null && textbox.Name == "website")
                //{
                //    save_Button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                //    this.Close();
                //}
                if (button != null && button.Name == "save_Button")
                {
                    ///do nothing
                }
                else
                {
                    e.Handled = true;
                    uie.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                }
            }
        }
        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            if (companyName.Text.Trim() == "")
            {
                MessageBox.Show("Company name is required");
                return;
            }
            else if (contactName.Text.Trim() == "")
            {
                MessageBox.Show("Contact name is required");
                return;
            }
            else if (city.Text.Trim() == "")
            {
                MessageBox.Show("City is required");
                return;
            }

            Address address = new Address { City = city.Text, AddressLine = addressLine.Text, Country = country.Text.Trim(), Fax = fax.Text.Trim(), Postal_Code = postalCode.Text.Trim(), Phone1 = phone1.Text.Trim(), Phone2 = phone2.Text.Trim(), Website = website.Text.Trim() };

            Supplier supplier = new Supplier { Company_Name = companyName.Text.Trim(), Contact_Designation = designation.Text.Trim(), Contact_Name = contactName.Text.Trim(), Email = email.Text.Trim() };
            
            supplier.Address = address;

            if (SupplierTableData.addNewSupplier(supplier) == false)
            {
                MessageBox.Show(CustomMessage.Message + "\n\nDetailed Error: " + CustomMessage.StackTrace);
                return;
            }
            this.supplier.ItemsSource = SupplierTableData.getAllSupplierName();
            this.supplier.SelectedItem = string.Format("{0} - {1}", contactName.Text, companyName.Text);
            this.Close();

        }

        private void cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void mainGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}
