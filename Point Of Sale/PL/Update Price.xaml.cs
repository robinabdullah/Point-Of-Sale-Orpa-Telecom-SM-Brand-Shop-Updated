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
    public partial class Update_Price : Window
    {


        public int unitPriceInt = 0;
        public int sellingPriceInt = 0;
        private Product product;
        

        public Update_Price()
        {
            InitializeComponent();
            unitPrice.Focus();
            
        }

        public Update_Price(Product product) : this()
        {
            this.product = product;
        }

        public void update_Button_Click(object sender, RoutedEventArgs e)
        {
            if(int.TryParse(unitPrice.Text, out unitPriceInt) == true && int.TryParse(sellingPrice.Text, out sellingPriceInt) == true)
            {
                try
                {
                    if (ProductTableData.updatePrice(product.ID, unitPriceInt, sellingPriceInt) == true)
                        Xceed.Wpf.Toolkit.MessageBox.Show("Price Updated Successfullly. Press ok to exit", "Info");
                }
                catch(Exception ex)
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, "Error");
                }
                this.Close();
            }
        }

        private void close_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
