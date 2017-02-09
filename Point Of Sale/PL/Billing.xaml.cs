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
using System.Windows.Shapes;
using Point_Of_Sale.DAL;
using iTextSharp.text.pdf;
using System.IO;
using System.Diagnostics;

namespace Point_Of_Sale.PL
{
    /// <summary>
    /// Interaction logic for Billing.xaml
    /// </summary>
    public partial class Billing : Window
    {
        DataGridItemsBilling dataGridItemsBilling;
        Product product;
        Customer customer;
        List<Customer_Sale> listCustomerSale = new List<Customer_Sale>();
        List<Free_Product> listFree_Product = new List<Free_Product>();
        DateTime datetime;
        bool hasCustomerinDB = false;
        int dataGridSerial = 1; 
        float totalTaka = 0;


        public Billing()
        {
            InitializeComponent();
            DB.resetConnString();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            date.Text = DateTime.Now.ToString();
            giftGrid.IsEnabled = false;

            mobile1.Focus();

            productType.AddHandler(System.Windows.Controls.Primitives.TextBoxBase.TextChangedEvent,
                      new TextChangedEventHandler(ProductType_ComboBox_TextChanged));
            productModel.AddHandler(System.Windows.Controls.Primitives.TextBoxBase.TextChangedEvent,
                                  new TextChangedEventHandler(ProductModel_ComboBox_TextChanged));
            

            try
            {
                productType.ItemsSource = ProductTableData.getAllProductTypes();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Environment.Exit(0);
            }
            
            //checking sale receipt pdf saving locaiton 
            try
            {
                FileManagement.checkReceiptSavingLocation();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            addDatagridColumns();
        }
        public void addDatagridColumns()
        {
            DataGridTextColumn c1 = new DataGridTextColumn();
            c1.Header = "SL.";
            c1.Binding = new Binding("SL");
            c1.Width = 40;
            dataGrid.Columns.Add(c1);

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
            c4.Width = 130;
            c4.Binding = new Binding("Model");
            dataGrid.Columns.Add(c4);

            DataGridTextColumn c5 = new DataGridTextColumn();
            c5.Header = "Quantity";
            c5.Width = 80;
            c5.Binding = new Binding("Quantity");
            dataGrid.Columns.Add(c5);
            
            DataGridTextColumn c7 = new DataGridTextColumn();
            c7.Header = "Sold Price";
            c7.Width = 80;
            c7.Binding = new Binding("SoldPrice");
            dataGrid.Columns.Add(c7);

            DataGridTextColumn c8 = new DataGridTextColumn();
            c8.Header = "Discount";
            c8.Width = 70;
            c8.Binding = new Binding("Discount");
            dataGrid.Columns.Add(c8);

            DataGridTextColumn c9 = new DataGridTextColumn();
            c9.Header = "Discount Price";
            c9.Width = 100;
            c9.Binding = new Binding("DiscountPrice");
            dataGrid.Columns.Add(c9);

            dataGrid.IsReadOnly = true;

        }
        private void ProductType_ComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            

            listView.Items.Clear();
            listView.IsEnabled = true;
            //clearAll.IsEnabled = true;
            refresh_Button.IsEnabled = true;
            try
            {
                productModel.ItemsSource = ProductTableData.getTypeMachedModelsQNonZero(productType.Text);

            }
            catch (Exception ex)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, "Error getting product models", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
            quantity.Text = "1";
            sellingPrice.Clear();
            quantityAvailable.Content = 0;
            SLNumber.Clear();
            giftCode.Clear();
            discountPrice.Clear();
            barcodeSerial.Items.Clear();


        }

