using System;
using System.Windows;
using System.Windows.Shapes;
using Point_Of_Sale.DAL;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace Point_Of_Sale.PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Reports : Window
    {
        public static BaseFont bf = BaseFont.CreateFont(
                    BaseFont.TIMES_BOLD,
                    BaseFont.CP1252,
                    BaseFont.EMBEDDED);
        public static BaseFont bf1 = BaseFont.CreateFont(
                                BaseFont.TIMES_ROMAN,
                                BaseFont.CP1252,
                                BaseFont.EMBEDDED);
        //Rectangle.BOTTOM=2,Rectangle.TOP=1,Rectangle.RIGHT=8,Rectangle.LEFT=4 
        Font headerFont = new Font(bf, 18, Font.UNDERLINE);
        Font columnHeaderFont = new Font(bf, 12, Font.NORMAL);
        Font rowDataFont = new Font(bf1, 11, Font.NORMAL);
        string tempFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\" + "test.pdf";
        public static bool preview = false;
        public Reports()
        {
            InitializeComponent();
            DB.resetConnString();
        }
        private void stockQuantity_Click(object sender, RoutedEventArgs e)
        {
            Document doc = new Document(PageSize.A4);
            PdfWriter.GetInstance(doc, new FileStream(tempFile, FileMode.Create));
            doc.Open();

            PdfPTable table = new PdfPTable(3);
            PdfPCell cell;
            
            float[] widths = new float[] { 1.7f, 3.5f, 1.5f };
            table.SetWidths(widths);
            table.TotalWidth = 500f;
            table.LockedWidth = true;

            cell = new PdfPCell(new Phrase("Stock Summary (Quantity Based)", headerFont));
            cell.Colspan = 3;
            cell.MinimumHeight = 40;
            cell.HorizontalAlignment = 1;
            cell.Border = 0;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Product Code", columnHeaderFont));
            cell.Colspan = 1;
            cell.DisableBorderSide(12);
            cell.PaddingBottom = 6;

            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Product Model", columnHeaderFont));
            cell.Colspan = 1;
            cell.DisableBorderSide(12);
            cell.PaddingBottom = 6; //cell.HorizontalAlignment = 1;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Quantity", columnHeaderFont));
            cell.Colspan = 1;
            cell.DisableBorderSide(12);
            cell.PaddingBottom = 6;
            table.AddCell(cell);

            try
            {
                var allTypes = ProductTableData.getAllProductTypes();

                foreach (var type in allTypes)
                {
                    var allModels = ProductTableData.getTypeMachedModelsQNonZero(type);

                    if (allModels.Count != 0)
                    {
                        cell = new PdfPCell(new Phrase(type, columnHeaderFont));
                        cell.Colspan = 3;
                        cell.DisableBorderSide(13);
                        cell.PaddingTop = 10;
                        table.AddCell(cell);
                    }

                    foreach (var model in allModels)
                    {
                        var product = ProductTableData.getProductByModel(model);

                        cell = new PdfPCell(new Phrase(product.ID.ToString(), rowDataFont));
                        cell.Colspan = 1;
                        cell.Border = 0;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase(product.Model, rowDataFont));
                        cell.Colspan = 1;
                        cell.Border = 0;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase(product.Quantity_Available.ToString(), rowDataFont));
                        cell.Colspan = 1;
                        cell.Border = 0;
                        table.AddCell(cell);
                    }
                }                
                //printing the file created date
                cell = new PdfPCell(new Phrase("Report Created Date: " + DateTime.Now, rowDataFont));
                cell.Colspan = 3;
                cell.Border = 0;
                cell.PaddingTop = 10;
                cell.HorizontalAlignment = 0;
                table.AddCell(cell);

                doc.Add(table);

                doc.Close();

                Print.previewPdfFile(tempFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void stockSummaryFull_Click(object sender, RoutedEventArgs e)
        {
            Document doc = new Document(PageSize.A4);
            PdfWriter.GetInstance(doc, new FileStream(tempFile, FileMode.Create));
            doc.Open();

            int columns = 3;
            PdfPTable table = new PdfPTable(columns);
            PdfPCell cell;
            
            float[] widths = new float[] { 3.4f, 1.1f, 1.7f };
            table.SetWidths(widths);
            table.TotalWidth = 500f;
            //table.LockedWidth = true;

            cell = new PdfPCell(new Phrase("Stock Summary", headerFont));
            cell.Colspan = columns;
            cell.MinimumHeight = 40;
            cell.HorizontalAlignment = 1;
            cell.Border = 0;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Barcode/IMEI", columnHeaderFont));
            cell.Colspan = 1;
            cell.DisableBorderSide(12);
            cell.PaddingBottom = 6;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Color", columnHeaderFont));
            cell.Colspan = 1;
            cell.DisableBorderSide(12);
            cell.PaddingBottom = 6;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Purchase Date", columnHeaderFont));
            cell.Colspan = 1;
            cell.DisableBorderSide(12);
            cell.PaddingBottom = 6;
            table.AddCell(cell);

            try
            {
                int netTotal = 0, total = 0;
                string tempDate;
                var allTypes = ProductTableData.getAllProductTypes();

                foreach (var type in allTypes)
                {
                    var allModels = ProductTableData.getTypeMachedModelsQNonZero(type);

                    if (allModels.Count != 0)
                    {
                        int qu = ProductTableData.getTotalQ(type);
                        cell = new PdfPCell(new Phrase(string.Format("{0} ({1} Pcs.)", type, qu), columnHeaderFont));
                        cell.Colspan = columns;
                        cell.DisableBorderSide(13);
                        cell.PaddingBottom = 6;
                        cell.Indent = 0;
                        table.AddCell(cell);
                    }

                    foreach (var model in allModels)
                    {
                        var product = ProductTableData.getProductByModel(model);
                        //printing the product type 
                        cell = new PdfPCell(new Phrase(string.Format("{0} [ID: {1}]", product.Model, product.ID), columnHeaderFont));
                        cell.Colspan = columns;
                        cell.Border = 0;
                        cell.Indent = 10;
                        table.AddCell(cell);

                        var allBarcodes = product.Barcodes;
                        foreach (var bar in allBarcodes)
                        {
                            bool hasBarcodeinGiftTable = Gift_TableData.hasBarcodeinGiftTable(bar.Barcode_Serial);
                            if (hasBarcodeinGiftTable == false)
                            {
                                //printing the barcode n color for individual product
                                cell = new PdfPCell(new Phrase(bar.Barcode_Serial, rowDataFont));
                                cell.Colspan = 1;
                                cell.Border = 0;
                                cell.Indent = 10;
                                table.AddCell(cell);

                                cell = new PdfPCell(new Phrase(bar.Color, rowDataFont));
                                cell.Colspan = 1;
                                cell.Border = 0;
                                cell.Indent = 10;
                                table.AddCell(cell);
                                tempDate = bar.Date.Value.ToString("dd-MM-yy    hh:mm tt");
                                cell = new PdfPCell(new Phrase(tempDate, rowDataFont));
                                cell.Colspan = 1;
                                cell.Border = 0;
                                cell.Indent = 10;
                                table.AddCell(cell);
                            }                            
                        }
                        //printing the Quantity Available ar total taka for that quantity
                        total = (int)product.Quantity_Available * (int)product.Unit_Price;
                        netTotal += total; ///calculating net total taka
                        string temp = string.Format("Quantity Available: {0}Pcs. Unit Price: {1} taka. Total Amount = {2} taka", product.Quantity_Available, product.Unit_Price, total);
                        cell = new PdfPCell(new Phrase(temp, rowDataFont));
                        cell.Colspan = columns;
                        cell.Border = 0;
                        cell.PaddingBottom = 10;
                        cell.Indent = 10;
                        table.AddCell(cell);
                    }
                }
                //printing the net stock cash taka
                cell = new PdfPCell(new Phrase("Total Products in Cash TK. " + netTotal.ToString(), columnHeaderFont));
                cell.Colspan = columns;
                //cell.HorizontalAlignment = 2;
                cell.Border = 15;
                table.AddCell(cell);

                //printing the file created date
                cell = new PdfPCell(new Phrase("Report Created Date: " + DateTime.Now, rowDataFont));
                cell.Colspan = columns;
                cell.Border = 0;
                cell.PaddingTop = 10;
                //cell.HorizontalAlignment = 0;
                table.AddCell(cell);
                
                doc.Add(table);
                doc.Close();

                Print.previewPdfFile(tempFile);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        

        public static DateTime fromDate = new DateTime(2017, 1, 1);
        public static DateTime toDate = DateTime.Now;
        
        public static List<Customer_Sale> cust_saleList = new List<Customer_Sale>();
        private void Print_Previous_Invoice_Click(object sender, RoutedEventArgs e)
        {
            invoiceFindNPrintPreview.Visibility = Visibility.Visible;
            invoiceTextBox.Visibility = Visibility.Visible;

            invoiceTextBox.Focus();
        }
        private void find_and_print_preview(object sender, RoutedEventArgs e)
        {
            try
            {
                Sale sales = Sale_TableData.getSalesbyInvoiceID(int.Parse(invoiceTextBox.Text));
                var cus_sale = sales.Customer_Sales;

                generateBillforPreview(cus_sale.ToList(), sales);
                Print.previewPdfFile(filePath);
                File.Delete(filePath);

                
            }
            catch (Exception ex)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            invoiceFindNPrintPreview.Visibility = Visibility.Hidden;
            invoiceTextBox.Visibility = Visibility.Hidden;
        }
        string filePath;
        private void generateBillforPreview(List<Customer_Sale> cust_saleList, Sale sales)
        {
            int i = 0, price = 0;
            int totalTaka = 0;
            Customer customer = sales.Customer;

            string invoiceNo = sales.Invoice_ID.ToString();
            filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\" + invoiceNo + " " + customer.ID + ".pdf"; // temporary invoice path 
            PdfReader reader = new PdfReader(FileManagement.receipt);
            PdfStamper pdfStamper = new PdfStamper(reader, new FileStream(filePath, FileMode.Create)); //FileManagement.generatedReceipt
            AcroFields pdfFormFields = pdfStamper.AcroFields;


            pdfFormFields.SetField("InvoiceNo", invoiceNo);
            pdfFormFields.SetField("Date", sales.Date.ToString());
            pdfFormFields.SetField("Name", customer.Name);
            pdfFormFields.SetField("Mobile", customer.ID.ToString());
            pdfFormFields.SetField("Mobile2", customer.Phone2.ToString());
            pdfFormFields.SetField("Address", customer.Address.ToString());
            foreach (var cust_Sale in cust_saleList)
            {
                int dis = 0;
                Product pro = cust_Sale.Product;
                Gift gift = null;
                DAL.Barcode bar = null;

                try
                {
                    gift = cust_Sale.Gifts.First();
                    dis = (int)gift.Discount_Price;
                    bar = ProductTableData.getBarcode(gift.Barcode); // if the product has barcode
                }
                catch{ }
                
                if (pro.Unique_Barcode.StartsWith("Y"))
                {
                    pdfFormFields.SetField("Description." + i, string.Format("Type: {0} Model: {1} B/IMEI: {2} SL: {3} Dis: {4}", pro.Type, pro.Model, bar.Barcode_Serial, gift.SL, gift.Discount_Price));
                }
                else if (pro.Unique_Barcode.StartsWith("NY"))
                {
                    bar = ProductTableData.getBarcodesByPID(pro.ID).First();
                    pdfFormFields.SetField("Description." + i, string.Format("Type: {0} Model: {1} B/IMEI: {2}", pro.Type, pro.Model, bar.Barcode_Serial));
                }
                else
                {
                    pdfFormFields.SetField("Description." + i, string.Format("Type: {0} Model: {1}", pro.Type, pro.Model));
                }
                pdfFormFields.SetField("SL." + i, (i + 1).ToString());
                pdfFormFields.SetField("Quantity." + i, cust_Sale.Quantity.ToString());

                //if the product has give free with the mobile phone like Glass Paper
                if (Free_ProductTableData.getFreeProductby(sales.Invoice_ID, pro.ID) == true)
                {
                    pdfFormFields.SetField("Rate." + i, pro.Selling_Price.ToString());

                    price = 0;

                    pdfFormFields.SetField("Taka." + i, price.ToString() + " (Free)");
                }
                else
                {
                    pdfFormFields.SetField("Rate." + i, cust_Sale.Sold_Price.ToString());

                    price = ((int)cust_Sale.Sold_Price * (int)cust_Sale.Quantity) - (int)dis;

                    pdfFormFields.SetField("Taka." + i, price.ToString());
                }
                
                i++;
                totalTaka += price;
                price = 0;
            }
            pdfFormFields.SetField("totalTaka", totalTaka.ToString());
            pdfStamper.FormFlattening = true;
            pdfStamper.Close();



        }
        private void dateWiseSales1_Click(object sender, RoutedEventArgs e)
        {
            Date_to_Date dateToDate = new Date_to_Date();
            dateToDate.ShowDialog();

            if (Reports.preview == false) ///if the date to date windows closes then retun
                return; 
            
            Document doc = new Document(PageSize.A4);
            PdfWriter.GetInstance(doc, new FileStream(tempFile, FileMode.Create));
            doc.Open();

            int totalColumn = 6;
            PdfPTable table = new PdfPTable(totalColumn);
            PdfPCell cell;

            float[] widths = new float[] { 1.2f, 1.3f, 3.5f, 1f, 1f, .9f }; //column widths
            table.SetWidths(widths);
            table.TotalWidth = 550f;
            table.LockedWidth = true;



            cell = new PdfPCell(new Phrase("Date wise Sales", headerFont));
            cell.Colspan = totalColumn;
            cell.MinimumHeight = 25;
            cell.HorizontalAlignment = 1;
            cell.Border = 0;
            table.AddCell(cell);

            string stringFormat = string.Format("Date From {0} to {1}", fromDate.ToShortDateString(), toDate.ToShortDateString());
            cell = new PdfPCell(new Phrase(stringFormat, columnHeaderFont));
            cell.Colspan = totalColumn;
            cell.MinimumHeight = 25;
            cell.HorizontalAlignment = 1;
            cell.Border = 0;
            table.AddCell(cell);

            // printing the column headings starts
            cell = new PdfPCell(new Phrase("Invoice No.", columnHeaderFont));
            cell.Colspan = 1;
            cell.DisableBorderSide(12);
            cell.PaddingBottom = 6;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Mobile", columnHeaderFont));
            cell.Colspan = 1;
            cell.DisableBorderSide(12);
            cell.PaddingBottom = 6;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Product", columnHeaderFont));
            cell.Colspan = 1;
            cell.DisableBorderSide(12);
            cell.PaddingBottom = 6;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Sold Price", columnHeaderFont));
            cell.Colspan = 1;
            cell.DisableBorderSide(12);
            cell.PaddingBottom = 6;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Profit", columnHeaderFont));
            cell.Colspan = 1;
            cell.DisableBorderSide(12);
            cell.PaddingBottom = 6;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Discount", columnHeaderFont));
            cell.Colspan = 1;
            cell.DisableBorderSide(12);
            cell.PaddingBottom = 6;
            table.AddCell(cell);
            // printing the column headings ends

            int discount = 0, totalSales = 0, totalDiscount = 0, totalProfit = 0;
            int netTotalSales = 0, netTotalDiscount = 0, netTotalProfit = 0;
            int invoiceBackup = 0;
            int borderSide = 12;
            try
            {
                DateTime increDate = fromDate;
                while (increDate <= toDate)
                {
                    var allInvoices = Sale_TableData.getSalesbyDate(increDate);

                    if (allInvoices.Count != 0)
                    {
                        ///printing date of sales
                        cell = new PdfPCell(new Phrase(increDate.ToShortDateString(), columnHeaderFont));
                        cell.Colspan = totalColumn;
                        cell.DisableBorderSide(13);
                        cell.PaddingBottom = 3;
                        table.AddCell(cell);
                    }

                    increDate = increDate.AddDays(1); ///increase date by one

                    foreach (var invoice in allInvoices)
                    {
                        var allCust_Sale = Customer_SaleTableData.getCustomer_SalesbyInvoiceID(invoice.Invoice_ID);

                        foreach (var cu_sale in allCust_Sale)
                        {
                            //printing the day wise sale 
                            /// duplicate printing of invoice id is discarded here
                            if (invoiceBackup != invoice.Invoice_ID)
                            {
                                ///disabling the border dise 
                                if (allCust_Sale.Count > 1) borderSide = 14;
                                else borderSide = 12;

                                cell = new PdfPCell(new Phrase(invoice.Invoice_ID.ToString(), rowDataFont));
                                cell.Colspan = 1;
                                cell.DisableBorderSide(borderSide);
                                table.AddCell(cell);

                                cell = new PdfPCell(new Phrase(invoice.Customer_ID.ToString(), rowDataFont));
                                cell.Colspan = 1;
                                cell.DisableBorderSide(borderSide);
                                table.AddCell(cell);
                            }
                            else
                            {
                                cell = new PdfPCell(new Phrase("", rowDataFont));
                                cell.Colspan = 1;
                                cell.DisableBorderSide(15);
                                table.AddCell(cell);

                                cell = new PdfPCell(new Phrase("", rowDataFont));
                                cell.Colspan = 1;
                                cell.DisableBorderSide(15);
                                table.AddCell(cell);
                            }
                            invoiceBackup = invoice.Invoice_ID;

                            ///to print the product type, model
                            Product pro = ProductTableData.getProductByID((int)cu_sale.Product_ID);
                            stringFormat = string.Format("{0} [{1} {2}P]", pro.Type, pro.Model, cu_sale.Quantity);
                            cell = new PdfPCell(new Phrase(stringFormat, rowDataFont));
                            cell.Colspan = 1;
                            cell.DisableBorderSide(borderSide);
                            table.AddCell(cell);

                            ///free product should not be added to total salew
                            if (Free_ProductTableData.getFreeProductby(invoice.Invoice_ID, pro.ID))
                            {
                                cell = new PdfPCell(new Phrase("Free", rowDataFont));
                                cell.Colspan = 1;
                                cell.DisableBorderSide(borderSide);
                                table.AddCell(cell);
                            }
                            else
                            {
                                cell = new PdfPCell(new Phrase((((int)cu_sale.Sold_Price) * (int)cu_sale.Quantity).ToString(), rowDataFont));
                                cell.Colspan = 1;
                                cell.DisableBorderSide(borderSide);
                                table.AddCell(cell);

                                ///add if it is not a free product
                                totalSales += ((int)cu_sale.Sold_Price * (int)cu_sale.Quantity);
                            }

                            cell = new PdfPCell(new Phrase(((cu_sale.Sold_Price - cu_sale.Unit_Price) * cu_sale.Quantity).ToString(), rowDataFont));
                            cell.Colspan = 1;
                            cell.DisableBorderSide(borderSide);
                            table.AddCell(cell);

                            ///prints the discount on product
                            var gift = Gift_TableData.getGiftRowbyCust_SaleID(cu_sale.ID);
                            if (gift != null)
                                discount = (int)gift.Discount_Price;

                            cell = new PdfPCell(new Phrase(discount.ToString(), rowDataFont));
                            cell.Colspan = 1;
                            cell.DisableBorderSide(borderSide);
                            table.AddCell(cell);

                            totalProfit += ((int)cu_sale.Sold_Price - (int)cu_sale.Unit_Price) * (int)cu_sale.Quantity;
                            totalDiscount += discount;
                            discount = 0; /// discount reset
                        }
                    }
                    if (allInvoices.Count == 0)
                        continue;

                    //printing the Day End Total Sale, profit, given discount starts
                    cell = new PdfPCell(new Phrase("", rowDataFont));///blank column data
                    cell.Colspan = 1;
                    cell.Border = 1;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase("", rowDataFont));///blank column data
                    cell.Colspan = 1;
                    cell.Border = 1;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase("", rowDataFont));///blank column data
                    cell.Colspan = 1;
                    cell.Border = 1;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(totalSales.ToString(), columnHeaderFont));
                    cell.Colspan = 1;
                    cell.Border = 1;
                    //cell.PaddingBottom = 15;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(totalProfit.ToString(), columnHeaderFont));
                    cell.Colspan = 1;
                    cell.Border = 1;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(totalDiscount.ToString(), columnHeaderFont));
                    cell.Colspan = 1;
                    cell.Border = 1;

                    table.AddCell(cell);

                    ///calculating the net total sales,profit, discount
                    netTotalSales += totalSales;
                    netTotalProfit += totalProfit;
                    netTotalDiscount += totalDiscount;

                    /// resetting the total sales,profit,discount
                    totalSales = 0;
                    totalProfit = 0;
                    totalDiscount = 0;
                    //printing the Day End Total Sale, profit, given discount ends
                }

                //Net total//printing the Day End Net Total Sale, profit, given discount starts
                cell = new PdfPCell(new Phrase("", rowDataFont));///blank column data
                cell.Colspan = 1;
                cell.Border = 1;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("", rowDataFont));///blank column data
                cell.Colspan = 1;
                cell.Border = 1;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Net Total", columnHeaderFont));///blank column data
                cell.Colspan = 1;
                cell.HorizontalAlignment = 2;
                cell.Border = 1;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(netTotalSales.ToString(), columnHeaderFont));
                cell.Colspan = 1;
                cell.Border = 1;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(netTotalProfit.ToString(), columnHeaderFont));
                cell.Colspan = 1;
                cell.Border = 1;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(netTotalDiscount.ToString(), columnHeaderFont));
                cell.Colspan = 1;
                cell.Border = 1;
                table.AddCell(cell);
                //Net total//printing the Day End Net Total Sale, profit, given discount ends

                //printing the file created date
                cell = new PdfPCell(new Phrase("Report Created Date: " + DateTime.Now, rowDataFont));
                cell.Colspan = 6;
                cell.Border = 0;
                cell.HorizontalAlignment = 0;
                table.AddCell(cell);

                doc.Add(table);

                doc.Close();

                if (preview == true)
                    Print.previewPdfFile(tempFile); ///preview the created file

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private void dateWiseSales2_Click(object sender, RoutedEventArgs e)
        {
            Date_to_Date dateToDate = new Date_to_Date();
            dateToDate.ShowDialog();

            if (Reports.preview == false)///if the date to date windows closes then retun
                return;

            Document doc = new Document(PageSize.A4);
            PdfWriter.GetInstance(doc, new FileStream(tempFile, FileMode.Create));
            doc.Open();

            int totalColumn = 6;
            PdfPTable table = new PdfPTable(totalColumn);
            PdfPCell cell;

            float[] widths = new float[] { 1.2f, 1.3f, 3.5f, 1f, 1f, .9f }; //column widths
            table.SetWidths(widths);
            table.TotalWidth = 550f;
            table.LockedWidth = true;



            cell = new PdfPCell(new Phrase("Date wise Sales", headerFont));
            cell.Colspan = totalColumn;
            cell.MinimumHeight = 25;
            cell.HorizontalAlignment = 1;
            cell.Border = 0;
            table.AddCell(cell);

            string stringFormat = string.Format("Date From {0} to {1}", fromDate.ToShortDateString(), toDate.ToShortDateString());
            cell = new PdfPCell(new Phrase(stringFormat, columnHeaderFont));
            cell.Colspan = totalColumn;
            cell.MinimumHeight = 25;
            cell.HorizontalAlignment = 1;
            cell.Border = 0;
            table.AddCell(cell);

            // printing the column headings starts
            cell = new PdfPCell(new Phrase("Invoice No.", columnHeaderFont));
            cell.Colspan = 1;
            cell.DisableBorderSide(12);
            cell.PaddingBottom = 6;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Mobile", columnHeaderFont));
            cell.Colspan = 1;
            cell.DisableBorderSide(12);
            cell.PaddingBottom = 6;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Product", columnHeaderFont));
            cell.Colspan = 1;
            cell.DisableBorderSide(12);
            cell.PaddingBottom = 6;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Sold Price", columnHeaderFont));
            cell.Colspan = 1;
            cell.DisableBorderSide(12);
            cell.PaddingBottom = 6;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Profit", columnHeaderFont));
            cell.Colspan = 1;
            cell.DisableBorderSide(12);
            cell.PaddingBottom = 6;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Discount", columnHeaderFont));
            cell.Colspan = 1;
            cell.DisableBorderSide(12);
            cell.PaddingBottom = 6;
            table.AddCell(cell);
            // printing the column headings ends

            int discount = 0, totalSales = 0, totalDiscount = 0, totalProfit = 0;
            int netTotalSales = 0, netTotalDiscount = 0, netTotalProfit = 0;
            int invoiceBackup = 0;
            int borderSide = 12;
            try
            {
                DateTime increDate = fromDate;
                while (increDate <= toDate)
                {
                    var allInvoices = Sale_TableData.getSalesbyDate(increDate);

                    if (allInvoices.Count != 0)
                    {
                        ///printing date of sales
                        cell = new PdfPCell(new Phrase(increDate.ToShortDateString(), columnHeaderFont));
                        cell.Colspan = totalColumn;
                        cell.DisableBorderSide(13);
                        cell.PaddingBottom = 3;
                        table.AddCell(cell);
                    }

                    increDate = increDate.AddDays(1); ///increase date by one

                    foreach (var invoice in allInvoices)
                    {
                        var allCust_Sale = Customer_SaleTableData.getCustomer_SalesbyInvoiceID(invoice.Invoice_ID);

                        foreach (var cu_sale in allCust_Sale)
                        {
                            //printing the day wise sale 
                            /// duplicate printing of invoice id is discarded here
                            if (invoiceBackup != invoice.Invoice_ID)
                            {
                                ///disabling the border dise 
                                if (allCust_Sale.Count > 1) borderSide = 14;
                                else borderSide = 12;

                                cell = new PdfPCell(new Phrase(invoice.Invoice_ID.ToString(), rowDataFont));
                                cell.Colspan = 1;
                                cell.DisableBorderSide(borderSide);
                                table.AddCell(cell);

                                cell = new PdfPCell(new Phrase(invoice.Customer_ID.ToString(), rowDataFont));
                                cell.Colspan = 1;
                                cell.DisableBorderSide(borderSide);
                                table.AddCell(cell);
                            }
                            else
                            {
                                cell = new PdfPCell(new Phrase("", rowDataFont));
                                cell.Colspan = 1;
                                cell.DisableBorderSide(15);
                                table.AddCell(cell);

                                cell = new PdfPCell(new Phrase("", rowDataFont));
                                cell.Colspan = 1;
                                cell.DisableBorderSide(15);
                                table.AddCell(cell);
                            }
                            invoiceBackup = invoice.Invoice_ID;

                            ///to print the product type, model
                            Product pro = ProductTableData.getProductByID((int)cu_sale.Product_ID);
                            stringFormat = string.Format("{0} [{1} {2}P]", pro.Type, pro.Model, cu_sale.Quantity);
                            cell = new PdfPCell(new Phrase(stringFormat, rowDataFont));
                            cell.Colspan = 1;
                            cell.DisableBorderSide(borderSide);
                            table.AddCell(cell);

                            ///prints the discount on product
                            var gift = Gift_TableData.getGiftRowbyCust_SaleID(cu_sale.ID);
                            if (gift != null)
                                discount = (int)gift.Discount_Price;
                            
                            ///free product sold price should not be added
                            if (Free_ProductTableData.getFreeProductby(invoice.Invoice_ID, pro.ID))
                            {
                                cell = new PdfPCell(new Phrase("Free", rowDataFont));
                                cell.Colspan = 1;
                                cell.DisableBorderSide(borderSide);
                                table.AddCell(cell);
                            }
                            else
                            {
                                cell = new PdfPCell(new Phrase((((int)cu_sale.Sold_Price - discount) * (int)cu_sale.Quantity).ToString(), rowDataFont));
                                cell.Colspan = 1;
                                cell.DisableBorderSide(borderSide);
                                table.AddCell(cell);

                                ///add if it is not a free product
                                totalSales += (((int)cu_sale.Sold_Price - discount) * (int)cu_sale.Quantity);
                            }
                            cell = new PdfPCell(new Phrase(((cu_sale.Sold_Price - cu_sale.Unit_Price) * cu_sale.Quantity).ToString(), rowDataFont));
                            cell.Colspan = 1;
                            cell.DisableBorderSide(borderSide);
                            table.AddCell(cell);
                            
                            cell = new PdfPCell(new Phrase(discount.ToString(), rowDataFont));
                            cell.Colspan = 1;
                            cell.DisableBorderSide(borderSide);
                            table.AddCell(cell);

                            //set total sale sold price ,profit , dicount
                            totalProfit += ((int)cu_sale.Sold_Price - (int)cu_sale.Unit_Price) * (int)cu_sale.Quantity;
                            totalDiscount += discount;
                            discount = 0; /// discount reset
                        }
                    }
                    if (allInvoices.Count == 0)
                        continue;

                    //printing the Day End Total Sale, profit, given discount starts
                    cell = new PdfPCell(new Phrase("", rowDataFont));///blank column data
                    cell.Colspan = 1;
                    cell.Border = 1;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase("", rowDataFont));///blank column data
                    cell.Colspan = 1;
                    cell.Border = 1;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase("", rowDataFont));///blank column data
                    cell.Colspan = 1;
                    cell.Border = 1;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(totalSales.ToString(), columnHeaderFont));
                    cell.Colspan = 1;
                    cell.Border = 1;
                    //cell.PaddingBottom = 15;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(totalProfit.ToString(), columnHeaderFont));
                    cell.Colspan = 1;
                    cell.Border = 1;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(totalDiscount.ToString(), columnHeaderFont));
                    cell.Colspan = 1;
                    cell.Border = 1;

                    table.AddCell(cell);

                    ///calculating the net total sales,profit, discount
                    netTotalSales += totalSales;
                    netTotalProfit += totalProfit;
                    netTotalDiscount += totalDiscount;

                    /// resetting the total sales,profit,discount
                    totalSales = 0;
                    totalProfit = 0;
                    totalDiscount = 0;
                    //printing the Day End Total Sale, profit, given discount ends
                }

                //Net total//printing the Day End Net Total Sale, profit, given discount starts
                cell = new PdfPCell(new Phrase("", rowDataFont));///blank column data
                cell.Colspan = 1;
                cell.Border = 1;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("", rowDataFont));///blank column data
                cell.Colspan = 1;
                cell.Border = 1;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Net Total", columnHeaderFont));///blank column data
                cell.Colspan = 1;
                cell.HorizontalAlignment = 2;
                cell.Border = 1;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(netTotalSales.ToString(), columnHeaderFont));
                cell.Colspan = 1;
                cell.Border = 1;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(netTotalProfit.ToString(), columnHeaderFont));
                cell.Colspan = 1;
                cell.Border = 1;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(netTotalDiscount.ToString(), columnHeaderFont));
                cell.Colspan = 1;
                cell.Border = 1;
                table.AddCell(cell);
                //Net total//printing the Day End Net Total Sale, profit, given discount ends

                //printing the file created date
                cell = new PdfPCell(new Phrase("Report Created Date: " + DateTime.Now, rowDataFont));
                cell.Colspan = 6;
                cell.Border = 0;
                cell.HorizontalAlignment = 0;
                table.AddCell(cell);

                doc.Add(table);

                doc.Close();

                if (preview == true)
                    Print.previewPdfFile(tempFile); ///preview the created file

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void dateWise_Stock_Click(object sender, RoutedEventArgs e)
        {
            Date_to_Date dateToDate = new Date_to_Date();
            dateToDate.ShowDialog();

            if (Reports.preview == false)///if the date to date windows closes then retun
                return;

            Document doc = new Document(PageSize.A4);
            PdfWriter.GetInstance(doc, new FileStream(tempFile, FileMode.Create));
            doc.Open();

            int totalColumn = 5;
            PdfPTable table = new PdfPTable(totalColumn);
            PdfPCell cell;

            float[] widths = new float[] { 3.6f, 1f, 1.2f, 1f, 1f }; //column widths
            table.SetWidths(widths);
            table.TotalWidth = 450f;
            table.LockedWidth = true;

            cell = new PdfPCell(new Phrase("Date wise Stock", headerFont));
            cell.Colspan = totalColumn;
            cell.MinimumHeight = 25;
            cell.HorizontalAlignment = 1;
            cell.Border = 0;
            table.AddCell(cell);

            string stringFormat = string.Format("Date From {0} to {1}", fromDate.ToShortDateString(), toDate.ToShortDateString());
            cell = new PdfPCell(new Phrase(stringFormat, columnHeaderFont));
            cell.Colspan = totalColumn;
            cell.MinimumHeight = 25;
            cell.HorizontalAlignment = 1;
            cell.Border = 0;
            table.AddCell(cell);

            // printing the column headings starts
            cell = new PdfPCell(new Phrase("Product Model.", columnHeaderFont));
            cell.Colspan = 1;
            cell.DisableBorderSide(12);
            cell.PaddingBottom = 6;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Stock IN", columnHeaderFont));
            cell.Colspan = 1;
            cell.DisableBorderSide(12);
            cell.PaddingBottom = 6;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Stock Cash", columnHeaderFont));
            cell.Colspan = 1;
            cell.DisableBorderSide(12);
            cell.PaddingBottom = 6;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Sale OUT", columnHeaderFont));
            cell.Colspan = 1;
            cell.DisableBorderSide(12);
            cell.PaddingBottom = 6;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Sale Cash", columnHeaderFont));
            cell.Colspan = 1;
            cell.DisableBorderSide(12);
            cell.PaddingBottom = 6;
            table.AddCell(cell);
            // printing the column headings ends

            int stockINCash = 0, saleOutCash = 0;
            int countStockTotal = 0, countSalesTotal = 0;
            int netStockINCash = 0, netSaleOutCash = 0;
            int netCountStock = 0, netCountSales = 0;
            int countStock = 0, countSale = 0;
            int saleOnDate = 0;
            int borderSide = 12;
            

            Customer_Sale cus_sale = new Customer_Sale();
            Gift gift = new Gift();
            int sale = 0;

            try
            {
                DateTime increDate = fromDate;

                var allProducts = ProductTableData.getAllProducts().Where(x => x.Unique_Barcode.StartsWith("Y"));
                while (increDate <= toDate)
                {
                    ///printing date of sales
                    cell = new PdfPCell(new Phrase(increDate.ToShortDateString(), columnHeaderFont));
                    cell.Colspan = totalColumn;
                    cell.DisableBorderSide(13);
                    cell.PaddingBottom = 3;
                    table.AddCell(cell);

                    foreach (var product in allProducts)
                    {

                        //printing the day wise stock 
                        stringFormat = string.Format("{0}  ", product.Model);// [{1}] , product.Type
                        cell = new PdfPCell(new Phrase(stringFormat, rowDataFont));
                        cell.Colspan = 1;
                        cell.DisableBorderSide(borderSide);
                        table.AddCell(cell);

                        var barcodes = ProductTableData.getBarcodesByPID(product.ID);
                        
                        // counts sold out and not sold out products
                        foreach (var barc in barcodes)
                        {
                            bool ha = Gift_TableData.hasBarcodeinGiftTable(barc.Barcode_Serial);
                           
                            if (ha == true)
                            {
                                gift = Gift_TableData.getGiftRowbtBarcode(barc.Barcode_Serial);
                                cus_sale = gift.Customer_Sale;
                                sale = (int)cus_sale.Sold_Price;
                                if (cus_sale.Sale.Date.Value.Date == increDate)
                                {
                                    countSale++;/// sold product count
                                    saleOnDate += (int)sale;
                                }
                            }

                            if (barc.Date.Value.Date == increDate)
                                countStock++;/// stock product count
                        }

                        cell = new PdfPCell(new Phrase(countStock.ToString(), rowDataFont));
                        cell.Colspan = 1;
                        cell.DisableBorderSide(borderSide);
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase((countStock * product.Unit_Price).ToString(), rowDataFont));
                        cell.Colspan = 1;
                        cell.DisableBorderSide(borderSide);
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase(countSale.ToString(), rowDataFont));
                        cell.Colspan = 1;
                        cell.DisableBorderSide(borderSide);
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase(saleOnDate.ToString(), rowDataFont));
                        cell.Colspan = 1;
                        cell.DisableBorderSide(borderSide);
                        table.AddCell(cell);

                        countSalesTotal += countSale;
                        countStockTotal += countStock;

                        stockINCash += countStock * (int)product.Unit_Price;
                        saleOutCash += saleOnDate;

                        countStock = 0;
                        countSale = 0;
                        saleOnDate = 0;
                    }
                    increDate = increDate.AddDays(1); ///increase date by one

                    //printing the Date End Total stock in, sale out
                    cell = new PdfPCell(new Phrase("", rowDataFont));///blank column data
                    cell.Colspan = 1;
                    cell.Border = 1;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(countStockTotal.ToString(), rowDataFont));
                    cell.Colspan = 1;
                    cell.Border = 1;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(stockINCash.ToString(), rowDataFont));
                    cell.Colspan = 1;
                    cell.Border = 1;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(countSalesTotal.ToString(), rowDataFont));
                    cell.Colspan = 1;
                    cell.Border = 1;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(saleOutCash.ToString(), rowDataFont));
                    cell.Colspan = 1;
                    cell.Border = 1;
                    table.AddCell(cell);

                    netStockINCash += stockINCash; /// stock cash
                    netSaleOutCash += saleOutCash; /// sale cash
                    netCountStock += countStockTotal; /// stock total in pieces
                    netCountSales += countSalesTotal; /// sale total in pieces

                    /// resetting the total sales,profit,discount
                    stockINCash = 0;
                    saleOutCash = 0;
                    countStockTotal = 0;
                    countSalesTotal = 0;
                    //printing the Day End Total Sale, profit, given discount ends
                }

                //Net total//printing the Day End Net Total .... starts
                cell = new PdfPCell(new Phrase("Net Total", columnHeaderFont));
                cell.Colspan = 1;
                cell.HorizontalAlignment = 2;
                cell.Border = 1;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(netCountStock.ToString(), columnHeaderFont));
                cell.Colspan = 1;
                cell.Border = 1;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(netStockINCash.ToString(), columnHeaderFont));
                cell.Colspan = 1;
                cell.Border = 1;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(netCountSales.ToString(), columnHeaderFont));
                cell.Colspan = 1;
                cell.Border = 1;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(netSaleOutCash.ToString(), columnHeaderFont));
                cell.Colspan = 1;
                cell.Border = 1;
                table.AddCell(cell);
                //Net total//printing the Day End Net Total .... ends


                //printing the file created date
                cell = new PdfPCell(new Phrase("Report Created Date: " + DateTime.Now, rowDataFont));
                cell.Colspan = totalColumn;
                cell.Border = 0;
                cell.HorizontalAlignment = 0;
                table.AddCell(cell);

                doc.Add(table);

                doc.Close();

                if (preview == true)
                    Print.previewPdfFile(tempFile); ///preview the created file

            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);
                //Console.WriteLine(ex.StackTrace);
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void promotionalEventSales_Click(object sender, RoutedEventArgs e)
        {
            Date_to_Date dateToDate = new Date_to_Date();
            dateToDate.ShowDialog();
            if (Reports.preview == false)///if the date to date windows closes then retun
                return;

            Document doc = new Document(PageSize.A4);
            PdfWriter.GetInstance(doc, new FileStream(tempFile, FileMode.Create));
            doc.Open();

            int totalColumn = 5;
            PdfPTable table = new PdfPTable(totalColumn);
            PdfPCell cell;

            float[] widths = new float[] { 4f, 1f, 1f, 1f, 1f }; //column widths
            table.SetWidths(widths);
            table.TotalWidth = 450f;
            table.LockedWidth = true;

            cell = new PdfPCell(new Phrase("Date wise Sale", headerFont));
            cell.Colspan = totalColumn;
            cell.MinimumHeight = 25;
            cell.HorizontalAlignment = 1;
            cell.Border = 0;
            table.AddCell(cell);

            string stringFormat = string.Format("Date From {0} to {1}", fromDate.ToShortDateString(), toDate.ToShortDateString());
            cell = new PdfPCell(new Phrase(stringFormat, columnHeaderFont));
            cell.Colspan = totalColumn;
            cell.MinimumHeight = 25;
            cell.HorizontalAlignment = 1;
            cell.Border = 0;
            table.AddCell(cell);

            // printing the column headings starts
            cell = new PdfPCell(new Phrase("Product Model.", columnHeaderFont));
            cell.Colspan = 1;
            cell.DisableBorderSide(12);
            cell.PaddingBottom = 6;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Stock IN", columnHeaderFont));
            cell.Colspan = 1;
            cell.DisableBorderSide(12);
            cell.PaddingBottom = 6;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Stock Cash", columnHeaderFont));
            cell.Colspan = 1;
            cell.DisableBorderSide(12);
            cell.PaddingBottom = 6;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Sale OUT", columnHeaderFont));
            cell.Colspan = 1;
            cell.DisableBorderSide(12);
            cell.PaddingBottom = 6;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Sale Cash", columnHeaderFont));
            cell.Colspan = 1;
            cell.DisableBorderSide(12);
            cell.PaddingBottom = 6;
            table.AddCell(cell);
            // printing the column headings ends

            int countStock = 0, countSale = 0;
            int saleOnDate = 0;
            int borderSide = 12;


            Customer_Sale cus_sale = new Customer_Sale();
            Gift gift = new Gift();
            int sale = 0;
            int i = 0;
            int[] stockINCashA;
            int[] saleOutCashA;
            int[] countStockA;
            int[] countSalesA;

            int netStockINCash = 0;
            int netSaleOutCash = 0;
            int netCountStock = 0;
            int netCountSales = 0;
            try
            {
                DateTime increDate = fromDate;

                var allProducts = ProductTableData.getAllProducts().Where(x => x.Unique_Barcode.StartsWith("Y"));

                stockINCashA = new int[allProducts.Count()];
                saleOutCashA = new int[allProducts.Count()];
                countStockA = new int[allProducts.Count()];
                countSalesA = new int[allProducts.Count()];

                while (increDate <= toDate)
                {

                    foreach (var product in allProducts)
                    {
                        

                        var barcodes = ProductTableData.getBarcodesByPID(product.ID);


                        // counts sold out and not sold out products
                        foreach (var barc in barcodes)
                        {
                            bool ha = Gift_TableData.hasBarcodeinGiftTable(barc.Barcode_Serial);

                            if (ha == true)
                            {
                                gift = Gift_TableData.getGiftRowbtBarcode(barc.Barcode_Serial);
                                cus_sale = gift.Customer_Sale;
                                sale = (int)cus_sale.Sold_Price;
                                if (cus_sale.Sale.Date.Value.Date == increDate)
                                {
                                    countSale++;
                                    saleOnDate += (int)sale;
                                }
                            }

                            if (barc.Date.Value.Date == increDate)
                                countStock++;// stock product count
                        }
                        
                        countSalesA[i] += countSale;
                        countStockA[i] += countStock;

                        stockINCashA[i] += (countStock * (int)product.Unit_Price);
                        saleOutCashA[i] += saleOnDate;

                        countStock = 0;
                        countSale = 0;
                        saleOnDate = 0;
                        i++;
                    }
                    i = 0;
                    increDate = increDate.AddDays(1); ///increase date by one
                }
                var allModels = allProducts.Select(x => x.Model).ToList();
                i = 0;
                foreach (var model in allModels)
                {
                    //printing the day wise stock and sale =>  product wise 
                    stringFormat = string.Format("{0}  ", model);
                    cell = new PdfPCell(new Phrase(stringFormat, rowDataFont));
                    cell.Colspan = 1;
                    cell.DisableBorderSide(borderSide);
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(countStockA[i].ToString(), rowDataFont));
                    cell.Colspan = 1;
                    cell.DisableBorderSide(borderSide);
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(stockINCashA[i].ToString(), rowDataFont));
                    cell.Colspan = 1;
                    cell.DisableBorderSide(borderSide);
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(countSalesA[i].ToString(), rowDataFont));
                    cell.Colspan = 1;
                    cell.DisableBorderSide(borderSide);
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(saleOutCashA[i].ToString(), rowDataFont));
                    cell.Colspan = 1;
                    cell.DisableBorderSide(borderSide);
                    table.AddCell(cell);

                    netCountStock += countStockA[i];
                    netStockINCash += stockINCashA[i];
                    netCountSales += countSalesA[i];
                    netSaleOutCash += saleOutCashA[i];
                    i++;
                }
                //Net// printing net total amounts starts
                cell = new PdfPCell(new Phrase("", rowDataFont));
                cell.Colspan = 1;
                cell.DisableBorderSide(borderSide);
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(netCountStock.ToString(), columnHeaderFont));
                cell.Colspan = 1;
                cell.DisableBorderSide(borderSide);
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(netStockINCash.ToString(), columnHeaderFont));
                cell.Colspan = 1;
                cell.DisableBorderSide(borderSide);
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(netCountSales.ToString(), columnHeaderFont));
                cell.Colspan = 1;
                cell.DisableBorderSide(borderSide);
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(netSaleOutCash.ToString(), columnHeaderFont));
                cell.Colspan = 1;
                cell.DisableBorderSide(borderSide);
                table.AddCell(cell);
                //Net// printing net total amounts ends

                //printing the file created date
                cell = new PdfPCell(new Phrase("Report Created Date: " + DateTime.Now, rowDataFont));
                cell.Colspan = totalColumn;
                cell.Border = 0;
                cell.HorizontalAlignment = 0;
                table.AddCell(cell);

                doc.Add(table);

                doc.Close();

                if (preview == true)
                    Print.previewPdfFile(tempFile); ///preview the created file

            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);
                //Console.WriteLine(ex.StackTrace);
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }
    }
}
