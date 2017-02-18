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

namespace Point_Of_Sale.PL
{
    /// <summary>
    /// Interaction logic for Product_Type_Add.xaml
    /// </summary>
    public partial class Product_Type_Add : Window
    {
        private ComboBox productType;
        private ComboBox productModel;
        private int unitPriceInt;
        private int sellingPriceInt;
        private bool isTextBox;
        TextBox newProductTypeTextBox = new TextBox();
        ComboBox newProductTypeComboBox = new ComboBox();

        //<TextBox x:Name="newProductType" HorizontalAlignment="Left" Height="23" Margin="115,9,0,0" TextWrapping="Wrap" VerticalAlignment="Top" Width="177"  IsEnabledChanged="productType_IsEnabledChanged" FontSize="13"/>
        //private bool v;

        public Product_Type_Add()
        {
            InitializeComponent();
        }
       

        public Product_Type_Add(bool isTextBox, ComboBox productType, ComboBox productModel) : this()
        {
            InitializeComponent();
            //newProductType.IsEnabled = isEnabled;
            this.productModel = productModel;
            this.productType = productType;
            this.isTextBox = isTextBox;

            if(isTextBox == true)
            {
                
                newProductTypeTextBox.HorizontalAlignment = HorizontalAlignment.Left;
                newProductTypeTextBox.VerticalAlignment = VerticalAlignment.Top;
                newProductTypeTextBox.Height = 23;
                newProductTypeTextBox.Width = 177;
                newProductTypeTextBox.FontSize = 13;
                newProductTypeTextBox.Margin = new Thickness(115, 9, 0, 0);
                newProductTypeTextBox.TextWrapping = TextWrapping.Wrap;
                mainGrid.Children.Add(newProductTypeTextBox);
                newProductTypeTextBox.CharacterCasing = CharacterCasing.Upper;
                newProductTypeTextBox.Focus();
            }
            else
            {
                newProductModel.Focus();
                newProductTypeComboBox.HorizontalAlignment = HorizontalAlignment.Left;
                newProductTypeComboBox.VerticalAlignment = VerticalAlignment.Top;
                newProductTypeComboBox.Height = 23;
                newProductTypeComboBox.Width = 177;
                newProductTypeComboBox.FontSize = 13;
                newProductTypeComboBox.Margin = new Thickness(115, 9, 0, 0);
                newProductTypeComboBox.IsEditable = true;
                newProductTypeComboBox.IsTextSearchEnabled = true;
                newProductTypeComboBox.MaxDropDownHeight = 150;
                newProductTypeComboBox.Loaded += NewProductType_Loaded;
                newProductTypeComboBox.ItemsSource = productType.ItemsSource;
                mainGrid.Children.Add(newProductTypeComboBox);
                newProductTypeComboBox.Text = productType.Text;
            }
        }

        private void NewProductType_Loaded(object sender, RoutedEventArgs e)
        {
            var textBox = (productType.Template.FindName("PART_EditableTextBox",
                       productType) as TextBox);
            if (textBox != null)
            {
                textBox.Focus();
                textBox.SelectionStart = textBox.Text.Length;
            }
        }

        //private void productType_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        //{
        //    if(newProductType.IsEnabled == false)
        //        newProductType.Background = new SolidColorBrush(Colors.LightBlue);
        //    else
        //        newProductType.Background = new SolidColorBrush(Colors.White);
        //}
        private string getBarcodeSystem()
        {
            if (uniqBarcode.IsChecked == true)
                return "Y";
            else if (sameBarcode.IsChecked == true)
                return "NY";
            else if (noBarcode.IsChecked == true)
                return "N";

            return "";
        }
        private void ok_Click(object sender, RoutedEventArgs e)
        {
            POSDataContext db = new POSDataContext(ConnectionString.connectionStringLinq);
            //string model = newProductModel.Text;
            //string type = newProductType.Text;
                        

            if (isTextBox == true && newProductModel.Text != "" && newProductTypeTextBox.Text != "" && unitPrice.Text != "" && sellingPrice.Text != "")
            {
                Console.WriteLine(ProductTableData.IsProductAlreadyExists(newProductTypeTextBox.Text, newProductModel.Text));

                if (ProductTableData.IsProductAlreadyExists(newProductTypeTextBox.Text, newProductModel.Text) == true)
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show("Product type or model is already in database. Please try again.\n\nPress ok to exit.", "Duplicate Product", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                    return;
                }

                Product product = new Product { Model = newProductModel.Text, Type = newProductTypeTextBox.Text, Quantity_Available = 0, Quantity_Sold = 0, Unit_Price = unitPriceInt, Selling_Price = sellingPriceInt, Des = "None", Date_Updated = DateTime.Now, Unique_Barcode = getBarcodeSystem() };  
                

                if (ProductTableData.addNewProduct(product) == true)
                {
                    FileManagement.setNewProductType(product.Type);
                    //FileManagement.setNewProductModel(product.Model);

                    productType.ItemsSource = ProductTableData.getAllProductTypes();
                    productModel.ItemsSource = ProductTableData.getAllTypeMachedModels(product.Type);

                    productType.SelectedItem = product.Type;
                    productModel.SelectedItem = product.Model;                    
                }
                else
                    Xceed.Wpf.Toolkit.MessageBox.Show("product is not added.", "No Intput", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                this.Close();
            }
            else if(isTextBox == false && newProductModel.Text != "" && newProductTypeComboBox.SelectedIndex != -1 && unitPrice.Text != "" && sellingPrice.Text != "")
            {
                if (ProductTableData.IsProductAlreadyExists("", newProductModel.Text) == true)
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show("Product model is already in database. Please try again.\n\nPress ok to exit.", "Duplicate Product Model", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                    return;
                }
                Product product = new Product { Model = newProductModel.Text, Type = newProductTypeComboBox.Text, Quantity_Available = 0, Quantity_Sold = 0, Unit_Price = unitPriceInt, Selling_Price = sellingPriceInt, Des = "None", Date_Updated = DateTime.Now, Unique_Barcode = getBarcodeSystem() };
                

                if (ProductTableData.addNewProduct(product) == true)
                {
                    
                    //FileManagement.setNewProductModel(product.Model);

                    productType.ItemsSource = ProductTableData.getAllProductTypes();
                    productModel.ItemsSource = ProductTableData.getAllTypeMachedModels(product.Type);

                    //productType.SelectedIndex = productType.Items.Count - 1;
                    //productModel.SelectedIndex = productModel.Items.Count - 1;
                    productType.Text = newProductTypeComboBox.Text;
                    productModel.Text = newProductModel.Text;
                }
                else
                    Xceed.Wpf.Toolkit.MessageBox.Show("product is not added.", "No Intput", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                this.Close();
            }
            else
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Some fields are empty.", "No Intput Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                //this.Close();
            }

        }
        

        private void unitPrice_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            //TextBox t = sender as TextBox;
            if (unitPrice.Text != "" && int.TryParse(unitPrice.Text, out unitPriceInt) != true)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Unit Price is invalid.", "Invalid input", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                unitPrice.Focus();
            }
        }

        private void sellingPrice_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (sellingPrice.Text != "" && int.TryParse(sellingPrice.Text, out sellingPriceInt) != true)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Selling price is invalid.", "Invalid input", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                sellingPrice.Focus();
            }
        }

        private void close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