        private bool premitBarcodeinCombobox(string barcodeToTest)
        {
            // checks whether the barcode is sold out or not
            bool hasBarcodeinGiftTable = Gift_TableData.hasBarcodeinGiftTable(barcodeToTest);
            //Console.WriteLine(hasBarcodeinGiftTable);
            if (hasBarcodeinGiftTable == true) // if proudct already sold out
                return false;

            if (dataGrid.Items.Count == 0)
                return true;
            if (product.Unique_Barcode.StartsWith("NY"))
                return true;
            if (product.Unique_Barcode.StartsWith("N"))
                return true;

            foreach (var obj in dataGrid.Items)
            {
                DataGridItemsBilling dgItems = (DataGridItemsBilling)obj;
                DAL.Barcode bar = dgItems.Barcodes.First();
                if (bar.Barcode_Serial == barcodeToTest)
                    return false;                

            }
            return true;
        }
        private void ProductModel_ComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (productModel.SelectedIndex != -1)
            {
                
                try
                {
                    product = ProductTableData.getProductByModel(productModel.Text);
                    
                    if (product.Unique_Barcode.StartsWith("Y"))//productType.Text == "Samsung Mobile"
                    {
                        quantity.IsEnabled = false;
                        quantity.Text = "1";
                    }

                    else
                        quantity.IsEnabled = true;


                    quantityAvailable.Content = product.Quantity_Available;
                    //Console.WriteLine(product.Quantity_Available + " " + quantityAvailable.Content);

                    DAL.Barcode[] barcodes = ProductTableData.getBarcodesByPID(product.ID);
                    barcodeSerial.Items.Clear();
                    
                    foreach (DAL.Barcode obj in barcodes)
                    {
                        //stop adding duplicate barcode which is already added in datagrid and which is already sold out
                        if (premitBarcodeinCombobox(obj.Barcode_Serial) == true)
                            barcodeSerial.Items.Add(obj.Barcode_Serial);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }


            }
            else
            {
                giftCheckBox.IsChecked = false;
                sellingPrice.Clear();
                loadSellingPrice.IsChecked = false;
                quantityAvailable.Content = 0;
                quantity.Clear();
                SLNumber.Clear();
                giftCode.Clear();
                quantity.Text = "1";
                barcodeSerial.Items.Clear();
                discountPrice.Clear();
                listView.Items.Clear();
                loadSellingPrice.IsChecked = false;
                freeProduct.IsChecked = false;
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
            bool flag = false;
            var barcode = new DAL.Barcode();
            if (e.Key == Key.Enter && barcodeSerial.Text != "")
            {
                if (IsAlreadyExists(barcodeSerial.Text) == true)
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show("The barcode is already in the list. Pres ok to exit.", "Duplicate Barcode", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                    return;
                }
                if (int.TryParse(quantity.Text, out quan) != true || listView.Items.Count > quan - 1)
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show("You cannot add more than quantity. Pres ok to exit.", "Duplicate Barcode", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                    return;
                }
                //matches the entered barcode with the list of barcode in combobox
                foreach (string obj in barcodeSerial.Items) 
                {
                    if (obj == barcodeSerial.Text)
                    {
                        barcode = ProductTableData.getBarcode(barcodeSerial.Text);
                        flag = true; // if true then user selected form the listed barcode
                        break;
                    }
                }
                bool hasBarcodeinDB = ProductTableData.hasBarcodeinDB(barcodeSerial.Text);

                ///if user wants to load the product model by entering the barcode
                if (flag == false && hasBarcodeinDB == true)
                {
                    barcode = ProductTableData.getBarcode(barcodeSerial.Text);
                    ///if the product type is selected and the barocde is associated with another product then the it will ask to load that model in following code
                    if (productType.SelectedIndex != -1) 
                    {
                        MessageBoxResult res = Xceed.Wpf.Toolkit.MessageBox.Show("This barcode is associated with '" + barcode.Product.Model + "' Model. Do you want to load the product. This ", "Product not found", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                        if (res == MessageBoxResult.Yes)
                            selectProductbyBarcode(barcode);
                        else
                            return;
                    }
                    else
                    {
                        selectProductbyBarcode(barcode);
                    }
                }
                else if(flag == false && hasBarcodeinDB == false)
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show("The barcode you entered is not found in DB", "Product not found", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                    return;
                }

                listView.Items.Add(new ListViewItems(listView.Items.Count + 1, barcode.Barcode_Serial, barcode.Color));
                listView.SelectedIndex = listView.Items.Count - 1;
                listView.ScrollIntoView(listView.SelectedItem);
                barcodeSerial.SelectedIndex = -1;
            }
        }
        private void selectProductbyBarcode(DAL.Barcode barcode)
        {
            //productType.Text = barcode.Product.Type;
            //productModel.Text = barcode.Product.Model;
            productType.SelectedItem = barcode.Product.Type;
            productModel.SelectedItem = barcode.Product.Model;
            barcodeSerial.Text = "";
            loadSellingPrice.IsChecked = true;
        }
        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            giftGrid.IsEnabled = true;
        }

        private void checkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            giftGrid.IsEnabled = false;
            giftCode.Clear();
            discountPrice.Clear();
        }

