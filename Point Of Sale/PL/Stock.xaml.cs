using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Point_Of_Sale.DAL;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections;
//using Xceed.Wpf.Toolkit;

namespace Point_Of_Sale.PL
{
    /// <summary>
    /// Interaction logic for Stock.xaml
    /// </summary>
    public partial class Stock : Window
    {
        Product product;
        List<Product> listProduct = new List<Product>();
        int totalAmountint = 0;
        public Stock()
        {
            InitializeComponent();
            DB.resetConnString();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            date.Text = DateTime.Now.ToString();

            color.AddHandler(System.Windows.Controls.Primitives.TextBoxBase.TextChangedEvent,
                      new TextChangedEventHandler(Color_ComboBox_TextChanged));

            productType.AddHandler(System.Windows.Controls.Primitives.TextBoxBase.TextChangedEvent,
                      new TextChangedEventHandler(ProductType_ComboBox_TextChanged));

            productModel.AddHandler(System.Windows.Controls.Primitives.TextBoxBase.TextChangedEvent,
                                  new TextChangedEventHandler(ProductModel_ComboBox_TextChanged));

            try
            {
                productType.ItemsSource = FileManagement.getAllProductTypes();
                color.ItemsSource = FileManagement.getAllColor();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
            barcodeType.IsEnabled = false;
            
            addDatagridColumns();
        }
        public void addDatagridColumns()
        {
            DataGridTextColumn c2 = new DataGridTextColumn();
            c2.Header = "Product ID";
            c2.Width = 70;
            c2.Binding = new Binding("ID");
            dataGrid.Columns.Add(c2);

            DataGridTextColumn c3 = new DataGridTextColumn();
            c3.Header = "Product Type";
            c3.Width = 150;
            c3.Binding = new Binding("Type");
            dataGrid.Columns.Add(c3);

            DataGridTextColumn c4 = new DataGridTextColumn();
            c4.Header = "Product Model";
            c4.Width = 150;
            c4.Binding = new Binding("Model");
            dataGrid.Columns.Add(c4);

            DataGridTextColumn c5 = new DataGridTextColumn();
            c5.Header = "Quantity";
            c5.Width = 70;
            c5.Binding = new Binding("Quantity_Available");
            dataGrid.Columns.Add(c5);

            DataGridTextColumn c6 = new DataGridTextColumn();
            c6.Header = "Unit Price";
            c6.Width = 80;
            c6.Binding = new Binding("Unit_Price");
            dataGrid.Columns.Add(c6);

            DataGridTextColumn c7 = new DataGridTextColumn();
            c7.Header = "Selling Price";
            c7.Width = 80;
            c7.Binding = new Binding("Selling_Price");
            dataGrid.Columns.Add(c7);

            dataGrid.IsReadOnly = true;

        }
        private void ProductModel_ComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (productModel.SelectedIndex != -1)
            {
                product = ProductTableData.getProductByModel(productModel.Text);

                unitPrice.Text = product.Unit_Price.ToString();
                sellingPrice.Text = product.Selling_Price.ToString();
                quantityAvailable.Content = product.Quantity_Available;
                description.Text = product.Des;

                if (product.Unique_Barcode.Contains("NY"))
                    sameBarcode.IsChecked = true;
                else if (product.Unique_Barcode.Contains("Y"))
                    uniqBarcode.IsChecked = true;
                else if (product.Unique_Barcode.Contains("N"))
                    noBarcode.IsChecked = true;
            }
            else
            {
                unitPrice.Text = "";
                sellingPrice.Text = "";
                quantityAvailable.Content = 0;
                quantity.Text = "";
                description.Text = "";
            }
        }

        private void ProductType_ComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            listView.Items.Clear();
            listView.IsEnabled = true;
            clearAll.IsEnabled = true;
            refresh_Button.IsEnabled = true;
            productModel.ItemsSource = ProductTableData.getAllTypeMachedModels(productType.Text);
        }

        private void Color_ComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            List<ListViewItems> items = listView.SelectedItems.Cast<ListViewItems>().ToList();

