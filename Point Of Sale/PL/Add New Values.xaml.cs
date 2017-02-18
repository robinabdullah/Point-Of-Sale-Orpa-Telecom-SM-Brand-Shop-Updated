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

namespace Point_Of_Sale.PL
{
    /// <summary>
    /// Interaction logic for Color_Add.xaml
    /// </summary>
    public partial class Add_New_Values : Window
    {
        private ComboBox color;
        private Product product;

        public Add_New_Values()
        {
            InitializeComponent();
        }

        public Add_New_Values(ComboBox color): this()
        {
            ok.Click -= adjust_quantity_Click; ///removing adjust quantity click
            ok.Click += ok_color_Click; ///adding adjust quantity click
            this.color = color;
        }

        public Add_New_Values(Product product): this()
        {
            ok.Click -= ok_color_Click;
            ok.Click += adjust_quantity_Click;
            this.product = product;
        }

        private void ok_color_Click(object sender, RoutedEventArgs e)
        {
            if (FileManagement.IsColorAlreadyExists(newColor.Text) == true)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Duplicate color. Pres ok to exit.", "Duplicate Color", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                return;
            }

            if (newColor.Text != "" )
            {
                FileManagement.setNewColor(newColor.Text);
                color.ItemsSource = FileManagement.getAllColor();
                color.SelectedIndex = color.Items.Count - 1;
                this.Close();
            }
            else
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Color textfield empty. Pres ok to exit.", "Empty extfield", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }
        private void adjust_quantity_Click(object sender, RoutedEventArgs e)
        {
            int quantity = 0;
           
            if (newColor.Text != "" && int.TryParse(newColor.Text, out quantity) == true)
            {
                product.Quantity_Available = quantity;
                ProductTableData.updateProductQuantity(quantity, product.ID);
                this.Close();
            }
            else
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Invalid value. Pres ok to exit.", "Invalid", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }
    }
}
