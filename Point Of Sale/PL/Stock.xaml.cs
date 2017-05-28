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
using Point_Of_Sale.BL;

namespace Point_Of_Sale.PL
{
    /// <summary>
    /// Interaction logic for Stock.xaml
    /// </summary>
    public partial class Stock : Window
    {
        Product product;
        List<Product> listProduct = new List<Product>();
        List<Product_Supplier> listProduct_Supplier = new List<Product_Supplier>();
        int quan = 0, unitP = 0, SellingP = 0;
        double grandTotalDouble = 0;
        double totalPaymentDouble = 0;
        double paymentDueDouble = 0;
        int totalQuantityInt = 0;
        int serial = 1;
        public Stock()
        {
            InitializeComponent();
            DB.resetConnString();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            date.Text = DateTime.Now.ToString();
            paymentDate.Text = DateTime.Now.ToString("dd/MM/yyyy");

            color.AddHandler(System.Windows.Controls.Primitives.TextBoxBase.TextChangedEvent,
                      new TextChangedEventHandler(Color_ComboBox_TextChanged));

            supplier.AddHandler(System.Windows.Controls.Primitives.TextBoxBase.TextChangedEvent,
                      new TextChangedEventHandler(Supplier_Combobox_TextChanged));

            productType.AddHandler(System.Windows.Controls.Primitives.TextBoxBase.TextChangedEvent,
                      new TextChangedEventHandler(ProductType_ComboBox_TextChanged));

            productModel.AddHandler(System.Windows.Controls.Primitives.TextBoxBase.TextChangedEvent,
                                  new TextChangedEventHandler(ProductModel_ComboBox_TextChanged));

            try
            {
                productType.ItemsSource = ProductTableData.getAllProductTypes();
                color.ItemsSource = FileManagement.getAllColor();
                supplier.ItemsSource = SupplierTableData.getAllSupplierName();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            barcodeType.IsEnabled = false;
            referenceNo.Focus();
            addDatagridColumns();
            addDatagridPaymentColumns();
        }
        public void addDatagridPaymentColumns()
        {
            DataGridTextColumn c = new DataGridTextColumn();
            c.Header = "Payment Mode";
            c.Binding = new Binding("Mode");
            paymentDatagrid.Columns.Add(c);

            c = new DataGridTextColumn();
            c.Header = "Amount";
            c.Width = 100;
            c.Binding = new Binding("Amount");
            paymentDatagrid.Columns.Add(c);

            c = new DataGridTextColumn();
            c.Header = "Payment Date";
            c.Width = 130;
            c.Binding = new Binding("Date");
            paymentDatagrid.Columns.Add(c);

            paymentDatagrid.IsReadOnly = true;

        }
        public void addDatagridColumns()
        {
            //DataGridCheckBoxColumn checkbox = new DataGridCheckBoxColumn();
            //checkbox.Header = "d";
            //checkbox.Binding = new Binding("IsChecked");
            //dataGrid.Columns.Add(checkbox);

            DataGridTextColumn c2 = new DataGridTextColumn();
            c2.Header = "SL.";
            c2.Width = 30;
            c2.Binding = new Binding("SL");
            dataGrid.Columns.Add(c2);

            c2 = new DataGridTextColumn();
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
            c5.Width = 60;
            c5.Binding = new Binding("Quantity_Available");
            dataGrid.Columns.Add(c5);

            DataGridTextColumn c6 = new DataGridTextColumn();
            c6.Header = "Unit Price";
            c6.Width = 70;
            c6.Binding = new Binding("Unit_Price");
            dataGrid.Columns.Add(c6);

            DataGridTextColumn c7 = new DataGridTextColumn();
            c7.Header = "Selling Price";
            c7.Width = 80;
            c7.Binding = new Binding("Selling_Price");
            dataGrid.Columns.Add(c7);

            c7 = new DataGridTextColumn();
            c7.Header = "Sub Total";
            c7.Width = 70;
            c7.Binding = new Binding("SubTotal");
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
        private void Supplier_Combobox_TextChanged(object sender, TextChangedEventArgs e)
        {

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

            if (e.Key == Key.Enter && quan == listView.Items.Count && product.Unique_Barcode.StartsWith("Y"))
            {
                var uie = e.OriginalSource as UIElement;
                e.Handled = true;
                uie.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
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
                color.IsEnabled = false;
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
            grandTotalDouble = 0;
            totalQuantityInt = 0;
            grandTotal.Content = "0.0";
            totalQuantity.Content = "0";
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
            Add_New_Values add = new Add_New_Values(color);
            add.ShowDialog();
        }
        //Loaded="productType_Loaded" 
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
        private bool checkErrors()
        {
            if (supplier.SelectedIndex == -1)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Please select a \"Supplier\" to continue.", "Select Product Type", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                productType.Focus();
                return false;
            }
            else if (productType.SelectedIndex == -1)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Please select a \"Product Type\" to continue.", "Select Product Type", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                productType.Focus();
                return false;
            }
            else if (productModel.SelectedIndex == -1)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Please select a \"Product Model\" to continue.", "Select Product Type", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                productModel.Focus();
                return false;
            }
            else if (int.TryParse(quantity.Text, out quan) == false)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Invalid \"Quantity\"", "Quantity Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                quantity.Focus();
                return false;
            }
            else if (int.TryParse(unitPrice.Text, out unitP) == false)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Invalid \"Unit Price\"", "Price Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                unitPrice.Focus();
                return false;
            }
            else if (int.TryParse(sellingPrice.Text, out SellingP) == false)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Invalid \"Selling Price\"", "Price Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                sellingPrice.Focus();
                return false;
            }
            else if (quan <= 0)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("\"Quantity\" cannot be less than or equal zero", "Quantity Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                quantity.Focus();
                return false;
            }
            else if (unitP <= 0)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("\"Unit Price\" cannot be less than or equal zero", "Price Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                unitPrice.Focus();
                return false;
            }
            else if (SellingP <= 0)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("\"Selling Price\" cannot be less than or equal zero", "Price Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                sellingPrice.Focus();
                return false;
            }
            else if (uniqBarcode.IsChecked == true && listView.Items.Count != quan)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Quantity and Barcodes are not equal in number. Pres ok to exit.", "Barcode List Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                barcodeSerial.Focus();
                return false;
            }
            else if (sameBarcode.IsChecked == true && listView.Items.Count != 1)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Please add a barcode. Pres ok to exit.", "Barcode List Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                barcodeSerial.Focus();

                return false;
            }


            return true;
        }
        private void AddProduct_Button_Click(object sender, RoutedEventArgs e)
        {

            if (DateTime.TryParse(date.ToString(), out datetime) == false)//parsing date for bill
                datetime = DateTime.Now;
            //checking trial subscription 
            if (DateTime.Compare(datetime, Register.SubscriptionDateEnd) > 0)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Hey Contact to the developer.", " Subscription Error:", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (checkErrors() == false)
            {
                return;
            }

            int supplierID = SupplierTableData.getIDBySupplier(supplier.Text);

            Product pro = new Product { ID = product.ID, Type = productType.Text, Model = productModel.Text, Quantity_Available = quan, Unit_Price = unitP, Selling_Price = SellingP, Unique_Barcode = product.Unique_Barcode, Date_Updated = datetime };

            Product_Supplier pro_supp = new Product_Supplier { Product_ID = product.ID, Quantity = quan, Ref_Number = referenceNo.Text, Supplier_ID = supplierID, Date = datetime };

            foreach (ListViewItems obj in listView.Items)
            {
                Barcode b = new Barcode { Product_ID = pro.ID, Barcode_Serial = obj.IMEI, Color = obj.Color, Date = datetime };

                pro.Barcodes.Add(b);
            }

            grandTotalDouble += (int)pro.Unit_Price * quan;///calculation total amount
            grandTotal.Content = grandTotalDouble;

            paymentDueDouble += (int)pro.Unit_Price * quan;
            paymentDue.Content = paymentDueDouble;


            totalQuantityInt +=  quan;///calculation total amount
            totalQuantity.Content = totalQuantityInt;

            DataGridItems DGItems = new DataGridItems(serial++, pro);
            DGItems.SubTotal = (int)pro.Unit_Price * quan;
            dataGrid.Items.Add(DGItems);
            listProduct.Add(pro);
            listProduct_Supplier.Add(pro_supp);

            productModel.SelectedIndex = -1;
            productType.SelectedIndex = -1;
            quantity.Clear();
            unitPrice.Clear();
            sellingPrice.Clear();
            listView.Items.Clear();
            description.Clear();
            color.SelectedIndex = -1;
            uniqBarcode.IsChecked = true;
            productType.Focus();
            supplier.IsEnabled = false;
            addSupplier_Button.IsEnabled = false;
        }

        private void deleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedIndex != -1)
            {
                Product p = listProduct.ElementAt(dataGrid.SelectedIndex);

                grandTotalDouble -= (int)p.Unit_Price * (int)p.Quantity_Available;
                totalQuantityInt -= (int)p.Quantity_Available;
                grandTotal.Content = grandTotalDouble;
                totalQuantity.Content = totalQuantityInt;
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
            int count = 0;

            try
            {
                foreach (Product obj in listProduct)
                {
                    ProductTableData.updateProduct(obj.ID, (int)obj.Quantity_Available, (int)obj.Unit_Price, (int)obj.Selling_Price, obj.Barcodes.ToArray(), obj.Unique_Barcode, DateTime.Now);

                    Product_SupplierTableData.addNewProduct_Supplier(listProduct_Supplier[count++]); /// inserting new row in product supplier

                    dataGrid.Items.RemoveAt(0);
                    //Console.WriteLine(dataGrid.Items.Count);

                }
                listProduct.Clear();
                listProduct_Supplier.Clear();
                product = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void clearDatagrid_Click(object sender, RoutedEventArgs e)
        {
            dataGrid.Items.Clear();
            listProduct.Clear();

            grandTotalDouble = 0;
            totalQuantityInt = 0;
            grandTotal.Content = "0.0";
            totalQuantity.Content = "0";
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

        private void addSupplier_Button_Click(object sender, RoutedEventArgs e)
        {
            Supplier_Add s = new Supplier_Add(supplier);
            s.ShowDialog();
        }

        private void dataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                List<DataGridItems> it = dataGrid.SelectedItems.Cast<DataGridItems>().ToList();

                ///single row delete with keyboard delete button
                if (it.Count != dataGrid.Items.Count)
                    foreach (DataGridItems item in it)
                    {
                        grandTotalDouble -= (int)item.Unit_Price * (int)item.Quantity_Available;
                        totalQuantityInt -= (int)item.Quantity_Available;
                        grandTotal.Content = grandTotalDouble;
                        totalQuantity.Content = totalQuantityInt;
                        listProduct.Remove(item);
                        dataGrid.Items.Remove(item);
                    }
                else ///all row delete with keyboard delete button
                {
                    dataGrid.Items.Clear();
                    listProduct.Clear();
                    supplier.IsEnabled = true;
                    addSupplier_Button.IsEnabled = true;
                    grandTotalDouble = 0;
                    totalQuantityInt = 0;
                    grandTotal.Content = 0;
                    totalQuantity.Content = 0;
                    referenceNo.Focus();
                }
            }
        }

        private void Datagird_SelectAll_Row(object sender, ExecutedRoutedEventArgs e)
        {
            if(dataGrid.Items.Count > 0)
                dataGrid.SelectAll();
        }

        private void listView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                List<ListViewItems> it = listView.SelectedItems.Cast<ListViewItems>().ToList();

                foreach (ListViewItems item in it)
                {
                    listView.Items.Remove(item);
                }

                sort();
            }
        }

        private void Listview_SelectAll_Row(object sender, ExecutedRoutedEventArgs e)
        {
            if(listView.Items.Count > 0)
                listView.SelectAll();
        }

        private void addPayment_Click(object sender, RoutedEventArgs e)
        {
            double paymentAmount = 0;
            string mode = "";
            if (paymentMode.SelectedIndex == 0)
                mode = "CA";
            else if (paymentMode.SelectedIndex == 1)
                mode = "CQ";
            else if (paymentMode.SelectedIndex == 2)
                mode = "D";
            else if (paymentMode.SelectedIndex == 3)
                mode = "C";

            if (payment.Text != "" && double.TryParse(payment.Text, out paymentAmount))
            {
                totalPaymentDouble += paymentAmount;
                totalPayment.Content = totalPaymentDouble;

                paymentDueDouble = grandTotalDouble - totalPaymentDouble;
                paymentDue.Content = paymentDueDouble;
                paymentDatagrid.Items.Add(new { Mode = paymentMode.Text, Amount = paymentAmount, Date = paymentDate });

                payment.Text = "";
                paymentDatagrid.SelectedIndex = paymentDatagrid.Items.Count - 1;
                paymentDate.Text = datetime.ToString("dd/MM/yyyy");
                paymentMode.Focus();
                
            }
            else
            {
                MessageBox.Show("error in payment section");
            }
        }

        private void Payment_Grid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var uie = e.OriginalSource as UIElement;
            Button button = new Button();

            //int quan = 0;
            //int.TryParse(quantity.Text, out quan);


            try { button = e.Source as Button; } catch { }

            if (e.Key == Key.Enter)
            {
                if (button != null && button.Name == "addPayment")
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

        private void removePayment_Click(object sender, RoutedEventArgs e)
        {
            if (paymentDatagrid.SelectedIndex == -1)
                return;

            List<object> items = paymentDatagrid.SelectedItems.Cast<object>().ToList();

            foreach (object item in items)
            {
                paymentDatagrid.Items.Remove(item);
            }

            if (paymentDatagrid.Items.Count <= 0)
            {
                removePayment.IsEnabled = false;
                updatePayment.IsEnabled = false;
                resetPayment.IsEnabled = false;
            }
        }

        private void Payment_DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Delete)
            {
                List<object> it = dataGrid.SelectedItems.Cast<object>().ToList();

                ///single row delete with keyboard delete button
                if (it.Count != dataGrid.Items.Count)
                    foreach (object item in it)
                    {
                        //grandTotalDouble -= (int)item.Unit_Price * (int)item.Quantity_Available;
                        //totalQuantityInt -= (int)item.Quantity_Available;
                        //grandTotal.Content = grandTotalDouble;
                        //totalQuantity.Content = totalQuantityInt;
                        //listProduct.Remove(item);
                        //dataGrid.Items.Remove(item);
                    }
                else ///all row delete with keyboard delete button
                {
                    //dataGrid.Items.Clear();
                    //listProduct.Clear();
                    //supplier.IsEnabled = true;
                    //addSupplier_Button.IsEnabled = true;
                    //grandTotalDouble = 0;
                    //totalQuantityInt = 0;
                    //grandTotal.Content = 0;
                    //totalQuantity.Content = 0;
                    //referenceNo.Focus();
                }
            }
        }

        private void paymentDatagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (paymentDatagrid.Items.Count > 0)
            {
                removePayment.IsEnabled = true;
                updatePayment.IsEnabled = true;
                resetPayment.IsEnabled = true;
            }
        }

        private void updatePayment_Click(object sender, RoutedEventArgs e)
        {

        }

        private void dropdownOpen_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            ComboBox cc = sender as ComboBox;
            if (cc.Items.Count > 0) /// if combobox items contains 
            {
                cc.IsDropDownOpen = true;
            }


        }
        private void Info_Grid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var uie = e.OriginalSource as UIElement;
            ComboBox com = new ComboBox();
            TextBox tex = new TextBox();
            //Button button = new Button();

            //int quan = 0;
            //int.TryParse(quantity.Text, out quan);

            try { com = e.Source as ComboBox; } catch { }

            try { tex = e.Source as TextBox; } catch { }

            //try { button = e.Source as Button; } catch { }

            if (e.Key == Key.Enter)
            {
                ///move focus should not work for barcodeSerial 
                //if (com != null )///&& com.Text.Trim() != ""
                //{
                //    //com.IsDropDownOpen = true;
                //}
                if (tex != null && tex.Name == "barcodeSerial")
                {
                    //do nothing
                }
                else if (tex != null && tex.Name == "description")
                {
                    AddProduct_Button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    referenceNo.Focus();
                }
                else
                {
                    e.Handled = true;
                    uie.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                }
            }
        }
    }

    class ListViewItems
    {
        public int SL { get; set; }
        public string IMEI { get; set; }
        public string Color { get; set; }
        public string Date { get; set; }

        public ListViewItems(int sl, string imei, string color)
        {
            SL = sl;
            IMEI = imei;
            Color = color;
        }
        public ListViewItems(int sl, string imei, string color, string date)
        {
            SL = sl;
            IMEI = imei;
            Color = color;
            Date = date;
        }

    }
    class DataGridItems : Product
    {
        public int SL { get; set; }
        public int SubTotal { get; set; }
        public DataGridItems(int sl, Product product)
        {
            SL = sl;
            ID = product.ID;
            Type = product.Type;
            Model = product.Model;
            Quantity_Available = product.Quantity_Available;
            Unit_Price = product.Unit_Price;
            Selling_Price = product.Selling_Price;
        }


    }
}