        private void refresh_Button_Click(object sender, RoutedEventArgs e)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listView.Items);
            view.SortDescriptions.Add(new System.ComponentModel.SortDescription("IMEI", System.ComponentModel.ListSortDirection.Ascending));
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
        private void deleteSelected_Click(object sender, RoutedEventArgs e)
        {
            List<ListViewItems> it = listView.SelectedItems.Cast<ListViewItems>().ToList();

            foreach (ListViewItems item in it)
            {
                listView.Items.Remove(item);
            }

            sort();
        }

        

        private void mobile1_Loaded(object sender, RoutedEventArgs e)
        {
            var textBox = (mobile1.Template.FindName("PART_EditableTextBox",
                       mobile1) as TextBox);
            if (textBox != null)
            {
                textBox.Focus();
                textBox.SelectionStart = textBox.Text.Length;
            }
        }
        private void mobile1_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            customer = null;
            hasCustomerinDB = false;
            mobile1.SelectAll();
            name.Clear();
            address.Clear();
            mobile2.Clear();
        }
        private void mobile1_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            Int64 temp = 0;
            if (mobile1.Text == "")
                return;
            else if (Int64.TryParse(mobile1.Text, out temp) != true)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Invalid mobile number. Press ok to exit.", "Duplicate Barcode", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                return;
            }

            try
            {
                customer = CustomerTableData.getCustomersbyMatchingID(mobile1.Text).First();
            }
            catch
            {
                //customer not found in database
            }
            if (customer != null)
            {
                name.Text = customer.Name;
                address.Text = customer.Address;
                mobile2.Text = customer.Phone2.ToString();
                hasCustomerinDB = true;
                productType.Focus();
            }

        }

        private void productType_Loaded(object sender, RoutedEventArgs e)
        {
            var textBox = (productType.Template.FindName("PART_EditableTextBox",
                       productType) as TextBox);
            if (textBox != null)
                textBox.SelectionStart = textBox.Text.Length;
        }



        //private void quantity_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        //{
        //    int quan = 0;
        //    if (int.TryParse(quantity.Text, out quan) != true)
        //        Xceed.Wpf.Toolkit.MessageBox.Show("Invalid quantity. Press ok to exit.", "Invalid quantity", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
        //    else if(quan <= 0)
        //        Xceed.Wpf.Toolkit.MessageBox.Show("Product Quantity cannot be less than zero. Pres ok to exit.", "Invalid quantity", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
        //}

        private void sellingPrice_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            int price = 0;
            if (int.TryParse(sellingPrice.Text, out price) != true)
            {

                Xceed.Wpf.Toolkit.MessageBox.Show("Invalid price. Pres ok to exit.", "Invalid price", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                sellingPrice.SelectAll();
                return;
            }
            try
            {
                if (product.Unit_Price > price)
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show("You can not sell on loss. Pres ok to exit.", "Loss?", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                    return;
                }
            }
            catch
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Please select product first then enter price.", "Product Error?", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }

        }

        private bool checkErrors()
        {
            int quan = 0, price = 0, disPrice = 0;
            Int64 mobileOne = 0, mobileTwo = 0;
            if (Int64.TryParse(mobile1.Text, out mobileOne) == false)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Invalid \"Customer Mobile\".", "Invalid Mobile Number.", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                mobile1.Focus();
                return false;
            }
            else if (name.Text == "")
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Please enter \"Name\" to continue.", "Name?", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                name.Focus();
                return false;
            }            
            //else if (Int64.TryParse(mobile2.Text, out mobileTwo) == false)
            //{
            //    Xceed.Wpf.Toolkit.MessageBox.Show("Invalid \"Customer Mobile2\".", "Invalid Mobile2 ?", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            //    name.Focus();
            //    return false;
            //}
            //else if (address.Text == "")
            //{
            //    Xceed.Wpf.Toolkit.MessageBox.Show("Please enter \"Address\" to continue.", "Address?", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            //    address.Focus();
            //    return false;
            //}
            else if (productType.SelectedIndex == -1)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Please select a \"Product Type\" to continue.", "Product Type?", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                productType.Focus();
                return false;
            }
            else if (productModel.SelectedIndex == -1)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Please select a \"Product Model\" to continue.", "Product Model?", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                productModel.Focus();
                return false;
            }
            else if (int.TryParse(quantity.Text, out quan) == false || quan < 0)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Invalid product quantity.", "Product Quantity?", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                quantity.Focus();
                quantity.SelectAll();
                return false;
            }
            else if (product.Quantity_Available == 0 || product.Quantity_Available < quan)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("No product in stock of Type: " + product.Type + " of Model: " + product.Model, "Product shortage?", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                return false;
            }

            else if (int.TryParse(sellingPrice.Text, out price) == false || price < 0)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Invalid selling price.", "Selling Price?", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                sellingPrice.Focus();
                sellingPrice.SelectAll();
                return false;
            }
            else if (price < product.Unit_Price)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("you cannot sell on loss.", "Loss?", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                return false;
            }
            else if (product.Unique_Barcode.StartsWith("Y") && listView.Items.Count != quan)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Please add barcode to the list.", "Barcode?", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                return false;
            }
            else if (product.Unique_Barcode.Contains("NY") && listView.Items.Count != 1)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Please add barcode to the list.", "Barcode?", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                return false;
            }
            else if (giftCode.Text == "" || int.TryParse(discountPrice.Text, out disPrice) == false || disPrice < 0)
            {
                // no warning if product has no unique barcode
                if (product.Unique_Barcode.StartsWith("Y") == false) 
                    return true;

                MessageBoxResult res = Xceed.Wpf.Toolkit.MessageBox.Show("\"Gift Code\" or \"Discount Price\" is invalid.\n\nDo you want to continue without gift.", "Gift?", MessageBoxButton.YesNo, MessageBoxImage.Error, MessageBoxResult.No);
                if (res.Equals(MessageBoxResult.Yes))
                    return true;
                else
                    return false;
            }
            //else if(hasShortageQ() == true)
            //{
            //    Xceed.Wpf.Toolkit.MessageBox.Show("Product stock shortage for Type \"" + product.Type + "\" Model \"" + product.Model, "Quantity Shortage?", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            //    return false;
            //}

            return true;
        }
        public bool hasShortageQ()
        {
            if (dataGrid.Items.Count == 0)
                return false;

            foreach (var item in listCustomerSale)
            {
                if(product.ID == item.ID)
                {
                    Product p = ProductTableData.getProductByID(product.ID);
                    if (p.Quantity_Available < (product.ID + item.ID))                    
                        return true;
                }
            }
            return false;
        }
        private void deleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedIndex != -1)
            {
                //DB.resetConnString();
                Customer_Sale cc = listCustomerSale.ElementAt(dataGrid.SelectedIndex);
                DataGridItemsBilling abc = (DataGridItemsBilling) dataGrid.Items.GetItemAt(dataGrid.SelectedIndex);
                //Console.WriteLine(abc.Discount);
                totalTaka -= ((int) cc.Sold_Price * (int)cc.Quantity) - abc.Discount; // deduct the deleted product price from total
                totalAmount.Content = totalTaka.ToString();
                listCustomerSale.RemoveAt(dataGrid.SelectedIndex);
                dataGrid.Items.RemoveAt(dataGrid.SelectedIndex);
            }
        }
        private void clearDatagrid_Click(object sender, RoutedEventArgs e)
        {
            DB.resetConnString();
            mobile1.Focus(); // to get the customer object from the db again.
            dataGridItemsBilling = null;
            product = null;
            dataGridSerial = 1;
            dataGrid.Items.Clear();
            listView.Items.Clear();
            productType.SelectedIndex = -1;
            productModel.SelectedIndex = -1;
            barcodeSerial.SelectedIndex = -1;
            listCustomerSale.Clear();
            totalTaka = 0;
            totalAmount.Content = 0;
            loadSellingPrice.IsChecked = false;
            freeProduct.IsChecked = false;
        }

        private void OK_Button_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.Items.Count == 0)
                return;
            try
            {     
                Bill.createBill(listCustomerSale, customer,listFree_Product, datetime);

                generateBillPdf(false); // show preview false

                listCustomerSale.Clear();
                dataGridSerial = 1;
                hasCustomerinDB = false;
                dataGrid.Items.Clear();
                this.Close();
            }
            catch (Exception ex)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Billing falied.\nNote: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                this.Close();
            }

        }
        private void previewBill_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.Items.Count > 0)
            {
                generateBillPdf(true); // show preview true

                try
                {
                    Print.previewPdfFile(tempFilePathforPreview); // file saved in documents folder for temporary preview

                    File.Delete(tempFilePathforPreview);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private void printBill()
        {
            //////print
            ProcessStartInfo info = new ProcessStartInfo();
            info.Verb = "print";
            info.FileName = FileManagement.generatedReceipt;
            info.CreateNoWindow = true;
            info.WindowStyle = ProcessWindowStyle.Hidden;

            Process p = new Process();
            p.StartInfo = info;
            p.Start();

            p.WaitForInputIdle();
            System.Threading.Thread.Sleep(3000);
            if (false == p.CloseMainWindow())
                p.Kill();
        }
        string tempFilePathforPreview;
        private void generateBillPdf(bool isPreview)
        {
            int i = 0;
             
            string invoiceNo = Bill.getInvoiceNumber().ToString();
            
            PdfReader reader = new PdfReader(FileManagement.receipt);
            PdfStamper pdfStamper;
            if (isPreview == false) // generate bill to save it and print it 
            {
                FileManagement.generatedReceipt = FileManagement.ReceiptSavingPath + @"\" + invoiceNo + " " + mobile1.Text + ".pdf"; // bill generate for saved location
                pdfStamper = new PdfStamper(reader, new FileStream(FileManagement.generatedReceipt, FileMode.Create)); //FileManagement.generatedReceipt
            }
            else// generate bill for preview purpose
            {
                tempFilePathforPreview = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\" + "preview invoice" + customer.ID + ".pdf";// temporary invoice path for preview
                pdfStamper = new PdfStamper(reader, new FileStream(tempFilePathforPreview, FileMode.Create)); //FileManagement.generatedReceipt
            }
                
           
            AcroFields pdfFormFields = pdfStamper.AcroFields;
            

            pdfFormFields.SetField("InvoiceNo", invoiceNo);
            pdfFormFields.SetField("Date", date.ToString());
            pdfFormFields.SetField("Name", name.Text);
            pdfFormFields.SetField("Mobile", mobile1.Text);
            pdfFormFields.SetField("Mobile2", mobile2.Text);
            pdfFormFields.SetField("Address", address.Text);

            foreach (var obj in dataGrid.Items)
            {
                DataGridItemsBilling dg = (DataGridItemsBilling)obj;
                DAL.Barcode bar = dg.Barcodes.FirstOrDefault(); // if the product has barcode


                if (dg.Unique_Barcode.StartsWith("Y"))
                {
                    pdfFormFields.SetField("Description." + i, string.Format("Type: {0} Model: {1} B/IMEI: {2} SL: {3} Dis: {4}", dg.Type, dg.Model, bar.Barcode_Serial, dg.gift.SL, dg.gift.Discount_Price));
                }
                else if (dg.Unique_Barcode.Contains("NY"))
                {
                    pdfFormFields.SetField("Description." + i, string.Format("Type: {0} Model: {1} B/IMEI: {2}", dg.Type, dg.Model, bar.Barcode_Serial));
                }
                else
                {
                    pdfFormFields.SetField("Description." + i, string.Format("Type: {0} Model: {1}", dg.Type, dg.Model));
                }
                pdfFormFields.SetField("SL." + i, (i + 1).ToString());
                pdfFormFields.SetField("Quantity." + i, dg.Quantity.ToString());
                pdfFormFields.SetField("Rate." + i, dg.SoldPrice.ToString());

                float price = ((float)dg.SoldPrice * dg.Quantity) - dg.Discount;
                
                pdfFormFields.SetField("Taka." + i, price.ToString());
                i++;

            }
            pdfFormFields.SetField("totalTaka", totalAmount.Content.ToString());
            pdfStamper.FormFlattening = true;
            pdfStamper.Close();

        }
        private void OK_n_Open_Button_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.Items.Count <= 0)
                return;
            try
            {
                Bill.createBill(listCustomerSale, customer, listFree_Product, datetime);

                generateBillPdf(false); //// show preview false

                listCustomerSale.Clear();
                dataGridSerial = 1;
                hasCustomerinDB = false;
                dataGrid.Items.Clear();
                this.Close();
            }
            catch (Exception ex)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Billing falied.\nNote: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }

            //pdf preview problem
            try
            {
                Print.previewPdfFile(FileManagement.generatedReceipt);
            }
            catch (Exception ex)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Pdf file could not be opened.\nNote: " + ex.Message, "PDF File Opeining problem", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }
        private void OK_n_Print_Button_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.Items.Count <= 0)
                return;
            try
            {
                Bill.createBill(listCustomerSale, customer, listFree_Product, datetime);

                generateBillPdf(false); //// show preview false


                listCustomerSale.Clear();
                dataGridSerial = 1;
                hasCustomerinDB = false;
                dataGrid.Items.Clear();
                this.Close();
            }
            catch (Exception ex)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Billing falied.\nNote: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }

            try
            {
                printBill();
            }
            catch (Exception ex)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Printing falied.\nDetailed Error: " + ex.Message, "Printing Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }

        }

        private void AddProduct_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
                if (checkErrors() == false) ///check all text fields text format
                    return;

                if(dataGrid.Items.Count == 9)
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show("You can add only 9 products in one invoice or sale receipt.");
                    return;
                }
                if (DateTime.TryParse(date.ToString(), out datetime) == false)///parsing date for bill
                    datetime = DateTime.Now;
                if (DateTime.Compare(datetime.Date, DB.subscriptionDatetime) > 0)
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show("Hey Contact to the developer.", " Subscription Error:", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                if (hasCustomerinDB == false) /// if customer is not in db then creates new customer
                {
                    if (mobile2.Text == "")
                        mobile2.Text = "0";
                    customer = new Customer { ID = int.Parse(mobile1.Text), Name = name.Text, Address = address.Text, Phone2 = int.Parse(mobile2.Text) };
                    //sale.Customer = customer;
                }

                Customer_Sale cus_Sale = new Customer_Sale { Quantity = int.Parse(quantity.Text), Unit_Price = product.Unit_Price, Sold_Price = int.Parse(sellingPrice.Text) };

                product.Quantity_Sold += cus_Sale.Quantity;
                product.Quantity_Available -= cus_Sale.Quantity;
                cus_Sale.Product = product;

                dataGridItemsBilling = new DataGridItemsBilling(dataGridSerial++, cus_Sale);

                int dis = 0;
                foreach (ListViewItems obj in listView.Items)
                {
                    DAL.Barcode b = new DAL.Barcode { Product_ID = product.ID, Barcode_Serial = obj.IMEI, Color = obj.Color };

                    dataGridItemsBilling.Barcodes.Add(b);

                    if (product.Unique_Barcode.StartsWith("Y"))
                    {
                        Gift gift = new Gift { };
                        DAL.Barcode bb = ProductTableData.getBarcode(obj.IMEI);
                        gift.Barcode1 = bb; // adds gift with mobile products
                        gift.Customer_Sale = cus_Sale;
                        gift.Discount_Price = 0;

                        if (SLNumber.Text != "")
                            gift.SL = SLNumber.Text;

                        if (giftCheckBox.IsChecked == true)
                        {
                            gift.Gift_Code = giftCode.Text;

                            if (int.TryParse(discountPrice.Text, out dis))
                            {
                                gift.Discount_Price = dis;
                                dataGridItemsBilling.Discount = dis;
                            }
                            else
                                gift.Discount_Price = dis;
                        }

                        dataGridItemsBilling.gift = gift;
                    }
                }
                if (freeProduct.IsChecked == true)
                {
                    if(dataGrid.Items.Count == 0)
                    {
                        MessageBox.Show("Please add a non free product first", "Error");
                        return;
                    }
                    foreach (var cus_saleItem in listCustomerSale)
                    {
                        if (cus_saleItem.Product.Unique_Barcode.StartsWith("Y"))
                        {
                            cus_saleItem.Unit_Price += product.Unit_Price;
                            break;
                        }
                        else
                        {
                            MessageBox.Show("Please add a mobile first", "Error");
                            return;
                        }
                            
                    }
                    Free_Product freePro = new Free_Product();
                    freePro.Product_ID = product.ID;
                    listFree_Product.Add(freePro);

                    cus_Sale.Sold_Price = product.Unit_Price; //sold price changed because of free
                    dataGridItemsBilling.Discount = (int)product.Selling_Price;
                    dataGridItemsBilling.DiscountPrice = 0;

                }
                else
                {
                    totalTaka = totalTaka + ((float)cus_Sale.Sold_Price - dis) * (int)cus_Sale.Quantity; // calculating total bill
                    dataGridItemsBilling.DiscountPrice = ((float)cus_Sale.Sold_Price - dis) * (int)cus_Sale.Quantity; // price after discount
                }

                totalAmount.Content = totalTaka; // set total taka on bill screen
                listCustomerSale.Add(cus_Sale);
                dataGrid.Items.Add(dataGridItemsBilling);
                productModel.SelectedIndex = -1;
                productType.SelectedIndex = -1;
                barcodeSerial.SelectedIndex = -1;
                quantityAvailable.Content = 0;
                quantity.Text = "1";
                sellingPrice.Clear();
                listView.Items.Clear();
                SLNumber.Clear();
                discountPrice.Clear();
                barcodeSerial.Items.Clear();
                giftCode.Clear();
                loadSellingPrice.IsChecked = false;
                freeProduct.IsChecked = false;
                //product = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            listView.Items.Clear();
            listView.IsEnabled = false;
            //clearAll.IsEnabled = false;
            refresh_Button.IsEnabled = false;
            List<DataGridItemsBilling> items = dataGrid.SelectedItems.Cast<DataGridItemsBilling>().ToList();
            Console.WriteLine(items.Count);
            foreach (var obj in items)
            {                
                foreach (var ob in obj.Barcodes)
                {
                    listView.Items.Add(new ListViewItems(listView.Items.Count + 1, ob.Barcode_Serial, ob.Color));
                    Console.WriteLine(ob.Barcode_Serial);
                }
            }
                    
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            DB.resetConnString();
            this.Close();
        }

        private void loadSellingPrice_Checked(object sender, RoutedEventArgs e)
        {
            if (productModel.SelectedIndex != -1)//&& productType.SelectedIndex != -1
                sellingPrice.Text = product.Selling_Price.ToString();
        }

        private void loadSellingPrice_Unchecked(object sender, RoutedEventArgs e)
        {
                sellingPrice.Text = "";
        }

        private void selectAllonGotFocus(object sender, RoutedEventArgs e)
        {
            TextBox t = sender as TextBox;
            t.SelectAll();
        }

        
    }
    class DataGridItemsBilling : Product
    {
        public int SL { get; set; }
        public Gift gift {get;set;}
        public int Quantity { get; set; }
        public int SoldPrice { get; set; }
        public float Discount { get; set; }
        public float DiscountPrice { get; set; }

        public DataGridItemsBilling(int SL, Customer_Sale cus_sale)
        {
            this.SL = SL;
            ID = cus_sale.Product.ID;
            Type = cus_sale.Product.Type;
            Model = cus_sale.Product.Model;
            Unique_Barcode = cus_sale.Product.Unique_Barcode;
            Quantity = (int)cus_sale.Quantity;
            Selling_Price = (int)cus_sale.Product.Selling_Price;            
            //Discount = (int)gift.Discount_Price;
            SoldPrice = (int)cus_sale.Sold_Price;
            //this.gift = cus_sale.Gifts.First();
            
        }
    }
}
