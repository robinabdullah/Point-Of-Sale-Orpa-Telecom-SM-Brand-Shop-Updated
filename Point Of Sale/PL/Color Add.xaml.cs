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
    public partial class Color_Add : Window
    {
        private ComboBox color;

        public Color_Add()
        {
            InitializeComponent();
        }

        public Color_Add(ComboBox color): this()
        {
            this.color = color;
        }

        private void ok_Click(object sender, RoutedEventArgs e)
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
    }
}
