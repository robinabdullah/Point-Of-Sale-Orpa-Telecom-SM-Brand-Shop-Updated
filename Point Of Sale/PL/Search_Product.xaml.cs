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

namespace Point_Of_Sale.PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Search_Product : Window
    {
        public Search_Product()
        {
            InitializeComponent();
            //this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            barcodeimeiTextBox.Focus();
            addDatagridColumninStockHistory();
            addDatagridColumninSalesHistory(salesHistoryDG, false); // dont show unit price
            barcodeIMEIRadio.IsChecked = true;

        }

        public void addDatagridColumninStockHistory()
        {
            DataGridTextColumn c1 = new DataGridTextColumn();
            c1.Header = "Product ID";
            c1.Binding = new Binding("ID");
            c1.Width = 70;
            stockHistoryDG.Columns.Add(c1);

            DataGridTextColumn c2 = new DataGridTextColumn();
            c2.Header = "Type";
            c2.Binding = new Binding("Type");
            c2.Width = 120;
            stockHistoryDG.Columns.Add(c2);

            DataGridTextColumn c3 = new DataGridTextColumn();
            c3.Header = "Model";
            c3.Binding = new Binding("Model");
            c3.Width = 100;
            stockHistoryDG.Columns.Add(c3);

            DataGridTextColumn c4 = new DataGridTextColumn();
            c4.Header = "Sell Price";
            c4.Binding = new Binding("Selling_Price");
            c4.Width = 70;
            stockHistoryDG.Columns.Add(c4);

            DataGridTextColumn c5 = new DataGridTextColumn();
            c5.Header = "Date Updated";
            c5.Binding = new Binding("Date_Updated");
            c5.Width = 130;
            stockHistoryDG.Columns.Add(c5);


            DataGridTextColumn c6 = new DataGridTextColumn();
            c6.Header = "Description";
            c6.Binding = new Binding("Des");
            c6.Width = 150;
            stockHistoryDG.Columns.Add(c6);



            stockHistoryDG.IsReadOnly = true;

        }
        public void addDatagridColumninSalesHistory(DataGrid salesHistoryDG,bool showUnitPrice)
        {
            DataGridTextColumn c1 = new DataGridTextColumn();
            c1.Header = "Product ID";
            c1.Binding = new Binding("Product_ID");
            //c1.Width = 70;
            salesHistoryDG.Columns.Add(c1);

            DataGridTextColumn c2 = new DataGridTextColumn();
            c2.Header = "Invoice";
            c2.Binding = new Binding("Invoice_ID");
            //c2.Width = 70;
            salesHistoryDG.Columns.Add(c2);

            DataGridTextColumn c10 = new DataGridTextColumn();
            c10.Header = "Cust_Sale ID";
            c10.Binding = new Binding("Customer_Sale_ID");
            //c10.Width = 70;
            salesHistoryDG.Columns.Add(c10);

            DataGridTextColumn c9 = new DataGridTextColumn();
            c9.Header = "Quantity";
            c9.Binding = new Binding("Quantity");
            //c9.Width = 70;
            salesHistoryDG.Columns.Add(c9);

            DataGridTextColumn c = new DataGridTextColumn();
            c.Header = "Unit Price";
            c.Binding = new Binding("Unit_Price");
            if(showUnitPrice ==true) salesHistoryDG.Columns.Add(c); //if true then show unit price

            DataGridTextColumn c4 = new DataGridTextColumn();
            c4.Header = "Sold Price";
            c4.Binding = new Binding("Sold_Price");
            //c4.Width = 70;
            salesHistoryDG.Columns.Add(c4);

            DataGridTextColumn c5 = new DataGridTextColumn();
            c5.Header = "Sold Date";
            c5.Binding = new Binding("Date");
            //c5.Width = 140;
            salesHistoryDG.Columns.Add(c5);


            DataGridTextColumn c6 = new DataGridTextColumn();
            c6.Header = "Mobile1";
            c6.Binding = new Binding("Customer_ID");
            //c6.Width = 100;
            salesHistoryDG.Columns.Add(c6);

            DataGridTextColumn c3 = new DataGridTextColumn();
            c3.Header = "Mobile2";
            c3.Binding = new Binding("Phone2");
            //c3.Width = 100;
            salesHistoryDG.Columns.Add(c3);

            DataGridTextColumn c7 = new DataGridTextColumn();
            c7.Header = "Customer Name";
            c7.Binding = new Binding("Name");
            //c7.Width = 110;
            salesHistoryDG.Columns.Add(c7);

            DataGridTextColumn c8 = new DataGridTextColumn();
            c8.Header = "Barcode/IMEI";
            c8.Binding = new Binding("IMEI");
            //c8.Width = 100;
            salesHistoryDG.Columns.Add(c8);

            salesHistoryDG.IsReadOnly = true;

        }

        private void barcodeIMEI_Checked(object sender, RoutedEventArgs e)
        {
            customerNumberTextbox.IsEnabled = false;
            invoiceTextbox.IsEnabled = false;
            barcodeimeiTextBox.IsEnabled = true;

            barcodeimeiTextBox.Focus();

            customerNumberTextbox.Clear();
            invoiceTextbox.Clear();

            stockHistoryDG.Items.Clear();
            salesHistoryDG.Items.Clear();
        }


        private void customerNumberRadio_Checked(object sender, RoutedEventArgs e)
        {
            customerNumberTextbox.IsEnabled = true;
            invoiceTextbox.IsEnabled = false;
            barcodeimeiTextBox.IsEnabled = false;

            customerNumberTextbox.Focus();

            invoiceTextbox.Clear();
            barcodeimeiTextBox.Clear();

            stockHistoryDG.Items.Clear();
            salesHistoryDG.Items.Clear();
        }

        private void invoiceRadio_Checked(object sender, RoutedEventArgs e)
        {
            customerNumberTextbox.IsEnabled = false;
            invoiceTextbox.IsEnabled = true;
            barcodeimeiTextBox.IsEnabled = false;

            invoiceTextbox.Focus();

            customerNumberTextbox.Clear();
            barcodeimeiTextBox.Clear();

            stockHistoryDG.Items.Clear();
            salesHistoryDG.Items.Clear();
        }
        private void barcodeimeiTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter && barcodeimeiTextBox.Text != "")
            {
                stockHistoryDG.Items.Clear();
                salesHistoryDG.Items.Clear();
                try
                {
                    //for stock datagrid
                    Barcode bar = ProductTableData.getBarcode(barcodeimeiTextBox.Text);
                    Product product = bar.Product;
                                        
                    stockHistoryDG.Items.Add(product);

                    //for sales datagrid
                    Gift gif = bar.Gift;
                    if (gif != null)
                    {
                        Customer_Sale cus_sale = gif.Customer_Sale;

                        SalesHistoryDataGrid salesHis = new SalesHistoryDataGrid(cus_sale, gif.Barcode);
                        salesHistoryDG.Items.Add(salesHis);
                    }
                }
                catch (Exception ex)
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show("No product found. Press 'OK' to exit.\n\nDetailed Error:" + ex.Message, "Empty Product", MessageBoxButton.OK, MessageBoxImage.Error);                    
                }

            }
        }

        private void customerNumberTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && customerNumberTextbox.Text != "")
            {
                stockHistoryDG.Items.Clear();
                salesHistoryDG.Items.Clear();
                try
                {
                    var sales = Sale_TableData.getSalesbyCustomerID(int.Parse(customerNumberTextbox.Text));

                    foreach (var obj in sales)
                    {
                        var cc = Customer_SaleTableData.getCustomer_SalesbyInvoiceID(obj.Invoice_ID);
                        foreach (var item in cc)
                        {
                            SalesHistoryDataGrid salesHis;
                            if (item.Gifts.Count == 0)
                                salesHis = new SalesHistoryDataGrid(item, "");
                            else
                                salesHis = new SalesHistoryDataGrid(item, item.Gifts.First().Barcode);
                            salesHistoryDG.Items.Add(salesHis);
                            stockHistoryDG.Items.Add(item.Product);
                        }
                        
                    }
                }
                catch (Exception ex)
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show("No product found. Press 'OK' to exit.\n\nDetailed Error:" + ex.Message, "Empty Product", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
        }

        private void invoiceTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && invoiceTextbox.Text != "")
            {
                stockHistoryDG.Items.Clear();
                salesHistoryDG.Items.Clear();
                try
                {
                    Sale sales = Sale_TableData.getSalesbyInvoiceID(int.Parse(invoiceTextbox.Text));
                    var cus_sale = sales.Customer_Sales;
                    
                    foreach (var item in cus_sale)
                    {
                        SalesHistoryDataGrid salesHis;
                        if (item.Gifts.Count == 0)
                            salesHis = new SalesHistoryDataGrid(item, "");
                        else
                            salesHis = new SalesHistoryDataGrid(item, item.Gifts.First().Barcode);
                        
                        salesHistoryDG.Items.Add(salesHis);
                        stockHistoryDG.Items.Add(item.Product);
                    }
                }
                catch (Exception ex)
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show("No product found. Press 'OK' to exit.\n\nDetailed Error:" + ex.Message, "Empty Product", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void deleteSale_Click(object sender, RoutedEventArgs e)
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

            if(flag == true)
                Xceed.Wpf.Toolkit.MessageBox.Show("Sale deleted successfully. Press ok to exit.", "Exit");
        }
    }

    class StockHistoryDataGrid
    {
        public int ID { get; set; }
        public string Type { get; set; }
        public string Model { get; set; }
        public int Unit_Price { get; set; }
        public int Selling_Price { get; set; }
        public DateTime Date_Updated { get; set; }
        public string Description { get; set; }
    }

    class SalesHistoryDataGrid
    {
        public int Product_ID { get; set; }
        public int Customer_Sale_ID { get; set; }
        public int Invoice_ID { get; set; }
        public int Unit_Price { get; set; }
        public int Quantity { get; set; }
        public int Sold_Price { get; set; }
        public DateTime Date { get; set; }
        public int Customer_ID { get; set; }
        public string Name { get; set; }
        public int Phone2 { get; set; }
        public string IMEI { get; set; }

        public SalesHistoryDataGrid(Customer_Sale cus_sale, string imei)
        {                       
            Customer_Sale_ID = cus_sale.ID;
            Product_ID = (int)cus_sale.Product_ID;
            Invoice_ID = (int)cus_sale.Invoice_ID;
            Quantity = (int)cus_sale.Quantity;
            Unit_Price = (int)cus_sale.Unit_Price;
            Sold_Price = (int)cus_sale.Sold_Price;
            Customer_ID = (int)cus_sale.Sale.Customer_ID;
            Name = cus_sale.Sale.Customer.Name;
            Phone2 = (int)cus_sale.Sale.Customer.Phone2;
            IMEI = imei;
            Date = (DateTime)cus_sale.Sale.Date;

        }
    }
}
