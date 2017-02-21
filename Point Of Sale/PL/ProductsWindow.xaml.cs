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
using Point_Of_Sale.BL;
using Point_Of_Sale.DAL;
using System.ComponentModel;
using System.Threading;

namespace Point_Of_Sale.PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class ProductsWindow : Window
    {
        Product product;
        public ProductsWindow()
        {
            InitializeComponent();
            DB.resetConnString();
            addDatagridColumninStockHistory();
            
            Search_Product SP = new Search_Product();
            SP.addDatagridColumninSalesHistory(salesHistoryDG, true); //show unit price

            productType.AddHandler(System.Windows.Controls.Primitives.TextBoxBase.TextChangedEvent,
                      new TextChangedEventHandler(ProductType_ComboBox_TextChanged));
            productModel.AddHandler(System.Windows.Controls.Primitives.TextBoxBase.TextChangedEvent, new TextChangedEventHandler(ProductModel_ComboBox_TextChanged));

            productType.ItemsSource = ProductTableData.getAllProductTypes();
        }

        public void addDatagridColumninStockHistory()
        {
            DataGridTextColumn c1 = new DataGridTextColumn();
            c1.Header = "Product ID";
            c1.Binding = new Binding("ID");
            c1.Width = 70;
            productsDG.Columns.Add(c1);

            DataGridTextColumn c2 = new DataGridTextColumn();
            c2.Header = "Quantity Available";
            c2.Binding = new Binding("Quantity_Available");
            c2.Width = 110;
            productsDG.Columns.Add(c2);

            DataGridTextColumn c7 = new DataGridTextColumn();
            c7.Header = "Quantity Sold";
            c7.Binding = new Binding("Quantity_Sold");
            c7.Width = 100;
            productsDG.Columns.Add(c7);

            DataGridTextColumn c3 = new DataGridTextColumn();
            c3.Header = "Unit Price";
            c3.Binding = new Binding("Unit_Price");
            c3.Width = 100;
            productsDG.Columns.Add(c3);

            DataGridTextColumn c4 = new DataGridTextColumn();
            c4.Header = "Sell Price";
            c4.Binding = new Binding("Selling_Price");
            c4.Width = 70;
            productsDG.Columns.Add(c4);

            DataGridTextColumn c5 = new DataGridTextColumn();
            c5.Header = "Date Updated";
            c5.Binding = new Binding("Date_Updated");
            c5.Width = 130;
            productsDG.Columns.Add(c5);


            DataGridTextColumn c6 = new DataGridTextColumn();
            c6.Header = "Description";
            c6.Binding = new Binding("Des");
            c6.Width = 150;
            productsDG.Columns.Add(c6);

            productsDG.IsReadOnly = true;

        }



        private bool premitBarcodeinListView(string barcodeToTest)
        {
            // checks whether the barcode is sold out or not
            bool hasBarcodeinGiftTable = Gift_TableData.hasBarcodeinGiftTable(barcodeToTest);
            Console.WriteLine(hasBarcodeinGiftTable);
            if (hasBarcodeinGiftTable == true) // if proudct already sold out
                return false;
            return true;
        }
        private void ProductType_ComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            
            productModel.ItemsSource = ProductTableData.getAllTypeMachedModels(productType.Text);
            
        }
        
        
        private void ProductModel_ComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            
            fillSaleDGandProductDG();
            
        }
        
        private void fillSaleDGandProductDG()
        {
            DB.resetConnString();
            productsDG.Items.Clear();
            salesHistoryDG.Items.Clear();
            listView.Items.Clear();
            deleteSelected.IsEnabled = false;
            adjustQuantity.IsEnabled = false;

            if (productModel.SelectedIndex == -1)
                return;

            // for product and barocde datagrid
            product = ProductTableData.getProductByModel(productModel.SelectedItem.ToString());
            Barcode[] barcodes = ProductTableData.getBarcodesByPID(product.ID);

            if (product.Unique_Barcode.StartsWith("Y"))
                deleteSelected.IsEnabled = true;
            else
                adjustQuantity.IsEnabled = true;

            foreach (var item in barcodes)
            {
                if (premitBarcodeinListView(item.Barcode_Serial) == true)
                    listView.Items.Add(new ListViewItems(listView.Items.Count + 1, item.Barcode_Serial, item.Color, item.Date.ToString()));
            }

            productsDG.Items.Add(product);

            //for sales table
            List<Customer_Sale> customer_Sale_List = Customer_SaleTableData.getCustomer_SalebyMatchingProduct(product);

            foreach (var item in customer_Sale_List)
            {
                if (item.Sale == null)
                {
                    MessageBox.Show("Error showing this item. ");
                    continue;
                }
                SalesHistoryDataGrid salesHis;
                if (item.Gifts.Count == 0)
                    salesHis = new SalesHistoryDataGrid(item, "");
                else
                    salesHis = new SalesHistoryDataGrid(item, item.Gifts.First().Barcode);

                salesHistoryDG.Items.Add(salesHis);
            }
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

        private void deleteSelectedBarcode_Click(object sender, RoutedEventArgs e)
        {
            List<ListViewItems> it = listView.SelectedItems.Cast<ListViewItems>().ToList();

            MessageBoxResult res = Xceed.Wpf.Toolkit.MessageBox.Show("Are you sure?. Press 'Yes' to Confirm.", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Error, MessageBoxResult.No);

            if (res != MessageBoxResult.Yes)
                return;

            foreach (ListViewItems item in it)
            {
                try
                {
                    listView.Items.Remove(item);
                    if (ProductTableData.delete1Barcode(item.IMEI, product.ID) == true)
                        Xceed.Wpf.Toolkit.MessageBox.Show("Barcode deleted successfully. Press ok to exit.", "Exit");
                }
                catch (Exception ex)
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, "Error");
                }

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

        private void updatePrice_Click(object sender, RoutedEventArgs e)
        {
            if (productsDG.Items.Count != 0)
            {
                Update_Price up = new Update_Price(product);
                up.ShowDialog();
            }
        }

        private void deleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (productsDG.Items.Count == 0)
            {
                return;
            }
            MessageBoxResult res = Xceed.Wpf.Toolkit.MessageBox.Show("Are you sure?. Press 'Yes' to Confirm.", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Error, MessageBoxResult.No);

            if (res != MessageBoxResult.Yes)
                return;

            try
            {
                if (ProductTableData.deleteProduct(product.ID) == true)
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show("Product deleted form database");
                    productType.ItemsSource = ProductTableData.getAllProductTypes();
                    productModel.SelectedIndex = -1;
                    productType.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message);
            }
        }

        private void delete_Selected_Sales_Click(object sender, RoutedEventArgs e)
        {
            if (salesHistoryDG.SelectedIndex == -1)
                return;

            MessageBoxResult res = Xceed.Wpf.Toolkit.MessageBox.Show("Are you sure?. Press 'Yes' to Confirm.", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Error, MessageBoxResult.No);

            if (res != MessageBoxResult.Yes)
                return;

            bool flag = false;
            List<SalesHistoryDataGrid> it = salesHistoryDG.SelectedItems.Cast<SalesHistoryDataGrid>().ToList();

            try
            {
                foreach (SalesHistoryDataGrid item in it)
                {
                    if (Customer_SaleTableData.deleteCustomer_Sale(item.Customer_Sale_ID, item.Quantity) == true)
                        flag = true;

                    salesHistoryDG.Items.Remove(item);
                }
            }
            catch (Exception ex)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, "Error");
                flag = false;
            }

            if (flag == true)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Sale deleted successfully. Press ok to exit.", "Exit");
                fillSaleDGandProductDG();
            }

        }

        

        private void adjustQuantity_Click(object sender, RoutedEventArgs e)
        {
            
            Add_New_Values add = new Add_New_Values(product);
            add.Title = "Quantity Adjust";
            add.labelName.Content = "Adjusted Quantity";
            
            add.ShowDialog();
        }
        

    }
}