            ComboBox combobox = sender as ComboBox;
            foreach (ListViewItems item in items)
            {
                Console.WriteLine("{0} {1} {2}", item.SL, item.IMEI, item.Color);
            }
            if (combobox.SelectedIndex != -1 && listView.SelectedItems.Count > 0)
            {
                Console.WriteLine(listView.SelectedItems.Count);
                Console.WriteLine(combobox.SelectedIndex);

                for (int i = 0; i < listView.SelectedItems.Count; i++)
                {
                    ListViewItems item = items[i];
                    item.Color = color.Items.GetItemAt(color.SelectedIndex).ToString();

                    Console.WriteLine(item.Color);
                    listView.Items.Refresh();
                }

            }

        }
        private bool IsAlreadyExists(string barcode)
        {
            foreach (ListViewItems item in listView.Items)
            {
                if (item.IMEI == barcode)
                    return true;
            }

            return false;
        }
        
        private void barcodeSerial_KeyDown(object sender, KeyEventArgs e)
        {
            int quan = 0;
            if (e.Key == Key.Enter && barcodeSerial.Text != "")
            {
                if (IsAlreadyExists(barcodeSerial.Text) == true)
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show("The barcode is already in the list. Pres ok to exit.", "Duplicate Barcode", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                    barcodeSerial.Clear();
                    return;
                }
                else if (ProductTableData.hasBarcodeinDB(barcodeSerial.Text) == true)
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show("The barcode is already in the Database. Pres ok to exit.", "Duplicate Barcode in DB", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                    barcodeSerial.Clear();
                    return;
                }
                else if (sameBarcode.IsChecked == true && listView.Items.Count >= 1)
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show("You can add only one barcode. Pres ok to exit.", "Barcode", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                    barcodeSerial.Clear();
                    return;
                }
                else if (uniqBarcode.IsChecked == true && int.TryParse(quantity.Text, out quan) == true && listView.Items.Count > quan - 1)
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show("You cannot add barcode more than quantity. Press ok to exit.", "Barcode", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                    barcodeSerial.Clear();
                    return;
                }
                listView.Items.Add(new ListViewItems(listView.Items.Count + 1, barcodeSerial.Text, "NONE"));
                listView.SelectedIndex = listView.Items.Count - 1;
                listView.ScrollIntoView(listView.SelectedItem);
                barcodeSerial.Clear();
            }
        }
        private void uniqBarcode_Checked(object sender, RoutedEventArgs e)
        {
            barcodeSerial.IsEnabled = true;
            color.IsEnabled = true;
            Add_Color.IsEnabled = true;
            barcodeSerial.Background = new SolidColorBrush(Colors.White);

        }

        private void uniqBarcode_Unchecked(object sender, RoutedEventArgs e)
        {

        }
        private void noBarcode_Checked(object sender, RoutedEventArgs e)
        {
            barcodeSerial.IsEnabled = false;
            color.IsEnabled = false;
            Add_Color.IsEnabled = false;
            listView.IsEnabled = false; 
            barcodeSerial.Background = new SolidColorBrush(Colors.LightBlue);
            //color.Background = new SolidColorBrush(Colors.LightBlue);
        }

        private void noBarcode_Unchecked(object sender, RoutedEventArgs e)
        {
            barcodeSerial.IsEnabled = true;
            color.IsEnabled = true;
            Add_Color.IsEnabled = true;
            listView.IsEnabled = true;
            barcodeSerial.Background = new SolidColorBrush(Colors.White);
            //color.Background = new SolidColorBrush(Colors.White);
        }
        private void sameBarcode_Checked(object sender, RoutedEventArgs e)
        {
            Product pro = ProductTableData.getProductByModel(productModel.Text);

            Barcode[] barcodeList = ProductTableData.getBarcodesByPID(pro.ID);
            listView.Items.Clear();
            if (barcodeList.Count() == 1)
            {
                listView.Items.Add(new ListViewItems(1, barcodeList[0].Barcode_Serial, barcodeList[0].Color));
                barcodeSerial.IsEnabled = false;
                barcodeSerial.Background = new SolidColorBrush(Colors.LightBlue);
                listView.IsEnabled = false;
                clearAll.IsEnabled = false;
                Add_Color.IsEnabled = false;
            }
        }
        private void sameBarcode_Unchecked(object sender, RoutedEventArgs e)
        {
            if (listView.Items.IsEmpty == false)
            {
                listView.Items.Clear();
                barcodeSerial.IsEnabled = true;
                barcodeSerial.Background = new SolidColorBrush(Colors.White);
                listView.IsEnabled = true;
                clearAll.IsEnabled = true;
                Add_Color.IsEnabled = true;
            }
        }
        private void clearAll_Click(object sender, RoutedEventArgs e)
        {
            listView.Items.Clear();
            totalAmountint = 0;
            totalAmount.Content = "0.0";
        }

        private void deleteSelected_Click(object sender, RoutedEventArgs e)
        {
            List<ListViewItems> it = listView.SelectedItems.Cast<ListViewItems>().ToList();

            foreach (ListViewItems item in it)
            {
                listView.Items.Remove(item);
            }

            sort();


        }

        private void sort()
        {
            try
            {
                //Console.WriteLine(listView.Items.Count);
                for (int i = 0; i < listView.Items.Count; i++)
                {
                    ListViewItems item = (ListViewItems)listView.Items[i];
                    //Console.WriteLine("selected index " + seletedindex + " IMEI " + item.IMEI);

                    //Console.WriteLine("OLD  {0} {1} {2}", item.SL, item.IMEI, item.Color);
                    item.SL = i + 1;
                    //Console.WriteLine("NEW  {0} {1} {2}", item.SL, item.IMEI, item.Color);
                    listView.Items[i] = item;

                    listView.Items.Refresh();
                }

            }
            catch
            {

            }
        }

        private void refresh_Button_Click(object sender, RoutedEventArgs e)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listView.Items);
            view.SortDescriptions.Add(new System.ComponentModel.SortDescription("IMEI", System.ComponentModel.ListSortDirection.Ascending));
            sort();
        }


        private void addProdctType_Button_Click(object sender, RoutedEventArgs e)
        {
            Product_Type_Add add = new Product_Type_Add(true, productType, productModel);//enabling productType textfield
            add.ShowDialog();
        }

        private void addProductModel_Button_Click(object sender, RoutedEventArgs e)
        {
            Product_Type_Add add = new Product_Type_Add(false, productType, productModel); //disabling productType textfield
            add.ShowDialog();
        }

        private void Add_Color_Click(object sender, RoutedEventArgs e)
        {
            Color_Add add = new Color_Add(color);
            add.ShowDialog();
        }

        private void productType_Loaded(object sender, RoutedEventArgs e)
        {
            var textBox = (productType.Template.FindName("PART_EditableTextBox",
                       productType) as TextBox);
            if (textBox != null)
            {
                textBox.Focus();
                textBox.SelectionStart = textBox.Text.Length;
            }
        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            color.SelectedIndex = -1;
        }
        private DateTime datetime;
        private void AddProduct_Button_Click(object sender, RoutedEventArgs e)
        {
            int quan = 0, unitP = 0, SellingP = 0;
            
            if (DateTime.TryParse(date.ToString(), out datetime) == false)//parsing date for bill
                datetime = DateTime.Now;
            //checking trial subscription 
            if (DateTime.Compare(datetime, DB.subscriptionDatetime) > 0)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Hey Contact to the developer.", " Subscription Error:", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (productType.SelectedIndex != -1 && productModel.SelectedIndex != -1 && int.TryParse(quantity.Text, out quan) != false && int.TryParse(unitPrice.Text, out unitP) != false && int.TryParse(sellingPrice.Text, out SellingP) != false && quan >= 0 && unitP >= 0 && SellingP >= 0)
            {
                if (uniqBarcode.IsChecked == true && listView.Items.Count != quan)
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show("Quantity and Barcodes are not equal in number. Pres ok to exit.", "Barcode List Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                    return;
                }
                else if (sameBarcode.IsChecked == true && listView.Items.Count != 1)
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show("Please add one barcode. Pres ok to exit.", "Barcode List Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                    return;
                }
                else
                {
                    Product pro = new Product { ID = product.ID, Type = productType.Text, Model = productModel.Text, Quantity_Available = quan, Unit_Price = unitP, Selling_Price = SellingP,Unique_Barcode = product.Unique_Barcode, Date_Updated = datetime };
                    
                    foreach (ListViewItems obj in listView.Items)
                    {
                        Barcode b = new Barcode {Product_ID = pro.ID, Barcode_Serial = obj.IMEI, Color = obj.Color,Date = datetime };
                        
                        pro.Barcodes.Add(b);
                    }

                    totalAmountint += (int)pro.Unit_Price * quan;//calculation total amount
                    totalAmount.Content = totalAmountint;

                    dataGrid.Items.Add(pro);
                    listProduct.Add(pro);

                    productModel.SelectedIndex = -1;
                    productType.SelectedIndex = -1;
                    quantity.Clear();
                    unitPrice.Clear();
                    sellingPrice.Clear();
                    listView.Items.Clear();
                    description.Clear();
                    color.SelectedIndex = -1;
                    uniqBarcode.IsChecked = true;
                }
            }
            else
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("No blank field is allowed. Pres ok to exit.", "Empty Field", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                return;
            }
        }
        private void deleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedIndex != -1)
            {
                Product p = listProduct.ElementAt(dataGrid.SelectedIndex);

                totalAmountint -= (int)p.Unit_Price * (int)p.Quantity_Available;
                totalAmount.Content = totalAmountint;
                listProduct.RemoveAt(dataGrid.SelectedIndex);
                dataGrid.Items.RemoveAt(dataGrid.SelectedIndex);                
            }
                
        }
        

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            listView.Items.Clear();
            listView.IsEnabled = false;
            clearAll.IsEnabled = false;
            refresh_Button.IsEnabled = false;
            List<Product> items = dataGrid.SelectedItems.Cast<Product>().ToList();            
            
            foreach (var obj in items)
                foreach (var ob in obj.Barcodes)
                    listView.Items.Add(new ListViewItems(listView.Items.Count + 1, ob.Barcode_Serial, ob.Color));
        }

        private void OK_Button_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.Items.Count == 0)
                return;
            
            foreach(Product obj in listProduct)
            {
                try
                {

                    ProductTableData.updateProduct(obj.ID, (int)obj.Quantity_Available, (int)obj.Unit_Price, (int)obj.Selling_Price, obj.Barcodes.ToArray(), obj.Unique_Barcode, DateTime.Now);

                    dataGrid.Items.Remove(obj);
                    Console.WriteLine(dataGrid.Items.Count);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            listProduct.Clear();
            product = null;
        }

        private void clearDatagrid_Click(object sender, RoutedEventArgs e)
        {
            dataGrid.Items.Clear();
            listProduct.Clear();

            totalAmountint = 0;
            totalAmount.Content = "0.0";
        }

        private void close_Click(object sender, RoutedEventArgs e)
        {
            //DB.resetConnString();
            this.Close();
        }

        private void quantity_TextChanged(object sender, TextChangedEventArgs e)
        {
            int quan = 0;
            if (int.TryParse(quantity.Text, out quan) == true && productModel.SelectedIndex != -1)
                newQuantity.Content = quan + int.Parse(quantityAvailable.Content.ToString());
            else if (quantity.Text == "")
                newQuantity.Content = 0;
        }

        private void selectAllTextonGotFocus(object sender, RoutedEventArgs e)
        {
            TextBox t = sender as TextBox;
            t.SelectAll();
        }
    }

    class ListViewItems
    {
        public int SL { get; set; }
        public string IMEI { get; set; }
        public string Color { get; set; }

        public ListViewItems(int sl, string imei, string color)
        {
            SL = sl;
            IMEI = imei;
            Color = color;
        }


    }
    class DataGridItems
    {
        public int SL { get; set; }
        public string ProductID { get; set; }
        public string ProductType { get; set; }
        public string ProductModel { get; set; }
        public int Quantity { get; set; }
        public int UnitPrice { get; set; }
        public int SellingPrice { get; set; }

        public DataGridItems(int sl, string productID, string type, string model, int quantity, int unitPrice, int sellingPrice)
        {
            SL = sl;
            ProductID = productID;
            ProductType = type;
            ProductModel = model;
            Quantity = quantity;
            UnitPrice = unitPrice;
            SellingPrice = sellingPrice;
        }


    }
}
