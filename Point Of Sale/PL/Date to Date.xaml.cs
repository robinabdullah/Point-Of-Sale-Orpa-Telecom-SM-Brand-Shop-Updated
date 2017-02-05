using System;
using System.Collections.Generic;
using System.IO;
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
using Point_Of_Sale.DAL;
using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.Win32;

namespace Point_Of_Sale.PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Date_to_Date : Window
    {
        public Date_to_Date()
        {
            InitializeComponent();
            toDatee.Text = DateTime.Now.ToString();
            fromDatee.Text = DateTime.Now.ToString();
            Reports.preview = false; // reset print preview of report
        }
                
        private void printPreviewButton_Click(object sender, RoutedEventArgs e)
        {
            Reports.fromDate = DateTime.Parse(fromDatee.Text);
            Reports.toDate = DateTime.Parse(toDatee.Text);
            
            Reports.preview = true;
            this.Close();
        }
               

        private void close_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void thisMonthCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            sevenDaysCheckBox.IsChecked = false;
            ThirtydaysCheckBox.IsChecked = false;
            fromDatee.SelectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            //toDatee.SelectedDate = DateTime.Now;
        }

        private void sevenDaysCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            thisMonthCheckBox.IsChecked = false;
            ThirtydaysCheckBox.IsChecked = false;
            fromDatee.SelectedDate = DateTime.Now.AddDays(-7); 
            //toDatee.SelectedDate = DateTime.Now;
        }

        private void ThirtydaysCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            thisMonthCheckBox.IsChecked = false;
            sevenDaysCheckBox.IsChecked = false;
            fromDatee.SelectedDate = DateTime.Now.AddDays(-30);
        }
    }
}
